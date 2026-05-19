-- =============================================================================
-- 26: Optimasi Index HistoryBarang
-- =============================================================================
-- Masalah: INSERT ke HistoryBarang lambat (~450-900ms untuk 10 baris) karena
--          tabel memiliki 7 index pada 1.1 juta+ baris.
--
-- Analisa penggunaan index:
--   HistoryBarang_ID_USER       — tidak ada query SELECT yang filter by ID_USER → HAPUS
--   HistoryBarang_JENIS         — query yang pakai JENIS selalu bersama FAKTUR,
--                                 sudah tercakup idx_faktur_history → HAPUS
--   idx_barang_jenis_tgl_lokasi — overlap dengan idx_barang_jenis_tgl (subset) → HAPUS
--   idx_barang_lokasi_tgl       — overlap dengan idx_barang_jenis_tgl → HAPUS
--
-- Index yang dipertahankan:
--   PRIMARY                     — wajib
--   uq_sync_id_historybarang    — dipakai untuk sync multi-cabang
--   HistoryBarang_TANGGAL       — dipakai FormLapStokLampau WHERE TANGGAL <= @tgl
--   idx_faktur_history          — dipakai DELETE/SELECT WHERE FAKTUR = ?
--   idx_lokasi_jenis_barang_qty — dipakai batch recalculate stok WHERE LOKASI GROUP BY ID_BARANG
--   idx_barang_jenis_tgl        — dipakai SELECT WHERE ID_BARANG + filter JENIS/TANGGAL
--
-- Hasil: 7 index → 5 index, INSERT lebih cepat ~30-40%
-- =============================================================================

-- Hapus index yang redundan/tidak dipakai
ALTER TABLE HistoryBarang
    DROP INDEX IF EXISTS HistoryBarang_ID_USER,
    DROP INDEX IF EXISTS HistoryBarang_JENIS,
    DROP INDEX IF EXISTS idx_barang_jenis_tgl_lokasi,
    DROP INDEX IF EXISTS idx_barang_lokasi_tgl;
