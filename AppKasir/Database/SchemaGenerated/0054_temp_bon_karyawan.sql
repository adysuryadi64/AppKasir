-- ============================================================
-- Table: temp_bon_karyawan
-- Alter table temp_bon_karyawan: 4 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- temp_bon_karyawan: 4 kolom berubah
ALTER TABLE `temp_bon_karyawan`    MODIFY COLUMN `NOMOR` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `JENIS` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `KETERANGAN` varchar(100) DEFAULT 'NULL';

