-- ============================================================
-- Migrasi tabel penjualan
-- Hanya menambahkan kolom yang belum ada
-- Aman dijalankan berulang kali
-- Jalankan setelah memilih database yang benar (USE nama_db;)
-- ============================================================

-- 1. Tambah kolom NOMINAL_TRANSFER (penjualan)
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'penjualan'
      AND COLUMN_NAME  = 'NOMINAL_TRANSFER'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE penjualan ADD COLUMN `NOMINAL_TRANSFER` decimal(15,2) DEFAULT ''0.00'' AFTER `BAYAR`',
    'SELECT ''NOMINAL_TRANSFER sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 2. Tambah kolom KODE_AKUN_TF (penjualan)
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'penjualan'
      AND COLUMN_NAME  = 'KODE_AKUN_TF'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE penjualan ADD COLUMN `KODE_AKUN_TF` varchar(20) NOT NULL DEFAULT '''' AFTER `JENIS_PEMBAYARAN`',
    'SELECT ''KODE_AKUN_TF sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 3. Tambah kolom NAMA_AKUN_TF (penjualan)
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'penjualan'
      AND COLUMN_NAME  = 'NAMA_AKUN_TF'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE penjualan ADD COLUMN `NAMA_AKUN_TF` varchar(50) NOT NULL DEFAULT '''' AFTER `KODE_AKUN_TF`',
    'SELECT ''NAMA_AKUN_TF sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ============================================================
-- Migrasi tabel pembelian — kolom split bayar
-- NOMINAL_TRANSFER : nominal bayar via transfer/non-tunai
-- KODE_AKUN_TF     : kode akun rekening tujuan transfer
-- NAMA_AKUN_TF     : nama akun rekening tujuan transfer
-- ============================================================

-- 4. Tambah kolom NOMINAL_TRANSFER (pembelian)
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'pembelian'
      AND COLUMN_NAME  = 'NOMINAL_TRANSFER'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE pembelian ADD COLUMN `NOMINAL_TRANSFER` decimal(15,2) DEFAULT ''0.00'' AFTER `PEMBAYARAN`',
    'SELECT ''pembelian.NOMINAL_TRANSFER sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 5. Tambah kolom KODE_AKUN_TF (pembelian)
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'pembelian'
      AND COLUMN_NAME  = 'KODE_AKUN_TF'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE pembelian ADD COLUMN `KODE_AKUN_TF` varchar(20) NOT NULL DEFAULT '''' AFTER `JENIS_BAYAR`',
    'SELECT ''pembelian.KODE_AKUN_TF sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 6. Tambah kolom NAMA_AKUN_TF (pembelian)
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'pembelian'
      AND COLUMN_NAME  = 'NAMA_AKUN_TF'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE pembelian ADD COLUMN `NAMA_AKUN_TF` varchar(50) NOT NULL DEFAULT '''' AFTER `KODE_AKUN_TF`',
    'SELECT ''pembelian.NAMA_AKUN_TF sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom split bayar pembelian selesai.' AS status;

-- ============================================================
-- Opsional: sesuaikan tipe decimal(15,0) -> decimal(15,2)
-- Kolom: TOTAL_HPP, GRAND_TOTAL_SBL_PAJAK, GRAND_TOTAL_STL_PAJAK,
--        BAYAR, KEMBALI, NOMINALBAYARPIUTANG, SISA_TAGIHAN
-- Uncomment blok di bawah jika ingin menyamakan presisi desimal
-- ============================================================

ALTER TABLE penjualan
    MODIFY COLUMN `TOTAL_HPP`              decimal(15,2) DEFAULT '0.00',
    MODIFY COLUMN `GRAND_TOTAL_SBL_PAJAK`  decimal(15,2) DEFAULT '0.00',
    MODIFY COLUMN `GRAND_TOTAL_STL_PAJAK`  decimal(15,2) DEFAULT '0.00',
    MODIFY COLUMN `BAYAR`                  decimal(15,2) DEFAULT '0.00',
    MODIFY COLUMN `KEMBALI`                decimal(15,2) DEFAULT '0.00',
    MODIFY COLUMN `NOMINALBAYARPIUTANG`    decimal(15,2) DEFAULT '0.00',
    MODIFY COLUMN `SISA_TAGIHAN`           decimal(15,2) DEFAULT '0.00';



-- ============================================================
-- Migrasi tabel tbl_user: tambah kolom status (Aktif/Non Aktif)
-- Aman dijalankan berulang kali
-- ============================================================

