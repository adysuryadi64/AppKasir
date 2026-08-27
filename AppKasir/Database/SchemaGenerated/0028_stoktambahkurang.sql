-- ============================================================
-- Table: stoktambahkurang
-- Alter table stoktambahkurang: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- stoktambahkurang: 12 kolom berubah
ALTER TABLE `stoktambahkurang`    MODIFY COLUMN `FAKTUR` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `JENIS` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

