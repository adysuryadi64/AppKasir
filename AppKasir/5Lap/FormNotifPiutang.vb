Imports Microsoft.Reporting.WinForms

Public Class FormNotifPiutang

    Private Sub FormNotifPiutang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

        ReportViewer1.RefreshReport()
    End Sub

    Public Sub TampilPiutang()

        ' Mendapatkan tanggal dari form utama
        Dim tanggalJatuhTempo As DateTime
        If DateTime.TryParse(FormUtama.StatusTanggal.Text, tanggalJatuhTempo) Then
            tanggalJatuhTempo = tanggalJatuhTempo.AddDays(1).AddTicks(-1)
            ' Format tanggal ke yyyy-MM-dd
            Dim formattedDate As String = tanggalJatuhTempo.ToString("yyyy-MM-dd HH:mm:ss")

            ' Perbarui query untuk menyesuaikan dengan AwalBulan dan AkhirBulan
            Dim query As String = "SELECT ID_PENJUALAN, TGL_TRANSAKSI, NAMA_PELANGGAN, GRAND_TOTAL_STL_PAJAK, (BAYAR - NILAI_RETUR + NOMINALBAYARPIUTANG) AS BAYAR, SISA_TAGIHAN, JATUH_TEMPO FROM penjualan WHERE JATUH_TEMPO <= @Tanggal AND STATUS_TRANSAKSI = 'Belum Lunas'"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Tanggal", formattedDate)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using dataset As New DataSetKL()
                        dataset.Load(rd, LoadOption.OverwriteChanges, "penjualan_Piutang")
                    Dim dtPiutangNotif As DataTable = ConvertColumnToDateTime(dataset.Tables("penjualan_Piutang"), "TGL_TRANSAKSI")
                    dtPiutangNotif = ConvertColumnToDateTime(dtPiutangNotif, "JATUH_TEMPO")

                    Dim parameters As New ReportParameterCollection From {
                New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
        New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
    }

                        ' Menetapkan dataset ke laporan RDLC
                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtPiutangNotif))
                        ReportViewer1.LocalReport.SetParameters(parameters)

                        ' Menampilkan laporan RDLC
                        ReportViewer1.RefreshReport()
                    End Using
                End Using
            End Using
        End If

    End Sub



End Class