-- ============================================================
-- 03_merk_default.sql
-- Data Default Merk/Brand Barang untuk Toko Retail di Indonesia
-- ============================================================
-- FORMAT KODE: max 4 karakter, generate otomatis oleh VB (GenerateSingkatan)
--
-- 1. SATU KATA  : 3 huruf pertama          → AQU, IND, BIM
-- 2. DUA KATA   : 1 huruf + 2 huruf        → GDA (Good Day), PSW (Pocari Sweat)
-- 3. TIGA+ KATA : 1 huruf dari 3 kata      → LWC (Luwak White Coffee)
-- 4. FALLBACK   : 3 huruf + 1 angka        → AQ1, AQ2 (jika duplikat)
--
-- Batas: VARCHAR(4) di database, MaxLength=4 di TextBox VB
-- ============================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

INSERT INTO tbl_merk (KODE, NAMA, KETERANGAN) VALUES

-- ═══════════════════════════════════════════════════════════
-- AIR MINUM
-- ═══════════════════════════════════════════════════════════
('AQU', 'Aqua',           'Air Minum'),
('LMI', 'Le Minerale',    'Air Minum'),
('VIT', 'Vit',            'Air Minum'),
('ADE', 'Ades',           'Air Minum'),
('PRI', 'Pristine',       'Air Minum'),
('OAS', 'Oasis',          'Air Minum'),
('CLU', 'Club',           'Air Minum'),

-- ═══════════════════════════════════════════════════════════
-- MINUMAN ENERGI & ISOTONIK
-- ═══════════════════════════════════════════════════════════
('TIG', 'Tiger',          'Minuman Energi'),
('KRA', 'Kratingdaeng',   'Minuman Energi'),
('M15', 'M-150',          'Minuman Energi'),
('KBI', 'Kuku Bima',      'Minuman Energi'),
('EJO', 'Extra Joss',     'Minuman Energi'),
('PSW', 'Pocari Sweat',   'Minuman Isotonik'),
('HCO', 'Hydro Coco',     'Minuman Kelapa'),
('TSL', 'Tropicana Slim', 'Minuman Diet'),

-- ═══════════════════════════════════════════════════════════
-- TEH & KOPI
-- ═══════════════════════════════════════════════════════════
('SOS', 'Sosro',          'Teh'),
('FRE', 'Frestea',        'Teh'),
('TTJ', 'Tong Tji',       'Teh'),
('PHU', 'Pucuk Harum',    'Teh'),
('GOL', 'Golda',          'Kopi'),
('KAP', 'Kapal Api',      'Kopi'),
('ABC', 'ABC',            'Kopi & Kecap'),
('TOP', 'Top',            'Kopi'),
('GDA', 'Good Day',       'Kopi'),
('TOR', 'Torabika',       'Kopi'),
('LWC', 'Luwak White Coffee', 'Kopi'),

-- ═══════════════════════════════════════════════════════════
-- SUSU
-- ═══════════════════════════════════════════════════════════
('NES', 'Nestle',         'Susu'),
('INM', 'Indomilk',       'Susu'),
('DAN', 'Dancow',         'Susu'),
('FFL', 'Frisian Flag',   'Susu'),
('UMI', 'Ultra Milk',     'Susu'),
('BBR', 'Bear Brand',     'Susu'),
('CAR', 'Carnation',      'Susu'),
('MIL', 'Milo',           'Minuman Coklat'),
('OVA', 'Ovaltine',       'Minuman Coklat'),

-- ═══════════════════════════════════════════════════════════
-- MIE INSTAN
-- ═══════════════════════════════════════════════════════════
('IND', 'Indomie',        'Mie Instan'),
('MSE', 'Mie Sedaap',     'Mie Instan'),
('SAR', 'Sarimi',         'Mie Instan'),
('SUP', 'Supermi',        'Mie Instan'),
('PMI', 'Pop Mie',        'Mie Instan'),

-- ═══════════════════════════════════════════════════════════
-- SNACK & MAKANAN RINGAN
-- ═══════════════════════════════════════════════════════════
('CHI', 'Chitato',        'Snack'),
('LAY', 'Lays',           'Snack'),
('DOR', 'Doritos',        'Snack'),
('TAR', 'Taro',           'Snack'),
('KUS', 'Kusuka',         'Snack'),
('PIL', 'Pillow',         'Snack'),
('QTE', 'Qtela',          'Snack'),
('PRG', 'Pringles',       'Snack'),

