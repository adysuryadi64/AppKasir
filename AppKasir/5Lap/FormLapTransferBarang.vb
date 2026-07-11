Imports Microsoft.Reporting.WinForms

Public Class FormLapTransferBarang

    Private Sub FormLapTransferBarang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Select Case LblHeaderForm.Text
            Case "LAPORAN TRANSFER BARANG"
                PanelTF.Visible = True
                PanelTFDetail.Visible = False
            Case "LAPORAN TRANSFER BARANG DETAIL"
                PanelTF.Visible = False
                PanelTFDetail.Visible = True
        End Select
    End Sub


    Private Sub BtnTampilkan_Click(sender As Object, e As EventArgs) Handles BtnTampilkan.Click
        Dim tanggalAwal As Date = DTPAwal.Value.Date
        Dim tanggalAkhir As Date = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)

        Select Case LblHeaderForm.Text
            Case "LAPORAN TRANSFER BARANG"
                TransferBarang(tanggalAwal, tanggalAkhir)
            Case "LAPORAN TRANSFER BARANG DETAIL"
                TransferBarangDetail(tanggalAwal, tanggalAkhir)
        End Select
    End Sub


    Private Sub TransferBarang(ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim queryTransferBarang As String = "SELECT ID_TRANSFER, TGL_TRANSFER, LOKASI, TOTAL_QTY, TOTAL_BARANG, TOTAL_RUPIAH, ID_USER " &
                                         "FROM transfer_barang " &
                                         "WHERE TGL_TRANSFER >= @AwalBulan AND TGL_TRANSFER <= @AkhirBulan " &
                                         "ORDER BY ID_TRANSFER"

        Using cmdTransferBarang As New MySqlCommand(queryTransferBarang, conn)
            cmdTransferBarang.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdTransferBarang.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using readerTransferBarang As MySqlDataReader = cmdTransferBarang.ExecuteReader()
                Dim dataSetTransfer As New DataSetKL()
                dataSetTransfer.Load(readerTransferBarang, LoadOption.OverwriteChanges, "transfer_barang")
                Dim dtTF As DataTable = ConvertColumnToDateTime(dataSetTransfer.Tables("transfer_barang"), "TGL_TRANSFER")

                Dim reportParameters As New ReportParameterCollection From {
                New ReportParameter("Periode", "Periode: " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy")),
                New ReportParameter("Kasir", "Dicetak oleh: " & FormUtama.StatusNamaUser.Text),
                New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
            }

                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewerTF.LocalReport.DataSources.Clear()
                ReportViewerTF.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtTF))
                ReportViewerTF.LocalReport.SetParameters(reportParameters)

                ' Menampilkan laporan RDLC
                ReportViewerTF.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub TransferBarangDetail(ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim queryTransferBarangDetail As String = "SELECT ID_TRANSFER, TGL_TRANSFER, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, TOTAL, ID_USER " &
                                              "FROM transfer_barang_detail " &
                                              "WHERE TGL_TRANSFER >= @AwalBulan AND TGL_TRANSFER <= @AkhirBulan " &
                                              "ORDER BY ID_TRANSFER"

        Using cmdTransferBarangDetail As New MySqlCommand(queryTransferBarangDetail, conn)
            cmdTransferBarangDetail.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdTransferBarangDetail.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using readerTransferBarangDetail As MySqlDataReader = cmdTransferBarangDetail.ExecuteReader()
                Dim dataSetTransferDetail As New DataSetKL()
                dataSetTransferDetail.Load(readerTransferBarangDetail, LoadOption.OverwriteChanges, "transfer_barang_detail")
                Dim dtTFDetail As DataTable = ConvertColumnToDateTime(dataSetTransferDetail.Tables("transfer_barang_detail"), "TGL_TRANSFER")

                Dim reportParametersDetail As New ReportParameterCollection From {
                New ReportParameter("Periode", "Periode: " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy")),
                New ReportParameter("Kasir", "Dicetak oleh: " & FormUtama.StatusNamaUser.Text),
                New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
            }

                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewerTFDetail.LocalReport.DataSources.Clear()
                ReportViewerTFDetail.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtTFDetail))
                ReportViewerTFDetail.LocalReport.SetParameters(reportParametersDetail)

                ' Menampilkan laporan RDLC
                ReportViewerTFDetail.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub PanelReturJual_Paint(sender As Object, e As PaintEventArgs) Handles PanelReturJual.Paint

    End Sub
    Private Sub FormLapTransferBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
        Case Keys.F5 : BtnTampilkan.PerformClick()
    End Select
    End Sub

End Class
