-- ============================================================
-- Table: transfer_cabang
-- Alter table transfer_cabang: 11 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_cabang: 11 kolom berubah
ALTER TABLE `transfer_cabang`    MODIFY COLUMN `TGL_TRANSFER` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI_ASAL` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `DARI_CABANG` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KE_CABANG` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_CLOUD_TRANSFER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `FILE_MANUAL` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

