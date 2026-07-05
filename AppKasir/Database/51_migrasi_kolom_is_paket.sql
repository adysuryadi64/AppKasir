-- ============================================================
-- MIGRASI 51: Tambah kolom IS_PAKET ke tbl_barang
-- Menandai apakah suatu barang adalah paket rakitan (bundle)
-- Default 0 = barang biasa, 1 = paket rakitan
-- ============================================================

ALTER TABLE tbl_barang
  ADD COLUMN IS_PAKET TINYINT(1) NOT NULL DEFAULT 0
  COMMENT '0=Barang biasa, 1=Paket rakitan/bundle'
  AFTER STATUS;

-- Index untuk filter cepat di pencarian paket
CREATE INDEX idx_barang_is_paket ON tbl_barang (IS_PAKET);

-- Sinkronisasi otomatis: tandai semua barang yang sudah punya BOM sebagai paket
UPDATE tbl_barang b
  INNER JOIN (
    SELECT DISTINCT kode_rakitan FROM tbl_rakitan_bom
  ) r ON r.kode_rakitan = b.ID_BARANG
SET b.IS_PAKET = 1;
