-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Apr 17, 2026 at 09:35 PM
-- Server version: 8.0.17
-- PHP Version: 8.3.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_kasirlancar`
--

-- --------------------------------------------------------

--
-- Table structure for table `bon_karyawan`
--

CREATE TABLE `bon_karyawan` (
  `FAKTUR` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AWAL_BON` decimal(15,0) DEFAULT '0',
  `NOMINAL` decimal(15,0) DEFAULT '0',
  `AKHIR_BON` decimal(15,0) DEFAULT '0',
  `KETERANGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `gaji_karyawan`
--

CREATE TABLE `gaji_karyawan` (
  `NOMOR` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `BULAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `TANGGALAWAL` datetime DEFAULT NULL,
  `TANGGALAKHIR` datetime DEFAULT NULL,
  `KODE` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POKOK` decimal(15,0) DEFAULT '0',
  `OMSET_JUAL` decimal(20,0) DEFAULT '0',
  `KOMISI_JUAL` decimal(10,0) DEFAULT '0',
  `SUPIR` int(11) DEFAULT '0',
  `SUPIR_RP` decimal(10,0) DEFAULT '0',
  `HELPER` int(11) DEFAULT '0',
  `HELPER_RP` decimal(10,0) DEFAULT '0',
  `LEMBUR` decimal(10,0) DEFAULT '0',
  `LEMBUR_RP` decimal(10,0) DEFAULT '0',
  `TUNJANGAN` decimal(10,0) DEFAULT '0',
  `TRANSP` decimal(10,0) DEFAULT '0',
  `TRANSPORT` decimal(10,0) DEFAULT '0',
  `UANG_MKN` decimal(10,0) DEFAULT '0',
  `UANG_MAKAN` decimal(10,0) DEFAULT '0',
  `SALDO_BON` decimal(10,0) DEFAULT '0',
  `POT_BON` decimal(10,0) DEFAULT '0',
  `ANGSURAN` decimal(10,0) DEFAULT '0',
  `NILAI_POTONGAN_ABSEN` decimal(10,0) DEFAULT '0',
  `ABSEN` decimal(10,0) DEFAULT '0',
  `ABSEN_RP` decimal(10,0) DEFAULT '0',
  `ABSEN_KHUSUS` decimal(10,0) DEFAULT '0',
  `ABSEN_KHUSUS_RP` decimal(10,0) DEFAULT '0',
  `TERLAMBAT` decimal(10,0) DEFAULT '0',
  `TERLAMBAT_RP` decimal(10,0) DEFAULT '0',
  `POT_LAIN` decimal(10,0) DEFAULT '0',
  `PENDAPATAN` decimal(15,0) DEFAULT '0',
  `POTONGAN` decimal(15,0) DEFAULT '0',
  `TERIMA` decimal(15,0) DEFAULT '0',
  `REKENING` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `hakaksesuser`
--

CREATE TABLE `hakaksesuser` (
  `NO` int(11) NOT NULL,
  `UserName` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ModuleName` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CanRead` tinyint(1) DEFAULT '0',
  `CanAdd` tinyint(1) DEFAULT '0',
  `CanEdit` tinyint(1) DEFAULT '0',
  `CanDelete` tinyint(1) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `history`
--

CREATE TABLE `history` (
  `NO` int(11) NOT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `Aksi` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `historybarang`
--

CREATE TABLE `historybarang` (
  `NO` int(11) NOT NULL,
  `FAKTUR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT '0',
  `ID_USER` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `hutang`
--

