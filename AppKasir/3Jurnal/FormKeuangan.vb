' Tambahkan impor ini di bagian paling atas file

Public Class FormKeuangan

#Region "Fields"
    ' ================= CACHE =================
    ' Perubahan: Mengubah Shared menjadi instance-specific.
    ' Alasan: Cache yang bersifat Shared bisa menyebabkan masalah jika form ini dibuka lebih dari satu kali secara bersamaan,
    ' karena semua instance akan berbagi cache yang sama. Instance-specific cache lebih aman.
    Private _cacheAkun As New Dictionary(Of String, List(Of String))
    Private _kodeAkunCache As New Dictionary(Of String, String)
    Private _cacheLock As New Object()
    Private _cacheLastUpdate As DateTime = DateTime.MinValue
    Private ReadOnly _cacheDuration As TimeSpan = TimeSpan.FromMinutes(30)

    ' ================= DATA BINDING =================
    Private bsKeuangan As New BindingSource()
    Private dtKeuangan As New DataTable()

    ' ================= FLAGS =================
    Private _isLoading As Boolean = False
#End Region

#Region "Form Events"
    Private Sub FormKeuangan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Inisialisasi UI terlebih dahulu
        InitializeUI()

        ' Load data cache secara asynchronous
        LoadCacheDataAsync()

        ' Setup DataGridView
        SetupDataGridViewBinding()
    End Sub

    ' Perubahan: Nama metode dan variabel diganti menjadi lebih deskriptif.
    Private Sub TxtNominalKeuangan_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalKeuangan.TextChanged
        Dim nominalValue As Double
        If Double.TryParse(TxtNominalKeuangan.Text, nominalValue) Then
            LblNominalKeuangan.Text = "Rp. " & nominalValue.ToString("N0")
        Else
            LblNominalKeuangan.Text = "Rp. 0"
            ' Hindari mengosongkan textbox saat pengguna sedang mengetik, misalnya mengetik "1."
            ' Jika ingin mengosongkan, pastikan logikanya tepat agar tidak mengganggu pengalaman pengguna.
            ' TxtNominalKeuangan.Text = "" 
            ' TxtNominalKeuangan.Focus()
        End If
    End Sub

    Private Sub DTPTglKeuangan_ValueChanged(sender As Object, e As EventArgs) Handles DTPTglKeuangan.ValueChanged
        GenerateTransactionId()
        LoadDataKeuangan() ' Mengganti DGVTAMPILDATAKEUANGAN()
        TxtUraianKeuangan.Focus()
        TxtUraianKeuangan.Select()
    End Sub

    Private Sub BTNKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTNKeluar.Click
        Close()
    End Sub
#End Region

#Region "UI Initialization"
    ' ================= INISIALISASI UI =================
    Private Sub InitializeUI()
        SetupTooltips()

        ' Contoh: Asumsikan ModulHakAkses adalah modul yang sudah ada
        Dim JURNAL As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "JURNAL", conn)
        BtnSimpanKeuangan.Visible = JURNAL(1)

        PanelPemasukan.Visible = False
        PanelRinciKeuangan.Visible = False

        ' Setup form state awal
        SetInitialFormState()
    End Sub

    Private Sub SetInitialFormState()
        ' Reset semua kontrol ke state awal
        LblIdBayar.Text = ""
        TxtNoNota.Text = ""
        TxtUraianKeuangan.Text = ""
        TxtNominalKeuangan.Text = ""
        LblNominalKeuangan.Text = "Rp. 0"

        ' Clear combo boxes
        CmbDebetKeuangan.Items.Clear()
        CmbKreditKeuangan.Items.Clear()
        CmbBantuDKeuangan.Items.Clear()
        CmbBantuKKeuangan.Items.Clear()

        ' Set format tanggal
        DTPTglKeuangan.Format = DateTimePickerFormat.Custom
        DTPTglKeuangan.CustomFormat = "dd/MM/yyyy"
        DTPTglKeuangan.Value = DateTime.Now

        ' Sembunyikan panel bantu
        HideHelperPanels()

        ' Set tombol state
        SetButtonState(FormState.Add)
    End Sub

    Private Sub HideHelperPanels()
        LblBantuDKeuangan.Visible = False
        CmbBantuDKeuangan.Visible = False
        TxtBantuDKeuanganNama.Visible = False
        TxtBantuDKeuangan.Visible = False

        LblBantuKKeuangan.Visible = False
        CmbBantuKKeuangan.Visible = False
        TxtBantuKKeuanganNama.Visible = False
        TxtBantuKKeuangan.Visible = False
    End Sub

    ' Enum untuk state tombol, membuat kode lebih mudah dibaca
    Private Enum FormState
        Add
        Edit
    End Enum

    Private Sub SetButtonState(state As FormState)
        Select Case state
            Case FormState.Add
                BtnSimpanKeuangan.Visible = True
                BtnEditKeuangan.Visible = False
                BtnBatalKeuangan.Visible = False
            Case FormState.Edit
                BtnSimpanKeuangan.Visible = False
                BtnEditKeuangan.Visible = True
                BtnBatalKeuangan.Visible = True
        End Select
    End Sub
