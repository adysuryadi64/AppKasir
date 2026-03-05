# ?? AppKasir - Sistem Point-of-Sale (POS) Terpadu

**Versi:** 1.0.0  
**Bahasa:** Visual Basic .NET (VB.NET)  
**Framework:** .NET Framework 4.7.2+ / .NET 6+  
**Database:** MySQL 5.7+  
**UI Framework:** Windows Forms  
**Repository:** https://github.com/adysuryadi64/AppKasir

---

## ?? Ringkasan Aplikasi

**AppKasir** adalah aplikasi **Point-of-Sale (POS)** lengkap berbasis Windows Forms yang dirancang untuk bisnis ritel/toko. Aplikasi ini mendukung transaksi penjualan kompleks dengan fitur-fitur profesional:

### ? Keunggulan Utama
- ? **Pembelian multi-item** dengan pencarian barang real-time
- ? **Barcode scanner support** dengan deteksi otomatis vs input manual
- ? **Sistem satuan multi-level** (kecil, sedang, besar / umum, partai)
- ? **Perhitungan diskon & pajak** fleksibel (persen/nominal per item dan per transaksi)
- ? **Berbagai metode pembayaran** (tunai, transfer bank, QRIS, tempo)
- ? **Manajemen stok** (toko & gudang terpisah)
- ? **Pencetakan nota** otomatis atau manual
- ? **Penahanan transaksi** (hold/recall untuk kemudian)
- ? **Integrasi jurnal akuntansi** (double-entry bookkeeping)
- ? **Kontrol akses berbasis role** (hak akses per pengguna)
- ? **Pengaturan printer** dan preferensi pengguna

---

## ?? Daftar Fitur Lengkap

| Fitur | Deskripsi |
|-------|-----------|
| **Form Penjualan** | Input transaksi penjualan dengan grid item dinamis |
| **Pencarian Barang** | Real-time search by ID/nama/barcode dengan ListBox dropdown |
| **Barcode Detection** | Hybrid mode: scan otomatis vs input manual dengan timing detection |
| **Multi-Satuan** | Dukungan 3 level satuan per barang (kecil/sedang/besar) |
| **Diskon & Pajak** | Per-item dan per-transaksi, fleksibel persen/nominal |
| **Pembayaran** | Tunai, transfer, tempo dengan manajemen rekening |
| **Penahanan** | Simpan transaksi sementara & panggil kembali |
| **Cetak Nota** | Pilihan: otomatis, manual, atau tanya setiap kali |
| **Jurnal Akuntansi** | Double-entry journaling otomatis |
| **Kontrol Akses** | Role-based permissions dari `tbl_hak_akses` |
| **Edit Transaksi** | Ubah transaksi yang sudah disimpan |
| **History Barang** | Audit trail setiap transaksi item |

---

## ??? Arsitektur Sistem

### Struktur Folder
```
AppKasir/
??? My Project/
?   ??? Application.myapp              (App config: MainForm, Visual styles)
?   ??? AssemblyInfo.vb                (Version, assembly info)
?   ??? Settings.settings              (User preferences)
?
??? 2Trans/
?   ??? FormPenjualan.vb               (Main sales form: logic)
?   ??? FormPenjualan.Designer.vb      (UI layout)
?   ??? FormPenjualan.resx             (Resources: icons, images)
?
??? Modules/
?   ??? ModulHakAkses.vb               (Role-based access control)
?   ??? ModulKoneksi.vb                (Database connection)
?   ??? ModulJurnal.vb                 (Journal management)
?
??? Forms/
?   ??? FormUtama.vb                   (Main menu / dashboard)
?   ??? FormGeneralSetting.vb          (Settings & permissions)
?   ??? TambahBarang.vb                (Add/edit product)
?   ??? TambahPelanggan.vb             (Add/edit customer)
?   ??? FormLogin.vb                   (Login screen)
?
??? DATABASE_SCHEMA.md                 (Database schema documentation)
??? DEVELOPER_GUIDE.md                 (Developer guide)
??? README.md                          (This file)
??? AppKasir.sln                       (Solution file)
??? AppKasir.vbproj                    (Project file)
```

