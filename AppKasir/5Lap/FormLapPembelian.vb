Imports Microsoft.Reporting.WinForms


Public Class FormLapPembelian

    Private Sub FormLapPembelian_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Cursor = Cursors.WaitCursor
        CbTanggal.Checked = True
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        Select Case LblHeaderForm.Text
            Case "LAPORAN PEMBELIAN"
                Panel2.Visible = True
                Panel3.Visible = False
                Panel4.Visible = False
                Panel5.Visible = False
            Case "LAPORAN PEMBELIAN DETAIL"
                Panel2.Visible = False
                Panel3.Visible = True
                Panel4.Visible = False
                Panel5.Visible = False
            Case "LAPORAN BARANG PEMBELIAN"
                Panel2.Visible = False
                Panel3.Visible = False
                Panel4.Visible = True
                Panel5.Visible = False
            Case "LAPORAN PEMBELIAN DIHUTANG"
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
            Case "LAPORAN PEMBELIAN"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilRekening(tanggalAwal, tanggalAkhir)
            Case "LAPORAN PEMBELIAN DETAIL"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilSupplier(tanggalAwal, tanggalAkhir)
            Case "LAPORAN BARANG PEMBELIAN"
                TampilKasir(tanggalAwal, tanggalAkhir)
                TampilSupplier(tanggalAwal, tanggalAkhir)
            Case "LAPORAN PEMBELIAN DIHUTANG"
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

        Dim query As String = "SELECT DISTINCT ID_USER FROM pembelian WHERE TGL_BELI >= @AwalBulan AND TGL_BELI <= @AkhirBulan ORDER BY ID_USER"

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
        Dim query As String = "SELECT DISTINCT JENIS_BAYAR FROM pembelian WHERE TGL_BELI >= @AwalBulan AND TGL_BELI <= @AkhirBulan ORDER BY JENIS_BAYAR"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        CmbRekening.Items.Add(reader("JENIS_BAYAR").ToString())
                    End While
                End If
            End Using
        End Using
        CmbRekening.SelectedIndex = 0
    End Sub

    Private Sub TampilSupplier(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Label5.Text = "Supplier"

        CmbRekening.Items.Clear()
        CmbRekening.Items.Add("SEMUA")

        Dim query As String = "SELECT DISTINCT NAMA_SUPLIYER FROM pembelian WHERE TGL_BELI >= @AwalBulan AND TGL_BELI <= @AkhirBulan ORDER BY NAMA_SUPLIYER"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        CmbRekening.Items.Add(reader("NAMA_SUPLIYER").ToString())
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
        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim rekeningatauSupplier As String = If(CmbRekening.Text = "SEMUA" Or CmbRekening.SelectedIndex = 0, "", CmbRekening.Text)

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked = True Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked = True Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If

        Select Case LblHeaderForm.Text
            Case "LAPORAN PEMBELIAN"
                Pembelian(kasir, rekeningatauSupplier, tanggalAwal, tanggalAkhir)
            Case "LAPORAN PEMBELIAN DETAIL"
                PembelianDetail(kasir, rekeningatauSupplier, tanggalAwal, tanggalAkhir)
            Case "LAPORAN BARANG PEMBELIAN"
                PembelianBarang(kasir, rekeningatauSupplier, tanggalAwal, tanggalAkhir)
            Case "LAPORAN PEMBELIAN DIHUTANG"
                PembelianBelumLunas(kasir, rekeningatauSupplier, tanggalAwal, tanggalAkhir)
        End Select
    End Sub


    Private Sub Pembelian(ByVal kasir As String, ByVal rekeningatauSupplier As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim queryReturJual As String = "SELECT " &
   "ID_PEMBELIAN, TGL_BELI, NAMA_SUPLIYER, LOKASI, TOTAL_BARANG, TOTAL_QTY, GRAND_TOTAL_BELI, PEMBAYARAN, STATUS_TRANSAKSI_BELI, ID_USER " &
   "FROM pembelian " &
   "WHERE TGL_BELI >= @AwalBulan AND TGL_BELI <= @AkhirBulan AND JENIS_BAYAR LIKE @JENIS_BAYAR AND ID_USER LIKE @IdUser " &
   "ORDER BY ID_PEMBELIAN"

        Using cmdDataRetur As New MySqlCommand(queryReturJual, conn)
            cmdDataRetur.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@JENIS_BAYAR", String.Format("%{0}%", rekeningatauSupplier))
            cmdDataRetur.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))

            Using rdDataRetur As MySqlDataReader = cmdDataRetur.ExecuteReader()
                Dim dt As New DataTable("pembelian")
                dt.Load(rdDataRetur)
                dt = ConvertColumnToDateTime(dt, "TGL_BELI")

                ' Menambahkan parameter ke laporan RDLC
                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Rekening : " & CmbRekening.Text

                Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy") & keterangan),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
}
                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewer1.LocalReport.DataSources.Clear()
                ReportViewer1.LocalReport.DataSources.Add(
        New ReportDataSource("DataSet1", dt)
    )
                ReportViewer1.LocalReport.SetParameters(parametersRetur)

                ' Menampilkan laporan RDLC
                ReportViewer1.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub PembelianDetail(ByVal kasir As String, ByVal rekeningatauSupplier As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim query As String = "SELECT FAKTUR_BELI, TANGGAL_MASUK, NAMA_SUPLIYER, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, " &
                              "HARGA_BELI_SATUAN, TOTAL, ID_USER " &
                              "FROM pembelian_detail " &
                              "WHERE TANGGAL_MASUK >= @AwalBulan AND TANGGAL_MASUK <= @AkhirBulan AND ID_USER LIKE @IdUser AND NAMA_SUPLIYER LIKE @NAMA_SUPLIYER " &
                              "ORDER BY FAKTUR_BELI"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))
            command.Parameters.AddWithValue("@NAMA_SUPLIYER", String.Format("%{0}%", rekeningatauSupplier))

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dt As New DataTable("pembelian_detail1")
                dt.Load(reader)
                dt = ConvertColumnToDateTime(dt, "TANGGAL_MASUK")

                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Supplier : " & CmbRekening.Text

                Dim parameters As New ReportParameterCollection From {
                    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
                    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                }

                ReportViewer2.LocalReport.DataSources.Clear()
                ReportViewer2.LocalReport.DataSources.Add(
        New ReportDataSource("DataSet1", dt)
    )
                ReportViewer2.LocalReport.SetParameters(parameters)
                ReportViewer2.RefreshReport()
            End Using
        End Using
    End Sub


    Private Sub PembelianBarang(ByVal kasir As String, ByVal rekeningatauSupplier As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, MAX(SATUAN) AS SATUAN, SUM(QTY) as QTY, " &
                       "SUM(HARGA_BELI_SATUAN) AS HARGA_BELI_SATUAN, SUM(TOTAL) AS TOTAL " &
                       "FROM pembelian_detail " &
                       "WHERE TANGGAL_MASUK >= @AwalBulan AND TANGGAL_MASUK <= @AkhirBulan AND ID_USER LIKE @IdUser AND NAMA_SUPLIYER LIKE @NAMA_SUPLIYER " &
                       "GROUP BY ID_BARANG, NAMA_BARANG " &
                       "ORDER BY NAMA_BARANG"


        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            command.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))
            command.Parameters.AddWithValue("@NAMA_SUPLIYER", String.Format("%{0}%", rekeningatauSupplier))

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dt As New DataTable("pembelian_barang")
                dt.Load(reader)   ' ✅ BENAR

                Dim keterangan As String = "                    kasir : " & CmbKasir.Text & "                    Supplier : " & CmbRekening.Text

                Dim parameters As New ReportParameterCollection From {
                    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
                    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
                    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                }

                ReportViewer3.LocalReport.DataSources.Clear()
                ReportViewer3.LocalReport.DataSources.Add(
                New ReportDataSource("DataSet1", dt)   ' ✅ BENAR
            )
                ReportViewer3.LocalReport.SetParameters(parameters)
                ReportViewer3.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub PembelianBelumLunas(ByVal kasir As String, ByVal rekeningatauSupplier As String, ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        Dim queryReturJual As String = "SELECT " &
   "ID_PEMBELIAN, TGL_BELI, NAMA_SUPLIYER, LOKASI, GRAND_TOTAL_BELI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, ID_USER " &
   "FROM pembelian " &
   "WHERE TGL_BELI >= @AwalBulan AND TGL_BELI <= @AkhirBulan AND JENIS_BAYAR LIKE @JENIS_BAYAR AND ID_USER LIKE @IdUser AND STATUS_TRANSAKSI_BELI = 'Belum Lunas' " &
   "ORDER BY ID_PEMBELIAN"

        Using cmdDataRetur As New MySqlCommand(queryReturJual, conn)
            cmdDataRetur.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@JENIS_BAYAR", String.Format("%{0}%", rekeningatauSupplier))
            cmdDataRetur.Parameters.AddWithValue("@IdUser", String.Format("%{0}%", kasir))

            Using rdDataRetur As MySqlDataReader = cmdDataRetur.ExecuteReader()
                Dim dt As New DataTable("pembelianHutang")
                dt.Load(rdDataRetur)
                dt = ConvertColumnToDateTime(dt, "TGL_BELI")
                dt = ConvertColumnToDateTime(dt, "JATUH_TEMPO")

                ' Menambahkan parameter ke laporan RDLC
                Dim keterangan As String = "          kasir : " & CmbKasir.Text & "          Rekening : " & CmbRekening.Text

                Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("Periode", "Periode : " & tanggalawal.ToString("dd/MM/yyyy") & " s/d " & tanggalakhir.ToString("dd/MM/yyyy") & keterangan),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
}
                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewer4.LocalReport.DataSources.Clear()
                ReportViewer4.LocalReport.DataSources.Add(
        New ReportDataSource("DataSet1", dt)
    )
                ReportViewer4.LocalReport.SetParameters(parametersRetur)

                ' Menampilkan laporan RDLC
                ReportViewer4.RefreshReport()
            End Using
        End Using
    End Sub
    Private Sub FormLapPembelian_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
        Case Keys.F5 : BtnTampilkan.PerformClick()
    End Select
    End Sub

End Class
