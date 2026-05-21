-- -------------------------------------------------------------
-- MIGRATION: 28_sales_order_migration.sql
-- AMAN TANPA DROP TABLE (SAFE COLUMN-BY-COLUMN MIGRATION)
-- -------------------------------------------------------------

-- 1. Buat tabel sales_order jika belum ada sama sekali dengan kolom minimal
CREATE TABLE IF NOT EXISTS `sales_order` (
  `ID_PENJUALAN` varchar(30) NOT NULL,
  PRIMARY KEY (`ID_PENJUALAN`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 2. Buat tabel sales_order_detail jika belum ada sama sekali dengan kolom minimal
CREATE TABLE IF NOT EXISTS `sales_order_detail` (
  `FAKTUR_JUAL` varchar(15) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 3. Definisikan Stored Procedure pembantu untuk migrasi kolom dan index secara aman
DELIMITER $$

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
      AND table_name = p_table_name
      AND column_name = p_column_name;
      
    IF col_exists = 0 THEN
        SET @sql_stmt = CONCAT('ALTER TABLE `', p_table_name, '` ADD COLUMN `', p_column_name, '` ', p_column_definition);
        PREPARE stmt FROM @sql_stmt;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DROP PROCEDURE IF EXISTS AddUniqueKeySafely$$
CREATE PROCEDURE AddUniqueKeySafely(
    IN p_table_name VARCHAR(64),
    IN p_index_name VARCHAR(64),
    IN p_column_name VARCHAR(64)
)
BEGIN
    DECLARE index_exists INT;
    
    SELECT COUNT(*) INTO index_exists
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = p_table_name
      AND index_name = p_index_name;
      
    IF index_exists = 0 THEN
        SET @sql_stmt = CONCAT('ALTER TABLE `', p_table_name, '` ADD UNIQUE KEY `', p_index_name, '` (`', p_column_name, '`)');
        PREPARE stmt FROM @sql_stmt;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DROP PROCEDURE IF EXISTS AddIndexSafely$$
CREATE PROCEDURE AddIndexSafely(
    IN p_table_name VARCHAR(64),
    IN p_index_name VARCHAR(64),
    IN p_column_name VARCHAR(64)
)
BEGIN
    DECLARE index_exists INT;
    
    SELECT COUNT(*) INTO index_exists
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = p_table_name
      AND index_name = p_index_name;
      
    IF index_exists = 0 THEN
        SET @sql_stmt = CONCAT('ALTER TABLE `', p_table_name, '` ADD KEY `', p_index_name, '` (`', p_column_name, '`)');
        PREPARE stmt FROM @sql_stmt;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DELIMITER ;

-- 4. Jalankan migrasi kolom untuk tabel `sales_order`
CALL AddColumnSafely('sales_order', 'ID_PELANGGAN', "varchar(10) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'NAMA_PELANGGAN', "varchar(100) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'JENIS_PELANGGAN', "varchar(30) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'ALAMAT_PELANGGAN', "varchar(200) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'LOKASIBARANG', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'ID_SALES', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'NAMA_SALES', "varchar(100) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'TGL_TRANSAKSI', "datetime DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'TOTAL_HPP', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'GRAND_TOTAL_SBL_PAJAK', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'DISKON_TOTAL_PERSEN', "decimal(10,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'DISKON_TOTAL_RP', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'PAJAK_PERSEN', "decimal(10,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'PAJAK_RP', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'GRAND_TOTAL_STL_PAJAK', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'LABA', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order', 'STATUS_TRANSAKSI', "varchar(20) DEFAULT 'Aktif'");
CALL AddColumnSafely('sales_order', 'CATATAN', "text");
CALL AddColumnSafely('sales_order', 'ID_USER', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'ID_KOMPUTER', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order', 'created_at', "datetime DEFAULT CURRENT_TIMESTAMP");
CALL AddColumnSafely('sales_order', 'updated_at', "datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
CALL AddColumnSafely('sales_order', 'sync_id', "varchar(36) DEFAULT NULL");

-- 5. Jalankan migrasi kolom untuk tabel `sales_order_detail`
CALL AddColumnSafely('sales_order_detail', 'ID_PELANGGAN', "varchar(30) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'NAMA_PELANGGAN', "varchar(100) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'JENIS_PELANGGAN', "varchar(10) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'LOKASIBARANG', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'TANGGAL_JUAL', "datetime DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'ID_BARANG', "varchar(15) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'NAMA_BARANG', "varchar(100) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'SERIAL_NUMBER', "varchar(50) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'HARGA_BELI', "decimal(15,4) DEFAULT '0.0000'");
CALL AddColumnSafely('sales_order_detail', 'QTY', "decimal(15,4) DEFAULT '0.0000'");
CALL AddColumnSafely('sales_order_detail', 'SATUAN', "varchar(10) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'ISI_SATUAN', "int(11) DEFAULT '1'");
CALL AddColumnSafely('sales_order_detail', 'HARGA_BELI_SATUAN', "decimal(15,4) DEFAULT '0.0000'");
CALL AddColumnSafely('sales_order_detail', 'HARGA_JUAL', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order_detail', 'QTY_SATUAN', "decimal(15,4) DEFAULT '0.0000'");
CALL AddColumnSafely('sales_order_detail', 'DISKON_PERSEN', "decimal(10,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order_detail', 'DISKON_RP', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order_detail', 'TOTAL_DISKON', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order_detail', 'TOTAL_Harga', "decimal(15,2) DEFAULT '0.00'");
CALL AddColumnSafely('sales_order_detail', 'ID_USER', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'ID_KOMPUTER', "varchar(20) DEFAULT NULL");
CALL AddColumnSafely('sales_order_detail', 'created_at', "datetime DEFAULT CURRENT_TIMESTAMP");
CALL AddColumnSafely('sales_order_detail', 'updated_at', "datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
CALL AddColumnSafely('sales_order_detail', 'sync_id', "varchar(36) DEFAULT NULL");

-- 6. Jalankan migrasi Unique Key & Key secara aman
CALL AddUniqueKeySafely('sales_order', 'sync_id_unique', 'sync_id');
CALL AddUniqueKeySafely('sales_order_detail', 'sync_id_unique', 'sync_id');
CALL AddIndexSafely('sales_order_detail', 'FK_SALES_ORDER', 'FAKTUR_JUAL');

-- 7. Bersihkan Stored Procedure setelah selesai migrasi
DROP PROCEDURE IF EXISTS AddColumnSafely;
DROP PROCEDURE IF EXISTS AddUniqueKeySafely;
DROP PROCEDURE IF EXISTS AddIndexSafely;
