Public Class FormSalesOrder

    Private ReadOnly Property IsModeTambahSO As Boolean
        Get
            Return String.Equals(TxtJenistransaksi.Text, "TambahSalesOrder", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' ✅ TAMBAHAN: Variabel untuk menyimpan jumlah kembalian
    Private kembaliAmount As Decimal = 0

    ' ✅ Variabel grand total SO — sumber akurat untuk penyimpanan database
    '    Textbox hanya untuk tampilan UI, variabel ini untuk kalkulasi & save
    Private _grandTotalSO As Decimal = 0

    ' Marquee (teks berjalan) 
    Private _timerMarquee As System.Timers.Timer = Nothing
    Private _marqueeX As Integer = 0
    Private _marqueeSpeed As Integer = 2
    Private _marqueeTextWidth As Integer = 0

    Private Sub Form_SO_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Try
            ' Terapkan tema warna otomatis sesuai kategori form
            ModuleTheme.TerapkanTheme(Me)

            ' Area input dan grand total otomatis via nama kontrol
            ' Rename GroupBox -> GBInput/GBTotal untuk tema otomatis
            ' --- 1. SETUP TAMPILAN DASAR ---
            LblTextJalanAtas.Text = "TERIMA KASIH TELAH BELANJA DI " & NAMA_PERUSAHAAN

            ' Mulai marquee 
            BtnKeluarForm.BringToFront()
            _marqueeTextWidth = LblTextJalanAtas.PreferredWidth
            _marqueeX = PanelHeader.Width - 35
            LblTextJalanAtas.Left = _marqueeX
            _timerMarquee = New System.Timers.Timer(30)
            _timerMarquee.AutoReset = True
            AddHandler _timerMarquee.Elapsed, AddressOf MarqueeElapsed
            _timerMarquee.Start()

            ' ✅ PERBAIKAN FASE 1: Safe FormUtama access menggunakan helper
            LblLokasiBarang.Text = FormUtama.StatusLokasi.Text

            ' Ukuran Form
            MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
            MinimumSize = Size

            ' --- 2. SETUP KOMPONEN UI & TIMER ---
            KosongTxtboxcari()

            barcodeTimer.Interval = 100
            AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick
            AddHandler _searchTimer.Tick, AddressOf SearchTimer_Tick

            ' --- 3. AMBIL HAK AKSES (SETTING) ---
            ' Setting dibaca langsung dari ModulHakAkses property (Boolean) — tidak perlu cache lokal

            ' --- 4. SETUP DGV ---
            DgvDataTransaksi.EnableHeadersVisualStyles = False
            DgvDataTransaksi.RowHeadersVisible = True

            ' --- 5. SETUP TANGGAL ---
            ' Mode tambah: reset ke sekarang, kunci jika backdate tidak diizinkan
            ResetDTPKeTanggalHariIni(DTPTgl)
            DTPTgl.Format = DateTimePickerFormat.Custom
            DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"

            ' --- 6. ATUR KOLOM GRID BERDASARKAN HAK AKSES ---
            If Not ModulHakAkses.SettingIzinkanUbahHargaJual Then
                DgvDataTransaksi.Columns("Harga").ReadOnly = True
            Else
                DgvDataTransaksi.Columns("Harga").ReadOnly = False
            End If

            If ModulHakAkses.SettingTampilInfoStok Then
                DgvDataTransaksi.Columns("StokToko").Visible = True
                DgvDataTransaksi.Columns("StokGudang").Visible = True
            Else
                DgvDataTransaksi.Columns("StokToko").Visible = False
                DgvDataTransaksi.Columns("StokGudang").Visible = False
            End If

            If Not ModulHakAkses.SettingIzinkanDiskonItem Then
                DgvDataTransaksi.Columns("DiskonPersen").Visible = False
                DgvDataTransaksi.Columns("DiskonRp").Visible = False
                DgvDataTransaksi.Columns("TotalDiskon").Visible = False
            Else
                DgvDataTransaksi.Columns("DiskonPersen").Visible = True
                DgvDataTransaksi.Columns("DiskonRp").Visible = True
                DgvDataTransaksi.Columns("TotalDiskon").Visible = True
            End If

            ' Sembunyikan panel pencarian jika setting diaktifkan
            If ModulHakAkses.SettingSembunyikanPencarianAtas Then
                PanelCari.Visible = False
            Else
                PanelCari.Visible = True
            End If

            FormatKolomDenganCultureIndonesia()

            ' --- 7. PENGATURAN USER SETTINGS (PREFERENCES) ---
            ChkTampilSN.Checked = AppConfig.Instance.GetValue(Of Boolean)("TampilSN", False)

            If DgvDataTransaksi.Columns.Contains("SerialNumber") Then
                DgvDataTransaksi.Columns("SerialNumber").Visible = ChkTampilSN.Checked
            End If

        Catch ex As Exception
            MessageBox.Show("Error Load: " & ex.Message)
        End Try

    End Sub

    Private Sub Form_SO_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        Try
            ' Paksa maximize — WindowState di designer diabaikan saat ShowDialog pada singleton form
            If Me.WindowState <> FormWindowState.Maximized Then
                Me.WindowState = FormWindowState.Maximized
            End If

            ' Paksa refresh TextAlign TxtGrandtotal — saat InitializeComponent form masih kecil
            ' (100x20) sehingga Windows fallback ke Left dan meng-cache posisi teks.
            ' Setelah maximize ukuran sudah benar, reset TextAlign agar Windows recalculate.
            TxtGrandtotal.TextAlign = HorizontalAlignment.Left
            TxtGrandtotal.TextAlign = HorizontalAlignment.Right

            If IsModeTambahSO Then
                ' Panggil reset form
                Kondisiawal()
            Else
                ' Panggil mode Edit
                Dim _tShown As DateTime = DateTime.Now
                EditSOHeader()
            End If

        Catch ex As Exception
            MessageBox.Show("Error Shown: " & ex.Message)
        End Try
    End Sub

    Private Sub FormatKolomDenganCultureIndonesia()
        ' Daftar nama kolom yang akan diformat
        Dim kolomList As String() = {
    "HargaBeli", "QTY", "Isi", "Totalhargabeli", "Harga", "QtySat",
    "DiskonPersen", "DiskonRp", "TotalDiskon", "TotalHarga",
    "StokToko", "StokGudang", "Stok"
}

        ModuleAngka.TerapkanFormatKolomAngka(DgvDataTransaksi, kolomList)
    End Sub

    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)

        ' ✅ PERBAIKAN FASE 1: Safe bounds checking untuk DataGridView
        If DgvDataTransaksi.Rows.Count > 0 AndAlso DgvDataTransaksi.Columns.Count > 1 Then
            Try
                ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                DgvDataTransaksi.CurrentCell = DgvDataTransaksi(1, DgvDataTransaksi.Rows.Count - 1)
                ' Mengatur baris terakhir sebagai baris yang dipilih
                DgvDataTransaksi.Rows(DgvDataTransaksi.Rows.Count - 1).Selected = True
            Catch
                ' Ignore jika cell access gagal
            End Try
        End If
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtNama_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        ResetBarcodeDetection()
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_Panel, ModuleTheme.D_Panel)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        ' Handle navigasi keyboard ke ListBox dari DGV/TxtNama
        ' DataGridView mengkonsumsi Down arrow, perlu ditangkap di ProcessCmdKey
        If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            Select Case keyData
                Case Keys.Down
                    ' Jika ListBox sudah fokus → biarkan Down diteruskan ke ListBox untuk navigasi normal
                    ' Jika belum fokus (user masih di TxtNama/DGV) → pindahkan fokus ke ListBox
                    If LstBarang.Focused Then
                        Return MyBase.ProcessCmdKey(msg, keyData)
                    End If
                    ' Simpan teks yang sedang diketik sebelum pindah ke ListBox
                    ' agar bisa di-restore saat user tekan Up untuk refine search
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                    Else
                        _teksSebelumPindahKeLstBarang = TxtNama.Text
                    End If
                    ' Masalah: LstBarang.Focus() tidak langsung berhasil karena DGV BeginEdit ulang
                    ' setelah Focus() merebut kembali fokus ke DGV.
                    ' Solusi: nested BeginInvoke — lapis pertama menunggu CellLeave+EditingControlShowing selesai,
                    ' lapis kedua baru panggil Focus() setelah DGV benar-benar selesai BeginEdit ulang.
                    _sedangPindahKeLstBarang = True
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    Me.BeginInvoke(New Action(Sub()
                                                  ' Lapis 1: tunggu CellLeave BeginInvoke selesai dulu
                                                  Me.BeginInvoke(New Action(Sub()
                                                                                ' Lapis 2: EndEdit dulu agar DGV tidak merebut fokus kembali.
                                                                                ' _sedangSetNilaiDariListBox = True agar CellEndEdit tidak memproses teks keyword.
                                                                                If LstBarang.Visible Then
                                                                                    _sedangSetNilaiDariListBox = True
                                                                                    DgvDataTransaksi.EndEdit()
                                                                                    _sedangSetNilaiDariListBox = False
                                                                                    LstBarang.Focus()
                                                                                End If
                                                                                _sedangPindahKeLstBarang = False
                                                                            End Sub))
                                              End Sub))
                    Return True
                Case Keys.Enter
                    ' Enter saat ListBox visible → pilih item pertama atau yang ter-highlight
                    If LstBarang.SelectedIndex < 0 Then
                        LstBarang.SelectedIndex = 0
                    End If
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

    Private Sub KosongTxtboxcari()
        ' Stop search timer dan reset keyword — cegah query sisa dari sesi sebelumnya
        _searchTimer.Stop()
        _searchKeywordPending = ""
        LstBarang.Items.Clear()
        LstBarang.Visible = False
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHargaBeli.Clear()
        TxtBarcode.Clear()
        TxtHargaJual.Clear()
        TxtStokToko.Clear()
        TXtStokGudang.Clear()
        TxtStok.Clear()
        TxtNama.Clear()
        rtbPetunjuk.Clear()

        ' Setup fokus berdasarkan setting setelah bersihkan input
        SetupFocusToGrid()
    End Sub

    Public Sub Kondisiawal()

        ' Persiapan untuk mode Tambah Baru
        LblJenisPl.Text = "Umum"
        LbLKodePel.Text = ""

        DgvDataTransaksi.DataSource = Nothing
        DgvDataTransaksi.Rows.Clear()

        TampilPelanggan()
        AmbilDataKaryawan()
        NomorSO()
        UpdateSemuaTotal()

        If CmbPelanggan.Items.Count > 0 Then CmbPelanggan.SelectedIndex = 0

        ' Set default sales ke user yang login dari Form Utama
        Dim userLogin As String = FormUtama.StatusNamaUser.Text.Trim()
        If Not String.IsNullOrEmpty(userLogin) AndAlso userLogin <> "Login" Then
            If Not CmbSales.Items.Contains(userLogin) Then
                CmbSales.Items.Add(userLogin)
            End If
            CmbSales.Text = userLogin
            Call AmbilInformasiSales()
        End If
        ' ✅ Gunakan format STANDAR (InvariantCulture - no separator) - tampilkan desimal hanya jika ada
        _grandTotalSO = 0
        TxtTotaljualStlPajak.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtTotalJualSblDiskonPajak.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtGrandtotal.Text = "Rp. 0"


        TxtLogin.Clear()
        TxtKomputer.Clear()

        SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
    End Sub

    ' ===== REUSABLE SUB - ATUR FOKUS KE DATAGRID =====
    ''' <summary>
    ''' Mengatur fokus ke DataGridView dengan behavior berbeda berdasarkan SettingFokusOtomatis
    ''' ✅ Jika SettingFokusOtomatis = True: fokus ke TxtNama (mode pencarian)
    ''' ✅ Jika SettingFokusOtomatis = False: fokus ke sel NamaBarang baris terakhir (mode edit langsung)
    ''' </summary>
    Public Sub SetupFocusToGrid()
        If ModulHakAkses.SettingFokusOtomatis Then
            ' MODE 1: Pencarian - fokus ke TxtNama (input manual/barcode)
            TxtNama.Focus()
            Return
        End If

        ' MODE 2: Edit Langsung - fokus ke sel NamaBarang untuk edit inline
        If DgvDataTransaksi.Rows.Count = 0 Then
            Return
        End If

        ' Cari baris kosong SETELAH baris terakhir yang terisi
        Dim targetRow As Integer = 0
        Dim lastFilledRow As Integer = -1

        ' Cari baris terakhir yang terisi (ada kode)
        For i As Integer = DgvDataTransaksi.Rows.Count - 1 To 0 Step -1
            If Not DgvDataTransaksi.Rows(i).IsNewRow Then
                Dim kodeVal = Convert.ToString(DgvDataTransaksi.Rows(i).Cells("Kode").Value).Trim()
                If Not String.IsNullOrEmpty(kodeVal) Then
                    lastFilledRow = i
                    Exit For
                End If
            End If
        Next

        ' Cari baris kosong setelah baris terakhir yang terisi
        If lastFilledRow >= 0 Then
            ' Ada baris terisi, cari baris kosong setelahnya
            Dim foundEmptyRow As Boolean = False
            For i As Integer = lastFilledRow + 1 To DgvDataTransaksi.Rows.Count - 1
                If Not DgvDataTransaksi.Rows(i).IsNewRow Then
                    Dim kodeVal = Convert.ToString(DgvDataTransaksi.Rows(i).Cells("Kode").Value).Trim()
                    If String.IsNullOrEmpty(kodeVal) Then
                        targetRow = i

                        foundEmptyRow = True
                        Exit For
                    End If
                End If
            Next

            ' Jika tidak ada baris kosong non-IsNewRow, cek IsNewRow — jangan Rows.Add() yang buat baris ekstra
            If Not foundEmptyRow Then
                Dim isNewRowIdx As Integer = -1
                For i As Integer = lastFilledRow + 1 To DgvDataTransaksi.Rows.Count - 1
                    If DgvDataTransaksi.Rows(i).IsNewRow Then
                        isNewRowIdx = i
                        Exit For
                    End If
                Next
                If isNewRowIdx >= 0 Then
                    targetRow = isNewRowIdx
                Else
                    ' Tidak ada IsNewRow, jangan paksa add baris baru
                    ' pakai baris aktif jika ada, atau keluar
                    If DgvDataTransaksi.CurrentCell IsNot Nothing Then
                        targetRow = DgvDataTransaksi.CurrentCell.RowIndex
                    Else
                        Exit Sub
                    End If
                End If
            End If
        Else
            ' Tidak ada baris terisi, gunakan baris pertama
            targetRow = 0
        End If

        ' Set CurrentCell dan fokus ke DGV
        If targetRow < DgvDataTransaksi.Rows.Count Then
            ' [F10-T28-1] SIMPAN: Simpan reference CurrentCell sebelum BeginInvoke
            ' Alasan: Mencegah race condition jika CurrentCell berubah selama async operation
            Dim targetColumnIndex As Integer = 1 ' Kolom NamaBarang
            Dim targetRowIndex As Integer = targetRow

            DgvDataTransaksi.CurrentCell = DgvDataTransaksi(targetColumnIndex, targetRowIndex)
            Me.ActiveControl = DgvDataTransaksi

            ' [F10-T28-2] SEDERHANAKAN: Nested BeginInvoke dari 3 level ke 1 level
            ' Alasan: Mengurangi delay dan kompleksitas async operation
            DgvDataTransaksi.BeginInvoke(New Action(Sub()
                                                        ' Cek apakah CurrentCell masih sama dengan target (race condition guard)
                                                        If DgvDataTransaksi.CurrentCell IsNot Nothing AndAlso
                   DgvDataTransaksi.CurrentCell.ColumnIndex = targetColumnIndex AndAlso
                   DgvDataTransaksi.CurrentCell.RowIndex = targetRowIndex Then
                                                            DgvDataTransaksi.BeginEdit(True)
                                                            DgvDataTransaksi.EditingControl?.Focus()
                                                        End If
                                                    End Sub))
        End If
    End Sub

    Private SkipValidation As Boolean = False
    Private ReadOnly _mapNoTelpKeNamaPelanggan As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _mapNamaPelanggan As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    Public Sub TampilPelanggan()
        Using cmd As New MySqlCommand("SELECT NAMA, NO_TELP FROM tbl_pelanggan WHERE Status = 'Aktif' ORDER BY NAMA ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbPelanggan.Items.Clear()
                CmbPelanggan.Items.Add("")
                CmbPelanggan.AutoCompleteCustomSource.Clear()
                _mapNoTelpKeNamaPelanggan.Clear()
                _mapNamaPelanggan.Clear()

                While rd.Read()
                    Dim nama As String = rd("NAMA").ToString().Trim()
                    Dim noTelp As String = ModuleAngka.SafeGetValue(Of String)(rd, "NO_TELP", "").Trim()
                    If String.IsNullOrWhiteSpace(nama) Then Continue While

                    CmbPelanggan.Items.Add(nama)
                    _mapNamaPelanggan.Add(nama)

                    If Not CmbPelanggan.AutoCompleteCustomSource.Contains(nama) Then
                        CmbPelanggan.AutoCompleteCustomSource.Add(nama)
                    End If

                    If Not String.IsNullOrWhiteSpace(noTelp) Then
                        Dim hpNorm As String = NormalisasiNoTelp(noTelp)
                        If Not String.IsNullOrWhiteSpace(hpNorm) Then
                            _mapNoTelpKeNamaPelanggan(hpNorm) = nama
                            If Not CmbPelanggan.AutoCompleteCustomSource.Contains(noTelp) Then
                                CmbPelanggan.AutoCompleteCustomSource.Add(noTelp)
                            End If
                            If hpNorm <> noTelp AndAlso Not CmbPelanggan.AutoCompleteCustomSource.Contains(hpNorm) Then
                                CmbPelanggan.AutoCompleteCustomSource.Add(hpNorm)
                            End If
                        End If
                    End If
                End While
            End Using
        End Using

        CmbPelanggan.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        CmbPelanggan.AutoCompleteSource = AutoCompleteSource.CustomSource

        ' Kosongkan combobox tanpa memicu validating
        SkipValidation = True
        CmbPelanggan.SelectedIndex = -1
        SkipValidation = False

        ' Cegah event handler dobel
        RemoveHandler CmbPelanggan.Validating, AddressOf ComboBox_Validating
        AddHandler CmbPelanggan.Validating, AddressOf ComboBox_Validating
    End Sub

    Private Function NormalisasiNoTelp(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""
        Dim hasil As String = input.Trim()
        hasil = hasil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
        Return hasil
    End Function

    Private Function CobaKonversiInputPelangganKeNama(ByVal inputText As String, ByRef namaHasil As String) As Boolean
        namaHasil = ""
        Dim text As String = If(inputText, "").Trim()
        If String.IsNullOrWhiteSpace(text) Then Return False

        If _mapNamaPelanggan.Contains(text) Then
            namaHasil = text
            Return True
        End If

        Dim hpNorm As String = NormalisasiNoTelp(text)
        If String.IsNullOrWhiteSpace(hpNorm) Then Return False

        If _mapNoTelpKeNamaPelanggan.ContainsKey(hpNorm) Then
            namaHasil = _mapNoTelpKeNamaPelanggan(hpNorm)
            Return True
        End If

        Return False
    End Function

    Private Sub SetPelangganDariNama(nama As String)
        If String.IsNullOrWhiteSpace(nama) Then Exit Sub

        SkipValidation = True
        Try
            Dim idx As Integer = CmbPelanggan.FindStringExact(nama)
            If idx >= 0 Then
                CmbPelanggan.SelectedIndex = idx
            Else
                CmbPelanggan.Text = nama
            End If
        Finally
            SkipValidation = False
        End Try

        AmbilInformasiPelanggan()
    End Sub


    Private Sub ComboBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        If SkipValidation Then Exit Sub ' cegah loop error

        Dim comboBox As ComboBox = CType(sender, ComboBox)

        ' Jika kosong (""), anggap valid
        If comboBox.Text = "" Then Exit Sub

        Dim namaFinal As String = ""
        If CobaKonversiInputPelangganKeNama(comboBox.Text, namaFinal) Then
            If comboBox Is CmbPelanggan Then
                SetPelangganDariNama(namaFinal)
            End If
            Exit Sub
        End If

        ' Jika nilai tidak ada dalam daftar → beri peringatan
        If Not comboBox.Items.Contains(comboBox.Text) Then
            MessageBox.Show("Harap pilih nama pelanggan yang valid dari daftar.",
                    "Pilihan pelanggan Tidak Valid",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)
            e.Cancel = True
        End If
    End Sub

    Private Sub DTPTgl_ValueChanged(sender As Object, e As EventArgs) Handles DTPTgl.ValueChanged
        If IsModeTambahSO Then
            NomorSO()
        End If
    End Sub

    Public Sub NomorSO()

        ' Pakai sp_hlp_faktur_generate — aman multi-user (FOR UPDATE), format konsisten
        Using cmd As New MySqlCommand(
        "CALL sp_hlp_faktur_generate(@prefix, @tgl, 'sales_order', 'ID_PENJUALAN', @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "SO")
            cmd.Parameters.AddWithValue("@tgl", DTPTgl.Value.Date)
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            TxtFaktur.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Public Sub AmbilDataKaryawan()
        CmbSales.Items.Clear()
        CmbSales.Items.Add("")
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan WHERE Status = 'Aktif' ORDER BY Nama ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()

                        CmbSales.Items.Add(rd("Nama").ToString())
                    End While
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbSales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSales.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi sales
        AmbilInformasiSales()
    End Sub

    Private Sub CmbSales_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSales.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            AmbilInformasiSales()
            SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub AmbilInformasiSales()
        If String.IsNullOrEmpty(CmbSales.Text.Trim()) Then
            LblSales.Text = ""
            Exit Sub
        End If

        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama " &
                            "UNION " &
                            "SELECT kode_user AS Kode FROM tbl_user WHERE nama_user = @Nama OR user_name = @Nama " &
                            "LIMIT 1"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbSales.Text.Trim())
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblSales.Text = reader("Kode").ToString()
                Else
                    LblSales.Text = ""
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbPelanggan_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbPelanggan.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi pelanggan
        Dim _t As DateTime = DateTime.Now
        AmbilInformasiPelanggan()
    End Sub

    Private Sub CmbPelanggan_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbPelanggan.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            Dim namaFinal As String = ""
            If CobaKonversiInputPelangganKeNama(CmbPelanggan.Text, namaFinal) Then
                SetPelangganDariNama(namaFinal)
            End If
            SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub AmbilInformasiPelanggan()
        ' Deklarasi variabel untuk menyimpan nilai
        Dim jenisPelanggan As String = "Umum"
        Dim kodePelanggan As String = ""
        Dim alamatPelanggan As String = ""
        Dim jangkaPiutang As Integer = 0

        ' Mengambil informasi pelanggan dari database
        Using cmd As New MySqlCommand("SELECT KODE, ALAMAT, JENIS, JangkaPiutang FROM tbl_pelanggan WHERE nama = @nama", conn)
            cmd.Parameters.AddWithValue("@nama", CmbPelanggan.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Simpan nilai dari database ke variabel
                    jenisPelanggan = ModuleAngka.SafeGetValue(Of String)(rd, "JENIS", "Umum")
                    kodePelanggan = ModuleAngka.SafeGetValue(Of String)(rd, "KODE", "")
                    alamatPelanggan = ModuleAngka.SafeGetValue(Of String)(rd, "ALAMAT", "")
                End If
            End Using
        End Using

        ' Masukkan nilai ke label di luar blok reader
        LblJenisPl.Text = jenisPelanggan
        LbLKodePel.Text = kodePelanggan
        LblAlamat.Text = alamatPelanggan

    End Sub

    Private Sub LblJenisPl_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblJenisPl.TextChanged
        ' Memperbarui data setelah pelanggan dipilih
        Dim _t As DateTime = DateTime.Now
        UpdateHargaBerdasarJenisPelanggan()
    End Sub

    Private Sub UpdateHargaBerdasarJenisPelanggan()
        ' ✅ BATCH 4 OPTIMIZATION: Batch query daripada per-item queries
        ' MASALAH LAMA: 100 item = 100 queries → 3-5 detik
        ' SOLUSI BARU: 1 batch query → 300-500ms (90% lebih cepat!)

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 1: Kumpulkan semua kode barang dari grid
        ' ═══════════════════════════════════════════════════════════════════
        Dim daftarKodeBarang As New List(Of String)()

        For Each dgvRow As DataGridViewRow In DgvDataTransaksi.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                daftarKodeBarang.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        ' Jika tidak ada barang, langsung selesai
        If daftarKodeBarang.Count = 0 Then
            Hitungbaris()
            UpdateSemuaTotal()
            SetupFocusToGrid()
            Return
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 2: ✅ SINGLE BATCH QUERY untuk semua barang (bukan per-item!)
        ' ═══════════════════════════════════════════════════════════════════
        Dim inClause As String = String.Join(",", daftarKodeBarang.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim queryBatch As String = "SELECT ID_BARANG, HARGA_BELI, " &
                               "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                               "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                               "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                               "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, " &
                               "STOK_TOKO, STOK_GUDANG " &
                               "FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        ' Simpan hasil batch query di Dictionary untuk akses cepat
        Dim dictBarang As New Dictionary(Of String, BarangHargaInfo)

        Try
            Using cmd As New MySqlCommand(queryBatch, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim idBarang As String = rd("ID_BARANG").ToString()

                        ' ✅ Simpan info barang di Dictionary
                        dictBarang(idBarang) = New BarangHargaInfo() With {
                        .HargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D),
                        .SatuanUmumKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                        .SatuanUmumSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                        .SatuanUmumBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", ""),
                        .HargaJualUmumKecil = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D),
                        .HargaJualUmumSedang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_SEDANG", 0D),
                        .HargaJualUmumBesar = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_BESAR", 0D),
                        .SatuanPartaiKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", ""),
                        .SatuanPartaiSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_SEDANG", ""),
                        .SatuanPartaiBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_BESAR", ""),
                        .HargaJualPartaiKecil = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D),
                        .HargaJualPartaiSedang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_SEDANG", 0D),
                        .HargaJualPartaiBesar = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_BESAR", 0D),
                        .StokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                        .StokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    }
                    End While
                End Using
            End Using
        Catch ex As Exception
            Hitungbaris()
            UpdateSemuaTotal()
            SetupFocusToGrid()
            Return
        End Try

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 3: Update grid OFFLINE (tanpa database queries!)
        ' ═══════════════════════════════════════════════════════════════════
        ' Stabilkan DGV dulu — EndEdit + flag agar CellEndEdit tidak terpicu
        _sedangSetNilaiDariListBox = True
        DgvDataTransaksi.EndEdit(True)
        DgvDataTransaksi.CurrentCell = Nothing

        For Each dgvRow As DataGridViewRow In DgvDataTransaksi.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                ' Ambil info dari dictionary (O(1) lookup, no query!)
                If dictBarang.ContainsKey(kodeBarangValue) Then
                    Dim infoBarang As BarangHargaInfo = dictBarang(kodeBarangValue)

                    ' Update harga beli
                    dgvRow.Cells("HargaBeli").Value = infoBarang.HargaBeli

                    ' Update harga jual berdasarkan satuan yang dipilih
                    ' Jika auto level aktif: tentukan level dari qty, lalu pilih harga sesuai level
                    ' Jika tidak: cocokkan dari nama satuan yang sudah ada di cell
                    If Not IsDBNull(dgvRow.Cells("Satuan").Value) Then
                        Dim nilaiSatuan As String = dgvRow.Cells("Satuan").Value.ToString()
                        Dim hargaValue As Decimal = 0D
                        Dim qtyBaris As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("QTY").Value)
                        Dim levelBaris As Integer = TentukanLevelDariQty(qtyBaris) ' 0 jika fitur nonaktif

                        If LblJenisPl.Text = "Partai" Then
                            If levelBaris > 0 Then
                                ' Auto level dari qty
                                Select Case levelBaris
                                    Case 3 : hargaValue = infoBarang.HargaJualPartaiBesar
                                        If Not String.IsNullOrEmpty(infoBarang.SatuanPartaiBesar) Then dgvRow.Cells("Satuan").Value = infoBarang.SatuanPartaiBesar
                                    Case 2 : hargaValue = infoBarang.HargaJualPartaiSedang
                                        If Not String.IsNullOrEmpty(infoBarang.SatuanPartaiSedang) Then dgvRow.Cells("Satuan").Value = infoBarang.SatuanPartaiSedang
                                    Case Else : hargaValue = infoBarang.HargaJualPartaiKecil
                                        If Not String.IsNullOrEmpty(infoBarang.SatuanPartaiKecil) Then dgvRow.Cells("Satuan").Value = infoBarang.SatuanPartaiKecil
                                End Select
                            Else
                                ' Cocokkan dari nama satuan
                                If nilaiSatuan = infoBarang.SatuanPartaiSedang Then
                                    hargaValue = infoBarang.HargaJualPartaiSedang
                                ElseIf nilaiSatuan = infoBarang.SatuanPartaiBesar Then
                                    hargaValue = infoBarang.HargaJualPartaiBesar
                                Else
                                    hargaValue = infoBarang.HargaJualPartaiKecil
                                End If
                            End If
                        Else
                            If levelBaris > 0 Then
                                ' Auto level dari qty
                                Select Case levelBaris
                                    Case 3 : hargaValue = infoBarang.HargaJualUmumBesar
                                        If Not String.IsNullOrEmpty(infoBarang.SatuanUmumBesar) Then dgvRow.Cells("Satuan").Value = infoBarang.SatuanUmumBesar
                                    Case 2 : hargaValue = infoBarang.HargaJualUmumSedang
                                        If Not String.IsNullOrEmpty(infoBarang.SatuanUmumSedang) Then dgvRow.Cells("Satuan").Value = infoBarang.SatuanUmumSedang
                                    Case Else : hargaValue = infoBarang.HargaJualUmumKecil
                                        If Not String.IsNullOrEmpty(infoBarang.SatuanUmumKecil) Then dgvRow.Cells("Satuan").Value = infoBarang.SatuanUmumKecil
                                End Select
                            Else
                                ' Cocokkan dari nama satuan
                                If nilaiSatuan = infoBarang.SatuanUmumSedang Then
                                    hargaValue = infoBarang.HargaJualUmumSedang
                                ElseIf nilaiSatuan = infoBarang.SatuanUmumBesar Then
                                    hargaValue = infoBarang.HargaJualUmumBesar
                                Else
                                    hargaValue = infoBarang.HargaJualUmumKecil
                                End If
                            End If
                        End If

                        dgvRow.Cells("Harga").Value = hargaValue
                    End If

                    ' Update stok
                    dgvRow.Cells("StokToko").Value = infoBarang.StokToko
                    dgvRow.Cells("StokGudang").Value = infoBarang.StokGudang

                    Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                              infoBarang.StokGudang,
                                              infoBarang.StokToko)
                    dgvRow.Cells("Stok").Value = stokValue
                End If
            End If
        Next

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 4: Hitung ulang total dan refresh display
        ' ═══════════════════════════════════════════════════════════════════
        _sedangSetNilaiDariListBox = False
        Hitungbaris()
        UpdateSemuaTotal()
        ' Kembalikan fokus ke grid setelah update harga selesai
        ' CurrentCell di-set Nothing di LANGKAH 3 — perlu dikembalikan
        SetupFocusToGrid()
    End Sub

    ' ===== BARCODE DETECTION - HYBRID SUPPORT =====
    Private isBarcodeMode As Boolean = False
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private barcodeTimer As New System.Windows.Forms.Timer()

    ' ── Debounce timer untuk pencarian manual ─────────────────────────────
    ' Menunda query ke DB sampai user berhenti ketik 250ms — cegah query per keystroke
    Private _searchTimer As New System.Windows.Forms.Timer() With {.Interval = 100}
    Private _searchKeywordPending As String = ""
    Private _searchKonteksPending As String = ""

    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100

    ' ===== DGV INLINE EDIT CONTEXT TRACKING =====
    Private _dgvEditingTextBox As TextBox = Nothing
    ' [F3-T04-1] HAPUS: Flag state navigasi ListView kompleks - tidak diperlukan untuk ListBox
    ' Alasan: ListBox tidak memerlukan flag state kompleks untuk navigasi keyboard
    ' Private _rowSaatPindahKeLst As Integer = -1  ' DIHAPUS
    ' Private _lstBarangSelectedIndex As Integer = -1  ' DIHAPUS
    ' Private _lstBarangBaruMasuk As Boolean = False  ' DIHAPUS
    ' [DEBUG FIX] TAMBAH KEMBALI: Flag sederhana untuk mencegah CellLeave menutup ListBox saat transisi
    Private _sedangPindahKeLstBarang As Boolean = False ' Guard transisi fokus ke ListBox
    Private _konteksLstBarang As String = "TXTNAMA" ' "TXTNAMA" atau "DGV" - PERTAHANKAN (logic bisnis)
    Private _sedangSetNilaiDariListBox As Boolean = False ' blok TextChanged saat set nilai programatically - PERTAHANKAN (logic bisnis)
    ' Simpan teks yang diketik user sebelum pindah ke ListBox.
    ' Dipakai untuk restore teks saat user tekan Up (kembali ke TextBox untuk refine search).
    ' Di-reset ke "" setelah dipakai agar tidak mengganggu sesi berikutnya.
    Private _teksSebelumPindahKeLstBarang As String = ""
    ' Simpan posisi sel DGV saat ListBox dibuka — untuk CellLeave guard.
    ' CellLeave hanya menutup ListBox jika sel yang ditinggalkan BERBEDA dari sel ini.
    Private _listBoxDibukaDiRow As Integer = -1
    Private _listBoxDibukaDiCol As Integer = -1

    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        ' [F3-T04-2] HAPUS: Navigasi Down arrow ke ListView - tidak diperlukan untuk ListBox
        ' Alasan: User akan menggunakan mouse click untuk memilih dari ListBox
        ' Logic navigasi keyboard sederhana akan ditambahkan di TASK-07b untuk Mode 2
        ' ===== SPECIAL KEYS =====
        ' If e.KeyCode = Keys.Down AndAlso LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
        '     _lstBarangSelectedIndex = 0
        '     _lstBarangBaruMasuk = True ' Flag: blok key Down bocor pertama
        '     LstBarang.FocusedItem = LstBarang.Items(0)
        '     LstBarang.SelectedItems.Clear()
        '     LstBarang.Items(0).Selected = True
        '     LstBarang.Items(0).Focused = True
        '     LstBarang.Items(0).EnsureVisible()
        '     LstBarang.Focus()
        '     LstBarang.BeginInvoke(New Action(Sub()
        '                                          If LstBarang.Items.Count > 0 Then
        '                                              LstBarang.SelectedItems.Clear()
        '                                              LstBarang.FocusedItem = LstBarang.Items(0)
        '                                              LstBarang.Items(0).Selected = True
        '                                              LstBarang.Items(0).Focused = True
        '                                          End If
        '                                      End Sub))
        '     e.SuppressKeyPress = True
        '     Return
        ' End If

        If e.KeyCode = Keys.Tab Then
            DgvDataTransaksi.Select()
            DgvDataTransaksi.Focus()
            e.SuppressKeyPress = True
            Return
        End If

        ' ===== PRINTABLE CHARACTERS =====
        Dim ch As Char = ChrW(e.KeyCode)
        If Not Char.IsControl(ch) Then
            ' Jika user mengetik huruf atau tanda '*' berarti input manual -> batalkan deteksi barcode
            If ch = "*"c OrElse Char.IsLetter(ch) Then
                ' Pastikan tidak ada proses barcode yang berjalan
                ResetBarcodeDetection()
                ' Biarkan event normal (teks masuk ke TxtNama)
                Return
            End If

            Dim currentTime = DateTime.Now

            ' Karakter pertama
            If barcodeChars.Count = 0 Then
                barcodeStartTime = currentTime
                barcodeChars.Add(ch)
                lastKeyTime = currentTime

                barcodeTimer.Interval = 100
                barcodeTimer.Stop()
                barcodeTimer.Start()
                Return
            End If

            ' Hitung interval dari karakter sebelumnya
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds

            ' Jika interval > threshold = MANUAL input
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then
                isBarcodeMode = False
            End If

            ' Tambah karakter
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

            ' Proses input sesuai tipe
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

        ' Tampilkan search list HANYA untuk input manual
        ' (ada huruf, atau format qty*... tanpa barcode)
        If currentText.Any(AddressOf Char.IsLetter) Then
            TriggerManualSearch(currentText)
        ElseIf currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            ' Cek bagian terakhir (keyword) punya huruf
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
                ' Jika buffer mengandung '*' atau huruf → input manual bertempo cepat
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    If _konteksLstBarang = "DGV" Then
                        ' Jalur DGV — manual search sudah ditangani TextChanged, tidak perlu ulang
                        ResetBarcodeDetection()
                    Else
                        TriggerManualSearch(bufferText)
                        ResetBarcodeDetection()
                    End If
                    Return
                End If

                ' Murni numerik/alphanumeric → kandidat barcode
                If _konteksLstBarang = "DGV" Then
                    ' Jalur DGV — proses barcode langsung ke baris DGV tanpa BeginEdit ulang
                    Dim barisDiisi As Integer = If(DgvDataTransaksi.CurrentCell IsNot Nothing, DgvDataTransaksi.CurrentCell.RowIndex, -1)
                    If barisDiisi >= 0 Then
                        Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
                        If qtyValue <= 0 Then qtyValue = 1D
                        ' Koreksi barisDiisi — cari baris kosong non-IsNewRow pertama
                        For i As Integer = 0 To DgvDataTransaksi.Rows.Count - 1
                            If Not DgvDataTransaksi.Rows(i).IsNewRow Then
                                Dim kodeVal = Convert.ToString(DgvDataTransaksi.Rows(i).Cells("Kode").Value).Trim()
                                Dim namaVal = Convert.ToString(DgvDataTransaksi.Rows(i).Cells("NamaBarang").Value).Trim()
                                If String.IsNullOrEmpty(kodeVal) AndAlso String.IsNullOrEmpty(namaVal) Then
                                    barisDiisi = i
                                    Exit For
                                End If
                            End If
                        Next
                        ' Set flag, EndEdit, isi baris — tidak perlu BeginEdit ulang
                        _sedangSetNilaiDariListBox = True
                        DgvDataTransaksi.EndEdit(True)
                        DgvDataTransaksi.CurrentCell = Nothing
                        ' Cari nama barang dari barcode
                        Dim namaBarang As String = ""
                        Try
                            Using cmd As New MySqlCommand(
                                "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND " &
                                "(BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc) LIMIT 1", conn)
                                cmd.Parameters.AddWithValue("@bc", bufferText)
                                Dim result = cmd.ExecuteScalar()
                                If result IsNot Nothing Then namaBarang = result.ToString()
                            End Using
                        Catch
                        End Try
                        If Not String.IsNullOrEmpty(namaBarang) Then
                            TxtBarcode.Text = bufferText
                            IsiBarangKeRow(barisDiisi, namaBarang, qtyValue, barcodeInput:=bufferText)
                            ' Fokus ke IsNewRow berikutnya — set CurrentCell saja, TANPA BeginEdit
                            ' Scanner akan trigger EditingControlShowing saat menembak karakter pertama
                            Dim nextRow As Integer = -1
                            For i As Integer = 0 To DgvDataTransaksi.Rows.Count - 1
                                If DgvDataTransaksi.Rows(i).IsNewRow Then nextRow = i : Exit For
                            Next
                            If nextRow >= 0 Then
                                DgvDataTransaksi.CurrentCell = DgvDataTransaksi(1, nextRow)
                                Me.ActiveControl = DgvDataTransaksi
                            End If
                        Else
                            MessageBox.Show("Barcode '" & bufferText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                        _sedangSetNilaiDariListBox = False
                    End If
                    ResetBarcodeDetection()
                Else
                    ' Jalur TxtNama — alur lama
                    ProcessInput(bufferText, (DateTime.Now - barcodeStartTime).TotalMilliseconds)
                    ResetBarcodeDetection()
                End If
            End If
        End If
    End Sub


    ' ===== BARCODE DETECTION - HELPER FUNCTIONS =====

    ''' <summary>
    ''' Tentukan apakah input adalah barcode candidate (rapid input + exists in DB)
    ''' ✅ Support: numeric (8991234567890), alphanumeric (ABC-123), mixed (P9K2L5)
    ''' </summary>
    Private Function IsBarcodeCandidate(input As String) As Boolean
        If input.Length < BARCODE_MIN_LENGTH Then Return False
        Return BarcodeExistsInDatabase(input)
    End Function

    ''' <summary>
    ''' Cek apakah barcode ada di salah satu kolom barcode
    ''' ✅ Support numeric, alphanumeric, dan mixed format
    ''' </summary>
    Private Function BarcodeExistsInDatabase(barcodeValue As String) As Boolean
        Const query = "SELECT 1 FROM tbl_barang " &
             "WHERE BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc LIMIT 1"
        Try

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@bc", barcodeValue)
                Dim result = cmd.ExecuteScalar()
                Return result IsNot Nothing
            End Using
        Catch ex As Exception
            Return False
        Finally
        End Try
    End Function

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

    ''' <summary>
    ''' Process input berdasarkan karakteristiknya
    ''' Support: qty*barcode, qty*satuan*nama, barcode murni, pencarian nama
    ''' ✅ Support barcode numeric, alphanumeric, dan mixed format
    ''' </summary>
    Private Sub ProcessInput(inputText As String, totalTimeMs As Double)
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
                If SearchByBarcode(secondPart) Then
                    Return
                Else
                    MessageBox.Show("Barcode '" & secondPart & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtNama.Clear()
                    Return
                End If
            End If

            ' Jika bukan scan, coba deteksi barcode di DB lalu fallback ke pencarian manual
            If IsBarcodeCandidate(secondPart) AndAlso SearchByBarcode(secondPart) Then
                Return
            End If

            ProcessManualSearchList(secondPart)
            TxtLevelSat.Text = "1"
            Return
        End If

        ' FORMAT 3: Barcode atau manual murni (no asterisk)
        If Not inputText.Contains("*") Then
            ' Jika input cepat dan panjangnya memenuhi syarat → anggap scan walau tidak ada di DB sebelum
            If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso inputText.Length >= BARCODE_MIN_LENGTH Then
                If SearchByBarcode(inputText) Then
                    Return
                Else
                    MessageBox.Show("Barcode '" & inputText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtNama.Clear()
                    Return
                End If
            End If

            ' Bukan scan cepat → normal flow: jika kandidat barcode dan ditemukan, proses; jika tidak, manual search
            If IsBarcodeCandidate(inputText) AndAlso SearchByBarcode(inputText) Then
                Return
            End If

            SetDefaultQtyAndSatuan()
            ProcessManualSearchList(inputText)
            Return
        End If
    End Sub
    ''' <summary>
    ''' Cari barcode di database dengan EXACT MATCH
    ''' Return True jika ditemukan, False jika tidak
    ''' DEFAULT QTY = 1 saat barcode ditemukan (jika belum ada qty)
    ''' </summary>
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
            TxtBarcode.Text = barcodeText
            TxtNama.Text = ""
            LstBarang.Visible = False

            ' Pastikan qty minimal 1 (jika kosong atau invalid)
            If ModuleAngka.ParseDecimal(TxtQty.Text) <= 0 Then
                TxtQty.Text = "1"
            End If

            ' Pastikan TxtLevelSat minimal 1
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                TxtLevelSat.Text = "1"
            End If

            Ambildatalaindaridbbarang(namaBarang)
            Return True
        End If

        Return False
    End Function

    Private Sub TriggerManualSearch(keyword As String)
        ' Pastikan hentikan deteksi barcode agar tidak menutup ListBox
        ResetBarcodeDetection()
        _konteksLstBarang = "TXTNAMA"

        ' Parse jika ada tanda *
        If keyword.Contains("*") Then
            Dim parts = keyword.Split("*"c)
            If parts.Length >= 2 Then
                keyword = parts.Last().Trim()
            End If
        End If

        If keyword.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            ' Ensure default qty = 1
            If String.IsNullOrEmpty(TxtQty.Text) Then
                TxtQty.Text = "1"
            End If
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                TxtLevelSat.Text = "1"
            End If
            Return
        End If

        ' ── Debounce: tunda query sampai user berhenti ketik 250ms ────────
        _searchKeywordPending = keyword
        _searchKonteksPending = "TXTNAMA"
        _searchTimer.Stop()
        _searchTimer.Start()
    End Sub

    ''' <summary>
    ''' Dipanggil oleh _searchTimer setelah debounce 250ms.
    ''' Baru jalankan query ke DB saat user berhenti ketik.
    ''' </summary>
    Private Sub SearchTimer_Tick(sender As Object, e As EventArgs)
        _searchTimer.Stop()
        If Not String.IsNullOrEmpty(_searchKeywordPending) Then
            SearchBarangToListBox(_searchKeywordPending, _searchKonteksPending)
        End If
    End Sub

    ''' <summary>
    ''' Metode pencarian umum untuk mengisi ListBox dengan hasil pencarian barang
    ''' </summary>
    ''' <param name="searchKeyword">Keyword pencarian</param>
    ''' <param name="konteks">"TXTNAMA" untuk pencarian dari TxtNama, "DGV" untuk pencarian dari DataGridView</param>
    Private Sub SearchBarangToListBox(searchKeyword As String, konteks As String)
        searchKeyword = searchKeyword.Trim()

        ' Validasi min 2 karakter
        If searchKeyword.Length < 2 AndAlso Not searchKeyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Untuk konteks DGV, validasi harus ada huruf
        If konteks = "DGV" AndAlso Not searchKeyword.Any(AddressOf Char.IsLetter) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' [F4-T10-1] UBAH: Query dinamis berdasarkan SettingTampilInfoStok
        ' Alasan: Optimasi database - hanya ambil kolom stok jika diperlukan
        Dim query As String
        If ModulHakAkses.SettingTampilInfoStok Then
            query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                    "WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @key ORDER BY NAMA_BARANG LIMIT 200"
        Else
            query = "SELECT NAMA_BARANG FROM tbl_barang " &
                    "WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @key ORDER BY NAMA_BARANG LIMIT 200"
        End If

        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@key", "%" & searchKeyword & "%")

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    LstBarang.Items.Clear()

                    ' [F4-T10-2] UBAH: Populate ListBox dengan format string
                    ' Format: "Nama Barang | T: {stokToko} | G: {stokGudang}" jika stok display enabled
                    '         "Nama Barang" jika stok display disabled
                    While rd.Read()
                        Dim namaBarang = rd("NAMA_BARANG").ToString()
                        Dim displayString As String

                        If ModulHakAkses.SettingTampilInfoStok Then
                            Dim stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                            Dim stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                            displayString = String.Format("{0} | T: {1} | G: {2}", namaBarang, stokToko.ToString("N0"), stokGudang.ToString("N0"))
                        Else
                            displayString = namaBarang
                        End If

                        LstBarang.Items.Add(displayString)
                    End While

                    ' Tampilkan ListBox jika ada hasil
                    If LstBarang.Items.Count > 0 Then
                        If konteks = "TXTNAMA" Then
                            PosisikanLstBarangDiBawahTxtNama()
                        ElseIf konteks = "DGV" Then
                            PosisikanLstBarangDiBawahSel()
                            LstBarang.BringToFront()
                        End If
                        ' Simpan posisi sel saat ListBox dibuka — untuk guard CellLeave
                        If konteks = "DGV" AndAlso DgvDataTransaksi.CurrentCell IsNot Nothing Then
                            _listBoxDibukaDiRow = DgvDataTransaksi.CurrentCell.RowIndex
                            _listBoxDibukaDiCol = DgvDataTransaksi.CurrentCell.ColumnIndex
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

                    ' Set default qty = 1 hanya untuk konteks TXTNAMA
                    If konteks = "TXTNAMA" Then
                        If String.IsNullOrEmpty(TxtQty.Text) Then
                            TxtQty.Text = "1"
                        End If
                        If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                            TxtLevelSat.Text = "1"
                        End If
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error search: " & ex.Message)
        End Try
    End Sub

    Private Sub ProcessManualSearchList(searchKeyword As String)
        SearchBarangToListBox(searchKeyword, "TXTNAMA")
    End Sub

    ''' <summary>Reset semua state deteksi barcode di jalur TxtNama.</summary>
    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False
        barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue
        lastKeyTime = DateTime.MinValue
        barcodeTimer.Stop()
    End Sub

    ' [F3-T05-3] HAPUS: Event handler LstBarang_SizeChanged - tidak diperlukan untuk ListBox
    ' Alasan: ListBox tidak memiliki Columns seperti ListView, menggunakan format string
    ' Private Sub LstBarang_SizeChanged(sender As Object, e As EventArgs) Handles LstBarang.SizeChanged
    ' End Sub

    ' [F3-T07b-2] TAMBAH: Event handler LstBarang_KeyDown untuk navigasi keyboard (Mode 2)
    ' Fungsi: Handle Up arrow (kembali ke TextBox) dan Enter (pilih item) dan Escape (tutup)
    ' Alur: Up di item pertama → kembali ke TextBox untuk refine search
    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    _sedangPindahKeLstBarang = True
                    e.SuppressKeyPress = True
                    If _konteksLstBarang = "DGV" Then
                        Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                        _teksSebelumPindahKeLstBarang = ""
                        DgvDataTransaksi.Focus()
                        DgvDataTransaksi.BeginInvoke(New Action(Sub()
                                                                    If DgvDataTransaksi.CurrentCell IsNot Nothing Then
                                                                        DgvDataTransaksi.BeginEdit(True)
                                                                        Dim editCtrl = TryCast(DgvDataTransaksi.EditingControl, TextBox)
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
                    DgvDataTransaksi.Focus()
                    DgvDataTransaksi.BeginInvoke(New Action(Sub()
                                                                If DgvDataTransaksi.CurrentCell IsNot Nothing Then
                                                                    DgvDataTransaksi.BeginEdit(True)
                                                                    Dim editCtrl = TryCast(DgvDataTransaksi.EditingControl, TextBox)
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

    Private Sub LstBarang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LstBarang.SelectedIndexChanged
        ' Hanya tracking — pemilihan aktual via LstBarang_Click dan LstBarang_KeyDown (Enter)
        ' Event ini terpicu saat navigasi keyboard juga, bukan hanya saat memilih
    End Sub

    Private Sub LstBarang_Click(sender As Object, e As EventArgs) Handles LstBarang.Click
        If LstBarang.SelectedIndex >= 0 Then
            _sedangPindahKeLstBarang = True
            AmbilDataDariListBox()
            _sedangPindahKeLstBarang = False
        End If
    End Sub

    Private Sub AmbilDataDariListBox()
        ' [F4-T11-1] UBAH: Parse string dari ListBox untuk mendapatkan nama barang
        ' Alasan: ListBox menggunakan format string, bukan ListViewItem
        ' Format: "Nama Barang | T: {stokToko} | G: {stokGudang}" atau "Nama Barang"

        ' Reset teks tersimpan — user sudah memilih item, teks lama tidak relevan lagi
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

        ' [F4-T11-2] UBAH: Extract nama barang dari format string
        ' Split pada "|" untuk memisahkan nama barang dari info stok
        Dim namayangdiambil As String = selectedValue
        If selectedValue.Contains("|") Then
            Dim parts = selectedValue.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length > 0 Then
                namayangdiambil = parts(0).Trim()
            End If
        End If

        TutupListBox()

        ' Konteks DGV inline edit
        If _konteksLstBarang = "DGV" AndAlso
           DgvDataTransaksi.CurrentCell IsNot Nothing AndAlso DgvDataTransaksi.CurrentCell.ColumnIndex = 1 Then

            ' Baca qty dari TxtQty — diisi oleh DgvNamaBarang_TextChanged saat user ketik "5*nama"
            ' Konsisten dengan jalur TxtNama yang juga pakai TxtQty sebagai sumber qty
            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
            If qtyValue <= 0 Then qtyValue = 1D

            Dim barisDiisi As Integer = DgvDataTransaksi.CurrentCell.RowIndex

            ' Guard: koreksi barisDiisi jika CurrentCell sudah bergeser akibat BeginEdit pada IsNewRow
            ' (WinForms otomatis tambah baris kosong saat BeginEdit pada IsNewRow, menggeser CurrentCell)
            ' Cari baris dengan Kode kosong pertama — baris yang sedang diedit punya NamaBarang = keyword
            ' tapi Kode masih kosong, itu adalah target yang benar. Tidak boleh cek NamaBarang.
            Dim BarisCurrent As Integer = barisDiisi
            For i As Integer = 0 To DgvDataTransaksi.Rows.Count - 1
                If Not DgvDataTransaksi.Rows(i).IsNewRow Then
                    Dim kodeVal = Convert.ToString(DgvDataTransaksi.Rows(i).Cells("Kode").Value).Trim()
                    If String.IsNullOrEmpty(kodeVal) Then
                        barisDiisi = i
                        Exit For
                    End If
                End If
            Next

            ' Selesaikan edit mode dulu sebelum mengubah cell
            ' Flag tetap True sampai seluruh proses selesai — blok CellEndEdit agar tidak ikut proses
            _sedangSetNilaiDariListBox = True
            DgvDataTransaksi.EndEdit(True)
            DgvDataTransaksi.CurrentCell = Nothing

            ' CEK DUPLIKAT berdasarkan SettingIzinkanSatuanBerbeda untuk konteks DGV
            If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                ' Ambil ID_BARANG dari nama barang untuk cek duplikat
                Dim idBarangBaru As String = AmbilKodeBarangDariNama(namayangdiambil)
                If String.IsNullOrEmpty(idBarangBaru) Then
                    ' Jika tidak ditemukan, lanjutkan isi data biasa
                    Dim lvl As Integer = If(Integer.TryParse(TxtLevelSat.Text, lvl), lvl, 1)
                    IsiBarangKeRow(barisDiisi, namayangdiambil, qtyValue, level:=lvl)
                    _sedangSetNilaiDariListBox = False
                    SetupFocusToGrid()
                    Return
                End If

                ' Cek apakah barang yang sama sudah ada di baris lain
                For Each row As DataGridViewRow In DgvDataTransaksi.Rows
                    If row.Index <> barisDiisi AndAlso row.Cells("Kode").Value IsNot Nothing AndAlso row.Cells("Kode").Value.ToString() = idBarangBaru Then
                        Dim qtyLama As Decimal = If(IsDBNull(row.Cells("QTY").Value), 0D, ModuleAngka.ParseDecimal(row.Cells("QTY").Value))
                        Dim qtyBaru As Decimal = qtyValue
                        Dim qtyTotal As Decimal = qtyLama + qtyBaru

                        Dim isi As Integer = If(IsDBNull(row.Cells("Isi").Value), 0, Convert.ToInt32(row.Cells("Isi").Value))
                        If isi = 0 Then isi = 1

                        row.Cells("QTY").Value = qtyTotal
                        row.Cells("QtySat").Value = qtyTotal * isi

                        HitungNilaiSetiapBaris(row.Index)
                        UpdateSemuaTotal()

                        ' Hapus baris yang sedang diisi (karena sudah digabungkan)
                        If Not DgvDataTransaksi.Rows(barisDiisi).IsNewRow Then
                            DgvDataTransaksi.Rows.RemoveAt(barisDiisi)
                        End If

                        _sedangSetNilaiDariListBox = False
                        SetupFocusToGrid()
                        Return
                    End If
                Next
            End If

            ' Isi semua data langsung ke baris — tidak lewat CellEndEdit
            Dim levelAmbil As Integer = If(Integer.TryParse(TxtLevelSat.Text, levelAmbil), levelAmbil, 1)
            IsiBarangKeRow(barisDiisi, namayangdiambil, qtyValue, level:=levelAmbil)
            _sedangSetNilaiDariListBox = False

            ' Navigasi ke baris kosong berikutnya
            SetupFocusToGrid()
            Return
        End If
        ' Konteks TxtNamaBarang
        Dim originalInputTxt As String = TxtNama.Text.Trim()
        If originalInputTxt.Contains("*"c) Then
            Dim inputParts As String() = originalInputTxt.Split("*"c)
            If inputParts.Length >= 3 Then
                SetQtyAndSatuan(inputParts(0).Trim(), inputParts(1).Trim())
            ElseIf inputParts.Length = 2 Then
                SetQtyOnly(inputParts(0).Trim())
            End If
        End If
        Ambildatalaindaridbbarang(namayangdiambil)
    End Sub

    Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                    "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                    "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
                    "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
                    "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                    "FROM tbl_barang " &
                    "WHERE NAMA_BARANG = @NamaBarang OR BARCODE_KECIL = @NamaBarang OR BARCODE_SEDANG = @NamaBarang OR BARCODE_BESAR = @NamaBarang LIMIT 1"

        Dim idBarang As String = ""
        Dim hargaBeli As String = ""
        Dim satuan As String = ""
        Dim isiSatuan As Integer = 1
        Dim hargaJual As Decimal = 0

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", namayangdiambil)

            Try
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        ' ✅ BATCH 2: SafeGetValue untuk standardisasi DBNull handling
                        idBarang = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                        hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D).ToString()

                        ' Tentukan satuan, isi, dan harga jual berdasarkan jenis pelanggan
                        Dim jenisPelanggan As String = LblJenisPl.Text
                        Dim levelSatuan As Integer

                        If Not Integer.TryParse(TxtLevelSat.Text.Trim(), levelSatuan) Then
                            levelSatuan = 1
                        End If

                        Dim barcodeInput As String = TxtBarcode.Text

                        ' Loop untuk mencari satuan yang tersedia dari besar ke kecil
                        For level As Integer = 3 To 1 Step -1
                            ' Cek apakah barcode cocok atau level sesuai
                            Dim useThisLevel As Boolean = False

                            If Not String.IsNullOrEmpty(barcodeInput) Then
                                ' Cek berdasarkan barcode
                                If level = 2 AndAlso barcodeInput = rd("BARCODE_SEDANG").ToString() Then
                                    useThisLevel = True
                                ElseIf level = 3 AndAlso barcodeInput = rd("BARCODE_BESAR").ToString() Then
                                    useThisLevel = True
                                ElseIf level = 1 AndAlso barcodeInput = rd("BARCODE_KECIL").ToString() Then
                                    useThisLevel = True
                                End If
                            Else
                                ' Cek berdasarkan level
                                useThisLevel = (level = levelSatuan)
                            End If

                            If useThisLevel Then
                                If jenisPelanggan = "Partai" Then
                                    ' ✅ BATCH 2: SafeGetValue untuk standardisasi satuan & harga
                                    ' Cek satuan partai
                                    Select Case level
                                        Case 3
                                            satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_BESAR", "")
                                            isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_BESAR", 1)
                                            hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_BESAR", 0D)
                                        Case 2
                                            satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_SEDANG", "")
                                            isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_SEDANG", 1)
                                            hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_SEDANG", 0D)
                                        Case 1
                                            satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", "")
                                            isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_KECIL", 1)
                                            hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D)
                                    End Select
                                Else
                                    ' Cek satuan umum
                                    Select Case level
                                        Case 3
                                            satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                                            isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1)
                                            hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_BESAR", 0D)
                                        Case 2
                                            satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                                            isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1)
                                            hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_SEDANG", 0D)
                                        Case 1
                                            satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                            isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)
                                            hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D)
                                    End Select
                                End If

                                ' Jika satuan ditemukan dan tidak kosong, keluar dari loop
                                If Not String.IsNullOrEmpty(satuan) Then
                                    Exit For
                                End If
                            End If
                        Next

                        ' Jika setelah loop satuan masih kosong, gunakan satuan kecil
                        If String.IsNullOrEmpty(satuan) Then
                            If jenisPelanggan = "Partai" Then
                                satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", "")
                                isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_KECIL", 1)
                                hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D)
                            Else
                                satuan = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                isiSatuan = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)
                                hargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D)
                            End If
                        End If

                        ' Ambil stok
                        Dim stokToko As String = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D).ToString()
                        Dim stokGudang As String = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D).ToString()

                        TxtStokToko.Text = stokToko
                        TXtStokGudang.Text = stokGudang

                        If LblLokasiBarang.Text = "GUDANG" Then
                            TxtStok.Text = stokGudang
                        ElseIf LblLokasiBarang.Text = "TOKO" Then
                            TxtStok.Text = stokToko
                        End If
                    Else
                        ' DATA TIDAK DITEMUKAN
                        MessageBox.Show("Barang '" & namayangdiambil & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        KosongTxtboxcari()
                        TxtNama.Focus()
                        Return
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error membaca data barang: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        ' SETELAH READER DITUTUP, SET NILAI KE TEXTBOX
        TxtKode.Text = idBarang
        TxtHargaBeli.Text = hargaBeli
        Txtsatuan.Text = satuan
        TxtIsi.Text = isiSatuan.ToString()
        TxtHargaJual.Text = hargaJual.ToString()

        ' JANGAN TAMPILKAN LISTBOX - LANGSUNG TAMBAH KE DATAGRID
        LstBarang.Items.Clear()
        LstBarang.Visible = False

        ' Panggil TambahDataLangsung untuk menambah ke DataGridView
        TambahDataLangsung(namayangdiambil)
    End Sub


    ''' <summary>Isi semua kolom baris DGV langsung dari DB berdasarkan nama barang — tanpa lewat CellEndEdit.
    ''' Mendukung jenis pelanggan Umum/Partai.
    ''' level: 1=kecil (default), 2=sedang, 3=besar. barcodeInput: jika dari scan, level ditentukan otomatis dari barcode.</summary>
    Private Sub IsiBarangKeRow(rowIdx As Integer, namaBarang As String, qty As Decimal,
                               Optional level As Integer = 1, Optional barcodeInput As String = "")
        If rowIdx < 0 OrElse rowIdx >= DgvDataTransaksi.Rows.Count Then Return

        Dim idBarang As String = ""
        Dim hargaBeli As Decimal = 0D
        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D
        Dim barcodeKecil As String = "" : Dim barcodeSedang As String = "" : Dim barcodeBesar As String = ""
        ' Satuan Umum
        Dim satUmumKecil As String = "" : Dim isiUmumKecil As Integer = 1 : Dim HargaUmumKecil As Decimal = 0D
        Dim satUmumSedang As String = "" : Dim isiUmumSedang As Integer = 1 : Dim HargaUmumSedang As Decimal = 0D
        Dim satUmumBesar As String = "" : Dim isiUmumBesar As Integer = 1 : Dim HargaUmumBesar As Decimal = 0D
        ' Satuan Partai
        Dim satPartaiKecil As String = "" : Dim isiPartaiKecil As Integer = 1 : Dim HargaPartaiKecil As Decimal = 0D
        Dim satPartaiSedang As String = "" : Dim isiPartaiSedang As Integer = 1 : Dim HargaPartaiSedang As Decimal = 0D
        Dim satPartaiBesar As String = "" : Dim isiPartaiBesar As Integer = 1 : Dim HargaPartaiBesar As Decimal = 0D

        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, HARGA_BELI, STOK_TOKO, STOK_GUDANG, " &
                "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                "SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, HARGA_JUAL_UMUM_KECIL, " &
                "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, HARGA_JUAL_UMUM_SEDANG, " &
                "SATUAN_UMUM_BESAR, ISI_UMUM_BESAR, HARGA_JUAL_UMUM_BESAR, " &
                "SATUAN_PARTAI_KECIL, ISI_PARTAI_KECIL, HARGA_JUAL_PARTAI_KECIL, " &
                "SATUAN_PARTAI_SEDANG, ISI_PARTAI_SEDANG, HARGA_JUAL_PARTAI_SEDANG, " &
                "SATUAN_PARTAI_BESAR, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_BESAR " &
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
                    ' Umum
                    satUmumKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                    isiUmumKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                    HargaUmumKecil = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D)
                    satUmumSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                    isiUmumSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                    HargaUmumSedang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_SEDANG", 0D)
                    satUmumBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                    isiUmumBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                    HargaUmumBesar = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_BESAR", 0D)
                    ' Partai
                    satPartaiKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", "")
                    isiPartaiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_KECIL", 1))
                    HargaPartaiKecil = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D)
                    satPartaiSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_SEDANG", "")
                    isiPartaiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_SEDANG", 1))
                    HargaPartaiSedang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_SEDANG", 0D)
                    satPartaiBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_PARTAI_BESAR", "")
                    isiPartaiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_PARTAI_BESAR", 1))
                    HargaPartaiBesar = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_BESAR", 0D)
                End Using
            End Using
        Catch ex As Exception
            Return
        End Try

        ' ── Tentukan satuan dan harga berdasarkan jenis pelanggan + level + barcode ──
        Dim isPartai As Boolean = LblJenisPl.Text = "Partai"
        Dim options As New List(Of KeyValuePair(Of String, Integer))()
        Dim HargaJual As Decimal = 0D

        ' Tentukan level aktif — barcode lebih prioritas dari level manual
        Dim levelAktif As Integer = level
        If Not String.IsNullOrEmpty(barcodeInput) Then
            If barcodeInput = barcodeKecil Then
                levelAktif = 1
            ElseIf barcodeInput = barcodeSedang Then
                levelAktif = 2
            ElseIf barcodeInput = barcodeBesar Then
                levelAktif = 3
            End If
        ElseIf level = 1 Then
            ' Tidak ada barcode dan level masih default (1) — cek auto level dari qty
            Dim levelDariQty As Integer = TentukanLevelDariQty(qty)
            If levelDariQty > 0 Then levelAktif = levelDariQty
        End If

        ' Pilih satuan, isi, harga sesuai levelAktif dan jenis pelanggan
        Dim satuanAktif As String = ""
        Dim isiAktif As Integer = 1

        If isPartai Then
            Select Case levelAktif
                Case 3 : satuanAktif = satPartaiBesar : isiAktif = isiPartaiBesar : HargaJual = HargaPartaiBesar
                Case 2 : satuanAktif = satPartaiSedang : isiAktif = isiPartaiSedang : HargaJual = HargaPartaiSedang
                Case Else : satuanAktif = satPartaiKecil : isiAktif = isiPartaiKecil : HargaJual = HargaPartaiKecil
            End Select
            ' Fallback ke kecil jika level yang diminta kosong
            If String.IsNullOrWhiteSpace(satuanAktif) Then
                satuanAktif = satPartaiKecil : isiAktif = isiPartaiKecil : HargaJual = HargaPartaiKecil
            End If
            ' Isi options untuk combo box
            If Not String.IsNullOrWhiteSpace(satPartaiKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satPartaiKecil, isiPartaiKecil))
            If Not String.IsNullOrWhiteSpace(satPartaiSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satPartaiSedang, isiPartaiSedang))
            If Not String.IsNullOrWhiteSpace(satPartaiBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satPartaiBesar, isiPartaiBesar))
        Else
            Select Case levelAktif
                Case 3 : satuanAktif = satUmumBesar : isiAktif = isiUmumBesar : HargaJual = HargaUmumBesar
                Case 2 : satuanAktif = satUmumSedang : isiAktif = isiUmumSedang : HargaJual = HargaUmumSedang
                Case Else : satuanAktif = satUmumKecil : isiAktif = isiUmumKecil : HargaJual = HargaUmumKecil
            End Select
            ' Fallback ke kecil jika level yang diminta kosong
            If String.IsNullOrWhiteSpace(satuanAktif) Then
                satuanAktif = satUmumKecil : isiAktif = isiUmumKecil : HargaJual = HargaUmumKecil
            End If
            ' Isi options untuk combo box
            If Not String.IsNullOrWhiteSpace(satUmumKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satUmumKecil, isiUmumKecil))
            If Not String.IsNullOrWhiteSpace(satUmumSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satUmumSedang, isiUmumSedang))
            If Not String.IsNullOrWhiteSpace(satUmumBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satUmumBesar, isiUmumBesar))
        End If

        ' Fallback jika tidak ada satuan sama sekali
        If options.Count = 0 Then options.Add(New KeyValuePair(Of String, Integer)("PCS", 1))
        If String.IsNullOrWhiteSpace(satuanAktif) Then satuanAktif = options(0).Key : isiAktif = options(0).Value

        ' ── Isi baris DGV ─────────────────────────────────────────────────────────
        Dim row = DgvDataTransaksi.Rows(rowIdx)
        row.Cells("Kode").Value = idBarang
        row.Cells("NamaBarang").Value = namaBarang
        row.Cells("QTY").Value = qty
        row.Cells("HargaBeli").Value = hargaBeli
        row.Cells("Harga").Value = HargaJual
        row.Cells("StokToko").Value = stokToko
        row.Cells("StokGudang").Value = stokGudang

        Dim stokLokasi As Decimal = If(LblLokasiBarang.Text = "GUDANG", stokGudang, stokToko)
        row.Cells("Stok").Value = stokLokasi

        ' Setup satuan combo box — isi semua opsi, pilih satuan sesuai level aktif
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()
        For Each opt In options
            kolomSatuan.Items.Add(opt.Key)
        Next
        kolomSatuan.Value = satuanAktif
        row.Cells("Isi").Value = isiAktif

        ' Set read-only setelah diisi
        row.Cells("NamaBarang").ReadOnly = True
        row.Cells("NamaBarang").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
        row.Cells("NamaBarang").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

        HitungNilaiSetiapBaris(rowIdx)
        UpdateSemuaTotal()
    End Sub

    ''' <summary>Ambil ID_BARANG dari nama barang.</summary>
    Private Function AmbilKodeBarangDariNama(namaBarang As String) As String
        If String.IsNullOrWhiteSpace(namaBarang) Then Return ""
        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND NAMA_BARANG=@n LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                Else
                    Return ""
                End If
            End Using
        Catch
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Ambil info stok terkini dari DB via SP.
    ''' Mode tambah  → sp_hlp_stok_ambil       (stok DB apa adanya)
    ''' Mode edit    → sp_hlp_stok_ambil_edit   (stok DB + qty di faktur lama yang akan dikembalikan)
    ''' Ringan — hanya SELECT, aman dipanggil berkali-kali.
    ''' Untuk validasi sebelum simpan, tetap pakai sp_hlp_stok_validasi (ada FOR UPDATE).
    ''' </summary>
    Private Function AmbilInfoStok(kodeBarang As String, ByRef stokToko As Decimal, ByRef stokGudang As Decimal) As Boolean
        stokToko = 0D
        stokGudang = 0D
        If String.IsNullOrWhiteSpace(kodeBarang) Then Return False
        Try
            If IsModeTambahSO Then
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
                ' Mode edit — stok efektif = stok DB + qty di faktur lama yang akan dikembalikan
                Using cmd As New MySqlCommand(
                    "CALL sp_hlp_stok_ambil_edit(@kode, @faktur, @lokasi, @toko, @gudang, @nama)", conn)
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
    ''' Dipakai untuk: klik kanan "Refresh Stok", load edit/draft.
    ''' Tidak mengubah data transaksi — hanya update kolom StokToko/StokGudang/Stok.
    ''' </summary>
    Private Sub RefreshStokBaris(rowIdx As Integer)
        If rowIdx < 0 OrElse rowIdx >= DgvDataTransaksi.Rows.Count Then Return
        Dim row = DgvDataTransaksi.Rows(rowIdx)
        If row.IsNewRow Then Return
        Dim kode As String = Convert.ToString(row.Cells("Kode").Value).Trim()
        If String.IsNullOrEmpty(kode) Then Return

        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D
        If AmbilInfoStok(kode, stokToko, stokGudang) Then
            row.Cells("StokToko").Value = stokToko
            row.Cells("StokGudang").Value = stokGudang
            row.Cells("Stok").Value = If(LblLokasiBarang.Text = "GUDANG", stokGudang, stokToko)
        End If
    End Sub

    ''' <summary>
    ''' Refresh info stok semua baris DGV yang sudah terisi.
    ''' Dipakai untuk: load edit/draft agar stok selalu fresh.
    ''' </summary>
    Private Sub RefreshStokSemuaBaris()
        For i As Integer = 0 To DgvDataTransaksi.Rows.Count - 1
            RefreshStokBaris(i)
        Next
    End Sub

    Private Sub TambahDataLangsung(ByVal namayangdiambil As String)
        ' CEK DUPLIKAT - merge qty jika barang sudah ada (konsisten dengan DGV inline edit)
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
            For Each row As DataGridViewRow In DgvDataTransaksi.Rows
                If row.Cells("Kode").Value IsNot Nothing AndAlso row.Cells("Kode").Value.ToString() = TxtKode.Text Then
                    Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY").Value)
                    Dim qtyBaru As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
                    Dim qtyTotal As Decimal = qtyLama + qtyBaru

                    row.Cells("QTY").Value = qtyTotal
                    Dim isiAtas As Integer = Math.Max(1, CInt(ModuleAngka.ParseDecimal(row.Cells("Isi").Value)))
                    row.Cells("QtySat").Value = qtyTotal * isiAtas

                    ' Cek auto level dari qty total jika fitur aktif
                    If ModulHakAkses.SettingAutoLevelSatuan Then
                        Dim levelBaru As Integer = TentukanLevelDariQty(qtyTotal)
                        If levelBaru > 0 Then
                            UpdateLevelSatuanBaris(row.Index, levelBaru)
                        End If
                    End If

                    HitungNilaiSetiapBaris(row.Index)
                    UpdateSemuaTotal()
                    KosongTxtboxcari()
                    SetupFocusToGrid()
                    Return
                End If
            Next
        End If

        ' TAMBAH KE DATAGRID
        Dim indeksBaris As Integer
        If DgvDataTransaksi.SelectedCells.Count > 0 Then
            indeksBaris = DgvDataTransaksi.SelectedCells(0).RowIndex
            DgvDataTransaksi.Rows.Insert(indeksBaris, "")
        Else
            indeksBaris = DgvDataTransaksi.Rows.Add()
        End If

        ' BACA SATUAN DARI DATABASE DALAM BLOK TERPISAH
        Dim isPartai As Boolean = LblJenisPl.Text = "Partai"
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvDataTransaksi.Rows(indeksBaris).Cells("Satuan"), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()

        Dim querySatuanPartai As String = "SELECT DISTINCT SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG = @ID"
        Dim querySatuanUmum As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ID"

        Dim query As String = If(isPartai, querySatuanPartai, querySatuanUmum)

        ' BACA SATUAN
        Using cmdSatuan As New MySqlCommand(query, conn)
            cmdSatuan.Parameters.AddWithValue("@ID", TxtKode.Text)

            Try
                Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                    If rdSatuan.HasRows Then
                        While rdSatuan.Read()
                            Dim satuanKecil As String = If(rdSatuan(0) IsNot DBNull.Value, rdSatuan(0).ToString(), String.Empty)
                            Dim satuanSedang As String = If(rdSatuan(1) IsNot DBNull.Value, rdSatuan(1).ToString(), String.Empty)
                            Dim satuanBesar As String = If(rdSatuan(2) IsNot DBNull.Value, rdSatuan(2).ToString(), String.Empty)

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
            Catch ex As Exception
                MessageBox.Show("Error membaca satuan: " & ex.Message)
            End Try
        End Using

        ' ISI DATAGRID
        Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeli.Text)
        Dim qty As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
        Dim isi As Decimal = Math.Max(1, ModuleAngka.ParseDecimal(TxtIsi.Text))  ' tidak boleh < 1
        Dim hargajual As Decimal = ModuleAngka.ParseDecimal(TxtHargaJual.Text)
        Dim stoktoko As Decimal = ModuleAngka.ParseDecimal(TxtStokToko.Text)
        Dim stokgudang As Decimal = ModuleAngka.ParseDecimal(TXtStokGudang.Text)
        Dim stok As Decimal = ModuleAngka.ParseDecimal(TxtStok.Text)

        DgvDataTransaksi.Rows(indeksBaris).Cells("Kode").Value = TxtKode.Text
        DgvDataTransaksi.Rows(indeksBaris).Cells("NamaBarang").Value = namayangdiambil
        DgvDataTransaksi.Rows(indeksBaris).Cells("HargaBeli").Value = hargaBeli
        DgvDataTransaksi.Rows(indeksBaris).Cells("QTY").Value = qty

        If Txtsatuan.Text = "" Then
            DgvDataTransaksi.Rows(indeksBaris).Cells("Satuan").Value = If(kolomSatuan.Items.Count > 0, kolomSatuan.Items(0).ToString(), "")
        Else
            DgvDataTransaksi.Rows(indeksBaris).Cells("Satuan").Value = Txtsatuan.Text
        End If

        DgvDataTransaksi.Rows(indeksBaris).Cells("Isi").Value = isi
        DgvDataTransaksi.Rows(indeksBaris).Cells("Totalhargabeli").Value = hargaBeli * isi * qty
        DgvDataTransaksi.Rows(indeksBaris).Cells("Harga").Value = hargajual
        DgvDataTransaksi.Rows(indeksBaris).Cells("QtySat").Value = qty * isi
        DgvDataTransaksi.Rows(indeksBaris).Cells("DiskonPersen").Value = 0
        DgvDataTransaksi.Rows(indeksBaris).Cells("DiskonRp").Value = 0
        DgvDataTransaksi.Rows(indeksBaris).Cells("TotalDiskon").Value = 0
        DgvDataTransaksi.Rows(indeksBaris).Cells("TotalHarga").Value = hargajual * qty
        DgvDataTransaksi.Rows(indeksBaris).Cells("StokToko").Value = stoktoko
        DgvDataTransaksi.Rows(indeksBaris).Cells("StokGudang").Value = stokgudang
        DgvDataTransaksi.Rows(indeksBaris).Cells("Stok").Value = stok

        ' ✅ PERBAIKAN: Set read-only setelah data diisi
        DgvDataTransaksi.Rows(indeksBaris).Cells("NamaBarang").ReadOnly = True
        DgvDataTransaksi.Rows(indeksBaris).Cells("NamaBarang").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
        DgvDataTransaksi.Rows(indeksBaris).Cells("NamaBarang").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

        UpdateSemuaTotal()
        KosongTxtboxcari()

        SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
    End Sub

    Private Sub HitungNilaiSetiapBaris(ByVal indeksBaris As Integer)
        Dim row = DgvDataTransaksi.Rows(indeksBaris)

        Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
        Dim qtyBarang As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY").Value)
        Dim isiBarang As Integer = Math.Max(1, CInt(ModuleAngka.ParseDecimal(row.Cells("Isi").Value)))  ' guard ISI=0
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(row.Cells("Harga").Value)
        Dim diskonRp As Decimal = ModuleAngka.ParseDecimal(row.Cells("DiskonRp").Value)

        Dim totalDiskon As Decimal = qtyBarang * diskonRp
        Dim totalHargaBeli As Decimal = qtyBarang * hargaBeli * isiBarang
        Dim qtySat As Decimal = qtyBarang * isiBarang
        Dim totalHarga As Decimal = hargaJual * qtyBarang - totalDiskon

        row.Cells("TotalDiskon").Value = totalDiskon
        row.Cells("Totalhargabeli").Value = totalHargaBeli
        row.Cells("QtySat").Value = qtySat
        row.Cells("TotalHarga").Value = totalHarga
    End Sub

    ' Fungsi untuk mengonversi objek menjadi angka desimal

    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvDataTransaksi.CellEndEdit
        ' ---> SOLUSI BUG BARCODE: Bersihkan handler TextBox DGV setiap kali selesai edit sel <---
        If _dgvEditingTextBox IsNot Nothing Then
            RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
            RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
            _dgvEditingTextBox = Nothing
        End If
        ResetBarcodeDetection()

        ' Guard: jangan proses jika sedang diisi dari ListBox — IsiBarangKeRow yang akan mengisi
        If _sedangSetNilaiDariListBox Then
            Return
        End If

        If e.ColumnIndex = 1 Then 'nama
            '=======================================
            If Not String.IsNullOrEmpty(DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Value) Then
                Dim inputText As String = DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Value.ToString().Trim()
                Dim qtyValue As Decimal = 1
                Dim namaBarangValue As String = inputText

                ' Cek apakah ada tanda bintang
                Dim indexAsteriskQty As Integer = inputText.IndexOf("*")
                Dim indexAsteriskHarga As Integer = -1

                If indexAsteriskQty >= 0 Then
                    indexAsteriskHarga = inputText.IndexOf("*", indexAsteriskQty + 1)
                End If

                If indexAsteriskQty >= 0 AndAlso indexAsteriskHarga > indexAsteriskQty Then
                    ' Format: qty * level * namaBarang
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ModuleAngka.ParseDecimal(angkaQty)
                    namaBarangValue = inputText.Substring(indexAsteriskHarga + 1).Trim()

                ElseIf indexAsteriskQty >= 0 Then
                    ' Format: qty * namaBarang
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ModuleAngka.ParseDecimal(angkaQty)
                    namaBarangValue = inputText.Substring(indexAsteriskQty + 1).Trim()
                End If

                ' Update kembali ke datagrid kolom NamaBarang setelah parsing
                DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Value = namaBarangValue

                Dim prefix As String = If(LblJenisPl.Text = "Partai", "PARTAI", "UMUM")
                Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
                "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
                "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                "FROM tbl_barang " &
                "WHERE NAMA_BARANG = @NamaBarang OR BARCODE_KECIL = @NamaBarang OR BARCODE_SEDANG = @NamaBarang OR BARCODE_BESAR = @NamaBarang LIMIT 1"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@NamaBarang", namaBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.HasRows Then
                            rd.Read()

                            ' Guard: CurrentCell bisa Nothing setelah EndEdit
                            If DgvDataTransaksi.CurrentCell Is Nothing Then Return

                            Dim comboCell As DataGridViewComboBoxCell = CType(DgvDataTransaksi.Rows(e.RowIndex).Cells(Satuan.Index), DataGridViewComboBoxCell)
                            comboCell.Items.Clear()

                            ' Ambil satuan sesuai jenis pelanggan
                            Dim satuanKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_" & prefix & "_KECIL", "")
                            Dim satuanSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_" & prefix & "_SEDANG", "")
                            Dim satuanBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_" & prefix & "_BESAR", "")

                            If Not String.IsNullOrEmpty(satuanKecil) Then comboCell.Items.Add(satuanKecil)
                            If Not String.IsNullOrEmpty(satuanSedang) Then comboCell.Items.Add(satuanSedang)
                            If Not String.IsNullOrEmpty(satuanBesar) Then comboCell.Items.Add(satuanBesar)

                            ' Tentukan satuan, isi, harga berdasarkan barcode atau default kecil
                            Dim satuanValue As String = satuanKecil
                            Dim isiField As String = "ISI_" & prefix & "_KECIL"
                            Dim hargaField As String = "HARGA_JUAL_" & prefix & "_KECIL"

                            If namaBarangValue = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_SEDANG", "") Then
                                satuanValue = satuanSedang
                                isiField = "ISI_" & prefix & "_SEDANG"
                                hargaField = "HARGA_JUAL_" & prefix & "_SEDANG"
                            ElseIf namaBarangValue = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_BESAR", "") Then
                                satuanValue = satuanBesar
                                isiField = "ISI_" & prefix & "_BESAR"
                                hargaField = "HARGA_JUAL_" & prefix & "_BESAR"
                            ElseIf satuanValue = satuanKecil Then
                                ' Tidak ada barcode — cek auto level dari qty jika fitur aktif
                                ' Konsisten dengan IsiBarangKeRow: auto level hanya jika level masih default (kecil)
                                Dim levelDariQty As Integer = TentukanLevelDariQty(qtyValue)
                                If levelDariQty = 2 Then
                                    satuanValue = satuanSedang
                                    isiField = "ISI_" & prefix & "_SEDANG"
                                    hargaField = "HARGA_JUAL_" & prefix & "_SEDANG"
                                ElseIf levelDariQty = 3 Then
                                    satuanValue = satuanBesar
                                    isiField = "ISI_" & prefix & "_BESAR"
                                    hargaField = "HARGA_JUAL_" & prefix & "_BESAR"
                                End If
                            End If

                            ' Isi kolom DGV
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value = rd("ID_BARANG")
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Value = rd("NAMA_BARANG")
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("HargaBeli").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("Satuan").Value = satuanValue
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("Isi").Value = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, isiField, 1))
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, hargaField, 0D)
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("QTY").Value = qtyValue
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("StokToko").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("StokGudang").Value = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)

                            Dim stokLokasi As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D),
                                ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D))
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("Stok").Value = stokLokasi

                            ' Set read-only setelah diisi
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").ReadOnly = True
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

                        Else
                            ' Tidak ditemukan — kosongkan sel nama
                            DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Value = ""

                            ' Cleanup row + refocus hanya untuk input yang pola scan barcode.
                            ' Input manual (huruf / qty*nama) jangan dipaksa agar ListBox tetap bisa muncul.
                            Dim isScanLikeInput As Boolean =
                                _konteksLstBarang = "DGV" AndAlso
                                Not namaBarangValue.Any(AddressOf Char.IsLetter) AndAlso
                                Not namaBarangValue.Contains("*"c) AndAlso
                                namaBarangValue.Length >= BARCODE_MIN_LENGTH

                            If isScanLikeInput Then
                                ' Jika row hasil scan tidak punya Kode, jangan biarkan jadi row kosong permanen.
                                Dim kodeNow As String = ""
                                If DgvDataTransaksi.Columns.Contains("Kode") AndAlso
                                   DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value IsNot Nothing Then
                                    kodeNow = Convert.ToString(DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value).Trim()
                                End If

                                If String.IsNullOrEmpty(kodeNow) Then
                                    If Not DgvDataTransaksi.Rows(e.RowIndex).IsNewRow Then
                                        DgvDataTransaksi.Rows.RemoveAt(e.RowIndex)
                                    End If

                                    ' Kembalikan fokus ke IsNewRow yang tersedia, tanpa memaksa tambah row baru.
                                    For i As Integer = 0 To DgvDataTransaksi.Rows.Count - 1
                                        If DgvDataTransaksi.Rows(i).IsNewRow Then
                                            DgvDataTransaksi.CurrentCell = DgvDataTransaksi(1, i)
                                            Me.ActiveControl = DgvDataTransaksi
                                            ' Setelah fokus kembali ke IsNewRow, masuk lagi ke mode edit
                                            ' agar scanner berikutnya tetap langsung menembak ke sel NamaBarang.
                                            DgvDataTransaksi.BeginInvoke(New Action(Sub()
                                                                                        If DgvDataTransaksi.CurrentCell IsNot Nothing Then
                                                                                            DgvDataTransaksi.BeginEdit(True)
                                                                                            DgvDataTransaksi.EditingControl?.Focus()
                                                                                        End If
                                                                                    End Sub))
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                        End If ' rd.HasRows
                    End Using ' rd
                End Using ' cmd

                ' Cek duplikat jika tidak izinkan satuan berbeda — dilakukan di luar DataReader
                If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                    For barisatas As Integer = 0 To DgvDataTransaksi.RowCount - 1
                        For barisbawah As Integer = barisatas + 1 To DgvDataTransaksi.RowCount - 1
                            Dim kodeAtas As Object = DgvDataTransaksi.Rows(barisatas).Cells("Kode").Value
                            Dim kodeBawah As Object = DgvDataTransaksi.Rows(barisbawah).Cells("Kode").Value
                            If kodeAtas IsNot Nothing AndAlso kodeBawah IsNot Nothing AndAlso kodeBawah.Equals(kodeAtas) Then
                                Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(barisatas).Cells("QTY").Value)
                                Dim qtyBaru As Decimal = ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(barisbawah).Cells("QTY").Value)
                                Dim qtyTotal As Decimal = qtyLama + qtyBaru
                                DgvDataTransaksi.Rows(barisatas).Cells("QTY").Value = qtyTotal
                                Dim isiAtas As Integer = Math.Max(1, CInt(ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(barisatas).Cells("Isi").Value)))
                                DgvDataTransaksi.Rows(barisatas).Cells("QtySat").Value = qtyTotal * isiAtas

                                ' Cek auto level dari qty total jika fitur aktif
                                If ModulHakAkses.SettingAutoLevelSatuan Then
                                    Dim levelBaru As Integer = TentukanLevelDariQty(qtyTotal)
                                    If levelBaru > 0 Then
                                        UpdateLevelSatuanBaris(barisatas, levelBaru)
                                    End If
                                End If

                                If Not DgvDataTransaksi.Rows(barisbawah).IsNewRow Then
                                    DgvDataTransaksi.Rows.RemoveAt(barisbawah)
                                End If
                                HitungNilaiSetiapBaris(barisatas)
                                UpdateSemuaTotal()
                                SetupFocusToGrid()
                                Exit Sub
                            End If
                        Next
                    Next
                End If

                HitungNilaiSetiapBaris(e.RowIndex)
                UpdateSemuaTotal()
                SetupFocusToGrid()
            End If
        End If

        '========================== qty
        If e.ColumnIndex = 3 Then
            ' Guard: jangan proses jika baris kosong (kolom Kode belum terisi)
            ' Mencegah pesan validasi muncul saat user tab ke kolom QTY di baris baru
            Dim kodeVal As String = If(DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value IsNot Nothing,
                                       DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value.ToString().Trim(), "")
            If String.IsNullOrEmpty(kodeVal) Then Return

            ' Reset buffer barcode — nilai qty tidak boleh bocor ke deteksi barcode
            ResetBarcodeDetection()

            ' Baca qty dari cell — ParseDecimal handle Nothing, DBNull, koma, titik, format apapun
            Dim qtyParsed As Decimal = ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(e.RowIndex).Cells("QTY").Value)

            ' Validasi: qty harus > 0
            If qtyParsed <= 0 Then
                MessageBox.Show("Qty hanya boleh angka dan satu tanda koma atau titik.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                qtyParsed = 1D
            End If

            ' Simpan kembali sebagai Decimal murni — sesuai standar-input-angka.md
            DgvDataTransaksi.Rows(e.RowIndex).Cells("QTY").Value = qtyParsed

            ' ── Auto level satuan berdasarkan qty ─────────────────────────
            ' Jika fitur aktif di General Setting, satuan/isi/harga otomatis
            ' menyesuaikan level berdasarkan threshold yang dikonfigurasi.
            Dim levelBaru As Integer = TentukanLevelDariQty(qtyParsed)
            If levelBaru > 0 Then
                UpdateLevelSatuanBaris(e.RowIndex, levelBaru)
            End If

            HitungNilaiSetiapBaris(e.RowIndex)
        End If


        '========================== harga jual
        If e.ColumnIndex = 7 Then
            ' Guard: jangan proses jika baris kosong
            If String.IsNullOrEmpty(If(DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value IsNot Nothing,
                                       DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value.ToString().Trim(), "")) Then Return

            Dim hargaCellValue As String = DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value

            Dim harga As Decimal
            Dim diskonRp As Decimal = DgvDataTransaksi.Rows(e.RowIndex).Cells("DiskonRp").Value

            If Not Decimal.TryParse(hargaCellValue, harga) Then
                MessageBox.Show("Harga harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                harga = 0
            End If

            ' Hitung diskon persen
            Dim diskonPersen As Decimal = If(harga > 0, (diskonRp / harga) * 100, 0)
            DgvDataTransaksi.Rows(e.RowIndex).Cells("DiskonPersen").Value = diskonPersen

            HitungNilaiSetiapBaris(e.RowIndex)


            ' Jika harga jual diubah, maka akan muncul edit master barang
            If ModulHakAkses.SettingHargaJualOtomatisUpdateMaster Then
                Dim idBarangObj = DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value
                If idBarangObj IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(idBarangObj.ToString()) Then
                    Dim idBarang As String = idBarangObj.ToString()
                    Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value)
                    Dim satuan As String = DgvDataTransaksi.Rows(e.RowIndex).Cells("Satuan").Value.ToString()
                    Dim jenispelanggan As String = If(LblJenisPl.Text = "Partai", "Partai", "Umum")
                    ' Panggil fungsi untuk mengupdate harga jual di database
                    UpdateHargaJual(idBarang, hargaJual, satuan, jenispelanggan)
                End If
            End If

        End If


        '========================== diskonpersen
        If e.ColumnIndex = 9 Then
            ' Guard: jangan proses jika baris kosong
            If String.IsNullOrEmpty(If(DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value IsNot Nothing,
                                       DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value.ToString().Trim(), "")) Then Return

            Dim diskonPersenCellValue As String = DgvDataTransaksi.Rows(e.RowIndex).Cells("DiskonPersen").Value

            Dim harga As Decimal = If(IsDBNull(DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value), 0D, ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value))
            Dim diskonPersen As Decimal

            If Not Decimal.TryParse(diskonPersenCellValue, diskonPersen) Then
                MessageBox.Show("Diskon persen harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                diskonPersen = 0
            End If

            ' Hitung Diskon Rp
            Dim diskonRp As Decimal = harga * diskonPersen / 100
            DgvDataTransaksi.Rows(e.RowIndex).Cells("DiskonRp").Value = diskonRp

            HitungNilaiSetiapBaris(e.RowIndex)
        End If

        '========================== diskonrp
        If e.ColumnIndex = 10 Then
            Dim diskonRpCellValue As String = DgvDataTransaksi.Rows(e.RowIndex).Cells("DiskonRp").Value

            Dim harga As Decimal = If(IsDBNull(DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value), 0D, ModuleAngka.ParseDecimal(DgvDataTransaksi.Rows(e.RowIndex).Cells("Harga").Value))
            Dim diskonRp As Decimal

            If Not Decimal.TryParse(diskonRpCellValue, diskonRp) Then
                MessageBox.Show("Diskon Rp harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                diskonRp = 0
            End If


            ' Hitung diskon persen
            Dim diskonPersen As Decimal = If(harga > 0, (diskonRp / harga) * 100, 0)
            DgvDataTransaksi.Rows(e.RowIndex).Cells("DiskonPersen").Value = diskonPersen

            HitungNilaiSetiapBaris(e.RowIndex)
        End If


        If Not String.IsNullOrEmpty(DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value) Then
            UpdateSemuaTotal()
        End If


    End Sub

    Private Sub UpdateHargaJual(ByVal idBarang As String, ByVal hargaJual As Decimal, ByVal satuan As String, ByVal jenispelanggan As String)
        With TambahBarang
            .LblHeaderForm.Text = "EDIT HARGA JUAL DARI SALES ORDER"
            .GBInput1.Visible = False
            .GBInput4.Visible = False
            .GBInput.Enabled = False
            .GBInput5.Visible = False
            .PanelInfoRubahHarga.Visible = False
            .BtnTambahKategori.Visible = False
            .BtnTambahSupliyer.Visible = False
            .BtnTambahSatuan.Visible = False
            .CBManual.Visible = False
            .BtnBaru.Visible = False
            '.BackColor = Color.DarkCyan
            .Size = New Size(816, 705)
            TambahBarang.Tampilkategori()
            TambahBarang.TampilSatuan()
            TambahBarang.Tampilsupliyer()
            .TxtKode.Text = idBarang
            .LblHargaDrJual.Text = hargaJual
            .LblsatuanDrJual.Text = satuan
            .LblJenisDrJual.Text = jenispelanggan
            .ShowDialog()
        End With
    End Sub


    Private Sub DgvData_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DgvDataTransaksi.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvDataTransaksi.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 8, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DgvData_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles DgvDataTransaksi.EditingControlShowing
        If DgvDataTransaksi.CurrentCell.ColumnIndex = 1 AndAlso DgvDataTransaksi.Columns(1).HeaderText = "Nama Barang" Then
            ' Jika sedang dalam proses pindah fokus ke ListBox, jangan re-attach handler.
            ' DGV BeginEdit ulang karena fokus kembali saat LstBarang.Focus() belum berhasil.
            ' Biarkan handler lama tetap aktif — TextBox editing control masih sama.
            If _sedangPindahKeLstBarang Then
                Return
            End If
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.None
                If _dgvEditingTextBox IsNot Nothing Then
                    RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                End If
                ' ✅ FIX: Reset buffer barcode setiap kali masuk ke sel NamaBarang baru.
                ' Mencegah nilai dari sel lain (QTY, dll) bocor ke deteksi barcode.
                ResetBarcodeDetection()
                _dgvEditingTextBox = autoText
                AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                PosisikanLstBarangDiBawahSel()
            End If
        Else
            ' [F3-T08b-1] HAPUS: Check LstBarang.Focused - tidak diperlukan untuk ListBox
            ' Alasan: ListBox akan ditutup oleh DgvData_CellLeave saat user pindah sel
            ' If Not LstBarang.Focused Then
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            ' End If
        End If

        If DgvDataTransaksi.CurrentCell.ColumnIndex = 4 Then
            If TypeOf e.Control Is ComboBox Then
                Dim comboBox As ComboBox = DirectCast(e.Control, ComboBox)
                RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
            End If
        End If
    End Sub

    ' [F3-T07-2] HAPUS: Event handler DgvNamaBarang_PreviewKeyDown - subroutine kosong, tidak diperlukan
    ' Alasan: Subroutine kosong, tidak ada logic
    ' Private Sub DgvNamaBarang_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
    ' End Sub

    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        If _sedangSetNilaiDariListBox Then Return
        _konteksLstBarang = "DGV"
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return
        Dim currentText = txt.Text.Trim()
        If String.IsNullOrEmpty(currentText) Then
            ' Jangan sembunyikan listbox jika user sedang pindah ke listbox (tekan panah bawah)
            ' atau jika listbox sudah difokus — teks kosong karena DGV BeginEdit ulang, bukan karena user hapus teks
            If _sedangPindahKeLstBarang OrElse LstBarang.Focused OrElse LstBarang.Visible Then
                Return
            End If
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            ResetBarcodeDetection()
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
        ' ✅ FIX: Hanya tambah karakter BARU (delta) ke buffer — bukan seluruh currentText.
        ' TextChanged membawa seluruh isi TextBox setiap kali, bukan hanya karakter terakhir.
        ' Jika buffer sudah punya N karakter dan currentText punya M karakter (M > N),
        ' hanya tambah karakter dari posisi N ke M.
        Dim charsSchon As Integer = barcodeChars.Count
        If currentText.Length > charsSchon Then
            For i As Integer = charsSchon To currentText.Length - 1
                If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(currentText(i))
            Next
        ElseIf currentText.Length < charsSchon Then
            ' User hapus karakter (backspace) — reset buffer, mulai ulang
            barcodeChars.Clear()
            barcodeStartTime = currentTime
            For Each ch As Char In currentText
                If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(ch)
            Next
        End If
        ' Jika currentText.Length = charsSchon → tidak ada perubahan, skip
        lastKeyTime = currentTime
        barcodeTimer.Stop()
        barcodeTimer.Start()

        ' Untuk input manual (ada huruf) — tampilkan ListBox langsung tanpa tunggu timer
        Dim keyword As String = currentText
        Dim levelDGV As Integer = 1
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            Dim qty As Decimal = ModuleAngka.ParseDecimal(parts(0).Trim())
            If qty > 0 Then TxtQty.Text = qty.ToString()
            If parts.Length >= 3 Then
                Dim lvl As Integer = 0
                If Integer.TryParse(parts(1).Trim(), lvl) AndAlso lvl >= 1 AndAlso lvl <= 3 Then levelDGV = lvl
                keyword = parts(parts.Length - 1).Trim()
            Else
                keyword = parts(parts.Length - 1).Trim()
            End If
        End If

        ' Hanya tampilkan ListBox untuk input manual (ada huruf) — barcode murni ditangani timer
        If keyword.Any(AddressOf Char.IsLetter) Then
            TxtLevelSat.Text = levelDGV.ToString()
            ' ── Debounce: tunda query sampai user berhenti ketik 250ms ────
            _searchKeywordPending = keyword
            _searchKonteksPending = "DGV"
            _searchTimer.Stop()
            _searchTimer.Start()
        End If
    End Sub

    ' [F3-T07b-1] TAMBAH: Event handler DgvNamaBarang_KeyDown untuk navigasi ListBox (Mode 2)
    ' Fungsi: Handle Down arrow dan Enter untuk navigasi/pemilihan dari TextBox DGV ke ListBox
    ' Alur: Down → pindah ke ListBox | Enter → ambil item pertama/terpilih langsung
    Private Sub DgvNamaBarang_KeyDown(sender As Object, e As KeyEventArgs)
        ' Hanya handle jika ListBox visible dan ada items
        If Not LstBarang.Visible OrElse LstBarang.Items.Count = 0 Then Return

        Select Case e.KeyCode
            Case Keys.Down
                ' Simpan teks yang sedang diketik sebelum pindah ke ListBox
                If _dgvEditingTextBox IsNot Nothing Then
                    _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                End If
                _sedangPindahKeLstBarang = True
                If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                Me.BeginInvoke(New Action(Sub()
                                              Me.BeginInvoke(New Action(Sub()
                                                                            If LstBarang.Visible Then
                                                                                _sedangSetNilaiDariListBox = True
                                                                                DgvDataTransaksi.EndEdit()
                                                                                _sedangSetNilaiDariListBox = False
                                                                                LstBarang.Focus()
                                                                            End If
                                                                            _sedangPindahKeLstBarang = False
                                                                        End Sub))
                                          End Sub))
                e.SuppressKeyPress = True

            Case Keys.Enter
                ' Enter langsung dari TextBox DGV → ambil item pertama atau yang ter-highlight
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

    ''' <summary>Tutup ListBox dan reset semua state terkait posisi.</summary>
    Private Sub TutupListBox()
        LstBarang.Visible = False
        LstBarang.Items.Clear()
        _listBoxDibukaDiRow = -1
        _listBoxDibukaDiCol = -1
    End Sub

    Private Sub PosisikanLstBarangDiBawahSel()
        If DgvDataTransaksi.CurrentCell Is Nothing Then Return
        Try
            Dim cellRect = DgvDataTransaksi.GetCellDisplayRectangle(
                DgvDataTransaksi.CurrentCell.ColumnIndex, DgvDataTransaksi.CurrentCell.RowIndex, True)
            Dim ptDgv = DgvDataTransaksi.PointToScreen(New Point(cellRect.Left, cellRect.Bottom))
            Dim ptPanel = Me.PointToClient(ptDgv)

            LstBarang.Width = Math.Max(300, cellRect.Width)

            ' Cek sisa ruang vertikal di bawah sel aktif untuk menentukan posisi LstBarang (Atas/Bawah)
            Dim spaceBelow As Integer = Me.ClientSize.Height - ptPanel.Y
            If spaceBelow < LstBarang.Height + 40 Then
                ' Tampilkan di atas sel: Y = Bawah Sel - Tinggi Sel - Tinggi ListBox
                Dim targetY As Integer = ptPanel.Y - cellRect.Height - LstBarang.Height
                LstBarang.Location = New Point(ptPanel.X, targetY)
            Else
                ' Tampilkan di bawah sel
                LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            End If
        Catch
        End Try
    End Sub

    Private Sub PosisikanLstBarangDiBawahTxtNama()
        Try
            Dim ptTxt = TxtNama.PointToScreen(New Point(0, TxtNama.Height))
            Dim ptPanel = Me.PointToClient(ptTxt)
            LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            LstBarang.Width = Math.Max(300, TxtNama.Width)
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Guard penutup ListBox saat user pindah sel di DGV.
    ''' WAJIB DIPERTAHANKAN — event handler ini tidak dipanggil secara eksplisit,
    ''' tapi otomatis terpicu oleh DGV setiap kali sel ditinggalkan.
    '''
    ''' Logika:
    ''' - Jika ListBox sedang difokus (user sedang navigasi di ListBox) → jangan tutup
    ''' - Jika sedang dalam proses pindah ke ListBox (_sedangPindahKeLstBarang) → jangan tutup
    ''' - Jika pindah sel masih di baris yang sama dengan saat ListBox dibuka → jangan tutup
    ''' - Selain itu → tutup ListBox dan bersihkan items
    '''
    ''' Tanpa fungsi ini, ListBox akan tetap tampil saat user klik sel lain.
    ''' </summary>
    Private Sub DgvData_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvDataTransaksi.CellLeave
        ' Guard: BeginInvoke hanya bisa dipanggil setelah window handle terbentuk.
        If Not Me.IsHandleCreated Then Return

        ' PENTING: Gunakan BeginInvoke agar cek dilakukan SETELAH fokus benar-benar berpindah.
        ' Masalah tanpa BeginInvoke: saat user klik ListBox, CellLeave terpicu sebelum ListBox
        ' mendapat fokus → LstBarang.Focused masih False → ListBox ditutup sebelum user bisa memilih.
        ' Dengan BeginInvoke: cek dilakukan di message loop berikutnya, fokus sudah berpindah.
        Me.BeginInvoke(New Action(Sub()
                                      If LstBarang.Visible Then
                                          If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then
                                              Return
                                          End If
                                          If _listBoxDibukaDiRow >= 0 AndAlso
                                             DgvDataTransaksi.CurrentCell IsNot Nothing AndAlso
                                             DgvDataTransaksi.CurrentCell.RowIndex = _listBoxDibukaDiRow AndAlso
                                             DgvDataTransaksi.CurrentCell.ColumnIndex = _listBoxDibukaDiCol Then
                                              Return
                                          End If
                                          LstBarang.Visible = False
                                          LstBarang.Items.Clear()
                                          _listBoxDibukaDiRow = -1
                                          _listBoxDibukaDiCol = -1
                                      End If
                                  End Sub))
    End Sub


    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)
        Dim cell As DataGridViewComboBoxCell = DirectCast(DgvDataTransaksi.CurrentCell, DataGridViewComboBoxCell)
        If LblJenisPl.Text = "Partai" Then
            Using cmd As New MySqlCommand("select ID_BARANG,ISI_PARTAI_KECIL,HARGA_JUAL_PARTAI_KECIL,ISI_PARTAI_SEDANG,HARGA_JUAL_PARTAI_SEDANG,ISI_PARTAI_BESAR,HARGA_JUAL_PARTAI_BESAR from tbl_barang WHERE ID_BARANG = ?", conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", cell.OwningRow.Cells("Kode").Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        If comboBox.SelectedIndex = 0 Then
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(rd.Item("ISI_PARTAI_KECIL")), 1, CInt(rd.Item("ISI_PARTAI_KECIL"))))
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_KECIL")
                        ElseIf comboBox.SelectedIndex = 1 Then
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(rd.Item("ISI_PARTAI_SEDANG")), 1, CInt(rd.Item("ISI_PARTAI_SEDANG"))))
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_SEDANG")
                        Else
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(rd.Item("ISI_PARTAI_BESAR")), 1, CInt(rd.Item("ISI_PARTAI_BESAR"))))
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_BESAR")
                        End If
                    Else
                        MessageBox.Show("Satuan barang dan atau harga jual belum di input ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using
            End Using
        Else
            Using cmd As New MySqlCommand("select ID_BARANG,ISI_UMUM_KECIL,HARGA_JUAL_UMUM_KECIL, ISI_UMUM_SEDANG,HARGA_JUAL_UMUM_SEDANG, ISI_UMUM_BESAR,HARGA_JUAL_UMUM_BESAR from tbl_barang WHERE ID_BARANG = ?", conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", cell.OwningRow.Cells("Kode").Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        ' Update the contents of the adjacent text box column based on the selected option in the combo box
                        If comboBox.SelectedIndex = 0 Then
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(rd.Item("ISI_UMUM_KECIL")), 1, CInt(rd.Item("ISI_UMUM_KECIL"))))
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_KECIL")
                        ElseIf comboBox.SelectedIndex = 1 Then
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(rd.Item("ISI_UMUM_SEDANG")), 1, CInt(rd.Item("ISI_UMUM_SEDANG"))))
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_SEDANG")
                        Else
                            cell.OwningRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(rd.Item("ISI_UMUM_BESAR")), 1, CInt(rd.Item("ISI_UMUM_BESAR"))))
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_BESAR")
                        End If
                    Else
                        MessageBox.Show("Satuan barang dan atau harga jual belum di input ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using
            End Using
        End If
        Dim rowI As Integer = DgvDataTransaksi.CurrentCell.RowIndex
        HitungNilaiSetiapBaris(rowI)
        UpdateSemuaTotal()
    End Sub


    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvDataTransaksi.KeyDown
        If e.KeyCode = Keys.Delete Then
            If DgvDataTransaksi.SelectedCells.Count > 0 Then
                Dim selectedCell As DataGridViewCell = DgvDataTransaksi.SelectedCells(0)

                ' Periksa apakah sel yang dipilih berada di kolom "Nama"
                If selectedCell.ColumnIndex = DgvDataTransaksi.Columns("NamaBarang").Index Then
                    Dim rowIndex As Integer = selectedCell.RowIndex

                    ' Periksa apakah nilai di kolom "Nama" tidak kosong
                    If Not String.IsNullOrEmpty(DgvDataTransaksi.Rows(rowIndex).Cells("NamaBarang").Value.ToString()) Then
                        ' Hapus baris jika nilai di kolom "Nama" tidak kosong
                        Hapusbaris()
                        DgvDataTransaksi.ClearSelection()
                        ' Kembalikan fokus ke baris kosong berikutnya sesuai setting
                        SetupFocusToGrid()
                    Else
                        MessageBox.Show("Klik kanan pada baris yang tidak kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
            UpdateSemuaTotal()
        End If

    End Sub

    Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvDataTransaksi.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' ✅ PERBAIKAN: Izinkan hapus meskipun read-only via klik kanan
            Dim cell As DataGridViewCell = DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang")
            If cell IsNot Nothing AndAlso cell.Value IsNot Nothing Then
                ' Periksa apakah nilai di kolom "Nama" pada baris yang diklik tidak kosong
                Dim namaValue As String = cell.Value.ToString()
                If Not String.IsNullOrEmpty(namaValue) Then
                    ' Setel sel saat ini ke sel "Nama"
                    DgvDataTransaksi.CurrentCell = cell

                    ' Tampilkan ContextMenuStrip di lokasi kursor
                    Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                    ContextMenuStrip1.Show(cursorPosition)
                End If
            End If
        End If
    End Sub


    Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvDataTransaksi.CellEnter
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Kolom Satuan — langsung BeginEdit + buka dropdown agar panah atas/bawah
            ' bisa memilih satuan tanpa F2 atau klik.
            ' EditMode = EditOnKeystroke (di Designer) — BeginEdit dipanggil manual di sini
            ' karena keystroke saja tidak cukup untuk membuka dropdown ComboBox.
            ' Jika auto level satuan aktif: satuan sudah ditentukan dari qty — dropdown tidak perlu dibuka
            If DgvDataTransaksi.Columns(e.ColumnIndex).Name = "Satuan" AndAlso
               Not ModulHakAkses.SettingAutoLevelSatuan Then
                DgvDataTransaksi.BeginInvoke(New Action(Sub()
                                                            If DgvDataTransaksi.CurrentCell IsNot Nothing AndAlso
                       DgvDataTransaksi.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
                       DgvDataTransaksi.CurrentCell.RowIndex = e.RowIndex Then
                                                                DgvDataTransaksi.BeginEdit(True)
                                                                Dim combo = TryCast(DgvDataTransaksi.EditingControl, ComboBox)
                                                                If combo IsNot Nothing Then combo.DroppedDown = True
                                                            End If
                                                        End Sub))
            End If

            ' Kontrol read-only berdasarkan kolom Kode
            If e.ColumnIndex = 1 Then ' Kolom NamaBarang (index 1)
                Dim kodeValue = DgvDataTransaksi.Rows(e.RowIndex).Cells("Kode").Value

                ' Jika Kode sudah terisi, buat NamaBarang read-only
                If kodeValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(kodeValue.ToString()) Then
                    DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").ReadOnly = True
                    DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                    DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                Else
                    DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").ReadOnly = False
                    DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
                    DgvDataTransaksi.Rows(e.RowIndex).Cells("NamaBarang").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                End If
            End If

            Dim colName As String = DgvDataTransaksi.Columns(e.ColumnIndex).Name
            SetRichTextBoxWithLimitedTooltip(rtbPetunjuk, colName)
        Else
            rtbPetunjuk.Clear()
        End If
    End Sub

    Private Sub DgvData_Leave(sender As Object, e As EventArgs) Handles DgvDataTransaksi.Leave
        rtbPetunjuk.Clear()
        'rtbPetunjuk.SelectionColor = Color.Black
        'rtbPetunjuk.Text = "🔍 Arahkan ke kolom untuk melihat petunjuk penggunaan."
    End Sub

    Private Sub SetRichTextBoxWithLimitedTooltip(rtb As RichTextBox, colName As String)
        Dim fullText As String = GetTooltipForColumn(colName)
        If String.IsNullOrEmpty(fullText) Then
            rtb.Clear()
            Return
        End If

        Dim lines() As String = fullText.Split(New String() {vbCrLf}, StringSplitOptions.None)

        ' Baris pertama = judul + icon
        Dim judul As String = lines(0)

        ' Batasi petunjuk maksimal 2 baris setelah judul
        Dim petunjukLines As String = ""
        If lines.Length > 1 Then
            Dim maxLines = Math.Min(2, lines.Length - 1)
            petunjukLines = String.Join(vbCrLf, lines, 1, maxLines)
        End If

        ' Bersihkan dulu
        rtb.Clear()

        ' Set warna sama untuk semua teks
        rtb.SelectionColor = ModuleTheme.C(ModuleTheme.L_Primary, ModuleTheme.D_Primary)

        ' Tulis judul + petunjuk
        rtb.AppendText(judul & vbCrLf)
        If petunjukLines <> "" Then
            rtb.AppendText(petunjukLines)
        End If

        ' Reset selection supaya tidak terselect
        rtb.SelectionStart = 0
        rtb.SelectionLength = 0
    End Sub


    Private Function GetTooltipForColumn(colName As String) As String
        Select Case colName
            Case "NamaBarang"
                Return "📦 Kolom Nama Barang: Bisa diketik atau scan barcode. Gunakan 🔼/🔽 untuk navigasi, ↵ ENTER untuk memilih." & vbCrLf &
      "✍️ Untuk isi Qty secara otomatis bisa menggunakan format qty*barcode : contoh ketik 2* lalu scan → kolom QTY langsung terisi 2."


            Case "QTY"
                Return "🔢 Kolom Qty: Jumlah unit/barang yang dibeli." & vbCrLf &
               "Pastikan isi angka yang valid."

            Case "Satuan"
                Return "📏 Kolom Satuan: Satuan barang, misal PCS, DUS, atau PACK." & vbCrLf &
               "Gunakan satuan yang sesuai produk."

            Case "Harga"
                Return "💰 Kolom Harga: Harga jual per unit sebelum diskon." & vbCrLf &
               "Pastikan harga sudah benar."

            Case "DiskonPersen"
                Return "🔻 Kolom Diskon %: Diskon dalam persen dari harga jual." & vbCrLf &
               "Masukkan nilai antara 0-100."

            Case "DiskonRp"
                Return "💸 Kolom Diskon Rp: Diskon dalam nominal (potongan harga langsung)." & vbCrLf &
               "Pastikan nominal diskon sesuai."

            Case "TotalDiskon"
                Return "💸 Kolom Total Diskon: Total diskon = Diskon × Qty." & vbCrLf &
               "Menunjukkan potongan harga keseluruhan."

            Case "TotalHarga"
                Return "🧾 Kolom Total Harga: Total bayar = (Harga - Diskon) × Qty." & vbCrLf &
               "Jumlah yang harus dibayar pelanggan."

            Case "StokToko"
                Return "🏪 Kolom Stok Toko: Stok barang yang tersedia di toko." & vbCrLf &
               "Gunakan untuk cek ketersediaan barang di Toko."

            Case "StokGudang"
                Return "🏢 Kolom Stok Gudang: Stok barang yang tersedia di gudang." & vbCrLf &
               "Gunakan untuk cek ketersediaan barang di Gudang."

            Case "SerialNumber"
                Return "🏢 Kolom S N: Di isi serial number untuk barang ber serial number." & vbCrLf &
               "Gunakan untuk klaim garansi."

            Case Else
                Return ""
        End Select
    End Function


    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
        ' [F10-T28-3] TAMBAH: Kembalikan fokus sesuai setting setelah hapus baris
        ' Alasan: Setelah action context menu, fokus perlu kembali ke posisi yang sesuai
        SetupFocusToGrid()
    End Sub

    Private Sub Hapusbaris()
        ' Periksa apakah ada sel yang dipilih
        If DgvDataTransaksi.CurrentCell Is Nothing Then
            MessageBox.Show("Tidak ada baris yang dipilih.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Dim baris As Integer = DgvDataTransaksi.CurrentCell.RowIndex

        ' Periksa apakah sel dalam mode edit
        If DgvDataTransaksi.IsCurrentCellInEditMode Then
            MessageBox.Show("Tidak dapat menghapus baris dalam mode edit.", "Mode Edit Aktif", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Konfirmasi penghapusan untuk baris yang berisi data
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus baris ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            DgvDataTransaksi.Rows.RemoveAt(baris)
            UpdateSemuaTotal()
        End If
    End Sub

    Private Sub DgvData_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvDataTransaksi.CellFormatting
        ' Pastikan kolom "StokToko" dan "StokGudang" ada
        If DgvDataTransaksi.Columns("StokToko") IsNot Nothing AndAlso DgvDataTransaksi.Columns("StokGudang") IsNot Nothing Then
            Dim stokTokoIndex As Integer = DgvDataTransaksi.Columns("StokToko").Index
            Dim stokGudangIndex As Integer = DgvDataTransaksi.Columns("StokGudang").Index

            ' Cek apakah sel yang sedang diformat adalah bagian dari "StokToko" atau "StokGudang"
            If e.ColumnIndex = stokTokoIndex OrElse e.ColumnIndex = stokGudangIndex Then
                Dim stokValue As Object = e.Value
                If stokValue IsNot Nothing AndAlso ModuleAngka.ParseDecimal(stokValue) < 1 Then
                    ' Stok habis — warna informasi (amber), bukan merah
                    e.CellStyle.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowStokHabis, ModuleTheme.D_DgvRowStokHabis)
                    e.CellStyle.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                End If
            End If
        End If
    End Sub

    Private Sub HitungUlangBarisIniToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HitungUlangBarisIniToolStripMenuItem.Click
        Hitungbaris()
        UpdateSemuaTotal()
        ' [F10-T28-4] TAMBAH: Kembalikan fokus sesuai setting setelah hitung ulang
        SetupFocusToGrid()
    End Sub

    ''' <summary>Refresh info stok baris yang dipilih via klik kanan.</summary>
    Private Sub RefreshStokBarisIniToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RefreshStokBarisIniToolStripMenuItem.Click
        If DgvDataTransaksi.CurrentCell IsNot Nothing Then
            RefreshStokBaris(DgvDataTransaksi.CurrentCell.RowIndex)
        End If
        ' [F10-T28-5] TAMBAH: Kembalikan fokus sesuai setting setelah refresh stok
        SetupFocusToGrid()
    End Sub

    ''' <summary>Refresh info stok semua baris via klik kanan.</summary>
    Private Sub RefreshStokSemuaBarisToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RefreshStokSemuaBarisToolStripMenuItem.Click
        RefreshStokSemuaBaris()
        ' [F10-T28-6] TAMBAH: Kembalikan fokus sesuai setting setelah refresh semua stok
        SetupFocusToGrid()
    End Sub

    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs) Handles DgvDataTransaksi.DataError
        Dim errorMessage As String = "Kesalahan data: " & e.Exception.Message & Environment.NewLine &
                                 "Periksa baris yang disorot dan perbaiki."

        MessageBox.Show(errorMessage, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)

        ' Menyorot baris yang bermasalah
        If e.RowIndex >= 0 Then
            For Each cell As DataGridViewCell In DgvDataTransaksi.Rows(e.RowIndex).Cells
                cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_Danger, ModuleTheme.D_Danger)
            Next
        End If

        e.Cancel = True
    End Sub

    Public Sub UpdateSemuaTotal()
        ' ✅ PHASE 2 OPTIMIZATION: Single loop instead of 4+ separate loops
        Dim totalHpp As Decimal = 0
        Dim totalGrand As Decimal = 0
        Dim totalQtyBarang As Decimal = 0
        Dim totalItemCount As Integer = 0
        Dim totalQtySat As Decimal = 0

        ' Single loop untuk semua calculations
        For Each row As DataGridViewRow In DgvDataTransaksi.Rows
            If Not row.IsNewRow Then
                ' === Hitung Total Harga Beli (HPP) ===
                If row.Cells("Totalhargabeli").Value IsNot Nothing Then
                    totalHpp += Math.Round(ModuleAngka.ParseDecimal(row.Cells("Totalhargabeli").Value))
                End If

                ' === Hitung Grand Total Harga Jual ===
                If row.Cells("TotalHarga").Value IsNot Nothing Then
                    totalGrand += Math.Round(ModuleAngka.ParseDecimal(row.Cells("TotalHarga").Value))
                End If

                ' === Hitung Jumlah Barang dan Jumlah Item ===
                Dim qtyObj As Object = row.Cells("QTY").Value
                If qtyObj IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyObj.ToString()) Then
                    totalQtyBarang += Math.Round(ModuleAngka.ParseDecimal(qtyObj))
                    totalItemCount += 1
                End If

                ' === Hitung Total QTY Satuan ===
                If row.Cells("QtySat").Value IsNot Nothing Then
                    totalQtySat += Math.Round(ModuleAngka.ParseDecimal(row.Cells("QtySat").Value), 2)
                End If
            End If
        Next

        ' Set semua values sekaligus
        _grandTotalSO = totalGrand
        TxtTotalHpp.Text = totalHpp.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtTotalJualSblDiskonPajak.Text = totalGrand.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtTotaljualStlPajak.Text = totalGrand.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtGrandtotal.Text = "Rp. " & totalGrand.ToString("N0")
        TxtJmlhQty.Text = totalQtyBarang.ToString()
        TxtJmlhItem.Text = totalItemCount.ToString()
        LblRecord.Text = "Total record : " & totalItemCount.ToString()
        TxtJmlhQtySatuan.Text = totalQtySat.ToString()

        ' === Scroll otomatis ke baris terakhir ===
        If DgvDataTransaksi.Rows.Count > 0 Then
            DgvDataTransaksi.FirstDisplayedScrollingRowIndex = DgvDataTransaksi.Rows.Count - 1
        End If
    End Sub


    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click
        TekanBarang()
    End Sub

    Private Sub BtnPelanggan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPelanggan.Click
        Tekanpelanggan()
    End Sub

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        TekanSimpan()
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs)
        TxtNama.Clear()
        ' Menambahkan pertanyaan apakah akan keluar
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar dari halaman Sales Order ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Close()
        End If
    End Sub

    Private Sub Form_SO_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F2

                CmbSales.Select()
                CmbSales.Focus()
                CmbSales.DroppedDown = True
            Case Keys.F3

                CmbPelanggan.Select()
                CmbPelanggan.Focus()
                CmbPelanggan.DroppedDown = True
            Case Keys.F4
                TekanBarang()

            Case Keys.F5

            Case Keys.F12
                Tekanpelanggan()

            Case Keys.F6


            Case Keys.F7


            Case Keys.F8
                ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
                If DgvDataTransaksi.IsCurrentCellInEditMode Then
                    DgvDataTransaksi.EndEdit()
                End If
                TekanSimpan()

            Case Keys.F9


            Case Keys.F10




            Case Keys.F11


            Case Keys.Escape

                TxtNama.Clear()

                ' Menambahkan pertanyaan apakah akan keluar
                Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Close()
                End If

            Case Keys.Back, Keys.Delete
                ' Jika LstBarang visible dan tombol Backspace atau Delete ditekan
                ' Hanya untuk konteks TXTNAMA — di DGV biarkan backspace bekerja normal di editing control
                If LstBarang.Visible = True AndAlso _konteksLstBarang = "TXTNAMA" Then
                    TxtNama.Select()
                End If

        End Select
    End Sub

    Private Sub BtnKeluarForm_Click(sender As Object, e As EventArgs) Handles BtnKeluarForm.Click
        ' Menambahkan pertanyaan apakah akan keluar
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Close()
        End If
    End Sub

    Private Sub Hitungbaris()
        For Each row As DataGridViewRow In DgvDataTransaksi.Rows
            If Not row.IsNewRow AndAlso Not IsDBNull(row.Cells("Kode").Value) AndAlso Not String.IsNullOrEmpty(row.Cells("Kode").Value.ToString().Trim()) Then
                ' Variables for HargaBeli calculation
                Dim hargaBeli = If(Not IsDBNull(row.Cells("HargaBeli").Value), ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value), 0)
                Dim qty = If(Not IsDBNull(row.Cells("QTY").Value), ModuleAngka.ParseDecimal(row.Cells("QTY").Value), 0)
                Dim isi = If(Not IsDBNull(row.Cells("Isi").Value), ModuleAngka.ParseInteger(row.Cells("Isi").Value, defaultValue:=1), 1)

                ' Variables for QtySat calculation
                Dim qtySat = qty * isi

                ' Variables for TotalHarga calculation
                Dim harga = If(Not IsDBNull(row.Cells("Harga").Value), ModuleAngka.ParseDecimal(row.Cells("Harga").Value), 0)
                Dim totalDiskon = If(Not IsDBNull(row.Cells("TotalDiskon").Value), ModuleAngka.ParseDecimal(row.Cells("TotalDiskon").Value), 0)

                ' Variables for TotalHarga calculation
                Dim totalHarga = qty * harga - totalDiskon

                ' Assign values to DataGridView columns
                row.Cells("Totalhargabeli").Value = hargaBeli * qtySat
                row.Cells("QtySat").Value = qtySat
                row.Cells("TotalHarga").Value = totalHarga
            End If
        Next
    End Sub

    Public Sub TekanBarang()
        Using f As New TambahBarang()
            f.LblHeaderForm.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using
    End Sub

    Public Sub Tekanpelanggan()
        TambahPelanggan.ShowDialog()
        Call TampilPelanggan()
    End Sub


    Public Class StokInfo
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    Public Class BarangInfo
        Public Property HargaBeli As Decimal
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    ''' <summary>
    ''' ✅ BATCH 4: Helper class untuk menyimpan info harga barang dari batch query
    ''' Digunakan oleh: UpdateHargaBerdasarJenisPelanggan()
    ''' </summary>
    Public Class BarangHargaInfo
        Public Property HargaBeli As Decimal
        Public Property SatuanUmumKecil As String
        Public Property SatuanUmumSedang As String
        Public Property SatuanUmumBesar As String
        Public Property HargaJualUmumKecil As Decimal
        Public Property HargaJualUmumSedang As Decimal
        Public Property HargaJualUmumBesar As Decimal
        Public Property SatuanPartaiKecil As String
        Public Property SatuanPartaiSedang As String
        Public Property SatuanPartaiBesar As String
        Public Property HargaJualPartaiKecil As Decimal
        Public Property HargaJualPartaiSedang As Decimal
        Public Property HargaJualPartaiBesar As Decimal
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    Public Function CekStok() As Boolean
        ' ✅ PHASE 2 OPTIMIZATION: Batch query instead of per-item queries
        Dim stokDict As New Dictionary(Of String, StokInfo)
        Dim kodeBarangList As New List(Of String)()

        ' Step 1: Kumpulkan semua kode barang dari grid
        For Each dgvRow As DataGridViewRow In DgvDataTransaksi.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                kodeBarangList.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        If kodeBarangList.Count = 0 Then Return False

        ' Step 2: ✅ SINGLE BATCH QUERY untuk semua stok (bukan per-item!)
        Dim inClause As String = String.Join(",", kodeBarangList.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim batchStokQuery As String = "SELECT ID_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        Try
            Using cmd As New MySqlCommand(batchStokQuery, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kodeBarang As String = rd("ID_BARANG").ToString()
                        stokDict(kodeBarang) = New StokInfo() With {
                        .StokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                        .StokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    }
                    End While
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try

        ' Step 3: ✅ SINGLE BATCH QUERY untuk sales_order_detail (kalau edit transaksi)
        If Not IsModeTambahSO Then
            Dim batchDetailQuery As String = "SELECT ID_BARANG, SUM(QTY_SATUAN) AS TOTAL_QTY FROM sales_order_detail WHERE FAKTUR_JUAL = @FK AND ID_BARANG IN (" & inClause & ") GROUP BY ID_BARANG"
            Try
                Using cmd As New MySqlCommand(batchDetailQuery, conn)
                    cmd.Parameters.AddWithValue("@FK", TxtFaktur.Text)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            Dim idBarang As String = rd("ID_BARANG").ToString()
                            If stokDict.ContainsKey(idBarang) Then
                                Dim totalQty As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TOTAL_QTY", 0D)
                                If LblLokasiBarang.Text = "TOKO" Then
                                    stokDict(idBarang).StokToko += totalQty
                                Else
                                    stokDict(idBarang).StokGudang += totalQty
                                End If
                            End If
                        End While
                    End Using
                End Using
            Catch ex As Exception
            End Try
        End If

        ' Step 4: Validasi offline (no more queries!)
        For Each dgvRow As DataGridViewRow In DgvDataTransaksi.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()
                Dim totalQtyTerjual As Decimal = If(IsDBNull(dgvRow.Cells("QtySat").Value), 0D, ModuleAngka.ParseDecimal(dgvRow.Cells("QtySat").Value))

                If stokDict.ContainsKey(kodeBarangValue) Then
                    Dim stokInfo As StokInfo = stokDict(kodeBarangValue)
                    Dim totalStok As Decimal = If(LblLokasiBarang.Text = "TOKO", stokInfo.StokToko, stokInfo.StokGudang)

                    If totalQtyTerjual > totalStok Then
                        Dim errorMessage As String = "Stok ==> " & dgvRow.Cells("NamaBarang").Value & " <== tidak mencukupi untuk dijual." & vbCrLf & vbCrLf & "Total Terjual: " & totalQtyTerjual & vbCrLf & "Total Stok: " & totalStok
                        MessageBox.Show(errorMessage, "Stok Tidak cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowPeringatan, ModuleTheme.D_DgvRowPeringatan)
                        Next

                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1)
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True
                    End If

                    ' Reset warna
                    Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = defaultBackColor
                    Next
                End If
            End If
        Next

        Return False
    End Function

    Public Function Cekjualrugi() As Boolean
        ' ✅ PHASE 2 OPTIMIZATION: Batch query instead of per-item queries
        Dim hargaBeliDict As New Dictionary(Of String, Decimal)()
        Dim kodeBarangList As New List(Of String)()

        ' Step 1: Kumpulkan semua kode barang dari grid
        For Each dgvRow As DataGridViewRow In DgvDataTransaksi.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                kodeBarangList.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        If kodeBarangList.Count = 0 Then Return False

        ' Step 2: ✅ SINGLE BATCH QUERY untuk semua HARGA_BELI (bukan per-item!)
        Dim inClause As String = String.Join(",", kodeBarangList.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim batchQuery As String = "SELECT ID_BARANG, HARGA_BELI FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        Try
            Using cmd As New MySqlCommand(batchQuery, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kodeBarang As String = rd("ID_BARANG").ToString()
                        hargaBeliDict(kodeBarang) = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    End While
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try

        ' Step 3: Validasi offline (no more queries!)
        For Each dgvRow As DataGridViewRow In DgvDataTransaksi.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()
                Dim Hargajual As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("Harga").Value)

                If hargaBeliDict.ContainsKey(kodeBarangValue) Then
                    Dim Hargabeli As Decimal = hargaBeliDict(kodeBarangValue)

                    If Hargabeli > Hargajual Then
                        Dim errorMessage As String = "Barang: " & dgvRow.Cells("NamaBarang").Value & vbCrLf & vbCrLf & "Harga beli: " & Hargabeli.ToString("N0") & vbCrLf & vbCrLf & "Harga jual: " & Hargajual.ToString("N0")
                        MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowPeringatan, ModuleTheme.D_DgvRowPeringatan)
                        Next

                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1)
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True
                    End If
                End If

                ' Reset warna
                Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = defaultBackColor
                Next
            End If
        Next

        Return False
    End Function


    Public Sub TekanSimpan()
        If String.IsNullOrWhiteSpace(TxtFaktur.Text) Then
            MessageBox.Show("Nomor faktur wajib diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtFaktur.Focus()
            Exit Sub
        End If

        ' Cek apakah ada item yang dimasukkan
        If String.IsNullOrWhiteSpace(TxtJmlhItem.Text) OrElse DgvDataTransaksi.Rows.Count = 0 Then
            MessageBox.Show("Belum ada barang yang dimasukkan ke dalam transaksi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Cek apakah nominal 0 diizinkan
        If Not ModulHakAkses.SettingIzinkanNominalJualNol AndAlso (String.IsNullOrEmpty(TxtTotalJualSblDiskonPajak.Text) OrElse TxtTotalJualSblDiskonPajak.Text = "0") Then
            MessageBox.Show("Total Sales Order belum terisi. Tidak bisa melanjutkan proses penyimpanan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Cek jika menjual rugi tidak diizinkan
        If Not ModulHakAkses.SettingIzinkanJualRugi AndAlso Cekjualrugi() Then
            Exit Sub
        End If

        ' Cek jika stok minus tidak diizinkan
        If Not ModulHakAkses.SettingIzinkanBarangMinus AndAlso CekStok() Then
            Exit Sub
        End If

        If Not ValidateDataBarangGrid() Then Exit Sub

        ' ═══════════════════════════════════════════════════════════════
        ' SEMUA VALIDASI PASSED → SIMPAN
        ' ═══════════════════════════════════════════════════════════════
        Simpanatauedit()
    End Sub

    ''' <summary>
    ''' Validasi data barang di grid sebelum simpan
    ''' Return True jika valid, False jika ada error
    ''' </summary>
    Private Function ValidateDataBarangGrid() As Boolean
        For Each row As DataGridViewRow In DgvDataTransaksi.Rows
            If Not row.IsNewRow AndAlso row.Cells("Kode").Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(row.Cells("Kode").Value.ToString().Trim()) Then

                ' Cek Kode
                If String.IsNullOrEmpty(row.Cells("Kode").Value.ToString().Trim()) Then
                    MessageBox.Show("Kode barang tidak boleh kosong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

                ' Cek Nama Barang
                If String.IsNullOrEmpty(row.Cells("NamaBarang").Value.ToString()) Then
                    MessageBox.Show("Nama barang tidak boleh kosong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

                ' Cek Harga Jual > 0
                Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(row.Cells("Harga").Value)
                If hargaJual <= 0 Then
                    MessageBox.Show(
                    "Harga jual barang '" & row.Cells("NamaBarang").Value & "' harus lebih dari 0.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Return False
                End If

                ' Cek QTY > 0
                Dim qty As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY").Value)
                If qty <= 0 Then
                    MessageBox.Show(
                    "Jumlah barang '" & row.Cells("NamaBarang").Value & "' harus lebih dari 0.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Return False
                End If

                ' Cek Satuan
                If String.IsNullOrEmpty(row.Cells("Satuan").Value.ToString()) Then
                    MessageBox.Show(
                    "Satuan barang '" & row.Cells("NamaBarang").Value & "' tidak boleh kosong.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Return False
                End If
            End If
        Next

        Return True
    End Function


    Public Sub Simpanatauedit()
        Cursor = Cursors.WaitCursor
        If IsModeTambahSO AndAlso Not ModulHakAkses.SettingIzinkanTanggalLampau Then
            DTPTgl.Value = DateTime.Now
        End If
        If IsModeTambahSO Then
            ' ── Cek duplikat faktur (Multi-Kasir Protection) ─────────────────
            Dim query As String = "SELECT ID_PENJUALAN FROM sales_order WHERE ID_PENJUALAN = ?"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    ' Nomor sudah dipakai kasir lain, generate baru
                    NomorSO()
                End If
            End Using
        End If
        Prosessimpan()
        Cursor = Cursors.Default
    End Sub

    Public Sub Prosessimpan()
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            ' Jika mode Edit: hapus dulu data lama menggunakan modul pusat
            If Not IsModeTambahSO Then
                ' ========================================
                ' START: Audit Trail - Edit Sales Order
                ' ========================================
                ModuleAuditTrail.CatatAudit(TxtFaktur.Text, "EDIT", "Sales Order", ket:="[KRITIS] Edit Sales Order", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Sales Order
                ' ========================================
                ModuleHapusTransaksi.HapusSalesOrder(TxtFaktur.Text, LblLokasiBarang.Text, transaction)
            End If

            Dim idSO As String = TxtFaktur.Text
            Dim tgl As String = DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss")
            Dim totalHppHeader As Decimal = ModuleAngka.ParseDecimal(TxtTotalHpp.Text)
            Dim totalJualHeader As Decimal = _grandTotalSO
            Dim labaHeader As Decimal = totalJualHeader - totalHppHeader
            Dim idUser As String = If(IsModeTambahSO, FormUtama.StatusNamaUser.Text, TxtLogin.Text)
            Dim idKomputer As String = If(IsModeTambahSO, FormUtama.StatusNamaPC.Text, TxtKomputer.Text)

            ' Query Header: Dipecah menjadi beberapa baris agar mudah diaudit urutannya sesuai migrasi SQL
            Dim qSO As String = "INSERT INTO sales_order (" &
                "ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, ALAMAT_PELANGGAN, " &
                "LOKASIBARANG, ID_SALES, NAMA_SALES, TGL_TRANSAKSI, TOTAL_HPP, " &
                "GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, " &
                "GRAND_TOTAL_STL_PAJAK, LABA, STATUS_TRANSAKSI, CATATAN, ID_USER, ID_KOMPUTER) " &
                "VALUES (" &
                "@ID_PENJUALAN, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @ALAMAT_PELANGGAN, " &
                "@LOKASIBARANG, @ID_SALES, @NAMA_SALES, @TGL_TRANSAKSI, @TOTAL_HPP, " &
                "@GRAND_TOTAL_SBL_PAJAK, 0, 0, 0, 0, " &
                "@GRAND_TOTAL_STL_PAJAK, @LABA, 'Aktif', @CATATAN, @ID_USER, @ID_KOMPUTER)"
            Using cmd As New MySqlCommand(qSO, conn, transaction)
                cmd.Parameters.AddWithValue("@ID_PENJUALAN", idSO)
                cmd.Parameters.AddWithValue("@ID_PELANGGAN", LbLKodePel.Text)
                cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
                cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPl.Text)
                cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", LblAlamat.Text)
                cmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
                cmd.Parameters.AddWithValue("@ID_SALES", LblSales.Text)
                cmd.Parameters.AddWithValue("@NAMA_SALES", CmbSales.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
                cmd.Parameters.AddWithValue("@TOTAL_HPP", totalHppHeader)
                cmd.Parameters.AddWithValue("@GRAND_TOTAL_SBL_PAJAK", totalJualHeader)
                cmd.Parameters.AddWithValue("@DISKON_TOTAL_PERSEN", 0)
                cmd.Parameters.AddWithValue("@DISKON_TOTAL_RP", 0)
                cmd.Parameters.AddWithValue("@PAJAK_PERSEN", 0)
                cmd.Parameters.AddWithValue("@PAJAK_RP", 0)
                cmd.Parameters.AddWithValue("@GRAND_TOTAL_STL_PAJAK", totalJualHeader)
                cmd.Parameters.AddWithValue("@LABA", labaHeader)
                cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI", "Aktif")
                cmd.Parameters.AddWithValue("@CATATAN", "")
                cmd.Parameters.AddWithValue("@ID_USER", idUser)
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", idKomputer)
                cmd.ExecuteNonQuery()
            End Using

            ' Query Detail: Dipecah agar mudah memverifikasi pemetaan kolom (Tanpa LABA dan ID_AUTO)
            Dim qDetail As String = "INSERT INTO sales_order_detail (" &
                "FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, " &
                "TANGGAL_JUAL, ID_BARANG, NAMA_BARANG, SERIAL_NUMBER, HARGA_BELI, " &
                "QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, " &
                "QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_Harga, " &
                "ID_USER, ID_KOMPUTER, URUTAN) " &
                "VALUES (" &
                "@FAKTUR_JUAL, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @LOKASIBARANG, " &
                "@TANGGAL_JUAL, @ID_BARANG, @NAMA_BARANG, @SERIAL_NUMBER, @HARGA_BELI, " &
                "@QTY, @SATUAN, @ISI_SATUAN, @HARGA_BELI_SATUAN, @HARGA_JUAL, " &
                "@QTY_SATUAN, @DISKON_PERSEN, @DISKON_RP, @TOTAL_DISKON, @TOTAL_Harga, " &
                "@ID_USER, @ID_KOMPUTER, @URUTAN)"

            ' Query History: Dipecah untuk kejelasan struktur data history stok
            Dim sbSql As New System.Text.StringBuilder("INSERT INTO HistoryBarang (" &
                "FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, " &
                "NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, " &
                "TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) VALUES ")

            Using cmdD As New MySqlCommand(qDetail, conn, transaction)
                Using cmdH As New MySqlCommand("", conn, transaction)
                    cmdD.Parameters.Add("@FAKTUR_JUAL", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@ID_PELANGGAN", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@NAMA_PELANGGAN", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@JENIS_PELANGGAN", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@LOKASIBARANG", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@TANGGAL_JUAL", MySqlDbType.DateTime)
                    cmdD.Parameters.Add("@ID_BARANG", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@NAMA_BARANG", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@SERIAL_NUMBER", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@HARGA_BELI", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@QTY", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@SATUAN", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@ISI_SATUAN", MySqlDbType.Int32)
                    cmdD.Parameters.Add("@HARGA_BELI_SATUAN", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@HARGA_JUAL", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@QTY_SATUAN", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@DISKON_PERSEN", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@DISKON_RP", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@TOTAL_DISKON", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@TOTAL_Harga", MySqlDbType.Decimal)
                    cmdD.Parameters.Add("@ID_USER", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@ID_KOMPUTER", MySqlDbType.VarChar)
                    cmdD.Parameters.Add("@URUTAN", MySqlDbType.Int32)

                    Dim idx As Integer = 0
                    For Each row As DataGridViewRow In DgvDataTransaksi.Rows
                        If Not row.IsNewRow AndAlso row.Cells("Kode").Value IsNot Nothing AndAlso row.Cells("Kode").Value.ToString() <> "" Then
                            Dim idBarangRow As String = row.Cells("Kode").Value.ToString()
                            Dim namaBarangRow As String = row.Cells("NamaBarang").Value.ToString()
                            Dim snRow As String = If(row.Cells("SerialNumber").Value IsNot Nothing, row.Cells("SerialNumber").Value.ToString(), "")
                            Dim hrgBeliRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
                            Dim qtyRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY").Value)
                            Dim satuanRow As String = row.Cells("Satuan").Value.ToString()
                            Dim isiRow As Integer = ModuleAngka.ParseInteger(row.Cells("Isi").Value)
                            Dim hrgJualRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("Harga").Value)
                            Dim qtySatRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                            Dim discPersenRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("DiskonPersen").Value)
                            Dim discRpRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("DiskonRp").Value)
                            Dim totalDiscRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("TotalDiskon").Value)
                            Dim totalHargaRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("TotalHarga").Value)
                            Dim totalHppRow As Decimal = ModuleAngka.ParseDecimal(row.Cells("Totalhargabeli").Value)

                            cmdD.Parameters("@FAKTUR_JUAL").Value = idSO
                            cmdD.Parameters("@ID_PELANGGAN").Value = LbLKodePel.Text
                            cmdD.Parameters("@NAMA_PELANGGAN").Value = CmbPelanggan.Text
                            cmdD.Parameters("@JENIS_PELANGGAN").Value = LblJenisPl.Text
                            cmdD.Parameters("@LOKASIBARANG").Value = LblLokasiBarang.Text
                            cmdD.Parameters("@TANGGAL_JUAL").Value = tgl
                            cmdD.Parameters("@ID_BARANG").Value = idBarangRow
                            cmdD.Parameters("@NAMA_BARANG").Value = namaBarangRow
                            cmdD.Parameters("@SERIAL_NUMBER").Value = snRow
                            cmdD.Parameters("@HARGA_BELI").Value = hrgBeliRow
                            cmdD.Parameters("@QTY").Value = qtyRow
                            cmdD.Parameters("@SATUAN").Value = satuanRow
                            cmdD.Parameters("@ISI_SATUAN").Value = isiRow
                            cmdD.Parameters("@HARGA_BELI_SATUAN").Value = totalHppRow
                            cmdD.Parameters("@HARGA_JUAL").Value = hrgJualRow
                            cmdD.Parameters("@QTY_SATUAN").Value = qtySatRow
                            cmdD.Parameters("@DISKON_PERSEN").Value = discPersenRow
                            cmdD.Parameters("@DISKON_RP").Value = discRpRow
                            cmdD.Parameters("@TOTAL_DISKON").Value = totalDiscRow
                            cmdD.Parameters("@TOTAL_Harga").Value = totalHargaRow
                            cmdD.Parameters("@ID_USER").Value = idUser
                            cmdD.Parameters("@ID_KOMPUTER").Value = idKomputer
                            cmdD.Parameters("@URUTAN").Value = idx + 1
                            cmdD.ExecuteNonQuery()

                            If idx > 0 Then sbSql.Append(",")
                            sbSql.Append("(@F" & idx & ",@T" & idx & ",@J" & idx & ",@L" & idx & ",@IB" & idx & ",@NB" & idx & ",@Q" & idx & ",@S" & idx & ",@IS" & idx & ",@TQ" & idx & ",@TR" & idx & ",@U" & idx & ",@K" & idx & ")")
                            cmdH.Parameters.AddWithValue("@F" & idx, idSO)
                            cmdH.Parameters.AddWithValue("@T" & idx, tgl)
                            cmdH.Parameters.AddWithValue("@J" & idx, "PENJUALAN")
                            cmdH.Parameters.AddWithValue("@L" & idx, LblLokasiBarang.Text)
                            cmdH.Parameters.AddWithValue("@IB" & idx, idBarangRow)
                            cmdH.Parameters.AddWithValue("@NB" & idx, namaBarangRow)
                            cmdH.Parameters.AddWithValue("@Q" & idx, qtyRow)
                            cmdH.Parameters.AddWithValue("@S" & idx, satuanRow)
                            cmdH.Parameters.AddWithValue("@IS" & idx, isiRow)
                            cmdH.Parameters.AddWithValue("@TQ" & idx, qtySatRow)
                            cmdH.Parameters.AddWithValue("@TR" & idx, totalHargaRow)
                            cmdH.Parameters.AddWithValue("@U" & idx, idUser)
                            cmdH.Parameters.AddWithValue("@K" & idx, idKomputer)

                            Dim updateStokField As String = If(LblLokasiBarang.Text = "TOKO", "PENJUALAN_TOKO", "PENJUALAN_GUDANG")
                            Dim queryUpd As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " + @TQUP" & idx & " WHERE ID_BARANG = @IBUP" & idx
                            Using cmdUpd As New MySqlCommand(queryUpd, conn, transaction)
                                cmdUpd.Parameters.AddWithValue("@TQUP" & idx, qtySatRow)
                                cmdUpd.Parameters.AddWithValue("@IBUP" & idx, idBarangRow)
                                cmdUpd.ExecuteNonQuery()
                            End Using
                            idx += 1
                        End If
                    Next
                    If idx > 0 Then
                        cmdH.CommandText = sbSql.ToString()
                        cmdH.ExecuteNonQuery()
                    End If
                End Using
            End Using

            transaction.Commit()
            ' MessageBox.Show("Sales Order berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' ── Cetak nota Sales Order ────────────────────────────────────────────
            Try
                Select Case BacaPengaturanPrinter("Jual", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetak(idSO)
                    Case "SELALU TANYA"
                        Dim resultCetak As DialogResult = MessageBox.Show(
                            "Apakah Anda ingin mencetak nota Sales Order?",
                            "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        If resultCetak = DialogResult.Yes Then LakukanCetak(idSO)
                    Case "TAMPILKAN DI MONITOR"
                        ModulePrinterJual.PreviewPenjualan(idSO, isSalesOrder:=True)
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak Sales Order. Anda bisa mencetak ulang nanti." & vbCrLf &
                                "Detail: " & ex.Message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try

            Dim wasEditMode As Boolean = Not IsModeTambahSO
            Kondisiawal()

            If wasEditMode Then
                TxtJenistransaksi.Text = "TambahSalesOrder"
                TxtFaktur.Text = ""
                Close()
            End If
        Catch ex As Exception
            Try : transaction?.Rollback() : Catch : End Try

            ' Bangun pesan detail untuk membantu diagnosis
            Dim mode As String = If(IsModeTambahSO, "TAMBAH", "EDIT")
            Dim faktur As String = If(String.IsNullOrEmpty(TxtFaktur.Text), "(kosong)", TxtFaktur.Text)
            Dim lokasi As String = If(String.IsNullOrEmpty(LblLokasiBarang.Text), "(kosong)", LblLokasiBarang.Text)

            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("Transaksi Sales Order dibatalkan (rollback).")
            sb.AppendLine()
            sb.AppendLine($"Mode    : {mode}")
            sb.AppendLine($"Faktur  : {faktur}")
            sb.AppendLine($"Lokasi  : {lokasi}")
            sb.AppendLine($"User    : {FormUtama.StatusNamaUser.Text}")
            sb.AppendLine()
            sb.AppendLine("Error   : " & ex.GetType().Name)
            sb.AppendLine("Pesan   : " & ex.Message)

            MessageBox.Show(sb.ToString(), "Gagal Simpan Sales Order", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            System.Windows.Forms.Cursor.Current = Cursors.Default
        End Try
    End Sub

    ' Subroutine untuk mengurangi duplikasi kode cetak Sales Order
    Private Sub LakukanCetak(noFaktur As String)
        If BacaPengaturanPrinter("Jual", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterJual.TanyaPilihPrinter(noFaktur, isSalesOrder:=True)
        Else
            ModulePrinterJual.CetakPenjualan(noFaktur, isSalesOrder:=True)
        End If
    End Sub


    Public Sub EditSOHeader()
        Dim kodepel As String = String.Empty
        Dim namaPelanggan As String = String.Empty
        Dim jenisPelanggan As String = String.Empty
        Dim lokasibarang As String = String.Empty
        Dim tglTransaksi As DateTime = DateTime.MinValue

        Dim SALES As String = String.Empty
        Dim NAMASALES As String = String.Empty
        Dim USER As String = String.Empty
        Dim KOMPUTER As String = String.Empty

        Dim queryString As String = "SELECT " &
                        "ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, " &
                        "ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER " &
                        "FROM sales_order WHERE ID_PENJUALAN = ?"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    kodepel = rd("ID_PELANGGAN").ToString()
                    namaPelanggan = rd("NAMA_PELANGGAN").ToString()
                    jenisPelanggan = rd("JENIS_PELANGGAN").ToString()
                    lokasibarang = rd("LOKASIBARANG").ToString()
                    tglTransaksi = Convert.ToDateTime(rd("TGL_TRANSAKSI"))

                    SALES = rd("ID_SALES").ToString()
                    NAMASALES = rd("NAMA_SALES").ToString()
                    USER = rd("ID_USER").ToString()
                    KOMPUTER = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using

        CmbPelanggan.SelectedIndex = CmbPelanggan.FindStringExact(namaPelanggan)
        LbLKodePel.Text = kodepel
        LblJenisPl.Text = jenisPelanggan
        LblLokasiBarang.Text = lokasibarang
        TerapkanModeDTP(DTPTgl, isEditMode:=True, tanggalEdit:=tglTransaksi)

        EditSO()

        LblSales.Text = SALES
        SetComboBoxValue(CmbSales, NAMASALES)
        TxtLogin.Text = USER
        TxtKomputer.Text = KOMPUTER

        SetupFocusToGrid()
        RefreshStokSemuaBaris()
    End Sub


    Public Sub EditSO()
        If TxtJenistransaksi.Text = "EditSalesOrder" Then
            Dim _tDetail As DateTime = DateTime.Now
            DgvDataTransaksi.Rows.Clear()

            Dim soDetail As DataTable = GetSODetail(TxtFaktur.Text)

            ' ✅ Kumpulkan semua ID_BARANG unik
            Dim semuaIdBarang As New List(Of String)()
            For Each row As DataRow In soDetail.Rows
                Dim id As String = row("ID_BARANG").ToString()
                If Not semuaIdBarang.Contains(id) Then semuaIdBarang.Add(id)
            Next

            Dim dictSatuan As New Dictionary(Of String, List(Of String))()
            Dim dictStok As New Dictionary(Of String, Tuple(Of Decimal, Decimal))()

            If semuaIdBarang.Count > 0 Then
                Dim isPartai As Boolean = (LblJenisPl.Text = "Partai")
                Dim inClause As String = String.Join(",", semuaIdBarang.Select(Function(x) "'" & x.Replace("'", "''") & "'"))

                Dim satuanKolom As String = If(isPartai,
                "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR",
                "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR")

                ' ✅ 1 query untuk satuan + stok
                Dim _tBatch As DateTime = DateTime.Now
                Using cmd As New MySqlCommand("SELECT ID_BARANG, STOK_TOKO, STOK_GUDANG, " & satuanKolom &
                                          " FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")", conn)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            Dim id As String = rd("ID_BARANG").ToString()
                            dictStok(id) = Tuple.Create(
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D))
                            Dim satuanList As New List(Of String)()
                            For i As Integer = 0 To 2
                                Dim s As String = rd(i + 3).ToString()
                                If Not String.IsNullOrEmpty(s) Then satuanList.Add(s)
                            Next
                            dictSatuan(id) = satuanList
                        End While
                    End Using
                End Using

                ' ✅ 1 query untuk qty satuan dari sales_order_detail
                Dim _tQty As DateTime = DateTime.Now
                Using cmd As New MySqlCommand("SELECT ID_BARANG, COALESCE(SUM(QTY_SATUAN),0) AS QTY_SATUAN " &
                                          "FROM sales_order_detail WHERE FAKTUR_JUAL = ? AND ID_BARANG IN (" & inClause & ") " &
                                          "GROUP BY ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            Dim id As String = rd("ID_BARANG").ToString()
                            Dim qty As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "QTY_SATUAN", 0D)
                            If dictStok.ContainsKey(id) Then
                                Dim st As Decimal = dictStok(id).Item1
                                Dim sg As Decimal = dictStok(id).Item2
                                If LblLokasiBarang.Text = "GUDANG" Then sg += qty Else st += qty
                                dictStok(id) = Tuple.Create(st, sg)
                            End If
                        End While
                    End Using
                End Using
            End If

            ' âœ… Isi grid tanpa query
            DgvDataTransaksi.SuspendLayout()
            For Each row As DataRow In soDetail.Rows
                Dim dgvRow As DataGridViewRow = DgvDataTransaksi.Rows(DgvDataTransaksi.Rows.Add())
                Dim idBarang As String = row("ID_BARANG").ToString()

                dgvRow.Cells("Kode").Value = idBarang
                dgvRow.Cells("NamaBarang").Value = row("NAMA_BARANG")
                dgvRow.Cells("HargaBeli").Value = row("HARGA_BELI")
                dgvRow.Cells("QTY").Value = row("QTY")
                dgvRow.Cells("Satuan").Value = row("SATUAN")
                dgvRow.Cells("Isi").Value = Math.Max(1, If(IsDBNull(row("ISI_SATUAN")), 1, Convert.ToInt32(row("ISI_SATUAN"))))
                dgvRow.Cells("Totalhargabeli").Value = row("HARGA_BELI_SATUAN")
                dgvRow.Cells("Harga").Value = row("HARGA_JUAL")
                dgvRow.Cells("QtySat").Value = row("QTY_SATUAN")
                dgvRow.Cells("DiskonPersen").Value = row("DISKON_PERSEN")
                dgvRow.Cells("DiskonRp").Value = row("DISKON_RP")
                dgvRow.Cells("TotalDiskon").Value = row("TOTAL_DISKON")
                dgvRow.Cells("TotalHarga").Value = row("TOTAL_HARGA")
                dgvRow.Cells("SerialNumber").Value = row("SERIAL_NUMBER")

                Dim comboCell As DataGridViewComboBoxCell = CType(dgvRow.Cells("Satuan"), DataGridViewComboBoxCell)
                comboCell.Items.Clear()
                If dictSatuan.ContainsKey(idBarang) Then comboCell.Items.AddRange(dictSatuan(idBarang).ToArray())

                If dictStok.ContainsKey(idBarang) Then
                    dgvRow.Cells("StokToko").Value = dictStok(idBarang).Item1
                    dgvRow.Cells("StokGudang").Value = dictStok(idBarang).Item2
                End If
            Next
            DgvDataTransaksi.ResumeLayout()

            UpdateSemuaTotal()
        End If
    End Sub

    Private Function GetSODetail(faktur As String) As DataTable
        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, SERIAL_NUMBER FROM sales_order_detail WHERE FAKTUR_JUAL = ?", conn)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", faktur)
            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    ' Ganti handler ChkTampilSN_CheckedChanged dengan versi yang menyimpan setting
    Private Sub ChkTampilSN_CheckedChanged(sender As Object, e As EventArgs) Handles ChkTampilSN.CheckedChanged
        Dim chk As CheckBox = TryCast(sender, CheckBox)
        If chk Is Nothing Then Return

        If DgvDataTransaksi.Columns.Contains("SerialNumber") Then
            DgvDataTransaksi.Columns("SerialNumber").Visible = chk.Checked
        End If

        ' Simpan preferensi user agar permanen antar sesi
        Try
            AppConfig.Instance.SetValue("TampilSN", chk.Checked)
        Catch ex As Exception
            ' Jangan ganggu UX bila save gagal; log jika perlu
        End Try
    End Sub


    ''' <summary>
    ''' HELPER 3: SetComboBoxValue - Safe ComboBox selection
    ''' Menggantikan CmbPelanggan.SelectedIndex = FindStringExact() pattern
    ''' </summary>
    Private Sub SetComboBoxValue(cmb As ComboBox, value As String)
        If cmb Is Nothing Then Exit Sub

        If String.IsNullOrEmpty(value) Then
            cmb.SelectedIndex = -1
            Exit Sub
        End If

        Try
            Dim index As Integer = cmb.FindStringExact(value)
            If index >= 0 Then
                cmb.SelectedIndex = index
            Else
                cmb.SelectedIndex = -1
            End If
        Catch
            cmb.SelectedIndex = -1
        End Try
    End Sub

    ''' <summary>
    ''' HELPER 12: ParseDecimal dihapus Ã¢â‚¬â€ gunakan ModuleAngka.ParseDecimal
    ''' </summary>

    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "Jual"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub


    ''' <summary>
    ''' Tentukan level satuan (1=kecil, 2=sedang, 3=besar) dari qty
    ''' berdasarkan threshold dari General Setting.
    ''' Jika fitur nonaktif, selalu kembalikan 0 (tidak ada perubahan).
    ''' </summary>
    Private Function TentukanLevelDariQty(qty As Decimal) As Integer
        If Not ModulHakAkses.SettingAutoLevelSatuan Then Return 0 ' 0 = fitur nonaktif
        Dim batasSedang As Integer = ModulHakAkses.SettingBatasSatuanSedang
        Dim batasBesar As Integer = ModulHakAkses.SettingBatasSatuanBesar
        If qty >= batasBesar Then Return 3
        If qty >= batasSedang Then Return 2
        Return 1
    End Function

    ''' <summary>
    ''' Update satuan, isi, dan harga di baris DGV berdasarkan level (1/2/3).
    ''' Query DB untuk ambil Isi dan Harga sesuai level dan jenis pelanggan.
    ''' Dipanggil dari CellEndEdit kolom QTY saat auto level aktif.
    ''' </summary>
    Private Sub UpdateLevelSatuanBaris(rowIdx As Integer, level As Integer)
        If rowIdx < 0 OrElse rowIdx >= DgvDataTransaksi.Rows.Count Then Return
        Dim row = DgvDataTransaksi.Rows(rowIdx)
        Dim kode As String = If(row.Cells("Kode").Value IsNot Nothing, row.Cells("Kode").Value.ToString().Trim(), "")
        If String.IsNullOrEmpty(kode) Then Return

        Dim isPartai As Boolean = LblJenisPl.Text = "Partai"
        Dim prefix As String = If(isPartai, "PARTAI", "UMUM")
        Dim levelStr As String = If(level = 3, "BESAR", If(level = 2, "SEDANG", "KECIL"))

        Try
            Using cmd As New MySqlCommand(
                $"SELECT SATUAN_{prefix}_{levelStr}, ISI_{prefix}_{levelStr}, HARGA_JUAL_{prefix}_{levelStr} " &
                "FROM tbl_barang WHERE ID_BARANG = @kode LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim satuan As String = ModuleAngka.SafeGetValue(Of String)(rd, $"SATUAN_{prefix}_{levelStr}", "")
                        Dim isi As Integer = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, $"ISI_{prefix}_{levelStr}", 1))
                        Dim harga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, $"HARGA_JUAL_{prefix}_{levelStr}", 0D)

                        ' Hanya update jika satuan tersedia di ComboBox baris ini
                        Dim comboCell As DataGridViewComboBoxCell = TryCast(row.Cells("Satuan"), DataGridViewComboBoxCell)
                        If comboCell IsNot Nothing AndAlso Not String.IsNullOrEmpty(satuan) Then
                            If comboCell.Items.Contains(satuan) Then
                                comboCell.Value = satuan
                            End If
                        End If
                        row.Cells("Isi").Value = isi
                        row.Cells("Harga").Value = harga
                    End If
                End Using
            End Using
        Catch
            ' Jika gagal, biarkan nilai lama — jangan crash
        End Try
    End Sub

    ' Marquee handler
    Private Sub MarqueeElapsed(sender As Object, e As System.Timers.ElapsedEventArgs)
        _marqueeX -= _marqueeSpeed
        If _marqueeX < -_marqueeTextWidth Then
            _marqueeX = PanelHeader.Width - 35
        End If
        If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then
            Me.BeginInvoke(Sub() LblTextJalanAtas.Left = _marqueeX)
        End If
    End Sub

    Private Sub Form_SO_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If _timerMarquee IsNot Nothing Then
            _timerMarquee.Stop()
            RemoveHandler _timerMarquee.Elapsed, AddressOf MarqueeElapsed
            _timerMarquee.Dispose()
            _timerMarquee = Nothing
        End If
    End Sub

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                       "F1      : Tampilkan bantuan ini" & vbCrLf &
                       "F2      : Fokus ke Sales" & vbCrLf &
                       "F3      : Fokus ke Pelanggan" & vbCrLf &
                       "F4      : Buka form pilih Barang" & vbCrLf &
                       "F8      : Simpan transaksi" & vbCrLf &
                       "F12     : Buka form pilih Pelanggan" & vbCrLf &
                       "ESC     : Keluar" & vbCrLf &
                       "↓       : Pindah ke list hasil pencarian" & vbCrLf &
                       "ENTER   : Pilih item dari list" & vbCrLf &
                       "DELETE  : Hapus baris di grid"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                    MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub


End Class









