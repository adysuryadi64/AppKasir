Imports System.Globalization
Imports Microsoft.Reporting.WinForms


Public Class FormLapNeracaLR

    Private Sub FormLaporan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub


    Private Sub RbtRentang_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbtTanggal.CheckedChanged
        ' Pastikan RadioButton dalam keadaan Checked sebelum memanggil Ambildataperusahaan
        If RbtTanggal.Checked Then
            'DtpAwal.Value = tanggalAwalPeriodeKerja
            'DtpAkhir.Value = tanggalAkhirPeriodeKerja
        End If
    End Sub

    Private Sub RbtBulan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbtBulan.CheckedChanged
        If RbtBulan.Checked Then
            MuatComboBoxBulanTahun()
        End If
    End Sub




    '---------------------------------------- NERACA ----------------------------------------------------------------------------------------
    '---------------------------------------- NERACA ----------------------------------------------------------------------------------------
    '---------------------------------------- NERACA ----------------------------------------------------------------------------------------
    '---------------------------------------- NERACA ----------------------------------------------------------------------------------------

    Public Sub HITUNGSEMUASALDO()
        ' 0===================> update Saldo_Sebelumnya dari SALDO_AWAL

        ' Pertama, ambil semua KODE_AKUN dari tabel
        Dim selectKodeAkunQuery As String = "SELECT KODE_AKUN FROM tbl_datareferensi"
        Dim kodeAkunList As New List(Of String)()

        Using cmdSelect As New MySqlCommand(selectKodeAkunQuery, conn)
            Using reader As MySqlDataReader = cmdSelect.ExecuteReader()
                While reader.Read()
                    kodeAkunList.Add(reader("KODE_AKUN").ToString())
                End While
            End Using
        End Using

        ' Lakukan loop untuk setiap KODE_AKUN dan update Saldo_Sebelumnya dengan SALDO_AWAL
        For Each kodeAkun As String In kodeAkunList
            Dim updateSaldoQuery As String = "UPDATE tbl_datareferensi SET SALDO_SEBELUMNYA = SALDO_AWAL WHERE KODE_AKUN = @KODE_AKUN"

            Using cmdUpdateSaldo As New MySqlCommand(updateSaldoQuery, conn)
                ' Tambahkan nilai parameter KODE_AKUN
                cmdUpdateSaldo.Parameters.AddWithValue("@KODE_AKUN", kodeAkun)

                ' Eksekusi query untuk memperbarui database
                cmdUpdateSaldo.ExecuteNonQuery()
            End Using
        Next


        Try
            ' Mulai transaksi
            Using transaction As MySqlTransaction = conn.BeginTransaction()

                ' Dictionary untuk menyimpan hasil pembacaan
                Dim akunTotals As New Dictionary(Of String, Tuple(Of Decimal, Decimal))()

                ' Gabungkan penghitungan total debet dan kredit untuk setiap KODE_AKUN
                Dim hitungTotal As String = "SELECT tbl_datareferensi.KODE_AKUN, " &
                            "SUM(CASE WHEN JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_DEBET, " &
                            "SUM(CASE WHEN JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_KREDIT " &
                            "FROM tbl_datareferensi " &
                            "LEFT JOIN JurnalUmum ON JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN OR JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN " &
                            "GROUP BY tbl_datareferensi.KODE_AKUN"


                ' Membaca data dan menyimpan ke dictionary
                Using cmdHitungTotal As New MySqlCommand(hitungTotal, conn, transaction)
                    Using rd As MySqlDataReader = cmdHitungTotal.ExecuteReader()
                        While rd.Read()
                            Dim kodeAkun As String = rd("KODE_AKUN").ToString()
                            Dim totalDebet As Decimal = If(rd("TOTAL_DEBET") IsNot DBNull.Value, CDec(rd("TOTAL_DEBET")), 0)
                            Dim totalKredit As Decimal = If(rd("TOTAL_KREDIT") IsNot DBNull.Value, CDec(rd("TOTAL_KREDIT")), 0)

                            ' Simpan hasil ke dictionary
                            akunTotals(kodeAkun) = Tuple.Create(totalDebet, totalKredit)
                        End While
                    End Using
                End Using

                ' Gunakan data dari dictionary untuk melakukan update
                Dim updateTotal As String = "UPDATE tbl_datareferensi SET S_DEBET = @TotalDebet, S_KREDIT = @TotalKredit WHERE KODE_AKUN = @KODE_AKUN"
                Using cmdUpdateTotal As New MySqlCommand(updateTotal, conn, transaction)
                    For Each kvp As KeyValuePair(Of String, Tuple(Of Decimal, Decimal)) In akunTotals
                        cmdUpdateTotal.Parameters.Clear()
                        cmdUpdateTotal.Parameters.AddWithValue("@TotalDebet", kvp.Value.Item1)
                        cmdUpdateTotal.Parameters.AddWithValue("@TotalKredit", kvp.Value.Item2)
                        cmdUpdateTotal.Parameters.AddWithValue("@KODE_AKUN", kvp.Key)
                        cmdUpdateTotal.ExecuteNonQuery()
                    Next
                End Using


                ' Commit transaksi
                transaction.Commit()

            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


        ' 2===================> Update kolom SALDO_AKHIR dengan menambahkan SALDO_SEBELUMNYA, S_DEBET, dan mengurangkan S_KREDIT jika KODE_AKUN sama
        Dim hitungSaldo As String = "UPDATE tbl_datareferensi " &
                             "SET SALDO_AKHIR = CASE " &
                             "WHEN AKUN_DK = 'DEBET' THEN SALDO_SEBELUMNYA + S_DEBET - S_KREDIT " &
                             "WHEN AKUN_DK = 'KREDIT' THEN SALDO_SEBELUMNYA - S_DEBET + S_KREDIT " &
                             "ELSE 0 END " &
                             "WHERE TYPE_AKUN <> 'LABA RUGI'"

        Using cmd As New MySqlCommand(hitungSaldo, conn)
            cmd.ExecuteNonQuery()
        End Using



        ' 3===================> Hitung total SALDO_AWAL, total Debet dan Kredit
        ' Hitung total SALDO_AWAL untuk SUB_AKUN 'LABA'
        Dim totalLaba As Decimal = HitungTotalSaldoAwal("LABA", conn)

        ' Hitung total SALDO_AWAL untuk SUB_AKUN 'RUGI'
        Dim totalRugi As Decimal = HitungTotalSaldoAwal("RUGI", conn)

        ' Hitung Laba Rugi
        Dim labaRugi As Decimal = totalLaba - totalRugi

        ' Perbarui tabel tbl_datareferensi dengan Laba Rugi
        PerbaruiSaldoAwal("LABA RUGI", labaRugi, conn)

        ' Hitung total Debet dan Kredit untuk SUB_AKUN 'LABA'
        Dim totalDebetLaba As Decimal = HitungTotalDebet("LABA", conn)
        Dim totalKreditLaba As Decimal = HitungTotalKredit("LABA", conn)

        ' Hitung total Debet dan Kredit untuk SUB_AKUN 'RUGI'
        Dim totalDebetRugi As Decimal = HitungTotalDebet("RUGI", conn)
        Dim totalKreditRugi As Decimal = HitungTotalKredit("RUGI", conn)

        ' Hitung Debet dan Kredit untuk Laba Rugi
        Dim debetLabaRugi As Decimal = totalDebetLaba + totalDebetRugi
        Dim kreditLabaRugi As Decimal = totalKreditLaba + totalKreditRugi

        ' Perbarui tabel tbl_datareferensi dengan hasil perhitungan Debet dan Kredit
        PerbaruiDebetKredit("LABA RUGI", debetLabaRugi, kreditLabaRugi, conn)


        ' 4===================> Update kolom Saldo Akhir laba rugi dengan menambahkan Saldo Awal, Debet, dan mengurangkan Kredit jika kode akun sama hanya pada laba rugi
        Dim hitungSaldolabarugi As String = "UPDATE tbl_datareferensi " &
                               "SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT Where TYPE_AKUN = 'LABA RUGI'"

        Using cmd As New MySqlCommand(hitungSaldolabarugi, conn)
            cmd.ExecuteNonQuery()
        End Using


    End Sub

    ' Fungsi untuk menghitung total SALDO_AWAL
    Public Function HitungTotalSaldoAwal(ByVal subAkun As String, ByVal conn As MySqlConnection) As Decimal
        Dim totalSaldo As Decimal = 0
        Dim hitungSaldo As String = "SELECT SUM(SALDO_SEBELUMNYA) FROM tbl_datareferensi WHERE SUB_AKUN = @subAkun"

        Using cmdHitungSaldo As New MySqlCommand(hitungSaldo, conn)
            cmdHitungSaldo.Parameters.AddWithValue("@subAkun", subAkun)
            Dim result As Object = cmdHitungSaldo.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(result.ToString()) Then
                totalSaldo = CDec(result)
            End If
        End Using

        Return totalSaldo
    End Function

    ' Fungsi untuk memperbarui SALDO_AWAL
    Public Sub PerbaruiSaldoAwal(ByVal typeAkun As String, ByVal saldo As Decimal, ByVal conn As MySqlConnection)
        Dim perbaruiSaldo As String = "UPDATE tbl_datareferensi SET SALDO_SEBELUMNYA = @Saldo WHERE TYPE_AKUN = @TypeAkun"

        Using cmdPerbaruiSaldo As New MySqlCommand(perbaruiSaldo, conn)
            cmdPerbaruiSaldo.Parameters.AddWithValue("@Saldo", saldo)
            cmdPerbaruiSaldo.Parameters.AddWithValue("@TypeAkun", typeAkun)
            cmdPerbaruiSaldo.ExecuteNonQuery()
        End Using
    End Sub

    ' Fungsi untuk menghitung total Debet
    Public Function HitungTotalDebet(ByVal subAkun As String, ByVal conn As MySqlConnection) As Decimal
        Dim totalDebet As Decimal = 0
        Dim hitungDebet As String = "SELECT SUM(S_DEBET) FROM tbl_datareferensi WHERE SUB_AKUN = @subAkun"

        Using cmdHitungDebet As New MySqlCommand(hitungDebet, conn)
            cmdHitungDebet.Parameters.AddWithValue("@subAkun", subAkun)
            Dim result As Object = cmdHitungDebet.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(result.ToString()) Then
                totalDebet = CDec(result)
            End If
        End Using

        Return totalDebet
    End Function

    ' Fungsi untuk menghitung total Kredit
    Public Function HitungTotalKredit(ByVal subAkun As String, ByVal conn As MySqlConnection) As Decimal
        Dim totalKredit As Decimal = 0
        Dim hitungKredit As String = "SELECT SUM(S_KREDIT) FROM tbl_datareferensi WHERE SUB_AKUN = @subAkun"

        Using cmdHitungKredit As New MySqlCommand(hitungKredit, conn)
            cmdHitungKredit.Parameters.AddWithValue("@subAkun", subAkun)
            Dim result As Object = cmdHitungKredit.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(result.ToString()) Then
                totalKredit = CDec(result)
            End If
        End Using

        Return totalKredit
    End Function

    ' Sub untuk memperbarui Debet dan Kredit
    Public Sub PerbaruiDebetKredit(ByVal typeAkun As String, ByVal debet As Decimal, ByVal kredit As Decimal, ByVal conn As MySqlConnection)
        Dim perbaruiDebetKredit As String = "UPDATE tbl_datareferensi SET S_DEBET = @Debet, S_KREDIT = @Kredit WHERE TYPE_AKUN = @TypeAkun"

        Using cmdPerbaruiDebetKredit As New MySqlCommand(perbaruiDebetKredit, conn)
            cmdPerbaruiDebetKredit.Parameters.AddWithValue("@Debet", debet)
            cmdPerbaruiDebetKredit.Parameters.AddWithValue("@Kredit", kredit)
            cmdPerbaruiDebetKredit.Parameters.AddWithValue("@TypeAkun", typeAkun)
            cmdPerbaruiDebetKredit.ExecuteNonQuery()
        End Using
    End Sub







    Public Sub HITUNGSALDOAWAL(ByVal tanggalAwal As Date)

        ' 1===================> Gabungkan penghitungan total DEBET dan KREDIT untuk setiap KODE_AKUN
        Dim hitungTotal As String = "SELECT tbl_datareferensi.KODE_AKUN, " &
                            "SUM(CASE WHEN JurnalUmum.TGL_TRANSAKSI < @TANGGAL_AWAL AND JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_DEBET, " &
                            "SUM(CASE WHEN JurnalUmum.TGL_TRANSAKSI < @TANGGAL_AWAL AND JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_KREDIT " &
                            "FROM tbl_datareferensi " &
                            "LEFT JOIN JurnalUmum ON JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN OR JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN " &
                            "GROUP BY tbl_datareferensi.KODE_AKUN"


        ' Membuat dictionary untuk menyimpan total DEBET dan KREDIT per KODE_AKUN
        Dim hasilPerKodeAkun As New Dictionary(Of String, Tuple(Of Decimal, Decimal))()

        Using cmdHitungTotal As New MySqlCommand(hitungTotal, conn)
            cmdHitungTotal.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmdHitungTotal.ExecuteReader()
                While rd.Read()
                    Dim kodeAkun As String = rd("KODE_AKUN").ToString()
                    Dim totalDebet As Decimal = If(Not IsDBNull(rd("TOTAL_DEBET")), CDec(rd("TOTAL_DEBET")), 0)
                    Dim totalKredit As Decimal = If(Not IsDBNull(rd("TOTAL_KREDIT")), CDec(rd("TOTAL_KREDIT")), 0)

                    ' Simpan hasil ke dictionary menggunakan Tuple untuk VB.NET 2010
                    hasilPerKodeAkun(kodeAkun) = Tuple.Create(totalDebet, totalKredit)
                End While
            End Using
        End Using


        ' 2===================> Update kolom SALDO_SEBELUMNYA berdasarkan hasil dictionary
        For Each item As KeyValuePair(Of String, Tuple(Of Decimal, Decimal)) In hasilPerKodeAkun
            Dim kodeAkun As String = item.Key
            Dim totalDebet As Decimal = item.Value.Item1
            Dim totalKredit As Decimal = item.Value.Item2

            ' Query untuk update SALDO_SEBELUMNYA
            Dim hitungSaldo As String = "UPDATE tbl_datareferensi " &
                             "SET SALDO_SEBELUMNYA = CASE " &
                             "WHEN AKUN_DK = 'DEBET' THEN SALDO_AWAL + @TotalDebet - @TotalKredit " &
                             "WHEN AKUN_DK = 'KREDIT' THEN SALDO_AWAL - @TotalDebet + @TotalKredit " &
                             "ELSE 0 END " &
                             "WHERE KODE_AKUN = @KodeAkun AND TYPE_AKUN <> 'LABA RUGI'"


            Using cmd As New MySqlCommand(hitungSaldo, conn)
                cmd.Parameters.AddWithValue("@TotalDebet", totalDebet)
                cmd.Parameters.AddWithValue("@TotalKredit", totalKredit)
                cmd.Parameters.AddWithValue("@KodeAkun", kodeAkun)
                cmd.ExecuteNonQuery()
            End Using
        Next




        ' 3===================> Hitung total Debet, dan Kredit untuk SUB_AKUN 'LABA' dan 'RUGI'

        ' Hitung Debet untuk Laba
        Dim debetLabaRugi As Decimal
        Dim queryDebet As String = "SELECT SUM(SALDO_SEBELUMNYA) FROM tbl_datareferensi WHERE SUB_AKUN = 'LABA'"
        Using cmdDebet As New MySqlCommand(queryDebet, conn)
            Dim result As Object = cmdDebet.ExecuteScalar()
            debetLabaRugi = If(result Is DBNull.Value, 0, Convert.ToDecimal(result))
        End Using

        ' Hitung Kredit untuk Rugi
        Dim kreditLabaRugi As Decimal
        Dim queryKredit As String = "SELECT SUM(SALDO_SEBELUMNYA) FROM tbl_datareferensi WHERE SUB_AKUN = 'RUGI'"
        Using cmdKredit As New MySqlCommand(queryKredit, conn)
            Dim result As Object = cmdKredit.ExecuteScalar()
            kreditLabaRugi = If(result Is DBNull.Value, 0, Convert.ToDecimal(result))
        End Using


        ' 4===================> Update kolom Saldo Akhir laba rugi dengan menambahkan Saldo Awal, Debet, dan mengurangkan Kredit jika kode akun sama hanya pada laba rugi
        Dim hitungSaldolabarugi As String = "UPDATE tbl_datareferensi " &
                                            "SET SALDO_SEBELUMNYA = @DebetLabaRugi - @KreditLabaRugi " &
                                            "WHERE TYPE_AKUN = 'LABA RUGI'"

        Using cmd As New MySqlCommand(hitungSaldolabarugi, conn)
            ' Tambahkan parameter untuk Debet dan Kredit
            cmd.Parameters.AddWithValue("@DebetLabaRugi", debetLabaRugi)
            cmd.Parameters.AddWithValue("@KreditLabaRugi", kreditLabaRugi)

            ' Eksekusi perintah SQL
            cmd.ExecuteNonQuery()
        End Using


    End Sub

    Private Sub HITUNGDEBETKREDIT(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        ' Gabungkan penghitungan total DEBET dan KREDIT untuk setiap KODE_AKUN
        Dim hitungTotal As String = "SELECT tbl_datareferensi.KODE_AKUN, " &
                            "SUM(CASE WHEN JurnalUmum.TGL_TRANSAKSI >= @TANGGAL_AWAL AND JurnalUmum.TGL_TRANSAKSI <= @TANGGAL_AKHIR AND JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_DEBET, " &
                            "SUM(CASE WHEN JurnalUmum.TGL_TRANSAKSI >= @TANGGAL_AWAL AND JurnalUmum.TGL_TRANSAKSI <= @TANGGAL_AKHIR AND JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_KREDIT " &
                            "FROM tbl_datareferensi " &
                            "LEFT JOIN JurnalUmum ON (JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN OR JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN) " &
                            "GROUP BY tbl_datareferensi.KODE_AKUN"

        ' List untuk menyimpan hasil dalam bentuk dictionary
        Dim updateDataList As New List(Of Dictionary(Of String, Object))()

        Using cmdHitungTotal As New MySqlCommand(hitungTotal, conn)
            cmdHitungTotal.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalawal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungTotal.Parameters.AddWithValue("@TANGGAL_AKHIR", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmdHitungTotal.ExecuteReader()
                While rd.Read()
                    Dim kodeAkun As String = rd("KODE_AKUN").ToString()
                    Dim totalDebet As Decimal = If(Not IsDBNull(rd("TOTAL_DEBET")), CDec(rd("TOTAL_DEBET")), 0)
                    Dim totalKredit As Decimal = If(Not IsDBNull(rd("TOTAL_KREDIT")), CDec(rd("TOTAL_KREDIT")), 0)

                    ' Menambahkan hasil dalam dictionary dan memasukkan ke dalam list
                    Dim updateData As New Dictionary(Of String, Object)()
                    updateData("KODE_AKUN") = kodeAkun
                    updateData("TOTAL_DEBET") = totalDebet
                    updateData("TOTAL_KREDIT") = totalKredit

                    updateDataList.Add(updateData)
                End While
            End Using
        End Using

        ' Melakukan update ke database menggunakan data yang sudah disiapkan di list
        For Each updateData As Dictionary(Of String, Object) In updateDataList
            Dim kodeAkun As String = updateData("KODE_AKUN").ToString()
            Dim totalDebet As Decimal = CDec(updateData("TOTAL_DEBET"))
            Dim totalKredit As Decimal = CDec(updateData("TOTAL_KREDIT"))

            ' Update kolom DEBET dan KREDIT di tabel tbl_datareferensi
            Dim updateTotal As String = "UPDATE tbl_datareferensi SET S_DEBET = @TOTAL_DEBET, S_KREDIT = @TOTAL_KREDIT WHERE KODE_AKUN = @KODE_AKUN"
            Using cmdUpdateTotal As New MySqlCommand(updateTotal, conn)
                cmdUpdateTotal.Parameters.AddWithValue("@TOTAL_DEBET", totalDebet)
                cmdUpdateTotal.Parameters.AddWithValue("@TOTAL_KREDIT", totalKredit)
                cmdUpdateTotal.Parameters.AddWithValue("@KODE_AKUN", kodeAkun)
                cmdUpdateTotal.ExecuteNonQuery()
            End Using
        Next


        ' 3===================> Hitung S_DEBET untuk SUB_AKUN 'LABA' dan 'RUGI'
        Dim SaldoDebet As Decimal = 0

        ' Hitung Saldo Debet untuk SUB_AKUN 'RUGI' dan 'LABA'
        Dim queryTambahDebet As String = "SELECT SUM(S_DEBET) FROM tbl_datareferensi WHERE SUB_AKUN = 'RUGI'"
        Dim queryKurangDebet As String = "SELECT SUM(S_DEBET) FROM tbl_datareferensi WHERE SUB_AKUN = 'LABA'"

        ' Hitung nilai tambah Debet
        Using cmdTambahDebet As New MySqlCommand(queryTambahDebet, conn)
            Dim tambahResultDebet As Object = cmdTambahDebet.ExecuteScalar()
            Dim nilaiTambahDebet As Decimal = If(IsDBNull(tambahResultDebet), 0D, Convert.ToDecimal(tambahResultDebet))

            ' Hitung nilai kurang Debet
            Using cmdKurangDebet As New MySqlCommand(queryKurangDebet, conn)
                Dim kurangResultDebet As Object = cmdKurangDebet.ExecuteScalar()
                Dim nilaiKurangDebet As Decimal = If(IsDBNull(kurangResultDebet), 0D, Convert.ToDecimal(kurangResultDebet))

                ' Hitung saldo Debet
                SaldoDebet = nilaiTambahDebet - nilaiKurangDebet
            End Using
        End Using

        ' Update S_DEBET di tabel tbl_datareferensi
        Dim updateQueryDebet As String = "UPDATE tbl_datareferensi SET S_DEBET = ? WHERE TYPE_AKUN = 'LABA RUGI'"
        Using cmdUpdateDebet As New MySqlCommand(updateQueryDebet, conn)
            cmdUpdateDebet.Parameters.AddWithValue("?", SaldoDebet)
            cmdUpdateDebet.ExecuteNonQuery()
        End Using

        ' 4===================> Hitung S_KREDIT untuk SUB_AKUN 'LABA' dan 'RUGI'
        Dim SaldoKredit As Decimal = 0

        ' Hitung Saldo Kredit untuk SUB_AKUN 'LABA' dan 'RUGI'
        Dim queryTambahKredit As String = "SELECT SUM(S_KREDIT) FROM tbl_datareferensi WHERE SUB_AKUN = 'LABA'"
        Dim queryKurangKredit As String = "SELECT SUM(S_KREDIT) FROM tbl_datareferensi WHERE SUB_AKUN = 'RUGI'"

        ' Hitung nilai tambah Kredit
        Using cmdTambahKredit As New MySqlCommand(queryTambahKredit, conn)
            Dim tambahResultKredit As Object = cmdTambahKredit.ExecuteScalar()
            Dim nilaiTambahKredit As Decimal = If(IsDBNull(tambahResultKredit), 0D, Convert.ToDecimal(tambahResultKredit))

            ' Hitung nilai kurang Kredit
            Using cmdKurangKredit As New MySqlCommand(queryKurangKredit, conn)
                Dim kurangResultKredit As Object = cmdKurangKredit.ExecuteScalar()
                Dim nilaiKurangKredit As Decimal = If(IsDBNull(kurangResultKredit), 0D, Convert.ToDecimal(kurangResultKredit))

                ' Hitung saldo Kredit
                SaldoKredit = nilaiTambahKredit - nilaiKurangKredit
            End Using
        End Using

        ' Update S_KREDIT di tabel tbl_datareferensi
        Dim updateQueryKredit As String = "UPDATE tbl_datareferensi SET S_KREDIT = ? WHERE TYPE_AKUN = 'LABA RUGI'"
        Using cmdUpdateKredit As New MySqlCommand(updateQueryKredit, conn)
            cmdUpdateKredit.Parameters.AddWithValue("?", SaldoKredit)
            cmdUpdateKredit.ExecuteNonQuery()
        End Using

    End Sub

    Public Sub HITUNGSALDOAKHIR(ByVal tanggalakhir As Date)

        ' 1===================> Gabungkan penghitungan total DEBET dan KREDIT untuk setiap KODE_AKUN
        Dim hitungTotal As String = "SELECT tbl_datareferensi.KODE_AKUN, " &
                            "SUM(CASE WHEN JurnalUmum.TGL_TRANSAKSI <= @TANGGA_AKHIR AND JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_DEBET, " &
                            "SUM(CASE WHEN JurnalUmum.TGL_TRANSAKSI <= @TANGGA_AKHIR AND JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN THEN JurnalUmum.NOMINAL ELSE 0 END) AS TOTAL_KREDIT " &
                            "FROM tbl_datareferensi " &
                            "LEFT JOIN JurnalUmum ON (JurnalUmum.NOMOR_AKUN_D = tbl_datareferensi.KODE_AKUN OR JurnalUmum.NOMOR_AKUN_K = tbl_datareferensi.KODE_AKUN) " &
                            "GROUP BY tbl_datareferensi.KODE_AKUN"


        ' Membuat dictionary untuk menyimpan total DEBET dan KREDIT per KODE_AKUN
        Dim hasilPerKodeAkun As New Dictionary(Of String, Tuple(Of Decimal, Decimal))()

        Using cmdHitungTotal As New MySqlCommand(hitungTotal, conn)
            cmdHitungTotal.Parameters.AddWithValue("@TANGGA_AKHIR", tanggalakhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmdHitungTotal.ExecuteReader()
                While rd.Read()
                    Dim kodeAkun As String = rd("KODE_AKUN").ToString()
                    Dim totalDebet As Decimal = If(Not IsDBNull(rd("TOTAL_DEBET")), CDec(rd("TOTAL_DEBET")), 0)
                    Dim totalKredit As Decimal = If(Not IsDBNull(rd("TOTAL_KREDIT")), CDec(rd("TOTAL_KREDIT")), 0)

                    ' Simpan hasil ke dictionary menggunakan Tuple untuk VB.NET 2010
                    hasilPerKodeAkun(kodeAkun) = Tuple.Create(totalDebet, totalKredit)
                End While
            End Using
        End Using


        ' 2===================> Update kolom SALDO_SEBELUMNYA berdasarkan hasil dictionary
        For Each item As KeyValuePair(Of String, Tuple(Of Decimal, Decimal)) In hasilPerKodeAkun
            Dim kodeAkun As String = item.Key
            Dim totalDebet As Decimal = item.Value.Item1
            Dim totalKredit As Decimal = item.Value.Item2

            ' Query untuk update SALDO_SEBELUMNYA
            Dim hitungSaldo As String = "UPDATE tbl_datareferensi " &
                             "SET SALDO_AKHIR = CASE " &
                             "WHEN AKUN_DK = 'DEBET' THEN SALDO_AWAL + @TotalDebet - @TotalKredit " &
                             "WHEN AKUN_DK = 'KREDIT' THEN SALDO_AWAL - @TotalDebet + @TotalKredit " &
                             "ELSE 0 END " &
                             "WHERE KODE_AKUN = @KodeAkun AND TYPE_AKUN <> 'LABA RUGI'"


            Using cmd As New MySqlCommand(hitungSaldo, conn)
                cmd.Parameters.AddWithValue("@TotalDebet", totalDebet)
                cmd.Parameters.AddWithValue("@TotalKredit", totalKredit)
                cmd.Parameters.AddWithValue("@KodeAkun", kodeAkun)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 3===================> Hitung total Debet, dan Kredit untuk SUB_AKUN 'LABA' dan 'RUGI'

        ' Hitung Debet untuk Laba
        Dim debetLabaRugi As Decimal
        Dim queryDebet As String = "SELECT SUM(SALDO_AKHIR) FROM tbl_datareferensi WHERE SUB_AKUN = 'LABA'"
        Using cmdDebet As New MySqlCommand(queryDebet, conn)
            Dim result As Object = cmdDebet.ExecuteScalar()
            debetLabaRugi = If(result Is DBNull.Value, 0, Convert.ToDecimal(result))
        End Using

        ' Hitung Kredit untuk Rugi
        Dim kreditLabaRugi As Decimal
        Dim queryKredit As String = "SELECT SUM(SALDO_AKHIR) FROM tbl_datareferensi WHERE SUB_AKUN = 'RUGI'"
        Using cmdKredit As New MySqlCommand(queryKredit, conn)
            Dim result As Object = cmdKredit.ExecuteScalar()
            kreditLabaRugi = If(result Is DBNull.Value, 0, Convert.ToDecimal(result))
        End Using


        ' 4===================> Update kolom Saldo Akhir laba rugi dengan menambahkan Saldo Awal, Debet, dan mengurangkan Kredit jika kode akun sama hanya pada laba rugi
        Dim hitungSaldolabarugi As String = "UPDATE tbl_datareferensi " &
                                            "SET SALDO_AKHIR = @DebetLabaRugi - @KreditLabaRugi " &
                                            "WHERE TYPE_AKUN = 'LABA RUGI'"

        Using cmd As New MySqlCommand(hitungSaldolabarugi, conn)
            ' Tambahkan parameter untuk Debet dan Kredit
            cmd.Parameters.AddWithValue("@DebetLabaRugi", debetLabaRugi)
            cmd.Parameters.AddWithValue("@KreditLabaRugi", kreditLabaRugi)

            ' Eksekusi perintah SQL
            cmd.ExecuteNonQuery()
        End Using



    End Sub





    '---------------------------------------- TabLapNeraca --------------------------------------------------------------------------------------
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
        CmbBln.Items.Clear()

        ' Tambahkan tahun dari 2022 hingga tahun sekarang
        For i As Integer = 2022 To Year(Now)
            CmbThn.Items.Add(i)
        Next

        ' Tambahkan daftar bulan
        Dim daftarBulan As String() = {"Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember"}
        CmbBln.Items.AddRange(daftarBulan)

        ' Set tahun sekarang sebagai tahun default
        CmbThn.SelectedItem = Year(Now)

        ' Set bulan sekarang sebagai bulan default
        CmbBln.SelectedIndex = Month(Now) - 1 ' Bulan dalam VB.NET berbasis 1, jadi kurangi 1 untuk mendapatkan indeks yang benar
    End Sub

    Dim Periode As String
    Private Sub BtnTampilNeraca_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTampilNeraca.Click
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()

        If RbtSemua.Checked Then
            Dim tanggalAwal As Date
            Dim tanggalAkhir As Date
            ' Ambil tanggal dari JurnalUmum
            Dim queryTanggal As String = "SELECT MIN(TGL_TRANSAKSI) AS TANGGAL_AWAL, MAX(TGL_TRANSAKSI) AS TANGGAL_AKHIR FROM JurnalUmum"

            Using cmd As New MySqlCommand(queryTanggal, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        tanggalAwal = If(IsDBNull(reader("TANGGAL_AWAL")), Date.MinValue, Convert.ToDateTime(reader("TANGGAL_AWAL")))
                        tanggalAkhir = If(IsDBNull(reader("TANGGAL_AKHIR")), Date.MinValue, Convert.ToDateTime(reader("TANGGAL_AKHIR")))
                    End If
                End Using
            End Using


            Periode = "Periode : " & tanggalAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " - " & tanggalAkhir.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))
            HITUNGSEMUASALDO()
        Else
            Dim tanggalAwal As Date
            Dim tanggalAkhir As Date

            If RbtTanggal.Checked Then
                tanggalAwal = DtpAwal.Value.Date
                tanggalAkhir = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)
            ElseIf RbtBulan.Checked Then
                KonversiBulanKeAngka()
                Dim bulan As Integer = bulanTerpilih
                Dim tahun As Integer = CmbThn.Text
                tanggalAwal = New DateTime(tahun, bulan, 1)
                tanggalAkhir = tanggalAwal.AddMonths(1).AddSeconds(-1)
            End If
            Periode = "Periode : " & tanggalAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " - " & tanggalAkhir.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))
            HITUNGSALDOAWAL(tanggalAwal)
            HITUNGDEBETKREDIT(tanggalAwal, tanggalAkhir)
            HITUNGSALDOAKHIR(tanggalAkhir)
        End If

        Tampilnerca()
        TampilkanLabaRugi()
        TampilNeracaLajur()
    End Sub


    Private Sub Tampilnerca()
        ReportViewer1.LocalReport.DataSources.Clear()

        ' Daftar query dan parameter untuk setiap jenis akun
        Dim queries As New Dictionary(Of String, String) From {
            {"DataSet1", "ASET LANCAR"},
            {"DataSet2", "ASET TETAP"},
            {"DataSet3", "PASIVA"},
            {"DataSet4", "MODAL"}
        }

        ' Eksekusi query untuk setiap jenis akun dan tambahkan ke data source laporan
        For Each kvp As KeyValuePair(Of String, String) In queries
            Dim query As String = "SELECT KODE_AKUN, NAMA_AKUN, SALDO_SEBELUMNYA, (SALDO_AKHIR - SALDO_SEBELUMNYA) AS Perubahan, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN LIKE @JENIS_AKUN ORDER BY KODE_AKUN"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@JENIS_AKUN", kvp.Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Dim dataset As New DataSet()
                    dataset.Load(rd, LoadOption.OverwriteChanges, kvp.Key)
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource(kvp.Key, dataset.Tables(kvp.Key)))
                End Using
            End Using
        Next


        Dim AWAL As String = String.Empty
        Dim RUBAH As String = String.Empty
        Dim AKHIR As String = "SALDO AKHIR"

        Select Case True
            Case RbtSemua.Checked
                AWAL = "SALDO AWAL"
                RUBAH = "PERIODE INI"
            Case RbtTanggal.Checked
                AWAL = "PERIODE LALU"
                RUBAH = "PERIODE INI"
            Case RbtBulan.Checked
                AWAL = "BULAN LALU"
                RUBAH = "BULAN INI"
        End Select

        ' Set parameter laporan
        Dim parameters As ReportParameter() = {
            New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
            New ReportParameter("PERIODE", Periode),
            New ReportParameter("AWAL", AWAL),
            New ReportParameter("RUBAH", RUBAH),
            New ReportParameter("AKHIR", AKHIR),
            New ReportParameter("USER", "Dicetak oleh : " & FormUtama.SLogin.Text)
        }

        ReportViewer1.LocalReport.SetParameters(parameters)
        ReportViewer1.RefreshReport()
    End Sub

    '---------------------------------------- TabLapLabaRugi ------------------------------------------------------------------------------------


    Private Sub TampilkanLabaRugi()
        ' Membersihkan sumber data laporan sebelum menambahkan yang baru
        ReportViewer2.LocalReport.DataSources.Clear()

        ' Daftar query dan nama dataset untuk masing-masing jenis akun
        Dim accountTypeQueries As New Dictionary(Of String, String)() From {
            {"DataSet1", "SELECT KODE_AKUN, NAMA_AKUN, SALDO_SEBELUMNYA, (SALDO_AKHIR - SALDO_SEBELUMNYA) AS Perubahan, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN = 'HPP' ORDER BY KODE_AKUN"},
            {"DataSet2", "SELECT KODE_AKUN, NAMA_AKUN, SALDO_SEBELUMNYA, (SALDO_AKHIR - SALDO_SEBELUMNYA) AS Perubahan, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN = 'BIAYA' ORDER BY KODE_AKUN"},
            {"DataSet3", "SELECT KODE_AKUN, NAMA_AKUN, SALDO_SEBELUMNYA, (SALDO_AKHIR - SALDO_SEBELUMNYA) AS Perubahan, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN = 'PENDAPATAN LAIN' ORDER BY KODE_AKUN"},
            {"DataSet4", "SELECT KODE_AKUN, NAMA_AKUN, SALDO_SEBELUMNYA, (SALDO_AKHIR - SALDO_SEBELUMNYA) AS Perubahan, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN = 'PAJAK' ORDER BY KODE_AKUN"}
        }

        ' Menggunakan satu dataset untuk mengurangi penggunaan memori
        Dim dataset As New DataSet()

        ' Loop melalui query dan tambahkan data ke dataset
        For Each accountTypeQuery As KeyValuePair(Of String, String) In accountTypeQueries
            Dim query As String = accountTypeQuery.Value
            Dim datasetName As String = accountTypeQuery.Key

            Using cmd As New MySqlCommand(query, conn)
                Using adapter As New MySqlDataAdapter(cmd)
                    adapter.Fill(dataset, datasetName)
                End Using
            End Using

            ' Tambahkan data ke ReportViewer
            ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource(datasetName, dataset.Tables(datasetName)))
        Next

        Dim AWAL As String = String.Empty
        Dim RUBAH As String = String.Empty
        Dim AKHIR As String = "SALDO AKHIR"

        Select Case True
            Case RbtSemua.Checked
                AWAL = "SALDO AWAL"
                RUBAH = "PERIODE INI"
            Case RbtTanggal.Checked
                AWAL = "TANGGAL LALU"
                RUBAH = "TANGGAL INI"
            Case RbtBulan.Checked
                AWAL = "BULAN LALU"
                RUBAH = "BULAN INI"
        End Select

        ' Set parameter laporan
        Dim parameters As ReportParameter() = {
            New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
            New ReportParameter("PERIODE", Periode),
            New ReportParameter("AWAL", AWAL),
            New ReportParameter("RUBAH", RUBAH),
            New ReportParameter("AKHIR", AKHIR),
            New ReportParameter("USER", "Dicetak oleh : " & FormUtama.SLogin.Text)
        }

        ReportViewer2.LocalReport.SetParameters(parameters)
        ReportViewer2.RefreshReport()
    End Sub


    Private Sub TampilNeracaLajur()
        ReportViewer3.LocalReport.DataSources.Clear()
        Dim query As String = "SELECT KODE_AKUN, NAMA_AKUN, AKUN_NRLR, SALDO_SEBELUMNYA, S_DEBET, S_KREDIT, SALDO_AKHIR " &
                              "FROM tbl_datareferensi " &
                              "ORDER BY KODE_AKUN"

        Using command As New MySqlCommand(query, conn)

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dataset As New DataSetKL()
                dataset.Load(reader, LoadOption.OverwriteChanges, "NeracaLajur")

                ReportViewer3.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("NeracaLajur")))
                ' Set parameter laporan
                Dim parameters As ReportParameter() = {
                    New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
                     New ReportParameter("PERIODE", Periode)
                }

                ReportViewer3.LocalReport.SetParameters(parameters)
                ReportViewer3.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub FormLaporan_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        ReportViewer3.LocalReport.DataSources.Clear()
    End Sub


End Class