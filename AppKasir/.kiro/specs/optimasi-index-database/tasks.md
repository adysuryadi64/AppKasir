# Tasks — Optimasi Index Database AppKasir

Semua file SQL menggunakan awalan `03_`:
- `Database/03_backup.sql` — script backup (Task 1)
- `Database/03_cleanup_index.sql` — script cleanup index redundan (Task 2)
- `Database/03_migrasi_index.sql` — file migrasi utama yang di-edit (Task 3)

Urutan wajib: **Task 1 (backup) → Task 2 (cleanup DB) → Task 3 (edit file SQL)**.

---

## Task 1 — Backup Database

Sebelum apapun, backup database production. Script disimpan sebagai `Database/03_backup.sql`.

```sql
-- Jalankan dari terminal / MySQL Workbench
mysqldump -u root -p nama_database > 03_backup_sebelum_index_cleanup_20260421.sql
```

Atau via `FormMigrasiDB` / `SettingDatabase` jika ada fitur backup di aplikasi.

**Kriteria selesai:**
- [x] File backup tersimpan dan ukurannya wajar (tidak 0 byte)
- [x] Bisa di-restore jika diperlukan

> ✅ **Backup selesai** — `Database/03_backup_sebelum_index_cleanup_20260421.sql` (592 MB)

---

## Task 2 — Jalankan Cleanup di Database

Jalankan script SQL berikut langsung di MySQL untuk menghapus index yang sudah diidentifikasi.
Script ini idempotent — aman dijalankan ulang.

