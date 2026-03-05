Public Class FormReturBeli
    ' ============================================
    ' DEKLARASI VARIABEL DAN KONSTANTA
    ' ============================================
#Region "Deklarasi Variabel dan Cache"

    Private ReadOnly LokasiBarang As String
    Private AwalReturBeli As String
    Private SatuanReturBeli As String
    Private ReturBeliStokMinus As String
    Private NavigasiSetelahNama As String
    Private namaBarangLookupCache As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private listBarangOriginalLocation As Point
    Private currentGridRow As Integer
    Private TransaksiLampau As String

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

        ' ============================================
        ' PROPERTY UNTUK MENGAMBIL STOK LOKASI SEKARANG
        ' ============================================
        Public ReadOnly Property STOK_LOKASI As Decimal
            Get
                ' Catatan: Property ini tidak bisa digunakan langsung karena membutuhkan parameter lokasi
                ' Sebaiknya gunakan GetStokByLokasi(lokasi) dengan parameter eksplisit
                Return 0
            End Get
        End Property
    End Class

    ' ============================================
    ' KELAS DATA ITEM UNTUK LISTBOX
    ' ============================================
    Public Class ListBarangItem
        Public Property NamaBarang As String
        Public Property Stok As Decimal
        Public Property ID_Barang As String

        Public Sub New(nama As String, stok As Decimal, id As String)
            Me.NamaBarang = nama
            Me.Stok = stok
            Me.ID_Barang = id
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
    Private WithEvents barcodeTimer As New Timer()
    Private Const BARCODE_TIMEOUT_MS As Integer = 100
    Private isBarcodeMode As Boolean = False

    ' ============================================
    ' VARIABEL DEBOUNCED SEARCH
    ' ============================================
    Private WithEvents searchDebounceTimer As New Timer()
    Private lastSearchText As String = String.Empty
    Private Const SEARCH_DEBOUNCE_MS As Integer = 300

    ' ============================================
    ' VARIABEL CACHE REFRESH TIMER
    ' ============================================
    Private WithEvents cacheRefreshTimer As New Timer()

#End Region

    ' ============================================
    ' EVENT HANDLER - LOAD, KEYBOARD DAN CLOSING
    ' ============================================
