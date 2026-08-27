-- ============================================================
-- Table: temp_mutasi_barang
-- Alter table temp_mutasi_barang: 8 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- temp_mutasi_barang: 8 kolom berubah
ALTER TABLE `temp_mutasi_barang`    MODIFY COLUMN `FAKTUR` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `JENIS` varchar(150) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(255) DEFAULT 'NULL',
    MODIFY COLUMN `QTY_MASUK` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `QTY_KELUAR` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `SALDO` decimal(10,2) DEFAULT '0.00',
    MODIFY COLUMN `ID_USER` varchar(255) DEFAULT 'NULL';

