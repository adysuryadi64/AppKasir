# ============================================================
#  Build-Installer.ps1
#  Auto-scan bin\Debug, generate [Files] section, lalu compile
#  Jalankan: powershell -ExecutionPolicy Bypass -File Installer\Build-Installer.ps1
# ============================================================

# Pindah ke folder script agar semua path relatif bekerja dengan benar
Set-Location $PSScriptRoot

$DebugDir   = "..\bin\Debug"
$IssTemplate = "KasirLancar_Setup.iss"
$IsccExe    = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

# File config yang tidak ditimpa saat update (onlyifdoesntexist)
$ConfigFiles = @(
    "config_printer.ini", "config.bin", "ConfigLabelBarang.ini",
    "database.json", "printer.ini", "license.ini", "Kasir Lancar.xml"
)

# File/folder yang di-skip dari copy ke {app} karena sudah ditangani khusus
# (Logs = runtime, file .log/.pdb = debug, WebView2 = dibuat otomatis saat app jalan)
$SkipFiles = @("*.log", "*.pdb", "debug_original.sql")
$SkipFolders = @("Logs", "KasirLancar.exe.WebView2")

# Folder Printer Driver Software - file installer ke {tmp}, folder tetap ke {app}
$DriverFolder = "Printer Driver Software"

# File installer prerequisite yang ke {tmp}
$PrereqFiles = @{
    "appserv-9-3-0.exe"                              = "appserv"
    "ReportViewer.exe"                               = "reportviewer"
    "mysql-connector-net-9.1.0.msi"                  = "mysqlconn"
    "POS Printer Driver Setup .exe"                  = "posprinter"
    "MicrosoftEdgeWebView2RuntimeInstallerX64.exe"   = "edgewebview"
    "VC_redist.x64.exe"                              = "vcredist"
    "VC_redist.x86.exe"                              = "vcredist"
}

# File lama yang tidak perlu dikemas (versi lama / developer tools)
$ExcludeDriverFiles = @(
    "appserv-win32-8.5.0.exe",
    "appserv-x64-9.3.0.exe",
    "mysql-connector-net-8.0.24.msi",
    "mysql-for-visualstudio-1.2.10.msi"
)

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  KASIR LANCAR - Auto Build Installer" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# ---- Scan dan hitung semua file ----
Write-Host "Scanning $DebugDir ..." -ForegroundColor Yellow
$allFiles = Get-ChildItem -Path $DebugDir -Recurse -File
$totalFiles = $allFiles.Count
Write-Host "  Total file ditemukan: $totalFiles" -ForegroundColor Green

# Hitung per kategori
$rootFiles   = ($allFiles | Where-Object { $_.DirectoryName -eq (Resolve-Path $DebugDir).Path }).Count
$subDirFiles = $totalFiles - $rootFiles
Write-Host "  - File di root Debug : $rootFiles"
Write-Host "  - File di subfolder  : $subDirFiles"
Write-Host ""

# Daftar subfolder
$subDirs = Get-ChildItem -Path $DebugDir -Directory
Write-Host "  Subfolder ditemukan ($($subDirs.Count)):"
foreach ($d in $subDirs) {
    $count = (Get-ChildItem -Path $d.FullName -Recurse -File).Count
    Write-Host "    [$count file] $($d.Name)"
}
Write-Host ""

