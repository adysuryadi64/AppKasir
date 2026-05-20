' placeholder
Imports System.Reflection
Imports System.Drawing.Text

' ════════════════════════════════════════════════════════════════════
'  MODULE THEME — AppKasir
'  Satu-satunya modul tema untuk seluruh aplikasi.
'  Semua warna, helper, dan logika tema ada di sini.
'
'  PRINSIP: SEDIKIT WARNA = FOKUS TINGGI
'  • Hanya 3 aksen: Biru (primary), Hijau (tambah/sukses), Merah (hapus/bahaya)
'  • Warna semantik hanya untuk sinyal penting (peringatan, nilai keuangan, notifikasi)
'  • Tidak ada warna dekoratif — setiap warna harus punya makna
'
'  LIGHT "Fintech Clean" — background putih/slate-50, border hitam pekat
'  DARK  "SaaS Midnight" — background Slate-900, border putih terang
' ════════════════════════════════════════════════════════════════════
Public Module ModuleTheme

#Region "State & Config"

    ''' <summary>True = dark mode aktif. Sumber kebenaran tunggal untuk seluruh aplikasi.</summary>
    Public IsDarkMode As Boolean = False

    ''' <summary>Balik mode dan simpan ke AppConfig.</summary>
    Public Sub Toggle()
        IsDarkMode = Not IsDarkMode
        SaveToConfig()
    End Sub

    ''' <summary>Baca preferensi mode dari AppConfig — panggil sekali saat startup.</summary>
    Public Sub LoadFromConfig()
        IsDarkMode = AppConfig.Instance.GetValue(Of Boolean)("DarkMode", False)
    End Sub

    ''' <summary>Simpan preferensi mode ke AppConfig.</summary>
    Public Sub SaveToConfig()
        AppConfig.Instance.SetValue("DarkMode", IsDarkMode)
        AppConfig.Instance.Save()
    End Sub

#End Region

#Region "Palet Warna — Aksen Universal"

    ' Hanya 3 aksen — dipakai di kedua mode
    Public ReadOnly Aksen_Biru As Color = Color.FromArgb(37, 99, 235)   ' #2563EB — primary/edit
    Public ReadOnly Aksen_Hijau As Color = Color.FromArgb(22, 163, 74)   ' #16A34A — tambah/sukses
    Public ReadOnly Aksen_Merah As Color = Color.FromArgb(220, 38, 38)   ' #DC2626 — hapus/bahaya

#End Region

#Region "Token Primitif — Nilai Warna Dasar"
    ' ════════════════════════════════════════════════════════════════
    '  UBAH DI SINI untuk mengubah warna seluruh aplikasi sekaligus.
    '  Token semantik di bawah hanya mereferensikan token ini.
    ' ════════════════════════════════════════════════════════════════

    ' ── Light ────────────────────────────────────────────────────────
    Public ReadOnly L_Primary As Color = Color.FromArgb(37, 99, 235)    ' #2563EB Blue-600
    Public ReadOnly L_PrimaryHover As Color = Color.FromArgb(29, 78, 216)    ' #1D4ED8 Blue-700
    Public ReadOnly L_Success As Color = Color.FromArgb(22, 163, 74)    ' #16A34A Green-600
    Public ReadOnly L_SuccessHover As Color = Color.FromArgb(21, 128, 61)    ' #15803D Green-700
    Public ReadOnly L_SuccessDown As Color = Color.FromArgb(20, 83, 45)     ' #14532D Green-900
    Public ReadOnly L_Danger As Color = Color.FromArgb(220, 38, 38)    ' #DC2626 Red-600
    Public ReadOnly L_DangerHover As Color = Color.FromArgb(185, 28, 28)    ' #B91C1C Red-700
    Public ReadOnly L_DangerDown As Color = Color.FromArgb(127, 29, 29)    ' #7F1D1D Red-900
    Public ReadOnly L_Warning As Color = Color.FromArgb(217, 119, 6)    ' #D97706 Amber-600
    Public ReadOnly L_Orange As Color = Color.FromArgb(194, 65, 12)    ' #C2410C Orange-700
    Public ReadOnly L_Bg As Color = Color.FromArgb(248, 250, 252)  ' #F8FAFC Slate-50
    Public ReadOnly L_Surface As Color = Color.FromArgb(255, 255, 255)  ' #FFFFFF putih
    Public ReadOnly L_Subtle As Color = Color.FromArgb(241, 245, 249)  ' #F1F5F9 Slate-100
    Public ReadOnly L_Border As Color = Color.FromArgb(203, 213, 225)  ' #CBD5E1 Slate-300
    Public ReadOnly L_Muted As Color = Color.FromArgb(148, 163, 184)  ' #94A3B8 Slate-400
    Public ReadOnly L_Secondary As Color = Color.FromArgb(71, 85, 105)    ' #475569 Slate-600
    Public ReadOnly L_Text As Color = Color.FromArgb(15, 23, 42)     ' #0F172A Slate-900
    Public ReadOnly L_InputArea As Color = Color.FromArgb(191, 219, 254)  ' #BFDBFE Blue-200 (Lebih gelap dari Blue-100)
    Public ReadOnly L_SearchFocusBg As Color = Color.FromArgb(254, 240, 138)  ' #FEF08A Yellow-200
    Public ReadOnly L_NotifDanger As Color = Color.FromArgb(254, 202, 202)  ' #FECACA Red-200
    Public ReadOnly L_NotifInfo As Color = Color.FromArgb(191, 219, 254)  ' #BFDBFE Blue-200

    ' ── Dark ─────────────────────────────────────────────────────────
    Public ReadOnly D_Primary As Color = Color.FromArgb(59, 130, 246)   ' #3B82F6 Blue-400
    Public ReadOnly D_PrimaryHover As Color = Color.FromArgb(96, 165, 250)   ' #60A5FA Blue-300
    Public ReadOnly D_Success As Color = Color.FromArgb(34, 197, 94)    ' #22C55E Green-500
    Public ReadOnly D_SuccessHover As Color = Color.FromArgb(22, 163, 74)    ' #16A34A Green-600
    Public ReadOnly D_SuccessDown As Color = Color.FromArgb(20, 83, 45)     ' #14532D Green-900
    Public ReadOnly D_Danger As Color = Color.FromArgb(248, 113, 113)  ' #F87171 Red-400
    Public ReadOnly D_DangerHover As Color = Color.FromArgb(220, 38, 38)    ' #DC2626 Red-600
    Public ReadOnly D_DangerDown As Color = Color.FromArgb(127, 29, 29)    ' #7F1D1D Red-900
    Public ReadOnly D_Warning As Color = Color.FromArgb(251, 191, 36)   ' #FBBF24 Amber-400
    Public ReadOnly D_Orange As Color = Color.FromArgb(249, 115, 22)   ' #F97316 Orange-500
    Public ReadOnly D_Bg As Color = Color.FromArgb(15, 23, 42)     ' #0F172A Slate-900
    Public ReadOnly D_Surface As Color = Color.FromArgb(30, 41, 59)     ' #1E293B Slate-800
    Public ReadOnly D_Subtle As Color = Color.FromArgb(51, 65, 85)     ' #334155 Slate-700
    Public ReadOnly D_Border As Color = Color.FromArgb(71, 85, 105)    ' #475569 Slate-600
    Public ReadOnly D_Muted As Color = Color.FromArgb(148, 163, 184)  ' #94A3B8 Slate-400
    Public ReadOnly D_Secondary As Color = Color.FromArgb(148, 163, 184)  ' #94A3B8 Slate-400
    Public ReadOnly D_Text As Color = Color.FromArgb(226, 232, 240)  ' #E2E8F0 Slate-200
    Public ReadOnly D_InputArea As Color = Color.FromArgb(51, 65, 85)     ' #334155 Slate-700
    Public ReadOnly D_SearchFocusBg As Color = Color.FromArgb(120, 53, 15)    ' #78350F Amber-900
    Public ReadOnly D_NotifDanger As Color = Color.FromArgb(127, 29, 29)    ' #7F1D1D Red-900
    Public ReadOnly D_NotifInfo As Color = Color.FromArgb(29, 78, 216)    ' #1D4ED8 Blue-700

    ' ── Universal ────────────────────────────────────────────────────
    Public ReadOnly White As Color = Color.FromArgb(255, 255, 255)  ' #FFFFFF

#End Region

