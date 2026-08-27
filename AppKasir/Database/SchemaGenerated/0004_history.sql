-- ============================================================
-- Table: history
-- Alter table history: 2 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- history: 2 kolom berubah
ALTER TABLE `history`    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

