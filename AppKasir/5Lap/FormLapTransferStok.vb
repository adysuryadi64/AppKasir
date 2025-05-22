Imports Microsoft.Reporting.WinForms

Public Class FormLapTransferStok

    Private Sub NotaStokOpname_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub


    Private Sub BtnPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPreview.Click
        Dim tanggalAwal As Date = DateTimePicker1.Value.Date.ToString("yyyy-MM-dd HH:mm:ss")
        Dim tanggalAkhir As Date = DateTimePicker1.Value.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss")
        ' Ambil data StokOpname
        Dim queryStokOpname As String = "SELECT ID_TRANSFER, URAIAN, TANGGAL, NAMA_BARANG_M, QTY_M, SATUAN_M, TOTAL_HARGA_M, NAMA_BARANG_K, QTY_K, SATUAN_K, TOTAL_HARGA_K, Selisih, ID_USER FROM Transfer_stok WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir"
        Using cmdStokOpname As New MySqlCommand(queryStokOpname, conn)
            cmdStokOpname.Parameters.AddWithValue("@tanggalAwal", tanggalAwal)
            cmdStokOpname.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir)
            Using rd As MySqlDataReader = cmdStokOpname.ExecuteReader()
                Using datasetStokOpname As New DataSetKL()
                    datasetStokOpname.Load(rd, LoadOption.OverwriteChanges, "Transfer_stok_barang")
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetStokOpname.Tables("Transfer_stok_barang")))
                End Using
            End Using
        End Using

        ' Set parameter laporan
        ' Menambahkan parameter ke laporan RDLC
        Dim parameters As New ReportParameterCollection From {
            New ReportParameter("Periode", "PERIODE TANGGAL : " & tanggalAwal.ToString("dd/MM/yyyy")),
            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.SLogin.Text),
            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
        }

        ReportViewer1.LocalReport.SetParameters(parameters)

        ReportViewer1.RefreshReport()
    End Sub

    Private Sub BtnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHide.Click
        Me.Close()
    End Sub
End Class