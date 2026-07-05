-- ============================================================
-- 38_migrasi_fix_penjualan_retur_harga_salah.sql
-- ============================================================
-- Tujuan: Koreksi data penjualan_detail & retur_penjualan_detail
--   yang HARGA_BELI / HARGA_BELI_SATUAN-nya ×10000 atau overflow
--   akibat bug konversi decimal (CDec di culture id-ID).
--
-- Penyebab bug:
--   Decimal dari MySQL (misal 3325.0000) dibaca sebagai string lalu
--   di-CDec() di culture id-ID → titik '.' dibaca pemisah ribuan
--   → 33250000 (salah ×10000). Kolom DECIMAL overflow → 99999999.99.
--   Juga menimpa LABA, TOTAL_HPP penjualan, dan propagate ke JurnalUmum
--   → PERSEDIAAN BARANG di tbl_datareferensi ikut korup.
--
-- STRATEGI KOREKSI:
--   Bagian A: Koreksi penjualan_detail — HARGA_BELI, HARGA_BELI_SATUAN, LABA
--   Bagian B: Koreksi retur_penjualan_detail — HARGA_BELI, HARGA_BELI_SATUAN, LABA
--   Bagian C: Koreksi penjualan.TOTAL_HPP — recalculate dari detail yg sudah fix
--   Bagian D: Koreksi JurnalUmum — HPP (06.01.001) & PERSEDIAAN BARANG (01.04.001)
--   Bagian E: Jalankan sp_bat_saldo_semua_akun — perbaiki tbl_datareferensi
--   Bagian F: Verifikasi hasil
--
-- DETEKSI:
--   pd = penjualan_detail / retur_penjualan_detail
--   tb = tbl_barang
--
--   1. OVERFLOW: pd.HARGA_BELI >= 99999999
--      → corrected_HB = tb.HARGA_BELI (tidak bisa /10000, presisi hilang)
--   2. X10000: pd.HARGA_BELI >= 1000000 AND pd.HARGA_BELI > 100 * tb.HARGA_BELI
--      → corrected_HB = pd.HARGA_BELI / 10000 (pulihkan harga historis)
--   3. HB_NORMAL: pd.HARGA_BELI < 1000000 (atau tidak masuk kriteria di atas)
--      → corrected_HB = pd.HARGA_BELI (sudah benar)
--   4. corrected_HBS = corrected_HB * pd.ISI_SATUAN
--   5. corrected_LABA = pd.TOTAL_HARGA - corrected_HBS
--
-- Cara pakai:
--   mysql -u root -p db_anda < 38_migrasi_fix_penjualan_retur_harga_salah.sql
-- ============================================================

START TRANSACTION;

SELECT '=== BAGIAN A: KOREKSI penjualan_detail ===' AS info;

-- =============================================================================
-- BAGIAN A: KOREKSI penjualan_detail
-- =============================================================================

-- Langkah A1: Simpan ID baris corrupt ke temp table
CREATE TEMPORARY TABLE tmp_fix_pd AS
SELECT
    pd.FAKTUR_JUAL,
    pd.ID_BARANG,
    pd.ISI_SATUAN,
    pd.QTY_SATUAN,
    pd.TOTAL_HARGA,
    pd.HARGA_BELI AS hb_salah,
    pd.HARGA_BELI_SATUAN AS hbs_salah,
    pd.LABA AS laba_salah,
    tb.HARGA_BELI AS hb_barang,
    CASE
        -- OVERFLOW: pakai harga barang saat ini
        WHEN pd.HARGA_BELI >= 99999999 THEN tb.HARGA_BELI
        -- X10000: bagi 10000 untuk pulihkan harga historis
        WHEN tb.HARGA_BELI > 0
         AND pd.HARGA_BELI > 100 * tb.HARGA_BELI
         AND ABS(pd.HARGA_BELI / tb.HARGA_BELI - 10000) < 100
            THEN ROUND(pd.HARGA_BELI / 10000, 4)
        -- Sudah benar
        ELSE pd.HARGA_BELI
    END AS hb_benar