### Lapisan Aplikasi
```
?????????????????????????????????????
?  PRESENTATION LAYER               ?
?  FormPenjualan, FormUtama, etc    ?
?  UI: WinForms controls            ?
?????????????????????????????????????
               ?
?????????????????????????????????????
?  BUSINESS LOGIC LAYER             ?
?  - Kalkulasi diskon/pajak         ?
?  - Manajemen transaksi            ?
?  - Barcode detection              ?
?  - Stok management                ?
?  - Jurnal creation                ?
?????????????????????????????????????
               ?
?????????????????????????????????????
?  DATA ACCESS LAYER                ?
?  - MySql queries                  ?
?  - Connection management          ?
?  - Transaction handling           ?
?  - Data validation                ?
?????????????????????????????????????
               ?
?????????????????????????????????????
?  DATABASE LAYER (MySQL)           ?
?  - tbl_barang                     ?
?  - penjualan                      ?
?  - penjualan_detail               ?
?  - tbl_pelanggan                  ?
?  - tbl_karyawan                   ?
?  - tbl_hak_akses (roles)          ?
?  - tbl_piutang                    ?
?????????????????????????????????????
```

---

## ?? Quick Start Setup

### Prasyarat Sistem
- **OS:** Windows 7 SP1+ atau Windows Server 2008 R2+
- **IDE:** Visual Studio 2022 / 2026
- **Runtime:** .NET Framework 4.7.2+ atau .NET 6+
- **Database:** MySQL 5.7+ atau MySQL 8.0+
- **Koneksi:** LAN ke MySQL server

### Langkah 1: Clone Repository
```bash
git clone https://github.com/adysuryadi64/AppKasir.git
cd AppKasir
```

### Langkah 2: Setup Database MySQL
```sql
-- Buat database
CREATE DATABASE IF NOT EXISTS appkasir DEFAULT CHARSET utf8mb4;
USE appkasir;

-- Buat tabel utama (import dari schema file jika ada)
-- Contoh tabel barang:
CREATE TABLE tbl_barang (
  ID_BARANG VARCHAR(50) PRIMARY KEY,
  NAMA_BARANG VARCHAR(255) NOT NULL,
  HARGA_BELI DECIMAL(18,2),
  BARCODE_KECIL VARCHAR(50),
  BARCODE_SEDANG VARCHAR(50),
  BARCODE_BESAR VARCHAR(50),
  SATUAN_UMUM_KECIL VARCHAR(50),
  SATUAN_UMUM_SEDANG VARCHAR(50),
  SATUAN_UMUM_BESAR VARCHAR(50),
  ISI_UMUM_KECIL INT DEFAULT 1,
  ISI_UMUM_SEDANG INT DEFAULT 12,
  ISI_UMUM_BESAR INT DEFAULT 100,
  HARGA_JUAL_UMUM_KECIL DECIMAL(18,2),
  HARGA_JUAL_UMUM_SEDANG DECIMAL(18,2),
  HARGA_JUAL_UMUM_BESAR DECIMAL(18,2),
  SATUAN_PARTAI_KECIL VARCHAR(50),
  SATUAN_PARTAI_SEDANG VARCHAR(50),
  SATUAN_PARTAI_BESAR VARCHAR(50),
  ISI_PARTAI_KECIL INT DEFAULT 1,
  ISI_PARTAI_SEDANG INT DEFAULT 12,
  ISI_PARTAI_BESAR INT DEFAULT 100,
  HARGA_JUAL_PARTAI_KECIL DECIMAL(18,2),
  HARGA_JUAL_PARTAI_SEDANG DECIMAL(18,2),
  HARGA_JUAL_PARTAI_BESAR DECIMAL(18,2),
  STOK_TOKO DECIMAL(18,2) DEFAULT 0,
  STOK_GUDANG DECIMAL(18,2) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

### Langkah 3: Konfigurasi Koneksi Database
**File:** `ModulKoneksi.vb`

```vb
Public Module ModulKoneksi
    Public conn As New MySqlConnection("Server=localhost;Database=appkasir;Uid=root;Pwd=your_password;Charset=utf8;")
