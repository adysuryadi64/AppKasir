# ============================================================
#  Build-Update.ps1
#  Buat AppKasir_Update.zip untuk AutoUpdater.NET secara otomatis.
#
#  Cara pakai:
#    1. Build Release di Visual Studio (ZIP dibuat otomatis)
#    2. Atau jalankan manual:
#       powershell -ExecutionPolicy Bypass -File Installer\Build-Update.ps1
#
#  Isi ZIP (struktur sama dengan folder EXE di client):
#    - KasirLancar.exe + DLL runtime
#    - Subfolder Resources\, 0Form\, 5Lap\, 8Uty\, dll
#
#  TIDAK dimasukkan ke ZIP (file milik user / data lokal):
#    - config.bin, database.json, license.ini, dll (data user)
#    - logo.png, toko.jpg, gudang.jpg (foto toko milik user)
#    - mysql.exe, mysqldump.exe, WebView2 installer (tools besar)
#    - *.pdb, *.xml, *.tmp (debug/doc)
#    - Folder Backup\, Printer Driver Software\, dll
# ============================================================

param(
    [switch]$NonInteractive  # Set otomatis saat dipanggil dari MSBuild
)

Set-Location $PSScriptRoot

# ── Konfigurasi ───────────────────────────────────────────────────
$DebugDir    = Join-Path $PSScriptRoot "..\bin\Debug"
$ReleaseDir  = Join-Path $PSScriptRoot "..\bin\Release"
$OutputDir   = Join-Path $PSScriptRoot "Output"
$ZipName     = "AppKasir_Update.zip"
$ZipOutput   = Join-Path $OutputDir $ZipName

