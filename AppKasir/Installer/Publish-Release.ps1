# ============================================================
#  Publish-Release.ps1
#  Script UTAMA untuk publish release Kasir Lancar.
#  Cukup jalankan script ini — semua otomatis:
#    0. Build Release via MSBuild
#    1. Buat AppKasir_Update.zip (via Build-Update.ps1)
#    2. Git commit + push update.xml & AssemblyInfo.vb
#    3. Buat Git tag versi
#    4. Push tag ke GitHub
#    5. Buat GitHub Release + upload ZIP otomatis (pakai gh CLI)
#
#  Cara pakai:
#    powershell -ExecutionPolicy Bypass -File Installer\Publish-Release.ps1
#
#  Prasyarat: GitHub CLI (gh) harus terinstall dan sudah login
#  Install: https://cli.github.com/
# ============================================================

param(
    [switch]$NonInteractive  # Set otomatis saat dipanggil dari MSBuild
)

Set-Location (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent)  # ke root solution (AppKasir_2026\)

# ── [0/6] Build Release via MSBuild ──────────────────────────────
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  KASIR LANCAR - Publish Release (All-in-One)" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  [0/6] Building Release..." -ForegroundColor Yellow

# Cari MSBuild via vswhere
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = ""

if (Test-Path $vswhere) {
    $msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild `
        -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
}

# Fallback: cari di PATH
if (-not $msbuild -or -not (Test-Path $msbuild)) {
    $msbuild = (Get-Command MSBuild.exe -ErrorAction SilentlyContinue).Source
}

if (-not $msbuild -or -not (Test-Path $msbuild)) {
    Write-Host "  ERROR: MSBuild tidak ditemukan!" -ForegroundColor Red
    Write-Host "         Pastikan Visual Studio atau Build Tools terinstall." -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

Write-Host "        MSBuild: $msbuild" -ForegroundColor DarkGray

# Restore NuGet packages dulu
$nuget = Get-Command nuget.exe -ErrorAction SilentlyContinue
if ($nuget) {
    & nuget.exe restore "AppKasir.sln"
}

# Build Release
& $msbuild "AppKasir\AppKasir.vbproj" /p:Configuration=Release /t:Build
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ERROR: Build Release gagal!" -ForegroundColor Red
    Write-Host "         Periksa error di output MSBuild di atas." -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}
Write-Host "        Build Release selesai." -ForegroundColor Green

# ── [1/6] Buat ZIP via Build-Update.ps1 ─────────────────────────
Write-Host "  [1/6] Membuat ZIP update..." -ForegroundColor Yellow
& powershell -ExecutionPolicy Bypass -NoProfile -File "AppKasir\Installer\Build-Update.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ERROR: Pembuatan ZIP gagal!" -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

# ── Baca versi dari update.xml ────────────────────────────────────
$UpdateXmlPath = "update.xml"
$zipPath       = "AppKasir\Installer\Output\AppKasir_Update.zip"
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

$tag     = "v$versi"

Write-Host ""
Write-Host "  Versi : $versi" -ForegroundColor Cyan
Write-Host "  Tag   : $tag" -ForegroundColor Cyan
Write-Host ""

# ── Cek ZIP ada ───────────────────────────────────────────────────
if (-not (Test-Path $zipPath)) {
    Write-Host "ERROR: $zipPath tidak ditemukan!" -ForegroundColor Red
    if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
    exit 1
}

$zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
Write-Host "  ZIP   : $zipPath ($zipSize MB)" -ForegroundColor Green
Write-Host ""

# ── Cek gh CLI tersedia ───────────────────────────────────────────
$ghAvailable = $null -ne (Get-Command gh -ErrorAction SilentlyContinue)

# ── [1.5/6] Auto Changelog dengan AI (Gemini → Groq → OpenRouter) ──
Write-Host "  [1.5/6] Menyiapkan file untuk Commit & Generate Changelog AI..." -ForegroundColor Yellow

# Tambahkan semua file yang berubah ke dalam antrean (Staging)
git add -A 2>$null

$aiKeyPath = Join-Path $PSScriptRoot "..\..\..\.ai_key"
if (-not (Test-Path $aiKeyPath)) {
    $aiKeyPath = Join-Path $PSScriptRoot "..\..\.ai_key"
}

# ── Parse .ai_key file ──
$aiKeys = @{}
if (Test-Path $aiKeyPath) {
    $lines = Get-Content $aiKeyPath | Where-Object { $_.Trim() -ne "" }
    $cleanLines = @()
    foreach ($l in $lines) {
        $trimmed = $l.Trim()
        if ($trimmed -and -not $trimmed.StartsWith("#")) {
            $cleanLines += $trimmed
        }
    }
    
    if ($cleanLines.Count -eq 1 -and $cleanLines[0] -notmatch "=") {
        $aiKeys["GEMINI"] = $cleanLines[0]
    } else {
        foreach ($line in $cleanLines) {
            if ($line -match "^(\w+)=(.+)$") {
                $aiKeys[$Matches[1].ToUpper()] = $Matches[2].Trim()
            }
        }
    }
}

$hasValidKey = $false
foreach ($k in $aiKeys.Keys) {
    if ($aiKeys[$k] -ne "") {
        $hasValidKey = $true
        break
    }
}

if (-not $hasValidKey) {
    Write-Host "        File .ai_key tidak ditemukan atau tidak memiliki key yang aktif. Changelog dilewati." -ForegroundColor DarkGray
} else {
    # Ambil diff untuk prompt
    $lastTag = git describe --tags --abbrev=0 2>$null
    if ($lastTag) {
        $diff = (git diff -U1 $lastTag --cached) -join "`n"
    } else {
        $diff = (git diff -U1 HEAD --cached) -join "`n"
    }

    if ($diff) {
        # Truncate diff jika terlalu panjang (hemat token, cegah error 400)
        $maxDiffLen = 12000
        if ($diff.Length -gt $maxDiffLen) {
            $diff = $diff.Substring(0, $maxDiffLen) + "`n... [diff dipotong untuk hemat token]"
            Write-Host "        Diff dipotong ke $maxDiffLen karakter." -ForegroundColor DarkGray
        }

        Write-Host "        Meminta AI merangkum perubahan..." -ForegroundColor DarkGray
        $prompt = @"
Kamu adalah pembuat catatan rilis teknis aplikasi kasir (POS). Berdasarkan git diff berikut, buatkan changelog dalam bahasa Indonesia.

Aturan:
- Sebutkan nama file/komponen yang diubah
- Jelaskan apa yang berubah secara teknis (singkat, padat, jelas)
- Format: markdown bullet points
- Jangan tulis penjelasan umum — fokus ke perubahan spesifik
- Jangan tulis "Tidak ada perubahan signifikan" — jika memang tidak ada, tulis "Tidak ada perubahan kode"

Data perubahan:
$diff
"@

        # Gunakan [System.Text.Json.JsonSerializer] untuk serialisasi yang benar
        # ConvertTo-Json PowerShell tidak escape backslash & karakter kontrol dengan benar
        Add-Type -AssemblyName System.Text.Json
        function New-JsonBody($model, $promptText) {
            $obj = [ordered]@{
                model    = $model
                messages = @(
                    [ordered]@{ role = "user"; content = $promptText }
                )
            }
            $opts = [System.Text.Json.JsonSerializerOptions]::new()
            $opts.Encoder = [System.Text.Encodings.Web.JavaScriptEncoder]::UnsafeRelaxedJsonEscaping
            return [System.Text.Json.JsonSerializer]::Serialize([object]$obj, $opts)
        }

        # ── Daftar provider AI (3 aktif, tanpa Cohere & HuggingFace) ──
        $providers = @()

        # 1. Gemini (Google AI Studio - Free, paling pintar)
        if ($aiKeys.ContainsKey("GEMINI") -and $aiKeys["GEMINI"] -ne "") {
            $providers += @{
                Name    = "Gemini"
                Url     = "https://generativelanguage.googleapis.com/v1beta/openai/chat/completions"
                Headers = @{ "Content-Type" = "application/json; charset=utf-8"; "Authorization" = "Bearer $($aiKeys['GEMINI'])" }
                Body    = New-JsonBody "gemini-2.5-flash" $prompt
                Timeout = 60
            }
        }

        # 2. Groq (Sangat cepat, model ringan untuk hemat quota)
        if ($aiKeys.ContainsKey("GROQ") -and $aiKeys["GROQ"] -ne "") {
            $providers += @{
                Name    = "Groq"
                Url     = "https://api.groq.com/openai/v1/chat/completions"
                Headers = @{ "Content-Type" = "application/json; charset=utf-8"; "Authorization" = "Bearer $($aiKeys['GROQ'])" }
                Body    = New-JsonBody "llama-3.1-8b-instant" $prompt
                Timeout = 45
            }
        }

        # 3. OpenRouter (Banyak model gratis)
        if ($aiKeys.ContainsKey("OPENROUTER") -and $aiKeys["OPENROUTER"] -ne "") {
            $providers += @{
                Name    = "OpenRouter"
                Url     = "https://openrouter.ai/api/v1/chat/completions"
                Headers = @{ "Content-Type" = "application/json; charset=utf-8"; "Authorization" = "Bearer $($aiKeys['OPENROUTER'])"; "HTTP-Referer" = "https://github.com/adysuryadi64/AppKasir" }
                Body    = New-JsonBody "google/gemini-2.5-flash:free" $prompt
                Timeout = 60
            }
        }

        $changelogText = $null
        foreach ($prov in $providers) {
            if ($changelogText) { break }
            for ($attempt = 1; $attempt -le 2; $attempt++) {
                try {
                    Write-Host "        Mencoba $($prov.Name) (percobaan $attempt)..." -ForegroundColor DarkGray
                    $bodyBytes = [System.Text.Encoding]::UTF8.GetBytes($prov.Body)
                    $response = Invoke-RestMethod -Method Post -Uri $prov.Url -Headers $prov.Headers -Body $bodyBytes -TimeoutSec $prov.Timeout
                    $changelogText = $response.choices[0].message.content
                    if ($changelogText) {
                        Write-Host "        Changelog berhasil dari $($prov.Name)." -ForegroundColor Green
                        break
                    }
                } catch {
                    $errMsg = $_.Exception.Message
                    if ($errMsg -match "429") {
                        Write-Host "        $($prov.Name): rate limit, tunggu 5 detik..." -ForegroundColor Yellow
                        Start-Sleep -Seconds 5
                    } else {
                        Write-Host "        $($prov.Name) gagal: $errMsg" -ForegroundColor Yellow
                    }
                    if ($attempt -lt 2) { Start-Sleep -Seconds 2 }
                }
            }
        }

        if ($changelogText) {
            Write-Host ""
            Write-Host "        [Hasil Changelog AI]:" -ForegroundColor Cyan
            $changelogText -split "`n" | ForEach-Object { Write-Host "        $_" -ForegroundColor Cyan }
            Write-Host ""

            $changelogPath = Join-Path $PSScriptRoot "..\..\changelog.md"
            Set-Content -Path $changelogPath -Value $changelogText -Encoding UTF8
            git add "changelog.md" 2>$null
        } else {
            Write-Host "        Semua provider AI gagal. Changelog dilewati." -ForegroundColor Red
        }
    }
}

