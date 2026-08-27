-- ============================================================
-- Table: poin_barang
-- Alter table poin_barang: 3 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- poin_barang: 3 kolom berubah
ALTER TABLE `poin_barang`    MODIFY COLUMN `updated_by` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

