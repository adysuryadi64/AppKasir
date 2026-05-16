-- ============================================================
-- 03_migrasi_index.sql (OPTIMASI)
-- Hanya menambah index jika belum ada → proses cepat saat dijalankan ulang
-- Kompatibel MySQL 8.0.17
-- ============================================================

DROP PROCEDURE IF EXISTS add_index_if_not_exists;
DELIMITER $
CREATE PROCEDURE add_index_if_not_exists(IN tbl VARCHAR(100), IN idx VARCHAR(100), IN cols TEXT)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = tbl
          AND INDEX_NAME   = idx
    ) THEN
        SET @sql = CONCAT('ALTER TABLE `', tbl, '` ADD INDEX `', idx, '` (', cols, ')');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$
DELIMITER ;

-- ============================================================
-- bon_karyawan
-- ============================================================
CALL add_index_if_not_exists('bon_karyawan', 'Bon_karyawan_ID_USER', 'ID_USER');
CALL add_index_if_not_exists('bon_karyawan', 'Bon_karyawan_Nama', 'NAMA');
CALL add_index_if_not_exists('bon_karyawan', 'idx_tanggal_bon', 'TANGGAL');
CALL add_index_if_not_exists('bon_karyawan', 'idx_tanggal_jenis_bon', 'TANGGAL,JENIS');
CALL add_index_if_not_exists('bon_karyawan', 'idx_kode_tanggal_bon', 'KODE,TANGGAL');
CALL add_index_if_not_exists('bon_karyawan', 'idx_kode_jenis_tanggal_bon', 'KODE,JENIS,TANGGAL');
CALL add_index_if_not_exists('bon_karyawan', 'idx_faktur_bon', 'FAKTUR');
CALL add_index_if_not_exists('bon_karyawan', 'idx_jenis_bon', 'JENIS');
CALL add_index_if_not_exists('bon_karyawan', 'idx_kode_jenis_bon', 'KODE,JENIS');
-- Optimal untuk FormLapBonPerorang: WHERE KODE=@k AND TANGGAL<@t AND JENIS='BON'
-- KODE (equality) → TANGGAL (range) → JENIS (covering) — lebih baik dari KODE,JENIS,TANGGAL
CALL add_index_if_not_exists('bon_karyawan', 'idx_kode_tanggal_jenis_bon', 'KODE,TANGGAL,JENIS');

-- ============================================================
-- gaji_karyawan
-- ============================================================
CALL add_index_if_not_exists('gaji_karyawan', 'idx_nomor_gaji', 'NOMOR');
CALL add_index_if_not_exists('gaji_karyawan', 'idx_bulan_gaji', 'BULAN');
CALL add_index_if_not_exists('gaji_karyawan', 'idx_kode_gaji', 'KODE');

-- ============================================================
-- hakaksesuser
-- ============================================================
CALL add_index_if_not_exists('hakaksesuser', 'idx_username_hakakses', 'UserName');
CALL add_index_if_not_exists('hakaksesuser', 'idx_username_role_hakakses', 'UserName,Role');
CALL add_index_if_not_exists('hakaksesuser', 'idx_username_module_hakakses', 'UserName,ModuleName');
CALL add_index_if_not_exists('hakaksesuser', 'idx_updated_at_hakakses', 'updated_at');
CALL add_index_if_not_exists('hakaksesuser', 'idx_role_hakakses', 'Role');

-- ============================================================
-- historybarang
-- ============================================================
-- HistoryBarang_ID_BARANG dihapus → redundan, prefix dari idx_barang_jenis_tgl (ID_BARANG,JENIS,TANGGAL)
CALL add_index_if_not_exists('historybarang', 'HistoryBarang_ID_USER', 'ID_USER');
CALL add_index_if_not_exists('historybarang', 'HistoryBarang_JENIS', 'JENIS');       -- DIPERTAHANKAN: bukan kolom pertama di composite manapun
CALL add_index_if_not_exists('historybarang', 'HistoryBarang_TANGGAL', 'TANGGAL');   -- DIPERTAHANKAN: tidak ada composite yang dimulai TANGGAL
-- HistoryBarang_LOKASI dihapus → redundan, prefix dari idx_lokasi_jenis_barang_qty (LOKASI,JENIS,...)
CALL add_index_if_not_exists('historybarang', 'idx_faktur_history', 'FAKTUR');
CALL add_index_if_not_exists('historybarang', 'idx_lokasi_jenis_barang_qty', 'LOKASI,JENIS,ID_BARANG,TOTAL_QTY');
CALL add_index_if_not_exists('historybarang', 'idx_barang_jenis_tgl', 'ID_BARANG,JENIS,TANGGAL');
CALL add_index_if_not_exists('historybarang', 'idx_barang_jenis_tgl_lokasi', 'ID_BARANG,JENIS,TANGGAL,LOKASI');
CALL add_index_if_not_exists('historybarang', 'idx_barang_lokasi_tgl', 'ID_BARANG,LOKASI,TANGGAL');

-- ============================================================
-- History
-- ============================================================
CALL add_index_if_not_exists('History', 'idx_tanggal_history', 'Tanggal');