# ── [2/6] Git commit & push ──────────────────────────────────────
Write-Host "  [2/6] Git commit..." -ForegroundColor Yellow
$status = git status --porcelain
if ($status) {
    git commit -m "Release $tag"
    Write-Host "        Committed: Semua perubahan sebagai Release $tag" -ForegroundColor Green
} else {
    Write-Host "        Tidak ada perubahan untuk di-commit." -ForegroundColor DarkGray
}

Write-Host "  [3/6] Git push..." -ForegroundColor Yellow
git push origin master
Write-Host "        Push selesai." -ForegroundColor Green

Write-Host "  [4/6] Git tag $tag..." -ForegroundColor Yellow
# Hapus tag lama jika ada (untuk re-release versi yang sama)
git tag -d $tag 2>$null
git push origin ":refs/tags/$tag" 2>$null
git tag $tag
git push origin $tag
Write-Host "        Tag $tag dibuat dan dipush." -ForegroundColor Green

# ── [5/6] GitHub Release ─────────────────────────────────────────
Write-Host "  [5/6] GitHub Release..." -ForegroundColor Yellow

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

# ── [6/6] Verifikasi akhir ───────────────────────────────────────
Write-Host "  [6/6] Verifikasi..." -ForegroundColor Yellow

# Verifikasi versi EXE di ZIP = update.xml
Add-Type -AssemblyName System.IO.Compression.FileSystem
try {
    $archive = [System.IO.Compression.ZipFile]::OpenRead((Resolve-Path $zipPath).Path)
    $exeEntry = $archive.Entries | Where-Object { $_.Name -eq "KasirLancar.exe" -and $_.FullName -eq "KasirLancar.exe" }
    if ($exeEntry) {
        $tempCheck = Join-Path $env:TEMP "KasirLancar_VersionCheck.exe"
        if (Test-Path $tempCheck) { Remove-Item $tempCheck -Force }
        [System.IO.Compression.ZipFileExtensions]::ExtractToFile($exeEntry, $tempCheck, $true)
        $zipExeVer = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($tempCheck).FileVersion
        Remove-Item $tempCheck -Force
        
        if ($zipExeVer -eq $versi) {
            Write-Host "        [OK] Versi ZIP EXE ($zipExeVer) = update.xml ($versi)" -ForegroundColor Green
        } else {
            Write-Host "        [ERROR] PERINGATAN: Versi ZIP EXE ($zipExeVer) != update.xml ($versi)" -ForegroundColor Red
        }
    }
    $archive.Dispose()
} catch {
    Write-Host "        Tidak bisa verifikasi ZIP: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "  SELESAI! Versi $versi sudah live." -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
Write-Host ""
if (-not $NonInteractive) { Read-Host "Tekan Enter untuk keluar" }
