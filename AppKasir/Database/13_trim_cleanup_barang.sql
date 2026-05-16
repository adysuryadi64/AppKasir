-- =============================================================================
-- 13_trim_cleanup_barang.sql
-- Bersihkan spasi tersembunyi di kolom tbl_barang
-- Tujuan : memastikan query pencarian barang bisa memanfaatkan index
--          (TRIM() di WHERE = full table scan, tanpa TRIM = index scan)
--
-- Latar belakang (dari specs optimasi-index-database):
--   Query pencarian barang di FormPembelian, FormTransferBarang, FormTransferStok
--   menggunakan TRIM() di WHERE clause:
--     WHERE TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama ...
--   MySQL tidak bisa pakai index jika kolom dibungkus fungsi → full table scan.
--   Solusi: bersihkan data sekali, lalu hapus TRIM() dari query VB (Task 4b/4c).
--
-- URUTAN WAJIB:
--   1. Jalankan file ini dulu (bersihkan data lama)
--   2. Baru edit VB: hapus TRIM() dari query SELECT di FormPembelian,
--      FormTransferBarang, FormTransferStok, FormTransferCabang
--   3. Pastikan INSERT/UPDATE di TambahBarang.vb dan SyncManager.vb
--      sudah pakai .Trim() sebelum AddWithValue
--
-- Aman dijalankan berulang kali — UPDATE hanya menyentuh baris yang kotor.
-- Jalankan SETELAH 12_hapus_index_orphan.sql
-- =============================================================================

SELECT '=== STEP 1: Cek data kotor sebelum cleanup ===' AS status;

-- Cek berapa baris yang punya spasi di kolom kritis
SELECT
    COUNT(*) AS total_barang,
    SUM(CASE WHEN ID_BARANG      != TRIM(ID_BARANG)      THEN 1 ELSE 0 END) AS kotor_id_barang,
    SUM(CASE WHEN NAMA_BARANG    != TRIM(NAMA_BARANG)    THEN 1 ELSE 0 END) AS kotor_nama_barang,
    SUM(CASE WHEN BARCODE_KECIL  != TRIM(BARCODE_KECIL)  THEN 1 ELSE 0 END) AS kotor_barcode_kecil,
    SUM(CASE WHEN BARCODE_SEDANG != TRIM(BARCODE_SEDANG) THEN 1 ELSE 0 END) AS kotor_barcode_sedang,
    SUM(CASE WHEN BARCODE_BESAR  != TRIM(BARCODE_BESAR)  THEN 1 ELSE 0 END) AS kotor_barcode_besar,
    SUM(CASE WHEN NAMA_KATEGORI  != TRIM(NAMA_KATEGORI)  THEN 1 ELSE 0 END) AS kotor_nama_kategori,
    SUM(CASE WHEN KODE_KATEGORI  != TRIM(KODE_KATEGORI)  THEN 1 ELSE 0 END) AS kotor_kode_kategori,
    SUM(CASE WHEN NAMA_MERK      != TRIM(NAMA_MERK)      THEN 1 ELSE 0 END) AS kotor_nama_merk,
    SUM(CASE WHEN NAMA_SUPLIYER  != TRIM(NAMA_SUPLIYER)  THEN 1 ELSE 0 END) AS kotor_nama_supliyer
FROM tbl_barang;

-- =============================================================================
-- STEP 2: Bersihkan semua kolom string di tbl_barang
-- Hanya menyentuh baris yang benar-benar kotor (ada spasi) — efisien
-- =============================================================================
SELECT '=== STEP 2: Cleanup spasi di tbl_barang ===' AS status;

UPDATE tbl_barang SET
    ID_BARANG            = TRIM(ID_BARANG),
    NAMA_BARANG          = TRIM(NAMA_BARANG),
    BARCODE_KECIL        = TRIM(BARCODE_KECIL),
    BARCODE_SEDANG       = TRIM(BARCODE_SEDANG),
    BARCODE_BESAR        = TRIM(BARCODE_BESAR),
    NAMA_KATEGORI        = TRIM(NAMA_KATEGORI),
    KODE_KATEGORI        = TRIM(KODE_KATEGORI),
    NAMA_MERK            = TRIM(NAMA_MERK),
    KODE_MERK            = TRIM(KODE_MERK),
    NAMA_SUPLIYER        = TRIM(NAMA_SUPLIYER),
    KODE_SUPLIYER        = TRIM(KODE_SUPLIYER),
    SATUAN_UMUM_KECIL    = TRIM(SATUAN_UMUM_KECIL),
    SATUAN_UMUM_SEDANG   = TRIM(SATUAN_UMUM_SEDANG),
    SATUAN_UMUM_BESAR    = TRIM(SATUAN_UMUM_BESAR),
    SATUAN_PARTAI_KECIL  = TRIM(SATUAN_PARTAI_KECIL),
    SATUAN_PARTAI_SEDANG = TRIM(SATUAN_PARTAI_SEDANG),
    SATUAN_PARTAI_BESAR  = TRIM(SATUAN_PARTAI_BESAR),
    SATUAN_STOK          = TRIM(SATUAN_STOK)
