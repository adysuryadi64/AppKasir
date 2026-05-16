Imports System.Drawing.Printing

' ================================================================
' GdiCetakTransferCabang
' Cetak nota transfer antar cabang via GDI+ (Thermal GDI / Dot Matrix GDI).
' Mengikuti pola GdiCetakJualThermalMatrik yang sudah verified.
' ================================================================
Public Class GdiCetakTransferCabang

#Region "Field & Konstruktor"

    Private ReadOnly _cfg As KonfigurasiThermal
    Private ReadOnly _cfgDot As KonfigurasiDotMatrix
    Private _panjangKertas As Integer
    Private _lebarKertas As Integer
    Private WithEvents _pd As New PrintDocument

    Public Sub New()
        _cfg = New KonfigurasiThermal("TransferCabang")
        _cfgDot = New KonfigurasiDotMatrix("TransferCabang")
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
        Return TCRp(nilai)
    End Function

#End Region

#Region "Cetak / Preview / PDF"

    Public Sub Cetak()
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfg.JumlahCetak
            HitungUkuranKertas(isThermal:=True)
            _pd.PrinterSettings.PrinterName = _cfg.NamaPrinter
            _pd.Print()
        Next
    End Sub

    Public Sub CetakDotMatrix()
        If String.IsNullOrEmpty(_cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfgDot.JumlahCetak
            HitungUkuranKertas(isThermal:=False)
            _pd.PrinterSettings.PrinterName = _cfgDot.NamaPrinter
            _pd.Print()
        Next
    End Sub

    Public Sub TampilkanPreview()
        HitungUkuranKertas(isThermal:=True)
        Dim ppd As New PrintPreviewDialog() With {.Document = _pd, .WindowState = FormWindowState.Maximized}
        ppd.ShowDialog()
    End Sub

    Public Sub RenderToBitmaps(bitmaps As List(Of System.Drawing.Bitmap))
        HitungUkuranKertas(isThermal:=True)
        Dim bmp As New System.Drawing.Bitmap(_lebarKertas, _panjangKertas)
        bmp.SetResolution(100, 100)
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            g.Clear(System.Drawing.Color.White)
            Dim e As New PrintPageEventArgs(g,
                New System.Drawing.Rectangle(0, 0, _lebarKertas, _panjangKertas),
                New System.Drawing.Rectangle(0, 0, _lebarKertas, _panjangKertas),
                _pd.DefaultPageSettings)
            CetakHalaman(e)
        End Using
        bitmaps.Add(bmp)
    End Sub

    Private Sub HitungUkuranKertas(isThermal As Boolean)
        Dim lebarMm As Integer = If(isThermal, _cfg.LebarKertas, _cfgDot.LebarKertas)
        Dim dpi As Integer = If(isThermal, _cfg.DpiCetak, 100)
        _lebarKertas = CInt(lebarMm / 25.4 * dpi)
        _panjangKertas = TC_DaftarItem.Count * 30 + 420
    End Sub

#End Region

#Region "PrintDocument Handlers"

    Private Sub Pd_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd.BeginPrint
        _pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", _lebarKertas, _panjangKertas)
        _pd.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub Pd_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakHalaman(e)
    End Sub

#End Region

#Region "Properties Helper"

    Private ReadOnly Property LebarPx As Integer
        Get
            Return _lebarKertas
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
    Private ReadOnly Property PosNilaiKanan As Integer
        Get
            Return BatasKiri + CInt(LebarPx * 0.25)
        End Get
    End Property
    Private ReadOnly Property Jarak As Integer
        Get
            Return _cfg.JarakBaris
        End Get
    End Property
    Private ReadOnly Property GarisPemisah As String
        Get
            Return BuatGaris(HitungLebarGaris(_cfg.LebarKertas))
        End Get
    End Property

#End Region

#Region "Font Properties"

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

#Region "Helper Gambar GDI+"

    Private Sub Tulis(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        g.DrawString(teks, fnt, Brushes.Black, x, y)
    End Sub

    Private Sub TulisKanan(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim rect As New RectangleF(BatasKiri, y, x - BatasKiri, fnt.GetHeight(g) + 2)
        g.DrawString(teks, fnt, Brushes.Black, rect, fmt)
    End Sub

    Private Sub TulisTengah(g As Graphics, teks As String, fnt As Font, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center}
        g.DrawString(teks, fnt, Brushes.Black, Tengah, y, fmt)
    End Sub

#End Region

#Region "Cetak Halaman"

    Private Sub CetakHalaman(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = 0

        y = CetakHeader(g, y)
        y = CetakInfoTransaksi(g, y)
        y = CetakItem(g, y)
        y = CetakTotal(g, y)
        CetakFooter(g, y)

        e.HasMorePages = False
    End Sub

    Private Function CetakHeader(g As Graphics, y As Integer) As Integer
        If _cfg.TampilLogo Then
            Try
                Dim logo As Image = Image.FromFile(Application.StartupPath() & "\logo.png")
                g.DrawImage(logo, CInt((LebarPx - 150) / 2), y, 150, 35)
                logo.Dispose()
            Catch
            End Try
            y += 30 + Jarak
        End If
        TulisTengah(g, NAMA_PERUSAHAAN, FJudul, y) : y += 20 + Jarak
        TulisTengah(g, ALAMAT_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        TulisTengah(g, KOTA_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        TulisTengah(g, KONTAK_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        Return y
    End Function

    Private Function CetakInfoTransaksi(g As Graphics, y As Integer) As Integer
        Dim posNilai As Integer = PosNilaiKanan
        y += 10 + Jarak
        TulisTengah(g, "NOTA TRANSFER ANTAR CABANG", New Font(_cfg.FontJudul, _cfg.UkuranJudul, FontStyle.Bold), y)
        y += 20 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y) : y += 5 + Jarak

        Tulis(g, "No Trans", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_IdTransfer, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Tanggal", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Dari", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_DariCabang, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Ke", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_KeCabang, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Mode", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_ModeKirim, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Status", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_StatusTransfer, FKet, posNilai, y) : y += 10 + Jarak
        If Not String.IsNullOrEmpty(TC_Keterangan) Then
            Tulis(g, "Keterangan", FKet, BatasKiri, y)
            Tulis(g, ": " & TC_Keterangan, FKet, posNilai, y) : y += 10 + Jarak
        End If
        Tulis(g, "User", FKet, BatasKiri, y)
        Tulis(g, ": " & TC_IdUser, FKet, posNilai, y) : y += 14 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    Private Function CetakItem(g As Graphics, y As Integer) As Integer
        Dim m1 As Integer = BatasKiri + CInt(LebarPx * 0.11)
        Dim m2 As Integer = BatasKiri + CInt(LebarPx * 0.11)
        Dim m3 As Integer = BatasKiri + CInt(LebarPx * 0.65)
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)

        y += 10 + Jarak
        Tulis(g, "Nama Barang", FIsi, BatasKiri, y)
        TulisKanan(g, "Harga", FIsi, m3, y)
        TulisKanan(g, "Total", FIsi, m5, y) : y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)

        For Each item As ItemTransferCabang In TC_DaftarItem
            y += 14 + Jarak
            Tulis(g, item.NamaBarang, FIsi, BatasKiri, y) : y += 10 + Jarak
            TulisKanan(g, item.QtySatuan.ToString("#,0.##", cultureIndonesia), FIsi, m1, y)
            Tulis(g, item.Satuan, FIsi, m2, y)
            TulisKanan(g, Rp(item.Harga), FIsi, m3, y)
            TulisKanan(g, Rp(item.Total), FIsi, m5, y)
        Next

        y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    Private Function CetakTotal(g As Graphics, y As Integer) As Integer
        Dim m3 As Integer = BatasKiri + CInt(LebarPx * 0.51)
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)

        y += 5 + Jarak
        Tulis(g, TC_DaftarItem.Count & " item", FIsi, BatasKiri, y)
        TulisKanan(g, "Total Qty :", FIsi, m3, y)
        TulisKanan(g, TC_TotalQty.ToString("N0"), FIsi, m5, y) : y += 10 + Jarak
        TulisKanan(g, "Total Nilai :", FIsi, m3, y)
        TulisKanan(g, Rp(TC_TotalRupiah), FIsi, m5, y) : y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y) : y += 10 + Jarak
        g.DrawString("Terbilang: " & Terbilang(TC_TotalRupiah) & " Rupiah",
                     New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1), FontStyle.Italic),
                     Brushes.Gray, BatasKiri, y) : y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    Private Sub CetakFooter(g As Graphics, y As Integer)
        y += 10 + Jarak
        If ShowFooter1 Then
            For Each baris As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, baris, FFooter, y) : y += 12 + Jarak
            Next
        End If
        If ShowFooter2 Then
            For Each baris As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, baris, FFooter, y) : y += 12 + Jarak
            Next
        End If
        If ShowFooter3 Then
            For Each baris As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, baris, FFooter, y) : y += 12 + Jarak
            Next
        End If
    End Sub

#End Region

End Class
