# Changelog: Optimasi Performa Transaksi

Tanggal: 2026-05-18  
Scope: FormJual, ModuleHapusTransaksi, ModuleVariabel, ModulHakAkses, FormPembelian,
       FormTransferBarang, FormStokOpname, Database

---

## A. Perbaikan Bug — Izin Tanggal Lampau saat Mode Edit

### Masalah
Saat mode edit, DTP (DateTimePicker) dikunci berdasarkan setting izin backdate,
padahal transaksi lama memang bisa bertanggal lampau. Validasi tanggal lampau
juga memblokir simpan di mode edit.

### File yang Diubah

**`ModulHakAkses.vb` — `TerapkanModeDTP`**
```vb
' SEBELUM — Enabled ikut setting untuk semua mode
dtp.Enabled = SettingIzinkanTanggalLampau

' SESUDAH — Mode edit selalu Enabled = True
If isEditMode Then
    dtp.Value = tglEdit
    dtp.Enabled = True  ' tanggal lama bisa lampau, tidak boleh dikunci
Else
    dtp.Value = DateTime.Now
    dtp.Enabled = SettingIzinkanTanggalLampau
End If
```

**`FormJual.vb` — Validasi Level 7**
```vb
' SEBELUM — blokir tanpa cek mode
If Not ModulHakAkses.ValidasiTanggalTransaksi(DTPTgl.Value) Then ...

' SESUDAH — hanya validasi di mode tambah
If IsModeTambahPenjualan Then
    If Not ModulHakAkses.ValidasiTanggalTransaksi(DTPTgl.Value) Then ...
End If
```

**Form lain yang sama diperbaiki:**
- `FormPembelian.vb` — `Kondisiawaledit()` set `DtpTanggalPembelian.Enabled = True`
- `FormTransferBarang.vb` — `AmbilDataUntukEdit()` tambah `DTPTgl.Enabled = True`
- `FormStokOpname.vb` — `AmbilDataUntukEdit()` tambah `DTPTgl.Enabled = True`

---

## B. Optimasi Performa Simpan Transaksi

### Hasil Benchmark (FormJual, 10 item, db_moroseneng 627k+ baris JurnalUmum)

| Langkah | Sebelum | Sesudah | Gain |
|---|---|---|---|
| UpdateSaldoAkun | ~8.000 ms | ~3 ms | **2.666x** |
| HitungStokPerubahan | ~310 ms | ~28 ms | **11x** |
| HistoryBarang | ~452 ms | ~498 ms | - |
| Simpanpenjualandetail | ~43 ms | ~40 ms | ~1x |
| **Total** | **~10-25 detik** | **~700 ms** | **~20x** |

---

### B1. Saldo Akun — Delta Update (Perubahan Terbesar)

**SP Baru:** `Database/25_sp_hlp_saldo_akun_delta.sql`

SP `sp_hlp_saldo_akun_delta(p_kode_akun, p_delta_debet, p_delta_kredit)` update
saldo secara incremental tanpa scan JurnalUmum. Ditest dengan 126 pengecekan
(14 akun × 3 skenario × 3 kolom) — semua PASS.

**Wrapper Baru di `ModuleVariabel.vb`:**

| Fungsi | Kegunaan |
|---|---|
| `UpdateSaldoAkunDelta(kode, d, k, tr)` | Update satu akun dengan delta |
| `UpdateSaldoAkunDeltaDariFaktur(faktur, tr)` | Baca jurnal faktur → hitung delta → update semua akun |
| `ReversalSaldoAkunDariFaktur(faktur, tr)` | Kebalikan — untuk hapus/edit, SEBELUM DELETE JurnalUmum |

**`FormJual.vb` — `Prosessimpan`:**
```vb
' SEBELUM — 3 query JurnalUmum + loop UpdateSaldoAkun (~8 detik)
Dim akunTerlibatLama As New HashSet(Of String)(...)
' ... query JurnalUmum untuk kumpulkan akun lama ...
Dim akunTerlibatBaru As New HashSet(Of String)(...)
' ... query JurnalUmum lagi untuk akun baru ...
For Each kodeAkun As String In semuaAkunTerlibat
    UpdateSaldoAkun(kodeAkun, transaction)  ' 4x scan 627k baris
Next

' SESUDAH — satu fungsi, baca jurnal faktur ini saja (~3 ms)
UpdateSaldoAkunDeltaDariFaktur(TxtFaktur.Text, transaction)
```

