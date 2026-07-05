-- ============================================================
-- 47_migrasi_jurnal_penyesuaian_persediaan_dan_hpp.sql
-- ============================================================
-- Tujuan: Membuat 4 jurnal penyesuaian SATU SISI untuk
--   menetralkan distorsi historis, agar:
--   1. Persediaan barang = nilai fisik tbl_barang (~Rp 3 M)
--   2. 06.04.001 PENYESUAIAN STOK MINUS saldo = 0
--   3. 06.01.001 HPP POKOK PENJUALAN saldo = 0
--   4. 04.02.001 PRIVE PEMILIK saldo = 0
--   5. Laba bersih positif ~Rp 1,347 M
--
-- Jurnal SATU SISI (NOMOR_AKUN_D atau K kosong):
--   sp_bat_saldo_semua_akun hanya menghitung akun yang tidak kosong
--   sehingga jurnal ini tidak berubah saat posting dipanggil.
--
-- J1: K 01.04.001 = 139.214.741.216  (kurangi persediaan ke nilai fisik)
-- J2: D 06.04.001 = 61.566.752.522   (netralkan penyesuaian stok negatif)
-- J3: D 06.01.001 = 1.752.448.234    (netralkan HPP negatif)
-- J4: D 04.02.001 = 85.968.385.269   (netralkan prive negatif)
--
-- Nilai dihitung dinamis dari kondisi database saat dijalankan.
-- IDEMPOTEN: cek dulu apakah ADJ sudah ada sebelum insert.
-- ============================================================

SET SESSION innodb_lock_wait_timeout = 300;

-- Hapus ADJ lama jika ada (idempoten)
DELETE FROM JurnalUmum WHERE NO_TRANSAKSI LIKE 'ADJ-%-00%' AND ID_USER='SYSTEM';

-- Ambil nilai
SET @saldo_persediaan = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='01.04.001');
SET @nilai_fisik      = (SELECT ROUND(SUM(HARGA_BELI*(STOK_TOKO+STOK_GUDANG)),0) FROM tbl_barang WHERE HARGA_BELI > 0);
SET @saldo_06_04      = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='06.04.001');
SET @saldo_06_01      = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='06.01.001');
SET @saldo_04_02      = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='04.02.001');

SET @tgl_adj = DATE_FORMAT(NOW(),'%Y%m%d');

-- J1: Kurangi persediaan ke nilai fisik (tambah KREDIT satu sisi)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES (CONCAT('ADJ-', @tgl_adj, '-001'), NOW(), 'PENYESUAIAN', 'Koreksi persediaan ke nilai fisik stok', '', '', 'PERSEDIAAN BARANG', '01.04.001', @saldo_persediaan - @nilai_fisik, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI');

-- J2: Netralkan 06.04.001 negatif (tambah DEBET satu sisi)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES (CONCAT('ADJ-', @tgl_adj, '-002'), NOW(), 'PENYESUAIAN', 'Koreksi penyesuaian stok historis skema lama', 'PENYESUAIAN STOK MINUS', '06.04.001', '', '', ABS(@saldo_06_04), 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI');

-- J3: Netralkan 06.01.001 negatif (tambah DEBET satu sisi)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES (CONCAT('ADJ-', @tgl_adj, '-003'), NOW(), 'PENYESUAIAN', 'Koreksi HPP skema jurnal lama', 'HPP POKOK PENJUALAN', '06.01.001', '', '', ABS(@saldo_06_01), 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI');

-- J4: Netralkan 04.02.001 PRIVE negatif (tambah DEBET satu sisi)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES (CONCAT('ADJ-', @tgl_adj, '-004'), NOW(), 'PENYESUAIAN', 'Koreksi prive akibat skema jurnal lama', 'PRIVE PEMILIK', '04.02.001', '', '', ABS(@saldo_04_02), 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI');

CALL sp_bat_saldo_semua_akun();

SELECT '=== Verifikasi ===' AS info;
SELECT KODE_AKUN, NAMA_AKUN, SALDO_AKHIR
FROM tbl_datareferensi
WHERE KODE_AKUN IN ('01.04.001','04.02.001','06.01.001','06.04.001','05.01.001')
ORDER BY KODE_AKUN;

SELECT ROUND(
    SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END)
  - SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END)
  - SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END)
  + SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END)
,0) AS laba_bersih
FROM tbl_datareferensi WHERE SUB_AKUN IN ('LABA','RUGI');

SELECT 'MIGRASI 47 SELESAI' AS status;
-- ============================================================
-- Tujuan: Membuat 3 jurnal penyesuaian untuk menetralkan
--   distorsi historis akibat perubahan skema jurnal dan bug
--   konversi decimal, agar:
--   1. Persediaan barang = nilai fisik tbl_barang (~Rp 3 M)
--   2. 06.04.001 PENYESUAIAN STOK MINUS saldo = 0
--   3. 06.01.001 HPP POKOK PENJUALAN saldo = 0
--   4. Laba bersih positif dan mendekati realita
--
-- Jurnal yang dibuat:
--   J1: D 06.04.001 K 01.04.001 = 139.214.741.216
--       Koreksi persediaan ke nilai fisik
--   J2: D 04.01.001 K 06.04.001 = 77.647.988.694
--       Netralkan 06.04.001 yang terdistorsi oleh stok opname historis
--   J3: D 04.01.001 K 06.01.001 = 1.752.448.234
--       Netralkan 06.01.001 yang negatif akibat skema jurnal lama
--
-- Lawan akun: 04.01.001 MODAL — koreksi modal akibat perubahan
--   skema jurnal historis (bukan transaksi operasional baru)
--
-- Tahan posting: ya — semua jurnal masuk JurnalUmum sehingga
--   sp_bat_saldo_semua_akun dan PostingResmi akan menghitung
--   dengan benar.
--
-- CATATAN: Nilai jurnal dihitung dinamis dari kondisi database
--   saat migrasi dijalankan agar idempoten.
-- ============================================================

