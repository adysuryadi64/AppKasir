Imports System.Drawing.Printing

' ================================================================
' ModuleCetakReturBeliInkjet — Cetak nota retur pembelian Inkjet/Laser
' ================================================================
Module ModuleCetakReturBeliInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument
    Private _xNo, _xNama, _xQty, _xHarga, _xJml, _xKanan As Integer

    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("ReturBeli")
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
        For i As Integer = 1 To Math.Max(1, _cfg.JumlahCetak)
            _pd.Print()
        Next
    End Sub

    Private Sub HitungKolom(b As Rectangle)
        Dim w As Integer = b.Width
        Dim pNo As Integer = _cfg.PctKolomNo
        Dim pQty As Integer = _cfg.PctKolomQty
        Dim pHarga As Integer = _cfg.PctKolomHarga
        Dim pJml As Integer = pHarga
        Dim pNama As Integer = 100 - pNo - pQty - pHarga - pJml
        If pNama < 10 Then pNama = 10

        _xNo = b.Left
        _xNama = b.Left + CInt(w * pNo / 100.0)
        _xQty = b.Left + CInt(w * (pNo + pNama) / 100.0)
        _xHarga = b.Left + CInt(w * (pNo + pNama + pQty) / 100.0)
        _xJml = b.Right
        _xKanan = b.Right
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
        Dim y As Integer = b.Top

        HitungKolom(b)

        ' Header toko
        Dim yT As Integer = y
        Try
            Dim logoPath As String = IO.Path.Combine(Application.StartupPath, "logo.png")
            If Not IO.File.Exists(logoPath) Then logoPath = IO.Path.Combine(Application.StartupPath, "logo.jpg")
            If IO.File.Exists(logoPath) Then
                Using logo As Image = Image.FromFile(logoPath)
                    Dim logoH As Integer = 50
                    Dim logoW As Integer = CInt(logo.Width * (logoH / logo.Height))
                    g.DrawImage(logo, b.Left, yT, logoW, logoH)
                    y = Math.Max(y, yT + logoH + 4)
                End Using
            End If
        Catch : End Try

        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, tengah, yT, fmtTengah)
        yT += CInt(fJudul.GetHeight(g)) + 2
        g.DrawString(ALAMAT_PERUSAHAAN & "  " & KOTA_PERUSAHAAN, fIsi, Brushes.Black, tengah, yT, fmtTengah)
        yT += lh + 4
        y = Math.Max(y, yT)
        g.DrawLine(New Pen(Color.Black, 2), b.Left, y, _xKanan, y) : y += 6
        g.DrawString("NOTA RETUR PEMBELIAN", New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold), Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' Info transaksi
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.12)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.12)
        g.DrawString("No. Retur", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & ReturBeli_NoRetur, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan2, y)
        g.DrawString(": " & ReturBeli_IdUser, fIsi, Brushes.Black, xVal2, y) : y += lh
        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & ReturBeli_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y) : y += lh
        g.DrawString("Supplier", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & ReturBeli_NamaSupplier, fIsi, Brushes.Black, xVal1, y) : y += lh + 4
        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 6

        ' Header tabel
        g.DrawString("No", fBold, Brushes.Black, _xNo, y)
        g.DrawString("Nama Barang", fBold, Brushes.Black, _xNama, y)
        g.DrawString("Qty", fBold, Brushes.Black, _xQty, y, fmtKanan)
        g.DrawString("Harga", fBold, Brushes.Black, _xHarga, y, fmtKanan)
        g.DrawString("Jumlah", fBold, Brushes.Black, _xJml, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 4

        ' Item
        Dim nomor As Integer = 1
        For Each item As ItemNotaReturBeli In ReturBeli_DaftarItem
            g.DrawString(nomor & ".", fIsi, Brushes.Black, _xNo, y)
            g.DrawString(item.NamaBarang, fIsi, Brushes.Black, _xNama, y)
            g.DrawString(item.Qty.ToString("#,0.##", cultureIndonesia) & " " & item.Satuan, fIsi, Brushes.Black, _xQty, y, fmtKanan)
            g.DrawString(item.Harga.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, _xHarga, y, fmtKanan)
            g.DrawString(item.Total.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, _xJml, y, fmtKanan)
            y += lh : nomor += 1
        Next
        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 4

        ' Total
        Dim xLbl As Integer = b.Left + CInt(b.Width * 0.58)
        Dim fmtLbl As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim TulisLbl = Sub(lbl As String, fnt As Font, yy As Integer)
                           g.DrawString(lbl, fnt, Brushes.Black, New RectangleF(b.Left, yy, xLbl - b.Left, fnt.GetHeight(g) + 2), fmtLbl)
                       End Sub
        g.DrawString(ReturBeli_DaftarItem.Count & " item", fKecil, Brushes.Gray, b.Left, y)
        TulisLbl("Total Retur :", fBold, y)
        g.DrawString(ReturBeli_Total.ToString("N0", cultureIndonesia), fBold, Brushes.Black, _xJml, y, fmtKanan) : y += lh
        g.DrawLine(New Pen(Color.Black, 2), xLbl, y, _xJml, y) : y += 4
        y += 4
        g.DrawLine(Pens.LightGray, b.Left, y, _xKanan, y) : y += 4
        g.DrawString("Terbilang: " & Terbilang(ReturBeli_Total), fKecil, Brushes.Gray, b.Left, y) : y += lh + 8

        ' Tanda tangan
        If _cfg.TampilTandaTangan Then
            Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
            Dim xT2 As Integer = b.Left + CInt(b.Width * 0.45)
            Dim xT3 As Integer = b.Left + CInt(b.Width * 0.75)
            g.DrawString("Hormat Kami", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("Penerima", fKecil, Brushes.Black, xT2, y, fmtTengah)
            g.DrawString("Kasir", fKecil, Brushes.Black, xT3, y, fmtTengah) : y += 40
            g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
            g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y)
            g.DrawLine(Pens.Black, xT3 - 30, y, xT3 + 30, y) : y += 4
            g.DrawString("( .............. )", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("( .............. )", fKecil, Brushes.Black, xT2, y, fmtTengah)
            g.DrawString("( " & ReturBeli_IdUser & " )", fKecil, Brushes.Black, xT3, y, fmtTengah) : y += lh + 10
        End If

        ' Footer
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
