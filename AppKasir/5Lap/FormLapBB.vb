Imports System.Globalization
Imports Microsoft.Reporting.WinForms

Public Class FormLapBB
    Private Sub FormLapBB_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        'ReportViewer3.LocalReport.DataSources.Clear()
        Rekening()
    End Sub


    Public Sub Rekening()
        CbmAkunBB.Items.Clear()
        Dim namaakun As String = "SELECT Nama_Akun FROM tbl_datareferensi ORDER BY Kode_akun ASC"

        Using cmd As New MySqlCommand(namaakun, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        Dim combinedValue As String = rd("Nama_Akun").ToString()
                        CbmAkunBB.Items.Add(combinedValue)
                    End While
                End If
            End Using
        End Using

    End Sub

    Private Sub CbmAkunBB_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbmAkunBB.SelectedIndexChanged

        Using cmd As New MySqlCommand("SELECT Kode_akun, Akun_DK FROM tbl_datareferensi WHERE Nama_Akun = @nama", conn)
            cmd.Parameters.AddWithValue("@nama", CbmAkunBB.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtAkunBB.Text = rd.Item("Kode_akun")
                    TxtAkunDK.Text = rd.Item("Akun_DK")
                Else
                    TxtAkunBB.Text = ""
                    TxtAkunDK.Text = ""
                End If
            End Using
        End Using
    End Sub

    Private Sub BtnTampilBB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTampilBB.Click
        Cursor = Cursors.WaitCursor

        Dim deleteQuery As String = "DELETE FROM TempJurnalUmum"
        Using deleteCmd As New MySqlCommand(deleteQuery, conn)
            deleteCmd.ExecuteNonQuery()
        End Using

        AmbilSaldoAwal()
        AmbilDanMasukkanData()
        IsiSaldo()
        Tampilbukubesar()

        Cursor = Cursors.Default
    End Sub

    Public Sub AmbilSaldoAwal()
        ' Deklarasi variabel untuk penyimpanan data
        Dim saldoAwalData As New List(Of Dictionary(Of String, Object))()

        ' Koneksi dan query pertama untuk mendapatkan data dari tbl_datareferensi
        Dim selectQuery As String = "SELECT Akun_DK, Saldo_Awal " &
                                "FROM tbl_datareferensi " &
                                "WHERE Kode_akun LIKE @AkunBB"

        Using cmdSelect As New MySqlCommand(selectQuery, conn)
            cmdSelect.Parameters.AddWithValue("@AkunBB", TxtAkunBB.Text)

            Using rd As MySqlDataReader = cmdSelect.ExecuteReader()
                While rd.Read()
                    ' Simpan data dari reader ke dalam dictionary
                    saldoAwalData.Add(New Dictionary(Of String, Object) From {
                    {"AkunDK", rd("Akun_DK").ToString()},
                    {"SaldoAwal", ModuleAngka.SafeGetValue(Of Decimal)(rd, "Saldo_Awal", 0D)}
                })
                End While
            End Using
        End Using

        ' Variabel untuk memproses data lebih lanjut
        Dim tanggal As Date = DtpAwal.Value.Date.AddTicks(-1)

        ' Query kedua untuk debet
        Dim queryDebet As String = "SELECT SUM(NOMINAL) AS NOMINAL_DEBET FROM JurnalUmum WHERE NOMOR_AKUN_D LIKE @AKUN AND TGL_TRANSAKSI < @TANGGAL"
        Dim NominalDebet As Decimal = 0
        Using cmdDebet As New MySqlCommand(queryDebet, conn)
            cmdDebet.Parameters.AddWithValue("@AKUN", TxtAkunBB.Text)
            cmdDebet.Parameters.AddWithValue("@TANGGAL", tanggal.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rdDebet As MySqlDataReader = cmdDebet.ExecuteReader()
                If rdDebet.Read() Then
                    NominalDebet = ModuleAngka.SafeGetValue(Of Decimal)(rdDebet, "NOMINAL_DEBET", 0D)
                End If
            End Using
        End Using

        ' Query ketiga untuk kredit
        Dim queryKredit As String = "SELECT SUM(NOMINAL) AS NOMINAL_KREDIT FROM JurnalUmum WHERE NOMOR_AKUN_K LIKE @AKUN AND TGL_TRANSAKSI < @TANGGAL"
        Dim NominalKredit As Decimal = 0
        Using cmdKredit As New MySqlCommand(queryKredit, conn)
            cmdKredit.Parameters.AddWithValue("@AKUN", TxtAkunBB.Text)
            cmdKredit.Parameters.AddWithValue("@TANGGAL", tanggal.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rdKredit As MySqlDataReader = cmdKredit.ExecuteReader()
                If rdKredit.Read() Then
                    NominalKredit = ModuleAngka.SafeGetValue(Of Decimal)(rdKredit, "NOMINAL_KREDIT", 0D)
                End If
            End Using
        End Using

        ' Proses data dari saldoAwalData
        For Each akunData As Dictionary(Of String, Object) In saldoAwalData
            Dim AkunDK As String = akunData("AkunDK").ToString()
            Dim SaldoAwal As Decimal = Convert.ToDecimal(akunData("SaldoAwal"))

            Dim jumlahDebet As Decimal = If(AkunDK = "DEBET", NominalDebet + SaldoAwal, NominalDebet)
            Dim jumlahKredit As Decimal = If(AkunDK = "KREDIT", NominalKredit + SaldoAwal, NominalKredit)

            ' Masukkan data ke TempJurnalUmum
            Dim insertQuery As String = "INSERT INTO TempJurnalUmum (NOMOR, TGLTRANSAKSI, URAIAN, DEBET, KREDIT) " &
                                     "VALUES (@NOMOR, @TGLTRANSAKSI, @URAIAN, @DEBET, @KREDIT)"
            Using cmdInsert As New MySqlCommand(insertQuery, conn)
                cmdInsert.Parameters.AddWithValue("@NOMOR", "1")
                cmdInsert.Parameters.AddWithValue("@TGLTRANSAKSI", tanggal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdInsert.Parameters.AddWithValue("@URAIAN", "Saldo awal")
                cmdInsert.Parameters.AddWithValue("@DEBET", jumlahDebet)
                cmdInsert.Parameters.AddWithValue("@KREDIT", jumlahKredit)
                cmdInsert.ExecuteNonQuery()
            End Using
        Next

    End Sub


    Public Sub AmbilDanMasukkanData()
        Dim Nomor As Integer = 2 ' Mulai dari 2
        Dim tanggalAwal As Date = DtpAwal.Value.Date
        Dim tanggalAkhir As Date = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT JENIS_TRANSAKSI, NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NOMOR_AKUN_D, NOMOR_AKUN_K, NOMINAL FROM JurnalUmum WHERE (NOMOR_AKUN_D = @AKUN OR NOMOR_AKUN_K = @AKUN) AND TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR ORDER BY TGL_TRANSAKSI"

        ' List untuk menyimpan data hasil query
        Dim dataList As New List(Of Dictionary(Of String, Object))

        ' Membaca data dari database dan menyimpannya ke dalam list
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AKUN", TxtAkunBB.Text)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    ' Simpan setiap baris data dalam Dictionary
                    Dim row As New Dictionary(Of String, Object)
                    row("JENIS_TRANSAKSI") = rd("JENIS_TRANSAKSI")
                    row("NO_TRANSAKSI") = rd("NO_TRANSAKSI")
                    row("TGL_TRANSAKSI") = rd("TGL_TRANSAKSI")
                    row("NO_NOTA") = rd("NO_NOTA")
                    row("URAIAN") = rd("URAIAN")
                    row("NOMOR_AKUN_D") = rd("NOMOR_AKUN_D")
                    row("NOMOR_AKUN_K") = rd("NOMOR_AKUN_K")
                    row("NOMINAL") = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINAL", 0D)

                    ' Tambahkan row ke dalam list
                    dataList.Add(row)
                End While
            End Using
        End Using


        Using transaction = conn.BeginTransaction()
            Try
                ' Memasukkan data dari list ke dalam TempJurnalUmum dalam satu transaksi
                For Each row As Dictionary(Of String, Object) In dataList
                    Dim AkunDebet As String = row("NOMOR_AKUN_D").ToString()
                    Dim AkunKredit As String = row("NOMOR_AKUN_K").ToString()
                    Dim Nominal As Decimal = Convert.ToDecimal(row("NOMINAL"))

                    ' Siapkan pernyataan INSERT INTO
                    Dim insertQuery As String = "INSERT INTO TempJurnalUmum (NOMOR, JENISTRANSAKSI, NOTRANSAKSI, TGLTRANSAKSI, NONOTA, URAIAN, DEBET, KREDIT) VALUES (@NOMOR, @JENISTRANSAKSI, @NOTRANSAKSI, @TGLTRANSAKSI, @NONOTA, @URAIAN, @DEBET, @KREDIT)"

                    Using insertCmd As New MySqlCommand(insertQuery, conn)
                        insertCmd.Transaction = transaction
                        insertCmd.Parameters.AddWithValue("@NOMOR", Nomor)
                        Nomor += 1
                        insertCmd.Parameters.AddWithValue("@JENISTRANSAKSI", row("JENIS_TRANSAKSI"))
                        insertCmd.Parameters.AddWithValue("@NOTRANSAKSI", row("NO_TRANSAKSI"))
                        insertCmd.Parameters.AddWithValue("@TGLTRANSAKSI", row("TGL_TRANSAKSI"))
                        insertCmd.Parameters.AddWithValue("@NONOTA", row("NO_NOTA"))
                        insertCmd.Parameters.AddWithValue("@URAIAN", row("URAIAN"))

                        If AkunDebet = TxtAkunBB.Text Then
                            insertCmd.Parameters.AddWithValue("@DEBET", Nominal)
                            insertCmd.Parameters.AddWithValue("@KREDIT", DBNull.Value)
                        Else
                            insertCmd.Parameters.AddWithValue("@DEBET", DBNull.Value)
                            insertCmd.Parameters.AddWithValue("@KREDIT", Nominal)
                        End If

                        insertCmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi setelah semua data di-insert
                transaction.Commit()
            Catch ex As Exception
                ' Jika ada error, rollback transaksi
                transaction.Rollback()
                Throw
            End Try
        End Using
    End Sub

    Public Sub IsiSaldo()
        ' List untuk menyimpan data sementara dari tabel TempJurnalUmum
        Dim dataList As New List(Of Dictionary(Of String, Object))

        ' Ambil data dari tabel TempJurnalUmum
        Dim selectQuery As String = "SELECT NOMOR, DEBET, KREDIT FROM TempJurnalUmum"
        Using cmdSelect As New MySqlCommand(selectQuery, conn)
            Using rd As MySqlDataReader = cmdSelect.ExecuteReader()
                While rd.Read()
                    Dim row As New Dictionary(Of String, Object)
                    row("NOMOR") = ModuleAngka.SafeGetValue(Of Integer)(rd, "NOMOR", 0)
                    row("DEBET") = ModuleAngka.SafeGetValue(Of Decimal)(rd, "DEBET", 0D)
                    row("KREDIT") = ModuleAngka.SafeGetValue(Of Decimal)(rd, "KREDIT", 0D)

                    ' Tambahkan row ke dalam list
                    dataList.Add(row)
                End While
            End Using
        End Using

        ' Sekarang lakukan pembaruan saldo untuk setiap baris data dalam list
        For Each row As Dictionary(Of String, Object) In dataList
            Dim Nomor As Integer = Convert.ToInt32(row("NOMOR"))
            Dim Debet As Decimal = Convert.ToDecimal(row("DEBET"))
            Dim Kredit As Decimal = Convert.ToDecimal(row("KREDIT"))
            Dim Saldo As Decimal = 0

            ' Untuk Nomor pertama
            If Nomor = 1 Then
                If TxtAkunDK.Text = "DEBET" Then
                    Saldo = Debet - Kredit
                Else
                    Saldo = Kredit - Debet
                End If
            Else
                ' Untuk Nomor > 1, ambil Saldo dari nomor sebelumnya
                Dim SaldoSebelumnya As Decimal = 0
                Dim querySaldoSebelumnya As String = "SELECT SALDO FROM TempJurnalUmum WHERE NOMOR = @NomorSebelumnya"
                Using cmdSaldoSebelumnya As New MySqlCommand(querySaldoSebelumnya, conn)
                    cmdSaldoSebelumnya.Parameters.AddWithValue("@NomorSebelumnya", Nomor - 1)
                    Dim result As Object = cmdSaldoSebelumnya.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                        SaldoSebelumnya = Convert.ToDecimal(result)
                    End If
                End Using

                ' Hitung saldo berdasarkan akun DEBET atau KREDIT
                If TxtAkunDK.Text = "DEBET" Then
                    Saldo = SaldoSebelumnya + Debet - Kredit
                Else
                    Saldo = SaldoSebelumnya - Debet + Kredit
                End If
            End If

            ' Update saldo ke dalam database
            Dim updateQuery As String = "UPDATE TempJurnalUmum SET SALDO = @Saldo WHERE NOMOR = @Nomor"
            Using cmdUpdate As New MySqlCommand(updateQuery, conn)
                cmdUpdate.Parameters.AddWithValue("@Saldo", Saldo)
                cmdUpdate.Parameters.AddWithValue("@Nomor", Nomor)
                cmdUpdate.ExecuteNonQuery()
            End Using
        Next
    End Sub


    Private Sub Tampilbukubesar()
        ReportViewer3.LocalReport.DataSources.Clear()

        ' Daftar query untuk masing-masing jenis akun
        Dim QueryBB As String = "SELECT NOTRANSAKSI, TGLTRANSAKSI, URAIAN, DEBET, KREDIT, SALDO FROM TempJurnalUmum ORDER BY TGLTRANSAKSI"

        ' Ambil data HPP
        Using cmdBB As New MySqlCommand(QueryBB, conn)
            Using rd As MySqlDataReader = cmdBB.ExecuteReader()
                Using datasetBB As New DataSetKL()
                    datasetBB.Load(rd, LoadOption.OverwriteChanges, "BukuBesar")
                    ReportViewer3.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetBB.Tables("BukuBesar")))
                End Using
            End Using
        End Using

        ' Mengambil bulan dan tahun
        Dim periodeawal As Date = DtpAkhir.Value
        Dim tanggalawal As Integer = periodeawal.Day
        Dim namaBulanawal As String = periodeawal.ToString("MMMM", New CultureInfo("id-ID"))
        Dim tahunawal As Integer = periodeawal.Year

        Dim periode As Date = DtpAkhir.Value
        Dim tanggal As Integer = periode.Day
        Dim namaBulan As String = periode.ToString("MMMM", New CultureInfo("id-ID"))
        Dim tahun As Integer = periode.Year

        ' Set parameter laporan
        Dim parameters As ReportParameter() = New ReportParameter(3) {}
        parameters(0) = New ReportParameter("NAMA", NAMA_PERUSAHAAN)
        parameters(1) = New ReportParameter("PERIODE", "Periode " & tanggalawal & " " & namaBulanawal & " " & tahunawal & " S/d : " & tanggal & " " & namaBulan & " " & tahun)
        parameters(2) = New ReportParameter("AKUN", "AKUN : " & TxtAkunBB.Text & " " & CbmAkunBB.Text)
        parameters(3) = New ReportParameter("AKUNDK", TxtAkunDK.Text)

        ReportViewer3.LocalReport.SetParameters(parameters)
        ReportViewer3.RefreshReport()
    End Sub




    Private Sub FormLapBB_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampilBB.PerformClick()
        End Select
    End Sub

End Class
