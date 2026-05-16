# Requirements — Kasir Lancar Mobile: UI + Sinkronisasi Backend (v2)

## Latar Belakang

Aplikasi Kasir Lancar Mobile sudah fungsional untuk Penjualan dan Stok Opname dasar.
Revisi v2 ini mencakup redesign menyeluruh UI, penambahan fitur yang setara dengan
desktop VB.NET, dan penyempurnaan sinkronisasi backend.

---

## Ruang Lingkup

### Yang TERMASUK dalam spec ini
1. **Login** — redesign layout agar dropdown & password tidak perlu scroll
2. **Dashboard** — modern dengan sidebar, header perusahaan, data real dari server
3. **AI Analytics** — 6 kartu ringkasan di dashboard, klik → modal detail lengkap
4. **Penjualan** — alur multi-screen (Tanggal/Pelanggan/Sales → Item → Pembayaran → Preview Nota), diskon item + global, pajak, biaya kirim, akun COA dari tbl_perusahaan, hak akses user, cetak thermal Bluetooth
5. **Stok Opname** — redesign: tampilkan stok sistem & input real lebih besar, satuan, keterangan, tombol simpan di bawah
6. **List Barang & List Opname** — redesign card/list
7. **Nota Thermal** — cetak 58mm/80mm via Bluetooth, layout kolom persentase, 8 model
8. **Laporan Stok** — screen laporan stok barang per lokasi
9. **Hak Akses User** — ambil dari server, terapkan di penjualan (boleh minus, boleh satuan berbeda, boleh rugi, dll)
10. **Data Perusahaan** — ambil dari tbl_perusahaan untuk header nota dan default akun COA
11. **Konsistensi nama** — "Kasir Lancar" di semua screen

### Yang TIDAK termasuk dalam spec ini
- Retur, Pembelian, Transfer Stok
- Perubahan logika bisnis yang sudah ada dan sudah benar

---

## User Stories

### US-01: Login — Layout Tidak Perlu Scroll
**Sebagai** kasir yang login di HP,  
**Saya ingin** melihat dropdown user dan input password tanpa scroll,  
**Agar** login cepat dan nyaman.

**Acceptance Criteria:**
- Layout: Logo + nama aplikasi di atas (compact), dropdown user + password di tengah layar
- Informasi lain (versi, konfigurasi server) di bawah input
- Tidak perlu scroll untuk mengisi dropdown dan password

### US-02: Dashboard Modern dengan Sidebar
**Sebagai** kasir,  
**Saya ingin** dashboard yang modern dengan sidebar navigasi,  
**Agar** mudah berpindah fitur dan melihat info toko sekilas.

**Acceptance Criteria:**
- Header: nama perusahaan, lokasi aktif (TOKO/GUDANG), nama user
- Sidebar: navigasi ke Penjualan, Stok Opname, Laporan Stok, Pengaturan
- Summary cards: total penjualan hari ini, jumlah transaksi, jumlah opname — data real dari server
- AI Analytics section dihapus
- Data summary diambil sesuai lokasi login

### US-03: Penjualan — Alur Multi-Screen
**Sebagai** kasir,  
**Saya ingin** alur penjualan yang terstruktur dalam beberapa screen,  
**Agar** tidak ada data yang terlewat dan bisa back tanpa kehilangan data.

**Acceptance Criteria:**
- Screen 1: Pilih tanggal, pelanggan, sales
- Screen 2: Tambah item (cari barang, input qty, pilih satuan)
- Screen 3: List item + rincian harga (diskon item per baris, diskon global, pajak, biaya kirim)
- Screen 4: Pembayaran (tunai, transfer, akun COA, info bank jika transfer > 0)
- Screen 5: Preview nota (persis hasil cetak thermal)
- Back antar screen tanpa kehilangan data
- Simpan → cetak/kirim/simpan saja

### US-04: Penjualan — Diskon Item + Diskon Global
**Sebagai** kasir,  
**Saya ingin** bisa input diskon per item DAN diskon global,  
**Agar** total yang tersimpan identik dengan desktop.

**Acceptance Criteria:**
- Setiap item di grid punya kolom diskon (persen atau Rp)
- Ada field diskon total transaksi (persen atau Rp) — opsional
- Ada field pajak (persen atau Rp) — opsional
- Ada field biaya kirim (Rp) — opsional
- Grand total dihitung: subtotal - diskon_item - diskon_global + pajak + biaya_kirim

### US-05: Penjualan — Akun COA dari tbl_perusahaan
**Sebagai** kasir,  
**Saya ingin** akun kas dan transfer terisi otomatis dari pengaturan perusahaan,  
**Agar** jurnal akuntansi selalu benar tanpa perlu pilih manual setiap transaksi.

