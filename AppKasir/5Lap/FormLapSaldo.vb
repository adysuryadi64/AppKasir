Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO

Public Class FormLapSaldo

    Private Sub FormLapSaldo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Isi ComboBox dengan data dari list
        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")

        CmbRekening.SelectedIndex = 0
        CbTanggal.Checked = True
        Ambildataprinter()
        Ambildataperusahaan()
        PanelView.Visible = False
    End Sub

    Private Sub CbTanggal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked = True Then
            DtpTanggal.Enabled = True
            CbBulan.Checked = False
            CmbBln.Enabled = False
            CmbThn.Enabled = False
            CmbBln.Text = ""
            CmbThn.Text = ""
            CmbKasir.SelectedIndex = 0
            TampilKasirTanggal()
        Else
            CbBulan.Checked = True
        End If
    End Sub

    Private Sub DtpTanggal_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DtpTanggal.ValueChanged
        TampilKasirTanggal()
    End Sub

    Public Sub TampilKasirTanggal()
        CmbKasir.Items.Clear()
        CmbKasir.Items.Add("Semua") ' Tambahkan opsi "Semua"

        Dim tanggalAwal As Date = DtpTanggal.Value.Date
        Dim tanggalAkhir As Date = DtpTanggal.Value.Date.AddDays(1).AddTicks(-1)

        Dim query As String = "SELECT DISTINCT ID_USER FROM JurnalUmum WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR ORDER BY ID_USER"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbKasir.Items.Add(rd("ID_USER").ToString())
                End While
            End Using
        End Using

        CmbKasir.SelectedIndex = 0
    End Sub


    Private Sub CbBulan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked = True Then
            Tampildatablnthn()
            CmbBln.Enabled = True
            CmbThn.Enabled = True
            CmbThn.Text = Now.ToString("yyyy")
            CbTanggal.Checked = False
            CmbKasir.SelectedIndex = 0
            DtpTanggal.Enabled = False
        Else
            CmbBln.Enabled = False
            CmbThn.Enabled = False
            CmbBln.Text = ""
            CmbThn.Text = ""
            CbTanggal.Checked = True
        End If
    End Sub

    Private Sub CmbBln_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbBln.SelectedIndexChanged
        TampilKasirBulan()
    End Sub

    Private Sub CmbThn_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbThn.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            TampilKasirBulan()
        End If
    End Sub

    Private angkabulan As Integer

    Private Sub Convertbulansetor()
        angkabulan = CmbBln.SelectedIndex + 1
    End Sub

    Public Sub Tampildatablnthn()
        CmbBln.Items.Clear()
        CmbThn.Items.Clear()

        ' Tambah tahun dari 2022 sampai tahun sekarang
        For i As Integer = 2022 To Year(Now)
            CmbThn.Items.Add(i)
        Next

        ' Tambah nama-nama bulan
        Dim namaBulan() As String = {
        "Januari", "Februari", "Maret", "April", "Mei", "Juni",
        "Juli", "Agustus", "September", "Oktober", "November", "Desember"
    }

        CmbBln.Items.AddRange(namaBulan)
    End Sub


    Public Sub TampilKasirBulan()
        CmbKasir.Items.Clear()
        CmbKasir.Items.Add("Semua") ' Tambahkan opsi "Semua"

        Convertbulansetor()
        Dim bulan As Integer = angkabulan
        Dim tahun As Integer = CmbThn.Text

        Dim AwalBulan As New Date(tahun, bulan, 1)
        Dim AkhirBulan As Date = AwalBulan.AddMonths(1).AddDays(-1).AddSeconds(86399)

        Dim query As String = "SELECT DISTINCT ID_USER FROM JurnalUmum WHERE TGL_TRANSAKSI >= @AWAL_BULAN AND TGL_TRANSAKSI <= @AKHIR_BULAN ORDER BY ID_USER"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@AWAL_BULAN", AwalBulan.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AKHIR_BULAN", AkhirBulan.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbKasir.Items.Add(rd("ID_USER").ToString())
                End While
            End Using
        End Using

        CmbKasir.SelectedIndex = 0
    End Sub


    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim namaAkunD As String = CmbRekening.Text
        Dim sql As String = "SELECT Kode_akun, Type_Akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtRekening.Text = reader("Kode_akun").ToString()
                    TxtTypeAkun.Text = reader("Type_Akun").ToString()
                End If
            End Using
        End Using
    End Sub


    Private Sub BtnHitung_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHitung.Click
        If Not (CbTanggal.Checked Or CbBulan.Checked) Then
            MessageBox.Show("Pilih dulu berdasarkan tanggal atau bulan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Cursor = Cursors.WaitCursor
        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalsaldoawal As Date

        If CbTanggal.Checked = True Then
            tanggalAwal = DtpTanggal.Value.Date
            tanggalAkhir = tanggalAwal.AddDays(1).AddTicks(-1)
            tanggalsaldoawal = DtpTanggal.Value.Date
        ElseIf CbBulan.Checked = True Then
            Convertbulansetor()
            Dim bulan As Integer = angkabulan
            Dim tahun As Integer = CmbThn.Text
            tanggalAwal = New DateTime(tahun, bulan, 1)
            tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1).AddSeconds(86399)
            tanggalsaldoawal = tanggalAwal.AddDays(-1).AddSeconds(86399)
        End If

        ' metode cepat
        LoadRekapSekaliBaca(tanggalAwal, tanggalAkhir, kasir)

        Dim saldoAwal As Decimal = HitungSaldoAwalSekaliBaca(TxtRekening.Text, kasir, tanggalsaldoawal)
        TxtSaldoAwal.Text = saldoAwal.ToString("N0", cultureIndonesia)

        'metode lama
        'ExecuteQuery("Pembelian", "NOMOR_AKUN_K", {"Pembelian"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Penjualan", "NOMOR_AKUN_D", {"Penjualan"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Retur Pembelian", "NOMOR_AKUN_D", {"Retur Pembelian"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Retur Penjualan", "NOMOR_AKUN_K", {"Retur Penjualan"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Bayar Hutang", "NOMOR_AKUN_K", {"Bayar Hutang"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Bayar Piutang", "NOMOR_AKUN_D", {"Bayar Piutang"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Pemasukan", "NOMOR_AKUN_D", {"Pemasukan", "Bayar bon"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Pengeluaran", "NOMOR_AKUN_K", {"Pengeluaran", "Bon", "Gaji"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Biaya", "NOMOR_AKUN_K", {"Biaya"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("RekeningDebet", "NOMOR_AKUN_D", {"PINDAH REKENING"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("RekeningKredit", "NOMOR_AKUN_K", {"PINDAH REKENING"}, kasir, tanggalAwal, tanggalAkhir)
        'ExecuteQuery("Setor Bos", "NOMOR_AKUN_K", {"SETOR KE BOS"}, kasir, tanggalAwal, tanggalAkhir)

        'Dim saldoAwal As Decimal = HitungSaldoAwal(TxtRekening.Text, kasir, tanggalsaldoawal)
        'TxtSaldoAwal.Text = saldoAwal.ToString("N0", cultureIndonesia)
        Cursor = Cursors.Default
    End Sub


    Private Sub LoadRekapSekaliBaca(tglAwal As Date, tglAkhir As Date, kasir As String)

        Dim sql As String =
"SELECT
-- PEMBELIAN
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Pembelian' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS PembelianTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Pembelian' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS PembelianNota,

-- PENJUALAN
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Penjualan' AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS PenjualanTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Penjualan' AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS PenjualanNota,

-- RETUR BELI
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Retur Pembelian' AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS ReturBeliTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Retur Pembelian' AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS ReturBeliNota,

-- RETUR JUAL
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Retur Penjualan' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS ReturJualTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Retur Penjualan' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS ReturJualNota,

-- BAYAR HUTANG
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Bayar Hutang' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS BayarHutangTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Bayar Hutang' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS BayarHutangNota,

-- BAYAR PIUTANG
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Bayar Piutang' AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS BayarPiutangTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Bayar Piutang' AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS BayarPiutangNota,

-- PEMASUKAN
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI IN ('Pemasukan','Bayar bon') AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS PemasukanTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI IN ('Pemasukan','Bayar bon') AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS PemasukanNota,

-- PENGELUARAN
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI IN ('Pengeluaran','Bon','Gaji') AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS PengeluaranTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI IN ('Pengeluaran','Bon','Gaji') AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS PengeluaranNota,

-- BIAYA
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Biaya' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS BiayaTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Biaya' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS BiayaNota,

-- PINDAH REKENING
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='PINDAH REKENING' AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS PRDebetTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='PINDAH REKENING' AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS PRDebetNota,

IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='PINDAH REKENING' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS PRKreditTotal,
IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='PINDAH REKENING' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS PRKreditNota,

-- SETOR BOS
IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='SETOR KE BOS' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS SetorBosTotal

