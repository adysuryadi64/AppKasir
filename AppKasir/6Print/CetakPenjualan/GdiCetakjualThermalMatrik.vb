Imports System.Drawing.Printing

' ================================================================
' GdiCetakJualThermalMatrik
' Versi GDI+ dari PrintJual.vb — tanpa form, query database sendiri.
' Mendukung 15 model struk thermal.
'
' Cara pakai:
'   Dim cetak As New GdiCetakJualThermalMatrik()
'   cetak.Cetak()          ' cetak langsung
'   cetak.TampilkanPreview() ' preview di layar
' ================================================================
Public Class GdiCetakJualThermalMatrik

#Region "Field & Konstruktor"

    ' ── Konfigurasi dari printer.ini ─────────────────────────
    Private ReadOnly _cfg As KonfigurasiThermal

    ' ── State dot matrix GDI ──────────────────────────────────
    Private _dotDebugItemLogged As Boolean = False
    Private _dotPosHarga As Integer = 0   ' posisi batas terbilang/total dalam karakter
    Private _dotN As Integer = 80         ' total lebar karakter

    ' ── PrintDocument per model ───────────────────────────────
    Private WithEvents _pd As New PrintDocument   ' Model 1 Lengkap
    Private WithEvents _pd1 As New PrintDocument  ' (tidak dipakai — alias ke _pd)
    Private WithEvents _pd2 As New PrintDocument  ' Model 2 Tanpa Diskon
    Private WithEvents _pd3 As New PrintDocument  ' Model 3 Tanpa Header
    Private WithEvents _pd4 As New PrintDocument  ' (tidak dipakai)
    Private WithEvents _pd5 As New PrintDocument  ' Model 6 Dengan Sales
    Private WithEvents _pd6 As New PrintDocument  ' Model 7 Dengan Persen
    Private WithEvents _pd7 As New PrintDocument  ' Model 8 Dengan Total Hutang
    Private WithEvents _pd8 As New PrintDocument  ' (tidak dipakai)
    Private WithEvents _pd9 As New PrintDocument  ' (tidak dipakai)
    Private WithEvents _pd10 As New PrintDocument ' (tidak dipakai)
    Private WithEvents _pd11 As New PrintDocument ' (tidak dipakai)
    Private WithEvents _pd12 As New PrintDocument ' (tidak dipakai)
    Private WithEvents _pd13 As New PrintDocument ' (tidak dipakai)
    Private WithEvents _pd14 As New PrintDocument ' (tidak dipakai)
    Private _panjangKertas As Integer

    ' ── Konstruktor ───────────────────────────────────────────
    ' Data transaksi sudah dimuat di ModulePrinterJual.MuatDataPenjualan()
    Public Sub New()
        _cfg = New KonfigurasiThermal("Jual")
    End Sub

    ' Override footer untuk mode Monitor (tidak pakai _cfg.TampilFooter)
    Public Property TampilFooter1Override As Boolean? = Nothing
    Public Property TampilFooter2Override As Boolean? = Nothing
    Public Property TampilFooter3Override As Boolean? = Nothing

    Private ReadOnly Property ShowFooter1 As Boolean
        Get
            Return If(TampilFooter1Override.HasValue, TampilFooter1Override.GetValueOrDefault(), _cfg.TampilFooter1)
        End Get
    End Property
    Private ReadOnly Property ShowFooter2 As Boolean
        Get
            Return If(TampilFooter2Override.HasValue, TampilFooter2Override.GetValueOrDefault(), _cfg.TampilFooter2)
        End Get
    End Property
    Private ReadOnly Property ShowFooter3 As Boolean
        Get
            Return If(TampilFooter3Override.HasValue, TampilFooter3Override.GetValueOrDefault(), _cfg.TampilFooter3)
        End Get
    End Property

    ' ── Shortcut format rupiah ────────────────────────────────
    Private Function Rp(nilai As Decimal) As String
        Return JualRp(nilai)
    End Function

#End Region

#Region "Cetak / Preview / PDF"

    Public Sub Cetak()
        Debug.WriteLine("[GdiCetakJualThermalMatrik.Cetak] Mulai cetak thermal GDI")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] NamaPrinter: {_cfg.NamaPrinter}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] ModeCetak: {_cfg.ModeCetak}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] ModelStruk: {_cfg.ModelStruk}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] LebarKertas: {_cfg.LebarKertas} mm")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] UkuranKertas: {_cfg.UkuranKertas}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] DpiCetak: {_cfg.DpiCetak}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.Cetak] LebarPx: {LebarPx} px")
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfg.JumlahCetak
            AturDanCetak(preview:=False)
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then
            BukaLaciKasir("Jual")
        End If
    End Sub

    Public Sub TampilkanPreview()
        AturDanCetak(preview:=True)
    End Sub

    ' Render semua halaman ke List(Of Bitmap) — dipakai oleh PDF export
    Public Sub RenderToBitmaps(bitmaps As List(Of System.Drawing.Bitmap))
        HitungPanjangKertas()
        Dim pd As PrintDocument = PilihPrintDocument()

        ' Render ke bitmap via Graphics.FromImage
        Dim bmp As New System.Drawing.Bitmap(LebarPx, _panjangKertas)
        bmp.SetResolution(100, 100)
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            g.Clear(System.Drawing.Color.White)
            Dim e As New PrintPageEventArgs(g,
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                New System.Drawing.Rectangle(0, 0, LebarPx, _panjangKertas),
                pd.DefaultPageSettings)
            ' Panggil handler PrintPage yang sesuai model
            Select Case _cfg.ModelStruk
                Case "Model 1 — Judul Kolom, Diskon, Sisa Hutang" : CetakSatu(e, True,  True,  True)
                Case "Model 2 — Judul Kolom, Diskon" : CetakSatu(e, True,  True,  False)
                Case "Model 3 — Judul Kolom, Sisa Hutang" : CetakSatu(e, True,  False, True)
                Case "Model 4 — Judul Kolom" : CetakSatu(e, True,  False, False)
                Case "Model 5 — Diskon, Sisa Hutang" : CetakSatu(e, False, True,  True)
                Case "Model 6 — Diskon" : CetakSatu(e, False, True,  False)
                Case "Model 7 — Sisa Hutang" : CetakSatu(e, False, False, True)
                Case "Model 8 — Ringkas" : CetakSatu(e, False, False, False)
                Case Else : CetakSatu(e, True,  True,  False)
            End Select
        End Using
        bitmaps.Add(bmp)
    End Sub

    ' Kirim raw ESC/POS cut command ke printer setelah print job GDI+ selesai.
    ' HANYA dipanggil jika driver printer tidak auto-cut sendiri.
    ' Pada kebanyakan driver thermal (Epson TM, Xprinter, dll), driver sudah
    ' mengirim cut otomatis di akhir job — memanggil ini akan menyebabkan 2x potong.
    ' Gunakan hanya jika driver printer adalah "Generic / Text Only" atau sejenisnya.
    ' ESC d 3  = feed 3 baris
    ' GS V 65 0 = partial cut (lebih aman, menyisakan sedikit kertas)
    Private Sub KirimCutGdi(namaPrinter As String)
        Try
            Dim cmd As Byte() = {
                &H1B, &H64, 3,      ' ESC d 3  — feed 3 baris
                &H1D, &H56, 65, 0   ' GS V 65 0 — partial cut
            }
            RawPrinterHelper.KirimKePrinter(namaPrinter, cmd)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub AturDanCetak(preview As Boolean)
        HitungPanjangKertas()
        Dim pd As PrintDocument = PilihPrintDocument()
        pd.PrinterSettings.PrinterName = _cfg.NamaPrinter

        ' Baca DPI aktual dari printer — hanya untuk info debug
        Dim dpiX As Integer = pd.PrinterSettings.DefaultPageSettings.PrinterResolution.X
        Dim dpiY As Integer = pd.PrinterSettings.DefaultPageSettings.PrinterResolution.Y

        If preview Then
            Dim ppd As New PrintPreviewDialog() With {
                .Document = pd,
                .WindowState = FormWindowState.Maximized
            }
            ppd.ShowDialog()
        Else
            pd.Print()
        End If
    End Sub

    ' Pilih PrintDocument sesuai model struk
    Private Function PilihPrintDocument() As PrintDocument
        Select Case _cfg.ModelStruk
            Case "Model 1 — Judul Kolom, Diskon, Sisa Hutang" : Return _pd
            Case "Model 2 — Judul Kolom, Diskon" : Return _pd2
            Case "Model 3 — Judul Kolom, Sisa Hutang" : Return _pd3
            Case "Model 4 — Judul Kolom" : Return _pd3
            Case "Model 5 — Diskon, Sisa Hutang" : Return _pd5
            Case "Model 6 — Diskon" : Return _pd5
            Case "Model 7 — Sisa Hutang" : Return _pd6
            Case "Model 8 — Ringkas" : Return _pd7
            Case Else : Return _pd
        End Select
    End Function

    ' Hitung panjang kertas dinamis berdasarkan jumlah item dan baris footer
    Private Sub HitungPanjangKertas()
        _panjangKertas = Jual_DaftarItem.Count * 30
        _panjangKertas += If(Jual_NominalTransfer > 0, 380, 330)

        ' Tambah tinggi per baris footer (12px + Jarak per baris)
        Dim tinggiPerBaris As Integer = 12 + Jarak
        If _cfg.TampilFooter1 Then
            _panjangKertas += FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tinggiPerBaris
        End If
        If _cfg.TampilFooter2 Then
            _panjangKertas += FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tinggiPerBaris
        End If
        If _cfg.TampilFooter3 Then
            _panjangKertas += FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None).Length * tinggiPerBaris
        End If

        ' Tambah tinggi blok poin loyalitas jika aktif dan ada pelanggan (Req 6)
        If LP_Aktif AndAlso Not String.IsNullOrEmpty(Jual_IdPelanggan) Then
            _panjangKertas += If(Jual_PoinDiperoleh > 0, 60, 50)
        End If
    End Sub

