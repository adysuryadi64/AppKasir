-- ============================================================
-- Table: stok_opname
-- Alter table stok_opname: 16 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- stok_opname: 16 kolom berubah
ALTER TABLE `stok_opname`    MODIFY COLUMN `ID_STOK_OPNAME` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_BARANG` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_BARANG` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `KATEGORI` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `HARGA` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `STOK_SYSTEM` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `STOK_NYATA` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `STOK_SELISIH` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SATUAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `KETERANGAN` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

