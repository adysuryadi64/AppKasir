-- ============================================================
-- 04_migrasi_collation.sql
-- Samakan collation semua tabel ke utf8mb4_unicode_ci
-- Kompatibel MySQL 5.7 / 8.0
-- PERINGATAN: Jalankan saat tidak ada user aktif.
--             CONVERT TO akan rebuild semua index di tabel.
-- Aman dijalankan berulang kali (idempotent).
-- Tabel yang tidak ada akan dilewati otomatis.
-- Jalankan setelah: USE nama_database;
-- ============================================================

ALTER DATABASE CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Helper procedure: CONVERT TO hanya jika tabel ada
DROP PROCEDURE IF EXISTS ConvertCollationIfExists;
DELIMITER $
CREATE PROCEDURE ConvertCollationIfExists(IN tbl VARCHAR(64))
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.TABLES
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND TABLE_TYPE   = 'BASE TABLE'
    ) THEN
        SET @s = CONCAT('ALTER TABLE `', tbl, '` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci');
        PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;
    END IF;
END$
DELIMITER ;

-- Tabel transaksi
CALL ConvertCollationIfExists('bon_karyawan');
CALL ConvertCollationIfExists('gaji_karyawan');
CALL ConvertCollationIfExists('hakaksesuser');
CALL ConvertCollationIfExists('history');
CALL ConvertCollationIfExists('historybarang');
CALL ConvertCollationIfExists('hutang');
CALL ConvertCollationIfExists('hutang_detail');
CALL ConvertCollationIfExists('jurnalumum');
CALL ConvertCollationIfExists('pembelian');
CALL ConvertCollationIfExists('pembelian_detail');
CALL ConvertCollationIfExists('pembelian_ditahan');
CALL ConvertCollationIfExists('pembelian_ditahan_detail');
CALL ConvertCollationIfExists('penjualan');
CALL ConvertCollationIfExists('penjualan_detail');
CALL ConvertCollationIfExists('penjualan_ditahan');
CALL ConvertCollationIfExists('penjualan_ditahan_detail');
CALL ConvertCollationIfExists('piutang');
CALL ConvertCollationIfExists('piutang_detail');
CALL ConvertCollationIfExists('retur_pembelian');
CALL ConvertCollationIfExists('retur_pembelian_detail');
CALL ConvertCollationIfExists('retur_penjualan');
CALL ConvertCollationIfExists('retur_penjualan_detail');
CALL ConvertCollationIfExists('saldo_tahunan');
CALL ConvertCollationIfExists('stoktambahkurang');
CALL ConvertCollationIfExists('stok_opname');
CALL ConvertCollationIfExists('surat_jalan');
CALL ConvertCollationIfExists('surat_jalan_detail');
CALL ConvertCollationIfExists('transfer_barang');
CALL ConvertCollationIfExists('transfer_barang_detail');
CALL ConvertCollationIfExists('transfer_cabang');
CALL ConvertCollationIfExists('transfer_cabang_detail');
CALL ConvertCollationIfExists('transfer_masuk_manual');
CALL ConvertCollationIfExists('transfer_stok');
CALL ConvertCollationIfExists('tukarbarang');

-- Tabel master
CALL ConvertCollationIfExists('tbl_armada');
CALL ConvertCollationIfExists('tbl_barang');
CALL ConvertCollationIfExists('tbl_datareferensi');
CALL ConvertCollationIfExists('tbl_gaji');
CALL ConvertCollationIfExists('tbl_karyawan');
CALL ConvertCollationIfExists('tbl_kategori');
CALL ConvertCollationIfExists('tbl_merk');
CALL ConvertCollationIfExists('tbl_pelanggan');
CALL ConvertCollationIfExists('tbl_perusahaan');
CALL ConvertCollationIfExists('tbl_satuan');
CALL ConvertCollationIfExists('tbl_supliyer');
CALL ConvertCollationIfExists('tbl_user');
CALL ConvertCollationIfExists('tbl_cabang');

-- Tabel temp
CALL ConvertCollationIfExists('tempbukubesarpembantu');
CALL ConvertCollationIfExists('tempjurnalumum');
CALL ConvertCollationIfExists('temp_bon_karyawan');
CALL ConvertCollationIfExists('temp_datareferensi');
CALL ConvertCollationIfExists('temp_jurnal');
CALL ConvertCollationIfExists('temp_labarugi');
CALL ConvertCollationIfExists('temp_loading');
CALL ConvertCollationIfExists('temp_mutasi_barang');
CALL ConvertCollationIfExists('temp_supliyerbayar');
CALL ConvertCollationIfExists('temp_supliyerhutang');

-- Tabel sync
CALL ConvertCollationIfExists('sync_queue');
CALL ConvertCollationIfExists('sync_log');
CALL ConvertCollationIfExists('sync_config');

-- Bersihkan procedure
DROP PROCEDURE IF EXISTS ConvertCollationIfExists;

-- Verifikasi (seharusnya 0 baris jika semua berhasil)
SELECT TABLE_NAME, TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_COLLATION != 'utf8mb4_unicode_ci'
  AND TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

SELECT 'Migrasi collation selesai.' AS status;
