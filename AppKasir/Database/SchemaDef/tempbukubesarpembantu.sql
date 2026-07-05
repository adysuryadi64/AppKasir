CREATE TABLE IF NOT EXISTS `tempbukubesarpembantu` (
  `NOMOR` int(11) DEFAULT '0',
  `JENISTRANSAKSI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTRANSAKSI` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLTRANSAKSI` datetime DEFAULT NULL,
  `NONOTA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
