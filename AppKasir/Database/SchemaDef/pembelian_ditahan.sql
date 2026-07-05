CREATE TABLE IF NOT EXISTS `pembelian_ditahan` (
  `ID_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_SUPPLIER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BELI` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_BAYAR` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GRAND_TOTAL_BELI` decimal(15,0) DEFAULT NULL,
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` decimal(10,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `RETUR` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID_PEMBELIAN`),
  KEY `idx_id_pembelian_ditahan` (`ID_PEMBELIAN`),
  KEY `idx_lokasi_pembelian_ditahan` (`LOKASI`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
