-- ============================================================
-- Table: tbl_satuan
-- Alter table tbl_satuan: 4 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_satuan: 4 kolom berubah
ALTER TABLE `tbl_satuan`    MODIFY COLUMN `NAMA` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `updated_by` varchar(50) DEFAULT 'NULL';

