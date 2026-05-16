# Design — Optimasi Index Database AppKasir

## Analisis Index per Tabel

### `jurnalumum` — 13 index saat ini

Tabel ini adalah tabel paling berat dari sisi query (laporan keuangan, buku besar, mutasi kas).
Setiap transaksi (jual, beli, retur, bon, gaji) INSERT ke tabel ini.

#### Peta Query → Index

| Query Pattern | File | Index yang Dipakai |
|---|---|---|
| `WHERE NO_TRANSAKSI = @fk` | `ModuleVariabel.vb`, banyak form DELETE | `idx_no_transaksi_jurnal` ✅ |
| `WHERE TGL_TRANSAKSI >= @a AND <= @b` | `ModuleVerifikasiJurnal.vb`, `ModuleLaporanKalkulasi.vb` | `idx_tgl_jenis_transaksi` ✅ |
| `WHERE NOMOR_AKUN_D = @akun AND TGL <= @tgl` | `FormLapBB.vb`, `FormLapMutasiKeuangan.vb` | `idx_nomor_akun_d_jurnal` ✅ |
| `WHERE NOMOR_AKUN_K = @akun AND TGL <= @tgl` | `FormLapBB.vb`, `FormLapMutasiKeuangan.vb` | `idx_nomor_akun_k_jurnal` ✅ |
| `WHERE TGL >= @a AND <= @b AND NOMOR_AKUN_D = @akun AND JENIS IN (...)` | `FormLapMutasiKeuangan.vb` | `idx_tgl_jenis_akun_d_nominal` ✅ |
| `WHERE TGL >= @a AND <= @b AND ID_USER LIKE @user` | `FormLapMutasiKeuangan.vb` | `idx_tgl_id_user_jurnal` ✅ |
| `GROUP BY NOMOR_AKUN_D` (tanpa filter TGL) | `ModuleLaporanKalkulasi.vb` (hitung saldo semua) | `idx_nomor_akun_d_jurnal` ✅ |
| `WHERE LOKASI = @lok` (batch delete) | `FormHapusTransaksi.vb` | `idx_lokasi_jurnal` ✅ |

#### Index yang Tidak Punya Query Pendukung

```
idx_tgl_akun_d_nominal    (TGL_TRANSAKSI, NOMOR_AKUN_D, NOMINAL)
```
Semua query yang filter `TGL + NOMOR_AKUN_D` juga filter `JENIS_TRANSAKSI` atau tidak filter `NOMINAL` sama sekali.
MySQL akan memilih `idx_tgl_jenis_akun_d_nominal` (4 kolom) yang lebih informatif.

```
idx_akun_d_nominal    (NOMOR_AKUN_D, NOMINAL)
idx_akun_k_nominal    (NOMOR_AKUN_K, NOMINAL)
```
Tidak ada query `WHERE NOMOR_AKUN_D = x AND NOMINAL = y` tanpa filter TGL.
Semua query pakai TGL → optimizer pilih `idx_nomor_akun_d_jurnal (NOMOR_AKUN_D, TGL_TRANSAKSI)`.

#### Hasil Setelah Cleanup

| # | Index | Status |
|---|---|---|
| 1 | `idx_no_transaksi_jurnal` | ✅ Pertahankan |
| 2 | `idx_tgl_jenis_transaksi` | ✅ Pertahankan |
| 3 | `idx_id_user_jurnal` | ✅ Pertahankan |
| 4 | `idx_nomor_akun_d_jurnal` | ✅ Pertahankan |
| 5 | `idx_nomor_akun_k_jurnal` | ✅ Pertahankan |
| 6 | `idx_tgl_akun_d_nominal` | ❌ Hapus — prefix dari #10 |
| 7 | `idx_tgl_akun_k_nominal` | ❌ Hapus — prefix dari #11 |
| 8 | `idx_akun_d_nominal` | ❌ Hapus — tidak ada query pendukung |
| 9 | `idx_akun_k_nominal` | ❌ Hapus — tidak ada query pendukung |
| 10 | `idx_tgl_jenis_akun_d_nominal` | ✅ Pertahankan |
| 11 | `idx_tgl_jenis_akun_k_nominal` | ✅ Pertahankan |
| 12 | `idx_tgl_id_user_jurnal` | ✅ Pertahankan |
| 13 | `idx_lokasi_jurnal` | ✅ Pertahankan (FormHapusTransaksi) |

**Dari 13 → 9 index.** Setiap INSERT ke `jurnalumum` sebelumnya update 13 index, setelah cleanup hanya 9.

---

### `tbl_barang` — 14 index saat ini

Tabel ini di-UPDATE setiap transaksi (stok berubah). 14 index = overhead besar.

#### Index Stok yang Tumpang Tindih

```
idx_stok_toko_gudang     (STOK_TOKO, STOK_GUDANG)           ← prefix dari ↓
idx_stok_minimum         (STOK_MIN, STOK_TOKO, STOK_GUDANG) ← lebih lengkap
```

