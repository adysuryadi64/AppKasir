# PLAN: Saldo Akun Realtime + Refactor History

---

## KONDISI SAAT INI

### Masalah Utama: Saldo Akun Tidak Realtime

`tbl_datareferensi.Saldo_Akhir` hanya diupdate saat:
- User membuka `FormLapNeracaLR` dan klik tombol laporan (memanggil `HITUNGSEMUASALDO`, `HITUNGSALDOAWAL`, `HITUNGSALDOAKHIR`)
- Proses Posting di `FormLoading.MulaiPosting()` (memanggil `FormLapNeracaLR.HITUNGSEMUASALDO()`)

Artinya: setelah transaksi penjualan, pembelian, bayar hutang, dll — saldo akun di `tbl_datareferensi` **tidak langsung berubah** sampai user membuka laporan neraca.

### Fungsi SaldoAkunTambah / SaldoAkunKurang

Sudah ada di `ModuleVariabel.vb` tapi **tidak dipanggil di mana pun** (orphan functions). Ini kemungkinan dibuat tapi tidak pernah diintegrasikan.

### Struktur JurnalUmum — Pola Split

Hampir semua transaksi menggunakan **pola split** — satu transaksi ditulis dalam beberapa baris terpisah, di mana setiap baris hanya berisi **satu sisi** (D saja atau K saja) karena nilai antar akun berbeda dan tidak bisa digabung:

- Baris hanya D: `NOMOR_AKUN_D` diisi, `NOMOR_AKUN_K` kosong/NULL
- Baris hanya K: `NOMOR_AKUN_K` diisi, `NOMOR_AKUN_D` kosong/NULL
- Baris D+K: keduanya diisi hanya jika nilai D = nilai K (jarang)

Contoh nyata FormPenjualan — satu transaksi bisa menghasilkan hingga **9 baris jurnal**:

| Baris | Sisi | Akun | Nilai |
|-------|------|------|-------|
| J1 | D only | Kas Tunai | nilai tunai diterima |
| J2 | D only | Kas/Bank Transfer | nilai transfer |
| J3 | D only | Piutang Jual | sisa hutang |
| J4 | D only | Beban Diskon | diskon item |
| J5 | D only | Beban Diskon | diskon total |
| J6 | K only | Persediaan Barang (HPP) | nilai HPP |
| J7 | K only | Hutang Pajak | nilai pajak |
| J8 | K only | Laba Kotor Penjualan | laba kotor |
| J9 | K only | Pendapatan Lain-lain | biaya kirim |

FormPembelian, FormGaji, FormReturPenjualan, dan hampir semua form lain mengikuti pola yang sama. FormPenjualan bahkan sudah memiliki `totalDebet`/`totalKredit` accumulator dan debug output untuk memverifikasi keseimbangan.

Implikasi untuk `UpdateSaldoSemuaAkun`: query recalculate menangani pola split dengan benar karena SUM dihitung terpisah per sisi — MySQL mengabaikan NULL di SUM secara otomatis:
```sql
SUM(NOMINAL) WHERE NOMOR_AKUN_D = kode  -- hanya baris yang punya sisi D
SUM(NOMINAL) WHERE NOMOR_AKUN_K = kode  -- hanya baris yang punya sisi K
```

Saldo akun yang benar = `Saldo_Awal + SUM(semua baris D) - SUM(semua baris K)`

### Kolom tbl_datareferensi yang relevan
- `Kode_akun` — PK
- `Saldo_Akhir` — saldo running (yang ingin dibuat realtime)
- `SALDO_SEBELUMNYA` — dipakai untuk laporan neraca periode
- `S_DEBET`, `S_KREDIT` — dipakai untuk laporan neraca periode

---

## ANALISIS KOMPLEKSITAS

### Kenapa Kompleks

1. **Satu transaksi = 1-3 baris jurnal** — FormPenjualan bisa tulis 2 jurnal, FormGaji tulis 3 jurnal, semuanya dalam 1 transaction
2. **Hapus transaksi = harus rollback saldo** — saat hapus gaji/penjualan/pembelian, semua jurnal terkait dihapus, saldo harus dikurangi kembali
3. **Edit transaksi = hapus lama + simpan baru** — pola ini ada di hampir semua form
4. **Saldo akun bersifat kumulatif** — tidak bisa reset per transaksi, harus akumulasi dari semua transaksi
5. **Dua pendekatan berbeda:**
   - **Incremental** (SaldoAkunTambah/Kurang): update +/- per transaksi → cepat tapi rawan drift jika ada bug
   - **Recalculate** (seperti UpdateBonKaryawan): hitung ulang dari JurnalUmum → lambat tapi selalu akurat

