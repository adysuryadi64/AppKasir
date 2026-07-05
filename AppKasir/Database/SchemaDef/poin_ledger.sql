CREATE TABLE IF NOT EXISTS `poin_ledger` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `KODE_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'FK ke tbl_pelanggan.KODE',
  `TIPE` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'EARN | REDEEM | VOID_EARN',
  `JUMLAH_POIN` int(11) NOT NULL DEFAULT '0' COMMENT 'Positif=tambah, Negatif=kurang',
  `NO_REFERENSI` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Nomor faktur penjualan atau nomor penukaran TP-xxx',
  `KETERANGAN` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `uq_sync_id_poin_ledger` (`sync_id`),
  KEY `idx_poin_ledger_pelanggan` (`KODE_PELANGGAN`),
  KEY `idx_poin_ledger_referensi` (`NO_REFERENSI`),
  KEY `idx_poin_ledger_created_at` (`created_at`),
  KEY `idx_poin_ledger_updated_at` (`updated_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
