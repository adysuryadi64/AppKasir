
# Catatan Rilis Teknis - Fitur Sistem Poin Loyalitas  
**Versi Aplikasi:** 15.2026.522.28  

Berikut adalah daftar perubahan teknis yang diimplementasikan dalam rilis ini, berdasarkan analisis git diff:

---

## ð· Fitur Utama: Sistem Poin Loyalitas Pelanggan

### 1. **Konfigurasi & Pengaturan**
- **File Diubah:**  
  `1Master/FormGeneralSetting.vb`, `1Master/FormGeneralSetting.Designer.vb`  
  - Menambahkan **Section Loyalty Poin** di Form General Setting.  
  - Menambahkan dua kontrol baru:  
    - `CmbPoinAktif` (ComboBox aktivasi sistem poin)  
    - `CmbPoinMekanisme` (ComboBox mekanisme perolehan poin: "Per Item (Qty)" atau "Per Kelipatan Nominal")  
  - Visibilitas kontrol `CmbPoinMekanisme` hanya ditampilkan jika sistem poin diaktifkan.  
  - Nilai konfigurasi disimpan ke tabel `hakaksesuser` via `RoleComboList` (migrasi dari key lama).  
  - Menambahkan tooltip penjelasan untuk setiap kontrol.

### 2. **Master Poin & Harga Barang**
- **File Baru:**  
  `1Master/FormMasterPoin.vb`, `1Master/FormMasterPoin.Designer.vb`, `1Master/FormMasterPoin.resx`  
  - Form **Master Poin** dengan 3 tab:  
    1. **Konfigurasi Poin** â Atur earn rate, minimum redeem, dan mekanisme poin.  
    2. **Harga Poin Barang** â Atur harga poin per barang (hanya barang aktif dengan harga poin > 0).  
    3. **Riwayat Poin Pelanggan** â Tampilkan riwayat transaksi poin per pelanggan.  
  - Validasi: Nilai poin harus > 0 saat penyimpanan.  
  - Simpan data ke tabel `poin_config` dan `poin_barang` via transaksi atomik.

### 3. **Integrasi Transaksi Penjualan**
- **File Diubah:**  
  `2Trans/FormJual.vb`, `2Trans/FormJual.Designer.vb`  
  - Menambahkan **Label Saldo Poin** (`LblSaldoPoin`) di area informasi pelanggan.  
  - Label hanya terlihat jika sistem poin aktif (`LP_Aktif = True`).  
  - Poin dihitung otomatis saat transaksi penjualan dan dicatat ke `poin_ledger`.

### 4. **Form Penukaran Poin**
- **File Baru:**  
  `2Trans/FormTukarPoin.vb`, `2Trans/FormTukarPoin.Designer.vb`, `2Trans/FormTukarPoin.resx`  
  - Form **Tukar Poin** untuk menukar poin dengan barang.  
  - Fitur:  
    - Cari pelanggan (auto-complete dari `tbl_pelanggan`).  
    - Tampilkan saldo poin dan status minimum redeem.  
    - Pilih barang dan qty dari daftar barang yang dapat ditukar (aktif + harga poin > 0).  
    - Hitung total poin dibutuhkan dan sisa poin secara real-time.  
    - Validasi: Saldo harus mencukupi minimum redeem dan total poin tidak boleh melebihi saldo.  
    - Konfirmasi penukaran akan mengurangi saldo poin dan stok barang dalam satu transaksi.  
    - Cetak bukti penukaran via MessageBox.

### 5. **Integrasi Retur Penjualan**
- **File Diubah:**  
  `2Trans/FormReturPenjualan.vb`  
  - Menambahkan logika **Void EARN** saat retur penjualan (hanya untuk mode normal, bukan mode bebas).  
  - Hitung poin void secara proporsional jika retur parsial, atau void seluruhnya jika retur penuh.  
  - Catat transaksi void ke `poin_ledger` dengan tipe `VOID_EARN`.

### 6. **Cetak Struk Penjualan**
- **File Diubah:**  
  `6Print/CetakPenjualan/EscPosCetakjualThermalMatrik.vb`,  
  `6Print/CetakPenjualan/GdiCetakjualThermalMatrik.vb`,  
  `6Print/CetakPenjualan/ModulePrinterJual.vb`  
  - Menambahkan blok **Poin Loyalitas** di footer struk:  
    - Tampilkan `Saldo Poin` dan `Poin Diperoleh` (jika ada).  
    - Hanya ditampilkan jika sistem poin aktif dan ada pelanggan.  
  - Variabel publik `Jual_PoinDiperoleh` dan `Jual_SaldoPoinAkhir` diisi dari modul poin.

---

## ð· Modul Pendukung

