-- ============================================================
-- 03_cleanup_index.sql
-- Hapus index redundan & tambah index yang hilang — AppKasir
-- Jalankan SETELAH 03_backup, SEBELUM edit 03_migrasi_index.sql
-- Script idempotent — aman dijalankan ulang
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

-- ── tbl_barang: 2 index redundan ─────────────────────────────
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

-- ── penjualan: 8 index redundan / tanpa query pendukung ──────
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

DROP PROCEDURE IF EXISTS drop_index_if_exists;

-- ── Tambah index yang hilang ──────────────────────────────────
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

-- bon_karyawan: urutan KODE,TANGGAL,JENIS optimal untuk range TANGGAL setelah equality KODE
-- Query FormLapBonPerorang: WHERE KODE=@k AND TANGGAL<@t AND JENIS='BON'
CALL add_index_if_not_exists_tmp('bon_karyawan', 'idx_kode_tanggal_jenis_bon', 'KODE,TANGGAL,JENIS');

-- tbl_satuan: gap index untuk query di TambahSatuan.vb
-- WHERE kode = @Kode (equality) dan ORDER BY isi
CALL add_index_if_not_exists_tmp('tbl_satuan', 'idx_kode_satuan', 'kode');
CALL add_index_if_not_exists_tmp('tbl_satuan', 'idx_isi_satuan', 'isi');

DROP PROCEDURE IF EXISTS add_index_if_not_exists_tmp;

-- ── Verifikasi ────────────────────────────────────────────────
-- SHOW INDEX FROM jurnalumum;       → harus 6 index
-- SHOW INDEX FROM penjualan;        → harus 18 index
-- SHOW INDEX FROM pembelian;        → berkurang 2 (idx_jatuh_tempo_beli, idx_nama_supliyer)
-- SHOW INDEX FROM tbl_barang;       → berkurang 2 (idx_stok_toko_gudang, idx_id_barang_prefix)
-- SHOW INDEX FROM tbl_datareferensi → berkurang 1 (idx_jenis_akun)

SELECT '=== 03_cleanup_index selesai. Lanjut ke Task 3: edit 03_migrasi_index.sql ===' AS status;
