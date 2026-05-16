# Dokumentasi Implementasi AppAndroid/api ke Stored Procedure

---

## 1. Tujuan Refaktor
Tujuan utama dari refaktor ini adalah untuk **memindahkan validasi stok dan bisnis logic dari klien (Flutter) ke server-side (MySQL Stored Procedure)**, sehingga:
- Validasi stok menjadi **100% akurat** (tidak bisa dilewatkan dari klien)
- Neraca selalu seimbang
- Tidak ada perubahan UI di Flutter atau VB.NET
- VB.NET tetap berjalan normal tanpa perubahan

---

## 2. Kondisi Saat Ini (Sebelum Refaktor)
### Aplikasi Utama: VB.NET
- ✅ Tetap berjalan normal, **TIDAK DIUBAH**
- ✅ Masih menggunakan inline SQL untuk semua transaksi
- ✅ Validasi stok masih di sisi VB.NET

### Aplikasi Pembantu: Flutter (AppAndroid/api)
- ✅ `sync_penjualan.php`: Sudah CALL `sp_trx_penjualan_simpan` dan **parameter order sudah diperbaiki**
- ✅ `sync_stokopname.php`: Sudah dibuat, CALL `sp_trx_opname_simpan`, gaya 1:1 seperti `sync_penjualan.php`
- ✅ `sync_transfer_stok.php`: Sudah dibuat! Gaya seperti `sync_penjualan.php` dan `sync_stokopname.php`, test berhasil! 🎉
- ✅ `transfer_stok_screen.dart`: Sudah dibuat dengan mengikuti gaya `stok_opname_screen.dart` 1:1!
- ✅ Menu "Transfer Stok" sudah ditambahkan di `app_drawer.dart`!
- ✅ Method `syncTransferStok` sudah ditambahkan di `api_service.dart`! dan juga ui/ux ikuti standart dari stok opname) bagaimana pncarian barang di stok opname dll

---

### Database
- ✅ Semua helper SP (`sp_hlp_*`) sudah ada
- ✅ Semua skeleton SP transaksi (`sp_trx_*`) sudah ada di `07_migrasi_sp_transaksi.sql`
- ✅ Semua SP sudah dijalankan ke database `db_kasirlancar`

---

## 3. Daftar Fitur yang Sudah Ada di Database
### Helper SP (`sp_hlp_*`)
| Nama SP | Fungsi |
|---------|--------|
| `sp_hlp_stok_hitung` | Recalculate `STOK_TOKO` dan `STOK_GUDANG` dari semua counter |
| `sp_hlp_stok_validasi` | Cek apakah stok barang cukup untuk transaksi (support izinkan stok minus) |
| `sp_hlp_faktur_generate` | Generate nomor faktur unik (format: {PREFIX}-{YYMMDD}{XXXX}) |
| `sp_hlp_saldo_akun_update` | Recalculate `S_DEBET`, `S_KREDIT`, dan `SALDO_AKHIR` untuk satu akun |
| `sp_hlp_saldo_kas_validasi` | Cek apakah saldo kas/bank cukup sebelum pengeluaran |

### SP Transaksi (`sp_trx_*`) - Skeleton
| Nama SP | Fungsi | Temporary Table |
|---------|--------|------------------|
| `sp_trx_penjualan_simpan` | Orkestrasi simpan penjualan | `tmp_penjualan_items` |
| `sp_trx_opname_simpan` | Orkestrasi simpan stok opname | `tmp_stokopname_items` |
| `sp_trx_transfer_stok_simpan` | Orkestrasi transfer stok antar barang (K = Keluar, M = Masuk) | Tidak butuh (1 transaksi = 1 barang) |

---

## 4. Mekanisme Refaktor (Agar Fungsi Tetap & UI Tidak Berubah)
### Prinsip Utama
1. **VB.NET TIDAK DIUBAH** sama sekali
2. **Flutter UI TIDAK DIUBAH** sama sekali
3. Hanya **AppAndroid/api (PHP)** yang diubah untuk CALL `sp_trx_*`
4. Validasi stok dan bisnis logic **100% di SP**, tidak boleh di PHP

