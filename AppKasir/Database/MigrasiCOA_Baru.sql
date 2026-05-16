-- =============================================================================
-- MIGRASI COA LAMA → COA BARU
-- Database : db_kasirlancar
-- Jalankan di : MySQL Workbench / HeidiSQL / phpMyAdmin
--
-- PENTING: Jalankan seluruh script sekaligus (tidak sepotong-sepotong)
--          agar urutan tahap sementara tidak menyebabkan konflik.
-- =============================================================================

USE db_kasirlancar;

-- =============================================================================
-- STEP 1 : BACKUP
-- =============================================================================

CREATE TABLE IF NOT EXISTS JurnalUmum_backup_coa
    SELECT * FROM JurnalUmum;

CREATE TABLE IF NOT EXISTS tbl_datareferensi_backup_coa
    SELECT * FROM tbl_datareferensi;

-- Verifikasi backup
SELECT 'Backup JurnalUmum'       AS Info, COUNT(*) AS Jumlah FROM JurnalUmum_backup_coa
UNION ALL
SELECT 'Backup tbl_datareferensi', COUNT(*) FROM tbl_datareferensi_backup_coa;

-- =============================================================================
-- STEP 2 : CEK DATA SEBELUM MIGRASI
-- Lihat berapa baris yang akan terpengaruh per kode akun lama
-- =============================================================================

SELECT
    'SEBELUM MIGRASI' AS Status,
    kode_lama,
    nama_lama,
    kode_baru,
    (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_D = kode_lama) AS JurnalD,
    (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_K = kode_lama) AS JurnalK,
    (SELECT COUNT(*) FROM tbl_datareferensi WHERE KODE_AKUN = kode_lama) AS COA
FROM (
    SELECT '05.03.001' AS kode_lama, 'PENJUALAN'                AS nama_lama, '05.02.001' AS kode_baru UNION ALL
    SELECT '05.03.002',              'RETUR PENJUALAN',                        '05.03.001'             UNION ALL
    SELECT '05.03.003',              'POTONGAN DISKON PENJUALAN',               '05.04.001'             UNION ALL
    SELECT '06.02.001',              'BIAYA KIRIM PENJUALAN',                   '06.03.001'             UNION ALL
    SELECT '06.03.001',              'PENYESUAIAN STOK MINUS',                  '06.04.001'             UNION ALL
    SELECT '06.04.001',              'POTONGAN DISKON PEMBELIAN',               '06.05.001'             UNION ALL
    SELECT '06.05.001',              'BIAYA KIRIM PEMBELIAN',                   '06.02.001'             UNION ALL
    SELECT '04.03.001',              'REKENING KORAN PUSAT',                    '04.01.003'             UNION ALL
    SELECT '07.01.010',              'BEBAN DISKON PENJUALAN',                  '05.04.001'
) t;

-- =============================================================================
-- STEP 3 : MIGRASI — TAHAP 1 (pindah ke kode sementara 99.xx.xxx)
-- Diperlukan karena beberapa kode saling bertukar nilai
-- (misal 06.02.001 → 06.03.001 dan 06.05.001 → 06.02.001)
-- =============================================================================

-- ── 05.03.001 PENJUALAN → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.03.001a' WHERE NOMOR_AKUN_D = '05.03.001';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.03.001a' WHERE NOMOR_AKUN_K = '05.03.001';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.03.001a' WHERE KODE_AKUN    = '05.03.001';

-- ── 05.03.002 RETUR PENJUALAN → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.03.002'  WHERE NOMOR_AKUN_D = '05.03.002';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.03.002'  WHERE NOMOR_AKUN_K = '05.03.002';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.03.002'  WHERE KODE_AKUN    = '05.03.002';

-- ── 05.03.003 POTONGAN DISKON PENJUALAN → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.03.003'  WHERE NOMOR_AKUN_D = '05.03.003';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.03.003'  WHERE NOMOR_AKUN_K = '05.03.003';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.03.003'  WHERE KODE_AKUN    = '05.03.003';

-- ── 06.02.001 BIAYA KIRIM PENJUALAN → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.06.002'  WHERE NOMOR_AKUN_D = '06.02.001';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.06.002'  WHERE NOMOR_AKUN_K = '06.02.001';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.06.002'  WHERE KODE_AKUN    = '06.02.001';

-- ── 06.03.001 PENYESUAIAN STOK MINUS → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.06.003'  WHERE NOMOR_AKUN_D = '06.03.001';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.06.003'  WHERE NOMOR_AKUN_K = '06.03.001';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.06.003'  WHERE KODE_AKUN    = '06.03.001';

