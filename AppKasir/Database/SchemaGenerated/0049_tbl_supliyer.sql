-- ============================================================
-- Table: tbl_supliyer
-- Alter table tbl_supliyer: 6 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_supliyer: 6 kolom berubah
ALTER TABLE `tbl_supliyer`    MODIFY COLUMN `NAMA` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `ALAMAT` varchar(200) DEFAULT 'NULL',
    MODIFY COLUMN `HP` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `updated_by` varchar(50) DEFAULT 'NULL';

