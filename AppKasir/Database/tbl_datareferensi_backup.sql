-- =============================================================================
-- BACKUP tbl_datareferensi — COA BARU FINAL
-- Sumber  : db_moroseneng
-- Tanggal : 2026-04-17
-- Total   : 52 akun
--
-- Cara pakai:
--   USE nama_database;
--   SOURCE tbl_datareferensi_backup.sql;
-- Atau jalankan langsung di MySQL Workbench / HeidiSQL / phpMyAdmin
-- =============================================================================

-- Kosongkan dulu jika ingin replace penuh (opsional, uncomment jika perlu)
-- TRUNCATE TABLE tbl_datareferensi;

-- Insert semua akun — gunakan INSERT IGNORE agar aman dijalankan berulang kali
INSERT IGNORE INTO tbl_datareferensi
    (STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, KETERANGAN)
VALUES
-- ── ASET LANCAR ─────────────────────────────────────────────────────────────
('Terkunci', 'ASET LANCAR', 'KAS',              '01.01.001', 'KAS DI TOKO',               'AKTIVA', 'DEBET',  'NERACA',    'Mencatat uang tunai fisik yang berada di mesin kasir/laci toko. Digunakan untuk transaksi penjualan tunai harian dan pengeluaran kecil di toko. Rekonsiliasi fisik dilakukan setiap tutup kasir.'),
('Terkunci', 'ASET LANCAR', 'KAS',              '01.01.002', 'KAS DI GUDANG',              'AKTIVA', 'DEBET',  'NERACA',    'Uang tunai kecil (petty cash) yang disimpan di gudang untuk operasional harian seperti pembelian bahan pembantu, bayar upah lepas, dsb. Sistem imprest atau fluktuasi.'),
('NULL',     'ASET LANCAR', 'KAS',              '01.01.003', 'KAS KIRIMAN TOKO',           'AKTIVA', 'DEBET',  'NERACA',    'Uang tunai yang sudah dikeluarkan dari bank atau kas gudang tetapi belum diterima secara fisik oleh toko. Digunakan saat arus kas antar lokasi. Harus segera direkonsiliasi.'),
('NULL',     'ASET LANCAR', 'KAS',              '01.01.004', 'KAS KIRIMAN GUDANG',         'AKTIVA', 'DEBET',  'NERACA',    'Uang tunai yang sedang dalam perjalanan menuju gudang (misal dari toko atau bank). Akun transit, saldo harus nol setelah periode tertentu.'),
('NULL',     'ASET LANCAR', 'BANK',             '01.02.001', 'TRANSFER BANK',              'AKTIVA', 'DEBET',  'NERACA',    'Seluruh rekening bank perusahaan (giro, tabungan, deposito). Mencatat semua penerimaan dan pengeluaran via transfer, kliring, atau setoran tunai. Rekonsiliasi bank setiap bulan wajib.'),
('NULL',     'ASET LANCAR', 'PIUTANG',          '01.03.001', 'PIUTANG USAHA',              'AKTIVA', 'DEBET',  'NERACA',    'Tagihan penjualan kredit kepada pelanggan/reseller yang jatuh tempo 12 bulan. Disajikan neto setelah cadangan kerugian piutang. Umur piutang dipantau rutin.'),
('Terkunci', 'ASET LANCAR', 'PIUTANG',          '01.03.002', 'PIUTANG KARYAWAN',           'AKTIVA', 'DEBET',  'NERACA',    'Uang yang dipinjam karyawan (kas bon) yang akan dipotong dari gaji atau dicicil. Jangka pendek. Tidak boleh dikapitalisasi sebagai beban.'),
('Terkunci', 'ASET LANCAR', 'A LANCAR',         '01.04.001', 'PERSEDIAAN BARANG',          'AKTIVA', 'DEBET',  'NERACA',    'Nilai barang dagang yang tersedia di gudang/toko (harga perolehan). Sistem pencatatan perpetual atau periodik. Dicatat sesuai PSAK 14 (persediaan).'),
('Terkunci', 'ASET LANCAR', 'A LANCAR',         '01.04.002', 'TAGIHAN / SALDO PIUTANG',    'AKTIVA', 'DEBET',  'NERACA',    'Tagihan lain-lain yang bersifat jangka pendek selain piutang usaha & karyawan. Contoh: uang muka pembelian, klaim asuransi, deposit sewa.'),
('NULL',     'ASET LANCAR', 'A LANCAR',         '01.04.003', 'PERLENGKAPAN KANTOR',        'AKTIVA', 'DEBET',  'NERACA',    'Stok perlengkapan kantor (kertas, tinta, alat tulis, dll) yang belum terpakai. Saat dipakai, diakui sebagai beban perlengkapan.'),
('NULL',     'ASET LANCAR', 'PAJAK AL',         '01.05.001', 'PPN MASUKAN',                'AKTIVA', 'DEBET',  'NERACA',    'PPN yang dibayar saat pembelian barang/jasa kena pajak. Dapat dikreditkan (di-offset) dengan PPN Keluaran. Disajikan sebagai aset pajak.'),