-- ── 06.04.001 POTONGAN DISKON PEMBELIAN → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.06.004'  WHERE NOMOR_AKUN_D = '06.04.001';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.06.004'  WHERE NOMOR_AKUN_K = '06.04.001';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.06.004'  WHERE KODE_AKUN    = '06.04.001';

-- ── 06.05.001 BIAYA KIRIM PEMBELIAN → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.06.005'  WHERE NOMOR_AKUN_D = '06.05.001';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.06.005'  WHERE NOMOR_AKUN_K = '06.05.001';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.06.005'  WHERE KODE_AKUN    = '06.05.001';

-- ── 04.03.001 REKENING KORAN PUSAT → sementara
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '99.04.003'  WHERE NOMOR_AKUN_D = '04.03.001';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '99.04.003'  WHERE NOMOR_AKUN_K = '04.03.001';
UPDATE tbl_datareferensi SET KODE_AKUN   = '99.04.003'  WHERE KODE_AKUN    = '04.03.001';

-- =============================================================================
-- STEP 4 : MIGRASI — TAHAP 2 (pindah dari sementara ke kode baru final)
-- =============================================================================

-- ── 99.03.001a → 05.02.001 PENJUALAN
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '05.02.001', NAMA_AKUN_D = 'PENJUALAN'                WHERE NOMOR_AKUN_D = '99.03.001a';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '05.02.001', NAMA_AKUN_K = 'PENJUALAN'                WHERE NOMOR_AKUN_K = '99.03.001a';
UPDATE tbl_datareferensi SET KODE_AKUN   = '05.02.001', NAMA_AKUN   = 'PENJUALAN'                WHERE KODE_AKUN    = '99.03.001a';

-- ── 99.03.002 → 05.03.001 RETUR PENJUALAN
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '05.03.001', NAMA_AKUN_D = 'RETUR PENJUALAN'          WHERE NOMOR_AKUN_D = '99.03.002';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '05.03.001', NAMA_AKUN_K = 'RETUR PENJUALAN'          WHERE NOMOR_AKUN_K = '99.03.002';
UPDATE tbl_datareferensi SET KODE_AKUN   = '05.03.001', NAMA_AKUN   = 'RETUR PENJUALAN'          WHERE KODE_AKUN    = '99.03.002';

-- ── 99.03.003 → 05.04.001 POTONGAN DISKON PENJUALAN
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '05.04.001', NAMA_AKUN_D = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_D = '99.03.003';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '05.04.001', NAMA_AKUN_K = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_K = '99.03.003';
UPDATE tbl_datareferensi SET KODE_AKUN   = '05.04.001', NAMA_AKUN   = 'POTONGAN DISKON PENJUALAN' WHERE KODE_AKUN    = '99.03.003';

-- ── 99.06.002 → 06.03.001 BIAYA KIRIM PENJUALAN
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '06.03.001', NAMA_AKUN_D = 'BIAYA KIRIM PENJUALAN'    WHERE NOMOR_AKUN_D = '99.06.002';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '06.03.001', NAMA_AKUN_K = 'BIAYA KIRIM PENJUALAN'    WHERE NOMOR_AKUN_K = '99.06.002';
UPDATE tbl_datareferensi SET KODE_AKUN   = '06.03.001', NAMA_AKUN   = 'BIAYA KIRIM PENJUALAN'    WHERE KODE_AKUN    = '99.06.002';

-- ── 99.06.003 → 06.04.001 PENYESUAIAN STOK MINUS
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '06.04.001', NAMA_AKUN_D = 'PENYESUAIAN STOK MINUS'   WHERE NOMOR_AKUN_D = '99.06.003';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '06.04.001', NAMA_AKUN_K = 'PENYESUAIAN STOK MINUS'   WHERE NOMOR_AKUN_K = '99.06.003';
UPDATE tbl_datareferensi SET KODE_AKUN   = '06.04.001', NAMA_AKUN   = 'PENYESUAIAN STOK MINUS'   WHERE KODE_AKUN    = '99.06.003';

-- ── 99.06.004 → 06.05.001 POTONGAN DISKON PEMBELIAN
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '06.05.001', NAMA_AKUN_D = 'POTONGAN DISKON PEMBELIAN' WHERE NOMOR_AKUN_D = '99.06.004';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '06.05.001', NAMA_AKUN_K = 'POTONGAN DISKON PEMBELIAN' WHERE NOMOR_AKUN_K = '99.06.004';
UPDATE tbl_datareferensi SET KODE_AKUN   = '06.05.001', NAMA_AKUN   = 'POTONGAN DISKON PEMBELIAN' WHERE KODE_AKUN    = '99.06.004';

