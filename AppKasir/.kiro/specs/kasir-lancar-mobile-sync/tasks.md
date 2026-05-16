# Tasks — Kasir Lancar Mobile: UI + Sinkronisasi Backend (v2)

## ── FASE 1: PHP BACKEND ✅ SELESAI ────────────────────────────

- [x] 1. Buat `get_ai_analytics.php` — 6 type: produk_terlaris, barang_lambat, reorder_alert, jam_puncak, margin_profit, pelanggan_aktif
- [x] 2. Buat `get_perusahaan.php` — data perusahaan + semua akun COA default
- [x] 3. Buat `get_hak_akses.php` — 8 setting dari hakaksesuser, default permissive
- [x] 4. Buat `get_dashboard_summary.php` — total penjualan + transaksi + opname hari ini
- [x] 5. Buat `get_karyawan.php` — daftar karyawan aktif untuk dropdown sales
- [x] 6. Buat `get_akun_coa.php` — akun COA dengan filter tipe KAS/BANK
- [x] 7. Buat `get_laporan_stok.php` — stok barang dengan search, filter, pagination
- [x] 8. Validasi `get_opname_list.php` — sudah benar, JUMLAH_ITEM dan TOTAL_SELISIH ada

## ── FASE 2: MODEL & PROVIDER FLUTTER ✅ SELESAI ──────────────

- [x] 9. Buat `PerusahaanModel` (`lib/models/perusahaan_model.dart`) + `AkunCOA`
- [x] 10. Buat `HakAksesModel` (`lib/models/hak_akses_model.dart`) + default permissive
- [x] 11. Update `StorageService` — tambah key perusahaan & hak akses
- [x] 12. Update `AuthProvider` — load perusahaan + hak akses saat login, cache ke SharedPreferences
- [x] 13. Update `ApiService` — tambah 7 endpoint baru (perusahaan, hak_akses, dashboard_summary, karyawan, akun_coa, laporan_stok, ai_analytics)
- [x] 14. `flutter analyze` — **No issues found** ✅

## ── FASE 3: UI SCREENS ────────────────────────────────────────

- [x] 15. Login Screen — layout sudah benar, tidak perlu redesign besar
  - [x] 15.1 Logo + nama app compact di atas ✅ (sudah ada)
  - [x] 15.2 Dropdown user + input password di tengah ✅ (sudah ada)
  - [x] 15.3 Tombol MASUK langsung di bawah input ✅ (sudah ada)
  - [x] 15.4 Konfigurasi Server di bawah ✅ (sudah ada)

- [x] 16. Buat `AppDrawer` widget
  - [x] 16.1 Buat `AppAndroid/lib/widgets/app_drawer.dart`
  - [x] 16.2 Header: avatar inisial, nama user, nama perusahaan, badge lokasi
  - [x] 16.3 Menu: Dashboard, Penjualan, Stok Opname, Laporan Stok, Ganti Lokasi
  - [x] 16.4 Footer: Logout dengan konfirmasi dialog
  - [x] 16.5 Navigasi menggunakan `Navigator.pushReplacement`

- [x] 17. Dashboard Screen — redesign dengan sidebar + data real + AI Analytics
  - [x] 17.1 Tambah `Drawer` menggunakan `AppDrawer`
  - [x] 17.2 Header: nama perusahaan, lokasi, nama user, tombol refresh
  - [x] 17.3 Summary cards: total penjualan, jumlah transaksi, jumlah opname — data real
  - [x] 17.4 Loading skeleton saat fetch, "—" jika gagal
  - [x] 17.5 Section AI Analytics: 6 kartu grid 2 kolom
  - [x] 17.6 Menu utama: Penjualan, Stok Opname
  - [x] 17.7 Pull-to-refresh untuk semua data

- [x] 18. Buat `AIAnalyticsCard` widget dan 6 modal detail
  - [x] 18.1 Buat `AppAndroid/lib/widgets/ai_analytics_card.dart`
  - [x] 18.2 Load 6 kartu paralel di `initState` dengan `Future.wait`
  - [x] 18.3 Auto-refresh setiap 5 menit (`Timer.periodic`)
  - [x] 18.4 Modal Produk Terlaris — tabel top 10 + trend
  - [x] 18.5 Modal Barang Lambat — list + nilai tertahan + tip
  - [x] 18.6 Modal Reorder Alert — list + estimasi hari habis + saran order
  - [x] 18.7 Modal Jam Puncak — bar chart per jam + rekomendasi
  - [x] 18.8 Modal Margin Profit — top 5 tertinggi + 5 terendah
  - [x] 18.9 Modal Pelanggan Aktif — top 10 + badge VIP/Reguler/Baru + stat card
  - [x] 18.10 Semua modal: `showModalBottomSheet` dengan `DraggableScrollableSheet`
  - [x] 18.11 `flutter analyze` — No issues found ✅

