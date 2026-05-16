# Kasir Lancar Mobile

Aplikasi mobile Point of Sale pendamping **Kasir Lancar** desktop (VB.NET).  
Dikembangkan dengan Flutter, terhubung ke database MySQL yang sama melalui PHP API di server lokal (LAN/WiFi).

---

## Prinsip Utama

| Prinsip | Penjelasan |
|---|---|
| **Sama persis dengan desktop** | Logika bisnis, jurnal akuntansi, update stok, dan format nomor transaksi identik dengan `FormPenjualan.vb` dan `FormStokOpname.vb` |
| **Lokasi menentukan stok** | Login sebagai TOKO → hanya `STOK_TOKO` yang berubah. Login sebagai GUDANG → hanya `STOK_GUDANG` |
| **Nomor transaksi di-generate server** | Bukan di Flutter — mencegah duplikat saat multi-user |
| **Data mobile bisa diedit di desktop** | Semua kolom header penjualan diisi lengkap agar kompatibel |
| **Token sesi per login** | Setiap login menghasilkan token unik 48 karakter yang disimpan di `login_session_key` |

---

## Alur Penggunaan

```
1. Buka browser PC → http://[IP-server]/api/ → konfigurasi database
2. Buka app Android → masukkan IP server → test koneksi → simpan
3. Login dengan akun yang sama seperti desktop
4. Pilih lokasi: TOKO atau GUDANG
5. Dashboard → Penjualan atau Stok Opname
```

---

## Fitur

### Penjualan
- Cari barang by nama, barcode, kategori, merk (JOIN ke tabel master)
- Debounce 400ms — tidak spam request ke server
- Pilih satuan Kecil/Sedang/Besar dengan harga otomatis
- Harga Umum atau Partai sesuai jenis pelanggan
- Pembayaran tunai, transfer, atau kombinasi (split payment)
- Piutang otomatis jika bayar < total — wajib pilih pelanggan
- Validasi sebelum simpan: stok mencukupi, harga > 0, qty > 0, pelanggan untuk piutang
- Jurnal akuntansi 10 entri otomatis (sama persis VB)
- Nomor faktur `PJ-YYMMDD-XXXX` di-generate server dalam transaksi

### Stok Opname
- List history opname per nomor SO dengan pull-to-refresh
- FAB `+` untuk membuat opname baru
- Cari barang inline (autocomplete langsung di bawah search field)
- Scan barcode kamera
- Stok sistem ditampilkan sesuai lokasi login
- Input stok nyata: tombol +/- atau ketik langsung
- Selisih dihitung otomatis, warna merah/hijau
- Dropdown kategori dan merk langsung di card item (update realtime ke server)
- Jurnal penyesuaian stok otomatis
- Simpan cepat (fast mode): response langsung, proses berat di background
- Nomor SO `SO-YYMMDD-XXXX` di-generate server dalam transaksi

### Autentikasi & Keamanan
- Token sesi real dari server (bukan dummy) — disimpan di `SharedPreferences`
- Validasi token ke server saat app dibuka (timeout 5 detik, graceful offline)
- Logout menghapus token dari storage dan server
- Semua endpoint PHP dilindungi dari akses browser langsung

---

## Arsitektur Flutter

```
lib/
├── main.dart                        # Entry point, AuthWrapper dengan loading state
├── providers/
│   ├── auth_provider.dart           # Login, token, lokasi, validasi sesi
│   └── connectivity_provider.dart  # Status koneksi WiFi
├── screens/
│   ├── server_config_screen.dart    # Setup IP API + scan jaringan otomatis
│   ├── login_screen.dart            # Dropdown user + password
│   ├── location_selection_screen.dart  # Pilih TOKO/GUDANG
│   ├── dashboard_screen.dart        # Menu utama
│   ├── penjualan_screen.dart        # Transaksi penjualan
│   ├── opname_list_screen.dart      # List history stok opname
│   ├── stok_opname_screen.dart      # Input stok opname baru
│   └── kategori_merk_screen.dart    # CRUD kategori & merk
├── services/
│   ├── api_service.dart             # HTTP client — semua endpoint, debug logging
│   └── storage_service.dart        # SharedPreferences — token, user, lokasi, server
├── widgets/
│   ├── product_search_sheet.dart    # Bottom sheet pencarian barang (reusable, debounce)
│   ├── custom_text_field.dart
│   └── loading_overlay.dart
├── models/
│   └── penjualan_model.dart
└── utils/
    └── safe_convert.dart            # safeToDouble, safeToInt
```

