-- ============================================================
-- Table: pembelian_ditahan
-- Alter table pembelian_ditahan: 13 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- pembelian_ditahan: 13 kolom berubah
ALTER TABLE `pembelian_ditahan`    MODIFY COLUMN `ID_SUPPLIER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_SUPLIYER` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `NOTA_PEMBELIAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_BELI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_BAYAR` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `GRAND_TOTAL_BELI` decimal(15,0) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOTAL_BARANG` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TGL_RETUR` datetime DEFAULT 'NULL',
    MODIFY COLUMN `RETUR` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(50) DEFAULT 'NULL';

