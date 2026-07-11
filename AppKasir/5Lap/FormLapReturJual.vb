Imports Microsoft.Reporting.WinForms

Public Class FormLapReturJual

    Private Sub FormLapReturJual_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Cursor = Cursors.WaitCursor
        CbTanggal.Checked = True
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        Select Case LblHeaderForm.Text
            Case "LAPORAN RETUR PENJUALAN"
                PanelReturJual.Visible = True
                PanelReturJualDetail.Visible = False
                PanelReturBarang.Visible = False
            Case "LAPORAN RETUR PENJUALAN DETAIL"
                PanelReturJual.Visible = False
                PanelReturJualDetail.Visible = True
                PanelReturBarang.Visible = False
            Case "LAPORAN BARANG RETUR PENJUALAN"
                PanelReturJual.Visible = False
                PanelReturJualDetail.Visible = False
                PanelReturBarang.Visible = True
        End Select
        Cursor = Cursors.Default
    End Sub

    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            Dim angkaBulan As String = (CmbBln.SelectedIndex + 1).ToString("D2")
            Dim teksBulanTahunTerpilih As String = angkaBulan & "/" & CmbThn.Text
            TampilkanLaporan(LblHeaderForm.Text)
        End If
    End Sub

    Private Sub TampilkanLaporan(ByVal judul As String)
        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If

        Select Case judul
            Case "LAPORAN RETUR PENJUALAN"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilRekening(tanggalAwal, tanggalAkhir)
            Case "LAPORAN RETUR PENJUALAN DETAIL"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilPelanggan(tanggalAwal, tanggalAkhir)
            Case "LAPORAN BARANG RETUR PENJUALAN"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilPelanggan(tanggalAwal, tanggalAkhir)
        End Select
    End Sub



    Public Sub TampilKasir(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)

        CmbKasir.Items.Clear()
        CmbKasir.Items.Add("Semua")

        Dim query As String = "SELECT DISTINCT ID_USER FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @AwalBulan AND TGL_RETUR_JUAL <= @AkhirBulan ORDER BY ID_USER"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbKasir.Items.Add(rd("ID_USER").ToString())
                End While
            End Using
        End Using

        CmbKasir.SelectedIndex = 0
    End Sub



    Private Sub TampilRekening(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Label5.Text = "Rekening"

        CmbRekening.Items.Clear()
        CmbRekening.Items.Add("SEMUA")
        Dim query As String = "SELECT DISTINCT NAMA_REKENING FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @AwalBulan AND TGL_RETUR_JUAL <= @AkhirBulan ORDER BY NAMA_REKENING"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        CmbRekening.Items.Add(reader("NAMA_REKENING").ToString())
                    End While
                End If
            End Using
        End Using
        CmbRekening.SelectedIndex = 0
    End Sub

    Private Sub TampilPelanggan(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Label5.Text = "Pelanggan"

        CmbRekening.Items.Clear()
        CmbRekening.Items.Add("SEMUA")

        Dim query As String = "SELECT DISTINCT NAMA_PELANGGAN FROM retur_penjualan_detail WHERE TGL_RETUR_JUAL >= @AwalBulan AND TGL_RETUR_JUAL <= @AkhirBulan ORDER BY NAMA_PELANGGAN"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        CmbRekening.Items.Add(reader("NAMA_PELANGGAN").ToString())
                    End While
                End If
            End Using
        End Using
        CmbRekening.SelectedIndex = 0
    End Sub

    Private Sub DTPTanggal_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DTPAwal.ValueChanged
        TampilkanLaporan(LblHeaderForm.Text)
    End Sub

    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim namaAkunD As String = CmbRekening.Text

        Dim sql As String = "SELECT Type_Akun, Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtRekening.Text = reader("Kode_akun").ToString()
                Else
                    TxtRekening.Clear()
                End If
            End Using
        End Using
    End Sub

    Private Sub CbTanggal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
            TampilkanLaporan(LblHeaderForm.Text)
        End If
    End Sub

    Private Sub CbBulan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
            If Not String.IsNullOrEmpty(CmbBln.Text) Then
                TampilkanLaporan(LblHeaderForm.Text)
            End If
        End If
    End Sub


    Private Sub CmbBln_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBln.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub CmbThn_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbThn.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub BtnTampilkan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTampilkan.Click
        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)


        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked = True Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked = True Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If

        Select Case LblHeaderForm.Text
            Case "LAPORAN RETUR PENJUALAN"
                Dim rekeningatauPelanggan As String = If(CmbRekening.Text = "SEMUA" Or CmbRekening.SelectedIndex = 0, "", TxtRekening.Text)
                ReturPenjualan(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN RETUR PENJUALAN DETAIL"
                Dim rekeningatauPelanggan As String = If(CmbRekening.Text = "SEMUA" Or CmbRekening.SelectedIndex = 0, "", CmbRekening.Text)
                ReturPenjualanDetail(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN BARANG RETUR PENJUALAN"
                Dim rekeningatauPelanggan As String = If(CmbRekening.Text = "SEMUA" Or CmbRekening.SelectedIndex = 0, "", CmbRekening.Text)
                ReturPenjualanBarang(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
        End Select
    End Sub


    Private Sub ReturPenjualan(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim queryReturJual As String = "SELECT " &
   "ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, NAMA_PELANGGAN, ID_PENJUALAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER " &
   "FROM retur_penjualan " &
   "WHERE TGL_RETUR_JUAL >= @AwalBulan AND TGL_RETUR_JUAL <= @AkhirBulan AND KODE_REKENING LIKE @KODE_REKENING AND ID_USER LIKE @IdUser " &
   "ORDER BY ID_RETUR_PENJUALAN"

        Using cmdDataRetur As New MySqlCommand(queryReturJual, conn) ' Perbaikan: cmdDataReturBarang menjadi cmdDataRetur
            cmdDataRetur.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@KODE_REKENING", String.Format("%{0}%", rekeningatauPelanggan))
            cmdDataRetur.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))

            Using rdDataRetur As MySqlDataReader = cmdDataRetur.ExecuteReader() ' Perbaikan: rdDataReturBarang menjadi rdDataRetur
                Dim datasetRetur As New DataSetKL() ' Perbaikan: datasetReturBarang menjadi datasetRetur
                datasetRetur.Load(rdDataRetur, LoadOption.OverwriteChanges, "retur_penjualan") ' Perbaikan: datasetReturBarang menjadi datasetRetur
                Dim dtReturJual As DataTable = ConvertColumnToDateTime(datasetRetur.Tables("retur_penjualan"), "TGL_RETUR_JUAL")

                ' Menambahkan parameter ke laporan RDLC
                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Rekening : " & CmbRekening.Text

                Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy") & keterangan),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
}
                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewerReturJual.LocalReport.DataSources.Clear()
                ReportViewerReturJual.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtReturJual)) ' Perbaikan: ReportViewer6 menjadi ReportViewer7
                ReportViewerReturJual.LocalReport.SetParameters(parametersRetur) ' Perbaikan: ReportViewer6 menjadi ReportViewer7

                ' Menampilkan laporan RDLC
                ReportViewerReturJual.RefreshReport() ' Perbaikan: ReportViewer5 menjadi ReportViewer7
            End Using
        End Using
    End Sub

    Private Sub ReturPenjualanDetail(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim query As String = "SELECT ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, NAMA_PELANGGAN, NAMA_BARANG, QTY, SATUAN, " &
                       "HARGA_BELI_SATUAN, TOTAL_DISKON, TOTAL_HARGA, LABA, ID_USER " &
                       "FROM retur_penjualan_detail " &
                       "WHERE TGL_RETUR_JUAL >= @AwalBulan AND TGL_RETUR_JUAL <= @AkhirBulan AND ID_USER LIKE @IdUser AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN " &
                       "ORDER BY ID_RETUR_PENJUALAN"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))
            command.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", rekeningatauPelanggan))


            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dataset As New DataSetKL()
                dataset.Load(reader, LoadOption.OverwriteChanges, "retur_penjualan_detail")
                Dim dtReturJualDetail As DataTable = ConvertColumnToDateTime(dataset.Tables("retur_penjualan_detail"), "TGL_RETUR_JUAL")

                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Pelanggan : " & CmbRekening.Text

                Dim parameters As New ReportParameterCollection From {
                    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
                    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                }

                ReportViewerReturJualDetail.LocalReport.DataSources.Clear()
                ReportViewerReturJualDetail.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtReturJualDetail))
                ReportViewerReturJualDetail.LocalReport.SetParameters(parameters)
                ReportViewerReturJualDetail.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub ReturPenjualanBarang(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, MAX(SATUAN) AS SATUAN, SUM(QTY) as QTY, " &
                              "SUM(HARGA_BELI_SATUAN) AS HARGA_BELI_SATUAN, SUM(TOTAL_DISKON) AS TOTAL_DISKON, " &
                              "SUM(TOTAL_HARGA) AS TOTAL_HARGA, SUM(LABA) AS LABA " &
                              "FROM retur_penjualan_detail " &
                              "WHERE TGL_RETUR_JUAL >= @AwalBulan AND TGL_RETUR_JUAL <= @AkhirBulan AND ID_USER LIKE @IdUser AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN " &
                              "GROUP BY ID_BARANG, NAMA_BARANG " &
                              "ORDER BY NAMA_BARANG"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))
            command.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", rekeningatauPelanggan))

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dataset As New DataSetKL()
                dataset.Load(reader, LoadOption.OverwriteChanges, "retur_penjualan_barang")

                Dim keterangan As String = "                    kasir : " & CmbKasir.Text & "                    Pelanggan : " & CmbRekening.Text

                Dim parameters As New ReportParameterCollection From {
                    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
                    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                }

                ReportViewerReturBarang.LocalReport.DataSources.Clear()
                ReportViewerReturBarang.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("retur_penjualan_barang")))
                ReportViewerReturBarang.LocalReport.SetParameters(parameters)
                ReportViewerReturBarang.RefreshReport()
            End Using
        End Using
    End Sub



    Private Sub FormLapReturJual_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
        Case Keys.F5 : BtnTampilkan.PerformClick()
    End Select
    End Sub

End Class
