''' <summary>
''' FormTambahEditRakitan — Dialog Tambah/Edit Paket Rakitan
''' Top: Kode(auto) | Nama | Barcode(auto) | Harga Beli(auto dari BOM) | Harga Jual | Satuan
''' Bottom: BOM grid — Kolom: X(hapus) | Id(hidden) | Nama(search) | Qty | Satuan(ComboBox) | Isi
''' Logic DGV: Persis seperti FormJual — satu jalur inline edit + barcode + ListBox.
''' </summary>
Public Class FormTambahEditRakitan

    Private _kodeRakitan As String = ""
    Private _namaRakitan As String = ""
    Private _modeTambah As Boolean = True

    Private ReadOnly COL_ID As String = NameOf(ColId)
    Private ReadOnly COL_NAMA As String = NameOf(ColNama)
    Private ReadOnly COL_QTY As String = NameOf(ColQty)
    Private ReadOnly COL_SATUAN As String = NameOf(ColSatuan)
    Private ReadOnly COL_ISI As String = NameOf(ColIsi)
    Private ReadOnly COL_HARGABELI As String = NameOf(ColHargaBeli)
    Private ReadOnly COL_TOTALHARGABELI As String = NameOf(ColTotalHargaBeli)
    Private ReadOnly COL_STOKTOKO As String = NameOf(ColStokToko)
    Private ReadOnly COL_STOKGUDANG As String = NameOf(ColStokGudang)
    Private ReadOnly COL_STOK As String = NameOf(ColStok)

    ' ═══════════════════════════════════════════════════════════════
    '  STATE — Persis seperti FormJual
    ' ═══════════════════════════════════════════════════════════════
    Private WithEvents _searchTimer As New Timer() With {.Interval = 100}
    Private _searchKeywordPending As String = ""
    Private WithEvents barcodeTimer As New System.Windows.Forms.Timer()
    Private barcodeChars As New List(Of Char)
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private isBarcodeMode As Boolean = False
    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100
    Private _dgvEditingTextBox As TextBox = Nothing
    Private _sedangPindahKeLstBarang As Boolean = False
    Private _sedangSetNilaiDariListBox As Boolean = False
    Private _teksSebelumPindahKeLstBarang As String = ""
    Private _listBoxDibukaDiRow As Integer = -1
    Private _listBoxDibukaDiCol As Integer = -1

    ' ═══════════════════════════════════════════════════════════════
    '  MODE
    ' ═══════════════════════════════════════════════════════════════
    Public Sub SetModeTambah()
        _modeTambah = True : _kodeRakitan = "" : _namaRakitan = ""
    End Sub
    Public Sub SetModeEdit(kode As String, nama As String)
        _modeTambah = False : _kodeRakitan = kode : _namaRakitan = nama
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  LOAD
    ' ═══════════════════════════════════════════════════════════════
    Private Sub FormTambahEditRakitan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        barcodeTimer.Interval = 100
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick
        SetupDgv()
        TampilSatuan()

        If _modeTambah Then
            LblHeader.Text = "TAMBAH PAKET RAKITAN BARU"
            TxtKode.Text = GenerateKodePaket()
            GenerateBarcode()
            TxtNama.Clear() : TxtNama.Focus()
            TxtHargaBeli.Text = "0"
            TxtHargaJual.Text = "0"
            HitungHpp()
        Else
            LblHeader.Text = "EDIT BOM PAKET RAKITAN"
            TxtKode.Text = _kodeRakitan
            LoadDataPaket()
            HitungHpp()
        End If
        LstBarang.BringToFront()
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  SETUP DGV — Kolom identik struktur dengan FormJual (disesuaikan)
    ' ═══════════════════════════════════════════════════════════════
    Private Sub SetupDgv()
        DgvKomponen.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2

        ModuleTheme.ApplyThemeDataGridView(DgvKomponen)
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  TAMPIL SATUAN — dari tbl_satuan
    ' ═══════════════════════════════════════════════════════════════
    Public Sub TampilSatuan()
        CmbSatuan.Items.Clear()
        Try
            Using cmd As New MySqlCommand("SELECT nama FROM tbl_satuan ORDER BY nama ASC", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Do While rd.Read()
                        Dim namaSatuan As String = rd.Item("nama").ToString()
                        If Not String.IsNullOrEmpty(namaSatuan) Then CmbSatuan.Items.Add(namaSatuan)
                    Loop
                End Using
            End Using
        Catch : End Try
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  SETUP FOKUS — Persis seperti FormJual.SetupFocusToGrid
    ' ═══════════════════════════════════════════════════════════════
    Public Sub SetupFocusToGrid()
        If DgvKomponen.Rows.Count = 0 Then Return

        ' Cari baris kosong SETELAH baris terakhir yang terisi
        Dim targetRow As Integer = 0
        Dim lastFilledRow As Integer = -1

        For i As Integer = DgvKomponen.Rows.Count - 1 To 0 Step -1
            If Not DgvKomponen.Rows(i).IsNewRow Then
                Dim kodeVal = Convert.ToString(DgvKomponen.Rows(i).Cells(COL_ID).Value).Trim()
                If Not String.IsNullOrEmpty(kodeVal) Then
                    lastFilledRow = i
                    Exit For
                End If
            End If
        Next

        If lastFilledRow >= 0 Then
            Dim foundEmptyRow As Boolean = False
            For i As Integer = lastFilledRow + 1 To DgvKomponen.Rows.Count - 1
                If Not DgvKomponen.Rows(i).IsNewRow Then
                    Dim kodeVal = Convert.ToString(DgvKomponen.Rows(i).Cells(COL_ID).Value).Trim()
                    If String.IsNullOrEmpty(kodeVal) Then
                        targetRow = i
                        foundEmptyRow = True
                        Exit For
                    End If
                End If
            Next

            If Not foundEmptyRow Then
                Dim isNewRowIdx As Integer = -1
                For i As Integer = lastFilledRow + 1 To DgvKomponen.Rows.Count - 1
                    If DgvKomponen.Rows(i).IsNewRow Then
                        isNewRowIdx = i
                        Exit For
                    End If
                Next
                If isNewRowIdx >= 0 Then
                    targetRow = isNewRowIdx
                Else
                    If DgvKomponen.CurrentCell IsNot Nothing Then
                        targetRow = DgvKomponen.CurrentCell.RowIndex
                    Else
                        Exit Sub
                    End If
                End If
            End If
        Else
            targetRow = 0
        End If

        If targetRow < DgvKomponen.Rows.Count Then
            Dim targetColumnIndex As Integer = DgvKomponen.Columns(COL_NAMA).Index
            Dim targetRowIndex As Integer = targetRow

            DgvKomponen.CurrentCell = DgvKomponen(targetColumnIndex, targetRowIndex)
            Me.ActiveControl = DgvKomponen

            DgvKomponen.BeginInvoke(New Action(Sub()
                                                   If DgvKomponen.CurrentCell IsNot Nothing AndAlso
                   DgvKomponen.CurrentCell.ColumnIndex = targetColumnIndex AndAlso
                   DgvKomponen.CurrentCell.RowIndex = targetRowIndex Then
                                                       DgvKomponen.BeginEdit(True)
                                                       DgvKomponen.EditingControl?.Focus()
                                                   End If
                                               End Sub))
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  LOAD DATA PAKET (mode Edit)
    ' ═══════════════════════════════════════════════════════════════
    Private Sub LoadDataPaket()
        Using cmd As New MySqlCommand(
            "SELECT NAMA_BARANG, BARCODE_KECIL, HARGA_BELI, HARGA_JUAL_UMUM_KECIL, " &
            "SATUAN_UMUM_KECIL, ISI_UMUM_KECIL FROM tbl_barang WHERE ID_BARANG=@kode", conn)
            cmd.Parameters.AddWithValue("@kode", _kodeRakitan)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtNama.Text = rd("NAMA_BARANG").ToString()
                    TxtBarcode.Text = rd("BARCODE_KECIL").ToString()
                    TxtHargaJual.Text = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL")).ToString("N0")
                    CmbSatuan.Text = rd("SATUAN_UMUM_KECIL").ToString()
                End If
            End Using
        End Using
        ' Load BOM
        DgvKomponen.Rows.Clear()
        Using cmd As New MySqlCommand(
            "SELECT r.kode_komponen, r.nama_komponen, r.qty, r.satuan, " &
            "b.HARGA_BELI, b.STOK_TOKO, b.STOK_GUDANG, " &
            "b.SATUAN_UMUM_KECIL, b.SATUAN_UMUM_SEDANG, b.SATUAN_UMUM_BESAR, " &
            "b.ISI_UMUM_KECIL, b.ISI_UMUM_SEDANG, b.ISI_UMUM_BESAR " &
            "FROM tbl_rakitan_bom r LEFT JOIN tbl_barang b ON b.ID_BARANG=r.kode_komponen " &
            "WHERE r.kode_rakitan=@kode ORDER BY r.urutan", conn)
            cmd.Parameters.AddWithValue("@kode", _kodeRakitan)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim rowIdx As Integer = DgvKomponen.Rows.Add()
                    Dim row = DgvKomponen.Rows(rowIdx)
                    ' Isi per kolom by name — aman, tidak bergantung urutan index
                    row.Cells(COL_ID).Value = rd("kode_komponen").ToString()
                    row.Cells(COL_NAMA).Value = rd("nama_komponen").ToString()
                    Dim qtyVal As Decimal = ModuleAngka.ParseDecimal(rd("qty"))
                    Dim hargaBeliVal As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    Dim stokTokoVal As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    Dim stokGudangVal As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    row.Cells(COL_QTY).Value = qtyVal
                    row.Cells(COL_HARGABELI).Value = hargaBeliVal
                    row.Cells(COL_TOTALHARGABELI).Value = hargaBeliVal * qtyVal
                    row.Cells(COL_STOKTOKO).Value = stokTokoVal
                    row.Cells(COL_STOKGUDANG).Value = stokGudangVal
                    row.Cells(COL_STOK).Value = stokTokoVal
                    Dim cc = CType(row.Cells(COL_SATUAN), DataGridViewComboBoxCell)
                    cc.Items.Clear()
                    Dim sK = rd("SATUAN_UMUM_KECIL").ToString()
                    Dim sS = rd("SATUAN_UMUM_SEDANG").ToString()
                    Dim sB = rd("SATUAN_UMUM_BESAR").ToString()
                    If Not String.IsNullOrEmpty(sK) Then cc.Items.Add(sK)
                    If Not String.IsNullOrEmpty(sS) Then cc.Items.Add(sS)
                    If Not String.IsNullOrEmpty(sB) Then cc.Items.Add(sB)
                    cc.Value = rd("satuan").ToString()
                    Dim isi As Integer = 1
                    If rd("satuan").ToString() = sS Then
                        isi = ModuleAngka.ParseInteger(rd("ISI_UMUM_SEDANG"), 1)
                    ElseIf rd("satuan").ToString() = sB Then
                        isi = ModuleAngka.ParseInteger(rd("ISI_UMUM_BESAR"), 1)
                    Else
                        isi = ModuleAngka.ParseInteger(rd("ISI_UMUM_KECIL"), 1)
                    End If
                    row.Cells(COL_ISI).Value = isi
                    row.Cells(COL_NAMA).ReadOnly = True
                    row.Cells(COL_NAMA).Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                End While
            End Using
        End Using
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  GENERATE KODE — PAKET-000001
    ' ═══════════════════════════════════════════════════════════════
    Private Function GenerateKodePaket() As String
        Dim mk As String = "PAKET-000001"
        Try
            Using cmd As New MySqlCommand("SELECT MAX(ID_BARANG) FROM tbl_barang WHERE ID_BARANG LIKE 'PAKET-%'", conn)
                Dim r = cmd.ExecuteScalar()
                If r IsNot Nothing AndAlso Not IsDBNull(r) Then
                    Dim num As Long = 0
                    If Long.TryParse(Convert.ToString(r).Substring(5), num) Then mk = "PAKET-" & (num + 1).ToString("000000")
                End If
            End Using
        Catch : End Try
        Return mk
    End Function

    ' ═══════════════════════════════════════════════════════════════
    '  GENERATE BARCODE — EAN-13
    ' ═══════════════════════════════════════════════════════════════
    Private Sub GenerateBarcode()
        Try
            Dim negara As String = "91"
            Randomize()
            Dim produk As String = (Int(8888 * Rnd()) + 1111).ToString()
            Dim angkaMax As Integer = 899999
            Dim angkaMin As Integer = 100000
            Dim angkaRandom As Integer = Int(angkaMax * Rnd()) + angkaMin
            Dim barcode As String = negara & produk & angkaRandom.ToString()
            Dim X As Integer = 0, Y As Integer = 0, j As Integer = 11
            For i As Integer = 1 To 12
                If i Mod 2 = 0 Then
                    X += Integer.Parse(barcode(j).ToString())
                Else
                    Y += Integer.Parse(barcode(j).ToString())
                End If
                j -= 1
            Next
            Dim Z As Integer = X + (3 * Y)
            Dim checkDigit As Integer = (10 - (Z Mod 10)) Mod 10
            TxtBarcode.Text = barcode & checkDigit.ToString()
        Catch
            TxtBarcode.Text = ""
        End Try
    End Sub

    Private Sub BtnGenBarcode_Click(sender As Object, e As EventArgs) Handles BtnGenBarcode.Click
        GenerateBarcode()
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  HITUNG HPP OTOMATIS dari total harga beli × qty komponen
    ' ═══════════════════════════════════════════════════════════════
    Private Sub HitungHpp()
        Dim totalHpp As Decimal = 0
        For Each row As DataGridViewRow In DgvKomponen.Rows
            If row.IsNewRow Then Continue For
            Dim kd As String = If(row.Cells(COL_ID).Value IsNot Nothing, row.Cells(COL_ID).Value.ToString().Trim(), "")
            If String.IsNullOrEmpty(kd) Then Continue For
            ' Ambil total harga beli dari kolom DGV (lebih efisien, tanpa query DB)
            Dim totalBaris As Decimal = ModuleAngka.ParseDecimal(row.Cells(COL_TOTALHARGABELI).Value)
            totalHpp += totalBaris
        Next
        TxtHargaBeli.Text = totalHpp.ToString("N0")
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  EDITING CONTROL SHOWING — Persis seperti FormJual
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvKomponen_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DgvKomponen.EditingControlShowing
        If DgvKomponen.CurrentCell IsNot Nothing AndAlso DgvKomponen.CurrentCell.OwningColumn.Name = COL_NAMA Then
            If _sedangPindahKeLstBarang Then Return
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.None
                If _dgvEditingTextBox IsNot Nothing Then
                    RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                End If
                ResetBarcodeDetection()
                _dgvEditingTextBox = autoText
                AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                PosisikanLstBarangDiBawahSel()
            End If
        Else
            LstBarang.Visible = False
            LstBarang.Items.Clear()
        End If
        ' ComboBox Satuan — update isi saat edit
        If DgvKomponen.CurrentCell IsNot Nothing AndAlso DgvKomponen.CurrentCell.OwningColumn.Name = COL_SATUAN Then
            Dim combo As ComboBox = TryCast(e.Control, ComboBox)
            If combo IsNot Nothing Then
                RemoveHandler combo.SelectedIndexChanged, AddressOf ComboBox_Satuan_SelectedIndexChanged
                AddHandler combo.SelectedIndexChanged, AddressOf ComboBox_Satuan_SelectedIndexChanged
            End If
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  DGV NAMA TEXTCHANGED — Delta barcode + debounce search
    '  Persis seperti FormJual.DgvNamaBarang_TextChanged
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        If _sedangSetNilaiDariListBox Then Return
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return
        Dim currentText = txt.Text.Trim()
        If String.IsNullOrEmpty(currentText) Then
            If _sedangPindahKeLstBarang OrElse LstBarang.Focused OrElse LstBarang.Visible Then Return
            LstBarang.Items.Clear() : LstBarang.Visible = False : ResetBarcodeDetection() : Return
        End If

        ' Feed karakter ke barcodeChars — delta-only seperti FormJual
        Dim currentTime = DateTime.Now
        If barcodeChars.Count = 0 Then
            barcodeStartTime = currentTime
        Else
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then isBarcodeMode = False
        End If
        ' Delta: hanya tambah karakter BARU
        Dim charsSchon As Integer = barcodeChars.Count
        If currentText.Length > charsSchon Then
            For i As Integer = charsSchon To currentText.Length - 1
                If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(currentText(i))
            Next
        ElseIf currentText.Length < charsSchon Then
            barcodeChars.Clear()
            barcodeStartTime = currentTime
            For Each ch As Char In currentText
                If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(ch)
            Next
        End If
        lastKeyTime = currentTime : barcodeTimer.Stop() : barcodeTimer.Start()

        ' Manual search: hanya untuk input yang ada huruf
        Dim keyword As String = currentText
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            keyword = parts(parts.Length - 1).Trim()
        End If
        If keyword.Any(AddressOf Char.IsLetter) Then
            _searchKeywordPending = keyword : _searchTimer.Stop() : _searchTimer.Start()
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  DGV NAMA KEYDOWN — Handle Down/Enter/Escape untuk ListBox
    '  Persis seperti FormJual.DgvNamaBarang_KeyDown
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvNamaBarang_KeyDown(sender As Object, e As KeyEventArgs)
        If Not LstBarang.Visible OrElse LstBarang.Items.Count = 0 Then Return

        Select Case e.KeyCode
            Case Keys.Down
                If _dgvEditingTextBox IsNot Nothing Then
                    _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                End If
                _sedangPindahKeLstBarang = True
                If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                Me.BeginInvoke(New Action(Sub()
                                              Me.BeginInvoke(New Action(Sub()
                                                                            If LstBarang.Visible Then
                                                                                _sedangSetNilaiDariListBox = True
                                                                                DgvKomponen.EndEdit()
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

    ' ═══════════════════════════════════════════════════════════════
    '  SEARCH TIMER TICK — Debounced search
    ' ═══════════════════════════════════════════════════════════════
    Private Sub SearchTimer_Tick(sender As Object, e As EventArgs) Handles _searchTimer.Tick
        _searchTimer.Stop()
        If Not String.IsNullOrEmpty(_searchKeywordPending) Then SearchBarangToListBox(_searchKeywordPending)
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  BARCODE TIMER TICK — Proses barcode setelah timeout
    '  Persis seperti FormJual.BarcodeTimer_Tick (DGV path)
    ' ═══════════════════════════════════════════════════════════════
    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsedSinceLastKey = (DateTime.Now - lastKeyTime).TotalMilliseconds
        If elapsedSinceLastKey > 100 Then
            barcodeTimer.Stop()
            Dim bufferText = New String(barcodeChars.ToArray())

            If bufferText.Length >= BARCODE_MIN_LENGTH Then
                ' Jika buffer mengandung '*' atau huruf → manual search
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    ResetBarcodeDetection()
                    Return
                End If

                ' Murni numerik/alphanumeric → kandidat barcode
                Dim barisDiisi As Integer = If(DgvKomponen.CurrentCell IsNot Nothing, DgvKomponen.CurrentCell.RowIndex, -1)
                If barisDiisi >= 0 Then
                    ' Cari baris kosong non-IsNewRow
                    For i As Integer = 0 To DgvKomponen.Rows.Count - 1
                        If Not DgvKomponen.Rows(i).IsNewRow Then
                            Dim kodeVal = Convert.ToString(DgvKomponen.Rows(i).Cells(COL_ID).Value).Trim()
                            Dim namaVal = Convert.ToString(DgvKomponen.Rows(i).Cells(COL_NAMA).Value).Trim()
                            If String.IsNullOrEmpty(kodeVal) AndAlso String.IsNullOrEmpty(namaVal) Then
                                barisDiisi = i
                                Exit For
                            End If
                        End If
                    Next

                    _sedangSetNilaiDariListBox = True
                    DgvKomponen.EndEdit(True)
                    DgvKomponen.CurrentCell = Nothing

                    ' Cari nama barang dari barcode
                    Dim namaBarang As String = ""
                    Try
                        Using cmd As New MySqlCommand(
                            "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND " &
                            "(BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc OR ID_BARANG=@bc) LIMIT 1", conn)
                            cmd.Parameters.AddWithValue("@bc", bufferText)
                            Dim result = cmd.ExecuteScalar()
                            If result IsNot Nothing Then namaBarang = result.ToString()
                        End Using
                    Catch : End Try

                    If Not String.IsNullOrEmpty(namaBarang) Then
                        IsiBarangKeRow(barisDiisi, namaBarang, 1, barcodeInput:=bufferText)
                        ' Fokus ke IsNewRow berikutnya
                        Dim nextRow As Integer = -1
                        For i As Integer = 0 To DgvKomponen.Rows.Count - 1
                            If DgvKomponen.Rows(i).IsNewRow Then nextRow = i : Exit For
                        Next
                        If nextRow >= 0 Then
                            DgvKomponen.CurrentCell = DgvKomponen(DgvKomponen.Columns(COL_NAMA).Index, nextRow)
                            Me.ActiveControl = DgvKomponen
                        End If
                    Else
                        MessageBox.Show("Barcode '" & bufferText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                    _sedangSetNilaiDariListBox = False
                End If
                ResetBarcodeDetection()
            End If
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  RESET BARCODE DETECTION
    ' ═══════════════════════════════════════════════════════════════
    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False : barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue : lastKeyTime = DateTime.MinValue : barcodeTimer.Stop()
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  SEARCH BARANG TO LISTBOX — Query + populate ListBox
    ' ═══════════════════════════════════════════════════════════════
    Private Sub SearchBarangToListBox(keyword As String)
        keyword = keyword.Trim()
        If keyword.Length < 2 AndAlso Not keyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear() : LstBarang.Visible = False : Return
        End If

        LstBarang.Items.Clear()
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG " &
                "FROM tbl_barang WHERE STATUS='Aktif' AND " &
                "(ID_BARANG LIKE @s OR NAMA_BARANG LIKE @s OR BARCODE_KECIL LIKE @s OR BARCODE_SEDANG LIKE @s OR BARCODE_BESAR LIKE @s) " &
                "ORDER BY NAMA_BARANG LIMIT 200", conn)
                cmd.Parameters.AddWithValue("@s", "%" & keyword & "%")
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim namaBarang = rd("NAMA_BARANG").ToString()
                        Dim stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                        Dim stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                        LstBarang.Items.Add(String.Format("{0} | T: {1} | G: {2}", namaBarang,
                            stokToko.ToString("N0"), stokGudang.ToString("N0")))
                    End While
                End Using
            End Using
            If LstBarang.Items.Count > 0 Then
                PosisikanLstBarangDiBawahSel() : LstBarang.BringToFront() : LstBarang.Visible = True
                If DgvKomponen.CurrentCell IsNot Nothing Then
                    _listBoxDibukaDiRow = DgvKomponen.CurrentCell.RowIndex
                    _listBoxDibukaDiCol = DgvKomponen.CurrentCell.ColumnIndex
                End If
            Else
                LstBarang.Visible = False
                _listBoxDibukaDiRow = -1 : _listBoxDibukaDiCol = -1
            End If
        Catch : End Try
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  POSISIKAN LISTBOX — Persis seperti FormJual
    ' ═══════════════════════════════════════════════════════════════
    Private Sub PosisikanLstBarangDiBawahSel()
        If DgvKomponen.CurrentCell Is Nothing Then Return
        Try
            Dim cellRect = DgvKomponen.GetCellDisplayRectangle(
                DgvKomponen.CurrentCell.ColumnIndex, DgvKomponen.CurrentCell.RowIndex, True)
            Dim ptDgv = DgvKomponen.PointToScreen(New Point(cellRect.Left, cellRect.Bottom))
            Dim ptPanel = Me.PointToClient(ptDgv)

            LstBarang.Width = Math.Max(400, cellRect.Width)

            Dim spaceBelow As Integer = Me.ClientSize.Height - ptPanel.Y
            If spaceBelow < LstBarang.Height + 40 Then
                Dim targetY As Integer = ptPanel.Y - cellRect.Height - LstBarang.Height
                LstBarang.Location = New Point(ptPanel.X, targetY)
            Else
                LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            End If
        Catch : End Try
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  PROCESS CMD KEY — Navigasi keyboard ke ListBox
    '  Persis seperti FormJual.ProcessCmdKey
    ' ═══════════════════════════════════════════════════════════════
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            Select Case keyData
                Case Keys.Down
                    If LstBarang.Focused Then Return MyBase.ProcessCmdKey(msg, keyData)
                    If _dgvEditingTextBox IsNot Nothing Then
                        _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                    End If
                    _sedangPindahKeLstBarang = True
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    Me.BeginInvoke(New Action(Sub()
                                                  Me.BeginInvoke(New Action(Sub()
                                                                                If LstBarang.Visible Then
                                                                                    _sedangSetNilaiDariListBox = True
                                                                                    DgvKomponen.EndEdit()
                                                                                    _sedangSetNilaiDariListBox = False
                                                                                    LstBarang.Focus()
                                                                                End If
                                                                                _sedangPindahKeLstBarang = False
                                                                            End Sub))
                                              End Sub))
                    Return True
                Case Keys.Enter
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    _sedangPindahKeLstBarang = True : AmbilDataDariListBox() : _sedangPindahKeLstBarang = False
                    Return True
                Case Keys.Escape
                    TutupListBox()
                    If _dgvEditingTextBox IsNot Nothing Then _dgvEditingTextBox.Focus()
                    Return True
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' ═══════════════════════════════════════════════════════════════
    '  LISTBOX KEYDOWN — Up/Enter/Escape
    '  Persis seperti FormJual.LstBarang_KeyDown
    ' ═══════════════════════════════════════════════════════════════
    Private Sub LstBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    _sedangPindahKeLstBarang = True
                    e.SuppressKeyPress = True
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    DgvKomponen.Focus()
                    DgvKomponen.BeginInvoke(New Action(Sub()
                                                           If DgvKomponen.CurrentCell IsNot Nothing Then
                                                               DgvKomponen.BeginEdit(True)
                                                               Dim editCtrl = TryCast(DgvKomponen.EditingControl, TextBox)
                                                               If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                                   editCtrl.Text = teksSimpan
                                                                   editCtrl.SelectionStart = teksSimpan.Length
                                                                   editCtrl.SelectionLength = 0
                                                               End If
                                                               editCtrl?.Focus()
                                                           End If
                                                           _sedangPindahKeLstBarang = False
                                                       End Sub))
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
                Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                _teksSebelumPindahKeLstBarang = ""
                DgvKomponen.Focus()
                DgvKomponen.BeginInvoke(New Action(Sub()
                                                       If DgvKomponen.CurrentCell IsNot Nothing Then
                                                           DgvKomponen.BeginEdit(True)
                                                           Dim editCtrl = TryCast(DgvKomponen.EditingControl, TextBox)
                                                           If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                               editCtrl.Text = teksSimpan
                                                               editCtrl.SelectionStart = teksSimpan.Length
                                                               editCtrl.SelectionLength = 0
                                                           End If
                                                           editCtrl?.Focus()
                                                       End If
                                                       _sedangPindahKeLstBarang = False
                                                   End Sub))
                e.SuppressKeyPress = True
        End Select
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  LISTBOX CLICK
    ' ═══════════════════════════════════════════════════════════════
    Private Sub LstBarang_Click(sender As Object, e As EventArgs) Handles LstBarang.Click
        If LstBarang.SelectedIndex >= 0 Then
            _sedangPindahKeLstBarang = True
            AmbilDataDariListBox()
            _sedangPindahKeLstBarang = False
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  AMBIL DATA DARI LISTBOX — Parse nama, cek duplikat, isi row
    '  Persis seperti FormJual.AmbilDataDariListBox (DGV path)
    ' ═══════════════════════════════════════════════════════════════
    Private Sub AmbilDataDariListBox()
        _teksSebelumPindahKeLstBarang = ""

        Dim selectedValue As String = ""
        If LstBarang.SelectedIndex >= 0 AndAlso LstBarang.SelectedIndex < LstBarang.Items.Count Then
            selectedValue = LstBarang.Items(LstBarang.SelectedIndex).ToString()
        ElseIf LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        End If
        If String.IsNullOrEmpty(selectedValue) Then Return

        ' Extract nama barang dari format "Nama | T: x | G: y"
        Dim namayangdiambil As String = selectedValue
        If selectedValue.Contains("|") Then
            Dim parts = selectedValue.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length > 0 Then namayangdiambil = parts(0).Trim()
        End If

        TutupListBox()

        If DgvKomponen.CurrentCell IsNot Nothing AndAlso DgvKomponen.CurrentCell.OwningColumn.Name = COL_NAMA Then
            Dim barisDiisi As Integer = DgvKomponen.CurrentCell.RowIndex

            ' Cari baris dengan kode kosong pertama
            For i As Integer = 0 To DgvKomponen.Rows.Count - 1
                If Not DgvKomponen.Rows(i).IsNewRow Then
                    Dim kodeVal = Convert.ToString(DgvKomponen.Rows(i).Cells(COL_ID).Value).Trim()
                    If String.IsNullOrEmpty(kodeVal) Then
                        barisDiisi = i
                        Exit For
                    End If
                End If
            Next

            ' EndEdit + flag
            _sedangSetNilaiDariListBox = True
            DgvKomponen.EndEdit(True)
            DgvKomponen.CurrentCell = Nothing

            ' Cek duplikat — gabung qty jika barang sama sudah ada
            Dim idBarangBaru As String = AmbilIdBarang(namayangdiambil)
            If Not String.IsNullOrEmpty(idBarangBaru) Then
                For Each row As DataGridViewRow In DgvKomponen.Rows
                    If Not row.IsNewRow AndAlso row.Index <> barisDiisi Then
                        Dim kodeRow = If(row.Cells(COL_ID).Value IsNot Nothing, row.Cells(COL_ID).Value.ToString().Trim(), "")
                        If kodeRow = idBarangBaru Then
                            ' Gabung qty
                            Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(row.Cells(COL_QTY).Value)
                            Dim qtyBaru As Decimal = 1D
                            Dim qtyTotal As Decimal = qtyLama + qtyBaru
                            row.Cells(COL_QTY).Value = qtyTotal
                            ' Recalculate TotalHargaBeli
                            Dim hargabeliRow As Decimal = ModuleAngka.ParseDecimal(row.Cells(COL_HARGABELI).Value)
                            row.Cells(COL_TOTALHARGABELI).Value = hargabeliRow * qtyTotal
                            ' Hapus baris yang sedang diisi
                            If Not DgvKomponen.Rows(barisDiisi).IsNewRow Then
                                DgvKomponen.Rows.RemoveAt(barisDiisi)
                            End If
                            _sedangSetNilaiDariListBox = False
                            HitungHpp() : SetupFocusToGrid()
                            Return
                        End If
                    End If
                Next
            End If

            IsiBarangKeRow(barisDiisi, namayangdiambil, 1)
            _sedangSetNilaiDariListBox = False
            HitungHpp() : SetupFocusToGrid()
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  AMBIL ID BARANG
    ' ═══════════════════════════════════════════════════════════════
    Private Function AmbilIdBarang(namaBarang As String) As String
        Try
            Using cmd As New MySqlCommand("SELECT ID_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND NAMA_BARANG=@n LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang)
                Dim r = cmd.ExecuteScalar()
                If r IsNot Nothing Then Return r.ToString()
            End Using
        Catch : End Try
        Return ""
    End Function

    ' ═══════════════════════════════════════════════════════════════
    '  ISI BARANG KE ROW — Query DB untuk semua data satuan
    '  Persis seperti FormJual.IsiBarangKeRow (disesuaikan kolom)
    ' ═══════════════════════════════════════════════════════════════
    Private Sub IsiBarangKeRow(rowIdx As Integer, namaBarang As String, qty As Decimal,
                               Optional level As Integer = 1, Optional barcodeInput As String = "")
        If rowIdx < 0 OrElse rowIdx >= DgvKomponen.Rows.Count Then Return
        If String.IsNullOrWhiteSpace(namaBarang) Then Return

        Dim idBarang As String = ""
        Dim hargaBeli As Decimal = 0D
        Dim stokToko As Decimal = 0D : Dim stokGudang As Decimal = 0D
        Dim barcodeKecil As String = "" : Dim barcodeSedang As String = "" : Dim barcodeBesar As String = ""
        Dim satUmumKecil As String = "" : Dim isiUmumKecil As Integer = 1
        Dim satUmumSedang As String = "" : Dim isiUmumSedang As Integer = 1
        Dim satUmumBesar As String = "" : Dim isiUmumBesar As Integer = 1

        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, HARGA_BELI, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                "SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, " &
                "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, " &
                "SATUAN_UMUM_BESAR, ISI_UMUM_BESAR " &
                "FROM tbl_barang WHERE STATUS='Aktif' AND NAMA_BARANG=@n LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang.Trim())
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then Return
                    idBarang = rd("ID_BARANG").ToString()
                    hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    barcodeKecil = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_KECIL", "")
                    barcodeSedang = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_SEDANG", "")
                    barcodeBesar = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_BESAR", "")
                    satUmumKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                    isiUmumKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                    satUmumSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                    isiUmumSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                    satUmumBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                    isiUmumBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                End Using
            End Using
        Catch ex As Exception
            Return
        End Try

        If String.IsNullOrEmpty(idBarang) Then
            MessageBox.Show("Barang '" & namaBarang & "' tidak ditemukan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' ── Tentukan level aktif — barcode lebih prioritas dari level manual ──
        Dim levelAktif As Integer = level
        If Not String.IsNullOrEmpty(barcodeInput) Then
            If barcodeInput = barcodeKecil Then
                levelAktif = 1
            ElseIf barcodeInput = barcodeSedang Then
                levelAktif = 2
            ElseIf barcodeInput = barcodeBesar Then
                levelAktif = 3
            End If
        End If

        ' Pilih satuan, isi sesuai level
        Dim satuanAktif As String = ""
        Dim isiAktif As Integer = 1
        Select Case levelAktif
            Case 3 : satuanAktif = satUmumBesar : isiAktif = isiUmumBesar
            Case 2 : satuanAktif = satUmumSedang : isiAktif = isiUmumSedang
            Case Else : satuanAktif = satUmumKecil : isiAktif = isiUmumKecil
        End Select
        If String.IsNullOrWhiteSpace(satuanAktif) Then
            satuanAktif = satUmumKecil : isiAktif = isiUmumKecil
        End If

        ' ── Isi baris DGV ──
        Dim row = DgvKomponen.Rows(rowIdx)
        row.Cells(COL_ID).Value = idBarang
        row.Cells(COL_NAMA).Value = namaBarang
        row.Cells(COL_QTY).Value = qty
        row.Cells(COL_HARGABELI).Value = hargaBeli
        row.Cells(COL_TOTALHARGABELI).Value = hargaBeli * qty
        row.Cells(COL_STOKTOKO).Value = stokToko
        row.Cells(COL_STOKGUDANG).Value = stokGudang
        row.Cells(COL_STOK).Value = stokToko ' default toko, sesuai lokasi aktif

        ' Setup satuan combo box — isi semua opsi
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(row.Cells(COL_SATUAN), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()
        If Not String.IsNullOrWhiteSpace(satUmumKecil) Then kolomSatuan.Items.Add(satUmumKecil)
        If Not String.IsNullOrWhiteSpace(satUmumSedang) Then kolomSatuan.Items.Add(satUmumSedang)
        If Not String.IsNullOrWhiteSpace(satUmumBesar) Then kolomSatuan.Items.Add(satUmumBesar)
        If kolomSatuan.Items.Count > 0 Then kolomSatuan.Value = satuanAktif
        row.Cells(COL_ISI).Value = isiAktif

        ' Set read-only setelah diisi
        row.Cells(COL_NAMA).ReadOnly = True
        row.Cells(COL_NAMA).Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
        row.Cells(COL_NAMA).Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  COMBOBOX SATUAN CHANGED — Update isi
    ' ═══════════════════════════════════════════════════════════════
    Private Sub ComboBox_Satuan_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim combo = DirectCast(sender, ComboBox)
        Dim cell = TryCast(DgvKomponen.CurrentCell, DataGridViewComboBoxCell)
        If cell Is Nothing Then Return
        Dim idBarang As String = cell.OwningRow.Cells(COL_ID).Value?.ToString()
        If String.IsNullOrEmpty(idBarang) Then Return
        Dim isiValue As Integer = 1
        Try
            Using cmd As New MySqlCommand("SELECT SATUAN_UMUM_KECIL,ISI_UMUM_KECIL,SATUAN_UMUM_SEDANG,ISI_UMUM_SEDANG,SATUAN_UMUM_BESAR,ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG=@id", conn)
                cmd.Parameters.AddWithValue("@id", idBarang)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim satuanDipilih As String = If(combo.SelectedItem IsNot Nothing, combo.SelectedItem.ToString(), "")
                        If satuanDipilih = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "") Then
                            isiValue = Math.Max(1, ModuleAngka.ParseInteger(rd("ISI_UMUM_SEDANG"), 1))
                        ElseIf satuanDipilih = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "") Then
                            isiValue = Math.Max(1, ModuleAngka.ParseInteger(rd("ISI_UMUM_BESAR"), 1))
                        Else
                            isiValue = Math.Max(1, ModuleAngka.ParseInteger(rd("ISI_UMUM_KECIL"), 1))
                        End If
                    End If
                End Using
            End Using
        Catch : End Try
        cell.OwningRow.Cells(COL_ISI).Value = isiValue
        HitungHpp()
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  CELL END EDIT — Persis seperti FormJual
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvKomponen_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DgvKomponen.CellEndEdit
        ' Bersihkan handler TextBox DGV — Persis seperti FormJual
        If _dgvEditingTextBox IsNot Nothing Then
            RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
            RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
            _dgvEditingTextBox = Nothing
        End If
        ResetBarcodeDetection()

        ' Guard: jangan proses jika sedang diisi dari ListBox
        If _sedangSetNilaiDariListBox Then Return

        ' ═══════════════════════════════════════════════════════════
        '  KOLOM NAMA — Parse qty*nama, cari di DB, isi row
        ' ═══════════════════════════════════════════════════════════
        If e.ColumnIndex = DgvKomponen.Columns(COL_NAMA).Index Then
            Dim cv = If(DgvKomponen(COL_NAMA, e.RowIndex).Value IsNot Nothing,
                        DgvKomponen(COL_NAMA, e.RowIndex).Value.ToString().Trim(), "")
            If String.IsNullOrEmpty(cv) Then TutupListBox() : Return

            Dim qtyValue As Decimal = 1
            Dim namaBarangValue As String = cv

            ' Parse format qty*nama atau qty*level*nama
            Dim indexAsteriskQty As Integer = cv.IndexOf("*")
            Dim indexAsteriskHarga As Integer = -1
            If indexAsteriskQty >= 0 Then
                indexAsteriskHarga = cv.IndexOf("*", indexAsteriskQty + 1)
            End If

            If indexAsteriskQty >= 0 AndAlso indexAsteriskHarga > indexAsteriskQty Then
                ' Format: qty * level * namaBarang
                qtyValue = ModuleAngka.ParseDecimal(cv.Substring(0, indexAsteriskQty).Trim())
                namaBarangValue = cv.Substring(indexAsteriskHarga + 1).Trim()
            ElseIf indexAsteriskQty >= 0 Then
                ' Format: qty * namaBarang
                qtyValue = ModuleAngka.ParseDecimal(cv.Substring(0, indexAsteriskQty).Trim())
                namaBarangValue = cv.Substring(indexAsteriskQty + 1).Trim()
            End If

            If qtyValue <= 0 Then qtyValue = 1

            ' Update cell ke namaBarang bersih
            DgvKomponen(COL_NAMA, e.RowIndex).Value = namaBarangValue

            ' Cek duplikat — gabung qty jika sudah ada
            Dim idBarangCek As String = AmbilIdBarang(namaBarangValue)
            If Not String.IsNullOrEmpty(idBarangCek) Then
                For barisatas As Integer = 0 To DgvKomponen.RowCount - 1
                    If barisatas = e.RowIndex Then Continue For
                    For barisbawah As Integer = barisatas + 1 To DgvKomponen.RowCount - 1
                        Dim kodeAtas As Object = DgvKomponen.Rows(barisatas).Cells(COL_ID).Value
                        Dim kodeBawah As Object = DgvKomponen.Rows(barisbawah).Cells(COL_ID).Value
                        If kodeAtas IsNot Nothing AndAlso kodeBawah IsNot Nothing AndAlso kodeBawah.Equals(kodeAtas) Then
                            Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(DgvKomponen.Rows(barisatas).Cells(COL_QTY).Value)
                            Dim qtyTotal As Decimal = qtyLama + qtyValue
                            DgvKomponen.Rows(barisatas).Cells(COL_QTY).Value = qtyTotal
                            ' Recalculate TotalHargaBeli
                            Dim hargabeliRow As Decimal = ModuleAngka.ParseDecimal(DgvKomponen.Rows(barisatas).Cells(COL_HARGABELI).Value)
                            DgvKomponen.Rows(barisatas).Cells(COL_TOTALHARGABELI).Value = hargabeliRow * qtyTotal
                            If Not DgvKomponen.Rows(barisbawah).IsNewRow Then
                                DgvKomponen.Rows.RemoveAt(barisbawah)
                            End If
                            HitungHpp() : SetupFocusToGrid()
                            Return
                        End If
                    Next
                Next
            End If

            TutupListBox()
            IsiBarangKeRow(e.RowIndex, namaBarangValue, qtyValue)
            HitungHpp()
            SetupFocusToGrid()
        End If

        ' ═══════════════════════════════════════════════════════════
        '  KOLOM QTY — Validasi + hitung HPP
        ' ═══════════════════════════════════════════════════════════
        If e.ColumnIndex = DgvKomponen.Columns(COL_QTY).Index Then
            Dim kodeVal As String = If(DgvKomponen.Rows(e.RowIndex).Cells(COL_ID).Value IsNot Nothing,
                                       DgvKomponen.Rows(e.RowIndex).Cells(COL_ID).Value.ToString().Trim(), "")
            If String.IsNullOrEmpty(kodeVal) Then Return

            ResetBarcodeDetection()

            Dim qtyParsed As Decimal = ModuleAngka.ParseDecimal(DgvKomponen.Rows(e.RowIndex).Cells(COL_QTY).Value)
            If qtyParsed <= 0 Then
                MessageBox.Show("Qty harus lebih dari 0.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                qtyParsed = 1D
            End If
            DgvKomponen.Rows(e.RowIndex).Cells(COL_QTY).Value = qtyParsed
            ' Recalculate TotalHargaBeli = HargaBeli × Qty
            Dim hargabeliRow As Decimal = ModuleAngka.ParseDecimal(DgvKomponen.Rows(e.RowIndex).Cells(COL_HARGABELI).Value)
            DgvKomponen.Rows(e.RowIndex).Cells(COL_TOTALHARGABELI).Value = hargabeliRow * qtyParsed
            HitungHpp()
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  CELL CLICK — Hapus baris
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvKomponen_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DgvKomponen.CellClick
        If e.RowIndex < 0 Then Return
        If DgvKomponen.Columns(e.ColumnIndex).Name = NameOf(ColHapus) AndAlso Not DgvKomponen.Rows(e.RowIndex).IsNewRow Then
            DgvKomponen.Rows.RemoveAt(e.RowIndex) : HitungHpp()
        End If
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  CELL LEAVE — Guard penutup ListBox
    '  Persis seperti FormJual.DgvData_CellLeave
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvKomponen_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvKomponen.CellLeave
        If Not Me.IsHandleCreated Then Return
        Me.BeginInvoke(New Action(Sub()
                                      If LstBarang.Visible Then
                                          If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return
                                          If _listBoxDibukaDiRow >= 0 AndAlso
                   DgvKomponen.CurrentCell IsNot Nothing AndAlso
                   DgvKomponen.CurrentCell.RowIndex = _listBoxDibukaDiRow AndAlso
                   DgvKomponen.CurrentCell.ColumnIndex = _listBoxDibukaDiCol Then Return
                                          LstBarang.Visible = False : LstBarang.Items.Clear()
                                          _listBoxDibukaDiRow = -1 : _listBoxDibukaDiCol = -1
                                      End If
                                  End Sub))
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  TUTUP LISTBOX
    ' ═══════════════════════════════════════════════════════════════
    Private Sub TutupListBox()
        LstBarang.Visible = False : LstBarang.Items.Clear()
        _listBoxDibukaDiRow = -1 : _listBoxDibukaDiCol = -1
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  ROW POST PAINT — Nomor urut
    ' ═══════════════════════════════════════════════════════════════
    Private Sub DgvKomponen_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvKomponen.RowPostPaint
        Using b As New SolidBrush(DgvKomponen.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b,
                                  e.RowBounds.Location.X + 8, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    ' ═══════════════════════════════════════════════════════════════
    '  SIMPAN (F2)
    ' ═══════════════════════════════════════════════════════════════
    '  SIMPAN (F2) — Kurangi stok komponen + Insert BOM + History
    ' ═══════════════════════════════════════════════════════════════
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        TutupListBox()
        Dim namaPaket As String = TxtNama.Text.Trim()
        If String.IsNullOrWhiteSpace(namaPaket) Then MessageBox.Show("Nama paket wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TxtNama.Focus() : Return
        If _modeTambah Then _kodeRakitan = TxtKode.Text.Trim()
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHargaJual.Text)
        Dim satuan As String = If(CmbSatuan.Text, "Pcs")
        Dim lokasi As String = FormUtama.StatusLokasi.Text
        Dim tanggal As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        ' Validasi baris
        Dim lstKomponen As New List(Of (Kode As String, Nama As String, Qty As Decimal, Satuan As String))
        For Each row As DataGridViewRow In DgvKomponen.Rows
            If row.IsNewRow Then Continue For
            Dim kd = If(row.Cells(COL_ID).Value IsNot Nothing, row.Cells(COL_ID).Value.ToString().Trim(), "")
            Dim nm = If(row.Cells(COL_NAMA).Value IsNot Nothing, row.Cells(COL_NAMA).Value.ToString(), "")
            Dim qt = ModuleAngka.ParseDecimal(row.Cells(COL_QTY).Value)
            Dim sat = If(row.Cells(COL_SATUAN).Value IsNot Nothing, row.Cells(COL_SATUAN).Value.ToString(), "")
            If String.IsNullOrWhiteSpace(kd) Then MessageBox.Show("Ada baris kosong.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            If kd = _kodeRakitan Then MessageBox.Show("Komponen = paket.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            If qt <= 0 Then MessageBox.Show("Qty > 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            lstKomponen.Add((kd, nm, qt, sat))
        Next
        If lstKomponen.Count = 0 Then MessageBox.Show("Minimal 1 komponen.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return

        Try
            Dim trx = conn.BeginTransaction()
            Try
                ' Insert paket baru jika mode tambah
                If _modeTambah Then InsertBarangPaket(_kodeRakitan, namaPaket, hargaJual, satuan, trx)

                ' Update data paket
                Using cmd As New MySqlCommand(
                    "UPDATE tbl_barang SET NAMA_BARANG=@nm, HARGA_BELI=@hb, HARGA_BELI_TERAKHIR=@hb, " &
                    "BARCODE_KECIL=@bc, HARGA_JUAL_UMUM_KECIL=@hj, SATUAN_UMUM_KECIL=@sat WHERE ID_BARANG=@kd", conn, trx)
                    cmd.Parameters.AddWithValue("@nm", namaPaket)
                    cmd.Parameters.AddWithValue("@hb", ModuleAngka.ParseDecimal(TxtHargaBeli.Text))
                    cmd.Parameters.AddWithValue("@bc", TxtBarcode.Text.Trim())
                    cmd.Parameters.AddWithValue("@hj", hargaJual)
                    cmd.Parameters.AddWithValue("@sat", satuan)
                    cmd.Parameters.AddWithValue("@kd", _kodeRakitan)
                    cmd.ExecuteNonQuery()
                End Using

                ' ── Hitung selisih stok: BOM lama vs baru ──
                Dim bomLama As Dictionary(Of String, Decimal) = AmbilBomLama(trx)
                Dim kolKurang As String = If(lokasi = "GUDANG", "KURANG_GUDANG", "KURANG_TOKO")
                Dim kolTambah As String = If(lokasi = "GUDANG", "TAMBAH_GUDANG", "TAMBAH_TOKO")

                ' Kumpulkan BOM baru
                Dim bomBaru As New Dictionary(Of String, Decimal)
                For Each k In lstKomponen
                    If bomBaru.ContainsKey(k.Kode) Then
                        bomBaru(k.Kode) += k.Qty
                    Else
                        bomBaru(k.Kode) = k.Qty
                    End If
                Next

                ' Cek stok cukup untuk komponen yang bertambah qty
                Dim pesanKurang As New List(Of String)
                For Each kvp In bomBaru
                    Dim qtyLama As Decimal = 0
                    bomLama.TryGetValue(kvp.Key, qtyLama)
                    Dim selisih As Decimal = kvp.Value - qtyLama
                    If selisih > 0 Then
                        Dim stok As Decimal = AmbilStok(kvp.Key, lokasi)
                        If stok < selisih Then
                            Dim nmBarang As String = ""
                            Using cmd As New MySqlCommand("SELECT NAMA_BARANG FROM tbl_barang WHERE ID_BARANG=@id", conn, trx)
                                cmd.Parameters.AddWithValue("@id", kvp.Key)
                                Dim r = cmd.ExecuteScalar()
                                If r IsNot Nothing Then nmBarang = r.ToString()
                            End Using
                            pesanKurang.Add($"{nmBarang}: stok {stok:N2}, butuh {selisih:N2}")
                        End If
                    End If
                Next
                If pesanKurang.Count > 0 Then
                    trx.Rollback()
                    MessageBox.Show("Stok komponen tidak mencukupi:" & vbCrLf & String.Join(vbCrLf, pesanKurang),
                                    "Validasi Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Kurangi stok untuk komponen yang qty-nya naik
                For Each kvp In bomBaru
                    Dim qtyLama As Decimal = 0
                    bomLama.TryGetValue(kvp.Key, qtyLama)
                    Dim selisih As Decimal = kvp.Value - qtyLama
                    If selisih > 0 Then
                        Using cmd As New MySqlCommand($"UPDATE tbl_barang SET {kolKurang} = {kolKurang} + @qty WHERE ID_BARANG=@kode", conn, trx)
                            cmd.Parameters.AddWithValue("@qty", selisih)
                            cmd.Parameters.AddWithValue("@kode", kvp.Key)
                            cmd.ExecuteNonQuery()
                        End Using
                        HitungStokPerubahan(kvp.Key, trx)
                        Dim nmBarang As String = CariNamaBarang(kvp.Key, trx)
                        InsertHistory(_kodeRakitan, tanggal, "BAHAN RAKITAN KELUAR", kvp.Key, nmBarang, selisih, "", trx)
                    End If
                Next

                ' Kembalikan stok untuk komponen yang qty-nya turun/hapus
                For Each kvp In bomLama
                    Dim qtyBaru As Decimal = 0
                    bomBaru.TryGetValue(kvp.Key, qtyBaru)
                    Dim selisih As Decimal = kvp.Value - qtyBaru
                    If selisih > 0 Then
                        Using cmd As New MySqlCommand($"UPDATE tbl_barang SET {kolTambah} = {kolTambah} + @qty WHERE ID_BARANG=@kode", conn, trx)
                            cmd.Parameters.AddWithValue("@qty", selisih)
                            cmd.Parameters.AddWithValue("@kode", kvp.Key)
                            cmd.ExecuteNonQuery()
                        End Using
                        HitungStokPerubahan(kvp.Key, trx)
                        Dim nmBarang As String = CariNamaBarang(kvp.Key, trx)
                        InsertHistory(_kodeRakitan, tanggal, "BAHAN RAKITAN MASUK", kvp.Key, nmBarang, selisih, "", trx)
                    End If
                Next

                ' REPLACE BOM
                Using cmd As New MySqlCommand("DELETE FROM tbl_rakitan_bom WHERE kode_rakitan=@kode", conn, trx) : cmd.Parameters.AddWithValue("@kode", _kodeRakitan) : cmd.ExecuteNonQuery() : End Using
                Dim urutan As Integer = 0
                For Each k In lstKomponen
                    urutan += 1
                    Using cmd As New MySqlCommand("INSERT INTO tbl_rakitan_bom(kode_rakitan,kode_komponen,nama_komponen,qty,satuan,urutan) VALUES(@kr,@kk,@nk,@qty,@sat,@urt)", conn, trx)
                        cmd.Parameters.AddWithValue("@kr", _kodeRakitan) : cmd.Parameters.AddWithValue("@kk", k.Kode)
                        cmd.Parameters.AddWithValue("@nk", k.Nama) : cmd.Parameters.AddWithValue("@qty", k.Qty)
                        cmd.Parameters.AddWithValue("@sat", k.Satuan) : cmd.Parameters.AddWithValue("@urt", urutan)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                Using cmd As New MySqlCommand("UPDATE tbl_barang SET IS_PAKET=1 WHERE ID_BARANG=@kode", conn, trx) : cmd.Parameters.AddWithValue("@kode", _kodeRakitan) : cmd.ExecuteNonQuery() : End Using
                ModuleAuditTrail.CatatAuditMaster(_kodeRakitan, If(_modeTambah, "ADD", "EDIT"), "Rakitan BOM", If(_modeTambah, "Tambah", "Edit") & " paket " & namaPaket, trans:=trx)
                trx.Commit()
            Catch : trx.Rollback() : Throw : End Try
            MessageBox.Show("Berhasil.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information) : Me.DialogResult = DialogResult.OK : Close()
        Catch ex As Exception : MessageBox.Show("Gagal: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) : End Try
    End Sub

    Private Sub InsertBarangPaket(kode As String, nama As String, hargaJual As Decimal, satuan As String, trx As MySqlTransaction)
        Using cmd As New MySqlCommand(
            "INSERT INTO tbl_barang(ID_BARANG,NAMA_BARANG,KODE_KATEGORI,NAMA_KATEGORI,KODE_SUPLIYER,NAMA_SUPLIYER,KODE_MERK,NAMA_MERK," &
            "HARGA_BELI,HARGA_BELI_TERAKHIR,BARCODE_KECIL,BARCODE_SEDANG,BARCODE_BESAR," &
            "SATUAN_UMUM_KECIL,SATUAN_UMUM_SEDANG,SATUAN_UMUM_BESAR,ISI_UMUM_KECIL,ISI_UMUM_SEDANG,ISI_UMUM_BESAR," &
            "HARGA_JUAL_UMUM_KECIL,HARGA_JUAL_UMUM_SEDANG,HARGA_JUAL_UMUM_BESAR," &
            "SATUAN_PARTAI_KECIL,SATUAN_PARTAI_SEDANG,SATUAN_PARTAI_BESAR,ISI_PARTAI_KECIL,ISI_PARTAI_SEDANG,ISI_PARTAI_BESAR," &
            "HARGA_JUAL_PARTAI_KECIL,HARGA_JUAL_PARTAI_SEDANG,HARGA_JUAL_PARTAI_BESAR," &
            "AWAL_TOKO,TAMBAH_TOKO,KURANG_TOKO,PEMBELIAN_TOKO,PENJUALAN_TOKO,RETUR_BELI_TOKO,RETUR_JUAL_TOKO,OPNAME_TOKO," &
            "TRANSFER_STOK_MASUK_TOKO,TRANSFER_STOK_KELUAR_TOKO,TRANSFER_BARANG_MASUK_TOKO,TRANSFER_BARANG_KELUAR_TOKO," &
            "TRANSFER_CABANG_MASUK_TOKO,TRANSFER_CABANG_KELUAR_TOKO," &
            "AWAL_GUDANG,TAMBAH_GUDANG,KURANG_GUDANG,PEMBELIAN_GUDANG,PENJUALAN_GUDANG,RETUR_BELI_GUDANG,RETUR_JUAL_GUDANG,OPNAME_GUDANG," &
            "TRANSFER_STOK_MASUK_GUDANG,TRANSFER_STOK_KELUAR_GUDANG,TRANSFER_BARANG_MASUK_GUDANG,TRANSFER_BARANG_KELUAR_GUDANG," &
            "TRANSFER_CABANG_MASUK_GUDANG,TRANSFER_CABANG_KELUAR_GUDANG," &
            "SATUAN_STOK,SATUAN_ISI_STOK,STOK_MIN,STOK_MAX,STATUS) " &
            "VALUES(@kd,@nm,'PAKET','Paket Rakitan','','','',''," & ModuleAngka.ParseDecimal(TxtHargaBeli.Text).ToString() & "," & ModuleAngka.ParseDecimal(TxtHargaBeli.Text).ToString() & ",'" & TxtBarcode.Text.Trim() & "','','', " &
            "@sat,'','',1,1,1, @hj,0,0, '','',0,0,0, 0,0,0," &
            "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," &
            "0,0,0,0,0,0,0,0,0,0,0,0,0, " &
            "'Pcs',1,0,0,'Aktif')", conn, trx)
            cmd.Parameters.AddWithValue("@kd", kode)
            cmd.Parameters.AddWithValue("@nm", nama)
            cmd.Parameters.AddWithValue("@sat", satuan)
            cmd.Parameters.AddWithValue("@hj", hargaJual)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ========================================================================
    '  INSERT HISTORY — log perubahan stok komponen ke HistoryBarang
    ' ========================================================================
    Private Sub InsertHistory(faktur As String, tanggal As String, jenis As String,
                               idBarang As String, namaBarang As String,
                               qty As Decimal, satuan As String,
                               trx As MySqlTransaction)
        Dim lokasi As String = FormUtama.StatusLokasi.Text
        Using cmd As New MySqlCommand(
            "INSERT INTO HistoryBarang " &
            "(FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, " &
            " QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
            "VALUES (@fk,@tgl,@jns,@lok,@id,@nm,@qty,@sat,1,@qty,0,@usr,@pc)", conn, trx)
            cmd.Parameters.AddWithValue("@fk", faktur)
            cmd.Parameters.AddWithValue("@tgl", tanggal)
            cmd.Parameters.AddWithValue("@jns", jenis)
            cmd.Parameters.AddWithValue("@lok", lokasi)
            cmd.Parameters.AddWithValue("@id", idBarang)
            cmd.Parameters.AddWithValue("@nm", namaBarang)
            cmd.Parameters.AddWithValue("@qty", qty)
            cmd.Parameters.AddWithValue("@sat", satuan)
            cmd.Parameters.AddWithValue("@usr", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@pc", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ========================================================================
    '  HELPER: Ambil BOM lama dari DB
    ' ========================================================================
    Private Function AmbilBomLama(trx As MySqlTransaction) As Dictionary(Of String, Decimal)
        Dim result As New Dictionary(Of String, Decimal)
        Using cmd As New MySqlCommand("SELECT kode_komponen, qty FROM tbl_rakitan_bom WHERE kode_rakitan=@kode", conn, trx)
            cmd.Parameters.AddWithValue("@kode", _kodeRakitan)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim kd = rd("kode_komponen").ToString()
                    Dim qt = ModuleAngka.ParseDecimal(rd("qty"))
                    If result.ContainsKey(kd) Then
                        result(kd) += qt
                    Else
                        result(kd) = qt
                    End If
                End While
            End Using
        End Using
        Return result
    End Function

    ' ========================================================================
    '  HELPER: Ambil stok komponen
    ' ========================================================================
    Private Function AmbilStok(kodeBarang As String, lokasi As String) As Decimal
        Dim kolom As String = If(lokasi = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
        Using cmd As New MySqlCommand($"SELECT {kolom} FROM tbl_barang WHERE ID_BARANG=@id", conn)
            cmd.Parameters.AddWithValue("@id", kodeBarang)
            Dim r = cmd.ExecuteScalar()
            Return If(r IsNot Nothing AndAlso Not IsDBNull(r), ModuleAngka.ParseDecimal(r), 0D)
        End Using
    End Function

    ' ========================================================================
    '  HELPER: Hitung stok perubahan
    ' ========================================================================
    Private Sub HitungStokPerubahan(kodeBarang As String, trx As MySqlTransaction)
        Using cmd As New MySqlCommand(
            "UPDATE tbl_barang SET " &
            "STOK_TOKO = AWAL_TOKO + TAMBAH_TOKO - KURANG_TOKO + PEMBELIAN_TOKO - PENJUALAN_TOKO " &
            "- RETUR_BELI_TOKO + RETUR_JUAL_TOKO + OPNAME_TOKO " &
            "+ TRANSFER_STOK_MASUK_TOKO - TRANSFER_STOK_KELUAR_TOKO " &
            "+ TRANSFER_BARANG_MASUK_TOKO - TRANSFER_BARANG_KELUAR_TOKO " &
            "+ COALESCE(TRANSFER_CABANG_MASUK_TOKO,0) - COALESCE(TRANSFER_CABANG_KELUAR_TOKO,0), " &
            "STOK_GUDANG = AWAL_GUDANG + TAMBAH_GUDANG - KURANG_GUDANG + PEMBELIAN_GUDANG - PENJUALAN_GUDANG " &
            "- RETUR_BELI_GUDANG + RETUR_JUAL_GUDANG + OPNAME_GUDANG " &
            "+ TRANSFER_STOK_MASUK_GUDANG - TRANSFER_STOK_KELUAR_GUDANG " &
            "+ TRANSFER_BARANG_MASUK_GUDANG - TRANSFER_BARANG_KELUAR_GUDANG " &
            "+ COALESCE(TRANSFER_CABANG_MASUK_GUDANG,0) - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG,0) " &
            "WHERE ID_BARANG=@id", conn, trx)
            cmd.Parameters.AddWithValue("@id", kodeBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ========================================================================
    '  HELPER: Cari nama barang
    ' ========================================================================
    Private Function CariNamaBarang(kodeBarang As String, trx As MySqlTransaction) As String
        Using cmd As New MySqlCommand("SELECT NAMA_BARANG FROM tbl_barang WHERE ID_BARANG=@id", conn, trx)
            cmd.Parameters.AddWithValue("@id", kodeBarang)
            Dim r = cmd.ExecuteScalar()
            Return If(r IsNot Nothing, r.ToString(), "")
        End Using
    End Function

    Private Sub BtnBatal_Click(sender As Object, e As EventArgs) Handles BtnBatal.Click
        TutupListBox() : Me.DialogResult = DialogResult.Cancel : Close()
    End Sub

    Private Sub FormTambahEditRakitan_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : If Not DgvKomponen.IsCurrentCellInEditMode Then BtnSimpan.PerformClick()
            Case Keys.Escape : If Not DgvKomponen.IsCurrentCellInEditMode Then BtnBatal.PerformClick()
        End Select
    End Sub

End Class
