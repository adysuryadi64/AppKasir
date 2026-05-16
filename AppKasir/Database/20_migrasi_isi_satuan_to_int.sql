-- ============================================================
-- Migrasi: Ubah tipe ISI_SATUAN dari decimal(10,2) ke int(11)
-- Tabel terdampak: pembelian_detail, pembelian_ditahan_detail,
--                  retur_penjualan_detail, transfer_barang_detail,
--                  transfer_cabang_detail
-- Alasan: ISI_SATUAN selalu bilangan bulat (tidak ada pecahan).
--         Tipe decimal(10,2) tidak konsisten dengan tabel lain
--         (historybarang, penjualan_detail, dll sudah int).
-- Aman: ROUND() memastikan tidak ada data yang hilang.
-- ============================================================

-- 1. pembelian_detail
ALTER TABLE pembelian_detail
    MODIFY COLUMN ISI_SATUAN int(11) NOT NULL DEFAULT 1;

-- 2. pembelian_ditahan_detail
ALTER TABLE pembelian_ditahan_detail
    MODIFY COLUMN ISI_SATUAN int(11) NOT NULL DEFAULT 1;

-- 3. retur_penjualan_detail
ALTER TABLE retur_penjualan_detail
    MODIFY COLUMN ISI_SATUAN int(11) NOT NULL DEFAULT 1;

-- 4. transfer_barang_detail
ALTER TABLE transfer_barang_detail
    MODIFY COLUMN ISI_SATUAN int(11) NOT NULL DEFAULT 1;

-- 5. transfer_cabang_detail
ALTER TABLE transfer_cabang_detail
    MODIFY COLUMN ISI_SATUAN int(11) NOT NULL DEFAULT 1;
