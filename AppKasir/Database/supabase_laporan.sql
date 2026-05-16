-- ============================================================
-- SUPABASE LAPORAN - Jalankan di Supabase SQL Editor
-- Tabel snapshot + view untuk aplikasi laporan pihak ketiga
-- Aman dijalankan berulang kali (idempotent)
--
-- KONSEP:
-- Tabel snapshot diisi saat toko upload (UPSERT per toko).
-- View menggabungkan data dari semua toko untuk laporan.
-- Aplikasi laporan hanya perlu GET ke view — tidak perlu JOIN.
-- ============================================================

-- ============================================================
-- 1. SNAPSHOT STOK BARANG PER CABANG
--    Setiap cabang upload stok toko + gudang miliknya sendiri.
--    Kolom sesuai persis dengan tbl_barang MySQL.
-- ============================================================
CREATE TABLE IF NOT EXISTS stok_per_cabang (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    id_barang    VARCHAR(50) NOT NULL,
    UNIQUE (kode_cabang, id_barang)
);

ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS nama_barang                  VARCHAR(200);
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS kode_kategori                VARCHAR(30);
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS nama_kategori                VARCHAR(50);
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS kode_supliyer                VARCHAR(50);
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS nama_supliyer                VARCHAR(100);
-- Stok toko
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS awal_toko                    NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS tambah_toko                  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS kurang_toko                  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS pembelian_toko               NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS penjualan_toko               NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS retur_beli_toko              NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS retur_jual_toko              NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS opname_toko                  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_stok_masuk_toko     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_stok_keluar_toko    NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_barang_masuk_toko   NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_barang_keluar_toko  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS stok_toko                    NUMERIC(10,2) DEFAULT 0;
-- Stok gudang (setiap cabang punya gudang sendiri)
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS awal_gudang                  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS tambah_gudang                NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS kurang_gudang                NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS pembelian_gudang             NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS penjualan_gudang             NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS retur_beli_gudang            NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS retur_jual_gudang            NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS opname_gudang                NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_stok_masuk_gudang   NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_stok_keluar_gudang  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_barang_masuk_gudang NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS transfer_barang_keluar_gudang NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS stok_gudang                  NUMERIC(10,2) DEFAULT 0;
-- Harga
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_beli                   NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_beli_terakhir          NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_jual_umum_kecil        NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_jual_umum_sedang       NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_jual_umum_besar        NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_jual_partai_kecil      NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_jual_partai_sedang     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS harga_jual_partai_besar      NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS stok_min                     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS stok_max                     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE stok_per_cabang ADD COLUMN IF NOT EXISTS updated_at                   TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_spc_kode_cabang ON stok_per_cabang (kode_cabang);
CREATE INDEX IF NOT EXISTS idx_spc_id_barang   ON stok_per_cabang (id_barang);
CREATE INDEX IF NOT EXISTS idx_spc_updated     ON stok_per_cabang (updated_at);

-- ============================================================
-- 2. SNAPSHOT HUTANG SUPPLIER PER CABANG
-- ============================================================
CREATE TABLE IF NOT EXISTS hutang_supliyer_snapshot (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    kode         VARCHAR(20) NOT NULL,
    UNIQUE (kode_cabang, kode)
);
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS nama        VARCHAR(100);
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS alamat      VARCHAR(200);
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS hp          VARCHAR(15);
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS jangkahutang INT DEFAULT 0;
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS hutangawal  NUMERIC(15,0) DEFAULT 0;
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS totalhutang NUMERIC(15,0) DEFAULT 0;
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS totalbayar  NUMERIC(15,0) DEFAULT 0;
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS hutangakhir NUMERIC(15,0) DEFAULT 0;
ALTER TABLE hutang_supliyer_snapshot ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_hss_kode_cabang ON hutang_supliyer_snapshot (kode_cabang);
CREATE INDEX IF NOT EXISTS idx_hss_kode        ON hutang_supliyer_snapshot (kode);