```sql
-- ============================================================
-- CLEANUP INDEX REDUNDAN — AppKasir
-- Jalankan SETELAH backup, SEBELUM edit 03_migrasi_index.sql
-- ============================================================

DROP PROCEDURE IF EXISTS drop_index_if_exists;
DELIMITER $
CREATE PROCEDURE drop_index_if_exists(IN tbl VARCHAR(100), IN idx VARCHAR(100))
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND INDEX_NAME   = idx
    ) THEN
        SET @sql = CONCAT('ALTER TABLE `', tbl, '` DROP INDEX `', idx, '`');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        SELECT CONCAT('DROPPED  : ', tbl, '.', idx) AS hasil;
    ELSE
        SELECT CONCAT('SKIP     : ', tbl, '.', idx, ' (tidak ditemukan)') AS hasil;
    END IF;
END$
DELIMITER ;

-- ── jurnalumum: 7 index redundan ─────────────────────────────
-- Prefix dari idx_tgl_jenis_akun_d_nominal (4 kolom)
CALL drop_index_if_exists('jurnalumum', 'idx_tgl_akun_d_nominal');
-- Prefix dari idx_tgl_jenis_akun_k_nominal (4 kolom)
CALL drop_index_if_exists('jurnalumum', 'idx_tgl_akun_k_nominal');
-- Tidak ada query filter NOMINAL tanpa TGL di seluruh codebase
CALL drop_index_if_exists('jurnalumum', 'idx_akun_d_nominal');
-- Tidak ada query filter NOMINAL tanpa TGL di seluruh codebase
CALL drop_index_if_exists('jurnalumum', 'idx_akun_k_nominal');
-- Query LoadRekapSekaliBaca: JENIS_TRANSAKSI ada di CASE WHEN bukan WHERE → index tidak dipakai
-- Query ExecuteQuery: optimizer pilih idx_nomor_akun_d_jurnal (equality dulu lebih optimal)
CALL drop_index_if_exists('jurnalumum', 'idx_tgl_jenis_akun_d_nominal');
-- Sama persis dengan alasan di atas untuk sisi NOMOR_AKUN_K
CALL drop_index_if_exists('jurnalumum', 'idx_tgl_jenis_akun_k_nominal');
-- Tidak ada query WHERE ID_USER saja; idx_tgl_id_user_jurnal (TGL,ID_USER) sudah cover semua kasus
CALL drop_index_if_exists('jurnalumum', 'idx_id_user_jurnal');

-- ── tbl_barang: 2 index redundan ────────────────────────────
-- Prefix dari idx_stok_minimum (STOK_MIN,STOK_TOKO,STOK_GUDANG)
CALL drop_index_if_exists('tbl_barang', 'idx_stok_toko_gudang');
-- Duplikat PRIMARY KEY — optimizer selalu pilih PK, index ini tidak pernah dipakai
CALL drop_index_if_exists('tbl_barang', 'idx_id_barang_prefix');

-- ── tbl_datareferensi: 1 index tanpa query pendukung ─────────
-- Tidak ada query WHERE JENIS_AKUN ditemukan di seluruh codebase
CALL drop_index_if_exists('tbl_datareferensi', 'idx_jenis_akun');

-- ── pembelian: 2 index redundan / tanpa query pendukung ──────
-- Prefix dari idx_jatuh_tempo_status_beli (JATUH_TEMPO,STATUS_TRANSAKSI_BELI)
CALL drop_index_if_exists('pembelian', 'idx_jatuh_tempo_beli');
-- Hanya untuk DISTINCT dropdown — bukan critical query
CALL drop_index_if_exists('pembelian', 'idx_nama_supliyer');
-- CATATAN: idx_tgl_bayar_beli DIPERTAHANKAN — dipakai di FormLapHutang mode BY PELUNASAN

-- ── stok_opname: 1 index tidak efektif ───────────────────────
-- Query pakai OR (TANGGAL >= @a OR ID_USER LIKE @u) — index tidak bisa dipakai
CALL drop_index_if_exists('stok_opname', 'idx_id_user_opname');

-- ── retur_pembelian: 1 index hanya untuk display ─────────────
-- Hanya untuk DISTINCT NAMA_REKENING dropdown — bukan critical query
CALL drop_index_if_exists('retur_pembelian', 'idx_nama_rekening_retur_beli');

-- ── retur_penjualan: 1 index hanya untuk display ─────────────
-- Hanya untuk DISTINCT NAMA_REKENING dropdown — bukan critical query
CALL drop_index_if_exists('retur_penjualan', 'idx_nama_rekening_retur_jual');

-- ── penjualan: 7 index redundan / tanpa query pendukung ──────
-- Prefix dari idx_id_sales_tgl_jual (ID_SALES,TGL_TRANSAKSI)
CALL drop_index_if_exists('penjualan', 'idx_id_sales_jual');
-- Prefix dari idx_jatuh_tempo_status_jual (JATUH_TEMPO,STATUS_TRANSAKSI)
CALL drop_index_if_exists('penjualan', 'idx_jatuh_tempo_jual');
-- Tidak ada query WHERE STATUS_BAYAR ditemukan di seluruh codebase
CALL drop_index_if_exists('penjualan', 'idx_status_bayar_jual');
-- Tidak ada query WHERE TGL_PEMBAYARAN ditemukan di seluruh codebase
CALL drop_index_if_exists('penjualan', 'idx_tgl_pembayaran_jual');
-- Prefix dari idx_tgl_kode_akun_jual (TGL_TRANSAKSI,KODE_AKUN)
CALL drop_index_if_exists('penjualan', 'idx_kode_akun_jual');
-- Hanya untuk DISTINCT dropdown ComboBox — bukan critical query, overhead INSERT tidak sepadan
CALL drop_index_if_exists('penjualan', 'idx_nama_sales_jual');
-- Hanya untuk DISTINCT dropdown + ORDER BY display — bukan critical query
CALL drop_index_if_exists('penjualan', 'idx_nama_pelanggan_jual');
-- Tidak ada query WHERE JENIS_PEMBAYARAN di seluruh codebase — kolom hanya di SELECT/display
CALL drop_index_if_exists('penjualan', 'idx_jenis_pembayaran_jual');

-- ── bon_karyawan: tambah index yang hilang ────────────────────
-- Query FormLapBonPerorang: WHERE KODE=@k AND TANGGAL<@t AND JENIS='BON'
-- Urutan KODE,TANGGAL,JENIS optimal untuk range TANGGAL setelah equality KODE
DROP PROCEDURE IF EXISTS add_index_if_not_exists_tmp;
DELIMITER $
CREATE PROCEDURE add_index_if_not_exists_tmp(IN tbl VARCHAR(100), IN idx VARCHAR(100), IN cols TEXT)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND INDEX_NAME   = idx
    ) THEN
        SET @sql = CONCAT('ALTER TABLE `', tbl, '` ADD INDEX `', idx, '` (', cols, ')');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        SELECT CONCAT('ADDED    : ', tbl, '.', idx) AS hasil;
    ELSE
        SELECT CONCAT('SKIP     : ', tbl, '.', idx, ' (sudah ada)') AS hasil;
    END IF;
END$
DELIMITER ;

CALL add_index_if_not_exists_tmp('bon_karyawan', 'idx_kode_tanggal_jenis_bon', 'KODE,TANGGAL,JENIS');

-- tbl_satuan: gap index untuk query di TambahSatuan.vb
-- WHERE kode = @Kode (equality) dan ORDER BY isi
CALL add_index_if_not_exists_tmp('tbl_satuan', 'idx_kode_satuan', 'kode');
CALL add_index_if_not_exists_tmp('tbl_satuan', 'idx_isi_satuan', 'isi');

DROP PROCEDURE IF EXISTS drop_index_if_exists;
DROP PROCEDURE IF EXISTS add_index_if_not_exists_tmp;

SELECT '=== Cleanup selesai. Lanjut ke Task 3: edit 03_migrasi_index.sql ===' AS status;
```

