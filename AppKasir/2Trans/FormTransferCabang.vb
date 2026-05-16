Imports System.Drawing.Printing
Imports System.IO
Imports System.Text

Public Class FormTransferCabang

    ' ── CATATAN: TIDAK ADA FITUR EDIT/HAPUS TRANSFER CABANG ─────────────────
    ' Fitur edit/hapus transfer cabang TIDAK boleh ditambahkan karena:
    ' 1. Tidak ada approval workflow antar cabang - edit di cabang asal tidak diketahui cabang tujuan
    ' 2. Offline mode membuat sync edit sulit - perlu kirim ulang CSV tapi cabang tujuan mungkin sudah DITERIMA
    ' 3. Race condition - jika transfer sudah DITERIMA di cabang tujuan, stok sudah berubah dan tidak bisa rollback
    ' 4. Tidak ada status "EDITED" atau "REVISED" di database
    '
    ' Jika ada kesalahan, solusinya:
    ' - Buat transfer baru untuk memperbaiki
    ' - Jika perlu, buat fitur "reverse" transfer (transfer balik) untuk membatalkan transfer yang sudah DITERIMA
    ' ─────────────────────────────────────────────────────────────────────────

#Region "Inner Classes"
    Private Class TransferItemSnapshot
        Public Property Kode As String
        Public Property NamaBarang As String
        Public Property Qty As Decimal
        Public Property Satuan As String
        Public Property Isi As Decimal
        Public Property QtySat As Decimal
        Public Property HargaBeli As Decimal
    End Class
#End Region

