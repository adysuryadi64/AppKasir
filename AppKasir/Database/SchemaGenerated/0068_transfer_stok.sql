-- ============================================================
-- Table: transfer_stok
-- Alter table transfer_stok: 22 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- transfer_stok: 22 kolom berubah
ALTER TABLE `transfer_stok`    MODIFY COLUMN `ID_TRANSFER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_TRANSFER` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `URAIAN` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG_M` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG_M` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `QTY_M` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN_M` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ISI_M` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `QTY_SAT_M` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `HARGA_SAT_M` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_BARANG_K` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG_K` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `QTY_K` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN_K` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ISI_K` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `QTY_SAT_K` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `HARGA_SAT_K` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `Selisih` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

