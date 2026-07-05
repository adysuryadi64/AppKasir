CREATE TABLE IF NOT EXISTS `transfer_barang` (
  `ID_TRANSFER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_QTY` decimal(10,2) DEFAULT '0.00',
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT NULL,
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID_TRANSFER`),
  UNIQUE KEY `uq_sync_id_transfer_barang` (`sync_id`),
  KEY `idx_tgl_transfer_barang` (`TGL_TRANSFER`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
