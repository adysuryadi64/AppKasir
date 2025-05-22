Imports System.Drawing.Printing
Imports System.IO


Public Class PrinterSuratJalan
    Private jenisprinter As String

    Dim printerDot As String
    Dim lebarDot As Integer
    Dim TinggiDot As Integer
    Dim batasKiriDot As Integer
    Dim jarakBarisDot As Integer
    Dim fontJudulDot As String
    Dim fontIsiDot As String
    Dim ukuranFontJudul As Integer
    Dim ukuranFontIsi As Integer


    ' Define variables to hold data from Surat_Jalan
    Dim tglPengiriman As Date
    Dim totalRupiah As Decimal
    Dim TerbilangTotal As String
    Dim armada As String = String.Empty
    Dim supir As String = String.Empty
    Dim helper1 As String = String.Empty
    Dim helper2 As String = String.Empty
    Dim idUser As String = String.Empty



    Public WithEvents PDDot As New PrintDocument
    Private ReadOnly PPDDot As New PrintPreviewDialog


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
                            Case "JenisPrinterJual"
                                jenisprinter = value
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

        AmbilaDataSuratJalan(TxtNota.Text)
        If jenisprinter = "Printer Dot Matrix" Then
            PrinterDotMatrik()
        Else
            NotaSuratJalan.AmbilaDataSuratJalan(TxtNota.Text)
        End If

        Close()
    End Sub

    Public Sub AmbilaDataSuratJalan(ByVal NOTA As String)

        Dim querySuratJalanDetail As String = "SELECT NOTA_BELANJA, NAMA_PELANGGAN, ALAMAT_PELANGGAN, NILAI_BELANJA, LOKASI FROM Surat_Jalan_Detail WHERE NOTA LIKE @NOTA"
        Dim ds As New DataSet()

        ' Retrieve data from Surat_Jalan_Detail
        Using command As New MySqlCommand(querySuratJalanDetail, conn)
            command.Parameters.AddWithValue("@NOTA", NOTA)
            Using adapter As New MySqlDataAdapter(command)
                adapter.Fill(ds, "Surat_Jalan_Detail")
            End Using
        End Using
        ' Display data in DataGridView
        DgvData.DataSource = ds.Tables("Surat_Jalan_Detail")

        ' Queries to get data from Surat_Jalan and Surat_Jalan_Detail
        Dim querySuratJalan As String = "SELECT TGL_PENGIRIMAN, TOTAL_RUPIAH, ARMADA, JENIS_ARMADA, SUPIR, HELPER1, HELPER2, ID_USER FROM Surat_Jalan WHERE NOTA LIKE @NOTA"


        ' Retrieve data from Surat_Jalan
        Using command As New MySqlCommand(querySuratJalan, conn)
            command.Parameters.AddWithValue("@NOTA", NOTA)
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    tglPengiriman = If(IsDBNull(reader("TGL_PENGIRIMAN")), Date.MinValue, Convert.ToDateTime(reader("TGL_PENGIRIMAN")))
                    totalRupiah = If(IsDBNull(reader("TOTAL_RUPIAH")), 0, Convert.ToDecimal(reader("TOTAL_RUPIAH")))
                    TerbilangTotal = Terbilang(totalRupiah)
                    armada = If(IsDBNull(reader("ARMADA")), String.Empty, reader("ARMADA").ToString()) & " " & If(IsDBNull(reader("JENIS_ARMADA")), String.Empty, reader("JENIS_ARMADA").ToString())
                    supir = If(IsDBNull(reader("SUPIR")), String.Empty, reader("SUPIR").ToString())
                    helper1 = If(IsDBNull(reader("HELPER1")), String.Empty, reader("HELPER1").ToString())
                    helper2 = If(IsDBNull(reader("HELPER2")), String.Empty, reader("HELPER2").ToString())
                    idUser = If(IsDBNull(reader("ID_USER")), String.Empty, reader("ID_USER").ToString())
                End If
            End Using
        End Using
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

        ' Asumsikan lebar satu karakter rata-rata adalah 8 piksel
        Dim lebarKarakter As Double = 7.35
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
        Dim Mulaikata3 As Integer = BatasKiri + (LebarKertas * 45 / 100)

        Dim Mulaikata5 As Integer = BatasKiri + (LebarKertas * 70 / 100)
        Dim Mulaikata6 As Integer = BatasKiri + (LebarKertas * 77 / 100)
        'Dim Mulaikata7 As Integer = BatasKiri + (LebarKertas * 82 / 100)
        'Dim Mulaikata8 As Integer = BatasKiri + (LebarKertas * 95 / 100)


        'tinggi -= TxtMAjuString
        'Dim escapeCommandsebelum As String = Chr(27) & "J" & Chr(TxtMAjuString) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(fontJudulDot, ukuranFontJudul, FontStyle.Bold), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("SURAT JALAN PENGIRIMAN", New Drawing.Font(fontJudulDot, ukuranFontJudul, FontStyle.Bold), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 20 + jarakBarisDot
        e.Graphics.DrawString("Tgl    : " & Microsoft.VisualBasic.Format(tglPengiriman, "dd-MM-yy hh:mm:ss"), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Nomor  : " & TxtNota.Text, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)


        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString("Kasir  : " & idUser, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Armada : " & armada, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 14 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString("No", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Nota", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi)
        e.Graphics.DrawString("Pelanggan", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Alamat", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi, kanan)
        e.Graphics.DrawString("Lokasi", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)
        e.Graphics.DrawString("! TTD Penerima", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata6, tinggi)

        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Print each row from DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso Not IsDBNull(row.Cells("NOTA_BELANJA").Value) AndAlso Not String.IsNullOrEmpty(row.Cells("NOTA_BELANJA").Value.ToString()) Then
                tinggi += 10 + jarakBarisDot
                Dim rowIndex As Integer = row.Index + 1 ' Assuming row index starts from 1
                e.Graphics.DrawString(rowIndex.ToString() & ". ", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("NOTA_BELANJA").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata1, tinggi)
                e.Graphics.DrawString(row.Cells("NAMA_PELANGGAN").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata2, tinggi)
                e.Graphics.DrawString(row.Cells("ALAMAT_PELANGGAN").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)
                e.Graphics.DrawString(Microsoft.VisualBasic.Format(Convert.ToDecimal(row.Cells("NILAI_BELANJA").Value), "##,##0"), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("LOKASI").Value.ToString(), New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)
                e.Graphics.DrawString("! . . . . . . . .", New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata6, tinggi)
            End If
        Next

        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + jarakBarisDot

        e.Graphics.DrawString("   Total :", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)
        Dim formattedTotalRupiah As String = totalRupiah.ToString("#,##0")
        e.Graphics.DrawString(formattedTotalRupiah, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi, kanan)
        ' Menuliskan terbilang dengan nilai yang sudah dikonversi
        tinggi += 5 + jarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + jarakBarisDot

        e.Graphics.DrawString("Terbilang : " & TerbilangTotal & " Rupiah", New Drawing.Font("Arial Narrow", 8, FontStyle.Italic), Brushes.Black, BatasKiri, tinggi)


        ' Menuliskan Hormat Kami, Penerima, dan Kasir
        tinggi += 10 + jarakBarisDot
        e.Graphics.DrawString("Sopir", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Helper 1", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)
        e.Graphics.DrawString("Helper 2", New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)



        ' Menuliskan garis-garis
        tinggi += 30 + jarakBarisDot
        e.Graphics.DrawString(supir, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(helper1, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata3, tinggi)
        e.Graphics.DrawString(helper2, New Drawing.Font(fontIsiDot, ukuranFontIsi), Brushes.Black, Mulaikata5, tinggi)


        ' Menuliskan Hormat Kami, Penerima, dan Kasir
        tinggi += 10 + jarakBarisDot
        Dim printedInfo As String = "Dicetak : " & FormUtama.SLogin.Text & " " & FormUtama.Comp.Text & " " & Now.ToString("dd-MM-yy hh:mm:ss")
        e.Graphics.DrawString(printedInfo, New Drawing.Font(fontIsiDot, ukuranFontIsi - 1), Brushes.Black, BatasKiri, tinggi)
    End Sub





End Class