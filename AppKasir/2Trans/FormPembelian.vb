Public Class FormPembelian
    Private ReadOnly tempatSimpan As String

    Private awalpembelian As String
    Private Edithargabeli As String
    Private MunculHargaJual As String
    Private Kodebarangsama As String
    Private UpdateHargabeli As String
    Private Updateberdasarkan As String
    Private TransaksiLampau As String

    Private Sub Form_Pembelian_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        LblLokasiBarang.Text = FormUtama.SLokasi.Text

        ' Set ukuran maksimum dan minimum untuk memastikan form tidak menutupi taskbar
        MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        MinimumSize = Size


        KosongTxtboxcari()

        ' Setup timer untuk deteksi barcode
        barcodeTimer.Interval = 100 ' 100ms timeout
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick


        awalpembelian = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliFokus.Text)
        Edithargabeli = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliEditHarga.Text)
        MunculHargaJual = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliMuculJual.Text)
        Kodebarangsama = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliSatuan.Text)
        UpdateHargabeli = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliUpdate.Text)
        Updateberdasarkan = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliAverage.Text)
        TransaksiLampau = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransaksiTanggalLampau.Text)

        LblUpdateHarga.Text = "Metode update hpp : " & UpdateHargabeli & " dari stok " & Updateberdasarkan
        ' Memastikan bahwa BacaHakAksesSemua mengembalikan sebuah String yang sesuai

        If Edithargabeli = "Tidak" Then
            DgvData.Columns("Hargabeli").ReadOnly = True
        Else
            DgvData.Columns("Hargabeli").ReadOnly = False
        End If

        If TxtJenisTrans.Text = "TambahPembelian" Then
            TxtSupplier.Clear()
            KosongkanDataSupplier()
            ' Hapus semua item dan tambahkan yang baru
            IsiComboBoxAkun(CmbJenisBayar, "KAS", "BANK", "EKUITAS")

            If LblLokasiBarang.Text = "TOKO" Then
                CmbJenisBayar.SelectedItem = nama_rek_Beli_toko
            ElseIf LblLokasiBarang.Text = "GUDANG" Then
                CmbJenisBayar.SelectedItem = nama_rek_Beli_Gudang
            End If
            Kondisiawal()
            AmbilKodeAkun()
        Else
            Kondisiawaledit()
            AmbilDataPembelian()
            AmbilDaftarBarangEditpembelian()
        End If
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
        TxtLevelSat.Clear()
        TxtStokToko.Clear()
        TXtStokGudang.Clear()
    End Sub

    Private Sub Kondisiawaledit()
        GBBayar.Visible = False

        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        DTPJatuhTempo.Format = DateTimePickerFormat.Custom
        DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"
        TxtBayar.Text = 0
    End Sub

    Private Sub Kondisiawal()
        DgvData.Rows.Clear()
        TxtSupplier.Clear()
        TxtBayar.Text = 0
        TxtKembali.Clear()
        TxtJmlhBrg.Clear()
        TxtBAntuanbayar.Clear()
        TxtTotal.Clear()
        TxtGrandtotal.Text = 0
        GBBayar.Visible = False

        DTPTgl.Value = DateTime.Now
        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        Dim newDate As Date = DTPJatuhTempo.Value.AddMonths(1)
        DTPJatuhTempo.Value = newDate
        DTPJatuhTempo.Format = DateTimePickerFormat.Custom
        DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"

        LblRecord.Text = "Total record : 0"

        NomorBeli()
        AmbilDataSupplier()

        SetupFocusToGrid()
    End Sub


    Public Sub SetupFocusToGrid()
        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom Nama (index 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True

            ' ✅ BEHAVIOR BERDASARKAN awalpembelian
            If awalpembelian = "Pencarian" Then
                ' MODE 1: Pencarian - fokus ke TxtNama (input manual/barcode)
                TxtNama.Select()
                TxtNama.Focus()
            Else
                ' MODE 2: Edit Langsung - fokus ke sel Nama untuk edit inline
                DgvData.Select()
                DgvData.Focus()
                DgvData.BeginEdit(True) ' Mulai edit mode
            End If
        End If
    End Sub

    Private SkipValidation As Boolean = False




    Public Sub NomorBeli()
        Dim cekTanggal As String = DTPTgl.Value.ToString("yyMMdd")
        Dim ceknomor As String = "PB-" & cekTanggal
        Dim UrutKode As String = ""

        Using cmd As New MySqlCommand("SELECT MAX(ID_PEMBELIAN) FROM pembelian WHERE ID_PEMBELIAN LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim maxKode As Object = rd.GetValue(0)
                    If Not IsDBNull(maxKode) Then
                        Dim maxKodePenjualan As String = maxKode.ToString()
                        If Not String.IsNullOrEmpty(maxKodePenjualan) Then
                            If Microsoft.VisualBasic.Left(maxKodePenjualan, 9) = "PB-" & cekTanggal Then
                                Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(maxKodePenjualan, 4)) + 1
                                UrutKode = "PB-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                            End If
                        End If
                    End If
                End If
            End Using
        End Using

        Using cmdTemp As New MySqlCommand("SELECT MAX(ID_PEMBELIAN) FROM pembelian_ditahan WHERE ID_PEMBELIAN LIKE @ceknomor", conn)
            cmdTemp.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Using rdTemp As MySqlDataReader = cmdTemp.ExecuteReader()
                If rdTemp.Read() Then
                    Dim maxKodeTemp As Object = rdTemp.GetValue(0)
                    If Not IsDBNull(maxKodeTemp) Then
                        Dim maxKode As String = maxKodeTemp.ToString()
                        If Not String.IsNullOrEmpty(maxKode) Then
                            If Microsoft.VisualBasic.Left(maxKode, 9) = "PB-" & cekTanggal Then
                                Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(maxKode, 4)) + 1
                                If UrutKode = "" Then
                                    UrutKode = "PB-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                                Else
                                    Dim maxKodeUrut As String = "PB-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
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
            UrutKode = "PB-" & cekTanggal & "0001"
        End If

        TxtFaktur.Text = UrutKode
    End Sub

    Private Sub Hapusbaris()
        ' Periksa apakah ada sel yang dipilih
        If DgvData.CurrentCell Is Nothing Then
            MsgBox("Tidak ada baris yang dipilih.", vbExclamation, "Peringatan")
            Return
        End If

        Dim baris As Integer = DgvData.CurrentCell.RowIndex

        ' Periksa apakah baris yang dipilih adalah baris baru
        If DgvData.Rows(baris).IsNewRow Then
            MsgBox("Baris baru tidak dapat dihapus.", vbExclamation, "Peringatan")
            Return
        End If

        ' Periksa apakah sel dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            MsgBox("Tidak dapat menghapus baris dalam mode edit.", vbExclamation + vbCritical, "Mode Edit Aktif")
            Return
        End If

        ' Konfirmasi penghapusan
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus baris ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            ' Hapus baris jika pengguna menekan Yes
            DgvData.Rows.RemoveAt(baris)
            UpdateSemuaTotal()
        End If
    End Sub

    Private Sub KosongkanDataSupplier()
        LblKodeSupplier.Text = ""
        LblAlamatSupplier.Text = ""
        LblKontakSupplier.Text = ""
        DTPJatuhTempo.Value = DTPTgl.Value.AddMonths(1)
    End Sub


    Public Class DataSupplier
        Public Property Kode As String
        Public Property Nama As String
        Public Property Alamat As String
        Public Property HP As String
        Public Property JangkaHutang As Integer

        Public Overrides Function ToString() As String
            Return Nama   ' <<< LISTBOX TAMPIL NAMA SAJA
        End Function
    End Class

    Dim IsSelectingSupplier As Boolean = False

    Dim ListDataSupplier As New List(Of DataSupplier)

    Public Sub AmbilDataSupplier()
        ListDataSupplier.Clear()

        Using cmd As New MySqlCommand("SELECT KODE, NAMA, ALAMAT, HP, JangkaHutang FROM tbl_supliyer", conn)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    ListDataSupplier.Add(New DataSupplier With {
                    .Kode = rd.GetString(0),
                    .Nama = rd.GetString(1),
                    .Alamat = rd.GetString(2),
                    .HP = rd.GetString(3),
                    .JangkaHutang = rd.GetInt32(4)
                })
                End While
            End Using
        End Using
    End Sub

    Private Sub FilterSupplier()
        If IsSelectingSupplier Then Exit Sub   ' <<< stop loop

        Dim filter As String = TxtSupplier.Text.Trim().ToLower()

        listSupplier.Items.Clear()

        ' Jika textbox kosong → semua data supplier hilang
        If filter = "" Then
            listSupplier.Visible = False
            KosongkanDataSupplier()
            Exit Sub
        End If

        Dim hasil = ListDataSupplier.
            Where(Function(x) x.Nama.ToLower().Contains(filter) _
                          Or x.HP.ToLower().Contains(filter)).
            ToList()

        If hasil.Count = 0 Then
            listSupplier.Visible = False
            Exit Sub
        End If

        If TxtJenisTrans.Text <> "TambahPembelian" Then
            If hasil.Count = 1 Then
                PilihSupplierLangsung(hasil(0), False)   ' ← tetap di txtSupplier
                Exit Sub
            End If
        End If


        For Each s In hasil
            listSupplier.Items.Add(s)
        Next
        AturTinggiListSupplier()
        ' Pastikan tetap di depan setelah ditampilkan
        listSupplier.BringToFront()
        listSupplier.Visible = True

    End Sub

    Private Sub AturTinggiListSupplier()
        Dim baris As Integer = listSupplier.Items.Count

        If baris = 0 Then
            listSupplier.Height = 0
            Return
        End If

        Dim tinggiBaris As Integer = listSupplier.ItemHeight

        If baris <= 20 Then
            listSupplier.Height = baris * tinggiBaris + 4
            listSupplier.ScrollAlwaysVisible = False
        Else
            listSupplier.Height = 20 * tinggiBaris + 4
            listSupplier.ScrollAlwaysVisible = True
        End If
    End Sub


    Private Sub PilihSupplierLangsung(s As DataSupplier, Optional PindahKeBarang As Boolean = False)
        IsSelectingSupplier = True

        TxtSupplier.Text = s.Nama
        LblKodeSupplier.Text = s.Kode
        LblAlamatSupplier.Text = s.Alamat
        LblKontakSupplier.Text = s.HP
        DTPJatuhTempo.Value = DTPTgl.Value.AddDays(s.JangkaHutang)

        listSupplier.Items.Clear()
        listSupplier.Visible = False

        TxtSupplier.Focus()
        TxtSupplier.SelectionStart = TxtSupplier.Text.Length

        IsSelectingSupplier = False

        If PindahKeBarang Then SetupFocusToGrid()   ' ← Ringkas & efisien
    End Sub


    Private Sub PilihSupplier()
        If listSupplier.SelectedItem Is Nothing Then Exit Sub

        Dim s As DataSupplier = CType(listSupplier.SelectedItem, DataSupplier)

        PilihSupplierLangsung(s)
    End Sub

    Private Sub TxtSupplier_TextChanged(sender As Object, e As EventArgs) Handles TxtSupplier.TextChanged
        FilterSupplier()
    End Sub

    Private Sub TxtSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtSupplier.KeyDown
        ' Tekan delete → hapus semua data supplier
        If e.KeyCode = Keys.Delete Then
            TxtSupplier.Clear()
            KosongkanDataSupplier()
            listSupplier.Visible = False
            Exit Sub
        End If

        If (e.KeyCode = Keys.Down Or e.KeyCode = Keys.Enter) _
        AndAlso listSupplier.Items.Count > 0 Then

            listSupplier.Focus()
            listSupplier.SelectedIndex = 0
        End If
    End Sub

    Private Sub listSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles listSupplier.KeyDown
        If e.KeyCode = Keys.Enter Then
            PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
        End If
    End Sub

    Private Sub listSupplier_Click(sender As Object, e As EventArgs) Handles listSupplier.Click
        PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
    End Sub

    Private Sub CmbJenisBayar_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbJenisBayar.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtBayar.Select()
        End If
    End Sub


    Private Sub CmbJenisBayar_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbJenisBayar.SelectedIndexChanged
        AmbilKodeAkun()
    End Sub

    Private Sub AmbilKodeAkun()
        Dim namaAkunD As String = CmbJenisBayar.Text

        Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtKodeRek.Text = reader("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub


    Public Sub UpdateSemuaTotal()
        ' Hitung Grand Total
        Dim grandTotal As Decimal = 0
        For i As Integer = 0 To DgvData.Rows.Count - 1
            grandTotal += Convert.ToDecimal(DgvData.Rows(i).Cells("Totalharga").Value)
        Next
        TxtGrandtotal.Text = grandTotal.ToString()


        ' Hitung Jumlah Barang
        Dim totalRows As Integer = 0 ' Inisialisasi jumlah baris

        ' Loop melalui setiap baris di DataGridView
        For i As Integer = 0 To DgvData.Rows.Count - 1
            ' Periksa apakah nilai di kolom "Qty" bukan null atau kosong
            Dim qtyValue As Object = DgvData.Rows(i).Cells("Qty").Value
            If Not DgvData.Rows(i).IsNewRow AndAlso qtyValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyValue.ToString()) Then
                ' Tambah satu pada jumlah baris
                totalRows += 1
            End If
        Next

        ' Setel nilai TextBox TxtJmlhBrg dengan hasil totalQtyBarang
        TxtJmlhBrg.Text = totalRows.ToString()

        ' Setel nilai TextBox TxtJmlhBaris dengan jumlah baris
        LblRecord.Text = "Total record : " & totalRows.ToString()


        ' Hitung Total QTY
        Dim totalQty As Decimal = 0
        For i As Integer = 0 To DgvData.Rows.Count - 1
            totalQty += Convert.ToDecimal(DgvData.Rows(i).Cells("QtySat").Value)
        Next
        TxtTotalQTY.Text = totalQty.ToString()

        Txtlihattotal.Text = "Rp. " & FormatNumber(grandTotal, 0)

        ' Pengaturan agar DataGridView selalu tampil dengan baris terakhir
        DgvData.FirstDisplayedScrollingRowIndex = DgvData.Rows.Count - 1
    End Sub

    ' ===== ADD SETELAH CLASS DECLARATION =====
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private isBarcodeMode As Boolean = False
    Private barcodeTimer As New System.Windows.Forms.Timer()

    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100

    Private suppressTextChanged As Boolean = False  ' ✅ TAMBAHKAN INI DI SINI
    Private isBarcodeInput As Boolean = False  ' ← TAMBAHKAN FLAG INI


    ' --- Replace existing TxtNama_KeyDown, TxtNama_TextChanged, ProcessInput and add barcode timer handlers/helpers ---
    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        ' ===== SPECIAL KEYS =====
        If e.KeyCode = Keys.Down AndAlso LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            LstBarang.Focus()
            LstBarang.SelectedIndex = 0
            e.SuppressKeyPress = True
            Return
        End If

        If e.KeyCode = Keys.Tab Then
            DgvData.Select()
            DgvData.Focus()
            e.SuppressKeyPress = True
            Return
        End If

        ' ===== PRINTABLE CHARACTERS =====
        Dim ch As Char = ChrW(e.KeyCode)
        If Not Char.IsControl(ch) Then
            ' If user types a letter or '*' -> manual input, cancel barcode detection
            If ch = "*"c OrElse Char.IsLetter(ch) Then
                ResetBarcodeDetection()
                Return
            End If

            Dim currentTime = DateTime.Now

            ' First character
            If barcodeChars.Count = 0 Then
                barcodeStartTime = currentTime
                barcodeChars.Add(ch)
                lastKeyTime = currentTime

                barcodeTimer.Interval = 100
                barcodeTimer.Stop()
                barcodeTimer.Start()
                Return
            End If

            ' Interval since last key
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds

            ' If slow typing -> not barcode
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then
                isBarcodeMode = False
            End If

            If barcodeChars.Count < BARCODE_MAX_LENGTH Then
                barcodeChars.Add(ch)
            End If

            lastKeyTime = currentTime
            barcodeTimer.Stop()
            barcodeTimer.Start()
            Return
        End If

        ' ===== ENTER KEY =====
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            barcodeTimer.Stop()

            If String.IsNullOrWhiteSpace(TxtNama.Text) Then
                ResetBarcodeDetection()
                Return
            End If

            Dim totalTimeMs = (DateTime.Now - barcodeStartTime).TotalMilliseconds
            Dim inputText = TxtNama.Text.Trim()

            ' Process input (barcode vs manual) using totalTimeMs heuristic
            ProcessInput(inputText, totalTimeMs)
            ResetBarcodeDetection()
            Return
        End If

        If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then
            isBarcodeMode = False
        End If
    End Sub

    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        Dim currentText = TxtNama.Text.Trim()

        If String.IsNullOrEmpty(currentText) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Show manual search only when user types letters or uses qty*... pattern
        If currentText.Any(AddressOf Char.IsLetter) Then
            TriggerManualSearch(currentText)
        ElseIf currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            If parts.Length > 0 AndAlso parts(parts.Length - 1).Any(AddressOf Char.IsLetter) Then
                TriggerManualSearch(currentText)
            End If
        End If
    End Sub

    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsedSinceLastKey = (DateTime.Now - lastKeyTime).TotalMilliseconds

        If elapsedSinceLastKey > 100 Then
            barcodeTimer.Stop()

            Dim bufferText = New String(barcodeChars.ToArray())
            If bufferText.Length >= BARCODE_MIN_LENGTH Then
                ' Jika buffer mengandung '*' atau huruf -> anggap input manual bertempo cepat.
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    TriggerManualSearch(bufferText)
                    ResetBarcodeDetection()
                    Return
                End If

                ' Murni numeric/alphanumeric tanpa '*'/'letter' -> kemungkinan barcode cepat
                isBarcodeInput = True
                ProcessInput(bufferText, (DateTime.Now - barcodeStartTime).TotalMilliseconds)
                ResetBarcodeDetection()
            End If
        End If
    End Sub

    Private Sub SetQtyAndSatuan(qtyStr As String, satuanStr As String)
        Dim qty = ParseDecimalSafe(qtyStr, 1)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = satuanStr
    End Sub

    Private Sub SetQtyOnly(qtyStr As String)
        Dim qty = ParseDecimalSafe(qtyStr, 1)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = "1"
    End Sub

    Private Sub SetDefaultQtyAndSatuan()
        TxtQty.Text = "1"
        TxtLevelSat.Text = "1"
    End Sub

    Private Function ParseDecimalSafe(value As String, defaultValue As Decimal) As Decimal
        If String.IsNullOrEmpty(value) Then Return defaultValue
        Dim result As Decimal
        Return If(Decimal.TryParse(value, result) AndAlso result > 0, result, defaultValue)
    End Function

    ''' <summary>
    ''' Process input using same heuristics as FormPenjualan:
    ''' - qty*satuan*name
    ''' - qty*something (barcode candidate vs manual)
    ''' - barcode candidate when input fast OR DB contains barcode
    ''' </summary>
    Private Sub ProcessInput(inputText As String, totalTimeMs As Double)
        If String.IsNullOrEmpty(inputText) Then Return

        Dim asteriskCount = inputText.Count(Function(c) c = "*"c)

        ' FORMAT 1: qty*satuan*nama
        If asteriskCount = 2 Then
            Dim parts As String() = inputText.Split(New Char() {"*"c})
            SetQtyAndSatuan(parts(0), parts(1))
            ProcessManualSearchList(parts(2).Trim())
            Return
        End If

        ' FORMAT 2: qty*sesuatu
        If asteriskCount = 1 Then
            Dim parts As String() = inputText.Split(New Char() {"*"c})
            SetQtyOnly(parts(0))

            Dim secondPart = parts(1).Trim()

            ' Jika input cepat (diperkirakan scan) dan panjangnya layak barcode -> perlakukan sebagai scan
            If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso secondPart.Length >= BARCODE_MIN_LENGTH Then
                isBarcodeInput = True
                If SearchByBarcode(secondPart) Then
                    Return
                Else
                    MessageBox.Show("Barcode '" & secondPart & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtNama.Clear()
                    Return
                End If
            End If

            ' Jika bukan scan cepat, coba deteksi barcode di DB lalu fallback ke pencarian manual
            If IsBarcodeCandidate(secondPart) Then
                isBarcodeInput = True
                If SearchByBarcode(secondPart) Then Return
                ' jika tidak ditemukan, reset flag dan lanjut manual
                isBarcodeInput = False
            End If

            ProcessManualSearchList(secondPart)
            TxtLevelSat.Text = "1"
            Return
        End If

        ' FORMAT 3: Barcode atau manual murni (no asterisk)
        If Not inputText.Contains("*") Then
            If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso inputText.Length >= BARCODE_MIN_LENGTH Then
                isBarcodeInput = True
                If SearchByBarcode(inputText) Then
                    Return
                Else
                    MessageBox.Show("Barcode '" & inputText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtNama.Clear()
                    Return
                End If
            End If

            If IsBarcodeCandidate(inputText) Then
                isBarcodeInput = True
                If SearchByBarcode(inputText) Then Return
                isBarcodeInput = False
            End If

            SetDefaultQtyAndSatuan()
            ProcessManualSearchList(inputText)
            Return
        End If
    End Sub


    Private Function IsBarcodeCandidate(input As String) As Boolean
        If input.Length < BARCODE_MIN_LENGTH Then Return False
        Return BarcodeExistsInDatabase(input)
    End Function

    Private Function BarcodeExistsInDatabase(barcodeValue As String) As Boolean
        Const query = "SELECT 1 FROM tbl_barang " &
                  "WHERE BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc LIMIT 1"
        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@bc", barcodeValue)
                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Function SearchByBarcode(barcodeText As String) As Boolean
        Dim query = "SELECT NAMA_BARANG FROM tbl_barang " &
               "WHERE BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc LIMIT 1"

        Dim namaBarang As String = ""
        Dim ditemukan As Boolean = False

        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@bc", barcodeText)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        namaBarang = rd("NAMA_BARANG").ToString()
                        ditemukan = True
                    End If
                End Using
            End Using
        Catch
            Return False
        End Try

        If ditemukan Then
            isBarcodeInput = True
            TxtBarcode.Text = barcodeText
            TxtNama.Text = ""
            LstBarang.Visible = False

            If String.IsNullOrEmpty(TxtQty.Text) OrElse Not Decimal.TryParse(TxtQty.Text, Nothing) OrElse ParseDecimal(TxtQty.Text) <= 0 Then
                TxtQty.Text = "1"
            End If

            If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                TxtLevelSat.Text = "1"
            End If

            Ambildatalaindaridbbarang(namaBarang)
            Return True
        End If

        Return False
    End Function


    Private Sub TriggerManualSearch(keyword As String)
        ' Stop barcode detection to avoid race closing listbox
        ResetBarcodeDetection()

        If keyword.Contains("*") Then
            Dim parts = keyword.Split("*"c)
            If parts.Length >= 2 Then
                keyword = parts.Last().Trim()
            End If
        End If

        If keyword.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            If String.IsNullOrEmpty(TxtQty.Text) Then TxtQty.Text = "1"
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then TxtLevelSat.Text = "1"
            Return
        End If

        ProcessManualSearchList(keyword)
    End Sub

    Private Sub ProcessManualSearchList(searchKeyword As String)
        searchKeyword = searchKeyword.Trim()
        If searchKeyword.Length < 2 AndAlso Not searchKeyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            If String.IsNullOrEmpty(TxtQty.Text) Then TxtQty.Text = "1"
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then TxtLevelSat.Text = "1"
            Return
        End If

        ' Reuse TampilkanDaftarBarang to populate list
        TampilkanDaftarBarang(searchKeyword)
    End Sub

    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False
        barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue
        lastKeyTime = DateTime.MinValue
        barcodeTimer.Stop()
    End Sub

    Private Sub TampilkanDaftarBarang(ByVal searchKeyword As String)
        ' Clear ListBox di awal
        LstBarang.Items.Clear()
        TxtBarcode.Clear()
        LstBarang.Visible = False

        Dim query As String = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR TRIM(BARCODE_BESAR) LIKE @Nama ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim itemText As String = rd("NAMA_BARANG").ToString()
                    Select Case LblLokasiBarang.Text
                        Case "TOKO"
                            Dim stokToko As Decimal = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            itemText &= " => " & stokToko.ToString("N0")
                        Case "GUDANG"
                            Dim stokGudang As Decimal = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
                            itemText &= " => " & stokGudang.ToString("N0")
                    End Select

                    If searchKeyword = rd("BARCODE_SEDANG").ToString() Or searchKeyword = rd("BARCODE_BESAR").ToString() Then
                        TxtBarcode.Text = searchKeyword
                    End If

                    LstBarang.Items.Add(itemText)
                End While

                ' ✅ PENTING: Jangan tampilkan ListBox jika hasil hanya 1 dan input barcode
                If Not (LstBarang.Items.Count = 1 AndAlso isBarcodeInput) Then
                    LstBarang.Visible = LstBarang.Items.Count > 0
                Else
                    LstBarang.Visible = False
                End If

                isBarcodeInput = False  ' ← RESET FLAG
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
        Dim namayangdiambil As String = ""
        Dim originalInput As String = TxtNama.Text.Trim()

        ' Ambil nama dari ListBox jika ada selection (prioritas)
        Dim selectedValue As String = ""
        If LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        ElseIf LstBarang.SelectedItem IsNot Nothing Then
            selectedValue = LstBarang.SelectedItem.ToString()
        End If

        If Not String.IsNullOrEmpty(selectedValue) Then
            ' Hapus postfix stok " => 123" jika ada
            Dim indexArrow As Integer = selectedValue.IndexOf(" => ")
            If indexArrow >= 0 Then
                namayangdiambil = selectedValue.Substring(0, indexArrow).Trim()
            Else
                namayangdiambil = selectedValue.Trim()
            End If

            ' Jika user mengetik qty atau qty*satuan*... di TxtNama, parse qty/satuan
            If originalInput.Contains("*"c) Then
                Dim inputParts As String() = originalInput.Split("*"c)
                If inputParts.Length >= 3 Then
                    SetQtyAndSatuan(inputParts(0).Trim(), inputParts(1).Trim())
                ElseIf inputParts.Length = 2 Then
                    SetQtyOnly(inputParts(0).Trim())
                End If
            End If
        Else
            ' Tidak ada selection di ListBox -> coba parse nama dari TxtNama (fallback)
            If originalInput.Contains("*"c) Then
                Dim inputParts As String() = originalInput.Split("*"c)
                If inputParts.Length >= 3 Then
                    SetQtyAndSatuan(inputParts(0).Trim(), inputParts(1).Trim())
                    namayangdiambil = String.Join("*", inputParts, 2, inputParts.Length - 2).Trim()
                ElseIf inputParts.Length = 2 Then
                    SetQtyOnly(inputParts(0).Trim())
                    namayangdiambil = inputParts(1).Trim()
                End If
            Else
                MessageBox.Show("Silakan pilih barang terlebih dahulu!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        End If

        If String.IsNullOrEmpty(namayangdiambil) Then
            MessageBox.Show("Nama barang tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Ambildatalaindaridbbarang(namayangdiambil)
    End Sub

    Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, HARGA_BELI FROM tbl_barang WHERE NAMA_BARANG = @NAMA", conn)
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
                    Else
                        ' Sesuaikan nilai berdasarkan barcode
                        If TxtLevelSat.Text = "1" Then
                            satuanUmum = If(Not IsDBNull(rd(3)), rd.GetString(3), String.Empty)
                            isiUmum = If(Not IsDBNull(rd(6)), rd.GetInt32(6), 0)
                        ElseIf TxtLevelSat.Text = "2" Then
                            satuanUmum = If(Not IsDBNull(rd(4)), rd.GetString(4), String.Empty)
                            isiUmum = If(Not IsDBNull(rd(7)), rd.GetInt32(7), 0)
                        ElseIf TxtLevelSat.Text = "3" Then
                            satuanUmum = If(Not IsDBNull(rd(5)), rd.GetString(5), String.Empty)
                            isiUmum = If(Not IsDBNull(rd(8)), rd.GetInt32(8), 0)
                        End If
                    End If
                    Dim Average As String = If(Not IsDBNull(rd(12)), rd.GetDecimal(12).ToString(), String.Empty)
                    Dim HargaSebelumnya As String = If(Not IsDBNull(rd(2)), rd.GetDecimal(2).ToString(), String.Empty)

                    ' Pastikan isiUmum tidak bernilai nol
                    If isiUmum = 0 Then
                        isiUmum = 1
                    End If

                    ' Set nilai textbox
                    TxtKode.Text = idBarang
                    TxtHarga.Text = hargaBeli
                    Txtsatuan.Text = satuanUmum
                    TxtIsi.Text = isiUmum.ToString()
                    TxtAverage.Text = Average
                    TxtHargaSebelumnya.Text = HargaSebelumnya
                End If
            End Using
        End Using
        ' Memanggil fungsi tambahan jika diperlukan
        TambahDataLangsung(namayangdiambil)
    End Sub


    Private Sub TambahDataLangsung(ByVal namayangdiambil As String)
        ' === CEK DUPLIKAT ===
        If Kodebarangsama = "Tidak" Then
            For Each row As DataGridViewRow In DgvData.Rows
                If row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() = TxtKode.Text Then
                    MessageBox.Show(namayangdiambil & " sudah ada dalam daftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    ' ✅ CLEAR HANYA JIKA:
                    ' 1. Input barcode (detected cepat), ATAU
                    ' 2. ListBox hanya 1 hasil
                    If isBarcodeInput OrElse LstBarang.Items.Count = 1 Then
                        KosongTxtboxcari()
                    Else
                        ' Jika manual & ListBox > 1: jangan clear, biarkan user pilih item lain
                        ' Hanya clear ListBox visibility agar tidak menganggu
                        LstBarang.Select()
                    End If

                    Return
                End If
            Next
        End If

        ' Insert data baru ke DataGridView
        Dim indeksBaris As Integer

        If DgvData.SelectedCells.Count > 0 Then
            ' Ambil indeks baris dari sel yang dipilih pertama
            indeksBaris = DgvData.SelectedCells(0).RowIndex
            ' Sisipkan baris baru di posisi indeks tersebut
            DgvData.Rows.Insert(indeksBaris, "")
        Else
            ' Tambahkan baris baru di akhir jika tidak ada sel yang dipilih
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
        Dim HPP As Decimal = hargaBeli * isi
        Dim Average As Decimal = Decimal.Parse(TxtAverage.Text)
        Dim HargaSebelumnya As Decimal = Decimal.Parse(TxtHargaSebelumnya.Text)

        ' Menetapkan nilai untuk baris yang baru ditambahkan
        DgvData.Rows(indeksBaris).Cells("Id").Value = kode
        DgvData.Rows(indeksBaris).Cells("nama").Value = namayangdiambil
        DgvData.Rows(indeksBaris).Cells("Hargabeli").Value = HPP
        DgvData.Rows(indeksBaris).Cells("qty").Value = qty
        DgvData.Rows(indeksBaris).Cells("Satuan").Value = satuan
        DgvData.Rows(indeksBaris).Cells("isi").Value = isi
        DgvData.Rows(indeksBaris).Cells("HargaBeliSat").Value = Average * isi
        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qty * isi
        DgvData.Rows(indeksBaris).Cells("Totalharga").Value = qty * HPP
        DgvData.Rows(indeksBaris).Cells("Average").Value = Average
        DgvData.Rows(indeksBaris).Cells("HargaSebelumnya").Value = HargaSebelumnya
        ' Melakukan pembaruan pada ringkasan atau operasi relevan lainnya
        UpdateSemuaTotal()

        ' Membersihkan field input
        KosongTxtboxcari()


        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        End If

        SetupFocusToGrid()

    End Sub

    Private Sub HitungNilaiSetiapBaris(ByVal indeksBaris As Integer)
        ' Mengambil nilai dari input lainnya
        Dim hargaBeli As Decimal = 0
        Dim qtyBarang As Decimal = 1
        Dim isiBarang As Integer = 1


        If Not IsNothing(DgvData.Rows(indeksBaris).Cells("Hargabeli").Value) AndAlso
           Decimal.TryParse(DgvData.Rows(indeksBaris).Cells("Hargabeli").Value.ToString(), hargaBeli) Then
            ' hargaJual berhasil di-parse
        End If

        If Not IsNothing(DgvData.Rows(indeksBaris).Cells("QTY").Value) AndAlso
           Decimal.TryParse(DgvData.Rows(indeksBaris).Cells("QTY").Value.ToString(), qtyBarang) Then
            ' qtyBarang berhasil di-parse
        End If

        If Not IsNothing(DgvData.Rows(indeksBaris).Cells("Isi").Value) AndAlso
           Integer.TryParse(DgvData.Rows(indeksBaris).Cells("Isi").Value.ToString(), isiBarang) Then
            ' isiBarang berhasil di-parse
        End If

        ' Menghitung nilai untuk setiap baris
        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qtyBarang * isiBarang
        DgvData.Rows(indeksBaris).Cells("TotalHarga").Value = hargaBeli * qtyBarang
    End Sub



    Private Function ParseDecimal(ByVal value As Object) As Decimal
        If value Is Nothing Then Return 0D

        ' Konversi ke string dan bersihkan
        Dim s As String = value.ToString().Trim()

        ' Ganti pemisah ribuan dan desimal secara cerdas:
        ' - Jika mengandung koma dan titik, asumsikan koma desimal (format Eropa) → hapus titik, ganti koma jadi titik
        ' - Jika hanya koma, ganti koma jadi titik
        If s.Contains(",") AndAlso s.Contains(".") Then
            s = s.Replace(".", "").Replace(",", ".") ' Contoh: 1.234,56 → 1234.56
        ElseIf s.Contains(",") Then
            s = s.Replace(",", ".") ' Contoh: 12,5 → 12.5
        End If

        ' Coba parsing menggunakan InvariantCulture (pemisah desimal = titik)
        Dim result As Decimal = 0
        Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result)

        Return result
    End Function

    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        '========================== Nama
        If e.ColumnIndex = 1 Then
            If DgvData.Rows(e.RowIndex) IsNot Nothing AndAlso DgvData.Rows(e.RowIndex).Cells("Nama") IsNot Nothing Then
                Dim inputText As String = DgvData.Rows(e.RowIndex).Cells("Nama").Value.ToString().Trim()
                Dim qtyValue As Decimal = 1
                Dim namaBarangValue As String = inputText

                ' Cek apakah ada tanda bintang
                Dim indexAsteriskQty As Integer = inputText.IndexOf("*")
                Dim indexAsteriskHarga As Integer = -1

                If indexAsteriskQty >= 0 Then
                    indexAsteriskHarga = inputText.IndexOf("*", indexAsteriskQty + 1)
                End If

                If indexAsteriskQty >= 0 AndAlso indexAsteriskHarga > indexAsteriskQty Then
                    ' Format: qty * harga * namaBarang → Ambil qty dan namaBarang, abaikan harga
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ParseDecimal(angkaQty) ' Gunakan fungsi yang sudah dibuat
                    namaBarangValue = inputText.Substring(indexAsteriskHarga + 1).Trim()

                ElseIf indexAsteriskQty >= 0 Then
                    ' Format: qty * namaBarang
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ParseDecimal(angkaQty) ' Gunakan fungsi yang sudah dibuat
                    namaBarangValue = inputText.Substring(indexAsteriskQty + 1).Trim()
                End If

                DgvData.Rows(e.RowIndex).Cells("Nama").Value = namaBarangValue

                Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @NamaBarang OR TRIM(NAMA_BARANG) LIKE @NamaBarang OR TRIM(BARCODE_KECIL) LIKE @NamaBarang OR TRIM(BARCODE_SEDANG) LIKE @NamaBarang OR TRIM(BARCODE_BESAR) LIKE @NamaBarang"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@NamaBarang", namaBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.HasRows Then
                            rd.Read() ' Lanjutkan ke data pertama
                            DgvData.Rows(e.RowIndex).Cells("Id").Value = rd("ID_BARANG")
                            DgvData.Rows(e.RowIndex).Cells("nama").Value = rd("NAMA_BARANG")

                            Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(e.RowIndex).Cells("Satuan"), DataGridViewComboBoxCell)
                            comboCell.Items.Clear()

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


                            Dim satuan As String = ""
                            Dim isi As Integer = 1

                            Dim namaBarang As String = If(Not rd.IsDBNull(rd.GetOrdinal("NAMA_BARANG")), rd("NAMA_BARANG").ToString(), "")
                            Dim barcodeKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("BARCODE_KECIL")), rd("BARCODE_KECIL").ToString(), "")
                            Dim barcodeSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("BARCODE_SEDANG")), rd("BARCODE_SEDANG").ToString(), "")
                            Dim barcodeBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("BARCODE_BESAR")), rd("BARCODE_BESAR").ToString(), "")

                            If namaBarangValue = barcodeSedang Then
                                satuan = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), rd("SATUAN_UMUM_SEDANG").ToString(), "")
                                isi = If(Not rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_SEDANG")), Convert.ToDecimal(rd("ISI_UMUM_SEDANG")), 1)
                            ElseIf namaBarangValue = barcodeBesar Then
                                satuan = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")), rd("SATUAN_UMUM_BESAR").ToString(), "")
                                isi = If(Not rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_BESAR")), Convert.ToDecimal(rd("ISI_UMUM_BESAR")), 1)
                            Else
                                satuan = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")), rd("SATUAN_UMUM_KECIL").ToString(), "")
                                isi = If(Not rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_KECIL")), Convert.ToDecimal(rd("ISI_UMUM_KECIL")), 1)
                            End If

                            DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = If(Not rd.IsDBNull(rd.GetOrdinal("HARGA_BELI_TERAKHIR")), rd.GetDecimal(rd.GetOrdinal("HARGA_BELI_TERAKHIR")), 0) * isi
                            DgvData.Rows(e.RowIndex).Cells("HargaBeliSat").Value = If(Not rd.IsDBNull(rd.GetOrdinal("HARGA_BELI")), rd.GetDecimal(rd.GetOrdinal("HARGA_BELI")), 0) * isi

                            DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuan
                            DgvData.Rows(e.RowIndex).Cells("isi").Value = isi
                            If DgvData.Rows(e.RowIndex).Cells("isi").Value = 0 Then
                                DgvData.Rows(e.RowIndex).Cells("qtysat").Value = 1
                            End If

                            DgvData.Rows(e.RowIndex).Cells("qty").Value = qtyValue

                            DgvData.Rows(e.RowIndex).Cells("Average").Value = If(Not rd.IsDBNull(rd.GetOrdinal("HARGA_BELI")), rd.GetDecimal(rd.GetOrdinal("HARGA_BELI")), 0)
                            DgvData.Rows(e.RowIndex).Cells("HargaSebelumnya").Value = If(Not rd.IsDBNull(rd.GetOrdinal("HARGA_BELI_TERAKHIR")), rd.GetDecimal(rd.GetOrdinal("HARGA_BELI_TERAKHIR")), 0)

                            If Kodebarangsama = "Tidak" Then
                                For barisatas As Integer = 0 To DgvData.RowCount - 1
                                    For barisbawah As Integer = barisatas + 1 To DgvData.RowCount - 2
                                        If DgvData.Rows(barisbawah).Cells("Id").Value = DgvData.Rows(barisatas).Cells("Id").Value Then
                                            DgvData.Rows(barisatas).Cells("qty").Value = DgvData.Rows(barisatas).Cells("qty").Value + 1

                                            ' Menghapus baris jika bukan baris baru
                                            If Not DgvData.Rows(barisbawah).IsNewRow Then
                                                DgvData.Rows.RemoveAt(barisbawah)
                                            End If
                                        End If
                                    Next
                                Next
                            End If

                        Else
                            rd.Close()

                            DgvData.Rows(e.RowIndex).Cells("nama").Value = ""
                            SendKeys.Send("{down}")
                        End If
                    End Using
                End Using
            End If
            HitungNilaiSetiapBaris(e.RowIndex)
        End If

        '========================== Harga beli
        If e.ColumnIndex = 5 Then
            Dim hargaBeliValue As Decimal

            ' Validasi nilai Harga Beli
            If Decimal.TryParse(DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value, hargaBeliValue) Then
                If hargaBeliValue <= 0 Then
                    MessageBox.Show("Harga beli harus lebih besar dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = 0
                Else

                    HitungNilaiSetiapBaris(e.RowIndex)

                    ' Ambil nilai Qty
                    Dim qtyValue As Decimal = If(IsDBNull(DgvData.Rows(e.RowIndex).Cells("Qty").Value) OrElse DgvData.Rows(e.RowIndex).Cells("Qty").Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(e.RowIndex).Cells("Qty").Value))

                    ' Ambil nilai Isi
                    Dim isiValue As Integer = If(IsDBNull(DgvData.Rows(e.RowIndex).Cells("isi").Value) OrElse DgvData.Rows(e.RowIndex).Cells("isi").Value Is Nothing OrElse DgvData.Rows(e.RowIndex).Cells("isi").Value = 0, 1, Convert.ToInt32(DgvData.Rows(e.RowIndex).Cells("isi").Value))

                    ' Hitung Harga Beli per satuan
                    Dim hargaBeliPerSatuan As Decimal = hargaBeliValue / isiValue

                    ' Hitung Qty dalam satuan
                    Dim qtySatValue As Decimal = qtyValue * isiValue

                    'DgvData.Rows(e.RowIndex).Cells("QtySat").Value = qtySatValue
                    'DgvData.Rows(e.RowIndex).Cells("HargaBeliSat").Value = CDec(DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value) * isiValue
                    'DgvData.Rows(e.RowIndex).Cells("Totalharga").Value = hargaBeliValue * qtyValue

                    If MunculHargaJual = "Iya" Then
                        Dim hargaLama As Decimal
                        If DgvData.Rows(e.RowIndex).Cells("Average").Value IsNot Nothing AndAlso
                           Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("Average").Value.ToString()) Then
                            Decimal.TryParse(DgvData.Rows(e.RowIndex).Cells("Average").Value.ToString(), hargaLama)
                        End If

                        Dim QtySbl As Decimal
                        If DgvData.Rows(e.RowIndex).Cells("QtySebelumnya").Value IsNot Nothing AndAlso
                           Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("QtySebelumnya").Value.ToString()) Then
                            Decimal.TryParse(DgvData.Rows(e.RowIndex).Cells("QtySebelumnya").Value.ToString(), QtySbl)
                        End If


                        With TambahBarang
                            .LblUtama.Text = "EDIT HARGA JUAL DARI PEMBELIAN"
                            .GBBarcode.Visible = False
                            .GBStok.Visible = False
                            .GBBarang.Enabled = False
                            .GBPoint.Visible = False
                            .PanelInfoRubahHarga.Visible = True
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
                            .TxtKode.Text = DgvData.Rows(e.RowIndex).Cells("Id").Value
                            .LblQtySbl.Text = QtySbl.ToString("N0")
                            .LblQtyBaru.Text = qtySatValue.ToString("N0")
                            .LblRpBaru.Text = hargaBeliPerSatuan.ToString("N2")
                            .LblRpLama.Text = hargaLama.ToString("N2")

                            If UpdateHargabeli = "Metode Average (Rata - Rata)" Then
                                .LblJenisUpdate.Text = UpdateHargabeli & " " & Updateberdasarkan
                                .LblMetode.Text = UpdateHargabeli
                                .LblJenis.Text = Updateberdasarkan
                            Else
                                .LblJenisUpdate.Text = UpdateHargabeli
                                .LblMetode.Text = UpdateHargabeli
                            End If
                            .ShowDialog()
                        End With

                    End If
                End If
            Else
                MessageBox.Show("Harga beli harus berupa angka.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = 0
            End If

        End If

        '========================== Qty
        If e.ColumnIndex = 2 Then
            Dim rowIndex As Integer = e.RowIndex
            Dim qtyCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("QTY")

            ' Pastikan nilai sel tidak null atau kosong
            If IsDBNull(qtyCell.Value) OrElse String.IsNullOrWhiteSpace(qtyCell.Value.ToString()) Then
                MsgBox("Kolom QTY tidak boleh kosong. Mohon masukkan angka.", vbExclamation, "Kesalahan Input")
                qtyCell.Value = 1
            End If

            ' Ganti format angka sesuai budaya Indonesia
            Dim qtyCellValue As String = qtyCell.Value.ToString().Replace(".", "").Replace(",", ".")
            If Not Decimal.TryParse(qtyCellValue, Globalization.NumberStyles.Number, Globalization.CultureInfo.InvariantCulture, Nothing) Then
                MsgBox("Oops! Sepertinya Anda lupa hanya masukkan angka untuk qty. Mohon periksa kembali.", vbExclamation, "Kesalahan Input")
                qtyCell.Value = 1
            End If

            HitungNilaiSetiapBaris(e.RowIndex)

            ' Optimalisasi refresh DataGridView
            DgvData.SuspendLayout()
            DgvData.ResumeLayout()
        End If

        UpdateSemuaTotal()
    End Sub


    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DgvData.DataError
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

                ' Mendapatkan teks dari TextBox autoText
                Dim searchTerm As String = autoText.Text

                ' Memanggil AddItems dengan nilai pencarian dari TextBox
                AddItems(DataCollection, searchTerm)

                autoText.AutoCompleteCustomSource = DataCollection
            End If
        End If

        ' Periksa apakah kolom yang saat ini sedang diedit adalah kolom yang berisi ComboBox
        If DgvData.CurrentCell.ColumnIndex = 3 Then
            Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)

            ' Hapus penanganan event SelectedIndexChanged jika ada
            RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged

            ' Tambahkan penanganan event SelectedIndexChanged ke ComboBox
            AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
        End If

    End Sub

    Public Sub AddItems(ByVal col As AutoCompleteStringCollection, ByVal searchTerm As String)
        Using cmd As New MySqlCommand("SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @searchTerm", conn)
            ' Menambahkan '%' di sekitar searchTerm di parameter
            cmd.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    col.Add(rd("NAMA_BARANG").ToString())
                Loop
            End Using
        End Using
    End Sub


    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)

        ' Dapatkan sel saat ini yang sedang diedit
        Dim cell As DataGridViewComboBoxCell = TryCast(DgvData.CurrentCell, DataGridViewComboBoxCell)
        If cell Is Nothing Then Return

        Dim selectedItemId As String = cell.OwningRow.Cells("Id").Value?.ToString()
        If String.IsNullOrEmpty(selectedItemId) Then
            MessageBox.Show("ID Barang tidak valid!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Using cmd As New MySqlCommand("SELECT HARGA_BELI, HARGA_BELI_TERAKHIR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ItemId", conn)
            cmd.Parameters.AddWithValue("@ItemId", selectedItemId)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Ambil nilai berdasarkan pilihan di ComboBox
                    Dim isiValue As Decimal = 1
                    Select Case comboBox.SelectedIndex
                        Case 0
                            Decimal.TryParse(rd("ISI_UMUM_KECIL").ToString(), isiValue)
                        Case 1
                            Decimal.TryParse(rd("ISI_UMUM_SEDANG").ToString(), isiValue)
                        Case Else
                            Decimal.TryParse(rd("ISI_UMUM_BESAR").ToString(), isiValue)
                    End Select
                    cell.OwningRow.Cells("Isi").Value = isiValue

                    ' Konversi nilai harga beli
                    Dim hargaBeli As Decimal = If(IsDBNull(rd("HARGA_BELI")), 0D, Convert.ToDecimal(rd("HARGA_BELI")))
                    Dim hargaBeliterakhir As Decimal = If(IsDBNull(rd("HARGA_BELI_TERAKHIR")), 0D, Convert.ToDecimal(rd("HARGA_BELI_TERAKHIR")))

                    ' Dapatkan indeks baris
                    Dim rowIndex As Integer = DgvData.CurrentCell.RowIndex

                    ' Hitung nilai lainnya
                    Dim isiQty As Decimal = If(Decimal.TryParse(DgvData("isi", rowIndex).Value?.ToString(), Nothing), CDec(DgvData("isi", rowIndex).Value), 1D)
                    Dim qty As Decimal = If(Decimal.TryParse(DgvData("qty", rowIndex).Value?.ToString(), Nothing), CDec(DgvData("qty", rowIndex).Value), 1D)

                    ' Menghitung HPP dan HPPAverage
                    Dim HPP As Decimal = hargaBeliterakhir * isiQty
                    Dim HPPAverage As Decimal = hargaBeli * isiQty

                    ' Update sel di DataGridView
                    DgvData("Hargabeli", rowIndex).Value = HPP
                    DgvData("HargaBeliSat", rowIndex).Value = HPPAverage
                    DgvData("qtysat", rowIndex).Value = isiQty * qty
                    DgvData("totalharga", rowIndex).Value = HPP * qty

                    ' Panggil metode untuk memperbarui total
                    UpdateSemuaTotal()
                Else
                    MessageBox.Show("Satuan barang dan atau harga jual belum di input !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End Using
        End Using
    End Sub


    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click, BtnClose.Click
        If GBBayar.Visible Then
            Tekanbatal()
        ElseIf TxtNama.Text <> "" Then
            TxtNama.Clear()
        Else
            ' Menambahkan pertanyaan apakah akan keluar
            Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Close()
            End If
        End If
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

    Private Sub EditHargaJualToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditHargaJualToolStripMenuItem.Click
        If DgvData.SelectedCells.Count > 0 Then
            Dim rowIndex As Integer = DgvData.SelectedCells(0).RowIndex
            Dim qtyValue As Decimal = DgvData.Rows(rowIndex).Cells("Qty").Value
            Dim isiValue As Integer = If(IsDBNull(DgvData.Rows(rowIndex).Cells("isi").Value) OrElse DgvData.Rows(rowIndex).Cells("isi").Value Is Nothing OrElse DgvData.Rows(rowIndex).Cells("isi").Value = 0, 1, Convert.ToInt32(DgvData.Rows(rowIndex).Cells("isi").Value))
            Dim hargaBeliValue As Decimal = If(IsDBNull(DgvData.Rows(rowIndex).Cells("Hargabeli").Value) OrElse DgvData.Rows(rowIndex).Cells("Hargabeli").Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(rowIndex).Cells("Hargabeli").Value)) / isiValue
            Dim qtySatValue As Decimal = qtyValue * isiValue

            DgvData.Rows(rowIndex).Cells("QtySat").Value = qtySatValue
            DgvData.Rows(rowIndex).Cells("HargaBeliSat").Value = hargaBeliValue * isiValue
            DgvData.Rows(rowIndex).Cells("Totalharga").Value = hargaBeliValue * qtyValue

            Dim hargaLama As Decimal
            If DgvData.Rows(rowIndex).Cells("Average").Value IsNot Nothing AndAlso
               Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("Average").Value.ToString()) Then
                Decimal.TryParse(DgvData.Rows(rowIndex).Cells("Average").Value.ToString(), hargaLama)
            End If

            Dim QtySbl As Decimal
            If DgvData.Rows(rowIndex).Cells("QtySebelumnya").Value IsNot Nothing AndAlso
               Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("QtySebelumnya").Value.ToString()) Then
                Decimal.TryParse(DgvData.Rows(rowIndex).Cells("QtySebelumnya").Value.ToString(), QtySbl)
            End If

            With TambahBarang
                .LblUtama.Text = "EDIT HARGA JUAL DARI PEMBELIAN"
                .GBBarcode.Visible = False
                .GBStok.Visible = False
                .GBBarang.Enabled = False
                .GBPoint.Visible = False
                .PanelInfoRubahHarga.Visible = True
                .BtnTambahKategori.Visible = False
                .BtnSupliyer.Visible = False
                .BtnTambahSatuan.Visible = False
                .CBManual.Visible = False
                .BtnBaru.Visible = False
                '.BackColor = Color.DarkCyan
                .Size = New Size(825, 590)
                TambahBarang.Tampilkategori()
                TambahBarang.TampilSatuan()
                TambahBarang.Tampilsupliyer()
                .TxtKode.Text = DgvData.Rows(rowIndex).Cells("Id").Value
                .LblQtySbl.Text = QtySbl.ToString("N0")
                .LblQtyBaru.Text = qtySatValue.ToString("N0")
                .LblRpBaru.Text = hargaBeliValue.ToString("N2")
                .LblRpLama.Text = hargaLama.ToString("N2")

                If UpdateHargabeli = "Metode Average (Rata - Rata)" Then
                    .LblJenisUpdate.Text = UpdateHargabeli & " " & Updateberdasarkan
                    .LblMetode.Text = UpdateHargabeli
                    .LblJenis.Text = Updateberdasarkan
                Else
                    .LblJenisUpdate.Text = UpdateHargabeli
                    .LblMetode.Text = UpdateHargabeli
                End If
                .ShowDialog()
                .TxtHArgaJUalUmumKecil.Focus()
            End With

        End If
    End Sub



    Private Sub TxtGrandtotal_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtGrandtotal.TextChanged
        If TxtGrandtotal.Text = "" Or Not IsNumeric(TxtGrandtotal.Text) Then
            Txtlihattotal.Text = "0"
            Exit Sub
        Else
            Txtlihattotal.Text = FormatNumber(TxtGrandtotal.Text, 0)
            TxtTotal.Text = Math.Round(Convert.ToDecimal(TxtGrandtotal.Text), 0).ToString()
        End If
    End Sub

    Private Sub TxtBayar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtBayar.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TxtBayar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtBayar.TextChanged
        Dim total As Decimal
        If Not Decimal.TryParse(TxtTotal.Text, total) Then
            total = 0D
        End If

        Dim bayar As Decimal
        If Not Decimal.TryParse(TxtBayar.Text, bayar) Then
            bayar = 0D
        End If

        Dim bantuanBayar As Decimal = total - bayar
        TxtKembali.Text = Math.Abs(bantuanBayar).ToString()
        TxtBAntuanbayar.Text = bantuanBayar.ToString()

        If TxtJenisTrans.Text = "TambahPembelian" AndAlso bayar > total Then
            MessageBox.Show("Pembayaran melebihi belanja !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ' Menghapus satu angka paling belakang dari TxtBayar.Text
            If TxtBayar.Text.Length > 0 Then
                TxtBayar.Text = TxtBayar.Text.Substring(0, TxtBayar.Text.Length - 1)
                ' Memastikan kursor tetap berada di posisi paling kanan
                TxtBayar.Select(TxtBayar.Text.Length, 0)
            End If
        End If
    End Sub

    Private Sub TxtBAntuanbayar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtBAntuanbayar.TextChanged
        Dim bantuanBayar As Decimal

        If Decimal.TryParse(TxtBAntuanbayar.Text, bantuanBayar) Then
            If bantuanBayar > 0 Then
                LblPembayaran.Text = "Hutang :"
                LblJatuhTempo.Visible = True
                DTPJatuhTempo.Visible = True
                LblStatusTrans.Text = "Belum Lunas"
                TxtKembali.Visible = True
                LblPembayaran.Visible = True
            Else
                LblPembayaran.Text = "Kembalian :"
                LblJatuhTempo.Visible = False
                DTPJatuhTempo.Visible = False
                LblStatusTrans.Text = "Lunas"
                TxtKembali.Visible = False
                LblPembayaran.Visible = False
            End If
        End If
    End Sub


    Private Sub Form_Pembelian_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                Tekanbayar()
            Case Keys.F2
                TxtSupplier.Select()
            Case Keys.F3
                Tekansupliyer()
            Case Keys.F4
                Tekanbarang()
            Case Keys.F5
            Case Keys.Escape
                If GBBayar.Visible Then
                    Tekanbatal()
                ElseIf TxtNama.Text <> "" Then
                    TxtNama.Clear()
                Else
                    ' Menambahkan pertanyaan apakah akan keluar
                    Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Close()
                    End If
                End If
            Case Keys.F9
                CmbJenisBayar.Select()
                CmbJenisBayar.DroppedDown = True
            Case Keys.F10
                If GBBayar.Visible Then
                    SimpanTransaksi()
                End If
            Case Keys.F11
                If GBBayar.Visible Then
                    Tekanbatal()
                End If
        End Select
    End Sub

    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        Tekanbayar()
    End Sub

    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click
        Tekanbarang()
    End Sub

    Private Sub BtnSupliyer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSupliyer.Click
        Tekansupliyer()
    End Sub

    Private Sub BtnSimpann_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpann.Click
        SimpanTransaksi()
    End Sub

    Private Sub BtnBatal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBatal.Click
        Tekanbatal()
    End Sub

    Public Sub Tekanbayar()
        ' Cek apakah supplier belum dipilih
        If String.IsNullOrEmpty(TxtSupplier.Text) Then
            MessageBox.Show("Supliyer belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TxtSupplier.Select()
            Exit Sub
        End If

        ' Cek apakah belum ada transaksi pembelian
        If TxtGrandtotal.Text = "0" OrElse DgvData.RowCount = 0 Then
            MessageBox.Show("Belum ada transaksi Pembelian", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            If DgvData.Rows.Count > 0 AndAlso DgvData.Columns.Count > 1 Then
                DgvData.CurrentCell = DgvData(1, 0)
                DgvData.Rows(0).Selected = True
            End If

            If awalpembelian = "Pencarian" Then
                TxtNama.Select()
            End If


            TxtNama.Focus()
            Exit Sub
        End If


        Dim ModulBeliRugi As String = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblBeliRugi.Text)

        If ModulBeliRugi = "Tidak" Then
            If Cekjualrugi() Then
                ' Ada barang yang merugi, keluar dari fungsi atau lakukan tindakan yang sesuai
                Return
            End If
        End If


        CenterPanelBayar()
        GBBayar.Visible = True

        If TxtJenisTrans.Text = "TambahPembelian" Then
            TxtBayar.Text = ""
        End If

        TxtBayar.Focus()

    End Sub

    Private Sub CenterPanelBayar()
        Dim x As Integer = (ClientSize.Width - GBBayar.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - GBBayar.Height) \ 2
        GBBayar.Location = New Point(x, y)
    End Sub

    Public Function Cekjualrugi() As Boolean
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Id").Value IsNot Nothing AndAlso dgvRow.Cells("Id").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Id").Value.ToString()

                Dim qtysat As Decimal
                If Decimal.TryParse(dgvRow.Cells("QtySat").Value.ToString(), qtysat) Then

                    Dim HargajualUmum As Decimal = 0
                    Dim HargajualPartai As Decimal = 0

                    ' Mengumpulkan informasi barang
                    Using cmd As New MySqlCommand("SELECT HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_PARTAI_KECIL FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @ID_BARANG", conn)
                        cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                        Using rd As MySqlDataReader = cmd.ExecuteReader()
                            If rd.Read() Then
                                HargajualUmum = Convert.ToDecimal(rd("HARGA_JUAL_UMUM_KECIL")) * qtysat
                                HargajualPartai = Convert.ToDecimal(rd("HARGA_JUAL_PARTAI_KECIL")) * qtysat
                            End If
                        End Using
                    End Using

                    ' Memproses data di DataGridView
                    Dim HargaBeli As Decimal
                    If Decimal.TryParse(dgvRow.Cells("TotalHarga").Value.ToString(), HargaBeli) Then
                        If HargaBeli > HargajualUmum Then
                            ' Harga jual rugi
                            Dim errorMessage As String = "Harga ==> " & dgvRow.Cells("Nama").Value.ToString() & " <== Terjual rugi. " & vbCrLf & vbCrLf &
                                                     "Harga beli: " & HargaBeli.ToString("N0") & ", Harga jual Umum: " & HargajualUmum.ToString("N0")
                            MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)

                            ' Menyorot baris yang bermasalah
                            dgvRow.Selected = True
                            For Each cell As DataGridViewCell In dgvRow.Cells
                                cell.Style.BackColor = Color.Red
                            Next

                            Return True ' Ada masalah
                        End If

                        If HargajualPartai <> 0 AndAlso HargaBeli > HargajualPartai Then
                            ' Harga jual rugi
                            Dim errorMessage As String = "Harga ==> " & dgvRow.Cells("Nama").Value.ToString() & " <== Terjual rugi. " & vbCrLf & vbCrLf &
                                                     "Harga beli: " & HargaBeli.ToString("N0") & ", Harga jual Partai: " & HargajualPartai.ToString("N0")
                            MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)

                            ' Menyorot baris yang bermasalah
                            dgvRow.Selected = True
                            For Each cell As DataGridViewCell In dgvRow.Cells
                                cell.Style.BackColor = Color.Red
                            Next

                            Return True ' Ada masalah
                        End If
                    Else
                        ' Penanganan ketika HargaBeli tidak dapat di-parse
                        MessageBox.Show("Harga jual tidak valid untuk barang " & kodeBarangValue, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Else
                    ' Penanganan ketika qtysat tidak dapat di-parse
                    MessageBox.Show("QtySat tidak valid untuk barang " & kodeBarangValue, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                ' Mengembalikan sel kembali ke warna default (jika ada)
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = dgvRow.DefaultCellStyle.BackColor
                Next
            End If
        Next

        Return False
    End Function

    Public Sub Tekanbarang()
        Using f As New TambahBarang()
            f.LblUtama.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using
    End Sub

    Public Sub Tekansupliyer()
        TambahSupliyer.ShowDialog()
        AmbilDataSupplier()
    End Sub

    Public Sub SimpanTransaksi()
        Dim bayarText = TxtBayar.Text.Trim()
        Dim jenisTrans = TxtJenisTrans.Text
        Dim isBayarKosong = String.IsNullOrEmpty(bayarText) OrElse bayarText = "0"

        ' Konfirmasi jika belum bayar
        If isBayarKosong Then
            Dim pesan As MsgBoxResult = MsgBox(
            "Nominal Pembayaran belum diisi. Lanjut sebagai hutang semua?" & vbCrLf &
            "Tekan OK jika lanjut, Cancel jika batal.",
            MsgBoxStyle.OkCancel, "Perhatian Penting")

            If pesan <> MsgBoxResult.Ok Then Exit Sub
        End If

        Cursor = Cursors.WaitCursor

        If TransaksiLampau = "Tidak" AndAlso jenisTrans = "TambahPembelian" Then
            DTPTgl.Value = DateTime.Now
            NomorBeli()
        ElseIf jenisTrans = "TambahPembelian" Then
            NomorBeli()
        End If

        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            If jenisTrans <> "TambahPembelian" Then Hapusbelanja(transaction)

            TxtBayar.Text = If(isBayarKosong, "0", bayarText)

            SimpanPembelian(transaction)
            SimpanPembelianDetail(transaction)
            HistoryBarang(transaction)

            Dim bayarDecimal = ParseDecimal(TxtBayar.Text)
            Dim statusLunas = LblStatusTrans.Text = "Lunas"

            If statusLunas OrElse bayarDecimal <> 0 Then Simpanjurnal(transaction)
            If Not statusLunas Then SimpanjurnalTidakLunas(transaction)

            transaction.Commit()
            DatabaseModule.CatatanAksiHistory("Simpan pembelian " & TxtFaktur.Text)

            ' Update stok
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso Not IsNothing(row.Cells("Id").Value) AndAlso row.Cells("Id").Value.ToString <> "" Then
                    HitungByKode(row.Cells("Id").Value)
                End If
            Next

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Transaksi dibatalkan karena kesalahan:" & vbCrLf &
                        ex.Message, "Gagal Simpan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Finally
            Cursor = Cursors.Default
        End Try

        ' CETAK DILUAR TRY utama agar tetap lanjut meski gagal
        Try
            If MessageBox.Show("Apakah Anda ingin mencetak Pembelian?", "Konfirmasi Cetak",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                NotaPembelian.TxtIdPembelian.Text = TxtFaktur.Text
                NotaPembelian.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show("Gagal mencetak pembelian. Anda bisa mencetak ulang nanti." & vbCrLf &
                        "Detail: " & ex.Message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        Finally
            ' Reset atau tutup
            If jenisTrans = "TambahPembelian" Then
                Kondisiawal()
            Else
                Me.Close()
            End If
        End Try


    End Sub


    Public Sub Tekanbatal()
        GBBayar.Visible = False
        TxtBayar.Text = 0
    End Sub


    Public Sub Hapusbelanja(ByVal transaction As MySqlTransaction)
        Dim updateStokField As String = String.Empty

        Select Case LblLokasiBarang.Text
            Case "TOKO"
                updateStokField = "PEMBELIAN_TOKO"
            Case "GUDANG"
                updateStokField = "PEMBELIAN_GUDANG"
        End Select

        Dim updateQuery As String = "UPDATE tbl_barang SET HARGA_BELI = ?, HARGA_BELI_TERAKHIR = ?, " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"


        For Each row As DataGridViewRow In FormUtama.DGVDetail.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
                Dim stokPengurangan As Decimal = If(row.Cells("QTY_SAT").Value IsNot Nothing, CDec(row.Cells("QTY_SAT").Value), 0)
                Dim Hargabeli As Decimal = If(IsDBNull(row.Cells("HARGA_AVERAGE").Value), 0, CDec(row.Cells("HARGA_AVERAGE").Value))
                Dim HARGA_BELI_SEBELUMNYA As Decimal = If(IsDBNull(row.Cells("HARGA_BELI_SEBELUMNYA").Value), 0, CDec(row.Cells("HARGA_BELI_SEBELUMNYA").Value))


                Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                    cmd.Parameters.AddWithValue("@Hargabeli", Hargabeli)
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SEBELUMNYA", HARGA_BELI_SEBELUMNYA)
                    cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                    cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                    cmd.ExecuteNonQuery()
                End Using

                HitungStokPerubahan(kodeBarang, transaction)
            End If


        Next

        Dim deleteQueries As String() = {
        "DELETE FROM pembelian WHERE ID_PEMBELIAN = @FakturPembelian",
        "DELETE FROM pembelian_detail WHERE FAKTUR_BELI = @FakturPembelian",
        "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FakturPembelian",
        "DELETE FROM HistoryBarang WHERE FAKTUR = @FakturPembelian"
    }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@FakturPembelian", TxtFaktur.Text)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Private Sub SimpanPembelian(ByVal transaction As MySqlTransaction)
        Dim sql As String
        ' Tentukan SQL berdasarkan status transaksi "Lunas"
        If LblStatusTrans.Text = "Lunas" Then
            sql = "INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, STATUS_JUAL, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER) " &
                  "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @PEMBAYARAN, @STATUS_JUAL, @STATUS_TRANSAKSI_BELI, @ID_USER, @ID_KOMPUTER)"
        Else
            sql = "INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, STATUS_JUAL, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER) " &
                  "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @PEMBAYARAN, @TAGIHAN, @JATUH_TEMPO, @STATUS_JUAL, @STATUS_TRANSAKSI_BELI, @ID_USER, @ID_KOMPUTER)"
        End If

        Using cmd As New MySqlCommand(sql, conn, transaction)
            ' Langsung gunakan If untuk nilai default 0 pada desimal di parameter AddWithValue
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@NOTA_PEMBELIAN", TxtNota.Text)
            cmd.Parameters.AddWithValue("@TGL_BELI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@JENIS_BAYAR", CmbJenisBayar.Text)
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_BELI", If(String.IsNullOrEmpty(TxtGrandtotal.Text), 0D, Convert.ToDecimal(TxtGrandtotal.Text)))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", If(String.IsNullOrEmpty(TxtTotalQTY.Text), 0D, Convert.ToDecimal(TxtTotalQTY.Text)))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", If(String.IsNullOrEmpty(TxtJmlhBrg.Text), 0D, Convert.ToDecimal(TxtJmlhBrg.Text)))
            cmd.Parameters.AddWithValue("@PEMBAYARAN", If(String.IsNullOrEmpty(TxtBayar.Text), 0D, Convert.ToDecimal(TxtBayar.Text)))

            ' Parameter tambahan berdasarkan status transaksi
            If LblStatusTrans.Text = "Lunas" Then
                cmd.Parameters.AddWithValue("@STATUS_JUAL", "TERBAYAR")
            Else
                cmd.Parameters.AddWithValue("@TAGIHAN", If(String.IsNullOrEmpty(TxtKembali.Text), 0D, Convert.ToDecimal(TxtKembali.Text)))
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", DTPJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@STATUS_JUAL", "TERHUTANG")
            End If

            ' Parameter tambahan lainnya
            cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI_BELI", LblStatusTrans.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.Comp.Text, TxtKomputer.Text))

            ' Eksekusi query
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub SimpanPembelianDetail(ByVal transaction As MySqlTransaction)
        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow OrElse row.Cells("Id").Value Is Nothing OrElse row.Cells("Id").Value.ToString().Trim() = "" Then Continue For

            ' Ambil data dari row
            Dim idBarang = row.Cells("Id").Value.ToString()
            Dim namaBarang = row.Cells("Nama").Value?.ToString()
            Dim hargaBeli = ParseDecimal(row.Cells("HargaBeli").Value)
            Dim hargaAverage = ParseDecimal(row.Cells("Average").Value)
            Dim hargaSebelumnya = ParseDecimal(row.Cells("HargaSebelumnya").Value)
            Dim qty = ParseDecimal(row.Cells("Qty").Value)
            Dim satuan = row.Cells("Satuan").Value?.ToString()
            Dim isi = SafeInt(row.Cells("Isi").Value, 1)
            Dim hargaBeliSat = ParseDecimal(row.Cells("HargaBeliSat").Value)
            Dim qtySat = ParseDecimal(row.Cells("QtySat").Value)
            Dim total = ParseDecimal(row.Cells("Totalharga").Value)

            ' Simpan detail pembelian
            Using cmd As New MySqlCommand("
            INSERT INTO pembelian_detail 
            (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER, ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, ID_USER, ID_KOMPUTER) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", conn, transaction)

                With cmd.Parameters
                    .AddWithValue("@FAKTUR_BELI", TxtFaktur.Text)
                    .AddWithValue("@NOTA_BELI", TxtNota.Text)
                    .AddWithValue("@TANGGAL_MASUK", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    .AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    .AddWithValue("@ID_SUPLIYER", LblKodeSupplier.Text)
                    .AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
                    .AddWithValue("@ID_BARANG", idBarang)
                    .AddWithValue("@NAMA_BARANG", namaBarang)
                    .AddWithValue("@HARGA_BELI", hargaBeli)
                    .AddWithValue("@HARGA_AVERAGE", hargaAverage)
                    .AddWithValue("@HARGA_BELI_SEBELUMNYA", hargaSebelumnya)
                    .AddWithValue("@QTY", qty)
                    .AddWithValue("@SATUAN", satuan)
                    .AddWithValue("@ISI_SATUAN", isi)
                    .AddWithValue("@HARGA_BELI_SATUAN", hargaBeliSat)
                    .AddWithValue("@QTY_SAT", qtySat)
                    .AddWithValue("@TOTAL", total)
                    .AddWithValue("@ID_USER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.SLogin.Text, TxtLogin.Text))
                    .AddWithValue("@ID_KOMPUTER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.Comp.Text, TxtKomputer.Text))
                End With
                cmd.ExecuteNonQuery()
            End Using

            ' Tentukan nama field stok
            Dim stokField As String = If(LblLokasiBarang.Text = "TOKO", "PEMBELIAN_TOKO", "PEMBELIAN_GUDANG")
            Dim hargaSatuan = If(isi = 0, 0, hargaBeli / isi)

            Select Case UpdateHargabeli
                Case "Harga Terbaru"
                    UpdateHargaTerbaru(idBarang, hargaSatuan, qtySat, stokField, transaction)

                Case "Metode Average (Rata - Rata)"
                    UpdateHargaAverage(idBarang, hargaSatuan, hargaAverage, qtySat, stokField, transaction)

                Case "Tidak Ada"
                    UpdateStokSaja(idBarang, qtySat, stokField, transaction)
            End Select
        Next
    End Sub


    Private Function SafeInt(value As Object, Optional defaultValue As Integer = 0) As Integer
        If IsDBNull(value) OrElse value Is Nothing OrElse Not IsNumeric(value) Then Return defaultValue
        Return Convert.ToInt32(value)
    End Function

    Private Sub UpdateHargaTerbaru(idBarang As String, harga As Decimal, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim sql As String = $"UPDATE tbl_barang SET KODE_SUPLIYER = ?, NAMA_SUPLIYER = ?, HARGA_BELI = ?, HARGA_BELI_TERAKHIR = ?, {stokField} = {stokField} + ? WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn, tr)
            cmd.Parameters.AddWithValue("@KODE_SUPLIYER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@HARGA_BELI", harga)
            cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", harga)
            cmd.Parameters.AddWithValue("@STOK", qtySat)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub UpdateHargaAverage(idBarang As String, hargaBaru As Decimal, hargaLama As Decimal, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim stokToko, stokGudang As Decimal
        Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn, tr)
            cmd.Parameters.AddWithValue("@ID", idBarang)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    stokToko = ParseDecimal(rd("STOK_TOKO"))
                    stokGudang = ParseDecimal(rd("STOK_GUDANG"))
                End If
            End Using
        End Using

        Dim totalStokLama = If(Updateberdasarkan = "Toko", stokToko, If(Updateberdasarkan = "Gudang", stokGudang, stokToko + stokGudang))
        Dim totalHargaLama = hargaLama * totalStokLama
        Dim totalHargaBaru = hargaBaru * qtySat
        Dim hargaAverageBaru = If(totalStokLama + qtySat = 0, hargaBaru, Math.Round((totalHargaLama + totalHargaBaru) / (totalStokLama + qtySat), 0))

        Dim sql As String = $"UPDATE tbl_barang SET KODE_SUPLIYER = ?, NAMA_SUPLIYER = ?, HARGA_BELI = ?, HARGA_BELI_TERAKHIR = ?, {stokField} = {stokField} + ? WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn, tr)
            cmd.Parameters.AddWithValue("@KODE_SUPLIYER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@HARGA_BELI", hargaAverageBaru)
            cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", hargaBaru)
            cmd.Parameters.AddWithValue("@STOK", qtySat)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub UpdateStokSaja(idBarang As String, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim sql As String = $"UPDATE tbl_barang SET KODE_SUPLIYER = ?, NAMA_SUPLIYER = ?, {stokField} = {stokField} + ? WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn, tr)
            cmd.Parameters.AddWithValue("@KODE_SUPLIYER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@STOK", qtySat)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "PEMBELIAN")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells("Id").Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells("Nama").Value)
                    cmd.Parameters.AddWithValue("@QTY", If(IsDBNull(row.Cells("Qty").Value), 0, Convert.ToDecimal(row.Cells("Qty").Value)))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells("Satuan").Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", If(IsDBNull(row.Cells("Isi").Value), 0, Convert.ToDecimal(row.Cells("Isi").Value)))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", If(IsDBNull(row.Cells("QtySat").Value), 0, Convert.ToDecimal(row.Cells("QtySat").Value)))
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", If(IsDBNull(row.Cells("Totalharga").Value), 0, Convert.ToDecimal(row.Cells("Totalharga").Value)))
                    cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.SLogin.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.Comp.Text, TxtKomputer.Text))
                    cmd.ExecuteNonQuery()
                End Using

            End If
        Next
    End Sub



    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        'simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                 "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", TxtNota.Text)

            If LblStatusTrans.Text = "Lunas" Then
                cmd.Parameters.AddWithValue("@URAIAN", "Pembayaran lunas belanja ke " & TxtSupplier.Text)
            Else
                cmd.Parameters.AddWithValue("@URAIAN", "Uang muka pembayaran belanja ke " & TxtSupplier.Text)
            End If

            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", CmbJenisBayar.Text)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", TxtKodeRek.Text)
            cmd.Parameters.AddWithValue("@NOMINAL", CDec(TxtBayar.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Pembelian")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using




    End Sub

    Private Sub SimpanjurnalTidakLunas(ByVal transaction As MySqlTransaction)
        'simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                       "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_K, @KODE_BANTU_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", TxtNota.Text)
            cmd.Parameters.AddWithValue("@URAIAN", "Hutang belanja ke " & TxtSupplier.Text & " Jatuh tempo " & DTPTgl.Value.ToString("dd MMMM yyyy"))
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", nama_rek_Hutang_Beli)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", Kode_rek_Hutang_Beli)
            cmd.Parameters.AddWithValue("@NAMA_BANTU_K", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@KODE_BANTU_K", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NOMINAL", CDec(TxtKembali.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Pembelian")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenisTrans.Text = "TambahPembelian", FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using

    End Sub




    Private Sub AmbilDaftarBarangEditpembelian()
        ' Kosongkan baris yang ada di DataGridView
        DgvData.Rows.Clear()

        Dim queryPembelian As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY_SAT FROM pembelian_detail WHERE FAKTUR_BELI = ?"
        Using cmd As New MySqlCommand(queryPembelian, conn)
            cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Proses setiap record dari data reader
                While rd.Read()
                    ' Tambahkan baris baru ke DataGridView
                    Dim baris As DataGridViewRow = DirectCast(DgvData.Rows(DgvData.Rows.Add()), DataGridViewRow)

                    ' Isi nilai ke sel baris berdasarkan nama kolom
                    baris.Cells("Id").Value = If(IsDBNull(rd("ID_BARANG")), 0, rd("ID_BARANG").ToString())
                    baris.Cells("Nama").Value = If(IsDBNull(rd("NAMA_BARANG")), String.Empty, rd("NAMA_BARANG").ToString())
                    baris.Cells("Hargabeli").Value = If(IsDBNull(rd("HARGA_BELI")), 0D, Convert.ToDecimal(rd("HARGA_BELI")))
                    baris.Cells("Qty").Value = If(IsDBNull(rd("QTY")), 0D, Convert.ToDecimal(rd("QTY")))
                    baris.Cells("Isi").Value = If(IsDBNull(rd("ISI_SATUAN")), 0, Convert.ToInt32(rd("ISI_SATUAN")))
                    baris.Cells("HargaBeliSat").Value = If(IsDBNull(rd("HARGA_BELI_SATUAN")), 0D, Convert.ToDecimal(rd("HARGA_BELI_SATUAN")))
                    baris.Cells("QtySat").Value = If(IsDBNull(rd("QTY_SAT")), 0D, Convert.ToDecimal(rd("QTY_SAT")))
                    baris.Cells("Totalharga").Value = If(IsDBNull(rd("TOTAL")), 0D, Convert.ToDecimal(rd("TOTAL")))
                    baris.Cells("Average").Value = If(IsDBNull(rd("HARGA_AVERAGE")), 0D, Convert.ToDecimal(rd("HARGA_AVERAGE")))
                    baris.Cells("HargaSebelumnya").Value = If(IsDBNull(rd("HARGA_BELI_SEBELUMNYA")), 0D, Convert.ToDecimal(rd("HARGA_BELI_SEBELUMNYA")))
                    baris.Cells("QtySebelumnya").Value = If(IsDBNull(rd("QTY_SAT")), 0D, Convert.ToDecimal(rd("QTY_SAT")))
                End While
            End Using
        End Using

        ' Mengisi ComboBoxCell dengan satuan yang sesuai
        For Each baris As DataGridViewRow In DgvData.Rows
            ' Lewati placeholder baris baru jika ada
            If baris.IsNewRow Then Continue For

            Dim idBarang As String = baris.Cells("Id").Value.ToString()
            Dim comboCell As DataGridViewComboBoxCell = CType(baris.Cells("Satuan"), DataGridViewComboBoxCell)
            comboCell.Items.Clear()

            ' Ambil daftar satuan umum dari barang
            Dim query As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = ?"
            Using cmdSatuan As New MySqlCommand(query, conn)
                cmdSatuan.Parameters.AddWithValue("@ID_BARANG", idBarang)

                Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                    While rdSatuan.Read()
                        ' Isi combo dengan satuan yang ditemukan
                        For i As Integer = 0 To 2
                            Dim satuan As String = rdSatuan(i).ToString()
                            If Not String.IsNullOrEmpty(satuan) Then
                                comboCell.Items.Add(satuan)
                            End If
                        Next
                    End While
                End Using
            End Using

            ' Mengisi ComboBoxCell dengan satuan dari pembelian detail
            query = "SELECT SATUAN FROM pembelian_detail WHERE ID_BARANG = ? AND FAKTUR_BELI = ?"
            Using cmdSatuan As New MySqlCommand(query, conn)
                cmdSatuan.Parameters.AddWithValue("@ID_BARANG", idBarang)
                cmdSatuan.Parameters.AddWithValue("@FAKTUR_BELI", TxtFaktur.Text)

                Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                    While rdSatuan.Read()
                        Dim satuan As String = If(IsDBNull(rdSatuan("SATUAN")), String.Empty, rdSatuan("SATUAN").ToString())
                        If Not String.IsNullOrEmpty(satuan) Then
                            ' Cek apakah satuan sudah ada dalam daftar dan set sebagai nilai default
                            If comboCell.Items.Contains(satuan) Then
                                comboCell.Value = satuan
                            End If
                        End If
                    End While
                End Using
            End Using
        Next

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
    End Sub

    Private Sub AmbilDataPembelian()
        Dim queryString As String = "SELECT ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, PEMBAYARAN, ID_USER, ID_KOMPUTER FROM pembelian WHERE ID_PEMBELIAN = ?"

        Dim IDSupplier As String
        Dim NamaSupplier As String = String.Empty
        Dim NotaPembelian As String = String.Empty
        Dim TanggalBeli As Date = Date.MinValue
        Dim Lokasi As String = String.Empty
        Dim JenisBayar As String = String.Empty
        Dim IDUser As String = String.Empty
        Dim IDKomputer As String = String.Empty

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    rd.Read()
                    IDSupplier = rd("ID_SUPPLIER").ToString()
                    NamaSupplier = rd("NAMA_SUPLIYER").ToString()
                    NotaPembelian = rd("NOTA_PEMBELIAN").ToString()
                    TanggalBeli = CDate(rd("TGL_BELI"))
                    Lokasi = rd("LOKASI").ToString()
                    JenisBayar = rd("JENIS_BAYAR").ToString()
                    IDUser = rd("ID_USER").ToString()
                    IDKomputer = rd("ID_KOMPUTER").ToString()
                Else
                    MessageBox.Show("Data tidak ditemukan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If
            End Using
        End Using

        ' Ambil data dari variabel
        TxtSupplier.Text = NamaSupplier
        TxtNota.Text = NotaPembelian
        DTPTgl.Value = TanggalBeli
        LblLokasiBarang.Text = Lokasi
        CmbJenisBayar.Text = JenisBayar
        TxtLogin.Text = IDUser
        TxtKomputer.Text = IDKomputer
    End Sub


End Class