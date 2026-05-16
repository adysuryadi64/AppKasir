Imports System.IO

' ================================================================
' ModuleKonfigurasi - Pusat pengaturan printer
'
' Format key pengaturan_cetak.ini:
'   [Transaksi]_JenisPrinter
'   [Transaksi]_Thermal_[Field]   -> Printer Thermal
'   [Transaksi]_DotMatrix_[Field] -> Printer Dot Matrix
'   [Transaksi]_Inkjet_[Field]    -> Printer Inkjet / Laser
'
' Transaksi: Jual, Beli, ReturJual, ReturBeli,
'            SuratJalan, BayarHutang, BayarPiutang,
'            Gaji, Bon, Laporan
'
' Contoh key lengkap:
'   Jual_JenisPrinter
'   Jual_Thermal_NamaPrinter
'   Jual_Thermal_FontJudul
'   Jual_DotMatrix_LebarKertas
'   Laporan_Inkjet_UkuranKertas
' ================================================================
Module ModuleKonfigurasi

#Region "PENGATURAN UMUM"

    ' ── Umum ────────────────────────────────────────────────────
    Public CetakPrinterDefault As String = ""
    Public AppStatusKomputer As String = "Server"

    ' ── Default cetak per transaksi ─────────────────────────────
    '
    ' Nilai yang valid (profil cetak lengkap, bukan sekadar jenis hardware):
    '   "Thermal_ESC"    → Printer Thermal, mode ESC/POS (Raw)
    '   "Thermal_GDI"    → Printer Thermal, mode GDI+ (Windows Print)
    '   "DotMatrix_GDI"  → Printer Dot Matrix, mode GDI+
    '   "DotMatrix_ESC"  → Printer Dot Matrix, mode ESC/P (Raw)
    '   "Inkjet"         → Printer Inkjet / Laser
    '   "Monitor"        → Tampilkan di Monitor (RDLC)
    '   "PDF"            → Export ke PDF
    '
    ' Dipakai saat user pilih "Langsung Cetak" — tanpa dialog pilih printer.
    ' Disimpan di pengaturan_cetak.ini sebagai key [Trx]_DefaultCetak.
    ' Diisi otomatis dari kombinasi JenisPrinter + ModeCetak saat form pengaturan disimpan.
    '
    Public CetakDefault_Jual As String = "Thermal_ESC"
    Public CetakDefault_Beli As String = "Thermal_ESC"
    Public CetakDefault_ReturJual As String = "Thermal_ESC"
    Public CetakDefault_ReturBeli As String = "Thermal_ESC"
    Public CetakDefault_SuratJalan As String = "Thermal_ESC"
    Public CetakDefault_TransferBarang As String = "DotMatrix_GDI"
    Public CetakDefault_TransferCabang As String = "DotMatrix_GDI"
    Public CetakDefault_BayarHutang As String = "Thermal_ESC"
    Public CetakDefault_BayarPiutang As String = "Thermal_ESC"
    Public CetakDefault_Gaji As String = "Inkjet"
    Public CetakDefault_Bon As String = "Thermal_ESC"
    Public CetakDefault_Laporan As String = "Inkjet"

    ''' <summary>
    ''' Muat semua pengaturan dari pengaturan_cetak.ini ke variabel global.
    ''' Panggil SEKALI saat aplikasi start (FormLoading) atau setelah simpan pengaturan.
    ''' Setelah ini semua form bisa langsung pakai CetakDefault_Jual, AppStatusKomputer, dll.
    ''' </summary>
    Public Sub MuatSemuaPengaturan()
        PastikanPrinterLengkap()   ' buat file dengan semua default jika belum ada
        MigrasiNamaModelLama()   ' konversi nama model lama di .ini ke nama baru
        CetakPrinterDefault = BacaPengaturanPrinter("", "DefaultPrinter", "")
        AppStatusKomputer = BacaPengaturanPrinter("", "StatusKomputer", "Server")
        CetakDefault_Jual = BacaDefaultCetak("Jual", "Thermal_ESC")
        CetakDefault_Beli = BacaDefaultCetak("Beli", "Thermal_ESC")
        CetakDefault_ReturJual = BacaDefaultCetak("ReturJual", "Thermal_ESC")
        CetakDefault_ReturBeli = BacaDefaultCetak("ReturBeli", "Thermal_ESC")
        CetakDefault_SuratJalan = BacaDefaultCetak("SuratJalan", "Thermal_ESC")
        CetakDefault_TransferBarang = BacaDefaultCetak("TransferBarang", "DotMatrix_GDI")
        CetakDefault_TransferCabang = BacaDefaultCetak("TransferCabang", "DotMatrix_GDI")
        CetakDefault_BayarHutang = BacaDefaultCetak("BayarHutang", "Thermal_ESC")
        CetakDefault_BayarPiutang = BacaDefaultCetak("BayarPiutang", "Thermal_ESC")
        CetakDefault_Gaji = BacaDefaultCetak("GajiKaryawan", "Inkjet")
        CetakDefault_Bon = BacaDefaultCetak("BonKaryawan", "Thermal_ESC")
        CetakDefault_Laporan = BacaDefaultCetak("LaporanKas", "Inkjet")
    End Sub

    ''' <summary>
    ''' Baca DefaultCetak dari .ini. Jika belum ada (file lama), derive otomatis
    ''' dari kombinasi JenisPrinter + ModeCetak yang sudah tersimpan.
    ''' </summary>
    Private Function BacaDefaultCetak(transaksi As String, fallback As String) As String
        Dim saved As String = BacaPengaturanPrinter(transaksi, "DefaultCetak", "")
        If Not String.IsNullOrEmpty(saved) Then Return saved

        ' Derive dari pengaturan lama agar backward compatible
        Dim jenis As String = BacaPengaturanPrinter(transaksi, "JenisPrinter", "")
        Dim mode As String = BacaPengaturanPrinter(transaksi, "Thermal_ModeCetak", "ESC/POS (Raw)")
        Select Case jenis
            Case "Printer Thermal"
                Return If(mode = "GDI+ (Windows Print)", "Thermal_GDI", "Thermal_ESC")
            Case "Printer Dot Matrix"
                Dim modeDot As String = BacaPengaturanPrinter(transaksi, "DotMatrix_ModeCetak", "GDI+ (Windows Print)")
                Return If(modeDot = "ESC/P (Raw)", "DotMatrix_ESC", "DotMatrix_GDI")
            Case "Printer Inkjet / Laser" : Return "Inkjet"
            Case "Tampilkan di Monitor" : Return "Monitor"
            Case "Export ke PDF" : Return "PDF"
            Case Else : Return fallback
        End Select
    End Function

    ' Alias lama — tetap berfungsi
    Public Sub MuatPengaturanUmum()
        MuatSemuaPengaturan()
    End Sub

    ''' <summary>
    ''' Ambil CetakDefault_* berdasarkan nama transaksi string.
    ''' Contoh: AmbilCetakDefault("Jual") → CetakDefault_Jual
    ''' </summary>
    Public Function AmbilCetakDefault(transaksi As String) As String
        Select Case transaksi
            Case "Jual" : Return CetakDefault_Jual
            Case "Beli" : Return CetakDefault_Beli
            Case "ReturJual" : Return CetakDefault_ReturJual
            Case "ReturBeli" : Return CetakDefault_ReturBeli
            Case "SuratJalan" : Return CetakDefault_SuratJalan
            Case "TransferBarang" : Return CetakDefault_TransferBarang
            Case "TransferCabang" : Return CetakDefault_TransferCabang
            Case "BayarHutang" : Return CetakDefault_BayarHutang
            Case "BayarPiutang" : Return CetakDefault_BayarPiutang
            Case "GajiKaryawan" : Return CetakDefault_Gaji
            Case "BonKaryawan" : Return CetakDefault_Bon
            Case "LaporanKas" : Return CetakDefault_Laporan
            Case Else : Return BacaDefaultCetak(transaksi, "Thermal_ESC")
        End Select
    End Function

    ' Alias lama AmbilCetakJenis — kembalikan format lama agar kode yang belum dimigrasi tidak rusak
    Public Function AmbilCetakJenis(transaksi As String) As String
        Select Case AmbilCetakDefault(transaksi)
            Case "Thermal_ESC", "Thermal_GDI" : Return "Printer Thermal"
            Case "DotMatrix_GDI", "DotMatrix_ESC" : Return "Printer Dot Matrix"
            Case "Inkjet" : Return "Printer Inkjet / Laser"
            Case "Monitor" : Return "Tampilkan di Monitor"
            Case "PDF" : Return "Export ke PDF"
            Case Else : Return "Printer Thermal"
        End Select
    End Function

    ' Alias lama AmbilJenisPrinter — sama seperti AmbilCetakJenis
    Public Function AmbilJenisPrinter(transaksi As String) As String
        Return AmbilCetakJenis(transaksi)
    End Function

    ''' <summary>
    ''' Migrasi nama model struk lama di pengaturan_cetak.ini ke nama baru.
    ''' Dipanggil otomatis saat MuatSemuaPengaturan().
    ''' Nama lama mengandung kata "Logo" — sekarang logo dikontrol via checkbox terpisah.
    ''' </summary>
    Private Sub MigrasiNamaModelLama()
        If Not File.Exists(FILE_PRINTER) Then Exit Sub

        ' Peta: nama lama → nama baru per transaksi
        ' Format: {transaksi, namaLama, namaBaru}
        Dim peta As (trx As String, lama As String, baru As String)() = {
            ("Jual", "Model 1 Lengkap",                         "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 2 Tanpa Diskon",                    "Model 4 — Judul Kolom"),
            ("Jual", "Model 3 Tanpa Header",                    "Model 8 — Ringkas"),
            ("Jual", "Model 6 Dengan Sales",                    "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 7 Dengan Persen",                   "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 8 Dengan Total Hutang",             "Model 1 — Judul Kolom, Diskon, Sisa Hutang"),
            ("Jual", "Model 4 Lengkap Tanpa Logo",              "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 5 Tanpa Logo Tanpa Diskon",         "Model 4 — Judul Kolom"),
            ("Jual", "Model 9 Tanpa Logo Lengkap",              "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 10 Tanpa Logo Tanpa Diskon",        "Model 4 — Judul Kolom"),
            ("Jual", "Model 11 Tanpa Logo Tanpa Header",        "Model 8 — Ringkas"),
            ("Jual", "Model 12 Tanpa Logo Info Singkat",        "Model 4 — Judul Kolom"),
            ("Jual", "Model 13 Tanpa Logo Dengan Sales",        "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 14 Tanpa Logo Dengan Persen",       "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 15 Tanpa Logo Dengan Total Hutang", "Model 1 — Judul Kolom, Diskon, Sisa Hutang"),
            ("Jual", "Model 1 Header Diskon Hutang",            "Model 1 — Judul Kolom, Diskon, Sisa Hutang"),
            ("Jual", "Model 2 Header Diskon",                   "Model 2 — Judul Kolom, Diskon"),
            ("Jual", "Model 3 Header Hutang",                   "Model 3 — Judul Kolom, Sisa Hutang"),
            ("Jual", "Model 4 Header",                          "Model 4 — Judul Kolom"),
            ("Jual", "Model 5 Diskon Hutang",                   "Model 5 — Diskon, Sisa Hutang"),
            ("Jual", "Model 6 Diskon",                          "Model 6 — Diskon"),
            ("Jual", "Model 7 Hutang",                          "Model 7 — Sisa Hutang"),
            ("Jual", "Model 8 Minimal",                         "Model 8 — Ringkas"),
            ("Beli", "Model 3 Tanpa Logo",                      "Model 1 Lengkap"),
            ("Beli", "Model 4 Dengan Total Hutang",             "Model 3 Dengan Total Hutang"),
            ("ReturJual", "Model 4 Lengkap Tanpa Logo",         "Model 1 Lengkap"),
            ("ReturBeli", "Model 3 Tanpa Logo",                 "Model 1 Lengkap"),
            ("SuratJalan", "Model 4 Lengkap Tanpa Logo",        "Model 1 Lengkap"),
            ("BayarHutang", "Model 4 Lengkap Tanpa Logo",       "Model 1 Lengkap"),
            ("BayarPiutang", "Model 4 Lengkap Tanpa Logo",      "Model 1 Lengkap"),
            ("BonKaryawan", "Model 4 Lengkap Tanpa Logo",          "Model 1 Lengkap"),
            ("GajiKaryawan", "Model 4 Lengkap Tanpa Logo",         "Model 1 Lengkap")
        }

        Dim baris As String() = File.ReadAllLines(FILE_PRINTER)
        Dim diubah As Boolean = False

        For i As Integer = 0 To baris.Length - 1
            Dim line As String = baris(i)
            If line.StartsWith(";") OrElse Not line.Contains("=") Then Continue For
            Dim bagian = line.Split({"="c}, 2)
            If bagian.Length <> 2 Then Continue For
            Dim kunci As String = bagian(0).Trim()
            Dim nilai As String = bagian(1).Trim()

            ' Cek apakah key ini adalah Thermal_ModelStruk untuk salah satu transaksi
            For Each p In peta
                Dim keyTarget As String = p.trx & "_Thermal_ModelStruk"
                If kunci = keyTarget AndAlso nilai = p.lama Then
                    baris(i) = kunci & "=" & p.baru
                    diubah = True
                    Exit For
                End If
            Next
        Next

        ' Migrasi nama model inkjet lama ("Lengkap", "Tanpa Diskon", "Dengan Total Hutang")
        ' ke format baru dengan prefix "Model N"
        Dim petaInkjet As (lama As String, baru As String)() = {
            ("Lengkap", "Model 1 Lengkap"),
            ("Tanpa Diskon", "Model 2 Tanpa Diskon"),
            ("Dengan Total Hutang", "Model 2 Dengan Total Hutang")
        }
        For i As Integer = 0 To baris.Length - 1
            Dim line As String = baris(i)
            If line.StartsWith(";") OrElse Not line.Contains("=") Then Continue For
            Dim bagian = line.Split({"="c}, 2)
            If bagian.Length <> 2 Then Continue For
            Dim kunci As String = bagian(0).Trim()
            Dim nilai As String = bagian(1).Trim()
            If kunci.EndsWith("_Inkjet_ModelNota") Then
                For Each p In petaInkjet
                    If nilai = p.lama Then
                        baris(i) = kunci & "=" & p.baru
                        diubah = True
                        Exit For
                    End If
                Next
            End If
        Next

        If diubah Then
            File.WriteAllLines(FILE_PRINTER, baris, System.Text.Encoding.UTF8)
        End If
    End Sub

