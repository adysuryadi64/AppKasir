# 🚀 AppKasir 2026 - Enterprise POS & Accounting System

**Versi:** 2.1.0 (Refactored & Stabilized)  
**Bahasa:** Visual Basic .NET (VB.NET)  
**Framework:** .NET Framework 4.7.2+ / .NET 6+  
**Database:** MySQL 5.7+ / 8.0+  
**UI Framework:** Windows Forms  
**Repository:** https://github.com/adysuryadi64/AppKasir

---

## 📌 Ringkasan Aplikasi

**AppKasir** adalah sistem **Point-of-Sale (POS)** dan **Akuntansi** terpadu yang dirancang untuk skala ritel menengah hingga besar. Berbeda dengan POS standar, AppKasir menggunakan pendekatan **Double-Entry Bookkeeping** dan **Atomic Stock Counters** untuk menjamin akurasi data 100% antara stok fisik, laporan keuangan, dan histori transaksi.

### ⭐ Keunggulan Utama
- 🛒 **Pembelian multi-item** dengan pencarian barang real-time (Rapid Entry `*`)
- 🔍 **Barcode scanner support** dengan deteksi hybrid (otomatis vs manual)
- 📦 **Sistem satuan multi-level** (kecil, sedang, besar / umum, partai)
- 🧮 **Perhitungan diskon & pajak** fleksibel (persen/nominal per item dan per transaksi)
- 💳 **Berbagai metode pembayaran** (tunai, transfer bank, QRIS, tempo)
- 📊 **Manajemen stok** (toko & gudang terpisah dengan sistem counter)
- 🖨️ **Pencetakan nota** otomatis atau manual
- ⏳ **Penahanan transaksi** (hold/recall untuk kemudian)
- 🏦 **Integrasi jurnal akuntansi** (double-entry bookkeeping real-time)
- 🔐 **Kontrol akses berbasis role** (hak akses per pengguna)

---

## 🏗️ Arsitektur & Teknologi Inti

Aplikasi ini menggunakan standar Enterprise POS dengan tingkat integritas data yang sangat tinggi melalui dua pilar utama:

### 1. Centralized Deletion Logic (`ModuleHapusTransaksi`)
Seluruh proses pembatalan atau perubahan transaksi (Edit) dipusatkan di satu modul. Ini menjamin alur **Reversal** yang atomik:
- **Piutang/Hutang Reversal**: Saldo tagihan pelanggan/supplier dikembalikan secara otomatis sebelum record dihapus.
- **Jurnal Reversal**: Record `JurnalUmum` dihapus dan saldo akun di `tbl_datareferensi` disinkronkan kembali.
- **Stock Reversal**: Counter barang dibalik secara presisi untuk menghindari selisih stok fisik.

### 2. Counter-Based Inventory System
Aplikasi tidak melakukan update langsung pada kolom stok akhir. Sebagai gantinya, setiap transaksi menggerakkan **Counter** di `tbl_barang`:
- **Counters**: `PEMBELIAN_TOKO`, `PENJUALAN_TOKO`, `RETUR_BELI_TOKO`, `RETUR_JUAL_TOKO`, dsb.
- **Sync Engine**: Prosedur `HitungStokPerubahan` secara rutin mensinkronkan:  
  `STOK_TOKO = (STOK_AWAL_TOKO + PEMBELIAN_TOKO + RETUR_JUAL_TOKO) - (PENJUALAN_TOKO + RETUR_BELI_TOKO)`

---

## 📂 Struktur Proyek Terkini (Full Architecture)

