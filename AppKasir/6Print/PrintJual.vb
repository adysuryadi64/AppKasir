Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Ports



Public Class PrintJual
    ' Deklarasi variabel sebagai string
    Private LblPrinterStrukString As String
    Private jenisprinter As String
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


    Dim printerDot As String
    Dim lebarDot As Integer
    Dim TinggiDot As Integer
    Dim batasKiriDot As Integer
    Dim jarakBarisDot As Integer
    Dim fontJudulDot As String
    Dim fontIsiDot As String
    Dim ukuranFontJudul As Integer
    Dim ukuranFontIsi As Integer



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
    Public WithEvents PD4 As New PrintDocument
    Private ReadOnly PPD4 As New PrintPreviewDialog

    Public WithEvents PDDot As New PrintDocument
    Private ReadOnly PPDDot As New PrintPreviewDialog

    Private longpaper As Integer
    Private Panjangkertas As Integer
    Private LebarKertas As Integer


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
                            Case "JenisPrinterJual"
                                jenisprinter = value
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

                            Case "PrinterDot"
                                printerDot = value
                            Case "LebarDot"
                                lebarDot = value
                            Case "TinggiDot"
                                TinggiDot = value
                            Case "BatasKiriDot"
                                batasKiriDot = value
                            Case "JarakBarisDot"
                                jarakBarisDot = value
                            Case "FontJudulDot"
                                fontJudulDot = value
                            Case "FontIsiDot"
                                fontIsiDot = value
                            Case "UkuranFontJudul"
                                ukuranFontJudul = value
                            Case "UkuranFontIsi"
                                ukuranFontIsi = value
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
        UrutkanNoDgvData()
        Ambildatapelanggan()

        If jenisprinter = "Printer Thermal" Then
            Printerstruk()
        ElseIf jenisprinter = "Printer Dot Matrix" Then
            PrinterDotMatrik()
        Else
            ' Jika bukan printer Thermal atau Dot Matrix, tampilkan preview
            MOdelStruk = CmbModelStrukString
            Changelongpaper()

            ' Gunakan model struk yang sesuai untuk preview
            Select Case MOdelStruk
                Case "Model 2 Tanpa Diskon"
                    PPD2.Document = PD2
                    PPD2.ShowDialog()
                Case "Model 3 Tanpa Header"
                    PPD3.Document = PD3
                    PPD3.ShowDialog()
                Case "Model 4 Lengkap Tanpa Logo"
                    PPD1.Document = PD1
                    PPD1.ShowDialog()
                Case "Model 5 Tanpa Logo Tanpa Diskon"
                    PPD4.Document = PD4
                    PPD4.ShowDialog()
                Case Else
                    PPD.Document = PD
                    PPD.ShowDialog()
            End Select
        End If

        Close()
    End Sub


    Private Sub Btnsimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btnsimpan.Click
        Printerstruk()
    End Sub

    Private Sub DgvData_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellValueChanged
        If e.ColumnIndex = 2 Then 'kolom index 2
            HitungBarang()
        End If
    End Sub

    Private Sub DgvData_RowsAdded(ByVal sender As Object, ByVal e As DataGridViewRowsAddedEventArgs) Handles DgvData.RowsAdded
        HitungBarang()
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
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = @faktur", conn)
            cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                Do While rd.Read()
                    DgvData.Rows.Add(rd("ID_BARANG"), rd("NAMA_BARANG"), rd("QTY"), rd("SATUAN"), rd("HARGA_JUAL"), rd("TOTAL_DISKON"), rd("TOTAL_HARGA"))
                Loop
            End Using
        End Using
    End Sub

    Private Sub UrutkanNoDgvData()
        ' Loop melalui setiap baris di DataGridView
        Dim nomor As Integer = 1
        For Each row As DataGridViewRow In DgvData.Rows
            ' Pastikan baris bukan baris baru yang kosong
            If Not row.IsNewRow Then
                ' Cek jika kolom "Kode" berisi data
                If Not IsDBNull(row.Cells("Kode").Value) AndAlso row.Cells("Kode").Value.ToString() <> "" Then
                    ' Set kolom "No" dengan nomor urut
                    row.Cells("No").Value = nomor
                    nomor += 1
                Else
                    ' Jika kolom "Kode" kosong, kolom "No" dikosongkan
                    row.Cells("No").Value = Nothing
                End If
            End If
        Next
    End Sub


    Public Sub Ambildatapelanggan()
        Using cmd As New MySqlCommand("SELECT NAMA_PELANGGAN, JENIS_PELANGGAN, TGL_TRANSAKSI, GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BIAYA_KIRIM, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, TYPE_AKUN, KODE_AKUN, JENIS_PEMBAYARAN, METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE ID_PENJUALAN = @faktur", conn)
            cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    DTPTgl.Value = rd("TGL_TRANSAKSI")
                    CmbPelanggan.Text = If(IsDBNull(rd("NAMA_PELANGGAN")), "", rd("NAMA_PELANGGAN").ToString())
                    LblJenisPl.Text = If(IsDBNull(rd("JENIS_PELANGGAN")), "", rd("JENIS_PELANGGAN").ToString())
                    TxtSblPajak.Text = If(IsDBNull(rd("GRAND_TOTAL_SBL_PAJAK")), "0", Convert.ToDecimal(rd("GRAND_TOTAL_SBL_PAJAK")).ToString())
                    TxtDiskonRp.Text = If(IsDBNull(rd("DISKON_TOTAL_RP")), "0", Convert.ToDecimal(rd("DISKON_TOTAL_RP")).ToString())
                    TxtPajakRp.Text = If(IsDBNull(rd("PAJAK_RP")), "0", Convert.ToDecimal(rd("PAJAK_RP")).ToString())
                    TxtBiayaKirim.Text = If(IsDBNull(rd("BIAYA_KIRIM")), "0", Convert.ToDecimal(rd("BIAYA_KIRIM")).ToString())
                    TxtTotal.Text = If(IsDBNull(rd("GRAND_TOTAL_STL_PAJAK")), "0", Convert.ToDecimal(rd("GRAND_TOTAL_STL_PAJAK")).ToString())
                    TxtBayar.Text = If(IsDBNull(rd("BAYAR")), "0", Convert.ToDecimal(rd("BAYAR")).ToString())
                    TxtKembali.Text = If(IsDBNull(rd("KEMBALI")), "0", Convert.ToDecimal(rd("KEMBALI")).ToString())

                    ' Mengelola Sisa Tagihan dan Label Pembayaran
                    Dim sisaTagihanValue As Object = rd("SISA_TAGIHAN")
                    Dim sisaTagihanDecimal As Decimal

                    If sisaTagihanValue Is Nothing OrElse IsDBNull(sisaTagihanValue) OrElse Convert.ToDecimal(sisaTagihanValue) = 0D Then
                        LblPembayaran.Text = "Kembali :"
                    Else
                        sisaTagihanDecimal = Convert.ToDecimal(sisaTagihanValue)
                        TxtKembali.Text = sisaTagihanDecimal.ToString()
                        LblPembayaran.Text = "Hutang :"
                        DTPJatuhTempo.Value = If(IsDBNull(rd("JATUH_TEMPO")), "", Convert.ToDateTime(rd("JATUH_TEMPO")).ToString("yyyy-MM-dd"))
                    End If

                    TxtStatusTrans.Text = If(IsDBNull(rd("STATUS_TRANSAKSI")), "", rd("STATUS_TRANSAKSI").ToString())

                    ' Perhitungan untuk Bantuan Bayar
                    Dim total As Decimal = 0D
                    Dim bayar As Decimal = 0D

                    If Decimal.TryParse(TxtTotal.Text, total) AndAlso Decimal.TryParse(TxtBayar.Text, bayar) Then
                        Dim bantuanBayar As Decimal = total - bayar
                        TxtBAntuanbayar.Text = bantuanBayar.ToString()
                    Else
                        TxtBAntuanbayar.Text = "0"
                    End If

                    ' Data tambahan lainnya
                    TxtType.Text = If(IsDBNull(rd("TYPE_AKUN")), "", rd("TYPE_AKUN").ToString())
                    TxtKode.Text = If(IsDBNull(rd("KODE_AKUN")), "", rd("KODE_AKUN").ToString())
                    TxtPenerima.Text = If(IsDBNull(rd("JENIS_PEMBAYARAN")), "", rd("JENIS_PEMBAYARAN").ToString())
                    TxtMetode.Text = If(IsDBNull(rd("METODE")), "", rd("METODE").ToString())
                    TxtBank.Text = If(IsDBNull(rd("BANK")), "", rd("BANK").ToString())
                    TxtNoRek.Text = If(IsDBNull(rd("NO_REKENING")), "", rd("NO_REKENING").ToString())
                    TxtNamaRek.Text = If(IsDBNull(rd("NAMA_REKENING")), "", rd("NAMA_REKENING").ToString())
                    TxtNoReff.Text = If(IsDBNull(rd("NO_REFFERENSI")), "", rd("NO_REFFERENSI").ToString())

                    TxtIdUser.Text = If(IsDBNull(rd("ID_USER")), "", rd("ID_USER").ToString())
                    TxtIdKomputer.Text = If(IsDBNull(rd("ID_KOMPUTER")), "", rd("ID_KOMPUTER").ToString())
                End If
            End Using
        End Using
    End Sub


    Public Sub Printerstruk()
        Dim printer_nota As String

        If LblPrinterStrukString <> "Printerdefault" Then
            printer_nota = LblPrinterStrukString
        Else
            ' Langsung menetapkan default printer ke printer_nota tanpa variabel tambahan
            printer_nota = New PrinterSettings().PrinterName
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
            'PPD1.ShowDialog()
            PD1.Print()
        ElseIf MOdelStruk = "Model 5 Tanpa Logo Tanpa Diskon" Then
            Changelongpaper()
            PPD4.Document = PD4
            'PPD4.ShowDialog()
            PD4.Print()
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
        If TxtType.Text = "BANK" Then
            longpaper += (380 + TxtPanjangString)
        Else
            longpaper += (330 + TxtPanjangString)
        End If


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
        Dim garisdua As String = "===================="
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
        'Dim escapeCommand As String = Chr(27) & "J" & Chr(TxtMAjuString) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

        Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
        'tinggi += 10
        e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + (lebar + (25 / 100 * lebar)) - lebar

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Jual", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
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
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)


        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)

        tinggi += 5 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        'e.Graphics.DrawString("Sat", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Disc", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString
            Dim qtyValue As Decimal = 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)

            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString(), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)

            Dim hargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("Harga").Value.ToString(), hargaValue)
            e.Graphics.DrawString(hargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)

            Dim totalDiskonValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TotalDiskon").Value.ToString(), totalDiskonValue)
            e.Graphics.DrawString(totalDiskonValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)

            Dim totalHargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TotalHarga").Value.ToString(), totalHargaValue)
            e.Graphics.DrawString(totalHargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Decimal = TxtTotal.Text
        e.Graphics.DrawString(harga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Dim diskonRp As Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskonRp) AndAlso diskonRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Diskon :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Diskon As Decimal = TxtDiskonRp.Text
            e.Graphics.DrawString(Diskon.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim pajakRp As Decimal
        If Decimal.TryParse(TxtPajakRp.Text, pajakRp) AndAlso pajakRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Pajak :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Pajak As Decimal = TxtPajakRp.Text
            e.Graphics.DrawString(Pajak.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim biayaKirim As Decimal
        If Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) AndAlso biayaKirim <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Biaya Kirim :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(biayaKirim.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Bayar :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Tunai As Decimal
        Decimal.TryParse(TxtBayar.Text, Tunai)
        e.Graphics.DrawString(Tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Kembali As Decimal
        Decimal.TryParse(TxtKembali.Text, Kembali)
        e.Graphics.DrawString(Kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        If TxtStatusTrans.Text = "Belum Lunas" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "dd-MM-yyyy"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        If TxtType.Text = "BANK" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("R Penerima", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtPenerima.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("R Pengirim", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtBank.Text & " - " & TxtNamaRek.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("No Rek", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtNoRek.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("No Reff", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtNoReff.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)

        'tinggi -= TxtMundurString
        Dim escapeCommand As String = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
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
        Dim garisdua As String = "===================="
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
        'Dim escapeCommand As String = Chr(27) & "J" & Chr(TxtMAjuString) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

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


        Dim Mulaikata As Integer = TxtBatasKiriString + (lebar + (25 / 100 * lebar)) - lebar
        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Jual", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
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
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)


        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)

        tinggi += 5 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        'e.Graphics.DrawString("Sat", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Disc", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString

            Dim qtyValue As Decimal = 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)

            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString(), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)

            Dim hargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("Harga").Value.ToString(), hargaValue)
            e.Graphics.DrawString(hargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)

            Dim totalDiskonValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TotalDiskon").Value.ToString(), totalDiskonValue)
            e.Graphics.DrawString(totalDiskonValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)

            Dim totalHargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TotalHarga").Value.ToString(), totalHargaValue)
            e.Graphics.DrawString(totalHargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Decimal = TxtTotal.Text
        e.Graphics.DrawString(harga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Dim diskonRp As Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskonRp) AndAlso diskonRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Diskon :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Diskon As Decimal = TxtDiskonRp.Text
            e.Graphics.DrawString(Diskon.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim pajakRp As Decimal
        If Decimal.TryParse(TxtPajakRp.Text, pajakRp) AndAlso pajakRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Pajak :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Pajak As Decimal = TxtPajakRp.Text
            e.Graphics.DrawString(Pajak.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim biayaKirim As Decimal
        If Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) AndAlso biayaKirim <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Biaya Kirim :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(biayaKirim.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Bayar :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Tunai As Decimal
        Decimal.TryParse(TxtBayar.Text, Tunai)
        e.Graphics.DrawString(Tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Kembali As Decimal
        Decimal.TryParse(TxtKembali.Text, Kembali)
        e.Graphics.DrawString(Kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        If TxtStatusTrans.Text = "Belum Lunas" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "dd-MM-yyyy"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        If TxtType.Text = "BANK" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("R Penerima", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtPenerima.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("R Pengirim", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtBank.Text & " - " & TxtNamaRek.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("No Rek", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtNoRek.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("No Reff", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & TxtNoReff.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)

        'tinggi -= TxtMundurString
        Dim escapeCommand As String = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
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

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String = "-------------------------------------------"
        Dim garisdua As String = "============="
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
        'tinggi += 10
        e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + (lebar + (25 / 100 * lebar)) - lebar

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Jual", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tgl", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pel", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

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
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 14 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString
            Dim qtyValue As Decimal = 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
            e.Graphics.DrawString(Convert.ToDecimal(DgvData.Rows(baris).Cells("Harga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Convert.ToDecimal(DgvData.Rows(baris).Cells("TotalHarga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Decimal = TxtTotal.Text
        e.Graphics.DrawString(harga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Dim diskonRp As Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskonRp) AndAlso diskonRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Diskon :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Diskon As Decimal = TxtDiskonRp.Text
            e.Graphics.DrawString(Diskon.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim pajakRp As Decimal
        If Decimal.TryParse(TxtPajakRp.Text, pajakRp) AndAlso pajakRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Pajak :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Pajak As Decimal = TxtPajakRp.Text
            e.Graphics.DrawString(Pajak.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim biayaKirim As Decimal
        If Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) AndAlso biayaKirim <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Biaya Kirim :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(biayaKirim.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Bayar :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Tunai As Decimal
        Decimal.TryParse(TxtBayar.Text, Tunai)
        e.Graphics.DrawString(Tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Kembali As Decimal
        Decimal.TryParse(TxtKembali.Text, Kembali)
        e.Graphics.DrawString(Kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        If TxtStatusTrans.Text = "Belum Lunas" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "dd-MM-yyyy"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        'tinggi -= TxtMundurString

        Dim escapeCommand As String = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
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
        Dim garisdua As String = "============="
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
        'tinggi += 10
        e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + (lebar + (25 / 100 * lebar)) - lebar

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Jual", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tgl", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pel", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)


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

            Dim qtyValue As Decimal = 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)

            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
            e.Graphics.DrawString(Convert.ToDecimal(DgvData.Rows(baris).Cells("Harga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Convert.ToDecimal(DgvData.Rows(baris).Cells("TotalHarga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Decimal = TxtTotal.Text
        e.Graphics.DrawString(harga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Dim diskonRp As Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskonRp) AndAlso diskonRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Diskon :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Diskon As Decimal = TxtDiskonRp.Text
            e.Graphics.DrawString(Diskon.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim pajakRp As Decimal
        If Decimal.TryParse(TxtPajakRp.Text, pajakRp) AndAlso pajakRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Pajak :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            Dim Pajak As Decimal = TxtPajakRp.Text
            e.Graphics.DrawString(Pajak.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim biayaKirim As Decimal
        If Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) AndAlso biayaKirim <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Biaya Kirim :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(biayaKirim.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Bayar :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Tunai As Decimal
        Decimal.TryParse(TxtBayar.Text, Tunai)
        e.Graphics.DrawString(Tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim Kembali As Decimal
        Decimal.TryParse(TxtKembali.Text, Kembali)
        e.Graphics.DrawString(Kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


        If TxtStatusTrans.Text = "Belum Lunas" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "dd-MM-yyyy"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)

        'tinggi -= TxtMundurString

        Dim escapeCommand As String = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
    End Sub


    Private Sub PD4_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD4.BeginPrint
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        PD4.DefaultPageSettings.PaperSize = ps
        PD4.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD4_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD4.PrintPage
        Dim leftmargin As Integer = PD4.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD4.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD4.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String = "-------------------------------------------"    ' TANPA LOGO TANPA DISKON
        Dim garisdua As String = "============="
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
        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + (lebar + (25 / 100 * lebar)) - lebar

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Jual", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tgl", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pel", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (65 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (98 / 100 * lebar)) - lebar)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri + 5, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)



        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 14 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString

            Dim qtyValue As Decimal = 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri + 5, tinggi)

            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)

            ' Variabel untuk menyimpan hasil parsing
            Dim hargaBarang As Decimal = 0
            Dim totalHarga As Decimal = 0

            ' Coba konversi nilai dari DGV menjadi Decimal, jika gagal tetap 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("Harga").Value.ToString(), hargaBarang)
            Decimal.TryParse(DgvData.Rows(baris).Cells("TotalHarga").Value.ToString(), totalHarga)

            ' Format hasilnya dan cetak ke grafik
            e.Graphics.DrawString(hargaBarang.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalHarga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        Next

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)

        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        Dim harga As Decimal = 0

        ' Coba konversi TxtTotal.Text menjadi Decimal, jika gagal tetap 0
        Decimal.TryParse(TxtTotal.Text, harga)

        ' Format hasilnya dan cetak ke grafik
        e.Graphics.DrawString(harga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


        Dim diskonRp As Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskonRp) AndAlso diskonRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Diskon :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(diskonRp.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim pajakRp As Decimal
        If Decimal.TryParse(TxtPajakRp.Text, pajakRp) AndAlso pajakRp <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Pajak :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(pajakRp.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim biayaKirim As Decimal
        If Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) AndAlso biayaKirim <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Biaya Kirim :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(biayaKirim.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Bayar :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)

        Dim tunai As Decimal
        If Decimal.TryParse(TxtBayar.Text, tunai) Then
            e.Graphics.DrawString(tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)

        Dim kembali As Decimal
        If Decimal.TryParse(TxtKembali.Text, kembali) Then
            e.Graphics.DrawString(kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If


        If TxtStatusTrans.Text = "Belum Lunas" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "dd-MM-yyyy"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi -= TxtMundurString

        Dim escapeCommand As String = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh ... baris
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


    Public Sub PrinterDotMatrik()
        Dim printer_nota As String

        If printerDot <> "Printerdefault" Then
            printer_nota = printerDot
        Else
            ' Baca default printer pada komputer
            Dim defaultPrinter As String = New PrinterSettings().PrinterName
            printer_nota = defaultPrinter
        End If

        PDDot.PrinterSettings.PrinterName = printer_nota
        RubahPanjangkertas()
        PPDDot.Document = PDDot
        'PPDDot.ShowDialog()
        PDDot.Print()
    End Sub

    Public Sub RubahPanjangkertas()
        Dim TinggiKertas As Integer = CInt((TinggiDot * 0.3937) * 72) ' Tinggi dalam dot
        'Dim TinggiKertas As Integer = 70
        Dim rowcount As Integer
        Panjangkertas = 0
        rowcount = DgvData.Rows.Count
        Panjangkertas = TinggiKertas
        Panjangkertas += rowcount * 20

        LebarKertas = CInt((lebarDot * 0.3937) * 72) ' Lebar dalam dot
    End Sub


    Private Sub PDDot_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PDDot.BeginPrint
        ' Lebar dan Tinggi kertas dalam dot (1 inch = 2.54 cm, 1 inch = 72 dot)

        Dim ps As New PaperSize("Custom", LebarKertas, Panjangkertas)
        PDDot.DefaultPageSettings.PaperSize = ps
        PDDot.DefaultPageSettings.Landscape = False
    End Sub


    Private Sub PDDot_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PDDot.PrintPage
        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        'Dim garis As String = "------------------------------------------------------------------------------------------------------------"

        ' Asumsikan lebar satu karakter rata-rata adalah 8 piksel
        Dim lebarKarakter As Decimal = 7.35
        Dim jumlahKarakter As Integer = CInt(Math.Floor(LebarKertas / lebarKarakter))

        ' Buat garis berdasarkan jumlah karakter
        Dim garis As String = New String("-"c, jumlahKarakter)


        Dim TopRight As New StringFormat With {
    .LineAlignment = StringAlignment.Near,
    .Alignment = StringAlignment.Far
    }

        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + batasKiriDot

        Dim Mulaikata1 As Integer = BatasKiri + (LebarKertas * 5 / 100)
        Dim Mulaikata2 As Integer = BatasKiri + (LebarKertas * 20 / 100)
        Dim Mulaikata3 As Integer = BatasKiri + (LebarKertas * 35 / 100)

        Dim Mulaikata5 As Integer = BatasKiri + (LebarKertas * 50 / 100)
        Dim Mulaikata6 As Integer = BatasKiri + (LebarKertas * 68 / 100)
        Dim Mulaikata7 As Integer = BatasKiri + (LebarKertas * 80 / 100)
        Dim Mulaikata8 As Integer = BatasKiri + (LebarKertas * 93 / 100)


        'tinggi -= TxtMAjuString
        'Dim escapeCommandsebelum As String = Chr(27) & "J" & Chr(TxtMAjuString) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(fontJudulDot, ukuranFontJudul, FontStyle.Bold), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("N O T A  P E N J U A L A N", New Drawing.Font(fontJudulDot, ukuranFontJudul, FontStyle.Bold), Brushes.Black, Mulaikata5, tinggi)

        tinggi += 20 + jarakBarisDot
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN & " " & KOTA_PERUSAHAAN, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Trx : " & TxtFaktur.Text & " / " & Microsoft.VisualBasic.Format(DTPTgl.Value, "dd-MM-yy hh:mm:ss"), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)


        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Pel : " & LblJenisPl.Text & " " & CmbPelanggan.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)

        tinggi += 14 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString("No", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi)
        e.Graphics.DrawString("Qty", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi, kanan)
        e.Graphics.DrawString("Sat", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata6, tinggi, kanan)
        e.Graphics.DrawString("Disc", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)

        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Print each row from DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso Not String.IsNullOrEmpty(row.Cells("NamaBarang").Value.ToString()) Then
                tinggi += 10 + jarakBarisDot
                Dim rowIndex As Integer = row.Index + 1 ' Assuming row index starts from 1
                e.Graphics.DrawString(rowIndex.ToString() & ". ", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("NamaBarang").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("QTY").Value).ToString("#,0", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("Satuan").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("Harga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata6, tinggi, kanan)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("TotalDiskon").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("TotalHarga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
            End If
        Next


        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + jarakBarisDot

        Dim totalValue As Decimal
        ' Coba untuk mengonversi TxtTotal.Text ke tipe data Decimal
        If Decimal.TryParse(TxtTotal.Text, totalValue) Then
            ' Menuliskan terbilang dengan nilai yang sudah dikonversi
            e.Graphics.DrawString("Terbilang : " & Terbilang(totalValue), New Drawing.Font("Arial Narrow", 8, FontStyle.Italic), Brushes.Black, BatasKiri, tinggi)
        End If

        ' Menuliskan Total
        e.Graphics.DrawString("Total :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim Total As Decimal
        If Decimal.TryParse(TxtSblPajak.Text, Total) Then
            e.Graphics.DrawString(Total.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        tinggi += 10 + jarakBarisDot

        ' Menuliskan Diskon jika ada
        Dim diskonRp As Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskonRp) Then
            e.Graphics.DrawString("Diskon :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
            e.Graphics.DrawString(diskonRp.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        ' Menuliskan Hormat Kami, Penerima, dan Kasir
        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString("Hormat kami", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Penerima", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Kasir", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)

        ' Menuliskan Pajak jika ada
        Dim pajakRp As Decimal
        If Decimal.TryParse(TxtPajakRp.Text, pajakRp) Then
            e.Graphics.DrawString("Pajak :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
            e.Graphics.DrawString(pajakRp.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        Dim biayaKirim As Decimal
        If Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) AndAlso biayaKirim <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Biaya Kirim :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
            e.Graphics.DrawString(biayaKirim.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If


        ' Menuliskan Total Akhir
        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString("Total :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim TotalAkhir As Decimal
        If Decimal.TryParse(TxtTotal.Text, TotalAkhir) Then
            e.Graphics.DrawString(TotalAkhir.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        ' Menuliskan garis-garis
        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString(".....", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(".....", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString(TxtIdUser.Text & "\" & TxtIdKomputer.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)

        ' Menuliskan Pembayaran
        e.Graphics.DrawString("Bayar :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim Tunai As Decimal
        If Decimal.TryParse(TxtBayar.Text, Tunai) Then
            e.Graphics.DrawString(Tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        ' Menuliskan label pembayaran dan kembali jika ada
        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim Kembali As Decimal
        If Decimal.TryParse(TxtKembali.Text, Kembali) Then
            e.Graphics.DrawString(Kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        ' Menambah tinggi untuk jarak antar baris terakhir
        tinggi += 10 + jarakBarisDot
        ' Menuliskan footer
        e.Graphics.DrawString(FOOTER2 & " " & FOOTER1, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)

        ' Menuliskan Jatuh Tempo jika status transaksi belum lunas
        If TxtStatusTrans.Text = "Belum Lunas" Then
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
            e.Graphics.DrawString(Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "dd-MM-yyyy"), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        'Dim escapeCommandsesudah As String = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
    End Sub



End Class