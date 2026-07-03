Imports Microsoft.Reporting.WinForms


Public Class FormLapkAS

    Private Sub FormLapBarangMasuk_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Nilai keuangan otomatis via nama TxtGrandtotal
        ' Rename TxtDiterima/TxtGrantotal/TxtPiutang -> TxtGrandtotal untuk tema otomatis

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
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
            CmbBln.Enabled = True
            CmbThn.Enabled = True
            CbTanggal.Checked = False
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
        CmbKasir.Items.Add("Semua")

        Dim tglAwal As DateTime
        Dim tglAkhir As DateTime
        If Not GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir) Then Return

        Dim query As String = "SELECT DISTINCT ID_USER FROM penjualan WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan ORDER BY ID_USER"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AkhirBulan", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
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
                            Dim totalHarga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TotalHarga", 0D)
                            Dim bayar As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "bayar", 0D)
                            Dim kembali As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "kembali", 0D)
                            Dim hutang As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "hutang", 0D)

                            TxtGrantotal.Text = totalHarga.ToString("N0", cultureIndonesia)
                            TxtDiterima.Text = (bayar - kembali).ToString("N0", cultureIndonesia)
                            TxtPiutang.Text = hutang.ToString("N0", cultureIndonesia)
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
                Dim tglAwal1 As DateTime
                Dim tglAkhir1 As DateTime
                If Not GetRentangBulan(CmbBln, CmbThn, tglAwal1, tglAkhir1) Then
                    Cursor = Cursors.Default
                    Return
                End If
                Dim AwalBulan As Date = tglAwal1
                Dim AkhirBulan As Date = tglAkhir1

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
                            Dim totalHarga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TotalHarga", 0D)
                            Dim bayar As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "bayar", 0D)
                            Dim kembali As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "kembali", 0D)
                            Dim hutang As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "hutang", 0D)

                            TxtGrantotal.Text = totalHarga.ToString("N0", cultureIndonesia)
                            TxtDiterima.Text = (bayar - kembali).ToString("N0", cultureIndonesia)
                            TxtPiutang.Text = hutang.ToString("N0", cultureIndonesia)
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
                            Dim totalHarga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TotalHarga", 0D)
                            Dim bayar As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "bayar", 0D)
                            Dim kembali As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "kembali", 0D)
                            Dim hutang As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "hutang", 0D)

                            TxtGrantotal.Text = totalHarga.ToString("N0", cultureIndonesia)
                            TxtDiterima.Text = (bayar - kembali).ToString("N0", cultureIndonesia)
                            TxtPiutang.Text = hutang.ToString("N0", cultureIndonesia)
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
                Dim tglAwal2 As DateTime
                Dim tglAkhir2 As DateTime
                If Not GetRentangBulan(CmbBln, CmbThn, tglAwal2, tglAkhir2) Then
                    Cursor = Cursors.Default
                    Return
                End If
                Dim AwalBulan As Date = tglAwal2
                Dim AkhirBulan As Date = tglAkhir2

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
                            Dim totalHarga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TotalHarga", 0D)
                            Dim bayar As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "bayar", 0D)
                            Dim kembali As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "kembali", 0D)
                            Dim hutang As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "hutang", 0D)

                            TxtGrantotal.Text = totalHarga.ToString("N0", cultureIndonesia)
                            TxtDiterima.Text = (bayar - kembali).ToString("N0", cultureIndonesia)
                            TxtPiutang.Text = hutang.ToString("N0", cultureIndonesia)
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


    Private Sub FormLapkAS_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnHitung.PerformClick()
        End Select
    End Sub

End Class