#End Region

#Region "Data Loading & Caching"
    ' Perubahan: Mengganti nama metode agar lebih deskriptif.
    ' LoadDataKeuangan sekarang menjadi titik masuk utama untuk memuat data.
    Private Sub LoadDataKeuangan()
        If _isLoading Then Return

        Try
            Dim dt As DataTable = GetKeuanganData()
            UpdateDataGridView(dt)
            UpdateTotalDisplay(dt)
        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Metode lama DGVTAMPILDATAKEUANGAN sudah tidak digunakan lagi dan bisa dihapus.
    ' Logikanya telah dipindah dan dipisah menjadi metode-metode yang lebih kecil di bawah ini.

    Private Function GetKeuanganData() As DataTable
        Dim dt As New DataTable()
        Dim tanggalAwal As Date = DTPTglKeuangan.Value.Date
        Dim tanggalAkhir As Date = tanggalAwal.AddDays(1).AddTicks(-1)

        ' Menggunakan multiline string untuk SQL agar lebih mudah dibaca
        Dim sql As String = "
            SELECT NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, 
                   AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, 
                   AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, 
                   NAMA_BANTU_D, KODE_BANTU_D, 
                   NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, ID_USER
            FROM jurnalumum
            WHERE TGL_TRANSAKSI BETWEEN @TANGGAL_AWAL AND @TANGGAL_AKHIR
              AND JENIS_TRANSAKSI = @JENIS_TRANSAKSI"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", tanggalAwal)
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", tanggalAkhir)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", LblNamaTransaksi.Text)

            Using adapter As New MySqlDataAdapter(cmd)
                adapter.Fill(dt)
            End Using
        End Using

        Return dt
    End Function

    Private Sub UpdateDataGridView(dt As DataTable)
        ' Pastikan update UI dilakukan di thread UI
        If InvokeRequired Then
            Invoke(Sub() UpdateDataGridView(dt))
            Return
        End If

        bsKeuangan.DataSource = dt
    End Sub

    Private Sub UpdateTotalDisplay(dt As DataTable)
        If InvokeRequired Then
            Invoke(Sub() UpdateTotalDisplay(dt))
            Return
        End If

        Dim total As Decimal = dt.AsEnumerable().Sum(Function(r)
                                                         Return If(IsDBNull(r("NOMINAL")), 0D, Convert.ToDecimal(r("NOMINAL")))
                                                     End Function)

        LblTotalNominal.Text = $"Total Nominal: Rp {total:N0}"
    End Sub

    ' ================= LOAD DATA (BACKGROUND) =================
    Private Async Sub LoadCacheDataAsync()
        Try
            _isLoading = True
            ' Jalankan loading data di background thread
            Await Task.Run(Sub() LoadAkunDataFromDatabase())

            ' Setelah data siap, update UI di thread UI
            Me.Invoke(Sub() UpdateUIAfterCacheLoaded())
        Catch ex As Exception
            MessageBox.Show($"Error loading cache: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isLoading = False
        End Try
    End Sub

    Private Sub LoadAkunDataFromDatabase()
        SyncLock _cacheLock
            ' Cek apakah perlu reload cache
            If _cacheAkun.Count > 0 AndAlso (DateTime.Now - _cacheLastUpdate) <= _cacheDuration Then
                Return ' Cache masih valid, tidak perlu reload
            End If

            _cacheAkun.Clear()
            _kodeAkunCache.Clear()

            Try
                Dim sql As String = "SELECT Type_Akun, Nama_Akun, Kode_Akun FROM tbl_datareferensi ORDER BY Kode_akun"
                Using cmd As New MySqlCommand(sql, conn)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            Dim typeAkun = rd("Type_Akun").ToString().Trim()
                            Dim namaAkun = rd("Nama_Akun").ToString().Trim()
                            Dim kodeAkun = rd("Kode_Akun").ToString().Trim()

                            If Not _cacheAkun.ContainsKey(typeAkun) Then
                                _cacheAkun(typeAkun) = New List(Of String)
                            End If

                            Dim displayText = $"{typeAkun} = {namaAkun}"
                            _cacheAkun(typeAkun).Add(displayText)

                            If Not _kodeAkunCache.ContainsKey(namaAkun) Then
                                _kodeAkunCache(namaAkun) = kodeAkun
                            End If
                        End While
                    End Using
                End Using

                _cacheLastUpdate = DateTime.Now

            Catch ex As Exception
                ' Log error ke console atau file log
                Console.WriteLine($"Error loading akun data: {ex.Message}")
                Throw ' Lempar kembali exception untuk ditangani di LoadCacheDataAsync
            End Try
        End SyncLock
    End Sub

    ' Metode InitializeAkunCache yang lama sudah tidak diperlukan lagi karena logikanya sudah ada di LoadAkunDataFromDatabase.
    ' Anda bisa menghapus metode InitializeAkunCache.

    Private Sub UpdateUIAfterCacheLoaded()
        ' Generate ID keuangan setelah cache siap
        GenerateTransactionId()

        ' Load data ke DataGridView
        LoadDataKeuangan()
    End Sub
#End Region

#Region "DataGridView Setup"
    ' ================= DATA GRIDVIEW SETUP =================
    Private Sub SetupDataGridViewBinding()
        DgvKeuangan.DataSource = bsKeuangan
        DgvKeuangan.AutoGenerateColumns = False

        ' Setup kolom hanya sekali
        SetupDataGridViewColumns()

        ' Enable double buffering untuk performa
        EnableDoubleBuffering(DgvKeuangan)
    End Sub

    Private Sub SetupDataGridViewColumns()
        ' Clear existing columns
        DgvKeuangan.Columns.Clear()

        ' Tambahkan kolom tombol
        AddButtonColumn("EDIT", "Edit", 60)
        AddButtonColumn("HAPUS", "Hapus", 60)

        ' Tambahkan kolom data dengan binding
        AddDataColumn("NO_TRANSAKSI", "No. Transaksi", 120)
        AddDataColumn("TGL_TRANSAKSI", "Tanggal", 100, "dd/MM/yyyy")
        AddDataColumn("URAIAN", "Uraian", 200)
        AddDataColumn("NAMA_AKUN_D", "Akun Debet", 150)
        AddDataColumn("NAMA_AKUN_K", "Akun Kredit", 150)
        AddDataColumn("NOMINAL", "Nominal", 120, "N0")
        AddDataColumn("ID_USER", "User", 80)

        ' Kolom tersembunyi untuk binding
        AddDataColumn("NO_NOTA", "No. Nota", 0, "", False)
        AddDataColumn("AKUN_D", "Kode Debet", 0, "", False)
        AddDataColumn("AKUN_K", "Kode Kredit", 0, "", False)
        AddDataColumn("NAMA_BANTU_D", "Bantu D", 0, "", False)
        AddDataColumn("KODE_BANTU_D", "Kode Bantu D", 0, "", False)
        AddDataColumn("NAMA_BANTU_K", "Bantu K", 0, "", False)
        AddDataColumn("KODE_BANTU_K", "Kode Bantu K", 0, "", False)

        ' Set properti khusus untuk kolom NOMINAL
        If DgvKeuangan.Columns.Contains("NOMINAL") Then
            DgvKeuangan.Columns("NOMINAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DgvKeuangan.Columns("NOMINAL").DefaultCellStyle.Font = New Font(DgvKeuangan.Font, FontStyle.Bold)
        End If
    End Sub

    Private Sub AddButtonColumn(name As String, text As String, width As Integer)
        Dim buttonCol As New DataGridViewButtonColumn With {
            .Name = name,
            .HeaderText = name,
            .Text = text,
            .UseColumnTextForButtonValue = True,
            .FillWeight = width
        }
        DgvKeuangan.Columns.Add(buttonCol)
    End Sub

    Private Sub AddDataColumn(name As String, headerText As String, width As Integer,
                             Optional format As String = "", Optional visible As Boolean = True)
        Dim col As New DataGridViewTextBoxColumn With {
            .Name = name,
            .HeaderText = headerText,
            .Width = width,
            .Visible = visible,
            .DataPropertyName = name ' Ini untuk binding
        }

        If Not String.IsNullOrEmpty(format) Then
            col.DefaultCellStyle.Format = format
        End If

        DgvKeuangan.Columns.Add(col)
    End Sub

    ' Perubahan: Memisahkan logika klik EDIT dan HAPUS.
    Private Sub DgvKeuangan_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvKeuangan.CellContentClick
        If e.RowIndex < 0 Then Return

        Try
            If e.ColumnIndex = DgvKeuangan.Columns("EDIT").Index Then
                HandleEditClick(e.RowIndex)
            ElseIf e.ColumnIndex = DgvKeuangan.Columns("HAPUS").Index Then
                HandleDeleteClick(e.RowIndex)
            End If
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat memproses data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HandleEditClick(rowIndex As Integer)
        SetButtonState(FormState.Edit)
        PopulateFormFromGridRow(rowIndex)
    End Sub

    Private Sub HandleDeleteClick(rowIndex As Integer)
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin akan menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Dim transactionId As String = DgvKeuangan.Rows(rowIndex).Cells("NO_TRANSAKSI").Value.ToString()
            DeleteTransaction(transactionId)
            ResetFormAfterTransaction()
        End If
    End Sub

    Private Sub PopulateFormFromGridRow(rowIndex As Integer)
        Dim row As DataGridViewRow = DgvKeuangan.Rows(rowIndex)

        LblIdBayar.Text = GetCellValue(row, "NO_TRANSAKSI")
        TxtNoNota.Text = GetCellValue(row, "NO_NOTA")
        TxtUraianKeuangan.Text = GetCellValue(row, "URAIAN")

        ' Untuk combo box, lebih aman mencari berdasarkan teks
        SetComboBoxText(CmbDebetKeuangan, GetCellValue(row, "NAMA_AKUN_D"))
        SetComboBoxText(CmbKreditKeuangan, GetCellValue(row, "NAMA_AKUN_K"))

        ' Isi helper jika ada
        If CmbBantuDKeuangan.Visible Then SetComboBoxText(CmbBantuDKeuangan, GetCellValue(row, "NAMA_BANTU_D"))
        If CmbBantuKKeuangan.Visible Then SetComboBoxText(CmbBantuKKeuangan, GetCellValue(row, "NAMA_BANTU_K"))

        Dim nominal As Decimal
        If Decimal.TryParse(GetCellValue(row, "NOMINAL"), nominal) Then
            TxtNominalKeuangan.Text = nominal.ToString("N0")
        Else
            TxtNominalKeuangan.Text = "0"
        End If
    End Sub

    Private Function GetCellValue(row As DataGridViewRow, columnName As String) As String
        Return If(row.Cells(columnName).Value?.ToString(), String.Empty)
    End Function

    Private Sub SetComboBoxText(combo As ComboBox, text As String)
        If combo.Items.Contains(text) Then
            combo.SelectedItem = text
        Else
            ' Jika teks tidak ada di items, set text langsung (hanya jika.DropDownStyle=DropDown)
            combo.Text = text
        End If
    End Sub

    Private Sub DeleteTransaction(transactionId As String)
        Try
            Using cmd As New MySqlCommand("DELETE FROM JurnalUmum WHERE NO_TRANSAKSI=@NO_TRANSAKSI", conn)
                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", transactionId)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Transaction Type Handlers"
    ' Perubahan: Metode HandleButtonClick sudah sangat baik, tidak perlu banyak perubahan.
    ' Saya hanya menambahkan komentar.
    Private Sub HandleButtonClick(clickedButton As Button, transactionName As String, detailText As String)
        ' Reset semua warna tombol ke warna asli
        ResetButtonColors()

        ' Set warna tombol yang diklik
        clickedButton.BackColor = Color.OrangeRed

        ' Update visibility dan label
        PanelPemasukan.Visible = True
        PanelRinciKeuangan.Visible = True
        LblNamaTransaksi.Text = transactionName
        LblRinciPengeluaran.Text = detailText

        DTPTglKeuangan.Value = DateTime.Now

        ' Reset form untuk transaksi baru
        ResetFormForNewTransaction()
    End Sub

    Private Sub ResetButtonColors()
        Dim originalColor As Color = SystemColors.Control ' Warna default tombol
        BtnPemasukan.BackColor = originalColor
        BtnPengeluaran.BackColor = originalColor
        BtnBiaya.BackColor = originalColor
        BtnSetorBos.BackColor = originalColor
        BtnBayarBon.BackColor = originalColor
        BtnPindahR.BackColor = originalColor
    End Sub

    ' Event handlers untuk tombol transaksi
    Private Sub BtnPemasukan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPemasukan.Click
        HandleButtonClick(BtnPemasukan, "PEMASUKAN", "RINCIAN PEMASUKAN")
    End Sub

    Private Sub BtnPengeluaran_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPengeluaran.Click
        HandleButtonClick(BtnPengeluaran, "PENGELUARAN", "RINCIAN PENGELUARAN")
    End Sub

    Private Sub BtnBiaya_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBiaya.Click
        HandleButtonClick(BtnBiaya, "BIAYA", "RINCIAN BIAYA")
    End Sub

    Private Sub BtnSetorBos_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSetorBos.Click
        HandleButtonClick(BtnSetorBos, "SETOR KE BOS", "RINCIAN SETOR KE BOS")
    End Sub

    Private Sub BtnBayarBon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBayarBon.Click
        HandleButtonClick(BtnBayarBon, "BAYAR BON PRIBADI", "RINCIAN BAYAR BON PRIBADI")
    End Sub

    Private Sub BtnPindahR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPindahR.Click
        HandleButtonClick(BtnPindahR, "PINDAH REKENING", "RINCIAN PINDAH REKENING")
    End Sub

    ' Perubahan: Mengganti nama metode dan memisahkannya dari logika reset tombol.
    Private Sub ResetFormForNewTransaction()
        SetButtonState(FormState.Add)
        SetInitialFormState() ' Gunakan metode yang sudah ada untuk reset kontrol
        PopulateAccountComboBoxes() ' Isi combo box akun
    End Sub

    Private Sub ResetFormAfterTransaction()
        ResetFormForNewTransaction()
    End Sub
#End Region

#Region "Account Comboboxes"
    ' Perubahan: Mengganti nama metode agar lebih deskriptif.
    Private Sub PopulateAccountComboBoxes()
        If InvokeRequired Then
            Invoke(Sub() PopulateAccountComboBoxes())
            Return
        End If

        CmbDebetKeuangan.Items.Clear()
        CmbKreditKeuangan.Items.Clear()

        Select Case LblNamaTransaksi.Text
            Case "PEMASUKAN"
                AddAccountsToComboBox(CmbDebetKeuangan, {"KAS", "BANK"})
                AddAccountsToComboBox(CmbKreditKeuangan, _cacheAkun.Keys.ToArray(), {"KAS", "BANK", "LABA RUGI"})
            Case "PENGELUARAN"
                AddAccountsToComboBox(CmbDebetKeuangan, _cacheAkun.Keys.ToArray(), {"KAS", "BANK", "LABA RUGI"})
                AddAccountsToComboBox(CmbKreditKeuangan, {"KAS", "BANK"})
            Case "BIAYA"
                AddAccountsToComboBox(CmbDebetKeuangan, {"BIAYA"})
                AddAccountsToComboBox(CmbKreditKeuangan, {"KAS", "BANK"})
            Case "SETOR KE BOS"
                AddAccountsToComboBox(CmbDebetKeuangan, {"04.02.001"}) ' Ini adalah kode spesifik, bukan tipe
                AddAccountsToComboBox(CmbKreditKeuangan, {"KAS"})
            Case "BAYAR BON PRIBADI"
                AddAccountsToComboBox(CmbDebetKeuangan, {"KAS", "BANK"})
                AddAccountsToComboBox(CmbKreditKeuangan, {"PIUTANG"})
            Case "PINDAH REKENING"
                AddAccountsToComboBox(CmbDebetKeuangan, _cacheAkun.Keys.ToArray(), {"LABA RUGI"})
                AddAccountsToComboBox(CmbKreditKeuangan, _cacheAkun.Keys.ToArray(), {"LABA RUGI"})
        End Select

        ' Set pilihan pertama sebagai default
        If CmbDebetKeuangan.Items.Count > 0 Then CmbDebetKeuangan.SelectedIndex = 0
        If CmbKreditKeuangan.Items.Count > 0 Then CmbKreditKeuangan.SelectedIndex = 0
    End Sub

    Private Sub AddAccountsToComboBox(combo As ComboBox, accountTypes() As String, Optional excludeTypes As String() = Nothing)
        For Each accountType In accountTypes
            ' Jika accountType adalah kode akun spesifik (seperti "04.02.001"), cari di cache
            If _kodeAkunCache.ContainsValue(accountType) Then
                Dim namaAkun = _kodeAkunCache.FirstOrDefault(Function(kvp) kvp.Value = accountType).Key
                If Not String.IsNullOrEmpty(namaAkun) Then
                    combo.Items.Add(namaAkun)
                End If
            ElseIf _cacheAkun.ContainsKey(accountType) AndAlso (excludeTypes Is Nothing OrElse Not excludeTypes.Contains(accountType)) Then
                combo.Items.AddRange(_cacheAkun(accountType).ToArray())
            End If
        Next
    End Sub

    ' Event handlers untuk combo box
    Private Sub CmbDebetKeuangan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbDebetKeuangan.SelectedIndexChanged
        SetAccountCodeFromCombo(CmbDebetKeuangan, TxtDebetKeuanganNama, TxtDebetKeuangan)
        CmbKreditKeuangan.Focus()
    End Sub

    Private Sub CmbKreditKeuangan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbKreditKeuangan.SelectedIndexChanged
        SetAccountCodeFromCombo(CmbKreditKeuangan, TxtKreditKeuanganNama, TxtKreditKeuangan)
        If CmbBantuDKeuangan.Visible Then
            CmbBantuDKeuangan.Focus()
        ElseIf CmbBantuKKeuangan.Visible Then
            CmbBantuKKeuangan.Focus()
        Else
            TxtNominalKeuangan.Focus()
        End If
    End Sub

    Private Sub SetAccountCodeFromCombo(combo As ComboBox, txtNama As TextBox, txtKode As TextBox)
        If combo.SelectedItem Is Nothing Then Return

        ' Asumsikan combo diisi dengan nama akun
        Dim selectedAccountName As String = combo.SelectedItem.ToString()
        txtNama.Text = selectedAccountName

        ' Cari kode akun berdasarkan nama
        If _kodeAkunCache.ContainsKey(selectedAccountName) Then
            txtKode.Text = _kodeAkunCache(selectedAccountName)
        Else
            ' Jika tidak ditemukan, mungkin item adalah "Type = Nama"
            Dim parts() As String = selectedAccountName.Split("="c)
            If parts.Length = 2 Then
                Dim namaAkun = parts(1).Trim()
                txtNama.Text = namaAkun
                If _kodeAkunCache.ContainsKey(namaAkun) Then
                    txtKode.Text = _kodeAkunCache(namaAkun)
                End If
            End If
        End If
    End Sub
#End Region

#Region "CRUD Operations"
    ' Perubahan: Memisahkan logika penyimpanan dari event handler.
    Private Sub BtnSimpanKeuangan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpanKeuangan.Click
        If ValidateInput() Then
            Try
                SaveNewTransaction()
                ResetFormAfterTransaction()
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan saat menyimpan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub SaveNewTransaction()
        ' Menggunakan multiline string untuk SQL agar lebih mudah dibaca
        Dim sql As String = "
            INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, 
                                   AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, 
                                   AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, 
                                   NAMA_BANTU_D, KODE_BANTU_D, 
                                   NAMA_BANTU_K, KODE_BANTU_K, 
                                   NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) 
            VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, 
                    @AKUN_D, @NAMA_AKUN_D, @NOMOR_AKUN_D, 
                    @AKUN_K, @NAMA_AKUN_K, @NOMOR_AKUN_K, 
                    @NAMA_BANTU_D, @KODE_BANTU_D, 
                    @NAMA_BANTU_K, @KODE_BANTU_K, 
                    @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)"

        ExecuteNonQuery(sql, GetTransactionParameters())
    End Sub

    ' Perubahan: Memisahkan logika update dari event handler.
    Private Sub BtnEditKeuangan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnEditKeuangan.Click
        If ValidateInput() Then
            Try
                UpdateExistingTransaction()
                ResetFormAfterTransaction()
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan saat mengedit data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub UpdateExistingTransaction()
        Dim sql As String = "
            UPDATE JurnalUmum 
            SET TGL_TRANSAKSI = @TGL_TRANSAKSI, 
                NO_NOTA = @NO_NOTA, 
                URAIAN = @URAIAN, 
                AKUN_D = @AKUN_D, 
                NAMA_AKUN_D = @NAMA_AKUN_D, 
                NOMOR_AKUN_D = @NOMOR_AKUN_D, 
                AKUN_K = @AKUN_K, 
                NAMA_AKUN_K = @NAMA_AKUN_K, 
                NOMOR_AKUN_K = @NOMOR_AKUN_K, 
                NAMA_BANTU_D = @NAMA_BANTU_D, 
                KODE_BANTU_D = @KODE_BANTU_D, 
                NAMA_BANTU_K = @NAMA_BANTU_K, 
                KODE_BANTU_K = @KODE_BANTU_K, 
                NOMINAL = @NOMINAL, 
                JENIS_TRANSAKSI = @JENIS_TRANSAKSI, 
                LOKASI = @LOKASI 
            WHERE NO_TRANSAKSI = @NO_TRANSAKSI"

        ExecuteNonQuery(sql, GetTransactionParameters())
    End Sub

    ' Metode helper untuk mengumpulkan parameter, menghindari duplikasi kode
    Private Function GetTransactionParameters() As MySqlParameter()
        Dim nominal As Decimal
        Decimal.TryParse(TxtNominalKeuangan.Text, nominal)

        Return {
            New MySqlParameter("@NO_TRANSAKSI", LblIdBayar.Text),
            New MySqlParameter("@TGL_TRANSAKSI", DTPTglKeuangan.Value),
            New MySqlParameter("@NO_NOTA", TxtNoNota.Text),
            New MySqlParameter("@URAIAN", TxtUraianKeuangan.Text),
            New MySqlParameter("@AKUN_D", CmbDebetKeuangan.Text),
            New MySqlParameter("@NAMA_AKUN_D", TxtDebetKeuanganNama.Text),
            New MySqlParameter("@NOMOR_AKUN_D", TxtDebetKeuangan.Text),
            New MySqlParameter("@AKUN_K", CmbKreditKeuangan.Text),
            New MySqlParameter("@NAMA_AKUN_K", TxtKreditKeuanganNama.Text),
            New MySqlParameter("@NOMOR_AKUN_K", TxtKreditKeuangan.Text),
            New MySqlParameter("@NAMA_BANTU_D", If(CmbBantuDKeuangan.Visible, CmbBantuDKeuangan.Text, String.Empty)),
            New MySqlParameter("@KODE_BANTU_D", If(CmbBantuDKeuangan.Visible, TxtBantuDKeuangan.Text, String.Empty)),
            New MySqlParameter("@NAMA_BANTU_K", If(CmbBantuKKeuangan.Visible, CmbBantuKKeuangan.Text, String.Empty)),
            New MySqlParameter("@KODE_BANTU_K", If(CmbBantuKKeuangan.Visible, TxtBantuKKeuangan.Text, String.Empty)),
            New MySqlParameter("@NOMINAL", nominal),
            New MySqlParameter("@JENIS_TRANSAKSI", LblNamaTransaksi.Text),
            New MySqlParameter("@LOKASI", FormUtama.SLokasi.Text),
            New MySqlParameter("@ID_USER", FormUtama.SLogin.Text),
            New MySqlParameter("@ID_KOMPUTER", FormUtama.Comp.Text)
        }
    End Function

    ' Metode helper untuk mengeksekusi non-query, menghindari duplikasi blok Using
    Private Sub ExecuteNonQuery(sql As String, parameters As MySqlParameter())
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddRange(parameters)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub BtnBatalKeuangan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBatalKeuangan.Click
        ResetFormAfterTransaction()
    End Sub

    ' Perubahan: Mengganti nama metode agar lebih deskriptif.
    Private Function ValidateInput() As Boolean
        ' Menggunakan array of tuples untuk validasi yang lebih bersih
        Dim requiredFields As (Control As Control, ErrorMessage As String)() = {
            (TxtUraianKeuangan, "Uraian harus diisi."),
            (CmbDebetKeuangan, "Akun Debet harus dipilih."),
            (CmbKreditKeuangan, "Akun Kredit harus dipilih.")
        }

        For Each field In requiredFields
            If String.IsNullOrWhiteSpace(field.Control.Text) Then
                MessageBox.Show(field.ErrorMessage, "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                field.Control.Focus()
                Return False
            End If
        Next

        If CmbBantuDKeuangan.Visible AndAlso String.IsNullOrWhiteSpace(CmbBantuDKeuangan.Text) Then
            MessageBox.Show("Bantu D harus diisi.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbBantuDKeuangan.Focus()
            Return False
        End If

        If CmbBantuKKeuangan.Visible AndAlso String.IsNullOrWhiteSpace(CmbBantuKKeuangan.Text) Then
            MessageBox.Show("Bantu K harus diisi.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbBantuKKeuangan.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TxtNominalKeuangan.Text) OrElse Not Decimal.TryParse(TxtNominalKeuangan.Text, Nothing) Then
            MessageBox.Show("Nominal harus diisi dengan angka yang valid.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNominalKeuangan.Focus()
            Return False
        End If

        Return True
    End Function
#End Region

#Region "Helper Functions"
    ' Perubahan: Mengganti nama metode agar lebih deskriptif.
    Private Sub GenerateTransactionId()
        Dim tanggal As String = DTPTglKeuangan.Value.ToString("yyMMdd")
        Dim prefix As String = GetTransactionPrefix(LblNamaTransaksi.Text)
        Dim ceknomor As String = $"{prefix}-{tanggal}"

        Dim sql As String = "SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Dim maxKode = cmd.ExecuteScalar()?.ToString()
            Dim urutanKode As String = If(String.IsNullOrEmpty(maxKode), ceknomor & "0001", $"{ceknomor}{(CInt(maxKode.Substring(maxKode.Length - 4)) + 1):0000}")
            LblIdBayar.Text = urutanKode
        End Using
    End Sub

    Private Function GetTransactionPrefix(transactionName As String) As String
        Select Case transactionName
            Case "PEMASUKAN" : Return "MS"
            Case "PENGELUARAN" : Return "KL"
            Case "BIAYA" : Return "BY"
            Case "SETOR KE BOS" : Return "SB"
            Case "BAYAR BON PRIBADI" : Return "BB"
            Case "PINDAH REKENING" : Return "PR"
            Case Else : Return "TR"
        End Select
    End Function

    ' Perubahan: Memisahkan logika setup tooltip ke metode yang lebih kecil.
    Private Sub SetupTooltips()
        ' Atur tampilan tooltip
        ToolTip1.IsBalloon = True
        ToolTip1.ToolTipIcon = ToolTipIcon.Info
        ToolTip1.ToolTipTitle = "Keterangan Menu"
        ToolTip1.AutoPopDelay = 15000
        ToolTip1.InitialDelay = 300
        ToolTip1.ReshowDelay = 100

        SetTooltip(BtnPemasukan, "💰 JURNAL PEMASUKAN", "Catat semua penerimaan uang tunai atau transfer di luar penjualan. Contoh: modal masuk, retur beli, pendapatan lain-lain.")
        SetTooltip(BtnPengeluaran, "💸 JURNAL PENGELUARAN", "Catat semua pengeluaran uang untuk operasional atau kebutuhan non-pembelian. Contoh: bayar listrik, transport, pembelian alat tulis.")
        SetTooltip(BtnBiaya, "📑 JURNAL BIAYA USAHA", "Digunakan untuk mencatat biaya-biaya tetap atau rutin perusahaan. Contoh: gaji karyawan, sewa bulanan, biaya iklan.")
        SetTooltip(BtnPindahR, "🔁 PINDAH ANTAR REKENING / KAS", "Pindahkan dana antar akun, contoh: dari Kas ke Bank BCA atau antar bank.")
        SetTooltip(BtnSetorBos, "🏦 SETOR KAS KE BOS / PEMILIK", "Digunakan untuk mencatat setoran uang hasil usaha ke rekening pribadi pemilik. Tercatat sebagai pengurangan kas usaha dan pengambilan modal.")
    End Sub

    Private Sub SetTooltip(control As Control, title As String, text As String)
        ToolTip1.SetToolTip(control, $"{title}{Environment.NewLine}{Environment.NewLine}{text}")
    End Sub

    ' Method untuk enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
    End Sub
#End Region

#Region "Keyboard Shortcuts"
    ' Perubahan: Tidak ada perubahan signifikan, logikanya sudah bagus.
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.F2
                If BtnSimpanKeuangan.Visible AndAlso BtnSimpanKeuangan.Enabled Then
                    BtnSimpanKeuangan.PerformClick()
                    Return True
                End If
            Case Keys.F3
                If BtnEditKeuangan.Visible AndAlso BtnEditKeuangan.Enabled Then
                    BtnEditKeuangan.PerformClick()
                    Return True
                End If
            Case Keys.F5
                LoadDataKeuangan() ' Mengganti DGVTAMPILDATAKEUANGAN()
                Return True
            Case Keys.Escape
                If BtnBatalKeuangan.Visible Then
                    BtnBatalKeuangan.PerformClick()
                Else
                    BTNKeluar.PerformClick()
                End If
                Return True
            Case Keys.Enter
                ' Handle Enter key untuk navigasi
                If ActiveControl Is TxtUraianKeuangan Then
                    CmbDebetKeuangan.Focus()
                    Return True
                ElseIf ActiveControl Is CmbDebetKeuangan Then
                    CmbKreditKeuangan.Focus()
                    Return True
                ElseIf ActiveControl Is CmbKreditKeuangan Then
                    If CmbBantuDKeuangan.Visible Then
                        CmbBantuDKeuangan.Focus()
                    ElseIf CmbBantuKKeuangan.Visible Then
                        CmbBantuKKeuangan.Focus()
                    Else
                        TxtNominalKeuangan.Focus()
                    End If
                    Return True
                ElseIf ActiveControl Is TxtNominalKeuangan Then
                    If BtnSimpanKeuangan.Visible Then
                        BtnSimpanKeuangan.PerformClick()
                    ElseIf BtnEditKeuangan.Visible Then
                        BtnEditKeuangan.PerformClick()
                    End If
                    Return True
                End If
        End Select

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
#End Region

End Class