-- ============================================================
-- Table: piutang_detail
-- Alter table piutang_detail: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- piutang_detail: 12 kolom berubah
ALTER TABLE `piutang_detail`    MODIFY COLUMN `ID_BAYAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_BAYAR` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_JUAL` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODE` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_JUAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `JATUH_TEMPO` datetime DEFAULT 'NULL',
    MODIFY COLUMN `STATUS` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

