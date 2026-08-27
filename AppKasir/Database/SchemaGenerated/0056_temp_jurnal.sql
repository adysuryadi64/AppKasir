-- ============================================================
-- Table: temp_jurnal
-- Alter table temp_jurnal: 6 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- temp_jurnal: 6 kolom berubah
ALTER TABLE `temp_jurnal`    MODIFY COLUMN `TYPE_AKUN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_AKUN` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_AKUN` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `SUB_AKUN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `AKUN_DK` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `AKUN_NLRL` varchar(20) DEFAULT 'NULL';