```text
AppKasir/
├── 0Form/                  # 🖥️ Dashboard & Central Control
│   ├── FormCekUpdate.vb    # Pengecekan pembaruan aplikasi
│   ├── FormKonfirmasi.vb   # Dialog konfirmasi aksi kritis
│   ├── FormLoading.vb      # Splash screen & Inisialisasi awal
│   ├── FormMasuk.vb        # Form Login / Masuk Aplikasi
│   ├── FormPilihanMasuk.vb # Pemilihan mode/cabang saat masuk
│   ├── FormUtama.vb        # Main Dashboard aplikasi
│   ├── ModuleHapusTransaksi.vb # Logika tersentralisasi untuk reversal data
│   ├── Notifikasi.vb       # Pop-up notifikasi umum
│   └── NotifikasiJatuhTempo.vb # Popup alert piutang/hutang jatuh tempo
│
├── 1Master/                # 📁 Manajemen Data Master
│   ├── CetakLabelBarcodeTSPL.vb # Engine Cetak Label Barcode TSPL
│   ├── FormArmada.vb       # Manajemen Armada/Kendaraan
│   ├── FormBarang.vb       # Daftar Master Barang
│   ├── FormCabang.vb       # Manajemen Multi-Cabang
│   ├── FormCetakBarcode.vb # Utility cetak barcode
│   ├── FormCetakLabel.vb   # Utility cetak label rak
│   ├── FormCompany.vb      # Pengaturan Profil Perusahaan
│   ├── FormFlash.vb        # Splash/Flash screen
│   ├── FormGeneralSetting.vb # Konfigurasi Umum Sistem
│   ├── FormHakUser.vb      # Manajemen Hak Akses Pengguna
│   ├── FormKaryawan.vb     # Master Data Karyawan
│   ├── FormLogin.vb        # Form Autentikasi User
│   ├── FormTabelReferensi.vb # Master Akun Buku Besar (COA)
│   ├── FormUser.vb         # Master Pengguna Sistem
│   ├── HistoriPembelianUC.vb # UserControl Riwayat Pembelian
│   ├── TambahBarang.vb     # Form Input/Edit Master Produk
│   ├── TambahKategori.vb   # Master Kategori Barang
│   ├── TambahMerk.vb       # Master Merk Barang
│   ├── TambahPelanggan.vb  # Master Data Pelanggan
│   ├── TambahSatuan.vb     # Master Satuan Multi-Level
│   └── TambahSupliyer.vb   # Master Data Supplier
│
├── 2Trans/                 # 🛒 Modul Transaksi Operasional
│   ├── FormBayarHutang.vb  # Pembayaran Hutang Supplier
│   ├── FormBayarPiutang.vb # Penerimaan Pembayaran Piutang
│   ├── FormEditBayarJual.vb # Koreksi Pembayaran Penjualan
│   ├── FormJual.vb         # Kasir Penjualan Retail & Partai
│   ├── FormPembelian.vb    # Pembelian / Restock Barang
│   ├── FormPembelianDitahan.vb # Hold & Recall Transaksi Pembelian
│   ├── FormPenjualanDitahan.vb # Hold & Recall Transaksi Penjualan
│   ├── FormReturBeli.vb    # Form Retur Pembelian
│   ├── FormReturPembelian.vb # Modul Retur Pembelian (Varian)
│   ├── FormReturPenjualan.vb # Retur Penjualan dari Pelanggan
│   ├── FormStokOpname.vb   # Penyesuaian Stok Fisik & Sistem
│   ├── FormStokOpnameBahan.vb # Penyesuaian Stok Bahan Baku
│   ├── FormSuratJalan.vb   # Pembuatan Surat Jalan Pengiriman
│   ├── FormTransferBarang.vb # Mutasi Barang Internal
│   ├── FormTransferCabang.vb # Pemindahan Stok antar Cabang
│   ├── FormTransferStok.vb # Modul Transfer Stok
│   ├── NotaStokOpname.vb   # Handler Cetak Nota Stok Opname
│   ├── NotaSuratJalan.vb   # Handler Cetak Nota Surat Jalan
│   └── NotaTransferBarang.vb # Handler Cetak Nota Transfer Barang
│
├── 3Jurnal/                # 📓 Akuntansi & Keuangan
│   └── FormKeuangan.vb     # Modul Manajemen Jurnal & Keuangan Terpadu
│
├── 4Gaji/                  # 👥 Manajemen SDM & Payroll
│   ├── FormBon.vb          # Manajemen Kasbon Karyawan
│   ├── FormGaji.vb         # Proses Penggajian Utama
│   ├── FormLapBon.vb       # Laporan Kasbon Keseluruhan
│   ├── FormLapBonPerorang.vb # Laporan Kasbon per Karyawan
│   ├── FormLaporanGaji.vb  # Laporan Rekapitulasi Gaji
│   └── FormMasterGaji.vb   # Master Komponen Gaji Dasar
│
├── 5Lap/                   # 📊 Reporting & Analitik Bisnis
│   ├── FormAuditTrail.vb   # Pelacakan Aktivitas User
│   ├── FormAuditTrailArsip.vb # Arsip Pelacakan Aktivitas
│   ├── FormGrafikLaba.vb   # Grafik Analisa Laba
│   ├── FormKartuStok.vb    # Kartu Stok Barang
│   ├── FormLapBB.vb        # Laporan Buku Besar Utama
│   ├── FormLapBBPembantu.vb # Laporan Buku Besar Pembantu
│   ├── FormLapBarang.vb    # Laporan Master Barang
│   ├── FormLapBarangTerlaris.vb # Laporan Ranking Barang Terlaris
│   ├── FormLapHutang.vb    # Laporan Hutang ke Supplier
│   ├── FormLapJurnal.vb    # Laporan Detail Jurnal Transaksi
│   ├── FormLapLabaRugi.vb  # Laporan Laba Rugi Komprehensif
│   ├── FormLapMarginProfit.vb # Analisis Margin Profit
│   ├── FormLapMutasiBarang.vb # Laporan Mutasi Stok Fisik
│   ├── FormLapNeracaLR.vb  # Laporan Neraca & Laba Rugi
│   ├── FormLapOmset.vb     # Analisa Pendapatan & Omset
│   ├── FormLapPembelian.vb # Rekapitulasi Pembelian
│   ├── FormLapPenjualanBaru.vb # Rekapitulasi Penjualan
│   ├── FormLapPenjualanSales.vb # Kinerja Penjualan per Sales
│   ├── FormLapPiutang.vb   # Laporan Piutang Pelanggan
│   ├── FormLapRanking.vb   # Ranking Penjualan/Kinerja
│   ├── FormLapRankingTagihan.vb # Ranking Tagihan Tertinggi
│   ├── FormLapRekapBayar.vb # Rekapitulasi Pembayaran
│   ├── FormLapReturBeli.vb # Laporan Retur Pembelian
│   ├── FormLapReturJual.vb # Laporan Retur Penjualan
│   ├── FormLapStokLampau.vb # Analisa Stok Periode Lampau
│   ├── FormLapStokMinim_takGerak.vb # Peringatan Stok Minimum/Mati
│   ├── FormLapTransferBarang.vb # Riwayat Transfer Barang
│   ├── FormLapTransferStok.vb # Riwayat Transfer Stok
│   ├── FormLapkAS.vb       # Laporan Arus Kas
│   ├── FormNotifHutang.vb  # Alert Hutang Jatuh Tempo
│   ├── FormNotifPiutang.vb # Alert Piutang Jatuh Tempo
│   ├── FormPenjualanPPn.vb # Laporan Penjualan dgn Pajak
│   ├── FormRopertJual.vb   # Custom Report Penjualan
│   ├── FormStokBarang.vb   # Posisi Stok Saat Ini
│   └── *.rdlc / *.frx      # Puluhan template ReportViewer & FastReport
│
├── 6Print/                 # 🖨️ Engine Cetak Hardware
│   ├── CetakBayarHutang/   # Modul Cetak Bukti Bayar Hutang
│   │   ├── EscPosCetakBayarHutang.vb
│   │   ├── GdiCetakBayarHutang.vb
│   │   ├── ModuleCetakBayarHutangInkjet.vb
│   │   ├── ModuleCetakBayarHutangPdf.vb
│   │   └── ModulePrinterBayarHutang.vb
│   ├── CetakBayarPiutang/  # Modul Cetak Bukti Terima Piutang
│   │   ├── EscPosCetakBayarPiutang.vb
│   │   ├── GdiCetakBayarPiutang.vb
│   │   ├── ModuleCetakBayarPiutangInkjet.vb
│   │   ├── ModuleCetakBayarPiutangPdf.vb
│   │   └── ModulePrinterBayarPiutang.vb
│   ├── CetakBonKaryawan/   # Modul Cetak Slip Kasbon
│   │   ├── EscPosCetakBonKaryawan.vb
│   │   ├── GdiCetakBonKaryawan.vb
│   │   ├── ModuleCetakBonKaryawanInkjet.vb
│   │   ├── ModuleCetakBonKaryawanPdf.vb
│   │   └── ModulePrinterBonKaryawan.vb
│   ├── CetakGajiKaryawan/  # Modul Cetak Slip Gaji
│   │   ├── EscPosCetakGajiKaryawan.vb
│   │   ├── GdiCetakGajiKaryawan.vb
│   │   ├── ModuleCetakGajiKaryawanInkjet.vb
│   │   ├── ModuleCetakGajiKaryawanPdf.vb
│   │   └── ModulePrinterGajiKaryawan.vb
│   ├── CetakLaporanKas/    # Modul Cetak Rekapitulasi Kas
│   │   ├── EscPosCetakLaporanKas.vb
│   │   ├── FormLapMutasiKeuangan.Designer.vb
│   │   ├── FormLapMutasiKeuangan.resx
│   │   ├── FormLapMutasiKeuangan.vb
│   │   ├── GdiCetakLaporanKas.vb
│   │   ├── ModuleCetakLaporanKasInkjet.vb
│   │   ├── ModuleCetakLaporanKasPdf.vb
│   │   └── ModulePrinterLaporanKas.vb
│   ├── CetakPembelian/     # Modul Cetak Faktur Pembelian
│   │   ├── EscPosCetakBeliThermalMatrik.vb
│   │   ├── GdiCetakBeliThermalMatrik.vb
│   │   ├── ModuleCetakBeliInkjet.vb
│   │   ├── ModuleCetakBeliPdf.vb
│   │   ├── ModulePrinterBeli.vb
│   │   ├── NotaPembelian.Designer.vb
│   │   ├── NotaPembelian.rdlc
│   │   ├── NotaPembelian.resx
│   │   └── NotaPembelian.vb
│   ├── CetakPenjualan/     # Modul Cetak Struk/Nota Penjualan
│   │   ├── EscPosCetakjualThermalMatrik.vb
│   │   ├── FormMonitorRDLC.Designer.vb
│   │   ├── FormMonitorRDLC.resx
│   │   ├── FormMonitorRDLC.vb
│   │   ├── GdiCetakjualThermalMatrik.vb
│   │   ├── ModuleCetakJualInkjet.vb
│   │   ├── ModuleCetakJualPdf.vb
│   │   ├── ModulePrinterJual.vb
│   │   └── ReportCetakJual.rdlc
│   ├── CetakReturBeli/     # Modul Cetak Bukti Retur Beli
│   │   ├── EscPosCetakReturBeliThermalMatrik.vb
│   │   ├── GdiCetakReturBeliThermalMatrik.vb
│   │   ├── ModuleCetakReturBeliInkjet.vb
│   │   ├── ModuleCetakReturBeliPdf.vb
│   │   ├── ModulePrinterReturBeli.vb
│   │   ├── PrintReturBeli.Designer.vb
│   │   ├── PrintReturBeli.resx
│   │   └── PrintReturBeli.vb
│   ├── CetakReturJual/     # Modul Cetak Bukti Retur Jual
│   │   ├── EscPosCetakReturJualThermalMatrik.vb
│   │   ├── GdiCetakReturJualThermalMatrik.vb
│   │   ├── ModuleCetakReturJualInkjet.vb
│   │   ├── ModuleCetakReturJualPdf.vb
│   │   ├── ModulePrinterReturJual.vb
│   │   ├── PrintReturJual.Designer.vb
│   │   ├── PrintReturJual.resx
│   │   └── PrintReturJual.vb
│   ├── CetakStokOpname/    # Modul Cetak Hasil Stok Opname
│   │   ├── EscPosCetakStokOpname.vb
│   │   ├── GdiCetakStokOpname.vb
│   │   ├── ModuleCetakStokOpnameInkjet.vb
│   │   ├── ModuleCetakStokOpnamePdf.vb
│   │   └── ModulePrinterStokOpname.vb
│   ├── CetakSuratJalan/    # Modul Cetak Surat Jalan
│   │   ├── EscPosCetakSuratJalan.vb
│   │   ├── GdiCetakSuratJalan.vb
│   │   ├── ModuleCetakSuratJalanInkjet.vb
│   │   ├── ModuleCetakSuratJalanPdf.vb
│   │   ├── ModulePrinterSuratJalan.vb
│   │   ├── PrinterSuratJalan.Designer.vb
│   │   ├── PrinterSuratJalan.resx
│   │   └── PrinterSuratJalan.vb
│   ├── CetakTransferBarang/ # Modul Cetak Mutasi Internal
│   │   ├── EscPosCetakTransferBarang.vb
│   │   ├── GdiCetakTransferBarang.vb
│   │   ├── ModuleCetakTransferBarangInkjet.vb
│   │   ├── ModuleCetakTransferBarangPdf.vb
│   │   ├── ModulePrinterTransferBarang.vb
│   │   ├── PrintTransferBarang.Designer.vb
│   │   ├── PrintTransferBarang.resx
│   │   └── PrintTransferBarang.vb
│   ├── CetakTransferCabang/ # Modul Cetak Mutasi Cabang
│   │   ├── EscPosCetakTransferCabang.vb
│   │   ├── GdiCetakTransferCabang.vb
│   │   ├── ModuleCetakTransferCabangInkjet.vb
│   │   ├── ModuleCetakTransferCabangPdf.vb
│   │   └── ModulePrinterTransferCabang.vb
│   ├── CetakTransferStok/  # Modul Cetak Pemindahan Stok
│   │   ├── EscPosCetakTransferStok.vb
│   │   ├── GdiCetakTransferStok.vb
│   │   ├── ModuleCetakTransferStokInkjet.vb
│   │   ├── ModuleCetakTransferStokPdf.vb
│   │   └── ModulePrinterTransferStok.vb
│   ├── DOKUMENTASI_6PRINT.md # Dokumentasi Engine Print
│   ├── FormPengaturanPrinter.designer.vb
│   ├── FormPengaturanPrinter.resx
│   ├── FormPengaturanPrinter.vb # Setting hardware printer & port
│   ├── ModuleKonfigurasi.vb # Parameter konfigurasi alat cetak
│   ├── PrinterEscPos.vb    # Direct-to-port thermal printing command
│   └── RawPrinterHelper.vb # API Spooler Windows bypass
│
├── 7Reg/                   # 🔐 Manajemen Lisensi & Keamanan
│   ├── ACTIVATION_FORM.vb  # Form Aktivasi Aplikasi
│   ├── KeyGenerator.vb     # Generator Serial Number
│   ├── ModuleReg.vb        # Modul Utilitas Registrasi
│   └── SecurityManager.vb  # Keamanan dan Validasi Lisensi
│
├── 8Uty/                   # ⚙️ Utilitas & Database Enterprise
│   ├── FormAbout.vb        # Informasi Aplikasi & Versi
│   ├── FormHapusTransaksi.vb # Utility Hapus/Reset Transaksi
│   ├── FormHistory.vb      # Riwayat Aktivitas & Log Historis
│   ├── FormMacAddres.vb    # Security Check by MAC Address
│   ├── FormMigrasiDB.vb    # Auto-updater struktur database
│   ├── FormPerbaikanDatabase.vb # Perbaikan Anomali Data Tabel
│   ├── FormQuery.vb        # Terminal Eksekusi Kueri SQL Manual
│   ├── FormUpdateTabelDb.vb # Engine Sinkronisasi Skema DB
│   ├── Formipkomputer.vb   # Network Configuration Info
│   └── SettingDatabase.vb  # Pengaturan Koneksi Server MySQL
│
├── 9Sync/                  # 🔄 Sinkronisasi Cloud (Supabase)
│   ├── FLUTTER_LAPORAN_PLAN.md # Perencanaan Laporan Flutter
│   ├── FormSync.vb         # UI Kontrol Sinkronisasi Manual
│   ├── README_INTEGRASI.md # Dokumentasi Integrasi Supabase
│   ├── SupabaseHelper.vb   # REST API Helper untuk Supabase
│   ├── SyncConfig.vb       # Konfigurasi Koneksi Cloud
│   ├── SyncLog.vb          # Pencatatan Log Sinkronisasi
│   ├── SyncManager.vb      # Engine Sinkronisasi Utama
│   ├── SyncQueue.vb        # Antrian Data Tersinkronisasi
│   └── SyncTrigger.vb      # Pemicu Sinkronisasi Otomatis
│
├── AppAndroid/             # 📱 Mobile App (Flutter)
│   ├── android/            # Proyek Native Android
│   ├── api/                # Endpoint & Services API
│   ├── assets/             # Assets Gambar & Ikon Mobile
│   ├── ios/                # Proyek Native iOS
│   ├── lib/                # Source Code Utama Aplikasi (Dart)
│   ├── test/               # Unit Test Flutter
│   ├── pubspec.yaml        # Konfigurasi Dependency Flutter
│   └── README.md           # Dokumentasi Aplikasi Mobile
│
├── Database/               # 🗄️ SQL Scripts & Migrations
│   ├── 01_migrasi_kolom.sql # Migrasi penambahan kolom
│   ├── 02_migrasi_sync_setup_supabase.sql # Migrasi sinkronisasi Supabase
│   ├── 03_cleanup_index.sql # Pembersihan indeks redundan
│   ├── 03_migrasi_index.sql # Penambahan indeks baru
│   ├── 04_migrasi_collation.sql # Penyeragaman Collation UTF8
│   ├── 05_migrasi_audit_trail.sql # Migrasi tabel log jejak
│   ├── 06_migrasi_stored_procedures.sql # Migrasi trigger & SP dasar
│   ├── 07_migrasi_sp_transaksi.sql # Migrasi SP Transaksi Utama
│   ├── 08_hapus_sp_lama.sql # Pembersihan SP Usang
│   ├── 09_standarisasi_jenis_transaksi.sql # Penyeragaman enum tipe transaksi
│   ├── 10_migrasi_coa_dari_kode_lama.sql # Migrasi sistem COA lama
│   ├── 11_migrasi_akun_coa.sql # Setting Akun COA default
│   ├── 12_hapus_index_orphan.sql # Hapus indeks tak terpakai
│   ├── 13_trim_cleanup_barang.sql # Cleanup data spasi tabel barang
│   ├── 14_resize_varchar.sql # Penyesuaian ukuran varchar 
│   ├── 15_sinkronisasi_temp_datareferensi.sql # Sinkronisasi tabel temp
│   ├── 16_sp_hlp_stok_ambil.sql # SP helper ambil stok
│   ├── 17_sp_hlp_stok_ambil_edit.sql # SP helper edit stok
│   ├── 18_migrasi_hutang_piutang_detail.sql # Update relasi hutang piutang
│   ├── 19_sp_val_pembelian_harga_beli_vs_jual.sql # Validasi harga beli/jual
│   ├── 20_migrasi_isi_satuan_to_int.sql # Update tipe data isi satuan
│   ├── 21_migrasi_presisi_harga_beli.sql # Presisi desimal harga beli
│   ├── 22_sp_hlp_stok_ambil_edit_retur_beli.sql # SP helper retur stok
│   ├── 23_migrasi_split_bayar_retur_pembelian.sql # Split retur & bayar
│   ├── MigrasiCOA_Baru.sql # Skema Chart of Account Baru
│   ├── STRATEGI_PERTUMBUHAN_DATA.md # Panduan Skalabilitas
│   ├── analisis_hutang_piutang.md # Analisis Modul Hutang/Piutang
│   ├── fix_jurnal_penyesuaian_pb003.sql # Patch Jurnal Penyesuaian
│   ├── migrate_decimal_upgrade.sql # Upgrade Tipe Data Desimal
│   ├── schema database.sql # Full Schema Database Terbaru
│   ├── seed_data_lengkap.sql # Dump Data Awal Lengkap
│   ├── supabase_laporan.sql # Skema Laporan Supabase
│   ├── supabase_setup.sql  # Skema Setup Supabase
│   ├── tbl_datareferensi_backup.sql # Backup Tabel Referensi
│   └── test/               # Script test migrasi
│
├── database_Default_Master/ # 📦 Data Default & Seeders
│   ├── 01_kategori_default.sql # Script Data Kategori Default
│   ├── 02_satuan_default.sql   # Script Data Satuan Default
│   ├── 03_merk_default.sql     # Script Data Merk Default
│   └── README.md               # Dokumentasi Data Default
│
├── Modules/                # 🧠 Library & Engine Utama
│   ├── BilanganTerbilang.vb # Konversi Angka ke Huruf
│   ├── DatabaseModule.vb   # Konektor Utama & Session MySQL
│   ├── DatabaseRestore.vb  # Engine Restore Backup SQL
│   ├── ModulHakAkses.vb    # Otentikasi & Otorisasi Global
│   ├── ModuleAngka.vb      # Formatting & Parsing Desimal/Kurs
│   ├── ModuleAuditTrail.vb # Engine Pencatat Jejak Pengguna
│   ├── ModuleEncrypt.vb    # Kriptografi & Hashing Password
│   ├── ModuleLaporanKalkulasi.vb # Engine Generator Rekapan Rumit
│   ├── ModuleTheme.vb      # Rendering UI, Warna & GDI+
│   ├── ModuleTooltip.vb    # Handler Bantuan & Hover Tips
│   ├── ModuleVariabel.vb   # Variabel Global & Konstanta Sistem
│   ├── ModuleVerifikasiJurnal.vb # Validasi Double-Entry Akuntansi
│   └── MySqlBackup.vb      # Engine Backup Database Otomatis
│
└── MySQL/                  # 🗄️ Driver Database Standalone
    ├── harness-library.dll # Library MySQL
    ├── libeay32.dll        # Library OpenSSL
    ├── libmecab.dll        # Library Mecab MySQL
    ├── logo.png            # Logo MySQL
    ├── mysql.exe           # Eksekusi MySQL Client
    ├── mysqldump.exe       # Eksekusi Dump/Backup MySQL
    └── ssleay32.dll        # Library OpenSSL
```

