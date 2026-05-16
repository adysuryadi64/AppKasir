-- ============================================================
-- Test Real: 4 Pembelian + 4 Penjualan — Verifikasi logika VB
-- Semua data dari DB real, prefix TST- untuk cleanup mudah
-- Kolom disesuaikan dengan skema db_kasirlancar aktual
-- ============================================================
USE db_kasirlancar;

-- ============================================================
-- STEP 0: Ambil data real & snapshot kondisi awal
-- ============================================================
SELECT @id_barang    := ID_BARANG,
       @nama_barang  := NAMA_BARANG,
       @harga_beli0  := HARGA_BELI,
       @harga_jual0  := HARGA_JUAL_UMUM_KECIL,
       @stok_awal    := STOK_TOKO,
       @kode_rek_brg := KODE_REK_BARANG
FROM tbl_barang
WHERE STOK_TOKO >= 15 AND HARGA_BELI > 0 AND HARGA_JUAL_UMUM_KECIL > 0
ORDER BY STOK_TOKO DESC LIMIT 1;

SELECT @id_supplier  := Kode, @nama_supplier := Nama FROM tbl_supliyer LIMIT 1;
SELECT @id_pelanggan := KODE, @nama_pelanggan := NAMA FROM tbl_pelanggan LIMIT 1;

SELECT @akun_kas          := KODE_AKUN, @nama_akun_kas          := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '01.01%' LIMIT 1;
SELECT @akun_persediaan   := KODE_AKUN, @nama_akun_persediaan   := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '01.04%' LIMIT 1;
SELECT @akun_piutang      := KODE_AKUN, @nama_akun_piutang      := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '01.03%' LIMIT 1;
SELECT @akun_hutang       := KODE_AKUN, @nama_akun_hutang       := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '03.01%' LIMIT 1;
SELECT @akun_penjualan    := KODE_AKUN, @nama_akun_penjualan    := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '05.02%' LIMIT 1;
SELECT @akun_hpp          := KODE_AKUN, @nama_akun_hpp          := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '06.01%' LIMIT 1;
SELECT @akun_penyesuaian  := KODE_AKUN, @nama_akun_penyesuaian  := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '06.04%' LIMIT 1;
SELECT @akun_diskon_jual  := KODE_AKUN, @nama_akun_diskon_jual  := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '05.04%' LIMIT 1;
SELECT @akun_diskon_beli  := KODE_AKUN, @nama_akun_diskon_beli  := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '06.05%' LIMIT 1;
SELECT @akun_ppn          := KODE_AKUN, @nama_akun_ppn          := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '03.02%' LIMIT 1;
SELECT @akun_biaya_kirim  := KODE_AKUN, @nama_akun_biaya_kirim  := NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN LIKE '06.02%' LIMIT 1;

-- Harga test
SET @harga_beli_pb1 = @harga_beli0;
SET @harga_beli_pb2 = ROUND(@harga_beli0 * 1.05, 0);
SET @harga_beli_pb3 = ROUND(@harga_beli0 * 0.97, 0);
SET @harga_beli_pb4 = @harga_beli0;
SET @harga_jual_pj  = @harga_jual0;

-- Snapshot awal
SELECT @stok_toko_awal           := STOK_TOKO,
       @pembelian_toko_awal      := PEMBELIAN_TOKO,
       @penjualan_toko_awal      := PENJUALAN_TOKO,
       @harga_beli_awal          := HARGA_BELI,
       @harga_beli_terakhir_awal := HARGA_BELI_TERAKHIR
FROM tbl_barang WHERE ID_BARANG = @id_barang;

SELECT @hutang_supplier_awal   := HutangAkhir FROM tbl_supliyer  WHERE Kode = @id_supplier;
SELECT @piutang_pelanggan_awal := HutangAkhir FROM tbl_pelanggan WHERE KODE = @id_pelanggan;

SELECT '=== DATA REAL YANG DIPAKAI ===' AS status;
SELECT @id_barang AS barang, @nama_barang AS nama,
       @harga_beli0 AS harga_beli_awal, @harga_jual0 AS harga_jual,
       @stok_awal AS stok_awal, @id_supplier AS supplier, @id_pelanggan AS pelanggan;

SELECT IF(@id_barang IS NULL OR @id_supplier IS NULL OR @id_pelanggan IS NULL,
    'STOP: Data tidak lengkap', 'OK: Data real tersedia') AS validasi_awal;