# ---- Generate [Files] section ----
Write-Host "Generating [Files] section ..." -ForegroundColor Yellow

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add("[Files]")
$lines.Add("; AUTO-GENERATED oleh Build-Installer.ps1 - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$lines.Add("; Total file di bin\Debug: $totalFiles")
$lines.Add("")

$debugFullPath = (Resolve-Path $DebugDir).Path

# Helper: cek apakah file harus skip
function ShouldSkipFile($fileName) {
    foreach ($pat in $SkipFiles) {
        if ($fileName -like $pat) { return $true }
    }
    return $false
}

# Helper: cek apakah folder harus skip
function ShouldSkipFolder($folderName) {
    return $SkipFolders -contains $folderName
}

# ---- 1. File di root Debug (bukan subfolder) ----
$lines.Add("; ----- File Utama Aplikasi -----")
$rootFileList = Get-ChildItem -Path $DebugDir -File | Sort-Object Name
foreach ($f in $rootFileList) {
    if (ShouldSkipFile $f.Name) { continue }

    if ($ConfigFiles -contains $f.Name) {
        $lines.Add("Source: ""{#MyAppSourceDir}\$($f.Name)""; DestDir: ""{app}""; Flags: onlyifdoesntexist; Components: mainapp")
    } else {
        $lines.Add("Source: ""{#MyAppSourceDir}\$($f.Name)""; DestDir: ""{app}""; Flags: ignoreversion; Components: mainapp")
    }
}
$lines.Add("")

# ---- 2. Subfolder - proses satu per satu ----
$subDirList = Get-ChildItem -Path $DebugDir -Directory | Sort-Object Name

foreach ($dir in $subDirList) {
    $dirName = $dir.Name
    $dirRelative = $dirName

    # Skip folder Logs (runtime)
    if (ShouldSkipFolder $dirName) {
        Write-Host "  SKIP folder: $dirName" -ForegroundColor DarkGray
        continue
    }

    # Folder Backup - copy ke {app}\Backup (rekursif)
    if ($dirName -eq "Backup") {
        $fileCount = (Get-ChildItem -Path $dir.FullName -Recurse -File).Count
        $lines.Add("; ----- Folder Backup ($fileCount file) -----")
        $lines.Add("Source: ""{#MyAppSourceDir}\Backup\*""; DestDir: ""{app}\Backup""; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mainapp")
        $lines.Add("")
        Write-Host "  + Backup: $fileCount file (recurse)" -ForegroundColor Green
        continue
    }

    # Folder Printer Driver Software - dual: ke {app} DAN installer ke {tmp}
    if ($dirName -eq "Printer Driver Software") {
        $driverFiles = Get-ChildItem -Path $dir.FullName -File | Sort-Object Name |
                       Where-Object { $ExcludeDriverFiles -notcontains $_.Name }
        $fileCount = $driverFiles.Count
        $lines.Add("; ----- Printer Driver Software ($fileCount file) -----")
        $lines.Add("; Semua file driver disertakan ke {app}\Printer Driver Software")
        foreach ($df in $driverFiles) {
            $lines.Add("Source: ""{#MyAppDriverDir}\$($df.Name)""; DestDir: ""{app}\Printer Driver Software""; Flags: ignoreversion; Components: mainapp")
        }
        $lines.Add("")
        $lines.Add("; Installer prerequisite juga ke {tmp} untuk dijalankan")
        foreach ($prereq in $PrereqFiles.GetEnumerator()) {
            $prereqPath = Join-Path $dir.FullName $prereq.Key
            if (Test-Path $prereqPath) {
                $lines.Add("Source: ""{#MyAppDriverDir}\$($prereq.Key)""; DestDir: ""{tmp}""; Flags: deleteafterinstall; Components: $($prereq.Value)")
            }
        }
        $lines.Add("")
        Write-Host "  + Printer Driver Software: $fileCount file (dikemas), $($PrereqFiles.Count) prereq ke {tmp}" -ForegroundColor Green
        continue
    }

    # Subfolder lainnya - gunakan wildcard rekursif
    $fileCount = (Get-ChildItem -Path $dir.FullName -Recurse -File).Count
    if ($fileCount -eq 0) { continue }

    $lines.Add("; ----- $dirName ($fileCount file) -----")
    $lines.Add("Source: ""{#MyAppSourceDir}\$dirRelative\*""; DestDir: ""{app}\$dirRelative""; Flags: ignoreversion recursesubdirs; Components: mainapp")
    $lines.Add("")
    Write-Host "  + $dirName`: $fileCount file" -ForegroundColor Green
}

$filesSection = $lines -join "`r`n"
Write-Host ""
Write-Host "  Total baris [Files] yang digenerate: $($lines.Count)" -ForegroundColor Green

# ---- Inject [Files] section ke .iss ----
Write-Host "Injecting ke $IssTemplate ..." -ForegroundColor Yellow

$issContent = Get-Content $IssTemplate -Raw -Encoding UTF8

# Cari batas [Files] section - dari [Files] sampai section berikutnya
$filesStart = $issContent.IndexOf("`n[Files]")
if ($filesStart -lt 0) { $filesStart = $issContent.IndexOf("[Files]") }
else { $filesStart++ }  # skip newline

# Cari section berikutnya setelah [Files]
$nextSectionPattern = [regex]'\r?\n\[(?!Files)[A-Za-z]'
$nextMatch = $nextSectionPattern.Match($issContent, $filesStart + 7)
if (-not $nextMatch.Success) {
    Write-Host "ERROR: Tidak bisa menemukan batas [Files] section!" -ForegroundColor Red
    exit 1
}
$filesEnd = $nextMatch.Index

# Ganti [Files] section
$before = $issContent.Substring(0, $filesStart)
$after  = $issContent.Substring($filesEnd)
$newContent = $before + $filesSection + "`r`n`r`n" +
              "; ----- Database Migration & Scripts (Auto-include all) -----" + "`r`n" +
              "Source: ""..\Database\*""; DestDir: ""{app}\Database""; Flags: ignoreversion; Components: mainapp" + "`r`n" +
              "; ----- Database Default Master (Data Kategori, Satuan, Merk) -----" + "`r`n" +
              "Source: ""..\database_Default_Master\*""; DestDir: ""{app}\database_Default_Master""; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mainapp" + "`r`n" +
              $after

$newContent | Set-Content $IssTemplate -Encoding UTF8 -NoNewline
Write-Host "  [Files] section berhasil diperbarui." -ForegroundColor Green
Write-Host ""

# ---- Regenerate banner dengan versi terbaru ----
Write-Host "Regenerating installer images ..." -ForegroundColor Yellow
& "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -ExecutionPolicy Bypass -NonInteractive -File "create_installer_images.ps1"
Write-Host "  Banner diperbarui." -ForegroundColor Green
Write-Host ""

# ---- Compile dengan Inno Setup ----
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  Compiling installer ..." -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# Ambil versi dari .iss untuk nama output
$issVersion = (Select-String -Path $IssTemplate -Pattern '#define MyAppVersion "(.+)"').Matches[0].Groups[1].Value
$outExe = "Output\KasirLancar_Setup_v$issVersion.exe"

# Jika file output terkunci, pakai nama temp untuk output lalu rename
$useTemp = $false
if (Test-Path $outExe) {
    try {
        $stream = [System.IO.File]::Open($outExe, 'Open', 'ReadWrite', 'None')
        $stream.Close()
    } catch {
        Write-Host "  File output terkunci (mungkin Windows Defender). Menggunakan nama temp..." -ForegroundColor Yellow
        $useTemp = $true
        # Ubah OutputBaseFilename sementara
        $issContent = Get-Content $IssTemplate -Raw
        $issContent = $issContent -replace 'OutputBaseFilename=KasirLancar_Setup_v\S+', "OutputBaseFilename=KasirLancar_Setup_v${issVersion}_new"
        $issContent | Set-Content $IssTemplate -Encoding UTF8 -NoNewline
    }
}

$result = & $IsccExe $IssTemplate 2>&1
$result | ForEach-Object {
    $line = $_.ToString()
    if ($line -match "^Error|Compile aborted") {
        Write-Host $line -ForegroundColor Red
    } elseif ($line -match "^Warning") {
        Write-Host $line -ForegroundColor Yellow
    } elseif ($line -match "Successful compile|Resulting Setup") {
        Write-Host $line -ForegroundColor Green
    } elseif ($line -match "Compressing|Reading|Parsing") {
        Write-Host $line -ForegroundColor DarkGray
    } else {
        Write-Host $line
    }
}

Write-Host ""
if ($LASTEXITCODE -eq 0) {
    # Restore OutputBaseFilename jika pakai temp
    if ($useTemp) {
        $issContent = Get-Content $IssTemplate -Raw
        $issContent = $issContent -replace "OutputBaseFilename=KasirLancar_Setup_v${issVersion}_new", "OutputBaseFilename=KasirLancar_Setup_v$issVersion"
        $issContent | Set-Content $IssTemplate -Encoding UTF8 -NoNewline
    }
    $outFile = Get-ChildItem "Output\*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    Write-Host "============================================================" -ForegroundColor Green
    Write-Host "  BERHASIL!" -ForegroundColor Green
    Write-Host "  Output : $($outFile.FullName)" -ForegroundColor Green
    Write-Host "  Ukuran : $([math]::Round($outFile.Length / 1MB, 2)) MB" -ForegroundColor Green
    Write-Host "  Total file dikemas: $totalFiles" -ForegroundColor Green
    Write-Host "============================================================" -ForegroundColor Green
} else {
    Write-Host "============================================================" -ForegroundColor Red
    Write-Host "  COMPILE GAGAL! Periksa error di atas." -ForegroundColor Red
    Write-Host "============================================================" -ForegroundColor Red
    exit 1
}
