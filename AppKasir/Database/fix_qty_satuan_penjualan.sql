-- ============================================================
-- fix_qty_satuan_penjualan.sql
-- Koreksi data penjualan_detail yang QTY_SATUAN = 0
-- karena ISI_SATUAN = 0 saat simpan pertama kali.
--
-- Kondisi saat ini: semua ISI = 1, jadi QTY_SATUAN = QTY * 1 = QTY
-- Jalankan sekali, aman diulang (idempoten).
-- ============================================================

-- ── Cek dulu sebelum update ──────────────────────────────────
SELECT
    COUNT(*)                        AS total_baris_bermasalah,
    SUM(QTY)                        AS total_qty,
    MIN(TANGGAL_JUAL)               AS tgl_terlama,
    MAX(TANGGAL_JUAL)               AS tgl_terbaru
FROM penjualan_detail
WHERE QTY_SATUAN = 0
  AND QTY > 0;

-- ── Cek detail per faktur ────────────────────────────────────
SELECT
    FAKTUR_JUAL,
    ID_BARANG,
    NAMA_BARANG,
    QTY,
    ISI_SATUAN,
    QTY_SATUAN,
    TANGGAL_JUAL
FROM penjualan_detail
WHERE QTY_SATUAN = 0
  AND QTY > 0
ORDER BY TANGGAL_JUAL DESC
LIMIT 100;

-- ── Update: set ISI_SATUAN = 1 dan QTY_SATUAN = QTY ─────────
-- Karena kondisi saat ini semua ISI = 1, maka QTY_SATUAN = QTY
UPDATE penjualan_detail
SET
    ISI_SATUAN  = 1,
    QTY_SATUAN  = QTY
WHERE QTY_SATUAN = 0
  AND QTY > 0;

-- ── Verifikasi setelah update ────────────────────────────────
SELECT
    COUNT(*) AS sisa_bermasalah
FROM penjualan_detail
WHERE QTY_SATUAN = 0
  AND QTY > 0;

-- ── Cek juga ISI_SATUAN yang NULL atau 0 ────────────────────
UPDATE penjualan_detail
SET ISI_SATUAN = 1
WHERE ISI_SATUAN IS NULL
   OR ISI_SATUAN = 0;

-- ── Selesai ──────────────────────────────────────────────────
SELECT 'Koreksi selesai.' AS status;