### Alur Kerja Baru
```
Flutter → AppAndroid/api (PHP) → Stored Procedure (MySQL) → Database
                    ↓
              CALL sp_trx_*
              (bukan inline SQL)
```

---

## 5. Langkah Implementasi Detail
### Langkah 1: Implementasi `sp_trx_penjualan_simpan`
1. Buka `07_migrasi_sp_transaksi.sql`
2. Hapus baris `SET p_error_code = 'NOT_IMPLEMENTED'` dan `ROLLBACK` di akhir `sp_trx_penjualan_simpan`
3. Tambahkan langkah-langkah berikut:
   a. **INSERT ke `penjualan`**: Gunakan semua parameter IN
   b. **INSERT ke `penjualan_detail`**: Loop semua item di `tmp_penjualan_items`
   c. **Update counter stok di `tbl_barang`**: Tambahkan `PENJUALAN_TOKO` atau `PENJUALAN_GUDANG` sesuai lokasi
   d. **Call `sp_hlp_stok_hitung`**: Untuk setiap barang yang terjual
   e. **INSERT ke `HistoryBarang`**: Untuk setiap barang yang terjual (JENIS = "PENJUALAN")
   f. **INSERT ke `JurnalUmum`**: Buat jurnal sesuai logika di `FormPenjualan.vb`
   g. **Call `sp_hlp_saldo_akun_update`**: Untuk setiap akun yang terpengaruh
   h. **Jika ada `p_id_draft`**: Hapus dari `penjualan_ditahan` dan `penjualan_ditahan_detail`
4. Jalankan `07_migrasi_sp_transaksi.sql` ke database
5. Test `sync_penjualan.php`

### Langkah 2: Perbaiki `sync_penjualan.php`
- ✅ Sudah siap, tidak butuh perubahan besar (sudah CALL `sp_trx_penjualan_simpan`)
- Hanya pastikan parameter yang dikirim sesuai dengan `sp_trx_penjualan_simpan`

### Langkah 3: Implementasi `sp_trx_opname_simpan`
1. Buka `07_migrasi_sp_transaksi.sql`
2. Hapus baris `SET p_error_code = 'NOT_IMPLEMENTED'` dan `ROLLBACK` di akhir `sp_trx_opname_simpan`
3. Tambahkan langkah-langkah berikut:
   a. Loop semua item di `tmp_stokopname_items`
   b. **INSERT ke `Stok_Opname`**: Gunakan data dari `tmp_stokopname_items`
   c. Hitung `STOK_SELISIH = STOK_NYATA - STOK_SYSTEM`
   d. **Update counter `OPNAME_TOKO` atau `OPNAME_GUDANG` di `tbl_barang`**
   e. **Call `sp_hlp_stok_hitung`**: Untuk setiap barang yang diopname
   f. **INSERT ke `HistoryBarang`**: Untuk setiap barang yang diopname (JENIS = "OPNAME")
   g. **INSERT ke `JurnalUmum`**: Jika `STOK_SELISIH ≠ 0`
   h. **Call `sp_hlp_saldo_akun_update`**: Untuk setiap akun yang terpengaruh
4. Jalankan `07_migrasi_sp_transaksi.sql` ke database
5. Buat/perbaiki `sync_stokopname.php` untuk CALL `sp_trx_opname_simpan` (mirip dengan `sync_penjualan.php`)