- [x] 19. Buat `PenjualanProvider`
  - [x] 19.1 Buat `AppAndroid/lib/providers/penjualan_provider.dart`
  - [x] 19.2 State lengkap: tanggal, pelanggan, sales, cartItems, diskon, pajak, biayaKirim
  - [x] 19.3 State pembayaran: tunai, transfer, akun, info bank
  - [x] 19.4 Computed: subtotal, grandTotal, sisaTagihan, kembali, totalHPP, laba
  - [x] 19.5 Method: addItem (handle duplikat), removeItem, updateQty, updateHarga, updateDiskonItem
  - [x] 19.6 Method: setDiskonPersen/Rp, setPajakPersen/Rp, setBiayaKirim (sinkron persen↔Rp)
  - [x] 19.7 Method: reset(), buildPayload()
  - [x] 19.8 Daftarkan di `main.dart` sebagai `ChangeNotifierProvider`

- [x] 20. Update `main.dart` — ganti nama app + tambah PenjualanProvider
  - [x] 20.1 Ganti `AppKasirMobile` → `KasirLancarApp`
  - [x] 20.2 Ganti title `'AppKasir Mobile'` → `'Kasir Lancar'`
  - [x] 20.3 Tambah `PenjualanProvider` ke MultiProvider

- [x] 23. Buat `LaporanStokScreen`
  - [x] 23.1 Buat `AppAndroid/lib/screens/laporan_stok_screen.dart`
  - [x] 23.2 Search field dengan debounce 400ms
  - [x] 23.3 Filter dropdown kategori (diambil dari data)
  - [x] 23.4 List barang: nama, kode, stok toko, stok gudang, satuan, badge kategori/merk
  - [x] 23.5 Pull-to-refresh
  - [x] 23.6 Load more (pagination scroll)
  - [x] 23.7 Terintegrasi di `AppDrawer`
  - [x] 23.8 `flutter analyze` — No issues found ✅

- [x] 20. Buat `PenjualanFlow` — multi-screen dengan PageView
  - [x] 20.1 Buat `AppAndroid/lib/screens/penjualan/penjualan_flow.dart` — PageView controller
  - [x] 20.2 Buat `step1_header_screen.dart` — tanggal, pelanggan, sales
    - [x] 20.2a DatePicker tanggal (default hari ini, bisa ubah jika `izinkanTanggalLampau`)
    - [x] 20.2b Dropdown pelanggan dengan search
    - [x] 20.2c Dropdown sales (opsional, dari `getKaryawan()`)
    - [x] 20.2d Tombol "Lanjut →"
  - [x] 20.3 Buat `step2_items_screen.dart` — tambah item + list
    - [x] 20.3a Search barang inline (debounce 400ms)
    - [x] 20.3b List item: nama, satuan, qty (editable), harga (editable jika `izinkanUbahHarga`), diskon per item, total
    - [x] 20.3c Tampilkan stok jika `tampilInfoStok = true`
    - [x] 20.3d Validasi duplikat satuan jika `izinkanSatuanBerbeda = false`
    - [x] 20.3e Tombol "Lanjut ke Pembayaran →"
  - [x] 20.4 Buat `step3_payment_screen.dart` — pembayaran
    - [x] 20.4a Subtotal, diskon global (persen+Rp sinkron), pajak (persen+Rp sinkron), biaya kirim
    - [x] 20.4b Grand total realtime
    - [x] 20.4c Input tunai + dropdown akun KAS (default dari `perusahaan.akunKasUntukLokasi()`)
    - [x] 20.4d Input transfer + dropdown akun BANK (default dari `perusahaan.akunTransfer`)
    - [x] 20.4e Section info bank (tampil hanya jika transfer > 0): bank, no rek, nama rek, no ref
    - [x] 20.4f Kembalian / sisa hutang otomatis
    - [x] 20.4g Status: LUNAS / Belum Lunas
    - [x] 20.4h Tombol "Preview Nota →"
  - [x] 20.5 Buat `step4_preview_screen.dart` — preview nota
    - [x] 20.5a Render `NotaWidget` — identik dengan cetakan thermal
    - [x] 20.5b Tombol [Cetak] [Kirim WA] [Simpan Saja]
    - [x] 20.5c Semua tombol: simpan dulu → jika berhasil → aksi cetak/kirim
    - [x] 20.5d Jika simpan gagal, tampilkan error — jangan cetak
  - [x] 20.6 Back antar screen tanpa kehilangan data (state di `PenjualanProvider`)
  - [x] 20.7 Validasi hak akses sebelum simpan:
    - [x] 20.7a Cek stok jika `izinkanJualStokMinus = false`
    - [x] 20.7b Cek rugi jika `izinkanJualRugi = false`
  - [x] 20.8 Update payload `syncPenjualan` — kirim semua field baru
  - [x] 20.9 Jalankan `flutter analyze`

