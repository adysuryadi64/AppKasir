Public Class FormReturBeli
    ' ============================================
    ' DEKLARASI VARIABEL DAN KONSTANTA
    ' ============================================
#Region "Deklarasi Variabel dan Cache"

    Private ReadOnly LokasiBarang As String
    Private namaBarangLookupCache As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private listBarangOriginalLocation As Point
    Private currentGridRow As Integer

    Private TanggalAsliEdit As DateTime = DateTime.MinValue

    ' ============================================
    ' PROPERTY MODE DETECTION
    ' ============================================
    Private ReadOnly Property IsModeTambahReturBeli As Boolean
        Get
            Return String.Equals(LblJenisTrans.Text, "TambahReturBeli", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    Private ReadOnly Property IsModeEditReturBeli As Boolean
        Get
            Return Not IsModeTambahReturBeli
        End Get
    End Property

    ' ============================================
    ' KELAS DATA SUPPLIER
    ' ============================================
    Public Class DataSupplier
        Public Property Kode As String
        Public Property Nama As String
        Public Property Alamat As String
        Public Property HP As String

        Public Overrides Function ToString() As String
            Return Nama
        End Function
    End Class

    ' ============================================
    ' KELAS DATA BARANG UNTUK CACHE
    ' ============================================
    Public Class DataBarang
        Public Property ID_BARANG As String
        Public Property NAMA_BARANG As String
        Public Property HARGA_BELI_TERAKHIR As Decimal
        Public Property SATUAN_UMUM_KECIL As String
        Public Property SATUAN_UMUM_SEDANG As String
        Public Property SATUAN_UMUM_BESAR As String
        Public Property ISI_UMUM_KECIL As Integer
        Public Property ISI_UMUM_SEDANG As Integer
        Public Property ISI_UMUM_BESAR As Integer
        Public Property BARCODE_KECIL As String
        Public Property BARCODE_SEDANG As String
        Public Property BARCODE_BESAR As String
        Public Property STOK_TOKO As Decimal
        Public Property STOK_GUDANG As Decimal

        ' ============================================
        ' METHOD UNTUK MENGAMBIL STOK BERDASARKAN LOKASI
        ' ============================================
        Public Function GetStokByLokasi(lokasi As String) As Decimal
            Select Case lokasi.ToUpper()
                Case "TOKO"
                    Return STOK_TOKO
                Case "GUDANG"
                    Return STOK_GUDANG
                Case Else
                    Return 0
            End Select
        End Function

    End Class

    ' ============================================
    ' KELAS DATA ITEM UNTUK LISTBOX
    ' ============================================
    Public Class ListBarangItem
        Public Property NamaBarang As String
        Public Property Stok As Decimal
        Public Property ID_Barang As String
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal

        Public Sub New(nama As String, stok As Decimal, id As String, Optional stokToko As Decimal = 0, Optional stokGudang As Decimal = 0)
            Me.NamaBarang = nama
            Me.Stok = stok
            Me.ID_Barang = id
            Me.StokToko = stokToko
            Me.StokGudang = stokGudang
        End Sub

        ' ============================================
        ' FORMAT TAMPILAN UNTUK LISTBOX
        ' ============================================
        Public Overrides Function ToString() As String
            If Stok < 0 Then
                Return $"{NamaBarang} [Stok: -]"
            Else
                Return $"{NamaBarang} [Stok: {Stok:N0}]"
            End If
        End Function

        ' ============================================
        ' METHOD UNTUK MENGEXTRACT NAMA BARANG DARI TEXT
        ' ============================================
        Public Shared Function ExtractNamaBarang(displayText As String) As String
            Dim indexBracket As Integer = displayText.IndexOf(" [Stok:")
            If indexBracket >= 0 Then
                Return displayText.Substring(0, indexBracket).Trim()
            End If
            Return displayText.Trim()
        End Function
    End Class

    ' ============================================
    ' KELAS DATA STOK INFO
    ' ============================================
    Public Class StokInfo
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    ' ============================================
    ' VARIABEL DATA SUPPLIER
    ' ============================================
    Private ListDataSupplier As New List(Of DataSupplier)
    Private IsSelectingSupplier As Boolean = False

    ' ============================================
    ' VARIABEL CACHE SYSTEM
    ' ============================================
    Private barangCacheById As New Dictionary(Of String, DataBarang)
    Private barcodeLookupCache As New Dictionary(Of String, String)
    Private barcodeToSatuanCache As New Dictionary(Of String, Tuple(Of String, Integer))
    Private lastCacheRefresh As DateTime = DateTime.MinValue

    ' ============================================
    ' VARIABEL BARCODE SCANNER HANDLER
    ' ============================================
    Private lastBarcodeInput As String = String.Empty
    Private WithEvents BarcodeTimer As New Timer()
    Private Const BARCODE_TIMEOUT_MS As Integer = 100
    Private isBarcodeMode As Boolean = False

    ' Barcode buffer untuk DGV inline search
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100

    ' ============================================
    ' VARIABEL DEBOUNCED SEARCH
    ' ============================================
    Private WithEvents SearchDebounceTimer As New Timer()
    Private lastSearchText As String = String.Empty
    Private lastSearchKonteks As String = "TXTNAMA"   ' konteks terakhir: "TXTNAMA" atau "DGV"
    Private Const SEARCH_DEBOUNCE_MS As Integer = 300

    ' ============================================
    ' VARIABEL CACHE REFRESH TIMER
    ' ============================================
    Private WithEvents CacheRefreshTimer As New Timer()

    ' ── State management untuk pencarian ListBox ──────────────────────
    Private _dgvEditingTextBox As TextBox = Nothing         ' TextBox editing control di DGV
    Private _sedangPindahKeLstBarang As Boolean = False     ' Flag saat fokus pindah ke ListBox
    Private _listBoxBaruDapatFokus As Boolean = False       ' Guard: blok Click saat ListBox baru dapat fokus via keyboard
    Private _teksSebelumPindahKeLstBarang As String = ""    ' Teks tersimpan sebelum pindah ke ListBox
    Private _rowSaatPindahKeLst As Integer = -1             ' Baris DGV saat pindah ke ListBox
    Private _konteksLstBarang As String = "TXTNAMA"         ' Konteks: "TXTNAMA" atau "DGV"
    Private _sedangSetNilaiDariListBox As Boolean = False   ' Guard CellEndEdit saat isi dari ListBox
    Private _isLoadingForm As Boolean = False               ' Guard untuk event selama proses load data
    Private _formSudahSiap As Boolean = False               ' Guard: form belum boleh terima fokus
    Private _sedangSetFokusAwal As Boolean = False          ' Guard: cegah rekursi TxtNama_GotFocus

#End Region

    ' ============================================
    ' EVENT HANDLER - LOAD, KEYBOARD DAN CLOSING
    ' ============================================
#Region "Event Handler Form"

    ' ============================================
    ' TANGKAP DOWN KE LISTBOX SEBELUM DGV MEMPROSESNYA
    ' ============================================
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        ' Handle navigasi keyboard ke ListBox Barang
        If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            Select Case keyData
                Case Keys.Down
                    If LstBarang.Focused Then
                        Return MyBase.ProcessCmdKey(msg, keyData)
                    End If
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                    Else
                        _teksSebelumPindahKeLstBarang = TxtNama.Text
                    End If
                    _sedangPindahKeLstBarang = True
                    ' KRITIS: Inisialisasi SelectedIndex agar item pertama langsung ter-highlight
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    Me.BeginInvoke(New Action(Sub()
                                                  Me.BeginInvoke(New Action(Sub()
                                                                                If LstBarang.Visible Then
                                                                                    _sedangSetNilaiDariListBox = True
                                                                                    DgvData.EndEdit()
                                                                                    _sedangSetNilaiDariListBox = False
                                                                                    _listBoxBaruDapatFokus = True
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
                    LstBarang.Visible = False
                    LstBarang.Items.Clear()
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _dgvEditingTextBox.Focus()
                    Else
                        TxtNama.Focus()
                    End If
                    Return True
            End Select
        End If

        ' Handle navigasi keyboard ke ListBox Supplier (Point 6)
        If listSupplier.Visible AndAlso listSupplier.Items.Count > 0 Then
            Select Case keyData
                Case Keys.Down
                    If Not listSupplier.Focused Then
                        listSupplier.Focus()
                        listSupplier.SelectedIndex = 0
                        Return True
                    End If
                Case Keys.Enter
                    If listSupplier.SelectedIndex < 0 Then listSupplier.SelectedIndex = 0
                    ' Ambil item yang dipilih dan tutup list
                    If listSupplier.SelectedItem IsNot Nothing Then
                        PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
                    End If
                    Return True
                Case Keys.Escape
                    listSupplier.Visible = False
                    TxtSupplier.Focus()
                    Return True
            End Select
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' ============================================
    ' EVENT: FORM LOAD - INISIALISASI AWAL
    ' ============================================
    Private Sub FormReturBeli_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            _isLoadingForm = True ' Point 5: Aktifkan guard
            ModuleTheme.TerapkanTheme(Me)
            ' Area input dan grand total otomatis via nama kontrol
            ' Rename GroupBox -> GBInput/GBTotal untuk tema otomatis
            ModuleTheme.SetWarnaRtbCatatan(RTBAlasanRetur)

            ' Set ukuran form
            MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
            MinimumSize = Size

            ' Inisialisasi komponen
            InisialisasiKomponen()

            ' Setup Cache System
            SetupCacheSystem()

            ' Setup Barcode Scanner Handler
            SetupBarcodeHandler()

            ' Setup Debounced Search
            SetupDebouncedSearch()

            ' Baca hak akses
            BacaHakAkses()

            ' Simpan posisi asli ListBarang
            listBarangOriginalLocation = LstBarang.Location


            UpdateLokasiDisplay()

            ' Set kondisi awal berdasarkan jenis transaksi
            If LblJenisTrans.Text = "TambahReturBeli" Then
                Kondisiawal()
            Else
                Kondisiawaledit()
                AmbilDataSupplier()
                AmbilDataUntukEdit()
            End If

            ' Atur fokus awal
            AturFokusAwal()

        Catch ex As Exception
            MessageBox.Show($"Error saat memuat form: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub FormReturBeli_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Try
            ' Paksa maximize — WindowState di designer sering diabaikan saat ShowDialog
            If Me.WindowState <> FormWindowState.Maximized Then
                Me.WindowState = FormWindowState.Maximized
            End If

            ' Paksa refresh TextAlign TxtGrandtotal — mencegah bug caching perataan teks saat form di-resize/maximize
            If TxtGrandtotal IsNot Nothing Then
                TxtGrandtotal.TextAlign = HorizontalAlignment.Left
                TxtGrandtotal.TextAlign = HorizontalAlignment.Right
            End If

            ' Fokus awal setelah form muncul sepenuhnya
            _formSudahSiap = True
            SetupFocusToGrid()

            _isLoadingForm = False ' Point 5: Matikan guard setelah form siap sepenuhnya

        Catch ex As Exception

        End Try
    End Sub


    Private Sub LakukanCetakReturBeli(noRetur As String)
        If BacaPengaturanPrinter("ReturBeli", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterReturBeli.TanyaPilihPrinterReturBeli(noRetur)
        Else
            ModulePrinterReturBeli.CetakReturBeli(noRetur)
        End If
    End Sub

    ' ============================================
    ' EVENT: KEY DOWN PADA FORM (SHORTCUT KEY)
    ' ============================================
    Private Sub FormReturBeli_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                ' F1: Tampilkan bantuan
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F2
                ' F2: Fokus ke supplier
                e.SuppressKeyPress = True
                TxtSupplier.Focus()
                TxtSupplier.SelectAll()
            Case Keys.F3
                ' F3: Fokus ke pencarian barang
                e.SuppressKeyPress = True
                TxtNama.Focus()
                TxtNama.SelectAll()
            Case Keys.F4
                ' F4: Tampilkan/sembunyikan kolom tambahan di grid
                e.SuppressKeyPress = True
                ToggleKolomGrid()
            Case Keys.F5
                ' F5: Refresh cache barang
                e.SuppressKeyPress = True
                LoadBarcodeCache()
                MessageBox.Show("Cache barang telah direfresh", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            Case Keys.F6
                ' F6: Hitung ulang total
                e.SuppressKeyPress = True
                UpdateSemuaTotal()
            Case Keys.F7
                ' F7: Clear semua input
                e.SuppressKeyPress = True
                If MessageBox.Show("Hapus semua barang dari daftar?", "Konfirmasi",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    DgvData.Rows.Clear()
                    UpdateSemuaTotal()
                End If
            Case Keys.F8
                ' F8: Simpan transaksi
                e.SuppressKeyPress = True
                Tekansimpan()
            Case Keys.F9
                ' F9: Fokus ke jenis bayar (jika GBBayar tampil)
                e.SuppressKeyPress = True
                If GBBayar.Visible Then
                    CmbAkunTunai.Focus()
                    CmbAkunTunai.DroppedDown = True
                End If
            Case Keys.F10
                ' F10: Simpan dari GBBayar
                e.SuppressKeyPress = True
                If GBBayar.Visible Then
                    ProsesSimpan()
                End If
            Case Keys.F11
                ' F11: Batal dari GBBayar
                e.SuppressKeyPress = True
                If GBBayar.Visible Then
                    GBBayar.Visible = False
                End If
            Case Keys.F12
                ' F12: Cetak retur
                e.SuppressKeyPress = True
                CetakRetur()
            Case Keys.Escape
                ' ESC: Keluar atau tutup dialog
                If GBBayar.Visible Then
                    GBBayar.Visible = False
                    e.SuppressKeyPress = True
                Else
                    Close()
                End If
            Case Keys.Tab
                ' Tab untuk navigasi cepat antar kolom
                If DgvData.Focused Or DgvData.IsCurrentCellInEditMode Then
                    Dim currentCol = DgvData.CurrentCell.ColumnIndex
                    Dim currentRow = DgvData.CurrentCell.RowIndex

                    If e.Shift Then
                        ' Shift+Tab: ke kiri
                        If currentCol > 1 Then ' Lewati kolom ID
                            DgvData.CurrentCell = DgvData.Rows(currentRow).Cells(currentCol - 1)
                        End If
                    Else
                        ' Tab: ke kanan
                        If currentCol < DgvData.ColumnCount - 1 Then
                            DgvData.CurrentCell = DgvData.Rows(currentRow).Cells(currentCol + 1)
                        ElseIf currentRow < DgvData.RowCount - 1 Then
                            ' Pindah ke baris berikutnya, kolom pertama yang bisa diedit
                            DgvData.CurrentCell = DgvData.Rows(currentRow + 1).Cells(1) ' Kolom NAMA_BARANG
                        End If
                    End If
                    e.SuppressKeyPress = True
                End If
        End Select
    End Sub

#Region "Shortcut Functions"

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F2      : Fokus ke Supplier" & vbCrLf &
                           "F3      : Fokus ke Pencarian Barang" & vbCrLf &
                           "F4      : Toggle kolom tersembunyi" & vbCrLf &
                           "F5      : Refresh cache barang" & vbCrLf &
                           "F6      : Hitung ulang total" & vbCrLf &
                           "F7      : Hapus semua barang" & vbCrLf &
                           "F8      : Simpan transaksi" & vbCrLf &
                           "F9      : Fokus ke Jenis Bayar" & vbCrLf &
                           "F10     : Simpan retur" & vbCrLf &
                           "F11     : Batal simpan" & vbCrLf &
                           "F12     : Cetak retur" & vbCrLf &
                           "ESC     : Keluar/Tutup" & vbCrLf &
                           "↓ (Panah Bawah): Pindah ke list hasil pencarian" & vbCrLf &
                           "↑ (Panah Atas) : Kembali ke input pencarian" & vbCrLf &
                           "ENTER   : Pilih item dari list" & vbCrLf &
                           "DELETE  : Hapus supplier/baris"

        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' ============================================
    ' FUNGSI: TOGGLE KOLOM GRID
    ' ============================================
    Private Sub ToggleKolomGrid()
        ' Toggle visibility kolom tersembunyi
        Dim toggleState As Boolean = Not DgvData.Columns("ISI_SATUAN").Visible

        DgvData.Columns("ISI_SATUAN").Visible = toggleState
        DgvData.Columns("HARGA_BELI_SATUAN").Visible = toggleState
        DgvData.Columns("QTY_SAT").Visible = toggleState

        Dim status As String = If(toggleState, "ditampilkan", "disembunyikan")
    End Sub

    ' ============================================
    ' FUNGSI: CETAK RETUR
    ' ============================================
    Private Sub CetakRetur()
        If String.IsNullOrEmpty(TxtFaktur.Text) Or TxtFaktur.Text = "0" Then
            MessageBox.Show("Tidak ada retur yang bisa dicetak", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Point 1: Gunakan modul printer Retur Beli (bukan Transfer Barang)
        LakukanCetakReturBeli(TxtFaktur.Text)
    End Sub

#End Region

    ' ============================================
    ' EVENT: FORM CLOSING - CLEANUP RESOURCES
    ' ============================================
    Private Sub FormReturBeli_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Cleanup timers dan cache
        CleanupResources()
    End Sub

#End Region

    ' ============================================
    ' INISIALISASI DAN CLEANUP
    ' ============================================
#Region "Inisialisasi dan Cleanup"

    ' ============================================
    ' FUNGSI: INISIALISASI KOMPONEN AWAL
    ' ============================================
    Private Sub InisialisasiKomponen()
        ' Set lokasi dari form utama
        LblLokasiBarang.Text = FormUtama.StatusLokasi.Text

        ' Setup GroupBox bayar
        GBBayar.Visible = False
        GBBayar.Location = New Point((Me.Width - GBBayar.Width) \ 2, (Me.Height - GBBayar.Height) \ 2)

        ' Kosongkan textbox pencarian
        KosongTxtboxcari()

        ' Setup supplier
        TxtSupplier.Text = ""
        listSupplier.Visible = False
        KosongkanDataSupplier()

        ' Setup combobox rekening
        IsiComboBoxAkun(CmbAkunTunai, "KAS", "BANK", "EKUITAS")
        IsiComboBoxAkun(CmbAkunTransfer, "BANK")

        ' Set rekening default berdasarkan lokasi (hanya untuk mode tambah)
        If IsModeTambahReturBeli Then
            If LblLokasiBarang.Text = "TOKO" Then
                CmbAkunTunai.SelectedItem = nama_rek_Retur_Pembelian_Toko
            ElseIf LblLokasiBarang.Text = "GUDANG" Then
                CmbAkunTunai.SelectedItem = nama_rek_Retur_Pembelian_Gudang
            End If
        End If

        ' Simpan posisi asli ListBarang
        listBarangOriginalLocation = LstBarang.Location

        ' Setup ListBox LstBarang
        LstBarang.Visible = False

        ' Setup DataGridView
        'SetupDataGridViewNavigation()

        ' Toggle visibility kolom stok berdasarkan setting
        If DgvData.Columns.Contains("StokToko") AndAlso DgvData.Columns.Contains("StokGudang") Then
            If ModulHakAkses.SettingTampilInfoStok Then
                DgvData.Columns("StokToko").Visible = True
                DgvData.Columns("StokGudang").Visible = True
            Else
                DgvData.Columns("StokToko").Visible = False
                DgvData.Columns("StokGudang").Visible = False
            End If
        End If

        ' Sembunyikan panel pencarian jika setting diaktifkan
        If ModulHakAkses.SettingSembunyikanPencarianAtas Then
            PanelCari.Visible = False
        Else
            PanelCari.Visible = True
        End If

        ' GBGrantotal (Panel Total) harus tetap terlihat
        GBGrantotal.Visible = True

        ' Tambahkan handler untuk keydown di grid
        AddHandler DgvData.KeyDown, AddressOf DgvData_CellKeyDown
    End Sub

    ' ============================================
    ' FUNGSI: SETUP DATA GRID VIEW NAVIGATION
    ' ============================================
    Private Sub SetupDataGridViewNavigation()
        ' Atur behavior untuk navigasi keyboard di DataGridView
        DgvData.EditMode = DataGridViewEditMode.EditOnEnter ' Ubah ke EditOnEnter
        DgvData.SelectionMode = DataGridViewSelectionMode.CellSelect

        ' Atur agar Enter dan Tab berjalan normal
        DgvData.StandardTab = True
        DgvData.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2

    End Sub


    ' ============================================
    ' EVENT: KEY DOWN UNTUK KOLOM NAMA BARANG DI GRID
    ' ============================================
    Private Sub DgvData_CellKeyDown(sender As Object, e As KeyEventArgs)
        If DgvData.CurrentCell.ColumnIndex = 1 AndAlso e.KeyCode = Keys.Enter Then
            ' Parse input jika mengandung "*" sebelum diproses
            Dim rowIndex = DgvData.CurrentCell.RowIndex
            Dim cellValue = DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value

            If cellValue IsNot Nothing Then
                Dim inputValue = cellValue.ToString()
                Dim asteriskIndex = inputValue.IndexOf("*")

                If asteriskIndex > 0 Then
                    ' Format valid, proses sekarang
                    DgvData.EndEdit()
                End If
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: BACA HAK AKSES USER
    ' ============================================
    Private Sub BacaHakAkses()
        ' Setting dibaca langsung dari ModulHakAkses property — tidak perlu cache lokal
    End Sub

    ' ============================================
    ' FUNGSI: ATUR FOKUS AWAL
    ' ============================================
    Private Sub AturFokusAwal()
        ' Dipertahankan untuk kompatibilitas — delegasikan ke SetupFocusToGrid
        SetupFocusToGrid()
    End Sub
    ' ============================================
    Private Sub Kondisiawal()
        ' Reset data grid
        TxtSupplier.Clear()
        DgvData.Rows.Clear()
        TxtTotalQTY.Text = "0"
        TxtGrandTotalRetur.Text = "0"
        GBBayar.Visible = False

        ' Setup tanggal
        ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTgl)
        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        ' Setup record
        LblRecord.Text = "0"

        ' Ambil data dan buat nomor retur
        AmbilDataSupplier()
        NomorRetur()
    End Sub

    ' ============================================
    ' FUNGSI: KONDISI AWAL UNTUK EDIT TRANSAKSI
    ' ============================================
    Private Sub Kondisiawaledit()
        ' Setup format tanggal untuk edit
        DTPTgl.Format = DateTimePickerFormat.Custom
        DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
    End Sub

    ' ============================================
    ' FUNGSI: KOSONGKAN TEXTBOX PENCARIAN
    ' ============================================
    Private Sub KosongTxtboxcari()
        ' Kosongkan semua textbox pencarian
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHarga.Clear()
        TxtBarcode.Clear()

        ' Bersihkan pencarian dan list barang
        BersihkanPencarian()
        TxtNama.Clear()
    End Sub

    ' ============================================
    ' FUNGSI: BERSIHKAN PENCARIAN
    ' ============================================
    Private Sub BersihkanPencarian()
        LstBarang.Items.Clear()
        LstBarang.Visible = False
        TxtQty.Text = "1"
        TxtNama.Focus()
        TxtNama.SelectAll()
    End Sub

    ' ============================================
    ' FUNGSI: CLEANUP RESOURCES SAAT FORM CLOSING
    ' ============================================
    Private Sub CleanupResources()
        ' Stop dan dispose semua timer
        If BarcodeTimer IsNot Nothing Then
            BarcodeTimer.Stop()
            BarcodeTimer.Dispose()
        End If

        If SearchDebounceTimer IsNot Nothing Then
            SearchDebounceTimer.Stop()
            SearchDebounceTimer.Dispose()
        End If

        If CacheRefreshTimer IsNot Nothing Then
            CacheRefreshTimer.Stop()
            CacheRefreshTimer.Dispose()
        End If

        ' Clear cache
        ClearAllCache()

        ' Unsubscribe event handlers
        RemoveHandlers()

        ' Force garbage collection
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub

    ' ============================================
    ' FUNGSI: CLEAR ALL CACHE DATA
    ' ============================================
    Private Sub ClearAllCache()
        If barangCacheById IsNot Nothing Then barangCacheById.Clear()
        If barcodeLookupCache IsNot Nothing Then barcodeLookupCache.Clear()
        If barcodeToSatuanCache IsNot Nothing Then barcodeToSatuanCache.Clear()
        If ListDataSupplier IsNot Nothing Then ListDataSupplier.Clear()
    End Sub

    ' ============================================
    ' FUNGSI: REMOVE EVENT HANDLERS
    ' ============================================
    Private Sub RemoveHandlers()
        ' Hapus event handler yang ditambahkan secara dinamis
        RemoveHandler TxtNama.TextChanged, AddressOf TxtNama_TextChanged_Debounced
        RemoveHandler TxtNama.KeyDown, AddressOf TxtNama_KeyDown_Barcode
        RemoveHandler TxtNama.KeyPress, AddressOf TxtNama_KeyPress_Barcode
        RemoveHandler BarcodeTimer.Tick, AddressOf BarcodeTimer_Tick
        RemoveHandler SearchDebounceTimer.Tick, AddressOf SearchDebounceTimer_Tick
        RemoveHandler CacheRefreshTimer.Tick, AddressOf CacheRefreshTimer_Tick
    End Sub

#End Region

    ' ============================================
    ' CACHE SYSTEM UNTUK PERFORMANCE
    ' ============================================
#Region "Cache System"

    ' ============================================
    ' FUNGSI: SETUP CACHE SYSTEM
    ' ============================================
    Private Sub SetupCacheSystem()
        ' Load cache awal
        LoadBarcodeCache()

        ' Setup periodic cache refresh (setiap 5 menit)
        CacheRefreshTimer.Interval = 300000 ' 5 menit
        AddHandler CacheRefreshTimer.Tick, AddressOf CacheRefreshTimer_Tick
        CacheRefreshTimer.Start()
    End Sub

    ' ============================================
    ' FUNGSI: LOAD BARCODE CACHE DARI DATABASE
    ' ============================================
    Private Sub LoadBarcodeCache()
        Try
            ClearAllCache()

            ' Query untuk mengambil semua data barang
            Dim query = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI_TERAKHIR, " &
               "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
               "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
               "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
               "STOK_TOKO, STOK_GUDANG FROM tbl_barang"

            Using cmd As New MySqlCommand(query, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        ' Ambil data dari reader
                        Dim idBarang As String = rd("ID_BARANG").ToString()
                        Dim namaBarang As String = rd("NAMA_BARANG").ToString()

                        ' Handle nullable integer conversions
                        Dim isiKecil As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        Dim isiSedang As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        Dim isiBesar As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))

                        ' Handle nullable decimal conversions
                        Dim hargaBeli As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI_TERAKHIR", 0D)
                        Dim stokToko As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                        Dim stokGudang As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)

                        ' Buat objek DataBarang
                        Dim barang As New DataBarang With {
                        .ID_BARANG = idBarang,
                        .NAMA_BARANG = namaBarang,
                        .HARGA_BELI_TERAKHIR = hargaBeli,
                        .SATUAN_UMUM_KECIL = rd("SATUAN_UMUM_KECIL").ToString(),
                        .SATUAN_UMUM_SEDANG = rd("SATUAN_UMUM_SEDANG").ToString(),
                        .SATUAN_UMUM_BESAR = rd("SATUAN_UMUM_BESAR").ToString(),
                        .ISI_UMUM_KECIL = isiKecil,
                        .ISI_UMUM_SEDANG = isiSedang,
                        .ISI_UMUM_BESAR = isiBesar,
                        .BARCODE_KECIL = rd("BARCODE_KECIL").ToString(),
                        .BARCODE_SEDANG = rd("BARCODE_SEDANG").ToString(),
                        .BARCODE_BESAR = rd("BARCODE_BESAR").ToString(),
                        .STOK_TOKO = stokToko,
                        .STOK_GUDANG = stokGudang
                    }

                        ' Cache by ID - HAPUS DUPLIKASI BARIS INI
                        barangCacheById(idBarang) = barang

                        ' TAMBAHKAN: Cache untuk lookup cepat berdasarkan nama
                        namaBarangLookupCache(barang.NAMA_BARANG.ToUpper()) = idBarang

                        ' Cache barcode lookup
                        Dim barcodeKecil = rd("BARCODE_KECIL").ToString()
                        Dim barcodeSedang = rd("BARCODE_SEDANG").ToString()
                        Dim barcodeBesar = rd("BARCODE_BESAR").ToString()

                        If Not String.IsNullOrEmpty(barcodeKecil) Then
                            barcodeLookupCache(barcodeKecil) = idBarang
                            barcodeToSatuanCache(barcodeKecil) = Tuple.Create(
                            rd("SATUAN_UMUM_KECIL").ToString(),
                            isiKecil)
                        End If

                        If Not String.IsNullOrEmpty(barcodeSedang) Then
                            barcodeLookupCache(barcodeSedang) = idBarang
                            barcodeToSatuanCache(barcodeSedang) = Tuple.Create(
                            rd("SATUAN_UMUM_SEDANG").ToString(),
                            isiSedang)
                        End If

                        If Not String.IsNullOrEmpty(barcodeBesar) Then
                            barcodeLookupCache(barcodeBesar) = idBarang
                            barcodeToSatuanCache(barcodeBesar) = Tuple.Create(
                            rd("SATUAN_UMUM_BESAR").ToString(),
                            isiBesar)
                        End If
                    End While
                End Using
            End Using

            lastCacheRefresh = DateTime.Now

        Catch ex As Exception
            MessageBox.Show($"Error loading barcode cache: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' EVENT: CACHE REFRESH TIMER TICK
    ' ============================================
    Private Sub CacheRefreshTimer_Tick(sender As Object, e As EventArgs)
        Try
            LoadBarcodeCache()
        Catch ex As Exception
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: GET BARANG FROM CACHE BY ID
    ' ============================================
    Private Function GetBarangFromCache(kodeBarang As String) As DataBarang
        If barangCacheById.ContainsKey(kodeBarang) Then
            Return barangCacheById(kodeBarang)
        End If
        Return Nothing
    End Function

    ' ============================================
    ' FUNGSI: GET BARANG BY BARCODE FROM CACHE
    ' ============================================
    Private Function GetBarangByBarcode(barcode As String) As DataBarang
        If barcodeLookupCache.ContainsKey(barcode) Then
            Dim idBarang = barcodeLookupCache(barcode)
            Return GetBarangFromCache(idBarang)
        End If
        Return Nothing
    End Function

#End Region

    ' ============================================
    ' BARCODE SCANNER HANDLER (FAST PATH)
    ' ============================================
#Region "Barcode Scanner Handler"

    ' ============================================
    ' FUNGSI: SETUP BARCODE HANDLER
    ' ============================================
    Private Sub SetupBarcodeHandler()
        ' Timer untuk mendeteksi barcode scanner
        BarcodeTimer.Interval = BARCODE_TIMEOUT_MS
        BarcodeTimer.Stop()
        AddHandler BarcodeTimer.Tick, AddressOf BarcodeTimer_Tick

        ' Setup event handlers khusus untuk barcode
        AddHandler TxtNama.KeyDown, AddressOf TxtNama_KeyDown_Barcode
        AddHandler TxtNama.KeyPress, AddressOf TxtNama_KeyPress_Barcode
    End Sub

    ' ============================================'
    ' EVENT: KEY DOWN UNTUK BARCODE SCANNER
    ' ============================================'
    Private Sub TxtNama_KeyDown_Barcode(sender As Object, e As KeyEventArgs)
        ' Deteksi Enter dari barcode scanner
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True

            If Not String.IsNullOrEmpty(TxtNama.Text) Then
                ProcessBarcodeInput(TxtNama.Text.Trim())
            End If

            ResetBarcodeInput()
        ElseIf e.KeyCode = Keys.Down Then
            ' PANAH BAWAH: Pindah ke ListBox
            If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
                _teksSebelumPindahKeLstBarang = TxtNama.Text
                _konteksLstBarang = "TXTNAMA"
                _sedangPindahKeLstBarang = True
                ' JANGAN set SelectedIndex di sini — bocor ke ListBox via Down key
                Me.BeginInvoke(New Action(Sub()
                                              Me.BeginInvoke(New Action(Sub()
                                                                            If LstBarang.Visible Then
                                                                                _listBoxBaruDapatFokus = True
                                                                                LstBarang.Focus()
                                                                            End If
                                                                            _sedangPindahKeLstBarang = False
                                                                        End Sub))
                                          End Sub))
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Up Then
            ' PANAH ATAS: Pindah ke DataGridView
            If DgvData.Rows.Count > 0 Then
                DgvData.Focus()
                'DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    ' ============================================
    ' EVENT: KEY PRESS UNTUK BARCODE SCANNER
    ' ============================================
    Private Sub TxtNama_KeyPress_Barcode(sender As Object, e As KeyPressEventArgs)
        ' Deteksi karakter cepat (barcode scanner biasanya sangat cepat)
        Static lastKeyPressTime As DateTime = DateTime.Now
        Dim currentTime As DateTime = DateTime.Now
        Dim elapsedMs = (currentTime - lastKeyPressTime).TotalMilliseconds

        ' Jika waktu antar karakter < 50ms, kemungkinan barcode scanner
        If elapsedMs < 50 AndAlso elapsedMs > 0 Then
            isBarcodeMode = True
        Else
            isBarcodeMode = False
        End If

        lastKeyPressTime = currentTime

        ' Start/restart timer untuk mengumpulkan input barcode
        BarcodeTimer.Stop()
        BarcodeTimer.Start()
    End Sub

    ' ============================================
    ' EVENT: BARCODE TIMER TICK
    ' ============================================
    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        BarcodeTimer.Stop()

        ' KRITIS: jangan proses apapun saat ListBox masih visible
        ' User sedang memilih dari ListBox — jangan ganggu dengan proses barcode/search
        If LstBarang.Visible Then
            ResetBarcodeDetection()
            Return
        End If

        ' Handle untuk TxtNama
        If _konteksLstBarang = "TXTNAMA" OrElse _konteksLstBarang = "" Then
            ' Jika timer expired dan ada input, proses sebagai barcode
            If Not String.IsNullOrEmpty(TxtNama.Text) AndAlso isBarcodeMode Then
                ProcessBarcodeInput(TxtNama.Text.Trim())
                ResetBarcodeInput()
            End If
        ElseIf _konteksLstBarang = "DGV" Then
            ' Handle untuk DGV inline search
            Dim elapsedSinceLastKey = (DateTime.Now - lastKeyTime).TotalMilliseconds

            If elapsedSinceLastKey > 100 Then
                Dim bufferText = New String(barcodeChars.ToArray())
                If bufferText.Length >= BARCODE_MIN_LENGTH Then
                    ' Jika buffer mengandung '*' atau huruf → input manual bertempo cepat
                    If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                        ' Jalur DGV — manual search sudah ditangani TextChanged, tidak perlu ulang
                        ResetBarcodeDetection()
                        Return
                    End If

                    ' Murni numerik/alphanumeric → kandidat barcode
                    ' Parse qty*level jika ada
                    Dim keyword As String = bufferText
                    Dim qty As Decimal = 1
                    Dim level As Integer = 1

                    If bufferText.Contains("*") Then
                        Dim parts = bufferText.Split("*"c)
                        If parts.Length >= 1 AndAlso Decimal.TryParse(parts(0).Trim(), qty) AndAlso qty > 0 Then
                            TxtQty.Text = qty.ToString()
                        End If
                        If parts.Length >= 2 Then
                            If Integer.TryParse(parts(1).Trim(), level) AndAlso level >= 1 AndAlso level <= 3 Then
                                TxtLevel.Text = level.ToString()
                            End If
                            keyword = parts(parts.Length - 1).Trim()
                        Else
                            keyword = parts(parts.Length - 1).Trim()
                        End If
                    End If

                    ' Proses barcode input
                    ProcessBarcodeInput(keyword)
                    ResetBarcodeDetection()
                End If
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PROCESS BARCODE INPUT
    ' ============================================
    Private Sub ProcessBarcodeInput(barcode As String)
        If LstBarang.Visible Then
            Return
        End If
        If barcodeLookupCache.ContainsKey(barcode) Then
            Dim idBarang As String = barcodeLookupCache(barcode)
            Dim barangInfo = barcodeToSatuanCache(barcode)

            ' Langsung proses tanpa query database
            ProcessBarangFromCache(idBarang, barcode, barangInfo.Item1, barangInfo.Item2)
        Else
            ' Fallback ke database jika barcode tidak ditemukan di cache
            HandleCacheMiss(barcode)
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PROCESS BARANG FROM CACHE
    ' ============================================
    Private Sub ProcessBarangFromCache(idBarang As String, barcode As String, satuan As String, isi As Integer)
        Try
            Dim barang As DataBarang = barangCacheById(idBarang)

            ' Validasi duplikasi barang
            If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                For Each row As DataGridViewRow In DgvData.Rows
                    If row.Cells("ID_BARANG").Value IsNot Nothing AndAlso
                       row.Cells("ID_BARANG").Value.ToString() = idBarang Then
                        MessageBox.Show(barang.NAMA_BARANG & " sudah ada dalam daftar!",
                                      "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        ResetBarcodeInput()
                        Return
                    End If
                Next
            End If

            ' Set nilai langsung ke TextBox
            TxtKode.Text = idBarang
            TxtNama.Text = barang.NAMA_BARANG
            TxtBarcode.Text = barcode
            Txtsatuan.Text = satuan
            TxtIsi.Text = isi.ToString()
            TxtHarga.Text = barang.HARGA_BELI_TERAKHIR.ToString("N0")
            TxtQty.Text = "1"

            ' Tambahkan ke DataGridView
            AddBarangToGrid(barang, satuan, isi)

            ' Reset input
            ResetBarcodeInput()

        Catch ex As Exception
            MessageBox.Show($"Error processing barcode: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: ADD BARANG TO GRID FROM CACHE
    ' ============================================
    Private Sub AddBarangToGrid(barang As DataBarang, satuan As String, isi As Integer)

        Dim rowIndex As Integer = DgvData.Rows.Add()

        ' Setup satuan combo
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(rowIndex).Cells("SATUAN"), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()

        ' Tambahkan satuan yang tersedia dari cache
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_KECIL) Then kolomSatuan.Items.Add(barang.SATUAN_UMUM_KECIL)
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_SEDANG) Then kolomSatuan.Items.Add(barang.SATUAN_UMUM_SEDANG)
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_BESAR) Then kolomSatuan.Items.Add(barang.SATUAN_UMUM_BESAR)

        ' Set nilai ke grid
        DgvData.Rows(rowIndex).Cells("ID_BARANG").Value = barang.ID_BARANG
        DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value = barang.NAMA_BARANG
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = barang.HARGA_BELI_TERAKHIR
        DgvData.Rows(rowIndex).Cells("QTY").Value = 1
        DgvData.Rows(rowIndex).Cells("SATUAN").Value = satuan
        DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = isi
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = barang.HARGA_BELI_TERAKHIR * isi
        DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = 1 * isi
        DgvData.Rows(rowIndex).Cells("TOTAL").Value = barang.HARGA_BELI_TERAKHIR * (1 * isi)
        DgvData.Rows(rowIndex).Cells("StokToko").Value = barang.STOK_TOKO
        DgvData.Rows(rowIndex).Cells("StokGudang").Value = barang.STOK_GUDANG

        UpdateSemuaTotal()
        SetWarnaReadOnlyNama(rowIndex)
        KosongTxtboxcari()
    End Sub

    ' ============================================
    ' FUNGSI: RESET BARCODE DETECTION
    ' ============================================
    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False
        barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue
        lastKeyTime = DateTime.MinValue
        BarcodeTimer.Stop()
    End Sub

    ' ============================================
    ' FUNGSI: RESET BARCODE INPUT (TXTNAMA)
    ' ============================================
    Private Sub ResetBarcodeInput()
        TxtNama.Text = ""
        lastBarcodeInput = String.Empty
        isBarcodeMode = False
        BarcodeTimer.Stop()
    End Sub

    ' ============================================
    ' FUNGSI: HANDLE CACHE MISS (BARCODE TIDAK ADA DI CACHE)
    ' ============================================
    Private Sub HandleCacheMiss(barcode As String)
        ' Query database untuk barcode yang tidak ada di cache
        Try
            Dim query = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI_TERAKHIR, " &
                       "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                       "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                       "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR " &
                       "FROM tbl_barang WHERE BARCODE_KECIL = @barcode " &
                       "OR BARCODE_SEDANG = @barcode OR BARCODE_BESAR = @barcode"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@barcode", barcode)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        ' Update cache dengan data baru
                        UpdateCacheWithNewItem(rd)

                        ' Proses item dari cache yang sudah di-update
                        Dim idBarang = rd("ID_BARANG").ToString()
                        ProcessBarangFromCache(idBarang, barcode,
                                             GetSatuanFromBarcode(rd, barcode),
                                             GetIsiFromBarcode(rd, barcode))
                    Else
                        MessageBox.Show($"Barcode '{barcode}' tidak ditemukan",
                                      "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        ResetBarcodeInput()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error handling cache miss: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: UPDATE CACHE WITH NEW ITEM
    ' ============================================
    Private Sub UpdateCacheWithNewItem(rd As MySqlDataReader)
        Dim idBarang = rd("ID_BARANG").ToString()
        Dim namaBarang = rd("NAMA_BARANG").ToString()

        ' Update cache by ID
        Dim barang As New DataBarang With {
        .ID_BARANG = idBarang,
        .NAMA_BARANG = namaBarang,
        .HARGA_BELI_TERAKHIR = CDec(rd("HARGA_BELI_TERAKHIR")),
        .SATUAN_UMUM_KECIL = rd("SATUAN_UMUM_KECIL").ToString(),
        .SATUAN_UMUM_SEDANG = rd("SATUAN_UMUM_SEDANG").ToString(),
        .SATUAN_UMUM_BESAR = rd("SATUAN_UMUM_BESAR").ToString(),
        .ISI_UMUM_KECIL = Math.Max(1, CInt(rd("ISI_UMUM_KECIL"))),
        .ISI_UMUM_SEDANG = Math.Max(1, CInt(rd("ISI_UMUM_SEDANG"))),
        .ISI_UMUM_BESAR = Math.Max(1, CInt(rd("ISI_UMUM_BESAR"))),
        .BARCODE_KECIL = rd("BARCODE_KECIL").ToString(),
        .BARCODE_SEDANG = rd("BARCODE_SEDANG").ToString(),
        .BARCODE_BESAR = rd("BARCODE_BESAR").ToString(),
        .STOK_TOKO = 0, ' Default value karena tidak diambil dari query
        .STOK_GUDANG = 0 ' Default value karena tidak diambil dari query
    }

        barangCacheById(idBarang) = barang

        ' Update barcode cache
        Dim barcodes = New String() {
        rd("BARCODE_KECIL").ToString(),
        rd("BARCODE_SEDANG").ToString(),
        rd("BARCODE_BESAR").ToString()
    }

        For Each barcode In barcodes
            If Not String.IsNullOrEmpty(barcode) Then
                barcodeLookupCache(barcode) = idBarang
                barcodeToSatuanCache(barcode) = Tuple.Create(
                GetSatuanFromBarcode(rd, barcode),
                GetIsiFromBarcode(rd, barcode))
            End If
        Next

        ' TAMBAHKAN: Update nama lookup cache
        namaBarangLookupCache(barang.NAMA_BARANG.ToUpper()) = idBarang
    End Sub

    ' ============================================
    ' FUNGSI: GET SATUAN FROM BARCODE
    ' ============================================
    Private Function GetSatuanFromBarcode(rd As MySqlDataReader, barcode As String) As String
        If barcode = rd("BARCODE_KECIL").ToString() Then
            Return rd("SATUAN_UMUM_KECIL").ToString()
        ElseIf barcode = rd("BARCODE_SEDANG").ToString() Then
            Return rd("SATUAN_UMUM_SEDANG").ToString()
        ElseIf barcode = rd("BARCODE_BESAR").ToString() Then
            Return rd("SATUAN_UMUM_BESAR").ToString()
        Else
            Return rd("SATUAN_UMUM_KECIL").ToString()
        End If
    End Function

    ' ============================================
    ' FUNGSI: GET ISI FROM BARCODE
    ' ============================================
    Private Function GetIsiFromBarcode(rd As MySqlDataReader, barcode As String) As Integer
        If barcode = rd("BARCODE_KECIL").ToString() Then
            Return Math.Max(1, CInt(rd("ISI_UMUM_KECIL")))
        ElseIf barcode = rd("BARCODE_SEDANG").ToString() Then
            Return Math.Max(1, CInt(rd("ISI_UMUM_SEDANG")))
        ElseIf barcode = rd("BARCODE_BESAR").ToString() Then
            Return Math.Max(1, CInt(rd("ISI_UMUM_BESAR")))
        Else
            Return Math.Max(1, CInt(rd("ISI_UMUM_KECIL")))
        End If
    End Function

#End Region

    ' ============================================
    ' DEBOUNCED SEARCH UNTUK PENCARIAN MANUAL
    ' ============================================
#Region "Debounced Search"

    ' ============================================
    ' FUNGSI: SETUP DEBOUNCED SEARCH
    ' ============================================
    Private Sub SetupDebouncedSearch()
        SearchDebounceTimer.Interval = SEARCH_DEBOUNCE_MS
        SearchDebounceTimer.Stop()
        AddHandler SearchDebounceTimer.Tick, AddressOf SearchDebounceTimer_Tick

        ' Setup event handler untuk pencarian manual
        AddHandler TxtNama.TextChanged, AddressOf TxtNama_TextChanged_Debounced
    End Sub

    ' ============================================
    ' EVENT: GOT FOCUS UNTUK TEXTBOX NAMA
    ' ============================================
    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)

        'If DgvData.Rows.Count > 0 Then
        '    DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)
        '    DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        'End If
    End Sub

    ' ============================================
    ' EVENT: LOST FOCUS UNTUK TEXTBOX NAMA
    ' ============================================
    Private Sub TxtNama_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_Panel, ModuleTheme.D_Panel)
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED DEBOUNCED UNTUK PENCARIAN
    ' ============================================
    Private Sub TxtNama_TextChanged_Debounced(sender As Object, e As EventArgs) Handles TxtNama.TextChanged
        ' Skip jika dalam mode barcode
        If isBarcodeMode Then Exit Sub

        lastSearchText = TxtNama.Text
        SearchDebounceTimer.Stop()
        SearchDebounceTimer.Start()
    End Sub

    ' ============================================
    ' EVENT: SEARCH DEBOUNCE TIMER TICK
    ' ============================================
    Private Sub SearchDebounceTimer_Tick(sender As Object, e As EventArgs)
        SearchDebounceTimer.Stop()

        If String.IsNullOrEmpty(lastSearchText) Then
            BersihkanPencarian()
            Exit Sub
        End If

        If lastSearchKonteks = "DGV" Then
            ' Jalur DGV — langsung search dengan keyword yang sudah di-parse

            SearchBarangByText(lastSearchText)
        Else
            ' Jalur TxtNama
            _konteksLstBarang = "TXTNAMA"
            Dim parsedResult = ParseSearchInput(lastSearchText)
            If parsedResult.Keyword.Length >= 2 Then
                SearchBarangByText(parsedResult.Keyword, parsedResult.Qty)
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PARSE SEARCH INPUT (FORMAT "QTY*NAMA")
    ' ============================================
    Private Function ParseSearchInput(input As String) As (Qty As Integer, Keyword As String)
        Dim indexAsterisk As Integer = input.IndexOf("*")

        If indexAsterisk >= 0 Then
            Dim qtyPart = input.Substring(0, indexAsterisk).Trim()
            Dim keywordPart = input.Substring(indexAsterisk + 1).Trim()

            Dim qty As Integer
            If Integer.TryParse(qtyPart, qty) AndAlso qty > 0 Then
                TxtQty.Text = qty.ToString()
                Return (qty, keywordPart)
            Else
                TxtQty.Text = "1"
                Return (1, keywordPart)
            End If
        Else
            TxtQty.Text = "1"
            Return (1, input.Trim())
        End If
    End Function

    ' ============================================'
    ' FUNGSI: SEARCH BARANG BY TEXT
    ' ============================================'
    Private Sub SearchBarangByText(keyword As String, Optional qty As Integer = 1)
        Dim results As New List(Of ListBarangItem)
        Dim lokasi = LblLokasiBarang.Text

        ' Cek apakah keyword adalah BARCODE
        If barcodeLookupCache.ContainsKey(keyword) Then
            Dim idBarang = barcodeLookupCache(keyword)
            If barangCacheById.ContainsKey(idBarang) Then
                Dim barang = barangCacheById(idBarang)
                Dim stok = barang.GetStokByLokasi(lokasi)
                results.Add(New ListBarangItem(barang.NAMA_BARANG, stok, idBarang, barang.STOK_TOKO, barang.STOK_GUDANG))
            End If
        Else
            ' Cari di cache by nama
            Dim keywordLower = keyword.ToLower()

            results = barangCacheById.Values.
            Where(Function(b) b.NAMA_BARANG.ToLower().Contains(keywordLower)).
            Select(Function(b) New ListBarangItem(b.NAMA_BARANG, b.GetStokByLokasi(lokasi), b.ID_BARANG, b.STOK_TOKO, b.STOK_GUDANG)).
            OrderByDescending(Function(r) r.Stok >= 0).
            ThenBy(Function(r) r.NamaBarang).
            Take(50).
            ToList()
        End If

        ' Update UI — isi ListBox
        Me.Invoke(Sub()
                      LstBarang.Items.Clear()

                      If results.Count = 0 Then
                          LstBarang.Visible = False
                          If _konteksLstBarang = "TXTNAMA" Then TxtNama.Focus()
                          Exit Sub
                      End If

                      For Each item In results
                          Dim displayString As String
                          If ModulHakAkses.SettingTampilInfoStok Then
                              displayString = String.Format("{0} | T: {1} | G: {2}",
                                  item.NamaBarang, item.StokToko.ToString("N0"), item.StokGudang.ToString("N0"))
                          Else
                              Dim stokVal = If(LblLokasiBarang.Text = "GUDANG", item.StokGudang, item.StokToko)
                              displayString = String.Format("{0} [Stok: {1}]", item.NamaBarang, stokVal.ToString("N0"))
                          End If
                          LstBarang.Items.Add(displayString)
                      Next

                      AturTinggiListBarang()

                      ' Posisikan ListBox sesuai konteks
                      If _konteksLstBarang = "DGV" Then
                          PosisikanLstBarangDiBawahSel()
                      Else
                          PosisikanLstBarangDiBawahTxtNama()
                      End If

                      LstBarang.Visible = True
                      LstBarang.BringToFront()
                      ' Stop BarcodeTimer — jangan ganggu saat user memilih dari ListBox
                      BarcodeTimer.Stop()
                      ResetBarcodeDetection()
                  End Sub)
    End Sub

    ' ============================================
    ' FUNGSI: ATUR TINGGI LIST BARANG (LISTBOX)
    ' ============================================
    Private Sub AturTinggiListBarang()
        Dim baris As Integer = LstBarang.Items.Count
        If baris = 0 Then
            LstBarang.Height = 0
            Return
        End If
        Dim maxHeight As Integer = 300
        Dim itemHeight As Integer = If(LstBarang.ItemHeight > 0, LstBarang.ItemHeight, 20)
        LstBarang.Height = Math.Min(baris * itemHeight + 4, maxHeight)
    End Sub

    ' ============================================
    ' FUNGSI: POSISIKAN LISTBOX DI BAWAH TXTNAMA
    ' ============================================
    Private Sub PosisikanLstBarangDiBawahTxtNama()
        Dim pt = Me.PointToClient(PanelCari.PointToScreen(New Point(0, PanelCari.Height)))
        LstBarang.Location = New Point(PanelCari.Left, pt.Y)
        LstBarang.Width = PanelCari.Width
        LstBarang.BringToFront()
    End Sub

    ' ============================================
    ' FUNGSI: POSISIKAN LISTBOX DI BAWAH SEL NAMA_BARANG DI DGV
    ' ============================================
    Private Sub PosisikanLstBarangDiBawahSel()
        If DgvData.CurrentCell Is Nothing Then Return
        Dim cellRect = DgvData.GetCellDisplayRectangle(DgvData.CurrentCell.ColumnIndex,
                                                        DgvData.CurrentCell.RowIndex, False)
        Dim pt = Me.PointToClient(DgvData.PointToScreen(New Point(cellRect.Left, cellRect.Bottom)))
        LstBarang.Location = New Point(pt.X, pt.Y)
        LstBarang.Width = Math.Max(cellRect.Width, 510)
        LstBarang.BringToFront()
    End Sub

#End Region

    ' ============================================
    ' MANAJEMEN DATA SUPPLIER
    ' ============================================
#Region "Manajemen Supplier"

    ' ============================================
    ' FUNGSI: AMBIL DATA SUPPLIER DARI DATABASE
    ' ============================================
    Private Sub AmbilDataSupplier()
        Try
            ListDataSupplier.Clear()

            Using cmd As New MySqlCommand("SELECT KODE, NAMA, ALAMAT, HP FROM tbl_supliyer WHERE Status = 'Aktif' ORDER BY NAMA", conn)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        ListDataSupplier.Add(New DataSupplier With {
                            .Kode = rd.GetString(0),
                            .Nama = rd.GetString(1),
                            .Alamat = rd.GetString(2),
                            .HP = rd.GetString(3)
                        })
                    End While
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error mengambil data supplier: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: KOSONGKAN DATA SUPPLIER
    ' ============================================
    Private Sub KosongkanDataSupplier()
        LblKodeSupplier.Text = ""
        LblAlamatSupplier.Text = ""
        LblKontakSupplier.Text = ""
    End Sub

    ' ============================================
    ' FUNGSI: FILTER SUPPLIER BERDASARKAN INPUT
    ' ============================================
    Private Sub FilterSupplier()
        If IsSelectingSupplier Then Exit Sub

        Dim filter As String = TxtSupplier.Text.Trim().ToLower()
        listSupplier.Items.Clear()

        If filter = "" Then
            listSupplier.Visible = False
            KosongkanDataSupplier()
            Exit Sub
        End If

        ' Filter supplier berdasarkan nama atau kontak
        Dim hasil = ListDataSupplier.
        Where(Function(x) x.Nama.ToLower().Contains(filter) _
                      Or x.HP.ToLower().Contains(filter)).
        ToList()

        If hasil.Count = 0 Then
            listSupplier.Visible = False
            Exit Sub
        End If

        If LblJenisTrans.Text <> "TambahReturBeli" Then
            If hasil.Count = 1 Then
                PilihSupplierLangsung(hasil(0), False)   ' ← tetap di txtSupplier
                Exit Sub
            End If
        End If

        ' Tambahkan hasil ke listbox
        For Each s In hasil
            listSupplier.Items.Add(s)
        Next
        AturTinggiListSupplier()
        listSupplier.Visible = True

        '' Auto-select item pertama
        'If listSupplier.Items.Count > 0 Then
        '    listSupplier.SelectedIndex = 0
        'End If
    End Sub

    ' ============================================
    ' FUNGSI: ATUR TINGGI LIST SUPPLIER
    ' ============================================
    Private Sub AturTinggiListSupplier()
        Dim baris As Integer = listSupplier.Items.Count

        If baris = 0 Then
            listSupplier.Height = 0
            Return
        End If

        Dim tinggiBaris As Integer = listSupplier.ItemHeight

        If baris <= 10 Then
            listSupplier.Height = baris * tinggiBaris + 4
            listSupplier.ScrollAlwaysVisible = False
        Else
            listSupplier.Height = 10 * tinggiBaris + 4
            listSupplier.ScrollAlwaysVisible = True
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PILIH SUPPLIER LANGSUNG
    ' ============================================
    Private Sub PilihSupplierLangsung(s As DataSupplier, Optional PindahKeBarang As Boolean = False)
        IsSelectingSupplier = True

        ' Set data supplier ke kontrol
        TxtSupplier.Text = s.Nama
        LblKodeSupplier.Text = s.Kode
        LblAlamatSupplier.Text = s.Alamat
        LblKontakSupplier.Text = s.HP

        ' Bersihkan dan sembunyikan list
        listSupplier.Items.Clear()
        listSupplier.Visible = False

        ' Fokus kembali ke textbox supplier
        TxtSupplier.Focus()
        TxtSupplier.SelectionStart = TxtSupplier.Text.Length

        IsSelectingSupplier = False

        ' Jika perlu pindah ke pencarian barang
        If PindahKeBarang Then Fokuskepencarianbarang()
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED UNTUK SUPPLIER
    ' ============================================
    Private Sub TxtSupplier_TextChanged(sender As Object, e As EventArgs) Handles TxtSupplier.TextChanged
        FilterSupplier()
    End Sub

    ' ============================================
    ' EVENT: KEY DOWN UNTUK SUPPLIER
    ' ============================================
    Private Sub TxtSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtSupplier.KeyDown
        If e.KeyCode = Keys.Delete Then
            ' Hapus supplier
            TxtSupplier.Clear()
            KosongkanDataSupplier()
            listSupplier.Visible = False
            Exit Sub
        End If

        If e.KeyCode = Keys.Down AndAlso listSupplier.Visible AndAlso listSupplier.Items.Count > 0 Then
            ' Pindah ke listSupplier dengan panah bawah
            listSupplier.Focus()
            listSupplier.SelectedIndex = 0
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Enter AndAlso listSupplier.Visible AndAlso listSupplier.Items.Count > 0 Then
            ' Enter untuk pilih supplier langsung (Auto-select first item jika belum dipilih)
            Dim selectedItem As DataSupplier = If(listSupplier.SelectedIndex >= 0,
                                                  CType(listSupplier.SelectedItem, DataSupplier),
                                                  CType(listSupplier.Items(0), DataSupplier))

            If selectedItem IsNot Nothing Then
                PilihSupplierLangsung(selectedItem, True)
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Enter Then
            ' Jika tidak ada list, fokus ke pencarian barang
            Fokuskepencarianbarang()
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ============================================
    ' EVENT: KEY DOWN UNTUK LIST SUPPLIER
    ' ============================================
    Private Sub ListSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles listSupplier.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Enter untuk pilih supplier
            PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
        ElseIf e.KeyCode = Keys.Down Then
            ' Jika sudah di item terakhir, tetap di item terakhir
            If listSupplier.SelectedIndex = listSupplier.Items.Count - 1 Then
                listSupplier.SelectedIndex = listSupplier.Items.Count - 1
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Up Then
            ' Jika sudah di item pertama, kembali ke TxtSupplier
            If listSupplier.SelectedIndex = 0 Then
                TxtSupplier.Focus()
                TxtSupplier.SelectionStart = TxtSupplier.Text.Length
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            ' ESC untuk sembunyikan list dan kembali ke TxtSupplier
            listSupplier.Visible = False
            TxtSupplier.Focus()
            TxtSupplier.SelectionStart = TxtSupplier.Text.Length
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ============================================
    ' EVENT: GOT FOCUS UNTUK LIST SUPPLIER
    ' ============================================
    Private Sub ListSupplier_GotFocus(sender As Object, e As EventArgs) Handles listSupplier.GotFocus
        ' Pastikan ada item yang terpilih saat mendapat fokus
        If listSupplier.Items.Count > 0 AndAlso listSupplier.SelectedIndex = -1 Then
            listSupplier.SelectedIndex = 0
        End If
    End Sub


    ' ============================================
    ' EVENT: CLICK UNTUK LIST SUPPLIER
    ' ============================================
    Private Sub ListSupplier_Click(sender As Object, e As EventArgs) Handles listSupplier.Click
        PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
    End Sub

#End Region

    ' ============================================
    ' OPERASI CRUD DATAGRIDVIEW
    ' ============================================
#Region "CRUD Operations"

    ' ============================================
    ' FUNGSI: AMBIL DATA DARI LISTBOX BARANG
    ' ============================================
    Private Sub AmbilDataDariListBox()
        If LstBarang.SelectedIndex < 0 Then Return

        ' Extract nama barang dari format string "Nama | T: x | G: y" atau "Nama [Stok: x]"
        Dim selectedValue As String = LstBarang.Items(LstBarang.SelectedIndex).ToString()
        Dim namaYangDiambil As String = selectedValue
        If selectedValue.Contains("|") Then
            namaYangDiambil = selectedValue.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)(0).Trim()
        ElseIf selectedValue.Contains(" [Stok:") Then
            namaYangDiambil = selectedValue.Substring(0, selectedValue.IndexOf(" [Stok:")).Trim()
        End If

        ' Tutup ListBox sebelum proses
        LstBarang.Visible = False
        LstBarang.Items.Clear()

        ' Guard: aktifkan flag agar CellEndEdit tidak terpicu
        _sedangSetNilaiDariListBox = True

        Try
            If _konteksLstBarang = "TXTNAMA" Then
                ' Cek stok sebelum melanjutkan
                Dim barang = barangCacheById.Values.FirstOrDefault(Function(b) b.NAMA_BARANG = namaYangDiambil)

                If barang IsNot Nothing AndAlso Not ModulHakAkses.SettingIzinkanBarangMinus Then
                    Dim stokTersedia = barang.GetStokByLokasi(LblLokasiBarang.Text)
                    If TxtNama.Text.Contains("*") Then
                        Dim parts = TxtNama.Text.Split("*"c)
                        Dim qty As Decimal = 1
                        If parts.Length >= 1 AndAlso Decimal.TryParse(parts(0).Trim(), qty) Then
                            ' Point 4: Perhitungkan multiplier berdasarkan Level (Satuan)
                            Dim currentLevel As Integer = ModuleAngka.ParseInteger(TxtLevel.Text)
                            Dim multiplier As Integer = 1
                            Select Case currentLevel
                                Case 2 : multiplier = barang.ISI_UMUM_SEDANG
                                Case 3 : multiplier = barang.ISI_UMUM_BESAR
                                Case Else : multiplier = barang.ISI_UMUM_KECIL
                            End Select

                            Dim qtySat = qty * multiplier
                            If qtySat > stokTersedia Then
                                MessageBox.Show($"Stok tidak mencukupi!{vbCrLf}" &
                                          $"Stok tersedia: {stokTersedia:N0}{vbCrLf}" &
                                          $"Qty yang diminta: {qtySat:N0}",
                                          "Peringatan Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Return
                            End If
                        End If
                    End If
                End If

                Ambildatalaindaridbbarang(namaYangDiambil)
                TambahDataLangsung(namaYangDiambil)

            ElseIf _konteksLstBarang = "DGV" Then
                If DgvData.CurrentCell IsNot Nothing Then
                    Dim rowIdx As Integer = _rowSaatPindahKeLst
                    If rowIdx < 0 OrElse rowIdx >= DgvData.Rows.Count Then
                        rowIdx = DgvData.CurrentCell.RowIndex
                    End If

                    ' Cari baris dengan ID_BARANG kosong pertama — target yang benar
                    Dim barisDiisi As Integer = rowIdx
                    For i As Integer = 0 To DgvData.Rows.Count - 1
                        If Not DgvData.Rows(i).IsNewRow Then
                            Dim kodeVal = Convert.ToString(DgvData.Rows(i).Cells("ID_BARANG").Value).Trim()
                            If String.IsNullOrEmpty(kodeVal) Then
                                barisDiisi = i
                                Exit For
                            End If
                        End If
                    Next

                    DgvData.EndEdit(True)
                    DgvData.CurrentCell = Nothing

                    Dim barangTarget As DataBarang = barangCacheById.Values.FirstOrDefault(
                        Function(b) b.NAMA_BARANG = namaYangDiambil)

                    If barangTarget IsNot Nothing Then
                        Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
                        If qtyValue <= 0 Then qtyValue = 1D
                        UpdateGridRowFromBarang(barisDiisi, barangTarget,
                                               barangTarget.SATUAN_UMUM_KECIL,
                                               barangTarget.ISI_UMUM_KECIL, qtyValue)
                    End If
                End If
            End If

        Finally
            _sedangSetNilaiDariListBox = False
        End Try

        ' Kembalikan fokus
        If _konteksLstBarang = "TXTNAMA" Then
            KosongTxtboxcari()
            SetupFocusToGrid()
        Else
            KosongTxtboxcari()
            SetupFocusToGrid()
        End If
    End Sub

    ' ============================================
    ' EVENT: LISTBOX — HANYA TRACKING, PILIH VIA CLICK ATAU ENTER
    ' ============================================
    Private Sub LstBarang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LstBarang.SelectedIndexChanged
    End Sub

    Private Sub LstBarang_Click(sender As Object, e As EventArgs) Handles LstBarang.Click
        If _listBoxBaruDapatFokus Then
            _listBoxBaruDapatFokus = False
            Return
        End If
        If LstBarang.SelectedIndex >= 0 Then
            _sedangPindahKeLstBarang = True
            AmbilDataDariListBox()
            _sedangPindahKeLstBarang = False
        End If
    End Sub

    ' ============================================
    ' FUNGSI: GET TEXT SETELAH ASTERISK
    ' ============================================
    Private Function GetTextAfterAsterisk(ByVal selectedValue As String) As String
        ' Bersihkan dulu dari format stok jika ada
        Dim cleanValue = ListBarangItem.ExtractNamaBarang(selectedValue)

        Dim indexAsterisk As Integer = cleanValue.IndexOf("*")

        If indexAsterisk >= 0 Then
            Return cleanValue.Substring(indexAsterisk + 1).Trim()
        Else
            Return cleanValue
        End If
    End Function



    ' ============================================
    ' FUNGSI: AMBIL DATA LAIN DARI DATABASE BARANG
    ' ============================================
    Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
        ' Coba dulu dari cache
        Dim barangFromCache = barangCacheById.Values.FirstOrDefault(Function(b) b.NAMA_BARANG = namayangdiambil)

        If barangFromCache IsNot Nothing Then
            ' Set data dari cache
            TxtKode.Text = barangFromCache.ID_BARANG
            TxtHarga.Text = barangFromCache.HARGA_BELI_TERAKHIR.ToString("N0")
            Txtsatuan.Text = barangFromCache.SATUAN_UMUM_KECIL
            TxtIsi.Text = barangFromCache.ISI_UMUM_KECIL.ToString()
            Return
        End If

        ' Fallback ke database jika tidak ada di cache
        Try
            Dim queryAmbilData As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI_TERAKHIR, " &
                                         "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                                         "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                                         "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                                         "STOK_TOKO, STOK_GUDANG " &
                                         "FROM tbl_barang WHERE NAMA_BARANG = @NAMA"

            Using cmd As New MySqlCommand(queryAmbilData, conn)
                cmd.Parameters.AddWithValue("@NAMA", namayangdiambil)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        Dim idBarang As String = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                        Dim hargaBeli As String = ModuleAngka.ParseDecimal(rd("HARGA_BELI_TERAKHIR")).ToString("0.##", Globalization.CultureInfo.InvariantCulture)

                        Dim satuanUmum As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", String.Empty)
                        Dim isiUmum As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))

                        ' Jika ada barcode, tentukan satuan berdasarkan barcode
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

                        ' Set data ke textbox
                        TxtKode.Text = idBarang
                        TxtHarga.Text = hargaBeli
                        Txtsatuan.Text = satuanUmum
                        TxtIsi.Text = isiUmum.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error mengambil data barang: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: TAMBAH DATA LANGSUNG KE GRID
    ' ============================================
    Private Sub TambahDataLangsung(ByVal namayangdiambil As String)

        Try
            ' Validasi duplikasi barang
            If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                For Each row As DataGridViewRow In DgvData.Rows
                    If row.Cells("ID_BARANG").Value IsNot Nothing AndAlso
                       row.Cells("ID_BARANG").Value.ToString() = TxtKode.Text Then
                        MessageBox.Show(namayangdiambil & " sudah ada dalam daftar!", "Peringatan",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        LstBarang.Select()
                        Exit Sub
                    End If
                Next
            End If

            ' Cari barang dari cache
            Dim barangFromCache = barangCacheById.Values.FirstOrDefault(Function(b) b.ID_BARANG = TxtKode.Text)

            ' Jika tidak ada di cache, cari dari database
            If barangFromCache Is Nothing Then
                barangFromCache = GetBarangFromCache(TxtKode.Text)
                If barangFromCache Is Nothing Then
                    MessageBox.Show("Data barang tidak ditemukan di cache", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If

            ' Tambah baris baru
            Dim indeksBaris As Integer
            If DgvData.SelectedCells.Count > 0 Then
                indeksBaris = DgvData.SelectedCells(0).RowIndex
                DgvData.Rows.Insert(indeksBaris, "")
            Else
                indeksBaris = DgvData.Rows.Add()
            End If

            ' Isi data satuan
            Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(indeksBaris).Cells("SATUAN"), DataGridViewComboBoxCell)
            kolomSatuan.Items.Clear()

            ' Tambahkan satuan yang tersedia
            If Not String.IsNullOrEmpty(barangFromCache.SATUAN_UMUM_KECIL) Then
                kolomSatuan.Items.Add(barangFromCache.SATUAN_UMUM_KECIL)
            End If
            If Not String.IsNullOrEmpty(barangFromCache.SATUAN_UMUM_SEDANG) Then
                kolomSatuan.Items.Add(barangFromCache.SATUAN_UMUM_SEDANG)
            End If
            If Not String.IsNullOrEmpty(barangFromCache.SATUAN_UMUM_BESAR) Then
                kolomSatuan.Items.Add(barangFromCache.SATUAN_UMUM_BESAR)
            End If

            ' Set nilai baris
            Dim kode As String = TxtKode.Text
            Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHarga.Text)
            Dim qty As Decimal = If(ModuleAngka.ParseDecimal(TxtQty.Text) > 0, ModuleAngka.ParseDecimal(TxtQty.Text), 1D)
            Dim satuan As String = Txtsatuan.Text
            Dim isi As Decimal = Math.Max(1, ModuleAngka.ParseDecimal(TxtIsi.Text))

            ' Set nilai ke grid
            DgvData.Rows(indeksBaris).Cells("ID_BARANG").Value = kode
            DgvData.Rows(indeksBaris).Cells("NAMA_BARANG").Value = namayangdiambil
            DgvData.Rows(indeksBaris).Cells("HARGA_BELI_TERAKHIR").Value = hargaBeli
            DgvData.Rows(indeksBaris).Cells("QTY").Value = qty
            DgvData.Rows(indeksBaris).Cells("SATUAN").Value = satuan
            DgvData.Rows(indeksBaris).Cells("ISI_SATUAN").Value = isi
            DgvData.Rows(indeksBaris).Cells("HARGA_BELI_SATUAN").Value = hargaBeli * isi
            DgvData.Rows(indeksBaris).Cells("QTY_SAT").Value = qty * isi
            DgvData.Rows(indeksBaris).Cells("TOTAL").Value = (hargaBeli * isi) * (qty * isi)
            DgvData.Rows(indeksBaris).Cells("StokToko").Value = barangFromCache.STOK_TOKO
            DgvData.Rows(indeksBaris).Cells("StokGudang").Value = barangFromCache.STOK_GUDANG

            UpdateSemuaTotal()
            KosongTxtboxcari()

        Catch ex As Exception
            MessageBox.Show($"Error menambah data barang: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: SETUP FOKUS KE GRID (GANTI Fokuskepencarianbarang)
    ' Dipanggil dari: PilihSupplierLangsung, TxtSupplier_KeyDown, TambahDataLangsung
    ' ============================================
    Public Sub SetupFocusToGrid()
        ' Guard: jika form tidak aktif atau belum siap, jangan paksa fokus
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

        ' Cari baris terakhir yang terisi (ada ID_BARANG)
        For i As Integer = DgvData.Rows.Count - 1 To 0 Step -1
            If Not DgvData.Rows(i).IsNewRow Then
                Dim kodeVal = Convert.ToString(DgvData.Rows(i).Cells("ID_BARANG").Value).Trim()
                If Not String.IsNullOrEmpty(kodeVal) Then
                    lastFilledRow = i
                    Exit For
                End If
            End If
        Next

        If lastFilledRow >= 0 Then
            Dim foundEmptyRow As Boolean = False
            ' Cari baris kosong setelah baris terakhir yang terisi
            For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    Dim kodeVal = Convert.ToString(DgvData.Rows(i).Cells("ID_BARANG").Value).Trim()
                    If String.IsNullOrEmpty(kodeVal) Then
                        targetRow = i
                        foundEmptyRow = True
                        Exit For
                    End If
                End If
            Next

            If Not foundEmptyRow Then
                ' Cari IsNewRow — JANGAN Rows.Add() yang menyebabkan baris ekstra
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

        If targetRow < DgvData.Rows.Count Then
            Dim targetColumnIndex As Integer = 1 ' Kolom NAMA_BARANG
            Dim targetRowIndex As Integer = targetRow

            DgvData.CurrentCell = DgvData(targetColumnIndex, targetRowIndex)
            Me.ActiveControl = DgvData

            ' Race-condition guard: pastikan CurrentCell belum berubah sebelum BeginEdit
            DgvData.BeginInvoke(New Action(Sub()
                                               If DgvData.CurrentCell IsNot Nothing AndAlso
                                                  DgvData.CurrentCell.ColumnIndex = targetColumnIndex AndAlso
                                                  DgvData.CurrentCell.RowIndex = targetRowIndex Then
                                                   DgvData.BeginEdit(True)
                                                   If DgvData.EditingControl IsNot Nothing Then DgvData.EditingControl.Focus()
                                               End If
                                           End Sub))
        End If
    End Sub

    ' ============================================
    ' FUNGSI: FOKUS KE PENCARIAN BARANG (DIPERTAHANKAN UNTUK KOMPATIBILITAS)
    ' ============================================
    Private Sub Fokuskepencarianbarang()
        SetupFocusToGrid()
    End Sub

    ' ============================================
    ' FUNGSI: HAPUS BARIS DARI GRID
    ' ============================================
    Private Sub Hapusbaris()
        Dim baris As Integer = DgvData.CurrentCell.RowIndex
        DgvData.Rows.RemoveAt(baris)
        UpdateSemuaTotal()
        SetupFocusToGrid()
    End Sub

    ' ============================================
    ' EVENT: CLICK MENU HAPUS TOOLSTRIP
    ' ============================================
    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
    End Sub

    Private Sub RefreshStokBarisIniToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RefreshStokBarisIniToolStripMenuItem.Click
        If DgvData.CurrentCell IsNot Nothing AndAlso DgvData.CurrentCell.RowIndex >= 0 Then
            RefreshStokBaris(DgvData.CurrentCell.RowIndex)
        End If
    End Sub

    Private Sub RefreshStokSemuaBarisToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RefreshStokSemuaBarisToolStripMenuItem.Click
        RefreshStokSemuaBaris()
    End Sub

    ' ============================================
    ' FUNGSI: UPDATE LOKASI DISPLAY
    ' ============================================
    Private Sub UpdateLokasiDisplay()
        ' Tambahkan informasi lokasi di judul atau label
        Dim lokasi = LblLokasiBarang.Text
        Dim warna As Color

        Select Case lokasi.ToUpper()
            Case "TOKO"
                warna = Color.LightBlue
            Case "GUDANG"
                warna = Color.LightGreen
            Case Else
                warna = Color.LightGray
        End Select

        ' Update warna background atau text
        LblLokasiBarang.BackColor = warna
        LblLokasiBarang.ForeColor = Color.Black
        LblLokasiBarang.Font = New Font(LblLokasiBarang.Font, FontStyle.Bold)

        ' Update judul form
        Me.Text = $"Retur Beli - {lokasi}"
    End Sub

#End Region

    ' ============================================
    ' MANAJEMEN DATAGRIDVIEW
    ' ============================================
#Region "DataGridView Management"

    ' ============================================
    ' EVENT: CELL END EDIT DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        Dim nilaiSel As String = If(DgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value IsNot Nothing, DgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString(), "")
        Try
            If _sedangSetNilaiDariListBox Then
                Return
            End If

            If e.RowIndex < 0 Or e.ColumnIndex < 0 Then Exit Sub

            ' Proses berdasarkan kolom yang diedit
            Select Case DgvData.Columns(e.ColumnIndex).Name
                Case "NAMA_BARANG"
                    ProsesEditNamaBarang(e.RowIndex)

                Case "HARGA_BELI_TERAKHIR"
                    ProsesEditHargaBeli(e.RowIndex)

                Case "QTY"
                    ProsesEditQty(e.RowIndex)
            End Select

            ' Update total - LANGSUNG saja, tidak perlu timer
            UpdateSemuaTotal()

        Catch ex As Exception
        End Try
    End Sub

    '' ============================================
    '' EVENT: LEAVE DATAGRIDVIEW
    '' ============================================
    'Private Sub DgvData_Leave(sender As Object, e As EventArgs) Handles DgvData.Leave
    '    ' Sembunyikan ListBarang saat DataGridView kehilangan fokus
    '    LstBarang.Visible = False
    'End Sub


    ' ============================================
    ' EVENT: CELL DOUBLE CLICK DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_CellDoubleClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvData.CellDoubleClick
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            If DgvData.Columns(e.ColumnIndex).Name = "NAMA_BARANG" Then
                ' Mulai edit mode di sel yang di-double click
                DgvData.CurrentCell = DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG")
                DgvData.BeginEdit(True)

                ' Set fokus dan select all text
                If TypeOf DgvData.EditingControl Is TextBox Then
                    Dim textBox = DirectCast(DgvData.EditingControl, TextBox)
                    textBox.SelectAll()
                End If
            ElseIf DgvData.Columns(e.ColumnIndex).Name = "QTY" Then
                ' Select all text di QTY untuk edit cepat
                DgvData.BeginEdit(True)
                If TypeOf DgvData.EditingControl Is TextBox Then
                    Dim textBox = DirectCast(DgvData.EditingControl, TextBox)
                    textBox.SelectAll()
                End If
            ElseIf DgvData.Columns(e.ColumnIndex).Name = "HARGA_BELI_TERAKHIR" Then
                ' Select all text di HARGA_BELI_TERAKHIR untuk edit cepat
                DgvData.BeginEdit(True)
                If TypeOf DgvData.EditingControl Is TextBox Then
                    Dim textBox = DirectCast(DgvData.EditingControl, TextBox)
                    textBox.SelectAll()
                End If
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PROSES EDIT NAMA BARANG DI GRID
    ' ============================================
    Private Sub ProsesEditNamaBarang(rowIndex As Integer)
        ' KRITIS: jika ListBox masih visible, user sedang memilih dari list
        ' jangan proses CellEndEdit — AmbilDataDariListBox yang akan handle
        If LstBarang.Visible OrElse _sedangSetNilaiDariListBox Then
            Return
        End If
        Try
            Dim cellValue As Object = DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value
            If cellValue Is Nothing OrElse String.IsNullOrEmpty(cellValue.ToString().Trim()) Then
                ClearGridRow(rowIndex)
                Return
            End If

            Dim inputValue As String = cellValue.ToString().Trim()

            ' PARSE FORMAT "QTY*LEVEL*NAMA" atau "QTY*NAMA" atau "BARCODE"
            Dim parts = inputValue.Split("*"c)
            Dim qty As Decimal = 1
            Dim level As Integer = 1
            Dim searchKey As String = inputValue

            If parts.Length >= 3 Then
                ' Pola: qty * level * nama
                If Decimal.TryParse(parts(0).Trim(), qty) AndAlso qty > 0 AndAlso
                   Integer.TryParse(parts(1).Trim(), level) AndAlso level >= 1 AndAlso level <= 3 Then
                    searchKey = parts(2).Trim()
                    TxtQty.Text = qty.ToString()
                    TxtLevel.Text = level.ToString()
                End If
            ElseIf parts.Length = 2 Then
                ' Pola: qty * nama
                If Decimal.TryParse(parts(0).Trim(), qty) AndAlso qty > 0 Then
                    searchKey = parts(1).Trim()
                    TxtQty.Text = qty.ToString()
                End If
            End If

            ' CARI BARANG DARI CACHE - LEBIH CEPAT
            Dim barangFromCache As DataBarang = Nothing

            ' 1. Cek apakah barcode
            If barcodeLookupCache.ContainsKey(searchKey) Then
                Dim idBarang = barcodeLookupCache(searchKey)
                barangFromCache = GetBarangFromCache(idBarang)
            Else
                ' 2. Cari dengan exact match di cache (O(1) dengan dictionary tambahan)
                ' Kita perlu buat dictionary untuk lookup cepat berdasarkan nama
                ' Untuk sementara, gunakan LINQ yang dioptimalkan
                barangFromCache = barangCacheById.Values.
                FirstOrDefault(Function(b) String.Equals(b.NAMA_BARANG, searchKey, StringComparison.OrdinalIgnoreCase))

                ' 3. Jika tidak ditemukan, cari dengan contains
                ' 3. Jika tidak ditemukan, cari dengan contains
                barangFromCache = If(barangFromCache, barangCacheById.Values.
                    FirstOrDefault(Function(b) b.NAMA_BARANG.ToLower().Contains(searchKey.ToLower())))
            End If

            ' JIKA TIDAK DITEMUKAN
            If barangFromCache Is Nothing Then
                MessageBox.Show($"Barang '{searchKey}' tidak ditemukan", "Peringatan",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning)

                ' Fokus kembali untuk edit ulang
                DgvData.CurrentCell = DgvData.Rows(rowIndex).Cells("NAMA_BARANG")
                DgvData.BeginEdit(True)
                Return
            End If

            ' TENTUKAN SATUAN BERDASARKAN LEVEL ATAU BARCODE
            Dim satuan As String = ""
            Dim isi As Integer = 1

            If parts.Length >= 3 Then
                ' Gunakan level yang diparsing dari pola qty*level*nama
                Select Case level
                    Case 2
                        satuan = barangFromCache.SATUAN_UMUM_SEDANG
                        isi = barangFromCache.ISI_UMUM_SEDANG
                    Case 3
                        satuan = barangFromCache.SATUAN_UMUM_BESAR
                        isi = barangFromCache.ISI_UMUM_BESAR
                    Case Else
                        satuan = barangFromCache.SATUAN_UMUM_KECIL
                        isi = barangFromCache.ISI_UMUM_KECIL
                End Select
            ElseIf barcodeLookupCache.ContainsKey(searchKey) AndAlso barcodeToSatuanCache.ContainsKey(searchKey) Then
                ' Jika input adalah barcode, ambil satuan dari cache barcode
                satuan = barcodeToSatuanCache(searchKey).Item1
                isi = barcodeToSatuanCache(searchKey).Item2
            Else
                ' Default ke satuan kecil
                satuan = barangFromCache.SATUAN_UMUM_KECIL
                isi = barangFromCache.ISI_UMUM_KECIL
            End If

            ' Fallback jika satuan pada level yang dipilih kosong
            If String.IsNullOrEmpty(satuan) Then
                satuan = barangFromCache.SATUAN_UMUM_KECIL
                isi = barangFromCache.ISI_UMUM_KECIL
            End If

            ' Jika isi = 0, set ke 1
            isi = Math.Max(1, isi)

            ' UPDATE BARIS DI DATAGRIDVIEW
            UpdateGridRowFromBarang(rowIndex, barangFromCache, satuan, isi, qty)


        Catch ex As Exception
            MessageBox.Show($"Error memproses nama barang: {ex.Message}", "Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================'
    ' FUNGSI: UPDATE GRID ROW FROM BARANG
    ' ============================================'
    Private Sub UpdateGridRowFromBarang(rowIndex As Integer, barang As DataBarang, satuan As String, isi As Integer, qty As Decimal)

        DgvData.Rows(rowIndex).Cells("ID_BARANG").Value = barang.ID_BARANG
        DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value = barang.NAMA_BARANG
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = barang.HARGA_BELI_TERAKHIR
        DgvData.Rows(rowIndex).Cells("QTY").Value = qty

        ' Setup ComboBox satuan
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(
        DgvData.Rows(rowIndex).Cells("SATUAN"),
        DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()

        ' Tambahkan satuan yang tersedia
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_KECIL) Then
            kolomSatuan.Items.Add(barang.SATUAN_UMUM_KECIL)
        End If
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_SEDANG) Then
            kolomSatuan.Items.Add(barang.SATUAN_UMUM_SEDANG)
        End If
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_BESAR) Then
            kolomSatuan.Items.Add(barang.SATUAN_UMUM_BESAR)
        End If

        ' PASTIKAN SATUAN YANG DIPILIH ADA DALAM COMBOBOX
        ' Jika satuan yang dipilih tidak ada dalam daftar, tambahkan
        Dim satuanExists As Boolean = False
        For Each item As Object In kolomSatuan.Items
            If item.ToString() = satuan Then
                satuanExists = True
                Exit For
            End If
        Next

        If Not satuanExists AndAlso Not String.IsNullOrEmpty(satuan) Then
            kolomSatuan.Items.Add(satuan)
        End If

        ' Set nilai ke grid
        DgvData.Rows(rowIndex).Cells("SATUAN").Value = satuan
        DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = isi
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = barang.HARGA_BELI_TERAKHIR * isi
        DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = qty * isi
        DgvData.Rows(rowIndex).Cells("TOTAL").Value = barang.HARGA_BELI_TERAKHIR * (qty * isi)
        DgvData.Rows(rowIndex).Cells("StokToko").Value = barang.STOK_TOKO
        DgvData.Rows(rowIndex).Cells("StokGudang").Value = barang.STOK_GUDANG

        ' Format tampilan
        ModuleAngka.TerapkanFormatKolomAngka(DgvData, "HARGA_BELI_TERAKHIR", "TOTAL")

        ' Tampilkan info stok
        TampilkanInfoStokDiGrid(rowIndex, barang)

        ' Update total
        UpdateSemuaTotal()
        SetWarnaReadOnlyNama(rowIndex)
    End Sub
    ' ============================================
    ' FUNGSI: TAMPILKAN INFO STOK DI GRID
    ' ============================================
    Private Sub TampilkanInfoStokDiGrid(rowIndex As Integer, barang As DataBarang)
        ' Tampilkan info stok di tooltip atau status bar jika diperlukan
        Dim stok = barang.GetStokByLokasi(LblLokasiBarang.Text)
        Dim infoStok = $"Stok {LblLokasiBarang.Text}: {stok:N0}"

        ' Tampilkan di tooltip cell
        DgvData.Rows(rowIndex).Cells("NAMA_BARANG").ToolTipText = infoStok
    End Sub

    ' LstBarang_DrawItem dihapus — tidak relevan untuk ListView (hanya untuk ListBox)

    ' ═══════════════════════════════════════════════════════════════════
    ' PENCARIAN INLINE DARI KOLOM NAMA_BARANG DI DGV
    ' ═══════════════════════════════════════════════════════════════════

    ''' <summary>
    ''' Dipanggil saat user mengetik di kolom NAMA_BARANG di DGV.
    ''' Parse format qty*level*nama, feed ke barcode buffer, posisikan ListView.
    ''' </summary>
    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        ' Guard sama seperti FormJual/FormPembelian
        If _sedangSetNilaiDariListBox Then Return

        Dim tb As TextBox = TryCast(sender, TextBox)
        If tb Is Nothing Then Return

        Dim currentText As String = tb.Text.Trim()
        If String.IsNullOrEmpty(currentText) Then
            If _sedangPindahKeLstBarang OrElse LstBarang.Focused OrElse LstBarang.Visible Then
                Return
            End If
            LstBarang.Visible = False
            Return
        End If

        ' Feed karakter ke barcodeChars — pakai barcodeTimer yang sama dengan jalur TxtNama
        ' Jika input cepat (scanner) → barcodeTimer_Tick akan proses sebagai barcode
        ' Jika input lambat (ketik manual) → interval > BARCODE_CHAR_INTERVAL_MS → isBarcodeMode=False → manual search
        Dim currentTime = DateTime.Now
        If barcodeChars.Count = 0 Then
            barcodeStartTime = currentTime
        Else
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then isBarcodeMode = False
        End If
        ' Tambah semua karakter baru ke buffer (TextChanged bisa bawa >1 karakter saat paste/scan)
        For Each ch As Char In currentText
            If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(ch)
        Next
        lastKeyTime = currentTime
        BarcodeTimer.Stop()
        BarcodeTimer.Start()

        ' Parse format qty*level*nama
        Dim keyword As String = currentText
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            Dim qty As Decimal = 0
            If parts.Length >= 2 AndAlso Decimal.TryParse(parts(0).Trim(), qty) AndAlso qty > 0 Then
                TxtQty.Text = qty.ToString()
            End If
            If parts.Length >= 3 Then
                Dim lvl As Integer = 0
                If Integer.TryParse(parts(1).Trim(), lvl) AndAlso lvl >= 1 AndAlso lvl <= 3 Then
                    TxtLevel.Text = lvl.ToString()
                End If
                keyword = parts(parts.Length - 1).Trim()
            Else
                keyword = parts(parts.Length - 1).Trim()
            End If
        End If

        If keyword.Length < 2 Then
            LstBarang.Visible = False
            Return
        End If

        ' Set konteks DGV dan simpan baris saat ini
        _konteksLstBarang = "DGV"
        If DgvData.CurrentCell IsNot Nothing Then
            _rowSaatPindahKeLst = DgvData.CurrentCell.RowIndex
        End If
        ' Gunakan debounce — tunda query sampai user berhenti ketik
        ' Sama seperti FormJual yang pakai _searchTimer untuk mencegah CellEndEdit spurious
        lastSearchText = keyword
        lastSearchKonteks = "DGV"
        SearchDebounceTimer.Stop()
        SearchDebounceTimer.Start()
    End Sub

    ' ============================================
    ' FUNGSI: CLEAR GRID ROW
    ' ============================================
    Private Sub ClearGridRow(rowIndex As Integer)
        ' Kosongkan semua sel di baris
        DgvData.Rows(rowIndex).Cells("ID_BARANG").Value = ""
        DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value = ""
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = 0
        DgvData.Rows(rowIndex).Cells("QTY").Value = 1
        DgvData.Rows(rowIndex).Cells("SATUAN").Value = ""
        DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = 0
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = 0
        DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = 0
        DgvData.Rows(rowIndex).Cells("TOTAL").Value = 0
    End Sub

    ' ============================================
    ' FUNGSI: CARI BARANG DI DATABASE
    ' ============================================
    Private Function CariBarangDiDatabase(searchValue As String) As DataBarang
        Try
            Dim query = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI_TERAKHIR, " &
                   "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                   "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                   "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                   "STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                   "WHERE NAMA_BARANG LIKE @search OR BARCODE_KECIL = @search " &
                   "OR BARCODE_SEDANG = @search OR BARCODE_BESAR = @search " &
                   "OR ID_BARANG = @search LIMIT 1"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@search", searchValue)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim idBarang As String = rd("ID_BARANG").ToString()
                        Dim barang As New DataBarang With {
                            .ID_BARANG = idBarang,
                            .NAMA_BARANG = rd("NAMA_BARANG").ToString(),
                            .HARGA_BELI_TERAKHIR = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI_TERAKHIR", 0D),
                            .SATUAN_UMUM_KECIL = rd("SATUAN_UMUM_KECIL").ToString(),
                            .SATUAN_UMUM_SEDANG = rd("SATUAN_UMUM_SEDANG").ToString(),
                            .SATUAN_UMUM_BESAR = rd("SATUAN_UMUM_BESAR").ToString(),
                            .ISI_UMUM_KECIL = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)),
                            .ISI_UMUM_SEDANG = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1)),
                            .ISI_UMUM_BESAR = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1)),
                            .BARCODE_KECIL = rd("BARCODE_KECIL").ToString(),
                            .BARCODE_SEDANG = rd("BARCODE_SEDANG").ToString(),
                            .BARCODE_BESAR = rd("BARCODE_BESAR").ToString(),
                            .STOK_TOKO = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                            .STOK_GUDANG = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                        }

                        ' Update cache dengan item baru
                        If Not barangCacheById.ContainsKey(idBarang) Then
                            barangCacheById(idBarang) = barang

                            ' Update barcode cache
                            If Not String.IsNullOrEmpty(barang.BARCODE_KECIL) Then
                                barcodeLookupCache(barang.BARCODE_KECIL) = idBarang
                                barcodeToSatuanCache(barang.BARCODE_KECIL) = Tuple.Create(
                                    barang.SATUAN_UMUM_KECIL, barang.ISI_UMUM_KECIL)
                            End If

                            If Not String.IsNullOrEmpty(barang.BARCODE_SEDANG) Then
                                barcodeLookupCache(barang.BARCODE_SEDANG) = idBarang
                                barcodeToSatuanCache(barang.BARCODE_SEDANG) = Tuple.Create(
                                    barang.SATUAN_UMUM_SEDANG, barang.ISI_UMUM_SEDANG)
                            End If

                            If Not String.IsNullOrEmpty(barang.BARCODE_BESAR) Then
                                barcodeLookupCache(barang.BARCODE_BESAR) = idBarang
                                barcodeToSatuanCache(barang.BARCODE_BESAR) = Tuple.Create(
                                    barang.SATUAN_UMUM_BESAR, barang.ISI_UMUM_BESAR)
                            End If
                        End If

                        Return barang
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try

        Return Nothing
    End Function

    ' ============================================'
    ' FUNGSI: UPDATE BARIS DARI CACHE
    ' ============================================'
    Private Sub UpdateBarisDariCache(rowIndex As Integer, barang As DataBarang, inputValue As String, isBarcodeInput As Boolean)

        DgvData.Rows(rowIndex).Cells("ID_BARANG").Value = barang.ID_BARANG
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = barang.HARGA_BELI_TERAKHIR

        ' Setup ComboBox satuan
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(rowIndex).Cells("SATUAN"), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()

        ' Tambahkan satuan yang tersedia
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_KECIL) Then kolomSatuan.Items.Add(barang.SATUAN_UMUM_KECIL)
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_SEDANG) Then kolomSatuan.Items.Add(barang.SATUAN_UMUM_SEDANG)
        If Not String.IsNullOrEmpty(barang.SATUAN_UMUM_BESAR) Then kolomSatuan.Items.Add(barang.SATUAN_UMUM_BESAR)

        ' Tentukan satuan dan isi berdasarkan input
        Dim satuan As String = barang.SATUAN_UMUM_KECIL
        Dim isi As Integer = barang.ISI_UMUM_KECIL

        If isBarcodeInput Then
            ' Jika input adalah barcode, cari satuan yang sesuai
            If inputValue = barang.BARCODE_KECIL Then
                satuan = barang.SATUAN_UMUM_KECIL
                isi = barang.ISI_UMUM_KECIL
            ElseIf inputValue = barang.BARCODE_SEDANG Then
                satuan = barang.SATUAN_UMUM_SEDANG
                isi = barang.ISI_UMUM_SEDANG
            ElseIf inputValue = barang.BARCODE_BESAR Then
                satuan = barang.SATUAN_UMUM_BESAR
                isi = barang.ISI_UMUM_BESAR
            End If
        End If

        ' PASTIKAN SATUAN YANG DIPILIH ADA DALAM COMBOBOX
        ' Jika satuan yang dipilih tidak ada dalam daftar, tambahkan
        Dim satuanExists As Boolean = False
        For Each item As Object In kolomSatuan.Items
            If item.ToString() = satuan Then
                satuanExists = True
                Exit For
            End If
        Next

        If Not satuanExists AndAlso Not String.IsNullOrEmpty(satuan) Then
            kolomSatuan.Items.Add(satuan)
        End If

        ' Set nilai default
        isi = Math.Max(1, isi)

        ' Set nilai ke grid
        DgvData.Rows(rowIndex).Cells("SATUAN").Value = satuan
        DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = isi
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = barang.HARGA_BELI_TERAKHIR * isi
        DgvData.Rows(rowIndex).Cells("QTY").Value = 1 ' Default QTY
        DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = 1 * isi
        DgvData.Rows(rowIndex).Cells("TOTAL").Value = barang.HARGA_BELI_TERAKHIR * (1 * isi)

        ' Format harga
        ModuleAngka.TerapkanFormatKolomAngka(DgvData, "HARGA_BELI_TERAKHIR", "TOTAL")

        ' Cek duplikasi jika setting tidak mengizinkan
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
            ProsesMergeBarisDuplikat(rowIndex)
        End If

        ' Update total setelah perubahan
        UpdateSemuaTotal()
        SetWarnaReadOnlyNama(rowIndex)
    End Sub

    ' ============================================
    ' FUNGSI: UPDATE BARIS DARI DATABASE
    ' ============================================
    Private Sub UpdateBarisDariDatabase(rowIndex As Integer, rd As MySqlDataReader, namaValue As String)

        DgvData.Rows(rowIndex).Cells("ID_BARANG").Value = rd("ID_BARANG").ToString()
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_TERAKHIR"))

        ' Setup ComboBox satuan
        Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(rowIndex).Cells("SATUAN"), DataGridViewComboBoxCell)
        comboCell.Items.Clear()

        ' Ambil data satuan dari reader
        Dim satuanKecil As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
        Dim satuanSedang As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
        Dim satuanBesar As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")

        ' Tambahkan satuan yang tersedia
        If Not String.IsNullOrEmpty(satuanKecil) Then comboCell.Items.Add(satuanKecil)
        If Not String.IsNullOrEmpty(satuanSedang) Then comboCell.Items.Add(satuanSedang)
        If Not String.IsNullOrEmpty(satuanBesar) Then comboCell.Items.Add(satuanBesar)

        ' Tentukan satuan dan isi berdasarkan input
        Dim satuan As String = ""
        Dim isi As Integer = 1

        If namaValue = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_BARANG", "") Or
           namaValue = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_KECIL", "") Then
            satuan = rd("SATUAN_UMUM_KECIL")
            isi = rd("ISI_UMUM_KECIL")
        ElseIf namaValue = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_SEDANG", "") Then
            satuan = rd("SATUAN_UMUM_SEDANG")
            isi = rd("ISI_UMUM_SEDANG")
        ElseIf namaValue = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_BESAR", "") Then
            satuan = rd("SATUAN_UMUM_BESAR")
            isi = rd("ISI_UMUM_BESAR")
        End If

        ' Set nilai ke grid
        DgvData.Rows(rowIndex).Cells("SATUAN").Value = satuan
        DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = isi

        ' Jika isi = 0, set qty_sat = 1
        If DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = 0 Then
            DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = 1
        End If

        ' Hitung nilai lainnya
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = CDec(DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value) * isi
        DgvData.Rows(rowIndex).Cells("QTY").Value = 1
        DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = CDec(DgvData.Rows(rowIndex).Cells("QTY").Value) * isi
        DgvData.Rows(rowIndex).Cells("TOTAL").Value = CDec(DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value) *
                                                      CDec(DgvData.Rows(rowIndex).Cells("QTY_SAT").Value)

        ' Cek duplikasi jika setting tidak mengizinkan
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
            ProsesMergeBarisDuplikat(rowIndex)
        End If
        SetWarnaReadOnlyNama(rowIndex)
    End Sub

    ' ============================================
    ' FUNGSI: PROSES MERGE BARIS DUPLIKAT
    ' ============================================
    Private Sub ProsesMergeBarisDuplikat(currentRowIndex As Integer)
        For barisatas As Integer = 0 To DgvData.RowCount - 1
            For barisbawah As Integer = barisatas + 1 To DgvData.RowCount - 2
                If DgvData.Rows(barisbawah).Cells("ID_BARANG").Value = DgvData.Rows(barisatas).Cells("ID_BARANG").Value Then
                    ' Merge qty
                    DgvData.Rows(barisatas).Cells("QTY").Value = DgvData.Rows(barisatas).Cells("QTY").Value + 1

                    ' Hitung qty_sat
                    If DgvData.Rows(barisbawah).Cells("ISI_SATUAN").Value = 0 Then
                        DgvData.Rows(barisatas).Cells("QTY_SAT").Value = DgvData.Rows(barisatas).Cells("QTY_SAT").Value + 1
                    Else
                        DgvData.Rows(barisatas).Cells("QTY_SAT").Value = DgvData.Rows(barisatas).Cells("ISI_SATUAN").Value *
                                                                        DgvData.Rows(barisatas).Cells("QTY").Value
                    End If

                    ' Hitung total
                    DgvData.Rows(barisatas).Cells("TOTAL").Value = DgvData.Rows(barisatas).Cells(2).Value *
                                                                   DgvData.Rows(barisatas).Cells("QTY_SAT").Value

                    ' Hapus baris duplikat dan hentikan loop
                    Call Hapusbaris()
                    UpdateSemuaTotal()
                    Exit Sub ' Cegah ArgumentOutOfRangeException karena RowCount berubah
                End If
            Next
        Next
    End Sub

    ' ============================================
    ' FUNGSI: PROSES EDIT HARGA BELI
    ' ============================================
    Private Sub ProsesEditHargaBeli(rowIndex As Integer)
        Dim hargaBeliValue As Decimal

        If Decimal.TryParse(DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value, hargaBeliValue) Then
            If hargaBeliValue <= 0 Then
                MessageBox.Show("Harga beli harus lebih besar dari 0.", "Peringatan",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = 0
            Else
                Dim qtyValue As Decimal = DgvData.Rows(rowIndex).Cells("QTY").Value
                Dim isiValue As Integer = DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value
                Dim qtySatValue As Decimal = qtyValue * isiValue

                ' Update nilai terkait
                DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = qtySatValue
                DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = CDec(DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value) * isiValue
                DgvData.Rows(rowIndex).Cells("TOTAL").Value = hargaBeliValue * qtySatValue

                ' Format kolom harga beli
                ModuleAngka.TerapkanFormatKolomAngka(DgvData, "HARGA_BELI_TERAKHIR")
            End If
        Else
            MessageBox.Show("Harga beli harus berupa angka.", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = 0
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PROSES EDIT QTY
    ' ============================================
    Private Sub ProsesEditQty(rowIndex As Integer)
        ' Ambil referensi sel
        Dim qtyCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("QTY")
        Dim qtySatCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("QTY_SAT")
        Dim hargaBeliCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR")
        Dim isiCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("ISI_SATUAN")
        Dim totalHargaCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("TOTAL")

        ' Ambil nilai dengan handling null
        Dim qtyValue As Decimal = If(IsDBNull(qtyCell.Value) OrElse qtyCell.Value Is Nothing, 0D, Convert.ToDecimal(qtyCell.Value))
        Dim isiValue As Decimal = If(IsDBNull(isiCell.Value) OrElse isiCell.Value Is Nothing, 0D, Convert.ToDecimal(isiCell.Value))
        Dim hargaBeliValue As Decimal = If(IsDBNull(hargaBeliCell.Value) OrElse hargaBeliCell.Value Is Nothing, 0D, Convert.ToDecimal(hargaBeliCell.Value))

        ' Validasi qty
        If qtyValue <= 0D Then
            MessageBox.Show("Qty harus lebih besar dari 0.", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            qtyCell.Value = 1D
            qtyValue = 1D
        End If

        ' Point: Validasi Stok real-time (Sesuai FormJual)
        Dim idBarang As String = DgvData.Rows(rowIndex).Cells("ID_BARANG").Value?.ToString()
        If Not String.IsNullOrEmpty(idBarang) AndAlso Not ModulHakAkses.SettingIzinkanBarangMinus Then
            Dim barang = GetBarangFromCache(idBarang)
            If barang IsNot Nothing Then
                Dim stokTersedia = barang.GetStokByLokasi(LblLokasiBarang.Text)
                Dim qtySatBaru = qtyValue * isiValue
                If qtySatBaru > stokTersedia Then
                    MessageBox.Show($"Stok tidak mencukupi!{vbCrLf}" &
                                    $"Stok tersedia: {stokTersedia:N0}{vbCrLf}" &
                                    $"Kebutuhan: {qtySatBaru:N0}",
                                    "Peringatan Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    ' Kembalikan ke 1 agar aman
                    qtyCell.Value = 1D
                    qtyValue = 1D
                End If
            End If
        End If

        ' Update nilai terkait
        qtySatCell.Value = qtyValue * isiValue
        totalHargaCell.Value = hargaBeliValue * CDec(qtySatCell.Value)

        ' Refresh tampilan grid
        DgvData.SuspendLayout()
        DgvData.ResumeLayout()
    End Sub

    ' ============================================
    ' EVENT: DATA ERROR DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DgvData.DataError
        ' Cancel error untuk mencegah crash
        e.Cancel = True
    End Sub

    ' ============================================
    ' EVENT: KEY DOWN DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Delete Then
            If DgvData.SelectedCells.Count > 0 Then
                Dim selectedCell As DataGridViewCell = DgvData.SelectedCells(0)

                If selectedCell.ColumnIndex = DgvData.Columns("NAMA_BARANG").Index Then
                    Dim rowIndex As Integer = selectedCell.RowIndex

                    If Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value.ToString()) Then
                        DgvData.Rows.RemoveAt(rowIndex)
                        DgvData.ClearSelection()
                    Else
                        MessageBox.Show("Klik kanan pada baris yang tidak kosong.", "Peringatan",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
        End If

        UpdateSemuaTotal()
    End Sub

    ' ============================================
    ' EVENT: ROW POST PAINT DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Draw row number
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b,
                                  e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub


    ' ============================================
    ' EVENT: EDITING CONTROL SHOWING DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles DgvData.EditingControlShowing

        Try
            ' Simpan baris saat ini
            currentGridRow = DgvData.CurrentCell.RowIndex

            ' KRITIS: Selalu remove handler TextChanged dari editing control lama
            ' karena DGV menggunakan satu TextBox yang di-share untuk semua kolom teks.
            ' Tanpa ini, pindah ke kolom QTY masih memicu pencarian nama barang.
            If _dgvEditingTextBox IsNot Nothing Then
                RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                _dgvEditingTextBox = Nothing
            End If

            ' ── Kolom SATUAN (index 4) — attach ComboBox handlers ─────────────
            If DgvData.CurrentCell.ColumnIndex = 4 Then
                Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)
                If comboBox IsNot Nothing Then
                    RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                    AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                    RemoveHandler comboBox.KeyDown, AddressOf ComboBox_KeyDown
                    AddHandler comboBox.KeyDown, AddressOf ComboBox_KeyDown
                End If
            End If

            ' ── Kolom NAMA_BARANG (index 1) — attach handler pencarian inline ──
            If DgvData.CurrentCell.ColumnIndex = 1 Then
                Dim editText As TextBox = TryCast(e.Control, TextBox)
                If editText IsNot Nothing Then
                    editText.AutoCompleteMode = AutoCompleteMode.None

                    _dgvEditingTextBox = editText

                    ' TextChanged hanya di-attach jika ListBox tidak visible
                    ' KRITIS: Hapus handler KeyDown/PreviewKeyDown karena navigasi ditangani ProcessCmdKey
                    If Not LstBarang.Visible AndAlso Not _sedangPindahKeLstBarang Then
                        AddHandler editText.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    Else
                    End If

                    PosisikanLstBarangDiBawahSel()
                End If
            End If

        Catch ex As Exception
        End Try
    End Sub
    ' ============================================
    Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellEnter
        ' Kolom SATUAN — langsung BeginEdit + buka dropdown agar panah atas/bawah
        ' bisa memilih satuan tanpa F2 atau klik, meski EditMode = EditOnKeystrokeOrF2
        If DgvData.Columns(e.ColumnIndex).Name = "SATUAN" Then
            DgvData.BeginInvoke(New Action(Sub()
                                               If DgvData.CurrentCell IsNot Nothing AndAlso
                   DgvData.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
                   DgvData.CurrentCell.RowIndex = e.RowIndex Then
                                                   DgvData.BeginEdit(True)
                                                   Dim combo = TryCast(DgvData.EditingControl, ComboBox)
                                                   If combo IsNot Nothing Then combo.DroppedDown = True
                                               End If
                                           End Sub))
        End If

        ' Kolom NAMA_BARANG — langsung BeginEdit agar bisa langsung ketik
        If DgvData.Columns(e.ColumnIndex).Name = "NAMA_BARANG" Then
            DgvData.BeginInvoke(New Action(Sub()
                                               If DgvData.CurrentCell IsNot Nothing AndAlso
                   DgvData.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
                   DgvData.CurrentCell.RowIndex = e.RowIndex Then
                                                   DgvData.BeginEdit(True)
                                               End If
                                           End Sub))
        End If

        ' Sembunyikan ListBox saat pindah ke kolom lain
        If DgvData.Columns(e.ColumnIndex).Name <> "NAMA_BARANG" Then
            LstBarang.Visible = False
        End If

        ' Guard ReadOnly kolom NAMA_BARANG berdasarkan ID_BARANG — konsisten dengan FormJual/Pembelian
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 1 Then ' Kolom NAMA_BARANG (index 1)
            SetWarnaReadOnlyNama(e.RowIndex)
        End If
    End Sub

    ' ============================================
    ' EVENT: KEY DOWN UNTUK COMBOBOX SATUAN
    ' ============================================
    Private Sub ComboBox_KeyDown(sender As Object, e As KeyEventArgs)
        ' SEDERHANAKAN: Hanya handle dropdown, biarkan navigasi default
        Dim comboBox As ComboBox = TryCast(sender, ComboBox)
        If comboBox Is Nothing Then Exit Sub

        Select Case e.KeyCode
            Case Keys.Down, Keys.Up
                ' Buka dropdown jika belum terbuka
                If Not comboBox.DroppedDown Then
                    comboBox.DroppedDown = True
                    e.SuppressKeyPress = True
                End If

            Case Keys.Escape
                ' Tutup dropdown
                If comboBox.DroppedDown Then
                    comboBox.DroppedDown = False
                    e.SuppressKeyPress = True
                End If
        End Select
    End Sub


    ' ============================================'
    ' EVENT: KEY DOWN UNTUK LIST BARANG (LISTBOX)
    ' ============================================'
    Private Sub LstBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstBarang.KeyDown

        _listBoxBaruDapatFokus = False

        Select Case e.KeyCode
            Case Keys.Enter
                If LstBarang.SelectedIndex >= 0 Then
                    _sedangPindahKeLstBarang = True
                    AmbilDataDariListBox()
                    _sedangPindahKeLstBarang = False
                End If
                e.SuppressKeyPress = True

            Case Keys.Escape
                LstBarang.Visible = False
                LstBarang.Items.Clear()
                If _konteksLstBarang = "DGV" AndAlso DgvData.CurrentCell IsNot Nothing Then
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    DgvData.Focus()
                    DgvData.BeginInvoke(New Action(Sub()
                                                       If DgvData.CurrentCell IsNot Nothing Then
                                                           DgvData.BeginEdit(True)
                                                           Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                                                           If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                               editCtrl.Text = teksSimpan
                                                               editCtrl.SelectionStart = teksSimpan.Length
                                                           End If
                                                           editCtrl?.Focus()
                                                       End If
                                                   End Sub))
                Else
                    _teksSebelumPindahKeLstBarang = ""
                    TxtNama.Focus()
                    TxtNama.SelectAll()
                End If
                e.SuppressKeyPress = True

            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    ' Di item pertama — kembali ke DGV atau TxtNama
                    LstBarang.Visible = False
                    LstBarang.Items.Clear()
                    If _konteksLstBarang = "DGV" AndAlso DgvData.CurrentCell IsNot Nothing Then
                        Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                        _teksSebelumPindahKeLstBarang = ""
                        DgvData.Focus()
                        DgvData.BeginInvoke(New Action(Sub()
                                                           If DgvData.CurrentCell IsNot Nothing Then
                                                               DgvData.BeginEdit(True)
                                                               Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                                                               If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                                   editCtrl.Text = teksSimpan
                                                                   editCtrl.SelectionStart = teksSimpan.Length
                                                               End If
                                                               editCtrl?.Focus()
                                                           End If
                                                       End Sub))
                    Else
                        _teksSebelumPindahKeLstBarang = ""
                        TxtNama.Focus()
                        TxtNama.SelectAll()
                    End If
                End If
                ' Jika bukan item pertama, biarkan ListBox handle navigasi Up secara default
        End Select
    End Sub



    ' ============================================
    ' EVENT: COMBOBOX SELECTED INDEX CHANGED
    ' ============================================
    Private isUpdatingFromCombo As Boolean = False ' Flag untuk mencegah loop

    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        ' Cegah reentrant call
        If isUpdatingFromCombo Then Exit Sub

        Try
            isUpdatingFromCombo = True

            Dim comboBox As ComboBox = DirectCast(sender, ComboBox)
            If DgvData.CurrentCell Is Nothing Then Exit Sub

            Dim cell As DataGridViewComboBoxCell = DirectCast(DgvData.CurrentCell, DataGridViewComboBoxCell)
            Dim selectedItemId As String = cell.OwningRow.Cells("ID_BARANG").Value?.ToString()

            If String.IsNullOrEmpty(selectedItemId) Then Exit Sub

            ' Cari di cache
            Dim barangFromCache = GetBarangFromCache(selectedItemId)
            If barangFromCache Is Nothing Then Exit Sub

            ' Tentukan isi berdasarkan pilihan satuan
            Dim isiSatuan As Integer = 0
            Select Case comboBox.SelectedIndex
                Case 0 : isiSatuan = barangFromCache.ISI_UMUM_KECIL
                Case 1 : isiSatuan = barangFromCache.ISI_UMUM_SEDANG
                Case Else : isiSatuan = barangFromCache.ISI_UMUM_BESAR
            End Select

            isiSatuan = Math.Max(1, isiSatuan)

            ' Validasi stok saat ganti satuan (Point)
            Dim rowIndex As Integer = cell.RowIndex
            If Not ModulHakAkses.SettingIzinkanBarangMinus Then
                Dim stokTersedia = barangFromCache.GetStokByLokasi(LblLokasiBarang.Text)
                Dim qty = If(DgvData.Rows(rowIndex).Cells("QTY").Value IsNot Nothing,
                             Convert.ToDecimal(DgvData.Rows(rowIndex).Cells("QTY").Value), 1D)
                Dim qtySatBaru = qty * isiSatuan
                If qtySatBaru > stokTersedia Then
                    MessageBox.Show($"Stok tidak mencukupi untuk satuan ini!{vbCrLf}" &
                                    $"Stok tersedia: {stokTersedia:N0}{vbCrLf}" &
                                    $"Kebutuhan: {qtySatBaru:N0}",
                                    "Peringatan Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    ' Kembalikan ke satuan terkecil agar transaksi tetap bisa lanjut
                    isUpdatingFromCombo = True
                    comboBox.SelectedIndex = 0
                    isiSatuan = barangFromCache.ISI_UMUM_KECIL
                    isUpdatingFromCombo = False
                End If
            End If

            ' Update row - LANGSUNG tanpa event trigger
            rowIndex = cell.RowIndex
            With DgvData.Rows(rowIndex)
                Dim hargaBeli As Decimal = If(.Cells("HARGA_BELI_TERAKHIR").Value IsNot Nothing,
                                          Convert.ToDecimal(.Cells("HARGA_BELI_TERAKHIR").Value), 0D)
                Dim qty As Decimal = If(.Cells("QTY").Value IsNot Nothing,
                                   Convert.ToDecimal(.Cells("QTY").Value), 1D)

                .Cells("ISI_SATUAN").Value = isiSatuan
                .Cells("HARGA_BELI_SATUAN").Value = hargaBeli * isiSatuan
                .Cells("QTY_SAT").Value = qty * isiSatuan
                .Cells("TOTAL").Value = hargaBeli * (qty * isiSatuan)
            End With

            ' Update total
            UpdateSemuaTotal()

        Catch ex As Exception
        Finally
            isUpdatingFromCombo = False
        End Try
    End Sub

    ' ============================================
    ' EVENT: AUTO TEXT CHANGED
    ' ============================================
    Private Sub AutoText_TextChanged(sender As Object, e As EventArgs)
        Dim textBox As TextBox = TryCast(sender, TextBox)
        If textBox IsNot Nothing AndAlso DgvData.CurrentCell IsNot Nothing Then
            ' Update AutoComplete collection berdasarkan teks yang sedang diketik
            Dim currentText = textBox.Text.Trim()
            If currentText.Length >= 2 Then ' Mulai filter setelah 2 karakter
                Dim DataCollection As New AutoCompleteStringCollection()
                AddItems(DataCollection, currentText)
                textBox.AutoCompleteCustomSource = DataCollection
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: ADD ITEMS TO AUTOCOMPLETE COLLECTION (WILDCARD LENGKAP, TANPA LIMIT)
    ' ============================================
    Public Sub AddItems(ByVal col As AutoCompleteStringCollection, ByVal namaValue As String)
        Try
            col.Clear()
            Dim keyword = namaValue.Trim().ToLower()
            If keyword = "" Then Exit Sub

            ' Multi-keyword split
            Dim keys = keyword.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

            Dim results As New List(Of String)

            ' ============================================
            ' Fungsi wildcard fleksibel: semua kata harus cocok
            ' ============================================
            Dim matchWildcard As Func(Of String, Boolean) =
            Function(text As String)
                Dim lowerText = text.ToLower()

                ' contoh: "a m" → "a*m"
                Dim joinKey = String.Join("*", keys)

                ' Pecah wildcard
                Dim parts = joinKey.Split("*"c)

                ' Semua bagian harus muncul berurutan
                Dim pos As Integer = 0
                For Each part In parts
                    pos = lowerText.IndexOf(part, pos)
                    If pos = -1 Then Return False
                    pos += part.Length
                Next

                Return True
            End Function

            ' ============================================
            ' Cari berdasarkan Nama Barang
            ' ============================================
            For Each b In barangCacheById.Values
                If matchWildcard(b.NAMA_BARANG) Then
                    results.Add(b.NAMA_BARANG)
                End If
            Next

            ' ============================================
            ' Cari berdasarkan Barcode
            ' ============================================
            For Each bc In barcodeLookupCache.Keys
                If matchWildcard(bc) Then
                    Dim idBarang = barcodeLookupCache(bc)
                    If barangCacheById.ContainsKey(idBarang) Then
                        Dim nama = barangCacheById(idBarang).NAMA_BARANG
                        If Not results.Contains(nama) Then
                            results.Add(nama)
                        End If
                    End If
                End If
            Next

            ' ============================================
            ' Cari berdasarkan Kode Barang
            ' ============================================
            For Each b In barangCacheById.Values
                If matchWildcard(b.ID_BARANG) Then
                    If Not results.Contains(b.NAMA_BARANG) Then
                        results.Add(b.NAMA_BARANG)
                    End If
                End If
            Next

            ' ============================================
            ' Kirim semua ke AutoComplete
            ' ============================================
            col.AddRange(results.ToArray())

        Catch ex As Exception
        End Try
    End Sub



    ' ============================================
    ' EVENT: CELL MOUSE UP DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvData.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            Dim cell As DataGridViewCell = DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG")
            If cell IsNot Nothing AndAlso cell.Value IsNot Nothing Then
                Dim namaValue As String = cell.Value.ToString()
                If Not String.IsNullOrEmpty(namaValue) Then
                    DgvData.CurrentCell = cell
                    Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                    ContextMenuStrip1.Show(cursorPosition)
                End If
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: SET WARNA READONLY KOLOM NAMA_BARANG
    ' ============================================
    ''' <summary>
    ''' Set ReadOnly dan warna kolom NAMA_BARANG berdasarkan apakah ID_BARANG sudah terisi.
    ''' Konsisten dengan FormJual, FormPembelian, FormReturPenjualan.
    ''' Dipanggil dari semua fungsi yang mengisi baris DGV.
    ''' </summary>
    Private Sub SetWarnaReadOnlyNama(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= DgvData.Rows.Count Then Return
        If DgvData.Rows(rowIndex).IsNewRow Then Return
        Dim idValue = DgvData.Rows(rowIndex).Cells("ID_BARANG").Value
        Dim adaId As Boolean = idValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(idValue.ToString())
        Dim cell = DgvData.Rows(rowIndex).Cells("NAMA_BARANG")
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

    ' ============================================
    ' EVENT: CELL FORMATTING DI DATAGRIDVIEW
    ' ============================================
    Private Sub DgvData_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvData.CellFormatting
        ' Set read-only Nama jika ID sudah terisi (samakan dengan FormPembelian)
        If e.ColumnIndex = 1 Then ' Kolom Nama (index 1)
            Dim idValue = DgvData.Rows(e.RowIndex).Cells("ID_BARANG").Value

            ' Jika ID sudah terisi, buat Nama read-only
            If idValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(idValue.ToString()) Then
                DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG").ReadOnly = True
                DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            Else
                DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG").ReadOnly = False
                DgvData.Rows(e.RowIndex).Cells("NAMA_BARANG").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
            End If
        End If

        ' Guard: hanya proses jika kolom stok ada
        If Not DgvData.Columns.Contains("StokToko") OrElse Not DgvData.Columns.Contains("StokGudang") Then Return

        Dim stokTokoIndex As Integer = DgvData.Columns("StokToko").Index
        Dim stokGudangIndex As Integer = DgvData.Columns("StokGudang").Index

        If e.ColumnIndex = stokTokoIndex OrElse e.ColumnIndex = stokGudangIndex Then
            If e.Value IsNot Nothing AndAlso ModuleAngka.ParseDecimal(e.Value) < 1 Then
                ' Stok habis — warna informasi (amber), bukan merah
                e.CellStyle.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowStokHabis, ModuleTheme.D_DgvRowStokHabis)
                e.CellStyle.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            End If
        End If
    End Sub

#End Region

    ' ============================================
    ' PERHITUNGAN TOTAL DAN UPDATE UI
    ' ============================================
#Region "Perhitungan Total"

    ' ============================================
    ' FUNGSI: UPDATE SEMUA TOTAL
    ' ============================================
    Private Sub UpdateSemuaTotal()
        If _isLoadingForm Then Return ' Point 5: Skip saat sedang load data
        Try
            Dim grandTotal As Decimal = 0
            Dim totalQty As Decimal = 0
            Dim totalRows As Integer = 0

            ' Hitung total untuk setiap baris
            For Each row As DataGridViewRow In DgvData.Rows
                If row.IsNewRow Then Continue For

                ' Hitung Grand Total
                Dim totalValue As Object = row.Cells("TOTAL").Value
                If totalValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(totalValue.ToString()) Then
                    grandTotal += Convert.ToDecimal(totalValue)
                End If

                ' Hitung Total QTY
                Dim qtySatValue As Object = row.Cells("QTY_SAT").Value
                If qtySatValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtySatValue.ToString()) Then
                    totalQty += Convert.ToDecimal(qtySatValue)
                End If

                ' Hitung Jumlah Baris
                Dim qtyValue As Object = row.Cells("QTY").Value
                If qtyValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyValue.ToString()) Then
                    totalRows += 1
                End If
            Next

            TxtGrandTotalRetur.Text = grandTotal.ToString("N0")
            TxtTotalQTY.Text = totalQty.ToString("N0")
            LblRecord.Text = totalRows.ToString("N0")
            TxtGrandtotal.Text = "Rp. " & grandTotal.ToString("N0")

            ' Scroll ke baris terakhir
            If DgvData.Rows.Count > 0 Then
                DgvData.FirstDisplayedScrollingRowIndex = DgvData.Rows.Count - 1
            End If

        Catch ex As Exception
        End Try
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED UNTUK GRAND TOTAL
    ' ============================================
    Private Sub TxtGrandTotalRetur_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtGrandTotalRetur.TextChanged
        If _isLoadingForm Then Return
        Dim grandTotal As Decimal = ModuleAngka.ParseDecimal(TxtGrandTotalRetur.Text)
        TxtGrandtotal.Text = ModuleAngka.FormatRupiah(grandTotal)
        LblGrandTotalRetur.Text = ModuleAngka.FormatRupiah(grandTotal)
    End Sub

#End Region

    ' ============================================
    ' VALIDASI DAN CEK STOK
    ' ============================================
#Region "Validasi dan Stok"

    ' ============================================
    ' FUNGSI: CEK STOK BARANG
    ' ============================================
    Public Function CekStok() As Boolean
        Try
            ' Kumpulkan semua ID barang yang perlu dicek
            Dim barangIds As New List(Of String)()
            Dim barangQty As New Dictionary(Of String, Decimal)()

            ' Kumpulkan data dari grid
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                    Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
                    If Not String.IsNullOrEmpty(kodeBarang) Then
                        barangIds.Add(kodeBarang)
                        Dim qtySat As Decimal = If(row.Cells("QTY_SAT").Value IsNot Nothing,
                                                   Convert.ToDecimal(row.Cells("QTY_SAT").Value), 0D)
                        barangQty(kodeBarang) = qtySat
                    End If
                End If
            Next

            If barangIds.Count = 0 Then Return False

            ' Ambil stok semua barang sekaligus dengan IN clause
            Dim stokDict As New Dictionary(Of String, StokInfo)()
            Dim idList As String = String.Join("','", barangIds)
            Dim query As String = $"SELECT ID_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG IN ('{idList}')"

            Using cmd As New MySqlCommand(query, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim idBarang As String = rd("ID_BARANG").ToString()
                        Dim stokToko As Decimal = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                        Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                        stokDict(idBarang) = New StokInfo With {.StokToko = stokToko, .StokGudang = stokGudang}
                    End While
                End Using
            End Using

            ' Proses pengecekan
            For Each kvp In barangQty
                Dim kodeBarang As String = kvp.Key
                Dim qtyRetur As Decimal = kvp.Value

                If stokDict.ContainsKey(kodeBarang) Then
                    Dim stokInfo As StokInfo = stokDict(kodeBarang)
                    Dim stokTersedia As Decimal = If(LblLokasiBarang.Text = "TOKO",
                                                     stokInfo.StokToko, stokInfo.StokGudang)

                    If qtyRetur > stokTersedia Then
                        ' Temukan baris yang bermasalah
                        For Each row As DataGridViewRow In DgvData.Rows
                            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value?.ToString() = kodeBarang Then
                                HighlightProblemRow(row, $"Stok tidak mencukupi. Qty: {qtyRetur}, Stok: {stokTersedia}")
                                Return True
                            End If
                        Next
                    End If
                End If
            Next

            Return False

        Catch ex As Exception
            MessageBox.Show($"Error cek stok: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return True
        End Try
    End Function

    ' ============================================
    ' FUNGSI: HIGHLIGHT PROBLEM ROW
    ' ============================================
    Private Sub HighlightProblemRow(row As DataGridViewRow, message As String)
        row.Selected = True
        For Each cell As DataGridViewCell In row.Cells
            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowPeringatan, ModuleTheme.D_DgvRowPeringatan)
        Next
        DgvData.CurrentCell = row.Cells(1)
        MessageBox.Show(message, "Stok Tidak Cukup",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    ' ============================================
    ' FUNGSI: AMBIL INFO STOK VIA SP
    ' ============================================
    Private Function AmbilInfoStok(kodeBarang As String, ByRef stokToko As Decimal, ByRef stokGudang As Decimal) As Boolean
        stokToko = 0D
        stokGudang = 0D
        If String.IsNullOrWhiteSpace(kodeBarang) Then Return False
        Try
            If IsModeTambahReturBeli Then
                ' Mode tambah — stok DB apa adanya
                Using cmd As New MySqlCommand(
                    "CALL sp_hlp_stok_ambil(@kode, @toko, @gudang, @nama)", conn)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    Dim pToko = cmd.Parameters.Add("@toko", MySqlDbType.Decimal)
                    pToko.Direction = ParameterDirection.Output
                    Dim pGudang = cmd.Parameters.Add("@gudang", MySqlDbType.Decimal)
                    pGudang.Direction = ParameterDirection.Output
                    Dim pNama = cmd.Parameters.Add("@nama", MySqlDbType.VarChar, 200)
                    pNama.Direction = ParameterDirection.Output
                    cmd.ExecuteNonQuery()
                    stokToko = ModuleAngka.ParseDecimal(pToko.Value)
                    stokGudang = ModuleAngka.ParseDecimal(pGudang.Value)
                    Return True
                End Using
            Else
                ' Mode edit — stok efektif = stok DB - qty di retur lama yang akan dikurangi
                Using cmd As New MySqlCommand(
                    "CALL sp_hlp_stok_ambil_edit_retur_beli(@kode, @faktur, @lokasi, @toko, @gudang, @nama)", conn)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                    cmd.Parameters.AddWithValue("@lokasi", LblLokasiBarang.Text)
                    Dim pToko = cmd.Parameters.Add("@toko", MySqlDbType.Decimal)
                    pToko.Direction = ParameterDirection.Output
                    Dim pGudang = cmd.Parameters.Add("@gudang", MySqlDbType.Decimal)
                    pGudang.Direction = ParameterDirection.Output
                    Dim pNama = cmd.Parameters.Add("@nama", MySqlDbType.VarChar, 200)
                    pNama.Direction = ParameterDirection.Output
                    cmd.ExecuteNonQuery()
                    stokToko = ModuleAngka.ParseDecimal(pToko.Value)
                    stokGudang = ModuleAngka.ParseDecimal(pGudang.Value)
                    Return True
                End Using
            End If
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Refresh info stok satu baris DGV dari DB.
    ''' Tidak mengubah data transaksi — hanya update kolom StokToko/StokGudang.
    ''' </summary>
    Private Sub RefreshStokBaris(rowIdx As Integer)
        If rowIdx < 0 OrElse rowIdx >= DgvData.Rows.Count Then Return
        Dim row = DgvData.Rows(rowIdx)
        If row.IsNewRow Then Return
        Dim kode As String = Convert.ToString(row.Cells("ID_BARANG").Value).Trim()
        If String.IsNullOrEmpty(kode) Then Return

        If Not DgvData.Columns.Contains("StokToko") OrElse Not DgvData.Columns.Contains("StokGudang") Then Return

        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D
        If AmbilInfoStok(kode, stokToko, stokGudang) Then
            row.Cells("StokToko").Value = stokToko
            row.Cells("StokGudang").Value = stokGudang
        End If
    End Sub

    ''' <summary>
    ''' Refresh info stok semua baris DGV yang sudah terisi.
    ''' Dipakai untuk: load edit/draft agar stok selalu fresh.
    ''' </summary>
    Private Sub RefreshStokSemuaBaris()
        For i As Integer = 0 To DgvData.Rows.Count - 1
            RefreshStokBaris(i)
        Next
    End Sub

#End Region

    ' ============================================
    ' PROSES PEMILIHAN REKENING
    ' ============================================
#Region "Proses Rekening"

    ' ============================================
    ' EVENT: SELECTED INDEX CHANGED UNTUK COMBOBOX REKENING TUNAI
    ' ============================================
    Private Sub CmbAkunTunai_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbAkunTunai.SelectedIndexChanged
        Dim namaAkunD As String = CmbAkunTunai.Text
        Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtKodeAkunTunai.Text = reader("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub

    ' ============================================
    ' EVENT: SELECTED INDEX CHANGED UNTUK COMBOBOX REKENING TRANSFER
    ' ============================================
    Private Sub CmbAkunTransfer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbAkunTransfer.SelectedIndexChanged
        Dim namaAkunD As String = CmbAkunTransfer.Text
        Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtKodeAkunTransfer.Text = reader("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED UNTUK NOMINAL BAYAR TUNAI
    ' ============================================
    Private Sub TxtNominalBayarTunai_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNominalBayarTunai.TextChanged
        Dim nominalTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        LblBayarTunai.Text = ModuleAngka.FormatRupiah(nominalTunai)
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED UNTUK NOMINAL BAYAR TRANSFER
    ' ============================================
    Private Sub TxtNominalBayarTransfer_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNominalBayarTransfer.TextChanged
        Dim nominalTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        LblBayarTransfer.Text = ModuleAngka.FormatRupiah(nominalTransfer)
    End Sub

#End Region

    ' ============================================
    ' PROSES SIMPAN DAN TRANSAKSI
    ' ============================================
#Region "Proses Simpan"

    ' ============================================
    ' EVENT: CLICK BUTTON BAYAR
    ' ============================================
    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        Tekansimpan()
    End Sub

    ' ============================================
    ' FUNGSI: TEKAN SIMPAN
    ' ============================================
    Public Sub Tekansimpan()

        If String.IsNullOrEmpty(LblKodeSupplier.Text) Then
            MessageBox.Show("Supliyer belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TxtSupplier.Select()
            Exit Sub
        End If

        ' Validasi data sebelum simpan
        Dim totalQty As Integer

        If Not Integer.TryParse(TxtTotalQTY.Text, totalQty) OrElse totalQty = 0 _
   OrElse DgvData.RowCount = 0 Then
            MessageBox.Show("Belum ada transaksi Retur", "Kesalahan",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            ' Fokus ke grid jika ada data
            If DgvData.Rows.Count > 0 AndAlso DgvData.Columns.Count > 1 Then
                DgvData.CurrentCell = DgvData(1, 0)
                DgvData.Rows(0).Selected = True
            End If

            ' Fokus berdasarkan setting
            SetupFocusToGrid()
            Exit Sub
        End If

        ' Cek stok jika setting tidak mengizinkan minus
        If Not ModulHakAkses.SettingIzinkanBarangMinus Then
            If CekStok() Then
                Return
            End If
        End If



        ' Tampilkan form bayar
        GBBayar.Visible = True

        ' Isi nominal bayar berdasarkan setting
        If LblJenisTrans.Text = "TambahReturBeli" Then
            If ModulHakAkses.SettingLangsungIsiNominalTotal Then
                TxtNominalBayarTunai.Text = TxtGrandTotalRetur.Text
            Else
                TxtNominalBayarTunai.Text = ""
            End If
        End If

        ' Fokus ke nominal bayar tunai dan seleksi semua teks
        TxtNominalBayarTunai.Focus()
        TxtNominalBayarTunai.SelectAll()
    End Sub

    ' ============================================
    ' EVENT: CLICK BUTTON SIMPAN
    ' ============================================
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        ProsesSimpan()
    End Sub

    ' ============================================
    ' EVENT: CLICK BUTTON BATAL
    ' ============================================
    Private Sub BtnBatal_Click(sender As Object, e As EventArgs) Handles BtnBatal.Click
        GBBayar.Visible = False
    End Sub

    ' ============================================
    ' FUNGSI: PROSES SIMPAN TRANSAKSI
    ' ============================================
    Public Sub ProsesSimpan()
        ' Validasi split bayar - wajib lunas (tidak ada piutang supplier)
        Dim grandTotal As Decimal = ModuleAngka.ParseDecimal(TxtGrandTotalRetur.Text)
        Dim nominalTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim nominalTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim totalBayar As Decimal = nominalTunai + nominalTransfer

        If totalBayar <> grandTotal Then
            MessageBox.Show($"Total bayar harus sama dengan Grand Total ({ModuleAngka.FormatRupiah(grandTotal)})." & vbCrLf &
                           $"Total Bayar saat ini: {ModuleAngka.FormatRupiah(totalBayar)}" & vbCrLf &
                           "Retur pembelian wajib dibayar penuh (tidak ada piutang supplier).",
                           "Validasi Pembayaran", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNominalBayarTunai.Focus()
            Exit Sub
        End If

        ' Validasi akun tunai jika ada pembayaran tunai
        If nominalTunai > 0 AndAlso String.IsNullOrEmpty(CmbAkunTunai.Text) Then
            MessageBox.Show("Akun Tunai harus dipilih jika ada pembayaran tunai.",
                           "Validasi Pembayaran", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbAkunTunai.Focus()
            Exit Sub
        End If

        ' Validasi akun transfer jika ada pembayaran transfer
        If nominalTransfer > 0 AndAlso String.IsNullOrEmpty(CmbAkunTransfer.Text) Then
            MessageBox.Show("Akun Transfer harus dipilih jika ada pembayaran transfer.",
                           "Validasi Pembayaran", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbAkunTransfer.Focus()
            Exit Sub
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 8: CEK STOK REAL-TIME VIA SP (ANTI RACE CONDITION)
        ' Validasi ini berbeda dari CekStok() di Tekansimpan:
        ' - CekStok() di Tekansimpan: cek stok dari data yang sudah di-load ke form
        ' - sp_hlp_stok_validasi di sini: cek stok LANGSUNG dari DB dengan FOR UPDATE
        '   untuk menangkap kasus user lain sudah transaksi duluan sejak form dibuka
        ' ═══════════════════════════════════════════════════════════════
        If Not ModulHakAkses.SettingIzinkanBarangMinus Then
            For Each dgvRow As DataGridViewRow In DgvData.Rows
                If Not dgvRow.IsNewRow AndAlso
                   dgvRow.Cells("ID_BARANG").Value IsNot Nothing AndAlso
                   Not String.IsNullOrEmpty(dgvRow.Cells("ID_BARANG").Value.ToString()) Then

                    Dim kodeBarang As String = dgvRow.Cells("ID_BARANG").Value.ToString()
                    Dim qtySat As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("QTY_SAT").Value)
                    Dim namaBarang As String = Convert.ToString(dgvRow.Cells("NAMA_BARANG").Value)

                    ' Untuk mode edit: kurangi qty yang sudah tersimpan di faktur ini
                    Dim qtyDibutuhkan As Decimal = qtySat
                    If Not IsModeTambahReturBeli Then
                        Try
                            Using cmdQtyLama As New MySqlCommand(
                                "SELECT COALESCE(SUM(QTY_SAT), 0) FROM retur_pembelian_detail " &
                                "WHERE ID_RETUR_PEMBELIAN = @fk AND ID_BARANG = @id", conn)
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

                    ' Panggil SP validasi stok real-time
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
                                ' Stok sudah berubah sejak form dibuka — kemungkinan user lain sudah transaksi
                                MessageBox.Show(
                                    "⚠️ Stok berubah sejak form dibuka!" & vbCrLf & vbCrLf &
                                    pErrMsg.Value?.ToString() & vbCrLf & vbCrLf &
                                    "Kemungkinan ada transaksi lain yang baru saja memproses barang ini." & vbCrLf &
                                    "Silakan periksa kembali jumlah yang akan diretur.",
                                    "Konflik Stok",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                )
                                ' Highlight baris yang bermasalah
                                dgvRow.Selected = True
                                For Each cell As DataGridViewCell In dgvRow.Cells
                                    cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowKonflik, ModuleTheme.D_DgvRowKonflik)
                                Next
                                DgvData.Focus()
                                DgvData.CurrentCell = dgvRow.Cells("NAMA_BARANG")
                                Exit Sub
                            End If
                        End Using
                    Catch ex As Exception
                        ' Jika SP gagal dipanggil (misal koneksi putus), lanjutkan saja
                        ' Validasi VB di Tekansimpan sudah cukup sebagai lapisan pertama
                    End Try
                End If
            Next
        End If

        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Try
            ' Simpan faktur asli SEBELUM NomorRetur() bisa mengubah TxtFaktur.Text
            Dim fakturAsli As String = TxtFaktur.Text

            If LblJenisTrans.Text = "TambahReturBeli" Then
                ' ===== MODE TAMBAH =====
                ' SettingIzinkanTanggalLampau=False → paksa tanggal ke Now, generate nomor baru
                ' SettingIzinkanTanggalLampau=True  → pakai tanggal dari DTP user, generate nomor baru
                If Not ModulHakAkses.SettingIzinkanTanggalLampau Then
                    ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTgl)
                End If
                NomorRetur()
            Else
                ' ===== MODE EDIT =====
                ' Nomor transaksi SELALU tetap pakai yang asli
                ' Tanggal TIDAK diubah - pakai apa adanya dari DTP (sudah diisi dari DB saat load)
                ' SettingIzinkanTanggalLampau=False → warning hanya jika user MENGUBAH tanggal ke lampau
                ' Jika tanggal sama dengan aslinya (tidak diubah user), langsung simpan tanpa warning
                If Not ModulHakAkses.SettingIzinkanTanggalLampau AndAlso
                   DTPTgl.Value.Date <> TanggalAsliEdit.Date AndAlso
                   DTPTgl.Value.Date < DateTime.Now.Date Then
                    Dim konfirmasi = MessageBox.Show(
                        $"Tanggal transaksi ({DTPTgl.Value:dd/MM/yyyy}) adalah tanggal lampau." & vbCrLf &
                        "Lanjutkan simpan dengan tanggal ini?",
                        "Konfirmasi Tanggal Lampau",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    If konfirmasi = DialogResult.No Then
                        Exit Sub
                    End If
                End If
                ' Hapus data lama pakai faktur asli
                ' ========================================
                ' START: Audit Trail - Edit Retur Pembelian
                ' ========================================
                ModuleAuditTrail.CatatAudit(fakturAsli, "EDIT", "Retur Pembelian", ket:="[KRITIS] Edit retur pembelian", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Retur Pembelian
                ' ========================================
                HapusUntukEdit(transaction, fakturAsli)
            End If

            ' Simpan semua data transaksi
            ReturBeli(transaction)
            ReturBeli_Detail(transaction)

            ' Audit: inisialisasi dictionary
            Dim auditDGV As New Dictionary(Of String, Decimal)()
            Dim auditHistory As New Dictionary(Of String, Decimal)()
            Dim auditDetail As New Dictionary(Of String, Decimal)()
            Dim auditStokDelta As New Dictionary(Of String, Decimal)()

            ' Audit A + C: baca qty dari DGV (kolom QTY_SAT)
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    Dim kodeA As String = row.Cells("ID_BARANG").Value.ToString()
                    Dim qtyA As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY_SAT").Value)
                    If auditDGV.ContainsKey(kodeA) Then auditDGV(kodeA) += qtyA Else auditDGV(kodeA) = qtyA
                    If auditDetail.ContainsKey(kodeA) Then auditDetail(kodeA) += qtyA Else auditDetail(kodeA) = qtyA
                End If
            Next

            HistoryBarang(transaction, auditHistory)   ' mengisi B
            Simpanjurnal(transaction)

            ' Recalculate stok + Audit D
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso
                   row.Cells(0).Value.ToString() <> "" Then
                    Dim kodeD As String = row.Cells("ID_BARANG").Value.ToString()
                    Dim stokSebelum As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    HitungStokPerubahan(kodeD, transaction)
                    Dim stokSesudah As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    Dim delta As Decimal = stokSebelum - stokSesudah  ' retur beli mengurangi stok
                    If auditStokDelta.ContainsKey(kodeD) Then auditStokDelta(kodeD) += delta Else auditStokDelta(kodeD) = delta
                End If
            Next

            AuditStokTransaksi(TxtFaktur.Text, "Retur Beli", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)

            ' ========================================
            ' STEP 3: UPDATE saldo akun — incremental delta
            ' ========================================
            UpdateSaldoAkunDeltaDariFaktur(TxtFaktur.Text, transaction)

            ' Commit transaksi
            transaction.Commit()

            Dim rbNominal As Decimal = ModuleAngka.ParseDecimal(TxtGrandTotalRetur.Text)
            CatatJurnalTidakSeimbang(TxtFaktur.Text, rbNominal, rbNominal, "Retur Beli",
                {"ReturBeli"})

            ' Catat history

            ' Tutup form jika edit
            If LblJenisTrans.Text <> "TambahReturBeli" Then
                Close()
            End If

            ' Cetak setelah simpan
            Dim noRetur As String = TxtFaktur.Text
            Kondisiawal()

            Try
                Select Case BacaPengaturanPrinter("ReturBeli", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakReturBeli(noRetur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak retur barang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakReturBeli(noRetur)
                        End If
                    Case "TAMPILKAN DI MONITOR"
                        ModulePrinterReturBeli.PreviewReturBeli(noRetur)
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak retur barang." & vbCrLf & "Detail: " & ex.Message,
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try

        Catch ex As Exception
            ' Rollback jika ada error
            MessageBox.Show("Oh tidak! Transaksi retur barang dibatalkan karena terjadi kesalahan." & vbCrLf &
                         "Detail kesalahan: " & ex.Message,
              "Oops! Ada masalah simpan retur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            transaction.Rollback()
        Finally
            System.Windows.Forms.Cursor.Current = Cursors.Default
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: HAPUS UNTUK EDIT TRANSAKSI
    ' ============================================
    Private Sub HapusUntukEdit(ByVal transaction As MySqlTransaction, ByVal fakturAsli As String)
        ' Gunakan modul pusat agar logika reversal (stok, jurnal, saldo) 100% akurat dan konsisten
        ModuleHapusTransaksi.HapusReturPembelian(fakturAsli, LblLokasiBarang.Text, transaction)
    End Sub

    ' ============================================
    ' FUNGSI: SIMPAN RETUR BELI (HEADER)
    ' ============================================
    Private Sub ReturBeli(ByVal transaction As MySqlTransaction)

        Dim sql As String = "INSERT INTO retur_pembelian (" &
        "ID_RETUR_PEMBELIAN, TGL_RETUR_BELI, " &
        "ID_SUPPLIER, NAMA_SUPPLIER, ALAMAT_SUPPLIER, KONTAK_SUPPLIER, " &
        "PENYIMPANAN, TOTAL_BARANG, TOTAL_QTY, TOTAL_RUPIAH, " &
        "JENIS_PENGEMBALIAN, NAMA_REKENING, KODE_REKENING, " &
        "NAMA_REKENING_TRANSFER, KODE_REKENING_TRANSFER, " &
        "NOMINAL_TUNAI, NOMINAL_TRANSFER, ALASAN_RETUR, " &
        "ID_USER, ID_KOMPUTER) " &
        "VALUES (" &
        "@ID_RETUR, @TGL_RETUR, " &
        "@ID_SUPPLIER, @NAMA_SUPPLIER, @ALAMAT_SUPPLIER, @KONTAK_SUPPLIER, " &
        "@PENYIMPANAN, @TOTAL_BARANG, @TOTAL_QTY, @TOTAL_RUPIAH, " &
        "@JENIS_PENGEMBALIAN, @NAMA_REKENING, @KODE_REKENING, " &
        "@NAMA_REKENING_TRANSFER, @KODE_REKENING_TRANSFER, " &
        "@NOMINAL_TUNAI, @NOMINAL_TRANSFER, @ALASAN_RETUR, " &
        "@ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(sql, conn, transaction)

            ' ===== HEADER WAJIB =====
            cmd.Parameters.AddWithValue("@ID_RETUR", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TGL_RETUR", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))

            ' ===== SUPPLIER =====
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPPLIER", TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_SUPPLIER", LblAlamatSupplier.Text)
            cmd.Parameters.AddWithValue("@KONTAK_SUPPLIER", LblKontakSupplier.Text)

            ' Jika ada penyimpanan di form
            cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasiBarang.Text)

            ' ===== TOTAL =====
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", ModuleAngka.ParseDecimal(LblRecord.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtGrandTotalRetur.Text))

            ' ===== RETURN DETAIL =====
            Dim nominalTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
            Dim nominalTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)

            ' Tentukan jenis pengembalian dan rekening utama
            ' Kolom lama (NAMA_REKENING, KODE_REKENING) digunakan untuk tunai
            If nominalTunai > 0 AndAlso nominalTransfer > 0 Then
                ' Split payment
                cmd.Parameters.AddWithValue("@JENIS_PENGEMBALIAN", "Split Tunai & Transfer")
                cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbAkunTunai.Text)
                cmd.Parameters.AddWithValue("@KODE_REKENING", TxtKodeAkunTunai.Text)
            ElseIf nominalTunai > 0 Then
                ' Tunai saja
                cmd.Parameters.AddWithValue("@JENIS_PENGEMBALIAN", "Tunai")
                cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbAkunTunai.Text)
                cmd.Parameters.AddWithValue("@KODE_REKENING", TxtKodeAkunTunai.Text)
            ElseIf nominalTransfer > 0 Then
                ' Transfer saja
                cmd.Parameters.AddWithValue("@JENIS_PENGEMBALIAN", "Transfer")
                cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbAkunTransfer.Text)
                cmd.Parameters.AddWithValue("@KODE_REKENING", TxtKodeAkunTransfer.Text)
            Else
                ' Default tunai
                cmd.Parameters.AddWithValue("@JENIS_PENGEMBALIAN", "Tunai")
                cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbAkunTunai.Text)
                cmd.Parameters.AddWithValue("@KODE_REKENING", TxtKodeAkunTunai.Text)
            End If

            ' Simpan split bayar ke kolom baru
            ' Kolom lama (NAMA_REKENING, KODE_REKENING) sudah diset di atas untuk tunai
            cmd.Parameters.AddWithValue("@NAMA_REKENING_TRANSFER", If(nominalTransfer > 0, CmbAkunTransfer.Text, DBNull.Value))
            cmd.Parameters.AddWithValue("@KODE_REKENING_TRANSFER", If(nominalTransfer > 0, TxtKodeAkunTransfer.Text, DBNull.Value))
            cmd.Parameters.AddWithValue("@NOMINAL_TUNAI", nominalTunai)
            cmd.Parameters.AddWithValue("@NOMINAL_TRANSFER", nominalTransfer)
            cmd.Parameters.AddWithValue("@ALASAN_RETUR", RTBAlasanRetur.Text)

            ' ===== USER & KOMPUTER =====
            cmd.Parameters.AddWithValue("@ID_USER",
            If(LblJenisTrans.Text = "TambahReturBeli",
               FormUtama.StatusNamaUser.Text, TxtLogin.Text))

            cmd.Parameters.AddWithValue("@ID_KOMPUTER",
            If(LblJenisTrans.Text = "TambahReturBeli",
               FormUtama.StatusNamaPC.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    ' ============================================
    ' FUNGSI: SIMPAN RETUR BELI DETAIL
    ' ============================================
    Private Sub ReturBeli_Detail(ByVal transaction As MySqlTransaction)
        Dim noUrut As Integer = 0

        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso
               row.Cells(0).Value.ToString() <> "" Then

                noUrut += 1

                Dim sqlrinci As String = "INSERT INTO retur_pembelian_detail " &
                    "(ID_RETUR_PEMBELIAN, TGL_RETUR_BELI, ID_SUPLIYER, NAMA_SUPLIYER, " &
                    "ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, " &
                    "HARGA_BELI_SATUAN, QTY_SAT, TOTAL, PENYIMPANAN, ID_USER, ID_KOMPUTER) " &
                    "VALUES (@ID_RETUR_PEMBELIAN, @TGL_RETUR_BELI, @ID_SUPLIYER, @NAMA_SUPLIYER, " &
                    "@ID_BARANG, @NAMA_BARANG, @HARGA_BELI, @QTY, @SATUAN, @ISI_SATUAN, " &
                    "@HARGA_BELI_SATUAN, @QTY_SAT, @TOTAL, @PENYIMPANAN, @ID_USER, @ID_KOMPUTER)"

                Using cmd As New MySqlCommand(sqlrinci, conn, transaction)
                    ' Set parameter dari grid
                    cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)
                    cmd.Parameters.AddWithValue("@TGL_RETUR_BELI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@ID_SUPLIYER", LblKodeSupplier.Text)
                    cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtSupplier.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells("ID_BARANG").Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells("NAMA_BARANG").Value)
                    cmd.Parameters.AddWithValue("@HARGA_BELI", ModuleAngka.ParseDecimal(row.Cells("HARGA_BELI_TERAKHIR").Value))
                    cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells("QTY").Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells("SATUAN").Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("ISI_SATUAN").Value))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("HARGA_BELI_SATUAN").Value))
                    cmd.Parameters.AddWithValue("@QTY_SAT", ModuleAngka.ParseDecimal(row.Cells("QTY_SAT").Value))
                    cmd.Parameters.AddWithValue("@TOTAL", ModuleAngka.ParseDecimal(row.Cells("TOTAL").Value))
                    cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                    cmd.ExecuteNonQuery()
                End Using

                ' Update stok barang
                Dim updateStokField As String = String.Empty

                ' Tentukan field stok berdasarkan lokasi
                Select Case LblLokasiBarang.Text
                    Case "TOKO"
                        updateStokField = "RETUR_BELI_TOKO"
                    Case "GUDANG"
                        updateStokField = "RETUR_BELI_GUDANG"
                    Case Else
                        Throw New InvalidOperationException("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " &
                                            updateStokField & " + @StokPengurangan WHERE ID_BARANG = @KodeBarang"

                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim stokPengurangan As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY_SAT").Value)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End If
        Next
    End Sub

    ' ============================================
    ' FUNGSI: SIMPAN HISTORY BARANG
    ' ============================================
    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction, ByRef auditHistory As Dictionary(Of String, Decimal))
        Dim query As String = "INSERT INTO HistoryBarang " &
            "(FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, " &
            "QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, " &
            "@QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso
               row.Cells(0).Value.ToString() <> "" Then
                SaveHistory(query, transaction, "RETUR BELI", LblLokasiBarang.Text, row)

                ' Audit B
                Dim kodeB As String = row.Cells("ID_BARANG").Value.ToString()
                Dim qtyB As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY_SAT").Value)
                If auditHistory.ContainsKey(kodeB) Then auditHistory(kodeB) += qtyB Else auditHistory(kodeB) = qtyB
            End If
        Next
    End Sub

    ' ============================================
    ' FUNGSI: SAVE HISTORY
    ' ============================================
    Private Sub SaveHistory(ByVal query As String, ByVal transaction As MySqlTransaction,
                           ByVal jenis As String, ByVal Lokasi As String, ByVal row As DataGridViewRow)

        Using cmd As New MySqlCommand(query, conn, transaction)
            ' Set semua parameter
            cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@JENIS", jenis)
            cmd.Parameters.AddWithValue("@LOKASI", Lokasi)
            cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells("ID_BARANG").Value)
            cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells("NAMA_BARANG").Value)
            cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells("QTY").Value))
            cmd.Parameters.AddWithValue("@SATUAN", row.Cells("SATUAN").Value)
            cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("ISI_SATUAN").Value))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(row.Cells("QTY_SAT").Value))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(row.Cells("TOTAL").Value))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                       FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                           FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ============================================
    ' FUNGSI: SIMPAN JURNAL
    ' ============================================
    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        Dim nominalTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim nominalTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim tgl As String = DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss")

        ' Variabel audit untuk Debug
        Dim totalDebet As Decimal = 0D
        Dim totalKredit As Decimal = 0D

        Debug.WriteLine("")
        Debug.WriteLine("════════════════════════════ JURNAL RETUR PEMBELIAN ════════════════════════════")
        Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12} {5,12}", "ID", "KETERANGAN", "DEBET", "KREDIT", "NOMINAL_D", "NOMINAL_K"))
        Debug.WriteLine(New String("-"c, 115))

        ' Simpan jurnal untuk pembayaran tunai (jika ada)
        If nominalTunai > 0 Then
            Using cmd As New MySqlCommand("INSERT INTO JurnalUmum " &
                "(NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, " &
                "NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, " &
                "@NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)",
                conn, transaction)

                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
                cmd.Parameters.AddWithValue("@URAIAN", "Retur pembelian barang dari " &
                                            LblLokasiBarang.Text & " supplier " & TxtSupplier.Text & " (Tunai)")
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", CmbAkunTunai.Text)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", TxtKodeAkunTunai.Text)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMINAL", nominalTunai)
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "RETUR PEMBELIAN")
                cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                           FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                               FormUtama.StatusNamaPC.Text, TxtKomputer.Text))

                cmd.ExecuteNonQuery()
            End Using
            totalDebet += nominalTunai
            totalKredit += nominalTunai
            Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "J1", "Tunai", CmbAkunTunai.Text, NAMA_REK_BARANG, nominalTunai, nominalTunai))
        End If

        ' Simpan jurnal untuk pembayaran transfer (jika ada)
        If nominalTransfer > 0 Then
            Using cmd As New MySqlCommand("INSERT INTO JurnalUmum " &
                "(NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, " &
                "NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, " &
                "@NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)",
                conn, transaction)

                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
                cmd.Parameters.AddWithValue("@URAIAN", "Retur pembelian barang dari " &
                                            LblLokasiBarang.Text & " supplier " & TxtSupplier.Text & " (Transfer)")
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", CmbAkunTransfer.Text)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", TxtKodeAkunTransfer.Text)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMINAL", nominalTransfer)
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "RETUR PEMBELIAN")
                cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                           FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                               FormUtama.StatusNamaPC.Text, TxtKomputer.Text))

                cmd.ExecuteNonQuery()
            End Using
            totalDebet += nominalTransfer
            totalKredit += nominalTransfer
            Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "J2", "Transfer", CmbAkunTransfer.Text, NAMA_REK_BARANG, nominalTransfer, nominalTransfer))
        End If

        ' Final Summary Debug
        Debug.WriteLine(New String("-"c, 115))
        Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "TOT", "GRAND TOTAL", "", "", totalDebet, totalKredit))
        Debug.WriteLine(New String("═"c, 115))
        If totalDebet = totalKredit Then
            Debug.WriteLine("✅ JURNAL SEIMBANG - Debet = Kredit = " & totalDebet.ToString("N0"))
        Else
            Debug.WriteLine("❌ JURNAL TIDAK SEIMBANG! Selisih: " & (totalDebet - totalKredit).ToString("N0"))
        End If
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════════════════")
    End Sub

#End Region

    ' ============================================
    ' FUNGSI BANTUAN DAN UTILITAS
    ' ============================================
#Region "Fungsi Bantuan"

    ' ============================================
    ' FUNGSI: GENERATE NOMOR RETUR
    ' ============================================
    Private Sub NomorRetur()
        Try
            Using cmd As New MySqlCommand(
                "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
                cmd.Parameters.AddWithValue("@prefix", "RB")
                cmd.Parameters.AddWithValue("@tgl", DTPTgl.Value.Date)
                cmd.Parameters.AddWithValue("@tabel", "retur_pembelian")
                cmd.Parameters.AddWithValue("@kolom", "ID_RETUR_PEMBELIAN")
                Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
                pNomor.Direction = ParameterDirection.Output
                cmd.ExecuteNonQuery()
                TxtFaktur.Text = pNomor.Value?.ToString()
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error generate nomor retur: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DTPTgl_ValueChanged(sender As Object, e As EventArgs) Handles DTPTgl.ValueChanged
        If LblJenisTrans.Text = "TambahReturBeli" Then
            NomorRetur()
        End If
    End Sub

    ' ============================================
    ' FUNGSI: AMBIL DATA UNTUK EDIT TRANSAKSI
    ' ============================================
    Private Sub AmbilDataUntukEdit()
        Try
            ' ================= HEADER =================
            Dim queryString As String =
            "SELECT TGL_RETUR_BELI, NAMA_SUPPLIER, ALASAN_RETUR, ID_USER, ID_KOMPUTER, " &
            "JENIS_PENGEMBALIAN, NAMA_REKENING, KODE_REKENING, " &
            "NAMA_REKENING_TRANSFER, KODE_REKENING_TRANSFER, " &
            "NOMINAL_TUNAI, NOMINAL_TRANSFER, TOTAL_RUPIAH " &
            "FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN"

            Using cmd As New MySqlCommand(queryString, conn)
                cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then

                        ' Tanggal
                        If Not IsDBNull(rd("TGL_RETUR_BELI")) Then
                            DTPTgl.Value = Convert.ToDateTime(rd("TGL_RETUR_BELI"))
                            TanggalAsliEdit = DTPTgl.Value  ' simpan tanggal asli dari DB
                        End If

                        TxtSupplier.Text = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_SUPPLIER", "")
                        RTBAlasanRetur.Text = ModuleAngka.SafeGetValue(Of String)(rd, "ALASAN_RETUR", "")
                        TxtLogin.Text = ModuleAngka.SafeGetValue(Of String)(rd, "ID_USER", "")
                        TxtKomputer.Text = ModuleAngka.SafeGetValue(Of String)(rd, "ID_KOMPUTER", "")

                        ' Split bayar
                        ' Kolom lama (NAMA_REKENING, KODE_REKENING) digunakan untuk tunai
                        ' Kolom baru (NAMA_REKENING_TRANSFER, KODE_REKENING_TRANSFER) untuk transfer
                        Dim nominalTunai As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINAL_TUNAI", 0D)
                        Dim nominalTransfer As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINAL_TRANSFER", 0D)
                        Dim namaRekeningTunai As String = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_REKENING", "")
                        Dim kodeRekeningTunai As String = ModuleAngka.SafeGetValue(Of String)(rd, "KODE_REKENING", "")
                        Dim namaRekeningTransfer As String = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_REKENING_TRANSFER", "")
                        Dim kodeRekeningTransfer As String = ModuleAngka.SafeGetValue(Of String)(rd, "KODE_REKENING_TRANSFER", "")

                        ' Jika kolom split bayar belum ada (data lama), gunakan kolom lama
                        If nominalTunai = 0 AndAlso nominalTransfer = 0 Then
                            Dim jenisPengembalian As String = ModuleAngka.SafeGetValue(Of String)(rd, "JENIS_PENGEMBALIAN", "Tunai")
                            Dim totalRupiah As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TOTAL_RUPIAH", 0D)

                            If jenisPengembalian.ToLower().Contains("transfer") Then
                                nominalTransfer = totalRupiah
                                namaRekeningTransfer = namaRekeningTunai
                                kodeRekeningTransfer = kodeRekeningTunai
                                namaRekeningTunai = ""
                                kodeRekeningTunai = ""
                            Else
                                nominalTunai = totalRupiah
                                ' NAMA_REKENING dan KODE_REKENING sudah terisi dari kolom lama
                            End If
                        End If

                        ' Set nilai ke form
                        TxtNominalBayarTunai.Text = nominalTunai.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
                        TxtNominalBayarTransfer.Text = nominalTransfer.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

                        ' Set combo tunai — pilih item yang cocok, fallback ke .Text jika tidak ada
                        Dim idxTunai As Integer = CmbAkunTunai.FindStringExact(namaRekeningTunai)
                        If idxTunai >= 0 Then
                            CmbAkunTunai.SelectedIndex = idxTunai
                        Else
                            CmbAkunTunai.Text = namaRekeningTunai
                        End If
                        TxtKodeAkunTunai.Text = kodeRekeningTunai

                        ' Set combo transfer — pilih item yang cocok, fallback ke .Text jika tidak ada
                        Dim idxTransfer As Integer = CmbAkunTransfer.FindStringExact(namaRekeningTransfer)
                        If idxTransfer >= 0 Then
                            CmbAkunTransfer.SelectedIndex = idxTransfer
                        Else
                            CmbAkunTransfer.Text = namaRekeningTransfer
                        End If
                        TxtKodeAkunTransfer.Text = kodeRekeningTransfer

                    End If
                End Using
            End Using

            ' ================= DETAIL =================
            AmbilDataDetailUntukEdit()

            ' Refresh stok semua baris setelah DGV terisi
            RefreshStokSemuaBaris()

            ' Atur fokus ke grid atau TxtNama berdasarkan setting
            SetupFocusToGrid()

        Catch ex As Exception
            MessageBox.Show(
            "Error mengambil data untuk edit:" & Environment.NewLine & ex.Message,
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)
        End Try
    End Sub


    ' ============================================
    ' FUNGSI: AMBIL DATA DETAIL UNTUK EDIT
    ' ============================================
    Private Sub AmbilDataDetailUntukEdit()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            DgvData.Rows.Clear()

            ' Ambil data detail retur
            Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, " &
                                         "SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, " &
                                         "STOK_TOKO, STOK_GUDANG " &
                                         "FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN", conn)
                cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)
                cmd.Transaction = transaction

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Do While rd.Read()
                        Dim row As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
                        row.Cells("ID_BARANG").Value = rd("ID_BARANG").ToString()
                        row.Cells("NAMA_BARANG").Value = rd("NAMA_BARANG").ToString()
                        row.Cells("HARGA_BELI_TERAKHIR").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                        row.Cells("QTY").Value = ModuleAngka.ParseDecimal(rd("QTY"))
                        row.Cells("SATUAN").Value = rd("SATUAN").ToString()
                        row.Cells("ISI_SATUAN").Value = CInt(Math.Max(1, ModuleAngka.ParseDecimal(rd("ISI_SATUAN"))))
                        row.Cells("HARGA_BELI_SATUAN").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SATUAN"))
                        row.Cells("QTY_SAT").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                        row.Cells("TOTAL").Value = ModuleAngka.ParseDecimal(rd("TOTAL"))
                        row.Cells("StokToko").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                        row.Cells("StokGudang").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)

                        ' Isi ComboBox satuan dari cache
                        Dim idBarang As String = row.Cells("ID_BARANG").Value.ToString()
                        Dim comboCell As DataGridViewComboBoxCell = CType(row.Cells("SATUAN"), DataGridViewComboBoxCell)
                        comboCell.Items.Clear()

                        Dim barangFromCache = GetBarangFromCache(idBarang)
                        If barangFromCache IsNot Nothing Then
                            If Not String.IsNullOrEmpty(barangFromCache.SATUAN_UMUM_KECIL) Then
                                comboCell.Items.Add(barangFromCache.SATUAN_UMUM_KECIL)
                            End If
                            If Not String.IsNullOrEmpty(barangFromCache.SATUAN_UMUM_SEDANG) Then
                                comboCell.Items.Add(barangFromCache.SATUAN_UMUM_SEDANG)
                            End If
                            If Not String.IsNullOrEmpty(barangFromCache.SATUAN_UMUM_BESAR) Then
                                comboCell.Items.Add(barangFromCache.SATUAN_UMUM_BESAR)
                            End If
                        End If
                    Loop
                End Using
            End Using

            transaction.Commit()

            ' Set ReadOnly kolom NAMA_BARANG untuk semua baris yang sudah terisi — konsisten dengan FormJual/Pembelian
            For i As Integer = 0 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    SetWarnaReadOnlyNama(i)
                End If
            Next

            UpdateSemuaTotal()
            AturFokusAwal()

        Catch ex As Exception
            MessageBox.Show("Masalah saat mengambil data. Jenis kesalahan: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            transaction.Rollback()
        End Try
    End Sub

#End Region

    ' ============================================
    ' EVENT HANDLER TOMBOL DAN FORM
    ' ============================================
#Region "Event Handler Tombol"

    ' ============================================
    ' EVENT: CLICK BUTTON KELUAR DAN CLOSE
    ' ============================================
    Private Sub BtnKeluarForm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluarForm.Click
        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
        Close()
    End Sub



#End Region

    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "ReturBeli"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class