#Region "Token Semantik — Fungsi → Token Primitif"
    ' ════════════════════════════════════════════════════════════════
    '  Tidak ada Color.FromArgb di sini — hanya referensi ke token primitif.
    '  Nama field dipertahankan agar tidak ada perubahan di form lain.
    ' ════════════════════════════════════════════════════════════════

    ' ── Layout ──────────────────────────────────────────────────────
    Public ReadOnly L_FormBack As Color = L_Bg
    Public ReadOnly L_Toolbar As Color = L_Surface
    Public ReadOnly L_ToolbarFore As Color = L_Text
    Public ReadOnly L_Panel As Color = L_Bg
    Public ReadOnly L_PanelFore As Color = L_Text
    Public ReadOnly L_SurfaceFore As Color = L_Text

    Public ReadOnly D_FormBack As Color = D_Bg
    Public ReadOnly D_Toolbar As Color = D_Bg
    Public ReadOnly D_ToolbarFore As Color = D_Text
    Public ReadOnly D_Panel As Color = D_Bg
    Public ReadOnly D_PanelFore As Color = D_Text
    Public ReadOnly D_SurfaceFore As Color = D_Text

    ' ── Border ──────────────────────────────────────────────────────
    Public ReadOnly L_BtnBorder As Color = L_Text
    Public ReadOnly D_BtnBorder As Color = D_Text

    ' ── Button CRUD ─────────────────────────────────────────────────
    Public ReadOnly L_BtnBack As Color = L_Surface
    Public ReadOnly L_BtnHover As Color = L_Border
    Public ReadOnly L_BtnDown As Color = L_Border

    Public ReadOnly D_BtnBack As Color = D_Surface
    Public ReadOnly D_BtnHover As Color = D_Subtle
    Public ReadOnly D_BtnDown As Color = D_Border

    ' ── Button solid Tambah ─────────────────────────────────────────
    Public ReadOnly L_BtnSolidTambah As Color = L_Success
    Public ReadOnly L_BtnSolidTambahHover As Color = L_SuccessHover
    Public ReadOnly L_BtnSolidTambahDown As Color = L_SuccessDown
    Public ReadOnly D_BtnSolidTambah As Color = D_SuccessHover
    Public ReadOnly D_BtnSolidTambahHover As Color = D_Success
    Public ReadOnly D_BtnSolidTambahDown As Color = D_SuccessDown

    ' ── Button solid Keluar ─────────────────────────────────────────
    Public ReadOnly L_BtnSolidKeluar As Color = L_Danger
    Public ReadOnly L_BtnSolidKeluarHover As Color = L_DangerHover
    Public ReadOnly L_BtnSolidKeluarDown As Color = L_DangerDown
    Public ReadOnly D_BtnSolidKeluar As Color = D_DangerHover
    Public ReadOnly D_BtnSolidKeluarHover As Color = D_Danger
    Public ReadOnly D_BtnSolidKeluarDown As Color = D_DangerDown

    ' ── Teks fungsi button ──────────────────────────────────────────
    Public ReadOnly L_ForeTambah As Color = L_Success
    Public ReadOnly L_ForeEdit As Color = L_Primary
    Public ReadOnly L_ForeHapus As Color = L_Danger
    Public ReadOnly L_ForeCetak As Color = L_Secondary
    Public ReadOnly L_ForeKeluar As Color = L_Danger
    Public ReadOnly D_ForeTambah As Color = D_Success
    Public ReadOnly D_ForeEdit As Color = D_Primary
    Public ReadOnly D_ForeHapus As Color = D_Danger
    Public ReadOnly D_ForeCetak As Color = D_Muted
    Public ReadOnly D_ForeKeluar As Color = D_Danger

    ' ── Nav buttons ─────────────────────────────────────────────────
    Public ReadOnly L_NavIdle As Color = L_Surface
    Public ReadOnly L_NavIdleFore As Color = L_Text
    Public ReadOnly L_NavHover As Color = L_Border
    Public ReadOnly L_NavDown As Color = L_Border
    Public ReadOnly L_NavActive As Color = L_Primary
    Public ReadOnly L_NavActiveFore As Color = White
    Public ReadOnly L_NavActiveHover As Color = L_PrimaryHover
    Public ReadOnly L_MenuActive As Color = L_Primary
    Public ReadOnly D_NavIdle As Color = D_Surface
    Public ReadOnly D_NavIdleFore As Color = D_Text
    Public ReadOnly D_NavHover As Color = D_Subtle
    Public ReadOnly D_NavDown As Color = D_Border
    Public ReadOnly D_NavActive As Color = D_Primary
    Public ReadOnly D_NavActiveFore As Color = White
    Public ReadOnly D_NavActiveHover As Color = D_PrimaryHover
    Public ReadOnly D_MenuActive As Color = D_Primary

    ' ── DataGridView ────────────────────────────────────────────────
    Public ReadOnly L_DgvHeader As Color = L_Border
    Public ReadOnly L_DgvHeaderFore As Color = L_Text
    Public ReadOnly L_DgvAlt As Color = L_Border
    Public ReadOnly L_DgvGrid As Color = L_Muted
    Public ReadOnly L_DgvSelect As Color = L_Primary
    Public ReadOnly D_DgvHeader As Color = D_Subtle
    Public ReadOnly D_DgvHeaderFore As Color = D_Text
    Public ReadOnly D_DgvAlt As Color = D_Surface
    Public ReadOnly D_DgvGrid As Color = D_Border
    Public ReadOnly D_DgvSelect As Color = D_Primary

    ' ── StatusBar ───────────────────────────────────────────────────
    Public ReadOnly L_StatusBar As Color = L_Subtle
    Public ReadOnly L_StatusFore As Color = L_Secondary
    Public ReadOnly L_StatusAccent As Color = L_Primary
    Public ReadOnly D_StatusBar As Color = D_Bg
    Public ReadOnly D_StatusFore As Color = D_Muted
    Public ReadOnly D_StatusAccent As Color = D_Primary

    ' ── Header kategori form ─────────────────────────────────────────
    ' Konvensi penamaan form → warna header otomatis (lihat GetKategoriForm)
    ' Master    : nama mengandung tidak ada kata kunci khusus → fallback biru
    ' Transaksi : Jual/Beli/Pembelian/Penjualan/Retur/Bayar/Stok/Transfer/Surat/Cabang/Keuangan
    ' Laporan   : Lap/Laporan/Grafik/Kartu/Report/Notif/Ropert/Omset/Ranking/PPn
    ' Gaji      : Gaji/Bon/MasterGaji/LapBon/LaporanGaji
    ' Cetak     : Cetak/Barcode/Label/Print
    ' Sync      : Query/Sync/Update/Setting/Migrasi/Database
    Public ReadOnly L_HeaderMaster As Color = L_Primary
    Public ReadOnly L_HeaderTransaksi As Color = L_Success
    Public ReadOnly L_HeaderGaji As Color = Color.FromArgb(124, 58, 237)  ' #7C3AED Violet-600
    Public ReadOnly L_HeaderLaporan As Color = Color.FromArgb(8, 145, 178)   ' #0891B2 Cyan-600
    Public ReadOnly L_HeaderSync As Color = L_Secondary
    Public ReadOnly L_HeaderCetak As Color = Color.FromArgb(100, 116, 139) ' #64748B Slate-500
    Public ReadOnly L_HeaderFore As Color = White
    Public ReadOnly D_HeaderMaster As Color = Color.FromArgb(29, 78, 216)   ' #1D4ED8 Blue-700
    Public ReadOnly D_HeaderTransaksi As Color = Color.FromArgb(21, 128, 61)   ' #15803D Green-700
    Public ReadOnly D_HeaderGaji As Color = Color.FromArgb(109, 40, 217)  ' #6D28D9 Violet-700
    Public ReadOnly D_HeaderLaporan As Color = Color.FromArgb(14, 116, 144)  ' #0E7490 Cyan-700
    Public ReadOnly D_HeaderCetak As Color = Color.FromArgb(71, 85, 105)    ' #475569 Slate-600
    Public ReadOnly D_HeaderSync As Color = D_Muted
    Public ReadOnly D_HeaderFore As Color = White

    ' ── Area input transaksi (GroupBox/Panel berisi field input) ─────
    Public ReadOnly L_TransInput As Color = L_InputArea
    Public ReadOnly L_TransInputFore As Color = L_Text
    Public ReadOnly D_TransInput As Color = D_InputArea
    Public ReadOnly D_TransInputFore As Color = D_Text

    ' ── Grand total (TextBox besar — hitam+hijau, kontras tinggi) ────
    Public ReadOnly L_TransTotal As Color = L_Text
    Public ReadOnly L_TransTotalFore As Color = L_Success
    Public ReadOnly D_TransTotal As Color = Color.FromArgb(2, 6, 23)      ' #020617 hampir hitam
    Public ReadOnly D_TransTotalFore As Color = Color.FromArgb(74, 222, 128)  ' #4ADE80 Green-400

    ' ── Nilai keuangan penting (ForeColor hijau) ─────────────────────
    Public ReadOnly L_TransNilai As Color = L_Success
    Public ReadOnly D_TransNilai As Color = Color.FromArgb(74, 222, 128)      ' #4ADE80 Green-400

    ' ── Status transaksi (LUNAS / Belum Lunas) ───────────────────────
    Public ReadOnly L_StatusLunas As Color = L_Success
    Public ReadOnly L_StatusBelumLunas As Color = L_Warning
    Public ReadOnly D_StatusLunas As Color = Color.FromArgb(74, 222, 128)  ' #4ADE80 Green-400
    Public ReadOnly D_StatusBelumLunas As Color = D_Warning

    ' ── Label peringatan stok kritis (merah) ─────────────────────────
    Public ReadOnly L_LabelWarning As Color = L_DangerHover
    Public ReadOnly D_LabelWarning As Color = D_Danger

    ' ── Highlight baris DGV — warna semantik untuk validasi transaksi ─
    ' Gunakan token ini di semua form transaksi agar konsisten:
    '   DgvRowStokHabis   : kolom stok = 0 di CellFormatting (informasi, bukan error)
    '   DgvRowPeringatan  : baris yang gagal validasi CekStok/Cekjualrugi (peringatan, user masih bisa ubah)
    '   DgvRowKonflik     : baris yang gagal validasi SP race condition (konflik multi-user)
    '   DgvRowError       : baris yang gagal karena error sistem (DataError, dll)
    Public ReadOnly L_DgvRowStokHabis As Color = L_Warning      ' Amber — stok 0, informasi
    Public ReadOnly D_DgvRowStokHabis As Color = D_Warning
    Public ReadOnly L_DgvRowPeringatan As Color = L_Warning     ' Amber — peringatan validasi
    Public ReadOnly D_DgvRowPeringatan As Color = D_Warning
    Public ReadOnly L_DgvRowKonflik As Color = L_Warning        ' Amber — konflik multi-user
    Public ReadOnly D_DgvRowKonflik As Color = D_Warning
    Public ReadOnly L_DgvRowError As Color = L_Danger           ' Merah — error sistem
    Public ReadOnly D_DgvRowError As Color = D_Danger

    ' ── Panel notifikasi jatuh tempo ─────────────────────────────────
    Public ReadOnly L_PanelNotifMerah As Color = L_NotifDanger
    Public ReadOnly L_PanelNotifBiru As Color = L_NotifInfo
    Public ReadOnly D_PanelNotifMerah As Color = D_NotifDanger
    Public ReadOnly D_PanelNotifBiru As Color = D_NotifInfo

    ' ── TxtTransaksi (display nomor transaksi di FormUtama) ──────────
    Public ReadOnly L_TxtTransaksi As Color = L_Primary
    Public ReadOnly D_TxtTransaksi As Color = D_Primary

    ' ── RichTextBox catatan/alasan (warna hangat — beda dari TextBox data) ──
    Public ReadOnly L_RtbCatatan As Color = Color.FromArgb(255, 253, 235)     ' #FFFBEB Yellow-50
    Public ReadOnly D_RtbCatatan As Color = Color.FromArgb(41, 37, 36)        ' #292524 Stone-800

    ' ── Area pencarian aktif (kontras tinggi saat fokus) ─────────────
    Public ReadOnly L_SearchFocus As Color = L_SearchFocusBg
    Public ReadOnly D_SearchFocus As Color = D_SearchFocusBg

    ' ── Tombol aksi di dalam DataGridView ────────────────────────────
    Public ReadOnly L_DgvBtnEdit As Color = Color.FromArgb(99, 102, 241)    ' #6366F1 Indigo-500
    Public ReadOnly L_DgvBtnHapus As Color = Color.FromArgb(239, 68, 68)     ' #EF4444 Red-500
    Public ReadOnly L_DgvBtnNonaktif As Color = Color.FromArgb(245, 158, 11)   ' #F59E0B Amber-500
    Public ReadOnly L_DgvBtnAktif As Color = Color.FromArgb(34, 197, 94)     ' #22C55E Green-500
    Public ReadOnly L_DgvBtnDisabled As Color = Color.FromArgb(229, 231, 235)  ' #E5E7EB Gray-200
    Public ReadOnly L_DgvBtnDisabledFore As Color = Color.FromArgb(107, 114, 128)  ' #6B7280 Gray-500
    Public ReadOnly D_DgvBtnEdit As Color = Color.FromArgb(129, 140, 248)   ' #818CF8 Indigo-400
    Public ReadOnly D_DgvBtnHapus As Color = Color.FromArgb(248, 113, 113)  ' #F87171 Red-400
    Public ReadOnly D_DgvBtnNonaktif As Color = Color.FromArgb(251, 191, 36)  ' #FBBF24 Amber-400
    Public ReadOnly D_DgvBtnAktif As Color = Color.FromArgb(74, 222, 128)    ' #4ADE80 Green-400
    Public ReadOnly D_DgvBtnDisabled As Color = Color.FromArgb(75, 85, 99)     ' #4B5563 Gray-600
    Public ReadOnly D_DgvBtnDisabledFore As Color = Color.FromArgb(156, 163, 175)  ' #9CA3AF Gray-400
    Public ReadOnly DgvBtnFore As Color = White

    ' ── Baris nonaktif/disabled di DataGridView ──────────────────────
    Public ReadOnly L_DgvRowNonaktif As Color = L_Muted
    Public ReadOnly L_DgvRowNonaktifBack As Color = L_Subtle
    Public ReadOnly D_DgvRowNonaktif As Color = D_Border
    Public ReadOnly D_DgvRowNonaktifBack As Color = D_Surface