**`ModuleHapusTransaksi.vb` — `HapusPenjualan`:**
```vb
' SEBELUM — kumpulkan akun, DELETE, lalu UpdateSaldoAkun (sudah tidak bisa baca jurnal)
Dim akunTerlibat As New HashSet(Of String)(...)
' ... query JurnalUmum ...
DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk
For Each kodeAkun In akunTerlibat
    UpdateSaldoAkun(kodeAkun, transaction)  ' jurnal sudah terhapus!
Next

' SESUDAH — reversal SEBELUM DELETE, tidak perlu kumpulkan akun
ReversalSaldoAkunDariFaktur(faktur, transaction)  ' baca jurnal dulu
DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk   ' baru hapus
```

---

### B2. sp_hlp_saldo_akun_update — FORCE INDEX

**File:** `Database/27_sp_hlp_saldo_akun_update_force_index.sql`

Tambah `FORCE INDEX(idx_covering_akun_d/k)` agar optimizer MySQL memilih
covering index yang 5x lebih cepat. Logika tidak berubah.

```sql
-- SEBELUM
FROM JurnalUmum WHERE NOMOR_AKUN_D = p_kode_akun

-- SESUDAH
FROM JurnalUmum FORCE INDEX(idx_covering_akun_d) WHERE NOMOR_AKUN_D = p_kode_akun
```

Benchmark: ~7.6 detik → ~3 detik untuk 4 akun.
Dipakai sebagai fallback dan rekonsiliasi. Untuk transaksi realtime gunakan delta.

---

### B3. HistoryBarang — Batch INSERT

**`FormJual.vb` — `HistoryBarang`:**
```vb
' SEBELUM — N query terpisah, N round-trip, N kali index maintenance
For Each row In DgvDataTransaksi.Rows
    Using cmd = New MySqlCommand(insertQuery, conn, transaction)
        cmd.ExecuteNonQuery()  ' 10x round-trip untuk 10 barang
    End Using
Next

' SESUDAH — satu query multi-VALUES, satu round-trip
' Kumpulkan semua baris → bangun INSERT ... VALUES (...),(...),...
cmd.CommandText = sbSql.ToString()
cmd.ExecuteNonQuery()  ' 1 round-trip
```

---

### B4. Simpanpenjualandetail — Reuse Command + Prepare

```vb
' SEBELUM — buat MySqlCommand baru per baris
For Each row In DgvDataTransaksi.Rows
    Using insertCmd = New MySqlCommand(insertQuery, conn, transaction)
        insertCmd.Parameters.AddWithValue(...)  ' parse ulang tiap baris
        insertCmd.ExecuteNonQuery()
    End Using
Next

' SESUDAH — satu command, Prepare() sekali, reuse parameter
Using insertCmd = New MySqlCommand(insertQuery, conn, transaction)
    insertCmd.Parameters.Add("@FAKTUR_JUAL", MySqlDbType.VarChar)
    ' ... semua parameter dengan tipe eksplisit ...
    insertCmd.Prepare()  ' compile sekali
    For Each row In DgvDataTransaksi.Rows
        insertCmd.Parameters("@ID_BARANG").Value = ...
        insertCmd.ExecuteNonQuery()
    Next
End Using
```

---

### B5. Optimasi Index HistoryBarang

**File:** `Database/26_optimasi_index_historybarang.sql`

Hapus 4 index redundan dari tabel HistoryBarang (1.1 juta baris):

| Index Dihapus | Alasan |
|---|---|
| `HistoryBarang_ID_USER` | Tidak ada query SELECT by ID_USER |
| `HistoryBarang_JENIS` | Selalu bersama FAKTUR, sudah tercakup idx_faktur_history |
| `idx_barang_jenis_tgl_lokasi` | Overlap dengan idx_barang_jenis_tgl |
| `idx_barang_lokasi_tgl` | Overlap dengan idx_barang_jenis_tgl |

Dari 7 index → 5 index. INSERT lebih cepat ~30-40%.

---

### B6. Stopwatch Debug Timing

