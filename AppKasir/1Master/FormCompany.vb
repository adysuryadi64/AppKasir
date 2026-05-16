Imports System.IO

Public Class FormCompany
    Private Sub FormProfilPerusahaan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Dim Toko As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Toko")
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpan.Visible = Toko(2) ' CanEdit 

        IsiComboBoxAkunDefault()
        Ambildataperusahaan()

        ' Debug: pantau perubahan nama
        AddHandler TxtNamaPerusahaan.TextChanged, Sub(s, ev)
                                                      Debug.WriteLine($"[Company] TxtNamaPerusahaan BERUBAH → '{TxtNamaPerusahaan.Text}' (caller: {New System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod()?.Name})")
                                                  End Sub

        ' Generate kode_cabang setelah data perusahaan dimuat
        ' agar prefix kode perusahaan sudah tersedia
        Dim kodeCabang As String = SyncConfig.KodeCabang
        TxtKodeCloud.Text = kodeCabang


        TampilkanGambarJikaBelum(PBNota, "LOGO.PNG")
        TampilkanGambarJikaBelum(PBToko, "TOKO.JPG")
        TampilkanGambarJikaBelum(PBGudang, "GUDANG.JPG")

        Me.Cursor = Cursors.Default
    End Sub


    Private Sub TampilkanGambarJikaBelum(picBox As PictureBox, namaFile As String)
        ' Jika PictureBox sudah memiliki gambar, tidak perlu memuat ulang
        If picBox.Image IsNot Nothing Then Return

        Dim path As String = IO.Path.Combine(Application.StartupPath, namaFile)

        ' Periksa apakah file gambar ada
        If Not IO.File.Exists(path) Then Return

        Try
            ' Baca semua byte sekaligus (File.ReadAllBytes otomatis menutup file)
            Dim imageBytes As Byte() = IO.File.ReadAllBytes(path)

            ' Buat gambar dari memory stream
            Using ms As New IO.MemoryStream(imageBytes)
                ' Buat salinan gambar yang benar-benar independen
                Dim newImage As Image = Image.FromStream(ms)

                ' Hapus gambar lama jika ada (sebagai langkah preventif)
                If picBox.Image IsNot Nothing Then
                    picBox.Image.Dispose()
                End If

                ' Set gambar baru ke PictureBox
                picBox.Image = CType(newImage.Clone(), Image)
            End Using

        Catch ex As Exception
            MessageBox.Show("Gagal memuat gambar: " & ex.Message, "Error",
                      MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub IsiComboBoxAkunDefault()
        IsiComboBoxAkun(CmbRekBarang, "A LANCAR")
        IsiComboBoxAkun(CmbLawanRekBarang, "EKUITAS")
        IsiComboBoxAkun(CmbJualToko, "KAS")
        IsiComboBoxAkun(CmbJualGudang, "KAS")
        IsiComboBoxAkun(CmbBeliToko, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbBeliGudang, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbHutangBeli, "HUTANG")
        IsiComboBoxAkun(CmbPiutangJual, "A LANCAR")
        IsiComboBoxAkun(CmbReturPembelianToko, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbReturPenjualanToko, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbReturPembelianGudang, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbReturPenjualanGudang, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbBonKaryawan, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbGajiKaryawan, "KAS", "EKUITAS")
        IsiComboBoxAkun(CmbBayarHutang, "KAS", "BANK", "EKUITAS")
        IsiComboBoxAkun(CmbBayarPiutang, "KAS", "BANK", "EKUITAS")
        IsiComboBoxAkun(CmbTransferJual, "BANK")
    End Sub


    Private Sub SetKodeAkunFromComboBox(ByVal comboBox As ComboBox, ByVal textBox As TextBox)
        EnsureConnectionReady()
        Using cmd As New MySqlCommand("SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @Nama_Akun", conn)
            cmd.Parameters.AddWithValue("@Nama_Akun", comboBox.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    textBox.Text = rd("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub


    Private Sub CmbRekBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekBarang.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbRekBarang, TxtRekBarang)
    End Sub


    Private Sub CmbLawanRekBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbLawanRekBarang.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbLawanRekBarang, TxtLawanRekBarang)
    End Sub

    Private Sub CmbBeliToko_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBeliToko.SelectedIndexChanged

        SetKodeAkunFromComboBox(CmbBeliToko, TxtBeliToko)

    End Sub

    Private Sub CmbBeliGudang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBeliGudang.SelectedIndexChanged

        SetKodeAkunFromComboBox(CmbBeliGudang, TxtBeliGudang)

    End Sub

    Private Sub CmbJualToko_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJualToko.SelectedIndexChanged

        SetKodeAkunFromComboBox(CmbJualToko, TxtJualToko)

    End Sub

    Private Sub CmbJualGudang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJualGudang.SelectedIndexChanged

        SetKodeAkunFromComboBox(CmbJualGudang, TxtJualGudang)

    End Sub

    Private Sub CmbHutangBeli_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbHutangBeli.SelectedIndexChanged

        SetKodeAkunFromComboBox(CmbHutangBeli, TxtHutangBeli)

    End Sub

    Private Sub CmbPiutangJual_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbPiutangJual.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbPiutangJual, TxtPiutangJual)
    End Sub

    Private Sub CmbReturPembelianToko_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbReturPembelianToko.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbReturPembelianToko, TxtReturPembelianToko)
    End Sub

    Private Sub CmbReturPenjualanToko_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbReturPenjualanToko.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbReturPenjualanToko, TxtReturPenjualanToko)
    End Sub

    Private Sub CmbReturPembelianGudang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbReturPembelianGudang.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbReturPembelianGudang, TxtReturPembelianGudang)
    End Sub

    Private Sub CmbReturPenjualanGudang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbReturPenjualanGudang.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbReturPenjualanGudang, TxtReturPenjualanGudang)
    End Sub

    Private Sub CmbBonKaryawan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbBonKaryawan.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbBonKaryawan, TxtBonKaryawan)
    End Sub

    Private Sub CmbGajiKaryawan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbGajiKaryawan.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbGajiKaryawan, TxtGajiKaryawan)
    End Sub

    Private Sub CmbBayarHutang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbBayarHutang.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbBayarHutang, TxtBayarHutang)
    End Sub

    Private Sub CmbBayarPiutang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbBayarPiutang.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbBayarPiutang, TxtBayarPiutang)
    End Sub

    Private Sub CmbTransferJual_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbTransferJual.SelectedIndexChanged
        SetKodeAkunFromComboBox(CmbTransferJual, TxtTransferJual)
    End Sub

    Public Sub Ambildataperusahaan()
        Debug.WriteLine("[Company.Load] AmbilDataMasterPerusahaan dipanggil")
        AmbilDataMasterPerusahaan()
        Debug.WriteLine($"[Company.Load] Setelah ambil — NAMA_PERUSAHAAN='{NAMA_PERUSAHAAN}', KODE='{KODE_PERUSAHAAN}'")

        Dim adaData As Boolean = Not String.IsNullOrEmpty(KODE_PERUSAHAAN)
        Debug.WriteLine($"[Company.Load] adaData={adaData}")

        If adaData Then
            TxtKodePerusahaan.Text = KODE_PERUSAHAAN
            TxtNamaPerusahaan.Text = NAMA_PERUSAHAAN
            Debug.WriteLine($"[Company.Load] TxtNamaPerusahaan diisi: '{TxtNamaPerusahaan.Text}'")
            TxtNamaCloud.Text = NAMA_CLOUD
            TxtAlamatCloud.Text = ALAMAT_CLOUD
            TxtAlamatPerusahaan.Text = ALAMAT_PERUSAHAAN
            TxtKotaPerusahaan.Text = KOTA_PERUSAHAAN
            TxtKontakPerusahaan.Text = KONTAK_PERUSAHAAN
            TxtPemilikPerusahaan.Text = PEMILIK_PERUSAHAAN
            TxtFooter1.Text = FOOTER1
            TxtFooter2.Text = FOOTER2
            TxtFooter3.Text = FOOTER3
            TxtRekBarang.Text = KODE_REK_BARANG
            CmbRekBarang.Text = NAMA_REK_BARANG
            TxtLawanRekBarang.Text = LAWAN_KODE_REK_BARANG
            CmbLawanRekBarang.Text = LAWAN_NAMA_REK_BARANG
            TxtBeliToko.Text = Kode_rek_Beli_toko
            CmbBeliToko.Text = nama_rek_Beli_toko
            TxtBeliGudang.Text = Kode_rek_Beli_Gudang
            CmbBeliGudang.Text = nama_rek_Beli_Gudang
            TxtJualToko.Text = Kode_rek_Jual_Toko
            CmbJualToko.Text = nama_rek_Jual_Toko
            TxtJualGudang.Text = Kode_rek_Jual_Gudang
            CmbJualGudang.Text = nama_rek_Jual_Gudang
            TxtHutangBeli.Text = Kode_rek_Hutang_Beli
            CmbHutangBeli.Text = nama_rek_Hutang_Beli
            TxtPiutangJual.Text = Kode_rek_Piutang_Jual
            CmbPiutangJual.Text = nama_rek_Piutang_Jual
            CmbTutupBulan.Text = JENIS_TUTUP_BULAN
            TxtTutupBulan.Text = TANGGAL_TUTUP_BULAN
            TxtReturPembelianToko.Text = Kode_rek_Retur_Pembelian_Toko
            CmbReturPembelianToko.Text = nama_rek_Retur_Pembelian_Toko
            TxtReturPenjualanToko.Text = Kode_rek_Retur_Penjualan_Toko
            CmbReturPenjualanToko.Text = nama_rek_Retur_Penjualan_Toko
            TxtReturPembelianGudang.Text = Kode_rek_Retur_Pembelian_Gudang
            CmbReturPembelianGudang.Text = nama_rek_Retur_Pembelian_Gudang
            TxtReturPenjualanGudang.Text = Kode_rek_Retur_Penjualan_Gudang
            CmbReturPenjualanGudang.Text = nama_rek_Retur_Penjualan_Gudang
            TxtBonKaryawan.Text = Kode_rek_Bon_Karyawan
            CmbBonKaryawan.Text = nama_rek_Bon_Karyawan
            TxtGajiKaryawan.Text = Kode_rek_Gaji_Karyawan
            CmbGajiKaryawan.Text = nama_rek_Gaji_Karyawan
            TxtBayarHutang.Text = Kode_rek_Bayar_Hutang
            CmbBayarHutang.Text = nama_rek_Bayar_Hutang
            TxtBayarPiutang.Text = Kode_rek_Bayar_Piutang
            CmbBayarPiutang.Text = nama_rek_Bayar_Piutang
            TxtTransferJual.Text = Kode_rek_Transfer_Jual
            CmbTransferJual.Text = nama_rek_Transfer_Jual
        End If

        ' Kode dikunci jika data sudah ada — tidak boleh diubah karena dipakai sebagai kunci
        TxtKodePerusahaan.Enabled = Not adaData

        If Not adaData Then IsiNilaiDefault()

    End Sub

    Private Sub IsiNilaiDefault()
        TxtKodePerusahaan.Text = "TK-000001"
        TxtNamaPerusahaan.Text = "TOKO SAYA"
        TxtAlamatPerusahaan.Text = "Jl. Contoh No. 1, Rt 01/Rw 01"
        TxtKotaPerusahaan.Text = "Kota"
        TxtKontakPerusahaan.Text = "0812-3456-7890"
        TxtPemilikPerusahaan.Text = "NAMA PEMILIK"
        TxtFooter1.Text = "Terima Kasih Telah Berbelanja"
        TxtFooter2.Text = "Barang Yang Sudah Dibeli Tidak Dapat Dikembalikan"
        TxtFooter3.Text = "Garansi Barang Elektronik 1 Minggu Setelah Pembelian"
        TxtNamaCloud.Text = "Toko Saya - Cabang Utama"
        TxtAlamatCloud.Text = "Jl. Contoh No. 1, Kota"
        CmbTutupBulan.SelectedIndex = 0

        SetCmbDefault(CmbRekBarang, TxtRekBarang, "PERSEDIAAN BARANG")
        SetCmbDefault(CmbLawanRekBarang, TxtLawanRekBarang, "MODAL")
        SetCmbDefault(CmbJualToko, TxtJualToko, "KAS DI TOKO")
        SetCmbDefault(CmbJualGudang, TxtJualGudang, "KAS DI GUDANG")
        SetCmbDefault(CmbBeliToko, TxtBeliToko, "KAS DI TOKO")
        SetCmbDefault(CmbBeliGudang, TxtBeliGudang, "KAS DI GUDANG")
        SetCmbDefault(CmbHutangBeli, TxtHutangBeli, "HUTANG BELANJA")
        SetCmbDefault(CmbPiutangJual, TxtPiutangJual, "TAGIHAN / SALDO PIUTANG")
        SetCmbDefault(CmbReturPembelianToko, TxtReturPembelianToko, "KAS DI TOKO")
        SetCmbDefault(CmbReturPenjualanToko, TxtReturPenjualanToko, "KAS DI TOKO")
        SetCmbDefault(CmbReturPembelianGudang, TxtReturPembelianGudang, "KAS DI GUDANG")
        SetCmbDefault(CmbReturPenjualanGudang, TxtReturPenjualanGudang, "KAS DI GUDANG")
        SetCmbDefault(CmbBonKaryawan, TxtBonKaryawan, "KAS DI TOKO")
        SetCmbDefault(CmbGajiKaryawan, TxtGajiKaryawan, "KAS DI TOKO")
        SetCmbDefault(CmbBayarHutang, TxtBayarHutang, "KAS DI TOKO")
        SetCmbDefault(CmbBayarPiutang, TxtBayarPiutang, "KAS DI TOKO")
        SetCmbDefault(CmbTransferJual, TxtTransferJual, "TRANSFER BANK")
    End Sub

    ''' <summary>
    ''' Set combobox ke nama default. Jika nama tidak ada di items, reset ke -1 dan kosongkan kode.
    ''' Jika ada, SelectedIndexChanged akan mengisi kode otomatis.
    ''' </summary>
    Private Sub SetCmbDefault(cmb As ComboBox, txt As TextBox, namaDefault As String)
        Dim idx As Integer = cmb.FindStringExact(namaDefault)
        If idx >= 0 Then
            cmb.SelectedIndex = idx  ' trigger SelectedIndexChanged → isi kode otomatis
        Else
            cmb.SelectedIndex = -1
            txt.Clear()
        End If
    End Sub

    Private Function ValidasiDataperusahaan() As Boolean
        If String.IsNullOrEmpty(TxtKodePerusahaan.Text) Then
            MessageBox.Show("Kode perusahaan tidak boleh kosong")
            TxtKodePerusahaan.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(TxtNamaPerusahaan.Text) Then
            MessageBox.Show("Nama perusahaan tidak boleh kosong")
            TxtNamaPerusahaan.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(TxtAlamatPerusahaan.Text) Then
            MessageBox.Show("Alamat perusahaan tidak boleh kosong")
            TxtAlamatPerusahaan.Focus()
            Return False
        End If
        Return True
    End Function


    Public Sub Simpandataperusahaan()
        Debug.WriteLine("[Company] Simpan dimulai")
        Debug.WriteLine($"[Company] TxtNamaPerusahaan='{TxtNamaPerusahaan.Text}', TxtKode='{TxtKodePerusahaan.Text}'")
        Debug.WriteLine($"[Company] TxtNamaCloud='{TxtNamaCloud.Text}', TxtAlamat='{TxtAlamatPerusahaan.Text}'")
        If Not ValidasiDataperusahaan() Then
            Debug.WriteLine("[Company] Validasi gagal")
            Return
        End If

        EnsureConnectionReady()
        Debug.WriteLine($"[Company] Koneksi: {conn?.State}")
        Dim transaction As MySqlTransaction = Nothing
        Try
            transaction = conn.BeginTransaction()
            Debug.WriteLine("[Company] Transaction dimulai")

            Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM tbl_perusahaan", conn, transaction)
                Dim adaData As Boolean = Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0
                Debug.WriteLine($"[Company] adaData={adaData}")

                If adaData Then
                    ' Singleton: DELETE lama + INSERT baru — menangani perubahan KODE (PRIMARY KEY)
                    Using cmdDel As New MySqlCommand("DELETE FROM tbl_perusahaan", conn, transaction)
                        cmdDel.ExecuteNonQuery()
                        Debug.WriteLine("[Company] DELETE lama berhasil")
                    End Using
                End If

                ' INSERT (berlaku untuk data baru maupun update setelah DELETE)
                Dim tc As Globalization.TextInfo = New Globalization.CultureInfo("id-ID").TextInfo
                Dim query As String =
                    "INSERT INTO tbl_perusahaan (" &
                    "KODE, KODE_CLOUD, NAMA_CLOUD, ALAMAT_CLOUD, " &
                    "NAMA, ALAMAT, KOTA, HP, PEMILIK, " &
                    "FOOTER1, FOOTER2, FOOTER3, " &
                    "SYSTEM_TUTUP_BULAN, TANGGAL_TUTUP_BULAN, " &
                    "NAMA_REK_BARANG, KODE_REK_BARANG, " &
                    "LAWAN_NAMA_REK_BARANG, LAWAN_KODE_REK_BARANG, " &
                    "NAMA_REK_BELI_TOKO, KODE_REK_BELI_TOKO, " &
                    "NAMA_REK_BELI_GUDANG, KODE_REK_BELI_GUDANG, " &
                    "NAMA_REK_JUAL_TOKO, KODE_REK_JUAL_TOKO, " &
                    "NAMA_REK_JUAL_GUDANG, KODE_REK_JUAL_GUDANG, " &
                    "NAMA_REK_HUTANG_BELI, KODE_REK_HUTANG_BELI, " &
                    "NAMA_REK_PIUTANG_JUAL, KODE_REK_PIUTANG_JUAL, " &
                    "NAMA_REK_RETUR_PEMBELIAN_TOKO, KODE_REK_RETUR_PEMBELIAN_TOKO, " &
                    "NAMA_REK_RETUR_PENJUALAN_TOKO, KODE_REK_RETUR_PENJUALAN_TOKO, " &
                    "NAMA_REK_RETUR_PEMBELIAN_GUDANG, KODE_REK_RETUR_PEMBELIAN_GUDANG, " &
                    "NAMA_REK_RETUR_PENJUALAN_GUDANG, KODE_REK_RETUR_PENJUALAN_GUDANG, " &
                    "NAMA_REK_BON_KARYAWAN, KODE_REK_BON_KARYAWAN, " &
                    "NAMA_REK_GAJI_KARYAWAN, KODE_REK_GAJI_KARYAWAN, " &
                    "NAMA_REK_BAYAR_HUTANG, KODE_REK_BAYAR_HUTANG, " &
                    "NAMA_REK_BAYAR_PIUTANG, KODE_REK_BAYAR_PIUTANG, " &
                    "NAMA_REK_TRANSFER_JUAL, KODE_REK_TRANSFER_JUAL) " &
                    "VALUES (" &
                    "@KODE, @KODE_CLOUD, @NAMA_CLOUD, @ALAMAT_CLOUD, " &
                    "@NAMA, @ALAMAT, @KOTA, @HP, @PEMILIK, " &
                    "@FOOTER1, @FOOTER2, @FOOTER3, " &
                    "@SYSTEM_TUTUP_BULAN, @TANGGAL_TUTUP_BULAN, " &
                    "@NAMA_REK_BARANG, @KODE_REK_BARANG, " &
                    "@LAWAN_NAMA_REK_BARANG, @LAWAN_KODE_REK_BARANG, " &
                    "@NAMA_REK_BELI_TOKO, @KODE_REK_BELI_TOKO, " &
                    "@NAMA_REK_BELI_GUDANG, @KODE_REK_BELI_GUDANG, " &
                    "@NAMA_REK_JUAL_TOKO, @KODE_REK_JUAL_TOKO, " &
                    "@NAMA_REK_JUAL_GUDANG, @KODE_REK_JUAL_GUDANG, " &
                    "@NAMA_REK_HUTANG_BELI, @KODE_REK_HUTANG_BELI, " &
                    "@NAMA_REK_PIUTANG_JUAL, @KODE_REK_PIUTANG_JUAL, " &
                    "@NAMA_REK_RETUR_PEMBELIAN_TOKO, @KODE_REK_RETUR_PEMBELIAN_TOKO, " &
                    "@NAMA_REK_RETUR_PENJUALAN_TOKO, @KODE_REK_RETUR_PENJUALAN_TOKO, " &
                    "@NAMA_REK_RETUR_PEMBELIAN_GUDANG, @KODE_REK_RETUR_PEMBELIAN_GUDANG, " &
                    "@NAMA_REK_RETUR_PENJUALAN_GUDANG, @KODE_REK_RETUR_PENJUALAN_GUDANG, " &
                    "@NAMA_REK_BON_KARYAWAN, @KODE_REK_BON_KARYAWAN, " &
                    "@NAMA_REK_GAJI_KARYAWAN, @KODE_REK_GAJI_KARYAWAN, " &
                    "@NAMA_REK_BAYAR_HUTANG, @KODE_REK_BAYAR_HUTANG, " &
                    "@NAMA_REK_BAYAR_PIUTANG, @KODE_REK_BAYAR_PIUTANG, " &
                    "@NAMA_REK_TRANSFER_JUAL, @KODE_REK_TRANSFER_JUAL)"

                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.AddWithValue("@KODE", TxtKodePerusahaan.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@KODE_CLOUD", TxtKodeCloud.Text.Trim())
                    cmd.Parameters.AddWithValue("@NAMA_CLOUD", TxtNamaCloud.Text.Trim())
                    cmd.Parameters.AddWithValue("@ALAMAT_CLOUD", TxtAlamatCloud.Text.Trim())
                    cmd.Parameters.AddWithValue("@NAMA", TxtNamaPerusahaan.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@ALAMAT", tc.ToTitleCase(TxtAlamatPerusahaan.Text.ToLower()))
                    cmd.Parameters.AddWithValue("@KOTA", tc.ToTitleCase(TxtKotaPerusahaan.Text.ToLower()))
                    cmd.Parameters.AddWithValue("@HP", tc.ToTitleCase(TxtKontakPerusahaan.Text.ToLower()))
                    cmd.Parameters.AddWithValue("@PEMILIK", TxtPemilikPerusahaan.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@FOOTER1", tc.ToTitleCase(TxtFooter1.Text.ToLower()))
                    cmd.Parameters.AddWithValue("@FOOTER2", tc.ToTitleCase(TxtFooter2.Text.ToLower()))
                    cmd.Parameters.AddWithValue("@FOOTER3", tc.ToTitleCase(TxtFooter3.Text.ToLower()))
                    cmd.Parameters.AddWithValue("@SYSTEM_TUTUP_BULAN", CmbTutupBulan.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL_TUTUP_BULAN", ModuleAngka.ParseInteger(TxtTutupBulan.Text, defaultValue:=1))
                    cmd.Parameters.AddWithValue("@NAMA_REK_BARANG", CmbRekBarang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_BARANG", TxtRekBarang.Text)
                    cmd.Parameters.AddWithValue("@LAWAN_NAMA_REK_BARANG", CmbLawanRekBarang.Text)
                    cmd.Parameters.AddWithValue("@LAWAN_KODE_REK_BARANG", TxtLawanRekBarang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_BELI_TOKO", CmbBeliToko.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_BELI_TOKO", TxtBeliToko.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_BELI_GUDANG", CmbBeliGudang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_BELI_GUDANG", TxtBeliGudang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_JUAL_TOKO", CmbJualToko.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_JUAL_TOKO", TxtJualToko.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_JUAL_GUDANG", CmbJualGudang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_JUAL_GUDANG", TxtJualGudang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_HUTANG_BELI", CmbHutangBeli.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_HUTANG_BELI", TxtHutangBeli.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_PIUTANG_JUAL", CmbPiutangJual.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_PIUTANG_JUAL", TxtPiutangJual.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_RETUR_PEMBELIAN_TOKO", CmbReturPembelianToko.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_RETUR_PEMBELIAN_TOKO", TxtReturPembelianToko.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_RETUR_PENJUALAN_TOKO", CmbReturPenjualanToko.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_RETUR_PENJUALAN_TOKO", TxtReturPenjualanToko.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_RETUR_PEMBELIAN_GUDANG", CmbReturPembelianGudang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_RETUR_PEMBELIAN_GUDANG", TxtReturPembelianGudang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_RETUR_PENJUALAN_GUDANG", CmbReturPenjualanGudang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_RETUR_PENJUALAN_GUDANG", TxtReturPenjualanGudang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_BON_KARYAWAN", CmbBonKaryawan.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_BON_KARYAWAN", TxtBonKaryawan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_GAJI_KARYAWAN", CmbGajiKaryawan.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_GAJI_KARYAWAN", TxtGajiKaryawan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_BAYAR_HUTANG", CmbBayarHutang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_BAYAR_HUTANG", TxtBayarHutang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_BAYAR_PIUTANG", CmbBayarPiutang.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_BAYAR_PIUTANG", TxtBayarPiutang.Text)
                    cmd.Parameters.AddWithValue("@NAMA_REK_TRANSFER_JUAL", CmbTransferJual.Text)
                    cmd.Parameters.AddWithValue("@KODE_REK_TRANSFER_JUAL", TxtTransferJual.Text)
                    Debug.WriteLine($"[Company] Parameter count={cmd.Parameters.Count}, KODE={TxtKodePerusahaan.Text}")
                    cmd.ExecuteNonQuery()
                    Debug.WriteLine("[Company] INSERT berhasil")
                End Using

                ' Simpan kode ke sync_config hanya saat INSERT pertama (bukan update)
                If Not adaData Then
                    SyncConfig.SetNilai("kode_perusahaan_lokal", TxtKodePerusahaan.Text.ToUpper())
                End If
            End Using

            transaction.Commit()
            Debug.WriteLine("[Company] Commit berhasil")
            Ambildataperusahaan()
            AmbilDataPeriodeTanggal()
            If SupabaseHelper.IsInitialized() Then SyncManager.UploadSnapshotCabang()
            FormUtama.Text = "KASIR LANCAR " & FormUtama.StatusLokasi.Text & " " & TxtNamaPerusahaan.Text.ToUpper()
            MessageBox.Show("Data berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            Debug.WriteLine($"[Company] ERROR: {ex.GetType().Name}: {ex.Message}")
            If ex.InnerException IsNot Nothing Then
                Debug.WriteLine($"[Company] INNER: {ex.InnerException.Message}")
            End If
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If transaction IsNot Nothing Then transaction.Rollback()
        End Try
    End Sub

    Private Sub BtnSimpan_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        Simpandataperusahaan()
    End Sub

    Private Sub FormProfilPerusahaan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpan.PerformClick()
            Case Keys.Escape
                BtnClose.PerformClick()
        End Select
    End Sub

    Private Sub CmbTutupBulan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbTutupBulan.SelectedIndexChanged
        If CmbTutupBulan.SelectedIndex = 0 Then
            TxtTutupBulan.Visible = False
            LblTutupBulan.Visible = False
            TxtTutupBulan.Text = 1
        ElseIf CmbTutupBulan.SelectedIndex = 1 Then
            TxtTutupBulan.Visible = True
            LblTutupBulan.Visible = True
        End If
    End Sub

    Private Sub BtnNota_Click(sender As Object, e As EventArgs) Handles BtnNota.Click
        PilihDanTampilkanGambar(PBNota, "LOGO.PNG")
    End Sub

    Private Sub BtnToko_Click(sender As Object, e As EventArgs) Handles BtnToko.Click
        PilihDanTampilkanGambar(PBToko, "TOKO.JPG")
    End Sub

    Private Sub BtnGudang_Click(sender As Object, e As EventArgs) Handles BtnGudang.Click
        PilihDanTampilkanGambar(PBGudang, "GUDANG.JPG")
    End Sub
    Private Sub PilihDanTampilkanGambar(picBox As PictureBox, namaFileTujuan As String)
        Using dlg As New OpenFileDialog With {
        .Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
        .Title = "Pilih Gambar",
        .RestoreDirectory = True
    }
            If dlg.ShowDialog() <> DialogResult.OK Then Return

            Try
                ' 1. Optimasi pembebasan gambar lama
                DisposeImage(picBox.Image)
                picBox.Image = Nothing

                ' 2. Salin file dengan optimasi
                Dim tujuan As String = Path.Combine(Application.StartupPath, namaFileTujuan)
                File.Copy(dlg.FileName, tujuan, True)

                ' 3. Load gambar dengan cara lebih efisien
                picBox.Image = LoadImageWithoutLocking(dlg.FileName)

                ' 4. Perbarui background dengan optimasi
                Dim lokasi As String = FormUtama.StatusLokasi.Text
                Dim bgImage As String = If(lokasi = "TOKO", "Toko.jpg", "Gudang.jpg")
                GantiBackground(bgImage)

                MessageBox.Show("Gambar berhasil diperbarui!", "Sukses",
                         MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As IOException When ex.HResult = &H80070020 ' File sedang digunakan
                MessageBox.Show("File sedang digunakan oleh proses lain.", "Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Error)
            Catch ex As Exception
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}", "Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Function LoadImageWithoutLocking(filePath As String) As Image
        ' Menggunakan FileStream dengan buffering optimal
        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan)
            Dim buffer As Byte() = New Byte(fs.Length - 1) {}
            fs.Read(buffer, 0, buffer.Length)

            ' Membuat MemoryStream dari buffer
            Using ms As New MemoryStream(buffer)
                ' Membuat salinan independen
                Return New Bitmap(ms)
            End Using
        End Using
    End Function

    Private Sub GantiBackground(bgImage As String)
        Try
            Dim fullPath As String = Path.Combine(Application.StartupPath, bgImage)

            If Not File.Exists(fullPath) Then
                MessageBox.Show($"Gambar latar '{bgImage}' tidak ditemukan.", "Perhatian",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If FormUtama.IsHandleCreated Then
                If FormUtama.InvokeRequired Then
                    FormUtama.Invoke(Sub() FormUtama.ChangeBackgroundImage(bgImage))
                Else
                    FormUtama.ChangeBackgroundImage(bgImage)
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal memperbarui latar belakang: " & ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DisposeImage(img As Image)
        If img IsNot Nothing Then
            Try
                img.Dispose()
            Catch
                ' Ignore disposal errors
            End Try
        End If
    End Sub

    Private Sub BtnDefault_Click(sender As Object, e As EventArgs) Handles BtnDefault.Click
        IsiNilaiDefault()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub


End Class
