# Dokumentasi Modul 6Print — Sistem Cetak AppKasir

**Diperbarui:** April 2026
**Folder:** `AppKasir\6Print`
**Versi:** 3.3

---

## 📊 STATUS KESELURUHAN

```
Infrastruktur Cetak        ████████████████████  100%
Form Pengaturan Printer    ████████████████████  100%
Cetak Penjualan            ████████████████████  100%
Cetak Pembelian            ████████████████████  100%
Cetak Retur Jual           ████████████████████  100%
Cetak Retur Beli           ████████████████████  100%
Cetak Surat Jalan          ████████████████████  100%
Cetak Bayar Hutang         ████████████████████  100%
Cetak Bayar Piutang        ████████████████████  100%
Cetak Slip Gaji            ████████████████████  100%
Cetak Bon Karyawan         ████████████████████  100%
Cetak Transfer Barang      ████████████████████  100%
Cetak Transfer Stok        ████████████████████  100%
Cetak Stok Opname          ████████████████████  100%
Cetak Laporan Kas          ████████████████████  100%
─────────────────────────────────────────────────
TOTAL                      ████████████████████  100%
```

---

## 📋 RINGKASAN STATUS KOMPONEN

| Komponen | Status | Keterangan |
|----------|--------|------------|
| `pengaturan_cetak.ini` | ✅ | Format baru, ganti dari `printer.ini` |
| `ModuleKonfigurasi.vb` | ✅ | API class per jenis printer + buka laci semua mode |
| `FormPengaturanPrinter.vb` | ✅ | 11 tab, UI dinamis, laci kasir dikontrol per transaksi |
| `PrinterEscPos.vb` | ✅ | ESCPOS_NET 3.0, USB/Network/Serial, logo 1-bit |
| `RawPrinterHelper.vb` | ✅ | Kirim raw bytes via winspool — dipakai buka laci GDI+ |
| Cetak Penjualan | ✅ | ESC/POS + GDI+ + Inkjet + Monitor + PDF + buka laci |
| Cetak Pembelian | ✅ | ESC/POS + GDI+ + Inkjet + PDF + buka laci |
| Cetak Retur Jual | ✅ | ESC/POS + GDI+ + Inkjet + Monitor + PDF + buka laci |
| Cetak Retur Beli | ✅ | ESC/POS + GDI+ + Inkjet + Monitor + PDF + buka laci |
| Cetak Surat Jalan | ✅ | ESC/POS dot + GDI+ dot + Inkjet + PDF |
| Cetak Bayar Hutang | ✅ | ESC/POS + GDI+ + Inkjet + PDF + buka laci |
| Cetak Bayar Piutang | ✅ | ESC/POS + GDI+ + Inkjet + PDF + buka laci |
| Cetak Slip Gaji | ✅ | ESC/POS thermal + GDI+ + Inkjet + PDF |
| Cetak Bon Karyawan | ✅ | ESC/POS thermal+dot + GDI+ + Inkjet + PDF + buka laci |
| Cetak Transfer Barang | ✅ | ESC/POS dot + GDI+ dot + Inkjet + PDF |
| Cetak Transfer Stok | ✅ | ESC/POS dot + GDI+ dot + Inkjet + PDF |
| Cetak Stok Opname | ✅ | ESC/POS dot + GDI+ dot + Inkjet + PDF |
| Cetak Laporan Kas | ✅ | ESC/POS dot + GDI+ + Inkjet + PDF + buka laci |

---

## 🚨 PRIORITAS SAAT INI

| # | Task | File | Status |
|---|------|------|--------|
| 1 | Migrasi `FormPenjualan` baca key lama `JenisPrinterJual` | `2Trans\FormPenjualan.vb` | ⚠️ Belum |
| 2 | Migrasi `FormReturPenjualan` baca key lama | `2Trans\FormReturPenjualan.vb` | ⚠️ Belum |
| 3 | Migrasi `FormUtama.AmbilKomputer()` key `StatusComp` → `StatusKomputer` | `0Form\FormUtama.vb` | ⚠️ Belum |
| 4 | Hubungkan `ModulePrinterBeli` ke `FormPembelian` (ganti `NotaPembelian` lama) | `2Trans\FormPembelian.vb` | ⚠️ Belum |

---

## 🗂️ DAFTAR FILE

### `6Print\` (root)

| File | Keterangan |
|------|------------|
| `ModuleKonfigurasi.vb` | API konfigurasi + `BukaLaciKasir` semua mode |
| `FormPengaturanPrinter.vb` | UI 11 tab, kontrol dinamis per mode & transaksi |
| `PrinterEscPos.vb` | ESCPOS_NET 3.0 — USB, Network, Serial, logo |
| `RawPrinterHelper.vb` | Kirim raw bytes via winspool (buka laci GDI+) |