End Module
```

Atau di **app.config**:
```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=localhost;Database=appkasir;Uid=root;Pwd=your_password;" 
         providerName="MySql.Data.MySqlClient" />
  </connectionStrings>
</configuration>
```

### Langkah 4: Restore NuGet Packages
```bash
cd AppKasir
dotnet restore
```

Atau via Visual Studio:
```
Tools ? NuGet Package Manager ? Package Manager Console
PM> Update-Package
```

### Langkah 5: Build & Run
```bash
# Build
dotnet build

# Run
dotnet run
# atau F5 di Visual Studio
```

---

## ?? Panduan Pengguna

### Login & Akses
1. Jalankan `AppKasir.exe`
2. Form login akan muncul
3. Masukkan username & password
4. Pilih lokasi (TOKO atau GUDANG)
5. Klik **MASUK**

### Menu Utama
```
???????????????????????????????????
?   APPKASIR - MENU UTAMA        ?
???????????????????????????????????
?  [PENJUALAN] (F1)               ?
?  [PEMBELIAN] (F2)               ?
?  [MASTER BARANG] (F3)           ?
?  [MASTER PELANGGAN] (F4)        ?
?  [SETTING] (F5)                 ?
?  [LAPORAN] (F6)                 ?
?  [KELUAR] (F12)                 ?
?                                 ?
?  Lokasi: [TOKO ?]               ?
?  User: Admin | Waktu: 10:30 AM  ?
???????????????????????????????????
```

---

## ?? Calculation Formulas (Standardized)

**Note:** All formulas use `*` for multiplication, `/` for division, `()` for grouping

### Item-Level Formulas

**Formula 1: Calculate Quantity in Base Unit**
```
qty_satuan = qty * isi_satuan

Example: qty=2 pcs, isi_satuan=12
? qty_satuan = 2 * 12 = 24 pcs
```

**Formula 2: Calculate Cost of Goods Sold (COGS)**
```
total_harga_beli = harga_beli * isi_satuan * qty

Example: harga_beli=2500, isi=12, qty=2
? total_harga_beli = 2500 * 12 * 2 = 60000
```

**Formula 3: Calculate Item Discount Amount**
```
?? ACTUAL IMPLEMENTATION:
total_diskon = qty * diskon_rp

OR (if percent input):
diskon_rp = harga_jual * diskon_persen / 100
then: total_diskon = qty * diskon_rp

Example A (percent): harga=50000, diskon%=10, qty=2
? diskon_rp = 50000 * 10 / 100 = 5000
? total_diskon = 2 * 5000 = 10000

Example B (nominal): diskon_rp=5000, qty=2
? total_diskon = 2 * 5000 = 10000
```

?? **NOTE:** Diskon dihitung PER UNIT, bukan total item sekaligus!

**Formula 4: Calculate Item Total**
```
total_harga = (harga_jual * qty) - diskon_item

Example: harga=50000, qty=2, diskon=5000
? total_harga = (50000 * 2) - 5000 = 95000
```

### Transaction-Level Formulas

**Formula 5: Calculate Subtotal (Before Discount)**
```
subtotal = SUM(all_items.total_harga)

Example: item1=95000 + item2=330000
? subtotal = 425000
```

**Formula 6: Calculate Transaction Discount**
```
diskon_transaksi = subtotal * diskon_persen / 100

OR (if user input nominal):
diskon_transaksi = diskon_rp_input

Example A (percent): subtotal=425000, diskon%=10
? diskon_transaksi = 425000 * 10 / 100 = 42500

Example B (nominal): diskon_rp=42500
? diskon_transaksi = 42500
```

**Formula 7: Calculate Tax**
```
pajak = (subtotal - diskon_transaksi) * pajak_persen / 100

OR (if user input nominal):
pajak = pajak_rp_input

Example A (percent): subtotal=425000, diskon=42500, pajak%=11
? pajak = (425000 - 42500) * 11 / 100 = 42075

Example B (nominal): pajak_rp=42075
? pajak = 42075
```

**Formula 8: Calculate Final Total**
```
total_akhir = subtotal - diskon_transaksi + pajak + biaya_kirim

