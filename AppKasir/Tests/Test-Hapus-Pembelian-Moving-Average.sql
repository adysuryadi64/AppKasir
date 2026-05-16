-- =============================================================================
-- Test Hapus Pembelian dengan Moving Average Recalculate
-- =============================================================================

-- Test case: Hapus pembelian dan verifikasi moving average di-recalculate dengan benar
-- Skenario: 3 pembelian berturut-turut, lalu hapus yang tengah

USE db_kasirlancar;

-- Cleanup data test
DELETE FROM pembelian_detail WHERE FAKTUR_BELI LIKE 'TST-HPP-%';
DELETE FROM pembelian WHERE ID_PEMBELIAN LIKE 'TST-HPP-%';
DELETE FROM JurnalUmum WHERE NO_TRANSAKSI LIKE 'TST-HPP-%';
DELETE FROM tbl_barang WHERE ID_BARANG = 'TST-HPP-001';

-- Setup data barang test
INSERT INTO tbl_barang (ID_BARANG, NAMA_BARANG, HARGA_BELI, STOK_TOKO, PEMBELIAN_TOKO)
VALUES ('TST-HPP-001', 'Test Moving Average', 0, 0, 0);

-- Variables
SET @id_barang = 'TST-HPP-001';
SET @id_supplier = 'SUP001';
SET @nama_supplier = 'Supplier Test';
SET @lokasi = 'TOKO';

-- =============================================================================
-- STEP 1: Buat 3 pembelian berturut-turut
-- =============================================================================

-- Pembelian 1: 100 unit @ 10.000 = 1.000.000
INSERT INTO pembelian (ID_PEMBELIAN, TGL_BELI, ID_SUPPLIER, NAMA_SUPLIYER, LOKASI, GRAND_TOTAL_BELI, STATUS_TRANSAKSI_BELI)
VALUES ('TST-HPP-PB1', '2026-04-27 09:00:00', @id_supplier, @nama_supplier, @lokasi, 1000000, 'LUNAS');

INSERT INTO pembelian_detail (FAKTUR_BELI, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY_SAT, TOTAL, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER)
VALUES ('TST-HPP-PB1', @id_barang, 'Test Moving Average', 10000, 100, 1000000, @lokasi, @id_supplier, @nama_supplier);

UPDATE tbl_barang SET 
    PEMBELIAN_TOKO = PEMBELIAN_TOKO + 100,
    HARGA_BELI = 10000,
    HARGA_BELI_TERAKHIR = 10000
WHERE ID_BARANG = @id_barang;

CALL sp_hlp_stok_hitung(@id_barang);

-- Pembelian 2: 100 unit @ 12.000 = 1.200.000
-- Moving average: (10000*100 + 12000*100) / 200 = 11.000
INSERT INTO pembelian (ID_PEMBELIAN, TGL_BELI, ID_SUPPLIER, NAMA_SUPLIYER, LOKASI, GRAND_TOTAL_BELI, STATUS_TRANSAKSI_BELI)
VALUES ('TST-HPP-PB2', '2026-04-27 10:00:00', @id_supplier, @nama_supplier, @lokasi, 1200000, 'LUNAS');

INSERT INTO pembelian_detail (FAKTUR_BELI, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY_SAT, TOTAL, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER)
VALUES ('TST-HPP-PB2', @id_barang, 'Test Moving Average', 12000, 100, 1200000, @lokasi, @id_supplier, @nama_supplier);

UPDATE tbl_barang SET 
    PEMBELIAN_TOKO = PEMBELIAN_TOKO + 100,
    HARGA_BELI = ROUND((10000*100 + 12000*100) / 200, 2),
    HARGA_BELI_TERAKHIR = 12000
WHERE ID_BARANG = @id_barang;

CALL sp_hlp_stok_hitung(@id_barang);

-- Pembelian 3: 100 unit @ 15.000 = 1.500.000
-- Moving average: (11000*200 + 15000*100) / 300 = 12.333,33
INSERT INTO pembelian (ID_PEMBELIAN, TGL_BELI, ID_SUPPLIER, NAMA_SUPLIYER, LOKASI, GRAND_TOTAL_BELI, STATUS_TRANSAKSI_BELI)
VALUES ('TST-HPP-PB3', '2026-04-27 11:00:00', @id_supplier, @nama_supplier, @lokasi, 1500000, 'LUNAS');

INSERT INTO pembelian_detail (FAKTUR_BELI, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY_SAT, TOTAL, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER)
VALUES ('TST-HPP-PB3', @id_barang, 'Test Moving Average', 15000, 100, 1500000, @lokasi, @id_supplier, @nama_supplier);

UPDATE tbl_barang SET 
    PEMBELIAN_TOKO = PEMBELIAN_TOKO + 100,
    HARGA_BELI = ROUND((11000*200 + 15000*100) / 300, 2),
    HARGA_BELI_TERAKHIR = 15000
