-- ============================================================
-- seed_data_lengkap.sql
-- Data awal bersih untuk testing db_kasirlancar
--
-- Idempotent: hapus dulu → insert ulang → hasil selalu sama
-- Jalankan:
--   Get-Content Database/seed_data_lengkap.sql | .\MySQL\mysql.exe -u root -p12345678 db_kasirlancar
--
-- CATATAN:
--   tbl_satuan, tbl_kategori, tbl_merk TIDAK diisi di sini.
--   Gunakan file terpisah di database_Default_Master/:
--     01_kategori_default.sql
--     02_satuan_default.sql
--     03_merk_default.sql
-- ============================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ── Bersihkan tabel transaksi & log ──────────────────────────
TRUNCATE TABLE historybarang;
TRUNCATE TABLE tbl_audit_trail;
TRUNCATE TABLE tbl_audit_trail_arsip;
TRUNCATE TABLE transfer_cabang_detail;
TRUNCATE TABLE sync_queue;
TRUNCATE TABLE sync_log;

-- ── Bersihkan master transaksi ───────────────────────────────
DELETE FROM tbl_barang;
DELETE FROM tbl_supliyer  WHERE KODE LIKE 'SPL-%';
DELETE FROM tbl_pelanggan WHERE KODE LIKE 'PEL-%';
DELETE FROM tbl_karyawan  WHERE KODE LIKE 'KRY-%';
DELETE FROM tbl_Armada    WHERE KODE LIKE 'ARM-%';
DELETE FROM tbl_cabang    WHERE kode_cabang LIKE 'CBG-%';

-- ── 1. tbl_supliyer ──────────────────────────────────────────
-- Format: SPL-0001 (prefix SPL-, 4 digit) sesuai TambahSupliyer.vb
INSERT INTO tbl_supliyer (KODE, NAMA, ALAMAT, HP, JANGKAHUTANG, HUTANGAWAL, TOTALHUTANG, TOTALBAYAR, HUTANGAKHIR, Status) VALUES
('SPL-0001', 'PT Tirta Investama',    'Jl. Raya Bogor No.1',    '021-1234567', 30, 0, 0, 0, 0, 'Aktif'),
('SPL-0002', 'PT Indofood Sukses',    'Jl. Sudirman No.5',      '021-2345678', 30, 0, 0, 0, 0, 'Aktif'),
('SPL-0003', 'PT Salim Ivomas',       'Jl. Gatot Subroto No.3', '021-3456789', 30, 0, 0, 0, 0, 'Aktif'),
('SPL-0004', 'PT Unilever Indonesia', 'Jl. Jend. Gatot No.7',   '021-4567890', 30, 0, 0, 0, 0, 'Aktif'),
('SPL-0005', 'PT Gudang Garam Tbk',   'Jl. Semampir No.1',      '0354-123456', 30, 0, 0, 0, 0, 'Aktif'),
('SPL-0006', 'PT Heinz ABC',          'Jl. Industri No.3',      '021-5678901', 30, 0, 0, 0, 0, 'Aktif');

-- ── 2. tbl_pelanggan ─────────────────────────────────────────
-- Format: PEL-0001 (prefix PEL-, 4 digit) sesuai TambahPelanggan.vb
INSERT INTO tbl_pelanggan (KODE, NAMA, ALAMAT, NO_TELP, JENIS, JANGKAPIUTANG, HUTANGAWAL, TOTALHUTANG, TOTALBAYAR, HUTANGAKHIR, Status) VALUES
('PEL-0001', 'Umum',            '-',                  '-',            'Umum',   0,  0, 0, 0, 0, 'Aktif'),
('PEL-0002', 'Toko Makmur',     'Jl. Pasar No.10',    '081234567890', 'Partai', 30, 0, 0, 0, 0, 'Aktif'),
('PEL-0003', 'Warung Berkah',   'Jl. Melati No.5',    '082345678901', 'Partai', 30, 0, 0, 0, 0, 'Aktif'),
('PEL-0004', 'Minimarket Jaya', 'Jl. Merdeka No.20',  '083456789012', 'Partai', 30, 0, 0, 0, 0, 'Aktif'),
('PEL-0005', 'Budi Santoso',    'Jl. Kenanga No.3',   '084567890123', 'Umum',   0,  0, 0, 0, 0, 'Aktif');