---

## ⌨️ Keyboard Shortcuts (Kasir Mode)

| Shortcut | Action | Method |
|----------|--------|--------|
| **F1** | Buka Penjualan | Menu Utama |
| **F2** | Buka Form Karyawan | BtnSales_Click() |
| **F4** | Buka Form Barang | BtnBarang_Click() |
| **F6** | Tahan Transaksi | BtnTahan_Click() |
| **F7** | Panggil Transaks | BtnPanggil_Click() |
| **F8** | Proses Pembayaran | BtnBayar_Click() |
| **F10** | Simpan Transaksi | BtnSimpan_Click() |
| **F11** | Batal/Reset | BtnBatal_Click() |
| **F12** | Buka Form Pelanggan | BtnPelanggan_Click() |
| **Tab** | Pindah dari search ke grid | TxtNama_KeyDown() |
| **Down** | Pilih item di ListBox | LstBarang_KeyDown() |
| **Enter** | Confirm & Tambah Item | LstBarang_KeyDown() |

---

## 🔍 Fitur Barcode Hybrid

Sistem mendeteksi input berdasarkan timing:

```text
Mode Scanner (Auto-detect):
- Input cepat: <200ms antar karakter
- Karakter: numeric atau alphanumeric
- Hasil: Barang + Qty default = 1

Mode Input Manual:
- Slow input: >30ms per karakter
- Support: huruf, angka, format qty*barang
- Tampilkan ListBox dropdown
```

