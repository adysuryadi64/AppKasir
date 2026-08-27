-- ============================================================
-- Table: tempjurnalumum
-- Alter table tempjurnalumum: 5 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tempjurnalumum: 5 kolom berubah
ALTER TABLE `tempjurnalumum`    MODIFY COLUMN `JENISTRANSAKSI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NOTRANSAKSI` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `TGLTRANSAKSI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `NONOTA` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `URAIAN` varchar(200) DEFAULT 'NULL';

