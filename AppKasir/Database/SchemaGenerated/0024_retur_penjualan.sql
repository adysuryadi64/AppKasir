-- ============================================================
-- Table: retur_penjualan
-- Alter table retur_penjualan: 19 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- retur_penjualan: 19 kolom berubah
ALTER TABLE `retur_penjualan`    MODIFY COLUMN `ID_RETUR_PENJUALAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_RETUR_JUAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `ID_PELANGGAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_PELANGGAN` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `JENIS_PELANGGAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ALAMAT_PELANGGAN` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `KONTAK_PELANGGAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_PENJUALAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_PENJUALAN` datetime DEFAULT 'NULL',
    MODIFY COLUMN `STATUS_PENJUALAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `PENYIMPANAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `HUTANG_PENJUALAN` decimal(15,0) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `NAMA_REKENING` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_REKENING` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ALASAN_RETUR` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