#End Region

#Region "Properties Helper"

    ' Lebar kertas dalam pixel — pakai DpiCetak dari pengaturan printer
    Private ReadOnly Property LebarPx As Integer
        Get
            Return CInt(_cfg.LebarKertas / 25.4 * _cfg.DpiCetak)
        End Get
    End Property

    ' Posisi tengah halaman
    Private ReadOnly Property Tengah As Integer
        Get
            Return LebarPx \ 2
        End Get
    End Property

    ' Posisi batas kiri efektif
    Private ReadOnly Property BatasKiri As Integer
        Get
            Return 2 + _cfg.BatasKiri
        End Get
    End Property

    ' Posisi nilai kanan (25% dari lebar) — sama dengan Mulaikata di PrintJual.vb
    Private ReadOnly Property PosNilaiKanan As Integer
        Get
            Return BatasKiri + CInt(LebarPx * 0.25)
        End Get
    End Property

    ' Jarak baris dari konfigurasi
    Private ReadOnly Property Jarak As Integer
        Get
            Return _cfg.JarakBaris
        End Get
    End Property

    ' Garis pemisah
    Private ReadOnly Property GarisPemisah As String
        Get
            Return BuatGaris(HitungLebarGaris(_cfg.LebarKertas))
        End Get
    End Property
    Private ReadOnly Property GarisGanda As String
        Get
            Return BuatGarisGanda(HitungLebarGaris(_cfg.LebarKertas))
        End Get
    End Property

#End Region

#Region "Font Properties"

    ' Font shortcut
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

#End Region

#Region "BeginPrint Handlers"

    Private Sub Pd_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd.BeginPrint
        AturUkuranKertas(_pd)
    End Sub
    Private Sub Pd1_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd1.BeginPrint
        AturUkuranKertas(_pd1)
    End Sub
    Private Sub Pd2_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd2.BeginPrint
        AturUkuranKertas(_pd2)
    End Sub
    Private Sub Pd3_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd3.BeginPrint
        AturUkuranKertas(_pd3)
    End Sub
    Private Sub Pd4_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd4.BeginPrint
        AturUkuranKertas(_pd4)
    End Sub
    Private Sub Pd5_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd5.BeginPrint
        AturUkuranKertas(_pd5)
    End Sub
    Private Sub Pd6_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd6.BeginPrint
        AturUkuranKertas(_pd6)
    End Sub
    Private Sub Pd7_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd7.BeginPrint
        AturUkuranKertas(_pd7)
    End Sub
    Private Sub Pd8_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd8.BeginPrint
        AturUkuranKertas(_pd8)
    End Sub
    Private Sub Pd9_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd9.BeginPrint
        AturUkuranKertas(_pd9)
    End Sub
    Private Sub Pd10_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd10.BeginPrint
        AturUkuranKertas(_pd10)
    End Sub
    Private Sub Pd11_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd11.BeginPrint
        AturUkuranKertas(_pd11)
    End Sub
    Private Sub Pd12_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd12.BeginPrint
        AturUkuranKertas(_pd12)
    End Sub
    Private Sub Pd13_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd13.BeginPrint
        AturUkuranKertas(_pd13)
    End Sub
    Private Sub Pd14_BeginPrint(sender As Object, e As PrintEventArgs) Handles _pd14.BeginPrint
        AturUkuranKertas(_pd14)
    End Sub

    Private Sub AturUkuranKertas(pd As PrintDocument)
        pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", LebarPx, _panjangKertas)
        pd.DefaultPageSettings.Landscape = False
    End Sub

#End Region

#Region "PrintPage Handlers"

    Private Sub Pd_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd.PrintPage
        CetakModel1(e)
    End Sub
    Private Sub Pd1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd1.PrintPage
        CetakModel1(e)
    End Sub
    Private Sub Pd2_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd2.PrintPage
        CetakModel2(e)
    End Sub
    Private Sub Pd3_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd3.PrintPage
        CetakModel3(e)
    End Sub
    Private Sub Pd4_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd4.PrintPage
        CetakModel5(e)
    End Sub
    Private Sub Pd5_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd5.PrintPage
        CetakModel6(e)
    End Sub
    Private Sub Pd6_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd6.PrintPage
        CetakModel7(e)
    End Sub
    Private Sub Pd7_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd7.PrintPage
        CetakModel8(e)
    End Sub
    Private Sub Pd8_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd8.PrintPage
        CetakModel1(e)
    End Sub
    Private Sub Pd9_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd9.PrintPage
        CetakModel2(e)
    End Sub
    Private Sub Pd10_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd10.PrintPage
        CetakModel3(e)
    End Sub
    Private Sub Pd11_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd11.PrintPage
        CetakModel2(e)
    End Sub
    Private Sub Pd12_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd12.PrintPage
        CetakModel6(e)
    End Sub
    Private Sub Pd13_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd13.PrintPage
        CetakModel7(e)
    End Sub
    Private Sub Pd14_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _pd14.PrintPage
        CetakModel8(e)
    End Sub

#End Region

#Region "Helper Gambar GDI+"

    Private Sub Tulis(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer,
                      Optional fmt As StringFormat = Nothing)
        g.DrawString(teks, fnt, Brushes.Black, x, y, If(fmt, StringFormat.GenericDefault))
    End Sub

    Private Sub TulisKanan(g As Graphics, teks As String, fnt As Font, x As Integer, y As Integer)
        ' Pakai RectangleF dari BatasKiri sampai x agar teks benar-benar berakhir di x
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim rect As New RectangleF(BatasKiri, y, x - BatasKiri, fnt.GetHeight(g) + 2)
        g.DrawString(teks, fnt, Brushes.Black, rect, fmt)
    End Sub

    Private Sub TulisTengah(g As Graphics, teks As String, fnt As Font, y As Integer)
        Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center}
        g.DrawString(teks, fnt, Brushes.Black, Tengah, y, fmt)
    End Sub

