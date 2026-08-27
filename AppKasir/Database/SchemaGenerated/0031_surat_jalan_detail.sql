-- ============================================================
-- Table: surat_jalan_detail
-- Alter table surat_jalan_detail: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- surat_jalan_detail: 12 kolom berubah
ALTER TABLE `surat_jalan_detail`    MODIFY COLUMN `NOTA` varchar(15) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_KIRIM` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASISIMPAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NOTA_BELANJA` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_PELANGGAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ALAMAT_PELANGGAN` varchar(200) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL_BELANJA` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

