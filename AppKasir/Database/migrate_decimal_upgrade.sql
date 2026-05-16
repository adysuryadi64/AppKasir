-- =============================================================================
-- Migrasi: Upgrade semua kolom decimal(10,x) dan decimal(10,0)
-- Tujuan : Mencegah overflow pada transaksi volume besar
-- Aturan :
--   Kolom harga/nilai rupiah  → decimal(15,2)
--   Kolom qty/stok            → decimal(15,4)
--   Kolom nominal bulat (0)   → decimal(15,0)
--   Kolom persen              → decimal(10,2) — TIDAK diubah, sudah cukup
-- =============================================================================

-- =============================================================================
-- gaji_karyawan
-- =============================================================================
ALTER TABLE gaji_karyawan
    MODIFY ABSEN               decimal(15,0) NULL DEFAULT 0,
    MODIFY ABSEN_KHUSUS        decimal(15,0) NULL DEFAULT 0,
    MODIFY ABSEN_KHUSUS_RP     decimal(15,0) NULL DEFAULT 0,
    MODIFY ABSEN_RP            decimal(15,0) NULL DEFAULT 0,
    MODIFY ANGSURAN            decimal(15,0) NULL DEFAULT 0,
    MODIFY HELPER_RP           decimal(15,0) NULL DEFAULT 0,
    MODIFY KOMISI_JUAL         decimal(15,0) NULL DEFAULT 0,
    MODIFY LEMBUR              decimal(15,0) NULL DEFAULT 0,
    MODIFY LEMBUR_RP           decimal(15,0) NULL DEFAULT 0,
    MODIFY NILAI_POTONGAN_ABSEN decimal(15,0) NULL DEFAULT 0,
    MODIFY POT_BON             decimal(15,0) NULL DEFAULT 0,
    MODIFY POT_LAIN            decimal(15,0) NULL DEFAULT 0,
    MODIFY SALDO_BON           decimal(15,0) NULL DEFAULT 0,
    MODIFY SUPIR_RP            decimal(15,0) NULL DEFAULT 0,
    MODIFY TERLAMBAT           decimal(15,0) NULL DEFAULT 0,
    MODIFY TERLAMBAT_RP        decimal(15,0) NULL DEFAULT 0,
    MODIFY TRANSP              decimal(15,0) NULL DEFAULT 0,
    MODIFY TRANSPORT           decimal(15,0) NULL DEFAULT 0,
    MODIFY TUNJANGAN           decimal(15,0) NULL DEFAULT 0,
    MODIFY UANG_MAKAN          decimal(15,0) NULL DEFAULT 0,
    MODIFY UANG_MKN            decimal(15,0) NULL DEFAULT 0;

-- =============================================================================
-- historybarang
-- =============================================================================
ALTER TABLE historybarang
    MODIFY QTY       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- pembelian
-- =============================================================================
ALTER TABLE pembelian
    MODIFY TOTAL_BARANG decimal(15,2) NULL DEFAULT 0.00,
    MODIFY TOTAL_QTY    decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- pembelian_detail
-- =============================================================================
ALTER TABLE pembelian_detail
    MODIFY HARGA_BELI          decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_AVERAGE       decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_SATUAN   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_SEBELUMNYA decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY                 decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SAT             decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- pembelian_ditahan
-- =============================================================================
ALTER TABLE pembelian_ditahan
    MODIFY RETUR        decimal(15,2) NULL DEFAULT 0.00,
    MODIFY TOTAL_BARANG decimal(15,2) NULL DEFAULT 0.00,
    MODIFY TOTAL_QTY    decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- pembelian_ditahan_detail
-- =============================================================================
ALTER TABLE pembelian_ditahan_detail
    MODIFY HARGA_BELI          decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_AVERAGE       decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_SATUAN   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_SEBELUMNYA decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY                 decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SAT             decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL               decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- penjualan
