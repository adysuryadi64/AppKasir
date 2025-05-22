Imports System.IO
Public Class FormPenjualan
    Private jenisprintercetak As String
    Private AwalPenjualan As String
    Private EdithargaJual As String
    Private Kodebarangsama As String
    Private Editmasterhargajual As String

    Private Sub Form_Penjualan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        LblTextJalanAtas.Text = "TERIMA KASIH TELAH BELANJA DI " & NAMA_PERUSAHAAN
        LblLokasiBarang.Text = FormUtama.SLokasi.Text
        ' Set ukuran maksimum dan minimum untuk memastikan form tidak menutupi taskbar
        MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        MinimumSize = Size

        CmbCetak.Text = My.Settings.CetakJual

        KosongTxtboxcari()


        AwalPenjualan = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblJualFokus.Text)
        EdithargaJual = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblJualEditHarga.Text)
        Kodebarangsama = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblJualSatuan.Text)
        Editmasterhargajual = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblEditHargaJual.Text)


        ' Hapus semua item dan tambahkan yang baru
        CmbJenisBayar.Items.Clear()
        ' Isi ComboBox dengan data dari list
        CmbJenisBayar.Items.AddRange(GetAkunList().ToArray())


        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        DTPJatuhTempo.Format = DateTimePickerFormat.Custom
        DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"

        Dim newDate As Date = DTPJatuhTempo.Value.AddMonths(1)
        DTPJatuhTempo.Value = newDate

        AmbilJenisPrinter()


        GBBayar.Visible = False


        If EdithargaJual = "Tidak" Then
            DgvData.Columns("Harga").ReadOnly = True
        Else
            DgvData.Columns("Harga").ReadOnly = False
        End If

        FormatKolomDenganCultureIndonesia()

        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            'TxtFakturDitahan.Text = ""
            LblJenisPl.Text = "Umum"
            CmbPelanggan.Text = ""
            LbLKodePel.Text = ""
            Kondisiawal()
        Else
            Editpenjualanheader()
        End If
    End Sub


    Private Sub FormatKolomDenganCultureIndonesia()
        ' Daftar nama kolom yang akan diformat
        Dim kolomList As String() = {
        "HargaBeli", "QTY", "Isi", "Totalhargabeli", "Harga", "QtySat",
        "DiskonPersen", "DiskonRp", "TotalDiskon", "TotalHarga",
        "StokToko", "StokGudang", "Stok"
    }

        For Each kolom As String In kolomList
            If DgvData.Columns.Contains(kolom) Then
                DgvData.Columns(kolom).DefaultCellStyle.Format = "#,0.##" ' Format angka dengan 2 desimal
                DgvData.Columns(kolom).DefaultCellStyle.FormatProvider = cultureIndonesia
                DgvData.Columns(kolom).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next
    End Sub

    Private Sub CmbCetak_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbCetak.SelectedIndexChanged
        My.Settings.CetakJual = CmbCetak.Text
        My.Settings.Save()
    End Sub

    Private Sub ClearFocus()
        ' Dapatkan kontrol yang sedang memiliki fokus
        Dim currentControl As Control = ActiveControl

        ' Jika ada kontrol yang memiliki fokus, hilangkan fokusnya
        currentControl?.Parent.Focus()

        ' Hilangkan fokus dari seluruh kontrol
        ActiveControl = Nothing
    End Sub


    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCariNama.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan

        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        End If
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtNama_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCariNama.BackColor = SystemColors.ActiveCaption
    End Sub

    Private Sub KosongTxtboxcari()
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHargaBeli.Clear()
        TxtBarcode.Clear()
        TxtHargaJual.Clear()
        TxtStokToko.Clear()
        TXtStokGudang.Clear()
        TxtStok.Clear()
        TxtNama.Clear()
    End Sub

    Public Sub Kondisiawal()
        DgvData.DataSource = Nothing
        DgvData.Rows.Clear()

        TampilPelanggan()
        AmbilDataKaryawan()
        Nomorjual()
        JumlahTahan()
        UpdateSemuaTotal()

        LblSales.Text = ""
        TxtTotalStlPajak.Text = 0
        TxtBayar.Text = 0
        TxtKembali.Text = 0
        LblStatusTrans.Text = "Belum Lunas"

        TxtTotalSblDiskon.Text = 0
        TxtGrantotal.Text = "Rp. 0"
        TxtTotalBelanja.Text = 0
        TxtDiskonPersen.Text = 0
        TxtDiskonRp.Text = 0
        TxtBank.Clear()
        TxtNoRek.Clear()
        TxtNamaRek.Clear()
        TxtNoReff.Clear()

        TxtTypeAkun.Text = "TUNAI"

        If LblLokasiBarang.Text = "TOKO" Then
            CmbJenisBayar.SelectedItem = nama_rek_Jual_Toko
            TxtKodeRef.Text = Kode_rek_Jual_Toko
        ElseIf LblLokasiBarang.Text = "GUDANG" Then
            CmbJenisBayar.SelectedItem = nama_rek_Jual_Gudang
            TxtKodeRef.Text = Kode_rek_Jual_Gudang
        End If

        GBBayar.Visible = False

        TxtLogin.Clear()
        TxtKomputer.Clear()


        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        End If

        ' Jika AwalPenjualan adalah "Pencarian", pilih TxtNama
        If AwalPenjualan = "Pencarian" Then
            TxtNama.Select()
            TxtNama.Focus()
        End If
    End Sub

    Public Sub AmbilJenisPrinter()
        Dim filePath As String = "printer.ini"

        Using reader As New StreamReader(filePath)
            Do While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()
                If Not String.IsNullOrEmpty(line) Then
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 AndAlso parts(0).Trim() = "JenisPrinterJual" Then
                        jenisprintercetak = parts(1).Trim()
                        Exit Do
                    End If
                End If
            Loop
        End Using
    End Sub

    Public Sub TampilPelanggan()
        Using cmd As New MySqlCommand("SELECT NAMA FROM tbl_pelanggan ORDER BY NAMA ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbPelanggan.Items.Clear()
                CmbPelanggan.Items.Add("")
                While rd.Read()
                    CmbPelanggan.Items.Add(rd("NAMA").ToString())
                End While
            End Using
        End Using
        CmbPelanggan.SelectedIndex = 0
        ' Add validation event handler
        AddHandler CmbPelanggan.Validating, AddressOf ComboBox_Validating
    End Sub

    Private Sub ComboBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        Dim comboBox As ComboBox = CType(sender, ComboBox)
        If Not comboBox.Items.Contains(comboBox.Text) Then
            MessageBox.Show("Harap pilih nama pelanggan yang valid dari daftar.", "Pilihan pelanggan Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
        End If
    End Sub

    Public Sub Nomorjual()
        Dim cekTanggal As String = Microsoft.VisualBasic.Format(DTPTgl.Value, "yyMMdd")
        Dim ceknomor As String = "PJ-" & cekTanggal
        Dim UrutKode As String = ""

        Using cmd As New MySqlCommand("SELECT MAX(ID_PENJUALAN) FROM penjualan WHERE ID_PENJUALAN LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim maxKode As Object = rd.GetValue(0)
                    If Not IsDBNull(maxKode) Then
                        Dim maxKodePenjualan As String = maxKode.ToString()
                        If Not String.IsNullOrEmpty(maxKodePenjualan) Then
                            If Microsoft.VisualBasic.Left(maxKodePenjualan, 9) = "PJ-" & cekTanggal Then
                                Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(maxKodePenjualan, 4)) + 1
                                UrutKode = "PJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                            End If
                        End If
                    End If
                End If
            End Using
        End Using

        Using cmdTemp As New MySqlCommand("SELECT MAX(FAKTUR_JUAL) FROM penjualan_ditahan WHERE FAKTUR_JUAL LIKE @ceknomor", conn)
            cmdTemp.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Using rdTemp As MySqlDataReader = cmdTemp.ExecuteReader()
                If rdTemp.Read() Then
                    Dim maxKodeTemp As Object = rdTemp.GetValue(0)
                    If Not IsDBNull(maxKodeTemp) Then
                        Dim maxKode As String = maxKodeTemp.ToString()
                        If Not String.IsNullOrEmpty(maxKode) Then
                            If Microsoft.VisualBasic.Left(maxKode, 9) = "PJ-" & cekTanggal Then
                                Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(maxKode, 4)) + 1
                                If UrutKode = "" Then
                                    UrutKode = "PJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                                Else
                                    Dim maxKodeUrut As String = "PJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                                    If String.Compare(maxKodeUrut, UrutKode) > 0 Then
                                        UrutKode = maxKodeUrut
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End Using
        End Using


        If UrutKode = "" Then
            UrutKode = "PJ-" & cekTanggal & "0001"
        End If

        TxtFaktur.Text = UrutKode
    End Sub

    Public Sub AmbilDataKaryawan()
        CmbSales.Items.Clear()
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan ORDER BY Nama ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        CmbSales.Items.Add(rd("Nama").ToString())
                    End While
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbSales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSales.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi sales
        AmbilInformasiSales()
    End Sub

    Private Sub CmbSales_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSales.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            ' Panggil metode untuk mengambil informasi sales ketika Enter ditekan
            ' Cek apakah DgvData memiliki baris
            If DgvData.Rows.Count > 0 Then
                ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

                ' Mengatur baris terakhir sebagai baris yang dipilih
                DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
            End If

            ' Jika AwalPenjualan adalah "Pencarian", pilih TxtNama
            If AwalPenjualan = "Pencarian" Then
                TxtNama.Select()
                TxtNama.Focus()
            End If
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub AmbilInformasiSales()
        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbSales.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblSales.Text = reader("Kode").ToString()
                Else
                    LblSales.Text = ""
                End If
            End Using
        End Using


    End Sub

    Private Sub CmbPelanggan_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbPelanggan.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi pelanggan
        AmbilInformasiPelanggan()
    End Sub

    Private Sub CmbPelanggan_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbPelanggan.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            ' Cek apakah DgvData memiliki baris
            If DgvData.Rows.Count > 0 Then
                ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

                ' Mengatur baris terakhir sebagai baris yang dipilih
                DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
            End If

            ' Jika AwalPenjualan adalah "Pencarian", pilih TxtNama
            If AwalPenjualan = "Pencarian" Then
                TxtNama.Select()
            End If
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub AmbilInformasiPelanggan()
        ' Deklarasi variabel untuk menyimpan nilai
        Dim jenisPelanggan As String = "Umum"
        Dim kodePelanggan As String = ""
        Dim alamatPelanggan As String = ""
        Dim jangkaPiutang As Integer = 0

        ' Mengambil informasi pelanggan dari database
        Using cmd As New MySqlCommand("SELECT KODE, ALAMAT, JENIS, JangkaPiutang FROM tbl_pelanggan WHERE nama = @nama", conn)
            cmd.Parameters.AddWithValue("@nama", CmbPelanggan.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Simpan nilai dari database ke variabel
                    jenisPelanggan = If(rd.IsDBNull(rd.GetOrdinal("JENIS")), "Umum", rd.Item("JENIS").ToString())
                    kodePelanggan = If(rd.IsDBNull(rd.GetOrdinal("KODE")), "", rd.Item("KODE").ToString())
                    alamatPelanggan = If(rd.IsDBNull(rd.GetOrdinal("ALAMAT")), "", rd.Item("ALAMAT").ToString())
                    jangkaPiutang = If(rd.IsDBNull(rd.GetOrdinal("JangkaPiutang")), 0, Convert.ToInt32(rd.Item("JangkaPiutang")))
                Else
                    ' Jika tidak ada data, gunakan nilai default (sudah diinisialisasi)
                    jangkaPiutang = 30 ' Default jatuh tempo 30 hari
                End If
            End Using
        End Using

        ' Masukkan nilai ke label di luar blok reader
        LblJenisPl.Text = jenisPelanggan
        LbLKodePel.Text = kodePelanggan
        LblAlamat.Text = alamatPelanggan

        ' Hitung dan set tanggal jatuh tempo
        DTPJatuhTempo.Value = DTPTgl.Value.AddDays(jangkaPiutang)

    End Sub



    Private Sub LblJenisPl_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblJenisPl.TextChanged
        ' Memperbarui data setelah pelanggan dipilih
        UpdateHargaBerdasarJenisPelanggan()
    End Sub

    Private Sub UpdateHargaBerdasarJenisPelanggan()
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                'Mengumpulkan informasi barang
                Dim sql As String = "SELECT HARGA_BELI, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                   "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                   "FROM tbl_barang " &
                   "WHERE TRIM(ID_BARANG) LIKE @ID_BARANG"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            dgvRow.Cells("HargaBeli").Value = If(Not IsDBNull(rd("HARGA_BELI")), ParseDecimal(rd("HARGA_BELI")), DBNull.Value)

                            If Not IsDBNull(dgvRow.Cells("Satuan").Value) Then
                                Dim nilaiSatuan As String = dgvRow.Cells("Satuan").Value.ToString()
                                Dim hargaValue As Decimal

                                If LblJenisPl.Text = "Partai" Then
                                    Select Case nilaiSatuan
                                        Case rd("SATUAN_PARTAI_SEDANG").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_PARTAI_SEDANG"))
                                        Case rd("SATUAN_PARTAI_BESAR").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_PARTAI_BESAR"))
                                        Case Else
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_PARTAI_KECIL"))
                                    End Select
                                Else
                                    Select Case nilaiSatuan
                                        Case rd("SATUAN_UMUM_SEDANG").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_UMUM_SEDANG"))
                                        Case rd("SATUAN_UMUM_BESAR").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_UMUM_BESAR"))
                                        Case Else
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL"))
                                    End Select
                                End If

                                dgvRow.Cells("Harga").Value = hargaValue
                            End If

                            dgvRow.Cells("StokToko").Value = If(Not IsDBNull(rd("STOK_TOKO")), ParseDecimal(rd("STOK_TOKO")), DBNull.Value)
                            dgvRow.Cells("StokGudang").Value = If(Not IsDBNull(rd("STOK_GUDANG")), ParseDecimal(rd("STOK_GUDANG")), DBNull.Value)

                            Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                                          If(Not IsDBNull(rd("STOK_GUDANG")), ParseDecimal(rd("STOK_GUDANG")), 0D),
                                                          If(Not IsDBNull(rd("STOK_TOKO")), ParseDecimal(rd("STOK_TOKO")), 0D))

                            dgvRow.Cells("Stok").Value = stokValue
                        End If
                    End Using
                End Using
            End If
        Next
        Hitungbaris()
        UpdateSemuaTotal()
    End Sub

    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        ProsesInput()
    End Sub

    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' jika listbox hanya satu hasil pencarian langsung panggil
            If LstBarang.Items.Count = 1 Then
                AmbilDataDariListBox()
            ElseIf LstBarang.Items.Count > 0 Then
                LstBarang.Focus()
                LstBarang.SelectedIndex = 0
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Down AndAlso LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            LstBarang.Focus()
            LstBarang.SelectedIndex = 0
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Tab Then
            DgvData.Select()
            DgvData.Focus()
        End If
    End Sub


    Private Sub ProsesInput()
        If Not String.IsNullOrEmpty(TxtNama.Text) Then
            ' Menghitung jumlah huruf alfabet yang valid (hanya huruf, tidak termasuk angka atau tanda baca)
            Dim validLetters As String = ""
            For Each c As Char In TxtNama.Text
                If Char.IsLetter(c) Then
                    validLetters &= c
                End If
            Next

            ' Lanjutkan hanya jika ada setidaknya 2 huruf yang valid
            If validLetters.Length >= 2 Then
                ' Temukan posisi * pertama dan kedua
                Dim indexAsteriskQty As Integer = TxtNama.Text.IndexOf("*")
                Dim indexAsteriskHarga As Integer = -1

                If indexAsteriskQty >= 0 Then
                    ' Cari * kedua setelah * pertama
                    indexAsteriskHarga = TxtNama.Text.IndexOf("*", indexAsteriskQty + 1)
                End If

                If indexAsteriskQty >= 0 And indexAsteriskHarga > indexAsteriskQty Then
                    ' Jika ditemukan dua *, ambil qty, harga, dan nama barang
                    LstBarang.Items.Clear()

                    ' Ambil nilai sebelum * pertama sebagai Qty
                    Dim angkaSebelumAsterisk As String = TxtNama.Text.Substring(0, indexAsteriskQty).Trim()
                    If angkaSebelumAsterisk.Contains(".") Or angkaSebelumAsterisk.Contains(",") Then
                        angkaSebelumAsterisk = angkaSebelumAsterisk.Replace(".", ",")
                        TxtQty.Text = angkaSebelumAsterisk
                    ElseIf Decimal.TryParse(angkaSebelumAsterisk, Nothing) Then
                        TxtQty.Text = angkaSebelumAsterisk
                    Else
                        TxtQty.Text = "1"
                    End If

                    ' Ambil nilai sebelum * kedua sebagai Harga Manual
                    Dim hargaSebelumAsterisk As String = TxtNama.Text.Substring(indexAsteriskQty + 1, indexAsteriskHarga - indexAsteriskQty - 1).Trim()
                    TxtLevelSat.Text = hargaSebelumAsterisk

                    ' Ambil nama barang setelah * kedua
                    Dim searchKeyword As String = TxtNama.Text.Substring(indexAsteriskHarga + 1).Trim()
                    TampilkanDaftarBarang(searchKeyword)

                ElseIf indexAsteriskQty >= 0 Then
                    ' Jika hanya ada satu *, ambil qty dan nama barang
                    LstBarang.Items.Clear()

                    ' Ambil nilai sebelum * sebagai Qty
                    Dim angkaSebelumAsterisk As String = TxtNama.Text.Substring(0, indexAsteriskQty).Trim()
                    If angkaSebelumAsterisk.Contains(".") Or angkaSebelumAsterisk.Contains(",") Then
                        angkaSebelumAsterisk = angkaSebelumAsterisk.Replace(".", ",")
                        TxtQty.Text = angkaSebelumAsterisk
                    ElseIf Decimal.TryParse(angkaSebelumAsterisk, Nothing) Then
                        TxtQty.Text = angkaSebelumAsterisk
                    Else
                        TxtQty.Text = "1"
                    End If

                    ' Ambil nama barang setelah *
                    Dim searchKeyword As String = TxtNama.Text.Substring(indexAsteriskQty + 1).Trim()
                    TampilkanDaftarBarang(searchKeyword)

                    ' Kosongkan harga manual
                    TxtLevelSat.Text = "1"

                Else
                    ' Jika tidak ada *, gunakan nama barang secara keseluruhan
                    TampilkanDaftarBarang(TxtNama.Text)
                    TxtQty.Text = "1"
                    TxtLevelSat.Text = "1"
                End If
            End If
        Else
            ' Jika input kosong, kosongkan list dan text box
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            TxtQty.Text = "1"
            TxtLevelSat.Text = "1"
        End If
    End Sub

    Private Sub TampilkanDaftarBarang(ByVal searchKeyword As String)
        Dim query As String = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR TRIM(BARCODE_BESAR) LIKE @Nama ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Clear ListBox before adding new items
                LstBarang.Items.Clear()
                TxtBarcode.Clear()

                While rd.Read()
                    Dim itemText As String = rd("NAMA_BARANG").ToString()
                    Select Case LblLokasiBarang.Text
                        Case "TOKO"
                            ' Tambahkan stok toko setelah nama barang
                            Dim stokToko As Decimal = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            itemText &= " => " & stokToko.ToString("N0") ' Format stok dengan dua desimal
                        Case "GUDANG"
                            ' Tambahkan stok gudang setelah nama barang
                            Dim stokGudang As Decimal = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
                            itemText &= " => " & stokGudang.ToString("N0") ' Format stok dengan dua desimal
                    End Select

                    ' Check if the searchKeyword matches any barcode field
                    If searchKeyword = rd("BARCODE_SEDANG").ToString() Or searchKeyword = rd("BARCODE_BESAR").ToString() Then
                        ' Set TxtBarcode.Text to the matched barcode value
                        TxtBarcode.Text = searchKeyword
                    End If

                    ' Tambahkan item ke ListBox
                    LstBarang.Items.Add(itemText)
                End While

                ' Tampilkan ListBox hanya jika lebih dari satu hasil pencarian
                LstBarang.Visible = LstBarang.Items.Count > 0


            End Using
        End Using
    End Sub

    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
        If e.KeyCode = Keys.Enter AndAlso LstBarang.SelectedItem IsNot Nothing Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub LstBarang_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub AmbilDataDariListBox()
        Dim namayangdiambil As String

        If LstBarang.Items.Count = 1 OrElse (LstBarang.Items.Count > 1 AndAlso LstBarang.SelectedItem IsNot Nothing) Then
            ' Ambil nilai dari item yang dipilih atau item pertama jika hanya satu
            Dim selectedValue As String = If(LstBarang.Items.Count = 1, LstBarang.Items(0).ToString(), LstBarang.SelectedItem.ToString())

            ' Cari posisi karakter "*" jika ada
            Dim indexAsterisk As Integer = selectedValue.IndexOf("*")

            ' Tentukan nilai namayangdiambil berdasarkan kondisi pertama
            If indexAsterisk >= 0 Then
                namayangdiambil = selectedValue.Substring(0, indexAsterisk).Trim()
            Else
                namayangdiambil = selectedValue
            End If

            ' Mencari posisi karakter " => " jika ada (mengganti - dengan => sesuai dengan kebutuhan Anda)
            Dim indexArrow As Integer = selectedValue.IndexOf(" => ")

            If indexArrow >= 0 Then
                ' Ambil teks sebelum karakter " => "
                namayangdiambil = selectedValue.Substring(0, indexArrow).Trim()
            End If

            ' Panggil fungsi dengan nama yang telah diproses
            Ambildatalaindaridbbarang(namayangdiambil)
        Else
            ' Menampilkan pesan jika tidak ada item yang dipilih atau lebih dari satu item dan tidak ada yang dipilih
            MessageBox.Show("Silakan pilih barang terlebih dahulu!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                            "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                            "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
                            "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
                            "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                            "FROM tbl_barang " &
                            "WHERE TRIM(NAMA_BARANG) LIKE @NamaBarang OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", namayangdiambil)
            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    ' Ambil nilai dari database
                    Dim idBarang As String = If(Not IsDBNull(rd(0)), rd.GetString(0), String.Empty)
                    Dim hargaBeli As String = If(Not IsDBNull(rd(2)), rd.GetDecimal(2).ToString(), String.Empty)

                    TxtKode.Text = idBarang
                    TxtHargaBeli.Text = hargaBeli

                    Dim satuan As String
                    Dim isiSatuan As Integer
                    Dim hargaJual As Decimal

                    If LblJenisPl.Text = "Partai" Then
                        ' Periksa apakah TxtBarcode.Text kosong
                        If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
                            ' Sesuaikan nilai berdasarkan barcode
                            If TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
                                satuan = If(Not IsDBNull(rd(16)), rd.GetString(16), String.Empty) ' SATUAN_PARTAI_SEDANG
                                isiSatuan = If(Not IsDBNull(rd(19)), rd.GetInt32(19), 0)
                                hargaJual = If(Not IsDBNull(rd(22)), rd.GetDecimal(22), 0)
                            ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
                                satuan = If(Not IsDBNull(rd(17)), rd.GetString(17), String.Empty) ' SATUAN_PARTAI_BESAR
                                isiSatuan = If(Not IsDBNull(rd(20)), rd.GetInt32(20), 0)
                                hargaJual = If(Not IsDBNull(rd(23)), rd.GetDecimal(23), 0)
                            Else
                                satuan = If(Not IsDBNull(rd(15)), rd.GetString(15), String.Empty) ' SATUAN_PARTAI_KECIL
                                isiSatuan = If(Not IsDBNull(rd(18)), rd.GetInt32(18), 0)
                                hargaJual = If(Not IsDBNull(rd(21)), rd.GetDecimal(21), 0)
                            End If
                        Else
                            ' Barcode kosong, cek hanya berdasarkan TxtLevelSat.Text
                            If TxtLevelSat.Text = "2" Then
                                satuan = If(Not IsDBNull(rd(16)), rd.GetString(16), String.Empty) ' SATUAN_PARTAI_SEDANG
                                isiSatuan = If(Not IsDBNull(rd(19)), rd.GetInt32(19), 0)
                                hargaJual = If(Not IsDBNull(rd(22)), rd.GetDecimal(22), 0)
                            ElseIf TxtLevelSat.Text = "3" Then
                                satuan = If(Not IsDBNull(rd(17)), rd.GetString(17), String.Empty) ' SATUAN_PARTAI_BESAR
                                isiSatuan = If(Not IsDBNull(rd(20)), rd.GetInt32(20), 0)
                                hargaJual = If(Not IsDBNull(rd(23)), rd.GetDecimal(23), 0)
                            Else
                                satuan = If(Not IsDBNull(rd(15)), rd.GetString(15), String.Empty) ' SATUAN_PARTAI_KECIL
                                isiSatuan = If(Not IsDBNull(rd(18)), rd.GetInt32(18), 0)
                                hargaJual = If(Not IsDBNull(rd(21)), rd.GetDecimal(21), 0)
                            End If
                        End If
                    Else
                        ' Periksa apakah TxtBarcode.Text kosong
                        If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
                            ' Sesuaikan nilai berdasarkan barcode
                            If TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
                                satuan = If(Not IsDBNull(rd(7)), rd.GetString(7), String.Empty) ' SATUAN_UMUM_SEDANG
                                isiSatuan = If(Not IsDBNull(rd(10)), rd.GetInt32(10), 0)
                                hargaJual = If(Not IsDBNull(rd(13)), rd.GetDecimal(13), 0)
                            ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
                                satuan = If(Not IsDBNull(rd(8)), rd.GetString(8), String.Empty) ' SATUAN_UMUM_BESAR
                                isiSatuan = If(Not IsDBNull(rd(11)), rd.GetInt32(11), 0)
                                hargaJual = If(Not IsDBNull(rd(14)), rd.GetDecimal(14), 0)
                            Else
                                satuan = If(Not IsDBNull(rd(6)), rd.GetString(6), String.Empty) ' SATUAN_UMUM_KECIL
                                isiSatuan = If(Not IsDBNull(rd(9)), rd.GetInt32(9), 0)
                                hargaJual = If(Not IsDBNull(rd(12)), rd.GetDecimal(12), 0)
                            End If
                        Else
                            ' Barcode kosong, cek hanya berdasarkan TxtLevelSat.Text
                            If TxtLevelSat.Text = "2" Then
                                satuan = If(Not IsDBNull(rd(7)), rd.GetString(7), String.Empty) ' SATUAN_UMUM_SEDANG
                                isiSatuan = If(Not IsDBNull(rd(10)), rd.GetInt32(10), 0)
                                hargaJual = If(Not IsDBNull(rd(13)), rd.GetDecimal(13), 0)
                            ElseIf TxtLevelSat.Text = "3" Then
                                satuan = If(Not IsDBNull(rd(8)), rd.GetString(8), String.Empty) ' SATUAN_UMUM_BESAR
                                isiSatuan = If(Not IsDBNull(rd(11)), rd.GetInt32(11), 0)
                                hargaJual = If(Not IsDBNull(rd(14)), rd.GetDecimal(14), 0)
                            Else
                                satuan = If(Not IsDBNull(rd(6)), rd.GetString(6), String.Empty) ' SATUAN_UMUM_KECIL
                                isiSatuan = If(Not IsDBNull(rd(9)), rd.GetInt32(9), 0)
                                hargaJual = If(Not IsDBNull(rd(12)), rd.GetDecimal(12), 0)
                            End If
                        End If
                    End If

                    ' Pastikan isiSatuan tidak bernilai nol
                    If isiSatuan = 0 Then
                        isiSatuan = 1
                    End If

                    Txtsatuan.Text = satuan
                    TxtIsi.Text = isiSatuan.ToString()
                    TxtHargaJual.Text = hargaJual.ToString()

                    TxtStokToko.Text = If(Not IsDBNull(rd("STOK_TOKO")), Convert.ToString(rd("STOK_TOKO")), "0")
                    TXtStokGudang.Text = If(Not IsDBNull(rd("STOK_GUDANG")), Convert.ToString(rd("STOK_GUDANG")), "0")

                    If LblLokasiBarang.Text = "GUDANG" Then
                        TxtStok.Text = TXtStokGudang.Text
                    ElseIf LblLokasiBarang.Text = "TOKO" Then
                        TxtStok.Text = TxtStokToko.Text
                    End If
                End If
            End Using
        End Using
        ' Memanggil fungsi tambahan jika diperlukan
        TambahDataLangsung(namayangdiambil)
    End Sub

    Private Sub TambahDataLangsung(ByVal namayangdiambil As String)

        If Kodebarangsama = "Tidak" Then
            For Each row As DataGridViewRow In DgvData.Rows
                If row.Cells("Kode").Value IsNot Nothing AndAlso row.Cells("Kode").Value.ToString() = TxtKode.Text Then
                    MessageBox.Show(namayangdiambil & " sudah ada dalam daftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    'BersihkanPencarian()
                    LstBarang.Select()
                    Exit Sub
                End If
            Next
        End If

        ' Insert data baru ke DataGridView
        Dim indeksBaris As Integer
        If DgvData.SelectedCells.Count > 0 Then
            indeksBaris = DgvData.SelectedCells(0).RowIndex
            DgvData.Rows.Insert(indeksBaris, "")
        Else
            indeksBaris = DgvData.Rows.Add()
        End If

        ' Sekarang tambahkan kode untuk mengisi ComboBoxCell dengan item yang sesuai

        Dim isPartai As Boolean = LblJenisPl.Text = "Partai"
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(indeksBaris).Cells("Satuan"), DataGridViewComboBoxCell)

        ' Membersihkan item yang sudah ada di kolom ComboBoxDataGridView
        kolomSatuan.Items.Clear()

        Dim querySatuanPartai As String = "SELECT DISTINCT SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG LIKE ?"
        Dim querySatuanUmum As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG LIKE ?"

        Dim satuanKecil As String = String.Empty
        Dim satuanSedang As String
        Dim satuanBesar As String


        Dim query As String = If(isPartai, querySatuanPartai, querySatuanUmum)
        Using cmdSatuan As New MySqlCommand(query, conn)
            cmdSatuan.Parameters.AddWithValue("@ID_BARANG", "%" & TxtKode.Text & "%")

            Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                If rdSatuan.HasRows Then
                    While rdSatuan.Read()
                        satuanKecil = If(rdSatuan(0) IsNot DBNull.Value, rdSatuan(0).ToString(), String.Empty)
                        satuanSedang = If(rdSatuan(1) IsNot DBNull.Value, rdSatuan(1).ToString(), String.Empty)
                        satuanBesar = If(rdSatuan(2) IsNot DBNull.Value, rdSatuan(2).ToString(), String.Empty)

                        ' Menambahkan item yang tidak kosong ke ComboBoxDataGridView
                        If Not String.IsNullOrEmpty(satuanKecil) Then
                            kolomSatuan.Items.Add(satuanKecil)
                        End If

                        If Not String.IsNullOrEmpty(satuanSedang) Then
                            kolomSatuan.Items.Add(satuanSedang)
                        End If

                        If Not String.IsNullOrEmpty(satuanBesar) Then
                            kolomSatuan.Items.Add(satuanBesar)
                        End If
                    End While
                End If
            End Using

        End Using


        ' Mengambil nilai dari input lainnya
        Dim hargaBeli As Decimal = If(Not Decimal.TryParse(TxtHargaBeli.Text, hargaBeli), 0D, hargaBeli)
        Dim qty As Decimal = If(Not Decimal.TryParse(TxtQty.Text, qty), 1D, qty)
        Dim isi As Decimal = If(Not Decimal.TryParse(TxtIsi.Text, isi), 0D, Decimal.Parse(TxtIsi.Text))
        Dim hargajual As Decimal = If(Not Decimal.TryParse(TxtHargaJual.Text, hargajual), 0D, Decimal.Parse(TxtHargaJual.Text))
        Dim Stoktoko As Decimal = If(Not Decimal.TryParse(TxtStokToko.Text, Stoktoko), 0D, Stoktoko)
        Dim Stokgudang As Decimal = If(Not Decimal.TryParse(TXtStokGudang.Text, Stokgudang), 0D, Stokgudang)
        Dim Stok As Decimal = If(Not Decimal.TryParse(TxtStok.Text, Stok), 0D, Stok)


        ' Menetapkan nilai untuk baris yang baru ditambahkan
        DgvData.Rows(indeksBaris).Cells("Kode").Value = TxtKode.Text
        DgvData.Rows(indeksBaris).Cells("NamaBarang").Value = namayangdiambil
        DgvData.Rows(indeksBaris).Cells("HargaBeli").Value = hargaBeli
        DgvData.Rows(indeksBaris).Cells("QTY").Value = qty

        If Txtsatuan.Text = "" Then
            DgvData.Rows(indeksBaris).Cells("Satuan").Value = satuanKecil
        Else
            DgvData.Rows(indeksBaris).Cells("Satuan").Value = Txtsatuan.Text
        End If

        DgvData.Rows(indeksBaris).Cells("Isi").Value = isi
        DgvData.Rows(indeksBaris).Cells("Totalhargabeli").Value = hargaBeli * isi * qty
        DgvData.Rows(indeksBaris).Cells("Harga").Value = hargajual
        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qty * isi
        DgvData.Rows(indeksBaris).Cells("DiskonPersen").Value = 0
        DgvData.Rows(indeksBaris).Cells("DiskonRp").Value = 0
        DgvData.Rows(indeksBaris).Cells("TotalDiskon").Value = 0
        DgvData.Rows(indeksBaris).Cells("TotalHarga").Value = hargajual * qty
        DgvData.Rows(indeksBaris).Cells("StokToko").Value = Stoktoko
        DgvData.Rows(indeksBaris).Cells("StokGudang").Value = Stokgudang
        DgvData.Rows(indeksBaris).Cells("Stok").Value = Stok



        ' Melakukan pembaruan pada ringkasan atau operasi relevan lainnya
        UpdateSemuaTotal()

        ' Membersihkan field input
        KosongTxtboxcari()

        ' Jika AwalPenjualan adalah "Pencarian", pilih TxtNama
        If AwalPenjualan = "Pencarian" Then
            TxtNama.Select()
            TxtNama.Focus()
        End If
    End Sub

    Private Sub HitungNilaiSetiapBaris(ByVal indeksBaris As Integer)
        Dim row = DgvData.Rows(indeksBaris)

        Dim hargaBeli As Decimal = ParseDecimal(row.Cells("HargaBeli").Value)
        Dim qtyBarang As Decimal = ParseDecimal(row.Cells("QTY").Value)
        Dim isiBarang As Integer = CInt(ParseDecimal(row.Cells("Isi").Value))
        Dim hargaJual As Decimal = ParseDecimal(row.Cells("Harga").Value)
        Dim diskonRp As Decimal = ParseDecimal(row.Cells("DiskonRp").Value)

        Dim totalDiskon As Decimal = qtyBarang * diskonRp
        Dim totalHargaBeli As Decimal = qtyBarang * hargaBeli * isiBarang
        Dim qtySat As Decimal = qtyBarang * isiBarang
        Dim totalHarga As Decimal = hargaJual * qtyBarang - totalDiskon

        row.Cells("TotalDiskon").Value = totalDiskon
        row.Cells("TotalHargaBeli").Value = totalHargaBeli
        row.Cells("QtySat").Value = qtySat
        row.Cells("TotalHarga").Value = totalHarga
    End Sub

    Private Function ParseDecimal(ByVal value As Object) As Decimal
        If value Is Nothing Then Return 0D

        Dim s As String = value.ToString().Trim().Replace(",", ".") ' selalu ubah ke titik
        Dim result As Decimal = 0
        Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result)
        Return result
    End Function


    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        If e.ColumnIndex = 1 Then 'nama
            '=======================================
            If Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value) Then
                Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                    "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                    "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
                    "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
                    "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                    "FROM tbl_barang " &
                    "WHERE TRIM(NAMA_BARANG) LIKE @NamaBarang OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@NamaBarang", DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.HasRows Then
                            rd.Read() ' Lanjutkan ke data pertama
                            Dim namaBarangValue As String = DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value.ToString()
                            Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(DgvData.CurrentCell.RowIndex).Cells(Satuan.Index), DataGridViewComboBoxCell)
                            comboCell.Items.Clear()
                            If LblJenisPl.Text = "Partai" Then
                                If namaBarangValue = rd("NAMA_BARANG") OrElse
                                   namaBarangValue = rd("BARCODE_KECIL") OrElse
                                   namaBarangValue = rd("BARCODE_SEDANG") OrElse
                                   namaBarangValue = rd("BARCODE_BESAR") Then

                                    DgvData.Rows(e.RowIndex).Cells("Kode").Value = rd("ID_BARANG")
                                    DgvData.Rows(e.RowIndex).Cells("HARGABELI").Value = rd("HARGA_BELI")

                                    Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_PARTAI_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_PARTAI_KECIL")), "")
                                    Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_PARTAI_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_PARTAI_SEDANG")), "")
                                    Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_PARTAI_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_PARTAI_BESAR")), "")


                                    If Not String.IsNullOrEmpty(satuanKecil) Then
                                        comboCell.Items.Add(satuanKecil)
                                    End If

                                    If Not String.IsNullOrEmpty(satuanSedang) Then
                                        comboCell.Items.Add(satuanSedang)
                                    End If

                                    If Not String.IsNullOrEmpty(satuanBesar) Then
                                        comboCell.Items.Add(satuanBesar)
                                    End If
                                    If namaBarangValue = rd("BARCODE_SEDANG") Then
                                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanSedang
                                        DgvData.Rows(e.RowIndex).Cells("isi").Value = rd("ISI_PARTAI_SEDANG")
                                        DgvData.Rows(e.RowIndex).Cells("harga").Value = rd("HARGA_JUAL_PARTAI_SEDANG")
                                    ElseIf namaBarangValue = rd("BARCODE_BESAR") Then
                                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanBesar
                                        DgvData.Rows(e.RowIndex).Cells("isi").Value = rd("ISI_PARTAI_BESAR")
                                        DgvData.Rows(e.RowIndex).Cells("harga").Value = rd("HARGA_JUAL_PARTAI_BESAR")
                                    Else
                                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanKecil
                                        DgvData.Rows(e.RowIndex).Cells("isi").Value = rd("ISI_PARTAI_KECIL")
                                        DgvData.Rows(e.RowIndex).Cells("harga").Value = rd("HARGA_JUAL_PARTAI_KECIL")
                                    End If

                                    DgvData.Rows(e.RowIndex).Cells("qty").Value = 1
                                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value = rd("NAMA_BARANG")
                                End If

                            Else
                                If namaBarangValue = rd("NAMA_BARANG") OrElse
                                    namaBarangValue = rd("BARCODE_KECIL") OrElse
                                    namaBarangValue = rd("BARCODE_SEDANG") OrElse
                                    namaBarangValue = rd("BARCODE_BESAR") Then

                                    DgvData.Rows(e.RowIndex).Cells("Kode").Value = rd("ID_BARANG")
                                    DgvData.Rows(e.RowIndex).Cells("HARGABELI").Value = rd("HARGA_BELI")

                                    Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                                    Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                                    Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_BESAR")), "")


                                    If Not String.IsNullOrEmpty(satuanKecil) Then
                                        comboCell.Items.Add(satuanKecil)
                                    End If

                                    If Not String.IsNullOrEmpty(satuanSedang) Then
                                        comboCell.Items.Add(satuanSedang)
                                    End If

                                    If Not String.IsNullOrEmpty(satuanBesar) Then
                                        comboCell.Items.Add(satuanBesar)
                                    End If

                                    If namaBarangValue = rd("BARCODE_SEDANG").ToString() Then
                                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanSedang
                                        DgvData.Rows(e.RowIndex).Cells("isi").Value = rd("ISI_UMUM_SEDANG")
                                        DgvData.Rows(e.RowIndex).Cells("harga").Value = rd("HARGA_JUAL_UMUM_SEDANG")
                                    ElseIf namaBarangValue = rd("BARCODE_BESAR").ToString() Then
                                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanBesar
                                        DgvData.Rows(e.RowIndex).Cells("isi").Value = rd("ISI_UMUM_BESAR")
                                        DgvData.Rows(e.RowIndex).Cells("harga").Value = rd("HARGA_JUAL_UMUM_BESAR")
                                    Else
                                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanKecil
                                        DgvData.Rows(e.RowIndex).Cells("isi").Value = rd("ISI_UMUM_KECIL")
                                        DgvData.Rows(e.RowIndex).Cells("harga").Value = rd("HARGA_JUAL_UMUM_KECIL")
                                    End If

                                    DgvData.Rows(e.RowIndex).Cells("qty").Value = 1
                                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value = rd("NAMA_BARANG")
                                End If
                            End If

                            DgvData.Rows(e.RowIndex).Cells("StokToko").Value = rd("STOK_TOKO")
                            DgvData.Rows(e.RowIndex).Cells("StokGudang").Value = rd("STOK_GUDANG")
                            If LblLokasiBarang.Text = "GUDANG" Then
                                DgvData.Rows(e.RowIndex).Cells("Stok").Value = DgvData.Rows(e.RowIndex).Cells("StokGudang").Value
                            ElseIf LblLokasiBarang.Text = "TOKO" Then
                                DgvData.Rows(e.RowIndex).Cells("Stok").Value = DgvData.Rows(e.RowIndex).Cells("StokToko").Value
                            End If

                            If DgvData.Rows(e.RowIndex).Cells("isi").Value = 0 Then
                                DgvData.Rows(e.RowIndex).Cells("qtysat").Value = 1
                            End If


                            If Kodebarangsama = "Tidak" Then
                                ' Hapus data ganda
                                For barisatas As Integer = 0 To DgvData.RowCount - 1
                                    For barisbawah As Integer = barisatas + 1 To DgvData.RowCount - 1
                                        Dim kodeBarisAtas As Object = DgvData.Rows(barisatas).Cells("Kode").Value
                                        Dim kodeBarisBawah As Object = DgvData.Rows(barisbawah).Cells("Kode").Value

                                        If kodeBarisAtas IsNot Nothing AndAlso kodeBarisBawah IsNot Nothing AndAlso kodeBarisBawah.Equals(kodeBarisAtas) Then
                                            ' Mengupdate nilai Qty
                                            DgvData.Rows(barisatas).Cells("Qty").Value = If(IsDBNull(DgvData.Rows(barisatas).Cells("Qty").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("Qty").Value)) + 1

                                            ' Mengambil nilai isi sebagai Integer dan qty sebagai Decimal
                                            Dim isiValue As Integer = If(IsDBNull(DgvData.Rows(barisatas).Cells("isi").Value), 0, Convert.ToInt32(DgvData.Rows(barisatas).Cells("isi").Value))
                                            Dim qtyValue As Decimal = If(IsDBNull(DgvData.Rows(barisatas).Cells("qty").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("qty").Value))

                                            ' Mengupdate qtysat
                                            If isiValue = 0 Then
                                                DgvData.Rows(barisatas).Cells("qtysat").Value = If(IsDBNull(DgvData.Rows(barisatas).Cells("qtysat").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("qtysat").Value)) + 1
                                            Else
                                                DgvData.Rows(barisatas).Cells("qtysat").Value = isiValue * qtyValue
                                            End If

                                            ' Mengupdate TotalHarga
                                            Dim hargaValue As Decimal = If(IsDBNull(DgvData.Rows(barisatas).Cells("Harga").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("Harga").Value))
                                            Dim totalDiskonValue As Decimal = If(IsDBNull(DgvData.Rows(barisatas).Cells("TotalDiskon").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("TotalDiskon").Value))
                                            DgvData.Rows(barisatas).Cells("TotalHarga").Value = (hargaValue * qtyValue) - totalDiskonValue

                                            ' Menghapus baris jika bukan baris baru
                                            If Not DgvData.Rows(barisbawah).IsNewRow Then
                                                DgvData.Rows.RemoveAt(barisbawah)
                                            End If

                                            ' Menggeser fokus ke bawah
                                            SendKeys.Send("{down}")

                                            Exit Sub
                                        End If
                                    Next
                                Next
                            End If

                            HitungNilaiSetiapBaris(e.RowIndex)
                        Else
                            DgvData.Rows(e.RowIndex).Cells("namabarang").Value = ""
                            SendKeys.Send("{down}")
                            CariBarang.TxtJenisTransaksi.Text = "Penjualan"
                            rd.Close()
                            CariBarang.ShowDialog()
                        End If

                    End Using
                End Using
            Else
                DgvData.Rows(e.RowIndex).Cells("namabarang").Value = ""
                SendKeys.Send("{down}")
                CariBarang.TxtJenisTransaksi.Text = "Penjualan"
                CariBarang.ShowDialog()
            End If
        End If

        '========================== qty
        If e.ColumnIndex = 3 Then
            Dim qtyCellValue As String = DgvData.Rows(e.RowIndex).Cells("qty").Value.Trim()

            ' Validasi hanya angka dan satu koma atau titik
            Dim isValid As Boolean = System.Text.RegularExpressions.Regex.IsMatch(qtyCellValue, "^\d+([,.]\d+)?$")

            If Not isValid Then
                MessageBox.Show("Qty hanya boleh angka dan satu tanda koma atau titik.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                DgvData.Rows(e.RowIndex).Cells("qty").Value = "1"
            Else
                ' Ubah koma ke titik untuk standar database
                qtyCellValue = qtyCellValue.Replace(",", ".")
                DgvData.Rows(e.RowIndex).Cells("qty").Value = qtyCellValue
            End If

            HitungNilaiSetiapBaris(e.RowIndex)
        End If


        '========================== harga jual
        If e.ColumnIndex = 7 Then
            Dim hargaCellValue As String = DgvData.Rows(e.RowIndex).Cells("harga").Value

            Dim harga As Decimal
            Dim diskonRp As Decimal = DgvData.Rows(e.RowIndex).Cells("DiskonRp").Value

            If Not Decimal.TryParse(hargaCellValue, harga) Then
                MessageBox.Show("Harga harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                harga = 0
            End If

            ' Hitung diskon persen
            Dim diskonPersen As Decimal = If(harga > 0, (diskonRp / harga) * 100, 0)
            DgvData.Rows(e.RowIndex).Cells("diskonpersen").Value = diskonPersen

            HitungNilaiSetiapBaris(e.RowIndex)


            ' Jika harga jual diubah, maka akan muncul edit master barang
            If Editmasterhargajual = "Iya" Then
                Dim idBarangObj = DgvData.Rows(e.RowIndex).Cells("Kode").Value
                If idBarangObj IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(idBarangObj.ToString()) Then
                    Dim idBarang As String = idBarangObj.ToString()
                    Dim hargaJual As Decimal = ParseDecimal(DgvData.Rows(e.RowIndex).Cells("harga").Value)
                    Dim satuan As String = DgvData.Rows(e.RowIndex).Cells("Satuan").Value.ToString()
                    Dim jenispelanggan As String = If(LblJenisPl.Text = "Partai", "Partai", "Umum")
                    ' Panggil fungsi untuk mengupdate harga jual di database
                    UpdateHargaJual(idBarang, hargaJual, satuan, jenispelanggan)
                End If
            End If

        End If


        '========================== diskonpersen
        If e.ColumnIndex = 9 Then
            Dim diskonPersenCellValue As String = DgvData.Rows(e.RowIndex).Cells("Diskonpersen").Value

            Dim harga As Decimal = If(IsDBNull(DgvData.Rows(e.RowIndex).Cells("harga").Value), 0D, ParseDecimal(DgvData.Rows(e.RowIndex).Cells("harga").Value))
            Dim diskonPersen As Decimal

            If Not Decimal.TryParse(diskonPersenCellValue, diskonPersen) Then
                MessageBox.Show("Diskon persen harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                diskonPersen = 0
            End If

            ' Hitung Diskon Rp
            Dim diskonRp As Decimal = harga * diskonPersen / 100
            DgvData.Rows(e.RowIndex).Cells("DiskonRp").Value = diskonRp

            HitungNilaiSetiapBaris(e.RowIndex)
        End If

        '========================== diskonrp
        If e.ColumnIndex = 10 Then
            Dim diskonRpCellValue As String = DgvData.Rows(e.RowIndex).Cells("DiskonRp").Value

            Dim harga As Decimal = If(IsDBNull(DgvData.Rows(e.RowIndex).Cells("harga").Value), 0D, ParseDecimal(DgvData.Rows(e.RowIndex).Cells("harga").Value))
            Dim diskonRp As Decimal

            If Not Decimal.TryParse(diskonRpCellValue, diskonRp) Then
                MessageBox.Show("Diskon Rp harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                diskonRp = 0
            End If


            ' Hitung diskon persen
            Dim diskonPersen As Decimal = If(harga > 0, (diskonRp / harga) * 100, 0)
            DgvData.Rows(e.RowIndex).Cells("diskonpersen").Value = diskonPersen

            HitungNilaiSetiapBaris(e.RowIndex)
        End If


        If Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("Kode").Value) Then
            UpdateSemuaTotal()
        End If


    End Sub

    Private Sub UpdateHargaJual(ByVal idBarang As String, ByVal hargaJual As Decimal, ByVal satuan As String, ByVal jenispelanggan As String)
        With TambahBarang
            .LblUtama.Text = "EDIT HARGA JUAL DARI PENJUALAN"
            .GBBarcode.Visible = False
            .GBStok.Visible = False
            .GBBarang.Enabled = False
            .GBPoint.Visible = False
            .PanelInfoRubahHarga.Visible = False
            .BtnTambahKategori.Visible = False
            .BtnSupliyer.Visible = False
            .BtnTambahSatuan.Visible = False
            .CBManual.Visible = False
            .BtnBaru.Visible = False
            '.BackColor = Color.DarkCyan
            .Size = New Size(825, 630)
            TambahBarang.Tampilkategori()
            TambahBarang.TampilSatuan()
            TambahBarang.Tampilsupliyer()
            .TxtKode.Text = idBarang
            .LblHargaDrJual.Text = hargaJual
            .LblsatuanDrJual.Text = satuan
            .LblJenisDrJual.Text = jenispelanggan
            .ShowDialog()
        End With
    End Sub


    Public Sub AddItems(ByVal col As AutoCompleteStringCollection, ByVal searchTerm As String)
        Using cmd As New MySqlCommand("SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @searchTerm", conn)
            ' Menggabungkan '%' dengan parameter di dalam kode, bukan di query
            cmd.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    col.Add(rd("NAMA_BARANG").ToString())
                Loop
            End Using
        End Using
    End Sub


    Private Sub DgvData_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DgvData_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles DgvData.EditingControlShowing
        Dim titleText As String = DgvData.Columns(1).HeaderText
        If titleText.Equals("Nama Barang") Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.Suggest
                autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                Dim DataCollection As New AutoCompleteStringCollection()

                ' Mendapatkan teks dari TextBox autoText
                Dim searchTerm As String = autoText.Text

                ' Memanggil AddItems dengan nilai pencarian dari TextBox
                AddItems(DataCollection, searchTerm)

                autoText.AutoCompleteCustomSource = DataCollection
            End If
        End If

        If DgvData.CurrentCell.ColumnIndex = 4 Then
            If TypeOf e.Control Is ComboBox Then
                Dim comboBox As ComboBox = DirectCast(e.Control, ComboBox)
                RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
            End If
        End If
    End Sub

    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)
        Dim cell As DataGridViewComboBoxCell = DirectCast(DgvData.CurrentCell, DataGridViewComboBoxCell)
        If LblJenisPl.Text = "Partai" Then
            Using cmd As New MySqlCommand("select ID_BARANG,ISI_PARTAI_KECIL,HARGA_JUAL_PARTAI_KECIL,ISI_PARTAI_SEDANG,HARGA_JUAL_PARTAI_SEDANG,ISI_PARTAI_BESAR,HARGA_JUAL_PARTAI_BESAR from tbl_barang WHERE ID_BARANG = ?", conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", cell.OwningRow.Cells("Kode").Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        If comboBox.SelectedIndex = 0 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_PARTAI_KECIL")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_KECIL")
                        ElseIf comboBox.SelectedIndex = 1 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_PARTAI_SEDANG")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_SEDANG")
                        Else
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_PARTAI_BESAR")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_BESAR")
                        End If
                    Else
                        MessageBox.Show("Satuan barang dan atau harga jual belum di input ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using
            End Using
        Else
            Using cmd As New MySqlCommand("select ID_BARANG,ISI_UMUM_KECIL,HARGA_JUAL_UMUM_KECIL, ISI_UMUM_SEDANG,HARGA_JUAL_UMUM_SEDANG, ISI_UMUM_BESAR,HARGA_JUAL_UMUM_BESAR from tbl_barang WHERE ID_BARANG = ?", conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", cell.OwningRow.Cells("Kode").Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        ' Update the contents of the adjacent text box column based on the selected option in the combo box
                        If comboBox.SelectedIndex = 0 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_UMUM_KECIL")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_KECIL")
                        ElseIf comboBox.SelectedIndex = 1 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_UMUM_SEDANG")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_SEDANG")
                        Else
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_UMUM_BESAR")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_BESAR")
                        End If
                    Else
                        MessageBox.Show("Satuan barang dan atau harga jual belum di input ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using
            End Using
        End If
        Dim rowI As Integer = DgvData.CurrentCell.RowIndex
        HitungNilaiSetiapBaris(rowI)
        UpdateSemuaTotal()
    End Sub


    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Delete Then
            If DgvData.SelectedCells.Count > 0 Then
                Dim selectedCell As DataGridViewCell = DgvData.SelectedCells(0)

                ' Periksa apakah sel yang dipilih berada di kolom "Nama"
                If selectedCell.ColumnIndex = DgvData.Columns("NamaBarang").Index Then
                    Dim rowIndex As Integer = selectedCell.RowIndex

                    ' Periksa apakah nilai di kolom "Nama" tidak kosong
                    If Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("NamaBarang").Value.ToString()) Then
                        ' Hapus baris jika nilai di kolom "Nama" tidak kosong
                        Hapusbaris()
                        ' Setelah menghapus baris, pastikan untuk menghilangkan seleksi agar tidak ada baris yang dipilih secara default.
                        DgvData.ClearSelection()
                    Else
                        MessageBox.Show("Klik kanan pada baris yang tidak kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
            UpdateSemuaTotal()
        End If

    End Sub

    'Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvData.CellMouseUp
    '    If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 Then
    '        ' Simpan sel yang aktif sebelumnya
    '        Dim previousCell As DataGridViewCell = DgvData.CurrentCell

    '        ' Pilih seluruh baris yang diklik kanan
    '        DgvData.Rows(e.RowIndex).Selected = True
    '        DgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect

    '        ' Setel sel saat ini ke sel di kolom pertama pada baris yang diklik
    '        DgvData.CurrentCell = DgvData.Rows(e.RowIndex).Cells(1)

    '        ' Tampilkan ContextMenuStrip di lokasi kursor
    '        Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
    '        ContextMenuStrip1.Show(cursorPosition)

    '        ' Kembalikan seleksi ke kolom semula setelah menu konteks ditutup
    '        AddHandler ContextMenuStrip1.Closed, Sub()
    '                                                 DgvData.SelectionMode = DataGridViewSelectionMode.CellSelect
    '                                                 If previousCell IsNot Nothing Then
    '                                                     DgvData.CurrentCell = previousCell
    '                                                 End If
    '                                             End Sub
    '    End If
    'End Sub

    Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvData.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Periksa apakah sel yang diklik kanan ada
            Dim cell As DataGridViewCell = DgvData.Rows(e.RowIndex).Cells("NamaBarang")
            If cell IsNot Nothing AndAlso cell.Value IsNot Nothing Then
                ' Periksa apakah nilai di kolom "Nama" pada baris yang diklik tidak kosong
                Dim namaValue As String = cell.Value.ToString()
                If Not String.IsNullOrEmpty(namaValue) Then
                    ' Setel sel saat ini ke sel "Nama"
                    DgvData.CurrentCell = cell

                    ' Tampilkan ContextMenuStrip di lokasi kursor
                    Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                    ContextMenuStrip1.Show(cursorPosition)
                End If
            End If
        End If
    End Sub

    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
    End Sub

    'Public Sub HapusBaris()
    '    Dim baris As Integer = DgvData.CurrentCell.RowIndex

    '    ' Periksa apakah baris baru yang belum di-commit
    '    If DgvData.Rows(baris).IsNewRow Then
    '        ' Commit perubahan pada baris baru
    '        DgvData.EndEdit()
    '    End If

    '    ' Periksa apakah sel dalam mode edit
    '    If DgvData.IsCurrentCellInEditMode Then
    '        MsgBox("Tidak dapat menghapus baris dalam mode edit.", vbExclamation + vbCritical, "Mode Edit Aktif")
    '    Else
    '        ' Jika tidak dalam mode edit, hapus baris
    '        DgvData.Rows.RemoveAt(baris)
    '        UpdateSemuaTotal()
    '    End If
    'End Sub


    Private Sub Hapusbaris()
        ' Periksa apakah ada sel yang dipilih
        If DgvData.CurrentCell Is Nothing Then
            MsgBox("Tidak ada baris yang dipilih.", vbExclamation, "Peringatan")
            Return
        End If

        Dim baris As Integer = DgvData.CurrentCell.RowIndex

        ' Periksa apakah sel dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            MsgBox("Tidak dapat menghapus baris dalam mode edit.", vbExclamation + vbCritical, "Mode Edit Aktif")
            Return
        End If


        ' Konfirmasi penghapusan untuk baris yang berisi data
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus baris ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            DgvData.Rows.RemoveAt(baris)
            UpdateSemuaTotal()
        End If
    End Sub

    Private Sub DgvData_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvData.CellFormatting
        ' Pastikan kolom "StokToko" dan "StokGudang" ada
        If DgvData.Columns("StokToko") IsNot Nothing AndAlso DgvData.Columns("StokGudang") IsNot Nothing Then
            Dim stokTokoIndex As Integer = DgvData.Columns("StokToko").Index
            Dim stokGudangIndex As Integer = DgvData.Columns("StokGudang").Index

            ' Cek apakah sel yang sedang diformat adalah bagian dari "StokToko" atau "StokGudang"
            If e.ColumnIndex = stokTokoIndex OrElse e.ColumnIndex = stokGudangIndex Then
                Dim stokValue As Object = e.Value
                If stokValue IsNot Nothing AndAlso IsNumeric(stokValue) AndAlso ParseDecimal(stokValue) < 1 Then
                    ' Jika nilai stok < 1, ubah warna latar belakang menjadi merah
                    e.CellStyle.BackColor = Color.Red
                    e.CellStyle.ForeColor = Color.White ' Opsional, agar teks lebih terlihat
                End If
            End If
        End If
    End Sub

    Private Sub HitungUlangBarisIniToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HitungUlangBarisIniToolStripMenuItem.Click
        Hitungbaris()
        UpdateSemuaTotal()
    End Sub

    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs) Handles DgvData.DataError
        Dim errorMessage As String = "Kesalahan data: " & e.Exception.Message & Environment.NewLine &
                                     "Periksa baris yang disorot dan perbaiki."

        MessageBox.Show(errorMessage, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)

        ' Menyorot baris yang bermasalah
        If e.RowIndex >= 0 Then
            For Each cell As DataGridViewCell In DgvData.Rows(e.RowIndex).Cells
                cell.Style.BackColor = Color.Red
            Next
        End If

        e.Cancel = True
    End Sub



    Public Sub UpdateSemuaTotal()
        ' === Hitung Total Harga Beli (HPP) ===
        Dim totalHpp As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Totalhargabeli").Value IsNot Nothing Then
                totalHpp += Math.Round(ParseDecimal(row.Cells("Totalhargabeli").Value))
            End If
        Next
        TxtHpp.Text = totalHpp.ToString()

        ' === Hitung Grand Total Harga Jual ===
        Dim totalGrand As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Totalharga").Value IsNot Nothing Then
                totalGrand += Math.Round(ParseDecimal(row.Cells("Totalharga").Value))
            End If
        Next
        TxtTotalSblDiskon.Text = totalGrand.ToString()

        ' === Hitung Jumlah Barang dan Jumlah Item ===
        Dim totalQtyBarang As Decimal = 0
        Dim totalItemCount As Integer = 0
        For Each row As DataGridViewRow In DgvData.Rows
            Dim qtyObj As Object = row.Cells("Qty").Value
            If Not row.IsNewRow AndAlso qtyObj IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyObj.ToString()) Then
                totalQtyBarang += Math.Round(ParseDecimal(qtyObj))
                totalItemCount += 1
            End If
        Next
        TxtJmlhBrg.Text = totalQtyBarang.ToString()
        TxtJmlhItem.Text = totalItemCount.ToString()
        LblRecord.Text = "Total record : " & totalItemCount.ToString()

        ' === Hitung Total QTY Satuan ===
        Dim totalQtySat As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("QtySat").Value IsNot Nothing Then
                totalQtySat += Math.Round(ParseDecimal(row.Cells("QtySat").Value), 2)
            End If
        Next
        TxtQtySat.Text = totalQtySat.ToString()

        ' === Scroll otomatis ke baris terakhir ===
        If DgvData.Rows.Count > 0 Then
            DgvData.FirstDisplayedScrollingRowIndex = DgvData.Rows.Count - 1
        End If

    End Sub


    Private Sub TxtDiskonRp_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtDiskonRp.TextChanged
        HitungDiskonRp()
    End Sub


    Private Sub HitungDiskonRp()
        Dim grandTotal As Decimal
        Dim diskonRp As Decimal

        If Decimal.TryParse(TxtTotalSblDiskon.Text, grandTotal) AndAlso
           Decimal.TryParse(TxtDiskonRp.Text, diskonRp) Then

            ' Hitung total belanja setelah diskon
            Dim totalBelanja As Decimal = grandTotal - diskonRp
            ' Format output sesuai dengan format Indonesia
            TxtTotalBelanja.Text = totalBelanja.ToString()

            ' Format diskonRp dengan pemisah ribuan dan tanpa desimal
            LblDiskonRp.Text = "Rp. " & diskonRp.ToString("#,0.##", cultureIndonesia)

            If grandTotal > 0 Then
                ' Hitung diskon persen
                Dim diskonPersen As Decimal = Math.Round((diskonRp / grandTotal * 100), 0)
                TxtDiskonPersen.Text = diskonPersen.ToString("#,0.##", cultureIndonesia)
            Else
                ' Jika grandTotal nol atau negatif, atur diskon persen ke nol
                TxtDiskonPersen.Text = "0"
            End If

        Else
            ' Jika parsing gagal, set diskon persen ke nol
            TxtDiskonPersen.Text = "0"
        End If

        ' Panggil metode untuk menghitung total setelah pajak
        HitungTotalSetelahPajak()
    End Sub



    Private Sub TxtDiskonRp_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtDiskonRp.KeyDown
        Dim allowedKeys As New List(Of Keys) From {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}

        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
       (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
       Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtDiskonPersen_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtDiskonPersen.KeyUp
        Dim grandTotal As Decimal
        Dim diskonPersen As Decimal

        If Decimal.TryParse(TxtTotalSblDiskon.Text, grandTotal) AndAlso
       Decimal.TryParse(TxtDiskonPersen.Text, diskonPersen) Then
            TxtDiskonRp.Text = Math.Round(grandTotal * (diskonPersen / 100), 0).ToString()
        Else
            TxtDiskonRp.Text = "0"
        End If
    End Sub

    Private Sub TxtDiskonPersen_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtDiskonPersen.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Menghentikan karakter dari ditampilkan
        End If
    End Sub

    Private Sub TxtTotalBelanja_TextChanged(sender As Object, e As EventArgs) Handles TxtTotalBelanja.TextChanged
        Dim totalBelanja As Decimal = 0
        Decimal.TryParse(TxtTotalBelanja.Text, totalBelanja)

        Dim diskonPersen As Decimal = 0
        Decimal.TryParse(TxtDiskonPersen.Text, diskonPersen)

        Dim pajakPersen As Decimal = 0
        Decimal.TryParse(TxtPajakPersen.Text, pajakPersen)

        If diskonPersen <> 0 Then
            TxtDiskonRp.Text = Math.Round(totalBelanja * (diskonPersen / 100), 0).ToString()
        Else
            TxtDiskonRp.Text = "0"
        End If

        If pajakPersen <> 0 Then
            TxtPajakRp.Text = Math.Round(totalBelanja * (pajakPersen / 100), 0).ToString()
        Else
            TxtPajakRp.Text = "0"
        End If
    End Sub


    Private Sub TxtGrandtotalSblDiskon_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotalSblDiskon.TextChanged
        HitungTotalSetelahPajak()
    End Sub


    Private Sub HandlePajakTextChanged(ByVal pajakText As TextBox, ByVal labelPajak As Label, ByVal totalBelanjaText As TextBox, ByVal totalStlPajakText As TextBox)
        ' Deklarasi variabel untuk menyimpan nilai teks
        Dim pajakTextValue As String = If(String.IsNullOrEmpty(pajakText.Text), "0", pajakText.Text)
        Dim totalBelanjaTextValue As String = If(String.IsNullOrEmpty(totalBelanjaText.Text), "0", totalBelanjaText.Text)
        Dim totalStlPajakTextValue As String = totalStlPajakText.Text
        Dim txtTotalBelanjaValue As String = If(String.IsNullOrEmpty(TxtTotalBelanja.Text), "0", TxtTotalBelanja.Text)

        If Not String.IsNullOrEmpty(pajakTextValue) Then
            Dim pajakRp As Decimal
            If Decimal.TryParse(pajakTextValue, pajakRp) Then
                labelPajak.Text = "Rp. " & pajakRp.ToString("#,0.##", cultureIndonesia)
                totalStlPajakText.Text = (ParseDecimal(totalBelanjaTextValue) + pajakRp).ToString()
                If ParseDecimal(txtTotalBelanjaValue) > 0 Then
                    TxtPajakPersen.Text = Math.Round((pajakRp / ParseDecimal(txtTotalBelanjaValue) * 100), 0).ToString()
                Else
                    TxtPajakPersen.Text = "0"
                End If
            Else
                pajakText.Text = "0"
            End If
        Else
            pajakText.Text = "0"
        End If
        HitungTotalSetelahPajak()
    End Sub


    Private Sub TxtPajakRp_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtPajakRp.TextChanged
        HandlePajakTextChanged(TxtPajakRp, LblPajakRp, TxtTotalBelanja, TxtTotalStlPajak)
    End Sub

    Private Sub TxtPajakPersen_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TxtPajakPersen.Validating
        Dim pajakPersen As Decimal
        If Decimal.TryParse(TxtPajakPersen.Text, pajakPersen) Then
            TxtPajakRp.Text = Math.Round(ParseDecimal(TxtTotalBelanja.Text) * (pajakPersen / 100), 0).ToString()
        Else
            TxtPajakRp.Text = "0"
        End If
    End Sub

    Private Sub TxtPajakRp_KeyPress(ByVal sender As System.Object, ByVal e As KeyPressEventArgs) Handles TxtPajakRp.KeyPress
        If Not (Char.IsDigit(e.KeyChar) Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub TxtPajakPersen_KeyPress(ByVal sender As System.Object, ByVal e As KeyPressEventArgs) Handles TxtPajakPersen.KeyPress
        If Not (Char.IsDigit(e.KeyChar) Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub TxtBiayaKirim_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtBiayaKirim.TextChanged

        Dim biayaKirim As Decimal = 0

        ' Parsing Biaya Kirim
        If String.IsNullOrEmpty(TxtBiayaKirim.Text) Or Not Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) Then
            LblBiayaKirim.Text = "0"
        Else
            LblBiayaKirim.Text = biayaKirim.ToString("#,0.##", cultureIndonesia)
        End If

        HitungTotalSetelahPajak()

    End Sub


    Private Sub HitungTotalSetelahPajak()
        ' Ambil nilai dari TextBox, parsing ke format desimal internasional
        Dim totalSebelumDiskon As Decimal = ParseDecimal(TxtTotalSblDiskon.Text)
        Dim diskonRp As Decimal = ParseDecimal(TxtDiskonRp.Text)
        Dim pajakRp As Decimal = ParseDecimal(TxtPajakRp.Text)
        Dim biayaKirim As Decimal = ParseDecimal(TxtBiayaKirim.Text)

        ' Hitung Total Belanja (setelah diskon)
        Dim totalBelanja As Decimal = totalSebelumDiskon - diskonRp
        TxtTotalBelanja.Text = totalBelanja.ToString()

        ' Hitung Total Setelah Pajak (total belanja + pajak + biaya kirim)
        Dim totalSetelahPajak As Decimal = totalBelanja + pajakRp + biayaKirim
        TxtTotalStlPajak.Text = totalSetelahPajak.ToString()
    End Sub




    Private Sub TxtTotalStlPajak_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles TxtTotalStlPajak.TextChanged
        Dim totalStlPajak As Decimal

        If String.IsNullOrEmpty(TxtTotalStlPajak.Text) Or Not Decimal.TryParse(TxtTotalStlPajak.Text, totalStlPajak) Then
            LblTotalStlPajak.Text = "0"
            TxtGrantotal.Text = "Rp. 0"
        Else
            LblTotalStlPajak.Text = totalStlPajak.ToString("#,0.##", cultureIndonesia)
            TxtGrantotal.Text = "Rp. " & totalStlPajak.ToString("#,0.##", cultureIndonesia)
        End If
    End Sub

    Private Sub TxtKembali_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles TxtKembali.TextChanged
        LblKembali.Text = If(String.IsNullOrEmpty(TxtKembali.Text) Or Not IsNumeric(TxtKembali.Text), "0",
                     ParseDecimal(TxtKembali.Text).ToString("#,0.##", cultureIndonesia))
    End Sub

    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If

        Call Tekanbayar()
    End Sub

    Private Sub BtnTahan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTahan.Click
        Call Tekantahan()
    End Sub

    Private Sub BtnPanggil_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPanggil.Click
        Call Tekanpanggil()
    End Sub

    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click
        TekanBarang()
    End Sub

    Private Sub BtnPelanggan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPelanggan.Click
        Tekanpelanggan()
    End Sub

    Private Sub BtnSimpann_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpann.Click
        Tekansimpan()
    End Sub
    Private Sub BtnBatal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBatal.Click
        TekanBatal()
    End Sub
    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Public Sub TekanBatal()
        GBBayar.Visible = False
        TxtBayar.Text = 0
    End Sub

    Private Sub CenterPanelBayar()
        Dim x As Integer = (ClientSize.Width - GBBayar.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - GBBayar.Height) \ 2
        GBBayar.Location = New Point(x, y)
    End Sub

    Private Sub Form_Penjualan_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                CmbSales.Select()
                CmbSales.DroppedDown = True

            Case Keys.F3
                CmbPelanggan.Select()
                CmbPelanggan.DroppedDown = True
            Case Keys.F4
                TekanBarang()

            Case Keys.F5
                CariBarang.TxtJenisTransaksi.Text = "Penjualan"
                CariBarang.ShowDialog()

            Case Keys.F12
                Tekanpelanggan()

            Case Keys.F6
                Tekantahan()

            Case Keys.F7
                Tekanpanggil()

            Case Keys.F8
                ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
                If DgvData.IsCurrentCellInEditMode Then
                    DgvData.EndEdit()
                End If
                Tekanbayar()

            Case Keys.F9
                CmbJenisBayar.Select()
                CmbJenisBayar.DroppedDown = True

            Case Keys.F10
                If GBBayar.Visible Then
                    Tekansimpan()
                End If

            Case Keys.F11
                If GBBayar.Visible Then
                    TekanBatal()
                End If

            Case Keys.Escape
                If GBBayar.Visible Then
                    TekanBatal()
                ElseIf TxtNama.Text <> "" Then
                    TxtNama.Clear()
                Else
                    ' Menambahkan pertanyaan apakah akan keluar
                    Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Close()
                    End If
                End If

            Case Keys.Back, Keys.Delete
                ' Jika LstBarang visible dan tombol Backspace atau Delete ditekan
                If LstBarang.Visible = True Then
                    TxtNama.Select()
                End If

        End Select
    End Sub

    Private Sub BtnCariBarang_Click(sender As Object, e As EventArgs) Handles BtnCariBarang.Click
        CariBarang.TxtJenisTransaksi.Text = "Penjualan"
        CariBarang.ShowDialog()
    End Sub

    Public Sub Tekanbayar()
        TxtBayar.Text = ""
        If String.IsNullOrEmpty(TxtTotalSblDiskon.Text) OrElse TxtTotalSblDiskon.Text = "0" Then
            MessageBox.Show("Belum ada transaksi penjualan ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim ModulJualRugi As String = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblJualRugi.Text)
        Dim ModulJualMinus As String = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblJualMinus.Text)

        If ModulJualRugi = "Tidak" Then
            If Cekjualrugi() Then
                Return
            End If
        End If

        If ModulJualMinus = "Tidak" Then
            If CekStok() Then
                Return
            End If
        End If


        CenterPanelBayar()
        GBBayar.Visible = True
        TxtBayar.Focus()


    End Sub

    Public Sub Tekantahan()
        ' Pastikan setidaknya satu baris di kolom "kode" terisi
        Dim isDataValid As Boolean = False

        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso Not String.IsNullOrEmpty(Convert.ToString(dgvRow.Cells(0).Value)) Then
                isDataValid = True
                Exit For
            End If
        Next

        If Not isDataValid Then
            MessageBox.Show("Belum ada barang yang di input", "Kosong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = Nothing

        Try

            transaction = conn.BeginTransaction()
            If TxtJenistransaksi.Text = "TambahPenjualan" Then
                Nomorjual()
            End If


            ' Menyimpan data penjualan_ditahan
            For Each dgvRow As DataGridViewRow In DgvData.Rows
                ' Cek apakah baris bukan baris baru (kosong)
                If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                    Dim query As String = "INSERT INTO penjualan_ditahan_detail (FAKTUR_JUAL, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, TOKO, GUDANG, STOK) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                    Using cmd As New MySqlCommand(query, conn, transaction)

                        cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                        cmd.Parameters.AddWithValue("@idBarang", dgvRow.Cells(0).Value)
                        cmd.Parameters.AddWithValue("@namaBarang", dgvRow.Cells(1).Value)
                        cmd.Parameters.AddWithValue("@hargaBeli", ParseDecimal(dgvRow.Cells(2).Value))
                        cmd.Parameters.AddWithValue("@qty", ParseDecimal(dgvRow.Cells(3).Value))
                        cmd.Parameters.AddWithValue("@satuan", dgvRow.Cells(4).Value)
                        cmd.Parameters.AddWithValue("@isiSatuan", ParseDecimal(dgvRow.Cells(5).Value))
                        cmd.Parameters.AddWithValue("@hargaBeliSatuan", ParseDecimal(dgvRow.Cells(6).Value))
                        cmd.Parameters.AddWithValue("@hargaJual", ParseDecimal(dgvRow.Cells(7).Value))
                        cmd.Parameters.AddWithValue("@qtySatuan", ParseDecimal(dgvRow.Cells(8).Value))
                        cmd.Parameters.AddWithValue("@diskonPersen", ParseDecimal(dgvRow.Cells(9).Value))
                        cmd.Parameters.AddWithValue("@diskonRp", ParseDecimal(dgvRow.Cells(10).Value))
                        cmd.Parameters.AddWithValue("@totalDiskon", ParseDecimal(dgvRow.Cells(11).Value))
                        cmd.Parameters.AddWithValue("@totalHarga", ParseDecimal(dgvRow.Cells(12).Value))
                        cmd.Parameters.AddWithValue("@toko", dgvRow.Cells(13).Value)
                        cmd.Parameters.AddWithValue("@gudang", dgvRow.Cells(14).Value)
                        cmd.Parameters.AddWithValue("@stok", ParseDecimal(dgvRow.Cells(15).Value))

                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Next




            ' Menyimpan data Temp_penjualan_ditahan
            Dim tempQuery As String = "INSERT INTO penjualan_ditahan (FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, TANGGAL_JUAL, GRAN_TOTAL, TOTAL_QTY, TOTAL_ITEM, ID_USER, ID_KOMPUTER) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
            Using cmd As New MySqlCommand(tempQuery, conn, transaction)

                cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@idPelanggan", LbLKodePel.Text)
                cmd.Parameters.AddWithValue("@namaPelanggan", CmbPelanggan.Text)
                cmd.Parameters.AddWithValue("@jenisPelanggan", LblJenisPl.Text)
                cmd.Parameters.AddWithValue("@tanggalJual", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))

                ' Tidak perlu pakai cultureIndonesia lagi
                cmd.Parameters.AddWithValue("@grantTotal", ParseDecimal(TxtTotalBelanja.Text))
                cmd.Parameters.AddWithValue("@totalQty", ParseDecimal(TxtQtySat.Text))
                cmd.Parameters.AddWithValue("@totalItem", ParseDecimal(TxtJmlhItem.Text))

                cmd.Parameters.AddWithValue("@idUser", FormUtama.SLogin.Text)
                cmd.Parameters.AddWithValue("@idKomputer", FormUtama.Comp.Text)

                cmd.ExecuteNonQuery()
            End Using



            ' Commit transaksi
            transaction.Commit()

            DatabaseModule.CatatanAksiHistory("Simpan penjualan " & TxtFaktur.Text)
            JumlahTahan()
            TxtFaktur.Text = ""
            Call Kondisiawal()

        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
            transaction?.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub Tekanpanggil()
        If TxtTahan.Text > 0 Then
            FormPenjualanDitahan.ShowDialog()
            JumlahTahan()
            'AmbilDataDitahan()
        Else
            MessageBox.Show("Tidak ada data ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Sub

    Public Sub JumlahTahan()
        Dim queryString As String = "SELECT COUNT(FAKTUR_JUAL) as jumlah FROM penjualan_ditahan"
        Dim jumlah As Integer

        Using command As New MySqlCommand(queryString, conn)
            jumlah = CInt(command.ExecuteScalar())
            TxtTahan.Text = jumlah
        End Using
    End Sub

    Public Sub AmbilDataDitahan()
        If TxtFaktur.Text = "" Then
            Exit Sub
        Else
            If TxtJenistransaksi.Text = "TambahPenjualan" Then
                DgvData.DataSource = Nothing
                DgvData.Rows.Clear()

                ' Dictionary untuk menyimpan daftar satuan berdasarkan ID_BARANG
                Dim satuanBarang As New Dictionary(Of String, List(Of String))

                ' Query satu kali untuk mengambil semua satuan
                Dim querySatuan As String = "SELECT ID_BARANG, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang"
                Using cmdSatuan As New MySqlCommand(querySatuan, conn)
                    Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                        While rdSatuan.Read()
                            Dim idBarang As String = rdSatuan("ID_BARANG").ToString()
                            Dim listSatuan As New List(Of String)

                            If LblJenisPl.Text = "Partai" Then
                                ' Tambahkan satuan partai ke daftar
                                For i As Integer = 1 To 3 ' Kolom SATUAN_PARTAI_KECIL, SEDANG, BESAR
                                    Dim satuan As String = rdSatuan(i).ToString()
                                    If Not String.IsNullOrEmpty(satuan) Then listSatuan.Add(satuan)
                                Next
                            Else
                                ' Tambahkan satuan umum ke daftar
                                For i As Integer = 4 To 6 ' Kolom SATUAN_UMUM_KECIL, SEDANG, BESAR
                                    Dim satuan As String = rdSatuan(i).ToString()
                                    If Not String.IsNullOrEmpty(satuan) Then listSatuan.Add(satuan)
                                Next
                            End If

                            ' Simpan ke dictionary
                            satuanBarang(idBarang) = listSatuan
                        End While
                    End Using
                End Using

                ' Query utama untuk data penjualan_ditahan_detail
                Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, TOKO, GUDANG, STOK, SISA FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = ?"

                ' Query utama untuk data penjualan_ditahan_detail
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)

                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            ' Tambahkan baris ke DataGridView
                            Dim row As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
                            row.Cells(0).Value = rd("ID_BARANG")
                            row.Cells(1).Value = rd("NAMA_BARANG")
                            row.Cells(2).Value = rd("HARGA_BELI")
                            row.Cells(3).Value = rd("QTY")
                            row.Cells(4).Value = rd("SATUAN")
                            row.Cells(5).Value = rd("ISI_SATUAN")
                            row.Cells(6).Value = rd("HARGA_BELI_SATUAN")
                            row.Cells(7).Value = rd("HARGA_JUAL")
                            row.Cells(8).Value = rd("QTY_SATUAN")
                            row.Cells(9).Value = rd("DISKON_PERSEN")
                            row.Cells(10).Value = rd("DISKON_RP")
                            row.Cells(11).Value = rd("TOTAL_DISKON")
                            row.Cells(12).Value = rd("TOTAL_HARGA")
                            row.Cells(13).Value = rd("TOKO")
                            row.Cells(14).Value = rd("GUDANG")
                            row.Cells(15).Value = rd("STOK")

                            ' Ambil ID_BARANG untuk mengisi ComboBoxCell
                            Dim idBarang As String = row.Cells(0).Value.ToString()
                            If satuanBarang.ContainsKey(idBarang) Then
                                Dim comboCell As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
                                comboCell.Items.Clear()
                                comboCell.Items.AddRange(satuanBarang(idBarang).ToArray())
                            End If
                        End While
                    End Using
                End Using


                UpdateSetelahTahan()
                UpdateSemuaTotal()
            End If
        End If
    End Sub


    Private Sub UpdateSetelahTahan()
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                ' Mengumpulkan informasi barang
                Dim sql As String = "SELECT HARGA_BELI, STOK_TOKO, STOK_GUDANG " &
                   "FROM tbl_barang " &
                   "WHERE TRIM(ID_BARANG) LIKE @ID_BARANG"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            dgvRow.Cells("HargaBeli").Value = If(IsDBNull(rd("HARGA_BELI")), 0D, ParseDecimal(rd("HARGA_BELI")))
                            dgvRow.Cells("StokToko").Value = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            dgvRow.Cells("StokGudang").Value = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))

                            Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                                          If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG"))),
                                                          If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO"))))

                            dgvRow.Cells("Stok").Value = stokValue
                        End If
                    End Using
                End Using
            End If
        Next
        Hitungbaris()
    End Sub

    Private Sub Hitungbaris()
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso Not IsDBNull(row.Cells("Kode").Value) AndAlso Not String.IsNullOrEmpty(row.Cells("Kode").Value.ToString()) Then
                ' Variables for HargaBeli calculation
                Dim hargaBeli = If(Not IsDBNull(row.Cells("HargaBeli").Value), ParseDecimal(row.Cells("HargaBeli").Value), 0)
                Dim qty = If(Not IsDBNull(row.Cells("QTY").Value), ParseDecimal(row.Cells("QTY").Value), 0)
                Dim isi = If(Not IsDBNull(row.Cells("isi").Value), ParseDecimal(row.Cells("isi").Value), 1)

                ' Variables for QtySat calculation
                Dim qtySat = qty * isi

                ' Variables for TotalHarga calculation
                Dim harga = If(Not IsDBNull(row.Cells("Harga").Value), ParseDecimal(row.Cells("Harga").Value), 0)
                Dim totalDiskon = If(Not IsDBNull(row.Cells("TotalDiskon").Value), ParseDecimal(row.Cells("TotalDiskon").Value), 0)

                ' Variables for TotalHarga calculation
                Dim totalHarga = qty * harga - totalDiskon

                ' Assign values to DataGridView columns
                row.Cells("Totalhargabeli").Value = hargaBeli * qtySat
                row.Cells("QtySat").Value = qtySat
                row.Cells("TotalHarga").Value = totalHarga
            End If
        Next
    End Sub

    Public Sub TekanBarang()
        TambahBarang.LblUtama.Text = "T A M B A H   B A R A N G"
        'TambahBarang.BtnSimpan.Text = "SIMPAN TAMBAH"
        TambahBarang.ShowDialog()
    End Sub

    Public Sub Tekanpelanggan()
        TambahPelanggan.ShowDialog()
        Call TampilPelanggan()
    End Sub


    Public Class StokInfo
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    Public Function CekStok() As Boolean
        ' Membuat Dictionary untuk menyimpan informasi stok barang dan penjualan
        Dim stokDict As New Dictionary(Of String, StokInfo) ' (stokToko, stokGudang)

        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                ' Mengumpulkan informasi stok barang
                Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE trim(ID_BARANG) like @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            Dim stokToko As Decimal = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            Dim stokGudang As Decimal = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))

                            stokDict(kodeBarangValue) = New StokInfo() With {
                                .StokToko = stokToko,
                                .StokGudang = stokGudang
                            }
                        End If
                    End Using
                End Using

                If TxtJenistransaksi.Text <> "TambahPenjualan" Then
                    ' Mengumpulkan informasi penjualan
                    Using cmdjual As New MySqlCommand("SELECT ID_BARANG, SUM(QTY_SATUAN) AS TOTAL_QTY FROM penjualan_detail WHERE FAKTUR_JUAL = @FAKTUR_JUAL AND ID_BARANG = @ID_BARANG GROUP BY ID_BARANG", conn)
                        cmdjual.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)
                        cmdjual.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                        Using rdjual As MySqlDataReader = cmdjual.ExecuteReader()
                            While rdjual.Read()
                                Dim idBarang As String = rdjual("ID_BARANG").ToString()

                                ' Memastikan TOTAL_QTY tidak DBNull sebelum konversi
                                Dim totalQtyTerjualRow As Decimal = If(IsDBNull(rdjual("TOTAL_QTY")), 0D, ParseDecimal(rdjual("TOTAL_QTY")))

                                If stokDict.ContainsKey(idBarang) Then
                                    Dim stokInfo As StokInfo = stokDict(idBarang)

                                    If LblLokasiBarang.Text = "TOKO" Then
                                        stokInfo.StokToko += totalQtyTerjualRow
                                    Else
                                        stokInfo.StokGudang += totalQtyTerjualRow
                                    End If
                                End If
                            End While
                        End Using
                    End Using
                End If

                ' Memproses data di DataGridView
                Dim totalQtyTerjual As Decimal = If(IsDBNull(dgvRow.Cells("QtySat").Value), 0D, ParseDecimal(dgvRow.Cells("QtySat").Value))
                If stokDict.ContainsKey(kodeBarangValue) Then
                    Dim stokInfo As StokInfo = stokDict(kodeBarangValue)
                    Dim totalStok As Decimal

                    If LblLokasiBarang.Text = "TOKO" Then
                        totalStok = stokInfo.StokToko
                    Else
                        totalStok = stokInfo.StokGudang
                    End If

                    If totalQtyTerjual > totalStok Then
                        ' Stok tidak mencukupi
                        Dim errorMessage As String = "Stok ==> " & dgvRow.Cells("NamaBarang").Value & " <== tidak mencukupi untuk dijual. " & vbCrLf & vbCrLf & "Total Terjual: " & totalQtyTerjual & vbCrLf & ", Total Stok: " & totalStok
                        MessageBox.Show(errorMessage, "Stok Tidak cukup", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        ' Menyorot baris yang bermasalah
                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = Color.Red
                        Next

                        ' Setelah menyaring baris, pastikan bahwa baris tersebut terpilih juga
                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1) ' Pilih sel pertama atau sesuaikan dengan indeks kolom yang ingin Anda pilih
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True ' Ada masalah
                    End If

                    ' Simpan warna default
                    Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor

                    ' Kemudian, untuk mengembalikan sel kembali ke warna default
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = defaultBackColor
                    Next
                End If
            End If
        Next

        Return False
    End Function

    Public Function Cekjualrugi() As Boolean
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                Dim Hargabeli As Decimal = 0
                ' Mengumpulkan informasi barang
                Using cmd As New MySqlCommand("SELECT HARGA_BELI FROM tbl_barang WHERE trim(ID_BARANG) like @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            ' Memastikan HARGA_BELI tidak DBNull sebelum konversi
                            Hargabeli = If(IsDBNull(rd("HARGA_BELI")), 0D, ParseDecimal(rd("HARGA_BELI")))
                        End If
                    End Using
                End Using

                ' Memproses data di DataGridView
                Dim Hargajual As Decimal
                If Decimal.TryParse(dgvRow.Cells("Harga").Value.ToString(), Hargajual) Then
                    If Hargabeli > Hargajual Then
                        ' Harga jual rugi
                        Dim errorMessage As String = "Barang: " & dgvRow.Cells("NamaBarang").Value & vbCrLf & vbCrLf & "Harga beli: " & Hargabeli.ToString("N0") & vbCrLf & vbCrLf & "Harga jual: " & Hargajual.ToString("N0")
                        MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        ' Menyorot baris yang bermasalah
                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = Color.Red
                        Next

                        ' Setelah menyaring baris, pastikan bahwa baris tersebut terpilih juga
                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1) ' Pilih sel pertama atau sesuaikan dengan indeks kolom yang ingin Anda pilih
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True ' Ada masalah
                    End If
                Else
                    ' Penanganan ketika Hargajual tidak dapat di-parse
                    MessageBox.Show("Harga jual tidak valid untuk barang " & kodeBarangValue, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If


                ' Simpan warna default
                Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor

                ' Kemudian, untuk mengembalikan sel kembali ke warna default
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = defaultBackColor
                Next
            End If
        Next

        Return False
    End Function


    Private Sub TxtBayar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtBayar.KeyPress
        ' Memeriksa apakah karakter yang dimasukkan adalah angka atau kontrol seperti backspace
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Mengabaikan karakter yang tidak diizinkan
        End If
        If e.KeyChar = Chr(13) Then
            If PanelTFPelanggan.Visible = True And TxtNamaRek.Text = "" Then
                TxtNamaRek.Focus()
            Else
                Tekansimpan()
            End If
        End If
    End Sub


    Private Sub CmbJenisBayar_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJenisBayar.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi pelanggan
        AmbiuldataRekening()
    End Sub

    Private Sub CmbJenisBayar_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbJenisBayar.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            If PanelTFPelanggan.Visible = True Then
                TxtBank.Focus()
            Else
                TxtBayar.Focus()
            End If
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub AmbiuldataRekening()
        Dim namaAkunD As String = CmbJenisBayar.Text

        Dim sql As String = "SELECT Type_Akun, Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtKodeRef.Text = reader("Kode_akun").ToString()
                    Dim typeakun As String = reader("Type_Akun").ToString()

                    If typeakun = "BANK" Then
                        ' Set properti untuk kondisi BANK
                        GBBayar.Size = New Size(933, 280)
                        BtnSimpann.Location = New Point(536, 230)
                        BtnBatal.Location = New Point(737, 230)

                        PanelTFPelanggan.Visible = True
                        TxtTypeAkun.Text = "BANK"
                    Else
                        TxtBank.Clear()
                        TxtNoRek.Clear()
                        TxtNamaRek.Clear()
                        TxtNoReff.Clear()
                        ' Set properti untuk kondisi selain BANK (misalnya, TUNAI)
                        GBBayar.Size = New Size(529, 280)
                        BtnSimpann.Location = New Point(115, 230)
                        BtnBatal.Location = New Point(316, 230)

                        PanelTFPelanggan.Visible = False
                        TxtTypeAkun.Text = "TUNAI"
                    End If

                End If
            End Using
        End Using

    End Sub

    Private bantuanBayar As Decimal
    Private kas As Decimal
    Private bayar As Decimal
    Private kembali As Decimal

    Private Sub TxtBayar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtBayar.TextChanged, TxtTotalStlPajak.TextChanged
        Dim input As String = TxtBayar.Text.Trim()

        ' Memeriksa apakah input kosong atau tidak valid
        If String.IsNullOrEmpty(input) OrElse Not Decimal.TryParse(input, bayar) Then
            TxtKembali.Text = TxtTotalStlPajak.Text
            LblBayar.Text = "0"
            Exit Sub
        End If

        ' Format jumlah bayar
        LblBayar.Text = bayar.ToString("#,0.##", cultureIndonesia)


        Dim total As Decimal
        If Decimal.TryParse(TxtTotalStlPajak.Text, total) Then
            bantuanBayar = total - bayar
            TxtKembali.Text = Math.Abs(bantuanBayar)
            kembali = Math.Abs(bantuanBayar)
            kas = bayar - kembali

            ' Menampilkan elemen UI sesuai dengan kondisi

            LblJatuhTempo.Visible = False
            DTPJatuhTempo.Visible = False

            ' Memeriksa apakah ada hutang atau kembali
            If bantuanBayar > 0 Then
                LblPembayaran.Text = "Hutang :"
                LblStatusTrans.Text = "Belum Lunas"
                LblJatuhTempo.Visible = True
                DTPJatuhTempo.Visible = True
            Else
                LblPembayaran.Text = "Kembali :"
                LblStatusTrans.Text = "Lunas"
                LblJatuhTempo.Visible = False
                DTPJatuhTempo.Visible = False
            End If
        End If
    End Sub

    Private Sub PanelTFPelanggan_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelTFPelanggan.VisibleChanged
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            TxtBank.Clear()
            TxtNoRek.Clear()
            TxtNamaRek.Clear()
            TxtNoReff.Clear()
        End If

    End Sub

    Public Sub Tekansimpan()
        If TxtTypeAkun.Text = "BANK" And TxtBayar.Text = "" Or TxtBayar.Text = "0" Then
            MessageBox.Show("Jika transfer nomial harus di isi sesuai transfernya ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
            TxtBayar.Focus()
        End If

        If LblStatusTrans.Text = "Belum Lunas" And CmbPelanggan.Text = "" Then
            MessageBox.Show("Jika pembayaran belum lunas, Pelanggan harus di pilih atau tambahkan jika belum ada ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbPelanggan.DroppedDown = True ' Memunculkan dropdown list
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(TxtBayar.Text) OrElse TxtBayar.Text = "0" Then
            Dim userResponse As MsgBoxResult = MsgBox("Nominal Pembayaran belum diisi. Lanjutkan?", MsgBoxStyle.OkCancel, "Perhatian Penting")
            If userResponse = MsgBoxResult.Ok Then
                Simpanatauedit()
            End If
        Else
            Simpanatauedit()
        End If


    End Sub

    Public Sub Simpanatauedit()
        Cursor = Cursors.WaitCursor
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            DTPTgl.Value = DateTime.Now

            Dim query As String = "SELECT ID_PENJUALAN FROM penjualan WHERE ID_PENJUALAN = ?"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Nomorjual()
                End If
            End Using
            Prosessimpan()
        Else
            Prosessimpan()
        End If
        Cursor = Cursors.Default
    End Sub

    Public Sub Prosessimpan()
        If String.IsNullOrEmpty(TxtBayar.Text) OrElse TxtBayar.Text = "0" Then
            TxtBayar.Text = "0"
            TxtTypeAkun.Text = "PIUTANG"
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        ' Mengubah kursor menjadi menunggu
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Try
            ' Panggil metode Hapusuntukedit jika bukan TambahPenjualan
            If TxtJenistransaksi.Text <> "TambahPenjualan" Then
                Hapusuntukedit(transaction)
            End If

            ' Panggil metode-metode Simpan dengan menggunakan transaksi yang sama
            Simpanpenjualan(transaction)
            'Simpanpiutang(transaction)
            Simpanpenjualandetail(transaction)
            Simpanjurnal(transaction)
            HistoryBarang(transaction)

            ' Commit transaksi
            transaction.Commit()

            ' Jika semuanya berhasil, kembalikan kondisi awal
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    HitungByKode(row.Cells(0).Value)
                End If
            Next


            If CmbCetak.Text.ToLower() = "iya" Then
                ' Proceed with printing
                CetakFaktur()
            ElseIf CmbCetak.Text.ToLower() = "tanya" Then
                ' Display a confirmation dialog
                Dim result As DialogResult = MessageBox.Show("Apakah Anda ingin mencetak transfer barang?", "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    ' Proceed with printing
                    CetakFaktur()
                End If
            End If




            ' Bersihkan dan kembali ke kondisi awal
            DgvData.DataSource = Nothing
            DgvData.Rows.Clear()
            Kondisiawal()

            If TxtJenistransaksi.Text <> "TambahPenjualan" Then
                TxtJenistransaksi.Text = "TambahPenjualan"
                TxtFaktur.Text = ""
                Close()
            End If

        Catch ex As Exception
            MessageBox.Show("Oh tidak! Transaksi penjualan dibatalkan karena terjadi kesalahan." & vbCrLf &
                     "Detail kesalahan: " & ex.Message,
           "Oops! Ada masalah simpan penjualan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Rollback transaksi secara otomatis karena ada kesalahan
            transaction.Rollback()
        Finally
            ' Mengembalikan kursor ke normal setelah proses selesai atau terjadi kesalahan
            System.Windows.Forms.Cursor.Current = Cursors.Default
        End Try

    End Sub

    ' Subroutine untuk mengurangi duplikasi kode cetak
    Sub CetakFaktur()
        With PrintJual
            .TxtFaktur.Text = TxtFaktur.Text
            .ProsesCetak()
        End With
    End Sub

    Public Sub Hapusuntukedit(ByVal transaction As MySqlTransaction)
        Dim updateStokField As String

        Select Case LblLokasiBarang.Text
            Case "TOKO"
                updateStokField = "PENJUALAN_TOKO"
            Case "GUDANG"
                updateStokField = "PENJUALAN_GUDANG"
            Case Else
                Throw New Exception("Lokasi barang tidak valid.")
                Exit Sub
        End Select

        Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"

        For Each row As DataGridViewRow In FormUtama.DGVDetail.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim QtyTotal As Decimal = If(row.Cells("QTY_SATUAN").Value IsNot Nothing, ParseDecimal(row.Cells("QTY_SATUAN").Value), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@StokPengurangan", QtyTotal)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using

                    HitungStokPerubahan(kodeBarang, transaction)
                End If
            End If
        Next

        Dim fakturPenjualan As String = TxtFaktur.Text

        Dim deleteQueries As String() = {
        "DELETE FROM penjualan WHERE ID_PENJUALAN = @FakturPenjualan",
        "DELETE FROM penjualan_detail WHERE FAKTUR_JUAL = @FakturPenjualan",
        "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FakturPenjualan",
        "DELETE FROM HistoryBarang WHERE FAKTUR = @FakturPenjualan"
    }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@FakturPenjualan", fakturPenjualan)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Public Sub Simpanpenjualan(ByVal transaction As MySqlTransaction)
        ' menghitung laba dari data di DataGridView (DgvData)

        Dim totalHarga As Decimal = 0
        Dim totalHargaBeli As Decimal = 0
        Dim diskon As Decimal = 0

        ' Loop melalui setiap baris di DgvData untuk menghitung total harga dan total harga beli
        For Each row As DataGridViewRow In DgvData.Rows
            ' Tambahkan totalHarga dan totalHargaBeli dengan nilai dari kolom, anggap 0 jika null
            totalHarga += If(IsDBNull(row.Cells("TotalHarga").Value), 0D, ParseDecimal(row.Cells("TotalHarga").Value))
            totalHargaBeli += If(IsDBNull(row.Cells("Totalhargabeli").Value), 0D, ParseDecimal(row.Cells("Totalhargabeli").Value))
        Next

        ' Cek apakah TxtDiskonRp memiliki nilai dan konversi ke Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskon) = False Then
            diskon = 0
        End If

        ' Hitung laba
        Dim laba As Decimal = (totalHarga - totalHargaBeli) - diskon



        Dim query As String = "INSERT INTO penjualan (ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, " &
                        "TGL_TRANSAKSI, GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, " &
                        "GRAND_TOTAL_STL_PAJAK, LABA, BAYAR, TOTAL_HPP, BIAYA_KIRIM, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_BAYAR, STATUS_TRANSAKSI, " &
                        "TYPE_AKUN, KODE_AKUN, JENIS_PEMBAYARAN, METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, " &
                        "ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER) " &
                        "VALUES (@ID_PENJUALAN, @ID_PELANGGAN, @NAMA_PELANGGAN, @ALAMAT_PELANGGAN, @JENIS_PELANGGAN, @LOKASIBARANG, " &
                        "@TGL_TRANSAKSI, @GRAND_TOTAL_SBL_PAJAK, @DISKON_TOTAL_PERSEN, @DISKON_TOTAL_RP, @PAJAK_PERSEN, @PAJAK_RP, " &
                        "@GRAND_TOTAL_STL_PAJAK, @LABA, @BAYAR, @TOTAL_HPP, @BIAYA_KIRIM, @KEMBALI, @SISA_TAGIHAN, @JATUH_TEMPO, @STATUS_BAYAR, @STATUS_TRANSAKSI, " &
                        "@TYPE_AKUN, @KODE_AKUN, @JENIS_PEMBAYARAN, @METODE, @BANK, @NO_REKENING, @NAMA_REKENING, @NO_REFFERENSI, " &
                        "@ID_SALES, @NAMA_SALES, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", LbLKodePel.Text)
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", LblAlamat.Text)
            cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPl.Text)
            cmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", Microsoft.VisualBasic.Format(DTPTgl.Value, "yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_SBL_PAJAK", ParseDecimal(TxtTotalBelanja.Text))
            cmd.Parameters.AddWithValue("@DISKON_TOTAL_PERSEN", ParseDecimal(TxtDiskonPersen.Text))
            cmd.Parameters.AddWithValue("@DISKON_TOTAL_RP", ParseDecimal(TxtDiskonRp.Text))
            cmd.Parameters.AddWithValue("@PAJAK_PERSEN", ParseDecimal(TxtPajakPersen.Text))
            cmd.Parameters.AddWithValue("@PAJAK_RP", ParseDecimal(TxtPajakRp.Text))
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_STL_PAJAK", ParseDecimal(TxtTotalStlPajak.Text))
            cmd.Parameters.AddWithValue("@LABA", laba)
            cmd.Parameters.AddWithValue("@BAYAR", ParseDecimal(TxtBayar.Text))
            cmd.Parameters.AddWithValue("@TOTAL_HPP", ParseDecimal(TxtHpp.Text)) 'Di isi hpp pembelian barang
            cmd.Parameters.AddWithValue("@BIAYA_KIRIM", ParseDecimal(TxtBiayaKirim.Text))

            Dim statusbayar As String
            If LblStatusTrans.Text = "Lunas" Then
                statusbayar = "TERBAYAR"
                cmd.Parameters.AddWithValue("@KEMBALI", ParseDecimal(TxtKembali.Text))
                cmd.Parameters.AddWithValue("@SISA_TAGIHAN", 0)
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", DBNull.Value)
            Else
                statusbayar = "TERHUTANG"
                cmd.Parameters.AddWithValue("@KEMBALI", 0)
                cmd.Parameters.AddWithValue("@SISA_TAGIHAN", ParseDecimal(TxtKembali.Text))
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "yyyy-MM-dd"))
            End If

            cmd.Parameters.AddWithValue("@STATUS_BAYAR", statusbayar)
            cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI", LblStatusTrans.Text)
            cmd.Parameters.AddWithValue("@TYPE_AKUN", TxtTypeAkun.Text)
            cmd.Parameters.AddWithValue("@KODE_AKUN", TxtKodeRef.Text)
            cmd.Parameters.AddWithValue("@JENIS_PEMBAYARAN", CmbJenisBayar.Text)
            cmd.Parameters.AddWithValue("@METODE", "Transfer")
            cmd.Parameters.AddWithValue("@BANK", TxtBank.Text)
            cmd.Parameters.AddWithValue("@NO_REKENING", TxtNoRek.Text)
            cmd.Parameters.AddWithValue("@NAMA_REKENING", TxtNamaRek.Text)
            cmd.Parameters.AddWithValue("@NO_REFFERENSI", TxtNoReff.Text)
            cmd.Parameters.AddWithValue("@ID_SALES", LblSales.Text)
            cmd.Parameters.AddWithValue("@NAMA_SALES", CmbSales.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using

    End Sub

    Public Sub Simpanpiutang(ByVal transaction As MySqlTransaction)
        If LblStatusTrans.Text = "Belum Lunas" Then
            ' Simpan hutang pelanggan
            Using cmdSimpanHutang As New MySqlCommand("INSERT INTO penjualan_Piutang (IDPENJUALAN, TGL_BELANJA, LOKASI, KODE_PELANGGAN, NAMA_PELANGGAN, QTY, TOTAL_RUPIAH, BAYAR, HUTANG, JATUH_TEMPO, ID_USER, ID_KOMPUTER, STATUS) VALUES (@ID_TRANSAKSI, @TglBelanja, @LOKASI, @KodeSp, @NamaSP, @Qty, @TOTAL_RUPIAH, @BAYAR, @HUTANG, @JATUH_TEMPO, @ID_USER, @ID_KOMPUTER, @STATUS)", conn, transaction)
                cmdSimpanHutang.Parameters.AddWithValue("@IDPENJUALAN", TxtFaktur.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@TglBelanja", Microsoft.VisualBasic.Format(DTPTgl.Value, "yyyy-MM-dd HH:mm:ss"))
                cmdSimpanHutang.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@KodeSp", LbLKodePel.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@NamaSP", CmbPelanggan.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@Qty", ParseDecimal(TxtJmlhBrg.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@TOTAL_RUPIAH", ParseDecimal(TxtTotalStlPajak.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@BAYAR", ParseDecimal(TxtBayar.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@HUTANG", ParseDecimal(TxtKembali.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@JATUH_TEMPO", Microsoft.VisualBasic.Format(DTPJatuhTempo.Value, "yyyy-MM-dd HH:mm:ss"))
                cmdSimpanHutang.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@STATUS", LblStatusTrans.Text)

                cmdSimpanHutang.ExecuteNonQuery()
            End Using
        End If
    End Sub

    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "PENJUALAN")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@QTY", If(IsDBNull(row.Cells(3).Value), 0D, ParseDecimal(row.Cells(3).Value)))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", If(IsDBNull(row.Cells(5).Value), 0D, ParseDecimal(row.Cells(5).Value)))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", If(IsDBNull(row.Cells(8).Value), 0D, ParseDecimal(row.Cells(8).Value)))
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", If(IsDBNull(row.Cells(12).Value), 0D, ParseDecimal(row.Cells(12).Value)))
                    cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

                    cmd.ExecuteNonQuery()
                End Using

            End If
        Next
    End Sub

    Public Sub Simpanpenjualandetail(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_jual
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                ' Insert data into penjualan_detail
                Dim insertQuery As String = "INSERT INTO penjualan_detail (FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TANGGAL_JUAL, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, LABA, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@FAKTUR_JUAL, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @LOKASIBARANG, @TANGGAL_JUAL, @ID_BARANG, @NAMA_BARANG, @HARGA_BELI, @QTY, @SATUAN, @ISI_SATUAN, @HARGA_BELI_SATUAN, @HARGA_JUAL, @QTY_SATUAN, @DISKON_PERSEN, @DISKON_RP, @TOTAL_DISKON, @TOTAL_HARGA, @LABA, @ID_USER, @ID_KOMPUTER)"

                Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
                    ' Menambahkan parameter dengan nilai dari kontrol dan baris DataGridView
                    insertCmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)
                    insertCmd.Parameters.AddWithValue("@ID_PELANGGAN", LbLKodePel.Text)
                    insertCmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
                    insertCmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPl.Text)
                    insertCmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
                    insertCmd.Parameters.AddWithValue("@TANGGAL_JUAL", Microsoft.VisualBasic.Format(DTPTgl.Value, "yyyy-MM-dd HH:mm:ss"))
                    insertCmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    insertCmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)

                    ' Penanganan DBNull untuk setiap nilai yang dikonversi menggunakan ParseDecimal
                    insertCmd.Parameters.AddWithValue("@HARGA_BELI", If(IsDBNull(row.Cells(2).Value), 0D, ParseDecimal(row.Cells(2).Value)))
                    insertCmd.Parameters.AddWithValue("@QTY", If(IsDBNull(row.Cells(3).Value), 0D, ParseDecimal(row.Cells(3).Value)))
                    insertCmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    insertCmd.Parameters.AddWithValue("@ISI_SATUAN", If(IsDBNull(row.Cells(5).Value), 0D, ParseDecimal(row.Cells(5).Value)))
                    insertCmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", If(IsDBNull(row.Cells(6).Value), 0D, ParseDecimal(row.Cells(6).Value)))
                    insertCmd.Parameters.AddWithValue("@HARGA_JUAL", If(IsDBNull(row.Cells(7).Value), 0D, ParseDecimal(row.Cells(7).Value)))
                    insertCmd.Parameters.AddWithValue("@QTY_SATUAN", If(IsDBNull(row.Cells(8).Value), 0D, ParseDecimal(row.Cells(8).Value)))
                    insertCmd.Parameters.AddWithValue("@DISKON_PERSEN", If(IsDBNull(row.Cells(9).Value), 0D, ParseDecimal(row.Cells(9).Value)))
                    insertCmd.Parameters.AddWithValue("@DISKON_RP", If(IsDBNull(row.Cells(10).Value), 0D, ParseDecimal(row.Cells(10).Value)))
                    insertCmd.Parameters.AddWithValue("@TOTAL_DISKON", If(IsDBNull(row.Cells(11).Value), 0D, ParseDecimal(row.Cells(11).Value)))
                    insertCmd.Parameters.AddWithValue("@TOTAL_HARGA", If(IsDBNull(row.Cells(12).Value), 0D, ParseDecimal(row.Cells(12).Value)))

                    ' Menghitung LABA dengan nilai default
                    Dim hargaJual As Decimal = If(IsDBNull(row.Cells(12).Value), 0D, ParseDecimal(row.Cells(12).Value))
                    Dim hargaBeli As Decimal = If(IsDBNull(row.Cells(6).Value), 0D, ParseDecimal(row.Cells(6).Value))
                    insertCmd.Parameters.AddWithValue("@LABA", hargaJual - hargaBeli)


                    insertCmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
                    insertCmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

                    insertCmd.ExecuteNonQuery()
                End Using


                Dim updateStokField As String
                Select Case LblLokasiBarang.Text
                    Case "TOKO"
                        updateStokField = "PENJUALAN_TOKO"
                    Case "GUDANG"
                        updateStokField = "PENJUALAN_GUDANG"
                    Case Else
                        Throw New Exception("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " + ? WHERE ID_BARANG = ?"
                Dim kodeBarang As String = If(row.Cells("Kode").Value IsNot Nothing, row.Cells("Kode").Value.ToString(), String.Empty)
                If Not String.IsNullOrEmpty(kodeBarang) Then
                    ' Menggunakan format default untuk memastikan format desimal
                    Dim qtyTotal As Decimal = If(row.Cells("QtySat").Value IsNot Nothing, Decimal.Parse(row.Cells("QtySat").Value.ToString()), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        ' Menambahkan parameter untuk query
                        cmd.Parameters.AddWithValue("@P1", qtyTotal)
                        cmd.Parameters.AddWithValue("@P2", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End If
        Next
    End Sub


    Public Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        Dim nominalkas As Decimal

        ' KAS/BANK
        If bantuanBayar < 0 Then
            nominalkas = kas
        Else
            nominalkas = bayar
        End If

        ' PERSEDIAAN BARANG
        Dim persediaanbarang As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                persediaanbarang += ParseDecimal(row.Cells("Totalhargabeli").Value)
            End If
        Next

        ' LABA KOTOR PENJUALAN
        Dim labakotor As Decimal = ParseDecimal(TxtTotalSblDiskon.Text) - persediaanbarang

        ' SIMPAN JURNAL KAS atau PIUTANG
        If bayar > 0 Then
            Dim uraian As String
            If bantuanBayar <= 0 Then
                uraian = "Dibayar lunas penjualan dari " & CmbPelanggan.Text
            Else
                uraian = "Uang muka pembayaran penjualan dari " & CmbPelanggan.Text
            End If

            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, uraian, CmbJenisBayar.Text, TxtKodeRef.Text, "", "", nominalkas, "Penjualan", "", "")
        Else
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Piutang penjualan dari " & CmbPelanggan.Text, nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, "", "", kembali, "Penjualan", CmbPelanggan.Text, LbLKodePel.Text)
        End If

        ' SIMPAN JURNAL SISA PIUTANG
        If bayar > 0 And bantuanBayar > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Piutang penjualan dari " & CmbPelanggan.Text, nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, "", "", kembali, "Penjualan", CmbPelanggan.Text, LbLKodePel.Text)
        End If

        ' SIMPAN JURNAL DISKON ITEM
        Dim diskontotal As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                diskontotal += ParseDecimal(row.Cells("TotalDiskon").Value)
            End If
        Next

        If diskontotal > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Diskon item penjualan dari " & CmbPelanggan.Text, "BEBAN DISKON PENJUALAN", "07.01.010", "LABA KOTOR PENJUALAN", "06.01.001", diskontotal, "Penjualan", "", "")
        End If

        ' SIMPAN JURNAL TOTAL DISKON
        If ParseDecimal(TxtDiskonRp.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Diskon total penjualan dari " & CmbPelanggan.Text, "BEBAN DISKON PENJUALAN", "07.01.010", "", "", ParseDecimal(TxtDiskonRp.Text), "Penjualan", "", "")
        End If

        ' SIMPAN JURNAL PERSEDIAAN BARANG
        SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "HPP penjualan kepada " & CmbPelanggan.Text, "", "", NAMA_REK_BARANG, KODE_REK_BARANG, persediaanbarang, "Penjualan", "", "")

        ' SIMPAN JURNAL HUTANG PAJAK
        If ParseDecimal(TxtPajakRp.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Hutang pajak penjualan dari " & CmbPelanggan.Text, "", "", "HUTANG PAJAK", "03.02.001", ParseDecimal(TxtPajakRp.Text), "Penjualan", "", "")
        End If

        ' SIMPAN JURNAL LABA
        SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Laba kotor penjualan dari " & CmbPelanggan.Text, "", "", "LABA KOTOR PENJUALAN", "06.01.001", labakotor, "Penjualan", "", "")

        ' SIMPAN JURNAL BIAYA KIRIM
        If ParseDecimal(TxtBiayaKirim.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Jasa kirim/Lain " & CmbPelanggan.Text, "", "", "PENDAPATAN LAIN LAIN", "08.01.002", ParseDecimal(TxtBiayaKirim.Text), "Penjualan", "", "")
        End If

    End Sub

    Private Sub SimpanJurnalUmum(ByVal transaction As MySqlTransaction, ByVal NO_TRANSAKSI As String, ByVal tglTransaksi As DateTime, ByVal uraian As String, ByVal namaAkunD As String, ByVal nomorAkunD As String, ByVal namaAkunK As String, ByVal nomorAkunK As String, ByVal nominal As Decimal, ByVal jenisTransaksi As String, ByVal namaBantuD As String, ByVal kodeBantuD As String)
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_D, KODE_BANTU_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_D, @KODE_BANTU_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", NO_TRANSAKSI)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tglTransaksi.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", namaAkunD)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkunD)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", namaAkunK)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkunK)
            cmd.Parameters.AddWithValue("@NAMA_BANTU_D", namaBantuD)
            cmd.Parameters.AddWithValue("@KODE_BANTU_D", kodeBantuD)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", jenisTransaksi)
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Public Sub Editpenjualanheader()
        ' Deklarasikan variabel di luar MySqlDataReader
        Dim kodepel As String = String.Empty
        Dim namaPelanggan As String = String.Empty
        Dim jenisPelanggan As String = String.Empty
        Dim lokasibarang As String = String.Empty
        Dim tglTransaksi As DateTime = DateTime.MinValue

        Dim diskonPersen As Decimal = 0D
        Dim diskonRp As Decimal = 0D
        Dim pajakPersen As Decimal = 0D
        Dim pajakRp As Decimal = 0D
        Dim BiayaKirim As Decimal = 0D

        Dim kodeRef As String = String.Empty
        Dim jenisPembayaran As String = String.Empty
        Dim BANK As String = String.Empty
        Dim NO_REKENING As String = String.Empty
        Dim NAMA_REKENING As String = String.Empty
        Dim NO_REFFERENSI As String = String.Empty
        Dim SALES As String = String.Empty
        Dim NAMASALES As String = String.Empty
        Dim USER As String = String.Empty
        Dim KOMPUTER As String = String.Empty

        Dim queryString As String = "SELECT ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, BIAYA_KIRIM, KODE_AKUN, JENIS_PEMBAYARAN, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER FROM penjualan " &
                            "WHERE ID_PENJUALAN = ?"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Assign nilai dari MySqlDataReader ke variabel
                    kodepel = rd("ID_PELANGGAN").ToString()
                    namaPelanggan = rd("NAMA_PELANGGAN").ToString()
                    jenisPelanggan = rd("JENIS_PELANGGAN").ToString()
                    lokasibarang = rd("LOKASIBARANG").ToString()
                    tglTransaksi = Convert.ToDateTime(rd("TGL_TRANSAKSI"))

                    diskonPersen = If(IsDBNull(rd("DISKON_TOTAL_PERSEN")), 0D, ParseDecimal(rd("DISKON_TOTAL_PERSEN")))
                    diskonRp = If(IsDBNull(rd("DISKON_TOTAL_RP")), 0D, ParseDecimal(rd("DISKON_TOTAL_RP")))
                    pajakPersen = If(IsDBNull(rd("PAJAK_PERSEN")), 0D, ParseDecimal(rd("PAJAK_PERSEN")))
                    pajakRp = If(IsDBNull(rd("PAJAK_RP")), 0D, ParseDecimal(rd("PAJAK_RP")))
                    BiayaKirim = If(IsDBNull(rd("BIAYA_KIRIM")), 0D, ParseDecimal(rd("BIAYA_KIRIM")))

                    kodeRef = rd("KODE_AKUN").ToString()
                    jenisPembayaran = rd("JENIS_PEMBAYARAN").ToString()
                    BANK = rd("BANK").ToString()
                    NO_REKENING = rd("NO_REKENING").ToString()
                    NAMA_REKENING = rd("NAMA_REKENING").ToString()
                    NO_REFFERENSI = rd("NO_REFFERENSI").ToString()
                    SALES = rd("ID_SALES").ToString()
                    NAMASALES = rd("NAMA_SALES").ToString()
                    USER = rd("ID_USER").ToString()
                    KOMPUTER = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using

        ' Set nilai ke kontrol
        CmbPelanggan.SelectedIndex = CmbPelanggan.FindStringExact(namaPelanggan)
        LbLKodePel.Text = kodepel
        LblJenisPl.Text = jenisPelanggan
        LblLokasiBarang.Text = lokasibarang
        DTPTgl.Value = tglTransaksi

        Editpenjualan()
        TxtDiskonPersen.Text = diskonPersen.ToString("#,0.##")
        TxtDiskonRp.Text = diskonRp.ToString("#,0.##")
        TxtPajakPersen.Text = pajakPersen.ToString("#,0.##")
        TxtPajakRp.Text = pajakRp.ToString("#,0.##")
        TxtBiayaKirim.Text = BiayaKirim.ToString("#,0.##")
        TxtKodeRef.Text = kodeRef
        CmbJenisBayar.SelectedIndex = CmbJenisBayar.FindStringExact(jenisPembayaran)
        TxtBank.Text = BANK
        TxtNoRek.Text = NO_REKENING
        TxtNamaRek.Text = NAMA_REKENING
        TxtNoReff.Text = NO_REFFERENSI
        LblSales.Text = SALES
        CmbSales.SelectedIndex = CmbSales.FindStringExact(NAMASALES)
        TxtLogin.Text = USER
        TxtKomputer.Text = KOMPUTER
    End Sub


    Public Sub Editpenjualan()
        If TxtJenistransaksi.Text = "EditPenjualan" Then
            TxtFaktur.Text = TxtFaktur.Text
            DgvData.Rows.Clear()

            ' Mengisi DataGridView dari penjualan_detail
            Dim penjualanDetail As DataTable = GetPenjualanDetail(TxtFaktur.Text)
            For Each row As DataRow In penjualanDetail.Rows
                Dim dgvRow As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
                For i As Integer = 0 To penjualanDetail.Columns.Count - 1
                    dgvRow.Cells(i).Value = row(i)
                Next

                ' Mengisi ComboBoxCell berdasarkan satuan
                Dim idBarang As String = dgvRow.Cells(0).Value.ToString()
                Dim satuanList As List(Of String) = GetSatuanBarang(idBarang, LblJenisPl.Text = "Partai")

                Dim comboCell As DataGridViewComboBoxCell = CType(dgvRow.Cells("Satuan"), DataGridViewComboBoxCell)
                comboCell.Items.Clear()
                comboCell.Items.AddRange(satuanList.ToArray())

                ' Mengisi stok barang
                Dim stok As Tuple(Of Decimal, Decimal) = GetStokBarang(idBarang, TxtFaktur.Text)
                dgvRow.Cells("StokToko").Value = stok.Item1
                dgvRow.Cells("StokGudang").Value = stok.Item2
            Next

            UpdateSemuaTotal()

            ' Mengatur baris terakhir di DgvData
            If DgvData.Rows.Count > 0 Then
                DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)
                DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
            End If

            ' Mengatur fokus jika AwalPenjualan adalah "Pencarian"
            If AwalPenjualan = "Pencarian" Then
                TxtNama.Select()
                TxtNama.Focus()
            End If
        End If
    End Sub

    Private Function GetPenjualanDetail(faktur As String) As DataTable
        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = ?", conn)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", faktur)
            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    Private Function GetSatuanBarang(idBarang As String, isPartai As Boolean) As List(Of String)
        Dim satuanList As New List(Of String)()
        Dim query As String = If(isPartai, "SELECT DISTINCT SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG = ?", "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = ?")
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    For i As Integer = 0 To 2
                        Dim satuan As String = rd(i).ToString()
                        If Not String.IsNullOrEmpty(satuan) Then
                            satuanList.Add(satuan)
                        End If
                    Next
                End While
            End Using
        End Using
        Return satuanList
    End Function

    Private Function GetStokBarang(idBarang As String, faktur As String) As Tuple(Of Decimal, Decimal)
        Dim stokToko As Decimal = 0
        Dim stokGudang As Decimal = 0
        Dim qtySatuan As Decimal = 0

        ' Mendapatkan stok toko dan gudang
        Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    stokToko = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                    stokGudang = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
                End If
            End Using
        End Using

        ' Mendapatkan qty_satuan
        Using cmd As New MySqlCommand("SELECT SUM(QTY_SATUAN) AS QTY_SATUAN FROM penjualan_detail WHERE ID_BARANG = ? AND FAKTUR_JUAL = ?", conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", faktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    qtySatuan = If(IsDBNull(rd("QTY_SATUAN")), 0D, ParseDecimal(rd("QTY_SATUAN")))
                End If
            End Using
        End Using

        ' Menyesuaikan stok berdasarkan lokasi barang
        If LblLokasiBarang.Text = "GUDANG" Then
            stokGudang += qtySatuan
        ElseIf LblLokasiBarang.Text = "TOKO" Then
            stokToko += qtySatuan
        End If

        Return Tuple.Create(stokToko, stokGudang)
    End Function


End Class