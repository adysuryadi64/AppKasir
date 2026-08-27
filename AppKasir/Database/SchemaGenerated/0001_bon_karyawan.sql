-- ============================================================
-- Table: bon_karyawan
-- Alter table bon_karyawan: 10 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- bon_karyawan: 10 kolom berubah
ALTER TABLE `bon_karyawan`    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_REK` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_REK` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KETERANGAN` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

