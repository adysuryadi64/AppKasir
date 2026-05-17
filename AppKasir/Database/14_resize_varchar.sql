-- =============================================================================
-- 14_resize_varchar.sql
-- Perkecil definisi VARCHAR berdasarkan panjang data aktual di db_moroseneng
--
-- Latar belakang:
--   VARCHAR di InnoDB disimpan sesuai panjang aktual (tidak terpengaruh definisi).
--   TAPI index dialokasikan berdasarkan panjang DEFINISI — bukan aktual.
--   Contoh: NAMA_PELANGGAN VARCHAR(100) di-index → buffer 100 byte per baris.
--   Jika max aktual hanya 35 byte, resize ke VARCHAR(60) hemat ~40% ukuran index.
--
-- Analisis data aktual (dari db_moroseneng, diukur sebelum migrasi):
--   NAMA_BARANG      : max 41 byte  → resize 200/100/50 → 60
--   NAMA_PELANGGAN   : max 35 byte  → resize 100/50     → 60
--   NAMA_SUPLIYER    : max 31 byte  → resize 100        → 50
--   ALAMAT_PELANGGAN : max 31 byte  → resize 200        → 60
--   AKUN_D / AKUN_K  : max 34 byte  → resize 100        → 60
--   NAMA_AKUN_D/K    : max 26 byte  → resize 50         → 40
--   NO_TRANSAKSI     : max 14 byte  → resize 50         → 30
--   JENIS (history)  : max 22 byte  → resize 50         → 30
--   URAIAN           : max 181 byte → TIDAK diubah (terlalu dekat batas)
--
-- Margin keamanan: ukuran baru = CEIL(max_aktual * 1.5) dibulatkan ke atas
-- Aman dijalankan berulang kali — ALTER TABLE hanya jalan jika ukuran berbeda
-- TIDAK menghapus data. TIDAK mengubah index (index otomatis rebuild oleh MySQL).
-- =============================================================================

SELECT '=== STEP 1: Cek panjang aktual sebelum resize ===' AS status;

SELECT
    'penjualan_detail.NAMA_PELANGGAN' AS kolom, MAX(LENGTH(NAMA_PELANGGAN)) AS max_aktual, 100 AS definisi_lama, 60 AS definisi_baru FROM penjualan_detail
