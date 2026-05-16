Imports System.Drawing.Printing

' ================================================================
' ModuleCetakLaporanKasInkjet
' Cetak laporan mutasi kas ke printer Inkjet/Laser via GDI+.
' Data diambil dari instance GdiCetakLaporanKas yang aktif.
' ================================================================
Module ModuleCetakLaporanKasInkjet

    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument
    Private _cetak As GdiCetakLaporanKas = Nothing

    ' Dipakai oleh EscPosCetakLaporanKas untuk akses data
    Public Function GetCurrentCetak() As GdiCetakLaporanKas
        Return _cetak
    End Function

    Public Sub CetakNota(cetak As GdiCetakLaporanKas)
        _cetak = cetak
        _cfg = New KonfigurasiInkjet("LaporanKas")
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

    ' Overload tanpa parameter — pakai _cetak yang sudah diset
    Public Sub CetakNota()
        If _cetak Is Nothing Then
            MessageBox.Show("Data laporan belum diisi.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        CetakNota(_cetak)
    End Sub

    Private Sub Pd_PrintPage(sender As Object, e As PrintPageEventArgs)
        If _cetak Is Nothing Then
            e.HasMorePages = False
            Return
        End If

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

        ' Header toko
        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, tengah, y, fmtTengah) : y += CInt(fJudul.GetHeight(g)) + 2
        g.DrawString(ALAMAT_PERUSAHAAN & "  " & KOTA_PERUSAHAAN & "  " & KONTAK_PERUSAHAAN, fIsi, Brushes.Black, tengah, y, fmtTengah) : y += lh + 4
        g.DrawLine(New Pen(Color.Black, 2), b.Left, y, xKanan, y) : y += 3
        g.DrawLine(New Pen(Color.Black, 1), b.Left, y, xKanan, y) : y += 6
        g.DrawString("LAPORAN MUTASI KEUANGAN", New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold), Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6

        ' Info filter
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.10)
        Dim xKanan2 As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.10)
        g.DrawString("Rekening", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & _cetak.LK_Rekening, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan2, y) : g.DrawString(": " & _cetak.LK_Kasir, fIsi, Brushes.Black, xVal2, y) : y += lh
        g.DrawString("Periode", fIsi, Brushes.Black, xKiri, y) : g.DrawString(": " & _cetak.LK_PeriodeLabel, fIsi, Brushes.Black, xVal1, y) : y += lh + 4
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 6

        ' Header tabel
        Dim xNama As Integer = b.Left
        Dim xNota As Integer = b.Left + CInt(b.Width * 0.55)
        Dim xTotal As Integer = b.Right

        g.DrawString("Transaksi", fBold, Brushes.Black, xNama, y)
        g.DrawString("Nota", fBold, Brushes.Black, xNota, y, fmtKanan)
        g.DrawString("Sub Total", fBold, Brushes.Black, xTotal, y, fmtKanan) : y += lh
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        ' Baris transaksi
        Dim TulisBaris = Sub(tanda As String, nama As String, nota As Integer, total As Decimal)
                             If total = 0 Then Return
                             g.DrawString(tanda & " " & nama, fIsi, Brushes.Black, xNama, y)
                             g.DrawString(nota.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xNota, y, fmtKanan)
                             g.DrawString(total.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xTotal, y, fmtKanan)
                             y += lh
                         End Sub

        TulisBaris("(-)", "Pembelian", _cetak.LK_NotaPembelian, _cetak.LK_TotalPembelian)
        TulisBaris("(+)", "Penjualan", _cetak.LK_NotaPenjualan, _cetak.LK_TotalPenjualan)
        TulisBaris("(+)", "Retur Beli", _cetak.LK_NotaReturBeli, _cetak.LK_TotalReturBeli)
        TulisBaris("(-)", "Retur Jual", _cetak.LK_NotaReturJual, _cetak.LK_TotalReturJual)
        TulisBaris("(-)", "Bayar Hutang", _cetak.LK_NotaBayarHutang, _cetak.LK_TotalBayarHutang)
        TulisBaris("(+)", "Bayar Piutang", _cetak.LK_NotaBayarPiutang, _cetak.LK_TotalBayarPiutang)
        TulisBaris("(+)", "Jurnal Pemasukan", _cetak.LK_NotaPemasukan, _cetak.LK_TotalPemasukan)
        TulisBaris("(-)", "Jurnal Pengeluaran", _cetak.LK_NotaPengeluaran, _cetak.LK_TotalPengeluaran)
        TulisBaris("(-)", "Jurnal Biaya", _cetak.LK_NotaBiaya, _cetak.LK_TotalBiaya)
        TulisBaris("(+)", "Pindah Rek (+)", _cetak.LK_NotaPRDebet, _cetak.LK_TotalPRDebet)
        TulisBaris("(-)", "Pindah Rek (-)", _cetak.LK_NotaPRKredit, _cetak.LK_TotalPRKredit)
        TulisBaris("(-)", "Bon Karyawan", _cetak.LK_NotaBon, _cetak.LK_TotalBon)
        TulisBaris("(+)", "Bayar Bon", _cetak.LK_NotaBayarBon, _cetak.LK_TotalBayarBon)
        TulisBaris("(-)", "Gaji Karyawan", _cetak.LK_NotaGaji, _cetak.LK_TotalGaji)
        TulisBaris("(+)", "Pinjaman Supplier", _cetak.LK_NotaPinjamanSupplier, _cetak.LK_TotalPinjamanSupplier)
        TulisBaris("(-)", "Pinjaman Pelanggan", _cetak.LK_NotaPinjamanPelanggan, _cetak.LK_TotalPinjamanPelanggan)

        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4

        ' Ringkasan saldo
        Dim xLbl As Integer = b.Left + CInt(b.Width * 0.55)
        Dim fmtLbl As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim TulisLbl = Sub(lbl As String, val As Decimal, bold As Boolean)
                           Dim fnt As Font = If(bold, fBold, fIsi)
                           g.DrawString(lbl, fnt, Brushes.Black, New RectangleF(b.Left, y, xLbl - b.Left, fnt.GetHeight(g) + 2), fmtLbl)
                           g.DrawString(val.ToString("N0", cultureIndonesia), fnt, Brushes.Black, xTotal, y, fmtKanan)
                           y += lh
                       End Sub

        TulisLbl("Saldo Hari ini :", _cetak.LK_SaldoHariIni, False)
        If _cetak.LK_TypeAkun.ToUpper() = "KAS" Then
            TulisLbl("Uang di setor  :", _cetak.LK_SetorBos, False)
            TulisLbl("Uang di laci   :", _cetak.LK_SaldoDilaci, False)
        End If
        g.DrawLine(Pens.Black, b.Left, y, xKanan, y) : y += 4
        TulisLbl("Saldo Awal     :", _cetak.LK_SaldoAwal, False)
        TulisLbl("Hari ini       :", _cetak.LK_TotalHariIni, False)
        g.DrawLine(New Pen(Color.Black, 2), xLbl, y, xTotal, y) : y += 4
        TulisLbl("Saldo Akhir    :", _cetak.LK_SaldoAkhir, True)
        y += 8

        ' Tanggal + tanda tangan
        g.DrawString(KOTA_PERUSAHAAN & ", " & Now.ToString("dd-MM-yyyy"), fKecil, Brushes.Black, xTotal, y, fmtKanan) : y += lh + 4
        Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
        g.DrawString("ACC", fKecil, Brushes.Black, xT1, y, fmtTengah) : y += 35
        g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y) : y += 4
        g.DrawString("( " & If(String.IsNullOrEmpty(_cetak.LK_Pemilik), NAMA_PERUSAHAAN, _cetak.LK_Pemilik) & " )", fKecil, Brushes.Black, xT1, y, fmtTengah) : y += lh + 8

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