-- Bersihkan sisa test lama
DELETE FROM HistoryBarang WHERE FAKTUR LIKE 'TST-%';
DELETE FROM JurnalUmum WHERE NO_TRANSAKSI LIKE 'TST-%';
DELETE FROM penjualan_detail WHERE FAKTUR_JUAL LIKE 'TST-%';
DELETE FROM penjualan WHERE ID_PENJUALAN LIKE 'TST-%';
DELETE FROM pembelian_detail WHERE FAKTUR_BELI LIKE 'TST-%';
DELETE FROM pembelian WHERE ID_PEMBELIAN LIKE 'TST-%';

SELECT '=== KONDISI AWAL ===' AS status;
SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, STOK_TOKO FROM tbl_barang WHERE ID_BARANG = @id_barang;
-- ============================================================
-- PB-1: Tunai, harga sama — average = harga beli
-- ============================================================
SET @qty_pb1   = 10;
SET @total_pb1 = @harga_beli_pb1 * @qty_pb1;
SET @avg_pb1   = ROUND((@harga_beli0 * @stok_awal + @harga_beli_pb1 * @qty_pb1) / (@stok_awal + @qty_pb1), 0);

SELECT CONCAT('=== PB-1: Tunai ', @qty_pb1, ' pcs @ ', @harga_beli_pb1, ' = ', @total_pb1, ' ===') AS status;

INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI,
  JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, TAGIHAN, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-001', @id_supplier, @nama_supplier, 'NOTA-TST-001',
  '2026-04-20 09:00:00', 'TOKO', @nama_akun_kas,
  @total_pb1, @qty_pb1, 1, @total_pb1, 0, 'LUNAS', 'admin', 'PC01');

INSERT INTO pembelian_detail (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER,
  ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA,
  QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-001', 'NOTA-TST-001', '2026-04-20 09:00:00', 'TOKO', @id_supplier, @nama_supplier,
  @id_barang, @nama_barang, @harga_beli_pb1, @avg_pb1, @harga_beli0,
  @qty_pb1, 'PCS', 1, @harga_beli_pb1, @qty_pb1, @total_pb1, 'admin', 'PC01');

INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
  QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-001', '2026-04-20 09:00:00', 'PEMBELIAN', 'TOKO',
  @id_barang, @nama_barang, @qty_pb1, 'PCS', 1, @qty_pb1, @total_pb1, 'admin', 'PC01');

UPDATE tbl_barang SET PEMBELIAN_TOKO = PEMBELIAN_TOKO + @qty_pb1,
  HARGA_BELI = @avg_pb1, HARGA_BELI_TERAKHIR = @harga_beli_pb1,
  KODE_SUPLIYER = @id_supplier, NAMA_SUPLIYER = @nama_supplier
WHERE ID_BARANG = @id_barang;
CALL sp_hlp_stok_hitung(@id_barang);

INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN,
  NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
  NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-001', '2026-04-20 09:00:00', 'NOTA-TST-001', CONCAT('PB-1 Tunai: ', @nama_barang),
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_kas, @akun_kas,
  @total_pb1, 'Pembelian', 'TOKO', 'admin', 'PC01');

CALL sp_hlp_saldo_akun_update(@akun_persediaan);
CALL sp_hlp_saldo_akun_update(@akun_kas);
CALL sp_bat_hutang_semua_supplier();

SELECT 'Verifikasi PB-1:' AS status;
SELECT ID_BARANG, STOK_TOKO, PEMBELIAN_TOKO, HARGA_BELI, HARGA_BELI_TERAKHIR,
  @avg_pb1 AS EXPECTED_AVG,
  IF(ABS(HARGA_BELI - @avg_pb1) < 1, 'PASS: average benar', CONCAT('FAIL: got=', HARGA_BELI)) AS cek_avg,
  IF(HARGA_BELI_TERAKHIR = @harga_beli_pb1, 'PASS: terakhir benar', 'FAIL') AS cek_terakhir
