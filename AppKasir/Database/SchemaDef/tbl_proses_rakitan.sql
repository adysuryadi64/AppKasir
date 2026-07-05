CREATE TABLE IF NOT EXISTS `tbl_proses_rakitan` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `no_rakitan` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `tanggal` datetime NOT NULL,
  `kode_rakitan` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_rakitan` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `qty_hasil` decimal(10,2) NOT NULL DEFAULT '1.00',
  `jenis` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Rakit' COMMENT 'Rakit atau Bongkar',
  `lokasi` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'TOKO',
  `id_ref` int(11) DEFAULT NULL COMMENT 'Untuk Bongkar: id transaksi Rakit asal',
  `keterangan` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `id_user` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `id_komputer` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `no_rakitan` (`no_rakitan`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
