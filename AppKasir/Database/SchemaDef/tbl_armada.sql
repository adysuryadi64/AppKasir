CREATE TABLE IF NOT EXISTS `tbl_armada` (
  `KODE` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NOPOL` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `JENIS` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`KODE`),
  UNIQUE KEY `uq_sync_id_tbl_armada` (`sync_id`),
  KEY `idx_nopol_armada` (`NOPOL`),
  KEY `idx_updated_at_armada` (`updated_at`),
  KEY `idx_is_dirty` (`is_dirty`),
  KEY `idx_id_cloud` (`id_cloud`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
