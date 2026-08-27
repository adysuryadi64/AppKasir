-- ============================================================
-- Table: transfer_keluar_offline
-- Alter table transfer_keluar_offline: 10 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_keluar_offline: 10 kolom berubah
ALTER TABLE `transfer_keluar_offline`    MODIFY COLUMN `id_transfer` varchar(30) NOT NULL,
    MODIFY COLUMN `dari_cabang` varchar(50) NOT NULL,
    MODIFY COLUMN `ke_cabang` varchar(50) NOT NULL,
    MODIFY COLUMN `kode_barang` varchar(50) NOT NULL,
    MODIFY COLUMN `nama_barang` varchar(150) DEFAULT 'NULL',
    MODIFY COLUMN `qty` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `satuan` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `qty_satuan` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `keterangan` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `status` varchar(20) NOT NULL DEFAULT 'PENDING';

