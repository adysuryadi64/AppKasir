-- ============================================================
-- Table: tbl_cabang
-- Alter table tbl_cabang: 8 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_cabang: 8 kolom berubah
ALTER TABLE `tbl_cabang`    MODIFY COLUMN `nama_cabang` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `alamat` varchar(200) DEFAULT 'NULL',
    MODIFY COLUMN `kota` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `hp` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `pemilik` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `updated_by` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

