-- ================================================================
-- 33_loyalty_point_verify.sql
-- Script verifikasi end-to-end Sistem Poin Loyalitas
-- Task 9 — Testing dan Verifikasi
-- ================================================================

-- ── 1. Cek struktur tabel ────────────────────────────────────────
SHOW TABLES LIKE 'poin_%';
SHOW COLUMNS FROM poin_config;
SHOW COLUMNS FROM poin_ledger;
SHOW COLUMNS FROM poin_barang;
SHOW COLUMNS FROM tbl_pelanggan LIKE 'SALDO_POIN';

-- ── 2. Cek konfigurasi default ───────────────────────────────────
SELECT * FROM poin_config;

-- ── 3. Rekonstruksi saldo dari ledger (harus = SALDO_POIN di tbl_pelanggan) ──
-- Req 9.11: SUM(JUMLAH_POIN) per pelanggan harus sama dengan SALDO_POIN
SELECT
    p.KODE,
    p.NAMA,
    p.SALDO_POIN                          AS saldo_di_tabel,
    COALESCE(SUM(pl.JUMLAH_POIN), 0)      AS saldo_dari_ledger,
    p.SALDO_POIN - COALESCE(SUM(pl.JUMLAH_POIN), 0) AS selisih
FROM tbl_pelanggan p
LEFT JOIN poin_ledger pl ON pl.KODE_PELANGGAN = p.KODE
GROUP BY p.KODE, p.NAMA, p.SALDO_POIN
HAVING selisih <> 0
ORDER BY p.NAMA;
-- Hasil harus KOSONG (tidak ada selisih)

-- ── 4. Cek tidak ada SALDO_POIN negatif ─────────────────────────
SELECT KODE, NAMA, SALDO_POIN
FROM tbl_pelanggan
WHERE SALDO_POIN < 0;
-- Hasil harus KOSONG

-- ── 5. Riwayat poin per pelanggan (ganti @kode sesuai kebutuhan) ─
-- COLLATE utf8mb4_unicode_ci wajib agar collation variable cocok dengan kolom
SET @kode = CONVERT('P001' USING utf8mb4) COLLATE utf8mb4_unicode_ci;  -- ganti dengan kode pelanggan yang ditest

SELECT
    CREATED_AT,
    TIPE,
    JUMLAH_POIN,
    NO_REFERENSI,
    KETERANGAN,
    @running := @running + JUMLAH_POIN AS saldo_berjalan
FROM poin_ledger,
     (SELECT @running := 0) AS init
WHERE KODE_PELANGGAN = @kode
ORDER BY CREATED_AT ASC;

-- ── 6. Cek atomisitas — tidak ada EARN tanpa faktur penjualan ────
SELECT pl.*
FROM poin_ledger pl
LEFT JOIN penjualan pj ON pj.ID_PENJUALAN = pl.NO_REFERENSI
WHERE pl.TIPE = 'EARN'
  AND pj.ID_PENJUALAN IS NULL;
-- Hasil harus KOSONG (setiap EARN punya faktur penjualan)

-- ── 7. Cek VOID_EARN tidak melebihi EARN asal ───────────────────
SELECT
    earn.NO_REFERENSI AS faktur,
    earn.JUMLAH_POIN  AS poin_earn,
    COALESCE(ABS(SUM(void.JUMLAH_POIN)), 0) AS total_void,
    earn.JUMLAH_POIN + COALESCE(SUM(void.JUMLAH_POIN), 0) AS sisa_earn
FROM poin_ledger earn
LEFT JOIN poin_ledger void
    ON void.NO_REFERENSI = earn.NO_REFERENSI
    AND void.TIPE = 'VOID_EARN'
    AND void.KODE_PELANGGAN = earn.KODE_PELANGGAN
WHERE earn.TIPE = 'EARN'
GROUP BY earn.NO_REFERENSI, earn.JUMLAH_POIN
HAVING sisa_earn < 0;
-- Hasil harus KOSONG (void tidak boleh melebihi earn)

-- ── 8. Statistik ringkas ─────────────────────────────────────────
SELECT
    TIPE,
    COUNT(*)          AS jumlah_transaksi,
    SUM(JUMLAH_POIN)  AS total_poin,
    MIN(CREATED_AT)   AS pertama,
    MAX(CREATED_AT)   AS terakhir
FROM poin_ledger
GROUP BY TIPE
ORDER BY TIPE;

-- ── 9. Barang yang bisa ditukar ──────────────────────────────────
-- tbl_barang tidak punya kolom STOK — gunakan STOK_TOKO dan STOK_GUDANG
SELECT
    b.ID_BARANG,
    b.NAMA_BARANG,
    b.STOK_TOKO,
    b.STOK_GUDANG,
    (b.STOK_TOKO + b.STOK_GUDANG) AS STOK_TOTAL,
    pb.HARGA_POIN,
    pb.AKTIF
FROM poin_barang pb
INNER JOIN tbl_barang b ON b.ID_BARANG = pb.ID_BARANG
WHERE pb.AKTIF = 1 AND pb.HARGA_POIN > 0
ORDER BY b.NAMA_BARANG;
