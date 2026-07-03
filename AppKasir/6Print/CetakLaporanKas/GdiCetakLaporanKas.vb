Imports System.Drawing.Printing

' ================================================================
' GdiCetakLaporanKas
' Cetak laporan mutasi kas/bank via GDI+.
' Data dibaca dari variabel LK_* di ModulePrinterLaporanKas.
' Mendukung thermal, dot matrix, dan sumber render PDF.
' ================================================================
Public Class GdiCetakLaporanKas

    Private ReadOnly _cfg As KonfigurasiThermal
    Private _panjangKertas As Integer
    Private WithEvents _pd As New PrintDocument

    ' Data tambahan yang diisi dari FormLapSaldoKas sebelum cetak
    Public LK_Rekening As String = ""
    Public LK_TypeAkun As String = ""
    Public LK_Kasir As String = ""
    Public LK_PeriodeLabel As String = ""

    ' Ringkasan per jenis transaksi (diisi dari form)
    Public LK_TotalPembelian As Decimal
    Public LK_NotaPembelian As Integer
    Public LK_TotalPenjualan As Decimal
    Public LK_NotaPenjualan As Integer
    Public LK_TotalReturBeli As Decimal
    Public LK_NotaReturBeli As Integer
    Public LK_TotalReturJual As Decimal
    Public LK_NotaReturJual As Integer
    Public LK_TotalBayarHutang As Decimal
    Public LK_NotaBayarHutang As Integer
    Public LK_TotalBayarPiutang As Decimal
    Public LK_NotaBayarPiutang As Integer
    Public LK_TotalPemasukan As Decimal
    Public LK_NotaPemasukan As Integer
    Public LK_TotalPengeluaran As Decimal
    Public LK_NotaPengeluaran As Integer
    Public LK_TotalBiaya As Decimal
    Public LK_NotaBiaya As Integer
    Public LK_TotalPRDebet As Decimal
    Public LK_NotaPRDebet As Integer
    Public LK_TotalPRKredit As Decimal
    Public LK_NotaPRKredit As Integer
    Public LK_SetorBos As Decimal

    ' Jenis transaksi baru (Fase 9)
    Public LK_TotalBon As Decimal
    Public LK_NotaBon As Integer
    Public LK_TotalBayarBon As Decimal
    Public LK_NotaBayarBon As Integer
    Public LK_TotalGaji As Decimal
    Public LK_NotaGaji As Integer
    Public LK_TotalPinjamanSupplier As Decimal
    Public LK_NotaPinjamanSupplier As Integer
    Public LK_TotalPinjamanPelanggan As Decimal
    Public LK_NotaPinjamanPelanggan As Integer

    Public LK_SaldoAwal As Decimal
    Public LK_SaldoHariIni As Decimal
    Public LK_TotalHariIni As Decimal
    Public LK_SaldoDilaci As Decimal
    Public LK_SaldoAkhir As Decimal
    Public LK_Pemilik As String = ""

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

    Public Sub New()
        _cfg = New KonfigurasiThermal("LaporanKas")
    End Sub

    Private Function Rp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
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
            Return BK + CInt(LebarPx * 0.28)
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
    Private ReadOnly Property FBold As Font
        Get
            Return New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold)
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
            HitungPanjang() : _pd.PrinterSettings.PrinterName = _cfg.NamaPrinter : _pd.Print()
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir("LaporanKas")
    End Sub

    Public Sub CetakDotMatrix()
        Dim cfgDot As New KonfigurasiDotMatrix("LaporanKas")
        If String.IsNullOrEmpty(cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To cfgDot.JumlahCetak
            HitungPanjang() : _pd.PrinterSettings.PrinterName = cfgDot.NamaPrinter : _pd.Print()
        Next
    End Sub

    Public Sub TampilkanPreview()
        HitungPanjang()
        Dim ppd As New PrintPreviewDialog() With {.Document = _pd, .WindowState = FormWindowState.Maximized}
        ppd.ShowDialog()
    End Sub

    Public Sub RenderToBitmaps(bitmaps As List(Of System.Drawing.Bitmap))
        HitungPanjang()
        Dim bmp As New System.Drawing.Bitmap(LebarPx, _panjangKertas)
        bmp.SetResolution(100, 100)
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            g.Clear(System.Drawing.Color.White)
            Dim e As New PrintPageEventArgs(g,
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                _pd.DefaultPageSettings)
            CetakHalaman(e)
        End Using
        bitmaps.Add(bmp)
    End Sub

    Private Sub HitungPanjang()
        ' Hitung baris transaksi yang tidak nol
        Dim barisTerisi As Integer = 0
        If LK_TotalPembelian <> 0 Then barisTerisi += 1
        If LK_TotalPenjualan <> 0 Then barisTerisi += 1
        If LK_TotalReturBeli <> 0 Then barisTerisi += 1
        If LK_TotalReturJual <> 0 Then barisTerisi += 1
        If LK_TotalBayarHutang <> 0 Then barisTerisi += 1
        If LK_TotalBayarPiutang <> 0 Then barisTerisi += 1
        If LK_TotalPemasukan <> 0 Then barisTerisi += 1
        If LK_TotalPengeluaran <> 0 Then barisTerisi += 1
        If LK_TotalBiaya <> 0 Then barisTerisi += 1
        If LK_TotalPRDebet <> 0 Then barisTerisi += 1
        If LK_TotalPRKredit <> 0 Then barisTerisi += 1
        If LK_TotalBon <> 0 Then barisTerisi += 1
        If LK_TotalBayarBon <> 0 Then barisTerisi += 1
        If LK_TotalGaji <> 0 Then barisTerisi += 1
        If LK_TotalPinjamanSupplier <> 0 Then barisTerisi += 1
        If LK_TotalPinjamanPelanggan <> 0 Then barisTerisi += 1

        ' Tinggi per baris dinamis berdasarkan ukuran font + jarak
        Dim tinggiBaris As Integer = _cfg.UkuranIsi + 4 + _cfg.JarakBaris

        ' Header: nama toko (4 baris) + judul + garis + info filter (3 baris) + header kolom = ~22 baris minimal
        ' Ringkasan: 5-7 baris saldo + tanda tangan + footer = ~200px
        Dim headerPx As Integer = 22 * (tinggiBaris + 2)  ' lebih longgar
        Dim ringkasanPx As Integer = 200
        Dim footerPx As Integer = 0
        If ShowF1 Then footerPx += 16
        If ShowF2 Then footerPx += 16
        If ShowF3 Then footerPx += 16

        _panjangKertas = headerPx + (barisTerisi * (tinggiBaris + 2)) + ringkasanPx + footerPx + 60
    End Sub

    Private Sub Pd_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd.BeginPrint
        _pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        _pd.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub Pd_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakHalaman(e)
    End Sub

    Private Sub CetakHalaman(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim fmtTengah As New StringFormat() With {.Alignment = StringAlignment.Center}
        Dim fmtKanan As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim y As Integer = 5

        ' Header toko
        g.DrawString(NAMA_PERUSAHAAN, FJudul, Brushes.Black, Tengah, y, fmtTengah) : y += 20 + Jarak
        g.DrawString(ALAMAT_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 10 + Jarak
        g.DrawString(KOTA_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 10 + Jarak
        g.DrawString(KONTAK_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 15 + Jarak

        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak
        g.DrawString("LAPORAN MUTASI KEUANGAN", FBold, Brushes.Black, Tengah, y, fmtTengah) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak

        ' Info filter
        Dim pv As Integer = PosNilai
        g.DrawString("Rekening", FKet, Brushes.Black, BK, y) : g.DrawString(": " & LK_Rekening, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Periode", FKet, Brushes.Black, BK, y) : g.DrawString(": " & LK_PeriodeLabel, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Kasir", FKet, Brushes.Black, BK, y) : g.DrawString(": " & LK_Kasir, FKet, Brushes.Black, pv, y) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak

        ' Header kolom
        ' m3 = posisi kanan kolom Nota (~45% dari kiri — beri ruang cukup untuk nama transaksi panjang)
        ' m4 = posisi kanan label ringkasan saldo
        ' m5 = posisi kanan nilai (hampir penuh lebar kertas)
        Dim m3 As Integer = BK + CInt(LebarPx * 0.45)
        Dim m4 As Integer = BK + CInt(LebarPx * 0.62)
        Dim m5 As Integer = BK + CInt(LebarPx * 0.97)

        g.DrawString("Transaksi", FIsi, Brushes.Black, BK, y)
        g.DrawString("Nota", FIsi, Brushes.Black, m3, y, fmtKanan)
        g.DrawString("Sub Total", FIsi, Brushes.Black, m5, y, fmtKanan) : y += 12 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak

        ' Baris transaksi — hanya tampil jika tidak nol
        ' Kolom nama dibatasi lebar sampai sebelum m3 agar tidak menabrak kolom Nota
        Dim lebarNama As Integer = m3 - BK - 4
        Dim TulisTransaksi = Sub(tanda As String, nama As String, nota As Integer, total As Decimal)
                                 y += 10 + Jarak
                                 ' Gambar nama dengan clipping agar tidak menabrak kolom Nota
                                 g.DrawString(tanda & " " & nama, FIsi, Brushes.Black,
                                     New RectangleF(BK, y, lebarNama, FIsi.GetHeight(g) + 2),
                                     StringFormat.GenericDefault)
                                 g.DrawString(nota.ToString("N0", cultureIndonesia), FIsi, Brushes.Black, m3, y, fmtKanan)
                                 g.DrawString(Rp(total), FIsi, Brushes.Black, m5, y, fmtKanan)
                             End Sub

        If LK_TotalPembelian <> 0 Then TulisTransaksi("(-)", "Pembelian", LK_NotaPembelian, LK_TotalPembelian)
        If LK_TotalPenjualan <> 0 Then TulisTransaksi("(+)", "Penjualan", LK_NotaPenjualan, LK_TotalPenjualan)
        If LK_TotalReturBeli <> 0 Then TulisTransaksi("(+)", "Retur Beli", LK_NotaReturBeli, LK_TotalReturBeli)
        If LK_TotalReturJual <> 0 Then TulisTransaksi("(-)", "Retur Jual", LK_NotaReturJual, LK_TotalReturJual)
        If LK_TotalBayarHutang <> 0 Then TulisTransaksi("(-)", "Bayar Hutang", LK_NotaBayarHutang, LK_TotalBayarHutang)
        If LK_TotalBayarPiutang <> 0 Then TulisTransaksi("(+)", "Bayar Piutang", LK_NotaBayarPiutang, LK_TotalBayarPiutang)
        If LK_TotalPemasukan <> 0 Then TulisTransaksi("(+)", "Jurnal Pemasukan", LK_NotaPemasukan, LK_TotalPemasukan)
        If LK_TotalPengeluaran <> 0 Then TulisTransaksi("(-)", "Jurnal Pengeluaran", LK_NotaPengeluaran, LK_TotalPengeluaran)
        If LK_TotalBiaya <> 0 Then TulisTransaksi("(-)", "Jurnal Biaya", LK_NotaBiaya, LK_TotalBiaya)
        If LK_TotalPRDebet <> 0 Then TulisTransaksi("(+)", "Pindah Rek (+)", LK_NotaPRDebet, LK_TotalPRDebet)
        If LK_TotalPRKredit <> 0 Then TulisTransaksi("(-)", "Pindah Rek (-)", LK_NotaPRKredit, LK_TotalPRKredit)
        If LK_TotalBon <> 0 Then TulisTransaksi("(-)", "Bon Karyawan", LK_NotaBon, LK_TotalBon)
        If LK_TotalBayarBon <> 0 Then TulisTransaksi("(+)", "Bayar Bon", LK_NotaBayarBon, LK_TotalBayarBon)
        If LK_TotalGaji <> 0 Then TulisTransaksi("(-)", "Gaji Karyawan", LK_NotaGaji, LK_TotalGaji)
        If LK_TotalPinjamanSupplier <> 0 Then TulisTransaksi("(+)", "Pinjaman Supplier", LK_NotaPinjamanSupplier, LK_TotalPinjamanSupplier)
        If LK_TotalPinjamanPelanggan <> 0 Then TulisTransaksi("(-)", "Pinjaman Pelanggan", LK_NotaPinjamanPelanggan, LK_TotalPinjamanPelanggan)

        y += 10 + Jarak
        g.DrawString(BuatGarisGanda(HitungLebarGaris(_cfg.LebarKertas)), FGaris, Brushes.Black, m3, y) : y += 10 + Jarak

        ' Ringkasan saldo — label rata kanan di m4, nilai rata kanan di m5
        ' Gunakan RectangleF agar label panjang tidak terpotong
        Dim lebarLbl As Integer = m4 - BK
        Dim TulisRingkasan = Sub(lbl As String, val As Decimal, bold As Boolean)
                                 Dim fnt As Font = If(bold, FBold, FIsi)
                                 g.DrawString(lbl, fnt, Brushes.Black,
                                     New RectangleF(BK, y, lebarLbl, fnt.GetHeight(g) + 2),
                                     New StringFormat() With {.Alignment = StringAlignment.Far})
                                 g.DrawString(Rp(val), fnt, Brushes.Black, m5, y, fmtKanan)
                                 y += 10 + Jarak
                             End Sub

        TulisRingkasan("Saldo Hari ini :", LK_SaldoHariIni, False)

        If LK_TypeAkun.ToUpper() = "KAS" Then
            TulisRingkasan("Uang di setor  :", LK_SetorBos, False)
            TulisRingkasan("Uang di laci   :", LK_SaldoDilaci, False)
        End If

        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak
        TulisRingkasan("Saldo Awal     :", LK_SaldoAwal, False)
        TulisRingkasan("Hari ini       :", LK_TotalHariIni, False)
        TulisRingkasan("Saldo Akhir    :", LK_SaldoAkhir, True)
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak

        ' Tanggal cetak + tanda tangan
        g.DrawString(KOTA_PERUSAHAAN & ", " & Now.ToString("dd-MM-yyyy"), FKet, Brushes.Black, m5, y, fmtKanan) : y += 10 + Jarak
        g.DrawString("ACC", FFooter, Brushes.Black, BK, y) : y += 30 + Jarak
        g.DrawString(If(String.IsNullOrEmpty(LK_Pemilik), NAMA_PERUSAHAAN, LK_Pemilik), FFooter, Brushes.Black, BK, y) : y += 10 + Jarak

        If ShowF1 Then
            For Each b As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                g.DrawString(b, FFooter, Brushes.Gray, Tengah, y, fmtTengah) : y += 10 + Jarak
            Next
        End If
        If ShowF2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                g.DrawString(b, FFooter, Brushes.Gray, Tengah, y, fmtTengah) : y += 10 + Jarak
            Next
        End If
        If ShowF3 Then
            For Each b As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                g.DrawString(b, FFooter, Brushes.Gray, Tengah, y, fmtTengah) : y += 10 + Jarak
            Next
        End If

        e.HasMorePages = False
    End Sub

End Class
