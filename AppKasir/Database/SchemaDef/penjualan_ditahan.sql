CREATE TABLE IF NOT EXISTS `penjualan_ditahan` (
  `FAKTUR_JUAL` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GRAN_TOTAL` decimal(15,0) DEFAULT '0',
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_ITEM` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  KEY `idx_faktur_jual_ditahan` (`FAKTUR_JUAL`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
