# ============================================================
#  Publish-Release.ps1
#  Jalankan SETELAH Build Release di Visual Studio selesai.
#  Script ini akan:
#    1. Git commit + push update.xml & AssemblyInfo.vb
#    2. Buat Git tag versi
#    3. Push tag ke GitHub
#    4. Buat GitHub Release + upload ZIP otomatis (pakai gh CLI)
#
#  Prasyarat: GitHub CLI (gh) harus terinstall dan sudah login
#  Install: https://cli.github.com/
# ============================================================

param(
    [switch]$NonInteractive  # Set otomatis saat dipanggil dari MSBuild
)

Set-Location (Split-Path $PSScriptRoot -Parent)  # ke root solution

# ── Baca versi dari update.xml ────────────────────────────────────
$UpdateXmlPath = "update.xml"
$versi = ""
if (Test-Path $UpdateXmlPath) {
    $match = Select-String -Path $UpdateXmlPath -Pattern '<version>([^<]+)</version>'
    if ($match) { $versi = $match.Matches[0].Groups[1].Value }
}

if ($versi -eq "") {
    Write-Host "ERROR: Tidak bisa membaca versi dari update.xml" -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

$zipPath = "AppKasir\Installer\Output\AppKasir_Update.zip"
$tag     = "v$versi"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  KASIR LANCAR - Publish Release" -ForegroundColor Cyan
Write-Host "  Versi : $versi" -ForegroundColor Cyan
Write-Host "  Tag   : $tag" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# ── Cek ZIP ada ───────────────────────────────────────────────────
if (-not (Test-Path $zipPath)) {
    Write-Host "ERROR: $zipPath tidak ditemukan!" -ForegroundColor Red
    Write-Host "       Pastikan Build Release sudah dijalankan." -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

$zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
Write-Host "  ZIP   : $zipPath ($zipSize MB)" -ForegroundColor Green
Write-Host ""

# ── Cek gh CLI tersedia ───────────────────────────────────────────
$ghAvailable = $null -ne (Get-Command gh -ErrorAction SilentlyContinue)

# ── Git commit & push ─────────────────────────────────────────────
Write-Host "  [1/4] Git commit..." -ForegroundColor Yellow
git add "update.xml" "AppKasir/My Project/AssemblyInfo.vb"
$status = git status --porcelain "update.xml" "AppKasir/My Project/AssemblyInfo.vb"
if ($status) {
    git commit -m "Release $tag"
    Write-Host "        Committed: Release $tag" -ForegroundColor Green
} else {
    Write-Host "        Tidak ada perubahan untuk di-commit." -ForegroundColor DarkGray
}

Write-Host "  [2/4] Git push..." -ForegroundColor Yellow
git push origin master
Write-Host "        Push selesai." -ForegroundColor Green

Write-Host "  [3/4] Git tag $tag..." -ForegroundColor Yellow
# Hapus tag lama jika ada (untuk re-release versi yang sama)
git tag -d $tag 2>$null
git push origin ":refs/tags/$tag" 2>$null
git tag $tag
git push origin $tag
Write-Host "        Tag $tag dibuat dan dipush." -ForegroundColor Green

# ── GitHub Release ────────────────────────────────────────────────
Write-Host "  [4/4] GitHub Release..." -ForegroundColor Yellow

if ($ghAvailable) {
    # Hapus release lama jika ada
    gh release delete $tag --yes 2>$null

    # Buat release baru + upload ZIP
    gh release create $tag $zipPath `
        --title "Kasir Lancar $tag" `
        --notes "Update otomatis versi $versi`n`nUnduh dan jalankan melalui menu Cek Update di aplikasi." `
        --latest

    if ($LASTEXITCODE -eq 0) {
        Write-Host "        GitHub Release $tag berhasil dibuat." -ForegroundColor Green
    } else {
        Write-Host "        GitHub Release gagal. Buat manual di GitHub." -ForegroundColor Red
    }
} else {
    Write-Host "        GitHub CLI (gh) tidak ditemukan." -ForegroundColor Yellow
    Write-Host "        Buat release manual di:" -ForegroundColor Yellow
    Write-Host "        https://github.com/adysuryadi64/AppKasir/releases/new?tag=$tag" -ForegroundColor Cyan
    Write-Host "        Upload file: $((Resolve-Path $zipPath).Path)" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "  SELESAI! Versi $versi sudah live." -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
Write-Host ""
if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
