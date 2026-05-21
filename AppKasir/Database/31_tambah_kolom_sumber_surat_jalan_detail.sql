-- ============================================================
-- MIGRATION: 31_tambah_kolom_sumber_surat_jalan_detail.sql
-- Menambahkan kolom SUMBER ke tabel surat_jalan_detail
-- untuk membedakan asal nota: 'Jual' (dari penjualan) atau
-- 'SO' (dari sales_order yang belum diproses jual).
--
-- Alur bisnis yang didukung:
--   1. SO → Surat Jalan → (pulang) → Proses Jual
--   2. Penjualan → Surat Jalan (alur lama, tetap berjalan)
--   Dalam satu surat jalan bisa ada campuran SO dan Jual.
-- ============================================================

DELIMITER $$

-- Helper: tambah kolom hanya jika belum ada
DROP PROCEDURE IF EXISTS AddColumnSafely$$
CREATE PROCEDURE AddColumnSafely(
    IN p_table_name VARCHAR(64),
    IN p_column_name VARCHAR(64),
    IN p_column_definition VARCHAR(500)
)
BEGIN
    DECLARE col_exists INT;

    SELECT COUNT(*) INTO col_exists
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name   = p_table_name
      AND column_name  = p_column_name;

    IF col_exists = 0 THEN
        SET @sql_stmt = CONCAT(
            'ALTER TABLE `', p_table_name,
            '` ADD COLUMN `', p_column_name, '` ',
            p_column_definition
        );
        PREPARE stmt FROM @sql_stmt;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DELIMITER ;

-- Tambah kolom SUMBER setelah kolom LOKASI
-- DEFAULT 'Jual' agar data lama otomatis dianggap dari penjualan
CALL AddColumnSafely(
    'surat_jalan_detail',
    'SUMBER',
    "varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Jual' AFTER `LOKASI`"
);

-- Bersihkan helper
DROP PROCEDURE IF EXISTS AddColumnSafely;