**Format Input Support:**
- `8991234567890` - Barcode murni
- `2*Sabun` - Qty + Barang
- `3*2*Minyak` - Qty + Satuan + Barang
- `ABC-123-XYZ` - Barcode alphanumeric
- Scan langsung tanpa prefix

---

## 🧮 Calculation Formulas (Standardized)

**Note:** All formulas use `*` for multiplication, `/` for division, `()` for grouping

### Item-Level Formulas

**Formula 1: Calculate Quantity in Base Unit**
```text
qty_satuan = qty * isi_satuan

Example: qty=2 pcs, isi_satuan=12
👉 qty_satuan = 2 * 12 = 24 pcs
```

**Formula 2: Calculate Cost of Goods Sold (COGS)**
```text
total_harga_beli = harga_beli * isi_satuan * qty

Example: harga_beli=2500, isi=12, qty=2
👉 total_harga_beli = 2500 * 12 * 2 = 60000
```

**Formula 3: Calculate Item Discount Amount**
```text
⚠️ ACTUAL IMPLEMENTATION:
total_diskon = qty * diskon_rp

OR (if percent input):
diskon_rp = harga_jual * diskon_persen / 100
then: total_diskon = qty * diskon_rp
```

**Formula 4: Calculate Item Total**
```text
total_harga = (harga_jual * qty) - diskon_item
```

