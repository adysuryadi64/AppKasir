Imports System.Globalization
Imports Microsoft.Reporting.WinForms


Public Class FormLapJurnal
    Private Sub ReportViewer1_Load(sender As Object, e As EventArgs) Handles ReportViewer1.Load
        CbTanggal.Checked = True
        ReportViewer1.LocalReport.DataSources.Clear()
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

                        Dim Periode As String = "Periode : " & AwalBulan.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " - " & AkhirBulan.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))

                        Dim parameters As New ReportParameterCollection From {
                        New ReportParameter("PERIODE", Periode),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.SLogin.Text),
                        New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN)
                    }

                        ' Menetapkan dataset ke laporan RDLC
                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("jurnalumum")))
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

    End Sub
End Class