Imports System.Drawing.Printing

' ================================================================
' GdiCetakBeliThermalMatrik
' GDI+ untuk cetak nota pembelian — thermal, dot matrix, PDF.
' ================================================================
Public Class GdiCetakBeliThermalMatrik

    Private ReadOnly _cfg As KonfigurasiThermal
    Private _panjangKertas As Integer
    Private WithEvents _pd As New PrintDocument
    Private WithEvents _pd1 As New PrintDocument
    Private WithEvents _pd2 As New PrintDocument
    Private WithEvents _pd3 As New PrintDocument

    Public Sub New()
        _cfg = New KonfigurasiThermal("Beli")
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
        Return BeliRp(v)
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
            AturDanCetak(False)
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir("Beli")
    End Sub

    Public Sub CetakDotMatrix()
        Dim cfgDot As New KonfigurasiDotMatrix("Beli")
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
        Dim c As New GdiCetakBeliThermalMatrik()
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
                Case "Model 2 Tanpa Header"          : CetakModel2(e)
                Case "Model 3 Dengan Total Hutang"   : CetakModel4(e)
                Case Else                            : CetakModel1(e)
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
            Case "Model 2 Tanpa Header"        : Return _pd1
            Case "Model 3 Dengan Total Hutang" : Return _pd3
            Case Else                          : Return _pd
        End Select
    End Function

    Private Sub HitungPanjangKertas()
        _panjangKertas = Beli_DaftarItem.Count * 30 + 380
        Dim tpb As Integer = 12 + Jarak
        If ShowF1 Then _panjangKertas += FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tpb
        If ShowF2 Then _panjangKertas += FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tpb
        If ShowF3 Then _panjangKertas += FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tpb
    End Sub

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
    Private Sub Pd3_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd3.BeginPrint
        _pd3.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        _pd3.DefaultPageSettings.Landscape = False
    End Sub
    Private Sub Pd_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakModel1(e)
    End Sub
    Private Sub Pd1_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd1.PrintPage
        CetakModel2(e)
    End Sub
    Private Sub Pd2_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd2.PrintPage
        CetakModel3(e)
    End Sub
    Private Sub Pd3_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd3.PrintPage
        CetakModel4(e)
    End Sub

    Private Sub Tulis(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        g.DrawString(teks, fnt, Brushes.Black, x, y)
    End Sub
    Private Sub TulisKanan(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Far}
        g.DrawString(teks, fnt, Brushes.Black, New RectangleF(BK, y, x - BK, fnt.GetHeight(g) + 2), fmt)
    End Sub
    Private Sub TulisTengah(g As Graphics, teks As String, fnt As Font, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center}
        g.DrawString(teks, fnt, Brushes.Black, Tengah, y, fmt)
    End Sub

    Private Sub CetakModel1(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeader(g, 5)
        y = CetakInfo(g, y) : y = CetakItem(g, y) : y = CetakTotal(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub
    Private Sub CetakModel2(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeader(g, 5)
        y = CetakInfoSingkat(g, y) : y = CetakItem(g, y) : y = CetakTotal(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub
    Private Sub CetakModel3(e As PrintPageEventArgs)
        ' Alias lama — logo dikontrol via _cfg.TampilLogo
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeader(g, 5)
        y = CetakInfo(g, y) : y = CetakItem(g, y) : y = CetakTotal(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub
    Private Sub CetakModel4(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = CetakHeader(g, 5)
        y = CetakInfo(g, y) : y = CetakItem(g, y)
        y = CetakTotalDenganHutangSupplier(g, y) : CetakFooter(g, y)
        e.HasMorePages = False
    End Sub

    Private Function CetakHeader(g As Graphics, y As Integer) As Integer
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

    Private Function CetakInfo(g As Graphics, y As Integer, Optional singkat As Boolean = False) As Integer
        Dim pv As Integer = PosNilai
        y += 15 + Jarak
        Tulis(g, "No Beli", FKet, BK, y) : Tulis(g, ": " & Beli_IdPembelian, FKet, pv, y) : y += 10 + Jarak
        If Not singkat Then
            Tulis(g, "Nota", FKet, BK, y) : Tulis(g, ": " & Beli_NotaPembelian, FKet, pv, y) : y += 10 + Jarak
        End If
        Tulis(g, "Tanggal", FKet, BK, y) : Tulis(g, ": " & Beli_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), FKet, pv, y) : y += 10 + Jarak
        Tulis(g, "Kasir", FKet, BK, y) : Tulis(g, ": " & Beli_IdUser & " - " & Beli_IdKomputer, FKet, pv, y) : y += 10 + Jarak
        Tulis(g, "Supplier", FKet, BK, y) : Tulis(g, ": " & Beli_NamaSupplier, FKet, pv, y) : y += 10 + Jarak
        Tulis(g, "Lokasi", FKet, BK, y) : Tulis(g, ": " & Beli_Lokasi, FKet, pv, y) : y += 14 + Jarak
        Tulis(g, Garis, FGaris, BK, y) : y += 10 + Jarak
        TulisTengah(g, "NOTA PEMBELIAN", New Font(_cfg.FontKeterangan, _cfg.UkuranKeterangan + 2), y) : y += 14 + Jarak
        Tulis(g, Garis, FGaris, BK, y)
        Return y
    End Function

    Private Function CetakInfoSingkat(g As Graphics, y As Integer) As Integer
        Return CetakInfo(g, y, singkat:=True)
    End Function

    Private Function CetakItem(g As Graphics, y As Integer) As Integer
        Dim m1 As Integer = BK + CInt(LebarPx * 0.11)
        Dim m2 As Integer = BK + CInt(LebarPx * 0.11)
        Dim m3 As Integer = BK + CInt(LebarPx * 0.65)
        Dim m5 As Integer = BK + CInt(LebarPx * 0.95)

        y += 10 + Jarak
        Tulis(g, "Nama Barang", FIsi, BK, y)
        TulisKanan(g, "Harga", FIsi, m3, y)
        TulisKanan(g, "Jumlah", FIsi, m5, y) : y += 10 + Jarak
        Tulis(g, Garis, FGaris, BK, y)

        For Each item As ItemNotaBeli In Beli_DaftarItem
            y += 14 + Jarak
            Tulis(g, item.NamaBarang, FIsi, BK, y) : y += 10 + Jarak
            TulisKanan(g, item.Qty.ToString("#,0.##", cultureIndonesia), FIsi, m1, y)
            Tulis(g, item.Satuan, FIsi, m2, y)
            TulisKanan(g, Rp(item.HargaBeli), FIsi, m3, y)
            TulisKanan(g, Rp(item.Total), FIsi, m5, y)
        Next
        y += 10 + Jarak
        Tulis(g, Garis, FGaris, BK, y)
        Return y
    End Function

    Private Function CetakTotal(g As Graphics, y As Integer) As Integer
        Dim m3 As Integer = BK + CInt(LebarPx * 0.51)
        Dim m5 As Integer = BK + CInt(LebarPx * 0.95)
        y += 5 + Jarak
        Tulis(g, Beli_DaftarItem.Count & " item", FIsi, BK, y)
        TulisKanan(g, "Total    :", FIsi, m3, y)
        TulisKanan(g, Rp(Beli_Tagihan), FIsi, m5, y) : y += 10 + Jarak
        TulisKanan(g, "Tunai    :", FIsi, m3, y)
        TulisKanan(g, Rp(Beli_Pembayaran), FIsi, m5, y) : y += 10 + Jarak
        If Beli_NominalTransfer > 0 Then
            Dim lblTf As String = If(String.IsNullOrEmpty(Beli_NamaAkunTf), "Transfer", Beli_NamaAkunTf)
            TulisKanan(g, lblTf & "  :", FIsi, m3, y)
            TulisKanan(g, Rp(Beli_NominalTransfer), FIsi, m5, y) : y += 10 + Jarak
        End If
        Dim totalBayar As Decimal = Beli_Pembayaran + Beli_NominalTransfer
        If Beli_Tagihan > totalBayar Then
            TulisKanan(g, "Hutang   :", FIsi, m3, y)
            TulisKanan(g, Rp(Beli_NominalBayar), New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), m5, y) : y += 10 + Jarak
            If Beli_JatuhTempo > Date.MinValue Then
                Tulis(g, "Jth Tempo", FIsi, BK, y)
                Tulis(g, ": " & Beli_JatuhTempo.ToString("dd-MM-yyyy"), FIsi, BK + CInt(LebarPx * 0.25), y) : y += 10 + Jarak
            End If
        End If
        Tulis(g, Garis, FGaris, BK, y) : y += 5 + Jarak
        Tulis(g, "Status", FIsi, BK, y)
        Tulis(g, ": " & Beli_StatusTransaksi, New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), BK + CInt(LebarPx * 0.25), y) : y += 10 + Jarak
        Tulis(g, Garis, FGaris, BK, y)
        Return y
    End Function

    ' Model 4 — total + ringkasan hutang supplier dari tbl_supliyer
    Private Function CetakTotalDenganHutangSupplier(g As Graphics, y As Integer) As Integer
        Dim m3 As Integer = BK + CInt(LebarPx * 0.51)
        Dim m5 As Integer = BK + CInt(LebarPx * 0.95)
        y += 5 + Jarak
        Tulis(g, Beli_DaftarItem.Count & " item", FIsi, BK, y)
        TulisKanan(g, "Total    :", FIsi, m3, y)
        TulisKanan(g, Rp(Beli_Tagihan), FIsi, m5, y) : y += 10 + Jarak
        TulisKanan(g, "Tunai    :", FIsi, m3, y)
        TulisKanan(g, Rp(Beli_Pembayaran), FIsi, m5, y) : y += 10 + Jarak
        If Beli_NominalTransfer > 0 Then
            Dim lblTf As String = If(String.IsNullOrEmpty(Beli_NamaAkunTf), "Transfer", Beli_NamaAkunTf)
            TulisKanan(g, lblTf & "  :", FIsi, m3, y)
            TulisKanan(g, Rp(Beli_NominalTransfer), FIsi, m5, y) : y += 10 + Jarak
        End If
        If Beli_NominalBayar > 0 Then
            TulisKanan(g, "Hutang   :", FIsi, m3, y)
            TulisKanan(g, Rp(Beli_NominalBayar), New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), m5, y) : y += 10 + Jarak
            If Beli_JatuhTempo > Date.MinValue Then
                Tulis(g, "Jth Tempo", FIsi, BK, y)
                Tulis(g, ": " & Beli_JatuhTempo.ToString("dd-MM-yyyy"), FIsi, BK + CInt(LebarPx * 0.25), y) : y += 10 + Jarak
            End If
        End If
        Tulis(g, Garis, FGaris, BK, y) : y += 5 + Jarak
        Tulis(g, "Status", FIsi, BK, y)
        Tulis(g, ": " & Beli_StatusTransaksi, New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), BK + CInt(LebarPx * 0.25), y) : y += 10 + Jarak
        ' Sisa hutang supplier
        Tulis(g, Garis, FGaris, BK, y) : y += 5 + Jarak
        Tulis(g, "Sisa Hutang Supplier", New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), BK, y)
        TulisKanan(g, Rp(Beli_HutangAkhir), New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold), m5, y) : y += 10 + Jarak
        Tulis(g, Garis, FGaris, BK, y)
        Return y
    End Function

    Private Sub CetakFooter(g As Graphics, y As Integer)
        y += 10 + Jarak
        If ShowF1 Then
            For Each b As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, b, FFooter, y) : y += 10 + Jarak
            Next
        End If
        If ShowF2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, b, FFooter, y) : y += 10 + Jarak
            Next
        End If
        If ShowF3 Then
            For Each b As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, b, FFooter, y) : y += 10 + Jarak
            Next
        End If
    End Sub

End Class