-- ── ASET TETAP ──────────────────────────────────────────────────────────────
('NULL',     'ASET TETAP',  'A TETAP',          '02.01.001', 'TANAH',                      'AKTIVA', 'DEBET',  'NERACA',    'Aset tetap berupa tanah (harga perolehan termasuk biaya perolehan hak). Tidak disusutkan. Disajikan sebesar harga perolehan.'),
('NULL',     'ASET TETAP',  'A TETAP',          '02.01.002', 'GEDUNG',                     'AKTIVA', 'DEBET',  'NERACA',    'Aset tetap berupa bangunan/gudang/toko (harga perolehan). Disusutkan metode garis lurus sesuai taksiran masa manfaat (umumnya 20 tahun).'),
('NULL',     'ASET TETAP',  'A TETAP',          '02.01.003', 'INVENTARIS',                 'AKTIVA', 'DEBET',  'NERACA',    'Aset tetap berupa perabotan, meja kursi, lemari, AC, komputer, mesin kantor (nilai signifikan). Disusutkan 4-8 tahun.'),
('NULL',     'ASET TETAP',  'A TETAP',          '02.01.004', 'KENDARAAN',                  'AKTIVA', 'DEBET',  'NERACA',    'Aset tetap berupa kendaraan operasional (mobil, motor, truck). Disusutkan 4-8 tahun. BPKB atas nama perusahaan.'),
('NULL',     'ASET TETAP',  'AKM PENY.',        '02.02.002', 'AKUM. PENY. GEDUNG',         'AKTIVA', 'KREDIT', 'NERACA',    'Akun kontra aset (pengurang nilai gedung). Mencatat akumulasi beban penyusutan gedung sejak perolehan. Saldo kredit.'),
('NULL',     'ASET TETAP',  'AKM PENY.',        '02.02.003', 'AKUM. PENY. INVENTARIS',     'AKTIVA', 'KREDIT', 'NERACA',    'Akun kontra aset (pengurang nilai inventaris). Mencatat akumulasi beban penyusutan inventaris. Saldo kredit.'),
('NULL',     'ASET TETAP',  'AKM PENY.',        '02.02.004', 'AKUM. PENY. KENDARAAN',      'AKTIVA', 'KREDIT', 'NERACA',    'Akun kontra aset (pengurang nilai kendaraan). Mencatat akumulasi beban penyusutan kendaraan. Saldo kredit.'),

