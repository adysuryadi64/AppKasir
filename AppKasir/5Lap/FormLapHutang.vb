Imports Microsoft.Reporting.WinForms



Public Class FormLapHutang

    Private Sub FormLapHutang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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

    Public Sub TampilSupliyer()
        CmbSupliyer.Items.Clear()
        CmbSupliyer.Items.Add("SEMUA") ' Tambahkan opsi "Semua"
        Dim query As String = "SELECT DISTINCT NAMA_SUPLIYER FROM pembelian ORDER BY NAMA_SUPLIYER"
        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbSupliyer.Items.Add(rd("NAMA_SUPLIYER").ToString())
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
            Dim Supliyer As String = If(CmbSupliyer.Text = "SEMUA" Or CmbSupliyer.SelectedIndex = 0, "", CmbSupliyer.Text)
            Dim Status As String = GetStatusPembayaran()

            Dim totalHutang As Decimal = 0
            Dim totalBayar As Decimal = 0


            Dim query As String = Nothing
            If LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY PEMBELIAN" Then
                query = "SELECT Sum(TAGIHAN + NOMINALBAYAR) as NOMINALHUTANG, Sum(NOMINALBAYAR) as NOMINALBAYAR " &
                                      "FROM pembelian " &
                                      "WHERE (@AwalBulan IS NULL OR TGL_BELI >= @AwalBulan) " &
                                      "AND (@AkhirBulan IS NULL OR TGL_BELI <= @AkhirBulan) " &
                                      "AND NAMA_SUPLIYER LIKE @Supliyer " &
                                      "AND STATUS_TRANSAKSI_BELI LIKE @Status " &
                                      "AND STATUS_JUAL = 'TERHUTANG'"

            ElseIf LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY PELUNASAN" Then
                query = "SELECT Sum(TAGIHAN + NOMINALBAYAR) as NOMINALHUTANG, Sum(NOMINALBAYAR) as NOMINALBAYAR " &
                                      "FROM pembelian " &
                                      "WHERE (@AwalBulan IS NULL OR TGL_BAYAR >= @AwalBulan) " &
                                      "AND (@AkhirBulan IS NULL OR TGL_BAYAR <= @AkhirBulan) " &
                                      "AND NAMA_SUPLIYER LIKE @Supliyer " &
                                      "AND STATUS_TRANSAKSI_BELI LIKE @Status " &
                                      "AND STATUS_JUAL = 'TERHUTANG'"
            Else
                query = "SELECT Sum(TAGIHAN + NOMINALBAYAR) as NOMINALHUTANG, Sum(NOMINALBAYAR) as NOMINALBAYAR " &
                                      "FROM pembelian " &
                                      "WHERE (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) " &
                                      "AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) " &
                                      "AND NAMA_SUPLIYER LIKE @Supliyer " &
                                      "AND STATUS_TRANSAKSI_BELI LIKE @Status " &
                                      "AND STATUS_JUAL = 'TERHUTANG'"
            End If


            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@AwalBulan", AwalBulan)
                cmd.Parameters.AddWithValue("@AkhirBulan", AkhirBulan)
                cmd.Parameters.AddWithValue("@Supliyer", String.Format("%{0}%", Supliyer))
                cmd.Parameters.AddWithValue("@Status", String.Format("{0}%", Status))

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        If Not rd.IsDBNull(rd.GetOrdinal("NOMINALHUTANG")) Then
                            totalHutang = rd.GetDecimal(rd.GetOrdinal("NOMINALHUTANG"))
                        Else
                            totalHutang = 0
                        End If

                        If Not rd.IsDBNull(rd.GetOrdinal("NOMINALBAYAR")) Then
                            totalBayar = rd.GetDecimal(rd.GetOrdinal("NOMINALBAYAR"))
                        Else
                            totalBayar = 0
                        End If
                    End If
                End Using
            End Using


            ' Tampilkan hasil perhitungan di TextBox
            TxtTotalHutang.Text = totalHutang.ToString("N0")
            TxtBayar.Text = totalBayar.ToString("N0")
            TxtHutang.Text = totalHutang - totalBayar.ToString("N0")

            Dim queryTampil As String = Nothing
            If LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY PEMBELIAN" Then
                queryTampil = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, TGL_BELI, GRAND_TOTAL_BELI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, TGL_BAYAR, NOMINALBAYAR, STATUS_TRANSAKSI_BELI, ID_USER " &
                                 "FROM pembelian " &
                                 "WHERE (@AwalBulan IS NULL OR TGL_BELI >= @AwalBulan) " &
                                 "AND (@AkhirBulan IS NULL OR TGL_BELI <= @AkhirBulan) " &
                                 "AND NAMA_SUPLIYER LIKE @Supliyer " &
                                 "AND STATUS_TRANSAKSI_BELI LIKE @Status " &
                                 "AND STATUS_JUAL = 'TERHUTANG'"

            ElseIf LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY PELUNASAN" Then
                queryTampil = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, TGL_BELI, GRAND_TOTAL_BELI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, TGL_BAYAR, NOMINALBAYAR, STATUS_TRANSAKSI_BELI, ID_USER " &
                                 "FROM pembelian " &
                                 "WHERE (@AwalBulan IS NULL OR TGL_BAYAR >= @AwalBulan) " &
                                 "AND (@AkhirBulan IS NULL OR TGL_BAYAR <= @AkhirBulan) " &
                                 "AND NAMA_SUPLIYER LIKE @Supliyer " &
                                 "AND STATUS_TRANSAKSI_BELI LIKE @Status " &
                                 "AND STATUS_JUAL = 'TERHUTANG'"
            Else
                queryTampil = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, TGL_BELI, GRAND_TOTAL_BELI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, TGL_BAYAR, NOMINALBAYAR, STATUS_TRANSAKSI_BELI, ID_USER " &
                                "FROM pembelian " &
                                "WHERE (@AwalBulan IS NULL OR JATUH_TEMPO >= @AwalBulan) " &
                                "AND (@AkhirBulan IS NULL OR JATUH_TEMPO <= @AkhirBulan) " &
                                "AND NAMA_SUPLIYER LIKE @Supliyer " &
                                "AND STATUS_TRANSAKSI_BELI LIKE @Status " &
                                "AND STATUS_JUAL = 'TERHUTANG'"
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
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.SLogin.Text),
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





End Class