# ?? SALES MODULE COMPLETE GUIDE - FormPenjualan

**For:** Dart Developers, Full-Stack Engineers, Database Architects  
**Reference Files:**
- FormPenjualan.vb (Main sales form - 3600+ lines)
- FormPenjualan.Designer.vb (UI design - 17 columns DataGrid)
- FormUtama.vb (Main form integration)
- DatabaseModule.vb (Database layer)
- FormPenjualanDitahan.vb (Hold/Recall feature)

**Status:** ? Production Ready  
**Last Updated:** Maret 2026  

---

## ?? TABLE OF CONTENTS

1. [Module Overview](#module-overview)
2. [Database CRUD Operations](#database-crud)
3. [Complete Database Schema](#database-schema)
4. [Form Components & UI](#form-components)
5. [Functional Flows](#functional-flows)
6. [Business Logic & Formulas](#business-logic)
7. [Barcode Detection System](#barcode-detection)
8. [Validation & Error Handling](#validation)
9. [Implementation Checklist for Dart](#dart-implementation)

---

## <a name="module-overview"></a>?? MODULE OVERVIEW

### What is FormPenjualan?

**FormPenjualan** adalah form penjualan lengkap yang menangani:

```
???????????????????????????????????????????????????????
?   PENJUALAN MODULE (FormPenjualan)                  ?
???????????????????????????????????????????????????????
?                                                     ?
?  1. Item Input                                      ?
?     ?? Barcode scanning (rapid detection)          ?
?     ?? Manual name search (like search)            ?
?     ?? Auto quantity/satuan parsing                ?
?                                                     ?
?  2. Grid Management                                ?
?     ?? 17 column DataGrid                          ?
?     ?? Real-time calculation                       ?
?     ?? Duplicate item handling                     ?
?     ?? Row deletion & recalc                       ?
?                                                     ?
?  3. Discount & Tax                                 ?
?     ?? Item-level discount (% or Rp)              ?
?     ?? Transaction-level discount                 ?
?     ?? PPN tax (11% or custom)                     ?
?     ?? Shipping fee support                        ?
?                                                     ?
?  4. Payment                                        ?
?     ?? Tunai (Cash)                                ?
?     ?? Transfer Bank                               ?
?     ?? Tempo (Hutang/Piutang)                      ?
?     ?? Auto change/debt calculation                ?
?                                                     ?
?  5. Data Persistence                               ?
?     ?? Save to penjualan header                    ?
?     ?? Save to penjualan_detail lines              ?
?     ?? Stock audit trail (history)                 ?
?     ?? Accounting entries (jurnal)                 ?
?     ?? Transaction rollback on error               ?
?                                                     ?
?  6. Special Features                               ?
?     ?? Hold transaction (Tahan/Panggil)            ?
?     ?? Recall held transactions                    ?
?     ?? Profit calculation                          ?
?     ?? Memo printing support                       ?
?     ?? User & computer audit trail                 ?
?                                                     ?
???????????????????????????????????????????????????????
```

### Operating Modes

| Mode | Trigger | Behavior |
|------|---------|----------|
| **TambahPenjualan** | Form load / F8 Bayar | Fresh transaction, clear form |
| **EditPenjualan** | Called from FormUtama | Edit existing transaction, no stock re-adjustment |
| **Recall** | F7 Panggil | Restore from penjualan_ditahan, continue |

---

## <a name="database-crud"></a>??? DATABASE CRUD OPERATIONS

### **CREATE** - Simpan Penjualan (INSERT)

#### Flow Diagram
```
User Click BtnSimpan (F10)
    ?
[TekanSimpan] - Validasi pembayaran
    ?? ? Cek bank payment wajib nominal > 0
    ?? ? Cek hutang wajib customer dipilih
    ?? ? Konfirmasi jika nominal 0
    ?
[Simpanatauedit]
    ?? Cek invoice sudah exist
    ?? Generate invoice jika duplikat
    ?? Call Prosessimpan()
    ?
[Prosessimpan] - DATABASE TRANSACTION
    ?? BEGIN TRANSACTION
    ?
    ?? 1. Simpanpenjualan() ? INSERT penjualan header
    ?  ?? 34 columns including profit
    ?
    ?? 2. Simpanpenjualandetail() ? INSERT penjualan_detail
    ?  ?? Loop all grid rows
    ?
    ?? 3. Simpanjurnal() ? INSERT jurnal accounting
    ?  ?? Debit/Kredit entries
    ?
    ?? 4. HistoryBarang() ? INSERT history barang
    ?  ?? Stock audit trail per item
    ?
    ?? 5. UPDATE tbl_barang (Reduce Stock)
    ?  ?? STOK_TOKO or STOK_GUDANG -= qty_satuan
    ?
    ?? COMMIT TRANSACTION
    ?
    ?? Print nota (optional)
    ?? Show return message
    ?? Reset form (Kondisiawal)
```

#### SQL Inserted Tables

**Table 1: penjualan (Header)**
```sql
INSERT INTO penjualan (
    ID_PENJUALAN,
    FAKTUR_JUAL,
    TANGGAL_PENJUALAN,
    ID_PELANGGAN,
    NAMA_PELANGGAN,
    JENIS_PELANGGAN,
    LOKASIBARANG,
    GRAND_TOTAL_SBL_PAJAK,
    DISKON_TOTAL_RP,
    PAJAK_RP,
    GRAND_TOTAL_STL_PAJAK,
    TOTAL_HPP,
    LABA,
    BAYAR,
    KEMBALI,
    SISA_TAGIHAN,
    STATUS_BAYAR,
    TYPE_AKUN,
    METODE,
    BANK,
    NO_REKENING,
    NAMA_REKENING,
    NO_REFF,
    ID_SALES,
    NAMA_SALES,
    JANGKA_PIUTANG,
    BIAYA_KIRIM,
    ID_USER,
    ID_KOMPUTER,
    CREATED_AT,
    UPDATED_AT
) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NOW(), NOW())
```

**Table 2: penjualan_detail (Line Items)**
```sql
INSERT INTO penjualan_detail (
    FAKTUR_JUAL,
    ID_BARANG,
    NAMA_BARANG,
    SERIAL_NUMBER,
    HARGA_BELI,
    QTY,
    SATUAN,
    ISI_SATUAN,
    HARGA_BELI_SATUAN,
    HARGA_JUAL,
    QTY_SATUAN,
    DISKON_PERSEN,
    DISKON_RP,
    TOTAL_DISKON,
    TOTAL_HARGA,
    STOK_TOKO,
    STOK_GUDANG,
    STOK,
    CREATED_AT
) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NOW())
```

**Table 3: tbl_history_barang (Stock Audit Trail)**
```sql
INSERT INTO tbl_history_barang (
    ID_BARANG,
    TIPE_TRANSAKSI,
    QTY,
    STOK_SEBELUM,
    STOK_SESUDAH,
    CREATED_AT
) VALUES (?, 'PENJUALAN', ?, ?, ?, NOW())

-- Untuk setiap barang:
-- STOK_SEBELUM = STOK_TOKO or STOK_GUDANG saat transaksi
-- STOK_SESUDAH = STOK_SEBELUM - QTY_SATUAN
```

---

### **READ** - Ambil Data Penjualan (SELECT)

#### 1. Get Transaction Header
```sql
SELECT * FROM penjualan 
WHERE ID_PENJUALAN = 'PJ-260304-0001'
```

**Purpose:** Load existing transaction untuk edit mode

#### 2. Get Transaction Detail Lines
```sql
SELECT * FROM penjualan_detail 
WHERE FAKTUR_JUAL = 'PJ-260304-0001'
ORDER BY ID_DETAIL ASC
```

**Purpose:** Populate grid dengan barang-barang dalam transaksi

#### 3. Get Held Transactions (Tahan)
```sql
SELECT 
    FAKTUR_JUAL,
    ID_PELANGGAN,
    NAMA_PELANGGAN,
    JENIS_PELANGGAN,
    TANGGAL_JUAL,
    GRAN_TOTAL,
    TOTAL_QTY,
    TOTAL_ITEM,
    ID_USER
FROM penjualan_ditahan
ORDER BY TANGGAL_JUAL DESC
```

**Purpose:** Show list of held transactions di FormPenjualanDitahan

#### 4. Get Customer List
```sql
SELECT NAMA FROM tbl_pelanggan ORDER BY NAMA ASC
```

**Purpose:** Populate CmbPelanggan dropdown

#### 5. Get Customer Info
```sql
SELECT 
    KODE,
    ALAMAT,
    JENIS,
    JangkaPiutang
FROM tbl_pelanggan 
WHERE NAMA = 'Budi Santoso'
```

**Purpose:** Get jenis pelanggan (Umum/Partai) untuk harga tier

---

### **UPDATE** - Edit Penjualan

#### Edit Transaction Header
```sql
UPDATE penjualan SET
    TANGGAL_PENJUALAN = ?,
    DISKON_TOTAL_RP = ?,
    PAJAK_RP = ?,
    GRAND_TOTAL_STL_PAJAK = ?,
    BAYAR = ?,
    KEMBALI = ?,
    SISA_TAGIHAN = ?,
    STATUS_BAYAR = ?,
    UPDATED_AT = NOW()
WHERE ID_PENJUALAN = 'PJ-260304-0001'
```

#### Important: Edit Mode Stock Handling
```vb
' Jika TxtJenistransaksi.Text = "EditPenjualan"
' JANGAN ubah stok barang
' Hanya update transaksi header & detail
' Stok hanya berkurang saat pertama kali INSERT
```

---

### **DELETE** - Hapus Penjualan

#### Delete Transaction (Hapustransaksi)
```sql
-- Step 1: Restore stock untuk setiap barang
UPDATE tbl_barang SET 
    STOK_TOKO = STOK_TOKO + qty_satuan
WHERE ID_BARANG = ?

-- Step 2: Delete detail lines
DELETE FROM penjualan_detail 
WHERE FAKTUR_JUAL = 'PJ-260304-0001'

-- Step 3: Delete header
DELETE FROM penjualan 
WHERE ID_PENJUALAN = 'PJ-260304-0001'

-- Step 4: Delete history entries
DELETE FROM tbl_history_barang 
WHERE ... (related transaction)

-- ? ALL WITHIN TRANSACTION - ROLLBACK IF ERROR
```

#### Delete Held Transaction (FormPenjualanDitahan)
```sql
-- Delete from both tables
DELETE FROM penjualan_ditahan 
WHERE FAKTUR_JUAL = 'PJ-260304-0001'

DELETE FROM penjualan_ditahan_detail 
WHERE FAKTUR_JUAL = 'PJ-260304-0001'
```

---

## <a name="database-schema"></a>??? COMPLETE DATABASE SCHEMA

### Table 1: penjualan (Main Transaction)
```
PK: ID_PENJUALAN (VARCHAR(50))
Format: PJ-YYMMDD-XXXX
Example: PJ-260304-0001 (4 Maret 2026, transaksi ke-1)

Columns (34 total):
?? Transaction Info
?  ?? ID_PENJUALAN (PK)
?  ?? FAKTUR_JUAL (UNIQUE)
?  ?? TANGGAL_PENJUALAN (DATETIME)
?  ?? LOKASIBARANG (VARCHAR(10): TOKO/GUDANG)
?
?? Customer Info
?  ?? ID_PELANGGAN (FK ? tbl_pelanggan)
?  ?? NAMA_PELANGGAN (VARCHAR(255))
?  ?? JENIS_PELANGGAN (VARCHAR(20): Umum/Partai)
?  ?? JANGKA_PIUTANG (INT, days for due date)
?
?? Amount Calculation
?  ?? GRAND_TOTAL_SBL_PAJAK (DECIMAL(18,2))
?  ?  ?? SUM(all items.TOTAL_HARGA) before discount
?  ?? DISKON_PERSEN (DECIMAL(5,2), 0-100%)
?  ?? DISKON_TOTAL_RP (DECIMAL(18,2))
?  ?  ?? GRAND_TOTAL_SBL_PAJAK × DISKON_PERSEN / 100
?  ?? PAJAK_PERSEN (DECIMAL(5,2), typically 11%)
?  ?? PAJAK_RP (DECIMAL(18,2))
?  ?  ?? (SBL_PAJAK - DISKON) × PAJAK_PERSEN / 100
?  ?? BIAYA_KIRIM (DECIMAL(18,2), shipping/extra fee)
?  ?? GRAND_TOTAL_STL_PAJAK (DECIMAL(18,2))
?     ?? SBL_PAJAK - DISKON + PAJAK + KIRIM
?
?? Cost Analysis (HPP = Harga Pokok Penjualan)
?  ?? TOTAL_HPP (DECIMAL(18,2))
?  ?  ?? SUM(item.HARGA_BELI × ITEM.ISI × ITEM.QTY)
?  ?? LABA (DECIMAL(18,2))
?     ?? (TOTAL_STL_PAJAK - TOTAL_HPP) - DISKON
?
?? Payment Info
?  ?? BAYAR (DECIMAL(18,2))
?  ?  ?? Amount paid by customer
?  ?? KEMBALI (DECIMAL(18,2))
?  ?  ?? Change/balance returned
?  ?? SISA_TAGIHAN (DECIMAL(18,2))
?  ?  ?? Debt remaining (if tempo/hutang)
?  ?? STATUS_BAYAR (VARCHAR(20))
?  ?  ?? TERBAYAR (paid) / TERHUTANG (debt)
?  ?? TYPE_AKUN (VARCHAR(20))
?  ?  ?? TUNAI / BANK / PIUTANG
?  ?? METODE (VARCHAR(50))
?  ?  ?? Transfer / Tunai / Tempo / QRIS
?  ?? BANK (VARCHAR(50))
?  ?  ?? Bank name if transfer
?  ?? NO_REKENING (VARCHAR(30))
?  ?? NAMA_REKENING (VARCHAR(255))
?  ?? NO_REFF (VARCHAR(50))
?     ?? Reference number for transfer proof
?
?? Sales Info
?  ?? ID_SALES (FK ? tbl_karyawan)
?  ?? NAMA_SALES (VARCHAR(255))
?
?? Audit Trail
   ?? ID_USER (VARCHAR(50))
   ?? ID_KOMPUTER (VARCHAR(50))
   ?? CREATED_AT (DATETIME)
   ?? UPDATED_AT (DATETIME)

Indexes:
?? idx_faktur_jual (UNIQUE)
?? idx_id_pelanggan (FK lookup)
?? idx_tanggal_penjualan (Report filter)
?? idx_status_bayar (Quick filter)
```

### Table 2: penjualan_detail (Line Items)
```
PK: ID_DETAIL (INT AUTO_INCREMENT)
FK: FAKTUR_JUAL ? penjualan, ID_BARANG ? tbl_barang

Columns (18 total):
?? Reference
?  ?? ID_DETAIL (PK)
?  ?? FAKTUR_JUAL (FK)
?  ?? ID_BARANG (FK)
?
?? Product Info (Snapshot)
?  ?? NAMA_BARANG (VARCHAR(255))
?  ?? SERIAL_NUMBER (VARCHAR(100), optional)
?
?? Pricing & Quantity
?  ?? HARGA_BELI (DECIMAL(18,2))
?  ?  ?? Cost price at transaction time
?  ?? HARGA_JUAL (DECIMAL(18,2))
?  ?  ?? Selling price at transaction time
?  ?? QTY (DECIMAL(18,2))
?  ?  ?? Quantity in selected unit
?  ?? SATUAN (VARCHAR(50))
?  ?  ?? Unit name (Pcs, Box, Dus)
?  ?? ISI_SATUAN (INT)
?  ?  ?? Content per unit (1, 12, 100)
?  ?? QTY_SATUAN (DECIMAL(18,2))
?  ?  ?? QTY × ISI (base unit quantity)
?  ?? HARGA_BELI_SATUAN (DECIMAL(18,2))
?  ?  ?? HARGA_BELI × ISI (total cost per unit)
?  ?? TOTAL_HARGA_BELI (DECIMAL(18,2))
?     ?? HARGA_BELI × ISI × QTY (total cost)
?
?? Discount (Per-Line)
?  ?? DISKON_PERSEN (DECIMAL(5,2))
?  ?  ?? Discount percentage
?  ?? DISKON_RP (DECIMAL(18,2))
?  ?  ?? Discount amount per unit
?  ?? TOTAL_DISKON (DECIMAL(18,2))
?     ?? QTY × DISKON_RP
?
?? Line Total
?  ?? TOTAL_HARGA (DECIMAL(18,2))
?     ?? (HARGA_JUAL × QTY) - TOTAL_DISKON
?
?? Stock Reference
?  ?? STOK_TOKO (DECIMAL(18,2))
?  ?  ?? Store stock at transaction time
?  ?? STOK_GUDANG (DECIMAL(18,2))
?  ?  ?? Warehouse stock at transaction time
?  ?? STOK (DECIMAL(18,2))
?     ?? Active location stock
?
?? Audit
   ?? CREATED_AT (DATETIME)

Indexes:
?? idx_faktur_jual (For detail lookup)
?? idx_id_barang (For stock reference)
```

### Table 3: penjualan_ditahan (Held Transactions)
```
Purpose: Temporary storage for incomplete transactions (Hold feature)
Used By: FormPenjualanDitahan for Tahan/Panggil (Hold/Recall)

Columns: SAME as penjualan + penjualan_ditahan_detail (duplicated)

Key Difference:
?? Not committed to main penjualan table
?? Stock NOT reduced
?? Can be deleted anytime
?? Can be recalled and completed later
?? Cleared when transaction finishes

Delete Condition:
?? When user clicks "Proses" (F9) in FormPenjualanDitahan
   ? Data moved to main penjualan table
   ? Deleted from penjualan_ditahan
```

### Table 4: tbl_history_barang (Stock Audit Trail)
```
Purpose: Track all stock movements for inventory audit

PK: ID (INT AUTO_INCREMENT)
FK: ID_BARANG ? tbl_barang

Columns:
?? ID (PK)
?? ID_BARANG (FK)
?? TIPE_TRANSAKSI (VARCHAR(50))
?  ?? PENJUALAN / PEMBELIAN / RETUR / OPNAME / TRANSFER
?? QTY (DECIMAL(18,2))
?  ?? Quantity changed (positive or negative)
?? STOK_SEBELUM (DECIMAL(18,2))
?  ?? Stock before transaction
?? STOK_SESUDAH (DECIMAL(18,2))
?  ?? Stock after transaction
?? CREATED_AT (DATETIME)

Example for Penjualan:
?? TIPE_TRANSAKSI = 'PENJUALAN'
?? QTY = -24 (24 units sold)
?? STOK_SEBELUM = 100
?? STOK_SESUDAH = 76
?? Used for: Stock reconciliation, audit trail
```

### Table 5: tbl_barang (Product Master)
```
Used Columns for Sales:
?? ID_BARANG (PK) - Product code
?? NAMA_BARANG - Product name
?? HARGA_BELI - Cost price (from supplier)
?? BARCODE_KECIL/SEDANG/BESAR - 3-level barcode support
?
?? SATUAN_UMUM_KECIL/SEDANG/BESAR - Unit names
?? ISI_UMUM_KECIL/SEDANG/BESAR - Content per unit (1, 12, 100)
?? HARGA_JUAL_UMUM_KECIL/SEDANG/BESAR - Retail prices
?
?? SATUAN_PARTAI_KECIL/SEDANG/BESAR - Wholesale units
?? ISI_PARTAI_KECIL/SEDANG/BESAR - Content per unit
?? HARGA_JUAL_PARTAI_KECIL/SEDANG/BESAR - Wholesale prices
?
?? STOK_TOKO (DECIMAL(18,2)) - Current store stock
?? STOK_GUDANG (DECIMAL(18,2)) - Current warehouse stock
?
?? Stock Mutation Tracking
   ?? PENJUALAN_TOKO / PENJUALAN_GUDANG
   ?? PEMBELIAN_TOKO / PEMBELIAN_GUDANG
   ?? RETUR_JUAL_TOKO / RETUR_JUAL_GUDANG
   ?? OPNAME_TOKO / OPNAME_GUDANG
   ?? TRANSFER_STOK_MASUK/KELUAR
   ?? Used by HitungStokToko() / HitungStokGudang()
```

---

## <a name="form-components"></a>?? FORM COMPONENTS & UI LAYOUT

### Screen Resolution & Layout
```
Form Size: 1291 × 630 px (Maximized)
Culture: Indonesian (id-ID)
Font: Century Gothic, Microsoft Sans Serif
Layout: Dock-based with panels

Structure:
???????????????????????????????????????????????????????
? [GroupBox1 - Header] (172px height)                 ?
? ?? LblTextJalanAtas (Running text - company name)   ?
? ?? GroupBox2 (Invoice, Date, Customer, Sales)       ?
? ?? GroupBox3 (Grand Total display - large)          ?
???????????????????????????????????????????????????????
? PanelCariNama (Yellow highlight when focused)       ?
? ?? TxtNama (Main input - barcode/name)              ?
? ?? LstBarang (Dropdown list - search results)       ?
? ?? BtnCari (Search button - disabled)               ?
???????????????????????????????????????????????????????
? DgvData (DataGridView - 17 columns)                 ?
? ?? 360px height, auto-scroll                        ?
? ?? No resize rows/columns allowed                   ?
? ?? RMB context menu: Hapus, Hitung Ulang           ?
? ?? RowPostPaint: Auto numbering (1,2,3...)          ?
???????????????????????????????????????????????????????
? Panel1 (Bottom - Discount, Tax, Buttons)            ?
? ?? Diskon section (% and Rp)                        ?
? ?? Pajak section (% and Rp)                         ?
? ?? Buttons: Bayar(F8), Tahan(F6), Panggil(F7)      ?
? ??          Barang(F4), Pelanggan(F12)             ?
? ??          CmbCetak (Iya/Tidak/Tanya)             ?
???????????????????????????????????????????????????????
? GBBayar (Payment Modal - Initially Hidden)          ?
? ?? 933×280px (BANK) / 529×280px (TUNAI)            ?
? ?? Centered on form                                ?
? ?? CmbJenisBayar (Payment method dropdown)          ?
? ?? PanelTFPelanggan (Bank fields - conditional)    ?
? ?? TxtNominalBayar (Amount input)                   ?
? ?? BtnSimpan(F10), BtnBatal(F11)                    ?
? ?? DTPJatuhTempo (Due date - if tempo)             ?
?                                                     ?
???????????????????????????????????????????????????????
```

### DataGridView Columns (17 total)

| # | Column Name | Type | Width | Format | Editable | Purpose |
|---|---|---|---|---|---|---|
| 0 | Kode | Text | 50px | Text | No | Product code (hidden, logic only) |
| 1 | NamaBarang | Text | 200px | Text | No (after set) | Product name |
| 2 | HargaBeli | Text | 80px | #,0.## | No | Cost price (display only) |
| 3 | QTY | Text | 40px | #,0.## | **Yes** | User enters quantity |
| 4 | Satuan | ComboBox | 60px | List | **Yes** | Unit selection (Pcs/Box/Dus) |
| 5 | Isi | Text | 30px | #0 | No | Content per unit |
| 6 | TotalHargaBeli | Text | 100px | #,0.## | No | Cost calculation |
| 7 | Harga | Text | 70px | #,0.## | **Yes** | User edits selling price |
| 8 | QtySat | Text | 60px | #,0.## | No | Auto: QTY × ISI |
| 9 | DiskonPersen | Text | 60px | #0.## | **Yes** | User enters % |
| 10 | DiskonRp | Text | 80px | #,0.## | **Yes** | User enters amount |
| 11 | TotalDiskon | Text | 80px | #,0.## | No | Auto: QTY × DiskonRp |
| 12 | TotalHarga | Text | 100px | #,0.## | No | Auto: (Harga × QTY) - Diskon |
| 13 | StokToko | Text | 60px | #,0.## | No | Store stock (blue bg) |
| 14 | StokGudang | Text | 70px | #,0.## | No | Warehouse stock (blue bg) |
| 15 | Stok | Text | 30px | #,0.## | No | Active location stock |
| 16 | SerialNumber | Text | 80px | Text | **Yes** | Optional serial # |

**Grid Features:**
- Auto scroll to last row on add
- Row header auto-numbering (1, 2, 3...)
- Delete key: Remove row if name filled
- RMB context menu on NamaBarang column
- Red highlight: Stock < 1 or validation error

---

## <a name="functional-flows"></a>?? FUNCTIONAL FLOWS

### FLOW 1: Fresh Transaction (TambahPenjualan)

```
[Start]
    ?
[Form_Load]
    ?? Load permissions from FormGeneralSetting
    ?? Setup UI components (tooltips, culture)
    ?? Load printer settings
    ?? Setup barcode timer (100ms interval)
    ?
[Form_Shown]
    ?? Check TxtJenistransaksi = "TambahPenjualan"
    ?? Call Kondisiawal()
    ?
[Kondisiawal] - RESET EVERYTHING
    ?? Clear all textboxes
    ?? DgvData.Rows.Clear()
    ?? Load CmbPelanggan (TampilPelanggan)
    ?? Load CmbSales (AmbilDataKaryawan)
    ?? Generate invoice (Nomorjual)
    ?? Get count of held (JumlahTahan)
    ?? Init all totals = 0
    ?? Set default payment account
    ?? Set focus mode (Search or Direct Edit)
    ?? Ready for user input
    ?
[User Input Item]
    ?? Type in TxtNama (or scan barcode)
    ?? Barcode detection logic (200ms timing)
    ?? Manual search list appears
    ?? Select from LstBarang or press ENTER
    ?
[TambahDataLangsung]
    ?? Validate duplicate (Kodebarangsama setting)
    ?? Insert row to grid
    ?? Load satuan combobox
    ?? Calculate row total
    ?? SetupFocusToGrid() for next entry
    ?? Ready for next item
    ?
[User Reviews & Edits]
    ?? Can edit: QTY, Satuan, Harga, Diskon
    ?? Grid recalculates on each change
    ?? UpdateSemuaTotal() updates grand total
    ?? Delete rows with DEL key
    ?
[User Clicks BtnBayar (F8)]
    ?? TekanBayar() validates
    ?  ?? Check invoice exists
    ?  ?? Check items added
    ?  ?? Check stock (if enabled)
    ?  ?? Check no loss sales (if enabled)
    ?? Show GBBayar payment panel
    ?? Focus to TxtNominalBayar
    ?? Await payment entry
    ?
[User Enters Payment Amount]
    ?? TxtNominalBayar_TextChanged
    ?? Calculate change/debt
    ?? Update status (LUNAS/BELUM LUNAS)
    ?? Show due date if tempo
    ?? Ready for save
    ?
[User Clicks BtnSimpan (F10)]
    ?? TekanSimpan() validates payment
    ?? Simpanatauedit() checks duplicate
    ?? Call Prosessimpan()
    ?
[Prosessimpan] - TRANSACTION START
    ?? BEGIN TRANSACTION
    ?? Simpanpenjualan() ? Insert header
    ?? Simpanpenjualandetail() ? Insert detail rows
    ?? Simpanjurnal() ? Insert accounting
    ?? HistoryBarang() ? Insert stock trail
    ?? UPDATE tbl_barang (reduce stock)
    ?? COMMIT ?
    ?? CetakFaktur() ? Print (optional)
    ?? TampilkanPesanKembaliPelanggan() ? Show change
    ?? Kondisiawal() ? Reset form
    ?? [End - Ready for next transaction]
    ?
[Error Occurs]
    ?? ROLLBACK entire transaction ? No partial data

KEY ENTRY POINTS:
?? Kondisiawal() - Always call to reset
?? UpdateSemuaTotal() - Always recalculate after change
?? Prosessimpan() - ONLY save with transaction
?? CetakFaktur() - Print after successful commit
```

### FLOW 2: Edit Transaction (EditPenjualan)

```
[User selects transaction in FormUtama]
    ?
[Edit button clicked]
    ?? Set TxtJenistransaksi = "EditPenjualan"
    ?? Pass FAKTUR_JUAL to FormPenjualan
    ?? Open FormPenjualan
    ?
[Form_Shown]
    ?? Check TxtJenistransaksi = "EditPenjualan"
    ?? Call Editpenjualanheader()
    ?
[Editpenjualanheader]
    ?? Load penjualan header data
    ?? Populate all header fields
    ?? Call AmbilDataDitahan() OR GetPenjualanDetail()
    ?? Populate DgvData with existing lines
    ?? Calculate current totals
    ?? Ready for edits
    ?
[User Edits]
    ?? Can change: Qty, Satuan, Harga, Diskon
    ?? Grid recalculates
    ?? BUT: STOK NOT affected
    ?? Can change: Payment, Due date
    ?
[User Clicks BtnSimpan (F10)]
    ?? TekanSimpan() validates
    ?? Call Prosessimpan()
    ?
[Prosessimpan - EDIT MODE]
    ?? BEGIN TRANSACTION
    ?? Hapusuntukedit() - Only if location changed
    ?  ?? Restore old stock
    ?? Simpanpenjualan() ? UPDATE header
    ?? Simpanpenjualandetail() ? DELETE detail + INSERT new
    ?? Simpanjurnal() ? DELETE old + INSERT new
    ?? HistoryBarang() ? DELETE old + INSERT new
    ?? COMMIT ?
    ?? CetakFaktur() ? Print (optional)
    ?? Close form
    ?? [Return to FormUtama]
    ?
?? CRITICAL RULE FOR EDIT:
   Do NOT re-reduce stock in edit mode
   Only reduce when FIRST CREATED
```

### FLOW 3: Hold & Recall (Tahan/Panggil)

```
[Incomplete Transaction - User Press F6 Tahan]
    ?
[Tekantahan]
    ?? INSERT to penjualan_ditahan (header)
    ?? INSERT to penjualan_ditahan_detail (all rows)
    ?? Commit transaction
    ?? Update TxtTahan count
    ?? Kondisiawal() reset form
    ?? [Ready for next transaction]
    
?? Held transaction:
   ?? NOT in main penjualan table
   ?? Stock NOT affected
   ?? Can be edited/completed later
   ?? Can be deleted anytime
    ?
[Later - User Press F7 Panggil]
    ?
[Tekanpanggil]
    ?? Check TxtTahan > 0
    ?? Open FormPenjualanDitahan (list)
    ?
[FormPenjualanDitahan]
    ?? AmbilData() ? SELECT from penjualan_ditahan
    ?? Show grid of held transactions
    ?? User selects one
    ?? Click BtnProses (or press F9)
    ?
[Process Held Transaction]
    ?? Set TxtFaktur = held invoice number
    ?? Set TxtJenistransaksi = "TambahPenjualan"
    ?? Call AmbilDataDitahan()
    ?? Populate grid with held items
    ?? Delete from penjualan_ditahan
    ?? DELETE from penjualan_ditahan_detail
    ?? Close FormPenjualanDitahan
    ?? [Back to FormPenjualan - resume transaction]
    ?
[User continues editing, then save]
    ?? Same as FLOW 1 above
    ?? INSERT to main penjualan table
```

---

## <a name="business-logic"></a>?? BUSINESS LOGIC & FORMULAS

### Item-Level Calculations

**Formula 1: Quantity in Base Unit**
```
qtySat = qty × isiSatuan

Example:
  qty = 2 (boxes)
  isiSatuan = 12 (items per box)
  qtySat = 24 (total items)
  
Used for: Stock reduction, cost calculation
```

**Formula 2: Total Cost (COGS)**
```
totalHargaBeli = hargaBeli × isiSatuan × qty

Example:
  hargaBeli = 2,500/item
  isiSatuan = 12 items/box
  qty = 2 boxes
  totalHargaBeli = 2,500 × 12 × 2 = 60,000
```

**Formula 3: Item Discount**
```
totalDiskon = qty × diskonRp

Where diskonRp is calculated if user enters %:
  diskonRp = hargaJual × diskonPersen / 100

Example:
  hargaJual = 50,000
  diskonPersen = 10%
  diskonRp = 50,000 × 10/100 = 5,000 (per unit)
  qty = 2
  totalDiskon = 2 × 5,000 = 10,000
```

**Formula 4: Line Total**
```
totalHarga = (hargaJual × qty) - totalDiskon

Example:
  hargaJual = 50,000
  qty = 2
  (50,000 × 2) - 10,000 = 90,000
```

### Transaction-Level Calculations

**Formula 5: Subtotal (Before discount)**
```
subtotal = SUM(all_items.totalHarga)

Example: Item1(90k) + Item2(60k) + Item3(50k) = 200,000
```

**Formula 6: Transaction-Level Discount**
```
diskonTransaksi = subtotal × diskonPersen / 100

Example:
  subtotal = 200,000
  diskonPersen = 10%
  diskonTransaksi = 20,000
  
  After discount = 200,000 - 20,000 = 180,000
```

**Formula 7: Tax (PPN)**
```
pajak = (subtotal - diskonTransaksi) × pajakPersen / 100

?? CRITICAL: Tax is AFTER discount, not on subtotal!

Example:
  subtotal = 200,000
  diskonTransaksi = 20,000
  baseTax = 180,000
  pajakPersen = 11% (standard PPN)
  pajak = 180,000 × 11/100 = 19,800
```

**Formula 8: Final Total**
```
grandTotal = subtotal - diskonTransaksi + pajak + biayaKirim

Example:
  subtotal = 200,000
  diskon = 20,000
  pajak = 19,800
  kirim = 5,000
  grandTotal = 200,000 - 20,000 + 19,800 + 5,000 = 204,800
```

**Formula 9: Payment Balance**
```
If nominalBayar >= grandTotal:
  kembalian = nominalBayar - grandTotal
  statusBayar = "TERBAYAR" (PAID)
  hutang = 0
  
Else:
  kembalian = 0
  hutang = grandTotal - nominalBayar
  statusBayar = "TERHUTANG" (DEBT)
  
If nominalBayar = 0:
  statusBayar = "TERHUTANG" (DEBT)
  hutang = grandTotal
```

**Formula 10: Profit (Laba)**
```
laba = (grandTotal - totalHPP) - diskonTransaksi

Where totalHPP = SUM(item.totalHargaBeli)

Example:
  grandTotal = 204,800
  totalHPP = 80,000 (sum of all costs)
  diskonTransaksi = 20,000
  laba = (204,800 - 80,000) - 20,000 = 104,800
  margin = 104,800 / 204,800 = 51%
```

---

## <a name="barcode-detection"></a>?? BARCODE DETECTION SYSTEM

### Algorithm

```
Constants:
?? BARCODE_CHAR_INTERVAL_MS = 30ms (time between chars)
?? BARCODE_TOTAL_TIME_MS = 200ms (max total time)
?? BARCODE_MIN_LENGTH = 4 (minimum barcode length)
?? BARCODE_MAX_LENGTH = 100 (maximum barcode length)

Process:
[User types/scans]
    ?
[TxtNama_KeyDown]
    ?? char 1: Start timer, add to buffer
    ?? char 2: Check interval
    ?  ?? If < 30ms: Barcode candidate
    ?  ?? If > 30ms: Manual input
    ?? char 3-N: Continue collecting
    ?? Buffer full (100 chars) or ENTER pressed
    ?
[Evaluate timing]
    ?? If totalTime ? 200ms AND length ? 4
    ?  ?? BARCODE MODE: SearchByBarcode()
    ?
    ?? If totalTime > 200ms OR contains letters
       ?? MANUAL MODE: ProcessManualSearchList()
    ?
[BARCODE MODE: SearchByBarcode]
    ?? EXACT MATCH in tbl_barang
    ?  ?? WHERE BARCODE_KECIL = input
    ?  ?? OR BARCODE_SEDANG = input
    ?  ?? OR BARCODE_BESAR = input
    ?? If found:
    ?  ?? TambahDataLangsung() ? Add to grid
    ?? If NOT found:
    ?  ?? MessageBox: "Barcode not found"
    ?? Clear TxtNama, ready for next
    ?
[MANUAL MODE: ProcessManualSearchList]
    ?? LIKE SEARCH in tbl_barang
    ?  ?? WHERE ID_BARANG LIKE '%keyword%'
    ?  ?? OR NAMA_BARANG LIKE '%keyword%'
    ?  ?? OR BARCODE_* LIKE '%keyword%'
    ?? Return max 20 results
    ?? Show in LstBarang dropdown
    ?? User navigates with ?? keys
    ?? Press ENTER or CLICK to select
    ?? TambahDataLangsung() ? Add to grid
```

### Input Format Support

| Format | Example | Behavior |
|--------|---------|----------|
| **Barcode only** | `8991234567890` | Scan barcode, exact match, add if found |
| **Name only** | `Sabun` | Manual search, show list |
| **Qty × Name** | `2*Sabun` | Qty=2, search for "Sabun", add |
| **Qty × Level × Name** | `3*2*Minyak` | Qty=3, Level=2, search "Minyak" |
| **Qty × Barcode** | `2*8991234567890` | Qty=2, scan barcode, exact match |

---

## <a name="validation"></a>? VALIDATION & ERROR HANDLING

### Pre-Payment Validation (TekanBayar)

```
? Invoice number exists
? At least 1 item added
? Total > 0 (if Nominal0="Tidak")
? Stock sufficient (if modulJualMinus="Tidak")
? No loss sales (if modulJualRugi="Tidak")

If ANY check fails:
?? MessageBox with error description
?? Highlight RED row (if item-specific)
?? Focus form to problem area
?? STOP - Do not proceed to payment
```

### Payment Validation (TekanSimpan)

```
? If Bank payment: nominal > 0 (required)
? If Hutang (Debt): customer must be selected
? If nominal = 0 (non-bank): Ask for confirmation

If validation fails:
?? MessageBox with error
?? Focus to problem control
?? STOP - Do not save
```

### Grid Cell Validation (DgvData_CellEndEdit)

```
NamaBarang column:
?? Check duplicate (if Kodebarangsama="Tidak")
?? Check exists in DB
?? Show error if not found
?? Clear cell, ready for retry

QTY column:
?? Validate numeric only (no letters)
?? Validate single decimal point
?? Default to "1" if invalid

Harga column:
?? Validate numeric
?? Parse to decimal
?? Default to "0" if invalid

DiskonPersen:
?? Validate 0-100%
?? Auto-calculate DiskonRp

DiskonRp:
?? Validate numeric
?? Auto-calculate DiskonPersen
```

### Stock Validation (CekStok)

```
If modulJualMinus = "Tidak":
  FOR each item in grid:
    ?? Get current STOK_TOKO or STOK_GUDANG
    ?? Add back any qty from existing transactions
    ?? Compare with order qtySat
    ?? If order > available:
    ?  ?? MessageBox: "Stock not sufficient"
    ?  ?? Highlight row RED
    ?  ?? Focus to grid
    ?  ?? RETURN FALSE (block save)
    ?? Check passed
  RETURN TRUE (allow save)
```

### Profit Validation (Cekjualrugi)

```
If modulJualRugi = "Tidak":
  FOR each item in grid:
    ?? Get HARGA_BELI from DB
    ?? Compare with item.Harga (selling price)
    ?? If Harga < HargaBeli:
    ?  ?? MessageBox: "Loss sale - [product]"
    ?  ?? Show both prices
    ?  ?? Highlight row RED
    ?  ?? Focus to grid
    ?  ?? RETURN FALSE (block save)
    ?? Check passed
  RETURN TRUE (allow save)
```

### Database Error Handling

```
[Prosessimpan - Transaction Save]
    ?? BEGIN TRANSACTION
    ?? TRY:
    ?  ?? Execute all INSERT/UPDATE
    ?  ?? COMMIT ?
    ?  ?? Success message
    ?? CATCH Exception:
       ?? ROLLBACK ?? (undo all changes)
       ?? MessageBox: "Error details"
       ?? Log error to system
       ?? User can retry
```

---

## <a name="dart-implementation"></a>?? IMPLEMENTATION CHECKLIST FOR DART

### Phase 1: Setup (Week 1)

- [ ] Create Flutter project: `flutter create appkasir_dart`
- [ ] Add dependencies:
  ```yaml
  dependencies:
    mysql1: ^0.21.0      # MySQL driver
    provider: ^6.0.0     # State management
    intl: ^0.19.0        # Number formatting
    get: ^4.6.0          # Navigation
    shared_preferences: ^2.2.0  # Local settings
  ```
- [ ] Setup MySQL connection module
- [ ] Configure Indonesian locale
- [ ] Setup file structure

### Phase 2: Models (Week 1-2)

Create 6 Dart model classes:
- [ ] `Penjualan` (transaction header - 34 fields)
- [ ] `PenjualanDetail` (line items - 18 fields)
- [ ] `Barang` (product master - 25 fields)
- [ ] `Pelanggan` (customer - 10 fields)
- [ ] `Karyawan` (employee - 5 fields)
- [ ] `PenjualanDitahan` (held transaction - 26 fields)

### Phase 3: Database Service (Week 2-3)

Create database layer:
- [ ] `DatabaseService` class with methods:

**READ Operations:**
- [ ] `getPenjualanHeader(faktur)` - Load header
- [ ] `getPenjualanDetails(faktur)` - Load detail rows
- [ ] `getBarangByName(keyword)` - Search product
- [ ] `getBarangByBarcode(barcode)` - Exact match
- [ ] `getPelangganList()` - All customers
- [ ] `getKaryawanList()` - All employees
- [ ] `getPenjualanDitahanList()` - All held

**WRITE Operations:**
- [ ] `savePenjualan(penjualan, details)` - With transaction
- [ ] `updatePenjualan(penjualan)` - Edit existing
- [ ] `deletePenjualan(faktur)` - Soft delete
- [ ] `savePenjualanDitahan(data)` - Hold transaction
- [ ] `deletePenjualanDitahan(faktur)` - Remove held

**Utility:**
- [ ] `generateInvoiceNumber()` - PJ-YYMMDD-XXXX
- [ ] `calculateStock(barangId, location)` - Current stock
- [ ] `recordStockHistory(barangId, qty, before, after)` - Audit trail
- [ ] `updateBarangStock(barangId, qty, location)` - Reduce stock

### Phase 4: Business Logic (Week 3)

Create calculation service:
- [ ] `calculateQtySatuan(qty, isi)`
- [ ] `calculateTotalHargaBeli(hargaBeli, isi, qty)`
- [ ] `calculateItemDiscount(hargaJual, qty, diskonRp)`
- [ ] `calculateLineTotal(hargaJual, qty, totalDiskon)`
- [ ] `calculateSubtotal(allItems)`
- [ ] `calculateDiscount(subtotal, diskonPersen)`
- [ ] `calculateTax(subtotal, diskon, pajakPersen)`
- [ ] `calculateGrandTotal(subtotal, diskon, pajak, kirim)`
- [ ] `calculateChange(nominal, total)`
- [ ] `calculateProfit(grandTotal, cogs, diskon)`

### Phase 5: UI Development (Week 4-6)

Create screens:
- [ ] `PenjualanListScreen` - List of transactions
- [ ] `PenjualanFormScreen` - Main sales form
  - [ ] Header section (invoice, date, customer, sales)
  - [ ] Grand total display (large, prominent)
  - [ ] Item search panel (barcode/name input)
  - [ ] Item dropdown list
  - [ ] DataTable (17 columns)
  - [ ] Discount section
  - [ ] Tax section
  - [ ] Action buttons (Bayar, Tahan, Panggil, etc.)
- [ ] `PaymentPanelScreen` - Payment modal
  - [ ] Payment method dropdown
  - [ ] Bank fields (conditional)
  - [ ] Amount input
  - [ ] Save/Cancel buttons
  - [ ] Due date picker (if tempo)
- [ ] `PenjualanDitahanScreen` - Hold/Recall list
  - [ ] Grid of held transactions
  - [ ] Process/Delete buttons

### Phase 6: Barcode Detection (Week 6)

Implement barcode detection:
- [ ] Timer-based input detection (200ms total)
- [ ] Character interval tracking (30ms threshold)
- [ ] EXACT MATCH search vs LIKE search
- [ ] Format parsing: qty*name, qty*level*name, etc.
- [ ] Auto quantity/satuan assignment

### Phase 7: Testing (Week 7)

- [ ] Unit tests for all formulas
- [ ] Integration tests for database
- [ ] UI tests for main flows
- [ ] End-to-end: Fresh transaction ? Save
- [ ] End-to-end: Edit transaction
- [ ] End-to-end: Hold & Recall
- [ ] Error scenario tests

---

## ?? SUMMARY TABLE

| Component | VB.NET Code | Lines | Complexity | Priority |
|---|---|---|---|---|
| **Form Load & Init** | Form_Penjualan_Load | ~80 | Medium | High |
| **Barcode Detection** | TxtNama_KeyDown, BarcodeTimer_Tick | ~400 | High | High |
| **Item Add to Grid** | TambahDataLangsung | ~150 | Medium | High |
| **Grid Edit & Calc** | DgvData_CellEndEdit | ~300 | High | High |
| **Total Calculation** | UpdateSemuaTotal, HitungNilaiSetiapBaris | ~150 | Medium | High |
| **Discount & Tax** | HitungDiskon, HitungPajak | ~100 | Low | High |
| **Payment Processing** | TekanBayar, AmbiuldataRekening | ~150 | Medium | High |
| **Database Save** | Simpanpenjualan, Simpanpenjualandetail, etc | ~500 | High | High |
| **Validation** | CekStok, Cekjualrugi, ComboBox_Validating | ~250 | High | Medium |
| **Hold & Recall** | Tekantahan, Tekanpanggil, AmbilDataDitahan | ~200 | Medium | Medium |
| **TOTAL** | **FormPenjualan.vb** | **3600+** | **High** | **?** |

---

## ?? QUICK START FOR DART DEVELOPER

### Step 1: Understand the Database
1. Read **Table 1: penjualan** (34 columns)
2. Read **Table 2: penjualan_detail** (18 columns)
3. Understand the 10 formulas in **Business Logic**

### Step 2: Create Core Models
```dart
// models/penjualan.dart
class Penjualan {
  String idPenjualan;      // PJ-YYMMDD-XXXX
  String fakturJual;
  DateTime tanggalPenjualan;
  // ... 31 more fields
}

class PenjualanDetail {
  int idDetail;
  String fakturJual;
  String idBarang;
  // ... 15 more fields
}
```

### Step 3: Create Database Service
```dart
// services/database_service.dart
class DatabaseService {
  Future<Penjualan> savePenjualan(Penjualan p, List<PenjualanDetail> details) {
    // BEGIN TRANSACTION
    // INSERT penjualan
    // INSERT all details
    // UPDATE stock
    // COMMIT or ROLLBACK
  }
}
```

### Step 4: Create UI Screen
```dart
// screens/penjualan_form_screen.dart
class PenjualanFormScreen extends StatefulWidget {
  // Similar to FormPenjualan layout
  // 17-column table
  // Real-time calculations
  // Barcode detection
  // Payment modal
}
```

### Step 5: Implement Formulas
```dart
// utils/penjualan_calculator.dart
class PenjualanCalculator {
  static Decimal calculateGrandTotal({
    required Decimal subtotal,
    required Decimal diskon,
    required Decimal pajak,
    required Decimal kirim,
  }) {
    return subtotal - diskon + pajak + kirim;
  }
  // ... 9 more methods
}
```

---

## ?? Reference Files

| File | Purpose | Size |
|------|---------|------|
| FormPenjualan.vb | Main sales form | 3600+ lines |
| FormPenjualan.Designer.vb | UI design | 1200+ lines |
| DatabaseModule.vb | Database layer | 500+ lines |
| FormUtama.vb | Main form (integration point) | 2000+ lines |
| FormPenjualanDitahan.vb | Hold/Recall feature | 150+ lines |

---

**Document Version:** 2.0.0  
**Last Updated:** Maret 2026  
**Status:** ? READY FOR DART IMPLEMENTATION  

---