### Transaction-Level Formulas

**Formula 5: Calculate Subtotal (Before Discount)**
```text
subtotal = SUM(all_items.total_harga)
```

**Formula 6: Calculate Transaction Discount**
```text
diskon_transaksi = subtotal * diskon_persen / 100
OR diskon_transaksi = diskon_rp_input
```

**Formula 7: Calculate Tax**
```text
pajak = (subtotal - diskon_transaksi) * pajak_persen / 100
OR pajak = pajak_rp_input
```

**Formula 8: Calculate Final Total**
```text
total_akhir = subtotal - diskon_transaksi + pajak + biaya_kirim
```

**Formula 9: Calculate Change or Balance**
```text
kembalian = nominal_bayar - total_akhir

If kembalian > 0: Uang kembalian
If kembalian < 0: Sisa hutang
If kembalian = 0: Pas
```

---

## 📄 FormJual - Halaman Penjualan (Detail Lengkap)

#### DataGridView Column Mapping

**Total Columns:** 17

| Index | Column Name | Data Type | Source | ReadOnly | Nullable | Default | Formula/Calc |
|-------|-------------|-----------|--------|----------|----------|---------|-------------|
| 0 | Kode | String | tbl_barang.ID_BARANG | Yes | No | - | - |
| 1 | NamaBarang | String | tbl_barang.NAMA_BARANG | Yes | No | - | - |
| 2 | HargaBeli | Decimal | tbl_barang.HARGA_BELI | Yes | No | 0.00 | - |
| 3 | QTY | Decimal | penjualan_detail.QTY | No | No | 1 | - |
| 4 | Satuan | String | penjualan_detail.SATUAN | No | No | - | - |
| 5 | Isi | Int | penjualan_detail.ISI_SATUAN | Yes | No | 1 | - |
| 6 | TotalHargaBeli | Decimal | CALC | Yes | No | 0.00 | HARGA_BELI * ISI * QTY |
| 7 | Harga | Decimal | penjualan_detail.HARGA_JUAL | Editable* | No | - | - |
| 8 | QtySat | Decimal | CALC | Yes | No | 0 | QTY * ISI |
| 9 | DiskonPersen | Decimal | penjualan_detail.DISKON_PERSEN | No | No | 0 | - |
| 10 | DiskonRp | Decimal | penjualan_detail.DISKON_RP | No | No | 0 | - |
| 11 | TotalDiskon | Decimal | CALC | Yes | No | 0.00 | HARGA_JUAL * DISKON_PERSEN / 100 |
| 12 | TotalHarga | Decimal | penjualan_detail.TOTAL_HARGA | Yes | No | 0.00 | (HARGA_JUAL * QTY) - TOTAL_DISKON |
| 13 | StokToko | Decimal | tbl_barang.STOK_TOKO | Yes | No | 0 | - |
| 14 | StokGudang | Decimal | tbl_barang.STOK_GUDANG | Yes | No | 0 | - |
| 15 | Stok | Decimal | CALC | Yes | No | 0 | STOK_TOKO + STOK_GUDANG (if TOKO loc) |
| 16 | SerialNumber | String | penjualan_detail.SERIAL_NUMBER | No | Yes | NULL | - |

