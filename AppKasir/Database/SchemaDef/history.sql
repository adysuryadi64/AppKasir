CREATE TABLE IF NOT EXISTS `history` (
  `NO` int(11) NOT NULL AUTO_INCREMENT,
  `TANGGAL` datetime DEFAULT NULL,
  `Aksi` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`NO`),
  UNIQUE KEY `uq_sync_id_history` (`sync_id`),
  KEY `idx_tanggal_history` (`TANGGAL`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
