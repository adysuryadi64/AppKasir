Imports Microsoft.Reporting.WinForms

Public Class FormLapPenjualanSales

    Private Sub FormLapPenjualanSales_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Cursor = Cursors.WaitCursor
        CbTanggal.Checked = True
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        Cursor = Cursors.Default
        Me.ReportViewer1.RefreshReport()
    End Sub

    'Private Sub Ambildataperiodekerja()
    '    Dim tanggal As Integer
    '    If Integer.TryParse(TANGGAL_TUTUP_BULAN.ToString(), tanggal) Then
    '        If tanggal >= 1 AndAlso tanggal <= 28 Then
    '            ' Dapatkan bulan dan tahun sekarang dari combo box
    '            Dim bulanSekarang As Integer = DateTime.Now.Month ' Mendapatkan bulan sekarang
    '            Dim tahunSekarang As Integer = DateTime.Now.Year ' Mendapatkan tahun sekarang

    '            If JENIS_TUTUP_BULAN = "Berdasar bulan kalender" Then
    '                ' Atur DtpAwal dengan tanggal 1 pada bulan sebelumnya
    '                Dim bulanAwal As Integer = If(bulanSekarang = 1, 12, bulanSekarang)
    '                Dim tahunAwal As Integer = If(bulanSekarang = 1, tahunSekarang - 1, tahunSekarang)
    '                Dim tanggalAwal As New Date(tahunAwal, bulanAwal, 1)
    '                DtpAwal.Value = tanggalAwal

    '                ' Atur DtpAkhir dengan tanggal akhir bulan sesuai kalender bulan sebelumnya
    '                Dim tanggalAkhir As New Date(tahunAwal, bulanAwal, Date.DaysInMonth(tahunAwal, bulanAwal))
    '                DtpAkhir.Value = tanggalAkhir
    '            Else
    '                ' Atur DtpAkhir dengan tanggal yang sama dengan TxtTanggal.text, bulan berikutnya
    '                Dim bulanAkhir As Integer = If(bulanSekarang = 12, 1, bulanSekarang)
    '                Dim tahunAkhir As Integer = If(bulanSekarang = 12, tahunSekarang + 1, tahunSekarang)
    '                Dim tanggalAkhir As New Date(tahunAkhir, bulanAkhir, tanggal)
    '                DtpAkhir.Value = tanggalAkhir

    '                ' Atur DtpAwal dengan tanggal 1 hari setelah TxtTanggal.text, bulan sebelumnya
    '                Dim tanggalAwal As Date = New Date(tahunSekarang, bulanSekarang, tanggal).AddDays(1)
    '                Dim bulanAwal As Integer = If(tanggalAwal.Month = 12, 1, tanggalAwal.Month - 1)
    '                Dim tahunAwal As Integer = If(tanggalAwal.Month = 12, tahunSekarang + 1, tahunSekarang)
    '                DtpAwal.Value = New Date(tahunAwal, bulanAwal, tanggal + 1)
    '            End If
    '        End If
    '    End If
    'End Sub



    Private Sub DTPAwal_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DTPAwal.ValueChanged
        AmbilDataSales()
    End Sub

    Private Sub DTPAkhir_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DTPAkhir.ValueChanged
        AmbilDataSales()
    End Sub

    Private Sub AmbilDataSales()
        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If

        TampilSales(tanggalAwal, tanggalAkhir)


    End Sub


    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            Dim angkaBulan As String = (CmbBln.SelectedIndex + 1).ToString("D2")
            Dim teksBulanTahunTerpilih As String = angkaBulan & "/" & CmbThn.Text
            AmbilDataSales()
        End If
    End Sub

    Public Sub TampilSales(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        CmbSales.Items.Clear()
        CmbSales.Items.Add("Semua")

        Dim query As String = "SELECT DISTINCT NAMA_SALES FROM PENJUALAN WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND NAMA_SALES IS NOT NULL AND NAMA_SALES <> '' ORDER BY NAMA_SALES"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AkhirBulan", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbSales.Items.Add(rd("NAMA_SALES").ToString())
                End While
            End Using
        End Using

        CmbSales.SelectedIndex = 0
    End Sub



    Private Sub CmbSales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSales.SelectedIndexChanged
        Dim namaAkunD As String = CmbSales.Text

        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtSales.Text = reader("Kode").ToString()
                Else
                    TxtSales.Clear()
                End If
            End Using
        End Using
    End Sub

    Private Sub CbTanggal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
            AmbilDataSales()
        End If
    End Sub

    Private Sub CbBulan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
            If Not String.IsNullOrEmpty(CmbBln.Text) Then
                AmbilDataSales()
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
        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date

        If CbTanggal.Checked Then
            tanggalAwal = DTPAwal.Value.Date
            tanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CbBulan.Checked Then
            If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
        End If

        Dim queryReturJual As String = "SELECT " &
 "ID_PENJUALAN, NAMA_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, GRAND_TOTAL_STL_PAJAK, ID_USER " &
 "FROM PENJUALAN " &
 "WHERE TGL_TRANSAKSI >= @AwalBulan AND TGL_TRANSAKSI <= @AkhirBulan AND ID_SALES LIKE @ID_SALES " &
 "ORDER BY ID_PENJUALAN"

        Using cmdDataRetur As New MySqlCommand(queryReturJual, conn)
            cmdDataRetur.Parameters.AddWithValue("@AwalBulan", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@AkhirBulan", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdDataRetur.Parameters.AddWithValue("@ID_SALES", String.Format("%{0}%", TxtSales.Text))


            Using rdDataRetur As MySqlDataReader = cmdDataRetur.ExecuteReader()
                Dim datasetRetur As New DataSetKL()
                datasetRetur.Load(rdDataRetur, LoadOption.OverwriteChanges, "PenjualanSales")

                ' Menambahkan parameter ke laporan RDLC
                Dim keterangan As String = "          Sales : " & CmbSales.Text

                Dim parametersRetur As New ReportParameterCollection From {
    New ReportParameter("Periode", "Periode : " & tanggalAwal.ToString("dd/MM/yyyy") & " s/d " & tanggalAkhir.ToString("dd/MM/yyyy") & keterangan),
    New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text),
    New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
}
                ' Menetapkan dataset dan parameter ke laporan RDLC
                ReportViewer1.LocalReport.DataSources.Clear()
                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetRetur.Tables("PenjualanSales")))
                ReportViewer1.LocalReport.SetParameters(parametersRetur)

                ' Menampilkan laporan RDLC
                ReportViewer1.RefreshReport()
            End Using
        End Using


    End Sub


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
    Private Sub FormLapPenjualanSales_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
        Case Keys.F5 : BtnTampilkan.PerformClick()
    End Select
    End Sub

End Class
