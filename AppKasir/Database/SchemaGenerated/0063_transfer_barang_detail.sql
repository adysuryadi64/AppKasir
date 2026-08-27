-- ============================================================
-- Table: transfer_barang_detail
-- Alter table transfer_barang_detail: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_barang_detail: 12 kolom berubah
ALTER TABLE `transfer_barang_detail`    MODIFY COLUMN `ID_TRANSFER` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_TRANSFER` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `HARGA` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