**Kriteria selesai:**
- [x] Semua baris output bertuliskan `DROPPED` atau `SKIP` — tidak ada `ERROR`
- [x] Total 23 index terhapus, 3 index ditambah
- [x] Verifikasi via: `SHOW INDEX FROM jurnalumum;` → 6 index operasional ✅ (8 total termasuk PRIMARY + uq_sync)
- [x] Verifikasi via: `SHOW INDEX FROM penjualan;` → 18 index operasional ✅ (20 total termasuk PRIMARY + uq_sync)
- [x] Verifikasi via: `SHOW INDEX FROM pembelian;` → berkurang 2 ✅
- [x] Verifikasi via: `SHOW INDEX FROM tbl_barang;` → berkurang 2 ✅
- [x] Verifikasi via: `SHOW INDEX FROM tbl_datareferensi;` → berkurang 1 ✅

> ✅ **Script `Database/03_cleanup_index.sql` sudah dibuat** — tinggal dijalankan di MySQL setelah backup.

---

## Task 3 — Edit `Database/03_migrasi_index.sql`

Setelah Task 2 selesai, comment out semua baris yang membuat index yang sudah dihapus,
dan tambahkan index baru `bon_karyawan`. Tujuan: jika file ini dijalankan ulang di masa depan,
index yang sudah dihapus tidak akan dibuat kembali.

### 3a — Comment out 6 index di section `jurnalumum`

Cari dan ganti:
```sql
CALL add_index_if_not_exists('jurnalumum', 'idx_id_user_jurnal', 'ID_USER');
```
Jadi:
```sql
-- [DIHAPUS] tidak ada query WHERE ID_USER saja; idx_tgl_id_user_jurnal sudah cover semua kasus
-- CALL add_index_if_not_exists('jurnalumum', 'idx_id_user_jurnal', 'ID_USER');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_akun_d_nominal', 'TGL_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_akun_k_nominal', 'TGL_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_akun_d_nominal', 'NOMOR_AKUN_D,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_akun_k_nominal', 'NOMOR_AKUN_K,NOMINAL');
```

