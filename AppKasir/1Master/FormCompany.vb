Imports System.Globalization
Imports System.IO



Public Class FormCompany
    Private Sub FormProfilPerusahaan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Dim Toko As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "Toko", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpan.Visible = Toko(2) ' CanEdit 
        BtnHapus.Visible = Toko(3) ' CanDelete 

        ' Panggil untuk mengambil data rekening KAS dan BANK
        Rekeningkasbank()
        ' Isi ComboBox dengan data dari list
        CmbJualToko.Items.AddRange(GetAkunList().ToArray())
        CmbJualGudang.Items.AddRange(GetAkunList().ToArray())

        Kosongdataperusahaan()
        IsiComboBoxAkun()
        Ambildataperusahaan()


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



    Public Sub IsiComboBoxAkun()
        CmbRekBarang.Items.Clear()
        CmbLawanRekBarang.Items.Clear()
        CmbJualToko.Items.Clear()
        CmbJualGudang.Items.Clear()
        CmbBeliToko.Items.Clear()
        CmbBeliGudang.Items.Clear()
        CmbHutangBeli.Items.Clear()
        CmbPiutangJual.Items.Clear()

        Dim queries As New Dictionary(Of ComboBox, String) From {
            {CmbRekBarang, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'A LANCAR' ORDER BY Kode_akun ASC"},
            {CmbLawanRekBarang, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'EKUITAS' ORDER BY Kode_akun ASC"},
            {CmbJualToko, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' ORDER BY Kode_akun ASC"},
            {CmbJualGudang, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' ORDER BY Kode_akun ASC"},
            {CmbBeliToko, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'EKUITAS' ORDER BY Kode_akun ASC"},
            {CmbBeliGudang, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'EKUITAS' ORDER BY Kode_akun ASC"},
            {CmbHutangBeli, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'HUTANG' ORDER BY Kode_akun ASC"},
            {CmbPiutangJual, "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'A LANCAR' ORDER BY Kode_akun ASC"}
        }

        For Each kvp As KeyValuePair(Of ComboBox, String) In queries
            Using cmd As New MySqlCommand(kvp.Value, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            kvp.Key.Items.Add(rd("Nama_Akun").ToString())
                        End While
                    End If
                End Using
            End Using
        Next
    End Sub


    Private Sub SetKodeAkunFromComboBox(ByRef comboBox As ComboBox, ByRef textBox As TextBox)
        Using cmd As New MySqlCommand("SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = ?", conn)
            cmd.Parameters.AddWithValue("Nama_Akun", comboBox.Text)

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


    Public Sub Ambildataperusahaan()
        AmbilDataMasterPerusahaan()
        TxtKode.Text = KODE_PERUSAHAAN
        txtnamatoko.Text = NAMA_PERUSAHAAN
        Txtalamattoko.Text = ALAMAT_PERUSAHAAN
        Txtkotatoko.Text = KOTA_PERUSAHAAN
        Txtkontaktoko.Text = KONTAK_PERUSAHAAN
        TxtPemilik.Text = PEMILIK_PERUSAHAAN
        txtfoter1.Text = FOOTER1
        txtfoter2.Text = FOOTER2
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

        TxtKode.Enabled = False
        'Else
        '    TxtKode.Clear()
        '    txtnamatoko.Clear()
        '    Txtalamattoko.Clear()
        '    Txtkontaktoko.Clear()
        '    Txtkotatoko.Clear()
        '    TxtPemilik.Clear()
        '    txtfoter1.Clear()
        '    txtfoter2.Clear()
        '    CmbLawanRekBarang.SelectedIndex = -1
        '    TxtLawanRekBarang.Clear()
        '    TxtBeliToko.Clear()
        '    CmbBeliToko.SelectedIndex = -1
        '    TxtBeliGudang.Clear()
        '    CmbBeliGudang.SelectedIndex = -1
        '    TxtJualToko.Clear()
        '    CmbJualToko.SelectedIndex = -1
        '    TxtJualGudang.Clear()
        '    CmbJualGudang.SelectedIndex = -1
        '    TxtHutangBeli.Clear()
        '    CmbHutangBeli.SelectedIndex = -1
        '    TxtPiutangJual.Clear()
        '    CmbPiutangJual.SelectedIndex = -1
        '    CmbTutupBulan.SelectedIndex = 0
        '    TxtTutupBulan.Clear()
        '    TxtKode.Enabled = True
        'End If

    End Sub

    Private Function ValidasiDataperusahaan() As Boolean
        If String.IsNullOrEmpty(TxtKode.Text) Then
            MessageBox.Show("Kode perusahaan tidak boleh kosong")
            TxtKode.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(txtnamatoko.Text) Then
            MessageBox.Show("Nama perusahaan tidak boleh kosong")
            txtnamatoko.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(Txtalamattoko.Text) Then
            MessageBox.Show("Alamat perusahaan tidak boleh kosong")
            Txtalamattoko.Focus()
            Return False
        End If
        Return True
    End Function


    Public Sub Simpandataperusahaan()
        If ValidasiDataperusahaan() Then
            Dim transaction As MySqlTransaction = Nothing

            Try
                ' Memulai transaksi
                transaction = conn.BeginTransaction()

                ' Mengecek apakah data perusahaan sudah ada sebelum menyimpan
                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM tbl_perusahaan", conn, transaction)
                    Dim dataCount As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                    If dataCount > 0 Then
                        ' Jika data perusahaan sudah ada, lakukan proses edit data
                        Dim query As String = "UPDATE TBL_PERUSAHAAN SET " &
                                              "NAMA=@NAMA, ALAMAT=@ALAMAT, KOTA=@KOTA, HP=@HP, PEMILIK=@PEMILIK, FOOTER1=@FOOTER1, FOOTER2=@FOOTER2, FOOTER3=@FOOTER3," &
                                              "NAMA_REK_BARANG=@NAMA_REK_BARANG, KODE_REK_BARANG=@KODE_REK_BARANG, LAWAN_NAMA_REK_BARANG=@LAWAN_NAMA_REK_BARANG, " &
                                              "LAWAN_KODE_REK_BARANG=@LAWAN_KODE_REK_BARANG, NAMA_REK_BELI_TOKO=@NAMA_REK_BELI_TOKO, KODE_REK_BELI_TOKO=@KODE_REK_BELI_TOKO, " &
                                              "NAMA_REK_BELI_GUDANG=@NAMA_REK_BELI_GUDANG, KODE_REK_BELI_GUDANG=@KODE_REK_BELI_GUDANG, NAMA_REK_JUAL_TOKO=@NAMA_REK_JUAL_TOKO, " &
                                              "KODE_REK_JUAL_TOKO=@KODE_REK_JUAL_TOKO, NAMA_REK_JUAL_GUDANG=@NAMA_REK_JUAL_GUDANG, KODE_REK_JUAL_GUDANG=@KODE_REK_JUAL_GUDANG, " &
                                              "NAMA_REK_HUTANG_BELI=@NAMA_REK_HUTANG_BELI, KODE_REK_HUTANG_BELI=@KODE_REK_HUTANG_BELI, NAMA_REK_PIUTANG_JUAL=@NAMA_REK_PIUTANG_JUAL, " &
                                              "KODE_REK_PIUTANG_JUAL=@KODE_REK_PIUTANG_JUAL, SYSTEM_TUTUP_BULAN=@SYSTEM_TUTUP_BULAN, TANGGAL_TUTUP_BULAN=@TANGGAL_TUTUP_BULAN " &
                                              "WHERE KODE=@KODE"

                        Using cmd As New MySqlCommand(query, conn, transaction)
                            cmd.Parameters.AddWithValue("@NAMA", txtnamatoko.Text.ToUpper())
                            cmd.Parameters.AddWithValue("@ALAMAT", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Txtalamattoko.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@KOTA", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Txtkotatoko.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@HP", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Txtkontaktoko.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@PEMILIK", TxtPemilik.Text.ToUpper())
                            cmd.Parameters.AddWithValue("@FOOTER1", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtfoter1.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@FOOTER2", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtfoter2.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@FOOTER3", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TxtFooter3.Text.ToLower()))
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
                            cmd.Parameters.AddWithValue("@SYSTEM_TUTUP_BULAN", CmbTutupBulan.Text)

                            Dim tanggalTutupBulan As Integer = If(String.IsNullOrEmpty(TxtTutupBulan.Text), 1, Integer.Parse(TxtTutupBulan.Text))
                            cmd.Parameters.AddWithValue("@TANGGAL_TUTUP_BULAN", tanggalTutupBulan)
                            cmd.Parameters.AddWithValue("@KODE", TxtKode.Text.ToUpper())

                            cmd.ExecuteNonQuery()
                            MessageBox.Show("Data berhasil diperbarui!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End Using

                    Else
                        Dim query As String = "INSERT INTO tbl_perusahaan (KODE, NAMA, ALAMAT, KOTA, HP, PEMILIK, FOOTER1, FOOTER2, FOOTER3, " &
                       "NAMA_REK_BARANG, KODE_REK_BARANG, LAWAN_NAMA_REK_BARANG, LAWAN_KODE_REK_BARANG, " &
                       "NAMA_REK_BELI_TOKO, KODE_REK_BELI_TOKO, NAMA_REK_BELI_GUDANG, KODE_REK_BELI_GUDANG, " &
                       "NAMA_REK_JUAL_TOKO, KODE_REK_JUAL_TOKO, NAMA_REK_JUAL_GUDANG, KODE_REK_JUAL_GUDANG, " &
                       "NAMA_REK_HUTANG_BELI, KODE_REK_HUTANG_BELI, NAMA_REK_PIUTANG_JUAL, KODE_REK_PIUTANG_JUAL, " &
                       "SYSTEM_TUTUP_BULAN, TANGGAL_TUTUP_BULAN) " &
                       "VALUES (@KODE, @NAMA, @ALAMAT, @KOTA, @HP, @PEMILIK, @FOOTER1, @FOOTER2, @FOOTER3, @NAMA_REK_BARANG, @KODE_REK_BARANG, " &
                       "@LAWAN_NAMA_REK_BARANG, @LAWAN_KODE_REK_BARANG, @NAMA_REK_BELI_TOKO, @KODE_REK_BELI_TOKO, " &
                       "@NAMA_REK_BELI_GUDANG, @KODE_REK_BELI_GUDANG, @NAMA_REK_JUAL_TOKO, @KODE_REK_JUAL_TOKO, " &
                       "@NAMA_REK_JUAL_GUDANG, @KODE_REK_JUAL_GUDANG, @NAMA_REK_HUTANG_BELI, @KODE_REK_HUTANG_BELI, " &
                       "@NAMA_REK_PIUTANG_JUAL, @KODE_REK_PIUTANG_JUAL, @SYSTEM_TUTUP_BULAN, @TANGGAL_TUTUP_BULAN)"

                        Using cmd As New MySqlCommand(query, conn, transaction)
                            cmd.Parameters.AddWithValue("@KODE", TxtKode.Text.ToUpper())
                            cmd.Parameters.AddWithValue("@NAMA", txtnamatoko.Text.ToUpper())
                            cmd.Parameters.AddWithValue("@ALAMAT", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Txtalamattoko.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@KOTA", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Txtkotatoko.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@HP", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Txtkontaktoko.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@PEMILIK", TxtPemilik.Text.ToUpper())
                            cmd.Parameters.AddWithValue("@FOOTER1", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtfoter1.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@FOOTER2", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtfoter2.Text.ToLower()))
                            cmd.Parameters.AddWithValue("@FOOTER3", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TxtFooter3.Text.ToLower()))
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
                            cmd.Parameters.AddWithValue("@SYSTEM_TUTUP_BULAN", CmbTutupBulan.Text)

                            Dim tanggalTutupBulan As Integer = If(String.IsNullOrEmpty(TxtTutupBulan.Text), 1, Integer.Parse(TxtTutupBulan.Text))
                            cmd.Parameters.AddWithValue("@TANGGAL_TUTUP_BULAN", tanggalTutupBulan)

                            cmd.ExecuteNonQuery()
                        End Using

                        MessageBox.Show("Data berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using



                ' Commit transaksi jika berhasil
                transaction.Commit()
                Ambildataperusahaan()
                AmbilDataMasterPerusahaan()
                AmbilDataPeriodeTanggal()
                DatabaseModule.CatatanAksiHistory("Update perusahaan " & txtnamatoko.Text)
                ' Pengaturan judul form
                FormUtama.Text = "KASIR LANCAR " & FormUtama.SLokasi.Text & " " & txtnamatoko.Text.ToUpper()
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                transaction.Rollback()
            End Try


        End If
    End Sub



    Public Sub Hapusdataperusahaan()
        If ValidasiDataperusahaan() Then
            If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Using cmd As New MySqlCommand("DELETE FROM tbl_perusahaan WHERE Kode = ?", conn)
                    cmd.Parameters.AddWithValue("?", TxtKode.Text)
                    cmd.ExecuteNonQuery()
                    Kosongdataperusahaan()
                End Using
            End If
        End If
    End Sub

    Private Sub Kosongdataperusahaan()
        TxtKode.Clear()
        txtnamatoko.Clear()
        Txtalamattoko.Clear()
        Txtkontaktoko.Clear()
        Txtkotatoko.Clear()
        txtfoter1.Clear()
        txtfoter2.Clear()
        TxtFooter3.Clear()
    End Sub


    Private Sub BtnSimpan_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        Simpandataperusahaan()
    End Sub



    Private Sub BtnHapus_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHapus.Click
        Hapusdataperusahaan()
        Ambildataperusahaan()
        DatabaseModule.CatatanAksiHistory("Hapus perusahaan " & txtnamatoko.Text)
    End Sub

    'Private Sub FormProfilPerusahaan_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
    '    Using i As New System.Drawing.Drawing2D.LinearGradientBrush(ClientRectangle, Color.MediumPurple, Color.ForestGreen, Drawing2D.LinearGradientMode.BackwardDiagonal)
    '        e.Graphics.FillRectangle(i, ClientRectangle)
    '    End Using
    'End Sub

    Private Sub FormProfilPerusahaan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpan.PerformClick()
            Case Keys.F3
                BtnHapus.PerformClick()
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

    Private Function LoadImage(ByVal fileName As String) As Image
        Dim imagePath As String = Path.Combine(Application.StartupPath, fileName)

        If File.Exists(imagePath) Then
            Using fs As New FileStream(imagePath, FileMode.Open, FileAccess.Read)
                Return Image.FromStream(fs)
            End Using
        Else
            Return Nothing
        End If
    End Function

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
                Dim lokasi As String = FormUtama.SLokasi.Text
                Dim bgImage As String = If(lokasi = "TOKO", "Toko.jpg", "Gudang.jpg")
                GantiBackground(bgImage, lokasi)

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

    Private Sub GantiBackground(bgImage As String, lokasi As String)
        Try
            Dim fullPath As String = Path.Combine(Application.StartupPath, bgImage)

            If Not File.Exists(fullPath) Then
                MessageBox.Show($"Gambar latar '{bgImage}' tidak ditemukan.", "Perhatian",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Load gambar ke dalam memory tanpa mengunci file
            Dim newBg As Image
            Using fs As New FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                newBg = Image.FromStream(fs)
            End Using

            ' Aman dari thread manapun
            If FormUtama.IsHandleCreated Then
                If FormUtama.InvokeRequired Then
                    FormUtama.Invoke(Sub()
                                         FormUtama.BackgroundImage?.Dispose()
                                         FormUtama.BackgroundImage = newBg
                                     End Sub)
                Else
                    FormUtama.BackgroundImage?.Dispose()
                    FormUtama.BackgroundImage = newBg
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


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub


End Class