#### Alur Lengkap Transaksi Penjualan

**STEP 1: Load Form**
Kondisiawal() -> Generate faktur -> Load pelanggan/karyawan -> Cek hak akses.

**STEP 2: Pilih Pelanggan & Sales**
Query pelanggan -> Cek Jenis (Umum/Partai) -> Hitung Jatuh Tempo -> UpdateHargaBerdasarJenisPelanggan().

**STEP 3: Tambah Barang (4 Metode)**
1. **Pencarian Manual**: Ketik nama -> ListBox Dropdown -> Pilih.
2. **Format Cepat**: Input `Qty*Barang` (contoh: `2*Sabun`).
3. **Barcode Scanner**: Input <200ms -> Auto-tambah Qty 1.
4. **Edit Langsung Grid**: Ketik barcode/nama di sel grid.

**STEP 4: Tambah ke DataGridView**
Cek duplikasi -> Jika sama, merge dan update Qty -> Hitung nilai -> UpdateSemuaTotal() -> Fokus kembali ke pencarian.

**STEP 5: Edit Item di Grid**
Edit Qty/Harga/Diskon -> `DgvData_CellEndEdit` -> Kalkulasi ulang baris tersebut -> UpdateSemuaTotal().

**STEP 6: Kalkulasi Total Transaksi**
SUM(TotalHarga) -> Kurangi Diskon Transaksi -> Tambah Pajak & Ongkir -> `TxtGrantotal`.

**STEP 7: Pembayaran (F8)**
Pilih metode (Tunai/Transfer) -> Input nominal -> Hitung kembalian/hutang -> Set status "LUNAS" atau "BELUM LUNAS".

**STEP 8: Simpan (F10)**
1. Start `MySqlTransaction`.
2. Jika Mode Edit: Panggil `ModuleHapusTransaksi.HapusPenjualan(transaction)`.
3. Simpan `penjualan` dan `penjualan_detail`.
4. Simpan `tbl_piutang` (jika kredit).
5. Simpan `JurnalUmum`.
6. Simpan `HistoryBarang`.
7. Hitung ulang stok via `HitungStokPerubahan`.
8. Commit transaksi & Cetak Nota.

---

## 🗄️ Database Schema (Kritis & Akurat)