-- ============================================================
-- hutang
-- ============================================================
CALL add_index_if_not_exists('hutang', 'idx_tgl_pembayaran_hutang', 'TGLPEMBAYARAN');
CALL add_index_if_not_exists('hutang', 'idx_nobayarhutang', 'NOBAYARHUTANG');
CALL add_index_if_not_exists('hutang', 'idx_tgl_supplier_hutang', 'TGLPEMBAYARAN,NAMASUPLIYER');
CALL add_index_if_not_exists('hutang', 'idx_namasupliyer_hutang', 'NAMASUPLIYER');
CALL add_index_if_not_exists('hutang', 'idx_tgl_lokasi_hutang', 'TGLPEMBAYARAN,LOKASI');

-- ============================================================
-- hutang_detail
-- ============================================================
CALL add_index_if_not_exists('hutang_detail', 'Hutang_Detail_ID_JUAL', 'ID_BELI');
CALL add_index_if_not_exists('hutang_detail', 'Hutang_Detail_ID_USER_BAYAR', 'ID_USER');
CALL add_index_if_not_exists('hutang_detail', 'idx_id_bayar_hutang', 'ID_BAYAR');

-- ============================================================
-- jurnalumum
-- ============================================================
CALL add_index_if_not_exists('jurnalumum', 'idx_no_transaksi_jurnal', 'NO_TRANSAKSI');
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_transaksi', 'TGL_TRANSAKSI,JENIS_TRANSAKSI');
CALL add_index_if_not_exists('jurnalumum', 'idx_nomor_akun_d_jurnal', 'NOMOR_AKUN_D,TGL_TRANSAKSI');
CALL add_index_if_not_exists('jurnalumum', 'idx_nomor_akun_k_jurnal', 'NOMOR_AKUN_K,TGL_TRANSAKSI');
CALL add_index_if_not_exists('jurnalumum', 'idx_covering_akun_d', 'TGL_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_covering_akun_k', 'TGL_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_id_user_jurnal', 'TGL_TRANSAKSI,ID_USER');
-- Untuk MAX(updated_at) di cek skip posting — tanpa index ini full scan 627K baris
CALL add_index_if_not_exists('jurnalumum', 'idx_updated_at_jurnal', 'updated_at');
-- Query: WHERE TGL_TRANSAKSI < @tgl AND NOMOR_AKUN_D <> '' GROUP BY NOMOR_AKUN_D → SUM(NOMINAL)
-- Tanpa index ini: full index scan 627K baris (~22 detik per query)
-- Dengan index ini: range scan + covering index (tidak baca row) → jauh lebih cepat
CALL add_index_if_not_exists('jurnalumum', 'idx_covering_akun_d', 'TGL_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_covering_akun_k', 'TGL_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
-- [DIHAPUS] prefix dari idx_tgl_jenis_akun_d_nominal — tidak ada query filter NOMINAL tanpa JENIS
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_akun_d_nominal', 'TGL_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
-- [DIHAPUS] prefix dari idx_tgl_jenis_akun_k_nominal — tidak ada query filter NOMINAL tanpa JENIS
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_akun_k_nominal', 'TGL_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
-- [DIHAPUS] tidak ada query WHERE NOMOR_AKUN_D + NOMINAL tanpa TGL di seluruh codebase
-- CALL add_index_if_not_exists('jurnalumum', 'idx_akun_d_nominal', 'NOMOR_AKUN_D,NOMINAL');
-- [DIHAPUS] tidak ada query WHERE NOMOR_AKUN_K + NOMINAL tanpa TGL di seluruh codebase
-- CALL add_index_if_not_exists('jurnalumum', 'idx_akun_k_nominal', 'NOMOR_AKUN_K,NOMINAL');
-- [DIHAPUS] query LoadRekapSekaliBaca pakai CASE WHEN — JENIS_TRANSAKSI tidak di WHERE, index tidak dipakai
-- query ExecuteQuery: optimizer pilih idx_nomor_akun_d_jurnal (equality NOMOR_AKUN_D lebih selektif)
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_akun_d_nominal', 'TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_D,NOMINAL');
-- [DIHAPUS] alasan sama untuk sisi NOMOR_AKUN_K — idx_nomor_akun_k_jurnal lebih optimal
-- CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_jenis_akun_k_nominal', 'TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_K,NOMINAL');
CALL add_index_if_not_exists('jurnalumum', 'idx_tgl_id_user_jurnal', 'TGL_TRANSAKSI,ID_USER');

-- ============================================================
-- pembelian
-- ============================================================
CALL add_index_if_not_exists('pembelian', 'idx_tgl_beli', 'TGL_BELI');
CALL add_index_if_not_exists('pembelian', 'idx_supplier_tagihan', 'ID_SUPPLIER,TAGIHAN');
CALL add_index_if_not_exists('pembelian', 'idx_id_supplier', 'ID_SUPPLIER');
CALL add_index_if_not_exists('pembelian', 'idx_status_transaksi_beli', 'STATUS_TRANSAKSI_BELI');
-- [DIHAPUS] hanya DISTINCT dropdown NAMA_SUPLIYER — bukan critical query
-- CALL add_index_if_not_exists('pembelian', 'idx_nama_supliyer', 'NAMA_SUPLIYER');
CALL add_index_if_not_exists('pembelian', 'idx_jenis_bayar', 'JENIS_BAYAR');
CALL add_index_if_not_exists('pembelian', 'idx_id_user_pembelian', 'ID_USER');
CALL add_index_if_not_exists('pembelian', 'idx_updated_at_beli', 'updated_at');
CALL add_index_if_not_exists('pembelian', 'idx_tgl_jenis_bayar', 'TGL_BELI,JENIS_BAYAR');
CALL add_index_if_not_exists('pembelian', 'idx_supplier_status', 'ID_SUPPLIER,STATUS_TRANSAKSI_BELI');
-- [DIHAPUS] prefix dari idx_jatuh_tempo_status_beli (JATUH_TEMPO,STATUS_TRANSAKSI_BELI)
-- CALL add_index_if_not_exists('pembelian', 'idx_jatuh_tempo_beli', 'JATUH_TEMPO');
CALL add_index_if_not_exists('pembelian', 'idx_jatuh_tempo_status_beli', 'JATUH_TEMPO,STATUS_TRANSAKSI_BELI');
-- DIPERTAHANKAN: dipakai di FormLapHutang mode BY PELUNASAN (WHERE TGL_BAYAR >= @a AND <= @b)
CALL add_index_if_not_exists('pembelian', 'idx_tgl_bayar_beli', 'TGL_BAYAR');
CALL add_index_if_not_exists('pembelian', 'idx_status_jual_beli', 'STATUS_JUAL');
CALL add_index_if_not_exists('pembelian', 'idx_status_lokasi_beli', 'STATUS_TRANSAKSI_BELI,LOKASI');
CALL add_index_if_not_exists('pembelian', 'idx_tgl_lokasi_beli', 'TGL_BELI,LOKASI');

-- ============================================================
-- pembelian_detail
-- ============================================================
CALL add_index_if_not_exists('pembelian_detail', 'idx_faktur_beli', 'FAKTUR_BELI');
CALL add_index_if_not_exists('pembelian_detail', 'idx_faktur_beli_barang', 'FAKTUR_BELI,ID_BARANG');
CALL add_index_if_not_exists('pembelian_detail', 'idx_tanggal_masuk_beli', 'TANGGAL_MASUK');
CALL add_index_if_not_exists('pembelian_detail', 'idx_tgl_masuk_barang', 'TANGGAL_MASUK,ID_BARANG');
CALL add_index_if_not_exists('pembelian_detail', 'idx_id_barang_tgl_masuk', 'ID_BARANG,TANGGAL_MASUK');
CALL add_index_if_not_exists('pembelian_detail', 'idx_id_barang_beli', 'ID_BARANG');

-- ============================================================
-- pembelian_ditahan
-- ============================================================
CALL add_index_if_not_exists('pembelian_ditahan', 'idx_id_pembelian_ditahan', 'ID_PEMBELIAN');
CALL add_index_if_not_exists('pembelian_ditahan', 'idx_lokasi_pembelian_ditahan', 'LOKASI');

-- ============================================================
-- penjualan
-- ============================================================
CALL add_index_if_not_exists('penjualan', 'idx_tgl_transaksi', 'TGL_TRANSAKSI');
CALL add_index_if_not_exists('penjualan', 'idx_pelanggan_tagihan', 'ID_PELANGGAN,SISA_TAGIHAN');
CALL add_index_if_not_exists('penjualan', 'idx_id_pelanggan', 'ID_PELANGGAN');
CALL add_index_if_not_exists('penjualan', 'idx_status_transaksi', 'STATUS_TRANSAKSI');
-- [DIHAPUS] hanya untuk DISTINCT dropdown + ORDER BY display — bukan critical query
-- CALL add_index_if_not_exists('penjualan', 'idx_nama_pelanggan_jual', 'NAMA_PELANGGAN');
CALL add_index_if_not_exists('penjualan', 'idx_lokasibarang', 'LOKASIBARANG');
-- [DIHAPUS] prefix dari idx_tgl_kode_akun_jual (TGL_TRANSAKSI,KODE_AKUN)
-- CALL add_index_if_not_exists('penjualan', 'idx_kode_akun_jual', 'KODE_AKUN');
CALL add_index_if_not_exists('penjualan', 'idx_id_user_penjualan', 'ID_USER');
CALL add_index_if_not_exists('penjualan', 'idx_updated_at_jual', 'updated_at');
CALL add_index_if_not_exists('penjualan', 'idx_tgl_kode_akun_jual', 'TGL_TRANSAKSI,KODE_AKUN');
CALL add_index_if_not_exists('penjualan', 'idx_tgl_kode_akun_tf', 'TGL_TRANSAKSI,KODE_AKUN_TF');
CALL add_index_if_not_exists('penjualan', 'idx_pelanggan_status', 'ID_PELANGGAN,STATUS_TRANSAKSI');
CALL add_index_if_not_exists('penjualan', 'idx_lokasi_tanggal', 'LOKASIBARANG,TGL_TRANSAKSI');
CALL add_index_if_not_exists('penjualan', 'idx_pelanggan_tgl_jual', 'ID_PELANGGAN,TGL_TRANSAKSI');
CALL add_index_if_not_exists('penjualan', 'idx_lokasi_tgl_pelanggan', 'LOKASIBARANG,TGL_TRANSAKSI,ID_PELANGGAN');
-- [DIHAPUS] prefix dari idx_jatuh_tempo_status_jual (JATUH_TEMPO,STATUS_TRANSAKSI)
-- CALL add_index_if_not_exists('penjualan', 'idx_jatuh_tempo_jual', 'JATUH_TEMPO');
CALL add_index_if_not_exists('penjualan', 'idx_jatuh_tempo_status_jual', 'JATUH_TEMPO,STATUS_TRANSAKSI');
-- [DIHAPUS] tidak ada query WHERE TGL_PEMBAYARAN ditemukan di seluruh codebase
-- CALL add_index_if_not_exists('penjualan', 'idx_tgl_pembayaran_jual', 'TGL_PEMBAYARAN');
-- [DIHAPUS] tidak ada query WHERE STATUS_BAYAR ditemukan di seluruh codebase
-- CALL add_index_if_not_exists('penjualan', 'idx_status_bayar_jual', 'STATUS_BAYAR');
-- [DIHAPUS] tidak ada query WHERE JENIS_PEMBAYARAN di seluruh codebase — kolom hanya di SELECT/display
-- CALL add_index_if_not_exists('penjualan', 'idx_jenis_pembayaran_jual', 'JENIS_PEMBAYARAN');
-- [DIHAPUS] hanya untuk DISTINCT dropdown ComboBox — bukan critical query, overhead INSERT tidak sepadan
-- CALL add_index_if_not_exists('penjualan', 'idx_nama_sales_jual', 'NAMA_SALES');
-- [DIHAPUS] prefix dari idx_id_sales_tgl_jual (ID_SALES,TGL_TRANSAKSI)
-- CALL add_index_if_not_exists('penjualan', 'idx_id_sales_jual', 'ID_SALES');
CALL add_index_if_not_exists('penjualan', 'idx_id_sales_tgl_jual', 'ID_SALES,TGL_TRANSAKSI');
CALL add_index_if_not_exists('penjualan', 'idx_type_akun_jual', 'TYPE_AKUN');
CALL add_index_if_not_exists('penjualan', 'idx_tgl_type_akun_jual', 'TGL_TRANSAKSI,TYPE_AKUN');
-- Covering index untuk subquery barang_lambat & reorder_alert:
-- JOIN dari penjualan_detail ON ID_PENJUALAN, filter TGL + LOKASI → satu index cover semua
CALL add_index_if_not_exists('penjualan', 'idx_id_tgl_lokasi', 'ID_PENJUALAN,TGL_TRANSAKSI,LOKASIBARANG');

-- ============================================================
-- penjualan_detail
-- ============================================================
CALL add_index_if_not_exists('penjualan_detail', 'idx_faktur_jual', 'FAKTUR_JUAL');
CALL add_index_if_not_exists('penjualan_detail', 'idx_faktur_barang', 'FAKTUR_JUAL,ID_BARANG');
CALL add_index_if_not_exists('penjualan_detail', 'idx_tgl_lokasi_jual', 'TANGGAL_JUAL,LOKASIBARANG');
CALL add_index_if_not_exists('penjualan_detail', 'idx_pelanggan_tgl_jual', 'ID_PELANGGAN,TANGGAL_JUAL');
CALL add_index_if_not_exists('penjualan_detail', 'idx_id_barang_detail_jual', 'ID_BARANG');
CALL add_index_if_not_exists('penjualan_detail', 'idx_tgl_pelanggan_user', 'TANGGAL_JUAL,NAMA_PELANGGAN,ID_USER');
-- Covering index untuk subquery barang_lambat & reorder_alert:
-- JOIN penjualan ON FAKTUR_JUAL, GROUP BY ID_BARANG → bisa full index scan
CALL add_index_if_not_exists('penjualan_detail', 'idx_barang_faktur', 'ID_BARANG,FAKTUR_JUAL');
-- margin_profit: filter TANGGAL_JUAL + LOKASIBARANG langsung di detail (tidak JOIN header)
CALL add_index_if_not_exists('penjualan_detail', 'idx_tgl_lokasi_barang', 'TANGGAL_JUAL,LOKASIBARANG,ID_BARANG');

-- ============================================================
-- penjualan_ditahan
-- ============================================================
CALL add_index_if_not_exists('penjualan_ditahan', 'idx_faktur_jual_ditahan', 'FAKTUR_JUAL');

-- ============================================================
-- penjualan_ditahan_detail
-- ============================================================
CALL add_index_if_not_exists('penjualan_ditahan_detail', 'idx_faktur_jual_ditahan_detail', 'FAKTUR_JUAL');
CALL add_index_if_not_exists('penjualan_ditahan_detail', 'idx_id_barang_ditahan_detail', 'ID_BARANG');

-- ============================================================
-- piutang
-- ============================================================
CALL add_index_if_not_exists('piutang', 'idx_tgl_bayar_piutang', 'TGL_BAYAR');
CALL add_index_if_not_exists('piutang', 'idx_id_bayar_piutang', 'ID_BAYAR_PIUTANG');
CALL add_index_if_not_exists('piutang', 'idx_tgl_pelanggan_piutang', 'TGL_BAYAR,NAMA_PELANGGAN');
CALL add_index_if_not_exists('piutang', 'idx_nama_pelanggan_piutang', 'NAMA_PELANGGAN');
CALL add_index_if_not_exists('piutang', 'idx_tgl_lokasi_piutang', 'TGL_BAYAR,LOKASI');

-- ============================================================
-- piutang_detail
-- ============================================================
CALL add_index_if_not_exists('piutang_detail', 'idx_id_bayar_piutang_detail', 'ID_BAYAR');
CALL add_index_if_not_exists('piutang_detail', 'idx_id_jual', 'ID_JUAL');

-- ============================================================
-- retur_pembelian
-- ============================================================
CALL add_index_if_not_exists('retur_pembelian', 'retur_pembelian_ID_KOMPUTER', 'ID_KOMPUTER');
CALL add_index_if_not_exists('retur_pembelian', 'retur_pembelian_ID_PEMBELIAN', 'ID_PEMBELIAN');
CALL add_index_if_not_exists('retur_pembelian', 'retur_pembelian_ID_RETUR_PEMBELIAN', 'ID_RETUR_PEMBELIAN');
CALL add_index_if_not_exists('retur_pembelian', 'retur_pembelian_ID_SUPPLIER', 'ID_SUPPLIER');
CALL add_index_if_not_exists('retur_pembelian', 'idx_tgl_retur_beli', 'TGL_RETUR_BELI');
CALL add_index_if_not_exists('retur_pembelian', 'idx_kode_rekening_retur_beli', 'KODE_REKENING');
CALL add_index_if_not_exists('retur_pembelian', 'idx_id_user_retur_beli', 'ID_USER');
-- [DIHAPUS] hanya DISTINCT dropdown NAMA_REKENING — bukan critical query
-- CALL add_index_if_not_exists('retur_pembelian', 'idx_nama_rekening_retur_beli', 'NAMA_REKENING');

-- ============================================================
-- retur_pembelian_detail
-- ============================================================
CALL add_index_if_not_exists('retur_pembelian_detail', 'idx_id_retur_pembelian', 'ID_RETUR_PEMBELIAN');
CALL add_index_if_not_exists('retur_pembelian_detail', 'idx_tgl_retur_beli_detail', 'TGL_RETUR_BELI');
CALL add_index_if_not_exists('retur_pembelian_detail', 'idx_tgl_supplier_retur_beli', 'TGL_RETUR_BELI,NAMA_SUPLIYER');
CALL add_index_if_not_exists('retur_pembelian_detail', 'idx_penyimpanan_retur_beli_detail', 'PENYIMPANAN');

-- ============================================================
-- retur_penjualan
-- ============================================================
CALL add_index_if_not_exists('retur_penjualan', 'idx_tgl_retur_jual', 'TGL_RETUR_JUAL');
CALL add_index_if_not_exists('retur_penjualan', 'idx_id_penjualan_retur', 'ID_PENJUALAN');
CALL add_index_if_not_exists('retur_penjualan', 'idx_id_retur_penjualan_header', 'ID_RETUR_PENJUALAN');
CALL add_index_if_not_exists('retur_penjualan', 'idx_kode_rekening_retur_jual', 'KODE_REKENING');
CALL add_index_if_not_exists('retur_penjualan', 'idx_id_user_retur_jual', 'ID_USER');
-- [DIHAPUS] hanya DISTINCT dropdown NAMA_REKENING — bukan critical query
-- CALL add_index_if_not_exists('retur_penjualan', 'idx_nama_rekening_retur_jual', 'NAMA_REKENING');

-- ============================================================
-- retur_penjualan_detail
-- ============================================================
CALL add_index_if_not_exists('retur_penjualan_detail', 'idx_id_retur_penjualan', 'ID_RETUR_PENJUALAN');
CALL add_index_if_not_exists('retur_penjualan_detail', 'idx_retur_jual_barang', 'ID_RETUR_PENJUALAN,ID_BARANG');
CALL add_index_if_not_exists('retur_penjualan_detail', 'idx_tgl_retur_jual_detail', 'TGL_RETUR_JUAL');
CALL add_index_if_not_exists('retur_penjualan_detail', 'idx_tgl_pelanggan_retur_jual', 'TGL_RETUR_JUAL,NAMA_PELANGGAN');
CALL add_index_if_not_exists('retur_penjualan_detail', 'idx_lokasi_retur_jual_detail', 'LOKASI');

-- ============================================================
-- stok_opname
-- ============================================================
CALL add_index_if_not_exists('stok_opname', 'idx_tanggal_opname', 'TANGGAL');
CALL add_index_if_not_exists('stok_opname', 'idx_id_stok_opname', 'ID_STOK_OPNAME');
CALL add_index_if_not_exists('stok_opname', 'idx_id_barang_opname', 'ID_BARANG');
-- [DIHAPUS] query pakai OR (TANGGAL >= @a OR ID_USER LIKE @u) — index tidak efektif untuk kondisi OR
-- CALL add_index_if_not_exists('stok_opname', 'idx_id_user_opname', 'ID_USER');
CALL add_index_if_not_exists('stok_opname', 'idx_barang_tanggal', 'ID_BARANG,TANGGAL');
CALL add_index_if_not_exists('stok_opname', 'idx_tanggal_lokasi_opname', 'TANGGAL,LOKASI');
CALL add_index_if_not_exists('stok_opname', 'idx_lokasi_tanggal_opname', 'LOKASI,TANGGAL');

-- ============================================================
-- StokTambahKurang
-- ============================================================
CALL add_index_if_not_exists('StokTambahKurang', 'idx_tanggal_stok_tk', 'TANGGAL');
CALL add_index_if_not_exists('StokTambahKurang', 'idx_lokasi_stok_tk', 'LOKASI');
CALL add_index_if_not_exists('StokTambahKurang', 'idx_id_barang_stok_tk', 'ID_BARANG');
CALL add_index_if_not_exists('StokTambahKurang', 'idx_faktur_stok_tk', 'FAKTUR');

-- ============================================================
-- surat_jalan
-- ============================================================
CALL add_index_if_not_exists('surat_jalan', 'idx_tgl_pengiriman', 'TGL_PENGIRIMAN');
CALL add_index_if_not_exists('surat_jalan', 'idx_nota_sj', 'NOTA');
CALL add_index_if_not_exists('surat_jalan', 'idx_kode_supir_tgl', 'KODE_SUPIR,TGL_PENGIRIMAN');
CALL add_index_if_not_exists('surat_jalan', 'idx_kode_helper1_tgl', 'KODE_HELPER1,TGL_PENGIRIMAN');
CALL add_index_if_not_exists('surat_jalan', 'idx_kode_helper2_tgl', 'KODE_HELPER2,TGL_PENGIRIMAN');

-- ============================================================
-- surat_jalan_detail
-- ============================================================
CALL add_index_if_not_exists('surat_jalan_detail', 'idx_nota_sj_detail', 'NOTA');

-- ============================================================
-- sync_log
-- ============================================================
CALL add_index_if_not_exists('sync_log', 'idx_waktu_log', 'waktu');
CALL add_index_if_not_exists('sync_log', 'idx_jenis_log', 'jenis');

-- ============================================================
-- sync_queue
-- ============================================================
CALL add_index_if_not_exists('sync_queue', 'idx_status_queue', 'status');
CALL add_index_if_not_exists('sync_queue', 'idx_tabel_queue', 'tabel');

-- ============================================================
-- tbl_armada
-- ============================================================
CALL add_index_if_not_exists('tbl_armada', 'idx_nopol_armada', 'NOPOL');
CALL add_index_if_not_exists('tbl_armada', 'idx_updated_at_armada', 'updated_at');
CALL add_index_if_not_exists('tbl_armada', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_armada', 'idx_id_cloud', 'id_cloud');

-- ============================================================
-- tbl_barang
-- ============================================================
CALL add_index_if_not_exists('tbl_barang', 'idx_nama_barang', 'NAMA_BARANG');
CALL add_index_if_not_exists('tbl_barang', 'idx_barcode_kecil', 'BARCODE_KECIL');
CALL add_index_if_not_exists('tbl_barang', 'idx_barcode_sedang', 'BARCODE_SEDANG');
CALL add_index_if_not_exists('tbl_barang', 'idx_barcode_besar', 'BARCODE_BESAR');
CALL add_index_if_not_exists('tbl_barang', 'idx_updated_at_barang', 'updated_at');
CALL add_index_if_not_exists('tbl_barang', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_barang', 'idx_id_cloud', 'id_cloud');
CALL add_index_if_not_exists('tbl_barang', 'idx_stok_minimum', 'STOK_MIN,STOK_TOKO,STOK_GUDANG');
-- [DIHAPUS] prefix dari idx_stok_minimum (STOK_MIN,STOK_TOKO,STOK_GUDANG)
-- CALL add_index_if_not_exists('tbl_barang', 'idx_stok_toko_gudang', 'STOK_TOKO,STOK_GUDANG');
CALL add_index_if_not_exists('tbl_barang', 'idx_kategori_barang', 'NAMA_KATEGORI');
CALL add_index_if_not_exists('tbl_barang', 'idx_kode_kategori_barang', 'KODE_KATEGORI');
CALL add_index_if_not_exists('tbl_barang', 'idx_status_barang', 'STATUS');
CALL add_index_if_not_exists('tbl_barang', 'idx_status_nama_barang', 'STATUS,NAMA_BARANG');
-- [DIHAPUS] duplikat PRIMARY KEY — optimizer selalu pilih PK, index ini tidak pernah dipakai
-- CALL add_index_if_not_exists('tbl_barang', 'idx_id_barang_prefix', 'ID_BARANG');
-- Covering index untuk barang_lambat & reorder_alert:
-- filter STOK_TOKO/STOK_GUDANG > 0, ambil ID_BARANG, NAMA_BARANG, HARGA_BELI
CALL add_index_if_not_exists('tbl_barang', 'idx_stok_toko_id_nama_harga', 'STOK_TOKO,ID_BARANG,NAMA_BARANG,HARGA_BELI');
CALL add_index_if_not_exists('tbl_barang', 'idx_stok_gudang_id_nama_harga', 'STOK_GUDANG,ID_BARANG,NAMA_BARANG,HARGA_BELI');

-- ============================================================
-- tbl_datareferensi
-- ============================================================
CALL add_index_if_not_exists('tbl_datareferensi', 'idx_nama_akun', 'NAMA_AKUN');
CALL add_index_if_not_exists('tbl_datareferensi', 'idx_type_akun', 'TYPE_AKUN');
CALL add_index_if_not_exists('tbl_datareferensi', 'idx_kode_akun_ref', 'KODE_AKUN');
CALL add_index_if_not_exists('tbl_datareferensi', 'idx_sub_akun', 'SUB_AKUN');
-- [DIHAPUS] tidak ada query WHERE JENIS_AKUN ditemukan di seluruh codebase
-- CALL add_index_if_not_exists('tbl_datareferensi', 'idx_jenis_akun', 'JENIS_AKUN');

-- ============================================================
-- tbl_karyawan
-- ============================================================
CALL add_index_if_not_exists('tbl_karyawan', 'idx_nama_karyawan', 'NAMA');
CALL add_index_if_not_exists('tbl_karyawan', 'idx_status_nama', 'Status,Nama');
CALL add_index_if_not_exists('tbl_karyawan', 'idx_kode_karyawan', 'Kode');
CALL add_index_if_not_exists('tbl_karyawan', 'idx_saldo_akhir_karyawan', 'SaldoAkhir');

-- ============================================================
-- tbl_kategori
-- ============================================================
CALL add_index_if_not_exists('tbl_kategori', 'idx_updated_at_kategori', 'updated_at');
CALL add_index_if_not_exists('tbl_kategori', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_kategori', 'idx_id_cloud', 'id_cloud');
CALL add_index_if_not_exists('tbl_kategori', 'idx_nama_kategori', 'nama');
CALL add_index_if_not_exists('tbl_kategori', 'idx_kode_kategori', 'kode');

-- ============================================================
-- tbl_merk
-- ============================================================
CALL add_index_if_not_exists('tbl_merk', 'idx_updated_at_merk', 'updated_at');
CALL add_index_if_not_exists('tbl_merk', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_merk', 'idx_id_cloud', 'id_cloud');
CALL add_index_if_not_exists('tbl_merk', 'idx_nama_merk', 'nama');
CALL add_index_if_not_exists('tbl_merk', 'idx_kode_merk', 'kode');

-- ============================================================
-- tbl_pelanggan
-- ============================================================
CALL add_index_if_not_exists('tbl_pelanggan', 'idx_nama_pelanggan', 'NAMA');
CALL add_index_if_not_exists('tbl_pelanggan', 'idx_updated_at_pelanggan', 'updated_at');
CALL add_index_if_not_exists('tbl_pelanggan', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_pelanggan', 'idx_id_cloud', 'id_cloud');
CALL add_index_if_not_exists('tbl_pelanggan', 'idx_status_pelanggan', 'Status');
CALL add_index_if_not_exists('tbl_pelanggan', 'idx_status_nama_pelanggan', 'Status,NAMA');

-- ============================================================
-- tbl_satuan
-- ============================================================
CALL add_index_if_not_exists('tbl_satuan', 'idx_nama_satuan', 'NAMA');
CALL add_index_if_not_exists('tbl_satuan', 'idx_updated_at_satuan', 'updated_at');
CALL add_index_if_not_exists('tbl_satuan', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_satuan', 'idx_id_cloud', 'id_cloud');
-- Gap index: TambahSatuan.vb WHERE kode = @Kode dan ORDER BY isi
CALL add_index_if_not_exists('tbl_satuan', 'idx_kode_satuan', 'kode');
CALL add_index_if_not_exists('tbl_satuan', 'idx_isi_satuan', 'isi');

-- ============================================================
-- tbl_supliyer
-- ============================================================
CALL add_index_if_not_exists('tbl_supliyer', 'idx_nama_supliyer', 'NAMA');
CALL add_index_if_not_exists('tbl_supliyer', 'idx_status_nama', 'Status, NAMA');
CALL add_index_if_not_exists('tbl_supliyer', 'idx_updated_at_supliyer', 'updated_at');
CALL add_index_if_not_exists('tbl_supliyer', 'idx_is_dirty', 'is_dirty');
CALL add_index_if_not_exists('tbl_supliyer', 'idx_id_cloud', 'id_cloud');

-- ============================================================
-- tbl_user
-- ============================================================
CALL add_index_if_not_exists('tbl_user', 'idx_status_user', 'status');
CALL add_index_if_not_exists('tbl_user', 'idx_username_user', 'user_name');
CALL add_index_if_not_exists('tbl_user', 'idx_username_pwd_status', 'user_name,pwd,status');

-- ============================================================
-- transfer_barang
-- ============================================================
CALL add_index_if_not_exists('transfer_barang', 'idx_tgl_transfer_barang', 'TGL_TRANSFER');

-- ============================================================
-- transfer_barang_detail
-- ============================================================
CALL add_index_if_not_exists('transfer_barang_detail', 'idx_id_transfer_barang_detail', 'ID_TRANSFER');
CALL add_index_if_not_exists('transfer_barang_detail', 'idx_id_barang_transfer', 'ID_BARANG');
CALL add_index_if_not_exists('transfer_barang_detail', 'idx_transfer_barang_id', 'ID_TRANSFER,ID_BARANG');
CALL add_index_if_not_exists('transfer_barang_detail', 'idx_tgl_transfer_detail', 'TGL_TRANSFER');

-- ============================================================
-- transfer_cabang
-- ============================================================
CALL add_index_if_not_exists('transfer_cabang', 'idx_tgl_transfer_cabang', 'TGL_TRANSFER');
CALL add_index_if_not_exists('transfer_cabang', 'idx_status_transfer_cabang', 'STATUS_TRANSFER');
CALL add_index_if_not_exists('transfer_cabang', 'idx_mode_kirim_cabang', 'MODE_KIRIM');
CALL add_index_if_not_exists('transfer_cabang', 'idx_dari_ke_cabang', 'DARI_CABANG,KE_CABANG');
CALL add_index_if_not_exists('transfer_cabang', 'idx_ke_status_cabang', 'KE_CABANG,STATUS_TRANSFER');
CALL add_index_if_not_exists('transfer_cabang', 'idx_cloud_transfer_cabang', 'ID_CLOUD_TRANSFER');

-- ============================================================
-- transfer_cabang_detail
-- ============================================================
CALL add_index_if_not_exists('transfer_cabang_detail', 'idx_id_transfer_cabang_detail', 'ID_TRANSFER');
CALL add_index_if_not_exists('transfer_cabang_detail', 'idx_id_barang_transfer_cabang', 'ID_BARANG');
CALL add_index_if_not_exists('transfer_cabang_detail', 'idx_transfer_cabang_id_barang', 'ID_TRANSFER,ID_BARANG');
CALL add_index_if_not_exists('transfer_cabang_detail', 'idx_tgl_transfer_cabang_detail', 'TGL_TRANSFER');
CALL add_index_if_not_exists('transfer_cabang_detail', 'idx_status_item_transfer_cabang', 'STATUS_ITEM');

-- ============================================================
-- transfer_masuk_manual (antar cabang)
-- ============================================================
CALL add_index_if_not_exists('transfer_masuk_manual', 'idx_id_transfer_masuk_manual', 'id_transfer');
CALL add_index_if_not_exists('transfer_masuk_manual', 'idx_status_transfer_masuk_manual', 'status_transfer');
CALL add_index_if_not_exists('transfer_masuk_manual', 'idx_ke_status_transfer_masuk_manual', 'ke_cabang,status_transfer');
CALL add_index_if_not_exists('transfer_masuk_manual', 'idx_kode_barang_transfer_masuk_manual', 'kode_barang');

-- ============================================================
-- tbl_cabang
-- ============================================================
CALL add_index_if_not_exists('tbl_cabang', 'idx_nama_cabang', 'nama_cabang');
CALL add_index_if_not_exists('tbl_cabang', 'idx_sumber_cabang', 'sumber');
CALL add_index_if_not_exists('tbl_cabang', 'idx_updated_at_cabang', 'updated_at');
CALL add_index_if_not_exists('tbl_cabang', 'idx_id_cloud_cabang', 'id_cloud');

-- ============================================================
-- transfer_stok
-- ============================================================
CALL add_index_if_not_exists('transfer_stok', 'idx_tanggal_transfer_stok', 'TANGGAL');
CALL add_index_if_not_exists('transfer_stok', 'idx_id_transfer_stok', 'ID_TRANSFER');
-- Untuk HapusTransaksi batch delete WHERE JENIS_TRANSFER = 'TOKO'/'GUDANG'
CALL add_index_if_not_exists('transfer_stok', 'idx_jenis_transfer', 'JENIS_TRANSFER');

-- ============================================================
-- Index tambahan untuk HapusTransaksi (batch DELETE WHERE LOKASI = ?)
-- Tabel-tabel ini belum punya single-column index pada LOKASI
-- ============================================================

-- penjualan: sudah ada idx_lokasibarang — tambah alias nama idx_hapus agar EnsureIndex tidak buat duplikat
-- (EnsureIndex cek by INDEX_NAME, bukan by kolom — pakai nama yang sama dengan yang sudah ada)
-- → tidak perlu tambah, cukup update EnsureIndex di VB (lihat catatan di bawah)

-- penjualan_detail: idx_tgl_lokasi_jual (TANGGAL_JUAL,LOKASIBARANG) sudah ada tapi LOKASIBARANG posisi 2
-- Tambah single-column agar WHERE LOKASIBARANG = ? efisien
CALL add_index_if_not_exists('penjualan_detail', 'idx_lokasibarang_detail', 'LOKASIBARANG');

-- transfer_barang: belum ada index LOKASI sama sekali
CALL add_index_if_not_exists('transfer_barang', 'idx_lokasi_transfer_barang', 'LOKASI');

-- transfer_barang_detail: belum ada index LOKASI
CALL add_index_if_not_exists('transfer_barang_detail', 'idx_lokasi_transfer_barang_detail', 'LOKASI');

-- transfer_cabang: belum ada index LOKASI
CALL add_index_if_not_exists('transfer_cabang', 'idx_lokasi_transfer_cabang', 'LOKASI');

-- transfer_cabang_detail: belum ada index LOKASI
CALL add_index_if_not_exists('transfer_cabang_detail', 'idx_lokasi_transfer_cabang_detail', 'LOKASI');

-- surat_jalan: belum ada index LOKASI
CALL add_index_if_not_exists('surat_jalan', 'idx_lokasi_surat_jalan', 'LOKASI');

-- surat_jalan_detail: belum ada index LOKASI
CALL add_index_if_not_exists('surat_jalan_detail', 'idx_lokasi_surat_jalan_detail', 'LOKASI');

-- jurnalumum: belum ada index LOKASI
CALL add_index_if_not_exists('jurnalumum', 'idx_lokasi_jurnal', 'LOKASI');

-- hutang_detail: belum ada index LOKASI
CALL add_index_if_not_exists('hutang_detail', 'idx_lokasi_hutang_detail', 'LOKASI');

-- hutang: idx_tgl_lokasi_hutang ada tapi LOKASI posisi 2 — tambah single-column
CALL add_index_if_not_exists('hutang', 'idx_lokasi_hutang', 'LOKASI');

-- piutang: idx_tgl_lokasi_piutang ada tapi LOKASI posisi 2 — tambah single-column
CALL add_index_if_not_exists('piutang', 'idx_lokasi_piutang', 'LOKASI');

-- piutang_detail: belum ada index LOKASI
CALL add_index_if_not_exists('piutang_detail', 'idx_lokasi_piutang_detail', 'LOKASI');

-- bon_karyawan: belum ada index LOKASI (ada idx_kode_jenis_bon dll tapi bukan LOKASI)
CALL add_index_if_not_exists('bon_karyawan', 'idx_lokasi_bon_karyawan', 'LOKASI');

-- gaji_karyawan: belum ada index LOKASI
CALL add_index_if_not_exists('gaji_karyawan', 'idx_lokasi_gaji_karyawan', 'LOKASI');

-- ============================================================
-- Bersihkan helper procedures
-- ============================================================
DROP PROCEDURE IF EXISTS add_index_if_not_exists;

SELECT 'Migrasi index selesai (hanya menambah jika belum ada).' AS status;
