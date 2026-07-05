CREATE TABLE IF NOT EXISTS `tbl_rakitan_bom` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kode_rakitan` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ID_BARANG paket rakitan',
  `kode_komponen` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ID_BARANG komponen',
  `nama_komponen` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `qty` decimal(10,2) NOT NULL DEFAULT '1.00',
  `satuan` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `urutan` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_bom` (`kode_rakitan`,`kode_komponen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
