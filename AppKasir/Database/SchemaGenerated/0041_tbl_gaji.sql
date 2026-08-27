-- ============================================================
-- Table: tbl_gaji
-- Alter table tbl_gaji: 9 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_gaji: 9 kolom berubah
ALTER TABLE `tbl_gaji`    MODIFY COLUMN `BONUS_SUPIR` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `BONUS_HELPER` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `BONUS_TRANSPORT` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `BONUS_MAKAN` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `BONUS_LEMBUR` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `JENIS_POTONGAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `POTONGAN_ABSEN` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `POTONGAN_ABSEN_KHUSUS` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `POTONGAN_TERLAMBAT` decimal(10,2) DEFAULT '0.00';

