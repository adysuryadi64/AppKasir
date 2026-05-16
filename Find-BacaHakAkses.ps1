# PowerShell Script untuk mencari semua penggunaan BacaHakAkses yang masih ada

param(
    [string]$SearchPath = "E:\1 Visual Studio 2026\AppKasir - 2026\AppKasir",
    [string]$Pattern = "BacaHakAkses"
)

Write-Host "🔍 Mencari semua penggunaan $Pattern..." -ForegroundColor Cyan
Write-Host "📁 Path: $SearchPath" -ForegroundColor Yellow
Write-Host ""

# Filter untuk VB files saja
$vbFiles = Get-ChildItem -Path $SearchPath -Filter "*.vb" -Recurse | 
           Where-Object { $_.FullName -notlike "*Designer*" -and $_.FullName -notlike "*.resx*" }

$results = @()

foreach ($file in $vbFiles) {
    $content = Get-Content $file.FullName -Raw
    $lines = $content -split "`n"
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        if ($line -match $Pattern) {
            $results += [PSCustomObject]@{
                File = $file.Name
                Path = $file.FullName
                LineNumber = $i + 1
                Content = $line.Trim()
            }
        }
    }
}

if ($results.Count -gt 0) {
    Write-Host "✅ DITEMUKAN $($results.Count) INSTANCE(S):" -ForegroundColor Green
    Write-Host ""
    
    $results | Format-Table -AutoSize -Property `
        @{Name = "FILE"; Expression = {$_.File}},
        @{Name = "LINE"; Expression = {$_.LineNumber}},
        @{Name = "USAGE"; Expression = {$_.Content.Substring(0, [Math]::Min(60, $_.Content.Length)) + "..."}}
    
    Write-Host ""
    Write-Host "📋 DETAIL PENGGUNAAN:" -ForegroundColor Yellow
    
    $results | Group-Object File | ForEach-Object {
        Write-Host "`n📄 $($_.Name) - $($_.Count) instance(s)" -ForegroundColor Cyan
        $_.Group | ForEach-Object {
            Write-Host "   Line $($_.LineNumber): $($_.Content.Trim().Substring(0, [Math]::Min(70, $_.Content.Trim().Length)))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "✅ SEMPURNA! Tidak ada penggunaan $Pattern ditemukan!" -ForegroundColor Green
}

Write-Host ""
Write-Host "=" * 80
Write-Host "📊 SUMMARY:" -ForegroundColor Cyan
Write-Host "Total Files Checked: $($vbFiles.Count)"
Write-Host "Files dengan $Pattern : $($results | Select-Object -ExpandProperty File -Unique | Measure-Object | Select-Object -ExpandProperty Count)"
Write-Host "Total Instances: $($results.Count)"
Write-Host "=" * 80
