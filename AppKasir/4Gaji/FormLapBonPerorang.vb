Imports System.Globalization
Imports Microsoft.Reporting.WinForms

Public Class FormLapBonPerorang

    Private Sub FormLapBonPerorang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        AmbilDataKaryawan()


        DtpAwal.Value = tanggalAwalPeriodeKerja
        DtpAkhir.Value = tanggalAkhirPeriodeKerja

        Me.ReportViewer1.RefreshReport()
    End Sub



    Private Sub AmbilDataKaryawan()
        CmbNama.Items.Clear()
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan WHERE Status = 'Aktif' ORDER BY Nama ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        CmbNama.Items.Add(rd("Nama").ToString())
                    End While

                End If
            End Using
        End Using
        CmbNama.SelectedIndex = 0
    End Sub


    Private Sub CmbNama_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbNama.SelectedIndexChanged
        Dim sql As String = "SELECT Kode, Gaji FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbNama.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKode.Text = reader("Kode").ToString()
                Else
                    LblKode.Text = ""
                End If
            End Using
        End Using
    End Sub


    Private Sub BtnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTampilBB.Click

        Dim deleteQuery As String = "DELETE FROM Temp_Bon_Karyawan"
        Using deleteCmd As New MySqlCommand(deleteQuery, conn)
            deleteCmd.ExecuteNonQuery()
        End Using

        Ambilsaldoawal()
        AmbilDanMasukkanData()
        HitungSaldoTempBonKaryawan()
        TampilkanLaporanBonKaryawan()
    End Sub

    Public Sub Ambilsaldoawal()
        Dim NominalDebet As Decimal = 0
        Dim NominalKredit As Decimal = 0

        Dim tanggal As Date = DtpAwal.Value.Date.AddTicks(-1)

        ' Query untuk mendapatkan total nominal debet (BON)
        Dim queryDebet As String = "SELECT SUM(NOMINAL) AS NominalDebet FROM Bon_Karyawan WHERE KODE LIKE @KODE AND TANGGAL < @TANGGAL AND JENIS = 'BON'"
        Using cmdDebet As New MySqlCommand(queryDebet, conn)
            cmdDebet.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmdDebet.Parameters.AddWithValue("@TANGGAL", tanggal.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdDebet As MySqlDataReader = cmdDebet.ExecuteReader()
                If rdDebet.Read() Then
                    NominalDebet = ModuleAngka.SafeGetValue(Of Decimal)(rdDebet, "NominalDebet", 0D)
                End If
            End Using
        End Using

        ' Query untuk mendapatkan total nominal kredit (BAYAR)
        Dim queryKredit As String = "SELECT SUM(NOMINAL) AS NominalKredit FROM Bon_Karyawan WHERE KODE LIKE @KODE AND TANGGAL < @TANGGAL AND JENIS = 'BAYAR'"
        Using cmdKredit As New MySqlCommand(queryKredit, conn)
            cmdKredit.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmdKredit.Parameters.AddWithValue("@TANGGAL", tanggal.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdKredit As MySqlDataReader = cmdKredit.ExecuteReader()
                If rdKredit.Read() Then
                    NominalKredit = ModuleAngka.SafeGetValue(Of Decimal)(rdKredit, "NominalKredit", 0D)
                End If
            End Using
        End Using

        ' Menghitung saldo awal (Debet - Kredit)
        Dim saldoAwal As Decimal = NominalDebet - NominalKredit

        ' Kode untuk memasukkan data ke Temp_Bon_Karyawan
        Dim insertQuery As String = "INSERT INTO Temp_Bon_Karyawan (NO, NOMOR, TANGGAL, JENIS, DEBET, KREDIT, SALDO) " &
                                    "VALUES (@NO, @NOMOR, @TANGGAL, @JENIS, @DEBET, @KREDIT, @SALDO)"

        Using cmdInsert As New MySqlCommand(insertQuery, conn)
            ' Tambahkan parameter ke command
            cmdInsert.Parameters.AddWithValue("@NO", 1) ' Pastikan tipe data integer jika kolom NO adalah integer
            cmdInsert.Parameters.AddWithValue("@NOMOR", "SA-000001")
            cmdInsert.Parameters.AddWithValue("@TANGGAL", tanggal.ToString("yyyy-MM-dd HH:mm:ss")) ' Format tanggal
            cmdInsert.Parameters.AddWithValue("@JENIS", "SALDO AWAL")
            cmdInsert.Parameters.AddWithValue("@DEBET", NominalDebet)
            cmdInsert.Parameters.AddWithValue("@KREDIT", NominalKredit)
            cmdInsert.Parameters.AddWithValue("@SALDO", saldoAwal)

            ' Eksekusi perintah
            cmdInsert.ExecuteNonQuery()
        End Using

    End Sub

    Public Sub AmbilDanMasukkanData()
        Dim Nomor As Integer = 2 ' Mulai dari 2
        Dim tanggalAwal As Date = DtpAwal.Value.Date
        Dim tanggalAkhir As Date = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)

        ' Query untuk mengambil data berdasarkan rentang tanggal
        Dim query As String = "SELECT FAKTUR, TANGGAL, JENIS, KETERANGAN, NOMINAL FROM Bon_Karyawan WHERE KODE = @KODE AND TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir ORDER BY TANGGAL"

        Dim dataList As New List(Of Dictionary(Of String, Object))()

        ' Ambil data ke dalam list sementara
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim data As New Dictionary(Of String, Object) From {
                    {"FAKTUR", rd("FAKTUR").ToString()},
                    {"TANGGAL", Convert.ToDateTime(rd("TANGGAL"))},
                    {"JENIS", rd("JENIS").ToString()},
                    {"KETERANGAN", rd("KETERANGAN").ToString()},
                    {"NOMINAL", ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINAL", 0D)}
                }
                    dataList.Add(data)
                End While
            End Using
        End Using

        ' Masukkan data ke dalam tabel Temp_Bon_Karyawan
        For Each Data As Dictionary(Of String, Object) In dataList
            Dim insertQuery As String = "INSERT INTO Temp_Bon_Karyawan (NO, NOMOR, TANGGAL, JENIS, KETERANGAN, DEBET, KREDIT) " &
                                    "VALUES (@NO, @NOMOR, @TANGGAL, @JENIS, @KETERANGAN, @DEBET, @KREDIT)"

            Using cmdInsert As New MySqlCommand(insertQuery, conn)
                cmdInsert.Parameters.AddWithValue("@NO", Nomor)
                cmdInsert.Parameters.AddWithValue("@NOMOR", Data("FAKTUR"))
                cmdInsert.Parameters.AddWithValue("@TANGGAL", Convert.ToDateTime(Data("TANGGAL")).ToString("yyyy-MM-dd HH:mm:ss"))
                cmdInsert.Parameters.AddWithValue("@JENIS", Data("JENIS"))
                cmdInsert.Parameters.AddWithValue("@KETERANGAN", Data("KETERANGAN"))

                ' Jika JENIS adalah "BON", maka nominal masuk ke DEBET, selain itu masuk ke KREDIT
                If Data("JENIS").ToString() = "BON" Then
                    cmdInsert.Parameters.AddWithValue("@DEBET", Data("NOMINAL"))
                    cmdInsert.Parameters.AddWithValue("@KREDIT", 0)
                Else
                    cmdInsert.Parameters.AddWithValue("@DEBET", 0)
                    cmdInsert.Parameters.AddWithValue("@KREDIT", Data("NOMINAL"))
                End If

                cmdInsert.ExecuteNonQuery()
                Nomor += 1 ' Tambah nomor setiap kali data dimasukkan
            End Using
        Next
    End Sub


    Public Sub HitungSaldoTempBonKaryawan()
        ' Inisialisasi variabel untuk menyimpan saldo sebelumnya
        Dim saldoSebelumnya As Decimal = 0
        Dim saldoSekarang As Decimal
        Dim nomor As Integer = 1

        ' Query untuk mengambil data dari Temp_Bon_Karyawan dengan urutan nomor
        Dim query As String = "SELECT NO, DEBET, KREDIT FROM Temp_Bon_Karyawan ORDER BY NO"

        ' List untuk menyimpan data sementara
        Dim dataList As New List(Of Dictionary(Of String, Object))()

        ' Ambil data ke dalam list sementara
        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Pastikan hasil query tidak kosong
                If rd.HasRows Then
                    While rd.Read()
                        Dim data As New Dictionary(Of String, Object) From {
                        {"NO", rd("NO")},
                        {"DEBET", ModuleAngka.SafeGetValue(Of Decimal)(rd, "DEBET", 0D)},
                        {"KREDIT", ModuleAngka.SafeGetValue(Of Decimal)(rd, "KREDIT", 0D)}
                    }
                        dataList.Add(data)
                    End While
                End If
            End Using
        End Using

        ' Loop untuk menghitung saldo dan memperbarui data
        For Each Data As Dictionary(Of String, Object) In dataList
            Dim debet As Decimal = Convert.ToDecimal(Data("DEBET"))
            Dim kredit As Decimal = Convert.ToDecimal(Data("KREDIT"))
            Dim no As Integer = Convert.ToInt32(Data("NO"))

            ' Jika nomor adalah 1, maka saldo = debet - kredit
            If nomor = 1 Then
                saldoSekarang = debet - kredit
            Else
                ' Untuk nomor selanjutnya, saldo = saldo sebelumnya + debet - kredit
                saldoSekarang = saldoSebelumnya + debet - kredit
            End If

            ' Simpan saldo sekarang untuk digunakan di nomor berikutnya
            saldoSebelumnya = saldoSekarang

            ' Update saldo pada baris yang sesuai di Temp_Bon_Karyawan
            Dim updateQuery As String = "UPDATE Temp_Bon_Karyawan SET SALDO = @SALDO WHERE NO = @NO"
            Using cmdUpdate As New MySqlCommand(updateQuery, conn)
                cmdUpdate.Parameters.AddWithValue("@SALDO", saldoSekarang)
                cmdUpdate.Parameters.AddWithValue("@NO", no)
                cmdUpdate.ExecuteNonQuery()
            End Using

            nomor += 1 ' Pindah ke nomor berikutnya
        Next




    End Sub

    Public Sub TampilkanLaporanBonKaryawan()

        ReportViewer1.LocalReport.DataSources.Clear()

        ' Daftar query untuk masing-masing jenis akun
        Dim queryBonKaryawan As String = "SELECT NO, NOMOR, TANGGAL, JENIS, KETERANGAN, DEBET, KREDIT, SALDO FROM Temp_Bon_Karyawan ORDER BY TANGGAL"

        ' Ambil data HPP
        Using cmdBB As New MySqlCommand(queryBonKaryawan, conn)
            Using rd As MySqlDataReader = cmdBB.ExecuteReader()
                Using datasetBB As New DataSetKL()
                    datasetBB.Load(rd, LoadOption.OverwriteChanges, "Temp_Bon_Karyawan")
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetBB.Tables("Temp_Bon_Karyawan")))
                End Using
            End Using
        End Using

        Dim periodeawal As Date = DtpAwal.Value
        Dim tanggalawal As Integer = periodeawal.Day
        Dim namaBulanawal As String = periodeawal.ToString("MMMM", New CultureInfo("id-ID"))
        Dim tahunawal As Integer = periodeawal.Year

        Dim periode As Date = DtpAkhir.Value
        Dim tanggal As Integer = periode.Day
        Dim namaBulan As String = periode.ToString("MMMM", New CultureInfo("id-ID"))
        Dim tahun As Integer = periode.Year

        ' Menambahkan parameter ke dalam report
        Dim parameters As New List(Of Microsoft.Reporting.WinForms.ReportParameter)()

        ' Tambahkan parameter untuk Nama Perusahaan, Periode, Nama Karyawan, dan User
        parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("NamaPerusahaan", NAMA_PERUSAHAAN))
        parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("Periode", String.Format("Periode {0} {1} {2} S/d {3} {4} {5}", tanggalawal, namaBulanawal, tahunawal, tanggal, namaBulan, tahun)))
        parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("NamaKaryawan", String.Format("{0} {1}", LblKode.Text, CmbNama.Text)))
        parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("User", FormUtama.StatusNamaUser.Text))

        ' Set parameters ke ReportViewer
        ReportViewer1.LocalReport.SetParameters(parameters)

        ' Refresh report untuk menampilkan data terbaru
        ReportViewer1.RefreshReport()

    End Sub


    Private Sub FormLapBonPerorang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampilBB.PerformClick()
        End Select
    End Sub

End Class
