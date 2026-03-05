Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Ports

Public Class PrintReturBeli
    ' Deklarasi variabel yang benar-benar digunakan
    Private LblPrinterStrukString As String
    Private TxtMAjuString As Integer
    Private TxtPanjangString As Integer
    Private TxtLebarString As Integer
    Private TxtpikselString As Integer
    Private TxtBatasKiriString As Integer
    Private TxtJarakString As Integer
    Private lblBaudRateString As Integer
    Private CmbPortCashString As String
    Private CmbCodeCashString As String
    Private CmbModelStrukString As String
    Private CmbFNAmaString As String
    Private CmbFKetString As String
    Private CmbFIsiString As String
    Private CmbFFootString As String
    Private CmbUNamaString As Integer
    Private CmbUKetString As Integer
    Private CmbUIsiString As Integer
    Private CmbUFootString As Integer

    Public printer_nota As String
    Public MOdelStruk As String
    Public WithEvents PD As New PrintDocument
    Private ReadOnly PPD As New PrintPreviewDialog
    Private longpaper As Integer

    Public Sub Ambildataprinter()
        Dim filePath As String = "printer.ini"

        If File.Exists(filePath) Then
            Using reader As New StreamReader(filePath)
                Dim line As String = reader.ReadLine()
                While line IsNot Nothing
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 Then
                        Dim key As String = parts(0)
                        Dim value As String = parts(1)

                        Select Case key
                            Case "PrinterPos"
                                LblPrinterStrukString = value
                            Case "Maju"
                                TxtMAjuString = Integer.Parse(value)
                            Case "Panjang"
                                TxtPanjangString = Integer.Parse(value)
                            Case "Lebar"
                                TxtLebarString = Integer.Parse(value)
                            Case "Piksel"
                                TxtpikselString = Integer.Parse(value)
                            Case "BatasKiri"
                                TxtBatasKiriString = Integer.Parse(value)
                            Case "Jarak"
                                TxtJarakString = Integer.Parse(value)
                            Case "BaudRate"
                                lblBaudRateString = Integer.Parse(value)
                            Case "PortCashDraw"
                                CmbPortCashString = value
                            Case "CodeCashDraw"
                                CmbCodeCashString = value
                            Case "ModelStruk"
                                CmbModelStrukString = value
                            Case "FontNama"
                                CmbFNAmaString = value
                            Case "FontKet"
                                CmbFKetString = value
                            Case "FontIsi"
                                CmbFIsiString = value
                            Case "FOntFoot"
                                CmbFFootString = value
                            Case "FontUNama"
                                CmbUNamaString = Integer.Parse(value)
                            Case "FontUKet"
                                CmbUKetString = Integer.Parse(value)
                            Case "FontUIsi"
                                CmbUIsiString = Integer.Parse(value)
                            Case "FontUFoot"
                                CmbUFootString = Integer.Parse(value)
                        End Select
                    End If
                    line = reader.ReadLine()
                End While
            End Using
        End If
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
                    TxtTotal.Text = Convert.ToDecimal(rd("TOTAL_RUPIAH")).ToString("N0")
                    TxtIdUser.Text = rd("ID_USER").ToString()
                    TxtIdKomputer.Text = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using
    End Sub

    Public Sub Printerstruk()
        Dim printer_nota As String

        If LblPrinterStrukString = "Model 1 Lengkap" Then
            ' Baca default printer pada komputer
            Dim defaultPrinter As String = New PrinterSettings().PrinterName
            printer_nota = defaultPrinter
        Else
            printer_nota = LblPrinterStrukString
        End If

        PD.PrinterSettings.PrinterName = printer_nota
        MOdelStruk = CmbModelStrukString

        Changelongpaper()
        PPD.Document = PD
        PD.Print()
    End Sub

    Public Sub Changelongpaper()
        Dim rowcount As Integer
        longpaper = 0
        rowcount = DgvData.Rows.Count
        longpaper = rowcount * 30
        longpaper += (320 + TxtPanjangString)
    End Sub

    Private Sub PD_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD.BeginPrint
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
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

        Dim garis As String = "-------------------------------------------"
        Dim TopRight As New StringFormat With {
            .LineAlignment = StringAlignment.Near,
            .Alignment = StringAlignment.Far
        }

        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim lebar As Integer = thermalPaperWidthInPixel

        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + TxtBatasKiriString

        tinggi -= TxtMAjuString

        ' Tampilkan header berdasarkan model struk
        If MOdelStruk = "Model 1 Lengkap" Then
            Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
            e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
            tinggi += 30 + TxtJarakString
        End If

        If MOdelStruk <> "Model 3 Tanpa Header" Then
            e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
            tinggi += 20 + TxtJarakString
            e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        End If

        Dim Mulaikata As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Retur", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Supplier", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtNamaSupplier.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString("RETUR PEMBELIAN", New Drawing.Font(CmbFKetString, (CmbUKetString + 2)), Brushes.Black, centermargin, tinggi, tengah)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Sesuaikan posisi kolom berdasarkan model struk
        Dim Mulaikata1, Mulaikata2, Mulaikata3, Mulaikata4 As Integer

        If MOdelStruk = "Model 2 Tanpa Diskon" Or MOdelStruk = "Model 3 Tanpa Header" Then
            Mulaikata1 = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata2 = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata3 = TxtBatasKiriString + ((lebar + (65 / 100 * lebar)) - lebar)
            Mulaikata4 = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)
        Else
            Mulaikata1 = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata2 = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
            Mulaikata3 = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
            Mulaikata4 = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)

        If MOdelStruk = "Model 2 Tanpa Diskon" Or MOdelStruk = "Model 3 Tanpa Header" Then
            e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        Else
            e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Cetak data barang
        For baris As Integer = 0 To DgvData.Rows.Count - 2
            tinggi += 14 + TxtJarakString

            ' Nama Barang
            Dim namaBarang As String = DgvData.Rows(baris).Cells("NAMA_BARANG").Value?.ToString()
            e.Graphics.DrawString(namaBarang, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

            tinggi += 10 + TxtJarakString

            ' QTY
            Dim qtyValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value?.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)

            ' Satuan
            Dim satuan As String = DgvData.Rows(baris).Cells("SATUAN").Value?.ToString()
            e.Graphics.DrawString(satuan, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)

            ' Harga Beli Satuan
            Dim hargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("HARGA_BELI_SATUAN").Value?.ToString(), hargaValue)
            e.Graphics.DrawString(hargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)

            ' Total
            Dim totalValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TOTAL").Value?.ToString(), totalValue)
            e.Graphics.DrawString(totalValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        ' Ambil nilai total dari TextBox secara aman
        Dim total As Double
        Double.TryParse(TxtTotal.Text, total)

        e.Graphics.DrawString(
    total.ToString("N0", cultureIndonesia),
    New Drawing.Font(CmbFIsiString, CmbUIsiString),
    Brushes.Black,
    Mulaikata4,
    tinggi,
    kanan)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Tampilkan footer
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)

        TxtMundurString -= TxtMundurString
    End Sub

    Public Sub Bukalaci()
        Try
            If CmbCodeCashString = "OPTION 1" Then
                Dim comPort As String = CmbPortCashString
                Dim baudRate As Integer = lblBaudRateString
                Dim dataBits As Integer = lblDataBitsString
                Dim stopBits As StopBits = IO.Ports.StopBits.One
                Dim parity As IO.Ports.Parity = IO.Ports.Parity.None

                Using port As New IO.Ports.SerialPort(comPort, baudRate, parity, dataBits, stopBits)
                    port.Open()
                    port.Write(Chr(&H1B) & Chr(&H70) & Chr(&H0) & Chr(&H50) & Chr(&H50)) ' Perintah untuk membuka laci kasir
                End Using

            ElseIf CmbCodeCashString = "OPTION 2" Then
                Dim comPort As String = CmbPortCashString
                Dim baudRate As Integer = lblBaudRateString
                Dim dataBits As Integer = lblDataBitsString
                Dim stopBits As StopBits = IO.Ports.StopBits.One
                Dim parity As IO.Ports.Parity = IO.Ports.Parity.None

                Using port As New IO.Ports.SerialPort(comPort, baudRate, parity, dataBits, stopBits)
                    port.Open()
                    Dim openDrawer As Byte() = {&H1B, &H70, &H0, &H50, &H50} ' Perintah untuk membuka laci kasir
                    port.Write(openDrawer, 0, openDrawer.Length)
                End Using
            End If
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class