FROM tbl_barang WHERE ID_BARANG = @id_barang;
SELECT IF(COUNT(*)=1,'PASS: header pembelian','FAIL') AS cek FROM pembelian WHERE ID_PEMBELIAN='TST-PB-001';
SELECT IF(COUNT(*)=1,'PASS: detail pembelian','FAIL') AS cek FROM pembelian_detail WHERE FAKTUR_BELI='TST-PB-001';
SELECT IF(COUNT(*)=1,'PASS: HistoryBarang','FAIL') AS cek FROM HistoryBarang WHERE FAKTUR='TST-PB-001' AND JENIS='PEMBELIAN';
SELECT IF(COUNT(*)=1,'PASS: Jurnal D PERSEDIAAN/K KAS','FAIL') AS cek FROM JurnalUmum WHERE NO_TRANSAKSI='TST-PB-001' AND NOMOR_AKUN_D=@akun_persediaan AND NOMOR_AKUN_K=@akun_kas;

-- ============================================================
-- PB-2: Kredit, harga NAIK 5% — ada selisih HPP average
-- ============================================================
SET @qty_pb2        = 5;
SET @total_pb2      = @harga_beli_pb2 * @qty_pb2;
SET @stok_sblm_pb2  = (SELECT STOK_TOKO FROM tbl_barang WHERE ID_BARANG = @id_barang);
SET @harga_sblm_pb2 = (SELECT HARGA_BELI FROM tbl_barang WHERE ID_BARANG = @id_barang);
SET @avg_pb2        = ROUND((@harga_sblm_pb2 * @stok_sblm_pb2 + @harga_beli_pb2 * @qty_pb2) / (@stok_sblm_pb2 + @qty_pb2), 0);
SET @selisih_pb2    = ROUND((@avg_pb2 - @harga_sblm_pb2) * @stok_sblm_pb2, 0);

SELECT CONCAT('=== PB-2: Kredit ', @qty_pb2, ' pcs @ ', @harga_beli_pb2, ' avg=', @avg_pb2, ' selisih=', @selisih_pb2, ' ===') AS status;

INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI,
  JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-002', @id_supplier, @nama_supplier, 'NOTA-TST-002',
  '2026-04-20 10:00:00', 'TOKO', 'HUTANG',
  @total_pb2, @qty_pb2, 1, 0, @total_pb2, '2026-05-20', 'BELUM LUNAS', 'admin', 'PC01');

INSERT INTO pembelian_detail (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER,
  ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA,
  QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-002', 'NOTA-TST-002', '2026-04-20 10:00:00', 'TOKO', @id_supplier, @nama_supplier,
  @id_barang, @nama_barang, @harga_beli_pb2, @avg_pb2, @harga_sblm_pb2,
  @qty_pb2, 'PCS', 1, @harga_beli_pb2, @qty_pb2, @total_pb2, 'admin', 'PC01');

INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
  QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-002', '2026-04-20 10:00:00', 'PEMBELIAN', 'TOKO',
  @id_barang, @nama_barang, @qty_pb2, 'PCS', 1, @qty_pb2, @total_pb2, 'admin', 'PC01');

UPDATE tbl_barang SET PEMBELIAN_TOKO = PEMBELIAN_TOKO + @qty_pb2,
  HARGA_BELI = @avg_pb2, HARGA_BELI_TERAKHIR = @harga_beli_pb2,
  KODE_SUPLIYER = @id_supplier, NAMA_SUPLIYER = @nama_supplier
WHERE ID_BARANG = @id_barang;
CALL sp_hlp_stok_hitung(@id_barang);

INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN,
  NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
  NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-002', '2026-04-20 10:00:00', 'NOTA-TST-002', CONCAT('PB-2 Kredit: ', @nama_barang),
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_hutang, @akun_hutang,
  @total_pb2, 'Pembelian', 'TOKO', 'admin', 'PC01');

INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN,
  NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
  NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-002-HPP', '2026-04-20 10:00:00', 'NOTA-TST-002', CONCAT('PB-2 Selisih HPP: ', @nama_barang),
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_penyesuaian, @akun_penyesuaian,
  @selisih_pb2, 'Pembelian', 'TOKO', 'admin', 'PC01');

CALL sp_hlp_saldo_akun_update(@akun_persediaan);
CALL sp_hlp_saldo_akun_update(@akun_hutang);
CALL sp_hlp_saldo_akun_update(@akun_penyesuaian);
CALL sp_bat_hutang_semua_supplier();

SELECT 'Verifikasi PB-2:' AS status;
SELECT ID_BARANG, STOK_TOKO, HARGA_BELI, @avg_pb2 AS EXPECTED_AVG,
  IF(ABS(HARGA_BELI - @avg_pb2) < 1, 'PASS: average naik benar', CONCAT('FAIL: got=', HARGA_BELI)) AS cek_avg
