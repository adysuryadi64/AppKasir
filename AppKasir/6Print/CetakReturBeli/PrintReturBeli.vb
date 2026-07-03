Imports System.Drawing.Printing

Public Class PrintReturBeli
    ' Konfigurasi printer dibaca dari ModuleKonfigurasi (format baru)
    Private cfg As KonfigurasiThermal

    ' Alias untuk kompatibilitas kode cetak yang sudah ada
    Private ReadOnly Property NamaPrinterThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.NamaPrinter, "")
        End Get
    End Property
    Private ReadOnly Property LebarKertasThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.LebarKertas, 80)
        End Get
    End Property
    Private ReadOnly Property BatasKiriThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.BatasKiri, 0)
        End Get
    End Property
    Private ReadOnly Property JarakBarisThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.JarakBaris, 2)
        End Get
    End Property
    Private ReadOnly Property PortLaciKasirThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.PortLaciKasir, "")
        End Get
    End Property
    Private ReadOnly Property KodeLaciKasirThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.KodeLaciKasir, "")
        End Get
    End Property
    Private ReadOnly Property ModelStrukThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.ModelStruk, "Model 1 Lengkap")
        End Get
    End Property
    Private ReadOnly Property FontJudulThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.FontJudul, "Century")
        End Get
    End Property
    Private ReadOnly Property FontKeteranganThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.FontKeterangan, "Arial Narrow")
        End Get
    End Property
    Private ReadOnly Property FontIsiThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.FontIsi, "Arial Narrow")
        End Get
    End Property
    Private ReadOnly Property FontFooterThermal As String
        Get
            Return If(cfg IsNot Nothing, cfg.FontFooter, "Arial Narrow")
        End Get
    End Property
    Private ReadOnly Property UkuranJudulThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.UkuranJudul, 14)
        End Get
    End Property
    Private ReadOnly Property UkuranKeteranganThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.UkuranKeterangan, 10)
        End Get
    End Property
    Private ReadOnly Property UkuranIsiThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.UkuranIsi, 10)
        End Get
    End Property
    Private ReadOnly Property UkuranFooterThermal As Integer
        Get
            Return If(cfg IsNot Nothing, cfg.UkuranFooter, 10)
        End Get
    End Property
    ' Nilai lama yang tidak relevan - tetap 0
    ' 0 dihapus - tidak relevan di ESC/POS
    ' 0 dihapus - tidak relevan di ESC/POS
    ' 100 dihapus - tidak relevan di ESC/POS
    ' LblBaudRateString dihapus - tidak relevan
    ' LblDataBitsString dihapus - tidak relevan

    Public printer_nota As String
    Public MOdelStruk As String
    Public WithEvents PD As New PrintDocument
    Private ReadOnly PPD As New PrintPreviewDialog
    Private longpaper As Integer

    Public Sub Ambildataprinter()
        cfg = New KonfigurasiThermal("ReturBeli")
    End Sub

    Public Sub ProsesCetak()
        Ambildataprinter()
        Ambil_data()
        AmbildataSupplier()
        Printerstruk()
    End Sub


    Private Sub Btnsimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btnsimpan.Click
        Printerstruk()
    End Sub

    Public Sub Ambil_data()
        Using cmd As New MySqlCommand("
        SELECT 
            ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_BELI_SATUAN, TOTAL
        FROM retur_pembelian_detail
        WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN", conn)

            cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                While rd.Read()
                    DgvData.Rows.Add(
                        rd("ID_BARANG"),
                        rd("NAMA_BARANG"),
                        rd("QTY"),
                        rd("SATUAN"),
                        rd("HARGA_BELI_SATUAN"),
                        rd("TOTAL")
                    )
                End While
            End Using
        End Using
    End Sub

    Public Sub AmbildataSupplier()
        Using cmd As New MySqlCommand("
        SELECT 
            TGL_RETUR_BELI,
            NAMA_SUPPLIER,
            ALAMAT_SUPPLIER,
            TOTAL_RUPIAH,
            ID_USER,
            ID_KOMPUTER
        FROM retur_pembelian
        WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN", conn)

            cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    DTPTgl.Value = Convert.ToDateTime(rd("TGL_RETUR_BELI"))
                    TxtNamaSupplier.Text = rd("NAMA_SUPPLIER").ToString()
                    TxtAlamatSupplier.Text = rd("ALAMAT_SUPPLIER").ToString()
                    TxtTotal.Text = Convert.ToDecimal(rd("TOTAL_RUPIAH")).ToString("N0", cultureIndonesia)
                    TxtIdUser.Text = rd("ID_USER").ToString()
                    TxtIdKomputer.Text = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using
    End Sub

    Public Sub Printerstruk()
        Dim printer_nota As String

        If NamaPrinterThermal = "Model 1 Lengkap" Then
            ' Baca default printer pada komputer
            Dim defaultPrinter As String = New PrinterSettings().PrinterName
            printer_nota = defaultPrinter
        Else
            printer_nota = NamaPrinterThermal
        End If

        PD.PrinterSettings.PrinterName = printer_nota
        MOdelStruk = ModelStrukThermal

        Changelongpaper()
        PPD.Document = PD
        PD.Print()
    End Sub

    Public Sub Changelongpaper()
        Dim rowcount As Integer
        longpaper = 0
        rowcount = DgvData.Rows.Count
        longpaper = rowcount * 30
        longpaper += (320 + 0)
    End Sub

    Private Sub PD_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD.BeginPrint
        Dim thermalPaperWidthInmm As Integer = LebarKertasThermal
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * 100)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        PD.DefaultPageSettings.PaperSize = ps
        PD.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD.PrintPage
        Dim leftmargin As Integer = PD.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garisPemisah As String = BuatGaris(HitungLebarGaris(LebarKertasThermal))
        Dim TopRight As New StringFormat With {
            .LineAlignment = StringAlignment.Near,
            .Alignment = StringAlignment.Far
        }

        Dim thermalPaperWidthInmm As Integer = LebarKertasThermal
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * 100)
        Dim lebar As Integer = thermalPaperWidthInPixel

        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + BatasKiriThermal

        tinggi -= 0

        ' Tampilkan header berdasarkan model struk
        If MOdelStruk = "Model 1 Lengkap" Then
            Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
            e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
            tinggi += 30 + JarakBarisThermal
        End If

        If MOdelStruk <> "Model 3 Tanpa Header" Then
            e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(FontJudulThermal, UkuranJudulThermal), Brushes.Black, centermargin, tinggi, tengah)
            tinggi += 20 + JarakBarisThermal
            e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, centermargin, tinggi, tengah)
            tinggi += 10 + JarakBarisThermal
            e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, centermargin, tinggi, tengah)
            tinggi += 10 + JarakBarisThermal
            e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, centermargin, tinggi, tengah)
        End If

        Dim Mulaikata As Integer = BatasKiriThermal + ((lebar + (25 / 100 * lebar)) - lebar)

        tinggi += 15 + JarakBarisThermal
        e.Graphics.DrawString("Nota Retur", New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString("Tanggal", New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"), New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString("Kasir", New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString("Supplier", New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtNamaSupplier.Text, New Drawing.Font(FontKeteranganThermal, UkuranKeteranganThermal), Brushes.Black, Mulaikata, tinggi)

        tinggi += 12 + JarakBarisThermal
        e.Graphics.DrawString(garisPemisah, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 14 + JarakBarisThermal
        e.Graphics.DrawString("RETUR PEMBELIAN", New Drawing.Font(FontKeteranganThermal, (UkuranKeteranganThermal + 2)), Brushes.Black, centermargin, tinggi, tengah)

        tinggi += 12 + JarakBarisThermal
        e.Graphics.DrawString(garisPemisah, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Sesuaikan posisi kolom berdasarkan model struk
        Dim Mulaikata1, Mulaikata2, Mulaikata3, Mulaikata4 As Integer

        If MOdelStruk = "Model 2 Tanpa Diskon" Or MOdelStruk = "Model 3 Tanpa Header" Then
            Mulaikata1 = BatasKiriThermal + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata2 = BatasKiriThermal + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata3 = BatasKiriThermal + ((lebar + (65 / 100 * lebar)) - lebar)
            Mulaikata4 = BatasKiriThermal + ((lebar + (95 / 100 * lebar)) - lebar)
        Else
            Mulaikata1 = BatasKiriThermal + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata2 = BatasKiriThermal + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata3 = BatasKiriThermal + ((lebar + (51 / 100 * lebar)) - lebar)
            Mulaikata4 = BatasKiriThermal + ((lebar + (95 / 100 * lebar)) - lebar)
        End If

        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString("Qty", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata2, tinggi)

        If MOdelStruk = "Model 2 Tanpa Diskon" Or MOdelStruk = "Model 3 Tanpa Header" Then
            e.Graphics.DrawString("Harga", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString("Jumlah", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata4, tinggi, kanan)
        Else
            e.Graphics.DrawString("Harga", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString("Jumlah", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata4, tinggi, kanan)
        End If

        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString(garisPemisah, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Cetak data barang
        For baris As Integer = 0 To DgvData.Rows.Count - 2
            tinggi += 14 + JarakBarisThermal

            ' Nama Barang
            Dim namaBarang As String = DgvData.Rows(baris).Cells("NAMA_BARANG").Value?.ToString()
            e.Graphics.DrawString(namaBarang, New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, BatasKiri, tinggi)

            tinggi += 10 + JarakBarisThermal

            ' QTY
            Dim qtyValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value?.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata1, tinggi, kanan)

            ' Satuan
            Dim satuan As String = DgvData.Rows(baris).Cells("SATUAN").Value?.ToString()
            e.Graphics.DrawString(satuan, New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata2, tinggi)

            ' Harga Beli Satuan
            Dim hargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("HARGA_BELI_SATUAN").Value?.ToString(), hargaValue)
            e.Graphics.DrawString(hargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata3, tinggi, kanan)

            ' Total
            Dim totalValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TOTAL").Value?.ToString(), totalValue)
            e.Graphics.DrawString(totalValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata4, tinggi, kanan)
        Next

        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString(garisPemisah, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + JarakBarisThermal

        e.Graphics.DrawString("Total :", New Drawing.Font(FontIsiThermal, UkuranIsiThermal), Brushes.Black, Mulaikata3, tinggi, kanan)
        ' Ambil nilai total dari TextBox secara aman
        Dim total As Double
        Double.TryParse(TxtTotal.Text, total)

        e.Graphics.DrawString(
    total.ToString("N0", cultureIndonesia),
    New Drawing.Font(FontIsiThermal, UkuranIsiThermal),
    Brushes.Black,
    Mulaikata4,
    tinggi,
    kanan)


        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString(garisPemisah, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Tampilkan footer
        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(FontFooterThermal, UkuranFooterThermal), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + JarakBarisThermal
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(FontFooterThermal, UkuranFooterThermal), Brushes.Black, centermargin, tinggi, tengah)

        '0 -= 0
    End Sub

    Public Sub Bukalaci()
        BukaLaciKasir("ReturBeli")
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class