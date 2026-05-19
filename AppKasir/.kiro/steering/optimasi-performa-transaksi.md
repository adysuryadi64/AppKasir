# Optimasi Performa Transaksi

Dokumen ini mencatat semua optimasi yang sudah diterapkan dan pola yang WAJIB diikuti
saat menulis kode baru yang menyentuh transaksi, jurnal, stok, dan saldo akun.

---

## 1. Saldo Akun — Gunakan Delta, Bukan Recalculate

### Masalah Lama
`UpdateSaldoAkun()` memanggil `sp_hlp_saldo_akun_update` yang melakukan `SUM(NOMINAL)`
dari seluruh `JurnalUmum` (600k+ baris) per akun. Untuk 4 akun = 4x full scan = **~8 detik**.

### Solusi: sp_hlp_saldo_akun_delta (Database/25_sp_hlp_saldo_akun_delta.sql)
SP baru yang update saldo secara **incremental** tanpa scan JurnalUmum.

```sql
CALL sp_hlp_saldo_akun_delta(p_kode_akun, p_delta_debet, p_delta_kredit)
```

- `p_delta_debet`  : nominal yang masuk ke sisi DEBET (negatif = reversal)
- `p_delta_kredit` : nominal yang masuk ke sisi KREDIT (negatif = reversal)

**Hasil: ~3 detik → 3 ms (1000x lebih cepat)**

### Wrapper VB.NET (ModuleVariabel.vb)

```vb
' Untuk simpan transaksi baru — baca jurnal faktur, hitung delta, update semua akun
UpdateSaldoAkunDeltaDariFaktur(noFaktur, transaction)

' Untuk hapus/reversal — WAJIB dipanggil SEBELUM DELETE JurnalUmum
ReversalSaldoAkunDariFaktur(noFaktur, transaction)

' Untuk update satu akun dengan delta yang sudah diketahui
UpdateSaldoAkunDelta(kodeAkun, deltaDebet, deltaKredit, transaction)
```

### Aturan Penggunaan

**WAJIB** pakai `UpdateSaldoAkunDeltaDariFaktur` setelah INSERT JurnalUmum, bukan loop `UpdateSaldoAkun`.

**WAJIB** pakai `ReversalSaldoAkunDariFaktur` SEBELUM DELETE JurnalUmum (bukan sesudah).

**JANGAN** pakai loop `For Each kodeAkun In akunTerlibat: UpdateSaldoAkun(...)` untuk transaksi baru.
`UpdateSaldoAkun` masih boleh dipakai untuk rekonsiliasi/fallback saja.

### Contoh Pola Simpan (FormJual, FormPembelian, dll)

```vb
' ✅ BENAR — setelah Simpanjurnal INSERT ke JurnalUmum
Simpanjurnal(transaction, jD, jK)
UpdateSaldoAkunDeltaDariFaktur(TxtFaktur.Text, transaction)

' ❌ SALAH — jangan pakai ini untuk transaksi baru
For Each kodeAkun As String In semuaAkunTerlibat
    UpdateSaldoAkun(kodeAkun, transaction)
Next
```

### Contoh Pola Hapus (ModuleHapusTransaksi)

```vb
' ✅ BENAR — reversal SEBELUM DELETE JurnalUmum
ReversalSaldoAkunDariFaktur(faktur, transaction)
' ... lalu DELETE JurnalUmum ...
"DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk"

' ❌ SALAH — UpdateSaldoAkun setelah DELETE tidak bisa baca jurnal lagi
For Each kodeAkun In akunTerlibat
    UpdateSaldoAkun(kodeAkun, transaction)
Next
```

### Keamanan Data
SP delta bisa drift jika ada bug di pemanggil. Mitigasi:
- `sp_bat_saldo_semua_akun` dijalankan saat startup (FormLoading) untuk rekonsiliasi penuh
- `PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()` untuk posting resmi

---

## 2. sp_hlp_saldo_akun_update — FORCE INDEX

File: `Database/27_sp_hlp_saldo_akun_update_force_index.sql`

