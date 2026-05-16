# Daftar Nama Kontrol Baru - UI Theme System

Gunakan nama ini untuk rename kontrol lama agar seragam di semua form.

## PANEL

1. **PanelHeader** - Header form (warna kategori otomatis)
2. **PanelInput** - Area input user (biru muda)
3. **PanelCari** - Area pencarian (warna biasa)
4. **PanelGrid** - Area DataGridView (menyatu dengan form)
5. **PanelFooter** - Tombol aksi di bawah (seamless)
6. **PnlBatas** - Border dekoratif (warna kategori form)

## GROUPBOX

1. **GBInput** - Area input user (biru muda)
2. **GBBayar** - Area bayar (hijau muda)
3. **GBTotal** - Area total/ringkasan (kuning muda)
4. **GBAction** - Area aksi/tombol (ungu muda)

## TEXTBOX

1. **TxtGrandtotal** - Grand Total (hitam + teks hijau kontras + bold)

## LABEL

1. **LblHeader*** - Header kategori form (warna kategori + putih + bold)
   - Contoh: `LblHeader`, `LblHeaderStok`, `LblHeaderDetail`
2. **LblTextJalanAtas** - Running text penjualan (kuning/amber kontras terbalik)
3. **LblHeaderPanel** - Header di panel (warna kategori + putih + bold)

## CARA RENAME KONTROL

### **Di Properties Editor:**
1. Klik kontrol yang ingin direname
2. Di Properties window, cari properti `(Name)`
3. Ganti nama sesuai konvensi di atas
4. Tekan Enter

### **Contoh Rename:**
- `Label1` (header form) -> `LblHeader`
- `Panel1` (area input) -> `PanelInput`
- `TextBox1` (grand total) -> `TxtGrandtotal`
- `GroupBox1` (area bayar) -> `GBBayar`

## HASIL SETELAH RENAME

Setelah rename kontrol, cukup panggil:
```vb
Private Sub Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
    ModuleTheme.TerapkanTheme(Me)
End Sub
```

**Semua kontrol akan otomatis mendapat warna tema yang sesuai tanpa kode tambahan!**

## DAFTAR RENAME KONTROL PER FORM (1Master)

### **TambahBarang**
- **Panel**: `Panel1` (bottom border) -> `PnlBatas`, `Panel2` (right border) -> `PnlBatas`, `Panel3` (left border) -> `PnlBatas`
- **GroupBox**: `GBInput`, `GBInput1`, `GBInput2`, `GBInput3`, `GBInput4`, `GBInput5` (SUDAH BENAR)
- **Label**: `LblHeader` (SUDAH BENAR)

### **TambahPelanggan**
- **Panel**: `Panel1` (bottom border) -> `PnlBatas`, `Panel2` (right border) -> `PnlBatas`, `Panel3` (left border) -> `PnlBatas`
- **GroupBox**: Tidak ada
- **Label**: `LblHeaderForm` (SUDAH BENAR)

### **TambahSupliyer**
- **Panel**: `Panel2` (area input) -> `PanelInput`
- **GroupBox**: Tidak ada
- **Label**: `LblHeaderForm` (SUDAH BENAR)

### **TambahSatuan**
- **Panel**: `Panel1` (bottom border) -> `PnlBatas`, `Panel2` (right border) -> `PnlBatas`, `Panel3` (left border) -> `PnlBatas`
- **GroupBox**: Tidak ada
- **Label**: `LblHeaderForm` (SUDAH BENAR)

### **FormCetakLabel**
- **Panel**: `Panel2` (area setting) -> `PanelInput`
- **GroupBox**: Tidak ada
- **Label**: `LblUtama` (SUDAH BENAR), `Label10` (header panel) -> `LblHeaderPanel`

### **FormCetakBarcode**
- **Panel**: `Panel1` (area preview) -> `PanelGrid`
- **GroupBox**: Tidak ada
- **Label**: `LblUtama` (SUDAH BENAR)

### **HistoriPembelianUC**
- **Panel**: `Panel1` (area grid) -> `PanelGrid`
- **GroupBox**: Tidak ada
- **Label**: Tidak ada

### **FormGeneralSetting**
- **GroupBox**: `GbReturBeli` -> `GBAction`, `GBReturJual` -> `GBAction`, `GBPenjualan` -> `GBAction`, `GBPembelian` -> `GBAction`, `GbGlobalTransaksi` -> `GBAction`
- **Panel**: Tidak ada
- **Label**: Tidak ada

### **FormBarang**
- **GroupBox**: `GBTambah` -> `GBAction`, `GbStokSaatIni` -> `GBTotal`
- **Panel**: Tidak ada
- **Label**: Tidak ada

## PRIORITAS RENAME

### **Tinggi (Penting Sekali):**
1. **Panel Border**: `Panel1/2/3` -> `PnlBatas` (6 form)
2. **GroupBox Action**: `GB*` -> `GBAction` (FormGeneralSetting, FormBarang)

### **Sedang:**
1. **Panel Input**: `Panel2` -> `PanelInput` (TambahSupliyer, FormCetakLabel)
2. **Panel Grid**: `Panel1` -> `PanelGrid` (FormCetakBarcode, HistoriPembelianUC)

### **Rendah (Opsional):**
1. **Label Panel Header**: `Label10` -> `LblHeaderPanel` (FormCetakLabel)

## TOTAL RENAME YANG DIBUTUHKAN

- **Panel**: 12 kontrol
- **GroupBox**: 7 kontrol  
- **Label**: 1 kontrol

**Total: 20 kontrol yang perlu direname untuk tema sempurna!**


selanjutnya cek datagridview yang ada di folder 1master