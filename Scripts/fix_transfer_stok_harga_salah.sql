-- ============================================================
-- SCRIPT KOREKSI: Transfer Stok — Harga Tidak Masuk Akal
-- ============================================================
-- Penyebab bug:
--   Decimal dari MySQL (misal 343500.0000) di-ToString() menghasilkan
--   "343500.0000". ParseDecimal membaca 4 digit setelah titik sebagai
--   pemisah ribuan → hapus titik → 3435000000 (salah).
--   Kolom HARGA_SAT_M/K bertipe DECIMAL(10,2), max = 99,999,999.99
--   Bug menyebabkan nilai overflow → tersimpan sebagai 99999999.99 (max cap).
--
-- STRATEGI DETEKSI (aman, tidak menyentuh data lama yang valid):
--   Hanya koreksi baris di mana HARGA_SAT_M atau HARGA_SAT_K >= 99999999.
--   Nilai ini mustahil untuk harga satuan barang normal — ciri khas bug.
--   Sumber harga yang benar: tbl_barang.HARGA_BELI saat ini.
--
-- PENTING: Gunakan TEMPORARY TABLE untuk simpan ID sebelum UPDATE,
--   agar subquery di Step 2 & 3 tidak terpengaruh perubahan Step 1.
--
-- SUDAH DITEST di db_moroseneng — hasil benar, COMMIT berhasil.
--
-- JALANKAN DI PRODUKSI:
--   mysql -u root -p12345678 db_rejeki < fix_transfer_stok_harga_salah.sql
-- ============================================================

START TRANSACTION;

-- Simpan ID + nilai benar sebelum UPDATE mengubah data
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

-- Step 1: Koreksi transfer_stok
UPDATE transfer_stok ts
JOIN tmp_fix_ids f ON f.ID_TRANSFER = ts.ID_TRANSFER
SET
    ts.HARGA_SAT_M   = f.harga_m_benar,
    ts.TOTAL_HARGA_M = f.QTY_SAT_M * f.harga_m_benar,
    ts.HARGA_SAT_K   = f.harga_k_benar,
    ts.TOTAL_HARGA_K = f.QTY_SAT_K * f.harga_k_benar,
    ts.Selisih       = (f.QTY_SAT_M * f.harga_m_benar)
                     - (f.QTY_SAT_K * f.harga_k_benar);

-- Step 2: Koreksi JurnalUmum
UPDATE JurnalUmum ju
JOIN tmp_fix_ids f ON f.ID_TRANSFER = ju.NO_TRANSAKSI
SET ju.NOMINAL = ABS(
    (f.QTY_SAT_M * f.harga_m_benar)
  - (f.QTY_SAT_K * f.harga_k_benar)
)
WHERE ju.JENIS_TRANSAKSI = 'TRANSFER STOK';

-- Step 3: Koreksi HistoryBarang
UPDATE HistoryBarang hb
JOIN tbl_barang b ON b.ID_BARANG = hb.ID_BARANG
JOIN tmp_fix_ids f ON f.ID_TRANSFER = hb.FAKTUR
SET hb.TOTAL_RUPIAH = hb.TOTAL_QTY * TRUNCATE(b.HARGA_BELI, 0)
WHERE hb.JENIS IN ('TRANSFER BARANG MASUK', 'TRANSFER BARANG KELUAR');

DROP TEMPORARY TABLE tmp_fix_ids;

COMMIT;

SELECT 'KOREKSI SELESAI' AS status;