### Form yang Menulis JurnalUmum (lengkap)

| # | Form / File | Jumlah Baris Jurnal | Pola | Akun Debet | Akun Kredit | Kondisi |
|---|-------------|-------------------|------|-----------|------------|---------|
| 1 | FormPenjualan | hingga 9 | Split D-only & K-only | Kas/Bank tunai, Kas/Bank transfer, Piutang, Beban Diskon (×2) | HPP/Persediaan, Hutang Pajak, Laba Kotor, Pendapatan Lain | Baris muncul sesuai kondisi (tunai/transfer/piutang/diskon/pajak/kirim) |
| 2 | FormPembelian | 1 atau 2 | D+K dalam 1 baris | `KODE_REK_BARANG` | Kas/Bank (J1 lunas/DP) atau `Kode_rek_Hutang_Beli` (J2 hutang) | J1 jika ada bayar, J2 jika ada sisa hutang |
| 3 | FormBayarHutang | 1 per baris DGV | D+K dalam 1 baris | `Kode_rek_Hutang_Beli` | Kas/Bank | 1 baris per faktur hutang yang dibayar |
| 4 | FormBayarPiutang | 1 per baris DGV | D+K dalam 1 baris | Kas/Bank | `Kode_rek_Piutang_Jual` | 1 baris per faktur piutang yang dibayar |
| 5 | FormReturPenjualan | 3 | Split K-only, D-only, D-only | `KODE_REK_BARANG` (HPP), `06.01.001` (Laba Kotor) | Kas/Bank atau Piutang | Selalu 3 baris |
| 6 | FormReturPembelian | 1 | D+K dalam 1 baris | Kas/Bank atau `Kode_rek_Hutang_Beli` | `KODE_REK_BARANG` | Selalu |
| 7 | FormReturBeli | 1 | D+K dalam 1 baris | Kas/Bank (`TxtKodeRek`) | `KODE_REK_BARANG` | Selalu |
| 8 | FormGaji | 1–3 | D+K dalam 1 baris | `07.01.001` (Beban Gaji) | Kas/Bank, `08.01.002` (Pot Lain), `01.03.002` (Pot Bon) | J1 selalu; J2 jika `potonganlain <> 0`; J3 jika `potonganBon <> 0` |
| 9 | FormBon | 1 | D+K dalam 1 baris | `01.03.002` (BON) atau Kas/Bank (BAYAR) | Kas/Bank (BON) atau `01.03.002` (BAYAR) | Selalu |
| 10 | FormKeuangan | 1 | D+K dalam 1 baris | Akun dinamis pilihan user | Akun dinamis pilihan user | Selalu |
| 11 | FormStokOpname | 0 atau 1 | D+K dalam 1 baris | `KODE_REK_BARANG` atau `LAWAN_KODE_REK_BARANG` | `LAWAN_KODE_REK_BARANG` atau `KODE_REK_BARANG` | Hanya jika `nilaiSelisih <> 0` |
| 12 | FormTransferStok | 1 | D+K dalam 1 baris | `KODE_REK_BARANG` atau `LAWAN_KODE_REK_BARANG` | `LAWAN_KODE_REK_BARANG` atau `KODE_REK_BARANG` | Selalu (arah tergantung selisih nilai) |
| 13 | FormTransferBarang | 1 | D+K dalam 1 baris | `KODE_REK_BARANG` | `KODE_REK_BARANG` | Selalu (D = K, jurnal internal) |
| 14 | TambahBarang (Tambah) | 0 atau 1 | D+K dalam 1 baris | `KODE_REK_BARANG` | `LAWAN_KODE_REK_BARANG` | Hanya jika `TotalNilaiBarang <> 0` |
| 15 | TambahBarang (Edit) | 0 atau 1 | D+K dalam 1 baris | `KODE_REK_BARANG` atau `LAWAN_KODE_REK_BARANG` | `LAWAN_KODE_REK_BARANG` atau `KODE_REK_BARANG` | Hanya jika `SelisihNilaiBarang <> 0` |
| 16 | FormBarang (Hapus) | 0 atau 1 | D+K dalam 1 baris | `LAWAN_KODE_REK_BARANG` | `KODE_REK_BARANG` | Hanya jika `nominal <> 0` |
| 17 | FormBarang (Tambah/Kurang Stok) | 1 | D+K dalam 1 baris | `KODE_REK_BARANG` atau `LAWAN_KODE_REK_BARANG` | `LAWAN_KODE_REK_BARANG` atau `KODE_REK_BARANG` | Selalu |
| 18 | FormUtama — `JurnalEksekusiTransaksi` | 1 | D+K dalam 1 baris | `LAWAN_KODE_REK_BARANG` | `KODE_REK_BARANG` | Saat hapus transaksi yang mempengaruhi stok |
| 19 | TambahSupliyer (Insert/Edit) ✅ | 0 atau 1 | D+K dalam 1 baris | `MODAL` (04.01.001) | `TAGIHAN / SALDO PIUTANG` (01.04.002) | Hanya jika `hutangAwal <> 0` atau berubah |
| 20 | TambahPelanggan (Insert/Edit) ✅ | 0 atau 1 | D+K dalam 1 baris | `HUTANG BELANJA` (03.01.001) | `MODAL` (04.01.001) | Hanya jika `hutangAwal <> 0` atau berubah |

