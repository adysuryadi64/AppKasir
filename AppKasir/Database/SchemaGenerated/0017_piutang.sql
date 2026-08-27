-- ============================================================
-- Table: piutang
-- Alter table piutang: 8 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- piutang: 8 kolom berubah
ALTER TABLE `piutang`    MODIFY COLUMN `ID_BAYAR_PIUTANG` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_PELANGGAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_BAYAR` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER_BAYAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER_BAYAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