Example: subtotal=425000, diskon=42500, pajak=42075, kirim=10000
? total_akhir = 425000 - 42500 + 42075 + 10000 = 434575
```

**Formula 9: Calculate Change or Balance**
```
kembalian = nominal_bayar - total_akhir

If kembalian > 0: Uang kembalian
If kembalian < 0: Sisa hutang
If kembalian = 0: Pas

Example A (Overpay): bayar=500000, total=434575
? kembalian = 500000 - 434575 = 65425 (return)

Example B (Underpay): bayar=400000, total=434575
? kembalian = 400000 - 434575 = -34575 (debt)
```

### Important Notes

- All monetary values use `DECIMAL(18,2)` for precision
- Rounding: Use ROUND(..., 2) for final display
- Tax is calculated on (subtotal - discount), NOT on subtotal alone
- Discount & tax can be set to 0 (not required)
- Shipping cost is always added to final total

---

### FormPenjualan - Halaman Penjualan (Detail Lengkap)

#### Layout Form Penjualan

```
??????????????????????????????????????????????????????????????????
? FORM PENJUALAN                                          [X]    ?
??????????????????????????????????????????????????????????????????
?                                                                ?
? ?? HEADER TRANSAKSI ????????????????????????????????????????? ?
? ? Faktur: [PJ-260304-0001] ? Tanggal: [04/03/2026 10:30]   ? ?
? ? Pelanggan: [Budi Santoso ?]  (F12)                       ? ?
? ? Jenis: Umum ? Kode: PEL001 ? Lokasi: TOKO                ? ?
? ? Sales: [Adi Suryadi ?] (F2) ? Jangka: 30 hari            ? ?
? ? Jatuh Tempo: [04/04/2026]                                ? ?
? ????????????????????????????????????????????????????????????? ?
?                                                                ?
? ?? PENCARIAN & PENAMBAHAN BARANG ???????????????????????????? ?
? ? Cari Barang: [Sabun________________] (F4 = Add Product)   ? ?
? ? ?? Hasil Pencarian (ListBox) ??????????????????????????? ? ?
? ? ? • Sabun Mandi Cuci => 150 stok                      ? ? ?
? ? ? • Sabun Colek => 85 stok                            ? ? ?
? ? ? • Sabun Batang => 120 stok                          ? ? ?
? ? ???????????????????????????????????????????????????????? ? ?
? ????????????????????????????????????????????????????????????? ?
?                                                                ?
? ?? DETAIL ITEM (DataGridView) ??????????????????????????????? ?
? ? # ?Kode  ?Nama Barang  ?Harga  ?Qty?Stn?Isi?Total   ?Stk? ?
? ??????????????????????????????????????????????????????????? ?
? ?1  ?B001  ?Sabun Mandi  ?50000  ?2  ?Box?12 ?100000  ?150? ?
? ?2  ?B002  ?Minyak Goreng?330000 ?1  ?Dus?100?330000  ?85 ? ?
? ?3  ?      ?             ?       ?   ?   ?   ?        ?   ? ?
? ??????????????????????????????????????????????????????????? ?
?                                                                ?
? ?? KALKULASI ???????????????????????????????????????????????? ?
? ? Total Jual Sebelum Diskon: 430000                         ? ?
? ? Diskon %: [10___] %  ?  Diskon Rp: [43000______]         ? ?
? ? Pajak %:  [11___] %  ?  Pajak Rp:  [42570_____]         ? ?
? ? Biaya Kirim: [10000_____]                                ? ?
? ?                                                            ? ?
? ? TOTAL AKHIR: Rp 439570                                   ? ?
? ????????????????????????????????????????????????????????????? ?
?                                                                ?
? ?? PEMBAYARAN (GroupBox - Hidden sampai BAYAR diklik) ????? ?
? ? Jenis Bayar: [Transfer Bank ?]  ? Nominal: [439570]    ? ?
? ? Bank: [BCA____] ? No Rek: [1234567] ? Nama: [PT XYZ]    ? ?
? ? No Referensi: [TRF-2603001]                             ? ?
? ? Jatuh Tempo: [04/04/2026]                               ? ?
? ? Status: LUNAS ? ? Kembalian: Rp 0                       ? ?
? ????????????????????????????????????????????????????????????? ?
?                                                                ?
? [BAYAR(F8)] [TAHAN(F6)] [PANGGIL(F7)] [BARANG(F4)] [PEL(F12)]?
? [SIMPAN(F10)] [BATAL(F11)] [KELUAR]                           ?
?                                                                ?
??????????????????????????????????????????????????????????????????
```

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

*Editable if LblJualEditHarga = "Iya"
CALC = Calculated field (from formula, not stored)

#### Arti Ikon Status Bar

| Icon | Artinya |
|------|---------|
| ?? | Mode Normal: Semua fitur aktif |
| ?? | Mode Hemat Daya: Beberapa fitur dinonaktifkan |
| ? | Mode Aman: Hanya fitur dasar, akses dibatasi |
| ?? | Terlindungi: Form terkunci, hanya baca |
| ?? | Menunggu: Proses masih berlangsung |

#### Alur Lengkap Transaksi Penjualan

**STEP 1: Load Form**
```
Kondisi Awal:
- Mode: TambahPenjualan (baru) atau EditPenjualan (edit)
- Generate nomor faktur: PJ-YYMMDD-XXXX
- Load list pelanggan dari tbl_pelanggan
- Load list karyawan dari tbl_karyawan
- Set default: Tanggal hari ini, Jangka 30 hari
- Baca hak akses dari ModulHakAkses
```

**STEP 2: Pilih Pelanggan & Sales**
```
User Action: Klik CmbPelanggan
??
Event: CmbPelanggan_SelectedIndexChanged()
??
System:
1. Query: SELECT KODE, JENIS, ALAMAT, JangkaPiutang FROM tbl_pelanggan WHERE NAMA = ?
2. Update LblJenisPl.Text = "Umum" atau "Partai"
3. Update LbLKodePel.Text = Kode pelanggan
4. Hitung Jatuh Tempo = DTPTgl.Value.AddDays(jangka_piutang)
5. Call UpdateHargaBerdasarJenisPelanggan()
   ?? Update harga semua item di DgvData sesuai jenis pelanggan
