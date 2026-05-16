Public Class FormTransferBarang

    Private Sub Formtransferbarang(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Area input dan grand total otomatis via nama kontrol
        ' Rename GroupBox -> GBInput/GBTotal untuk tema otomatis
        ' Set ukuran maksimum dan minimum untuk memastikan form tidak menutupi taskbar
        MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        MinimumSize = Size

        KosongTxtboxcari()

        ' Setting dibaca langsung dari ModulHakAkses property

        If LblJenisTrans.Text = "TambahTransfer" Then
            Kondisiawal()
        Else
            Kondisiawaledit()
            AmbilDataUntukEdit()
        End If


        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        End If

        If ModulHakAkses.SettingFokusOtomatis Then
            TxtNama.Select()
        End If
    End Sub


    Private Sub LakukanCetakTransferBarang(idTransfer As String)
        If BacaPengaturanPrinter("TransferBarang", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterTransferBarang.TanyaPilihPrinterTransferBarang(idTransfer)
        Else
            ModulePrinterTransferBarang.CetakTransferBarang(idTransfer)
        End If
    End Sub

    Private Sub LblLokasiBarang_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblLokasiBarang.TextChanged
        Select Case LblLokasiBarang.Text
            Case "TOKO"
                LblTujuanTransfer.Text = "GUDANG"
            Case "GUDANG"
                LblTujuanTransfer.Text = "TOKO"
        End Select
    End Sub

    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCari.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan

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
        PanelCari.BackColor = SystemColors.ActiveCaption
    End Sub

    Private Sub KosongTxtboxcari()
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHarga.Clear()
        TxtBarcode.Clear()
        TxtNama.Clear()
    End Sub

    Private Sub Kondisiawaledit()

        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
    End Sub

    Private Sub Kondisiawal()
        DgvData.Rows.Clear()
        TxtTotalQTY.Text = 0
        TxtGrandtotal.Text = ""
        TxtGrandtotal.Text = 0


        ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTgl)
        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        LblRecord.Text = "Total record : 0"

        NomorTransfer()

    End Sub


    Private Sub NomorTransfer()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "TB")
            cmd.Parameters.AddWithValue("@tgl", DTPTgl.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "transfer_barang")
            cmd.Parameters.AddWithValue("@kolom", "ID_TRANSFER")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            TxtFaktur.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Private Sub Hapusbaris()
        Dim baris As Integer = DgvData.CurrentCell.RowIndex
        DgvData.Rows.RemoveAt(baris)
        UpdateSemuaTotal()
    End Sub


    Public Sub UpdateSemuaTotal()
        Dim grandTotal As Decimal = 0
        Dim totalQtyBarang As Decimal = 0
        Dim totalRows As Integer = 0
        Dim totalQty As Decimal = 0

        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow Then Continue For

            ' Total Harga
            grandTotal += ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value)

            ' Jumlah Barang
            Dim qtyValue As Object = row.Cells("Qty").Value
            If qtyValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyValue.ToString()) Then
                totalQtyBarang += ModuleAngka.ParseDecimal(qtyValue)
                totalRows += 1
            End If

            ' Total QTY
            totalQty += ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
        Next

        ' Tampilkan hasil ke UI
        TxtGrandtotal.Text = grandTotal.ToString("N0")
        Txtlihattotal.Text = "Rp. " & grandTotal.ToString("N0")
        TxtTotalQTY.Text = totalQty.ToString("N0")
        LblRecord.Text = totalRows.ToString()

        ' Scroll ke baris terakhir jika ada baris
        If DgvData.Rows.Count > 0 Then
            Try
                DgvData.FirstDisplayedScrollingRowIndex = DgvData.Rows.Count - 1
            Catch ex As Exception
                ' Optional: Log atau abaikan jika tidak penting
            End Try
        End If
    End Sub

    ' ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal


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

    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        If suppressTextChanged Then
            suppressTextChanged = False
            Return
        End If
        ProsesInput(False) 'Manual input
    End Sub

    Private Sub ProsesInput(ByVal isBarcode As Boolean)
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
                    TxtHarga.Text = hargaSebelumAsterisk

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

                Else
                    ' Jika tidak ada *, gunakan nama barang secara keseluruhan
                    TampilkanDaftarBarang(TxtNama.Text)
                    TxtQty.Text = "1"
                End If
            End If
        Else
            ' Jika input kosong, kosongkan list dan text box
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            TxtQty.Text = "1"
        End If
    End Sub


    Private Sub TampilkanDaftarBarang(ByVal searchKeyword As String)
        ' Mengambil data dari database
        Dim query As String = "SELECT NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR ,STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @Nama OR NAMA_BARANG LIKE @Nama OR BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama OR BARCODE_BESAR LIKE @Nama) ORDER BY NAMA_BARANG"

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
        Dim queryAmbilData As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE STATUS = 'Aktif' AND NAMA_BARANG = @NAMA"

        Using cmd As New MySqlCommand(queryAmbilData, conn)
            cmd.Parameters.AddWithValue("@NAMA", namayangdiambil)
            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    ' Ambil nilai dari database
                    Dim idBarang As String = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                    Dim hargaBeli As String = ModuleAngka.ParseDecimal(rd("HARGA_BELI")).ToString()

                    Dim satuanUmum As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                    Dim isiUmum As Integer = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)

                    ' Periksa apakah TxtBarcode.Text tidak kosong
                    If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
                        ' Sesuaikan nilai berdasarkan barcode
                        If TxtBarcode.Text = rd("BARCODE_KECIL").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                            isiUmum = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)
                        ElseIf TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", String.Empty)
                            isiUmum = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 0)
                        ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", String.Empty)
                            isiUmum = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 0)
                        End If
                    End If

                    ' Pastikan isiUmum tidak bernilai nol
                    If isiUmum = 0 Then
                        isiUmum = 1
                    End If

                    Dim StokToko As String = ModuleAngka.ParseDecimal(rd("STOK_TOKO")).ToString()
                    Dim StokGudang As String = ModuleAngka.ParseDecimal(rd("STOK_GUDANG")).ToString()

                    ' Set nilai textbox
                    TxtKode.Text = idBarang
                    TxtHarga.Text = hargaBeli
                    Txtsatuan.Text = satuanUmum
                    TxtIsi.Text = isiUmum.ToString()

                    Select Case LblLokasiBarang.Text
                        Case "TOKO"
                            TxtStok.Text = StokToko
                        Case "GUDANG"
                            TxtStok.Text = StokGudang
                    End Select
                End If
            End Using
        End Using
        ' Memanggil fungsi tambahan jika diperlukan
        TambahDataLangsung(namayangdiambil)
    End Sub


    Private Sub TambahDataLangsung(ByVal namayangdiambil As String)
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
            For Each row As DataGridViewRow In DgvData.Rows
                If row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() = TxtKode.Text Then
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

        ' Mendapatkan kolom ComboBoxDataGridView dengan nama "SATUAN"
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(indeksBaris).Cells("Satuan"), DataGridViewComboBoxCell)

        ' Membersihkan item yang sudah ada di kolom ComboBoxDataGridView
        kolomSatuan.Items.Clear()

        ' Mengambil satuan yang berbeda dari database
        Dim sql As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG LIKE ?"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", "%" & TxtKode.Text & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        Dim satuanKecil As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                        Dim satuanSedang As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                        Dim satuanBesar As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")

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
        Dim kode As String = TxtKode.Text
        Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHarga.Text)
        Dim qty As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
        If qty <= 0 Then qty = 1D
        Dim satuan As String = Txtsatuan.Text

        Dim isi As Decimal = ModuleAngka.ParseDecimal(TxtIsi.Text)
        Dim Stok As Decimal = ModuleAngka.ParseDecimal(TxtStok.Text)

        ' Menetapkan nilai untuk baris yang baru ditambahkan
        DgvData.Rows(indeksBaris).Cells("Id").Value = kode
        DgvData.Rows(indeksBaris).Cells("nama").Value = namayangdiambil
        DgvData.Rows(indeksBaris).Cells("Hargabeli").Value = hargaBeli
        DgvData.Rows(indeksBaris).Cells("qty").Value = qty
        DgvData.Rows(indeksBaris).Cells("Satuan").Value = satuan
        DgvData.Rows(indeksBaris).Cells("isi").Value = isi
        DgvData.Rows(indeksBaris).Cells("HargaBeliSat").Value = hargaBeli * isi
        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qty * isi
        DgvData.Rows(indeksBaris).Cells("Totalharga").Value = qty * isi * hargaBeli
        DgvData.Rows(indeksBaris).Cells("Stok").Value = Stok

        ' Melakukan pembaruan pada ringkasan atau operasi relevan lainnya
        UpdateSemuaTotal()

        ' Membersihkan field input
        KosongTxtboxcari()

    End Sub



    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        '========================== Nama
        If e.ColumnIndex = 1 Then
            Dim row = DgvData.Rows(e.RowIndex)
            Dim namaCell = row?.Cells("Nama")
            Dim namaValue As String = namaCell?.Value?.ToString().Trim()

            If Not String.IsNullOrEmpty(namaValue) Then
                Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, " &
            "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
            "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
            "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
            "STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
            "WHERE ID_BARANG LIKE @NamaBarang OR NAMA_BARANG LIKE @NamaBarang " &
            "OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang"

                Dim dataTidakDitemukan As Boolean = False

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@NamaBarang", namaValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            row.Cells("Id").Value = rd("ID_BARANG")
                            row.Cells("Hargabeli").Value = rd("HARGA_BELI")

                            ' Isi ComboBox satuan
                            Dim comboCell As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
                            comboCell.Items.Clear()

                            Dim satuanKecil = If(Not rd.IsDBNull(3), rd.GetString(3), "")
                            Dim satuanSedang = If(Not rd.IsDBNull(4), rd.GetString(4), "")
                            Dim satuanBesar = If(Not rd.IsDBNull(5), rd.GetString(5), "")

                            If satuanKecil <> "" Then comboCell.Items.Add(satuanKecil)
                            If satuanSedang <> "" Then comboCell.Items.Add(satuanSedang)
                            If satuanBesar <> "" Then comboCell.Items.Add(satuanBesar)

                            Dim satuan As String = ""
                            Dim isi As Integer = 1

                            If namaValue = rd("NAMA_BARANG").ToString() OrElse namaValue = rd("BARCODE_KECIL").ToString() Then
                                satuan = satuanKecil
                                isi = If(Not rd.IsDBNull(6), rd.GetInt32(6), 1)
                            ElseIf namaValue = rd("BARCODE_SEDANG").ToString() Then
                                satuan = satuanSedang
                                isi = If(Not rd.IsDBNull(7), rd.GetInt32(7), 1)
                            ElseIf namaValue = rd("BARCODE_BESAR").ToString() Then
                                satuan = satuanBesar
                                isi = If(Not rd.IsDBNull(8), rd.GetInt32(8), 1)
                            End If

                            row.Cells("Satuan").Value = satuan
                            row.Cells("isi").Value = isi
                            If isi = 0 Then isi = 1
                            row.Cells("HargaBeliSat").Value = CDec(row.Cells("Hargabeli").Value) * isi
                            row.Cells("qty").Value = 1
                            row.Cells("QtySat").Value = 1 * isi
                            row.Cells("Totalharga").Value = CDec(row.Cells("Hargabeli").Value) * isi

                            row.Cells("Stok").Value = If(LblLokasiBarang.Text = "TOKO", rd("STOK_TOKO"), rd("STOK_GUDANG"))
                            row.Cells("nama").Value = rd("NAMA_BARANG")
                        Else
                            dataTidakDitemukan = True
                        End If
                    End Using
                End Using

                If dataTidakDitemukan Then
                    row.Cells("nama").Value = ""
                    SendKeys.Send("{down}")
                End If

                ' Gabungkan Qty jika SettingIzinkanSatuanBerbeda = False
                If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                    For i = 0 To DgvData.RowCount - 1
                        For j = i + 1 To DgvData.RowCount - 2
                            If DgvData.Rows(i).Cells("Id").Value = DgvData.Rows(j).Cells("Id").Value Then
                                DgvData.Rows(i).Cells("qty").Value += 1
                                Dim isiVal = CInt(DgvData.Rows(i).Cells("isi").Value)
                                DgvData.Rows(i).Cells("qtysat").Value = If(isiVal = 0, CInt(DgvData.Rows(i).Cells("qtysat").Value) + 1, isiVal * CInt(DgvData.Rows(i).Cells("qty").Value))
                                DgvData.Rows(i).Cells("totalharga").Value = CDec(DgvData.Rows(i).Cells("Hargabeli").Value) * CDec(DgvData.Rows(i).Cells("qtysat").Value)
                                Hapusbaris()
                                SendKeys.Send("{down}")
                            End If
                        Next
                    Next
                End If
            Else
                ' Kosong atau null
                row.Cells("nama").Value = ""
                SendKeys.Send("{down}")
            End If
        End If


        '========================== Harga beli (Kolom 2)
        If e.ColumnIndex = 2 Then
            Dim row = DgvData.Rows(e.RowIndex)
            Dim harga As Decimal = CDec(If(row.Cells("Hargabeli").Value Is Nothing, 0, row.Cells("Hargabeli").Value))

            If harga <= 0 Then
                row.Cells("Hargabeli").Value = 0
                MessageBox.Show("Harga harus > 0", "Peringatan")
                Return
            End If

            Dim qty As Decimal = CDec(row.Cells("Qty").Value)
            Dim isi As Integer = CInt(row.Cells("Isi").Value)

            row.Cells("QtySat").Value = qty * isi
            row.Cells("HargaBeliSat").Value = harga * isi
            row.Cells("Totalharga").Value = harga * qty * isi
        End If

        '========================== Qty (Kolom 3)
        If e.ColumnIndex = 3 Then
            Dim row = DgvData.Rows(e.RowIndex)
            Dim qty As Decimal = CDec(If(row.Cells("Qty").Value Is Nothing, 0, row.Cells("Qty").Value))

            If qty <= 0 Then
                row.Cells("Qty").Value = 1
                qty = 1
            End If

            Dim harga As Decimal = CDec(row.Cells("Hargabeli").Value)
            Dim isi As Integer = CInt(row.Cells("Isi").Value)

            row.Cells("QtySat").Value = qty * isi
            row.Cells("Totalharga").Value = harga * qty * isi
        End If


        UpdateSemuaTotal()
    End Sub

    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DgvData.DataError
        e.Cancel = True
    End Sub

    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Delete Then
            If DgvData.SelectedCells.Count > 0 Then
                Dim selectedCell As DataGridViewCell = DgvData.SelectedCells(0)

                ' Periksa apakah sel yang dipilih berada di kolom "Nama"
                If selectedCell.ColumnIndex = DgvData.Columns("Nama").Index Then
                    Dim rowIndex As Integer = selectedCell.RowIndex

                    ' Periksa apakah nilai di kolom "Nama" tidak kosong
                    If Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("Nama").Value.ToString()) Then
                        ' Hapus baris jika nilai di kolom "Nama" tidak kosong
                        DgvData.Rows.RemoveAt(rowIndex)
                        ' Setelah menghapus baris, pastikan untuk menghilangkan seleksi agar tidak ada baris yang dipilih secara default.
                        DgvData.ClearSelection()
                    Else
                        MessageBox.Show("Klik kanan pada baris yang tidak kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
        End If

        UpdateSemuaTotal()
    End Sub

    Private Sub DgvData_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub


    Private Sub DgvData_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles DgvData.EditingControlShowing

        Dim titleText As String = DgvData.Columns(1).HeaderText
        If titleText.Equals("Nama") Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.Suggest
                autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                Dim DataCollection As New AutoCompleteStringCollection()
                AddItems(DataCollection, autoText.Text.Trim()) ' Melewati nilai autoText.Text sebagai namaValue.
                autoText.AutoCompleteCustomSource = DataCollection
            End If
        End If

        ' Periksa apakah kolom yang saat ini sedang diedit adalah kolom yang berisi ComboBox (misalnya, kolom dengan indeks 4)
        If DgvData.CurrentCell.ColumnIndex = 4 Then
            Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)

            ' Hapus penanganan event SelectedIndexChanged jika ada
            RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged

            ' Tambahkan penanganan event SelectedIndexChanged ke ComboBox
            AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
        End If

    End Sub

    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)

        ' Dapatkan sel saat ini yang sedang diedit
        Dim cell As DataGridViewComboBoxCell = DirectCast(DgvData.CurrentCell, DataGridViewComboBoxCell)
        Dim selectedItemId As String = cell.OwningRow.Cells("Id").Value.ToString()

        Using cmd As New MySqlCommand("SELECT ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ItemId", conn)
            cmd.Parameters.AddWithValue("@ItemId", selectedItemId)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Update nilai pada kolom "Isi" berdasarkan indeks yang dipilih dalam ComboBox
                    Select Case comboBox.SelectedIndex
                        Case 0
                            cell.OwningRow.Cells("Isi").Value = rd("ISI_UMUM_KECIL").ToString()
                        Case 1
                            cell.OwningRow.Cells("Isi").Value = rd("ISI_UMUM_SEDANG").ToString()
                        Case Else
                            cell.OwningRow.Cells("Isi").Value = rd("ISI_UMUM_BESAR").ToString()
                    End Select

                    ' Lakukan perhitungan sel lain yang berkaitan dengan perubahan ini
                    Dim rowIndex As Integer = DgvData.CurrentCell.RowIndex
                    DgvData("HargaBeliSat", rowIndex).Value = CDec(DgvData("Hargabeli", rowIndex).Value) * CDec(DgvData("isi", rowIndex).Value)
                    DgvData("qtysat", rowIndex).Value = CDec(DgvData("isi", rowIndex).Value) * CDec(DgvData("qty", rowIndex).Value)
                    DgvData("totalharga", rowIndex).Value = CDec(DgvData("hargabeli", rowIndex).Value) * CDec(DgvData("qtysat", rowIndex).Value)

                    UpdateSemuaTotal()
                Else
                    MessageBox.Show("Satuan barang dan atau harga jual belum di input !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End Using
        End Using
    End Sub


    Public Sub AddItems(ByVal col As AutoCompleteStringCollection, ByVal namaValue As String)
        Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @Nama"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & namaValue & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    col.Add(rd("NAMA_BARANG").ToString())
                Loop
            End Using ' rd will be properly disposed here
        End Using ' cmd will be properly disposed here
    End Sub


    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluarForm.Click
        Close()
    End Sub

    Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvData.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Periksa apakah sel yang diklik kanan ada
            Dim cell As DataGridViewCell = DgvData.Rows(e.RowIndex).Cells("Nama")
            If cell IsNot Nothing AndAlso cell.Value IsNot Nothing Then
                ' Periksa apakah nilai di kolom "Nama" pada baris yang diklik tidak kosong
                Dim namaValue As String = cell.Value.ToString()
                If Not String.IsNullOrEmpty(namaValue) Then
                    ' Setel sel saat ini ke sel "Nama"
                    DgvData.CurrentCell = cell
                    ' Tampilkan menu konteks
                    Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                    ContextMenuStrip1.Show(cursorPosition)
                End If
            End If
        End If
    End Sub




    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
    End Sub

    Private Sub TxtGrandtotal_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtGrandtotal.TextChanged
        Dim grandTotal As Decimal = ModuleAngka.ParseDecimal(TxtGrandtotal.Text)
        Txtlihattotal.Text = ModuleAngka.FormatRupiah(grandTotal)
    End Sub

    Private Sub Form_Pembelian_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                Tekansimpan()
            Case Keys.Escape
                Close()
        End Select
    End Sub


    Private Sub BtnSimpann_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpann.Click
        Tekansimpan()
    End Sub

    Public Class StokInfo
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    Public Function CekStok() As Boolean
        ' Membuat Dictionary untuk menyimpan informasi stok barang dan penjualan
        Dim stokDict As New Dictionary(Of String, StokInfo)

        ' Iterasi melalui setiap baris di DataGridView
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            ' Mengecek apakah baris baru dan apakah kolom "Id" tidak kosong
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Id").Value IsNot Nothing AndAlso dgvRow.Cells("Id").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Id").Value.ToString()

                ' Mengumpulkan informasi stok barang dari database
                Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG LIKE @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            Dim stokToko As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                            Dim stokGudang As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                            stokDict(kodeBarangValue) = New StokInfo() With {.StokToko = stokToko, .StokGudang = stokGudang}
                        End If
                    End Using
                End Using

                ' Jika jenis transaksi bukan "TambahTransfer", kumpulkan informasi data sebelumnya
                If LblJenisTrans.Text <> "TambahTransfer" Then
                    Using cmdjual As New MySqlCommand("SELECT ID_BARANG, SUM(TOTAL_QTY) AS TOTAL_QTY FROM Transfer_barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER AND ID_BARANG = @ID_BARANG GROUP BY ID_BARANG", conn)
                        cmdjual.Parameters.AddWithValue("@ID_TRANSFER", TxtFaktur.Text)
                        cmdjual.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                        Using rdjual As MySqlDataReader = cmdjual.ExecuteReader()
                            While rdjual.Read()
                                Dim idBarang As String = rdjual("ID_BARANG").ToString()
                                Dim totalQtyTerjualRow As Decimal = Convert.ToDecimal(rdjual("TOTAL_QTY"))
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
                Dim totalQtyTerjual As Decimal = Convert.ToDecimal(dgvRow.Cells("QtySat").Value)
                If stokDict.ContainsKey(kodeBarangValue) Then
                    Dim stokInfo As StokInfo = stokDict(kodeBarangValue)
                    Dim totalStok As Decimal

                    ' Menentukan stok yang akan digunakan berdasarkan jenis transaksi
                    If LblLokasiBarang.Text = "TOKO" Then
                        totalStok = stokInfo.StokToko
                    Else
                        totalStok = stokInfo.StokGudang
                    End If

                    ' Mengecek apakah stok mencukupi
                    If totalQtyTerjual > totalStok Then
                        ' Jika stok tidak mencukupi, tampilkan pesan error
                        Dim errorMessage As String = "Stok ==> " & dgvRow.Cells("Nama").Value & " <== tidak mencukupi untuk ditransfer. " & vbCrLf & vbCrLf & "Total Qty transfer: " & totalQtyTerjual & ", Total Stok: " & totalStok
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

                    ' Mengembalikan warna sel ke warna default jika stok mencukupi
                    Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = defaultBackColor
                    Next
                End If
            End If
        Next

        Return False ' Tidak ada masalah
    End Function


    Public Sub Tekansimpan()
        ' Cek apakah belum ada transaksi pembelian
        If TxtGrandtotal.Text = "0" OrElse DgvData.RowCount = 0 Then
            MessageBox.Show("Belum ada transaksi Pembelian", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            If DgvData.Rows.Count > 0 AndAlso DgvData.Columns.Count > 1 Then
                DgvData.CurrentCell = DgvData(1, 0)
                DgvData.Rows(0).Selected = True
            End If

            If ModulHakAkses.SettingFokusOtomatis Then
                TxtNama.Select()
                TxtNama.Focus()
                Exit Sub
            End If
        End If

        If Not ModulHakAkses.SettingIzinkanBarangMinus Then
            If CekStok() Then
                Return
            End If
        End If

        ProsesSimpan()
    End Sub

    Public Sub ProsesSimpan()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        ' Mengubah kursor menjadi menunggu
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Try

            Dim akunLama As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            If LblJenisTrans.Text <> "TambahTransfer" Then
                ' ========================================
                ' STEP 1: SELECT daftar akun LAMA SEBELUM DELETE JurnalUmum
                ' ========================================
                Using cmdAkunLama As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, transaction)
                    cmdAkunLama.Parameters.AddWithValue("@fk", TxtFaktur.Text)
                    Using rd = cmdAkunLama.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunLama.Add(kode)
                        End While
                    End Using
                End Using
                ' ========================================
                ' START: Audit Trail - Edit Transfer Barang
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Try
                    Using cmdSnap As New MySqlCommand(
                        "SELECT ID_TRANSFER, TGL_TRANSFER, LOKASI_ASAL, LOKASI_TUJUAN, TOTAL_QTY, KETERANGAN, ID_USER " &
                        "FROM Transfer_barang WHERE ID_TRANSFER = @id LIMIT 1", conn, transaction)
                        cmdSnap.Parameters.AddWithValue("@id", TxtFaktur.Text)
                        Using rdSnap = cmdSnap.ExecuteReader()
                            If rdSnap.Read() Then
                                sbSnapshot.AppendLine($"ID Transfer: {rdSnap("ID_TRANSFER")}")
                                sbSnapshot.AppendLine($"Tanggal Transfer: {Convert.ToDateTime(rdSnap("TGL_TRANSFER")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                sbSnapshot.AppendLine($"Lokasi Asal: {rdSnap("LOKASI_ASAL")}")
                                sbSnapshot.AppendLine($"Lokasi Tujuan: {rdSnap("LOKASI_TUJUAN")}")
                                sbSnapshot.AppendLine($"Total Qty: {ModuleAngka.ParseDecimal(rdSnap("TOTAL_QTY"))} unit")
                                sbSnapshot.AppendLine($"Keterangan: {rdSnap("KETERANGAN")}")
                            End If
                        End Using
                    End Using

                    sbSnapshot.AppendLine(vbCrLf & "Detail Barang:")
                    Using cmdSnapDetail As New MySqlCommand(
                        "SELECT KODE_BARANG, NAMA_BARANG, QTY_TRANSFER " &
                        "FROM Transfer_barang_detail WHERE ID_TRANSFER = @id ORDER BY KODE_BARANG", conn, transaction)
                        cmdSnapDetail.Parameters.AddWithValue("@id", TxtFaktur.Text)
                        Using rdSnapDetail = cmdSnapDetail.ExecuteReader()
                            While rdSnapDetail.Read()
                                sbSnapshot.AppendLine($"- {rdSnapDetail("KODE_BARANG")} - {rdSnapDetail("NAMA_BARANG")}: {rdSnapDetail("QTY_TRANSFER")} unit")
                            End While
                        End Using
                    End Using
                Catch
                    sbSnapshot.AppendLine("Gagal baca data sebelum edit")
                End Try
                ModuleAuditTrail.CatatAuditMaster("TRF:" & TxtFaktur.Text, "EDIT", "Transfer Barang", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Transfer Barang
                ' ========================================
                HapusUntukEdit(transaction)
            Else
                If Not ModulHakAkses.SettingIzinkanTanggalLampau Then
                    ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTgl)
                    NomorTransfer()
                End If
            End If

            SimpanSurat_Jalan(transaction)
            SimpanSurat_Jalan_Detail(transaction)

            ' Audit: inisialisasi dictionary
            Dim auditDGV As New Dictionary(Of String, Decimal)()
            Dim auditHistory As New Dictionary(Of String, Decimal)()
            Dim auditDetail As New Dictionary(Of String, Decimal)()
            Dim auditStokDelta As New Dictionary(Of String, Decimal)()

            ' Audit A + C: baca qty dari DGV (kolom 7 = TOTAL_QTY)
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    Dim kodeA As String = row.Cells(0).Value.ToString()
                    Dim qtyA As Decimal = ModuleAngka.ParseDecimal(row.Cells(7).Value)
                    If auditDGV.ContainsKey(kodeA) Then auditDGV(kodeA) += qtyA Else auditDGV(kodeA) = qtyA
                    If auditDetail.ContainsKey(kodeA) Then auditDetail(kodeA) += qtyA Else auditDetail(kodeA) = qtyA
                End If
            Next

            HistoryBarang(transaction, auditHistory)   ' mengisi B

            Simpanjurnal(transaction)

            ' Recalculate stok + Audit D
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    Dim kodeD As String = row.Cells(0).Value.ToString()
                    Dim stokSebelum As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    HitungStokPerubahan(kodeD, transaction)
                    Dim stokSesudah As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    Dim delta As Decimal = stokSebelum - stokSesudah  ' transfer keluar mengurangi stok asal
                    If auditStokDelta.ContainsKey(kodeD) Then auditStokDelta(kodeD) += delta Else auditStokDelta(kodeD) = delta
                End If
            Next

            AuditStokTransaksi(TxtFaktur.Text, "Transfer Barang", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)

            ' ========================================
            ' STEP 2: SELECT daftar akun BARU
            ' ========================================
            Dim akunBaru As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmdAkunBaru As New MySqlCommand(
                "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                "UNION " &
                "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                conn, transaction)
                cmdAkunBaru.Parameters.AddWithValue("@fk", TxtFaktur.Text)
                Using rd = cmdAkunBaru.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd(0).ToString().Trim()
                        If kode <> "" Then akunBaru.Add(kode)
                    End While
                End Using
            End Using

            ' ========================================
            ' STEP 3: GABUNGKAN daftar akun LAMA + BARU
            ' ========================================
            Dim semuaAkunTerlibat As New HashSet(Of String)(akunLama, StringComparer.OrdinalIgnoreCase)
            For Each akun In akunBaru
                semuaAkunTerlibat.Add(akun)
            Next

            ' ========================================
            ' STEP 4: UPDATE saldo untuk SEMUA akun yang terlibat
            ' ========================================
            For Each kodeAkun As String In semuaAkunTerlibat
                UpdateSaldoAkun(kodeAkun, transaction)
            Next

            ' Commit transaksi jika tidak ada kesalahan
            transaction.Commit()

            Dim tbNominal As Decimal = ModuleAngka.ParseDecimal(TxtGrandtotal.Text)
            CatatJurnalTidakSeimbang(TxtFaktur.Text, tbNominal, tbNominal, "Transfer Barang",
                {"TransferBarang"})

            Dim idTransfer As String = TxtFaktur.Text
            Kondisiawal()

            If LblJenisTrans.Text <> "TambahTransfer" Then
                Close()
            End If

            Try
                Select Case BacaPengaturanPrinter("TransferBarang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakTransferBarang(idTransfer)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak transfer barang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakTransferBarang(idTransfer)
                        End If
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak transfer barang." & vbCrLf & "Detail: " & ex.Message,
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        Catch ex As Exception
            MessageBox.Show("Oh tidak! Transaksi transfer barang dibatalkan karena terjadi kesalahan." & vbCrLf &
                         "Detail kesalahan: " & ex.Message,
              "Oops! Ada masalah simpan pembelian", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Rollback transaksi secara otomatis karena ada kesalahan
            transaction.Rollback()

        Finally
            ' Mengembalikan kursor ke normal setelah proses selesai atau terjadi kesalahan
            System.Windows.Forms.Cursor.Current = Cursors.Default
        End Try
    End Sub



    Private Sub HapusUntukEdit(ByVal transaction As MySqlTransaction)
        Dim stokKeluarField As String
        Dim stokMasukField As String

        Select Case LblLokasiBarang.Text
            Case "TOKO"
                stokKeluarField = "TRANSFER_BARANG_KELUAR_TOKO"
                stokMasukField = "TRANSFER_BARANG_MASUK_GUDANG"
            Case "GUDANG"
                stokKeluarField = "TRANSFER_BARANG_KELUAR_GUDANG"
                stokMasukField = "TRANSFER_BARANG_MASUK_TOKO"
            Case Else
                Throw New Exception("Lokasi barang tidak valid.")
        End Select

        Dim updateQuery As String = "UPDATE tbl_barang SET " & stokKeluarField & " = " & stokKeluarField & " - @QtySatKeluar, " & stokMasukField & " = " & stokMasukField & " - @QtySatMasuk WHERE ID_BARANG = @KodeBarang"

        ' Audit hapus lama: A dari DGVDetail, D dari delta stok
        Dim auditDGVHapusTB As New Dictionary(Of String, Decimal)()
        Dim auditDeltaHapusTB As New Dictionary(Of String, Decimal)()

        For Each row As DataGridViewRow In FormUtama.DGVDetail.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim qtySat As Decimal = If(row.Cells("TOTAL_QTY").Value IsNot Nothing, Convert.ToDecimal(row.Cells("TOTAL_QTY").Value), 0D)
                    If auditDGVHapusTB.ContainsKey(kodeBarang) Then auditDGVHapusTB(kodeBarang) += qtySat Else auditDGVHapusTB(kodeBarang) = qtySat

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@QtySatKeluar", qtySat)
                        cmd.Parameters.AddWithValue("@QtySatMasuk", qtySat)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using

                    Dim sebelumEditTB As Decimal = BacaStokSaatIni(kodeBarang, LblLokasiBarang.Text, transaction)
                    HitungStokPerubahan(kodeBarang, transaction)
                    Dim sesudahEditTB As Decimal = BacaStokSaatIni(kodeBarang, LblLokasiBarang.Text, transaction)
                    Dim deltaTB As Decimal = sesudahEditTB - sebelumEditTB  ' hapus transfer lama mengembalikan stok asal
                    If auditDeltaHapusTB.ContainsKey(kodeBarang) Then auditDeltaHapusTB(kodeBarang) += deltaTB Else auditDeltaHapusTB(kodeBarang) = deltaTB
                End If
            End If
        Next

        AuditStokTransaksi(TxtFaktur.Text & " [HAPUS-EDIT]", "Edit Transfer Barang (hapus lama)", auditDGVHapusTB, Nothing, Nothing, auditDeltaHapusTB, transaction)

        Dim deleteQueries As String() = {
            "DELETE FROM Transfer_Barang WHERE ID_TRANSFER = @ID_TRANSFER",
            "DELETE FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_TRANSFER",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_TRANSFER"
        }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFaktur.Text)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub


    Private Sub SimpanSurat_Jalan(ByVal transaction As MySqlTransaction)
        Dim sql As String = "INSERT INTO Transfer_Barang (ID_TRANSFER, TGL_TRANSFER, LOKASI, TOTAL_QTY, TOTAL_BARANG, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                            "VALUES (@ID_TRANSFER, @TGL_TRANSFER, @LOKASI, @TOTAL_QTY, @TOTAL_BARANG, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSFER", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", ModuleAngka.ParseDecimal(LblRecord.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub SimpanSurat_Jalan_Detail(ByVal transaction As MySqlTransaction)
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim sqlrinci As String = "INSERT INTO Transfer_Barang_Detail (ID_TRANSFER, TGL_TRANSFER, LOKASI, ID_BARANG, NAMA_BARANG, HARGA, QTY, SATUAN, ISI_SATUAN, HARGA_QTY, TOTAL_QTY, TOTAL, ID_USER, ID_KOMPUTER) " &
                         "VALUES (@ID_TRANSFER, @TGL_TRANSFER, @LOKASI, @ID_BARANG, @NAMA_BARANG, @HARGA, @QTY, @SATUAN, @ISI_SATUAN, @HARGA_QTY, @TOTAL_QTY, @TOTAL, @ID_USER, @ID_KOMPUTER)"

                Using cmd As New MySqlCommand(sqlrinci, conn, transaction)
                    cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFaktur.Text)
                    cmd.Parameters.AddWithValue("@TGL_TRANSFER", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", If(row.Cells(0).Value IsNot Nothing, row.Cells(0).Value.ToString(), String.Empty))
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", If(row.Cells(1).Value IsNot Nothing, row.Cells(1).Value.ToString(), String.Empty))
                    cmd.Parameters.AddWithValue("@HARGA", If(row.Cells(2).Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells(2).Value), 0D))
                    cmd.Parameters.AddWithValue("@QTY", If(row.Cells(3).Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells(3).Value), 0D))
                    cmd.Parameters.AddWithValue("@SATUAN", If(row.Cells(4).Value IsNot Nothing, row.Cells(4).Value.ToString(), String.Empty))
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", If(row.Cells(5).Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells(5).Value), 0D))
                    cmd.Parameters.AddWithValue("@HARGA_QTY", If(row.Cells(6).Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells(6).Value), 0D))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", If(row.Cells(7).Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells(7).Value), 0D))
                    cmd.Parameters.AddWithValue("@TOTAL", If(row.Cells(8).Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells(8).Value), 0D))
                    cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))

                    cmd.ExecuteNonQuery()
                End Using




                Dim stokKeluarField As String
                Dim stokMasukField As String

                ' Memilih field berdasarkan lokasi barang
                Select Case LblLokasiBarang.Text
                    Case "TOKO"
                        stokKeluarField = "TRANSFER_BARANG_KELUAR_TOKO"
                        stokMasukField = "TRANSFER_BARANG_MASUK_GUDANG"
                    Case "GUDANG"
                        stokKeluarField = "TRANSFER_BARANG_KELUAR_GUDANG"
                        stokMasukField = "TRANSFER_BARANG_MASUK_TOKO"
                    Case Else
                        Throw New Exception("Lokasi barang tidak valid.")
                End Select

                ' Query untuk update stok keluar dan masuk
                Dim updateQuery As String = "UPDATE tbl_barang SET " & stokKeluarField & " = " & stokKeluarField & " + @QtySatKeluar, " & stokMasukField & " = " & stokMasukField & " + @QtySatMasuk WHERE ID_BARANG = @KodeBarang"

                ' Mendapatkan kode barang dari DataGridView
                Dim kodeBarang As String = If(row.Cells("Id").Value IsNot Nothing, row.Cells("Id").Value.ToString(), String.Empty)

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    ' Mendapatkan nilai quantity satuan
                    Dim qtySat As Decimal = If(row.Cells("QtySat").Value IsNot Nothing, ModuleAngka.ParseDecimal(row.Cells("QtySat").Value), 0D)

                    ' Menjalankan query update
                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@QtySatKeluar", qtySat)
                        cmd.Parameters.AddWithValue("@QtySatMasuk", qtySat)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If

            End If
        Next
    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction, ByRef auditHistory As Dictionary(Of String, Decimal))
        Dim LokasiA As String = ""
        Dim LokasiB As String = ""

        Select Case LblLokasiBarang.Text
            Case "TOKO" : LokasiA = "TOKO" : LokasiB = "GUDANG"
            Case "GUDANG" : LokasiA = "GUDANG" : LokasiB = "TOKO"
        End Select

        Dim query As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                              "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                SaveHistory(query, transaction, "TRANSFER BARANG KELUAR", LokasiA, row)
                SaveHistory(query, transaction, "TRANSFER BARANG MASUK", LokasiB, row)

                ' Audit B: qty yang masuk ke HistoryBarang (kolom 7 = TOTAL_QTY)
                Dim kodeB As String = row.Cells(0).Value.ToString()
                Dim qtyB As Decimal = ModuleAngka.ParseDecimal(row.Cells(7).Value)
                If auditHistory.ContainsKey(kodeB) Then auditHistory(kodeB) += qtyB Else auditHistory(kodeB) = qtyB
            End If
        Next
    End Sub

    Private Sub SaveHistory(ByVal query As String, ByVal transaction As MySqlTransaction, ByVal jenis As String, ByVal Lokasi As String, ByVal row As DataGridViewRow)
        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@JENIS", jenis)
            cmd.Parameters.AddWithValue("@LOKASI", Lokasi)
            cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
            cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
            cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells(3).Value))
            cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
            cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells(5).Value))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(row.Cells(7).Value))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(row.Cells(8).Value))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        ' Simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                          "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))

            ' Mengatur Uraian berdasarkan lokasi
            Select Case LblLokasiBarang.Text
                Case "TOKO"
                    cmd.Parameters.AddWithValue("@URAIAN", "Transfer stok barang dari toko ke gudang")
                Case "GUDANG"
                    cmd.Parameters.AddWithValue("@URAIAN", "Transfer stok barang dari gudang ke toko")
                Case Else
                    cmd.Parameters.AddWithValue("@URAIAN", "Transfer stok barang")
            End Select

            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)

            ' Konversi nilai grand total ke Decimal
            cmd.Parameters.AddWithValue("@NOMINAL", ModuleAngka.ParseDecimal(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "TRANSFER BARANG")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)

            ' Penentuan ID_USER dan ID_KOMPUTER berdasarkan jenis transaksi
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.StatusNamaPC.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub AmbilDataUntukEdit()
        Dim queryString As String = "SELECT TGL_TRANSFER, ID_USER, ID_KOMPUTER FROM Transfer_Barang WHERE ID_TRANSFER = ?"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    DTPTgl.Value = CDate(rd("TGL_TRANSFER"))
                    TxtLogin.Text = rd("ID_USER").ToString()
                    TxtKomputer.Text = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using



        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Dim satuanDictionary As New Dictionary(Of String, Tuple(Of String, String, String))()
        Try
            ' Clear DataGridView
            DgvData.Rows.Clear()

            ' Fetch satuan untuk semua barang terlebih dahulu
            Using cmdFetchSatuan As New MySqlCommand("SELECT ID_BARANG, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang", conn)
                cmdFetchSatuan.Transaction = transaction
                Using rdFetchSatuan As MySqlDataReader = cmdFetchSatuan.ExecuteReader()
                    While rdFetchSatuan.Read()
                        Dim idBarang As String = rdFetchSatuan("ID_BARANG").ToString()
                        Dim satuanKecil As String = rdFetchSatuan("SATUAN_UMUM_KECIL").ToString()
                        Dim satuanSedang As String = rdFetchSatuan("SATUAN_UMUM_SEDANG").ToString()
                        Dim satuanBesar As String = rdFetchSatuan("SATUAN_UMUM_BESAR").ToString()

                        If Not satuanDictionary.ContainsKey(idBarang) Then
                            satuanDictionary.Add(idBarang, Tuple.Create(satuanKecil, satuanSedang, satuanBesar))
                        End If
                    End While
                End Using
            End Using

            ' Fetch data untuk DataGridView
            Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, HARGA, QTY, SATUAN, ISI_SATUAN, HARGA_QTY, TOTAL_QTY, TOTAL FROM Transfer_Barang_Detail WHERE ID_TRANSFER= ?", conn)
                cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFaktur.Text)
                cmd.Transaction = transaction

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Do While rd.Read()
                        Dim row As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
                        For i As Integer = 0 To rd.FieldCount - 1
                            row.Cells(i).Value = rd(i)
                        Next i

                        ' Isi ComboBoxCell berdasarkan dictionary
                        Dim idBarang As String = row.Cells(0).Value.ToString()
                        Dim comboCell As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
                        comboCell.Items.Clear()

                        If satuanDictionary.ContainsKey(idBarang) Then
                            Dim satuanTuple = satuanDictionary(idBarang)
                            If Not String.IsNullOrEmpty(satuanTuple.Item1) Then comboCell.Items.Add(satuanTuple.Item1)
                            If Not String.IsNullOrEmpty(satuanTuple.Item2) Then comboCell.Items.Add(satuanTuple.Item2)
                            If Not String.IsNullOrEmpty(satuanTuple.Item3) Then comboCell.Items.Add(satuanTuple.Item3)
                        End If
                    Loop
                End Using
            End Using

            ' Commit transaksi jika berhasil
            transaction.Commit()

            ' Panggil UpdateSemuaTotal() di sini
            UpdateSemuaTotal()

            ' Cek apakah DgvData memiliki baris
            If DgvData.Rows.Count > 0 Then
                ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

                ' Mengatur baris terakhir sebagai baris yang dipilih
                DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
            End If

            If ModulHakAkses.SettingFokusOtomatis Then
                TxtNama.Select()
            End If

        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
            MessageBox.Show("Masalah saat mengambil data. Jenis kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            transaction.Rollback()
        End Try

    End Sub


    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "TransferBarang"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F8      : Simpan transfer barang" & vbCrLf &
                           "ESC     : Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
