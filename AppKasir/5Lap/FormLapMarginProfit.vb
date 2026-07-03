Imports MySql.Data.MySqlClient
Imports Microsoft.Reporting.WinForms

Public Class FormLapMarginProfit

    Private Sub FormLapMarginProfit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        IsiComboBoxJumlah()
        CmbLokasi.SelectedIndex = 0
        CbMarginAtas.Checked = True
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True   ' default mode tanggal
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub

    Private Sub IsiComboBoxJumlah()
        CmbJumlah.Items.Clear()
        CmbJumlah.Items.AddRange({"10", "20", "25", "50", "100", "Semua"})
        CmbJumlah.SelectedIndex = 0
    End Sub

    ' Mutually exclusive checkboxes
    Private Sub CbMarginAtas_CheckedChanged(sender As Object, e As EventArgs) Handles CbMarginAtas.CheckedChanged
        If CbMarginAtas.Checked Then
            CbMarginTerendah.Checked = False
        End If
    End Sub

    Private Sub CbMarginTerendah_CheckedChanged(sender As Object, e As EventArgs) Handles CbMarginTerendah.CheckedChanged
        If CbMarginTerendah.Checked Then
            CbMarginAtas.Checked = False
        End If
    End Sub

    ' ── Checkbox saling eksklusif (Tanggal / Bulan) ────────────────────
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

    ' ── Ambil rentang tanggal sesuai mode aktif ──────────────────────
    Private Function GetRentangTanggal(ByRef tglAwal As DateTime, ByRef tglAkhir As DateTime) As Boolean
        If CbTanggal.Checked Then
            tglAwal = DTPAwal.Value.Date
            tglAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
            Return True
        ElseIf CbBulan.Checked Then
            Return GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir)
        Else
            MessageBox.Show("Harap pilih mode filter (Tanggal atau Bulan).", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
    End Function

    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        TampilLaporan()
    End Sub

    Private Sub FormLapMarginProfit_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampil.PerformClick()
            Case Keys.Escape : Me.Close()
        End Select
    End Sub

    Private Sub TampilLaporan()
        ' Get date range based on selected filter mode
        Dim tglAwal As DateTime
        Dim tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Exit Sub

        Dim lokasi As String = CmbLokasi.Text
        Dim limitStr As String = CmbJumlah.Text

        ' Build filters
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND pd.LOKASIBARANG = @LOKASI")
        Dim limitClause As String = If(limitStr = "Semua", "", "LIMIT " & limitStr)

        ' Determine sort order based on checkbox
        Dim orderBy As String = If(CbMarginAtas.Checked, "DESC", "ASC")

        ' Build query
        Dim query As String =
            "SELECT " &
            "    pd.ID_BARANG, " &
            "    pd.NAMA_BARANG, " &
            "    AVG(pd.HARGA_JUAL) AS avg_harga_jual, " &
            "    AVG(pd.HARGA_BELI) AS avg_harga_beli, " &
            "    AVG(CASE WHEN pd.HARGA_JUAL > 0 THEN (pd.HARGA_JUAL - pd.HARGA_BELI) / pd.HARGA_JUAL * 100 ELSE 0 END) AS margin_persen, " &
            "    SUM(pd.TOTAL_HARGA) AS total_omzet, " &
            "    SUM(pd.QTY_SATUAN) AS total_qty, " &
            "    b.HARGA_JUAL_UMUM_KECIL AS harga_jual_master, " &
            "    b.HPP_UMUM_KECIL AS hpp_master, " &
            "    CASE WHEN b.HARGA_JUAL_UMUM_KECIL > 0 THEN (b.HARGA_JUAL_UMUM_KECIL - b.HPP_UMUM_KECIL) / b.HARGA_JUAL_UMUM_KECIL * 100 ELSE 0 END AS margin_master " &
            "FROM penjualan_detail pd " &
            "INNER JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL " &
            "LEFT JOIN tbl_barang b ON b.ID_BARANG = pd.ID_BARANG " &
            "WHERE pd.TANGGAL_JUAL BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " &
            "GROUP BY pd.ID_BARANG, pd.NAMA_BARANG, b.HARGA_JUAL_UMUM_KECIL, b.HPP_UMUM_KECIL " &
            "HAVING total_omzet > 0 " &
            "ORDER BY margin_persen " & orderBy & " " &
            limitClause

        Try
            Cursor = Cursors.WaitCursor

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "MarginProfit")

                        ' Calculate totals
                        Dim totalItem As Integer = ds.Tables("MarginProfit").Rows.Count
                        Dim totalQty As Decimal = 0
                        Dim totalOmset As Decimal = 0

                        For Each row As DataRow In ds.Tables("MarginProfit").Rows
                            totalQty += Convert.ToDecimal(row("total_qty"))
                            totalOmset += Convert.ToDecimal(row("total_omzet"))
                        Next

                        ' Update labels
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalQty.Text = totalQty.ToString("N0")
                        LblTotalOmset.Text = "Rp. " & totalOmset.ToString("N0")

                        ' Build report parameters
                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        Dim jenisMargin As String = If(CbMarginAtas.Checked, "Margin Tertinggi", "Margin Terendah")

                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("MarginProfit")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Laporan Profit Margin - " & jenisMargin & " - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalQty", totalQty.ToString("N0")),
                            New ReportParameter("TotalOmset", "Rp. " & totalOmset.ToString("N0")),
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

End Class
