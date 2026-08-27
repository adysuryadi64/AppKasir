-- ============================================================
-- Table: sales_order_detail
-- Alter table sales_order_detail: 13 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- sales_order_detail: 13 kolom berubah
ALTER TABLE `sales_order_detail`    MODIFY COLUMN `FAKTUR_JUAL` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `ID_PELANGGAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_PELANGGAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASIBARANG` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_JUAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `SERIAL_NUMBER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `SATUAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