- [x] 21. Stok Opname Screen — redesign
  - [x] 21.1 Layout item: stok sistem dan input stok nyata dengan font besar (min 28sp)
  - [x] 21.2 Tampilkan satuan di bawah angka (sama seperti desktop)
  - [x] 21.3 Tambah field keterangan per item
  - [x] 21.4 Selisih dihitung otomatis — merah jika minus, hijau jika plus/nol
  - [x] 21.5 Tombol simpan sticky di bawah layar
  - [x] 21.6 Update payload `syncStokOpname` — kirim field keterangan
  - [x] 21.7 Jalankan `flutter analyze`

- [x] 22. Opname List Screen — redesign card
  - [x] 22.1 Card lebih informatif: tanggal, lokasi, jumlah item, total selisih
  - [x] 22.2 Warna badge selisih: merah jika negatif, hijau jika positif/nol
  - [x] 22.3 Jalankan `flutter analyze`

- [x] 23. Buat `LaporanStokScreen`
  - [x] 23.1 Buat `AppAndroid/lib/screens/laporan_stok_screen.dart`
  - [x] 23.2 AppBar dengan search field
  - [x] 23.3 Filter dropdown kategori
  - [x] 23.4 List barang: nama, kode, stok toko, stok gudang, satuan
  - [x] 23.5 Pull-to-refresh
  - [x] 23.6 Load more (pagination)
  - [x] 23.7 Tambahkan ke `AppDrawer` dan route
  - [x] 23.8 Jalankan `flutter analyze`

## ── FASE 4: CETAK THERMAL ─────────────────────────────────────

- [x] 24. Setup package cetak thermal
  - [x] 24.1 Tambah `bluetooth_print: ^4.4.0` dan `esc_pos_utils: ^0.4.1` ke `pubspec.yaml`
  - [x] 24.2 Tambah permission Bluetooth di `AndroidManifest.xml`
  - [x] 24.3 Jalankan `flutter pub get`

- [x] 25. Buat `ThermalPrintService`
  - [x] 25.1 Buat `AppAndroid/lib/services/thermal_print_service.dart`
  - [x] 25.2 Method `scanDevices()` — scan Bluetooth devices
  - [x] 25.3 Method `buildNotaBytes(notaData, config)` — bangun bytes ESC/POS
  - [x] 25.4 Layout kolom persentase (sama dengan VB): Qty=11%, Harga=51%/65%, Disc=70%, Jml=95%
  - [x] 25.5 8 model nota (kombinasi header kolom, diskon, sisa hutang)
  - [x] 25.6 Method `printNota(device, bytes)` — kirim ke printer

- [x] 26. Buat `PrinterConfig` model dan `PrinterSettingsScreen`
  - [x] 26.1 Buat `AppAndroid/lib/models/printer_config.dart`
  - [x] 26.2 Buat `AppAndroid/lib/screens/printer_settings_screen.dart`
  - [x] 26.3 Scan dan pilih device Bluetooth
  - [x] 26.4 Pilih lebar kertas (58mm / 80mm) dan model nota (1-8)
  - [x] 26.5 Simpan ke SharedPreferences, tambahkan ke `AppDrawer`

- [x] 27. Buat `NotaWidget`
  - [x] 27.1 Buat `AppAndroid/lib/widgets/nota_widget.dart`
  - [x] 27.2 Render nota sebagai widget Flutter (Column + Text font monospace)
  - [x] 27.3 Layout identik dengan cetakan thermal (persentase kolom)
  - [x] 27.4 Header toko, info transaksi, item, total, info bank, footer

- [x] 28. Integrasi cetak di Step 4
  - [x] 28.1 Tombol [Cetak]: simpan → berhasil → `ThermalPrintService.printNota()`
  - [x] 28.2 Tombol [Kirim WA]: simpan → berhasil → share teks nota
  - [x] 28.3 Tombol [Simpan Saja]: simpan → berhasil → kembali ke dashboard
  - [x] 28.4 Jika printer belum dikonfigurasi, arahkan ke `PrinterSettingsScreen`
  - [x] 28.5 Jalankan `flutter analyze`
  - [x] 28.5 Jalankan `flutter analyze`

## ── FASE 5: VERIFIKASI AKHIR ──────────────────────────────────

