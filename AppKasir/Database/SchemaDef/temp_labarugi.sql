CREATE TABLE IF NOT EXISTS `temp_labarugi` (
  `TANGGAL` datetime DEFAULT NULL,
  `BULAN` int(11) DEFAULT '0',
  `TOTAL` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
