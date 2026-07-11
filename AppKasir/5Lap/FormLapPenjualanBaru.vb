Imports Microsoft.Reporting.WinForms

Public Class FormLapPenjualanBaru

    Private Sub FormLapPenjualanBaru_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Cursor = Cursors.WaitCursor
        CbTanggal.Checked = True
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        Select Case LblHeaderForm.Text
            Case "LAPORAN REKAP PENJUALAN NOTA"
                Panel1.Visible = True
                Panel2.Visible = False
                Panel3.Visible = False
                Panel4.Visible = False
                Panel5.Visible = False

            Case "LAPORAN REKAP PENJUALAN BARANG"
                Panel1.Visible = True
                Panel2.Visible = False
                Panel3.Visible = False
                Panel4.Visible = False
                Panel5.Visible = False
            Case "LAPORAN PENJUALAN"
                Panel1.Visible = False
                Panel2.Visible = True
                Panel3.Visible = False
                Panel4.Visible = False
                Panel5.Visible = False
            Case "LAPORAN PENJUALAN DETAIL"
                Panel1.Visible = False
                Panel2.Visible = False
                Panel3.Visible = True
                Panel4.Visible = False
                Panel5.Visible = False
            Case "LAPORAN BARANG PENJUALAN"
                Panel1.Visible = False
                Panel2.Visible = False
                Panel3.Visible = False
                Panel4.Visible = True
                Panel5.Visible = False
            Case "LAPORAN PENJUALAN DIHUTANG"
                Panel1.Visible = False
                Panel2.Visible = False
                Panel3.Visible = False
                Panel4.Visible = False
                Panel5.Visible = True
        End Select
        Cursor = Cursors.Default
    End Sub

    Private Sub TampilkanLaporan(ByVal judul As String)
        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If


        Select Case judul
            Case "LAPORAN REKAP PENJUALAN NOTA"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilRekening(tanggalAwal, tanggalAkhir)
            Case "LAPORAN REKAP PENJUALAN BARANG"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilPelanggan(tanggalAwal, tanggalAkhir)
            Case "LAPORAN PENJUALAN"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilRekening(tanggalAwal, tanggalAkhir)
            Case "LAPORAN PENJUALAN DETAIL"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilPelanggan(tanggalAwal, tanggalAkhir)
            Case "LAPORAN BARANG PENJUALAN"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilPelanggan(tanggalAwal, tanggalAkhir)
            Case "LAPORAN PENJUALAN DIHUTANG"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilRekening(tanggalAwal, tanggalAkhir)
        End Select
    End Sub



    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            Dim angkaBulan As String = (CmbBln.SelectedIndex + 1).ToString("D2")
            Dim teksBulanTahunTerpilih As String = angkaBulan & "/" & CmbThn.Text
            TampilkanLaporan(LblHeaderForm.Text)
        End If
    End Sub

    Public Sub TampilKasir(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        CmbKasir.Items.Clear()
        CmbKasir.Items.Add("Semua")

        Dim query As String = "SELECT DISTINCT ID_USER FROM PENJUALAN WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan ORDER BY ID_USER"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbKasir.Items.Add(rd("ID_USER").ToString())
                End While
            End Using
        End Using

        CmbKasir.SelectedIndex = 0
    End Sub



    Private Sub TampilRekening(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Label5.Text = "Rekening"

        CmbRekening.Items.Clear()
        CmbRekening.Items.Add("SEMUA")
        Dim query As String = "SELECT DISTINCT JENIS_PEMBAYARAN FROM PENJUALAN WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan ORDER BY JENIS_PEMBAYARAN"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        CmbRekening.Items.Add(reader("JENIS_PEMBAYARAN").ToString())
                    End While
                End If
            End Using
        End Using
        CmbRekening.SelectedIndex = 0
    End Sub

    Private Sub TampilPelanggan(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Label5.Text = "Pelanggan"

        CmbRekening.Items.Clear()
        CmbRekening.Items.Add("SEMUA")

        Dim query As String = "SELECT DISTINCT NAMA_PELANGGAN FROM PENJUALAN WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan ORDER BY NAMA_PELANGGAN"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        CmbRekening.Items.Add(reader("NAMA_PELANGGAN").ToString())
                    End While
                End If
            End Using
        End Using
        CmbRekening.SelectedIndex = 0
    End Sub

    Private Sub DTPTanggal_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DTPAwal.ValueChanged
        TampilkanLaporan(LblHeaderForm.Text)
    End Sub

    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim namaAkunD As String = CmbRekening.Text

        Dim sql As String = "SELECT Type_Akun, Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtRekening.Text = reader("Kode_akun").ToString()
                Else
                    TxtRekening.Clear()
                End If
            End Using
        End Using
    End Sub

    Private Sub CbTanggal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
            TampilkanLaporan(LblHeaderForm.Text)
        End If
    End Sub

    Private Sub CbBulan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
            If Not String.IsNullOrEmpty(CmbBln.Text) Then
                TampilkanLaporan(LblHeaderForm.Text)
            End If
        End If
    End Sub


    Private Sub CmbBln_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBln.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub CmbThn_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbThn.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub BtnTampilkan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTampilkan.Click
        Cursor = Cursors.WaitCursor
        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim rekeningatauPelanggan As String = If(CmbRekening.Text = "SEMUA" Or CmbRekening.SelectedIndex = 0, "", CmbRekening.Text)

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked = True Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked = True Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If

        Select Case LblHeaderForm.Text
            Case "LAPORAN REKAP PENJUALAN NOTA"
                REKAPPENJUALANNOTA(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN REKAP PENJUALAN BARANG"
                REKAPPENJUALANBARANG(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN PENJUALAN"
                PENJUALAN(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN PENJUALAN DETAIL"
                PENJUALANDetail(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN BARANG PENJUALAN"
                PENJUALANBarang(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
            Case "LAPORAN PENJUALAN DIHUTANG"
                PENJUALANBelumLunas(kasir, rekeningatauPelanggan, tanggalAwal, tanggalAkhir)
        End Select
        Cursor = Cursors.Default
    End Sub

    Private Sub REKAPPENJUALANNOTA(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim pendapatanPenjualan As Decimal = 0
        Dim Labapenjualan As Decimal = 0

        Dim queryHitung As String = "SELECT Sum(TOTAL_HPP) as HargaBeli, " &
                          "Sum(GRAND_TOTAL_SBL_PAJAK) as HargaJual, " &
                          "Sum(DISKON_TOTAL_RP) as Diskon, " &
                          "Sum(GRAND_TOTAL_STL_PAJAK) as Pendapatan, " &
                          "Sum(BAYAR - KEMBALI) as Laba " &
                          "FROM penjualan " &
                          "WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND JENIS_PEMBAYARAN LIKE @JENIS_PEMBAYARAN AND ID_USER LIKE @ID_USER"

        Using cmdHitung As New MySqlCommand(queryHitung, conn)
            cmdHitung.Parameters.AddWithValue("@tanggalAwal", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitung.Parameters.AddWithValue("@tanggalAkhir", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitung.Parameters.AddWithValue("@JENIS_PEMBAYARAN", String.Format("%{0}%", rekeningatauPelanggan))
            cmdHitung.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rdHitung As MySqlDataReader = cmdHitung.ExecuteReader()
                rdHitung.Read()
                If rdHitung.HasRows Then
                    ' Menghitung Pendapatan dari hasil penjualan
                    pendapatanPenjualan = If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Pendapatan")), rdHitung("Pendapatan"), 0)
                    Labapenjualan = If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Pendapatan")), rdHitung("Pendapatan"), 0) - If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("HargaBeli")), rdHitung("HargaBeli"), 0)

                    ' Menghitung prosentase laba
                    Dim prosentaseLaba As Decimal = 0

                    If pendapatanPenjualan <> 0 Then
                        prosentaseLaba = Math.Round((Labapenjualan / pendapatanPenjualan) * 100, 2)
                    End If


                    ' Menyampaikan nilai sebagai parameter ke laporan RDLC
                    Dim parameters As New ReportParameterCollection From {
    New ReportParameter("HargaBeli", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("HargaBeli")), rdHitung("HargaBeli").ToString(), "0")),
    New ReportParameter("HargaJual", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("HargaJual")), rdHitung("HargaJual").ToString(), "0")),
    New ReportParameter("Diskon", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Diskon")), rdHitung("Diskon").ToString(), "0")),
    New ReportParameter("Pendapatan", pendapatanPenjualan.ToString()),
    New ReportParameter("Persenpenjualan", prosentaseLaba.ToString() & " %"),
    New ReportParameter("Laba", Labapenjualan.ToString()),
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy")),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
    New ReportParameter("User", "kasir : " & CmbKasir.Text)
}



                    ' Mengatur parameter untuk laporan RDLC
                    ReportViewer1.LocalReport.SetParameters(parameters)
                    ReportViewer1.RefreshReport()
                End If
            End Using
        End Using

        Dim queryRetur As String = "SELECT SUM(retur_penjualan_detail.HARGA_BELI_SATUAN) AS HargaBeli, " &
                                   "SUM(retur_penjualan_detail.HARGA_JUAL) AS HargaJual, " &
                                   "SUM(retur_penjualan_detail.TOTAL_DISKON) AS Diskon, " &
                                   "SUM(retur_penjualan_detail.TOTAL_HARGA) AS Pendapatan, " &
                                   "SUM(retur_penjualan_detail.LABA) AS Laba " &
                                   "FROM retur_penjualan_detail " &
                                   "INNER JOIN retur_penjualan ON retur_penjualan_detail.ID_RETUR_PENJUALAN = retur_penjualan.ID_RETUR_PENJUALAN " &
                                   "WHERE retur_penjualan.TGL_RETUR_JUAL >= @tanggalAwal " &
                                   "AND retur_penjualan.TGL_RETUR_JUAL <= @tanggalAkhir " &
                                   "AND retur_penjualan.NAMA_REKENING LIKE @NAMA_REKENING " &
                                   "AND retur_penjualan.ID_USER LIKE @ID_USER"


        Using cmdRetur As New MySqlCommand(queryRetur, conn)
            cmdRetur.Parameters.AddWithValue("@tanggalAwal", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdRetur.Parameters.AddWithValue("@tanggalAkhir", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdRetur.Parameters.AddWithValue("@NAMA_REKENING", String.Format("%{0}%", rekeningatauPelanggan))
            cmdRetur.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rdRetur As MySqlDataReader = cmdRetur.ExecuteReader()
                rdRetur.Read()
                If rdRetur.HasRows Then
                    ' Menghitung Pendapatan dari retur penjualan
                    Dim pendapatanRetur As Decimal = If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Pendapatan")), rdRetur("Pendapatan"), 0)
                    Dim LabaRetur As Decimal = If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Laba")), rdRetur("Laba"), 0)

                    ' Menghitung prosentase laba
                    Dim prosentaseLaba As Decimal = 0

                    If pendapatanRetur <> 0 Then
                        prosentaseLaba = Math.Round((LabaRetur / pendapatanRetur) * 100, 2)
                    End If


                    ' Menghitung selisih antara Pendapatan hasil penjualan dan retur penjualan
                    Dim selisihPendapatan As Decimal = pendapatanPenjualan - pendapatanRetur
                    Dim selisihLaba As Decimal = Labapenjualan - LabaRetur

                    ' Menghitung prosentase laba
                    Dim prosentase As Decimal = 0

                    If selisihPendapatan <> 0 Then
                        prosentase = Math.Round((selisihLaba / selisihPendapatan) * 100, 2)
                    End If

                    ' Menyampaikan nilai sebagai parameter ke laporan RDLC
                    Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("HargaBeliRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("HargaBeli")), rdRetur("HargaBeli").ToString(), "0")),
    New ReportParameter("HargaJualRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("HargaJual")), rdRetur("HargaJual").ToString(), "0")),
    New ReportParameter("DiskonRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Diskon")), rdRetur("Diskon").ToString(), "0")),
    New ReportParameter("PendapatanRetur", pendapatanRetur.ToString()),
    New ReportParameter("LabaRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Laba")), rdRetur("Laba").ToString(), "0")),
    New ReportParameter("SelisihPendapatan", selisihPendapatan.ToString()),
    New ReportParameter("SelisihLaba", selisihLaba.ToString()),
    New ReportParameter("LabaRetur1", prosentaseLaba.ToString() & " %"),
    New ReportParameter("LabaBersih", prosentase.ToString() & " %")
}

                    ' Mengatur parameter untuk laporan RDLC
                    ReportViewer1.LocalReport.SetParameters(parametersRetur)
                    ReportViewer1.RefreshReport()
                End If
            End Using
        End Using
    End Sub

    Private Sub REKAPPENJUALANBARANG(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim pendapatanPenjualan As Decimal = 0
        Dim Labapenjualan As Decimal = 0

        Dim queryHitung As String = "SELECT Sum(HARGA_BELI_SATUAN) as HargaBeli, " &
                          "Sum(HARGA_JUAL) as HargaJual, " &
                          "Sum(TOTAL_DISKON) as Diskon, " &
                          "Sum(TOTAL_HARGA) as Pendapatan, " &
                          "Sum(LABA) as Laba " &
                          "FROM penjualan_detail " &
                          "WHERE TANGGAL_JUAL >= @tanggalAwal AND TANGGAL_JUAL <= @tanggalAkhir AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND ID_USER LIKE @ID_USER"

        Using cmdHitung As New MySqlCommand(queryHitung, conn)
            cmdHitung.Parameters.AddWithValue("@tanggalAwal", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitung.Parameters.AddWithValue("@tanggalAkhir", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitung.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", rekeningatauPelanggan))
            cmdHitung.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rdHitung As MySqlDataReader = cmdHitung.ExecuteReader()
                rdHitung.Read()
                If rdHitung.HasRows Then
                    ' Menghitung Pendapatan dari hasil penjualan
                    pendapatanPenjualan = If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Pendapatan")), rdHitung("Pendapatan"), 0)
                    Labapenjualan = If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Laba")), rdHitung("Laba"), 0)

                    ' Menghitung prosentase laba
                    Dim prosentaseLaba As Decimal = 0

                    If pendapatanPenjualan <> 0 Then
                        prosentaseLaba = Math.Round((Labapenjualan / pendapatanPenjualan) * 100, 2)
                    End If


                    ' Menyampaikan nilai sebagai parameter ke laporan RDLC
                    Dim parameters As New ReportParameterCollection From {
    New ReportParameter("HargaBeli", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("HargaBeli")), rdHitung("HargaBeli").ToString(), "0")),
    New ReportParameter("HargaJual", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("HargaJual")), rdHitung("HargaJual").ToString(), "0")),
    New ReportParameter("Diskon", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Diskon")), rdHitung("Diskon").ToString(), "0")),
    New ReportParameter("Pendapatan", pendapatanPenjualan.ToString()),
    New ReportParameter("Persenpenjualan", prosentaseLaba.ToString() & " %"),
    New ReportParameter("Laba", If(Not rdHitung.IsDBNull(rdHitung.GetOrdinal("Laba")), rdHitung("Laba").ToString(), "0")),
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy")),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
    New ReportParameter("User", "kasir : " & CmbKasir.Text)
}



                    ' Mengatur parameter untuk laporan RDLC
                    ReportViewer1.LocalReport.SetParameters(parameters)
                    ReportViewer1.RefreshReport()
                End If
            End Using
        End Using

        Dim queryRetur As String = "SELECT Sum(HARGA_BELI_SATUAN) as HargaBeli, " &
                                    "Sum(HARGA_JUAL) as HargaJual, " &
                                    "Sum(TOTAL_DISKON) as Diskon, " &
                                    "Sum(TOTAL_HARGA) as Pendapatan, " &
                                    "Sum(LABA) as Laba " &
                                    "FROM retur_penjualan_detail " &
                                    "WHERE TGL_RETUR_JUAL >= @tanggalAwal AND TGL_RETUR_JUAL <= @tanggalAkhir AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND ID_USER LIKE @ID_USER"

        Using cmdRetur As New MySqlCommand(queryRetur, conn)
            cmdRetur.Parameters.AddWithValue("@tanggalAwal", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdRetur.Parameters.AddWithValue("@tanggalAkhir", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdRetur.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", rekeningatauPelanggan))
            cmdRetur.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rdRetur As MySqlDataReader = cmdRetur.ExecuteReader()
                rdRetur.Read()
                If rdRetur.HasRows Then
                    ' Menghitung Pendapatan dari retur penjualan
                    Dim pendapatanRetur As Decimal = If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Pendapatan")), rdRetur("Pendapatan"), 0)
                    Dim LabaRetur As Decimal = If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Laba")), rdRetur("Laba"), 0)

                    ' Menghitung prosentase laba
                    Dim prosentaseLaba As Decimal = 0

                    If pendapatanRetur <> 0 Then
                        prosentaseLaba = Math.Round((LabaRetur / pendapatanRetur) * 100, 2)
                    End If


                    ' Menghitung selisih antara Pendapatan hasil penjualan dan retur penjualan
                    Dim selisihPendapatan As Decimal = pendapatanPenjualan - pendapatanRetur
                    Dim selisihLaba As Decimal = Labapenjualan - LabaRetur

                    ' Menghitung prosentase laba
                    Dim prosentase As Decimal = 0

                    If selisihPendapatan <> 0 Then
                        prosentase = Math.Round((selisihLaba / selisihPendapatan) * 100, 2)
                    End If

                    ' Menyampaikan nilai sebagai parameter ke laporan RDLC
                    Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("HargaBeliRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("HargaBeli")), rdRetur("HargaBeli").ToString(), "0")),
    New ReportParameter("HargaJualRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("HargaJual")), rdRetur("HargaJual").ToString(), "0")),
    New ReportParameter("DiskonRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Diskon")), rdRetur("Diskon").ToString(), "0")),
    New ReportParameter("PendapatanRetur", pendapatanRetur.ToString()),
    New ReportParameter("LabaRetur", If(Not rdRetur.IsDBNull(rdRetur.GetOrdinal("Laba")), rdRetur("Laba").ToString(), "0")),
    New ReportParameter("SelisihPendapatan", selisihPendapatan.ToString()),
    New ReportParameter("SelisihLaba", selisihLaba.ToString()),
    New ReportParameter("LabaRetur1", prosentaseLaba.ToString() & " %"),
    New ReportParameter("LabaBersih", prosentase.ToString() & " %")
}

                    ' Mengatur parameter untuk laporan RDLC
                    ReportViewer1.LocalReport.SetParameters(parametersRetur)
                    ReportViewer1.RefreshReport()
                End If
            End Using
        End Using
    End Sub

    Private Sub PENJUALAN(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim queryReturJual As String = "SELECT " &
   "ID_PENJUALAN, NAMA_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, DISKON_TOTAL_RP, PAJAK_RP, GRAND_TOTAL_STL_PAJAK, STATUS_TRANSAKSI, JENIS_PEMBAYARAN, ID_USER " &
   "FROM PENJUALAN " &
   "WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND JENIS_PEMBAYARAN LIKE @JENIS_PEMBAYARAN AND ID_USER LIKE @ID_USER " &
   "ORDER BY ID_PENJUALAN"

        Using cmdDataRetur As New MySqlCommand(queryReturJual, conn)
            cmdDataRetur.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@JENIS_PEMBAYARAN", String.Format("%{0}%", rekeningatauPelanggan))
            cmdDataRetur.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rdDataRetur As MySqlDataReader = cmdDataRetur.ExecuteReader()
                Dim datasetRetur As New DataSetKL()
                datasetRetur.Load(rdDataRetur, LoadOption.OverwriteChanges, "penjualan")
                Dim dtPenjualan As DataTable = ConvertColumnToDateTime(datasetRetur.Tables("penjualan"), "TGL_TRANSAKSI")

                ' Menambahkan parameter ke laporan RDLC
                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Rekening : " & CmbRekening.Text

                Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy") & keterangan),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
}
                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewer2.LocalReport.DataSources.Clear()
                ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtPenjualan))
                ReportViewer2.LocalReport.SetParameters(parametersRetur)

                ' Menampilkan laporan RDLC
                ReportViewer2.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub PENJUALANDetail(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim query As String = "SELECT FAKTUR_JUAL, NAMA_PELANGGAN, TANGGAL_JUAL, NAMA_BARANG, QTY, SATUAN, HARGA_BELI_SATUAN, TOTAL_DISKON, TOTAL_HARGA, LABA, ID_USER " &
                              "FROM PENJUALAN_detail " &
                              "WHERE TANGGAL_JUAL >= @AwalBulan AND TANGGAL_JUAL <= @AkhirBulan AND ID_USER LIKE @ID_USER AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN " &
                              "ORDER BY FAKTUR_JUAL"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))
            command.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", rekeningatauPelanggan))

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dataset As New DataSetKL()
                dataset.Load(reader, LoadOption.OverwriteChanges, "penjualan_detail")
                Dim dtDetail As DataTable = ConvertColumnToDateTime(dataset.Tables("penjualan_detail"), "TANGGAL_JUAL")

                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Pelanggan : " & CmbRekening.Text

                Dim parameters As New ReportParameterCollection From {
                    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
                    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                }

                ReportViewer3.LocalReport.DataSources.Clear()
                ReportViewer3.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtDetail))
                ReportViewer3.LocalReport.SetParameters(parameters)
                ReportViewer3.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub PENJUALANBarang(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim query As String = "SELECT ID_BARANG, " &
                            "NAMA_BARANG, " &
                            "MAX(SATUAN) AS SATUAN, " &
                            "SUM(QTY) as QTY, " &
                            "SUM(HARGA_BELI_SATUAN) AS HARGA_BELI_SATUAN, " &
                            "SUM(TOTAL_DISKON) AS TOTAL_DISKON, " &
                            "SUM(TOTAL_HARGA) AS TOTAL_HARGA, " &
                            "SUM(LABA) AS LABA " &
                       "FROM PENJUALAN_detail " &
                       "WHERE TANGGAL_JUAL >= @AwalBulan AND TANGGAL_JUAL <= @AkhirBulan AND ID_USER LIKE @ID_USER AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN " &
                       "GROUP BY ID_BARANG, NAMA_BARANG " &
                       "ORDER BY NAMA_BARANG"


        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))
            command.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", rekeningatauPelanggan))

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dataset As New DataSetKL()
                dataset.Load(reader, LoadOption.OverwriteChanges, "penjualan_barang")

                Dim keterangan As String = "                    kasir : " & CmbKasir.Text & "                    Pelanggan : " & CmbRekening.Text

                Dim parameters As New ReportParameterCollection From {
                    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
                    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                }

                ReportViewer4.LocalReport.DataSources.Clear()
                ReportViewer4.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("penjualan_barang")))
                ReportViewer4.LocalReport.SetParameters(parameters)
                ReportViewer4.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub PENJUALANBelumLunas(ByVal kasir As String, ByVal rekeningatauPelanggan As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim queryReturJual As String = "SELECT " &
   "ID_PENJUALAN, TGL_TRANSAKSI, NAMA_PELANGGAN, LOKASIBARANG, GRAND_TOTAL_STL_PAJAK, (BAYAR - KEMBALI) AS PEMBAYARAN, SISA_TAGIHAN, JATUH_TEMPO, ID_USER " &
   "FROM PENJUALAN " &
   "WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND JENIS_PEMBAYARAN LIKE @JENIS_PEMBAYARAN AND ID_USER LIKE @ID_USER AND STATUS_TRANSAKSI = 'Belum Lunas' " &
   "ORDER BY ID_PENJUALAN"

        Using cmdDataRetur As New MySqlCommand(queryReturJual, conn)
            cmdDataRetur.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@JENIS_PEMBAYARAN", String.Format("%{0}%", rekeningatauPelanggan))
            cmdDataRetur.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rdDataRetur As MySqlDataReader = cmdDataRetur.ExecuteReader()
                Dim datasetRetur As New DataSetKL()
                datasetRetur.Load(rdDataRetur, LoadOption.OverwriteChanges, "PenjualanHutang")
                Dim dtHutangJual As DataTable = ConvertColumnToDateTime(datasetRetur.Tables("PenjualanHutang"), "TGL_TRANSAKSI")
                dtHutangJual = ConvertColumnToDateTime(dtHutangJual, "JATUH_TEMPO")

                ' Menambahkan parameter ke laporan RDLC
                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Rekening : " & CmbRekening.Text

                Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy") & keterangan),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
}
                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewer5.LocalReport.DataSources.Clear()
                ReportViewer5.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtHutangJual))
                ReportViewer5.LocalReport.SetParameters(parametersRetur)

                ' Menampilkan laporan RDLC
                ReportViewer5.RefreshReport()
            End Using
        End Using
    End Sub








    Private Sub FormLapPenjualanBaru_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
        Case Keys.F5 : BtnTampilkan.PerformClick()
    End Select
    End Sub

End Class