WHERE ID_BARANG            != TRIM(ID_BARANG)
   OR NAMA_BARANG          != TRIM(NAMA_BARANG)
   OR BARCODE_KECIL        != TRIM(BARCODE_KECIL)
   OR BARCODE_SEDANG       != TRIM(BARCODE_SEDANG)
   OR BARCODE_BESAR        != TRIM(BARCODE_BESAR)
   OR NAMA_KATEGORI        != TRIM(NAMA_KATEGORI)
   OR KODE_KATEGORI        != TRIM(KODE_KATEGORI)
   OR NAMA_MERK            != TRIM(NAMA_MERK)
   OR KODE_MERK            != TRIM(KODE_MERK)
   OR NAMA_SUPLIYER        != TRIM(NAMA_SUPLIYER)
   OR KODE_SUPLIYER        != TRIM(KODE_SUPLIYER)
   OR SATUAN_UMUM_KECIL    != TRIM(SATUAN_UMUM_KECIL)
   OR SATUAN_UMUM_SEDANG   != TRIM(SATUAN_UMUM_SEDANG)
   OR SATUAN_UMUM_BESAR    != TRIM(SATUAN_UMUM_BESAR)
   OR SATUAN_PARTAI_KECIL  != TRIM(SATUAN_PARTAI_KECIL)
   OR SATUAN_PARTAI_SEDANG != TRIM(SATUAN_PARTAI_SEDANG)
   OR SATUAN_PARTAI_BESAR  != TRIM(SATUAN_PARTAI_BESAR)
   OR SATUAN_STOK          != TRIM(SATUAN_STOK);

SELECT ROW_COUNT() AS baris_diperbaiki;

-- =============================================================================
-- STEP 3: Verifikasi — semua kolom kritis harus 0 baris kotor
-- =============================================================================
SELECT '=== STEP 3: Verifikasi setelah cleanup ===' AS status;

SELECT
    SUM(CASE WHEN ID_BARANG      != TRIM(ID_BARANG)      THEN 1 ELSE 0 END) AS sisa_kotor_id,
    SUM(CASE WHEN NAMA_BARANG    != TRIM(NAMA_BARANG)    THEN 1 ELSE 0 END) AS sisa_kotor_nama,
    SUM(CASE WHEN BARCODE_KECIL  != TRIM(BARCODE_KECIL)  THEN 1 ELSE 0 END) AS sisa_kotor_bkecil,
    SUM(CASE WHEN BARCODE_SEDANG != TRIM(BARCODE_SEDANG) THEN 1 ELSE 0 END) AS sisa_kotor_bsedang,
    SUM(CASE WHEN BARCODE_BESAR  != TRIM(BARCODE_BESAR)  THEN 1 ELSE 0 END) AS sisa_kotor_bbesar
FROM tbl_barang;

-- Semua kolom di atas harus bernilai 0
-- Jika ada yang > 0, jangan lanjutkan ke Task 4b/4c

-- =============================================================================
-- STEP 4: Cek EXPLAIN query pencarian — pastikan pakai index setelah TRIM dihapus
-- Jalankan manual di MySQL Workbench untuk lihat execution plan
-- =============================================================================
SELECT '=== STEP 4: Contoh EXPLAIN query pencarian barang ===' AS status;

-- Contoh query SEBELUM (dengan TRIM — full table scan):
-- EXPLAIN SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang
-- WHERE STATUS = 'Aktif'
--   AND (TRIM(BARCODE_KECIL) LIKE 'ABC%' OR TRIM(NAMA_BARANG) LIKE 'ABC%');

-- Contoh query SESUDAH (tanpa TRIM — pakai index):
EXPLAIN SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang
WHERE STATUS = 'Aktif'
  AND (BARCODE_KECIL LIKE 'ABC%' OR BARCODE_SEDANG LIKE 'ABC%'
    OR BARCODE_BESAR LIKE 'ABC%' OR ID_BARANG LIKE 'ABC%'
    OR NAMA_BARANG LIKE 'ABC%');

SELECT '=== 13_trim_cleanup_barang selesai ===' AS status;
SELECT '=== Lanjutkan ke Task 4b/4c: hapus TRIM() dari query VB ===' AS status;
