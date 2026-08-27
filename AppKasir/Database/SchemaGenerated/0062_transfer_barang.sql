-- ============================================================
-- Table: transfer_barang
-- Alter table transfer_barang: 7 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_barang: 7 kolom berubah
ALTER TABLE `transfer_barang`    MODIFY COLUMN `TGL_TRANSFER` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `TOTAL_RUPIAH` decimal(15,0) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