FROM JurnalUmum
WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR
AND ID_USER LIKE @USER;"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@AKUN", TxtRekening.Text)
            cmd.Parameters.AddWithValue("@USER", "%" & kasir & "%")

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtTotalPembelian.Text = Convert.ToDecimal(rd("PembelianTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaPembelian.Text = Convert.ToInt32(rd("PembelianNota")).ToString("N0", cultureIndonesia)

                    TxtTotalPenjualan.Text = Convert.ToDecimal(rd("PenjualanTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaPenjualan.Text = Convert.ToInt32(rd("PenjualanNota")).ToString("N0", cultureIndonesia)

                    TxtTotalReturBeli.Text = Convert.ToDecimal(rd("ReturBeliTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaReturBeli.Text = Convert.ToInt32(rd("ReturBeliNota")).ToString("N0", cultureIndonesia)

                    TxtTotalReturJual.Text = Convert.ToDecimal(rd("ReturJualTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaReturJual.Text = Convert.ToInt32(rd("ReturJualNota")).ToString("N0", cultureIndonesia)

                    TxtTotalBayarHutang.Text = Convert.ToDecimal(rd("BayarHutangTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaBAyarHutang.Text = Convert.ToInt32(rd("BayarHutangNota")).ToString("N0", cultureIndonesia)

                    TxtTotalBayarPiutang.Text = Convert.ToDecimal(rd("BayarPiutangTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaBayarPiutang.Text = Convert.ToInt32(rd("BayarPiutangNota")).ToString("N0", cultureIndonesia)

                    TxtTotalJurnalPemasukan.Text = Convert.ToDecimal(rd("PemasukanTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaJurnalPemasukan.Text = Convert.ToInt32(rd("PemasukanNota")).ToString("N0", cultureIndonesia)

                    TxtTotalJurnalPengeluaran.Text = Convert.ToDecimal(rd("PengeluaranTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaJurnalPengeluaran.Text = Convert.ToInt32(rd("PengeluaranNota")).ToString("N0", cultureIndonesia)

                    TxtTotalJurnalBiaya.Text = Convert.ToDecimal(rd("BiayaTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaJurnalBiaya.Text = Convert.ToInt32(rd("BiayaNota")).ToString("N0", cultureIndonesia)

                    TxtTotalJurnalPR.Text = Convert.ToDecimal(rd("PRDebetTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaJurnalPR.Text = Convert.ToInt32(rd("PRDebetNota")).ToString("N0", cultureIndonesia)

                    TxtTotalJurnalPRK.Text = Convert.ToDecimal(rd("PRKreditTotal")).ToString("N0", cultureIndonesia)
                    TxtNotaJurnalPRK.Text = Convert.ToInt32(rd("PRKreditNota")).ToString("N0", cultureIndonesia)

                    TxtSetorbos.Text = Convert.ToDecimal(rd("SetorBosTotal")).ToString("N0", cultureIndonesia)
                End If
            End Using
        End Using
    End Sub


    Private Function HitungSaldoAwalSekaliBaca(ByVal akun As String, ByVal kasir As String, ByVal batas As Date) As Decimal
        Dim sql As String =
"SELECT
SUM(CASE WHEN NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END) AS Debet,
SUM(CASE WHEN NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END) AS Kredit
FROM JurnalUmum
WHERE TGL_TRANSAKSI <= @BATAS
AND ID_USER LIKE @USER;"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@AKUN", akun)
            cmd.Parameters.AddWithValue("@BATAS", batas.AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@USER", "%" & kasir & "%")

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim d As Decimal = If(IsDBNull(rd("Debet")), 0D, rd("Debet"))
                    Dim k As Decimal = If(IsDBNull(rd("Kredit")), 0D, rd("Kredit"))
                    Return d - k
                End If
            End Using
        End Using

        Return 0
    End Function



    Private Sub ExecuteQuery(ByVal namaTransaksi As String, ByVal akun As String, ByVal jenisTransaksiList As String(), ByVal kasir As String, ByVal tanggalAwal As Date, ByVal tanggalAkhir As Date)
        Dim jenisTransaksiInClause As String = String.Join("', '", jenisTransaksiList)
        jenisTransaksiInClause = "'" & jenisTransaksiInClause & "'"

        Dim query As String = String.Format("SELECT Sum(NOMINAL) as Result FROM JurnalUmum WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR AND {0} = @NOMOR_AKUN AND JENIS_TRANSAKSI IN ({1}) AND ID_USER LIKE @ID_USER", akun, jenisTransaksiInClause)

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NOMOR_AKUN", TxtRekening.Text)
            cmd.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                rd.Read()
                If rd.HasRows Then
                    Dim result As Decimal = If(Not rd.IsDBNull(rd.GetOrdinal("Result")), Convert.ToDecimal(rd("Result")), 0)
                    Select Case namaTransaksi
                        Case "Pembelian"
                            TxtTotalPembelian.Text = result.ToString("N0", cultureIndonesia)
                        Case "Penjualan"
                            TxtTotalPenjualan.Text = result.ToString("N0", cultureIndonesia)
                        Case "Retur Pembelian"
                            TxtTotalReturBeli.Text = result.ToString("N0", cultureIndonesia)
                        Case "Retur Penjualan"
                            TxtTotalReturJual.Text = result.ToString("N0", cultureIndonesia)
                        Case "Bayar Hutang"
                            TxtTotalBayarHutang.Text = result.ToString("N0", cultureIndonesia)
                        Case "Bayar Piutang"
                            TxtTotalBayarPiutang.Text = result.ToString("N0", cultureIndonesia)
                        Case "Pemasukan"
                            TxtTotalJurnalPemasukan.Text = result.ToString("N0", cultureIndonesia)
                        Case "Pengeluaran"
                            TxtTotalJurnalPengeluaran.Text = result.ToString("N0", cultureIndonesia)
                        Case "Biaya"
                            TxtTotalJurnalBiaya.Text = result.ToString("N0", cultureIndonesia)
                        Case "RekeningDebet"
                            TxtTotalJurnalPR.Text = result.ToString("N0", cultureIndonesia)
                        Case "RekeningKredit"
                            TxtTotalJurnalPRK.Text = result.ToString("N0", cultureIndonesia)
                        Case "Setor Bos"
                            TxtSetorbos.Text = result.ToString("N0", cultureIndonesia)
                    End Select
                End If

            End Using
        End Using

        Dim query2 As String = String.Format("SELECT COUNT(*) AS RESULT FROM JurnalUmum WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR AND {0} = @NOMOR_AKUN AND JENIS_TRANSAKSI IN ({1}) AND ID_USER LIKE @ID_USER", akun, jenisTransaksiInClause)

        Using cmd2 As New MySqlCommand(query2, conn)
            cmd2.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd2.Parameters.AddWithValue("@TANGGAL_AKHIR", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd2.Parameters.AddWithValue("@NOMOR_AKUN", TxtRekening.Text)
            cmd2.Parameters.AddWithValue("@ID_USER", String.Format("%{0}%", kasir))

            Using rd2 As MySqlDataReader = cmd2.ExecuteReader()
                rd2.Read()
                If rd2.HasRows Then
                    Dim result2 As Integer = If(Not rd2.IsDBNull(rd2.GetOrdinal("Result")), Convert.ToInt32(rd2("Result")), 0)
                    Select Case namaTransaksi
                        Case "Pembelian"
                            TxtNotaPembelian.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Penjualan"
                            TxtNotaPenjualan.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Retur Pembelian"
                            TxtNotaReturBeli.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Retur Penjualan"
                            TxtNotaReturJual.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Bayar Hutang"
                            TxtNotaBAyarHutang.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Bayar Piutang"
                            TxtNotaBayarPiutang.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Pemasukan"
                            TxtNotaJurnalPemasukan.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Pengeluaran"
                            TxtNotaJurnalPengeluaran.Text = result2.ToString("N0", cultureIndonesia)
                        Case "Biaya"
                            TxtNotaJurnalBiaya.Text = result2.ToString("N0", cultureIndonesia)
                        Case "RekeningDebet"
                            TxtNotaJurnalPR.Text = result2.ToString("N0", cultureIndonesia)
                        Case "RekeningKredit"
                            TxtNotaJurnalPRK.Text = result2.ToString("N0", cultureIndonesia)
                    End Select
                End If
            End Using

        End Using
    End Sub

    Private Sub TxtTotal_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _
    TxtTotalPembelian.TextChanged, TxtTotalPenjualan.TextChanged, TxtTotalReturBeli.TextChanged,
    TxtTotalReturJual.TextChanged, TxtTotalBayarHutang.TextChanged, TxtTotalBayarPiutang.TextChanged,
    TxtTotalJurnalPemasukan.TextChanged, TxtTotalJurnalPengeluaran.TextChanged, TxtTotalJurnalBiaya.TextChanged,
    TxtTotalJurnalPR.TextChanged, TxtTotalJurnalPRK.TextChanged, TxtSetorbos.TextChanged

        Dim totalHariIni As Decimal = 0
        Dim value As Decimal

        If Decimal.TryParse(TxtTotalPembelian.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni -= value
        If Decimal.TryParse(TxtTotalPenjualan.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni += value
        If Decimal.TryParse(TxtTotalReturBeli.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni += value
        If Decimal.TryParse(TxtTotalReturJual.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni -= value
        If Decimal.TryParse(TxtTotalBayarHutang.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni -= value
        If Decimal.TryParse(TxtTotalBayarPiutang.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni += value
        If Decimal.TryParse(TxtTotalJurnalPemasukan.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni += value
        If Decimal.TryParse(TxtTotalJurnalPengeluaran.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni -= value
        If Decimal.TryParse(TxtTotalJurnalBiaya.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni -= value
        If Decimal.TryParse(TxtTotalJurnalPR.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni += value
        If Decimal.TryParse(TxtTotalJurnalPRK.Text, Globalization.NumberStyles.Any, cultureIndonesia, value) Then totalHariIni -= value

        TxtTotalHariIni.Text = totalHariIni.ToString("N0", cultureIndonesia)

        Dim setorBosDecimal As Decimal = 0
        If Decimal.TryParse(TxtSetorbos.Text, Globalization.NumberStyles.Any, cultureIndonesia, setorBosDecimal) Then
            totalHariIni -= setorBosDecimal
        End If

        TxtSaldoHAriIni.Text = totalHariIni.ToString("N0", cultureIndonesia)
        TxtSaldoDilaci.Text = totalHariIni.ToString("N0", cultureIndonesia)
    End Sub


    Private Sub TxtNota_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _
    TxtNotaPembelian.TextChanged, TxtNotaPenjualan.TextChanged, TxtNotaReturBeli.TextChanged,
    TxtNotaReturJual.TextChanged, TxtNotaBAyarHutang.TextChanged, TxtNotaBayarPiutang.TextChanged,
    TxtNotaJurnalPemasukan.TextChanged, TxtNotaJurnalPengeluaran.TextChanged, TxtNotaJurnalBiaya.TextChanged,
    TxtNotaJurnalPR.TextChanged, TxtNotaJurnalPRK.TextChanged

        Dim notaHariIni As Integer = 0
        Dim val As Integer

        If Integer.TryParse(TxtNotaPembelian.Text, val) Then notaHariIni -= val
        If Integer.TryParse(TxtNotaPenjualan.Text, val) Then notaHariIni += val
        If Integer.TryParse(TxtNotaReturBeli.Text, val) Then notaHariIni += val
        If Integer.TryParse(TxtNotaReturJual.Text, val) Then notaHariIni -= val
        If Integer.TryParse(TxtNotaBAyarHutang.Text, val) Then notaHariIni -= val
        If Integer.TryParse(TxtNotaBayarPiutang.Text, val) Then notaHariIni += val
        If Integer.TryParse(TxtNotaJurnalPemasukan.Text, val) Then notaHariIni += val
        If Integer.TryParse(TxtNotaJurnalPengeluaran.Text, val) Then notaHariIni -= val
        If Integer.TryParse(TxtNotaJurnalBiaya.Text, val) Then notaHariIni -= val
        If Integer.TryParse(TxtNotaJurnalPR.Text, val) Then notaHariIni += val
        If Integer.TryParse(TxtNotaJurnalPRK.Text, val) Then notaHariIni -= val

        TxtNotaHariIni.Text = notaHariIni.ToString("N0", cultureIndonesia)
    End Sub


    Private Function HitungSaldoAwal(ByVal nomorAkun As String, ByVal kasir As String, ByVal tanggalsaldoawal As Date) As Decimal
        Dim saldoAwalD As Decimal = 0
        Dim saldoAwalK As Decimal = 0
        Dim cultureID As New CultureInfo("id-ID")
        Dim tanggalStr As String = tanggalsaldoawal.AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss")
        Dim userFilter As String = "%" & kasir & "%"

        ' Ambil total debit (D)
        Dim queryNominalD As String = "SELECT SUM(NOMINAL) AS TOTAL FROM JurnalUmum WHERE NOMOR_AKUN_D = @NOMOR_AKUN_D AND TGL_TRANSAKSI <= @TANGGAL AND ID_USER LIKE @ID_USER"
        Using cmdNominalD As New MySqlCommand(queryNominalD, conn)
            cmdNominalD.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkun)
            cmdNominalD.Parameters.AddWithValue("@TANGGAL", tanggalStr)
            cmdNominalD.Parameters.AddWithValue("@ID_USER", userFilter)
            Dim result As Object = cmdNominalD.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Decimal.TryParse(result.ToString(), NumberStyles.Any, cultureID, saldoAwalD)
            End If
        End Using

        ' Ambil total kredit (K)
        Dim queryNominalK As String = "SELECT SUM(NOMINAL) AS TOTAL FROM JurnalUmum WHERE NOMOR_AKUN_K = @NOMOR_AKUN_K AND TGL_TRANSAKSI <= @TANGGAL AND ID_USER LIKE @ID_USER"
        Using cmdNominalK As New MySqlCommand(queryNominalK, conn)
            cmdNominalK.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkun)
            cmdNominalK.Parameters.AddWithValue("@TANGGAL", tanggalStr)
            cmdNominalK.Parameters.AddWithValue("@ID_USER", userFilter)
            Dim result As Object = cmdNominalK.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Decimal.TryParse(result.ToString(), NumberStyles.Any, cultureID, saldoAwalK)
            End If
        End Using

        Return saldoAwalD - saldoAwalK
    End Function



    Private Sub TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtSaldoAwal.TextChanged, TxtSaldoHAriIni.TextChanged
        Dim TotalAkhir As Decimal = 0
        Dim nilai As Decimal

        If Decimal.TryParse(TxtSaldoAwal.Text, NumberStyles.Number, cultureIndonesia, nilai) Then
            TotalAkhir += nilai
        End If

        If Decimal.TryParse(TxtSaldoHAriIni.Text, NumberStyles.Number, cultureIndonesia, nilai) Then
            TotalAkhir += nilai
        End If

        TxtSaldoAkhir.Text = TotalAkhir.ToString("N0", cultureIndonesia)
    End Sub

    ' Deklarasi variabel sebagai string
    Private LblPrinterStrukString As String
    Private CmbPortThermalString As String
    Private CmbPortString As String
    Private TxtMAjuString As Integer
    Private TxtMundurString As Integer
    Private TxtPanjangString As Integer
    Private TxtLebarString As Integer
    Private TxtpikselString As Integer
    Private TxtBatasKiriString As Integer
    Private TxtJarakString As Integer
    Private lblPortNameString As String
    Private lblBaudRateString As Integer
    Private lblParityString As String
    Private lblDataBitsString As Integer
    Private CmbPortCashString As String
    Private CmbCodeCashString As String
    Private CBPotongChecked As Boolean
    Private CmbModelStrukString As String
    Private CmbFNAmaString As String
    Private CmbFKetString As String
    Private CmbFIsiString As String
    Private CmbFFootString As String
    Private CmbUNamaString As Integer
    Private CmbUKetString As Integer
    Private CmbUIsiString As Integer
    Private CmbUFootString As Integer
    Private StatusComp As String

    ' Deklarasi variabel terpisah
    Private kodetoko As String
    Private namatoko As String
    Private alamattoko As String
    Private kotatoko As String
    Private kontaktoko As String
    Private pemilik As String
    Private foter1 As String
    Private foter2 As String


    Public printer_nota As String
    Public MOdelStruk As String
    Public WithEvents PD As New PrintDocument
    Private ReadOnly PPD As New PrintPreviewDialog
    Private longpaper As Integer


    Dim printerDot As String
    Dim lebarDot As Integer
    Dim TinggiDot As Integer
    Dim batasKiriDot As Integer
    Dim jarakBarisDot As Integer
    Dim fontJudulDot As String
    Dim fontIsiDot As String
    Dim ukuranFontJudul As Integer
    Dim ukuranFontIsi As Integer

    Public WithEvents PDDot As New PrintDocument
    Private ReadOnly PPDDot As New PrintPreviewDialog

    Private Panjangkertas As Integer
    Private LebarKertas As Integer

    Private jenisprinter As String

    Public Sub Ambildataperusahaan()
        Dim sql As String = "SELECT kode, nama, alamat, KOTA, hp, pemilik, footer1, footer2 FROM tbl_perusahaan"

        Using cmd As New MySqlCommand(sql, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    rd.Read()
                    kodetoko = rd("KODE").ToString()
                    namatoko = rd("NAMA").ToString()
                    alamattoko = rd("ALAMAT").ToString()
                    kotatoko = rd("KOTA").ToString()
                    kontaktoko = rd("HP").ToString()
                    pemilik = rd("pemilik").ToString()
                    foter1 = rd("FOOTER1").ToString()
                    foter2 = rd("FOOTER2").ToString()
                End If
            End Using
        End Using

    End Sub

    Public Sub Ambildataprinter()
        Dim filePath As String = "printer.ini"

        ' Check if the file exists
        If File.Exists(filePath) Then
            ' File exists, read the values from the file
            Using reader As New StreamReader(filePath)
                Dim line As String = reader.ReadLine()
                While line IsNot Nothing
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 Then
                        Dim key As String = parts(0)
                        Dim value As String = parts(1)

                        ' Assign values to application settings
                        Select Case key
                            Case "PrinterPos"
                                LblPrinterStrukString = value
                                CmbPortThermalString = value
                            Case "PortPrinter"
                                CmbPortString = value
                            Case "Maju"
                                TxtMAjuString = value
                            Case "Mundur"
                                TxtMundurString = value
                            Case "Panjang"
                                TxtPanjangString = value
                            Case "Lebar"
                                TxtLebarString = value
                            Case "Piksel"
                                TxtpikselString = value
                            Case "BatasKiri"
                                TxtBatasKiriString = value
                            Case "Jarak"
                                TxtJarakString = value
                            Case "PortName"
                                lblPortNameString = value
                            Case "BaudRate"
                                lblBaudRateString = value
                            Case "Parity"
                                lblParityString = value
                            Case "DataBits"
                                lblDataBitsString = value
                            Case "PortCashDraw"
                                CmbPortCashString = value
                            Case "CodeCashDraw"
                                CmbCodeCashString = value
                            Case "Potongkertas"
                                CBPotongChecked = Boolean.Parse(value)
                            Case "ModelStruk"
                                CmbModelStrukString = value
                            Case "FontNama"
                                CmbFNAmaString = value
                            Case "FontKet"
                                CmbFKetString = value
                            Case "FontIsi"
                                CmbFIsiString = value
                            Case "FOntFoot"
                                CmbFFootString = value
                            Case "FontUNama"
                                CmbUNamaString = value
                            Case "FontUKet"
                                CmbUKetString = value
                            Case "FontUIsi"
                                CmbUIsiString = value
                            Case "FontUFoot"
                                CmbUFootString = value
                            Case "StatusComp"
                                StatusComp = value


                            Case "JenisPrinterJual"
                                jenisprinter = value
                            Case "PrinterDot"
                                printerDot = value
                            Case "LebarDot"
                                lebarDot = value
                            Case "TinggiDot"
                                TinggiDot = value
                            Case "BatasKiriDot"
                                batasKiriDot = value
                            Case "JarakBarisDot"
                                jarakBarisDot = value
                            Case "FontJudulDot"
                                fontJudulDot = value
                            Case "FontIsiDot"
                                fontIsiDot = value
                            Case "UkuranFontJudul"
                                ukuranFontJudul = value
                            Case "UkuranFontIsi"
                                ukuranFontIsi = value

                        End Select
                    End If
                    line = reader.ReadLine()
                End While
            End Using
        End If

    End Sub

    Private Sub BtnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPrint.Click
        If jenisprinter = "Printer Dot Matrix" Then
            PrinterDotMatrik()
        Else
            Printerstruk()
        End If

    End Sub

    Public Sub Printerstruk()
        Dim printer_nota As String

        If LblPrinterStrukString = "Printerdefault" Then
            ' Baca default printer pada komputer
            Dim defaultPrinter As String = New PrinterSettings().PrinterName
            printer_nota = defaultPrinter
        Else
            printer_nota = LblPrinterStrukString
        End If

        PD.PrinterSettings.PrinterName = printer_nota

        MOdelStruk = CmbModelStrukString
        Changelongpaper()
        PPD.Document = PD
        'PPD.ShowDialog()
        PD.Print()

    End Sub

    Public Sub Changelongpaper()
        Dim rowcount As Integer
        longpaper = 0

        Dim totalTextBoxTidakNol As Integer = 0

        For Each ctrl As Control In Controls
            If TypeOf ctrl Is TextBox Then
                Dim textbox As TextBox = DirectCast(ctrl, TextBox)
                If Not String.IsNullOrEmpty(textbox.Text) AndAlso textbox.Text <> "0" Then
                    totalTextBoxTidakNol += 1
                End If
            End If
        Next

        rowcount = totalTextBoxTidakNol + 4
        longpaper = rowcount * 10
        longpaper += (250 + TxtPanjangString)
    End Sub

    Private Sub PD_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PD.BeginPrint
        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim ps As New PaperSize("Custom", thermalPaperWidthInPixel, longpaper)
        PD.DefaultPageSettings.PaperSize = ps
        PD.DefaultPageSettings.Landscape = False
    End Sub

    Private Sub PD_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PD.PrintPage
        Dim leftmargin As Integer = PD.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String = "-------------------------------------------"
        Dim garisdua As String = "===================="
        Dim TopRight As New StringFormat With {
    .LineAlignment = StringAlignment.Near,
    .Alignment = StringAlignment.Far
}

        Dim thermalPaperWidthInmm As Integer = TxtLebarString
        Dim thermalPaperWidthInPixel As Integer = Convert.ToInt32(thermalPaperWidthInmm / 25.4 * TxtpikselString)
        Dim lebar As Integer = thermalPaperWidthInPixel


        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + TxtBatasKiriString

        'tinggi -= TxtMAjuString
        Dim escapeCommand As String

        escapeCommand = Chr(27) & "J" & Chr(TxtMAjuString) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

        e.Graphics.DrawString(namatoko, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(alamattoko, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(kotatoko, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(kontaktoko, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = TxtBatasKiriString + ((lebar + (25 / 100 * lebar)) - lebar)

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("JENIS", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": LAPORAN MUTASI KEUANGAN", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)


        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Rekening", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & CmbRekening.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        If CbTanggal.Checked = True Then
            e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            ' Set culture ke bahasa Indonesia
            System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("id-ID")

            ' Format tanggal
            Dim formattedDate As String = DtpTanggal.Value.ToString("dd MMMM yyyy")

            ' Menampilkan tanggal
            e.Graphics.DrawString(": " & formattedDate, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        Else
            e.Graphics.DrawString("Bulan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & CmbBln.Text & " " & CmbThn.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & CmbKasir.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)


        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)


        Dim Mulaikata1 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata2 As Integer = TxtBatasKiriString + ((lebar + (11 / 100 * lebar)) - lebar)
        Dim Mulaikata3 As Integer = TxtBatasKiriString + ((lebar + (50 / 100 * lebar)) - lebar)
        Dim Mulaikata4 As Integer = TxtBatasKiriString + ((lebar + (65 / 100 * lebar)) - lebar)
        Dim Mulaikata5 As Integer = TxtBatasKiriString + ((lebar + (90 / 100 * lebar)) - lebar)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Transaksi", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Nota", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Sub Total", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        Dim totalPembelian As Decimal = If(Decimal.TryParse(TxtTotalPembelian.Text, totalPembelian), totalPembelian, 0)
        Dim totalPenjualan As Decimal = If(Decimal.TryParse(TxtTotalPenjualan.Text, totalPenjualan), totalPenjualan, 0)
        Dim totalReturBeli As Decimal = If(Decimal.TryParse(TxtTotalReturBeli.Text, totalReturBeli), totalReturBeli, 0)
        Dim totalReturJual As Decimal = If(Decimal.TryParse(TxtTotalReturJual.Text, totalReturJual), totalReturJual, 0)
        Dim totalBayarHutang As Decimal = If(Decimal.TryParse(TxtTotalBayarHutang.Text, totalBayarHutang), totalBayarHutang, 0)
        Dim totalBayarPiutang As Decimal = If(Decimal.TryParse(TxtTotalBayarPiutang.Text, totalBayarPiutang), totalBayarPiutang, 0)
        Dim totalJurnalPemasukan As Decimal = If(Decimal.TryParse(TxtTotalJurnalPemasukan.Text, totalJurnalPemasukan), totalJurnalPemasukan, 0)
        Dim totalJurnalPengeluaran As Decimal = If(Decimal.TryParse(TxtTotalJurnalPengeluaran.Text, totalJurnalPengeluaran), totalJurnalPengeluaran, 0)
        Dim totalJurnalBiaya As Decimal = If(Decimal.TryParse(TxtTotalJurnalBiaya.Text, totalJurnalBiaya), totalJurnalBiaya, 0)
        Dim totalJurnalPR As Decimal = If(Decimal.TryParse(TxtTotalJurnalPR.Text, totalJurnalPR), totalJurnalPR, 0)
        Dim totalJurnalPRK As Decimal = If(Decimal.TryParse(TxtTotalJurnalPRK.Text, totalJurnalPRK), totalJurnalPRK, 0)


        Dim totalNotaPembelian As Integer = If(Integer.TryParse(TxtNotaPembelian.Text, totalNotaPembelian), totalNotaPembelian, 0)
        Dim totalNotaPenjualan As Integer = If(Integer.TryParse(TxtNotaPenjualan.Text, totalNotaPenjualan), totalNotaPenjualan, 0)
        Dim totalNotaReturBeli As Integer = If(Integer.TryParse(TxtNotaReturBeli.Text, totalNotaReturBeli), totalNotaReturBeli, 0)
        Dim totalNotaReturJual As Integer = If(Integer.TryParse(TxtNotaReturJual.Text, totalNotaReturJual), totalNotaReturJual, 0)
        Dim totalNotaBayarHutang As Integer = If(Integer.TryParse(TxtNotaBAyarHutang.Text, totalNotaBayarHutang), totalNotaBayarHutang, 0)
        Dim totalNotaBayarPiutang As Integer = If(Integer.TryParse(TxtNotaBayarPiutang.Text, totalNotaBayarPiutang), totalNotaBayarPiutang, 0)
        Dim totalNotaJurnalPemasukan As Integer = If(Integer.TryParse(TxtNotaJurnalPemasukan.Text, totalNotaJurnalPemasukan), totalNotaJurnalPemasukan, 0)
        Dim totalNotaJurnalPengeluaran As Integer = If(Integer.TryParse(TxtNotaJurnalPengeluaran.Text, totalNotaJurnalPengeluaran), totalNotaJurnalPengeluaran, 0)
        Dim totalNotaJurnalBiaya As Integer = If(Integer.TryParse(TxtNotaJurnalBiaya.Text, totalNotaJurnalBiaya), totalNotaJurnalBiaya, 0)
        Dim totalNotaJurnalPR As Integer = If(Integer.TryParse(TxtNotaJurnalPR.Text, totalNotaJurnalPR), totalNotaJurnalPR, 0)
        Dim totalNotaJurnalPRK As Integer = If(Integer.TryParse(TxtNotaJurnalPRK.Text, totalNotaJurnalPRK), totalNotaJurnalPRK, 0)



        If totalPembelian <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Pembelian", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaPembelian.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalPembelian.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalPenjualan <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Penjualan", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaPenjualan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalPenjualan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalReturBeli <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Retur beli", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaReturBeli.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalReturBeli.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalReturJual <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Retur jual", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaReturJual.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalReturJual.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalBayarHutang <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Bayar hutang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaBayarHutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalBayarHutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalBayarPiutang <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Piutang di bayar", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaBayarPiutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalBayarPiutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPemasukan <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Jurnal Pemasukan", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPemasukan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPemasukan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPengeluaran <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Jurnal Pengeluaran", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPengeluaran.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPengeluaran.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalBiaya <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Jurnal Biaya", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalBiaya.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalBiaya.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPR <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Jurnal Pindah rek (+)", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPR.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPR.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPRK <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Jurnal Pindah rek (-)", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPRK.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPRK.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim saldoHariIni As Decimal = If(Decimal.TryParse(TxtTotalHariIni.Text, saldoHariIni), saldoHariIni, 0)
        Dim totalHariIni As Decimal = If(Decimal.TryParse(TxtSaldoHAriIni.Text, totalHariIni), totalHariIni, 0)
        Dim setorkebos As Decimal = If(Decimal.TryParse(TxtSetorbos.Text, setorkebos), setorkebos, 0)
        Dim sisadilaci As Decimal = If(Decimal.TryParse(TxtSaldoDilaci.Text, sisadilaci), sisadilaci, 0)


        Dim saldoAwal As Decimal = If(Decimal.TryParse(TxtSaldoAwal.Text, saldoAwal), saldoAwal, 0)
        Dim saldoTotal As Decimal = If(Decimal.TryParse(TxtSaldoAkhir.Text, saldoTotal), saldoTotal, 0)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata3, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("       Saldo Hari ini", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(saldoHariIni.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        If TxtTypeAkun.Text = "KAS" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("       Uang di setor", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(setorkebos.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("       Uang di laci", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(sisadilaci.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Saldo awal :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(saldoAwal.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Hari ini :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(totalHariIni.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Saldo Akhir :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(saldoTotal.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(kotatoko & ", " & Now.ToString("dd-MM-yyyy"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata5, tinggi, kanan)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("ACC", New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(pemilik, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, BatasKiri, tinggi)

        'tinggi = tinggi - TxtMundurString
        escapeCommand = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
    End Sub

    Public Sub PrinterDotMatrik()
        Dim printer_nota As String

        If printerDot <> "Printerdefault" Then
            printer_nota = printerDot
        Else
            ' Baca default printer pada komputer
            Dim defaultPrinter As String = New PrinterSettings().PrinterName
            printer_nota = defaultPrinter
        End If

        PDDot.PrinterSettings.PrinterName = printer_nota
        RubahPanjangkertas()
        PPDDot.Document = PDDot
        'PPDDot.ShowDialog()
        PDDot.Print()
    End Sub

    Public Sub RubahPanjangkertas()
        Dim TinggiKertas As Integer = CInt((TinggiDot * 0.3937) * 72) ' Tinggi dalam dot
        'Dim TinggiKertas As Integer = 70
        Dim rowcount As Integer

        Panjangkertas = 0
        Dim totalTextBoxTidakNol As Integer = 0

        For Each ctrl As Control In Controls
            If TypeOf ctrl Is TextBox Then
                Dim textbox As TextBox = DirectCast(ctrl, TextBox)
                If Not String.IsNullOrEmpty(textbox.Text) AndAlso textbox.Text <> "0" Then
                    totalTextBoxTidakNol += 1
                End If
            End If
        Next

        Panjangkertas = TinggiKertas
        rowcount = totalTextBoxTidakNol + 4
        Panjangkertas += rowcount * 20

        LebarKertas = CInt((lebarDot * 0.3937) * 72) ' Lebar dalam dot
    End Sub

    Private Sub PDDot_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PDDot.BeginPrint
        ' Lebar dan Tinggi kertas dalam dot (1 inch = 2.54 cm, 1 inch = 72 dot)

        Dim ps As New PaperSize("Custom", LebarKertas, Panjangkertas)
        PDDot.DefaultPageSettings.PaperSize = ps
        PDDot.DefaultPageSettings.Landscape = False
    End Sub


    Private Sub PDDot_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PDDot.PrintPage
        Dim centermargin As Integer = LebarKertas / 4
        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        ' Asumsikan lebar satu karakter rata-rata adalah 8 piksel
        Dim lebarKarakter As Double = 7.35
        Dim jumlahKarakter As Integer = CInt(Math.Floor(LebarKertas / lebarKarakter)) / 1.8

        ' Buat garis berdasarkan jumlah karakter
        Dim garis As String = New String("-"c, jumlahKarakter)
        Dim garisdua As String = "========================"

        Dim TopRight As New StringFormat With {
    .LineAlignment = StringAlignment.Near,
    .Alignment = StringAlignment.Far
    }

        Dim tinggi As Integer = 10
        Dim BatasKiri As Integer = 2 + batasKiriDot

        'tinggi -= TxtMAjuString
        'Dim escapeCommand As String

        'escapeCommand = Chr(27) & "J" & Chr(TxtMAjuString) 'Penting untuk dicatat bahwa karakter Chr(27) adalah kode untuk karakter escape, J adalah perintah untuk memundurkan kertas, dan Chr(5) adalah parameter yang menentukan jumlah baris yang akan diundurkan. Anda dapat mengubah nilai 5 sesuai dengan kebutuhan Anda

        e.Graphics.DrawString(namatoko, New Drawing.Font(CmbFNAmaString, CmbUNamaString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 20 + TxtJarakString
        e.Graphics.DrawString(alamattoko, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(kotatoko, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)
        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(kontaktoko, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, centermargin, tinggi, tengah)


        Dim Mulaikata As Integer = BatasKiri + (LebarKertas * 15 / 100)

        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("JENIS", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": LAPORAN MUTASI KEUANGAN", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)


        tinggi += 15 + TxtJarakString
        e.Graphics.DrawString("Rekening", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & CmbRekening.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        tinggi += 10 + TxtJarakString
        If CbTanggal.Checked = True Then
            e.Graphics.DrawString("Tanggal", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            ' Set culture ke bahasa Indonesia
            System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("id-ID")

            ' Format tanggal
            Dim formattedDate As String = DtpTanggal.Value.ToString("dd MMMM yyyy")

            ' Menampilkan tanggal
            e.Graphics.DrawString(": " & formattedDate, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)

        Else
            e.Graphics.DrawString("Bulan", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(": " & CmbBln.Text & " " & CmbThn.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Kasir", New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString(": " & CmbKasir.Text, New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata, tinggi)


        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)


        'Dim Mulaikata1 As Integer = BatasKiri + (LebarKertas * 5 / 100)
        'Dim Mulaikata2 As Integer = BatasKiri + (LebarKertas * 20 / 100)
        Dim Mulaikata3 As Integer = BatasKiri + (LebarKertas * 30 / 100)
        Dim Mulaikata4 As Integer = BatasKiri + (LebarKertas * 35 / 100)
        Dim Mulaikata5 As Integer = BatasKiri + (LebarKertas * 50 / 100)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Transaksi", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
        e.Graphics.DrawString("Nota", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
        e.Graphics.DrawString("Sub Total", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 14 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        Dim totalPembelian As Decimal = If(Decimal.TryParse(TxtTotalPembelian.Text, totalPembelian), totalPembelian, 0)
        Dim totalPenjualan As Decimal = If(Decimal.TryParse(TxtTotalPenjualan.Text, totalPenjualan), totalPenjualan, 0)
        Dim totalReturBeli As Decimal = If(Decimal.TryParse(TxtTotalReturBeli.Text, totalReturBeli), totalReturBeli, 0)
        Dim totalReturJual As Decimal = If(Decimal.TryParse(TxtTotalReturJual.Text, totalReturJual), totalReturJual, 0)
        Dim totalBayarHutang As Decimal = If(Decimal.TryParse(TxtTotalBayarHutang.Text, totalBayarHutang), totalBayarHutang, 0)
        Dim totalBayarPiutang As Decimal = If(Decimal.TryParse(TxtTotalBayarPiutang.Text, totalBayarPiutang), totalBayarPiutang, 0)
        Dim totalJurnalPemasukan As Decimal = If(Decimal.TryParse(TxtTotalJurnalPemasukan.Text, totalJurnalPemasukan), totalJurnalPemasukan, 0)
        Dim totalJurnalPengeluaran As Decimal = If(Decimal.TryParse(TxtTotalJurnalPengeluaran.Text, totalJurnalPengeluaran), totalJurnalPengeluaran, 0)
        Dim totalJurnalBiaya As Decimal = If(Decimal.TryParse(TxtTotalJurnalBiaya.Text, totalJurnalBiaya), totalJurnalBiaya, 0)
        Dim totalJurnalPR As Decimal = If(Decimal.TryParse(TxtTotalJurnalPR.Text, totalJurnalPR), totalJurnalPR, 0)
        Dim totalJurnalPRK As Decimal = If(Decimal.TryParse(TxtTotalJurnalPRK.Text, totalJurnalPRK), totalJurnalPRK, 0)


        Dim totalNotaPembelian As Integer = If(Integer.TryParse(TxtNotaPembelian.Text, totalNotaPembelian), totalNotaPembelian, 0)
        Dim totalNotaPenjualan As Integer = If(Integer.TryParse(TxtNotaPenjualan.Text, totalNotaPenjualan), totalNotaPenjualan, 0)
        Dim totalNotaReturBeli As Integer = If(Integer.TryParse(TxtNotaReturBeli.Text, totalNotaReturBeli), totalNotaReturBeli, 0)
        Dim totalNotaReturJual As Integer = If(Integer.TryParse(TxtNotaReturJual.Text, totalNotaReturJual), totalNotaReturJual, 0)
        Dim totalNotaBayarHutang As Integer = If(Integer.TryParse(TxtNotaBAyarHutang.Text, totalNotaBayarHutang), totalNotaBayarHutang, 0)
        Dim totalNotaBayarPiutang As Integer = If(Integer.TryParse(TxtNotaBayarPiutang.Text, totalNotaBayarPiutang), totalNotaBayarPiutang, 0)
        Dim totalNotaJurnalPemasukan As Integer = If(Integer.TryParse(TxtNotaJurnalPemasukan.Text, totalNotaJurnalPemasukan), totalNotaJurnalPemasukan, 0)
        Dim totalNotaJurnalPengeluaran As Integer = If(Integer.TryParse(TxtNotaJurnalPengeluaran.Text, totalNotaJurnalPengeluaran), totalNotaJurnalPengeluaran, 0)
        Dim totalNotaJurnalBiaya As Integer = If(Integer.TryParse(TxtNotaJurnalBiaya.Text, totalNotaJurnalBiaya), totalNotaJurnalBiaya, 0)
        Dim totalNotaJurnalPR As Integer = If(Integer.TryParse(TxtNotaJurnalPR.Text, totalNotaJurnalPR), totalNotaJurnalPR, 0)
        Dim totalNotaJurnalPRK As Integer = If(Integer.TryParse(TxtNotaJurnalPRK.Text, totalNotaJurnalPRK), totalNotaJurnalPRK, 0)



        If totalPembelian <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Pembelian", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaPembelian.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalPembelian.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalPenjualan <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Penjualan", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaPenjualan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalPenjualan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalReturBeli <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Retur beli", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaReturBeli.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalReturBeli.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalReturJual <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Retur jual", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaReturJual.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalReturJual.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalBayarHutang <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Bayar hutang", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaBayarHutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalBayarHutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalBayarPiutang <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Piutang di bayar", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaBayarPiutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalBayarPiutang.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPemasukan <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Jurnal Pemasukan", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPemasukan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPemasukan.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPengeluaran <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Jurnal Pengeluaran", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPengeluaran.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPengeluaran.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalBiaya <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Jurnal Biaya", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalBiaya.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalBiaya.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPR <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(+) Jurnal Pindah rek (+)", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPR.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPR.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        If totalJurnalPRK <> 0 Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("(-)  Jurnal Pindah rek (-)", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, BatasKiri, tinggi)
            e.Graphics.DrawString(":", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(totalNotaJurnalPRK.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata3, tinggi, kanan)
            e.Graphics.DrawString(totalJurnalPRK.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        Dim saldoHariIni As Decimal = If(Decimal.TryParse(TxtTotalHariIni.Text, saldoHariIni), saldoHariIni, 0)
        Dim totalHariIni As Decimal = If(Decimal.TryParse(TxtSaldoHAriIni.Text, totalHariIni), totalHariIni, 0)
        Dim setorkebos As Decimal = If(Decimal.TryParse(TxtSetorbos.Text, setorkebos), setorkebos, 0)
        Dim sisadilaci As Decimal = If(Decimal.TryParse(TxtSaldoDilaci.Text, sisadilaci), sisadilaci, 0)


        Dim saldoAwal As Decimal = If(Decimal.TryParse(TxtSaldoAwal.Text, saldoAwal), saldoAwal, 0)
        Dim saldoTotal As Decimal = If(Decimal.TryParse(TxtSaldoAkhir.Text, saldoTotal), saldoTotal, 0)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garisdua, New Drawing.Font("Courier New", 8), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Saldo Hari ini :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(saldoHariIni.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        If TxtTypeAkun.Text = "KAS" Then
            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Uang di setor :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(setorkebos.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


            tinggi += 10 + TxtJarakString
            e.Graphics.DrawString("Uang di laci : ", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
            e.Graphics.DrawString(sisadilaci.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)
        End If

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Saldo awal :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(saldoAwal.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Hari ini :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(totalHariIni.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("Saldo Akhir :", New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata4, tinggi, kanan)
        e.Graphics.DrawString(saldoTotal.ToString("N0", cultureIndonesia), New Drawing.Font(CmbFIsiString, CmbUIsiString), Brushes.Black, Mulaikata5, tinggi, kanan)


        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(garis, New Drawing.Font("Courier New", 8), Brushes.Black, BatasKiri, tinggi)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString(kotatoko & ", " & Now.ToString("dd-MM-yyyy"), New Drawing.Font(CmbFKetString, CmbUKetString), Brushes.Black, Mulaikata5, tinggi, kanan)

        tinggi += 10 + TxtJarakString
        e.Graphics.DrawString("ACC", New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, BatasKiri, tinggi)
        tinggi += 30 + TxtJarakString
        e.Graphics.DrawString(pemilik, New Drawing.Font(CmbFFootString, CmbUFootString), Brushes.Black, BatasKiri, tinggi)

        'tinggi = tinggi - TxtMundurString
        'escapeCommand = Chr(27) & "d" & Chr(TxtMundurString) ' Memundurkan kertas sejauh 5 baris
    End Sub


    Private Sub FormLapSaldo_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                BtnHitung.PerformClick()
            Case Keys.F4
                BtnPrint.PerformClick()
            Case Keys.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub ValidateDateSelection()
        If Not (CbTanggal.Checked Or CbBulan.Checked) Then
            MessageBox.Show("Pilih dulu berdasarkan tanggal atau bulan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Throw New ApplicationException("Tanggal atau bulan belum dipilih.")
        Else
            PanelView.Visible = True

            ' Posisikan PanelView di tengah form induk
            PanelView.Left = (Me.ClientSize.Width - PanelView.Width) \ 2
            PanelView.Top = (Me.ClientSize.Height - PanelView.Height) \ 2

            ' Clear DataTable sebelum mengisi data baru
            If dt.Rows.Count > 0 Then
                dt.Clear()
            End If
            ' Memutuskan sumber data
            DGVView.DataSource = Nothing
        End If
    End Sub


    Private Sub GetTanggalAwalAkhir(ByRef tanggalAwal As Date, ByRef tanggalAkhir As Date, ByRef tanggalperiode As String)
        If CbTanggal.Checked Then
            tanggalAwal = DtpTanggal.Value.Date
            tanggalAkhir = tanggalAwal.AddDays(1).AddTicks(-1)
            tanggalperiode = DtpTanggal.Value.Date.ToString("dd MMMM yyyy")
        ElseIf CbBulan.Checked Then
            Convertbulansetor()
            Dim bulan As Integer = angkabulan
            Dim tahun As Integer = CmbThn.Text
            tanggalAwal = New DateTime(tahun, bulan, 1)
            tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1).AddSeconds(86399)
            tanggalperiode = "1 - " & tanggalAkhir.ToString("dd MMMM yyyy")
        End If
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        Dim dgvType As Type = dgv.GetType()
        Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
        pi.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub BtnBeli_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBeli.Click
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = CmbRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        LblView.Text = "Daftar laporan pembelian tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text


        ' Buat DataTable baru
        dt = New DataTable()

        Dim query As String = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, TGL_BELI, LOKASI, GRAND_TOTAL_BELI, PEMBAYARAN FROM pembelian WHERE TGL_BELI >= @tanggalAwal AND TGL_BELI <= @tanggalAkhir AND JENIS_BAYAR = @jenisBayar"

        If kasir <> "" Then
            query &= " AND ID_USER = @idUser"
        End If

        query &= " ORDER BY ID_PEMBELIAN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", strTanggalAwal)
            cmd.Parameters.AddWithValue("@tanggalAkhir", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@jenisBayar", Rekening)
            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@idUser", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        DGVView.DataSource = dt

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = totalPembayaran
        Next

        Dim columnsToFormat As String() = {"No", "GRAND_TOTAL_BELI", "PEMBAYARAN", "Saldo"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"No", "No"},
            {"ID_PEMBELIAN", "Nota"},
            {"NAMA_SUPLIYER", "Supplier"},
            {"TGL_BELI", "Tanggal"},
            {"LOKASI", "Lokasi"},
            {"GRAND_TOTAL_BELI", "Total pembelian"},
            {"PEMBAYARAN", "Pembayaran"},
            {"Saldo", "Saldo"}
        }

        With DGVView
            ' Loop through columns and set format and alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Use custom format to display numbers with commas and up to two decimal places if not zero
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            ' ===== KHUSUS KOLOM NO =====
            If DGVView.Columns.Contains("No") Then
                With DGVView.Columns("No")
                    .Width = 40
                    .MinimumWidth = 40
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                End With
            End If

            ' Rename columns
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(DGVView)
        End With
    End Sub

    Private Sub BtnJual_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnJual.Click
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = TxtRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        LblView.Text = "Daftar laporan penjualan tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text


        ' Buat DataTable baru
        dt = New DataTable()

        Dim query As String = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, TGL_TRANSAKSI, LOKASIBARANG, GRAND_TOTAL_STL_PAJAK, (BAYAR - KEMBALI) AS PEMBAYARAN FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND KODE_AKUN = @kodeAkun"

        If kasir <> "" Then
            query &= " AND ID_USER = @idUser"
        End If

        query &= " ORDER BY ID_PENJUALAN"


        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", strTanggalAwal)
            cmd.Parameters.AddWithValue("@tanggalAkhir", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@kodeAkun", Rekening)

            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@idUser", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        DGVView.DataSource = dt

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = totalPembayaran
        Next

        Dim columnsToFormat As String() = {"No", "GRAND_TOTAL_STL_PAJAK", "PEMBAYARAN", "Saldo"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"No", "No"},
            {"ID_PENJUALAN", "Nota"},
            {"NAMA_PELANGGAN", "Pelanggan"},
            {"TGL_TRANSAKSI", "Tanggal"},
            {"LOKASIBARANG", "Lokasi"},
            {"GRAND_TOTAL_STL_PAJAK", "Total penjualan"},
            {"PEMBAYARAN", "Pembayaran"},
            {"Saldo", "Saldo"}
        }

        With DGVView
            ' Loop through columns and set format and alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Use custom format to display numbers with commas and up to two decimal places if not zero
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            ' ===== KHUSUS KOLOM NO =====
            If DGVView.Columns.Contains("No") Then
                With DGVView.Columns("No")
                    .Width = 40
                    .MinimumWidth = 40
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                End With
            End If

            ' Rename columns
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(DGVView)
        End With
    End Sub

    Private Sub BtnReturBeli_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturBeli.Click
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = TxtRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        LblView.Text = "Daftar laporan retur pembelian tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text


        ' Buat DataTable baru
        dt = New DataTable()

        Dim query As String = "SELECT ID_RETUR_PEMBELIAN, NAMA_SUPPLIER, TGL_RETUR_BELI, PENYIMPANAN, TOTAL_RUPIAH AS PEMBAYARAN FROM retur_pembelian WHERE TGL_RETUR_BELI >= @tanggalAwal AND TGL_RETUR_BELI <= @tanggalAkhir AND KODE_REKENING = @kodeAkun"

        If kasir <> "" Then
            query &= " AND ID_USER = @idUser"
        End If

        query &= " ORDER BY ID_RETUR_PEMBELIAN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", strTanggalAwal)
            cmd.Parameters.AddWithValue("@tanggalAkhir", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@kodeAkun", Rekening)

            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@idUser", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        DGVView.DataSource = dt

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = totalPembayaran
        Next

        Dim columnsToFormat As String() = {"No", "PEMBAYARAN", "Saldo"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"No", "No"},
            {"ID_RETUR_PEMBELIAN", "Nota"},
            {"NAMA_SUPPLIER", "Supplier"},
            {"TGL_RETUR_BELI", "Tanggal"},
             {"PENYIMPANAN", "Penyimpanan"},
            {"PEMBAYARAN", "Pembayaran"},
            {"Saldo", "Saldo"}
        }

        With DGVView
            ' Loop through columns and set format and alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Use custom format to display numbers with commas and up to two decimal places if not zero
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            ' ===== KHUSUS KOLOM NO =====
            If DGVView.Columns.Contains("No") Then
                With DGVView.Columns("No")
                    .Width = 40
                    .MinimumWidth = 40
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                End With
            End If

            ' Rename columns
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(DGVView)
        End With
    End Sub

    Private Sub BtnReturJual_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReturJual.Click
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = TxtRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        LblView.Text = "Daftar laporan retur penjualan tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text


        ' Buat DataTable baru
        dt = New DataTable()

        Dim query As String = "SELECT ID_RETUR_PENJUALAN, NAMA_PELANGGAN, TGL_RETUR_JUAL, PENYIMPANAN, TOTAL_RUPIAH AS PEMBAYARAN FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @tanggalAwal AND TGL_RETUR_JUAL <= @tanggalAkhir AND KODE_REKENING = @kodeAkun"

        If kasir <> "" Then
            query &= " AND ID_USER = @idUser"
        End If

        query &= " ORDER BY ID_RETUR_PENJUALAN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", strTanggalAwal)
            cmd.Parameters.AddWithValue("@tanggalAkhir", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@kodeAkun", Rekening)

            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@idUser", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        DGVView.DataSource = dt

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = totalPembayaran
        Next

        Dim columnsToFormat As String() = {"No", "PEMBAYARAN", "Saldo"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"No", "No"},
            {"ID_RETUR_PENJUALAN", "Nota"},
            {"NAMA_PELANGGAN", "Pelanggan"},
            {"TGL_RETUR_JUAL", "Tanggal"},
            {"PENYIMPANAN", "Penyimpanan"},
            {"PEMBAYARAN", "Pembayaran"},
            {"Saldo", "Saldo"}
        }

        With DGVView
            ' Loop through columns and set format and alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Use custom format to display numbers with commas and up to two decimal places if not zero
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            ' ===== KHUSUS KOLOM NO =====
            If DGVView.Columns.Contains("No") Then
                With DGVView.Columns("No")
                    .Width = 40
                    .MinimumWidth = 40
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                End With
            End If

            ' Rename columns
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(DGVView)
        End With
    End Sub

    Private Sub BtnHutang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHutang.Click
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = TxtRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        LblView.Text = "Daftar pembayaran hutang tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text

        ' Buat DataTable baru
        dt = New DataTable()

        ' Query SQL dengan nama kolom yang konsisten
        Dim query As String = "SELECT NO_TRANSAKSI, NAMA_BANTU_D, TGL_TRANSAKSI, NOMINAL AS PEMBAYARAN FROM JurnalUmum WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR AND NOMOR_AKUN_K = @KODE_AKUN AND JENIS_TRANSAKSI = 'Bayar hutang'"

        If kasir <> "" Then
            query &= " AND ID_USER = @ID_USER"
        End If

        query &= " ORDER BY NO_TRANSAKSI"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", strTanggalAwal)
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@KODE_AKUN", Rekening)

            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@ID_USER", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        DGVView.DataSource = dt

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = Math.Round(totalPembayaran, 2) ' Bulatkan saldo ke dua angka desimal
        Next

        ' Daftar kolom yang akan diformat
        Dim columnsToFormat As String() = {"No", "PEMBAYARAN", "Saldo"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"No", "No"},
            {"NO_TRANSAKSI", "Nota"},
            {"NAMA_BANTU_D", "Supplier"},
            {"TGL_TRANSAKSI", "Tanggal"},
            {"PEMBAYARAN", "Pembayaran"},
            {"Saldo", "Saldo"}
        }

        With DGVView
            ' Loop untuk mengatur format dan alignment kolom
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Format custom untuk angka
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            ' ===== KHUSUS KOLOM NO =====
            If DGVView.Columns.Contains("No") Then
                With DGVView.Columns("No")
                    .Width = 40
                    .MinimumWidth = 40
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                End With
            End If

            ' Ganti nama kolom
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Gaya header
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Gaya baris selang-seling
            .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

            ' Gaya visual lainnya
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Aktifkan double buffering untuk mengurangi flicker
            EnableDoubleBuffering(DGVView)
        End With
    End Sub


    Private Sub BtnPiutang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPiutang.Click
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = TxtRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        LblView.Text = "Daftar pembayaran Piutang tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text

        ' Buat DataTable baru
        dt = New DataTable()

        ' Query SQL dengan nama kolom yang konsisten
        Dim query As String = "SELECT NO_TRANSAKSI, NAMA_BANTU_K, TGL_TRANSAKSI, NOMINAL AS PEMBAYARAN FROM JurnalUmum WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR AND NOMOR_AKUN_D = @KODE_AKUN AND JENIS_TRANSAKSI = 'Bayar Piutang'"

        If kasir <> "" Then
            query &= " AND ID_USER = @ID_USER"
        End If

        query &= " ORDER BY NO_TRANSAKSI"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", strTanggalAwal)
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@KODE_AKUN", Rekening)

            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@ID_USER", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        DGVView.DataSource = dt

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = Math.Round(totalPembayaran, 2) ' Pastikan nilai saldo dibulatkan ke dua desimal
        Next

        ' Daftar kolom yang akan diformat
        Dim columnsToFormat As String() = {"No", "PEMBAYARAN", "Saldo"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"No", "No"},
            {"NO_TRANSAKSI", "Nota"},
            {"NAMA_BANTU_K", "Pelanggan"},
            {"TGL_TRANSAKSI", "Tanggal"},
            {"PEMBAYARAN", "Pembayaran"},
            {"Saldo", "Saldo"}
        }

        With DGVView
            ' Loop melalui kolom untuk mengatur format dan alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Format custom untuk angka
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next


            ' ===== KHUSUS KOLOM NO =====
            If DGVView.Columns.Contains("No") Then
                With DGVView.Columns("No")
                    .Width = 40
                    .MinimumWidth = 40
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                End With
            End If

            ' Ganti nama kolom
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Gaya header
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Gaya baris selang-seling
            .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

            ' Gaya visual lainnya
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Aktifkan double buffering untuk mengurangi flicker
            EnableDoubleBuffering(DGVView)
        End With
    End Sub


    Private Sub BtnJurnalMasuk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnJurnalMasuk.Click
        DisplayJurnalUmum("PEMASUKAN")
    End Sub

    Private Sub BtnJurnalKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnJurnalKeluar.Click
        DisplayJurnalUmum("PENGELUARAN")
    End Sub

    Private Sub BtnJurnalBiaya_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnJurnalBiaya.Click
        DisplayJurnalUmum("BIAYA")
    End Sub

    Private Sub BtnJurnalPindahMasuk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnJurnalPindahMasuk.Click
        DisplayJurnalUmum("PINDAH REKENING MASUK")
    End Sub

    Private Sub BtnJurnalPindahKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnJurnalPindahKeluar.Click
        DisplayJurnalUmum("PINDAH REKENING KELUAR")
    End Sub

    Private Sub BtnSetorBos_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSetorBos.Click
        DisplayJurnalUmum("SETOR KE BOS")
    End Sub

    Private Sub DisplayJurnalUmum(ByVal jenisTransaksi As String)
        ValidateDateSelection()

        Dim kasir As String = If(CmbKasir.Text = "Semua" Or CmbKasir.SelectedIndex = 0, "", CmbKasir.Text)
        Dim Rekening As String = TxtRekening.Text

        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        Dim tanggalperiode As String = ""  ' Deklarasi dan inisialisasi tanggalperiode

        GetTanggalAwalAkhir(tanggalAwal, tanggalAkhir, tanggalperiode)

        ' Format tanggal untuk keperluan query atau tampilan
        Dim strTanggalAwal As String = tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss")
        Dim strTanggalAkhir As String = tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss")

        Dim jenisLabel As String = ""
        Dim kodeAkunField As String = ""
        Select Case jenisTransaksi
            Case "PEMASUKAN"
                jenisLabel = "Pemasukan, Bayar bon"
                kodeAkunField = "NOMOR_AKUN_D"
            Case "PENGELUARAN"
                jenisLabel = "Pengeluaran, Bon, Gaji"
                kodeAkunField = "NOMOR_AKUN_K"
            Case "BIAYA"
                jenisLabel = "Biaya"
                kodeAkunField = "NOMOR_AKUN_K"
            Case "PINDAH REKENING MASUK"
                jenisLabel = "pindah rekening masuk"
                kodeAkunField = "NOMOR_AKUN_D"
            Case "PINDAH REKENING KELUAR"
                jenisLabel = "pindah rekening keluar"
                kodeAkunField = "NOMOR_AKUN_K"
            Case "SETOR KE BOS"
                jenisLabel = "SETOR KE BOS"
                kodeAkunField = "NOMOR_AKUN_K"
        End Select

        LblView.Text = "Daftar " & jenisLabel & " tanggal " & tanggalperiode & " kasir : " & CmbKasir.Text & " Rekening : " & CmbRekening.Text

        ' Buat DataTable baru
        Dim dt As New DataTable()

        ' Buat list untuk jenisLabel dan kondisi WHERE
        Dim jenisLabelConditions As New List(Of String)

        Select Case jenisTransaksi
            Case "PEMASUKAN"
                jenisLabelConditions.Add("(" & kodeAkunField & " = @KODE_AKUN AND (JENIS_TRANSAKSI = 'Pemasukan' OR JENIS_TRANSAKSI = 'Bayar bon'))")
            Case "PENGELUARAN"
                jenisLabelConditions.Add("(" & kodeAkunField & " = @KODE_AKUN AND (JENIS_TRANSAKSI = 'Pengeluaran' OR JENIS_TRANSAKSI = 'Bon' OR JENIS_TRANSAKSI = 'Gaji'))")
            Case "BIAYA"
                jenisLabelConditions.Add("(" & kodeAkunField & " = @KODE_AKUN AND JENIS_TRANSAKSI = 'Biaya')")
            Case "PINDAH REKENING MASUK"
                jenisLabelConditions.Add("(" & kodeAkunField & " = @KODE_AKUN AND JENIS_TRANSAKSI = 'PINDAH REKENING')")
            Case "PINDAH REKENING KELUAR"
                jenisLabelConditions.Add("(" & kodeAkunField & " = @KODE_AKUN AND JENIS_TRANSAKSI = 'PINDAH REKENING')")
            Case "SETOR KE BOS"
                jenisLabelConditions.Add("(" & kodeAkunField & " = @KODE_AKUN AND JENIS_TRANSAKSI = 'SETOR KE BOS')")
        End Select

        ' Gabungkan kondisi WHERE dalam satu string
        Dim jenisLabelCondition As String = String.Join(" OR ", jenisLabelConditions)

        ' Query SQL dengan kondisi WHERE
        Dim query As String = "SELECT NO_TRANSAKSI, URAIAN, TGL_TRANSAKSI, NOMINAL AS PEMBAYARAN FROM JurnalUmum WHERE TGL_TRANSAKSI >= @TANGGAL_AWAL AND TGL_TRANSAKSI <= @TANGGAL_AKHIR AND (" & jenisLabelCondition & ")"

        ' Tambahkan kondisi kodeAkunField dan kasir jika ada
        If Not String.IsNullOrEmpty(kodeAkunField) Then
            query &= " AND " & kodeAkunField & " = @KODE_AKUN"
        End If

        If kasir <> "" Then
            query &= " AND ID_USER = @ID_USER"
        End If

        ' Tambahkan ORDER BY
        query &= " ORDER BY NO_TRANSAKSI"


        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", strTanggalAwal)
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", strTanggalAkhir)
            cmd.Parameters.AddWithValue("@KODE_AKUN", Rekening)

            If kasir <> "" Then
                cmd.Parameters.AddWithValue("@ID_USER", kasir)
            End If

            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        ' Tambahkan kolom "No" di awal DataTable
        dt.Columns.Add("No", GetType(Integer)).SetOrdinal(0)

        ' Isi kolom "No" dengan nilai berurutan
        Dim rowIndex As Integer = 1
        For Each row As DataRow In dt.Rows
            row("No") = rowIndex
            rowIndex += 1
        Next

        ' Tambahkan kolom "Saldo" ke DataTable di akhir
        dt.Columns.Add("Saldo", GetType(Decimal))

        ' Hitung saldo untuk setiap baris
        Dim totalPembayaran As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalPembayaran += Convert.ToDecimal(row("PEMBAYARAN"))
            row("Saldo") = totalPembayaran
        Next

        ' Bind DataTable ke DataGridView
        DGVView.DataSource = dt

        ' Format kolom dan tampilan DataGridView
        FormatDataGridView()

        ' Tampilkan header style, alternating row style, dan visual style DataGridView
        SetDataGridViewStyles()
    End Sub

    Private Sub FormatDataGridView()
        ' Kolom yang akan diformat
        Dim columnsToFormat As String() = {"No", "PEMBAYARAN", "Saldo"}

        ' Format dan alignment untuk kolom DataGridView
        For Each columnName As String In columnsToFormat
            If DGVView.Columns.Contains(columnName) Then
                DGVView.Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                DGVView.Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                DGVView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next

        ' ===== KHUSUS KOLOM NO =====
        If DGVView.Columns.Contains("No") Then
            With DGVView.Columns("No")
                .Width = 40
                .MinimumWidth = 40
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            End With
        End If

        If DGVView.Columns.Contains("URAIAN") Then
            With DGVView.Columns("URAIAN")
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            End With
        End If

    End Sub

    Private Sub SetDataGridViewStyles()
        ' Header style
        DGVView.ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

        ' Alternating row style
        DGVView.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

        ' Visual style
        DGVView.BorderStyle = BorderStyle.FixedSingle
        DGVView.GridColor = Color.Silver
        DGVView.BackgroundColor = Color.White

        ' Enable double buffering to reduce flickering
        EnableDoubleBuffering(DGVView)
    End Sub

    Private Sub BtnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHide.Click
        PanelView.Visible = False
    End Sub


End Class