-- ── PASIVA ──────────────────────────────────────────────────────────────────
('Terkunci', 'PASIVA',      'HUTANG',           '03.01.001', 'HUTANG BELANJA',             'PASIVA', 'KREDIT', 'NERACA',    'Hutang usaha kepada supplier atas pembelian barang dagang secara kredit (jatuh tempo 12 bulan). Dicatat saat faktur diterima.'),
('NULL',     'PASIVA',      'HUTANG',           '03.01.002', 'HUTANG USAHA',               'PASIVA', 'KREDIT', 'NERACA',    'Hutang lain-lain terkait operasional usaha selain pembelian barang dagang. Contoh: hutang jasa perbaikan, hutang konsultan.'),
('NULL',     'PASIVA',      'HUTANG',           '03.01.003', 'HUTANG LAIN LAIN',           'PASIVA', 'KREDIT', 'NERACA',    'Hutang non-operasional jangka pendek. Contoh: hutang kepada pihak ketiga bukan pemasok utama, uang muka pelanggan.'),
('Terkunci', 'PASIVA',      'BEBAN',            '03.02.001', 'HUTANG PAJAK',               'PASIVA', 'KREDIT', 'NERACA',    'Hutang Pajak Penghasilan (PPh) yang masih harus disetor: Pasal 21, 22, 23, 25, 29 (badan). KAP sesuai DJP.'),
('NULL',     'PASIVA',      'BEBAN',            '03.02.002', 'HUTANG BANK JANGKA PENDEK',  'PASIVA', 'KREDIT', 'NERACA',    'Pinjaman bank dengan jatuh tempo kurang dari 1 tahun (termasuk cicilan pokok utang jangka panjang yang jatuh tempo tahun berjalan).'),
('NULL',     'PASIVA',      'BEBAN',            '03.02.003', 'HUTANG BANK JANGKA PANJANG', 'PASIVA', 'KREDIT', 'NERACA',    'Pinjaman bank dengan jatuh tempo lebih dari 1 tahun. Disajikan setelah dikurangi bagian yang jatuh tempo dalam 1 tahun.'),
('NULL',     'PASIVA',      'PAJAK',            '03.02.004', 'PPN KELUARAN',               'PASIVA', 'KREDIT', 'NERACA',    'PPN yang dipungut dari pembeli saat penjualan barang/jasa kena pajak. Harus disetor ke negara selisihnya dengan PPN Masukan.'),
('NULL',     'PASIVA',      'SOSIAL',           '03.03.001', 'DANA KESEJAHTERAAN',         'PASIVA', 'KREDIT', 'NERACA',    'Dana sosial karyawan (koperasi, iuran kegiatan, sumbangan yang dipotong dari gaji atau kas perusahaan). Bersifat hutang kepada karyawan.'),

-- ── MODAL ───────────────────────────────────────────────────────────────────
('Terkunci', 'MODAL',       'EKUITAS',          '04.01.001', 'MODAL',                      'PASIVA', 'KREDIT', 'NERACA',    'Modal dasar yang disetor oleh pemilik perusahaan (setoran awal dan setoran tambahan yang disahkan). Akun permanen.'),
('Terkunci', 'MODAL',       'EKUITAS',          '04.01.002', 'MODAL PEMILIK',              'PASIVA', 'KREDIT', 'NERACA',    'Penyesuaian modal pemilik (misal revaluasi, tambahan investasi non-tunai, atau koreksi modal). Tidak untuk transaksi laba/rugi.'),
('Terkunci', 'MODAL',       'EKUITAS',          '04.01.003', 'REKENING KORAN PUSAT',       'PASIVA', 'KREDIT', 'NERACA',    'Akun penampung untuk transfer aset internal antar cabang/toko tanpa melibatkan kas. Digunakan saat memindahkan persediaan, kas, atau aset lain antar unit yang memiliki pembukuan terpisah namun satu entitas. Saldo normal kredit. Pada konsolidasi, saldo antar cabang saling menghapus.'),
('Terkunci', 'MODAL',       'PRIVE',            '04.02.001', 'PRIVE PEMILIK',              'PASIVA', 'DEBET',  'NERACA',    'Pengambilan uang atau aset perusahaan untuk keperluan pribadi pemilik. Mengurangi ekuitas. Saldo normal debet. Ditutup ke modal akhir tahun.'),
('Terkunci', 'MODAL',       'LABA RUGI',        '05.01.001', 'LABA RUGI BERJALAN',         'LABA RUGI', 'KREDIT', 'NERACA', 'Akun penampung laba/rugi tahun berjalan (ringkasan pendapatan & beban). Saldo kredit = laba, debet = rugi. Ditutup ke modal pada akhir periode.'),

