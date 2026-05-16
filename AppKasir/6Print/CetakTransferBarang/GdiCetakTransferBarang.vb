Imports System.Drawing.Printing

Public Class GdiCetakTransferBarang

    Private ReadOnly _cfgDot As KonfigurasiDotMatrix
    Private _panjangKertas As Integer
    Private _lebarKertas As Integer
    Private WithEvents _pd As New PrintDocument

    Public Sub New()
        _cfgDot = New KonfigurasiDotMatrix("TransferBarang")
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
        _panjangKertas = CInt((7 * 0.3937) * 72) + TB_DaftarItem.Count * 20 + 100
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

        Dim m1 As Integer = bk + CInt(lebar * 0.05)
        Dim m2 As Integer = bk + CInt(lebar * 0.17)
        Dim m3 As Integer = bk + CInt(lebar * 0.45)
        Dim m5 As Integer = bk + CInt(lebar * 0.55)
        Dim m6 As Integer = bk + CInt(lebar * 0.65)
        Dim m7 As Integer = bk + CInt(lebar * 0.90)

        Dim y As Integer = 10
        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, bk, y)
        g.DrawString(TB_KeteranganLokasi, fJudul, Brushes.Black, m3, y) : y += 20 + jarak

        g.DrawString("Tgl    : " & TB_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), fIsi, Brushes.Black, bk, y)
        g.DrawString("Nomor  : " & TB_IdTransfer, fIsi, Brushes.Black, m3, y) : y += 10 + jarak

        g.DrawString("Kasir  : " & TB_IdUser, fIsi, Brushes.Black, bk, y)
        g.DrawString("Lokasi : " & TB_Lokasi, fIsi, Brushes.Black, m3, y) : y += 14 + jarak

        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 5 + jarak

        g.DrawString("No", fIsi, Brushes.Black, bk, y)
        g.DrawString("Kode", fIsi, Brushes.Black, m1, y)
        g.DrawString("Nama Barang", fIsi, Brushes.Black, m2, y)
        g.DrawString("Harga", fIsi, Brushes.Black, m5, y, fmtKanan)
        g.DrawString("Qty", fIsi, Brushes.Black, m6, y, fmtKanan)
        g.DrawString("Satuan", fIsi, Brushes.Black, m6, y)
        g.DrawString("Jumlah", fIsi, Brushes.Black, m7, y, fmtKanan) : y += 5 + jarak

        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 5 + jarak

        Dim no As Integer = 1
        For Each item As ItemTransferBarang In TB_DaftarItem
            g.DrawString(no & ".", fIsi, Brushes.Black, m1, y, fmtKanan)
            g.DrawString(item.IdBarang, fIsi, Brushes.Black, m1, y)
            g.DrawString(item.NamaBarang, fIsi, Brushes.Black, m2, y)
            g.DrawString(item.Harga.ToString("N0"), fIsi, Brushes.Black, m5, y, fmtKanan)
            g.DrawString(item.Qty.ToString("N0"), fIsi, Brushes.Black, m6, y, fmtKanan)
            g.DrawString(item.Satuan, fIsi, Brushes.Black, m6, y)
            g.DrawString(item.Total.ToString("N0"), fIsi, Brushes.Black, m7, y, fmtKanan)
            y += 10 + jarak : no += 1
        Next

        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 5 + jarak
        g.DrawString("Total :", fIsi, Brushes.Black, m5, y)
        g.DrawString(TB_TotalRupiah.ToString("#,##0"), fIsi, Brushes.Black, m7, y, fmtKanan) : y += 5 + jarak
        g.DrawString(garis, New Font("Courier New", 8), Brushes.Black, bk, y) : y += 10 + jarak
        g.DrawString("Terbilang : " & Terbilang(TB_TotalRupiah) & " Rupiah", fKecil, Brushes.Black, bk, y) : y += 10 + jarak
        g.DrawString("Dicetak : " & TB_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"),
                     New Font("Arial Narrow", 7), Brushes.Gray, bk, y)

        e.HasMorePages = False
    End Sub

End Class
