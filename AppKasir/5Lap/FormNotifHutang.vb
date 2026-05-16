Imports Microsoft.Reporting.WinForms

Public Class FormNotifHutang

    Public Sub IsiComboBoxSuplier()
        Dim tanggalJatuhTempo As DateTime
        If DateTime.TryParse(FormUtama.StatusTanggal.Text, tanggalJatuhTempo) Then
            ' Mengatur waktu menjadi akhir hari
            tanggalJatuhTempo = tanggalJatuhTempo.AddDays(1).AddTicks(-1)

            ' Format tanggal ke yyyy-MM-dd HH:mm:ss
            Dim formattedDate As String = tanggalJatuhTempo.ToString("yyyy-MM-dd HH:mm:ss")
            Dim query As String = "SELECT DISTINCT NAMA_SUPLIYER FROM pembelian WHERE JATUH_TEMPO <= @Tanggal AND STATUS_TRANSAKSI_BELI = 'Belum Lunas' ORDER BY NAMA_SUPLIYER"

            Dim suplierList As New List(Of String)

            Using cmdSuplier As New MySqlCommand(query, conn)
                cmdSuplier.Parameters.AddWithValue("@Tanggal", formattedDate)

                Using readerSuplier As MySqlDataReader = cmdSuplier.ExecuteReader()
                    While readerSuplier.Read()
                        suplierList.Add(readerSuplier("NAMA_SUPLIYER").ToString())
                    End While
                End Using
            End Using

            CmbSupplier.Items.Clear()
            CmbSupplier.Items.Add("Semua") ' Tambahkan pilihan "Semua"
            For Each suplier As String In suplierList
                CmbSupplier.Items.Add(suplier)
            Next
            CmbSupplier.SelectedIndex = 0
        End If
    End Sub


    Private Function GetHutangData(ByVal tanggalJatuhTempo As DateTime) As DataTable
        ' Format tanggal ke yyyy-MM-dd HH:mm:ss
        Dim formattedDate As String = tanggalJatuhTempo.ToString("yyyy-MM-dd HH:mm:ss")

        Dim query As String
        If CmbSupplier.Text = "Semua" Then
            query = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, TGL_BELI, GRAND_TOTAL_BELI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO FROM pembelian WHERE JATUH_TEMPO <= @Tanggal AND STATUS_TRANSAKSI_BELI = 'Belum Lunas' ORDER BY ID_PEMBELIAN"
        Else
            query = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, TGL_BELI, GRAND_TOTAL_BELI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO FROM pembelian WHERE JATUH_TEMPO <= @Tanggal AND STATUS_TRANSAKSI_BELI = 'Belum Lunas' AND NAMA_SUPLIYER = @NAMA_SUPLIYER ORDER BY ID_PEMBELIAN"
        End If

        Dim dataTable As New DataTable()
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Tanggal", formattedDate)
            If CmbSupplier.Text <> "Semua" Then
                cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", CmbSupplier.Text)
            End If

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                dataTable.Load(rd)
            End Using
        End Using

        Return dataTable
    End Function

    ' Method untuk mengatur parameter laporan
    Private Function GetReportParameters() As ReportParameterCollection
        Return New ReportParameterCollection From {
            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
        }
    End Function

    ' Method untuk menampilkan laporan hutang
    Public Sub TampilHutang()
        Dim tanggalJatuhTempo As DateTime
        If DateTime.TryParse(FormUtama.StatusTanggal.Text, tanggalJatuhTempo) Then
            tanggalJatuhTempo = tanggalJatuhTempo.AddDays(1).AddTicks(-1)

            ' Ambil data hutang
            Dim hutangData As DataTable = GetHutangData(tanggalJatuhTempo)

            ' Tentukan parameter laporan
            Dim parameters As ReportParameterCollection = GetReportParameters()

            ' Menetapkan dataset ke laporan RDLC
            Dim reportDataSource As New ReportDataSource("DataSet1", hutangData)
            ReportViewer1.LocalReport.DataSources.Clear()
            ReportViewer1.LocalReport.DataSources.Add(reportDataSource)
            ReportViewer1.LocalReport.SetParameters(parameters)

            ' Menampilkan laporan RDLC
            ReportViewer1.RefreshReport()
        End If
    End Sub



    Private Sub FormNotifHutang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ReportViewer1.RefreshReport()
    End Sub

    Private Sub CmbSupplier_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbSupplier.SelectedIndexChanged
        TampilHutang()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class
