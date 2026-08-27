-- ============================================================
-- Table: retur_penjualan_detail
-- Alter table retur_penjualan_detail: 17 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- retur_penjualan_detail: 17 kolom berubah
ALTER TABLE `retur_penjualan_detail`    MODIFY COLUMN `ID_RETUR_PENJUALAN` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_RETUR_JUAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_PELANGGAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_PELANGGAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `QTY_SATUAN` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `HARGA_JUAL` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOTAL_DISKON` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `LABA` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