#End Region

#Region "Helper"

    Private _pfcDigital As PrivateFontCollection = Nothing

    ''' <summary>
    ''' Mengambil memori font Digital-7 dari file .ttf tanpa instalasi OS.
    ''' Memastikan TextBox besar di Windows 7 tidak fallback ke Sans Serif.
    ''' </summary>
    Public Function GetDigitalFont(size As Single, style As FontStyle) As Font
        Try
            If _pfcDigital Is Nothing Then
                _pfcDigital = New PrivateFontCollection()
                Dim fontPath As String = IO.Path.Combine(Application.StartupPath, "Fonts", "digital-7.ttf")
                If IO.File.Exists(fontPath) Then
                    _pfcDigital.AddFontFile(fontPath)
                End If
            End If

            If _pfcDigital.Families.Length > 0 Then
                Return New Font(_pfcDigital.Families(0), size, style)
            End If
        Catch ex As Exception
            ' Fallback diam-diam jika gagal load file
        End Try
        ' Fallback ke Sans Serif standar jika tidak ada font fisik
        Return New Font("Microsoft Sans Serif", size, style)
    End Function

    ''' <summary>Kembalikan warna sesuai mode aktif.</summary>
    Public Function C(light As Color, dark As Color) As Color
        Return If(IsDarkMode, dark, light)
    End Function

    ''' <summary>
    ''' Cek apakah font dengan nama tertentu terinstall di sistem.
    ''' Dipakai untuk fallback font DGV transaksi (Century Gothic → Segoe UI).
    ''' </summary>
    Public Function IsFontInstalled(namaFont As String) As Boolean
        Using fonts As New InstalledFontCollection()
            For Each ff As FontFamily In fonts.Families
                If String.Equals(ff.Name, namaFont, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
        End Using
        Return False
    End Function

    ''' <summary>
    ''' Tentukan kategori form berdasarkan nama — dipakai untuk warna header otomatis.
    ''' Konvensi penamaan form harus mengandung kata kunci kategori.
    ''' </summary>
    Public Function GetKategoriForm(formName As String) As String
        Select Case True
            Case formName.Contains("Gaji") OrElse formName.Contains("Bon") OrElse
                 formName.Contains("MasterGaji") OrElse formName.Contains("LapBon") OrElse
                 formName.Contains("LaporanGaji")
                Return "Gaji"
            Case formName.Contains("Lap") OrElse formName.Contains("Laporan") OrElse
                 formName.Contains("Grafik") OrElse formName.Contains("Kartu") OrElse
                 formName.Contains("Report") OrElse formName.Contains("Notif") OrElse
                 formName.Contains("Ropert") OrElse formName.Contains("Omset") OrElse
                 formName.Contains("Ranking") OrElse formName.Contains("PPn")
                Return "Laporan"
            Case formName.Contains("Cetak") OrElse formName.Contains("Barcode") OrElse
                 formName.Contains("Label") OrElse formName.Contains("Print")
                Return "Cetak"
            Case formName.Contains("Query") OrElse formName.Contains("Sync") OrElse
                 formName.Contains("Update") OrElse formName.Contains("Setting") OrElse
                 formName.Contains("Migrasi") OrElse formName.Contains("Database") OrElse
                 formName.Contains("History") OrElse formName.Contains("Perbaikan")
                Return "Sync"
            Case formName.Contains("Jual") OrElse formName.Contains("Beli") OrElse
                 formName.Contains("Pembelian") OrElse formName.Contains("Penjualan") OrElse
                 formName.Contains("Retur") OrElse formName.Contains("Bayar") OrElse
                 formName.Contains("Stok") OrElse formName.Contains("Transfer") OrElse
                 formName.Contains("Surat") OrElse formName.Contains("Cabang") OrElse
                 formName.Contains("Keuangan")
                Return "Transaksi"
            Case Else
                Return "Master"
        End Select
    End Function

    ''' <summary>Kembalikan pasangan warna header (back, fore) sesuai kategori dan mode.</summary>
    Public Function GetWarnaHeader(kategori As String) As (Back As Color, Fore As Color)
        Dim fore As Color = C(L_HeaderFore, D_HeaderFore)
        Select Case kategori
            Case "Gaji" : Return (C(L_HeaderGaji, D_HeaderGaji), fore)
            Case "Laporan" : Return (C(L_HeaderLaporan, D_HeaderLaporan), fore)
            Case "Cetak" : Return (C(L_HeaderCetak, D_HeaderCetak), fore)
            Case "Sync" : Return (C(L_HeaderSync, D_HeaderSync), fore)
            Case "Transaksi" : Return (C(L_HeaderTransaksi, D_HeaderTransaksi), fore)
            Case Else : Return (C(L_HeaderMaster, D_HeaderMaster), fore)
        End Select
    End Function

    ''' <summary>Icon BtnMode — dark-mode.png saat light aktif, light-mode.png saat dark aktif.</summary>
    Public Function GetModeIcon(size As Integer) As Image
        Dim baseName As String = If(IsDarkMode, "light-mode", "dark-mode")
        Dim suffix As String = If(size = 16, "", $"_{size}")
        Dim iconPath As String = IO.Path.Combine(
            IO.Path.GetDirectoryName(Application.ExecutablePath),
            "Resources", "Icons", $"{baseName}{suffix}.png")
        If IO.File.Exists(iconPath) Then Return Image.FromFile(iconPath)
        Return Nothing
    End Function

    ''' <summary>Tooltip BtnMode sesuai mode aktif.</summary>
    Public Function GetModeTooltip() As String
        Return If(IsDarkMode, "Beralih ke Mode Terang", "Beralih ke Mode Gelap")
    End Function

