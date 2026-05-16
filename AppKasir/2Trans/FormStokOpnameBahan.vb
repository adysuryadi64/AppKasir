Imports Microsoft.Reporting.WinForms

Public Class FormStokOpnameBahan

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit StokOpnameBahan ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("OPN-BHN:" & noOpname, "HAPUS"/"EDIT",
    '       "Stok Opname Bahan", snapshotJson, "[KRITIS] Hapus/Edit stok opname bahan", trans)
    ' Snapshot minimal: id_opname, tanggal, lokasi, id_barang, stok_system, stok_nyata, selisih
    ' ─────────────────────────────────────────────────────────────────────────

    Private Sub StokOpnameBahan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            ReportViewer1.LocalReport.DataSources.Clear()

            Dim queryStokOpname As String = ""
            Select Case FormUtama.StatusLokasi.Text
                Case "TOKO"
                    queryStokOpname = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, STOK_TOKO AS STOK, SATUAN_STOK FROM tbl_barang ORDER BY NAMA_BARANG"
                Case "GUDANG"
                    queryStokOpname = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, STOK_GUDANG AS STOK, SATUAN_STOK FROM tbl_barang ORDER BY NAMA_BARANG"
            End Select

            Using cmdStokOpname As New MySqlCommand(queryStokOpname, conn)
                Using rd As MySqlDataReader = cmdStokOpname.ExecuteReader()
                    Using datasetStokOpname As New DataSet()
                        datasetStokOpname.Load(rd, LoadOption.OverwriteChanges, "BahanStokOpname")
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetStokOpname.Tables("BahanStokOpname")))
                    End Using
                End Using
            End Using

            Dim queryTotalHargaBeli As String = ""
            Select Case FormUtama.StatusLokasi.Text
                Case "TOKO"
                    queryTotalHargaBeli = "SELECT SUM(HARGA_BELI * STOK_TOKO) FROM tbl_barang"
                Case "GUDANG"
                    queryTotalHargaBeli = "SELECT SUM(HARGA_BELI * STOK_GUDANG) FROM tbl_barang"
            End Select

            Using cmdTotalHargaBeli As New MySqlCommand(queryTotalHargaBeli, conn)
                Dim totalHargaBeli As Decimal = Convert.ToDecimal(cmdTotalHargaBeli.ExecuteScalar())
                Dim parameters As ReportParameter() = New ReportParameter(2) {}
                parameters(0) = New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN)
                parameters(1) = New ReportParameter("TOTALHARGABELI", "Total Nilai HPP : Rp. " & totalHargaBeli.ToString("N0", Globalization.CultureInfo.CreateSpecificCulture("id-ID")))
                parameters(2) = New ReportParameter("LOKASI", "BAHAN STOK OPNAME " & FormUtama.StatusLokasi.Text)
                ReportViewer1.LocalReport.SetParameters(parameters)
                ReportViewer1.RefreshReport()
            End Using
        Catch ex As MySqlException
            If Not TawarMigrasi(ex) Then
                MessageBox.Show("Gagal memuat laporan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

End Class
