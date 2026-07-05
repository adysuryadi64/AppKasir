CREATE TABLE IF NOT EXISTS `tbl_audit_trail` (
  `id_audit` int(11) NOT NULL AUTO_INCREMENT,
  `waktu_aksi` datetime NOT NULL,
  `jenis_aksi` char(12) NOT NULL COMMENT 'HAPUS | EDIT | TAMBAH_STOK | KURANG_STOK',
  `jenis_trans` varchar(20) NOT NULL COMMENT 'Penjualan | Pembelian | Master User | dll',
  `identifier` varchar(35) NOT NULL COMMENT 'no_faktur atau PREFIX:nilai untuk master',
  `id_user` varchar(30) NOT NULL,
  `lokasi` char(6) DEFAULT NULL COMMENT 'TOKO atau GUDANG',
  `komputer` varchar(30) DEFAULT NULL,
  `ket` text COMMENT '[KRITIS]/[MENENGAH] | faktur | tgl | nominal | nama | status',
  PRIMARY KEY (`id_audit`),
  KEY `idx_audit_waktu` (`waktu_aksi`),
  KEY `idx_audit_user` (`id_user`),
  KEY `idx_audit_id` (`identifier`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Audit trail edit dan hapus transaksi serta data master';
