-- ============================================================
-- SUPABASE SETUP - Jalankan di Supabase SQL Editor
-- Aman dijalankan berulang kali (idempotent)
-- Tidak merusak data atau kolom yang sudah ada
--
-- Yang TIDAK disimpan di cloud: kolom stok per cabang
-- (STOK_TOKO, STOK_GUDANG, AWAL_TOKO, TAMBAH_TOKO, dll)
-- karena stok dikelola lokal masing-masing cabang.
-- Semua field master barang lainnya DISINKRONKAN.
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ============================================================
-- FUNGSI TRIGGER updated_at
-- ============================================================
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ============================================================
-- 1. barang_master
--    Semua field master barang dari tbl_barang lokal,
--    KECUALI kolom stok (dikelola lokal per toko).
-- ============================================================
CREATE TABLE IF NOT EXISTS barang_master (
    id        UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    id_barang VARCHAR(50) NOT NULL,
    UNIQUE (id_barang)
);

-- Identitas
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS id_barang_bantu   VARCHAR(50);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS nama_barang       VARCHAR(200);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS nama_barang_bantu VARCHAR(100);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS jenis             VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS kode_kategori     VARCHAR(30);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS nama_kategori     VARCHAR(50);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS kode_supliyer     VARCHAR(50);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS nama_supliyer     VARCHAR(100);
-- tinyint(1) di MySQL → SMALLINT di PostgreSQL
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS serial_number     SMALLINT;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS jenis_satuan      VARCHAR(50);

-- Harga beli & HPP (dibutuhkan untuk sinkronisasi harga antar toko)
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli                NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_terakhir       NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS hpp_umum_kecil            NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS hpp_umum_sedang           NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS hpp_umum_besar            NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_umum_kecil     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_umum_sedang    NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_umum_besar     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS hpp_partai_kecil          NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS hpp_partai_sedang         NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS hpp_partai_besar          NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_partai_kecil   NUMERIC(10,2) DEFAULT 0;
-- Nama kolom ini di MySQL: HARGA_BELI_UPARTAI_SEDANG (typo di schema asli, dipertahankan)
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_upartai_sedang NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_beli_partai_besar   NUMERIC(10,2) DEFAULT 0;

-- Barcode
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS barcode_kecil  VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS barcode_sedang VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS barcode_besar  VARCHAR(20);

-- Satuan & harga jual umum
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_umum_kecil      VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_umum_sedang     VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_umum_besar      VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS isi_umum_kecil         INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS isi_umum_sedang        INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS isi_umum_besar         INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_jual_umum_kecil  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_jual_umum_sedang NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_jual_umum_besar  NUMERIC(10,2) DEFAULT 0;

-- Satuan & harga jual partai
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_partai_kecil      VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_partai_sedang     VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_partai_besar      VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS isi_partai_kecil         INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS isi_partai_sedang        INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS isi_partai_besar         INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_jual_partai_kecil  NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_jual_partai_sedang NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS harga_jual_partai_besar  NUMERIC(10,2) DEFAULT 0;

-- Satuan stok & batas stok
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_stok     VARCHAR(20);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS satuan_isi_stok INT DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS stok_min        NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS stok_max        NUMERIC(10,2) DEFAULT 0;

-- Lokasi rak (referensi fisik barang)
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS lokasi_rak_toko   VARCHAR(50);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS lokasi_rak_gudang VARCHAR(50);

-- Poin & komisi
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS point_member        NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS point_karyawan      NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS komisi_sales_rp     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS komisi_sales_persen NUMERIC(10,2) DEFAULT 0;

-- Status aktif/nonaktif
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS status VARCHAR(10) DEFAULT 'Aktif';

-- Kolom sync — khusus cloud, tidak ada di tbl_barang lokal
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS updated_at     TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS updated_by     VARCHAR(50);
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS version        INT DEFAULT 1;
ALTER TABLE barang_master ADD COLUMN IF NOT EXISTS kode_cabang_asal VARCHAR(20);

-- Isi nilai default untuk baris lama
UPDATE barang_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE barang_master SET version    = 1     WHERE version    IS NULL;

-- Index
CREATE INDEX IF NOT EXISTS idx_bm_updated   ON barang_master (updated_at);
CREATE INDEX IF NOT EXISTS idx_bm_id_barang ON barang_master (id_barang);
CREATE INDEX IF NOT EXISTS idx_bm_barcode_k ON barang_master (barcode_kecil);
CREATE INDEX IF NOT EXISTS idx_bm_barcode_s ON barang_master (barcode_sedang);
CREATE INDEX IF NOT EXISTS idx_bm_barcode_b ON barang_master (barcode_besar);
CREATE INDEX IF NOT EXISTS idx_bm_nama      ON barang_master (nama_barang);

