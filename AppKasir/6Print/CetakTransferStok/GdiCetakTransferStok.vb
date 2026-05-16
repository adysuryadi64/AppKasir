Imports System.Drawing.Printing

Public Class GdiCetakTransferStok

    Private ReadOnly _cfgDot As KonfigurasiDotMatrix
    Private _panjangKertas As Integer
    Private _lebarKertas As Integer
    Private WithEvents _pd As New PrintDocument

    Public Sub New()
        _cfgDot = New KonfigurasiDotMatrix("TransferStok")
    End Sub

    Public Property TampilFooter1Override As Boolean? = Nothing
    Public Property TampilFooter2Override As Boolean? = Nothing
    Public Property TampilFooter3Override As Boolean? = Nothing

    Public Sub Cetak()
        If String.IsNullOrEmpty(_cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfgDot.JumlahCetak
            HitungUkuranKertas()
            _pd.PrinterSettings.PrinterName = _cfgDot.NamaPrinter
            _pd.Print()
        Next
    End Sub

    Public Sub TampilkanPreview()
        HitungUkuranKertas()
        Dim ppd As New PrintPreviewDialog() With {.Document = _pd, .WindowState = FormWindowState.Maximized}
        ppd.ShowDialog()
    End Sub

    Public Sub RenderToBitmaps(bitmaps As List(Of System.Drawing.Bitmap))
        HitungUkuranKertas()
        Dim bmp As New System.Drawing.Bitmap(_lebarKertas, _panjangKertas)
        bmp.SetResolution(100, 100)
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            g.Clear(System.Drawing.Color.White)
            Dim e As New PrintPageEventArgs(g,
                New System.Drawing.Rectangle(0, 0, _lebarKertas, _panjangKertas),
                New System.Drawing.Rectangle(0, 0, _lebarKertas, _panjangKertas),
                _pd.DefaultPageSettings)
            CetakHalaman(e)
        End Using
        bitmaps.Add(bmp)
    End Sub

    Private Sub HitungUkuranKertas()
        _panjangKertas = CInt((5 * 0.3937) * 72) + TS_DaftarItem.Count * 20 + 120
        _lebarKertas = CInt((_cfgDot.LebarKertas * 0.3937) * 72)
    End Sub

    Private Sub Pd_BeginPrint(s As Object, e As PrintEventArgs) Handles _pd.BeginPrint
        _pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", _lebarKertas, _panjangKertas)
        _pd.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub Pd_PrintPage(s As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakHalaman(e)
    End Sub

    Private Sub CetakHalaman(e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim lebar As Integer = _lebarKertas
        Dim bk As Integer = 2 + _cfgDot.BatasKiri
        Dim jarak As Integer = _cfgDot.JarakBaris
        Dim fJudul As New Font("Consolas", 12, FontStyle.Bold)
        Dim fIsi As New Font("Consolas", 9)
        Dim fKecil As New Font("Arial Narrow", 8, FontStyle.Italic)
        Dim fmtKanan As New StringFormat() With {.Alignment = StringAlignment.Far}

        Dim lebarKarakter As Double = 7.35
        Dim jumlahKarakter As Integer = CInt(Math.Floor(lebar / lebarKarakter))
        Dim garis As String = BuatGaris(jumlahKarakter)

        Dim mTengah As Integer = bk + CInt(lebar * 0.5)
        Dim m1 As Integer = bk + CInt(lebar * 0.22)
        Dim m2 As Integer = bk + CInt(lebar * 0.30)
        Dim m3 As Integer = bk + CInt(lebar * 0.44)
        Dim m4 As Integer = bk + CInt(lebar * 0.66)
        Dim m5 As Integer = bk + CInt(lebar * 0.74)
        Dim m6 As Integer = bk + CInt(lebar * 0.88)
        Dim m7 As Integer = bk + CInt(lebar * 0.97)

        Dim y As Integer = 10
        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, bk, y)
        g.DrawString("BUKTI TRANSFER STOK", fJudul, Brushes.Black, mTengah, y) : y += 20 + jarak

        g.DrawString("Tgl    : " & TS_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), fIsi, Brushes.Black, bk, y)
        g.DrawString("ID     : " & TS_IdTransfer, fIsi, Brushes.Black, mTengah, y) : y += 10 + jarak

        g.DrawString("Kasir  : " & TS_IdUser, fIsi, Brushes.Black, bk, y)
        g.DrawString("Jenis  : " & TS_JenisTransfer, fIsi, Brushes.Black, mTengah, y) : y += 10 + jarak

        If Not String.IsNullOrEmpty(TS_Uraian) Then
            g.DrawString("Uraian : " & TS_Uraian, fIsi, Brushes.Black, bk, y) : y += 10 + jarak
        End If

        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 5 + jarak

        g.DrawString("Barang Masuk", fIsi, Brushes.Black, bk, y)
        g.DrawString("Qty", fIsi, Brushes.Black, m1, y, fmtKanan)
        g.DrawString("Harga", fIsi, Brushes.Black, m3, y, fmtKanan)
        g.DrawString("Barang Keluar", fIsi, Brushes.Black, m3 + 4, y)
        g.DrawString("Qty", fIsi, Brushes.Black, m5, y, fmtKanan)
        g.DrawString("Harga", fIsi, Brushes.Black, m6, y, fmtKanan)
        g.DrawString("Selisih", fIsi, Brushes.Black, m7, y, fmtKanan) : y += 5 + jarak

        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 5 + jarak

        For Each item As ItemTransferStok In TS_DaftarItem
            g.DrawString(item.NamaBarangMasuk, fIsi, Brushes.Black, bk, y)
            g.DrawString(TSRp(item.QtyMasuk), fIsi, Brushes.Black, m1, y, fmtKanan)
            g.DrawString(TSRp(item.HargaMasuk), fIsi, Brushes.Black, m3, y, fmtKanan)
            g.DrawString(item.NamaBarangKeluar, fIsi, Brushes.Black, m3 + 4, y)
            g.DrawString(TSRp(item.QtyKeluar), fIsi, Brushes.Black, m5, y, fmtKanan)
            g.DrawString(TSRp(item.HargaKeluar), fIsi, Brushes.Black, m6, y, fmtKanan)
            g.DrawString(TSRp(item.Selisih), fIsi, Brushes.Black, m7, y, fmtKanan)
            y += 10 + jarak
        Next

        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 5 + jarak
        g.DrawString("Dicetak : " & TS_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"),
                     New Font("Arial Narrow", 7), Brushes.Gray, bk, y)

        e.HasMorePages = False
    End Sub

End Class