UNION ALL SELECT 'penjualan_detail.NAMA_BARANG',    MAX(LENGTH(NAMA_BARANG)),    100, 60 FROM penjualan_detail
UNION ALL SELECT 'historybarang.NAMA_BARANG',       MAX(LENGTH(NAMA_BARANG)),     50, 60 FROM historybarang
UNION ALL SELECT 'historybarang.JENIS',             MAX(LENGTH(JENIS)),           50, 30 FROM historybarang
UNION ALL SELECT 'historybarang.FAKTUR',            MAX(LENGTH(FAKTUR)),          20, 30 FROM historybarang
UNION ALL SELECT 'jurnalumum.AKUN_D',               MAX(LENGTH(AKUN_D)),         100, 60 FROM jurnalumum
UNION ALL SELECT 'jurnalumum.AKUN_K',               MAX(LENGTH(AKUN_K)),         100, 60 FROM jurnalumum
UNION ALL SELECT 'jurnalumum.NAMA_AKUN_D',          MAX(LENGTH(NAMA_AKUN_D)),     50, 40 FROM jurnalumum
UNION ALL SELECT 'jurnalumum.NAMA_AKUN_K',          MAX(LENGTH(NAMA_AKUN_K)),     50, 40 FROM jurnalumum
UNION ALL SELECT 'jurnalumum.NO_TRANSAKSI',         MAX(LENGTH(NO_TRANSAKSI)),    50, 30 FROM jurnalumum
UNION ALL SELECT 'penjualan.NAMA_PELANGGAN',        MAX(LENGTH(NAMA_PELANGGAN)), 100, 60 FROM penjualan
UNION ALL SELECT 'penjualan.ALAMAT_PELANGGAN',      MAX(LENGTH(ALAMAT_PELANGGAN)),200,60 FROM penjualan
UNION ALL SELECT 'tbl_barang.NAMA_BARANG',          MAX(LENGTH(NAMA_BARANG)),    200, 60 FROM tbl_barang
UNION ALL SELECT 'tbl_barang.NAMA_SUPLIYER',        MAX(LENGTH(NAMA_SUPLIYER)),  100, 50 FROM tbl_barang
UNION ALL SELECT 'hutang.NAMASUPLIYER',             MAX(LENGTH(NAMASUPLIYER)),   100, 50 FROM hutang
UNION ALL SELECT 'piutang.NAMA_PELANGGAN',          MAX(LENGTH(NAMA_PELANGGAN)), 100, 60 FROM piutang
UNION ALL SELECT 'pembelian_detail.NAMA_BARANG',    MAX(LENGTH(NAMA_BARANG)),    100, 60 FROM pembelian_detail
UNION ALL SELECT 'pembelian_detail.NAMA_SUPLIYER',  MAX(LENGTH(NAMA_SUPLIYER)),  100, 50 FROM pembelian_detail
UNION ALL SELECT 'stok_opname.NAMA_BARANG',         MAX(LENGTH(NAMA_BARANG)),    100, 60 FROM stok_opname
UNION ALL SELECT 'stoktambahkurang.NAMA_BARANG',    MAX(LENGTH(NAMA_BARANG)),    100, 60 FROM stoktambahkurang
UNION ALL SELECT 'transfer_barang_detail.NAMA_BARANG', MAX(LENGTH(NAMA_BARANG)),100, 60 FROM transfer_barang_detail
UNION ALL SELECT 'retur_penjualan_detail.NAMA_BARANG', MAX(LENGTH(NAMA_BARANG)),100, 60 FROM retur_penjualan_detail
UNION ALL SELECT 'retur_penjualan_detail.NAMA_PELANGGAN', MAX(LENGTH(NAMA_PELANGGAN)),100,60 FROM retur_penjualan_detail
ORDER BY max_aktual DESC;

SELECT '=== STEP 2: ALTER TABLE resize VARCHAR ===' AS status;

-- ── historybarang (1.1 juta baris — tabel terbesar, dampak terbesar) ──────────
-- FAKTUR diperlebar dari varchar(20) ke varchar(30) agar konsisten dengan
-- transfer_cabang.ID_TRANSFER varchar(30) dan transfer_stok.ID_TRANSFER varchar(20).
-- Data aktual max 14 karakter (TS-2604010135) — aman, tidak ada data terpotong.
ALTER TABLE historybarang
    MODIFY COLUMN FAKTUR      VARCHAR(30)  NULL,
    MODIFY COLUMN NAMA_BARANG VARCHAR(60)  NULL,
    MODIFY COLUMN JENIS       VARCHAR(30)  NULL;

-- ── jurnalumum (627 ribu baris) ───────────────────────────────────────────────
ALTER TABLE jurnalumum
    MODIFY COLUMN AKUN_D       VARCHAR(60)  NULL,
    MODIFY COLUMN AKUN_K       VARCHAR(60)  NULL,
    MODIFY COLUMN NAMA_AKUN_D  VARCHAR(40)  NULL,
    MODIFY COLUMN NAMA_AKUN_K  VARCHAR(40)  NULL,
    MODIFY COLUMN NO_TRANSAKSI VARCHAR(30)  NOT NULL;

-- ── penjualan_detail (855 ribu baris) ─────────────────────────────────────────
ALTER TABLE penjualan_detail
    MODIFY COLUMN NAMA_PELANGGAN VARCHAR(60)  NULL,
    MODIFY COLUMN NAMA_BARANG    VARCHAR(60)  NULL;

-- ── penjualan (161 ribu baris) ────────────────────────────────────────────────
ALTER TABLE penjualan
    MODIFY COLUMN NAMA_PELANGGAN    VARCHAR(60)  NULL,
    MODIFY COLUMN ALAMAT_PELANGGAN  VARCHAR(60)  NULL;