FROM tbl_barang WHERE ID_BARANG = @id_barang;
SELECT IF(COUNT(*)=1,'PASS: Jurnal D PERSEDIAAN/K HUTANG','FAIL') AS cek FROM JurnalUmum WHERE NO_TRANSAKSI='TST-PB-002' AND NOMOR_AKUN_D=@akun_persediaan AND NOMOR_AKUN_K=@akun_hutang;
SELECT IF(@selisih_pb2>0, IF(COUNT(*)=1,'PASS: Jurnal selisih HPP ada','FAIL'), 'SKIP: selisih=0') AS cek FROM JurnalUmum WHERE NO_TRANSAKSI='TST-PB-002-HPP';
SELECT IF(HutangAkhir > @hutang_supplier_awal, CONCAT('PASS: HutangAkhir naik=',HutangAkhir), CONCAT('FAIL: HutangAkhir=',HutangAkhir)) AS cek FROM tbl_supliyer WHERE Kode=@id_supplier;

-- ============================================================
-- PB-3: Kredit + DISKON SUPPLIER + BIAYA KIRIM, harga TURUN 3%
-- ============================================================
SET @qty_pb3      = 8;
SET @total_pb3    = @harga_beli_pb3 * @qty_pb3;
SET @diskon_pb3   = ROUND(@total_pb3 * 0.02, 0);
SET @biaya_pb3    = ROUND(@harga_beli_pb3 * 0.5, 0);
SET @grand_pb3    = @total_pb3 - @diskon_pb3 + @biaya_pb3;
SET @stok_sblm_pb3  = (SELECT STOK_TOKO FROM tbl_barang WHERE ID_BARANG = @id_barang);
SET @harga_sblm_pb3 = (SELECT HARGA_BELI FROM tbl_barang WHERE ID_BARANG = @id_barang);
SET @avg_pb3        = ROUND((@harga_sblm_pb3 * @stok_sblm_pb3 + @harga_beli_pb3 * @qty_pb3) / (@stok_sblm_pb3 + @qty_pb3), 0);
SET @selisih_pb3    = ROUND((@avg_pb3 - @harga_sblm_pb3) * @stok_sblm_pb3, 0);

SELECT CONCAT('=== PB-3: Kredit+Diskon+BiayaKirim ', @qty_pb3, ' pcs @ ', @harga_beli_pb3, ' avg=', @avg_pb3, ' ===') AS status;

INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI,
  JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, TAGIHAN, JATUH_TEMPO,
  STATUS_TRANSAKSI_BELI, DISKON_SUPPLIER, BIAYA_KIRIM, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003', @id_supplier, @nama_supplier, 'NOTA-TST-003',
  '2026-04-20 11:00:00', 'TOKO', 'HUTANG',
  @grand_pb3, @qty_pb3, 1, 0, @grand_pb3, '2026-05-20',
  'BELUM LUNAS', @diskon_pb3, @biaya_pb3, 'admin', 'PC01');

INSERT INTO pembelian_detail (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER,
  ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA,
  QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003', 'NOTA-TST-003', '2026-04-20 11:00:00', 'TOKO', @id_supplier, @nama_supplier,
  @id_barang, @nama_barang, @harga_beli_pb3, @avg_pb3, @harga_sblm_pb3,
  @qty_pb3, 'PCS', 1, @harga_beli_pb3, @qty_pb3, @total_pb3, 'admin', 'PC01');

INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
  QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003', '2026-04-20 11:00:00', 'PEMBELIAN', 'TOKO',
  @id_barang, @nama_barang, @qty_pb3, 'PCS', 1, @qty_pb3, @total_pb3, 'admin', 'PC01');

UPDATE tbl_barang SET PEMBELIAN_TOKO = PEMBELIAN_TOKO + @qty_pb3,
  HARGA_BELI = @avg_pb3, HARGA_BELI_TERAKHIR = @harga_beli_pb3,
  KODE_SUPLIYER = @id_supplier, NAMA_SUPLIYER = @nama_supplier
WHERE ID_BARANG = @id_barang;
CALL sp_hlp_stok_hitung(@id_barang);