```

**STEP 3: Tambah Barang (4 Metode)**

**Metode 1: Pencarian Manual (Mode "Pencarian")**
```
User: Ketik di TxtNama: "Sabun"
??
Event: TxtNama_TextChanged()
??
System: Call ProcessManualSearchList("Sabun")
??
Query: SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang
       WHERE NAMA_BARANG LIKE '%Sabun%' OR ID_BARANG LIKE '%Sabun%'
       LIMIT 20
??
Result: ListBox menampilkan:
  • Sabun Mandi => 150
  • Sabun Cuci => 85
??
User: Klik "Sabun Mandi" atau tekan Enter
??
System: Call AmbilDataDariListBox()
??
Lanjut ke STEP 3B
```

**Metode 2: Format Cepat (Qty + Barang)**
```
User: Ketik di TxtNama: "2*Sabun"
??
Event: TxtNama_KeyDown() ? TriggerManualSearch()
??
System:
1. Parse input: qty=2, barang="Sabun"
2. Set TxtQty.Text = "2"
3. Call ProcessManualSearchList("Sabun")
4. Tampilkan ListBox
??
User: Pilih barang
??
System: Call TambahDataLangsung()
```

**Metode 3: Barcode Scanner (Auto-Detect)**
```
User: Scan barcode: 8991111112 (cepat <200ms)
??
Event: TxtNama_KeyDown() ? BarcodeTimer_Tick()
??
System:
1. Deteksi timing: <200ms antar karakter
2. Deteksi panjang: >=4 karakter
3. Query: SELECT NAMA_BARANG FROM tbl_barang
          WHERE BARCODE_KECIL = '8991111112' 
             OR BARCODE_SEDANG = '8991111112'
             OR BARCODE_BESAR = '8991111112' LIMIT 1
