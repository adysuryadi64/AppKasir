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
│   ├── FormUtama.vb        # Main Dashboard dengan background loading
│   └── ModuleHapusTransaksi.vb # Logika tersentralisasi untuk reversal data
│
├── 1Master/                # 📁 Manajemen Data Master
│   ├── TambahBarang.vb     # Master Produk (Multi-Satuan, Harga Partai/Umum)
│   ├── TambahPelanggan.vb  # Master Pelanggan & Supplier
│   ├── TambahMerk.vb       # Manajemen Kategori & Merk
│   └── FormCabang.vb       # Manajemen Multi-Cabang & Lokasi Rak
│
├── 2Trans/                 # 🛒 Modul Transaksi Operasional
│   ├── FormJual.vb         # Kasir Penjualan Retail & Partai
│   ├── FormPembelian.vb    # Pembelian / Restock Barang
│   ├── FormReturBeli.vb    # Retur Pembelian ke Supplier
│   ├── FormReturPenjualan.vb # Retur Penjualan dari Pelanggan
│   ├── FormTransferCabang.vb # Pemindahan stok antar cabang
│   └── FormPembelianDitahan.vb # Hold & Recall Transaksi
│
├── 3Jurnal/                # 📓 Akuntansi & Keuangan
│   ├── JurnalUmum/         # Double-entry bookkeeping engine
│   ├── SetSaldoAwal/       # Setup Buku Besar
│   └── TutupBuku/          # Proses End-of-Month / End-of-Year
│
├── 4Gaji/                  # 👥 Manajemen SDM
│   └── Penggajian/         # Hitung Gaji, Potongan Kasbon, & Tunjangan
│
├── 5Lap/                   # 📊 Reporting & Analitik Bisnis
│   ├── FormLapLabaRugi.vb  # Analisis Keuntungan & Arus Kas
│   ├── FormLapOmset.vb     # Analisis Pendapatan Harian/Bulanan
│   ├── FormLapRanking.vb   # Analisis Kinerja (Barang Terlaris, Sales Terbaik)
│   ├── FormAuditTrail.vb   # Pelacakan Aktivitas User
│   └── *.rdlc / *.frx      # Puluhan template ReportViewer & FastReport
│
├── 6Print/                 # 🖨️ Engine Cetak Hardware
│   ├── PrinterEscPos.vb    # Direct-to-port thermal printing
│   ├── RawPrinterHelper.vb # API Spooler Windows bypass
│   └── Cetak*/             # Modul spesifik per dokumen (Nota, Surat Jalan, dll)
│
├── 8Uty/                   # ⚙️ Utilitas & Keamanan Enterprise
│   ├── FormMigrasiDB.vb    # Auto-updater struktur database
│   ├── FormPerbaikanDatabase.vb # Tools maintenance tabel
│   ├── FormHistory.vb      # Riwayat Mutasi Barang
│   └── FormQuery.vb        # Eksekusi kueri SQL manual
│
├── Modules/                # 🧠 Library & Engine Utama
│   ├── ModuleVariabel.vb   # Konstanta & Parameter Sistem
│   └── ModuleAuditTrail.vb # Engine Pencatat Jejak User
│
└── MySQL/                  # 🗄️ Driver Database Standalone
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

## 📄 FormPenjualan - Halaman Penjualan (Detail Lengkap)

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
| `ID_BARANG` | VARCHAR(50) | PRIMARY KEY - ID unik barang |
| `NAMA_BARANG` | VARCHAR(200) | Nama barang |
| `HARGA_BELI` | DECIMAL(15,4) | HPP Terkini (Moving Average) |
| `BARCODE_KECIL` | VARCHAR(20) | Barcode untuk satuan kecil |
| `SATUAN_UMUM_KECIL` | VARCHAR(20) | Nama satuan kecil |
| `ISI_UMUM_KECIL` | INT | Isi satuan kecil |
| `HARGA_JUAL_UMUM_KECIL` | DECIMAL(15,2) | Harga jual satuan kecil |
| `STOK_AWAL_TOKO` | DECIMAL(15,4) | Stok awal setup sistem |
| `PEMBELIAN_TOKO` | DECIMAL(15,4) | Counter total masuk (Beli) |
| `PENJUALAN_TOKO` | DECIMAL(15,4) | Counter total keluar (Jual) |
| `RETUR_BELI_TOKO` | DECIMAL(15,4) | Counter retur ke supplier |
| `RETUR_JUAL_TOKO` | DECIMAL(15,4) | Counter retur dari pelanggan |
| `STOK_TOKO` | DECIMAL(15,4) | Saldo stok fisik (Computed) |
| `STATUS` | VARCHAR(10) | Aktif/Non-Aktif |

#### Tabel: `penjualan` - Header Transaksi
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `ID_PENJUALAN` | VARCHAR(30) | PRIMARY KEY - ID transaksi |
| `ID_PELANGGAN` | VARCHAR(10) | FK ke tbl_pelanggan |
| `TGL_TRANSAKSI` | DATETIME | Waktu transaksi dibuat |
| `GRAND_TOTAL_STL_PAJAK` | DECIMAL(15,0) | Total setelah pajak |
| `BAYAR` | DECIMAL(15,0) | Nominal dibayar |
| `STATUS_BAYAR` | VARCHAR(20) | LUNAS / BELUM LUNAS |
| `NILAI_RETUR` | DECIMAL(15,0) | Akumulasi nominal retur |
| `TGL_RETUR` | DATETIME | Tgl terakhir diretur |

#### Tabel: `penjualan_detail` - Detail Item
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `FAKTUR_JUAL` | VARCHAR(15) | FK ke penjualan.ID_PENJUALAN |
| `ID_BARANG` | VARCHAR(15) | FK ke tbl_barang.ID_BARANG |
| `QTY` | DECIMAL(10,2) | Jumlah pesanan |
| `QTY_SATUAN` | DECIMAL(10,2) | Jumlah dalam satuan dasar |
| `HARGA_JUAL` | DECIMAL(15,0) | Harga jual per item |
| `TOTAL_HARGA` | DECIMAL(15,0) | Total setelah diskon item |

#### Tabel: `tbl_datareferensi` - Buku Besar Akun
| Kolom | Tipe | Deskripsi |
|-------|------|-----------|
| `KODE_AKUN` | VARCHAR(20) | Kode rekening (Contoh: 11.01.001) |
| `NAMA_AKUN` | VARCHAR(100) | Nama perkiraan kas/bank/beban |
| `SALDO_AKHIR` | DECIMAL(20,0) | Saldo real-time sinkronisasi jurnal |
| `TYPE_AKUN` | VARCHAR(30) | Aktiva, Pasiva, Modal, Pendapatan, Beban |

#### Tabel: `tbl_hak_akses` - Hak Akses Pengguna
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `NO` | INT | PRIMARY KEY - Nomor urut |
| `UserName` | VARCHAR(30) | Nama pengguna |
| `Role` | VARCHAR(100) | Role pengguna |
| `CanRead` | TINYINT(1) | Hak baca (0/1) |
| `CanAdd` | TINYINT(1) | Hak tambah (0/1) |
| `CanEdit` | TINYINT(1) | Hak edit (0/1) |
| `CanDelete` | TINYINT(1) | Hak hapus (0/1) |

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