-- ── PENJUALAN ────────────────────────────────────────────────────────────────
('Terkunci', 'PENJUALAN', 'PEND. KOTOR',   '05.02.001', 'PENJUALAN',                  'LABA', 'KREDIT', 'LABA RUGI', 'Pendapatan kotor dari penjualan barang dagang kepada pelanggan (belum dikurangi retur, diskon, dan potongan). Akun nominal.'),
('Terkunci', 'PENJUALAN', 'RETUR PEND.',   '05.03.001', 'RETUR PENJUALAN',            'LABA', 'DEBET',  'LABA RUGI', 'Pengembalian barang oleh pelanggan karena cacat atau tidak sesuai. Mengurangi penjualan kotor. Saldo normal debet.'),
('Terkunci', 'PENJUALAN', 'DISKON PEND.',  '05.04.001', 'POTONGAN DISKON PENJUALAN',  'LABA', 'DEBET',  'LABA RUGI', 'Potongan harga tunai yang diberikan kepada pelanggan (misal 2/10 n/30). Mengurangi penjualan. Akun kontra pendapatan.'),

-- ── HPP ─────────────────────────────────────────────────────────────────────
('Terkunci', 'HPP', 'HPP POKOK',    '06.01.001', 'HPP POKOK PENJUALAN',        'RUGI', 'DEBET',  'LABA RUGI', 'Harga perolehan barang yang terjual (COGS). Dihitung dari persediaan awal + pembelian bersih - persediaan akhir. Akun nominal debet.'),
('NULL',     'HPP', 'ANGKUT BELI', '06.02.001', 'BIAYA KIRIM PEMBELIAN',      'RUGI', 'DEBET',  'LABA RUGI', 'Biaya angkut pembelian (freight in) untuk mendatangkan barang ke gudang. Menambah nilai persediaan / HPP.'),
('NULL',     'HPP', 'ANGKUT JUAL', '06.03.001', 'BIAYA KIRIM PENJUALAN',      'RUGI', 'DEBET',  'LABA RUGI', 'Beban ongkos kirim (freight out) yang ditanggung perusahaan untuk mengirim barang ke pelanggan. Menambah HPP atau beban terpisah.'),
('NULL',     'HPP', 'PENY. STOK',  '06.04.001', 'PENYESUAIAN STOK MINUS',     'RUGI', 'DEBET',  'LABA RUGI', 'Pencatatan selisih kurang (rugi) saat stok opname: barang hilang, rusak, expired. Menambah HPP atau beban tersendiri.'),
('NULL',     'HPP', 'DISKON BELI', '06.05.001', 'POTONGAN DISKON PEMBELIAN',  'RUGI', 'KREDIT', 'LABA RUGI', 'Diskon yang diperoleh dari supplier karena pembayaran lebih awal (purchase discount). Mengurangi HPP. Akun kontra-HPP (kredit).'),
('NULL',     'HPP', 'RETUR BELI', '06.06.001', 'RETUR PEMBELIAN',            'RUGI', 'KREDIT', 'LABA RUGI', 'Pengembalian barang ke supplier karena cacat atau tidak sesuai. Mengurangi nilai pembelian/HPP. Akun kontra-HPP (kredit). Saldo normal kredit.'),

