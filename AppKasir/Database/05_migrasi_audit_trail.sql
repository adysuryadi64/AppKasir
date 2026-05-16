-- ============================================================
-- Migrasi Transaction Audit Trail
-- Membuat tabel tbl_audit_trail dan tbl_audit_trail_arsip
-- beserta index yang diperlukan.
-- Aman dijalankan berulang kali (idempoten)
-- Jalankan setelah memilih database yang benar (USE nama_db;)
--
-- Format penyimpanan:
--   Kolom `ket` (TEXT) menyimpan plain text ringkas:
--   "[KRITIS] Hapus penjualan | PJ-001 | 2026-04-20 | Rp 1.500.000 | Budi | Lunas"
--   Tujuan: jejak audit siapa mengubah apa kapan — BUKAN untuk restore data.
--   Tidak ada kompresi, tidak ada JSON, langsung bisa dibaca di FormAuditTrail.
-- ============================================================

-- ── 1. Tabel aktif: tbl_audit_trail ─────────────────────────
CREATE TABLE IF NOT EXISTS `tbl_audit_trail` (
    `id_audit`      INT           NOT NULL AUTO_INCREMENT,
    `waktu_aksi`    DATETIME      NOT NULL,
    `jenis_aksi`    CHAR(12)      NOT NULL COMMENT 'HAPUS | EDIT | TAMBAH_STOK | KURANG_STOK',
    `jenis_trans`   VARCHAR(20)   NOT NULL COMMENT 'Penjualan | Pembelian | Master User | dll',
    `identifier`    VARCHAR(35)   NOT NULL COMMENT 'no_faktur atau PREFIX:nilai untuk master',
    `id_user`       VARCHAR(30)   NOT NULL,
    `lokasi`        CHAR(6)       NULL     COMMENT 'TOKO atau GUDANG',
    `komputer`      VARCHAR(30)   NULL,
    `ket`           TEXT          NULL     COMMENT '[KRITIS]/[MENENGAH] | faktur | tgl | nominal | nama | status',
    PRIMARY KEY (`id_audit`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
  COMMENT='Audit trail edit dan hapus transaksi serta data master';

-- ── 2. Tabel arsip: tbl_audit_trail_arsip ───────────────────
-- Struktur identik dengan tbl_audit_trail
-- Record lama (> retensi bulan) dipindahkan ke sini secara otomatis
CREATE TABLE IF NOT EXISTS `tbl_audit_trail_arsip` LIKE `tbl_audit_trail`;

-- ── 3. Index tbl_audit_trail ─────────────────────────────────
-- Hanya 3 kolom dengan kardinalitas tinggi
-- jenis_aksi dan jenis_trans TIDAK diindex (kardinalitas rendah)

-- idx_audit_waktu — filter rentang tanggal
SET @idx = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail'
      AND INDEX_NAME   = 'idx_audit_waktu'
);
SET @sql = IF(@idx = 0,
    'CREATE INDEX idx_audit_waktu ON tbl_audit_trail (waktu_aksi)',
    'SELECT ''idx_audit_waktu sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- idx_audit_user — filter berdasarkan user
SET @idx = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail'
      AND INDEX_NAME   = 'idx_audit_user'
);
SET @sql = IF(@idx = 0,
    'CREATE INDEX idx_audit_user ON tbl_audit_trail (id_user)',
    'SELECT ''idx_audit_user sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- idx_audit_id — filter berdasarkan identifier (no_faktur / PREFIX:nilai)
SET @idx = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail'
      AND INDEX_NAME   = 'idx_audit_id'
);
SET @sql = IF(@idx = 0,
    'CREATE INDEX idx_audit_id ON tbl_audit_trail (identifier)',
    'SELECT ''idx_audit_id sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ── 4. Index tbl_audit_trail_arsip ───────────────────────────
-- Index identik agar query arsip sama cepatnya

SET @idx = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail_arsip'
      AND INDEX_NAME   = 'idx_audit_waktu'
);
SET @sql = IF(@idx = 0,
    'CREATE INDEX idx_audit_waktu ON tbl_audit_trail_arsip (waktu_aksi)',
    'SELECT ''idx_audit_waktu arsip sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @idx = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail_arsip'
      AND INDEX_NAME   = 'idx_audit_user'
);
SET @sql = IF(@idx = 0,
    'CREATE INDEX idx_audit_user ON tbl_audit_trail_arsip (id_user)',
    'SELECT ''idx_audit_user arsip sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @idx = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail_arsip'
      AND INDEX_NAME   = 'idx_audit_id'
);
SET @sql = IF(@idx = 0,
    'CREATE INDEX idx_audit_id ON tbl_audit_trail_arsip (identifier)',
    'SELECT ''idx_audit_id arsip sudah ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ── 5. Seed konfigurasi default di hakaksesuser ───────────────
-- Retensi aktif default: 3 bulan
-- Gunakan INSERT IGNORE agar tidak overwrite nilai yang sudah dikonfigurasi admin
INSERT IGNORE INTO `hakaksesuser`
    (`UserName`, `Role`, `ModuleName`, `CanRead`, `CanAdd`, `CanEdit`, `CanDelete`)
VALUES
    ('SYSTEM', 'AuditRetensi',      '3',  0, 0, 0, 0),
    ('SYSTEM', 'AuditArsipTerakhir','',   0, 0, 0, 0);

-- ── 6. Migrasi tabel yang sudah ada (jika dijalankan ulang) ──
-- Hapus kolom data_sebelum jika masih ada (format lama pakai MEDIUMBLOB)
-- Perlebar kolom ket dari VARCHAR(100) ke TEXT
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail'
      AND COLUMN_NAME  = 'data_sebelum'
);
SET @sql = IF(@col > 0,
    'ALTER TABLE tbl_audit_trail DROP COLUMN data_sebelum',
    'SELECT ''data_sebelum sudah tidak ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail_arsip'
      AND COLUMN_NAME  = 'data_sebelum'
);
SET @sql = IF(@col > 0,
    'ALTER TABLE tbl_audit_trail_arsip DROP COLUMN data_sebelum',
    'SELECT ''data_sebelum arsip sudah tidak ada, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Perlebar ket ke TEXT jika masih VARCHAR
SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail'
      AND COLUMN_NAME  = 'ket'
      AND DATA_TYPE    = 'varchar'
);
SET @sql = IF(@col > 0,
    'ALTER TABLE tbl_audit_trail MODIFY COLUMN ket TEXT NULL COMMENT ''[KRITIS]/[MENENGAH] | faktur | tgl | nominal | nama | status''',
    'SELECT ''ket sudah TEXT, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'tbl_audit_trail_arsip'
      AND COLUMN_NAME  = 'ket'
      AND DATA_TYPE    = 'varchar'
);
SET @sql = IF(@col > 0,
    'ALTER TABLE tbl_audit_trail_arsip MODIFY COLUMN ket TEXT NULL',
    'SELECT ''ket arsip sudah TEXT, dilewati'' AS info'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ── Selesai ───────────────────────────────────────────────────
SELECT 'Migrasi audit trail selesai.' AS info;
