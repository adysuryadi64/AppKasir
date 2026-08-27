-- ============================================================
-- Table: tbl_user
-- Alter table tbl_user: 7 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_user: 7 kolom berubah
ALTER TABLE `tbl_user`    MODIFY COLUMN `PWD` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `LVL` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `login_session_key` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `email_status` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `password_expire_date` datetime DEFAULT 'NULL',
    MODIFY COLUMN `password_reset_key` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

