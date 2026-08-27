-- ============================================================
-- Table: transfer_cabang_detail
-- Alter table transfer_cabang_detail: 11 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_cabang_detail: 11 kolom berubah
ALTER TABLE `transfer_cabang_detail`    MODIFY COLUMN `TGL_TRANSFER` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(120) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI_ASAL` varchar(10) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(150) DEFAULT 'NULL',
    MODIFY COLUMN `QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `DITERIMA_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