#Region "Properties & Fields"
    Public Property LokasiBarang As String = "TOKO"

    Private ReadOnly Property KolomStokKeluar As String
        Get
            Return If(LokasiBarang = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
        End Get
    End Property

    Private ReadOnly Property KolomTransferKeluar As String
        Get
            Return If(LokasiBarang = "GUDANG", "TRANSFER_CABANG_KELUAR_GUDANG", "TRANSFER_CABANG_KELUAR_TOKO")
        End Get
    End Property

    Private _currentTransferId As String = ""
    Private _selectedSatuan As String = "PCS"
    Private _selectedQty As Decimal = 1D
    Private _selectedIsi As Integer = 1
    Private _selectedLevelIndex As Integer = -1
    Private _selectedStokToko As Decimal = 0D
    Private _selectedStokGudang As Decimal = 0D
    Private _selectedSatuanOptions As New List(Of KeyValuePair(Of String, Integer))()
    Private _selectedKodeBarang As String = ""
    Private _notaText As String = ""
    Private WithEvents _printDoc As New PrintDocument()
    Private _panelTerima As Panel
    Private _dgvMasuk As DataGridView
    Private _btnTerima As Button
    Private _btnImportManual As Button
    Private _btnRefreshMasuk As Button
    Private _btnUploadOffline As Button
    Private _lblInfoTerima As Label
    Private isBarcodeMode As Boolean = False
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private barcodeTimer As New System.Windows.Forms.Timer()
    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100

    ' Barcode buffer untuk DGV inline search
    Private dgvBarcodeChars As New List(Of Char)()
    Private dgvBarcodeStartTime As DateTime = DateTime.MinValue
    Private dgvLastKeyTime As DateTime = DateTime.MinValue
    Private dgvBarcodeTimer As New System.Windows.Forms.Timer()
    Private Const DGV_BARCODE_TIMEOUT_MS As Integer = 100
#End Region


#Region "Form Load & Key Handling"
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If LstBarang.Visible Then
            If LstBarang.Focused Then
                Select Case keyData
                    Case Keys.Escape
                        LstBarang.Visible = False
                        LstBarang.Items.Clear()
                        If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                            _dgvEditingTextBox.Focus()
                        Else
                            TxtNamaBarang.Focus()
                        End If
                        Return True
                    Case Keys.Enter
                        If LstBarang.SelectedItem IsNot Nothing Then AmbilDataDariListBox()
                        Return True
                End Select
                Return MyBase.ProcessCmdKey(msg, keyData)
            End If

            Select Case keyData
                Case Keys.Down
                    _sedangPindahKeLstBarang = True
                    _rowSaatPindahKeLst = If(DgvDetail.CurrentCell IsNot Nothing, DgvDetail.CurrentCell.RowIndex, -1)
                    LstBarang.Focus()
                    If LstBarang.Items.Count > 0 Then LstBarang.SelectedIndex = 0
                    _sedangPindahKeLstBarang = False
                    Return True
                Case Keys.Up
                    Return MyBase.ProcessCmdKey(msg, keyData)
                Case Keys.Escape
                    LstBarang.Visible = False
                    LstBarang.Items.Clear()
                    Return True
                Case Keys.Enter
                    Return True
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub FormTransferCabang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Grand total otomatis via nama TxtGrandtotal
        ' Rename TxtGrantotal -> TxtGrandtotal untuk tema otomatis
        SetupGrid()
        InitSupabase()
        BuildModeUiRuntime()

        MuatDaftarCabang()
        MuatTransferMasuk()
        MuatTransferKeluarOfflinePending()
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick
        AddHandler dgvBarcodeTimer.Tick, AddressOf DgvBarcodeTimer_Tick
        Me.KeyPreview = True
        ConfigureAutoCompleteTxtNama()
        TxtNamaBarang.Clear()

        PanelCari.BackColor = SystemColors.ActiveCaption
        SetStatus("Status: siap input transfer antar cabang.")
    End Sub

    Private Sub FormTransferCabang_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Atur TabStop untuk CmbCabangTujuan berdasarkan setting fokus
        ' Jika mode edit langsung (False), nonaktifkan TabStop agar tidak mengambil fokus
        If Not ModulHakAkses.SettingFokusOtomatis Then
            CmbCabangTujuan.TabStop = False
        End If

        ' Setup fokus awal berdasarkan setting - dipanggil saat form sudah ditampilkan
        SetupFocusToGrid()
    End Sub

    Private Sub FormTransferCabang_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ' PanelGrid Dock=Fill — tidak perlu resize manual
    End Sub

    Private Sub FormTransferCabang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F4
                TxtNamaBarang.Focus()
                e.SuppressKeyPress = True
            Case Keys.F8
                If CmbMode Is Nothing OrElse CmbMode.SelectedItem Is Nothing OrElse CmbMode.SelectedItem.ToString() = "KIRIM" Then
                    TambahDataLangsung()
                End If
                e.SuppressKeyPress = True
            Case Keys.F10
                If CmbMode IsNot Nothing AndAlso CmbMode.SelectedItem IsNot Nothing AndAlso CmbMode.SelectedItem.ToString() = "TERIMA" Then
                    BtnTerimaTerpilih_Click(Nothing, EventArgs.Empty)
                Else
                    BtnKirimCloud_Click(Nothing, EventArgs.Empty)
                End If
                e.SuppressKeyPress = True
            Case Keys.F9
                CetakUlangNota(_currentTransferId)
                e.SuppressKeyPress = True
            Case Keys.F11
                If CmbMode Is Nothing OrElse CmbMode.SelectedItem Is Nothing OrElse CmbMode.SelectedItem.ToString() = "KIRIM" Then
                    DgvDetail.Rows.Clear()
                    BersihkanInputBarang()
                End If
                e.SuppressKeyPress = True
            Case Keys.Escape
                Me.Close()
        End Select
    End Sub
#End Region


#Region "Setup Fokus"
    ' ===== REUSABLE SUB - ATUR FOKUS KE DATAGRID =====
    ''' <summary>
    ''' Mengatur fokus ke DataGridView dengan behavior berbeda berdasarkan SettingFokusOtomatis
    ''' ✅ Jika SettingFokusOtomatis = True: fokus ke TxtNamaBarang (mode input/barcode)
    ''' ✅ Jika SettingFokusOtomatis = False: fokus ke sel NamaBarang baris terakhir (mode edit langsung)
    ''' </summary>
    Public Sub SetupFocusToGrid()
        If ModulHakAkses.SettingFokusOtomatis Then
            ' MODE 1: Pencarian - fokus ke TxtNamaBarang (input manual/barcode)
            TxtNamaBarang.Focus()
        Else
            ' MODE 2: Edit Langsung - fokus ke sel NamaBarang untuk edit inline
            ' Cari baris kosong SETELAH baris terakhir yang terisi
            Dim targetRow As Integer = 0
            Dim lastFilledRow As Integer = -1

            ' Cari baris terakhir yang terisi (ada kode)
            For i As Integer = DgvDetail.Rows.Count - 1 To 0 Step -1
                If Not DgvDetail.Rows(i).IsNewRow Then
                    Dim kodeVal = Convert.ToString(DgvDetail.Rows(i).Cells("Kode").Value).Trim()
                    If Not String.IsNullOrEmpty(kodeVal) Then
                        lastFilledRow = i
                        Exit For
                    End If
                End If
            Next

            ' Cari baris kosong setelah baris terakhir yang terisi
            If lastFilledRow >= 0 Then
                ' Ada baris terisi, cari baris kosong setelahnya
                For i As Integer = lastFilledRow + 1 To DgvDetail.Rows.Count - 1
                    If Not DgvDetail.Rows(i).IsNewRow Then
                        Dim kodeVal = Convert.ToString(DgvDetail.Rows(i).Cells("Kode").Value).Trim()
                        If String.IsNullOrEmpty(kodeVal) Then
                            targetRow = i
                            Exit For
                        End If
                    End If
                Next
            Else
                ' Tidak ada baris terisi, gunakan baris pertama
                targetRow = 0
            End If

            ' Set CurrentCell dan fokus ke DGV
            If targetRow < DgvDetail.Rows.Count Then
                DgvDetail.CurrentCell = DgvDetail(1, targetRow)
                Me.ActiveControl = DgvDetail

                ' Nested BeginInvoke untuk memastikan form sudah siap sebelum BeginEdit
                DgvDetail.BeginInvoke(New Action(Sub()
                                                     DgvDetail.BeginInvoke(New Action(Sub()
                                                                                          If DgvDetail.CurrentCell IsNot Nothing Then
                                                                                              DgvDetail.BeginEdit(True)
                                                                                              DgvDetail.EditingControl?.Focus()
                                                                                          End If
                                                                                      End Sub))
                                                 End Sub))
            End If
        End If
    End Sub

#End Region


#Region "Mode UI (KIRIM / TERIMA)"
    Private Sub BuildModeUiRuntime()
        CmbMode.SelectedIndex = 0
        AddHandler CmbMode.SelectedIndexChanged, AddressOf ModeChanged

        ' Label info mode TERIMA — tampil di PanelTopInfo menggantikan kontrol KIRIM
        _lblInfoTerima = New Label() With {
            .Text = "Mode TERIMA: data transfer masuk hanya dari Supabase Cloud atau file CSV." &
                    " Klik 'Refresh Masuk' untuk ambil data terbaru dari cloud." &
                    " Pilih satu atau lebih baris lalu klik 'Terima Terpilih' (F10).",
            .Font = New Font("Century Gothic", 9.0!, FontStyle.Regular),
            .ForeColor = Color.White,
            .BackColor = Color.SteelBlue,
            .Location = New Point(327, 50),
            .Size = New Size(430, 50),
            .Visible = False,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        PanelTopInfo.Controls.Add(_lblInfoTerima)

        ' Tombol TERIMA ditempatkan di PanelCariNama — panel ini height=35 di kedua mode
        _btnTerima = New Button() With {
            .Text = "Terima Terpilih", .Size = New Size(120, 26), .Visible = False
        }
        _btnImportManual = New Button() With {
            .Text = "Import Manual", .Size = New Size(120, 26), .Visible = False
        }
        _btnRefreshMasuk = New Button() With {
            .Text = "Refresh Masuk", .Size = New Size(120, 26), .Visible = False
        }
        _btnUploadOffline = New Button() With {
            .Text = "Upload Offline", .Size = New Size(130, 26),
            .BackColor = Color.Goldenrod, .ForeColor = Color.Black,
            .Font = New Font("Century Gothic", 8.25!, FontStyle.Bold),
            .Visible = False
        }
        AddHandler _btnTerima.Click, AddressOf BtnTerimaTerpilih_Click
        AddHandler _btnImportManual.Click, AddressOf BtnImportManualMasuk_Click
        AddHandler _btnRefreshMasuk.Click, AddressOf BtnRefreshMasuk_Click
        AddHandler _btnUploadOffline.Click, AddressOf BtnUploadOffline_Click
        PanelCari.Controls.Add(_btnTerima)
        PanelCari.Controls.Add(_btnImportManual)
        PanelCari.Controls.Add(_btnRefreshMasuk)
        PanelCari.Controls.Add(_btnUploadOffline)
        LayoutTombolTerima()

        ' _panelTerima hanya berisi DGV masuk, fill PanelGrid
        _panelTerima = New Panel() With {.Visible = False, .Dock = DockStyle.Fill}
        _dgvMasuk = New DataGridView() With {
            .Dock = DockStyle.Fill,
            .ReadOnly = True, .AllowUserToAddRows = False, .AllowUserToDeleteRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .RowHeadersVisible = False
        }
        _panelTerima.Controls.Add(_dgvMasuk)
        PanelGrid.Controls.Add(_panelTerima)
        _panelTerima.BringToFront()
        PanelFooter.BringToFront()
    End Sub

    ''' <summary>Posisikan tombol TERIMA di baris pertama PanelCariNama (Y=5), TxtNama di baris kedua.</summary>
    Private Sub LayoutTombolTerima()
        Dim y As Integer = 5
        Dim x As Integer = 7
        For Each btn As Button In {_btnTerima, _btnImportManual, _btnRefreshMasuk, _btnUploadOffline}
            btn.Location = New Point(x, y)
            x += btn.Width + 6
        Next
    End Sub

    Private Sub UpdatePanelTerimaLayout()
        ' Tidak diperlukan lagi — _dgvMasuk menggunakan Dock=Fill di dalam _panelTerima
    End Sub

    Private Sub ModeChanged(sender As Object, e As EventArgs)
        Dim isTerima As Boolean = CmbMode.SelectedItem.ToString().Equals("TERIMA", StringComparison.OrdinalIgnoreCase)

        ' --- PanelCariNama ---
        ' KIRIM: TxtNama + BtnCari tampil di Y=5, panel height=35
        ' TERIMA: TxtNama + BtnCari disembunyikan, hanya tombol aksi di Y=5, panel height=35
        TxtNamaBarang.Visible = Not isTerima
        BtnCari.Visible = Not isTerima
        TxtNamaBarang.Location = New Point(TxtNamaBarang.Left, 5)
        BtnCari.Location = New Point(BtnCari.Left, 5)
        PanelCari.Height = 35
        PanelCari.BackColor = If(isTerima, Color.SteelBlue, SystemColors.ActiveCaption)

        ' Tombol TERIMA
        _btnTerima.Visible = isTerima
        _btnImportManual.Visible = isTerima
        _btnRefreshMasuk.Visible = isTerima
        _btnUploadOffline.Visible = isTerima

        ' --- PanelGrid ---
        _panelTerima.Visible = isTerima
        DgvDetail.Visible = Not isTerima
        LstBarang.Visible = False

        ' --- PanelTopInfo: sembunyikan kontrol KIRIM saat TERIMA ---
        CmbCabangTujuan.Visible = Not isTerima
        LblCabangTujuan.Visible = Not isTerima
        BtnRefreshCabang.Visible = Not isTerima
        _lblInfoTerima.Visible = isTerima
        ' TxtKeterangan tetap tampil di kedua mode, label berubah sesuai konteks
        LblKeterangan.Text = If(isTerima, "Catatan Terima :", "Keterangan")

        ' --- PanelFooter: sembunyikan tombol KIRIM saat TERIMA ---
        BtnKirimCloud.Visible = Not isTerima
        BtnExportManual.Visible = Not isTerima

        SetStatus(If(isTerima,
            "Status: mode TERIMA — pilih baris lalu klik 'Terima Terpilih', atau Import CSV.",
            "Status: mode KIRIM aktif."))

        If isTerima Then
            MuatTransferMasuk()
            MuatTransferKeluarOfflinePending()
        End If
    End Sub
#End Region


#Region "TxtNamaBarang & Barcode"
    Private Sub TxtNamaBarang_GotFocus(sender As Object, e As EventArgs) Handles TxtNamaBarang.GotFocus
        If CmbMode.SelectedItem?.ToString() = "TERIMA" Then Return
        PanelCari.BackColor = Color.Yellow
        If DgvDetail.Rows.Count > 0 AndAlso DgvDetail.Columns.Count > 1 Then
            Try
                DgvDetail.CurrentCell = DgvDetail(1, DgvDetail.Rows.Count - 1)
                DgvDetail.Rows(DgvDetail.Rows.Count - 1).Selected = True
            Catch
            End Try
        End If
    End Sub

    Private Sub TxtNamaBarang_LostFocus(sender As Object, e As EventArgs) Handles TxtNamaBarang.LostFocus
        If CmbMode.SelectedItem?.ToString() = "TERIMA" Then Return
        PanelCari.BackColor = SystemColors.ActiveCaption
    End Sub

    Private Sub ConfigureAutoCompleteTxtNama()
        ' AutoComplete dimatikan — pencarian dilakukan sepenuhnya via LstBarang
        TxtNamaBarang.AutoCompleteMode = AutoCompleteMode.None
        TxtNamaBarang.AutoCompleteSource = AutoCompleteSource.None
    End Sub

    Private Sub TxtNamaBarang_TextChanged(sender As Object, e As EventArgs) Handles TxtNamaBarang.TextChanged
        If _sedangSetNilaiDariListBox Then Return
        Dim currentText = TxtNamaBarang.Text.Trim()

        If String.IsNullOrEmpty(currentText) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Tampilkan search list HANYA untuk input manual
        ' (ada huruf, atau format qty*... tanpa barcode)
        If currentText.Any(AddressOf Char.IsLetter) Then
            TriggerManualSearch(currentText)
        ElseIf currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            If parts.Length > 0 AndAlso parts(parts.Length - 1).Any(AddressOf Char.IsLetter) Then
                TriggerManualSearch(currentText)
            End If
        End If
    End Sub

    Private Sub TxtNamaBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtNamaBarang.KeyDown
        If e.KeyCode = Keys.Down AndAlso LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            LstBarang.Focus()
            LstBarang.SelectedIndex = 0
            e.SuppressKeyPress = True
            Return
        End If
        If e.KeyCode = Keys.Tab Then
            DgvDetail.Focus()
            e.SuppressKeyPress = True
            Return
        End If

        Dim ch As Char = ChrW(e.KeyCode)
        If Not Char.IsControl(ch) Then
            If ch = "*"c OrElse Char.IsLetter(ch) Then
                ResetBarcodeDetection()
                Return
            End If
            Dim currentTime = DateTime.Now
            If barcodeChars.Count = 0 Then
                barcodeStartTime = currentTime
                barcodeChars.Add(ch)
                lastKeyTime = currentTime
                barcodeTimer.Interval = 100
                barcodeTimer.Stop()
                barcodeTimer.Start()
                Return
            End If
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then isBarcodeMode = False
            If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(ch)
            lastKeyTime = currentTime
            barcodeTimer.Stop()
            barcodeTimer.Start()
            Return
        End If

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            barcodeTimer.Stop()
            If String.IsNullOrWhiteSpace(TxtNamaBarang.Text) Then
                ResetBarcodeDetection()
                Return
            End If
            Dim totalTimeMs = (DateTime.Now - barcodeStartTime).TotalMilliseconds
            ProcessInput(TxtNamaBarang.Text.Trim(), totalTimeMs)
            ResetBarcodeDetection()
            Return
        End If
        If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then isBarcodeMode = False
    End Sub

    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsedSinceLastKey = (DateTime.Now - lastKeyTime).TotalMilliseconds
        If elapsedSinceLastKey > 100 Then
            barcodeTimer.Stop()
            Dim bufferText = New String(barcodeChars.ToArray())
            If bufferText.Length >= BARCODE_MIN_LENGTH Then
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    TriggerManualSearch(bufferText)
                    ResetBarcodeDetection()
                    Return
                End If
                ProcessInput(bufferText, (DateTime.Now - barcodeStartTime).TotalMilliseconds)
                ResetBarcodeDetection()
            End If
        End If
    End Sub

    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False
        barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue
        lastKeyTime = DateTime.MinValue
        barcodeTimer.Stop()
    End Sub

    Private Sub ProcessInput(inputText As String, totalTimeMs As Double)
        Dim asteriskCount = inputText.Count(Function(c) c = "*"c)
        If asteriskCount = 2 Then
            Dim parts = inputText.Split("*"c)
            SetQtyAndSatuan(parts(0), parts(1))
            ProcessManualSearchList(parts(2).Trim())
            Return
        End If
        If asteriskCount = 1 Then
            Dim parts = inputText.Split("*"c)
            SetQtyOnly(parts(0))
            Dim secondPart = parts(1).Trim()
            If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso secondPart.Length >= BARCODE_MIN_LENGTH Then
                If SearchByBarcode(secondPart) Then Return
                MessageBox.Show("Barcode '" & secondPart & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                TxtNamaBarang.Clear()
                Return
            End If
            If IsBarcodeCandidate(secondPart) AndAlso SearchByBarcode(secondPart) Then Return
            ProcessManualSearchList(secondPart)
            Return
        End If
        If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso inputText.Length >= BARCODE_MIN_LENGTH Then
            If SearchByBarcode(inputText) Then Return
            MessageBox.Show("Barcode '" & inputText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            TxtNamaBarang.Clear()
            Return
        End If
        If IsBarcodeCandidate(inputText) AndAlso SearchByBarcode(inputText) Then Return
        SetDefaultQtyAndSatuan()
        ProcessManualSearchList(inputText)
    End Sub

    Private Sub ProcessInput(inputText As String)
        ProcessInput(inputText, BARCODE_TOTAL_TIME_MS + 1)
    End Sub

    Private Function IsBarcodeCandidate(input As String) As Boolean
        If input.Length < BARCODE_MIN_LENGTH Then Return False
        Return BarcodeExistsInDatabase(input)
    End Function

    Private Function BarcodeExistsInDatabase(barcodeValue As String) As Boolean
        Try
            Using cmd As New MySqlCommand(
                "SELECT 1 FROM tbl_barang WHERE BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@bc", barcodeValue)
                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Function SearchByBarcode(barcodeText As String) As Boolean
        Dim namaBarang As String = ""
        Dim ditemukan As Boolean = False
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND (BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc) LIMIT 1", conn)
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
            TxtNamaBarang.Text = ""
            LstBarang.Visible = False
            If _selectedQty <= 0D Then _selectedQty = 1D
            IsiBarangDariDb(namaBarang, True)
            Return True
        End If
        Return False
    End Function

    Private Sub SetQtyAndSatuan(qtyStr As String, satuanStr As String)
        _selectedQty = Math.Max(1D, ModuleAngka.ParseDecimal(qtyStr))
        Dim satuanNorm = If(String.IsNullOrWhiteSpace(satuanStr), "1", satuanStr.Trim())
        Dim lvl As Integer
        If Integer.TryParse(satuanNorm, lvl) AndAlso lvl >= 1 AndAlso lvl <= 3 Then
            _selectedLevelIndex = lvl - 1
            _selectedSatuan = ""
        Else
            _selectedLevelIndex = -1
            _selectedSatuan = satuanNorm.ToUpperInvariant()
        End If
    End Sub

    Private Sub SetQtyOnly(qtyStr As String)
        _selectedQty = Math.Max(1D, ModuleAngka.ParseDecimal(qtyStr))
        _selectedLevelIndex = -1
    End Sub

    Private Sub SetDefaultQtyAndSatuan()
        _selectedQty = 1D
        If String.IsNullOrWhiteSpace(_selectedSatuan) Then _selectedSatuan = "PCS"
        _selectedLevelIndex = -1
    End Sub
#End Region


#Region "ListBox Pencarian (LstBarang)"
    Private Sub LstBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If LstBarang.SelectedItem IsNot Nothing Then
                    AmbilDataDariListBox()
                    e.SuppressKeyPress = True
                End If
            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    LstBarang.SelectedIndex = -1
                    KembalikanFokusKeKonteksAsal()
                    e.SuppressKeyPress = True
                End If
            Case Keys.Escape
                LstBarang.Visible = False
                LstBarang.Items.Clear()
                KembalikanFokusKeKonteksAsal()
                e.SuppressKeyPress = True
        End Select
    End Sub

    Private Sub LstBarang_MouseClick(sender As Object, e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then AmbilDataDariListBox()
    End Sub

    ''' <summary>Kembalikan fokus ke TxtNamaBarang atau editing TextBox DGV sesuai konteks asal.</summary>
    Private Sub KembalikanFokusKeKonteksAsal()
        If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
            _dgvEditingTextBox.Focus()
        Else
            TxtNamaBarang.Focus()
        End If
    End Sub

    Private Sub TriggerManualSearch(keyword As String)
        ' Pastikan hentikan deteksi barcode agar tidak menutup ListBox
        ResetBarcodeDetection()
        _konteksLstBarang = "TXTNAMA"

        ' Parse jika ada tanda *
        If keyword.Contains("*") Then
            Dim parts = keyword.Split("*"c)
            If parts.Length >= 2 Then keyword = parts.Last().Trim()
        End If
        If keyword.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If
        PosisikanLstBarangDiBawahTxtNama()
        ProcessManualSearchList(keyword)
    End Sub

    Private Sub ProcessManualSearchList(searchKeyword As String)
        searchKeyword = searchKeyword.Trim()
        If searchKeyword.Length < 2 AndAlso Not searchKeyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If
        Dim stokField As String = If(LokasiBarang = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
        Dim hasil As New List(Of String)()
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                "WHERE STATUS='Aktif' AND (ID_BARANG LIKE @key OR NAMA_BARANG LIKE @key " &
                "OR BARCODE_KECIL LIKE @key OR BARCODE_SEDANG LIKE @key OR BARCODE_BESAR LIKE @key) " &
                "ORDER BY " & stokField & "", conn)
                cmd.Parameters.AddWithValue("@key", "%" & searchKeyword & "%")
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim stok = ModuleAngka.ParseDecimal(rd(stokField))
                        hasil.Add(rd("NAMA_BARANG").ToString() & " => " & stok.ToString("N0"))
                    End While
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error search: " & ex.Message)
            Return
        End Try
        LstBarang.Items.Clear()
        For Each item In hasil
            LstBarang.Items.Add(item)
        Next
        If LstBarang.Items.Count > 0 Then
            LstBarang.Visible = True
            LstBarang.BringToFront()
        Else
            LstBarang.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Ambil pilihan dari ListBox.
    ''' Konteks ditentukan oleh _konteksLstBarang: "TXTNAMA" atau "DGV".
    ''' </summary>
    Private Sub AmbilDataDariListBox()
        Dim selectedValue As String = ""
        If LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        ElseIf LstBarang.SelectedItem IsNot Nothing Then
            selectedValue = LstBarang.SelectedItem.ToString()
        End If
        If String.IsNullOrEmpty(selectedValue) Then Return

        Dim namayangdiambil As String = selectedValue
        Dim idxArrow As Integer = selectedValue.IndexOf(" => ")
        If idxArrow >= 0 Then namayangdiambil = selectedValue.Substring(0, idxArrow).Trim()

        LstBarang.Visible = False
        LstBarang.Items.Clear()
        _rowSaatPindahKeLst = -1

        ' Konteks DGV inline edit
        If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing AndAlso
           DgvDetail.CurrentCell IsNot Nothing AndAlso DgvDetail.CurrentCell.ColumnIndex = DgvDetail.Columns("NamaBarang").Index Then

            Dim originalInput As String = _dgvEditingTextBox.Text.Trim()
            Dim qtyValue As Decimal = 1D
            If originalInput.Contains("*"c) Then
                Dim parts = originalInput.Split("*"c)
                qtyValue = ModuleAngka.ParseDecimal(parts(0).Trim())
            End If

            Dim barisDiisi As Integer = DgvDetail.CurrentCell.RowIndex
            ' Batalkan edit mode dulu tanpa commit nilai lama
            _sedangSetNilaiDariListBox = True
            DgvDetail.CancelEdit()
            _sedangSetNilaiDariListBox = False

            ' Isi semua data langsung ke baris — tidak lewat CellEndEdit
            _selectedQty = qtyValue
            _selectedKodeBarang = AmbilKodeBarangDariNama(namayangdiambil)
            IsiBarangKeRow(barisDiisi, namayangdiambil, qtyValue)

            NavigasiKeBarisDgvKosong(barisDiisi)
            Return
        End If

        ' Konteks TxtNamaBarang
        Dim originalInputTxt As String = TxtNamaBarang.Text.Trim()
        If originalInputTxt.Contains("*"c) Then
            Dim inputParts As String() = originalInputTxt.Split("*"c)
            If inputParts.Length >= 3 Then
                SetQtyAndSatuan(inputParts(0).Trim(), inputParts(1).Trim())
            ElseIf inputParts.Length = 2 Then
                SetQtyOnly(inputParts(0).Trim())
            End If
        End If
        IsiBarangDariDb(namayangdiambil, True)
    End Sub
#End Region


#Region "DataGridView Setup & Events"
    Private Sub SetupGrid()
        AddHandler DgvDetail.PreviewKeyDown, AddressOf DgvDetail_PreviewKeyDown
        DgvDetail.ReadOnly = False
        DgvDetail.Columns("Kode").ReadOnly = True
        DgvDetail.Columns("Isi").ReadOnly = True
        DgvDetail.Columns("QtySat").ReadOnly = True
        DgvDetail.Columns("TotalHarga").ReadOnly = True
        DgvDetail.Columns("StokToko").ReadOnly = True
        DgvDetail.Columns("StokGudang").ReadOnly = True
        DgvDetail.Columns("Isi").DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)
        DgvDetail.Columns("QtySat").DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)
        DgvDetail.Columns("TotalHarga").DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)
        DgvDetail.Columns("StokToko").DefaultCellStyle.BackColor = Color.LightBlue
        DgvDetail.Columns("StokGudang").DefaultCellStyle.BackColor = Color.LightBlue

        Dim kolomAngka As String() = {"HargaBeli", "QTY", "Isi", "QtySat", "TotalHarga", "StokToko", "StokGudang"}
        ModuleAngka.TerapkanFormatKolomAngka(DgvDetail, kolomAngka)

        DgvDetail.EnableHeadersVisualStyles = False
        DgvDetail.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
        DgvDetail.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DgvDetail.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray
        DgvDetail.RowHeadersVisible = True
        DgvDetail.GetType().InvokeMember("DoubleBuffered",
            Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty,
            Nothing, DgvDetail, New Object() {True})

        Dim satuanCol = TryCast(DgvDetail.Columns("Satuan"), DataGridViewComboBoxColumn)
        If satuanCol IsNot Nothing Then
            satuanCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            satuanCol.FlatStyle = FlatStyle.Flat
        End If
    End Sub

    Private Sub DgvDetail_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        If LstBarang.Visible AndAlso Not LstBarang.Focused Then
            Select Case e.KeyCode
                Case Keys.Down, Keys.Up, Keys.Enter, Keys.Escape
                    e.IsInputKey = False
            End Select
        End If
    End Sub

    Private Sub DgvDetail_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvDetail.CellLeave
        Debug.WriteLine($"[CellLeave] col={e.ColumnIndex} row={e.RowIndex} LstVisible={LstBarang.Visible} LstFocused={LstBarang.Focused} Pindah={_sedangPindahKeLstBarang}")
        If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return
        If LstBarang.Visible AndAlso e.RowIndex = _rowSaatPindahKeLst Then Return
        If LstBarang.Visible Then
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            _rowSaatPindahKeLst = -1
        End If
    End Sub

    Private Sub DgvDetail_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DgvDetail.EditingControlShowing
        If DgvDetail.CurrentCell.ColumnIndex = DgvDetail.Columns("NamaBarang").Index Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.None
                If _dgvEditingTextBox IsNot Nothing Then
                    RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                    RemoveHandler _dgvEditingTextBox.PreviewKeyDown, AddressOf DgvNamaBarang_PreviewKeyDown
                End If
                _dgvEditingTextBox = autoText
                AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                AddHandler _dgvEditingTextBox.PreviewKeyDown, AddressOf DgvNamaBarang_PreviewKeyDown
                PosisikanLstBarangDiBawahSel()
            End If
        Else
            If Not LstBarang.Focused Then
                LstBarang.Visible = False
                LstBarang.Items.Clear()
            End If
        End If
        If DgvDetail.CurrentCell.ColumnIndex = DgvDetail.Columns("Satuan").Index Then
            If TypeOf e.Control Is ComboBox Then
                Dim cmb As ComboBox = DirectCast(e.Control, ComboBox)
                RemoveHandler cmb.SelectedIndexChanged, AddressOf DgvDetail_SatuanChanged
                AddHandler cmb.SelectedIndexChanged, AddressOf DgvDetail_SatuanChanged
            End If
        End If
    End Sub

    ''' <summary>Update warna dan ReadOnly kolom NamaBarang berdasarkan apakah Kode sudah terisi.</summary>
    Private Sub UpdateWarnaKodeBarang(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= DgvDetail.Rows.Count Then Return
        Dim kodeValue = DgvDetail.Rows(rowIndex).Cells("Kode").Value
        Dim adaKode As Boolean = kodeValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(kodeValue.ToString())
        Dim cell = DgvDetail.Rows(rowIndex).Cells("NamaBarang")
        If adaKode Then
            cell.ReadOnly = True
            cell.Style.BackColor = Color.FromArgb(144, 238, 144) ' LightGreen — jelas terisi
            cell.Style.ForeColor = Color.DarkGreen
        Else
            cell.ReadOnly = False
            cell.Style.BackColor = Color.White
            cell.Style.ForeColor = Color.Black
        End If
    End Sub

    Private Sub DgvDetail_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvDetail.CellEnter
        If e.RowIndex < 0 Then Return
        If e.ColumnIndex = DgvDetail.Columns("NamaBarang").Index Then
            UpdateWarnaKodeBarang(e.RowIndex)
        End If
    End Sub

    Private Sub DgvDetail_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DgvDetail.CellEndEdit
        Debug.WriteLine($"[CellEndEdit] col={e.ColumnIndex} row={e.RowIndex}")
        If e.RowIndex < 0 Then Return

        If e.ColumnIndex = DgvDetail.Columns("NamaBarang").Index Then
            Dim inputText As String = Convert.ToString(DgvDetail.Rows(e.RowIndex).Cells("NamaBarang").Value).Trim()
            Debug.WriteLine($"[CellEndEdit] NamaBarang='{inputText}'")
            If String.IsNullOrWhiteSpace(inputText) Then Return
            Dim qtyValue As Decimal = 1D
            Dim namaBarang As String = inputText
            Dim idxAsterisk As Integer = inputText.IndexOf("*")
            If idxAsterisk > 0 Then
                qtyValue = ModuleAngka.ParseDecimal(inputText.Substring(0).Trim())
                namaBarang = inputText.Substring(idxAsterisk + 1).Trim()
                DgvDetail.Rows(e.RowIndex).Cells("NamaBarang").Value = namaBarang
            End If
            Dim idBarang As String = "" : Dim satKecil As String = "" : Dim isiKecil As Integer = 1
            Dim satSedang As String = "" : Dim isiSedang As Integer = 1
            Dim satBesar As String = "" : Dim isiBesar As Integer = 1
            Dim stokToko As Decimal = 0D : Dim stokGudang As Decimal = 0D
            Dim hargaBeli As Decimal = 0D : Dim ditemukan As Boolean = False
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, HARGA_BELI, SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, " &
                "SATUAN_UMUM_BESAR, ISI_UMUM_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                "WHERE STATUS='Aktif' AND (NAMA_BARANG=@n OR BARCODE_KECIL=@n OR BARCODE_SEDANG=@n OR BARCODE_BESAR=@n) LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        ditemukan = True
                        idBarang = rd("ID_BARANG").ToString()
                        hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                        satKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                        isiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        satSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                        isiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        satBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                        isiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                        stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                        stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    End If
                End Using
            End Using
            If Not ditemukan Then
                DgvDetail.Rows(e.RowIndex).Cells("NamaBarang").Value = ""
                Return
            End If
            Dim options As New List(Of KeyValuePair(Of String, Integer))()
            If Not String.IsNullOrWhiteSpace(satKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satKecil, isiKecil))
            If Not String.IsNullOrWhiteSpace(satSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satSedang, isiSedang))
            If Not String.IsNullOrWhiteSpace(satBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satBesar, isiBesar))
            If options.Count = 0 Then options.Add(New KeyValuePair(Of String, Integer)("PCS", 1))
            Dim row = DgvDetail.Rows(e.RowIndex)
            row.Cells("Kode").Value = idBarang
            row.Cells("NamaBarang").Value = namaBarang
            Debug.WriteLine($"[CellEndEdit] SET Kode='{idBarang}' di row={e.RowIndex}")
            UpdateWarnaKodeBarang(e.RowIndex)
            TerapkanSatuanKeRow(row, options, options(0).Key, options(0).Value)
            row.Cells("QTY").Value = qtyValue
            row.Cells("StokToko").Value = stokToko
            row.Cells("StokGudang").Value = stokGudang
            row.Cells("HargaBeli").Value = hargaBeli
            HitungBaris(e.RowIndex)
            Return
        End If

        If e.ColumnIndex = DgvDetail.Columns("QTY").Index Then
            Dim row = DgvDetail.Rows(e.RowIndex)
            Dim qty As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY").Value)
            If qty <= 0D Then
                MessageBox.Show("Qty hanya boleh angka lebih besar dari 0.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                qty = 1D
            End If
            row.Cells("QTY").Value = qty

            ' Validasi stok setelah ganti qty
            Dim kode As String = Convert.ToString(row.Cells("Kode").Value).Trim()
            If Not String.IsNullOrWhiteSpace(kode) Then
                Dim qtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("QtySat").Value))
                If qtySat > 0D Then
                    Dim errorCode As String = ""
                    Dim errorMessage As String = ""
                    Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                        cmd.Parameters.AddWithValue("@kode", kode)
                        cmd.Parameters.AddWithValue("@qty", qtySat)
                        cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                        cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                        cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                        cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                        cmd.ExecuteNonQuery()
                        errorCode = Convert.ToString(cmd.Parameters("@error_code").Value)
                        errorMessage = Convert.ToString(cmd.Parameters("@error_message").Value)
                    End Using

                    If errorCode = "STOK_KURANG" Then
                        MessageBox.Show(errorMessage, "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        ' Kembalikan qty ke 1
                        row.Cells("QTY").Value = 1D
                        HitungBaris(e.RowIndex)
                        Return
                    End If
                End If
            End If

            HitungBaris(e.RowIndex)
        End If

        If e.ColumnIndex = DgvDetail.Columns("HargaBeli").Index Then
            Dim row = DgvDetail.Rows(e.RowIndex)
            Dim harga As Decimal = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
            If harga < 0D Then harga = 0D
            row.Cells("HargaBeli").Value = harga
            HitungBaris(e.RowIndex)
        End If
    End Sub

    Private Sub DgvDetail_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DgvDetail.CurrentCellDirtyStateChanged
        If DgvDetail.IsCurrentCellDirty AndAlso TypeOf DgvDetail.CurrentCell Is DataGridViewComboBoxCell Then
            DgvDetail.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DgvDetail_SatuanChanged(sender As Object, e As EventArgs)
        If DgvDetail.CurrentCell Is Nothing Then Return
        Dim cmb As ComboBox = TryCast(sender, ComboBox)
        If cmb Is Nothing Then Return
        Dim rowIdx As Integer = DgvDetail.CurrentCell.RowIndex
        If rowIdx < 0 Then Return
        Dim kode As String = Convert.ToString(DgvDetail.Rows(rowIdx).Cells("Kode").Value).Trim()
        If String.IsNullOrWhiteSpace(kode) Then Return
        Dim options = AmbilSatuanByIdBarang(kode)
        If options.Count = 0 Then Return
        Dim idx As Integer = cmb.SelectedIndex
        If idx < 0 OrElse idx >= options.Count Then idx = 0
        DgvDetail.Rows(rowIdx).Cells("Isi").Value = options(idx).Value

        ' Validasi stok setelah ganti satuan
        Dim qtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(DgvDetail.Rows(rowIdx).Cells("QtySat").Value))
        If qtySat > 0D Then
            Dim errorCode As String = ""
            Dim errorMessage As String = ""
            Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                cmd.Parameters.AddWithValue("@qty", qtySat)
                cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                cmd.ExecuteNonQuery()
                errorCode = Convert.ToString(cmd.Parameters("@error_code").Value)
                errorMessage = Convert.ToString(cmd.Parameters("@error_message").Value)
            End Using

            If errorCode = "STOK_KURANG" Then
                MessageBox.Show(errorMessage, "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                ' Kembalikan ke satuan sebelumnya
                Dim currentIdx As Integer = options.FindIndex(Function(x) x.Key.Equals(cmb.Text, StringComparison.OrdinalIgnoreCase))
                If currentIdx < 0 Then currentIdx = 0
                DgvDetail.Rows(rowIdx).Cells("Satuan").Value = options(currentIdx).Key
                DgvDetail.Rows(rowIdx).Cells("Isi").Value = options(currentIdx).Value
                HitungBaris(rowIdx)
                Return
            End If
        End If

        HitungBaris(rowIdx)
    End Sub

    Private Sub DgvDetail_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvDetail.CellFormatting
        If DgvDetail.Columns("StokToko") IsNot Nothing AndAlso DgvDetail.Columns("StokGudang") IsNot Nothing Then
            If e.ColumnIndex = DgvDetail.Columns("StokToko").Index OrElse e.ColumnIndex = DgvDetail.Columns("StokGudang").Index Then
                If e.Value IsNot Nothing AndAlso ModuleAngka.ParseDecimal(e.Value) < 1 Then
                    e.CellStyle.BackColor = Color.Red
                    e.CellStyle.ForeColor = Color.White
                End If
            End If
        End If
    End Sub

    Private Sub DgvDetail_KeyDown(sender As Object, e As KeyEventArgs) Handles DgvDetail.KeyDown
        ' Navigasi panah atas/bawah di kolom Satuan (kolom 4) — ganti satuan & hitung ulang
        If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down) AndAlso
           DgvDetail.CurrentCell IsNot Nothing AndAlso
           DgvDetail.CurrentCell.ColumnIndex = DgvDetail.Columns("Satuan").Index Then

            Dim rowIdx As Integer = DgvDetail.CurrentCell.RowIndex
            Dim kode As String = Convert.ToString(DgvDetail.Rows(rowIdx).Cells("Kode").Value).Trim()
            If Not String.IsNullOrWhiteSpace(kode) Then
                Dim options = AmbilSatuanByIdBarang(kode)
                If options.Count > 1 Then
                    Dim comboCell = TryCast(DgvDetail.Rows(rowIdx).Cells("Satuan"), DataGridViewComboBoxCell)
                    If comboCell IsNot Nothing Then
                        Dim currentSatuan As String = Convert.ToString(comboCell.Value)
                        Dim currentIdx As Integer = options.FindIndex(Function(x) x.Key.Equals(currentSatuan, StringComparison.OrdinalIgnoreCase))
                        If currentIdx < 0 Then currentIdx = 0
                        Dim newIdx As Integer = If(e.KeyCode = Keys.Down,
                            Math.Min(currentIdx + 1, options.Count - 1),
                            Math.Max(currentIdx - 1, 0))
                        If newIdx <> currentIdx Then
                            DgvDetail.Rows(rowIdx).Cells("Satuan").Value = options(newIdx).Key
                            DgvDetail.Rows(rowIdx).Cells("Isi").Value = options(newIdx).Value

                            ' Validasi stok setelah ganti satuan
                            Dim qtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(DgvDetail.Rows(rowIdx).Cells("QtySat").Value))
                            If qtySat > 0D Then
                                Dim errorCode As String = ""
                                Dim errorMessage As String = ""
                                Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                                    cmd.Parameters.AddWithValue("@kode", kode)
                                    cmd.Parameters.AddWithValue("@qty", qtySat)
                                    cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                                    cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                                    cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                                    cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                                    cmd.ExecuteNonQuery()
                                    errorCode = Convert.ToString(cmd.Parameters("@error_code").Value)
                                    errorMessage = Convert.ToString(cmd.Parameters("@error_message").Value)
                                End Using

                                If errorCode = "STOK_KURANG" Then
                                    MessageBox.Show(errorMessage, "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    ' Kembalikan ke satuan sebelumnya
                                    DgvDetail.Rows(rowIdx).Cells("Satuan").Value = options(currentIdx).Key
                                    DgvDetail.Rows(rowIdx).Cells("Isi").Value = options(currentIdx).Value
                                    e.SuppressKeyPress = True
                                    Return
                                End If
                            End If

                            HitungBaris(rowIdx)
                        End If
                        e.SuppressKeyPress = True
                        Return
                    End If
                End If
            End If
        End If

        If e.KeyCode = Keys.Delete Then
            If DgvDetail.CurrentCell IsNot Nothing AndAlso DgvDetail.CurrentCell.RowIndex >= 0 Then
                If DgvDetail.Columns(DgvDetail.CurrentCell.ColumnIndex).ReadOnly Then
                    If MessageBox.Show("Hapus baris ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        DgvDetail.Rows.RemoveAt(DgvDetail.CurrentCell.RowIndex)
                        HitungGrandTotal()
                    End If
                    e.SuppressKeyPress = True
                End If
            End If
        End If
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If DgvDetail.CurrentCell Is Nothing Then Return
            Dim r = DgvDetail.CurrentCell.RowIndex
            Dim c = DgvDetail.CurrentCell.ColumnIndex
            Dim nextCol As Integer = c + 1
            If nextCol >= DgvDetail.ColumnCount Then
                nextCol = 1 : r += 1
                If r >= DgvDetail.RowCount Then TxtNamaBarang.Focus() : Return
            End If
            DgvDetail.CurrentCell = DgvDetail(nextCol, r)
        End If
    End Sub

    Private Sub DgvDetail_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvDetail.RowPostPaint
        Using b As New SolidBrush(DgvDetail.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 8, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DgvDetail_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles DgvDetail.DataError
        e.ThrowException = False
    End Sub
#End Region


#Region "Pencarian Inline DGV (DgvNamaBarang)"
    Private _dgvEditingTextBox As TextBox = Nothing
    Private _sedangPindahKeLstBarang As Boolean = False
    Private _rowSaatPindahKeLst As Integer = -1
    Private _konteksLstBarang As String = "TXTNAMA" ' "TXTNAMA" atau "DGV"
    Private _sedangSetNilaiDariListBox As Boolean = False ' blok TextChanged saat set nilai programatically

    Private Sub DgvNamaBarang_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        Debug.WriteLine($"[PreviewKeyDown] key={e.KeyCode} LstVisible={LstBarang.Visible}")
        ' ProcessCmdKey di level form yang menangkap — tidak set IsInputKey di sini
    End Sub

    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        If _sedangSetNilaiDariListBox Then Return
        _konteksLstBarang = "DGV"
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return
        Dim currentText = txt.Text.Trim()

        ' Feed karakter ke DGV barcode buffer
        Dim currentTime = DateTime.Now
        If dgvBarcodeChars.Count = 0 Then
            dgvBarcodeStartTime = currentTime
            dgvBarcodeChars.Clear()
        End If
        Dim intervalMs = If(dgvBarcodeChars.Count > 0, (currentTime - dgvLastKeyTime).TotalMilliseconds, 0)
        dgvLastKeyTime = currentTime

        ' Reset DGV barcode timer
        dgvBarcodeTimer.Stop()
        dgvBarcodeTimer.Interval = DGV_BARCODE_TIMEOUT_MS
        dgvBarcodeTimer.Start()

        If String.IsNullOrEmpty(currentText) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            dgvBarcodeChars.Clear()
            Return
        End If

        ' Tampilkan search list untuk input manual (ada huruf atau format qty*... tanpa barcode)
        If currentText.Any(AddressOf Char.IsLetter) AndAlso Not currentText.Contains("*") Then
            TriggerManualSearchDGV(currentText)
            Return
        End If

        ' Parse qty*level*nama atau qty*nama
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            If parts.Length >= 2 Then
                Dim keywordPart = parts(parts.Length - 1).Trim()
                If keywordPart.Any(AddressOf Char.IsLetter) Then
                    TriggerManualSearchDGV(currentText)
                    Return
                End If
            End If
        End If

        ' Feed karakter ke barcode buffer (hanya digit)
        For Each ch As Char In currentText
            If Char.IsDigit(ch) Then
                If dgvBarcodeChars.Count < BARCODE_MAX_LENGTH Then
                    dgvBarcodeChars.Add(ch)
                End If
            End If
        Next

        Dim keyword As String = currentText
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            keyword = parts(parts.Length - 1).Trim()
        End If
        If keyword.Length < 2 OrElse Not keyword.Any(AddressOf Char.IsLetter) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
        Else
            TriggerManualSearchDGV(keyword)
        End If
    End Sub

    Private Sub DgvNamaBarang_KeyDown(sender As Object, e As KeyEventArgs)
        Debug.WriteLine($"[DgvNamaBarang_KeyDown] key={e.KeyCode} LstVisible={LstBarang.Visible}")
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return

        ' Handle barcode scanner input
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            e.Handled = True
            dgvBarcodeTimer.Stop()

            Dim bufferText = New String(dgvBarcodeChars.ToArray())
            Dim totalTimeMs = (DateTime.Now - dgvBarcodeStartTime).TotalMilliseconds

            ' Reset buffer
            dgvBarcodeChars.Clear()
            dgvBarcodeStartTime = DateTime.MinValue
            dgvLastKeyTime = DateTime.MinValue

            If String.IsNullOrWhiteSpace(txt.Text) Then Return

            ' Parse input
            Dim asteriskCount = txt.Text.Count(Function(c) c = "*"c)
            If asteriskCount = 2 Then
                ' Format: qty*level*nama
                Dim parts = txt.Text.Split("*"c)
                SetQtyAndSatuan(parts(0), parts(1))
                ProcessDgvBarcodeSearch(parts(2).Trim())
            ElseIf asteriskCount = 1 Then
                ' Format: qty*nama atau barcode
                Dim parts = txt.Text.Split("*"c)
                SetQtyOnly(parts(0))
                Dim secondPart = parts(1).Trim()
                If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso secondPart.Length >= BARCODE_MIN_LENGTH Then
                    If SearchByBarcodeDGV(secondPart) Then Return
                End If
                If IsBarcodeCandidate(secondPart) AndAlso SearchByBarcodeDGV(secondPart) Then Return
                ProcessDgvManualSearchList(secondPart)
            ElseIf totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso bufferText.Length >= BARCODE_MIN_LENGTH Then
                ' Barcode scanner input
                If SearchByBarcodeDGV(bufferText) Then Return
            Else
                ProcessDgvManualSearchList(txt.Text)
            End If
            Return
        End If

        If Not LstBarang.Visible Then Return
        Select Case e.KeyCode
            Case Keys.Down
                _sedangPindahKeLstBarang = True
                LstBarang.Focus()
                If LstBarang.Items.Count > 0 Then LstBarang.SelectedIndex = 0
                _sedangPindahKeLstBarang = False
                e.SuppressKeyPress = True
            Case Keys.Escape
                LstBarang.Visible = False
                LstBarang.Items.Clear()
                e.SuppressKeyPress = True
            Case Keys.Enter
                e.SuppressKeyPress = True
                e.Handled = True
        End Select
    End Sub

    Private Sub PosisikanLstBarangDiBawahSel()
        If DgvDetail.CurrentCell Is Nothing Then Return
        Try
            Dim cellRect = DgvDetail.GetCellDisplayRectangle(
                DgvDetail.CurrentCell.ColumnIndex, DgvDetail.CurrentCell.RowIndex, True)
            Dim ptDgv = DgvDetail.PointToScreen(New Point(cellRect.Left, cellRect.Bottom))
            Dim ptPanel = PanelRoot.PointToClient(ptDgv)
            LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            LstBarang.Width = Math.Max(300, cellRect.Width)
        Catch
        End Try
    End Sub

    Private Sub PosisikanLstBarangDiBawahTxtNama()
        Try
            Dim ptTxt = TxtNamaBarang.PointToScreen(New Point(0, TxtNamaBarang.Height))
            Dim ptPanel = PanelRoot.PointToClient(ptTxt)
            LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            LstBarang.Width = Math.Max(300, TxtNamaBarang.Width)
        Catch
        End Try
    End Sub

    ''' <summary>Trigger manual search untuk DGV context.</summary>
    Private Sub TriggerManualSearchDGV(keyword As String)
        ' Parse jika ada tanda *
        If keyword.Contains("*") Then
            Dim parts = keyword.Split("*"c)
            If parts.Length >= 2 Then keyword = parts.Last().Trim()
        End If
        If keyword.Length < 2 AndAlso Not keyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If
        Dim stokField As String = If(LokasiBarang = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
        Dim hasil As New List(Of String)()
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                "WHERE STATUS='Aktif' AND (ID_BARANG LIKE @key OR NAMA_BARANG LIKE @key " &
                "OR BARCODE_KECIL LIKE @key OR BARCODE_SEDANG LIKE @key OR BARCODE_BESAR LIKE @key) " &
                "ORDER BY " & stokField & "", conn)
                cmd.Parameters.AddWithValue("@key", "%" & keyword & "%")
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim stok = ModuleAngka.SafeGetValue(Of Decimal)(rd, stokField, 0D)
                        hasil.Add(rd("NAMA_BARANG").ToString() & " => " & stok.ToString("N0"))
                    End While
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error search: " & ex.Message)
            Return
        End Try
        LstBarang.Items.Clear()
        For Each item In hasil
            LstBarang.Items.Add(item)
        Next
        If LstBarang.Items.Count > 0 Then
            PosisikanLstBarangDiBawahSel()
            LstBarang.Visible = True
            LstBarang.BringToFront()
        Else
            LstBarang.Visible = False
        End If
    End Sub

    ''' <summary>Process DGV barcode search.</summary>
    Private Sub ProcessDgvBarcodeSearch(keyword As String)
        If String.IsNullOrWhiteSpace(keyword) Then Return
        Dim namaBarang As String = ""
        Dim ditemukan As Boolean = False
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND (BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc) LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@bc", keyword)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        namaBarang = rd("NAMA_BARANG").ToString()
                        ditemukan = True
                    End If
                End Using
            End Using
        Catch
            Return
        End Try
        If ditemukan Then
            IsiBarangDariDbDGV(namaBarang)
        End If
    End Sub

    ''' <summary>Process DGV manual search list.</summary>
    Private Sub ProcessDgvManualSearchList(keyword As String)
        keyword = keyword.Trim()
        If keyword.Length < 2 AndAlso Not keyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If
        TriggerManualSearchDGV(keyword)
    End Sub

    ''' <summary>Search by barcode in DGV context.</summary>
    Private Function SearchByBarcodeDGV(barcodeText As String) As Boolean
        Dim namaBarang As String = ""
        Dim ditemukan As Boolean = False
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND (BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc) LIMIT 1", conn)
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
            IsiBarangDariDbDGV(namaBarang)
            Return True
        End If
        Return False
    End Function

    ''' <summary>DGV Barcode Timer Tick - handle barcode scanner detection in DGV context.</summary>
    Private Sub DgvBarcodeTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsedSinceLastKey = (DateTime.Now - dgvLastKeyTime).TotalMilliseconds
        If elapsedSinceLastKey > DGV_BARCODE_TIMEOUT_MS Then
            dgvBarcodeTimer.Stop()
            Dim bufferText = New String(dgvBarcodeChars.ToArray())
            If bufferText.Length >= BARCODE_MIN_LENGTH Then
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    TriggerManualSearchDGV(bufferText)
                Else
                    SearchByBarcodeDGV(bufferText)
                End If
            End If
            dgvBarcodeChars.Clear()
            dgvBarcodeStartTime = DateTime.MinValue
            dgvLastKeyTime = DateTime.MinValue
        End If
    End Sub

    ''' <summary>Isi barang dari database untuk DGV context.</summary>
    Private Sub IsiBarangDariDbDGV(keyword As String)
        If String.IsNullOrWhiteSpace(keyword) Then Return
        If DgvDetail.CurrentCell Is Nothing Then Return
        Dim rowIdx As Integer = DgvDetail.CurrentCell.RowIndex

        Dim idBarang As String = "" : Dim namaBarang As String = ""
        Dim satKecil As String = "" : Dim isiKecil As Integer = 1
        Dim satSedang As String = "" : Dim isiSedang As Integer = 1
        Dim satBesar As String = "" : Dim isiBesar As Integer = 1
        Dim stokToko As Decimal = 0D : Dim stokGudang As Decimal = 0D
        Dim hargaBeli As Decimal = 0D

        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, " &
            "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_BESAR, STOK_TOKO, STOK_GUDANG " &
            "FROM tbl_barang WHERE STATUS='Aktif' AND " &
            "(NAMA_BARANG=@k OR BARCODE_KECIL=@k OR BARCODE_SEDANG=@k OR BARCODE_BESAR=@k) LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@k", keyword)
            Try
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        idBarang = rd("ID_BARANG").ToString()
                        namaBarang = rd("NAMA_BARANG").ToString()
                        hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                        satKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                        isiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        satSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                        isiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        satBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                        isiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                        stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                        stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    Else
                        MessageBox.Show("Barang '" & keyword & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error membaca data barang: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        Dim options As New List(Of KeyValuePair(Of String, Integer))()
        If Not String.IsNullOrWhiteSpace(satKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satKecil, isiKecil))
        If Not String.IsNullOrWhiteSpace(satSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satSedang, isiSedang))
        If Not String.IsNullOrWhiteSpace(satBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satBesar, isiBesar))
        If options.Count = 0 Then options.Add(New KeyValuePair(Of String, Integer)("PCS", 1))

        ' Cek jika barang sama sudah ada di grid
        For Each existingRow As DataGridViewRow In DgvDetail.Rows
            If existingRow.IsNewRow Then Continue For
            If existingRow.Index = rowIdx Then Continue For
            If existingRow.Cells("Kode").Value IsNot Nothing AndAlso existingRow.Cells("Kode").Value.ToString() = idBarang Then
                Dim currentQtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(existingRow.Cells("QtySat").Value))
                Dim newQtySat As Decimal = (_selectedQty * options(0).Value) + currentQtySat

                ' Validasi stok gabungan
                Dim errorCode As String = ""
                Dim errorMessage As String = ""
                Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                    cmd.Parameters.AddWithValue("@kode", idBarang)
                    cmd.Parameters.AddWithValue("@qty", newQtySat)
                    cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                    cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                    cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                    cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                    cmd.ExecuteNonQuery()
                    errorCode = Convert.ToString(cmd.Parameters("@error_code").Value)
                    errorMessage = Convert.ToString(cmd.Parameters("@error_message").Value)
                End Using

                If errorCode = "STOK_KURANG" Then
                    MessageBox.Show(errorMessage & vbCrLf & "Barang yang sama sudah ada di grid dengan qty " & currentQtySat.ToString("N2") & ".", "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    ' Hapus baris yang baru diisi
                    DgvDetail.Rows.RemoveAt(rowIdx)
                    NavigasiKeBarisDgvKosong()
                    Return
                End If

                Dim newQty As Decimal = ModuleAngka.ParseDecimal(existingRow.Cells("QTY").Value) + _selectedQty
                TerapkanSatuanKeRow(existingRow, options, options(0).Key, options(0).Value)
                existingRow.Cells("QTY").Value = newQty
                existingRow.Cells("StokToko").Value = stokToko
                existingRow.Cells("StokGudang").Value = stokGudang
                HitungBaris(existingRow.Index)
                ' Hapus baris kosong yang baru diisi
                DgvDetail.Rows.RemoveAt(rowIdx)
                NavigasiKeBarisDgvKosong()
                Return
            End If
        Next

        Dim row = DgvDetail.Rows(rowIdx)
        row.Cells("Kode").Value = idBarang
        row.Cells("NamaBarang").Value = namaBarang
        row.Cells("QTY").Value = _selectedQty
        TerapkanSatuanKeRow(row, options, options(0).Key, options(0).Value)
        row.Cells("StokToko").Value = stokToko
        row.Cells("StokGudang").Value = stokGudang
        row.Cells("HargaBeli").Value = hargaBeli
        HitungBaris(rowIdx)

        ' Validasi stok setelah input barang
        Dim qtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("QtySat").Value))
        If qtySat > 0D Then
            Dim errorCode As String = ""
            Dim errorMessage As String = ""
            Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                cmd.Parameters.AddWithValue("@kode", idBarang)
                cmd.Parameters.AddWithValue("@qty", qtySat)
                cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                cmd.ExecuteNonQuery()
                errorCode = Convert.ToString(cmd.Parameters("@error_code").Value)
                errorMessage = Convert.ToString(cmd.Parameters("@error_message").Value)
            End Using

            If errorCode = "STOK_KURANG" Then
                MessageBox.Show(errorMessage, "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                ' Hapus baris yang baru diisi
                DgvDetail.Rows.RemoveAt(rowIdx)
                NavigasiKeBarisDgvKosong()
                Return
            End If
        End If

        UpdateWarnaKodeBarang(rowIdx)
        NavigasiKeBarisDgvKosong(rowIdx)
    End Sub
#End Region


#Region "Data Barang (IsiBarangDariDb, TambahDataLangsung, Bersihkan)"
    Private Sub IsiBarangDariDb(keyword As String, Optional langsungTambah As Boolean = False)
        If String.IsNullOrWhiteSpace(keyword) Then Return
        Dim idBarang As String = "" : Dim namaBarang As String = ""
        Dim satKecil As String = "" : Dim isiKecil As Integer = 1
        Dim satSedang As String = "" : Dim isiSedang As Integer = 1
        Dim satBesar As String = "" : Dim isiBesar As Integer = 1
        Dim stokToko As Decimal = 0D : Dim stokGudang As Decimal = 0D
        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, " &
            "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_BESAR, STOK_TOKO, STOK_GUDANG " &
            "FROM tbl_barang WHERE STATUS='Aktif' AND " &
            "(NAMA_BARANG=@k OR BARCODE_KECIL=@k OR BARCODE_SEDANG=@k OR BARCODE_BESAR=@k) LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@k", keyword)
            Try
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        idBarang = rd("ID_BARANG").ToString()
                        namaBarang = rd("NAMA_BARANG").ToString()
                        satKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                        isiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        satSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                        isiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        satBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                        isiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                        stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                        stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    Else
                        MessageBox.Show("Barang '" & keyword & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        TxtNamaBarang.Clear()
                        TxtNamaBarang.Focus()
                        Return
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error membaca data barang: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using
        _selectedKodeBarang = idBarang
        _selectedStokToko = stokToko
        _selectedStokGudang = stokGudang
        _selectedSatuanOptions = New List(Of KeyValuePair(Of String, Integer))()
        If Not String.IsNullOrWhiteSpace(satKecil) Then _selectedSatuanOptions.Add(New KeyValuePair(Of String, Integer)(satKecil, isiKecil))
        If Not String.IsNullOrWhiteSpace(satSedang) Then _selectedSatuanOptions.Add(New KeyValuePair(Of String, Integer)(satSedang, isiSedang))
        If Not String.IsNullOrWhiteSpace(satBesar) Then _selectedSatuanOptions.Add(New KeyValuePair(Of String, Integer)(satBesar, isiBesar))
        If _selectedSatuanOptions.Count = 0 Then _selectedSatuanOptions.Add(New KeyValuePair(Of String, Integer)("PCS", 1))
        _selectedSatuan = _selectedSatuanOptions(0).Key
        _selectedIsi = _selectedSatuanOptions(0).Value
        LstBarang.Items.Clear()
        LstBarang.Visible = False
        TxtNamaBarang.Text = namaBarang
        If langsungTambah Then TambahDataLangsung()
    End Sub

    Private Sub TambahDataLangsung()
        Dim kode = _selectedKodeBarang.Trim()
        Dim nama = TxtNamaBarang.Text.Trim()
        Dim satuan = _selectedSatuan.Trim()
        Dim qty As Decimal = Math.Max(1D, _selectedQty)
        Dim isi As Integer = Math.Max(1, _selectedIsi)
        If String.IsNullOrEmpty(kode) OrElse String.IsNullOrEmpty(nama) Then
            MessageBox.Show("Kode barang dan nama barang wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Validasi stok sebelum tambah ke grid
        Dim hargaBeli As Decimal = AmbilHargaBeliBarang(kode)
        Dim qtySat As Decimal = qty * isi
        Dim errorCodeVal As String = ""
        Dim errorMessageVal As String = ""
        Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
            cmd.Parameters.AddWithValue("@kode", kode)
            cmd.Parameters.AddWithValue("@qty", qtySat)
            cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
            cmd.Parameters.AddWithValue("@izinkan_minus", 0)
            cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
            cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            errorCodeVal = Convert.ToString(cmd.Parameters("@error_code").Value)
            errorMessageVal = Convert.ToString(cmd.Parameters("@error_message").Value)
        End Using

        If errorCodeVal = "STOK_KURANG" Then
            MessageBox.Show(errorMessageVal, "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            BersihkanInputBarang()
            Return
        End If

        ' Jika barang sama sudah ada, tambahkan qty
        For Each existingRow As DataGridViewRow In DgvDetail.Rows
            If existingRow.IsNewRow Then Continue For
            If existingRow.Cells("Kode").Value IsNot Nothing AndAlso existingRow.Cells("Kode").Value.ToString() = kode Then
                Dim currentQtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(existingRow.Cells("QtySat").Value))
                Dim newQtySat As Decimal = qtySat + currentQtySat

                ' Validasi stok gabungan
                Dim errorCodeVal2 As String = ""
                Dim errorMessageVal2 As String = ""
                Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                    cmd.Parameters.AddWithValue("@kode", kode)
                    cmd.Parameters.AddWithValue("@qty", newQtySat)
                    cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                    cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                    cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                    cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                    cmd.ExecuteNonQuery()
                    errorCodeVal2 = Convert.ToString(cmd.Parameters("@error_code").Value)
                    errorMessageVal2 = Convert.ToString(cmd.Parameters("@error_message").Value)
                End Using

                If errorCodeVal2 = "STOK_KURANG" Then
                    MessageBox.Show(errorMessageVal2 & vbCrLf & "Barang yang sama sudah ada di grid dengan qty " & currentQtySat.ToString("N2") & ".", "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    BersihkanInputBarang()
                    Return
                End If

                Dim newQty As Decimal = ModuleAngka.ParseDecimal(existingRow.Cells("QTY").Value) + qty
                TerapkanSatuanKeRow(existingRow, AmbilSatuanByIdBarang(kode), satuan, isi)
                existingRow.Cells("QTY").Value = newQty
                existingRow.Cells("StokToko").Value = _selectedStokToko
                existingRow.Cells("StokGudang").Value = _selectedStokGudang
                HitungBaris(existingRow.Index)
                BersihkanInputBarang()
                Return
            End If
        Next

        ' Cari baris kosong paling atas
        Dim targetIdx As Integer = -1
        For i As Integer = 0 To DgvDetail.Rows.Count - 1
            If DgvDetail.Rows(i).IsNewRow Then Continue For
            If String.IsNullOrEmpty(Convert.ToString(DgvDetail.Rows(i).Cells("Kode").Value).Trim()) Then
                targetIdx = i : Exit For
            End If
        Next
        If targetIdx = -1 Then targetIdx = DgvDetail.Rows.Add()

        Dim r = DgvDetail.Rows(targetIdx)
        r.Cells("Kode").Value = kode
        r.Cells("NamaBarang").Value = nama
        r.Cells("QTY").Value = qty
        Dim opsiSatuan = If(_selectedSatuanOptions IsNot Nothing AndAlso _selectedSatuanOptions.Count > 0,
                            New List(Of KeyValuePair(Of String, Integer))(_selectedSatuanOptions),
                            AmbilSatuanByIdBarang(kode))
        TerapkanSatuanKeRow(r, opsiSatuan, satuan, isi)
        r.Cells("StokToko").Value = _selectedStokToko
        r.Cells("StokGudang").Value = _selectedStokGudang
        r.Cells("HargaBeli").Value = hargaBeli
        HitungBaris(targetIdx)
        UpdateWarnaKodeBarang(targetIdx)
        BersihkanInputBarang()
    End Sub

    ''' <summary>Navigasi ke baris kosong paling atas di DGV, fokus ke kolom NamaBarang.</summary>
    ''' <param name="skipRow">Index baris yang baru diisi — dilewati meski Kode belum ter-set oleh CellEndEdit.</param>
    Private Sub NavigasiKeBarisDgvKosong(Optional skipRow As Integer = -1)
        Debug.WriteLine($"[NavBarisDgvKosong] skipRow={skipRow} TotalRows={DgvDetail.Rows.Count}")
        For i As Integer = 0 To DgvDetail.Rows.Count - 1
            If DgvDetail.Rows(i).IsNewRow Then
                Debug.WriteLine($"  row={i} IsNewRow=True → skip")
                Continue For
            End If
            Dim kodeVal = Convert.ToString(DgvDetail.Rows(i).Cells("Kode").Value).Trim()
            Dim namaVal = Convert.ToString(DgvDetail.Rows(i).Cells("NamaBarang").Value).Trim()
            Debug.WriteLine($"  row={i} Kode='{kodeVal}' Nama='{namaVal}' skip={i = skipRow}")
            If i = skipRow Then Continue For
            If String.IsNullOrEmpty(kodeVal) Then
                Debug.WriteLine($"  → PILIH row={i} sebagai baris kosong")
                Try
                    DgvDetail.CurrentCell = DgvDetail(1, i)
                    DgvDetail.BeginEdit(True)
                Catch ex As Exception
                    Debug.WriteLine($"  → ERROR BeginEdit: {ex.Message}")
                End Try
                Return
            End If
        Next
        Debug.WriteLine($"  → Semua terisi, ke new row={DgvDetail.Rows.Count - 1}")
        Try
            DgvDetail.CurrentCell = DgvDetail(1, DgvDetail.Rows.Count - 1)
            DgvDetail.BeginEdit(True)
        Catch
        End Try
    End Sub

    Private Sub BersihkanInputBarang()
        Debug.WriteLine($"[BersihkanInputBarang] konteks={_konteksLstBarang} stack={New System.Diagnostics.StackTrace(1, False).GetFrame(0).GetMethod().Name}")
        _selectedKodeBarang = ""
        _selectedSatuan = "PCS"
        _selectedQty = 1D
        _selectedIsi = 1
        _selectedLevelIndex = -1
        _selectedStokToko = 0D
        _selectedStokGudang = 0D
        _selectedSatuanOptions.Clear()
        ' Guard agar TxtNamaBarang.Clear() tidak memicu TextChanged → search ulang
        _sedangSetNilaiDariListBox = True
        TxtNamaBarang.Clear()
        _sedangSetNilaiDariListBox = False
        LstBarang.Items.Clear()
        LstBarang.Visible = False

        ' Setup fokus berdasarkan setting
        SetupFocusToGrid()

        _konteksLstBarang = "TXTNAMA"
    End Sub
#End Region


#Region "Helper Satuan & Parsing"
    Private Function AmbilSatuanByIdBarang(idBarang As String) As List(Of KeyValuePair(Of String, Integer))
        Dim result As New List(Of KeyValuePair(Of String, Integer))()
        If String.IsNullOrWhiteSpace(idBarang) Then Return result
        Using cmd As New MySqlCommand(
            "SELECT SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_BESAR " &
            "FROM tbl_barang WHERE ID_BARANG=@id LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@id", idBarang)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    TambahSatuanOption(result, rd, "SATUAN_UMUM_KECIL", "ISI_UMUM_KECIL")
                    TambahSatuanOption(result, rd, "SATUAN_UMUM_SEDANG", "ISI_UMUM_SEDANG")
                    TambahSatuanOption(result, rd, "SATUAN_UMUM_BESAR", "ISI_UMUM_BESAR")
                End If
            End Using
        End Using
        If result.Count = 0 Then result.Add(New KeyValuePair(Of String, Integer)("PCS", 1))
        Return result
    End Function

    Private Sub TambahSatuanOption(list As List(Of KeyValuePair(Of String, Integer)), rd As IDataRecord, namaField As String, isiField As String)
        Dim nama As String = If(rd(namaField) Is DBNull.Value, "", rd(namaField).ToString().Trim())
        If String.IsNullOrWhiteSpace(nama) Then Return
        If list.Any(Function(x) x.Key.Equals(nama, StringComparison.OrdinalIgnoreCase)) Then Return
        Dim isi As Integer = 1
        If rd(isiField) IsNot DBNull.Value Then
            Dim parsed As Integer
            If Integer.TryParse(rd(isiField).ToString(), parsed) AndAlso parsed > 0 Then isi = parsed
        End If
        list.Add(New KeyValuePair(Of String, Integer)(nama, isi))
    End Sub

    ''' <summary>
    ''' Helper terpusat: hitung QtySat, HargaBeli, TotalHarga dari QTY dan Isi yang sudah ada di baris,
    ''' lalu update GrandTotal. Panggil ini setiap kali QTY, Isi/Satuan, atau HargaBeli berubah.
    ''' </summary>
    Private Sub HitungBaris(rowIdx As Integer)
        If rowIdx < 0 OrElse rowIdx >= DgvDetail.Rows.Count Then Return
        Dim row = DgvDetail.Rows(rowIdx)
        If row.IsNewRow Then Return

        Dim qty As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("QTY").Value))
        Dim isi As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("Isi").Value))
        Dim qtySat As Decimal = qty * isi

        ' Ambil harga dari DB jika sel kosong/nol
        Dim harga As Decimal = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
        If harga < 0D Then harga = 0D
        If harga = 0D Then
            Dim kode As String = Convert.ToString(row.Cells("Kode").Value).Trim()
            If Not String.IsNullOrWhiteSpace(kode) Then harga = AmbilHargaBeliBarang(kode)
        End If

        row.Cells("QTY").Value = qty
        row.Cells("Isi").Value = isi
        row.Cells("QtySat").Value = qtySat
        row.Cells("HargaBeli").Value = harga
        row.Cells("TotalHarga").Value = qtySat * harga
        HitungGrandTotal()
    End Sub

    Private Sub TerapkanSatuanKeRow(row As DataGridViewRow, options As List(Of KeyValuePair(Of String, Integer)), selectedSatuan As String, selectedIsi As Integer)
        Dim comboCell = TryCast(row.Cells("Satuan"), DataGridViewComboBoxCell)
        If comboCell Is Nothing Then Return
        comboCell.Items.Clear()
        For Each opt In options
            comboCell.Items.Add(opt.Key)
        Next
        Dim satuanPakai = selectedSatuan
        If _selectedLevelIndex >= 0 AndAlso _selectedLevelIndex < options.Count Then
            satuanPakai = options(_selectedLevelIndex).Key
            selectedIsi = options(_selectedLevelIndex).Value
        ElseIf String.IsNullOrWhiteSpace(satuanPakai) OrElse Not options.Any(Function(x) x.Key.Equals(satuanPakai, StringComparison.OrdinalIgnoreCase)) Then
            satuanPakai = options(0).Key
            selectedIsi = options(0).Value
        End If
        row.Cells("Satuan").Value = satuanPakai
        row.Cells("Isi").Value = Math.Max(1, selectedIsi)
    End Sub

    ''' <summary>Isi semua kolom baris DGV langsung dari DB berdasarkan nama barang — tanpa lewat CellEndEdit.</summary>
    Private Sub IsiBarangKeRow(rowIdx As Integer, namaBarang As String, qty As Decimal)
        If rowIdx < 0 OrElse rowIdx >= DgvDetail.Rows.Count Then Return
        Dim idBarang As String = "" : Dim satKecil As String = "" : Dim isiKecil As Integer = 1
        Dim satSedang As String = "" : Dim isiSedang As Integer = 1
        Dim satBesar As String = "" : Dim isiBesar As Integer = 1
        Dim stokToko As Decimal = 0D : Dim stokGudang As Decimal = 0D
        Dim hargaBeli As Decimal = 0D
        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, HARGA_BELI, SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, " &
                "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_BESAR, " &
                "STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                "WHERE STATUS='Aktif' AND TRIM(NAMA_BARANG)=@n LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang.Trim())
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then Return
                    idBarang = rd("ID_BARANG").ToString()
                    hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    satKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                    isiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                    satSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                    isiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                    satBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                    isiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                    stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                End Using
            End Using
        Catch
            Return
        End Try

        Dim options As New List(Of KeyValuePair(Of String, Integer))()
        If Not String.IsNullOrWhiteSpace(satKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satKecil, isiKecil))
        If Not String.IsNullOrWhiteSpace(satSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satSedang, isiSedang))
        If Not String.IsNullOrWhiteSpace(satBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satBesar, isiBesar))
        If options.Count = 0 Then options.Add(New KeyValuePair(Of String, Integer)("PCS", 1))

        Dim row = DgvDetail.Rows(rowIdx)
        row.Cells("Kode").Value = idBarang
        row.Cells("NamaBarang").Value = namaBarang
        TerapkanSatuanKeRow(row, options, options(0).Key, options(0).Value)
        row.Cells("QTY").Value = qty
        row.Cells("HargaBeli").Value = hargaBeli.ToString("#,0.##", cultureIndonesia)
        row.Cells("StokToko").Value = stokToko
        row.Cells("StokGudang").Value = stokGudang
        HitungBaris(rowIdx)
        UpdateWarnaKodeBarang(rowIdx)
        Debug.WriteLine($"[IsiBarangKeRow] row={rowIdx} Kode='{idBarang}' Nama='{namaBarang}' Qty={qty}")
    End Sub

    ''' <summary>Ambil ID_BARANG dari nama barang.</summary>
    Private Function AmbilKodeBarangDariNama(namaBarang As String) As String
        If String.IsNullOrWhiteSpace(namaBarang) Then Return ""
        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND TRIM(NAMA_BARANG)=@n LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang.Trim())
                Dim val = cmd.ExecuteScalar()
                Return If(val Is Nothing OrElse val Is DBNull.Value, "", val.ToString())
            End Using
        Catch
            Return ""
        End Try
    End Function

    Private Function AmbilHargaBeliBarang(kodeBarang As String) As Decimal
        Using cmd As New MySqlCommand("SELECT COALESCE(HARGA_BELI,0) FROM tbl_barang WHERE ID_BARANG=@kode LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@kode", kodeBarang)
            Dim val = cmd.ExecuteScalar()
            Return ModuleAngka.ParseDecimal(val)
        End Using
    End Function

    ' ParseDecimalSafe dan ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal

    Private Sub HitungGrandTotal()
        Dim total As Decimal = 0D
        For Each row As DataGridViewRow In DgvDetail.Rows
            If row.IsNewRow Then Continue For
            total += ModuleAngka.ParseDecimal(row.Cells("TotalHarga").Value)
        Next
        TxtGrantotal.Text = total.ToString("#,0.##", cultureIndonesia)
    End Sub
