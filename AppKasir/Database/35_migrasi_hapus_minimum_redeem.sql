-- ============================================================
-- Migrasi 35 — Hapus kolom MINIMUM_REDEEM dari poin_config
--
-- Latar belakang:
--   MINIMUM_REDEEM adalah gatekeeper global: pelanggan harus
--   punya ≥ X poin SEBELUM boleh menukar apa pun. Dalam praktiknya
--   ini redundan dengan HARGA_POIN per item — jika item termurah
--   50 poin, maka minimum efektif sudah 50 poin secara alami.
--
--   Keputusan: hapus MINIMUM_REDEEM. Validasi cukup: total poin
--   dibutuhkan (Σ qty × HARGA_POIN) ≤ saldo poin pelanggan.
--
-- Aman dijalankan berulang kali (idempoten).
-- ============================================================

-- ── 1. Hapus kolom MINIMUM_REDEEM hanya jika masih ada ──────
DELIMITER $$
DROP PROCEDURE IF EXISTS DropColumnSafely$$
CREATE PROCEDURE DropColumnSafely(
    IN p_table_name  VARCHAR(64),
    IN p_column_name VARCHAR(64)
)
BEGIN
    DECLARE col_exists INT;
    SELECT COUNT(*) INTO col_exists
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = p_table_name
      AND COLUMN_NAME  = p_column_name;
    IF col_exists > 0 THEN
        SET @sql = CONCAT('ALTER TABLE `', p_table_name,
                          '` DROP COLUMN `', p_column_name, '`');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$
DELIMITER ;
CALL DropColumnSafely('poin_config', 'MINIMUM_REDEEM');
DROP PROCEDURE IF EXISTS DropColumnSafely;

-- ── 2. Update komentar tabel ─────────────────────────────────
ALTER TABLE `poin_config`
    COMMENT='Konfigurasi aturan earn poin & data sinkronisasi';