#End Region

#Region "Blok Cetak Bersama Thermal"

    ' Cetak header toko — logo ditentukan dari _cfg.TampilLogo
    Private Function CetakHeader(g As Graphics, y As Integer) As Integer
        If _cfg.TampilLogo Then
            Try
                Dim logo As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
                g.DrawImage(logo, CInt((LebarPx - 150) / 2), y, 150, 35)
                logo.Dispose()
            Catch
                ' Logo tidak ada — lewati
            End Try
            y += 30 + Jarak
        End If
        TulisTengah(g, NAMA_PERUSAHAAN, FJudul, y) : y += 20 + Jarak
        TulisTengah(g, ALAMAT_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        TulisTengah(g, KOTA_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        TulisTengah(g, KONTAK_PERUSAHAAN, FKet, y) : y += 10 + Jarak
        ' Tampilkan label jenis dokumen jika Sales Order
        If Jual_JudulNota = "Nota Order" Then
            y += 4 + Jarak
            TulisTengah(g, "** NOTA PESANAN / SALES ORDER **", FKet, y) : y += 12 + Jarak
        End If
        Return y
    End Function

    ' Alias lama — tetap berfungsi untuk kompatibilitas
    Private Function CetakHeaderDenganLogo(g As Graphics, y As Integer) As Integer
        Return CetakHeader(g, y)
    End Function

    Private Function CetakHeaderTanpaLogo(g As Graphics, y As Integer) As Integer
        Return CetakHeader(g, y)
    End Function

    ' Cetak info transaksi (Nota, Tanggal, Kasir, Pelanggan)
    Private Function CetakInfoTransaksi(g As Graphics, y As Integer,
                                         Optional labelSingkat As Boolean = False) As Integer
        Dim posNilai As Integer = PosNilaiKanan
        y += 15 + Jarak
        Tulis(g, Jual_JudulNota, FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_NoFaktur, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, If(labelSingkat, "Tgl", "Tanggal"), FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Kasir", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_IdUser & " - " & Jual_IdKomputer, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, If(labelSingkat, "Pel", "Pelanggan"), FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_JenisPelanggan & " - " & Jual_NamaPelanggan, FKet, posNilai, y) : y += 10 + Jarak
        If Not String.IsNullOrEmpty(Jual_NoSO) Then
            Tulis(g, "Ref. SO", FKet, BatasKiri, y)
            Tulis(g, ": " & Jual_NoSO, FKet, posNilai, y) : y += 10 + Jarak
        End If
        y += 4 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    ' Cetak info transaksi dengan sales (Model 6)
    Private Function CetakInfoTransaksiDenganSales(g As Graphics, y As Integer) As Integer
        Dim posNilai As Integer = PosNilaiKanan
        y += 15 + Jarak
        Tulis(g, Jual_JudulNota, FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_NoFaktur, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Tanggal", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Kasir", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_IdUser & " - " & Jual_IdKomputer, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Pelanggan", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_JenisPelanggan & " - " & Jual_NamaPelanggan, FKet, posNilai, y) : y += 10 + Jarak
        If Not String.IsNullOrEmpty(Jual_NamaSales) Then
            Tulis(g, "Sales", FKet, BatasKiri, y)
            Tulis(g, ": " & Jual_NamaSales, FKet, posNilai, y) : y += 10 + Jarak
        End If
        If Not String.IsNullOrEmpty(Jual_LokasiBarang) Then
            Tulis(g, "Lokasi", FKet, BatasKiri, y)
            Tulis(g, ": " & Jual_LokasiBarang, FKet, posNilai, y) : y += 10 + Jarak
        End If
        If Not String.IsNullOrEmpty(Jual_NoSO) Then
            Tulis(g, "Ref. SO", FKet, BatasKiri, y)
            Tulis(g, ": " & Jual_NoSO, FKet, posNilai, y) : y += 10 + Jarak
        End If
        y += 4 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    ' Satu fungsi item GDI+ — pakaHeader mengontrol baris header kolom,
    ' pakaDiskon mengontrol kolom Disc dan posisi harga (51% vs 65%)
    Private Function CetakItemGdi(g As Graphics, y As Integer,
                                   pakaHeader As Boolean,
                                   pakaDiskon As Boolean) As Integer
        Dim m1 As Integer = BatasKiri + CInt(LebarPx * 0.11)
        Dim m2 As Integer = BatasKiri + CInt(LebarPx * 0.11)
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)

        If pakaDiskon Then
            Dim m3 As Integer = BatasKiri + CInt(LebarPx * 0.51)
            Dim m4 As Integer = BatasKiri + CInt(LebarPx * 0.7)
            y += 5 + Jarak
            If pakaHeader Then
                Tulis(g, "Nama Barang", FIsi, BatasKiri, y)
                TulisKanan(g, "Harga", FIsi, m3, y)
                TulisKanan(g, "Disc", FIsi, m4, y)
                TulisKanan(g, "Jumlah", FIsi, m5, y) : y += 14 + Jarak
                Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
            End If
            For Each item As ItemNotaJual In Jual_DaftarItem
                y += 10 + Jarak
                Tulis(g, item.NamaBarang, FIsi, BatasKiri, y) : y += 10 + Jarak
                TulisKanan(g, item.Qty.ToString("#,0.##", cultureIndonesia), FIsi, m1, y)
                Tulis(g, item.Satuan, FIsi, m2, y)
                TulisKanan(g, Rp(item.Harga), FIsi, m3, y)
                TulisKanan(g, Rp(item.TotalDiskon), FIsi, m4, y)
                TulisKanan(g, Rp(item.TotalHarga), FIsi, m5, y)
                If Not String.IsNullOrEmpty(item.SerialNumber) Then
                    y += 10 + Jarak
                    Tulis(g, "  SN: " & item.SerialNumber, FIsi, BatasKiri, y)
                End If
            Next
        Else
            Dim m3 As Integer = BatasKiri + CInt(LebarPx * 0.65)
            y += 10 + Jarak
            If pakaHeader Then
                Tulis(g, "Nama Barang", FIsi, BatasKiri, y)
                TulisKanan(g, "Harga", FIsi, m3, y)
                TulisKanan(g, "Jumlah", FIsi, m5, y) : y += 10 + Jarak
                Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
            End If
            For Each item As ItemNotaJual In Jual_DaftarItem
                y += 14 + Jarak
                Tulis(g, item.NamaBarang, FIsi, BatasKiri, y) : y += 10 + Jarak
                TulisKanan(g, item.Qty.ToString("#,0.##", cultureIndonesia), FIsi, m1, y)
                Tulis(g, item.Satuan, FIsi, m2, y)
                TulisKanan(g, Rp(item.Harga), FIsi, m3, y)
                TulisKanan(g, Rp(item.TotalHarga), FIsi, m5, y)
                If Not String.IsNullOrEmpty(item.SerialNumber) Then
                    y += 10 + Jarak
                    Tulis(g, "  SN: " & item.SerialNumber, FIsi, BatasKiri, y)
                End If
            Next
        End If
        y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    ' Alias lama — dipakai kode dot matrix GDI yang belum dimigrasi
    Private Function CetakItemDenganDiskon(g As Graphics, y As Integer) As Integer
        Return CetakItemGdi(g, y, pakaHeader:=True, pakaDiskon:=True)
    End Function
    Private Function CetakItemTanpaDiskon(g As Graphics, y As Integer) As Integer
        Return CetakItemGdi(g, y, pakaHeader:=True, pakaDiskon:=False)
    End Function

    ' Cetak blok total — pakaDiskon mengontrol posisi label dan kolom diskon.
    ' Persen diskon/pajak tampil otomatis jika ada data.
    Private Function CetakTotal(g As Graphics, y As Integer, pakaDiskon As Boolean) As Integer
        Dim m3 As Integer = BatasKiri + CInt(LebarPx * If(pakaDiskon, 0.51, 0.65))
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)
        Dim pakaPersen As Boolean = (Jual_DiskonPersen > 0 OrElse Jual_PajakPersen > 0)

        y += 5 + Jarak
        Tulis(g, Jual_DaftarItem.Count & " item", FIsi, BatasKiri, y)
        If pakaPersen Then
            TulisKanan(g, "Subtotal :", FIsi, m3, y)
            TulisKanan(g, Rp(Jual_TotalSebelumPajak), FIsi, m5, y)
        Else
            TulisKanan(g, "Total :", FIsi, m3, y)
            TulisKanan(g, Rp(Jual_Total), FIsi, m5, y)
        End If

        If pakaDiskon AndAlso Jual_Diskon <> 0 Then
            y += 10 + Jarak
            Dim lblDis As String = If(Jual_DiskonPersen > 0,
                "Diskon " & Jual_DiskonPersen.ToString("0.##") & "% :", "Diskon :")
            TulisKanan(g, lblDis, FIsi, m3, y)
            TulisKanan(g, Rp(Jual_Diskon), FIsi, m5, y)
        End If
        If Jual_Pajak <> 0 Then
            y += 10 + Jarak
            Dim lblPjk As String = If(Jual_PajakPersen > 0,
                "Pajak " & Jual_PajakPersen.ToString("0.##") & "% :", "Pajak :")
            TulisKanan(g, lblPjk, FIsi, m3, y)
            TulisKanan(g, Rp(Jual_Pajak), FIsi, m5, y)
        End If
        If Jual_BiayaKirim <> 0 Then
            y += 10 + Jarak
            TulisKanan(g, "Biaya Kirim :", FIsi, m3, y)
            TulisKanan(g, Rp(Jual_BiayaKirim), FIsi, m5, y)
        End If
        If pakaPersen Then
            y += 10 + Jarak
            TulisKanan(g, "Total :", FIsi, m3, y)
            TulisKanan(g, Rp(Jual_Total), FIsi, m5, y)
        End If

        If Jual_JudulNota <> "Nota Order" Then
            y += 10 + Jarak
            If Jual_NominalTransfer > 0 Then
                If Jual_Bayar > 0 Then
                    TulisKanan(g, "Tunai (" & Jual_Penerima & ") :", FIsi, m3, y)
                    TulisKanan(g, Rp(Jual_Bayar), FIsi, m5, y)
                    y += 10 + Jarak
                End If
                TulisKanan(g, "Transfer (" & Jual_NamaAkunTransfer & ") :", FIsi, m3, y)
                TulisKanan(g, Rp(Jual_NominalTransfer), FIsi, m5, y)
            Else
                TulisKanan(g, "Bayar :", FIsi, m3, y)
                TulisKanan(g, Rp(Jual_Bayar), FIsi, m5, y)
            End If

            y += 10 + Jarak
            Tulis(g, GarisGanda, FGaris, m3, y)
            y += 10 + Jarak
            TulisKanan(g, Jual_LabelPembayaran, FIsi, m3, y)
            TulisKanan(g, Rp(Jual_Kembali), FIsi, m5, y)
        End If

        If Jual_StatusTransaksi = "Belum Lunas" AndAlso Jual_AdaJatuhTempo Then
            y += 10 + Jarak
            TulisKanan(g, "Jatuh Tempo :", FIsi, m3, y)
            TulisKanan(g, Jual_JatuhTempoDate.ToString("dd-MM-yyyy"), FIsi, m5, y)
        End If

        y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    ' Alias lama — dipakai kode dot matrix GDI yang belum dimigrasi
    Private Function CetakTotalDenganPersen(g As Graphics, y As Integer) As Integer
        Return CetakTotal(g, y, pakaDiskon:=True)
    End Function

    ' Cetak info bank/transfer (jika ada nominal transfer — termasuk split bayar)
    Private Function CetakInfoBank(g As Graphics, y As Integer) As Integer
        If Jual_NominalTransfer <= 0 Then Return y
        Dim posNilai As Integer = PosNilaiKanan
        y += 10 + Jarak
        Tulis(g, "Metode", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_Metode, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "Bank", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_Bank & " - " & Jual_NamaRekening, FKet, posNilai, y) : y += 10 + Jarak
        Tulis(g, "No Rek", FKet, BatasKiri, y)
        Tulis(g, ": " & Jual_NoRekening, FKet, posNilai, y) : y += 10 + Jarak
        If Not String.IsNullOrEmpty(Jual_NoReferensi) Then
            Tulis(g, "No Reff", FKet, BatasKiri, y)
            Tulis(g, ": " & Jual_NoReferensi, FKet, posNilai, y) : y += 10 + Jarak
        End If
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    ' Cetak sisa hutang pelanggan (Model 8) — satu baris saja
    Private Function CetakSisaHutang(g As Graphics, y As Integer) As Integer
        Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)
        y += 5 + Jarak
        Tulis(g, "Sisa Hutang :", FIsi, BatasKiri, y)
        TulisKanan(g, Rp(Jual_HutangAkhir), FIsi, m5, y)
        y += 10 + Jarak
        Tulis(g, GarisPemisah, FGaris, BatasKiri, y)
        Return y
    End Function

    ' Cetak footer
    Private Sub CetakFooter(g As Graphics, y As Integer)
        ' ── Blok Poin Loyalitas (Req 6) ──────────────────────────
        ' Hanya tampil jika: sistem poin aktif, ada pelanggan, dan ada data poin
        If LP_Aktif AndAlso Not String.IsNullOrEmpty(Jual_IdPelanggan) Then
            Dim m5 As Integer = BatasKiri + CInt(LebarPx * 0.95)
            y += 5 + Jarak
            Tulis(g, GarisPemisah, FGaris, BatasKiri, y) : y += 10 + Jarak
            Tulis(g, "Saldo Poin  :", FIsi, BatasKiri, y)
            TulisKanan(g, Jual_SaldoPoinAkhir.ToString("N0"), FIsi, m5, y) : y += 10 + Jarak
            If Jual_PoinDiperoleh > 0 Then
                Tulis(g, "Poin Diperoleh:", FIsi, BatasKiri, y)
                TulisKanan(g, "+" & Jual_PoinDiperoleh.ToString("N0"), FIsi, m5, y) : y += 10 + Jarak
            End If
            Tulis(g, GarisPemisah, FGaris, BatasKiri, y) : y += 10 + Jarak
        End If

        y += 10 + Jarak
        If ShowFooter1 Then
            For Each baris As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, baris, FFooter, y) : y += 12 + Jarak
            Next
        End If
        If ShowFooter2 Then
            For Each baris As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, baris, FFooter, y) : y += 12 + Jarak
            Next
        End If
        If ShowFooter3 Then
            For Each baris As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                TulisTengah(g, baris, FFooter, y) : y += 12 + Jarak
            Next
        End If
    End Sub