### Subfolder per Transaksi (pola seragam)

```
CetakXxx\
    ModulePrinterXxx.vb              ← entry point + data Xxx_* variables
    EscPosCetakXxx*.vb               ← ESC/POS thermal + dot matrix
    GdiCetakXxx*.vb                  ← GDI+ thermal/dot + sumber PDF
    ModuleCetakXxxInkjet.vb          ← inkjet/laser
    ModuleCetakXxxPdf.vb             ← export PDF
```

---

## � BUKnA LACI KASIR

### Transaksi yang Butuh Laci

| Transaksi | Alasan |
|-----------|--------|
| Jual | Uang masuk ke laci |
| Beli | Uang keluar dari laci |
| ReturJual | Uang dikembalikan ke pelanggan |
| ReturBeli | Uang diterima dari supplier |
| BayarHutang | Pembayaran tunai |
| BayarPiutang | Penerimaan tunai |
| Bon | Bon karyawan melibatkan kas |
| Laporan | Rekap kas harian — buka laci saat tutup kasir |

Transaksi lain (SuratJalan, TransferBarang, TransferStok, StokOpname, Gaji) tidak butuh laci.

### Cara Kerja `BukaLaciKasir(transaksi)`

```
1. Baca KodeLaciKasir dari INI
2. Jika "(Tidak Ada)" → Exit Sub (tidak mengganggu aplikasi)
3. Jika ada nama printer:
   a. Network printer → kirim via TCP (PrinterEscPos)
   b. GDI+ mode      → kirim raw bytes via RawPrinterHelper.KirimKePrinter()
   c. ESC/POS mode   → kirim via PrinterEscPos
4. Fallback: kirim via Serial port langsung
5. Semua jalur dibungkus Try/Catch — gagal tidak menghentikan cetak
```

### UI Pengaturan Laci

Panel `grpLaci` hanya tampil untuk transaksi yang relevan (dikontrol `TransaksiButuhLaci(key)`).
Panel laci tampil di semua mode cetak (ESC/POS maupun GDI+) karena keduanya sudah support buka laci.

### Kode Pin Laci

| Kode | Pin | Keterangan |
|------|-----|------------|
| OPTION 1 | Pin 2 | Standar — paling umum |
| OPTION 2 | Pin 2 | Standar alternatif |
| OPTION 3 | Pin 5 | Untuk printer tertentu |
| OPTION 4 | Pin 5 | Untuk printer tertentu |

---

## ⚙️ FILE KONFIGURASI: `pengaturan_cetak.ini`

File disimpan di direktori yang sama dengan `AppKasir.exe`.
Dibaca setiap kali cetak — perubahan langsung berlaku tanpa restart.

### Format Key

```
[Transaksi]_[Jenis]_[Field]
```

Contoh: `Jual_Thermal_NamaPrinter`, `Beli_DotGdi_LebarKertas`

### Contoh Isi Lengkap (Penjualan)

