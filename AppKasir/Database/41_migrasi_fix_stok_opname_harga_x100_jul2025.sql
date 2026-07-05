-- ============================================================
-- 42_migrasi_fix_stok_opname_harga_x100_jul2025.sql
-- ============================================================
-- Tujuan: Koreksi 4 transaksi stok opname Juli 2025 di mana
--   kolom HARGA tersimpan ×100 dari HARGA_BELI master barang,
--   menyebabkan TOTAL_HARGA dan jurnal 01.04.001 corrupt.
--
-- Penyebab bug:
--   Bug konversi decimal di form stok opname, menyerang kolom
--   HARGA (bukan HARGA_BELI_SATUAN seperti di penjualan).
--   Faktor inflasi: ×100 (bukan ×10000 seperti bug CDec biasa).
--
-- Transaksi terdampak (4):
--   SO-2507090002  Deterjen Boom Dos       selisih -2475  Rp 23,2 M → Rp 232 juta
--   SO-2507090003  Sunlight 640+585 Banded selisih -4950  Rp 47,5 M → Rp 475 juta
--   SO-2507090012  Nusa Jaya Jumbo Pack    selisih  -990  Rp  6,3 M → Rp  63 juta
--   SO-2507090013  Nusa Jaya Kecil Pack    selisih -9801  Rp 38,7 M → Rp 387 juta
--
-- Total inflasi di jurnal 01.04.001: Rp 115,8 M → seharusnya Rp 1,158 M
-- Selisih: Rp 114,6 M yang harus dikoreksi
--
-- Tabel terdampak:
--   stok_opname   — HARGA, TOTAL_HARGA
--   JurnalUmum    — NOMINAL (DEBET 01.04.001, KREDIT 04.02.001)
--
-- Deteksi:
--   ROUND(so.HARGA / tb.HARGA_BELI, 0) IN (100, 1000, 10000)
--   AND ABS(so.HARGA / tb.HARGA_BELI - faktor) < 0.01
--
-- Rumus fix:
--   HARGA_benar       = tb.HARGA_BELI
--   TOTAL_HARGA_benar = STOK_SELISIH * HARGA_benar
--   Jurnal NOMINAL    = ABS(TOTAL_HARGA_benar)
--
-- IDEMPOTEN: aman dijalankan berulang
-- ============================================================

SET SESSION innodb_lock_wait_timeout = 300;

START TRANSACTION;

-- ============================================================
-- BAGIAN 1: Deteksi
-- ============================================================

SELECT '=== BAGIAN 1: DETEKSI stok_opname corrupt ===' AS info;

CREATE TEMPORARY TABLE tmp_fix_so AS
SELECT
    so.ID_STOK_OPNAME,
    so.ID_BARANG,
    so.STOK_SELISIH,
    so.HARGA              AS harga_corrupt,
    so.TOTAL_HARGA        AS total_corrupt,
    tb.HARGA_BELI         AS harga_benar,
    ROUND(so.STOK_SELISIH * tb.HARGA_BELI, 0) AS total_benar,
    ROUND(so.HARGA / tb.HARGA_BELI, 0) AS faktor
FROM stok_opname so
JOIN tbl_barang tb ON tb.ID_BARANG = so.ID_BARANG
WHERE tb.HARGA_BELI > 0
  AND ROUND(so.HARGA / tb.HARGA_BELI, 0) IN (100, 1000, 10000)
  AND ABS(so.HARGA / tb.HARGA_BELI
      - ROUND(so.HARGA / tb.HARGA_BELI, 0)) < 0.01;

SELECT CONCAT('  Terdeteksi ', COUNT(*), ' transaksi stok_opname corrupt') AS info
FROM tmp_fix_so;

SELECT ID_STOK_OPNAME, ID_BARANG, STOK_SELISIH,
       harga_corrupt, harga_benar, faktor,
       total_corrupt, total_benar
FROM tmp_fix_so;

-- ============================================================
-- BAGIAN 2: Fix stok_opname
-- ============================================================