-- ═══════════════════════════════════════════════════════════
-- PERMEN & COKLAT
-- ═══════════════════════════════════════════════════════════
('TTA', 'Tic Tac',        'Permen'),
('MEN', 'Mentos',         'Permen'),
('FIS', 'Fisherman',      'Permen'),
('KIT', 'KitKat',         'Coklat'),
('SNI', 'Snickers',       'Coklat'),
('DMI', 'Dairy Milk',     'Coklat'),
('FER', 'Ferrero',        'Coklat'),
('TOB', 'Toblerone',      'Coklat'),

-- ═══════════════════════════════════════════════════════════
-- BISKUIT
-- ═══════════════════════════════════════════════════════════
('ORE', 'Oreo',           'Biskuit'),
('RIT', 'Ritz',           'Biskuit'),
('ROM', 'Roma',           'Biskuit'),
('KGU', 'Khong Guan',     'Biskuit'),
('GTI', 'Good Time',      'Biskuit'),

-- ═══════════════════════════════════════════════════════════
-- ROKOK
-- ═══════════════════════════════════════════════════════════
('GGA', 'Gudang Garam',   'Rokok'),
('SAM', 'Sampoerna',      'Rokok'),
('DJA', 'Djarum',         'Rokok'),
('WIS', 'Wismilak',       'Rokok'),
('MAR', 'Marlboro',       'Rokok'),
('LLI', 'LA Lights',      'Rokok'),
('CMI', 'Class Mild',     'Rokok'),
('SUR', 'Surya',          'Rokok'),
('DUN', 'Dunhill',        'Rokok'),
('PMO', 'Philip Morris',  'Rokok'),

-- ═══════════════════════════════════════════════════════════
-- MINYAK GORENG & SEMBAKO
-- ═══════════════════════════════════════════════════════════
('BIM', 'Bimoli',         'Minyak Goreng'),
('FOR', 'Fortune',        'Minyak Goreng'),
('SAN', 'Sania',          'Minyak Goreng'),
('TRO', 'Tropical',       'Minyak Goreng'),
('FIL', 'Filma',          'Minyak Goreng'),
('SBI', 'Segitiga Biru',  'Tepung'),
('GMA', 'Gunung Mas',     'Tepung'),
('GUL', 'Gulaku',         'Gula'),
('MON', 'Monas',          'Gula'),

-- ═══════════════════════════════════════════════════════════
-- KECAP & SAUS
-- ═══════════════════════════════════════════════════════════
('BAN', 'Bango',          'Kecap'),
('SED', 'Sedap',          'Kecap'),
('DBE', 'Dua Belibis',    'Kecap'),
('DMO', 'Del Monte',      'Saus'),
('HEI', 'Heinz',          'Saus'),

-- ═══════════════════════════════════════════════════════════
-- DETERJEN & PEMBERSIH
-- ═══════════════════════════════════════════════════════════
('RIN', 'Rinso',          'Deterjen'),
('DAI', 'Daia',           'Deterjen'),
('ATT', 'Attack',         'Deterjen'),
('SOK', 'Soklin',         'Deterjen'),
('BRE', 'Breeze',         'Deterjen'),
('WIP', 'Wipol',          'Pembersih'),
('MMU', 'Mr. Muscle',     'Pembersih'),
('DET', 'Dettol',         'Antiseptik'),

-- ═══════════════════════════════════════════════════════════
-- SABUN & PERAWATAN TUBUH
-- ═══════════════════════════════════════════════════════════
('LIF', 'Lifebuoy',       'Sabun'),
('LUX', 'Lux',            'Sabun'),
('NIV', 'Nivea',          'Sabun & Perawatan'),
('GIV', 'Giv',            'Sabun'),
('CIT', 'Citra',          'Sabun'),
('BIO', 'Biore',          'Sabun & Perawatan'),
('PRT', 'Protex',         'Sabun'),
('SHI', 'Shinzui',        'Sabun'),

-- ═══════════════════════════════════════════════════════════
-- PEWANGI & TISU
-- ═══════════════════════════════════════════════════════════
('STE', 'Stella',         'Pewangi'),
('GAN', 'Gantelle',       'Pewangi'),
('PAS', 'Paseo',          'Tisu'),
('TEM', 'Tempo',          'Tisu'),
('KLX', 'Kleenex',        'Tisu'),
('CHA', 'Charm',          'Tisu'),

