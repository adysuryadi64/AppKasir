Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Ports

Public Class FormCetakJual
    Private PrinterPosString As String
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
    Private lblDataBitsString As Integer
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

    Dim printerDot As String
    Dim lebarDot As Integer
    Dim TinggiDot As Integer
    Dim batasKiriDot As Integer
    Dim jarakBarisDot As Integer
    Dim fontJudulDot As String
    Dim fontIsiDot As String
    Dim ukuranFontJudul As Integer
    Dim ukuranFontIsi As Integer

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
        If Not File.Exists(filePath) Then Exit Sub

        Using reader As New StreamReader(filePath)
            Dim line As String = reader.ReadLine()
            While line IsNot Nothing
                Dim parts As String() = line.Split("="c)
                If parts.Length = 2 Then
                    Dim key As String = parts(0)
                    Dim value As String = parts(1)

                    Select Case key
                        Case "PrinterPos" : PrinterPosString = value
                        Case "JenisPrinterJual" : jenisprinter = value
                        Case "Maju" : TxtMAjuString = value
                        Case "Mundur" : TxtMundurString = value
                        Case "Panjang" : TxtPanjangString = value
                        Case "Lebar" : TxtLebarString = value
                        Case "Piksel" : TxtpikselString = value
                        Case "BatasKiri" : TxtBatasKiriString = value
                        Case "Jarak" : TxtJarakString = value
                        Case "PortName" : lblPortNameString = value
                        Case "BaudRate" : lblBaudRateString = value
                        Case "DataBits" : lblDataBitsString = value
                        Case "PortCashDraw" : CmbPortCashString = value
                        Case "CodeCashDraw" : CmbCodeCashString = value
                        Case "ModelStruk" : CmbModelStrukString = value
                        Case "FontNama" : CmbFNAmaString = value
                        Case "FontKet" : CmbFKetString = value
                        Case "FontIsi" : CmbFIsiString = value
                        Case "FOntFoot" : CmbFFootString = value
                        Case "FontUNama" : CmbUNamaString = value
                        Case "FontUKet" : CmbUKetString = value
                        Case "FontUIsi" : CmbUIsiString = value
                        Case "FontUFoot" : CmbUFootString = value
                        Case "PrinterDot" : printerDot = value
                        Case "LebarDot" : lebarDot = value
                        Case "TinggiDot" : TinggiDot = value
                        Case "BatasKiriDot" : batasKiriDot = value
                        Case "JarakBarisDot" : jarakBarisDot = value
                        Case "FontJudulDot" : fontJudulDot = value
                        Case "FontIsiDot" : fontIsiDot = value
                        Case "UkuranFontJudul" : ukuranFontJudul = value
                        Case "UkuranFontIsi" : ukuranFontIsi = value
                    End Select
                End If
                line = reader.ReadLine()
            End While
        End Using
    End Sub

    Public Sub ProsesCetak(ByVal pilihankertas As String)
        Ambildataprinter()
        Ambil_data()
        UrutkanNoDgvData()
        Ambildatapelanggan()

        Select Case pilihankertas
            Case "Printer Thermal" : Printerstruk()
            Case "Printer Dot Matrix" : PrinterDotMatrik()
            Case Else
                Select Case jenisprinter
                    Case "Printer Thermal" : Printerstruk()
                    Case "Printer Dot Matrix" : PrinterDotMatrik()
                    Case Else
                        MOdelStruk = CmbModelStrukString
                        Changelongpaper()
                        ShowPreviewBasedOnModel()
                End Select
        End Select
        Close()
    End Sub

    Private Sub ShowPreviewBasedOnModel()
        Select Case MOdelStruk
            Case "Model 2 Tanpa Diskon" : PPD2.Document = PD2 : PPD2.ShowDialog()
            Case "Model 3 Tanpa Header" : PPD3.Document = PD3 : PPD3.ShowDialog()
            Case "Model 4 Lengkap Tanpa Logo" : PPD1.Document = PD1 : PPD1.ShowDialog()
            Case "Model 5 Tanpa Logo Tanpa Diskon" : PPD4.Document = PD4 : PPD4.ShowDialog()
            Case Else : PPD.Document = PD : PPD.ShowDialog()
        End Select
    End Sub

    Private Sub Btnsimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btnsimpan.Click
        Printerstruk()
    End Sub

    Private Sub DgvData_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellValueChanged
        If e.ColumnIndex = 2 Then HitungBarang()
    End Sub

    Private Sub DgvData_RowsAdded(ByVal sender As Object, ByVal e As DataGridViewRowsAddedEventArgs) Handles DgvData.RowsAdded
        HitungBarang()
    End Sub

    Public Sub HitungBarang()
        TxtJmlhBrg.Text = If(DgvData IsNot Nothing AndAlso DgvData.RowCount > 0, (DgvData.RowCount - 1).ToString(), "0")
    End Sub

    Public Sub Ambil_data()
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, SERIAL_NUMBER, QTY, SATUAN, HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = @faktur", conn)
            cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                While rd.Read()
                    DgvData.Rows.Add(rd("ID_BARANG"), rd("NAMA_BARANG"), rd("QTY"), rd("SATUAN"), rd("HARGA_JUAL"), rd("TOTAL_DISKON"), rd("TOTAL_HARGA"), rd("SERIAL_NUMBER"))
                End While
            End Using
        End Using
    End Sub

    Private Sub UrutkanNoDgvData()
        Dim nomor As Integer = 1
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                If Not IsDBNull(row.Cells("Kode").Value) AndAlso row.Cells("Kode").Value.ToString() <> "" Then
                    row.Cells("No").Value = nomor
                    nomor += 1
                Else
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

                    Dim sisaTagihanValue As Object = rd("SISA_TAGIHAN")
                    If sisaTagihanValue Is Nothing OrElse IsDBNull(sisaTagihanValue) OrElse Convert.ToDecimal(sisaTagihanValue) = 0D Then
                        LblPembayaran.Text = "Kembali :"
                    Else
                        TxtKembali.Text = Convert.ToDecimal(sisaTagihanValue).ToString()
                        LblPembayaran.Text = "Hutang :"
                        DTPJatuhTempo.Value = If(IsDBNull(rd("JATUH_TEMPO")), "", Convert.ToDateTime(rd("JATUH_TEMPO")).ToString("yyyy-MM-dd"))
                    End If

                    TxtStatusTrans.Text = If(IsDBNull(rd("STATUS_TRANSAKSI")), "", rd("STATUS_TRANSAKSI").ToString())

                    Dim total As Decimal = 0D
                    Dim bayar As Decimal = 0D
                    If Decimal.TryParse(TxtTotal.Text, total) AndAlso Decimal.TryParse(TxtBayar.Text, bayar) Then
                        TxtBAntuanbayar.Text = (total - bayar).ToString()
                    Else
                        TxtBAntuanbayar.Text = "0"
                    End If

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

    Private Function AmbilPrinterStruk() As String
        Dim targetPrinter As String = If(LblPrinterStrukString <> "Printerdefault", LblPrinterStrukString, New PrinterSettings().PrinterName)
        If PrinterSettings.InstalledPrinters.Cast(Of String)().Any(Function(p) p.Equals(targetPrinter, StringComparison.OrdinalIgnoreCase)) Then Return targetPrinter

        Dim kandidat = PrinterSettings.InstalledPrinters.Cast(Of String)().FirstOrDefault(Function(p) p.ToLower().Contains(targetPrinter.ToLower()))
        If kandidat IsNot Nothing Then
            MessageBox.Show($"Printer '{targetPrinter}' tidak ditemukan. Diganti dengan: {kandidat}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return kandidat
        Else
            MessageBox.Show($"Printer '{targetPrinter}' tidak ditemukan!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End If
    End Function

    Public Sub Printerstruk()
        Dim printer_nota_pos As String = PrinterPosString
        If String.IsNullOrEmpty(printer_nota_pos) Then
            MessageBox.Show("Tidak ada printer yang tersedia untuk mencetak nota.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        PD.PrinterSettings.PrinterName = printer_nota_pos
        MOdelStruk = CmbModelStrukString
        Changelongpaper()
        PrintBasedOnModel()
    End Sub

    Private Sub PrintBasedOnModel()
        Select Case MOdelStruk
            Case "Model 2 Tanpa Diskon" : PPD2.Document = PD2 : PD2.Print()
            Case "Model 3 Tanpa Header" : PPD3.Document = PD3 : PD3.Print()
            Case "Model 4 Lengkap Tanpa Logo" : PPD1.Document = PD1 : PD1.Print()
            Case "Model 5 Tanpa Logo Tanpa Diskon" : PPD4.Document = PD4 : PD4.Print()
            Case Else : PPD.Document = PD : PD.Print()
        End Select
    End Sub

    Public Sub Changelongpaper()
        Dim rowcount As Integer = DgvData.Rows.Count
        longpaper = rowcount * 30
        longpaper += If(TxtType.Text = "BANK", 380, 330) + TxtPanjangString
    End Sub

    Private Sub SetupPrintDocument(ByVal pd As PrintDocument)
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        pd.DefaultPageSettings.PaperSize = ps
        pd.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD.BeginPrint
        SetupPrintDocument(PD)
    End Sub

    Private Sub PD1_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD1.BeginPrint
        SetupPrintDocument(PD1)
    End Sub

    Private Sub PD2_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD2.BeginPrint
        SetupPrintDocument(PD2)
    End Sub

    Private Sub PD3_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD3.BeginPrint
        SetupPrintDocument(PD3)
    End Sub

    Private Sub PD4_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD4.BeginPrint
        SetupPrintDocument(PD4)
    End Sub

    Private Sub PrintCommonContent(ByVal e As PrintPageEventArgs, ByVal withLogo As Boolean)
        Dim kanan As New StringFormat : kanan.Alignment = StringAlignment.Far
        Dim tengah As New StringFormat : tengah.Alignment = StringAlignment.Center
        Dim garis As String = "-------------------------------------------"
        Dim garisdua As String = "===================="

        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim lebar As Integer = thermalPaperWidthInPixel
        Dim tinggi As Integer = 10 - TxtMAjuString
        Dim BatasKiri As Integer = 2 + TxtBatasKiriString

        If withLogo Then
            Dim logoImage As Image = Image.FromFile(Application.StartupPath() & "\logo.Png")
            e.Graphics.DrawImage(logoImage, CInt((e.PageBounds.Width - 150) / 2), 5, 150, 35)
            tinggi += 30 + TxtJarakString
        End If

        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, e.PageBounds.Width / 2, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, e.PageBounds.Width / 2, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, e.PageBounds.Width / 2, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(KONTAK_PERUSAHAAN, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, e.PageBounds.Width / 2, tinggi, tengah)

        Dim Mulaikata As Integer = TxtBatasKiriString + (lebar + (25 / 100 * lebar)) - lebar
        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Nota Jual", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtFaktur.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & TxtIdUser.Text & " - " & TxtIdKomputer.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Pelanggan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & LblJenisPl.Text & " - " & CmbPelanggan.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        PrintItems(e, tinggi, kanan, lebar, BatasKiri, withDetails:=True)
        PrintFooter(e, tinggi, kanan, lebar, BatasKiri, Mulaikata)
    End Sub

    Private Sub PrintItems(ByVal e As PrintPageEventArgs, ByRef tinggi As Integer, ByVal kanan As StringFormat, ByVal lebar As Integer, ByVal BatasKiri As Integer, ByVal withDetails As Boolean)
        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (70 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)

        tinggi += 5 + TxtJarakString
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Qty", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)
        If withDetails Then
            e.Graphics.DrawString("Harga", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString("Disc", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        End If
        e.Graphics.DrawString("Jumlah", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString("-------------------------------------------", New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        For baris As Integer = 0 To DgvData.RowCount - 2
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("NamaBarang").Value.ToString, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            tinggi += 10 + TxtJarakString

            Dim qtyValue As Decimal = 0
            Decimal.TryParse(DgvData.Rows(baris).Cells("QTY").Value.ToString(), qtyValue)
            e.Graphics.DrawString(qtyValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata1, tinggi, kanan)
            e.Graphics.DrawString(DgvData.Rows(baris).Cells("Satuan").Value.ToString(), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata2, tinggi)

            If withDetails Then
                Dim hargaValue As Decimal, totalDiskonValue As Decimal
                Decimal.TryParse(DgvData.Rows(baris).Cells("Harga").Value.ToString(), hargaValue)
                Decimal.TryParse(DgvData.Rows(baris).Cells("TotalDiskon").Value.ToString(), totalDiskonValue)
                e.Graphics.DrawString(hargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
                e.Graphics.DrawString(totalDiskonValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            End If

            Dim totalHargaValue As Decimal
            Decimal.TryParse(DgvData.Rows(baris).Cells("TotalHarga").Value.ToString(), totalHargaValue)
            e.Graphics.DrawString(totalHargaValue.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        Next
    End Sub

    Private Sub PrintFooter(ByVal e As PrintPageEventArgs, ByRef tinggi As Integer, ByVal kanan As StringFormat, ByVal lebar As Integer, ByVal BatasKiri As Integer, ByVal Mulaikata As Integer)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (51 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (95 / 100 * lebar)) - lebar)
        Dim garis As String = "-------------------------------------------"
        Dim garisdua As String = "===================="

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + TxtJarakString
        e.Graphics.DrawString(TxtJmlhBrg.Text & " item", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Total :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)

        Dim harga As Decimal = TxtTotal.Text
        e.Graphics.DrawString(harga.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        PrintOptionalLine(e, TxtDiskonRp.Text, "Diskon :", tinggi, Mulaikata3, Mulaikata5)
        PrintOptionalLine(e, TxtPajakRp.Text, "Pajak :", tinggi, Mulaikata3, Mulaikata5)
        PrintOptionalLine(e, TxtBiayaKirim.Text, "Biaya Kirim :", tinggi, Mulaikata3, Mulaikata5)

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
            e.Graphics.DrawString(DTPJatuhTempo.Value.ToString("dd-MM-yyyy"), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        If TxtType.Text = "BANK" Then
            PrintBankDetails(e, tinggi, BatasKiri, Mulaikata)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, e.PageBounds.Width / 2, tinggi, New StringFormat With {.Alignment = StringAlignment.Center})
        tinggi += 12 + TxtJarakString
        e.Graphics.DrawString(FOOTER2, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, e.PageBounds.Width / 2, tinggi, New StringFormat With {.Alignment = StringAlignment.Center})

        Dim escapeCommand As String = Chr(27) & "d" & Chr(TxtMundurString)
    End Sub

    Private Sub PrintOptionalLine(e As PrintPageEventArgs, valueText As String, label As String, ByRef tinggi As Integer, mulaikata3 As Integer, mulaikata5 As Integer)
        Dim value As Decimal
        If Decimal.TryParse(valueText, value) AndAlso value <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString(label, New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, mulaikata3, tinggi, New StringFormat With {.Alignment = StringAlignment.Far})
            e.Graphics.DrawString(value.ToString("#,0.##", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, mulaikata5, tinggi, New StringFormat With {.Alignment = StringAlignment.Far})
        End If
    End Sub

    Private Sub PrintBankDetails(e As PrintPageEventArgs, ByRef tinggi As Integer, BatasKiri As Integer, Mulaikata As Integer)
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
        e.Graphics.DrawString("-------------------------------------------", New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)
    End Sub

    Private Sub PD_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD.PrintPage
        PrintCommonContent(e, withLogo:=True)
    End Sub

    Private Sub PD1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD1.PrintPage
        PrintCommonContent(e, withLogo:=False)
    End Sub

    Private Sub PD2_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD2.PrintPage
        PrintCommonContent(e, withLogo:=True)
    End Sub

    Private Sub PD3_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD3.PrintPage
        PrintCommonContent(e, withLogo:=True)
    End Sub

    Private Sub PD4_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD4.PrintPage
        PrintCommonContent(e, withLogo:=False)
    End Sub

    Public Sub Bukalaci()
        Try
            If CmbCodeCashString = "OPTION 1" OrElse CmbCodeCashString = "OPTION 2" Then
                Using port As New IO.Ports.SerialPort(CmbPortCashString, lblBaudRateString, IO.Ports.Parity.None, lblDataBitsString, IO.Ports.StopBits.One)
                    port.Open()
                    If CmbCodeCashString = "OPTION 1" Then
                        port.Write(Chr(&H1B) & Chr(&H70) & Chr(&H0) & Chr(&H50) & Chr(&H50))
                    Else
                        Dim openDrawer As Byte() = {&H1B, &H70, &H0, &H50, &H50}
                        port.Write(openDrawer, 0, openDrawer.Length)
                    End If
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
        Dim printer_nota_Dot As String = printerDot
        If String.IsNullOrEmpty(printer_nota_Dot) Then
            MessageBox.Show("Tidak ada printer yang tersedia untuk mencetak nota.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        PDDot.PrinterSettings.PrinterName = printer_nota_Dot
        RubahPanjangkertas()
        PPDDot.Document = PDDot
        PDDot.Print()
    End Sub

    Public Sub RubahPanjangkertas()
        Dim TinggiKertas As Integer = CInt((TinggiDot * 0.3937) * 72)
        Dim rowcount As Integer = DgvData.Rows.Count
        Panjangkertas = TinggiKertas + rowcount * 20
        LebarKertas = CInt((lebarDot * 0.3937) * 72)
    End Sub

    Private Sub PDDot_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PDDot.BeginPrint
        Dim ps As New PaperSize("Custom", LebarKertas, Panjangkertas)
        PDDot.DefaultPageSettings.PaperSize = ps
        PDDot.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PDDot_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PDDot.PrintPage
        Dim kanan As New StringFormat : kanan.Alignment = StringAlignment.Far
        Dim tengah As New StringFormat : tengah.Alignment = StringAlignment.Center

        Dim lebarKarakter As Decimal = 7.35
        Dim jumlahKarakter As Integer = CInt(Math.Floor(LebarKertas / lebarKarakter))
        Dim garis As String = New String("-"c, jumlahKarakter)

        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + batasKiriDot

        Dim Mulaikata1 As Integer = BatasKiri + (LebarKertas * 5 / 100)
        Dim Mulaikata2 As Integer = BatasKiri + (LebarKertas * 20 / 100)
        Dim Mulaikata3 As Integer = BatasKiri + (LebarKertas * 35 / 100)
        Dim Mulaikata5 As Integer = BatasKiri + (LebarKertas * 50 / 100)
        Dim Mulaikata6 As Integer = BatasKiri + (LebarKertas * 68 / 100)
        Dim Mulaikata7 As Integer = BatasKiri + (LebarKertas * 80 / 100)
        Dim Mulaikata8 As Integer = BatasKiri + (LebarKertas * 93 / 100)

        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(fontJudulDot, ukuranFontJudul, FontStyle.Bold), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("N O T A  P E N J U A L A N", New Drawing.Font(fontJudulDot, ukuranFontJudul, FontStyle.Bold), Brushes.Black, Mulaikata5, tinggi)

        tinggi += 20 + jarakBarisDot
        e.Graphics.DrawString(ALAMAT_PERUSAHAAN, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Trx : " & TxtFaktur.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)

        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString(KOTA_PERUSAHAAN, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Tgl : " & DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)

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

        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso Not String.IsNullOrEmpty(row.Cells("NamaBarang").Value.ToString()) Then
                tinggi += 10 + jarakBarisDot
                Dim rowIndex As Integer = row.Index + 1
                e.Graphics.DrawString(rowIndex.ToString() & ". ", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("NamaBarang").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("QTY").Value).ToString("#,0", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("Satuan").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("Harga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata6, tinggi, kanan)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("TotalDiskon").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("TotalHarga").Value).ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)

                If row.Cells("SN").Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(row.Cells("SN").Value.ToString()) Then
                    tinggi += 10 + jarakBarisDot
                    e.Graphics.DrawString("SN: " & row.Cells("SN").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi)
                End If
            End If
        Next

        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + jarakBarisDot
        Dim totalValue As Decimal
        If Decimal.TryParse(TxtTotal.Text, totalValue) Then
            e.Graphics.DrawString("Terbilang : " & Terbilang(totalValue), New Drawing.Font("Arial Narrow", 8, FontStyle.Italic), Brushes.Black, BatasKiri, tinggi)
        End If

        e.Graphics.DrawString("Total :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim Total As Decimal
        If Decimal.TryParse(TxtSblPajak.Text, Total) Then
            e.Graphics.DrawString(Total.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        tinggi += 10 + jarakBarisDot
        PrintOptionalDotLine(e, TxtDiskonRp.Text, "Diskon :", tinggi, Mulaikata7, Mulaikata8)
        PrintOptionalDotLine(e, TxtPajakRp.Text, "Pajak :", tinggi, Mulaikata7, Mulaikata8)
        PrintOptionalDotLine(e, TxtBiayaKirim.Text, "Biaya Kirim :", tinggi, Mulaikata7, Mulaikata8)

        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString("Total :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim TotalAkhir As Decimal
        If Decimal.TryParse(TxtTotal.Text, TotalAkhir) Then
            e.Graphics.DrawString(TotalAkhir.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString("Bayar :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim Tunai As Decimal
        If Decimal.TryParse(TxtBayar.Text, Tunai) Then
            e.Graphics.DrawString(Tunai.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString(LblPembayaran.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
        Dim Kembali As Decimal
        If Decimal.TryParse(TxtKembali.Text, Kembali) Then
            e.Graphics.DrawString(Kembali.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If

        e.Graphics.DrawString(".....", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(".....", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString(TxtIdUser.Text & "\" & TxtIdKomputer.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 20 + jarakBarisDot
        e.Graphics.DrawString(FOOTER1, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString(FOOTER3, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)

        If TxtStatusTrans.Text = "Belum Lunas" Then
            e.Graphics.DrawString("Jatuh Tempo :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata7, tinggi, kanan)
            e.Graphics.DrawString(DTPJatuhTempo.Value.ToString("dd-MM-yyyy"), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata8, tinggi, kanan)
        End If
    End Sub

    Private Sub PrintOptionalDotLine(e As PrintPageEventArgs, valueText As String, label As String, ByRef tinggi As Integer, mulaikata7 As Integer, mulaikata8 As Integer)
        Dim value As Decimal
        If Decimal.TryParse(valueText, value) Then
            e.Graphics.DrawString(label, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, mulaikata7, tinggi, New StringFormat With {.Alignment = StringAlignment.Far})
            e.Graphics.DrawString(value.ToString("#,0.##", cultureIndonesia), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, mulaikata8, tinggi, New StringFormat With {.Alignment = StringAlignment.Far})
            tinggi += 10 + jarakBarisDot
        End If
    End Sub
End Class