-- 4. Tambah kolom status di tbl_user
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_user'
      AND COLUMN_NAME  = 'status'
);
SET @sql = IF(@col = 0,
    'ALTER TABLE tbl_user ADD COLUMN `status` varchar(10) NOT NULL DEFAULT ''Aktif'' AFTER `lvl`',
    'SELECT ''status sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi selesai.' AS status;

-- ============================================================
-- Schema lokal cabang + transfer antar cabang (baseline)
-- Penting: dibuat sebelum migrasi timestamp/sync kolom berjalan
-- supaya instalasi DB baru tidak gagal saat ALTER TABLE.
-- ============================================================

CREATE TABLE IF NOT EXISTS `tbl_cabang` (
  `kode_cabang` VARCHAR(50) NOT NULL,
  `nama_cabang` VARCHAR(100) DEFAULT NULL,
  `alamat` VARCHAR(200) DEFAULT NULL,
  `kota` VARCHAR(60) DEFAULT NULL,
  `hp` VARCHAR(60) DEFAULT NULL,
  `pemilik` VARCHAR(100) DEFAULT NULL,
  `sumber` VARCHAR(20) NOT NULL DEFAULT 'manual',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`kode_cabang`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `transfer_cabang` (
  `ID_TRANSFER` VARCHAR(30) NOT NULL,
  `TGL_TRANSFER` DATETIME DEFAULT NULL,
  `LOKASI` VARCHAR(100) DEFAULT NULL,
  `DARI_CABANG` VARCHAR(50) DEFAULT NULL,
  `KE_CABANG` VARCHAR(50) DEFAULT NULL,
  `MODE_KIRIM` VARCHAR(20) NOT NULL DEFAULT 'OFFLINE_EXPORT',
  `STATUS_TRANSFER` VARCHAR(20) NOT NULL DEFAULT 'PENDING',
  `ID_CLOUD_TRANSFER` VARCHAR(50) DEFAULT NULL,
  `FILE_MANUAL` VARCHAR(255) DEFAULT NULL,
  `TOTAL_QTY` DECIMAL(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` INT DEFAULT '0',
  `TOTAL_RUPIAH` DECIMAL(15,2) DEFAULT '0.00',
  `ID_USER` VARCHAR(50) DEFAULT NULL,
  `ID_KOMPUTER` VARCHAR(50) DEFAULT NULL,
  PRIMARY KEY (`ID_TRANSFER`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `transfer_cabang_detail` (
  `ID_TRANSFER` VARCHAR(30) NOT NULL,
  `TGL_TRANSFER` DATETIME DEFAULT NULL,
  `LOKASI` VARCHAR(120) DEFAULT NULL,
  `ID_BARANG` VARCHAR(30) NOT NULL,
  `NAMA_BARANG` VARCHAR(150) DEFAULT NULL,
  `HARGA` DECIMAL(15,2) DEFAULT '0.00',
  `QTY` DECIMAL(10,2) DEFAULT '0.00',
  `SATUAN` VARCHAR(20) DEFAULT NULL,
  `ISI_SATUAN` DECIMAL(10,2) DEFAULT '1.00',
  `HARGA_QTY` DECIMAL(15,2) DEFAULT '0.00',
  `TOTAL_QTY` DECIMAL(10,2) DEFAULT '0.00',
  `DITERIMA_QTY` DECIMAL(10,2) DEFAULT '0.00',
  `STATUS_ITEM` VARCHAR(20) NOT NULL DEFAULT 'PENDING',
  `TOTAL` DECIMAL(15,2) DEFAULT '0.00',
  `ID_USER` VARCHAR(50) DEFAULT NULL,
  `ID_KOMPUTER` VARCHAR(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `transfer_masuk_manual` (
  `id`              INT AUTO_INCREMENT PRIMARY KEY,
  `id_transfer`     VARCHAR(30)  NOT NULL,
  `sumber_transfer` VARCHAR(20)  NOT NULL DEFAULT 'MANUAL',
  `id_cloud`        VARCHAR(50)  NULL DEFAULT NULL,
  `dari_cabang`     VARCHAR(50)  DEFAULT NULL,
  `ke_cabang`       VARCHAR(50)  DEFAULT NULL,
  `kode_barang`     VARCHAR(50)  NOT NULL,
  `nama_barang`     VARCHAR(150) DEFAULT NULL,
  `qty`             DECIMAL(10,2) DEFAULT '0.00',
  `satuan`          VARCHAR(20)  DEFAULT NULL,
  `isi_satuan`      INT          DEFAULT 1,
  `qty_satuan`      DECIMAL(10,2) DEFAULT '0.00',
  `harga_beli`      DECIMAL(15,2) NOT NULL DEFAULT '0.00',
  `keterangan`      VARCHAR(255) DEFAULT NULL,
  `tgl_kirim`       DATETIME     NULL DEFAULT NULL,
  `tgl_terima`      DATETIME     NULL DEFAULT NULL,
  `id_user_terima`  VARCHAR(50)  NULL DEFAULT NULL,
  `catatan_terima`  VARCHAR(255) NULL DEFAULT NULL,
  `status_transfer` VARCHAR(20)  NOT NULL DEFAULT 'PENDING',
  `created_at`      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_status` (`status_transfer`),
  INDEX `idx_id_cloud` (`id_cloud`),
  UNIQUE KEY `uk_id_cloud` (`id_cloud`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SELECT 'Schema lokal cabang & transfer antar cabang siap.' AS status;

-- ============================================================
-- Migrasi timestamp untuk sinkronisasi offline -> cloud
-- Tambah created_at + updated_at pada tabel transaksi & master
-- Aman dijalankan berulang kali
-- ============================================================

-- Helper procedure untuk tambah timestamp columns
DROP PROCEDURE IF EXISTS AddTimestampColumns;
DELIMITER $$
CREATE PROCEDURE AddTimestampColumns(IN tbl VARCHAR(64))
BEGIN
    -- created_at
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'created_at'
    ) THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;

    -- updated_at
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'updated_at'
    ) THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
END$$
DELIMITER ;

-- Tabel transaksi utama
CALL AddTimestampColumns('penjualan');
CALL AddTimestampColumns('penjualan_detail');
CALL AddTimestampColumns('pembelian');
CALL AddTimestampColumns('pembelian_detail');
CALL AddTimestampColumns('retur_pembelian');
CALL AddTimestampColumns('retur_pembelian_detail');
CALL AddTimestampColumns('retur_penjualan');
CALL AddTimestampColumns('retur_penjualan_detail');
CALL AddTimestampColumns('hutang');
CALL AddTimestampColumns('hutang_detail');
CALL AddTimestampColumns('piutang');
CALL AddTimestampColumns('piutang_detail');
CALL AddTimestampColumns('jurnalumum');
CALL AddTimestampColumns('bon_karyawan');
CALL AddTimestampColumns('gaji_karyawan');
CALL AddTimestampColumns('stoktambahkurang');
CALL AddTimestampColumns('stok_opname');
CALL AddTimestampColumns('transfer_barang');
CALL AddTimestampColumns('transfer_barang_detail');
CALL AddTimestampColumns('transfer_cabang');
CALL AddTimestampColumns('transfer_cabang_detail');
CALL AddTimestampColumns('transfer_stok');
CALL AddTimestampColumns('surat_jalan');
CALL AddTimestampColumns('surat_jalan_detail');

-- Tabel master yang sering berubah
CALL AddTimestampColumns('tbl_barang');
CALL AddTimestampColumns('tbl_pelanggan');
CALL AddTimestampColumns('tbl_supliyer');
CALL AddTimestampColumns('tbl_karyawan');
CALL AddTimestampColumns('tbl_user');
CALL AddTimestampColumns('hakaksesuser');
CALL AddTimestampColumns('tbl_datareferensi');
CALL AddTimestampColumns('history');
CALL AddTimestampColumns('historybarang');
-- Tabel master yang disync ke cloud
CALL AddTimestampColumns('tbl_kategori');
CALL AddTimestampColumns('tbl_satuan');
CALL AddTimestampColumns('tbl_merk');
CALL AddTimestampColumns('tbl_armada');

-- Bersihkan procedure setelah selesai
DROP PROCEDURE IF EXISTS AddTimestampColumns;

-- Index updated_at dipindahkan ke 03_migrasi_index.sql

SELECT 'Migrasi timestamp selesai. Siap untuk sinkronisasi cloud.' AS status;

-- ============================================================
-- Migrasi sync_id (UUID) untuk identifikasi unik global
-- Dipakai sebagai primary key di Supabase/cloud
-- Primary key lokal TIDAK diubah
-- Aman dijalankan berulang kali
-- ============================================================

DROP PROCEDURE IF EXISTS AddSyncId;
DELIMITER $$
CREATE PROCEDURE AddSyncId(IN tbl VARCHAR(64))
BEGIN
    -- Tambah kolom sync_id jika belum ada
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'sync_id'
    ) THEN
        SET @s = CONCAT(
            'ALTER TABLE `', tbl, '`',
            ' ADD COLUMN `sync_id` VARCHAR(36) NULL DEFAULT NULL,',
            ' ADD UNIQUE INDEX `uq_sync_id_', tbl, '` (`sync_id`)'
        );
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;

        -- Isi sync_id untuk data yang sudah ada dengan UUID
        SET @s = CONCAT('UPDATE `', tbl, '` SET `sync_id` = UUID() WHERE `sync_id` IS NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
END$$
DELIMITER ;

-- Tabel transaksi utama
CALL AddSyncId('penjualan');
CALL AddSyncId('penjualan_detail');
CALL AddSyncId('pembelian');
CALL AddSyncId('pembelian_detail');
CALL AddSyncId('retur_pembelian');
CALL AddSyncId('retur_pembelian_detail');
CALL AddSyncId('retur_penjualan');
CALL AddSyncId('retur_penjualan_detail');
CALL AddSyncId('hutang');
CALL AddSyncId('hutang_detail');
CALL AddSyncId('piutang');
CALL AddSyncId('piutang_detail');
CALL AddSyncId('jurnalumum');
CALL AddSyncId('bon_karyawan');
CALL AddSyncId('gaji_karyawan');
CALL AddSyncId('stoktambahkurang');
CALL AddSyncId('stok_opname');
CALL AddSyncId('transfer_barang');
CALL AddSyncId('transfer_barang_detail');
CALL AddSyncId('transfer_cabang');
CALL AddSyncId('transfer_cabang_detail');
CALL AddSyncId('transfer_stok');
CALL AddSyncId('surat_jalan');
CALL AddSyncId('surat_jalan_detail');

-- Tabel master
CALL AddSyncId('tbl_barang');
CALL AddSyncId('tbl_pelanggan');
CALL AddSyncId('tbl_supliyer');
CALL AddSyncId('tbl_karyawan');
CALL AddSyncId('tbl_user');
CALL AddSyncId('hakaksesuser');
CALL AddSyncId('tbl_datareferensi');
CALL AddSyncId('history');
CALL AddSyncId('historybarang');
-- Tabel master yang disync ke cloud
CALL AddSyncId('tbl_kategori');
CALL AddSyncId('tbl_satuan');
CALL AddSyncId('tbl_merk');
CALL AddSyncId('tbl_armada');

DROP PROCEDURE IF EXISTS AddSyncId;

-- ============================================================
-- Tambah kolom sync (id_cloud, updated_by, is_dirty, version)
-- ke tabel master yang disync ke cloud
-- Aman dijalankan berulang kali
-- ============================================================
DROP PROCEDURE IF EXISTS AddSyncColsMaster;
DELIMITER $$
CREATE PROCEDURE AddSyncColsMaster(IN tbl VARCHAR(64))
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'id_cloud') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `id_cloud` VARCHAR(50) NULL DEFAULT NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'updated_by') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `updated_by` VARCHAR(50) NULL DEFAULT NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'is_dirty') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `is_dirty` TINYINT NOT NULL DEFAULT 1');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = tbl AND COLUMN_NAME = 'version') THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` ADD COLUMN `version` INT NOT NULL DEFAULT 1');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    -- Index idx_id_cloud dan idx_is_dirty dipindahkan ke 03_migrasi_index.sql
END$$
DELIMITER ;

