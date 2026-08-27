-- ============================================================
-- Table: penjualan_ditahan_detail
-- Alter table penjualan_ditahan_detail: 13 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- penjualan_ditahan_detail: 13 kolom berubah
ALTER TABLE `penjualan_ditahan_detail`    MODIFY COLUMN `FAKTUR_JUAL` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `HARGA_JUAL` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `QTY_SATUAN` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `DISKON_RP` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOTAL_DISKON` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOKO` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `GUDANG` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `STOK` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SISA` decimal(10,2) DEFAULT '0.00';