**Acceptance Criteria:**
- Akun kas tunai default dari `Kode_rek_Jual_Toko` / `Kode_rek_Jual_Gudang` sesuai lokasi
- Akun transfer default dari `Kode_rek_Transfer_Jual`
- User bisa override dengan dropdown akun dari `tbl_datareferensi`
- Data perusahaan (nama, alamat, footer) diambil untuk header nota

### US-06: Penjualan — Hak Akses User
**Sebagai** admin,  
**Saya ingin** hak akses user diterapkan di mobile sama seperti desktop,  
**Agar** kasir tidak bisa melakukan hal yang tidak diizinkan.

**Acceptance Criteria:**
- `IzinkanJualStokMinus` — jika "Tidak", blokir simpan jika stok kurang
- `IzinkanSatuanBerbeda` — jika "Tidak", blokir item duplikat beda satuan
- `IzinkanJualRugi` — jika "Tidak", blokir simpan jika harga jual < harga beli
- `IzinkanUserUbahHargaJual` — jika "Tidak", field harga di item tidak bisa diedit
- `TampilInfoStok` — jika "Iya", tampilkan kolom stok di list item
- Hak akses diambil dari server via endpoint baru `get_hak_akses.php`

### US-07: Penjualan — Cetak Nota Thermal Bluetooth
**Sebagai** kasir,  
**Saya ingin** cetak nota ke printer thermal Bluetooth setelah simpan,  
**Agar** pelanggan mendapat bukti transaksi.

**Acceptance Criteria:**
- Mendukung kertas 58mm (32 karakter) dan 80mm (48 karakter)
- Layout kolom menggunakan persentase (sama dengan desktop VB)
- 8 model nota: kombinasi header kolom, diskon, sisa hutang
- Urutan: simpan dulu → jika berhasil → cetak/kirim/simpan saja
- Jika cetak gagal, transaksi tetap tersimpan
- Screen preview nota menampilkan hasil yang persis sama dengan cetakan

### US-08: Stok Opname — Redesign
**Sebagai** kasir yang melakukan opname,  
**Saya ingin** tampilan opname yang lebih jelas dengan angka besar,  
**Agar** tidak salah input stok nyata.

**Acceptance Criteria:**
- Stok sistem dan input stok nyata ditampilkan dengan font besar
- Informasi satuan ditampilkan (sama seperti desktop)
- Ada field keterangan per item
- Tombol simpan di bawah layar
- Selisih dihitung dan ditampilkan otomatis

### US-09: Laporan Stok Barang
**Sebagai** kasir,  
**Saya ingin** melihat laporan stok barang dari mobile,  
**Agar** bisa cek stok tanpa buka desktop.

**Acceptance Criteria:**
- List barang dengan stok toko dan stok gudang
- Filter per kategori dan merk
- Search by nama/barcode
- Tampilkan: nama, kode, stok toko, stok gudang, satuan

### US-10: AI Analytics — Kartu Ringkasan + Modal Detail
**Sebagai** pemilik toko,  
**Saya ingin** melihat insight bisnis berbasis AI langsung di dashboard,  
**Agar** saya bisa mengambil keputusan operasional yang tepat tanpa harus analisis manual.

**Latar Belakang:**  
Semua data sudah ada di database (penjualan, stok, pelanggan, HPP). AI Analytics di sini
bukan model ML eksternal — melainkan **query analitik cerdas** yang mengolah data historis
untuk menghasilkan insight yang actionable, disajikan dalam bahasa bisnis yang mudah dipahami.

**6 Kartu AI Analytics di Dashboard:**

| # | Kartu | Insight Utama | Sumber Data |
|---|-------|---------------|-------------|
| 1 | 🔥 Produk Terlaris | Top 5 barang by qty & omzet 7 hari terakhir | `penjualan_detail` |
| 2 | 🐌 Barang Lambat | Barang tidak terjual >30 hari, stok masih ada | `penjualan_detail` + `tbl_barang` |
| 3 | ⚠️ Reorder Alert | Barang stok < rata-rata penjualan 7 hari | `tbl_barang` + `penjualan_detail` |
| 4 | ⏰ Jam Puncak | Jam tersibuk transaksi hari ini & minggu ini | `penjualan` |
| 5 | 💰 Margin Profit | Top 5 & bottom 5 barang by margin % | `penjualan_detail` (harga_jual vs harga_beli) |
| 6 | 👥 Pelanggan Aktif | Pelanggan terbaik (RFM sederhana: frekuensi + nilai) | `penjualan` |

**Acceptance Criteria:**
- Setiap kartu menampilkan: ikon, judul, 1-2 angka kunci, 1 kalimat insight
- Kartu menampilkan loading skeleton saat fetch
- Kartu menampilkan "Data tidak cukup" jika data < threshold minimum
- Klik kartu → buka modal/bottom sheet dengan detail lengkap (list, chart sederhana, rekomendasi aksi)
- Data diambil dari endpoint `get_ai_analytics.php?type=xxx&lokasi=xxx`
- Semua kalkulasi dilakukan di PHP (query SQL), bukan di Flutter
- Refresh otomatis setiap 5 menit atau saat pull-to-refresh dashboard

