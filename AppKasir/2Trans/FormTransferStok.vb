Public Class FormTransferStok

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit TransferStok ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("TRF-STK:" & noTransfer, "HAPUS"/"EDIT",
    '       "Transfer Stok", snapshotJson, "[MENENGAH] Hapus/Edit transfer stok", trans)
    ' Snapshot minimal: id_transfer, tanggal, lokasi_asal, lokasi_tujuan, total_qty
    ' ─────────────────────────────────────────────────────────────────────────

    Private Sub FormGanti_Barang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Setting dibaca langsung dari ModulHakAkses property
        LblLokasi.Text = FormUtama.StatusLokasi.Text
        Kondisiawal()
    End Sub


    Private Sub LakukanCetakTransferStok(idTransfer As String)
        If BacaPengaturanPrinter("TransferStok", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterTransferStok.TanyaPilihPrinterTransferStok(idTransfer)
        Else
            ModulePrinterTransferStok.CetakTransferStok(idTransfer)
        End If
    End Sub

    Private Sub LblLokasi_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblLokasi.TextChanged
        Select Case LblLokasi.Text
            Case "TOKO"
                Panel4.Visible = False
                Panel5.Visible = False
                Panel3.Visible = True
                Panel6.Visible = True
            Case "GUDANG"
                Panel4.Visible = True
                Panel5.Visible = True
                Panel3.Visible = False
                Panel6.Visible = False
        End Select
    End Sub

    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtCariMsk_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCariMsk.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCariMasuk.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan

    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtCariMsk_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCariMsk.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCariMasuk.BackColor = Color.White
    End Sub

    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtCariKlr_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCariKlr.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCariKeluar.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan

    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtCariKlr_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCariKlr.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCariKeluar.BackColor = Color.White
    End Sub

    Private Sub Kondisiawal()
        ' Clear TextBoxes
        TxtHargaKlr.Clear()
        TxtHargaMsk.Clear()
        TxtIsiKlr.Clear()
        TxtIsiMsk.Clear()
        TxtKodeKlr.Clear()
        TxtKodeMsk.Clear()
        TxtNamaKlr.Clear()
        TxtCariMsk.Clear()
        TxtCariKlr.Clear()
        TxtNamaMsk.Clear()
        TxtTotalHargaKlr.Clear()
        TxtQtyKlr.Clear()
        TxtQtyMsk.Clear()

        ' Clear ComboBoxes
        CmbSatuanKlr.Items.Clear()
        CmbSatuanMsk.Items.Clear()

        ' Clear Labels
        LblStokTokoKlr.Text = ""
        LblStokgudangKlr.Text = ""
        LblStokTokoMsk.Text = ""
        LblStokgudangMsk.Text = ""
        LblTotalhargaMsk.Text = "Rp. 0"
        LblTotalhargaKlr.Text = "Rp. 0"
        LblSatTokoMsk.Text = ""
        LblSatGudangMsk.Text = ""
        LblSatTokoKlr.Text = ""
        LblSatGudangKlr.Text = ""

        ' Other initializations
        BtnBayar.Visible = True
        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        GenerateNomorTransferstok()

        TxtCariKlr.Select()
    End Sub

    Public Sub TampilSatuan()
        Using cmd As New MySqlCommand("SELECT nama FROM tbl_satuan order by nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbSatuanKlr.Items.Clear()
                CmbSatuanMsk.Items.Clear()
                Do While rd.Read()
                    CmbSatuanKlr.Items.Add(rd.Item("nama"))
                    CmbSatuanMsk.Items.Add(rd.Item("nama"))
                Loop
            End Using
        End Using
    End Sub

    Private Sub GenerateNomorTransferstok()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "TS")
            cmd.Parameters.AddWithValue("@tgl", DtpTanggal.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "transfer_stok")
            cmd.Parameters.AddWithValue("@kolom", "ID_TRANSFER")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblIdTransaksi.Text = pNomor.Value?.ToString()
        End Using
    End Sub


    Private Sub TxtCariMsk_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCariMsk.TextChanged
        If IsValidInputMsk(TxtCariMsk.Text) Then
            AmbilNamaDariTxtNamaMsk()
        End If
    End Sub

    Private Function IsValidInputMsk(ByVal input As String) As Boolean
        If Not String.IsNullOrEmpty(input) Then
            Dim indexAsterisk As Integer = input.IndexOf("*")

            If indexAsterisk >= 0 Then
                LstBarangMsk.Items.Clear()
                ' Jika tanda '*' ditemukan
                If indexAsterisk + 2 <= input.Length Then
                    ' Jika terdapat minimal 2 karakter setelah '*'
                    Return True
                End If
            ElseIf input.Length >= 2 Then
                ' Jika tidak ditemukan karakter '*' dan panjang input minimal 2 karakter
                Return True
            End If
        End If

        ' Input kosong atau tidak memenuhi syarat
        Return False
    End Function


    Private Sub TxtCariMsk_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtCariMsk.KeyDown
        If e.KeyCode = Keys.Enter AndAlso LstBarangMsk.Visible AndAlso LstBarangMsk.SelectedItem IsNot Nothing Then
            AmbilNamaDariTxtNamaMsk()
        ElseIf e.KeyCode = Keys.Down AndAlso LstBarangMsk.Visible Then
            ' Jika panah bawah ditekan dan ada item di ListBox, pilih item pertama
            LstBarangMsk.Focus()
            LstBarangMsk.SelectedIndex = 0
            e.SuppressKeyPress = True  ' Hentikan kejadian keypress agar tidak memicu kejadian TxtNama_KeyDown lagi
        End If
    End Sub

    Private Sub AmbilNamaDariTxtNamaMsk()
        If Not String.IsNullOrEmpty(TxtCariMsk.Text) Then
            ' Mencari tanda '*' dalam teks
            Dim indexAsterisk As Integer = TxtCariMsk.Text.IndexOf("*")

            ' Set TxtQty.Text ke nilai yang ditemukan sebelum tanda '*'
            TxtQtyMsk.Text = If(indexAsterisk >= 0 AndAlso Integer.TryParse(TxtCariMsk.Text.Substring(0, indexAsterisk).Trim(), Nothing), TxtCariMsk.Text.Substring(0, indexAsterisk).Trim(), "1")

            ' Ambil teks setelah '*' untuk pencarian
            Dim searchKeyword As String = If(indexAsterisk >= 0, TxtCariMsk.Text.Substring(indexAsterisk + 1).Trim(), TxtCariMsk.Text.Trim())
            TampilkanDaftarBarangMsk(searchKeyword)
        Else
            BersihkanPencarianMsk()
        End If
    End Sub

    Private Sub TampilkanDaftarBarangMsk(ByVal searchKeyword As String)
        ' Mengambil data dari database
        Dim query As String = "SELECT NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @Nama OR NAMA_BARANG LIKE @Nama OR BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama OR BARCODE_BESAR LIKE @Nama) ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Clear ListBox before adding new items
                LstBarangMsk.Items.Clear()
                TxtBarcodeMsk.Clear()

                While rd.Read()
                    ' Tambahkan nama barang ke dalam ListBox
                    LstBarangMsk.Items.Add(rd("NAMA_BARANG").ToString())

                    ' Check if the searchKeyword matches any barcode field
                    If searchKeyword = rd("BARCODE_SEDANG").ToString() Or searchKeyword = rd("BARCODE_BESAR").ToString() Then
                        ' Set TxtBarcode.Text to the matched barcode value
                        TxtBarcodeMsk.Text = searchKeyword
                    End If
                End While

                ' Tampilkan ListBox hanya jika lebih dari satu hasil pencarian
                LstBarangMsk.Visible = LstBarangMsk.Items.Count > 0

                ' jika listbox hanya satu hasil pencarian langsung panggil
                If LstBarangMsk.Items.Count = 1 Then
                    AmbilDataDariListBoxMsk()
                End If
            End Using
        End Using
    End Sub


    Private Sub BersihkanPencarianMsk()
        ' Jika TxtNama kosong, bersihkan ListBox dan kosongkan TxtQty.Text
        LstBarangMsk.Items.Clear()
        LstBarangMsk.Visible = False ' Sembunyikan ListBox jika TxtNama kosong
    End Sub


    Private Sub AmbilDataDariListBoxMsk()
        If LstBarangMsk.SelectedItem IsNot Nothing Then
            Dim namaYangDiambil As String = GetTextAfterAsteriskMsk(LstBarangMsk.SelectedItem.ToString())
            IsisatuanmasukMsk(namaYangDiambil)
            AmbildatalaindaridbbarangMsk(namaYangDiambil)
            MasukkandatabarangMsk()
        End If
    End Sub

    Private Sub LstBarangMsk_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarangMsk.KeyDown
        If e.KeyCode = Keys.Enter Then
            AmbilDataDariListBoxMsk()
        End If
    End Sub

    Private Sub LstBarangMsk_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LstBarangMsk.MouseClick
        AmbilDataDariListBoxMsk()
    End Sub

    Private Function GetTextAfterAsteriskMsk(ByVal selectedValue As String) As String
        Dim indexAsterisk As Integer = selectedValue.IndexOf("*")

        If indexAsterisk >= 0 Then
            Return selectedValue.Substring(indexAsterisk + 1).Trim()
        Else
            Return selectedValue
        End If
    End Function

    Private Sub IsisatuanmasukMsk(ByVal namayangdiambil As String)
        ' Deklarasikan variabel di luar reader
        Dim satuanKecil As String = ""
        Dim satuanSedang As String = ""
        Dim satuanBesar As String = ""

        ' Lakukan query ke tabel "tbl_barang" untuk mendapatkan data terpilih berdasarkan input dari TxtNamaMasuk
        Dim sqlQuery As String = "SELECT DISTINCT SATUAN_UMUM_KECIL,SATUAN_UMUM_SEDANG,SATUAN_UMUM_BESAR FROM tbl_barang WHERE NAMA_BARANG LIKE @selectedData"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@selectedData", namayangdiambil)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Isi variabel dengan nilai yang sesuai dari reader
                    satuanKecil = If(Not reader.IsDBNull(reader.GetOrdinal("SATUAN_UMUM_KECIL")), reader.GetString(reader.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                    satuanSedang = If(Not reader.IsDBNull(reader.GetOrdinal("SATUAN_UMUM_SEDANG")), reader.GetString(reader.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                    satuanBesar = If(Not reader.IsDBNull(reader.GetOrdinal("SATUAN_UMUM_BESAR")), reader.GetString(reader.GetOrdinal("SATUAN_UMUM_BESAR")), "")
                End If
            End Using
        End Using
        ' Clear ComboBox items before adding new ones
        CmbSatuanMsk.Items.Clear()

        ' Tambahkan item ke ComboBox jika tidak kosong
        If Not String.IsNullOrEmpty(satuanKecil) Then
            CmbSatuanMsk.Items.Add(satuanKecil)
        End If

        If Not String.IsNullOrEmpty(satuanSedang) Then
            CmbSatuanMsk.Items.Add(satuanSedang)
        End If

        If Not String.IsNullOrEmpty(satuanBesar) Then
            CmbSatuanMsk.Items.Add(satuanBesar)
        End If

        ' Set text ComboBox dengan nilai satuan kecil
        CmbSatuanMsk.Text = satuanKecil
    End Sub


    Private idBarangResultMsk As String = ""
    Private namaBarangResultMsk As String = ""
    Private hargaBarangResultMsk As Decimal = 0D
    Private satuanBarangResultMsk As String = ""
    Private isiBarangResultMsk As Integer = 0
    Private stokTokoResultMsk As Decimal = 0D
    Private satuanStokTokoResultMsk As String = ""
    Private stokGudangResultMsk As Decimal = 0D
    Private satuanStokGudangResultMsk As String = ""

    Private Sub AmbildatalaindaridbbarangMsk(ByVal namayangdiambil As String)
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                   "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                   "STOK_TOKO, SATUAN_STOK, STOK_GUDANG, SATUAN_STOK " &
                   "FROM tbl_barang " &
                   "WHERE NAMA_BARANG LIKE @NamaBarang OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", namayangdiambil)

            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    idBarangResultMsk = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                    namaBarangResultMsk = namayangdiambil
                    hargaBarangResultMsk = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    satuanBarangResultMsk = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", Nothing)
                    isiBarangResultMsk = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)

                    If Not String.IsNullOrEmpty(TxtBarcodeMsk.Text) Then
                        Select Case TxtBarcodeMsk.Text
                            Case rd("BARCODE_SEDANG").ToString()
                                satuanBarangResultMsk = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", Nothing)
                                isiBarangResultMsk = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 0)
                            Case rd("BARCODE_BESAR").ToString()
                                satuanBarangResultMsk = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", Nothing)
                                isiBarangResultMsk = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 0)
                        End Select
                    End If

                    ' Memastikan isiSatuan tidak bisa 0
                    If isiBarangResultMsk = 0 Then
                        isiBarangResultMsk = 1
                    End If

                    stokTokoResultMsk = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    satuanStokTokoResultMsk = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_STOK", String.Empty)
                    stokGudangResultMsk = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    satuanStokGudangResultMsk = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_STOK", String.Empty)
                    BersihkanPencarianMsk()
                    TxtCariMsk.Clear()
                End If
            End Using
        End Using
    End Sub

    Private Sub MasukkandatabarangMsk()
        TxtKodeMsk.Text = idBarangResultMsk
        TxtNamaMsk.Text = namaBarangResultMsk
        TxtHargaMsk.Text = hargaBarangResultMsk.ToString()
        CmbSatuanMsk.SelectedItem = satuanBarangResultMsk
        TxtIsiMsk.Text = isiBarangResultMsk.ToString()
        LblStokTokoMsk.Text = stokTokoResultMsk.ToString("N0")
        LblSatTokoMsk.Text = satuanStokTokoResultMsk
        LblStokgudangMsk.Text = stokGudangResultMsk.ToString("N0")
        LblSatGudangMsk.Text = satuanStokGudangResultMsk

        UpdateTotalHargaMsk()
        TxtQtyMsk.Select()

    End Sub

    Private Sub CmbSatuanMsk_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSatuanMsk.SelectedIndexChanged
        Dim sqlQuery As String = "SELECT ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ID_BARANG"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", TxtKodeMsk.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    If CmbSatuanMsk.SelectedIndex = 1 Then
                        TxtIsiMsk.Text = reader("ISI_UMUM_SEDANG").ToString()
                    ElseIf CmbSatuanMsk.SelectedIndex = 2 Then
                        TxtIsiMsk.Text = reader("ISI_UMUM_BESAR").ToString()
                    Else
                        TxtIsiMsk.Text = reader("ISI_UMUM_KECIL").ToString()
                    End If

                    If String.IsNullOrEmpty(TxtIsiMsk.Text) Or TxtIsiMsk.Text = "0" Then
                        TxtIsiMsk.Text = "1"
                    End If

                    UpdateTotalHargaMsk()
                End If
            End Using
        End Using
    End Sub

    Private Sub TxtQtyMsk_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtQtyMsk.TextChanged
        UpdateTotalHargaMsk()
    End Sub

    Private Sub TxtIsiMsk_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtIsiMsk.TextChanged
        UpdateTotalHargaMsk()
    End Sub

    Private Sub UpdateTotalHargaMsk()
        ' Convert the text values to numeric types, assuming 0 if they are empty
        Dim qtyMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtQtyMsk.Text)
        Dim isiMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtIsiMsk.Text)
        Dim hargaMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtHargaMsk.Text)

        ' Check if all relevant fields are not empty or equal to zero
        If qtyMasukValue <> 0 AndAlso isiMasukValue <> 0 AndAlso hargaMasukValue <> 0 Then
            ' Calculate the new value for TxtQtySatMasuk
            Dim result As Decimal = qtyMasukValue * isiMasukValue

            ' Calculate the total price for the items with the discount
            Dim totalHargaMasuk As Decimal = (qtyMasukValue * isiMasukValue * hargaMasukValue)

            ' Update TxtQtySatMasuk and TxtTotalHargaMasuk with the calculated values
            TxtTotalQtyMsk.Text = result.ToString()
            TxtTotalHargaMsk.Text = totalHargaMasuk.ToString()
            LblTotalhargaMsk.Text = "Rp. " & totalHargaMasuk.ToString("N0")
        Else
            ' Set TxtQtySatMasuk and TxtTotalHargaMasuk to 0 when the condition is not met
            TxtTotalQtyMsk.Text = "0"
            TxtTotalHargaMsk.Text = "0"
            LblTotalhargaMsk.Text = "Rp. 0"
        End If
        HitungDanTampilkanKeterangan()
    End Sub


    Private Sub HitungDanTampilkanKeterangan()
        Dim totalHargaMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtTotalHargaMsk.Text)
        Dim totalHargaKeluarValue As Decimal = ModuleAngka.ParseDecimal(TxtTotalHargaKlr.Text)

        Dim selisih As Decimal = totalHargaMasukValue - totalHargaKeluarValue
        LblNominalKet.Text = selisih
        ' Tampilkan keterangan berdasarkan nilai selisih
        If selisih > 0 Then
            LblKeterangan.Text = "Kelebihan Rp. " & selisih.ToString("N0")
        ElseIf selisih < 0 Then
            LblKeterangan.Text = "Kekurangan Rp. " & Math.Abs(selisih).ToString("N0")
        Else
            LblKeterangan.Text = "HPP masuk dan HPP keluar sama"
        End If
    End Sub

    '--------------------------------------------------------------------------------------------------------------------------

    Private Sub TxtCariKlr_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCariKlr.TextChanged
        If IsValidInputKlr(TxtCariKlr.Text) Then
            AmbilNamaDariTxtNamaKlr()
        End If
    End Sub

    Private Function IsValidInputKlr(ByVal input As String) As Boolean
        If Not String.IsNullOrEmpty(input) Then
            Dim indexAsterisk As Integer = input.IndexOf("*")

            If indexAsterisk >= 0 Then
                LstBarangKlr.Items.Clear()
                ' Jika tanda '*' ditemukan
                If indexAsterisk + 2 <= input.Length Then
                    ' Jika terdapat minimal 2 karakter setelah '*'
                    Return True
                End If
            ElseIf input.Length >= 2 Then
                ' Jika tidak ditemukan karakter '*' dan panjang input minimal 2 karakter
                Return True
            End If
        End If

        ' Input kosong atau tidak memenuhi syarat
        Return False
    End Function


    Private Sub TxtCariKlr_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtCariKlr.KeyDown
        If e.KeyCode = Keys.Enter AndAlso LstBarangKlr.Visible AndAlso LstBarangKlr.SelectedItem IsNot Nothing Then
            AmbilNamaDariTxtNamaKlr()
        ElseIf e.KeyCode = Keys.Down AndAlso LstBarangKlr.Visible Then
            ' Jika panah bawah ditekan dan ada item di ListBox, pilih item pertama
            LstBarangKlr.Focus()
            LstBarangKlr.SelectedIndex = 0
            e.SuppressKeyPress = True  ' Hentikan kejadian keypress agar tidak memicu kejadian TxtNama_KeyDown lagi
        End If
    End Sub

    Private Sub AmbilNamaDariTxtNamaKlr()
        If Not String.IsNullOrEmpty(TxtCariKlr.Text) Then
            ' Mencari tanda '*' dalam teks
            Dim indexAsterisk As Integer = TxtCariKlr.Text.IndexOf("*")

            ' Set TxtQty.Text ke nilai yang ditemukan sebelum tanda '*'
            TxtQtyKlr.Text = If(indexAsterisk >= 0 AndAlso Integer.TryParse(TxtCariKlr.Text.Substring(0, indexAsterisk).Trim(), Nothing), TxtCariKlr.Text.Substring(0, indexAsterisk).Trim(), "1")

            ' Ambil teks setelah '*' untuk pencarian
            Dim searchKeyword As String = If(indexAsterisk >= 0, TxtCariKlr.Text.Substring(indexAsterisk + 1).Trim(), TxtCariKlr.Text.Trim())
            TampilkanDaftarBarangKlr(searchKeyword)
        Else
            BersihkanPencarianKlr()
        End If
    End Sub

    Private Sub TampilkanDaftarBarangKlr(ByVal searchKeyword As String)
        ' Mengambil data dari database
        Dim query As String = "SELECT NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @Nama OR NAMA_BARANG LIKE @Nama OR BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama OR BARCODE_BESAR LIKE @Nama) ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Clear ListBox before adding new items
                LstBarangKlr.Items.Clear()
                TxtBarcodeKlr.Clear()

                While rd.Read()
                    ' Tambahkan nama barang ke dalam ListBox
                    LstBarangKlr.Items.Add(rd("NAMA_BARANG").ToString())

                    ' Check if the searchKeyword matches any barcode field
                    If searchKeyword = rd("BARCODE_SEDANG").ToString() Or searchKeyword = rd("BARCODE_BESAR").ToString() Then
                        ' Set TxtBarcode.Text to the matched barcode value
                        TxtBarcodeKlr.Text = searchKeyword
                    End If
                End While

                ' Tampilkan ListBox hanya jika lebih dari satu hasil pencarian
                LstBarangKlr.Visible = LstBarangKlr.Items.Count > 0

                ' jika listbox hanya satu hasil pencarian langsung panggil
                If LstBarangKlr.Items.Count = 1 Then
                    AmbilDataDariListBoxKlr()
                End If
            End Using
        End Using
    End Sub


    Private Sub BersihkanPencarianKlr()
        ' Jika TxtNama kosong, bersihkan ListBox dan kosongkan TxtQty.Text
        LstBarangKlr.Items.Clear()
        LstBarangKlr.Visible = False ' Sembunyikan ListBox jika TxtNama kosong
    End Sub


    Private Sub AmbilDataDariListBoxKlr()
        If LstBarangKlr.SelectedItem IsNot Nothing Then
            Dim namaYangDiambilKlr As String = GetTextAfterAsteriskKlr(LstBarangKlr.SelectedItem.ToString())
            IsisatuanmasukKlr(namaYangDiambilKlr)
            AmbildatalaindaridbbarangKlr(namaYangDiambilKlr)
            MasukkandatabarangKlr()
        End If
    End Sub

    Private Sub LstBarangKlr_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarangKlr.KeyDown
        If e.KeyCode = Keys.Enter Then
            AmbilDataDariListBoxKlr()
        End If
    End Sub

    Private Sub LstBarangKlr_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LstBarangKlr.MouseClick
        AmbilDataDariListBoxKlr()
    End Sub

    Private Function GetTextAfterAsteriskKlr(ByVal selectedValue As String) As String
        Dim indexAsterisk As Integer = selectedValue.IndexOf("*")

        If indexAsterisk >= 0 Then
            Return selectedValue.Substring(indexAsterisk + 1).Trim()
        Else
            Return selectedValue
        End If
    End Function

    Private Sub IsisatuanmasukKlr(ByVal namaYangDiambilKlr As String)
        ' Deklarasikan variabel di luar reader
        Dim satuanKecil As String = ""
        Dim satuanSedang As String = ""
        Dim satuanBesar As String = ""

        ' Lakukan query ke tabel "tbl_barang" untuk mendapatkan data terpilih berdasarkan input dari TxtNamaMasuk
        Dim sqlQuery As String = "SELECT DISTINCT SATUAN_UMUM_KECIL,SATUAN_UMUM_SEDANG,SATUAN_UMUM_BESAR FROM tbl_barang WHERE NAMA_BARANG LIKE @selectedData"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@selectedData", namaYangDiambilKlr)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Isi variabel dengan nilai yang sesuai dari reader
                    satuanKecil = If(Not reader.IsDBNull(reader.GetOrdinal("SATUAN_UMUM_KECIL")), reader.GetString(reader.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                    satuanSedang = If(Not reader.IsDBNull(reader.GetOrdinal("SATUAN_UMUM_SEDANG")), reader.GetString(reader.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                    satuanBesar = If(Not reader.IsDBNull(reader.GetOrdinal("SATUAN_UMUM_BESAR")), reader.GetString(reader.GetOrdinal("SATUAN_UMUM_BESAR")), "")
                End If
            End Using
        End Using
        ' Clear ComboBox items before adding new ones
        CmbSatuanKlr.Items.Clear()

        ' Tambahkan item ke ComboBox jika tidak kosong
        If Not String.IsNullOrEmpty(satuanKecil) Then
            CmbSatuanKlr.Items.Add(satuanKecil)
        End If

        If Not String.IsNullOrEmpty(satuanSedang) Then
            CmbSatuanKlr.Items.Add(satuanSedang)
        End If

        If Not String.IsNullOrEmpty(satuanBesar) Then
            CmbSatuanKlr.Items.Add(satuanBesar)
        End If

        ' Set text ComboBox dengan nilai satuan kecil
        CmbSatuanKlr.Text = satuanKecil
    End Sub


    Private idBarangResultKlr As String = ""
    Private namaBarangResultKlr As String = ""
    Private hargaBarangResultKlr As Decimal = 0D
    Private satuanBarangResultKlr As String = ""
    Private isiBarangResultKlr As Integer = 0
    Private stokTokoResultKlr As Decimal = 0D
    Private satuanStokTokoResultKlr As String = ""
    Private stokGudangResultKlr As Decimal = 0D
    Private satuanStokGudangResultKlr As String = ""

    Private Sub AmbildatalaindaridbbarangKlr(ByVal namaYangDiambilKlr As String)
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                   "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                   "STOK_TOKO, SATUAN_STOK, STOK_GUDANG, SATUAN_STOK " &
                   "FROM tbl_barang " &
                   "WHERE NAMA_BARANG LIKE @NamaBarang OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", namaYangDiambilKlr)

            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    idBarangResultKlr = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                    namaBarangResultKlr = namaYangDiambilKlr
                    hargaBarangResultKlr = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    satuanBarangResultKlr = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", Nothing)
                    isiBarangResultKlr = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)

                    If Not String.IsNullOrEmpty(TxtBarcodeKlr.Text) Then
                        Select Case TxtBarcodeKlr.Text
                            Case rd("BARCODE_SEDANG").ToString()
                                satuanBarangResultKlr = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", Nothing)
                                isiBarangResultKlr = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 0)
                            Case rd("BARCODE_BESAR").ToString()
                                satuanBarangResultKlr = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", Nothing)
                                isiBarangResultKlr = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 0)
                        End Select
                    End If

                    ' Memastikan isiSatuan tidak bisa 0
                    If isiBarangResultKlr = 0 Then
                        isiBarangResultKlr = 1
                    End If

                    stokTokoResultKlr = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    satuanStokTokoResultKlr = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_STOK", String.Empty)
                    stokGudangResultKlr = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    satuanStokGudangResultKlr = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_STOK", String.Empty)
                    BersihkanPencarianKlr()
                    TxtCariKlr.Clear()
                    BtnBayar.Focus()
                End If
            End Using
        End Using
    End Sub

    Private Sub MasukkandatabarangKlr()
        TxtKodeKlr.Text = idBarangResultKlr
        TxtNamaKlr.Text = namaBarangResultKlr
        TxtHargaKlr.Text = hargaBarangResultKlr.ToString()
        CmbSatuanKlr.SelectedItem = satuanBarangResultKlr
        TxtIsiKlr.Text = isiBarangResultKlr.ToString()
        LblStokTokoKlr.Text = stokTokoResultKlr.ToString("N0")
        LblSatTokoKlr.Text = satuanStokTokoResultKlr
        LblStokgudangKlr.Text = stokGudangResultKlr.ToString("N0")
        LblSatGudangKlr.Text = satuanStokGudangResultKlr

        UpdateTotalHargaKlr()
        TxtQtyKlr.Select()
    End Sub

    Private Sub CmbSatuanKlr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSatuanKlr.SelectedIndexChanged
        Dim sqlQuery As String = "SELECT ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ID_BARANG"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", TxtKodeKlr.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    If CmbSatuanKlr.SelectedIndex = 1 Then
                        TxtIsiKlr.Text = reader("ISI_UMUM_SEDANG").ToString()
                    ElseIf CmbSatuanKlr.SelectedIndex = 2 Then
                        TxtIsiKlr.Text = reader("ISI_UMUM_BESAR").ToString()
                    Else
                        TxtIsiKlr.Text = reader("ISI_UMUM_KECIL").ToString()
                    End If

                    If String.IsNullOrEmpty(TxtIsiKlr.Text) Or TxtIsiKlr.Text = "0" Then
                        TxtIsiKlr.Text = "1"
                    End If

                    UpdateTotalHargaKlr()
                End If
            End Using
        End Using
    End Sub

    Private Sub QtyKlr_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtQtyKlr.TextChanged
        UpdateTotalHargaKlr()
    End Sub


    Private Sub TxtIsiKlr_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtIsiKlr.TextChanged
        UpdateTotalHargaKlr()
    End Sub

    Private Sub UpdateTotalHargaKlr()
        ' Convert the text values to numeric types, assuming 0 if they are empty
        Dim qtyMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtQtyKlr.Text)
        Dim isiMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtIsiKlr.Text)
        Dim hargaMasukValue As Decimal = ModuleAngka.ParseDecimal(TxtHargaKlr.Text)

        ' Check if all relevant fields are not empty or equal to zero
        If qtyMasukValue <> 0 AndAlso isiMasukValue <> 0 AndAlso hargaMasukValue <> 0 Then
            ' Calculate the new value for TxtQtySatMasuk
            Dim result As Decimal = qtyMasukValue * isiMasukValue

            ' Calculate the total price for the items with the discount
            Dim totalHargaMasuk As Decimal = (qtyMasukValue * isiMasukValue * hargaMasukValue)

            ' Update TxtQtySatMasuk and TxtTotalHargaMasuk with the calculated values
            TxtTotalQtyKlr.Text = result.ToString()
            TxtTotalHargaKlr.Text = totalHargaMasuk.ToString()
            LblTotalhargaKlr.Text = "Rp. " & totalHargaMasuk.ToString("N0")
        Else
            ' Set TxtQtySatMasuk and TxtTotalHargaMasuk to 0 when the condition is not met
            TxtTotalQtyKlr.Text = "0"
            TxtTotalHargaKlr.Text = "0"
            LblTotalhargaKlr.Text = "Rp. 0"
        End If
        HitungDanTampilkanKeterangan()
    End Sub


    '----------------------------------------------------------------------------------------------------------------------------------------

    Private Sub BtnBayar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBayar.Click
        If Not IsFormValid() Then
            ' Jika validasi gagal, hentikan penyimpanan data
            Return
        End If

        If Not ModulHakAkses.SettingIzinkanTanggalLampau Then
            ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
            GenerateNomorTransferstok()
        End If


        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Audit: 2 barang — A=qty dari form, B=qty di HistoryBarang, C=qty di Transfer_stok, D=delta stok
            Dim qtySatMsk As Decimal = If(String.IsNullOrEmpty(TxtTotalQtyMsk.Text), 0D, ModuleAngka.ParseDecimal(TxtTotalQtyMsk.Text))
            Dim qtySatKlr As Decimal = If(String.IsNullOrEmpty(TxtTotalQtyKlr.Text), 0D, ModuleAngka.ParseDecimal(TxtTotalQtyKlr.Text))
            Dim auditDGV As New Dictionary(Of String, Decimal)() From {{TxtKodeMsk.Text, qtySatMsk}, {TxtKodeKlr.Text, qtySatKlr}}
            Dim auditHistory As New Dictionary(Of String, Decimal)() From {{TxtKodeMsk.Text, qtySatMsk}, {TxtKodeKlr.Text, qtySatKlr}}
            Dim auditDetail As New Dictionary(Of String, Decimal)() From {{TxtKodeMsk.Text, qtySatMsk}, {TxtKodeKlr.Text, qtySatKlr}}
            Dim auditStokDelta As New Dictionary(Of String, Decimal)()

            UpdateStokMasukKeluar(transaction)
            InsertJurnalUmum(transaction)
            InsertTransferStok(transaction)
            InsertIntoHistoryBarang(transaction)

            ' Recalculate stok + Audit D (masuk menambah, keluar mengurangi)
            Dim stokMskSebelum As Decimal = BacaStokSaatIni(TxtKodeMsk.Text, LblLokasi.Text, transaction)
            HitungStokPerubahan(TxtKodeMsk.Text, transaction)
            auditStokDelta(TxtKodeMsk.Text) = BacaStokSaatIni(TxtKodeMsk.Text, LblLokasi.Text, transaction) - stokMskSebelum

            Dim stokKlrSebelum As Decimal = BacaStokSaatIni(TxtKodeKlr.Text, LblLokasi.Text, transaction)
            HitungStokPerubahan(TxtKodeKlr.Text, transaction)
            auditStokDelta(TxtKodeKlr.Text) = stokKlrSebelum - BacaStokSaatIni(TxtKodeKlr.Text, LblLokasi.Text, transaction)

            AuditStokTransaksi(LblIdTransaksi.Text, "Transfer Stok", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)


            Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmdAkun As New MySqlCommand(
                "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                "UNION " &
                "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                conn, transaction)
                cmdAkun.Parameters.AddWithValue("@fk", LblIdTransaksi.Text)
                Using rd = cmdAkun.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd(0).ToString().Trim()
                        If kode <> "" Then akunTerlibat.Add(kode)
                    End While
                End Using
            End Using
            For Each kodeAkun As String In akunTerlibat
                UpdateSaldoAkun(kodeAkun, transaction)
            Next

            ' Commit transaksi
            transaction.Commit()

            Dim tsNominal As Decimal = Math.Abs(ModuleAngka.ParseDecimal(LblNominalKet.Text))
            CatatJurnalTidakSeimbang(LblIdTransaksi.Text, tsNominal, tsNominal, "Transfer Stok",
                {"TransferStok"})

            Dim idTransfer As String = LblIdTransaksi.Text
            Kondisiawal()

            Try
                Select Case BacaPengaturanPrinter("TransferStok", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakTransferStok(idTransfer)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak transfer stok?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakTransferStok(idTransfer)
                        End If
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak transfer stok." & vbCrLf & "Detail: " & ex.Message,
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        Catch ex As Exception
            MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                       "Detail kesalahan: " & ex.Message,
           "Oops! Ada masalah simpan Pindah Stok antar barang", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Rollback transaksi secara otomatis karena ada kesalahan
            transaction.Rollback()
        End Try

    End Sub


    Private Function IsFormValid() As Boolean
        ' Validasi kolom masuk
        If String.IsNullOrEmpty(TxtNamaMsk.Text) Then
            MessageBox.Show("Silahkan pilih barang masuk.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtCariMsk.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(TxtQtyMsk.Text) Then
            MessageBox.Show("Kolom Qty Masuk tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtQtyMsk.Focus()
            Return False
        End If

        ' Validasi kolom keluar
        If String.IsNullOrEmpty(TxtNamaKlr.Text) Then
            MessageBox.Show("Silahkan pilih barang keluar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtCariKlr.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(TxtQtyKlr.Text) Then
            MessageBox.Show("Kolom Qty Keluar tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtQtyKlr.Focus()
            Return False
        End If

        ' Validasi stok berdasarkan lokasi
        Dim stokKlr As Decimal
        Dim stokMsk As Decimal
        Dim qtySatKlr As Decimal = If(Not String.IsNullOrEmpty(TxtTotalQtyKlr.Text) AndAlso Not IsDBNull(TxtTotalQtyKlr.Text), ModuleAngka.ParseDecimal(TxtTotalQtyKlr.Text), 0)
        Dim qtySatMsk As Decimal = If(Not String.IsNullOrEmpty(TxtTotalQtyMsk.Text) AndAlso Not IsDBNull(TxtTotalQtyMsk.Text), ModuleAngka.ParseDecimal(TxtTotalQtyMsk.Text), 0)

        If LblLokasi.Text = "GUDANG" Then
            stokKlr = If(Not String.IsNullOrEmpty(LblStokgudangKlr.Text) AndAlso Not IsDBNull(LblStokgudangKlr.Text), ModuleAngka.ParseDecimal(LblStokgudangKlr.Text), 0)
            stokMsk = If(Not String.IsNullOrEmpty(LblStokgudangMsk.Text) AndAlso Not IsDBNull(LblStokgudangMsk.Text), ModuleAngka.ParseDecimal(LblStokgudangMsk.Text), 0)
        ElseIf LblLokasi.Text = "TOKO" Then
            stokKlr = If(Not String.IsNullOrEmpty(LblStokTokoKlr.Text) AndAlso Not IsDBNull(LblStokTokoKlr.Text), ModuleAngka.ParseDecimal(LblStokTokoKlr.Text), 0)
            stokMsk = If(Not String.IsNullOrEmpty(LblStokTokoMsk.Text) AndAlso Not IsDBNull(LblStokTokoMsk.Text), ModuleAngka.ParseDecimal(LblStokTokoMsk.Text), 0)
        End If

        If stokKlr <= 0 OrElse qtySatKlr > stokKlr Then
            MessageBox.Show("Stok " & LblLokasi.Text.ToLower() & " tidak cukup untuk dikeluarkan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtQtyKlr.Focus()
            Return False
        End If

        If qtySatMsk <= 0 Then
            MessageBox.Show("Qty harus diisi terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtQtyMsk.Focus()
            Return False
        End If

        Return True ' Jika semua kolom sudah diisi, maka kembalikan nilai true
    End Function

    Private Sub UpdateStokMasukKeluar(ByVal transaction As MySqlTransaction)
        ' Mulai transaksi
        Dim idBarangMasuk As String = TxtKodeMsk.Text
        Dim idBarangKeluar As String = TxtKodeKlr.Text

        Dim qtySatMasuk As Decimal = If(Not String.IsNullOrEmpty(TxtTotalQtyMsk.Text) AndAlso Not IsDBNull(TxtTotalQtyMsk.Text), ModuleAngka.ParseDecimal(TxtTotalQtyMsk.Text), 0)
        Dim qtySatKeluar As Decimal = If(Not String.IsNullOrEmpty(TxtTotalQtyKlr.Text) AndAlso Not IsDBNull(TxtTotalQtyKlr.Text), ModuleAngka.ParseDecimal(TxtTotalQtyKlr.Text), 0)

        Dim queryUpdateStokMasuk As String = String.Empty
        Dim queryUpdateStokKeluar As String = String.Empty

        ' Tentukan query berdasarkan lokasi
        Select Case LblLokasi.Text
            Case "GUDANG"
                queryUpdateStokMasuk = "UPDATE tbl_barang SET TRANSFER_STOK_MASUK_GUDANG = TRANSFER_STOK_MASUK_GUDANG + ? WHERE ID_BARANG = ?"
                queryUpdateStokKeluar = "UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_GUDANG = TRANSFER_STOK_KELUAR_GUDANG + ? WHERE ID_BARANG = ?"
            Case "TOKO"
                queryUpdateStokMasuk = "UPDATE tbl_barang SET TRANSFER_STOK_MASUK_TOKO = TRANSFER_STOK_MASUK_TOKO + ? WHERE ID_BARANG = ?"
                queryUpdateStokKeluar = "UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_TOKO = TRANSFER_STOK_KELUAR_TOKO + ? WHERE ID_BARANG = ?"
        End Select

        ' Update stok masuk
        Using cmdUpdateStok As New MySqlCommand(queryUpdateStokMasuk, conn, transaction)
            cmdUpdateStok.Parameters.AddWithValue("@QtySat", qtySatMasuk)
            cmdUpdateStok.Parameters.AddWithValue("@ID_BARANG", idBarangMasuk)
            cmdUpdateStok.ExecuteNonQuery()
        End Using

        ' Update stok keluar
        Using cmdUpdateKeluar As New MySqlCommand(queryUpdateStokKeluar, conn, transaction)
            cmdUpdateKeluar.Parameters.AddWithValue("@QtySat", qtySatKeluar)
            cmdUpdateKeluar.Parameters.AddWithValue("@ID_BARANG", idBarangKeluar)
            cmdUpdateKeluar.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertJurnalUmum(ByVal transaction As MySqlTransaction)
        Dim jenisStok As String = LblLokasi.Text
        Dim uraian As String = String.Format("Transfer stok {0} dari {1} ke {2}", jenisStok, TxtNamaMsk.Text, TxtNamaKlr.Text)
        Dim selisih As Decimal = ModuleAngka.ParseDecimal(LblNominalKet.Text)

        ' Akun selisih konversi satuan sesuai COA: 06.04.001 PENYESUAIAN STOK MINUS
        ' (bukan 01.04.002 karena ini penyesuaian nilai persediaan, bukan tagihan uang)
        Const KODE_SELISIH As String = "06.04.001"
        Const NAMA_SELISIH As String = "PENYESUAIAN STOK MINUS"

        ' Hanya catat jurnal jika ada selisih nilai — jika nol tidak perlu jurnal
        If selisih = 0 Then
            Debug.WriteLine("DEBUG JURNAL TRANSFER STOK - ID: " & LblIdTransaksi.Text & " | Selisih = 0, jurnal tidak dibuat")
            Return
        End If

        ' Insert data jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblIdTransaksi.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", uraian)

            ' Selisih positif (nilai masuk > keluar): D PERSEDIAAN, K PENYESUAIAN STOK MINUS
            ' Selisih negatif (nilai masuk < keluar): D PENYESUAIAN STOK MINUS, K PERSEDIAAN
            If selisih > 0 Then
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_SELISIH)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_SELISIH)
            Else
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_SELISIH)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_SELISIH)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
            End If

            cmd.Parameters.AddWithValue("@NOMINAL", Math.Abs(selisih))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "TRANSFER STOK")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

            cmd.ExecuteNonQuery()
        End Using

        ' Debug jurnal transfer stok
        Debug.WriteLine("═══════════════════════════════════════════════════════")
        Debug.WriteLine("DEBUG JURNAL TRANSFER STOK - ID: " & LblIdTransaksi.Text & " | Selisih: " & selisih.ToString("N0"))
        If selisih > 0 Then
            Debug.WriteLine(String.Format("{0,-4} {1,-20} {2,-30} {3,-30} {4,12:N0} {5,12:N0}", "J1", "Selisih Lebih", NAMA_REK_BARANG, NAMA_SELISIH & " [" & KODE_SELISIH & "]", Math.Abs(selisih), Math.Abs(selisih)))
        Else
            Debug.WriteLine(String.Format("{0,-4} {1,-20} {2,-30} {3,-30} {4,12:N0} {5,12:N0}", "J1", "Selisih Kurang", NAMA_SELISIH & " [" & KODE_SELISIH & "]", NAMA_REK_BARANG, Math.Abs(selisih), Math.Abs(selisih)))
        End If
        Debug.WriteLine("✅ JURNAL SEIMBANG - D=K=" & Math.Abs(selisih).ToString("N0"))
        Debug.WriteLine("═══════════════════════════════════════════════════════")
    End Sub


    Private Sub InsertTransferStok(ByVal transaction As MySqlTransaction)
        Using cmd As New MySqlCommand("INSERT INTO Transfer_stok (ID_TRANSFER, JENIS_TRANSFER, URAIAN, TANGGAL, ID_BARANG_M, NAMA_BARANG_M, QTY_M, SATUAN_M, ISI_M, QTY_SAT_M, HARGA_SAT_M, TOTAL_HARGA_M, ID_BARANG_K, NAMA_BARANG_K, QTY_K, SATUAN_K, ISI_K, QTY_SAT_K, HARGA_SAT_K, TOTAL_HARGA_K, Selisih, ID_USER, ID_KOMPUTER) " &
                            "VALUES (@ID_TRANSFER, @JENIS_TRANSFER, @URAIAN, @TANGGAL, @ID_BARANG_M, @NAMA_BARANG_M, @QTY_M, @SATUAN_M, @ISI_M, @QTY_SAT_M, @HARGA_SAT_M, @TOTAL_HARGA_M, @ID_BARANG_K, @NAMA_BARANG_K, @QTY_K, @SATUAN_K, @ISI_K, @QTY_SAT_K, @HARGA_SAT_K, @TOTAL_HARGA_K, @Selisih, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@ID_TRANSFER", LblIdTransaksi.Text)
            If LblLokasi.Text = "TOKO" Then
                cmd.Parameters.AddWithValue("@JENIS_TRANSFER", "TOKO")
                cmd.Parameters.AddWithValue("@URAIAN", "Transfer stok toko antar barang")
            Else
                cmd.Parameters.AddWithValue("@JENIS_TRANSFER", "GUDANG")
                cmd.Parameters.AddWithValue("@URAIAN", "Transfer stok gudang antar barang")
            End If
            cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@ID_BARANG_M", TxtKodeMsk.Text)
            cmd.Parameters.AddWithValue("@NAMA_BARANG_M", TxtNamaMsk.Text)
            cmd.Parameters.AddWithValue("@QTY_M", ModuleAngka.ParseDecimal(TxtQtyMsk.Text))
            cmd.Parameters.AddWithValue("@SATUAN_M", CmbSatuanMsk.Text)
            cmd.Parameters.AddWithValue("@ISI_M", ModuleAngka.ParseInteger(TxtIsiMsk.Text))
            cmd.Parameters.AddWithValue("@QTY_SAT_M", ModuleAngka.ParseDecimal(TxtTotalQtyMsk.Text))
            cmd.Parameters.AddWithValue("@HARGA_SAT_M", ModuleAngka.ParseDecimal(TxtHargaMsk.Text))
            cmd.Parameters.AddWithValue("@TOTAL_HARGA_M", ModuleAngka.ParseDecimal(TxtTotalHargaMsk.Text))
            cmd.Parameters.AddWithValue("@ID_BARANG_K", TxtKodeKlr.Text)
            cmd.Parameters.AddWithValue("@NAMA_BARANG_K", TxtNamaKlr.Text)
            cmd.Parameters.AddWithValue("@QTY_K", ModuleAngka.ParseDecimal(TxtQtyKlr.Text))
            cmd.Parameters.AddWithValue("@SATUAN_K", CmbSatuanKlr.Text)
            cmd.Parameters.AddWithValue("@ISI_K", ModuleAngka.ParseInteger(TxtIsiKlr.Text))
            cmd.Parameters.AddWithValue("@QTY_SAT_K", ModuleAngka.ParseDecimal(TxtTotalQtyKlr.Text))
            cmd.Parameters.AddWithValue("@HARGA_SAT_K", ModuleAngka.ParseDecimal(TxtHargaKlr.Text))
            cmd.Parameters.AddWithValue("@TOTAL_HARGA_K", ModuleAngka.ParseDecimal(TxtTotalHargaKlr.Text))
            cmd.Parameters.AddWithValue("@Selisih", ModuleAngka.ParseDecimal(LblNominalKet.Text))
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

            ' Eksekusi kueri
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub InsertIntoHistoryBarang(ByVal transaction As MySqlTransaction)
        Dim idBarangMasuk As String = TxtKodeMsk.Text
        Dim idBarangKeluar As String = TxtKodeKlr.Text
        Dim qtySatMasuk As Decimal = If(Not String.IsNullOrEmpty(TxtTotalQtyMsk.Text) AndAlso Not IsDBNull(TxtTotalQtyMsk.Text), ModuleAngka.ParseDecimal(TxtTotalQtyMsk.Text), 0)
        Dim qtySatKeluar As Decimal = If(Not String.IsNullOrEmpty(TxtTotalQtyKlr.Text) AndAlso Not IsDBNull(TxtTotalQtyKlr.Text), ModuleAngka.ParseDecimal(TxtTotalQtyKlr.Text), 0)

        Dim query As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                              "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        ' Insert history untuk barang masuk
        Using cmdInsertHistoryMasuk As New MySqlCommand(query, conn, transaction)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@FAKTUR", LblIdTransaksi.Text)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@JENIS", "TRANSFER BARANG MASUK")
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@ID_BARANG", idBarangMasuk)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@NAMA_BARANG", TxtNamaMsk.Text)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@QTY", qtySatMasuk)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@SATUAN", CmbSatuanMsk.Text)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(TxtIsiMsk.Text))
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQtyMsk.Text))
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtTotalHargaMsk.Text))
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmdInsertHistoryMasuk.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmdInsertHistoryMasuk.ExecuteNonQuery()
        End Using

        ' Insert history untuk barang keluar
        Using cmdInsertHistoryKeluar As New MySqlCommand(query, conn, transaction)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@FAKTUR", LblIdTransaksi.Text)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@JENIS", "TRANSFER BARANG KELUAR")
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@ID_BARANG", idBarangKeluar)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@NAMA_BARANG", TxtNamaKlr.Text)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@QTY", qtySatKeluar)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@SATUAN", CmbSatuanKlr.Text)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(TxtIsiKlr.Text))
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQtyKlr.Text))
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtTotalHargaKlr.Text))
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmdInsertHistoryKeluar.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmdInsertHistoryKeluar.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub QtyKeluar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtQtyKlr.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Mencegah karakter yang bukan angka dimasukkan
        End If

        If e.KeyChar = Chr(13) Then
            TxtCariMsk.Select()
        End If
    End Sub
    Private Sub QtyMasuk_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtQtyMsk.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Mencegah karakter yang bukan angka dimasukkan
        End If



    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluarForm.Click
        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
        Close()
    End Sub


    Private Sub FormGanti_Barang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                BtnBayar.PerformClick()
            Case Keys.Escape
                BtnKeluarForm.PerformClick()
        End Select
    End Sub

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F8      : Simpan transfer stok" & vbCrLf &
                           "ESC     : Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' ============================================
    ' ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal
    ' ============================================

End Class
