-- ============================================================
-- MIGRATION: 32_loyalty_point_schema.sql
-- Sistem Poin Loyalitas Pelanggan
--
-- Tabel baru:
--   poin_config   — konfigurasi aturan earn poin (1 baris)
--   poin_ledger   — ledger immutable setiap transaksi poin
--   poin_barang   — harga poin per barang untuk penukaran
--
-- Kolom baru:
--   tbl_pelanggan.SALDO_POIN — saldo poin terkini (denormalisasi)
--
-- Kolom sync mengikuti pola tabel yang sudah ada:
--   Tabel master  (poin_barang) : sync_id, id_cloud, updated_by, is_dirty, version, created_at, updated_at
--   Tabel transaksi (poin_ledger): sync_id, created_at, updated_at
--   Tabel config  (poin_config) : sync_id, updated_at  (tanpa id_cloud, is_dirty, version, created_at)
-- ============================================================

DELIMITER $$

-- ── Helper: tambah kolom hanya jika belum ada ────────────────
DROP PROCEDURE IF EXISTS AddColumnSafely$$
CREATE PROCEDURE AddColumnSafely(
    IN p_table_name  VARCHAR(64),
    IN p_column_name VARCHAR(64),
    IN p_column_def  VARCHAR(500)
)
BEGIN
    DECLARE col_exists INT;
    SELECT COUNT(*) INTO col_exists
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name   = p_table_name
      AND column_name  = p_column_name;
    IF col_exists = 0 THEN
        SET @sql = CONCAT('ALTER TABLE `', p_table_name,
                          '` ADD COLUMN `', p_column_name, '` ', p_column_def);
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

-- ── Helper: tambah index hanya jika belum ada ────────────────
DROP PROCEDURE IF EXISTS AddIndexSafely$$
CREATE PROCEDURE AddIndexSafely(
    IN p_table_name  VARCHAR(64),
    IN p_index_name  VARCHAR(64),
    IN p_column_name VARCHAR(64)
)
BEGIN
    DECLARE idx_exists INT;
    SELECT COUNT(*) INTO idx_exists
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name   = p_table_name
      AND index_name   = p_index_name;
    IF idx_exists = 0 THEN
        SET @sql = CONCAT('ALTER TABLE `', p_table_name,
                          '` ADD KEY `', p_index_name,
                          '` (`', p_column_name, '`)');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

-- ── Helper: tambah unique key hanya jika belum ada ───────────
DROP PROCEDURE IF EXISTS AddUniqueKeySafely$$
CREATE PROCEDURE AddUniqueKeySafely(
    IN p_table_name  VARCHAR(64),
    IN p_index_name  VARCHAR(64),
    IN p_column_name VARCHAR(64)
)
BEGIN
    DECLARE idx_exists INT;
    SELECT COUNT(*) INTO idx_exists
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name   = p_table_name
      AND index_name   = p_index_name;
    IF idx_exists = 0 THEN
        SET @sql = CONCAT('ALTER TABLE `', p_table_name,
                          '` ADD UNIQUE KEY `', p_index_name,
                          '` (`', p_column_name, '`)');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DELIMITER ;

