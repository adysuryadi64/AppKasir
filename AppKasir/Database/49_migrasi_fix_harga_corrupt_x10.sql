-- ============================================================
-- 47_migrasi_fix_harga_corrupt_x10.sql
-- ============================================================
-- Tujuan: Koreksi semua harga yang terkorupsi ×10/×100/×10000
--   akibat bug ParseDecimal culture id-ID pada ModuleAngka.
--
-- Yang diperbaiki (NOMINAL/HARGA saja):
--   penjualan_detail         — HARGA_BELI, HARGA_BELI_SATUAN, LABA
--   retur_penjualan_detail   — HARGA_BELI, HARGA_BELI_SATUAN, LABA
--   penjualan                — TOTAL_HPP
--   transfer_barang_detail   — HARGA, HARGA_QTY, TOTAL
--   transfer_barang          — TOTAL_RUPIAH
--   stok_opname              — HARGA, TOTAL_HARGA
--   JurnalUmum               — NOMINAL saja (tidak sentuh akun)
--   HistoryBarang            — TOTAL_RUPIAH
--
-- Yang TIDAK disentuh:
--   Kode/nama akun di JurnalUmum (NOMOR_AKUN_D/K, NAMA_AKUN_D/K)
--   Akun lawan stok opname (tetap 04.02.001)
--
-- Rumus:
--   HARGA_BELI_SATUAN = HARGA_BELI × QTY_SATUAN
--   LABA             = TOTAL_HARGA - HARGA_BELI_SATUAN
--   TOTAL_HPP        = SUM(HARGA_BELI_SATUAN) per faktur
--
-- IDEMPOTEN: aman dijalankan berulang
-- ============================================================

SET SESSION innodb_lock_wait_timeout = 300;
SET autocommit = 0;

START TRANSACTION;

-- ============================================================
-- BAGIAN A: DETEKSI
-- ============================================================

SELECT '=== BAGIAN A: DETEKSI ===' AS info;

-- A1: penjualan_detail corrupt
CREATE TEMPORARY TABLE tmp_fix_pd AS
SELECT pd.FAKTUR_JUAL, pd.ID_BARANG, pd.ISI_SATUAN,
       pd.QTY_SATUAN, pd.TOTAL_HARGA,
       pd.HARGA_BELI AS hb_corrupt,
       tb.HARGA_BELI AS hb_benar
FROM penjualan_detail pd
JOIN tbl_barang tb ON tb.ID_BARANG = pd.ID_BARANG
WHERE tb.HARGA_BELI > 0 AND pd.HARGA_BELI > tb.HARGA_BELI * 100;

SELECT CONCAT('  penjualan_detail corrupt: ', COUNT(*)) AS info FROM tmp_fix_pd;
SELECT CONCAT('  Faktur terdampak: ', COUNT(DISTINCT FAKTUR_JUAL)) AS info FROM tmp_fix_pd;

-- A2: retur_penjualan_detail corrupt
CREATE TEMPORARY TABLE tmp_fix_rd AS
SELECT rd.ID_RETUR_PENJUALAN, rd.ID_BARANG, rd.ISI_SATUAN,
       rd.QTY_SATUAN, rd.TOTAL_HARGA,
       rd.HARGA_BELI AS hb_corrupt,
       tb.HARGA_BELI AS hb_benar
FROM retur_penjualan_detail rd
JOIN tbl_barang tb ON tb.ID_BARANG = rd.ID_BARANG
WHERE tb.HARGA_BELI > 0 AND rd.HARGA_BELI > tb.HARGA_BELI * 100;

SELECT CONCAT('  retur_penjualan_detail corrupt: ', COUNT(*)) AS info FROM tmp_fix_rd;

-- A3: transfer_barang_detail overflow
CREATE TEMPORARY TABLE tmp_fix_tbd AS
SELECT td.ID_TRANSFER, td.ID_BARANG, td.ISI_SATUAN,
       td.TOTAL_QTY, td.HARGA AS harga_corrupt,
       tb.HARGA_BELI AS harga_benar
FROM transfer_barang_detail td
JOIN tbl_barang tb ON tb.ID_BARANG = td.ID_BARANG
WHERE td.HARGA >= 99999999;

SELECT CONCAT('  transfer_barang_detail overflow: ', COUNT(*)) AS info FROM tmp_fix_tbd;

-- A4: stok_opname ×100/1000/10000
CREATE TEMPORARY TABLE tmp_fix_so AS
SELECT so.ID_STOK_OPNAME, so.ID_BARANG, so.STOK_SELISIH,
       so.HARGA AS harga_corrupt,
       tb.HARGA_BELI AS harga_benar,
       ROUND(so.STOK_SELISIH * tb.HARGA_BELI, 0) AS total_benar