```ini
Jual_JenisPrinter=Printer Thermal
Jual_DefaultCetak=Thermal_ESC

; Thermal
Jual_Thermal_ModeCetak=ESC/POS (Raw)
Jual_Thermal_TipeKoneksi=USB / Windows Spooler
Jual_Thermal_NamaPrinter=XP-80C
Jual_Thermal_IpAddress=192.168.1.50
Jual_Thermal_NetworkPort=9100
Jual_Thermal_UkuranKertas=POS-80 (80mm)
Jual_Thermal_LebarKertas=80
Jual_Thermal_BatasKiri=0
Jual_Thermal_JarakBaris=4
Jual_Thermal_JarakBarisEsc=0
Jual_Thermal_PotongOtomatisEsc=True
Jual_Thermal_PotongOtomatisGdi=True
Jual_Thermal_JumlahCetakEsc=1
Jual_Thermal_JumlahCetakGdi=1
Jual_Thermal_ModelStruk=Model 4 Lengkap Tanpa Logo
Jual_Thermal_PortLaciKasir=COM1
Jual_Thermal_KodeLaciKasir=OPTION 1
Jual_Thermal_DpiCetak=203
Jual_Thermal_FontJudul=Arial Narrow
Jual_Thermal_UkuranJudul=12
Jual_Thermal_FontKeterangan=Arial Narrow
Jual_Thermal_UkuranKeterangan=9
Jual_Thermal_FontIsi=Courier New
Jual_Thermal_UkuranIsi=8
Jual_Thermal_FontFooter=Arial Narrow
Jual_Thermal_UkuranFooter=8
Jual_Thermal_TampilFooter1=True
Jual_Thermal_TampilFooter2=True
Jual_Thermal_TampilFooter3=True

; Dot Matrix
Jual_DotMatrix_NamaPrinter=EPSON LX-310
Jual_DotMatrix_ModeCetak=GDI+ (Windows Print)
Jual_DotGdi_LebarKertas=80
Jual_DotGdi_UkuranKertas=Continuous Form (Auto)
Jual_DotGdi_BatasKiri=2
Jual_DotGdi_JarakBaris=2
Jual_DotGdi_UkuranFont=9
Jual_DotGdi_JumlahCetak=1
Jual_DotGdi_ModelStruk=Model 1 Lengkap
Jual_DotGdi_TampilFooter1=True
Jual_DotEsc_LebarKertas=80
Jual_DotEsc_BatasKiri=2
Jual_DotEsc_JarakBaris=1
Jual_DotEsc_JumlahCetak=1
Jual_DotEsc_ModelStruk=Model 1 Lengkap
Jual_DotEsc_TampilFooter1=True

; Inkjet / Laser
Jual_Inkjet_NamaPrinter=HP LaserJet
Jual_Inkjet_UkuranKertas=A4
Jual_Inkjet_Orientasi=Portrait
Jual_Inkjet_JumlahCetak=1
Jual_Inkjet_MarginAtas=10
Jual_Inkjet_MarginBawah=10
Jual_Inkjet_MarginKiri=15
Jual_Inkjet_MarginKanan=10
Jual_Inkjet_FontJudul=Arial
Jual_Inkjet_UkuranJudul=12
Jual_Inkjet_FontIsi=Arial
Jual_Inkjet_UkuranIsi=10
Jual_Inkjet_TampilFooter1=True
Jual_Inkjet_ModelNota=Lengkap
Jual_Inkjet_TampilLogo=True
Jual_Inkjet_TampilTandaTangan=True
Jual_Inkjet_PctKolomNo=5
Jual_Inkjet_PctKolomQty=8
Jual_Inkjet_PctKolomHarga=15
Jual_Inkjet_PctKolomDiskon=10

; Monitor & PDF
Jual_Monitor_TampilFooter1=True
Jual_PDF_TampilFooter1=True
```

> **Catatan:** `KodeLaciKasir` disimpan di key Thermal dan dibaca juga oleh `KonfigurasiDotMatrix` — laci dikonfigurasi satu tempat, berlaku untuk semua mode cetak.

---

## 🖨️ FORM PENGATURAN PRINTER

### Cara Buka

```vb
' Dari menu utama — semua tab
Dim frm As New FormPengaturanPrinter()
frm.ShowDialog()

' Dari form transaksi — satu tab saja
Dim frm As New FormPengaturanPrinter()
frm.FilterTab = "Jual"
frm.ShowDialog()
```

### 11 Tab

`Jual`, `Beli`, `ReturJual`, `ReturBeli`, `SuratJalan`, `TransferBarang`, `BayarHutang`, `BayarPiutang`, `Gaji`, `Bon`, `Laporan`

### Kontrol Terpisah per Mode (ESC/POS vs GDI+)

| Kontrol | ESC/POS | GDI+ |
|---------|---------|------|
| Potong kertas | `chkPotongEsc_[key]` → `PotongOtomatisEsc` | `chkPotongGdi_[key]` → `PotongOtomatisGdi` |
| Jumlah cetak | `numCopiesEsc_[key]` → `JumlahCetakEsc` | `numCopiesGdi_[key]` → `JumlahCetakGdi` |
| Jarak baris | `txtJarakEsc_[key]` → `JarakBarisEsc` | `txtJarakGdi_[key]` → `JarakBaris` |
| DPI | — | `txtDpiCetak_[key]` → `DpiCetak` |
| Panel font | — (hidden) | `grpFont` (tampil) |
| Laci kasir | `grpLaci_[key]` | `grpLaci_[key]` |

> **Laci kasir** tampil di semua mode (ESC/POS dan GDI+) untuk transaksi yang relevan.
> Untuk transaksi yang tidak butuh laci, panel ini disembunyikan otomatis.

### Nama Kontrol Footer

| Panel | Nama Kontrol |
|-------|-------------|
| Thermal | `chkFooterT1/T2/T3_[key]` |
| Inkjet | `chkFooterI1/I2/I3_[key]` |
| Monitor | `chkFooterM1/M2/M3_[key]` |
| PDF | `chkFooterP1/P2/P3_[key]` |
| Dot GDI+ | `chkF1DotGdi/chkF2DotGdi/chkF3DotGdi_[key]` |
| Dot ESC/P | `chkF1DotEsc/chkF2DotEsc/chkF3DotEsc_[key]` |