---

## PHP API

Semua file di folder `api/`. Berjalan di Apache/AppServ lokal.

| File | Fungsi | Proteksi |
|---|---|---|
| `index.php` | Admin panel konfigurasi database (web UI dengan animasi) | Publik |
| `config.php` | Kredensial database | 403 jika diakses browser |
| `db_connect.php` | Koneksi PDO MySQL | 403 jika diakses browser |
| `auth_check.php` | Helper validasi Bearer token | 403 jika diakses browser |
| `no_browser.php` | Blokir akses browser ke endpoint API | — |
| `auth_login.php` | Login MD5, generate token sesi | Blokir browser |
| `get_users.php` | Daftar user aktif (untuk dropdown login) | Blokir browser |
| `get_pelanggan.php` | Pelanggan aktif + HUTANGAKHIR + JANGKAPIUTANG | Blokir browser |
| `get_stock.php` | Cari barang dengan JOIN kategori/merk/supplier | Blokir browser |
| `get_databases.php` | List database MySQL (bukan hardcoded) | Blokir browser |
| `get_opname_list.php` | List history stok opname (header saja, ringan) | Blokir browser |
| `master_kategori_merk.php` | CRUD kategori & merk — validasi duplikat | Blokir browser |
| `update_product.php` | Update NAMA_KATEGORI dan NAMA_MERK di tbl_barang | Blokir browser |
| `sync_penjualan.php` | Simpan transaksi penjualan lengkap | Blokir browser |
| `sync_stokopname.php` | Simpan stok opname (fast mode + background) | Blokir browser |
| `save_config.php` | Simpan config database | Token `X-Admin-Token` |
| `test_db_connection.php` | Test koneksi + list database | Publik (untuk setup) |
| `.htaccess` | Blokir listing direktori, error page kustom, security headers | — |
| `403.html` | Halaman 403 kustom (animasi api + kilat) | — |

---

## Logika Simpan Penjualan (`sync_penjualan.php`)

Urutan dalam satu transaksi database — sama persis `Prosessimpan` di VB:

1. **Generate nomor** `PJ-YYMMDD-XXXX` — cek MAX dari `penjualan` DAN `penjualan_ditahan`
2. **Insert header** `penjualan` — semua kolom termasuk akun kas, transfer, metode, bank, sales
3. **Insert detail** `penjualan_detail` per item
4. **Update** `PENJUALAN_TOKO` atau `PENJUALAN_GUDANG` += qty_satuan per item
5. **Insert** `HistoryBarang` per item
6. **Recalculate** `STOK_TOKO` dan `STOK_GUDANG` per barang yang terlibat (`HitungStokPerubahan`)
7. **Insert jurnal** 10 entri
8. **Recalculate saldo** semua akun dari `JurnalUmum` (`UpdateSaldoSemuaAkun`)
9. **Insert** `penjualan_Piutang` jika ada sisa tagihan
10. **Recalculate** `HUTANGAKHIR` pelanggan dari `SUM(SISA_TAGIHAN)` di tabel penjualan

### Kode Akun COA (sesuai `MigrasiCOA_Baru.sql`)

| Jurnal | Akun Debet | Akun Kredit |
|---|---|---|
| Kas tunai | KAS (dari `tbl_perusahaan`) | — |
| Transfer | BANK (dari payload) | — |
| Piutang | PIUTANG USAHA (dari `tbl_perusahaan`) | — |
| Diskon item/total | `05.04.001` POTONGAN DISKON PENJUALAN | — |
| HPP | `06.01.001` HPP POKOK PENJUALAN | — |
| Penjualan kotor | — | `05.02.001` PENJUALAN |
| Persediaan keluar | — | PERSEDIAAN BARANG (dari `tbl_perusahaan`) |
| Pajak | — | `03.02.001` HUTANG PAJAK |
| Biaya kirim | — | `08.01.002` PENDAPATAN LAIN LAIN |

