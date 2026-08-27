-- ============================================================
-- Table: tempbukubesarpembantu
-- Alter table tempbukubesarpembantu: 5 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tempbukubesarpembantu: 5 kolom berubah
ALTER TABLE `tempbukubesarpembantu`    MODIFY COLUMN `JENISTRANSAKSI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NOTRANSAKSI` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `TGLTRANSAKSI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `NONOTA` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `URAIAN` varchar(100) DEFAULT 'NULL';

