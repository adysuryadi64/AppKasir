# SetVersion.ps1
# Dipanggil otomatis oleh Pre-build Event Visual Studio
#
# Format versi: 15.YYYY.MMDD.Rev
#   - YYYY  = tahun          (contoh: 2026)
#   - MMDD  = bulan+tanggal  (contoh: 519 untuk 19 Mei, 1231 untuk 31 Des)
#   - Rev   = urutan build hari ini, mulai dari 0
#
# Contoh hari ini 19 Mei 2026:
#   Build ke-1 → 15.2026.519.0
#   Build ke-2 → 15.2026.519.1
#   Build ke-3 → 15.2026.519.2

param(
    [string]$ProjectDir,
    [string]$UpdateXmlPath,
    [string]$Configuration = "Debug"
)

# Bersihkan tanda kutip yang mungkin ikut dari MSBuild
$ProjectDir     = $ProjectDir.Trim('"').Trim("'").TrimEnd('\') + '\'
$UpdateXmlPath  = $UpdateXmlPath.Trim('"').Trim("'")

$AssemblyInfoPath = Join-Path $ProjectDir "My Project\AssemblyInfo.vb"
$RevFilePath      = Join-Path $ProjectDir ".buildrev"

$today   = Get-Date
$major   = 15
$year    = $today.Year
$mmdd    = "$($today.Month)$($today.Day)"   # 519, 1231, dst — tanpa leading zero di bulan
$dateKey = "$year.$mmdd"

# ── Baca revision counter untuk hari ini ─────────────────────
$rev = 0
if (Test-Path $RevFilePath) {
    try {
        $stored = Get-Content $RevFilePath -Raw | ConvertFrom-Json
        if ($stored.date -eq $dateKey) {
            $rev = [int]$stored.rev + 1
        }
    } catch { $rev = 0 }
}

# Simpan revision terbaru ke file
@{ date = $dateKey; rev = $rev } | ConvertTo-Json | Set-Content $RevFilePath -Encoding UTF8

# ── Bentuk string versi ───────────────────────────────────────
$versionString = "$major.$year.$mmdd.$rev"

# ── Update AssemblyInfo.vb ────────────────────────────────────
$content = Get-Content $AssemblyInfoPath -Raw
$content = $content -replace 'AssemblyVersion\("[^"]+"\)',     "AssemblyVersion(""$versionString"")"
$content = $content -replace 'AssemblyFileVersion\("[^"]+"\)', "AssemblyFileVersion(""$versionString"")"
Set-Content $AssemblyInfoPath $content -Encoding UTF8

Write-Host "##[info] Versi di-set ke: $versionString (build ke-$($rev + 1) hari ini)"

# ── Update update.xml HANYA saat Release build ──────────────
if ($Configuration -eq "Release" -and $UpdateXmlPath -ne "" -and (Test-Path $UpdateXmlPath)) {
    $xmlContent = Get-Content $UpdateXmlPath -Raw
    $xmlContent = $xmlContent -replace '<version>[^<]+</version>',   "<version>$versionString</version>"
    $xmlContent = $xmlContent -replace 'releases/download/v[^/]+/', "releases/download/v$versionString/"
    $xmlContent = $xmlContent -replace 'releases/tag/v[^<"]+',       "releases/tag/v$versionString"
    Set-Content $UpdateXmlPath $xmlContent -Encoding UTF8
    Write-Host "##[info] update.xml di-sync ke versi: $versionString"
} elseif ($Configuration -ne "Release") {
    Write-Host "##[info] update.xml TIDAK diubah (build $Configuration - hanya Release yang update)"
}
