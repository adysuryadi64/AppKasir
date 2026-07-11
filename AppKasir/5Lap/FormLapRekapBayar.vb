Imports Microsoft.Reporting.WinForms

Public Class FormLapRekapBayar
    Inherits System.Windows.Forms.Form

    Public Property JenisLaporan As String = "Hutang"

#Region "Form Events"
    Private Sub FormLapRekapBayar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True
        CmbLokasi.SelectedIndex = 0
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        MuatCmbNama()
        TerapkanJenis()
    End Sub

    Private Sub FormLapRekapBayar_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then BtnTampil.PerformClick()
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub TerapkanJenis()
        Dim isHutang As Boolean = (JenisLaporan = "Hutang")
        ReportViewer1.Visible = isHutang
        ReportViewer2.Visible = Not isHutang
        If isHutang Then
            LblHeaderForm.Text = "REKAP BAYAR HUTANG"
            Me.Text = "Rekap Bayar Hutang"
            LblLabelTotal1.Text = "Total Hutang :"
            LblNama.Text = "Supplier :"
        Else
            LblHeaderForm.Text = "REKAP BAYAR PIUTANG"
            Me.Text = "Rekap Bayar Piutang"
            LblLabelTotal1.Text = "Total Piutang :"
            LblNama.Text = "Pelanggan :"
        End If
        MuatCmbNama()
    End Sub

    Private Sub MuatCmbNama()
        CmbNama.Items.Clear()
        CmbNama.Items.Add("SEMUA")
        Try
            Dim query As String
            If JenisLaporan = "Hutang" Then
                query = "SELECT DISTINCT NAMASUPLIYER FROM hutang ORDER BY NAMASUPLIYER"
            Else
                query = "SELECT DISTINCT NAMA_PELANGGAN FROM piutang ORDER BY NAMA_PELANGGAN"
            End If
            Using cmd As New MySqlCommand(query, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        CmbNama.Items.Add(rd(0).ToString())
                    End While
                End Using
            End Using
        Catch
        End Try
        CmbNama.SelectedIndex = 0
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
        If JenisLaporan = "Hutang" Then TampilHutang() Else TampilPiutang()
    End Sub

    Private Sub TampilHutang()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return

        Dim nama As String = CmbNama.Text
        Dim lokasi As String = CmbLokasi.Text
        Dim namaFilter As String = If(nama = "SEMUA", "", "AND NAMASUPLIYER LIKE @NAMA")
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASI = @LOKASI")

        Dim query As String =
            "SELECT NOBAYARHUTANG, NAMASUPLIYER, TGLPEMBAYARAN, " &
            "LOKASI, TOTALHUTANG, NOMINALBAYAR, SISAHUTANG, ID_USER_BAYAR " &
            "FROM hutang " &
            "WHERE TGLPEMBAYARAN BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            namaFilter & " " & lokasiFilter & " " &
            "ORDER BY TGLPEMBAYARAN"

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If nama <> "SEMUA" Then cmd.Parameters.AddWithValue("@NAMA", "%" & nama & "%")
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RekapBayarHutang")
                        Dim dtHutang As DataTable = ConvertColumnToDateTime(ds.Tables("RekapBayarHutang"), "TGLPEMBAYARAN")
                        Dim totalItem As Integer = dtHutang.Rows.Count
                        Dim totalHutang As Decimal = 0, totalBayar As Decimal = 0, totalSisa As Decimal = 0
                        For Each row As DataRow In dtHutang.Rows
                            totalHutang += Convert.ToDecimal(row("TOTALHUTANG"))
                            totalBayar += Convert.ToDecimal(row("NOMINALBAYAR"))
                            totalSisa += Convert.ToDecimal(row("SISAHUTANG"))
                        Next
                        LblTotal1.Text = "Rp. " & totalHutang.ToString("N0")
                        LblTotal2.Text = "Rp. " & totalBayar.ToString("N0")
                        LblTotal3.Text = "Rp. " & totalSisa.ToString("N0")

                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)

                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtHutang))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Rekap Bayar Hutang - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalHutang", "Rp. " & totalHutang.ToString("N0")),
                            New ReportParameter("TotalDibayar", "Rp. " & totalBayar.ToString("N0")),
                            New ReportParameter("TotalSisa", "Rp. " & totalSisa.ToString("N0")),
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

    Private Sub TampilPiutang()
        Dim tglAwal As DateTime, tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return

        Dim nama As String = CmbNama.Text
        Dim lokasi As String = CmbLokasi.Text
        Dim namaFilter As String = If(nama = "SEMUA", "", "AND NAMA_PELANGGAN LIKE @NAMA")
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASI = @LOKASI")

        Dim query As String =
            "SELECT ID_BAYAR_PIUTANG, NAMA_PELANGGAN, TGL_BAYAR, " &
            "LOKASI, TOTAL_PIUTANG, NOMINAL_BAYAR, SISA_PIUTANG, ID_USER_BAYAR " &
            "FROM piutang " &
            "WHERE TGL_BAYAR BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            namaFilter & " " & lokasiFilter & " " &
            "ORDER BY TGL_BAYAR"

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If nama <> "SEMUA" Then cmd.Parameters.AddWithValue("@NAMA", "%" & nama & "%")
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RekapBayarPiutang")
                        Dim dtPiutang As DataTable = ConvertColumnToDateTime(ds.Tables("RekapBayarPiutang"), "TGL_BAYAR")
                        Dim totalItem As Integer = dtPiutang.Rows.Count
                        Dim totalPiutang As Decimal = 0, totalBayar As Decimal = 0, totalSisa As Decimal = 0
                        For Each row As DataRow In dtPiutang.Rows
                            totalPiutang += Convert.ToDecimal(row("TOTAL_PIUTANG"))
                            totalBayar += Convert.ToDecimal(row("NOMINAL_BAYAR"))
                            totalSisa += Convert.ToDecimal(row("SISA_PIUTANG"))
                        Next
                        LblTotal1.Text = "Rp. " & totalPiutang.ToString("N0")
                        LblTotal2.Text = "Rp. " & totalBayar.ToString("N0")
                        LblTotal3.Text = "Rp. " & totalSisa.ToString("N0")

                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)

                        ReportViewer2.LocalReport.DataSources.Clear()
                        ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtPiutang))
                        ReportViewer2.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Rekap Bayar Piutang - " & judulLokasi),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("TotalItem", totalItem.ToString("N0")),
                            New ReportParameter("TotalPiutang", "Rp. " & totalPiutang.ToString("N0")),
                            New ReportParameter("TotalDibayar", "Rp. " & totalBayar.ToString("N0")),
                            New ReportParameter("TotalSisa", "Rp. " & totalSisa.ToString("N0")),
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



End Class
