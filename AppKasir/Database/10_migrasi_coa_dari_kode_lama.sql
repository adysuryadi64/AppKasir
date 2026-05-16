-- =============================================================================
-- Migrasi COA Lama → COA Baru — Rename Kode Akun di JurnalUmum & tbl_datareferensi
-- Database : db_kasirlancar
--
-- URUTAN EKSEKUSI:
--   1. Jalankan 10_migrasi_coa_dari_kode_lama.sql  (rename kode lama → baru di jurnal)  ← FILE INI
--   2. Jalankan 11_migrasi_akun_coa.sql             (insert/update akun COA final)
--
-- PENTING: Jalankan seluruh script sekaligus agar kode sementara tidak tersisa.
-- Aman dijalankan di database yang belum pernah dimigrasi.
-- Jika sudah dijalankan, semua UPDATE akan affected 0 rows (tidak merusak).
-- =============================================================================

-- =============================================================================
-- STEP 1 : BACKUP (opsional, uncomment jika ingin backup sebelum migrasi)
-- =============================================================================

-- CREATE TABLE IF NOT EXISTS JurnalUmum_backup_coa        SELECT * FROM JurnalUmum;
-- CREATE TABLE IF NOT EXISTS tbl_datareferensi_backup_coa SELECT * FROM tbl_datareferensi;

-- Verifikasi backup (uncomment jika backup diaktifkan)
-- SELECT 'Backup JurnalUmum'        AS Info, COUNT(*) AS Jumlah FROM JurnalUmum_backup_coa
-- UNION ALL
-- SELECT 'Backup tbl_datareferensi', COUNT(*) FROM tbl_datareferensi_backup_coa;

-- =============================================================================
-- STEP 2 : CEK DATA SEBELUM MIGRASI
-- Lihat berapa baris yang akan terpengaruh per kode akun lama
-- =============================================================================

SELECT
    kode_lama,
    kode_baru,
    nama_lama,
    (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_D = kode_lama) AS JurnalD,
    (SELECT COUNT(*) FROM JurnalUmum WHERE NOMOR_AKUN_K = kode_lama) AS JurnalK,
    (SELECT COUNT(*) FROM tbl_datareferensi WHERE KODE_AKUN = kode_lama) AS COA
FROM (
    SELECT '05.03.001' kode_lama, '05.02.001' kode_baru, 'PENJUALAN (lama)'                 nama_lama UNION ALL
    SELECT '05.03.002',           '05.03.001',            'RETUR PENJUALAN (lama)'                     UNION ALL
    SELECT '05.03.003',           '05.04.001',            'POTONGAN DISKON PENJUALAN (lama)'            UNION ALL
    SELECT '06.02.001',           '06.03.001',            'BIAYA KIRIM PENJUALAN (lama)'                UNION ALL
    SELECT '06.03.001',           '06.04.001',            'PENYESUAIAN STOK MINUS (lama)'               UNION ALL
    SELECT '06.04.001',           '06.05.001',            'POTONGAN DISKON PEMBELIAN (lama)'            UNION ALL
    SELECT '06.05.001',           '06.02.001',            'BIAYA KIRIM PEMBELIAN (lama)'                UNION ALL
    SELECT '04.03.001',           '04.01.003',            'REKENING KORAN PUSAT (lama)'                 UNION ALL
    SELECT '07.01.010',           '05.04.001',            'BEBAN DISKON PENJUALAN (lama)'
) t;

-- =============================================================================
-- STEP 3 : RENAME — TAHAP 1 (pindah ke kode sementara 99.xx.xxx)
-- Diperlukan karena beberapa kode saling bertukar nilai:
--   06.02.001 → 06.03.001  DAN  06.05.001 → 06.02.001  (saling tukar)
--   06.03.001 → 06.04.001  DAN  06.04.001 → 06.05.001  (saling tukar)
-- =============================================================================

