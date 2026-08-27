-- ============================================================
-- Table: jurnalumum
-- Alter table jurnalumum: 18 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- jurnalumum: 18 kolom berubah
ALTER TABLE `jurnalumum`    MODIFY COLUMN `TGL_TRANSAKSI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `NO_NOTA` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `URAIAN` varchar(200) DEFAULT 'NULL',
    MODIFY COLUMN `AKUN_D` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_AKUN_D` varchar(40) DEFAULT 'NULL',
    MODIFY COLUMN `NOMOR_AKUN_D` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `AKUN_K` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_AKUN_K` varchar(40) DEFAULT 'NULL',
    MODIFY COLUMN `NOMOR_AKUN_K` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BANTU_D` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_BANTU_D` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BANTU_K` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_BANTU_K` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_TRANSAKSI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

