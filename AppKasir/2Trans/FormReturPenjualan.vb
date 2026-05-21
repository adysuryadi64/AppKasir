Public Class FormReturPenjualan
    Private jenisprintercetak As String = ""

    ' ── Pencarian inline DGV ──────────────────────────────────────────────────
    Private _dgvEditingTextBox As TextBox = Nothing
    Private _sedangPindahKeLstBarang As Boolean = False
    Private _rowSaatPindahKeLst As Integer = -1
    Private _sedangSetNilaiDariListBox As Boolean = False
    Private _sedangAmbilDariListBox As Boolean = False  ' blok CellEndEdit saat AmbilDataDariListBox berjalan
    Private _selectedQty As Decimal = 1D
    Private _formSudahSiap As Boolean = False           ' Guard: form belum boleh terima fokus
    Private _sedangSetFokusAwal As Boolean = False      ' Guard: cegah rekursi GotFocus

    ' ── Cache data barang dari nota penjualan (mode normal) ───────────────────
    Private Structure CacheBarangNota
        Public IdBarang As String
        Public NamaBarang As String
        Public HargaBeli As Decimal
        Public HargaJual As Decimal
        Public QtyTerjual As Decimal ' Qty dalam satuan kecil
        Public QtySudahRetur As Decimal ' Qty yang sudah diretur sebelumnya (satuan kecil)
        Public TotalDiskon As Decimal
        Public SatKecil As String
        Public IsiKecil As Integer
        Public SatSedang As String
        Public IsiSedang As Integer
        Public SatBesar As String
        Public IsiBesar As Integer
    End Structure
    Private _cacheBarangNota As New List(Of CacheBarangNota)()
    Private _cacheNota As String = ""
    ' ── Barcode detection ────────────────────────────────────────────────────
    Private isBarcodeMode As Boolean = False
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private barcodeTimer As New System.Windows.Forms.Timer()
    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100
    Private Sub FormReturPenjualan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Nilai keuangan otomatis via nama TxtGrandtotal
        ' Rename TxtTotalBarang/TxtTotalQTY/TxtTotalRupiah/TxtTotalLaba/TxtTotalJual/TxtSisaJual -> TxtGrandtotal untuk tema otomatis
        ModuleTheme.SetWarnaRtbCatatan(RTBAlasanRetur)
        AmbilJenisPrinter()
        ' Load preferensi CbJenisRetur sebelum Kondisiawalretur agar mode sudah benar
        CbJenisRetur.Checked = AppConfig.Instance.GetValue(Of Boolean)("ReturJual_JenisRetur", False)
        Kondisiawalretur()
        Datagrid()
        MuatSemuaPengaturan() ' Terapkan pengaturan hak akses dan sistem
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick
    End Sub

    Private Sub FormReturPenjualan_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        _formSudahSiap = True
        SetupFocusToGrid()
    End Sub

    ''' <summary>
    ''' Pusat kendali fokus — dipanggil dari Form_Shown, MuatSemuaPengaturan, Hapusbaris.
    ''' Smart focus: mempertimbangkan SettingFokusOtomatis DAN mode form (Normal vs Bebas).
    ''' </summary>
    Public Sub SetupFocusToGrid()
        ' Guard: form belum siap atau tidak visible
        If Not Me.Visible OrElse Me.WindowState = FormWindowState.Minimized Then Return
        If Not _formSudahSiap Then Return

        ' MODE 1: Fokus Otomatis — arahkan ke kontrol input header yang relevan
        If ModulHakAkses.SettingFokusOtomatis Then
            _sedangSetFokusAwal = True
            Me.BeginInvoke(New Action(Sub()
                                          If CbJenisRetur.Checked Then
                                              CmbNamaPel.Focus()  ' Mode bebas: langsung ke pelanggan
                                          Else
                                              TxtNotaJual.Focus() ' Mode normal: pilih nota dulu
                                          End If
                                          _sedangSetFokusAwal = False
                                      End Sub))
            Return
        End If

        ' MODE 2: Edit Langsung — fokus ke baris kosong di DGVReturjual
        NavigasiKeBarisDgvKosong()
    End Sub

    Private Sub Kondisiawalretur()
        Using newFontheader As New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            Dim headerCellStyle As New DataGridViewCellStyle With {
        .Font = newFontheader,
        .Alignment = DataGridViewContentAlignment.MiddleCenter
    }

            DGVPenjualan.ColumnHeadersDefaultCellStyle = headerCellStyle
            DGVReturjual.ColumnHeadersDefaultCellStyle = headerCellStyle
        End Using

        DTPtglJual.Value = DateTime.Now
        DTPtglJual.Format = DateTimePickerFormat.Custom
        DTPtglJual.CustomFormat = "dd/MM/yyyy"

        ModulHakAkses.ResetDTPKeTanggalHariIni(DTPRetur)
        DTPRetur.Format = DateTimePickerFormat.Custom
        DTPRetur.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        TxtNotaJual.Text = ""
        LblLokasi.Text = FormUtama.StatusLokasi.Text
        LblStatusJual.Text = ""
        LblTotalJual.Text = "Rp. 0"
        LblBayarJual.Text = "Rp. 0"
        LblSisaJual.Text = "Rp. 0"
        TxtTotalJual.Text = ""
        TxtBayarJual.Text = ""
        TxtSisaJual.Text = ""
        DGVReturjual.DataSource = Nothing
        DGVReturjual.Rows.Clear()
        DGVPenjualan.DataSource = Nothing
        DGVPenjualan.Rows.Clear()
        RTBAlasanRetur.Text = ""
        PanelInput1.Visible = False
        LblTotalBarang.Text = "Rp. 0"
        LblTotalQTY.Text = "Rp. 0"
        LblTotalRupiah.Text = "Rp. 0"
        TxtTotalBarang.Text = ""
        TxtTotalQTY.Text = ""
        TxtTotalRupiah.Text = ""
        TxtTotalLaba.Text = ""
        TxtHPP.Text = ""
        LblKodePel.Text = ""
        CmbNamaPel.Text = ""
        LblAlamatPel.Text = ""
        LblKontakPel.Text = ""
        LblJenisPel.Text = "Umum"
        ' Sesuaikan visibility berdasarkan mode retur
        If CbJenisRetur.Checked Then
            CbPotongHutang.Visible = False
            PanelInput2.Visible = False
            PanelInput.Visible = True
            DGVReturjual.Visible = True
            DGVReturjual.ReadOnly = False
            PanelFooter.Visible = True
        Else
            CbPotongHutang.Visible = True
            PanelInput2.Visible = True
            PanelInput.Visible = False
            DGVReturjual.Visible = True
            DGVReturjual.ReadOnly = True
            PanelFooter.Visible = False
        End If
        LstBarang.Visible = False
        LstBarang.Items.Clear()
        AmbilRekeningKasBank()
        CbTunai.Checked = True
        GenerateNomorReturPenjualan()
    End Sub

    Public Sub AmbilJenisPrinter()
        jenisprintercetak = AmbilCetakJenis("ReturJual")
    End Sub

    Private Sub DTPRetur_ValueChanged(sender As Object, e As EventArgs) Handles DTPRetur.ValueChanged
        GenerateNomorReturPenjualan()
    End Sub

    Private Sub GenerateNomorReturPenjualan()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "RP")
            cmd.Parameters.AddWithValue("@tgl", DTPRetur.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "retur_penjualan")
            cmd.Parameters.AddWithValue("@kolom", "ID_RETUR_PENJUALAN")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblNoNotaRetur.Text = pNomor.Value?.ToString()
        End Using
    End Sub


    Private Sub CenterPanelPencarian()
        Dim x As Integer = (ClientSize.Width - PanelInput1.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - PanelInput1.Height) \ 2
        PanelInput1.Location = New Point(x, y)
    End Sub

    Private Sub PBcariNotaBeli_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PBcariNotaJual.Click, TxtNotaJual.Click
        If CbJenisRetur.Checked Then Return
        DateTimePicker1.Value = DateTime.Now
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.CustomFormat = "dd/MM/yyyy"

        CenterPanelPencarian()
        PanelInput1.Visible = True

    End Sub

    Private Sub BtnHidePilihTanggal_Click(sender As Object, e As EventArgs) Handles BtnHidePilihTanggal.Click
        PanelInput1.Visible = False
    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged
        Dim tanggalAwal As Date = DateTimePicker1.Value.Date
        Dim tanggalAkhir As Date = DateTimePicker1.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, GRAND_TOTAL_STL_PAJAK, (BAYAR-KEMBALI) AS PEMBAYARAN, SISA_TAGIHAN, STATUS_TRANSAKSI FROM penjualan WHERE TGL_TRANSAKSI BETWEEN @tanggalAwal AND @tanggalAkhir AND LOKASIBARANG = @LOKASIBARANG  ORDER BY ID_PENJUALAN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasi.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    DGVPenjualan.SuspendLayout() ' Suspend layout untuk meningkatkan kinerja

                    DGVPenjualan.Rows.Clear()

                    Do While rd.Read()
                        DGVPenjualan.Rows.Add(rd("ID_PENJUALAN"), rd("ID_PELANGGAN"), rd("NAMA_PELANGGAN"), rd("JENIS_PELANGGAN"), rd("LOKASIBARANG"), rd("TGL_TRANSAKSI"), rd("GRAND_TOTAL_STL_PAJAK"), rd("PEMBAYARAN"), rd("SISA_TAGIHAN"), rd("STATUS_TRANSAKSI"))
                    Loop

                    DGVPenjualan.ResumeLayout() ' Lanjutkan layout setelah menambahkan baris
                Else
                    DGVPenjualan.Rows.Clear()
                End If
            End Using
        End Using

        ' ...

        With DGVPenjualan
            ' Pengaturan tampilan DataGridView
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False

            ' Loop untuk mengatur preferensi kolom
            For Each col As DataGridViewColumn In .Columns
                Select Case col.Name
                    Case "TGL_TRANSAKSI"
                        col.DefaultCellStyle.Format = "dd/MM/yyyy"
                End Select
            Next

            ' Mengubah nama header kolom
            .Columns("ID_PENJUALAN").HeaderText = "NO NOTA"
            .Columns("ID_PELANGGAN").Visible = False
            .Columns("NAMA_PELANGGAN").HeaderText = "PELANGGAN"
            .Columns("JENIS_PELANGGAN").HeaderText = "JENIS"
            .Columns("TGL_TRANSAKSI").HeaderText = "TANGGAL JUAL"
            .Columns("LOKASIBARANG").HeaderText = "LOKASI"
            .Columns("BAYAR").HeaderText = "PEMBAYARAN"
            .Columns("SISA_TAGIHAN").HeaderText = "HUTANG"
            .Columns("STATUS_TRANSAKSI").HeaderText = "STATUS"
        End With
        ModuleAngka.TerapkanFormatKolomAngka(DGVPenjualan, "GRAND_TOTAL_STL_PAJAK", "BAYAR", "SISA_TAGIHAN")

    End Sub

    Private Sub LblKodePel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblKodePel.TextChanged
        If LblKodePel.Text <> "" Then
            Using cmd As New MySqlCommand("SELECT ALAMAT, NO_TELP FROM tbl_pelanggan where KODE like @KODE", conn)
                cmd.Parameters.AddWithValue("@KODE", LblKodePel.Text) ' Move this line up

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        rd.Read()
                        LblAlamatPel.Text = rd("ALAMAT").ToString()
                        LblKontakPel.Text = rd("NO_TELP").ToString()
                    End If
                End Using
            End Using
        Else
            LblAlamatPel.Text = ""
            LblKontakPel.Text = ""
        End If
    End Sub

    Private Sub TxtNotaJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNotaJual.TextChanged
        DGVReturjual.DataSource = Nothing
        DGVReturjual.Rows.Clear()
        HitungSemua()
        ' Bersihkan cache saat nota berubah
        _cacheBarangNota.Clear()
        _cacheNota = ""
    End Sub

    Private Sub TxtNotaJual_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtNotaJual.KeyDown
        If e.KeyCode = Keys.Enter Then
            If Not String.IsNullOrEmpty(TxtNotaJual.Text) Then
                ' Jika nota sudah diisi, langsung fokus ke DGV untuk mulai retur
                NavigasiKeBarisDgvKosong()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    ''' <summary>Muat semua barang dari nota ke cache — satu kali baca DB, tanpa TRIM agar index efektif.</summary>
    Private Sub MuatCacheBarangNota(nota As String)
        If String.IsNullOrEmpty(nota) Then Return
        If _cacheNota = nota.Trim() AndAlso _cacheBarangNota.Count > 0 Then Return  ' sudah di-cache

        _cacheBarangNota.Clear()
        _cacheNota = nota.Trim()
        Debug.WriteLine($"[MuatCacheBarangNota] memuat nota='{_cacheNota}'")
        Try
            ' 1. Ambil data penjualan_detail
            Using cmd As New MySqlCommand(
                "SELECT pd.ID_BARANG, pd.NAMA_BARANG, pd.HARGA_BELI, pd.HARGA_JUAL, pd.QTY_SATUAN, pd.TOTAL_DISKON, " &
                "tb.SATUAN_UMUM_KECIL, tb.ISI_UMUM_KECIL, tb.SATUAN_UMUM_SEDANG, tb.ISI_UMUM_SEDANG, " &
                "tb.SATUAN_UMUM_BESAR, tb.ISI_UMUM_BESAR " &
                "FROM penjualan_detail pd " &
                "LEFT JOIN tbl_barang tb ON tb.ID_BARANG = pd.ID_BARANG " &
                "WHERE pd.FAKTUR_JUAL = @nota", conn)
                cmd.Parameters.AddWithValue("@nota", _cacheNota)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim item As New CacheBarangNota With {
                            .IdBarang = rd("ID_BARANG").ToString(),
                            .NamaBarang = rd("NAMA_BARANG").ToString(),
                            .HargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D),
                            .HargaJual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL", 0D),
                            .QtyTerjual = ModuleAngka.SafeGetValue(Of Decimal)(rd, "QTY_SATUAN", 0D),
                            .QtySudahRetur = 0D, ' Akan diisi di step 2
                            .TotalDiskon = ModuleAngka.SafeGetValue(Of Decimal)(rd, "TOTAL_DISKON", 0D),
                            .SatKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                            .IsiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)),
                            .SatSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                            .IsiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1)),
                            .SatBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", ""),
                            .IsiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                        }
                        _cacheBarangNota.Add(item)
                    End While
                End Using
            End Using

            ' 2. Ambil data akumulasi retur sebelumnya untuk nota ini
            If _cacheBarangNota.Count > 0 Then
                Using cmdRetur As New MySqlCommand(
                    "SELECT rpd.ID_BARANG, SUM(rpd.QTY_SATUAN) as TOTAL_RETUR " &
                    "FROM retur_penjualan_detail rpd " &
                    "INNER JOIN retur_penjualan rp ON rp.ID_RETUR_PENJUALAN = rpd.ID_RETUR_PENJUALAN " &
                    "WHERE rp.ID_PENJUALAN = @nota " &
                    "GROUP BY rpd.ID_BARANG", conn)
                    cmdRetur.Parameters.AddWithValue("@nota", _cacheNota)
                    Using rdR = cmdRetur.ExecuteReader()
                        While rdR.Read()
                            Dim idB = rdR("ID_BARANG").ToString()
                            Dim qtyR = ModuleAngka.SafeGetValue(Of Decimal)(rdR, "TOTAL_RETUR", 0D)
                            ' Update cache
                            Dim idx = _cacheBarangNota.FindIndex(Function(x) x.IdBarang = idB)
                            if idx >= 0 Then
                                Dim updated = _cacheBarangNota(idx)
                                updated.QtySudahRetur = qtyR
                                _cacheBarangNota(idx) = updated
                            End If
                        End While
                    End Using
                End Using
            End If
        Catch ex As Exception
            Debug.WriteLine($"[MuatCacheBarangNota] ERROR: {ex.Message}")
        End Try
        Debug.WriteLine($"[MuatCacheBarangNota] selesai: {_cacheBarangNota.Count} barang di-cache")
    End Sub


    Private Sub DGVPenjualan_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVPenjualan.CellClick
        If DGVPenjualan.Rows.Count >= 1 Then
            TxtNotaJual.Text = DGVPenjualan.Item(0, DGVPenjualan.CurrentRow.Index).Value
            LblKodePel.Text = DGVPenjualan.Item(1, DGVPenjualan.CurrentRow.Index).Value
            CmbNamaPel.Text = DGVPenjualan.Item(2, DGVPenjualan.CurrentRow.Index).Value.ToString()
            LblJenisPel.Text = DGVPenjualan.Item(3, DGVPenjualan.CurrentRow.Index).Value.ToString()
            DTPtglJual.Text = DGVPenjualan.Item(5, DGVPenjualan.CurrentRow.Index).Value

            ' Mengganti tipe Double dengan Decimal
            Dim total As Decimal
            If Not Decimal.TryParse(DGVPenjualan.Item(6, DGVPenjualan.CurrentRow.Index).Value.ToString(), total) Then
                total = 0D
            End If
            TxtTotalJual.Text = total.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            LblTotalJual.Text = "Rp. " & total.ToString("#,0.##", cultureIndonesia)

            Dim tagihan As Decimal
            If Not Decimal.TryParse(DGVPenjualan.Item(7, DGVPenjualan.CurrentRow.Index).Value.ToString(), tagihan) Then
                tagihan = 0D
            End If
            TxtBayarJual.Text = tagihan.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            LblBayarJual.Text = "Rp. " & tagihan.ToString("#,0.##", cultureIndonesia)

            Dim sisaBayar As Decimal
            If Not Decimal.TryParse(DGVPenjualan.Item(8, DGVPenjualan.CurrentRow.Index).Value.ToString(), sisaBayar) Then
                sisaBayar = 0D
            End If
            TxtSisaJual.Text = sisaBayar.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            LblSisaJual.Text = "Rp. " & sisaBayar.ToString("#,0.##", cultureIndonesia)

            LblStatusJual.Text = DGVPenjualan.Item(9, DGVPenjualan.CurrentRow.Index).Value
            PanelInput.Visible = True
            DGVReturjual.Visible = True
            DGVReturjual.ReadOnly = False
            PanelFooter.Visible = True
            PanelInput1.Visible = False
            ' Muat cache barang dari nota ini agar pencarian tidak baca DB berulang
            MuatCacheBarangNota(TxtNotaJual.Text)
        Else
            ' Tambahkan logika jika diperlukan
        End If
    End Sub

    Private Sub DGVReturjual_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles DGVReturjual.EditingControlShowing

        Dim titleText As String = DGVReturjual.Columns(1).HeaderText
        If titleText.Equals("NAMA BARANG") Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                Debug.WriteLine($"[EditingControlShowing] col=1 row={DGVReturjual.CurrentCell?.RowIndex} mode={If(CbJenisRetur.Checked, "BEBAS", "NORMAL")} nota='{TxtNotaJual.Text}'")
                If CbJenisRetur.Checked Then
                    autoText.AutoCompleteMode = AutoCompleteMode.None
                    autoText.AutoCompleteSource = AutoCompleteSource.None
                    If _dgvEditingTextBox IsNot Nothing Then
                        RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                        RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                    End If
                    _dgvEditingTextBox = autoText
                    AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                    PosisikanLstBarangDiBawahSel()
                    Debug.WriteLine($"[EditingControlShowing] handler terpasang, _dgvEditingTextBox={_dgvEditingTextBox IsNot Nothing}")
                Else
                    If String.IsNullOrEmpty(TxtNotaJual.Text) Then
                        Debug.WriteLine($"[EditingControlShowing] BLOKIR — nota kosong")
                        MessageBox.Show("Silahkan pilih nota penjualan terlebih dahulu (klik F1).", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        DGVReturjual.CancelEdit()
                        Return
                    End If
                    autoText.AutoCompleteMode = AutoCompleteMode.None
                    autoText.AutoCompleteSource = AutoCompleteSource.None
                    If _dgvEditingTextBox IsNot Nothing Then
                        RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                        RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                    End If
                    _dgvEditingTextBox = autoText
                    AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                    PosisikanLstBarangDiBawahSel()
                    Debug.WriteLine($"[EditingControlShowing] handler terpasang mode normal, nota='{TxtNotaJual.Text}'")
                End If
            End If
        End If

        If DGVReturjual.CurrentCell IsNot Nothing AndAlso DGVReturjual.CurrentCell.ColumnIndex = 4 Then
            If TypeOf e.Control Is ComboBox Then
                Dim cmb As ComboBox = DirectCast(e.Control, ComboBox)
                RemoveHandler cmb.SelectedIndexChanged, AddressOf DGVReturjual_SatuanChanged
                AddHandler cmb.SelectedIndexChanged, AddressOf DGVReturjual_SatuanChanged
            End If
        Else
            ' Kolom bukan NAMA BARANG dan bukan SATUAN
            If Not LstBarang.Focused Then
                LstBarang.Visible = False
                LstBarang.Items.Clear()
            End If
        End If

    End Sub

    Private Sub DGVReturjual_SatuanChanged(sender As Object, e As EventArgs)
        If DGVReturjual.CurrentCell Is Nothing Then Return
        Dim cmb As ComboBox = TryCast(sender, ComboBox)
        If cmb Is Nothing Then Return
        Dim rowIdx As Integer = DGVReturjual.CurrentCell.RowIndex
        If rowIdx < 0 Then Return
        Dim kode As String = Convert.ToString(DGVReturjual.Rows(rowIdx).Cells("ID_BARANG").Value).Trim()
        If String.IsNullOrWhiteSpace(kode) Then Return
        Dim options = AmbilSatuanByIdBarang(kode)
        If options.Count = 0 Then Return
        Dim idx As Integer = cmb.SelectedIndex
        If idx < 0 OrElse idx >= options.Count Then idx = 0
        DGVReturjual.Rows(rowIdx).Cells("ISI_SATUAN").Value = options(idx).Value
        HitungBaris(rowIdx)
        HitungSemua()
    End Sub

    Private Sub DGVReturjual_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DGVReturjual.CurrentCellDirtyStateChanged
        If DGVReturjual.IsCurrentCellDirty AndAlso TypeOf DGVReturjual.CurrentCell Is DataGridViewComboBoxCell Then
            DGVReturjual.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DGVReturjual_KeyDown(sender As Object, e As KeyEventArgs) Handles DGVReturjual.KeyDown
        ' Navigasi ↑↓ di kolom SATUAN untuk ganti satuan
        If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down) AndAlso
           DGVReturjual.CurrentCell IsNot Nothing AndAlso
           DGVReturjual.CurrentCell.ColumnIndex = DGVReturjual.Columns("SATUAN").Index Then
            Dim rowIdx As Integer = DGVReturjual.CurrentCell.RowIndex
            Dim kode As String = Convert.ToString(DGVReturjual.Rows(rowIdx).Cells("ID_BARANG").Value).Trim()
            If Not String.IsNullOrWhiteSpace(kode) Then
                Dim options = AmbilSatuanByIdBarang(kode)
                If options.Count > 1 Then
                    Dim comboCell = TryCast(DGVReturjual.Rows(rowIdx).Cells("SATUAN"), DataGridViewComboBoxCell)
                    If comboCell IsNot Nothing Then
                        Dim currentSatuan As String = Convert.ToString(comboCell.Value)
                        Dim currentIdx As Integer = options.FindIndex(Function(x) x.Key.Equals(currentSatuan, StringComparison.OrdinalIgnoreCase))
                        If currentIdx < 0 Then currentIdx = 0
                        Dim newIdx As Integer = If(e.KeyCode = Keys.Down,
                            Math.Min(currentIdx + 1, options.Count - 1),
                            Math.Max(currentIdx - 1, 0))
                        If newIdx <> currentIdx Then
                            DGVReturjual.Rows(rowIdx).Cells("SATUAN").Value = options(newIdx).Key
                            DGVReturjual.Rows(rowIdx).Cells("ISI_SATUAN").Value = options(newIdx).Value
                            HitungBaris(rowIdx)
                            HitungSemua()
                        End If
                        e.SuppressKeyPress = True
                        Return
                    End If
                End If
            End If
        End If

        ' Delete — hapus baris dengan konfirmasi
        If e.KeyCode = Keys.Delete Then
            If DGVReturjual.CurrentCell IsNot Nothing AndAlso DGVReturjual.CurrentCell.RowIndex >= 0 Then
                Dim rowIdx As Integer = DGVReturjual.CurrentCell.RowIndex
                If Not DGVReturjual.Rows(rowIdx).IsNewRow Then
                    If MessageBox.Show("Hapus baris ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        DGVReturjual.Rows.RemoveAt(rowIdx)
                        HitungSemua()
                    End If
                    e.SuppressKeyPress = True
                End If
            End If
        End If

        ' Enter — navigasi ke kolom berikutnya
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If DGVReturjual.CurrentCell Is Nothing Then Return
            Dim r As Integer = DGVReturjual.CurrentCell.RowIndex
            Dim c As Integer = DGVReturjual.CurrentCell.ColumnIndex
            Dim nextCol As Integer = c + 1
            ' Skip kolom hidden
            Do While nextCol < DGVReturjual.ColumnCount AndAlso Not DGVReturjual.Columns(nextCol).Visible
                nextCol += 1
            Loop
            If nextCol >= DGVReturjual.ColumnCount Then
                nextCol = 1 : r += 1
                If r >= DGVReturjual.RowCount Then Return
            End If
            Try
                DGVReturjual.CurrentCell = DGVReturjual(nextCol, r)
            Catch
            End Try
        End If
    End Sub

    Private Sub DGVReturjual_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DGVReturjual.CellEnter
        If e.RowIndex < 0 Then Return
        If e.ColumnIndex = 1 Then
            UpdateWarnaKodeBarang(e.RowIndex)
        End If
        ' Auto-open dropdown saat fokus masuk ke kolom SATUAN
        If e.ColumnIndex = 4 Then
            Dim kode As String = Convert.ToString(DGVReturjual.Rows(e.RowIndex).Cells("ID_BARANG").Value).Trim()
            If Not String.IsNullOrWhiteSpace(kode) Then
                Me.BeginInvoke(New Action(Sub()
                    Try
                        DGVReturjual.BeginEdit(True)
                        Dim cmb As ComboBox = TryCast(DGVReturjual.EditingControl, ComboBox)
                        If cmb IsNot Nothing Then cmb.DroppedDown = True
                    Catch
                    End Try
                End Sub))
            End If
        End If
    End Sub

    Private Sub DGVReturjual_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles DGVReturjual.DataError
        e.ThrowException = False
    End Sub

    Private Sub DGVReturjual_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVReturjual.CellEndEdit
        ' ---> SOLUSI BUG BARCODE: Bersihkan handler TextBox DGV setiap kali selesai edit sel <---
        If _dgvEditingTextBox IsNot Nothing Then
            RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
            RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
            _dgvEditingTextBox = Nothing
        End If
        ResetBarcodeDetection()

        Debug.WriteLine($"[CellEndEdit] col={e.ColumnIndex} row={e.RowIndex} _sedangSetNilai={_sedangSetNilaiDariListBox} LstVisible={LstBarang.Visible}")
        '========================== Nama — identik FormTransferCabang CellEndEdit
        If e.ColumnIndex = 1 Then
            ' KRITIS: jika LstBarang masih visible, user sedang memilih dari list
            ' jangan proses CellEndEdit — AmbilDataDariListBox yang akan handle
            If LstBarang.Visible OrElse _sedangAmbilDariListBox Then
                Debug.WriteLine($"[CellEndEdit] SKIP — LstBarang.Visible={LstBarang.Visible} _sedangAmbil={_sedangAmbilDariListBox}")
                Return
            End If
            If DGVReturjual.Rows(e.RowIndex).IsNewRow Then
                Debug.WriteLine($"[CellEndEdit] IsNewRow — skip")
                Return
            End If
            Dim inputText As String = Convert.ToString(DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value).Trim()
            Debug.WriteLine($"[CellEndEdit] inputText='{inputText}'")
            If String.IsNullOrWhiteSpace(inputText) Then Return

            If Not CbJenisRetur.Checked AndAlso String.IsNullOrEmpty(TxtNotaJual.Text) Then
                Debug.WriteLine($"[CellEndEdit] BLOKIR — mode normal, nota kosong")
                DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = ""
                Hapusbaris()
                Return
            End If

            Dim qtyValue As Decimal = 1D
            Dim namaBarang As String = inputText
            Dim levelSatuan As Integer = 1 ' Default: Kecil

            ' Parsing Asterisk (Support: qty*nama ATAU qty*satuan*nama)
            If inputText.Contains("*"c) Then
                Dim parts As String() = inputText.Split("*"c)
                If parts.Length = 3 Then
                    ' FORMAT: qty*satuan*nama (Contoh: 5*DUS*INDOMIE)
                    qtyValue = ModuleAngka.ParseDecimal(parts(0).Trim())
                    Dim teksSatuan As String = parts(1).Trim()
                    namaBarang = parts(2).Trim()
                    ' Pendeteksian level satuan akan dilakukan di dalam IsiBarangKeRow
                    IsiBarangKeRow(e.RowIndex, namaBarang, qtyValue, inputSatuan:=teksSatuan)
                ElseIf parts.Length = 2 Then
                    ' FORMAT: qty*nama (Contoh: 5*INDOMIE)
                    qtyValue = ModuleAngka.ParseDecimal(parts(0).Trim())
                    namaBarang = parts(1).Trim()
                    IsiBarangKeRow(e.RowIndex, namaBarang, qtyValue)
                End If

                ' Update teks di cell agar bersih (hanya nama barang)
                DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = namaBarang
            Else
                ' Tanpa asterisk
                IsiBarangKeRow(e.RowIndex, namaBarang, qtyValue)
            End If

            Dim idHasil As String = Convert.ToString(DGVReturjual.Rows(e.RowIndex).Cells("ID_BARANG").Value).Trim()
            Debug.WriteLine($"[CellEndEdit] setelah IsiBarangKeRow: ID_BARANG='{idHasil}'")
            If String.IsNullOrEmpty(idHasil) Then
                Debug.WriteLine($"[CellEndEdit] barang tidak ditemukan → Hapusbaris")
                Hapusbaris()
                Return
            End If

            Debug.WriteLine($"[CellEndEdit] → NavigasiKeBarisDgvKosong(skip={e.RowIndex})")
            NavigasiKeBarisDgvKosong(e.RowIndex)
            Return
        End If

        '========================== QTY (kolom 3)
        If e.ColumnIndex = 3 Then
            Dim qty As Decimal = ModuleAngka.ParseDecimal(DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value?.ToString())
            If qty <= 0 Then
                MessageBox.Show("Qty harus angka lebih besar dari 0", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
                qty = 1D
            End If
            DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value = qty
            HitungBaris(e.RowIndex)
            HitungSemua()
            Return
        End If

        '========================== HARGA JUAL (kolom 8)
        If e.ColumnIndex = 8 Then
            Dim hj As Decimal = ModuleAngka.ParseDecimal(DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value?.ToString())
            DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value = hj
            Debug.WriteLine($"[CellEndEdit] HARGA_JUAL diubah manual → {hj}, tidak override dari DB")
            HitungBaris(e.RowIndex, False)  ' False = jangan override harga dari DB
            HitungSemua()
            Return
        End If

        '========================== TOTAL DISKON (kolom 9)
        If e.ColumnIndex = 9 Then
            Dim diskon As Decimal = ModuleAngka.ParseDecimal(DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value?.ToString())
            DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value = diskon
            HitungBaris(e.RowIndex, False)  ' False = jangan override harga dari DB
            HitungSemua()
            Return
        End If

        Datagrid()
        HitungSemua()
    End Sub

    Private Sub DGVReturPembelian_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs)
        e.Cancel = True
    End Sub
    Private Sub Datagrid()
        With DGVReturjual
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
        End With
        ' Tampilkan ComboBox hanya di cell aktif agar tidak membingungkan
        SATUAN.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
        SATUAN.DisplayStyleForCurrentCellOnly = True
        ModuleAngka.TerapkanFormatKolomAngka(DGVReturjual, "QTY", "QTY_SATUAN", "HARGA_BELI", "HARGA_BELI_SATUAN", "HARGA_JUAL", "TOTAL_DISKON", "TOTAL_HARGA")
    End Sub


    Private Sub Hapusbaris()
        Dim baris As Integer = DGVReturjual.CurrentCell.RowIndex

        ' Cek apakah baris yang dipilih adalah baris baru yang belum dikonfirmasi.
        If baris < DGVReturjual.Rows.Count - 1 AndAlso Not DGVReturjual.Rows(baris).IsNewRow Then
            ' Jika bukan baris baru, hapus baris tersebut.
            DGVReturjual.Rows.RemoveAt(baris)
        Else
            ' Batalkan pengeditan dan hapus baris baru.
            ' Pastikan untuk mengonfirmasi terlebih dahulu apakah baris bukan baris baru sebelum mencoba membatalkan edit.
            If DGVReturjual.IsCurrentCellInEditMode Then
                DGVReturjual.EndEdit()
            End If

            ' Hapus baris baru (setelah konfirmasi edit).
            DGVReturjual.Rows.RemoveAt(baris)
        End If

        ' Panggil fungsi-fungsi lainnya
        HitungSemua()
        SetupFocusToGrid()
    End Sub

    Private Sub HitungSemua()
        Dim totalBarang As Integer = DGVReturjual.RowCount - 1
        Dim totalQty As Decimal = 0
        Dim totalHPP As Decimal = 0
        Dim grandTotal As Decimal = 0
        Dim totalLaba As Decimal = 0

        For i As Integer = 0 To DGVReturjual.Rows.Count - 1
            If DGVReturjual.Rows(i).Cells("QTY_SATUAN").Value IsNot Nothing Then
                totalQty += ModuleAngka.ParseDecimal(DGVReturjual.Rows(i).Cells("QTY_SATUAN").Value)
            End If

            If DGVReturjual.Rows(i).Cells("HARGA_BELI_SATUAN").Value IsNot Nothing Then
                totalHPP += ModuleAngka.ParseDecimal(DGVReturjual.Rows(i).Cells("HARGA_BELI_SATUAN").Value)
            End If

            If DGVReturjual.Rows(i).Cells("TOTAL_HARGA").Value IsNot Nothing Then
                grandTotal += ModuleAngka.ParseDecimal(DGVReturjual.Rows(i).Cells("TOTAL_HARGA").Value)
                totalLaba += ModuleAngka.ParseDecimal(DGVReturjual.Rows(i).Cells("TOTAL_HARGA").Value) - ModuleAngka.ParseDecimal(DGVReturjual.Rows(i).Cells("HARGA_BELI_SATUAN").Value)
            End If
        Next

        ' Update hasil perhitungan ke textbox dan label
        TxtTotalBarang.Text = totalBarang.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblTotalBarang.Text = totalBarang.ToString("#,0.##", cultureIndonesia)

        TxtTotalQTY.Text = totalQty.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblTotalQTY.Text = totalQty.ToString("#,0.##", cultureIndonesia)

        TxtHPP.Text = totalHPP.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

        TxtTotalRupiah.Text = grandTotal.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblTotalRupiah.Text = "Rp. " & grandTotal.ToString("#,0.##", cultureIndonesia)

        TxtTotalLaba.Text = totalLaba.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
    End Sub



    Private Sub DGVReturPembelian_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs)
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            DGVReturjual.CurrentCell = DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG")
            Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
            CMSHapus.Show(cursorPosition)
        End If
    End Sub

    Private Sub TSMhapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TSMhapus.Click
        Call Hapusbaris()
    End Sub

    Private Sub AmbilRekeningKasBank()
        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")

        ' Set akun berdasarkan lokasi
        If LblLokasi.Text = "TOKO" Then
            CmbRekening.SelectedItem = nama_rek_Retur_Penjualan_Toko
            LblKodeAkun.Text = Kode_rek_Retur_Penjualan_Toko
        ElseIf LblLokasi.Text = "GUDANG" Then
            CmbRekening.SelectedItem = nama_rek_Retur_Penjualan_Gudang
            LblKodeAkun.Text = Kode_rek_Retur_Penjualan_Gudang
        End If
    End Sub

    Private Sub CbTunai_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTunai.CheckedChanged
        ' Cek apakah perubahan ini berasal dari interaksi pengguna
        If CbTunai.Checked Then
            CbPotongHutang.Checked = False
            AmbilRekeningKasBank()
        End If
    End Sub

    Private Sub CbPotongHutang_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPotongHutang.CheckedChanged
        ' Cek apakah perubahan ini berasal dari interaksi pengguna
        If CbPotongHutang.Checked Then
            CbTunai.Checked = False
            CmbRekening.Items.Clear()

            ' Query untuk mengambil akun dengan kode tertentu
            Dim queryAkun As String = "SELECT Nama_Akun FROM tbl_datareferensi WHERE Kode_akun LIKE '01.04.002'"
            Using cmd As New MySqlCommand(queryAkun, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            CmbRekening.Items.Add(rd("Nama_Akun").ToString())
                        End While
                    End If
                End Using
            End Using

            CmbRekening.SelectedItem = nama_rek_Piutang_Jual
            'CmbRekening.SelectedIndex = 0
            LblKodeAkun.Text = Kode_rek_Piutang_Jual

            Dim sisaJual As Decimal = ModuleAngka.ParseDecimal(TxtSisaJual.Text)
            Dim totalRupiah As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)
            LblStatusPiutang.Text = If(sisaJual = totalRupiah, "Lunas", "Belum Lunas")

        End If
    End Sub


    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        If CbTunai.Checked = True Then
            Dim namaAkunD As String = CmbRekening.Text
            Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        LblKodeAkun.Text = reader("Kode_akun").ToString()
                    End If
                End Using
            End Using
        End If
    End Sub

    Private Sub LblStatusJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblStatusJual.TextChanged
        If CbJenisRetur.Checked Then Return
        If LblLokasi.Text = "Lunas" Then
            CbPotongHutang.Visible = False
            CbTunai.Checked = True
        Else
            CbPotongHutang.Visible = True
        End If
    End Sub

    Private Sub TxtTotalRupiah_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtTotalRupiah.TextChanged
        If CbJenisRetur.Checked Then Return
        Dim totalRupiah As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)
        Dim sisaJual As Decimal = ModuleAngka.ParseDecimal(TxtSisaJual.Text)

        If totalRupiah > sisaJual Then
            CbPotongHutang.Visible = False
            CbTunai.Checked = True
        Else
            CbPotongHutang.Visible = True
        End If
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        If Not Validasi() Then
            Return ' Batalkan aksi jika validasi gagal
        End If

        If CekQtyRetur() Then
            ' Mulai transaksi
            Dim transaction As MySqlTransaction = Nothing

            Try
                If Not ModulHakAkses.SettingIzinkanTanggalLampau Then
                    ModulHakAkses.ResetDTPKeTanggalHariIni(DTPRetur)
                    GenerateNomorReturPenjualan()
                End If


                transaction = conn.BeginTransaction()

                ' Audit: inisialisasi dictionary
                Dim auditDGV As New Dictionary(Of String, Decimal)()
                Dim auditHistory As New Dictionary(Of String, Decimal)()
                Dim auditDetail As New Dictionary(Of String, Decimal)()
                Dim auditStokDelta As New Dictionary(Of String, Decimal)()

                ' Audit A: baca qty dari DGV (kolom 6 = QTY_SATUAN)
                For Each row As DataGridViewRow In DGVReturjual.Rows
                    If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                        Dim kodeA As String = row.Cells(0).Value.ToString()
                        Dim qtyA As Decimal = ModuleAngka.ParseDecimal(row.Cells(6).Value)
                        If auditDGV.ContainsKey(kodeA) Then auditDGV(kodeA) += qtyA Else auditDGV(kodeA) = qtyA
                        ' Audit C: detail retur = sama dengan DGV untuk retur penjualan
                        If auditDetail.ContainsKey(kodeA) Then auditDetail(kodeA) += qtyA Else auditDetail(kodeA) = qtyA
                    End If
                Next

                SimpanUpdatePiutangPenjualan(transaction)
                ' ========================================
                ' START: Audit Trail - Simpan Retur Penjualan
                ' ========================================
                ModuleAuditTrail.CatatAudit(LblNoNotaRetur.Text, "TAMBAH", "Retur Penjualan", ket:="Simpan retur penjualan baru", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Simpan Retur Penjualan
                ' ========================================
                Simpanreturpenjualan(transaction)

                ' ── Catat RETUR ke piutang_detail (hanya Mode Normal + PotongHutang) ──
                If Not CbJenisRetur.Checked AndAlso CbPotongHutang.Checked Then
                    ' Ambil data tambahan dari tabel penjualan (jatuh tempo)
                    Dim jatuhTempoAsal As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    Dim totalPiutangAsal As Decimal = ModuleAngka.ParseDecimal(TxtTotalJual.Text)
                    Dim totalRupiahRetur As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)

                    Try
                        Using cmdAmbilJatuhTempo As New MySqlCommand(
                            "SELECT JATUH_TEMPO FROM penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN LIMIT 1",
                            conn, transaction)
                            cmdAmbilJatuhTempo.Parameters.AddWithValue("@ID_PENJUALAN", TxtNotaJual.Text)
                            Dim hasilJatuhTempo = cmdAmbilJatuhTempo.ExecuteScalar()
                            If hasilJatuhTempo IsNot Nothing AndAlso Not IsDBNull(hasilJatuhTempo) Then
                                jatuhTempoAsal = Convert.ToDateTime(hasilJatuhTempo).ToString("yyyy-MM-dd HH:mm:ss")
                            End If
                        End Using
                    Catch
                        ' Jika gagal ambil jatuh tempo, gunakan tanggal sekarang
                    End Try

                    ' INSERT baris RETUR ke piutang_detail
                    Using cmdRetur As New MySqlCommand(
                        "INSERT INTO piutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_JUAL, KODE, NAMA, " &
                        "JENIS, TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, " &
                        "PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
                        "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_JUAL, @KODE, @NAMA, " &
                        "'RETUR', @TANGGAL_JUAL, @PIUTANG, 0, @RETUR_NILAI, 0, @JATUH_TEMPO, " &
                        "@PEMBAYARAN, 'Belum Lunas', @ID_USER, @ID_KOMPUTER)", conn, transaction)
                        cmdRetur.Parameters.AddWithValue("@ID_BAYAR", "RETUR-" & LblNoNotaRetur.Text)
                        cmdRetur.Parameters.AddWithValue("@TANGGAL_BAYAR", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdRetur.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
                        cmdRetur.Parameters.AddWithValue("@ID_JUAL", TxtNotaJual.Text)
                        cmdRetur.Parameters.AddWithValue("@KODE", LblKodePel.Text)
                        cmdRetur.Parameters.AddWithValue("@NAMA", CmbNamaPel.Text)
                        cmdRetur.Parameters.AddWithValue("@TANGGAL_JUAL", DTPtglJual.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdRetur.Parameters.AddWithValue("@PIUTANG", totalPiutangAsal)
                        cmdRetur.Parameters.AddWithValue("@RETUR_NILAI", totalRupiahRetur)
                        cmdRetur.Parameters.AddWithValue("@JATUH_TEMPO", jatuhTempoAsal)
                        cmdRetur.Parameters.AddWithValue("@PEMBAYARAN", totalRupiahRetur)
                        cmdRetur.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                        cmdRetur.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                        cmdRetur.ExecuteNonQuery()
                    End Using

                    ' Perbarui baris JUAL — kurangi HUTANG, tambah RETUR
                    Using cmdUpdateTimbul As New MySqlCommand(
                        "UPDATE piutang_detail SET " &
                        "HUTANG = HUTANG - @RETUR, " &
                        "RETUR = RETUR + @RETUR, " &
                        "STATUS = CASE WHEN (HUTANG - @RETUR) <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
                        "WHERE ID_JUAL = @ID_JUAL AND JENIS = 'JUAL'", conn, transaction)
                        cmdUpdateTimbul.Parameters.AddWithValue("@RETUR", totalRupiahRetur)
                        cmdUpdateTimbul.Parameters.AddWithValue("@ID_JUAL", TxtNotaJual.Text)
                        cmdUpdateTimbul.ExecuteNonQuery()
                        ' Jika baris JUAL tidak ditemukan (faktur lama), tidak error — lanjutkan
                    End Using
                End If
                ' ── Selesai catat RETUR ke piutang_detail ──────────────────────────

                Simpanreturpenjualandetail(transaction)
                HistoryBarang(transaction, auditHistory)   ' mengisi B
                Simpanjurnal(transaction)

                If CbPotongHutang.Checked Then
                    UpdatePiutangPelanggan(LblKodePel.Text, transaction)
                End If

                ' Recalculate stok + Audit D
                For Each row As DataGridViewRow In DGVReturjual.Rows
                    If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                        Dim kodeD As String = row.Cells(0).Value.ToString()
                        Dim stokSebelum As Decimal = BacaStokSaatIni(kodeD, LblLokasi.Text, transaction)
                        HitungStokPerubahan(kodeD, transaction)
                        Dim stokSesudah As Decimal = BacaStokSaatIni(kodeD, LblLokasi.Text, transaction)
                        Dim delta As Decimal = stokSesudah - stokSebelum  ' retur jual menambah stok
                        If auditStokDelta.ContainsKey(kodeD) Then auditStokDelta(kodeD) += delta Else auditStokDelta(kodeD) = delta
                    End If
                Next

                AuditStokTransaksi(LblNoNotaRetur.Text, "Retur Penjualan", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)

                ' Update saldo akun — incremental delta
                UpdateSaldoAkunDeltaDariFaktur(LblNoNotaRetur.Text, transaction)

                ' Commit transaksi jika berhasil
                transaction.Commit()

                ' Audit jurnal keseimbangan
                ' D = ReturPenjualan + Persediaan, K = Kas/Piutang + HPP → selalu seimbang
                Dim rjNominal As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text) + ModuleAngka.ParseDecimal(TxtHPP.Text)
                CatatJurnalTidakSeimbang(LblNoNotaRetur.Text, rjNominal, rjNominal, "Retur Penjualan",
                    {"ReturPenjualan", "Kas/Piutang", "Persediaan", "HPP"})


            Catch ex As Exception
                transaction.Rollback()

                ' Tampilkan pesan kesalahan kepada pengguna
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try


            Try
                Select Case BacaPengaturanPrinter("ReturJual", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakReturJual(LblNoNotaRetur.Text)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota retur penjualan?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakReturJual(LblNoNotaRetur.Text)
                        End If
                    Case "TAMPILKAN DI MONITOR"
                        ModulePrinterReturJual.PreviewReturJual(LblNoNotaRetur.Text)
                End Select
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan saat mencetak retur: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                Kondisiawalretur()
            End Try


        End If

    End Sub

    Private Sub LakukanCetakReturJual(noRetur As String)
        If BacaPengaturanPrinter("ReturJual", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterReturJual.TanyaPilihPrinterReturJual(noRetur)
        Else
            ModulePrinterReturJual.CetakReturJual(noRetur)
        End If

    End Sub

    Private Function Validasi() As Boolean
        If DGVReturjual.RowCount <= 1 Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If Not CbJenisRetur.Checked Then
            If TxtNotaJual.Text = "" Then
                MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        If TxtTotalRupiah.Text = "" Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If CmbRekening.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih metode pengembalian pembayaran", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' [SETTING] Wajib isi alasan retur jual (ModulHakAkses)
        If ModulHakAkses.SettingWajibAlasanReturJual AndAlso String.IsNullOrWhiteSpace(RTBAlasanRetur.Text) Then
            MessageBox.Show("Alasan retur wajib diisi sesuai kebijakan perusahaan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            RTBAlasanRetur.Focus()
            Return False
        End If

        ' [SETTING] Validasi Tanggal Transaksi (Backdate)
        If Not ModulHakAkses.ValidasiTanggalTransaksi(DTPRetur.Value) Then
            MessageBox.Show("Tanggal transaksi tidak valid! (Backdate tidak diizinkan oleh sistem)", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            DTPRetur.Focus()
            Return False
        End If

        If Not CbJenisRetur.Checked Then
            If CbPotongHutang.Visible Then
                Dim totalRupiah As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)
                Dim sisaJual As Decimal = ModuleAngka.ParseDecimal(TxtSisaJual.Text)
                If totalRupiah > sisaJual Then
                    MessageBox.Show("Jumlah nilai retur melebihi piutang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        End If

        Return True
    End Function


    Private Function CekQtyRetur() As Boolean
        ' Jika mode retur bebas, tidak perlu cek qty terhadap nota
        If CbJenisRetur.Checked Then Return True

        ' Loop untuk validasi qty retur vs qty jual
        For Each row As DataGridViewRow In DGVReturjual.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim kodeBarang As String = row.Cells(0).Value.ToString()
                Dim qtyReturSatuan As Decimal = ModuleAngka.ParseDecimal(row.Cells("QTY_SATUAN").Value)

                ' Cari di cache
                Dim itemCache = _cacheBarangNota.FirstOrDefault(Function(x) x.IdBarang = kodeBarang)
                If itemCache.IdBarang IsNot Nothing Then
                    Dim sisaBisaDiretur = itemCache.QtyTerjual - itemCache.QtySudahRetur
                    If qtyReturSatuan > sisaBisaDiretur Then
                        MessageBox.Show($"Jumlah retur untuk barang '{row.Cells(1).Value}' melebihi batas!{vbCrLf}" &
                                        $"Dijual: {itemCache.QtyTerjual}{vbCrLf}" &
                                        $"Sudah Diretur: {itemCache.QtySudahRetur}{vbCrLf}" &
                                        $"Sisa Maksimal: {sisaBisaDiretur}{vbCrLf}" &
                                        $"Diminta: {qtyReturSatuan}",
                                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                Else
                    MessageBox.Show($"Barang '{row.Cells(1).Value}' tidak ditemukan dalam nota penjualan '{TxtNotaJual.Text}'!",
                                    "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        Next

        Return True
    End Function


    Private Sub SimpanUpdatePiutangPenjualan(ByVal transaction As MySqlTransaction)
        ' Jika retur bebas (tanpa nota penjualan), tidak perlu update tabel penjualan
        If CbJenisRetur.Checked Then Return

        Dim sql As String
        Dim returValue As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)

        If CbPotongHutang.Checked Then
            ' Potong Piutang: Update NILAI_RETUR, SISA_TAGIHAN, dan STATUS
            sql = "UPDATE PENJUALAN SET " &
                  "TGL_RETUR = @TGL_RETUR, " &
                  "NILAI_RETUR = NILAI_RETUR + @NILAI_RETUR, " &
                  "SISA_TAGIHAN = CASE WHEN SISA_TAGIHAN < @POTONGAN THEN 0 ELSE SISA_TAGIHAN - @POTONGAN END, " &
                  "STATUS_TRANSAKSI = @STATUS_TRANSAKSI " &
                  "WHERE ID_PENJUALAN = @ID_PENJUALAN"
        Else
            ' Normal/Tunai: Hanya update NILAI_RETUR
            sql = "UPDATE PENJUALAN SET " &
                  "TGL_RETUR = @TGL_RETUR, " &
                  "NILAI_RETUR = NILAI_RETUR + @NILAI_RETUR " &
                  "WHERE ID_PENJUALAN = @ID_PENJUALAN"
        End If

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@TGL_RETUR", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NILAI_RETUR", returValue)

            If CbPotongHutang.Checked Then
                cmd.Parameters.AddWithValue("@POTONGAN", returValue)
                cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI", LblStatusPiutang.Text)
            End If

            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtNotaJual.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Simpanreturpenjualan(ByVal transaction As MySqlTransaction)
        Dim query As String = "INSERT INTO retur_penjualan (ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, ALAMAT_PELANGGAN, KONTAK_PELANGGAN, ID_PENJUALAN, TGL_PENJUALAN, STATUS_PENJUALAN, PENYIMPANAN, BAYAR_PENJUALAN, HUTANG_PENJUALAN, TOTAL_BARANG, TOTAL_QTY, TOTAL_RUPIAH, NAMA_REKENING, KODE_REKENING, ALASAN_RETUR, ID_USER, ID_KOMPUTER) " &
                       "VALUES (@ID_RETUR_PENJUALAN, @TGL_RETUR_JUAL, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @ALAMAT_PELANGGAN, @KONTAK_PELANGGAN, @ID_PENJUALAN, @TGL_PENJUALAN, @STATUS_PENJUALAN, @PENYIMPANAN, @BAYAR_PENJUALAN, @HUTANG_PENJUALAN, @TOTAL_BARANG, @TOTAL_QTY, @TOTAL_RUPIAH, @NAMA_REKENING, @KODE_REKENING, @ALASAN_RETUR, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_RETUR_JUAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", LblKodePel.Text)
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbNamaPel.Text)
            cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPel.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", LblAlamatPel.Text)
            cmd.Parameters.AddWithValue("@KONTAK_PELANGGAN", LblKontakPel.Text)
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtNotaJual.Text)
            cmd.Parameters.AddWithValue("@TGL_PENJUALAN", DTPtglJual.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@STATUS_PENJUALAN", LblStatusJual.Text)
            cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@BAYAR_PENJUALAN", If(TxtBayarJual.Text <> "", ModuleAngka.ParseDecimal(TxtBayarJual.Text), 0D))
            cmd.Parameters.AddWithValue("@HUTANG_PENJUALAN", If(TxtSisaJual.Text <> "", ModuleAngka.ParseDecimal(TxtSisaJual.Text), 0D))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", ModuleAngka.ParseDecimal(TxtTotalBarang.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", ModuleAngka.ParseDecimal(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtTotalRupiah.Text))
            cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@KODE_REKENING", LblKodeAkun.Text)
            cmd.Parameters.AddWithValue("@ALASAN_RETUR", RTBAlasanRetur.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction, ByRef auditHistory As Dictionary(Of String, Decimal))
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVReturjual.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", LblNoNotaRetur.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "RETUR JUAL")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells(3).Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells(5).Value))
                    Dim totalQty As Decimal = ModuleAngka.ParseDecimal(row.Cells(6).Value)
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", totalQty)
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(row.Cells(10).Value))
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                    cmd.ExecuteNonQuery()
                End Using

                ' Audit B
                Dim kodeB As String = row.Cells(0).Value.ToString()
                Dim qtyB As Decimal = ModuleAngka.ParseDecimal(row.Cells(6).Value)
                If auditHistory.ContainsKey(kodeB) Then auditHistory(kodeB) += qtyB Else auditHistory(kodeB) = qtyB
            End If
        Next
    End Sub

    Private Sub Simpanreturpenjualandetail(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVReturjual.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim sqlrinci As String = "INSERT INTO retur_penjualan_detail (ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, LOKASI, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, QTY_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA, LABA, ID_USER, ID_KOMPUTER) VALUES " &
                                         "(@ID_RETUR_PENJUALAN, @TGL_RETUR_JUAL, @LOKASI, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @ID_BARANG, @NAMA_BARANG, @HARGA_BELI, @QTY, @SATUAN, @ISI_SATUAN, @QTY_SATUAN, @HARGA_BELI_SATUAN, @HARGA_JUAL, @TOTAL_DISKON, @TOTAL_HARGA, @LABA, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(sqlrinci, conn, transaction)
                    cmd.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", LblNoNotaRetur.Text)
                    cmd.Parameters.AddWithValue("@TGL_RETUR_JUAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_PELANGGAN", LblKodePel.Text)
                    cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbNamaPel.Text)
                    cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPel.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@HARGA_BELI", ModuleAngka.ParseDecimal(row.Cells(2).Value))
                    cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells(3).Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseDecimal(row.Cells(5).Value))
                    cmd.Parameters.AddWithValue("@QTY_SATUAN", ModuleAngka.ParseDecimal(row.Cells(6).Value))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", ModuleAngka.ParseDecimal(row.Cells(7).Value))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL", ModuleAngka.ParseDecimal(row.Cells(8).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_DISKON", ModuleAngka.ParseDecimal(row.Cells(9).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_HARGA", ModuleAngka.ParseDecimal(row.Cells(10).Value))
                    cmd.Parameters.AddWithValue("@LABA", ModuleAngka.ParseDecimal(row.Cells(10).Value) - ModuleAngka.ParseDecimal(row.Cells(7).Value))
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                    cmd.ExecuteNonQuery()
                End Using

                Dim updateStokField As String = String.Empty ' Inisialisasi dengan nilai default

                Select Case LblLokasi.Text
                    Case "TOKO"
                        updateStokField = "RETUR_JUAL_TOKO"
                    Case "GUDANG"
                        updateStokField = "RETUR_JUAL_GUDANG"
                    Case Else
                        Throw New InvalidOperationException("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " + @StokPengurangan WHERE ID_BARANG = @KodeBarang"

                Dim kodeBarang As String = row.Cells(0).Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim stokPengurangan As Decimal = ModuleAngka.ParseDecimal(row.Cells(6).Value)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End If
        Next
    End Sub

    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        Dim nominalKasBank As Decimal = ModuleAngka.ParseDecimal(TxtTotalRupiah.Text)
        Dim nominalHPP As Decimal = ModuleAngka.ParseDecimal(TxtHPP.Text)
        Dim uraian As String = "Retur penjualan dari " & CmbNamaPel.Text & " Jmlh Item " & TxtTotalBarang.Text & " Qty " & TxtTotalQTY.Text
        Dim tgl As String = DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Dim totalDebet As Decimal = nominalKasBank + nominalHPP
        Dim totalKredit As Decimal = nominalKasBank + nominalHPP

        Debug.WriteLine("═══════════════════════════════════════════════════════")
        Debug.WriteLine("DEBUG JURNAL RETUR PENJUALAN - Nota: " & LblNoNotaRetur.Text)
        Debug.WriteLine("═══════════════════════════════════════════════════════")
        Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12} {5,12}", "No", "Uraian", "Akun Debet", "Akun Kredit", "Debet", "Kredit"))
        Debug.WriteLine(New String("─"c, 115))

        ' ── J1: DEBIT Retur Penjualan (kontra pendapatan) ──────────────────────
        ' Mencatat pengurangan pendapatan akibat barang dikembalikan pelanggan
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_BANTU_D, KODE_BANTU_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                      "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_BANTU_D, @KODE_BANTU_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "RETUR PENJUALAN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "05.03.001")
            If CbTunai.Checked Then
                cmd.Parameters.AddWithValue("@NAMA_BANTU_D", DBNull.Value)
                cmd.Parameters.AddWithValue("@KODE_BANTU_D", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@NAMA_BANTU_D", CmbNamaPel.Text)
                cmd.Parameters.AddWithValue("@KODE_BANTU_D", LblKodePel.Text)
            End If
            cmd.Parameters.AddWithValue("@NOMINAL", nominalKasBank)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "RETUR PENJUALAN")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using
        Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12}", "J1", "Retur Penjualan", "RETUR PENJUALAN [05.03.001]", "-", nominalKasBank, "-"))

        ' ── J2: KREDIT KAS / PIUTANG ───────────────────────────────────────────
        ' Mencatat pengembalian uang (tunai) atau pengurangan piutang (kredit)
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                      "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_K, @KODE_BANTU_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LblKodeAkun.Text)
            If CbTunai.Checked Then
                cmd.Parameters.AddWithValue("@NAMA_BANTU_K", DBNull.Value)
                cmd.Parameters.AddWithValue("@KODE_BANTU_K", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@NAMA_BANTU_K", CmbNamaPel.Text)
                cmd.Parameters.AddWithValue("@KODE_BANTU_K", LblKodePel.Text)
            End If
            cmd.Parameters.AddWithValue("@NOMINAL", nominalKasBank)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "RETUR PENJUALAN")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using
        Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12} {5,12:N0}", "J2", "Kas/Piutang", "-", CmbRekening.Text & " [" & LblKodeAkun.Text & "]", "-", nominalKasBank))

        ' ── J3: DEBIT Persediaan Barang ────────────────────────────────────────
        ' Barang kembali masuk ke stok → persediaan bertambah
        If nominalHPP > 0 Then
            Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                          "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
                cmd.Parameters.AddWithValue("@URAIAN", uraian)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
                cmd.Parameters.AddWithValue("@NOMINAL", nominalHPP)
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "RETUR PENJUALAN")
                cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                cmd.ExecuteNonQuery()
            End Using
            Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12}", "J3", "Persediaan", NAMA_REK_BARANG & " [" & KODE_REK_BARANG & "]", "-", nominalHPP, "-"))

            ' ── J4: KREDIT HPP Pokok Penjualan ─────────────────────────────────
            ' Membalik HPP yang sudah dicatat saat penjualan
            Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                          "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
                cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
                cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
                cmd.Parameters.AddWithValue("@URAIAN", uraian)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", "HPP POKOK PENJUALAN")
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", "06.01.001")
                cmd.Parameters.AddWithValue("@NOMINAL", nominalHPP)
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "RETUR PENJUALAN")
                cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                cmd.ExecuteNonQuery()
            End Using
            Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12} {5,12:N0}", "J4", "HPP", "-", "HPP POKOK PENJUALAN [06.01.001]", "-", nominalHPP))
        End If

        Debug.WriteLine(New String("─"c, 115))
        Debug.WriteLine(String.Format("{0,-4} {1,-30} {2,-25} {3,-25} {4,12:N0} {5,12:N0}", "TOTAL", "", "", "", totalDebet, totalKredit))
        Debug.WriteLine(New String("═"c, 115))
        If totalDebet = totalKredit Then
            Debug.WriteLine("✅ JURNAL SEIMBANG - Debet = Kredit = " & totalDebet.ToString("N0"))
        Else
            Debug.WriteLine("❌ JURNAL TIDAK SEIMBANG! D:" & totalDebet.ToString("N0") & " K:" & totalKredit.ToString("N0") & " Selisih:" & (totalDebet - totalKredit).ToString("N0"))
        End If
        Debug.WriteLine("═══════════════════════════════════════════════════════")

    End Sub


    Private Sub FormReturPenjualan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F2
                ' F2: Buka panel pencarian nota jual
                If Not CbJenisRetur.Checked Then
                    DateTimePicker1.Value = DateTime.Now
                    DateTimePicker1.Format = DateTimePickerFormat.Custom
                    DateTimePicker1.CustomFormat = "dd/MM/yyyy"
                    CenterPanelPencarian()
                    PanelInput1.Visible = True
                End If
            Case Keys.F8
                BtnSimpan.PerformClick()
            Case Keys.F12
                BtnReset.PerformClick()

            Case Keys.Escape
                If PanelInput1.Visible = True Then
                    BtnHidePilihTanggal.PerformClick()
                Else
                    BtnKeluarForm.PerformClick()
                End If

        End Select
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReset.Click
        Kondisiawalretur()
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
        Close()
    End Sub

    Private Sub CbJenisRetur_CheckedChanged(sender As Object, e As EventArgs) Handles CbJenisRetur.CheckedChanged
        ' Simpan preferensi ke JSON
        AppConfig.Instance.SetValue("ReturJual_JenisRetur", CbJenisRetur.Checked)
        AppConfig.Instance.Save()

        If CbJenisRetur.Checked Then
            CbPotongHutang.Visible = False
            CbTunai.Checked = True
            PanelInput2.Visible = False
            PanelInput.Visible = True
            DGVReturjual.Visible = True
            DGVReturjual.ReadOnly = False
            PanelFooter.Visible = True
            PanelInput1.Visible = False
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            IsiCmbNamaPelanggan()
            TxtNotaJual.Text = ""
            DGVReturjual.Rows.Clear()
            HitungSemua()
        Else
            CbPotongHutang.Visible = True
            PanelInput2.Visible = True
            PanelInput.Visible = False
            PanelFooter.Visible = False
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            DGVReturjual.Rows.Clear()
            HitungSemua()
        End If
        MuatSemuaPengaturan()
    End Sub
    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "ReturJual"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

    ''' <summary>Memuat semua pengaturan sistem dan hak akses dari ModulHakAkses.</summary>
    Private Sub MuatSemuaPengaturan()
        Try
            ' 1. Pengaturan Tanggal (Backdate)
            DTPRetur.Enabled = ModulHakAkses.SettingIzinkanTanggalLampau

            ' 2. Pengaturan Diskon per Item
            If DGVReturjual.Columns.Contains("TOTAL_DISKON") Then
                DGVReturjual.Columns("TOTAL_DISKON").Visible = ModulHakAkses.SettingIzinkanDiskonItem
            End If

            ' 3. Pengaturan Satuan Berbeda
            If DGVReturjual.Columns.Contains("SATUAN") Then
                DGVReturjual.Columns("SATUAN").ReadOnly = Not ModulHakAkses.SettingIzinkanSatuanBerbeda
            End If

            ' 4. Fokus Otomatis — delegasi ke SetupFocusToGrid
            SetupFocusToGrid()
        Catch ex As Exception
            Debug.WriteLine("Error MuatSemuaPengaturan: " & ex.Message)
        End Try
    End Sub

    ' ══════════════════════════════════════════════════════════════════════════
    ' Pencarian inline DGV — hanya aktif saat CbJenisRetur = True
    ' ══════════════════════════════════════════════════════════════════════════

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If LstBarang.Visible Then
            If LstBarang.Focused Then
                Select Case keyData
                    Case Keys.Escape
                        LstBarang.Visible = False
                        LstBarang.Items.Clear()
                        If _dgvEditingTextBox IsNot Nothing Then _dgvEditingTextBox.Focus()
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
                    _rowSaatPindahKeLst = If(DGVReturjual.CurrentCell IsNot Nothing, DGVReturjual.CurrentCell.RowIndex, -1)
                    LstBarang.Focus()
                    If LstBarang.Items.Count > 0 Then LstBarang.SelectedIndex = 0
                    _sedangPindahKeLstBarang = False
                    Return True
                Case Keys.Escape
                    LstBarang.Visible = False
                    LstBarang.Items.Clear()
                    Return True
                Case Keys.Enter
                    If LstBarang.Items.Count > 0 Then
                        ' Auto-select first item jika belum dipilih, lalu ambil data
                        If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                        AmbilDataDariListBox()
                    End If
                    Return True
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub DGVReturjual_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DGVReturjual.CellLeave
        If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return
        If LstBarang.Visible AndAlso e.RowIndex = _rowSaatPindahKeLst Then Return
        If LstBarang.Visible Then
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            _rowSaatPindahKeLst = -1
        End If
        ' Saat meninggalkan kolom NAMA_BARANG, bersihkan handler dan barcode timer
        If e.ColumnIndex = 1 Then
            If _dgvEditingTextBox IsNot Nothing Then
                RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
                _dgvEditingTextBox = Nothing
            End If
            ResetBarcodeDetection()
        End If
    End Sub

    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        If _sedangSetNilaiDariListBox Then Return
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return
        Dim currentText As String = txt.Text.Trim()
        Debug.WriteLine($"[DgvNamaBarang_TextChanged] text='{currentText}' mode={If(CbJenisRetur.Checked, "BEBAS", "NORMAL")}")
        If String.IsNullOrEmpty(currentText) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If
        ' Parse * separator — ambil bagian terakhir sebagai keyword pencarian
        Dim keyword As String = currentText
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            keyword = parts(parts.Length - 1).Trim()
        End If
        If keyword.Length < 2 OrElse Not keyword.Any(AddressOf Char.IsLetter) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If
        Dim hasil As New List(Of String)()
        Try
            If CbJenisRetur.Checked Then
                ' Mode bebas: cari dari tbl_barang
                Using cmd As New MySqlCommand(
                    "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND " &
                    "(TRIM(ID_BARANG) LIKE @q OR TRIM(NAMA_BARANG) LIKE @q OR " &
                    "TRIM(BARCODE_KECIL) LIKE @q OR TRIM(BARCODE_SEDANG) LIKE @q OR TRIM(BARCODE_BESAR) LIKE @q) " &
                    "ORDER BY NAMA_BARANG", conn)
                    cmd.Parameters.AddWithValue("@q", "%" & keyword & "%")
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            hasil.Add(rd("NAMA_BARANG").ToString())
                        End While
                    End Using
                End Using
            Else
                ' Mode normal: cari dari cache (sudah dimuat saat nota dipilih)
                If String.IsNullOrEmpty(TxtNotaJual.Text) Then
                    LstBarang.Items.Clear()
                    LstBarang.Visible = False
                    Return
                End If
                ' Pastikan cache sudah dimuat
                If _cacheNota <> TxtNotaJual.Text.Trim() OrElse _cacheBarangNota.Count = 0 Then
                    MuatCacheBarangNota(TxtNotaJual.Text)
                End If
                ' Filter dari cache — tidak ada DB call
                Dim kw = keyword.ToLower()
                For Each item In _cacheBarangNota
                    If item.NamaBarang.ToLower().Contains(kw) Then
                        If Not hasil.Contains(item.NamaBarang) Then
                            hasil.Add(item.NamaBarang)
                        End If
                    End If
                Next
            End If
        Catch
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
            Debug.WriteLine($"[DgvNamaBarang_TextChanged] LstBarang tampil dengan {LstBarang.Items.Count} item")
        Else
            LstBarang.Visible = False
            Debug.WriteLine($"[DgvNamaBarang_TextChanged] tidak ada hasil")
        End If
    End Sub

    Private Sub DgvNamaBarang_KeyDown(sender As Object, e As KeyEventArgs)
        Dim ch As Char = ChrW(e.KeyCode)

        ' ── Barcode detection (mode bebas DAN mode normal) ───────────────────
        If Not LstBarang.Visible Then
            If Not Char.IsControl(ch) Then
                If ch = "*"c OrElse Char.IsLetter(ch) Then
                    ResetBarcodeDetection()
                Else
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
            End If
        End If

        ' ── Navigasi LstBarang ───────────────────────────────────────────────
        If Not LstBarang.Visible Then Return
        Select Case e.KeyCode
            Case Keys.Down
                _sedangPindahKeLstBarang = True
                LstBarang.Focus()
                If LstBarang.Items.Count > 0 Then LstBarang.SelectedIndex = 0
                _sedangPindahKeLstBarang = False
                e.SuppressKeyPress = True
                e.Handled = True
            Case Keys.Escape
                LstBarang.Visible = False
                LstBarang.Items.Clear()
                e.SuppressKeyPress = True
            Case Keys.Enter
                e.SuppressKeyPress = True
                e.Handled = True
        End Select
    End Sub

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
                    LstBarang.Visible = False
                    LstBarang.Items.Clear()
                    If _dgvEditingTextBox IsNot Nothing Then _dgvEditingTextBox.Focus()
                    e.SuppressKeyPress = True
                End If
            Case Keys.Escape
                LstBarang.Visible = False
                LstBarang.Items.Clear()
                If _dgvEditingTextBox IsNot Nothing Then _dgvEditingTextBox.Focus()
                e.SuppressKeyPress = True
        End Select
    End Sub

    Private Sub LstBarang_MouseClick(sender As Object, e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then AmbilDataDariListBox()
    End Sub

    Private Sub AmbilDataDariListBox()
        Dim selectedValue As String = ""
        If LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        ElseIf LstBarang.SelectedItem IsNot Nothing Then
            selectedValue = LstBarang.SelectedItem.ToString()
        End If
        Debug.WriteLine($"[AmbilDataDariListBox] selectedValue='{selectedValue}'")
        If String.IsNullOrEmpty(selectedValue) Then Return

        Dim idxArrow As Integer = selectedValue.IndexOf(" => ")
        Dim namayangdiambil As String = If(idxArrow >= 0, selectedValue.Substring(0, idxArrow).Trim(), selectedValue)
        Debug.WriteLine($"[AmbilDataDariListBox] namayangdiambil='{namayangdiambil}'")

        LstBarang.Visible = False
        LstBarang.Items.Clear()
        _rowSaatPindahKeLst = -1

        If DGVReturjual.CurrentCell IsNot Nothing AndAlso
           DGVReturjual.CurrentCell.ColumnIndex = 1 Then

            Dim originalInput As String = ""
            If _dgvEditingTextBox IsNot Nothing Then
                originalInput = _dgvEditingTextBox.Text.Trim()
            ElseIf DGVReturjual.CurrentCell.Value IsNot Nothing Then
                originalInput = DGVReturjual.CurrentCell.Value.ToString().Trim()
            End If
            Dim qtyValue As Decimal = 1D
            Dim levelValue As Integer = 1 ' Default Satuan Kecil

            If originalInput.Contains("*"c) Then
                Dim parts = originalInput.Split("*"c)
                If parts.Length >= 3 Then
                    ' Pola: qty*level*nama (contoh: 10*2*indomie)
                    qtyValue = ModuleAngka.ParseDecimal(parts(0).Trim())
                    levelValue = ModuleAngka.ParseInteger(parts(1).Trim())
                ElseIf parts.Length = 2 Then
                    ' Pola: qty*nama (contoh: 5*indomie)
                    qtyValue = ModuleAngka.ParseDecimal(parts(0).Trim())
                    levelValue = 1
                End If
                If qtyValue <= 0 Then qtyValue = 1
                If levelValue < 1 Then levelValue = 1
                If levelValue > 3 Then levelValue = 3
            End If

            Dim barisDiisi As Integer = DGVReturjual.CurrentCell.RowIndex
            Debug.WriteLine($"[AmbilDataDariListBox] barisDiisi={barisDiisi} originalInput='{originalInput}' qty={qtyValue} level={levelValue}")

            _sedangAmbilDariListBox = True
            _sedangSetNilaiDariListBox = True
            DGVReturjual.CancelEdit()
            _sedangSetNilaiDariListBox = False

            _selectedQty = qtyValue
            Debug.WriteLine($"[AmbilDataDariListBox] → IsiBarangKeRow(row={barisDiisi}, nama='{namayangdiambil}', qty={qtyValue}, level={levelValue})")
            IsiBarangKeRow(barisDiisi, namayangdiambil, qtyValue, levelValue)
            _sedangAmbilDariListBox = False
            Debug.WriteLine($"[AmbilDataDariListBox] → NavigasiKeBarisDgvKosong(skip={barisDiisi})")
            NavigasiKeBarisDgvKosong(barisDiisi)
        Else
            Debug.WriteLine($"[AmbilDataDariListBox] SKIP — _dgvEditingTextBox={_dgvEditingTextBox IsNot Nothing} CurrentCell={DGVReturjual.CurrentCell?.ColumnIndex}")
        End If
    End Sub

    ''' <summary>Isi semua kolom baris DGV langsung dari DB — identik IsiBarangKeRow FormTransferCabang.
    ''' Mode bebas: dari tbl_barang. Mode normal: dari penjualan_detail dibatasi nota.</summary>
    Private Sub IsiBarangKeRow(rowIdx As Integer, namaBarang As String, qty As Decimal, Optional level As Integer = 1, Optional inputSatuan As String = "")
        Debug.WriteLine($"[IsiBarangKeRow] row={rowIdx} nama='{namaBarang}' qty={qty} level={level} mode={If(CbJenisRetur.Checked, "BEBAS", "NORMAL")}")
        If rowIdx < 0 OrElse rowIdx >= DGVReturjual.Rows.Count Then
            Debug.WriteLine($"[IsiBarangKeRow] SKIP — rowIdx out of range")
            Return
        End If
        If DGVReturjual.Rows(rowIdx).IsNewRow Then
            Debug.WriteLine($"[IsiBarangKeRow] SKIP — IsNewRow")
            Return
        End If

        ' Kumpulkan semua data ke variabel lokal dulu — hindari open reader saat set cell
        Dim idBarang As String = ""
        Dim namaBarangDb As String = namaBarang
        Dim hargaBeli As Decimal = 0D
        Dim hargaJual As Decimal = 0D
        Dim totalDiskon As Decimal = 0D
        Dim satKecil As String = "" : Dim isiKecil As Integer = 1
        Dim satSedang As String = "" : Dim isiSedang As Integer = 1
        Dim satBesar As String = "" : Dim isiBesar As Integer = 1
        Dim hKecil As Decimal = 0D : Dim hSedang As Decimal = 0D : Dim hBesar As Decimal = 0D

        Try
            If CbJenisRetur.Checked Then
                ' Mode bebas: ambil dari tbl_barang
                Using cmd As New MySqlCommand(
                    "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, " &
                    "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                    "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, " &
                    "SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, " &
                    "SATUAN_UMUM_BESAR, ISI_UMUM_BESAR " &
                    "FROM tbl_barang WHERE STATUS='Aktif' AND " &
                    "(TRIM(NAMA_BARANG)=@n OR BARCODE_KECIL=@n OR BARCODE_SEDANG=@n OR BARCODE_BESAR=@n) LIMIT 1", conn)
                    cmd.Parameters.AddWithValue("@n", namaBarang.Trim())
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If Not rd.Read() Then Return
                        idBarang = rd("ID_BARANG").ToString()
                        namaBarangDb = rd("NAMA_BARANG").ToString()
                        hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                        hKecil = If(LblJenisPel.Text = "Partai",
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D),
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D))
                        hSedang = If(LblJenisPel.Text = "Partai",
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_SEDANG", 0D),
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_SEDANG", 0D))
                        hBesar = If(LblJenisPel.Text = "Partai",
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_BESAR", 0D),
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_BESAR", 0D))

                        hargaJual = hKecil ' Default
                        satKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                        isiKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                        satSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                        isiSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                        satBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                        isiBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                    End Using
                End Using
            Else
                ' Mode normal: ambil dari cache (tidak baca DB)
                If _cacheNota <> TxtNotaJual.Text.Trim() OrElse _cacheBarangNota.Count = 0 Then
                    MuatCacheBarangNota(TxtNotaJual.Text)
                End If
                Dim found = _cacheBarangNota.FirstOrDefault(Function(x) x.NamaBarang.Equals(namaBarang.Trim(), StringComparison.OrdinalIgnoreCase))
                If String.IsNullOrEmpty(found.IdBarang) Then
                    Debug.WriteLine($"[IsiBarangKeRow] tidak ditemukan di cache untuk nama='{namaBarang}'")
                    Return
                End If
                idBarang = found.IdBarang
                namaBarangDb = found.NamaBarang
                hargaBeli = found.HargaBeli
                hargaJual = found.HargaJual
                totalDiskon = found.TotalDiskon
                satKecil = found.SatKecil
                isiKecil = found.IsiKecil
                satSedang = found.SatSedang
                isiSedang = found.IsiSedang
                satBesar = found.SatBesar
                isiBesar = found.IsiBesar
            End If
        Catch
            Return
        End Try

        If String.IsNullOrEmpty(idBarang) Then
            Debug.WriteLine($"[IsiBarangKeRow] barang tidak ditemukan untuk nama='{namaBarang}'")
            Return
        End If

        Debug.WriteLine($"[IsiBarangKeRow] ditemukan: ID='{idBarang}' hargaBeli={hargaBeli} hargaJual={hargaJual} diskon={totalDiskon} satKecil='{satKecil}'/{isiKecil} satSedang='{satSedang}'/{isiSedang} satBesar='{satBesar}'/{isiBesar}")

        ' Cek duplikat — gabungkan qty jika barang sama sudah ada di baris lain
        For Each row As DataGridViewRow In DGVReturjual.Rows
            If row.IsNewRow Then Continue For
            If row.Index = rowIdx Then Continue For
            If Convert.ToString(row.Cells("ID_BARANG").Value).Trim() = idBarang Then
                Dim newQty As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("QTY").Value)) + qty
                row.Cells("QTY").Value = newQty
                HitungBaris(row.Index)
                HitungSemua()
                ' Hapus baris yang baru diisi (duplikat)
                If Not DGVReturjual.Rows(rowIdx).IsNewRow Then DGVReturjual.Rows.RemoveAt(rowIdx)
                NavigasiKeBarisDgvKosong()
                Return
            End If
        Next

        ' Bangun options satuan
        Dim options As New List(Of KeyValuePair(Of String, Integer))()
        If Not String.IsNullOrWhiteSpace(satKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satKecil, isiKecil))
        If Not String.IsNullOrWhiteSpace(satSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satSedang, isiSedang))
        If Not String.IsNullOrWhiteSpace(satBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satBesar, isiBesar))
        If options.Count = 0 Then options.Add(New KeyValuePair(Of String, Integer)("PCS", 1))

        ' Tentukan indeks satuan berdasarkan level (1=Kecil, 2=Sedang, 3=Besar)
        Dim optIdx As Integer = level - 1

        ' Jika ada inputSatuan (dari parsing asterisk), cari levelnya berdasarkan nama satuan
        If Not String.IsNullOrWhiteSpace(inputSatuan) Then
            Dim searchSatuan As String = inputSatuan.Trim().ToUpper()
            If searchSatuan = satBesar.Trim().ToUpper() Then
                optIdx = 2
                If CbJenisRetur.Checked Then hargaJual = hBesar
            ElseIf searchSatuan = satSedang.Trim().ToUpper() Then
                optIdx = 1
                If CbJenisRetur.Checked Then hargaJual = hSedang
            ElseIf searchSatuan = satKecil.Trim().ToUpper() Then
                optIdx = 0
                If CbJenisRetur.Checked Then hargaJual = hKecil
            End If
        End If

        If optIdx < 0 Then optIdx = 0
        If optIdx >= options.Count Then optIdx = options.Count - 1

        ' Isi semua kolom baris
        Dim r = DGVReturjual.Rows(rowIdx)
        r.Cells("ID_BARANG").Value = idBarang
        r.Cells("NAMA_BARANG").Value = namaBarangDb
        r.Cells("HARGA_BELI").Value = hargaBeli
        r.Cells("HARGA_JUAL").Value = hargaJual
        r.Cells("TOTAL_DISKON").Value = totalDiskon
        TerapkanSatuanKeRow(r, options, options(optIdx).Key, options(optIdx).Value)
        r.Cells("QTY").Value = qty

        UpdateWarnaKodeBarang(rowIdx)
        HitungBaris(rowIdx)
        HitungSemua()
        Debug.WriteLine($"[IsiBarangKeRow] selesai row={rowIdx} ID='{idBarang}' QTY={qty} SATUAN='{DGVReturjual.Rows(rowIdx).Cells("SATUAN").Value}' ISI={DGVReturjual.Rows(rowIdx).Cells("ISI_SATUAN").Value} QTY_SAT={DGVReturjual.Rows(rowIdx).Cells("QTY_SATUAN").Value} TOTAL={DGVReturjual.Rows(rowIdx).Cells("TOTAL_HARGA").Value}")
    End Sub

    ''' <summary>Navigasi ke baris kosong paling atas di DGV, fokus ke kolom NAMA_BARANG.
    ''' Jika semua baris sudah terisi, tambah baris baru dulu.</summary>
    Private Sub NavigasiKeBarisDgvKosong(Optional skipRow As Integer = -1)
        Debug.WriteLine($"[NavigasiKeBarisDgvKosong] skipRow={skipRow} totalRows={DGVReturjual.Rows.Count}")
        For i As Integer = 0 To DGVReturjual.Rows.Count - 1
            If DGVReturjual.Rows(i).IsNewRow Then Continue For
            If i = skipRow Then Continue For
            Dim kodeVal As String = Convert.ToString(DGVReturjual.Rows(i).Cells("ID_BARANG").Value).Trim()
            If String.IsNullOrEmpty(kodeVal) Then
                Debug.WriteLine($"[NavigasiKeBarisDgvKosong] → baris kosong ditemukan di row={i}")
                Try
                    DGVReturjual.CurrentCell = DGVReturjual(1, i)
                    DGVReturjual.BeginEdit(True)
                Catch ex As Exception
                    Debug.WriteLine($"[NavigasiKeBarisDgvKosong] ERROR BeginEdit row={i}: {ex.Message}")
                End Try
                Return
            End If
        Next
        Debug.WriteLine($"[NavigasiKeBarisDgvKosong] semua terisi → Rows.Add()")
        Try
            Dim newIdx As Integer = DGVReturjual.Rows.Add()
            Debug.WriteLine($"[NavigasiKeBarisDgvKosong] baris baru di row={newIdx}")
            DGVReturjual.CurrentCell = DGVReturjual(1, newIdx)
            DGVReturjual.BeginEdit(True)
        Catch ex As Exception
            Debug.WriteLine($"[NavigasiKeBarisDgvKosong] ERROR Rows.Add: {ex.Message}")
        End Try
    End Sub

    ''' <summary>Warnai kolom NAMA_BARANG: hijau jika ID_BARANG sudah terisi, putih jika kosong.</summary>
    Private Sub UpdateWarnaKodeBarang(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= DGVReturjual.Rows.Count Then Return
        Dim kodeValue = DGVReturjual.Rows(rowIndex).Cells("ID_BARANG").Value
        Dim adaKode As Boolean = kodeValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(kodeValue.ToString())
        Dim cell = DGVReturjual.Rows(rowIndex).Cells("NAMA_BARANG")
        If adaKode Then
            ' Identik FormTransferCabang: ReadOnly=True agar CancelEdit tidak trigger CellEndEdit
            cell.ReadOnly = True
            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
            cell.Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
        Else
            cell.ReadOnly = False
            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
            cell.Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
        End If
    End Sub

    Private Sub PosisikanLstBarangDiBawahSel()
        If DGVReturjual.CurrentCell Is Nothing Then Return
        Try
            Dim cellRect = DGVReturjual.GetCellDisplayRectangle(
                DGVReturjual.CurrentCell.ColumnIndex, DGVReturjual.CurrentCell.RowIndex, True)
            Dim ptDgv = DGVReturjual.PointToScreen(New Point(cellRect.Left, cellRect.Bottom))
            Dim ptForm = Me.PointToClient(ptDgv)
            
            LstBarang.Width = Math.Max(300, cellRect.Width)
            
            ' Cek sisa ruang vertikal di bawah sel aktif untuk menentukan posisi LstBarang (Atas/Bawah)
            Dim spaceBelow As Integer = Me.ClientSize.Height - ptForm.Y
            If spaceBelow < LstBarang.Height + 40 Then
                ' Tampilkan di atas sel: Y = Bawah Sel - Tinggi Sel - Tinggi ListBox
                Dim targetY As Integer = ptForm.Y - cellRect.Height - LstBarang.Height
                LstBarang.Location = New Point(ptForm.X, targetY)
            Else
                ' Tampilkan di bawah sel
                LstBarang.Location = New Point(ptForm.X, ptForm.Y)
            End If
        Catch
        End Try
    End Sub

    ' ── Barcode detection (identik FormTransferCabang) ────────────────────────

    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsedSinceLastKey = (DateTime.Now - lastKeyTime).TotalMilliseconds
        If elapsedSinceLastKey > 100 Then
            barcodeTimer.Stop()
            Dim bufferText = New String(barcodeChars.ToArray())
            If bufferText.Length >= BARCODE_MIN_LENGTH Then
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    ' bukan barcode murni — abaikan, biarkan TextChanged handle
                    ResetBarcodeDetection()
                    Return
                End If
                ' Barcode murni dari DGV — cari langsung
                If CbJenisRetur.Checked Then
                    ' Mode bebas: cari dari tbl_barang
                    Dim namaBarang As String = CariNamaDariBarcode(bufferText)
                    If Not String.IsNullOrEmpty(namaBarang) Then
                        _sedangSetNilaiDariListBox = True
                        If _dgvEditingTextBox IsNot Nothing Then _dgvEditingTextBox.Text = namaBarang
                        _sedangSetNilaiDariListBox = False
                        LstBarang.Visible = False
                        LstBarang.Items.Clear()
                        ' Trigger CellEndEdit via commit
                        DGVReturjual.CommitEdit(DataGridViewDataErrorContexts.Commit)
                    Else
                        MessageBox.Show("Barcode '" & bufferText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Else
                    ' Mode normal: cari dari cache nota
                    If Not String.IsNullOrEmpty(TxtNotaJual.Text) Then
                        If _cacheNota <> TxtNotaJual.Text.Trim() OrElse _cacheBarangNota.Count = 0 Then
                            MuatCacheBarangNota(TxtNotaJual.Text)
                        End If
                        Dim namaBarang As String = CariNamaDariBarcodeDiCache(bufferText)
                        If Not String.IsNullOrEmpty(namaBarang) Then
                            _sedangSetNilaiDariListBox = True
                            If _dgvEditingTextBox IsNot Nothing Then _dgvEditingTextBox.Text = namaBarang
                            _sedangSetNilaiDariListBox = False
                            LstBarang.Visible = False
                            LstBarang.Items.Clear()
                            DGVReturjual.CommitEdit(DataGridViewDataErrorContexts.Commit)
                        Else
                            MessageBox.Show("Barcode '" & bufferText & "' tidak ditemukan dalam nota '" & TxtNotaJual.Text & "'!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End If
                End If
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

    Private Function CariNamaDariBarcode(barcodeText As String) As String
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND " &
                "(BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc) LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@bc", barcodeText)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then Return rd("NAMA_BARANG").ToString()
                End Using
            End Using
        Catch
        End Try
        Return ""
    End Function

    ''' <summary>Cari nama barang dari barcode di dalam cache nota (mode normal).
    ''' Tidak baca DB — hanya cocokkan ID_BARANG dari cache dengan barcode di tbl_barang.</summary>
    Private Function CariNamaDariBarcodeDiCache(barcodeText As String) As String
        If _cacheBarangNota.Count = 0 Then Return ""
        Try
            ' Kumpulkan semua ID_BARANG dari cache
            Dim idList As New List(Of String)()
            For Each item In _cacheBarangNota
                If Not idList.Contains(item.IdBarang) Then idList.Add(item.IdBarang)
            Next
            If idList.Count = 0 Then Return ""

            ' Cari barcode yang cocok di antara barang-barang dalam nota
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND " &
                "(BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc) AND " &
                "ID_BARANG IN (" & String.Join(",", idList.Select(Function(x) "'" & x.Replace("'", "''") & "'")) & ") LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@bc", barcodeText)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then Return rd("NAMA_BARANG").ToString()
                End Using
            End Using
        Catch
        End Try
        Return ""
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

    Private Sub SetQtyOnly(qtyStr As String)
        Dim q As Decimal
        If Decimal.TryParse(qtyStr.Trim(), q) AndAlso q > 0 Then _selectedQty = q
    End Sub

    ' ── Helper terpusat: HitungBaris, TerapkanSatuanKeRow, AmbilSatuanByIdBarang ──

    Private Sub HitungBaris(rowIdx As Integer, Optional updateHargaJualDariDb As Boolean = True)
        If rowIdx < 0 OrElse rowIdx >= DGVReturjual.Rows.Count Then Return
        Dim row = DGVReturjual.Rows(rowIdx)
        If row.IsNewRow Then Return

        Dim qty As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("QTY").Value))
        Dim isi As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("ISI_SATUAN").Value))
        Dim qtySat As Decimal = qty * isi

        Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("HARGA_BELI").Value))
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("HARGA_JUAL").Value))
        Dim diskon As Decimal = 0D

        ' Skalasi Diskon: Jika Mode Normal, ambil diskon per unit dari cache
        If Not CbJenisRetur.Checked Then
            Dim idB As String = Convert.ToString(row.Cells("ID_BARANG").Value)
            Dim itemCache = _cacheBarangNota.FirstOrDefault(Function(x) x.IdBarang = idB)
            If itemCache.IdBarang IsNot Nothing AndAlso itemCache.QtyTerjual > 0 Then
                Dim diskonSatuan = itemCache.TotalDiskon / itemCache.QtyTerjual
                diskon = diskonSatuan * qtySat
            End If
        Else
            ' Mode Bebas: Gunakan diskon yang diinput manual (jika ada)
            diskon = ModuleAngka.ParseDecimal(Convert.ToString(row.Cells("TOTAL_DISKON").Value))
        End If

        ' Mode bebas: update harga jual dari DB hanya jika diminta
        ' (tidak update saat user sedang manual edit harga)
        If CbJenisRetur.Checked AndAlso updateHargaJualDariDb Then
            Dim kode As String = Convert.ToString(row.Cells("ID_BARANG").Value).Trim()
            Dim satuan As String = Convert.ToString(row.Cells("SATUAN").Value).Trim()
            If Not String.IsNullOrEmpty(kode) Then
                hargaJual = AmbilHargaJualBarangBySatuan(kode, LblJenisPel.Text, satuan)
                row.Cells("HARGA_JUAL").Value = hargaJual
            End If
        End If

        row.Cells("QTY").Value = qty
        row.Cells("ISI_SATUAN").Value = isi
        row.Cells("QTY_SATUAN").Value = qtySat
        row.Cells("TOTAL_DISKON").Value = diskon
        row.Cells("HARGA_BELI_SATUAN").Value = hargaBeli * qtySat
        row.Cells("TOTAL_HARGA").Value = (hargaJual * qtySat) - diskon
    End Sub

    Private Sub TerapkanSatuanKeRow(row As DataGridViewRow, options As List(Of KeyValuePair(Of String, Integer)), selectedSatuan As String, selectedIsi As Integer)
        Dim comboCell = TryCast(row.Cells("SATUAN"), DataGridViewComboBoxCell)
        If comboCell Is Nothing Then Return
        comboCell.Items.Clear()
        For Each opt In options
            comboCell.Items.Add(opt.Key)
        Next
        Dim satuanPakai = selectedSatuan
        If String.IsNullOrWhiteSpace(satuanPakai) OrElse Not options.Any(Function(x) x.Key.Equals(satuanPakai, StringComparison.OrdinalIgnoreCase)) Then
            satuanPakai = options(0).Key
            selectedIsi = options(0).Value
        End If
        row.Cells("SATUAN").Value = satuanPakai
        row.Cells("ISI_SATUAN").Value = Math.Max(1, selectedIsi)
    End Sub

    Private Function AmbilSatuanByIdBarang(idBarang As String) As List(Of KeyValuePair(Of String, Integer))
        Dim result As New List(Of KeyValuePair(Of String, Integer))()
        If String.IsNullOrWhiteSpace(idBarang) Then
            result.Add(New KeyValuePair(Of String, Integer)("PCS", 1))
            Return result
        End If
        Try
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
        Catch
        End Try
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

    ' ParseDecimalSafe lokal dihapus — gunakan ModuleAngka.ParseDecimal

    ' ── Pelanggan (mode CbJenisRetur = True) ─────────────────────────────────

    Private Sub IsiCmbNamaPelanggan()
        CmbNamaPel.Items.Clear()
        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA FROM tbl_pelanggan WHERE Status='Aktif' ORDER BY NAMA", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        CmbNamaPel.Items.Add(rd("NAMA").ToString())
                    End While
                End Using
            End Using
        Catch
        End Try
        CmbNamaPel.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        CmbNamaPel.AutoCompleteSource = AutoCompleteSource.ListItems
    End Sub

    Private Sub CmbNamaPel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbNamaPel.SelectedIndexChanged
        If Not CbJenisRetur.Checked Then Return
        If String.IsNullOrEmpty(CmbNamaPel.Text) Then Return
        AmbilDataPelangganDariNama(CmbNamaPel.Text)

        ' Pindah fokus ke DGV agar bisa langsung cari barang (Mode Bebas)
        NavigasiKeBarisDgvKosong()
    End Sub

    Private Sub CmbNamaPel_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbNamaPel.KeyDown
        If e.KeyCode = Keys.Enter Then
            If Not String.IsNullOrEmpty(CmbNamaPel.Text) Then
                AmbilDataPelangganDariNama(CmbNamaPel.Text)
                NavigasiKeBarisDgvKosong()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CmbNamaPel_Leave(sender As Object, e As EventArgs) Handles CmbNamaPel.Leave
        If Not CbJenisRetur.Checked Then Return
        If String.IsNullOrEmpty(CmbNamaPel.Text) Then Return
        AmbilDataPelangganDariNama(CmbNamaPel.Text)
    End Sub

    Private Sub AmbilDataPelangganDariNama(namaPelanggan As String)
        ' Kumpulkan data ke variabel lokal dulu — jangan set label di dalam reader
        ' karena LblKodePel.TextChanged akan coba buka reader baru di koneksi yang sama
        Dim kode As String = ""
        Dim jenis As String = ""
        Dim alamat As String = ""
        Dim telp As String = ""
        Try
            Using cmd As New MySqlCommand(
                "SELECT KODE, JENIS, ALAMAT, NO_TELP FROM tbl_pelanggan WHERE TRIM(NAMA)=@nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@nama", namaPelanggan.Trim())
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        kode = rd("KODE").ToString()
                        jenis = rd("JENIS").ToString()
                        alamat = rd("ALAMAT").ToString()
                        telp = rd("NO_TELP").ToString()
                    End If
                End Using
            End Using
        Catch
        End Try

        ' Set ke label setelah reader sudah ditutup
        If String.IsNullOrEmpty(kode) Then Return
        Dim jenisLama As String = LblJenisPel.Text
        LblKodePel.Text = kode
        LblJenisPel.Text = jenis
        LblAlamatPel.Text = alamat
        LblKontakPel.Text = telp
        ' Jika jenis berubah, update harga di semua baris DGV
        If jenisLama <> jenis AndAlso DGVReturjual.RowCount > 1 Then
            UpdateHargaSesuaiJenisPelanggan()
        End If
    End Sub

    Private Sub UpdateHargaSesuaiJenisPelanggan()
        For i As Integer = 0 To DGVReturjual.Rows.Count - 1
            Dim row = DGVReturjual.Rows(i)
            If row.IsNewRow Then Continue For
            Dim kode As String = Convert.ToString(row.Cells("ID_BARANG").Value).Trim()
            If String.IsNullOrEmpty(kode) Then Continue For
            Dim satuanPakai As String = Convert.ToString(row.Cells("SATUAN").Value).Trim()
            Dim hargaJual As Decimal = AmbilHargaJualBarangBySatuan(kode, LblJenisPel.Text, satuanPakai)
            row.Cells("HARGA_JUAL").Value = hargaJual
            HitungBaris(i)
        Next
        HitungSemua()
    End Sub

    Private Function AmbilHargaJualBarang(idBarang As String, jenisPelanggan As String) As Decimal
        Return AmbilHargaJualBarangBySatuan(idBarang, jenisPelanggan, "")
    End Function

    Private Function AmbilHargaJualBarangBySatuan(idBarang As String, jenisPelanggan As String, satuan As String) As Decimal
        Try
            Using cmd As New MySqlCommand(
                "SELECT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR " &
                "FROM tbl_barang WHERE ID_BARANG=@id LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@id", idBarang)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        If jenisPelanggan = "Partai" Then
                            If satuan = Convert.ToString(rd("SATUAN_PARTAI_SEDANG")) Then
                                Return ModuleAngka.ParseDecimal(rd("HARGA_JUAL_PARTAI_SEDANG"))
                            ElseIf satuan = Convert.ToString(rd("SATUAN_PARTAI_BESAR")) Then
                                Return ModuleAngka.ParseDecimal(rd("HARGA_JUAL_PARTAI_BESAR"))
                            Else
                                Return ModuleAngka.ParseDecimal(rd("HARGA_JUAL_PARTAI_KECIL"))
                            End If
                        Else
                            If satuan = Convert.ToString(rd("SATUAN_UMUM_SEDANG")) Then
                                Return ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_SEDANG"))
                            ElseIf satuan = Convert.ToString(rd("SATUAN_UMUM_BESAR")) Then
                                Return ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_BESAR"))
                            Else
                                Return ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL"))
                            End If
                        End If
                    End If
                End Using
            End Using
        Catch
        End Try
        Return 0D
    End Function

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F2      : Buka pencarian nota penjualan" & vbCrLf &
                           "F8      : Simpan retur penjualan" & vbCrLf &
                           "F12     : Reset form" & vbCrLf &
                           "ESC     : Tutup panel pencarian / Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnKeluarForm_Click(sender As Object, e As EventArgs) Handles BtnKeluarForm.Click
        Me.Close()
    End Sub
End Class