-- ============================================================
-- 1. TABEL poin_config
--    Satu baris konfigurasi global sistem poin.
--    Pola sync: sync_id + updated_at (tabel konfigurasi, bukan master/transaksi)
--    Tidak perlu id_cloud, is_dirty, version, created_at
-- ============================================================
CREATE TABLE IF NOT EXISTS `poin_config` (
    `ID`                int(11)        NOT NULL AUTO_INCREMENT,
    `AKTIF`             tinyint(4)     NOT NULL DEFAULT '0'
                            COMMENT '0=Tidak Aktif, 1=Aktif',
    `MEKANISME`         varchar(20)    COLLATE utf8mb4_unicode_ci
                            NOT NULL DEFAULT 'PER_ITEM'
                            COMMENT 'PER_ITEM atau PER_NOMINAL',
    `POIN_PER_QTY`      decimal(10,2)  NOT NULL DEFAULT '1.00'
                            COMMENT 'Poin per 1 qty satuan item (dipakai saat PER_ITEM)',
    `KELIPATAN_NOMINAL` decimal(15,0)  NOT NULL DEFAULT '10000'
                            COMMENT 'Nilai Rp per 1 poin (dipakai saat PER_NOMINAL)',
    `MINIMUM_REDEEM`    int(11)        NOT NULL DEFAULT '100'
                            COMMENT 'Minimum saldo poin untuk bisa redeem',
    `updated_at`        datetime       NOT NULL DEFAULT CURRENT_TIMESTAMP
                            ON UPDATE CURRENT_TIMESTAMP,
    `sync_id`           varchar(36)    COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    PRIMARY KEY (`ID`),
    UNIQUE KEY `uq_sync_id_poin_config` (`sync_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Seed baris default jika tabel masih kosong
INSERT INTO `poin_config`
    (`AKTIF`, `MEKANISME`, `POIN_PER_QTY`, `KELIPATAN_NOMINAL`, `MINIMUM_REDEEM`)
SELECT 0, 'PER_ITEM', 1.00, 10000, 100
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `poin_config` LIMIT 1);

-- ============================================================
-- 2. TABEL poin_ledger
--    Setiap baris adalah satu transaksi poin (immutable).
--    TIPE: EARN | REDEEM | VOID_EARN
--    JUMLAH_POIN: positif untuk EARN, negatif untuk REDEEM/VOID_EARN
--    NO_REFERENSI: nomor faktur penjualan atau nomor TP-YYYYMMDD-XXXX
--    Pola sync: sync_id + created_at + updated_at (seperti tabel transaksi)
-- ============================================================
CREATE TABLE IF NOT EXISTS `poin_ledger` (
    `ID`              int(11)       NOT NULL AUTO_INCREMENT,
    `KODE_PELANGGAN`  varchar(20)   COLLATE utf8mb4_unicode_ci NOT NULL
                          COMMENT 'FK ke tbl_pelanggan.KODE',
    `TIPE`            varchar(15)   COLLATE utf8mb4_unicode_ci NOT NULL
                          COMMENT 'EARN | REDEEM | VOID_EARN',
    `JUMLAH_POIN`     int(11)       NOT NULL DEFAULT '0'
                          COMMENT 'Positif=tambah, Negatif=kurang',
    `NO_REFERENSI`    varchar(30)   COLLATE utf8mb4_unicode_ci DEFAULT NULL
                          COMMENT 'Nomor faktur penjualan atau nomor penukaran TP-xxx',
    `KETERANGAN`      varchar(200)  COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    `ID_USER`         varchar(50)   COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    `ID_KOMPUTER`     varchar(50)   COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    `created_at`      datetime      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at`      datetime      NOT NULL DEFAULT CURRENT_TIMESTAMP
                          ON UPDATE CURRENT_TIMESTAMP,
    `sync_id`         varchar(36)   COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    PRIMARY KEY (`ID`),
    UNIQUE KEY `uq_sync_id_poin_ledger` (`sync_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 3. TABEL poin_barang
--    Harga poin per barang untuk penukaran di FormTukarPoin.
--    ID_BARANG: varchar(50) konsisten dengan tbl_barang.ID_BARANG
--    Pola sync: lengkap seperti tabel master (sync_id, id_cloud,
--               updated_by, is_dirty, version, created_at, updated_at)
-- ============================================================
CREATE TABLE IF NOT EXISTS `poin_barang` (
    `ID_BARANG`   varchar(50)  COLLATE utf8mb4_unicode_ci NOT NULL
                      COMMENT 'FK ke tbl_barang.ID_BARANG',
    `HARGA_POIN`  int(11)      NOT NULL DEFAULT '0'
                      COMMENT 'Jumlah poin yang dibutuhkan untuk 1 unit barang ini',
    `AKTIF`       tinyint(4)   NOT NULL DEFAULT '1'
                      COMMENT '1=Tersedia untuk ditukar, 0=Tidak tersedia',
    `updated_by`  varchar(50)  COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    `is_dirty`    tinyint(4)   NOT NULL DEFAULT '1',
    `version`     int(11)      NOT NULL DEFAULT '1',
    `id_cloud`    varchar(50)  COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    `created_at`  datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at`  datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP
                      ON UPDATE CURRENT_TIMESTAMP,
    `sync_id`     varchar(36)  COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    PRIMARY KEY (`ID_BARANG`),
    UNIQUE KEY `uq_sync_id_poin_barang` (`sync_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 4. KOLOM BARU di tbl_pelanggan
--    SALDO_POIN: saldo poin terkini (denormalisasi dari poin_ledger)
--    Tipe int(11) konsisten dengan JUMLAH_POIN di poin_ledger
--    Disisipkan AFTER `HUTANGAKHIR` (MySQL tidak mengenal BEFORE pada ADD COLUMN)
-- ============================================================
CALL AddColumnSafely(
    'tbl_pelanggan',
    'SALDO_POIN',
    "int(11) NOT NULL DEFAULT '0' COMMENT 'Saldo poin loyalitas terkini' AFTER `HUTANGAKHIR`"
);

-- ============================================================
-- 5. INDEX untuk performa query
-- ============================================================
-- poin_ledger
CALL AddIndexSafely('poin_ledger', 'idx_poin_ledger_pelanggan',  'KODE_PELANGGAN');
CALL AddIndexSafely('poin_ledger', 'idx_poin_ledger_referensi',  'NO_REFERENSI');
CALL AddIndexSafely('poin_ledger', 'idx_poin_ledger_created_at', 'created_at');
CALL AddIndexSafely('poin_ledger', 'idx_poin_ledger_updated_at', 'updated_at');

-- poin_barang
CALL AddIndexSafely('poin_barang', 'idx_poin_barang_aktif',      'AKTIF');
CALL AddIndexSafely('poin_barang', 'idx_poin_barang_is_dirty',   'is_dirty');
CALL AddIndexSafely('poin_barang', 'idx_poin_barang_id_cloud',   'id_cloud');
CALL AddIndexSafely('poin_barang', 'idx_poin_barang_updated_at', 'updated_at');

-- poin_config
CALL AddIndexSafely('poin_config', 'idx_poin_config_updated_at', 'updated_at');

-- ============================================================
-- 6. Bersihkan helper procedures
-- ============================================================
DROP PROCEDURE IF EXISTS AddColumnSafely;
DROP PROCEDURE IF EXISTS AddIndexSafely;
DROP PROCEDURE IF EXISTS AddUniqueKeySafely;
