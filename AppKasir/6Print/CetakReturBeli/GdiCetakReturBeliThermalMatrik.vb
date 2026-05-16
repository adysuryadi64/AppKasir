Imports System.Drawing.Printing

' ================================================================
' GdiCetakReturBeliThermalMatrik — GDI+ untuk nota retur pembelian
' ================================================================
Public Class GdiCetakReturBeliThermalMatrik

#Region "Field & Konstruktor"
    Private ReadOnly _cfg As KonfigurasiThermal
    Private _panjangKertas As Integer
    Private WithEvents _pd As New PrintDocument
    Private WithEvents _pd1 As New PrintDocument
    Private WithEvents _pd2 As New PrintDocument

    Public Sub New()
        _cfg = New KonfigurasiThermal("ReturBeli")
    End Sub

    Public Property TampilFooter1Override As Boolean? = Nothing
    Public Property TampilFooter2Override As Boolean? = Nothing
    Public Property TampilFooter3Override As Boolean? = Nothing

    Private ReadOnly Property ShowFooter1 As Boolean
        Get
            Return If(TampilFooter1Override.HasValue, TampilFooter1Override.GetValueOrDefault(), _cfg.TampilFooter1)
        End Get
    End Property
    Private ReadOnly Property ShowFooter2 As Boolean
        Get
            Return If(TampilFooter2Override.HasValue, TampilFooter2Override.GetValueOrDefault(), _cfg.TampilFooter2)
        End Get
    End Property
    Private ReadOnly Property ShowFooter3 As Boolean
        Get
            Return If(TampilFooter3Override.HasValue, TampilFooter3Override.GetValueOrDefault(), _cfg.TampilFooter3)
        End Get
    End Property

    Private Function Rp(nilai As Decimal) As String
        Return ReturBeliRp(nilai)
    End Function
#End Region