FROM penjualan_detail pd
LEFT JOIN tbl_barang tb ON tb.ID_BARANG = pd.ID_BARANG
WHERE pd.HARGA_BELI_SATUAN >= 99999999
   OR (tb.HARGA_BELI > 0
       AND pd.HARGA_BELI > 100 * tb.HARGA_BELI
       AND ABS(pd.HARGA_BELI / tb.HARGA_BELI - 10000) < 100);

SELECT CONCAT('  Terdeteksi ', COUNT(*), ' baris corrupt di penjualan_detail') AS info FROM tmp_fix_pd;

-- Langkah A2: Update penjualan_detail
UPDATE penjualan_detail pd
JOIN tmp_fix_pd f
    ON f.FAKTUR_JUAL  = pd.FAKTUR_JUAL
   AND f.ID_BARANG    = pd.ID_BARANG
SET
    pd.HARGA_BELI        = f.hb_benar,
    pd.HARGA_BELI_SATUAN = ROUND(f.hb_benar * f.ISI_SATUAN, 4),
    pd.LABA              = ROUND(f.TOTAL_HARGA - (f.hb_benar * f.ISI_SATUAN * f.QTY_SATUAN), 2);

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris penjualan_detail') AS info;

-- =============================================================================
-- BAGIAN B: KOREKSI retur_penjualan_detail
-- =============================================================================

SELECT '=== BAGIAN B: KOREKSI retur_penjualan_detail ===' AS info;

CREATE TEMPORARY TABLE tmp_fix_rd AS
SELECT
    rd.ID_RETUR_PENJUALAN,
    rd.ID_BARANG,
    rd.QTY_SATUAN,
    rd.ISI_SATUAN,
    rd.TOTAL_HARGA,
    rd.HARGA_BELI AS hb_salah,
    rd.HARGA_BELI_SATUAN AS hbs_salah,
    rd.LABA AS laba_salah,
    tb.HARGA_BELI AS hb_barang,
    CASE
        WHEN rd.HARGA_BELI >= 99999999 THEN tb.HARGA_BELI
        WHEN tb.HARGA_BELI > 0
         AND rd.HARGA_BELI > 100 * tb.HARGA_BELI
         AND ABS(rd.HARGA_BELI / tb.HARGA_BELI - 10000) < 100
            THEN ROUND(rd.HARGA_BELI / 10000, 4)
        ELSE rd.HARGA_BELI
    END AS hb_benar
FROM retur_penjualan_detail rd
LEFT JOIN tbl_barang tb ON tb.ID_BARANG = rd.ID_BARANG
WHERE rd.HARGA_BELI_SATUAN >= 99999999
   OR (tb.HARGA_BELI > 0
       AND rd.HARGA_BELI > 100 * tb.HARGA_BELI
       AND ABS(rd.HARGA_BELI / tb.HARGA_BELI - 10000) < 100);

SELECT CONCAT('  Terdeteksi ', COUNT(*), ' baris corrupt di retur_penjualan_detail') AS info FROM tmp_fix_rd;

UPDATE retur_penjualan_detail rd
JOIN tmp_fix_rd f
    ON f.ID_RETUR_PENJUALAN = rd.ID_RETUR_PENJUALAN
   AND f.ID_BARANG          = rd.ID_BARANG
SET
    rd.HARGA_BELI        = f.hb_benar,
    rd.HARGA_BELI_SATUAN = ROUND(f.hb_benar * f.ISI_SATUAN, 4),
    rd.LABA              = ROUND(f.TOTAL_HARGA - (f.hb_benar * f.ISI_SATUAN * f.QTY_SATUAN), 2);

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris retur_penjualan_detail') AS info;