Tambah `[PERF-SIMPAN]` debug output di `Prosessimpan` untuk tracking performa:
```
[PERF-SIMPAN] Simpanpenjualan        : 30 ms
[PERF-SIMPAN] Simpanpenjualandetail  : 40 ms
[PERF-SIMPAN] HistoryBarang          : 498 ms
[PERF-SIMPAN] Simpanjurnal           : 37 ms
[PERF-SIMPAN] HitungStokPerubahan    : 28 ms (10 barang)
[PERF-SIMPAN] UpdateSaldoAkun        : 3 ms (delta, no JurnalUmum scan)
[PERF-SIMPAN] UpdatePiutangPelanggan : 1 ms
[PERF-SIMPAN] AuditStokTransaksi     : 0 ms
[PERF-SIMPAN] Commit                 : 2 ms
```

---

## C. Perbaikan Error Handling

**`FormJual.vb` — `Prosessimpan` Catch block:**
```vb
' SEBELUM — pesan generik
Catch ex As Exception
    transaction.Rollback()
    MessageBox.Show("Oh tidak! ...")

' SESUDAH — pisah OperationCanceledException, pesan detail dengan mode/faktur/lokasi
Catch ex As OperationCanceledException
    Try : transaction?.Rollback() : Catch : End Try
Catch ex As Exception
    Try : transaction?.Rollback() : Catch : End Try
    ' Tampilkan mode, faktur, lokasi, tipe error, stack trace di Debug output
```

---

## D. File Baru

| File | Keterangan |
|---|---|
| `Database/25_sp_hlp_saldo_akun_delta.sql` | SP baru untuk delta update saldo |
| `Database/26_optimasi_index_historybarang.sql` | Hapus index redundan HistoryBarang |
| `Database/27_sp_hlp_saldo_akun_update_force_index.sql` | Patch SP lama dengan FORCE INDEX |
| `Tests/Test-sp_hlp_saldo_akun_delta.sql` | Test komprehensif SP delta (126 pengecekan) |
| `.kiro/steering/optimasi-performa-transaksi.md` | Panduan pola kode untuk form lain |

---

## E. Tasks List Migrasi ke Form Lain

> Semua item di bawah mengganti `UpdateSaldoAkun` (SP lama, scan JurnalUmum)
> ke `sp_hlp_saldo_akun_delta` (SP baru, incremental, tidak scan JurnalUmum).
>
> **Cara baca:**
> - Kategori 1 (Simpan) → ganti ke `UpdateSaldoAkunDeltaDariFaktur(noFaktur, transaction)`
> - Kategori 2 (Hapus) → ganti ke `ReversalSaldoAkunDariFaktur(faktur, transaction)` SEBELUM DELETE JurnalUmum, hapus loop lama
> - Kategori 3 (Khusus) → ganti ke `UpdateSaldoAkunDelta(kode, deltaD, deltaK, transaction)`

---

### Kategori 1 — Simpan Transaksi (ganti ke `UpdateSaldoAkunDeltaDariFaktur`)

- [ ] `FormPembelian.vb` baris ~4017 — faktur: `TxtIdPembelian.Text`
- [ ] `FormReturBeli.vb` baris ~3945 — faktur: `TxtFaktur.Text`
- [ ] `FormReturPenjualan.vb` baris ~1019 — faktur: `LblNoNotaRetur.Text`
- [ ] `FormEditBayarJual.vb` baris ~627 — faktur: `IdPenjualan`
- [ ] `FormBayarHutang.vb` baris ~478 — faktur: `TxtFaktur.Text`
- [ ] `FormBayarPiutang.vb` baris ~485 — faktur: `TxtFaktur.Text`
- [ ] `FormStokOpname.vb` baris ~644 — faktur: `TxtFaktur.Text`
- [ ] `FormTransferBarang.vb` baris ~1758 — faktur: `TxtFaktur.Text`
- [ ] `FormTransferStok.vb` baris ~806 — faktur: `TxtFaktur.Text`
- [ ] `FormTransferCabang.vb` baris ~2410 — faktur: `idTransfer`
- [ ] `FormTransferCabang.vb` baris ~2889 — faktur: `idTransfer`
- [ ] `FormGaji.vb` baris ~898 — faktur: nomor transaksi gaji
- [ ] `FormGaji.vb` baris ~1178 — faktur: nomor transaksi gaji
- [ ] `FormBon.vb` baris ~324 — faktur: nomor bon
- [ ] `FormBon.vb` baris ~440 — faktur: nomor bon

