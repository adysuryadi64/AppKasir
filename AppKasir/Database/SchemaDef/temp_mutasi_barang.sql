CREATE TABLE IF NOT EXISTS `temp_mutasi_barang` (
  `FAKTUR` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_MASUK` decimal(10,2) DEFAULT '0.00',
  `QTY_KELUAR` decimal(10,2) DEFAULT '0.00',
  `SALDO` decimal(10,2) DEFAULT '0.00',
  `ID_USER` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
