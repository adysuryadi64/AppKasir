Imports Microsoft.Reporting.WinForms

Public Class NotaStokOpname

    Private Sub NotaStokOpname_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub


    Private Sub BtnPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPreview.Click
        Dim tanggalAwal As Date = DateTimePicker1.Value.Date
        Dim tanggalAkhir As Date = DateTimePicker2.Value.Date.AddDays(1).AddTicks(-1)
        ' Ambil data StokOpname
        Dim queryStokOpname As String = "SELECT ID_STOK_OPNAME, TANGGAL, LOKASI, NAMA_BARANG, KATEGORI, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, SATUAN, TOTAL_QTY, TOTAL_HARGA, KETERANGAN, ID_USER FROM stok_opname WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir"
        Using cmdStokOpname As New MySqlCommand(queryStokOpname, conn)
            cmdStokOpname.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdStokOpname.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmdStokOpname.ExecuteReader()
                Using datasetStokOpname As New DataSetKL()
                    datasetStokOpname.Load(rd, LoadOption.OverwriteChanges, "NotaStokOpname")
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetStokOpname.Tables("NotaStokOpname")))
                End Using
            End Using
        End Using

        ' Set parameter laporan
        Dim parameters As ReportParameter() = New ReportParameter(0) {}
        parameters(0) = New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN)

        ReportViewer1.LocalReport.SetParameters(parameters)

        ReportViewer1.RefreshReport()
    End Sub
End Class