-- ============================================================
-- 3. SNAPSHOT PIUTANG PELANGGAN PER CABANG
-- ============================================================
CREATE TABLE IF NOT EXISTS piutang_pelanggan_snapshot (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    kode         VARCHAR(20) NOT NULL,
    UNIQUE (kode_cabang, kode)
);
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS nama          VARCHAR(50);
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS alamat        VARCHAR(100);
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS no_telp       VARCHAR(15);
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS jenis         VARCHAR(20);
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS jangkapiutang SMALLINT DEFAULT 0;
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS hutangawal    NUMERIC(15,0) DEFAULT 0;
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS totalhutang   NUMERIC(15,0) DEFAULT 0;
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS totalbayar    NUMERIC(15,0) DEFAULT 0;
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS hutangakhir   NUMERIC(15,0) DEFAULT 0;
ALTER TABLE piutang_pelanggan_snapshot ADD COLUMN IF NOT EXISTS updated_at    TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_pps_kode_cabang ON piutang_pelanggan_snapshot (kode_cabang);
CREATE INDEX IF NOT EXISTS idx_pps_kode        ON piutang_pelanggan_snapshot (kode);

-- ============================================================
-- 4. SNAPSHOT KARYAWAN & GAJI PER CABANG
-- ============================================================
CREATE TABLE IF NOT EXISTS karyawan_snapshot (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    kode         VARCHAR(10) NOT NULL,
    UNIQUE (kode_cabang, kode)
);
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS nama       VARCHAR(50);
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS jabatan    VARCHAR(50);
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS tglmasuk   TIMESTAMPTZ;
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS gaji       NUMERIC(10,0) DEFAULT 0;
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS saldoawal  NUMERIC(15,0) DEFAULT 0;
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS totalbon   NUMERIC(15,0) DEFAULT 0;
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS totalbayar NUMERIC(15,0) DEFAULT 0;
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS saldoakhir NUMERIC(15,0) DEFAULT 0;
ALTER TABLE karyawan_snapshot ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_ks_kode_cabang ON karyawan_snapshot (kode_cabang);

-- Ringkasan gaji per bulan per cabang
CREATE TABLE IF NOT EXISTS gaji_ringkasan_snapshot (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    bulan        VARCHAR(20) NOT NULL,
    kode         VARCHAR(20) NOT NULL,
    UNIQUE (kode_cabang, bulan, kode)
);
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS nama        VARCHAR(50);
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS pokok       NUMERIC(15,0) DEFAULT 0;
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS pendapatan  NUMERIC(15,0) DEFAULT 0;
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS potongan    NUMERIC(15,0) DEFAULT 0;
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS terima      NUMERIC(15,0) DEFAULT 0;
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS tanggal     TIMESTAMPTZ;
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS lokasi      VARCHAR(20);
ALTER TABLE gaji_ringkasan_snapshot ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_grs_kode_cabang ON gaji_ringkasan_snapshot (kode_cabang);
CREATE INDEX IF NOT EXISTS idx_grs_bulan       ON gaji_ringkasan_snapshot (bulan);

-- ============================================================
-- 5. SNAPSHOT COA / AKUN (tbl_datareferensi) PER CABANG
--    Termasuk saldo neraca dan laba rugi
-- ============================================================
CREATE TABLE IF NOT EXISTS coa_snapshot (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    kode_akun    VARCHAR(20) NOT NULL,
    UNIQUE (kode_cabang, kode_akun)
);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS status          VARCHAR(10);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS jenis_akun      VARCHAR(50);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS type_akun       VARCHAR(30);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS nama_akun       VARCHAR(100);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS sub_akun        VARCHAR(20);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS akun_dk         VARCHAR(20);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS akun_nrlr       VARCHAR(20);
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS saldo_awal      NUMERIC(20,0) DEFAULT 0;
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS saldo_sebelumnya NUMERIC(20,0) DEFAULT 0;
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS s_debet         NUMERIC(20,0) DEFAULT 0;
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS s_kredit        NUMERIC(20,0) DEFAULT 0;
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS saldo_akhir     NUMERIC(20,0) DEFAULT 0;
ALTER TABLE coa_snapshot ADD COLUMN IF NOT EXISTS updated_at      TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_coa_kode_cabang ON coa_snapshot (kode_cabang);
CREATE INDEX IF NOT EXISTS idx_coa_kode_akun   ON coa_snapshot (kode_akun);
CREATE INDEX IF NOT EXISTS idx_coa_akun_nrlr   ON coa_snapshot (akun_nrlr);
CREATE INDEX IF NOT EXISTS idx_coa_type_akun   ON coa_snapshot (type_akun);

-- ============================================================
-- 6. VIEW LAPORAN — siap pakai untuk aplikasi pihak ketiga
--    Akses via Supabase REST: GET /rest/v1/v_laporan_barang
-- ============================================================