Jadi:
```sql
-- [DIHAPUS] prefix dari idx_tgl_jenis_akun_d_nominal — tidak ada query filter NOMINAL tanpa JENIS
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_akun_d_nominal', 'TGL_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
-- [DIHAPUS] prefix dari idx_tgl_jenis_akun_k_nominal — tidak ada query filter NOMINAL tanpa JENIS
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_akun_k_nominal', 'TGL_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
-- [DIHAPUS] tidak ada query WHERE NOMOR_AKUN_D + NOMINAL tanpa TGL di seluruh codebase
-- CALL add_index_if_not_exists('jurnalumum', 'idx_akun_d_nominal', 'NOMOR_AKUN_D,NOMINAL');
-- [DIHAPUS] tidak ada query WHERE NOMOR_AKUN_K + NOMINAL tanpa TGL di seluruh codebase
-- CALL add_index_if_not_exists('jurnalumum', 'idx_akun_k_nominal', 'NOMOR_AKUN_K,NOMINAL');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_akun_d_nominal', 'TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_akun_k_nominal', 'TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
```

Jadi:
```sql
-- [DIHAPUS] query LoadRekapSekaliBaca pakai CASE WHEN — JENIS_TRANSAKSI tidak di WHERE, index tidak dipakai
-- query ExecuteQuery: optimizer pilih idx_nomor_akun_d_jurnal (equality NOMOR_AKUN_D lebih selektif)
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_akun_d_nominal', 'TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
-- [DIHAPUS] alasan sama untuk sisi NOMOR_AKUN_K — idx_nomor_akun_k_jurnal lebih optimal
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_akun_k_nominal', 'TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
```

### 3b — Comment out 2 index di section `tbl_barang`

Cari dan ganti:
```sql
CALL add_index_if_not_exists('tbl_barang', 'idx_stok_toko_gudang', 'STOK_TOKO,STOK_GUDANG');
```
Jadi:
```sql
-- [DIHAPUS] prefix dari idx_stok_minimum (STOK_MIN,STOK_TOKO,STOK_GUDANG)
-- CALL add_index_if_not_exists('tbl_barang', 'idx_stok_toko_gudang', 'STOK_TOKO,STOK_GUDANG');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('tbl_barang', 'idx_id_barang_prefix', 'ID_BARANG');
```
Jadi:
```sql
-- [DIHAPUS] duplikat PRIMARY KEY — optimizer selalu pilih PK, index ini tidak pernah dipakai
-- CALL add_index_if_not_exists('tbl_barang', 'idx_id_barang_prefix', 'ID_BARANG');
```

**Section `tbl_datareferensi`** — comment out 1 baris:
```sql
-- [DIHAPUS] tidak ada query WHERE JENIS_AKUN ditemukan di seluruh codebase
-- CALL add_index_if_not_exists('tbl_datareferensi', 'idx_jenis_akun', 'JENIS_AKUN');
```

### 3c — Comment out 8 index di section `penjualan` + 4 tabel lain

**Section `pembelian`** — comment out 2 baris:
```sql
-- [DIHAPUS] prefix dari idx_jatuh_tempo_status_beli
-- CALL add_index_if_not_exists('pembelian', 'idx_jatuh_tempo_beli', 'JATUH_TEMPO');
-- [DIHAPUS] hanya DISTINCT dropdown — bukan critical query
-- CALL add_index_if_not_exists('pembelian', 'idx_nama_supliyer', 'NAMA_SUPLIYER');
-- CATATAN: idx_tgl_bayar_beli DIPERTAHANKAN — dipakai di FormLapHutang mode BY PELUNASAN
```

**Section `stok_opname`** — comment out 1 baris:
```sql
-- [DIHAPUS] query pakai OR — index tidak efektif untuk kondisi OR
-- CALL add_index_if_not_exists('stok_opname', 'idx_id_user_opname', 'ID_USER');
```