DROP TRIGGER IF EXISTS trg_barang_master_updated ON barang_master;
CREATE TRIGGER trg_barang_master_updated
    BEFORE UPDATE ON barang_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 2. transfer_barang_cloud
--    Transfer barang antar toko melalui cloud.
-- ============================================================
CREATE TABLE IF NOT EXISTS transfer_barang_cloud (
    id             UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    dari_toko      VARCHAR(20),
    ke_toko        VARCHAR(20),
    kode_barang    VARCHAR(50),
    nama_barang    VARCHAR(200),
    qty            NUMERIC(10,2) DEFAULT 0,
    satuan         VARCHAR(20),
    isi_satuan     INT DEFAULT 1,
    qty_satuan     NUMERIC(10,2) DEFAULT 0,
    keterangan     VARCHAR(255),
    status         VARCHAR(20) DEFAULT 'pending',
    id_user_kirim  VARCHAR(50),
    id_user_terima VARCHAR(50),
    tgl_kirim      TIMESTAMPTZ DEFAULT NOW(),
    tgl_terima     TIMESTAMPTZ,
    updated_at     TIMESTAMPTZ DEFAULT NOW()
);

ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS dari_toko      VARCHAR(20);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS ke_toko        VARCHAR(20);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS kode_barang    VARCHAR(50);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS nama_barang    VARCHAR(200);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS qty            NUMERIC(10,2) DEFAULT 0;
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS satuan         VARCHAR(20);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS isi_satuan     INT DEFAULT 1;
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS qty_satuan     NUMERIC(10,2) DEFAULT 0;
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS keterangan     VARCHAR(255);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS status         VARCHAR(20) DEFAULT 'pending';
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS id_user_kirim  VARCHAR(50);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS id_user_terima VARCHAR(50);
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS tgl_kirim      TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS tgl_terima     TIMESTAMPTZ;
ALTER TABLE transfer_barang_cloud ADD COLUMN IF NOT EXISTS updated_at     TIMESTAMPTZ DEFAULT NOW();

CREATE INDEX IF NOT EXISTS idx_tbc_ke_toko   ON transfer_barang_cloud (ke_toko, status);
CREATE INDEX IF NOT EXISTS idx_tbc_dari_toko ON transfer_barang_cloud (dari_toko);
CREATE INDEX IF NOT EXISTS idx_tbc_updated   ON transfer_barang_cloud (updated_at);

DROP TRIGGER IF EXISTS trg_transfer_updated ON transfer_barang_cloud;
CREATE TRIGGER trg_transfer_updated
    BEFORE UPDATE ON transfer_barang_cloud
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 3. sync_conflict_log
--    Catat konflik versi saat upload.
-- ============================================================
CREATE TABLE IF NOT EXISTS sync_conflict_log (
    id            UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tabel         VARCHAR(50),
    id_lokal      VARCHAR(50),
    id_cloud      VARCHAR(50),
    kode_cabang   VARCHAR(20),
    version_lokal INT,
    version_cloud INT,
    payload_lokal TEXT,
    created_at    TIMESTAMPTZ DEFAULT NOW()
);

ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS tabel         VARCHAR(50);
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS id_lokal      VARCHAR(50);
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS id_cloud      VARCHAR(50);
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS kode_cabang   VARCHAR(20);
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS version_lokal INT;
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS version_cloud INT;
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS payload_lokal TEXT;
ALTER TABLE sync_conflict_log ADD COLUMN IF NOT EXISTS created_at    TIMESTAMPTZ DEFAULT NOW();

-- ============================================================
-- 4. kategori_master  (dari tbl_kategori)
-- ============================================================
CREATE TABLE IF NOT EXISTS kategori_master (
    id   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode VARCHAR(20) NOT NULL,
    UNIQUE (kode)
);
ALTER TABLE kategori_master ADD COLUMN IF NOT EXISTS nama       VARCHAR(100);
ALTER TABLE kategori_master ADD COLUMN IF NOT EXISTS jenis      VARCHAR(50);
ALTER TABLE kategori_master ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE kategori_master ADD COLUMN IF NOT EXISTS updated_by VARCHAR(50);
ALTER TABLE kategori_master ADD COLUMN IF NOT EXISTS version    INT DEFAULT 1;

