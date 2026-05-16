-- ============================================================
-- 02_satuan_default.sql
-- Data Default Satuan Barang untuk Toko Retail di Indonesia
-- ============================================================
-- FORMAT KODE: max 5 karakter, generate otomatis oleh VB (GenerateSingkatan)
--
-- 1. SATU KATA  : 3 huruf pertama          → PCS, BOT, DUS
-- 2. DUA KATA   : 1 huruf + 2 huruf        → KAR (Karton), PAL (Pallet)
-- 3. TIGA+ KATA : 1 huruf dari 3 kata      → PIU (Pieces / Unit)
-- 4. FALLBACK   : 4 huruf + 1 angka        → BOT1 (jika duplikat)
--
-- Batas: VARCHAR(5) di database, MaxLength=5 di TextBox VB
-- Catatan: KODE satuan TIDAK disimpan di transaksi.
--          Yang disimpan di transaksi adalah NAMA satuan langsung.
-- ============================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

INSERT INTO tbl_satuan (KODE, NAMA, ISI) VALUES

-- ═══════════════════════════════════════════════════════════
-- SATUAN ECERAN
-- ═══════════════════════════════════════════════════════════
('PCS', 'Pcs',           1),
('PAK', 'Pack',          1),
('BOX', 'Box',           1),
('BKS', 'Bungkus',       1),
('BTL', 'Botol',         1),
('KAL', 'Kaleng',        1),
('DUS', 'Dus',           1),
('SCH', 'Sachet',        1),
('STR', 'Strip',         1),
('TAB', 'Tablet',        1),
('KAP', 'Kapsul',        1),
('ROL', 'Roll',          1),
('LBR', 'Lembar',        1),
('BUT', 'Butir',         1),
('POT', 'Potong',        1),
('IKT', 'Ikat',          1),
('KTK', 'Kotak',         1),
('BAG', 'Bag',           1),
('CAN', 'Can',           1),
('CUP', 'Cup',           1),
('POU', 'Pouch',         1),
('TUB', 'Tube',          1),
('JAR', 'Jar',           1),

-- ═══════════════════════════════════════════════════════════
-- SATUAN BERAT & VOLUME
-- ═══════════════════════════════════════════════════════════
('GRM', 'Gram',          1),
('KG',  'Kg',            1),
('ONS', 'Ons',           100),
('LTR', 'Liter',         1),
('ML',  'Ml',            1),
('CC',  'Cc',            1),

-- ═══════════════════════════════════════════════════════════
-- SATUAN PANJANG
-- ═══════════════════════════════════════════════════════════
('MTR', 'Meter',         1),
('CM',  'Cm',            1),

-- ═══════════════════════════════════════════════════════════
-- SATUAN KEMASAN BESAR
-- ═══════════════════════════════════════════════════════════
('LSN', 'Lusin',         12),
('GRS', 'Gross',         144),
('KDI', 'Kodi',          20),
('RIM', 'Rim',           500),
('KRT', 'Karton',        1),
('BAL', 'Bal',           1),
('KRG', 'Karung',        1),
('SAK', 'Sak',           1),
('PLT', 'Pallet',        1),

-- ═══════════════════════════════════════════════════════════
-- SATUAN KHUSUS
-- ═══════════════════════════════════════════════════════════
('SLP', 'Slop',          10),
('GAL', 'Galon',         1),
('EMB', 'Ember',         1),
('DRM', 'Drum',          1),
('TNK', 'Tangki',        1),
('SET', 'Set',           1),
('PSG', 'Pasang',        2),
('UNT', 'Unit',          1)

ON DUPLICATE KEY UPDATE
    NAMA = VALUES(NAMA),
    ISI  = VALUES(ISI);

SET FOREIGN_KEY_CHECKS = 1;

-- Verifikasi
SELECT 'Total Satuan Default' AS keterangan, COUNT(*) AS jumlah
FROM tbl_satuan;
