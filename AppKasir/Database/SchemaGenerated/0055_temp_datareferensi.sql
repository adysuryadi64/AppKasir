-- ============================================================
-- Table: temp_datareferensi
-- Alter table temp_datareferensi: 5 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- temp_datareferensi: 5 kolom berubah
ALTER TABLE `temp_datareferensi`    MODIFY COLUMN `STATUS` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `SUB_AKUN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `AKUN_DK` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `AKUN_NRLR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