#End Region

#Region "API UTAMA - Baca pengaturan_cetak.ini"

    Private Const FILE_PRINTER As String = "pengaturan_cetak.ini"
    Private Const FILE_PERILAKU As String = "perilaku_cetak.ini"

    ' Field yang disimpan di perilaku_cetak.ini (terpisah dari pengaturan printer)
    Private ReadOnly FieldPerilaku As String() = {"CetakOtomatis", "PilihPrinter"}

    ' Baca satu nilai dari pengaturan_cetak.ini atau perilaku_cetak.ini
    ' Field CetakOtomatis/PilihPrinter → perilaku_cetak.ini
    ' Field lainnya → pengaturan_cetak.ini
    Public Function BacaPengaturanPrinter(transaksi As String, field As String,
                                           Optional nilaiDefault As String = "") As String
        Dim path As String = If(FieldPerilaku.Contains(field), FILE_PERILAKU, FILE_PRINTER)
        If Not File.Exists(path) Then Return nilaiDefault
        Dim kunci As String = If(String.IsNullOrEmpty(transaksi), field, transaksi & "_" & field)
        For Each baris As String In File.ReadAllLines(path)
            If baris.StartsWith(";") OrElse Not baris.Contains("=") Then Continue For
            Dim bagian = baris.Split({"="c}, 2)
            If bagian.Length = 2 AndAlso bagian(0).Trim() = kunci Then
                Return bagian(1).Trim()
            End If
        Next
        Return nilaiDefault
    End Function

    ' Tulis satu nilai — CetakOtomatis/PilihPrinter ke perilaku_cetak.ini, lainnya ke pengaturan_cetak.ini
    Public Sub TulisPengaturanPrinter(transaksi As String, field As String, nilai As String)
        Dim path As String = If(FieldPerilaku.Contains(field), FILE_PERILAKU, FILE_PRINTER)
        Dim kunci As String = If(String.IsNullOrEmpty(transaksi), field, transaksi & "_" & field)
        TulisKeFile(path, kunci, nilai)
    End Sub

    ' Upsert satu key=nilai ke file INI
    Private Sub TulisKeFile(path As String, kunci As String, nilai As String)
        Dim baris As New List(Of String)
        Dim ditemukan As Boolean = False

        If File.Exists(path) Then
            For Each line As String In File.ReadAllLines(path)
                If Not line.StartsWith(";") AndAlso line.Contains("=") Then
                    Dim bagian = line.Split({"="c}, 2)
                    If bagian.Length = 2 AndAlso bagian(0).Trim() = kunci Then
                        baris.Add(kunci & "=" & nilai)
                        ditemukan = True
                        Continue For
                    End If
                End If
                baris.Add(line)
            Next
        Else
            ' File baru — tulis header
            baris.Add("; perilaku_cetak.ini — Pengaturan cetak otomatis per transaksi")
            baris.Add("; CetakOtomatis : IYA | SELALU TANYA | TAMPILKAN DI MONITOR")
            baris.Add("; PilihPrinter  : LANGSUNG CETAK | TANYA PILIH PRINTER")
            baris.Add("")
        End If

        If Not ditemukan Then baris.Add(kunci & "=" & nilai)
        File.WriteAllLines(path, baris, System.Text.Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' Pastikan pengaturan_cetak.ini sudah ada dengan semua key default.
    ''' Jika file belum ada, buat lengkap dengan nilai default untuk semua transaksi dan semua tab
    ''' (Thermal, Dot Matrix, Inkjet, Monitor, PDF).
    ''' Key yang sudah ada tidak ditimpa — hanya key yang belum ada yang ditambahkan.
    ''' Dipanggil otomatis dari MuatSemuaPengaturan() saat aplikasi start.
    ''' </summary>
    Public Sub PastikanPrinterLengkap()
        Dim transaksiList As String() = {
            "Jual", "Beli", "ReturJual", "ReturBeli",
            "SuratJalan", "TransferBarang", "TransferCabang",
            "BayarHutang", "BayarPiutang",
            "GajiKaryawan", "BonKaryawan", "LaporanKas"
        }

        ' Baca isi file sekarang ke dictionary (satu kali baca)
        Dim existing As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If File.Exists(FILE_PRINTER) Then
            For Each line As String In File.ReadAllLines(FILE_PRINTER)
                If line.StartsWith(";") OrElse Not line.Contains("=") Then Continue For
                Dim bagian = line.Split({"="c}, 2)
                If bagian.Length = 2 Then existing(bagian(0).Trim()) = bagian(1).Trim()
            Next
        End If

        ' Ambil default printer Windows sekali — dipakai untuk semua NamaPrinter
        Dim winDefaultPrinter As String = ""
        Try
            winDefaultPrinter = New System.Drawing.Printing.PrinterSettings().PrinterName
        Catch
        End Try

        ' Kumpulkan semua key default yang belum ada di file
        Dim tambahan As New List(Of String)

        ' ── Pengaturan Umum ──────────────────────────────────────
        If Not existing.ContainsKey("DefaultPrinter") Then tambahan.Add("DefaultPrinter=" & winDefaultPrinter)
        If Not existing.ContainsKey("StatusKomputer") Then tambahan.Add("StatusKomputer=Server")

        For Each trx As String In transaksiList
            ' ── JenisPrinter & DefaultCetak ─────────────────────
            If Not existing.ContainsKey(trx & "_JenisPrinter") Then
                tambahan.Add(trx & "_JenisPrinter=Printer Thermal")
            End If
            If Not existing.ContainsKey(trx & "_DefaultCetak") Then
                tambahan.Add(trx & "_DefaultCetak=Thermal_ESC")
            End If

            ' ── Thermal ─────────────────────────────────────────
            Dim defThermal As New Dictionary(Of String, String) From {
                {"Thermal_ModeCetak",              "ESC/POS (Raw)"},
                {"Thermal_TipeKoneksi",             "USB / Windows Spooler"},
                {"Thermal_NamaPrinter",             winDefaultPrinter},
                {"Thermal_IpAddress",               "192.168.1.50"},
                {"Thermal_NetworkPort",             "9100"},
                {"Thermal_UkuranKertas",            "POS-80 (80mm)"},
                {"Thermal_LebarKertas",             "80"},
                {"Thermal_BatasKiri",               "0"},
                {"Thermal_JarakBaris",              "4"},
                {"Thermal_JarakBarisEsc",           "0"},
                {"Thermal_PortLaciKasir",           ""},
                {"Thermal_KodeLaciKasir",           "(Tidak Ada)"},
                {"Thermal_PotongOtomatisEsc",       "True"},
                {"Thermal_PotongOtomatisGdi",       "True"},
                {"Thermal_JumlahCetakEsc",          "1"},
                {"Thermal_JumlahCetakGdi",          "1"},
                {"Thermal_ModelStruk",              "Model 2 — Judul Kolom, Diskon"},
                {"Thermal_FontJudul",               "Arial"},
                {"Thermal_UkuranJudul",             "12"},
                {"Thermal_FontKeterangan",          "Arial"},
                {"Thermal_UkuranKeterangan",        "9"},
                {"Thermal_FontIsi",                 "Courier New"},
                {"Thermal_UkuranIsi",               "9"},
                {"Thermal_FontFooter",              "Arial"},
                {"Thermal_UkuranFooter",            "9"},
                {"Thermal_EscUkuranJudul",          "Besar (2x)"},
                {"Thermal_EscUkuranKeterangan",     "Normal"},
                {"Thermal_EscUkuranIsi",            "Normal"},
                {"Thermal_EscUkuranFooter",         "Normal"},
                {"Thermal_TampilFooter1",           "True"},
                {"Thermal_TampilFooter2",           "True"},
                {"Thermal_TampilFooter3",           "True"},
                {"Thermal_DpiCetak",                "100"},
                {"Thermal_TampilLogo",              "True"}
            }
            For Each kv In defThermal
                Dim k As String = trx & "_" & kv.Key
                If Not existing.ContainsKey(k) Then tambahan.Add(k & "=" & kv.Value)
            Next

            ' ── Dot Matrix ──────────────────────────────────────
            Dim defDot As New Dictionary(Of String, String) From {
                {"DotMatrix_NamaPrinter",           winDefaultPrinter},
                {"DotMatrix_ModeCetak",             "GDI+ (Windows Print)"},
                {"DotGdi_LebarKertas",              "80"},
                {"DotGdi_UkuranKertas",             "Continuous Form (Auto)"},
                {"DotGdi_BatasKiri",                "0"},
                {"DotGdi_JarakBaris",               "2"},
                {"DotGdi_UkuranFont",               "9"},
                {"DotGdi_JumlahCetak",              "1"},
                {"DotGdi_ModelStruk",               "Model 1 Lengkap"},
                {"DotGdi_TampilFooter1",            "True"},
                {"DotGdi_TampilFooter2",            "True"},
                {"DotGdi_TampilFooter3",            "True"},
                {"DotEsc_LebarKertas",              "80"},
                {"DotEsc_BatasKiri",                "0"},
                {"DotEsc_JarakBaris",               "0"},
                {"DotEsc_JumlahCetak",              "1"},
                {"DotEsc_ModelStruk",               "Model 1 Lengkap"},
                {"DotEsc_EscUkuranJudul",           "Besar (2x)"},
                {"DotEsc_EscUkuranKeterangan",      "Normal"},
                {"DotEsc_EscUkuranIsi",             "Normal"},
                {"DotEsc_EscUkuranFooter",          "Normal"},
                {"DotEsc_TampilFooter1",            "True"},
                {"DotEsc_TampilFooter2",            "True"},
                {"DotEsc_TampilFooter3",            "True"}
            }
            For Each kv In defDot
                Dim k As String = trx & "_" & kv.Key
                If Not existing.ContainsKey(k) Then tambahan.Add(k & "=" & kv.Value)
            Next

            ' ── Inkjet / Laser ───────────────────────────────────
            Dim defInk As New Dictionary(Of String, String) From {
                {"Inkjet_NamaPrinter",              winDefaultPrinter},
                {"Inkjet_UkuranKertas",             "A4"},
                {"Inkjet_Orientasi",                "Portrait"},
                {"Inkjet_JumlahCetak",              "1"},
                {"Inkjet_MarginAtas",               "10"},
                {"Inkjet_MarginBawah",              "10"},
                {"Inkjet_MarginKiri",               "15"},
                {"Inkjet_MarginKanan",              "10"},
                {"Inkjet_FontJudul",                "Arial"},
                {"Inkjet_UkuranJudul",              "12"},
                {"Inkjet_FontIsi",                  "Arial"},
                {"Inkjet_UkuranIsi",                "10"},
                {"Inkjet_TampilFooter1",            "True"},
                {"Inkjet_TampilFooter2",            "True"},
                {"Inkjet_TampilFooter3",            "True"},
                {"Inkjet_ModelNota",                "Model 1 Lengkap"},
                {"Inkjet_TampilLogo",               "True"},
                {"Inkjet_TampilTandaTangan",        "True"},
                {"Inkjet_PctKolomNo",               "4"},
                {"Inkjet_PctKolomQty",              "8"},
                {"Inkjet_PctKolomHarga",            "12"},
                {"Inkjet_PctKolomDiskon",           "10"}
            }
            For Each kv In defInk
                Dim k As String = trx & "_" & kv.Key
                If Not existing.ContainsKey(k) Then tambahan.Add(k & "=" & kv.Value)
            Next

            ' ── Monitor ─────────────────────────────────────────
            For Each f In {"Monitor_TampilFooter1", "Monitor_TampilFooter2", "Monitor_TampilFooter3"}
                Dim k As String = trx & "_" & f
                If Not existing.ContainsKey(k) Then tambahan.Add(k & "=True")
            Next

            ' ── PDF ─────────────────────────────────────────────
            For Each f In {"PDF_TampilFooter1", "PDF_TampilFooter2", "PDF_TampilFooter3"}
                Dim k As String = trx & "_" & f
                If Not existing.ContainsKey(k) Then tambahan.Add(k & "=True")
            Next
        Next

        ' Tidak ada yang kurang — lewati
        If tambahan.Count = 0 Then Exit Sub

        ' Tulis ke file — buat header jika file belum ada
        Dim baris As New List(Of String)
        If File.Exists(FILE_PRINTER) Then
            baris.AddRange(File.ReadAllLines(FILE_PRINTER))
        Else
            baris.Add("; pengaturan_cetak.ini — Konfigurasi printer per transaksi")
            baris.Add("; Dibuat otomatis oleh aplikasi. Edit via menu Pengaturan > Printer.")
            baris.Add("")
        End If
        baris.AddRange(tambahan)
        File.WriteAllLines(FILE_PRINTER, baris, System.Text.Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' Pastikan perilaku_cetak.ini sudah berisi semua key yang diperlukan.
    ''' Key yang belum ada ditambahkan dengan nilai default.
    ''' Key yang sudah ada dibiarkan — tidak ditimpa.
    ''' Panggil dari FormPengaturanPrinter_Load.
    ''' </summary>
    Public Sub PastikanPerilakuLengkap()
        ' Daftar transaksi dan default masing-masing
        Dim transaksiList As String() = {
            "Jual", "Beli", "ReturJual", "ReturBeli",
            "SuratJalan", "TransferBarang", "TransferCabang",
            "BayarHutang", "BayarPiutang",
            "GajiKaryawan", "BonKaryawan", "LaporanKas"
        }

        ' Baca isi file sekarang ke dictionary agar cepat (satu kali baca)
        Dim existing As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If File.Exists(FILE_PERILAKU) Then
            For Each line As String In File.ReadAllLines(FILE_PERILAKU)
                If line.StartsWith(";") OrElse Not line.Contains("=") Then Continue For
                Dim bagian = line.Split({"="c}, 2)
                If bagian.Length = 2 Then existing(bagian(0).Trim()) = bagian(1).Trim()
            Next
        End If

        ' Tentukan default: IYA + LANGSUNG CETAK jika file baru, SELALU TANYA jika file sudah ada
        Dim fileBaruDibuat As Boolean = Not File.Exists(FILE_PERILAKU)
        Dim defaultCetak As String = If(fileBaruDibuat, "IYA", "SELALU TANYA")
        Dim defaultPilih As String = If(fileBaruDibuat, "LANGSUNG CETAK", "TANYA PILIH PRINTER")

        ' Kumpulkan key yang belum ada
        Dim tambahan As New List(Of String)
        For Each trx As String In transaksiList
            Dim keyCetak As String = trx & "_CetakOtomatis"
            Dim keyPilih As String = trx & "_PilihPrinter"
            If Not existing.ContainsKey(keyCetak) Then
                tambahan.Add(keyCetak & "=" & defaultCetak)
            End If
            If Not existing.ContainsKey(keyPilih) Then
                tambahan.Add(keyPilih & "=" & defaultPilih)
            End If
        Next

        ' Tidak ada yang kurang — lewati
        If tambahan.Count = 0 Then Exit Sub

        ' Tulis ke file — buat header jika file belum ada
        Dim baris As New List(Of String)
        If File.Exists(FILE_PERILAKU) Then
            baris.AddRange(File.ReadAllLines(FILE_PERILAKU))
        Else
            baris.Add("; perilaku_cetak.ini — Pengaturan cetak otomatis per transaksi")
            baris.Add("; CetakOtomatis : IYA | SELALU TANYA | TAMPILKAN DI MONITOR")
            baris.Add("; PilihPrinter  : LANGSUNG CETAK | TANYA PILIH PRINTER")
            baris.Add("")
        End If
        baris.AddRange(tambahan)
        File.WriteAllLines(FILE_PERILAKU, baris, System.Text.Encoding.UTF8)
    End Sub

#End Region

#Region "SHORTCUT API"

    Public Function AmbilNamaPrinter(transaksi As String, jenis As String) As String
        Return BacaPengaturanPrinter(transaksi, jenis & "_NamaPrinter", "")
    End Function

    Public Function AmbilJumlahCetak(transaksi As String, jenis As String) As Integer
        Return Math.Max(1, KeBilangan(BacaPengaturanPrinter(transaksi, jenis & "_JumlahCetak", "1")))
    End Function

#End Region

#Region "BUKA LACI KASIR"

    ' Dipanggil setelah cetak nota.
    ' Mendukung semua mode: ESC/POS, GDI+, via Serial port.
    ' Contoh: BukaLaciKasir("Jual")
    Public Sub BukaLaciKasir(transaksi As String)
        Dim portSerial As String = BacaPengaturanPrinter(transaksi, "Thermal_PortLaciKasir", "")
        Dim kode As String = BacaPengaturanPrinter(transaksi, "Thermal_KodeLaciKasir", "(Tidak Ada)")
        Dim namaPrinter As String = BacaPengaturanPrinter(transaksi, "Thermal_NamaPrinter", "")
        Dim modeCetak As String = BacaPengaturanPrinter(transaksi, "Thermal_ModeCetak", "ESC/POS (Raw)")

        If kode = "(Tidak Ada)" Then Exit Sub

        ' Parse pin dan pulse dari deskripsi
        ' Format: "Pin 2 — Pulse 100ms (standar)", "Pin 5 — Pulse 200ms", dll
        Dim pinLaci As Integer = If(kode.Contains("Pin 5"), 5, 2)
        Dim pulseMs As Integer = If(kode.Contains("200ms"), 200, 100)
        Dim pulseByte As Byte = CByte(Math.Min(255, pulseMs \ 10))  ' t1/t2 dalam unit 10ms

        ' Byte perintah ESC p m t1 t2
        Dim pinByte As Byte = If(pinLaci = 5, CByte(1), CByte(0))
        Dim escP As Byte() = {&H1B, &H70, pinByte, pulseByte, pulseByte}

        If Not String.IsNullOrEmpty(namaPrinter) Then
            Try
                Dim isNetwork As Boolean = BacaPengaturanPrinter(transaksi, "Thermal_TipeKoneksi", "") = "Network / WiFi (IP)"
                Dim ipAddress As String = BacaPengaturanPrinter(transaksi, "Thermal_IpAddress", "")
                Dim networkPort As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_NetworkPort", "9100"))
                If networkPort = 0 Then networkPort = 9100

                If isNetwork AndAlso Not String.IsNullOrEmpty(ipAddress) Then
                    ' Network printer — selalu kirim raw via TCP, tidak peduli mode cetak
                    Dim mesinCetak As New PrinterEscPos(ipAddress, networkPort, 80)
                    mesinCetak.BukaLaci(pinLaci)
                ElseIf modeCetak = "GDI+ (Windows Print)" Then
                    ' GDI+ mode — kirim raw bytes via RawPrinterHelper (bypass GDI rendering)
                    RawPrinterHelper.KirimKePrinter(namaPrinter, escP)
                Else
                    ' ESC/POS mode — kirim via PrinterEscPos seperti biasa
                    Dim mesinCetak As New PrinterEscPos(namaPrinter, 80)
                    mesinCetak.BukaLaci(pinLaci)
                End If
            Catch
                ' Gagal buka laci tidak menghentikan proses cetak
            End Try
        ElseIf Not String.IsNullOrEmpty(portSerial) Then
            ' Fallback: buka laci via Serial port langsung
            Try
                Using port As New System.IO.Ports.SerialPort(portSerial, 9600,
                        System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One)
                    port.Open()
                    port.Write(escP, 0, escP.Length)
                End Using
            Catch
            End Try
        End If
    End Sub

#End Region

#Region "KONFIGURASI THERMAL"

    ' Cara pakai:
    '   Dim cfg As New KonfigurasiThermal("Jual")
    '   Dim esc As New PrinterEscPos(cfg.NamaPrinter, cfg.LebarKertas)
    '   If cfg.PotongOtomatis Then esc.PotongKertas()
    Public Class KonfigurasiThermal
        Public NamaPrinter As String
        Public JenisPrinter As String
        Public ModeCetak As String       ' "ESC/POS (Raw)" atau "GDI+ (Windows Print)"
        Public TipeKoneksi As String     ' "USB / Windows Spooler" atau "Network / WiFi (IP)"
        Public IpAddress As String       ' untuk Network printer
        Public NetworkPort As Integer    ' default 9100
        Public LebarKertas As Integer
        Public BatasKiri As Integer
        Public JarakBaris As Integer      ' jarak baris GDI+ dalam pixel (default 4)
        Public JarakBarisEsc As Integer   ' jarak baris ESC/POS dalam baris karakter (default 0)
        Public PotongOtomatis As Boolean
        Public JumlahCetak As Integer
        Public ModelStruk As String
        Public PortLaciKasir As String
        Public KodeLaciKasir As String
        Public PinLaciKasir As Integer   ' 2 = standar, 5 = alternatif (dari KodeLaciKasir OPTION 3/4)
        Public UkuranKertas As String
        ' Font
        Public FontJudul As String : Public UkuranJudul As Integer
        Public FontKeterangan As String : Public UkuranKeterangan As Integer
        Public FontIsi As String : Public UkuranIsi As Integer
        Public FontFooter As String : Public UkuranFooter As Integer
        Public EscUkuranJudul As String
        Public EscUkuranKeterangan As String
        Public EscUkuranIsi As String
        Public EscUkuranFooter As String
        Public TampilFooter1 As Boolean
        Public TampilFooter2 As Boolean
        Public TampilFooter3 As Boolean
        Public DpiCetak As Integer       ' DPI untuk kalkulasi lebar pixel GDI+ (default 96)
        Public TampilLogo As Boolean     ' tampilkan logo di header (dari checkbox pengaturan)

        Sub New(transaksi As String)
            JenisPrinter = BacaPengaturanPrinter(transaksi, "JenisPrinter", "Printer Thermal")
            ModeCetak = BacaPengaturanPrinter(transaksi, "Thermal_ModeCetak", "ESC/POS (Raw)")
            TipeKoneksi = BacaPengaturanPrinter(transaksi, "Thermal_TipeKoneksi", "USB / Windows Spooler")
            NamaPrinter = BacaPengaturanPrinter(transaksi, "Thermal_NamaPrinter", "")
            IpAddress = BacaPengaturanPrinter(transaksi, "Thermal_IpAddress", "192.168.1.50")
            NetworkPort = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_NetworkPort", "9100"))
            If NetworkPort = 0 Then NetworkPort = 9100
            UkuranKertas = BacaPengaturanPrinter(transaksi, "Thermal_UkuranKertas", "POS-80 (80mm)")
            ' Sinkronisasi LebarKertas dari UkuranKertas jika tidak diisi manual
            Dim lebarTersimpan As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_LebarKertas", "0"))
            If lebarTersimpan > 0 Then
                LebarKertas = lebarTersimpan
            Else
                ' Fallback dari UkuranKertas
                LebarKertas = If(UkuranKertas.Contains("58"), 58, 80)
            End If
            BatasKiri = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_BatasKiri", "0"))
            JarakBaris = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_JarakBaris", "4"))
            JarakBarisEsc = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_JarakBarisEsc", "0"))
            PotongOtomatis = If(
                ModeCetak = "GDI+ (Windows Print)",
                BacaPengaturanPrinter(transaksi, "Thermal_PotongOtomatisGdi", "True").ToLower() = "true",
                BacaPengaturanPrinter(transaksi, "Thermal_PotongOtomatisEsc", "True").ToLower() = "true"
            )
            JumlahCetak = Math.Max(1, KeBilangan(If(
                ModeCetak = "GDI+ (Windows Print)",
                BacaPengaturanPrinter(transaksi, "Thermal_JumlahCetakGdi", "1"),
                BacaPengaturanPrinter(transaksi, "Thermal_JumlahCetakEsc", "1")
            )))
            ModelStruk = BacaPengaturanPrinter(transaksi, "Thermal_ModelStruk", "Model 2 — Judul Kolom, Diskon")
            PortLaciKasir = BacaPengaturanPrinter(transaksi, "Thermal_PortLaciKasir", "")
            KodeLaciKasir = BacaPengaturanPrinter(transaksi, "Thermal_KodeLaciKasir", "(Tidak Ada)")
            PinLaciKasir = If(KodeLaciKasir.Contains("Pin 5"), 5, 2)
            FontJudul = BacaPengaturanPrinter(transaksi, "Thermal_FontJudul", "Arial")
            UkuranJudul = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_UkuranJudul", "12"))
            FontKeterangan = BacaPengaturanPrinter(transaksi, "Thermal_FontKeterangan", "Arial")
            UkuranKeterangan = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_UkuranKeterangan", "9"))
            FontIsi = BacaPengaturanPrinter(transaksi, "Thermal_FontIsi", "Courier New")
            UkuranIsi = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_UkuranIsi", "9"))
            FontFooter = BacaPengaturanPrinter(transaksi, "Thermal_FontFooter", "Arial")
            UkuranFooter = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_UkuranFooter", "9"))
            TampilFooter1 = BacaPengaturanPrinter(transaksi, "Thermal_TampilFooter1", "True").ToLower() = "true"
            TampilFooter2 = BacaPengaturanPrinter(transaksi, "Thermal_TampilFooter2", "True").ToLower() = "true"
            TampilFooter3 = BacaPengaturanPrinter(transaksi, "Thermal_TampilFooter3", "True").ToLower() = "true"
            Dim dpi As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Thermal_DpiCetak", "100"))
            DpiCetak = If(dpi > 0, dpi, 100)
            TampilLogo = BacaPengaturanPrinter(transaksi, "Thermal_TampilLogo", "True").ToLower() = "true"
            EscUkuranJudul = BacaPengaturanPrinter(transaksi, "Thermal_EscUkuranJudul", "Besar (2x)")
            EscUkuranKeterangan = BacaPengaturanPrinter(transaksi, "Thermal_EscUkuranKeterangan", "Normal")
            EscUkuranIsi = BacaPengaturanPrinter(transaksi, "Thermal_EscUkuranIsi", "Normal")
            EscUkuranFooter = BacaPengaturanPrinter(transaksi, "Thermal_EscUkuranFooter", "Normal")
        End Sub
    End Class

#End Region

#Region "KONFIGURASI DOT MATRIX"

    ' Cara pakai:
    '   Dim cfgDot As New KonfigurasiDotMatrix("Jual")
    '   Dim esc As New PrinterEscPos(cfgDot.NamaPrinter, cfgDot.LebarKertas)
    '
    ' Printer: LX-310, LQ-2190, FX-890, FX-2190, dll
    ' Mode GDI+ : via driver Windows (direkomendasikan)
    ' Mode ESC/P: raw bytes, driver harus Generic Text Only
    Public Class KonfigurasiDotMatrix
        Public NamaPrinter As String
        Public LebarKertas As Integer
        Public BatasKiri As Integer
        Public JarakBaris As Integer
        Public JumlahCetak As Integer
        Public TampilFooter1 As Boolean
        Public TampilFooter2 As Boolean
        Public TampilFooter3 As Boolean
        Public ModeCetak As String
        Public UkuranFont As Integer
        Public ModelStruk As String
        Public UkuranKertas As String
        Public LebarKertasMm As Integer
        Public KodeLaciKasir As String  ' dibaca dari setting Thermal — laci dikonfigurasi di panel Thermal
        Public PinLaciKasir As Integer
        Public EscUkuranJudul As String
        Public EscUkuranKeterangan As String
        Public EscUkuranIsi As String
        Public EscUkuranFooter As String

        Sub New(transaksi As String)
            NamaPrinter = BacaPengaturanPrinter(transaksi, "DotMatrix_NamaPrinter", "")
            ModeCetak = BacaPengaturanPrinter(transaksi, "DotMatrix_ModeCetak", "GDI+ (Windows Print)")
            KodeLaciKasir = BacaPengaturanPrinter(transaksi, "Thermal_KodeLaciKasir", "(Tidak Ada)")
            PinLaciKasir = If(KodeLaciKasir.Contains("Pin 5"), 5, 2)

            ' Baca key sesuai mode — GDI dan ESC/P tidak saling menimpa
            If ModeCetak = "GDI+ (Windows Print)" Then
                LebarKertas = KeBilangan(BacaPengaturanPrinter(transaksi, "DotGdi_LebarKertas", "80"))
                BatasKiri = KeBilangan(BacaPengaturanPrinter(transaksi, "DotGdi_BatasKiri", "0"))
                JarakBaris = KeBilangan(BacaPengaturanPrinter(transaksi, "DotGdi_JarakBaris", "2"))
                JumlahCetak = Math.Max(1, KeBilangan(BacaPengaturanPrinter(transaksi, "DotGdi_JumlahCetak", "1")))
                TampilFooter1 = BacaPengaturanPrinter(transaksi, "DotGdi_TampilFooter1", "True").ToLower() = "true"
                TampilFooter2 = BacaPengaturanPrinter(transaksi, "DotGdi_TampilFooter2", "True").ToLower() = "true"
                TampilFooter3 = BacaPengaturanPrinter(transaksi, "DotGdi_TampilFooter3", "True").ToLower() = "true"
                Dim fs As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "DotGdi_UkuranFont", "9"))
                UkuranFont = If(fs > 0, fs, 9)
                ModelStruk = BacaPengaturanPrinter(transaksi, "DotGdi_ModelStruk", "Model 1 Lengkap")
                UkuranKertas = BacaPengaturanPrinter(transaksi, "DotGdi_UkuranKertas", "Continuous Form (Auto)")
                ' Hitung lebar mm dari LebarKertas karakter × lebar karakter Courier New 9pt
                ' Courier New 9pt ≈ 1.5mm per karakter (perkiraan untuk dot matrix)
                LebarKertasMm = CInt(LebarKertas * 2.54)  ' 1 char ≈ 2.54mm untuk 10cpi
            Else
                LebarKertas = KeBilangan(BacaPengaturanPrinter(transaksi, "DotEsc_LebarKertas", "80"))
                BatasKiri = KeBilangan(BacaPengaturanPrinter(transaksi, "DotEsc_BatasKiri", "0"))
                JarakBaris = KeBilangan(BacaPengaturanPrinter(transaksi, "DotEsc_JarakBaris", "0"))
                JumlahCetak = Math.Max(1, KeBilangan(BacaPengaturanPrinter(transaksi, "DotEsc_JumlahCetak", "1")))
                TampilFooter1 = BacaPengaturanPrinter(transaksi, "DotEsc_TampilFooter1", "True").ToLower() = "true"
                TampilFooter2 = BacaPengaturanPrinter(transaksi, "DotEsc_TampilFooter2", "True").ToLower() = "true"
                TampilFooter3 = BacaPengaturanPrinter(transaksi, "DotEsc_TampilFooter3", "True").ToLower() = "true"
                UkuranFont = 9  ' tidak relevan untuk ESC/P
                ModelStruk = BacaPengaturanPrinter(transaksi, "DotEsc_ModelStruk", "Model 1 Lengkap")
                UkuranKertas = ""  ' tidak relevan untuk ESC/P
                EscUkuranJudul = BacaPengaturanPrinter(transaksi, "DotEsc_EscUkuranJudul", "Besar (2x)")
                EscUkuranKeterangan = BacaPengaturanPrinter(transaksi, "DotEsc_EscUkuranKeterangan", "Normal")
                EscUkuranIsi = BacaPengaturanPrinter(transaksi, "DotEsc_EscUkuranIsi", "Normal")
                EscUkuranFooter = BacaPengaturanPrinter(transaksi, "DotEsc_EscUkuranFooter", "Normal")
            End If
        End Sub
    End Class

#End Region

#Region "KONFIGURASI INKJET / LASER"

    ' Cara pakai:
    '   Dim cfgInk As New KonfigurasiInkjet("LaporanKas")
    '   PD.PrinterSettings.PrinterName = cfgInk.NamaPrinter
    Public Class KonfigurasiInkjet
        Public NamaPrinter As String
        Public UkuranKertas As String
        Public Orientasi As String
        Public JumlahCetak As Integer
        Public MarginAtas As Integer
        Public MarginBawah As Integer
        Public MarginKiri As Integer
        Public MarginKanan As Integer
        Public FontJudul As String : Public UkuranJudul As Integer
        Public FontIsi As String : Public UkuranIsi As Integer
        Public TampilFooter1 As Boolean
        Public TampilFooter2 As Boolean
        Public TampilFooter3 As Boolean
        ' Layout nota inkjet — semua dinamis dari printer.ini
        Public ModelNota As String          ' "Model 1 Lengkap" | "Model 2 Tanpa Diskon" | "Model 2 Dengan Total Hutang"
        Public TampilLogo As Boolean        ' tampilkan logo di header
        Public TampilTandaTangan As Boolean ' tampilkan area tanda tangan (dikontrol via checkbox)
        ' Lebar kolom tabel item — dalam persen dari lebar area cetak (total harus <= 100)
        ' Kolom: No | Nama Barang | Qty | Harga | Diskon | Jumlah
        Public PctKolomNo As Integer        ' default 4
        Public PctKolomQty As Integer       ' default 14
        Public PctKolomHarga As Integer     ' default 16
        Public PctKolomDiskon As Integer    ' default 14
        ' Kolom Nama = sisa (100 - No - Qty - Harga - Diskon - Jumlah)
        ' Kolom Jumlah = PctKolomHarga (sama lebar dengan Harga)

        Sub New(transaksi As String)
            NamaPrinter = BacaPengaturanPrinter(transaksi, "Inkjet_NamaPrinter", "")
            UkuranKertas = BacaPengaturanPrinter(transaksi, "Inkjet_UkuranKertas", "A4")
            Orientasi = BacaPengaturanPrinter(transaksi, "Inkjet_Orientasi", "Portrait")
            JumlahCetak = Math.Max(1, KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_JumlahCetak", "1")))
            MarginAtas = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_MarginAtas", "10"))
            MarginBawah = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_MarginBawah", "10"))
            MarginKiri = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_MarginKiri", "15"))
            MarginKanan = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_MarginKanan", "10"))
            FontJudul = BacaPengaturanPrinter(transaksi, "Inkjet_FontJudul", "Arial")
            UkuranJudul = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_UkuranJudul", "12"))
            FontIsi = BacaPengaturanPrinter(transaksi, "Inkjet_FontIsi", "Arial")
            UkuranIsi = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_UkuranIsi", "10"))
            TampilFooter1 = BacaPengaturanPrinter(transaksi, "Inkjet_TampilFooter1", "True").ToLower() = "true"
            TampilFooter2 = BacaPengaturanPrinter(transaksi, "Inkjet_TampilFooter2", "True").ToLower() = "true"
            TampilFooter3 = BacaPengaturanPrinter(transaksi, "Inkjet_TampilFooter3", "True").ToLower() = "true"
            ModelNota = BacaPengaturanPrinter(transaksi, "Inkjet_ModelNota", "Model 1 Lengkap")
            TampilLogo = BacaPengaturanPrinter(transaksi, "Inkjet_TampilLogo", "True").ToLower() = "true"
            TampilTandaTangan = BacaPengaturanPrinter(transaksi, "Inkjet_TampilTandaTangan", "True").ToLower() = "true"
            Dim pNo As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_PctKolomNo", "4"))
            Dim pQty As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_PctKolomQty", "8"))
            Dim pHarga As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_PctKolomHarga", "12"))
            Dim pDiskon As Integer = KeBilangan(BacaPengaturanPrinter(transaksi, "Inkjet_PctKolomDiskon", "10"))
            PctKolomNo = If(pNo > 0, pNo, 4)
            PctKolomQty = If(pQty > 0, pQty, 8)
            PctKolomHarga = If(pHarga > 0, pHarga, 12)
            PctKolomDiskon = If(pDiskon > 0, pDiskon, 10)
        End Sub
    End Class

#End Region

#Region "KONFIGURASI MONITOR"

    ' Hanya menyimpan pilihan footer — tidak ada printer.
    Public Class KonfigurasiMonitor
        Public TampilFooter1 As Boolean
        Public TampilFooter2 As Boolean
        Public TampilFooter3 As Boolean

        Sub New(transaksi As String)
            TampilFooter1 = BacaPengaturanPrinter(transaksi, "Monitor_TampilFooter1", "True").ToLower() = "true"
            TampilFooter2 = BacaPengaturanPrinter(transaksi, "Monitor_TampilFooter2", "True").ToLower() = "true"
            TampilFooter3 = BacaPengaturanPrinter(transaksi, "Monitor_TampilFooter3", "True").ToLower() = "true"
        End Sub
    End Class

#End Region

#Region "KONFIGURASI PDF"

    ' Hanya menyimpan pilihan footer — printer pakai KonfigurasiInkjet.
    Public Class KonfigurasiPDF
        Public TampilFooter1 As Boolean
        Public TampilFooter2 As Boolean
        Public TampilFooter3 As Boolean

        Sub New(transaksi As String)
            TampilFooter1 = BacaPengaturanPrinter(transaksi, "PDF_TampilFooter1", "True").ToLower() = "true"
            TampilFooter2 = BacaPengaturanPrinter(transaksi, "PDF_TampilFooter2", "True").ToLower() = "true"
            TampilFooter3 = BacaPengaturanPrinter(transaksi, "PDF_TampilFooter3", "True").ToLower() = "true"
        End Sub
    End Class

#End Region

#Region "DETEKSI JENIS PRINTER"

    ' ================================================================
    ' Daftar keyword nama printer Dot Matrix ESC/P (Raw)
    ' Printer ini TIDAK mendukung cetak gambar/logo via ESC/P.
    ' Termasuk juga jika nama mengandung "(Copy" — printer virtual/duplikat.
    ' ================================================================
    Private ReadOnly _keywordDotMatrixEscP As String() = {
        "LX-300", "LX-350", "LX-1170", "LX300", "LX350", "LX1170",
        "LQ-300", "LQ-350", "LQ-590", "LQ-690", "LQ-2090", "LQ-2190",
        "LQ300", "LQ350", "LQ590", "LQ690", "LQ2090", "LQ2190",
        "FX-890", "FX-2190", "FX890", "FX2190",
        "DFX-9000", "DFX9000",
        "OKI ML", "OKI MICROLINE", "MICROLINE",
        "PANASONIC KX-P", "KX-P1121", "KX-P1131", "KX-P2130",
        "CITIZEN GSX", "CITIZEN GSX-190",
        "STAR NX", "STAR LC",
        "GENERIC TEXT", "GENERIC / TEXT ONLY", "TEXT ONLY"
    }

    ' Daftar keyword nama printer Dot Matrix GDI+ (via driver Windows)
    ' Printer ini bisa cetak gambar tapi lambat dan berisik.
    ' Referensi: Epson LQ/LX/FX series dengan driver Windows resmi,
    ' OKI Microline series, Panasonic KX-P series dengan driver GDI.
    Private ReadOnly _keywordDotMatrixGdi As String() = {
        "LQ-590II", "LQ-690II", "LQ-2090II", "LQ-2190II",
        "LQ590II", "LQ690II", "LQ2090II", "LQ2190II",
        "LQ-2180", "LQ2180",
        "LQ-350", "LQ350",
        "LQ-300+II", "LQ300+II",
        "LX-350", "LX350",
        "EPSON LQ", "EPSON LX", "EPSON FX",
        "OKI ML5521", "OKI ML5591", "OKI ML5720", "OKI ML5790",
        "OKI ML1120", "OKI ML3320", "OKI ML3390",
        "PANASONIC KX-P2023", "PANASONIC KX-P2624",
        "PANASONIC KX-P3626", "PANASONIC KX-P3696",
        "CITIZEN 120D", "CITIZEN 124D", "CITIZEN 132D",
        "STAR LC-100", "STAR LC-200", "STAR LC24"
    }

    ''' <summary>
    ''' Cek apakah nama printer adalah Dot Matrix ESC/P (Raw) — tidak mendukung logo.
    ''' Juga mendeteksi printer virtual/copy (mengandung "(Copy").
    ''' </summary>
    Public Function IsPrinterDotMatrixEscP(namaPrinter As String) As Boolean
        If String.IsNullOrEmpty(namaPrinter) Then Return False
        Dim upper As String = namaPrinter.ToUpper()
        ' Printer copy/virtual — tidak bisa cetak logo
        If upper.Contains("(COPY") Then Return True
        For Each kw As String In _keywordDotMatrixEscP
            If upper.Contains(kw.ToUpper()) Then Return True
        Next
        Return False
    End Function

    ''' <summary>
    ''' Cek apakah nama printer adalah Dot Matrix GDI+ — bisa logo tapi lambat.
    ''' </summary>
    Public Function IsPrinterDotMatrixGdi(namaPrinter As String) As Boolean
        If String.IsNullOrEmpty(namaPrinter) Then Return False
        If IsPrinterDotMatrixEscP(namaPrinter) Then Return False
        Dim upper As String = namaPrinter.ToUpper()
        For Each kw As String In _keywordDotMatrixGdi
            If upper.Contains(kw.ToUpper()) Then Return True
        Next
        Return False
    End Function

    ''' <summary>
    ''' Tentukan apakah logo bisa dicetak berdasarkan 3 fase:
    ''' 1. JenisPrinter (Thermal/Dot/Inkjet)
    ''' 2. ModeCetak (ESC/POS, GDI+, ESC/P)
    ''' 3. NamaPrinter (deteksi dot matrix ESC/P dari nama)
    ''' </summary>
    Public Function LogoBisaDicetak(jenisPrinter As String, modeCetak As String,
                                     namaPrinter As String) As Boolean
        Select Case jenisPrinter
            Case "Printer Thermal"
                ' ESC/POS dan GDI+ thermal sama-sama mendukung logo
                ' Kecuali jika nama printer mengandung "(Copy" atau keyword ESC/P
                Return Not IsPrinterDotMatrixEscP(namaPrinter)
            Case "Printer Dot Matrix"
                If modeCetak = "ESC/P (Raw)" Then Return False
                ' GDI+ dot matrix: bisa logo tapi tidak direkomendasikan
                ' Tetap izinkan, user yang memutuskan via checkbox
                Return Not IsPrinterDotMatrixEscP(namaPrinter)
            Case Else
                Return False  ' Inkjet/Monitor/PDF tidak pakai panel ini
        End Select
    End Function

#End Region

#Region "HELPER GDI+"

    ' Untuk GDI+ (file cetak lama)
    '
    ' Cara pakai:
    '   Dim garis As String = BuatGaris(lebar)         ' ----...
    '   Dim garisdua As String = BuatGarisGanda(lebar)  ' ====...
    '   Dim garisTitik As String = BuatGarisTitik(lebar) ' ....
    '
    ' lebar = jumlah karakter per baris, dihitung dari lebar kertas:
    '   Dim lebar As Integer = CInt(lebarPixel / lebarPerKarakter)
    '   Atau pakai konstanta: LebarGarisDefault(lebarKertasMm)

    Public Function BuatGaris(lebarKarakter As Integer) As String
        Return New String("-"c, Math.Max(1, lebarKarakter))
    End Function

    Public Function BuatGarisGanda(lebarKarakter As Integer) As String
        Return New String("="c, Math.Max(1, lebarKarakter))
    End Function

    Public Function BuatGarisTitik(lebarKarakter As Integer) As String
        Return New String("."c, Math.Max(1, lebarKarakter))
    End Function

    Public Function BuatGarisKustom(karakter As Char, lebarKarakter As Integer) As String
        Return New String(karakter, Math.Max(1, lebarKarakter))
    End Function

    ' Hitung jumlah karakter per baris dari lebar kertas mm dan pixel per karakter
    ' lebarKertasMm: lebar kertas dalam mm (80 atau 58)
    ' dpi: resolusi printer (default 100)
    ' lebarFontPx: lebar rata-rata satu karakter dalam pixel (default 6 untuk font 10pt)
    Public Function HitungLebarGaris(lebarKertasMm As Integer,
                                      Optional dpi As Integer = 100,
                                      Optional lebarFontPx As Integer = 6) As Integer
        Dim lebarPx As Integer = CInt(lebarKertasMm / 25.4 * dpi)
        Return Math.Max(1, CInt(lebarPx / lebarFontPx))
    End Function

#End Region

#Region "HELPER INTERNAL"

    Friend Function KeBilangan(nilai As String) As Integer
        Dim n As Integer
        Return If(Integer.TryParse(nilai, n), n, 0)
    End Function

#End Region

#Region "ALIAS LAMA - Kompatibilitas (hapus setelah semua file dimigrasi)"

    Public Function GetPrinterTransaksi(transaksi As String, field As String,
                                         Optional defaultVal As String = "") As String
        Return BacaPengaturanPrinter(transaksi, field, defaultVal)
    End Function

    Public Function GetJenisPrinterTransaksi(transaksi As String) As String
        Return AmbilJenisPrinter(transaksi)
    End Function

    Public Function GetNamaPrinterTransaksi(transaksi As String, jenis As String) As String
        Return AmbilNamaPrinter(transaksi, jenis)
    End Function

    Public Function GetCopiesTransaksi(transaksi As String, jenis As String) As Integer
        Return AmbilJumlahCetak(transaksi, jenis)
    End Function

    Friend Function GetInt(value As String) As Integer
        Return KeBilangan(value)
    End Function

#End Region

End Module
