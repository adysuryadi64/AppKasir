-- ============================================================
-- Table: retur_pembelian
-- Alter table retur_pembelian: 21 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- retur_pembelian: 21 kolom berubah
ALTER TABLE `retur_pembelian`    MODIFY COLUMN `ID_RETUR_PEMBELIAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_RETUR_BELI` datetime DEFAULT 'NULL',
    MODIFY COLUMN `ID_SUPPLIER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_SUPPLIER` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `ALAMAT_SUPPLIER` varchar(100) DEFAULT 'NULL',
    MODIFY COLUMN `KONTAK_SUPPLIER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_PEMBELIAN` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `TGL_PEMBELIAN` datetime DEFAULT 'NULL',
    MODIFY COLUMN `STATUS_PEMBELIAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `PENYIMPANAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `SISA_PEMBELIAN` decimal(15,0) DEFAULT 'NULL',
    MODIFY COLUMN `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `JENIS_PENGEMBALIAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_REKENING` varchar(60) DEFAULT 'NULL',
    MODIFY COLUMN `KODE_REKENING` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `NAMA_REKENING_TRANSFER` varchar(100) DEFAULT 'NULL' COMMENT 'Nama akun untuk pembayaran transfer',
    MODIFY COLUMN `KODE_REKENING_TRANSFER` varchar(50) DEFAULT 'NULL' COMMENT 'Kode akun untuk pembayaran transfer',
    MODIFY COLUMN `ALASAN_RETUR` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

