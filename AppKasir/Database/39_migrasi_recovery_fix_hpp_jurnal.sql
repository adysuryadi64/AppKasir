-- ============================================================
-- 39_migrasi_recovery_fix_hpp_jurnal.sql
-- ============================================================
-- Tujuan: Perbaiki side effect migrasi 38 yang salah set
--   HARGA_BELI_SATUAN = HB * ISI (per-unit).
--
--   Pada FormJual / FormReturPenjualan, kolom HARGA_BELI_SATUAN
--   menyimpan TOTAL LINE COST = HB * ISI * QTY, BUKAN per-unit.
--   Nama kolom menyesatkan, tapi kode sumber menggunakannya
--   sebagai total biaya per baris.
--
--   Rumus benar:
--     penjualan_detail:
--       HARGA_BELI_SATUAN = HB * ISI_SATUAN * QTY_SATUAN
--       LABA             = TOTAL_HARGA - HARGA_BELI_SATUAN
--     retur_penjualan_detail:
--       HARGA_BELI_SATUAN = HB * QTY_SATUAN
--       LABA             = TOTAL_HARGA - HARGA_BELI_SATUAN
--     penjualan.TOTAL_HPP = SUM(HARGA_BELI_SATUAN)   -- sudah termasuk QTY
--     JurnalUmum.NOMINAL  = SUM(HARGA_BELI_SATUAN)
--
-- IDEMPOTEN: aman dijalankan berulang
-- ============================================================

SET SESSION innodb_lock_wait_timeout = 300;

SELECT '=== BAGIAN 1: FIX HARGA_BELI_SATUAN + LABA penjualan_detail ===' AS info;

-- Deteksi yg salah: HBS = HB * ISI (per-unit, tanpa QTY)
-- Ciri: ABS(HBS - HB*ISI) < 1 TETAPI HBS != HB*ISI*QTY (saat QTY > 1)
SELECT CONCAT('  Terdeteksi ', COUNT(*), ' baris HBS per-unit (salah)') AS info
FROM penjualan_detail
WHERE QTY_SATUAN > 0 AND ISI_SATUAN > 0
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN, 4)) < 1
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN * QTY_SATUAN, 4)) > 1;

-- Fix: set HBS ke total line cost + LABA
UPDATE penjualan_detail
SET HARGA_BELI_SATUAN = ROUND(HARGA_BELI * ISI_SATUAN * QTY_SATUAN, 4),
    LABA = ROUND(TOTAL_HARGA - (HARGA_BELI * ISI_SATUAN * QTY_SATUAN), 2)
WHERE QTY_SATUAN > 0 AND ISI_SATUAN > 0
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN, 4)) < 1
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN * QTY_SATUAN, 4)) > 1;

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris penjualan_detail') AS info;

-- 1b: Safety net — perbaiki LABA untuk SEMUA penjualan_detail yg masih salah
-- (mencakup record yg mungkin rusak oleh test v1 script sebelumnya)
SELECT CONCAT('  Safety net: ', COUNT(*), ' baris LABA penjualan_detail masih salah') AS info
FROM penjualan_detail
WHERE HARGA_BELI_SATUAN > 0
  AND ABS(ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2) - LABA) > 100;

UPDATE penjualan_detail
SET LABA = ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2)
WHERE HARGA_BELI_SATUAN > 0
  AND ABS(ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2) - LABA) > 100;

SELECT CONCAT('  Safety net fixed: ', ROW_COUNT(), ' baris penjualan_detail') AS info;


SELECT '=== BAGIAN 2: FIX HARGA_BELI_SATUAN + LABA retur_penjualan_detail ===' AS info;

-- Deteksi yg salah (jika migrasi 38 versi "fix" saya yg pakai ISI dijalankan):
-- Ciri: HBS = HB * ISI (salah untuk retur, harusnya HB * QTY)
SELECT CONCAT('  Terdeteksi ', COUNT(*), ' baris HBS retur salah (ISI)') AS info
FROM retur_penjualan_detail
WHERE QTY_SATUAN > 0 AND ISI_SATUAN > 0
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN, 4)) < 1
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * QTY_SATUAN, 4)) > 1;