-- ── tbl_barang (master barang) ────────────────────────────────────────────────
ALTER TABLE tbl_barang
    MODIFY COLUMN NAMA_BARANG   VARCHAR(60)  NOT NULL,
    MODIFY COLUMN NAMA_SUPLIYER VARCHAR(50)  NULL;

-- ── tbl_kategori / tbl_satuan / tbl_merk — standarisasi panjang KODE ─────────
-- CATATAN PENTING: transaksi TIDAK menyimpan KODE dari ketiga tabel ini.
-- Yang disimpan di transaksi adalah NAMA satuan langsung (denormalisasi).
-- Kolom KODE hanya dipakai di tbl_barang sebagai referensi master.
--
-- Kategori : KODE max aktual 3 karakter → VARCHAR(4) (margin 1 untuk fallback duplikat)
-- Satuan   : KODE max aktual 4 karakter di produksi (contoh: "Pack", "Rntg") → VARCHAR(5)
-- Merk     : KODE max aktual 3 karakter → VARCHAR(4) (margin 1 untuk fallback duplikat)
--
-- tbl_barang.SATUAN_* menyimpan NAMA satuan (bukan KODE) — max aktual 7 ("Renteng")
-- → resize ke VARCHAR(15) memberi margin 2x dari max aktual
-- tbl_barang.KODE_KATEGORI dan KODE_MERK menyimpan KODE → VARCHAR(4)
ALTER TABLE tbl_kategori
    MODIFY COLUMN KODE VARCHAR(4)  NOT NULL;

ALTER TABLE tbl_satuan
    MODIFY COLUMN KODE VARCHAR(5) NOT NULL;

ALTER TABLE tbl_merk
    MODIFY COLUMN KODE VARCHAR(4)  NOT NULL;

ALTER TABLE tbl_barang
    MODIFY COLUMN KODE_KATEGORI        VARCHAR(4)  NULL,
    MODIFY COLUMN KODE_MERK            VARCHAR(4)  NULL,
    MODIFY COLUMN SATUAN_UMUM_KECIL    VARCHAR(15) NULL,
    MODIFY COLUMN SATUAN_UMUM_SEDANG   VARCHAR(15) NULL,
    MODIFY COLUMN SATUAN_UMUM_BESAR    VARCHAR(15) NULL,
    MODIFY COLUMN SATUAN_PARTAI_KECIL  VARCHAR(15) NULL,
    MODIFY COLUMN SATUAN_PARTAI_SEDANG VARCHAR(15) NULL,
    MODIFY COLUMN SATUAN_PARTAI_BESAR  VARCHAR(15) NULL,
    MODIFY COLUMN SATUAN_STOK          VARCHAR(15) NULL;

-- ── hutang ────────────────────────────────────────────────────────────────────
ALTER TABLE hutang
    MODIFY COLUMN NAMASUPLIYER VARCHAR(50)  NULL;

-- ── piutang ───────────────────────────────────────────────────────────────────
ALTER TABLE piutang
    MODIFY COLUMN NAMA_PELANGGAN VARCHAR(60)  NULL;

-- ── pembelian_detail ──────────────────────────────────────────────────────────
ALTER TABLE pembelian_detail
    MODIFY COLUMN NAMA_BARANG   VARCHAR(60)  NULL,
    MODIFY COLUMN NAMA_SUPLIYER VARCHAR(50)  NULL;

-- ── stok_opname ───────────────────────────────────────────────────────────────
ALTER TABLE stok_opname
    MODIFY COLUMN NAMA_BARANG VARCHAR(60)  NULL;

-- ── stoktambahkurang ──────────────────────────────────────────────────────────
ALTER TABLE stoktambahkurang
    MODIFY COLUMN NAMA_BARANG VARCHAR(60)  NULL;

-- ── transfer_barang_detail ────────────────────────────────────────────────────
ALTER TABLE transfer_barang_detail
    MODIFY COLUMN NAMA_BARANG VARCHAR(60)  NULL;

-- ── retur_penjualan_detail ────────────────────────────────────────────────────
ALTER TABLE retur_penjualan_detail
    MODIFY COLUMN NAMA_BARANG    VARCHAR(60)  NULL,
    MODIFY COLUMN NAMA_PELANGGAN VARCHAR(60)  NULL;

