-- ============================================================
-- Table: sales_order
-- Alter table sales_order: 11 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- sales_order: 11 kolom berubah
ALTER TABLE `sales_order`    MODIFY COLUMN `ID_PELANGGAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_PELANGGAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `ALAMAT_PELANGGAN` varchar(200) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASIBARANG` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_SALES` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_SALES` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_TRANSAKSI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