-- ── 99.06.005 → 06.02.001 BIAYA KIRIM PEMBELIAN
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '06.02.001', NAMA_AKUN_D = 'BIAYA KIRIM PEMBELIAN'    WHERE NOMOR_AKUN_D = '99.06.005';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '06.02.001', NAMA_AKUN_K = 'BIAYA KIRIM PEMBELIAN'    WHERE NOMOR_AKUN_K = '99.06.005';
UPDATE tbl_datareferensi SET KODE_AKUN   = '06.02.001', NAMA_AKUN   = 'BIAYA KIRIM PEMBELIAN'    WHERE KODE_AKUN    = '99.06.005';

-- ── 99.04.003 → 04.01.003 REKENING KORAN PUSAT
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '04.01.003', NAMA_AKUN_D = 'REKENING KORAN PUSAT'     WHERE NOMOR_AKUN_D = '99.04.003';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '04.01.003', NAMA_AKUN_K = 'REKENING KORAN PUSAT'     WHERE NOMOR_AKUN_K = '99.04.003';
UPDATE tbl_datareferensi SET KODE_AKUN   = '04.01.003', NAMA_AKUN   = 'REKENING KORAN PUSAT'     WHERE KODE_AKUN    = '99.04.003';

-- ── 07.01.010 BEBAN DISKON PENJUALAN → 05.04.001 POTONGAN DISKON PENJUALAN
-- Tidak perlu kode sementara karena 05.04.001 sudah final di tahap ini
-- Sisi DEBIT: 07.01.010 → 05.04.001
UPDATE JurnalUmum       SET NOMOR_AKUN_D = '05.04.001', NAMA_AKUN_D = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_D = '07.01.010';
UPDATE JurnalUmum       SET NOMOR_AKUN_K = '05.04.001', NAMA_AKUN_K = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_K = '07.01.010';
UPDATE tbl_datareferensi SET KODE_AKUN   = '05.04.001', NAMA_AKUN   = 'POTONGAN DISKON PENJUALAN' WHERE KODE_AKUN    = '07.01.010';

-- ── 06.01.001 nama salah 'LABA KOTOR PENJUALAN' → koreksi nama menjadi 'HPP POKOK PENJUALAN'
-- Hanya koreksi NAMA_AKUN_K di baris yang berpasangan dengan diskon penjualan
-- (06.01.001 sebagai kode tetap benar, hanya nama yang salah di data lama)
UPDATE JurnalUmum
SET NAMA_AKUN_K = 'HPP POKOK PENJUALAN'
WHERE NOMOR_AKUN_K = '06.01.001'
  AND NAMA_AKUN_K  = 'LABA KOTOR PENJUALAN';

-- Koreksi juga di sisi NAMA_AKUN_D jika ada
UPDATE JurnalUmum
SET NAMA_AKUN_D = 'HPP POKOK PENJUALAN'
WHERE NOMOR_AKUN_D = '06.01.001'
  AND NAMA_AKUN_D  = 'LABA KOTOR PENJUALAN';

-- Koreksi nama di tbl_datareferensi
UPDATE tbl_datareferensi
SET NAMA_AKUN = 'HPP POKOK PENJUALAN'
WHERE KODE_AKUN = '06.01.001'
  AND NAMA_AKUN = 'LABA KOTOR PENJUALAN';

-- ── HAPUS akun penyusutan tanah (tidak ada di COA baru, belum pernah dipakai di jurnal)
DELETE FROM tbl_datareferensi WHERE KODE_AKUN = '02.02.001'; -- AKUM. PENY. TANAH
DELETE FROM tbl_datareferensi WHERE KODE_AKUN = '07.01.006'; -- BEBAN PENYUSUTAN TANAH

-- ── Koreksi JENIS di historybarang: 'RETUR BELI KELUAR' → 'RETUR BELI'
UPDATE historybarang SET JENIS = 'RETUR BELI' WHERE JENIS = 'RETUR BELI KELUAR';

-- ── Kunci akun yang dipakai hardcoded di FormPenjualan agar tidak bisa diubah user
-- 03.02.001 HUTANG PAJAK — dipakai langsung di kode jurnal penjualan (SimpanJurnalUmum)
UPDATE tbl_datareferensi SET STATUS = 'Terkunci' WHERE KODE_AKUN = '03.02.001';

-- =============================================================================
-- STEP 4b : TAMBAH AKUN BARU YANG BELUM ADA DI COA LAMA
-- =============================================================================

-- ── 06.06.001 RETUR PEMBELIAN (kontra-HPP, KREDIT)
-- Akun ini tidak ada di COA lama maupun COA baru sebelumnya.
-- Dibutuhkan untuk jurnal sp_trx_retur_beli_simpan.
INSERT IGNORE INTO tbl_datareferensi
    (STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, KETERANGAN)
