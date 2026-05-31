-- ============================================================
-- 37_migrasi_fix_transfer_stok_harga_salah.sql
-- ============================================================
-- Tujuan: Koreksi data transfer_stok & transfer_barang_detail
--   yang HARGA-nya overflow ke 99999999.99 akibat bug konversi decimal.
--
-- Penyebab bug:
--   Decimal dari MySQL (misal 343500.0000) di-ToString() menghasilkan
--   "343500.0000". ParseDecimal membaca 4 digit setelah titik sebagai
--   pemisah ribuan di culture id-ID → 3435000000 (salah ×10000).
--   Kolom HARGA_SAT_M/K dan HARGA bertipe DECIMAL(10,2), max = 99,999,999.99
--   Bug menyebabkan nilai overflow → tersimpan sebagai 99999999.99.
--
-- STRATEGI DETEKSI (aman, tidak menyentuh data lama yang valid):
--   Hanya koreksi baris di mana HARGA >= 99999999.
--   Nilai ini mustahil untuk harga satuan barang normal — ciri khas bug.
--   Sumber harga yang benar: tbl_barang.HARGA_BELI saat ini.
--
-- Tabel terdampak:
--   transfer_stok, transfer_barang_detail, transfer_barang,
--   JurnalUmum, HistoryBarang
--
-- PENTING: Gunakan TEMPORARY TABLE untuk simpan ID sebelum UPDATE,
--   agar subquery di Step 2 & 3 tidak terpengaruh perubahan Step 1.
--
-- SUDAH DITEST di db_moroseneng — hasil benar, COMMIT berhasil.
--
-- Cara pakai:
--   mysql -u root -p db_anda < 37_migrasi_fix_transfer_stok_harga_salah.sql
-- ============================================================

START TRANSACTION;

-- =============================================================================
-- BAGIAN A: KOREKSI transfer_stok
-- =============================================================================

CREATE TEMPORARY TABLE tmp_fix_ids AS
SELECT
    ts.ID_TRANSFER,
    TRUNCATE(bm.HARGA_BELI, 0) AS harga_m_benar,
    TRUNCATE(bk.HARGA_BELI, 0) AS harga_k_benar,
    ts.QTY_SAT_M,
    ts.QTY_SAT_K
FROM transfer_stok ts
JOIN tbl_barang bm ON bm.ID_BARANG = ts.ID_BARANG_M
JOIN tbl_barang bk ON bk.ID_BARANG = ts.ID_BARANG_K
WHERE ts.HARGA_SAT_M >= 99999999
   OR ts.HARGA_SAT_K >= 99999999;

UPDATE transfer_stok ts
JOIN tmp_fix_ids f ON f.ID_TRANSFER = ts.ID_TRANSFER
SET
    ts.HARGA_SAT_M   = f.harga_m_benar,
    ts.TOTAL_HARGA_M = f.QTY_SAT_M * f.harga_m_benar,
    ts.HARGA_SAT_K   = f.harga_k_benar,
    ts.TOTAL_HARGA_K = f.QTY_SAT_K * f.harga_k_benar,
    ts.Selisih       = (f.QTY_SAT_M * f.harga_m_benar)
                     - (f.QTY_SAT_K * f.harga_k_benar);

UPDATE JurnalUmum ju
JOIN tmp_fix_ids f ON f.ID_TRANSFER = ju.NO_TRANSAKSI
SET ju.NOMINAL = ABS(
    (f.QTY_SAT_M * f.harga_m_benar)
  - (f.QTY_SAT_K * f.harga_k_benar)
)
WHERE ju.JENIS_TRANSAKSI = 'TRANSFER STOK';

UPDATE HistoryBarang hb
JOIN tbl_barang b ON b.ID_BARANG = hb.ID_BARANG
JOIN tmp_fix_ids f ON f.ID_TRANSFER = hb.FAKTUR
SET hb.TOTAL_RUPIAH = hb.TOTAL_QTY * TRUNCATE(b.HARGA_BELI, 0)
WHERE hb.JENIS IN ('TRANSFER BARANG MASUK', 'TRANSFER BARANG KELUAR');

DROP TEMPORARY TABLE tmp_fix_ids;

SELECT CONCAT('[transfer_stok] ', ROW_COUNT(), ' baris HistoryBarang diperbaiki') AS info;

-- =============================================================================
-- BAGIAN B: KOREKSI transfer_barang_detail & transfer_barang
-- =============================================================================

CREATE TEMPORARY TABLE tmp_fix_barang AS
SELECT
    td.ID_TRANSFER,
    td.ID_BARANG,
    td.ISI_SATUAN,
    td.TOTAL_QTY,
    TRUNCATE(b.HARGA_BELI, 0) AS harga_benar
FROM transfer_barang_detail td
JOIN tbl_barang b ON b.ID_BARANG = td.ID_BARANG
WHERE td.HARGA >= 99999999
   OR td.HARGA_QTY >= 99999999
   OR td.TOTAL >= 99999999;

UPDATE transfer_barang_detail td
JOIN tmp_fix_barang f
    ON f.ID_TRANSFER = td.ID_TRANSFER
   AND f.ID_BARANG   = td.ID_BARANG
SET
    td.HARGA     = f.harga_benar,
    td.HARGA_QTY = f.harga_benar * f.ISI_SATUAN,
    td.TOTAL     = f.harga_benar * f.TOTAL_QTY;

UPDATE transfer_barang tb
JOIN (
    SELECT td.ID_TRANSFER, SUM(td.TOTAL) AS total_benar
    FROM transfer_barang_detail td
    WHERE td.ID_TRANSFER IN (SELECT DISTINCT ID_TRANSFER FROM tmp_fix_barang)
    GROUP BY td.ID_TRANSFER
) c ON c.ID_TRANSFER = tb.ID_TRANSFER
SET tb.TOTAL_RUPIAH = c.total_benar;

UPDATE JurnalUmum ju
JOIN (
    SELECT td.ID_TRANSFER, SUM(td.TOTAL) AS total_benar
    FROM transfer_barang_detail td
    WHERE td.ID_TRANSFER IN (SELECT DISTINCT ID_TRANSFER FROM tmp_fix_barang)
    GROUP BY td.ID_TRANSFER
) c ON c.ID_TRANSFER = ju.NO_TRANSAKSI
SET ju.NOMINAL = c.total_benar
WHERE ju.JENIS_TRANSAKSI = 'TRANSFER BARANG';

UPDATE HistoryBarang hb
JOIN tbl_barang b ON b.ID_BARANG = hb.ID_BARANG
JOIN tmp_fix_barang f ON f.ID_TRANSFER = hb.FAKTUR
SET hb.TOTAL_RUPIAH = hb.TOTAL_QTY * f.harga_benar
WHERE hb.JENIS IN ('TRANSFER BARANG MASUK', 'TRANSFER BARANG KELUAR');

DROP TEMPORARY TABLE tmp_fix_barang;

SELECT CONCAT('[transfer_barang] ', ROW_COUNT(), ' baris HistoryBarang diperbaiki') AS info;

COMMIT;

SELECT 'KOREKSI SELESAI' AS status;
