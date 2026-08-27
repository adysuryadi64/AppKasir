-- ============================================================
-- Table: poin_ledger
-- Alter table poin_ledger: 5 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- poin_ledger: 5 kolom berubah
ALTER TABLE `poin_ledger`    MODIFY COLUMN `NO_REFERENSI` varchar(30) DEFAULT 'NULL' COMMENT 'Nomor faktur penjualan atau nomor penukaran TP-xxx',
    MODIFY COLUMN `KETERANGAN` varchar(200) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

