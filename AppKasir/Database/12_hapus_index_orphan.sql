-- =============================================================================
-- 12_hapus_index_orphan.sql
-- Hapus index lama yang tidak terdaftar di 03_migrasi_index.sql
-- Tujuan : bebaskan space disk yang ditempati index tidak terpakai
--          tanpa menyentuh data sama sekali
--
-- Latar belakang:
--   Saat database masih kosong (data sudah dihapus), ukuran db_kasirlancar
--   masih 1.46 GB — 93% ditempati 3 tabel:
--     penjualan_detail  : 594 MB (index 398 MB, data 195 MB)
--     jurnalumum        : 556 MB (index 424 MB, data 131 MB)
--     penjualan         : 208 MB (index 162 MB, data  46 MB)
--   Index jauh lebih besar dari data → ada index orphan yang tidak pernah
--   dibersihkan sejak migrasi sebelumnya.
--
-- Aman dijalankan berulang kali (idempoten) — pakai DROP IF EXISTS via
-- stored procedure, tidak akan error jika index sudah tidak ada.
--
-- TIDAK menghapus data. TIDAK mengubah struktur tabel.
-- Jalankan SETELAH 11_migrasi_akun_coa.sql
-- =============================================================================

DROP PROCEDURE IF EXISTS hapus_index_jika_ada;
DELIMITER $
CREATE PROCEDURE hapus_index_jika_ada(IN tbl VARCHAR(100), IN idx VARCHAR(100))
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND INDEX_NAME   = idx
    ) THEN
        SET @sql = CONCAT('ALTER TABLE `', tbl, '` DROP INDEX `', idx, '`');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        SELECT CONCAT('HAPUS    : ', tbl, '.', idx) AS hasil;
    ELSE
        SELECT CONCAT('SKIP     : ', tbl, '.', idx, ' (tidak ditemukan)') AS hasil;
    END IF;
END$
DELIMITER ;

-- =============================================================================
-- bon_karyawan
-- idx_lokasi_bon_karyawan — tidak ada query WHERE LOKASI di bon_karyawan
-- =============================================================================
CALL hapus_index_jika_ada('bon_karyawan', 'idx_lokasi_bon_karyawan');

-- =============================================================================
-- gaji_karyawan
-- idx_lokasi_gaji_karyawan — tidak ada query WHERE LOKASI di gaji_karyawan
-- =============================================================================
CALL hapus_index_jika_ada('gaji_karyawan', 'idx_lokasi_gaji_karyawan');

-- =============================================================================
-- historybarang
-- HistoryBarang_ID_BARANG — prefix dari idx_barang_jenis_tgl (ID_BARANG,JENIS,TANGGAL)
--                           optimizer selalu pilih composite, single ini tidak pernah dipakai
-- HistoryBarang_LOKASI    — prefix dari idx_lokasi_jenis_barang_qty (LOKASI,JENIS,...)
--                           redundan, composite sudah cover semua kasus
-- =============================================================================
CALL hapus_index_jika_ada('historybarang', 'HistoryBarang_ID_BARANG');
CALL hapus_index_jika_ada('historybarang', 'HistoryBarang_LOKASI');

-- =============================================================================
-- hutang
-- idx_lokasi_hutang — tidak ada query WHERE LOKASI saja di hutang;
--                     idx_tgl_lokasi_hutang (TGLPEMBAYARAN,LOKASI) sudah cover
-- =============================================================================
CALL hapus_index_jika_ada('hutang', 'idx_lokasi_hutang');

-- =============================================================================
-- hutang_detail
-- idx_lokasi_hutang_detail — tidak ada query WHERE LOKASI di hutang_detail
-- =============================================================================
CALL hapus_index_jika_ada('hutang_detail', 'idx_lokasi_hutang_detail');

-- =============================================================================
-- penjualan_detail
-- idx_lokasibarang_detail — prefix dari idx_tgl_lokasi_jual (TANGGAL_JUAL,LOKASIBARANG)
--                           dan idx_tgl_lokasi_barang (TANGGAL_JUAL,LOKASIBARANG,ID_BARANG)
--                           tidak ada query WHERE LOKASIBARANG saja tanpa TANGGAL
-- =============================================================================
CALL hapus_index_jika_ada('penjualan_detail', 'idx_lokasibarang_detail');

-- =============================================================================
-- piutang
-- idx_lokasi_piutang — prefix dari idx_tgl_lokasi_piutang (TGL_BAYAR,LOKASI)
--                      tidak ada query WHERE LOKASI saja di piutang
-- =============================================================================
CALL hapus_index_jika_ada('piutang', 'idx_lokasi_piutang');

-- =============================================================================
-- piutang_detail
-- idx_lokasi_piutang_detail — tidak ada query WHERE LOKASI di piutang_detail
-- =============================================================================
CALL hapus_index_jika_ada('piutang_detail', 'idx_lokasi_piutang_detail');

-- =============================================================================
-- surat_jalan
-- idx_lokasi_surat_jalan — tidak ada query WHERE LOKASI saja di surat_jalan
-- =============================================================================
CALL hapus_index_jika_ada('surat_jalan', 'idx_lokasi_surat_jalan');

