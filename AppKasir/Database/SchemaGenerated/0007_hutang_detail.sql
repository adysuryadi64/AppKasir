-- ============================================================
-- Table: hutang_detail
-- Alter table hutang_detail: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- hutang_detail: 12 kolom berubah
ALTER TABLE `hutang_detail`    MODIFY COLUMN `ID_BAYAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_BAYAR` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BELI` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODE` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_BELI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `JATUH_TEMPO` datetime DEFAULT 'NULL',
    MODIFY COLUMN `STATUS` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

