Public Class FormTransferBarang
    Private AwalTransfer As String
    Private SatuanTransfer As String
    Private TransferStokMinus As String
    Private TransaksiLampau As String

    Private Sub Formtransferbarang(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Set ukuran maksimum dan minimum untuk memastikan form tidak menutupi taskbar
        MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        MinimumSize = Size

        KosongTxtboxcari()

        AwalTransfer = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransferFocus.Text)
        SatuanTransfer = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransferSatuan.Text)
        TransferStokMinus = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransferMinus.Text)
        TransaksiLampau = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransaksiTanggalLampau.Text)

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

        If AwalTransfer = "Pencarian" Then
            TxtNama.Select()
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


        DTPTgl.Value = DateTime.Now
        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        LblRecord.Text = "Total record : 0"

        NomorTransfer()

    End Sub


    Private Sub NomorTransfer()
        Dim cekTanggal As String = DTPTgl.Value.ToString("yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "TB-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(ID_TRANSFER) FROM Transfer_Barang WHERE ID_TRANSFER LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "TB-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "TB-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "TB-" & cekTanggal & "0001"
        End If

        TxtFaktur.Text = UrutKOde
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
            grandTotal += ParseDecimal(row.Cells("Totalharga").Value)

            ' Jumlah Barang
            Dim qtyValue As Object = row.Cells("Qty").Value
            If qtyValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyValue.ToString()) Then
                totalQtyBarang += ParseDecimal(qtyValue)
                totalRows += 1
            End If

            ' Total QTY
            totalQty += ParseDecimal(row.Cells("QtySat").Value)
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

    Private Function ParseDecimal(ByVal value As Object) As Decimal
        If value Is Nothing Then Return 0D

        Dim s As String = value.ToString().Trim().Replace(",", ".")
        Dim result As Decimal = 0D
        Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result)
        Return result
    End Function


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
        Dim query As String = "SELECT NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR ,STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR TRIM(BARCODE_BESAR) LIKE @Nama ORDER BY NAMA_BARANG"

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
        Dim queryAmbilData As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE NAMA_BARANG = @NAMA"

        Using cmd As New MySqlCommand(queryAmbilData, conn)
            cmd.Parameters.AddWithValue("@NAMA", namayangdiambil)
            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    ' Ambil nilai dari database
                    Dim idBarang As String = If(Not IsDBNull(rd(0)), rd.GetString(0), String.Empty)
                    Dim hargaBeli As String = If(Not IsDBNull(rd(2)), rd.GetDecimal(2).ToString(), String.Empty)

                    Dim satuanUmum As String = If(Not IsDBNull(rd(3)), rd.GetString(3), String.Empty)
                    Dim isiUmum As Integer = If(Not IsDBNull(rd(6)), rd.GetInt32(6), 0)

                    ' Periksa apakah TxtBarcode.Text tidak kosong
                    If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
                        ' Sesuaikan nilai berdasarkan barcode
                        If TxtBarcode.Text = rd("BARCODE_KECIL").ToString() Then
                            satuanUmum = If(Not IsDBNull(rd(3)), rd.GetString(3), String.Empty)
                            isiUmum = If(Not IsDBNull(rd(6)), rd.GetInt32(6), 0)
                        ElseIf TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
                            satuanUmum = If(Not IsDBNull(rd(4)), rd.GetString(4), String.Empty)
                            isiUmum = If(Not IsDBNull(rd(7)), rd.GetInt32(7), 0)
                        ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
                            satuanUmum = If(Not IsDBNull(rd(5)), rd.GetString(5), String.Empty)
                            isiUmum = If(Not IsDBNull(rd(8)), rd.GetInt32(8), 0)
                        End If
                    End If

                    ' Pastikan isiUmum tidak bernilai nol
                    If isiUmum = 0 Then
                        isiUmum = 1
                    End If

                    Dim StokToko As String = If(Not IsDBNull(rd(12)), rd.GetDecimal(12).ToString(), String.Empty)
                    Dim StokGudang As String = If(Not IsDBNull(rd(13)), rd.GetDecimal(13).ToString(), String.Empty)

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
        If SatuanTransfer = "Tidak" Then
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
                        Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                        Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                        Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_BESAR")), "")

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
        Dim hargaBeli As Decimal = Decimal.Parse(TxtHarga.Text)
        Dim qty As Decimal = If(Decimal.TryParse(TxtQty.Text, qty), qty, 1)
        Dim satuan As String = Txtsatuan.Text

        'Dim satuan As String = kolomSatuan.Items(0).ToString()
        Dim isi As Decimal = Decimal.Parse(TxtIsi.Text)
        Dim Stok As Decimal = Decimal.Parse(TxtStok.Text)

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
            "WHERE TRIM(ID_BARANG) LIKE @NamaBarang OR TRIM(NAMA_BARANG) LIKE @NamaBarang " &
            "OR TRIM(BARCODE_KECIL) LIKE @NamaBarang OR TRIM(BARCODE_SEDANG) LIKE @NamaBarang OR TRIM(BARCODE_BESAR) LIKE @NamaBarang"

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

                ' Gabungkan Qty jika SatuanTransfer = Tidak
                If SatuanTransfer = "Tidak" Then
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
        Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @Nama"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & namaValue & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    col.Add(rd("NAMA_BARANG").ToString())
                Loop
            End Using ' rd will be properly disposed here
        End Using ' cmd will be properly disposed here
    End Sub


    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click, BtnClose.Click
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
        If TxtGrandtotal.Text = "" Or Not IsNumeric(TxtGrandtotal.Text) Then
            Txtlihattotal.Text = "0"
            Exit Sub
        Else
            Txtlihattotal.Text = FormatNumber(TxtGrandtotal.Text, 0)
            TxtGrandtotal.Text = TxtGrandtotal.Text
        End If
    End Sub

    Private Sub Form_Pembelian_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
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
                Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE trim(ID_BARANG) like @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            Dim stokToko As Decimal = Convert.ToDecimal(rd("STOK_TOKO"))
                            Dim stokGudang As Decimal = Convert.ToDecimal(rd("STOK_GUDANG"))
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

            If AwalTransfer = "Pencarian" Then
                TxtNama.Select()
                TxtNama.Focus()
                Exit Sub
            End If
        End If

        If TransferStokMinus = "Tidak" Then
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

            If LblJenisTrans.Text <> "TambahTransfer" Then
                HapusUntukEdit(transaction)
            Else
                If TransaksiLampau = "TIDAK" Then
                    DTPTgl.Value = Now
                    NomorTransfer()
                End If
            End If

            SimpanSurat_Jalan(transaction)
            SimpanSurat_Jalan_Detail(transaction)
            HistoryBarang(transaction)

            Simpanjurnal(transaction)

            ' Commit transaksi jika tidak ada kesalahan
            transaction.Commit()

            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    HitungByKode(row.Cells(0).Value)
                End If
            Next

            ' Display a confirmation dialog
            Dim result As DialogResult = MessageBox.Show("Apakah Anda ingin mencetak transfer barang?", "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                ' Proceed with printing
                With PrintTransferBarang
                    .TxtNota.Text = TxtFaktur.Text
                    .ProsesCetak()
                End With
            End If

            DatabaseModule.CatatanAksiHistory("Simpan transfer barang " & TxtFaktur.Text)

            Kondisiawal()



            If LblJenisTrans.Text <> "TambahTransfer" Then
                Close()
            End If
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

        For Each row As DataGridViewRow In FormUtama.DGVDetail.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim qtySat As Decimal = If(row.Cells("TOTAL_QTY").Value IsNot Nothing, Convert.ToDecimal(row.Cells("TOTAL_QTY").Value), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@QtySatKeluar", qtySat)
                        cmd.Parameters.AddWithValue("@QtySatMasuk", qtySat)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using

                    HitungStokPerubahan(kodeBarang, transaction)
                End If
            End If
        Next

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
            cmd.Parameters.AddWithValue("@TOTAL_QTY", CDbl(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", CDbl(LblRecord.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", CDbl(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.Comp.Text, TxtKomputer.Text))
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
                    cmd.Parameters.AddWithValue("@HARGA", If(row.Cells(2).Value IsNot Nothing, Convert.ToDecimal(row.Cells(2).Value), 0D))
                    cmd.Parameters.AddWithValue("@QTY", If(row.Cells(3).Value IsNot Nothing, Convert.ToDecimal(row.Cells(3).Value), 0D))
                    cmd.Parameters.AddWithValue("@SATUAN", If(row.Cells(4).Value IsNot Nothing, row.Cells(4).Value.ToString(), String.Empty))
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", If(row.Cells(5).Value IsNot Nothing, Convert.ToDecimal(row.Cells(5).Value), 0D))
                    cmd.Parameters.AddWithValue("@HARGA_QTY", If(row.Cells(6).Value IsNot Nothing, Convert.ToDecimal(row.Cells(6).Value), 0D))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", If(row.Cells(7).Value IsNot Nothing, Convert.ToDecimal(row.Cells(7).Value), 0D))
                    cmd.Parameters.AddWithValue("@TOTAL", If(row.Cells(8).Value IsNot Nothing, Convert.ToDecimal(row.Cells(8).Value), 0D))
                    cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.SLogin.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.Comp.Text, TxtKomputer.Text))

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
                    Dim qtySat As Decimal = If(row.Cells("QtySat").Value IsNot Nothing, Convert.ToDecimal(row.Cells("QtySat").Value), 0D)

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


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        Dim LokasiA As String = ""
        Dim LokasiB As String = ""

        Select Case LblLokasiBarang.Text
            Case "TOKO"
                LokasiA = "TOKO"
                LokasiB = "GUDANG"
            Case "GUDANG"
                LokasiA = "GUDANG"
                LokasiB = "TOKO"
        End Select

        Dim query As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                              "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                SaveHistory(query, transaction, "TRANSFER BARANG KELUAR", LokasiA, row)
                SaveHistory(query, transaction, "TRANSFER BARANG MASUK", LokasiB, row)
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
            cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells(3).Value))
            cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
            cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells(5).Value))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", Convert.ToDecimal(row.Cells(7).Value))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", Convert.ToDecimal(row.Cells(8).Value))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.Comp.Text, TxtKomputer.Text))
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
            cmd.Parameters.AddWithValue("@NOMINAL", Convert.ToDecimal(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Transfer barang")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)

            ' Penentuan ID_USER dan ID_KOMPUTER berdasarkan jenis transaksi
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahTransfer", FormUtama.Comp.Text, TxtKomputer.Text))

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

            If AwalTransfer = "Pencarian" Then
                TxtNama.Select()
            End If

        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
            MessageBox.Show("Masalah saat mengambil data. Jenis kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            transaction.Rollback()
        End Try

    End Sub


End Class