-- View: master barang lengkap + stok total semua toko
CREATE OR REPLACE VIEW v_laporan_barang AS
SELECT
    b.id_barang,
    b.id_barang_bantu,
    b.nama_barang,
    b.nama_barang_bantu,
    b.jenis,
    b.kode_kategori,
    b.nama_kategori,
    b.kode_supliyer,
    b.nama_supliyer,
    b.jenis_satuan,
    b.harga_beli,
    b.harga_beli_terakhir,
    b.hpp_umum_kecil,    b.hpp_umum_sedang,    b.hpp_umum_besar,
    b.harga_beli_umum_kecil, b.harga_beli_umum_sedang, b.harga_beli_umum_besar,
    b.hpp_partai_kecil,  b.hpp_partai_sedang,  b.hpp_partai_besar,
    b.harga_beli_partai_kecil, b.harga_beli_upartai_sedang, b.harga_beli_partai_besar,
    b.barcode_kecil,     b.barcode_sedang,     b.barcode_besar,
    b.satuan_umum_kecil, b.satuan_umum_sedang, b.satuan_umum_besar,
    b.isi_umum_kecil,    b.isi_umum_sedang,    b.isi_umum_besar,
    b.harga_jual_umum_kecil, b.harga_jual_umum_sedang, b.harga_jual_umum_besar,
    b.satuan_partai_kecil, b.satuan_partai_sedang, b.satuan_partai_besar,
    b.isi_partai_kecil,  b.isi_partai_sedang,  b.isi_partai_besar,
    b.harga_jual_partai_kecil, b.harga_jual_partai_sedang, b.harga_jual_partai_besar,
    b.satuan_stok,       b.satuan_isi_stok,
    b.stok_min,          b.stok_max,
    b.lokasi_rak_toko,   b.lokasi_rak_gudang,
    b.point_member,      b.point_karyawan,
    b.komisi_sales_rp,   b.komisi_sales_persen,
    -- Stok agregat semua cabang
    COALESCE(SUM(s.stok_toko),   0) AS total_stok_toko,
    COALESCE(SUM(s.stok_gudang), 0) AS total_stok_gudang,
    COALESCE(SUM(s.stok_toko + s.stok_gudang), 0) AS total_stok,
    -- Nilai stok (harga beli × total stok)
    COALESCE(SUM((s.stok_toko + s.stok_gudang) * b.harga_beli), 0) AS nilai_stok,
    b.updated_at,
    b.kode_cabang_asal
FROM barang_master b
LEFT JOIN stok_per_cabang s ON s.id_barang = b.id_barang
GROUP BY b.id, b.id_barang, b.id_barang_bantu, b.nama_barang, b.nama_barang_bantu,
    b.jenis, b.kode_kategori, b.nama_kategori, b.kode_supliyer, b.nama_supliyer,
    b.jenis_satuan, b.harga_beli, b.harga_beli_terakhir,
    b.hpp_umum_kecil, b.hpp_umum_sedang, b.hpp_umum_besar,
    b.harga_beli_umum_kecil, b.harga_beli_umum_sedang, b.harga_beli_umum_besar,
    b.hpp_partai_kecil, b.hpp_partai_sedang, b.hpp_partai_besar,
    b.harga_beli_partai_kecil, b.harga_beli_upartai_sedang, b.harga_beli_partai_besar,
    b.barcode_kecil, b.barcode_sedang, b.barcode_besar,
    b.satuan_umum_kecil, b.satuan_umum_sedang, b.satuan_umum_besar,
    b.isi_umum_kecil, b.isi_umum_sedang, b.isi_umum_besar,
    b.harga_jual_umum_kecil, b.harga_jual_umum_sedang, b.harga_jual_umum_besar,
    b.satuan_partai_kecil, b.satuan_partai_sedang, b.satuan_partai_besar,
    b.isi_partai_kecil, b.isi_partai_sedang, b.isi_partai_besar,
    b.harga_jual_partai_kecil, b.harga_jual_partai_sedang, b.harga_jual_partai_besar,
    b.satuan_stok, b.satuan_isi_stok, b.stok_min, b.stok_max,
    b.lokasi_rak_toko, b.lokasi_rak_gudang,
    b.point_member, b.point_karyawan, b.komisi_sales_rp, b.komisi_sales_persen,
    b.updated_at, b.kode_cabang_asal;

