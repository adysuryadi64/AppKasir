Imports Microsoft.Reporting.WinForms

Public Class FormLapStokMinim_takGerak

    ''' <summary>
    ''' Set dari FormUtama sebelum Show(). Nilai: "StokMinimum" atau "BarangTidakBergerak"
    ''' </summary>
    Public Property JenisLaporan As String = "StokMinimum"

#Region "Form Events"
    Private Sub FormLapStokMinimum_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ModuleTheme.SetWarnaLabelWarning(LblTidakTerjualSejak)
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True
        CmbLokasi.SelectedIndex = 0
        MuatKategori()
        ReportViewer1.LocalReport.DataSources.Clear()
        TerapkanJenisLaporan()
    End Sub

    Private Sub FormLapStokMinimum_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F8 Then BtnTampil.PerformClick()
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub TerapkanJenisLaporan()
        Dim isBTB As Boolean = (JenisLaporan = "BarangTidakBergerak")

        ' Tampilkan viewer yang sesuai
        ReportViewer1.Visible = Not isBTB   ' StokMinimum
        ReportViewer2.Visible = isBTB       ' BarangTidakBergerak

        ' BTB: hanya tampil label + DTPAwal (sejak tanggal)
        ' Stok Minimum: semua filter tanggal disembunyikan
        LblTidakTerjualSejak.Visible = isBTB
        DTPAwal.Visible = isBTB
        CbTanggal.Visible = False
        Label1.Visible = False
        DTPAkhir.Visible = False
        CbBulan.Visible = False
        CmbBln.Visible = False
        CmbThn.Visible = False

        If isBTB Then
            LblHeaderForm.Text = "LAPORAN BARANG TIDAK BERGERAK"
            Me.Text = "Laporan Barang Tidak Bergerak"
        Else
            LblHeaderForm.Text = "LAPORAN STOK MINIMUM"
            Me.Text = "Laporan Stok Minimum"
        End If
    End Sub

    Private Sub MuatKategori()
        CmbKategori.Items.Clear()
        CmbKategori.Items.Add("SEMUA")
        Try
            Using cmd As New MySqlCommand(
                "SELECT DISTINCT NAMA_KATEGORI FROM tbl_barang " &
                "WHERE NAMA_KATEGORI IS NOT NULL AND NAMA_KATEGORI <> '' " &
                "ORDER BY NAMA_KATEGORI", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        CmbKategori.Items.Add(rd("NAMA_KATEGORI").ToString())
                    End While
                End Using
            End Using
        Catch
        End Try
        CmbKategori.SelectedIndex = 0
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
        ' Mode BTB: langsung pakai DTPAwal saja (tidak butuh rentang)
        If JenisLaporan = "BarangTidakBergerak" Then
            tglAwal = DTPAwal.Value.Date
            Return True
        End If
        ' Mode Stok Minimum: tidak pakai filter tanggal sama sekali
        Return True
    End Function
#End Region

#Region "Tampil Laporan"
    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        If JenisLaporan = "BarangTidakBergerak" Then
            TampilBarangTidakBergerak()
        Else
            TampilStokMinimum()
        End If
    End Sub

    Private Sub TampilStokMinimum()
        Dim lokasi As String = CmbLokasi.Text
        Dim kategori As String = CmbKategori.Text

        Dim whereStok As String
        Select Case lokasi
            Case "TOKO"
                whereStok = "STOK_MIN > 0 AND STOK_TOKO <= STOK_MIN"
            Case "GUDANG"
                whereStok = "STOK_MIN > 0 AND STOK_GUDANG <= STOK_MIN"
            Case Else ' SEMUA
                whereStok = "STOK_MIN > 0 AND (STOK_TOKO + STOK_GUDANG) <= STOK_MIN"
        End Select

        Dim kategoriFilter As String = If(kategori = "SEMUA", "", "AND NAMA_KATEGORI = @KATEGORI")

        Dim query As String =
            "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, " &
            "STOK_TOKO, STOK_GUDANG, " &
            "(STOK_TOKO + STOK_GUDANG) AS TOTAL_STOK, " &
            "STOK_MIN, SATUAN_STOK " &
            "FROM tbl_barang " &
            "WHERE " & whereStok & " " &
            kategoriFilter & " " &
            "ORDER BY NAMA_BARANG"

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                If kategori <> "SEMUA" Then cmd.Parameters.AddWithValue("@KATEGORI", kategori)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "StokMinimum")

                        Dim totalItem As Integer = ds.Tables("StokMinimum").Rows.Count
                        LblTotalItem.Text = totalItem.ToString("N0")

                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        Dim judulKategori As String = If(kategori = "SEMUA", "Semua Kategori", kategori)

                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("StokMinimum")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Laporan Stok Minimum - " & judulLokasi & " - " & judulKategori),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
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

    Private Sub TampilBarangTidakBergerak()
        Dim tglAwal As DateTime
        Dim tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return

        Dim lokasi As String = CmbLokasi.Text
        Dim kategori As String = CmbKategori.Text

        Dim lokasiFilterStok As String
        Select Case lokasi
            Case "TOKO"
                lokasiFilterStok = "b.STOK_TOKO > 0"
            Case "GUDANG"
                lokasiFilterStok = "b.STOK_GUDANG > 0"
            Case Else
                lokasiFilterStok = "(b.STOK_TOKO > 0 OR b.STOK_GUDANG > 0)"
        End Select

        Dim lokasiSubquery As String = If(lokasi = "SEMUA", "", "AND h.LOKASI = @LOKASI")
        Dim kategoriFilter As String = If(kategori = "SEMUA", "", "AND b.NAMA_KATEGORI = @KATEGORI")

        Dim query As String =
            "SELECT b.ID_BARANG, b.NAMA_BARANG, b.NAMA_KATEGORI, " &
            "b.STOK_TOKO, b.STOK_GUDANG, " &
            "(b.STOK_TOKO + b.STOK_GUDANG) AS TOTAL_STOK, " &
            "b.SATUAN_STOK, " &
            "COALESCE((" &
            "  SELECT DATE_FORMAT(MAX(h2.TANGGAL), '%d/%m/%Y') " &
            "  FROM historybarang h2 " &
            "  WHERE h2.ID_BARANG = b.ID_BARANG " &
            "  AND h2.JENIS = 'PENJUALAN' " &
            lokasiSubquery.Replace("h.LOKASI", "h2.LOKASI") & " " &
            "), 'Belum pernah') AS TERAKHIR_TERJUAL " &
            "FROM tbl_barang b " &
            "WHERE " & lokasiFilterStok & " " &
            kategoriFilter & " " &
            "AND NOT EXISTS ( " &
            "  SELECT 1 FROM historybarang h " &
            "  WHERE h.ID_BARANG = b.ID_BARANG " &
            "  AND h.JENIS = 'PENJUALAN' " &
            "  AND h.TANGGAL >= @TGL_AWAL " &
            lokasiSubquery & " " &
            ") " &
            "ORDER BY b.NAMA_BARANG"

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)
                If kategori <> "SEMUA" Then cmd.Parameters.AddWithValue("@KATEGORI", kategori)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "BarangTidakBergerak")

                        Dim totalItem As Integer = ds.Tables("BarangTidakBergerak").Rows.Count
                        LblTotalItem.Text = totalItem.ToString("N0")

                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        Dim judulKategori As String = If(kategori = "SEMUA", "Semua Kategori", kategori)
                        Dim judulTgl As String = "Tidak terjual sejak " & tglAwal.ToString("dd/MM/yyyy")

                        ReportViewer2.LocalReport.DataSources.Clear()
                        ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("BarangTidakBergerak")))
                        ReportViewer2.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Laporan Barang Tidak Bergerak - " & judulLokasi & " - " & judulKategori),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
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

    Private Sub FormLapStokMinim_takGerak_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampil.PerformClick()
        End Select
    End Sub

End Class