### 1. **Engine Kalkulasi Poin**
- **File Baru:**  
  `2Trans/ModuleLoyaltyPoin.vb`  
  - Cache konfigurasi poin (`LP_Aktif`, `LP_Mekanisme`, `LP_PoinPerQty`, `LP_KelipatanNominal`, `LP_MinimumRedeem`).  
  - Fungsi:  
    - `HitungPoinEarn()` â Kalkulasi poin dari transaksi penjualan (sesuai mekanisme).  
    - `CatatEarn()` â Catat EARN ke `poin_ledger` + update `SALDO_POIN`.  
    - `CatatRedeem()` â Catat REDEEM ke `poin_ledger` + kurangi `SALDO_POIN`.  
    - `CatatVoidEarn()` â Catat VOID_EARN ke `poin_ledger` + kurangi `SALDO_POIN` (dengan batas minimum 0).  
    - `AmbilSaldoPoin()` â Query saldo poin pelanggan.  
    - `AmbilPoinEarnDariFaktur()` â Ambil poin EARN dari nomor faktur (untuk retur).  
  - Semua operasi tulis menerima `MySqlTransaction` untuk atomisitas.

### 2. **Verifikasi & Testing**
- **File Baru:**  
  `Database/33_loyalty_point_verify.sql`  
  - Script SQL untuk verifikasi end-to-end:  
    - Cek struktur tabel `poin_*`.  
    - Rekonstruksi saldo dari ledger (harus sama dengan `SALDO_POIN`).  
    - Pastikan tidak ada saldo negatif.  
    - Cek integritas referensi (EARN harus punya faktur penjualan).  
    - Cek VOID_EARN tidak melebihi EARN asal.  
    - Statistik ringkas transaksi poin.

---

## ð· Perubahan UI & Navigasi

### 1. **Menu Utama**
- **File Diubah:**  
  `0Form/FormUtama.vb`, `0Form/FormUtama.Designer.vb`, `0Form/FormUtama.resx`  
  - Menambahkan tombol:  
    - `BtnMasterPoin` (di panel Master)  
    - `BtnTukarPoin` (di panel Transaksi, setelah `BtnSuratJalan`)  
  - Event handler untuk navigasi ke form masing-masing.  
  - Mengganti beberapa gambar menu (misal: `MenuLaporan.Image`, `MenuUtility.Image`, dll.) untuk konsistensi visual.

### 2. **Tooltip & Tema**
- **File Diubah:**  
  `Modules/ModuleTooltip.vb`  
  - Menambahkan tooltip untuk `BtnTukarPoin` yang menjelaskan fungsi form penukaran poin.  
  `Modules/ModuleTheme.vb`  
  - Menambahkan `BtnTukarPoin` dan `BtnMasterPoin` ke daftar tombol yang diberi tema.

---

## ð· Migrasi & Perbaikan Database

### 1. **Tabel Audit Config**
- **File Baru:**  
  `Database/34_migrasi_audit_config.sql`  
  - Buat tabel `tbl_audit_config` khusus untuk konfigurasi audit (retensi, arsip terakhir).  
  - Migrasi data dari `hakaksesuser` ke tabel baru.  
  - Hapus data lama dari `hakaksesuser` untuk menghindari duplikasi.  
  - Script idempoten (aman dijalankan berulang).

### 2. **Perbaikan Baca Setting**
- **File Diubah:**  
  `Modules/ModulHakAkses.vb`  
  - Perbarui cara baca batas satuan (`JualBatasSatuanSedang/Besar`) untuk mendukung migrasi key (dari label text).  
  - Baca setting dari `hakaksesuser` dengan filter key baru dan lama.

### 3. **Audit Trail**
- **File Diubah:**  
  `Modules/ModuleAuditTrail.vb`  
  - Ganti sumber data `AuditRetensi` dan `AuditArsipTerakhir` dari `hakaksesuser` ke `tbl_audit_config`.

---

## ð· File Proyek & Versi

### 1. **Penambahan File ke Proyek**
- **File Proyek:** `AppKasir.vbproj`  
  - Tambah compile untuk:  
    - `1Master/FormMasterPoin.vb/.Designer.vb`  
    - `2Trans/FormTukarPoin.vb/.Designer.vb`  
    - `2Trans/ModuleLoyaltyPoin.vb`  
  - Tambah embedded resource untuk file `.resx` baru.

### 2. **Update Versi**
- **File Diubah:**  
  `My Project/AssemblyInfo.vb`  
  - Update versi assembly menjadi **15.2026.522.28**.  
  `update.xml`  
  - Update versi update ke **15.2026.522.28** dengan link changelog dan download.

---

## ð· Catatan Tambahan

- **Requirement Terpenuhi:**  
  - Req 1â9 (Sistem Poin Loyalitas)  
  - Req 6 (Cetak poin di struk)  
  - Req 8 (Void poin saat retur)  
  - Req 9 (Testing & verifikasi)

- **Ketergantungan:**  
  - Fitur poin hanya aktif jika diaktifkan via Form General Setting.  
  - Semua operasi poin (EARN, REDEEM, VOID) tercatat di `poin_ledger` dan memengaruhi `SALDO_POIN` di `tbl_pelanggan`.  
  - Transaksi poin dan transaksi induk (penjualan/retur) harus dalam **satu transaksi database** untuk konsistensi.

- **Verifikasi:**  
  Jalankan `33_loyalty_point_verify.sql` di database development untuk memastikan implementasi berjalan dengan benar.

---

**Diterbitkan oleh:** Tim Pengembang AppKasir  
**Tanggal:** 2026-05-22
