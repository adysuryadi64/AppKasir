Imports System.Drawing.Printing
Imports System.IO


Public Class PrintTransferBarang
    ' Konfigurasi printer dibaca dari ModuleKonfigurasi (format baru)
    Private cfgDot As KonfigurasiDotMatrix

    ' Alias untuk kompatibilitas kode cetak yang sudah ada
    Private ReadOnly Property JenisPrinter As String
        Get
            Return GetJenisPrinterTransaksi("TransferBarang")
        End Get
    End Property
    Private ReadOnly Property NamaPrinterDot As String
        Get
            Return If(cfgDot IsNot Nothing, cfgDot.NamaPrinter, "")
        End Get
    End Property
    Private ReadOnly Property LebarKertasDot As Integer
        Get
            Return If(cfgDot IsNot Nothing, cfgDot.LebarKertas, 27)
        End Get
    End Property
    Private ReadOnly Property TinggiDot As Integer
        Get
            Return 7
        End Get
    End Property
    Private ReadOnly Property BatasKiriDot As Integer
        Get
            Return If(cfgDot IsNot Nothing, cfgDot.BatasKiri, 0)
        End Get
    End Property
    Private ReadOnly Property JarakBarisDot As Integer
        Get
            Return If(cfgDot IsNot Nothing, cfgDot.JarakBaris, 2)
        End Get
    End Property
    Private ReadOnly Property FontJudulDot As String
        Get
            Return "Consolas"
        End Get
    End Property
    Private ReadOnly Property FontIsiDot As String
        Get
            Return "Consolas"
        End Get
    End Property
    Private ReadOnly Property UkuranJudulDot As Integer
        Get
            Return 12
        End Get
    End Property
    Private ReadOnly Property UkuranIsiDot As Integer
        Get
            Return 9
        End Get
    End Property

    Public WithEvents PDDot As New PrintDocument
    Private ReadOnly PPDDot As New PrintPreviewDialog


    Private Panjangkertas As Integer
    Private LebarKertas As Integer


    Dim tglTransfer As Date
    Dim lokasi As String = String.Empty
    Dim totalRupiah As Decimal
    Dim TerbilangRupiah As String
    Dim idUser As String = String.Empty
    Dim keteranganLokasi As String = String.Empty


    Public Sub Ambildataprinter()
        cfgDot = New KonfigurasiDotMatrix("TransferBarang")
    End Sub


    Public Sub ProsesCetak()
        Ambildataprinter()

        AmbilaDataTransferBarang(TxtNota.Text)
        'If JenisPrinter = "Printer Dot Matrix" Then
        PrinterDotMatrik()
        'Else
        'NotaTransferBarang.AmbilDataTransferBarang(TxtNota.Text)
        'End If

        Close()
    End Sub

    Public Sub AmbilaDataTransferBarang(ByVal ID_TRANSFER As String)

        ' Query untuk detail transfer barang
        Dim queryTransferBarangDetail As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA, QTY, SATUAN, TOTAL FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER"
        Dim ds As New DataSet()

        ' Ambil data dari Transfer_Barang_Detail
        Using command As New MySqlCommand(queryTransferBarangDetail, conn)
            command.Parameters.AddWithValue("@ID_TRANSFER", ID_TRANSFER)
            Using adapter As New MySqlDataAdapter(command)
                adapter.Fill(ds, "TransferBarang")
            End Using
        End Using

        ' Tampilkan data di DataGridView
        DgvData.DataSource = ds.Tables("TransferBarang")

        ' Query untuk header transfer barang
        Dim queryTransferBarang As String = "SELECT TGL_TRANSFER, LOKASI, TOTAL_RUPIAH, ID_USER FROM Transfer_Barang WHERE ID_TRANSFER = @ID_TRANSFER"

        ' Ambil data dari Transfer_Barang
        Using command As New MySqlCommand(queryTransferBarang, conn)
            command.Parameters.AddWithValue("@ID_TRANSFER", ID_TRANSFER)
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    tglTransfer = If(IsDBNull(reader("TGL_TRANSFER")), Date.MinValue, Convert.ToDateTime(reader("TGL_TRANSFER")))
                    lokasi = If(IsDBNull(reader("LOKASI")), String.Empty, reader("LOKASI").ToString())

                    ' Tentukan keterangan lokasi berdasarkan nilai lokasi
                    keteranganLokasi = If(lokasi = "TOKO", "TRANSFER BARANG DARI TOKO KE GUDANG", "TRANSFER BARANG DARI GUDANG KE TOKO")

                    totalRupiah = If(IsDBNull(reader("TOTAL_RUPIAH")), 0D, Convert.ToDecimal(reader("TOTAL_RUPIAH")))
                    TerbilangRupiah = Terbilang(totalRupiah)
                    idUser = If(IsDBNull(reader("ID_USER")), String.Empty, reader("ID_USER").ToString())
                End If
            End Using
        End Using
    End Sub


    Public Sub PrinterDotMatrik()
        Dim printer_nota As String

        If NamaPrinterDot <> "Printerdefault" Then
            printer_nota = NamaPrinterDot
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

        LebarKertas = CInt((LebarKertasDot * 0.3937) * 72) ' Lebar dalam dot
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
        Dim garis As String = BuatGaris(jumlahKarakter)





        Dim TopRight As New StringFormat With {
    .LineAlignment = StringAlignment.Near,
    .Alignment = StringAlignment.Far
    }

        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + BatasKiriDot

        Dim Mulaikata1 As Integer = BatasKiri + (LebarKertas * 5 / 100)
        Dim Mulaikata2 As Integer = BatasKiri + (LebarKertas * 17 / 100)
        Dim Mulaikata3 As Integer = BatasKiri + (LebarKertas * 45 / 100)

        Dim Mulaikata5 As Integer = BatasKiri + (LebarKertas * 55 / 100)
        Dim Mulaikata6 As Integer = BatasKiri + (LebarKertas * 65 / 100)
        Dim Mulaikata7 As Integer = BatasKiri + (LebarKertas * 90 / 100)
        'Dim Mulaikata8 As Integer = BatasKiri + (LebarKertas * 95 / 100)


        'tinggi -= 0
        'Dim escapeCommandsebelum As String = Chr(27) & "J" & Chr(0) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

        e.Graphics.DrawString(NAMA_PERUSAHAAN, New Drawing.Font(FontJudulDot, UkuranJudulDot, FontStyle.Bold), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(keteranganLokasi, New Drawing.Font(FontJudulDot, UkuranJudulDot, FontStyle.Bold), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 20 + JarakBarisDot
        e.Graphics.DrawString("Tgl    : " & tglTransfer.ToString("yyyy-MM-dd HH:mm:ss"), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Nomor  : " & TxtNota.Text, New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata3, tinggi)


        tinggi += 10 + JarakBarisDot
        e.Graphics.DrawString("Kasir  : " & idUser, New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Lokasi : " & lokasi, New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 14 + JarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + JarakBarisDot
        e.Graphics.DrawString("No", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Kode", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata1, tinggi)
        e.Graphics.DrawString("Nama Barang", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata2, tinggi)
        e.Graphics.DrawString("Harga", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata5, tinggi, kanan)
        e.Graphics.DrawString("Qty", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata6, tinggi, kanan)
        e.Graphics.DrawString("Satuan", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata6, tinggi)
        e.Graphics.DrawString("Jumlah", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata7, tinggi, kanan)

        tinggi += 5 + JarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        ' Print each row from DataGridView  , , , , , 
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso Not IsDBNull(row.Cells("ID_BARANG").Value) AndAlso Not String.IsNullOrEmpty(row.Cells("ID_BARANG").Value.ToString()) Then
                tinggi += 10 + JarakBarisDot
                Dim rowIndex As Integer = row.Index + 1 ' Assuming row index starts from 1
                e.Graphics.DrawString(rowIndex.ToString() & ". ", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata1, tinggi, kanan)
                e.Graphics.DrawString(row.Cells("ID_BARANG").Value.ToString(), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata1, tinggi)
                e.Graphics.DrawString(row.Cells("NAMA_BARANG").Value.ToString(), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata2, tinggi)

                ' Format angka dengan ribuan dan tanpa desimal
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("HARGA").Value).ToString("N0"), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata5, tinggi, kanan)
                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("QTY").Value).ToString("N0"), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata6, tinggi, kanan)

                e.Graphics.DrawString(row.Cells("SATUAN").Value.ToString(), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata6, tinggi)

                e.Graphics.DrawString(Convert.ToDecimal(row.Cells("TOTAL").Value).ToString("N0"), New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata7, tinggi, kanan)

            End If
        Next

        tinggi += 5 + JarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 5 + JarakBarisDot

        e.Graphics.DrawString("   Total :", New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata5, tinggi)
        Dim formattedTotalRupiah As String = totalRupiah.ToString("#,##0")
        e.Graphics.DrawString(formattedTotalRupiah, New Drawing.Font(FontIsiDot, UkuranIsiDot), Brushes.Black, Mulaikata7, tinggi, kanan)
        ' Menuliskan terbilang dengan nilai yang sudah dikonversi
        tinggi += 5 + JarakBarisDot
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + JarakBarisDot

        e.Graphics.DrawString("Terbilang : " & TerbilangRupiah & " Rupiah", New Drawing.Font("Arial Narrow", 8, FontStyle.Italic), Brushes.Black, BatasKiri, tinggi)

        ' Menuliskan Hormat Kami, Penerima, dan Kasir
        tinggi += 10 + JarakBarisDot
        Dim printedInfo As String = "Dicetak : " & FormUtama.StatusNamaUser.Text & " " & FormUtama.StatusNamaPC.Text & " " & Now.ToString("dd-MM-yy hh:mm:ss")
        e.Graphics.DrawString(printedInfo, New Drawing.Font(FontIsiDot, UkuranIsiDot - 1), Brushes.Black, BatasKiri, tinggi)


    End Sub


End Class