FROM stok_opname so
JOIN tbl_barang tb ON tb.ID_BARANG = so.ID_BARANG
WHERE tb.HARGA_BELI > 0
  AND ROUND(so.HARGA / tb.HARGA_BELI, 0) IN (100, 1000, 10000)
  AND ABS(so.HARGA / tb.HARGA_BELI
      - ROUND(so.HARGA / tb.HARGA_BELI, 0)) < 0.01;

SELECT CONCAT('  stok_opname corrupt: ', COUNT(*)) AS info FROM tmp_fix_so;

-- A5: EDIT BARANG jurnal inflasi
CREATE TEMPORARY TABLE tmp_fix_eb AS
SELECT NO_TRANSAKSI, NOMINAL
FROM JurnalUmum
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000;

SELECT CONCAT('  EDIT BARANG inflasi: ', COUNT(*)) AS info FROM tmp_fix_eb;

-- ============================================================
-- BAGIAN B: FIX penjualan_detail
-- ============================================================

SELECT '=== BAGIAN B: FIX penjualan_detail ===' AS info;

UPDATE penjualan_detail pd
JOIN tmp_fix_pd f ON f.FAKTUR_JUAL = pd.FAKTUR_JUAL AND f.ID_BARANG = pd.ID_BARANG
SET pd.HARGA_BELI        = f.hb_benar,
    pd.HARGA_BELI_SATUAN = ROUND(f.hb_benar * f.QTY_SATUAN, 4),
    pd.LABA              = ROUND(f.TOTAL_HARGA - (f.hb_benar * f.QTY_SATUAN), 2);

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN C: FIX retur_penjualan_detail
-- ============================================================

SELECT '=== BAGIAN C: FIX retur_penjualan_detail ===' AS info;

UPDATE retur_penjualan_detail rd
JOIN tmp_fix_rd f ON f.ID_RETUR_PENJUALAN = rd.ID_RETUR_PENJUALAN AND f.ID_BARANG = rd.ID_BARANG
SET rd.HARGA_BELI        = f.hb_benar,
    rd.HARGA_BELI_SATUAN = ROUND(f.hb_benar * f.QTY_SATUAN, 4),
    rd.LABA              = ROUND(f.TOTAL_HARGA - (f.hb_benar * f.QTY_SATUAN), 2);

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN D: FIX penjualan.TOTAL_HPP
-- ============================================================

SELECT '=== BAGIAN D: FIX penjualan.TOTAL_HPP ===' AS info;

CREATE TEMPORARY TABLE tmp_hpp AS
SELECT FAKTUR_JUAL, ROUND(SUM(HARGA_BELI_SATUAN), 2) AS hpp_benar
FROM penjualan_detail
WHERE FAKTUR_JUAL IN (SELECT DISTINCT FAKTUR_JUAL FROM tmp_fix_pd)
GROUP BY FAKTUR_JUAL;

UPDATE penjualan p JOIN tmp_hpp t ON t.FAKTUR_JUAL = p.ID_PENJUALAN
SET p.TOTAL_HPP = t.hpp_benar;

