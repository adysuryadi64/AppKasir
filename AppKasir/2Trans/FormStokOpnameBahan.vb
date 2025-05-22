Imports Microsoft.Reporting.WinForms

Public Class FormStokOpnameBahan

    Private Sub StokOpnameBahan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReportViewer1.LocalReport.DataSources.Clear()

        Dim queryStokOpname As String = ""
        ' Ambil data StokOpname
        Select Case FormUtama.SLokasi.Text
            Case "TOKO"
                queryStokOpname = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, STOK_TOKO AS STOK, SATUAN_STOK FROM tbl_barang ORDER BY NAMA_BARANG"
            Case "GUDANG"
                queryStokOpname = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, STOK_GUDANG AS STOK, SATUAN_STOK FROM tbl_barang ORDER BY NAMA_BARANG"
        End Select

        ' Mengisi DataSet dengan data dari database
        Using cmdStokOpname As New MySqlCommand(queryStokOpname, conn)
            Using rd As MySqlDataReader = cmdStokOpname.ExecuteReader()
                Using datasetStokOpname As New DataSet()
                    datasetStokOpname.Load(rd, LoadOption.OverwriteChanges, "BahanStokOpname")
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetStokOpname.Tables("BahanStokOpname")))
                End Using
            End Using
        End Using

        ' Mengambil total harga beli
        Dim queryTotalHargaBeli As String = ""
        Select Case FormUtama.SLokasi.Text
            Case "TOKO"
                queryTotalHargaBeli = "SELECT SUM(HARGA_BELI * STOK_TOKO) FROM tbl_barang"
            Case "GUDANG"
                queryTotalHargaBeli = "SELECT SUM(HARGA_BELI * STOK_GUDANG) FROM tbl_barang"
        End Select

        Using cmdTotalHargaBeli As New MySqlCommand(queryTotalHargaBeli, conn)
            Dim totalHargaBeli As Decimal = Convert.ToDecimal(cmdTotalHargaBeli.ExecuteScalar())

            ' Set parameter laporan
            Dim parameters As ReportParameter() = New ReportParameter(2) {}
            parameters(0) = New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN)
            parameters(1) = New ReportParameter("TOTALHARGABELI", "Total Nilai HPP : Rp. " & totalHargaBeli.ToString("N0", Globalization.CultureInfo.CreateSpecificCulture("id-ID")))
            parameters(2) = New ReportParameter("LOKASI", "BAHAN STOK OPNAME " & FormUtama.SLokasi.Text)
            ReportViewer1.LocalReport.SetParameters(parameters)

            ReportViewer1.RefreshReport()
        End Using
    End Sub

End Class
