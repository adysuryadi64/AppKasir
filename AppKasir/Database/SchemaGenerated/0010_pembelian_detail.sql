-- ============================================================
-- Table: pembelian_detail
-- Alter table pembelian_detail: 14 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- pembelian_detail: 14 kolom berubah
ALTER TABLE `pembelian_detail`    MODIFY COLUMN `FAKTUR_BELI` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `NOTA_BELI` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_MASUK` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_SUPLIYER` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_SUPLIYER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `QTY_SAT` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