UPDATE retur_penjualan_detail
SET HARGA_BELI_SATUAN = ROUND(HARGA_BELI * QTY_SATUAN, 4),
    LABA = ROUND(TOTAL_HARGA - (HARGA_BELI * QTY_SATUAN), 2)
WHERE QTY_SATUAN > 0 AND ISI_SATUAN > 0
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN, 4)) < 1
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * QTY_SATUAN, 4)) > 1;

SELECT CONCAT('  Diperbaiki HBS+LABA retur: ', ROW_COUNT(), ' baris') AS info;

-- 2b: Safety net — perbaiki LABA retur yg masih salah
SELECT CONCAT('  Safety net retur: ', COUNT(*), ' baris LABA retur masih salah') AS info
FROM retur_penjualan_detail
WHERE HARGA_BELI_SATUAN > 0
  AND ABS(ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2) - LABA) > 100;

UPDATE retur_penjualan_detail
SET LABA = ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2)
WHERE HARGA_BELI_SATUAN > 0
  AND ABS(ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2) - LABA) > 100;

SELECT CONCAT('  Safety net fixed retur: ', ROW_COUNT(), ' baris') AS info;

-- Juga perbaiki LABA untuk retur yg HBS-nya sudah benar (HB*QTY) tapi LABA-nya salah
-- (misal dari migrasi 38 asli yg pakai LABA = TOTAL - HB*QTY saja, yg mana = TOTAL - HBS = sama saja...)
-- Sebenarnya jika HBS = HB*QTY dan LABA = TOTAL - HBS, sudah benar.
-- Tapi periksa kasus LABA overflow:
SELECT CONCAT('  Terdeteksi ', COUNT(*), ' baris LABA retur overflow') AS info
FROM retur_penjualan_detail
WHERE LABA <= -99999999;

UPDATE retur_penjualan_detail
SET LABA = ROUND(TOTAL_HARGA - HARGA_BELI_SATUAN, 2)
WHERE LABA <= -99999999 AND HARGA_BELI_SATUAN > 0;

SELECT CONCAT('  Diperbaiki LABA overflow retur: ', ROW_COUNT(), ' baris') AS info;


SELECT '=== BAGIAN 3: FIX TOTAL_HPP penjualan ===' AS info;

-- TOTAL_HPP = SUM(HARGA_BELI_SATUAN) — karena HBS sudah total line cost
-- Ini sama dengan formula migrasi 38 Bagian C, tapi SEKARANG HBS-nya sudah benar (total cost)
CREATE TEMPORARY TABLE tmp_hpp AS
SELECT p.ID_PENJUALAN,
       p.TOTAL_HPP AS hpp_lama,
       ROUND(SUM(pd.HARGA_BELI_SATUAN), 2) AS hpp_benar
FROM penjualan p
JOIN penjualan_detail pd ON pd.FAKTUR_JUAL = p.ID_PENJUALAN
GROUP BY p.ID_PENJUALAN, p.TOTAL_HPP
HAVING ABS(p.TOTAL_HPP - ROUND(SUM(pd.HARGA_BELI_SATUAN), 2)) > 100;

SELECT CONCAT('  Terdeteksi ', COUNT(*), ' penjualan dgn TOTAL_HPP salah') AS info FROM tmp_hpp;

UPDATE penjualan p
JOIN tmp_hpp t ON t.ID_PENJUALAN = p.ID_PENJUALAN
SET p.TOTAL_HPP = t.hpp_benar;

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris penjualan.TOTAL_HPP') AS info;


SELECT '=== BAGIAN 4: FIX JurnalUmum ===' AS info;

-- 4a: PENJUALAN — HPP (06.01.001) DEBET = TOTAL_HPP = SUM(HBS)
CREATE TEMPORARY TABLE tmp_jurnal_p AS
SELECT pd.FAKTUR_JUAL,
       COALESCE(ROUND(SUM(pd.HARGA_BELI_SATUAN), 0), 0) AS nominal_benar
FROM penjualan_detail pd
GROUP BY pd.FAKTUR_JUAL;

UPDATE JurnalUmum ju
JOIN tmp_jurnal_p t ON t.FAKTUR_JUAL = ju.NO_TRANSAKSI
SET ju.NOMINAL = t.nominal_benar
WHERE ju.JENIS_TRANSAKSI = 'PENJUALAN'
  AND ju.NOMOR_AKUN_D = '06.01.001'
  AND ABS(ju.NOMINAL - t.nominal_benar) > 0;