### Langkah 4: Implementasi `sp_trx_transfer_stok_simpan`
1. Buka `07_migrasi_sp_transaksi.sql`
2. Hapus baris `SET p_error_code = 'NOT_IMPLEMENTED'` dan `ROLLBACK` di akhir `sp_trx_transfer_stok_simpan`
3. Tambahkan langkah-langkah berikut:
   a. **INSERT ke `transfer_stok`**: Gunakan semua parameter IN
   b. **Update counter stok barang ASAL (K = Keluar)**: Kurangi counter sesuai lokasi
   c. **Update counter stok barang TUJUAN (M = Masuk)**: Tambahkan counter sesuai lokasi
   d. **Call `sp_hlp_stok_hitung`**: Untuk kedua barang
   e. **INSERT ke `HistoryBarang`**: Untuk kedua barang (JENIS = "TRANSFER")
   f. Catatan: **TIDAK BUTUH JURNAL** karena hanya transfer stok, tidak merubah nilai persediaan
4. Jalankan `07_migrasi_sp_transaksi.sql` ke database
5. Buat `sync_transfer_stok.php` untuk CALL `sp_trx_transfer_stok_simpan`

---

## 6. Struktur Temporary Table
### `tmp_penjualan_items` (untuk `sp_trx_penjualan_simpan`)
Sudah ada di `sync_penjualan.php`, kolom:
- `ID_BARANG`, `NAMA_BARANG`, `HARGA_BELI`, `HARGA_JUAL`
- `QTY`, `SATUAN`, `ISI_SATUAN`, `QTY_SATUAN`
- `TOTAL_HARGA`, `DISKON_PERSEN`, `DISKON_RP`, `TOTAL_DISKON`
- `LABA`, `SERIAL_NUMBER`
- `KODE_REK_BARANG`, `NAMA_REK_BARANG`
- `TOTAL_HARGA_BELI`, `KODE_REK_JUAL`, `NAMA_REK_JUAL`

### `tmp_stokopname_items` (untuk `sp_trx_opname_simpan`)
Nanti buat di `sync_stokopname.php`, kolom sesuai tabel `Stok_Opname`:
- `ID_BARANG`, `NAMA_BARANG`, `KATEGORI`, `HARGA`
- `STOK_SYSTEM`, `STOK_NYATA`, `SATUAN`, `ISI_SATUAN`
- `TOTAL_QTY`, `TOTAL_HARGA`, `KETERANGAN`

---

## 7. Catatan Penting
- **JANGAN PERNAH RUBAH UI** di Flutter atau VB.NET
- **JANGAN PERNAH RUBAH VB.NET** di fase ini
- Semua validasi stok dan bisnis logic **HARUS di SP**, tidak boleh di PHP
- Semua operasi database di SP **HARUS di dalam satu transaksi** (`START TRANSACTION` dan `COMMIT` di akhir)
- Jika terjadi error di SP, **HARUS ROLLBACK** dan keluar (`LEAVE proc_body`)

---

## 8. Daftar File yang Akan Diubah
| File | Tujuan |
|------|--------|
| `07_migrasi_sp_transaksi.sql` | Implementasi detail `sp_trx_*` (hilangkan `NOT_IMPLEMENTED`) |
| `AppAndroid/api/sync_penjualan.php` | Sudah siap, hanya test |
| `AppAndroid/api/sync_stokopname.php` | Buat/perbaiki untuk CALL `sp_trx_opname_simpan` |
| `AppAndroid/api/sync_transfer_stok.php` | Buat baru untuk CALL `sp_trx_transfer_stok_simpan` (HANYA jika front end Flutter untuk transfer stok sudah dibuat!) |

---

## 9. Progres Implementasi Bertahap dan Urut
Berikut adalah urutan langkah yang harus dilakukan secara bertahap untuk menghindari kesalahan:

