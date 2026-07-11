Imports System.Globalization
Imports Microsoft.Reporting.WinForms


Public Class FormLapJurnal
    Private Sub ReportViewer1_Load(sender As Object, e As EventArgs) Handles ReportViewer1.Load
        CbTanggal.Checked = True
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub

    Private Sub CmbBln_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbBln.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub CmbThn_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbThn.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            Dim angkaBulan As String = (CmbBln.SelectedIndex + 1).ToString("D2")
            Dim teksBulanTahunTerpilih As String = angkaBulan & "/" & CmbThn.Text
        End If
    End Sub

    Private Sub CbTanggal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked = True Then
            CbBulan.Checked = False
        End If
    End Sub


    Private Sub CBBulan_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked = True Then
            CbTanggal.Checked = False
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


    ' Event handler untuk tombol BtnLunas (atau BtnHitung)
    Private Sub BtnLunas_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnLunas.Click
        ' Jika tidak ada CheckBox yang dicentang
        If Not CbBulan.Checked And Not CbTanggal.Checked Then
            ' Tampilkan pesan peringatan
            MessageBox.Show("Harap pilih jenis laporan terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            Dim AwalBulan As DateTime
            Dim AkhirBulan As DateTime

            ' Set AwalBulan dan AkhirBulan ke Nothing untuk menampilkan semua data
            If CbTanggal.Checked Then
                AwalBulan = DTPAwal.Value.Date.ToString("yyyy-MM-dd HH:mm:ss")
                AkhirBulan = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss")
            ElseIf CbBulan.Checked Then
                Dim tglAwal As DateTime
                Dim tglAkhir As DateTime
                If Not GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir) Then Exit Sub
                AwalBulan = tglAwal.ToString("yyyy-MM-dd HH:mm:ss")
                AkhirBulan = tglAkhir.ToString("yyyy-MM-dd HH:mm:ss")
            End If

            Cursor = Cursors.WaitCursor

            Dim queryTampil As String = "SELECT NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER " &
                                 "FROM jurnalumum " &
                                 "WHERE (@AwalBulan IS NULL OR TGL_TRANSAKSI >= @AwalBulan) " &
                                 "AND (@AkhirBulan IS NULL OR TGL_TRANSAKSI <= @AkhirBulan) " &
                                 "ORDER BY TGL_TRANSAKSI "

            Using cmd As New MySqlCommand(queryTampil, conn)
                cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan)
                cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using dataset As New DataSet()
                        dataset.Load(rd, LoadOption.OverwriteChanges, "jurnalumum")
                        Dim dtJurnal As DataTable = ConvertColumnToDateTime(dataset.Tables("jurnalumum"), "TGL_TRANSAKSI")

                        Dim Periode As String = "Periode : " & AwalBulan.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " - " & AkhirBulan.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))

                        Dim parameters As New ReportParameterCollection From {
                        New ReportParameter("PERIODE", Periode),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                        New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN)
                    }

                        ' Menetapkan dataset ke laporan RDLC
                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtJurnal))
                        ReportViewer1.LocalReport.SetParameters(parameters)

                        ' Menampilkan laporan RDLC
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

    Private Sub FormLapJurnal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

    End Sub
    Private Sub FormLapJurnal_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnLunas.PerformClick()
        End Select
    End Sub

End Class