UPDATE kategori_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE kategori_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_km_kode       ON kategori_master (kode);
CREATE INDEX IF NOT EXISTS idx_km_updated    ON kategori_master (updated_at);

DROP TRIGGER IF EXISTS trg_kategori_updated ON kategori_master;
CREATE TRIGGER trg_kategori_updated
    BEFORE UPDATE ON kategori_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 5. satuan_master  (dari tbl_satuan)
-- ============================================================
CREATE TABLE IF NOT EXISTS satuan_master (
    id   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode VARCHAR(10) NOT NULL,
    UNIQUE (kode)
);
ALTER TABLE satuan_master ADD COLUMN IF NOT EXISTS nama       VARCHAR(20);
ALTER TABLE satuan_master ADD COLUMN IF NOT EXISTS isi        INT DEFAULT 0;
ALTER TABLE satuan_master ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE satuan_master ADD COLUMN IF NOT EXISTS updated_by VARCHAR(50);
ALTER TABLE satuan_master ADD COLUMN IF NOT EXISTS version    INT DEFAULT 1;

UPDATE satuan_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE satuan_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_sm_kode    ON satuan_master (kode);
CREATE INDEX IF NOT EXISTS idx_sm_updated ON satuan_master (updated_at);

DROP TRIGGER IF EXISTS trg_satuan_updated ON satuan_master;
CREATE TRIGGER trg_satuan_updated
    BEFORE UPDATE ON satuan_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 6. merk_master  (dari tbl_merk)
-- ============================================================
CREATE TABLE IF NOT EXISTS merk_master (
    id   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode VARCHAR(10) NOT NULL,
    UNIQUE (kode)
);
ALTER TABLE merk_master ADD COLUMN IF NOT EXISTS nama        VARCHAR(20);
ALTER TABLE merk_master ADD COLUMN IF NOT EXISTS keterangan  VARCHAR(50);
ALTER TABLE merk_master ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE merk_master ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(50);
ALTER TABLE merk_master ADD COLUMN IF NOT EXISTS version     INT DEFAULT 1;

UPDATE merk_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE merk_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_mm_kode    ON merk_master (kode);
CREATE INDEX IF NOT EXISTS idx_mm_updated ON merk_master (updated_at);

DROP TRIGGER IF EXISTS trg_merk_updated ON merk_master;
CREATE TRIGGER trg_merk_updated
    BEFORE UPDATE ON merk_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 7. supliyer_master  (dari tbl_supliyer)
--    Kolom hutang (HUTANGAWAL, TOTALHUTANG, dll) tidak disync
--    karena hutang dikelola per toko masing-masing.
-- ============================================================
CREATE TABLE IF NOT EXISTS supliyer_master (
    id   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode VARCHAR(20) NOT NULL,
    UNIQUE (kode)
);
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS nama          VARCHAR(100);
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS alamat        VARCHAR(200);
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS hp            VARCHAR(15);
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS jangkahutang  INT DEFAULT 0;
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS status        VARCHAR(10) DEFAULT 'Aktif';
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS updated_at    TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS updated_by    VARCHAR(50);
ALTER TABLE supliyer_master ADD COLUMN IF NOT EXISTS version       INT DEFAULT 1;

UPDATE supliyer_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE supliyer_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_sup_kode    ON supliyer_master (kode);
CREATE INDEX IF NOT EXISTS idx_sup_nama    ON supliyer_master (nama);
CREATE INDEX IF NOT EXISTS idx_sup_updated ON supliyer_master (updated_at);

DROP TRIGGER IF EXISTS trg_supliyer_updated ON supliyer_master;
CREATE TRIGGER trg_supliyer_updated
    BEFORE UPDATE ON supliyer_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 8. pelanggan_master  (dari tbl_pelanggan)
--    Kolom hutang (HUTANGAWAL, TOTALHUTANG, dll) tidak disync
--    karena saldo piutang dikelola per toko masing-masing.
-- ============================================================
CREATE TABLE IF NOT EXISTS pelanggan_master (
    id   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode VARCHAR(20) NOT NULL,
    UNIQUE (kode)
);
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS nama          VARCHAR(50);
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS alamat        VARCHAR(100);
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS no_telp       VARCHAR(15);
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS jenis         VARCHAR(20);
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS jangkapiutang SMALLINT DEFAULT 0;
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS status        VARCHAR(10) DEFAULT 'Aktif';
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS updated_at    TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS updated_by    VARCHAR(50);
ALTER TABLE pelanggan_master ADD COLUMN IF NOT EXISTS version       INT DEFAULT 1;

