-- ============================================================
-- MIGRASI: Perbaikan Histori Hutang/Piutang
-- File   : 18_migrasi_hutang_piutang_detail.sql
-- Tanggal: 2026-04-24
-- ============================================================
-- PENTING: Backup database sebelum menjalankan skrip ini!
-- Skrip ini bersifat IDEMPOTEN — aman dijalankan lebih dari satu kali.
--
-- Nilai JENIS yang dipakai:
--   hutang_detail  : 'BELI' (hutang timbul), 'BAYAR', 'RETUR', 'HAPUS'
--   piutang_detail : 'JUAL' (piutang timbul), 'BAYAR', 'RETUR', 'HAPUS'
--
-- ID_BAYAR untuk baris BELI/JUAL = ID faktur itu sendiri (tanpa prefix)
--   Contoh: hutang_detail.ID_BAYAR = 'PB-2604240001' untuk baris JENIS='BELI'
--   Contoh: piutang_detail.ID_BAYAR = 'PJ-2604240001' untuk baris JENIS='JUAL'
-- ============================================================

-- ── BAGIAN 1: Persiapan kolom JENIS di hutang_detail ────────
-- Isi NULL/kosong dulu SEBELUM ALTER agar tidak error NOT NULL
-- Nilai lama 'TIMBUL' diubah ke 'BELI' (nama baru yang lebih deskriptif)

UPDATE hutang_detail
SET JENIS = 'BAYAR'
WHERE JENIS IS NULL OR JENIS = '';

-- Ubah nilai lama 'TIMBUL' ke 'BELI'
UPDATE hutang_detail
SET JENIS = 'BELI'
WHERE JENIS = 'TIMBUL';

-- Perbaiki ID_BAYAR lama yang pakai prefix 'TIMBUL-' — hapus prefix, sisakan ID faktur saja
UPDATE hutang_detail
SET ID_BAYAR = SUBSTRING(ID_BAYAR, 8)   -- hapus 'TIMBUL-' (7 karakter + 1)
WHERE ID_BAYAR LIKE 'TIMBUL-%' AND JENIS = 'BELI';

ALTER TABLE hutang_detail
    MODIFY COLUMN JENIS VARCHAR(10) NOT NULL DEFAULT 'BAYAR';

-- ── BAGIAN 2: Persiapan kolom JENIS di piutang_detail ───────
-- Nilai lama 'TIMBUL', 'Partai', 'Umum' (data salah di produksi) dibenahi

UPDATE piutang_detail
SET JENIS = 'BAYAR'
WHERE JENIS IS NULL OR JENIS = '' OR JENIS IN ('Partai', 'Umum');

-- Ubah nilai lama 'TIMBUL' ke 'JUAL'
UPDATE piutang_detail
SET JENIS = 'JUAL'
WHERE JENIS = 'TIMBUL';

-- Perbaiki ID_BAYAR lama yang pakai prefix 'TIMBUL-' — hapus prefix, sisakan ID faktur saja
UPDATE piutang_detail
SET ID_BAYAR = SUBSTRING(ID_BAYAR, 8)   -- hapus 'TIMBUL-' (7 karakter + 1)
WHERE ID_BAYAR LIKE 'TIMBUL-%' AND JENIS = 'JUAL';

ALTER TABLE piutang_detail
    MODIFY COLUMN JENIS VARCHAR(20) NOT NULL DEFAULT 'BAYAR';

-- ── BAGIAN 3: Index performa (idempoten via IF NOT EXISTS) ───

-- Index untuk query WHERE JENIS='BELI' AND ID_BELI=...
SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
               WHERE table_schema = DATABASE() 
               AND table_name = 'hutang_detail' 
               AND index_name = 'idx_hutang_detail_jenis_beli');
SET @sqlstmt := IF(@exist = 0, 
    'CREATE INDEX idx_hutang_detail_jenis_beli ON hutang_detail (JENIS, ID_BELI)', 
    'SELECT "Index idx_hutang_detail_jenis_beli already exists" AS Info');
PREPARE stmt FROM @sqlstmt;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Index untuk query WHERE JENIS='JUAL' AND ID_JUAL=...
SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
               WHERE table_schema = DATABASE() 
               AND table_name = 'piutang_detail' 
               AND index_name = 'idx_piutang_detail_jenis_jual');
SET @sqlstmt := IF(@exist = 0, 
    'CREATE INDEX idx_piutang_detail_jenis_jual ON piutang_detail (JENIS, ID_JUAL)', 
    'SELECT "Index idx_piutang_detail_jenis_jual already exists" AS Info');
PREPARE stmt FROM @sqlstmt;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ── BAGIAN 4: Migrasi baris BELI untuk hutang lama ──────────
-- Sumber : tabel pembelian
-- Filter : STATUS_TRANSAKSI_BELI = 'Belum Lunas' (nilai enum yang benar di DB)
-- Guard  : NOT EXISTS mencegah duplikat (idempoten) — cek JENIS='BELI'
-- Catatan: DIBAYAR dan RETUR diisi dari data aktual, bukan 0
-- ID_BAYAR = ID_PEMBELIAN itu sendiri (tanpa prefix)

