Imports System.Drawing.Printing

Module ModuleCetakBayarHutangInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("BayarHutang")
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer Inkjet/Laser belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        _pd = New PrintDocument()
        _pd.PrinterSettings.PrinterName = _cfg.NamaPrinter
        _pd.DefaultPageSettings.Landscape = (_cfg.Orientasi = "Landscape")
        For Each ps As PaperSize In _pd.PrinterSettings.PaperSizes
            Dim nama As String = _cfg.UkuranKertas.ToLower()
            If (nama.Contains("a4") AndAlso ps.Kind = PaperKind.A4) OrElse
               (nama.Contains("folio") AndAlso ps.Kind = PaperKind.Folio) OrElse
               (nama.Contains("letter") AndAlso ps.Kind = PaperKind.Letter) OrElse
               (nama.Contains("a5") AndAlso ps.Kind = PaperKind.A5) Then
                _pd.DefaultPageSettings.PaperSize = ps : Exit For
            End If
        Next
        Dim mm As Func(Of Integer, Integer) = Function(v) CInt(v * 3.937)
        _pd.DefaultPageSettings.Margins = New Margins(mm(_cfg.MarginKiri), mm(_cfg.MarginKanan), mm(_cfg.MarginAtas), mm(_cfg.MarginBawah))
        AddHandler _pd.PrintPage, AddressOf Pd_PrintPage
        For i As Integer = 1 To Math.Max(1, _cfg.JumlahCetak) : _pd.Print() : Next
    End Sub

    Private Sub Pd_PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim b As Rectangle = e.MarginBounds
        Dim fJudul As New Font(_cfg.FontJudul, _cfg.UkuranJudul, FontStyle.Bold)
        Dim fIsi As New Font(_cfg.FontIsi, _cfg.UkuranIsi)
        Dim fBold As New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold)
        Dim fKecil As New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1))
        Dim lh As Integer = CInt(fIsi.GetHeight(g)) + 2
        Dim fmtKanan As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim fmtTengah As New StringFormat() With {.Alignment = StringAlignment.Center}
        Dim tengah As Integer = b.Left + b.Width \ 2
        Dim xKanan As Integer = b.Right
        Dim y As Integer = b.Top

        ' Header
        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, tengah, y, fmtTengah) : y += CInt(fJudul.GetHeight(g)) + 2
        g.DrawString(ALAMAT_PERUSAHAAN & "  " & KOTA_PERUSAHAAN, fIsi, Brushes.Black, tengah, y, fmtTengah) : y += lh + 4
        g.DrawLine(New Pen(Color.Black, 2), b.Left, y, xKanan, y) : y += 6
        g.DrawString("BUKTI PEMBAYARAN HUTANG", New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold), Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' Info
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.12)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.12)
        g.DrawString("No. Bayar", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & BH_NoBayar, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & BH_IdUser, fIsi, Brushes.Black, xVal2, y) : y += lh
        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & BH_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y) : y += lh
        g.DrawString("Supplier", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & BH_NamaSupplier, fIsi, Brushes.Black, xVal1, y) : y += lh + 4
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 6

        ' Tabel
        Dim xNo As Integer = b.Left
        Dim xFaktur As Integer = b.Left + CInt(b.Width * 0.04)
        Dim xTgl As Integer = b.Left + CInt(b.Width * 0.28)
        Dim xTotal As Integer = b.Left + CInt(b.Width * 0.50)
        Dim xBayar As Integer = b.Left + CInt(b.Width * 0.68)
        Dim xSisa As Integer = b.Right

        g.DrawString("No", fBold, Brushes.Black, xNo, y)
        g.DrawString("No Faktur Beli", fBold, Brushes.Black, xFaktur, y)
        g.DrawString("Tgl Beli", fBold, Brushes.Black, xTgl, y)
        g.DrawString("Total Hutang", fBold, Brushes.Black, xTotal, y, fmtKanan)
        g.DrawString("Dibayar", fBold, Brushes.Black, xBayar, y, fmtKanan)
        g.DrawString("Sisa", fBold, Brushes.Black, xSisa, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        Dim no As Integer = 1
        For Each item As ItemBayarHutang In BH_DaftarDetail
            g.DrawString(no & ".", fIsi, Brushes.Black, xNo, y)
            g.DrawString(item.IdBeli, fIsi, Brushes.Black, xFaktur, y)
            g.DrawString(item.TanggalBeli.ToString("dd-MM-yyyy"), fIsi, Brushes.Black, xTgl, y)
            g.DrawString(item.TotalHutang.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xTotal, y, fmtKanan)
            g.DrawString(item.Pembayaran.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xBayar, y, fmtKanan)
            g.DrawString(item.Sisa.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xSisa, y, fmtKanan)
            y += lh : no += 1
        Next
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        ' Total
        Dim xLbl As Integer = b.Left + CInt(b.Width * 0.55)
        Dim fmtLbl As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim TulisLbl = Sub(lbl As String, fnt As Font, yy As Integer)
                           g.DrawString(lbl, fnt, Brushes.Black, New RectangleF(b.Left, yy, xLbl - b.Left, fnt.GetHeight(g) + 2), fmtLbl)
                       End Sub
        TulisLbl("Total Hutang :", fIsi, y)
        g.DrawString(BH_TotalHutang.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xSisa, y, fmtKanan) : y += lh
        TulisLbl("Dibayar      :", fIsi, y)
        g.DrawString(BH_NominalBayar.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xSisa, y, fmtKanan) : y += lh
        g.DrawLine(New Pen(Color.Black, 2), xLbl, y, xSisa, y) : y += 4
        TulisLbl("Sisa Hutang  :", fBold, y)
        g.DrawString(BH_SisaHutang.ToString("N0", cultureIndonesia), fBold, Brushes.Black, xSisa, y, fmtKanan) : y += lh + 8

        ' Tanda tangan
        If _cfg.TampilTandaTangan Then
            Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
            Dim xT2 As Integer = b.Left + CInt(b.Width * 0.6)
            g.DrawString("Hormat Kami", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("Kasir", fKecil, Brushes.Black, xT2, y, fmtTengah) : y += 35
            g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
            g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y) : y += 4
            g.DrawString("( .............. )", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("( " & BH_IdUser & " )", fKecil, Brushes.Black, xT2, y, fmtTengah) : y += lh + 8
        End If

        If _cfg.TampilFooter1 Then
            g.DrawString(FOOTER1, fKecil, Brushes.Gray, tengah, y, fmtTengah)
            y += lh
        End If
        If _cfg.TampilFooter2 Then
            g.DrawString(FOOTER2, fKecil, Brushes.Gray, tengah, y, fmtTengah)
            y += lh
        End If
        If _cfg.TampilFooter3 Then g.DrawString(FOOTER3, fKecil, Brushes.Gray, tengah, y, fmtTengah)
        e.HasMorePages = False
    End Sub

End Module