-- =============================================================================
ALTER TABLE penjualan
    MODIFY BIAYA_KIRIM         decimal(15,2) NULL DEFAULT 0.00,
    MODIFY DISKON_TOTAL_PERSEN decimal(10,2) NULL DEFAULT 0.00,
    MODIFY DISKON_TOTAL_RP     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY LABA                decimal(15,2) NULL DEFAULT 0.00,
    MODIFY NILAI_RETUR         decimal(15,2) NULL DEFAULT 0.00,
    MODIFY PAJAK_PERSEN        decimal(10,2) NULL DEFAULT 0.00,
    MODIFY PAJAK_RP            decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- penjualan_detail
-- =============================================================================
ALTER TABLE penjualan_detail
    MODIFY DISKON_PERSEN decimal(10,2) NULL DEFAULT 0.00,
    MODIFY DISKON_RP     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY LABA          decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY           decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SATUAN    decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_DISKON  decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- penjualan_ditahan
-- =============================================================================
ALTER TABLE penjualan_ditahan
    MODIFY TOTAL_ITEM decimal(15,2) NULL DEFAULT 0.00,
    MODIFY TOTAL_QTY  decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- penjualan_ditahan_detail
-- =============================================================================
ALTER TABLE penjualan_ditahan_detail
    MODIFY DISKON_PERSEN decimal(10,2) NULL DEFAULT 0.00,
    MODIFY DISKON_RP     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY GUDANG        decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY HARGA_BELI    decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL    decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY           decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SATUAN    decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY SISA          decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK          decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOKO          decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_DISKON  decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- retur_pembelian
-- =============================================================================
ALTER TABLE retur_pembelian
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- retur_pembelian_detail
-- =============================================================================
ALTER TABLE retur_pembelian_detail
    MODIFY QTY     decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SAT decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- retur_penjualan
-- =============================================================================
ALTER TABLE retur_penjualan
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- retur_penjualan_detail
-- =============================================================================
ALTER TABLE retur_penjualan_detail
    MODIFY HARGA_BELI   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY LABA         decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY          decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SATUAN   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_DISKON decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- stok_opname
-- =============================================================================
ALTER TABLE stok_opname
    MODIFY HARGA       decimal(15,2) NULL DEFAULT 0.00,
    MODIFY STOK_NYATA  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_SELISIH decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_SYSTEM decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_QTY   decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- stoktambahkurang
-- =============================================================================
ALTER TABLE stoktambahkurang
    MODIFY QTY       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- tbl_barang — harga
-- =============================================================================
ALTER TABLE tbl_barang
    MODIFY HARGA_BELI              decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_TERAKHIR     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_PARTAI_BESAR decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_PARTAI_KECIL decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_UMUM_BESAR   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_UMUM_KECIL   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_UMUM_SEDANG  decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_BELI_UPARTAI_SEDANG decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL_PARTAI_BESAR decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL_PARTAI_KECIL decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL_PARTAI_SEDANG decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL_UMUM_BESAR   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL_UMUM_KECIL   decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_JUAL_UMUM_SEDANG  decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HPP_PARTAI_BESAR        decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HPP_PARTAI_KECIL        decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HPP_PARTAI_SEDANG       decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HPP_UMUM_BESAR          decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HPP_UMUM_KECIL          decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HPP_UMUM_SEDANG         decimal(15,2) NULL DEFAULT 0.00,
    MODIFY KOMISI_SALES_RP         decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- tbl_barang — stok dan counter (qty)
