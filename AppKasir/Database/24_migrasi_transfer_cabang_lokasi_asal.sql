-- =============================================================================
-- 24_migrasi_transfer_cabang_lokasi_asal.sql
--
-- Latar belakang:
--   Saat hapus Transfer Cabang, modul HapusTransferCabang perlu tahu lokasi asal
--   ("TOKO" atau "GUDANG") untuk menentukan kolom counter yang dikurangi:
--     TRANSFER_CABANG_KELUAR_TOKO  atau  TRANSFER_CABANG_KELUAR_GUDANG
--
--   Sebelumnya lokasi dibaca dari HistoryBarang.LOKASI WHERE JENIS='TRANSFER_CABANG_KELUAR'.
--   Ini rapuh karena:
--     1. HistoryBarang.FAKTUR varchar(20) < transfer_cabang.ID_TRANSFER varchar(30)
--        → faktur bisa terpotong, query tidak menemukan data → default salah ke "TOKO"
--     2. Jika HistoryBarang dihapus manual atau corrupt → lokasi tidak diketahui
--
--   Solusi: tambah kolom LOKASI_ASAL varchar(10) di transfer_cabang dan
--   transfer_cabang_detail, diisi saat INSERT dengan nilai "TOKO" atau "GUDANG".
--   Modul hapus membaca LOKASI_ASAL sebagai sumber primer, HistoryBarang sebagai fallback.
--
-- Perubahan terkait di kode VB.NET:
--   - FormTransferCabang.SimpanArsipTransferLokal: tambah @lokasiAsal ke INSERT
--   - ModuleHapusTransaksi.HapusTransferCabang: baca LOKASI_ASAL dulu, fallback HistoryBarang
--
-- Perubahan terkait di file migrasi lain:
--   - 14_resize_varchar.sql: historybarang.FAKTUR diperlebar varchar(20)→varchar(30)
--
-- Aman dijalankan berulang kali — IF NOT EXISTS mencegah error jika kolom sudah ada.
-- Tidak menghapus data. Data lama (LOKASI_ASAL = NULL) ditangani oleh fallback di kode.
-- =============================================================================

SELECT '=== 24: Tambah LOKASI_ASAL di transfer_cabang ===' AS status;

-- ── transfer_cabang (header) ──────────────────────────────────────────────────
-- Cek dulu apakah kolom sudah ada sebelum ALTER
SET @dbname = DATABASE();
SET @tblname = 'transfer_cabang';
SET @colname = 'LOKASI_ASAL';

SELECT IF(
    EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = @dbname
          AND TABLE_NAME   = @tblname
          AND COLUMN_NAME  = @colname
    ),
    'SKIP — kolom LOKASI_ASAL sudah ada di transfer_cabang',
    'AKAN ditambahkan'
) AS cek_kolom_header;

-- ALTER hanya jalan jika kolom belum ada
-- (MySQL tidak support IF NOT EXISTS untuk ADD COLUMN sebelum 8.0 — pakai stored proc)
DROP PROCEDURE IF EXISTS sp_tmp_add_lokasi_asal_header;
DELIMITER //
CREATE PROCEDURE sp_tmp_add_lokasi_asal_header()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'transfer_cabang'
          AND COLUMN_NAME  = 'LOKASI_ASAL'
    ) THEN
        ALTER TABLE transfer_cabang
            ADD COLUMN LOKASI_ASAL VARCHAR(10) NULL DEFAULT NULL
            AFTER LOKASI;
    END IF;
END //
DELIMITER ;
CALL sp_tmp_add_lokasi_asal_header();
DROP PROCEDURE IF EXISTS sp_tmp_add_lokasi_asal_header;

-- ── transfer_cabang_detail ────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_tmp_add_lokasi_asal_detail;
DELIMITER //
CREATE PROCEDURE sp_tmp_add_lokasi_asal_detail()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'transfer_cabang_detail'
          AND COLUMN_NAME  = 'LOKASI_ASAL'
    ) THEN
        ALTER TABLE transfer_cabang_detail
            ADD COLUMN LOKASI_ASAL VARCHAR(10) NULL DEFAULT NULL
            AFTER LOKASI;
    END IF;
END //
DELIMITER ;
CALL sp_tmp_add_lokasi_asal_detail();
DROP PROCEDURE IF EXISTS sp_tmp_add_lokasi_asal_detail;

-- ── Verifikasi ────────────────────────────────────────────────────────────────
SELECT '=== Verifikasi kolom setelah migrasi ===' AS status;

SELECT
    TABLE_NAME AS tabel,
    COLUMN_NAME AS kolom,
    COLUMN_TYPE AS tipe,
    IS_NULLABLE AS nullable,
    COLUMN_DEFAULT AS default_val
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('transfer_cabang', 'transfer_cabang_detail')
  AND COLUMN_NAME IN ('LOKASI', 'LOKASI_ASAL')
ORDER BY TABLE_NAME, ORDINAL_POSITION;

SELECT '=== 24_migrasi_transfer_cabang_lokasi_asal selesai ===' AS status;
