-- ============================================================
-- Table: hakaksesuser
-- Alter table hakaksesuser: 3 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- hakaksesuser: 3 kolom berubah
ALTER TABLE `hakaksesuser`    MODIFY COLUMN `Role` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `ModuleName` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

