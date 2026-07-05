CREATE TABLE IF NOT EXISTS `poin_config` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `AKTIF` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0=Tidak Aktif, 1=Aktif',
  `MEKANISME` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PER_ITEM' COMMENT 'PER_ITEM atau PER_NOMINAL',
  `POIN_PER_QTY` decimal(10,2) NOT NULL DEFAULT '1.00' COMMENT 'Poin per 1 qty satuan item (dipakai saat PER_ITEM)',
  `KELIPATAN_NOMINAL` decimal(15,0) NOT NULL DEFAULT '10000' COMMENT 'Nilai Rp per 1 poin (dipakai saat PER_NOMINAL)',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `uq_sync_id_poin_config` (`sync_id`),
  KEY `idx_poin_config_updated_at` (`updated_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Konfigurasi aturan earn poin & data sinkronisasi';
