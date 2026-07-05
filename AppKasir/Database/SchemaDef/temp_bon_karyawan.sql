CREATE TABLE IF NOT EXISTS `temp_bon_karyawan` (
  `NO` int(11) NOT NULL DEFAULT '0',
  `NOMOR` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(15,0) DEFAULT '0',
  `KREDIT` decimal(15,0) DEFAULT '0',
  `SALDO` decimal(15,0) DEFAULT '0',
  PRIMARY KEY (`NO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
