-- ============================================================
-- 01_kategori_default.sql
-- Data Default Kategori Barang untuk Toko Retail di Indonesia
-- ============================================================
-- FORMAT KODE: max 4 karakter, generate otomatis oleh VB (GenerateSingkatan)
--
-- 1. SATU KATA  : 3 huruf pertama          → MIN, GUL, KOP
-- 2. DUA KATA   : 1 huruf + 2 huruf        → AMN (Air Minum), MGO (Minyak Goreng)
-- 3. TIGA+ KATA : 1 huruf dari 3 kata      → MRI (Makanan Ringan)
-- 4. FALLBACK   : 3 huruf + 1 angka        → AM1, AM2 (jika duplikat)
--
-- Batas: VARCHAR(4) di database, MaxLength=4 di TextBox VB
-- ============================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

INSERT INTO tbl_kategori (KODE, NAMA, JENIS) VALUES

-- ═══════════════════════════════════════════════════════════
-- MINUMAN & BAHAN POKOK
-- ═══════════════════════════════════════════════════════════
('MIN','Minuman',              'Barang Dagangan'),
('AMN','Air Minum',            'Barang Dagangan'),
('MGO','Minyak Goreng',        'Barang Dagangan'),
('GUL','Gula',                 'Barang Dagangan'),
('BER','Beras',                'Barang Dagangan'),
('TEP','Tepung',               'Barang Dagangan'),
('SUS','Susu',                 'Barang Dagangan'),
('KOP','Kopi',                 'Barang Dagangan'),
('TEH','Teh',                  'Barang Dagangan'),
('SAU','Saus',                 'Barang Dagangan'),
('KEC','Kecap',                'Barang Dagangan'),
('SAO','Saos Tomat',           'Barang Dagangan'),
('KRI','Krim Kental',          'Barang Dagangan'),
('SIR','Sirup',                'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- MAKANAN
-- ═══════════════════════════════════════════════════════════
('MRI','Makanan Ringan',       'Barang Dagangan'),
('MIE','Mie Instan',           'Barang Dagangan'),
('BIS','Biskuit',              'Barang Dagangan'),
('KUE','Kue',                  'Barang Dagangan'),
('ROT','Roti',                 'Barang Dagangan'),
('SNK','Snack',                'Barang Dagangan'),
('KAC','Kacang',               'Barang Dagangan'),
('PER','Permen',               'Barang Dagangan'),
('COK','Coklat',               'Barang Dagangan'),
('ESK','Es Krim',              'Barang Dagangan'),
('DON','Donat',                'Barang Dagangan'),
('SOS','Sosis',                'Barang Dagangan'),
('NGG','Nugget',               'Barang Dagangan'),
('KOR','Kornet',               'Barang Dagangan'),
('ABA','Abon Ayam',            'Barang Dagangan'),
('SKL','Sarden Kaleng',        'Barang Dagangan'),
('MAK','Makaroni',             'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- SEMBAKO & KEBUTUHAN DAPUR
-- ═══════════════════════════════════════════════════════════
('SMB','Sembako',              'Barang Dagangan'),
('MSG','MSG/Vetsin',           'Barang Dagangan'),
('MRC','Merica',               'Barang Dagangan'),
('KUN','Kunyit',               'Barang Dagangan'),
('KET','Ketumbar',             'Barang Dagangan'),
('LEN','Lengkuas',             'Barang Dagangan'),
('JAE','Jahe',                 'Barang Dagangan'),
('SAL','Salam',                'Barang Dagangan'),
('SER','Sereh',                'Barang Dagangan'),
('MIK','Mie Kering',           'Barang Dagangan'),
('BUM','Bumbu Instan',         'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- KEBERSIHAN
-- ═══════════════════════════════════════════════════════════
('KEB','Kebersihan',           'Barang Dagangan'),
('DET','Deterjen',             'Barang Dagangan'),
('SAB','Sabun',                'Barang Dagangan'),
('SHP','Shampoo',              'Barang Dagangan'),
('PEW','Pewangi',              'Barang Dagangan'),
('KPC','Kapur Barus',          'Barang Dagangan'),
('PEL','Pel',                  'Barang Dagangan'),
('LAP','Lap',                  'Barang Dagangan'),
('TIS','Tisu',                 'Barang Dagangan'),
('SPL','Sampah Plastik',       'Barang Dagangan'),
('KMS','Kemoceng',             'Barang Dagangan'),
('SAP','Sapu',                 'Barang Dagangan'),
('SRB','Serbet',               'Barang Dagangan'),
('PBL','Pembersih Lantai',     'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- PERAWATAN TUBUH & KESEHATAN
-- ═══════════════════════════════════════════════════════════
('PRW','Perawatan Tubuh',      'Barang Dagangan'),
('PAS','Pasta Gigi',           'Barang Dagangan'),
('ODL','Odol',                 'Barang Dagangan'),
('SKG','Sikat Gigi',           'Barang Dagangan'),
('BED','Bedak',                'Barang Dagangan'),
('KPD','Kapas',                'Barang Dagangan'),
('TSB','Tisu Basah',           'Barang Dagangan'),
('HSN','Hand Sanitizer',       'Barang Dagangan'),
('MAS','Masker',               'Barang Dagangan'),
('VIT','Vitamin',              'Barang Dagangan'),
('OBT','Obat',                 'Barang Dagangan'),
('MKP','Minyak Kayu Putih',    'Barang Dagangan'),
('MTL','Minyak Telon',         'Barang Dagangan'),
('BDB','Bedak Bayi',           'Barang Dagangan'),
('POP','Popok',                'Barang Dagangan'),
('TTL','Tisu Toilet',          'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- ROKOK & ALAT TULIS
-- ═══════════════════════════════════════════════════════════
('ROK','Rokok',                'Barang Dagangan'),
('KRE','Kretek',               'Barang Dagangan'),
('FIL','Filter',               'Barang Dagangan'),
('KRK','Korek Api',            'Barang Dagangan'),
('ATS','Alat Tulis',           'Barang Dagangan'),
('BOL','Bola Pen',             'Barang Dagangan'),
('PSL','Pensil',               'Barang Dagangan'),
('BUK','Buku',                 'Barang Dagangan'),
('KER','Kertas',               'Barang Dagangan'),
('AMP','Amplop',               'Barang Dagangan'),
('STI','Stiker',               'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- ELEKTRONIK & AKSESORIS
-- ═══════════════════════════════════════════════════════════
('ELK','Elektronik',           'Barang Dagangan'),
('BAT','Baterai',              'Barang Dagangan'),
('LAM','Lampu',                'Barang Dagangan'),
('KBL','Kabel',                'Barang Dagangan'),
('CHG','Charger',              'Barang Dagangan'),
('HEA','Headset',              'Barang Dagangan'),
('EAR','Earphone',             'Barang Dagangan'),
('KIP','Kipas',                'Barang Dagangan'),
('RSE','Rice Cooker',          'Barang Dagangan'),
('KOM','Kompor',               'Barang Dagangan'),
('REG','Regulator',            'Barang Dagangan'),
('SLG','Selang Gas',           'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- PERLENGKAPAN RUMAH TANGGA
-- ═══════════════════════════════════════════════════════════
('PRT','Perlengkapan Rumah',   'Barang Dagangan'),
('PLS','Plastik',              'Barang Dagangan'),
('GEL','Gelas',                'Barang Dagangan'),
('PIR','Piring',               'Barang Dagangan'),
('MAN','Mangkok',              'Barang Dagangan'),
('TEK','Teko',                 'Barang Dagangan'),
('TRM','Termos',               'Barang Dagangan'),
('WAD','Wadah',                'Barang Dagangan'),
('TOP','Toples',               'Barang Dagangan'),
('TAL','Tali',                 'Barang Dagangan'),
('LEM','Lem',                  'Barang Dagangan'),
('LAK','Lakban',               'Barang Dagangan'),
('KAN','Kantong Plastik',      'Barang Dagangan'),
('SEN','Sendok Garpu',         'Barang Dagangan'),
('PIS','Pisau',                'Barang Dagangan'),
('TLR','Tali Rafia',           'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- GAS & BBM
-- ═══════════════════════════════════════════════════════════
('GAS','Gas LPG',              'Barang Dagangan'),
('GLP','Gas 3 Kg',             'Barang Dagangan'),
('GLB','Gas 12 Kg',            'Barang Dagangan'),
('BBM','BBM',                  'Barang Dagangan'),
('MNT','Minyak Tanah',         'Barang Dagangan'),
('SOL','Solar',                'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- SAYUR MAYUR & BUAH
-- ═══════════════════════════════════════════════════════════
('SAY','Sayur',                'Barang Dagangan'),
('BUH','Buah',                 'Barang Dagangan'),
('DAG','Daging',               'Barang Dagangan'),
('IKN','Ikan',                 'Barang Dagangan'),
('AYM','Ayam',                 'Barang Dagangan'),
('TLU','Telur',                'Barang Dagangan'),
('TMP','Tempe',                'Barang Dagangan'),
('TAH','Tahu',                 'Barang Dagangan'),

-- ═══════════════════════════════════════════════════════════
-- LAIN-LAIN
-- ═══════════════════════════════════════════════════════════
('PRF','Parfum',               'Barang Dagangan'),
('KOS','Kosmetik',             'Barang Dagangan'),
('KCM','Kacamata',             'Barang Dagangan'),
('JAM','Jam',                  'Barang Dagangan'),
('SPT','Sepatu',               'Barang Dagangan'),
('SND','Sandal',               'Barang Dagangan'),
('TPI','Topi',                 'Barang Dagangan'),
('PAY','Payung',               'Barang Dagangan'),
('KAS','Kasur',                'Barang Dagangan'),
('BAN','Bantal',               'Barang Dagangan'),
('SPB','Seprai Bedcover',      'Barang Dagangan'),
('HDK','Handuk',               'Barang Dagangan'),
('SAR','Sarung',               'Barang Dagangan'),
('SAJ','Sajadah',              'Barang Dagangan'),
('MUK','Mukena',               'Barang Dagangan')

ON DUPLICATE KEY UPDATE
    NAMA  = VALUES(NAMA),
    JENIS = VALUES(JENIS);

SET FOREIGN_KEY_CHECKS = 1;

-- Verifikasi
SELECT 'Total Kategori Default' AS keterangan, COUNT(*) AS jumlah
FROM tbl_kategori
WHERE LENGTH(KODE) <= 4;
