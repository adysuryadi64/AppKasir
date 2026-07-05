CREATE TABLE IF NOT EXISTS `transfer_keluar_offline` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_transfer` varchar(30) NOT NULL,
  `dari_cabang` varchar(50) NOT NULL,
  `ke_cabang` varchar(50) NOT NULL,
  `kode_barang` varchar(50) NOT NULL,
  `nama_barang` varchar(150) DEFAULT NULL,
  `qty` decimal(10,2) DEFAULT '0.00',
  `satuan` varchar(20) DEFAULT NULL,
  `isi_satuan` int(11) DEFAULT '1',
  `qty_satuan` decimal(10,2) DEFAULT '0.00',
  `keterangan` varchar(255) DEFAULT NULL,
  `status` varchar(20) NOT NULL DEFAULT 'PENDING',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_id_transfer` (`id_transfer`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
