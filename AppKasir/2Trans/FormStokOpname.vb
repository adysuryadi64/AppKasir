Imports System.Globalization
Imports System.Reflection


Public Class FormStokOpname

    Private TransaksiLampau As String

    Private Sub BarangStokOpnameForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If LblUtama.Text <> "TAMBAH STOK OPNAME" Then
            AmbilDataUntukEdit()
            PanelCariNama.Visible = False
            Label4.Visible = False
            lstBarang.Visible = False
            TxtNyata.Select()
        Else
            TransaksiLampau = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransaksiTanggalLampau.Text)
            TxtLokasi.Text = FormUtama.SLokasi.Text
            TxtNama.Visible = True
            Label4.Visible = True
            Kondisiawaltambah()
            GenerateNomorOpname()
            TxtNama.Select()
        End If

    End Sub

    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtCari_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCariNama.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtCari_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCariNama.BackColor = BackColor
    End Sub

    Public Sub Kondisiawaltambah()
        Panel3.Enabled = True
        TxtKode.Clear()
        TxtKategori.Clear()
        TxtNama.Clear()
        TxtHarga.Clear()
        TxtnamaHasil.Clear()
        TxtStokSystem.Clear()
        TxtNyata.Clear()
        TxtSelisih.Clear()
        TxtTotalRupiah.Clear()
        TxtKeteranganToko.Clear()
        LblSatIsi.Text = ""
        LblSat.Text = ""
        TxtSelisihQty.Clear()
        TxtSelisihRp.Clear()

        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        ' Simpan kontrol yang sedang berfokus
        Dim currentActiveControl As Control = ActiveControl

        ' Cek apakah kontrol yang sedang berfokus adalah TextBox
        If TypeOf currentActiveControl Is TextBox Then
            ' Jika ya, hilangkan fokus dari TextBox
            currentActiveControl.Focus()
        End If
        Tampildata()
    End Sub

    Private Sub GenerateNomorOpname()
        Dim cekTanggal As String = DTPTgl.Value.ToString("yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "SO-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(ID_STOK_OPNAME) FROM Stok_Opname WHERE ID_STOK_OPNAME LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "SO-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "SO-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "SO-" & cekTanggal & "0001"
        End If

        TxtFaktur.Text = UrutKOde
    End Sub

    Private Sub AmbilDataUntukEdit()
        ' Define the query
        Dim query As String = "SELECT TANGGAL, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI, " &
                          "HARGA, STOK_SYSTEM, STOK_NYATA, " &
                          "SATUAN, ISI_SATUAN, " &
                          "KETERANGAN, ID_USER, ID_KOMPUTER " &
                          "FROM Stok_Opname " &
                          "WHERE ID_STOK_OPNAME = @ID_STOK_OPNAME"

        ' Declare variables to hold data
        Dim tanggal As Date = Date.MinValue
        Dim lokasi As String = String.Empty
        Dim idBarang As String = String.Empty
        Dim namaBarang As String = String.Empty
        Dim kategori As String = String.Empty
        Dim harga As Decimal = 0D
        Dim stokSystem As Decimal = 0D
        Dim stokNyata As Decimal = 0D
        Dim satuan As String = String.Empty
        Dim isiSatuan As Integer = 0
        Dim keterangan As String = String.Empty
        Dim idUser As String = String.Empty
        Dim idKomputer As String = String.Empty


        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@ID_STOK_OPNAME", TxtFaktur.Text)

            ' Execute the command and read the data
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Assign values to variables
                    tanggal = If(IsDBNull(reader("TANGGAL")), Date.MinValue, Convert.ToDateTime(reader("TANGGAL")))
                    lokasi = If(IsDBNull(reader("LOKASI")), String.Empty, reader("LOKASI").ToString())
                    idBarang = If(IsDBNull(reader("ID_BARANG")), String.Empty, reader("ID_BARANG").ToString())
                    namaBarang = If(IsDBNull(reader("NAMA_BARANG")), String.Empty, reader("NAMA_BARANG").ToString())
                    kategori = If(IsDBNull(reader("KATEGORI")), String.Empty, reader("KATEGORI").ToString())
                    harga = If(IsDBNull(reader("HARGA")), 0D, Convert.ToDecimal(reader("HARGA")))
                    stokSystem = If(IsDBNull(reader("STOK_SYSTEM")), 0D, Convert.ToDecimal(reader("STOK_SYSTEM")))
                    stokNyata = If(IsDBNull(reader("STOK_NYATA")), 0D, Convert.ToDecimal(reader("STOK_NYATA")))
                    satuan = If(IsDBNull(reader("SATUAN")), String.Empty, reader("SATUAN").ToString())
                    isiSatuan = If(IsDBNull(reader("ISI_SATUAN")), 0, Convert.ToInt32(reader("ISI_SATUAN")))
                    keterangan = If(IsDBNull(reader("KETERANGAN")), String.Empty, reader("KETERANGAN").ToString())
                    idUser = If(IsDBNull(reader("ID_USER")), String.Empty, reader("ID_USER").ToString())
                    idKomputer = If(IsDBNull(reader("ID_KOMPUTER")), String.Empty, reader("ID_KOMPUTER").ToString())

                End If
            End Using
        End Using

        ' Populate the form fields with the retrieved data
        DTPTgl.Value = tanggal
        TxtLokasi.Text = lokasi
        TxtKode.Text = idBarang
        TxtnamaHasil.Text = namaBarang
        TxtKategori.Text = kategori
        TxtHarga.Text = harga
        TxtStokSystem.Text = stokSystem.ToString("N0")
        TxtNyata.Text = stokNyata.ToString("N0")
        LblSat.Text = satuan
        LblSatIsi.Text = isiSatuan
        TxtKeteranganToko.Text = keterangan
        TxtIdUser.Text = idUser
        TxtKomputer.Text = idKomputer

        ' Perform calculations or other operations after populating controls
        HitungNilai()
        Tampildata()
    End Sub



    Private Sub Tampildata()
        DGVData.Columns.Clear()

        Dim tanggalAwal As Date = DTPTgl.Value.Date
        Dim tanggalAkhir As Date = DTPTgl.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT ID_STOK_OPNAME, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI, " &
                              "HARGA, STOK_SYSTEM, STOK_NYATA, " &
                              "STOK_SELISIH, SATUAN, ISI_SATUAN, " &
                              "TOTAL_QTY, TOTAL_HARGA, KETERANGAN " &
                              "FROM Stok_Opname " &
                              "WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir " &
                              "OR ID_USER LIKE @ID_USER " &
                              "ORDER BY ID_STOK_OPNAME ASC"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)

            Using ds As New DataSet
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(ds)
                    DGVData.DataSource = ds.Tables(0)
                End Using
            End Using
        End Using

        With DGVData
            .Columns("ID_STOK_OPNAME").HeaderText = "NOMOR"
            .Columns("ID_BARANG").Visible = False
            .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
            .Columns("KATEGORI").Visible = False
            .Columns("HARGA").Visible = False
            .Columns("STOK_SYSTEM").HeaderText = "STOK SYSTEM"
            .Columns("STOK_NYATA").HeaderText = "STOK NYATA"
            .Columns("SATUAN").HeaderText = "SATUAN"
            .Columns("ISI_SATUAN").Visible = False
            .Columns("STOK_SELISIH").HeaderText = "SELISIH"
            .Columns("TOTAL_QTY").Visible = False
            .Columns("TOTAL_HARGA").HeaderText = "TOTAL HARGA"
            .Columns("KETERANGAN").HeaderText = "KETERANGAN"

            ' Daftar nama kolom yang akan diatur format dan alignment
            Dim columnsToFormat As String() = {
                "STOK_SYSTEM", "STOK_NYATA", "STOK_SELISIH", "TOTAL_HARGA"
            }

            ' Loop melalui kolom dan atur format serta alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Gunakan format kustom untuk menampilkan angka di belakang koma jika bukan 0
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            .Columns("NAMA_BARANG").Frozen = True

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            DataGridViewExtension.EnableDoubleBuffering(DGVData)
        End With

        ' Pengaturan agar DataGridView selalu tampil dengan baris terakhir
        If DGVData.Rows.Count > 0 Then
            DGVData.ClearSelection()
            DGVData.FirstDisplayedScrollingRowIndex = DGVData.Rows.Count - 1
        End If

    End Sub


    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class


    Dim lastKeyTime As DateTime = DateTime.Now
    Dim isBarcodeScan As Boolean = False
    Dim suppressTextChanged As Boolean = False

    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        'Deteksi kecepatan input
        Dim currentTime = DateTime.Now
        Dim elapsedMs = (currentTime - lastKeyTime).TotalMilliseconds
        lastKeyTime = currentTime

        'Deteksi barcode (input cepat + Enter)
        If e.KeyCode = Keys.Enter Then
            isBarcodeScan = (elapsedMs < 50) AndAlso (TxtNama.Text.Length >= 5 OrElse TxtNama.Text.All(AddressOf Char.IsDigit))
            suppressTextChanged = True
            ProsesInput(isBarcodeScan)

            'Logika existing untuk listbox
            If lstBarang.Items.Count = 1 Then
                AmbilDataDariListBox()
            ElseIf lstBarang.Items.Count > 0 Then
                lstBarang.Focus()
                lstBarang.SelectedIndex = 0
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Down AndAlso lstBarang.Visible AndAlso lstBarang.Items.Count > 0 Then
            lstBarang.Focus()
            lstBarang.SelectedIndex = 0
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        If suppressTextChanged Then
            suppressTextChanged = False
            Return
        End If
        ProsesInput(False) 'Manual input
    End Sub

    Private Sub ProsesInput(ByVal isBarcode As Boolean)
        If Not String.IsNullOrEmpty(TxtNama.Text) Then
            Dim inputText As String = TxtNama.Text.Trim()

            ' Cek apakah input kemungkinan barcode (semua karakter angka atau panjang >= 5)
            Dim kemungkinanBarcode As Boolean = inputText.All(AddressOf Char.IsDigit) OrElse inputText.Length >= 5

            ' Hitung jumlah huruf alfabet
            Dim validLetters As String = ""
            For Each c As Char In inputText
                If Char.IsLetter(c) Then
                    validLetters &= c
                End If
            Next

            ' Lanjutkan hanya jika huruf alfabet >= 2 ATAU kemungkinan barcode
            If validLetters.Length >= 2 OrElse kemungkinanBarcode Then

                ' Temukan posisi * pertama
                Dim indexAsterisk As Integer = inputText.IndexOf("*")

                If indexAsterisk >= 0 Then
                    ' Format: qty * nama
                    lstBarang.Items.Clear()

                    Dim angkaSebelumAsterisk As String = inputText.Substring(0, indexAsterisk).Trim()
                    If angkaSebelumAsterisk.Contains(".") OrElse angkaSebelumAsterisk.Contains(",") Then
                        angkaSebelumAsterisk = angkaSebelumAsterisk.Replace(".", ",")
                        TxtQty.Text = angkaSebelumAsterisk
                    ElseIf Decimal.TryParse(angkaSebelumAsterisk, Nothing) Then
                        TxtQty.Text = angkaSebelumAsterisk
                    Else
                        TxtQty.Text = "1"
                    End If

                    Dim searchKeyword As String = inputText.Substring(indexAsterisk + 1).Trim()
                    TampilkanDaftarBarang(searchKeyword)

                Else
                    ' Tidak ada * → proses langsung
                    TampilkanDaftarBarang(inputText)
                    TxtQty.Text = "1"
                End If
            End If
        Else
            lstBarang.Items.Clear()
            lstBarang.Visible = False
            TxtQty.Text = "1"
        End If
    End Sub


    Private Sub TampilkanDaftarBarang(ByVal searchKeyword As String)
        ' Mengambil data dari database
        Dim query As String = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR TRIM(BARCODE_BESAR) LIKE @Nama ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Clear ListBox before adding new items
                lstBarang.Items.Clear()
                TxtBarcode.Clear()

                While rd.Read()
                    Dim itemText As String = rd("NAMA_BARANG").ToString()
                    Select Case TxtLokasi.Text
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
                    lstBarang.Items.Add(itemText)
                End While

                ' Tampilkan ListBox hanya jika lebih dari satu hasil pencarian
                lstBarang.Visible = lstBarang.Items.Count > 0
            End Using
        End Using
    End Sub

    Private Function ParseDecimal(ByVal value As Object) As Decimal
        If value Is Nothing Then Return 0D

        Dim s As String = value.ToString().Trim().Replace(",", ".") ' selalu ubah ke titik
        Dim result As Decimal = 0
        Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result)
        Return result
    End Function


    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles lstBarang.KeyDown
        If e.KeyCode = Keys.Enter AndAlso lstBarang.SelectedItem IsNot Nothing Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub LstBarang_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lstBarang.MouseClick
        If lstBarang.SelectedItem IsNot Nothing Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub AmbilDataDariListBox()
        Dim namayangdiambil As String

        If lstBarang.Items.Count = 1 OrElse (lstBarang.Items.Count > 1 AndAlso lstBarang.SelectedItem IsNot Nothing) Then
            ' Ambil nilai dari item yang dipilih atau item pertama jika hanya satu
            Dim selectedValue As String = If(lstBarang.Items.Count = 1, lstBarang.Items(0).ToString(), lstBarang.SelectedItem.ToString())

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
        ' Mendeklarasikan variabel di luar reader
        Dim idBarang As String = String.Empty
        Dim kategori As String = String.Empty
        Dim hargaBeli As Decimal = 0
        Dim stokToko As Decimal = 0
        Dim stokGudang As Decimal = 0
        Dim satuanStok As String = String.Empty
        Dim satuanIsiStok As Integer = 0

        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, STOK_GUDANG, SATUAN_STOK, SATUAN_ISI_STOK FROM tbl_barang " &
                                  "WHERE (TRIM(ID_BARANG) LIKE @NAMA OR " &
                                  "TRIM(NAMA_BARANG) LIKE @NAMA OR " &
                                  "BARCODE_KECIL LIKE @NAMA OR " &
                                  "BARCODE_SEDANG LIKE @NAMA OR " &
                                  "BARCODE_BESAR LIKE @NAMA)", conn)
            cmd.Parameters.AddWithValue("@NAMA", namayangdiambil)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Mengambil data dari reader dan menyimpannya ke variabel
                    idBarang = If(Not IsDBNull(rd(0)), rd.GetString(0), String.Empty)
                    kategori = If(Not IsDBNull(rd(1)), rd.GetString(1), String.Empty)
                    hargaBeli = If(Not IsDBNull(rd(2)), rd.GetDecimal(2), 0)
                    stokToko = If(Not IsDBNull(rd(3)), rd.GetDecimal(3), 0)
                    stokGudang = If(Not IsDBNull(rd(4)), rd.GetDecimal(4), 0)
                    satuanStok = If(Not IsDBNull(rd(5)), rd.GetString(5), String.Empty)
                    satuanIsiStok = If(Not IsDBNull(rd(6)), rd.GetInt32(6), 0)
                End If
            End Using
        End Using

        ' Masukkan nilai yang telah diambil dari database ke dalam TextBox di luar reader
        TxtnamaHasil.Text = namayangdiambil
        TxtKode.Text = idBarang
        TxtKategori.Text = kategori
        TxtHarga.Text = hargaBeli.ToString("#,0.##")

        Select Case TxtLokasi.Text
            Case "TOKO"
                TxtStokSystem.Text = stokToko.ToString("#,0.##")
            Case "GUDANG"
                TxtStokSystem.Text = stokGudang.ToString("#,0.##")
        End Select

        LblSat.Text = satuanStok
        LblSatIsi.Text = satuanIsiStok.ToString()

        TxtNyata.Text = TxtQty.Text
        HitungNilai()

        ' Fokus pada TxtNyata
        TxtNyata.Focus()
        TxtNama.Clear()
    End Sub


    Private Sub TxtNyata_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TxtNyata.KeyPress
        ' Memastikan hanya angka dan backspace yang dapat dimasukkan
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then
            e.Handled = True
        End If

        ' Jika tombol Enter ditekan
        If e.KeyChar = ChrW(Keys.Enter) Then
            ' Menghentikan bunyi 'ding' default saat menekan Enter
            e.Handled = True

            ' Fokus pada TxtKeteranganToko
            TxtKeteranganToko.Focus()
        End If
    End Sub

    Private Sub TxtKeteranganToko_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TxtKeteranganToko.KeyPress
        ' Jika tombol Enter ditekan
        If e.KeyChar = ChrW(Keys.Enter) Then
            ' Menghentikan bunyi 'ding' default saat menekan Enter
            e.Handled = True

            Simpandata()
        End If
    End Sub

    Private Sub TxtNyata_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNyata.TextChanged
        HitungNilai()
    End Sub

    Private Sub HitungNilai()
        ' Deklarasi variabel
        Dim nyata As Decimal
        Dim stokSystem As Decimal
        Dim satIsi As Integer
        Dim harga As Decimal

        ' Konversi tipe data dengan penanganan kesalahan
        If Decimal.TryParse(TxtNyata.Text, nyata) AndAlso
           Decimal.TryParse(TxtStokSystem.Text, stokSystem) AndAlso
           Integer.TryParse(LblSatIsi.Text, satIsi) AndAlso
           Decimal.TryParse(TxtHarga.Text, harga) Then

            ' Perhitungan dengan tipe data yang sesuai
            TxtSelisih.Text = (nyata - stokSystem).ToString()
            TxtSelisihQty.Text = (nyata - stokSystem) * satIsi
            TxtSelisihRp.Text = harga * (nyata - stokSystem) * satIsi
        End If
    End Sub

    Private Sub TxtSelisihRp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtSelisihRp.TextChanged
        Dim selisihRp As Decimal

        ' Konversi tipe data dengan penanganan kesalahan
        If Decimal.TryParse(TxtSelisihRp.Text, selisihRp) Then
            TxtTotalRupiah.Text = selisihRp.ToString("#,0.##")
        Else
            ' Jika konversi gagal, set nilai ke 0
            TxtTotalRupiah.Text = "0"
        End If
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        Simpandata()
    End Sub


    Private Sub Simpandata()
        If TxtnamaHasil.Text = "" Then
            MessageBox.Show("Silahkan pilih data barang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNama.Select()
            Exit Sub
        End If

        If LblUtama.Text = "TAMBAH STOK OPNAME" AndAlso TransaksiLampau = "Tidak" Then
            DTPTgl.Value = DateTime.Now
            GenerateNomorOpname()
        End If


        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            If LblUtama.Text <> "TAMBAH STOK OPNAME" Then
                Hapusstokopname(transaction)
            End If

            ' Panggil metode-metode Simpan dengan menggunakan transaksi yang sama
            SimpanMasukDetail(transaction)
            UpdateStokBarang(transaction)
            Simpanjurnal(transaction)
            HistoryBarang(transaction)

            ' Commit transaksi
            transaction.Commit()
            DatabaseModule.CatatanAksiHistory("Simpan stok opname " & TxtFaktur.Text)

            HitungByKode(TxtKode.Text)

            If LblUtama.Text = "TAMBAH STOK OPNAME" Then
                Kondisiawaltambah()
                TxtNama.Select()
            Else
                FormUtama.DataStokOpname()
                FormUtama.GBTransaksi.Visible = True
                Close()
            End If


        Catch ex As Exception
            If LblUtama.Text = "Edit Stok Opname" Then
                MessageBox.Show("Ups! Terdapat kendala saat mencoba menyimpan transaksi." & vbCrLf &
                               "Detail kesalahan: " & ex.Message,
                 "Oops! Ada masalah saat edit stok opnam", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show("Ups! Terdapat kendala saat mencoba menyimpan transaksi." & vbCrLf &
                              "Detail kesalahan: " & ex.Message,
                 "Oops! Ada masalah saat menyimpan stok opname", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If


            ' Rollback transaksi secara otomatis karena ada kesalahan
            transaction.Rollback()
        End Try

    End Sub

    Public Sub SimpanMasukDetail(ByVal transaction As MySqlTransaction)
        Dim insertQuery As String = "INSERT INTO Stok_Opname(ID_STOK_OPNAME, TANGGAL, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI, HARGA, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_HARGA, KETERANGAN, ID_USER, ID_KOMPUTER) " &
                                    "VALUES (@ID_STOK_OPNAME, @TANGGAL, @LOKASI, @ID_BARANG, @NAMA_BARANG, @KATEGORI, @HARGA, @STOK_SYSTEM, @STOK_NYATA, @STOK_SELISIH, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_HARGA, @KETERANGAN, @ID_USER, @ID_KOMPUTER)"

        Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
            insertCmd.Parameters.AddWithValue("@ID_STOK_OPNAME", TxtFaktur.Text)
            insertCmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            insertCmd.Parameters.AddWithValue("@LOKASI", TxtLokasi.Text)
            insertCmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)
            insertCmd.Parameters.AddWithValue("@NAMA_BARANG", TxtnamaHasil.Text)
            insertCmd.Parameters.AddWithValue("@KATEGORI", TxtKategori.Text)
            insertCmd.Parameters.AddWithValue("@HARGA", If(String.IsNullOrEmpty(TxtHarga.Text), 0D, Convert.ToDecimal(TxtHarga.Text)))
            insertCmd.Parameters.AddWithValue("@STOK_SYSTEM", If(String.IsNullOrEmpty(TxtStokSystem.Text), 0D, Convert.ToDecimal(TxtStokSystem.Text)))
            insertCmd.Parameters.AddWithValue("@STOK_NYATA", If(String.IsNullOrEmpty(TxtNyata.Text), 0D, Convert.ToDecimal(TxtNyata.Text)))
            insertCmd.Parameters.AddWithValue("@STOK_SELISIH", If(String.IsNullOrEmpty(TxtSelisih.Text), 0D, Convert.ToDecimal(TxtSelisih.Text)))
            insertCmd.Parameters.AddWithValue("@SATUAN", If(String.IsNullOrEmpty(LblSat.Text), String.Empty, LblSat.Text))
            insertCmd.Parameters.AddWithValue("@ISI_SATUAN", If(String.IsNullOrEmpty(LblSatIsi.Text), 0, Convert.ToInt32(LblSatIsi.Text)))
            insertCmd.Parameters.AddWithValue("@TOTAL_QTY", If(String.IsNullOrEmpty(TxtSelisihQty.Text), 0D, Convert.ToDecimal(TxtSelisihQty.Text)))
            insertCmd.Parameters.AddWithValue("@TOTAL_HARGA", If(String.IsNullOrEmpty(TxtSelisihRp.Text), 0D, Convert.ToDecimal(TxtSelisihRp.Text)))

            insertCmd.Parameters.AddWithValue("@KETERANGAN", TxtKeteranganToko.Text)

            insertCmd.Parameters.AddWithValue("@ID_USER", If(LblUtama.Text = "TAMBAH STOK OPNAME", FormUtama.SLogin.Text, TxtIdUser.Text))
            insertCmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblUtama.Text = "TAMBAH STOK OPNAME", FormUtama.Comp.Text, TxtKomputer.Text))
            insertCmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub UpdateStokBarang(ByVal transaction As MySqlTransaction)
        Dim updateQuery As String
        Dim stokField As String = ""

        Select Case TxtLokasi.Text
            Case "TOKO"
                stokField = "OPNAME_TOKO"
            Case "GUDANG"
                stokField = "OPNAME_GUDANG"
        End Select

        updateQuery = "UPDATE tbl_barang SET " & stokField & " = " & stokField & " + ? WHERE ID_BARANG = ?"

        Using cmd As New MySqlCommand(updateQuery, conn, transaction)
            cmd.Parameters.AddWithValue("@STOK_OPNAME", If(String.IsNullOrEmpty(TxtSelisihQty.Text), 0D, Convert.ToDecimal(TxtSelisihQty.Text)))
            cmd.Parameters.AddWithValue("@KODE_BANTU", TxtKode.Text)
            cmd.ExecuteNonQuery()
        End Using

    End Sub


    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        ' Hitung nilai selisih toko
        Dim nilaiSelisih As Decimal = If(String.IsNullOrEmpty(TxtSelisihRp.Text), 0D, Decimal.Parse(TxtSelisihRp.Text))

        If nilaiSelisih <> 0 Then
            Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                          "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@NO_NOTA", TxtKode.Text)
                cmd.Parameters.AddWithValue("@URAIAN", "Stok opnam stok " & TxtLokasi.Text & ", barang " & TxtnamaHasil.Text & " Jumlah Selisih " & Decimal.Parse(TxtSelisihQty.Text))

                ' Tentukan akun berdasarkan nilai selisih
                If nilaiSelisih <= 0 Then
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_K", LAWAN_NAMA_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LAWAN_KODE_REK_BARANG)
                Else
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_D", LAWAN_NAMA_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", LAWAN_KODE_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                End If

                cmd.Parameters.AddWithValue("@NOMINAL", Math.Abs(nilaiSelisih))
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Stok Opnam")
                cmd.Parameters.AddWithValue("@LOKASI", TxtLokasi.Text)

                ' Penentuan IdUser dan IdKomputer berdasarkan status LblUtama
                cmd.Parameters.AddWithValue("@ID_USER", If(LblUtama.Text = "TAMBAH STOK OPNAME", FormUtama.SLogin.Text, TxtIdUser.Text))
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblUtama.Text = "TAMBAH STOK OPNAME", FormUtama.Comp.Text, TxtKomputer.Text))

                cmd.ExecuteNonQuery()
            End Using
        End If

    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
        Using cmd As New MySqlCommand(querySimpan, conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@JENIS", "OPNAME")
            cmd.Parameters.AddWithValue("@LOKASI", TxtLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)
            cmd.Parameters.AddWithValue("@NAMA_BARANG", TxtnamaHasil.Text)
            cmd.Parameters.AddWithValue("@QTY", If(String.IsNullOrEmpty(TxtNyata.Text), 0D, Convert.ToDecimal(TxtNyata.Text)))
            cmd.Parameters.AddWithValue("@SATUAN", If(String.IsNullOrEmpty(LblSat.Text), String.Empty, LblSat.Text))
            cmd.Parameters.AddWithValue("@ISI_SATUAN", If(String.IsNullOrEmpty(LblSatIsi.Text), 0D, Convert.ToDecimal(LblSatIsi.Text)))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", If(String.IsNullOrEmpty(TxtSelisihQty.Text), 0D, Convert.ToDecimal(TxtSelisihQty.Text)))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", If(String.IsNullOrEmpty(TxtSelisihRp.Text), 0D, Convert.ToDecimal(TxtSelisihRp.Text)))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblUtama.Text = "TAMBAH STOK OPNAME", FormUtama.SLogin.Text, TxtIdUser.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblUtama.Text = "TAMBAH STOK OPNAME", FormUtama.Comp.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub Hapusstokopname(ByVal transaction As MySqlTransaction)

        Dim updateQuery As String
        Dim stokField As String = ""

        Select Case TxtLokasi.Text
            Case "TOKO"
                stokField = "OPNAME_TOKO"
            Case "GUDANG"
                stokField = "OPNAME_GUDANG"
        End Select

        updateQuery = "UPDATE tbl_barang SET " & stokField & " = " & stokField & " - ? WHERE ID_BARANG = ?"

        Using cmd As New MySqlCommand(updateQuery, conn, transaction)
            cmd.Parameters.AddWithValue("@STOK_OPNAME", If(String.IsNullOrEmpty(TxtQtyUntukEdit.Text), 0D, Convert.ToDecimal(TxtQtyUntukEdit.Text)))
            cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)
            cmd.ExecuteNonQuery()
        End Using
        HitungStokPerubahan(TxtKode.Text, transaction)

        Dim deleteQueries As String() = {
                  "DELETE FROM Stok_Opname WHERE ID_STOK_OPNAME = @ID_STOK_OPNAME",
                  "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_STOK_OPNAME",
                  "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_STOK_OPNAME"
              }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@ID_STOK_OPNAME", TxtFaktur.Text)
                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub


    Private Sub BarangStokOpnameForm_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                Simpandata()
            Case Keys.Escape
                FormUtama.DataStokOpname()
                FormUtama.GBTransaksi.Visible = True
                Close()
        End Select
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluar.Click
        FormUtama.DataStokOpname()
        FormUtama.GBTransaksi.Visible = True
        Close()
    End Sub

    Private Sub TxtKode_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtKode.TextChanged
        Dim query As String = "SELECT TANGGAL, LOKASI, STOK_SYSTEM, STOK_NYATA, TOTAL_QTY, TOTAL_HARGA " &
                          "FROM Stok_Opname WHERE ID_BARANG = @ID_BARANG ORDER BY TANGGAL DESC LIMIT 1"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text.Trim())

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim tanggal As String = If(IsDBNull(rd("TANGGAL")), "Tidak ada data", Convert.ToDateTime(rd("TANGGAL")).ToString("dd-MM-yyyy"))
                    Dim stokSystem As Decimal = If(IsDBNull(rd("STOK_SYSTEM")), 0D, Convert.ToDecimal(rd("STOK_SYSTEM")))
                    Dim stokNyata As Decimal = If(IsDBNull(rd("STOK_NYATA")), 0D, Convert.ToDecimal(rd("STOK_NYATA")))
                    Dim totalQty As Decimal = If(IsDBNull(rd("TOTAL_QTY")), 0D, Convert.ToDecimal(rd("TOTAL_QTY")))
                    Dim totalHarga As Decimal = If(IsDBNull(rd("TOTAL_HARGA")), 0D, Convert.ToDecimal(rd("TOTAL_HARGA")))

                    ' Update labels with the retrieved data
                    LblKetTerakhir.Text = "Nama barang: " & TxtnamaHasil.Text & " | Tanggal: " & tanggal
                    LblToko.Text = "Stok Sistem: " & stokSystem.ToString("N0") & " | Stok Nyata: " & stokNyata.ToString("N0") &
                               " | Selisih: " & totalQty.ToString("N0") & " | Nominal: " & totalHarga.ToString("N0")
                Else
                    ' Handle no data case
                    LblKetTerakhir.Text = "Nama barang: " & TxtnamaHasil.Text & " | Tanggal: Belum ada data"
                    LblToko.Text = "Belum ada data"
                End If
            End Using
        End Using
    End Sub



    Private Sub BtNCetak_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtNCetak.Click
        FormStokOpnameBahan.ShowDialog()
    End Sub
End Class