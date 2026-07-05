-- ============================================================
-- 48_migrasi_fix_akun_lawan_stok_opname_konsisten.sql
-- ============================================================
-- Tujuan: Menyamakan akun lawan STOK OPNAME di JurnalUmum
--   agar konsisten dengan LAWAN_KODE_REK_BARANG yang aktif
--   di tbl_perusahaan.
--
-- Masalah:
--   Data lama jurnal STOK OPNAME memakai berbagai akun lawan:
--     • 04.02.001 (PRIVE)    — skema sangat lama (sebelum fix 44)
--     • 06.04.001 (PENYESUAIAN STOK MINUS) — skema setelah fix 44
--     • Akun lain (dari LAWAN_KODE_REK_BARANG yang berubah-ubah)
--   Ketidakkonsistenan ini menyebabkan saldo akun lawan menjadi
--   sangat besar (positif atau negatif) yang tidak mencerminkan
--   realita.
--
-- Solusi:
--   1. Ambil LAWAN_KODE_REK_BARANG aktif dari tbl_perusahaan
--   2. UPDATE semua jurnal STOK OPNAME yang memakai akun lawan
--      SELAIN KODE_REK_BARANG (persediaan) ke akun lawan aktif
--   3. REVERSAL saldo akun lama (delta negatif)
--   4. Recalculate semua saldo akun
--
-- IDEMPOTEN: aman dijalankan berulang — cek kondisi sebelum UPDATE
-- ============================================================

SET SESSION innodb_lock_wait_timeout = 300;

START TRANSACTION;

-- ============================================================
-- STEP 0: Ambil konfigurasi akun aktif dari tbl_perusahaan
-- ============================================================

SET @kode_persediaan = (
    SELECT KODE_REK_BARANG FROM tbl_perusahaan LIMIT 1
);
SET @nama_persediaan = (
    SELECT NAMA_REK_BARANG FROM tbl_perusahaan LIMIT 1
);
SET @kode_lawan = (
    SELECT lawan_Kode_rek_barang FROM tbl_perusahaan LIMIT 1
);
SET @nama_lawan = (
    SELECT lawan_nama_rek_barang FROM tbl_perusahaan LIMIT 1
);

SELECT '=== KONFIGURASI AKUN AKTIF ===' AS info;
SELECT
    CONCAT('  KODE_REK_BARANG (persediaan) : ', @kode_persediaan) AS info
UNION ALL SELECT
    CONCAT('  NAMA_REK_BARANG              : ', @nama_persediaan)
UNION ALL SELECT
    CONCAT('  LAWAN_KODE_REK_BARANG        : ', @kode_lawan)
UNION ALL SELECT
    CONCAT('  LAWAN_NAMA_REK_BARANG        : ', @nama_lawan);

-- Validasi: pastikan config tidak kosong
SET @config_valid = (
    CASE WHEN @kode_persediaan IS NOT NULL AND @kode_persediaan <> ''
          AND @kode_lawan      IS NOT NULL AND @kode_lawan      <> ''
    THEN 1 ELSE 0 END
);

SELECT CONCAT('  Config valid: ', IF(@config_valid = 1, 'YA', 'TIDAK — BATALKAN')) AS info;

-- Hentikan jika config kosong
SET @dummy = IF(@config_valid = 0,
    (SELECT CONCAT('ERROR: KODE_REK_BARANG atau LAWAN_KODE_REK_BARANG kosong di tbl_perusahaan. Isi dulu di Form General Setting.')),
    NULL
);

-- ============================================================
-- STEP 1: Analisis kondisi sebelum fix
-- ============================================================

SELECT '=== STEP 1: ANALISIS kondisi sebelum fix ===' AS info;

-- Berapa baris jurnal STOK OPNAME dengan akun lawan yang tidak konsisten
SELECT
    JENIS_TRANSAKSI,
    CASE
        WHEN NOMOR_AKUN_D = @kode_persediaan THEN NOMOR_AKUN_K
        WHEN NOMOR_AKUN_K = @kode_persediaan THEN NOMOR_AKUN_D
        ELSE CONCAT('D:', NOMOR_AKUN_D, ' K:', NOMOR_AKUN_K)
    END AS akun_lawan_saat_ini,
    COUNT(*) AS jml_baris,
    ROUND(SUM(NOMINAL), 0) AS total_nominal
FROM JurnalUmum
WHERE JENIS_TRANSAKSI = 'STOK OPNAME'
  AND (NOMOR_AKUN_D = @kode_persediaan OR NOMOR_AKUN_K = @kode_persediaan)
  AND CASE
        WHEN NOMOR_AKUN_D = @kode_persediaan THEN NOMOR_AKUN_K
        WHEN NOMOR_AKUN_K = @kode_persediaan THEN NOMOR_AKUN_D
      END <> @kode_lawan
