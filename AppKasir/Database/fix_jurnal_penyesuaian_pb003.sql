-- Script Perbaikan Jurnal Penyesuaian PB-2604270003
-- Dibuat: 2026-04-27
-- Tujuan: Memperbaiki jurnal penyesuaian yang salah akibat bug di RecalculateHppSetelahHapus

-- ═══════════════════════════════════════════════════════════════════
-- STEP 1: Hapus jurnal penyesuaian yang salah
-- ═══════════════════════════════════════════════════════════════════
DELETE FROM JurnalUmum 
WHERE NO_TRANSAKSI = 'PB-2604270003' 
AND JENIS_TRANSAKSI = 'Penyesuaian HPP';

-- ═══════════════════════════════════════════════════════════════════
-- STEP 2: Buat jurnal penyesuaian yang benar
-- ═══════════════════════════════════════════════════════════════════
-- Selisih = Nilai persediaan (2.335.000.000) - Saldo jurnal (2.141.350.039)
--         = 193.649.961
-- Persediaan kurang dicatat → D: 01.04.001, K: 06.04.002

INSERT INTO JurnalUmum (
    NO_TRANSAKSI, 
    TGL_TRANSAKSI, 
    NO_NOTA, 
    URAIAN,
    NAMA_AKUN_D, 
    NOMOR_AKUN_D, 
    NAMA_AKUN_K, 
    NOMOR_AKUN_K, 
    NOMINAL,
    JENIS_TRANSAKSI, 
    LOKASI, 
    ID_USER, 
    ID_KOMPUTER
) VALUES (
    'PB-2604270003',
    NOW(),
    'ADJ-PB-2604270003',
    'Penyesuaian HPP moving average — hapus faktur PB-2604270003 (nilai persediaan kurang dicatat Rp 193.649.961)',
    'PERSEDIAAN BARANG',
    '01.04.001',
    'PENYESUAIAN HARGA POKOK',
    '06.04.002',
    193649961,
    'Penyesuaian HPP',
    'TOKO',
    'ADMIN',
    'SYSTEM'
);

-- ═══════════════════════════════════════════════════════════════════
-- STEP 3: Update saldo akun di tbl_datareferensi
-- ═══════════════════════════════════════════════════════════════════
-- Recalculate saldo 01.04.001 (PERSEDIAAN BARANG)
UPDATE tbl_datareferensi 
SET SALDO_AKHIR = (
    SELECT SALDO_AWAL + 
           COALESCE(SUM(CASE WHEN NOMOR_AKUN_D = '01.04.001' THEN NOMINAL ELSE 0 END), 0) -
           COALESCE(SUM(CASE WHEN NOMOR_AKUN_K = '01.04.001' THEN NOMINAL ELSE 0 END), 0)
    FROM JurnalUmum
)
WHERE KODE_AKUN = '01.04.001';

-- Recalculate saldo 06.04.002 (PENYESUAIAN HARGA POKOK)
UPDATE tbl_datareferensi 
SET SALDO_AKHIR = (
    SELECT SALDO_AWAL + 
           COALESCE(SUM(CASE WHEN NOMOR_AKUN_D = '06.04.002' THEN NOMINAL ELSE 0 END), 0) -
           COALESCE(SUM(CASE WHEN NOMOR_AKUN_K = '06.04.002' THEN NOMINAL ELSE 0 END), 0)
    FROM JurnalUmum
)
WHERE KODE_AKUN = '06.04.002';

-- ═══════════════════════════════════════════════════════════════════
-- STEP 4: Perbaiki HARGA_BELI_TERAKHIR
-- ═══════════════════════════════════════════════════════════════════
-- ROK-000001: seharusnya 18.000 dari PB-005 (bukan 15.000 dari PB-002)
UPDATE tbl_barang 
SET HARGA_BELI_TERAKHIR = (
    SELECT pd.HARGA_BELI_SATUAN
    FROM pembelian_detail pd
    INNER JOIN pembelian p ON p.ID_PEMBELIAN = pd.FAKTUR_BELI
    WHERE pd.ID_BARANG = 'ROK-000001'
    AND pd.LOKASI = 'TOKO'
    ORDER BY p.TGL_BELI DESC, pd.NO DESC
    LIMIT 1
)
WHERE ID_BARANG = 'ROK-000001';

-- ROK-000002: seharusnya 28.000 dari PB-005 (bukan 20.000 dari PB-002)
UPDATE tbl_barang 
SET HARGA_BELI_TERAKHIR = (
    SELECT pd.HARGA_BELI_SATUAN
    FROM pembelian_detail pd
    INNER JOIN pembelian p ON p.ID_PEMBELIAN = pd.FAKTUR_BELI
    WHERE pd.ID_BARANG = 'ROK-000002'
    AND pd.LOKASI = 'TOKO'
    ORDER BY p.TGL_BELI DESC, pd.NO DESC
    LIMIT 1
)
WHERE ID_BARANG = 'ROK-000002';

-- ═══════════════════════════════════════════════════════════════════
-- VERIFIKASI
-- ═══════════════════════════════════════════════════════════════════
SELECT 'VERIFIKASI HASIL PERBAIKAN' AS INFO;

SELECT 
    'tbl_barang' AS Tabel,
    ID_BARANG,
    NAMA_BARANG,
    STOK_TOKO,
    HARGA_BELI AS HPP,
    HARGA_BELI_TERAKHIR,
    ROUND(HARGA_BELI * STOK_TOKO, 0) AS NILAI_PERSEDIAAN
FROM tbl_barang 
WHERE ID_BARANG IN ('ROK-000001', 'ROK-000002');

SELECT 
    'tbl_datareferensi' AS Tabel,
    KODE_AKUN,
    NAMA_AKUN,
    SALDO_AKHIR
FROM tbl_datareferensi 
WHERE KODE_AKUN IN ('01.04.001', '06.04.002');

SELECT 
    'Keseimbangan' AS INFO,
    (SELECT SUM(ROUND(HARGA_BELI * STOK_TOKO, 0)) 
     FROM tbl_barang 
     WHERE ID_BARANG IN ('ROK-000001', 'ROK-000002')) AS NILAI_PERSEDIAAN,
    (SELECT SALDO_AKHIR 
     FROM tbl_datareferensi 
     WHERE KODE_AKUN = '01.04.001') AS SALDO_JURNAL,
    (SELECT SUM(ROUND(HARGA_BELI * STOK_TOKO, 0)) 
     FROM tbl_barang 
     WHERE ID_BARANG IN ('ROK-000001', 'ROK-000002')) -
    (SELECT SALDO_AKHIR 
     FROM tbl_datareferensi 
     WHERE KODE_AKUN = '01.04.001') AS SELISIH;