- [x] 29. Konsistensi nama aplikasi
  - [x] 29.1 Cari semua teks "AppKasir Mobile", "Kasir Lancar" dan "AppKasir" di semua file `.dart`
  - [x] 29.2 Ganti dengan "Kasir Lancar Mobile" ganti juga icon dengan logomobile.png dan update versi 
  - [x] 29.3 Jalankan `flutter analyze`

- [ ] 30. Verifikasi end-to-end
  - [ ] 30.1 Jalankan `flutter analyze` — harus `No issues found`
  - [ ] 30.2 Copy semua file PHP baru ke folder AppServ (`www/api/`)
  - [ ] 30.3 Test login: dropdown user tampil tanpa scroll, login berhasil, hak akses + data perusahaan tersimpan
  - [ ] 30.4 Test dashboard: summary cards menampilkan data real sesuai lokasi
  - [ ] 30.5 Test AI Analytics: 6 kartu tampil dengan data, klik → modal detail terbuka
  - [ ] 30.6 Test AI Reorder Alert: barang kritis tampil dengan estimasi hari habis
  - [ ] 30.7 Test AI Jam Puncak: bar chart jam tampil, rekomendasi muncul
  - [ ] 30.8 Test penjualan: alur 4 screen, back tanpa kehilangan data, simpan + cetak
  - [ ] 30.9 Test penjualan dengan diskon item + diskon global: cek nilai di desktop VB
  - [ ] 30.10 Test penjualan dengan transfer: cek jurnal akuntansi di desktop
  - [ ] 30.11 Test hak akses: login user terbatas → pastikan blokir berjalan
  - [ ] 30.12 Test opname: input stok nyata + keterangan, cek selisih, simpan
  - [ ] 30.13 Test laporan stok: search, filter, load more
  - [ ] 30.14 Test cetak thermal: scan printer, cetak nota, cek layout kolom

## ── FASE 6: RIWAYAT PENJUALAN ─────────────────────────────────

- [x] 31. Buat `get_riwayat_penjualan.php` — endpoint list + detail
  - [x] 31.1 Query list: `ID_PENJUALAN, TGL_TRANSAKSI, NAMA_PELANGGAN, LOKASIBARANG, JENIS_PEMBAYARAN, GRAND_TOTAL_STL_PAJAK, BAYAR, NOMINAL_TRANSFER, KEMBALI, NILAI_RETUR, SISA_TAGIHAN, STATUS_TRANSAKSI, ID_USER` — identik dengan query VB `Datapenjualan()`
  - [x] 31.2 Filter: lokasi, tgl_dari, tgl_sampai, search (faktur/pelanggan), limit, offset
  - [x] 31.3 Query detail header: semua kolom dari tabel `penjualan` WHERE `ID_PENJUALAN = ?`
  - [x] 31.4 Query detail items: `ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA` FROM `penjualan_detail` WHERE `FAKTUR_JUAL = ?` — identik dengan query VB detail penjualan
  - [x] 31.5 Hanya SELECT — tidak ada UPDATE/DELETE

- [x] 32. Update `ApiService` — tambah endpoint riwayat penjualan
  - [x] 32.1 Method `getRiwayatPenjualan({lokasi, tglDari, tglSampai, search, limit, offset})`
  - [x] 32.2 Method `getDetailPenjualan(idPenjualan)`

- [x] 33. Buat `RiwayatPenjualanScreen`
  - [x] 33.1 Buat `AppAndroid/lib/screens/riwayat_penjualan_screen.dart`
  - [x] 33.2 Filter tanggal (default hari ini) + shortcut 7H/30H/Bulan
  - [x] 33.3 Search field: nomor faktur atau nama pelanggan (debounce 400ms)
  - [x] 33.4 Card: nomor faktur, tanggal, pelanggan, total, badge status, badge metode
  - [x] 33.5 Pull-to-refresh + load more (pagination)
  - [x] 33.6 Klik card → buka `DetailPenjualanScreen`
  - [x] 33.7 Tambahkan ke `AppDrawer` menu Penjualan
  - [x] 33.8 `flutter analyze` — No issues found

- [x] 34. Buat `DetailPenjualanScreen`
  - [x] 34.1 Buat `AppAndroid/lib/screens/detail_penjualan_screen.dart`
  - [x] 34.2 Render `_NotaPreview` dari data yang diambil API (bukan dari provider)
  - [x] 34.3 Tombol [Cetak] → `ThermalPrintService.printNota()`
  - [x] 34.4 Tombol [Kirim WA] → share teks nota
  - [x] 34.5 Tidak ada tombol Hapus / Edit
  - [x] 34.6 `flutter analyze` — No issues found