-- View: stok detail per cabang (toko + gudang dipisah)
CREATE OR REPLACE VIEW v_stok_detail_per_cabang AS
SELECT
    s.kode_cabang,
    s.id_barang,
    s.nama_barang,
    s.kode_kategori,
    s.nama_kategori,
    s.kode_supliyer,
    s.nama_supliyer,
    s.stok_toko,
    s.stok_gudang,
    (s.stok_toko + s.stok_gudang)          AS stok_total,
    s.awal_toko,    s.pembelian_toko,    s.penjualan_toko,
    s.retur_beli_toko, s.retur_jual_toko, s.opname_toko,
    s.transfer_stok_masuk_toko, s.transfer_stok_keluar_toko,
    s.transfer_barang_masuk_toko, s.transfer_barang_keluar_toko,
    s.awal_gudang,  s.pembelian_gudang,  s.penjualan_gudang,
    s.retur_beli_gudang, s.retur_jual_gudang, s.opname_gudang,
    s.transfer_stok_masuk_gudang, s.transfer_stok_keluar_gudang,
    s.transfer_barang_masuk_gudang, s.transfer_barang_keluar_gudang,
    s.harga_beli,
    s.harga_jual_umum_kecil, s.harga_jual_umum_sedang, s.harga_jual_umum_besar,
    s.harga_jual_partai_kecil, s.harga_jual_partai_sedang, s.harga_jual_partai_besar,
    s.stok_min,     s.stok_max,
    ((s.stok_toko + s.stok_gudang) * s.harga_beli) AS nilai_stok,
    s.updated_at
FROM stok_per_cabang s;

-- View: hutang supplier semua cabang
CREATE OR REPLACE VIEW v_hutang_supliyer AS
SELECT
    h.kode_cabang,
    h.kode,
    h.nama,
    h.alamat,
    h.hp,
    h.jangkahutang,
    h.hutangawal,
    h.totalhutang,
    h.totalbayar,
    h.hutangakhir,
    h.updated_at
FROM hutang_supliyer_snapshot h;

-- View: hutang supplier diringkas per supplier (semua cabang)
CREATE OR REPLACE VIEW v_hutang_supliyer_total AS
SELECT
    kode,
    MAX(nama)          AS nama,
    SUM(hutangawal)    AS total_hutangawal,
    SUM(totalhutang)   AS total_hutang,
    SUM(totalbayar)    AS total_bayar,
    SUM(hutangakhir)   AS total_sisa_hutang,
    MAX(updated_at)    AS last_update
FROM hutang_supliyer_snapshot
GROUP BY kode;

-- View: piutang pelanggan semua cabang
CREATE OR REPLACE VIEW v_piutang_pelanggan AS
SELECT
    p.kode_cabang,
    p.kode,
    p.nama,
    p.alamat,
    p.no_telp,
    p.jenis,
    p.jangkapiutang,
    p.hutangawal,
    p.totalhutang,
    p.totalbayar,
    p.hutangakhir,
    p.updated_at
FROM piutang_pelanggan_snapshot p;

-- View: piutang pelanggan diringkas per pelanggan (semua cabang)
CREATE OR REPLACE VIEW v_piutang_pelanggan_total AS
SELECT
    kode,
    MAX(nama)          AS nama,
    SUM(hutangawal)    AS total_piutangawal,
    SUM(totalhutang)   AS total_piutang,
    SUM(totalbayar)    AS total_bayar,
    SUM(hutangakhir)   AS total_sisa_piutang,
    MAX(updated_at)    AS last_update
FROM piutang_pelanggan_snapshot
GROUP BY kode;

-- View: karyawan semua cabang
CREATE OR REPLACE VIEW v_karyawan AS
SELECT
    k.kode_cabang,
    k.kode,
    k.nama,
    k.jabatan,
    k.tglmasuk,
    k.gaji,
    k.saldoawal,
    k.totalbon,
    k.totalbayar,
    k.saldoakhir,
    k.updated_at
FROM karyawan_snapshot k;

-- View: ringkasan gaji per bulan semua cabang
CREATE OR REPLACE VIEW v_gaji_ringkasan AS
SELECT
    g.kode_cabang,
    g.bulan,
    g.kode,
    g.nama,
    g.pokok,
    g.pendapatan,
    g.potongan,
    g.terima,
    g.tanggal,
    g.lokasi,
    g.updated_at
