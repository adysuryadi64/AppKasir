-- ============================================================
-- Table: tbl_kategori
-- Alter table tbl_kategori: 4 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_kategori: 4 kolom berubah
ALTER TABLE `tbl_kategori`    MODIFY COLUMN `JENIS` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `updated_by` varchar(50) DEFAULT 'NULL';