-- D PERSEDIAAN / K HUTANG
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003', '2026-04-20 11:00:00', 'NOTA-TST-003', CONCAT('PB-3 Kredit: ', @nama_barang),
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_hutang, @akun_hutang, @total_pb3, 'Pembelian', 'TOKO', 'admin', 'PC01');
-- D PERSEDIAAN / K PENYESUAIAN (selisih HPP)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003-HPP', '2026-04-20 11:00:00', 'NOTA-TST-003', 'PB-3 Selisih HPP',
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_penyesuaian, @akun_penyesuaian, ABS(@selisih_pb3), 'Pembelian', 'TOKO', 'admin', 'PC01');
-- D KAS / K DISKON PEMBELIAN
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003-DSK', '2026-04-20 11:00:00', 'NOTA-TST-003', 'PB-3 Diskon supplier',
  @nama_akun_kas, @akun_kas, @nama_akun_diskon_beli, @akun_diskon_beli, @diskon_pb3, 'Pembelian', 'TOKO', 'admin', 'PC01');
-- D BIAYA KIRIM / K KAS
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-003-BK', '2026-04-20 11:00:00', 'NOTA-TST-003', 'PB-3 Biaya kirim',
  @nama_akun_biaya_kirim, @akun_biaya_kirim, @nama_akun_kas, @akun_kas, @biaya_pb3, 'Pembelian', 'TOKO', 'admin', 'PC01');

CALL sp_hlp_saldo_akun_update(@akun_persediaan);
CALL sp_hlp_saldo_akun_update(@akun_hutang);
CALL sp_hlp_saldo_akun_update(@akun_penyesuaian);
CALL sp_hlp_saldo_akun_update(@akun_kas);
CALL sp_hlp_saldo_akun_update(@akun_diskon_beli);
CALL sp_hlp_saldo_akun_update(@akun_biaya_kirim);
CALL sp_bat_hutang_semua_supplier();

SELECT 'Verifikasi PB-3:' AS status;
SELECT ID_BARANG, HARGA_BELI, @avg_pb3 AS EXPECTED_AVG,
  IF(ABS(HARGA_BELI - @avg_pb3) < 1, 'PASS: average turun benar', CONCAT('FAIL: got=', HARGA_BELI)) AS cek_avg
FROM tbl_barang WHERE ID_BARANG = @id_barang;
SELECT IF(COUNT(*)=1,'PASS: Jurnal diskon supplier','FAIL') AS cek FROM JurnalUmum WHERE NO_TRANSAKSI='TST-PB-003-DSK' AND NOMOR_AKUN_D=@akun_kas AND NOMOR_AKUN_K=@akun_diskon_beli;
SELECT IF(COUNT(*)=1,'PASS: Jurnal biaya kirim','FAIL') AS cek FROM JurnalUmum WHERE NO_TRANSAKSI='TST-PB-003-BK' AND NOMOR_AKUN_D=@akun_biaya_kirim;

-- ============================================================
-- PB-4: Tunai + Transfer (split payment), harga sama
-- ============================================================
SET @qty_pb4             = 6;
SET @total_pb4           = @harga_beli_pb4 * @qty_pb4;
SET @bayar_tunai_pb4     = ROUND(@total_pb4 * 0.6, 0);
SET @bayar_transfer_pb4  = @total_pb4 - @bayar_tunai_pb4;
SET @stok_sblm_pb4       = (SELECT STOK_TOKO FROM tbl_barang WHERE ID_BARANG = @id_barang);
SET @harga_sblm_pb4      = (SELECT HARGA_BELI FROM tbl_barang WHERE ID_BARANG = @id_barang);
SET @avg_pb4             = ROUND((@harga_sblm_pb4 * @stok_sblm_pb4 + @harga_beli_pb4 * @qty_pb4) / (@stok_sblm_pb4 + @qty_pb4), 0);

SELECT @akun_bank := KODE_AKUN, @nama_akun_bank := NAMA_AKUN
FROM tbl_datareferensi WHERE JENIS_AKUN = 'ASET LANCAR' AND NAMA_AKUN LIKE '%BANK%' LIMIT 1;
SET @akun_bank      = COALESCE(@akun_bank, @akun_kas);
SET @nama_akun_bank = COALESCE(@nama_akun_bank, @nama_akun_kas);

SELECT CONCAT('=== PB-4: Tunai+Transfer ', @qty_pb4, ' pcs @ ', @harga_beli_pb4, ' tunai=', @bayar_tunai_pb4, ' tf=', @bayar_transfer_pb4, ' ===') AS status;

INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI,
  JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, NOMINAL_TRANSFER, TAGIHAN, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-004', @id_supplier, @nama_supplier, 'NOTA-TST-004',
  '2026-04-20 12:00:00', 'TOKO', @nama_akun_kas,
  @total_pb4, @qty_pb4, 1, @bayar_tunai_pb4, @bayar_transfer_pb4, 0, 'LUNAS', 'admin', 'PC01');

INSERT INTO pembelian_detail (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER,
  ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA,
  QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-004', 'NOTA-TST-004', '2026-04-20 12:00:00', 'TOKO', @id_supplier, @nama_supplier,
  @id_barang, @nama_barang, @harga_beli_pb4, @avg_pb4, @harga_sblm_pb4,
  @qty_pb4, 'PCS', 1, @harga_beli_pb4, @qty_pb4, @total_pb4, 'admin', 'PC01');

INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
  QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-004', '2026-04-20 12:00:00', 'PEMBELIAN', 'TOKO',
  @id_barang, @nama_barang, @qty_pb4, 'PCS', 1, @qty_pb4, @total_pb4, 'admin', 'PC01');

UPDATE tbl_barang SET PEMBELIAN_TOKO = PEMBELIAN_TOKO + @qty_pb4,
  HARGA_BELI = @avg_pb4, HARGA_BELI_TERAKHIR = @harga_beli_pb4,
  KODE_SUPLIYER = @id_supplier, NAMA_SUPLIYER = @nama_supplier
WHERE ID_BARANG = @id_barang;
CALL sp_hlp_stok_hitung(@id_barang);

-- D PERSEDIAAN / K KAS (tunai)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-004', '2026-04-20 12:00:00', 'NOTA-TST-004', CONCAT('PB-4 Tunai: ', @nama_barang),
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_kas, @akun_kas, @bayar_tunai_pb4, 'Pembelian', 'TOKO', 'admin', 'PC01');
-- D PERSEDIAAN / K BANK (transfer)
INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
VALUES ('TST-PB-004-TF', '2026-04-20 12:00:00', 'NOTA-TST-004', CONCAT('PB-4 Transfer: ', @nama_barang),
  @nama_akun_persediaan, @akun_persediaan, @nama_akun_bank, @akun_bank, @bayar_transfer_pb4, 'Pembelian', 'TOKO', 'admin', 'PC01');

CALL sp_hlp_saldo_akun_update(@akun_persediaan);
CALL sp_hlp_saldo_akun_update(@akun_kas);
CALL sp_hlp_saldo_akun_update(@akun_bank);
CALL sp_bat_hutang_semua_supplier();

SELECT 'Verifikasi PB-4:' AS status;
SELECT ID_BARANG, HARGA_BELI, @avg_pb4 AS EXPECTED_AVG,
  IF(ABS(HARGA_BELI - @avg_pb4) < 1, 'PASS: average benar', CONCAT('FAIL: got=', HARGA_BELI)) AS cek_avg
FROM tbl_barang WHERE ID_BARANG = @id_barang;
SELECT IF(COUNT(*)=2,'PASS: 2 jurnal split payment','FAIL') AS cek FROM JurnalUmum WHERE NO_TRANSAKSI IN ('TST-PB-004','TST-PB-004-TF') AND NOMOR_AKUN_D=@akun_persediaan;

SELECT '=== RINGKASAN 4 TEST PEMBELIAN ===' AS status;
SELECT p.ID_PEMBELIAN, p.STATUS_TRANSAKSI_BELI, p.GRAND_TOTAL_BELI,
  COUNT(DISTINCT pd.FAKTUR_BELI) AS ada_detail,
  COUNT(DISTINCT h.FAKTUR) AS ada_history,
  COUNT(DISTINCT j.NO) AS jml_jurnal
FROM pembelian p
LEFT JOIN pembelian_detail pd ON pd.FAKTUR_BELI = p.ID_PEMBELIAN
LEFT JOIN HistoryBarang h ON h.FAKTUR = p.ID_PEMBELIAN AND h.JENIS='PEMBELIAN'
LEFT JOIN JurnalUmum j ON j.NO_TRANSAKSI LIKE CONCAT(p.ID_PEMBELIAN,'%')
WHERE p.ID_PEMBELIAN LIKE 'TST-PB-%'
GROUP BY p.ID_PEMBELIAN, p.STATUS_TRANSAKSI_BELI, p.GRAND_TOTAL_BELI;
