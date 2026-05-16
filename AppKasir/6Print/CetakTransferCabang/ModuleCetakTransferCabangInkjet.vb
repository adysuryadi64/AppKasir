Imports System.Drawing.Printing

' ================================================================
' ModuleCetakTransferCabangInkjet
' Cetak nota transfer antar cabang ke printer Inkjet/Laser via GDI+.
' Mengikuti pola ModuleCetakJualInkjet yang sudah verified.
' ================================================================
Module ModuleCetakTransferCabangInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("TransferCabang")
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
                _pd.DefaultPageSettings.PaperSize = ps : Exit For
            End If
        Next

        Dim mm As Func(Of Integer, Integer) = Function(v) CInt(v * 3.937)
        _pd.DefaultPageSettings.Margins = New Margins(
            mm(_cfg.MarginKiri), mm(_cfg.MarginKanan),
            mm(_cfg.MarginAtas), mm(_cfg.MarginBawah))

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

        ' ── Header ──────────────────────────────────────────────
        Dim yTeks As Integer = y
        Dim xTeks As Integer = b.Left
        Dim lebarTeks As Integer = b.Width

        If _cfg.TampilLogo Then
            Try
                Dim logoPath As String = IO.Path.Combine(Application.StartupPath, "logo.png")
                If Not IO.File.Exists(logoPath) Then logoPath = IO.Path.Combine(Application.StartupPath, "logo.jpg")
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
        g.DrawString(ALAMAT_PERUSAHAAN, fIsi, Brushes.Black, tengahTeks, yT, fmtTengah) : yT += lh
        g.DrawString(KOTA_PERUSAHAAN & "  " & KONTAK_PERUSAHAAN, fIsi, Brushes.Black, tengahTeks, yT, fmtTengah) : yT += lh + 4
        y = Math.Max(y, yT)

        g.DrawLine(New Pen(Color.Black, 2), b.Left, y, xKanan, y) : y += 3
        g.DrawLine(New Pen(Color.Black, 1), b.Left, y, xKanan, y) : y += 6
        g.DrawString("NOTA TRANSFER BARANG ANTAR CABANG",
                     New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold),
                     Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' ── Info Transaksi ──────────────────────────────────────
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.12)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.12)

        g.DrawString("No. Transfer", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & TC_IdTransfer, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Tanggal", fIsi, Brushes.Black, xKanan2, y)
        g.DrawString(": " & TC_Tanggal.ToString("dd-MM-yyyy HH:mm"), fIsi, Brushes.Black, xVal2, y) : y += lh

        g.DrawString("Dari Cabang", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & TC_DariCabang, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Ke Cabang", fIsi, Brushes.Black, xKanan2, y)
        g.DrawString(": " & TC_KeCabang, fBold, Brushes.Black, xVal2, y) : y += lh

        g.DrawString("Mode Kirim", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & TC_ModeKirim, fIsi, Brushes.Black, xVal1, y)
        g.DrawString("Status", fIsi, Brushes.Black, xKanan2, y)
        g.DrawString(": " & TC_StatusTransfer, fIsi, Brushes.Black, xVal2, y) : y += lh

        If Not String.IsNullOrEmpty(TC_Keterangan) Then
            g.DrawString("Keterangan", fIsi, Brushes.Black, xKiri, y)
            g.DrawString(": " & TC_Keterangan, fIsi, Brushes.Black, xVal1, y) : y += lh
        End If

        g.DrawString("User", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & TC_IdUser, fIsi, Brushes.Black, xVal1, y) : y += lh + 4
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 6

        ' ── Tabel Item ──────────────────────────────────────────
        Dim xNo As Integer = b.Left
        Dim xNama As Integer = b.Left + CInt(b.Width * 0.05)
        Dim xQty As Integer = b.Left + CInt(b.Width * 0.52)
        Dim xSat As Integer = b.Left + CInt(b.Width * 0.60)
        Dim xHarga As Integer = b.Left + CInt(b.Width * 0.75)
        Dim xJml As Integer = b.Right

        g.DrawString("No", fBold, Brushes.Black, xNo, y)
        g.DrawString("Nama Barang", fBold, Brushes.Black, xNama, y)
        g.DrawString("Qty", fBold, Brushes.Black, xQty, y, fmtKanan)
        g.DrawString("Satuan", fBold, Brushes.Black, xSat, y)
        g.DrawString("Harga", fBold, Brushes.Black, xHarga, y, fmtKanan)
        g.DrawString("Total", fBold, Brushes.Black, xJml, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        Dim no As Integer = 1
        For Each item As ItemTransferCabang In TC_DaftarItem
            g.DrawString(no & ".", fIsi, Brushes.Black, xNo, y)
            g.DrawString(item.NamaBarang, fIsi, Brushes.Black, xNama, y)
            g.DrawString(item.QtySatuan.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xQty, y, fmtKanan)
            g.DrawString(item.Satuan, fIsi, Brushes.Black, xSat, y)
            g.DrawString(item.Harga.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xHarga, y, fmtKanan)
            g.DrawString(item.Total.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xJml, y, fmtKanan)
            y += lh : no += 1
        Next
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        ' ── Total ───────────────────────────────────────────────
        Dim xLbl As Integer = b.Left + CInt(b.Width * 0.58)
        Dim fmtLbl As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim TulisLbl = Sub(lbl As String, fnt As Font, yy As Integer)
                           g.DrawString(lbl, fnt, Brushes.Black,
                                        New RectangleF(b.Left, yy, xLbl - b.Left, fnt.GetHeight(g) + 2), fmtLbl)
                       End Sub

        g.DrawString(TC_DaftarItem.Count & " item", fKecil, Brushes.Gray, b.Left, y)
        TulisLbl("Total Qty :", fBold, y)
        g.DrawString(TC_TotalQty.ToString("N0", cultureIndonesia), fBold, Brushes.Black, xJml, y, fmtKanan) : y += lh
        TulisLbl("Total Nilai :", fBold, y)
        g.DrawString(TC_TotalRupiah.ToString("N0", cultureIndonesia), fBold, Brushes.Black, xJml, y, fmtKanan) : y += lh
        g.DrawLine(New Pen(Color.Black, 2), xLbl, y, xJml, y) : y += 4
        g.DrawLine(Pens.LightGray, b.Left, y, xKanan, y) : y += 4
        g.DrawString("Terbilang: " & Terbilang(TC_TotalRupiah) & " Rupiah",
                     New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1), FontStyle.Italic),
                     Brushes.Gray, b.Left, y) : y += lh + 8

        ' ── Tanda Tangan ────────────────────────────────────────
        If _cfg.TampilTandaTangan Then
            Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
            Dim xT2 As Integer = b.Left + CInt(b.Width * 0.55)
            g.DrawString("Diserahkan Oleh", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("Diterima Oleh", fKecil, Brushes.Black, xT2, y, fmtTengah) : y += 35
            g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
            g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y) : y += 4
            g.DrawString("( " & TC_IdUser & " )", fKecil, Brushes.Black, xT1, y, fmtTengah)
            g.DrawString("( .............. )", fKecil, Brushes.Black, xT2, y, fmtTengah) : y += lh + 8
        End If

        ' ── Footer ──────────────────────────────────────────────
        If _cfg.TampilFooter1 Then g.DrawString(FOOTER1, fKecil, Brushes.Gray, tengah, y, fmtTengah) : y += lh
        If _cfg.TampilFooter2 Then g.DrawString(FOOTER2, fKecil, Brushes.Gray, tengah, y, fmtTengah) : y += lh
        If _cfg.TampilFooter3 Then g.DrawString(FOOTER3, fKecil, Brushes.Gray, tengah, y, fmtTengah)

        e.HasMorePages = False
    End Sub

End Module