-- ── 3. tbl_karyawan ──────────────────────────────────────────
-- Format: KRY-0001 (prefix KRY-, 4 digit) sesuai FormKaryawan.vb
INSERT INTO tbl_karyawan (KODE, NAMA, JABATAN, TGLMASUK, GAJI, SALDOAWAL, TOTALBON, TOTALBAYAR, SALDOAKHIR, Status) VALUES
('KRY-0001', 'Siti Rahayu',  'Kasir',   '2023-01-01', 2500000, 0, 0, 0, 0, 'Aktif'),
('KRY-0002', 'Budi Hartono', 'Gudang',  '2023-01-01', 2200000, 0, 0, 0, 0, 'Aktif'),
('KRY-0003', 'Dewi Lestari', 'Sales',   '2023-03-01', 2300000, 0, 0, 0, 0, 'Aktif'),
('KRY-0004', 'Ahmad Fauzi',  'Manajer', '2022-06-01', 4000000, 0, 0, 0, 0, 'Aktif'),
('KRY-0005', 'Rina Wati',    'Kasir',   '2024-01-01', 2500000, 0, 0, 0, 0, 'Aktif');

-- ── 4. tbl_Armada ────────────────────────────────────────────
-- Format: ARM-0001 (prefix ARM-, 4 digit) sesuai FormArmada.vb
INSERT INTO tbl_Armada (KODE, NOPOL, JENIS) VALUES
('ARM-0001', 'B 1234 ABC', 'Motor'),
('ARM-0002', 'B 5678 DEF', 'Mobil Box'),
('ARM-0003', 'B 9012 GHI', 'Motor');

-- ── 5. tbl_cabang ────────────────────────────────────────────
INSERT INTO tbl_cabang (kode_cabang, nama_cabang, alamat, kota, hp, pemilik, sumber) VALUES
('CBG-PUSAT', 'Toko Pusat',     'Jl. Utama No.1',   'Jakarta', '021-9999999', 'Pemilik', 'manual'),
('CBG-001',   'Cabang Selatan', 'Jl. Selatan No.5', 'Jakarta', '021-8888888', 'Pemilik', 'manual');

-- ── 6. tbl_barang ────────────────────────────────────────────
-- ID_BARANG  : format {KODE_KATEGORI}-{6 digit urut} sesuai TambahBarang.vb
-- SATUAN_*   : NAMA satuan (bukan kode) — sesuai temuan: transaksi simpan NAMA
-- KODE_MERK  : VARCHAR(4), KODE_KATEGORI: VARCHAR(4)
--
-- Kolom yang diisi 0 (history stok): semua TAMBAH/KURANG/PEMBELIAN/PENJUALAN/
-- RETUR/OPNAME/TRANSFER — dihitung ulang oleh sp_hlp_stok_hitung saat dibutuhkan

INSERT INTO tbl_barang (
    ID_BARANG, NAMA_BARANG, JENIS,
    KODE_KATEGORI, NAMA_KATEGORI,
    KODE_SUPLIYER, NAMA_SUPLIYER,
    KODE_MERK, NAMA_MERK,
    HARGA_BELI, HARGA_BELI_TERAKHIR,
    BARCODE_KECIL,
    SATUAN_UMUM_KECIL,    ISI_UMUM_KECIL,    HARGA_JUAL_UMUM_KECIL,
    SATUAN_UMUM_SEDANG,   ISI_UMUM_SEDANG,   HARGA_JUAL_UMUM_SEDANG,
    SATUAN_UMUM_BESAR,    ISI_UMUM_BESAR,    HARGA_JUAL_UMUM_BESAR,
    SATUAN_PARTAI_KECIL,  ISI_PARTAI_KECIL,  HARGA_JUAL_PARTAI_KECIL,
    SATUAN_PARTAI_SEDANG, ISI_PARTAI_SEDANG, HARGA_JUAL_PARTAI_SEDANG,
    SATUAN_PARTAI_BESAR,  ISI_PARTAI_BESAR,  HARGA_JUAL_PARTAI_BESAR,
    AWAL_TOKO, STOK_TOKO, AWAL_GUDANG, STOK_GUDANG,
    SATUAN_STOK, SATUAN_ISI_STOK, STOK_MIN, STOK_MAX, STATUS
) VALUES