-- =============================================================================
ALTER TABLE tbl_barang
    MODIFY AWAL_TOKO                    decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY AWAL_GUDANG                  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_AWAL_TOKO               decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_AWAL_GUDANG             decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_TOKO                    decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_GUDANG                  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_MIN                     decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY STOK_MAX                     decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY PEMBELIAN_TOKO               decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY PEMBELIAN_GUDANG             decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY PENJUALAN_TOKO               decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY PENJUALAN_GUDANG             decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY RETUR_BELI_TOKO              decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY RETUR_BELI_GUDANG            decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY RETUR_JUAL_TOKO              decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY RETUR_JUAL_GUDANG            decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY OPNAME_TOKO                  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY OPNAME_GUDANG                decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TAMBAH_TOKO                  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TAMBAH_GUDANG                decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY KURANG_TOKO                  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY KURANG_GUDANG                decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_STOK_MASUK_TOKO     decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_STOK_KELUAR_TOKO    decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_STOK_MASUK_GUDANG   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_STOK_KELUAR_GUDANG  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_BARANG_MASUK_TOKO   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_BARANG_KELUAR_TOKO  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_BARANG_MASUK_GUDANG decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_BARANG_KELUAR_GUDANG decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_CABANG_MASUK_TOKO   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_CABANG_KELUAR_TOKO  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_CABANG_MASUK_GUDANG decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TRANSFER_CABANG_KELUAR_GUDANG decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- tbl_gaji
-- =============================================================================
ALTER TABLE tbl_gaji
    MODIFY BONUS_HELPER        decimal(15,2) NULL DEFAULT 0.00,
    MODIFY BONUS_LEMBUR        decimal(15,2) NULL DEFAULT 0.00,
    MODIFY BONUS_MAKAN         decimal(15,2) NULL DEFAULT 0.00,
    MODIFY BONUS_SUPIR         decimal(15,2) NULL DEFAULT 0.00,
    MODIFY BONUS_TRANSPORT     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY POTONGAN_ABSEN      decimal(15,2) NULL DEFAULT 0.00,
    MODIFY POTONGAN_ABSEN_KHUSUS decimal(15,2) NULL DEFAULT 0.00,
    MODIFY POTONGAN_TERLAMBAT  decimal(15,2) NULL DEFAULT 0.00,
    MODIFY PROSENTASE_KOMISI   decimal(10,2) NULL DEFAULT 0.00;

-- =============================================================================
-- tbl_karyawan
-- =============================================================================
ALTER TABLE tbl_karyawan
    MODIFY GAJI decimal(15,0) NULL DEFAULT 0;

-- =============================================================================
-- temp_mutasi_barang
-- =============================================================================
ALTER TABLE temp_mutasi_barang
    MODIFY QTY_KELUAR decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_MASUK  decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY SALDO      decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_barang
-- =============================================================================
ALTER TABLE transfer_barang
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_barang_detail
-- =============================================================================
ALTER TABLE transfer_barang_detail
    MODIFY HARGA     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_cabang
-- =============================================================================
ALTER TABLE transfer_cabang
    MODIFY TOTAL_QTY decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_cabang_detail
-- =============================================================================
ALTER TABLE transfer_cabang_detail
    MODIFY DITERIMA_QTY decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY          decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY TOTAL_QTY    decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_keluar_offline
-- =============================================================================
ALTER TABLE transfer_keluar_offline
    MODIFY qty         decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY qty_satuan  decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_masuk_manual
-- =============================================================================
ALTER TABLE transfer_masuk_manual
    MODIFY qty        decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY qty_satuan decimal(15,4) NULL DEFAULT 0.0000;

-- =============================================================================
-- transfer_stok
-- =============================================================================
ALTER TABLE transfer_stok
    MODIFY HARGA_SAT_K decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGA_SAT_M decimal(15,2) NULL DEFAULT 0.00,
    MODIFY ISI_K       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY ISI_M       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_K       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_M       decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SAT_K   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTY_SAT_M   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY Selisih     decimal(15,2) NULL DEFAULT 0.00;

-- =============================================================================
-- tukarbarang
-- =============================================================================
ALTER TABLE tukarbarang
    MODIFY DISKON      decimal(15,2) NULL DEFAULT 0.00,
    MODIFY HARGASATUAN decimal(15,2) NULL DEFAULT 0.00,
    MODIFY QTY         decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY QTYSATUAN   decimal(15,4) NULL DEFAULT 0.0000,
    MODIFY SELISIH     decimal(15,2) NULL DEFAULT 0.00,
    MODIFY TOTALHARGA  decimal(15,2) NULL DEFAULT 0.00;
