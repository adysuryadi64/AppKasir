Imports Microsoft.Reporting.WinForms



Public Class FormLapHutang

    Private Sub FormLapHutang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Nilai keuangan otomatis via nama TxtGrandtotal
        ' Rename TxtBayar/TxtHutang/TxtTotalHutang -> TxtGrandtotal untuk tema otomatis
        Kondisiawal()
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        TampilSupliyer()
        ReportViewer1.LocalReport.DataSources.Clear()
        CmbSupliyer.SelectedIndex = 0
        CmbLunas.SelectedIndex = 0
        CbTanggal.Checked = True
    End Sub

    Public Sub Kondisiawal()
        TxtTotalHutang.Text = 0
        TxtBayar.Text = 0
        TxtHutang.Text = 0
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

    Public Sub TampilSupliyer()
        CmbSupliyer.Items.Clear()
        CmbSupliyer.Items.Add("SEMUA") ' Tambahkan opsi "Semua"
        Dim query As String = "SELECT DISTINCT NAMA FROM hutang_detail WHERE JENIS = 'BELI' ORDER BY NAMA"
        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbSupliyer.Items.Add(rd("NAMA").ToString())
                End While
            End Using
        End Using
        CmbSupliyer.SelectedIndex = 0
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
            Dim Supliyer As String = If(CmbSupliyer.Text = "SEMUA" Or CmbSupliyer.SelectedIndex = 0, "", CmbSupliyer.Text)
            Dim Status As String = GetStatusPembayaran()

            Dim totalHutang As Decimal = 0
            Dim totalBayar As Decimal = 0


            Dim query As String = Nothing
            If LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY PEMBELIAN" Then
                query = "SELECT SUM(TOTAL_HUTANG) AS NOMINALHUTANG, SUM(DIBAYAR) AS NOMINALBAYAR " &
                        "FROM hutang_detail " &
                        "WHERE JENIS = 'BELI' " &
                        "AND (@AwalBulan IS NULL OR TANGGAL_BELI >= @AwalBulan) " &
                        "AND (@AkhirBulan IS NULL OR TANGGAL_BELI <= @AkhirBulan) " &
                        "AND NAMA LIKE @Supliyer " &
                        "AND STATUS LIKE @Status"

            ElseIf LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY PELUNASAN" Then
                query = "SELECT SUM(TOTAL_HUTANG) AS NOMINALHUTANG, SUM(DIBAYAR) AS NOMINALBAYAR " &
                        "FROM hutang_detail " &
                        "WHERE JENIS = 'BELI' " &
                        "AND (@AwalBulan IS NULL OR TANGGAL_BAYAR >= @AwalBulan) " &
                        "AND (@AkhirBulan IS NULL OR TANGGAL_BAYAR <= @AkhirBulan) " &
                        "AND NAMA LIKE @Supliyer " &
                        "AND STATUS LIKE @Status"
            Else
                query = "SELECT SUM(TOTAL_HUTANG) AS NOMINALHUTANG, SUM(DIBAYAR) AS NOMINALBAYAR " &
                        "FROM hutang_detail " &
                        "WHERE JENIS = 'BELI' " &
                        "AND (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) " &
                        "AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) " &
                        "AND NAMA LIKE @Supliyer " &
                        "AND STATUS LIKE @Status"
            End If


            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan)
                cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan)
                cmd.Parameters.AddWithValue("@Supliyer", String.Format("%{0}%", Supliyer))
                cmd.Parameters.AddWithValue("@Status", String.Format("{0}%", Status))

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        totalHutang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINALHUTANG", 0D)
                        totalBayar = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINALBAYAR", 0D)
                    End If
                End Using
            End Using


            ' Tampilkan hasil perhitungan di TextBox
            TxtTotalHutang.Text = totalHutang.ToString("N0", cultureIndonesia)
            TxtBayar.Text = totalBayar.ToString("N0", cultureIndonesia)
            TxtHutang.Text = (totalHutang - totalBayar).ToString("N0", cultureIndonesia)

            Dim queryTampil As String = Nothing
            If LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY PEMBELIAN" Then
                queryTampil = "SELECT ID_BELI AS ID_PEMBELIAN, NAMA AS NAMA_SUPLIYER, TANGGAL_BELI AS TGL_BELI, " &
                              "TOTAL_HUTANG AS GRAND_TOTAL_BELI, DIBAYAR AS PEMBAYARAN, HUTANG AS TAGIHAN, " &
                              "JATUH_TEMPO, TANGGAL_BAYAR AS TGL_BAYAR, DIBAYAR AS NOMINALBAYAR, STATUS AS STATUS_TRANSAKSI_BELI, ID_USER " &
                              "FROM hutang_detail " &
                              "WHERE JENIS = 'BELI' " &
                              "AND (@AwalBulan IS NULL OR TANGGAL_BELI >= @AwalBulan) " &
                              "AND (@AkhirBulan IS NULL OR TANGGAL_BELI <= @AkhirBulan) " &
                              "AND NAMA LIKE @Supliyer " &
                              "AND STATUS LIKE @Status " &
                              "ORDER BY JATUH_TEMPO ASC"

            ElseIf LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY PELUNASAN" Then
                queryTampil = "SELECT ID_BELI AS ID_PEMBELIAN, NAMA AS NAMA_SUPLIYER, TANGGAL_BELI AS TGL_BELI, " &
                              "TOTAL_HUTANG AS GRAND_TOTAL_BELI, DIBAYAR AS PEMBAYARAN, HUTANG AS TAGIHAN, " &
                              "JATUH_TEMPO, TANGGAL_BAYAR AS TGL_BAYAR, DIBAYAR AS NOMINALBAYAR, STATUS AS STATUS_TRANSAKSI_BELI, ID_USER " &
                              "FROM hutang_detail " &
                              "WHERE JENIS = 'BELI' " &
                              "AND (@AwalBulan IS NULL OR TANGGAL_BAYAR >= @AwalBulan) " &
                              "AND (@AkhirBulan IS NULL OR TANGGAL_BAYAR <= @AkhirBulan) " &
                              "AND NAMA LIKE @Supliyer " &
                              "AND STATUS LIKE @Status " &
                              "ORDER BY JATUH_TEMPO ASC"
            Else
                queryTampil = "SELECT ID_BELI AS ID_PEMBELIAN, NAMA AS NAMA_SUPLIYER, TANGGAL_BELI AS TGL_BELI, " &
                              "TOTAL_HUTANG AS GRAND_TOTAL_BELI, DIBAYAR AS PEMBAYARAN, HUTANG AS TAGIHAN, " &
                              "JATUH_TEMPO, TANGGAL_BAYAR AS TGL_BAYAR, DIBAYAR AS NOMINALBAYAR, STATUS AS STATUS_TRANSAKSI_BELI, ID_USER " &
                              "FROM hutang_detail " &
                              "WHERE JENIS = 'BELI' " &
                              "AND (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) " &
                              "AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) " &
                              "AND NAMA LIKE @Supliyer " &
                              "AND STATUS LIKE @Status " &
                              "ORDER BY JATUH_TEMPO ASC"
            End If

            ' Muat laporan berdasarkan kriteria yang dipilih


            Using cmd As New MySqlCommand(queryTampil, conn)
                cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan)
                cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan)
                cmd.Parameters.AddWithValue("@Supliyer", String.Format("%{0}%", Supliyer))
                cmd.Parameters.AddWithValue("@Status", String.Format("{0}%", Status))
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using dataset As New DataSet()
                        dataset.Load(rd, LoadOption.OverwriteChanges, "pembelian_hutang")

                        Dim parameters As New ReportParameterCollection From {
                        New ReportParameter("Supliyer", "Supliyer : " & Supliyer),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                        New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    }

                        ' Menetapkan dataset ke laporan RDLC
                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("pembelian_hutang")))
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

    ' Method untuk mendapatkan status pembayaran dari ComboBox
    Private Function GetStatusPembayaran() As String
        Select Case CmbLunas.SelectedIndex
            Case 1
                Return "Lunas"
            Case 2
                Return "Belum Lunas"
            Case Else
                Return "" ' Untuk "Semua" atau indeks 0
        End Select
    End Function





    Private Sub FormLapHutang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnLunas.PerformClick()
        End Select
    End Sub

End Class
