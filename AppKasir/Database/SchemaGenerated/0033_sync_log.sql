-- ============================================================
-- Table: sync_log
-- Alter table sync_log: 3 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- sync_log: 3 kolom berubah
ALTER TABLE `sync_log`    MODIFY COLUMN `tabel` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `id_lokal` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL';