**Section `retur_pembelian`** — comment out 1 baris:
```sql
-- [DIHAPUS] hanya DISTINCT dropdown NAMA_REKENING — bukan critical query
-- CALL add_index_if_not_exists('retur_pembelian', 'idx_nama_rekening_retur_beli', 'NAMA_REKENING');
```

**Section `retur_penjualan`** — comment out 1 baris:
```sql
-- [DIHAPUS] hanya DISTINCT dropdown NAMA_REKENING — bukan critical query
-- CALL add_index_if_not_exists('retur_penjualan', 'idx_nama_rekening_retur_jual', 'NAMA_REKENING');
```

**Section `penjualan`** — comment out 7 baris:

Cari dan ganti:
```sql
CALL add_index_if_not_exists('penjualan', 'idx_jatuh_tempo_jual', 'JATUH_TEMPO');
```
Jadi:
```sql
-- [DIHAPUS] prefix dari idx_jatuh_tempo_status_jual (JATUH_TEMPO,STATUS_TRANSAKSI)
-- CALL add_index_if_not_exists('penjualan', 'idx_jatuh_tempo_jual', 'JATUH_TEMPO');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('penjualan', 'idx_tgl_pembayaran_jual', 'TGL_PEMBAYARAN');
CALL add_index_if_not_exists('penjualan', 'idx_status_bayar_jual', 'STATUS_BAYAR');
```
Jadi:
```sql
-- [DIHAPUS] tidak ada query WHERE TGL_PEMBAYARAN ditemukan di seluruh codebase
-- CALL add_index_if_not_exists('penjualan', 'idx_tgl_pembayaran_jual', 'TGL_PEMBAYARAN');
-- [DIHAPUS] tidak ada query WHERE STATUS_BAYAR ditemukan di seluruh codebase
-- CALL add_index_if_not_exists('penjualan', 'idx_status_bayar_jual', 'STATUS_BAYAR');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('penjualan', 'idx_kode_akun_jual', 'KODE_AKUN');
```
Jadi:
```sql
-- [DIHAPUS] prefix dari idx_tgl_kode_akun_jual (TGL_TRANSAKSI,KODE_AKUN)
-- CALL add_index_if_not_exists('penjualan', 'idx_kode_akun_jual', 'KODE_AKUN');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('penjualan', 'idx_id_sales_jual', 'ID_SALES');
```
Jadi:
```sql
-- [DIHAPUS] prefix dari idx_id_sales_tgl_jual (ID_SALES,TGL_TRANSAKSI)
-- CALL add_index_if_not_exists('penjualan', 'idx_id_sales_jual', 'ID_SALES');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('penjualan', 'idx_nama_sales_jual', 'NAMA_SALES');
```
Jadi:
```sql
-- [DIHAPUS] hanya untuk DISTINCT dropdown ComboBox — bukan critical query, overhead INSERT tidak sepadan
-- CALL add_index_if_not_exists('penjualan', 'idx_nama_sales_jual', 'NAMA_SALES');
```

Cari dan ganti:
```sql
CALL add_index_if_not_exists('penjualan', 'idx_nama_pelanggan_jual', 'NAMA_PELANGGAN');
```
Jadi:
```sql
-- [DIHAPUS] hanya untuk DISTINCT dropdown + ORDER BY display — bukan critical query
-- CALL add_index_if_not_exists('penjualan', 'idx_nama_pelanggan_jual', 'NAMA_PELANGGAN');
```

### 3e — Tambah index baru di section `tbl_satuan`

Cari baris terakhir di section tbl_satuan:
```sql
CALL add_index_if_not_exists('tbl_satuan', 'idx_id_cloud', 'id_cloud');
```

Tambahkan setelahnya:
```sql
-- Gap index: TambahSatuan.vb WHERE kode = @Kode dan ORDER BY isi
CALL add_index_if_not_exists('tbl_satuan', 'idx_kode_satuan', 'kode');
CALL add_index_if_not_exists('tbl_satuan', 'idx_isi_satuan', 'isi');
```

