Imports System.Drawing.Printing

Public Class GdiCetakBonKaryawan

    Private ReadOnly _cfg As KonfigurasiThermal
    Private _panjangKertas As Integer
    Private WithEvents _pd As New PrintDocument

    Public Sub New()
        _cfg = New KonfigurasiThermal("BonKaryawan")
    End Sub

    Public Property TampilFooter1Override As Boolean? = Nothing
    Public Property TampilFooter2Override As Boolean? = Nothing
    Public Property TampilFooter3Override As Boolean? = Nothing

    Private ReadOnly Property ShowF1 As Boolean
        Get
            Return If(TampilFooter1Override.HasValue, TampilFooter1Override.GetValueOrDefault(), _cfg.TampilFooter1)
        End Get
    End Property
    Private ReadOnly Property ShowF2 As Boolean
        Get
            Return If(TampilFooter2Override.HasValue, TampilFooter2Override.GetValueOrDefault(), _cfg.TampilFooter2)
        End Get
    End Property
    Private ReadOnly Property ShowF3 As Boolean
        Get
            Return If(TampilFooter3Override.HasValue, TampilFooter3Override.GetValueOrDefault(), _cfg.TampilFooter3)
        End Get
    End Property

    Private Function Rp(v As Decimal) As String
        Return BKRp(v)
    End Function
    Private ReadOnly Property LebarPx As Integer
        Get
            Return CInt(_cfg.LebarKertas / 25.4 * _cfg.DpiCetak)
        End Get
    End Property
    Private ReadOnly Property Tengah As Integer
        Get
            Return LebarPx \ 2
        End Get
    End Property
    Private ReadOnly Property BK As Integer
        Get
            Return 2 + _cfg.BatasKiri
        End Get
    End Property
    Private ReadOnly Property PosNilai As Integer
        Get
            Return BK + CInt(LebarPx * 0.25)
        End Get
    End Property
    Private ReadOnly Property Jarak As Integer
        Get
            Return _cfg.JarakBaris
        End Get
    End Property
    Private ReadOnly Property Garis As String
        Get
            Return BuatGaris(HitungLebarGaris(_cfg.LebarKertas))
        End Get
    End Property
    Private ReadOnly Property FJudul As Font
        Get
            Return New Font(_cfg.FontJudul, _cfg.UkuranJudul)
        End Get
    End Property
    Private ReadOnly Property FKet As Font
        Get
            Return New Font(_cfg.FontKeterangan, _cfg.UkuranKeterangan)
        End Get
    End Property
    Private ReadOnly Property FIsi As Font
        Get
            Return New Font(_cfg.FontIsi, _cfg.UkuranIsi)
        End Get
    End Property
    Private ReadOnly Property FFooter As Font
        Get
            Return New Font(_cfg.FontFooter, _cfg.UkuranFooter)
        End Get
    End Property
    Private ReadOnly Property FGaris As Font
        Get
            Return New Font("Courier New", 8)
        End Get
    End Property

    Public Sub Cetak()
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfg.JumlahCetak
            HitungPanjang() : _pd.PrinterSettings.PrinterName = _cfg.NamaPrinter : _pd.Print()
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir("BonKaryawan")
    End Sub

    Public Sub CetakDotMatrix()
        Dim cfgDot As New KonfigurasiDotMatrix("BonKaryawan")
        If String.IsNullOrEmpty(cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To cfgDot.JumlahCetak
            HitungPanjang() : _pd.PrinterSettings.PrinterName = cfgDot.NamaPrinter : _pd.Print()
        Next
    End Sub

    Public Sub TampilkanPreview()
        HitungPanjang()
        Dim ppd As New PrintPreviewDialog() With {.Document = _pd, .WindowState = FormWindowState.Maximized}
        ppd.ShowDialog()
    End Sub

    Public Sub RenderToBitmaps(bitmaps As List(Of System.Drawing.Bitmap))
        HitungPanjang()
        Dim bmp As New System.Drawing.Bitmap(LebarPx, _panjangKertas)
        bmp.SetResolution(100, 100)
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            g.Clear(System.Drawing.Color.White)
            Dim e As New PrintPageEventArgs(g,
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                _pd.DefaultPageSettings)
            CetakHalaman(e)
        End Using
        bitmaps.Add(bmp)
    End Sub

    Private Sub HitungPanjang()
        _panjangKertas = 380
    End Sub

    Private Sub Pd_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd.BeginPrint
        _pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        _pd.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub Pd_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakHalaman(e)
    End Sub

    Private Sub CetakHalaman(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim fmtTengah As New StringFormat() With {.Alignment = StringAlignment.Center}
        Dim fmtKanan As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim y As Integer = 5

        ' Header
        If _cfg.TampilLogo Then
            Try
                Dim logo As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
                g.DrawImage(logo, CInt((LebarPx - 150) / 2), y, 150, 35) : logo.Dispose()
            Catch : End Try
            y += 30 + Jarak
        End If
        g.DrawString(NAMA_PERUSAHAAN, FJudul, Brushes.Black, Tengah, y, fmtTengah) : y += 20 + Jarak
        g.DrawString(ALAMAT_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 10 + Jarak
        g.DrawString(KOTA_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 10 + Jarak
        g.DrawString(KONTAK_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 15 + Jarak

        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak
        Dim pv As Integer = PosNilai
        g.DrawString("Faktur", FKet, Brushes.Black, BK, y) : g.DrawString(": " & BK_Faktur, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Tanggal", FKet, Brushes.Black, BK, y) : g.DrawString(": " & BK_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Kasir", FKet, Brushes.Black, BK, y) : g.DrawString(": " & BK_IdUser & " - " & BK_IdKomputer, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Karyawan", FKet, Brushes.Black, BK, y) : g.DrawString(": " & BK_NamaKaryawan, FKet, Brushes.Black, pv, y) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak
        g.DrawString("SLIP BON KARYAWAN", New Font(_cfg.FontKeterangan, _cfg.UkuranKeterangan + 2), Brushes.Black, Tengah, y, fmtTengah) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak

        ' Isi
        Dim mNilai As Integer = BK + CInt(LebarPx * 0.95)
        g.DrawString("Saldo Awal", FIsi, Brushes.Black, BK, y)
        g.DrawString(Rp(BK_AwalBon), FIsi, Brushes.Black, mNilai, y, fmtKanan) : y += 10 + Jarak
        g.DrawString("Bon (" & BK_Jenis & ")", FIsi, Brushes.Black, BK, y)
        g.DrawString(Rp(BK_Nominal), FIsi, Brushes.Black, mNilai, y, fmtKanan) : y += 10 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak
        g.DrawString("Saldo Akhir", New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), Brushes.Black, BK, y)
        g.DrawString(Rp(BK_AkhirBon), New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), Brushes.Black, mNilai, y, fmtKanan) : y += 10 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak
        If Not String.IsNullOrEmpty(BK_Keterangan) Then
            g.DrawString("Ket: " & BK_Keterangan, FKet, Brushes.Gray, BK, y) : y += 10 + Jarak
        End If

        If ShowF1 Then
            For Each b As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                g.DrawString(b, FFooter, Brushes.Gray, Tengah, y, fmtTengah) : y += 10 + Jarak
            Next
        End If
        If ShowF2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                g.DrawString(b, FFooter, Brushes.Gray, Tengah, y, fmtTengah) : y += 10 + Jarak
            Next
        End If
        If ShowF3 Then
            For Each b As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                g.DrawString(b, FFooter, Brushes.Gray, Tengah, y, fmtTengah) : y += 10 + Jarak
            Next
        End If

        e.HasMorePages = False
    End Sub

End Class