-- =============================================================================
-- surat_jalan_detail
-- idx_lokasi_surat_jalan_detail — tidak ada query WHERE LOKASI di surat_jalan_detail
-- =============================================================================
CALL hapus_index_jika_ada('surat_jalan_detail', 'idx_lokasi_surat_jalan_detail');

-- =============================================================================
-- transfer_barang
-- idx_lokasi_transfer_barang — tidak ada query WHERE LOKASI saja di transfer_barang
-- =============================================================================
CALL hapus_index_jika_ada('transfer_barang', 'idx_lokasi_transfer_barang');

-- =============================================================================
-- transfer_barang_detail
-- idx_lokasi_transfer_barang_detail — prefix dari idx_transfer_barang_id (ID_TRANSFER,ID_BARANG)
--                                     tidak ada query WHERE LOKASI saja di tabel ini
-- =============================================================================
CALL hapus_index_jika_ada('transfer_barang_detail', 'idx_lokasi_transfer_barang_detail');

-- =============================================================================
-- transfer_masuk_manual
-- idx_status_transfer_masuk_manual — duplikat dari idx_status (kolom sama: status_transfer)
-- =============================================================================
CALL hapus_index_jika_ada('transfer_masuk_manual', 'idx_status_transfer_masuk_manual');

DROP PROCEDURE IF EXISTS hapus_index_jika_ada;

-- =============================================================================
-- OPTIMIZE TABLE — rebuild semua tabel di database ini secara dinamis
-- Tidak hardcode nama tabel — otomatis cover semua tabel apapun kondisi database
-- CATATAN: OPTIMIZE TABLE akan lock tabel sementara — jalankan saat tidak ada user aktif
-- =============================================================================
SELECT '=== Mulai OPTIMIZE TABLE semua tabel — proses ini bisa memakan beberapa menit ===' AS status;

DROP PROCEDURE IF EXISTS optimize_semua_tabel;
DELIMITER $
CREATE PROCEDURE optimize_semua_tabel()
BEGIN
    DECLARE selesai INT DEFAULT 0;
    DECLARE nama_tabel VARCHAR(100);
    DECLARE cur CURSOR FOR
        SELECT table_name
        FROM information_schema.tables
        WHERE table_schema = DATABASE()
          AND table_type = 'BASE TABLE'
        ORDER BY (data_length + index_length) DESC;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET selesai = 1;

    OPEN cur;
    loop_tabel: LOOP
        FETCH cur INTO nama_tabel;
        IF selesai = 1 THEN LEAVE loop_tabel; END IF;
        SET @sql = CONCAT('OPTIMIZE TABLE `', nama_tabel, '`');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END LOOP;
    CLOSE cur;
END$
DELIMITER ;

CALL optimize_semua_tabel();
DROP PROCEDURE IF EXISTS optimize_semua_tabel;

-- =============================================================================
-- Refresh statistik information_schema — paksa MySQL update cache semua tabel
-- Dilakukan dinamis — tidak hardcode nama tabel
-- =============================================================================
SELECT '=== Refresh statistik semua tabel ===' AS status;

DROP PROCEDURE IF EXISTS analyze_semua_tabel;
DELIMITER $
CREATE PROCEDURE analyze_semua_tabel()
BEGIN
    DECLARE selesai INT DEFAULT 0;
    DECLARE nama_tabel VARCHAR(100);
    DECLARE cur CURSOR FOR
        SELECT table_name
        FROM information_schema.tables
        WHERE table_schema = DATABASE()
          AND table_type = 'BASE TABLE';
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET selesai = 1;

    OPEN cur;
    loop_tabel: LOOP
        FETCH cur INTO nama_tabel;
        IF selesai = 1 THEN LEAVE loop_tabel; END IF;
        SET @sql = CONCAT('ANALYZE TABLE `', nama_tabel, '`');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END LOOP;
    CLOSE cur;
END$
DELIMITER ;

CALL analyze_semua_tabel();
DROP PROCEDURE IF EXISTS analyze_semua_tabel;

-- =============================================================================
-- Verifikasi ukuran setelah migrasi
-- =============================================================================
SELECT
    table_name                                                    AS 'Tabel',
    ROUND((data_length + index_length) / 1024 / 1024, 2)         AS 'Ukuran_MB',
    ROUND(data_length  / 1024 / 1024, 2)                         AS 'Data_MB',
    ROUND(index_length / 1024 / 1024, 2)                         AS 'Index_MB',
    table_rows                                                    AS 'Jml_Baris'
FROM information_schema.tables
WHERE table_schema = 'db_kasirlancar'
ORDER BY (data_length + index_length) DESC
LIMIT 15;

SELECT
    ROUND(SUM(data_length + index_length) / 1024 / 1024, 2)  AS 'Total_MB',
    ROUND(SUM(data_length + index_length) / 1024 / 1024 / 1024, 4) AS 'Total_GB'
FROM information_schema.tables
WHERE table_schema = 'db_kasirlancar';

SELECT '=== 12_hapus_index_orphan selesai ===' AS status;
