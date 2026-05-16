Imports System.Drawing.Printing

Module ModuleCetakStokOpnameInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("StokOpname")
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
               (nama.Contains("letter") AndAlso ps.Kind = PaperKind.Letter) Then
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
        g.DrawString("LAPORAN STOK OPNAME", New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold), Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' Info
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.10)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.10)
        g.DrawString("ID Opname", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & SO_IdOpname, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & SO_IdUser, fIsi, Brushes.Black, xVal2, y) : y += lh
        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & SO_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y)
        g.DrawString("Lokasi", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & SO_Lokasi, fIsi, Brushes.Black, xVal2, y) : y += lh + 4
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 6

        ' Tabel
        Dim xNo As Integer = b.Left
        Dim xNama As Integer = b.Left + CInt(b.Width * 0.04)
        Dim xSat As Integer = b.Left + CInt(b.Width * 0.38)
        Dim xSys As Integer = b.Left + CInt(b.Width * 0.52)
        Dim xNyata As Integer = b.Left + CInt(b.Width * 0.66)
        Dim xSelisih As Integer = b.Left + CInt(b.Width * 0.80)
        Dim xTotal As Integer = b.Right

        g.DrawString("No", fBold, Brushes.Black, xNo, y)
        g.DrawString("Nama Barang", fBold, Brushes.Black, xNama, y)
        g.DrawString("Satuan", fBold, Brushes.Black, xSat, y)
        g.DrawString("Stok Sys", fBold, Brushes.Black, xSys, y, fmtKanan)
        g.DrawString("Stok Nyata", fBold, Brushes.Black, xNyata, y, fmtKanan)
        g.DrawString("Selisih", fBold, Brushes.Black, xSelisih, y, fmtKanan)
        g.DrawString("Total Harga", fBold, Brushes.Black, xTotal, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        Dim no As Integer = 1
        For Each item As ItemStokOpname In SO_DaftarItem
            g.DrawString(no & ".", fIsi, Brushes.Black, xNo, y)
            g.DrawString(item.NamaBarang, fIsi, Brushes.Black, xNama, y)
            g.DrawString(item.Satuan, fIsi, Brushes.Black, xSat, y)
            g.DrawString(item.StokSystem.ToString("N2", cultureIndonesia), fIsi, Brushes.Black, xSys, y, fmtKanan)
            g.DrawString(item.StokNyata.ToString("N2", cultureIndonesia), fIsi, Brushes.Black, xNyata, y, fmtKanan)
            g.DrawString(item.StokSelisih.ToString("N2", cultureIndonesia), fIsi, Brushes.Black, xSelisih, y, fmtKanan)
            g.DrawString(item.TotalHarga.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xTotal, y, fmtKanan)
            y += lh : no += 1
        Next
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4
        g.DrawString("Total Item : " & SO_DaftarItem.Count.ToString(), fKecil, Brushes.Gray, b.Left, y) : y += lh + 8

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