#Region "Event Handler Form"

    ' ============================================
    ' EVENT: FORM LOAD - INISIALISASI AWAL
    ' ============================================
    Private Sub FormReturBeli_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
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

    ' ============================================
    ' EVENT: KEY DOWN - HANDLER SHORTCUT KEYBOARD
    ' ============================================
    Private Sub Form_Pembelian_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
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
                    CmbRekening.Focus()
                    CmbRekening.DroppedDown = True
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
        Debug.WriteLine($"Kolom tambahan {status}")
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

        With PrintTransferBarang
            .TxtNota.Text = TxtFaktur.Text
            .ProsesCetak()
        End With
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
        LblLokasiBarang.Text = FormUtama.SLokasi.Text

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
        CmbRekening.Items.Clear()
        CmbRekening.Items.AddRange(GetDaftarAkun().ToArray())

        ' Set rekening default berdasarkan lokasi
        If LblLokasiBarang.Text = "TOKO" Then
            CmbRekening.SelectedItem = nama_rek_Beli_toko
        ElseIf LblLokasiBarang.Text = "GUDANG" Then
            CmbRekening.SelectedItem = nama_rek_Beli_Gudang
        End If

        ' Simpan posisi asli ListBarang
        listBarangOriginalLocation = LstBarang.Location

        ' Setup DataGridView
        'SetupDataGridViewNavigation()

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
        AwalReturBeli = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblReturBeliFokus.Text)
        SatuanReturBeli = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblReturBeliSatuan.Text)
        ReturBeliStokMinus = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblReturBeliMinus.Text)
        TransaksiLampau = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransaksiTanggalLampau.Text)
    End Sub

    ' ============================================
    ' FUNGSI: ATUR FOKUS AWAL
    ' ============================================
    Private Sub AturFokusAwal()
        If DgvData.Rows.Count > 0 Then
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        End If

        If AwalReturBeli = "Pencarian" Then
            TxtNama.Select()
        End If
    End Sub

    ' ============================================
    ' FUNGSI: KONDISI AWAL UNTUK TRANSAKSI BARU
    ' ============================================
    Private Sub Kondisiawal()
        ' Reset data grid
        TxtSupplier.Clear()
        DgvData.Rows.Clear()
        TxtTotalQTY.Text = "0"
        TxtGrandtotal.Text = "0"
        GBBayar.Visible = False

        ' Setup tanggal
        DTPTgl.Value = DateTime.Now
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
        ' Kosongkan dan sembunyikan list barang
        LstBarang.Items.Clear()
        LstBarang.Visible = False
        TxtQty.Text = "1"

        ' Fokus kembali ke input pencarian
        TxtNama.Focus()
        TxtNama.SelectAll()
    End Sub

    ' ============================================
    ' FUNGSI: CLEANUP RESOURCES SAAT FORM CLOSING
    ' ============================================
    Private Sub CleanupResources()
        ' Stop dan dispose semua timer
        If barcodeTimer IsNot Nothing Then
            barcodeTimer.Stop()
            barcodeTimer.Dispose()
        End If

        If searchDebounceTimer IsNot Nothing Then
            searchDebounceTimer.Stop()
            searchDebounceTimer.Dispose()
        End If

        If cacheRefreshTimer IsNot Nothing Then
            cacheRefreshTimer.Stop()
            cacheRefreshTimer.Dispose()
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
        RemoveHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick
        RemoveHandler searchDebounceTimer.Tick, AddressOf SearchDebounceTimer_Tick
        RemoveHandler cacheRefreshTimer.Tick, AddressOf CacheRefreshTimer_Tick
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
        cacheRefreshTimer.Interval = 300000 ' 5 menit
        AddHandler cacheRefreshTimer.Tick, AddressOf CacheRefreshTimer_Tick
        cacheRefreshTimer.Start()
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
                        Dim isiKecil As Integer = If(rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_KECIL")), 0, Convert.ToInt32(rd("ISI_UMUM_KECIL")))
                        Dim isiSedang As Integer = If(rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_SEDANG")), 0, Convert.ToInt32(rd("ISI_UMUM_SEDANG")))
                        Dim isiBesar As Integer = If(rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_BESAR")), 0, Convert.ToInt32(rd("ISI_UMUM_BESAR")))

                        ' Handle nullable decimal conversions
                        Dim hargaBeli As Decimal = If(rd.IsDBNull(rd.GetOrdinal("HARGA_BELI_TERAKHIR")), 0D, Convert.ToDecimal(rd("HARGA_BELI_TERAKHIR")))
                        Dim stokToko As Decimal = If(rd.IsDBNull(rd.GetOrdinal("STOK_TOKO")), 0D, Convert.ToDecimal(rd("STOK_TOKO")))
                        Dim stokGudang As Decimal = If(rd.IsDBNull(rd.GetOrdinal("STOK_GUDANG")), 0D, Convert.ToDecimal(rd("STOK_GUDANG")))

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
            Debug.WriteLine($"Cache loaded: {barcodeLookupCache.Count} barcodes, {barangCacheById.Count} items")

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
            Debug.WriteLine($"Cache refresh failed: {ex.Message}")
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
        barcodeTimer.Interval = BARCODE_TIMEOUT_MS
        barcodeTimer.Stop()
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick

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
            ' PANAH BAWAH: Pindah ke ListBarang dan PILIH ITEM PERTAMA
            If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
                LstBarang.Focus()
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
        barcodeTimer.Stop()
        barcodeTimer.Start()
    End Sub

    ' ============================================
    ' EVENT: BARCODE TIMER TICK
    ' ============================================
    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        barcodeTimer.Stop()

        ' Jika timer expired dan ada input, proses sebagai barcode
        If Not String.IsNullOrEmpty(TxtNama.Text) AndAlso isBarcodeMode Then
            ProcessBarcodeInput(TxtNama.Text.Trim())
            ResetBarcodeInput()
        End If
    End Sub

    ' ============================================
    ' FUNGSI: PROCESS BARCODE INPUT
    ' ============================================
    Private Sub ProcessBarcodeInput(barcode As String)
        ' Cek di cache terlebih dahulu - O(1) operation!
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
            If SatuanReturBeli = "Tidak" Then
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

        UpdateSemuaTotal()
        KosongTxtboxcari()
    End Sub

    ' ============================================
    ' FUNGSI: RESET BARCODE INPUT
    ' ============================================
    Private Sub ResetBarcodeInput()
        TxtNama.Clear()
        TxtNama.Focus()
        lastBarcodeInput = String.Empty
        isBarcodeMode = False
        barcodeTimer.Stop()
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
        .ISI_UMUM_KECIL = CInt(rd("ISI_UMUM_KECIL")),
        .ISI_UMUM_SEDANG = CInt(rd("ISI_UMUM_SEDANG")),
        .ISI_UMUM_BESAR = CInt(rd("ISI_UMUM_BESAR")),
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
            Return CInt(rd("ISI_UMUM_KECIL"))
        ElseIf barcode = rd("BARCODE_SEDANG").ToString() Then
            Return CInt(rd("ISI_UMUM_SEDANG"))
        ElseIf barcode = rd("BARCODE_BESAR").ToString() Then
            Return CInt(rd("ISI_UMUM_BESAR"))
        Else
            Return CInt(rd("ISI_UMUM_KECIL"))
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
        searchDebounceTimer.Interval = SEARCH_DEBOUNCE_MS
        searchDebounceTimer.Stop()
        AddHandler searchDebounceTimer.Tick, AddressOf SearchDebounceTimer_Tick

        ' Setup event handler untuk pencarian manual
        AddHandler TxtNama.TextChanged, AddressOf TxtNama_TextChanged_Debounced
    End Sub

    ' ============================================
    ' EVENT: GOT FOCUS UNTUK TEXTBOX NAMA
    ' ============================================
    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        PanelCariNama.BackColor = Color.Yellow

        'If DgvData.Rows.Count > 0 Then
        '    DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)
        '    DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        'End If
    End Sub

    ' ============================================
    ' EVENT: LOST FOCUS UNTUK TEXTBOX NAMA
    ' ============================================
    Private Sub TxtNama_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        PanelCariNama.BackColor = SystemColors.ActiveCaption
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED DEBOUNCED UNTUK PENCARIAN
    ' ============================================
    Private Sub TxtNama_TextChanged_Debounced(sender As Object, e As EventArgs) Handles TxtNama.TextChanged
        ' Skip jika dalam mode barcode
        If isBarcodeMode Then Exit Sub

        lastSearchText = TxtNama.Text
        searchDebounceTimer.Stop()
        searchDebounceTimer.Start()
    End Sub

    ' ============================================
    ' EVENT: SEARCH DEBOUNCE TIMER TICK
    ' ============================================
    Private Sub SearchDebounceTimer_Tick(sender As Object, e As EventArgs)
        searchDebounceTimer.Stop()

        If String.IsNullOrEmpty(lastSearchText) Then
            BersihkanPencarian()
            Exit Sub
        End If

        ' Parse qty dan keyword
        Dim parsedResult = ParseSearchInput(lastSearchText)

        If parsedResult.Keyword.Length >= 2 Then
            SearchBarangByText(parsedResult.Keyword, parsedResult.Qty)
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
                results.Add(New ListBarangItem(barang.NAMA_BARANG, stok, idBarang))
            End If
        Else
            ' Cari di cache by nama - OPTIMASI DENGAN DICTIONARY
            Dim keywordLower = keyword.ToLower()

            ' Gunakan LINQ yang lebih efisien
            results = barangCacheById.Values.
            AsParallel().  ' Paralel processing untuk dataset besar
            Where(Function(b) b.NAMA_BARANG.ToLower().Contains(keywordLower)).
            Select(Function(b) New ListBarangItem(b.NAMA_BARANG, b.GetStokByLokasi(lokasi), b.ID_BARANG)).
            OrderByDescending(Function(r) r.Stok >= 0).  ' Stok positif dulu
            ThenBy(Function(r) r.NamaBarang).
            Take(50).  ' Batasi hasil
            ToList()
        End If

        ' Update UI
        Me.Invoke(Sub()
                      LstBarang.Items.Clear()

                      If results.Count = 0 Then
                          LstBarang.Visible = False
                          TxtNama.Focus()
                          Exit Sub
                      End If

                      For Each item In results
                          LstBarang.Items.Add(item)
                      Next

                      AturTinggiListBarang()
                      LstBarang.Visible = True
                  End Sub)
    End Sub

    ' ============================================
    ' FUNGSI: ATUR TINGGI LIST BARANG
    ' ============================================
    Private Sub AturTinggiListBarang()
        Dim baris As Integer = LstBarang.Items.Count
        If baris = 0 Then
            LstBarang.Height = 0
            Return
        End If

        Dim tinggiBaris As Integer = LstBarang.ItemHeight
        Dim maxHeight As Integer = 300

        If baris <= 10 Then
            LstBarang.Height = Math.Min(baris * tinggiBaris + 4, maxHeight)
        Else
            LstBarang.Height = maxHeight
            LstBarang.ScrollAlwaysVisible = True
        End If
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

            Using cmd As New MySqlCommand("SELECT KODE, NAMA, ALAMAT, HP FROM tbl_supliyer", conn)
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
            ' Enter untuk pilih supplier jika list tampil
            If listSupplier.Items.Count > 0 Then
                listSupplier.Focus()
                listSupplier.SelectedIndex = 0
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Enter Then
            ' Jika tidak ada list, fokus ke pencarian barang
            Fokuskepencarianbarang()
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ============================================
    ' EVENT: KEY DOWN UNTUK LIST SUPPLIER
    ' ============================================
    Private Sub listSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles listSupplier.KeyDown
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
    Private Sub listSupplier_GotFocus(sender As Object, e As EventArgs) Handles listSupplier.GotFocus
        ' Pastikan ada item yang terpilih saat mendapat fokus
        If listSupplier.Items.Count > 0 AndAlso listSupplier.SelectedIndex = -1 Then
            listSupplier.SelectedIndex = 0
        End If
    End Sub


    ' ============================================
    ' EVENT: CLICK UNTUK LIST SUPPLIER
    ' ============================================
    Private Sub listSupplier_Click(sender As Object, e As EventArgs) Handles listSupplier.Click
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
        If LstBarang.SelectedItem IsNot Nothing AndAlso LstBarang.SelectedIndex >= 0 Then
            Dim selectedItem = TryCast(LstBarang.SelectedItem, ListBarangItem)
            Dim namaYangDiambil As String

            If selectedItem IsNot Nothing Then
                namaYangDiambil = selectedItem.NamaBarang
            Else
                ' Untuk backward compatibility
                namaYangDiambil = GetTextAfterAsterisk(LstBarang.SelectedItem.ToString())
                ' Bersihkan dari format stok jika ada
                namaYangDiambil = ListBarangItem.ExtractNamaBarang(namaYangDiambil)
            End If

            ' Cek stok sebelum melanjutkan
            Dim barang = barangCacheById.Values.FirstOrDefault(
                Function(b) b.NAMA_BARANG = namaYangDiambil)

            If barang IsNot Nothing AndAlso ReturBeliStokMinus = "Tidak" Then
                Dim stokTersedia = barang.GetStokByLokasi(LblLokasiBarang.Text)
                Dim qty As Decimal = 1

                ' Parse qty dari input TxtNama jika ada
                If TxtNama.Text.Contains("*") Then
                    Dim parts = TxtNama.Text.Split("*"c)
                    If parts.Length >= 1 AndAlso Decimal.TryParse(parts(0).Trim(), qty) Then
                        Dim isi = barang.ISI_UMUM_KECIL
                        Dim qtySat = qty * isi

                        If qtySat > stokTersedia Then
                            MessageBox.Show($"Stok tidak mencukupi!{vbCrLf}" &
                                      $"Stok tersedia: {stokTersedia:N0}{vbCrLf}" &
                                      $"Qty yang diminta: {qtySat:N0}",
                                      "Peringatan Stok",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning)
                            Return
                        End If
                    End If
                End If
            End If

            Ambildatalaindaridbbarang(namaYangDiambil)
            TambahDataLangsung(namaYangDiambil)

            ' Setelah mengambil data, fokus kembali ke TxtNama
            TxtNama.Focus()
            TxtNama.SelectAll()
        End If
    End Sub

    ' ============================================
    ' EVENT: GOT FOCUS UNTUK LIST BARANG
    ' ============================================
    Private Sub LstBarang_GotFocus(sender As Object, e As EventArgs) Handles LstBarang.GotFocus
        ' Pastikan ada item yang terpilih saat mendapat fokus
        If LstBarang.Items.Count > 0 AndAlso LstBarang.SelectedIndex = -1 Then
            LstBarang.SelectedIndex = 0
        End If
    End Sub

    ' ============================================
    ' EVENT: LOST FOCUS UNTUK LIST BARANG
    ' ============================================
    Private Sub LstBarang_LostFocus(sender As Object, e As EventArgs) Handles LstBarang.LostFocus
        ' Opsional: Sembunyikan list barang saat kehilangan fokus
    End Sub


    ' ============================================
    ' EVENT: MOUSE CLICK UNTUK LIST BARANG
    ' ============================================


    Private Sub LstBarang_MouseClick(sender As Object, e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedIndex < 0 Then Exit Sub

        ' Mode pencarian normal dari TxtNama
        AmbilDataDariListBox()

        ' Setelah pilih, tutup list
        LstBarang.Visible = False
    End Sub

    Private Sub LstBarang_Enter(sender As Object, e As EventArgs) Handles LstBarang.Enter
        ' Saat masuk ke ListBarang, pastikan ada item yang terpilih
        If LstBarang.Items.Count > 0 AndAlso LstBarang.SelectedIndex = -1 Then
            LstBarang.SelectedIndex = 0
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
            TxtHarga.Text = barangFromCache.HARGA_BELI_TERAKHIR.ToString()
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
                        Dim idBarang As String = If(Not IsDBNull(rd(0)), rd.GetString(0), String.Empty)
                        Dim hargaBeli As String = If(Not IsDBNull(rd(2)), rd.GetDecimal(2).ToString(), String.Empty)

                        Dim satuanUmum As String = If(Not IsDBNull(rd(3)), rd.GetString(3), String.Empty)
                        Dim isiUmum As Integer = If(Not IsDBNull(rd(6)), rd.GetInt32(6), 0)

                        ' Jika ada barcode, tentukan satuan berdasarkan barcode
                        If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
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

                        ' Jika isi = 0, set ke 1
                        If isiUmum = 0 Then
                            isiUmum = 1
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
            If SatuanReturBeli = "Tidak" Then
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
            Dim hargaBeli As Decimal = Decimal.Parse(TxtHarga.Text)
            Dim qty As Decimal = If(Decimal.TryParse(TxtQty.Text, qty), qty, 1)
            Dim satuan As String = Txtsatuan.Text
            Dim isi As Decimal = Decimal.Parse(TxtIsi.Text)

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

            UpdateSemuaTotal()
            KosongTxtboxcari()

        Catch ex As Exception
            MessageBox.Show($"Error menambah data barang: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================
    ' FUNGSI: FOKUS KE PENCARIAN BARANG
    ' ============================================
    Private Sub Fokuskepencarianbarang()
        If AwalReturBeli = "Pencarian" Then
            TxtNama.Select()
        Else
            Dim rowKosong As Integer = -1

            ' Cari baris kosong
            For i As Integer = 0 To DgvData.Rows.Count - 1
                Dim nama As String = ""

                If Not IsDBNull(DgvData.Rows(i).Cells("NAMA_BARANG").Value) Then
                    nama = Trim(DgvData.Rows(i).Cells("NAMA_BARANG").Value.ToString())
                End If

                If nama = "" Then
                    rowKosong = i
                    Exit For
                End If
            Next

            ' Jika tidak ada baris kosong, tambah baris baru
            If rowKosong = -1 Then
                rowKosong = DgvData.Rows.Add()
            End If

            ' Fokus ke kolom nama barang
            DgvData.CurrentCell = DgvData.Rows(rowKosong).Cells("NAMA_BARANG")

            ' Tentukan fokus berdasarkan setting
            If AwalReturBeli = "Pencarian" Then
                TxtNama.Select()
            Else
                DgvData.Select()
            End If
        End If
    End Sub

    ' ============================================
    ' FUNGSI: HAPUS BARIS DARI GRID
    ' ============================================
    Private Sub Hapusbaris()
        Dim baris As Integer = DgvData.CurrentCell.RowIndex
        DgvData.Rows.RemoveAt(baris)
        UpdateSemuaTotal()
    End Sub

    ' ============================================
    ' EVENT: CLICK MENU HAPUS TOOLSTRIP
    ' ============================================
    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
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
        Try
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
            Debug.WriteLine($"Error saat edit sel: {ex.Message}")
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
        Try
            Dim cellValue As Object = DgvData.Rows(rowIndex).Cells("NAMA_BARANG").Value
            If cellValue Is Nothing OrElse String.IsNullOrEmpty(cellValue.ToString().Trim()) Then
                ClearGridRow(rowIndex)
                Return
            End If

            Dim inputValue As String = cellValue.ToString().Trim()

            ' VARIABEL PARSING
            Dim qty As Decimal = 1
            Dim searchKey As String = inputValue

            ' PARSE FORMAT "QTY*NAMA" atau "QTY*BARCODE"
            Dim asteriskIndex As Integer = inputValue.IndexOf("*")

            If asteriskIndex > 0 Then
                Dim qtyPart = inputValue.Substring(0, asteriskIndex).Trim()
                Dim keywordPart = inputValue.Substring(asteriskIndex + 1).Trim()

                If Decimal.TryParse(qtyPart, qty) AndAlso qty > 0 Then
                    searchKey = keywordPart
                Else
                    qty = 1
                    searchKey = inputValue
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
                If barangFromCache Is Nothing Then
                    barangFromCache = barangCacheById.Values.
                    FirstOrDefault(Function(b) b.NAMA_BARANG.ToLower().Contains(searchKey.ToLower()))
                End If
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

            ' TENTUKAN SATUAN
            Dim satuan As String = barangFromCache.SATUAN_UMUM_KECIL
            Dim isi As Integer = barangFromCache.ISI_UMUM_KECIL

            ' Jika input adalah barcode, ambil satuan dari cache barcode
            If barcodeLookupCache.ContainsKey(searchKey) AndAlso barcodeToSatuanCache.ContainsKey(searchKey) Then
                satuan = barcodeToSatuanCache(searchKey).Item1
                isi = barcodeToSatuanCache(searchKey).Item2
            End If

            ' Jika isi = 0, set ke 1
            If isi = 0 Then isi = 1

            ' UPDATE BARIS DI DATAGRIDVIEW
            UpdateGridRowFromBarang(rowIndex, barangFromCache, satuan, isi, qty)


            ' Navigasi setelah edit nama barang
            If NavigasiSetelahNama = "Harga" Then
                ' Pindah ke HARGA_BELI_TERAKHIR di baris yang sama
                BeginInvoke(New Action(Sub()
                                           If rowIndex < DgvData.Rows.Count Then
                                               DgvData.CurrentCell = DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR")
                                               DgvData.BeginEdit(True)
                                           End If
                                       End Sub))
            Else
                ' Pindah ke NAMA_BARANG di baris berikutnya
                BeginInvoke(New Action(Sub()
                                           Dim nextRow As Integer = rowIndex + 1

                                           ' Tambah baris baru jika perlu
                                           If nextRow >= DgvData.Rows.Count Then
                                               DgvData.Rows.Add()
                                           End If

                                           ' Pindah ke baris baru
                                           If nextRow < DgvData.Rows.Count Then
                                               DgvData.CurrentCell = DgvData.Rows(nextRow).Cells("NAMA_BARANG")
                                               DgvData.BeginEdit(True)
                                           End If
                                       End Sub))
            End If


        Catch ex As Exception
            MessageBox.Show($"Error memproses nama barang: {ex.Message}", "Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ============================================'
    ' FUNGSI: UPDATE GRID ROW FROM BARANG
    ' ============================================'
    Private Sub UpdateGridRowFromBarang(rowIndex As Integer, barang As DataBarang, satuan As String, isi As Integer, qty As Decimal)
        ' Update DataGridView
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

        ' Format tampilan
        DgvData.Columns("HARGA_BELI_TERAKHIR").DefaultCellStyle.Format = "N0"
        DgvData.Columns("HARGA_BELI_TERAKHIR").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DgvData.Columns("TOTAL").DefaultCellStyle.Format = "N0"
        DgvData.Columns("TOTAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Tampilkan info stok
        TampilkanInfoStokDiGrid(rowIndex, barang)

        ' Update total
        UpdateSemuaTotal()
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

    ' ============================================
    ' EVENT: DRAW ITEM UNTUK LIST BARANG
    ' ============================================
    Private Sub LstBarang_DrawItem(sender As Object, e As DrawItemEventArgs) Handles LstBarang.DrawItem
        ' Custom drawing untuk menonjolkan stok rendah
        If e.Index < 0 Then Exit Sub

        e.DrawBackground()

        Dim item = TryCast(LstBarang.Items(e.Index), ListBarangItem)
        Dim text As String
        Dim brushColor As Brush

        If item IsNot Nothing Then
            text = item.ToString()

            ' Warna berdasarkan stok
            If item.Stok <= 0 Then
                brushColor = Brushes.Red
            ElseIf item.Stok < 10 Then
                brushColor = Brushes.Orange
            Else
                brushColor = Brushes.Black
            End If
        Else
            text = LstBarang.Items(e.Index).ToString()
            brushColor = Brushes.Black
        End If

        ' Draw text
        e.Graphics.DrawString(text, LstBarang.Font, brushColor, e.Bounds)

        ' Draw focus rectangle
        e.DrawFocusRectangle()
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
                            .HARGA_BELI_TERAKHIR = If(rd.IsDBNull(rd.GetOrdinal("HARGA_BELI_TERAKHIR")), 0D, Convert.ToDecimal(rd("HARGA_BELI_TERAKHIR"))),
                            .SATUAN_UMUM_KECIL = rd("SATUAN_UMUM_KECIL").ToString(),
                            .SATUAN_UMUM_SEDANG = rd("SATUAN_UMUM_SEDANG").ToString(),
                            .SATUAN_UMUM_BESAR = rd("SATUAN_UMUM_BESAR").ToString(),
                            .ISI_UMUM_KECIL = If(rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_KECIL")), 0, Convert.ToInt32(rd("ISI_UMUM_KECIL"))),
                            .ISI_UMUM_SEDANG = If(rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_SEDANG")), 0, Convert.ToInt32(rd("ISI_UMUM_SEDANG"))),
                            .ISI_UMUM_BESAR = If(rd.IsDBNull(rd.GetOrdinal("ISI_UMUM_BESAR")), 0, Convert.ToInt32(rd("ISI_UMUM_BESAR"))),
                            .BARCODE_KECIL = rd("BARCODE_KECIL").ToString(),
                            .BARCODE_SEDANG = rd("BARCODE_SEDANG").ToString(),
                            .BARCODE_BESAR = rd("BARCODE_BESAR").ToString(),
                            .STOK_TOKO = If(rd.IsDBNull(rd.GetOrdinal("STOK_TOKO")), 0D, Convert.ToDecimal(rd("STOK_TOKO"))),
                            .STOK_GUDANG = If(rd.IsDBNull(rd.GetOrdinal("STOK_GUDANG")), 0D, Convert.ToDecimal(rd("STOK_GUDANG")))
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
            Debug.WriteLine($"Error cari barang di database: {ex.Message}")
        End Try

        Return Nothing
    End Function

    ' ============================================'
    ' FUNGSI: UPDATE BARIS DARI CACHE
    ' ============================================'
    Private Sub UpdateBarisDariCache(rowIndex As Integer, barang As DataBarang, inputValue As String, isBarcodeInput As Boolean)
        ' Set nilai dasar
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
        If isi = 0 Then isi = 1

        ' Set nilai ke grid
        DgvData.Rows(rowIndex).Cells("SATUAN").Value = satuan
        DgvData.Rows(rowIndex).Cells("ISI_SATUAN").Value = isi
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_SATUAN").Value = barang.HARGA_BELI_TERAKHIR * isi
        DgvData.Rows(rowIndex).Cells("QTY").Value = 1 ' Default QTY
        DgvData.Rows(rowIndex).Cells("QTY_SAT").Value = 1 * isi
        DgvData.Rows(rowIndex).Cells("TOTAL").Value = barang.HARGA_BELI_TERAKHIR * (1 * isi)

        ' Format harga
        Dim hargabeliColumn As DataGridViewColumn = DgvData.Columns("HARGA_BELI_TERAKHIR")
        hargabeliColumn.DefaultCellStyle.Format = "N0"
        hargabeliColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Format total
        Dim totalColumn As DataGridViewColumn = DgvData.Columns("TOTAL")
        totalColumn.DefaultCellStyle.Format = "N0"
        totalColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Cek duplikasi jika setting tidak mengizinkan
        If SatuanReturBeli = "Tidak" Then
            ProsesMergeBarisDuplikat(rowIndex)
        End If

        ' Update total setelah perubahan
        UpdateSemuaTotal()
    End Sub

    ' ============================================
    ' FUNGSI: UPDATE BARIS DARI DATABASE
    ' ============================================
    Private Sub UpdateBarisDariDatabase(rowIndex As Integer, rd As MySqlDataReader, namaValue As String)
        ' Set nilai dasar
        DgvData.Rows(rowIndex).Cells("ID_BARANG").Value = rd("ID_BARANG")
        DgvData.Rows(rowIndex).Cells("HARGA_BELI_TERAKHIR").Value = rd("HARGA_BELI_TERAKHIR")

        ' Setup ComboBox satuan
        Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(rowIndex).Cells("SATUAN"), DataGridViewComboBoxCell)
        comboCell.Items.Clear()

        ' Ambil data satuan dari reader
        Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")),
                                       rd.GetString(rd.GetOrdinal("SATUAN_UMUM_KECIL")), "")
        Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")),
                                        rd.GetString(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
        Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")),
                                       rd.GetString(rd.GetOrdinal("SATUAN_UMUM_BESAR")), "")

        ' Tambahkan satuan yang tersedia
        If Not String.IsNullOrEmpty(satuanKecil) Then comboCell.Items.Add(satuanKecil)
        If Not String.IsNullOrEmpty(satuanSedang) Then comboCell.Items.Add(satuanSedang)
        If Not String.IsNullOrEmpty(satuanBesar) Then comboCell.Items.Add(satuanBesar)

        ' Tentukan satuan dan isi berdasarkan input
        Dim satuan As String = ""
        Dim isi As Integer = 1

        If namaValue = If(Not rd.IsDBNull(rd.GetOrdinal("NAMA_BARANG")), rd("NAMA_BARANG").ToString(), "") Or
           namaValue = If(Not rd.IsDBNull(rd.GetOrdinal("BARCODE_KECIL")), rd("BARCODE_KECIL").ToString(), "") Then
            satuan = rd("SATUAN_UMUM_KECIL")
            isi = rd("ISI_UMUM_KECIL")
        ElseIf namaValue = If(Not rd.IsDBNull(rd.GetOrdinal("BARCODE_SEDANG")), rd("BARCODE_SEDANG").ToString(), "") Then
            satuan = rd("SATUAN_UMUM_SEDANG")
            isi = rd("ISI_UMUM_SEDANG")
        ElseIf namaValue = If(Not rd.IsDBNull(rd.GetOrdinal("BARCODE_BESAR")), rd("BARCODE_BESAR").ToString(), "") Then
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
        If SatuanReturBeli = "Tidak" Then
            ProsesMergeBarisDuplikat(rowIndex)
        End If
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

                    ' Hapus baris duplikat
                    Call Hapusbaris()
                    SendKeys.Send("{down}")
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
                Dim hargabeliColumn As DataGridViewColumn = DgvData.Columns("HARGA_BELI_TERAKHIR")
                hargabeliColumn.DefaultCellStyle.Format = "N0"
                hargabeliColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
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

            ' UNTUK KOLOM SATUAN - HANYA ATTACH EVENT
            If DgvData.CurrentCell.ColumnIndex = 4 Then ' SATUAN
                Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)
                If comboBox IsNot Nothing Then
                    RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                    AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged

                    ' HAPUS KeyDown handler yang kompleks
                    RemoveHandler comboBox.KeyDown, AddressOf ComboBox_KeyDown
                    AddHandler comboBox.KeyDown, AddressOf ComboBox_KeyDown
                End If
            End If

            ' UNTUK KOLOM NAMA_BARANG - AutoComplete saja
            If DgvData.CurrentCell.ColumnIndex = 1 Then ' NAMA_BARANG
                Dim editText As TextBox = TryCast(e.Control, TextBox)
                If editText IsNot Nothing Then
                    editText.AutoCompleteMode = AutoCompleteMode.Suggest
                    editText.AutoCompleteSource = AutoCompleteSource.CustomSource

                    Dim autoCompleteCollection As New AutoCompleteStringCollection()
                    For Each barang In barangCacheById.Values
                        autoCompleteCollection.Add(barang.NAMA_BARANG)
                    Next
                    editText.AutoCompleteCustomSource = autoCompleteCollection
                End If
            End If

            ' HAPUS SEMUA KeyDown handler untuk HARGA_BELI_TERAKHIR dan QTY
            ' Biarkan DataGridView handle secara default

        Catch ex As Exception
            Debug.WriteLine($"Error in EditingControlShowing: {ex.Message}")
        End Try
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
    ' EVENT: KEY DOWN UNTUK LIST BARANG DI GRID MODE
    ' ============================================'
    Private Sub LstBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstBarang.KeyDown
        ' HANYA UNTUK PENCARIAN DARI TXTNAMA
        Select Case e.KeyCode
            Case Keys.Enter
                ' ENTER: Pilih item dari list
                If LstBarang.SelectedIndex >= 0 Then
                    AmbilDataDariListBox()
                End If
                e.SuppressKeyPress = True

            Case Keys.Escape
                ' ESC: Sembunyikan list dan kembali ke TxtNama
                LstBarang.Visible = False
                TxtNama.Focus()
                TxtNama.SelectAll()
                e.SuppressKeyPress = True

            Case Keys.Down
                ' PANAH BAWAH: Navigasi ke item berikutnya
                If LstBarang.SelectedIndex < LstBarang.Items.Count - 1 Then
                    LstBarang.SelectedIndex += 1
                End If
                e.SuppressKeyPress = True

            Case Keys.Up
                ' PANAH ATAS: Navigasi ke item sebelumnya
                If LstBarang.SelectedIndex > 0 Then
                    LstBarang.SelectedIndex -= 1
                Else
                    ' Jika di item pertama, kembali ke TxtNama
                    LstBarang.Visible = False
                    TxtNama.Focus()
                    TxtNama.SelectAll()
                End If
                e.SuppressKeyPress = True

            Case Keys.PageDown
                ' PAGE DOWN: Loncat 10 item
                Dim newIndex = Math.Min(LstBarang.SelectedIndex + 10, LstBarang.Items.Count - 1)
                LstBarang.SelectedIndex = newIndex
                e.SuppressKeyPress = True

            Case Keys.PageUp
                ' PAGE UP: Loncat 10 item ke atas
                Dim newIndex = Math.Max(LstBarang.SelectedIndex - 10, 0)
                LstBarang.SelectedIndex = newIndex
                e.SuppressKeyPress = True
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
                Case 0
                    isiSatuan = barangFromCache.ISI_UMUM_KECIL
                Case 1
                    isiSatuan = barangFromCache.ISI_UMUM_SEDANG
                Case Else
                    isiSatuan = barangFromCache.ISI_UMUM_BESAR
            End Select

            If isiSatuan = 0 Then isiSatuan = 1

            ' Update row - LANGSUNG tanpa event trigger
            Dim rowIndex As Integer = cell.RowIndex
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
            Debug.WriteLine($"Error in ComboBox_SelectedIndexChanged: {ex.Message}")
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
            Debug.WriteLine($"Error in AddItems: {ex.Message}")
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

#End Region

    ' ============================================
    ' PERHITUNGAN TOTAL DAN UPDATE UI
    ' ============================================
#Region "Perhitungan Total"

    ' ============================================
    ' FUNGSI: UPDATE SEMUA TOTAL
    ' ============================================
    Private Sub UpdateSemuaTotal()
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

            TxtGrandtotal.Text = grandTotal.ToString("N0")
            TxtTotalQTY.Text = totalQty.ToString("N0")
            LblRecord.Text = totalRows.ToString("N0")
            Txtlihattotal.Text = "Rp. " & grandTotal.ToString("N0")

            ' Scroll ke baris terakhir
            If DgvData.Rows.Count > 0 Then
                DgvData.FirstDisplayedScrollingRowIndex = DgvData.Rows.Count - 1
            End If

        Catch ex As Exception
            Debug.WriteLine($"Error in UpdateSemuaTotal: {ex.Message}")
        End Try
    End Sub

    ' ============================================
    ' EVENT: TEXT CHANGED UNTUK GRAND TOTAL
    ' ============================================
    Private Sub TxtGrandtotal_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtGrandtotal.TextChanged
        If TxtGrandtotal.Text = "" Or Not IsNumeric(TxtGrandtotal.Text) Then
            Txtlihattotal.Text = "0"
            Exit Sub
        Else
            Txtlihattotal.Text = FormatNumber(TxtGrandtotal.Text, 0)
        End If
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
                        Dim stokToko As Decimal = Convert.ToDecimal(rd("STOK_TOKO"))
                        Dim stokGudang As Decimal = Convert.ToDecimal(rd("STOK_GUDANG"))
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
            cell.Style.BackColor = Color.LightCoral
        Next
        DgvData.CurrentCell = row.Cells(1)
        MessageBox.Show(message, "Stok Tidak Cukup",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

#End Region

    ' ============================================
    ' PROSES PEMILIHAN REKENING
    ' ============================================
#Region "Proses Rekening"

    ' ============================================
    ' EVENT: SELECTED INDEX CHANGED UNTUK COMBOBOX REKENING
    ' ============================================
    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim namaAkunD As String = CmbRekening.Text
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
            If AwalReturBeli = "Pencarian" Then
                TxtNama.Select()
                TxtNama.Focus()
                Exit Sub
            End If
        End If

        ' Cek stok jika setting tidak mengizinkan minus
        If ReturBeliStokMinus = "Tidak" Then
            If CekStok() Then
                Return
            End If
        End If



        ' Tampilkan form bayar
        GBBayar.Visible = True
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
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Try

            If TransaksiLampau = "Tidak" AndAlso LblJenisTrans.Text <> "TambahReturBeli" Then
                DTPTgl.Value = Now
                NomorRetur()
            End If

            ' Jika edit transaksi, hapus data lama
            If LblJenisTrans.Text <> "TambahReturBeli" Then
                HapusUntukEdit(transaction)
            End If

            ' Simpan semua data transaksi
            ReturBeli(transaction)
            ReturBeli_Detail(transaction)
            HistoryBarang(transaction)
            Simpanjurnal(transaction)

            ' Commit transaksi
            transaction.Commit()

            ' Update stok per barang
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso
                   row.Cells(0).Value.ToString() <> "" Then
                    HitungByKode(row.Cells(0).Value)
                End If
            Next

            ' Catat history
            DatabaseModule.CatatanAksiHistory("Simpan retur barang " & TxtFaktur.Text)

            ' Tutup form jika edit
            If LblJenisTrans.Text <> "TambahReturBeli" Then
                Close()
            End If

            ' Tampilkan konfirmasi cetak
            Dim result As DialogResult = MessageBox.Show("Apakah Anda ingin mencetak retur barang?",
                                                         "Konfirmasi Cetak",
                                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                With PrintReturBeli
                    .TxtFaktur.Text = TxtFaktur.Text
                    .ProsesCetak()
                End With
            End If

            Kondisiawal()

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
    Private Sub HapusUntukEdit(ByVal transaction As MySqlTransaction)
        Dim stokField As String

        ' Tentukan field stok berdasarkan lokasi
        Select Case LblLokasiBarang.Text
            Case "TOKO"
                stokField = "STOK_TOKO"
            Case "GUDANG"
                stokField = "STOK_GUDANG"
            Case Else
                Throw New Exception("Lokasi barang tidak valid.")
        End Select

        ' Query untuk update stok
        Dim updateQuery As String = "UPDATE tbl_barang SET " & stokField & " = " &
                                    stokField & " + @QtyRetur WHERE ID_BARANG = @KodeBarang"

        ' Kembalikan stok untuk semua barang
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim qtyRetur As Decimal = If(row.Cells("QTY_SAT").Value IsNot Nothing,
                                                 Convert.ToDecimal(row.Cells("QTY_SAT").Value), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@QtyRetur", qtyRetur)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using

                    HitungStokPerubahan(kodeBarang, transaction)
                End If
            End If
        Next

        ' Hapus data terkait dari semua tabel
        Dim deleteQueries As String() = {
            "DELETE FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @FAKTUR",
            "DELETE FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @FAKTUR",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FAKTUR",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @FAKTUR"
        }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    ' ============================================
    ' FUNGSI: SIMPAN RETUR BELI (HEADER)
    ' ============================================
    Private Sub ReturBeli(ByVal transaction As MySqlTransaction)
        Dim sql As String = "INSERT INTO retur_pembelian (" &
        "ID_RETUR_PEMBELIAN, TGL_RETUR_BELI, " &
        "ID_SUPPLIER, NAMA_SUPPLIER, ALAMAT_SUPPLIER, KONTAK_SUPPLIER, " &
        "PENYIMPANAN, TOTAL_BARANG, TOTAL_QTY, TOTAL_RUPIAH, " &
        "JENIS_PENGEMBALIAN, NAMA_REKENING, KODE_REKENING, ALASAN_RETUR, " &
        "ID_USER, ID_KOMPUTER) " &
        "VALUES (" &
        "@ID_RETUR, @TGL_RETUR, " &
        "@ID_SUPPLIER, @NAMA_SUPPLIER, @ALAMAT_SUPPLIER, @KONTAK_SUPPLIER, " &
        "@PENYIMPANAN, @TOTAL_BARANG, @TOTAL_QTY, @TOTAL_RUPIAH, " &
        "@JENIS_PENGEMBALIAN, @NAMA_REKENING, @KODE_REKENING, @ALASAN_RETUR, " &
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
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", CDbl(LblRecord.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", CDbl(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", CDbl(TxtGrandtotal.Text))

            ' ===== RETURN DETAIL =====
            cmd.Parameters.AddWithValue("@JENIS_PENGEMBALIAN", "Tunai")
            cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@KODE_REKENING", TxtKodeRek.Text)
            cmd.Parameters.AddWithValue("@ALASAN_RETUR", RTBAlasanRetur.Text)

            ' ===== USER & KOMPUTER =====
            cmd.Parameters.AddWithValue("@ID_USER",
            If(LblJenisTrans.Text = "TambahReturBeli",
               FormUtama.SLogin.Text, TxtLogin.Text))

            cmd.Parameters.AddWithValue("@ID_KOMPUTER",
            If(LblJenisTrans.Text = "TambahReturBeli",
               FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    ' ============================================
    ' FUNGSI: SIMPAN RETUR BELI DETAIL
    ' ============================================
    Private Sub ReturBeli_Detail(ByVal transaction As MySqlTransaction)
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso
               row.Cells(0).Value.ToString() <> "" Then

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
                    cmd.Parameters.AddWithValue("@HARGA_BELI", Convert.ToDecimal(row.Cells("HARGA_BELI_TERAKHIR").Value))
                    cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells("QTY").Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells("SATUAN").Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells("ISI_SATUAN").Value))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", Convert.ToDecimal(row.Cells("HARGA_BELI_SATUAN").Value))
                    cmd.Parameters.AddWithValue("@QTY_SAT", Convert.ToDecimal(row.Cells("QTY_SAT").Value))
                    cmd.Parameters.AddWithValue("@TOTAL", Convert.ToDecimal(row.Cells("TOTAL").Value))
                    cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
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
                    Dim stokPengurangan As Decimal = If(row.Cells("QTY_SAT").Value IsNot Nothing,
                                                        Convert.ToDecimal(row.Cells("QTY_SAT").Value), 0D)

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
    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        Dim query As String = "INSERT INTO HistoryBarang " &
            "(FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, " &
            "QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, " &
            "@QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        ' Simpan history untuk setiap barang
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso
               row.Cells(0).Value.ToString() <> "" Then
                SaveHistory(query, transaction, "RETUR BELI KELUAR", LblLokasiBarang.Text, row)
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
            cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells("QTY").Value))
            cmd.Parameters.AddWithValue("@SATUAN", row.Cells("SATUAN").Value)
            cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells("ISI_SATUAN").Value))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", Convert.ToDecimal(row.Cells("QTY_SAT").Value))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", Convert.ToDecimal(row.Cells("TOTAL").Value))
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                       FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                           FormUtama.Comp.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ============================================
    ' FUNGSI: SIMPAN JURNAL
    ' ============================================
    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum " &
            "(NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, " &
            "NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
            "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, " &
            "@NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)",
            conn, transaction)

            ' Set semua parameter jurnal
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", "Retur pembelian barang dari " &
                                        LblLokasiBarang.Text & " supplier " & TxtSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", TxtKodeRek.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMINAL", Convert.ToDecimal(TxtGrandtotal.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Retur Pembelian")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                       FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(LblJenisTrans.Text = "TambahReturBeli",
                                                           FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using
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
            Dim cekTanggal As String = DTPTgl.Value.ToString("yyMMdd")
            Dim UrutKOde As String = ""
            Dim cekNomor As String = "RB-" & cekTanggal

            ' Cari nomor terakhir
            Using cmd As New MySqlCommand("SELECT MAX(ID_RETUR_PEMBELIAN) FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")
                Dim maxKode As Object = cmd.ExecuteScalar()

                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "RB-" & cekTanggal Then
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        UrutKOde = "RB-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika tidak ada nomor sebelumnya, buat nomor baru
            If String.IsNullOrEmpty(UrutKOde) Then
                UrutKOde = "RB-" & cekTanggal & "0001"
            End If

            TxtFaktur.Text = UrutKOde

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
            "SELECT TGL_RETUR_BELI, NAMA_SUPPLIER, ALASAN_RETUR, ID_USER, ID_KOMPUTER " &
            "FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN"

            Using cmd As New MySqlCommand(queryString, conn)
                cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then

                        ' Tanggal
                        If Not IsDBNull(rd("TGL_RETUR_BELI")) Then
                            DTPTgl.Value = Convert.ToDateTime(rd("TGL_RETUR_BELI"))
                        End If

                        ' Supplier
                        TxtSupplier.Text = If(IsDBNull(rd("NAMA_SUPPLIER")), "", rd("NAMA_SUPPLIER").ToString())

                        ' Alasan Retur
                        RTBAlasanRetur.Text = If(IsDBNull(rd("ALASAN_RETUR")), "", rd("ALASAN_RETUR").ToString())

                        ' User & Komputer
                        TxtLogin.Text = If(IsDBNull(rd("ID_USER")), "", rd("ID_USER").ToString())
                        TxtKomputer.Text = If(IsDBNull(rd("ID_KOMPUTER")), "", rd("ID_KOMPUTER").ToString())

                    End If
                End Using
            End Using

            ' ================= DETAIL =================
            AmbilDataDetailUntukEdit()

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
                                         "SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL " &
                                         "FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN", conn)
                cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", TxtFaktur.Text)
                cmd.Transaction = transaction

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Do While rd.Read()
                        Dim row As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
                        row.Cells("ID_BARANG").Value = rd("ID_BARANG")
                        row.Cells("NAMA_BARANG").Value = rd("NAMA_BARANG")
                        row.Cells("HARGA_BELI_TERAKHIR").Value = rd("HARGA_BELI")
                        row.Cells("QTY").Value = rd("QTY")
                        row.Cells("SATUAN").Value = rd("SATUAN")
                        row.Cells("ISI_SATUAN").Value = rd("ISI_SATUAN")
                        row.Cells("HARGA_BELI_SATUAN").Value = rd("HARGA_BELI_SATUAN")
                        row.Cells("QTY_SAT").Value = rd("QTY_SAT")
                        row.Cells("TOTAL").Value = rd("TOTAL")

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
    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click, BtnClose.Click
        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
        Close()
    End Sub



#End Region

End Class