VALUES
    ('NULL', 'HPP', 'RETUR BELI', '06.06.001', 'RETUR PEMBELIAN', 'LABA', 'KREDIT', 'LABA RUGI',
     'Pengembalian barang ke supplier karena cacat atau tidak sesuai. Mengurangi nilai pembelian/HPP. Akun kontra-HPP (kredit). Saldo normal kredit.');

-- =============================================================================
-- STEP 5 : VERIFIKASI — pastikan tidak ada kode sementara tersisa
-- =============================================================================

SELECT
    kode,
    (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_D = kode) AS JurnalD,
    (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_K = kode) AS JurnalK,
    (SELECT COUNT(*) FROM tbl_datareferensi WHERE KODE_AKUN = kode) AS COA,
    CASE
        WHEN (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_D = kode) +
             (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_K = kode) +
             (SELECT COUNT(*) FROM tbl_datareferensi WHERE KODE_AKUN = kode) = 0
        THEN '✅ Bersih'
        ELSE '❌ MASIH ADA SISA'
    END AS Status
FROM (
    SELECT '99.03.001a' AS kode UNION ALL
    SELECT '99.03.002'          UNION ALL
    SELECT '99.03.003'          UNION ALL
    SELECT '99.06.002'          UNION ALL
    SELECT '99.06.003'          UNION ALL
    SELECT '99.06.004'          UNION ALL
    SELECT '99.06.005'          UNION ALL
    SELECT '99.04.003'          UNION ALL
    -- kode lama asli
    SELECT '05.03.001'          UNION ALL
    SELECT '05.03.002'          UNION ALL
    SELECT '05.03.003'          UNION ALL
    SELECT '04.03.001'          UNION ALL
    SELECT '07.01.010'
) t;

-- Verifikasi tambahan: pastikan tidak ada lagi nama 'LABA KOTOR PENJUALAN' di JurnalUmum
SELECT
    CASE WHEN COUNT(*) = 0
        THEN '✅ Tidak ada lagi LABA KOTOR PENJUALAN di JurnalUmum'
        ELSE CONCAT('❌ Masih ada ', COUNT(*), ' baris LABA KOTOR PENJUALAN')
    END AS Status_Nama_06_01_001
FROM JurnalUmum
WHERE NAMA_AKUN_K = 'LABA KOTOR PENJUALAN'
   OR NAMA_AKUN_D = 'LABA KOTOR PENJUALAN';

-- Verifikasi nama 06.01.001 di tbl_datareferensi sudah benar
SELECT KODE_AKUN, NAMA_AKUN,
    CASE WHEN NAMA_AKUN = 'HPP POKOK PENJUALAN'
        THEN '✅ Nama sudah benar'
        ELSE '❌ Nama masih salah'
    END AS Status
FROM tbl_datareferensi WHERE KODE_AKUN = '06.01.001';

-- Verifikasi akun penyusutan tanah sudah dihapus
SELECT
    CASE WHEN COUNT(*) = 0
        THEN '✅ 02.02.001 dan 07.01.006 sudah dihapus'
        ELSE CONCAT('❌ Masih ada ', COUNT(*), ' akun penyusutan tanah')
    END AS Status_Hapus_Tanah
FROM tbl_datareferensi
WHERE KODE_AKUN IN ('02.02.001', '07.01.006');

-- Verifikasi historybarang RETUR BELI KELUAR sudah dikoreksi
SELECT
    CASE WHEN COUNT(*) = 0
        THEN '✅ Tidak ada lagi RETUR BELI KELUAR di historybarang'
        ELSE CONCAT('❌ Masih ada ', COUNT(*), ' baris RETUR BELI KELUAR')
    END AS Status_Retur_Beli
FROM historybarang WHERE JENIS = 'RETUR BELI KELUAR';

-- =============================================================================
-- STEP 6 : VERIFIKASI — tampilkan kode baru di tbl_datareferensi
-- =============================================================================

SELECT KODE_AKUN, NAMA_AKUN
FROM tbl_datareferensi
WHERE KODE_AKUN IN (
    '05.02.001','05.03.001','05.04.001',
    '06.02.001','06.03.001','06.04.001','06.05.001',
    '04.01.003'
)
ORDER BY KODE_AKUN;

-- =============================================================================
-- SELESAI
-- Jika ada masalah, restore dari backup:
--   TRUNCATE TABLE JurnalUmum;
--   INSERT INTO JurnalUmum SELECT * FROM JurnalUmum_backup_coa;
--   TRUNCATE TABLE tbl_datareferensi;
--   INSERT INTO tbl_datareferensi SELECT * FROM tbl_datareferensi_backup_coa;
-- =============================================================================
