# ?? DOKUMENTASI IMPLEMENTASI HALAMAN PENJUALAN - AppKasir

**Untuk Developer Dart Flutter**  
**Status:** Complete & Detailed  
**Version:** 1.0.0  
**Last Updated:** Maret 2026

---

## ?? TABLE OF CONTENTS

1. [Ringkasan Sistem](#ringkasan-sistem)
2. [Flow & Alur Transaksi](#flow--alur-transaksi)
3. [Struktur UI](#struktur-ui)
4. [Data Models](#data-models)
5. [Database Operations](#database-operations)
6. [Business Logic & Kalkulasi](#business-logic--kalkulasi)
7. [Input & Barcode Detection](#input--barcode-detection)
8. [Payment Processing](#payment-processing)
9. [Validasi & Error Handling](#validasi--error-handling)
10. [Testing Scenarios](#testing-scenarios)

---

## ?? RINGKASAN SISTEM

### Tujuan Halaman Penjualan
Halaman ini adalah **core transaction engine** untuk mencatat penjualan barang dengan:
- Input manual atau barcode scanning
- Support untuk 2 jenis customer (Umum / Partai)
- Support untuk 2 lokasi stok (Toko / Gudang)
- Multi-unit pricing (3 level satuan per barang)
- Dynamic discount & tax calculation
- Payment modes: Tunai / Transfer Bank
- Hutang/Piutang tracking

### Mode Operasi
| Mode | Deskripsi | Trigger |
|------|-----------|---------|
| **TambahPenjualan** | Transaksi baru | FormPenjualan.Load() |
| **EditPenjualan** | Edit transaksi existing | Mode edit dari history |

---

## ?? FLOW & ALUR TRANSAKSI

### 1?? INITIALIZATION FLOW

```
START FormPenjualan
    ?
[Form_Load]
  - Setup komponen UI (timer, culture, permissions)
  - Load setting dari database (role-based access)
  - Setup grid columns & formatting
  ?
[Form_Shown]
  - Cek TxtJenistransaksi.Text
  - Jika "TambahPenjualan" ? Kondisiawal()
  - Jika "EditPenjualan" ? Editpenjualanheader()
  ?
[Kondisiawal] ? MAIN RESET FUNCTION
  - Clear semua textbox & grid
  - Load customer list (CmbPelanggan)
  - Load sales list (CmbSales)
  - Generate invoice number (Nomorjual)
  - Load bank/payment options
  - Update total calculations
  - Set focus (SearchMode vs EditMode)
  ?
READY FOR INPUT
```

### 2?? ITEM INPUT FLOW

```
User input di TxtNama
    ?
[TxtNama_KeyDown] - Deteksi karakter
    ?
    ?? NUMERIC/ALPHANUMERIC (Barcode candidate?)
    ?  ?? Cek ke BarcodeExistsInDatabase()
    ?  ?? Jika yes ? SearchByBarcode()
    ?  ?? Jika no ? Fallback ke manual search
    ?
    ?? LETTER/*  (Manual entry)
       ?? TriggerManualSearch() ? Show LstBarang
    ?
[ProcessInput] - Parse format input
    ?? qty*satuan*nama ? SetQtyAndSatuan()
    ?? qty*nama       ? SetQtyOnly()
    ?? nama murni     ? SetDefaultQtyAndSatuan()
    ?
[Ambildatalaindaridbbarang] - Fetch dari DB
    ?? Load price tiers berdasarkan jenis customer
    ?? Load stok toko & gudang
    ?? Tentukan satuan/isi berdasarkan barcode atau level
    ?
[TambahDataLangsung] - Insert ke grid
    ?? Cek duplikat (jika Kodebarangsama="Tidak")
    ?? Load satuan combo untuk baris
    ?? Hitung total baris
    ?
ITEM ADDED TO GRID
```

### 3?? GRID EDITING FLOW

```
User edit cell di DgvData
    ?
[DgvData_CellEndEdit]
    ?? Column 1 (NamaBarang) - Edit inline search
    ?? Column 3 (QTY) - Validasi numeric
    ?? Column 7 (Harga) - Update diskon % atau edit master
    ?? Column 9 (DiskonPersen) - Hitung DiskonRp
    ?? Column 10 (DiskonRp) - Hitung DiskonPersen
    ?
[HitungNilaiSetiapBaris] - Per-row calculation
    ?? QtySat = QTY × ISI
    ?? TotalDiskon = QTY × DiskonRp
    ?? TotalHarga = (Harga × QTY) - TotalDiskon
    ?
[UpdateSemuaTotal] - Aggregate calculation
    ?? TotalHargaBeli (HPP) = SUM(HargaBeli × ISI × QTY)
    ?? Subtotal = SUM(TotalHarga per item)
    ?? TotalQtyBarang = SUM(QTY)
    ?? TotalQtySatuan = SUM(QtySat)
    ?? TotalItem = COUNT(rows)
    ?
GRID UPDATED
```

### 4?? DISCOUNT & TAX FLOW

```
User update TxtDiskonPersen atau TxtDiskonRp
    ?
[TxtDiskon_TextChanged]
    ?? Jika DiskonPersen diubah
    ?  ?? DiskonRp = Subtotal × DiskonPersen / 100
    ?? Jika DiskonRp diubah
       ?? DiskonPersen = (DiskonRp / Subtotal) × 100
    ?
[HitungTotalPenjualanAkhir]
    ?? Pajak = (Subtotal - Diskon) × PajakPersen / 100
    ?? TotalAkhir = (Subtotal - Diskon) + Pajak + BiayaKirim
    ?? Update label display
    ?
TOTAL CALCULATED
```

### 5?? PAYMENT FLOW

```
User click BtnBayar (F8)
    ?
[TekanBayar] - Validasi sebelum payment panel
    ?? Cek invoice number (TxtFaktur)
    ?? Cek ada barang (DgvData.Rows.Count > 0)
    ?? Cek total > 0 (jika Nominal0="Tidak")
    ?? Cek jual rugi (jika modulJualRugi="Tidak")
    ?? Cek stok minus (jika modulJualMinus="Tidak")
    ?
Show GBBayar (Payment Panel)
    ?? Set TxtNominalBayar = TxtTotaljualStlPajak (jika Isinominal="Tidak")
    ?? Focus to TxtNominalBayar
    ?? Show bank fields (jika TYPE_AKUN="BANK")
    ?
[TxtNominalBayar_TextChanged]
    ?? Hitung kembali/hutang = Nominal - TotalAkhir
    ?? Jika kembali > 0 ? Label "Kembali:" + status "Lunas"
    ?? Jika hutang > 0 ? Label "Hutang:" + status "Belum Lunas"
    ?
READY FOR SAVE
```

### 6?? SAVE & DATABASE FLOW

```
User click BtnSimpan (F10) or press ENTER in TxtNominalBayar
    ?
[TekanSimpan] - Validasi pembayaran
    ?? Jika BANK type, nominal harus > 0
    ?? Jika belum lunas, pelanggan harus dipilih
    ?? Konfirmasi jika nominal 0 untuk non-bank
    ?
[Simpanatauedit]
    ?? Cek apakah invoice sudah exist
    ?? Jika ya & TransaksiLampau="Tidak" ? update tanggal ke sekarang
    ?? Call Prosessimpan()
    ?
[Prosessimpan] - Database transaction
    ?? BEGIN TRANSACTION
    ?? Hapusuntukedit() [jika mode edit]
    ?? Simpanpenjualan() ? INSERT penjualan header
    ?? Simpanpenjualandetail() ? INSERT penjualan_detail rows
    ?? Simpanjurnal() ? UPDATE/INSERT jurnal akuntansi
    ?? HistoryBarang() ? INSERT tbl_history_barang (stok tracking)
    ?? COMMIT TRANSACTION
    ?
    ?? UPDATE stok barang (HitungByKode)
    ?? Print nota (berdasarkan CmbCetak setting)
    ?? Tampilkan pesan kembalian (jika ada)
    ?? Reset form (Kondisiawal)
    ?
TRANSACTION COMPLETE
```

---

## ?? STRUKTUR UI

### Header Section (GroupBox1)
**Lebar Total:** 1291px, **Tinggi:** 172px

```
???????????????????????????????????????????????????????????????
?  [TITLE] TERIMA KASIH TELAH BELANJA DI [PERUSAHAAN]    [X] ?
???????????????????????????????????????????????????????????????
?                                                               ?
? ?? GroupBox2 (Transaction Header) ??????? ?? GroupBox3 ????
? ? [Faktur] ? [Tgl] ? [Pelanggan] ? [Sales] ? ? Grand Total  ??
? ? Kode: ___ ? Jenis: ___ ? Alamat: ___ ? ? [00000000000]??
? ?                                       ? ?              ??
? ??????????????????????????????????????????? ?????????????????
?                                                               ?
? [Hidden Fields - For Logic Only]                             ?
? TxtKode, TxtQty, Txtsatuan, TxtIsi, TxtHargaBeli, ...        ?
???????????????????????????????????????????????????????????????
```

#### GroupBox2 Details (Transaction Header)
| Element | Type | Default | Purpose |
|---------|------|---------|---------|
| TxtFaktur | TextBox | "PJ-YYMMDD-XXXX" | Invoice number (read-only, auto-generated) |
| DTPTgl | DateTimePicker | DateTime.Now | Transaction date/time |
| CmbPelanggan | ComboBox | (dropdown) | Customer selection |
| LblJenisPl | Label | "Umum" | Customer type display |
| LbLKodePel | Label | "" | Customer code (hidden) |
| LblAlamat | Label | "" | Customer address |
| CmbSales | ComboBox | (dropdown) | Sales person |
| LblSales | Label | "" | Sales code |

#### GroupBox3 Details (Grand Total)
| Element | Type | Style | Purpose |
|---------|------|-------|---------|
| TxtGrantotal | TextBox | Black BG, Lime text, 36pt bold | **Display only** - Grand total display |

### Search Bar Section (PanelCariNama)
**Location:** Below header, **Height:** 30px

```
[SEARCH PANEL - YELLOW HIGHLIGHT WHEN FOCUSED]
????????????????????????????????????????????????????????????
? TxtNama: [_________________________________] [Cari Btn]  ?
?         (BARCODE or NAME INPUT)                          ?
?                                                           ?
? [LstBarang - Dropdown List]                              ?
? ?? Barang 1 => 10 (stok)                                ?
? ?? Barang 2 => 25 (stok)                                ?
? ?? Barang 3 => 5 (stok)                                 ?
?                                                           ?
? [rtbPetunjuk - CONTEXT HELP]                             ?
? Menampilkan hint sesuai kolom yang hovered               ?
????????????????????????????????????????????????????????????
```

### Data Grid Section (DgvData)
**Columns:** 17 columns total

| # | Kolom | Type | Width | Format | Read-Only | Editable Notes |
|---|-------|------|-------|--------|-----------|---|
| 0 | Kode | TextBox | 50px | Text | Yes | Hidden (logic only) |
| 1 | NamaBarang | TextBox | 150px | Text | Yes after set | Can search/edit when empty |
| 2 | HargaBeli | TextBox | 80px | #,0.## | Yes | Display only (from DB) |
| 3 | QTY | TextBox | 50px | #,0.## | No | User input quantity |
| 4 | Satuan | ComboBox | 80px | Dropdown | No | Level satuan (Pcs/Box/Dus) |
| 5 | Isi | TextBox | 40px | #0 | No | Content per unit |
| 6 | TotalHargaBeli | TextBox | 100px | #,0.## | Yes | HargaBeli × Isi × QTY |
| 7 | Harga | TextBox | 80px | #,0.## | No | Unit selling price |
| 8 | QtySat | TextBox | 60px | #,0.## | Yes | QTY × Isi (auto-calc) |
| 9 | DiskonPersen | TextBox | 70px | #0.## | No | Discount percentage |
| 10 | DiskonRp | TextBox | 80px | #,0.## | No | Discount amount |
| 11 | TotalDiskon | TextBox | 90px | #,0.## | Yes | QTY × DiskonRp (auto-calc) |
| 12 | TotalHarga | TextBox | 100px | #,0.## | Yes | (Harga × QTY) - TotalDiskon |
| 13 | StokToko | TextBox | 70px | #,0.## | Yes | From tbl_barang (display only) |
| 14 | StokGudang | TextBox | 70px | #,0.## | Yes | From tbl_barang (display only) |
| 15 | Stok | TextBox | 60px | #,0.## | Yes | Active stok (toko/gudang) |
| 16 | SerialNumber | TextBox | 80px | Text | No | Optional serial # (if ChkTampilSN=True) |

**Grid Features:**
- Row number display (auto numbering di row header)
- Red highlight untuk stok < 1
- Auto-scroll ke baris terakhir
- Right-click context menu (Hapus, Hitung Ulang)

### Discount & Tax Section (Panel1)
**Location:** Below grid, **Height:** 80px

```
?? DISKON ???????????????????????????????????????????????????
? [TxtDiskonPersen: 0 %] ? "Rp. [LblDiskonRp]"             ?
? [TxtDiskonRp: 0]                                           ?
? ????????????????????????????????????????????????????????? ?
? ? [TxtTotalJualSblDiskonPajak] = Subtotal              ? ?
? ????????????????????????????????????????????????????????? ?
?????????????????????????????????????????????????????????????

?? PAJAK ????????????????????????????????????????????????????
? [TxtPajakPersen: 11 %] ? "Rp. [LblPajakRp]"             ?
? [TxtPajakRp: 0]                                            ?
? ????????????????????????????????????????????????????????? ?
? ? [LblTotalStlPajak] = Final Total                      ? ?
? ????????????????????????????????????????????????????????? ?
?????????????????????????????????????????????????????????????

?? SHIPPING ?????????????????????????????????????????????????
? [TxtBiayaKirim: 0] ? "Rp. [LblBiayaKirim]"              ?
? [TxtTotaljualStlPajak] - Final total (hidden)             ?
?????????????????????????????????????????????????????????????
```

### Payment Panel (GBBayar)
**Visible:** Only when BtnBayar clicked  
**Size:** 933×280px (BANK) / 529×280px (TUNAI)  
**Position:** Centered on form

```
??????????????????????????????????????????????
?        PROSES PEMBAYARAN TRANSAKSI         ?
??????????????????????????????????????????????
?                                            ?
? Jenis Bayar: [CmbJenisBayar] (BANK/TUNAI) ?
?                                            ?
? ?? JIKA BANK (Conditional Panel) ????????? ?
? ? Bank: [TxtBank]                         ? ?
? ? No. Rekening: [TxtNoRek]                ? ?
? ? Nama Rekening: [TxtNamaRek]             ? ?
? ? No. Referensi: [TxtNoReff]              ? ?
? ??????????????????????????????????????????? ?
?                                            ?
? Total: [LblTotalStlPajak] (Rp. _______)   ?
? Nominal Bayar: [TxtNominalBayar]          ?
? [LblBayar text: "0"]                      ?
?                                            ?
? Kembali: [LblKembali] (Rp. _______)       ?
? Status: [LblStatusTrans] (Lunas/BelumLunas)?
? Jatuh Tempo: [DTPJatuhTempo] (if hutang) ?
?                                            ?
? [BtnSimpan] [BtnBatal]                    ?
?                                            ?
??????????????????????????????????????????????
```

### Action Buttons Section (Bottom)
**Row 1:**
- BtnBayar (F8) - Light blue - Show payment panel
- BtnTahan (F6) - Orange - Hold transaction
- BtnPanggil (F7) - Green - Recall held transaction

**Row 2:**
- BtnBarang (F4) - Gray - Open product master
- BtnPelanggan (F12) - Gray - Open customer master
- CmbCetak - Dropdown (Iya/Tidak/Tanya) - Printing option

**Row 3 (In GBBayar):**
- BtnSimpann (F10) - Green - Save transaction
- BtnBatal (F11) - Red - Cancel payment

---

## ?? DATA MODELS

### Model: TransaksiPenjualan (Header)
```dart
class TransaksiPenjualan {
  String idPenjualan;          // PK: PJ-YYMMDD-XXXX
  String fakturJual;           // UNIQUE invoice number
  DateTime tanggalPenjualan;   // Transaction date/time
  String idPelanggan;          // FK to tbl_pelanggan
  String namaPelanggan;        // Customer name snapshot
  String jenisPelanggan;       // "Umum" or "Partai"
  String idSales;              // FK to tbl_karyawan
  String namaSales;            // Sales name snapshot
  
  // Location & Source
  String lokasiBarang;         // "TOKO" or "GUDANG"
  
  // Totals (BEFORE discount)
  Decimal totalSebelumDiskon;  // SUM(item.totalHarga)
  
  // Discounts
  Decimal diskonPersen;        // Discount %
  Decimal diskonRp;            // Discount amount
  
  // Tax (on total after discount)
  Decimal pajakPersen;         // Tax %
  Decimal pajakRp;             // Tax amount
  
  // Final totals
  Decimal totalSetelahPajak;   // = totalSebelumDiskon - diskonRp + pajakRp
  Decimal biayaKirim;          // Shipping fee
  Decimal grandTotal;          // = totalSetelahPajak + biayaKirim
  
  // HPP (Cost of Goods Sold)
  Decimal totalHpp;            // SUM(item.totalHargaBeli)
  Decimal profit;              // grandTotal - totalHpp - diskonRp
  
  // Payment
  Decimal nominalBayar;        // Amount paid
  Decimal sisaHutang;          // Remaining debt
  Decimal kembalian;           // Change
  String statusBayar;          // "TERBAYAR" or "TERHUTANG"
  
  // Payment details
  String typeAkun;             // "TUNAI" or "BANK" or "PIUTANG"
  String jenisPembayaran;      // Method name (e.g., "Transfer BCA")
  String? namaAkun;            // Bank account name (optional)
  String? kodeAkun;            // Account code (optional)
  String? nomorRekening;       // Account number (optional)
  String? namaRekening;        // Account holder name (optional)
  String? nomorReferensi;      // Reference number (optional)
  DateTime? tanggalJatuhTempo;  // Due date (if hutang)
  
  // Audit
  String idUser;               // User who created
  String idKomputer;           // Computer/Terminal ID
  DateTime createdAt;          // Record creation time
  DateTime updatedAt;          // Last modification time
}
```

### Model: DetailPenjualan (Line Items)
```dart
class DetailPenjualan {
  int idDetail;                // PK: AUTO_INCREMENT
  String fakturJual;           // FK to penjualan
  String idBarang;             // FK to tbl_barang
  String namaBarang;           // Product name snapshot
  
  // Quantity & Unit
  Decimal qty;                 // Quantity ordered
  String satuan;               // Unit selected (Pcs, Box, Dus)
  int isiSatuan;               // Content per unit (1, 12, 100)
  Decimal qtySatuan;           // qty * isiSatuan (base unit qty)
  
  // Pricing
  Decimal hargaBeli;           // Cost price per unit
  Decimal totalHargaBeli;      // hargaBeli * isiSatuan * qty
  Decimal hargaJual;           // Selling price per unit
  
  // Discount (per line item)
  Decimal diskonPersen;        // Discount %
  Decimal diskonRp;            // Discount amount per unit
  Decimal totalDiskon;         // qty * diskonRp (total line discount)
  
  // Line total
  Decimal totalHarga;          // (hargaJual * qty) - totalDiskon
  
  // Stock info at transaction time
  Decimal stokToko;            // Store stock available
  Decimal stokGudang;          // Warehouse stock available
  
  // Optional
  String? serialNumber;        // Serial # (if applicable)
  
  // Audit
  DateTime createdAt;
  DateTime updatedAt;
}
```

### Model: Barang (Product Master)
```dart
class Barang {
  String idBarang;             // PK
  String namaBarang;           // Product name
  String? deskripsi;           // Description
  
  // Barcodes (3 levels)
  String? barcodeKecil;        // Level 1 barcode
  String? barcodeSedang;       // Level 2 barcode
  String? barcodeBesar;        // Level 3 barcode
  
  // UMUM (General) - 3 levels
  String satuanUmumKecil;      // Pcs
  int isiUmumKecil;            // 1
  Decimal hargaJualUmumKecil;  // Unit price
  
  String satuanUmumSedang;     // Box
  int isiUmumSedang;           // 12
  Decimal hargaJualUmumSedang; // 12 pcs price
  
  String satuanUmumBesar;      // Dus
  int isiUmumBesar;            // 100
  Decimal hargaJualUmumBesar;  // 100 pcs price
  
  // PARTAI (Wholesale) - 3 levels
  String satuanPartaiKecil;    // Pcs
  int isiPartaiKecil;          // 1
  Decimal hargaJualPartaiKecil;
  
  String satuanPartaiSedang;   // Box
  int isiPartaiSedang;         // 12
  Decimal hargaJualPartaiSedang;
  
  String satuanPartaiBesar;    // Dus
  int isiPartaiBesar;          // 100
  Decimal hargaJualPartaiBesar;
  
  // Cost
  Decimal hargaBeli;           // Cost price from supplier
  
  // Stock
  Decimal stokToko;            // Store inventory
  Decimal stokGudang;          // Warehouse inventory
  
  // Audit
  DateTime createdAt;
  DateTime updatedAt;
}
```

### Model: Pelanggan (Customer)
```dart
class Pelanggan {
  String kode;                 // PK
  String nama;                 // Customer name
  String? alamat;              // Address
  String jenis;                // "Umum" or "Partai"
  int jangkaPiutang;           // Credit period in days (default: 30)
  String? telepon;             // Phone
  String? email;               // Email
  
  // Credit info
  Decimal limitPiutang;        // Credit limit
  Decimal piutangSementara;    // Current debt
  
  DateTime createdAt;
  DateTime updatedAt;
}
```

### Model: Karyawan (Employee/Sales)
```dart
class Karyawan {
  String kode;                 // PK
  String nama;                 // Employee name
  String? telepon;             // Phone
  String? email;               // Email
  
  DateTime createdAt;
  DateTime updatedAt;
}
```

---

## ??? DATABASE OPERATIONS

### 1. Ambil Master Data

#### Get Pelanggan List
```sql
SELECT NAMA FROM tbl_pelanggan ORDER BY NAMA ASC
```
**Usage:** Load CmbPelanggan

#### Get Pelanggan Info
```sql
SELECT KODE, ALAMAT, JENIS, JangkaPiutang 
FROM tbl_pelanggan 
WHERE NAMA = ?
```
**Usage:** Populate jenis, kode, alamat when customer selected

#### Get Karyawan List
```sql
SELECT Kode, Nama FROM tbl_karyawan ORDER BY Nama ASC
```
**Usage:** Load CmbSales

#### Get Karyawan Info
```sql
SELECT Kode FROM tbl_karyawan WHERE Nama = ?
```
**Usage:** Populate sales code

#### Get Bank/Payment List
```sql
SELECT Type_Akun, Kode_Akun, Nama_Akun 
FROM tbl_datareferensi 
WHERE Type_Akun IN ('BANK', 'TUNAI')
```
**Usage:** Load CmbJenisBayar

---

### 2. Barcode & Product Search

#### Search by Barcode
```sql
SELECT NAMA_BARANG 
FROM tbl_barang 
WHERE BARCODE_KECIL = ? 
   OR BARCODE_SEDANG = ? 
   OR BARCODE_BESAR = ? 
LIMIT 1
```
**Usage:** SearchByBarcode() - EXACT MATCH only

#### Check Barcode Exists
```sql
SELECT 1 FROM tbl_barang 
WHERE BARCODE_KECIL = ? 
   OR BARCODE_SEDANG = ? 
   OR BARCODE_BESAR = ? 
LIMIT 1
```
**Usage:** IsBarcodeCandidate() - for detection

#### Search Product by Name/Code/Barcode
```sql
SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG 
FROM tbl_barang 
WHERE TRIM(ID_BARANG) LIKE ? 
   OR TRIM(NAMA_BARANG) LIKE ? 
   OR TRIM(BARCODE_KECIL) LIKE ? 
   OR TRIM(BARCODE_SEDANG) LIKE ? 
   OR TRIM(BARCODE_BESAR) LIKE ? 
ORDER BY NAMA_BARANG 
LIMIT 20
```
**Usage:** ProcessManualSearchList() - LIKE search with wildcards

---

### 3. Product Detail Fetch (Complex)

#### Get Full Product Data
```sql
SELECT 
  ID_BARANG, NAMA_BARANG, HARGA_BELI, 
  BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR,
  SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR,
  ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR,
  HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR,
  SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR,
  ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR,
  HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR,
  STOK_TOKO, STOK_GUDANG
FROM tbl_barang 
WHERE TRIM(NAMA_BARANG) = ? 
   OR BARCODE_KECIL = ? 
   OR BARCODE_SEDANG = ? 
   OR BARCODE_BESAR = ? 
LIMIT 1
```
**Usage:** Ambildatalaindaridbbarang() - Load all pricing tiers

**Logic untuk menentukan satuan/isi/harga:**
```
Untuk setiap baris yang ditambah:
  1. Cek jenis pelanggan (Umum vs Partai)
  2. Cek TxtLevelSat (1/2/3 level satuan)
  3. Cek TxtBarcode (jika scan, match dengan barcode level)
  4. Pilih satuan/isi/harga sesuai level
  5. Loop dari level 3?2?1 untuk fallback jika level kosong
```

---

### 4. Generate Invoice Number

#### Get Max Invoice for Today
```sql
SELECT MAX(ID_PENJUALAN) 
FROM penjualan 
WHERE ID_PENJUALAN LIKE 'PJ-YYMMDD%'
```

#### Get Max Invoice from Held Transactions
```sql
SELECT MAX(FAKTUR_JUAL) 
FROM penjualan_ditahan 
WHERE FAKTUR_JUAL LIKE 'PJ-YYMMDD%'
```

**Logic:**
```
cekTanggal = DTPTgl.Value.ToString("yyMMdd")  // e.g., "260304"
ceknomor = "PJ-" & cekTanggal                 // "PJ-260304"

1. Query penjualan table untuk max ID dengan prefix ceknomor
2. Query penjualan_ditahan table untuk max ID dengan prefix ceknomor
3. Ambil yang paling besar dari keduanya
4. Increment counter (last 4 digits)
5. Jika tidak ada, mulai dari 0001

Result: "PJ-260304-0001", "PJ-260304-0002", ...
```

---

### 5. Save Transaction (MAIN - dengan Transaction)

```sql
-- BEGIN TRANSACTION

-- 1. INSERT penjualan (header)
INSERT INTO penjualan (
  ID_PENJUALAN, FAKTUR_JUAL, TANGGAL_PENJUALAN, ID_PELANGGAN,
  NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG,
  GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_RP, PAJAK_RP,
  GRAND_TOTAL_STL_PAJAK, TOTAL_HPP, LABA,
  BAYAR, KEMBALI, SISA_TAGIHAN, STATUS_BAYAR,
  TYPE_AKUN, METODE, BANK, NO_REKENING, NAMA_REKENING,
  ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER,
  CREATED_AT, UPDATED_AT
) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NOW(), NOW())

-- 2. INSERT penjualan_detail (line items) - LOOP untuk setiap row
INSERT INTO penjualan_detail (
  FAKTUR_JUAL, ID_BARANG, QTY, SATUAN, ISI_SATUAN, 
  HARGA_JUAL, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA,
  CREATED_AT, UPDATED_AT
) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NOW(), NOW())

-- 3. INSERT tbl_history_barang (stock audit trail)
INSERT INTO tbl_history_barang (
  ID_BARANG, TIPE_TRANSAKSI, QTY, STOK_SEBELUM, STOK_SESUDAH, 
  CREATED_AT
) VALUES (?, 'PENJUALAN', ?, ?, ?, NOW())

-- 4. UPDATE tbl_barang (reduce stock)
UPDATE tbl_barang 
SET STOK_TOKO = STOK_TOKO - ? 
WHERE ID_BARANG = ?
-- (or STOK_GUDANG depending on location)

-- 5. INSERT jurnal (accounting entries)
-- [DEBIT] Kas/Bank account
-- [KREDIT] Sales account
-- [DEBIT] COGS account
-- [KREDIT] Inventory account

-- COMMIT TRANSACTION
```

---

### 6. Check Stok Minus

```sql
-- Per barang di grid:
SELECT STOK_TOKO, STOK_GUDANG 
FROM tbl_barang 
WHERE ID_BARANG = ?

-- Calculate available after transaction:
IF location = "TOKO":
  availableStok = STOK_TOKO
ELSE:
  availableStok = STOK_GUDANG

-- Compare with order quantity:
IF qtySatuan > availableStok AND modulJualMinus = "Tidak":
  ERROR: "Stok tidak cukup"
  HIGHLIGHT: Red row
ELSE:
  ALLOW: Lanjut ke payment
```

---

### 7. Check Jual Rugi

```sql
-- Per barang di grid:
SELECT HARGA_BELI 
FROM tbl_barang 
WHERE ID_BARANG = ?

-- Compare:
IF hargaJual < hargaBeli AND modulJualRugi = "Tidak":
  ERROR: "Harga jual rugi untuk [barang]"
  HIGHLIGHT: Red row
ELSE:
  ALLOW: Lanjut ke payment
```

---

## ?? BUSINESS LOGIC & KALKULASI

### Formula 1: Quantity in Base Units
```
QtySat = QTY × ISI_SATUAN

Example: 
  - User input QTY = 2, satuan = "Box" (ISI = 12)
  - QtySat = 2 × 12 = 24 (satuan dasar)
```

### Formula 2: Cost of Goods Sold (COGS) per Item
```
TotalHargaBeli = HARGA_BELI × ISI_SATUAN × QTY

Example:
  - HARGA_BELI = 2.500
  - ISI_SATUAN = 12 (box)
  - QTY = 2
  - TotalHargaBeli = 2.500 × 12 × 2 = 60.000
```

### Formula 3: Item Discount (Amount)
```
TotalDiskon = QTY × DISKON_RP

Where DISKON_RP is per unit (or calculated from %):
  DISKON_RP = HARGA_JUAL × DISKON_PERSEN / 100

Example:
  - HARGA_JUAL = 50.000
  - DISKON_PERSEN = 10%
  - DISKON_RP = 50.000 × 10 / 100 = 5.000 (per unit)
  - QTY = 2
  - TotalDiskon = 2 × 5.000 = 10.000
```

### Formula 4: Item Total (Line Total)
```
TotalHarga = (HARGA_JUAL × QTY) - TotalDiskon

Example:
  - HARGA_JUAL = 50.000, QTY = 2
  - (50.000 × 2) - 10.000 = 90.000
```

### Formula 5: Subtotal (All Items)
```
Subtotal = SUM(all_items.TotalHarga)

This is the total BEFORE any transaction-level discount
```

### Formula 6: Transaction-Level Discount
```
DISKON_TOTAL_RP = Subtotal × DISKON_PERSEN / 100

Example:
  - Subtotal = 425.000
  - DISKON_PERSEN = 10%
  - DISKON_TOTAL_RP = 425.000 × 10 / 100 = 42.500
```

### Formula 7: Tax (PPN)
```
PAJAK_RP = (Subtotal - DISKON_TOTAL_RP) × PAJAK_PERSEN / 100

?? CRITICAL: Tax is calculated on (Subtotal - Discount), NOT on subtotal alone

Example:
  - Subtotal = 425.000
  - DISKON_TOTAL_RP = 42.500
  - Base for tax = 425.000 - 42.500 = 382.500
  - PAJAK_PERSEN = 11% (standard PPN)
  - PAJAK_RP = 382.500 × 11 / 100 = 42.075
```

### Formula 8: Final Total
```
GRAND_TOTAL_STL_PAJAK = Subtotal - DISKON_TOTAL_RP + PAJAK_RP + BIAYA_KIRIM

Example:
  - Subtotal = 425.000
  - DISKON_TOTAL_RP = 42.500
  - PAJAK_RP = 42.075
  - BIAYA_KIRIM = 10.000 (optional)
  - GRAND_TOTAL_STL_PAJAK = 425.000 - 42.500 + 42.075 + 10.000 = 434.575
```

### Formula 9: Profit Calculation
```
LABA = (GRAND_TOTAL_STL_PAJAK - TOTAL_HPP) - DISKON_TOTAL_RP

Where:
  - GRAND_TOTAL_STL_PAJAK = Actual revenue received
  - TOTAL_HPP = SUM(all items.TotalHargaBeli)
  - DISKON_TOTAL_RP = Transaction discount given

Example:
  - Revenue = 434.575
  - COGS = 200.000 (sum of all HargaBeli × Isi × Qty)
  - Discount = 42.500
  - LABA = (434.575 - 200.000) - 42.500 = 192.075
```

### Formula 10: Payment Balance
```
SISA_HUTANG = GRAND_TOTAL_STL_PAJAK - NOMINAL_BAYAR

If SISA_HUTANG > 0:
  - Status = "BELUM LUNAS" (Hutang)
  - KEMBALI = 0
  
If SISA_HUTANG < 0:
  - Status = "LUNAS" (Paid)
  - KEMBALI = ABS(SISA_HUTANG)
  - SISA_HUTANG = 0
  
If SISA_HUTANG = 0:
  - Status = "LUNAS" (Exact payment)
  - KEMBALI = 0
```

### Key Validation Rules

**Rule 1: Stok Tidak Boleh Minus**
```
IF modulJualMinus = "Tidak":
  FOR each item in grid:
    IF item.QtySat > availableStok:
      REJECT: "Stok tidak cukup"
      HIGHLIGHT: Red
```

**Rule 2: Tidak Boleh Jual Rugi**
```
IF modulJualRugi = "Tidak":
  FOR each item in grid:
    IF item.HargaJual < item.HargaBeli:
      REJECT: "Harga jual rugi"
      HIGHLIGHT: Red
```

**Rule 3: Pembayaran Hutang Wajib Customer**
```
IF status = "BELUM LUNAS":
  IF customer NOT SELECTED:
    REJECT: "Pelanggan harus dipilih untuk transaksi hutang"
```

**Rule 4: Pembayaran Bank Wajib Nominal > 0**
```
IF typeAkun = "BANK":
  IF nominal = 0:
    REJECT: "Pembayaran BANK harus nominal > 0"
```

---

## ?? INPUT & BARCODE DETECTION

### Barcode Detection Algorithm

**Constants:**
```
BARCODE_CHAR_INTERVAL_MS = 30    // Max interval between chars for barcode
BARCODE_TOTAL_TIME_MS = 200      // Max total time for barcode scan
BARCODE_MIN_LENGTH = 4           // Minimum barcode length
BARCODE_MAX_LENGTH = 100         // Maximum barcode length
```

**Logic Flow:**

```
[User types in TxtNama]
    ?
[TxtNama_KeyDown] - Character detection
    ?? START: Initialize buffer, start timer
    ?
    ?? NEXT CHAR: Check interval from last char
    ?  ?? If interval > 30ms ? MANUAL input (not barcode)
    ?  ?? If interval ? 30ms ? Possible barcode
    ?
    ?? ENTER KEY: Process input
    ?  ?? Calculate total time
    ?  ?? If totalTime ? 200ms AND length ? 4 ? BARCODE
    ?  ?? If totalTime > 200ms ? MANUAL
    ?
    ?? TIMER TICK (100ms): Auto-process if no input
       ?? If elapsed > 100ms since last char ? Process buffer

[ProcessInput] - Format parsing
    ?? qty*satuan*nama  ? 2 asterisks
    ?? qty*nama         ? 1 asterisk
    ?? barcode/nama     ? 0 asterisks
    
[SearchByBarcode vs TriggerManualSearch]
    ?? Barcode ? EXACT MATCH only in DB
    ?? Manual ? LIKE search + show list
```

**Input Format Support:**

| Format | Example | Hasil |
|--------|---------|-------|
| Barcode scan | `8991234567890` | SearchByBarcode() ? Direct add if found |
| Name search | `Sabun` | TriggerManualSearch() ? Show list |
| Qty × Name | `2*Sabun` | Qty=2, search Sabun |
| Qty × Level × Name | `3*2*Minyak` | Qty=3, Level=2, search Minyak |
| Qty × Barcode | `2*8991234567890` | Qty=2, scan barcode |

**Manual Search Result:**
```
LstBarang (Dropdown list):
?? Barang 1 => 10 (stock di lokasi aktif)
?? Barang 2 => 25
?? Barang 3 => 5
```
User dapat:
- ? Arrow untuk navigate
- ? Enter untuk select
- Click item untuk select

---

## ?? PAYMENT PROCESSING

### Payment Flow Diagram

```
[User click BtnBayar]
    ?
[TekanBayar] - Validasi pra-payment
    ?? ? Invoice number ada
    ?? ? Ada barang di grid
    ?? ? Total > 0 (jika nominal0="Tidak")
    ?? ? Stock cukup (jika modulJualMinus="Tidak")
    ?? ? Harga tidak rugi (jika modulJualRugi="Tidak")
    ?
Show GBBayar Panel
    ?? Load payment methods (CmbJenisBayar)
    ?? Set default nominal (jika Isinominal="Iya")
    ?? Focus to TxtNominalBayar
    ?
[AmbiuldataRekening] - Get payment method info
    ?? Cek type akun (BANK vs TUNAI)
    ?? Show/hide bank fields berdasarkan type
    ?
User input TxtNominalBayar
    ?
[TxtNominalBayar_TextChanged] - Calculate balance
    ?? Hitung: Nominal - Total
    ?? Jika balance < 0 ? Kembali (LUNAS)
    ?? Jika balance > 0 ? Hutang (BELUM LUNAS)
    ?? Update: LblStatusTrans, LblPembayaran
    ?? Update: LblKembali (amount)
    ?
User click BtnSimpan or press ENTER
    ?
[TekanSimpan] - Final payment validation
    ?? ? Jika BANK type, nominal harus > 0
    ?? ? Jika BELUM LUNAS, customer harus dipilih
    ?? ? Konfirmasi jika nominal 0 untuk non-bank
    ?? NEXT: Simpanatauedit()
```

### Payment Status Logic

```
IF nominalBayar >= totalAkhir:
  statusBayar = "TERBAYAR" (LUNAS)
  kembalian = nominalBayar - totalAkhir
  sisaHutang = 0
  HIDE: DTPJatuhTempo
  
ELSE:
  statusBayar = "BELUM TERBAYAR" (BELUM LUNAS / HUTANG)
  kembalian = 0
  sisaHutang = totalAkhir - nominalBayar
  SHOW: DTPJatuhTempo
  Set DTPJatuhTempo = DTPTgl + jangkaPiutang (dari customer)
```

### Bank Payment Additional Fields

**Jika type akun = BANK:**
```
TxtBank: Nama bank pengirim (e.g., "BCA", "Mandiri")
TxtNoRek: Nomor rekening pengirim
TxtNamaRek: Nama pemilik rekening
TxtNoReff: Nomor referensi/bukti transfer
```

**Saved to penjualan table:**
```
METODE = "Transfer"
BANK = [TxtBank]
NO_REKENING = [TxtNoRek]
NAMA_REKENING = [TxtNamaRek]
NO_REFF = [TxtNoReff]
```

---

## ? VALIDASI & ERROR HANDLING

### Pre-Payment Validations (TekanBayar)

| # | Validasi | Error Message | Aksi |
|---|----------|---------------|------|
| 1 | Invoice number exist | "Nomor faktur wajib diisi!" | Focus TxtFaktur, EXIT |
| 2 | Ada barang di grid | "Belum ada barang yang dimasukkan!" | EXIT |
| 3 | Total > 0 (jika Nominal0="Tidak") | "Total penjualan belum terisi." | EXIT |
| 4 | Stock cukup (jika modulJualMinus="Tidak") | "Stok [barang] tidak mencukupi. Total Terjual: X, Total Stok: Y" | Highlight RED, Focus row, EXIT |
| 5 | Harga ? cost (jika modulJualRugi="Tidak") | "Barang: [nama]. Harga beli: X, Harga jual: Y" | Highlight RED, Focus row, EXIT |

### Payment Validations (TekanSimpan)

| # | Validasi | Error Message | Aksi |
|---|----------|---------------|------|
| 1 | Bank transfer harus nominal > 0 | "Jika pembayaran melalui BANK, nominal harus > 0" | Focus TxtNominalBayar, EXIT |
| 2 | Hutang wajib customer | "Jika pembayaran belum lunas, pelanggan harus dipilih." | Open CmbPelanggan dropdown, EXIT |
| 3 | Nominal 0 non-bank (confirm) | "Pembayaran [metode] tanpa nominal. Transaksi akan BELUM LUNAS. Lanjutkan?" | If Cancel ? EXIT |

### Grid Cell Validations (DgvData_CellEndEdit)

| Column | Validasi | Action |
|--------|----------|--------|
| NamaBarang | Cek duplikat (jika Kodebarangsama="Tidak") | "Barang sudah ada dalam daftar!" ? Revert |
| NamaBarang | Cek exist di DB | "Barang tidak ditemukan!" ? Clear, Focus TxtNama |
| QTY | Format: angka saja, 1 koma/titik | "Qty hanya boleh angka dan satu koma/titik!" ? Set default "1" |
| Harga | Format: decimal | "Harga harus berupa angka!" ? Set "0" |
| DiskonPersen | Format: 0-100 | Otomatis calc DiskonRp |
| DiskonRp | Format: numeric | Otomatis calc DiskonPersen |

### Database Error Handling

```dart
// Transaction-level error handling
TRY:
  BEGIN TRANSACTION
    - Simpanpenjualan()
    - Simpanpenjualandetail()
    - Simpanjurnal()
    - HistoryBarang()
  COMMIT
CATCH Exception:
  ROLLBACK
  MessageBox.Show(
    "Oh tidak! Transaksi penjualan dibatalkan." +
    "Detail: {ex.Message}",
    "Oops! Ada masalah simpan penjualan",
    ERROR
  )
  EXIT
FINALLY:
  Restore cursor to normal
  Close form (jika mode edit)
```

### Red Highlighting Rules

**When to highlight row RED:**
1. Stock tidak cukup (CekStok)
2. Harga jual rugi (Cekjualrugi)
3. Data error di grid (DgvData_DataError)

**How to restore:**
- Automatically restored when row is fixed
- Or when moving to another row

---

## ?? TESTING SCENARIOS

### Test Case 1: Basic Penjualan (Umum, TOKO)

**Setup:**
- Customer: "Budi" (Umum)
- Sales: "Adi Suryadi"
- Location: TOKO
- Payment: TUNAI

**Steps:**
1. Form load ? Kondisiawal() ? Grid kosong, ready input
2. Input "2*Sabun" ? TambahDataLangsung() ? 1 row added
3. Qty=2, Satuan="Pcs", Harga=10.000, Total=20.000
4. Update total ? Subtotal=20.000, Final=20.000
5. Click BtnBayar ? Show payment panel
6. Input Nominal=20.000 ? Status=LUNAS, Kembali=0
7. Click BtnSimpan ? Simpan ke DB, Reset form

**Expected:**
- Transaksi saved dengan STATUS_BAYAR="TERBAYAR"
- Stok toko berkurang 2 unit
- Profit = 20.000 - HPP

---

### Test Case 2: Penjualan dengan Diskon & Pajak (Partai, GUDANG)

**Setup:**
- Customer: "PT ABC" (Partai)
- Sales: "Rini"
- Location: GUDANG
- Payment: BANK (Transfer)

**Steps:**
1. Select customer PT ABC ? LblJenisPl="Partai"
2. Input "1*3*Minyak" ? Qty=1, Level=3 (Dus), search Minyak
3. Select Minyak ? TambahDataLangsung() ? Harga=50.000, Isi=100
4. QtySat=100, Total=5.000.000
5. Input DiskonPersen=10% ? DiskonRp=500.000
6. Input PajakPersen=11% ? PajakRp=495.000
7. Final=(5.000.000-500.000+495.000)=4.995.000
8. Click BtnBayar ? Show bank fields
9. Input:
   - Nominal=4.000.000
   - Bank=BCA, NoRek=123456, NamaRek=PT Xyz
10. Status=BELUM LUNAS, Hutang=995.000
11. DTPJatuhTempo auto-set ke +30 hari
12. Click BtnSimpan

**Expected:**
- Transaksi saved dengan STATUS_BAYAR="BELUM TERBAYAR"
- SISA_HUTANG=995.000
- BAYAR=4.000.000
- BANK=BCA, NO_REKENING=123456
- Stok gudang berkurang 100 unit
- tbl_piutang insert untuk hutang

---

### Test Case 3: Stock Minus Validation

**Setup:**
- modulJualMinus="Tidak"
- Item dengan stok=5, order qty=10

**Steps:**
1. Add item dengan stok=5, input qty=10
2. QtySat=10
3. Click BtnBayar
4. CekStok() runs ? stok < qty
5. REJECT ? MessageBox "Stok tidak cukup"
6. Row highlighted RED
7. Can't proceed to payment

**Expected:**
- Form tidak dimulai payment
- Focus kembali ke grid
- User dapat edit qty atau hapus item

---

### Test Case 4: Harga Rugi Validation

**Setup:**
- modulJualRugi="Tidak"
- Item: HargaBeli=10.000, HargaJual=9.000

**Steps:**
1. Add item dengan harga jual < harga beli
2. Click BtnBayar
3. Cekjualrugi() runs ? Hargajual < Hargabeli
4. REJECT ? MessageBox "Harga jual rugi"
5. Row highlighted RED

**Expected:**
- Form tidak dimulai payment
- Focus kembali ke grid
- User dapat update harga atau hapus item

---

### Test Case 5: Barcode Scan (Rapid Input)

**Setup:**
- Barcode exists di DB: "8991234567890" = "Sabun Cuci"

**Steps:**
1. TxtNama focus
2. Scanner sends: `8` ? buffer=[8], start timer
3. Scanner sends rapid chars: `9,9,1,...,0` (total time=150ms)
4. Total length=13, totalTime=150ms
5. On ENTER: ProcessInput() detects barcode
6. SearchByBarcode() ? FOUND
7. TambahDataLangsung() ? Add to grid
8. TxtNama cleared, ready for next item

**Expected:**
- Item added immediately tanpa show list
- Fast input (2-3 detik per item)
- Qty default=1

---

### Test Case 6: Manual Search (Slow Input)

**Setup:**
- User types manually: "Sabun"

**Steps:**
1. TxtNama focus
2. User types "S" ? 500ms
3. User types "a" ? 1000ms
4. TxtNama_TextChanged detects letter
5. TriggerManualSearch("S...")
6. Show LstBarang with matching items
7. User press ? to navigate
8. User press ? ENTER to select
9. AmbilDataDariListBox() ? TambahDataLangsung()

**Expected:**
- List shown with 1-20 results
- User dapat pilih dengan keyboard/mouse
- Item added dengan data dari DB

---

### Test Case 7: Format qty*satuan*nama

**Setup:**
- Input: "3*2*Minyak" (Qty=3, Level satuan=2, nama=Minyak)

**Steps:**
1. User types "3*2*Minyak"
2. ProcessInput() detects asterisk count=2
3. Parse: qty="3", level="2", name="Minyak"
4. SetQtyAndSatuan(3, "2")
5. ProcessManualSearchList("Minyak")
6. LstBarang shows matching items
7. Select or ENTER ? TambahDataLangsung()
8. TxtLevelSat="2" used to pick satuan level

**Expected:**
- Qty=3, Satuan=Level2 (e.g., "Box" for Umum/Partai)
- Isi & Harga loaded sesuai level
- Total calculated correctly

---

### Test Case 8: Edit Mode (From History)

**Setup:**
- Open existing penjualan transaction (status: not yet printed)

**Steps:**
1. Form_Penjualan.Load() ? Form_Penjualan_Shown()
2. TxtJenistransaksi.Text="EditPenjualan"
3. Editpenjualanheader() runs
4. Load transaksi dari DB
5. Populate grid dengan penjualan_detail rows
6. User dapat edit qty, harga, diskon
7. Click BtnSimpan
8. Prosessimpan() with edit mode
9. Hapusuntukedit() removes old detail rows
10. Insert new detail rows
11. Update stok

**Expected:**
- Transaksi dapat di-edit (jika belum di-cetak)
- Stok adjustment otomatis
- Transaksi status updated

---

### Test Case 9: Held Transaction (Tahan)

**Setup:**
- Mid-transaction, barang sudah input

**Steps:**
1. User click BtnTahan (F6)
2. Tekantahan() runs
3. Validate data exist
4. INSERT into penjualan_ditahan (header)
5. INSERT into penjualan_ditahan_detail (rows)
6. TxtTahan.Text updated dengan count
7. Form cleared untuk transaksi baru
8. Later: User click BtnPanggil (F7)
9. Tekanpanggil() shows FormPenjualanDitahan
10. Select transaksi ? Restore ke grid
11. AmbilDataDitahan() populates grid
12. Continue input/edit/save

**Expected:**
- Transaksi di-hold di penjualan_ditahan table
- Dapat di-recall & continue editing
- Dapat di-delete tanpa saving

---

### Test Case 10: Change Customer Type Mid-Transaction

**Setup:**
- Start with Umum, change to Partai after item added

**Steps:**
1. Select customer "Budi" (Umum)
2. Add item Sabun dengan satuan "Pcs"
3. Change customer to "PT XYZ" (Partai)
4. LblJenisPl changed to "Partai"
5. LblJenisPl_TextChanged ? UpdateHargaBerdasarJenisPelanggan()
6. Setiap item di grid di-update:
   - Harga ? ambil dari partai tier
   - Satuan combo ? refresh dengan satuan partai
7. Grid recalculated

**Expected:**
- Harga semua item updated ke partai tier
- Satuan options changed
- Total recalculated instantly

---

## ?? END-TO-END TRANSACTION EXAMPLE

### Scenario: Penjualan Umum dengan Diskon (Real World)

**Tanggal:** 4 Maret 2026, 14:30  
**Customer:** Budi Santoso (Umum)  
**Sales:** Adi Suryadi  
**Location:** TOKO  
**Payment:** Tunai

**Transaction:**

```
[STEP 1] Form Load
?? Kondisiawal()
?? TxtFaktur = "PJ-260304-0001"
?? DTPTgl = 2026-03-04 14:30
?? CmbPelanggan = ""
?? Grid empty, ready input

[STEP 2] Customer Selection
?? User select CmbPelanggan = "Budi Santoso"
?? AmbilInformasiPelanggan()
?? LblJenisPl = "Umum"
?? LbLKodePel = "CUST001"
?? DTPJatuhTempo = 2026-04-03 (30 days default)

[STEP 3] Sales Selection
?? CmbSales = "Adi Suryadi"
?? LblSales = "KAR001"
?? Ready to add items

[STEP 4] Add Item 1 - Sabun
?? User input "2*Sabun Cuci"
?? TxtQty = "2", name = "Sabun Cuci"
?? Ambildatalaindaridbbarang()
?  ?? ID_BARANG = "BAR001"
?  ?? SATUAN_UMUM_KECIL = "Pcs"
?  ?? HARGA_JUAL_UMUM_KECIL = 5.000
?  ?? HARGA_BELI = 3.000
?  ?? ISI_UMUM_KECIL = 1
?  ?? STOK_TOKO = 100
?? TambahDataLangsung()
?  ?? Insert row 0
?  ?? Kode = "BAR001"
?  ?? NamaBarang = "Sabun Cuci"
?  ?? QTY = 2
?  ?? Satuan = "Pcs"
?  ?? Isi = 1
?  ?? HargaBeli = 3.000
?  ?? Harga = 5.000
?  ?? QtySat = 2 × 1 = 2
?  ?? TotalHargaBeli = 3.000 × 1 × 2 = 6.000
?  ?? DiskonPersen = 0, DiskonRp = 0
?  ?? TotalHarga = (5.000 × 2) - 0 = 10.000
?? UpdateSemuaTotal()
?  ?? TotalHpp = 6.000
?  ?? Subtotal = 10.000
?  ?? TotalQtyBarang = 2
?  ?? TotalQtySatuan = 2
?  ?? TotalItem = 1
?? TxtNama cleared, ready next

[STEP 5] Add Item 2 - Minyak
?? User input "1*Minyak Goreng"
?? Ambildatalaindaridbbarang()
?  ?? ID_BARANG = "BAR002"
?  ?? HARGA_JUAL_UMUM_KECIL = 15.000
?  ?? HARGA_BELI = 12.000
?  ?? ISI_UMUM_KECIL = 1
?? TambahDataLangsung()
?  ?? Insert row 1
?  ?? Kode = "BAR002"
?  ?? NamaBarang = "Minyak Goreng"
?  ?? QTY = 1
?  ?? Harga = 15.000
?  ?? TotalHargaBeli = 12.000
?  ?? TotalHarga = 15.000
?  ?? Grid now has 2 rows
?? UpdateSemuaTotal()
?  ?? TotalHpp = 6.000 + 12.000 = 18.000
?  ?? Subtotal = 10.000 + 15.000 = 25.000
?  ?? TotalQtyBarang = 2 + 1 = 3
?  ?? TotalItem = 2
?? Ready to apply discount

[STEP 6] Apply Transaction Discount
?? User input TxtDiskonPersen = "10"
?? HitungDiskon("diskonpersen")
?  ?? DiskonRp = 25.000 × 10 / 100 = 2.500
?  ?? TxtDiskonRp = "2500"
?  ?? LblDiskonRp = "Rp. 2.500"
?? HitungTotalPenjualanAkhir()
?  ?? TotalSebelumDiskon = 25.000
?  ?? TotalSetelahDiskon = 25.000 - 2.500 = 22.500
?  ?? Pajak = 22.500 × 11 / 100 = 2.475
?  ?? GRAND_TOTAL = 22.500 + 2.475 = 24.975
?? TxtGrantotal = "Rp. 24.975"

[STEP 7] Validate & Proceed to Payment
?? User click BtnBayar (F8)
?? TekanBayar() validation
?  ?? ? Invoice = "PJ-260304-0001"
?  ?? ? Grid has 2 items
?  ?? ? Total = 24.975 > 0
?  ?? CekStok() - 2 units available ?
?  ?? Cekjualrugi() - Harga ? Cost ?
?? CenterPanelBayar() - Center GBBayar
?? GBBayar.Visible = True
?? TxtNominalBayar.Text = "24975" (jika Isinominal="Iya")
?? Focus ? TxtNominalBayar

[STEP 8] Payment Entry
?? CmbJenisBayar.SelectedIndex = 0 (TUNAI)
?? AmbiuldataRekening()
?  ?? Type_Akun = "TUNAI"
?  ?? PanelTFPelanggan.Visible = False (no bank fields)
?  ?? TxtTypeAkun = "TUNAI"
?? User input TxtNominalBayar = "25000"
?? TxtNominalBayar_TextChanged()
?  ?? Bayar = 25.000
?  ?? Balance = 25.000 - 24.975 = 25
?  ?? Kembali = 25
?  ?? LblStatusTrans = "LUNAS"
?  ?? LblPembayaran = "Kembali :"
?  ?? LblKembali = "Rp. 25"
?? Ready to save

[STEP 9] Save Transaction
?? User click BtnSimpan (F10)
?? TekanSimpan()
?  ?? ? TypeAkun = "TUNAI" (no bank validation)
?  ?? ? Customer selected
?? Simpanatauedit()
?  ?? Check invoice already exist ? No
?  ?? Call Prosessimpan()
?? Prosessimpan()
?  ?? BEGIN TRANSACTION
?  ?
?  ?? Simpanpenjualan()
?  ?  ?? INSERT INTO penjualan VALUES (
?  ?     'PJ-260304-0001',
?  ?     'PJ-260304-0001',
?  ?     2026-03-04 14:30:00,
?  ?     'CUST001',
?  ?     'Budi Santoso',
?  ?     'Umum',
?  ?     'TOKO',
?  ?     25.000,           -- GRAND_TOTAL_SBL_PAJAK
?  ?     2.500,            -- DISKON_TOTAL_RP
?  ?     2.475,            -- PAJAK_RP
?  ?     24.975,           -- GRAND_TOTAL_STL_PAJAK
?  ?     18.000,           -- TOTAL_HPP
?  ?     (24.975 - 18.000 - 2.500) = 4.475,  -- LABA
?  ?     25.000,           -- BAYAR
?  ?     25,               -- KEMBALI
?  ?     0,                -- SISA_TAGIHAN
?  ?     'TERBAYAR',       -- STATUS_BAYAR
?  ?     'TUNAI',          -- TYPE_AKUN
?  ?     'Tunai',          -- METODE
?  ?     NULL,             -- BANK
?  ?     NULL,             -- NO_REKENING
?  ?     NULL,             -- NAMA_REKENING
?  ?     'KAR001',         -- ID_SALES
?  ?     'Adi Suryadi',    -- NAMA_SALES
?  ?     'USER001',        -- ID_USER
?  ?     'CASHIER-01',     -- ID_KOMPUTER
?  ?     NOW(), NOW()
?  ?  )
?  ?
?  ?? Simpanpenjualandetail()
?  ?  ?? INSERT row 0: Sabun
?  ?  ?  ?? FAKTUR_JUAL = 'PJ-260304-0001'
?  ?  ?  ?? ID_BARANG = 'BAR001'
?  ?  ?  ?? QTY = 2
?  ?  ?  ?? SATUAN = 'Pcs'
?  ?  ?  ?? HARGA_JUAL = 5.000
?  ?  ?  ?? DISKON_RP = 0
?  ?  ?  ?? TOTAL_HARGA = 10.000
?  ?  ?  ?? ... other fields
?  ?  ?
?  ?  ?? INSERT row 1: Minyak
?  ?     ?? ID_BARANG = 'BAR002'
?  ?     ?? QTY = 1
?  ?     ?? HARGA_JUAL = 15.000
?  ?     ?? TOTAL_HARGA = 15.000
?  ?     ?? ... other fields
?  ?
?  ?? Simpanjurnal()
?  ?  ?? INSERT jurnal DEBIT Kas (24.975)
?  ?  ?? INSERT jurnal KREDIT Penjualan (25.000)
?  ?  ?? INSERT jurnal DEBIT COGS (18.000)
?  ?  ?? INSERT jurnal KREDIT Inventory (18.000)
?  ?
?  ?? HistoryBarang()
?  ?  ?? INSERT tbl_history_barang untuk BAR001
?  ?  ?  ?? TIPE_TRANSAKSI = 'PENJUALAN'
?  ?  ?  ?? QTY = 2
?  ?  ?  ?? STOK_SEBELUM = 100
?  ?  ?  ?? STOK_SESUDAH = 98
?  ?  ?
?  ?  ?? INSERT tbl_history_barang untuk BAR002
?  ?     ?? TIPE_TRANSAKSI = 'PENJUALAN'
?  ?     ?? QTY = 1
?  ?     ?? STOK_SEBELUM = 150
?  ?     ?? STOK_SESUDAH = 149
?  ?
?  ?? UPDATE tbl_barang
?  ?  ?? UPDATE BAR001: STOK_TOKO = 100 - 2 = 98
?  ?  ?? UPDATE BAR002: STOK_TOKO = 150 - 1 = 149
?  ?
?  ?? COMMIT TRANSACTION ? All or nothing
?  ?
?  ?? Print nota (CmbCetak = "IYA")
?  ?  ?? CetakFaktur() ? PrintJual.ProsesCetak()
?  ?
?  ?? Display kembalian message
?  ?  ?? TampilkanPesanKembaliPelanggan(25)
?  ?     [POPUP] KEMBALIAN
?  ?           Rp. 25
?  ?          [OK]
?  ?
?  ?? Reset form
?  ?  ?? Kondisiawal()
?  ?     ?? Clear all fields
?  ?     ?? New TxtFaktur = "PJ-260304-0002"
?  ?     ?? Grid empty
?  ?     ?? Ready for next transaction
?  ?
?  ?? DatabaseModule.CatatanAksiHistory("Simpan penjualan PJ-260304-0001")
?
?? Save to My.Settings
?  ?? My.Settings.CetakJual = "IYA"
?
?? [?] TRANSACTION COMPLETE

[DATABASE STATE AFTER]
?? penjualan: 1 record (PJ-260304-0001, TERBAYAR, Rp. 24.975)
?? penjualan_detail: 2 records (Sabun, Minyak)
?? tbl_history_barang: 2 records (stok adjustment log)
?? tbl_barang: updated stok
?  ?? BAR001: 98 units (dari 100)
?  ?? BAR002: 149 units (dari 150)
?? jurnal: 4 entries (double-entry accounting)
?? Nota printed (thermal/dot-matrix)
?? All audit fields (createdAt, idUser, idKomputer) logged
```

---

## ?? RESPONSIVE UI NOTES (For Flutter Implementation)

### Screen Sizes to Support
- Desktop: 1920×1080 (min 1280×720)
- Tablet: 1024×768
- Mobile: Adapt with bottom panel

### Key Layout Considerations
1. **Grid Flexibility:** Allow horizontal scroll for mobile
2. **Payment Panel:** Modal overlay (always on top)
3. **Search List:** Dropdown adaptive height
4. **Font Sizes:** Readable on all devices
5. **Touch Targets:** Min 48px for buttons on mobile

### Dark Mode Support
- Optional dark theme toggle
- Maintain contrast ratios (WCAG AA)

---

## ?? NOTES FOR IMPLEMENTATION

### Critical Success Factors

1. **Transaction Safety**
   - Always use database transactions
   - Rollback on any error
   - Never partially save

2. **Real-time Calculation**
   - Update totals on every grid change
   - No async delays for calculations
   - Immediate feedback to user

3. **Stock Accuracy**
   - Validate before payment
   - Update in single transaction
   - Log all changes in history table

4. **Barcode Support**
   - Timing-based detection (30ms threshold)
   - Support numeric + alphanumeric formats
   - Fallback to manual search

5. **User Experience**
   - Fast keyboard navigation
   - F-key shortcuts (F2-F12)
   - Visual feedback for all actions
   - Clear error messages

6. **Data Integrity**
   - Prevent duplicate items (if setting="Tidak")
   - Validate all inputs before DB
   - Audit trail (user, computer, timestamp)

### Missing Elements (For Reference)
- Edit Existing Penjualan (Editpenjualanheader, Hapusuntukedit)
- Hold & Recall (Tekantahan, Tekanpanggil, AmbilDataDitahan)
- Receivables (Simpanpiutang, tbl_piutang tracking)
- Printing (PrintJual, CetakFaktur, nota template)
- Accounting (Simpanjurnal, jurnal entries)

These are documented but not detailed in this guide.

---

**Document Version:** 1.0.0  
**Created:** Maret 2026  
**For Dart Flutter Developers**  
**Status:** ? READY FOR IMPLEMENTATION