#End Region

#Region "TerapkanTheme — Entry Point"

    ''' <summary>
    ''' Entry point utama — panggil di Load event setiap form.
    ''' MDI child: dipanggil otomatis via MdiChildActivate di FormUtama.
    ''' ShowDialog: wajib dipanggil manual di Load event form itu sendiri.
    ''' </summary>
    Public Sub TerapkanTheme(frm As Form)
        ' Form Pola A (BackColor + Padding sebagai border) — warna sesuai kategori
        If frm.Padding.Left >= 6 AndAlso frm.Padding.Right >= 6 Then
            Dim kategori As String = GetKategoriForm(frm.Name)
            Dim warna = GetWarnaHeader(kategori)
            frm.BackColor = warna.Back
        Else
            frm.BackColor = C(L_FormBack, D_FormBack)
        End If

        frm.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        TerapkanKontrol(frm.Controls)

        If TypeOf frm Is FormUtama Then
            TerapkanFormUtama(CType(frm, FormUtama))
        End If
    End Sub

    ''' <summary>
    ''' Rekursif ke semua kontrol dalam form.
    ''' Urutan case penting — tipe yang lebih spesifik harus di atas.
    ''' </summary>
    Public Sub TerapkanKontrol(controls As Control.ControlCollection)
        For Each ctrl As Control In controls
            Select Case True

                Case TypeOf ctrl Is MenuStrip
                    If ctrl.FindForm() IsNot Nothing AndAlso TypeOf ctrl.FindForm() Is FormUtama Then
                        ' FormUtama ditangani TerapkanFormUtama — skip
                    Else
                        TerapkanMenuStrip(CType(ctrl, MenuStrip))
                    End If

                Case TypeOf ctrl Is StatusStrip
                    Dim ss = CType(ctrl, StatusStrip)
                    ss.BackColor = C(L_StatusBar, D_StatusBar)
                    ss.ForeColor = C(L_StatusFore, D_StatusFore)

                Case TypeOf ctrl Is Panel
                    TerapkanPanel(CType(ctrl, Panel))

                Case TypeOf ctrl Is GroupBox
                    TerapkanGroupBox(CType(ctrl, GroupBox))

                Case TypeOf ctrl Is DataGridView
                    ApplyThemeDataGridView(CType(ctrl, DataGridView))

                Case TypeOf ctrl Is Button
                    TerapkanButton(CType(ctrl, Button))

                Case TypeOf ctrl Is ContextMenuStrip
                    TerapkanContextMenu(CType(ctrl, ContextMenuStrip))

                Case TypeOf ctrl Is RichTextBox
                    ' RichTextBox standar — warna surface biasa
                    ' Untuk RichTextBox catatan, panggil SetWarnaRtbCatatan() di Load
                    Dim rtb = CType(ctrl, RichTextBox)
                    rtb.BackColor = C(L_Surface, D_Surface)
                    rtb.ForeColor = C(L_SurfaceFore, D_SurfaceFore)

                Case TypeOf ctrl Is TextBox
                    TerapkanTextBox(CType(ctrl, TextBox))

                Case TypeOf ctrl Is ComboBox
                    TerapkanComboBox(CType(ctrl, ComboBox))

                Case TypeOf ctrl Is Label
                    TerapkanLabel(CType(ctrl, Label))

                Case TypeOf ctrl Is DateTimePicker
                    Dim dtp = CType(ctrl, DateTimePicker)
                    dtp.BackColor = C(L_Surface, D_Surface)
                    dtp.ForeColor = C(L_SurfaceFore, D_SurfaceFore)

                Case TypeOf ctrl Is CheckBox
                    ctrl.ForeColor = C(L_PanelFore, D_PanelFore)
                    ctrl.BackColor = Color.Transparent

                Case TypeOf ctrl Is RadioButton
                    ctrl.ForeColor = C(L_PanelFore, D_PanelFore)
                    ctrl.BackColor = Color.Transparent

                Case TypeOf ctrl Is ListBox
                    Dim lb = CType(ctrl, ListBox)
                    lb.BackColor = C(L_Surface, D_Surface)
                    lb.ForeColor = C(L_SurfaceFore, D_SurfaceFore)

                Case TypeOf ctrl Is NumericUpDown
                    Dim nud = CType(ctrl, NumericUpDown)
                    nud.BackColor = C(L_Surface, D_Surface)
                    nud.ForeColor = C(L_SurfaceFore, D_SurfaceFore)

                Case TypeOf ctrl Is TabControl
                    Dim tc = CType(ctrl, TabControl)
                    tc.BackColor = C(L_Surface, D_Surface)
                    For Each tp As TabPage In tc.TabPages
                        tp.BackColor = C(L_Surface, D_Surface)
                        tp.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
                        TerapkanKontrol(tp.Controls)
                    Next

                Case Else
                    Dim typeName As String = ctrl.GetType().FullName
                    If typeName IsNot Nothing AndAlso typeName.Contains("ReportViewer") Then
                        ' ReportViewer — skip sepenuhnya, tidak set warna apapun dan tidak rekursif
                        ' agar rendering laporan tidak rusak
                    ElseIf ctrl.Controls.Count > 0 Then
                        TerapkanKontrol(ctrl.Controls)
                    End If

            End Select
        Next
    End Sub

#End Region

#Region "TerapkanTheme — Kontrol Spesifik"

    ''' <summary>
    ''' Terapkan warna GroupBox berdasarkan nama (4 warna berbeda):
    ''' - GBInput/GBInput1/GBInput2/GBBayar: Area input user (biru muda/Slate-700)
    ''' - GBInfo/GBInfo1/GBInfo2: Area info/detail (hijau muda/Slate-800)
    ''' - GBTotal/GBTotal1/GBTotal2: Area total/ringkasan (kuning muda/Slate-800)
    ''' - GBAction/GBAction1/GBAction2: Area aksi/tombol (ungu muda/Slate-800)
    ''' - Lainnya: Warna panel biasa
    ''' </summary>
    Private Sub TerapkanGroupBox(gb As GroupBox)
        Dim gbName As String = gb.Name

        Select Case True
            ' ── Area input user (biru — mencolok, beda dari background) ─
            Case gbName.StartsWith("GBInput")
                gb.BackColor = C(Color.FromArgb(147, 197, 253), Color.FromArgb(30, 58, 138))  ' Blue-300 / Blue-900
                gb.ForeColor = C(L_Text, Color.FromArgb(191, 219, 254))                        ' Slate-900 / Blue-200
                TerapkanKontrol(gb.Controls)

            ' ── Area bayar (hijau — mencolok) ───────────────────────────
            Case gbName = "GBBayar"
                gb.BackColor = C(Color.FromArgb(134, 239, 172), Color.FromArgb(20, 83, 45))   ' Green-300 / Green-900
                gb.ForeColor = C(L_Text, Color.FromArgb(187, 247, 208))                        ' Slate-900 / Green-200
                TerapkanKontrol(gb.Controls)

            ' ── Area info/detail (hijau muda — beda dari GBBayar) ───────
            Case gbName.StartsWith("GBInfo")
                gb.BackColor = C(Color.FromArgb(167, 243, 208), Color.FromArgb(6, 78, 59))    ' Emerald-200 / Emerald-900
                gb.ForeColor = C(L_Text, Color.FromArgb(167, 243, 208))                        ' Slate-900 / Emerald-200
                TerapkanKontrol(gb.Controls)

            ' ── Area total/ringkasan (kuning — mencolok) ─────────────────
            Case gbName.StartsWith("GBTotal")
                gb.BackColor = C(Color.FromArgb(253, 224, 71), Color.FromArgb(113, 63, 18))   ' Yellow-300 / Yellow-900
                gb.ForeColor = C(L_Text, Color.FromArgb(254, 240, 138))                        ' Slate-900 / Yellow-200
                TerapkanKontrol(gb.Controls)

            ' ── Area grand total (hitam pekat + teks putih — kedua mode sama) ──
            Case gbName.StartsWith("GBGrantotal")
                gb.BackColor = Color.FromArgb(15, 23, 42)   ' #0F172A Slate-900 — hitam pekat
                gb.ForeColor = Color.White
                TerapkanKontrol(gb.Controls)

            ' ── Area aksi/tombol (ungu — mencolok) ──────────────────────
            Case gbName.StartsWith("GBAction")
                gb.BackColor = C(Color.FromArgb(196, 181, 253), Color.FromArgb(76, 29, 149))  ' Violet-300 / Violet-900
                gb.ForeColor = C(L_Text, Color.FromArgb(221, 214, 254))                        ' Slate-900 / Violet-200
                TerapkanKontrol(gb.Controls)

            ' ── Semua GroupBox lain ─────────────────────────────────────
            Case Else
                gb.BackColor = C(L_Panel, D_Panel)
                gb.ForeColor = C(L_PanelFore, D_PanelFore)
                TerapkanKontrol(gb.Controls)

        End Select
    End Sub

    ''' <summary>
    ''' Pasang custom Paint handler agar bingkai GroupBox selalu kontras.
    ''' WinForms tidak mendukung BorderColor langsung — harus digambar manual.
    ''' Handler lama dilepas dulu agar tidak menumpuk saat tema diulang.
    ''' </summary>
    Private Sub TerapkanBingkaiGroupBox(gb As GroupBox, borderColor As Color)
        ' Lepas handler lama jika ada (simpan di Tag agar bisa dilepas)
        If gb.Tag IsNot Nothing AndAlso TypeOf gb.Tag Is EventHandler Then
            RemoveHandler gb.Paint, CType(gb.Tag, PaintEventHandler)
        End If

        Dim handler As PaintEventHandler = Sub(sender As Object, e As PaintEventArgs)
            Dim g As Graphics = e.Graphics
            Dim box As GroupBox = CType(sender, GroupBox)
            Dim rect As New Rectangle(0, 0, box.Width - 1, box.Height - 1)

            ' Ukur lebar teks judul
            Dim titleSize As SizeF = g.MeasureString(box.Text, box.Font)
            Dim titleX As Integer = 8
            Dim titleW As Integer = CInt(titleSize.Width) + 4

            ' Gambar bingkai dengan 4 segmen — lewati area teks judul di atas
            Using pen As New Pen(borderColor, 1.5!)
                ' Kiri
                g.DrawLine(pen, rect.Left, rect.Top + CInt(titleSize.Height / 2), rect.Left, rect.Bottom)
                ' Bawah
                g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom)
                ' Kanan
                g.DrawLine(pen, rect.Right, rect.Top + CInt(titleSize.Height / 2), rect.Right, rect.Bottom)
                ' Atas kiri (sebelum teks)
                g.DrawLine(pen, rect.Left, rect.Top + CInt(titleSize.Height / 2), titleX - 2, rect.Top + CInt(titleSize.Height / 2))
                ' Atas kanan (setelah teks)
                g.DrawLine(pen, titleX + titleW, rect.Top + CInt(titleSize.Height / 2), rect.Right, rect.Top + CInt(titleSize.Height / 2))
            End Using

            ' Gambar ulang teks judul dengan warna ForeColor yang benar
            TextRenderer.DrawText(e.Graphics, box.Text, box.Font,
                                  New Point(titleX, 0), box.ForeColor)
        End Sub

        gb.Tag = handler
        AddHandler gb.Paint, handler
    End Sub

    Private Sub TerapkanBingkaiPanel(pnl As Panel, borderColor As Color)
        ' Lepas handler lama jika ada (simpan di Tag agar bisa dilepas)
        ' HATI-HATI: Jika panel menggunakan Tag untuk hal lain (seperti PanelCari), 
        ' kita harus memastikan tidak terjadi konflik.
        If pnl.Name.StartsWith("PanelCari") Then Return ' Lewati PanelCari karena menggunakan Tag untuk status fokus

        If pnl.Tag IsNot Nothing AndAlso TypeOf pnl.Tag Is PaintEventHandler Then
            RemoveHandler pnl.Paint, CType(pnl.Tag, PaintEventHandler)
        End If

        Dim handler As PaintEventHandler = Sub(sender As Object, e As PaintEventArgs)
            Dim g As Graphics = e.Graphics
            Dim p As Panel = CType(sender, Panel)
            Dim rect As New Rectangle(0, 0, p.Width - 1, p.Height - 1)

            Using pen As New Pen(borderColor, 1.0!)
                g.DrawRectangle(pen, rect)
            End Using
        End Sub

        pnl.Tag = handler
        AddHandler pnl.Paint, handler
    End Sub

    ''' <summary>
    ''' Terapkan warna panel berdasarkan 5 kategori standar:
    ''' - PanelHeader/PnlHeader: Header form (warna kategori)
    ''' - PanelInput/PanelInput1/PanelInput2: Area input user (biru muda/Slate-700)
    ''' - PanelCari/PanelCari1/PanelCari2: Area pencarian (warna biasa, fokus kuning via event)
    ''' - PanelGrid/PanelGrid1/PanelGrid2: Area DataGridView (menyatu dengan form)
    ''' - PanelFooter/PanelFooter1/PanelFooter2: Tombol aksi di bawah (seamless)
    ''' - Panel1-6 tipis (≤8px): Border dekoratif (warna kategori)
    ''' - Lainnya: Warna panel biasa
    ''' </summary>
    Private Sub TerapkanPanel(pnl As Panel)
        Dim formName As String = If(pnl.FindForm()?.Name, "")
        Dim panelName As String = pnl.Name

        Select Case True
            ' ── Header form ─────────────────────────────────────────────
            Case panelName = "PanelHeader" OrElse panelName = "PnlHeader"
                ' Warna kategori form — termasuk FormPengaturanPrinter (dapat warna Cetak/abu)
                Dim warnaHeader = GetWarnaHeader(GetKategoriForm(formName))
                pnl.BackColor = warnaHeader.Back
                pnl.ForeColor = warnaHeader.Fore
                TerapkanKontrol(pnl.Controls)

            ' ── Area input user (bukan pencarian) ──────────────────────
            Case panelName.StartsWith("PanelInput")
                pnl.BackColor = C(Color.FromArgb(147, 197, 253), Color.FromArgb(30, 58, 138))  ' Blue-300 / Blue-900
                pnl.ForeColor = C(L_Text, Color.FromArgb(191, 219, 254))                        ' Slate-900 / Blue-200
                TerapkanKontrol(pnl.Controls)
                TerapkanBingkaiPanel(pnl, C(L_Text, Color.White))

            ' ── Area pencarian (fokus kuning saat GotFocus) ───────
            Case panelName.StartsWith("PanelCari")
                ' Cek apakah sedang fokus (gunakan tag untuk tracking)
                If pnl.Tag?.ToString() = "FOCUSED" Then
                    pnl.BackColor = C(L_SearchFocus, D_SearchFocus)
                    pnl.ForeColor = C(L_Text, D_Text)
                Else
                    pnl.BackColor = C(L_Panel, D_Panel)
                    pnl.ForeColor = C(L_PanelFore, D_PanelFore)
                End If
                TerapkanKontrol(pnl.Controls)

            ' ── Area DataGridView ───────────────────────────────────────
            Case panelName.StartsWith("PanelGrid")
                pnl.BackColor = C(L_Panel, D_Panel)
                pnl.ForeColor = C(L_PanelFore, D_PanelFore)
                TerapkanKontrol(pnl.Controls)
                TerapkanBingkaiPanel(pnl, C(L_Border, D_Border))

            ' ── Tombol aksi di bawah ────────────────────────────────────
            Case panelName.StartsWith("PanelFooter")
                pnl.BackColor = C(Color.FromArgb(196, 181, 253), Color.FromArgb(76, 29, 149))  ' Violet-300 / Violet-900
                pnl.ForeColor = C(L_Text, Color.FromArgb(221, 214, 254))                        ' Slate-900 / Violet-200
                TerapkanKontrol(pnl.Controls)
                TerapkanBingkaiPanel(pnl, C(L_Text, Color.White))

            ' ── Batas dekoratif (kiri/kanan/bawah) — warna kategori form ─
            Case panelName.StartsWith("PnlBatas")
                Dim warnaBatas = GetWarnaHeader(GetKategoriForm(formName))
                pnl.BackColor = warnaBatas.Back

                ' ── Semua panel lain ────────────────────────────────────────
            Case Else
                pnl.BackColor = C(L_Panel, D_Panel)
                pnl.ForeColor = C(L_PanelFore, D_PanelFore)
                TerapkanKontrol(pnl.Controls)

        End Select
    End Sub

    Public Sub SetPanelCariFocus(panelCari As Panel, hasFocus As Boolean)
    If hasFocus Then
        panelCari.Tag = "FOCUSED"
        panelCari.BackColor = C(L_SearchFocus, D_SearchFocus)
        panelCari.ForeColor = C(L_Text, D_Text)
    Else
        panelCari.Tag = Nothing
        panelCari.BackColor = C(L_Panel, D_Panel)
        panelCari.ForeColor = C(L_PanelFore, D_PanelFore)
    End If