SELECT '=== BAGIAN 2: FIX stok_opname ===' AS info;

UPDATE stok_opname so
JOIN tmp_fix_so f ON f.ID_STOK_OPNAME = so.ID_STOK_OPNAME
SET
    so.HARGA       = f.harga_benar,
    so.TOTAL_HARGA = f.total_benar;

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris stok_opname') AS info;

-- ============================================================
-- BAGIAN 3: Fix JurnalUmum
-- Jurnal stok opname: DEBET 01.04.001 KREDIT 04.02.001
-- NOMINAL = ABS(TOTAL_HARGA) = ABS(SELISIH * HARGA)
-- ============================================================

SELECT '=== BAGIAN 3: FIX JurnalUmum STOK OPNAME ===' AS info;

-- 3a: Update NOMINAL — satu baris jurnal berisi DEBET dan KREDIT sekaligus
--     (format single-entry: NOMOR_AKUN_D dan NOMOR_AKUN_K dalam satu baris)
UPDATE JurnalUmum ju
JOIN tmp_fix_so f ON f.ID_STOK_OPNAME = ju.NO_TRANSAKSI
SET ju.NOMINAL = ABS(f.total_benar)
WHERE ju.JENIS_TRANSAKSI = 'STOK OPNAME'
  AND (ju.NOMOR_AKUN_D = '01.04.001' OR ju.NOMOR_AKUN_K = '04.02.001')
  AND ABS(ju.NOMINAL - ABS(f.total_benar)) > 0;

SELECT CONCAT('  Fixed jurnal STOK OPNAME: ', ROW_COUNT(), ' baris') AS info;

SELECT CONCAT('  Fixed KREDIT 04.02.001: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN 4: Recalculate tbl_datareferensi
-- ============================================================

SELECT '=== BAGIAN 4: RECALCULATE tbl_datareferensi ===' AS info;

CALL sp_bat_saldo_semua_akun();

SELECT '  sp_bat_saldo_semua_akun selesai' AS info;

-- ============================================================
-- BAGIAN 5: Verifikasi
-- ============================================================

SELECT '=== BAGIAN 5: VERIFIKASI ===' AS info;

-- Tidak boleh ada lagi stok_opname dengan HARGA ×100/1000/10000
SELECT CONCAT('  Sisa stok_opname corrupt (seharusnya 0): ', COUNT(*)) AS info
FROM stok_opname so
JOIN tbl_barang tb ON tb.ID_BARANG = so.ID_BARANG
WHERE tb.HARGA_BELI > 0
  AND ROUND(so.HARGA / tb.HARGA_BELI, 0) IN (100, 1000, 10000)
  AND ABS(so.HARGA / tb.HARGA_BELI
      - ROUND(so.HARGA / tb.HARGA_BELI, 0)) < 0.01;

-- Jurnal sudah match
SELECT CONCAT('  Jurnal tidak match (seharusnya 0): ', COUNT(*)) AS info
FROM JurnalUmum ju
JOIN tmp_fix_so f ON f.ID_STOK_OPNAME = ju.NO_TRANSAKSI
WHERE ju.JENIS_TRANSAKSI = 'STOK OPNAME'
  AND (ju.NOMOR_AKUN_D = '01.04.001' OR ju.NOMOR_AKUN_K = '04.02.001')
  AND ABS(ju.NOMINAL - ABS(f.total_benar)) > 1;

-- Saldo PERSEDIAAN BARANG dan PRIVE PEMILIK setelah fix
SELECT KODE_AKUN, NAMA_AKUN, S_DEBET, S_KREDIT, SALDO_AKHIR
FROM tbl_datareferensi
WHERE KODE_AKUN IN ('01.04.001', '04.02.001', '06.01.001');

-- ============================================================
-- CLEANUP & COMMIT
-- ============================================================

DROP TEMPORARY TABLE tmp_fix_so;

COMMIT;

SELECT 'MIGRASI 42 SELESAI' AS status;