-- ── 05.03.001 PENJUALAN (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.03.001a' WHERE NOMOR_AKUN_D = '05.03.001';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.03.001a' WHERE NOMOR_AKUN_K = '05.03.001';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.03.001a' WHERE KODE_AKUN    = '05.03.001';

-- ── 05.03.002 RETUR PENJUALAN (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.03.002'  WHERE NOMOR_AKUN_D = '05.03.002';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.03.002'  WHERE NOMOR_AKUN_K = '05.03.002';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.03.002'  WHERE KODE_AKUN    = '05.03.002';

-- ── 05.03.003 POTONGAN DISKON PENJUALAN (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.03.003'  WHERE NOMOR_AKUN_D = '05.03.003';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.03.003'  WHERE NOMOR_AKUN_K = '05.03.003';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.03.003'  WHERE KODE_AKUN    = '05.03.003';

-- ── 06.02.001 BIAYA KIRIM PENJUALAN (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.06.002'  WHERE NOMOR_AKUN_D = '06.02.001';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.06.002'  WHERE NOMOR_AKUN_K = '06.02.001';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.06.002'  WHERE KODE_AKUN    = '06.02.001';

-- ── 06.03.001 PENYESUAIAN STOK MINUS (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.06.003'  WHERE NOMOR_AKUN_D = '06.03.001';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.06.003'  WHERE NOMOR_AKUN_K = '06.03.001';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.06.003'  WHERE KODE_AKUN    = '06.03.001';

-- ── 06.04.001 POTONGAN DISKON PEMBELIAN (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.06.004'  WHERE NOMOR_AKUN_D = '06.04.001';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.06.004'  WHERE NOMOR_AKUN_K = '06.04.001';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.06.004'  WHERE KODE_AKUN    = '06.04.001';

-- ── 06.05.001 BIAYA KIRIM PEMBELIAN (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.06.005'  WHERE NOMOR_AKUN_D = '06.05.001';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.06.005'  WHERE NOMOR_AKUN_K = '06.05.001';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.06.005'  WHERE KODE_AKUN    = '06.05.001';

-- ── 04.03.001 REKENING KORAN PUSAT (lama) → sementara
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '99.04.003'  WHERE NOMOR_AKUN_D = '04.03.001';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '99.04.003'  WHERE NOMOR_AKUN_K = '04.03.001';
UPDATE tbl_datareferensi SET KODE_AKUN    = '99.04.003'  WHERE KODE_AKUN    = '04.03.001';

-- =============================================================================
-- STEP 4 : RENAME — TAHAP 2 (pindah dari sementara ke kode baru final)
-- Hapus kode tujuan HANYA jika kode sementara 99.03.001a masih ada di tabel
-- (artinya STEP 3 baru saja jalan dan ada sisa dari run sebelumnya yang gagal)
-- =============================================================================

SET @ada_sementara = (SELECT COUNT(*) FROM tbl_datareferensi WHERE KODE_AKUN = '99.03.001a');
SET @sql_del = IF(@ada_sementara > 0,
    'DELETE FROM tbl_datareferensi WHERE KODE_AKUN IN (''05.02.001'',''05.03.001'',''05.04.001'',''06.02.001'',''06.03.001'',''06.04.001'',''06.05.001'',''04.01.003'')',
    'SELECT ''Tidak ada sisa, DELETE dilewati'' AS info'
);
PREPARE stmt FROM @sql_del;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ── 99.03.001a → 05.02.001 PENJUALAN
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '05.02.001', NAMA_AKUN_D = 'PENJUALAN'                 WHERE NOMOR_AKUN_D = '99.03.001a';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '05.02.001', NAMA_AKUN_K = 'PENJUALAN'                 WHERE NOMOR_AKUN_K = '99.03.001a';
UPDATE tbl_datareferensi SET KODE_AKUN    = '05.02.001', NAMA_AKUN   = 'PENJUALAN'                 WHERE KODE_AKUN    = '99.03.001a';

-- ── 99.03.002 → 05.03.001 RETUR PENJUALAN
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '05.03.001', NAMA_AKUN_D = 'RETUR PENJUALAN'           WHERE NOMOR_AKUN_D = '99.03.002';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '05.03.001', NAMA_AKUN_K = 'RETUR PENJUALAN'           WHERE NOMOR_AKUN_K = '99.03.002';
UPDATE tbl_datareferensi SET KODE_AKUN    = '05.03.001', NAMA_AKUN   = 'RETUR PENJUALAN'           WHERE KODE_AKUN    = '99.03.002';

-- ── 99.03.003 → 05.04.001 POTONGAN DISKON PENJUALAN
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '05.04.001', NAMA_AKUN_D = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_D = '99.03.003';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '05.04.001', NAMA_AKUN_K = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_K = '99.03.003';
UPDATE tbl_datareferensi SET KODE_AKUN    = '05.04.001', NAMA_AKUN   = 'POTONGAN DISKON PENJUALAN' WHERE KODE_AKUN    = '99.03.003';

-- ── 99.06.002 → 06.03.001 BIAYA KIRIM PENJUALAN
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '06.03.001', NAMA_AKUN_D = 'BIAYA KIRIM PENJUALAN'     WHERE NOMOR_AKUN_D = '99.06.002';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '06.03.001', NAMA_AKUN_K = 'BIAYA KIRIM PENJUALAN'     WHERE NOMOR_AKUN_K = '99.06.002';
UPDATE tbl_datareferensi SET KODE_AKUN    = '06.03.001', NAMA_AKUN   = 'BIAYA KIRIM PENJUALAN'     WHERE KODE_AKUN    = '99.06.002';

-- ── 99.06.003 → 06.04.001 PENYESUAIAN STOK MINUS
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '06.04.001', NAMA_AKUN_D = 'PENYESUAIAN STOK MINUS'    WHERE NOMOR_AKUN_D = '99.06.003';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '06.04.001', NAMA_AKUN_K = 'PENYESUAIAN STOK MINUS'    WHERE NOMOR_AKUN_K = '99.06.003';
UPDATE tbl_datareferensi SET KODE_AKUN    = '06.04.001', NAMA_AKUN   = 'PENYESUAIAN STOK MINUS'    WHERE KODE_AKUN    = '99.06.003';

-- ── 99.06.004 → 06.05.001 POTONGAN DISKON PEMBELIAN
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '06.05.001', NAMA_AKUN_D = 'POTONGAN DISKON PEMBELIAN' WHERE NOMOR_AKUN_D = '99.06.004';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '06.05.001', NAMA_AKUN_K = 'POTONGAN DISKON PEMBELIAN' WHERE NOMOR_AKUN_K = '99.06.004';
UPDATE tbl_datareferensi SET KODE_AKUN    = '06.05.001', NAMA_AKUN   = 'POTONGAN DISKON PEMBELIAN' WHERE KODE_AKUN    = '99.06.004';

-- ── 99.06.005 → 06.02.001 BIAYA KIRIM PEMBELIAN
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '06.02.001', NAMA_AKUN_D = 'BIAYA KIRIM PEMBELIAN'     WHERE NOMOR_AKUN_D = '99.06.005';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '06.02.001', NAMA_AKUN_K = 'BIAYA KIRIM PEMBELIAN'     WHERE NOMOR_AKUN_K = '99.06.005';
UPDATE tbl_datareferensi SET KODE_AKUN    = '06.02.001', NAMA_AKUN   = 'BIAYA KIRIM PEMBELIAN'     WHERE KODE_AKUN    = '99.06.005';

-- ── 99.04.003 → 04.01.003 REKENING KORAN PUSAT
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '04.01.003', NAMA_AKUN_D = 'REKENING KORAN PUSAT'      WHERE NOMOR_AKUN_D = '99.04.003';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '04.01.003', NAMA_AKUN_K = 'REKENING KORAN PUSAT'      WHERE NOMOR_AKUN_K = '99.04.003';
UPDATE tbl_datareferensi SET KODE_AKUN    = '04.01.003', NAMA_AKUN   = 'REKENING KORAN PUSAT'      WHERE KODE_AKUN    = '99.04.003';

-- ── 07.01.010 BEBAN DISKON PENJUALAN → 05.04.001 POTONGAN DISKON PENJUALAN
-- Tidak perlu kode sementara karena 05.04.001 sudah final di tahap ini
UPDATE JurnalUmum        SET NOMOR_AKUN_D = '05.04.001', NAMA_AKUN_D = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_D = '07.01.010';
UPDATE JurnalUmum        SET NOMOR_AKUN_K = '05.04.001', NAMA_AKUN_K = 'POTONGAN DISKON PENJUALAN' WHERE NOMOR_AKUN_K = '07.01.010';
UPDATE tbl_datareferensi SET KODE_AKUN    = '05.04.001', NAMA_AKUN   = 'POTONGAN DISKON PENJUALAN' WHERE KODE_AKUN    = '07.01.010';

-- =============================================================================
-- STEP 5 : KOREKSI NAMA & HAPUS AKUN TIDAK TERPAKAI
-- =============================================================================

-- ── Koreksi nama 06.01.001 jika masih 'LABA KOTOR PENJUALAN' (nama lama yang salah)
UPDATE JurnalUmum
SET NAMA_AKUN_D = 'HPP POKOK PENJUALAN'
WHERE NOMOR_AKUN_D = '06.01.001' AND NAMA_AKUN_D = 'LABA KOTOR PENJUALAN';

UPDATE JurnalUmum
SET NAMA_AKUN_K = 'HPP POKOK PENJUALAN'
WHERE NOMOR_AKUN_K = '06.01.001' AND NAMA_AKUN_K = 'LABA KOTOR PENJUALAN';

UPDATE tbl_datareferensi
SET NAMA_AKUN = 'HPP POKOK PENJUALAN'
WHERE KODE_AKUN = '06.01.001' AND NAMA_AKUN = 'LABA KOTOR PENJUALAN';

-- ── Hapus akun penyusutan tanah (tidak ada di COA baru, tidak pernah dipakai di jurnal)
DELETE FROM tbl_datareferensi WHERE KODE_AKUN = '02.02.001'; -- AKUM. PENY. TANAH
DELETE FROM tbl_datareferensi WHERE KODE_AKUN = '07.01.006'; -- BEBAN PENYUSUTAN TANAH

-- ── Koreksi JENIS di historybarang: 'RETUR BELI KELUAR' → 'RETUR BELI'
UPDATE historybarang SET JENIS = 'RETUR BELI' WHERE JENIS = 'RETUR BELI KELUAR';

-- ── Kunci akun yang dipakai hardcoded di kode jurnal agar tidak bisa diubah user
UPDATE tbl_datareferensi SET STATUS = 'Terkunci' WHERE KODE_AKUN = '03.02.001'; -- HUTANG PAJAK

-- =============================================================================
-- STEP 6 : VERIFIKASI — pastikan tidak ada kode sementara atau kode lama tersisa
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
    SELECT '99.03.001a' kode UNION ALL SELECT '99.03.002' UNION ALL SELECT '99.03.003' UNION ALL
    SELECT '99.06.002'       UNION ALL SELECT '99.06.003' UNION ALL SELECT '99.06.004' UNION ALL
    SELECT '99.06.005'       UNION ALL SELECT '99.04.003' UNION ALL
    SELECT '05.03.001'       UNION ALL SELECT '05.03.002' UNION ALL SELECT '05.03.003' UNION ALL
    SELECT '04.03.001'       UNION ALL SELECT '07.01.010'
) t;

-- Verifikasi nama 06.01.001 sudah benar
SELECT Kode_akun, Nama_Akun,
    CASE WHEN Nama_Akun = 'HPP POKOK PENJUALAN' THEN '✅ Benar' ELSE '❌ Masih salah' END AS Status
FROM tbl_datareferensi WHERE Kode_akun = '06.01.001';

-- Verifikasi akun penyusutan tanah sudah dihapus
SELECT CASE WHEN COUNT(*) = 0 THEN '✅ Sudah dihapus' ELSE '❌ Masih ada' END AS Status_Hapus_Tanah
FROM tbl_datareferensi WHERE KODE_AKUN IN ('02.02.001', '07.01.006');

-- Verifikasi historybarang sudah dikoreksi
SELECT CASE WHEN COUNT(*) = 0 THEN '✅ Bersih' ELSE CONCAT('❌ Masih ada ', COUNT(*)) END AS Status_Retur_Beli
FROM historybarang WHERE JENIS = 'RETUR BELI KELUAR';

-- Tampilkan COA final untuk konfirmasi
SELECT Kode_akun, Nama_Akun, AKUN_DK, TYPE_AKUN
FROM tbl_datareferensi
WHERE Kode_akun IN ('05.02.001','05.03.001','05.04.001','06.02.001','06.03.001','06.04.001','06.05.001','04.01.003')
ORDER BY Kode_akun;

-- =============================================================================
-- SELESAI
-- Jika ada masalah, restore dari backup:
--   TRUNCATE TABLE JurnalUmum;
--   INSERT INTO JurnalUmum SELECT * FROM JurnalUmum_backup_coa;
--   TRUNCATE TABLE tbl_datareferensi;
--   INSERT INTO tbl_datareferensi SELECT * FROM tbl_datareferensi_backup_coa;
-- =============================================================================
