# ============================================================
#  Setup-DevEnvironment.ps1
#  Jalankan sekali di komputer baru / setelah install ulang.
#  Script ini akan setup semua yang dibutuhkan untuk development
#  dan release AppKasir secara otomatis.
#
#  Cara pakai:
#    Klik kanan → Run with PowerShell
#    atau: powershell -ExecutionPolicy Bypass -File Setup-DevEnvironment.ps1
# ============================================================

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  AppKasir - Setup Development Environment" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

$allOk = $true

# ── 1. Git ────────────────────────────────────────────────────────
Write-Host "  [1] Memeriksa Git..." -ForegroundColor Yellow
$git = Get-Command git -ErrorAction SilentlyContinue
if ($git) {
    $gitVer = git --version
    Write-Host "      ✓ $gitVer" -ForegroundColor Green
} else {
    Write-Host "      ✗ Git belum terinstall!" -ForegroundColor Red
    Write-Host "        Download: https://git-scm.com/download/win" -ForegroundColor Cyan
    $allOk = $false
}

# ── 2. GitHub CLI (gh) ────────────────────────────────────────────
Write-Host "  [2] Memeriksa GitHub CLI (gh)..." -ForegroundColor Yellow
$gh = Get-Command gh -ErrorAction SilentlyContinue
if ($gh) {
    $ghVer = gh --version | Select-Object -First 1
    Write-Host "      ✓ $ghVer" -ForegroundColor Green

    # Cek sudah login
    $authStatus = gh auth status 2>&1
    if ($authStatus -match "Logged in") {
        Write-Host "      ✓ Sudah login ke GitHub" -ForegroundColor Green
    } else {
        Write-Host "      ✗ Belum login ke GitHub CLI" -ForegroundColor Yellow
        Write-Host "        Menjalankan gh auth login..." -ForegroundColor Cyan
        gh auth login
    }
} else {
    Write-Host "      ✗ GitHub CLI belum terinstall!" -ForegroundColor Red

    # Cari installer di Downloads
    $msiPath = "$env:USERPROFILE\Downloads\gh_*_windows_amd64.msi" 
    $msiFile = Get-ChildItem $msiPath -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    
    if ($msiFile) {
        Write-Host "        Installer ditemukan: $($msiFile.Name)" -ForegroundColor Green
        Write-Host "        Menginstall..." -ForegroundColor Cyan
        Start-Process msiexec.exe -ArgumentList "/i `"$($msiFile.FullName)`" /quiet /norestart" -Wait
        
        # Refresh PATH
        $env:PATH = [System.Environment]::GetEnvironmentVariable("PATH", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("PATH", "User")
        
        $gh2 = Get-Command gh -ErrorAction SilentlyContinue
        if ($gh2) {
            Write-Host "        ✓ GitHub CLI berhasil diinstall" -ForegroundColor Green
            Write-Host "        Menjalankan gh auth login..." -ForegroundColor Cyan
            gh auth login
        } else {
            Write-Host "        ✗ Install gagal. Download manual:" -ForegroundColor Red
            Write-Host "          https://github.com/cli/cli/releases/latest" -ForegroundColor Cyan
            $allOk = $false
        }
    } else {
        Write-Host "        Download: https://github.com/cli/cli/releases/latest" -ForegroundColor Cyan
        Write-Host "        Pilih: gh_x.x.x_windows_amd64.msi" -ForegroundColor Cyan
        $allOk = $false
    }
}

# ── 3. Git config (nama & email) ──────────────────────────────────
Write-Host "  [3] Memeriksa Git config..." -ForegroundColor Yellow
$gitName  = git config --global user.name  2>$null
$gitEmail = git config --global user.email 2>$null

if ($gitName -and $gitEmail) {
    Write-Host "      ✓ user.name  : $gitName" -ForegroundColor Green
    Write-Host "      ✓ user.email : $gitEmail" -ForegroundColor Green
} else {
    Write-Host "      ✗ Git user belum dikonfigurasi" -ForegroundColor Yellow
    $name  = Read-Host "        Masukkan nama kamu"
    $email = Read-Host "        Masukkan email GitHub kamu"
    git config --global user.name  $name
    git config --global user.email $email
    Write-Host "      ✓ Git config disimpan" -ForegroundColor Green
}

# ── 4. Git remote ─────────────────────────────────────────────────
Write-Host "  [4] Memeriksa Git remote..." -ForegroundColor Yellow
$remote = git remote get-url origin 2>$null
if ($remote) {
    Write-Host "      ✓ origin: $remote" -ForegroundColor Green
} else {
    Write-Host "      ✗ Remote origin belum diset" -ForegroundColor Yellow
    git remote add origin https://github.com/adysuryadi64/AppKasir.git
    Write-Host "      ✓ Remote origin ditambahkan" -ForegroundColor Green
}

# ── 5. Visual Studio ──────────────────────────────────────────────
Write-Host "  [5] Memeriksa Visual Studio..." -ForegroundColor Yellow
$vsPath = "C:\Program Files\Microsoft Visual Studio\18\Professional\Common7\IDE\devenv.exe"
if (Test-Path $vsPath) {
    Write-Host "      ✓ Visual Studio 2022 Professional ditemukan" -ForegroundColor Green
} else {
    $vsAny = Get-ChildItem "C:\Program Files\Microsoft Visual Studio" -Filter "devenv.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($vsAny) {
        Write-Host "      ✓ Visual Studio ditemukan: $($vsAny.FullName)" -ForegroundColor Green
    } else {
        Write-Host "      ✗ Visual Studio belum terinstall" -ForegroundColor Red
        Write-Host "        Download: https://visualstudio.microsoft.com/" -ForegroundColor Cyan
        $allOk = $false
    }
}

# ── Ringkasan ─────────────────────────────────────────────────────
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
if ($allOk) {
    Write-Host "  ✓ Semua siap! Buka AppKasir.sln di Visual Studio." -ForegroundColor Green
    Write-Host ""
    Write-Host "  Alur kerja rilis:" -ForegroundColor White
    Write-Host "    1. Selesai coding" -ForegroundColor White
    Write-Host "    2. Ganti konfigurasi ke Release" -ForegroundColor White
    Write-Host "    3. Ctrl+Shift+B" -ForegroundColor White
    Write-Host "    → Versi naik, ZIP dibuat, push ke GitHub, release otomatis" -ForegroundColor DarkGray
} else {
    Write-Host "  ✗ Ada yang belum siap. Selesaikan langkah di atas dulu." -ForegroundColor Red
}
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Read-Host "Tekan Enter untuk keluar"
