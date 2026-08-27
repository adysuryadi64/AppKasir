-- ============================================================
-- Table: transfer_terima_pending
-- Alter table transfer_terima_pending: 4 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_terima_pending: 4 kolom berubah
ALTER TABLE `transfer_terima_pending`    MODIFY COLUMN `id_cloud` varchar(50) NOT NULL,
    MODIFY COLUMN `kode_barang` varchar(50) NOT NULL,
    MODIFY COLUMN `id_user` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `status` varchar(20) NOT NULL DEFAULT 'PENDING';