-- tbl_barang sudah ditangani sync_setup.sql, tapi panggil juga agar idempotent
CALL AddSyncColsMaster('tbl_barang');
CALL AddSyncColsMaster('tbl_pelanggan');
CALL AddSyncColsMaster('tbl_supliyer');
CALL AddSyncColsMaster('tbl_kategori');
CALL AddSyncColsMaster('tbl_satuan');
CALL AddSyncColsMaster('tbl_merk');
CALL AddSyncColsMaster('tbl_armada');

DROP PROCEDURE IF EXISTS AddSyncColsMaster;

SELECT 'Migrasi sync_id selesai.' AS status;

-- ============================================================
-- Hapus kolom NAMA_REK_A s/d NAMA_REK_F dari tbl_perusahaan
-- Kolom ini tidak dipakai di aplikasi (tidak ada UI, tidak ada
-- query INSERT/UPDATE yang menyentuhnya).
-- Aman dijalankan berulang kali.
-- ============================================================
DROP PROCEDURE IF EXISTS DropColIfExists;
DELIMITER $
CREATE PROCEDURE DropColIfExists(IN tbl VARCHAR(64), IN col VARCHAR(64))
BEGIN
    IF EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND COLUMN_NAME  = col
    ) THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` DROP COLUMN `', col, '`');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
END$
DELIMITER ;

CALL DropColIfExists('tbl_perusahaan', 'NAMA_REK_A');
CALL DropColIfExists('tbl_perusahaan', 'KODE_REK_A');
CALL DropColIfExists('tbl_perusahaan', 'NAMA_REK_B');
CALL DropColIfExists('tbl_perusahaan', 'KODE_REK_B');
CALL DropColIfExists('tbl_perusahaan', 'NAMA_REK_C');
CALL DropColIfExists('tbl_perusahaan', 'KODE_REK_C');
CALL DropColIfExists('tbl_perusahaan', 'NAMA_REK_D');
CALL DropColIfExists('tbl_perusahaan', 'KODE_REK_D');
CALL DropColIfExists('tbl_perusahaan', 'NAMA_REK_E');
CALL DropColIfExists('tbl_perusahaan', 'KODE_REK_E');
CALL DropColIfExists('tbl_perusahaan', 'NAMA_REK_F');
CALL DropColIfExists('tbl_perusahaan', 'KODE_REK_F');

DROP PROCEDURE IF EXISTS DropColIfExists;

SELECT 'Kolom NAMA/KODE_REK_A-F berhasil dihapus (jika ada).' AS status;

-- ============================================================
-- Tambah 6 kolom rekening akun default baru di tbl_perusahaan
-- (Retur Pembelian Toko/Gudang, Retur Penjualan Toko/Gudang,
--  Bon Karyawan, Gaji Karyawan)
-- Aman dijalankan berulang kali.
-- ============================================================
DROP PROCEDURE IF EXISTS AddRekPerusahaan;
DELIMITER $
CREATE PROCEDURE AddRekPerusahaan(IN colNama VARCHAR(64), IN colKode VARCHAR(64))
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_perusahaan' AND COLUMN_NAME = colNama) THEN
        SET @s = CONCAT('ALTER TABLE `tbl_perusahaan` ADD COLUMN `', colNama, '` VARCHAR(50) NULL DEFAULT NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_perusahaan' AND COLUMN_NAME = colKode) THEN
        SET @s = CONCAT('ALTER TABLE `tbl_perusahaan` ADD COLUMN `', colKode, '` VARCHAR(20) NULL DEFAULT NULL');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
END$
DELIMITER ;

CALL AddRekPerusahaan('NAMA_REK_RETUR_PEMBELIAN_TOKO',   'KODE_REK_RETUR_PEMBELIAN_TOKO');
CALL AddRekPerusahaan('NAMA_REK_RETUR_PENJUALAN_TOKO',   'KODE_REK_RETUR_PENJUALAN_TOKO');
CALL AddRekPerusahaan('NAMA_REK_RETUR_PEMBELIAN_GUDANG', 'KODE_REK_RETUR_PEMBELIAN_GUDANG');
CALL AddRekPerusahaan('NAMA_REK_RETUR_PENJUALAN_GUDANG', 'KODE_REK_RETUR_PENJUALAN_GUDANG');
CALL AddRekPerusahaan('NAMA_REK_BON_KARYAWAN',           'KODE_REK_BON_KARYAWAN');
CALL AddRekPerusahaan('NAMA_REK_GAJI_KARYAWAN',          'KODE_REK_GAJI_KARYAWAN');
CALL AddRekPerusahaan('NAMA_REK_BAYAR_HUTANG',           'KODE_REK_BAYAR_HUTANG');
CALL AddRekPerusahaan('NAMA_REK_BAYAR_PIUTANG',          'KODE_REK_BAYAR_PIUTANG');
CALL AddRekPerusahaan('NAMA_REK_TRANSFER_JUAL',          'KODE_REK_TRANSFER_JUAL');

DROP PROCEDURE IF EXISTS AddRekPerusahaan;

SELECT 'Migrasi kolom rekening akun default selesai.' AS status;

-- ============================================================
-- Set KODE sebagai PRIMARY KEY di tbl_perusahaan
-- Sebelumnya kolom ini DEFAULT NULL tanpa constraint apapun.
-- Aman dijalankan berulang kali.
-- ============================================================

-- 1. Pastikan tidak ada nilai NULL di KODE sebelum set NOT NULL
UPDATE `tbl_perusahaan` SET `KODE` = 'DEFAULT' WHERE `KODE` IS NULL OR `KODE` = '';

-- 2. Ubah kolom KODE menjadi NOT NULL
ALTER TABLE `tbl_perusahaan`
    MODIFY COLUMN `KODE` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

-- 3. Tambah PRIMARY KEY jika belum ada
SET @hasPK = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_perusahaan'
      AND CONSTRAINT_TYPE = 'PRIMARY KEY'
);
SET @sql = IF(@hasPK = 0,
    'ALTER TABLE `tbl_perusahaan` ADD PRIMARY KEY (`KODE`)',
    'SELECT ''PRIMARY KEY tbl_perusahaan sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi PRIMARY KEY tbl_perusahaan selesai.' AS status;

-- ============================================================
-- Pindahkan SYSTEM_TUTUP_BULAN dan TANGGAL_TUTUP_BULAN
-- ke setelah kolom FOOTER3 di tbl_perusahaan
-- ============================================================
ALTER TABLE `tbl_perusahaan`
    MODIFY COLUMN `SYSTEM_TUTUP_BULAN`  VARCHAR(50)  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL AFTER `FOOTER3`,
    MODIFY COLUMN `TANGGAL_TUTUP_BULAN` SMALLINT(6)  DEFAULT '0' AFTER `SYSTEM_TUTUP_BULAN`;

SELECT 'Pindah kolom SYSTEM_TUTUP_BULAN dan TANGGAL_TUTUP_BULAN selesai.' AS status;

-- ============================================================
-- Tambah kolom identitas cloud di tbl_perusahaan
-- KODE_CLOUD  : kode unik cabang untuk Supabase (bisa beda dengan KODE lokal)
-- NAMA_CLOUD  : nama cabang yang ditampilkan di laporan cloud
-- ============================================================
DROP PROCEDURE IF EXISTS AddCloudCols;
DELIMITER $
CREATE PROCEDURE AddCloudCols()
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_perusahaan' AND COLUMN_NAME = 'KODE_CLOUD') THEN
        ALTER TABLE `tbl_perusahaan` ADD COLUMN `KODE_CLOUD` VARCHAR(50) NULL DEFAULT NULL AFTER `KODE`;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_perusahaan' AND COLUMN_NAME = 'NAMA_CLOUD') THEN
        ALTER TABLE `tbl_perusahaan` ADD COLUMN `NAMA_CLOUD` VARCHAR(100) NULL DEFAULT NULL AFTER `KODE_CLOUD`;
    END IF;
END$
DELIMITER ;
CALL AddCloudCols();
DROP PROCEDURE IF EXISTS AddCloudCols;

SELECT 'Migrasi kolom KODE_CLOUD dan NAMA_CLOUD selesai.' AS status;

-- ============================================================
-- Tambah kolom ALAMAT_CLOUD di tbl_perusahaan
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_perusahaan' AND COLUMN_NAME = 'ALAMAT_CLOUD');
SET @sql = IF(@col = 0,
    'ALTER TABLE `tbl_perusahaan` ADD COLUMN `ALAMAT_CLOUD` VARCHAR(200) NULL DEFAULT NULL AFTER `NAMA_CLOUD`',
    'SELECT ''ALAMAT_CLOUD sudah ada'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ============================================================
-- Tambah kolom KODE_MERK dan NAMA_MERK ke tbl_barang
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'KODE_MERK');
SET @sql = IF(@col = 0,
    'ALTER TABLE `tbl_barang` ADD COLUMN `KODE_MERK` VARCHAR(10) NULL DEFAULT NULL AFTER `NAMA_SUPLIYER`',
    'SELECT ''KODE_MERK sudah ada'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'NAMA_MERK');
SET @sql = IF(@col = 0,
    'ALTER TABLE `tbl_barang` ADD COLUMN `NAMA_MERK` VARCHAR(20) NULL DEFAULT NULL AFTER `KODE_MERK`',
    'SELECT ''NAMA_MERK sudah ada'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom KODE_MERK dan NAMA_MERK selesai.' AS status;

-- ============================================================
-- Tambah kolom STATUS ke tbl_barang (Aktif / Non Aktif)
-- Posisi: setelah KOMISI_SALES_PERSEN
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'STATUS');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `STATUS` VARCHAR(10) NOT NULL DEFAULT 'Aktif' AFTER `KOMISI_SALES_PERSEN`",
    "SELECT 'STATUS sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom STATUS tbl_barang selesai.' AS status;

-- ============================================================
-- Tambah kolom Status ke tbl_supliyer (Aktif / Nonaktif)
-- Supplier dengan hutang tidak bisa dihapus, hanya dinonaktifkan
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_supliyer' AND COLUMN_NAME = 'Status');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_supliyer` ADD COLUMN `Status` VARCHAR(10) NOT NULL DEFAULT 'Aktif' AFTER `HutangAkhir`",
    "SELECT 'Status tbl_supliyer sudah ada, dilewati' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom Status tbl_supliyer selesai.' AS status;