---

## Logika Simpan Stok Opname (`sync_stokopname.php`)

Urutan — sama persis `Simpandata` di VB:

1. **Generate nomor** `SO-YYMMDD-XXXX` — cek MAX dari `Stok_Opname`
2. **Insert** `Stok_Opname` per item
3. **Update** `OPNAME_TOKO` atau `OPNAME_GUDANG` += selisih_qty per item
4. **Update** `NAMA_KATEGORI` dan `NAMA_MERK` di `tbl_barang` jika diubah *(fitur tambahan vs desktop)*
5. **Insert** `HistoryBarang` per item — `JENIS='OPNAME'`, `QTY=stok_nyata`
6. **Insert jurnal** per item jika `total_harga != 0`
   - Selisih minus: D `06.04.001` / K PERSEDIAAN BARANG
   - Selisih plus: D PERSEDIAAN BARANG / K `06.04.001`
7. **[Fast mode]** Response dikirim ke Flutter → proses berat berjalan di background:
8. **Recalculate** `STOK_TOKO` dan `STOK_GUDANG` per barang (`HitungStokPerubahan`)
9. **Recalculate saldo** semua akun (`UpdateSaldoSemuaAkun`)

---

## Rumus Recalculate Stok (`HitungStokPerubahan`)

```sql
STOK_TOKO = COALESCE(AWAL_TOKO, 0)
    + COALESCE(TAMBAH_TOKO, 0)       - COALESCE(KURANG_TOKO, 0)
    + COALESCE(PEMBELIAN_TOKO, 0)    - COALESCE(PENJUALAN_TOKO, 0)
    - COALESCE(RETUR_BELI_TOKO, 0)   + COALESCE(RETUR_JUAL_TOKO, 0)
    + COALESCE(OPNAME_TOKO, 0)
    + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)   - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
    + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0) - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
    + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0) - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0)
```
*(sama untuk GUDANG — pakai kolom `_GUDANG`)*

> **Penting:** `STOK_TOKO` dan `STOK_GUDANG` tidak pernah diupdate langsung.  
> Selalu update kolom mutasi (`PENJUALAN_TOKO`, `OPNAME_TOKO`, dll) lalu recalculate.

---

## Kolom Kunci `tbl_barang`

| Kolom | Keterangan |
|---|---|
| `STOK_TOKO` | Stok akhir toko — hasil recalculate, **jangan diupdate langsung** |
| `STOK_GUDANG` | Stok akhir gudang — hasil recalculate |
| `AWAL_TOKO/GUDANG` | Stok awal periode — tidak diubah oleh mobile |
| `PENJUALAN_TOKO/GUDANG` | Akumulasi qty terjual — diupdate saat sync penjualan |
| `OPNAME_TOKO/GUDANG` | Akumulasi selisih opname — diupdate saat sync opname |
| `NAMA_KATEGORI` | Kolom denormalized — bisa diupdate dari mobile via `update_product.php` |
| `NAMA_MERK` | Kolom denormalized — bisa diupdate dari mobile |

---

## Keamanan API

| Mekanisme | Detail |
|---|---|
| `no_browser.php` | Deteksi `Accept: text/html` → blokir browser, izinkan Flutter |
| `auth_check.php` | Validasi `Bearer token` ke kolom `login_session_key` di `tbl_user` |
| `config.php` | Return 403 jika diakses langsung via URL |
| `save_config.php` | Wajib header `X-Admin-Token: kasir-admin-2026` |
| `.htaccess` | `Options -Indexes`, security headers, blokir file sensitif |
| `403.html` | Halaman error kustom — tidak expose info sistem |
| PDO prepared statement | Semua query parameterized — aman dari SQL injection |
| Password | MD5 sesuai desktop VB.NET, tidak pernah dikirim ke JavaScript |

---