#End Region


#Region "Cabang & Supabase"
    Private Sub InitSupabase()
        Dim url As String = AppConfig.Instance.GetValue(Of String)("SupabaseUrl", "")
        Dim key As String = AppConfig.Instance.GetValue(Of String)("SupabaseKey", "")
        If Not String.IsNullOrEmpty(url) AndAlso Not String.IsNullOrEmpty(key) Then
            SupabaseHelper.Init(url, key)
        End If
    End Sub

    Private Sub MuatDaftarCabang()
        CmbCabangTujuan.Items.Clear()
        Try
            Using cmd As New MySqlCommand("SELECT kode_cabang, nama_cabang FROM tbl_cabang ORDER BY kode_cabang", conn)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        CmbCabangTujuan.Items.Add($"{rd("kode_cabang")} - {rd("nama_cabang")}")
                    End While
                End Using
            End Using
        Catch
        End Try
    End Sub

    Private Function AmbilKodeCabangTujuan() As String
        Dim txt = CmbCabangTujuan.Text.Trim()
        If String.IsNullOrEmpty(txt) Then Return ""
        Dim idx = txt.IndexOf(" - ", StringComparison.Ordinal)
        If idx > 0 Then Return txt.Substring(0, idx).Trim()
        Return txt
    End Function

    Private Sub BtnRefreshCabang_Click(sender As Object, e As EventArgs) Handles BtnRefreshCabang.Click
        If Not SupabaseHelper.IsInitialized() OrElse Not SupabaseHelper.CekKoneksi() Then
            SetStatus("Status: offline, daftar cabang dari lokal.")
            MuatDaftarCabang()
            Return
        End If
        Try
            Dim rows = SupabaseHelper.Get("cabang_master", "select=kode_cabang,nama_cabang&order=kode_cabang.asc")
            For Each row In rows
                UpsertCabangLokal(row("kode_cabang").ToString(), If(row("nama_cabang") Is Nothing, "", row("nama_cabang").ToString()), "cloud")
            Next
            MuatDaftarCabang()
            SetStatus($"Status: refresh cabang cloud sukses ({rows.Count} cabang).")
        Catch ex As Exception
            SetStatus("Status: gagal refresh cloud: " & ex.Message)
        End Try
    End Sub

    Private Sub UpsertCabangLokal(kode As String, nama As String, sumber As String)
        Try
            Using cmd As New MySqlCommand(
                "INSERT INTO tbl_cabang (kode_cabang, nama_cabang, sumber, updated_at) VALUES (@kode, @nama, @sumber, NOW())
                 ON DUPLICATE KEY UPDATE nama_cabang=VALUES(nama_cabang), sumber=VALUES(sumber), updated_at=NOW()", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                cmd.Parameters.AddWithValue("@nama", nama)
                cmd.Parameters.AddWithValue("@sumber", sumber)
                cmd.ExecuteNonQuery()
            End Using
        Catch
        End Try
    End Sub
#End Region


#Region "Transfer Masuk (TERIMA)"

    Private Sub MuatTransferMasuk()
        If _dgvMasuk Is Nothing Then Return
        Try
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT sumber_transfer AS sumber, COALESCE(id_cloud,'') AS id_cloud,
                        id_transfer, dari_cabang, kode_barang, nama_barang,
                        qty_satuan, COALESCE(satuan,'') AS satuan,
                        COALESCE(harga_beli,0) AS harga_beli,
                        COALESCE(keterangan,'') AS keterangan,
                        COALESCE(tgl_kirim,'') AS tgl_kirim,
                        status_transfer
                 FROM transfer_masuk_manual
                 WHERE status_transfer='PENDING'
                 ORDER BY COALESCE(tgl_kirim, created_at) ASC", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            _dgvMasuk.DataSource = Nothing
            _dgvMasuk.DataSource = dt
            Dim headers As New Dictionary(Of String, String) From {
                {"sumber", "Sumber"}, {"id_transfer", "ID Transfer"}, {"dari_cabang", "Dari Cabang"},
                {"kode_barang", "Kode"}, {"nama_barang", "Nama Barang"}, {"qty_satuan", "Qty Satuan"},
                {"satuan", "Satuan"}, {"harga_beli", "Harga Beli"}, {"keterangan", "Keterangan"},
                {"tgl_kirim", "Tgl Kirim"}, {"status_transfer", "Status"}}
            For Each pair In headers
                If _dgvMasuk.Columns.Contains(pair.Key) Then _dgvMasuk.Columns(pair.Key).HeaderText = pair.Value
            Next
            If _dgvMasuk.Columns.Contains("id_cloud") Then _dgvMasuk.Columns("id_cloud").Visible = False
            ModuleAngka.TerapkanFormatKolomAngka(_dgvMasuk, "harga_beli")
            _dgvMasuk.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            _dgvMasuk.RowHeadersVisible = False
            _dgvMasuk.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            _dgvMasuk.MultiSelect = True
            _dgvMasuk.EnableHeadersVisualStyles = False
            _dgvMasuk.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
            _dgvMasuk.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            _dgvMasuk.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray
            SetStatus($"Status: {dt.Rows.Count} transfer masuk pending.")
        Catch ex As Exception
            SetStatus("Status: gagal muat transfer masuk — " & ex.Message)
        End Try
    End Sub

    ''' <summary>Cache data transfer dari Supabase ke lokal. Tidak overwrite status DITERIMA.</summary>
    Private Sub SimpanTransferMasukKeLokal(row As Newtonsoft.Json.Linq.JObject)
        Try
            Dim g = Function(key As String) As String
                        Return If(row(key) IsNot Nothing AndAlso row(key).Type <> Newtonsoft.Json.Linq.JTokenType.Null, row(key).ToString(), "")
                    End Function
            Dim idCloud As String = g("id")
            If String.IsNullOrEmpty(idCloud) Then Return
            Dim idTransfer As String = "CLD-" & idCloud.Substring(0, 8).ToUpper()
            Dim qty As Decimal = ModuleAngka.ParseDecimal(row("qty"))
            Dim qtySat As Decimal = ModuleAngka.ParseDecimal(row("qty_satuan"))
            Dim isi As Integer = ModuleAngka.ParseInteger(row("isi_satuan"), 1)
            Dim tglKirim As String = If(row("tgl_kirim") IsNot Nothing AndAlso row("tgl_kirim").Type <> Newtonsoft.Json.Linq.JTokenType.Null,
                                        row("tgl_kirim").ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            Using cmd As New MySqlCommand(
                "INSERT INTO transfer_masuk_manual
                    (id_transfer, id_cloud, sumber_transfer, dari_cabang, ke_cabang,
                     kode_barang, nama_barang, qty, satuan, isi_satuan, qty_satuan,
                     keterangan, tgl_kirim, status_transfer)
                 VALUES
                    (@id, @idCloud, 'CLOUD', @dari, @ke,
                     @kode, @nama, @qty, @sat, @isi, @qtySat,
                     @ket, @tglKirim, 'PENDING')
                 ON DUPLICATE KEY UPDATE
                    qty_satuan = VALUES(qty_satuan),
                    tgl_kirim  = VALUES(tgl_kirim),
                    status_transfer = IF(status_transfer='DITERIMA','DITERIMA','PENDING')", conn)
                cmd.Parameters.AddWithValue("@id", idTransfer)
                cmd.Parameters.AddWithValue("@idCloud", idCloud)
                cmd.Parameters.AddWithValue("@dari", g("dari_toko"))
                cmd.Parameters.AddWithValue("@ke", g("ke_toko"))
                cmd.Parameters.AddWithValue("@kode", g("kode_barang"))
                cmd.Parameters.AddWithValue("@nama", g("nama_barang"))
                cmd.Parameters.AddWithValue("@qty", qty)
                cmd.Parameters.AddWithValue("@sat", g("satuan"))
                cmd.Parameters.AddWithValue("@isi", isi)
                cmd.Parameters.AddWithValue("@qtySat", qtySat)
                cmd.Parameters.AddWithValue("@ket", g("keterangan"))
                cmd.Parameters.AddWithValue("@tglKirim", tglKirim)
                cmd.ExecuteNonQuery()
            End Using
        Catch
        End Try
    End Sub

    Private Sub BtnRefreshMasuk_Click(sender As Object, e As EventArgs)
        SetStatus("Status: mengambil data transfer dari cloud...")
        Me.Cursor = Cursors.WaitCursor
        Try
            If SupabaseHelper.IsInitialized() AndAlso SupabaseHelper.CekKoneksi() Then
                ' Upload konfirmasi terima yang pending dulu
                UploadKonfirmasiTerimaPending()
                ' Download transfer masuk terbaru dari Supabase (pakai last_sync)
                SyncManager.SyncDownloadTransfer()
                SetStatus("Status: sync cloud selesai.")
            Else
                SetStatus("Status: offline — menampilkan data lokal.")
            End If
        Catch ex As Exception
            SetStatus("Status: gagal sync cloud — " & ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        MuatTransferMasuk()
        MuatTransferKeluarOfflinePending()
    End Sub

    Private Sub BtnImportManualMasuk_Click(sender As Object, e As EventArgs)
        Using ofd As New OpenFileDialog() With {.Filter = "CSV files|*.csv", .Title = "Import File Transfer Manual"}
            If ofd.ShowDialog() <> DialogResult.OK Then Return
            Dim lines As String()
            Try
                lines = File.ReadAllLines(ofd.FileName, Encoding.UTF8)
            Catch ex As Exception
                MessageBox.Show("Gagal baca file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
            If lines.Length <= 1 Then
                MessageBox.Show("File kosong atau hanya berisi header.", "Import Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            ' Validasi header
            ' Header baru menyertakan harga_beli; header lama (tanpa harga_beli) tetap diterima untuk kompatibilitas
            Dim headerLower = lines(0).ToLower().Replace(" ", "")
            Dim expectedHeaderBaru = "id_transfer,dari_cabang,ke_cabang,kode_barang,nama_barang,qty,satuan,isi_satuan,qty_satuan,harga_beli,keterangan"
            Dim expectedHeaderLama = "id_transfer,dari_cabang,ke_cabang,kode_barang,nama_barang,qty,satuan,isi_satuan,qty_satuan,keterangan"
            Dim adaKolomHarga As Boolean = headerLower.Contains("harga_beli")
            If Not headerLower.StartsWith(expectedHeaderBaru.Replace(" ", "")) AndAlso
               Not headerLower.StartsWith(expectedHeaderLama.Replace(" ", "")) Then
                MessageBox.Show("Format header CSV tidak sesuai." & Environment.NewLine &
                                "Header yang diharapkan:" & Environment.NewLine & expectedHeaderBaru,
                                "Format Salah", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Dim sukses As Integer = 0 : Dim lewat As Integer = 0 : Dim gagal As Integer = 0
            For i As Integer = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then Continue For
                Try
                    Dim cols = ParseCsvLine(lines(i))
                    If cols.Count < 10 Then gagal += 1 : Continue For
                    Dim idTransfer = cols(0).Trim()
                    Dim dariCabang = cols(1).Trim()
                    Dim keCabang = cols(2).Trim()
                    Dim kodeBarang = cols(3).Trim()
                    ' Validasi: ke_cabang harus cocok dengan cabang ini
                    If Not String.IsNullOrEmpty(keCabang) AndAlso
                       Not keCabang.Equals(SyncConfig.KodeCabang, StringComparison.OrdinalIgnoreCase) Then
                        lewat += 1 : Continue For  ' bukan untuk cabang ini
                    End If
                    If String.IsNullOrEmpty(idTransfer) OrElse String.IsNullOrEmpty(kodeBarang) Then
                        gagal += 1 : Continue For
                    End If
                    ' Parsing harga_beli dan keterangan sesuai format CSV (baru vs lama)
                    Dim hargaBeliCsv As Decimal = 0D
                    Dim keteranganCsv As String = ""
                    If adaKolomHarga AndAlso cols.Count > 10 Then
                        hargaBeliCsv = ModuleAngka.ParseDecimal(cols(9).Trim())
                        keteranganCsv = cols(10).Trim()
                    ElseIf cols.Count > 9 Then
                        keteranganCsv = cols(9).Trim()
                    End If
                    Using cmd As New MySqlCommand(
                        "INSERT IGNORE INTO transfer_masuk_manual
                            (id_transfer, sumber_transfer, dari_cabang, ke_cabang, kode_barang, nama_barang,
                             qty, satuan, isi_satuan, qty_satuan, harga_beli, keterangan, status_transfer)
                         VALUES
                            (@id, 'CSV', @dari, @ke, @kode, @nama,
                             @qty, @sat, @isi, @qtySat, @hargaBeli, @ket, 'PENDING')", conn)
                        cmd.Parameters.AddWithValue("@id", idTransfer)
                        cmd.Parameters.AddWithValue("@dari", dariCabang)
                        cmd.Parameters.AddWithValue("@ke", keCabang)
                        cmd.Parameters.AddWithValue("@kode", kodeBarang)
                        cmd.Parameters.AddWithValue("@nama", cols(4).Trim())
                        cmd.Parameters.AddWithValue("@qty", ModuleAngka.ParseDecimal(cols(5)))
                        cmd.Parameters.AddWithValue("@sat", cols(6).Trim())
                        cmd.Parameters.AddWithValue("@isi", If(Integer.TryParse(cols(7), 0), CInt(cols(7)), 1))
                        cmd.Parameters.AddWithValue("@qtySat", ModuleAngka.ParseDecimal(cols(8)))
                        cmd.Parameters.AddWithValue("@hargaBeli", hargaBeliCsv)
                        cmd.Parameters.AddWithValue("@ket", keteranganCsv)
                        Dim affected = cmd.ExecuteNonQuery()
                        If affected > 0 Then sukses += 1 Else lewat += 1
                    End Using
                Catch
                    gagal += 1
                End Try
            Next
            MuatTransferMasuk()
            SetStatus($"Status: import CSV — {sukses} baru, {lewat} dilewati, {gagal} gagal.")
            MessageBox.Show($"Import selesai:{Environment.NewLine}" &
                            $"  {sukses} baris berhasil diimport{Environment.NewLine}" &
                            $"  {lewat} baris dilewati (duplikat atau bukan untuk cabang ini){Environment.NewLine}" &
                            $"  {gagal} baris gagal",
                            "Hasil Import", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    Private Sub BtnTerimaTerpilih_Click(sender As Object, e As EventArgs)
        If _dgvMasuk.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih minimal satu baris transfer untuk diterima.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim konfirmasi = MessageBox.Show(
            $"Terima {_dgvMasuk.SelectedRows.Count} item transfer masuk?",
            "Konfirmasi Terima", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi <> DialogResult.Yes Then Return

        Dim sukses As Integer = 0 : Dim gagal As Integer = 0
        Dim pesanGagal As New List(Of String)()
        Me.Cursor = Cursors.WaitCursor
        Try
            For Each row As DataGridViewRow In _dgvMasuk.SelectedRows
                Dim sumber = Convert.ToString(row.Cells("sumber").Value)
                Dim idCloud = Convert.ToString(row.Cells("id_cloud").Value)
                Dim idTransfer = Convert.ToString(row.Cells("id_transfer").Value)
                Dim kodeBarang = Convert.ToString(row.Cells("kode_barang").Value)
                Dim namaBarang = Convert.ToString(row.Cells("nama_barang").Value)
                Dim dariCabang = Convert.ToString(row.Cells("dari_cabang").Value)
                Dim qtySat As Decimal
                If Not Decimal.TryParse(Convert.ToString(row.Cells("qty_satuan").Value), qtySat) OrElse qtySat <= 0D Then
                    pesanGagal.Add($"{kodeBarang}: qty tidak valid")
                    gagal += 1 : Continue For
                End If
                Dim hargaBeliRow As Decimal = 0D
                If _dgvMasuk.Columns.Contains("harga_beli") Then
                    hargaBeliRow = ModuleAngka.ParseDecimal(row.Cells("harga_beli").Value)
                End If
                Dim ok = TerimaTransferDenganAudit(sumber, idCloud, idTransfer, kodeBarang, namaBarang, dariCabang, qtySat, TxtKeterangan.Text.Trim(), hargaBeliRow)
                If ok Then sukses += 1 Else gagal += 1
            Next
        Finally
            Me.Cursor = Cursors.Default
        End Try

        MuatTransferMasuk()
        Dim pesan = $"Selesai: {sukses} berhasil diterima"
        If gagal > 0 Then pesan &= $", {gagal} gagal"
        SetStatus("Status: " & pesan & ".")
        MessageBox.Show(pesan & ".", If(gagal = 0, "Sukses", "Sebagian Gagal"),
                        MessageBoxButtons.OK, If(gagal = 0, MessageBoxIcon.Information, MessageBoxIcon.Warning))
    End Sub

    Private Function SudahPernahDiterima(idTransfer As String, kodeBarang As String) As Boolean
        Using cmd As New MySqlCommand(
            "SELECT 1 FROM HistoryBarang WHERE FAKTUR=@faktur AND ID_BARANG=@kode AND JENIS='TRANSFER_CABANG_MASUK' LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@faktur", idTransfer)
            cmd.Parameters.AddWithValue("@kode", kodeBarang)
            Return cmd.ExecuteScalar() IsNot Nothing
        End Using
    End Function

    ''' <summary>
    ''' Satu-satunya method untuk menerima transfer masuk.
    ''' Menangani CLOUD, CLOUD_LOKAL, dan CSV dalam satu alur terpadu.
    ''' Transaksi atomik: update stok + history + status + jurnal + hitung stok.
    ''' </summary>
    Private Function TerimaTransferDenganAudit(sumber As String, idCloud As String, idTransfer As String,
                                               kodeBarang As String, namaBarang As String,
                                               dariCabang As String, qtySatuan As Decimal,
                                               Optional catatanTerima As String = "",
                                               Optional hargaBeliTransfer As Decimal = 0D) As Boolean
        ' Cek duplikat via HistoryBarang
        If SudahPernahDiterima(idTransfer, kodeBarang) Then
            MessageBox.Show($"Transfer {idTransfer} untuk {kodeBarang} sudah pernah diterima.",
                            "Sudah Diterima", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End If
        ' Validasi barang ada di DB lokal
        Using cmd As New MySqlCommand("SELECT COUNT(1) FROM tbl_barang WHERE ID_BARANG=@kode", conn)
            cmd.Parameters.AddWithValue("@kode", kodeBarang)
            If ModuleAngka.ParseInteger(cmd.ExecuteScalar(), 0) = 0 Then
                MessageBox.Show($"Barang '{kodeBarang}' tidak ditemukan di database lokal.",
                                "Barang Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End Using
        Try
            ' Satu transaksi: update stok + history + status + jurnal + hitung stok
            Using trx = conn.BeginTransaction()
                ' Hitung harga sekali di awal — dipakai di HistoryBarang dan Jurnal
                Dim hargaBeli As Decimal = If(hargaBeliTransfer > 0D, hargaBeliTransfer, AmbilHargaBeliBarang(kodeBarang))
                Dim nilaiTransfer As Decimal = qtySatuan * hargaBeli

                ' 1. Tambah stok masuk
                Using cmd As New MySqlCommand(
                    "UPDATE tbl_barang SET
                        TRANSFER_CABANG_MASUK_TOKO = TRANSFER_CABANG_MASUK_TOKO + @qty,
                        is_dirty = 1, version = version + 1, updated_by = @user
                     WHERE ID_BARANG = @kode", conn, trx)
                    cmd.Parameters.AddWithValue("@qty", qtySatuan)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                    If cmd.ExecuteNonQuery() = 0 Then Throw New Exception($"Barang {kodeBarang} tidak ditemukan saat update stok.")
                End Using
                ' 2. History audit trail
                Using cmd As New MySqlCommand(
                    "INSERT INTO HistoryBarang
                        (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
                         QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
                     VALUES
                        (@faktur, @tgl, 'TRANSFER_CABANG_MASUK', @lokasi, @kode, @nama,
                         @qty, 'QTY_SATUAN', 1, @qty, @totalRupiah, @user, @pc)", conn, trx)
                    cmd.Parameters.AddWithValue("@faktur", idTransfer)
                    cmd.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@lokasi", SyncConfig.KodeCabang)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    cmd.Parameters.AddWithValue("@nama", namaBarang)
                    cmd.Parameters.AddWithValue("@qty", qtySatuan)
                    cmd.Parameters.AddWithValue("@totalRupiah", qtySatuan * hargaBeli)
                    cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                    cmd.Parameters.AddWithValue("@pc", FormUtama.StatusNamaPC.Text)
                    cmd.ExecuteNonQuery()
                End Using
                ' 3. Update status lokal + catat waktu, user, dan catatan terima
                Using cmd As New MySqlCommand(
                    "UPDATE transfer_masuk_manual
                     SET status_transfer='DITERIMA', tgl_terima=@tgl, id_user_terima=@user, catatan_terima=@catatan
                     WHERE (id_transfer=@id OR id_cloud=@idCloud) AND kode_barang=@kode", conn, trx)
                    cmd.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                    cmd.Parameters.AddWithValue("@catatan", If(String.IsNullOrEmpty(catatanTerima), CType(DBNull.Value, Object), catatanTerima))
                    cmd.Parameters.AddWithValue("@id", idTransfer)
                    cmd.Parameters.AddWithValue("@idCloud", If(String.IsNullOrEmpty(idCloud), "", idCloud))
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    cmd.ExecuteNonQuery()
                End Using
                ' 4. Jurnal akuntansi terima barang — pakai harga dari transfer jika ada, fallback ke DB lokal
                If nilaiTransfer > 0D Then
                    Dim uraian = $"Terima transfer dari {dariCabang} — {namaBarang} ({qtySatuan:N2})"
                    Using cmd As New MySqlCommand(
                        "INSERT INTO JurnalUmum
                            (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D,
                             NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
                         VALUES
                            (@no, @tgl, @uraian, @namaD, @noD, @namaK, @noK,
                             @nominal, 'Transfer cabang masuk', @lokasi, @user, @pc)", conn, trx)
                        cmd.Parameters.AddWithValue("@no", idTransfer)
                        cmd.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmd.Parameters.AddWithValue("@uraian", uraian)
                        ' Terima barang: D PERSEDIAAN BARANG, K REKENING KORAN PUSAT (04.01.003)
                        cmd.Parameters.AddWithValue("@namaD", NAMA_REK_BARANG)
                        cmd.Parameters.AddWithValue("@noD", KODE_REK_BARANG)
                        cmd.Parameters.AddWithValue("@namaK", "REKENING KORAN PUSAT")
                        cmd.Parameters.AddWithValue("@noK", "04.01.003")
                        cmd.Parameters.AddWithValue("@nominal", nilaiTransfer)
                        cmd.Parameters.AddWithValue("@lokasi", SyncConfig.KodeCabang)
                        cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                        cmd.Parameters.AddWithValue("@pc", FormUtama.StatusNamaPC.Text)
                        cmd.ExecuteNonQuery()
                    End Using


                    Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                    Using cmdAkun As New MySqlCommand(
                        "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                        "UNION " &
                        "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                        conn, trx)
                        cmdAkun.Parameters.AddWithValue("@fk", idTransfer)
                        Using rd = cmdAkun.ExecuteReader()
                            While rd.Read()
                                Dim kode As String = rd(0).ToString().Trim()
                                If kode <> "" Then akunTerlibat.Add(kode)
                            End While
                        End Using
                    End Using
                    For Each kodeAkun As String In akunTerlibat
                        UpdateSaldoAkun(kodeAkun, trx)
                    Next
                End If
                ' 5. Hitung ulang stok dalam transaksi yang sama
                HitungStokPerubahan(kodeBarang, trx)
                trx.Commit()
            End Using

            ' 6. Konfirmasi ke Supabase (di luar transaksi DB lokal — boleh gagal, ada antrian)
            If Not String.IsNullOrEmpty(idCloud) Then
                If SupabaseHelper.IsInitialized() AndAlso SupabaseHelper.CekKoneksi() Then
                    Try
                        SupabaseHelper.Patch("transfer_barang_cloud", $"id=eq.{idCloud}",
                            New Dictionary(Of String, Object) From {
                                {"status", "diterima"},
                                {"id_user_terima", ModuleVariabel.NamaUser},
                                {"tgl_terima", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}})
                    Catch
                        ' Simpan antrian konfirmasi untuk dikirim saat online
                        SimpanAntrianKonfirmasiTerima(idCloud, kodeBarang)
                    End Try
                Else
                    SimpanAntrianKonfirmasiTerima(idCloud, kodeBarang)
                End If
            End If

            SyncLog.Tulis("TERIMA", "transfer", kodeBarang, idCloud,
                          $"Diterima dari {dariCabang} qty={qtySatuan:N2} sumber={sumber}")
            Return True
        Catch ex As Exception
            MessageBox.Show($"Gagal terima transfer {kodeBarang}: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Private Sub SimpanAntrianKonfirmasiTerima(idCloud As String, kodeBarang As String)
        Try
            Using cmd As New MySqlCommand(
                "INSERT IGNORE INTO transfer_terima_pending (id_cloud, kode_barang, id_user) VALUES (@idCloud, @kode, @user)", conn)
                cmd.Parameters.AddWithValue("@idCloud", idCloud)
                cmd.Parameters.AddWithValue("@kode", kodeBarang)
                cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                cmd.ExecuteNonQuery()
            End Using
        Catch
        End Try
    End Sub

    Public Sub UploadKonfirmasiTerimaPending()
        SyncManager.UploadKonfirmasiTerimaPending()
    End Sub
#End Region


#Region "Transfer Keluar (KIRIM)"
    Private Function ValidasiSebelumProses() As Boolean
        If DgvDetail.Rows.Count = 0 Then
            MessageBox.Show("Detail transfer masih kosong.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Dim kodeTujuan = AmbilKodeCabangTujuan()
        If String.IsNullOrEmpty(kodeTujuan) Then
            MessageBox.Show("Cabang tujuan wajib dipilih.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        ' Validasi cabang tujuan tidak sama dengan cabang asal
        If kodeTujuan.Equals(SyncConfig.KodeCabang, StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Cabang tujuan tidak boleh sama dengan cabang asal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Function ValidasiStokKirim() As Boolean
        For Each row As DataGridViewRow In DgvDetail.Rows
            If row.IsNewRow Then Continue For
            Dim kode As String = Convert.ToString(row.Cells("Kode").Value).Trim()
            If String.IsNullOrWhiteSpace(kode) Then Continue For
            Dim qtySat As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("QtySat").Value))
            If qtySat <= 0D Then
                MessageBox.Show($"Qty transfer untuk barang {kode} harus lebih besar dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            ' Gunakan stored procedure untuk validasi stok real-time (anti race condition)
            Dim errorCode As String = ""
            Dim errorMessage As String = ""
            Using cmd As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan_minus, @error_code, @error_message)", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                cmd.Parameters.AddWithValue("@qty", qtySat)
                cmd.Parameters.AddWithValue("@lokasi", LokasiBarang)
                cmd.Parameters.AddWithValue("@izinkan_minus", 0)
                cmd.Parameters.Add("@error_code", MySqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@error_message", MySqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                cmd.ExecuteNonQuery()
                errorCode = Convert.ToString(cmd.Parameters("@error_code").Value)
                errorMessage = Convert.ToString(cmd.Parameters("@error_message").Value)
            End Using

            If errorCode = "STOK_KURANG" Then
                MessageBox.Show(errorMessage, "Stok Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        Next
        Return True
    End Function

    ''' <summary>Tampilkan status dengan warna otomatis sesuai konteks pesan.</summary>
    Private Sub SetStatus(pesan As String)
        LblStatus.Text = pesan
        Dim lower = pesan.ToLowerInvariant()
        If lower.Contains("gagal") OrElse lower.Contains("error") OrElse lower.Contains("tidak ada") Then
            LblStatus.BackColor = Color.Firebrick
            LblStatus.ForeColor = Color.White
        ElseIf lower.Contains("sukses") OrElse lower.Contains("selesai") OrElse lower.Contains("berhasil") OrElse lower.Contains("diterima") Then
            LblStatus.BackColor = Color.SeaGreen
            LblStatus.ForeColor = Color.White
        ElseIf lower.Contains("offline") OrElse lower.Contains("pending") OrElse lower.Contains("menunggu") Then
            LblStatus.BackColor = Color.Goldenrod
            LblStatus.ForeColor = Color.Black
        ElseIf lower.Contains("mengambil") OrElse lower.Contains("sync") OrElse lower.Contains("upload") Then
            LblStatus.BackColor = Color.SteelBlue
            LblStatus.ForeColor = Color.White
        Else
            LblStatus.BackColor = SystemColors.Control
            LblStatus.ForeColor = SystemColors.ControlText
        End If
    End Sub

    Private Sub BtnBarang_Click(sender As Object, e As EventArgs) Handles BtnBarang.Click
        Using f As New TambahBarang()
            f.LblHeaderForm.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using
        ' Refresh autocomplete agar barang baru langsung tersedia di TxtNamaBarang
        ConfigureAutoCompleteTxtNama()
    End Sub

    Private Sub BtnPelanggan_Click(sender As Object, e As EventArgs) Handles BtnPelanggan.Click
        Using f As New FormCabang()
            f.ShowDialog()
        End Using
        MuatDaftarCabang()
    End Sub

    Private Sub BtnKirimCloud_Click(sender As Object, e As EventArgs) Handles BtnKirimCloud.Click
        If Not ValidasiSebelumProses() Then Return
        If Not ValidasiStokKirim() Then Return
        Dim kodeTujuan = AmbilKodeCabangTujuan()
        If Not (SupabaseHelper.IsInitialized() AndAlso SupabaseHelper.CekKoneksi()) Then
            KirimTransferOffline(kodeTujuan)
            Return
        End If
        Try
            Dim cek = SupabaseHelper.Get("cabang_master", $"kode_cabang=eq.{Uri.EscapeDataString(kodeTujuan)}&select=kode_cabang")
            If cek.Count = 0 Then
                Dim jawab = MessageBox.Show("Cabang tujuan belum terdaftar di cloud." & vbCrLf &
                    "Kirim sebagai transfer offline?", "Cabang Belum Terdaftar", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If jawab = DialogResult.Yes Then KirimTransferOffline(kodeTujuan) Else SetStatus("Status: kirim dibatalkan.")
                Return
            End If
            Dim sukses As Integer = 0
            Dim itemSukses As New List(Of TransferItemSnapshot)()
            For Each row As DataGridViewRow In DgvDetail.Rows
                If row.IsNewRow Then Continue For
                Dim ok = SyncManager.KirimTransfer(
                    row.Cells("Kode").Value.ToString(), row.Cells("NamaBarang").Value.ToString(), kodeTujuan,
                    ModuleAngka.ParseDecimal(row.Cells("QTY").Value), row.Cells("Satuan").Value.ToString(),
                    ModuleAngka.ParseInteger(row.Cells("Isi").Value, 1), TxtKeterangan.Text.Trim(), ModuleVariabel.NamaUser)
                If ok Then
                    sukses += 1
                    itemSukses.Add(New TransferItemSnapshot With {
                        .Kode = row.Cells("Kode").Value.ToString(), .NamaBarang = row.Cells("NamaBarang").Value.ToString(),
                        .Qty = ModuleAngka.ParseDecimal(row.Cells("QTY").Value), .Satuan = row.Cells("Satuan").Value.ToString(),
                        .Isi = ModuleAngka.ParseDecimal(row.Cells("Isi").Value), .QtySat = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)})
                End If
            Next
            If sukses = 0 Then
                SetStatus("Status: kirim cloud gagal untuk semua item.")
                MessageBox.Show("Tidak ada item yang berhasil dikirim ke cloud.", "Kirim Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            _currentTransferId = GenerateNomorTransferCabang()
            Dim totalRows = DgvDetail.Rows.Cast(Of DataGridViewRow)().Count(Function(r) Not r.IsNewRow)
            Dim statusTransfer = If(sukses = totalRows, "TERKIRIM", "PARTIAL")
            SimpanArsipTransferLokal(_currentTransferId, "CLOUD", "", itemSukses, statusTransfer)
            SetStatus($"Status: kirim cloud selesai ({sukses}) [{statusTransfer}].")
            MessageBox.Show($"Transfer cloud berhasil: {sukses} item.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If sukses = totalRows Then ResetFormSetelahSimpan() Else HapusItemYangSudahSukses(itemSukses)
        Catch ex As Exception
            MessageBox.Show("Gagal kirim ke cloud: " & ex.Message & vbCrLf & "Data disimpan offline.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            KirimTransferOffline(kodeTujuan)
        End Try
    End Sub

    Private Sub KirimTransferOffline(kodeTujuan As String)
        Try
            Dim items As New List(Of TransferItemSnapshot)()
            For Each row As DataGridViewRow In DgvDetail.Rows
                If row.IsNewRow Then Continue For
                items.Add(New TransferItemSnapshot With {
                    .Kode = row.Cells("Kode").Value.ToString(), .NamaBarang = row.Cells("NamaBarang").Value.ToString(),
                    .Qty = ModuleAngka.ParseDecimal(row.Cells("QTY").Value), .Satuan = row.Cells("Satuan").Value.ToString(),
                    .Isi = ModuleAngka.ParseDecimal(row.Cells("Isi").Value), .QtySat = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)})
            Next
            _currentTransferId = GenerateNomorTransferCabang()
            SimpanArsipTransferLokal(_currentTransferId, "OFFLINE_QUEUE", "", items, "PENDING")
            For Each item In items
                Using cmd As New MySqlCommand(
                    "INSERT INTO transfer_keluar_offline (id_transfer, dari_cabang, ke_cabang, kode_barang, nama_barang,
                     qty, satuan, isi_satuan, qty_satuan, keterangan, status)
                     VALUES (@id, @dari, @ke, @kode, @nama, @qty, @sat, @isi, @qtySat, @ket, 'PENDING')", conn)
                    cmd.Parameters.AddWithValue("@id", _currentTransferId)
                    cmd.Parameters.AddWithValue("@dari", SyncConfig.KodeCabang)
                    cmd.Parameters.AddWithValue("@ke", kodeTujuan)
                    cmd.Parameters.AddWithValue("@kode", item.Kode)
                    cmd.Parameters.AddWithValue("@nama", item.NamaBarang)
                    cmd.Parameters.AddWithValue("@qty", item.Qty)
                    cmd.Parameters.AddWithValue("@sat", item.Satuan)
                    cmd.Parameters.AddWithValue("@isi", item.Isi)
                    cmd.Parameters.AddWithValue("@qtySat", item.QtySat)
                    cmd.Parameters.AddWithValue("@ket", TxtKeterangan.Text.Trim())
                    cmd.ExecuteNonQuery()
                End Using
            Next
            SetStatus($"Status: transfer disimpan offline ({items.Count} item). Kirim saat online.")
            MessageBox.Show($"Transfer disimpan offline ({items.Count} item)." & vbCrLf &
                            "Akan dikirim otomatis saat online via tombol 'Upload Offline'.", "Tersimpan Offline", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ResetFormSetelahSimpan()
            MuatTransferKeluarOfflinePending()
        Catch ex As Exception
            MessageBox.Show("Gagal simpan offline: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' EnsureTabelTransferKeluarOffline dihapus — tabel dibuat via 01_migrasi_kolom.sql

    Private Sub BtnUploadOffline_Click(sender As Object, e As EventArgs)
        If Not SupabaseHelper.IsInitialized() OrElse Not SupabaseHelper.CekKoneksi() Then
            MessageBox.Show("Tidak ada koneksi cloud.", "Offline", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim dt As New DataTable()
        Using cmd As New MySqlCommand(
            "SELECT id, id_transfer, ke_cabang, kode_barang, nama_barang, qty, satuan, isi_satuan, qty_satuan, keterangan
             FROM transfer_keluar_offline WHERE status='PENDING' ORDER BY id ASC", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using
        If dt.Rows.Count = 0 Then SetStatus("Status: tidak ada transfer offline pending.") : Return
        Dim sukses As Integer = 0 : Dim gagal As Integer = 0
        For Each row As DataRow In dt.Rows
            Try
                Dim ok = SyncManager.KirimTransfer(row("kode_barang").ToString(), row("nama_barang").ToString(),
                    row("ke_cabang").ToString(), ModuleAngka.ParseDecimal(row("qty")), row("satuan").ToString(),
                    ModuleAngka.ParseInteger(row("isi_satuan"), 1), row("keterangan").ToString(), ModuleVariabel.NamaUser)
                If ok Then
                    Using cmd As New MySqlCommand("UPDATE transfer_keluar_offline SET status='TERKIRIM' WHERE id=@id", conn)
                        cmd.Parameters.AddWithValue("@id", ModuleAngka.ParseInteger(row("id"), 0))
                        cmd.ExecuteNonQuery()
                    End Using
                    Using cmd As New MySqlCommand("UPDATE transfer_cabang SET STATUS_TRANSFER='TERKIRIM' WHERE ID_TRANSFER=@id", conn)
                        cmd.Parameters.AddWithValue("@id", row("id_transfer").ToString())
                        cmd.ExecuteNonQuery()
                    End Using
                    sukses += 1
                Else
                    gagal += 1
                End If
            Catch
                gagal += 1
            End Try
        Next
        SetStatus($"Status: upload offline selesai — {sukses} sukses, {gagal} gagal.")
        MuatTransferKeluarOfflinePending()
    End Sub

    Private Sub MuatTransferKeluarOfflinePending()
        Try
            Dim count As Integer = 0
            Using cmd As New MySqlCommand("SELECT COUNT(DISTINCT id_transfer) FROM transfer_keluar_offline WHERE status='PENDING'", conn)
                count = ModuleAngka.ParseInteger(cmd.ExecuteScalar(), 0)
            End Using
            If _btnUploadOffline IsNot Nothing Then
                _btnUploadOffline.Text = If(count > 0, $"Upload Offline ({count})", "Upload Offline")
                _btnUploadOffline.BackColor = If(count > 0, Color.OrangeRed, Color.Goldenrod)
            End If
        Catch
        End Try
    End Sub

    Private Sub BtnExportManual_Click(sender As Object, e As EventArgs) Handles BtnExportManual.Click
        If Not ValidasiSebelumProses() Then Return
        If Not ValidasiStokKirim() Then Return
        Dim idTransfer = GenerateNomorTransferCabang()
        Dim folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AppKasir", "TransferManual")
        Directory.CreateDirectory(folder)
        Dim filePath = Path.Combine(folder, idTransfer & ".csv")
        Dim sb As New StringBuilder()
        sb.AppendLine("id_transfer,dari_cabang,ke_cabang,kode_barang,nama_barang,qty,satuan,isi_satuan,qty_satuan,harga_beli,keterangan")
        For Each row As DataGridViewRow In DgvDetail.Rows
            If row.IsNewRow Then Continue For
            Dim hargaBeliCsv As Decimal = ModuleAngka.ParseDecimal(
                Convert.ToString(row.Cells("HargaBeli").Value))
            If hargaBeliCsv <= 0D Then hargaBeliCsv = AmbilHargaBeliBarang(Convert.ToString(row.Cells("Kode").Value))
            sb.AppendLine(String.Join(",", New String() {
                EscapeCsv(idTransfer), EscapeCsv(SyncConfig.KodeCabang), EscapeCsv(AmbilKodeCabangTujuan()),
                EscapeCsv(row.Cells("Kode").Value.ToString()), EscapeCsv(row.Cells("NamaBarang").Value.ToString()),
                EscapeCsv(row.Cells("QTY").Value.ToString()), EscapeCsv(row.Cells("Satuan").Value.ToString()),
                EscapeCsv(row.Cells("Isi").Value.ToString()), EscapeCsv(row.Cells("QtySat").Value.ToString()),
                EscapeCsv(hargaBeliCsv.ToString("0.##")),
                EscapeCsv(TxtKeterangan.Text.Trim())}))
        Next
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        _currentTransferId = idTransfer
        SimpanArsipTransferLokal(idTransfer, "OFFLINE_EXPORT", filePath)
        SetStatus("Status: export manual selesai -> " & filePath)
        MessageBox.Show("File export manual berhasil dibuat:" & Environment.NewLine & filePath, "Export Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ResetFormSetelahSimpan()
    End Sub

    Private Function GenerateNomorTransferCabang() As String
        Dim prefix As String = "TC-" & DateTime.Now.ToString("yyMMdd")
        Dim urut As Integer = 1
        Using cmd As New MySqlCommand("SELECT COALESCE(MAX(ID_TRANSFER), '') FROM transfer_cabang WHERE ID_TRANSFER LIKE @prefix", conn)
            cmd.Parameters.AddWithValue("@prefix", prefix & "%")
            Dim val = Convert.ToString(cmd.ExecuteScalar())
            If Not String.IsNullOrEmpty(val) AndAlso val.Length >= 12 Then
                Integer.TryParse(val.Substring(val.Length - 4), urut)
                urut += 1
            End If
        End Using
        Return prefix & urut.ToString("0000")
    End Function

    Private Sub ResetFormSetelahSimpan()
        CetakNotaOtomatis()
        DgvDetail.Rows.Clear()
        BersihkanInputBarang()
        _currentTransferId = ""
    End Sub

    Private Sub HapusItemYangSudahSukses(itemsSukses As List(Of TransferItemSnapshot))
        If itemsSukses Is Nothing OrElse itemsSukses.Count = 0 Then Return
        Dim keySet As New HashSet(Of String)(itemsSukses.Select(Function(i) $"{i.Kode}|{i.QtySat}|{i.Satuan}"), StringComparer.OrdinalIgnoreCase)
        For i As Integer = DgvDetail.Rows.Count - 1 To 0 Step -1
            Dim r = DgvDetail.Rows(i)
            Dim key = $"{Convert.ToString(r.Cells("Kode").Value)}|{ModuleAngka.ParseDecimal(Convert.ToString(r.Cells("QtySat").Value))}|{Convert.ToString(r.Cells("Satuan").Value)}"
            If keySet.Contains(key) Then DgvDetail.Rows.RemoveAt(i)
        Next
        TxtNamaBarang.Focus()
    End Sub
#End Region


#Region "Simpan Arsip & Jurnal"
    Private Sub SimpanArsipTransferLokal(idTransfer As String, modeKirim As String, referensi As String,
                                         Optional items As List(Of TransferItemSnapshot) = Nothing,
                                         Optional statusTransferOverride As String = Nothing)
        If items Is Nothing Then
            items = DgvDetail.Rows.Cast(Of DataGridViewRow)().Where(Function(r) Not r.IsNewRow).
                Select(Function(r) New TransferItemSnapshot With {
                    .Kode = Convert.ToString(r.Cells("Kode").Value),
                    .NamaBarang = Convert.ToString(r.Cells("NamaBarang").Value),
                    .Qty = ModuleAngka.ParseDecimal(r.Cells("QTY").Value),
                    .Satuan = Convert.ToString(r.Cells("Satuan").Value),
                    .Isi = ModuleAngka.ParseDecimal(r.Cells("Isi").Value),
                    .QtySat = ModuleAngka.ParseDecimal(r.Cells("QtySat").Value),
                    .HargaBeli = 0D}).ToList()
        End If
        If items.Count = 0 Then Throw New Exception("Tidak ada item valid untuk disimpan.")

        Dim totalQty As Decimal = 0D : Dim totalRupiah As Decimal = 0D
        For Each it In items
            If it.HargaBeli <= 0D Then it.HargaBeli = AmbilHargaBeliBarang(it.Kode)
            totalQty += it.QtySat
            totalRupiah += (it.QtySat * it.HargaBeli)
        Next

        Using trx = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand(
                    "INSERT INTO transfer_cabang (ID_TRANSFER, TGL_TRANSFER, LOKASI, DARI_CABANG, KE_CABANG, MODE_KIRIM,
                     STATUS_TRANSFER, ID_CLOUD_TRANSFER, FILE_MANUAL, TOTAL_QTY, TOTAL_BARANG, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
                     VALUES (@id, @tgl, @lokasi, @dariCabang, @keCabang, @modeKirim, @statusTransfer, @idCloudTransfer,
                     @fileManual, @qty, @totBarang, @totRupiah, @idUser, @idKomputer)", conn, trx)
                    cmd.Parameters.AddWithValue("@id", idTransfer)
                    cmd.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@lokasi", $"ANTAR_CABANG:{AmbilKodeCabangTujuan()} [{modeKirim}]")
                    cmd.Parameters.AddWithValue("@dariCabang", SyncConfig.KodeCabang)
                    cmd.Parameters.AddWithValue("@keCabang", AmbilKodeCabangTujuan())
                    cmd.Parameters.AddWithValue("@modeKirim", modeKirim)
                    cmd.Parameters.AddWithValue("@statusTransfer", If(String.IsNullOrWhiteSpace(statusTransferOverride), If(modeKirim = "CLOUD", "TERKIRIM", "PENDING"), statusTransferOverride))
                    cmd.Parameters.AddWithValue("@idCloudTransfer", DBNull.Value)
                    cmd.Parameters.AddWithValue("@fileManual", If(String.IsNullOrEmpty(referensi), CType(DBNull.Value, Object), referensi))
                    cmd.Parameters.AddWithValue("@qty", totalQty)
                    cmd.Parameters.AddWithValue("@totBarang", items.Count)
                    cmd.Parameters.AddWithValue("@totRupiah", totalRupiah)
                    cmd.Parameters.AddWithValue("@idUser", ModuleVariabel.NamaUser)
                    cmd.Parameters.AddWithValue("@idKomputer", FormUtama.StatusNamaPC.Text)
                    cmd.ExecuteNonQuery()
                End Using

                For Each row In items
                    Using cmdD As New MySqlCommand(
                        "INSERT INTO transfer_cabang_detail (ID_TRANSFER, TGL_TRANSFER, LOKASI, ID_BARANG, NAMA_BARANG, HARGA,
                         QTY, SATUAN, ISI_SATUAN, HARGA_QTY, TOTAL_QTY, DITERIMA_QTY, STATUS_ITEM, TOTAL, ID_USER, ID_KOMPUTER)
                         VALUES (@id, @tgl, @lokasi, @idBarang, @namaBarang, @harga, @qty, @satuan, @isi, @hargaQty,
                         @qtySat, 0, 'PENDING', @total, @idUser, @idKomputer)", conn, trx)
                        cmdD.Parameters.AddWithValue("@id", idTransfer)
                        cmdD.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdD.Parameters.AddWithValue("@lokasi", $"ANTAR_CABANG:{AmbilKodeCabangTujuan()} [{modeKirim}] {referensi}")
                        cmdD.Parameters.AddWithValue("@idBarang", row.Kode)
                        cmdD.Parameters.AddWithValue("@namaBarang", row.NamaBarang)
                        cmdD.Parameters.AddWithValue("@harga", row.HargaBeli)
                        cmdD.Parameters.AddWithValue("@qty", row.Qty)
                        cmdD.Parameters.AddWithValue("@satuan", row.Satuan)
                        cmdD.Parameters.AddWithValue("@isi", row.Isi)
                        cmdD.Parameters.AddWithValue("@hargaQty", row.HargaBeli * Math.Max(1D, row.Isi))
                        cmdD.Parameters.AddWithValue("@qtySat", row.QtySat)
                        cmdD.Parameters.AddWithValue("@total", row.HargaBeli * row.QtySat)
                        cmdD.Parameters.AddWithValue("@idUser", ModuleVariabel.NamaUser)
                        cmdD.Parameters.AddWithValue("@idKomputer", FormUtama.StatusNamaPC.Text)
                        cmdD.ExecuteNonQuery()
                    End Using
                    Using cmdStok As New MySqlCommand(
                        $"UPDATE tbl_barang SET {KolomTransferKeluar}={KolomTransferKeluar}+@qtySat,
                         is_dirty=1, version=version+1, updated_by=@user
                         WHERE ID_BARANG=@kode AND {KolomStokKeluar}>=@qtySat", conn, trx)
                        cmdStok.Parameters.AddWithValue("@qtySat", row.QtySat)
                        cmdStok.Parameters.AddWithValue("@kode", row.Kode)
                        cmdStok.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                        If cmdStok.ExecuteNonQuery() = 0 Then
                            Throw New Exception($"Stok {LokasiBarang.ToLower()} tidak cukup saat simpan transfer untuk barang {row.Kode}.")
                        End If
                    End Using
                    Using cmdHist As New MySqlCommand(
                        "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER)
                         VALUES (@faktur, @tanggal, @jenis, @lokasi, @idBarang, @namaBarang, @qty, @satuan, @isi, @totalQty, @totalRupiah, @idUser, @idKomputer)", conn, trx)
                        cmdHist.Parameters.AddWithValue("@faktur", idTransfer)
                        cmdHist.Parameters.AddWithValue("@tanggal", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdHist.Parameters.AddWithValue("@jenis", "TRANSFER_CABANG_KELUAR")
                        cmdHist.Parameters.AddWithValue("@lokasi", LokasiBarang)
                        cmdHist.Parameters.AddWithValue("@idBarang", row.Kode)
                        cmdHist.Parameters.AddWithValue("@namaBarang", row.NamaBarang)
                        cmdHist.Parameters.AddWithValue("@qty", row.Qty)
                        cmdHist.Parameters.AddWithValue("@satuan", row.Satuan)
                        cmdHist.Parameters.AddWithValue("@isi", row.Isi)
                        cmdHist.Parameters.AddWithValue("@totalQty", row.QtySat)
                        cmdHist.Parameters.AddWithValue("@totalRupiah", row.HargaBeli * row.QtySat)
                        cmdHist.Parameters.AddWithValue("@idUser", ModuleVariabel.NamaUser)
                        cmdHist.Parameters.AddWithValue("@idKomputer", FormUtama.StatusNamaPC.Text)
                        cmdHist.ExecuteNonQuery()
                    End Using
                Next

                SimpanJurnalTransferBarang(trx, idTransfer, totalRupiah, modeKirim)
                For Each row In items
                    HitungStokPerubahan(row.Kode, trx)
                Next


                Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using cmdAkun As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, trx)
                    cmdAkun.Parameters.AddWithValue("@fk", idTransfer)
                    Using rd = cmdAkun.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibat.Add(kode)
                        End While
                    End Using
                End Using
                For Each kodeAkun As String In akunTerlibat
                    UpdateSaldoAkun(kodeAkun, trx)
                Next
                trx.Commit()
            Catch
                trx.Rollback()
                Throw
            End Try
        End Using
    End Sub

    Private Sub SimpanJurnalTransferBarang(transaction As MySqlTransaction, noTransaksi As String, nominal As Decimal, modeKirim As String)
        If nominal <= 0D Then Return
        Dim uraian As String = $"Transfer cabang antar cabang ({modeKirim}) dari {SyncConfig.KodeCabang} ke {AmbilKodeCabangTujuan()}"
        Using cmd As New MySqlCommand(
            "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K,
             NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER)
             VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K,
             @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            ' Kirim barang ke cabang lain: D REKENING KORAN PUSAT (04.01.003), K PERSEDIAAN BARANG
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "REKENING KORAN PUSAT")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "04.01.003")
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "TRANSFER CABANG")
            cmd.Parameters.AddWithValue("@LOKASI", SyncConfig.KodeCabang)
            cmd.Parameters.AddWithValue("@ID_USER", ModuleVariabel.NamaUser)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
#End Region


#Region "Cetak & CSV"
    Private Sub CetakNotaOtomatis()
        If String.IsNullOrEmpty(_currentTransferId) Then Return
        Try
            ModulePrinterTransferCabang.CetakTransferCabang(_currentTransferId)
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "cetak", _currentTransferId, "", "CetakNotaOtomatis: " & ex.Message)
        End Try
    End Sub

    Private Sub CetakUlangNota(idTransfer As String)
        If String.IsNullOrEmpty(idTransfer) Then
            MessageBox.Show("Tidak ada transfer aktif untuk dicetak." & vbCrLf &
                            "Selesaikan transaksi terlebih dahulu atau pilih dari riwayat.",
                            "Cetak", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Try
            ModulePrinterTransferCabang.TanyaPilihPrinterTransferCabang(idTransfer)
        Catch ex As Exception
            MessageBox.Show("Gagal cetak: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function EscapeCsv(value As String) As String
        Return $"""{If(value, "").Replace("""", """""")}"""
    End Function

    Private Function ParseCsvLine(line As String) As List(Of String)
        Dim result As New List(Of String)()
        Dim sb As New StringBuilder()
        Dim inQuotes As Boolean = False
        For Each ch As Char In line
            If ch = """"c Then
                inQuotes = Not inQuotes
            ElseIf ch = ","c AndAlso Not inQuotes Then
                result.Add(sb.ToString())
                sb.Clear()
            Else
                sb.Append(ch)
            End If
        Next
        result.Add(sb.ToString())
        Return result
    End Function

    Private Sub PrintDoc_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _printDoc.PrintPage
        Using f As New Font("Consolas", 10.0F)
            e.Graphics.DrawString(_notaText, f, Brushes.Black, New RectangleF(20, 20, e.MarginBounds.Width, e.MarginBounds.Height))
        End Using
    End Sub
#End Region

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F4      : Fokus ke pencarian barang" & vbCrLf &
                           "F8      : Tambah / kirim transfer cabang" & vbCrLf &
                           "F9      : Cetak ulang nota" & vbCrLf &
                           "F10     : Terima transfer (mode TERIMA)" & vbCrLf &
                           "F11     : Hapus semua barang di grid" & vbCrLf &
                           "ESC     : Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
