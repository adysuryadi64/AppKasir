Imports Microsoft.Reporting.WinForms

Public Class FormLapBarangTerlaris

    Private Sub FormLapBarangTerlaris_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        IsiComboBoxJumlah()
        IsiComboBoxLokasi()
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True   ' default mode tanggal
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub

    ' ── Combobox helpers ────────────────────────────────────────────
    Private Sub IsiComboBoxJumlah()
        CmbJumlah.Items.Clear()
        CmbJumlah.Items.AddRange({"10", "20", "25", "50", "100", "Semua"})
        CmbJumlah.SelectedIndex = 0
    End Sub

    Private Sub IsiComboBoxLokasi()
        CmbLokasi.Items.Clear()
        CmbLokasi.Items.AddRange({"SEMUA", "TOKO", "GUDANG"})
        CmbLokasi.SelectedIndex = 0
    End Sub

    ' ── Checkbox saling eksklusif (sama persis pola FormLapHutang) ──
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

    ' ── Tampil laporan ───────────────────────────────────────────────
    Private Sub TampilLaporan()
        Dim tglAwal As DateTime
        Dim tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Exit Sub

        Dim limitStr As String = CmbJumlah.Text
        Dim lokasi As String = CmbLokasi.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND pd.LOKASIBARANG = @LOKASI")
        Dim limitClause As String = If(limitStr = "Semua", "", $"LIMIT {limitStr}")

        Dim query As String =
            "Select pd.ID_BARANG, pd.NAMA_BARANG, " &
            "SUM(Case When pd.LOKASIBARANG = 'TOKO'   THEN pd.QTY_SATUAN ELSE 0 END) AS TERJUAL_TOKO, " &
            "SUM(CASE WHEN pd.LOKASIBARANG = 'GUDANG' THEN pd.QTY_SATUAN ELSE 0 END) AS TERJUAL_GUDANG, " &
            "SUM(pd.QTY_SATUAN) AS TOTAL_TERJUAL, " &
            "SUM(pd.TOTAL_HARGA) AS TOTAL_OMSET " &
            "FROM penjualan_detail pd " &
            "INNER JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL " &
            "WHERE pd.TANGGAL_JUAL BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            "AND p.STATUS_TRANSAKSI <> 'BATAL' " &
            lokasiFilter & " " &
            "GROUP BY pd.ID_BARANG, pd.NAMA_BARANG " &
            "ORDER BY TOTAL_TERJUAL DESC " &
            limitClause

        Dim totalItem As Integer = 0
        Dim totalQty As Decimal = 0

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "BarangTerlaris")

                        Dim totalOmset As Decimal = 0
                        For Each row As DataRow In ds.Tables("BarangTerlaris").Rows
                            totalItem += 1
                            totalQty += Convert.ToDecimal(row("TOTAL_TERJUAL"))
                            totalOmset += Convert.ToDecimal(row("TOTAL_OMSET"))
                        Next

                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalQty.Text = totalQty.ToString("N0")
                        LblTotalOmset.Text = totalOmset.ToString("N0")

                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        Dim judulTop As String = If(limitStr = "Semua", "Semua", "Top " & limitStr)
                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")

                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("BarangTerlaris")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", $"Laporan Barang Terlaris {judulTop} - {judulLokasi}"),
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

    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        TampilLaporan()
    End Sub

    Private Sub FormLapBarangTerlaris_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampil.PerformClick()
        End Select
    End Sub

End Class
