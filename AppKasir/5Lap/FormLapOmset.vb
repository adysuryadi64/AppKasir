Imports Microsoft.Reporting.WinForms

Public Class FormLapOmset
    Inherits System.Windows.Forms.Form

    Public Property JenisLaporan As String = "Pelanggan"

#Region "Form Events"
    Private Sub FormLapOmset_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True
        CmbLokasi.SelectedIndex = 0
        CmbJumlah.Items.AddRange({"10", "20", "25", "50", "100", "Semua"})
        CmbJumlah.SelectedIndex = 0
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        TerapkanJenis()
    End Sub

    Private Sub TerapkanJenis()
        Dim isPelanggan As Boolean = (JenisLaporan = "Pelanggan")
        ReportViewer1.Visible = isPelanggan
        ReportViewer2.Visible = Not isPelanggan
        ' Top N hanya relevan untuk Pelanggan
        LblJumlah.Visible = isPelanggan
        CmbJumlah.Visible = isPelanggan
        If isPelanggan Then
            LblHeaderForm.Text = "OMSET PER PELANGGAN"
            Me.Text = "Omset per Pelanggan"
            LblLabelTotalItem.Text = "Total Pelanggan :"
        Else
            LblHeaderForm.Text = "OMSET PER KATEGORI"
            Me.Text = "Omset per Kategori"
            LblLabelTotalItem.Text = "Total Kategori :"
        End If
    End Sub
#End Region

#Region "Filter Tanggal & Bulan"
    Private Sub CbTanggal_CheckedChanged(sender As Object, e As EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
            DTPAwal.Enabled = True
            DTPAkhir.Enabled = True
        End If
    End Sub

    Private Sub CbBulan_CheckedChanged(sender As Object, e As EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            DTPAwal.Enabled = False
            DTPAkhir.Enabled = False
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
            CmbBln.Enabled = True
            CmbThn.Enabled = True
        Else
            CmbBln.Enabled = False
            CmbThn.Enabled = False
            CmbBln.Items.Clear()
            CmbThn.Items.Clear()
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
        If JenisLaporan = "Pelanggan" Then TampilPelanggan() Else TampilKategori()
    End Sub

    Private Sub TampilPelanggan()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return

        Dim lokasi As String = CmbLokasi.Text
        Dim limitStr As String = CmbJumlah.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND pd.LOKASIBARANG = @LOKASI")
        Dim limitClause As String = If(limitStr = "Semua", "", "LIMIT " & limitStr)

        Dim query As String =
            "SELECT pd.ID_PELANGGAN, pd.NAMA_PELANGGAN, " &
            "COUNT(DISTINCT pd.FAKTUR_JUAL) AS TOTAL_NOTA, " &
            "SUM(pd.QTY_SATUAN) AS TOTAL_QTY, " &
            "SUM(pd.TOTAL_HARGA) AS TOTAL_OMSET " &
            "FROM penjualan_detail pd " &
            "INNER JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL " &
            "WHERE pd.TANGGAL_JUAL BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " &
            "GROUP BY pd.ID_PELANGGAN, pd.NAMA_PELANGGAN " &
            "ORDER BY TOTAL_OMSET DESC " &
            limitClause

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "OmsetPelanggan")

                        Dim totalItem As Integer = ds.Tables("OmsetPelanggan").Rows.Count
                        Dim totalOmset As Decimal = 0
                        For Each row As DataRow In ds.Tables("OmsetPelanggan").Rows
                            totalOmset += Convert.ToDecimal(row("TOTAL_OMSET"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalOmset.Text = "Rp. " & totalOmset.ToString("N0")

                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        Dim judulTop As String = If(limitStr = "Semua", "Semua", "Top " & limitStr)

                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("OmsetPelanggan")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", $"Omset per Pelanggan {judulTop} - {judulLokasi}"),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalOmset", "Rp. " & totalOmset.ToString("N0")),
                            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                        })
                        ReportViewer1.RefreshReport()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub TampilKategori()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return

        Dim lokasi As String = CmbLokasi.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND pd.LOKASIBARANG = @LOKASI")

        Dim query As String =
            "SELECT COALESCE(b.NAMA_KATEGORI, 'Tanpa Kategori') AS NAMA_KATEGORI, " &
            "COUNT(DISTINCT pd.FAKTUR_JUAL) AS TOTAL_NOTA, " &
            "SUM(pd.QTY_SATUAN) AS TOTAL_QTY, " &
            "SUM(pd.TOTAL_HARGA) AS TOTAL_OMSET " &
            "FROM penjualan_detail pd " &
            "INNER JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL " &
            "LEFT JOIN tbl_barang b ON b.ID_BARANG = pd.ID_BARANG " &
            "WHERE pd.TANGGAL_JUAL BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " &
            "GROUP BY b.NAMA_KATEGORI " &
            "ORDER BY TOTAL_OMSET DESC"

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "OmsetKategori")

                        Dim totalItem As Integer = ds.Tables("OmsetKategori").Rows.Count
                        Dim totalOmset As Decimal = 0
                        For Each row As DataRow In ds.Tables("OmsetKategori").Rows
                            totalOmset += Convert.ToDecimal(row("TOTAL_OMSET"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalOmset.Text = "Rp. " & totalOmset.ToString("N0")

                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)

                        ReportViewer2.LocalReport.DataSources.Clear()
                        ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("OmsetKategori")))
                        ReportViewer2.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Omset per Kategori - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalOmset", "Rp. " & totalOmset.ToString("N0")),
                            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                        })
                        ReportViewer2.RefreshReport()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub
#End Region

    Private Sub FormLapOmset_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampil.PerformClick()
        End Select
    End Sub

End Class