-- ═══════════════════════════════════════════════════════════
-- PASTA GIGI & KESEHATAN
-- ═══════════════════════════════════════════════════════════
('COL', 'Colgate',        'Pasta Gigi'),
('PEP', 'Pepsodent',      'Pasta Gigi'),
('KDM', 'Kodomo',         'Pasta Gigi'),
('SEN', 'Sensodyne',      'Pasta Gigi'),
('CUP', 'Close Up',       'Pasta Gigi'),
('CIP', 'Ciptadent',      'Pasta Gigi'),
('FRM', 'Formula',        'Pasta Gigi'),
('BET', 'Betadine',       'Antiseptik'),
('ANT', 'Antangin',       'Obat'),
('TAN', 'Tolak Angin',    'Obat'),
('BOD', 'Bodrex',         'Obat'),
('PAN', 'Panadol',        'Obat'),

-- ═══════════════════════════════════════════════════════════
-- PRODUK BAYI
-- ═══════════════════════════════════════════════════════════
('ENF', 'Enfagrow',       'Susu Bayi'),
('SHU', 'Sari Husada',    'Susu Bayi'),
('CER', 'Cerelac',        'Makanan Bayi'),
('CBA', 'Cussons Baby',   'Produk Bayi'),
('ZWI', 'Zwitsal',        'Produk Bayi'),
('MPO', 'Mamy Poko',      'Popok'),
('SWE', 'Sweety',         'Popok'),
('PAM', 'Pampers',        'Popok'),

-- ═══════════════════════════════════════════════════════════
-- BUMBU DAPUR
-- ═══════════════════════════════════════════════════════════
('AJI', 'Ajinomoto',      'Bumbu Dapur'),
('MAS', 'Masako',         'Bumbu Dapur'),
('ROY', 'Royco',          'Bumbu Dapur'),
('SAJ', 'Sajiku',         'Bumbu Dapur'),
('KOK', 'Kokita',         'Bumbu'),
('KOB', 'Kobe',           'Bumbu'),
('KAR', 'Kara',           'Santan'),
('MRJ', 'Marjan',         'Sirup'),

-- ═══════════════════════════════════════════════════════════
-- BATERAI & KOREK API
-- ═══════════════════════════════════════════════════════════
('ENE', 'Energizer',      'Baterai'),
('DUR', 'Duracell',       'Baterai'),
('NIP', 'Nippo',          'Baterai'),
('PAN', 'Panasonic',      'Baterai'),

-- ═══════════════════════════════════════════════════════════
-- ALAT TULIS
-- ═══════════════════════════════════════════════════════════
('SNO', 'Snowman',        'Spidol & Tinta'),
('FCA', 'Faber-Castell',  'Alat Tulis'),
('SHA', 'Sharpie',        'Spidol'),
('PLT', 'Pilot',          'Pulpen'),
('UNI', 'Uniball',        'Pulpen'),
('PNT', 'Pentel',         'Pulpen'),
('ZEB', 'Zebra',          'Pulpen'),
('PON', 'Paper One',      'Kertas'),

-- ═══════════════════════════════════════════════════════════
-- ELEKTRONIK & PERLENGKAPAN RUMAH
-- ═══════════════════════════════════════════════════════════
('PHI', 'Philips',        'Elektronik'),
('PNA', 'Panasonic',      'Elektronik'),
('LG',  'LG',             'Elektronik'),
('SSG', 'Samsung',        'Elektronik'),
('SHA', 'Sharp',          'Elektronik'),
('MKI', 'Miyako',         'Rice Cooker & Kipas'),
('COS', 'Cosmos',         'Rice Cooker & Kipas'),
('MSP', 'Maspion',        'Perlengkapan Dapur'),
('PER', 'Pertamina',      'Gas & BBM')

ON DUPLICATE KEY UPDATE
    NAMA        = VALUES(NAMA),
    KETERANGAN  = VALUES(KETERANGAN);

SET FOREIGN_KEY_CHECKS = 1;

-- Verifikasi
SELECT 'Total Merk Default' AS keterangan, COUNT(*) AS jumlah
FROM tbl_merk;