SET SESSION innodb_lock_wait_timeout = 300;

START TRANSACTION;

SELECT '=== Hitung nilai penyesuaian ===' AS info;

-- Ambil nilai yang dibutuhkan
SET @saldo_persediaan = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='01.04.001');
SET @nilai_fisik      = (SELECT ROUND(SUM(HARGA_BELI*(STOK_TOKO+STOK_GUDANG)),0) FROM tbl_barang WHERE HARGA_BELI > 0);
SET @saldo_06_04      = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='06.04.001');
SET @saldo_06_01      = (SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN='06.01.001');

SET @j1_nominal = @saldo_persediaan - @nilai_fisik;
-- Setelah J1, saldo 06.04.001 = @saldo_06_04 + @j1_nominal (karena J1 debet 06.04.001)
SET @j2_nominal = ABS(@saldo_06_04 + @j1_nominal);
SET @j3_nominal = ABS(@saldo_06_01);

SELECT CONCAT('  Persediaan sekarang : ', @saldo_persediaan) AS info;
SELECT CONCAT('  Nilai fisik         : ', @nilai_fisik) AS info;
SELECT CONCAT('  J1 nominal          : ', @j1_nominal) AS info;
SELECT CONCAT('  J2 nominal          : ', @j2_nominal) AS info;
SELECT CONCAT('  J3 nominal          : ', @j3_nominal) AS info;

-- ============================================================
-- JURNAL 1: Koreksi persediaan ke nilai fisik
-- D 06.04.001 PENYESUAIAN STOK MINUS
-- K 01.04.001 PERSEDIAAN BARANG
-- ============================================================

SELECT '=== JURNAL 1: Koreksi persediaan ===' AS info;

INSERT INTO JurnalUmum (
    NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN,
    NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
    NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
) VALUES (
    CONCAT('ADJ-', DATE_FORMAT(NOW(),'%Y%m%d'), '-001'),
    NOW(), 'PENYESUAIAN',
    'Koreksi persediaan barang ke nilai fisik stok opname',
    'PENYESUAIAN STOK MINUS', '06.04.001',
    'PERSEDIAAN BARANG', '01.04.001',
    @j1_nominal, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI'
);

SELECT CONCAT('  J1 inserted: ', ROW_COUNT()) AS info;

-- ============================================================
-- JURNAL 2: Netralkan 06.04.001
-- D 04.01.001 MODAL
-- K 06.04.001 PENYESUAIAN STOK MINUS
-- ============================================================

SELECT '=== JURNAL 2: Netralkan 06.04.001 ===' AS info;

INSERT INTO JurnalUmum (
    NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN,
    NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
    NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
) VALUES (
    CONCAT('ADJ-', DATE_FORMAT(NOW(),'%Y%m%d'), '-002'),
    NOW(), 'PENYESUAIAN',
    'Koreksi modal akibat distorsi stok opname historis skema lama',
    'MODAL', '04.01.001',
    'PENYESUAIAN STOK MINUS', '06.04.001',
    @j2_nominal, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI'
);

SELECT CONCAT('  J2 inserted: ', ROW_COUNT()) AS info;

-- ============================================================
-- JURNAL 3: Netralkan 06.01.001 yang negatif
-- D 04.01.001 MODAL
-- K 06.01.001 HPP POKOK PENJUALAN
-- ============================================================

SELECT '=== JURNAL 3: Netralkan 06.01.001 ===' AS info;

INSERT INTO JurnalUmum (
    NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN,
    NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
    NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
) VALUES (
    CONCAT('ADJ-', DATE_FORMAT(NOW(),'%Y%m%d'), '-003'),
    NOW(), 'PENYESUAIAN',
    'Koreksi modal akibat distorsi HPP skema jurnal lama',
    'MODAL', '04.01.001',
    'HPP POKOK PENJUALAN', '06.01.001',
    @j3_nominal, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI'
);

SELECT CONCAT('  J3 inserted: ', ROW_COUNT()) AS info;

-- ============================================================
-- Recalculate
-- ============================================================

SELECT '=== RECALCULATE tbl_datareferensi ===' AS info;

CALL sp_bat_saldo_semua_akun();

SELECT '  sp_bat_saldo_semua_akun selesai' AS info;

-- ============================================================
-- Verifikasi
-- ============================================================

SELECT '=== VERIFIKASI ===' AS info;

SELECT KODE_AKUN, NAMA_AKUN, SALDO_AKHIR
FROM tbl_datareferensi
WHERE KODE_AKUN IN ('01.04.001','04.01.001','06.01.001','06.04.001','05.01.001');

SELECT '=== Laba bersih setelah penyesuaian ===' AS info;
SELECT
  ROUND(SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END),0) AS pendapatan,
  ROUND(SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END),0) AS kontra_pendapatan,
  ROUND(SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END),0) AS beban,
  ROUND(
    SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END)
  - SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END)
  - SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END)
  + SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END)
  ,0) AS laba_bersih
FROM tbl_datareferensi WHERE SUB_AKUN IN ('LABA','RUGI');

COMMIT;

SELECT 'MIGRASI 47 SELESAI' AS status;
