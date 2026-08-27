-- ============================================================
-- Table: temp_bbpembantu
-- Alter table temp_bbpembantu: 4 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- temp_bbpembantu: 4 kolom berubah
ALTER TABLE `temp_bbpembantu`    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `NOTA` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `ENTITAS` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `KETERANGAN` varchar(50) DEFAULT 'NULL';

