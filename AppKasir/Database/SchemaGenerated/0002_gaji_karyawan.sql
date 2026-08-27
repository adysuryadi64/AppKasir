-- ============================================================
-- Table: gaji_karyawan
-- Alter table gaji_karyawan: 31 kolom berubah
-- Generated: 2026-08-07 17:33:20
-- ============================================================

-- gaji_karyawan: 31 kolom berubah
ALTER TABLE `gaji_karyawan`    MODIFY COLUMN `BULAN` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `TANGGAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `TANGGALAWAL` datetime DEFAULT 'NULL',
    MODIFY COLUMN `TANGGALAKHIR` datetime DEFAULT 'NULL',
    MODIFY COLUMN `NAMA` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `KOMISI_JUAL` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `SUPIR_RP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `HELPER_RP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `LEMBUR` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `LEMBUR_RP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `TUNJANGAN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `TRANSP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `TRANSPORT` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `UANG_MKN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `UANG_MAKAN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `SALDO_BON` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `POT_BON` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `ANGSURAN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `NILAI_POTONGAN_ABSEN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `ABSEN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `ABSEN_RP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `ABSEN_KHUSUS` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `ABSEN_KHUSUS_RP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `TERLAMBAT` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `TERLAMBAT_RP` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `POT_LAIN` decimal(10,0) DEFAULT '0',
    MODIFY COLUMN `REKENING` varchar(50) DEFAULT 'NULL',
    MODIFY COLUMN `LOKASI` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_USER` varchar(20) DEFAULT 'NULL',
    MODIFY COLUMN `ID_KOMPUTER` varchar(30) DEFAULT 'NULL',
    MODIFY COLUMN `sync_id` varchar(36) DEFAULT 'NULL';