SELECT CONCAT('  Fixed HPP PENJUALAN (06.01.001): ', ROW_COUNT(), ' baris') AS info;

-- 4b: PENJUALAN — PERSEDIAAN BARANG (01.04.001) KREDIT = TOTAL_HPP
UPDATE JurnalUmum ju
JOIN tmp_jurnal_p t ON t.FAKTUR_JUAL = ju.NO_TRANSAKSI
SET ju.NOMINAL = t.nominal_benar
WHERE ju.JENIS_TRANSAKSI = 'PENJUALAN'
  AND ju.NOMOR_AKUN_K = '01.04.001'
  AND ABS(ju.NOMINAL - t.nominal_benar) > 0;

SELECT CONCAT('  Fixed PERSEDIAAN PENJUALAN (01.04.001): ', ROW_COUNT(), ' baris') AS info;

-- 4c: RETUR PENJUALAN — PERSEDIAAN BARANG (01.04.001) DEBET
CREATE TEMPORARY TABLE tmp_jurnal_rd AS
SELECT rd.ID_RETUR_PENJUALAN,
       COALESCE(ROUND(SUM(rd.HARGA_BELI_SATUAN), 0), 0) AS nominal_benar
FROM retur_penjualan_detail rd
GROUP BY rd.ID_RETUR_PENJUALAN;

UPDATE JurnalUmum ju
JOIN tmp_jurnal_rd t ON t.ID_RETUR_PENJUALAN = ju.NO_TRANSAKSI
SET ju.NOMINAL = t.nominal_benar
WHERE ju.JENIS_TRANSAKSI = 'RETUR PENJUALAN'
  AND ju.NOMOR_AKUN_D = '01.04.001'
  AND ABS(ju.NOMINAL - t.nominal_benar) > 0;

SELECT CONCAT('  Fixed PERSEDIAAN RETUR (01.04.001): ', ROW_COUNT(), ' baris') AS info;

-- 4d: RETUR PENJUALAN — HPP (06.01.001) KREDIT
UPDATE JurnalUmum ju
JOIN tmp_jurnal_rd t ON t.ID_RETUR_PENJUALAN = ju.NO_TRANSAKSI
SET ju.NOMINAL = t.nominal_benar
WHERE ju.JENIS_TRANSAKSI = 'RETUR PENJUALAN'
  AND ju.NOMOR_AKUN_K = '06.01.001'
  AND ABS(ju.NOMINAL - t.nominal_benar) > 0;

SELECT CONCAT('  Fixed HPP RETUR (06.01.001): ', ROW_COUNT(), ' baris') AS info;


SELECT '=== BAGIAN 5: RECALCULATE tbl_datareferensi ===' AS info;

CALL sp_bat_saldo_semua_akun();

SELECT CONCAT('  sp_bat_saldo_semua_akun selesai') AS info;


SELECT '=== BAGIAN 6: VERIFIKASI ===' AS info;

-- Verifikasi penjualan_detail: tidak ada HBS per-unit
SELECT CONCAT('  HBS per-unit tersisa: ', COUNT(*), ' baris') AS info
FROM penjualan_detail
WHERE QTY_SATUAN > 1 AND ISI_SATUAN > 0
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN, 4)) < 1
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN * QTY_SATUAN, 4)) > 1;

-- Verifikasi retur: tidak ada HBS pakai ISI
SELECT CONCAT('  HBS retur pakai ISI tersisa: ', COUNT(*), ' baris') AS info
FROM retur_penjualan_detail
WHERE QTY_SATUAN > 0 AND ISI_SATUAN > 0
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * ISI_SATUAN, 4)) < 1
  AND ABS(HARGA_BELI_SATUAN - ROUND(HARGA_BELI * QTY_SATUAN, 4)) > 1;

-- Verifikasi PERSEDIAAN BARANG
SELECT NAMA_AKUN, S_DEBET, S_KREDIT, SALDO_AKHIR,
       S_DEBET - S_KREDIT AS saldo_hitung
FROM tbl_datareferensi
WHERE NAMA_AKUN = 'PERSEDIAAN BARANG';

SELECT CONCAT('  Selesai') AS info;
