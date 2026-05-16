Public Class FormPembelianBackup
    Private ReadOnly tempatSimpan As String
    Private draftPembelianAktif As String = ""

    ''' <summary>
    ''' Variabel untuk menyimpan total selisih nilai persediaan akibat perubahan harga pokok barang
    ''' Digunakan untuk mencatat jurnal penyesuaian harga pokok (Requirement 21)
    ''' </summary>
    Private _totalSelisihHargaPokok As Decimal = 0D

    Private ReadOnly Property IsModeTambahPembelian As Boolean
        Get
            Return String.Equals(TxtJenisTrans.Text, "TambahPembelian", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    Private Sub Form_Pembelian_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Area input dan grand total otomatis via nama kontrol
        ' Rename GroupBox -> GBInput/GBTotal untuk tema otomatis
        ' Panel4 = header hijau transaksi — jaga warna & pastikan BtnClose di atas label
        Panel4.BackColor = ModuleTheme.C(
        System.Drawing.Color.FromArgb(22, 163, 74),
        System.Drawing.Color.FromArgb(20, 83, 45))
        LblUtama.ForeColor = System.Drawing.Color.White
        BtnKeluarForm.BringToFront()
        LblLokasiBarang.Text = FormUtama.StatusLokasi.Text
        MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        MinimumSize = Size


        KosongTxtboxcari()

        ' Setup timer untuk deteksi barcode
        barcodeTimer.Interval = 100 ' 100ms timeout
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick


        ' Setting dibaca langsung dari ModulHakAkses property — tidak perlu cache lokal
        LblUpdateHarga.Text = "Metode update hpp : " & ModulHakAkses.SettingMetodeUpdateHargaBeli & " dari stok " & ModulHakAkses.SettingAverageHargaBerdasarkanStok

        If Not ModulHakAkses.SettingIzinkanUbahHargaBeli Then
            DgvData.Columns("Hargabeli").ReadOnly = True
        Else
            DgvData.Columns("Hargabeli").ReadOnly = False
        End If

        If IsModeTambahPembelian Then
            TxtSupplier.Clear()
            KosongkanDataSupplier()
            ' Hapus semua item dan tambahkan yang baru
            IsiComboBoxAkun(CmbJenisBayarTunai, "KAS", "EKUITAS")
            IsiComboBoxAkun(CmbJenisBayarTransfer, "BANK")

            If LblLokasiBarang.Text = "TOKO" Then
                CmbJenisBayarTunai.SelectedItem = nama_rek_Beli_toko
            ElseIf LblLokasiBarang.Text = "GUDANG" Then
                CmbJenisBayarTunai.SelectedItem = nama_rek_Beli_Gudang
            End If
            Kondisiawal()
            AmbilKodeAkun()
        Else
            Kondisiawaledit()
            AmbilDataPembelian()
            AmbilDaftarBarangEditpembelian()
        End If
        BtnTahan.Visible = IsModeTambahPembelian
        BtnPanggil.Visible = IsModeTambahPembelian
        JumlahTahanPembelian()
    End Sub


    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Warna fokus ikut tema — biru muda (light) / Slate-600 (dark)
        PanelCariNama.BackColor = ModuleTheme.C(
        Color.FromArgb(219, 234, 254),
        Color.FromArgb(71, 85, 105))

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
        ' Kembalikan ke warna area input sesuai tema
        PanelCariNama.BackColor = ModuleTheme.C(
        ModuleTheme.L_TransInput,
        ModuleTheme.D_TransInput)
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
        BtnTahan.Visible = False
        BtnPanggil.Visible = False

        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        DTPJatuhTempo.Format = DateTimePickerFormat.Custom
        DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"

        ' ✅ Reset semua field pembayaran agar tidak ada nilai stale
        TxtNominalBayarTunai.Text = "0"
        TxtKembaliHutang.Text = "0"
        TxtBAntuanbayar.Text = "0"

        ' ✅ Reset total selisih harga pokok (Requirement 21)
        _totalSelisihHargaPokok = 0D
    End Sub

    Private Sub Kondisiawal()
        DgvData.Rows.Clear()
        TxtSupplier.Clear()
        TxtNota.Clear()
        TxtNominalBayarTunai.Text = "0"
        TxtNominalBayarTransfer.Text = "0"
        TxtKembaliHutang.Text = "0"
        TxtBAntuanbayar.Text = "0"
        TxtTotal.Text = "0"
        TxtGrandtotal.Text = "0"
        TxtJmlhBrg.Clear()
        TxtTotalQTY.Text = "0"
        Txtlihattotal.Text = "Rp. 0"
        LblRecord.Text = "Total record : 0"
        LblStatusTransLunas.Text = "Lunas"
        LblPembayaran.Text = "Kembalian :"
        LblJatuhTempo.Visible = False
        DTPJatuhTempo.Visible = False
        GBBayar.Visible = False

        ' ✅ Reset total selisih harga pokok (Requirement 21)
        _totalSelisihHargaPokok = 0D

        ' Reset diskon, PPN, biaya kirim
        TxtDiskonPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtDiskonRp.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakRp.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtBiayaKirim.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblDiskonRp.Text = "Rp. 0"
        LblPajakRp.Text = "Rp. 0"
        LblBiayaKirim.Text = "Rp. 0"

        DTPTgl.Value = DateTime.Now
        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        DTPTgl.Enabled = ModulHakAkses.SettingIzinkanTanggalLampau

        Dim newDate As Date = DTPTgl.Value.AddMonths(1)
        DTPJatuhTempo.Value = newDate
        DTPJatuhTempo.Format = DateTimePickerFormat.Custom
        DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"

        NomorBeli()
        AmbilDataSupplier()
        draftPembelianAktif = ""
        BtnTahan.Visible = True
        BtnPanggil.Visible = True

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
            If ModulHakAkses.SettingFokusOtomatis Then
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
        Using cmd As New MySqlCommand(
        "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "PB")
            cmd.Parameters.AddWithValue("@tgl", DTPTgl.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "pembelian")
            cmd.Parameters.AddWithValue("@kolom", "ID_PEMBELIAN")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            TxtFaktur.Text = pNomor.Value?.ToString()
        End Using
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

        Using cmd As New MySqlCommand("SELECT KODE, NAMA, ALAMAT, HP, JangkaHutang FROM tbl_supliyer WHERE Status = 'Aktif' ORDER BY NAMA", conn)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim jangka As Integer = ModuleAngka.SafeGetValue(Of Integer)(rd, "JangkaHutang", 30)
                    If jangka <= 0 Then jangka = 30
                    ListDataSupplier.Add(New DataSupplier With {
                .Kode = ModuleAngka.SafeGetValue(Of String)(rd, "KODE", ""),
                .Nama = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA", ""),
                .Alamat = ModuleAngka.SafeGetValue(Of String)(rd, "ALAMAT", ""),
                .HP = ModuleAngka.SafeGetValue(Of String)(rd, "HP", ""),
                .JangkaHutang = jangka
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

        If Not IsModeTambahPembelian Then
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

    Private Sub ListSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles listSupplier.KeyDown
        If e.KeyCode = Keys.Enter Then
            PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
        End If
    End Sub

    Private Sub ListSupplier_Click(sender As Object, e As EventArgs) Handles listSupplier.Click
        PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
    End Sub

    Private Sub CmbJenisBayar_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbJenisBayarTunai.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtNominalBayarTunai.Select()
        End If
    End Sub


    Private Sub CmbJenisBayar_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbJenisBayarTunai.SelectedIndexChanged
        AmbilKodeAkun()
    End Sub

    Private Sub AmbilKodeAkun()
        Dim namaAkunD As String = CmbJenisBayarTunai.Text

        Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtJenisBayarTunai.Text = reader("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub


    Public Sub UpdateSemuaTotal()
        ' Hitung subtotal item dari DGV
        Dim subtotalItem As Decimal = 0
        For i As Integer = 0 To DgvData.Rows.Count - 1
            If DgvData.Rows(i).Cells("Totalharga").Value IsNot Nothing Then
                subtotalItem += ModuleAngka.ParseDecimal(DgvData.Rows(i).Cells("Totalharga").Value)
            End If
        Next

        ' Hitung Jumlah Barang
        Dim totalRows As Integer = 0
        For i As Integer = 0 To DgvData.Rows.Count - 1
            Dim qtyValue As Object = DgvData.Rows(i).Cells("Qty").Value
            If Not DgvData.Rows(i).IsNewRow AndAlso qtyValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyValue.ToString()) Then
                totalRows += 1
            End If
        Next
        TxtJmlhBrg.Text = totalRows.ToString()
        LblRecord.Text = "Total record : " & totalRows.ToString()

        ' Hitung Total QTY
        Dim totalQty As Decimal = 0
        For i As Integer = 0 To DgvData.Rows.Count - 1
            If DgvData.Rows(i).Cells("QtySat").Value IsNot Nothing Then
                totalQty += ModuleAngka.ParseDecimal(DgvData.Rows(i).Cells("QtySat").Value)
            End If
        Next
        TxtTotalQTY.Text = totalQty.ToString()

        ' Hitung grand total dengan diskon/PPN/biaya lalu update semua display
        HitungGrandTotalBeli()

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
    Private isUpdatingDiskon As Boolean = False  ' Guard agar tidak loop event diskon
    Private isUpdatingPajak As Boolean = False   ' Guard agar tidak loop event pajak


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
        Dim qty = ModuleAngka.ParseDecimal(qtyStr)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = satuanStr
    End Sub

    Private Sub SetQtyOnly(qtyStr As String)
        Dim qty = ModuleAngka.ParseDecimal(qtyStr)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = "1"
    End Sub

    Private Sub SetDefaultQtyAndSatuan()
        TxtQty.Text = "1"
        TxtLevelSat.Text = "1"
    End Sub

    ' ParseDecimalSafe lokal dihapus — gunakan ModuleAngka.ParseDecimal

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
           "WHERE STATUS = 'Aktif' AND (BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc) LIMIT 1"

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

            If ModuleAngka.ParseDecimal(TxtQty.Text) <= 0 Then
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

        Dim query As String = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @Nama OR NAMA_BARANG LIKE @Nama OR BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama OR BARCODE_BESAR LIKE @Nama) ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim itemText As String = rd("NAMA_BARANG").ToString()
                    Select Case LblLokasiBarang.Text
                        Case "TOKO"
                            Dim stokToko As Decimal = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                            itemText &= " => " & stokToko.ToString("N0")
                        Case "GUDANG"
                            Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
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
                    Else
                        ' Sesuaikan nilai berdasarkan barcode
                        If TxtLevelSat.Text = "1" Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                            isiUmum = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)
                        ElseIf TxtLevelSat.Text = "2" Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", String.Empty)
                            isiUmum = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 0)
                        ElseIf TxtLevelSat.Text = "3" Then
                            satuanUmum = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", String.Empty)
                            isiUmum = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 0)
                        End If
                    End If
                    Dim Average As String = ModuleAngka.ParseDecimal(rd("HARGA_BELI")).ToString()
                    Dim HargaSebelumnya As String = ModuleAngka.ParseDecimal(rd("HARGA_BELI_TERAKHIR")).ToString()

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
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
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
        Dim qty As Decimal = If(ModuleAngka.ParseDecimal(TxtQty.Text) > 0, ModuleAngka.ParseDecimal(TxtQty.Text), 1D)
        Dim satuan As String = Txtsatuan.Text

        'Dim satuan As String = kolomSatuan.Items(0).ToString()
        Dim isi As Decimal = ModuleAngka.ParseDecimal(TxtIsi.Text)
        Dim HPP As Decimal = hargaBeli * isi
        Dim Average As Decimal = ModuleAngka.ParseDecimal(TxtAverage.Text)
        Dim HargaSebelumnya As Decimal = ModuleAngka.ParseDecimal(TxtHargaSebelumnya.Text)

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
        Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(indeksBaris).Cells("Hargabeli").Value)
        Dim qtyBarang As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(indeksBaris).Cells("QTY").Value)
        Dim isiBarang As Integer = ModuleAngka.ParseInteger(DgvData.Rows(indeksBaris).Cells("Isi").Value, defaultValue:=1)

        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qtyBarang * isiBarang
        DgvData.Rows(indeksBaris).Cells("TotalHarga").Value = hargaBeli * qtyBarang
    End Sub



    ' ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal

    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        '========================== Nama
        If e.ColumnIndex = 1 Then
            If DgvData.Rows(e.RowIndex) IsNot Nothing AndAlso DgvData.Rows(e.RowIndex).Cells("Nama") IsNot Nothing Then
                ' ✅ Tambahkan pengecekan Value sebelum ToString()
                If DgvData.Rows(e.RowIndex).Cells("Nama").Value IsNot Nothing AndAlso Not IsDBNull(DgvData.Rows(e.RowIndex).Cells("Nama").Value) Then
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
                        qtyValue = ModuleAngka.ParseDecimal(angkaQty) ' Gunakan fungsi yang sudah dibuat
                        namaBarangValue = inputText.Substring(indexAsteriskHarga + 1).Trim()

                    ElseIf indexAsteriskQty >= 0 Then
                        ' Format: qty * namaBarang
                        Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                        qtyValue = ModuleAngka.ParseDecimal(angkaQty) ' Gunakan fungsi yang sudah dibuat
                        namaBarangValue = inputText.Substring(indexAsteriskQty + 1).Trim()
                    End If

                    DgvData.Rows(e.RowIndex).Cells("Nama").Value = namaBarangValue

                    Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG LIKE @NamaBarang OR NAMA_BARANG LIKE @NamaBarang OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang)"

                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@NamaBarang", namaBarangValue)
                        Using rd As MySqlDataReader = cmd.ExecuteReader()
                            If rd.HasRows Then
                                rd.Read() ' Lanjutkan ke data pertama
                                DgvData.Rows(e.RowIndex).Cells("Id").Value = rd("ID_BARANG")
                                DgvData.Rows(e.RowIndex).Cells("nama").Value = rd("NAMA_BARANG")

                                Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(e.RowIndex).Cells("Satuan"), DataGridViewComboBoxCell)
                                comboCell.Items.Clear()

                                Dim satuanKecil As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                Dim satuanSedang As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                                Dim satuanBesar As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")

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

                                Dim namaBarang As String = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_BARANG", "")
                                Dim barcodeKecil As String = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_KECIL", "")
                                Dim barcodeSedang As String = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_SEDANG", "")
                                Dim barcodeBesar As String = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_BESAR", "")

                                If namaBarangValue = barcodeSedang Then
                                    satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                                    isi = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1)
                                ElseIf namaBarangValue = barcodeBesar Then
                                    satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                                    isi = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1)
                                Else
                                    satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                    isi = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)
                                End If

                                DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI_TERAKHIR", 0D) * isi
                                DgvData.Rows(e.RowIndex).Cells("HargaBeliSat").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D) * isi

                                DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuan
                                DgvData.Rows(e.RowIndex).Cells("isi").Value = isi
                                If DgvData.Rows(e.RowIndex).Cells("isi").Value = 0 Then
                                    DgvData.Rows(e.RowIndex).Cells("qtysat").Value = 1
                                End If

                                DgvData.Rows(e.RowIndex).Cells("qty").Value = qtyValue

                                DgvData.Rows(e.RowIndex).Cells("Average").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                                DgvData.Rows(e.RowIndex).Cells("HargaSebelumnya").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI_TERAKHIR", 0D)

                                If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
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
                Else
                    MsgBox("Nama barang tidak boleh kosong. Mohon masukkan nama barang.", vbExclamation, "Kesalahan Input")
                End If
                HitungNilaiSetiapBaris(e.RowIndex)
            End If
        End If
        '========================== Harga beli
        If e.ColumnIndex = 5 Then
            Dim hargaBeliValue As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value)

            ' Validasi nilai Harga Beli
            If hargaBeliValue <= 0 Then
                MessageBox.Show("Harga beli harus lebih besar dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = 0
            Else

                HitungNilaiSetiapBaris(e.RowIndex)

                ' Ambil nilai Qty
                Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("Qty").Value)

                ' Ambil nilai Isi
                Dim isiValue As Integer = ModuleAngka.ParseInteger(DgvData.Rows(e.RowIndex).Cells("isi").Value, defaultValue:=1)

                ' Hitung Harga Beli per satuan
                Dim hargaBeliPerSatuan As Decimal = If(isiValue = 0, 0D, hargaBeliValue / isiValue)

                ' Hitung Qty dalam satuan
                Dim qtySatValue As Decimal = qtyValue * isiValue

                'DgvData.Rows(e.RowIndex).Cells("QtySat").Value = qtySatValue
                'DgvData.Rows(e.RowIndex).Cells("HargaBeliSat").Value = CDec(DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value) * isiValue
                'DgvData.Rows(e.RowIndex).Cells("Totalharga").Value = hargaBeliValue * qtyValue

                If ModulHakAkses.SettingBeliOtomatisUpdateHargaJual Then
                    Dim hargaLama As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("Average").Value)
                    Dim QtySbl As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("QtySebelumnya").Value)


                    With TambahBarang
                        .LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN"
                        .GBInput1.Visible = False
                        .GBInput4.Visible = False
                        .GBInput.Enabled = False
                        .GBInput5.Visible = False
                        .PanelInfoRubahHarga.Visible = True
                        .BtnTambahKategori.Visible = False
                        .BtnTambahSupliyer.Visible = False
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

                        If ModulHakAkses.SettingMetodeUpdateHargaBeli = "Metode Average (Rata - Rata)" Then
                            .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli & " " & ModulHakAkses.SettingAverageHargaBerdasarkanStok
                            .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                            .LblJenis.Text = ModulHakAkses.SettingAverageHargaBerdasarkanStok
                        Else
                            .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                            .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                        End If
                        .ShowDialog()
                    End With

                End If
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
        Using cmd As New MySqlCommand("SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @searchTerm", conn)
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
                            isiValue = ModuleAngka.ParseDecimal(rd("ISI_UMUM_KECIL"))
                        Case 1
                            isiValue = ModuleAngka.ParseDecimal(rd("ISI_UMUM_SEDANG"))
                        Case Else
                            isiValue = ModuleAngka.ParseDecimal(rd("ISI_UMUM_BESAR"))
                    End Select
                    If isiValue = 0 Then isiValue = 1
                    cell.OwningRow.Cells("Isi").Value = isiValue

                    ' Konversi nilai harga beli
                    Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    Dim hargaBeliterakhir As Decimal = ModuleAngka.ParseDecimal(rd("HARGA_BELI_TERAKHIR"))

                    ' Dapatkan indeks baris
                    Dim rowIndex As Integer = DgvData.CurrentCell.RowIndex

                    ' Hitung nilai lainnya
                    Dim isiQty As Decimal = ModuleAngka.ParseDecimal(DgvData("isi", rowIndex).Value)
                    If isiQty = 0 Then isiQty = 1
                    Dim qty As Decimal = ModuleAngka.ParseDecimal(DgvData("qty", rowIndex).Value)
                    If qty = 0 Then qty = 1

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


    Private Sub BtnKeluarForm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluarForm.Click
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
            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("Qty").Value)
            Dim isiValue As Integer = ModuleAngka.ParseInteger(DgvData.Rows(rowIndex).Cells("isi").Value, defaultValue:=1)
            Dim hargaBeliValue As Decimal = If(isiValue = 0, 0D, ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("Hargabeli").Value) / isiValue)
            Dim qtySatValue As Decimal = qtyValue * isiValue

            DgvData.Rows(rowIndex).Cells("QtySat").Value = qtySatValue
            DgvData.Rows(rowIndex).Cells("HargaBeliSat").Value = hargaBeliValue * isiValue
            DgvData.Rows(rowIndex).Cells("Totalharga").Value = hargaBeliValue * qtyValue

            Dim hargaLama As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("Average").Value)
            Dim QtySbl As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("QtySebelumnya").Value)

            With TambahBarang
                .LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN"
                .GBInput1.Visible = False
                .GBInput4.Visible = False
                .GBInput.Enabled = False
                .GBInput5.Visible = False
                .PanelInfoRubahHarga.Visible = True
                .BtnTambahKategori.Visible = False
                .BtnTambahSupliyer.Visible = False
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

                If ModulHakAkses.SettingMetodeUpdateHargaBeli = "Metode Average (Rata - Rata)" Then
                    .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli & " " & ModulHakAkses.SettingAverageHargaBerdasarkanStok
                    .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                    .LblJenis.Text = ModulHakAkses.SettingAverageHargaBerdasarkanStok
                Else
                    .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                    .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                End If
                .ShowDialog()
                .TxtHArgaJUalUmumKecil.Focus()
            End With

        End If
    End Sub



    Private Sub TxtGrandtotal_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtGrandtotal.TextChanged
        ' TxtGrandtotal diisi plain oleh HitungGrandTotalBeli() yang sudah update Txtlihattotal dan TxtTotal.
        ' Handler ini tidak perlu melakukan apa-apa lagi.
    End Sub

    ' ── Handler KeyDown — hanya izinkan angka, backspace, delete, titik ────
    Private Sub TxtDiskon_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtDiskonRp.KeyDown, TxtDiskonPersen.KeyDown
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
       (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
       Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtPajak_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtPajakRp.KeyDown, TxtPajakPersen.KeyDown
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
       (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
       Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ── Diskon TextChanged — dua arah: persen ↔ Rp ──────────────────────
    Private Sub TxtDiskon_TextChanged(sender As Object, e As EventArgs) Handles TxtDiskonRp.TextChanged, TxtDiskonPersen.TextChanged
        If sender Is TxtDiskonRp Then
            HitungDiskon("diskonrupiah")
        ElseIf sender Is TxtDiskonPersen Then
            HitungDiskon("diskonpersen")
        End If
    End Sub

    Private Sub HitungDiskon(sumber As String)
        If isUpdatingDiskon Then Exit Sub
        isUpdatingDiskon = True

        Dim subtotalItem As Decimal = HitungSubtotalItem()
        Dim diskonPersen As Decimal = ModuleAngka.ParseDecimal(TxtDiskonPersen.Text)
        Dim diskonRupiah As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRp.Text)

        Select Case sumber.ToLower()
            Case "diskonpersen"
                diskonPersen = Math.Min(diskonPersen, 100)
                diskonRupiah = Math.Round(subtotalItem * diskonPersen / 100, 0)
                ' TextBox input — format plain (InvariantCulture)
                TxtDiskonRp.Text = diskonRupiah.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            Case "diskonrupiah"
                diskonPersen = If(subtotalItem = 0, 0, Math.Round((diskonRupiah / subtotalItem) * 100, 2))
                TxtDiskonPersen.Text = diskonPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        End Select

        ' Label display — format Indonesia
        LblDiskonRp.Text = "Rp. " & diskonRupiah.ToString("#,0.##", cultureIndonesia)

        HitungGrandTotalBeli()
        isUpdatingDiskon = False
    End Sub

    ' ── PPN TextChanged — dua arah: persen ↔ Rp ─────────────────────────
    Private Sub TxtPajak_TextChanged(sender As Object, e As EventArgs) Handles TxtPajakRp.TextChanged, TxtPajakPersen.TextChanged
        If sender Is TxtPajakRp Then
            HitungPajak("pajakrupiah")
        ElseIf sender Is TxtPajakPersen Then
            HitungPajak("pajakpersen")
        End If
    End Sub

    Private Sub HitungPajak(sumber As String)
        If isUpdatingPajak Then Exit Sub
        isUpdatingPajak = True

        ' PPN dihitung dari subtotal setelah diskon (sama seperti penjualan)
        Dim subtotalItem As Decimal = HitungSubtotalItem()
        Dim diskonRupiah As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRp.Text)
        Dim dasarPPN As Decimal = subtotalItem - diskonRupiah

        Dim pajakPersen As Decimal = ModuleAngka.ParseDecimal(TxtPajakPersen.Text)
        Dim pajakRupiah As Decimal = ModuleAngka.ParseDecimal(TxtPajakRp.Text)

        Select Case sumber.ToLower()
            Case "pajakpersen"
                pajakPersen = Math.Min(pajakPersen, 100)
                pajakRupiah = Math.Round(dasarPPN * pajakPersen / 100, 0)
                TxtPajakRp.Text = pajakRupiah.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            Case "pajakrupiah"
                pajakPersen = If(dasarPPN = 0, 0, Math.Round((pajakRupiah / dasarPPN) * 100, 2))
                TxtPajakPersen.Text = pajakPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        End Select

        ' Label display — format Indonesia
        LblPajakRp.Text = "Rp. " & pajakRupiah.ToString("#,0.##", cultureIndonesia)

        HitungGrandTotalBeli()
        isUpdatingPajak = False
    End Sub

    ' ── Biaya kirim TextChanged ───────────────────────────────────────────
    Private Sub TxtBiayaKirim_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtBiayaKirim.TextChanged
        Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
        ' Label display — format Indonesia
        LblBiayaKirim.Text = biayaKirim.ToString("#,0.##", cultureIndonesia)
        HitungGrandTotalBeli()
    End Sub

    ''' <summary>Hitung grand total = subtotalItem - diskon + ppn + biayaKirim dan update semua display.</summary>
    Private Sub HitungGrandTotalBeli()
        Dim subtotalItem As Decimal = HitungSubtotalItem()
        Dim diskon As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRp.Text)
        Dim ppn As Decimal = ModuleAngka.ParseDecimal(TxtPajakRp.Text)
        Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
        Dim grandTotal As Decimal = subtotalItem - diskon + ppn + biayaKirim

        ' TextBox internal — plain, desimal dipertahankan jika ada (sumber kebenaran untuk simpan ke DB)
        TxtGrandtotal.Text = grandTotal.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        ' TxtTotal di panel bayar — sama, plain
        TxtTotal.Text = grandTotal.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        ' Display utama — format Rupiah Indonesia (hanya untuk mata user, tidak dibaca logika)
        Txtlihattotal.Text = "Rp. " & ModuleAngka.FormatRupiah(grandTotal)
    End Sub

    ''' <summary>Hitung subtotal item dari DGV tanpa diskon/PPN/biaya.</summary>
    Private Function HitungSubtotalItem() As Decimal
        Dim total As Decimal = 0
        For i As Integer = 0 To DgvData.Rows.Count - 1
            If DgvData.Rows(i).Cells("Totalharga").Value IsNot Nothing Then
                total += ModuleAngka.ParseDecimal(DgvData.Rows(i).Cells("Totalharga").Value)
            End If
        Next
        Return total
    End Function

    Private Sub TxtBayar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtNominalBayarTunai.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TxtNominalBayarTunai_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayarTunai.TextChanged
        ' Handler ini digabung ke TxtBayar_TextChanged di bawah — dihapus untuk menghindari duplikat
    End Sub

    Private Sub TxtBAntuanbayar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtBAntuanbayar.TextChanged
        Dim bantuanBayar As Decimal = ModuleAngka.ParseDecimal(TxtBAntuanbayar.Text)
        If bantuanBayar > 0 Then
            LblPembayaran.Text = "Hutang :"
            LblJatuhTempo.Visible = True
            DTPJatuhTempo.Visible = True
            LblStatusTransLunas.Text = "Belum Lunas"
            TxtKembaliHutang.Visible = True
            LblPembayaran.Visible = True
        Else
            LblPembayaran.Text = "Kembalian :"
            LblJatuhTempo.Visible = False
            DTPJatuhTempo.Visible = False
            LblStatusTransLunas.Text = "Lunas"
            TxtKembaliHutang.Visible = False
            LblPembayaran.Visible = False
        End If
    End Sub


    Private Sub Form_Pembelian_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                Tekanbayar()
            Case Keys.F2
                TxtSupplier.Select()
            Case Keys.F3
                Tekansupliyer()
            Case Keys.F4
                Tekanbarang()
            Case Keys.F5
            Case Keys.F6
                If IsModeTambahPembelian Then Tekantahan()
            Case Keys.F7
                If IsModeTambahPembelian Then Tekanpanggil()
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
                CmbJenisBayarTunai.Select()
                CmbJenisBayarTunai.DroppedDown = True
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

    Private Sub BtnTahan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTahan.Click
        Tekantahan()
    End Sub

    Private Sub BtnPanggil_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPanggil.Click
        Tekanpanggil()
    End Sub

    Public Sub Tekanbayar()
        ' Cek apakah supplier belum dipilih
        If String.IsNullOrEmpty(TxtSupplier.Text) AndAlso Not ModulHakAkses.SettingIzinkanBeliTanpaSupplier Then
            MessageBox.Show("Supliyer belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TxtSupplier.Select()
            Exit Sub
        End If

        ' Cek apakah belum ada transaksi pembelian
        If (TxtGrandtotal.Text = "0" AndAlso Not ModulHakAkses.SettingIzinkanNominalBeliNol) OrElse DgvData.RowCount = 0 Then
            MessageBox.Show("Belum ada transaksi Pembelian", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            If DgvData.Rows.Count > 0 AndAlso DgvData.Columns.Count > 1 Then
                DgvData.CurrentCell = DgvData(1, 0)
                DgvData.Rows(0).Selected = True
            End If

            If ModulHakAkses.SettingFokusOtomatis Then
                TxtNama.Select()
            End If


            TxtNama.Focus()
            Exit Sub
        End If


        If Not ModulHakAkses.SettingIzinkanBeliRugi Then
            If Cekjualrugi() Then
                ' Ada barang yang merugi, keluar dari fungsi atau lakukan tindakan yang sesuai
                Return
            End If
        End If


        CenterPanelBayar()
        GBBayar.Visible = True

        If IsModeTambahPembelian Then
            If ModulHakAkses.SettingLangsungIsiNominalTotal Then
                TxtNominalBayarTunai.Text = TxtTotal.Text
            Else
                TxtNominalBayarTunai.Text = ""
            End If
        End If

        ' ✅ Hitung ulang TxtKembali agar selalu tepat saat panel bayar dibuka
        HitungUlangKembali()

        TxtNominalBayarTunai.Focus()

    End Sub

    ''' <summary>
    ''' Hitung ulang TxtKembali dari Grand Total - Bayar.
    ''' Dipanggil setiap kali panel bayar dibuka (tambah maupun edit)
    ''' agar nilai tidak stale dari transaksi sebelumnya.
    ''' </summary>
    Private Sub HitungUlangKembali()
        Dim total As Decimal = ModuleAngka.ParseDecimal(TxtTotal.Text)
        If total = 0 Then
            ' Fallback: ambil dari TxtGrandtotal jika TxtTotal belum terisi
            total = ModuleAngka.ParseDecimal(TxtGrandtotal.Text)
            TxtTotal.Text = total.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        End If

        Dim bayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim sisa As Decimal = total - bayarTunai - bayarTransfer

        TxtKembaliHutang.Text = Math.Abs(sisa).ToString()
        TxtBAntuanbayar.Text = sisa.ToString()
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

                Dim qtysat As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("QtySat").Value)
                Dim HargajualUmum As Decimal = 0
                Dim HargajualPartai As Decimal = 0

                ' Mengumpulkan informasi barang
                Using cmd As New MySqlCommand("SELECT HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_PARTAI_KECIL FROM tbl_barang WHERE ID_BARANG LIKE @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            HargajualUmum = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL")) * qtysat
                            HargajualPartai = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_PARTAI_KECIL")) * qtysat
                        End If
                    End Using
                End Using

                Dim HargaBeli As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("TotalHarga").Value)
                If HargaBeli > HargajualUmum Then
                    Dim errorMessage As String = "Harga ==> " & dgvRow.Cells("Nama").Value.ToString() & " <== Terjual rugi. " & vbCrLf & vbCrLf &
                                         "Harga beli: " & HargaBeli.ToString("N0") & ", Harga jual Umum: " & HargajualUmum.ToString("N0")
                    MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    dgvRow.Selected = True
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = Color.Red
                    Next
                    Return True
                End If

                If HargajualPartai <> 0 AndAlso HargaBeli > HargajualPartai Then
                    Dim errorMessage As String = "Harga ==> " & dgvRow.Cells("Nama").Value.ToString() & " <== Terjual rugi. " & vbCrLf & vbCrLf &
                                         "Harga beli: " & HargaBeli.ToString("N0") & ", Harga jual Partai: " & HargajualPartai.ToString("N0")
                    MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    dgvRow.Selected = True
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = Color.Red
                    Next
                    Return True
                End If

                ' Kembalikan warna default
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = Color.Empty
                Next
            End If
        Next

        Return False
    End Function

    Public Sub Tekanbarang()
        Using f As New TambahBarang()
            f.LblHeaderForm.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using
    End Sub

    Public Sub Tekansupliyer()
        TambahSupliyer.ShowDialog()
        AmbilDataSupplier()
    End Sub

    Public Sub SimpanTransaksi()
        Dim bayarText = TxtNominalBayarTunai.Text.Trim()
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

        If Not ModulHakAkses.SettingIzinkanTanggalLampau AndAlso jenisTrans = "TambahPembelian" AndAlso String.IsNullOrWhiteSpace(draftPembelianAktif) Then
            DTPTgl.Value = DateTime.Now
            NomorBeli()
        ElseIf jenisTrans = "TambahPembelian" AndAlso String.IsNullOrWhiteSpace(draftPembelianAktif) Then
            NomorBeli()
        End If

        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            ' ========================================
            ' LANGKAH 1: SIMPAN AKUN LAMA (JIKA MODE EDIT)
            ' ========================================
            Dim akunTerlibatLama As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            If Not IsModeTambahPembelian Then
                Using cmdAkunLama As New MySqlCommand(
                "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                "UNION " &
                "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                conn, transaction)
                    cmdAkunLama.Parameters.AddWithValue("@fk", TxtFaktur.Text)
                    Using rd = cmdAkunLama.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibatLama.Add(kode)
                        End While
                    End Using
                End Using
            End If

            ' Jika edit: hapus dulu transaksi lama
            If Not IsModeTambahPembelian Then
                ' ========================================
                ' START: Audit Trail - Edit Pembelian
                ' ========================================
                ModuleAuditTrail.CatatAudit(TxtFaktur.Text, "EDIT", "Pembelian", ket:="[KRITIS] Edit pembelian", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Pembelian
                ' ========================================
                Hapusbelanja(transaction)
            End If

            ' Audit dictionaries
            Dim auditDGV As New Dictionary(Of String, Decimal)()
            Dim auditHistory As New Dictionary(Of String, Decimal)()
            Dim auditDetail As New Dictionary(Of String, Decimal)()
            Dim auditStokDelta As New Dictionary(Of String, Decimal)()

            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                    Dim kodeA As String = row.Cells("Id").Value.ToString()
                    Dim qtyA As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                    If auditDGV.ContainsKey(kodeA) Then auditDGV(kodeA) += qtyA Else auditDGV(kodeA) = qtyA
                End If
            Next

            ' Simpan header, detail, history
            SimpanPembelian(transaction)
            SimpanPembelianDetail(transaction, auditDetail)
            HistoryBarang(transaction, auditHistory)

            ' Jurnal — semua kasus (tunai, transfer, diskon, PPN, biaya kirim, hutang) ditangani di Simpanjurnal
            Dim jD As Decimal = 0D, jK As Decimal = 0D
            Simpanjurnal(transaction, jD, jK)

            ' Update stok & saldo
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                    Dim kodeD As String = row.Cells("Id").Value.ToString()
                    Dim stokSebelum As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    HitungStokPerubahan(kodeD, transaction)
                    Dim stokSesudah As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    Dim delta As Decimal = stokSesudah - stokSebelum
                    If auditStokDelta.ContainsKey(kodeD) Then auditStokDelta(kodeD) += delta Else auditStokDelta(kodeD) = delta
                End If
            Next


            Dim akunTerlibatBaru As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmdAkunBaru As New MySqlCommand(
            "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
            "UNION " &
            "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
            conn, transaction)
                cmdAkunBaru.Parameters.AddWithValue("@fk", TxtFaktur.Text)
                Using rd = cmdAkunBaru.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd(0).ToString().Trim()
                        If kode <> "" Then akunTerlibatBaru.Add(kode)
                    End While
                End Using
            End Using

            ' Gabungkan akun lama dan baru (jika mode edit)
            Dim semuaAkunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each kodeAkun In akunTerlibatLama
                semuaAkunTerlibat.Add(kodeAkun)
            Next
            For Each kodeAkun In akunTerlibatBaru
                semuaAkunTerlibat.Add(kodeAkun)
            Next

            ' Update saldo semua akun yang terlibat
            For Each kodeAkun As String In semuaAkunTerlibat
                UpdateSaldoAkun(kodeAkun, transaction)
            Next
            UpdateHutangSupliyer(LblKodeSupplier.Text, transaction)

            ' Hapus draft jika ada
            If Not String.IsNullOrWhiteSpace(draftPembelianAktif) Then
                HapusDraftPembelian(transaction, draftPembelianAktif)
            End If

            AuditStokTransaksi(TxtFaktur.Text, "Pembelian", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)

            transaction.Commit()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Transaksi dibatalkan karena kesalahan:" & vbCrLf & ex.Message, "Gagal Simpan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Finally
            Cursor = Cursors.Default
        End Try

        JumlahTahanPembelian()

        ' CETAK DILUAR TRY utama agar tetap lanjut meski gagal
        Try
            Select Case BacaPengaturanPrinter("Beli", "CetakOtomatis", "IYA").Trim().ToUpper()
                Case "IYA"
                    LakukanCetakPembelian(TxtFaktur.Text)
                Case "SELALU TANYA"
                    If MessageBox.Show("Apakah Anda ingin mencetak Pembelian?", "Konfirmasi Cetak",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        LakukanCetakPembelian(TxtFaktur.Text)
                    End If
                Case "TAMPILKAN DI MONITOR"
                    LakukanCetakPembelian(TxtFaktur.Text)
            End Select
        Catch ex As Exception
            MessageBox.Show("Gagal mencetak pembelian. Anda bisa mencetak ulang nanti." & vbCrLf &
                    "Detail: " & ex.Message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            If IsModeTambahPembelian Then
                Kondisiawal()
            Else
                Me.Close()
            End If
        End Try

    End Sub

    Private Sub TxtBayar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayarTunai.TextChanged
        Dim total As Decimal = ModuleAngka.ParseDecimal(TxtTotal.Text)
        Dim bayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim totalBayar As Decimal = bayarTunai + bayarTransfer

        Dim bantuanBayar As Decimal = total - totalBayar
        TxtKembaliHutang.Text = Math.Abs(bantuanBayar).ToString()
        TxtBAntuanbayar.Text = bantuanBayar.ToString()

        If IsModeTambahPembelian AndAlso totalBayar > total Then
            MessageBox.Show("Pembayaran melebihi belanja !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            If TxtNominalBayarTunai.Text.Length > 0 Then
                TxtNominalBayarTunai.Text = TxtNominalBayarTunai.Text.Substring(0, TxtNominalBayarTunai.Text.Length - 1)
                TxtNominalBayarTunai.Select(TxtNominalBayarTunai.Text.Length, 0)
            End If
        End If
    End Sub

    Private Sub TxtBayarTransfer_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayarTransfer.TextChanged
        Dim total As Decimal = ModuleAngka.ParseDecimal(TxtTotal.Text)
        Dim bayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim totalBayar As Decimal = bayarTunai + bayarTransfer

        Dim bantuanBayar As Decimal = total - totalBayar
        TxtKembaliHutang.Text = Math.Abs(bantuanBayar).ToString()
        TxtBAntuanbayar.Text = bantuanBayar.ToString()
    End Sub



    Public Sub Tekantahan()
        If Not IsModeTambahPembelian Then
            MessageBox.Show("Mode edit tidak dapat menggunakan fitur Tahan untuk menghindari bentrok data.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If String.IsNullOrWhiteSpace(TxtFaktur.Text) Then
            MessageBox.Show("Nomor faktur wajib diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtFaktur.Focus()
            Return
        End If

        Dim adaItemValid As Boolean = False
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString().Trim() <> "" Then
                adaItemValid = True
                Exit For
            End If
        Next

        If Not adaItemValid Then
            MessageBox.Show("Belum ada barang yang diinput.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim transaction As MySqlTransaction = Nothing
        Try
            transaction = conn.BeginTransaction()

            If IsModeTambahPembelian AndAlso String.IsNullOrWhiteSpace(draftPembelianAktif) Then
                NomorBeli()
            End If

            HapusDraftPembelian(transaction, TxtFaktur.Text)
            SimpanPembelianDitahanHeader(transaction)
            SimpanPembelianDitahanDetail(transaction)

            transaction.Commit()
            MessageBox.Show("Pembelian berhasil ditahan sementara.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            If IsModeTambahPembelian Then
                Kondisiawal()
            Else
                Me.Close()
            End If
        Catch ex As Exception
            transaction?.Rollback()
            MessageBox.Show("Gagal menyimpan pembelian ditahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        JumlahTahanPembelian()
    End Sub

    Public Sub Tekanpanggil()
        If Not IsModeTambahPembelian Then
            MessageBox.Show("Mode edit tidak dapat memanggil transaksi ditahan untuk menghindari bentrok data.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using frm As New FormPembelianDitahan()
            If frm.ShowDialog() = DialogResult.OK AndAlso Not String.IsNullOrWhiteSpace(frm.SelectedFaktur) Then
                MuatDraftPembelian(frm.SelectedFaktur)
            End If
        End Using
        JumlahTahanPembelian()
    End Sub

    Public Sub JumlahTahanPembelian()
        Dim jumlah As Integer = 0
        Using cmd As New MySqlCommand("SELECT COUNT(ID_PEMBELIAN) FROM pembelian_ditahan", conn)
            Dim val = cmd.ExecuteScalar()
            If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                Integer.TryParse(val.ToString(), jumlah)
            End If
        End Using
        BtnPanggil.Text = " Panggil (F7) [" & jumlah.ToString() & "]"
    End Sub

    Private Sub SimpanPembelianDitahanHeader(ByVal transaction As MySqlTransaction)
        Dim sql As String =
        "INSERT INTO pembelian_ditahan (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, ID_USER, ID_KOMPUTER) " &
        "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @ID_USER, @ID_KOMPUTER)"
        ' TODO: Tambahkan kolom NOMINAL_TRANSFER, KODE_AKUN_TF, NAMA_AKUN_TF, DISKON_SUPPLIER, PPN_MASUKAN, BIAYA_KIRIM
        '       ke tabel pembelian_ditahan dan ke INSERT ini setelah migrasi skema dilakukan.

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", If(String.IsNullOrWhiteSpace(LblKodeSupplier.Text), DBNull.Value, CType(LblKodeSupplier.Text, Object)))
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", If(String.IsNullOrWhiteSpace(TxtSupplier.Text), DBNull.Value, CType(TxtSupplier.Text, Object)))
            cmd.Parameters.AddWithValue("@NOTA_PEMBELIAN", TxtNota.Text)
            cmd.Parameters.AddWithValue("@TGL_BELI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@JENIS_BAYAR", CmbJenisBayarTunai.Text)
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_BELI", ModuleAngka.ParseDecimal(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", ModuleAngka.ParseDecimal(TxtJmlhBrg.Text))
            cmd.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub SimpanPembelianDitahanDetail(ByVal transaction As MySqlTransaction)
        Dim sql As String =
        "INSERT INTO pembelian_ditahan_detail (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER, ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL) " &
        "VALUES (@FAKTUR_BELI, @NOTA_BELI, @TANGGAL_MASUK, @LOKASI, @ID_SUPLIYER, @NAMA_SUPLIYER, @ID_BARANG, @NAMA_BARANG, @HARGA_BELI, @HARGA_AVERAGE, @HARGA_BELI_SEBELUMNYA, @QTY, @SATUAN, @ISI_SATUAN, @HARGA_BELI_SATUAN, @QTY_SAT, @TOTAL)"

        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow OrElse row.Cells("Id").Value Is Nothing OrElse row.Cells("Id").Value.ToString().Trim() = "" Then Continue For

            Using cmd As New MySqlCommand(sql, conn, transaction)
                cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@NOTA_BELI", TxtNota.Text)
                cmd.Parameters.AddWithValue("@TANGGAL_MASUK", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                cmd.Parameters.AddWithValue("@ID_SUPLIYER", LblKodeSupplier.Text)
                cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
                cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells("Id").Value.ToString())
                cmd.Parameters.AddWithValue("@NAMA_BARANG", If(row.Cells("Nama").Value, ""))
                cmd.Parameters.AddWithValue("@HARGA_BELI", ModuleAngka.ParseDecimal(row.Cells("Hargabeli").Value))
                cmd.Parameters.AddWithValue("@HARGA_AVERAGE", ModuleAngka.ParseDecimal(row.Cells("Average").Value))
                cmd.Parameters.AddWithValue("@HARGA_BELI_SEBELUMNYA", ModuleAngka.ParseDecimal(row.Cells("HargaSebelumnya").Value))
                cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells("Qty").Value))
                cmd.Parameters.AddWithValue("@SATUAN", If(row.Cells("Satuan").Value, ""))
                cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("Isi").Value))
                cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("HargaBeliSat").Value))
                cmd.Parameters.AddWithValue("@QTY_SAT", ModuleAngka.ParseDecimal(row.Cells("QtySat").Value))
                cmd.Parameters.AddWithValue("@TOTAL", ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value))
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Private Sub HapusDraftPembelian(ByVal transaction As MySqlTransaction, ByVal faktur As String)
        If String.IsNullOrWhiteSpace(faktur) Then Return

        Using cmd As New MySqlCommand("DELETE FROM pembelian_ditahan_detail WHERE FAKTUR_BELI = @FAKTUR", conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", faktur)
            cmd.ExecuteNonQuery()
        End Using

        Using cmd As New MySqlCommand("DELETE FROM pembelian_ditahan WHERE ID_PEMBELIAN = @FAKTUR", conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", faktur)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub MuatDraftPembelian(ByVal faktur As String)
        DgvData.Rows.Clear()
        TxtFaktur.Text = faktur
        draftPembelianAktif = faktur

        Dim queryHeader As String =
        "SELECT ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, ID_USER, ID_KOMPUTER " &
        "FROM pembelian_ditahan WHERE ID_PEMBELIAN = @ID"

        Using cmd As New MySqlCommand(queryHeader, conn)
            cmd.Parameters.AddWithValue("@ID", faktur)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    LblKodeSupplier.Text = rd("ID_SUPPLIER").ToString()
                    TxtSupplier.Text = rd("NAMA_SUPLIYER").ToString()
                    TxtNota.Text = rd("NOTA_PEMBELIAN").ToString()
                    DTPTgl.Value = ModuleAngka.SafeGetValue(Of DateTime)(rd, "TGL_BELI", DateTime.Now)
                    LblLokasiBarang.Text = rd("LOKASI").ToString()
                    CmbJenisBayarTunai.Text = rd("JENIS_BAYAR").ToString()
                    ' Saat panggil draft, pembayaran selalu reset/default.
                    TxtNominalBayarTunai.Text = "0"
                    TxtNominalBayarTransfer.Text = "0"
                    TxtKembaliHutang.Text = "0"
                    TxtBAntuanbayar.Text = "0"
                    LblStatusTransLunas.Text = "Lunas"
                    TxtLogin.Text = rd("ID_USER").ToString()
                    TxtKomputer.Text = rd("ID_KOMPUTER").ToString()
                    DTPJatuhTempo.Value = DTPTgl.Value.AddMonths(1)
                    ' TODO: Restore TxtNominalBayarTransfer, CmbJenisBayarTransfer, TxtDiskonRp, TxtPajakRp, TxtBiayaKirim
                    '       setelah kolom NOMINAL_TRANSFER, KODE_AKUN_TF, NAMA_AKUN_TF, DISKON_SUPPLIER, PPN_MASUKAN, BIAYA_KIRIM
                    '       ditambahkan ke tabel pembelian_ditahan.
                Else
                    MessageBox.Show("Data draft tidak ditemukan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If
            End Using
        End Using

        Dim queryDetail As String =
        "SELECT pdd.ID_BARANG, pdd.NAMA_BARANG, pdd.HARGA_BELI, pdd.QTY, pdd.SATUAN, pdd.ISI_SATUAN, pdd.HARGA_BELI_SATUAN, pdd.QTY_SAT, pdd.TOTAL, pdd.HARGA_AVERAGE, pdd.HARGA_BELI_SEBELUMNYA, " &
        "tb.SATUAN_UMUM_KECIL, tb.SATUAN_UMUM_SEDANG, tb.SATUAN_UMUM_BESAR " &
        "FROM pembelian_ditahan_detail pdd " &
        "LEFT JOIN tbl_barang tb ON pdd.ID_BARANG = tb.ID_BARANG " &
        "WHERE pdd.FAKTUR_BELI = @FAKTUR"
        Using cmd As New MySqlCommand(queryDetail, conn)
            cmd.Parameters.AddWithValue("@FAKTUR", faktur)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim baris As DataGridViewRow = DirectCast(DgvData.Rows(DgvData.Rows.Add()), DataGridViewRow)
                    baris.Cells("Id").Value = rd("ID_BARANG").ToString()
                    baris.Cells("Nama").Value = rd("NAMA_BARANG").ToString()
                    baris.Cells("Hargabeli").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    baris.Cells("Qty").Value = ModuleAngka.ParseDecimal(rd("QTY"))
                    baris.Cells("Isi").Value = ModuleAngka.ParseDecimal(rd("ISI_SATUAN"))
                    baris.Cells("HargaBeliSat").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SATUAN"))
                    baris.Cells("QtySat").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                    baris.Cells("Totalharga").Value = ModuleAngka.ParseDecimal(rd("TOTAL"))
                    baris.Cells("Average").Value = ModuleAngka.ParseDecimal(rd("HARGA_AVERAGE"))
                    baris.Cells("HargaSebelumnya").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SEBELUMNYA"))
                    baris.Cells("QtySebelumnya").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                    Dim comboCell As DataGridViewComboBoxCell = CType(baris.Cells("Satuan"), DataGridViewComboBoxCell)
                    comboCell.Items.Clear()
                    For Each satNama As String In {ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                                               ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                                               ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")}
                        If satNama <> "" AndAlso Not comboCell.Items.Contains(satNama) Then comboCell.Items.Add(satNama)
                    Next
                    Dim satuanDraft As String = rd("SATUAN").ToString()
                    If satuanDraft <> "" AndAlso Not comboCell.Items.Contains(satuanDraft) Then
                        comboCell.Items.Add(satuanDraft)
                    End If
                    baris.Cells("Satuan").Value = satuanDraft
                End While
            End Using
        End Using

        LblJatuhTempo.Visible = False
        DTPJatuhTempo.Visible = False
        LblPembayaran.Text = "Kembalian :"

        TxtJenisTrans.Text = "TambahPembelian"
        UpdateSemuaTotal()
        AmbilKodeAkun()
    End Sub


    Private Sub LakukanCetakPembelian(idPembelian As String)
        If BacaPengaturanPrinter("Beli", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterBeli.TanyaPilihPrinterBeli(idPembelian)
        Else
            ModulePrinterBeli.CetakPembelian(idPembelian)
        End If
    End Sub

    Public Sub Tekanbatal()
        GBBayar.Visible = False
        TxtNominalBayarTunai.Text = 0
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


        ' Audit hapus lama: A dari DGVDetail, D dari delta stok
        Dim auditDGVHapusBeli As New Dictionary(Of String, Decimal)()
        Dim auditDeltaHapusBeli As New Dictionary(Of String, Decimal)()

        For Each row As DataGridViewRow In FormUtama.DGVDetail.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
                Dim stokPengurangan As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY_SAT").Value)
                Dim Hargabeli As Decimal = ModuleAngka.ParseDecimal(row.Cells("HARGA_AVERAGE").Value)
                Dim HARGA_BELI_SEBELUMNYA As Decimal = ModuleAngka.ParseDecimal(row.Cells("HARGA_BELI_SEBELUMNYA").Value)

                If auditDGVHapusBeli.ContainsKey(kodeBarang) Then auditDGVHapusBeli(kodeBarang) += stokPengurangan Else auditDGVHapusBeli(kodeBarang) = stokPengurangan

                Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                    cmd.Parameters.AddWithValue("@Hargabeli", Hargabeli)
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SEBELUMNYA", HARGA_BELI_SEBELUMNYA)
                    cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                    cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                    cmd.ExecuteNonQuery()
                End Using

                Dim sebelumEditBeli As Decimal = BacaStokSaatIni(kodeBarang, LblLokasiBarang.Text, transaction)
                HitungStokPerubahan(kodeBarang, transaction)
                Dim sesudahEditBeli As Decimal = BacaStokSaatIni(kodeBarang, LblLokasiBarang.Text, transaction)
                Dim deltaBeli As Decimal = sebelumEditBeli - sesudahEditBeli  ' hapus pembelian lama mengurangi stok
                If auditDeltaHapusBeli.ContainsKey(kodeBarang) Then auditDeltaHapusBeli(kodeBarang) += deltaBeli Else auditDeltaHapusBeli(kodeBarang) = deltaBeli
            End If
        Next

        AuditStokTransaksi(TxtFaktur.Text & " [HAPUS-EDIT]", "Edit Pembelian (hapus lama)", auditDGVHapusBeli, Nothing, Nothing, auditDeltaHapusBeli, transaction)

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
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim diskonSupplier As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRp.Text)
        Dim ppnMasukan As Decimal = ModuleAngka.ParseDecimal(TxtPajakRp.Text)
        Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)

        Dim sql As String
        If LblStatusTransLunas.Text = "Lunas" Then
            sql = "INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, STATUS_JUAL, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER, NOMINAL_TRANSFER, KODE_AKUN_TF, NAMA_AKUN_TF, DISKON_SUPPLIER, PPN_MASUKAN, BIAYA_KIRIM) " &
              "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @PEMBAYARAN, @STATUS_JUAL, @STATUS_TRANSAKSI_BELI, @ID_USER, @ID_KOMPUTER, @NOMINAL_TRANSFER, @KODE_AKUN_TF, @NAMA_AKUN_TF, @DISKON_SUPPLIER, @PPN_MASUKAN, @BIAYA_KIRIM)"
        Else
            sql = "INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, STATUS_JUAL, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER, NOMINAL_TRANSFER, KODE_AKUN_TF, NAMA_AKUN_TF, DISKON_SUPPLIER, PPN_MASUKAN, BIAYA_KIRIM) " &
              "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @PEMBAYARAN, @TAGIHAN, @JATUH_TEMPO, @STATUS_JUAL, @STATUS_TRANSAKSI_BELI, @ID_USER, @ID_KOMPUTER, @NOMINAL_TRANSFER, @KODE_AKUN_TF, @NAMA_AKUN_TF, @DISKON_SUPPLIER, @PPN_MASUKAN, @BIAYA_KIRIM)"
        End If

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@NOTA_PEMBELIAN", TxtNota.Text)
            cmd.Parameters.AddWithValue("@TGL_BELI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@JENIS_BAYAR", CmbJenisBayarTunai.Text)
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_BELI", ModuleAngka.ParseDecimal(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", ModuleAngka.ParseDecimal(TxtJmlhBrg.Text))
            cmd.Parameters.AddWithValue("@PEMBAYARAN", ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text))
            cmd.Parameters.AddWithValue("@NOMINAL_TRANSFER", bayarTransfer)
            cmd.Parameters.AddWithValue("@KODE_AKUN_TF", If(bayarTransfer > 0, TxtJenisBayarTransfer.Text, ""))
            cmd.Parameters.AddWithValue("@NAMA_AKUN_TF", If(bayarTransfer > 0, CmbJenisBayarTransfer.Text, ""))
            cmd.Parameters.AddWithValue("@DISKON_SUPPLIER", diskonSupplier)
            cmd.Parameters.AddWithValue("@PPN_MASUKAN", ppnMasukan)
            cmd.Parameters.AddWithValue("@BIAYA_KIRIM", biayaKirim)

            If LblStatusTransLunas.Text = "Lunas" Then
                cmd.Parameters.AddWithValue("@STATUS_JUAL", "TERBAYAR")
            Else
                cmd.Parameters.AddWithValue("@TAGIHAN", ModuleAngka.ParseDecimal(TxtKembaliHutang.Text))
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", DTPJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@STATUS_JUAL", "TERHUTANG")
            End If

            cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI_BELI", LblStatusTransLunas.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub SimpanPembelianDetail(ByVal transaction As MySqlTransaction, ByRef auditDetail As Dictionary(Of String, Decimal))
        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow OrElse row.Cells("Id").Value Is Nothing OrElse row.Cells("Id").Value.ToString().Trim() = "" Then Continue For

            ' Ambil data dari row
            Dim idBarang = row.Cells("Id").Value.ToString()
            Dim namaBarang = row.Cells("Nama").Value?.ToString()
            Dim hargaBeli = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
            Dim hargaAverage = ModuleAngka.ParseDecimal(row.Cells("Average").Value)
            Dim hargaSebelumnya = ModuleAngka.ParseDecimal(row.Cells("HargaSebelumnya").Value)
            Dim qty = ModuleAngka.ParseDecimal(row.Cells("Qty").Value)
            Dim satuan = row.Cells("Satuan").Value?.ToString()
            Dim isi = SafeInt(row.Cells("Isi").Value, 1)
            Dim hargaBeliSat = ModuleAngka.ParseDecimal(row.Cells("HargaBeliSat").Value)
            Dim qtySat = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
            Dim total = ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value)

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
                    .AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                    .AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
                End With
                cmd.ExecuteNonQuery()
            End Using

            ' Tentukan nama field stok
            Dim stokField As String = If(LblLokasiBarang.Text = "TOKO", "PEMBELIAN_TOKO", "PEMBELIAN_GUDANG")
            Dim hargaSatuan = If(isi = 0, 0, hargaBeli / isi)

            Select Case ModulHakAkses.SettingMetodeUpdateHargaBeli
                Case "Harga Terbaru"
                    UpdateHargaTerbaru(idBarang, hargaSatuan, qtySat, stokField, transaction)
                Case "Metode Average (Rata - Rata)"
                    UpdateHargaAverage(idBarang, hargaSatuan, hargaAverage, qtySat, stokField, transaction)
                Case "Tidak Ada"
                    UpdateStokSaja(idBarang, qtySat, stokField, transaction)
            End Select

            ' Audit C
            If auditDetail.ContainsKey(idBarang) Then auditDetail(idBarang) += qtySat Else auditDetail(idBarang) = qtySat
        Next
    End Sub


    Private Function SafeInt(value As Object, Optional defaultValue As Integer = 0) As Integer
        If IsDBNull(value) OrElse value Is Nothing OrElse Not IsNumeric(value) Then Return defaultValue
        Return Convert.ToInt32(value)
    End Function

    ''' <summary>
    ''' Update harga barang dengan metode Harga Terbaru
    ''' Selain update harga, juga menghitung selisih nilai persediaan dan akumulasi ke _totalSelisihHargaPokok (Requirement 21)
    ''' </summary>
    Private Sub UpdateHargaTerbaru(idBarang As String, harga As Decimal, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim hargaLama As Decimal = 0D
        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D

        ' STEP 1: Baca HARGA_BELI lama dan stok saat ini SEBELUM update
        Using cmd As New MySqlCommand("SELECT HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn, tr)
            cmd.Parameters.AddWithValue("@ID", idBarang)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    hargaLama = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    stokToko = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    stokGudang = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                End If
            End Using
        End Using

        ' STEP 2: Hitung total stok saat ini (berdasarkan setting)
        Dim totalStokLama = If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Toko", stokToko, If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Gudang", stokGudang, stokToko + stokGudang))

        ' STEP 3: Hitung selisih nilai persediaan jika harga berubah dan stok > 0
        If harga <> hargaLama AndAlso totalStokLama > 0 Then
            Dim selisih = (harga - hargaLama) * totalStokLama
            _totalSelisihHargaPokok += selisih
        End If

        ' STEP 4: Update harga barang di tbl_barang
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

    ''' <summary>
    ''' Update harga barang dengan metode Average (Rata-Rata)
    ''' Selain update harga, juga menghitung selisih nilai persediaan dan akumulasi ke _totalSelisihHargaPokok (Requirement 21)
    ''' </summary>
    Private Sub UpdateHargaAverage(idBarang As String, hargaBaru As Decimal, hargaLama As Decimal, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D

        ' STEP 1: Baca stok saat ini SEBELUM update
        Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn, tr)
            cmd.Parameters.AddWithValue("@ID", idBarang)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    stokToko = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    stokGudang = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                End If
            End Using
        End Using

        Dim totalStokLama = If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Toko", stokToko, If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Gudang", stokGudang, stokToko + stokGudang))
        Dim totalHargaLama = hargaLama * totalStokLama
        Dim totalHargaBaru = hargaBaru * qtySat
        Dim hargaAverageBaru = If(totalStokLama + qtySat = 0, hargaBaru, Math.Round((totalHargaLama + totalHargaBaru) / (totalStokLama + qtySat), 0))

        ' STEP 2: Hitung selisih nilai persediaan jika harga berubah dan stok > 0
        If hargaAverageBaru <> hargaLama AndAlso totalStokLama > 0 Then
            Dim selisih = (hargaAverageBaru - hargaLama) * totalStokLama
            _totalSelisihHargaPokok += selisih
        End If

        ' STEP 3: Update harga barang di tbl_barang
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


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction, ByRef auditHistory As Dictionary(Of String, Decimal))
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
                    cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells("Qty").Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells("Satuan").Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("Isi").Value))
                    Dim qtySat As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", qtySat)
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value))
                    cmd.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
                    cmd.ExecuteNonQuery()
                End Using

                ' Audit B
                Dim kodeB As String = row.Cells("Id").Value.ToString()
                Dim qtyB As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                If auditHistory.ContainsKey(kodeB) Then auditHistory(kodeB) += qtyB Else auditHistory(kodeB) = qtyB
            End If
        Next
    End Sub



    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction, ByRef outDebet As Decimal, ByRef outKredit As Decimal)
        Dim nominalBayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim nominalBayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim grandTotal As Decimal = ModuleAngka.ParseDecimal(TxtGrandtotal.Text)
        Dim diskonSupplier As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRp.Text)
        Dim ppnMasukan As Decimal = ModuleAngka.ParseDecimal(TxtPajakRp.Text)
        Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
        Dim statusLunas As Boolean = (LblStatusTransLunas.Text = "Lunas")

        ' Hitung subtotal item (sebelum diskon/PPN/biaya) = dasar jurnal D PERSEDIAAN
        Dim subtotalItem As Decimal = 0D
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing Then
                subtotalItem += ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value)
            End If
        Next

        Debug.WriteLine("═══════════════════════════════════════════════════════")
        Debug.WriteLine("DEBUG JURNAL PEMBELIAN - Faktur: " & TxtFaktur.Text)
        Debug.WriteLine("Supplier: " & TxtSupplier.Text)
        Debug.WriteLine("Subtotal Item: " & subtotalItem.ToString("N0"))
        Debug.WriteLine("Bayar Tunai: " & nominalBayarTunai.ToString("N0"))
        Debug.WriteLine("Bayar Transfer: " & nominalBayarTransfer.ToString("N0"))
        Debug.WriteLine("Diskon Supplier: " & diskonSupplier.ToString("N0"))
        Debug.WriteLine("PPN Masukan: " & ppnMasukan.ToString("N0"))
        Debug.WriteLine("Biaya Kirim: " & biayaKirim.ToString("N0"))
        Debug.WriteLine("Grand Total: " & grandTotal.ToString("N0"))
        Debug.WriteLine(New String("─"c, 115))

        Dim totalDebet As Decimal = 0D
        Dim totalKredit As Decimal = 0D

        ' ── J1: D PERSEDIAAN / K KAS TUNAI ──────────────────────────────
        If nominalBayarTunai > 0 Then
            InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
            If(statusLunas AndAlso nominalBayarTransfer = 0,
               "Pembelian lunas tunai ke " & TxtSupplier.Text,
               "Pembayaran tunai belanja ke " & TxtSupplier.Text),
            NAMA_REK_BARANG, KODE_REK_BARANG,
            CmbJenisBayarTunai.Text, TxtJenisBayarTunai.Text,
            nominalBayarTunai)
            totalDebet += nominalBayarTunai
            totalKredit += nominalBayarTunai
            Debug.WriteLine($"J1 D PERSEDIAAN / K KAS TUNAI = {nominalBayarTunai:N0}")
        End If

        ' ── J2: D PERSEDIAAN / K BANK TRANSFER ──────────────────────────
        If nominalBayarTransfer > 0 Then
            InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
            "Pembayaran transfer belanja ke " & TxtSupplier.Text & " via " & CmbJenisBayarTransfer.Text,
            NAMA_REK_BARANG, KODE_REK_BARANG,
            CmbJenisBayarTransfer.Text, TxtJenisBayarTransfer.Text,
            nominalBayarTransfer)
            totalDebet += nominalBayarTransfer
            totalKredit += nominalBayarTransfer
            Debug.WriteLine($"J2 D PERSEDIAAN / K BANK TRANSFER = {nominalBayarTransfer:N0}")
        End If

        ' ── J3: D KAS/HUTANG / K POTONGAN DISKON PEMBELIAN ─────────────
        ' Diskon supplier: mengurangi kewajiban bayar, dicatat sebagai pendapatan diskon
        If diskonSupplier > 0 Then
            Dim akunDDiskon As String = If(nominalBayarTunai > 0, CmbJenisBayarTunai.Text, If(nominalBayarTransfer > 0, CmbJenisBayarTransfer.Text, nama_rek_Hutang_Beli))
            Dim kodeDDiskon As String = If(nominalBayarTunai > 0, TxtJenisBayarTunai.Text, If(nominalBayarTransfer > 0, TxtJenisBayarTransfer.Text, Kode_rek_Hutang_Beli))
            InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
            "Diskon supplier dari " & TxtSupplier.Text,
            akunDDiskon, kodeDDiskon,
            "POTONGAN DISKON PEMBELIAN", "06.05.001",
            diskonSupplier)
            totalDebet += diskonSupplier
            totalKredit += diskonSupplier
            Debug.WriteLine($"J3 D KAS/HUTANG / K POTONGAN DISKON = {diskonSupplier:N0}")
        End If

        ' ── J4: D PPN MASUKAN / K KAS (atau hutang) ─────────────────────
        If ppnMasukan > 0 Then
            ' PPN Masukan: aset pajak yang bisa dikreditkan
            Dim akunKPpn As String = If(nominalBayarTunai > 0, CmbJenisBayarTunai.Text, If(nominalBayarTransfer > 0, CmbJenisBayarTransfer.Text, nama_rek_Hutang_Beli))
            Dim kodeKPpn As String = If(nominalBayarTunai > 0, TxtJenisBayarTunai.Text, If(nominalBayarTransfer > 0, TxtJenisBayarTransfer.Text, Kode_rek_Hutang_Beli))
            InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
            "PPN Masukan pembelian dari " & TxtSupplier.Text,
            "PPN MASUKAN", "01.05.001",
            akunKPpn, kodeKPpn,
            ppnMasukan)
            totalDebet += ppnMasukan
            totalKredit += ppnMasukan
            Debug.WriteLine($"J4 D PPN MASUKAN / K KAS/HUTANG = {ppnMasukan:N0}")
        End If

        ' ── J5: D BIAYA KIRIM / K KAS ───────────────────────────────────
        If biayaKirim > 0 Then
            Dim akunKBiaya As String = If(nominalBayarTunai > 0, CmbJenisBayarTunai.Text, If(nominalBayarTransfer > 0, CmbJenisBayarTransfer.Text, nama_rek_Hutang_Beli))
            Dim kodeKBiaya As String = If(nominalBayarTunai > 0, TxtJenisBayarTunai.Text, If(nominalBayarTransfer > 0, TxtJenisBayarTransfer.Text, Kode_rek_Hutang_Beli))
            InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
            "Biaya kirim pembelian dari " & TxtSupplier.Text,
            "BIAYA KIRIM PEMBELIAN", "06.02.001",
            akunKBiaya, kodeKBiaya,
            biayaKirim)
            totalDebet += biayaKirim
            totalKredit += biayaKirim
            Debug.WriteLine($"J5 D BIAYA KIRIM / K KAS = {biayaKirim:N0}")
        End If

        ' ── J6: D PERSEDIAAN / K HUTANG DAGANG (jika belum lunas) ───────
        Dim sisaHutang As Decimal = ModuleAngka.ParseDecimal(TxtKembaliHutang.Text)
        If Not statusLunas AndAlso sisaHutang > 0 Then
            InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
            "Hutang belanja ke " & TxtSupplier.Text & " jatuh tempo " & DTPJatuhTempo.Value.ToString("dd MMMM yyyy"),
            NAMA_REK_BARANG, KODE_REK_BARANG,
            nama_rek_Hutang_Beli, Kode_rek_Hutang_Beli,
            sisaHutang)
            totalDebet += sisaHutang
            totalKredit += sisaHutang
            Debug.WriteLine($"J6 D PERSEDIAAN / K HUTANG = {sisaHutang:N0}")
        End If

        ' ── J7: Jurnal Penyesuaian Harga Pokok (Requirement 21) ─────────────────
        If _totalSelisihHargaPokok <> 0 Then
            Dim nominalAbs As Decimal = Math.Abs(_totalSelisihHargaPokok)
            Dim uraian As String = "Penyesuaian nilai persediaan akibat perubahan harga pokok barang"

            If _totalSelisihHargaPokok > 0 Then
                ' Harga naik: D PERSEDIAAN BARANG, K PENYESUAIAN HARGA POKOK
                InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
                uraian,
                NAMA_REK_BARANG, KODE_REK_BARANG,
                "PENYESUAIAN HARGA POKOK", "06.04.002",
                nominalAbs)
                totalDebet += nominalAbs
                totalKredit += nominalAbs
                Debug.WriteLine($"J7 D PERSEDIAAN / K PENYESUAIAN HARGA POKOK = {nominalAbs:N0}")
            Else
                ' Harga turun: D PENYESUAIAN HARGA POKOK, K PERSEDIAAN BARANG
                InsertJurnal(transaction, TxtFaktur.Text, TxtNota.Text,
                uraian,
                "PENYESUAIAN HARGA POKOK", "06.04.002",
                NAMA_REK_BARANG, KODE_REK_BARANG,
                nominalAbs)
                totalDebet += nominalAbs
                totalKredit += nominalAbs
                Debug.WriteLine($"J7 D PENYESUAIAN HARGA POKOK / K PERSEDIAAN = {nominalAbs:N0}")
            End If
        End If

        Debug.WriteLine(New String("─"c, 115))
        Debug.WriteLine($"TOTAL D={totalDebet:N0} K={totalKredit:N0} " & If(totalDebet = totalKredit, "✅ SEIMBANG", "❌ TIDAK SEIMBANG"))
        Debug.WriteLine("═══════════════════════════════════════════════════════")

        outDebet = totalDebet
        outKredit = totalKredit
    End Sub

    ''' <summary>Helper INSERT satu baris ke JurnalUmum</summary>
    Private Sub InsertJurnal(trans As MySqlTransaction, noTrans As String, noNota As String,
                          uraian As String, namaD As String, kodeD As String,
                          namaK As String, kodeK As String, nominal As Decimal)
        Using cmd As New MySqlCommand(
        "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
        "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, " &
        "JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
        "VALUES (@NO, @TGL, @NOTA, @URAIAN, @ND, @KD, @NK, @KK, @NOM, @JENIS, @LOK, @USR, @PC)",
        conn, trans)
            cmd.Parameters.AddWithValue("@NO", noTrans)
            cmd.Parameters.AddWithValue("@TGL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NOTA", noNota)
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@ND", namaD)
            cmd.Parameters.AddWithValue("@KD", kodeD)
            cmd.Parameters.AddWithValue("@NK", namaK)
            cmd.Parameters.AddWithValue("@KK", kodeK)
            cmd.Parameters.AddWithValue("@NOM", nominal)
            cmd.Parameters.AddWithValue("@JENIS", "Pembelian")
            cmd.Parameters.AddWithValue("@LOK", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@USR", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@PC", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub AmbilDaftarBarangEditpembelian()
        ' Kosongkan baris yang ada di DataGridView
        DgvData.Rows.Clear()

        Dim queryPembelian As String =
        "SELECT pd.ID_BARANG, pd.NAMA_BARANG, pd.HARGA_BELI, pd.QTY, pd.SATUAN, pd.ISI_SATUAN, pd.HARGA_BELI_SATUAN, pd.QTY_SAT, pd.TOTAL, pd.HARGA_AVERAGE, pd.HARGA_BELI_SEBELUMNYA, " &
        "tb.SATUAN_UMUM_KECIL, tb.SATUAN_UMUM_SEDANG, tb.SATUAN_UMUM_BESAR " &
        "FROM pembelian_detail pd " &
        "LEFT JOIN tbl_barang tb ON pd.ID_BARANG = tb.ID_BARANG " &
        "WHERE pd.FAKTUR_BELI = ?"
        Using cmd As New MySqlCommand(queryPembelian, conn)
            cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Proses setiap record dari data reader
                While rd.Read()
                    ' Tambahkan baris baru ke DataGridView
                    Dim baris As DataGridViewRow = DirectCast(DgvData.Rows(DgvData.Rows.Add()), DataGridViewRow)

                    ' Isi nilai ke sel baris berdasarkan nama kolom
                    baris.Cells("Id").Value = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", "")
                    baris.Cells("Nama").Value = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_BARANG", String.Empty)
                    baris.Cells("Hargabeli").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    baris.Cells("Qty").Value = ModuleAngka.ParseDecimal(rd("QTY"))
                    baris.Cells("Isi").Value = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_SATUAN", 0)
                    baris.Cells("HargaBeliSat").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SATUAN"))
                    baris.Cells("QtySat").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                    baris.Cells("Totalharga").Value = ModuleAngka.ParseDecimal(rd("TOTAL"))
                    baris.Cells("Average").Value = ModuleAngka.ParseDecimal(rd("HARGA_AVERAGE"))
                    baris.Cells("HargaSebelumnya").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SEBELUMNYA"))
                    baris.Cells("QtySebelumnya").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))

                    Dim comboCell As DataGridViewComboBoxCell = CType(baris.Cells("Satuan"), DataGridViewComboBoxCell)
                    comboCell.Items.Clear()
                    For Each satNama As String In {ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                                               ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                                               ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")}
                        If satNama <> "" AndAlso Not comboCell.Items.Contains(satNama) Then comboCell.Items.Add(satNama)
                    Next

                    Dim satuanBeli As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN", "")
                    If satuanBeli <> "" AndAlso Not comboCell.Items.Contains(satuanBeli) Then
                        comboCell.Items.Add(satuanBeli)
                    End If
                    If satuanBeli <> "" Then
                        baris.Cells("Satuan").Value = satuanBeli
                    End If
                End While
            End Using
        End Using

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
        Dim queryString As String = "SELECT ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, " &
                                "JENIS_BAYAR, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, " &
                                "STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER, " &
                                "IFNULL(NOMINAL_TRANSFER,0) AS NOMINAL_TRANSFER, " &
                                "IFNULL(KODE_AKUN_TF,'') AS KODE_AKUN_TF, " &
                                "IFNULL(NAMA_AKUN_TF,'') AS NAMA_AKUN_TF, " &
                                "IFNULL(DISKON_SUPPLIER,0) AS DISKON_SUPPLIER, " &
                                "IFNULL(PPN_MASUKAN,0) AS PPN_MASUKAN, " &
                                "IFNULL(BIAYA_KIRIM,0) AS BIAYA_KIRIM " &
                                "FROM pembelian WHERE ID_PEMBELIAN = ?"

        Dim IDSupplier As String
        Dim NamaSupplier As String = String.Empty
        Dim NotaPembelian As String = String.Empty
        Dim TanggalBeli As Date = Date.MinValue
        Dim Lokasi As String = String.Empty
        Dim JenisBayar As String = String.Empty
        Dim Pembayaran As Decimal = 0D
        Dim Tagihan As Decimal = 0D
        Dim JatuhTempo As Date = Date.MinValue
        Dim StatusTransaksi As String = "Lunas"
        Dim IDUser As String = String.Empty
        Dim IDKomputer As String = String.Empty
        Dim NominalTransfer As Decimal = 0D
        Dim KodeAkunTF As String = ""
        Dim NamaAkunTF As String = ""
        Dim DiskonSupplier As Decimal = 0D
        Dim PpnMasukan As Decimal = 0D
        Dim BiayaKirim As Decimal = 0D

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
                    Pembayaran = ModuleAngka.ParseDecimal(rd("PEMBAYARAN"))
                    Tagihan = ModuleAngka.ParseDecimal(rd("TAGIHAN"))
                    JatuhTempo = ModuleAngka.SafeGetValue(Of DateTime)(rd, "JATUH_TEMPO", DTPTgl.Value.AddMonths(1))
                    StatusTransaksi = ModuleAngka.SafeGetValue(Of String)(rd, "STATUS_TRANSAKSI_BELI", "Lunas")
                    IDUser = rd("ID_USER").ToString()
                    IDKomputer = rd("ID_KOMPUTER").ToString()
                    NominalTransfer = ModuleAngka.ParseDecimal(rd("NOMINAL_TRANSFER"))
                    KodeAkunTF = rd("KODE_AKUN_TF").ToString()
                    NamaAkunTF = rd("NAMA_AKUN_TF").ToString()
                    DiskonSupplier = ModuleAngka.ParseDecimal(rd("DISKON_SUPPLIER"))
                    PpnMasukan = ModuleAngka.ParseDecimal(rd("PPN_MASUKAN"))
                    BiayaKirim = ModuleAngka.ParseDecimal(rd("BIAYA_KIRIM"))
                Else
                    MessageBox.Show("Data tidak ditemukan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If
            End Using
        End Using

        ' ✅ Set semua field dari data DB - tidak ada nilai stale
        TxtSupplier.Text = NamaSupplier
        TxtNota.Text = NotaPembelian
        DTPTgl.Value = TanggalBeli
        LblLokasiBarang.Text = Lokasi
        CmbJenisBayarTunai.Text = JenisBayar
        TxtNominalBayarTunai.Text = Pembayaran.ToString()
        TxtKembaliHutang.Text = Tagihan.ToString()
        TxtBAntuanbayar.Text = Tagihan.ToString()
        LblStatusTransLunas.Text = StatusTransaksi
        TxtLogin.Text = IDUser
        TxtKomputer.Text = IDKomputer
        TxtNominalBayarTransfer.Text = NominalTransfer.ToString()
        TxtJenisBayarTransfer.Text = KodeAkunTF
        CmbJenisBayarTransfer.Text = NamaAkunTF
        TxtDiskonRp.Text = DiskonSupplier.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakRp.Text = PpnMasukan.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtBiayaKirim.Text = BiayaKirim.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        ' Reset field persen — tidak disimpan di DB, cukup kosongkan
        TxtDiskonPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

        ' ✅ Set jatuh tempo dan tampilan status hutang
        If StatusTransaksi = "Belum Lunas" AndAlso JatuhTempo <> Date.MinValue Then
            DTPJatuhTempo.Value = JatuhTempo
            LblJatuhTempo.Visible = True
            DTPJatuhTempo.Visible = True
            LblPembayaran.Text = "Hutang :"
        Else
            LblJatuhTempo.Visible = False
            DTPJatuhTempo.Visible = False
            LblPembayaran.Text = "Kembalian :"
        End If
    End Sub


    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "Beli"}
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
                       "F2      : Fokus ke Supplier" & vbCrLf &
                       "F3      : Buka form pilih Supplier" & vbCrLf &
                       "F4      : Buka form pilih Barang" & vbCrLf &
                       "F6      : Tahan transaksi (draft)" & vbCrLf &
                       "F7      : Panggil transaksi ditahan" & vbCrLf &
                       "F8      : Bayar / proses pembayaran" & vbCrLf &
                       "F9      : Fokus ke Jenis Bayar" & vbCrLf &
                       "F10     : Simpan transaksi" & vbCrLf &
                       "F11     : Batal pembayaran" & vbCrLf &
                       "ESC     : Keluar / Tutup panel" & vbCrLf &
                       "↓       : Pindah ke list hasil pencarian" & vbCrLf &
                       "ENTER   : Pilih item dari list" & vbCrLf &
                       "DELETE  : Hapus baris di grid"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                    MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub



End Class