#### Tabel: `tbl_barang` - Master Inventori
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `ID_BARANG` | VARCHAR(50) | PRIMARY KEY |
| `ID_BARANG_BANTU` | VARCHAR(50) | - |
| `NAMA_BARANG` | VARCHAR(200) | - |
| `NAMA_BARANG_BANTU` | VARCHAR(100) | - |
| `JENIS` | VARCHAR(20) | - |
| `KODE_KATEGORI` | VARCHAR(30) | - |
| `NAMA_KATEGORI` | VARCHAR(50) | - |
| `KODE_SUPLIYER` | VARCHAR(50) | - |
| `NAMA_SUPLIYER` | VARCHAR(100) | - |
| `KODE_MERK` | VARCHAR(10) | - |
| `NAMA_MERK` | VARCHAR(20) | - |
| `SERIAL_NUMBER` | TINYINT(1) | - |
| `JENIS_SATUAN` | VARCHAR(50) | - |
| `HARGA_BELI` | DECIMAL(15,4) | - |
| `HARGA_BELI_TERAKHIR` | DECIMAL(15,4) | - |
| `HPP_UMUM_KECIL` | DECIMAL(15,2) | - |
| `HPP_UMUM_SEDANG` | DECIMAL(15,2) | - |
| `HPP_UMUM_BESAR` | DECIMAL(15,2) | - |
| `HARGA_BELI_UMUM_KECIL` | DECIMAL(15,2) | - |
| `HARGA_BELI_UMUM_SEDANG` | DECIMAL(15,2) | - |
| `HARGA_BELI_UMUM_BESAR` | DECIMAL(15,2) | - |
| `HPP_PARTAI_KECIL` | DECIMAL(15,2) | - |
| `HPP_PARTAI_SEDANG` | DECIMAL(15,2) | - |
| `HPP_PARTAI_BESAR` | DECIMAL(15,2) | - |
| `HARGA_BELI_PARTAI_KECIL` | DECIMAL(15,2) | - |
| `HARGA_BELI_UPARTAI_SEDANG` | DECIMAL(15,2) | - |
| `HARGA_BELI_PARTAI_BESAR` | DECIMAL(15,2) | - |
| `BARCODE_KECIL` | VARCHAR(20) | - |
| `BARCODE_SEDANG` | VARCHAR(20) | - |
| `BARCODE_BESAR` | VARCHAR(20) | - |
| `SATUAN_UMUM_KECIL` | VARCHAR(20) | - |
| `SATUAN_UMUM_SEDANG` | VARCHAR(20) | - |
| `SATUAN_UMUM_BESAR` | VARCHAR(20) | - |
| `ISI_UMUM_KECIL` | INT(11) | - |
| `ISI_UMUM_SEDANG` | INT(11) | - |
| `ISI_UMUM_BESAR` | INT(11) | - |
| `HARGA_JUAL_UMUM_KECIL` | DECIMAL(15,2) | - |
| `HARGA_JUAL_UMUM_SEDANG` | DECIMAL(15,2) | - |
| `HARGA_JUAL_UMUM_BESAR` | DECIMAL(15,2) | - |
| `SATUAN_PARTAI_KECIL` | VARCHAR(20) | - |
| `SATUAN_PARTAI_SEDANG` | VARCHAR(20) | - |
| `SATUAN_PARTAI_BESAR` | VARCHAR(20) | - |
| `ISI_PARTAI_KECIL` | INT(11) | - |
| `ISI_PARTAI_SEDANG` | INT(11) | - |
| `ISI_PARTAI_BESAR` | INT(11) | - |
| `HARGA_JUAL_PARTAI_KECIL` | DECIMAL(15,2) | - |
| `HARGA_JUAL_PARTAI_SEDANG` | DECIMAL(15,2) | - |
| `HARGA_JUAL_PARTAI_BESAR` | DECIMAL(15,2) | - |
| `AWAL_TOKO` | DECIMAL(15,4) | - |
| `STOK_AWAL_TOKO` | DECIMAL(15,4) | - |
| `TAMBAH_TOKO` | DECIMAL(15,4) | - |
| `KURANG_TOKO` | DECIMAL(15,4) | - |
| `PEMBELIAN_TOKO` | DECIMAL(15,4) | - |
| `PENJUALAN_TOKO` | DECIMAL(15,4) | - |
| `RETUR_BELI_TOKO` | DECIMAL(15,4) | - |
| `RETUR_JUAL_TOKO` | DECIMAL(15,4) | - |
| `OPNAME_TOKO` | DECIMAL(15,4) | - |
| `TRANSFER_STOK_MASUK_TOKO` | DECIMAL(15,4) | - |
| `TRANSFER_STOK_KELUAR_TOKO` | DECIMAL(15,4) | - |
| `TRANSFER_BARANG_MASUK_TOKO` | DECIMAL(15,4) | - |
| `TRANSFER_BARANG_KELUAR_TOKO` | DECIMAL(15,4) | - |
| `TRANSFER_CABANG_MASUK_TOKO` | DECIMAL(15,4) | - |
| `TRANSFER_CABANG_KELUAR_TOKO` | DECIMAL(15,4) | - |
| `STOK_TOKO` | DECIMAL(15,4) | - |
| `AWAL_GUDANG` | DECIMAL(15,4) | - |
| `STOK_AWAL_GUDANG` | DECIMAL(15,4) | - |
| `TAMBAH_GUDANG` | DECIMAL(15,4) | - |
| `KURANG_GUDANG` | DECIMAL(15,4) | - |
| `PEMBELIAN_GUDANG` | DECIMAL(15,4) | - |
| `PENJUALAN_GUDANG` | DECIMAL(15,4) | - |
| `RETUR_BELI_GUDANG` | DECIMAL(15,4) | - |
| `RETUR_JUAL_GUDANG` | DECIMAL(15,4) | - |
| `OPNAME_GUDANG` | DECIMAL(15,4) | - |
| `TRANSFER_STOK_MASUK_GUDANG` | DECIMAL(15,4) | - |
| `TRANSFER_STOK_KELUAR_GUDANG` | DECIMAL(15,4) | - |
| `TRANSFER_BARANG_MASUK_GUDANG` | DECIMAL(15,4) | - |
| `TRANSFER_BARANG_KELUAR_GUDANG` | DECIMAL(15,4) | - |
| `TRANSFER_CABANG_MASUK_GUDANG` | DECIMAL(15,4) | - |
| `TRANSFER_CABANG_KELUAR_GUDANG` | DECIMAL(15,4) | - |
| `STOK_GUDANG` | DECIMAL(15,4) | - |
| `SATUAN_STOK` | VARCHAR(20) | - |
| `SATUAN_ISI_STOK` | INT(11) | - |
| `STOK_MIN` | DECIMAL(15,4) | - |
| `STOK_MAX` | DECIMAL(15,4) | - |
| `LOKASI_RAK_TOKO` | VARCHAR(50) | - |
| `LOKASI_RAK_GUDANG` | VARCHAR(50) | - |
| `POINT_MEMBER` | DECIMAL(10,2) | - |
| `POINT_KARYAWAN` | DECIMAL(10,2) | - |
| `KOMISI_SALES_RP` | DECIMAL(15,2) | - |
| `KOMISI_SALES_PERSEN` | DECIMAL(10,2) | - |
| `STATUS` | VARCHAR(10) | - |
| `created_at` | DATETIME | - |
| `updated_at` | DATETIME | - |
| `sync_id` | VARCHAR(36) | - |
| `id_cloud` | VARCHAR(50) | - |
| `updated_by` | VARCHAR(50) | - |
| `is_dirty` | TINYINT(4) | - |
| `version` | INT(11) | - |