**Form yang MENGHAPUS JurnalUmum (di FormUtama) — juga perlu UpdateSaldoSemuaAkun setelah commit:**

| Fungsi di FormUtama | Transaksi yang Dihapus | Akun Terdampak |
|---------------------|----------------------|----------------|
| `Hapuspembelian()` | DELETE JurnalUmum + stok | `KODE_REK_BARANG`, `Kode_rek_Hutang_Beli`, kas/bank |
| `Hapuspenjualan()` | DELETE JurnalUmum + stok | Penjualan, `KODE_REK_BARANG`, `Kode_rek_Piutang_Jual`, kas/bank |
| `Hapusreturpembelian()` | DELETE JurnalUmum + stok | `Kode_rek_Hutang_Beli`, `KODE_REK_BARANG` |
| `Hapusreturpenjualan()` | DELETE JurnalUmum + stok | `KODE_REK_BARANG`, `Kode_rek_Piutang_Jual`, kas/bank |
| `Hapusbayarhutang()` | DELETE JurnalUmum | `Kode_rek_Hutang_Beli`, kas/bank |
| `HapusbayarPiutang()` | DELETE JurnalUmum | `Kode_rek_Piutang_Jual`, kas/bank |
| `Hapusstokopname()` | DELETE JurnalUmum + stok | `KODE_REK_BARANG`, `LAWAN_KODE_REK_BARANG` |
| `Hapustransferstok()` | DELETE JurnalUmum + stok | `KODE_REK_BARANG`, `LAWAN_KODE_REK_BARANG` |
| `Hapustransferbarng()` | DELETE JurnalUmum + stok | `KODE_REK_BARANG` |

Karena hapus di FormUtama tidak tahu akun mana yang terdampak (data sudah dihapus), strategi terbaik adalah memanggil `UpdateSaldoSemuaAkun()` (recalculate semua) setelah commit — bukan `UpdateSaldoAkun` per akun.

**Catatan khusus:** tidak ada.

---

## PLAN IMPLEMENTASI

### Pendekatan yang Dipilih: Recalculate per Akun (bukan Incremental)

**Alasan:**
- Konsisten dengan pola yang sudah ada (`UpdateBonKaryawan`, `UpdatePiutangPelanggan`, `UpdateHutangSupliyer`)
- Tidak rawan drift — selalu akurat karena dihitung dari sumber data (JurnalUmum)
- Lebih mudah di-debug
- Saldo akun tidak sebanyak karyawan/pelanggan/supplier, jadi performa masih oke

---

### BAGIAN 1: Fungsi Baru di ModuleVariabel.vb

#### 1.1 `UpdateSaldoAkun(kodeAkun, transaction)`
Recalculate `Saldo_Akhir` untuk **satu akun** dari JurnalUmum.

```vb
' Formula:
' Saldo_Akhir = Saldo_Awal + SUM(NOMINAL where NOMOR_AKUN_D = kode) 
'                           - SUM(NOMINAL where NOMOR_AKUN_K = kode)
'
' Catatan: ini asumsi semua akun bersifat DEBET NORMAL (aset, biaya)
' Untuk akun KREDIT NORMAL (hutang, modal, pendapatan) rumusnya terbalik
' Tapi karena Saldo_Akhir di sini adalah saldo RUNNING (bukan saldo normal akuntansi),
' kita pakai: Saldo_Awal + total_masuk_sebagai_debet - total_masuk_sebagai_kredit
```

