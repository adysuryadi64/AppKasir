-- ============================================================
-- Table: tbl_datareferensi
-- Alter table tbl_datareferensi: 5 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_datareferensi: 5 kolom berubah
ALTER TABLE `tbl_datareferensi`    MODIFY COLUMN `STATUS` varchar(10) DEFAULT 'NULL' COMMENT 'Terkunci = tidak bisa dihapus user | NULL = bebas dihapus',
    MODIFY COLUMN `SUB_AKUN` varchar(20) DEFAULT 'NULL' COMMENT 'AKTIVA | PASIVA | LABA RUGI | LABA (pendapatan+kontra-beban) | RUGI (beban+kontra-pendapatan)',
    MODIFY COLUMN `AKUN_DK` varchar(20) DEFAULT 'NULL' COMMENT 'Saldo normal: DEBET | KREDIT',
    MODIFY COLUMN `AKUN_NRLR` varchar(20) DEFAULT 'NULL' COMMENT 'Posisi di laporan: NERACA | LABA RUGI',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

