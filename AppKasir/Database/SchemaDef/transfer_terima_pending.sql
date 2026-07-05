CREATE TABLE IF NOT EXISTS `transfer_terima_pending` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_cloud` varchar(50) NOT NULL,
  `kode_barang` varchar(50) NOT NULL,
  `id_user` varchar(50) DEFAULT NULL,
  `tgl_terima` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` varchar(20) NOT NULL DEFAULT 'PENDING',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_cloud_kode` (`id_cloud`,`kode_barang`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
