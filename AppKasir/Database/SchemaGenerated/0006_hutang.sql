-- ============================================================
-- Table: hutang
-- Alter table hutang: 8 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- hutang: 8 kolom berubah
ALTER TABLE `hutang`    MODIFY COLUMN `NOBAYARHUTANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODESUPLIYER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMASUPLIYER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `TGLPEMBAYARAN` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER_BAYAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER_BAYAR` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