### Urutan 1: Fokus Penjualan Terlebih Dahulu
1. ✅ **Langkah 1.1**: Implementasi detail `sp_trx_penjualan_simpan` di `07_migrasi_sp_transaksi.sql`
   - ✅ Hapus `SET p_error_code = 'NOT_IMPLEMENTED'` dan `ROLLBACK`
   - ✅ Tambahkan langkah (E) sampai (L) sesuai dokumentasi
   - ✅ File `sp_test_final.sql` sudah dibuat untuk test tanpa tmp_penjualan_items
   - ✅ Perbaiki: Pakai string kosong (bukan NULL) untuk kolom `KODE_AKUN_TF` dan `NAMA_AKUN_TF`
   - ✅ Perbaiki: Pakai `HARGA_BELI` (bukan `HARGA_BELI_SATUAN`) ketika insert dari `tmp_penjualan_items`
2. ✅ **Langkah 1.2**: Test `sync_penjualan.php` dan `sp_trx_penjualan_simpan`
   - ✅ Parameter order sudah diperbaiki
   - ✅ CALL SP berhasil
   - ✅ Semua tabel diinsert dengan benar
   - ✅ Test tanpa tmp_penjualan_items berhasil (`sp_test_final.sql`)
   - ✅ Test dengan tmp_penjualan_items berhasil!
   - ✅ Update config.php: database menjadi `db_kasirlancar`

### Urutan 2: Lanjut ke Stok Opname
3. ✅ **Langkah 2.1**: Implementasi detail `sp_trx_opname_simpan` di `07_migrasi_sp_transaksi.sql`
   - ✅ Hapus `SET p_error_code = 'NOT_IMPLEMENTED'` dan `ROLLBACK`
   - ✅ Tambahkan langkah (D) sampai (K) sesuai dokumentasi
   - ✅ Fix: Pakai nama tabel `stok_opname` (huruf kecil)
   - ✅ Jalankan dan test berhasil!
4. ✅ **Langkah 2.2**: Buat `sync_stokopname.php`
   - ✅ Sudah dibuat! Gaya 1:1 seperti `sync_penjualan.php`: buat temporary table `tmp_stokopname_items`, set user variables, CALL `sp_trx_opname_simpan`, baca OUT parameter dan response JSON
5. ✅ **Langkah 2.3**: Test `sync_stokopname.php`
   - ✅ Test file `test_sync_stokopname.php` dibuat dan test berhasil! (dengan `izinkan_backdate = 1`)

### Urutan 3: Terakhir Transfer Stok
6. ✅ **Langkah 3.1**: Implementasi detail `sp_trx_transfer_stok_simpan` di `07_migrasi_sp_transaksi.sql`
   - ✅ Hapus `SET p_error_code = 'NOT_IMPLEMENTED'` dan `ROLLBACK`
   - ✅ Tambahkan langkah (E) sampai (J) sesuai dokumentasi
   - ✅ Jalankan dan test berhasil! (dengan `p_izinkan_stok_minus = 1`)
7. ✅ **Langkah 3.2**: Buat `sync_transfer_stok.php`
   - ✅ Sudah dibuat! Gaya seperti `sync_penjualan.php` dan `sync_stokopname.php`!
8. ✅ **Langkah 3.3**: Test `sync_transfer_stok.php`
   - ✅ Test file `test_sync_transfer_stok.php` dibuat dan test berhasil!

---

## 10. Catatan Selama Implementasi
- Selalu **test 1 fitur terlebih dahulu** sebelum lanjut ke fitur berikutnya
- Selalu **backup database** sebelum menjalankan migrasi
- Jika terjadi error, **rollback dan periksa log**
- **JANGAN PERNAH RUBAH UI** di Flutter atau VB.NET
- **JANGAN PERNAH RUBAH VB.NET** di fase ini
- **Semua file test wajib hanya di satu folder, jangan dipisah-pisah**:
  - Untuk test **SQL**: Hanya di folder `Database/test/`
  - Untuk test **PHP**: Hanya di folder `AppAndroid/api/test/`
- Jangan buat file test di folder utama (misal `Database/` atau `AppAndroid/api/`), hanya di sub-folder test!
- Setelah test selesai dan fitur berjalan normal, **hapus file test temporary** untuk menjaga kebersihan direktori!
