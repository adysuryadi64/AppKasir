Public Class FormStokOpname

    Private Sub BarangStokOpnameForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

        If LblHeader.Text <> "TAMBAH STOK OPNAME" Then
            AmbilDataUntukEdit()
            PanelCari.Visible = False
            Label4.Visible = False
            lstBarang.Visible = False
            TxtNyata.Select()
        Else
            ' Setting dibaca langsung dari ModulHakAkses property
            TxtLokasi.Text = FormUtama.StatusLokasi.Text
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
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtCari_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_Panel, ModuleTheme.D_Panel)
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
        ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTgl)

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
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "SO")
            cmd.Parameters.AddWithValue("@tgl", DTPTgl.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "stok_opname")
            cmd.Parameters.AddWithValue("@kolom", "ID_STOK_OPNAME")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            TxtFaktur.Text = pNomor.Value?.ToString()
        End Using
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
        DTPTgl.Enabled = True  ' Mode edit: selalu bisa ubah tanggal, tanggal lama bisa lampau
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
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)

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
            ModuleAngka.TerapkanFormatKolomAngka(DGVData, columnsToFormat)

            .Columns("NAMA_BARANG").Frozen = True

            .EnableHeadersVisualStyles = False

            ' Set alternating row style

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle

            ' Enable double buffering to reduce flickering
            ModuleTheme.ApplyThemeDataGridView(DGVData)
        End With

        ' Pengaturan agar DataGridView selalu tampil dengan baris terakhir
        If DGVData.Rows.Count > 0 Then
            DGVData.ClearSelection()
            DGVData.FirstDisplayedScrollingRowIndex = DGVData.Rows.Count - 1
        End If

    End Sub


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
        Dim query As String = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE STATUS = 'Aktif' AND (TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR TRIM(BARCODE_BESAR) LIKE @Nama) ORDER BY NAMA_BARANG"

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
                            Dim stokToko As Decimal = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                            itemText &= " => " & stokToko.ToString("N0") ' Format stok dengan dua desimal
                        Case "GUDANG"
                            ' Tambahkan stok gudang setelah nama barang
                            Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
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

    ' ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal


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
                    idBarang = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                    kategori = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_KATEGORI", String.Empty)
                    hargaBeli = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    stokToko = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    stokGudang = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                    satuanStok = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_STOK", String.Empty)
                    satuanIsiStok = ModuleAngka.SafeGetValue(Of Integer)(rd, "SATUAN_ISI_STOK", 0)
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

        ' Konversi tipe data — pakai ParseDecimal agar aman dengan format ribuan (1.000 atau 1,000)
        nyata = ModuleAngka.ParseDecimal(TxtNyata.Text)
        harga = ModuleAngka.ParseDecimal(TxtHarga.Text)

        stokSystem = ModuleAngka.ParseDecimal(TxtStokSystem.Text)
        If Integer.TryParse(LblSatIsi.Text, satIsi) Then

            ' Perhitungan selisih
            TxtSelisih.Text = (nyata - stokSystem).ToString()
            TxtSelisihQty.Text = (nyata - stokSystem) * satIsi
            TxtSelisihRp.Text = harga * (nyata - stokSystem) * satIsi
        End If
    End Sub

    Private Sub TxtSelisihRp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtSelisihRp.TextChanged
        Dim selisihRp As Decimal = ModuleAngka.ParseDecimal(TxtSelisihRp.Text)
        TxtTotalRupiah.Text = ModuleAngka.FormatRupiah(selisihRp)
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

        If LblHeader.Text = "TAMBAH STOK OPNAME" AndAlso Not ModulHakAkses.SettingIzinkanTanggalLampau Then
            ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTgl)
            GenerateNomorOpname()
        End If


        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            If LblHeader.Text <> "TAMBAH STOK OPNAME" Then
                Dim noOpname As String = TxtFaktur.Text

                ' ========================================
                ' START: Audit Trail - Edit Stok Opname
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Try
                    Using snapCmd As New MySqlCommand(
                        "SELECT ID_STOK_OPNAME, TANGGAL, LOKASI, ID_BARANG, NAMA_BARANG, " &
                        "STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, TOTAL_QTY, TOTAL_HARGA, KETERANGAN " &
                        "FROM Stok_Opname WHERE ID_STOK_OPNAME = @id LIMIT 1", conn, transaction)
                        snapCmd.Parameters.AddWithValue("@id", noOpname)
                        Using snapRd As MySqlDataReader = snapCmd.ExecuteReader()
                            If snapRd.Read() Then
                                sbSnapshot.AppendLine($"ID Opname: {snapRd("ID_STOK_OPNAME")}")
                                sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(snapRd("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                sbSnapshot.AppendLine($"Lokasi: {snapRd("LOKASI")}")
                                sbSnapshot.AppendLine($"Kode Barang: {snapRd("ID_BARANG")}")
                                sbSnapshot.AppendLine($"Nama Barang: {snapRd("NAMA_BARANG")}")
                                sbSnapshot.AppendLine($"Stok Sistem: {ModuleAngka.ParseDecimal(snapRd("STOK_SYSTEM"))} unit")
                                sbSnapshot.AppendLine($"Stok Nyata: {ModuleAngka.ParseDecimal(snapRd("STOK_NYATA"))} unit")
                                sbSnapshot.AppendLine($"Selisih Stok: {ModuleAngka.ParseDecimal(snapRd("STOK_SELISIH"))} unit")
                                sbSnapshot.AppendLine($"Total Qty: {ModuleAngka.ParseDecimal(snapRd("TOTAL_QTY"))} unit")
                                sbSnapshot.AppendLine($"Total Harga: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(snapRd("TOTAL_HARGA")))}")
                                sbSnapshot.AppendLine($"Keterangan: {snapRd("KETERANGAN")}")
                            End If
                        End Using
                    End Using
                Catch
                    sbSnapshot.AppendLine("Gagal baca data sebelum edit")
                End Try
                ModuleAuditTrail.CatatAuditMaster("OPN:" & noOpname, "EDIT", "Stok Opname", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Stok Opname
                ' ========================================

                Hapusstokopname(transaction)
            End If

            ' Audit: 1 barang — A=C=qty dari form, B=qty di HistoryBarang, D=delta stok
            Dim qtyOpname As Decimal = ModuleAngka.ParseDecimal(TxtSelisihQty.Text)
            Dim auditDGV As New Dictionary(Of String, Decimal)() From {{TxtKode.Text, qtyOpname}}
            Dim auditDetail As New Dictionary(Of String, Decimal)() From {{TxtKode.Text, qtyOpname}}
            Dim auditHistory As New Dictionary(Of String, Decimal)() From {{TxtKode.Text, qtyOpname}}

            SimpanMasukDetail(transaction)
            UpdateStokBarang(transaction)
            Simpanjurnal(transaction)
            HistoryBarang(transaction)

            Dim nilaiSelisihOpname As Decimal = ModuleAngka.ParseDecimal(TxtSelisihRp.Text)

            ' Recalculate stok + Audit D
            Dim stokSebelum As Decimal = BacaStokSaatIni(TxtKode.Text, TxtLokasi.Text, transaction)
            HitungStokPerubahan(TxtKode.Text, transaction)
            Dim stokSesudah As Decimal = BacaStokSaatIni(TxtKode.Text, TxtLokasi.Text, transaction)
            Dim auditStokDelta As New Dictionary(Of String, Decimal)() From {{TxtKode.Text, Math.Abs(stokSesudah - stokSebelum)}}

            AuditStokTransaksi(TxtFaktur.Text, "Stok Opname", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)

            ' ========================================
            ' STEP 3: UPDATE saldo akun — incremental delta
            ' ========================================
            UpdateSaldoAkunDeltaDariFaktur(TxtFaktur.Text, transaction)

            ' Commit transaksi
            transaction.Commit()

            If nilaiSelisihOpname <> 0 Then
                CatatJurnalTidakSeimbang(TxtFaktur.Text, Math.Abs(nilaiSelisihOpname), Math.Abs(nilaiSelisihOpname), "Stok Opname",
                    {"SelisihOpname"})
            End If

            If LblHeader.Text = "TAMBAH STOK OPNAME" Then
                Kondisiawaltambah()
                TxtNama.Select()
            Else
                FormUtama.DataStokOpname()
                FormUtama.GBTransaksi.Visible = True
                Close()
            End If


        Catch ex As Exception
            If LblHeader.Text = "Edit Stok Opname" Then
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
        Dim insertQuery As String = "INSERT INTO Stok_Opname(ID_STOK_OPNAME, TANGGAL, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI, HARGA, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_HARGA, KETERANGAN, ID_USER, ID_KOMPUTER, URUTAN) " &
                                    "VALUES (@ID_STOK_OPNAME, @TANGGAL, @LOKASI, @ID_BARANG, @NAMA_BARANG, @KATEGORI, @HARGA, @STOK_SYSTEM, @STOK_NYATA, @STOK_SELISIH, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_HARGA, @KETERANGAN, @ID_USER, @ID_KOMPUTER, @URUTAN)"

        Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
            insertCmd.Parameters.AddWithValue("@ID_STOK_OPNAME", TxtFaktur.Text)
            insertCmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            insertCmd.Parameters.AddWithValue("@LOKASI", TxtLokasi.Text)
            insertCmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)
            insertCmd.Parameters.AddWithValue("@NAMA_BARANG", TxtnamaHasil.Text)
            insertCmd.Parameters.AddWithValue("@KATEGORI", TxtKategori.Text)
            insertCmd.Parameters.AddWithValue("@HARGA", ModuleAngka.ParseDecimal(TxtHarga.Text))
            insertCmd.Parameters.AddWithValue("@STOK_SYSTEM", ModuleAngka.ParseDecimal(TxtStokSystem.Text))
            insertCmd.Parameters.AddWithValue("@STOK_NYATA", ModuleAngka.ParseDecimal(TxtNyata.Text))
            insertCmd.Parameters.AddWithValue("@STOK_SELISIH", ModuleAngka.ParseDecimal(TxtSelisih.Text))
            insertCmd.Parameters.AddWithValue("@SATUAN", If(String.IsNullOrEmpty(LblSat.Text), String.Empty, LblSat.Text))
            insertCmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseInteger(LblSatIsi.Text))
            insertCmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtSelisihQty.Text))
            insertCmd.Parameters.AddWithValue("@TOTAL_HARGA", ModuleAngka.ParseDecimal(TxtSelisihRp.Text))

            insertCmd.Parameters.AddWithValue("@KETERANGAN", TxtKeteranganToko.Text)

            insertCmd.Parameters.AddWithValue("@ID_USER", If(LblHeader.Text = "TAMBAH STOK OPNAME", FormUtama.StatusNamaUser.Text, TxtIdUser.Text))
            insertCmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblHeader.Text = "TAMBAH STOK OPNAME", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            insertCmd.Parameters.AddWithValue("@URUTAN", 1)
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
            cmd.Parameters.AddWithValue("@STOK_OPNAME", ModuleAngka.ParseDecimal(TxtSelisihQty.Text))
            cmd.Parameters.AddWithValue("@KODE_BANTU", TxtKode.Text)
            cmd.ExecuteNonQuery()
        End Using

    End Sub


    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        ' Hitung nilai selisih toko
        Dim nilaiSelisih As Decimal = ModuleAngka.ParseDecimal(TxtSelisihRp.Text)

        ' Akun penyesuaian stok sesuai COA: 06.04.001 PENYESUAIAN STOK MINUS
        Const KODE_PENYESUAIAN As String = "06.04.001"
        Const NAMA_PENYESUAIAN As String = "PENYESUAIAN STOK MINUS"

        If nilaiSelisih <> 0 Then
            Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                          "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@NO_NOTA", TxtKode.Text)
                cmd.Parameters.AddWithValue("@URAIAN", "Stok opnam stok " & TxtLokasi.Text & ", barang " & TxtnamaHasil.Text & " Jumlah Selisih " & ModuleAngka.ParseDecimal(TxtSelisihQty.Text))

                ' Tentukan akun berdasarkan nilai selisih
                ' Stok minus (kurang): D PENYESUAIAN STOK MINUS, K PERSEDIAAN BARANG
                ' Stok plus  (lebih) : D PERSEDIAAN BARANG,      K PENYESUAIAN STOK MINUS
                If nilaiSelisih < 0 Then
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_PENYESUAIAN)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_PENYESUAIAN)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                Else
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_PENYESUAIAN)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_PENYESUAIAN)
                End If

                cmd.Parameters.AddWithValue("@NOMINAL", Math.Abs(nilaiSelisih))
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "STOK OPNAME")
                cmd.Parameters.AddWithValue("@LOKASI", TxtLokasi.Text)

                ' Penentuan IdUser dan IdKomputer berdasarkan status LblUtama
                cmd.Parameters.AddWithValue("@ID_USER", If(LblHeader.Text = "TAMBAH STOK OPNAME", FormUtama.StatusNamaUser.Text, TxtIdUser.Text))
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblHeader.Text = "TAMBAH STOK OPNAME", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))

                cmd.ExecuteNonQuery()
            End Using

            ' Debug jurnal stok opname
            Debug.WriteLine("═══════════════════════════════════════════════════════")
            Debug.WriteLine("DEBUG JURNAL STOK OPNAME - Faktur: " & TxtFaktur.Text & " | " & TxtnamaHasil.Text)
            Debug.WriteLine("Selisih Rp: " & nilaiSelisih.ToString("N0") & " | Abs: " & Math.Abs(nilaiSelisih).ToString("N0"))
            Debug.WriteLine("═══════════════════════════════════════════════════════")
            If nilaiSelisih < 0 Then
                Debug.WriteLine(String.Format("{0,-4} {1,-20} {2,-30} {3,-30} {4,12:N0} {5,12:N0}", "J1", "Selisih Kurang", NAMA_PENYESUAIAN & " [" & KODE_PENYESUAIAN & "]", NAMA_REK_BARANG, Math.Abs(nilaiSelisih), Math.Abs(nilaiSelisih)))
            Else
                Debug.WriteLine(String.Format("{0,-4} {1,-20} {2,-30} {3,-30} {4,12:N0} {5,12:N0}", "J1", "Selisih Lebih", NAMA_REK_BARANG, NAMA_PENYESUAIAN & " [" & KODE_PENYESUAIAN & "]", Math.Abs(nilaiSelisih), Math.Abs(nilaiSelisih)))
            End If
            Debug.WriteLine("✅ JURNAL SEIMBANG - D=K=" & Math.Abs(nilaiSelisih).ToString("N0"))
            Debug.WriteLine("═══════════════════════════════════════════════════════")
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
            cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(TxtNyata.Text))
            cmd.Parameters.AddWithValue("@SATUAN", If(String.IsNullOrEmpty(LblSat.Text), String.Empty, LblSat.Text))
            cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(LblSatIsi.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtSelisihQty.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtSelisihRp.Text))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblHeader.Text = "TAMBAH STOK OPNAME", FormUtama.StatusNamaUser.Text, TxtIdUser.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblHeader.Text = "TAMBAH STOK OPNAME", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub Hapusstokopname(ByVal transaction As MySqlTransaction)
        ' Wrapper ke ModuleHapusTransaksi.HapusStokOpname — logika ada di modul.
        ' Label "[HAPUS-EDIT]" membedakan dari hapus permanen di FormUtama.
        ModuleHapusTransaksi.HapusStokOpname(
            TxtFaktur.Text,
            TxtKode.Text,
            ModuleAngka.ParseDecimal(TxtQtyUntukEdit.Text),
            TxtLokasi.Text,
            TxtFaktur.Text & " [HAPUS-EDIT]",
            transaction)
    End Sub


    Private Sub BarangStokOpnameForm_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                Simpandata()
            Case Keys.Escape
                FormUtama.DataStokOpname()
                FormUtama.GBTransaksi.Visible = True
                Close()
        End Select
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluarForm.Click
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
                    Dim tanggal As String = ModuleAngka.SafeGetValue(Of String)(rd, "TANGGAL", "Tidak ada data")
                    If tanggal <> "Tidak ada data" Then tanggal = Convert.ToDateTime(tanggal).ToString("dd-MM-yyyy")
                    Dim stokSystem As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_SYSTEM", 0D)
                    Dim stokNyata As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_NYATA", 0D)
                    Dim totalQty As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TOTAL_QTY", 0D)
                    Dim totalHarga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TOTAL_HARGA", 0D)

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
        With FormStokOpnameBahan
            .BringToFront()
            .ShowDialog()
        End With
    End Sub

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F8      : Simpan stok opname" & vbCrLf &
                           "ESC     : Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
