# ============================================================
#  create_installer_images.ps1
#  Generate banner & logo installer Kasir Lancar - Premium Design
# ============================================================

Add-Type -AssemblyName System.Drawing

$Version = "v25.3.2026.18"

# Warna tema - Deep Navy + Gold Accent
$cNavyDark   = [System.Drawing.Color]::FromArgb(255,  10,  22,  50)
$cNavyMid    = [System.Drawing.Color]::FromArgb(255,  18,  40,  85)
$cNavyLight  = [System.Drawing.Color]::FromArgb(255,  28,  65, 130)
$cGold       = [System.Drawing.Color]::FromArgb(255, 212, 175,  55)
$cGoldLight  = [System.Drawing.Color]::FromArgb(255, 240, 210, 100)
$cWhite      = [System.Drawing.Color]::White
$cWhiteDim   = [System.Drawing.Color]::FromArgb(180, 255, 255, 255)
$cWhiteFaint = [System.Drawing.Color]::FromArgb(60,  255, 255, 255)
$cAccentLine = [System.Drawing.Color]::FromArgb(255, 212, 175,  55)

# ============================================================
# HELPER: Gradient vertikal
# ============================================================
function Draw-VerticalGradient($g, $x, $y, $w, $h, $colorTop, $colorBottom) {
    $rect  = [System.Drawing.Rectangle]::new($x, $y, $w, $h)
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $rect, $colorTop, $colorBottom,
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical)
    $g.FillRectangle($brush, $rect)
    $brush.Dispose()
}

# ============================================================
# 1. SIDEBAR BANNER  164 x 314 px
# ============================================================
$W = 164; $H = 314
$bmp = New-Object System.Drawing.Bitmap($W, $H)
$g   = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode      = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint  = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$g.InterpolationMode  = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

# --- Background gradient utama ---
Draw-VerticalGradient $g 0 0 $W $H $cNavyDark $cNavyMid

# --- Panel bawah lebih terang ---
Draw-VerticalGradient $g 0 200 $W 114 $cNavyMid $cNavyLight

# --- Ornamen lingkaran besar (blur/transparan) di tengah atas ---
$brushCircle1 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(18, 255, 255, 255))
$g.FillEllipse($brushCircle1, -40, -40, 180, 180)
$brushCircle1.Dispose()

$brushCircle2 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(10, 255, 255, 255))
$g.FillEllipse($brushCircle2, 30, 20, 140, 140)
$brushCircle2.Dispose()

# --- Ikon "K" besar sebagai logo utama ---
$fontIcon = New-Object System.Drawing.Font("Segoe UI", 52, [System.Drawing.FontStyle]::Bold)
$sfCenter = New-Object System.Drawing.StringFormat
$sfCenter.Alignment     = [System.Drawing.StringAlignment]::Center
$sfCenter.LineAlignment = [System.Drawing.StringAlignment]::Center

# Shadow ikon
$brushIconShadow = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(40, 0, 0, 0))
$g.DrawString("K", $fontIcon, $brushIconShadow, [System.Drawing.RectangleF]::new(3, 23, $W, 100), $sfCenter)
$brushIconShadow.Dispose()

# Ikon utama gold
$brushIcon = New-Object System.Drawing.SolidBrush($cGold)
$g.DrawString("K", $fontIcon, $brushIcon, [System.Drawing.RectangleF]::new(0, 20, $W, 100), $sfCenter)
$brushIcon.Dispose()
$fontIcon.Dispose()

# --- Garis aksen gold horizontal ---
$penGold = New-Object System.Drawing.Pen($cGold, 1.5)
$g.DrawLine($penGold, 18, 128, $W - 18, 128)
$penGold.Dispose()

$penGoldThin = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(80, 212, 175, 55), 1)
$g.DrawLine($penGoldThin, 18, 131, $W - 18, 131)
$penGoldThin.Dispose()

# --- Teks "KASIR" ---
$fontTitle = New-Object System.Drawing.Font("Segoe UI", 15, [System.Drawing.FontStyle]::Bold)
$brushWhite = New-Object System.Drawing.SolidBrush($cWhite)
$g.DrawString("KASIR", $fontTitle, $brushWhite,
    [System.Drawing.RectangleF]::new(0, 138, $W, 28), $sfCenter)
$fontTitle.Dispose()

# --- Teks "LANCAR" dengan tracking lebih lebar ---
$fontTitle2 = New-Object System.Drawing.Font("Segoe UI Light", 13, [System.Drawing.FontStyle]::Regular)
$brushGold  = New-Object System.Drawing.SolidBrush($cGold)
$g.DrawString("LANCAR", $fontTitle2, $brushGold,
    [System.Drawing.RectangleF]::new(0, 162, $W, 26), $sfCenter)