Query:
```sql
UPDATE tbl_datareferensi r
LEFT JOIN (
    SELECT NOMOR_AKUN_D AS kode, SUM(NOMINAL) AS total
    FROM JurnalUmum WHERE NOMOR_AKUN_D = @Kode GROUP BY NOMOR_AKUN_D
) d ON d.kode = r.Kode_akun
LEFT JOIN (
    SELECT NOMOR_AKUN_K AS kode, SUM(NOMINAL) AS total
    FROM JurnalUmum WHERE NOMOR_AKUN_K = @Kode GROUP BY NOMOR_AKUN_K
) k ON k.kode = r.Kode_akun
SET r.Saldo_Akhir = IFNULL(r.Saldo_Awal, 0) 
                  + IFNULL(d.total, 0) 
                  - IFNULL(k.total, 0)
WHERE r.Kode_akun = @Kode
```

#### 1.2 `UpdateSaldoSemuaAkun()`
Recalculate `Saldo_Akhir` untuk **semua akun** sekaligus — dipakai saat posting/loading.

```sql
UPDATE tbl_datareferensi r
LEFT JOIN (
    SELECT NOMOR_AKUN_D, SUM(NOMINAL) AS total_debet
    FROM JurnalUmum GROUP BY NOMOR_AKUN_D
) d ON d.NOMOR_AKUN_D = r.Kode_akun
LEFT JOIN (
    SELECT NOMOR_AKUN_K, SUM(NOMINAL) AS total_kredit
    FROM JurnalUmum GROUP BY NOMOR_AKUN_K
) k ON k.NOMOR_AKUN_K = r.Kode_akun
SET r.Saldo_Akhir = IFNULL(r.Saldo_Awal, 0)
                  + IFNULL(d.total_debet, 0)
                  - IFNULL(k.total_kredit, 0)
```

---

### BAGIAN 2: Integrasi ke Setiap Form Transaksi

Pola yang sama dengan `UpdateBonKaryawan` — dipanggil **di dalam transaction yang sama**, setelah INSERT jurnal, sebelum COMMIT.

#### 2.1 FormPenjualan
Akun: `Kode_rek_Jual_Toko/Gudang`, `KODE_REK_BARANG`, `Kode_rek_Piutang_Jual`, rekening kas/bank

#### 2.2 FormPembelian
Akun: `KODE_REK_BARANG`, `Kode_rek_Hutang_Beli`, rekening kas/bank

#### 2.3 FormBayarHutang
Akun: `Kode_rek_Hutang_Beli`, rekening kas/bank (`TxtRekening.Text`)

#### 2.4 FormBayarPiutang
Akun: `Kode_rek_Piutang_Jual`, rekening kas/bank

#### 2.5 FormReturPenjualan
Akun: `KODE_REK_BARANG`, `06.01.001` (laba kotor), rekening kas/bank atau `Kode_rek_Piutang_Jual`

#### 2.6 FormReturPembelian
Akun: `Kode_rek_Hutang_Beli`, `KODE_REK_BARANG`

#### 2.7 FormReturBeli
Akun: rekening kas/bank (`TxtKodeRek`), `KODE_REK_BARANG`

#### 2.8 FormGaji
Akun: `Kode_rek_Gaji_Karyawan`, rekening kas/bank, `01.03.002`

#### 2.9 FormBon
Akun: `01.03.002`, rekening kas/bank

#### 2.10 FormKeuangan
Akun dinamis — ambil dari `NOMOR_AKUN_D` dan `NOMOR_AKUN_K` yang dipakai saat INSERT.
Perlu penyesuaian karena FormKeuangan pakai wrapper `ExecuteNonQuery()` bukan transaction langsung.

#### 2.11 FormStokOpname
Akun: `KODE_REK_BARANG`, `LAWAN_KODE_REK_BARANG` (hanya jika `nilaiSelisih <> 0`)

#### 2.12 FormTransferStok
Akun: `KODE_REK_BARANG`, `LAWAN_KODE_REK_BARANG`

#### 2.13 FormTransferBarang
Akun: `KODE_REK_BARANG` (D = K, saldo neto tidak berubah — UpdateSaldoAkun tetap dipanggil untuk konsistensi)

