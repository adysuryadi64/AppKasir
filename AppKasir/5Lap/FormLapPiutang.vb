Imports Microsoft.Reporting.WinForms

Public Class FormLapPiutang

    Private Sub FormLapHutang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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

    Private bulanTerpilih As Integer


    Private Sub KonversiBulanKeAngka()
        Select Case CmbBln.Text
            Case "Januari" : bulanTerpilih = 1
            Case "Februari" : bulanTerpilih = 2
            Case "Maret" : bulanTerpilih = 3
            Case "April" : bulanTerpilih = 4
            Case "Mei" : bulanTerpilih = 5
            Case "Juni" : bulanTerpilih = 6
            Case "Juli" : bulanTerpilih = 7
            Case "Agustus" : bulanTerpilih = 8
            Case "September" : bulanTerpilih = 9
            Case "Oktober" : bulanTerpilih = 10
            Case "November" : bulanTerpilih = 11
            Case "Desember" : bulanTerpilih = 12
        End Select
    End Sub

    Private Sub MuatComboBoxBulanTahun()
        ' Bersihkan item sebelum menambahkannya kembali
        CmbThn.Items.Clear()

        ' Tambahkan tahun dari 2022 hingga tahun sekarang
        For i As Integer = 2022 To Year(Now)
            CmbThn.Items.Add(i)
        Next

        ' Bersihkan item sebelum menambahkannya kembali
        CmbBln.Items.Clear()

        ' Tambahkan daftar bulan
        Dim daftarBulan As String() = {"Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember"}
        CmbBln.Items.AddRange(daftarBulan)

        ' Set tahun sekarang sebagai tahun default
        CmbThn.SelectedItem = Year(Now)

        ' Set bulan sekarang sebagai bulan default
        CmbBln.SelectedIndex = Month(Now) - 1
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
            MuatComboBoxBulanTahun()
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
            ' Cek apakah ComboBox belum dipilih
            If CmbBln.SelectedIndex = -1 Then
                ' Tampilkan pesan peringatan
                MessageBox.Show("Harap pilih bulan terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CmbBln.DroppedDown = True
                Exit Sub
            End If

            ' Tetapkan AwalBulan dan AkhirBulan berdasarkan bulan dan tahun yang dipilih
            KonversiBulanKeAngka() ' (Jika diperlukan)
            Dim bulan As Integer = bulanTerpilih
            Dim tahun As Integer = CmbThn.Text
            AwalBulan = New DateTime(tahun, bulan, 1).ToString("yyyy-MM-dd HH:mm:ss")
            AkhirBulan = AwalBulan.AddMonths(1).AddDays(-1).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss")

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
        If LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY PENJUALAN" Then
            queryhitung = "SELECT Sum(NOMINALBAYARPIUTANG + SISA_TAGIHAN) as HUTANG, sum(NOMINALBAYARPIUTANG) as NOMINAL_BAYAR FROM penjualan WHERE (@AwalBulan IS NULL OR TGL_TRANSAKSI >= @AwalBulan) AND (@AkhirBulan IS NULL OR TGL_TRANSAKSI <= @AkhirBulan) AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND STATUS_BAYAR LIKE 'TERHUTANG' AND STATUS_TRANSAKSI LIKE @STATUS"
        ElseIf LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY PELUNASAN" Then
            queryhitung = "SELECT Sum(NOMINALBAYARPIUTANG + SISA_TAGIHAN) as HUTANG, sum(NOMINALBAYARPIUTANG) as NOMINAL_BAYAR FROM penjualan WHERE (@AwalBulan IS NULL OR TGL_PEMBAYARAN >= @AwalBulan) AND (@AkhirBulan IS NULL OR TGL_PEMBAYARAN <= @AkhirBulan) AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND STATUS_BAYAR LIKE 'TERHUTANG' AND STATUS_TRANSAKSI LIKE @STATUS"
        Else
            queryhitung = "SELECT Sum(NOMINALBAYARPIUTANG + SISA_TAGIHAN) as HUTANG, sum(NOMINALBAYARPIUTANG) as NOMINAL_BAYAR FROM penjualan WHERE (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND STATUS_BAYAR LIKE 'TERHUTANG' AND STATUS_TRANSAKSI LIKE @STATUS"
        End If



        Using cmdHitung As New MySqlCommand(queryhitung, conn)
            cmdHitung.Parameters.AddWithValue("@AwalBulan", AwalBulan)
            cmdHitung.Parameters.AddWithValue("@AkhirBulan", AkhirBulan)
            cmdHitung.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", Supliyer))
            cmdHitung.Parameters.AddWithValue("@STATUS", String.Format("{0}%", Status))
            Using rd As MySqlDataReader = cmdHitung.ExecuteReader()
                rd.Read()
                If rd.HasRows Then
                    Dim HUTANG As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("HUTANG")), rd("HUTANG"), 0)
                    Dim NOMINAL_BAYAR As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("NOMINAL_BAYAR")), rd("NOMINAL_BAYAR"), 0)

                    TxtTotalHutang.Text = HUTANG.ToString("N0")
                    TxtBayar.Text = NOMINAL_BAYAR.ToString("N0")
                    TxtHutang.Text = HUTANG - NOMINAL_BAYAR.ToString("N0")
                End If
            End Using
        End Using

        Dim query As String
        If LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY PENJUALAN" Then
            query = "SELECT ID_PENJUALAN, TGL_TRANSAKSI, NAMA_PELANGGAN, GRAND_TOTAL_STL_PAJAK, (NOMINALBAYARPIUTANG + SISA_TAGIHAN) AS PIUTANG, NOMINALBAYARPIUTANG, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER FROM penjualan WHERE (@AwalBulan IS NULL OR TGL_TRANSAKSI >= @AwalBulan) AND (@AkhirBulan IS NULL OR TGL_TRANSAKSI <= @AkhirBulan) AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND STATUS_BAYAR LIKE 'TERHUTANG' AND STATUS_TRANSAKSI LIKE @STATUS ORDER BY ID_PENJUALAN"
        ElseIf LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY PELUNASAN" Then
            query = "SELECT ID_PENJUALAN, TGL_TRANSAKSI, NAMA_PELANGGAN, GRAND_TOTAL_STL_PAJAK, (NOMINALBAYARPIUTANG + SISA_TAGIHAN) AS PIUTANG, NOMINALBAYARPIUTANG, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER FROM penjualan WHERE (@AwalBulan IS NULL OR TGL_PEMBAYARAN >= @AwalBulan) AND (@AkhirBulan IS NULL OR TGL_PEMBAYARAN <= @AkhirBulan) AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND STATUS_BAYAR LIKE 'TERHUTANG' AND STATUS_TRANSAKSI LIKE @STATUS ORDER BY ID_PENJUALAN"
        Else
            query = "SELECT ID_PENJUALAN, TGL_TRANSAKSI, NAMA_PELANGGAN, GRAND_TOTAL_STL_PAJAK, (NOMINALBAYARPIUTANG + SISA_TAGIHAN) AS PIUTANG, NOMINALBAYARPIUTANG, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, ID_USER FROM penjualan WHERE (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) AND NAMA_PELANGGAN LIKE @NAMA_PELANGGAN AND STATUS_BAYAR LIKE 'TERHUTANG' AND STATUS_TRANSAKSI LIKE @STATUS ORDER BY ID_PENJUALAN"
        End If

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AwalBulan", If(AwalBulan = Date.MinValue, DBNull.Value, CType(AwalBulan, Object)))
            cmd.Parameters.AddWithValue("@AkhirBulan", If(AkhirBulan = Date.MaxValue, DBNull.Value, CType(AkhirBulan, Object)))
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", String.Format("%{0}%", Supliyer))
            cmd.Parameters.AddWithValue("@STATUS", String.Format("{0}%", Status))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using dataset As New DataSetKL()
                    dataset.Load(rd, LoadOption.OverwriteChanges, "Laporan_piutang")

                    ' Menambahkan parameter ke laporan RDLC
                    Dim parameters As New ReportParameterCollection From {
                        New ReportParameter("Supliyer", "PELANGGAN : " & CmbSupliyer.Text),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.SLogin.Text),
                        New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    }

                    ' Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("Laporan_piutang")))
                    ReportViewer1.LocalReport.SetParameters(parameters)

                    ' Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using

        Cursor = Cursors.Default
    End Sub


End Class