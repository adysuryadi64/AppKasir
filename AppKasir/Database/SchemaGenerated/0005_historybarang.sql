-- ============================================================
-- Table: historybarang
-- Alter table historybarang: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- historybarang: 12 kolom berubah
ALTER TABLE `historybarang`    MODIFY COLUMN `FAKTUR` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `JENIS` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