#### 2.14 TambahBarang (Tambah & Edit)
Akun: `KODE_REK_BARANG`, `LAWAN_KODE_REK_BARANG` (hanya jika nilai/selisih `<> 0`)

#### 2.15 FormBarang (Hapus & Tambah/Kurang Stok)
Akun: `KODE_REK_BARANG`, `LAWAN_KODE_REK_BARANG`

---

### BAGIAN 3: Integrasi ke FormLoading

Di `MulaiPosting()` dan `MulaiLoading()`, tambahkan `UpdateSaldoSemuaAkun()` setelah fungsi-fungsi update lainnya:

```vb
UpdateTotalBonDanTotalBayarKaryawan()
UpdatePiutangDibayar()
UpdateSupliyerFromPembelianHutangDibayar()
UpdateSaldoSemuaAkun()   ' <-- tambah ini
```

---

### BAGIAN 4: Hapus SaldoAkunTambah / SaldoAkunKurang

Setelah semua form diintegrasikan dengan `UpdateSaldoAkun`, fungsi incremental lama ini dihapus karena:
- Tidak pernah dipakai (orphan)
- Pendekatan incremental digantikan oleh recalculate

---

## PLAN REFACTOR: History → JurnalTidakSeimbang

### Kondisi Saat Ini

`CatatanAksiHistory` dipanggil dari **40+ tempat** di seluruh codebase untuk mencatat semua aksi (simpan barang, tambah merk, hapus satuan, simpan penjualan, dll). Ini membebani tabel `History` dengan data yang tidak penting.

### Rencana Baru

Ubah tujuan `#Region "History"` menjadi **pencatat jurnal tidak seimbang** saja.

#### 4.1 Fungsi Baru: `CatatJurnalTidakSeimbang(noTransaksi, totalDebet, totalKredit)`

```vb
' Dipanggil setelah setiap INSERT ke JurnalUmum
' Hanya menulis ke History jika debet ≠ kredit
Public Sub CatatJurnalTidakSeimbang(
    ByVal noTransaksi As String,
    ByVal totalDebet As Decimal,
    ByVal totalKredit As Decimal,
    ByVal jenisTransaksi As String)

    If totalDebet <> totalKredit Then
        Dim selisih As Decimal = totalDebet - totalKredit
        Dim pesan As String = $"[JURNAL TIDAK SEIMBANG] {noTransaksi} | {jenisTransaksi} | D:{totalDebet:N0} K:{totalKredit:N0} Selisih:{selisih:N0}"
        ' INSERT ke History
    End If
End Sub
```

#### 4.2 Cara Validasi Keseimbangan

Setiap form transaksi sudah tahu berapa nominal yang ditulis ke jurnal. Validasi dilakukan dengan menjumlahkan semua nominal debet dan kredit yang diinsert dalam satu transaksi:

```vb
' Contoh di FormPenjualan setelah semua jurnal diinsert:
Dim totalD As Decimal = nominalJurnal1 + nominalJurnal2  ' semua akun debet
Dim totalK As Decimal = nominalJurnal1 + nominalJurnal2  ' semua akun kredit
CatatJurnalTidakSeimbang(noTransaksi, totalD, totalK, "Penjualan")
```

Atau lebih sederhana: query langsung ke JurnalUmum setelah INSERT:

```sql
SELECT 
    SUM(NOMINAL) AS total_debet   -- dari baris dengan NO_TRANSAKSI ini sebagai debet
    -- tapi ini tidak bisa karena 1 baris = 1 debet + 1 kredit dengan nominal sama
```

**Catatan penting:** Karena struktur JurnalUmum adalah **1 baris = 1 pasang debet-kredit dengan nominal sama**, setiap baris selalu seimbang secara individual. Yang bisa tidak seimbang adalah jika ada baris yang seharusnya diinsert tapi gagal (partial insert dalam transaction yang tidak di-rollback dengan benar).

Jadi validasi yang lebih tepat: **cek apakah jumlah baris jurnal yang diinsert sesuai ekspektasi**, bukan cek debet=kredit per baris.

#### 4.3 Alternatif Validasi yang Lebih Praktis

Setelah transaction.Commit(), query JurnalUmum untuk NO_TRANSAKSI tersebut dan hitung:
```sql
SELECT 
    SUM(NOMINAL) AS total_debet_sisi,
    COUNT(*) AS jumlah_baris
FROM JurnalUmum 
WHERE NO_TRANSAKSI = @noTransaksi
```

Bandingkan dengan ekspektasi. Jika tidak sesuai, catat ke History.