-- ============================================================
-- Tambah kolom Status ke tbl_pelanggan (Aktif / Nonaktif)
-- Pelanggan dengan hutang tidak bisa dihapus, hanya dinonaktifkan
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_pelanggan' AND COLUMN_NAME = 'Status');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_pelanggan` ADD COLUMN `Status` VARCHAR(10) NOT NULL DEFAULT 'Aktif' AFTER `HutangAkhir`",
    "SELECT 'Status tbl_pelanggan sudah ada, dilewati' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom Status tbl_pelanggan selesai.' AS status;

-- ============================================================
-- Tambah kolom Status ke tbl_karyawan (Aktif / Nonaktif)
-- Karyawan dengan saldo bon tidak bisa dihapus, hanya dinonaktifkan
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_karyawan' AND COLUMN_NAME = 'Status');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_karyawan` ADD COLUMN `Status` VARCHAR(10) NOT NULL DEFAULT 'Aktif' AFTER `SaldoAkhir`",
    "SELECT 'Status tbl_karyawan sudah ada, dilewati' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom Status tbl_karyawan selesai.' AS status;

-- ============================================================
-- Tambah kolom LOKASI ke penjualan_ditahan
-- Menyimpan lokasi (TOKO/GUDANG) saat transaksi ditahan,
-- agar saat dipanggil kembali stok yang dipakai sesuai lokasi asal.
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'penjualan_ditahan'
      AND COLUMN_NAME  = 'LOKASI');
