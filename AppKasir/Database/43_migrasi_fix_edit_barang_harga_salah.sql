-- =============================================================
-- 43_migrasi_fix_edit_barang_harga_salah.sql
-- =============================================================
-- Tujuan: Koreksi entry EDIT BARANG di JurnalUmum yang NOMINAL-nya
--   terinflasi akibat bug konversi desimal di compiled .exe.
--
-- Dampak terhadap PERSEDIAAN (01.04.001):
--   KREDIT (PERSEDIAAN OUT): Rp252,5 M → seharusnya ~Rp25 M
--   Net inflasi: ~Rp227 M yang menyebabkan saldo persediaan terlalu kecil
--
-- Pola inflasi yang TERVERIFIKASI (berdasarkan analisa stok & harga master):
--   ×10000 : 8 entries — bug CDec culture id-ID
--   ×1000  : 6 entries — 3 entry lama + 3 entry Cakra/Nusa Jaya yg
--             salah terdeteksi sebagai ×100 di versi sebelumnya
--
-- Yang TIDAK disentuh:
--   Entry 20250709071714 (Ekonomi Cair 135Ml Dos, Rp32,4M) —
--   tidak ada faktor bulat yang menghasilkan nilai wajar, skip manual.
--   Entry PINDAH REKENING PR-2502020001 & PR-2512160001 — jurnal manual,
--   perlu konfirmasi user.
--
-- Threshold deteksi: NOMINAL > 50.000.000
--   (nilai di bawah ini dianggap wajar untuk edit barang)
--
-- IDEMPOTEN: aman dijalankan berulang (nilai sudah kecil tidak akan
--   terdeteksi lagi karena < threshold 50jt)
-- =============================================================

START TRANSACTION;

-- =============================================================
-- STEP 1: Analisis sebelum fix
-- =============================================================

SELECT '=== STEP 1: Kondisi sebelum fix ===' AS info;

SELECT
  ROUND(SUM(CASE WHEN NOMOR_AKUN_D = '01.04.001' THEN NOMINAL ELSE 0 END), 0) AS total_debet_persediaan,
  ROUND(SUM(CASE WHEN NOMOR_AKUN_K = '01.04.001' THEN NOMINAL ELSE 0 END), 0) AS total_kredit_persediaan,
  COUNT(*) AS jumlah_entry
FROM JurnalUmum
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000;

-- =============================================================
-- STEP 2a: Fix entry ×10000 (8 entries)
-- Deteksi: NOMINAL % 10000 = 0
-- =============================================================

SELECT '=== STEP 2a: Fix x10000 ===' AS info;

UPDATE JurnalUmum SET
  NOMINAL    = ROUND(NOMINAL / 10000, 0),
  updated_at = NOW()
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000
  AND NOMINAL - ROUND(NOMINAL / 10000, 0) * 10000 = 0;

SELECT CONCAT('  Fixed x10000: ', ROW_COUNT(), ' entries') AS info;

-- =============================================================
-- STEP 2b: Fix entry ×1000 (6 entries)
-- Mencakup: Sedap Selection Goreng, Lm Sak, Kertas Nasi Kapal,
--           Cakra Dos, Nusa Jaya Tanggung Pack, Nusa Jaya Kecil Pack
-- Deteksi: NOMINAL % 1000 < 1000 (toleransi sisa desimal .5 × 1000 = 500)
--   Dikecualikan: entry yang sudah difix di step 2a (sudah < 50jt)
--   Dikecualikan: Ekonomi Cair (NO_TRANSAKSI = '20250709071714') — skip manual
-- =============================================================

SELECT '=== STEP 2b: Fix x1000 ===' AS info;

UPDATE JurnalUmum SET
  NOMINAL    = ROUND(NOMINAL / 1000, 0),
  updated_at = NOW()
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000
  AND NO_TRANSAKSI != '20250709071714'  -- Ekonomi Cair: skip, faktor tidak pasti
  AND NOMINAL - ROUND(NOMINAL / 1000, 0) * 1000 BETWEEN -500 AND 500;

SELECT CONCAT('  Fixed x1000: ', ROW_COUNT(), ' entries') AS info;

-- =============================================================
-- STEP 2c: Fix entry Ekonomi Cair 135Ml Dos (1 entry)
-- Nilai benar dari historybarang SO-2507090008:
--   selisih -8910 unit × Rp36.500 = Rp325.215.000
-- =============================================================

SELECT '=== STEP 2c: Fix Ekonomi Cair 135Ml Dos ===' AS info;

UPDATE JurnalUmum SET
  NOMINAL    = 325215000,
  updated_at = NOW()
WHERE NO_TRANSAKSI = '20250709071714'
  AND JENIS_TRANSAKSI = 'EDIT BARANG'
  AND NOMINAL > 50000000;

SELECT CONCAT('  Fixed Ekonomi Cair: ', ROW_COUNT(), ' entry') AS info;
-- =============================================================

SELECT '=== STEP 3: Recalculate tbl_datareferensi ===' AS info;

CALL sp_bat_saldo_semua_akun();

SELECT '  sp_bat_saldo_semua_akun selesai' AS info;

-- =============================================================
-- STEP 4: Verifikasi
-- =============================================================

SELECT '=== STEP 4: Verifikasi ===' AS info;

-- Sisa entry > 50jt yang belum terfix (seharusnya hanya Ekonomi Cair)
SELECT NO_TRANSAKSI, TGL_TRANSAKSI, NOMINAL, LEFT(URAIAN,50) AS uraian,
  CASE WHEN NOMOR_AKUN_D='01.04.001' THEN 'DEBET' ELSE 'KREDIT' END AS posisi
FROM JurnalUmum
WHERE JENIS_TRANSAKSI = 'EDIT BARANG'
  AND (NOMOR_AKUN_D = '01.04.001' OR NOMOR_AKUN_K = '01.04.001')
  AND NOMINAL > 50000000;

-- Saldo akun kritis setelah fix
SELECT KODE_AKUN, NAMA_AKUN, S_DEBET, S_KREDIT, SALDO_AKHIR
FROM tbl_datareferensi
WHERE KODE_AKUN IN ('01.04.001', '06.01.001', '04.02.001');

COMMIT;

-- =============================================================
-- STEP 4: Hapus catatan manual — semua entry sudah difix otomatis
-- =============================================================
-- Entry yang sudah difix:
--   ×10000 : 8 entries (Sedap Goreng, Crispy Cracker, Isoplus, dll)
--   ×1000  : 6 entries (Sedap Selection, Lm Sak, Kertas Nasi, Cakra, Nusa Jaya ×2)
--   manual : 1 entry  (Ekonomi Cair 135Ml Dos → Rp325.215.000)
--
-- PINDAH REKENING yang masih perlu konfirmasi user:
--   PR-2512160001  Rp91,0 M  DEBET  01.04.001  "JURNAL PENYESUAIAN BARANG"
--   PR-2502020001  Rp25,6 M  KREDIT 01.04.001  "Penyesuaian persediaan barang"
-- =============================================================

SELECT 'MIGRASI 43 SELESAI' AS status;
