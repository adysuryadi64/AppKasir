-- ============================================================
-- Table: tukarbarang
-- Alter table tukarbarang: 18 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tukarbarang: 18 kolom berubah
ALTER TABLE `tukarbarang`    MODIFY COLUMN `ID_TUKAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_PENJUALAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `DESKRIPSI` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `KODEPEL` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMAPEL` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `JENISPEL` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ISISATUAN` int(11) DEFAULT 'NULL',
    MODIFY COLUMN `QTYSATUAN` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `HARGASATUAN` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `DISKON` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOTALHARGA` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SELISIH` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL';