-- ── Minuman ──────────────────────────────────────────────────
('MIN-000001', 'Aqua 600ml', 'Barang Dagangan',
 'MIN', 'Minuman', 'SPL-0001', 'PT Tirta Investama', 'AQU', 'Aqua',
 2000, 2000, '8999999001001',
 'Botol',  1, 3000,  'Dus',    24, 68000,  'Karton', 48, 132000,
 'Botol',  1, 2800,  'Dus',    24, 63000,  'Karton', 48, 122000,
 0, 0, 0, 0, 'Botol', 1, 10, 500, 'Aktif'),

('MIN-000002', 'Aqua 1500ml', 'Barang Dagangan',
 'MIN', 'Minuman', 'SPL-0001', 'PT Tirta Investama', 'AQU', 'Aqua',
 3500, 3500, '8999999001002',
 'Botol',  1, 5000,  'Dus',    12, 57000,  'Karton', 24, 110000,
 'Botol',  1, 4500,  'Dus',    12, 51000,  'Karton', 24, 99000,
 0, 0, 0, 0, 'Botol', 1, 5, 200, 'Aktif'),

('MIN-000003', 'Teh Kotak 300ml', 'Barang Dagangan',
 'MIN', 'Minuman', 'SPL-0002', 'PT Indofood Sukses', 'SOS', 'Sosro',
 3000, 3000, '8999999011001',
 'Pcs',    1, 4000,  'Dus',    24, 90000,  NULL, 0, 0,
 'Pcs',    1, 3800,  'Dus',    24, 86000,  NULL, 0, 0,
 0, 0, 0, 0, 'Pcs', 1, 12, 500, 'Aktif'),

-- ── Mie Instan ───────────────────────────────────────────────
('MIE-000001', 'Indomie Goreng', 'Barang Dagangan',
 'MIE', 'Mie Instan', 'SPL-0002', 'PT Indofood Sukses', 'IND', 'Indomie',
 2800, 2800, '8999999002001',
 'Bungkus', 1, 3500, 'Dus',    40, 132000, 'Karton', 80, 260000,
 'Bungkus', 1, 3200, 'Dus',    40, 120000, 'Karton', 80, 236000,
 0, 0, 0, 0, 'Bungkus', 1, 20, 1000, 'Aktif'),

('MIE-000002', 'Indomie Kuah Ayam', 'Barang Dagangan',
 'MIE', 'Mie Instan', 'SPL-0002', 'PT Indofood Sukses', 'IND', 'Indomie',
 2700, 2700, '8999999002002',
 'Bungkus', 1, 3500, 'Dus',    40, 132000, 'Karton', 80, 260000,
 'Bungkus', 1, 3200, 'Dus',    40, 120000, 'Karton', 80, 236000,
 0, 0, 0, 0, 'Bungkus', 1, 20, 800, 'Aktif'),

-- ── Minyak Goreng ────────────────────────────────────────────
('MGO-000001', 'Bimoli 1 Liter', 'Barang Dagangan',
 'MGO', 'Minyak Goreng', 'SPL-0003', 'PT Salim Ivomas', 'BIM', 'Bimoli',
 14000, 14000, '8999999003001',
 'Botol',  1, 17000, 'Dus',    12, 196000, 'Karton', 24, 388000,
 'Botol',  1, 16000, 'Dus',    12, 184000, 'Karton', 24, 364000,
 0, 0, 0, 0, 'Botol', 1, 5, 100, 'Aktif'),

('MGO-000002', 'Bimoli 2 Liter', 'Barang Dagangan',
 'MGO', 'Minyak Goreng', 'SPL-0003', 'PT Salim Ivomas', 'BIM', 'Bimoli',
 26000, 26000, '8999999003002',
 'Botol',  1, 32000, 'Dus',    6,  186000, 'Karton', 12, 368000,
 'Botol',  1, 30000, 'Dus',    6,  174000, 'Karton', 12, 344000,
 0, 0, 0, 0, 'Botol', 1, 3, 50, 'Aktif'),

