-- ============================================================
-- Migrasi: Split Bayar Retur Pembelian
-- Tabel: retur_pembelian
-- Alasan:
--   Menambah dukungan split pembayaran (tunai + transfer) untuk retur pembelian.
--   Retur pembelian wajib dibayar penuh (tidak ada piutang supplier).
--   Kolom yang sudah ada (NAMA_REKENING, KODE_REKENING) digunakan untuk tunai.
-- Kolom yang ditambahkan:
--   - NAMA_REKENING_TRANSFER: Nama akun untuk pembayaran transfer
--   - KODE_REKENING_TRANSFER: Kode akun untuk pembayaran transfer
--   - NOMINAL_TUNAI: Nominal pembayaran tunai (default = TOTAL_RUPIAH untuk data lama)
--   - NOMINAL_TRANSFER: Nominal pembayaran transfer
-- Aman: kolom baru dengan default NULL, tidak mempengaruhi data yang sudah ada
-- ============================================================

-- Tambahkan kolom untuk split bayar retur pembelian (hanya transfer)
ALTER TABLE retur_pembelian
    ADD COLUMN NAMA_REKENING_TRANSFER VARCHAR(100) NULL COMMENT 'Nama akun untuk pembayaran transfer' AFTER KODE_REKENING,
    ADD COLUMN KODE_REKENING_TRANSFER VARCHAR(50) NULL COMMENT 'Kode akun untuk pembayaran transfer' AFTER NAMA_REKENING_TRANSFER,
    ADD COLUMN NOMINAL_TUNAI DECIMAL(15,2) NULL DEFAULT 0.00 COMMENT 'Nominal pembayaran tunai' AFTER KODE_REKENING_TRANSFER,
    ADD COLUMN NOMINAL_TRANSFER DECIMAL(15,2) NULL DEFAULT 0.00 COMMENT 'Nominal pembayaran transfer' AFTER NOMINAL_TUNAI;

-- Update data yang sudah ada untuk kompatibilitas backward
-- Untuk data lama, set NOMINAL_TUNAI = TOTAL_RUPIAH (default tunai)
UPDATE retur_pembelian
SET
    NOMINAL_TUNAI = TOTAL_RUPIAH
WHERE NOMINAL_TUNAI IS NULL OR NOMINAL_TUNAI = 0;