$fontTitle2.Dispose()
$brushGold.Dispose()

# --- Garis tipis pemisah ---
$penFaint = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(50, 255, 255, 255), 1)
$g.DrawLine($penFaint, 30, 196, $W - 30, 196)
$penFaint.Dispose()

# --- Tagline ---
$fontTag   = New-Object System.Drawing.Font("Segoe UI", 7, [System.Drawing.FontStyle]::Italic)
$brushDim  = New-Object System.Drawing.SolidBrush($cWhiteDim)
$g.DrawString("Sistem Kasir Profesional", $fontTag, $brushDim,
    [System.Drawing.RectangleF]::new(0, 200, $W, 18), $sfCenter)
$fontTag.Dispose()
$brushDim.Dispose()

# --- Ornamen titik-titik dekoratif ---
$brushDot = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(60, 212, 175, 55))
for ($i = 0; $i -lt 5; $i++) {
    $g.FillEllipse($brushDot, (18 + $i * 26), 225, 5, 5)
}
$brushDot.Dispose()

# --- Versi ---
$fontVer   = New-Object System.Drawing.Font("Segoe UI", 7.5, [System.Drawing.FontStyle]::Regular)
$brushFaint = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(140, 255, 255, 255))
$g.DrawString($Version, $fontVer, $brushFaint,
    [System.Drawing.RectangleF]::new(0, 240, $W, 18), $sfCenter)
$fontVer.Dispose()
$brushFaint.Dispose()

# --- Strip gold di bagian paling bawah ---
$brushStrip = New-Object System.Drawing.SolidBrush($cGold)
$g.FillRectangle($brushStrip, 0, $H - 4, $W, 4)
$brushStrip.Dispose()

# --- Ornamen sudut kanan bawah ---
$brushCorner = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(25, 255, 255, 255))
$g.FillEllipse($brushCorner, 80, 260, 100, 100)
$brushCorner.Dispose()

# Cleanup & save
$g.Dispose()
$bmp.Save("installer_banner.bmp", [System.Drawing.Imaging.ImageFormat]::Bmp)
$bmp.Dispose()
Write-Host "installer_banner.bmp dibuat (164x314)" -ForegroundColor Green

# ============================================================
# 2. LOGO KECIL  55 x 55 px  (pojok kanan atas wizard)
# ============================================================
$bmpL = New-Object System.Drawing.Bitmap(55, 55)
$gL   = [System.Drawing.Graphics]::FromImage($bmpL)
$gL.SmoothingMode     = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$gL.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit

# Background lingkaran gradient navy
$rectL  = [System.Drawing.Rectangle]::new(1, 1, 53, 53)
$brushL = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    $rectL, $cNavyDark, $cNavyLight,
    [System.Drawing.Drawing2D.LinearGradientMode]::ForwardDiagonal)
$gL.FillEllipse($brushL, 1, 1, 53, 53)
$brushL.Dispose()

# Border gold tipis
$penBorder = New-Object System.Drawing.Pen($cGold, 1.5)
$gL.DrawEllipse($penBorder, 2, 2, 51, 51)
$penBorder.Dispose()

# Teks "K" di tengah
$fontK  = New-Object System.Drawing.Font("Segoe UI", 22, [System.Drawing.FontStyle]::Bold)
$sfC    = New-Object System.Drawing.StringFormat
$sfC.Alignment     = [System.Drawing.StringAlignment]::Center
$sfC.LineAlignment = [System.Drawing.StringAlignment]::Center
$brushK = New-Object System.Drawing.SolidBrush($cGold)
$gL.DrawString("K", $fontK, $brushK, [System.Drawing.RectangleF]::new(0, -1, 55, 55), $sfC)
$brushK.Dispose()
$fontK.Dispose()

# Titik aksen kecil di bawah huruf K
$brushDotL = New-Object System.Drawing.SolidBrush($cGoldLight)
$gL.FillEllipse($brushDotL, 25, 40, 5, 5)
$brushDotL.Dispose()

$gL.Dispose()
$bmpL.Save("installer_logo.bmp", [System.Drawing.Imaging.ImageFormat]::Bmp)
$bmpL.Dispose()
Write-Host "installer_logo.bmp dibuat (55x55)" -ForegroundColor Green

Write-Host ""
Write-Host "Selesai! Gambar installer premium telah dibuat." -ForegroundColor Cyan
