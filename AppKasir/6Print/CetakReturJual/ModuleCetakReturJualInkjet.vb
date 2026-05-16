Imports System.Drawing.Printing

' ================================================================
' ModuleCetakReturJualInkjet
' Cetak nota retur penjualan ke printer Inkjet/Laser via GDI+.
'
' Cara pakai:
'   ModuleCetakReturJualInkjet.CetakNota()
' ================================================================
Module ModuleCetakReturJualInkjet

#Region "State"
    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    Private _xNo As Integer
    Private _xNama As Integer
    Private _xQty As Integer
    Private _xHarga As Integer
    Private _xDiskon As Integer
    Private _xJml As Integer
    Private _xKanan As Integer

    Private _tampilDiskon As Boolean
    Private _tampilTtd As Boolean

    Private _xTtd1 As Integer
    Private _xTtd2 As Integer
    Private _xTtd3 As Integer
#End Region

#Region "Entry Point"
    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("ReturJual")

        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer Inkjet/Laser belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
                _pd.DefaultPageSettings.PaperSize = ps
                Exit For
            End If
        Next

        Dim mm As Func(Of Integer, Integer) = Function(v) CInt(v * 3.937)
        _pd.DefaultPageSettings.Margins = New Margins(
            mm(_cfg.MarginKiri), mm(_cfg.MarginKanan),
            mm(_cfg.MarginAtas), mm(_cfg.MarginBawah))

        ' Flag model nota — ModelNota hanya urusan diskon, tanda tangan dari checkbox
        _tampilDiskon = (_cfg.ModelNota <> "Model 2 Tanpa Diskon")
        _tampilTtd = _cfg.TampilTandaTangan

        AddHandler _pd.PrintPage, AddressOf Pd_PrintPage

        For i As Integer = 1 To Math.Max(1, _cfg.JumlahCetak)
            _pd.Print()
        Next
    End Sub
#End Region

#Region "Print Page"
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
        Dim y As Integer = b.Top

        HitungKolom(b)

        y = CetakHeader(g, b, fJudul, fIsi, fmtTengah, tengah, y, lh)
        y = CetakInfoTransaksi(g, b, fIsi, fBold, tengah, y, lh)
        y = CetakTabelItem(g, b, fIsi, fBold, fKecil, fmtKanan, y, lh)
        y = CetakTotal(g, b, fIsi, fBold, fmtKanan, y, lh)
        If _tampilTtd Then y = CetakTandaTangan(g, b, fKecil, fmtTengah, y, lh)
        CetakFooter(g, fKecil, fmtTengah, tengah, y, lh)

        e.HasMorePages = False
    End Sub
#End Region

#Region "Hitung Kolom"
    Private Sub HitungKolom(b As Rectangle)
        Dim w As Integer = b.Width
        Dim pNo As Integer = _cfg.PctKolomNo
        Dim pQty As Integer = _cfg.PctKolomQty
        Dim pHarga As Integer = _cfg.PctKolomHarga
        Dim pDiskon As Integer = If(_tampilDiskon, _cfg.PctKolomDiskon, 0)
        Dim pJml As Integer = pHarga
        Dim pNama As Integer = 100 - pNo - pQty - pHarga - pDiskon - pJml
        If pNama < 10 Then pNama = 10

        _xNo = b.Left
        _xNama = b.Left + CInt(w * pNo / 100.0)
        _xQty = b.Left + CInt(w * (pNo + pNama) / 100.0)
        _xHarga = b.Left + CInt(w * (pNo + pNama + pQty) / 100.0)
        _xDiskon = b.Left + CInt(w * (pNo + pNama + pQty + pHarga) / 100.0)
        _xJml = b.Right
        _xKanan = b.Right
    End Sub
#End Region