CREATE TABLE `hutang` (
  `NOBAYARHUTANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODESUPLIYER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMASUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLPEMBAYARAN` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTALHUTANG` decimal(15,0) DEFAULT '0',
  `NOMINALBAYAR` decimal(15,0) DEFAULT '0',
  `SISAHUTANG` decimal(15,0) DEFAULT '0',
  `ID_USER_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `hutang_detail`
--

CREATE TABLE `hutang_detail` (
  `ID_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BAYAR` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BELI` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BELI` datetime DEFAULT NULL,
  `TOTAL_HUTANG` decimal(15,0) DEFAULT '0',
  `DIBAYAR` decimal(15,0) DEFAULT '0',
  `RETUR` decimal(15,0) DEFAULT '0',
  `HUTANG` decimal(15,0) DEFAULT '0',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `PEMBAYARAN` decimal(15,0) DEFAULT '0',
  `STATUS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jurnalumum`
--

CREATE TABLE `jurnalumum` (
  `NO` int(11) NOT NULL,
  `NO_TRANSAKSI` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSAKSI` datetime DEFAULT NULL,
  `NO_NOTA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_D` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN_D` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMOR_AKUN_D` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_K` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN_K` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMOR_AKUN_K` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BANTU_D` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_BANTU_D` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BANTU_K` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_BANTU_K` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMINAL` decimal(20,0) DEFAULT '0',
  `JENIS_TRANSAKSI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jurnalumum_backup_coa`
--

CREATE TABLE `jurnalumum_backup_coa` (
  `NO` int(11) NOT NULL DEFAULT '0',
  `NO_TRANSAKSI` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSAKSI` datetime DEFAULT NULL,
  `NO_NOTA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_D` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN_D` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMOR_AKUN_D` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_K` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN_K` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMOR_AKUN_K` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BANTU_D` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_BANTU_D` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BANTU_K` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_BANTU_K` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMINAL` decimal(20,0) DEFAULT '0',
  `JENIS_TRANSAKSI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `pembelian`
--

CREATE TABLE `pembelian` (
  `ID_PEMBELIAN` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_SUPPLIER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_PEMBELIAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BELI` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_BAYAR` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN_TF` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `NAMA_AKUN_TF` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `GRAND_TOTAL_BELI` decimal(15,0) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` decimal(10,2) DEFAULT '0.00',
  `PEMBAYARAN` decimal(15,0) DEFAULT '0',
  `NOMINAL_TRANSFER` decimal(15,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `RETUR` decimal(15,0) DEFAULT '0',
  `TAGIHAN` decimal(15,0) DEFAULT '0',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `TGL_BAYAR` datetime DEFAULT NULL,
  `NOMINALBAYAR` decimal(15,0) DEFAULT '0',
  `STATUS_JUAL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `STATUS_TRANSAKSI_BELI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `pembelian_detail`
--

CREATE TABLE `pembelian_detail` (
  `NO` int(11) NOT NULL,
  `FAKTUR_BELI` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_BELI` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_MASUK` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_SUPLIYER` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(10,2) DEFAULT '0.00',
  `HARGA_AVERAGE` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_SEBELUMNYA` decimal(10,2) DEFAULT '0.00',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_SATUAN` decimal(10,2) DEFAULT '0.00',
  `QTY_SAT` decimal(10,2) DEFAULT '0.00',
  `TOTAL` decimal(15,0) DEFAULT '0',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `pembelian_ditahan`
--

CREATE TABLE `pembelian_ditahan` (
  `ID_PEMBELIAN` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_SUPPLIER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_PEMBELIAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BELI` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_BAYAR` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GRAND_TOTAL_BELI` decimal(15,0) DEFAULT NULL,
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` decimal(10,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `RETUR` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `pembelian_ditahan_detail`
--

CREATE TABLE `pembelian_ditahan_detail` (
  `FAKTUR_BELI` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_BELI` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_MASUK` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_SUPLIYER` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(10,2) DEFAULT '0.00',
  `HARGA_AVERAGE` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_SEBELUMNYA` decimal(10,2) DEFAULT '0.00',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_SATUAN` decimal(10,2) DEFAULT '0.00',
  `QTY_SAT` decimal(10,2) DEFAULT '0.00',
  `TOTAL` decimal(10,2) DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `penjualan`
--

CREATE TABLE `penjualan` (
  `ID_PENJUALAN` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_PELANGGAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASIBARANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_TRANSAKSI` datetime DEFAULT NULL,
  `TOTAL_HPP` decimal(15,2) DEFAULT '0.00',
  `GRAND_TOTAL_SBL_PAJAK` decimal(15,2) DEFAULT '0.00',
  `DISKON_TOTAL_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_TOTAL_RP` decimal(10,2) DEFAULT '0.00',
  `PAJAK_PERSEN` decimal(10,2) DEFAULT '0.00',
  `PAJAK_RP` decimal(10,2) DEFAULT '0.00',
  `GRAND_TOTAL_STL_PAJAK` decimal(15,2) DEFAULT '0.00',
  `LABA` decimal(10,2) DEFAULT '0.00',
  `BAYAR` decimal(15,2) DEFAULT '0.00',
  `NOMINAL_TRANSFER` decimal(15,2) DEFAULT '0.00',
  `BIAYA_KIRIM` decimal(10,2) DEFAULT '0.00',
  `KEMBALI` decimal(15,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `NILAI_RETUR` decimal(10,2) DEFAULT '0.00',
  `TGL_PEMBAYARAN` datetime DEFAULT NULL,
  `NOMINALBAYARPIUTANG` decimal(15,2) DEFAULT '0.00',
  `SISA_TAGIHAN` decimal(15,2) DEFAULT '0.00',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `STATUS_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `STATUS_TRANSAKSI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TYPE_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PEMBAYARAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN_TF` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `NAMA_AKUN_TF` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `TYPE_AKUNBANK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUNBANK` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PEMBAYARANBANK` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `METODE` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BANK` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NO_REKENING` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REKENING` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NO_REFFERENSI` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_SALES` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SALES` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `penjualan_detail`
--

CREATE TABLE `penjualan_detail` (
  `FAKTUR_JUAL` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASIBARANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `ID_BARANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SERIAL_NUMBER` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HARGA_BELI` decimal(15,0) DEFAULT '0',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `HARGA_BELI_SATUAN` decimal(15,0) DEFAULT '0',
  `HARGA_JUAL` decimal(15,0) DEFAULT '0',
  `QTY_SATUAN` decimal(10,2) DEFAULT '0.00',
  `DISKON_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_RP` decimal(10,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(10,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `LABA` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `penjualan_ditahan`
--

CREATE TABLE `penjualan_ditahan` (
  `FAKTUR_JUAL` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `GRAN_TOTAL` decimal(15,0) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_ITEM` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `penjualan_ditahan_detail`
--

CREATE TABLE `penjualan_ditahan_detail` (
  `FAKTUR_JUAL` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SERIAL_NUMBER` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HARGA_BELI` decimal(10,2) DEFAULT '0.00',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` smallint(6) DEFAULT '0',
  `HARGA_BELI_SATUAN` decimal(15,0) DEFAULT '0',
  `HARGA_JUAL` decimal(10,2) DEFAULT '0.00',
  `QTY_SATUAN` decimal(10,2) DEFAULT '0.00',
  `DISKON_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_RP` decimal(10,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(10,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `TOKO` decimal(10,2) DEFAULT '0.00',
  `GUDANG` decimal(10,2) DEFAULT '0.00',
  `STOK` decimal(10,2) DEFAULT '0.00',
  `SISA` decimal(10,2) DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `piutang`
--

CREATE TABLE `piutang` (
  `ID_BAYAR_PIUTANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_PELANGGAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BAYAR` datetime DEFAULT NULL,
  `TOTAL_PIUTANG` decimal(15,0) DEFAULT '0',
  `NOMINAL_BAYAR` decimal(15,0) DEFAULT '0',
  `SISA_PIUTANG` decimal(15,0) DEFAULT '0',
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `piutang_detail`
--

CREATE TABLE `piutang_detail` (
  `ID_BAYAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BAYAR` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_JUAL` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `PIUTANG` decimal(15,0) DEFAULT '0',
  `DIBAYAR` decimal(15,0) DEFAULT '0',
  `RETUR` decimal(15,0) DEFAULT '0',
  `HUTANG` decimal(15,0) DEFAULT '0',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `PEMBAYARAN` decimal(15,0) DEFAULT '0',
  `STATUS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `retur_pembelian`
--

CREATE TABLE `retur_pembelian` (
  `ID_RETUR_PEMBELIAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_BELI` datetime DEFAULT NULL,
  `ID_SUPPLIER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPPLIER` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_SUPPLIER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KONTAK_SUPPLIER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PEMBELIAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_PEMBELIAN` datetime DEFAULT NULL,
  `STATUS_PEMBELIAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PENYIMPANAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BAYAR_PEMBELIAN` decimal(15,0) DEFAULT '0',
  `SISA_PEMBELIAN` decimal(15,0) DEFAULT NULL,
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT '0',
  `JENIS_PENGEMBALIAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REKENING` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REKENING` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALASAN_RETUR` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `retur_pembelian_detail`
--

CREATE TABLE `retur_pembelian_detail` (
  `ID_RETUR_PEMBELIAN` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_BELI` datetime DEFAULT NULL,
  `ID_SUPLIYER` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(15,0) DEFAULT '0',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `HARGA_BELI_SATUAN` decimal(15,0) DEFAULT '0',
  `QTY_SAT` decimal(10,2) DEFAULT '0.00',
  `TOTAL` decimal(15,0) DEFAULT '0',
  `PENYIMPANAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `retur_penjualan`
--

CREATE TABLE `retur_penjualan` (
  `ID_RETUR_PENJUALAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_JUAL` datetime DEFAULT NULL,
  `ID_PELANGGAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KONTAK_PELANGGAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PENJUALAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_PENJUALAN` datetime DEFAULT NULL,
  `STATUS_PENJUALAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PENYIMPANAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BAYAR_PENJUALAN` decimal(15,0) DEFAULT '0',
  `HUTANG_PENJUALAN` decimal(15,0) DEFAULT NULL,
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT '0',
  `NAMA_REKENING` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REKENING` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALASAN_RETUR` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `retur_penjualan_detail`
--

CREATE TABLE `retur_penjualan_detail` (
  `ID_RETUR_PENJUALAN` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_JUAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(10,2) DEFAULT '0.00',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` decimal(10,2) DEFAULT '0.00',
  `QTY_SATUAN` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_SATUAN` decimal(15,0) DEFAULT '0',
  `HARGA_JUAL` decimal(10,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(10,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `LABA` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `saldo_tahunan`
--

CREATE TABLE `saldo_tahunan` (
  `TAHUN` int(11) NOT NULL,
  `KODE_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `TOTAL_DEBET` decimal(20,0) DEFAULT '0',
  `TOTAL_KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `stoktambahkurang`
--

CREATE TABLE `stoktambahkurang` (
  `FAKTUR` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `stok_opname`
--

CREATE TABLE `stok_opname` (
  `ID_STOK_OPNAME` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KATEGORI` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA` decimal(10,2) DEFAULT '0.00',
  `STOK_SYSTEM` decimal(10,2) DEFAULT '0.00',
  `STOK_NYATA` decimal(10,2) DEFAULT '0.00',
  `STOK_SELISIH` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` smallint(6) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `KETERANGAN` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `surat_jalan`
--

CREATE TABLE `surat_jalan` (
  `NOTA` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_PENGIRIMAN` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_PELANGGAN` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(20,0) DEFAULT '0',
  `KODE_ARMADA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ARMADA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_ARMADA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_SUPIR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SUPIR` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_HELPER1` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HELPER1` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_HELPER2` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HELPER2` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `surat_jalan_detail`
--

CREATE TABLE `surat_jalan_detail` (
  `NOTA` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_KIRIM` datetime DEFAULT NULL,
  `LOKASISIMPAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_BELANJA` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_PELANGGAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BELANJA` datetime DEFAULT NULL,
  `NILAI_BELANJA` decimal(15,0) DEFAULT '0',
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `sync_config`
--

CREATE TABLE `sync_config` (
  `kunci` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nilai` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `sync_log`
--

CREATE TABLE `sync_log` (
  `id` int(11) NOT NULL,
  `waktu` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `jenis` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'UPLOAD/DOWNLOAD/CONFLICT/ERROR',
  `tabel` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_lokal` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `pesan` text COLLATE utf8mb4_unicode_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `sync_queue`
--

CREATE TABLE `sync_queue` (
  `id` int(11) NOT NULL,
  `aksi` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'INSERT / UPDATE',
  `tabel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_lokal` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'PK lokal (ID_BARANG dll)',
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `payload` longtext COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'JSON data',
  `status` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending' COMMENT 'pending/done/failed',
  `retry_count` tinyint(4) NOT NULL DEFAULT '0',
  `last_error` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_armada`
--

CREATE TABLE `tbl_armada` (
  `KODE` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NOPOL` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `JENIS` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_barang`
--

CREATE TABLE `tbl_barang` (
  `ID_BARANG` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_BARANG_BANTU` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_BARANG_BANTU` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_KATEGORI` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_KATEGORI` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_SUPLIYER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_MERK` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_MERK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SERIAL_NUMBER` tinyint(1) DEFAULT NULL,
  `JENIS_SATUAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_TERAKHIR` decimal(10,2) DEFAULT '0.00',
  `HPP_UMUM_KECIL` decimal(10,2) DEFAULT '0.00',
  `HPP_UMUM_SEDANG` decimal(10,2) DEFAULT '0.00',
  `HPP_UMUM_BESAR` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_UMUM_KECIL` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_UMUM_SEDANG` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_UMUM_BESAR` decimal(10,2) DEFAULT '0.00',
  `HPP_PARTAI_KECIL` decimal(10,2) DEFAULT '0.00',
  `HPP_PARTAI_SEDANG` decimal(10,2) DEFAULT '0.00',
  `HPP_PARTAI_BESAR` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_PARTAI_KECIL` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_UPARTAI_SEDANG` decimal(10,2) DEFAULT '0.00',
  `HARGA_BELI_PARTAI_BESAR` decimal(10,2) DEFAULT '0.00',
  `BARCODE_KECIL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BARCODE_SEDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BARCODE_BESAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_UMUM_KECIL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_UMUM_SEDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_UMUM_BESAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_UMUM_KECIL` int(11) DEFAULT '0',
  `ISI_UMUM_SEDANG` int(11) DEFAULT '0',
  `ISI_UMUM_BESAR` int(11) DEFAULT '0',
  `HARGA_JUAL_UMUM_KECIL` decimal(10,2) DEFAULT '0.00',
  `HARGA_JUAL_UMUM_SEDANG` decimal(10,2) DEFAULT '0.00',
  `HARGA_JUAL_UMUM_BESAR` decimal(10,2) DEFAULT '0.00',
  `SATUAN_PARTAI_KECIL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_PARTAI_SEDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_PARTAI_BESAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_PARTAI_KECIL` int(11) DEFAULT '0',
  `ISI_PARTAI_SEDANG` int(11) DEFAULT '0',
  `ISI_PARTAI_BESAR` int(11) DEFAULT '0',
  `HARGA_JUAL_PARTAI_KECIL` decimal(10,2) DEFAULT '0.00',
  `HARGA_JUAL_PARTAI_SEDANG` decimal(10,2) DEFAULT '0.00',
  `HARGA_JUAL_PARTAI_BESAR` decimal(10,2) DEFAULT '0.00',
  `AWAL_TOKO` decimal(10,2) DEFAULT '0.00',
  `STOK_AWAL_TOKO` decimal(10,2) DEFAULT '0.00',
  `TAMBAH_TOKO` decimal(10,2) DEFAULT '0.00',
  `KURANG_TOKO` decimal(10,2) DEFAULT '0.00',
  `PEMBELIAN_TOKO` decimal(10,2) DEFAULT '0.00',
  `PENJUALAN_TOKO` decimal(10,2) DEFAULT '0.00',
  `RETUR_BELI_TOKO` decimal(10,2) DEFAULT '0.00',
  `RETUR_JUAL_TOKO` decimal(10,2) DEFAULT '0.00',
  `OPNAME_TOKO` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_STOK_MASUK_TOKO` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_STOK_KELUAR_TOKO` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_BARANG_MASUK_TOKO` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_BARANG_KELUAR_TOKO` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_CABANG_MASUK_TOKO` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_CABANG_KELUAR_TOKO` decimal(10,2) DEFAULT '0.00',
  `STOK_TOKO` decimal(10,2) DEFAULT '0.00',
  `AWAL_GUDANG` decimal(10,2) DEFAULT '0.00',
  `STOK_AWAL_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TAMBAH_GUDANG` decimal(10,2) DEFAULT '0.00',
  `KURANG_GUDANG` decimal(10,2) DEFAULT '0.00',
  `PEMBELIAN_GUDANG` decimal(10,2) DEFAULT '0.00',
  `PENJUALAN_GUDANG` decimal(10,2) DEFAULT '0.00',
  `RETUR_BELI_GUDANG` decimal(10,2) DEFAULT '0.00',
  `RETUR_JUAL_GUDANG` decimal(10,2) DEFAULT '0.00',
  `OPNAME_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_STOK_MASUK_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_STOK_KELUAR_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_BARANG_MASUK_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_BARANG_KELUAR_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_CABANG_MASUK_GUDANG` decimal(10,2) DEFAULT '0.00',
  `TRANSFER_CABANG_KELUAR_GUDANG` decimal(10,2) DEFAULT '0.00',
  `STOK_GUDANG` decimal(10,2) DEFAULT '0.00',
  `SATUAN_STOK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_ISI_STOK` int(11) DEFAULT '0',
  `STOK_MIN` decimal(10,2) DEFAULT '0.00',
  `STOK_MAX` decimal(10,2) DEFAULT '0.00',
  `LOKASI_RAK_TOKO` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI_RAK_GUDANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POINT_MEMBER` decimal(10,2) DEFAULT '0.00',
  `POINT_KARYAWAN` decimal(10,2) DEFAULT '0.00',
  `KOMISI_SALES_RP` decimal(10,2) DEFAULT '0.00',
  `KOMISI_SALES_PERSEN` decimal(10,2) DEFAULT '0.00',
  `STATUS` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_cabang`
--

CREATE TABLE `tbl_cabang` (
  `kode_cabang` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_cabang` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `alamat` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `kota` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `hp` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `pemilik` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `sumber` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'manual',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '0',
  `version` int(11) NOT NULL DEFAULT '1',
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_datareferensi`
--

CREATE TABLE `tbl_datareferensi` (
  `STATUS` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_AKUN` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TYPE_AKUN` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `KODE_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_AKUN` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SUB_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_DK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_NRLR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `SALDO_SEBELUMNYA` decimal(20,0) DEFAULT '0',
  `S_DEBET` decimal(20,0) DEFAULT '0',
  `S_KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_datareferensi_backup_coa`
--

CREATE TABLE `tbl_datareferensi_backup_coa` (
  `STATUS` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_AKUN` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TYPE_AKUN` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `KODE_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_AKUN` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SUB_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_DK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_NRLR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `SALDO_SEBELUMNYA` decimal(20,0) DEFAULT '0',
  `S_DEBET` decimal(20,0) DEFAULT '0',
  `S_KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_gaji`
--

CREATE TABLE `tbl_gaji` (
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HARI_KERJA` smallint(6) DEFAULT '0',
  `PROSENTASE_KOMISI` decimal(10,2) DEFAULT '0.00',
  `BONUS_SUPIR` decimal(10,2) DEFAULT '0.00',
  `BONUS_HELPER` decimal(10,2) DEFAULT '0.00',
  `BONUS_TRANSPORT` decimal(10,2) DEFAULT '0.00',
  `BONUS_MAKAN` decimal(10,2) DEFAULT '0.00',
  `BONUS_LEMBUR` decimal(10,2) DEFAULT '0.00',
  `JENIS_POTONGAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POTONGAN_ABSEN` decimal(10,2) DEFAULT '0.00',
  `POTONGAN_ABSEN_KHUSUS` decimal(10,2) DEFAULT '0.00',
  `POTONGAN_TERLAMBAT` decimal(10,2) DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_karyawan`
--

CREATE TABLE `tbl_karyawan` (
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JABATAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLMASUK` datetime DEFAULT NULL,
  `GAJI` decimal(10,0) DEFAULT '0',
  `SALDOAWAL` decimal(15,0) DEFAULT '0',
  `TOTALBON` decimal(15,0) DEFAULT '0',
  `TOTALBAYAR` decimal(15,0) DEFAULT '0',
  `SALDOAKHIR` decimal(15,0) DEFAULT '0',
  `Status` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_kategori`
--

CREATE TABLE `tbl_kategori` (
  `KODE` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `JENIS` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_merk`
--

CREATE TABLE `tbl_merk` (
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_pelanggan`
--

CREATE TABLE `tbl_pelanggan` (
  `KODE` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NO_TELP` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JANGKAPIUTANG` smallint(6) DEFAULT '0',
  `HUTANGAWAL` decimal(15,0) DEFAULT '0',
  `TOTALHUTANG` decimal(15,0) DEFAULT '0',
  `TOTALBAYAR` decimal(15,0) DEFAULT '0',
  `HUTANGAKHIR` decimal(15,0) DEFAULT '0',
  `Status` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_perusahaan`
--

CREATE TABLE `tbl_perusahaan` (
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `KODE_CLOUD` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_CLOUD` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_CLOUD` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KOTA` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HP` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PEMILIK` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FOOTER1` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FOOTER2` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FOOTER3` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SYSTEM_TUTUP_BULAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_TUTUP_BULAN` smallint(6) DEFAULT '0',
  `NAMA_REK_BARANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BARANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LAWAN_NAMA_REK_BARANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LAWAN_KODE_REK_BARANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BELI_TOKO` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BELI_TOKO` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BELI_GUDANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BELI_GUDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_JUAL_TOKO` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_JUAL_TOKO` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_JUAL_GUDANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_JUAL_GUDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_HUTANG_BELI` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_HUTANG_BELI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_PIUTANG_JUAL` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_PIUTANG_JUAL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PEMBELIAN_TOKO` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PEMBELIAN_TOKO` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PENJUALAN_TOKO` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PENJUALAN_TOKO` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PEMBELIAN_GUDANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PEMBELIAN_GUDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PENJUALAN_GUDANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PENJUALAN_GUDANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BON_KARYAWAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BON_KARYAWAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_GAJI_KARYAWAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_GAJI_KARYAWAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BAYAR_HUTANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BAYAR_HUTANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BAYAR_PIUTANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BAYAR_PIUTANG` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_TRANSFER_JUAL` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_TRANSFER_JUAL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_satuan`
--

CREATE TABLE `tbl_satuan` (
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI` int(11) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_supliyer`
--

CREATE TABLE `tbl_supliyer` (
  `KODE` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HP` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JANGKAHUTANG` int(11) DEFAULT '0',
  `HUTANGAWAL` decimal(15,0) DEFAULT '0',
  `TOTALHUTANG` decimal(15,0) DEFAULT '0',
  `TOTALBAYAR` decimal(15,0) DEFAULT '0',
  `HUTANGAKHIR` decimal(15,0) DEFAULT '0',
  `Status` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_user`
--

CREATE TABLE `tbl_user` (
  `KODE_USER` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_USER` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `USER_NAME` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PWD` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LVL` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `login_session_key` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email_status` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `password_expire_date` datetime DEFAULT NULL,
  `password_reset_key` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tempbukubesarpembantu`
--

CREATE TABLE `tempbukubesarpembantu` (
  `NOMOR` int(11) DEFAULT '0',
  `JENISTRANSAKSI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTRANSAKSI` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLTRANSAKSI` datetime DEFAULT NULL,
  `NONOTA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tempjurnalumum`
--

CREATE TABLE `tempjurnalumum` (
  `NOMOR` int(11) DEFAULT '0',
  `JENISTRANSAKSI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTRANSAKSI` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLTRANSAKSI` datetime DEFAULT NULL,
  `NONOTA` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_bbpembantu`
--

CREATE TABLE `temp_bbpembantu` (
  `ID` int(11) NOT NULL,
  `NOMOR` int(11) DEFAULT '0',
  `TANGGAL` datetime DEFAULT NULL,
  `NOTA` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ENTITAS` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(15,0) DEFAULT '0',
  `KREDIT` decimal(15,0) DEFAULT '0',
  `SALDO` decimal(15,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_bon_karyawan`
--

CREATE TABLE `temp_bon_karyawan` (
  `NO` int(11) NOT NULL DEFAULT '0',
  `NOMOR` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(15,0) DEFAULT '0',
  `KREDIT` decimal(15,0) DEFAULT '0',
  `SALDO` decimal(15,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_datareferensi`
--

CREATE TABLE `temp_datareferensi` (
  `STATUS` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_AKUN` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TYPE_AKUN` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `KODE_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_AKUN` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SUB_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_DK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_NRLR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `SALDO_SEBELUMNYA` decimal(20,0) DEFAULT '0',
  `S_DEBET` decimal(20,0) DEFAULT '0',
  `S_KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_jurnal`
--

CREATE TABLE `temp_jurnal` (
  `TYPE_AKUN` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SUB_AKUN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_DK` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_NLRL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_labarugi`
--

CREATE TABLE `temp_labarugi` (
  `TANGGAL` datetime DEFAULT NULL,
  `BULAN` int(11) DEFAULT '0',
  `TOTAL` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_loading`
--

CREATE TABLE `temp_loading` (
  `TANGGAL` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_mutasi_barang`
--

CREATE TABLE `temp_mutasi_barang` (
  `FAKTUR` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_MASUK` decimal(10,2) DEFAULT '0.00',
  `QTY_KELUAR` decimal(10,2) DEFAULT '0.00',
  `SALDO` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_supliyerbayar`
--

CREATE TABLE `temp_supliyerbayar` (
  `NO` smallint(6) DEFAULT NULL,
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BAYAR` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `temp_supliyerhutang`
--

CREATE TABLE `temp_supliyerhutang` (
  `NO` smallint(6) DEFAULT NULL,
  `KODE` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HUTANG` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_barang`
--

CREATE TABLE `transfer_barang` (
  `ID_TRANSFER` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT NULL,
  `ID_USER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_barang_detail`
--

CREATE TABLE `transfer_barang_detail` (
  `ID_TRANSFER` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA` decimal(10,2) DEFAULT '0.00',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` decimal(10,2) DEFAULT '0.00',
  `HARGA_QTY` decimal(15,0) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL` decimal(15,0) DEFAULT '0',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_cabang`
--

CREATE TABLE `transfer_cabang` (
  `ID_TRANSFER` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DARI_CABANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KE_CABANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MODE_KIRIM` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'OFFLINE_EXPORT',
  `STATUS_TRANSFER` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `ID_CLOUD_TRANSFER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FILE_MANUAL` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_cabang_detail`
--

CREATE TABLE `transfer_cabang_detail` (
  `ID_TRANSFER` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(120) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_BARANG` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA` decimal(15,2) DEFAULT '0.00',
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` decimal(10,2) DEFAULT '1.00',
  `HARGA_QTY` decimal(15,2) DEFAULT '0.00',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `DITERIMA_QTY` decimal(10,2) DEFAULT '0.00',
  `STATUS_ITEM` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `TOTAL` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_keluar_offline`
--

CREATE TABLE `transfer_keluar_offline` (
  `id` int(11) NOT NULL,
  `id_transfer` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `dari_cabang` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ke_cabang` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `kode_barang` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_barang` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `qty` decimal(10,2) DEFAULT '0.00',
  `satuan` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `isi_satuan` int(11) DEFAULT '1',
  `qty_satuan` decimal(10,2) DEFAULT '0.00',
  `keterangan` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_masuk_manual`
--

CREATE TABLE `transfer_masuk_manual` (
  `id` int(11) NOT NULL,
  `id_transfer` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sumber_transfer` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'MANUAL',
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dari_cabang` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ke_cabang` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `STATUS_TERIMA` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `kode_barang` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_barang` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `qty` decimal(10,2) DEFAULT '0.00',
  `satuan` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `isi_satuan` int(11) DEFAULT '1',
  `qty_satuan` decimal(10,2) DEFAULT '0.00',
  `harga_beli` decimal(15,2) NOT NULL DEFAULT '0.00',
  `keterangan` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tgl_kirim` datetime DEFAULT NULL,
  `tgl_terima` datetime DEFAULT NULL,
  `id_user_terima` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `catatan_terima` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status_transfer` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_stok`
--

CREATE TABLE `transfer_stok` (
  `ID_TRANSFER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_TRANSFER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `ID_BARANG_M` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG_M` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_M` decimal(10,2) DEFAULT '0.00',
  `SATUAN_M` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_M` decimal(10,2) DEFAULT '0.00',
  `QTY_SAT_M` decimal(10,2) DEFAULT '0.00',
  `HARGA_SAT_M` decimal(10,2) DEFAULT '0.00',
  `TOTAL_HARGA_M` decimal(15,0) DEFAULT '0',
  `ID_BARANG_K` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG_K` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_K` decimal(10,2) DEFAULT '0.00',
  `SATUAN_K` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_K` decimal(10,2) DEFAULT '0.00',
  `QTY_SAT_K` decimal(10,2) DEFAULT '0.00',
  `HARGA_SAT_K` decimal(10,2) DEFAULT '0.00',
  `TOTAL_HARGA_K` decimal(15,0) DEFAULT '0',
  `Selisih` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transfer_terima_pending`
--

CREATE TABLE `transfer_terima_pending` (
  `id` int(11) NOT NULL,
  `id_cloud` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `kode_barang` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_user` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tgl_terima` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tukarbarang`
--

CREATE TABLE `tukarbarang` (
  `ID_TUKAR` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PENJUALAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `DESKRIPSI` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODEPEL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMAPEL` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENISPEL` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY` decimal(10,2) DEFAULT '0.00',
  `SATUAN` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISISATUAN` int(11) DEFAULT NULL,
  `QTYSATUAN` decimal(10,2) DEFAULT '0.00',
  `HARGASATUAN` decimal(10,2) DEFAULT '0.00',
  `DISKON` decimal(10,2) DEFAULT '0.00',
  `TOTALHARGA` decimal(10,2) DEFAULT '0.00',
  `SELISIH` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `bon_karyawan`
--
ALTER TABLE `bon_karyawan`
  ADD PRIMARY KEY (`FAKTUR`),
  ADD UNIQUE KEY `uq_sync_id_bon_karyawan` (`sync_id`),
  ADD KEY `Bon_karyawan_ID_USER` (`ID_USER`),
  ADD KEY `Bon_karyawan_Nama` (`NAMA`),
  ADD KEY `idx_tanggal_bon` (`TANGGAL`),
  ADD KEY `idx_tanggal_jenis_bon` (`TANGGAL`,`JENIS`),
  ADD KEY `idx_kode_tanggal_bon` (`KODE`,`TANGGAL`),
  ADD KEY `idx_kode_jenis_tanggal_bon` (`KODE`,`JENIS`,`TANGGAL`),
  ADD KEY `idx_faktur_bon` (`FAKTUR`),
  ADD KEY `idx_jenis_bon` (`JENIS`),
  ADD KEY `idx_kode_jenis_bon` (`KODE`,`JENIS`);

--
-- Indexes for table `gaji_karyawan`
--
ALTER TABLE `gaji_karyawan`
  ADD PRIMARY KEY (`NOMOR`),
  ADD UNIQUE KEY `uq_sync_id_gaji_karyawan` (`sync_id`),
  ADD KEY `idx_nomor_gaji` (`NOMOR`),
  ADD KEY `idx_bulan_gaji` (`BULAN`),
  ADD KEY `idx_kode_gaji` (`KODE`);

--
-- Indexes for table `hakaksesuser`
--
ALTER TABLE `hakaksesuser`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_hakaksesuser` (`sync_id`),
  ADD KEY `idx_username_hakakses` (`UserName`),
  ADD KEY `idx_username_role_hakakses` (`UserName`,`Role`),
  ADD KEY `idx_username_module_hakakses` (`UserName`,`ModuleName`),
  ADD KEY `idx_updated_at_hakakses` (`updated_at`),
  ADD KEY `idx_role_hakakses` (`Role`);

--
-- Indexes for table `history`
--
ALTER TABLE `history`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_history` (`sync_id`),
  ADD KEY `idx_tanggal_history` (`TANGGAL`);

--
-- Indexes for table `historybarang`
--
ALTER TABLE `historybarang`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_historybarang` (`sync_id`),
  ADD KEY `HistoryBarang_ID_BARANG` (`ID_BARANG`),
  ADD KEY `HistoryBarang_ID_USER` (`ID_USER`),
  ADD KEY `HistoryBarang_JENIS` (`JENIS`),
  ADD KEY `HistoryBarang_LOKASI` (`LOKASI`),
  ADD KEY `HistoryBarang_TANGGAL` (`TANGGAL`),
  ADD KEY `idx_faktur_history` (`FAKTUR`),
  ADD KEY `idx_lokasi_jenis_barang_qty` (`LOKASI`,`JENIS`,`ID_BARANG`,`TOTAL_QTY`),
  ADD KEY `idx_barang_jenis_tgl` (`ID_BARANG`,`JENIS`,`TANGGAL`),
  ADD KEY `idx_barang_jenis_tgl_lokasi` (`ID_BARANG`,`JENIS`,`TANGGAL`,`LOKASI`),
  ADD KEY `idx_barang_lokasi_tgl` (`ID_BARANG`,`LOKASI`,`TANGGAL`);

--
-- Indexes for table `hutang`
--
ALTER TABLE `hutang`
  ADD UNIQUE KEY `uq_sync_id_hutang` (`sync_id`),
  ADD KEY `idx_tgl_pembayaran_hutang` (`TGLPEMBAYARAN`),
  ADD KEY `idx_nobayarhutang` (`NOBAYARHUTANG`),
  ADD KEY `idx_tgl_supplier_hutang` (`TGLPEMBAYARAN`,`NAMASUPLIYER`),
  ADD KEY `idx_namasupliyer_hutang` (`NAMASUPLIYER`),
  ADD KEY `idx_tgl_lokasi_hutang` (`TGLPEMBAYARAN`,`LOKASI`);

--
-- Indexes for table `hutang_detail`
--
ALTER TABLE `hutang_detail`
  ADD UNIQUE KEY `uq_sync_id_hutang_detail` (`sync_id`),
  ADD KEY `Hutang_Detail_ID_JUAL` (`ID_BELI`),
  ADD KEY `Hutang_Detail_ID_USER_BAYAR` (`ID_USER`),
  ADD KEY `idx_id_bayar_hutang` (`ID_BAYAR`);

--
-- Indexes for table `jurnalumum`
--
ALTER TABLE `jurnalumum`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_jurnalumum` (`sync_id`),
  ADD KEY `idx_no_transaksi_jurnal` (`NO_TRANSAKSI`),
  ADD KEY `idx_tgl_jenis_transaksi` (`TGL_TRANSAKSI`,`JENIS_TRANSAKSI`),
  ADD KEY `idx_id_user_jurnal` (`ID_USER`),
  ADD KEY `idx_nomor_akun_d_jurnal` (`NOMOR_AKUN_D`,`TGL_TRANSAKSI`),
  ADD KEY `idx_nomor_akun_k_jurnal` (`NOMOR_AKUN_K`,`TGL_TRANSAKSI`),
  ADD KEY `idx_tgl_akun_d_nominal` (`TGL_TRANSAKSI`,`NOMOR_AKUN_D`,`NOMINAL`),
  ADD KEY `idx_tgl_akun_k_nominal` (`TGL_TRANSAKSI`,`NOMOR_AKUN_K`,`NOMINAL`),
  ADD KEY `idx_akun_d_nominal` (`NOMOR_AKUN_D`,`NOMINAL`),
  ADD KEY `idx_akun_k_nominal` (`NOMOR_AKUN_K`,`NOMINAL`),
  ADD KEY `idx_tgl_jenis_akun_d_nominal` (`TGL_TRANSAKSI`,`JENIS_TRANSAKSI`,`NOMOR_AKUN_D`,`NOMINAL`),
  ADD KEY `idx_tgl_jenis_akun_k_nominal` (`TGL_TRANSAKSI`,`JENIS_TRANSAKSI`,`NOMOR_AKUN_K`,`NOMINAL`),
  ADD KEY `idx_tgl_id_user_jurnal` (`TGL_TRANSAKSI`,`ID_USER`);

--
-- Indexes for table `pembelian`
--
ALTER TABLE `pembelian`
  ADD PRIMARY KEY (`ID_PEMBELIAN`),
  ADD UNIQUE KEY `uq_sync_id_pembelian` (`sync_id`),
  ADD KEY `idx_tgl_beli` (`TGL_BELI`),
  ADD KEY `idx_supplier_tagihan` (`ID_SUPPLIER`,`TAGIHAN`),
  ADD KEY `idx_id_supplier` (`ID_SUPPLIER`),
  ADD KEY `idx_status_transaksi_beli` (`STATUS_TRANSAKSI_BELI`),
  ADD KEY `idx_nama_supliyer` (`NAMA_SUPLIYER`),
  ADD KEY `idx_jenis_bayar` (`JENIS_BAYAR`),
  ADD KEY `idx_id_user_pembelian` (`ID_USER`),
  ADD KEY `idx_updated_at_beli` (`updated_at`),
  ADD KEY `idx_tgl_jenis_bayar` (`TGL_BELI`,`JENIS_BAYAR`),
  ADD KEY `idx_supplier_status` (`ID_SUPPLIER`,`STATUS_TRANSAKSI_BELI`),
  ADD KEY `idx_jatuh_tempo_beli` (`JATUH_TEMPO`),
  ADD KEY `idx_jatuh_tempo_status_beli` (`JATUH_TEMPO`,`STATUS_TRANSAKSI_BELI`),
  ADD KEY `idx_tgl_bayar_beli` (`TGL_BAYAR`),
  ADD KEY `idx_status_jual_beli` (`STATUS_JUAL`),
  ADD KEY `idx_status_lokasi_beli` (`STATUS_TRANSAKSI_BELI`,`LOKASI`),
  ADD KEY `idx_tgl_lokasi_beli` (`TGL_BELI`,`LOKASI`);

--
-- Indexes for table `pembelian_detail`
--
ALTER TABLE `pembelian_detail`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_pembelian_detail` (`sync_id`),
  ADD KEY `idx_faktur_beli` (`FAKTUR_BELI`),
  ADD KEY `idx_faktur_beli_barang` (`FAKTUR_BELI`,`ID_BARANG`),
  ADD KEY `idx_tanggal_masuk_beli` (`TANGGAL_MASUK`),
  ADD KEY `idx_tgl_masuk_barang` (`TANGGAL_MASUK`,`ID_BARANG`),
  ADD KEY `idx_id_barang_tgl_masuk` (`ID_BARANG`,`TANGGAL_MASUK`),
  ADD KEY `idx_id_barang_beli` (`ID_BARANG`);

--
-- Indexes for table `pembelian_ditahan`
--
ALTER TABLE `pembelian_ditahan`
  ADD PRIMARY KEY (`ID_PEMBELIAN`),
  ADD KEY `idx_id_pembelian_ditahan` (`ID_PEMBELIAN`),
  ADD KEY `idx_lokasi_pembelian_ditahan` (`LOKASI`);

--
-- Indexes for table `penjualan`
--
ALTER TABLE `penjualan`
  ADD PRIMARY KEY (`ID_PENJUALAN`),
  ADD UNIQUE KEY `uq_sync_id_penjualan` (`sync_id`),
  ADD KEY `idx_tgl_transaksi` (`TGL_TRANSAKSI`),
  ADD KEY `idx_pelanggan_tagihan` (`ID_PELANGGAN`,`SISA_TAGIHAN`),
  ADD KEY `idx_id_pelanggan` (`ID_PELANGGAN`),
  ADD KEY `idx_status_transaksi` (`STATUS_TRANSAKSI`),
  ADD KEY `idx_nama_pelanggan_jual` (`NAMA_PELANGGAN`),
  ADD KEY `idx_lokasibarang` (`LOKASIBARANG`),
  ADD KEY `idx_kode_akun_jual` (`KODE_AKUN`),
  ADD KEY `idx_id_user_penjualan` (`ID_USER`),
  ADD KEY `idx_updated_at_jual` (`updated_at`),
  ADD KEY `idx_tgl_kode_akun_jual` (`TGL_TRANSAKSI`,`KODE_AKUN`),
  ADD KEY `idx_tgl_kode_akun_tf` (`TGL_TRANSAKSI`,`KODE_AKUN_TF`),
  ADD KEY `idx_pelanggan_status` (`ID_PELANGGAN`,`STATUS_TRANSAKSI`),
  ADD KEY `idx_lokasi_tanggal` (`LOKASIBARANG`,`TGL_TRANSAKSI`),
  ADD KEY `idx_jatuh_tempo_jual` (`JATUH_TEMPO`),
  ADD KEY `idx_jatuh_tempo_status_jual` (`JATUH_TEMPO`,`STATUS_TRANSAKSI`),
  ADD KEY `idx_tgl_pembayaran_jual` (`TGL_PEMBAYARAN`),
  ADD KEY `idx_status_bayar_jual` (`STATUS_BAYAR`),
  ADD KEY `idx_jenis_pembayaran_jual` (`JENIS_PEMBAYARAN`),
  ADD KEY `idx_nama_sales_jual` (`NAMA_SALES`),
  ADD KEY `idx_id_sales_jual` (`ID_SALES`),
  ADD KEY `idx_id_sales_tgl_jual` (`ID_SALES`,`TGL_TRANSAKSI`),
  ADD KEY `idx_type_akun_jual` (`TYPE_AKUN`),
  ADD KEY `idx_tgl_type_akun_jual` (`TGL_TRANSAKSI`,`TYPE_AKUN`);

--
-- Indexes for table `penjualan_detail`
--
ALTER TABLE `penjualan_detail`
  ADD UNIQUE KEY `uq_sync_id_penjualan_detail` (`sync_id`),
  ADD KEY `idx_faktur_jual` (`FAKTUR_JUAL`),
  ADD KEY `idx_faktur_barang` (`FAKTUR_JUAL`,`ID_BARANG`),
  ADD KEY `idx_tgl_lokasi_jual` (`TANGGAL_JUAL`,`LOKASIBARANG`),
  ADD KEY `idx_pelanggan_tgl_jual` (`ID_PELANGGAN`,`TANGGAL_JUAL`),
  ADD KEY `idx_id_barang_detail_jual` (`ID_BARANG`),
  ADD KEY `idx_tgl_pelanggan_user` (`TANGGAL_JUAL`,`NAMA_PELANGGAN`,`ID_USER`);

--
-- Indexes for table `penjualan_ditahan`
--
ALTER TABLE `penjualan_ditahan`
  ADD KEY `idx_faktur_jual_ditahan` (`FAKTUR_JUAL`);

--
-- Indexes for table `penjualan_ditahan_detail`
--
ALTER TABLE `penjualan_ditahan_detail`
  ADD KEY `idx_faktur_jual_ditahan_detail` (`FAKTUR_JUAL`),
  ADD KEY `idx_id_barang_ditahan_detail` (`ID_BARANG`);

--
-- Indexes for table `piutang`
--
ALTER TABLE `piutang`
  ADD UNIQUE KEY `uq_sync_id_piutang` (`sync_id`),
  ADD KEY `idx_tgl_bayar_piutang` (`TGL_BAYAR`),
  ADD KEY `idx_id_bayar_piutang` (`ID_BAYAR_PIUTANG`),
  ADD KEY `idx_tgl_pelanggan_piutang` (`TGL_BAYAR`,`NAMA_PELANGGAN`),
  ADD KEY `idx_nama_pelanggan_piutang` (`NAMA_PELANGGAN`),
  ADD KEY `idx_tgl_lokasi_piutang` (`TGL_BAYAR`,`LOKASI`);

--
-- Indexes for table `piutang_detail`
--
ALTER TABLE `piutang_detail`
  ADD UNIQUE KEY `uq_sync_id_piutang_detail` (`sync_id`),
  ADD KEY `idx_id_bayar_piutang_detail` (`ID_BAYAR`),
  ADD KEY `idx_id_jual` (`ID_JUAL`);

--
-- Indexes for table `retur_pembelian`
--
ALTER TABLE `retur_pembelian`
  ADD UNIQUE KEY `uq_sync_id_retur_pembelian` (`sync_id`),
  ADD KEY `retur_pembelian_ID_KOMPUTER` (`ID_KOMPUTER`),
  ADD KEY `retur_pembelian_ID_PEMBELIAN` (`ID_PEMBELIAN`),
  ADD KEY `retur_pembelian_ID_RETUR_PEMBELIAN` (`ID_RETUR_PEMBELIAN`),
  ADD KEY `retur_pembelian_ID_SUPPLIER` (`ID_SUPPLIER`),
  ADD KEY `idx_tgl_retur_beli` (`TGL_RETUR_BELI`),
  ADD KEY `idx_kode_rekening_retur_beli` (`KODE_REKENING`),
  ADD KEY `idx_id_user_retur_beli` (`ID_USER`),
  ADD KEY `idx_nama_rekening_retur_beli` (`NAMA_REKENING`);

--
-- Indexes for table `retur_pembelian_detail`
--
ALTER TABLE `retur_pembelian_detail`
  ADD UNIQUE KEY `uq_sync_id_retur_pembelian_detail` (`sync_id`),
  ADD KEY `idx_id_retur_pembelian` (`ID_RETUR_PEMBELIAN`),
  ADD KEY `idx_tgl_retur_beli_detail` (`TGL_RETUR_BELI`),
  ADD KEY `idx_tgl_supplier_retur_beli` (`TGL_RETUR_BELI`,`NAMA_SUPLIYER`),
  ADD KEY `idx_penyimpanan_retur_beli_detail` (`PENYIMPANAN`);

--
-- Indexes for table `retur_penjualan`
--
ALTER TABLE `retur_penjualan`
  ADD UNIQUE KEY `uq_sync_id_retur_penjualan` (`sync_id`),
  ADD KEY `idx_tgl_retur_jual` (`TGL_RETUR_JUAL`),
  ADD KEY `idx_id_penjualan_retur` (`ID_PENJUALAN`),
  ADD KEY `idx_id_retur_penjualan_header` (`ID_RETUR_PENJUALAN`),
  ADD KEY `idx_kode_rekening_retur_jual` (`KODE_REKENING`),
  ADD KEY `idx_id_user_retur_jual` (`ID_USER`),
  ADD KEY `idx_nama_rekening_retur_jual` (`NAMA_REKENING`);

--
-- Indexes for table `retur_penjualan_detail`
--
ALTER TABLE `retur_penjualan_detail`
  ADD UNIQUE KEY `uq_sync_id_retur_penjualan_detail` (`sync_id`),
  ADD KEY `idx_id_retur_penjualan` (`ID_RETUR_PENJUALAN`),
  ADD KEY `idx_retur_jual_barang` (`ID_RETUR_PENJUALAN`,`ID_BARANG`),
  ADD KEY `idx_tgl_retur_jual_detail` (`TGL_RETUR_JUAL`),
  ADD KEY `idx_tgl_pelanggan_retur_jual` (`TGL_RETUR_JUAL`,`NAMA_PELANGGAN`),
  ADD KEY `idx_lokasi_retur_jual_detail` (`LOKASI`);

--
-- Indexes for table `saldo_tahunan`
--
ALTER TABLE `saldo_tahunan`
  ADD PRIMARY KEY (`TAHUN`,`KODE_AKUN`);

--
-- Indexes for table `stoktambahkurang`
--
ALTER TABLE `stoktambahkurang`
  ADD UNIQUE KEY `uq_sync_id_stoktambahkurang` (`sync_id`),
  ADD KEY `idx_tanggal_stok_tk` (`TANGGAL`),
  ADD KEY `idx_lokasi_stok_tk` (`LOKASI`),
  ADD KEY `idx_id_barang_stok_tk` (`ID_BARANG`),
  ADD KEY `idx_faktur_stok_tk` (`FAKTUR`);

--
-- Indexes for table `stok_opname`
--
ALTER TABLE `stok_opname`
  ADD UNIQUE KEY `uq_sync_id_stok_opname` (`sync_id`),
  ADD KEY `idx_tanggal_opname` (`TANGGAL`),
  ADD KEY `idx_id_stok_opname` (`ID_STOK_OPNAME`),
  ADD KEY `idx_id_barang_opname` (`ID_BARANG`),
  ADD KEY `idx_id_user_opname` (`ID_USER`),
  ADD KEY `idx_barang_tanggal` (`ID_BARANG`,`TANGGAL`);

--
-- Indexes for table `surat_jalan`
--
ALTER TABLE `surat_jalan`
  ADD PRIMARY KEY (`NOTA`),
  ADD UNIQUE KEY `uq_sync_id_surat_jalan` (`sync_id`),
  ADD KEY `idx_tgl_pengiriman` (`TGL_PENGIRIMAN`),
  ADD KEY `idx_nota_sj` (`NOTA`),
  ADD KEY `idx_kode_supir_tgl` (`KODE_SUPIR`,`TGL_PENGIRIMAN`),
  ADD KEY `idx_kode_helper1_tgl` (`KODE_HELPER1`,`TGL_PENGIRIMAN`),
  ADD KEY `idx_kode_helper2_tgl` (`KODE_HELPER2`,`TGL_PENGIRIMAN`);

--
-- Indexes for table `surat_jalan_detail`
--
ALTER TABLE `surat_jalan_detail`
  ADD UNIQUE KEY `uq_sync_id_surat_jalan_detail` (`sync_id`),
  ADD KEY `idx_nota_sj_detail` (`NOTA`);

--
-- Indexes for table `sync_config`
--
ALTER TABLE `sync_config`
  ADD PRIMARY KEY (`kunci`);

--
-- Indexes for table `sync_log`
--
ALTER TABLE `sync_log`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_waktu_log` (`waktu`),
  ADD KEY `idx_jenis_log` (`jenis`);

--
-- Indexes for table `sync_queue`
--
ALTER TABLE `sync_queue`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_status_queue` (`status`),
  ADD KEY `idx_tabel_queue` (`tabel`);

--
-- Indexes for table `tbl_armada`
--
ALTER TABLE `tbl_armada`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_armada` (`sync_id`),
  ADD KEY `idx_nopol_armada` (`NOPOL`),
  ADD KEY `idx_updated_at_armada` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`);

--
-- Indexes for table `tbl_barang`
--
ALTER TABLE `tbl_barang`
  ADD PRIMARY KEY (`ID_BARANG`),
  ADD UNIQUE KEY `uq_sync_id_tbl_barang` (`sync_id`),
  ADD KEY `idx_nama_barang` (`NAMA_BARANG`),
  ADD KEY `idx_barcode_kecil` (`BARCODE_KECIL`),
  ADD KEY `idx_barcode_sedang` (`BARCODE_SEDANG`),
  ADD KEY `idx_barcode_besar` (`BARCODE_BESAR`),
  ADD KEY `idx_updated_at_barang` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_stok_minimum` (`STOK_MIN`,`STOK_TOKO`,`STOK_GUDANG`),
  ADD KEY `idx_stok_toko_gudang` (`STOK_TOKO`,`STOK_GUDANG`),
  ADD KEY `idx_kategori_barang` (`NAMA_KATEGORI`),
  ADD KEY `idx_kode_kategori_barang` (`KODE_KATEGORI`),
  ADD KEY `idx_status_barang` (`STATUS`),
  ADD KEY `idx_status_nama_barang` (`STATUS`,`NAMA_BARANG`),
  ADD KEY `idx_id_barang_prefix` (`ID_BARANG`);

--
-- Indexes for table `tbl_cabang`
--
ALTER TABLE `tbl_cabang`
  ADD PRIMARY KEY (`kode_cabang`),
  ADD UNIQUE KEY `uq_sync_id_tbl_cabang` (`sync_id`),
  ADD KEY `idx_nama_cabang` (`nama_cabang`),
  ADD KEY `idx_sumber_cabang` (`sumber`),
  ADD KEY `idx_updated_at_cabang` (`updated_at`),
  ADD KEY `idx_id_cloud_cabang` (`id_cloud`);

--
-- Indexes for table `tbl_datareferensi`
--
ALTER TABLE `tbl_datareferensi`
  ADD PRIMARY KEY (`KODE_AKUN`),
  ADD UNIQUE KEY `uq_sync_id_tbl_datareferensi` (`sync_id`),
  ADD KEY `idx_nama_akun` (`NAMA_AKUN`),
  ADD KEY `idx_type_akun` (`TYPE_AKUN`),
  ADD KEY `idx_kode_akun_ref` (`KODE_AKUN`),
  ADD KEY `idx_sub_akun` (`SUB_AKUN`),
  ADD KEY `idx_jenis_akun` (`JENIS_AKUN`);

--
-- Indexes for table `tbl_gaji`
--
ALTER TABLE `tbl_gaji`
  ADD PRIMARY KEY (`KODE`);

--
-- Indexes for table `tbl_karyawan`
--
ALTER TABLE `tbl_karyawan`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_karyawan` (`sync_id`),
  ADD KEY `idx_nama_karyawan` (`NAMA`),
  ADD KEY `idx_status_nama` (`Status`,`NAMA`),
  ADD KEY `idx_kode_karyawan` (`KODE`),
  ADD KEY `idx_saldo_akhir_karyawan` (`SALDOAKHIR`);

--
-- Indexes for table `tbl_kategori`
--
ALTER TABLE `tbl_kategori`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_kategori` (`sync_id`),
  ADD KEY `idx_updated_at_kategori` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_nama_kategori` (`NAMA`),
  ADD KEY `idx_kode_kategori` (`KODE`);

--
-- Indexes for table `tbl_merk`
--
ALTER TABLE `tbl_merk`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_merk` (`sync_id`),
  ADD KEY `idx_updated_at_merk` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_nama_merk` (`NAMA`),
  ADD KEY `idx_kode_merk` (`KODE`);

--
-- Indexes for table `tbl_pelanggan`
--
ALTER TABLE `tbl_pelanggan`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_pelanggan` (`sync_id`),
  ADD KEY `idx_nama_pelanggan` (`NAMA`),
  ADD KEY `idx_updated_at_pelanggan` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_status_pelanggan` (`Status`),
  ADD KEY `idx_status_nama_pelanggan` (`Status`,`NAMA`);

--
-- Indexes for table `tbl_perusahaan`
--
ALTER TABLE `tbl_perusahaan`
  ADD PRIMARY KEY (`KODE`);

--
-- Indexes for table `tbl_satuan`
--
ALTER TABLE `tbl_satuan`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_satuan` (`sync_id`),
  ADD KEY `idx_nama_satuan` (`NAMA`),
  ADD KEY `idx_updated_at_satuan` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`);

--
-- Indexes for table `tbl_supliyer`
--
ALTER TABLE `tbl_supliyer`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_supliyer` (`sync_id`),
  ADD KEY `idx_nama_supliyer` (`NAMA`),
  ADD KEY `idx_status_nama` (`Status`,`NAMA`),
  ADD KEY `idx_updated_at_supliyer` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`);

--
-- Indexes for table `tbl_user`
--
ALTER TABLE `tbl_user`
  ADD PRIMARY KEY (`KODE_USER`),
  ADD UNIQUE KEY `uq_sync_id_tbl_user` (`sync_id`),
  ADD KEY `idx_status_user` (`status`),
  ADD KEY `idx_username_user` (`USER_NAME`),
  ADD KEY `idx_username_pwd_status` (`USER_NAME`,`PWD`,`status`);

--
-- Indexes for table `temp_bbpembantu`
--
ALTER TABLE `temp_bbpembantu`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `temp_bon_karyawan`
--
ALTER TABLE `temp_bon_karyawan`
  ADD PRIMARY KEY (`NO`);

--
-- Indexes for table `temp_datareferensi`
--
ALTER TABLE `temp_datareferensi`
  ADD PRIMARY KEY (`KODE_AKUN`);

--
-- Indexes for table `transfer_barang`
--
ALTER TABLE `transfer_barang`
  ADD PRIMARY KEY (`ID_TRANSFER`),
  ADD UNIQUE KEY `uq_sync_id_transfer_barang` (`sync_id`),
  ADD KEY `idx_tgl_transfer_barang` (`TGL_TRANSFER`);

--
-- Indexes for table `transfer_barang_detail`
--
ALTER TABLE `transfer_barang_detail`
  ADD UNIQUE KEY `uq_sync_id_transfer_barang_detail` (`sync_id`),
  ADD KEY `idx_id_transfer_barang_detail` (`ID_TRANSFER`),
  ADD KEY `idx_id_barang_transfer` (`ID_BARANG`),
  ADD KEY `idx_transfer_barang_id` (`ID_TRANSFER`,`ID_BARANG`),
  ADD KEY `idx_tgl_transfer_detail` (`TGL_TRANSFER`);

--
-- Indexes for table `transfer_cabang`
--
ALTER TABLE `transfer_cabang`
  ADD PRIMARY KEY (`ID_TRANSFER`),
  ADD UNIQUE KEY `uq_sync_id_transfer_cabang` (`sync_id`),
  ADD KEY `idx_tgl_transfer_cabang` (`TGL_TRANSFER`),
  ADD KEY `idx_status_transfer_cabang` (`STATUS_TRANSFER`),
  ADD KEY `idx_mode_kirim_cabang` (`MODE_KIRIM`),
  ADD KEY `idx_dari_ke_cabang` (`DARI_CABANG`,`KE_CABANG`),
  ADD KEY `idx_ke_status_cabang` (`KE_CABANG`,`STATUS_TRANSFER`),
  ADD KEY `idx_cloud_transfer_cabang` (`ID_CLOUD_TRANSFER`);

--
-- Indexes for table `transfer_cabang_detail`
--
ALTER TABLE `transfer_cabang_detail`
  ADD UNIQUE KEY `uq_sync_id_transfer_cabang_detail` (`sync_id`),
  ADD KEY `idx_id_transfer_cabang_detail` (`ID_TRANSFER`),
  ADD KEY `idx_id_barang_transfer_cabang` (`ID_BARANG`),
  ADD KEY `idx_transfer_cabang_id_barang` (`ID_TRANSFER`,`ID_BARANG`),
  ADD KEY `idx_tgl_transfer_cabang_detail` (`TGL_TRANSFER`),
  ADD KEY `idx_status_item_transfer_cabang` (`STATUS_ITEM`);

--
-- Indexes for table `transfer_keluar_offline`
--
ALTER TABLE `transfer_keluar_offline`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_status` (`status`),
  ADD KEY `idx_id_transfer` (`id_transfer`);

--
-- Indexes for table `transfer_masuk_manual`
--
ALTER TABLE `transfer_masuk_manual`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_id_cloud` (`id_cloud`),
  ADD KEY `idx_status` (`status_transfer`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_id_transfer_masuk_manual` (`id_transfer`),
  ADD KEY `idx_status_transfer_masuk_manual` (`status_transfer`),
  ADD KEY `idx_ke_status_transfer_masuk_manual` (`ke_cabang`,`status_transfer`),
  ADD KEY `idx_kode_barang_transfer_masuk_manual` (`kode_barang`);

--
-- Indexes for table `transfer_stok`
--
ALTER TABLE `transfer_stok`
  ADD UNIQUE KEY `uq_sync_id_transfer_stok` (`sync_id`),
  ADD KEY `idx_tanggal_transfer_stok` (`TANGGAL`),
  ADD KEY `idx_id_transfer_stok` (`ID_TRANSFER`);

--
-- Indexes for table `transfer_terima_pending`
--
ALTER TABLE `transfer_terima_pending`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_cloud_kode` (`id_cloud`,`kode_barang`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `hakaksesuser`
--
ALTER TABLE `hakaksesuser`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `history`
--
ALTER TABLE `history`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `historybarang`
--
ALTER TABLE `historybarang`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `jurnalumum`
--
ALTER TABLE `jurnalumum`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `pembelian_detail`
--
ALTER TABLE `pembelian_detail`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `sync_log`
--
ALTER TABLE `sync_log`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `sync_queue`
--
ALTER TABLE `sync_queue`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `temp_bbpembantu`
--
ALTER TABLE `temp_bbpembantu`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `transfer_keluar_offline`
--
ALTER TABLE `transfer_keluar_offline`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `transfer_masuk_manual`
--
ALTER TABLE `transfer_masuk_manual`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `transfer_terima_pending`
--
ALTER TABLE `transfer_terima_pending`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
