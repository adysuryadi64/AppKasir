-- ============================================================
-- Table: transfer_masuk_manual
-- Alter table transfer_masuk_manual: 12 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_masuk_manual: 12 kolom berubah
ALTER TABLE `transfer_masuk_manual`    MODIFY COLUMN `id_cloud` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `dari_cabang` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `ke_cabang` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `nama_barang` varchar(150) DEFAULT 'NULL',
    MODIFY COLUMN `qty` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `satuan` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `qty_satuan` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `keterangan` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `tgl_kirim` datetime DEFAULT 'NULL',
    MODIFY COLUMN `tgl_terima` datetime DEFAULT 'NULL',
    MODIFY COLUMN `id_user_terima` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `catatan_terima` varchar(255) DEFAULT 'NULL';

