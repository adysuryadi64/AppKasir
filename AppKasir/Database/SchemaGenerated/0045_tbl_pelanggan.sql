-- ============================================================
-- Table: tbl_pelanggan
-- Alter table tbl_pelanggan: 7 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_pelanggan: 7 kolom berubah
ALTER TABLE `tbl_pelanggan`    MODIFY COLUMN `NAMA` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ALAMAT` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `NO_TELP` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL',
    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `updated_by` varchar(50) DEFAULT 'NULL';