## Debug Logging

Semua request API menghasilkan log di Flutter DevTools / logcat:

```
[Auth] 🔄 initializeAuth
[Auth]    serverConfigured=true  serverUrl=http://192.168.0.97/api
[Auth]    userData=✅ ada  token=✅ ada (a3f8b2c1...)
[Auth] 🔍 Validasi token ke server...
[API]  ➡️  GET http://192.168.0.97/api/get_users.php
[API]      token: ✅ ada (a3f8b2c1...)
[API]  ✅  200 (234ms) ← get_users.php
[Auth] ✅ Auto-login berhasil: Fajar @ TOKO
[Login] 🔐 _login: user=Fajar
[API]  ➡️  POST http://192.168.0.97/api/auth_login.php
[API]  ✅  200 (312ms) ← auth_login.php
[Auth]     token dari server: ✅ b7e9a1f2...
[Auth] ✅ Login berhasil: Fajar level=Master
```

---

## Setup

### Prasyarat
- Flutter SDK ≥ 3.11 (Dart ≥ 3.11)
- PHP ≥ 8.0 + MySQL 8.0
- Apache/AppServ di komputer server (sama dengan AppKasir desktop)
- HP Android dengan USB debugging atau di jaringan WiFi yang sama

### Langkah

```bash
# 1. Install dependencies Flutter
flutter pub get

# 2. Build APK debug (untuk testing)
flutter build apk --debug

# 3. Build APK release
flutter build apk --release
# APK: build/app/outputs/flutter-apk/app-release.apk

# 4. Jalankan di device (USB)
flutter run
```

### Setup Server

```
1. Copy seluruh folder api/ ke:
   AppServ  → C:\AppServ\www\api\
   XAMPP    → C:\xampp\htdocs\api\
   
2. Buka browser → http://localhost/api/
   - Test koneksi database
   - Pilih database AppKasir
   - Simpan konfigurasi

3. Pastikan kolom login_session_key ada di tbl_user:
   ALTER TABLE tbl_user ADD COLUMN login_session_key VARCHAR(100) DEFAULT NULL;
```

### Konfigurasi di App Android

```
URL format : http://192.168.x.x/api
             (tanpa trailing slash, tanpa nama file)

Contoh     : http://192.168.0.97/api

Tips       : Gunakan fitur "Scan Jaringan" di halaman konfigurasi
             untuk menemukan IP server otomatis
```

---

## Dependencies

| Package | Versi | Fungsi |
|---|---|---|
| `provider` | ^6.1.2 | State management |
| `http` | ^1.2.1 | HTTP client untuk API |
| `shared_preferences` | ^2.2.3 | Simpan token, user, lokasi, server URL |
| `mobile_scanner` | ^3.5.6 | Scan barcode kamera |
| `connectivity_plus` | ^5.0.2 | Deteksi status koneksi |
| `flutter_secure_storage` | ^9.0.0 | Simpan data sensitif |
| `intl` | ^0.19.0 | Format angka dan tanggal Indonesia |
| `json_annotation` | ^4.9.0 | Serialisasi JSON |

---

## Catatan Penting

### Hal yang TIDAK boleh dilakukan dari mobile
- Update `STOK_TOKO` atau `STOK_GUDANG` langsung — selalu lewat kolom mutasi
- Mengubah `AWAL_TOKO/GUDANG` — ini stok awal periode, dikelola desktop
- Menghapus transaksi — hanya bisa dari desktop VB.NET

### Sinkronisasi data
- Data yang disimpan mobile **langsung terlihat di desktop** karena pakai database yang sama
- Recalculate stok berjalan di background setelah response dikirim (fast mode)
- Jika desktop dan mobile simpan bersamaan, nomor transaksi dijamin unik karena di-generate dalam transaksi database

### Kolom `login_session_key`
- Wajib ada di `tbl_user` — dipakai untuk validasi token
- Diisi saat login, dikosongkan saat logout
- Jika kolom belum ada, jalankan: `ALTER TABLE tbl_user ADD COLUMN login_session_key VARCHAR(100) DEFAULT NULL;`