SET @sql = IF(@col = 0,
    "ALTER TABLE `penjualan_ditahan` ADD COLUMN `LOKASI` VARCHAR(20) NULL DEFAULT NULL AFTER `TANGGAL_JUAL`",
    "SELECT 'penjualan_ditahan.LOKASI sudah ada, dilewati' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom LOKASI penjualan_ditahan selesai.' AS status;

-- ============================================================
-- Ubah kolom Aksi di tabel History menjadi TEXT
-- Diperlukan karena audit jurnal dan stok menghasilkan string
-- yang bisa melebihi batas VARCHAR
-- Aman dijalankan berulang kali.
-- ============================================================
SET @tipe = (
    SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'History'
      AND COLUMN_NAME  = 'Aksi'
);
SET @sql = IF(@tipe <> 'text',
    'ALTER TABLE History MODIFY COLUMN Aksi TEXT',
    'SELECT ''Aksi sudah TEXT, dilewati'' AS info'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom Aksi History ke TEXT selesai.' AS status;

-- ============================================================
-- Perbesar kolom FOOTER1, FOOTER2, FOOTER3 di tbl_perusahaan
-- Diperlukan karena footer kini mendukung multi-baris (enter)
-- FOOTER1 & FOOTER2: varchar(50)  → varchar(255)
-- FOOTER3          : varchar(100) → varchar(255)
-- Aman dijalankan berulang kali.
-- ============================================================
ALTER TABLE `tbl_perusahaan`
    MODIFY COLUMN `FOOTER1` VARCHAR(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    MODIFY COLUMN `FOOTER2` VARCHAR(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
    MODIFY COLUMN `FOOTER3` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;

SELECT 'Migrasi kolom FOOTER1/2/3 selesai.' AS status;

-- ============================================================
-- Bersihkan kolom pembayaran pada pembelian_ditahan
-- Draft pembelian tidak menyimpan data pembayaran.
-- Aman dijalankan berulang kali.
-- ============================================================
DROP PROCEDURE IF EXISTS DropColIfExists;
DELIMITER $
CREATE PROCEDURE DropColIfExists(IN tbl VARCHAR(64), IN col VARCHAR(64))
BEGIN
    IF EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND COLUMN_NAME  = col
    ) THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` DROP COLUMN `', col, '`');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
END$
DELIMITER ;

CALL DropColIfExists('pembelian_ditahan', 'PEMBAYARAN');
CALL DropColIfExists('pembelian_ditahan', 'TAGIHAN');
CALL DropColIfExists('pembelian_ditahan', 'JATUH_TEMPO');
CALL DropColIfExists('pembelian_ditahan', 'TGL_BAYAR');
CALL DropColIfExists('pembelian_ditahan', 'NOMINALBAYAR');
CALL DropColIfExists('pembelian_ditahan', 'STATUS_JUAL');
CALL DropColIfExists('pembelian_ditahan', 'STATUS_TRANSAKSI_BELI');

DROP PROCEDURE IF EXISTS DropColIfExists;

SELECT 'Cleanup kolom pembayaran pembelian_ditahan selesai.' AS status;

-- ============================================================
-- Kolom lokal untuk transfer antar cabang (cloud/offline)
-- Aman dijalankan berulang kali.
-- ============================================================

-- transfer_cabang: metadata pengiriman antar cabang
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang' AND COLUMN_NAME = 'DARI_CABANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang` ADD COLUMN `DARI_CABANG` VARCHAR(50) NULL DEFAULT NULL AFTER `LOKASI`",
    "SELECT 'transfer_cabang.DARI_CABANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang' AND COLUMN_NAME = 'KE_CABANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang` ADD COLUMN `KE_CABANG` VARCHAR(50) NULL DEFAULT NULL AFTER `DARI_CABANG`",
    "SELECT 'transfer_cabang.KE_CABANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang' AND COLUMN_NAME = 'MODE_KIRIM');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang` ADD COLUMN `MODE_KIRIM` VARCHAR(20) NOT NULL DEFAULT 'OFFLINE_EXPORT' AFTER `KE_CABANG`",
    "SELECT 'transfer_cabang.MODE_KIRIM sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang' AND COLUMN_NAME = 'STATUS_TRANSFER');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang` ADD COLUMN `STATUS_TRANSFER` VARCHAR(20) NOT NULL DEFAULT 'PENDING' AFTER `MODE_KIRIM`",
    "SELECT 'transfer_cabang.STATUS_TRANSFER sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang' AND COLUMN_NAME = 'ID_CLOUD_TRANSFER');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang` ADD COLUMN `ID_CLOUD_TRANSFER` VARCHAR(50) NULL DEFAULT NULL AFTER `STATUS_TRANSFER`",
    "SELECT 'transfer_cabang.ID_CLOUD_TRANSFER sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang' AND COLUMN_NAME = 'FILE_MANUAL');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang` ADD COLUMN `FILE_MANUAL` VARCHAR(255) NULL DEFAULT NULL AFTER `ID_CLOUD_TRANSFER`",
    "SELECT 'transfer_cabang.FILE_MANUAL sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- transfer_cabang_detail: status terima per item
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang_detail' AND COLUMN_NAME = 'DITERIMA_QTY');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang_detail` ADD COLUMN `DITERIMA_QTY` DECIMAL(10,2) DEFAULT '0.00' AFTER `TOTAL_QTY`",
    "SELECT 'transfer_cabang_detail.DITERIMA_QTY sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_cabang_detail' AND COLUMN_NAME = 'STATUS_ITEM');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_cabang_detail` ADD COLUMN `STATUS_ITEM` VARCHAR(20) NOT NULL DEFAULT 'PENDING' AFTER `DITERIMA_QTY`",
    "SELECT 'transfer_cabang_detail.STATUS_ITEM sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom transfer antar cabang selesai.' AS status;

-- ============================================================
-- Kolom lokal untuk master cabang
-- Aman dijalankan berulang kali.
-- ============================================================

-- tbl_cabang: metadata source + sinkronisasi
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'sumber');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `sumber` VARCHAR(20) NOT NULL DEFAULT 'manual' AFTER `pemilik`",
    "SELECT 'tbl_cabang.sumber sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'updated_at');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP AFTER `sumber`",
    "SELECT 'tbl_cabang.updated_at sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'id_cloud');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `id_cloud` VARCHAR(50) NULL DEFAULT NULL AFTER `updated_at`",
    "SELECT 'tbl_cabang.id_cloud sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'is_dirty');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `is_dirty` TINYINT NOT NULL DEFAULT 0 AFTER `id_cloud`",
    "SELECT 'tbl_cabang.is_dirty sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'version');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `version` INT NOT NULL DEFAULT 1 AFTER `is_dirty`",
    "SELECT 'tbl_cabang.version sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- sync_id: UUID unik global untuk identifikasi di Supabase
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'sync_id');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `sync_id` VARCHAR(36) NULL DEFAULT NULL AFTER `version`",
    "SELECT 'tbl_cabang.sync_id sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Isi sync_id untuk baris yang sudah ada
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'sync_id');
SET @sql = IF(@col > 0,
    "UPDATE `tbl_cabang` SET `sync_id` = UUID() WHERE `sync_id` IS NULL",
    "SELECT 'skip' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Unique index untuk sync_id
SET @hasIdx = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND INDEX_NAME = 'uq_sync_id_tbl_cabang');
SET @sql = IF(@hasIdx = 0,
    "ALTER TABLE `tbl_cabang` ADD UNIQUE INDEX `uq_sync_id_tbl_cabang` (`sync_id`)",
    "SELECT 'uq_sync_id_tbl_cabang sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- updated_by: user terakhir yang mengubah data
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_cabang' AND COLUMN_NAME = 'updated_by');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_cabang` ADD COLUMN `updated_by` VARCHAR(50) NULL DEFAULT NULL AFTER `updated_at`",
    "SELECT 'tbl_cabang.updated_by sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom master cabang selesai.' AS status;

-- ============================================================
-- Kolom lokal penerimaan transfer manual antar cabang
-- Aman dijalankan berulang kali.
-- ============================================================

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'DARI_CABANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `DARI_CABANG` VARCHAR(50) NULL DEFAULT NULL AFTER `id_transfer`",
    "SELECT 'transfer_masuk_manual.DARI_CABANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'KE_CABANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `KE_CABANG` VARCHAR(50) NULL DEFAULT NULL AFTER `DARI_CABANG`",
    "SELECT 'transfer_masuk_manual.KE_CABANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'STATUS_TERIMA');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `STATUS_TERIMA` VARCHAR(20) NOT NULL DEFAULT 'PENDING' AFTER `KE_CABANG`",
    "SELECT 'transfer_masuk_manual.STATUS_TERIMA sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'TGL_TERIMA');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `TGL_TERIMA` DATETIME NULL DEFAULT NULL AFTER `STATUS_TERIMA`",
    "SELECT 'transfer_masuk_manual.TGL_TERIMA sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom transfer masuk manual selesai.' AS status;

-- ============================================================
-- Tambah kolom TRANSFER_CABANG ke tbl_barang
-- Pemisahan mutasi transfer antar cabang dari transfer barang
-- internal (toko<->gudang) agar laporan stok lebih akurat.
-- 4 kolom: MASUK/KELUAR x TOKO/GUDANG
-- Aman dijalankan berulang kali.
-- ============================================================

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'TRANSFER_CABANG_MASUK_TOKO');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `TRANSFER_CABANG_MASUK_TOKO` decimal(10,2) DEFAULT '0.00' AFTER `TRANSFER_BARANG_KELUAR_TOKO`",
    "SELECT 'TRANSFER_CABANG_MASUK_TOKO sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'TRANSFER_CABANG_KELUAR_TOKO');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `TRANSFER_CABANG_KELUAR_TOKO` decimal(10,2) DEFAULT '0.00' AFTER `TRANSFER_CABANG_MASUK_TOKO`",
    "SELECT 'TRANSFER_CABANG_KELUAR_TOKO sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'TRANSFER_CABANG_MASUK_GUDANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `TRANSFER_CABANG_MASUK_GUDANG` decimal(10,2) DEFAULT '0.00' AFTER `TRANSFER_BARANG_KELUAR_GUDANG`",
    "SELECT 'TRANSFER_CABANG_MASUK_GUDANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'TRANSFER_CABANG_KELUAR_GUDANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `TRANSFER_CABANG_KELUAR_GUDANG` decimal(10,2) DEFAULT '0.00' AFTER `TRANSFER_CABANG_MASUK_GUDANG`",
    "SELECT 'TRANSFER_CABANG_KELUAR_GUDANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom TRANSFER_CABANG tbl_barang selesai.' AS status;