**Detail Modal per Kartu:**

1. **Produk Terlaris** — tabel top 10, kolom: nama, qty terjual, omzet, trend (↑↓ vs minggu lalu)
2. **Barang Lambat** — list barang + hari terakhir terjual + nilai stok tertahan (qty × harga_beli)
3. **Reorder Alert** — list barang + stok saat ini + estimasi habis (stok ÷ rata penjualan/hari) + saran qty order
4. **Jam Puncak** — bar chart sederhana (Flutter) jam 07-22, highlight jam tersibuk, rekomendasi jadwal kasir
5. **Margin Profit** — tabel top 5 margin tertinggi & 5 terendah, kolom: nama, harga beli, harga jual, margin %
6. **Pelanggan Aktif** — list top 10 pelanggan: nama, frekuensi beli, total belanja, terakhir beli, badge (VIP/Reguler/Baru)

### US-11: Validasi Endpoint & Konsistensi
**Sebagai** developer,  
**Saya ingin** semua endpoint PHP tervalidasi dan nama aplikasi konsisten,  
**Agar** tidak ada bug tersembunyi.

**Acceptance Criteria:**
- `get_opname_list.php` mengembalikan `JUMLAH_ITEM` dan `TOTAL_SELISIH`
- Semua teks "AppKasir" diganti "Kasir Lancar" di semua file `.dart`
- `flutter analyze` tidak ada error

---

## Constraint Teknis

- Cetak thermal: gunakan package `bluetooth_print` atau `esc_pos_utils` + `flutter_bluetooth_serial`
- Hak akses: ambil dari server saat login, simpan di SharedPreferences
- Data perusahaan: ambil saat login, simpan di SharedPreferences
- Sidebar: gunakan `Drawer` Flutter bawaan agar mudah extend
- Preview nota: render sebagai widget Flutter (bukan PDF), sama persis dengan cetakan
- Recalculate stok PHP: wajib sama dengan `HitungStokPerubahan` di `ModuleVariabel.vb`
- Recalculate saldo akun PHP: wajib sama dengan `UpdateSaldoSemuaAkun` di `ModuleVariabel.vb`

---

## US-12: Riwayat Penjualan — List & Preview

**Sebagai** kasir,  
**Saya ingin** melihat daftar transaksi penjualan yang sudah tersimpan,  
**Agar** bisa mencetak ulang atau mengirim nota ke pelanggan tanpa harus input ulang.

**Latar Belakang:**  
Fitur ini hanya untuk **baca + cetak/kirim**. Hapus dan edit transaksi tidak tersedia di mobile karena membutuhkan audit jurnal yang kompleks — operasi tersebut tetap dilakukan di desktop VB.

**Acceptance Criteria:**

### List Penjualan
- Tampilkan daftar transaksi penjualan diurutkan terbaru di atas
- Filter tanggal: default hari ini, bisa ubah ke rentang tanggal lain (shortcut: 7H, 30H, Bulan ini)
- Filter lokasi: sesuai lokasi login user
- Setiap card menampilkan:
  - Nomor faktur (ID_PENJUALAN)
  - Tanggal & jam transaksi
  - Nama pelanggan (atau "Umum" jika kosong)
  - Grand total (GRAND_TOTAL_STL_PAJAK)
  - Bayar tunai (BAYAR) + Transfer (NOMINAL_TRANSFER)
  - Sisa piutang (SISA_TAGIHAN) — tampil jika > 0
  - Badge status: LUNAS (hijau) / BELUM LUNAS (oranye) dari STATUS_TRANSAKSI
  - Badge metode pembayaran dari JENIS_PEMBAYARAN
  - User kasir (ID_USER)
- Pull-to-refresh
- Load more (pagination, 30 per halaman)
- Search by nomor faktur atau nama pelanggan

### Detail / Preview Nota
- Klik card → buka preview nota
- Header: nomor faktur, tanggal, pelanggan, lokasi, kasir, metode, status
- Item list dari `penjualan_detail`: nama barang, qty, satuan, harga jual, diskon, total harga — identik kolom VB
- Footer: subtotal, diskon global, pajak, biaya kirim, grand total, bayar, transfer, kembali, sisa piutang
- Tombol [Cetak] → cetak ke printer thermal yang sudah dikonfigurasi
- Tombol [Kirim WA] → share teks nota via share_plus
- **Tidak ada** tombol Hapus
- **Tidak ada** tombol Edit

### Batasan Teknis
- Data diambil dari endpoint baru `get_riwayat_penjualan.php`
- Endpoint hanya SELECT — tidak ada UPDATE/DELETE
- Tidak ada modifikasi jurnal, stok, atau piutang dari mobile
