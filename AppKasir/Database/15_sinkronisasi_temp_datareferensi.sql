-- =============================================================================
-- 15_sinkronisasi_temp_datareferensi.sql
-- Sinkronisasi struktur temp_datareferensi agar sesuai dengan tbl_datareferensi
--
-- Perbedaan yang ditemukan (hasil DESCRIBE kedua tabel):
--   Kolom KETERANGAN  : ada di tbl_datareferensi (text), TIDAK ada di temp
--   Kolom created_at  : ada di tbl_datareferensi (datetime), TIDAK ada di temp
--   Kolom updated_at  : ada di tbl_datareferensi (datetime), TIDAK ada di temp
--   Kolom sync_id     : ada di tbl_datareferensi (varchar(36) UNIQUE), TIDAK ada di temp
--   Kolom STATUS      : varchar(10) di tbl, varchar(255) di temp → seragamkan ke varchar(10)
--
-- Aman dijalankan berulang kali — pakai IF NOT EXISTS / IF EXISTS
-- TIDAK menghapus data.
-- =============================================================================

USE db_kasirlancar;

SELECT '=== STEP 1: Cek struktur sebelum migrasi ===' AS status;

SELECT COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_KEY, COLUMN_DEFAULT, EXTRA
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'db_kasirlancar'
  AND TABLE_NAME   = 'temp_datareferensi'
ORDER BY ORDINAL_POSITION;

-- =============================================================================
-- STEP 2: Tambah kolom yang kurang (workaround MySQL 8.0.17 — tidak support IF NOT EXISTS)
-- Pakai PREPARE + information_schema agar aman dijalankan berulang kali
-- =============================================================================
SELECT '=== STEP 2: Tambah kolom yang kurang ===' AS status;

-- ── KETERANGAN ────────────────────────────────────────────────────────────────
SET @col = (SELECT COUNT(*) FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA='db_kasirlancar' AND TABLE_NAME='temp_datareferensi' AND COLUMN_NAME='KETERANGAN');
SET @sql = IF(@col=0,
    'ALTER TABLE temp_datareferensi ADD COLUMN KETERANGAN text NULL AFTER AKUN_NRLR',
    'SELECT ''Kolom KETERANGAN sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ── created_at ────────────────────────────────────────────────────────────────
SET @col = (SELECT COUNT(*) FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA='db_kasirlancar' AND TABLE_NAME='temp_datareferensi' AND COLUMN_NAME='created_at');
SET @sql = IF(@col=0,
    'ALTER TABLE temp_datareferensi ADD COLUMN created_at datetime NOT NULL DEFAULT CURRENT_TIMESTAMP AFTER SALDO_AKHIR',
    'SELECT ''Kolom created_at sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ── updated_at ────────────────────────────────────────────────────────────────
SET @col = (SELECT COUNT(*) FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA='db_kasirlancar' AND TABLE_NAME='temp_datareferensi' AND COLUMN_NAME='updated_at');
SET @sql = IF(@col=0,
    'ALTER TABLE temp_datareferensi ADD COLUMN updated_at datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP AFTER created_at',
    'SELECT ''Kolom updated_at sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ── sync_id ───────────────────────────────────────────────────────────────────
SET @col = (SELECT COUNT(*) FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA='db_kasirlancar' AND TABLE_NAME='temp_datareferensi' AND COLUMN_NAME='sync_id');
SET @sql = IF(@col=0,
    'ALTER TABLE temp_datareferensi ADD COLUMN sync_id varchar(36) NULL AFTER updated_at',
    'SELECT ''Kolom sync_id sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- =============================================================================
-- STEP 3: Seragamkan STATUS varchar(255) → varchar(10)
-- Cek dulu apakah ada data yang melebihi 10 karakter sebelum resize
-- =============================================================================
SELECT '=== STEP 3: Cek panjang aktual STATUS ===' AS status;

SELECT MAX(LENGTH(STATUS)) AS max_panjang_status, COUNT(*) AS total_baris
FROM temp_datareferensi;

ALTER TABLE temp_datareferensi
    MODIFY COLUMN STATUS varchar(10) NULL;

-- =============================================================================
-- STEP 4: Tambah UNIQUE index pada sync_id (jika belum ada)
-- =============================================================================
SELECT '=== STEP 4: Tambah UNIQUE index sync_id ===' AS status;

-- Hapus dulu jika sudah ada (hindari duplikat)
SET @idx_exists = (
    SELECT COUNT(*) FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA = 'db_kasirlancar'
      AND TABLE_NAME   = 'temp_datareferensi'
      AND INDEX_NAME   = 'sync_id'
);

SET @sql = IF(@idx_exists = 0,
    'ALTER TABLE temp_datareferensi ADD UNIQUE INDEX sync_id (sync_id)',
    'SELECT ''Index sync_id sudah ada, dilewati'' AS info'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- =============================================================================
-- STEP 5: Verifikasi struktur akhir
-- =============================================================================
SELECT '=== STEP 5: Struktur temp_datareferensi setelah migrasi ===' AS status;

SELECT COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_KEY, COLUMN_DEFAULT, EXTRA
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'db_kasirlancar'
  AND TABLE_NAME   = 'temp_datareferensi'
ORDER BY ORDINAL_POSITION;

SELECT '=== STEP 5: Bandingkan dengan tbl_datareferensi ===' AS status;

SELECT
    t.COLUMN_NAME,
    t.COLUMN_TYPE  AS tbl_type,
    tmp.COLUMN_TYPE AS temp_type,
    CASE
        WHEN tmp.COLUMN_NAME IS NULL THEN '❌ KURANG di temp'
        WHEN t.COLUMN_TYPE <> tmp.COLUMN_TYPE THEN '⚠️ BEDA tipe'
        ELSE '✅ Sama'
    END AS status_kolom
FROM (
    SELECT COLUMN_NAME, COLUMN_TYPE
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = 'db_kasirlancar' AND TABLE_NAME = 'tbl_datareferensi'
) t
LEFT JOIN (
    SELECT COLUMN_NAME, COLUMN_TYPE
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = 'db_kasirlancar' AND TABLE_NAME = 'temp_datareferensi'
) tmp ON t.COLUMN_NAME = tmp.COLUMN_NAME
ORDER BY t.COLUMN_NAME;

SELECT '=== 15_sinkronisasi_temp_datareferensi selesai ===' AS status;
