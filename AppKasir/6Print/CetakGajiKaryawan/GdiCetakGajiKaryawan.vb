Imports System.Drawing.Printing

Public Class GdiCetakGajiKaryawan

    Private ReadOnly _cfg As KonfigurasiThermal
    Private _panjangKertas As Integer
    Private WithEvents _pd As New PrintDocument

    Public Sub New()
        _cfg = New KonfigurasiThermal("GajiKaryawan")
    End Sub

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

    Private Function Rp(v As Decimal) As String
        Return GKRp(v)
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
            Return BK + CInt(LebarPx * 0.25)
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
        _panjangKertas = 600
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

        ' Header
        If _cfg.TampilLogo Then
            Try
                Dim logo As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
                g.DrawImage(logo, CInt((LebarPx - 150) / 2), y, 150, 35) : logo.Dispose()
            Catch : End Try
            y += 30 + Jarak
        End If
        g.DrawString(NAMA_PERUSAHAAN, FJudul, Brushes.Black, Tengah, y, fmtTengah) : y += 20 + Jarak
        g.DrawString(ALAMAT_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 10 + Jarak
        g.DrawString(KOTA_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 10 + Jarak
        g.DrawString(KONTAK_PERUSAHAAN, FKet, Brushes.Black, Tengah, y, fmtTengah) : y += 15 + Jarak

        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak
        Dim pv As Integer = PosNilai
        g.DrawString("Nomor", FKet, Brushes.Black, BK, y) : g.DrawString(": " & GK_Nomor, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Bulan", FKet, Brushes.Black, BK, y) : g.DrawString(": " & GK_Bulan, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Tanggal", FKet, Brushes.Black, BK, y) : g.DrawString(": " & GK_Tanggal.ToString("yyyy-MM-dd"), FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Karyawan", FKet, Brushes.Black, BK, y) : g.DrawString(": " & GK_NamaKaryawan, FKet, Brushes.Black, pv, y) : y += 10 + Jarak
        g.DrawString("Periode", FKet, Brushes.Black, BK, y) : g.DrawString(": " & GK_TanggalAwal.ToString("dd-MM-yyyy") & " s/d " & GK_TanggalAkhir.ToString("dd-MM-yyyy"), FKet, Brushes.Black, pv, y) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak
        g.DrawString("SLIP GAJI KARYAWAN", New Font(_cfg.FontKeterangan, _cfg.UkuranKeterangan + 2), Brushes.Black, Tengah, y, fmtTengah) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak

        ' 2 kolom: Pendapatan | Potongan
        Dim xKiri As Integer = BK
        Dim xTengah As Integer = BK + CInt(LebarPx * 0.5)
        Dim xNilaiKiri As Integer = xTengah - 5
        Dim xNilaiKanan As Integer = BK + CInt(LebarPx * 0.97)

        g.DrawString("PENDAPATAN", FBold, Brushes.Black, xKiri, y)
        g.DrawString("POTONGAN", FBold, Brushes.Black, xTengah, y) : y += 12 + Jarak

        Dim TulisKiri = Sub(lbl As String, val As Decimal, yy As Integer)
                            g.DrawString(lbl, FIsi, Brushes.Black, xKiri, yy)
                            g.DrawString(Rp(val), FIsi, Brushes.Black, xNilaiKiri, yy, fmtKanan)
                        End Sub
        Dim TulisKanan = Sub(lbl As String, val As Decimal, yy As Integer)
                             g.DrawString(lbl, FIsi, Brushes.Black, xTengah, yy)
                             g.DrawString(Rp(val), FIsi, Brushes.Black, xNilaiKanan, yy, fmtKanan)
                         End Sub

        TulisKiri("Gaji Pokok", GK_GajiPokok, y) : TulisKanan("Bon", GK_PotBon, y) : y += 10 + Jarak
        TulisKiri("Komisi Jual", GK_KomisiJual, y) : TulisKanan("Angsuran", GK_Angsuran, y) : y += 10 + Jarak
        TulisKiri("Supir", GK_SupirRp, y) : TulisKanan("Absen", GK_AbsenRp, y) : y += 10 + Jarak
        TulisKiri("Helper", GK_HelperRp, y) : TulisKanan("Absen Khusus", GK_AbsenKhususRp, y) : y += 10 + Jarak
        TulisKiri("Lembur", GK_LemburRp, y) : TulisKanan("Terlambat", GK_TerlambatRp, y) : y += 10 + Jarak
        TulisKiri("Tunjangan", GK_Tunjangan, y) : TulisKanan("Pot. Lain", GK_PotLain, y) : y += 10 + Jarak
        TulisKiri("Transport", GK_Transport, y) : y += 10 + Jarak
        TulisKiri("Uang Makan", GK_UangMakan, y) : y += 10 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak
        g.DrawString("Total Pendapatan", FBold, Brushes.Black, xKiri, y)
        g.DrawString(Rp(GK_TotalPendapatan), FBold, Brushes.Black, xNilaiKiri, y, fmtKanan)
        g.DrawString("Total Potongan", FBold, Brushes.Black, xTengah, y)
        g.DrawString(Rp(GK_TotalPotongan), FBold, Brushes.Black, xNilaiKanan, y, fmtKanan) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 5 + Jarak

        ' Total Terima
        g.DrawString("TOTAL TERIMA", FBold, Brushes.Black, BK, y)
        g.DrawString(Rp(GK_TotalTerima), FBold, Brushes.Black, xNilaiKanan, y, fmtKanan) : y += 14 + Jarak
        g.DrawString(Garis, FGaris, Brushes.Black, BK, y) : y += 10 + Jarak

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