#### 4.4 Hapus Semua Pemanggil CatatanAksiHistory Lama

Setelah fungsi baru siap, hapus semua 40+ pemanggil `CatatanAksiHistory` dari seluruh codebase. Tabel `History` lama bisa di-truncate atau di-archive.

---

## URUTAN EKSEKUSI

### Fase 0 — Jurnal Saldo Awal Master Data ✅ SELESAI
Tambah jurnal ke `JurnalUmum` saat input/edit saldo awal di form master:

| Form | Status | Akun Debet | Akun Kredit | Keterangan |
|------|--------|-----------|------------|-----------|
| `TambahSupliyer` | ✅ Done | `MODAL` (04.01.001) | `TAGIHAN / SALDO PIUTANG` (01.04.002) | Insert: jika `hutangAwal > 0`. Edit: jurnal selisih jika berubah, arah D/K menyesuaikan naik/turun |
| `TambahPelanggan` | ✅ Done | `HUTANG BELANJA` (03.01.001) | `MODAL` (04.01.001) | Insert: jika `hutangAwal > 0`. Edit: jurnal selisih jika berubah |
| `FormKaryawan` | N/A | — | — | `TxtAwal` adalah Gaji pokok, bukan saldo bon. Saldo bon dikelola via `Bon_karyawan`, tidak ada saldo awal bon di form ini |

Nomor transaksi jurnal saldo awal: format `SA-yyyyMMddHHmmss-{Kode}`
Jenis transaksi: `"Saldo Awal"`

### Fase 1 — Fungsi Baru di ModuleVariabel.vb
1. Tambah `UpdateSaldoAkun(kodeAkun, transaction)` di Region "Saldo Akun Jurnal"
2. Tambah `UpdateSaldoSemuaAkun()` di Region "Saldo Akun Jurnal"
3. Hapus `SaldoAkunTambah` dan `SaldoAkunKurang` (orphan)

### Fase 2 — Integrasi ke Form Transaksi
Urutan prioritas (dari yang paling sering dipakai):
1. FormPenjualan
2. FormPembelian
3. FormBayarHutang
4. FormBayarPiutang
5. FormGaji + FormBon
6. FormReturPenjualan + FormReturPembelian
7. FormKeuangan

### Fase 3 — Integrasi ke FormLoading
Tambah `UpdateSaldoSemuaAkun()` di `MulaiPosting()` dan `MulaiLoading()`

### Fase 4 — Refactor History
1. Buat fungsi `CatatJurnalTidakSeimbang()` baru
2. Hapus semua pemanggil `CatatanAksiHistory` lama (40+ lokasi)
3. Rename Region "History" → "Jurnal Audit"
4. Hapus fungsi `CatatanAksiHistory` lama

---

## RISIKO & MITIGASI

| Risiko | Mitigasi |
|--------|----------|
| `Saldo_Awal` di tbl_datareferensi mungkin NULL atau tidak konsisten | Gunakan `IFNULL(Saldo_Awal, 0)` di semua query |
| FormKeuangan akun dinamis — kode akun tidak selalu tersimpan di variabel global | Pastikan kode akun (bukan nama) selalu tersedia sebelum memanggil UpdateSaldoAkun |
| Performa — UpdateSaldoSemuaAkun dipanggil per transaksi | Index `idx_akun_d_nominal` dan `idx_akun_k_nominal` sudah ada — covering index scan, estimasi < 50ms |
| Hapus 40+ pemanggil CatatanAksiHistory — risiko miss | Gunakan grep/replace otomatis, verifikasi dengan build |

---

## CATATAN PENTING

- `HITUNGSEMUASALDO()` di `FormLapNeracaLR` mengupdate `SALDO_SEBELUMNYA` dan `S_DEBET/S_KREDIT` — ini **berbeda** dari `Saldo_Akhir`. Jangan disamakan. `Saldo_Akhir` adalah saldo running realtime, `SALDO_SEBELUMNYA` adalah untuk laporan neraca periode.
- `SaldoAkunTambah/Kurang` yang lama mengupdate **kedua akun (debet dan kredit) dengan operasi yang sama** — ini secara akuntansi salah (debet harusnya +, kredit harusnya -). Pendekatan recalculate menghindari masalah ini.
- Tabel `History` saat ini berisi data dari semua aksi master (tambah barang, edit merk, dll) — ini tidak perlu dihapus sekarang, cukup stop menulis data baru yang tidak penting.
