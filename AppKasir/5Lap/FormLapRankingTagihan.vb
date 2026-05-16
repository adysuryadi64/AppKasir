Imports Microsoft.Reporting.WinForms

Public Class FormLapRankingTagihan
    Inherits System.Windows.Forms.Form

    Public Property JenisLaporan As String = "Piutang"

#Region "Form Events"
    Private Sub FormLapRankingTagihan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        CmbLokasi.SelectedIndex = 0
        CmbJumlah.Items.AddRange({"10", "20", "25", "50", "100", "Semua"})
        CmbJumlah.SelectedIndex = 0
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        TerapkanJenis()
    End Sub

    Private Sub FormLapRankingTagihan_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then BtnTampil.PerformClick()
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub TerapkanJenis()
        Dim isPiutang As Boolean = (JenisLaporan = "Piutang")
        ReportViewer1.Visible = isPiutang
        ReportViewer2.Visible = Not isPiutang
        If isPiutang Then
            LblHeaderForm.Text = "RANKING PELANGGAN PIUTANG TERBESAR"
            Me.Text = "Ranking Piutang Pelanggan"
            LblLabelTotalItem.Text = "Total Pelanggan :"
            LblLabelTotalNilai.Text = "Total Piutang :"
        Else
            LblHeaderForm.Text = "RANKING SUPPLIER HUTANG TERBESAR"
            Me.Text = "Ranking Hutang Supplier"
            LblLabelTotalItem.Text = "Total Supplier :"
            LblLabelTotalNilai.Text = "Total Hutang :"
        End If
    End Sub
#End Region

#Region "Tampil Laporan"
    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        If JenisLaporan = "Piutang" Then TampilPiutang() Else TampilHutang()
    End Sub

    Private Sub TampilPiutang()
        Dim lokasi As String = CmbLokasi.Text
        Dim limitStr As String = CmbJumlah.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASIBARANG = @LOKASI")
        Dim limitClause As String = If(limitStr = "Semua", "", "LIMIT " & limitStr)
        Dim query As String =
            "SELECT ID_PELANGGAN, NAMA_PELANGGAN, " &
            "COUNT(ID_PENJUALAN) AS TOTAL_NOTA, " &
            "SUM(SISA_TAGIHAN) AS TOTAL_PIUTANG " &
            "FROM penjualan " &
            "WHERE STATUS_TRANSAKSI <> 'BATAL' " &
            "AND SISA_TAGIHAN > 0 " &
            lokasiFilter & " " &
            "GROUP BY ID_PELANGGAN, NAMA_PELANGGAN " &
            "ORDER BY TOTAL_PIUTANG DESC " & limitClause
        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RankingPiutang")
                        Dim totalItem As Integer = ds.Tables("RankingPiutang").Rows.Count
                        Dim totalNilai As Decimal = 0
                        For Each row As DataRow In ds.Tables("RankingPiutang").Rows
                            totalNilai += Convert.ToDecimal(row("TOTAL_PIUTANG"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalNilai.Text = "Rp. " & totalNilai.ToString("N0")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("RankingPiutang")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Ranking Piutang Pelanggan - " & judulLokasi),
                            New ReportParameter("Periode", "Snapshot saat ini"),
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

    Private Sub TampilHutang()
        Dim lokasi As String = CmbLokasi.Text
        Dim limitStr As String = CmbJumlah.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASI = @LOKASI")
        Dim limitClause As String = If(limitStr = "Semua", "", "LIMIT " & limitStr)
        Dim query As String =
            "SELECT ID_SUPPLIER, NAMA_SUPLIYER, " &
            "COUNT(ID_PEMBELIAN) AS TOTAL_NOTA, " &
            "SUM(TAGIHAN) AS TOTAL_HUTANG " &
            "FROM pembelian " &
            "WHERE STATUS_TRANSAKSI_BELI = 'Belum Lunas' " &
            lokasiFilter & " " &
            "GROUP BY ID_SUPPLIER, NAMA_SUPLIYER " &
            "ORDER BY TOTAL_HUTANG DESC " & limitClause
        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSetKL()
                        ds.Load(rd, LoadOption.OverwriteChanges, "RankingHutang")
                        Dim totalItem As Integer = ds.Tables("RankingHutang").Rows.Count
                        Dim totalNilai As Decimal = 0
                        For Each row As DataRow In ds.Tables("RankingHutang").Rows
                            totalNilai += Convert.ToDecimal(row("TOTAL_HUTANG"))
                        Next
                        LblTotalItem.Text = totalItem.ToString("N0")
                        LblTotalNilai.Text = "Rp. " & totalNilai.ToString("N0")
                        Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                        ReportViewer2.LocalReport.DataSources.Clear()
                        ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("RankingHutang")))
                        ReportViewer2.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("JenisLaporan", "Ranking Hutang Supplier - " & judulLokasi),
                            New ReportParameter("Periode", "Snapshot saat ini"),
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
#End Region



End Class