SP ini diupdate dengan `FORCE INDEX(idx_covering_akun_d/k)` agar optimizer MySQL
memilih covering index yang 5x lebih cepat. Logika tidak berubah.

Dipakai sebagai fallback dan rekonsiliasi. Untuk transaksi realtime gunakan delta.

---

## 3. INSERT Batch ke HistoryBarang

### Masalah Lama
INSERT per baris dalam loop = N round-trip ke DB + N kali index maintenance.

### Solusi
Kumpulkan semua baris dulu, lalu INSERT satu query multi-VALUES:

```vb
' ✅ BENAR — satu query untuk semua baris
Dim sbSql As New StringBuilder("INSERT INTO HistoryBarang (...) VALUES ")
For i = 0 To listIdBarang.Count - 1
    sbSql.Append($"(@F{i},@T{i},...),")
    cmd.Parameters.AddWithValue($"@F{i}", faktur)
    ' ...
Next
cmd.CommandText = sbSql.ToString().TrimEnd(",")
cmd.ExecuteNonQuery()

' ❌ SALAH — jangan INSERT per baris dalam loop
For Each row In DgvDataTransaksi.Rows
    Using cmd = New MySqlCommand(insertQuery, conn, transaction)
        cmd.ExecuteNonQuery()  ' N kali round-trip
    End Using
Next
```

---

## 4. Optimasi Index HistoryBarang

File: `Database/26_optimasi_index_historybarang.sql`

Index yang dihapus karena redundan/tidak dipakai:
- `HistoryBarang_ID_USER` — tidak ada query SELECT by ID_USER
- `HistoryBarang_JENIS` — selalu bersama FAKTUR, sudah tercakup idx_faktur_history
- `idx_barang_jenis_tgl_lokasi` — overlap dengan idx_barang_jenis_tgl
- `idx_barang_lokasi_tgl` — overlap dengan idx_barang_jenis_tgl

Index yang dipertahankan (5 index):
- `PRIMARY`
- `uq_sync_id_historybarang` — sync multi-cabang
- `HistoryBarang_TANGGAL` — FormLapStokLampau WHERE TANGGAL <= @tgl
- `idx_faktur_history` — DELETE/SELECT WHERE FAKTUR = ?
- `idx_lokasi_jenis_barang_qty` — batch recalculate stok
- `idx_barang_jenis_tgl` — SELECT WHERE ID_BARANG + JENIS/TANGGAL

---

## 5. Pola Simpanpenjualandetail — Reuse Command

Buat `MySqlCommand` sekali di luar loop, gunakan `Prepare()`, reuse parameter per baris.
Berlaku juga untuk `Simpanpenjualandetail` yang menggabungkan INSERT detail + UPDATE stok counter.

```vb
' ✅ BENAR
Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
    insertCmd.Parameters.Add("@FAKTUR_JUAL", MySqlDbType.VarChar)
    ' ... tambah semua parameter ...
    insertCmd.Parameters("@FAKTUR_JUAL").Value = TxtFaktur.Text
    insertCmd.Prepare()  ' compile sekali

    For Each row In DgvDataTransaksi.Rows
        insertCmd.Parameters("@ID_BARANG").Value = row.Cells(0).Value
        ' ... set parameter per baris ...
        insertCmd.ExecuteNonQuery()
    Next
End Using
```

---

## 6. Ringkasan Performa (FormJual, 10 item, db_moroseneng 600k+ baris)

| Langkah | Sebelum | Sesudah |
|---|---|---|
| UpdateSaldoAkun | ~8.000 ms | ~3 ms |
| HitungStokPerubahan | ~310 ms | ~28 ms |
| HistoryBarang | ~452 ms | ~498 ms* |
| Simpanpenjualandetail | ~43 ms | ~40 ms |
| Total proses | ~10-25 detik | ~700 ms |

*HistoryBarang masih ~500ms karena batas fisik InnoDB (1.1 juta baris, unique index sync_id).
Batch INSERT sudah diterapkan. Tidak bisa lebih cepat tanpa partisi tabel.