-- =============================================================================
-- BAGIAN C: KOREKSI penjualan.TOTAL_HPP
-- =============================================================================

SELECT '=== BAGIAN C: KOREKSI penjualan.TOTAL_HPP ===' AS info;

UPDATE penjualan p
JOIN (
    SELECT pd.FAKTUR_JUAL,
           ROUND(SUM(pd.HARGA_BELI_SATUAN * pd.QTY_SATUAN), 2) AS hpp_benar
    FROM penjualan_detail pd
    WHERE pd.FAKTUR_JUAL IN (SELECT DISTINCT FAKTUR_JUAL FROM tmp_fix_pd)
    GROUP BY pd.FAKTUR_JUAL
) c ON c.FAKTUR_JUAL = p.ID_PENJUALAN
SET p.TOTAL_HPP = c.hpp_benar;

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris penjualan.TOTAL_HPP') AS info;

-- =============================================================================
-- BAGIAN D: KOREKSI JurnalUmum
-- =============================================================================

SELECT '=== BAGIAN D: KOREKSI JurnalUmum ===' AS info;

-- D1: PENJUALAN — perbaiki HPP POKOK PENJUALAN (06.01.001) = DEBET
UPDATE JurnalUmum ju
JOIN (
    SELECT pd.FAKTUR_JUAL,
           ROUND(SUM(pd.HARGA_BELI_SATUAN * pd.QTY_SATUAN), 0) AS hpp_benar
    FROM penjualan_detail pd
    WHERE pd.FAKTUR_JUAL IN (SELECT DISTINCT FAKTUR_JUAL FROM tmp_fix_pd)
    GROUP BY pd.FAKTUR_JUAL
) c ON c.FAKTUR_JUAL = ju.NO_TRANSAKSI
SET ju.NOMINAL = c.hpp_benar
WHERE ju.JENIS_TRANSAKSI = 'PENJUALAN'
  AND ju.NOMOR_AKUN_D = '06.01.001';

SELECT CONCAT('  Jurnal HPP PENJUALAN (DEBET 06.01.001): ', ROW_COUNT(), ' baris') AS info;

-- D2: PENJUALAN — perbaiki PERSEDIAAN BARANG (01.04.001) = KREDIT
UPDATE JurnalUmum ju
JOIN (
    SELECT pd.FAKTUR_JUAL,
           ROUND(SUM(pd.HARGA_BELI_SATUAN * pd.QTY_SATUAN), 0) AS hpp_benar
    FROM penjualan_detail pd
    WHERE pd.FAKTUR_JUAL IN (SELECT DISTINCT FAKTUR_JUAL FROM tmp_fix_pd)
    GROUP BY pd.FAKTUR_JUAL
) c ON c.FAKTUR_JUAL = ju.NO_TRANSAKSI
SET ju.NOMINAL = c.hpp_benar
WHERE ju.JENIS_TRANSAKSI = 'PENJUALAN'
  AND ju.NOMOR_AKUN_K = '01.04.001';

SELECT CONCAT('  Jurnal PERSEDIAAN BARANG PENJUALAN (KREDIT 01.04.001): ', ROW_COUNT(), ' baris') AS info;

-- D3: RETUR PENJUALAN — perbaiki PERSEDIAAN BARANG (01.04.001) = DEBET
UPDATE JurnalUmum ju
JOIN (
    SELECT rd.ID_RETUR_PENJUALAN,
           ROUND(SUM(rd.HARGA_BELI_SATUAN * rd.QTY_SATUAN), 0) AS hpp_benar
    FROM retur_penjualan_detail rd
    WHERE rd.ID_RETUR_PENJUALAN IN (SELECT DISTINCT ID_RETUR_PENJUALAN FROM tmp_fix_rd)
    GROUP BY rd.ID_RETUR_PENJUALAN
) c ON c.ID_RETUR_PENJUALAN = ju.NO_TRANSAKSI
SET ju.NOMINAL = c.hpp_benar
WHERE ju.JENIS_TRANSAKSI = 'RETUR PENJUALAN'
  AND ju.NOMOR_AKUN_D = '01.04.001';