#End Region

#Region "Model Thermal 1-8 (GDI+)"

    ' Satu fungsi universal — semua 8 model memanggil ini
    Private Sub CetakSatu(e As PrintPageEventArgs,
                           pakaHeader As Boolean,
                           pakaDiskon As Boolean,
                           pakaHutang As Boolean)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = 10
        y = CetakHeader(g, y)
        y = CetakInfoTransaksi(g, y)
        y = CetakItemGdi(g, y, pakaHeader, pakaDiskon)
        y = CetakTotal(g, y, pakaDiskon)
        y = CetakInfoBank(g, y)
        If pakaHutang AndAlso Jual_AdaDataHutang Then y = CetakSisaHutang(g, y)
        CetakFooter(g, y)
        e.HasMorePages = False
    End Sub

    ' 8 model — kombinasi 3 dimensi
    Private Sub CetakModel1(e As PrintPageEventArgs)
        CetakSatu(e, True,  True,  True)
    End Sub
    Private Sub CetakModel2(e As PrintPageEventArgs)
        CetakSatu(e, True,  True,  False)
    End Sub
    Private Sub CetakModel3(e As PrintPageEventArgs)
        CetakSatu(e, True,  False, True)
    End Sub
    Private Sub CetakModel4(e As PrintPageEventArgs)
        CetakSatu(e, True,  False, False)
    End Sub
    Private Sub CetakModel5(e As PrintPageEventArgs)
        CetakSatu(e, False, True,  True)
    End Sub
    Private Sub CetakModel6(e As PrintPageEventArgs)
        CetakSatu(e, False, True,  False)
    End Sub
    Private Sub CetakModel7(e As PrintPageEventArgs)
        CetakSatu(e, False, False, True)
    End Sub
    Private Sub CetakModel8(e As PrintPageEventArgs)
        CetakSatu(e, False, False, False)
    End Sub

    ' Alias lama — dipakai PrintPage handlers yang masih ada
    Private Sub CetakModel9(e As PrintPageEventArgs)
        CetakModel1(e)
    End Sub
    Private Sub CetakModel10(e As PrintPageEventArgs)
        CetakModel2(e)
    End Sub
    Private Sub CetakModel11(e As PrintPageEventArgs)
        CetakModel3(e)
    End Sub
    Private Sub CetakModel12(e As PrintPageEventArgs)
        CetakModel4(e)
    End Sub
    Private Sub CetakModel13(e As PrintPageEventArgs)
        CetakModel5(e)
    End Sub
    Private Sub CetakModel14(e As PrintPageEventArgs)
        CetakModel6(e)
    End Sub
    Private Sub CetakModel15(e As PrintPageEventArgs)
        CetakModel7(e)
    End Sub