---

## 📦 KONFIGURASI (ModuleKonfigurasi.vb)

### Class yang Tersedia

```vb
Dim cfg    As New KonfigurasiThermal("Jual")
Dim cfgDot As New KonfigurasiDotMatrix("Jual")
Dim cfgInk As New KonfigurasiInkjet("Jual")
Dim cfgMon As New KonfigurasiMonitor("Jual")
Dim cfgPdf As New KonfigurasiPDF("Jual")
```

### Field Penting KonfigurasiThermal

```vb
NamaPrinter, JenisPrinter, ModeCetak, TipeKoneksi
IpAddress, NetworkPort
UkuranKertas, LebarKertas, BatasKiri
JarakBaris          ' GDI+ (px)
JarakBarisEsc       ' ESC/POS (baris)
PotongOtomatis      ' ESC/POS
PotongOtomatisGdi   ' GDI+
JumlahCetak         ' ESC/POS
JumlahCetakGdi      ' GDI+
ModelStruk
PortLaciKasir, KodeLaciKasir, PinLaciKasir
DpiCetak            ' GDI+ only
FontJudul, UkuranJudul, FontKeterangan, UkuranKeterangan
FontIsi, UkuranIsi, FontFooter, UkuranFooter
TampilFooter1, TampilFooter2, TampilFooter3
```

### Field Penting KonfigurasiDotMatrix

```vb
NamaPrinter, ModeCetak
LebarKertas, BatasKiri, JarakBaris, UkuranFont
JumlahCetak, ModelStruk, UkuranKertas
KodeLaciKasir, PinLaciKasir   ' dibaca dari key Thermal_KodeLaciKasir
TampilFooter1, TampilFooter2, TampilFooter3
```

### API Utama

```vb
BacaPengaturanPrinter(transaksi, field, default)
TulisPengaturanPrinter(transaksi, field, nilai)
BukaLaciKasir(transaksi)          ' support ESC/POS, GDI+, Network, Serial
MuatSemuaPengaturan()             ' panggil saat app start
TransaksiButuhLaci(key) As Boolean ' helper UI — true untuk 8 transaksi
```

---

## 📊 STATUS CETAK PER TRANSAKSI

| Transaksi | ESC/POS Thermal | ESC/POS Dot | GDI+ Thermal | GDI+ Dot | Inkjet | Monitor | PDF | Buka Laci |
|-----------|:--------------:|:-----------:|:------------:|:--------:|:------:|:-------:|:---:|:---------:|
| Penjualan | ✅ 15 model | ✅ | ✅ 15 model | ✅ | ✅ | ✅ RDLC | ✅ | ✅ |
| Pembelian | ✅ 3 model | ✅ | ✅ 3 model | ✅ | ✅ | — | ✅ | ✅ |
| Retur Jual | ✅ 4 model | ✅ | ✅ 4 model | ✅ | ✅ | ✅ | ✅ | ✅ |
| Retur Beli | ✅ 3 model | ✅ | ✅ 3 model | ✅ | ✅ | ✅ | ✅ | ✅ |
| Surat Jalan | — | ✅ | — | ✅ | ✅ | — | ✅ | — |
| Bayar Hutang | ✅ | ✅ | ✅ | ✅ | ✅ | — | ✅ | ✅ |
| Bayar Piutang | ✅ | ✅ | ✅ | ✅ | ✅ | — | ✅ | ✅ |
| Slip Gaji | ✅ | — | ✅ | — | ✅ | — | ✅ | — |
| Bon Karyawan | ✅ | ✅ | ✅ | ✅ | ✅ | — | ✅ | ✅ |
| Transfer Barang | — | ✅ | — | ✅ | ✅ | — | ✅ | — |
| Transfer Stok | — | ✅ | — | ✅ | ✅ | — | ✅ | — |
| Stok Opname | — | ✅ | — | ✅ | ✅ | — | ✅ | — |
| Laporan Kas | — | ✅ | ✅ | ✅ | ✅ | — | ✅ | ✅ |

---

## 🔄 ALUR CETAK (contoh Penjualan)