SELECT CONCAT('  Jurnal PERSEDIAAN BARANG RETUR (DEBET 01.04.001): ', ROW_COUNT(), ' baris') AS info;

-- D4: RETUR PENJUALAN — perbaiki HPP POKOK PENJUALAN (06.01.001) = KREDIT
UPDATE JurnalUmum ju
JOIN (
    SELECT rd.ID_RETUR_PENJUALAN,
           ROUND(SUM(rd.HARGA_BELI_SATUAN * rd.QTY_SATUAN), 0) AS hpp_benar
    FROM retur_penjualan_detail rd
    WHERE rd.ID_RETUR_PENJUALAN IN (SELECT DISTINCT ID_RETUR_PENJUALAN FROM tmp_fix_rd)
    GROUP BY rd.ID_RETUR_PENJUALAN
) c ON c.ID_RETUR_PENJUALAN = ju.NO_TRANSAKSI
SET ju.NOMINAL = c.hpp_benar
WHERE ju.JENIS_TRANSAKSI = 'RETUR PENJUALAN'
  AND ju.NOMOR_AKUN_K = '06.01.001';

SELECT CONCAT('  Jurnal HPP RETUR (KREDIT 06.01.001): ', ROW_COUNT(), ' baris') AS info;

-- =============================================================================
-- BAGIAN E: PERBAIKI tbl_datareferensi via stored procedure
-- =============================================================================

SELECT '=== BAGIAN E: JALANKAN sp_bat_saldo_semua_akun ===' AS info;

-- Pastikan stored procedure ada
-- Jika tidak ada, buat manual: UPDATE tbl_datareferensi di-skip dulu
CALL sp_bat_saldo_semua_akun();

SELECT CONCAT('  sp_bat_saldo_semua_akun selesai') AS info;

-- =============================================================================
-- BAGIAN F: VERIFIKASI
-- =============================================================================

SELECT '=== BAGIAN F: VERIFIKASI ===' AS info;

-- Verifikasi penjualan_detail
SELECT CONCAT('  penjualan_detail remaining >= 99999999: ',
    CAST(COUNT(*) AS CHAR)) AS info
FROM penjualan_detail
WHERE HARGA_BELI_SATUAN >= 99999999;

-- Verifikasi retur_penjualan_detail
SELECT CONCAT('  retur_penjualan_detail remaining >= 99999999: ',
    CAST(COUNT(*) AS CHAR)) AS info
FROM retur_penjualan_detail
WHERE HARGA_BELI_SATUAN >= 99999999;

-- Verifikasi penjualan.TOTAL_HPP
SELECT CONCAT('  penjualan.TOTAL_HPP remaining >= 99999999: ',
    CAST(COUNT(*) AS CHAR)) AS info
FROM penjualan
WHERE TOTAL_HPP >= 99999999;

-- Verifikasi JurnalUmum
SELECT CONCAT('  Jurnal PERSEDIAAN BARANG (01.04.001) remaining >= 99999999: ',
    CAST(COUNT(*) AS CHAR)) AS info
FROM JurnalUmum
WHERE (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL >= 99999999;

SELECT CONCAT('  Jurnal HPP (06.01.001) remaining >= 99999999: ',
    CAST(COUNT(*) AS CHAR)) AS info
FROM JurnalUmum
WHERE (NOMOR_AKUN_D = '06.01.001' OR NOMOR_AKUN_K = '06.01.001')
  AND NOMINAL >= 99999999;

-- =============================================================================
-- CLEANUP & COMMIT
-- =============================================================================

DROP TEMPORARY TABLE tmp_fix_pd;
DROP TEMPORARY TABLE tmp_fix_rd;

COMMIT;

SELECT 'KOREKSI SELESAI' AS status;
