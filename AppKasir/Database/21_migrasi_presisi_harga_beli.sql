-- ============================================================
-- Migrasi: Ubah presisi HARGA_BELI dan kolom HPP terkait
--          dari decimal(15,2) / decimal(15,0) ke decimal(15,4)
-- Alasan:
--   Weighted average HPP menghasilkan nilai pecahan lebih dari 2 desimal.
--   Dengan decimal(15,2), setiap transaksi mengakumulasi selisih rounding
--   antara tbl_barang.HARGA_BELI × STOK vs SALDO_AKHIR akun persediaan.
--   Contoh: 5 faktur senilai 2 miliar → selisih ~200 rupiah.
--   Dengan decimal(15,4), selisih turun ke ~0,02 (tidak signifikan).
-- Tabel terdampak:
--   tbl_barang, pembelian_detail, pembelian_ditahan_detail,
--   penjualan_detail, penjualan_ditahan_detail,
--   retur_pembelian_detail, retur_penjualan_detail,
--   transfer_masuk_manual
-- Aman: data yang sudah ada tidak hilang, hanya presisi bertambah.
-- VB: Math.Round di UpdateHargaAverage dan RecalculateHppSetelahHapus
--     diubah dari 2 ke 4 (lihat FormPembelian.vb dan ModuleHapusTransaksi.vb)
-- ============================================================

-- 1. tbl_barang — sumber kebenaran HPP
ALTER TABLE tbl_barang
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_TERAKHIR decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 2. pembelian_detail — snapshot HPP saat simpan pembelian
ALTER TABLE pembelian_detail
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SATUAN   decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_AVERAGE       decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SEBELUMNYA decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 3. pembelian_ditahan_detail — draft pembelian, struktur sama
ALTER TABLE pembelian_ditahan_detail
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SATUAN   decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_AVERAGE       decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SEBELUMNYA decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 4. penjualan_detail — HPP snapshot saat jual (dipakai hitung laba)
ALTER TABLE penjualan_detail
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SATUAN   decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 5. penjualan_ditahan_detail — draft penjualan
ALTER TABLE penjualan_ditahan_detail
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SATUAN   decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 6. retur_pembelian_detail — HPP saat retur ke supplier
ALTER TABLE retur_pembelian_detail
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SATUAN   decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 7. retur_penjualan_detail — HPP saat retur dari pelanggan
ALTER TABLE retur_penjualan_detail
    MODIFY COLUMN HARGA_BELI          decimal(15,4) NOT NULL DEFAULT 0.0000,
    MODIFY COLUMN HARGA_BELI_SATUAN   decimal(15,4) NOT NULL DEFAULT 0.0000;

-- 8. transfer_masuk_manual — HPP saat transfer masuk manual
ALTER TABLE transfer_masuk_manual
    MODIFY COLUMN harga_beli          decimal(15,4) NOT NULL DEFAULT 0.0000;