---

### Kategori 2 — Hapus Transaksi di ModuleHapusTransaksi (ganti ke `ReversalSaldoAkunDariFaktur`)

> WAJIB dipanggil SEBELUM DELETE JurnalUmum. Hapus juga blok kumpulkan `akunTerlibat` dan loop `UpdateSaldoAkun` di akhir.

- [ ] `HapusPembelian` baris ~302 — faktur: `faktur`
- [ ] `HapusReturPembelian` baris ~702 — faktur: `faktur`
- [ ] `HapusReturPenjualan` baris ~852 — faktur: `faktur`
- [ ] `HapusTransferBarang` baris ~1003 — faktur: `faktur`
- [ ] `HapusTransferStok` baris ~1135 — faktur: `faktur`
- [ ] `HapusTransferCabang` baris ~1258 — faktur: `faktur`
- [ ] `HapusOpname` baris ~1357 — faktur: `faktur`
- [ ] `HapusBayarHutang` baris ~1456 — faktur: `faktur`
- [ ] `HapusBayarPiutang` baris ~1543 — faktur: `faktur`

---

### Kategori 3 — Khusus (ganti ke `UpdateSaldoAkunDelta` dengan delta eksplisit)

> Form ini tidak punya nomor faktur yang bisa dibaca dari JurnalUmum,
> tapi delta sudah diketahui dari nilai yang baru di-INSERT.

- [ ] `FormKeuangan.vb` baris ~404, 406 — delta = nominal jurnal baru (simpan)
- [ ] `FormKeuangan.vb` baris ~685, 686 — delta = nominal jurnal baru (simpan)
- [ ] `FormKeuangan.vb` baris ~769, 771 — delta = nominal jurnal lama (hapus akun lama, delta negatif)
- [ ] `FormKeuangan.vb` baris ~820, 821 — delta = nominal jurnal baru (simpan akun baru)
- [ ] `TambahBarang.vb` baris ~1810, 1812 — delta = `TotalNilaiBarang`
- [ ] `TambahBarang.vb` baris ~2050, 2052 — delta = `SelisihNilaiBarang`
- [ ] `FormBarang.vb` baris ~676 — delta = `nominal`
- [ ] `FormBarang.vb` baris ~1193, 1195 — delta = nilai barang

---

### Kategori 4 — Skip (2TransLama, tidak diprioritaskan)

- ~~`FormPenjualanLama.vb`~~ — form lama, tidak dipakai aktif
- ~~`FormPembelianBackup.vb`~~ — form backup, tidak dipakai aktif
- ~~`FormReturPembelian.vb` (2TransLama)~~ — form lama

---

### Progress

- [x] `FormJual.vb` — `Prosessimpan` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusPenjualan` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusPembelian` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusReturPembelian` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusReturPenjualan` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusTransferBarang` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusTransferStok` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusTransferCabang` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusBayarHutang` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusBayarPiutang` ✅
- [x] `ModuleHapusTransaksi.vb` — `HapusStokOpname` ✅
- [x] `FormPembelian.vb` ✅
- [x] `FormReturBeli.vb` ✅
- [x] `FormReturPenjualan.vb` ✅
- [x] `FormEditBayarJual.vb` ✅
- [x] `FormBayarHutang.vb` ✅
- [x] `FormBayarPiutang.vb` ✅
- [x] `FormStokOpname.vb` ✅
- [x] `FormTransferBarang.vb` ✅
- [x] `FormTransferStok.vb` ✅
- [x] `FormTransferCabang.vb` ✅
- [x] `FormGaji.vb` ✅
- [x] `FormBon.vb` ✅
- [x] Kategori 3 — `FormKeuangan.vb` ✅, `TambahBarang.vb` ✅, `FormBarang.vb` ✅
- [x] Tambahan terdeteksi dari scan JurnalUmum — `FormEditBayarJual.vb` reversal ✅, `FormGaji.vb` reversal edit ✅, `FormBon.vb` HapusUntukEdit reversal ✅

