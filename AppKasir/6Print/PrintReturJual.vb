Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Ports



Public Class PrintReturJual



    ' Deklarasi variabel sebagai string
    Private LblPrinterStrukString As String
    'Dim CmbPortThermalString As String
    'Dim CmbPortString As String
    Private TxtMAjuString As Integer
    Private TxtMundurString As Integer
    Private TxtPanjangString As Integer
    Private TxtLebarString As Integer
    Private TxtpikselString As Integer
    Private TxtBatasKiriString As Integer
    Private TxtJarakString As Integer
    Private lblPortNameString As String
    Private lblBaudRateString As Integer
    'Dim lblParityString As String
    Private lblDataBitsString As Integer
    Private CmbPortCashString As String
    Private CmbCodeCashString As String
    'Dim CBPotongChecked As Boolean
    Private CmbModelStrukString As String
    Private CmbFNAmaString As String
    Private CmbFKetString As String
    Private CmbFIsiString As String
    Private CmbFFootString As String
    Private CmbUNamaString As Integer
    Private CmbUKetString As Integer
    Private CmbUIsiString As Integer
    Private CmbUFootString As Integer
    'Dim StatusComp As String

    Public printer_nota As String
    Public MOdelStruk As String
    Public WithEvents PD As New PrintDocument
    Private ReadOnly PPD As New PrintPreviewDialog
    Public WithEvents PD1 As New PrintDocument
    Private ReadOnly PPD1 As New PrintPreviewDialog
    Public WithEvents PD2 As New PrintDocument
    Private ReadOnly PPD2 As New PrintPreviewDialog
    Public WithEvents PD3 As New PrintDocument
    Private ReadOnly PPD3 As New PrintPreviewDialog
    Private longpaper As Integer



    Public Sub Ambildataprinter()
        Dim filePath As String = "printer.ini"

        ' Check if the file exists
        If File.Exists(filePath) Then
            ' File exists, read the values from the file
            Using reader As New StreamReader(filePath)
                Dim line As String = reader.ReadLine()
                While line IsNot Nothing
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 Then
                        Dim key As String = parts(0)
                        Dim value As String = parts(1)

                        ' Assign values to application settings
                        Select Case key
                            Case "PrinterPos"
                                LblPrinterStrukString = value
                                '    CmbPortThermalString = value
                                'Case "PortPrinter"
                                '    CmbPortString = value
                            Case "Maju"
                                TxtMAjuString = value
                            Case "Mundur"
                                TxtMundurString = value
                            Case "Panjang"
                                TxtPanjangString = value
                            Case "Lebar"
                                TxtLebarString = value
                            Case "Piksel"
                                TxtpikselString = value
                            Case "BatasKiri"
                                TxtBatasKiriString = value
                            Case "Jarak"
                                TxtJarakString = value
                            Case "PortName"
                                lblPortNameString = value
                            Case "BaudRate"
                                lblBaudRateString = value
                                'Case "Parity"
                                '    lblParityString = value
                            Case "DataBits"
                                lblDataBitsString = value
                            Case "PortCashDraw"
                                CmbPortCashString = value
                            Case "CodeCashDraw"
                                CmbCodeCashString = value
                                'Case "Potongkertas"
                                '    CBPotongChecked = Boolean.Parse(value)
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
                                CmbUNamaString = value
                            Case "FontUKet"
                                CmbUKetString = value
                            Case "FontUIsi"
                                CmbUIsiString = value
                            Case "FontUFoot"
                                CmbUFootString = value
                                'Case "StatusComp"
                                '    StatusComp = value
                        End Select
                    End If
                    line = reader.ReadLine()
                End While
            End Using
        End If

    End Sub



    Private Sub TxtFaktur_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtFaktur.TextChanged
        Ambildataprinter()
        Ambil_data()
        Ambildatapelanggan()
        Printerstruk()
        Close()
    End Sub

    Private Sub Btnsimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btnsimpan.Click
        Printerstruk()
    End Sub

    Private Sub DgvData_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellValueChanged
        If e.ColumnIndex = 2 Then 'kolom index 2
            Hitungbarang()
        End If
    End Sub

    Private Sub DgvData_RowsAdded(ByVal sender As Object, ByVal e As DataGridViewRowsAddedEventArgs) Handles DgvData.RowsAdded
        Hitungbarang()
    End Sub

    Public Sub HitungBarang()
        If DgvData IsNot Nothing AndAlso DgvData.RowCount > 0 Then
            Dim jumlahBaris As Integer = DgvData.RowCount - 1
            TxtJmlhBrg.Text = jumlahBaris.ToString()
        Else
            TxtJmlhBrg.Text = "0"
        End If
    End Sub

    Public Sub Ambil_data()
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_BELI_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @ID_RETUR_PENJUALAN", conn)
            cmd.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", TxtFaktur.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                Do While rd.Read()
                    DgvData.Rows.Add(rd("ID_BARANG"), rd("NAMA_BARANG"), rd("QTY"), rd("SATUAN"), rd("HARGA_BELI_SATUAN"), rd("TOTAL_DISKON"), rd("TOTAL_HARGA"))
                Loop
            End Using
        End Using
    End Sub

    Public Sub Ambildatapelanggan()
        Using cmd As New MySqlCommand("SELECT TGL_RETUR_JUAL, NAMA_PELANGGAN, JENIS_PELANGGAN, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER FROM retur_penjualan WHERE ID_RETUR_PENJUALAN = @ID_RETUR_PENJUALAN", conn)
            cmd.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", TxtFaktur.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    DTPTgl.Value = rd("TGL_RETUR_JUAL")
                    CmbPelanggan.Text = rd("NAMA_PELANGGAN").ToString()
                    LblJenisPl.Text = rd("JENIS_PELANGGAN").ToString()
                    TxtTotal.Text = Convert.ToDecimal(rd("TOTAL_RUPIAH")).ToString()
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
        If MOdelStruk = "Model 2 Tanpa Diskon" Then
            Changelongpaper()
            PPD2.Document = PD2
            'PPD2.ShowDialog()
            PD2.Print()
        ElseIf MOdelStruk = "Model 3 Tanpa Header" Then
            Changelongpaper()
            PPD3.Document = PD3
            'PPD3.ShowDialog()
            PD3.Print()
        ElseIf MOdelStruk = "Model 4 Lengkap Tanpa Logo" Then
            Changelongpaper()
            PPD1.Document = PD1
            'PPD3.ShowDialog()
            PD1.Print()
        Else
            Changelongpaper()
            PPD.Document = PD
            'PPD.ShowDialog()
            PD.Print()
        End If
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
        'Dim garisdua As String = "===================="
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
        Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
        e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Retur", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pelanggan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString("RETUR PENJUALAN", New Drawing.Font(CmbFKetString, (CmbUKetString + 2)), Brushes.Black, centermargin, tinggi, tengah)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        'e.Graphics.DrawString("Sat", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Disc", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 14 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("QTY").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("Harga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalDiskon").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalHarga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Double = TxtTotal.Text
        e.Graphics.DrawString(Microsoft.VisualBasic.Format(harga, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)

        tinggi -= TxtMundurString

    End Sub

    Private Sub PD1_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD1.BeginPrint
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        PD1.DefaultPageSettings.PaperSize = ps
        PD1.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD1.PrintPage
        Dim leftmargin As Integer = PD1.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD1.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD1.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String = "-------------------------------------------"
        'Dim garisdua As String = "===================="
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
        'Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
        'tinggi += 10
        'e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        'tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Retur", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pelanggan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString("RETUR PENJUALAN", New Drawing.Font(CmbFKetString, (CmbUKetString + 2)), Brushes.Black, centermargin, tinggi, tengah)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)


        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        'e.Graphics.DrawString("Sat", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Disc", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 14 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("QTY").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("Harga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalDiskon").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalHarga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Double = TxtTotal.Text
        e.Graphics.DrawString(Microsoft.VisualBasic.Format(harga, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)

        tinggi -= TxtMundurString

    End Sub

    Private Sub PD2_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD2.BeginPrint
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        PD2.DefaultPageSettings.PaperSize = ps
        PD2.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD2_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD2.PrintPage
        Dim leftmargin As Integer = PD2.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD2.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD2.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat With {.Alignment = StringAlignment.Far}
        Dim tengah As New StringFormat With {.Alignment = StringAlignment.Center}

        Dim garis As String = "-------------------------------------------"
        'Dim garisdua As String = "============="
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
        Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
        e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)

        Dim Mulaikata As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)
        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Retur", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pelanggan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString("RETUR PENJUALAN", New Drawing.Font(CmbFKetString, CmbUKetString + 2), Brushes.Black, centermargin, tinggi, tengah)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (65 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        'e.Graphics.DrawString("Disc", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 14 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("QTY").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("Harga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            'e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalDiskon").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalHarga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Double = TxtTotal.Text
        e.Graphics.DrawString(Microsoft.VisualBasic.Format(harga, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi -= TxtMundurString
    End Sub


    Private Sub PD3_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD3.BeginPrint
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        PD3.DefaultPageSettings.PaperSize = ps
        PD3.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD3_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD3.PrintPage
        Dim leftmargin As Integer = PD3.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD3.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD3.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String = "-------------------------------------------"
        'Dim garisdua As String = "============="
        Dim TopRight As New StringFormat With {
        .LineAlignment = StringAlignment.Near,
        .Alignment = StringAlignment.Far
    }

        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim lebar As Integer = thermalPaperWidthInPixel


        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + TxtBatasKiriString

        ' Gunakan penugasan gabungan untuk menyederhanakan kode
        tinggi -= TxtMundurString
        Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
        e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)

        Dim Mulaikata As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Retur", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pelanggan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString("RETUR PENJUALAN", New Drawing.Font(CmbFKetString, (CmbUKetString + 2)), Brushes.Black, centermargin, tinggi, tengah)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (65 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 14 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("QTY").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("Harga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DgvData.Rows(baris).Cells("TotalHarga").Value, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Double = TxtTotal.Text
        e.Graphics.DrawString(Microsoft.VisualBasic.Format(harga, "##,##0"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi -= TxtMundurString
    End Sub

    Public Sub Aturnilaiport()
        Try
            SerialPort1.PortName = lblPortNameString
            SerialPort1.BaudRate = lblBaudRateString
            SerialPort1.Parity = Parity.None
            SerialPort1.DataBits = lblDataBitsString
            SerialPort1.StopBits = StopBits.One
            SerialPort1.Open() 'Buka port serial.
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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