End Sub

    Private Sub TerapkanButton(btn As Button)
        btn.Cursor = Cursors.Hand
        btn.BackColor = C(L_BtnBack, D_BtnBack)
        btn.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        btn.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        btn.FlatAppearance.MouseOverBackColor = C(L_BtnHover, D_BtnHover)
        btn.FlatAppearance.MouseDownBackColor = C(L_BtnDown, D_BtnDown)
        TerapkanHoverBorder(btn)
    End Sub

    Private Sub TerapkanTextBox(tb As TextBox)
        ' ── TextBox Grand Total (hitam pekat + teks kuning — kedua mode sama) ───────
        If tb.Name.Equals("TxtGrandtotal", StringComparison.OrdinalIgnoreCase) OrElse
           tb.Name.Equals("TxtGrantotal", StringComparison.OrdinalIgnoreCase) Then
            tb.BackColor = Color.FromArgb(15, 23, 42)    ' #0F172A Slate-900 — hitam pekat
            tb.ForeColor = Color.White
            tb.BorderStyle = BorderStyle.FixedSingle
            ' Mencegah fallback GDI+ Windows 7: muat langsung dari memori .ttf
            tb.Font = GetDigitalFont(tb.Font.Size, FontStyle.Bold)
            Return
        End If

        ' ── TextBox Aktif vs Tidak Aktif ────────────────────────────
        If tb.ReadOnly OrElse Not tb.Enabled Then
            ' Tidak aktif — abu-abu terang (light & dark)
            tb.BackColor = C(Color.FromArgb(241, 245, 249), Color.FromArgb(51, 65, 85))  ' Slate-100 / Slate-700
            tb.ForeColor = C(Color.FromArgb(100, 116, 139), Color.FromArgb(148, 163, 184))  ' Slate-500 / Slate-400
        Else
            ' Aktif — selalu putih + hitam (di semua mode)
            tb.BackColor = Color.White
            tb.ForeColor = Color.Black
        End If
        tb.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Sub TerapkanComboBox(cb As ComboBox)
        ' ── ComboBox Aktif vs Tidak Aktif ───────────────────────────
        If Not cb.Enabled Then
            ' Tidak aktif — abu-abu terang
            cb.BackColor = C(Color.FromArgb(241, 245, 249), Color.FromArgb(51, 65, 85))  ' Slate-100 / Slate-700
            cb.ForeColor = C(Color.FromArgb(100, 116, 139), Color.FromArgb(148, 163, 184))  ' Slate-500 / Slate-400
        Else
            ' Aktif — selalu putih + hitam
            cb.BackColor = Color.White
            cb.ForeColor = Color.Black
        End If
    End Sub

    Private Sub TerapkanLabel(lbl As Label)
        ' ── LblTextJalanAtas — running text di penjualan ────────────
        ' BackColor transparan agar menyatu dengan PanelHeader (warna kategori form)
        If lbl.Name = "LblTextJalanAtas" Then
            Dim formName As String = If(lbl.FindForm()?.Name, "")
            Dim warna = GetWarnaHeader(GetKategoriForm(formName))
            lbl.BackColor = Color.Transparent
            lbl.ForeColor = warna.Fore
            lbl.Font = New Font(lbl.Font.FontFamily, lbl.Font.Size, FontStyle.Bold)
            Return
        End If

        ' ── LblHeaderPanel - header di panel (standalone)
        If lbl.Name = "LblHeaderPanel" Then
            Dim formName As String = If(lbl.FindForm()?.Name, "")
            Dim warna = GetWarnaHeader(GetKategoriForm(formName))
            lbl.BackColor = warna.Back
            lbl.ForeColor = warna.Fore
            lbl.Font = New Font(lbl.Font.FontFamily, lbl.Font.Size, FontStyle.Bold)
            Return
        End If

        ' ── LblHeader* (standalone, bukan di PanelHeader) ───────────
        ' Warna kategori form + putih + bold — sama dengan PanelHeader
        ' Contoh: LblHeader, LblHeaderStok, LblHeaderDetail
        ' CATATAN: blok ini HARUS setelah LblHeaderForm di atas agar tidak tertangkap duluan
        If lbl.Name.StartsWith("LblHeader") Then
            Dim formName As String = If(lbl.FindForm()?.Name, "")
            Dim warna = GetWarnaHeader(GetKategoriForm(formName))
            lbl.BackColor = warna.Back
            lbl.ForeColor = warna.Fore
            lbl.Font = New Font(lbl.Font.FontFamily, lbl.Font.Size, FontStyle.Bold)
            Return
        End If

        ' ── Label biasa ─────────────────────────────────────────────
        lbl.ForeColor = C(L_PanelFore, D_PanelFore)
        lbl.BackColor = Color.Transparent
    End Sub

    Private Sub TerapkanContextMenu(cms As ContextMenuStrip)
        cms.BackColor = C(L_Surface, D_Surface)
        cms.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        For Each item As ToolStripItem In cms.Items
            item.BackColor = C(L_Surface, D_Surface)
            item.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        Next
    End Sub

    Private Sub TerapkanMenuStrip(ms As MenuStrip)
        ms.BackColor = C(L_Toolbar, D_Toolbar)
        ms.ForeColor = C(L_ToolbarFore, D_ToolbarFore)
        For Each item As ToolStripItem In ms.Items
            TerapkanMenuItem(item)
        Next
    End Sub

    Private Sub TerapkanMenuItem(item As ToolStripItem)
        item.BackColor = C(L_Toolbar, D_Toolbar)
        item.ForeColor = C(L_ToolbarFore, D_ToolbarFore)
        If TypeOf item Is ToolStripMenuItem Then
            For Each sub_item As ToolStripItem In CType(item, ToolStripMenuItem).DropDownItems
                TerapkanMenuItem(sub_item)
            Next
        End If
    End Sub

    Public Sub TerapkanHoverBorder(btn As Button)
        RemoveHandler btn.MouseEnter, AddressOf OnBtnMouseEnter
        RemoveHandler btn.MouseLeave, AddressOf OnBtnMouseLeave
        AddHandler btn.MouseEnter, AddressOf OnBtnMouseEnter
        AddHandler btn.MouseLeave, AddressOf OnBtnMouseLeave
    End Sub

    Private Sub OnBtnMouseEnter(sender As Object, e As EventArgs)
        Dim btn = CType(sender, Button)
        If btn.FlatStyle <> FlatStyle.Flat Then Return
        Dim bc As Color = btn.FlatAppearance.BorderColor
        btn.FlatAppearance.BorderColor = Color.FromArgb(bc.A, CInt(bc.R * 0.7), CInt(bc.G * 0.7), CInt(bc.B * 0.7))
        btn.FlatAppearance.BorderSize = 2
    End Sub

    Private Sub OnBtnMouseLeave(sender As Object, e As EventArgs)
        Dim btn = CType(sender, Button)
        If btn.FlatStyle <> FlatStyle.Flat Then Return
        btn.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        btn.FlatAppearance.BorderSize = 1
    End Sub

    ''' <summary>Terapkan tema ke DataGridView — warna header, baris, seleksi, dan double buffering.</summary>
    Public Sub ApplyThemeDataGridView(dgv As DataGridView)
        dgv.EnableHeadersVisualStyles = False
        dgv.BackgroundColor = C(L_Surface, D_Panel)
        dgv.GridColor = C(L_DgvGrid, D_DgvGrid)

        dgv.DefaultCellStyle.BackColor = C(L_Surface, D_Panel)
        dgv.DefaultCellStyle.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        dgv.DefaultCellStyle.SelectionBackColor = C(L_DgvSelect, D_DgvSelect)
        dgv.DefaultCellStyle.SelectionForeColor = Color.White

        dgv.RowsDefaultCellStyle.BackColor = C(L_Surface, D_Panel)
        dgv.RowsDefaultCellStyle.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        dgv.RowsDefaultCellStyle.SelectionBackColor = C(L_DgvSelect, D_DgvSelect)
        dgv.RowsDefaultCellStyle.SelectionForeColor = Color.White

        dgv.AlternatingRowsDefaultCellStyle.BackColor = C(L_DgvAlt, D_DgvAlt)
        dgv.AlternatingRowsDefaultCellStyle.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = C(L_DgvSelect, D_DgvSelect)
        dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White

        dgv.ColumnHeadersDefaultCellStyle.BackColor = C(L_DgvHeader, D_DgvHeader)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = C(L_DgvHeaderFore, D_DgvHeaderFore)
        ' Font header TIDAK ditimpa di sini — diatur per jenis DGV:
        '   Master    : ApplyStandardDataGridViewSettings
        '   Transaksi : ApplyTransaksiDataGridViewSettings (dengan fallback font)
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = C(L_DgvHeader, D_DgvHeader)
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = C(L_DgvHeaderFore, D_DgvHeaderFore)

        dgv.RowHeadersDefaultCellStyle.BackColor = C(L_DgvHeader, D_DgvHeader)
        dgv.RowHeadersDefaultCellStyle.ForeColor = C(L_DgvHeaderFore, D_DgvHeaderFore)
        dgv.RowHeadersDefaultCellStyle.SelectionBackColor = C(L_DgvHeader, D_DgvHeader)
        dgv.RowHeadersDefaultCellStyle.SelectionForeColor = C(L_DgvHeaderFore, D_DgvHeaderFore)

        For Each col As DataGridViewColumn In dgv.Columns
            ' Set font untuk tombol aksi - Calibri 9.75 Bold
            If TypeOf col Is DataGridViewButtonColumn Then
                col.DefaultCellStyle.Font = New Font("Calibri", 9.75!, FontStyle.Bold)
                Continue For
            End If

            If col.DefaultCellStyle.SelectionBackColor = Color.White OrElse
               col.DefaultCellStyle.SelectionBackColor = Color.Empty Then
                col.DefaultCellStyle.SelectionBackColor = C(L_DgvSelect, D_DgvSelect)
                col.DefaultCellStyle.SelectionForeColor = Color.White
            End If
            If col.DefaultCellStyle.BackColor = Color.White OrElse
               col.DefaultCellStyle.BackColor = Color.Empty Then
                col.DefaultCellStyle.BackColor = Color.Empty
            End If
        Next

        dgv.GetType().InvokeMember("DoubleBuffered",
            BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty,
            Nothing, dgv, New Object() {True})
    End Sub

    ''' <summary>Terapkan pengaturan standar DataGridView untuk konsistensi di semua form.</summary>
    Public Sub ApplyStandardDataGridViewSettings(dgv As DataGridView)
        ' -- Pengaturan standar yang konsisten untuk semua DGV --
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.AllowUserToResizeColumns = False
        dgv.AllowUserToResizeRows = False
        dgv.BorderStyle = BorderStyle.FixedSingle
        dgv.RowHeadersVisible = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.ReadOnly = True
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
    End Sub

    ''' <summary>
    ''' Terapkan pengaturan DataGridView khusus untuk form TRANSAKSI.
    ''' Berbeda dengan master karena:
    ''' - User bisa mengedit sel (qty, harga, diskon, satuan)
    ''' - User bisa menambah dan menghapus baris
    ''' - Tidak pakai FullRowSelect — user perlu klik sel tertentu
    ''' - Tidak AutoSizeColumns — lebar kolom diatur manual sesuai kebutuhan input
    ''' - RowHeaders tampil untuk nomor urut baris
    ''' - Perlu ClipboardCopyMode untuk copy-paste antar sel
    ''' - Perlu EditMode yang responsif untuk operasi berat
    ''' </summary>
    Public Sub ApplyTransaksiDataGridViewSettings(dgv As DataGridView)
        ' ── Izin edit dan manipulasi baris ────────────────────────────
        dgv.AllowUserToAddRows = True       ' User bisa tambah baris baru
        dgv.AllowUserToDeleteRows = False   ' Hapus baris via tombol, bukan keyboard Delete
        dgv.AllowUserToResizeColumns = True ' User bisa resize kolom sesuai kebutuhan
        dgv.AllowUserToResizeRows = False   ' Tinggi baris tetap konsisten

        ' ── Mode edit ─────────────────────────────────────────────────
        ' EditOnKeystrokeOrF2: langsung edit saat ketik, tidak perlu double-click
        ' Lebih responsif untuk input cepat di kasir
        dgv.EditMode = DataGridViewEditMode.EditOnKeystroke

        ' ── Seleksi ───────────────────────────────────────────────────
        ' CellSelect: user bisa klik sel tertentu (qty, harga, diskon)
        ' Bukan FullRowSelect karena setiap kolom punya fungsi berbeda
        dgv.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgv.MultiSelect = False             ' Satu sel aktif sekaligus

        ' ── Tampilan ──────────────────────────────────────────────────
        dgv.RowHeadersVisible = True        ' Tampilkan nomor urut baris
        dgv.BorderStyle = BorderStyle.FixedSingle
        dgv.ReadOnly = False                ' Grid bisa diedit

        ' ── Performa — penting untuk operasi berat ────────────────────
        ' VirtualMode = False: data langsung di grid, tidak perlu event CellValueNeeded
        ' AutoSizeColumnsMode = None: jangan auto-resize saat data berubah
        ' (auto-resize sangat lambat saat ada banyak baris dan kalkulasi berjalan)
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

        ' ── Clipboard ─────────────────────────────────────────────────
        ' EnableWithoutHeaderText: user bisa copy nilai sel tanpa header
        dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText

        ' ── Scroll ────────────────────────────────────────────────────
        dgv.ScrollBars = ScrollBars.Both

        ' ── Font — fallback ke Segoe UI jika Century Gothic tidak terinstall ──
        ' Century Gothic sudah diset di designer sebagai default.
        ' Blok ini hanya berjalan di komputer tanpa Microsoft Office
        ' (Century Gothic bukan bawaan Windows, hanya bawaan Office).
        ' Segoe UI dijamin ada — disiapkan di installer AppKasir via folder Fonts.
        If Not IsFontInstalled("Century Gothic") Then
            Dim fontSel As New Font("Segoe UI", 9.75!, FontStyle.Bold)
            dgv.ColumnHeadersDefaultCellStyle.Font = fontSel
            dgv.DefaultCellStyle.Font = fontSel
            dgv.RowsDefaultCellStyle.Font = fontSel
        End If
    End Sub

#End Region

#Region "TerapkanTheme — FormUtama Khusus"

    Private Sub TerapkanFormUtama(f As FormUtama)
        f.BackColor = C(L_FormBack, D_FormBack)

        f.MenuStrip1.BackColor = C(L_Toolbar, D_Toolbar)
        f.MenuStrip1.ForeColor = C(L_ToolbarFore, D_ToolbarFore)
        For Each item As ToolStripItem In f.MenuStrip1.Items
            TerapkanMenuItem(item)
        Next

        For Each pnl As Panel In {f.PanelMaster, f.PanelTransaksi, f.Panel1}
            pnl.BackColor = C(L_Toolbar, D_Toolbar)
        Next

        f.GBTransaksi.BackColor = C(L_Panel, D_Panel)
        f.GBTransaksi.ForeColor = C(L_PanelFore, D_PanelFore)

        Dim navBtns As Button() = {
            f.BtnToko, f.BtnBarang, f.BTnPelanggan, f.BtnSupliyer,
            f.BtnUser, f.BtnTabelRef, f.BtnHakAksesUser, f.BtnGeneralSetting,
            f.BtnKaryawan, f.BtnArmada, f.BtnMasterCabang,
            f.BtnBelanja, f.BtnPenjualan, f.BtnRetuBelanja, f.BtnReturPenjualan,
            f.BtnBayarHutang, f.BtnBayarPiutang, f.BtnStokOpname,
            f.BtnPindahStok, f.BtnTransferBarang, f.BtnSuratJalan, f.BtnKirimCabang
        }
        For Each btn As Button In navBtns
            SetNavButtonIdle(btn)
        Next

        f.BtnNotif.BackColor = C(L_NavIdle, D_NavIdle)
        f.BtnNotif.ForeColor = C(L_NavIdleFore, D_NavIdleFore)
        f.BtnNotif.FlatAppearance.MouseOverBackColor = C(L_NavHover, D_NavHover)

        f.BtnTambah.BackColor = C(L_BtnSolidTambah, D_BtnSolidTambah)
        f.BtnTambah.ForeColor = Color.White
        f.BtnTambah.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        f.BtnTambah.FlatAppearance.MouseOverBackColor = C(L_BtnSolidTambahHover, D_BtnSolidTambahHover)
        f.BtnTambah.FlatAppearance.MouseDownBackColor = C(L_BtnSolidTambahDown, D_BtnSolidTambahDown)

        f.BTNEdit.BackColor = C(L_BtnBack, D_BtnBack)
        f.BTNEdit.ForeColor = C(L_ForeEdit, D_ForeEdit)
        f.BTNEdit.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        f.BTNEdit.FlatAppearance.MouseOverBackColor = C(L_BtnHover, D_BtnHover)
        f.BTNEdit.FlatAppearance.MouseDownBackColor = C(L_BtnDown, D_BtnDown)

        f.BtnHapus.BackColor = C(L_BtnBack, D_BtnBack)
        f.BtnHapus.ForeColor = C(L_ForeHapus, D_ForeHapus)
        f.BtnHapus.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        f.BtnHapus.FlatAppearance.MouseOverBackColor = C(L_BtnHover, D_BtnHover)
        f.BtnHapus.FlatAppearance.MouseDownBackColor = C(L_BtnDown, D_BtnDown)

        f.BtnPrint.BackColor = C(L_BtnBack, D_BtnBack)
        f.BtnPrint.ForeColor = C(L_ForeCetak, D_ForeCetak)
        f.BtnPrint.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        f.BtnPrint.FlatAppearance.MouseOverBackColor = C(L_BtnHover, D_BtnHover)
        f.BtnPrint.FlatAppearance.MouseDownBackColor = C(L_BtnDown, D_BtnDown)

        f.BtnSettingPrinter.BackColor = C(L_BtnBack, D_BtnBack)
        f.BtnSettingPrinter.ForeColor = C(L_ForeCetak, D_ForeCetak)
        f.BtnSettingPrinter.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        f.BtnSettingPrinter.FlatAppearance.MouseOverBackColor = C(L_BtnHover, D_BtnHover)
        f.BtnSettingPrinter.FlatAppearance.MouseDownBackColor = C(L_BtnDown, D_BtnDown)

        f.BTNKeluar.BackColor = C(L_BtnSolidKeluar, D_BtnSolidKeluar)
        f.BTNKeluar.ForeColor = Color.White
        f.BTNKeluar.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        f.BTNKeluar.FlatAppearance.MouseOverBackColor = C(L_BtnSolidKeluarHover, D_BtnSolidKeluarHover)
        f.BTNKeluar.FlatAppearance.MouseDownBackColor = C(L_BtnSolidKeluarDown, D_BtnSolidKeluarDown)

        f.TxtFilter.BackColor = C(L_Surface, D_Surface)
        f.TxtFilter.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
        f.TxtTransaksi.BackColor = C(L_Surface, D_Surface)
        f.TxtTransaksi.ForeColor = C(L_TxtTransaksi, D_TxtTransaksi)

        f.StatusStrip1.BackColor = C(L_StatusBar, D_StatusBar)
        For Each item As ToolStripItem In f.StatusStrip1.Items
            item.ForeColor = C(L_StatusFore, D_StatusFore)
        Next
        f.StatusLevelUser.ForeColor = C(L_StatusAccent, D_StatusAccent)
        f.StatusLokasi.ForeColor = C(L_StatusAccent, D_StatusAccent)
    End Sub


    ''' <summary>
    ''' Set status transaksi LUNAS/Belum Lunas dengan warna semantik.
    ''' Hijau = lunas, Amber = belum lunas.
    ''' </summary>
    Public Sub SetWarnaStatusTransaksi(lbl As Label, isLunas As Boolean)
        If lbl Is Nothing Then Return
        lbl.ForeColor = If(isLunas,
            C(L_StatusLunas, D_StatusLunas),
            C(L_StatusBelumLunas, D_StatusBelumLunas))
    End Sub

    ''' <summary>
    ''' Warna merah untuk label peringatan stok kritis/lampau.
    ''' Hanya untuk sinyal bahaya nyata yang membutuhkan tindakan user.
    ''' </summary>
    Public Sub SetWarnaLabelWarning(ParamArray labels As Label())
        For Each lbl In labels
            If lbl IsNot Nothing Then lbl.ForeColor = C(L_LabelWarning, D_LabelWarning)
        Next
    End Sub

    ''' <summary>
    ''' Warna hangat untuk RichTextBox catatan/alasan (beda dari TextBox data biasa).
    ''' </summary>
    Public Sub SetWarnaRtbCatatan(ParamArray controls As RichTextBox())
        For Each rtb In controls
            If rtb IsNot Nothing Then
                rtb.BackColor = C(L_RtbCatatan, D_RtbCatatan)
                rtb.ForeColor = C(L_SurfaceFore, D_SurfaceFore)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Warna panel notifikasi hutang jatuh tempo (merah = bahaya).
    ''' </summary>
    Public Sub SetWarnaPanelNotifMerah(ParamArray panels As Panel())
        For Each pnl In panels
            If pnl IsNot Nothing Then pnl.BackColor = C(L_PanelNotifMerah, D_PanelNotifMerah)
        Next
    End Sub

    ''' <summary>
    ''' Warna panel notifikasi piutang jatuh tempo (biru = info).
    ''' </summary>
    Public Sub SetWarnaPanelNotifBiru(ParamArray panels As Panel())
        For Each pnl In panels
            If pnl IsNot Nothing Then pnl.BackColor = C(L_PanelNotifBiru, D_PanelNotifBiru)
        Next
    End Sub


    ''' <summary>
    ''' Set tombol nav ke state AKTIF (diklik/dipilih).
    ''' BackColor = biru, ForeColor = putih, MouseOver = biru lebih gelap.
    ''' Dipanggil dari SetButtonBackgroundColor di FormUtama.
    ''' </summary>
    Public Sub SetNavButtonActive(btn As Button)
        If btn Is Nothing Then Return
        btn.Cursor = Cursors.Hand
        btn.BackColor = C(L_NavActive, D_NavActive)
        btn.ForeColor = C(L_NavActiveFore, D_NavActiveFore)
        btn.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        btn.FlatAppearance.MouseOverBackColor = C(L_NavActiveHover, D_NavActiveHover)
        btn.FlatAppearance.MouseDownBackColor = C(L_NavDown, D_NavDown)
    End Sub

    ''' <summary>
    ''' Set tombol nav ke state IDLE (tidak aktif).
    ''' BackColor = putih/slate, ForeColor = hitam/putih, MouseOver = abu-abu.
    ''' Dipanggil dari SetButtonBackgroundColor di FormUtama.
    ''' </summary>
    Public Sub SetNavButtonIdle(btn As Button)
        If btn Is Nothing Then Return
        btn.Cursor = Cursors.Hand
        btn.BackColor = C(L_NavIdle, D_NavIdle)
        btn.ForeColor = C(L_NavIdleFore, D_NavIdleFore)
        btn.FlatAppearance.BorderColor = C(L_BtnBorder, D_BtnBorder)
        btn.FlatAppearance.MouseOverBackColor = C(L_NavHover, D_NavHover)
        btn.FlatAppearance.MouseDownBackColor = C(L_NavDown, D_NavDown)
    End Sub

    ''' <summary>
    ''' Set warna tombol Edit di dalam sel DataGridView.
    ''' Aktif = biru, Disabled = abu.
    ''' </summary>
    Public Sub SetWarnaDgvBtnEdit(cell As DataGridViewCell, enabled As Boolean)
        If cell Is Nothing Then Return
        If enabled Then
            cell.Style.BackColor = C(L_DgvBtnEdit, D_DgvBtnEdit)
            cell.Style.ForeColor = DgvBtnFore
        Else
            cell.Style.BackColor = C(L_DgvBtnDisabled, D_DgvBtnDisabled)
            cell.Style.ForeColor = C(L_DgvBtnDisabledFore, D_DgvBtnDisabledFore)
        End If
    End Sub

    ''' <summary>
    ''' Set warna tombol Hapus di dalam sel DataGridView.
    ''' Aktif = merah, Disabled = abu.
    ''' </summary>
    Public Sub SetWarnaDgvBtnHapus(cell As DataGridViewCell, enabled As Boolean)
        If cell Is Nothing Then Return
        If enabled Then
            cell.Style.BackColor = C(L_DgvBtnHapus, D_DgvBtnHapus)
            cell.Style.ForeColor = DgvBtnFore
        Else
            cell.Style.BackColor = C(L_DgvBtnDisabled, D_DgvBtnDisabled)
            cell.Style.ForeColor = C(L_DgvBtnDisabledFore, D_DgvBtnDisabledFore)
        End If
    End Sub

    ''' <summary>
    ''' Set warna tombol Status (Nonaktifkan/Aktifkan) di dalam sel DataGridView.
    ''' isAktif=True → tampilkan tombol "Nonaktifkan" (oranye).
    ''' isAktif=False → tampilkan tombol "Aktifkan" (hijau).
    ''' </summary>
    Public Sub SetWarnaDgvBtnStatus(cell As DataGridViewCell, isAktif As Boolean)
        If cell Is Nothing Then Return
        If isAktif Then
            cell.Style.BackColor = C(L_DgvBtnNonaktif, D_DgvBtnNonaktif)
        Else
            cell.Style.BackColor = C(L_DgvBtnAktif, D_DgvBtnAktif)
        End If
        cell.Style.ForeColor = DgvBtnFore
    End Sub

    ''' <summary>
    ''' Terapkan warna sel stok (STOK_TOKO / STOK_GUDANG) sesuai tema.
    ''' Dipanggil dari CellFormatting — hanya untuk baris yang aktif.
    ''' Baris non-aktif tidak memanggil ini (warna abu sudah di-set di level baris).
    ''' </summary>
    Public Sub SetWarnaSelStok(e As DataGridViewCellFormattingEventArgs)
        Dim value As Long = 0
        Try
            If e.Value IsNot Nothing AndAlso Not Convert.IsDBNull(e.Value) Then
                value = Convert.ToInt64(e.Value)
            End If
        Catch
            Return  ' Jika konversi gagal, skip pewarnaan
        End Try

        If value <= 1 Then
            e.CellStyle.BackColor = C(Color.FromArgb(254, 202, 202), Color.FromArgb(127, 29, 29))   ' Red-200 / Red-900
            e.CellStyle.ForeColor = C(Color.FromArgb(127, 29, 29), Color.FromArgb(254, 202, 202))   ' Red-900 / Red-200
        ElseIf value <= 10 Then
            e.CellStyle.BackColor = C(Color.FromArgb(254, 215, 170), Color.FromArgb(124, 45, 18))   ' Orange-200 / Orange-900
            e.CellStyle.ForeColor = C(Color.FromArgb(124, 45, 18), Color.FromArgb(254, 215, 170))   ' Orange-900 / Orange-200
        Else
            e.CellStyle.BackColor = C(Color.FromArgb(187, 247, 208), Color.FromArgb(20, 83, 45))    ' Green-200 / Green-900
            e.CellStyle.ForeColor = C(Color.FromArgb(20, 83, 45), Color.FromArgb(187, 247, 208))    ' Green-900 / Green-200
        End If
    End Sub
    ''' <summary>
    ''' Set warna teks baris DataGridView yang statusnya nonaktif/disabled.
    ''' Baris nonaktif ditampilkan abu-abu muted + italic.
    ''' </summary>
    Public Sub SetWarnaBarisDgvNonaktif(row As DataGridViewRow, isNonaktif As Boolean)
        If row Is Nothing Then Return
        If isNonaktif Then
            Dim targetBack As Color = C(L_DgvRowNonaktifBack, D_DgvRowNonaktifBack)
            If row.DefaultCellStyle.BackColor = targetBack Then Return  ' sudah di-set, skip
            row.DefaultCellStyle.ForeColor = C(L_DgvRowNonaktif, D_DgvRowNonaktif)
            row.DefaultCellStyle.BackColor = targetBack
        Else
            If row.DefaultCellStyle.BackColor = Color.Empty Then Return  ' sudah default, skip
            row.DefaultCellStyle.ForeColor = Color.Empty
            row.DefaultCellStyle.BackColor = Color.Empty
        End If
    End Sub



#End Region

    ' ── Font DGV — Segoe UI jika tersedia, fallback ke Calibri ──────────────
    ''' <summary>
    ''' Kembalikan font terbaik untuk DataGridView.
    ''' Prioritas: Segoe UI → Calibri → Tahoma (semua built-in di Windows 7+).
    ''' Segoe UI adalah font sistem Windows modern dengan keterbacaan terbaik.
    ''' </summary>
    Public Function GetDgvFont(Optional size As Single = 9.75!, Optional style As FontStyle = FontStyle.Regular) As Font
        Dim preferred As String() = {"Segoe UI", "Calibri", "Tahoma", "Arial"}
        Dim installedFamilies As New System.Drawing.Text.InstalledFontCollection()
        Dim availableNames As New HashSet(Of String)(
            installedFamilies.Families.Select(Function(f) f.Name),
            StringComparer.OrdinalIgnoreCase)

        For Each name As String In preferred
            If availableNames.Contains(name) Then
                Return New Font(name, size, style)
            End If
        Next

        ' Ultimate fallback — selalu ada
        Return New Font(System.Drawing.SystemFonts.DefaultFont.FontFamily, size, style)
    End Function

End Module
