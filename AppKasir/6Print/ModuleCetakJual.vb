Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.Reporting.WinForms



' Modul untuk mencetak laporan penjualan
Module ModuleCetakJual
    ' Fungsi untuk mengambil data pelanggan berdasarkan faktur
    Public Function Ambildatapelanggan(ByVal Faktur As String) As Pelanggan
        Dim pelanggan As New Pelanggan()
        ' Query untuk mengambil data pelanggan dari database
        Using cmd As New MySqlCommand("SELECT NAMA_PELANGGAN, JENIS_PELANGGAN, TGL_TRANSAKSI, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, TYPE_AKUN, KODE_AKUN, JENIS_PEMBAYARAN, METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE ID_PENJUALAN = @faktur", conn)
            cmd.Parameters.AddWithValue("@faktur", Faktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Mengisi objek Pelanggan dengan data dari database
                    pelanggan.NamaPelanggan = rd("NAMA_PELANGGAN").ToString()
                    pelanggan.JenisPelanggan = rd("JENIS_PELANGGAN").ToString()

                    ' Pastikan bahwa nilai dari database diubah menjadi DateTime terlebih dahulu
                    Dim tglTransaksi As DateTime = Convert.ToDateTime(rd("TGL_TRANSAKSI"))
                    ' Format DateTime ke string sesuai format yang diinginkan
                    pelanggan.TglTransaksi = tglTransaksi.ToString("dd-MM-yy HH:mm:ss")

                    ' Format angka menggunakan pemisah ribuan Indonesia
                    Dim culture As CultureInfo = New CultureInfo("id-ID")
                    pelanggan.DiskonTotalRp = Convert.ToDecimal(rd("DISKON_TOTAL_RP")).ToString("N0", culture)
                    pelanggan.GrandTotalStlPajak = Convert.ToDecimal(rd("GRAND_TOTAL_STL_PAJAK")).ToString("N0", culture)
                    pelanggan.PajakRp = Convert.ToDecimal(rd("PAJAK_RP")).ToString("N0", culture)
                    pelanggan.Bayar = Convert.ToDecimal(rd("BAYAR")).ToString("N0", culture)

                    ' Menangani nilai Sisa Tagihan
                    Dim sisaTagihanValue As Object = rd("SISA_TAGIHAN")
                    Dim sisaTagihanDecimal As Decimal = 0D ' Menggunakan 0.0 sebagai nilai default

                    If sisaTagihanValue Is Nothing OrElse IsDBNull(sisaTagihanValue) OrElse sisaTagihanValue = 0 Then
                        pelanggan.Kembali = Convert.ToDecimal(rd("KEMBALI")).ToString("N0", culture)
                        pelanggan.StatusTransaksi = "Kembali :"
                        pelanggan.TanggalJT = ""
                    Else
                        If Decimal.TryParse(sisaTagihanValue.ToString(), sisaTagihanDecimal) Then
                            pelanggan.Kembali = sisaTagihanDecimal.ToString("N0", culture)
                            pelanggan.StatusTransaksi = "Hutang :"

                            ' Pastikan bahwa nilai dari database diubah menjadi DateTime terlebih dahulu
                            Dim tgljt As DateTime = Convert.ToDateTime(rd("JATUH_TEMPO"))
                            ' Format DateTime ke string sesuai format yang diinginkan
                            pelanggan.JatuhTempo = tgljt.ToString("dd-MM-yyyy")
                            pelanggan.TanggalJT = "Tanggal JT :"
                        End If
                    End If

                    pelanggan.TypeAkun = rd("TYPE_AKUN").ToString()
                    If pelanggan.TypeAkun = "BANK" Then
                        pelanggan.KodeAkun = rd("KODE_AKUN").ToString()
                        pelanggan.JenisPembayaran = "To " & rd("JENIS_PEMBAYARAN").ToString()
                        pelanggan.Metode = rd("METODE").ToString()
                        pelanggan.Bank = "From " & rd("BANK").ToString()
                        pelanggan.NoRekening = "Rek " & rd("NO_REKENING").ToString()
                        pelanggan.NamaRekening = rd("NAMA_REKENING").ToString()
                        pelanggan.NoRefferensi = "Ref " & rd("NO_REFFERENSI").ToString()
                    Else
                        pelanggan.KodeAkun = ""
                        pelanggan.JenisPembayaran = ""
                        pelanggan.Metode = ""
                        pelanggan.Bank = ""
                        pelanggan.NoRekening = ""
                        pelanggan.NamaRekening = ""
                        pelanggan.NoRefferensi = ""
                    End If
                    pelanggan.IdUser = rd("ID_USER").ToString()
                    pelanggan.IdKomputer = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using
        Return pelanggan
    End Function


    ' Variabel untuk menyimpan stream halaman yang akan dicetak
    Private streams As IList(Of Stream)
    Private currentPageIndex As Integer

    ' Prosedur untuk mencetak laporan berdasarkan faktur
    Public Sub PrintReport(ByVal Faktur As String)

        Dim ds As New DataSet()

        ' Query untuk mengambil data detail penjualan dari database
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = @faktur", conn)
            cmd.Parameters.AddWithValue("@faktur", Faktur)
            ' Mengisi DataSet dengan data dari database
            Using adapter As New MySqlDataAdapter(cmd)
                adapter.Fill(ds, "nota_penjualan")
            End Using
        End Using

        ' Ambil data pelanggan
        Dim pelanggan As Pelanggan = Ambildatapelanggan(Faktur)

        ' Mengatur objek LocalReport
        Dim report As New LocalReport()
        report.ReportPath = "ReportCetakJual.rdlc"

        Dim reportParameters As New List(Of ReportParameter) From {
    New ReportParameter("NamaToko", NAMA_PERUSAHAAN),
    New ReportParameter("AlamatToko", ALAMAT_PERUSAHAAN),
    New ReportParameter("KotaToko", KOTA_PERUSAHAAN),
    New ReportParameter("KontakToko", KONTAK_PERUSAHAAN),
    New ReportParameter("Footer1", FOOTER1),
    New ReportParameter("Footer2", FOOTER2),
    New ReportParameter("NamaPelanggan", pelanggan.NamaPelanggan),
    New ReportParameter("JenisPelanggan", pelanggan.JenisPelanggan),
    New ReportParameter("TglTransaksi", pelanggan.TglTransaksi),
    New ReportParameter("TanggalJT", pelanggan.TanggalJT),
    New ReportParameter("DiskonTotalRp", pelanggan.DiskonTotalRp),
    New ReportParameter("GrandTotalStlPajak", pelanggan.GrandTotalStlPajak),
    New ReportParameter("Terbilang", Terbilang(pelanggan.GrandTotalStlPajak)),
    New ReportParameter("PajakRp", pelanggan.PajakRp),
    New ReportParameter("Bayar", pelanggan.Bayar),
    New ReportParameter("Kembali", pelanggan.Kembali),
    New ReportParameter("StatusTransaksi", pelanggan.StatusTransaksi),
    New ReportParameter("JatuhTempo", pelanggan.JatuhTempo),
    New ReportParameter("TypeAkun", pelanggan.TypeAkun),
    New ReportParameter("KodeAkun", pelanggan.KodeAkun),
    New ReportParameter("JenisPembayaran", pelanggan.JenisPembayaran),
    New ReportParameter("Metode", pelanggan.Metode),
    New ReportParameter("Bank", pelanggan.Bank),
    New ReportParameter("NoRekening", pelanggan.NoRekening),
    New ReportParameter("NamaRekening", pelanggan.NamaRekening),
    New ReportParameter("NoRefferensi", pelanggan.NoRefferensi),
    New ReportParameter("IdUser", pelanggan.IdUser),
    New ReportParameter("IdKomputer", pelanggan.IdKomputer),
    New ReportParameter("Faktur", Faktur)
}


        ' Menentukan sumber data laporan
        Dim dataSource As New ReportDataSource("DataSet1", ds.Tables("nota_penjualan"))
        report.DataSources.Add(dataSource)

        ' Mengatur informasi perangkat untuk rendering laporan
        Dim deviceInfo As String = "<DeviceInfo>" &
            "<OutputFormat>EMF</OutputFormat>" &
            "<PageWidth>21cm</PageWidth>" &
            "<PageHeight>14cm</PageHeight>" &
            "<MarginTop>0.5cm</MarginTop>" &
            "<MarginLeft>0.5cm</MarginLeft>" &
            "<MarginRight>0.5cm</MarginRight>" &
            "<MarginBottom>0.5cm</MarginBottom>" &
            "</DeviceInfo>"

        Dim warnings() As Warning = Nothing
        streams = New List(Of Stream)()
        currentPageIndex = 0

        ' Fungsi callback untuk membuat stream
        Dim createStream As CreateStreamCallback =
            Function(name As String, extension As String, encoding As Encoding, mimeType As String, willSeek As Boolean) As Stream
                Dim stream As Stream = New MemoryStream()
                streams.Add(stream)
                Return stream
            End Function

        ' Render laporan menjadi byte array
        report.Render("Image", deviceInfo, createStream, warnings)

        ' Mengatur posisi stream untuk setiap halaman
        For Each stream As Stream In streams
            stream.Position = 0
        Next

        ' Membuat objek PrintDocument
        Dim printDoc As New PrintDocument()
        printDoc.PrinterSettings.PrinterName = GetDefaultPrinter()

        If Not printDoc.PrinterSettings.IsValid Then
            Throw New Exception("Error: cannot find the default printer.")
        End If

        ' Menambahkan event handler untuk mencetak halaman
        AddHandler printDoc.PrintPage, AddressOf PrintPage

        ' Mencetak dokumen
        printDoc.Print()

        ' Membersihkan stream setelah mencetak
        For Each stream As Stream In streams
            stream.Close()
        Next
        streams.Clear()
    End Sub

    ' Fungsi untuk mendapatkan printer default
    Private Function GetDefaultPrinter() As String
        Dim settings As New PrinterSettings()
        Return settings.PrinterName
    End Function

    ' Prosedur untuk mencetak halaman
    Private Sub PrintPage(ByVal sender As Object, ByVal ev As PrintPageEventArgs)
        ' Menggambar halaman saat ini
        Dim pageImage As New Metafile(streams(currentPageIndex))

        ' Menyesuaikan area persegi panjang dengan margin printer
        Dim adjustedRect As New Rectangle(ev.PageBounds.Left - CInt(ev.PageSettings.HardMarginX),
                                          ev.PageBounds.Top - CInt(ev.PageSettings.HardMarginY),
                                          ev.PageBounds.Width,
                                          ev.PageBounds.Height)

        ' Menggambar latar belakang putih untuk laporan
        ev.Graphics.FillRectangle(Brushes.White, adjustedRect)

        ' Menggambar konten laporan
        ev.Graphics.DrawImage(pageImage, adjustedRect)

        ' Mempersiapkan untuk halaman berikutnya. Pastikan belum mencapai akhir.
        currentPageIndex += 1
        ev.HasMorePages = (currentPageIndex < streams.Count)
    End Sub

    ' Kelas untuk menyimpan informasi pelanggan
    Public Class Pelanggan
        Public Property NamaPelanggan As String
        Public Property JenisPelanggan As String
        Public Property TglTransaksi As String
        Public Property TanggalJT As String
        Public Property DiskonTotalRp As String
        Public Property GrandTotalStlPajak As String
        Public Property PajakRp As String
        Public Property Bayar As String
        Public Property Kembali As String
        Public Property SisaTagihan As String
        Public Property JatuhTempo As String
        Public Property StatusTransaksi As String
        Public Property TypeAkun As String
        Public Property KodeAkun As String
        Public Property JenisPembayaran As String
        Public Property Metode As String
        Public Property Bank As String
        Public Property NoRekening As String
        Public Property NamaRekening As String
        Public Property NoRefferensi As String
        Public Property IdUser As String
        Public Property IdKomputer As String
    End Class
End Module
