-- ============================================================
-- Table: temp_supliyerbayar
-- Alter table temp_supliyerbayar: 3 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- temp_supliyerbayar: 3 kolom berubah
ALTER TABLE `temp_supliyerbayar`    MODIFY COLUMN `NO` smallint(6) DEFAULT 'NULL',
    MODIFY COLUMN `KODE` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA` varchar(50) DEFAULT 'NULL';

