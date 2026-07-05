CREATE TABLE IF NOT EXISTS `poin_barang` (
  `ID_BARANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'FK ke tbl_barang.ID_BARANG',
  `HARGA_POIN` int(11) NOT NULL DEFAULT '0' COMMENT 'Jumlah poin yang dibutuhkan untuk 1 unit barang ini',
  `AKTIF` tinyint(4) NOT NULL DEFAULT '1' COMMENT '1=Tersedia untuk ditukar, 0=Tidak tersedia',
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1',
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID_BARANG`),
  UNIQUE KEY `uq_sync_id_poin_barang` (`sync_id`),
  KEY `idx_poin_barang_aktif` (`AKTIF`),
  KEY `idx_poin_barang_is_dirty` (`is_dirty`),
  KEY `idx_poin_barang_id_cloud` (`id_cloud`),
  KEY `idx_poin_barang_updated_at` (`updated_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
