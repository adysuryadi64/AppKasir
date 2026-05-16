Imports System.Drawing.Printing

Module ModuleCetakTransferStokInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("TransferStok")
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
        g.DrawString("BUKTI TRANSFER STOK", New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold), Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' Info
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.10)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.10)
        g.DrawString("No. Transfer", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & TS_IdTransfer, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & TS_IdUser, fIsi, Brushes.Black, xVal2, y) : y += lh
        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & TS_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y)
        g.DrawString("Jenis", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & TS_JenisTransfer, fIsi, Brushes.Black, xVal2, y) : y += lh
        If Not String.IsNullOrEmpty(TS_Uraian) Then
            g.DrawString("Uraian", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & TS_Uraian, fIsi, Brushes.Black, xVal1, y) : y += lh
        End If
        y += 4
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 6

        ' Tabel
        Dim xNamaMasuk As Integer = b.Left
        Dim xQtyMasuk As Integer = b.Left + CInt(b.Width * 0.22)
        Dim xHargaMasuk As Integer = b.Left + CInt(b.Width * 0.35)
        Dim xNamaKeluar As Integer = b.Left + CInt(b.Width * 0.50)
        Dim xQtyKeluar As Integer = b.Left + CInt(b.Width * 0.72)
        Dim xHargaKeluar As Integer = b.Left + CInt(b.Width * 0.85)
        Dim xSelisih As Integer = b.Right

        g.DrawString("Barang Masuk", fBold, Brushes.Black, xNamaMasuk, y)
        g.DrawString("Qty", fBold, Brushes.Black, xQtyMasuk, y, fmtKanan)
        g.DrawString("Harga", fBold, Brushes.Black, xHargaMasuk, y, fmtKanan)
        g.DrawString("Barang Keluar", fBold, Brushes.Black, xNamaKeluar, y)
        g.DrawString("Qty", fBold, Brushes.Black, xQtyKeluar, y, fmtKanan)
        g.DrawString("Harga", fBold, Brushes.Black, xHargaKeluar, y, fmtKanan)
        g.DrawString("Selisih", fBold, Brushes.Black, xSelisih, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        For Each item As ItemTransferStok In TS_DaftarItem
            g.DrawString(item.NamaBarangMasuk, fIsi, Brushes.Black, xNamaMasuk, y)
            g.DrawString(item.QtyMasuk.ToString("N2", cultureIndonesia), fIsi, Brushes.Black, xQtyMasuk, y, fmtKanan)
            g.DrawString(item.HargaMasuk.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xHargaMasuk, y, fmtKanan)
            g.DrawString(item.NamaBarangKeluar, fIsi, Brushes.Black, xNamaKeluar, y)
            g.DrawString(item.QtyKeluar.ToString("N2", cultureIndonesia), fIsi, Brushes.Black, xQtyKeluar, y, fmtKanan)
            g.DrawString(item.HargaKeluar.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xHargaKeluar, y, fmtKanan)
            g.DrawString(item.Selisih.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xSelisih, y, fmtKanan)
            y += lh
        Next
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4
        g.DrawString("Total Item : " & TS_DaftarItem.Count.ToString(), fKecil, Brushes.Gray, b.Left, y) : y += lh + 8

        ' Tanda tangan
        If _cfg.TampilTandaTangan Then
            Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
            Dim xT2 As Integer = b.Left + CInt(b.Width * 0.55)
            g.DrawString("Diserahkan Oleh", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("Diterima Oleh", fKecil, Brushes.Black, xT2, y, fmtTengah) : y += 35
            g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
            g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y) : y += 4
            g.DrawString("( " & TS_IdUser & " )", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("( .............. )", fKecil, Brushes.Black, xT2, y, fmtTengah) : y += lh + 8
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
