Imports Microsoft.Reporting.WinForms

Public Class FormLapPiutang

    Private Sub FormLapHutang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Nilai keuangan otomatis via nama TxtGrandtotal
        ' Rename TxtBayar/TxtHutang/TxtTotalHutang -> TxtGrandtotal untuk tema otomatis
        Kondisiawal()
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        TampilPelanggan()
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

    Public Sub TampilPelanggan()
        CmbSupliyer.Items.Clear()
        CmbSupliyer.Items.Add("SEMUA") ' Tambahkan opsi "Semua"
        Dim query As String = "SELECT DISTINCT NAMA_PELANGGAN FROM penjualan ORDER BY NAMA_PELANGGAN"
        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbSupliyer.Items.Add(rd("NAMA_PELANGGAN").ToString())
                End While
            End Using
        End Using
        CmbSupliyer.SelectedIndex = 0
    End Sub


    Private Sub BtnHitung_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnLunas.Click
        ' Jika tidak ada CheckBox yang dicentang
        If Not CbBulan.Checked And Not CbTanggal.Checked Then
            ' Tampilkan pesan peringatan
            MessageBox.Show("Harap pilih jenis laporan terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If


        Dim AwalBulan As Date
        Dim AkhirBulan As Date

        ' Set AwalBulan dan AkhirBulan ke Nothing untuk menampilkan semua data
        If CbBulan.Checked Then
            Dim tglAwal As DateTime
            Dim tglAkhir As DateTime
            If Not GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir) Then Exit Sub
            AwalBulan = tglAwal.ToString("yyyy-MM-dd HH:mm:ss")
            AkhirBulan = tglAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        ElseIf CbTanggal.Checked Then
            AwalBulan = DTPAwal.Value.Date.ToString("yyyy-MM-dd HH:mm:ss")
            AkhirBulan = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss")
        End If

        Cursor = Cursors.WaitCursor

        Dim Supliyer As String = If(CmbSupliyer.Text = "SEMUA" Or CmbSupliyer.SelectedIndex = 0, "", CmbSupliyer.Text)

        Dim Status As String
        Select Case CmbLunas.SelectedIndex
            Case 1
                Status = "Lunas"
            Case 2
                Status = "Belum Lunas"
            Case Else
                Status = "" ' Untuk "Semua" atau indeks 0
        End Select

        Dim queryhitung As String
        If LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY PENJUALAN" Then
            queryhitung = "SELECT SUM(PIUTANG) AS HUTANG, SUM(DIBAYAR) AS NOMINAL_BAYAR " &
                          "FROM piutang_detail " &
                          "WHERE JENIS = 'JUAL' " &
                          "AND (@AwalBulan IS NULL OR TANGGAL_JUAL >= @AwalBulan) " &
                          "AND (@AkhirBulan IS NULL OR TANGGAL_JUAL <= @AkhirBulan) " &
                          "AND NAMA LIKE @NAMA_PELANGGAN " &
                          "AND STATUS LIKE @STATUS"
        ElseIf LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY PELUNASAN" Then
            queryhitung = "SELECT SUM(PIUTANG) AS HUTANG, SUM(DIBAYAR) AS NOMINAL_BAYAR " &
                          "FROM piutang_detail " &
                          "WHERE JENIS = 'JUAL' " &
                          "AND (@AwalBulan IS NULL OR TANGGAL_BAYAR >= @AwalBulan) " &
                          "AND (@AkhirBulan IS NULL OR TANGGAL_BAYAR <= @AkhirBulan) " &
                          "AND NAMA LIKE @NAMA_PELANGGAN " &
                          "AND STATUS LIKE @STATUS"
        Else
            queryhitung = "SELECT SUM(PIUTANG) AS HUTANG, SUM(DIBAYAR) AS NOMINAL_BAYAR " &
                          "FROM piutang_detail " &
                          "WHERE JENIS = 'JUAL' " &
                          "AND (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) " &
                          "AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) " &
                          "AND NAMA LIKE @NAMA_PELANGGAN " &
                          "AND STATUS LIKE @STATUS"
        End If



        Using cmdHitung As New MySqlCommand(queryhitung, conn)
            cmdHitung.Parameters.AddWithValue("@AwalBulan", AwalBulan)
            cmdHitung.Parameters.AddWithValue("@AkhirBulan", AkhirBulan)
            cmdHitung.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", Supliyer))
            cmdHitung.Parameters.AddWithValue("@STATUS", String.Format("{0}%", Status))
            Using rd As MySqlDataReader = cmdHitung.ExecuteReader()
                rd.Read()
                If rd.HasRows Then
                    Dim HUTANG As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HUTANG", 0D)
                    Dim NOMINAL_BAYAR As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINAL_BAYAR", 0D)

                    TxtTotalHutang.Text = HUTANG.ToString("N0", cultureIndonesia)
                    TxtBayar.Text = NOMINAL_BAYAR.ToString("N0", cultureIndonesia)
                    TxtHutang.Text = (HUTANG - NOMINAL_BAYAR).ToString("N0", cultureIndonesia)
                End If
            End Using
        End Using

        Dim query As String
        If LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY PENJUALAN" Then
            query = "SELECT ID_JUAL AS ID_PENJUALAN, TANGGAL_JUAL AS TGL_TRANSAKSI, NAMA AS NAMA_PELANGGAN, " &
                    "PIUTANG AS GRAND_TOTAL_STL_PAJAK, PIUTANG, DIBAYAR AS NOMINALBAYARPIUTANG, " &
                    "HUTANG AS SISA_TAGIHAN, JATUH_TEMPO, STATUS AS STATUS_TRANSAKSI, ID_USER " &
                    "FROM piutang_detail " &
                    "WHERE JENIS = 'JUAL' " &
                    "AND (@AwalBulan IS NULL OR TANGGAL_JUAL >= @AwalBulan) " &
                    "AND (@AkhirBulan IS NULL OR TANGGAL_JUAL <= @AkhirBulan) " &
                    "AND NAMA LIKE @NAMA_PELANGGAN " &
                    "AND STATUS LIKE @STATUS " &
                    "ORDER BY JATUH_TEMPO ASC"
        ElseIf LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY PELUNASAN" Then
            query = "SELECT ID_JUAL AS ID_PENJUALAN, TANGGAL_JUAL AS TGL_TRANSAKSI, NAMA AS NAMA_PELANGGAN, " &
                    "PIUTANG AS GRAND_TOTAL_STL_PAJAK, PIUTANG, DIBAYAR AS NOMINALBAYARPIUTANG, " &
                    "HUTANG AS SISA_TAGIHAN, JATUH_TEMPO, STATUS AS STATUS_TRANSAKSI, ID_USER " &
                    "FROM piutang_detail " &
                    "WHERE JENIS = 'JUAL' " &
                    "AND (@AwalBulan IS NULL OR TANGGAL_BAYAR >= @AwalBulan) " &
                    "AND (@AkhirBulan IS NULL OR TANGGAL_BAYAR <= @AkhirBulan) " &
                    "AND NAMA LIKE @NAMA_PELANGGAN " &
                    "AND STATUS LIKE @STATUS " &
                    "ORDER BY JATUH_TEMPO ASC"
        Else
            query = "SELECT ID_JUAL AS ID_PENJUALAN, TANGGAL_JUAL AS TGL_TRANSAKSI, NAMA AS NAMA_PELANGGAN, " &
                    "PIUTANG AS GRAND_TOTAL_STL_PAJAK, PIUTANG, DIBAYAR AS NOMINALBAYARPIUTANG, " &
                    "HUTANG AS SISA_TAGIHAN, JATUH_TEMPO, STATUS AS STATUS_TRANSAKSI, ID_USER " &
                    "FROM piutang_detail " &
                    "WHERE JENIS = 'JUAL' " &
                    "AND (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) " &
                    "AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) " &
                    "AND NAMA LIKE @NAMA_PELANGGAN " &
                    "AND STATUS LIKE @STATUS " &
                    "ORDER BY JATUH_TEMPO ASC"
        End If

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", If(AwalBulan = Date.MinValue, DBNull.Value, CType(AwalBulan, Object)))
            cmd.Parameters.AddWithValue("@AkhirBulan", If(AkhirBulan = Date.MaxValue, DBNull.Value, CType(AkhirBulan, Object)))
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", Supliyer))
            cmd.Parameters.AddWithValue("@STATUS", String.Format("{0}%", Status))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using dataset As New DataSetKL()
                    dataset.Load(rd, LoadOption.OverwriteChanges, "Laporan_piutang")
                    Dim dtPiutang As DataTable = ConvertColumnToDateTime(dataset.Tables("Laporan_piutang"), "TGL_TRANSAKSI")
                    dtPiutang = ConvertColumnToDateTime(dtPiutang, "JATUH_TEMPO")

                    ' Menambahkan parameter ke laporan RDLC
                    Dim parameters As New ReportParameterCollection From {
                        New ReportParameter("Supliyer", "PELANGGAN : " & CmbSupliyer.Text),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                        New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    }

                    ' Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtPiutang))
                    ReportViewer1.LocalReport.SetParameters(parameters)

                    ' Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using

        Cursor = Cursors.Default
    End Sub


    Private Sub FormLapPiutang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnLunas.PerformClick()
        End Select
    End Sub

End Class