4. Ditemukan: "Sabun Mandi"
5. Set TxtQty = "1" (default)
??
System: Call Ambildatalaindaridbbarang("Sabun Mandi")
```

**Metode 4: Edit Langsung Grid**
```
User: Klik sel NamaBarang di row kosong
User: Ketik: "Sabun*2" atau "Sabun"
User: Tekan Enter
??
Event: DgvData_CellEndEdit()
??
System: Call Ambildatalaindaridbbarang()
```

**STEP 3B: Ambil Data Barang dari Database**
```
Call: Ambildatalaindaridbbarang("Sabun Mandi")
??
Query 1: SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI,
         SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR,
         ISI_UMUM_*, HARGA_JUAL_UMUM_*,
         SATUAN_PARTAI_KECIL, ... (all fields)
         FROM tbl_barang WHERE NAMA_BARANG = ? LIMIT 1
??
Determine Satuan:
- If jenis="Partai" ? gunakan SATUAN_PARTAI_*
- If jenis="Umum" ? gunakan SATUAN_UMUM_*
??
Determine Level Satuan (1=Kecil, 2=Sedang, 3=Besar):
- If barcode match BARCODE_SEDANG ? level 2
- If barcode match BARCODE_BESAR ? level 3
- Else ? level 1
??
Query 2: SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang
??
Get Stok:
- If Lokasi="TOKO" ? tampilkan STOK_TOKO
- If Lokasi="GUDANG" ? tampilkan STOK_GUDANG
??
Fill Hidden TextBoxes:
- TxtKode = ID_BARANG
- TxtHargaBeli = HARGA_BELI
- Txtsatuan = SATUAN (pilihan level)
- TxtIsi = ISI (isi satuan)
- TxtHargaJual = HARGA_JUAL (sesuai level & jenis)
- TxtStokToko = STOK_TOKO
- TxtStokGudang = STOK_GUDANG
??
Call: TambahDataLangsung()
```

**STEP 4: Tambah ke DataGridView**
```
Call: TambahDataLangsung()
??
Check: Duplikat barang?
- If Kodebarangsama="Tidak" AND ID_BARANG sudah ada di grid
  ? Show warning, jangan tambah
  ? Merge dengan baris lama (update qty + hitung ulang)
??
Add Row:
DgvData.Rows.Add()
??
Fill Cells:
- Kode = TxtKode
- NamaBarang = NAMA_BARANG
- HargaBeli = HARGA_BELI
- QTY = TxtQty (1 jika kosong)
- Satuan = dropdown pilihan satuan (dari query)
- Isi = ISI_SATUAN
- Harga = HARGA_JUAL
- QtySatuan = QTY × ISI
- DiskonPersen = 0 (default)
- DiskonRp = 0 (default)
- TotalDiskon = 0
- TotalHarga = Harga × QTY
- StokToko = STOK_TOKO
- StokGudang = STOK_GUDANG
??
Call: UpdateSemuaTotal()
??
Clear: KosongTxtboxcari()
??
Focus: SetupFocusToGrid()
```

**STEP 5: Edit Item di Grid (Per Baris)**
```
User: Edit Qty, Satuan, Harga, Diskon di grid
??
Event: DgvData_CellEndEdit(row, col)
??
System: Call HitungNilaiSetiapBaris(rowIndex)
??
Calculate:
- QTY_SATUAN = QTY × ISI
- TOTAL_HARGA_BELI = HARGA_BELI × ISI × QTY
- TOTAL_DISKON = HARGA_JUAL × DISKON_PERSEN / 100
- TOTAL_HARGA = (HARGA_JUAL × QTY) - TOTAL_DISKON
??
Update Cells:
- QtySat.Value = QTY_SATUAN
- TotalHargaBeli.Value = TOTAL_HARGA_BELI
- TotalDiskon.Value = TOTAL_DISKON
- TotalHarga.Value = TOTAL_HARGA
??
Call: UpdateSemuaTotal()
```

**STEP 6: Kalkulasi Total Transaksi**
```
Call: UpdateSemuaTotal()
??
Calculate:
1. TOTAL_SEBELUM_DISKON = SUM(DgvData[TotalHarga])
2. TOTAL_DISKON_TRANSAKSI = TOTAL_SEBELUM_DISKON × (DISKON_PERSEN / 100)
3. PAJAK = (TOTAL_SEBELUM_DISKON - TOTAL_DISKON_TRANSAKSI) × (PAJAK_PERSEN / 100)
4. TOTAL_AKHIR = TOTAL_SEBELUM_DISKON - DISKON_RP + PAJAK_RP + BIAYA_KIRIM
??
Update Labels:
- TxtTotalJualSblDiskon = TOTAL_SEBELUM_DISKON
- TxtTotaljualStlPajak = TOTAL_AKHIR
- TxtGrantotal = TOTAL_AKHIR (formatted currency)
```

**STEP 7: Pembayaran (F8 - BAYAR)**
```
User: Click BtnBayar atau F8
??
Event: BtnBayar_Click()
??
System:
1. Show GBBayar.Visible = True
2. Focus ke TxtNominalBayar
3. Populate CmbJenisBayar dari GetAkunList()
??
User: Pilih jenis pembayaran
??
If Jenis="Transfer":
  - Query rekening: SELECT BANK, NO_REKENING, NAMA_REKENING FROM tbl_akunbank
  - Auto-fill: TxtBank, TxtNoRek, TxtNamaRek
