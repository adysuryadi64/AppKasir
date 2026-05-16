Imports Microsoft.Reporting.WinForms

Public Class FormGaji
    Dim teksBulanTahunTerpilih As String

    Private Sub FormGaji_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Panel2.Visible = False

        Dim GAJI As Boolean() = ModulHakAkses.BacaHakAksesDariCache("GAJI")
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpann.Visible = GAJI(1) ' CanAdd 

        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")

        ResetControls()
        AmbilDataKaryawan()
        AmbildataMasterGaji()
        UpdateTotalBonDanTotalBayarKaryawan()
        Me.ReportViewer1.RefreshReport()
    End Sub

    Private Sub ResetControls()
        'DtpTanggal.Value = DateTime.Today
        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        LblKode.Text = "KRY"
        CmbNama.SelectedIndex = -1
        TxtPokok.Clear()
        TxtKomisiJual.Clear()
        LblSupir.Text = "0"
        TxtSupir.Clear()
        LblHelper.Text = "0"
        TxtHelper.Clear()
        TxtLembur.Clear()
        TxtLemburRp.Clear()
        TxtTunjangan.Clear()
        TxtTransport.Clear()
        TxtMakan.Clear()
        TxtPotBon.Clear()
        TxtAngsuran.Clear()
        TxtAbsen.Clear()
        TxtAbsenRp.Clear()
        TxtAbsenKhusus.Clear()
        TxtAbsenKhususRp.Clear()
        TxtKeterlambatan.Clear()
        TxtKeterlambatanRp.Clear()
        TxtPotLain.Clear()
        TxtPendapatan.Clear()
        TxtPotongan.Clear()
        TxtTerima.Clear()
        TxtPotBonUntukEdit.Text = "0"
        LblSaldoBon.Text = "0"

        GenerateNomorGaji()
        UpdateTotalBonDanTotalBayarKaryawan()
        MuatComboBoxBulanTahun()
        Ambildataperiodekerja()
        BtnSimpann.Text = "SIMPAN (F8)"
    End Sub



    Private Sub GenerateNomorGaji()
        Dim cekTanggal As String = DtpTanggal.Value.ToString("yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "GJ-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(NOMOR) FROM Gaji_karyawan WHERE NOMOR LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "GJ-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "GJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "GJ-" & cekTanggal & "0001"
        End If

        LblNomor.Text = UrutKOde

    End Sub

    Private Sub MuatComboBoxBulanTahun()
        ' Bersihkan item sebelum menambahkannya kembali
        CmbTahun.Items.Clear()

        ' Tambahkan tahun dari 2022 hingga tahun sekarang
        For i As Integer = 2022 To Year(Now)
            CmbTahun.Items.Add(i)
        Next

        ' Set tahun sekarang sebagai tahun default
        CmbTahun.SelectedItem = Year(Now)

        ' Bersihkan item sebelum menambahkannya kembali
        CmbBulan.Items.Clear()

        ' Tambahkan daftar bulan
        Dim daftarBulan As String() = {"Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember"}
        CmbBulan.Items.AddRange(daftarBulan)

        ' Set bulan sekarang sebagai bulan default
        CmbBulan.SelectedIndex = Month(Now) - 1 ' Index bulan dimulai dari 0, jadi dikurangi 1
    End Sub


    Private Sub CmbBulan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBulan.SelectedIndexChanged, CmbTahun.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
        Ambildataperiodekerja()
    End Sub

    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBulan.Text) Then
            teksBulanTahunTerpilih = CmbBulan.Text & "/" & CmbTahun.Text
            SetupDataGridView()
            TampilkanDataGaji(teksBulanTahunTerpilih)
        End If
    End Sub

    Private Sub AmbilDataKaryawan()
        CmbNama.Items.Clear()
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan ORDER BY Nama ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        CmbNama.Items.Add(rd("Nama").ToString())
                    End While
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim namaAkunD As String = CmbRekening.Text

        Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblRekening.Text = reader("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub

    ' Variabel global untuk menyimpan data gaji
    Dim bonusSupir As Decimal = 0
    Dim bonusHelper As Decimal = 0
    Dim bonusLembur As Decimal = 0
    Dim potonganAbsen As Decimal = 0
    Dim potonganAbsenKhusus As Decimal = 0
    Dim potonganTerlambat As Decimal = 0
    Dim bonusTransport As Decimal = 0
    Dim bonusMakan As Decimal = 0
    Dim prosentaseKomisi As Decimal = 0
    Dim hariKerja As Integer = 30
    Dim jenisPotongan As String = ""


    Private Sub AmbildataMasterGaji()
        Dim query As String = "SELECT Hari_kerja, Prosentase_komisi, Bonus_Supir, Bonus_Helper, Bonus_Transport, Bonus_makan, " &
                               "Bonus_Lembur, Jenis_Potongan, Potongan_Absen, Potongan_Absen_Khusus, Potongan_Terlambat " &
                               "FROM tbl_gaji"

        Using cmd As New MySqlCommand(query, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Mengambil data dari reader
                    If Not Convert.IsDBNull(reader("Hari_kerja")) Then
                        hariKerja = Convert.ToInt32(reader("Hari_kerja"))
                    End If

                    If Not Convert.IsDBNull(reader("Prosentase_komisi")) Then
                        prosentaseKomisi = Convert.ToDecimal(reader("Prosentase_komisi"))
                    End If

                    If Not Convert.IsDBNull(reader("Bonus_Supir")) Then
                        bonusSupir = Convert.ToDecimal(reader("Bonus_Supir"))
                    End If

                    If Not Convert.IsDBNull(reader("Bonus_Helper")) Then
                        bonusHelper = Convert.ToDecimal(reader("Bonus_Helper"))
                    End If

                    If Not Convert.IsDBNull(reader("Bonus_Lembur")) Then
                        bonusLembur = Convert.ToDecimal(reader("Bonus_Lembur"))
                    End If

                    If Not Convert.IsDBNull(reader("Potongan_Absen")) Then
                        potonganAbsen = Convert.ToDecimal(reader("Potongan_Absen"))
                    End If

                    If Not Convert.IsDBNull(reader("Potongan_Absen_Khusus")) Then
                        potonganAbsenKhusus = Convert.ToDecimal(reader("Potongan_Absen_Khusus"))
                    End If

                    If Not Convert.IsDBNull(reader("Potongan_Terlambat")) Then
                        potonganTerlambat = Convert.ToDecimal(reader("Potongan_Terlambat"))
                    End If

                    If Not Convert.IsDBNull(reader("Bonus_Transport")) Then
                        bonusTransport = Convert.ToDecimal(reader("Bonus_Transport"))
                    End If

                    If Not Convert.IsDBNull(reader("Bonus_makan")) Then
                        bonusMakan = Convert.ToDecimal(reader("Bonus_makan"))
                    End If

                    If Not Convert.IsDBNull(reader("Jenis_Potongan")) Then
                        jenisPotongan = Convert.ToString(reader("Jenis_Potongan"))
                    End If

                Else
                    ' Jika data tidak ditemukan, atur nilai variabel menjadi default
                    hariKerja = 30
                    prosentaseKomisi = 0
                    bonusSupir = 0
                    bonusHelper = 0
                    bonusLembur = 0
                    potonganAbsen = 0
                    potonganAbsenKhusus = 0
                    potonganTerlambat = 0
                    bonusTransport = 0
                    bonusMakan = 0
                    jenisPotongan = ""
                End If
            End Using
        End Using


        ' Menampilkan data 
        TxtTanggal.Text = TANGGAL_TUTUP_BULAN.ToString()
    End Sub

    Private Sub CmbNama_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbNama.SelectedIndexChanged
        Dim sql As String = "SELECT Kode, Gaji FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbNama.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKode.Text = reader("Kode").ToString()
                    TxtPokok.Text = Decimal.Parse(reader("Gaji").ToString()).ToString("N0")

                Else
                    LblKode.Text = ""
                    TxtPokok.Text = "0"
                End If
            End Using
        End Using

        AmbilSaldoBon()
        AmbilDataSupirDanHelper()
        AmbilDataOmsetPenjualan()

        TxtTransport.Text = bonusTransport.ToString("N0")
        TxtMakan.Text = bonusMakan.ToString("N0")
        LblKomisJual.Text = "Komisi jual " & prosentaseKomisi.ToString("N0") & " % :"
        LblLembur.Text = "Lembur " & bonusLembur.ToString("N0") & " :"
        LblAbsenkhusus.Text = "Abs Khusus " & potonganAbsenKhusus.ToString("N0") & " :"
        LblTelat.Text = "Telat " & potonganTerlambat.ToString("N0") & " :"

        If jenisPotongan = "Manual" Then
            TxtPotAbsen.Text = Math.Round(Convert.ToDecimal(potonganAbsen), 0, MidpointRounding.AwayFromZero).ToString("N0")
            LblAbsen.Text = "Absen " & Math.Round(Convert.ToDecimal(potonganAbsen), 0, MidpointRounding.AwayFromZero).ToString("N0") & " :"
        Else
            Dim potAbsen As Decimal = Convert.ToDecimal(TxtPokok.Text) / hariKerja
            TxtPotAbsen.Text = Math.Round(potAbsen, 0, MidpointRounding.AwayFromZero).ToString("N0")
            LblAbsen.Text = "Absen " & Math.Round(potAbsen, 0, MidpointRounding.AwayFromZero).ToString("N0") & " :"
        End If
    End Sub

    Private Sub AmbilSaldoBon()
        Dim sql As String = "SELECT SaldoAkhir FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbNama.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    Dim saldoAkhir As Decimal
                    ' Periksa apakah nilai adalah DBNull
                    If IsDBNull(reader("SaldoAkhir")) Then
                        saldoAkhir = 0
                    ElseIf Decimal.TryParse(reader("SaldoAkhir").ToString(), saldoAkhir) Then
                        ' Jika parsing berhasil
                    Else
                        saldoAkhir = 0
                    End If
                    ' Set label dengan nilai yang sudah diperiksa
                    LblSaldoBon.Text = saldoAkhir.ToString("N0")
                Else
                    ' Jika tidak ada baris yang dikembalikan, set saldo ke 0
                    LblSaldoBon.Text = 0
                End If
            End Using
        End Using
    End Sub


    Private Sub TxtTanggal_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtTanggal.TextChanged
        Ambildataperiodekerja()
    End Sub

    Private Sub Ambildataperiodekerja()
        Dim tanggal As Integer
        If Integer.TryParse(TxtTanggal.Text, tanggal) Then
            If tanggal >= 1 AndAlso tanggal <= 28 Then
                ' Dapatkan bulan dan tahun sekarang dari combo box
                Dim bulanSekarang As Integer = CmbBulan.SelectedIndex + 1
                Dim tahunSekarang As Integer = Integer.Parse(CmbTahun.Text)

                If JENIS_TUTUP_BULAN = "Berdasar bulan kalender" Then
                    ' Atur DtpAwal dengan tanggal 11 bulan sebelumnya
                    Dim bulanAwal As Integer = If(bulanSekarang = 1, 12, bulanSekarang - 1)
                    Dim tahunAwal As Integer = If(bulanSekarang = 1, tahunSekarang - 1, tahunSekarang)
                    Dim tanggalAwal As New Date(tahunAwal, bulanAwal, tanggal + 1)
                    DtpAwal.Value = tanggalAwal

                    ' Atur DtpAkhir dengan tanggal 10 bulan sekarang
                    Dim bulanAkhir As Integer = bulanSekarang
                    Dim tahunAkhir As Integer = tahunSekarang
                    Dim tanggalAkhir As New Date(tahunAkhir, bulanAkhir, tanggal)
                    DtpAkhir.Value = tanggalAkhir
                Else
                    ' Logika jika bukan "Berdasar bulan kalender"
                    ' Atur DtpAwal dengan tanggal 11 bulan sebelumnya
                    Dim bulanAwal As Integer = If(bulanSekarang = 1, 12, bulanSekarang - 1)
                    Dim tahunAwal As Integer = If(bulanSekarang = 1, tahunSekarang - 1, tahunSekarang)
                    DtpAwal.Value = New Date(tahunAwal, bulanAwal, tanggal + 1)

                    ' Atur DtpAkhir dengan tanggal 10 bulan sekarang
                    DtpAkhir.Value = New Date(tahunSekarang, bulanSekarang, tanggal)
                End If
            End If
        End If
    End Sub

    Private Sub DtpAwal_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DtpAwal.ValueChanged, DtpAkhir.ValueChanged
        ' Panggil method untuk mengambil data supir dan helper
        AmbilDataSupirDanHelper()
        AmbilDataOmsetPenjualan()
    End Sub


    Private Sub AmbilDataSupirDanHelper()
        Dim tanggalAwal As Date = DtpAwal.Value.Date
        Dim tanggalAkhir As Date = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)

        Dim sqlSupir As String = "SELECT Count(NOTA) AS NOTA FROM Surat_Jalan WHERE KODE_SUPIR = @KODE_SUPIR AND TGL_PENGIRIMAN >= @tanggalAwal AND TGL_PENGIRIMAN <= @tanggalAkhir"
        Using cmdSupir As New MySqlCommand(sqlSupir, conn)
            cmdSupir.Parameters.AddWithValue("@KODE_SUPIR", LblKode.Text)
            cmdSupir.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdSupir.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = cmdSupir.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblSupir.Text = reader("NOTA").ToString()
                    LblKetSupir.Text = "Supir " & reader("NOTA").ToString() & " x " & bonusSupir.ToString("N0") & " :"
                Else
                    LblSupir.Text = "0"
                    LblKetSupir.Text = "Supir 0 x 0 :"
                End If
            End Using
        End Using

        Dim sqlHelper As String = "SELECT Count(NOTA) AS NOTA FROM Surat_Jalan WHERE (KODE_HELPER1 = @KODE_HELPER1 OR KODE_HELPER2 = @KODE_HELPER2) AND TGL_PENGIRIMAN >= @tanggalAwal AND TGL_PENGIRIMAN <= @tanggalAkhir"
        Using cmdHelper As New MySqlCommand(sqlHelper, conn)
            cmdHelper.Parameters.AddWithValue("@KODE_HELPER1", LblKode.Text)
            cmdHelper.Parameters.AddWithValue("@KODE_HELPER2", LblKode.Text)
            cmdHelper.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHelper.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using reader As MySqlDataReader = cmdHelper.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblHelper.Text = reader("NOTA").ToString()
                    LblKetHelp.Text = "Helper " & reader("NOTA").ToString() & " x " & bonusHelper.ToString("N0") & " :"
                Else
                    LblHelper.Text = "0"
                    LblKetHelp.Text = "Helper 0 x 0 :"
                End If
            End Using
        End Using

    End Sub

    Private Sub AmbilDataOmsetPenjualan()
        ' Ambil tanggal awal dan akhir dari DateTimePicker
        Dim tanggalAwal As Date = DtpAwal.Value.Date
        Dim tanggalAkhir As Date = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)

        ' Query untuk mendapatkan total omset penjualan
        Dim sqlQuery As String = "SELECT SUM(GRAND_TOTAL_STL_PAJAK) AS TotalOmset FROM penjualan WHERE ID_SALES = @ID_SALES AND TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            ' Menambahkan parameter ke query
            cmd.Parameters.AddWithValue("@ID_SALES", LblKode.Text)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                ' Mengecek apakah ada hasil yang dikembalikan
                If reader.HasRows Then
                    reader.Read()
                    ' Mengambil nilai total omset dari reader
                    Dim totalOmset As Object = reader("TotalOmset")
                    TxtOmsetJual.Text = If(totalOmset IsNot DBNull.Value, Convert.ToDecimal(totalOmset).ToString("N0"), "0")
                Else
                    TxtOmsetJual.Text = "0"
                End If
            End Using
        End Using
    End Sub


    Private Sub HitungPendapatan()
        ' Konversi nilai dari setiap TextBox ke tipe Decimal, jika kosong atau null maka 0
        Dim pokok As Decimal = If(Decimal.TryParse(TxtPokok.Text, pokok), pokok, 0D)
        Dim komisijual As Decimal = If(Decimal.TryParse(TxtKomisiJual.Text, komisijual), komisijual, 0D)
        Dim supir As Decimal = If(Decimal.TryParse(TxtSupir.Text, supir), supir, 0D)
        Dim helper As Decimal = If(Decimal.TryParse(TxtHelper.Text, helper), helper, 0D)
        Dim lemburRp As Decimal = If(Decimal.TryParse(TxtLemburRp.Text, lemburRp), lemburRp, 0D)
        Dim tunjangan As Decimal = If(Decimal.TryParse(TxtTunjangan.Text, tunjangan), tunjangan, 0D)
        Dim transport As Decimal = If(Decimal.TryParse(TxtTransport.Text, transport), transport, 0D)
        Dim makan As Decimal = If(Decimal.TryParse(TxtMakan.Text, makan), makan, 0D)

        ' Hitung total pendapatan
        Dim pendapatan As Decimal = pokok + komisijual + supir + helper + lemburRp + tunjangan + transport + makan

        ' Tampilkan total pendapatan dalam format tanpa tempat desimal
        TxtPendapatan.Text = pendapatan.ToString("N0")
    End Sub


    Private Sub TxtPokok_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtPokok.TextChanged, TxtKomisiJual.TextChanged, TxtSupir.TextChanged, TxtHelper.TextChanged, TxtLemburRp.TextChanged, TxtTunjangan.TextChanged, TxtTransport.TextChanged, TxtMakan.TextChanged
        HitungPendapatan()
    End Sub

    Private Sub HitungPotongan()
        ' Konversi nilai dari setiap TextBox ke tipe Decimal, jika null atau kosong maka 0
        Dim bon As Decimal = If(Decimal.TryParse(TxtPotBon.Text, bon), bon, 0D)
        Dim angsuran As Decimal = If(Decimal.TryParse(TxtAngsuran.Text, angsuran), angsuran, 0D)
        Dim absen As Decimal = If(Decimal.TryParse(TxtAbsenRp.Text, absen), absen, 0D)
        Dim absenKhusus As Decimal = If(Decimal.TryParse(TxtAbsenKhususRp.Text, absenKhusus), absenKhusus, 0D)
        Dim keterlambatan As Decimal = If(Decimal.TryParse(TxtKeterlambatanRp.Text, keterlambatan), keterlambatan, 0D)
        Dim potlain As Decimal = If(Decimal.TryParse(TxtPotLain.Text, potlain), potlain, 0D)

        ' Hitung total potongan
        Dim potongan As Decimal = bon + angsuran + absen + absenKhusus + keterlambatan + potlain

        ' Tampilkan total potongan dalam format tanpa tempat desimal
        TxtPotongan.Text = potongan.ToString("N0")
    End Sub


    Private Sub TxtPotBon_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtPotBon.TextChanged, TxtAngsuran.TextChanged, TxtAbsenRp.TextChanged, TxtAbsenKhususRp.TextChanged, TxtKeterlambatanRp.TextChanged, TxtPotLain.TextChanged
        HitungPotongan()
    End Sub

    Private Sub TxtPendapatan_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtPendapatan.TextChanged, TxtPotongan.TextChanged
        ' Konversi nilai dari TextBox ke tipe Decimal, jika null atau kosong maka 0
        Dim pendapatan As Decimal = If(Decimal.TryParse(TxtPendapatan.Text, pendapatan), pendapatan, 0D)
        Dim potongan As Decimal = If(Decimal.TryParse(TxtPotongan.Text, potongan), potongan, 0D)

        ' Hitung total penerimaan
        Dim penerimaan As Decimal = pendapatan - potongan

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtTerima.Text = penerimaan.ToString("N0")
    End Sub




    Private Sub TxtOmsetJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtOmsetJual.TextChanged
        ' Konversi nilai dari TextBox, jika kosong atau null, maka 0
        Dim OmsetPenjualan As Decimal = If(Decimal.TryParse(TxtOmsetJual.Text, OmsetPenjualan), OmsetPenjualan, 0D)

        ' Hitung total penerimaan dan bulatkan tanpa koma
        Dim KomisiJual As Decimal = Math.Round(OmsetPenjualan * (prosentaseKomisi / 100), 0, MidpointRounding.AwayFromZero)

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtKomisiJual.Text = KomisiJual.ToString("N0")
    End Sub


    Private Sub LblSupir_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblSupir.TextChanged
        ' Konversi nilai dari TextBox, jika kosong atau null, maka 0
        Dim Supir As Decimal = If(Decimal.TryParse(LblSupir.Text, Supir), Supir, 0D)

        ' Hitung total penerimaan
        Dim NilaiSupir As Decimal = Supir * bonusSupir

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtSupir.Text = NilaiSupir.ToString("N0")
    End Sub


    Private Sub LblHelper_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblHelper.TextChanged
        ' Konversi nilai dari TextBox, jika kosong atau null, maka 0
        Dim helper As Decimal = If(Decimal.TryParse(LblHelper.Text, helper), helper, 0D)

        ' Hitung total penerimaan
        Dim NilaiHelper As Decimal = helper * bonusHelper

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtHelper.Text = NilaiHelper.ToString("N0")
    End Sub


    Private Sub TxtPotBon_TextChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblSaldoBon.TextChanged, TxtPotBon.TextChanged, TxtAngsuran.TextChanged
        ' Konversi nilai dari setiap TextBox ke tipe Decimal, jika kosong atau null maka 0
        Dim SaldoBon As Decimal = If(Decimal.TryParse(LblSaldoBon.Text, SaldoBon), SaldoBon, 0D)
        Dim PotonganBon As Decimal = If(Decimal.TryParse(TxtPotBon.Text, PotonganBon), PotonganBon, 0D)
        Dim PotonganBonKhusus As Decimal = If(Decimal.TryParse(TxtAngsuran.Text, PotonganBonKhusus), PotonganBonKhusus, 0D)

        ' Hitung total sisa bon
        Dim SisaBon As Decimal = SaldoBon - PotonganBon - PotonganBonKhusus

        ' Tampilkan total sisa bon dalam format tanpa tempat desimal
        LblSisaBon.Text = SisaBon.ToString("N0")
    End Sub


    Private Sub TxtLembur_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtLembur.TextChanged
        ' Konversi nilai dari TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim lembur As Decimal = If(Decimal.TryParse(TxtLembur.Text, lembur), lembur, 0D)

        ' Hitung total nilai lembur
        Dim Nilailembur As Decimal = lembur * bonusLembur

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtLemburRp.Text = Nilailembur.ToString("N0")
    End Sub


    Private Sub TxtAbsen_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtAbsen.TextChanged
        ' Konversi nilai dari setiap TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim absen As Decimal = If(Decimal.TryParse(TxtAbsen.Text, absen), absen, 0D)
        Dim nilaiPotAbse As Decimal = If(Decimal.TryParse(TxtPotAbsen.Text, nilaiPotAbse), nilaiPotAbse, 0D)

        ' Hitung total penerimaan
        Dim Nilaiabsen As Decimal = absen * nilaiPotAbse

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtAbsenRp.Text = Nilaiabsen.ToString("N0")
    End Sub



    Private Sub TxtAbsenKhusus_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtAbsenKhusus.TextChanged
        ' Konversi nilai dari setiap TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim AbsenKhusus As Decimal = If(Decimal.TryParse(TxtAbsenKhusus.Text, AbsenKhusus), AbsenKhusus, 0D)

        ' Hitung total penerimaan
        Dim NilaiAbsenKhusus As Decimal = AbsenKhusus * potonganAbsenKhusus

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtAbsenKhususRp.Text = NilaiAbsenKhusus.ToString("N0")
    End Sub


    Private Sub TxtKeterlambatan_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtKeterlambatan.TextChanged
        ' Konversi nilai dari setiap TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim Keterlambatan As Decimal = If(Decimal.TryParse(TxtKeterlambatan.Text, Keterlambatan), Keterlambatan, 0D)

        ' Hitung total penerimaan
        Dim NilaiKeterlambatan As Decimal = Keterlambatan * potonganTerlambat

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtKeterlambatanRp.Text = NilaiKeterlambatan.ToString("N0")
    End Sub


    Private Sub TampilkanDataGaji(ByVal teksBulanTahunTerpilih As String)
        DGVGaji.Rows.Clear()

        Try
            Dim query As String = "SELECT NOMOR, BULAN, TANGGAL, TANGGALAWAL, TANGGALAKHIR, KODE, Nama, " &
                          "POKOK, OMSET_JUAL, KOMISI_JUAL, SUPIR, SUPIR_RP, HELPER, HELPER_RP, " &
                          "LEMBUR, LEMBUR_RP, TUNJANGAN, TRANSPORT, UANG_MAKAN, SALDO_BON, POT_BON, ANGSURAN, " &
                          "ABSEN, ABSEN_RP, ABSEN_KHUSUS, ABSEN_KHUSUS_RP, TERLAMBAT, TERLAMBAT_RP, " &
                          "POT_LAIN, PENDAPATAN, POTONGAN, TERIMA, REKENING, ID_USER " &
                          "FROM Gaji_karyawan WHERE BULAN LIKE ?"

            Using cmd As New MySqlCommand(query, conn)
                ' Mengisi parameter-parameter pada perintah SQL
                cmd.Parameters.AddWithValue("@BULAN", teksBulanTahunTerpilih & "%")

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ' Mengosongkan DataGridView sebelum menambahkan data baru
                    DGVGaji.Rows.Clear()

                    While reader.Read()
                        ' Mengambil data dari reader dengan memeriksa DBNull
                        Dim nomor As String = If(IsDBNull(reader("NOMOR")), String.Empty, reader("NOMOR").ToString())
                        Dim bulan As String = If(IsDBNull(reader("BULAN")), String.Empty, reader("BULAN").ToString())
                        Dim tanggal As DateTime = If(IsDBNull(reader("TANGGAL")), DateTime.MinValue, Convert.ToDateTime(reader("TANGGAL")))
                        Dim tanggalAwal As DateTime = If(IsDBNull(reader("TANGGALAWAL")), DateTime.MinValue, Convert.ToDateTime(reader("TANGGALAWAL")))
                        Dim tanggalAkhir As DateTime = If(IsDBNull(reader("TANGGALAKHIR")), DateTime.MinValue, Convert.ToDateTime(reader("TANGGALAKHIR")))
                        Dim kode As String = If(IsDBNull(reader("KODE")), String.Empty, reader("KODE").ToString())
                        Dim nama As String = If(IsDBNull(reader("Nama")), String.Empty, reader("Nama").ToString())
                        Dim pokok As Decimal = If(IsDBNull(reader("POKOK")), 0D, Convert.ToDecimal(reader("POKOK")))
                        Dim omsetJual As Decimal = If(IsDBNull(reader("OMSET_JUAL")), 0D, Convert.ToDecimal(reader("OMSET_JUAL")))
                        Dim komisiJual As Decimal = If(IsDBNull(reader("KOMISI_JUAL")), 0D, Convert.ToDecimal(reader("KOMISI_JUAL")))
                        Dim supir As Decimal = If(IsDBNull(reader("SUPIR")), 0D, Convert.ToDecimal(reader("SUPIR")))
                        Dim supirRp As Decimal = If(IsDBNull(reader("SUPIR_RP")), 0D, Convert.ToDecimal(reader("SUPIR_RP")))
                        Dim helper As Decimal = If(IsDBNull(reader("HELPER")), 0D, Convert.ToDecimal(reader("HELPER")))
                        Dim helperRp As Decimal = If(IsDBNull(reader("HELPER_RP")), 0D, Convert.ToDecimal(reader("HELPER_RP")))
                        Dim lembur As Decimal = If(IsDBNull(reader("LEMBUR")), 0D, Convert.ToDecimal(reader("LEMBUR")))
                        Dim lemburRp As Decimal = If(IsDBNull(reader("LEMBUR_RP")), 0D, Convert.ToDecimal(reader("LEMBUR_RP")))
                        Dim tunjangan As Decimal = If(IsDBNull(reader("TUNJANGAN")), 0D, Convert.ToDecimal(reader("TUNJANGAN")))
                        Dim transport As Decimal = If(IsDBNull(reader("TRANSPORT")), 0D, Convert.ToDecimal(reader("TRANSPORT")))
                        Dim uangMakan As Decimal = If(IsDBNull(reader("UANG_MAKAN")), 0D, Convert.ToDecimal(reader("UANG_MAKAN")))
                        Dim saldobon As Decimal = If(IsDBNull(reader("SALDO_BON")), 0D, Convert.ToDecimal(reader("SALDO_BON")))
                        Dim potBon As Decimal = If(IsDBNull(reader("POT_BON")), 0D, Convert.ToDecimal(reader("POT_BON")))
                        Dim angsuran As Decimal = If(IsDBNull(reader("ANGSURAN")), 0D, Convert.ToDecimal(reader("ANGSURAN")))
                        Dim absen As Decimal = If(IsDBNull(reader("ABSEN")), 0D, Convert.ToDecimal(reader("ABSEN")))
                        Dim absenRp As Decimal = If(IsDBNull(reader("ABSEN_RP")), 0D, Convert.ToDecimal(reader("ABSEN_RP")))
                        Dim absenKhusus As Decimal = If(IsDBNull(reader("ABSEN_KHUSUS")), 0D, Convert.ToDecimal(reader("ABSEN_KHUSUS")))
                        Dim absenKhususRp As Decimal = If(IsDBNull(reader("ABSEN_KHUSUS_RP")), 0D, Convert.ToDecimal(reader("ABSEN_KHUSUS_RP")))
                        Dim terlambat As Decimal = If(IsDBNull(reader("TERLAMBAT")), 0D, Convert.ToDecimal(reader("TERLAMBAT")))
                        Dim terlambatRp As Decimal = If(IsDBNull(reader("TERLAMBAT_RP")), 0D, Convert.ToDecimal(reader("TERLAMBAT_RP")))
                        Dim potLain As Decimal = If(IsDBNull(reader("POT_LAIN")), 0D, Convert.ToDecimal(reader("POT_LAIN")))
                        Dim pendapatan As Decimal = If(IsDBNull(reader("PENDAPATAN")), 0D, Convert.ToDecimal(reader("PENDAPATAN")))
                        Dim potongan As Decimal = If(IsDBNull(reader("POTONGAN")), 0D, Convert.ToDecimal(reader("POTONGAN")))
                        Dim terima As Decimal = If(IsDBNull(reader("TERIMA")), 0D, Convert.ToDecimal(reader("TERIMA")))
                        Dim rekening As String = If(IsDBNull(reader("REKENING")), String.Empty, reader("REKENING").ToString())
                        Dim idUser As String = If(IsDBNull(reader("ID_USER")), String.Empty, reader("ID_USER").ToString())

                        ' Menambahkan data ke DataGridView
                        DGVGaji.Rows.Add(nomor, bulan, tanggal.ToString("yyyy-MM-dd HH:mm:ss"), tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"), tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"), kode, nama,
                                         pokok.ToString("N0"), omsetJual.ToString("N0"), komisiJual.ToString("N0"), supir.ToString("N0"), supirRp.ToString("N0"),
                                         helper.ToString("N0"), helperRp.ToString("N0"), lembur.ToString("N0"), lemburRp.ToString("N0"), tunjangan.ToString("N0"),
                                         transport.ToString("N0"), uangMakan.ToString("N0"), saldobon.ToString("N0"), potBon.ToString("N0"), angsuran.ToString("N0"), absen.ToString("N0"),
                                         absenRp.ToString("N0"), absenKhusus.ToString("N0"), absenKhususRp.ToString("N0"), terlambat.ToString("N0"),
                                         terlambatRp.ToString("N0"), potLain.ToString("N0"), pendapatan.ToString("N0"), potongan.ToString("N0"),
                                         terima.ToString("N0"), rekening, idUser)
                    End While

                    DGVGaji.ClearSelection()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub SetupDataGridView()
        ' Bersihkan semua kolom sebelum menambahkan kolom baru
        DGVGaji.Columns.Clear()

        ' Tambahkan kolom-kolom baru ke DataGridView
        ' Contoh kolom untuk data
        DGVGaji.Columns.Add("Nomor", "Nomor")
        DGVGaji.Columns.Add("Bulan", "Bulan")
        DGVGaji.Columns.Add("Tanggal", "Tanggal")
        DGVGaji.Columns.Add("TanggalAwal", "Tanggal Awal")
        DGVGaji.Columns.Add("TanggalAkhir", "Tanggal Akhir")
        DGVGaji.Columns.Add("Kode", "Kode")
        DGVGaji.Columns.Add("Nama", "Nama")
        DGVGaji.Columns.Add("Pokok", "Pokok")
        DGVGaji.Columns.Add("OmsetJual", "Omset Jual")
        DGVGaji.Columns.Add("KomisiJual", "Komisi Jual")
        DGVGaji.Columns.Add("Supir", "Supir")
        DGVGaji.Columns.Add("SupirRp", "Supir Rp")
        DGVGaji.Columns.Add("Helper", "Helper")
        DGVGaji.Columns.Add("HelperRp", "Helper Rp")
        DGVGaji.Columns.Add("Lembur", "Lembur")
        DGVGaji.Columns.Add("LemburRp", "Lembur Rp")
        DGVGaji.Columns.Add("Tunjangan", "Tunjangan")
        DGVGaji.Columns.Add("Transport", "Transport")
        DGVGaji.Columns.Add("UangMakan", "Uang Makan")
        DGVGaji.Columns.Add("SaldoBon", "Bon")
        DGVGaji.Columns.Add("PotBon", "Pot Bon")
        DGVGaji.Columns.Add("Angsuran", "Angsuran")
        DGVGaji.Columns.Add("Absen", "Absen")
        DGVGaji.Columns.Add("AbsenRp", "Absen Rp")
        DGVGaji.Columns.Add("AbsenKhusus", "Absen Khusus")
        DGVGaji.Columns.Add("AbsenKhususRp", "Absen Khusus Rp")
        DGVGaji.Columns.Add("Terlambat", "Terlambat")
        DGVGaji.Columns.Add("TerlambatRp", "Terlambat Rp")
        DGVGaji.Columns.Add("PotLain", "Pot Lain")
        DGVGaji.Columns.Add("Pendapatan", "Pendapatan")
        DGVGaji.Columns.Add("Potongan", "Potongan")
        DGVGaji.Columns.Add("Terima", "Terima")
        DGVGaji.Columns.Add("Rekening", "Rekening")
        DGVGaji.Columns.Add("IdUser", "ID User")

        ' Kolom untuk tombol Edit
        Dim btnEdit As New DataGridViewButtonColumn() With {
    .Name = "BtnEdit",
    .HeaderText = "Edit",
    .Text = "Edit",
    .UseColumnTextForButtonValue = True
}
        DGVGaji.Columns.Add(btnEdit)

        ' Kolom untuk tombol Hapus
        Dim btnHapus As New DataGridViewButtonColumn() With {
    .Name = "BtnHapus",
    .HeaderText = "Hapus",
    .Text = "Hapus",
    .UseColumnTextForButtonValue = True
}
        DGVGaji.Columns.Add(btnHapus)

        ' Kolom untuk tombol Cetak
        Dim btnCetak As New DataGridViewButtonColumn() With {
    .Name = "BtnCetak",
    .HeaderText = "Cetak",
    .Text = "Cetak",
    .UseColumnTextForButtonValue = True
}
        DGVGaji.Columns.Add(btnCetak)


        ' Terapkan hak akses ke tombol-tombol
        Dim GAJI As Boolean() = ModulHakAkses.BacaHakAksesDariCache("GAJI")
        DGVGaji.Columns("BtnEdit").Visible = GAJI(2) ' CanEdit 
        DGVGaji.Columns("BtnHapus").Visible = GAJI(3) ' CanDelete 

        ' Mengatur format dan alignment
        FormatDGVGaji()

        ' Menyembunyikan kolom yang ada
        DGVGaji.Columns("Nomor").Visible = False
        DGVGaji.Columns("Tanggal").Visible = False
        DGVGaji.Columns("Kode").Visible = False
        DGVGaji.Columns("TanggalAwal").Visible = False
        DGVGaji.Columns("TanggalAkhir").Visible = False
        DGVGaji.Columns("OmsetJual").Visible = False
        DGVGaji.Columns("Supir").Visible = False
        DGVGaji.Columns("SupirRp").Visible = False
        DGVGaji.Columns("Helper").Visible = False
        DGVGaji.Columns("HelperRp").Visible = False
        DGVGaji.Columns("Lembur").Visible = False
        'DGVGaji.Columns("LemburRp").Visible = False
        'DGVGaji.Columns("Tunjangan").Visible = False
        DGVGaji.Columns("Transport").Visible = False
        DGVGaji.Columns("UangMakan").Visible = False
        DGVGaji.Columns("SaldoBon").Visible = False
        'DGVGaji.Columns("Angsuran").Visible = False
        DGVGaji.Columns("Absen").Visible = False
        'DGVGaji.Columns("AbsenRp").Visible = False
        DGVGaji.Columns("AbsenKhusus").Visible = False
        DGVGaji.Columns("AbsenKhususRp").Visible = False
        DGVGaji.Columns("Terlambat").Visible = False
        DGVGaji.Columns("Rekening").Visible = False
        DGVGaji.Columns("PotLain").Visible = False
        DGVGaji.Columns("IdUser").Visible = False

    End Sub

    Private Sub FormatDGVGaji()
        ' Daftar nama kolom yang akan diformat
        Dim columnsToFormat As String() = {"Pokok", "OmsetJual", "KomisiJual", "SupirRp", "HelperRp",
                                            "LemburRp", "Tunjangan", "Transport", "UangMakan", "SaldoBon", "PotBon",
                                            "Angsuran", "AbsenRp", "AbsenKhususRp", "TerlambatRp",
                                            "PotLain", "Pendapatan", "Potongan", "Terima"}

        ' Format kolom di DataGridView
        For Each columnName As String In columnsToFormat
            If DGVGaji.Columns.Contains(columnName) Then
                Dim column As DataGridViewColumn = DGVGaji.Columns(columnName)
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                column.DefaultCellStyle.Format = "N0" ' Format tanpa desimal
            End If
        Next

        With DGVGaji
            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(DGVGaji)
        End With
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
    End Sub

    Private Sub DGVGaji_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DGVGaji.CellContentClick
        ' Pastikan tombol Edit yang diklik
        If e.RowIndex >= 0 AndAlso DGVGaji.Columns(e.ColumnIndex).Name = "BtnEdit" Then
            ' Validasi baris tidak kosong
            If Not DGVGaji.Rows(e.RowIndex).IsNewRow Then
                ' Memasukkan data dari DataGridView ke TextBox
                LblNomor.Text = DGVGaji.Rows(e.RowIndex).Cells("Nomor").Value.ToString()
                DtpTanggal.Value = Convert.ToDateTime(DGVGaji.Rows(e.RowIndex).Cells("Tanggal").Value)
                DtpAwal.Value = Convert.ToDateTime(DGVGaji.Rows(e.RowIndex).Cells("TanggalAwal").Value)
                DtpAkhir.Value = Convert.ToDateTime(DGVGaji.Rows(e.RowIndex).Cells("TanggalAkhir").Value)
                CmbNama.Text = DGVGaji.Rows(e.RowIndex).Cells("Nama").Value.ToString()
                TxtPokok.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Pokok").Value).ToString("N0")
                TxtOmsetJual.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("OmsetJual").Value).ToString("N0")
                TxtKomisiJual.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("KomisiJual").Value).ToString("N0")
                LblSupir.Text = DGVGaji.Rows(e.RowIndex).Cells("Supir").Value.ToString()
                TxtSupir.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("SupirRp").Value).ToString("N0")
                LblHelper.Text = DGVGaji.Rows(e.RowIndex).Cells("Helper").Value.ToString()
                TxtHelper.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("HelperRp").Value).ToString("N0")
                TxtLembur.Text = DGVGaji.Rows(e.RowIndex).Cells("Lembur").Value.ToString()
                TxtLemburRp.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("LemburRp").Value).ToString("N0")
                TxtTunjangan.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Tunjangan").Value).ToString("N0")
                TxtTransport.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Transport").Value).ToString("N0")
                TxtMakan.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("UangMakan").Value).ToString("N0")
                LblSaldoBon.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("SaldoBon").Value).ToString("N0")
                TxtPotBon.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("PotBon").Value).ToString("N0")
                TxtAngsuran.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Angsuran").Value).ToString("N0")
                TxtAbsen.Text = DGVGaji.Rows(e.RowIndex).Cells("Absen").Value.ToString()
                TxtAbsenRp.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("AbsenRp").Value).ToString("N0")
                TxtAbsenKhusus.Text = DGVGaji.Rows(e.RowIndex).Cells("AbsenKhusus").Value.ToString()
                TxtAbsenKhususRp.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("AbsenKhususRp").Value).ToString("N0")
                TxtKeterlambatan.Text = DGVGaji.Rows(e.RowIndex).Cells("Terlambat").Value.ToString()
                TxtKeterlambatanRp.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("TerlambatRp").Value).ToString("N0")
                TxtPotLain.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("PotLain").Value).ToString("N0")
                TxtPendapatan.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Pendapatan").Value).ToString("N0")
                TxtPotongan.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Potongan").Value).ToString("N0")
                TxtTerima.Text = Convert.ToDecimal(DGVGaji.Rows(e.RowIndex).Cells("Terima").Value).ToString("N0")
                CmbRekening.Text = DGVGaji.Rows(e.RowIndex).Cells("Rekening").Value.ToString()

                ' Konversi nilai dari TxtPotBon dan TxtAngsuran menjadi Decimal dengan default 0 jika tidak valid
                Dim potBon As Decimal = If(Decimal.TryParse(TxtPotBon.Text, potBon), potBon, 0D)
                Dim angsuran As Decimal = If(Decimal.TryParse(TxtAngsuran.Text, angsuran), angsuran, 0D)
                TxtPotBonUntukEdit.Text = (potBon + angsuran).ToString("N0")

                BtnSimpann.Text = "EDIT (F8)"
            Else
                MessageBox.Show("Baris yang dipilih kosong atau tidak valid untuk dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If

        ' Pastikan tombol Hapus yang diklik dan baris tidak kosong
        If e.RowIndex >= 0 AndAlso DGVGaji.Columns(e.ColumnIndex).Name = "BtnHapus" Then
            ' Validasi baris tidak kosong
            If Not DGVGaji.Rows(e.RowIndex).IsNewRow Then
                ' Ambil nomor transaksi dan kode dari baris yang dipilih
                Dim nomorTransaksi As String = DGVGaji.Rows(e.RowIndex).Cells("Nomor").Value.ToString()
                Dim kodeKaryawan As String = DGVGaji.Rows(e.RowIndex).Cells("Kode").Value.ToString()

                ' Hitung total potongan bon dan angsuran
                Dim potonganBon As Decimal = Decimal.Parse(DGVGaji.Rows(e.RowIndex).Cells("PotBon").Value.ToString().Replace(".", "")) _
                                            + Decimal.Parse(DGVGaji.Rows(e.RowIndex).Cells("Angsuran").Value.ToString().Replace(".", ""))

                ' Konfirmasi penghapusan kepada pengguna
                Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    Dim transaction As MySqlTransaction = conn.BeginTransaction()
                    Try
                        ' Hapus data gaji karyawan berdasarkan nomor transaksi
                        Dim queryHapusGaji As String = "DELETE FROM Gaji_karyawan WHERE NOMOR = @NomorTransaksi"
                        Using cmdHapusGaji As New MySqlCommand(queryHapusGaji, conn, transaction)
                            cmdHapusGaji.Parameters.AddWithValue("@NomorTransaksi", nomorTransaksi)
                            cmdHapusGaji.ExecuteNonQuery()
                        End Using

                        ' Hapus data jurnal umum berdasarkan nomor transaksi
                        Dim queryHapusJurnal As String = "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NomorTransaksi"
                        Using cmdHapusJurnal As New MySqlCommand(queryHapusJurnal, conn, transaction)
                            cmdHapusJurnal.Parameters.AddWithValue("@NomorTransaksi", nomorTransaksi)
                            cmdHapusJurnal.ExecuteNonQuery()
                        End Using

                        ' Hapus data bon karyawan berdasarkan faktur
                        Dim queryHapusBon As String = "DELETE FROM Bon_karyawan WHERE FAKTUR = @Faktur"
                        Using cmdHapusBon As New MySqlCommand(queryHapusBon, conn, transaction)
                            cmdHapusBon.Parameters.AddWithValue("@Faktur", nomorTransaksi)
                            cmdHapusBon.ExecuteNonQuery()
                        End Using


                        ' Update total bayar karyawan jika ada potongan bon yang perlu di-edit
                        If potonganBon <> 0 Then
                            Dim queryUpdateBayar As String = "UPDATE tbl_karyawan SET TotalBayar = TotalBayar - ? WHERE Kode = ?"
                            Using cmdUpdateBayar As New MySqlCommand(queryUpdateBayar, conn, transaction)
                                cmdUpdateBayar.Parameters.AddWithValue("@Nominal", potonganBon)
                                cmdUpdateBayar.Parameters.AddWithValue("@Kode", kodeKaryawan)
                                cmdUpdateBayar.ExecuteNonQuery()
                            End Using
                        End If

                        ' Commit transaksi jika berhasil
                        transaction.Commit()
                        ' Reset nilai-nilai pada kontrol setelah berhasil diupdate
                        ResetControls()
                    Catch ex As Exception
                        transaction.Rollback()
                        MessageBox.Show("Terjadi kesalahan saat menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            Else
                MessageBox.Show("Baris yang dipilih kosong atau tidak valid untuk dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If

        ' Deteksi tombol Cetak
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = DGVGaji.Columns("BtnCetak").Index Then
            AmbilDataDanCetak(e)
        End If

    End Sub

    Private Sub AmbilDataDanCetak(ByVal e As DataGridViewCellEventArgs)
        ' Ambil data dari baris yang dipilih
        Dim x As Integer = (ClientSize.Width - Panel2.Width) \ 2
        Dim y As Integer = (ClientSize.Height - Panel2.Height) \ 2
        Panel2.Location = New Point(x, y)
        Panel2.BringToFront()
        Panel2.Visible = True

        Dim selectedRow As DataGridViewRow = DGVGaji.Rows(e.RowIndex)

        ' Ambil data dari setiap kolom yang dibutuhkan
        Dim nomor As String = selectedRow.Cells("Nomor").Value.ToString()

        Dim Penerimaan As Decimal = Convert.ToDecimal(selectedRow.Cells("Terima").Value)
        Dim TerimaTerbilang As String = Terbilang(Penerimaan)


        ' Initialize the DataSet
        Dim ds As New DataSet()

        ' Create the SQL query
        Dim sql As String = "SELECT BULAN, TANGGAL, TANGGALAWAL, TANGGALAKHIR, KODE, Nama, POKOK, OMSET_JUAL, KOMISI_JUAL, " &
                     "SUPIR, SUPIR_RP, HELPER, HELPER_RP, LEMBUR, LEMBUR_RP, TUNJANGAN, TRANSPORT, UANG_MAKAN, SALDO_BON, " &
                     "POT_BON, ANGSURAN, NILAI_POTONGAN_ABSEN, ABSEN, ABSEN_RP, ABSEN_KHUSUS, ABSEN_KHUSUS_RP, TERLAMBAT, TERLAMBAT_RP, POT_LAIN, " &
                     "pendapatan, potongan, terima, REKENING, ID_USER " &
                     "FROM Gaji_karyawan WHERE NOMOR = @NOMOR"


        ' Using block to ensure the resources are disposed properly
        Using cmd As New MySqlCommand(sql, conn)
            ' Add the parameter to the SQL query
            cmd.Parameters.AddWithValue("@NOMOR", nomor)

            ' Using block to ensure the adapter is disposed properly
            Using adapter As New MySqlDataAdapter(cmd)
                ' Fill the DataSet with data from the database
                adapter.Fill(ds, "Gaji_karyawan")
            End Using
        End Using


        ' Create a list to hold the report parameters
        Dim reportParams As New List(Of ReportParameter) From {
            New ReportParameter("NOMOR", nomor),
            New ReportParameter("SUPIR", "Tugas Supir " & bonusSupir.ToString("N0") & " :"),
            New ReportParameter("HELPER", "Tugas Helper " & bonusHelper.ToString("N0") & " :"),
            New ReportParameter("LEMBUR", "Lembur " & bonusLembur.ToString("N0") & " :"),
            New ReportParameter("ABSENKHUSUS", "Pot Absen Khusus " & potonganAbsenKhusus.ToString("N0") & " :"),
            New ReportParameter("TERLAMBAT", "Pot Terlambat " & potonganTerlambat.ToString("N0") & " :"),
            New ReportParameter("USER", FormUtama.SLogin.Text),
            New ReportParameter("TERBILANG", TerimaTerbilang),
            New ReportParameter("TOKO", NAMA_PERUSAHAAN),
            New ReportParameter("PEMILIK", PEMILIK_PERUSAHAAN)
        }

        ' Clear any existing DataSources
        ReportViewer1.LocalReport.DataSources.Clear()

        ' Add new DataSources
        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("Gaji_karyawan")))

        ' Set the parameters for the report
        ReportViewer1.LocalReport.SetParameters(reportParams)

        ' Refresh the ReportViewer
        ReportViewer1.RefreshReport()
    End Sub



    Private Sub BtnSimpann_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpann.Click

        If ValidateInputs() Then
            ' Memulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                ' Mengonversi nilai TextBox menjadi Decimal dengan penanganan nilai kosong atau tidak valid
                Dim terima As Decimal = If(Decimal.TryParse(TxtTerima.Text, Nothing), Decimal.Parse(TxtTerima.Text), 0D)

                ' Hitung potonganBon terlebih dahulu
                Dim potonganBon As Decimal =
                    If(Decimal.TryParse(TxtPotBon.Text, Nothing), Decimal.Parse(TxtPotBon.Text), 0D) +
                    If(Decimal.TryParse(TxtAngsuran.Text, Nothing), Decimal.Parse(TxtAngsuran.Text), 0D)

                ' Hitung potongan dengan mengurangi potonganBon dari potongan
                Dim potongan As Decimal =
                    If(Decimal.TryParse(TxtPotongan.Text, Nothing), Decimal.Parse(TxtPotongan.Text), 0D)

                Dim potonganlain As Decimal = potongan - potonganBon


                If BtnSimpann.Text = "EDIT (F8)" Then
                    ' Hapus data gaji karyawan berdasarkan nomor transaksi
                    Dim queryHapusGaji As String = "DELETE FROM Gaji_karyawan WHERE NOMOR = @NomorTransaksi"
                    Using cmdHapusGaji As New MySqlCommand(queryHapusGaji, conn, transaction)
                        cmdHapusGaji.Parameters.AddWithValue("@NomorTransaksi", LblNomor.Text)
                        cmdHapusGaji.ExecuteNonQuery()
                    End Using

                    ' Hapus data jurnal umum berdasarkan nomor transaksi
                    Dim queryHapusJurnal As String = "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NomorTransaksi"
                    Using cmdHapusJurnal As New MySqlCommand(queryHapusJurnal, conn, transaction)
                        cmdHapusJurnal.Parameters.AddWithValue("@NomorTransaksi", LblNomor.Text)
                        cmdHapusJurnal.ExecuteNonQuery()
                    End Using

                    ' Hapus data bon karyawan berdasarkan faktur
                    Dim queryHapusBon As String = "DELETE FROM Bon_karyawan WHERE FAKTUR = @Faktur"
                    Using cmdHapusBon As New MySqlCommand(queryHapusBon, conn, transaction)
                        cmdHapusBon.Parameters.AddWithValue("@Faktur", LblNomor.Text)
                        cmdHapusBon.ExecuteNonQuery()
                    End Using

                    ' Ambil nilai potongan bon untuk edit dan set ke 0 jika tidak valid
                    Dim potonganBonEdit As Decimal
                    If Not Decimal.TryParse(TxtPotBonUntukEdit.Text, potonganBonEdit) Then
                        potonganBonEdit = 0D
                    End If

                    ' Update total bayar karyawan jika ada potongan bon yang perlu di-edit
                    If potonganBonEdit <> 0 Then
                        Dim queryUpdateBayar As String = "UPDATE tbl_karyawan SET TotalBayar = TotalBayar - @Nominal WHERE Kode = @Kode"
                        Using cmdUpdateBayar As New MySqlCommand(queryUpdateBayar, conn, transaction)
                            cmdUpdateBayar.Parameters.AddWithValue("@Nominal", potonganBonEdit)
                            cmdUpdateBayar.Parameters.AddWithValue("@Kode", LblKode.Text)
                            cmdUpdateBayar.ExecuteNonQuery()
                        End Using
                    End If

                End If


                SimpanGaji(transaction)
                ' Simpan jurnal 
                Simpanjurnal(transaction, terima)

                If potonganlain <> 0 Then
                    SimpanjurnalPotonganLain(transaction, potonganlain)
                End If

                If potonganBon <> 0 Then
                    SimpanjurnalPotonganBon(transaction, potonganBon)
                End If

                ' Commit transaksi jika berhasil
                transaction.Commit()

                ' Reset nilai-nilai pada kontrol setelah berhasil disimpan
                ResetControls()

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        ' Validasi untuk input yang diperlukan
        If CmbBulan.SelectedIndex = -1 Then
            MessageBox.Show("Bulan belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbBulan.DroppedDown = True ' Memunculkan dropdown list
            CmbBulan.Focus()
            Return False
        End If

        If CmbTahun.SelectedIndex = -1 Then
            MessageBox.Show("Tahun belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbTahun.DroppedDown = True ' Memunculkan dropdown list
            CmbTahun.Focus()
            Return False
        End If

        If CmbNama.SelectedIndex = -1 Then
            MessageBox.Show("Karyawan belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbNama.DroppedDown = True ' Memunculkan dropdown list
            CmbNama.Focus()
            Return False
        End If

        If CmbRekening.SelectedIndex = -1 Then
            MessageBox.Show("Sumber dana untuk bayar gaji belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbRekening.DroppedDown = True ' Memunculkan dropdown list
            CmbRekening.Focus()
            Return False
        End If

        ' Validasi untuk TxtPokok
        If String.IsNullOrWhiteSpace(TxtPokok.Text) OrElse TxtPokok.Text.Trim() = "0" Then
            MessageBox.Show("Gaji Pokok harus diisi dan tidak boleh 0", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TxtPokok.Focus()
            Return False
        End If

        ' Konversi nilai TextBox dan Label ke Decimal, dengan default 0
        Dim potBon As Decimal = If(Not Decimal.TryParse(TxtPotBon.Text, potBon), 0, potBon)
        Dim angsuran As Decimal = If(Not Decimal.TryParse(TxtAngsuran.Text, angsuran), 0, angsuran)
        Dim sisaBon As Decimal = If(Not Decimal.TryParse(LblSisaBon.Text, sisaBon), 0, sisaBon)

        ' Cek apakah jumlah PotBon dan Angsuran lebih besar dari Sisa Bon
        If sisaBon < 0 Then
            MessageBox.Show("Pembayaran bon lebih besar dari pada nominal bon", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TxtPotBon.Focus()
            Return False
        End If


        If BtnSimpann.Text = "SIMPAN (F8)" Then
            ' Query untuk mengecek apakah gaji untuk karyawan pada bulan tertentu sudah dibuat
            Dim query As String = "SELECT Nama, Bulan FROM gaji_karyawan WHERE Nama = @Nama AND Bulan = @Bulan"

            ' Menggunakan parameter untuk mencegah SQL Injection
            Using cmd As New MySqlCommand(query, conn)
                ' Menambahkan parameter untuk Nama dan Bulan
                cmd.Parameters.AddWithValue("@Nama", CmbNama.Text)
                cmd.Parameters.AddWithValue("@Bulan", teksBulanTahunTerpilih)

                ' Menjalankan query dan memeriksa apakah ada hasil
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        ' Jika ada hasil, tampilkan pesan
                        MessageBox.Show("Gaji untuk karyawan " & CmbNama.Text & " pada bulan " & teksBulanTahunTerpilih & " sudah dibuat.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return False
                    End If
                End Using
            End Using
        End If

        ' Validasi untuk input yang bersifat numerik
        Dim numericFields() As Control = {TxtOmsetJual, TxtKomisiJual, TxtSupir, TxtHelper, TxtLembur, TxtLemburRp, TxtTunjangan, TxtTransport, TxtMakan, TxtPotBon, TxtAngsuran, TxtAbsen, TxtAbsenRp, TxtAbsenKhusus, TxtAbsenKhususRp, TxtKeterlambatan, TxtKeterlambatanRp, TxtPotLain, TxtPendapatan, TxtPotongan, TxtTerima}
        For Each control As Control In numericFields
            If TypeOf control Is TextBox Then
                Dim textBox As TextBox = DirectCast(control, TextBox)

                ' Mengubah nilai kosong atau tidak valid menjadi "0"
                If String.IsNullOrWhiteSpace(textBox.Text) OrElse textBox.Text.Trim() = "0" Then
                    textBox.Text = "0"
                Else
                    ' Memeriksa apakah nilai bisa di-parse menjadi Decimal
                    Dim value As Decimal
                    If Not Decimal.TryParse(textBox.Text.Replace(".", ""), value) Then
                        textBox.Text = "0"
                    End If
                End If
            End If
        Next
        Return True
    End Function

    Private Sub SimpanGaji(ByVal transaction As MySqlTransaction)
        ' Menyiapkan perintah SQL untuk INSERT
        Dim query As String = "INSERT INTO Gaji_karyawan " &
                              "(NOMOR, BULAN, TANGGAL, TANGGALAWAL, TANGGALAKHIR, KODE, Nama, " &
                              "POKOK, OMSET_JUAL, KOMISI_JUAL, SUPIR, SUPIR_RP, HELPER, HELPER_RP, " &
                              "LEMBUR, LEMBUR_RP, TUNJANGAN, TRANSPORT, UANG_MAKAN, SALDO_BON, POT_BON, " &
                              "ANGSURAN, NILAI_POTONGAN_ABSEN, ABSEN, ABSEN_RP, ABSEN_KHUSUS, ABSEN_KHUSUS_RP, TERLAMBAT, " &
                              "TERLAMBAT_RP, POT_LAIN, PENDAPATAN, POTONGAN, TERIMA, REKENING, " &
                              "LOKASI, ID_USER, ID_KOMPUTER) " &
                              "VALUES (@NOMOR, @BULAN, @TANGGAL, @TANGGALAWAL, @TANGGALAKHIR, @KODE, @Nama, " &
                              "@POKOK, @OMSET_JUAL, @KOMISI_JUAL, @SUPIR, @SUPIR_RP, @HELPER, @HELPER_RP, " &
                              "@LEMBUR, @LEMBUR_RP, @TUNJANGAN, @TRANSPORT, @UANG_MAKAN, @SALDO_BON, @POT_BON, " &
                              "@ANGSURAN, @NILAI_POTONGAN_ABSEN, @ABSEN, @ABSEN_RP, @ABSEN_KHUSUS, @ABSEN_KHUSUS_RP, @TERLAMBAT, " &
                              "@TERLAMBAT_RP, @POT_LAIN, @PENDAPATAN, @POTONGAN, @TERIMA, @REKENING, " &
                              "@LOKASI, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            ' Mengisi parameter-parameter pada perintah SQL
            cmd.Parameters.AddWithValue("@NOMOR", LblNomor.Text)
            cmd.Parameters.AddWithValue("@BULAN", CmbBulan.Text & "/" & CmbTahun.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGALAWAL", DtpAwal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGALAKHIR", DtpAkhir.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text.Trim())
            cmd.Parameters.AddWithValue("@Nama", CmbNama.Text.Trim())
            cmd.Parameters.AddWithValue("@POKOK", Decimal.Parse(TxtPokok.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@OMSET_JUAL", Decimal.Parse(TxtOmsetJual.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@KOMISI_JUAL", Decimal.Parse(TxtKomisiJual.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@SUPIR", Decimal.Parse(LblSupir.Text))
            cmd.Parameters.AddWithValue("@SUPIR_RP", Decimal.Parse(TxtSupir.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@HELPER", Decimal.Parse(LblHelper.Text))
            cmd.Parameters.AddWithValue("@HELPER_RP", Decimal.Parse(TxtHelper.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@LEMBUR", Decimal.Parse(TxtLembur.Text))
            cmd.Parameters.AddWithValue("@LEMBUR_RP", Decimal.Parse(TxtLemburRp.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@TUNJANGAN", Decimal.Parse(TxtTunjangan.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@TRANSPORT", Decimal.Parse(TxtTransport.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@UANG_MAKAN", Decimal.Parse(TxtMakan.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@SALDO_BON", Decimal.Parse(LblSaldoBon.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@POT_BON", Decimal.Parse(TxtPotBon.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@ANGSURAN", Decimal.Parse(TxtAngsuran.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@NILAI_POTONGAN_ABSEN", Decimal.Parse(TxtPotAbsen.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@ABSEN", Decimal.Parse(TxtAbsen.Text))
            cmd.Parameters.AddWithValue("@ABSEN_RP", Decimal.Parse(TxtAbsenRp.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@ABSEN_KHUSUS", Decimal.Parse(TxtAbsenKhusus.Text))
            cmd.Parameters.AddWithValue("@ABSEN_KHUSUS_RP", Decimal.Parse(TxtAbsenKhususRp.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@TERLAMBAT", Decimal.Parse(TxtKeterlambatan.Text))
            cmd.Parameters.AddWithValue("@TERLAMBAT_RP", Decimal.Parse(TxtKeterlambatanRp.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@POT_LAIN", Decimal.Parse(TxtPotLain.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@PENDAPATAN", Decimal.Parse(TxtPendapatan.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@POTONGAN", Decimal.Parse(TxtPotongan.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@TERIMA", Decimal.Parse(TxtTerima.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@REKENING", CmbRekening.Text.Trim())
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            ' Eksekusi perintah INSERT
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction, ByVal terima As Decimal)
        ' Simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", LblKode.Text)
            cmd.Parameters.AddWithValue("@URAIAN", "Gaji diterimakan atas nama " & CmbNama.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "BEBAN GAJI KARYAWAN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "07.01.001")
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LblRekening.Text)
            cmd.Parameters.AddWithValue("@NOMINAL", terima)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Gaji")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub SimpanjurnalPotonganLain(ByVal transaction As MySqlTransaction, ByVal potonganlain As Decimal)

        Dim absenKhusus As Decimal = If(Decimal.TryParse(TxtAbsenKhususRp.Text, absenKhusus), absenKhusus, 0D)
        ' Simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", LblKode.Text)
            cmd.Parameters.AddWithValue("@URAIAN", "Potongan lain lain dari gaji atas nama " & CmbNama.Text & " Selain Absen Khusus")
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "BEBAN GAJI KARYAWAN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "07.01.001")
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", "PENDAPATAN LAIN LAIN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", "08.01.002")
            cmd.Parameters.AddWithValue("@NOMINAL", potonganlain - absenKhusus)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Gaji")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub SimpanjurnalPotonganBon(ByVal transaction As MySqlTransaction, ByVal potonganBon As Decimal)
        ' Simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", LblKode.Text)
            cmd.Parameters.AddWithValue("@URAIAN", "Potongan bon dari gaji atas nama " & CmbNama.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "BEBAN GAJI KARYAWAN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "07.01.001")
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", "PIUTANG KARYAWAN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", "01.03.002")
            cmd.Parameters.AddWithValue("@NOMINAL", potonganBon)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Gaji")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
            cmd.ExecuteNonQuery()
        End Using


        ' Define the SQL Insert query
        Dim sql As String = "INSERT INTO Bon_karyawan (FAKTUR, TANGGAL, LOKASI, JENIS, KODE, NAMA, KODE_REK, NAMA_REK, AWAL_BON, NOMINAL, AKHIR_BON, KETERANGAN, ID_USER, ID_KOMPUTER) VALUES (@FAKTUR, @TANGGAL, @LOKASI, @JENIS, @KODE, @NAMA, @KODE_REK, @NAMA_REK,@AWAL_BON, @NOMINAL, @AKHIR_BON, @KETERANGAN, @ID_USER, @ID_KOMPUTER)"

        ' Create a new MySqlCommand
        Using cmd As New MySqlCommand(sql, conn, transaction)
            ' Add parameters to the command
            cmd.Parameters.AddWithValue("@FAKTUR", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@JENIS", "BAYAR")
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmd.Parameters.AddWithValue("@NAMA", CmbNama.Text)
            cmd.Parameters.AddWithValue("@KODE_REK", LblRekening.Text)
            cmd.Parameters.AddWithValue("@NAMA_REK", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@AWAL_BON", Decimal.Parse(LblSaldoBon.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@NOMINAL", potonganBon)
            cmd.Parameters.AddWithValue("@AKHIR_BON", Decimal.Parse(LblSisaBon.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@KETERANGAN", "POTONG GAJI")
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            ' Execute the command
            cmd.ExecuteNonQuery()
        End Using

        Dim updateQuery As String = "UPDATE tbl_karyawan SET TotalBayar = TotalBayar + ? WHERE Kode = ?"

        Using cmdUpdate As New MySqlCommand(updateQuery, conn, transaction)
            cmdUpdate.Parameters.AddWithValue("@Nominal", potonganBon)
            cmdUpdate.Parameters.AddWithValue("@IDPENJUALAN", LblKode.Text)

            cmdUpdate.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Panel2.Visible = False
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        ResetControls()
    End Sub



    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub




End Class