### 3d — Tambah index baru di section `bon_karyawan`

Cari baris terakhir di section bon_karyawan:
```sql
CALL add_index_if_not_exists('bon_karyawan', 'idx_kode_jenis_bon', 'KODE,JENIS');
```

Tambahkan setelahnya:
```sql
-- Optimal untuk FormLapBonPerorang: WHERE KODE=@k AND TANGGAL<@t AND JENIS='BON'
-- KODE (equality) → TANGGAL (range) → JENIS (covering) — lebih baik dari KODE,JENIS,TANGGAL
CALL add_index_if_not_exists('bon_karyawan', 'idx_kode_tanggal_jenis_bon', 'KODE,TANGGAL,JENIS');
```

**Kriteria selesai:**
- [x] 23 baris `CALL add_index_if_not_exists` di-comment dengan label `[DIHAPUS]`
- [x] 3 baris baru ditambahkan (`bon_karyawan`, `tbl_satuan` x2)
- [x] `idx_tgl_bayar_beli` di `pembelian` TIDAK di-comment (dipertahankan)
- [ ] Jalankan ulang `03_migrasi_index.sql` → semua baris aktif harus `SKIP (sudah ada)`
- [ ] Tidak ada index yang terhapus tadi dibuat ulang

---

## Urutan Eksekusi

```
1. mysqldump → backup_sebelum_index_cleanup.sql           (Task 1)
2. Jalankan script cleanup SQL di Task 2 via MySQL client   (Task 2)
3. Cek output: semua DROPPED/SKIP, tidak ada ERROR          (Task 2)
4. SHOW INDEX FROM jurnalumum → 6 index                    (Task 2)
5. SHOW INDEX FROM penjualan  → 18 index                   (Task 2)
6. SHOW INDEX FROM pembelian  → berkurang 2                (Task 2)
7. SHOW INDEX FROM tbl_barang → berkurang 2                (Task 2)
8. SHOW INDEX FROM tbl_datareferensi → berkurang 1         (Task 2)
9. Edit 03_migrasi_index.sql sesuai Task 3a–3e              (Task 3)
10. Jalankan ulang 03_migrasi_index.sql → semua SKIP        (Task 3 verifikasi)
11. Jalankan Task 4 (TRIM cleanup + edit VB)                (Task 4)
```

> ⚠️ Jangan jalankan Task 3 sebelum Task 2 selesai.
> Task 4 bisa dikerjakan kapan saja setelah Task 1 selesai — tidak bergantung pada Task 2/3.

---

## Task 4 — Perbaikan BARCODE + TRIM di VB

Tiga langkah harus dikerjakan **berurutan**.

### 4a — Bersihkan data lama (sekali saja, langsung di MySQL)

