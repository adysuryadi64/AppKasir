Imports Microsoft.Reporting.WinForms

Public Class FormGaji
    Dim teksBulanTahunTerpilih As String

    Private Sub FormGaji_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Panel2.Visible = False

        Dim GAJI As Boolean() = ModulHakAkses.BacaHakAksesDariCache("GAJI")
        BtnSimpann.Visible = GAJI(1)

        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")

        ResetControls()
        AmbilDataKaryawan()
        AmbildataMasterGaji()
        UpdateTotalBonDanTotalBayarKaryawan()
        ' Report hanya di-render saat ada karyawan dipilih — hindari warning dataset kosong saat load
        ' Me.ReportViewer1.RefreshReport()
        CmbPilihCetak.Text = BacaPengaturanPrinter("GajiKaryawan", "CetakOtomatis", "IYA")
        CmbProsesCetak.Text = BacaPengaturanPrinter("GajiKaryawan", "PilihPrinter", "LANGSUNG CETAK")
    End Sub

    Private Sub ResetControls()
        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        LblKode.Text = "KRY"
        LblRekening.Text = ""
        LblNomor.Text = ""
        LblSaldoBon.Text = "0"
        LblSisaBon.Text = "0"
        LblSupir.Text = "0"
        LblHelper.Text = "0"
        LblKetSupir.Text = "Supir 0 x 0 :"
        LblKetHelp.Text = "Helper 0 x 0 :"
        LblKomisJual.Text = "Komisi jual 0 % :"
        LblLembur.Text = "Lembur 0 :"
        LblAbsen.Text = "Absen 0 :"
        LblAbsenkhusus.Text = "Abs Khusus 0 :"
        LblTelat.Text = "Telat 0 :"

        CmbNama.SelectedIndex = -1
        CmbRekening.SelectedItem = nama_rek_Gaji_Karyawan

        TxtPokok.Clear()
        TxtOmsetJual.Clear()
        TxtKomisiJual.Clear()
        TxtSupir.Clear()
        TxtHelper.Clear()
        TxtLembur.Clear()
        TxtLemburRp.Clear()
        TxtTunjangan.Clear()
        TxtTransport.Clear()
        TxtMakan.Clear()
        TxtPotAbsen.Clear()
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

        GenerateNomorGaji()
        UpdateTotalBonDanTotalBayarKaryawan()
        MuatComboBoxBulanTahun(CmbBln, CmbThn)
        Ambildataperiodekerja()
        BtnSimpann.Text = "SIMPAN (F8)"
    End Sub



    Private Sub GenerateNomorGaji()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "GJ")
            cmd.Parameters.AddWithValue("@tgl", DtpTanggal.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "gaji_karyawan")
            cmd.Parameters.AddWithValue("@kolom", "NOMOR")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblNomor.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Private Sub CmbBulan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBln.SelectedIndexChanged, CmbThn.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
        Ambildataperiodekerja()
    End Sub

    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            teksBulanTahunTerpilih = CmbBln.Text & "/" & CmbThn.Text
            SetupDataGridView()
            TampilkanDataGaji(teksBulanTahunTerpilih)
        End If
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
                    hariKerja = If(IsDBNull(reader("Hari_kerja")), 30, Convert.ToInt32(reader("Hari_kerja")))
                    prosentaseKomisi = If(IsDBNull(reader("Prosentase_komisi")), 0D, Convert.ToDecimal(reader("Prosentase_komisi")))
                    bonusSupir = If(IsDBNull(reader("Bonus_Supir")), 0D, Convert.ToDecimal(reader("Bonus_Supir")))
                    bonusHelper = If(IsDBNull(reader("Bonus_Helper")), 0D, Convert.ToDecimal(reader("Bonus_Helper")))
                    bonusLembur = If(IsDBNull(reader("Bonus_Lembur")), 0D, Convert.ToDecimal(reader("Bonus_Lembur")))
                    bonusTransport = If(IsDBNull(reader("Bonus_Transport")), 0D, Convert.ToDecimal(reader("Bonus_Transport")))
                    bonusMakan = If(IsDBNull(reader("Bonus_makan")), 0D, Convert.ToDecimal(reader("Bonus_makan")))
                    potonganAbsen = If(IsDBNull(reader("Potongan_Absen")), 0D, Convert.ToDecimal(reader("Potongan_Absen")))
                    potonganAbsenKhusus = If(IsDBNull(reader("Potongan_Absen_Khusus")), 0D, Convert.ToDecimal(reader("Potongan_Absen_Khusus")))
                    potonganTerlambat = If(IsDBNull(reader("Potongan_Terlambat")), 0D, Convert.ToDecimal(reader("Potongan_Terlambat")))
                    jenisPotongan = If(IsDBNull(reader("Jenis_Potongan")), "", reader("Jenis_Potongan").ToString())
                Else
                    hariKerja = 30 : prosentaseKomisi = 0D : bonusSupir = 0D : bonusHelper = 0D
                    bonusLembur = 0D : bonusTransport = 0D : bonusMakan = 0D
                    potonganAbsen = 0D : potonganAbsenKhusus = 0D : potonganTerlambat = 0D
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
                    TxtPokok.Text = ModuleAngka.FormatUntukInput(If(IsDBNull(reader("Gaji")), 0D, Convert.ToDecimal(reader("Gaji"))))

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
            Dim potAbsen As Decimal = ModuleAngka.ParseDecimal(TxtPokok.Text) / hariKerja
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
                Dim bulanSekarang As Integer = CmbBln.SelectedIndex + 1
                Dim tahunSekarang As Integer = Integer.Parse(CmbThn.Text)

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
        Dim pokok As Decimal = ModuleAngka.ParseDecimal(TxtPokok.Text)
        Dim komisijual As Decimal = ModuleAngka.ParseDecimal(TxtKomisiJual.Text)
        Dim supir As Decimal = ModuleAngka.ParseDecimal(TxtSupir.Text)
        Dim helper As Decimal = ModuleAngka.ParseDecimal(TxtHelper.Text)
        Dim lemburRp As Decimal = ModuleAngka.ParseDecimal(TxtLemburRp.Text)
        Dim tunjangan As Decimal = ModuleAngka.ParseDecimal(TxtTunjangan.Text)
        Dim transport As Decimal = ModuleAngka.ParseDecimal(TxtTransport.Text)
        Dim makan As Decimal = ModuleAngka.ParseDecimal(TxtMakan.Text)

        ' Hitung total pendapatan
        Dim pendapatan As Decimal = pokok + komisijual + supir + helper + lemburRp + tunjangan + transport + makan

        ' Tampilkan total pendapatan dalam format tanpa tempat desimal
        TxtPendapatan.Text = pendapatan.ToString("N0")
    End Sub


    Private Sub TxtPokok_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtPokok.TextChanged, TxtKomisiJual.TextChanged, TxtSupir.TextChanged, TxtHelper.TextChanged, TxtLemburRp.TextChanged, TxtTunjangan.TextChanged, TxtTransport.TextChanged, TxtMakan.TextChanged
        HitungPendapatan()
    End Sub

    Private Sub HitungPotongan()
        Dim bon As Decimal = ModuleAngka.ParseDecimal(TxtPotBon.Text)
        Dim angsuran As Decimal = ModuleAngka.ParseDecimal(TxtAngsuran.Text)
        Dim absen As Decimal = ModuleAngka.ParseDecimal(TxtAbsenRp.Text)
        Dim absenKhusus As Decimal = ModuleAngka.ParseDecimal(TxtAbsenKhususRp.Text)
        Dim keterlambatan As Decimal = ModuleAngka.ParseDecimal(TxtKeterlambatanRp.Text)
        Dim potlain As Decimal = ModuleAngka.ParseDecimal(TxtPotLain.Text)

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
        Dim pendapatan As Decimal = ModuleAngka.ParseDecimal(TxtPendapatan.Text)
        Dim potongan As Decimal = ModuleAngka.ParseDecimal(TxtPotongan.Text)

        ' Hitung total penerimaan
        Dim penerimaan As Decimal = pendapatan - potongan

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtTerima.Text = penerimaan.ToString("N0")
    End Sub




    Private Sub TxtOmsetJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtOmsetJual.TextChanged
        ' Konversi nilai dari TextBox, jika kosong atau null, maka 0
        Dim OmsetPenjualan As Decimal = ModuleAngka.ParseDecimal(TxtOmsetJual.Text)

        ' Hitung total penerimaan dan bulatkan tanpa koma
        Dim KomisiJual As Decimal = Math.Round(OmsetPenjualan * (prosentaseKomisi / 100), 0, MidpointRounding.AwayFromZero)

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtKomisiJual.Text = KomisiJual.ToString("N0")
    End Sub


    Private Sub LblSupir_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblSupir.TextChanged
        ' Konversi nilai dari TextBox, jika kosong atau null, maka 0
        Dim Supir As Decimal = ModuleAngka.ParseDecimal(LblSupir.Text)

        ' Hitung total penerimaan
        Dim NilaiSupir As Decimal = Supir * bonusSupir

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtSupir.Text = NilaiSupir.ToString("N0")
    End Sub


    Private Sub LblHelper_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblHelper.TextChanged
        ' Konversi nilai dari TextBox, jika kosong atau null, maka 0
        Dim helper As Decimal = ModuleAngka.ParseDecimal(LblHelper.Text)

        ' Hitung total penerimaan
        Dim NilaiHelper As Decimal = helper * bonusHelper

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtHelper.Text = NilaiHelper.ToString("N0")
    End Sub


    Private Sub TxtPotBon_TextChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblSaldoBon.TextChanged, TxtPotBon.TextChanged, TxtAngsuran.TextChanged
        ' Konversi nilai dari setiap TextBox ke tipe Decimal, jika kosong atau null maka 0
        Dim SaldoBon As Decimal = ModuleAngka.ParseDecimal(LblSaldoBon.Text)
        Dim PotonganBon As Decimal = ModuleAngka.ParseDecimal(TxtPotBon.Text)
        Dim PotonganBonKhusus As Decimal = ModuleAngka.ParseDecimal(TxtAngsuran.Text)

        ' Hitung total sisa bon
        Dim SisaBon As Decimal = SaldoBon - PotonganBon - PotonganBonKhusus

        ' Tampilkan total sisa bon dalam format tanpa tempat desimal
        LblSisaBon.Text = SisaBon.ToString("N0")
    End Sub


    Private Sub TxtLembur_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtLembur.TextChanged
        ' Konversi nilai dari TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim lembur As Decimal = ModuleAngka.ParseDecimal(TxtLembur.Text)

        ' Hitung total nilai lembur
        Dim Nilailembur As Decimal = lembur * bonusLembur

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtLemburRp.Text = Nilailembur.ToString("N0")
    End Sub


    Private Sub TxtAbsen_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtAbsen.TextChanged
        ' Konversi nilai dari setiap TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim absen As Decimal = ModuleAngka.ParseDecimal(TxtAbsen.Text)
        Dim nilaiPotAbse As Decimal = ModuleAngka.ParseDecimal(TxtPotAbsen.Text)

        ' Hitung total penerimaan
        Dim Nilaiabsen As Decimal = absen * nilaiPotAbse

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtAbsenRp.Text = Nilaiabsen.ToString("N0")
    End Sub



    Private Sub TxtAbsenKhusus_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtAbsenKhusus.TextChanged
        ' Konversi nilai dari setiap TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim AbsenKhusus As Decimal = ModuleAngka.ParseDecimal(TxtAbsenKhusus.Text)

        ' Hitung total penerimaan
        Dim NilaiAbsenKhusus As Decimal = AbsenKhusus * potonganAbsenKhusus

        ' Tampilkan total penerimaan dalam format tanpa tempat desimal
        TxtAbsenKhususRp.Text = NilaiAbsenKhusus.ToString("N0")
    End Sub


    Private Sub TxtKeterlambatan_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtKeterlambatan.TextChanged
        ' Konversi nilai dari setiap TextBox ke Decimal, jika kosong maka nilai menjadi 0
        Dim Keterlambatan As Decimal = ModuleAngka.ParseDecimal(TxtKeterlambatan.Text)

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
        ModuleAngka.TerapkanFormatKolomAngka(DGVGaji, columnsToFormat)

        With DGVGaji
            ' Set header style

            ' Set alternating row style

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle

            ' Enable double buffering to reduce flickering
            ModuleTheme.ApplyThemeDataGridView(DGVGaji)
        End With
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
                Dim potBon As Decimal = ModuleAngka.ParseDecimal(TxtPotBon.Text)
                Dim angsuran As Decimal = ModuleAngka.ParseDecimal(TxtAngsuran.Text)
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
                Dim potonganBon As Decimal = ModuleAngka.ParseDecimal(DGVGaji.Rows(e.RowIndex).Cells("PotBon").Value) _
                                            + ModuleAngka.ParseDecimal(DGVGaji.Rows(e.RowIndex).Cells("Angsuran").Value)

                ' Konfirmasi penghapusan kepada pengguna
                Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    Dim transaction As MySqlTransaction = conn.BeginTransaction()
                    Try
                        ' ========================================
                        ' START: Audit Trail - Hapus Slip Gaji
                        ' ========================================
                        Dim sbSnapshot As New System.Text.StringBuilder()
                        Try
                            Using cmdSnap As New MySqlCommand(
                                "SELECT NOMOR, BULAN, TANGGAL, KODE, Nama, POKOK, OMSET_JUAL, KOMISI_JUAL, SUPIR_RP, HELPER_RP, LEMBUR_RP, TUNJANGAN, TRANSPORT, UANG_MAKAN, PENDAPATAN, POT_BON, ANGSURAN, ABSEN_RP, ABSEN_KHUSUS_RP, TERLAMBAT_RP, POT_LAIN, POTONGAN, TERIMA, REKENING " &
                                "FROM Gaji_karyawan WHERE NOMOR = @n LIMIT 1", conn, transaction)
                                cmdSnap.Parameters.AddWithValue("@n", nomorTransaksi)
                                Using rdSnap = cmdSnap.ExecuteReader()
                                    If rdSnap.Read() Then
                                        sbSnapshot.AppendLine($"Nomor: {rdSnap("NOMOR")}")
                                        sbSnapshot.AppendLine($"Bulan: {rdSnap("BULAN")}")
                                        sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                        sbSnapshot.AppendLine($"Kode Karyawan: {rdSnap("KODE")}")
                                        sbSnapshot.AppendLine($"Nama Karyawan: {rdSnap("Nama")}")
                                        sbSnapshot.AppendLine($"Gaji Pokok: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POKOK")))}")
                                        sbSnapshot.AppendLine($"Omset Jual: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("OMSET_JUAL")))}")
                                        sbSnapshot.AppendLine($"Komisi Jual: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("KOMISI_JUAL")))}")
                                        sbSnapshot.AppendLine($"Supir: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("SUPIR_RP")))}")
                                        sbSnapshot.AppendLine($"Helper: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("HELPER_RP")))}")
                                        sbSnapshot.AppendLine($"Lembur: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("LEMBUR_RP")))}")
                                        sbSnapshot.AppendLine($"Tunjangan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TUNJANGAN")))}")
                                        sbSnapshot.AppendLine($"Transport: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TRANSPORT")))}")
                                        sbSnapshot.AppendLine($"Uang Makan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("UANG_MAKAN")))}")
                                        sbSnapshot.AppendLine($"Total Pendapatan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("PENDAPATAN")))}")
                                        sbSnapshot.AppendLine($"Potongan Bon: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POT_BON")))}")
                                        sbSnapshot.AppendLine($"Angsuran: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("ANGSURAN")))}")
                                        sbSnapshot.AppendLine($"Potongan Absen: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("ABSEN_RP")))}")
                                        sbSnapshot.AppendLine($"Potongan Absen Khusus: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("ABSEN_KHUSUS_RP")))}")
                                        sbSnapshot.AppendLine($"Potongan Terlambat: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TERLAMBAT_RP")))}")
                                        sbSnapshot.AppendLine($"Potongan Lainnya: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POT_LAIN")))}")
                                        sbSnapshot.AppendLine($"Total Potongan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POTONGAN")))}")
                                        sbSnapshot.AppendLine($"Total Terima: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TERIMA")))}")
                                        sbSnapshot.AppendLine($"Rekening: {rdSnap("REKENING")}")
                                    End If
                                End Using
                            End Using
                        Catch
                            sbSnapshot.AppendLine("Gagal baca data sebelum hapus")
                        End Try
                        ModuleAuditTrail.CatatAuditMaster("SLIP:" & nomorTransaksi, "HAPUS", "Slip Gaji", sbSnapshot.ToString(), trans:=transaction)
                        ' ========================================
                        ' END: Audit Trail - Hapus Slip Gaji
                        ' ========================================

                        ' ========================================
                        ' STEP 1: REVERSAL saldo akun SEBELUM DELETE JurnalUmum
                        ' ========================================
                        ReversalSaldoAkunDariFaktur(nomorTransaksi, transaction)

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

                        ' Update SaldoAkhir karyawan secara realtime
                        UpdateBonKaryawan(kodeKaryawan, transaction)

                        ' Update saldo akun — sudah dilakukan sebelum DELETE di atas

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
            New ReportParameter("USER", FormUtama.StatusNamaUser.Text),
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
                Dim terima As Decimal = ModuleAngka.ParseDecimal(TxtTerima.Text)

                ' Hitung potonganBon terlebih dahulu
                Dim potonganBon As Decimal =
                    ModuleAngka.ParseDecimal(TxtPotBon.Text) +
                    ModuleAngka.ParseDecimal(TxtAngsuran.Text)

                ' Hitung potongan dengan mengurangi potonganBon dari potongan
                Dim potongan As Decimal = ModuleAngka.ParseDecimal(TxtPotongan.Text)

                Dim potonganlain As Decimal = potongan - potonganBon

                If BtnSimpann.Text = "EDIT (F8)" Then
                    ' ========================================
                    ' START: Audit Trail - Edit Slip Gaji
                    ' ========================================
                    Dim sbSnapshot As New System.Text.StringBuilder()
                    Try
                        Using cmdSnap As New MySqlCommand(
                            "SELECT NOMOR, BULAN, TANGGAL, KODE, Nama, POKOK, OMSET_JUAL, KOMISI_JUAL, SUPIR_RP, HELPER_RP, LEMBUR_RP, TUNJANGAN, TRANSPORT, UANG_MAKAN, PENDAPATAN, POT_BON, ANGSURAN, ABSEN_RP, ABSEN_KHUSUS_RP, TERLAMBAT_RP, POT_LAIN, POTONGAN, TERIMA, REKENING " &
                            "FROM Gaji_karyawan WHERE NOMOR = @n LIMIT 1", conn, transaction)
                            cmdSnap.Parameters.AddWithValue("@n", LblNomor.Text)
                            Using rdSnap = cmdSnap.ExecuteReader()
                                If rdSnap.Read() Then
                                    sbSnapshot.AppendLine($"Nomor: {rdSnap("NOMOR")}")
                                    sbSnapshot.AppendLine($"Bulan: {rdSnap("BULAN")}")
                                    sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                    sbSnapshot.AppendLine($"Kode Karyawan: {rdSnap("KODE")}")
                                    sbSnapshot.AppendLine($"Nama Karyawan: {rdSnap("Nama")}")
                                    sbSnapshot.AppendLine($"Gaji Pokok: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POKOK")))}")
                                    sbSnapshot.AppendLine($"Omset Jual: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("OMSET_JUAL")))}")
                                    sbSnapshot.AppendLine($"Komisi Jual: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("KOMISI_JUAL")))}")
                                    sbSnapshot.AppendLine($"Supir: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("SUPIR_RP")))}")
                                    sbSnapshot.AppendLine($"Helper: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("HELPER_RP")))}")
                                    sbSnapshot.AppendLine($"Lembur: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("LEMBUR_RP")))}")
                                    sbSnapshot.AppendLine($"Tunjangan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TUNJANGAN")))}")
                                    sbSnapshot.AppendLine($"Transport: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TRANSPORT")))}")
                                    sbSnapshot.AppendLine($"Uang Makan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("UANG_MAKAN")))}")
                                    sbSnapshot.AppendLine($"Total Pendapatan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("PENDAPATAN")))}")
                                    sbSnapshot.AppendLine($"Potongan Bon: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POT_BON")))}")
                                    sbSnapshot.AppendLine($"Angsuran: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("ANGSURAN")))}")
                                    sbSnapshot.AppendLine($"Potongan Absen: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("ABSEN_RP")))}")
                                    sbSnapshot.AppendLine($"Potongan Absen Khusus: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("ABSEN_KHUSUS_RP")))}")
                                    sbSnapshot.AppendLine($"Potongan Terlambat: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TERLAMBAT_RP")))}")
                                    sbSnapshot.AppendLine($"Potongan Lainnya: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POT_LAIN")))}")
                                    sbSnapshot.AppendLine($"Total Potongan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("POTONGAN")))}")
                                    sbSnapshot.AppendLine($"Total Terima: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TERIMA")))}")
                                    sbSnapshot.AppendLine($"Rekening: {rdSnap("REKENING")}")
                                End If
                            End Using
                        End Using
                    Catch
                        sbSnapshot.AppendLine("Gagal baca data sebelum edit")
                    End Try
                    ModuleAuditTrail.CatatAuditMaster("SLIP:" & LblNomor.Text, "EDIT", "Slip Gaji", sbSnapshot.ToString(), trans:=transaction)
                    ' ========================================
                    ' END: Audit Trail - Edit Slip Gaji
                    ' ========================================

                    ' Hapus data gaji karyawan berdasarkan nomor transaksi
                    Dim queryHapusGaji As String = "DELETE FROM Gaji_karyawan WHERE NOMOR = @NomorTransaksi"
                    Using cmdHapusGaji As New MySqlCommand(queryHapusGaji, conn, transaction)
                        cmdHapusGaji.Parameters.AddWithValue("@NomorTransaksi", LblNomor.Text)
                        cmdHapusGaji.ExecuteNonQuery()
                    End Using

                    ' Reversal saldo akun SEBELUM DELETE JurnalUmum
                    ReversalSaldoAkunDariFaktur(LblNomor.Text, transaction)

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
                    Dim potonganBonEdit As Decimal = ModuleAngka.ParseDecimal(TxtPotBonUntukEdit.Text)

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

                ' Debug summary jurnal gaji
                Dim totalGajiD As Decimal = terima
                If potonganlain <> 0 Then totalGajiD += potonganlain
                If potonganBon <> 0 Then totalGajiD += potonganBon
                Dim totalGajiK As Decimal = totalGajiD  ' D+K selalu sama per baris
                Debug.WriteLine("═══════════════════════════════════════════════════════")
                Debug.WriteLine("DEBUG JURNAL GAJI - Nomor: " & LblNomor.Text & " | " & CmbNama.Text)
                Debug.WriteLine("═══════════════════════════════════════════════════════")
                Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12} {5,12}", "No", "Uraian", "Akun Debet", "Akun Kredit", "Debet", "Kredit"))
                Debug.WriteLine(New String("─"c, 115))
                Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "J1", "GajiDiterima", "BEBAN GAJI [07.01.001]", CmbRekening.Text & " [" & LblRekening.Text & "]", terima, terima))
                If potonganlain <> 0 Then Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "J2", "PotonganLain", "BEBAN GAJI [07.01.001]", "PENDAPATAN LAIN [08.01.002]", potonganlain, potonganlain))
                If potonganBon <> 0 Then Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "J3", "PotonganBon", "BEBAN GAJI [07.01.001]", "PIUTANG KARYAWAN [01.03.002]", potonganBon, potonganBon))
                Debug.WriteLine(New String("─"c, 115))
                Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "TOTAL", "", "", "", totalGajiD, totalGajiK))
                Debug.WriteLine("✅ JURNAL SEIMBANG - D=K=" & totalGajiD.ToString("N0"))
                Debug.WriteLine("═══════════════════════════════════════════════════════")

                ' Update saldo bon karyawan secara realtime
                UpdateBonKaryawan(LblKode.Text, transaction)

                ' Update saldo akun — incremental delta
                UpdateSaldoAkunDeltaDariFaktur(LblNomor.Text, transaction)

                ' Commit transaksi jika berhasil
                transaction.Commit()

                ' Audit jurnal keseimbangan
                Dim totalGajiJurnal As Decimal = terima
                If potonganlain <> 0 Then totalGajiJurnal += potonganlain
                If potonganBon <> 0 Then totalGajiJurnal += potonganBon
                CatatJurnalTidakSeimbang(LblNomor.Text, totalGajiJurnal, totalGajiJurnal, "Gaji",
                    {"GajiDiterima", "PotonganLain", "PotonganBon"})

                ' Reset nilai-nilai pada kontrol setelah berhasil disimpan
                Dim noGaji As String = LblNomor.Text
                ResetControls()

                ' Cetak setelah simpan
                Try
                    Select Case CmbPilihCetak.Text.Trim().ToUpper()
                        Case "IYA"
                            LakukanCetakGaji(noGaji)
                        Case "SELALU TANYA"
                            If MessageBox.Show("Apakah Anda ingin mencetak slip gaji?",
                                               "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                                LakukanCetakGaji(noGaji)
                            End If
                        Case "TAMPILKAN DI MONITOR"
                            ModulePrinterGajiKaryawan.CetakGajiKaryawan(noGaji, "Tampilkan di Monitor")
                    End Select
                Catch ex As Exception
                    MessageBox.Show("Gagal mencetak slip gaji." & vbCrLf & "Detail: " & ex.Message,
                                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

    Private Sub LakukanCetakGaji(nomor As String)
        If CmbProsesCetak.Text = "TANYA PILIH PRINTER" Then
            ModulePrinterGajiKaryawan.TanyaPilihPrinterGajiKaryawan(nomor)
        Else
            ModulePrinterGajiKaryawan.CetakGajiKaryawan(nomor)
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        ' Validasi untuk input yang diperlukan
        If CmbBln.SelectedIndex = -1 Then
            MessageBox.Show("Bulan belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbBln.DroppedDown = True ' Memunculkan dropdown list
            CmbBln.Focus()
            Return False
        End If

        If CmbThn.SelectedIndex = -1 Then
            MessageBox.Show("Tahun belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbThn.DroppedDown = True ' Memunculkan dropdown list
            CmbThn.Focus()
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
        Dim potBon As Decimal = ModuleAngka.ParseDecimal(TxtPotBon.Text)
        Dim angsuran As Decimal = ModuleAngka.ParseDecimal(TxtAngsuran.Text)
        Dim sisaBon As Decimal = ModuleAngka.ParseDecimal(LblSisaBon.Text)

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
                    If ModuleAngka.ParseDecimal(textBox.Text) = 0D AndAlso textBox.Text.Trim() <> "0" Then
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
            cmd.Parameters.AddWithValue("@BULAN", CmbBln.Text & "/" & CmbThn.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGALAWAL", DtpAwal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGALAKHIR", DtpAkhir.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text.Trim())
            cmd.Parameters.AddWithValue("@Nama", CmbNama.Text.Trim())
            cmd.Parameters.AddWithValue("@POKOK", ModuleAngka.ParseDecimal(TxtPokok.Text))
            cmd.Parameters.AddWithValue("@OMSET_JUAL", ModuleAngka.ParseDecimal(TxtOmsetJual.Text))
            cmd.Parameters.AddWithValue("@KOMISI_JUAL", ModuleAngka.ParseDecimal(TxtKomisiJual.Text))
            cmd.Parameters.AddWithValue("@SUPIR", ModuleAngka.ParseDecimal(LblSupir.Text))
            cmd.Parameters.AddWithValue("@SUPIR_RP", ModuleAngka.ParseDecimal(TxtSupir.Text))
            cmd.Parameters.AddWithValue("@HELPER", ModuleAngka.ParseDecimal(LblHelper.Text))
            cmd.Parameters.AddWithValue("@HELPER_RP", ModuleAngka.ParseDecimal(TxtHelper.Text))
            cmd.Parameters.AddWithValue("@LEMBUR", ModuleAngka.ParseDecimal(TxtLembur.Text))
            cmd.Parameters.AddWithValue("@LEMBUR_RP", ModuleAngka.ParseDecimal(TxtLemburRp.Text))
            cmd.Parameters.AddWithValue("@TUNJANGAN", ModuleAngka.ParseDecimal(TxtTunjangan.Text))
            cmd.Parameters.AddWithValue("@TRANSPORT", ModuleAngka.ParseDecimal(TxtTransport.Text))
            cmd.Parameters.AddWithValue("@UANG_MAKAN", ModuleAngka.ParseDecimal(TxtMakan.Text))
            cmd.Parameters.AddWithValue("@SALDO_BON", ModuleAngka.ParseDecimal(LblSaldoBon.Text))
            cmd.Parameters.AddWithValue("@POT_BON", ModuleAngka.ParseDecimal(TxtPotBon.Text))
            cmd.Parameters.AddWithValue("@ANGSURAN", ModuleAngka.ParseDecimal(TxtAngsuran.Text))
            cmd.Parameters.AddWithValue("@NILAI_POTONGAN_ABSEN", ModuleAngka.ParseDecimal(TxtPotAbsen.Text))
            cmd.Parameters.AddWithValue("@ABSEN", ModuleAngka.ParseDecimal(TxtAbsen.Text))
            cmd.Parameters.AddWithValue("@ABSEN_RP", ModuleAngka.ParseDecimal(TxtAbsenRp.Text))
            cmd.Parameters.AddWithValue("@ABSEN_KHUSUS", ModuleAngka.ParseDecimal(TxtAbsenKhusus.Text))
            cmd.Parameters.AddWithValue("@ABSEN_KHUSUS_RP", ModuleAngka.ParseDecimal(TxtAbsenKhususRp.Text))
            cmd.Parameters.AddWithValue("@TERLAMBAT", ModuleAngka.ParseDecimal(TxtKeterlambatan.Text))
            cmd.Parameters.AddWithValue("@TERLAMBAT_RP", ModuleAngka.ParseDecimal(TxtKeterlambatanRp.Text))
            cmd.Parameters.AddWithValue("@POT_LAIN", ModuleAngka.ParseDecimal(TxtPotLain.Text))
            cmd.Parameters.AddWithValue("@PENDAPATAN", ModuleAngka.ParseDecimal(TxtPendapatan.Text))
            cmd.Parameters.AddWithValue("@POTONGAN", ModuleAngka.ParseDecimal(TxtPotongan.Text))
            cmd.Parameters.AddWithValue("@TERIMA", ModuleAngka.ParseDecimal(TxtTerima.Text))
            cmd.Parameters.AddWithValue("@REKENING", CmbRekening.Text.Trim())
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

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
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "GAJI")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub SimpanjurnalPotonganLain(ByVal transaction As MySqlTransaction, ByVal potonganlain As Decimal)

        Dim absenKhusus As Decimal = ModuleAngka.ParseDecimal(TxtAbsenKhususRp.Text)
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
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "GAJI")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
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
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "GAJI")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using


        ' Define the SQL Insert query
        Dim sql As String = "INSERT INTO Bon_karyawan (FAKTUR, TANGGAL, LOKASI, JENIS, KODE, NAMA, KODE_REK, NAMA_REK, AWAL_BON, NOMINAL, AKHIR_BON, KETERANGAN, ID_USER, ID_KOMPUTER) VALUES (@FAKTUR, @TANGGAL, @LOKASI, @JENIS, @KODE, @NAMA, @KODE_REK, @NAMA_REK,@AWAL_BON, @NOMINAL, @AKHIR_BON, @KETERANGAN, @ID_USER, @ID_KOMPUTER)"

        ' Create a new MySqlCommand
        Using cmd As New MySqlCommand(sql, conn, transaction)
            ' Add parameters to the command
            cmd.Parameters.AddWithValue("@FAKTUR", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@JENIS", "BAYAR")
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmd.Parameters.AddWithValue("@NAMA", CmbNama.Text)
            cmd.Parameters.AddWithValue("@KODE_REK", LblRekening.Text)
            cmd.Parameters.AddWithValue("@NAMA_REK", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@AWAL_BON", ModuleAngka.ParseDecimal(LblSaldoBon.Text))
            cmd.Parameters.AddWithValue("@NOMINAL", potonganBon)
            cmd.Parameters.AddWithValue("@AKHIR_BON", ModuleAngka.ParseDecimal(LblSisaBon.Text))
            cmd.Parameters.AddWithValue("@KETERANGAN", "POTONG GAJI")
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

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

    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "Gaji"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

End Class