```
FormPenjualan → ModulePrinterJual.CetakPenjualan(noFaktur)
    │
    ├─ MuatDataPenjualan() → query DB → isi Jual_* variables
    │
    └─ Baca JenisPrinter + ModeCetak dari pengaturan_cetak.ini
            │
            ├─ Thermal + ESC/POS → EscPosCetakjualThermalMatrik.CetakThermal()
            │                       └─ selesai → BukaLaciKasir("Jual")
            ├─ Thermal + GDI+    → GdiCetakJualThermalMatrik.Cetak()
            │                       └─ selesai → BukaLaciKasir("Jual")
            ├─ Dot Matrix        → EscPosCetakjualThermalMatrik.CetakDotMatrix()
            │                       └─ selesai → BukaLaciKasir("Jual")
            ├─ Inkjet            → ModuleCetakJualInkjet.CetakNota()
            ├─ Monitor           → FormMonitorRDLC (RDLC ReportViewer)
            └─ PDF               → ModuleCetakJualPdf → iTextSharp
```

---

## 🗺️ RENCANA KE DEPAN

### Prioritas Tinggi

| # | Task | File |
|---|------|------|
| 1 | Migrasi `FormPenjualan` key lama `JenisPrinterJual` | `2Trans\FormPenjualan.vb` |
| 2 | Migrasi `FormReturPenjualan` key lama | `2Trans\FormReturPenjualan.vb` |
| 3 | Migrasi `FormUtama.AmbilKomputer()` `StatusComp` → `StatusKomputer` | `0Form\FormUtama.vb` |
| 4 | Hubungkan `ModulePrinterBeli` ke `FormPembelian` (ganti `NotaPembelian` lama) | `2Trans\FormPembelian.vb` |

### Prioritas Sedang

| # | Task |
|---|------|
| 5 | Migrasi semua laporan di `5Lap\` ke `KonfigurasiInkjet` |
| 6 | Preview sebelum cetak (checkbox per transaksi) |
| 7 | Validasi nama printer ada di sistem sebelum cetak |
| 8 | Import/export `pengaturan_cetak.ini` (backup & restore) |

### Prioritas Rendah

| # | Task |
|---|------|
| 9 | Barcode/QR di struk (ZXing.Net sudah terinstall) |
| 10 | Export Excel data (ClosedXML sudah terinstall) |

---

## 📚 LIBRARY

| Library | Versi | Digunakan untuk |
|---------|-------|-----------------|
| `ESCPOS_NET` | 3.0.0 | ESC/POS thermal/dot matrix |
| `SixLabors.ImageSharp` | 3.1.12 | Logo (dipakai ESCPOS_NET) |
| `iTextSharp` | 5.5.13.4 | Export PDF nota |
| `Microsoft.ReportViewer` | 150.1652.0 | RDLC Monitor |
| `SuperSimpleTcp` | 2.4.0 | Network printer TCP |
| `System.IO.Ports` | 6.0.0 | Serial port laci kasir |
| `System.Management` | built-in | WMI — baca port fisik printer |
| `ZXing.Net` | 0.16.11 | Barcode/QR — tersedia, belum dipakai |
| `ClosedXML` | 0.105.0 | Export Excel — tersedia, belum dipakai |

---

## 📝 CATATAN PENTING

1. **`pengaturan_cetak.ini`** dibaca setiap cetak — perubahan langsung berlaku tanpa restart.

2. **Buka laci GDI+** — pakai `RawPrinterHelper.KirimKePrinter()` dengan `DataType="RAW"`, bypass GDI rendering, kirim byte `ESC p` langsung ke printer. Tidak perlu driver khusus.

3. **`KodeLaciKasir`** disimpan satu tempat (key `Thermal_KodeLaciKasir`) dan dibaca oleh semua class konfigurasi termasuk `KonfigurasiDotMatrix`.

4. **Panel laci kasir** di form pengaturan dikontrol oleh `TransaksiButuhLaci(key)` — otomatis tersembunyi untuk transaksi yang tidak relevan.

5. **Kontrol ESC/POS vs GDI+** — `chkPotongEsc/Gdi`, `numCopiesEsc/Gdi`, `txtJarakEsc/Gdi` adalah kontrol terpisah. Jangan gunakan nama lama.

6. **Font** hanya berlaku di mode GDI+. Section font tersembunyi otomatis saat ESC/POS dipilih.

7. **Logo** — letakkan `logo.png` atau `logo.jpg` di folder `.exe`. ESC/POS: max 384px (80mm) / 256px (58mm). GDI+: proporsional.

8. **`Flush()`** di `PrinterEscPos` WAJIB dipanggil — semua method hanya buffer, `Flush()` yang kirim ke printer.

9. **Surat Jalan, Transfer Barang, Transfer Stok, Stok Opname** — tidak punya jalur thermal karena dokumen ini untuk dot matrix/inkjet. Desain yang benar.

10. **Slip Gaji** — tidak punya dot matrix karena format kolom pendapatan/potongan tidak cocok untuk lebar 80 karakter. Desain yang benar.
