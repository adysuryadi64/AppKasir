# Panduan Build Installer Kasir Lancar

## Struktur Folder

```
Installer\
├── Build-Installer.ps1         ← Script utama untuk build installer
├── create_installer_images.ps1 ← Generate banner & logo wizard
├── KasirLancar_Setup.iss       ← Script Inno Setup
├── installer_banner.bmp        ← Gambar sidebar wizard (164x314 px)
├── installer_logo.bmp          ← Logo pojok kanan atas wizard (55x55 px)
├── Kasir lancar.ico            ← Icon installer
└── Output\
    └── KasirLancar_Setup_vXX.X.XXXX.XX.exe  ← Hasil build
```

## Cara Build Installer

Jalankan perintah berikut dari folder root project maupun dari mana saja:

```powershell
powershell -ExecutionPolicy Bypass -File "Installer\Build-Installer.ps1"
```

Atau klik kanan `Build-Installer.ps1` → **Run with PowerShell**.

## Yang Dilakukan Script Otomatis

1. Scan folder `bin\Debug` dan hitung semua file (termasuk subfolder baru)
2. Generate `[Files]` section di `KasirLancar_Setup.iss` secara otomatis
3. Regenerate gambar banner & logo dengan versi terbaru
4. Compile installer menggunakan Inno Setup 6

## Aturan Folder bin\Debug

| Folder | Perlakuan |
|--------|-----------|
| `Backup\` | Di-copy ke `{app}\Backup` (rekursif) |
| `Printer Driver Software\` | Di-copy ke `{app}\Printer Driver Software` + installer ke `{tmp}` |
| `Logs\` | Di-skip (folder runtime) |
| Subfolder lainnya | Di-copy rekursif ke `{app}\nama_folder` |

## Jenis Instalasi

- **Instalasi Baru** — Memasang semua komponen: AppServ, ReportViewer, MySQL Connector, POS Printer Driver
- **Update** — Hanya mengganti file aplikasi, komponen lain dilewati. Folder instalasi diverifikasi sebelum lanjut.

## Prasyarat

- Inno Setup 6 terinstal di `C:\Program Files (x86)\Inno Setup 6\`
- PowerShell 5.1 atau lebih baru
- File `bin\Debug\KasirLancar.exe` sudah ada (sudah di-build dari Visual Studio)

## Update Versi

Edit baris berikut di `KasirLancar_Setup.iss`:

```
#define MyAppVersion "25.3.2026.18"
```

Format: `YY.Bulan.Tahun.Revision` — sesuaikan dengan `ApplicationVersion` di `AppKasir.vbproj`.
