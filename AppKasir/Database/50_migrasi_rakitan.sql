-- ============================================================
-- MIGRASI: Fitur Barang Rakitan (Kitting / Bundle)
-- Jalankan via FormMigrasiDB → pilih file ini
-- ============================================================

-- 1. BOM: Resep komponen per paket rakitan
CREATE TABLE IF NOT EXISTS tbl_rakitan_bom (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    kode_rakitan  VARCHAR(50)    NOT NULL COMMENT 'ID_BARANG paket rakitan',
    kode_komponen VARCHAR(50)    NOT NULL COMMENT 'ID_BARANG komponen',
    nama_komponen VARCHAR(150)   NOT NULL DEFAULT '',
    qty           DECIMAL(10,2) NOT NULL DEFAULT 1,
    satuan        VARCHAR(20)   NOT NULL DEFAULT '',
    urutan        INT           NOT NULL DEFAULT 0,
    UNIQUE KEY uq_bom (kode_rakitan, kode_komponen)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. Header transaksi rakit / bongkar
CREATE TABLE IF NOT EXISTS tbl_proses_rakitan (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    no_rakitan    VARCHAR(30)   NOT NULL UNIQUE,
    tanggal       DATETIME      NOT NULL,
    kode_rakitan  VARCHAR(50)   NOT NULL,
    nama_rakitan  VARCHAR(150)  NOT NULL DEFAULT '',
    qty_hasil     DECIMAL(10,2) NOT NULL DEFAULT 1,
    jenis         VARCHAR(10)   NOT NULL DEFAULT 'Rakit' COMMENT 'Rakit atau Bongkar',
    lokasi        VARCHAR(20)   NOT NULL DEFAULT 'TOKO',
    id_ref        INT           NULL COMMENT 'Untuk Bongkar: id transaksi Rakit asal',
    keterangan    VARCHAR(255)  NOT NULL DEFAULT '',
    id_user       VARCHAR(50)   NOT NULL DEFAULT '',
    id_komputer   VARCHAR(50)   NOT NULL DEFAULT '',
    created_at    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. Detail komponen yang dipakai/dikembalikan per transaksi
CREATE TABLE IF NOT EXISTS tbl_proses_rakitan_detail (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    id_proses     INT           NOT NULL,
    kode_komponen VARCHAR(50)   NOT NULL,
    nama_komponen VARCHAR(150)  NOT NULL DEFAULT '',
    qty           DECIMAL(10,2) NOT NULL DEFAULT 0,
    satuan        VARCHAR(20)   NOT NULL DEFAULT '',
    INDEX idx_id_proses (id_proses)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