INSERT INTO hutang_detail (
    ID_BAYAR, TANGGAL_BAYAR, LOKASI,
    ID_BELI, KODE, NAMA, JENIS,
    TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG,
    JATUH_TEMPO, PEMBAYARAN, STATUS,
    ID_USER, ID_KOMPUTER
)
SELECT
    p.ID_PEMBELIAN,                        -- ID_BAYAR = ID faktur (tanpa prefix)
    NOW(),                                  -- TANGGAL_BAYAR = waktu migrasi
    IFNULL(p.LOKASI, ''),                  -- LOKASI
    p.ID_PEMBELIAN,                        -- ID_BELI
    p.ID_SUPPLIER,                         -- KODE
    p.NAMA_SUPLIYER,                       -- NAMA
    'BELI',                                -- JENIS = 'BELI' (hutang timbul dari pembelian)
    p.TGL_BELI,                            -- TANGGAL_BELI
    p.GRAND_TOTAL_BELI,                    -- TOTAL_HUTANG = nilai awal faktur
    p.PEMBAYARAN,                          -- DIBAYAR = sudah dibayar sebelumnya (bukan 0)
    p.RETUR,                               -- RETUR = sudah diretur sebelumnya (bukan 0)
    p.TAGIHAN,                             -- HUTANG = sisa hutang terkini
    p.JATUH_TEMPO,                         -- JATUH_TEMPO
    0,                                     -- PEMBAYARAN = 0 (baris BELI bukan baris bayar)
    CASE WHEN p.TAGIHAN <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END,  -- STATUS
    'MIGRASI',                             -- ID_USER
    'MIGRASI'                              -- ID_KOMPUTER
FROM pembelian p
WHERE p.STATUS_TRANSAKSI_BELI = 'Belum Lunas'
  AND NOT EXISTS (
      SELECT 1 FROM hutang_detail hd
      WHERE hd.ID_BELI = p.ID_PEMBELIAN
        AND hd.JENIS = 'BELI'
  );

-- ── BAGIAN 5: Migrasi baris JUAL untuk piutang lama ─────────
-- Sumber : tabel penjualan
-- Filter : STATUS_TRANSAKSI IN ('Belum Lunas', 'TERHUTANG') — kedua nilai ada di DB produksi
-- Guard  : NOT EXISTS mencegah duplikat (idempoten) — cek JENIS='JUAL'
-- Catatan: DIBAYAR diisi dari NOMINALBAYARPIUTANG, bukan 0
--          Kolom total: GRAND_TOTAL_STL_PAJAK (bukan GRAND_TOTAL)
--          Tidak ada kolom RETUR di penjualan — diisi 0
-- ID_BAYAR = ID_PENJUALAN itu sendiri (tanpa prefix)

INSERT INTO piutang_detail (
    ID_BAYAR, TANGGAL_BAYAR, LOKASI,
    ID_JUAL, KODE, NAMA, JENIS,
    TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG,
    JATUH_TEMPO, PEMBAYARAN, STATUS,
    ID_USER, ID_KOMPUTER
)
SELECT
    p.ID_PENJUALAN,                        -- ID_BAYAR = ID faktur (tanpa prefix)
    NOW(),                                  -- TANGGAL_BAYAR = waktu migrasi
    IFNULL(p.LOKASIBARANG, ''),            -- LOKASI
    p.ID_PENJUALAN,                        -- ID_JUAL
    p.ID_PELANGGAN,                        -- KODE
    p.NAMA_PELANGGAN,                      -- NAMA
    'JUAL',                                -- JENIS = 'JUAL' (piutang timbul dari penjualan)
    p.TGL_TRANSAKSI,                       -- TANGGAL_JUAL
    p.GRAND_TOTAL_STL_PAJAK,              -- PIUTANG = nilai awal faktur
    p.NOMINALBAYARPIUTANG,                 -- DIBAYAR = sudah dibayar sebelumnya (bukan 0)
    0,                                     -- RETUR = tidak ada kolom retur di penjualan
    p.SISA_TAGIHAN,                        -- HUTANG = sisa piutang terkini
    p.JATUH_TEMPO,                         -- JATUH_TEMPO
    0,                                     -- PEMBAYARAN = 0 (baris JUAL bukan baris bayar)
    CASE WHEN p.SISA_TAGIHAN <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END,  -- STATUS
    'MIGRASI',                             -- ID_USER
    'MIGRASI'                              -- ID_KOMPUTER
FROM penjualan p
WHERE p.STATUS_TRANSAKSI IN ('Belum Lunas', 'TERHUTANG')
  AND NOT EXISTS (
      SELECT 1 FROM piutang_detail pd
      WHERE pd.ID_JUAL = p.ID_PENJUALAN
        AND pd.JENIS = 'JUAL'
  );

-- ── VERIFIKASI (jalankan manual setelah migrasi) ─────────────
-- SELECT COUNT(*) FROM hutang_detail WHERE JENIS = 'BELI';
-- SELECT COUNT(*) FROM pembelian WHERE STATUS_TRANSAKSI_BELI = 'Belum Lunas';
-- -- Kedua angka harus sama
--
-- SELECT COUNT(*) FROM piutang_detail WHERE JENIS = 'JUAL';
-- SELECT COUNT(*) FROM penjualan WHERE STATUS_TRANSAKSI IN ('Belum Lunas', 'TERHUTANG');
-- -- Kedua angka harus sama
--
-- -- Cek tidak ada duplikat:
-- SELECT ID_BELI, COUNT(*) FROM hutang_detail WHERE JENIS='BELI' GROUP BY ID_BELI HAVING COUNT(*) > 1;
-- SELECT ID_JUAL, COUNT(*) FROM piutang_detail WHERE JENIS='JUAL' GROUP BY ID_JUAL HAVING COUNT(*) > 1;
--
-- -- Cek tidak ada prefix lama:
-- SELECT ID_BAYAR FROM hutang_detail WHERE ID_BAYAR LIKE 'TIMBUL-%';
-- SELECT ID_BAYAR FROM piutang_detail WHERE ID_BAYAR LIKE 'TIMBUL-%';