#End Region

#Region "Cetak Dot Matrix GDI+"

    ' =========================================================
    ' CETAK DOT MATRIX — GDI+ (Windows Print)
    '
    ' Mendukung semua 6 model sama dengan ESC/P, plus:
    ' - Font berbeda per bagian (header bold, isi regular, footer)
    ' - DrawLine untuk garis pemisah (lebih rapi dari ----)
    ' - Garis tanda tangan nyata
    ' - Model 6: garis tipis antar item via DrawLine
    ' =========================================================
    Public Sub CetakDotMatrix()
        Debug.WriteLine("[GdiCetakJualThermalMatrik.CetakDotMatrix] Mulai cetak DOT MATRIX GDI")
        Dim cfgDot As New KonfigurasiDotMatrix("Jual")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.CetakDotMatrix] NamaPrinter: {cfgDot.NamaPrinter}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.CetakDotMatrix] ModeCetak: {cfgDot.ModeCetak}")
        Debug.WriteLine($"[GdiCetakJualThermalMatrik.CetakDotMatrix] ModelStruk: {cfgDot.ModelStruk}")

        If String.IsNullOrEmpty(cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        For i As Integer = 1 To cfgDot.JumlahCetak
            CetakDotMatrixGdiSatu(cfgDot)
        Next
    End Sub

    Private Sub CetakDotMatrixGdiSatu(cfgDot As KonfigurasiDotMatrix)
        Dim fs As Single = cfgDot.UkuranFont
        Dim fntHeader As New Font("Courier New", fs, FontStyle.Bold, GraphicsUnit.Point)
        Dim fntIsi As New Font("Courier New", fs, FontStyle.Regular, GraphicsUnit.Point)
        Dim fntFooter As New Font("Courier New", fs - 1, FontStyle.Regular, GraphicsUnit.Point)

        Dim instruksi As New List(Of (tag As String, teks As String))
        BangunInstruksiDotGdi(cfgDot, instruksi)

        Dim instrIdx As Integer = 0
        Dim tinggiIsi As Single = 0
        Dim lebarKar As Single = 0
        Dim scaleX As Single = 1.0F
        _dotDebugItemLogged = False  ' reset flag debug per cetak

        Dim pd As New System.Drawing.Printing.PrintDocument()
        pd.PrinterSettings.PrinterName = cfgDot.NamaPrinter
        pd.DefaultPageSettings.Landscape = False

        ' ── Set ukuran kertas sesuai pengaturan ──────────────
        ' Unit PaperSize = 1/100 inch
        ' Lebar kertas dari pengaturan (mm → 1/100 inch: mm / 25.4 × 100)
        Dim lebarKertasUnit As Integer = CInt(cfgDot.LebarKertasMm / 25.4 * 100)
        ' Tinggi: hitung dari jumlah baris × tinggi font (continuous form)
        Dim tinggiBaris As Single = fs / 72.0F * 100.0F + cfgDot.JarakBaris
        Dim totalTinggi As Integer = CInt(instruksi.Count * tinggiBaris) + 100

        ' Pilih lebar berdasarkan UkuranKertas
        Dim lebarFinal As Integer
        Select Case cfgDot.UkuranKertas
            Case "9.5 x 11 inch (Continuous)" : lebarFinal = 950
            Case "9.5 x 12 inch (Continuous)" : lebarFinal = 950
            Case "14.875 x 11 inch (Wide)" : lebarFinal = 1488
            Case "A4 (210 x 297 mm)" : lebarFinal = 827
            Case "Letter (8.5 x 11 inch)" : lebarFinal = 850
            Case Else  ' "Continuous Form (Auto)" — hitung dari LebarKertasMm
                lebarFinal = If(lebarKertasUnit > 0, lebarKertasUnit, 950)
        End Select

        Dim tinggiFinal As Integer
        Select Case cfgDot.UkuranKertas
            Case "9.5 x 11 inch (Continuous)", "14.875 x 11 inch (Wide)", "Letter (8.5 x 11 inch)"
                tinggiFinal = Math.Max(totalTinggi, 1100)
            Case "9.5 x 12 inch (Continuous)"
                tinggiFinal = Math.Max(totalTinggi, 1200)
            Case "A4 (210 x 297 mm)"
                tinggiFinal = Math.Max(totalTinggi, 1169)
            Case Else  ' Continuous Form (Auto)
                tinggiFinal = totalTinggi  ' tinggi pas sesuai isi nota
        End Select

        Try
            pd.DefaultPageSettings.PaperSize = New PaperSize("DotMatrix", lebarFinal, tinggiFinal)
        Catch ex As Exception
        End Try

        AddHandler pd.PrintPage,
            Sub(s As Object, e As System.Drawing.Printing.PrintPageEventArgs)
                Dim g As Graphics = e.Graphics

                If tinggiIsi = 0 Then
                    Dim fmt As StringFormat = StringFormat.GenericTypographic
                    lebarKar = g.MeasureString("W", fntIsi, New PointF(0, 0), fmt).Width
                    tinggiIsi = fntIsi.GetHeight(g)

                    ' PageBounds dan MeasureString keduanya dalam unit 1/100 inch
                    ' Tidak perlu konversi DPI
                    Dim lebarFisikGfx As Single = e.PageBounds.Width - 4
                    Dim lebarTeks As Single = cfgDot.LebarKertas * lebarKar
                    scaleX = If(lebarTeks > 0, lebarFisikGfx / lebarTeks, 1.0F)

                End If

                ' x dalam unit Graphics (bukan PageBounds unit)
                Dim x As Single = e.PageBounds.Left + 2
                Dim y As Single = e.PageBounds.Top + 2
                Dim batasY As Single = e.PageBounds.Bottom - 2
                Dim jarakBaris As Single = tinggiIsi + cfgDot.JarakBaris

                Dim garisTebal = New String("-"c, cfgDot.LebarKertas)

                ' ScaleTransform: stretch horizontal agar 80 karakter pas di lebar kertas fisik
                g.ScaleTransform(scaleX, 1.0F)
                Dim xLogis As Single = x / scaleX

                While instrIdx < instruksi.Count
                    Dim item = instruksi(instrIdx)

                    Select Case item.tag
                        Case "G"  ' garis tebal
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            g.DrawString(garisTebal, fntIsi, Brushes.Black, xLogis, y)
                            y += tinggiIsi * 0.8F

                        Case "T"  ' garis tipis Model 6
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            Dim offset As Integer = CInt(cfgDot.LebarKertas * 0.4)
                            Dim garisTipis As String = "".PadRight(offset) &
                                New String("·"c, cfgDot.LebarKertas - offset)
                            g.DrawString(garisTipis, fntIsi, Brushes.DarkGray, xLogis, y)
                            y += tinggiIsi * 0.6F

                        Case "D"  ' garis tanda tangan — fixed 10 karakter per kolom
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            Dim lebarKolD As Single = (e.PageBounds.Width - 4) / scaleX / 3.0F
                            Dim garisTtdD = New String("-"c, 10)
                            g.DrawString(garisTtdD, fntIsi, Brushes.Black, xLogis, y)
                            g.DrawString(garisTtdD, fntIsi, Brushes.Black, xLogis + lebarKolD, y)
                            y += jarakBaris

                        Case "H"  ' header bold
                            If y + fntHeader.GetHeight(g) > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            g.DrawString(item.teks, fntHeader, Brushes.Black, xLogis, y)
                            y += fntHeader.GetHeight(g) + cfgDot.JarakBaris

                        Case "F"  ' footer
                            If y + fntFooter.GetHeight(g) > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            g.DrawString(item.teks, fntFooter, Brushes.Black, xLogis, y)
                            y += fntFooter.GetHeight(g) + cfgDot.JarakBaris

                        Case "FL"  ' footer kiri+kanan sejajar
                            If y + fntFooter.GetHeight(g) > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            Dim bagian As String() = item.teks.Split(Chr(1))
                            If bagian.Length = 2 Then
                                g.DrawString(bagian(0), fntFooter, Brushes.Black, xLogis, y)
                                ' Kanan: hitung posisi dalam koordinat logis
                                Dim szK As SizeF = g.MeasureString(bagian(1), fntFooter,
                                    New PointF(0, 0), StringFormat.GenericTypographic)
                                Dim xKananLogis As Single = (e.PageBounds.Width - 2) / scaleX - szK.Width
                                g.DrawString(bagian(1), fntFooter, Brushes.Black, xKananLogis, y)
                            Else
                                g.DrawString(item.teks, fntFooter, Brushes.Black, xLogis, y)
                            End If
                            y += fntFooter.GetHeight(g) + cfgDot.JarakBaris

                        Case "S"  ' label tanda tangan — posisi pixel
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            Dim bagianTtd As String() = item.teks.Split(Chr(1))
                            Dim lebarKolS As Single = (e.PageBounds.Width - 4) / scaleX / 3.0F
                            g.DrawString(If(bagianTtd.Length > 0, bagianTtd(0), ""), fntIsi, Brushes.Black, xLogis, y)
                            g.DrawString(If(bagianTtd.Length > 1, bagianTtd(1), ""), fntIsi, Brushes.Black, xLogis + lebarKolS, y)
                            g.DrawString(If(bagianTtd.Length > 2, bagianTtd(2), ""), fntIsi, Brushes.Black, xLogis + lebarKolS * 2, y)
                            y += jarakBaris

                        Case "N"  ' nama kasir — sejajar dengan kolom kasir (kolom ke-3)
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            Dim lebarKolN As Single = (e.PageBounds.Width - 4) / scaleX / 3.0F
                            g.DrawString(item.teks, fntIsi, Brushes.Black, xLogis + lebarKolN * 2, y)
                            y += jarakBaris

                        Case "IB"  ' isi bold (Total, Kembali/Hutang)
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True : Exit Sub
                            End If
                            g.DrawString(item.teks, fntHeader, Brushes.Black, xLogis, y)
                            y += jarakBaris

                        Case "IT"  ' total row: terbilang regular kiri, label+nilai kanan (bold opsional)
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            Dim bagianIT As String() = item.teks.Split(Chr(1))
                            Dim terbilangIT As String = If(bagianIT.Length > 0, bagianIT(0), "")
                            Dim isBoldIT As Boolean = bagianIT.Length > 1 AndAlso bagianIT(1) = "1"
                            Dim lblIT As String = If(bagianIT.Length > 2, bagianIT(2), "")
                            Dim valIT As String = If(bagianIT.Length > 3, bagianIT(3), "")
                            Dim fntIT As Font = If(isBoldIT, fntHeader, fntIsi)

                            ' xTotalKiri = batas kiri area total (_dotPosHarga karakter dari xLogis)
                            Dim xTotalKiri As Single = xLogis + _dotPosHarga * lebarKar
                            ' xTotalKanan = batas kanan fixed semua nilai (_dotN karakter dari xLogis)
                            Dim xTotalKanan As Single = xLogis + _dotN * lebarKar

                            ' Terbilang di kiri — fntIsi (regular), mulai dari xLogis
                            If Not String.IsNullOrEmpty(terbilangIT) Then
                                Dim szTerb As SizeF = g.MeasureString(terbilangIT, fntIsi, New PointF(0, 0), StringFormat.GenericTypographic)
                                g.DrawString(terbilangIT, fntIsi, Brushes.Black, xLogis, y)
                            End If

                            ' Nilai rata kanan di xTotalKanan — fntIT (bold/regular)
                            Dim szValIT As SizeF = g.MeasureString(valIT, fntIT, New PointF(0, 0), StringFormat.GenericTypographic)
                            Dim xValStart As Single = xTotalKanan - szValIT.Width
                            g.DrawString(valIT, fntIT, Brushes.Black, xValStart, y)

                            ' Label di kiri nilai — fntIT, rata kanan sebelum nilai
                            If Not String.IsNullOrEmpty(lblIT) Then
                                Dim szLblIT As SizeF = g.MeasureString(lblIT, fntIT, New PointF(0, 0), StringFormat.GenericTypographic)
                                Dim xLblStart As Single = xTotalKanan - szValIT.Width - szLblIT.Width - lebarKar * 0.3F
                                g.DrawString(lblIT, fntIT, Brushes.Black, xLblStart, y)
                            End If

                            y += jarakBaris

                        Case Else  ' "I" isi regular
                            If y + tinggiIsi > batasY Then
                                e.HasMorePages = True
                                Exit Sub
                            End If
                            ' Debug posisi kolom jumlah untuk item pertama
                            If Not _dotDebugItemLogged AndAlso item.teks.Length >= cfgDot.LebarKertas Then
                                _dotDebugItemLogged = True
                                Dim szItem As SizeF = g.MeasureString(item.teks, fntIsi, New PointF(0, 0), StringFormat.GenericTypographic)
                                Dim xKananItem As Single = xLogis + cfgDot.LebarKertas * lebarKar
                            End If
                            g.DrawString(item.teks, fntIsi, Brushes.Black, xLogis, y)
                            y += jarakBaris
                    End Select

                    instrIdx += 1
                End While
                e.HasMorePages = False
            End Sub

        pd.Print()
        fntHeader.Dispose() : fntIsi.Dispose() : fntFooter.Dispose()
    End Sub

#End Region

#Region "Dot Matrix — Bangun Instruksi GDI"

    ' ── Bangun instruksi render dot matrix GDI ────────────────
    ' Layout sepenuhnya independen dari ESC class
    Private Sub BangunInstruksiDotGdi(cfgDot As KonfigurasiDotMatrix,
                                       instruksi As List(Of (tag As String, teks As String)))
        Dim n As Integer = cfgDot.LebarKertas
        Dim isModel6 As Boolean = cfgDot.ModelStruk = "Model 6 Dengan Pemisah"

        ' Hitung kolom
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah As Integer
        Dim posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDotGdi(n, lebarNo, lebarNama, lebarQty, lebarSat,
                          lebarHarga, lebarDisc, lebarJumlah,
                          posHarga, lebarLabelTotal, lebarNilaiTotal)

        ' ── Header toko ──────────────────────────────────────
        instruksi.Add(("H", KiriKananGdi(NAMA_PERUSAHAAN, "N O T A  P E N J U A L A N", n)))
        instruksi.Add(("H", KiriKananGdi(ALAMAT_PERUSAHAAN, "Trx : " & Jual_NoFaktur, n)))
        instruksi.Add(("I", KiriKananGdi(KOTA_PERUSAHAAN, "Tgl : " & Jual_Tanggal.ToString("dd-MM-yyyy HH:mm"), n)))
        instruksi.Add(("I", KiriKananGdi(KONTAK_PERUSAHAAN, "Pel : " & Jual_JenisPelanggan & " " & Jual_NamaPelanggan, n)))

        Select Case cfgDot.ModelStruk
            Case "Model 3 Dengan Sales"
                If Not String.IsNullOrEmpty(Jual_NamaSales) Then
                    instruksi.Add(("I", KiriKananGdi("Sales : " & Jual_NamaSales,
                        If(Not String.IsNullOrEmpty(Jual_LokasiBarang), "Lok: " & Jual_LokasiBarang, ""), n)))
                End If
        End Select

        instruksi.Add(("G", ""))

        ' ── Header kolom item ────────────────────────────────
        Dim denganDiskon As Boolean = cfgDot.ModelStruk <> "Model 2 Tanpa Diskon"
        If denganDiskon Then
            instruksi.Add(("I", RataKiriGdi("No", lebarNo) & RataKiriGdi("Barang", lebarNama) &
                               RataKananGdi("Qty", lebarQty) & " " & RataKiriGdi("Sat", lebarSat - 1) &
                               RataKananGdi("Harga", lebarHarga) & RataKananGdi("Disc", lebarDisc) &
                               RataKananGdi("Jumlah", lebarJumlah)))
        Else
            Dim lH2, lJ2 As Integer
            Dim pH2, lLT2, lNT2 As Integer
            HitungKolomDotTanpaDiskonGdi(n, lebarNo, lebarNama, lebarQty, lebarSat, lH2, lJ2, pH2, lLT2, lNT2)
            instruksi.Add(("I", RataKiriGdi("No", lebarNo) & RataKiriGdi("Barang", lebarNama) &
                               RataKananGdi("Qty", lebarQty) & " " & RataKiriGdi("Sat", lebarSat - 1) &
                               RataKananGdi("Harga", lH2) & RataKananGdi("Jumlah", lJ2)))
            lebarHarga = lH2 : lebarJumlah = lJ2
            posHarga = pH2 : lebarLabelTotal = lLT2 : lebarNilaiTotal = lNT2
        End If
        instruksi.Add(("G", ""))

        ' ── Baris item ───────────────────────────────────────
        Dim nomor As Integer = 1
        For Each item As ItemNotaJual In Jual_DaftarItem
            Dim namaList As List(Of String) = WrapTeksGdi(item.NamaBarang, lebarNama)
            If denganDiskon Then
                instruksi.Add(("I", RataKiriGdi(nomor.ToString() & ".", lebarNo) &
                                   RataKiriGdi(namaList(0), lebarNama) &
                                   RataKananGdi(item.Qty.ToString("#,0", cultureIndonesia), lebarQty) &
                                   " " & RataKiriGdi(item.Satuan, lebarSat - 1) &
                                   RataKananGdi(Rp(item.Harga), lebarHarga) &
                                   RataKananGdi(Rp(item.TotalDiskon), lebarDisc) &
                                   RataKananGdi(Rp(item.TotalHarga), lebarJumlah)))
            Else
                instruksi.Add(("I", RataKiriGdi(nomor.ToString() & ".", lebarNo) &
                                   RataKiriGdi(namaList(0), lebarNama) &
                                   RataKananGdi(item.Qty.ToString("#,0", cultureIndonesia), lebarQty) &
                                   " " & RataKiriGdi(item.Satuan, lebarSat - 1) &
                                   RataKananGdi(Rp(item.Harga), lebarHarga) &
                                   RataKananGdi(Rp(item.TotalHarga), lebarJumlah)))
            End If
            ' Baris lanjutan nama
            For k As Integer = 1 To namaList.Count - 1
                instruksi.Add(("I", "".PadRight(lebarNo) & RataKiriGdi(namaList(k), lebarNama)))
            Next
            If Not String.IsNullOrEmpty(item.SerialNumber) Then
                instruksi.Add(("I", "".PadRight(lebarNo) & "SN: " & item.SerialNumber))
            End If
            ' Catatan: jarak antar item di GDI diatur via cfgDot.JarakBaris di renderer (pixel),
            ' tidak perlu baris kosong tambahan di sini

            ' Garis tipis Model 6 setiap 2 item
            If isModel6 AndAlso nomor Mod 2 = 0 AndAlso nomor < Jual_DaftarItem.Count Then
                instruksi.Add(("T", ""))
            End If
            nomor += 1
        Next
        instruksi.Add(("G", ""))

        ' ── Total + tanda tangan sejajar ─────────────────────
        ' Dicetak sebagai "I" biasa dengan padding — otomatis sejajar dengan kolom item
        ' Simpan posisi untuk renderer
        _dotPosHarga = posHarga
        _dotN = n

        Dim terbilangBaris As String() = PecahTeksGdi(Terbilang(Jual_Total), posHarga - 1)
        Dim totalBaris As New List(Of (lbl As String, val As String)) From {
            ("Sub Total :", Rp(Jual_TotalSebelumPajak))
        }
        If denganDiskon AndAlso Jual_Diskon <> 0 Then
            totalBaris.Add(("Diskon
            ", Rp(Jual_Diskon)))
        End If
        If Jual_Pajak <> 0 Then
            totalBaris.Add(("Pajak
            ", Rp(Jual_Pajak)))
        End If
        If Jual_BiayaKirim <> 0 Then
            totalBaris.Add(("Biaya Kirim
            ", Rp(Jual_BiayaKirim)))
        End If
        totalBaris.Add(("Total :", Rp(Jual_Total)))
        ' Jika split bayar: Tunai + Transfer langsung (tanpa baris Bayar)
        If Jual_JudulNota <> "Nota Order" Then
            If Jual_NominalTransfer > 0 Then
                If Jual_Bayar > 0 Then
                    totalBaris.Add(("Tunai (" & Jual_Penerima & ")
                ", Rp(Jual_Bayar)))
                End If
                totalBaris.Add(("Transfer (" & Jual_NamaAkunTransfer & ") :", Rp(Jual_NominalTransfer)))
            Else
                totalBaris.Add(("Bayar :", Rp(Jual_Bayar)))
            End If
            totalBaris.Add((Jual_LabelPembayaran, Rp(Jual_Kembali)))
        End If
        If Jual_StatusTransaksi = "Belum Lunas" AndAlso Not String.IsNullOrEmpty(Jual_JatuhTempo) Then
            totalBaris.Add(("Jatuh Tempo :", Jual_JatuhTempo))
        End If
        If cfgDot.ModelStruk = "Model 5 Dengan Hutang" AndAlso Jual_AdaDataHutang Then
            totalBaris.Add(("Sisa Hutang :", Rp(Jual_HutangAkhir)))
        End If

        ' Terbilang hanya muncul di baris "Total :" — baris lain kiri kosong.
        ' Kalau terbilang > 1 baris, baris lanjutan disisipkan setelah baris Total.
        Dim idxTotal As Integer = totalBaris.FindIndex(Function(t) t.lbl = "Total :")
        For i As Integer = 0 To totalBaris.Count - 1
            Dim kiri As String = If(i = idxTotal AndAlso terbilangBaris.Length > 0, terbilangBaris(0), "")
            Dim lbl As String = totalBaris(i).lbl
            Dim val As String = totalBaris(i).val
            Dim isBold As Boolean = (lbl = "Total :" OrElse lbl = Jual_LabelPembayaran)
            instruksi.Add(("IT", kiri & Chr(1) & If(isBold, "1", "0") & Chr(1) & lbl & Chr(1) & val))
            ' Sisipkan baris lanjutan terbilang (baris ke-2 dst) setelah baris Total, kanan kosong
            If i = idxTotal Then
                For k As Integer = 1 To terbilangBaris.Length - 1
                    instruksi.Add(("IT", terbilangBaris(k) & Chr(1) & "0" & Chr(1) & "" & Chr(1) & ""))
                Next
            End If
        Next

        ' Info transfer (Model 4) — hanya detail bank, Tunai/Transfer sudah di blok total
        If cfgDot.ModelStruk = "Model 4 Dengan Transfer" AndAlso Jual_NominalTransfer > 0 Then
            instruksi.Add(("G", ""))
            instruksi.Add(("I", KiriKananGdi("Metode  : " & Jual_Metode, "", n)))
            instruksi.Add(("I", KiriKananGdi("Bank    : " & Jual_Bank & " - " & Jual_NamaRekening, "No: " & Jual_NoRekening, n)))
            If Not String.IsNullOrEmpty(Jual_NoReferensi) Then
                instruksi.Add(("I", "No Ref
                " & Jual_NoReferensi))
            End If
        End If

        ' ── Tanda tangan ─────────────────────────────────────
        instruksi.Add(("G", ""))
        instruksi.Add(("S", "Hormat kami" & Chr(1) & "Penerima" & Chr(1) & "Kasir"))
        instruksi.Add(("I", ""))   ' jarak
        instruksi.Add(("D", ""))   ' garis tanda tangan
        instruksi.Add(("N", Jual_IdUser))  ' nama kasir

        ' ── Footer ───────────────────────────────────────────
        instruksi.Add(("I", ""))
        If cfgDot.TampilFooter1 OrElse cfgDot.TampilFooter3 Then
            Dim b1 As String() = If(cfgDot.TampilFooter1, FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None), New String() {""})
            Dim b3 As String() = If(cfgDot.TampilFooter3, FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None), New String() {""})
            Dim maxF As Integer = Math.Max(b1.Length, b3.Length)
            For i As Integer = 0 To maxF - 1
                Dim kiri As String = If(i < b1.Length, b1(i), "")
                Dim kanan As String = If(i < b3.Length, b3(i), "")
                If String.IsNullOrEmpty(kanan) Then
                    instruksi.Add(("F", kiri))
                Else
                    instruksi.Add(("FL", kiri & Chr(1) & kanan))
                End If
            Next
        End If
        If cfgDot.TampilFooter2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                instruksi.Add(("F", b))
            Next
        End If

    End Sub

#End Region

#Region "Dot Matrix — Helper Layout"

    ' ── Helper layout GDI dot matrix ─────────────────────────
    ' Semua kolom dihitung dari persentase n (lebar kertas dalam karakter)
    ' sama persis dengan pola ESC/P — otomatis menyesuaikan LebarKertas & UkuranFont dari printer.ini
    '
    ' Dengan Diskon:
    '   No=4%  Nama=sisanya  Qty=5%  Sat=6%  Harga=15%  Disc=9%  Jumlah=22%
    '   posHarga = No+Nama+Qty+1+Sat  (batas kiri area total/terbilang)
    '
    ' Tanpa Diskon:
    '   No=4%  Nama=sisanya  Qty=5%  Sat=6%  Harga=18%  Jumlah=22%
    Private Sub HitungKolomDotGdi(n As Integer,
                                   ByRef lebarNo As Integer, ByRef lebarNama As Integer,
                                   ByRef lebarQty As Integer, ByRef lebarSat As Integer,
                                   ByRef lebarHarga As Integer, ByRef lebarDisc As Integer,
                                   ByRef lebarJumlah As Integer, ByRef posHarga As Integer,
                                   ByRef lebarLabelTotal As Integer, ByRef lebarNilaiTotal As Integer)
        lebarNo = Math.Max(2, CInt(n * 0.04))
        lebarQty = Math.Max(4, CInt(n * 0.05))
        lebarSat = Math.Max(4, CInt(n * 0.06))
        lebarHarga = Math.Max(9, CInt(n * 0.15))
        lebarDisc = Math.Max(6, CInt(n * 0.09))
        lebarJumlah = Math.Max(9, CInt(n * 0.22))
        lebarNama = n - lebarNo - lebarQty - 1 - lebarSat - lebarHarga - lebarDisc - lebarJumlah
        If lebarNama < 8 Then lebarNama = 8
        posHarga = lebarNo + lebarNama + lebarQty + 1 + lebarSat
        lebarLabelTotal = lebarDisc
        lebarNilaiTotal = lebarJumlah
    End Sub

    Private Sub HitungKolomDotTanpaDiskonGdi(n As Integer,
                                              ByRef lebarNo As Integer, ByRef lebarNama As Integer,
                                              ByRef lebarQty As Integer, ByRef lebarSat As Integer,
                                              ByRef lebarHarga As Integer, ByRef lebarJumlah As Integer,
                                              ByRef posHarga As Integer,
                                              ByRef lebarLabelTotal As Integer, ByRef lebarNilaiTotal As Integer)
        lebarNo = Math.Max(2, CInt(n * 0.04))
        lebarQty = Math.Max(4, CInt(n * 0.05))
        lebarSat = Math.Max(4, CInt(n * 0.06))
        lebarHarga = Math.Max(11, CInt(n * 0.18))
        lebarJumlah = Math.Max(9, CInt(n * 0.22))
        lebarNama = n - lebarNo - lebarQty - 1 - lebarSat - lebarHarga - lebarJumlah
        If lebarNama < 8 Then lebarNama = 8
        posHarga = lebarNo + lebarNama + lebarQty + 1 + lebarSat
        lebarLabelTotal = Math.Max(6, CInt(lebarJumlah * 0.5))
        lebarNilaiTotal = lebarJumlah - lebarLabelTotal
    End Sub

    Private Function RataKiriGdi(teks As String, lebar As Integer) As String
        If teks.Length >= lebar Then Return teks.Substring(0, lebar)
        Return teks.PadRight(lebar)
    End Function

    Private Function RataKananGdi(teks As String, lebar As Integer) As String
        If teks.Length >= lebar Then Return teks.Substring(teks.Length - lebar)
        Return teks.PadLeft(lebar)
    End Function

    Private Function KiriKananGdi(kiri As String, kanan As String, totalLebar As Integer) As String
        Dim spasi As Integer = totalLebar - kiri.Length - kanan.Length
        If spasi < 1 Then spasi = 1
        Return kiri & New String(" "c, spasi) & kanan
    End Function

    Private Function WrapTeksGdi(teks As String, lebarMaks As Integer) As List(Of String)
        Dim hasil As New List(Of String)
        If teks.Length <= lebarMaks Then
            hasil.Add(teks)
            Return hasil
        End If
        Dim kata As String() = teks.Split(" "c)
        Dim sb As New System.Text.StringBuilder()
        For Each k As String In kata
            If sb.Length + k.Length + (If(sb.Length > 0, 1, 0)) > lebarMaks Then
                If sb.Length > 0 Then
                    hasil.Add(sb.ToString())
                    sb.Clear()
                End If
                If k.Length > lebarMaks Then
                    hasil.Add(k.Substring(0, lebarMaks))
                    k = k.Substring(lebarMaks)
                End If
            End If
            If sb.Length > 0 Then sb.Append(" ")
            sb.Append(k)
        Next
        If sb.Length > 0 Then hasil.Add(sb.ToString())
        Return hasil
    End Function

    Private Function PecahTeksGdi(teks As String, lebarMaks As Integer) As String()
        If lebarMaks < 1 Then Return New String() {teks}
        Dim hasil As New List(Of String)
        Dim kata As String() = teks.Split(" "c)
        Dim sb As New System.Text.StringBuilder()
        For Each k As String In kata
            If sb.Length + k.Length + 1 > lebarMaks AndAlso sb.Length > 0 Then
                hasil.Add(sb.ToString().TrimEnd())
                sb.Clear()
            End If
            If sb.Length > 0 Then sb.Append(" ")
            sb.Append(k)
        Next
        If sb.Length > 0 Then hasil.Add(sb.ToString())
        Return hasil.ToArray()
    End Function

#End Region

End Class