-- ── Deterjen ─────────────────────────────────────────────────
('DET-000001', 'Rinso 800gr', 'Barang Dagangan',
 'DET', 'Deterjen', 'SPL-0004', 'PT Unilever Indonesia', 'RIN', 'Rinso',
 18000, 18000, '8999999004001',
 'Bungkus', 1, 22000, 'Dus',   12, 256000, 'Karton', 24, 508000,
 'Bungkus', 1, 20000, 'Dus',   12, 232000, 'Karton', 24, 460000,
 0, 0, 0, 0, 'Bungkus', 1, 5, 100, 'Aktif'),

('DET-000002', 'Rinso 1.8Kg', 'Barang Dagangan',
 'DET', 'Deterjen', 'SPL-0004', 'PT Unilever Indonesia', 'RIN', 'Rinso',
 38000, 38000, '8999999004002',
 'Bungkus', 1, 45000, 'Dus',   6,  264000, 'Karton', 12, 524000,
 'Bungkus', 1, 42000, 'Dus',   6,  246000, 'Karton', 12, 488000,
 0, 0, 0, 0, 'Bungkus', 1, 3, 50, 'Aktif'),

-- ── Rokok ────────────────────────────────────────────────────
('ROK-000001', 'Gudang Garam Merah 12', 'Barang Dagangan',
 'ROK', 'Rokok', 'SPL-0005', 'PT Gudang Garam Tbk', 'GGA', 'Gudang Garam',
 16500, 16500, '8999999005001',
 'Bungkus', 1, 20000, 'Slop',  10, 195000, 'Karton', 200, 3800000,
 'Bungkus', 1, 19000, 'Slop',  10, 185000, 'Karton', 200, 3600000,
 0, 0, 0, 0, 'Bungkus', 1, 10, 500, 'Aktif'),

('ROK-000002', 'Gudang Garam Surya 16', 'Barang Dagangan',
 'ROK', 'Rokok', 'SPL-0005', 'PT Gudang Garam Tbk', 'GGA', 'Gudang Garam',
 20000, 20000, '8999999005002',
 'Bungkus', 1, 24000, 'Slop',  10, 235000, 'Karton', 200, 4600000,
 'Bungkus', 1, 22500, 'Slop',  10, 220000, 'Karton', 200, 4300000,
 0, 0, 0, 0, 'Bungkus', 1, 10, 400, 'Aktif'),

-- ── Gula ─────────────────────────────────────────────────────
('GUL-000001', 'Gula Pasir 1Kg', 'Barang Dagangan',
 'GUL', 'Gula', 'SPL-0003', 'PT Salim Ivomas', 'GUL', 'Gulaku',
 13000, 13000, '8999999006001',
 'Kg',     1, 15000, 'Sak',   50, 720000, NULL, 0, 0,
 'Kg',     1, 14000, 'Sak',   50, 670000, NULL, 0, 0,
 0, 0, 0, 0, 'Kg', 1, 10, 300, 'Aktif'),

-- ── Tepung ───────────────────────────────────────────────────
('TEP-000001', 'Tepung Segitiga 1Kg', 'Barang Dagangan',
 'TEP', 'Tepung', 'SPL-0002', 'PT Indofood Sukses', 'SBI', 'Segitiga Biru',
 9500, 9500, '8999999006002',
 'Kg',     1, 11000, 'Sak',   25, 265000, NULL, 0, 0,
 'Kg',     1, 10000, 'Sak',   25, 245000, NULL, 0, 0,
 0, 0, 0, 0, 'Kg', 1, 5, 200, 'Aktif'),

-- ── Kecap ────────────────────────────────────────────────────
('KEC-000001', 'Kecap Bango 135ml', 'Barang Dagangan',
 'KEC', 'Kecap', 'SPL-0006', 'PT Heinz ABC', 'BAN', 'Bango',
 7500, 7500, '8999999007001',
 'Botol',  1, 9500,  'Dus',   24, 216000, NULL, 0, 0,
 'Botol',  1, 8500,  'Dus',   24, 198000, NULL, 0, 0,
 0, 0, 0, 0, 'Botol', 1, 5, 100, 'Aktif'),

-- ── Sabun ────────────────────────────────────────────────────
('SAB-000001', 'Sabun Lifebuoy 110gr', 'Barang Dagangan',
 'SAB', 'Sabun', 'SPL-0004', 'PT Unilever Indonesia', 'LIF', 'Lifebuoy',
 4500, 4500, '8999999007002',
 'Pcs',    1, 6000,  'Dus',   48, 276000, NULL, 0, 0,
 'Pcs',    1, 5500,  'Dus',   48, 252000, NULL, 0, 0,
 0, 0, 0, 0, 'Pcs', 1, 10, 200, 'Aktif'),

