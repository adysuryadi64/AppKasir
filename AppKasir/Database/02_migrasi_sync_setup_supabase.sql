-- ============================================================
-- SYNC SETUP - MySQL Lokal
-- Jalankan setelah USE nama_database;
-- ============================================================

-- 1. Tambah kolom sync ke tbl_barang
DROP PROCEDURE IF EXISTS AddSyncColumnsBarang;
DELIMITER $$
CREATE PROCEDURE AddSyncColumnsBarang()
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'id_cloud') THEN
        ALTER TABLE tbl_barang ADD COLUMN `id_cloud` VARCHAR(50) NULL DEFAULT NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'updated_at') THEN
        ALTER TABLE tbl_barang ADD COLUMN `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'updated_by') THEN
        ALTER TABLE tbl_barang ADD COLUMN `updated_by` VARCHAR(50) NULL DEFAULT NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'is_dirty') THEN
        ALTER TABLE tbl_barang ADD COLUMN `is_dirty` TINYINT NOT NULL DEFAULT 1;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'version') THEN
        ALTER TABLE tbl_barang ADD COLUMN `version` INT NOT NULL DEFAULT 1;
    END IF;
    -- Index untuk performa query sync dipindahkan ke 03_migrasi_index.sql
END$$
DELIMITER ;
CALL AddSyncColumnsBarang();
DROP PROCEDURE IF EXISTS AddSyncColumnsBarang;

-- ============================================================
-- Tambah kolom sync ke tabel master lainnya
-- (id_cloud, updated_by, is_dirty, version)
-- ============================================================
DROP PROCEDURE IF EXISTS AddSyncColumnsMaster;
DELIMITER $$
CREATE PROCEDURE AddSyncColumnsMaster(IN tbl VARCHAR(64), IN pk_col VARCHAR(64))
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'id_cloud') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `id_cloud` VARCHAR(50) NULL DEFAULT NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'updated_by') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `updated_by` VARCHAR(50) NULL DEFAULT NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'is_dirty') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `is_dirty` TINYINT NOT NULL DEFAULT 1');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'version') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `version` INT NOT NULL DEFAULT 1');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    -- Index idx_is_dirty dan idx_id_cloud dipindahkan ke 03_migrasi_index.sql
END$$
DELIMITER ;

CALL AddSyncColumnsMaster('tbl_pelanggan',  'KODE');
CALL AddSyncColumnsMaster('tbl_supliyer',   'KODE');
CALL AddSyncColumnsMaster('tbl_kategori',   'KODE');
CALL AddSyncColumnsMaster('tbl_satuan',     'KODE');
CALL AddSyncColumnsMaster('tbl_merk',       'KODE');
CALL AddSyncColumnsMaster('tbl_armada',     'KODE');
CALL AddSyncColumnsMaster('tbl_cabang',     'kode_cabang');

DROP PROCEDURE IF EXISTS AddSyncColumnsMaster;

-- 2. Tabel sync_queue
CREATE TABLE IF NOT EXISTS `sync_queue` (
    `id`          INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `aksi`        VARCHAR(10) NOT NULL COMMENT 'INSERT / UPDATE',
    `tabel`       VARCHAR(50) NOT NULL,
    `id_lokal`    VARCHAR(50) NOT NULL COMMENT 'PK lokal (ID_BARANG dll)',
    `id_cloud`    VARCHAR(50) NULL DEFAULT NULL,
    `payload`     LONGTEXT NOT NULL COMMENT 'JSON data',
    `status`      VARCHAR(10) NOT NULL DEFAULT 'pending' COMMENT 'pending/done/failed',
    `retry_count` TINYINT NOT NULL DEFAULT 0,
    `last_error`  TEXT NULL DEFAULT NULL,
    `created_at`  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at`  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_status_queue (status),
    INDEX idx_tabel_queue (tabel)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. Tabel sync_log
CREATE TABLE IF NOT EXISTS `sync_log` (
    `id`         INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `waktu`      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `jenis`      VARCHAR(20) NOT NULL COMMENT 'UPLOAD/DOWNLOAD/CONFLICT/ERROR',
    `tabel`      VARCHAR(50) NULL DEFAULT NULL,
    `id_lokal`   VARCHAR(50) NULL DEFAULT NULL,
    `id_cloud`   VARCHAR(50) NULL DEFAULT NULL,
    `pesan`      TEXT NULL DEFAULT NULL,
    INDEX idx_waktu_log (waktu),
    INDEX idx_jenis_log (jenis)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4. Tabel sync_config (simpan last_sync per tabel)
CREATE TABLE IF NOT EXISTS `sync_config` (
    `kunci`  VARCHAR(50) NOT NULL PRIMARY KEY,
    `nilai`  VARCHAR(255) NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO sync_config (kunci, nilai) VALUES
    ('last_sync_barang',    '2000-01-01 00:00:00'),
    ('last_sync_transfer',  '2000-01-01 00:00:00'),
    ('last_sync_pelanggan', '2000-01-01 00:00:00'),
    ('last_sync_supliyer',  '2000-01-01 00:00:00'),
    ('last_sync_kategori',  '2000-01-01 00:00:00'),
    ('last_sync_satuan',    '2000-01-01 00:00:00'),
    ('last_sync_merk',      '2000-01-01 00:00:00'),
    ('last_sync_armada',    '2000-01-01 00:00:00'),
    ('last_sync_cabang',    '2000-01-01 00:00:00'),
    ('kode_toko',           'TOKO1');

SELECT 'sync_setup selesai.' AS status;