-- ── retur_pembelian_detail ────────────────────────────────────────────────────
ALTER TABLE retur_pembelian_detail
    MODIFY COLUMN NAMA_BARANG   VARCHAR(60)  NULL,
    MODIFY COLUMN NAMA_SUPLIYER VARCHAR(50)  NULL;

-- ── pembelian_ditahan_detail ──────────────────────────────────────────────────
ALTER TABLE pembelian_ditahan_detail
    MODIFY COLUMN NAMA_BARANG   VARCHAR(60)  NULL,
    MODIFY COLUMN NAMA_SUPLIYER VARCHAR(50)  NULL;

-- ── penjualan_ditahan_detail ──────────────────────────────────────────────────
ALTER TABLE penjualan_ditahan_detail
    MODIFY COLUMN NAMA_BARANG VARCHAR(60)  NULL;

-- =============================================================================
-- STEP 3: OPTIMIZE + ANALYZE untuk rebuild index dengan ukuran baru
-- =============================================================================
SELECT '=== STEP 3: OPTIMIZE tabel yang di-ALTER ===' AS status;

OPTIMIZE TABLE historybarang;
OPTIMIZE TABLE jurnalumum;
OPTIMIZE TABLE penjualan_detail;
OPTIMIZE TABLE penjualan;
OPTIMIZE TABLE tbl_barang;
OPTIMIZE TABLE tbl_kategori;
OPTIMIZE TABLE tbl_satuan;
OPTIMIZE TABLE tbl_merk;
OPTIMIZE TABLE hutang;OPTIMIZE TABLE piutang;
OPTIMIZE TABLE pembelian_detail;
OPTIMIZE TABLE stok_opname;
OPTIMIZE TABLE stoktambahkurang;
OPTIMIZE TABLE transfer_barang_detail;
OPTIMIZE TABLE retur_penjualan_detail;
OPTIMIZE TABLE retur_pembelian_detail;
OPTIMIZE TABLE pembelian_ditahan_detail;
OPTIMIZE TABLE penjualan_ditahan_detail;

ANALYZE TABLE historybarang;
ANALYZE TABLE jurnalumum;
ANALYZE TABLE penjualan_detail;
ANALYZE TABLE penjualan;
ANALYZE TABLE tbl_barang;
ANALYZE TABLE tbl_kategori;
ANALYZE TABLE tbl_satuan;
ANALYZE TABLE tbl_merk;

-- =============================================================================
-- STEP 4: Verifikasi ukuran setelah resize
-- =============================================================================
SELECT '=== STEP 4: Ukuran tabel setelah resize ===' AS status;

SELECT
    table_name AS Tabel,
    ROUND((data_length + index_length)/1024/1024, 2) AS Ukuran_MB,
    ROUND(data_length/1024/1024, 2) AS Data_MB,
    ROUND(index_length/1024/1024, 2) AS Index_MB,
    table_rows AS Estimasi_Baris
FROM information_schema.tables
WHERE table_schema = DATABASE()
  AND table_name IN (
    'historybarang','jurnalumum','penjualan_detail','penjualan',
    'tbl_barang','tbl_kategori','tbl_satuan','tbl_merk',
    'hutang','piutang','pembelian_detail',
    'stok_opname','stoktambahkurang','transfer_barang_detail',
    'retur_penjualan_detail','retur_pembelian_detail'
  )
ORDER BY (data_length + index_length) DESC;

SELECT
    ROUND(SUM(data_length + index_length)/1024/1024, 2) AS Total_MB,
    ROUND(SUM(data_length + index_length)/1024/1024/1024, 4) AS Total_GB
FROM information_schema.tables
WHERE table_schema = DATABASE();

-- =============================================================================
-- Catatan perubahan:
--   v1 (awal)   : resize NAMA_BARANG, JENIS, dan kolom lain
--   v2 (2026-05): tambah FAKTUR varchar(20)→varchar(30) untuk konsistensi
--                 dengan transfer_cabang.ID_TRANSFER varchar(30)
-- =============================================================================
SELECT '=== 14_resize_varchar selesai ===' AS status;
