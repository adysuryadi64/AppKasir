Imports Microsoft.Reporting.WinForms

Public Class FormLapRanking
    Inherits System.Windows.Forms.Form

    Public Property JenisLaporan As String = "Supplier"

#Region "Form Events"
    Private Sub FormLapRanking_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True
        CmbLokasi.SelectedIndex = 0
        CmbJumlah.Items.AddRange({"10", "20", "25", "50", "100", "Semua"})
        CmbJumlah.SelectedIndex = 0
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        ReportViewer3.LocalReport.DataSources.Clear()
        MuatCmbSupplier()
        TerapkanJenis()
    End Sub

    Private Sub FormLapRanking_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then BtnTampil.PerformClick()
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub TerapkanJenis()
        ReportViewer1.Visible = (JenisLaporan = "Supplier")
        ReportViewer2.Visible = (JenisLaporan = "BarangBeli")
        ReportViewer3.Visible = (JenisLaporan = "Kasir")
        LblSupplier.Visible = (JenisLaporan = "BarangBeli")
        CmbSupplier.Visible = (JenisLaporan = "BarangBeli")
        Select Case JenisLaporan
            Case "Supplier"
                LblHeaderForm.Text = "RANKING SUPPLIER" : Me.Text = "Ranking Supplier"
                LblLabelTotalItem.Text = "Total Supplier :"
                LblLabelTotalNilai.Text = "Total Pembelian :"
            Case "BarangBeli"
                LblHeaderForm.Text = "RANKING BARANG TERBANYAK DIBELI" : Me.Text = "Ranking Barang Dibeli"
                LblLabelTotalItem.Text = "Total Barang :"
                LblLabelTotalNilai.Text = "Total Pembelian :"
            Case "Kasir"
                LblHeaderForm.Text = "RANKING KASIR PENJUALAN" : Me.Text = "Ranking Kasir"
                LblLabelTotalItem.Text = "Total Kasir :"
                LblLabelTotalNilai.Text = "Total Omset :"
        End Select
    End Sub

    Private Sub MuatCmbSupplier()
        CmbSupplier.Items.Clear()
        CmbSupplier.Items.Add("SEMUA")
        Try
            Using cmd As New MySqlCommand("SELECT DISTINCT NAMA_SUPLIYER FROM pembelian ORDER BY NAMA_SUPLIYER", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        CmbSupplier.Items.Add(rd("NAMA_SUPLIYER").ToString())
                    End While
                End Using
            End Using
        Catch
        End Try
        CmbSupplier.SelectedIndex = 0
    End Sub
#End Region

#Region "Filter Tanggal & Bulan"
    Private Sub CbTanggal_CheckedChanged(sender As Object, e As EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
            DTPAwal.Enabled = True : DTPAkhir.Enabled = True
        End If
    End Sub

    Private Sub CbBulan_CheckedChanged(sender As Object, e As EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            DTPAwal.Enabled = False : DTPAkhir.Enabled = False
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
            CmbBln.Enabled = True : CmbThn.Enabled = True
        Else
            CmbBln.Enabled = False : CmbThn.Enabled = False
            CmbBln.Items.Clear() : CmbThn.Items.Clear()
        End If
    End Sub

    Private Function GetRentangTanggal(ByRef tglAwal As DateTime, ByRef tglAkhir As DateTime) As Boolean
        If CbTanggal.Checked Then
            tglAwal = DTPAwal.Value.Date
            tglAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
            Return True
        ElseIf CbBulan.Checked Then
            Return GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir)
        Else
            MessageBox.Show("Harap pilih mode filter (Tanggal atau Bulan).", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
    End Function
#End Region

#Region "Tampil Laporan"
    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        Select Case JenisLaporan
            Case "Supplier" : TampilSupplier()
            Case "BarangBeli" : TampilBarangBeli()
            Case "Kasir" : TampilKasir()
        End Select
    End Sub

    Private Sub TampilSupplier()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return
        Dim lokasi As String = CmbLokasi.Text
        Dim limitStr As String = CmbJumlah.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASI = @LOKASI")
        Dim limitClause As String = If(limitStr = "Semua", "", "LIMIT " & limitStr)
        Dim query As String =
            "SELECT ID_SUPPLIER, NAMA_SUPLIYER, " &
            "COUNT(ID_PEMBELIAN) AS TOTAL_TRANSAKSI, " &
            "SUM(TOTAL_QTY) AS TOTAL_QTY, " &
            "SUM(GRAND_TOTAL_BELI) AS TOTAL_PEMBELIAN " &
            "FROM pembelian " &
            "WHERE TGL_BELI BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " &
            "GROUP BY ID_SUPPLIER, NAMA_SUPLIYER " &
            "ORDER BY TOTAL_PEMBELIAN DESC " & limitClause
        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RankingSupplier")
                        Dim totalItem As Integer = ds.Tables("RankingSupplier").Rows.Count
                        Dim totalNilai As Decimal = 0
                        For Each row As DataRow In ds.Tables("RankingSupplier").Rows
                            totalNilai += Convert.ToDecimal(row("TOTAL_PEMBELIAN"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalNilai.Text = "Rp. " & totalNilai.ToString("N0")
                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("RankingSupplier")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Ranking Supplier - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalNilai", "Rp. " & totalNilai.ToString("N0")),
                            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                        })
                        ReportViewer1.RefreshReport()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub TampilBarangBeli()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return
        Dim lokasi As String = CmbLokasi.Text
        Dim supplier As String = CmbSupplier.Text
        Dim limitStr As String = CmbJumlah.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND pd.LOKASI = @LOKASI")
        Dim supplierFilter As String = If(supplier = "SEMUA", "", "AND pd.NAMA_SUPLIYER = @SUPLIYER")
        Dim limitClause As String = If(limitStr = "Semua", "", "LIMIT " & limitStr)
        Dim query As String =
            "SELECT pd.ID_BARANG, pd.NAMA_BARANG, pd.NAMA_SUPLIYER, " &
            "SUM(pd.QTY_SAT) AS TOTAL_QTY, SUM(pd.TOTAL) AS TOTAL_PEMBELIAN " &
            "FROM pembelian_detail pd " &
            "INNER JOIN pembelian p ON p.ID_PEMBELIAN = pd.FAKTUR_BELI " &
            "WHERE pd.TANGGAL_MASUK BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " & supplierFilter & " " &
            "GROUP BY pd.ID_BARANG, pd.NAMA_BARANG, pd.NAMA_SUPLIYER " &
            "ORDER BY TOTAL_QTY DESC " & limitClause
        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)
                If supplier <> "SEMUA" Then cmd.Parameters.AddWithValue("@SUPLIYER", supplier)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RankingBarangBeli")
                        Dim totalItem As Integer = ds.Tables("RankingBarangBeli").Rows.Count
                        Dim totalNilai As Decimal = 0
                        For Each row As DataRow In ds.Tables("RankingBarangBeli").Rows
                            totalNilai += Convert.ToDecimal(row("TOTAL_PEMBELIAN"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalNilai.Text = "Rp. " & totalNilai.ToString("N0")
                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        ReportViewer2.LocalReport.DataSources.Clear()
                        ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("RankingBarangBeli")))
                        ReportViewer2.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Ranking Barang Dibeli - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalNilai", "Rp. " & totalNilai.ToString("N0")),
                            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                        })
                        ReportViewer2.RefreshReport()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub TampilKasir()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return
        Dim lokasi As String = CmbLokasi.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASIBARANG = @LOKASI")
        Dim query As String =
            "SELECT ID_USER, COUNT(ID_PENJUALAN) AS TOTAL_TRANSAKSI, " &
            "SUM(GRAND_TOTAL_STL_PAJAK) AS TOTAL_OMSET " &
            "FROM penjualan " &
            "WHERE TGL_TRANSAKSI BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " &
            "GROUP BY ID_USER ORDER BY TOTAL_OMSET DESC"
        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RankingKasir")
                        Dim totalItem As Integer = ds.Tables("RankingKasir").Rows.Count
                        Dim totalNilai As Decimal = 0
                        For Each row As DataRow In ds.Tables("RankingKasir").Rows
                            totalNilai += Convert.ToDecimal(row("TOTAL_OMSET"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalNilai.Text = "Rp. " & totalNilai.ToString("N0")
                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        ReportViewer3.LocalReport.DataSources.Clear()
                        ReportViewer3.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("RankingKasir")))
                        ReportViewer3.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Ranking Kasir - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalNilai", "Rp. " & totalNilai.ToString("N0")),
                            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                        })
                        ReportViewer3.RefreshReport()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub
#End Region



End Class
