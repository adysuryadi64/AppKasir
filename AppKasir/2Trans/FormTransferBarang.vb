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
        If ModulHakAkses.SettingSembunyikanPencarianAtas Then
            PanelCari.Visible = False
        Else
            PanelCari.Visible = True
        End If

        If Not ModulHakAkses.SettingTampilInfoStok Then
            DgvData.Columns("Stok").Visible = False
        Else
            DgvData.Columns("Stok").Visible = True
        End If

        If Not ModulHakAkses.SettingIzinkanUbahHargaBeli Then
            DgvData.Columns("Hargabeli").ReadOnly = True
            TxtHarga.ReadOnly = True
        Else
            DgvData.Columns("Hargabeli").ReadOnly = False
            TxtHarga.ReadOnly = False
        End If

        ' Tampilkan ComboBox hanya di cell aktif agar tidak membingungkan
        Dim satuanColTB = TryCast(DgvData.Columns("Satuan"), DataGridViewComboBoxColumn)
        If satuanColTB IsNot Nothing Then
            satuanColTB.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            satuanColTB.DisplayStyleForCurrentCellOnly = True
        End If

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

        _formSudahSiap = False
        AddHandler _searchTimer.Tick, AddressOf SearchTimer_Tick
    End Sub

    Private Sub FormTransferBarang_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        _formSudahSiap = True
        SetupFocusToGrid()
    End Sub

    Public Sub SetupFocusToGrid()
        ' Guard: jika form tidak aktif atau visible, jangan paksa fokus
        If Not Me.Visible OrElse Me.WindowState = FormWindowState.Minimized Then Return
        If Not _formSudahSiap Then Return

        ' MODE 1: Setting Fokus Otomatis (ke TxtNama)
        If ModulHakAkses.SettingFokusOtomatis Then
            _sedangSetFokusAwal = True
            Me.BeginInvoke(New Action(Sub()
                                          TxtNama.Focus()
                                          _sedangSetFokusAwal = False
                                      End Sub))
            Return
        End If

        ' MODE 2: Edit Langsung (ke Grid)
        If DgvData.Rows.Count = 0 Then Return

        Dim targetRow As Integer = 0
        Dim lastFilledRow As Integer = -1

        ' Cari baris terakhir yang terisi (ada Id)
        For i As Integer = DgvData.Rows.Count - 1 To 0 Step -1
            If Not DgvData.Rows(i).IsNewRow Then
                Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                If Not String.IsNullOrEmpty(idVal) Then
                    lastFilledRow = i
                    Exit For
                End If
            End If
        Next

        ' Cari baris kosong setelah baris terakhir yang terisi
        If lastFilledRow >= 0 Then
            Dim foundEmptyRow As Boolean = False
            For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                    If String.IsNullOrEmpty(idVal) Then
                        targetRow = i
                        foundEmptyRow = True
                        Exit For
                    End If
                End If
            Next

            If Not foundEmptyRow Then
                Dim isNewRowIdx As Integer = -1
                For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
                    If DgvData.Rows(i).IsNewRow Then
                        isNewRowIdx = i
                        Exit For
                    End If
                Next
                If isNewRowIdx >= 0 Then
                    targetRow = isNewRowIdx
                Else
                    If DgvData.CurrentCell IsNot Nothing Then
                        targetRow = DgvData.CurrentCell.RowIndex
                    Else
                        Exit Sub
                    End If
                End If
            End If
        Else
            targetRow = 0
        End If

        ' Set CurrentCell dan fokus ke DGV (Race Condition Guard 1 Lapis seperti FormJual)
        If targetRow < DgvData.Rows.Count Then
            Dim targetColumnIndex As Integer = 1 ' Kolom Nama
            Dim targetRowIndex As Integer = targetRow

            DgvData.CurrentCell = DgvData(targetColumnIndex, targetRowIndex)
            Me.ActiveControl = DgvData

            DgvData.BeginInvoke(New Action(Sub()
                                               If DgvData.CurrentCell IsNot Nothing AndAlso
                                                  DgvData.CurrentCell.ColumnIndex = targetColumnIndex AndAlso
                                                  DgvData.CurrentCell.RowIndex = targetRowIndex Then
                                                   DgvData.BeginEdit(True)
                                                   DgvData.EditingControl?.Focus()
                                               End If
                                           End Sub))
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
        ' Jangan ubah fokus grid saat form baru dibuka (initialization)
        If _sedangSetFokusAwal Then Return

        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)

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
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_Panel, ModuleTheme.D_Panel)
    End Sub

    Private Sub KosongTxtboxcari()
        ' Stop search timer — cegah query sisa dari sesi sebelumnya
        _searchTimer.Stop()
        _searchKeywordPending = ""
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHarga.Clear()
        TxtBarcode.Clear()
        TxtNama.Clear()
        TutupListBox()
        _konteksLstBarang = "TXTNAMA"
    End Sub

    Private Sub Kondisiawaledit()

        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
    End Sub

    Private Sub Kondisiawal()
        DgvData.Rows.Clear()
        TxtTotalQTY.Text = 0
        TxtTotalRupiah.Text = ""
        TxtTotalRupiah.Text = 0


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
        SetupFocusToGrid()
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
        TxtTotalRupiah.Text = grandTotal.ToString("N0")
        TxtGrandtotal.Text = "Rp. " & grandTotal.ToString("N0")
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

    ' ── Debounce timer untuk pencarian DGV inline ─────────────────────
    ' Menunda query ke DB sampai user berhenti ketik 150ms — cegah query per keystroke
    Private _searchTimer As New System.Windows.Forms.Timer() With {.Interval = 150}
    Private _searchKeywordPending As String = ""

    ' ===== DGV INLINE EDIT + LISTBOX CONTEXT =====
    Private _dgvEditingTextBox As TextBox = Nothing
    Private _konteksLstBarang As String = "TXTNAMA"  ' "TXTNAMA" atau "DGV"
    Private _sedangSetNilaiDariListBox As Boolean = False
    Private _sedangPindahKeLstBarang As Boolean = False
    Private _teksSebelumPindahKeLstBarang As String = ""
    Private _listBoxDibukaDiRow As Integer = -1
    Private _listBoxDibukaDiCol As Integer = -1
    Private _formSudahSiap As Boolean = False
    Private _sedangSetFokusAwal As Boolean = False

    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        'Deteksi kecepatan input
        Dim currentTime = DateTime.Now
        Dim elapsedMs = (currentTime - lastKeyTime).TotalMilliseconds
        lastKeyTime = currentTime

        'Deteksi barcode (input cepat + Enter)
        If e.KeyCode = Keys.Enter Then
            isBarcodeScan = (elapsedMs < 50) AndAlso (TxtNama.Text.Length >= 5 OrElse TxtNama.Text.All(AddressOf Char.IsDigit))
            suppressTextChanged = True
            _konteksLstBarang = "TXTNAMA"
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
            _konteksLstBarang = "TXTNAMA"
            _teksSebelumPindahKeLstBarang = TxtNama.Text
            LstBarang.Focus()
            LstBarang.SelectedIndex = 0
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Tab Then
            SetupFocusToGrid()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        If suppressTextChanged Then
            suppressTextChanged = False
            Return
        End If
        _konteksLstBarang = "TXTNAMA"
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
        Dim query As String = "SELECT NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @Nama OR NAMA_BARANG LIKE @Nama OR BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama OR BARCODE_BESAR LIKE @Nama) ORDER BY NAMA_BARANG LIMIT 200"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Clear ListBox before adding new items
                LstBarang.Items.Clear()
                TxtBarcode.Clear()

                While rd.Read()
                    Dim itemText As String = rd("NAMA_BARANG").ToString()

                    If ModulHakAkses.SettingTampilInfoStok Then
                        Select Case LblLokasiBarang.Text
                            Case "TOKO"
                                Dim stokToko As Decimal = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                                itemText &= " => " & stokToko.ToString("N0")
                            Case "GUDANG"
                                Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                                itemText &= " => " & stokGudang.ToString("N0")
                        End Select
                    End If

                    ' Check if the searchKeyword matches any barcode field
                    If searchKeyword = rd("BARCODE_SEDANG").ToString() Or searchKeyword = rd("BARCODE_BESAR").ToString() Then
                        TxtBarcode.Text = searchKeyword
                    End If

                    LstBarang.Items.Add(itemText)
                End While

                ' Tampilkan ListBox dan posisikan berdasarkan konteks
                If LstBarang.Items.Count > 0 Then
                    If _konteksLstBarang = "DGV" Then
                        PosisikanLstBarangDiBawahSel()
                        LstBarang.BringToFront()
                    Else
                        ' Konteks TXTNAMA — posisikan di bawah TxtNama
                        PosisikanLstBarangDiBawahTxtNama()
                        LstBarang.BringToFront()
                    End If
                    ' Simpan posisi sel saat ListBox dibuka — untuk guard CellLeave
                    If _konteksLstBarang = "DGV" AndAlso DgvData.CurrentCell IsNot Nothing Then
                        _listBoxDibukaDiRow = DgvData.CurrentCell.RowIndex
                        _listBoxDibukaDiCol = DgvData.CurrentCell.ColumnIndex
                    Else
                        _listBoxDibukaDiRow = -1
                        _listBoxDibukaDiCol = -1
                    End If
                    LstBarang.Visible = True
                Else
                    LstBarang.Visible = False
                    _listBoxDibukaDiRow = -1
                    _listBoxDibukaDiCol = -1
                End If

            End Using
        End Using
    End Sub



    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    _sedangPindahKeLstBarang = True
                    e.SuppressKeyPress = True
                    If _konteksLstBarang = "DGV" Then
                        Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                        _teksSebelumPindahKeLstBarang = ""
                        SetupFocusToGrid()
                        DgvData.BeginInvoke(New Action(Sub()
                                                           If DgvData.CurrentCell IsNot Nothing Then
                                                               DgvData.BeginEdit(True)
                                                               Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                                                               If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                                   editCtrl.Text = teksSimpan
                                                                   editCtrl.SelectionStart = teksSimpan.Length
                                                                   editCtrl.SelectionLength = 0
                                                               End If
                                                               editCtrl?.Focus()
                                                           End If
                                                           _sedangPindahKeLstBarang = False
                                                       End Sub))
                    Else
                        ' Jalur TxtNama — kembalikan fokus ke TxtNama, restore teks
                        Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                        _teksSebelumPindahKeLstBarang = ""
                        TutupListBox()
                        TxtNama.Focus()
                        If Not String.IsNullOrEmpty(teksSimpan) Then
                            TxtNama.Text = teksSimpan
                            TxtNama.SelectionStart = teksSimpan.Length
                            TxtNama.SelectionLength = 0
                        End If
                        _sedangPindahKeLstBarang = False
                    End If
                End If

            Case Keys.Enter
                If LstBarang.SelectedIndex >= 0 Then
                    _sedangPindahKeLstBarang = True
                    AmbilDataDariListBox()
                    _sedangPindahKeLstBarang = False
                End If
                e.SuppressKeyPress = True

            Case Keys.Escape
                TutupListBox()
                If _konteksLstBarang = "DGV" Then
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    SetupFocusToGrid()
                    DgvData.BeginInvoke(New Action(Sub()
                                                       If DgvData.CurrentCell IsNot Nothing Then
                                                           DgvData.BeginEdit(True)
                                                           Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                                                           If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                               editCtrl.Text = teksSimpan
                                                               editCtrl.SelectionStart = teksSimpan.Length
                                                               editCtrl.SelectionLength = 0
                                                           End If
                                                           editCtrl?.Focus()
                                                       End If
                                                       _sedangPindahKeLstBarang = False
                                                   End Sub))
                Else
                    ' Jalur TxtNama — kembalikan fokus ke TxtNama, restore teks
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    TxtNama.Focus()
                    If Not String.IsNullOrEmpty(teksSimpan) Then
                        TxtNama.Text = teksSimpan
                        TxtNama.SelectionStart = teksSimpan.Length
                        TxtNama.SelectionLength = 0
                    End If
                    _sedangPindahKeLstBarang = False
                End If
                e.SuppressKeyPress = True
        End Select
    End Sub

    Private Sub LstBarang_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then
            _sedangPindahKeLstBarang = True
            AmbilDataDariListBox()
            _sedangPindahKeLstBarang = False
        End If
    End Sub

    Private Sub AmbilDataDariListBox()
        ' Reset teks tersimpan
        _teksSebelumPindahKeLstBarang = ""

        Dim selectedValue As String = ""
        If LstBarang.SelectedIndex >= 0 AndAlso LstBarang.SelectedIndex < LstBarang.Items.Count Then
            selectedValue = LstBarang.Items(LstBarang.SelectedIndex).ToString()
        ElseIf LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        End If

        If String.IsNullOrEmpty(selectedValue) Then
            Return
        End If

        ' Extract nama barang — strip stok info setelah " => "
        Dim namayangdiambil As String = selectedValue
        Dim indexArrow As Integer = selectedValue.IndexOf(" => ")
        If indexArrow >= 0 Then
            namayangdiambil = selectedValue.Substring(0, indexArrow).Trim()
        End If

        TutupListBox()

        ' ===== Konteks DGV inline edit =====
        If _konteksLstBarang = "DGV" AndAlso
           DgvData.CurrentCell IsNot Nothing AndAlso DgvData.CurrentCell.ColumnIndex = 1 Then

            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
            If qtyValue <= 0 Then qtyValue = 1D

            Dim barisDiisi As Integer = DgvData.CurrentCell.RowIndex

            ' Guard: cari baris dengan Id kosong pertama (baris yang sedang diedit)
            For i As Integer = 0 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                    If String.IsNullOrEmpty(idVal) Then
                        barisDiisi = i
                        Exit For
                    End If
                End If
            Next

            ' Selesaikan edit mode dulu
            _sedangSetNilaiDariListBox = True
            DgvData.EndEdit(True)
            DgvData.CurrentCell = Nothing

            ' Isi baris DGV langsung
            IsiBarangKeRowDGV(barisDiisi, namayangdiambil, qtyValue)

            _sedangSetNilaiDariListBox = False

            ' Kembali ke input
            KosongTxtboxcari()
            SetupFocusToGrid()
            Return
        End If

        ' ===== Konteks TxtNama (alur lama) =====
        Ambildatalaindaridbbarang(namayangdiambil)
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
                    Dim isiUmum As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))

                    ' Periksa apakah TxtBarcode.Text tidak kosong
                    If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
                        ' Sesuaikan nilai berdasarkan barcode
                        If TxtBarcode.Text = rd("BARCODE_KECIL").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                            isiUmum = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        ElseIf TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", String.Empty)
                            isiUmum = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", String.Empty)
                            isiUmum = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                        End If
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
                    SetupFocusToGrid()
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

        Dim isi As Decimal = Math.Max(1, ModuleAngka.ParseDecimal(TxtIsi.Text))
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
        UpdateWarnaKodeBarang(indeksBaris)

        ' Melakukan pembaruan pada ringkasan atau operasi relevan lainnya
        UpdateSemuaTotal()

        ' Membersihkan field input
        KosongTxtboxcari()

        SetupFocusToGrid()

    End Sub



    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        ' ---> SOLUSI BUG HANDLER: Bersihkan handler TextBox DGV setiap kali selesai edit sel <---
        If _dgvEditingTextBox IsNot Nothing Then
            RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
            RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
            _dgvEditingTextBox = Nothing
        End If

        ' Guard: jangan proses saat ListBox sedang set nilai ke DGV
        If _sedangSetNilaiDariListBox Then Return

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
                            isi = Math.Max(1, isi)
                            row.Cells("isi").Value = isi
                            row.Cells("HargaBeliSat").Value = CDec(row.Cells("Hargabeli").Value) * isi
                            row.Cells("qty").Value = 1
                            row.Cells("QtySat").Value = 1 * isi
                            row.Cells("Totalharga").Value = CDec(row.Cells("Hargabeli").Value) * isi

                            row.Cells("Stok").Value = If(LblLokasiBarang.Text = "TOKO", rd("STOK_TOKO"), rd("STOK_GUDANG"))
                            row.Cells("nama").Value = rd("NAMA_BARANG")
                            UpdateWarnaKodeBarang(e.RowIndex)
                        Else
                            dataTidakDitemukan = True
                        End If
                    End Using
                End Using

                If dataTidakDitemukan Then
                    row.Cells("nama").Value = ""
                    UpdateWarnaKodeBarang(e.RowIndex)
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
                UpdateWarnaKodeBarang(e.RowIndex)
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

    ''' <summary>
    ''' CellEnter — kolom Satuan: langsung BeginEdit dan buka dropdown.
    ''' Kolom Nama: set ReadOnly berdasarkan apakah Id sudah terisi.
    ''' </summary>
    Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellEnter
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Kolom Satuan — buka dropdown langsung agar bisa pilih pakai panah
            If DgvData.Columns(e.ColumnIndex).Name = "Satuan" Then
                DgvData.BeginInvoke(New Action(Sub()
                                                   If DgvData.CurrentCell IsNot Nothing AndAlso
                       DgvData.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
                       DgvData.CurrentCell.RowIndex = e.RowIndex Then
                                                       DgvData.BeginEdit(True)
                                                       Dim combo = TryCast(DgvData.EditingControl, ComboBox)
                                                       If combo IsNot Nothing Then
                                                           combo.DroppedDown = True
                                                       End If
                                                   End If
                                               End Sub))
            End If

            ' Kolom Nama (index 1) — set ReadOnly berdasarkan apakah Id sudah terisi
            If e.ColumnIndex = 1 Then
                Dim idValue = DgvData.Rows(e.RowIndex).Cells("Id").Value
                If idValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(idValue.ToString().Trim()) Then
                    DgvData.Rows(e.RowIndex).Cells("nama").ReadOnly = True
                    DgvData.Rows(e.RowIndex).Cells("nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                    DgvData.Rows(e.RowIndex).Cells("nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                Else
                    DgvData.Rows(e.RowIndex).Cells("nama").ReadOnly = False
                    DgvData.Rows(e.RowIndex).Cells("nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
                    DgvData.Rows(e.RowIndex).Cells("nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                End If
            End If
        End If
    End Sub

    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DgvData.DataError
        e.Cancel = True
    End Sub

    ''' <summary>
    ''' Tutup ListBox otomatis saat user pindah sel di DGV.
    ''' Guard: jangan tutup jika ListBox sedang difokus, sedang transisi, atau masih di sel yang sama.
    ''' </summary>
    Private Sub DgvData_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellLeave
        If Not Me.IsHandleCreated Then Return
        ' BeginInvoke: cek SETELAH fokus benar-benar berpindah.
        ' Tanpa ini: saat user klik ListBox, CellLeave terpicu sebelum ListBox dapat fokus
        ' → LstBarang.Focused masih False → ListBox ditutup sebelum user bisa memilih.
        Me.BeginInvoke(New Action(Sub()
                                      If LstBarang.Visible Then
                                          If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return
                                          If _listBoxDibukaDiRow >= 0 AndAlso
                                             DgvData.CurrentCell IsNot Nothing AndAlso
                                             DgvData.CurrentCell.RowIndex = _listBoxDibukaDiRow AndAlso
                                             DgvData.CurrentCell.ColumnIndex = _listBoxDibukaDiCol Then Return
                                          LstBarang.Visible = False
                                          LstBarang.Items.Clear()
                                          _listBoxDibukaDiRow = -1
                                          _listBoxDibukaDiCol = -1
                                      End If
                                  End Sub))
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
                        DgvData.ClearSelection()
                        UpdateSemuaTotal()
                        SetupFocusToGrid()
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

        ' ===== Kolom Nama (indeks 1): Hook TextChanged + KeyDown untuk ListBox =====
        If DgvData.CurrentCell IsNot Nothing AndAlso DgvData.CurrentCell.ColumnIndex = 1 Then
            ' KRITIS: skip re-attach saat sedang pindah ke ListBox
            ' DGV BeginEdit ulang karena fokus kembali — biarkan handler lama tetap aktif
            If _sedangPindahKeLstBarang Then Return

            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                ' Hapus AutoComplete bawaan — diganti ListBox
                autoText.AutoCompleteMode = AutoCompleteMode.None
                autoText.AutoCompleteSource = AutoCompleteSource.None

                ' Remove handler lama
                If _dgvEditingTextBox IsNot Nothing Then
                    RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                End If
                _dgvEditingTextBox = autoText
                AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                _konteksLstBarang = "DGV"
                PosisikanLstBarangDiBawahSel()
            End If
        Else
            ' Kolom selain Nama — tutup ListBox
            If Not LstBarang.Focused Then
                LstBarang.Visible = False
                LstBarang.Items.Clear()
            End If
        End If

        ' ===== Kolom Satuan ComboBox (indeks 4) =====
        If DgvData.CurrentCell IsNot Nothing AndAlso DgvData.CurrentCell.ColumnIndex = 4 Then
            Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)
            If comboBox IsNot Nothing Then
                RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
            End If
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
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, CInt(rd("ISI_UMUM_KECIL")))
                        Case 1
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, CInt(rd("ISI_UMUM_SEDANG")))
                        Case Else
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, CInt(rd("ISI_UMUM_BESAR")))
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


    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: DGV INLINE EDIT + LISTBOX INTEGRATION
    ' ═══════════════════════════════════════════════════════════════════

    ''' <summary>
    ''' Tangkap keyboard saat ListBox aktif — Enter/Escape/Down dari luar.
    ''' Sama persis dengan pattern FormJual dan FormPembelian.
    ''' </summary>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            Select Case keyData
                Case Keys.Down
                    ' Jika ListBox sudah fokus → biarkan Down diteruskan untuk navigasi normal
                    If LstBarang.Focused Then
                        Return MyBase.ProcessCmdKey(msg, keyData)
                    End If
                    ' Simpan teks sebelum pindah ke ListBox agar bisa di-restore saat Up
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                    Else
                        _teksSebelumPindahKeLstBarang = TxtNama.Text
                    End If
                    _sedangPindahKeLstBarang = True
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    ' Nested BeginInvoke: lapis 1 tunggu CellLeave selesai,
                    ' lapis 2 EndEdit dulu agar DGV tidak merebut fokus kembali, lalu Focus ke ListBox.
                    Me.BeginInvoke(New Action(Sub()
                                                  Me.BeginInvoke(New Action(Sub()
                                                                                If LstBarang.Visible Then
                                                                                    _sedangSetNilaiDariListBox = True
                                                                                    DgvData.EndEdit()
                                                                                    _sedangSetNilaiDariListBox = False
                                                                                    LstBarang.Focus()
                                                                                End If
                                                                                _sedangPindahKeLstBarang = False
                                                                            End Sub))
                                              End Sub))
                    Return True

                Case Keys.Enter
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    _sedangPindahKeLstBarang = True
                    AmbilDataDariListBox()
                    _sedangPindahKeLstBarang = False
                    Return True

                Case Keys.Escape
                    TutupListBox()
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _dgvEditingTextBox.Focus()
                    Else
                        TxtNama.Focus()
                    End If
                    Return True
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ''' <summary>
    ''' Handler TextChanged saat user mengetik di kolom Nama DGV.
    ''' Support format: nama, qty*nama, qty*harga*nama (sama seperti TxtNama ProsesInput)
    ''' </summary>
    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        If _sedangSetNilaiDariListBox Then Return
        _konteksLstBarang = "DGV"

        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return
        Dim currentText = txt.Text.Trim()

        If String.IsNullOrEmpty(currentText) Then
            ' Jangan sembunyikan jika sedang transisi ke ListBox
            If _sedangPindahKeLstBarang OrElse LstBarang.Focused OrElse LstBarang.Visible Then
                Return
            End If
            _searchTimer.Stop()
            _searchKeywordPending = ""
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Hitung jumlah huruf alfabet valid
        Dim validLetters As String = ""
        For Each c As Char In currentText
            If Char.IsLetter(c) Then
                validLetters &= c
            End If
        Next

        ' Lanjutkan hanya jika ada setidaknya 2 huruf valid
        If validLetters.Length < 2 Then Return

        ' Parse format qty*harga*nama atau qty*nama — ekstrak keyword pencarian
        Dim indexAsteriskQty As Integer = currentText.IndexOf("*")
        Dim indexAsteriskHarga As Integer = -1
        If indexAsteriskQty >= 0 Then
            indexAsteriskHarga = currentText.IndexOf("*", indexAsteriskQty + 1)
        End If

        Dim keyword As String = ""

        If indexAsteriskQty >= 0 And indexAsteriskHarga > indexAsteriskQty Then
            ' Dua * : qty*harga*nama
            Dim angkaSebelumAsterisk As String = currentText.Substring(0, indexAsteriskQty).Trim()
            If angkaSebelumAsterisk.Contains(".") Or angkaSebelumAsterisk.Contains(",") Then
                angkaSebelumAsterisk = angkaSebelumAsterisk.Replace(".", ",")
                TxtQty.Text = angkaSebelumAsterisk
            ElseIf Decimal.TryParse(angkaSebelumAsterisk, Nothing) Then
                TxtQty.Text = angkaSebelumAsterisk
            Else
                TxtQty.Text = "1"
            End If

            Dim hargaSebelumAsterisk As String = currentText.Substring(indexAsteriskQty + 1, indexAsteriskHarga - indexAsteriskQty - 1).Trim()
            TxtHarga.Text = hargaSebelumAsterisk

            keyword = currentText.Substring(indexAsteriskHarga + 1).Trim()

        ElseIf indexAsteriskQty >= 0 Then
            ' Satu * : qty*nama
            Dim angkaSebelumAsterisk As String = currentText.Substring(0, indexAsteriskQty).Trim()
            If angkaSebelumAsterisk.Contains(".") Or angkaSebelumAsterisk.Contains(",") Then
                angkaSebelumAsterisk = angkaSebelumAsterisk.Replace(".", ",")
                TxtQty.Text = angkaSebelumAsterisk
            ElseIf Decimal.TryParse(angkaSebelumAsterisk, Nothing) Then
                TxtQty.Text = angkaSebelumAsterisk
            Else
                TxtQty.Text = "1"
            End If

            keyword = currentText.Substring(indexAsteriskQty + 1).Trim()

        Else
            ' Tanpa * : langsung nama
            keyword = currentText
            TxtQty.Text = "1"
        End If

        ' Debounce: tunda query sampai user berhenti ketik 150ms
        If Not String.IsNullOrEmpty(keyword) Then
            _searchKeywordPending = keyword
            _searchTimer.Stop()
            _searchTimer.Start()
        End If
    End Sub

    ''' <summary>Dipanggil oleh _searchTimer setelah debounce 150ms.</summary>
    Private Sub SearchTimer_Tick(sender As Object, e As EventArgs)
        _searchTimer.Stop()
        If Not String.IsNullOrEmpty(_searchKeywordPending) Then
            TampilkanDaftarBarang(_searchKeywordPending)
            _searchKeywordPending = ""
        End If
    End Sub

    ''' <summary>
    ''' Handler KeyDown saat user mengetik di kolom Nama DGV.
    ''' Handle Down (pindah ke ListBox), Enter (ambil item), Escape (tutup ListBox).
    ''' </summary>
    Private Sub DgvNamaBarang_KeyDown(sender As Object, e As KeyEventArgs)
        If Not LstBarang.Visible OrElse LstBarang.Items.Count = 0 Then Return

        Select Case e.KeyCode
            Case Keys.Down
                If _dgvEditingTextBox IsNot Nothing Then
                    _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                End If
                _sedangPindahKeLstBarang = True
                If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                ' Nested BeginInvoke: EndEdit dulu agar DGV tidak merebut fokus, lalu Focus ke ListBox
                Me.BeginInvoke(New Action(Sub()
                                              Me.BeginInvoke(New Action(Sub()
                                                                            If LstBarang.Visible Then
                                                                                _sedangSetNilaiDariListBox = True
                                                                                DgvData.EndEdit()
                                                                                _sedangSetNilaiDariListBox = False
                                                                                LstBarang.Focus()
                                                                            End If
                                                                            _sedangPindahKeLstBarang = False
                                                                        End Sub))
                                          End Sub))
                e.SuppressKeyPress = True

            Case Keys.Enter
                If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                _sedangPindahKeLstBarang = True
                AmbilDataDariListBox()
                _sedangPindahKeLstBarang = False
                e.SuppressKeyPress = True

            Case Keys.Escape
                TutupListBox()
                e.SuppressKeyPress = True
        End Select
    End Sub

    ''' <summary>Warnai kolom "nama": abu (ReadOnly) jika Id sudah terisi, putih (editable) jika kosong.</summary>
    Private Sub UpdateWarnaKodeBarang(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= DgvData.Rows.Count Then Return
        If DgvData.Rows(rowIndex).IsNewRow Then Return
        Dim cell = DgvData.Rows(rowIndex).Cells("nama")
        Dim kodeValue = DgvData.Rows(rowIndex).Cells("Id").Value
        Dim adaId As Boolean = kodeValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(kodeValue.ToString().Trim())
        If adaId Then
            cell.ReadOnly = True
            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
            cell.Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
        Else
            cell.ReadOnly = False
            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
            cell.Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
        End If
    End Sub

    ''' <summary>Tutup ListBox dan reset semua state terkait posisi.</summary>
    Private Sub TutupListBox()
        LstBarang.Visible = False
        LstBarang.Items.Clear()
        _listBoxDibukaDiRow = -1
        _listBoxDibukaDiCol = -1
    End Sub

    ''' <summary>Posisikan ListBox tepat di bawah TxtNama (konteks pencarian atas).</summary>
    Private Sub PosisikanLstBarangDiBawahTxtNama()
        Try
            Dim ptTxt = TxtNama.PointToScreen(New Point(0, TxtNama.Height))
            Dim ptForm = Me.PointToClient(ptTxt)
            LstBarang.Location = New Point(ptForm.X, ptForm.Y)
            LstBarang.Width = Math.Max(300, TxtNama.Width)
        Catch
        End Try
    End Sub

    ''' <summary>Posisikan ListBox tepat di bawah sel DGV yang aktif.</summary>
    Private Sub PosisikanLstBarangDiBawahSel()
        If DgvData.CurrentCell Is Nothing Then Return
        Try
            Dim cellRect = DgvData.GetCellDisplayRectangle(
                DgvData.CurrentCell.ColumnIndex, DgvData.CurrentCell.RowIndex, True)
            Dim ptDgv = DgvData.PointToScreen(New Point(cellRect.Left, cellRect.Bottom))
            Dim ptForm = Me.PointToClient(ptDgv)
            LstBarang.Location = New Point(ptForm.X, ptForm.Y)
            LstBarang.Width = Math.Max(300, cellRect.Width)
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Isi baris DGV yang sudah ada dengan data barang dari database.
    ''' Digunakan oleh AmbilDataDariListBox konteks DGV.
    ''' Logic sama dengan CellEndEdit kolom 1 + TambahDataLangsung tapi untuk existing row.
    ''' </summary>
    Private Sub IsiBarangKeRowDGV(rowIndex As Integer, namaBarang As String, qty As Decimal)
        If rowIndex < 0 OrElse rowIndex >= DgvData.Rows.Count Then Return
        Dim row = DgvData.Rows(rowIndex)

        ' Cek duplikat jika tidak izinkan satuan berbeda
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
            ' Cari ID_BARANG dulu untuk cek
            Dim idBarangCek As String = ""
            Using cmdCek As New MySqlCommand("SELECT ID_BARANG FROM tbl_barang WHERE STATUS = 'Aktif' AND NAMA_BARANG = @NAMA", conn)
                cmdCek.Parameters.AddWithValue("@NAMA", namaBarang)
                Dim result = cmdCek.ExecuteScalar()
                If result IsNot Nothing Then idBarangCek = result.ToString()
            End Using

            If Not String.IsNullOrEmpty(idBarangCek) Then
                For Each existingRow As DataGridViewRow In DgvData.Rows
                    If existingRow.Index <> rowIndex AndAlso existingRow.Cells("Id").Value IsNot Nothing AndAlso existingRow.Cells("Id").Value.ToString() = idBarangCek Then
                        MessageBox.Show(namaBarang & " sudah ada dalam daftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                Next
            End If
        End If

        ' Query DB untuk data barang
        Dim queryAmbilData As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE STATUS = 'Aktif' AND NAMA_BARANG = @NAMA"

        Using cmd As New MySqlCommand(queryAmbilData, conn)
            cmd.Parameters.AddWithValue("@NAMA", namaBarang)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim idBarang As String = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                    Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))

                    Dim satuanUmum As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                    Dim isiUmum As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))

                    ' Periksa apakah TxtBarcode tidak kosong — sesuaikan satuan berdasarkan barcode
                    If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
                        If TxtBarcode.Text = rd("BARCODE_KECIL").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                            isiUmum = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        ElseIf TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", String.Empty)
                            isiUmum = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", String.Empty)
                            isiUmum = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                        End If
                    End If

                    Dim StokToko As Decimal = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    Dim StokGudang As Decimal = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))

                    ' Harga manual dari TxtHarga (jika user input qty*harga*nama)
                    Dim hargaManual As Decimal = ModuleAngka.ParseDecimal(TxtHarga.Text)
                    If hargaManual > 0 Then hargaBeli = hargaManual

                    ' Isi ComboBox satuan
                    Dim kolomSatuan As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
                    kolomSatuan.Items.Clear()

                    Dim satuanKecil As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                    Dim satuanSedang As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                    Dim satuanBesar As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")

                    If Not String.IsNullOrEmpty(satuanKecil) Then kolomSatuan.Items.Add(satuanKecil)
                    If Not String.IsNullOrEmpty(satuanSedang) Then kolomSatuan.Items.Add(satuanSedang)
                    If Not String.IsNullOrEmpty(satuanBesar) Then kolomSatuan.Items.Add(satuanBesar)

                    ' Set nilai baris
                    row.Cells("Id").Value = idBarang
                    row.Cells("nama").Value = namaBarang
                    row.Cells("Hargabeli").Value = hargaBeli
                    row.Cells("qty").Value = qty
                    row.Cells("Satuan").Value = satuanUmum
                    row.Cells("isi").Value = Math.Max(1, isiUmum)
                    row.Cells("HargaBeliSat").Value = hargaBeli * isiUmum
                    row.Cells("QtySat").Value = qty * isiUmum
                    row.Cells("Totalharga").Value = qty * isiUmum * hargaBeli

                    Select Case LblLokasiBarang.Text
                        Case "TOKO"
                            row.Cells("Stok").Value = StokToko
                        Case "GUDANG"
                            row.Cells("Stok").Value = StokGudang
                    End Select

                    UpdateWarnaKodeBarang(rowIndex)
                    UpdateSemuaTotal()
                End If
            End Using
        End Using
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

    Private Sub TxtGrandtotal_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotalRupiah.TextChanged
        Dim grandTotal As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)
        TxtGrandtotal.Text = ModuleAngka.FormatRupiah(grandTotal)
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
                            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowError, ModuleTheme.D_DgvRowError)
                        Next

                        ' Setelah menyaring baris, pastikan bahwa baris tersebut terpilih juga
                        SetupFocusToGrid()
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
        If TxtTotalRupiah.Text = "0" OrElse DgvData.RowCount = 0 Then
            MessageBox.Show("Belum ada transaksi Pembelian", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            If DgvData.Rows.Count > 0 AndAlso DgvData.Columns.Count > 1 Then
                DgvData.CurrentCell = DgvData(1, 0)
                DgvData.Rows(0).Selected = True
            End If

            SetupFocusToGrid()
            Exit Sub
        End If

        If Not ModulHakAkses.SettingIzinkanBarangMinus Then
            If CekStok() Then
                Return
            End If

            ' ═══════════════════════════════════════════════════════════════
            ' VALIDASI LEVEL 8: CEK STOK REAL-TIME VIA SP (ANTI RACE CONDITION)
            ' ═══════════════════════════════════════════════════════════════
            For Each dgvRow As DataGridViewRow In DgvData.Rows
                If Not dgvRow.IsNewRow AndAlso
                   dgvRow.Cells("Id").Value IsNot Nothing AndAlso
                   Not String.IsNullOrEmpty(dgvRow.Cells("Id").Value.ToString()) Then

                    Dim kodeBarang As String = dgvRow.Cells("Id").Value.ToString()
                    Dim qtySat As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("QtySat").Value)
                    Dim namaBarang As String = Convert.ToString(dgvRow.Cells("Nama").Value)

                    Dim qtyDibutuhkan As Decimal = qtySat
                    If LblJenisTrans.Text <> "TambahTransfer" Then
                        Try
                            Using cmdQtyLama As New MySqlCommand(
                                "SELECT COALESCE(SUM(TOTAL_QTY), 0) FROM Transfer_barang_Detail " &
                                "WHERE ID_TRANSFER = @fk AND ID_BARANG = @id", conn)
                                cmdQtyLama.Parameters.AddWithValue("@fk", TxtFaktur.Text)
                                cmdQtyLama.Parameters.AddWithValue("@id", kodeBarang)
                                Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(cmdQtyLama.ExecuteScalar())
                                qtyDibutuhkan = Math.Max(0D, qtySat - qtyLama)
                            End Using
                        Catch
                            ' Jika gagal baca qty lama, pakai qty penuh sebagai aman
                        End Try
                    End If

                    If qtyDibutuhkan <= 0 Then Continue For

                    Try
                        Using cmdSP As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan, @errcode, @errmsg)", conn)
                            cmdSP.Parameters.AddWithValue("@kode", kodeBarang)
                            cmdSP.Parameters.AddWithValue("@qty", qtyDibutuhkan)
                            cmdSP.Parameters.AddWithValue("@lokasi", LblLokasiBarang.Text)
                            cmdSP.Parameters.AddWithValue("@izinkan", 0) ' 0 = tidak izinkan minus

                            Dim pErrCode = cmdSP.Parameters.Add("@errcode", MySqlDbType.VarChar, 50)
                            pErrCode.Direction = ParameterDirection.Output
                            Dim pErrMsg = cmdSP.Parameters.Add("@errmsg", MySqlDbType.VarChar, 255)
                            pErrMsg.Direction = ParameterDirection.Output

                            cmdSP.ExecuteNonQuery()

                            Dim errCode As String = pErrCode.Value?.ToString()
                            If Not String.IsNullOrEmpty(errCode) Then
                                MessageBox.Show(
                                    "⚠️ Stok berubah sejak form dibuka!" & vbCrLf & vbCrLf &
                                    "Barang: " & namaBarang & vbCrLf &
                                    "Pesan: " & pErrMsg.Value?.ToString() & vbCrLf & vbCrLf &
                                    "Silakan periksa kembali daftar barang sebelum menyimpan.",
                                    "Stok Tidak Mencukupi (Real-time)",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning)

                                ' Sorot baris yang bermasalah
                                dgvRow.Selected = True
                                For Each cell As DataGridViewCell In dgvRow.Cells
                                    cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowError, ModuleTheme.D_DgvRowError)
                                Next
                                dgvRow.DataGridView.CurrentCell = dgvRow.Cells("Nama")

                                Exit Sub
                            End If
                        End Using
                    Catch ex As Exception
                        MessageBox.Show("Gagal memvalidasi stok real-time: " & ex.Message, "Error Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End Try
                End If
            Next
        End If

        ProsesSimpan()
    End Sub

    Public Sub ProsesSimpan()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        ' Mengubah kursor menjadi menunggu
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Try

            If LblJenisTrans.Text <> "TambahTransfer" Then
                ' ========================================
                ' START: Audit Trail - Edit Transfer Barang (snapshot SEBELUM hapus)
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

                ' HapusUntukEdit memanggil ModuleHapusTransaksi.HapusTransferBarang
                ' yang sudah menangani: reversal counter stok, HitungStokPerubahan,
                ' AuditStokTransaksi, DELETE 4 tabel, dan ReversalSaldoAkunDariFaktur.
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
            ' UPDATE saldo untuk akun BARU (akun lama sudah di-update oleh HapusUntukEdit)
            ' ========================================
            ' Update saldo akun — incremental delta
            UpdateSaldoAkunDeltaDariFaktur(TxtFaktur.Text, transaction)

            ' Commit transaksi jika tidak ada kesalahan
            transaction.Commit()

            Dim tbNominal As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)
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



    ''' <summary>
    ''' Hapus data transfer barang lama sebagai langkah awal proses edit.
    ''' Wrapper tipis ke ModuleHapusTransaksi.HapusTransferBarang — logika ada di modul.
    ''' ReversalSaldoAkunDariFaktur untuk akun lama sudah ditangani di dalam HapusTransferBarang.
    ''' </summary>
    Private Sub HapusUntukEdit(ByVal transaction As MySqlTransaction)
        ModuleHapusTransaksi.HapusTransferBarang(
            TxtFaktur.Text,
            LblLokasiBarang.Text,
            TxtFaktur.Text & " [HAPUS-EDIT]",
            transaction)
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
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtTotalRupiah.Text))
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
            cmd.Parameters.AddWithValue("@NOMINAL", ModuleAngka.ParseDecimal(TxtTotalRupiah.Text))
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
                    DTPTgl.Enabled = True  ' Mode edit: selalu bisa ubah tanggal, tanggal lama bisa lampau
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

                        UpdateWarnaKodeBarang(row.Index)
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

            SetupFocusToGrid()

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

    Private Sub BtnKeluar_Click_1(sender As Object, e As EventArgs) Handles BtnKeluar.Click
        Me.Close()
    End Sub
End Class
