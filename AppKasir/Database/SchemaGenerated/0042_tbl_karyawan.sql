-- ============================================================
-- Table: tbl_karyawan
-- Alter table tbl_karyawan: 5 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_karyawan: 5 kolom berubah
ALTER TABLE `tbl_karyawan`    MODIFY COLUMN `NAMA` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `JABATAN` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `TGLMASUK` datetime DEFAULT 'NULL',
    MODIFY COLUMN `GAJI` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