('SAB-000002', 'Pepsodent 190gr', 'Barang Dagangan',
 'SAB', 'Sabun', 'SPL-0004', 'PT Unilever Indonesia', 'PEP', 'Pepsodent',
 11000, 11000, '8999999012001',
 'Pcs',    1, 14000, 'Dus',   36, 486000, NULL, 0, 0,
 'Pcs',    1, 13000, 'Dus',   36, 450000, NULL, 0, 0,
 0, 0, 0, 0, 'Pcs', 1, 10, 300, 'Aktif'),

-- ── Susu ─────────────────────────────────────────────────────
('SUS-000001', 'Susu Indomilk 385ml', 'Barang Dagangan',
 'SUS', 'Susu', 'SPL-0002', 'PT Indofood Sukses', 'INM', 'Indomilk',
 8000, 8000, '8999999008001',
 'Kaleng', 1, 10000, 'Dus',   24, 228000, NULL, 0, 0,
 'Kaleng', 1, 9000,  'Dus',   24, 210000, NULL, 0, 0,
 0, 0, 0, 0, 'Kaleng', 1, 5, 100, 'Aktif'),

-- ── Makanan Ringan ───────────────────────────────────────────
('MRI-000001', 'Chitato 68gr', 'Barang Dagangan',
 'MRI', 'Makanan Ringan', 'SPL-0002', 'PT Indofood Sukses', 'CHI', 'Chitato',
 8500, 8500, '8999999009001',
 'Bungkus', 1, 10500, 'Dus',  30, 300000, NULL, 0, 0,
 'Bungkus', 1, 10000, 'Dus',  30, 285000, NULL, 0, 0,
 0, 0, 0, 0, 'Bungkus', 1, 10, 300, 'Aktif'),

-- ── Kebersihan ───────────────────────────────────────────────
('KEB-000001', 'Sunlight 400ml', 'Barang Dagangan',
 'KEB', 'Kebersihan', 'SPL-0004', 'PT Unilever Indonesia', 'WIP', 'Wipol',
 7000, 7000, '8999999010001',
 'Botol',  1, 9000,  'Dus',   24, 204000, NULL, 0, 0,
 'Botol',  1, 8500,  'Dus',   24, 192000, NULL, 0, 0,
 0, 0, 0, 0, 'Botol', 1, 5, 200, 'Aktif'),

-- ── Sembako ──────────────────────────────────────────────────
('SMB-000001', 'Beras Setra Ramos 5kg', 'Barang Dagangan',
 'SMB', 'Sembako', 'SPL-0003', 'PT Salim Ivomas', 'BIM', 'Bimoli',
 65000, 65000, '8999999013001',
 'Sak',    1, 75000, 'Karung', 10, 730000, NULL, 0, 0,
 'Sak',    1, 72000, 'Karung', 10, 700000, NULL, 0, 0,
 0, 0, 0, 0, 'Sak', 1, 2, 50, 'Aktif');

SET FOREIGN_KEY_CHECKS = 1;

-- ── Verifikasi ───────────────────────────────────────────────
SELECT 'tbl_supliyer'  AS tabel, COUNT(*) AS jumlah FROM tbl_supliyer  WHERE KODE LIKE 'SPL-%'
UNION ALL SELECT 'tbl_pelanggan', COUNT(*) FROM tbl_pelanggan WHERE KODE LIKE 'PEL-%'
UNION ALL SELECT 'tbl_karyawan',  COUNT(*) FROM tbl_karyawan  WHERE KODE LIKE 'KRY-%'
UNION ALL SELECT 'tbl_Armada',    COUNT(*) FROM tbl_Armada    WHERE KODE LIKE 'ARM-%'
UNION ALL SELECT 'tbl_cabang',    COUNT(*) FROM tbl_cabang    WHERE kode_cabang LIKE 'CBG-%'
UNION ALL SELECT 'tbl_barang',    COUNT(*) FROM tbl_barang    WHERE ID_BARANG LIKE '%-000%';