#Region "Cetak / Preview / PDF"
    Public Sub Cetak()
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfg.JumlahCetak
            AturDanCetak(False)
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir("ReturBeli")
    End Sub

    Public Sub CetakDotMatrix()
        Dim cfgDot As New KonfigurasiDotMatrix("ReturBeli")
        If String.IsNullOrEmpty(cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To cfgDot.JumlahCetak
            AturDanCetak(False)
        Next
    End Sub

    Public Sub TampilkanPreview()
        AturDanCetak(True)
    End Sub

    Public Shared Sub TampilkanPreviewStatic(f1 As Boolean, f2 As Boolean, f3 As Boolean)
        Dim c As New GdiCetakReturBeliThermalMatrik()
        c.TampilFooter1Override = f1 : c.TampilFooter2Override = f2 : c.TampilFooter3Override = f3
        c.TampilkanPreview()
    End Sub

    Public Sub RenderToBitmaps(bitmaps As List(Of System.Drawing.Bitmap))
        HitungPanjangKertas()
        Dim bmp As New System.Drawing.Bitmap(LebarPx, _panjangKertas)
        bmp.SetResolution(100, 100)
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            g.Clear(System.Drawing.Color.White)
            Dim e As New PrintPageEventArgs(g,
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                _pd.DefaultPageSettings)
            Select Case _cfg.ModelStruk
                Case "Model 2 Tanpa Header" : CetakModel2(e)
                Case Else                   : CetakModel1(e)
            End Select
        End Using
        bitmaps.Add(bmp)
    End Sub

    Private Sub AturDanCetak(preview As Boolean)
        HitungPanjangKertas()
        Dim pd As PrintDocument = PilihPd()
        pd.PrinterSettings.PrinterName = _cfg.NamaPrinter
        If preview Then
            Dim ppd As New PrintPreviewDialog() With {.Document = pd, .WindowState = FormWindowState.Maximized}
            ppd.ShowDialog()
        Else
            pd.Print()
        End If
    End Sub

    Private Function PilihPd() As PrintDocument
        Select Case _cfg.ModelStruk
            Case "Model 2 Tanpa Header" : Return _pd1
            Case Else                   : Return _pd
        End Select
    End Function

    Private Sub HitungPanjangKertas()
        _panjangKertas = ReturBeli_DaftarItem.Count * 30 + 320
        Dim tpb As Integer = 12 + Jarak
        If ShowFooter1 Then _panjangKertas += FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tpb
        If ShowFooter2 Then _panjangKertas += FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tpb
        If ShowFooter3 Then _panjangKertas += FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tpb
    End Sub
#End Region

#Region "Properties"
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
    Private ReadOnly Property BatasKiri As Integer
        Get
            Return 2 + _cfg.BatasKiri
        End Get
    End Property
    Private ReadOnly Property PosNilai As Integer
        Get
            Return BatasKiri + CInt(LebarPx * 0.25)
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
#End Region

#Region "BeginPrint & PrintPage Handlers"
    Private Sub Pd_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd.BeginPrint
        _pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        _pd.DefaultPageSettings.Landscape = False
    End Sub
    Private Sub Pd1_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd1.BeginPrint
        _pd1.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        _pd1.DefaultPageSettings.Landscape = False
    End Sub
    Private Sub Pd2_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd2.BeginPrint
        _pd2.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        _pd2.DefaultPageSettings.Landscape = False
    End Sub
    Private Sub Pd_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakModel1(e)
    End Sub
    Private Sub Pd1_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd1.PrintPage
        CetakModel2(e)
    End Sub
    Private Sub Pd2_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd2.PrintPage
        CetakModel1(e)
    End Sub
#End Region

#Region "Helper GDI+"
    Private Sub Tulis(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        g.DrawString(teks, fnt, Brushes.Black, x, y)
    End Sub
    Private Sub TulisKanan(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Far}
        g.DrawString(teks, fnt, Brushes.Black, New RectangleF(BatasKiri, y, x - BatasKiri, fnt.GetHeight(g) + 2), fmt)
    End Sub
    Private Sub TulisTengah(g As Graphics, teks As String, fnt As Font, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center}
        g.DrawString(teks, fnt, Brushes.Black, Tengah, y, fmt)
    End Sub
#End Region

#Region "Model Cetak"
    Private Sub CetakModel1(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeaderDenganLogo(g, 5)
        y = CetakInfo(g, y) : y = CetakItem(g, y) : y = CetakTotal(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub
    Private Sub CetakModel2(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeaderDenganLogo(g, 5)
        y = CetakInfoSingkat(g, y) : y = CetakItem(g, y) : y = CetakTotal(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub
    Private Sub CetakModel3(e As PrintPageEventArgs)
        ' Alias lama — logo dikontrol via _cfg.TampilLogo
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeaderDenganLogo(g, 5)
        y = CetakInfo(g, y) : y = CetakItem(g, y) : y = CetakTotal(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub
#End Region

#Region "Blok Cetak GDI+"
    Private Function CetakHeaderDenganLogo(g As Graphics, y As Integer) As Integer
        If _cfg.TampilLogo Then
            Try
                Dim logo As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
                g.DrawImage(logo, CInt((LebarPx - 150) / 2), y, 150, 35) : logo.Dispose()
            Catch : End Try
            y += 30 + Jarak
        End If
        TulisTengah(g, NAMA_PERUSAHAAN, FJudul, y) : y += 20 + Jarak
        TulisTengah(g, ALAMAT_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        TulisTengah(g, KOTA_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        TulisTengah(g, KONTAK_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        Return y
    End Function

    Private Function CetakHeaderTanpaLogo(g As Graphics, y As Integer) As Integer
        Return CetakHeaderDenganLogo(g, y)
    End Function

    Private Function CetakInfo(g As Graphics, y As Integer, Optional singkat As Boolean = False) As Integer
        Dim pv As Integer = PosNilai
        y += 15 + Jarak
        Tulis(g, "Nota Retur", FKet, BatasKiri, y) : Tulis(g, ": " & ReturBeli_NoRetur, FKet, pv, y) : y += 10 + Jarak
        Tulis(g, If(singkat, "Tgl", "Tanggal"), FKet, BatasKiri, y) : Tulis(g, ": " & ReturBeli_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), FKet, pv, y) : y += 10 + Jarak
        Tulis(g, "Kasir", FKet, BatasKiri, y) : Tulis(g, ": " & ReturBeli_IdUser & " - " & ReturBeli_IdKomputer, FKet, pv, y) : y += 10 + Jarak
        Tulis(g, "Supplier", FKet, BatasKiri, y) : Tulis(g, ": " & ReturBeli_NamaSupplier, FKet, pv, y) : y += 14 + Jarak
        Tulis(g, Garis, FGaris, BatasKiri, y) : y += 10 + Jarak
        TulisTengah(g, "RETUR PEMBELIAN", New Font(_cfg.FontKeterangan, _cfg.UkuranKeterangan + 2), y) : y += 14 + Jarak
        Tulis(g, Garis, FGaris, BatasKiri, y)
        Return y
    End Function

    Private Function CetakInfoSingkat(g As Graphics, y As Integer) As Integer
        Return CetakInfo(g, y, singkat:=True)
    End Function

    Private Function CetakItem(g As Graphics, y As Integer) As Integer
        Dim m1 As Integer = BatasKiri + CInt(LebarPx * 0.11)
        Dim m2 As Integer = BatasKiri + CInt(LebarPx * 0.11)
        Dim m3 As Integer = BatasKiri + CInt(LebarPx * 0.65)
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)

        y += 10 + Jarak
        Tulis(g, "Nama Barang", FIsi, BatasKiri, y)
        TulisKanan(g, "Harga", FIsi, m3, y)
        TulisKanan(g, "Jumlah", FIsi, m5, y) : y += 10 + Jarak
        Tulis(g, Garis, FGaris, BatasKiri, y)

        For Each item As ItemNotaReturBeli In ReturBeli_DaftarItem
            y += 14 + Jarak
            Tulis(g, item.NamaBarang, FIsi, BatasKiri, y) : y += 10 + Jarak
            TulisKanan(g, item.Qty.ToString("#,0.##", cultureIndonesia), FIsi, m1, y)
            Tulis(g, item.Satuan, FIsi, m2, y)
            TulisKanan(g, Rp(item.Harga), FIsi, m3, y)
            TulisKanan(g, Rp(item.Total), FIsi, m5, y)
        Next
        y += 10 + Jarak
        Tulis(g, Garis, FGaris, BatasKiri, y)
        Return y
    End Function

    Private Function CetakTotal(g As Graphics, y As Integer) As Integer
        Dim m3 As Integer = BatasKiri + CInt(LebarPx * 0.51)
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)
        y += 5 + Jarak
        Tulis(g, ReturBeli_DaftarItem.Count & " item", FIsi, BatasKiri, y)
        TulisKanan(g, "Total :", FIsi, m3, y)
        TulisKanan(g, Rp(ReturBeli_Total), FIsi, m5, y)
        y += 10 + Jarak
        Tulis(g, Garis, FGaris, BatasKiri, y)
        Return y
    End Function

    Private Sub CetakFooter(g As Graphics, y As Integer)
        y += 10 + Jarak
        If ShowFooter1 Then
            For Each b As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, b, FFooter, y) : y += 10 + Jarak
            Next
        End If
        If ShowFooter2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, b, FFooter, y) : y += 10 + Jarak
            Next
        End If
        If ShowFooter3 Then
            For Each b As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, b, FFooter, y) : y += 10 + Jarak
            Next
        End If
    End Sub
#End Region

End Class