-- ============================================================
-- Tabel temporary untuk Buku Besar Pembantu (Piutang & Hutang)
-- Dipakai oleh FormLapBBPembantu.vb
-- Aman dijalankan berulang kali.
-- ============================================================
CREATE TABLE IF NOT EXISTS `temp_bbpembantu` (
  `ID`          INT AUTO_INCREMENT PRIMARY KEY,
  `NOMOR`       INT DEFAULT 0,
  `TANGGAL`     DATETIME DEFAULT NULL,
  `NOTA`        VARCHAR(30) DEFAULT NULL,
  `ENTITAS`     VARCHAR(100) DEFAULT NULL,
  `KETERANGAN`  VARCHAR(50) DEFAULT NULL,
  `DEBET`       DECIMAL(15,0) DEFAULT '0',
  `KREDIT`      DECIMAL(15,0) DEFAULT '0',
  `SALDO`       DECIMAL(15,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SELECT 'Tabel temp_bbpembantu siap.' AS status;

-- ============================================================
-- Kolom STOK_AWAL_TOKO / STOK_AWAL_GUDANG di tbl_barang
-- Dipakai untuk menghitung stok sampai masa lampau:
--   Stok pada tanggal T = STOK_AWAL + mutasi historybarang s/d T
-- Logika:
--   STOK_AWAL_TOKO  = stok toko saat ini  - SUM(TOTAL_QTY historybarang TOKO)
--   STOK_AWAL_GUDANG= stok gudang saat ini - SUM(TOTAL_QTY historybarang GUDANG)
-- Kolom ini diisi sekali saat posting, lalu dipakai query laporan.
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'STOK_AWAL_TOKO');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `STOK_AWAL_TOKO` decimal(10,2) DEFAULT '0.00' AFTER `AWAL_TOKO`",
    "SELECT 'STOK_AWAL_TOKO sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_barang' AND COLUMN_NAME = 'STOK_AWAL_GUDANG');
SET @sql = IF(@col = 0,
    "ALTER TABLE `tbl_barang` ADD COLUMN `STOK_AWAL_GUDANG` decimal(10,2) DEFAULT '0.00' AFTER `AWAL_GUDANG`",
    "SELECT 'STOK_AWAL_GUDANG sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Kolom STOK_AWAL_TOKO dan STOK_AWAL_GUDANG siap.' AS status;

-- ============================================================
-- View: stok barang pada tanggal tertentu (stok masa lampau)
-- Cara pakai dari VB:
--   SET @tgl = '2025-12-31 23:59:59';
--   SELECT * FROM v_stok_per_tanggal;
-- Atau langsung query inline tanpa view (lebih fleksibel).
--
-- Query inline untuk stok per tanggal @tgl:
--   SELECT b.ID_BARANG, b.NAMA_BARANG,
--     b.STOK_AWAL_TOKO  + COALESCE(SUM(CASE WHEN h.LOKASI='TOKO'   THEN h.TOTAL_QTY ELSE 0 END),0) AS STOK_TOKO_PADA_TGL,
--     b.STOK_AWAL_GUDANG+ COALESCE(SUM(CASE WHEN h.LOKASI='GUDANG' THEN h.TOTAL_QTY ELSE 0 END),0) AS STOK_GUDANG_PADA_TGL
--   FROM tbl_barang b
--   LEFT JOIN historybarang h ON h.ID_BARANG = b.ID_BARANG AND h.TANGGAL <= @tgl
--   GROUP BY b.ID_BARANG, b.NAMA_BARANG, b.STOK_AWAL_TOKO, b.STOK_AWAL_GUDANG
-- ============================================================

SELECT 'Migrasi stok masa lampau selesai.' AS status;


-- ============================================================
-- Migrasi transfer_masuk_manual: kolom audit trail penerimaan
-- Ditambahkan untuk mendukung fitur TERIMA di FormTransferCabang
-- Aman dijalankan berulang kali.
-- ============================================================

-- sumber_transfer: CLOUD / CSV / MANUAL
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'sumber_transfer');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `sumber_transfer` VARCHAR(20) NOT NULL DEFAULT 'MANUAL' AFTER `id_transfer`",
    "SELECT 'transfer_masuk_manual.sumber_transfer sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- id_cloud: referensi ke transfer_barang_cloud di Supabase
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'id_cloud');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `id_cloud` VARCHAR(50) NULL DEFAULT NULL AFTER `sumber_transfer`",
    "SELECT 'transfer_masuk_manual.id_cloud sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- tgl_kirim: tanggal pengirim mengirim transfer
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'tgl_kirim');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `tgl_kirim` DATETIME NULL DEFAULT NULL",
    "SELECT 'transfer_masuk_manual.tgl_kirim sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- tgl_terima: tanggal penerima menerima transfer
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'tgl_terima');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `tgl_terima` DATETIME NULL DEFAULT NULL",
    "SELECT 'transfer_masuk_manual.tgl_terima sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- id_user_terima: user yang menerima transfer
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'id_user_terima');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `id_user_terima` VARCHAR(50) NULL DEFAULT NULL",
    "SELECT 'transfer_masuk_manual.id_user_terima sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- catatan_terima: catatan dari penerima saat menerima transfer
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'catatan_terima');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `catatan_terima` VARCHAR(255) NULL DEFAULT NULL",
    "SELECT 'transfer_masuk_manual.catatan_terima sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Unique key untuk id_cloud (mencegah duplikat dari Supabase)