WHERE ID_BARANG = @id_barang;

CALL sp_hlp_stok_hitung(@id_barang);

-- Verifikasi kondisi setelah 3 pembelian
SELECT 'Kondisi setelah 3 pembelian:' AS status;
SELECT ID_BARANG, HARGA_BELI, STOK_TOKO, PEMBELIAN_TOKO,
       12333.33 AS expected_hpp,
       IF(ABS(HARGA_BELI - 12333.33) < 1, 'PASS', CONCAT('FAIL: got=', HARGA_BELI)) AS cek_hpp,
       IF(STOK_TOKO = 300, 'PASS', CONCAT('FAIL: got=', STOK_TOKO)) AS cek_stok
FROM tbl_barang WHERE ID_BARANG = @id_barang;

-- =============================================================================
-- STEP 2: Test SP recalculate untuk simulasi hapus PB2 (yang tengah)
-- =============================================================================

-- Seharusnya recalculate dari PB1 dan PB3 saja:
-- PB1: 10000 * 100 = 1.000.000
-- PB3: 15000 * 100 = 1.500.000
-- Total: 2.500.000 / 200 = 12.500

CALL sp_hlp_moving_average_recalculate(@id_barang, 'TST-HPP-PB2', @lokasi, @hpp_baru, @nilai_lama, @nilai_baru);

SELECT 'Test recalculate hapus PB2:' AS status;
SELECT @hpp_baru AS hpp_hasil, 
       12500 AS hpp_expected,
       IF(ABS(@hpp_baru - 12500) < 1, 'PASS', CONCAT('FAIL: got=', @hpp_baru)) AS cek_hpp,
       @nilai_lama AS nilai_persediaan_lama,
       @nilai_baru AS nilai_persediaan_baru,
       (@nilai_lama - @nilai_baru) AS selisih_nilai;

-- =============================================================================
-- STEP 3: Test SP recalculate untuk simulasi hapus PB3 (yang terakhir)
-- =============================================================================

-- Seharusnya recalculate dari PB1 dan PB2 saja:
-- PB1: 10000 * 100 = 1.000.000
-- PB2: 12000 * 100 = 1.200.000
-- Total: 2.200.000 / 200 = 11.000

CALL sp_hlp_moving_average_recalculate(@id_barang, 'TST-HPP-PB3', @lokasi, @hpp_baru, @nilai_lama, @nilai_baru);

SELECT 'Test recalculate hapus PB3:' AS status;
SELECT @hpp_baru AS hpp_hasil, 
       11000 AS hpp_expected,
       IF(ABS(@hpp_baru - 11000) < 1, 'PASS', CONCAT('FAIL: got=', @hpp_baru)) AS cek_hpp,
       @nilai_lama AS nilai_persediaan_lama,
       @nilai_baru AS nilai_persediaan_baru,
       (@nilai_lama - @nilai_baru) AS selisih_nilai;

-- =============================================================================
-- STEP 4: Test SP recalculate untuk simulasi hapus PB1 (yang pertama)
-- =============================================================================

-- Seharusnya recalculate dari PB2 dan PB3 saja:
-- PB2: 12000 * 100 = 1.200.000
-- PB3: 15000 * 100 = 1.500.000
-- Total: 2.700.000 / 200 = 13.500

CALL sp_hlp_moving_average_recalculate(@id_barang, 'TST-HPP-PB1', @lokasi, @hpp_baru, @nilai_lama, @nilai_baru);

SELECT 'Test recalculate hapus PB1:' AS status;
SELECT @hpp_baru AS hpp_hasil, 
       13500 AS hpp_expected,
       IF(ABS(@hpp_baru - 13500) < 1, 'PASS', CONCAT('FAIL: got=', @hpp_baru)) AS cek_hpp,
       @nilai_lama AS nilai_persediaan_lama,
       @nilai_baru AS nilai_persediaan_baru,
       (@nilai_lama - @nilai_baru) AS selisih_nilai;

-- =============================================================================
-- STEP 5: Test edge case - hapus semua pembelian
-- =============================================================================

CALL sp_hlp_moving_average_recalculate(@id_barang, 'SEMUA', @lokasi, @hpp_baru, @nilai_lama, @nilai_baru);

SELECT 'Test recalculate hapus semua:' AS status;
SELECT @hpp_baru AS hpp_hasil, 
       0 AS hpp_expected,
       IF(@hpp_baru = 0, 'PASS', CONCAT('FAIL: got=', @hpp_baru)) AS cek_hpp;

-- Cleanup
DELETE FROM pembelian_detail WHERE FAKTUR_BELI LIKE 'TST-HPP-%';
DELETE FROM pembelian WHERE ID_PEMBELIAN LIKE 'TST-HPP-%';
DELETE FROM tbl_barang WHERE ID_BARANG = 'TST-HPP-001';

SELECT 'Test selesai - data cleanup berhasil' AS status;