#### Tabel: `penjualan` - Header Transaksi
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `ID_PENJUALAN` | VARCHAR(30) | PRIMARY KEY |
| `ID_PELANGGAN` | VARCHAR(10) | - |
| `NAMA_PELANGGAN` | VARCHAR(100) | - |
| `ALAMAT_PELANGGAN` | VARCHAR(200) | - |
| `JENIS_PELANGGAN` | VARCHAR(30) | - |
| `LOKASIBARANG` | VARCHAR(20) | - |
| `TGL_TRANSAKSI` | DATETIME | - |
| `TOTAL_HPP` | DECIMAL(15,2) | - |
| `GRAND_TOTAL_SBL_PAJAK` | DECIMAL(15,2) | - |
| `DISKON_TOTAL_PERSEN` | DECIMAL(10,2) | - |
| `DISKON_TOTAL_RP` | DECIMAL(15,2) | - |
| `PAJAK_PERSEN` | DECIMAL(10,2) | - |
| `PAJAK_RP` | DECIMAL(15,2) | - |
| `GRAND_TOTAL_STL_PAJAK` | DECIMAL(15,2) | - |
| `LABA` | DECIMAL(15,2) | - |
| `BAYAR` | DECIMAL(15,2) | - |
| `NOMINAL_TRANSFER` | DECIMAL(15,2) | - |
| `BIAYA_KIRIM` | DECIMAL(15,2) | - |
| `KEMBALI` | DECIMAL(15,2) | - |
| `TGL_RETUR` | DATETIME | - |
| `NILAI_RETUR` | DECIMAL(15,2) | - |
| `TGL_PEMBAYARAN` | DATETIME | - |
| `NOMINALBAYARPIUTANG` | DECIMAL(15,2) | - |
| `SISA_TAGIHAN` | DECIMAL(15,2) | - |
| `JATUH_TEMPO` | DATETIME | - |
| `STATUS_BAYAR` | VARCHAR(20) | - |
| `STATUS_TRANSAKSI` | VARCHAR(20) | - |
| `TYPE_AKUN` | VARCHAR(20) | - |
| `KODE_AKUN` | VARCHAR(50) | - |
| `JENIS_PEMBAYARAN` | VARCHAR(50) | - |
| `KODE_AKUN_TF` | VARCHAR(20) | - |
| `NAMA_AKUN_TF` | VARCHAR(50) | - |
| `TYPE_AKUNBANK` | VARCHAR(20) | - |
| `KODE_AKUNBANK` | VARCHAR(50) | - |
| `JENIS_PEMBAYARANBANK` | VARCHAR(50) | - |
| `METODE` | VARCHAR(20) | - |
| `BANK` | VARCHAR(50) | - |
| `NO_REKENING` | VARCHAR(30) | - |
| `NAMA_REKENING` | VARCHAR(50) | - |
| `NO_REFFERENSI` | VARCHAR(100) | - |
| `ID_SALES` | VARCHAR(20) | - |
| `NAMA_SALES` | VARCHAR(100) | - |
| `ID_USER` | VARCHAR(20) | - |
| `ID_KOMPUTER` | VARCHAR(20) | - |
| `created_at` | DATETIME | - |
| `updated_at` | DATETIME | - |
| `sync_id` | VARCHAR(36) | - |

#### Tabel: `penjualan_detail` - Detail Item
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `FAKTUR_JUAL` | VARCHAR(15) | - |
| `ID_PELANGGAN` | VARCHAR(30) | - |
| `NAMA_PELANGGAN` | VARCHAR(100) | - |
| `JENIS_PELANGGAN` | VARCHAR(10) | - |
| `LOKASIBARANG` | VARCHAR(20) | - |
| `TANGGAL_JUAL` | DATETIME | - |
| `ID_BARANG` | VARCHAR(15) | - |
| `NAMA_BARANG` | VARCHAR(100) | - |
| `SERIAL_NUMBER` | VARCHAR(50) | - |
| `HARGA_BELI` | DECIMAL(15,4) | - |
| `QTY` | DECIMAL(15,4) | - |
| `SATUAN` | VARCHAR(10) | - |
| `ISI_SATUAN` | INT(11) | - |
| `HARGA_BELI_SATUAN` | DECIMAL(15,4) | - |
| `HARGA_JUAL` | DECIMAL(15,0) | - |
| `QTY_SATUAN` | DECIMAL(15,4) | - |
| `DISKON_PERSEN` | DECIMAL(10,2) | - |
| `DISKON_RP` | DECIMAL(15,2) | - |
| `TOTAL_DISKON` | DECIMAL(15,2) | - |
| `TOTAL_HARGA` | DECIMAL(15,0) | - |
| `LABA` | DECIMAL(15,2) | - |
| `ID_USER` | VARCHAR(20) | - |
| `ID_KOMPUTER` | VARCHAR(20) | - |
| `created_at` | DATETIME | - |
| `updated_at` | DATETIME | - |
| `sync_id` | VARCHAR(36) | - |

#### Tabel: `tbl_datareferensi` - Buku Besar Akun
| Kolom | Tipe | Deskripsi |
|-------|------|-----------|
| `STATUS` | VARCHAR(10) | - |
| `JENIS_AKUN` | VARCHAR(50) | - |
| `TYPE_AKUN` | VARCHAR(30) | - |
| `KODE_AKUN` | VARCHAR(20) | PRIMARY KEY |
| `NAMA_AKUN` | VARCHAR(100) | - |
| `SUB_AKUN` | VARCHAR(20) | - |
| `AKUN_DK` | VARCHAR(20) | - |
| `AKUN_NRLR` | VARCHAR(20) | - |
| `KETERANGAN` | TEXT | - |
| `SALDO_AWAL` | DECIMAL(20,0) | - |
| `SALDO_SEBELUMNYA` | DECIMAL(20,0) | - |
| `S_DEBET` | DECIMAL(20,0) | - |
| `S_KREDIT` | DECIMAL(20,0) | - |
| `SALDO_AKHIR` | DECIMAL(20,0) | - |
| `created_at` | DATETIME | - |
| `updated_at` | DATETIME | - |
| `sync_id` | VARCHAR(36) | - |

#### Tabel: `hakaksesuser` - Hak Akses Pengguna
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `NO` | INT(11) | PRIMARY KEY |
| `UserName` | VARCHAR(30) | - |
| `Role` | VARCHAR(100) | - |
| `ModuleName` | VARCHAR(100) | - |
| `CanRead` | TINYINT(1) | - |
| `CanAdd` | TINYINT(1) | - |
| `CanEdit` | TINYINT(1) | - |
| `CanDelete` | TINYINT(1) | - |
| `created_at` | DATETIME | - |
| `updated_at` | DATETIME | - |
| `sync_id` | VARCHAR(36) | - |

---

## 🛠️ Developer Guidelines

### 1. Prosedur Edit/Koreksi
Jangan pernah mengedit record secara parsial. Alur yang benar adalah:
1. Panggil fungsi di `ModuleHapusTransaksi` untuk melakukan **Full Reversal** (Hapus Jurnal, Pembalikan Stok, Pembalikan Piutang).
2. Biarkan user melakukan input ulang di UI.
3. Simpan sebagai record baru.

### 2. Sinkronisasi Akuntansi
Setiap kali melakukan modifikasi pada `JurnalUmum`, pastikan memanggil `UpdateSaldoAkun(kode_akun)` agar saldo di Dashboard tetap akurat tanpa perlu perhitungan ulang massal.

---

## 📜 Lisensi & Kontribusi
Aplikasi ini dikembangkan secara internal untuk **App Kasir_2026**.  
**Copyright © 2026 adysuryadi64**. Seluruh hak cipta dilindungi undang-undang.