??
User: Input nominal bayar
??
Event: TxtNominalBayar_TextChanged()
??
Calculate Kembalian:
- If nominal > total ? Kembalian = nominal - total
- If nominal < total ? Hutang = total - nominal (Belum Lunas)
- If nominal = total ? Kembalian = 0 (Lunas)
??
Update Status:
- LblStatusTrans = "LUNAS" atau "BELUM LUNAS"
- TxtKembaliHutang = kembalian/hutang
```

**STEP 8: Simpan (F10 - SIMPAN)**
```
User: Click BtnSimpan atau F10
??
Event: BtnSimpan_Click()
??
System:
1. Start MySqlTransaction
??
2. If Mode="EditPenjualan":
   - Call Hapusbaris(transaction) ? DELETE dari penjualan_detail lama
??
3. Call Simpanpenjualan(transaction)
   - INSERT INTO penjualan (ID_PENJUALAN, FAKTUR_JUAL, TANGGAL_PENJUALAN, ...)
     VALUES (TxtFaktur.Text, ...)
??
4. Call Simpanpenjualandetail(transaction)
   - For each row in DgvData:
     INSERT INTO penjualan_detail (FAKTUR_JUAL, ID_BARANG, QTY, HARGA_JUAL, ...)
??
5. If STATUS_BAYAR="BELUM LUNAS":
   - Call Simpanpiutang(transaction)
   - INSERT INTO tbl_piutang (ID_PENJUALAN, ID_PELANGGAN, SISA_PIUTANG, ...)
??
6. Call Simpanjurnal(transaction)
   - INSERT INTO gl_jurnal (NO_TRANSAKSI, DEBIT_AKUN, KREDIT_AKUN, NOMINAL, ...)
   - Contoh: DEBIT: Kas/Bank, KREDIT: Penjualan
??
7. Call HistoryBarang(transaction)
   - INSERT INTO tbl_history_barang (ID_BARANG, TIPE, QTY, ...)
??
8. If Error:
   - transaction.Rollback()
   - Show error message
   - Exit
??
9. If Success:
   - transaction.Commit()
   - Update stok: UPDATE tbl_barang SET STOK_TOKO/GUDANG = ...
   - Call CetakFaktur() based on CmbCetak setting
   - Clear form: Kondisiawal()
   - Show success message
```

**STEP 9: Penahanan (F6 - TAHAN)**
```
User: Click BtnTahan atau F6 (sebelum SIMPAN)
??
System:
1. INSERT INTO penjualan_ditahan (...)
   - Copy header + detail ke tabel tertahan
