-- ============================================================
-- Table: tbl_audit_trail_arsip
-- Alter table tbl_audit_trail_arsip: 2 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- tbl_audit_trail_arsip: 2 kolom berubah
ALTER TABLE `tbl_audit_trail_arsip`    MODIFY COLUMN `lokasi` char(6) DEFAULT 'NULL' COMMENT 'TOKO atau GUDANG',
    MODIFY COLUMN `komputer` varchar(30) DEFAULT 'NULL';

