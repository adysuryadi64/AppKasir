Imports System.Drawing.Printing

Module ModuleCetakSuratJalanInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("SuratJalan")
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
        g.DrawString("SURAT JALAN PENGIRIMAN", New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold), Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' Info
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.1)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.1)
        g.DrawString("Nomor", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & SJ_Nota, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & SJ_IdUser, fIsi, Brushes.Black, xVal2, y) : y += lh
        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & SJ_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y)
        g.DrawString("Armada", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & SJ_Armada & " " & SJ_JenisArmada, fIsi, Brushes.Black, xVal2, y) : y += lh + 4
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 6

        ' Tabel header
        Dim xNo As Integer = b.Left
        Dim xNota As Integer = b.Left + CInt(b.Width * 0.04)
        Dim xNama As Integer = b.Left + CInt(b.Width * 0.16)
        Dim xAlamat As Integer = b.Left + CInt(b.Width * 0.36)
        Dim xJml As Integer = b.Left + CInt(b.Width * 0.62)
        Dim xLokasi As Integer = b.Left + CInt(b.Width * 0.75)
        Dim xTtd As Integer = b.Right

        g.DrawString("No", fBold, Brushes.Black, xNo, y)
        g.DrawString("Nota", fBold, Brushes.Black, xNota, y)
        g.DrawString("Pelanggan", fBold, Brushes.Black, xNama, y)
        g.DrawString("Alamat", fBold, Brushes.Black, xAlamat, y)
        g.DrawString("Jumlah", fBold, Brushes.Black, xJml, y, fmtKanan)
        g.DrawString("Lokasi", fBold, Brushes.Black, xLokasi, y)
        g.DrawString("TTD Penerima", fBold, Brushes.Black, xTtd, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        Dim no As Integer = 1
        For Each item As ItemSuratJalan In SJ_DaftarDetail
            g.DrawString(no & ".", fIsi, Brushes.Black, xNo, y)
            g.DrawString(item.NotaBelanja, fIsi, Brushes.Black, xNota, y)
            g.DrawString(item.NamaPelanggan, fIsi, Brushes.Black, xNama, y)
            g.DrawString(item.AlamatPelanggan, fIsi, Brushes.Black, xAlamat, y)
            g.DrawString(item.NilaiBelanja.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xJml, y, fmtKanan)
            g.DrawString(item.Lokasi, fIsi, Brushes.Black, xLokasi, y)
            g.DrawString(". . . . . . . . .", New Font("Courier New", 8), Brushes.Black, xTtd, y, fmtKanan)
            y += lh : no += 1
        Next
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        ' Total
        g.DrawString("Total :", fBold, Brushes.Black, xAlamat, y)
        g.DrawString(SJ_TotalRupiah.ToString("N0", cultureIndonesia), fBold, Brushes.Black, xJml, y, fmtKanan) : y += lh
        g.DrawLine(New Pen(Color.Black, 2), xAlamat, y, xJml, y) : y += 4
        g.DrawString("Terbilang: " & Terbilang(SJ_TotalRupiah) & " Rupiah",
                     New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1), FontStyle.Italic), Brushes.Gray, b.Left, y) : y += lh + 10

        ' Tanda tangan
        Dim xT1 As Integer = b.Left + CInt(b.Width * 0.05)
        Dim xT2 As Integer = b.Left + CInt(b.Width * 0.38)
        Dim xT3 As Integer = b.Left + CInt(b.Width * 0.70)
        g.DrawString("Sopir", fKecil, Brushes.Black, xT1, y, fmtTengah)
        g.DrawString("Helper 1", fKecil, Brushes.Black, xT2, y, fmtTengah)
        g.DrawString("Helper 2", fKecil, Brushes.Black, xT3, y, fmtTengah) : y += 35
        g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
        g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y)
        g.DrawLine(Pens.Black, xT3 - 30, y, xT3 + 30, y) : y += 4
        g.DrawString("( " & SJ_Supir & " )", fKecil, Brushes.Black, xT1, y, fmtTengah)
        g.DrawString("( " & SJ_Helper1 & " )", fKecil, Brushes.Black, xT2, y, fmtTengah)
        g.DrawString("( " & SJ_Helper2 & " )", fKecil, Brushes.Black, xT3, y, fmtTengah) : y += lh + 8

        g.DrawString("Dicetak : " & SJ_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"),
                     New Font(_cfg.FontIsi, 7), Brushes.Gray, b.Left, y)

        e.HasMorePages = False
    End Sub

End Module