#Region "Header Toko"
    Private Function CetakHeader(g As Graphics, b As Rectangle,
                                  fJudul As Font, fIsi As Font,
                                  fmtTengah As StringFormat,
                                  tengah As Integer, y As Integer, lh As Integer) As Integer
        Dim yTeks As Integer = y
        Dim xTeks As Integer = b.Left
        Dim lebarTeks As Integer = b.Width

        If _cfg.TampilLogo Then
            Try
                Dim logoPath As String = IO.Path.Combine(Application.StartupPath, "logo.png")
                If Not IO.File.Exists(logoPath) Then
                    logoPath = IO.Path.Combine(Application.StartupPath, "logo.jpg")
                End If
                If IO.File.Exists(logoPath) Then
                    Using logo As Image = Image.FromFile(logoPath)
                        Dim logoH As Integer = 50
                        Dim logoW As Integer = CInt(logo.Width * (logoH / logo.Height))
                        g.DrawImage(logo, b.Left, yTeks, logoW, logoH)
                        xTeks = b.Left + logoW + 8
                        lebarTeks = b.Right - xTeks
                        y = Math.Max(y, yTeks + logoH + 4)
                    End Using
                End If
            Catch
            End Try
        End If

        Dim tengahTeks As Integer = xTeks + lebarTeks \ 2
        Dim yT As Integer = yTeks

        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, tengahTeks, yT, fmtTengah)
        yT += CInt(fJudul.GetHeight(g)) + 2
        g.DrawString(ALAMAT_PERUSAHAAN, fIsi, Brushes.Black, tengahTeks, yT, fmtTengah)
        yT += lh
        g.DrawString(KOTA_PERUSAHAAN & "  " & KONTAK_PERUSAHAAN, fIsi, Brushes.Black, tengahTeks, yT, fmtTengah)
        yT += lh + 4

        y = Math.Max(y, yT)

        g.DrawLine(New Pen(Color.Black, 2), b.Left, y, _xKanan, y) : y += 3
        g.DrawLine(New Pen(Color.Black, 1), b.Left, y, _xKanan, y) : y += 6

        g.DrawString("NOTA RETUR PENJUALAN",
                     New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold),
                     Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6
        Return y
    End Function
#End Region

#Region "Info Transaksi"
    Private Function CetakInfoTransaksi(g As Graphics, b As Rectangle,
                                         fIsi As Font, fBold As Font,
                                         tengah As Integer, y As Integer, lh As Integer) As Integer
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.12)
        Dim xKanan As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.12)

        g.DrawString("No. Retur", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & ReturJual_NoRetur, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan, y)
        g.DrawString(": " & ReturJual_IdUser & " / " & ReturJual_IdKomputer, fIsi, Brushes.Black, xVal2, y)
        y += lh

        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & ReturJual_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y)
        y += lh

        g.DrawString("Pelanggan", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & ReturJual_JenisPelanggan & " - " & ReturJual_NamaPelanggan,
                     fIsi, Brushes.Black, xVal1, y)
        y += lh + 4

        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 6
        Return y
    End Function
#End Region

#Region "Tabel Item"
    Private Function CetakTabelItem(g As Graphics, b As Rectangle,
                                     fIsi As Font, fBold As Font, fKecil As Font,
                                     fmtKanan As StringFormat,
                                     y As Integer, lh As Integer) As Integer
        g.DrawString("No", fBold, Brushes.Black, _xNo, y)
        g.DrawString("Nama Barang", fBold, Brushes.Black, _xNama, y)
        g.DrawString("Qty", fBold, Brushes.Black, _xQty, y, fmtKanan)
        g.DrawString("Harga", fBold, Brushes.Black, _xHarga, y, fmtKanan)
        If _tampilDiskon Then
            g.DrawString("Diskon", fBold, Brushes.Black, _xDiskon, y, fmtKanan)
        End If
        g.DrawString("Jumlah", fBold, Brushes.Black, _xJml, y, fmtKanan)
        y += lh
        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 4

        Dim nomor As Integer = 1
        For Each item As ItemNotaReturJual In ReturJual_DaftarItem
            g.DrawString(nomor & ".", fIsi, Brushes.Black, _xNo, y)
            g.DrawString(item.NamaBarang, fIsi, Brushes.Black, _xNama, y)
            g.DrawString(item.Qty.ToString("#,0.##", cultureIndonesia) & " " & item.Satuan,
                         fIsi, Brushes.Black, _xQty, y, fmtKanan)
            g.DrawString(item.Harga.ToString("N0", cultureIndonesia),
                         fIsi, Brushes.Black, _xHarga, y, fmtKanan)
            If _tampilDiskon Then
                g.DrawString(item.TotalDiskon.ToString("N0", cultureIndonesia),
                             fIsi, Brushes.Black, _xDiskon, y, fmtKanan)
            End If
            g.DrawString(item.TotalHarga.ToString("N0", cultureIndonesia),
                         fIsi, Brushes.Black, _xJml, y, fmtKanan)
            y += lh
            nomor += 1
        Next

        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 4
        Return y
    End Function