SET @hasUK = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual'
    AND CONSTRAINT_NAME = 'uk_id_cloud' AND CONSTRAINT_TYPE = 'UNIQUE');
SET @sql = IF(@hasUK = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD UNIQUE KEY `uk_id_cloud` (`id_cloud`)",
    "SELECT 'uk_id_cloud sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi audit trail transfer_masuk_manual selesai.' AS status;

-- ============================================================
-- Tambah kolom harga_beli ke transfer_masuk_manual
-- Dipakai untuk menjaga konsistensi jurnal antara cabang
-- pengirim dan penerima. Nilai diambil dari CSV/cloud saat
-- import, fallback ke DB lokal jika 0.
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'transfer_masuk_manual' AND COLUMN_NAME = 'harga_beli');
SET @sql = IF(@col = 0,
    "ALTER TABLE `transfer_masuk_manual` ADD COLUMN `harga_beli` DECIMAL(15,2) NOT NULL DEFAULT '0.00' AFTER `qty_satuan`",
    "SELECT 'transfer_masuk_manual.harga_beli sudah ada' AS info");
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom harga_beli transfer_masuk_manual selesai.' AS status;

-- ============================================================
-- Tabel antrian transfer keluar offline antar cabang
-- Dipakai saat koneksi cloud tidak tersedia; di-upload saat online
-- Aman dijalankan berulang kali.
-- ============================================================
CREATE TABLE IF NOT EXISTS `transfer_keluar_offline` (
  `id`           INT AUTO_INCREMENT PRIMARY KEY,
  `id_transfer`  VARCHAR(30)   NOT NULL,
  `dari_cabang`  VARCHAR(50)   NOT NULL,
  `ke_cabang`    VARCHAR(50)   NOT NULL,
  `kode_barang`  VARCHAR(50)   NOT NULL,
  `nama_barang`  VARCHAR(150)  DEFAULT NULL,
  `qty`          DECIMAL(10,2) DEFAULT '0.00',
  `satuan`       VARCHAR(20)   DEFAULT NULL,
  `isi_satuan`   INT           DEFAULT 1,
  `qty_satuan`   DECIMAL(10,2) DEFAULT '0.00',
  `keterangan`   VARCHAR(255)  DEFAULT NULL,
  `status`       VARCHAR(20)   NOT NULL DEFAULT 'PENDING',
  `created_at`   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_status`      (`status`),
  INDEX `idx_id_transfer` (`id_transfer`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SELECT 'Tabel transfer_keluar_offline siap.' AS status;

-- ============================================================
-- Tabel antrian konfirmasi terima transfer (saat offline)
-- Dipakai untuk menyimpan konfirmasi TERIMA yang belum bisa
-- dikirim ke Supabase karena tidak ada koneksi.
-- Aman dijalankan berulang kali.
-- ============================================================
CREATE TABLE IF NOT EXISTS `transfer_terima_pending` (
  `id`          INT AUTO_INCREMENT PRIMARY KEY,
  `id_cloud`    VARCHAR(50)  NOT NULL,
  `kode_barang` VARCHAR(50)  NOT NULL,
  `id_user`     VARCHAR(50)  DEFAULT NULL,
  `tgl_terima`  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status`      VARCHAR(20)  NOT NULL DEFAULT 'PENDING',
  UNIQUE KEY `uk_cloud_kode` (`id_cloud`, `kode_barang`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SELECT 'Tabel transfer_terima_pending siap.' AS status;

-- ============================================================
-- Tambah kolom KETERANGAN di tbl_datareferensi
-- Berisi deskripsi/penjelasan akun COA
-- Aman dijalankan berulang kali.
-- ============================================================
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_datareferensi'
      AND COLUMN_NAME  = 'KETERANGAN');
SET @sql = IF(@col = 0,
    'ALTER TABLE `tbl_datareferensi` ADD COLUMN `KETERANGAN` TEXT NULL DEFAULT NULL AFTER `AKUN_NRLR`',
    'SELECT ''KETERANGAN sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Migrasi kolom KETERANGAN tbl_datareferensi selesai.' AS status;

-- ============================================================
-- Standarisasi JENIS_TRANSAKSI di jurnalumum ke PascalCase
-- Sebelumnya FormBarang.vb menggunakan uraian.ToLower() sehingga
-- menghasilkan "tambah barang" dan "kurang barang" (lowercase).
-- Distandarisasi ke PascalCase agar konsisten dengan nilai lain
-- seperti "Edit Barang", "Hapus Barang", "Penjualan", dll.
-- Aman dijalankan berulang kali.
-- ============================================================
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'Tambah Barang' WHERE JENIS_TRANSAKSI = 'tambah barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'Kurang Barang' WHERE JENIS_TRANSAKSI = 'kurang barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'Hapus Barang'  WHERE JENIS_TRANSAKSI = 'hapus barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'Edit Barang'   WHERE JENIS_TRANSAKSI = 'edit barang';

SELECT CONCAT(
    'Standarisasi JENIS_TRANSAKSI selesai. ',
    'Tambah Barang: ', (SELECT COUNT(*) FROM jurnalumum WHERE JENIS_TRANSAKSI = 'Tambah Barang'), ' baris, ',
    'Kurang Barang: ', (SELECT COUNT(*) FROM jurnalumum WHERE JENIS_TRANSAKSI = 'Kurang Barang'), ' baris.'
) AS status;

-- ============================================================
-- Task 3.0 — Kolom baru untuk fitur pembelian lengkap
-- Diskon supplier, PPN masukan, biaya kirim, biaya lain,
-- status transaksi beli, harga average & harga sebelumnya
-- Aman dijalankan berulang kali
-- ============================================================

-- DISKON_SUPPLIER (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'DISKON_SUPPLIER');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `DISKON_SUPPLIER` DECIMAL(15,2) NOT NULL DEFAULT 0 AFTER `GRAND_TOTAL_BELI`',
    'SELECT ''DISKON_SUPPLIER sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- PPN_MASUKAN (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'PPN_MASUKAN');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `PPN_MASUKAN` DECIMAL(15,2) NOT NULL DEFAULT 0 AFTER `DISKON_SUPPLIER`',
    'SELECT ''PPN_MASUKAN sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- BIAYA_KIRIM (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'BIAYA_KIRIM');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `BIAYA_KIRIM` DECIMAL(15,2) NOT NULL DEFAULT 0 AFTER `PPN_MASUKAN`',
    'SELECT ''BIAYA_KIRIM sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- BIAYA_LAIN (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'BIAYA_LAIN');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `BIAYA_LAIN` DECIMAL(15,2) NOT NULL DEFAULT 0 AFTER `BIAYA_KIRIM`',
    'SELECT ''BIAYA_LAIN sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- KODE_AKUN_BIAYA_LAIN (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'KODE_AKUN_BIAYA_LAIN');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `KODE_AKUN_BIAYA_LAIN` VARCHAR(20) NOT NULL DEFAULT '''' AFTER `BIAYA_LAIN`',
    'SELECT ''KODE_AKUN_BIAYA_LAIN sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- NAMA_AKUN_BIAYA_LAIN (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'NAMA_AKUN_BIAYA_LAIN');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `NAMA_AKUN_BIAYA_LAIN` VARCHAR(50) NOT NULL DEFAULT '''' AFTER `KODE_AKUN_BIAYA_LAIN`',
    'SELECT ''NAMA_AKUN_BIAYA_LAIN sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- STATUS_TRANSAKSI_BELI (pembelian)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian' AND COLUMN_NAME = 'STATUS_TRANSAKSI_BELI');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian` ADD COLUMN `STATUS_TRANSAKSI_BELI` VARCHAR(20) NOT NULL DEFAULT ''LUNAS'' AFTER `NAMA_AKUN_BIAYA_LAIN`',
    'SELECT ''STATUS_TRANSAKSI_BELI sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- HARGA_AVERAGE (pembelian_detail)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian_detail' AND COLUMN_NAME = 'HARGA_AVERAGE');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian_detail` ADD COLUMN `HARGA_AVERAGE` DECIMAL(15,2) NOT NULL DEFAULT 0 AFTER `HARGA_BELI`',
    'SELECT ''HARGA_AVERAGE sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- HARGA_BELI_SEBELUMNYA (pembelian_detail)
SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'pembelian_detail' AND COLUMN_NAME = 'HARGA_BELI_SEBELUMNYA');
SET @sql = IF(@col = 0,
    'ALTER TABLE `pembelian_detail` ADD COLUMN `HARGA_BELI_SEBELUMNYA` DECIMAL(15,2) NOT NULL DEFAULT 0 AFTER `HARGA_AVERAGE`',
    'SELECT ''HARGA_BELI_SEBELUMNYA sudah ada, dilewati'' AS info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SELECT 'Task 3.0 — Migrasi kolom pembelian selesai.' AS status;
