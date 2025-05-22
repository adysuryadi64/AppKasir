Imports Microsoft.Reporting.WinForms


Public Class FormLapkAS

    Private Sub FormLapBarangMasuk_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Dim fontPath As String = System.IO.Path.Combine(Application.StartupPath, "Font Digital\DS-DIGI.TTF") ' Ganti "YourFont.ttf" dengan nama file font yang sesuai
        'Using pfc As New PrivateFontCollection()
        '    pfc.AddFontFile(fontPath)
        '    Using customFont As New Font(pfc.Families(0), 24)
        '        TxtGrantotal.Font = customFont
        '        TxtPiutang.Font = customFont
        '        TxtDiterima.Font = customFont
        '    End Using
        'End Using
        Kondisiawal()

        ReportViewer1.RefreshReport()
    End Sub

    Public Sub Kondisiawal()
        DtpTanggal.Enabled = False
        'CmbKasir.Enabled = False
        CmbBln.Enabled = False
        CmbThn.Enabled = False

        CbBulan.Checked = False
        RbtSemua.Checked = True
        CbTanggal.Checked = True
    End Sub

    Private angkabulan As Integer

    Private Sub Convertbulansetor()
        Select Case CmbBln.Text
            Case "Januari"
                angkabulan = 1
            Case "Februari"
                angkabulan = 2
            Case "Maret"
                angkabulan = 3
            Case "April"
                angkabulan = 4
            Case "Mei"
                angkabulan = 5
            Case "Juni"
                angkabulan = 6
            Case "Juli"
                angkabulan = 7
            Case "Agustus"
                angkabulan = 8
            Case "September"
                angkabulan = 9
            Case "Oktober"
                angkabulan = 10
            Case "November"
                angkabulan = 11
            Case "Desember"
                angkabulan = 12
        End Select
    End Sub

    Public Sub Tampildatablnthn()
        CmbBln.Items.Clear()
        CmbThn.Items.Clear()
        Dim i As Integer
        For i = 2022 To Year(Now)
            CmbThn.Items.Add(i)
        Next
        CmbBln.Items.Add("")
        CmbBln.Items.Add("Januari")
        CmbBln.Items.Add("Februari")
        CmbBln.Items.Add("Maret")
        CmbBln.Items.Add("April")
        CmbBln.Items.Add("Mei")
        CmbBln.Items.Add("Juni")
        CmbBln.Items.Add("Juli")
        CmbBln.Items.Add("Agustus")
        CmbBln.Items.Add("September")
        CmbBln.Items.Add("Oktober")
        CmbBln.Items.Add("November")
        CmbBln.Items.Add("Desember")
    End Sub

    Private Sub CBTanggal_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked = True Then
            DtpTanggal.Enabled = True
            CbBulan.Checked = False
            CmbBln.Enabled = False
            CmbThn.Enabled = False
            CmbBln.Text = ""
            CmbThn.Text = ""
            TampilVoucherTanggal()
        End If
    End Sub
    Private Sub CBBulan_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked = True Then
            Tampildatablnthn()
            CmbBln.Enabled = True
            CmbThn.Enabled = True
            CmbThn.Text = Microsoft.VisualBasic.Format(Now, "yyyy")
            CbTanggal.Checked = False
            'CmbKasir.SelectedIndex = 0
            DtpTanggal.Enabled = False
        Else
            CmbBln.Enabled = False
            CmbThn.Enabled = False
            CmbBln.Text = ""
            CmbThn.Text = ""
        End If
    End Sub
    'Private Sub CBVoucher_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CbVoucher.CheckedChanged
    '    If CbVoucher.Checked = True And CbTanggal.Checked = True Then
    '        CmbKasir.Enabled = True
    '        TampilVoucherTanggal()
    '    ElseIf CbVoucher.Checked = True And CbBulan.Checked = True Then
    '        CmbKasir.Enabled = True
    '        TampilVoucherbulan()
    '    ElseIf CbVoucher.Checked = False Then
    '        CmbKasir.Enabled = False
    '        CmbKasir.Text = ""
    '    End If
    'End Sub

    Private Sub DtpTanggal_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DtpTanggal.ValueChanged
        TampilVoucherTanggal()
    End Sub

    Public Sub TampilVoucherTanggal()
        CmbKasir.Items.Clear()
        CmbKasir.Items.Add("Semua") ' Tambahkan opsi "Semua"
        Dim tanggalAwal As Date = DtpTanggal.Value.Date
        Dim tanggalAkhir As Date = DtpTanggal.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT DISTINCT ID_USER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir ORDER BY ID_USER"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbKasir.Items.Add(rd("ID_USER").ToString())
                End While
            End Using
        End Using
        CmbKasir.SelectedIndex = 0
    End Sub

    Public Sub TampilVoucherbulan()
        CmbKasir.Items.Clear()
        CmbKasir.Items.Add("Semua") ' Tambahkan opsi "Semua"
        Convertbulansetor()
        Dim bulan As Integer = angkabulan
        Dim tahun As Integer = CmbThn.Text

        Dim AwalBulan As New Date(tahun, bulan, 1)
        Dim AkhirBulan As Date = AwalBulan.AddMonths(1).AddDays(-1).AddSeconds(86399)
        Dim query As String = "SELECT DISTINCT ID_USER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan ORDER BY ID_USER"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbKasir.Items.Add(rd("ID_USER").ToString())
                End While
            End Using
        End Using
        CmbKasir.SelectedIndex = 0
    End Sub


    Private Sub BtnHitung_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHitung.Click
        If CbTanggal.Checked = False And CbBulan.Checked = False Then
            MessageBox.Show("Pilih dulu berdasarkan tanggal atau bulan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim query As String
            Dim queryhitung As String

            Cursor = Cursors.WaitCursor

            If CbTanggal.Checked = True And CmbKasir.SelectedIndex = 0 Then
                Dim tanggalAwal As Date = DtpTanggal.Value.Date
                Dim tanggalAkhir As Date = DtpTanggal.Value.Date.AddDays(1).AddTicks(-1)

                If RbtTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND TYPE_AKUN = 'TUNAI'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND TYPE_AKUN = 'TUNAI'"
                ElseIf RbtNonTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND TYPE_AKUN = 'BANK'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND TYPE_AKUN = 'BANK'"
                ElseIf RbNonBayar.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND TYPE_AKUN = 'PIUTANG'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND TYPE_AKUN = 'PIUTANG'"
                Else
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir"
                End If

                Using cmdHitung As New MySqlCommand(queryhitung, conn)
                    cmdHitung.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHitung.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

                    Using rd As MySqlDataReader = cmdHitung.ExecuteReader()
                        rd.Read()

                        If rd.HasRows Then
                            Dim totalHarga As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("TotalHarga")), rd("TotalHarga"), 0)
                            Dim bayar As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("bayar")), rd("bayar"), 0)
                            Dim kembali As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("kembali")), rd("kembali"), 0)
                            Dim hutang As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("hutang")), rd("hutang"), 0)

                            TxtGrantotal.Text = Microsoft.VisualBasic.Format(totalHarga, "N0")
                            TxtDiterima.Text = Microsoft.VisualBasic.Format(bayar - kembali, "N0")
                            TxtPiutang.Text = Microsoft.VisualBasic.Format(hutang, "N0")
                        End If

                    End Using
                End Using

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        Using dataset As New DataSetKL()
                            dataset.Load(rd, LoadOption.OverwriteChanges, "LapKAS")

                            'Menetapkan dataset ke laporan RDLC
                            ReportViewer1.LocalReport.DataSources.Clear()
                            ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("LapKAS")))

                            'Menampilkan laporan RDLC
                            ReportViewer1.RefreshReport()
                        End Using
                    End Using
                End Using

            ElseIf CbBulan.Checked = True And CmbKasir.SelectedIndex = 0 Then
                Convertbulansetor()
                Dim bulan As Integer = angkabulan
                Dim tahun As Integer = CmbThn.Text
                Dim AwalBulan As New Date(tahun, bulan, 1)
                Dim AkhirBulan As Date = AwalBulan.AddMonths(1).AddDays(-1).AddSeconds(86399)

                If RbtTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND TYPE_AKUN = 'TUNAI'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND TYPE_AKUN = 'TUNAI'"
                ElseIf RbtNonTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND TYPE_AKUN = 'BANK'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND TYPE_AKUN = 'BANK'"
                ElseIf RbNonBayar.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND TYPE_AKUN = 'PIUTANG'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND TYPE_AKUN = 'PIUTANG'"
                Else
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan"
                End If

                Using cmdHitung As New MySqlCommand(queryhitung, conn)
                    cmdHitung.Parameters.AddWithValue("@AwalBulan", AwalBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHitung.Parameters.AddWithValue("@AkhirBulan", AkhirBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    Using rd As MySqlDataReader = cmdHitung.ExecuteReader()
                        rd.Read()

                        If rd.HasRows Then
                            Dim totalHarga As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("TotalHarga")), rd("TotalHarga"), 0)
                            Dim bayar As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("bayar")), rd("bayar"), 0)
                            Dim kembali As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("kembali")), rd("kembali"), 0)
                            Dim hutang As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("hutang")), rd("hutang"), 0)

                            TxtGrantotal.Text = Microsoft.VisualBasic.Format(totalHarga, "N0")
                            TxtDiterima.Text = Microsoft.VisualBasic.Format(bayar - kembali, "N0")
                            TxtPiutang.Text = Microsoft.VisualBasic.Format(hutang, "N0")
                        End If

                    End Using
                End Using

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        Using dataset As New DataSetKL()
                            dataset.Load(rd, LoadOption.OverwriteChanges, "LapKAS")

                            'Menetapkan dataset ke laporan RDLC
                            ReportViewer1.LocalReport.DataSources.Clear()
                            ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("LapKAS")))

                            'Menampilkan laporan RDLC
                            ReportViewer1.RefreshReport()
                        End Using
                    End Using
                End Using



            ElseIf CbTanggal.Checked = True And CmbKasir.SelectedIndex <> 0 Then
                Dim tanggalAwal As Date = DtpTanggal.Value.Date
                Dim tanggalAkhir As Date = DtpTanggal.Value.Date.AddDays(1).AddTicks(-1)

                If RbtTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher AND TYPE_AKUN = 'TUNAI'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher AND TYPE_AKUN = 'TUNAI'"
                ElseIf RbtNonTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher AND TYPE_AKUN = 'BANK'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher AND TYPE_AKUN = 'BANK'"
                ElseIf RbNonBayar.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher AND TYPE_AKUN = 'PIUTANG'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher AND TYPE_AKUN = 'PIUTANG'"
                Else
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_USER LIKE @voucher"
                End If


                Using cmdHitung As New MySqlCommand(queryhitung, conn)
                    cmdHitung.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHitung.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHitung.Parameters.AddWithValue("@voucher", "%" & CmbKasir.Text & "%")
                    Using rd As MySqlDataReader = cmdHitung.ExecuteReader()
                        rd.Read()
                        If rd.HasRows Then
                            Dim totalHarga As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("TotalHarga")), rd("TotalHarga"), 0)
                            Dim bayar As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("bayar")), rd("bayar"), 0)
                            Dim kembali As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("kembali")), rd("kembali"), 0)
                            Dim hutang As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("hutang")), rd("hutang"), 0)

                            TxtGrantotal.Text = Microsoft.VisualBasic.Format(totalHarga, "N0")
                            TxtDiterima.Text = Microsoft.VisualBasic.Format(bayar - kembali, "N0")
                            TxtPiutang.Text = Microsoft.VisualBasic.Format(hutang, "N0")
                        End If
                    End Using
                End Using

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@voucher", "%" & CmbKasir.Text & "%")
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        Using dataset As New DataSetKL()
                            dataset.Load(rd, LoadOption.OverwriteChanges, "LapKAS")

                            'Menetapkan dataset ke laporan RDLC
                            ReportViewer1.LocalReport.DataSources.Clear()
                            ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("LapKAS")))

                            'Menampilkan laporan RDLC
                            ReportViewer1.RefreshReport()
                        End Using
                    End Using
                End Using
            ElseIf CbBulan.Checked = True And CmbKasir.SelectedIndex <> 0 Then
                Convertbulansetor()
                Dim bulan As Integer = angkabulan
                Dim tahun As Integer = CmbThn.Text
                Dim AwalBulan As New Date(tahun, bulan, 1)
                Dim AkhirBulan As Date = AwalBulan.AddMonths(1).AddDays(-1).AddSeconds(86399)

                If RbtTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher AND TYPE_AKUN = 'TUNAI'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher AND TYPE_AKUN = 'TUNAI'"
                ElseIf RbtNonTunai.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher AND TYPE_AKUN = 'BANK'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher AND TYPE_AKUN = 'BANK'"
                ElseIf RbNonBayar.Checked Then
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher AND TYPE_AKUN = 'PIUTANG'"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher AND TYPE_AKUN = 'PIUTANG'"
                Else
                    query = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, JENIS_PEMBAYARAN, DISKON_TOTAL_RP, GRAND_TOTAL_STL_PAJAK, PAJAK_RP, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER, ID_KOMPUTER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher"
                    queryhitung = "SELECT Sum(GRAND_TOTAL_STL_PAJAK) as TotalHarga, sum(BAYAR) as bayar, sum(KEMBALI) as kembali, sum(SISA_TAGIHAN) as hutang FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_USER LIKE @voucher"
                End If


                Using cmdHitung As New MySqlCommand(queryhitung, conn)
                    cmdHitung.Parameters.AddWithValue("@AwalBulan", AwalBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHitung.Parameters.AddWithValue("@AkhirBulan", AkhirBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHitung.Parameters.AddWithValue("@voucher", "%" & CmbKasir.Text & "%")
                    Using rd As MySqlDataReader = cmdHitung.ExecuteReader()
                        rd.Read()

                        If rd.HasRows Then
                            Dim totalHarga As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("TotalHarga")), rd("TotalHarga"), 0)
                            Dim bayar As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("bayar")), rd("bayar"), 0)
                            Dim kembali As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("kembali")), rd("kembali"), 0)
                            Dim hutang As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("hutang")), rd("hutang"), 0)

                            TxtGrantotal.Text = Microsoft.VisualBasic.Format(totalHarga, "N0")
                            TxtDiterima.Text = Microsoft.VisualBasic.Format(bayar - kembali, "N0")
                            TxtPiutang.Text = Microsoft.VisualBasic.Format(hutang, "N0")
                        End If

                    End Using
                End Using


                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@voucher", "%" & CmbKasir.Text & "%")
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        Using dataset As New DataSetKL()
                            dataset.Load(rd, LoadOption.OverwriteChanges, "LapKAS")

                            'Menetapkan dataset ke laporan RDLC
                            ReportViewer1.LocalReport.DataSources.Clear()
                            ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("LapKAS")))

                            'Menampilkan laporan RDLC
                            ReportViewer1.RefreshReport()
                        End Using
                    End Using
                End Using

            End If
            Cursor = Cursors.Default
        End If
    End Sub

    Private Sub CmbBln_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbBln.SelectedIndexChanged
        TampilVoucherbulan()
    End Sub

    Private Sub CmbThn_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbThn.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            TampilVoucherbulan()
        End If
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs)
        Close()
    End Sub


End Class