#End Region

#Region "Total"
    Private Function CetakTotal(g As Graphics, b As Rectangle,
                                 fIsi As Font, fBold As Font,
                                 fmtKanan As StringFormat,
                                 y As Integer, lh As Integer) As Integer
        Dim xLbl As Integer = b.Left + CInt(b.Width * 0.58)
        Dim xVal As Integer = _xKanan

        Dim fmtLbl As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim TulisLbl = Sub(lbl As String, fnt As Font, br As Brush, yy As Integer)
                           Dim r As New RectangleF(b.Left, yy, xLbl - b.Left, fnt.GetHeight(g) + 2)
                           g.DrawString(lbl, fnt, br, r, fmtLbl)
                       End Sub

        g.DrawString(ReturJual_DaftarItem.Count & " item", New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1)),
                     Brushes.Gray, b.Left, y)
        TulisLbl("Total Retur :", fBold, Brushes.Black, y)
        g.DrawString(ReturJual_Total.ToString("N0", cultureIndonesia), fBold, Brushes.Black, xVal, y, fmtKanan)
        y += lh

        g.DrawLine(New Pen(Color.Black, 2), xLbl, y, xVal, y) : y += 4

        ' Terbilang
        y += 4
        g.DrawLine(Pens.LightGray, b.Left, y, _xKanan, y) : y += 4
        g.DrawString("Terbilang: " & Terbilang(ReturJual_Total),
                     New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1)), Brushes.Gray, b.Left, y)
        y += lh + 8
        Return y
    End Function
#End Region

#Region "Tanda Tangan"
    Private Function CetakTandaTangan(g As Graphics, b As Rectangle,
                                       fKecil As Font, fmtTengah As StringFormat,
                                       y As Integer, lh As Integer) As Integer
        Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
        Dim xT2 As Integer = b.Left + CInt(b.Width * 0.45)
        Dim xT3 As Integer = b.Left + CInt(b.Width * 0.75)

        _xTtd1 = xT1 : _xTtd2 = xT2 : _xTtd3 = xT3

        g.DrawString("Hormat Kami", fKecil, Brushes.Black, xT1, y, fmtTengah)
        g.DrawString("Penerima", fKecil, Brushes.Black, xT2, y, fmtTengah)
        g.DrawString("Kasir", fKecil, Brushes.Black, xT3, y, fmtTengah)
        y += 40

        g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
        g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y)
        g.DrawLine(Pens.Black, xT3 - 30, y, xT3 + 30, y)
        y += 4

        g.DrawString("( .............. )", fKecil, Brushes.Black, xT1, y, fmtTengah)
        g.DrawString("( .............. )", fKecil, Brushes.Black, xT2, y, fmtTengah)
        g.DrawString("( " & ReturJual_IdUser & " )", fKecil, Brushes.Black, xT3, y, fmtTengah)
        y += lh + 10
        Return y
    End Function
#End Region

#Region "Footer"
    Private Sub CetakFooter(g As Graphics, fKecil As Font,
                             fmtTengah As StringFormat,
                             tengah As Integer, y As Integer, lh As Integer)
        If _tampilTtd Then
            If _cfg.TampilFooter1 Then g.DrawString(FOOTER1, fKecil, Brushes.Gray, _xTtd1, y, fmtTengah)
            If _cfg.TampilFooter2 Then g.DrawString(FOOTER2, fKecil, Brushes.Gray, _xTtd2, y, fmtTengah)
            If _cfg.TampilFooter3 Then g.DrawString(FOOTER3, fKecil, Brushes.Gray, _xTtd3, y, fmtTengah)
        Else
            If _cfg.TampilFooter1 Then
                g.DrawString(FOOTER1, fKecil, Brushes.Gray, tengah, y, fmtTengah) : y += lh
            End If
            If _cfg.TampilFooter2 Then
                g.DrawString(FOOTER2, fKecil, Brushes.Gray, tengah, y, fmtTengah) : y += lh
            End If
            If _cfg.TampilFooter3 Then
                g.DrawString(FOOTER3, fKecil, Brushes.Gray, tengah, y, fmtTengah)
            End If
        End If
    End Sub
#End Region

End Module