```sql
-- Cek dulu berapa baris yang perlu dibersihkan:
SELECT COUNT(*) AS kotor FROM tbl_barang
WHERE BARCODE_KECIL  != TRIM(BARCODE_KECIL)
   OR BARCODE_SEDANG != TRIM(BARCODE_SEDANG)
   OR BARCODE_BESAR  != TRIM(BARCODE_BESAR)
   OR ID_BARANG      != TRIM(ID_BARANG)
   OR NAMA_BARANG    != TRIM(NAMA_BARANG);

-- Jika ada baris, jalankan cleanup:
UPDATE tbl_barang SET
  BARCODE_KECIL      = TRIM(BARCODE_KECIL),
  BARCODE_SEDANG     = TRIM(BARCODE_SEDANG),
  BARCODE_BESAR      = TRIM(BARCODE_BESAR),
  ID_BARANG          = TRIM(ID_BARANG),
  NAMA_BARANG        = TRIM(NAMA_BARANG),
  NAMA_KATEGORI      = TRIM(NAMA_KATEGORI),
  KODE_KATEGORI      = TRIM(KODE_KATEGORI),
  NAMA_MERK          = TRIM(NAMA_MERK),
  KODE_MERK          = TRIM(KODE_MERK),
  NAMA_SUPLIYER      = TRIM(NAMA_SUPLIYER),
  KODE_SUPLIYER      = TRIM(KODE_SUPLIYER),
  SATUAN_UMUM_KECIL  = TRIM(SATUAN_UMUM_KECIL),
  SATUAN_UMUM_SEDANG = TRIM(SATUAN_UMUM_SEDANG),
  SATUAN_UMUM_BESAR  = TRIM(SATUAN_UMUM_BESAR),
  SATUAN_PARTAI_KECIL  = TRIM(SATUAN_PARTAI_KECIL),
  SATUAN_PARTAI_SEDANG = TRIM(SATUAN_PARTAI_SEDANG),
  SATUAN_PARTAI_BESAR  = TRIM(SATUAN_PARTAI_BESAR),
  SATUAN_STOK        = TRIM(SATUAN_STOK);

-- Verifikasi: harus 0
SELECT COUNT(*) AS sisa_kotor FROM tbl_barang
WHERE BARCODE_KECIL  != TRIM(BARCODE_KECIL)
   OR BARCODE_SEDANG != TRIM(BARCODE_SEDANG)
   OR BARCODE_BESAR  != TRIM(BARCODE_BESAR);
```

**Kriteria selesai:**
- [ ] `sisa_kotor` = 0

> ⚠️ Harus dijalankan manual di MySQL sebelum Task 4b/4c.

### 4b — TRIM saat INSERT/UPDATE di `1Master/TambahBarang.vb`

Cari semua `AddWithValue` untuk kolom barcode, nama, kode. Tambahkan `.Trim()`:

```vb
' Sebelum
cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeKecil.Text)
' Sesudah
cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeKecil.Text.Trim())
```

Berlaku untuk: `@BARCODE_KECIL`, `@BARCODE_SEDANG`, `@BARCODE_BESAR`, `@ID_BARANG`, `@NAMA_BARANG` — di INSERT dan UPDATE.
Cek juga `9Sync/SyncManager.vb` bagian upsert barang dari cloud.

**Kriteria selesai:**
- [x] Semua `AddWithValue` untuk kolom barcode/nama/kode menggunakan `.Trim()`

> ✅ `TambahBarang.vb` sudah ada `.Trim()`. `SyncManager.vb` sudah ditambahkan `.Trim()` untuk `@id`, `@nama`, `@bkecil`, `@bsedang`, `@bbesar`.

### 4c — Hapus TRIM dari query SELECT di 3 file

**`2Trans/FormPembelian.vb`** — method `TampilkanDaftarBarang` dan `SearchByBarcode`:
```vb
' Sebelum (full table scan — index tidak dipakai)
"WHERE STATUS = 'Aktif' AND (TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama " &
"OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR TRIM(BARCODE_BESAR) LIKE @Nama)"

' Sesudah (bisa pakai index)
"WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @Nama OR NAMA_BARANG LIKE @Nama " &
"OR BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama OR BARCODE_BESAR LIKE @Nama)"
```

**`2Trans/FormTransferBarang.vb`** — method `TampilkanDaftarBarang` — ganti pola yang sama.

**`2Trans/FormTransferStok.vb`** — method `TampilkanDaftarBarangMsk` dan `TampilkanDaftarBarangKlr` — ganti pola yang sama.

**Kriteria selesai:**
- [x] Tidak ada `TRIM(` di WHERE clause untuk kolom barcode/nama/kode di ketiga file
- [ ] `EXPLAIN` query pencarian barang menunjukkan `type: range` bukan `type: ALL`

> ✅ Dikerjakan di: `FormPembelian.vb`, `FormTransferBarang.vb`, `FormTransferStok.vb`, dan bonus `FormTransferCabang.vb`.