-- ── BIAYA ───────────────────────────────────────────────────────────────────
('Terkunci', 'BIAYA', 'BIAYA', '07.01.001', 'BEBAN GAJI KARYAWAN',         'RUGI', 'DEBET', 'LABA RUGI', 'Biaya gaji, upah, tunjangan, bonus, dan THR karyawan tetap & harian. Belum termasuk PPh 21 yang ditanggung karyawan.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.002', 'BEBAN PERLENGKAPAN ATK',      'RUGI', 'DEBET', 'LABA RUGI', 'Biaya pemakaian alat tulis kantor (ATK) yang sudah habis pakai dalam periode berjalan.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.003', 'BEBAN LISTRIK & AIR',         'RUGI', 'DEBET', 'LABA RUGI', 'Biaya utilitas: listrik (PLN), air (PDAM), telepon, internet untuk operasional kantor/toko/gudang.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.004', 'BEBAN BBM DAN ONGKOS KIRIM',  'RUGI', 'DEBET', 'LABA RUGI', 'Beban BBM kendaraan operasional dan ongkos kirim yang tidak terkait langsung dengan penjualan (misal antar gudang).'),
('NULL',     'BIAYA', 'BIAYA', '07.01.005', 'BEBAN PEMELIHARAAN GEDUNG',   'RUGI', 'DEBET', 'LABA RUGI', 'Biaya perbaikan, perawatan, cat, perpipaan, kebersihan gedung. Bukan biaya penyusutan.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.007', 'BEBAN PENYUSUTAN GEDUNG',     'RUGI', 'DEBET', 'LABA RUGI', 'Beban penyusutan aset tetap gedung per periode (metode garis lurus). Dikredit ke akumulasi penyusutan gedung.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.008', 'BEBAN PENYUSUTAN INVENTARIS', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban penyusutan inventaris/furniture per periode. Dikredit ke akumulasi penyusutan inventaris.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.009', 'BEBAN PENYUSUTAN KENDARAAN',  'RUGI', 'DEBET', 'LABA RUGI', 'Beban penyusutan kendaraan operasional per periode. Dikredit ke akumulasi penyusutan kendaraan.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.011', 'BEBAN ADM DAN BUNGA BANK',    'RUGI', 'DEBET', 'LABA RUGI', 'Biaya administrasi bank (fee bulanan, transfer, materai) dan bunga pinjaman bank. Bukan beban pajak.'),
('NULL',     'BIAYA', 'BIAYA', '07.01.012', 'BEBAN ADM DAN UMUM LAINNYA',  'RUGI', 'DEBET', 'LABA RUGI', 'Akun untuk beban operasional kecil lain-lain yang tidak material atau tidak sering terjadi: iuran, sumbangan (non-fiskal), denda, dsb.'),

-- ── PENDAPATAN LAIN ─────────────────────────────────────────────────────────
('NULL',     'PENDAPATAN LAIN', 'PEND. BUNGA', '08.01.001', 'PENDAPATAN BUNGA BANK', 'LABA', 'KREDIT', 'LABA RUGI', 'Pendapatan bunga dari saldo rekening giro, tabungan, atau deposito. Termasuk pendapatan lain-lain.'),
('Terkunci', 'PENDAPATAN LAIN', 'PEND. LAIN',  '08.01.002', 'PENDAPATAN LAIN LAIN',  'LABA', 'KREDIT', 'LABA RUGI', 'Pendapatan non-operasional: laba penjualan aset tetap, klaim asuransi, komplain supplier, hibah, dll. Perlu perhatian pajak.'),

-- ── PAJAK ───────────────────────────────────────────────────────────────────
('NULL',     'PAJAK', 'B PAJAK', '09.01.001', 'PAJAK PENGHASILAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban pajak penghasilan badan (PPh Badan) terutang untuk tahun berjalan (PPh 25/29). Bukan termasuk PPh 21/23 yang dipotong dari pihak lain.');

-- Verifikasi
SELECT COUNT(*) AS total_akun_inserted FROM tbl_datareferensi;
