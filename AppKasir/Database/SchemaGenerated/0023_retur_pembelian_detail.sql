-- ============================================================
-- Table: retur_pembelian_detail
-- Alter table retur_pembelian_detail: 13 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- retur_pembelian_detail: 13 kolom berubah
ALTER TABLE `retur_pembelian_detail`    MODIFY COLUMN `ID_RETUR_PEMBELIAN` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_RETUR_BELI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `ID_SUPLIYER` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_SUPLIYER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `QTY_SAT` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `PENYIMPANAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

