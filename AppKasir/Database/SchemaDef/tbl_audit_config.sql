CREATE TABLE IF NOT EXISTS `tbl_audit_config` (
  `config_key` varchar(50) NOT NULL,
  `config_value` varchar(100) NOT NULL DEFAULT '',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`config_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Konfigurasi modul audit trail (retensi, tanggal arsip terakhir)';
