CREATE TABLE IF NOT EXISTS `tbl_proses_rakitan_detail` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_proses` int(11) NOT NULL,
  `kode_komponen` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_komponen` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `qty` decimal(10,2) NOT NULL DEFAULT '0.00',
  `satuan` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `idx_id_proses` (`id_proses`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
