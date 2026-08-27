-- ============================================================
-- Table: penjualan_ditahan
-- Alter table penjualan_ditahan: 10 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- penjualan_ditahan: 10 kolom berubah
ALTER TABLE `penjualan_ditahan`    MODIFY COLUMN `FAKTUR_JUAL` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `ID_PELANGGAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_PELANGGAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_JUAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOTAL_ITEM` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL';