`idx_stok_toko_gudang` tidak pernah dipilih karena `idx_stok_minimum` sudah cover kolom yang sama plus `STOK_MIN`. Hapus `idx_stok_toko_gudang`.

**Dari 14 → 13 index.**

---

### `bon_karyawan` — Gap Index

Query di `FormLapBonPerorang.vb`:
```sql
SELECT SUM(NOMINAL) FROM Bon_Karyawan 
WHERE KODE LIKE @KODE AND TANGGAL < @TANGGAL AND JENIS = 'BON'
```

Index yang ada: `idx_kode_jenis_bon (KODE, JENIS)` — setelah scan KODE+JENIS, filter TANGGAL dilakukan di memory.

Index optimal: `idx_kode_tanggal_jenis_bon (KODE, TANGGAL, JENIS)`
- `KODE` = equality → paling selektif, posisi pertama ✅
- `TANGGAL` = range (`<` atau `BETWEEN`) → posisi kedua ✅
- `JENIS` = equality tapi setelah range tidak dipakai optimizer untuk filter, tapi berguna sebagai covering index

> **Catatan:** Index `idx_kode_jenis_tanggal_bon (KODE, JENIS, TANGGAL)` sudah ada di `03_migrasi_index.sql`.
> Urutan kolom ini kurang optimal untuk query range TANGGAL karena JENIS di posisi 2 memaksa full scan setelah KODE.
> Index baru `(KODE, TANGGAL, JENIS)` lebih baik untuk query dengan range TANGGAL.

---

### Index LOKASI untuk Tabel Utility — Trade-off Analysis

`FormHapusTransaksi.vb` menjalankan:
```vb
HapusWhere(cmd, "transfer_barang", "LOKASI", mode)
HapusWhere(cmd, "surat_jalan", "LOKASI", mode)
' ... dst
```

Yang menghasilkan: `DELETE FROM transfer_barang WHERE LOKASI = 'TOKO'`

**Frekuensi operasi:**
- INSERT ke `transfer_barang`: setiap ada transfer barang antar gudang/toko (bisa harian)
- DELETE by LOKASI: hanya saat admin hapus data (bulanan/tahunan)

**Keputusan per tabel:**

| Tabel | Volume INSERT | Keputusan | Alasan |
|---|---|---|---|
| `transfer_barang` | Rendah (tidak setiap hari) | ✅ Pertahankan | Overhead kecil |
| `transfer_barang_detail` | Rendah | ✅ Pertahankan | Overhead kecil |
| `transfer_cabang` | Rendah | ✅ Pertahankan | Overhead kecil |
| `transfer_cabang_detail` | Rendah | ✅ Pertahankan | Overhead kecil |
| `surat_jalan` | Sedang (per pengiriman) | ✅ Pertahankan | Masih acceptable |
| `surat_jalan_detail` | Sedang | ✅ Pertahankan | Masih acceptable |
| `hutang_detail` | Sedang (per bayar hutang) | ✅ Pertahankan | Masih acceptable |
| `piutang_detail` | Sedang (per bayar piutang) | ✅ Pertahankan | Masih acceptable |
| `bon_karyawan` | Tinggi (harian per karyawan) | ⚠️ Monitor | Jika > 1000 baris/hari, pertimbangkan hapus |
| `gaji_karyawan` | Rendah (bulanan) | ✅ Pertahankan | Overhead sangat kecil |

**Kesimpulan R3:** Semua 10 index LOKASI dipertahankan untuk saat ini, dengan komentar trade-off di SQL.

---

## Struktur Script Cleanup

### `Database/04b_optimasi_index_cleanup.sql`

```
1. Header & komentar
2. Procedure helper drop_index_if_exists
3. SECTION A: Hapus index redundan jurnalumum (4 index)
4. SECTION B: Hapus index redundan tbl_barang (1 index)  
5. SECTION C: Tambah index baru bon_karyawan (1 index)
6. Cleanup procedure
7. SELECT status ringkasan
```

### Perubahan di `03_migrasi_index.sql`

Tambahkan komentar `-- DIPINDAHKAN ke 04b_optimasi_index_cleanup.sql` pada index yang dihapus,
agar jika script lama dijalankan ulang tidak membuat ulang index yang sudah dihapus.

> **Penting:** Index yang dihapus di `04b` tidak boleh ada di `03` — jika `03` dijalankan setelah `04b`,
> index akan dibuat ulang. Solusi: tandai dengan komentar dan hapus dari `03`.

---

## Estimasi Dampak

| Metrik | Sebelum | Sesudah |
|---|---|---|
| Index di `jurnalumum` | 13 | 9 |
| Index di `tbl_barang` | 14 | 13 |
| Index baru di `bon_karyawan` | — | +1 |
| Overhead INSERT `jurnalumum` | 13x update index | 9x update index (~30% lebih cepat) |
| Overhead UPDATE `tbl_barang` | 14x update index | 13x update index |
| Query laporan bon per orang | Scan KODE+JENIS lalu filter TGL | Index scan KODE+TGL langsung |