SELECT CONCAT('  Diperbaiki: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN E: FIX JurnalUmum NOMINAL — PENJUALAN
-- ============================================================

SELECT '=== BAGIAN E: FIX JurnalUmum PENJUALAN ===' AS info;

UPDATE JurnalUmum ju
JOIN tmp_hpp t ON t.FAKTUR_JUAL = ju.NO_TRANSAKSI
SET ju.NOMINAL = ROUND(t.hpp_benar, 0)
WHERE ju.JENIS_TRANSAKSI = 'PENJUALAN'
  AND ju.NOMOR_AKUN_D = '06.01.001'
  AND ABS(ju.NOMINAL - ROUND(t.hpp_benar, 0)) > 0;

SELECT CONCAT('  Fixed HPP DEBET: ', ROW_COUNT(), ' baris') AS info;

UPDATE JurnalUmum ju
JOIN tmp_hpp t ON t.FAKTUR_JUAL = ju.NO_TRANSAKSI
SET ju.NOMINAL = ROUND(t.hpp_benar, 0)
WHERE ju.JENIS_TRANSAKSI = 'PENJUALAN'
  AND ju.NOMOR_AKUN_K = '01.04.001'
  AND ABS(ju.NOMINAL - ROUND(t.hpp_benar, 0)) > 0;

SELECT CONCAT('  Fixed PERSEDIAAN KREDIT: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN F: FIX JurnalUmum NOMINAL — RETUR PENJUALAN
-- ============================================================

SELECT '=== BAGIAN F: FIX JurnalUmum RETUR PENJUALAN ===' AS info;

CREATE TEMPORARY TABLE tmp_hpp_rd AS
SELECT ID_RETUR_PENJUALAN, ROUND(SUM(HARGA_BELI_SATUAN), 0) AS hpp_benar
FROM retur_penjualan_detail
WHERE ID_RETUR_PENJUALAN IN (SELECT DISTINCT ID_RETUR_PENJUALAN FROM tmp_fix_rd)
GROUP BY ID_RETUR_PENJUALAN;

UPDATE JurnalUmum ju
JOIN tmp_hpp_rd t ON t.ID_RETUR_PENJUALAN = ju.NO_TRANSAKSI
SET ju.NOMINAL = t.hpp_benar
WHERE ju.JENIS_TRANSAKSI = 'RETUR PENJUALAN'
  AND ju.NOMOR_AKUN_D = '01.04.001'
  AND ABS(ju.NOMINAL - t.hpp_benar) > 0;

SELECT CONCAT('  Fixed PERSEDIAAN DEBET: ', ROW_COUNT(), ' baris') AS info;

UPDATE JurnalUmum ju
JOIN tmp_hpp_rd t ON t.ID_RETUR_PENJUALAN = ju.NO_TRANSAKSI
SET ju.NOMINAL = t.hpp_benar
WHERE ju.JENIS_TRANSAKSI = 'RETUR PENJUALAN'
  AND ju.NOMOR_AKUN_K = '06.01.001'
  AND ABS(ju.NOMINAL - t.hpp_benar) > 0;

SELECT CONCAT('  Fixed HPP KREDIT: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN G: FIX transfer_barang_detail + header + jurnal
-- ============================================================

SELECT '=== BAGIAN G: FIX transfer_barang_detail ===' AS info;

UPDATE transfer_barang_detail td
JOIN tmp_fix_tbd f ON f.ID_TRANSFER = td.ID_TRANSFER AND f.ID_BARANG = td.ID_BARANG
SET td.HARGA     = f.harga_benar,
    td.HARGA_QTY = ROUND(f.harga_benar * f.ISI_SATUAN, 2),
    td.TOTAL     = ROUND(f.harga_benar * f.TOTAL_QTY, 0);

SELECT CONCAT('  Detail: ', ROW_COUNT(), ' baris') AS info;

UPDATE transfer_barang tb
JOIN (SELECT ID_TRANSFER, SUM(TOTAL) AS total_benar FROM transfer_barang_detail
      WHERE ID_TRANSFER IN (SELECT DISTINCT ID_TRANSFER FROM tmp_fix_tbd)
      GROUP BY ID_TRANSFER) c ON c.ID_TRANSFER = tb.ID_TRANSFER
SET tb.TOTAL_RUPIAH = c.total_benar;

SELECT CONCAT('  Header: ', ROW_COUNT(), ' baris') AS info;

UPDATE JurnalUmum ju
JOIN (SELECT ID_TRANSFER, SUM(TOTAL) AS total_benar FROM transfer_barang_detail
      WHERE ID_TRANSFER IN (SELECT DISTINCT ID_TRANSFER FROM tmp_fix_tbd)
      GROUP BY ID_TRANSFER) c ON c.ID_TRANSFER = ju.NO_TRANSAKSI
SET ju.NOMINAL = c.total_benar
WHERE ju.JENIS_TRANSAKSI = 'TRANSFER BARANG';

SELECT CONCAT('  Jurnal: ', ROW_COUNT(), ' baris') AS info;

UPDATE HistoryBarang hb
JOIN tmp_fix_tbd f ON f.ID_TRANSFER = hb.FAKTUR
JOIN tbl_barang b ON b.ID_BARANG = hb.ID_BARANG
SET hb.TOTAL_RUPIAH = hb.TOTAL_QTY * b.HARGA_BELI
WHERE hb.JENIS IN ('TRANSFER BARANG MASUK', 'TRANSFER BARANG KELUAR');

SELECT CONCAT('  HistoryBarang: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN H: FIX stok_opname + jurnal
-- ============================================================

SELECT '=== BAGIAN H: FIX stok_opname ===' AS info;

UPDATE stok_opname so
JOIN tmp_fix_so f ON f.ID_STOK_OPNAME = so.ID_STOK_OPNAME
SET so.HARGA = f.harga_benar, so.TOTAL_HARGA = f.total_benar;

SELECT CONCAT('  Stok opname: ', ROW_COUNT(), ' baris') AS info;

UPDATE JurnalUmum ju
JOIN tmp_fix_so f ON f.ID_STOK_OPNAME = ju.NO_TRANSAKSI
SET ju.NOMINAL = ABS(f.total_benar)
WHERE ju.JENIS_TRANSAKSI = 'STOK OPNAME'
  AND ABS(ju.NOMINAL - ABS(f.total_benar)) > 0;

SELECT CONCAT('  Jurnal: ', ROW_COUNT(), ' baris') AS info;

-- ============================================================
-- BAGIAN I: FIX EDIT BARANG jurnal NOMINAL
-- ============================================================

SELECT '=== BAGIAN I: FIX EDIT BARANG jurnal ===' AS info;

UPDATE JurnalUmum SET NOMINAL = ROUND(NOMINAL / 10000, 0)
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000
  AND NOMINAL - ROUND(NOMINAL / 10000, 0) * 10000 = 0;
SELECT CONCAT('  ×10000: ', ROW_COUNT(), ' entries') AS info;

UPDATE JurnalUmum SET NOMINAL = ROUND(NOMINAL / 1000, 0)
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000
  AND NOMINAL - ROUND(NOMINAL / 1000, 0) * 1000 BETWEEN -500 AND 500;
SELECT CONCAT('  ×1000: ', ROW_COUNT(), ' entries') AS info;

UPDATE JurnalUmum SET NOMINAL = 325215000
WHERE NO_TRANSAKSI = '20250709071714'
  AND JENIS_TRANSAKSI = 'EDIT BARANG' AND NOMINAL > 50000000;
SELECT CONCAT('  Ekonomi Cair: ', ROW_COUNT(), ' entry') AS info;

-- ============================================================
-- BAGIAN J: RECALCULATE saldo
-- ============================================================

SELECT '=== BAGIAN J: RECALCULATE ===' AS info;
CALL sp_bat_saldo_semua_akun();

-- ============================================================
-- BAGIAN K: VERIFIKASI
-- ============================================================

SELECT '=== BAGIAN K: VERIFIKASI ===' AS info;

SELECT CONCAT('  penjualan_detail corrupt: ', COUNT(*)) AS info
FROM penjualan_detail pd JOIN tbl_barang b ON pd.ID_BARANG = b.ID_BARANG
WHERE b.HARGA_BELI > 0 AND pd.HARGA_BELI > b.HARGA_BELI * 100;

SELECT CONCAT('  retur_penjualan_detail corrupt: ', COUNT(*)) AS info
FROM retur_penjualan_detail rd JOIN tbl_barang b ON rd.ID_BARANG = b.ID_BARANG
WHERE b.HARGA_BELI > 0 AND rd.HARGA_BELI > b.HARGA_BELI * 100;

SELECT CONCAT('  transfer_barang_detail overflow: ', COUNT(*)) AS info
FROM transfer_barang_detail WHERE HARGA >= 99999999;

SELECT CONCAT('  stok_opname corrupt: ', COUNT(*)) AS info
FROM stok_opname so JOIN tbl_barang b ON so.ID_BARANG = b.ID_BARANG
WHERE b.HARGA_BELI > 0 AND ROUND(so.HARGA / b.HARGA_BELI, 0) IN (100, 1000, 10000)
  AND ABS(so.HARGA / b.HARGA_BELI - ROUND(so.HARGA / b.HARGA_BELI, 0)) < 0.01;

SELECT CONCAT('  EDIT BARANG inflasi: ', COUNT(*)) AS info
FROM JurnalUmum WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001') AND NOMINAL > 50000000;

SELECT KODE_AKUN, NAMA_AKUN, SALDO_AKHIR FROM tbl_datareferensi
WHERE KODE_AKUN IN ('01.04.001', '06.01.001', '04.02.001');

-- ============================================================
-- CLEANUP & COMMIT
-- ============================================================

DROP TEMPORARY TABLE tmp_fix_pd;
DROP TEMPORARY TABLE tmp_fix_rd;
DROP TEMPORARY TABLE tmp_fix_tbd;
DROP TEMPORARY TABLE tmp_fix_so;
DROP TEMPORARY TABLE tmp_fix_eb;
DROP TEMPORARY TABLE tmp_hpp;
DROP TEMPORARY TABLE tmp_hpp_rd;

COMMIT;

SELECT 'MIGRASI 47 SELESAI' AS status;