UPDATE pelanggan_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE pelanggan_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_pel_kode    ON pelanggan_master (kode);
CREATE INDEX IF NOT EXISTS idx_pel_nama    ON pelanggan_master (nama);
CREATE INDEX IF NOT EXISTS idx_pel_updated ON pelanggan_master (updated_at);

DROP TRIGGER IF EXISTS trg_pelanggan_updated ON pelanggan_master;
CREATE TRIGGER trg_pelanggan_updated
    BEFORE UPDATE ON pelanggan_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 9. armada_master  (dari tbl_armada)
-- ============================================================
CREATE TABLE IF NOT EXISTS armada_master (
    id   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode VARCHAR(30) NOT NULL,
    UNIQUE (kode)
);
ALTER TABLE armada_master ADD COLUMN IF NOT EXISTS nopol      VARCHAR(40);
ALTER TABLE armada_master ADD COLUMN IF NOT EXISTS jenis      VARCHAR(50);
ALTER TABLE armada_master ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE armada_master ADD COLUMN IF NOT EXISTS updated_by VARCHAR(50);
ALTER TABLE armada_master ADD COLUMN IF NOT EXISTS version    INT DEFAULT 1;

UPDATE armada_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE armada_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_arm_kode    ON armada_master (kode);
CREATE INDEX IF NOT EXISTS idx_arm_updated ON armada_master (updated_at);

DROP TRIGGER IF EXISTS trg_armada_updated ON armada_master;
CREATE TRIGGER trg_armada_updated
    BEFORE UPDATE ON armada_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- 10. RLS — nonaktifkan untuk development
-- ============================================================
ALTER TABLE barang_master         DISABLE ROW LEVEL SECURITY;
ALTER TABLE transfer_barang_cloud DISABLE ROW LEVEL SECURITY;
ALTER TABLE sync_conflict_log     DISABLE ROW LEVEL SECURITY;
ALTER TABLE kategori_master       DISABLE ROW LEVEL SECURITY;
ALTER TABLE satuan_master         DISABLE ROW LEVEL SECURITY;
ALTER TABLE merk_master           DISABLE ROW LEVEL SECURITY;
ALTER TABLE supliyer_master       DISABLE ROW LEVEL SECURITY;
ALTER TABLE pelanggan_master      DISABLE ROW LEVEL SECURITY;
ALTER TABLE armada_master         DISABLE ROW LEVEL SECURITY;

-- ============================================================
-- Verifikasi
-- ============================================================
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_schema = 'public' AND table_name = 'barang_master'
ORDER BY ordinal_position;

-- ============================================================
-- LANGKAH SELANJUTNYA:
-- Jalankan Database/supabase_laporan.sql untuk membuat
-- tabel snapshot dan view laporan (stok, hutang, piutang,
-- karyawan, gaji, COA/neraca, laba rugi).
-- ============================================================

-- ============================================================
-- 11. cabang_master  (dari tbl_perusahaan)
--     Identitas cabang untuk header laporan dan referensi kode_cabang
--     di semua tabel snapshot. Diupload saat FormCompany simpan
--     dan saat SyncUploadSemua dijalankan.
--     Kolom konfigurasi operasional (kode rekening, tutup bulan)
--     TIDAK disync — hanya identitas yang dibutuhkan laporan.
-- ============================================================
CREATE TABLE IF NOT EXISTS cabang_master (
    id           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    kode_cabang  VARCHAR(20) NOT NULL,
    UNIQUE (kode_cabang)
);

ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS nama_cabang VARCHAR(100);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS alamat     VARCHAR(150);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS kota       VARCHAR(40);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS hp         VARCHAR(60);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS pemilik    VARCHAR(50);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS device_id  VARCHAR(100);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS claimed_at TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS updated_by VARCHAR(50);
ALTER TABLE cabang_master ADD COLUMN IF NOT EXISTS version    INT DEFAULT 1;

UPDATE cabang_master SET updated_at = NOW() WHERE updated_at IS NULL;
UPDATE cabang_master SET version    = 1     WHERE version    IS NULL;

CREATE INDEX IF NOT EXISTS idx_cm_kode_cabang ON cabang_master (kode_cabang);

DROP TRIGGER IF EXISTS trg_cabang_master_updated ON cabang_master;
CREATE TRIGGER trg_cabang_master_updated
    BEFORE UPDATE ON cabang_master
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

ALTER TABLE cabang_master DISABLE ROW LEVEL SECURITY;