# Folder sumber: HANYA bin\Release — tidak ada fallback ke Debug
$SourceDir = $ReleaseDir
if (-not (Test-Path (Join-Path $ReleaseDir "KasirLancar.exe"))) {
    Write-Host "  [ERROR] bin\Release\KasirLancar.exe tidak ditemukan!" -ForegroundColor Red
    Write-Host "          Jalankan Build Release terlebih dahulu," -ForegroundColor Red
    Write-Host "          atau gunakan Publish-Release.ps1 yang otomatis build." -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

# Baca versi dari AssemblyInfo.vb
$AssemblyInfoPath = Join-Path $PSScriptRoot "..\My Project\AssemblyInfo.vb"
$versi = "unknown"
if (Test-Path $AssemblyInfoPath) {
    $match = Select-String -Path $AssemblyInfoPath -Pattern 'AssemblyVersion\("([^"]+)"\)'
    if ($match) { $versi = $match.Matches[0].Groups[1].Value }
}

# ── File config milik user — JANGAN ditimpa saat update ──────────
# ZipExtractor akan overwrite semua file di folder EXE.
# File ini dikecualikan dari ZIP agar data user tidak hilang.
$ExcludeFiles = @(
    "config.bin",
    "database.json",
    "license.ini",
    "config_printer.ini",
    "ConfigLabelBarang.ini",
    "pengaturan_cetak.ini",
    "perilaku_cetak.ini",
    "printer.ini",
    "logo.png",
    "toko.jpg",
    "gudang.jpg",
    "mysql.exe",
    "mysqldump.exe",
    "MicrosoftEdgeWebView2RuntimeInstaller.exe",
    "MicrosoftEdgeWebView2RuntimeInstallerX64.exe",
    "EnvDTE.dll",
    "stdole.dll",
    "_dashboard_tmp.html"
)

# ── Ekstensi yang dikecualikan (debug/doc) ────────────────────────
$ExcludeExtensions = @(".pdb", ".tmp", ".log")

# ── Folder yang dikecualikan sepenuhnya ──────────────────────────
$ExcludeFolders = @(
    "Backup",
    "database_Default_Master",
    "Printer Driver Software",
    "Logs",
    "KasirLancar.exe.WebView2"
)

# ── Mulai ─────────────────────────────────────────────────────────
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  KASIR LANCAR - Build Update ZIP" -ForegroundColor Cyan
Write-Host "  Versi : $versi" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# Cek folder sumber ada
if (-not (Test-Path $SourceDir)) {
    Write-Host "ERROR: Folder bin\Release dan bin\Debug tidak ditemukan!" -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

$exePath = Join-Path $SourceDir "KasirLancar.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "ERROR: KasirLancar.exe tidak ditemukan!" -ForegroundColor Red
    Write-Host "       Lakukan Build Release di Visual Studio terlebih dahulu." -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

# ── Verifikasi versi EXE vs update.xml ────────────────────────────
$UpdateXmlFullPath = Join-Path $PSScriptRoot "..\..\update.xml"
$exeVi = [System.Diagnostics.FileVersionInfo]::GetVersionInfo((Resolve-Path $exePath).Path)
$exeVer = $exeVi.FileVersion

$xmlVer = ""
if (Test-Path $UpdateXmlFullPath) {
    $m = Select-String -Path $UpdateXmlFullPath -Pattern '<version>([^<]+)</version>'
    if ($m) { $xmlVer = $m.Matches[0].Groups[1].Value }
}

if ($xmlVer -ne "" -and $exeVer -ne $xmlVer) {
    Write-Host "" -ForegroundColor Red
    Write-Host "  !!! VERSI TIDAK COCOK !!!" -ForegroundColor Red
    Write-Host "  EXE version  : $exeVer" -ForegroundColor Red
    Write-Host "  update.xml   : $xmlVer" -ForegroundColor Red
    Write-Host "  Pastikan build Release sudah menggunakan versi terbaru." -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
} else {
    Write-Host "  Versi EXE cocok dengan update.xml: $exeVer" -ForegroundColor Green
}

# Buat folder Output jika belum ada
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Hapus ZIP lama jika ada
if (Test-Path $ZipOutput) {
    Remove-Item $ZipOutput -Force
    Write-Host "  ZIP lama dihapus." -ForegroundColor DarkGray
}

# ── Scan file yang akan dimasukkan ───────────────────────────────
Write-Host "  Scanning $SourceDir ..." -ForegroundColor Yellow

$sourceFullPath = (Resolve-Path $SourceDir).Path
$allFiles = Get-ChildItem -Path $SourceDir -Recurse -File

$included = [System.Collections.Generic.List[object]]::new()
$skipped  = [System.Collections.Generic.List[string]]::new()

foreach ($f in $allFiles) {
    # Cek apakah ada di folder yang dikecualikan
    $relativePath = $f.FullName.Substring($sourceFullPath.Length).TrimStart('\')
    $topFolder = $relativePath.Split('\')[0]

    # Kalau file ada di subfolder yang dikecualikan
    $inExcludedFolder = $false
    foreach ($ef in $ExcludeFolders) {
        if ($relativePath.StartsWith($ef + '\') -or $topFolder -eq $ef) {
            $inExcludedFolder = $true
            break
        }
    }
    if ($inExcludedFolder) {
        $skipped.Add("  [FOLDER] $relativePath")
        continue
    }

    # Cek nama file
    if ($ExcludeFiles -contains $f.Name) {
        $skipped.Add("  [CONFIG] $relativePath")
        continue
    }

    # Cek ekstensi
    if ($ExcludeExtensions -contains $f.Extension.ToLower()) {
        $skipped.Add("  [DEBUG]  $relativePath")
        continue
    }

    $included.Add([PSCustomObject]@{
        FullPath     = $f.FullName
        RelativePath = $relativePath
    })
}

# ── Tambahkan folder Database dari source project ─────────────────
# Folder Database\ (SchemaDef, migrasi SQL) tidak di-copy saat build,
# harus disertakan manual agar FormSchemaValidator bisa menemukan SchemaDef
# saat runtime di komputer client.
$DatabaseSourceDir = Join-Path $PSScriptRoot "..\Database"
if (Test-Path $DatabaseSourceDir) {
    $dbFiles = Get-ChildItem -Path $DatabaseSourceDir -Recurse -File
    $dbSourceFull = (Resolve-Path $DatabaseSourceDir).Path
    foreach ($f in $dbFiles) {
        $relativePath = "Database\" + $f.FullName.Substring($dbSourceFull.Length).TrimStart('\')
        $included.Add([PSCustomObject]@{
            FullPath     = $f.FullName
            RelativePath = $relativePath
        })
    }
    Write-Host "  Database\  : $($dbFiles.Count) file ditambahkan dari source project" -ForegroundColor Cyan
}

Write-Host "  File akan dimasukkan : $($included.Count)" -ForegroundColor Green
Write-Host "  File dikecualikan    : $($skipped.Count)" -ForegroundColor DarkGray
Write-Host ""

# ── Buat ZIP ─────────────────────────────────────────────────────
Write-Host "  Membuat ZIP ..." -ForegroundColor Yellow

# Buat folder temp untuk staging file sebelum di-zip
$tempDir = Join-Path $env:TEMP "AppKasir_Update_Staging"
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }
New-Item -ItemType Directory -Path $tempDir | Out-Null

Add-Type -AssemblyName System.IO.Compression.FileSystem

$count = 0
foreach ($item in $included) {
    $destPath = Join-Path $tempDir $item.RelativePath
    $destFolder = Split-Path $destPath -Parent
    if (-not (Test-Path $destFolder)) {
        New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
    }
    Copy-Item -Path $item.FullPath -Destination $destPath -Force
    $count++
    if ($count % 50 -eq 0) {
        Write-Host "    ... staging $count / $($included.Count) file" -ForegroundColor DarkGray
    }
}

Write-Host "    Staging selesai: $count file" -ForegroundColor DarkGray
Write-Host "    Mengkompresi ke ZIP ..." -ForegroundColor DarkGray

[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, $ZipOutput, [System.IO.Compression.CompressionLevel]::Optimal, $false)

# Bersihkan temp
Remove-Item $tempDir -Recurse -Force

# ── Hasil ─────────────────────────────────────────────────────────
$zipInfo = Get-Item $ZipOutput
$zipSizeMB = [math]::Round($zipInfo.Length / 1MB, 2)

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "  BERHASIL!" -ForegroundColor Green
Write-Host "  Output  : $($zipInfo.FullName)" -ForegroundColor Green
Write-Host "  Ukuran  : $zipSizeMB MB" -ForegroundColor Green
Write-Host "  Versi   : $versi" -ForegroundColor Green
Write-Host "  File    : $count file dikemas" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Langkah selanjutnya: (otomatis jika dijalankan via Publish-Release.ps1)" -ForegroundColor Cyan
Write-Host "  1. Git commit + push update.xml + AssemblyInfo.vb" -ForegroundColor White
Write-Host "  2. Buat GitHub Release + upload ZIP" -ForegroundColor White
Write-Host ""

# Tampilkan file yang dikecualikan jika mau debug
if (-not $NonInteractive) {
    Write-Host ""
    Write-Host "  File dikecualikan:" -ForegroundColor DarkGray
    $skipped | ForEach-Object { Write-Host $_ -ForegroundColor DarkGray }
    Write-Host ""
}