FROM gaji_ringkasan_snapshot g;

-- View: total gaji per bulan per cabang
CREATE OR REPLACE VIEW v_gaji_total_per_bulan AS
SELECT
    kode_cabang,
    bulan,
    COUNT(*)           AS jumlah_karyawan,
    SUM(pendapatan)    AS total_pendapatan,
    SUM(potongan)      AS total_potongan,
    SUM(terima)        AS total_terima,
    MAX(updated_at)    AS last_update
FROM gaji_ringkasan_snapshot
GROUP BY kode_cabang, bulan;

-- View: COA / Neraca per cabang
CREATE OR REPLACE VIEW v_neraca AS
SELECT
    c.kode_cabang,
    c.kode_akun,
    c.nama_akun,
    c.jenis_akun,
    c.type_akun,
    c.sub_akun,
    c.akun_dk,
    c.akun_nrlr,
    c.saldo_awal,
    c.saldo_sebelumnya,
    c.s_debet,
    c.s_kredit,
    c.saldo_akhir,
    c.status,
    c.updated_at
FROM coa_snapshot c
WHERE c.akun_nrlr = 'NERACA';

-- View: Laba Rugi per cabang
CREATE OR REPLACE VIEW v_laba_rugi AS
SELECT
    c.kode_cabang,
    c.kode_akun,
    c.nama_akun,
    c.jenis_akun,
    c.type_akun,
    c.sub_akun,
    c.akun_dk,
    c.saldo_awal,
    c.s_debet,
    c.s_kredit,
    c.saldo_akhir,
    c.updated_at
FROM coa_snapshot c
WHERE c.akun_nrlr = 'LABA RUGI';

-- View: Ringkasan laba rugi per cabang
CREATE OR REPLACE VIEW v_ringkasan_laba_rugi AS
SELECT
    kode_cabang,
    SUM(CASE WHEN sub_akun = 'LABA'  THEN saldo_akhir ELSE 0 END) AS total_pendapatan,
    SUM(CASE WHEN sub_akun = 'RUGI'  THEN saldo_akhir ELSE 0 END) AS total_beban,
    SUM(CASE WHEN sub_akun = 'LABA'  THEN saldo_akhir ELSE 0 END)
  - SUM(CASE WHEN sub_akun = 'RUGI'  THEN saldo_akhir ELSE 0 END) AS laba_bersih,
    MAX(updated_at) AS last_update
FROM coa_snapshot
WHERE akun_nrlr = 'LABA RUGI'
GROUP BY kode_cabang;

-- ============================================================
-- 7. RLS — nonaktifkan untuk development
-- ============================================================
ALTER TABLE stok_per_cabang            DISABLE ROW LEVEL SECURITY;
ALTER TABLE hutang_supliyer_snapshot   DISABLE ROW LEVEL SECURITY;
ALTER TABLE piutang_pelanggan_snapshot DISABLE ROW LEVEL SECURITY;
ALTER TABLE karyawan_snapshot          DISABLE ROW LEVEL SECURITY;
ALTER TABLE gaji_ringkasan_snapshot    DISABLE ROW LEVEL SECURITY;
ALTER TABLE coa_snapshot               DISABLE ROW LEVEL SECURITY;

-- ============================================================
-- ENDPOINT YANG TERSEDIA UNTUK APLIKASI LAPORAN
-- (via Supabase REST API — GET /rest/v1/nama_view)
-- ============================================================
-- v_laporan_barang          → master barang + stok total semua cabang
-- v_stok_detail_per_cabang  → stok toko + gudang per cabang per barang
-- v_hutang_supliyer         → hutang supplier per cabang
-- v_hutang_supliyer_total   → hutang supplier diringkas semua cabang
-- v_piutang_pelanggan       → piutang pelanggan per cabang
-- v_piutang_pelanggan_total → piutang pelanggan diringkas semua cabang
-- v_karyawan                → data karyawan semua cabang
-- v_gaji_ringkasan          → gaji per karyawan per bulan
-- v_gaji_total_per_bulan    → total gaji per bulan per cabang
-- v_neraca                  → akun neraca per cabang
-- v_laba_rugi               → akun laba rugi per cabang
-- v_ringkasan_laba_rugi     → ringkasan laba bersih per cabang