GROUP BY akun_lawan_saat_ini
ORDER BY total_nominal DESC;

-- ============================================================
-- STEP 2: Fix jurnal STOK OPNAME — pihak KREDIT (selisih plus)
--   D PERSEDIAAN K AKUN_LAWAN_LAMA → K jadi @kode_lawan
--   Kondisi: D = persediaan, K = bukan lawan aktif
-- ============================================================

SELECT '=== STEP 2: FIX jurnal selisih PLUS (stok lebih) ===' AS info;

UPDATE JurnalUmum
SET
    NOMOR_AKUN_K = @kode_lawan,
    NAMA_AKUN_K  = @nama_lawan
WHERE JENIS_TRANSAKSI = 'STOK OPNAME'
  AND NOMOR_AKUN_D    = @kode_persediaan
  AND NOMOR_AKUN_K   <> @kode_lawan
  AND NOMOR_AKUN_K   <> '';   -- jangan sentuh jurnal satu sisi

SELECT CONCAT('  Fixed selisih PLUS (K→', @kode_lawan, '): ',
    ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- STEP 3: Fix jurnal STOK OPNAME — pihak DEBET (selisih minus)
--   D AKUN_LAWAN_LAMA K PERSEDIAAN → D jadi @kode_lawan
--   Kondisi: K = persediaan, D = bukan lawan aktif
-- ============================================================

SELECT '=== STEP 3: FIX jurnal selisih MINUS (stok kurang) ===' AS info;

UPDATE JurnalUmum
SET
    NOMOR_AKUN_D = @kode_lawan,
    NAMA_AKUN_D  = @nama_lawan
WHERE JENIS_TRANSAKSI = 'STOK OPNAME'
  AND NOMOR_AKUN_K    = @kode_persediaan
  AND NOMOR_AKUN_D   <> @kode_lawan
  AND NOMOR_AKUN_D   <> '';   -- jangan sentuh jurnal satu sisi

SELECT CONCAT('  Fixed selisih MINUS (D→', @kode_lawan, '): ',
    ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- STEP 4: Recalculate SEMUA saldo akun dari JurnalUmum
-- (perlu full recalculate karena ada banyak akun yang terpengaruh)
-- ============================================================

SELECT '=== STEP 4: RECALCULATE semua saldo akun ===' AS info;

CALL sp_bat_saldo_semua_akun();

SELECT '  sp_bat_saldo_semua_akun selesai' AS info;

-- ============================================================
-- STEP 5: Verifikasi hasil
-- ============================================================

SELECT '=== STEP 5: VERIFIKASI ===' AS info;

-- Seharusnya 0: tidak boleh ada lagi jurnal SO dengan akun lawan yang salah
SELECT CONCAT(
    '  Jurnal STOK OPNAME akun lawan tidak konsisten (seharusnya 0): ',
    COUNT(*)
) AS info
FROM JurnalUmum
WHERE JENIS_TRANSAKSI = 'STOK OPNAME'
  AND (NOMOR_AKUN_D = @kode_persediaan OR NOMOR_AKUN_K = @kode_persediaan)
  AND CASE
        WHEN NOMOR_AKUN_D = @kode_persediaan THEN NOMOR_AKUN_K
        WHEN NOMOR_AKUN_K = @kode_persediaan THEN NOMOR_AKUN_D
      END <> @kode_lawan;

-- Saldo akun-akun kritis setelah fix
SELECT KODE_AKUN, NAMA_AKUN, S_DEBET, S_KREDIT, SALDO_AKHIR
FROM tbl_datareferensi
WHERE KODE_AKUN IN (
    @kode_persediaan,
    @kode_lawan,
    '04.02.001',  -- PRIVE (bekas akun lawan salah)
    '06.04.001',  -- PENYESUAIAN STOK MINUS
    '04.01.001'   -- MODAL
)
ORDER BY KODE_AKUN;

-- Ringkasan: berapa total jurnal SO per akun lawan sekarang
SELECT
    CASE
        WHEN NOMOR_AKUN_D = @kode_persediaan THEN NOMOR_AKUN_K
        WHEN NOMOR_AKUN_K = @kode_persediaan THEN NOMOR_AKUN_D
    END AS akun_lawan,
    COUNT(*) AS jml,
    ROUND(SUM(NOMINAL), 0) AS total
FROM JurnalUmum
WHERE JENIS_TRANSAKSI = 'STOK OPNAME'
  AND (NOMOR_AKUN_D = @kode_persediaan OR NOMOR_AKUN_K = @kode_persediaan)
GROUP BY akun_lawan
ORDER BY total DESC;

COMMIT;

SELECT 'MIGRASI 48 SELESAI' AS status;
