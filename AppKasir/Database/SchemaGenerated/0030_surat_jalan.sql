-- ============================================================
-- Table: surat_jalan
-- Alter table surat_jalan: 14 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- surat_jalan: 14 kolom berubah
ALTER TABLE `surat_jalan`    MODIFY COLUMN `TGL_PENGIRIMAN` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_ARMADA` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ARMADA` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_ARMADA` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_SUPIR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `SUPIR` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_HELPER1` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `HELPER1` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_HELPER2` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `HELPER2` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