2. Clear form: Kondisiawal()
??
Nanti:
User: Click BtnPanggil atau F7
??
System: SELECT * FROM penjualan_ditahan WHERE FAKTUR_JUAL = ?
- Load kembali data ke form
- User bisa edit lagi atau langsung bayar
```

---

#### Keyboard Shortcuts (Konfigurasi di Form)

| Shortcut | Action | Method |
|----------|--------|--------|
| **F1** | Buka Form Penjualan | Menu Utama |
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

## ?? Fitur Barcode Hybrid

Sistem mendeteksi input berdasarkan timing:

```
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

## ?? Database Schema (Lengkap)

#### Tabel: `tbl_barang` - Master Barang
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `ID_BARANG` | VARCHAR(50) | PRIMARY KEY - ID unik barang |
| `NAMA_BARANG` | VARCHAR(200) | Nama barang |
| `HARGA_BELI` | DECIMAL(10,2) | Harga beli barang |
| `BARCODE_KECIL` | VARCHAR(20) | Barcode untuk satuan kecil |
| `SATUAN_UMUM_KECIL` | VARCHAR(20) | Nama satuan kecil |
| `ISI_UMUM_KECIL` | INT | Isi satuan kecil |
| `HARGA_JUAL_UMUM_KECIL` | DECIMAL(10,2) | Harga jual satuan kecil |
| `STOK_TOKO` | DECIMAL(10,2) | Stok di lokasi toko |
| `STOK_GUDANG` | DECIMAL(10,2) | Stok di lokasi gudang |

#### Tabel: `penjualan` - Header Transaksi Penjualan
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `ID_PENJUALAN` | VARCHAR(30) | PRIMARY KEY - ID transaksi penjualan |
| `ID_PELANGGAN` | VARCHAR(10) | FK ke tbl_pelanggan |
| `TGL_TRANSAKSI` | DATETIME | Waktu transaksi dibuat |
| `GRAND_TOTAL_SBL_PAJAK` | DECIMAL(15,0) | Total sebelum pajak |
| `DISKON_TOTAL_RP` | DECIMAL(10,2) | Diskon total dalam rupiah |
| `PAJAK_RP` | DECIMAL(10,2) | Pajak dalam rupiah |
| `GRAND_TOTAL_STL_PAJAK` | DECIMAL(15,0) | Total setelah pajak |
| `STATUS_BAYAR` | VARCHAR(20) | Status pembayaran (LUNAS/BELUM LUNAS) |

#### Tabel: `penjualan_detail` - Detail Item Transaksi
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `FAKTUR_JUAL` | VARCHAR(15) | FK ke penjualan.ID_PENJUALAN |
| `ID_BARANG` | VARCHAR(15) | FK ke tbl_barang.ID_BARANG |
| `NAMA_BARANG` | VARCHAR(100) | Nama barang |
| `QTY` | DECIMAL(10,2) | Jumlah barang |
| `SATUAN` | VARCHAR(10) | Satuan barang |
| `HARGA_JUAL` | DECIMAL(15,0) | Harga jual barang |
| `TOTAL_HARGA` | DECIMAL(15,0) | Total harga barang |

#### Tabel: `tbl_pelanggan` - Master Pelanggan
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `KODE` | VARCHAR(20) | PRIMARY KEY - Kode pelanggan |
| `NAMA` | VARCHAR(50) | Nama pelanggan |
| `ALAMAT` | VARCHAR(100) | Alamat pelanggan |
| `JENIS` | VARCHAR(20) | Jenis pelanggan (Umum/Partai) |

#### Tabel: `tbl_karyawan` - Master Karyawan
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `KODE` | VARCHAR(10) | PRIMARY KEY - Kode karyawan |
| `NAMA` | VARCHAR(50) | Nama karyawan |
| `JABATAN` | VARCHAR(50) | Jabatan karyawan |

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

#### Tabel: `tbl_piutang` - Piutang Pelanggan
| Kolom | Tipe | Keterangan |
|-------|------|-----------|
| `ID_PIUTANG` | INT | PRIMARY KEY - ID piutang |
| `ID_PENJUALAN` | VARCHAR(50) | FK ke penjualan.ID_PENJUALAN |
| `SISA_PIUTANG` | DECIMAL(15,0) | Sisa piutang pelanggan |
| `STATUS` | VARCHAR(20) | Status piutang (HUTANG/LUNAS) |
