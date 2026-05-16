-- =============================================================================
-- 09_standarisasi_jenis_transaksi.sql
-- Standarisasi JENIS_TRANSAKSI di jurnalumum ke UPPERCASE
-- =============================================================================
-- Versi  : 1.0.0
-- Tanggal: 2026-04-19
-- Deskripsi:
--   Menstandarisasi kolom JENIS_TRANSAKSI di tabel jurnalumum ke format
--   UPPERCASE, konsisten dengan kolom JENIS di tabel lain:
--     - historybarang.JENIS  : PENJUALAN, PEMBELIAN, OPNAME, dll (UPPERCASE)
--     - stoktambahkurang.JENIS: TAMBAH, KURANG (UPPERCASE)
--     - bon_karyawan.JENIS   : BON, BAYAR (UPPERCASE)
--
-- Sebelum migrasi ini, jurnalumum.JENIS_TRANSAKSI berisi campuran:
--   PascalCase : Penjualan, Pembelian, Retur Penjualan, dll
--   Mixed case : Transfer stok, Bayar hutang, Stok Opnam, dll
--   UPPERCASE  : BIAYA, PEMASUKAN, PENGELUARAN, SETOR KE BOS, PINDAH REKENING
--
-- Setelah migrasi: semua nilai UPPERCASE.
--
-- AMAN DIJALANKAN BERULANG KALI:
--   Semua UPDATE menggunakan WHERE JENIS_TRANSAKSI = 'nilai_lama'
--   sehingga tidak mengubah baris yang sudah benar.
--
-- CATATAN:
--   'Stok Opnam' → 'STOK OPNAME' (sekaligus perbaiki typo: Opnam → OPNAME)
--   Konsisten dengan historybarang.JENIS = 'OPNAME'
-- =============================================================================

-- Transaksi utama
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'PENJUALAN'       WHERE JENIS_TRANSAKSI = 'Penjualan';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'PEMBELIAN'       WHERE JENIS_TRANSAKSI = 'Pembelian';

-- Retur
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'RETUR PENJUALAN' WHERE JENIS_TRANSAKSI = 'Retur Penjualan';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'RETUR PEMBELIAN' WHERE JENIS_TRANSAKSI = 'Retur Pembelian';

-- Transfer
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'TRANSFER STOK'   WHERE JENIS_TRANSAKSI = 'Transfer stok';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'TRANSFER BARANG'  WHERE JENIS_TRANSAKSI = 'Transfer barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'TRANSFER CABANG'  WHERE JENIS_TRANSAKSI = 'Transfer cabang';

-- Opname (sekaligus perbaiki typo Opnam → OPNAME)
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'STOK OPNAME'     WHERE JENIS_TRANSAKSI = 'Stok Opnam';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'STOK OPNAME'     WHERE JENIS_TRANSAKSI = 'Stok Opname';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'STOK OPNAME'     WHERE JENIS_TRANSAKSI = 'stok opname';

-- Bayar
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'BAYAR HUTANG'    WHERE JENIS_TRANSAKSI = 'Bayar hutang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'BAYAR PIUTANG'   WHERE JENIS_TRANSAKSI = 'Bayar piutang';

-- Gaji & Bon
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'GAJI'            WHERE JENIS_TRANSAKSI = 'Gaji';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'BON'             WHERE JENIS_TRANSAKSI = 'Bon';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'BAYAR BON'       WHERE JENIS_TRANSAKSI = 'Bayar bon';

-- Master barang
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'TAMBAH BARANG'   WHERE JENIS_TRANSAKSI = 'Tambah Barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'KURANG BARANG'   WHERE JENIS_TRANSAKSI = 'Kurang Barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'EDIT BARANG'     WHERE JENIS_TRANSAKSI = 'Edit Barang';
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'HAPUS BARANG'    WHERE JENIS_TRANSAKSI = 'Hapus Barang';

-- Saldo awal (dari TambahPelanggan/TambahSupliyer)
UPDATE jurnalumum SET JENIS_TRANSAKSI = 'SALDO AWAL'      WHERE JENIS_TRANSAKSI = 'Saldo Awal';

-- Jurnal manual FormKeuangan sudah UPPERCASE, tidak perlu diubah:
-- BIAYA, PEMASUKAN, PENGELUARAN, SETOR KE BOS, PINDAH REKENING

-- =============================================================================
-- VERIFIKASI — tampilkan semua nilai setelah migrasi
-- =============================================================================
SELECT JENIS_TRANSAKSI, COUNT(*) AS jml
FROM jurnalumum
GROUP BY JENIS_TRANSAKSI
ORDER BY jml DESC;

SELECT CONCAT(
    'Total baris jurnalumum: ',
    COUNT(*),
    ' | Nilai tidak UPPERCASE: ',
    SUM(CASE WHEN JENIS_TRANSAKSI <> UPPER(JENIS_TRANSAKSI) THEN 1 ELSE 0 END)
) AS verifikasi
FROM jurnalumum;
