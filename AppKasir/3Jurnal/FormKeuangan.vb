Public Class FormKeuangan

#Region "Fields"
    ' ================= DATA BINDING =================
    Private bsKeuangan As New BindingSource()
    Private dtKeuangan As New DataTable()

    ' ================= FLAGS =================
    Private _isLoading As Boolean = False
    Private _currentFormState As FormState = FormState.Add
    Private _hakSimpan As Boolean = False
    Private _hakEdit As Boolean = False
    Private _hakHapus As Boolean = False

    Private _connLock As New Object() ' reserved

    ' ================= SETTINGS =================
    ' Setting dibaca langsung dari ModulHakAkses property

    ''' <summary>
    ''' Jika True, form dibuka dari luar (misal FormLapMutasiKeuangan) khusus mode Setor ke Bos.
    ''' Semua button jenis transaksi lain disembunyikan, BtnSetorBos langsung aktif.
    ''' </summary>
    Public Property ModeSetorBosOnly As Boolean = False
#End Region

#Region "Form Events"
    Private Sub FormKeuangan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' PanelInput = area input otomatis via nama PanelInput*
        ' PanelInput sudah konsisten dengan tema otomatis
        TerapkanWarnaToolbar()
        InitializeUI()
        ' Setting dibaca langsung dari ModulHakAkses property
        SetupDataGridViewBinding()
        GenerateTransactionId()
        LoadDataKeuangan()

        ' Mode khusus: hanya tampilkan Setor ke Bos
        If ModeSetorBosOnly Then
            For Each btn As Button In {BtnPemasukan, BtnPengeluaran, BtnBiaya,
                                       BtnPinjamSuplier, BtnPindahR, BtnPinjamPelanggan}
                btn.Visible = False
            Next
            BtnSetorBos.PerformClick()
        End If
    End Sub

    ''' <summary>
    ''' Override warna PanelUtility dan button di dalamnya agar sama dengan
    ''' pola toolbar FormUtama: panel = L_Toolbar/D_Toolbar, button = L_NavIdle/D_NavIdle.
    ''' Dipanggil setelah TerapkanTheme supaya tidak di-override balik oleh TerapkanKontrol generik.
    ''' </summary>
    Private Sub TerapkanWarnaToolbar()
        ' Panel toolbar — seamless dengan background form
        PanelUtility.BackColor = ModuleTheme.C(ModuleTheme.L_Toolbar, ModuleTheme.D_Toolbar)

        ' Button navigasi di toolbar — pakai SetNavButtonIdle dari ModuleTheme
        Dim navBtns As Button() = {BtnPemasukan, BtnPengeluaran, BtnBiaya,
                                   BtnSetorBos, BtnPinjamSuplier, BtnPindahR,
                                   BtnPinjamPelanggan}
        For Each btn As Button In navBtns
            ModuleTheme.SetNavButtonIdle(btn)
        Next

        ' BTNKeluar di toolbar — solid merah, sama persis dengan FormUtama
        BTNKeluar.BackColor = ModuleTheme.C(ModuleTheme.L_BtnSolidKeluar, ModuleTheme.D_BtnSolidKeluar)
        BTNKeluar.ForeColor = Color.White
        BTNKeluar.FlatAppearance.BorderColor = ModuleTheme.C(ModuleTheme.L_BtnBorder, ModuleTheme.D_BtnBorder)
        BTNKeluar.FlatAppearance.MouseOverBackColor = ModuleTheme.C(ModuleTheme.L_BtnSolidKeluarHover, ModuleTheme.D_BtnSolidKeluarHover)
        BTNKeluar.FlatAppearance.MouseDownBackColor = ModuleTheme.C(ModuleTheme.L_BtnSolidKeluarDown, ModuleTheme.D_BtnSolidKeluarDown)
    End Sub

    Private Sub TxtNominalKeuangan_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalKeuangan.TextChanged
        UpdateNominalDisplay()
    End Sub

    Private Sub UpdateNominalDisplay()
        Dim nominalValue As Double
        If Double.TryParse(TxtNominalKeuangan.Text, nominalValue) Then
            LblNominalKeuangan.Text = "Rp. " & nominalValue.ToString("N0")
        Else
            LblNominalKeuangan.Text = "Rp. 0"
        End If
    End Sub

    Private Sub DTPTglKeuangan_ValueChanged(sender As Object, e As EventArgs) Handles DTPTglKeuangan.ValueChanged
        GenerateTransactionId()
        LoadDataKeuangan()
        TxtUraianKeuangan.Focus()
        TxtUraianKeuangan.Select()
    End Sub

    Private Sub BTNKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTNKeluar.Click
        Close()
    End Sub
#End Region

#Region "UI Initialization"
    Private Sub InitializeUI()
        SetupTooltips()

        Dim JURNAL As Boolean() = ModulHakAkses.BacaHakAksesDariCache("JURNAL")
        _hakSimpan = JURNAL(1)
        _hakEdit = JURNAL(2)
        _hakHapus = JURNAL(3)
        BtnSimpanKeuangan.Visible = _hakSimpan

        PanelInput.Visible = False
        PanelRinciKeuangan.Visible = False

        SetInitialFormState()
    End Sub

    Private Sub SetInitialFormState()
        LblIdBayar.Text = ""
        TxtNoNota.Text = ""
        TxtUraianKeuangan.Text = ""
        TxtNominalKeuangan.Text = ""
        LblNominalKeuangan.Text = "Rp. 0"

        CmbDebetKeuangan.Items.Clear()
        CmbKreditKeuangan.Items.Clear()
        CmbBantuDKeuangan.Items.Clear()
        CmbBantuKKeuangan.Items.Clear()

        DTPTglKeuangan.Format = DateTimePickerFormat.Custom
        DTPTglKeuangan.CustomFormat = "dd/MM/yyyy"
        ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTglKeuangan)

        HideHelperPanels()
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

    Private Enum FormState
        Add
        Edit
    End Enum

    Private Sub SetButtonState(state As FormState)
        _currentFormState = state
        Select Case state
            Case FormState.Add
                BtnSimpanKeuangan.Text = "Simpan (F2)"
                BtnBatalKeuangan.Visible = False
            Case FormState.Edit
                BtnSimpanKeuangan.Text = "Update (F2)"
                BtnBatalKeuangan.Visible = True
        End Select
    End Sub
#End Region

#Region "Data Loading"
    Private Sub LoadDataKeuangan()
        If _isLoading Then Return
        Try
            Dim dt As DataTable = GetKeuanganData()
            bsKeuangan.DataSource = dt
            Dim total As Decimal = dt.AsEnumerable().Sum(Function(r)
                                                             Return If(IsDBNull(r("NOMINAL")), 0D, Convert.ToDecimal(r("NOMINAL")))
                                                         End Function)
            LblTotalNominal.Text = $"Total Nominal: Rp {total:N0}"
        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetKeuanganData() As DataTable
        Dim dt As New DataTable()
        Dim tanggalAwal As Date = DTPTglKeuangan.Value.Date
        Dim tanggalAkhir As Date = tanggalAwal.AddDays(1).AddTicks(-1)

        Dim sql As String =
            "SELECT NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
            "AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, " &
            "NAMA_BANTU_D, KODE_BANTU_D, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, ID_USER " &
            "FROM jurnalumum " &
            "WHERE TGL_TRANSAKSI BETWEEN @TANGGAL_AWAL AND @TANGGAL_AKHIR " &
            "AND JENIS_TRANSAKSI = @JENIS_TRANSAKSI"

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

    ' Query langsung ke DB — tbl_datareferensi kecil (~50-100 baris), tidak perlu cache.
    ' Multi-client safe: selalu dapat data terbaru.
    Private Function GetAkunData() As DataTable
        Dim dt As New DataTable()
        Using cmd As New MySqlCommand(
            "SELECT Type_Akun, Nama_Akun, Kode_Akun FROM tbl_datareferensi ORDER BY Kode_Akun", conn)
            Using adapter As New MySqlDataAdapter(cmd)
                adapter.Fill(dt)
            End Using
        End Using
        Return dt
    End Function
#End Region

#Region "DataGridView Setup"
    Private Sub SetupDataGridViewBinding()
        DgvKeuangan.DataSource = bsKeuangan
        DgvKeuangan.AutoGenerateColumns = False
        SetupDataGridViewColumns()
        ModuleTheme.ApplyThemeDataGridView(DgvKeuangan)
    End Sub

    Private Sub SetupDataGridViewColumns()
        DgvKeuangan.Columns.Clear()

        If _hakEdit Then AddButtonColumn("EDIT", "✏ Edit", 80)
        If _hakHapus Then AddButtonColumn("HAPUS", "🗑 Hapus", 80)

        AddDataColumn("NO_TRANSAKSI", "No. Transaksi", 120)
        AddDataColumn("TGL_TRANSAKSI", "Tanggal", 100, "dd/MM/yyyy")
        AddDataColumn("URAIAN", "Uraian", 200)
        AddDataColumn("NAMA_AKUN_D", "Akun Debet", 150)
        AddDataColumn("NAMA_AKUN_K", "Akun Kredit", 150)
        AddDataColumn("NOMINAL", "Nominal", 120, "N0")
        AddDataColumn("ID_USER", "User", 80)

        AddDataColumn("NO_NOTA", "No. Nota", 0, "", False)
        AddDataColumn("AKUN_D", "Kode Debet", 0, "", False)
        AddDataColumn("AKUN_K", "Kode Kredit", 0, "", False)
        AddDataColumn("NAMA_BANTU_D", "Bantu D", 0, "", False)
        AddDataColumn("KODE_BANTU_D", "Kode Bantu D", 0, "", False)
        AddDataColumn("NAMA_BANTU_K", "Bantu K", 0, "", False)
        AddDataColumn("KODE_BANTU_K", "Kode Bantu K", 0, "", False)

        If DgvKeuangan.Columns.Contains("NOMINAL") Then
            DgvKeuangan.Columns("NOMINAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DgvKeuangan.Columns("NOMINAL").DefaultCellStyle.Font = New Font(DgvKeuangan.Font, FontStyle.Bold)
        End If
    End Sub

    Private Sub AddButtonColumn(name As String, text As String, width As Integer)
        Dim buttonCol As New DataGridViewButtonColumn With {
            .Name = name,
            .HeaderText = "",
            .Text = text,
            .UseColumnTextForButtonValue = True,
            .Width = width,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            .Resizable = DataGridViewTriState.False,
            .DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleCenter,
                .Font = New Font(DgvKeuangan.Font.FontFamily, DgvKeuangan.Font.Size)
            }
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
            .DataPropertyName = name
        }
        If Not String.IsNullOrEmpty(format) Then col.DefaultCellStyle.Format = format
        DgvKeuangan.Columns.Add(col)
    End Sub

    Private Sub DgvKeuangan_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvKeuangan.CellContentClick
        If e.RowIndex < 0 Then Return
        Try
            If _hakEdit AndAlso DgvKeuangan.Columns.Contains("EDIT") AndAlso
               e.ColumnIndex = DgvKeuangan.Columns("EDIT").Index Then
                HandleEditClick(e.RowIndex)
            ElseIf _hakHapus AndAlso DgvKeuangan.Columns.Contains("HAPUS") AndAlso
                   e.ColumnIndex = DgvKeuangan.Columns("HAPUS").Index Then
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

        SetComboBoxText(CmbDebetKeuangan, GetCellValue(row, "NAMA_AKUN_D"))
        SetComboBoxText(CmbKreditKeuangan, GetCellValue(row, "NAMA_AKUN_K"))

        If CmbBantuDKeuangan.Visible Then SetComboBoxText(CmbBantuDKeuangan, GetCellValue(row, "NAMA_BANTU_D"))
        If CmbBantuKKeuangan.Visible Then SetComboBoxText(CmbBantuKKeuangan, GetCellValue(row, "NAMA_BANTU_K"))

        Dim nominal As Decimal = ModuleAngka.ParseDecimal(GetCellValue(row, "NOMINAL"))
        TxtNominalKeuangan.Text = nominal.ToString()
    End Sub

    Private Function GetCellValue(row As DataGridViewRow, columnName As String) As String
        Return If(row.Cells(columnName).Value?.ToString(), String.Empty)
    End Function

    Private Sub SetComboBoxText(combo As ComboBox, text As String)
        If combo.Items.Contains(text) Then
            combo.SelectedItem = text
        Else
            combo.Text = text
        End If
    End Sub

    Private Sub DeleteTransaction(transactionId As String)
        ' Baca snapshot sebelum hapus — untuk audit trail dan update HutangAwal pinjaman
        Dim jenisTransaksi As String = ""
        Dim nominalHapus As Decimal = 0D
        Dim nomorAkunD As String = ""
        Dim nomorAkunK As String = ""

        Dim transaction As MySqlTransaction = Nothing

        Try
            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            ' ========================================
            ' START: Audit Trail - Hapus Jurnal Keuangan
            ' ========================================
            Dim sbSnapshot As New System.Text.StringBuilder()
            Try
                Using cmdSnap As New MySqlCommand(
                    "SELECT NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, NAMA_BANTU_D, KODE_BANTU_D, NAMA_BANTU_K, KODE_BANTU_K " &
                    "FROM JurnalUmum WHERE NO_TRANSAKSI = @id LIMIT 1", conn, transaction)
                    cmdSnap.Parameters.AddWithValue("@id", transactionId)
                    Using rdSnap = cmdSnap.ExecuteReader()
                        If rdSnap.Read() Then
                            nominalHapus = ModuleAngka.ParseDecimal(rdSnap("NOMINAL"))
                            jenisTransaksi = If(IsDBNull(rdSnap("JENIS_TRANSAKSI")), "", rdSnap("JENIS_TRANSAKSI").ToString())
                            nomorAkunD = If(IsDBNull(rdSnap("NOMOR_AKUN_D")), "", rdSnap("NOMOR_AKUN_D").ToString())
                            nomorAkunK = If(IsDBNull(rdSnap("NOMOR_AKUN_K")), "", rdSnap("NOMOR_AKUN_K").ToString())
                            sbSnapshot.AppendLine($"No. Transaksi: {rdSnap("NO_TRANSAKSI")}")
                            sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TGL_TRANSAKSI")).ToString("dd/MM/yyyy HH:mm:ss")}")
                            sbSnapshot.AppendLine($"No. Nota: {rdSnap("NO_NOTA")}")
                            sbSnapshot.AppendLine($"Uraian: {rdSnap("URAIAN")}")
                            sbSnapshot.AppendLine($"Jenis Transaksi: {rdSnap("JENIS_TRANSAKSI")}")
                            sbSnapshot.AppendLine($"Akun Debet: {rdSnap("NAMA_AKUN_D")} [{rdSnap("NOMOR_AKUN_D")}]")
                            sbSnapshot.AppendLine($"Akun Kredit: {rdSnap("NAMA_AKUN_K")} [{rdSnap("NOMOR_AKUN_K")}]")
                            If Not IsDBNull(rdSnap("NAMA_BANTU_D")) AndAlso Not String.IsNullOrEmpty(rdSnap("NAMA_BANTU_D").ToString()) Then
                                sbSnapshot.AppendLine($"Bantuan D: {rdSnap("NAMA_BANTU_D")} [{rdSnap("KODE_BANTU_D")}]")
                            End If
                            If Not IsDBNull(rdSnap("NAMA_BANTU_K")) AndAlso Not String.IsNullOrEmpty(rdSnap("NAMA_BANTU_K").ToString()) Then
                                sbSnapshot.AppendLine($"Bantuan K: {rdSnap("NAMA_BANTU_K")} [{rdSnap("KODE_BANTU_K")}]")
                            End If
                            sbSnapshot.AppendLine($"Nominal: {ModuleAngka.FormatRupiah(nominalHapus)}")
                        End If
                    End Using
                End Using
            Catch
                sbSnapshot.AppendLine("Gagal baca data sebelum hapus")
            End Try
            ModuleAuditTrail.CatatAuditMaster("JRN:" & transactionId, "HAPUS", "Jurnal Keuangan", sbSnapshot.ToString(), trans:=transaction)
            ' ========================================
            ' END: Audit Trail - Hapus Jurnal Keuangan
            ' ========================================

            ' Reversal saldo akun SEBELUM DELETE JurnalUmum
            ModuleVariabel.ReversalSaldoAkunDariFaktur(transactionId, transaction)

            ' Hapus jurnal
            Using cmdDel As New MySqlCommand("DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @id", conn, transaction)
                cmdDel.Parameters.AddWithValue("@id", transactionId)
                cmdDel.ExecuteNonQuery()
            End Using

            ' Commit
            transaction.Commit()

            ' Balikkan HutangAwal supplier/pelanggan jika jenis pinjaman
            If nominalHapus > 0 Then
                UpdateHutangAwalPinjaman(jenisTransaksi, nominalHapus, -1)
            End If

        Catch ex As Exception
            transaction?.Rollback()
            MessageBox.Show($"Gagal menghapus: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Transaction Type Handlers"
    Private Sub HandleButtonClick(clickedButton As Button, transactionName As String, detailText As String)
        ResetButtonColors()
        ModuleTheme.SetNavButtonActive(clickedButton)

        PanelInput.Visible = True
        PanelRinciKeuangan.Visible = True
        LblNamaTransaksi.Text = transactionName
        LblRinciPengeluaran.Text = detailText

        ModulHakAkses.ResetDTPKeTanggalHariIni(DTPTglKeuangan)
        ResetFormForNewTransaction()
    End Sub

    Private Sub ResetButtonColors()
        For Each btn As Button In {BtnPemasukan, BtnPengeluaran, BtnBiaya,
                                   BtnSetorBos, BtnPinjamSuplier, BtnPindahR,
                                   BtnPinjamPelanggan}
            ModuleTheme.SetNavButtonIdle(btn)
        Next
    End Sub

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

    Private Sub BtnPinjamSuplier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPinjamSuplier.Click
        HandleButtonClick(BtnPinjamSuplier, "PINJAMAN SUPPLIER", "RINCIAN PINJAMAN SUPPLIER")
    End Sub

    Private Sub BtnPinjamPelanggan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPinjamPelanggan.Click
        HandleButtonClick(BtnPinjamPelanggan, "PINJAMAN PELANGGAN", "RINCIAN PINJAMAN PELANGGAN")
    End Sub

    Private Sub BtnPindahR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPindahR.Click
        HandleButtonClick(BtnPindahR, "PINDAH REKENING", "RINCIAN PINDAH REKENING")
    End Sub

    Private Sub ResetFormForNewTransaction()
        SetButtonState(FormState.Add)
        SetInitialFormState()
        PopulateAccountComboBoxes()
    End Sub

    Private Sub ResetFormAfterTransaction()
        ResetFormForNewTransaction()
    End Sub
#End Region

#Region "Account Comboboxes"
    ' Query langsung ke DB setiap kali — tidak ada cache.
    ' tbl_datareferensi kecil, query dengan index cepat, dan data selalu fresh (multi-client safe).
    Private Sub PopulateAccountComboBoxes()
        Dim currentType = LblNamaTransaksi.Text
        Dim dt As DataTable = GetAkunData()

        ' Bangun lookup: TypeAkun -> List(NamaAkun), NamaAkun -> KodeAkun
        Dim byType As New Dictionary(Of String, List(Of String))()
        Dim kodeByNama As New Dictionary(Of String, String)()

        For Each row As DataRow In dt.Rows
            Dim typeAkun = row("Type_Akun").ToString().Trim()
            Dim namaAkun = row("Nama_Akun").ToString().Trim()
            Dim kodeAkun = row("Kode_Akun").ToString().Trim()

            If Not byType.ContainsKey(typeAkun) Then byType(typeAkun) = New List(Of String)()
            byType(typeAkun).Add(namaAkun)

            If Not kodeByNama.ContainsKey(namaAkun) Then kodeByNama(namaAkun) = kodeAkun
        Next

        Dim debetItems As New List(Of String)()
        Dim kreditItems As New List(Of String)()

        Select Case currentType
            Case "PEMASUKAN"
                AddFromTypes(debetItems, byType, {"KAS", "BANK"})
                AddFromTypes(kreditItems, byType, byType.Keys.ToArray(), {"KAS", "BANK", "LABA RUGI"})
            Case "PENGELUARAN"
                AddFromTypes(debetItems, byType, byType.Keys.ToArray(), {"KAS", "BANK", "LABA RUGI"})
                AddFromTypes(kreditItems, byType, {"KAS", "BANK"})
            Case "BIAYA"
                AddFromTypes(debetItems, byType, {"BIAYA"})
                AddFromTypes(kreditItems, byType, {"KAS", "BANK"})
            Case "SETOR KE BOS"
                ' Akun spesifik berdasarkan kode
                Dim akunSetor = kodeByNama.Where(Function(kvp) kvp.Value = "04.02.001").Select(Function(kvp) kvp.Key).FirstOrDefault()
                If Not String.IsNullOrEmpty(akunSetor) Then debetItems.Add(akunSetor)
                AddFromTypes(kreditItems, byType, {"KAS"})
            Case "PINJAMAN SUPPLIER"
                ' Supplier beri pinjaman → D KAS/BANK, K HUTANG BELANJA
                AddFromTypes(debetItems, byType, {"KAS", "BANK"})
                AddFromTypes(kreditItems, byType, {"HUTANG"})
            Case "PINJAMAN PELANGGAN"
                ' Pelanggan pinjam dari toko → D PIUTANG, K KAS/BANK
                AddFromTypes(debetItems, byType, {"PIUTANG"})
                AddFromTypes(kreditItems, byType, {"KAS", "BANK"})
            Case "PINDAH REKENING"
                AddFromTypes(debetItems, byType, byType.Keys.ToArray(), {"LABA RUGI"})
                AddFromTypes(kreditItems, byType, byType.Keys.ToArray(), {"LABA RUGI"})
        End Select

        _isLoading = True
        CmbDebetKeuangan.Items.Clear()
        CmbKreditKeuangan.Items.Clear()
        CmbDebetKeuangan.Items.AddRange(debetItems.ToArray())
        CmbKreditKeuangan.Items.AddRange(kreditItems.ToArray())
        If CmbDebetKeuangan.Items.Count > 0 Then CmbDebetKeuangan.SelectedIndex = 0
        If CmbKreditKeuangan.Items.Count > 0 Then CmbKreditKeuangan.SelectedIndex = 0
        _isLoading = False

        ' Tampilkan panel bantu untuk pinjaman
        Select Case LblNamaTransaksi.Text
            Case "PINJAMAN SUPPLIER"
                ' CmbBantuK = pilih supplier
                LblBantuKKeuangan.Visible = True
                LblBantuKKeuangan.Text = "Supplier :"
                CmbBantuKKeuangan.Visible = True
                TxtBantuKKeuanganNama.Visible = True
                TxtBantuKKeuangan.Visible = True
                IsiComboBoxSupplier(CmbBantuKKeuangan)
            Case "PINJAMAN PELANGGAN"
                ' CmbBantuD = pilih pelanggan
                LblBantuDKeuangan.Visible = True
                LblBantuDKeuangan.Text = "Pelanggan :"
                CmbBantuDKeuangan.Visible = True
                TxtBantuDKeuanganNama.Visible = True
                TxtBantuDKeuangan.Visible = True
                IsiComboBoxPelanggan(CmbBantuDKeuangan)
        End Select

        TxtUraianKeuangan.Focus()
    End Sub

    Private Sub AddFromTypes(targetList As List(Of String), byType As Dictionary(Of String, List(Of String)),
                             accountTypes As String(), Optional excludeTypes As String() = Nothing)
        For Each t In accountTypes
            If excludeTypes IsNot Nothing AndAlso excludeTypes.Contains(t) Then Continue For
            If byType.ContainsKey(t) Then targetList.AddRange(byType(t))
        Next
    End Sub

    Private Sub CmbDebetKeuangan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbDebetKeuangan.SelectedIndexChanged
        SetAccountCodeFromCombo(CmbDebetKeuangan, TxtDebetKeuanganNama, TxtDebetKeuangan)
        If Not _isLoading Then CmbKreditKeuangan.Focus()
    End Sub

    Private Sub CmbKreditKeuangan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbKreditKeuangan.SelectedIndexChanged
        SetAccountCodeFromCombo(CmbKreditKeuangan, TxtKreditKeuanganNama, TxtKreditKeuangan)
        If Not _isLoading Then
            If CmbBantuDKeuangan.Visible Then
                CmbBantuDKeuangan.Focus()
            ElseIf CmbBantuKKeuangan.Visible Then
                CmbBantuKKeuangan.Focus()
            Else
                TxtNominalKeuangan.Focus()
            End If
        End If
    End Sub

    ' Cari kode akun langsung dari DB berdasarkan nama yang dipilih.
    Private Sub SetAccountCodeFromCombo(combo As ComboBox, txtNama As TextBox, txtKode As TextBox)
        If combo.SelectedItem Is Nothing Then Return

        Dim selectedName As String = combo.SelectedItem.ToString()
        txtNama.Text = selectedName

        Using cmd As New MySqlCommand(
            "SELECT Kode_Akun FROM tbl_datareferensi WHERE Nama_Akun = @nama LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@nama", selectedName)
            Dim result = cmd.ExecuteScalar()
            txtKode.Text = If(result IsNot Nothing AndAlso result IsNot DBNull.Value, result.ToString(), String.Empty)
        End Using
    End Sub
#End Region

#Region "CRUD Operations"
    Private Sub BtnSimpanKeuangan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpanKeuangan.Click
        If Not ValidateInput() Then Return
        Try
            If _currentFormState = FormState.Add Then
                SaveNewTransaction()
            Else
                UpdateExistingTransaction()
            End If
            ResetFormAfterTransaction()
        Catch ex As Exception
            Dim action = If(_currentFormState = FormState.Add, "menyimpan", "mengedit")
            MessageBox.Show($"Terjadi kesalahan saat {action} data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveNewTransaction()
        Dim nominal As Decimal = ModuleAngka.ParseDecimal(TxtNominalKeuangan.Text)
        Dim izinkanBackdate As Integer = If(ModulHakAkses.SettingIzinkanTanggalLampau, 1, 0)
        Dim noTransaksi As String = LblIdBayar.Text
        Dim transaction As MySqlTransaction = Nothing

        Try
            ' Validasi backdate
            If izinkanBackdate = 0 AndAlso DTPTglKeuangan.Value.Date < Today.Date Then
                MessageBox.Show("Transaksi tanggal lampau tidak diizinkan.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            ' INSERT jurnal
            Dim sqlInsert As String =
                "INSERT INTO JurnalUmum (" &
                "NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
                "AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, " &
                "AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, " &
                "NAMA_BANTU_D, KODE_BANTU_D, NAMA_BANTU_K, KODE_BANTU_K, " &
                "NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER" &
                ") VALUES (" &
                "@no_transaksi, @tgl_transaksi, @no_nota, @uraian, " &
                "@akun_d, @nama_akun_d, @nomor_akun_d, " &
                "@akun_k, @nama_akun_k, @nomor_akun_k, " &
                "@nama_bantu_d, @kode_bantu_d, @nama_bantu_k, @kode_bantu_k, " &
                "@nominal, @jenis_transaksi, @lokasi, @id_user, @id_komputer" &
                ")"

            Using cmdInsert As New MySqlCommand(sqlInsert, conn, transaction)
                cmdInsert.Parameters.AddWithValue("@no_transaksi", noTransaksi)
                cmdInsert.Parameters.AddWithValue("@tgl_transaksi", DTPTglKeuangan.Value)
                cmdInsert.Parameters.AddWithValue("@no_nota", TxtNoNota.Text)
                cmdInsert.Parameters.AddWithValue("@uraian", TxtUraianKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@akun_d", CmbDebetKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@nama_akun_d", TxtDebetKeuanganNama.Text)
                cmdInsert.Parameters.AddWithValue("@nomor_akun_d", TxtDebetKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@akun_k", CmbKreditKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@nama_akun_k", TxtKreditKeuanganNama.Text)
                cmdInsert.Parameters.AddWithValue("@nomor_akun_k", TxtKreditKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@nama_bantu_d", If(CmbBantuDKeuangan.Visible, CmbBantuDKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@kode_bantu_d", If(CmbBantuDKeuangan.Visible, TxtBantuDKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@nama_bantu_k", If(CmbBantuKKeuangan.Visible, CmbBantuKKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@kode_bantu_k", If(CmbBantuKKeuangan.Visible, TxtBantuKKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@nominal", nominal)
                cmdInsert.Parameters.AddWithValue("@jenis_transaksi", LblNamaTransaksi.Text)
                cmdInsert.Parameters.AddWithValue("@lokasi", FormUtama.StatusLokasi.Text)
                cmdInsert.Parameters.AddWithValue("@id_user", FormUtama.StatusNamaUser.Text)
                cmdInsert.Parameters.AddWithValue("@id_komputer", FormUtama.StatusNamaPC.Text)
                cmdInsert.ExecuteNonQuery()
            End Using

            ' Update saldo akun — incremental delta dari jurnal yang baru di-INSERT
            ModuleVariabel.UpdateSaldoAkunDeltaDariFaktur(noTransaksi, transaction)

            ' Commit
            transaction.Commit()

            ' Audit jurnal keseimbangan
            CatatJurnalTidakSeimbang(noTransaksi & "-" & TxtUraianKeuangan.Text, nominal, nominal, "Jurnal Keuangan", {"JurnalManual"})

            ' Update HutangAwal supplier/pelanggan jika jenis pinjaman
            UpdateHutangAwalPinjaman(LblNamaTransaksi.Text, nominal, +1)

        Catch ex As Exception
            transaction?.Rollback()
            MessageBox.Show($"Gagal menyimpan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateExistingTransaction()
        Dim noTransaksiLama As String = LblIdBayar.Text
        Dim nominal As Decimal = ModuleAngka.ParseDecimal(TxtNominalKeuangan.Text)
        Dim izinkanBackdate As Integer = If(ModulHakAkses.SettingIzinkanTanggalLampau, 1, 0)
        Dim transaction As MySqlTransaction = Nothing
        Dim nomorAkunDLama As String = ""
        Dim nomorAkunKLama As String = ""
        Dim nominalLama As Decimal = 0D

        Try
            ' Validasi backdate
            If izinkanBackdate = 0 AndAlso DTPTglKeuangan.Value.Date < Today.Date Then
                MessageBox.Show("Transaksi tanggal lampau tidak diizinkan.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            ' ========================================
            ' START: Audit Trail - Edit Jurnal Keuangan
            ' ========================================
            Dim sbSnapshot As New System.Text.StringBuilder()
            Try
                Using cmdSnap As New MySqlCommand(
                    "SELECT NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, NAMA_BANTU_D, KODE_BANTU_D, NAMA_BANTU_K, KODE_BANTU_K " &
                    "FROM JurnalUmum WHERE NO_TRANSAKSI = @id LIMIT 1", conn, transaction)
                    cmdSnap.Parameters.AddWithValue("@id", noTransaksiLama)
                    Using rdSnap = cmdSnap.ExecuteReader()
                        If rdSnap.Read() Then
                            nomorAkunDLama = If(IsDBNull(rdSnap("NOMOR_AKUN_D")), "", rdSnap("NOMOR_AKUN_D").ToString())
                            nomorAkunKLama = If(IsDBNull(rdSnap("NOMOR_AKUN_K")), "", rdSnap("NOMOR_AKUN_K").ToString())
                            nominalLama = ModuleAngka.ParseDecimal(rdSnap("NOMINAL"))
                            sbSnapshot.AppendLine($"No. Transaksi: {rdSnap("NO_TRANSAKSI")}")
                            sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TGL_TRANSAKSI")).ToString("dd/MM/yyyy HH:mm:ss")}")
                            sbSnapshot.AppendLine($"No. Nota: {rdSnap("NO_NOTA")}")
                            sbSnapshot.AppendLine($"Uraian: {rdSnap("URAIAN")}")
                            sbSnapshot.AppendLine($"Jenis Transaksi: {rdSnap("JENIS_TRANSAKSI")}")
                            sbSnapshot.AppendLine($"Akun Debet: {rdSnap("NAMA_AKUN_D")} [{rdSnap("NOMOR_AKUN_D")}]")
                            sbSnapshot.AppendLine($"Akun Kredit: {rdSnap("NAMA_AKUN_K")} [{rdSnap("NOMOR_AKUN_K")}]")
                            If Not IsDBNull(rdSnap("NAMA_BANTU_D")) AndAlso Not String.IsNullOrEmpty(rdSnap("NAMA_BANTU_D").ToString()) Then
                                sbSnapshot.AppendLine($"Bantuan D: {rdSnap("NAMA_BANTU_D")} [{rdSnap("KODE_BANTU_D")}]")
                            End If
                            If Not IsDBNull(rdSnap("NAMA_BANTU_K")) AndAlso Not String.IsNullOrEmpty(rdSnap("NAMA_BANTU_K").ToString()) Then
                                sbSnapshot.AppendLine($"Bantuan K: {rdSnap("NAMA_BANTU_K")} [{rdSnap("KODE_BANTU_K")}]")
                            End If
                            sbSnapshot.AppendLine($"Nominal: {ModuleAngka.FormatRupiah(nominalLama)}")
                        End If
                    End Using
                End Using
            Catch
                sbSnapshot.AppendLine("Gagal baca data sebelum edit")
            End Try
            ModuleAuditTrail.CatatAuditMaster("JRN:" & noTransaksiLama, "EDIT", "Jurnal Keuangan", sbSnapshot.ToString(), trans:=transaction)
            ' ========================================
            ' END: Audit Trail - Edit Jurnal Keuangan
            ' ========================================

            ' Step 1: Reversal saldo akun lama SEBELUM DELETE JurnalUmum
            ModuleVariabel.ReversalSaldoAkunDariFaktur(noTransaksiLama, transaction)

            ' Hapus jurnal lama
            Using cmdDel As New MySqlCommand("DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @id", conn, transaction)
                cmdDel.Parameters.AddWithValue("@id", noTransaksiLama)
                cmdDel.ExecuteNonQuery()
            End Using

            ' Balikkan HutangAwal supplier/pelanggan untuk jurnal lama
            If nominalLama > 0 Then
                UpdateHutangAwalPinjaman(LblNamaTransaksi.Text, nominalLama, -1)
            End If

            ' Step 2: Insert jurnal baru dengan nomor yang sama
            Dim sqlInsert As String =
                "INSERT INTO JurnalUmum (" &
                "NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
                "AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, " &
                "AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, " &
                "NAMA_BANTU_D, KODE_BANTU_D, NAMA_BANTU_K, KODE_BANTU_K, " &
                "NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER" &
                ") VALUES (" &
                "@no_transaksi, @tgl_transaksi, @no_nota, @uraian, " &
                "@akun_d, @nama_akun_d, @nomor_akun_d, " &
                "@akun_k, @nama_akun_k, @nomor_akun_k, " &
                "@nama_bantu_d, @kode_bantu_d, @nama_bantu_k, @kode_bantu_k, " &
                "@nominal, @jenis_transaksi, @lokasi, @id_user, @id_komputer" &
                ")"

            Using cmdInsert As New MySqlCommand(sqlInsert, conn, transaction)
                cmdInsert.Parameters.AddWithValue("@no_transaksi", noTransaksiLama)
                cmdInsert.Parameters.AddWithValue("@tgl_transaksi", DTPTglKeuangan.Value)
                cmdInsert.Parameters.AddWithValue("@no_nota", TxtNoNota.Text)
                cmdInsert.Parameters.AddWithValue("@uraian", TxtUraianKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@akun_d", CmbDebetKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@nama_akun_d", TxtDebetKeuanganNama.Text)
                cmdInsert.Parameters.AddWithValue("@nomor_akun_d", TxtDebetKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@akun_k", CmbKreditKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@nama_akun_k", TxtKreditKeuanganNama.Text)
                cmdInsert.Parameters.AddWithValue("@nomor_akun_k", TxtKreditKeuangan.Text)
                cmdInsert.Parameters.AddWithValue("@nama_bantu_d", If(CmbBantuDKeuangan.Visible, CmbBantuDKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@kode_bantu_d", If(CmbBantuDKeuangan.Visible, TxtBantuDKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@nama_bantu_k", If(CmbBantuKKeuangan.Visible, CmbBantuKKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@kode_bantu_k", If(CmbBantuKKeuangan.Visible, TxtBantuKKeuangan.Text, String.Empty))
                cmdInsert.Parameters.AddWithValue("@nominal", nominal)
                cmdInsert.Parameters.AddWithValue("@jenis_transaksi", LblNamaTransaksi.Text)
                cmdInsert.Parameters.AddWithValue("@lokasi", FormUtama.StatusLokasi.Text)
                cmdInsert.Parameters.AddWithValue("@id_user", FormUtama.StatusNamaUser.Text)
                cmdInsert.Parameters.AddWithValue("@id_komputer", FormUtama.StatusNamaPC.Text)
                cmdInsert.ExecuteNonQuery()
            End Using

            ' Update saldo akun baru — incremental delta dari jurnal yang baru di-INSERT
            ModuleVariabel.UpdateSaldoAkunDeltaDariFaktur(noTransaksiLama, transaction)

            ' Update HutangAwal supplier/pelanggan untuk jurnal baru
            If nominal > 0 Then
                UpdateHutangAwalPinjaman(LblNamaTransaksi.Text, nominal, +1)
            End If

            ' Commit
            transaction.Commit()

            ' Audit jurnal keseimbangan
            CatatJurnalTidakSeimbang(noTransaksiLama & "-" & TxtUraianKeuangan.Text, nominal, nominal, "Jurnal Keuangan (Edit)", {"JurnalManual"})

        Catch ex As Exception
            transaction?.Rollback()
            MessageBox.Show($"Gagal mengedit: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnBatalKeuangan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBatalKeuangan.Click
        ResetFormAfterTransaction()
    End Sub

    Private Function ValidateInput() As Boolean
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

        If ModuleAngka.ParseDecimal(TxtNominalKeuangan.Text) <= 0 Then
            MessageBox.Show("Nominal harus diisi dengan angka yang valid.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNominalKeuangan.Focus()
            Return False
        End If

        Return True
    End Function
#End Region

#Region "Helper Functions"
    Private Sub GenerateTransactionId()
        Dim prefix As String = GetTransactionPrefix(LblNamaTransaksi.Text)
        Dim tgl As Date = DTPTglKeuangan.Value.Date

        ' Pakai sp_hlp_faktur_generate — aman multi-user (FOR UPDATE), format konsisten
        ' Format hasil: PREFIX-YYMMDDXXXX (contoh: MS-2604190001)
        Using cmd As New MySqlCommand("CALL sp_hlp_faktur_generate(@prefix, @tgl, 'jurnalumum', 'NO_TRANSAKSI', @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", prefix)
            cmd.Parameters.AddWithValue("@tgl", tgl)
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblIdBayar.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Private Function GetTransactionPrefix(transactionName As String) As String
        Select Case transactionName
            Case "PEMASUKAN" : Return "MS"
            Case "PENGELUARAN" : Return "KL"
            Case "BIAYA" : Return "BY"
            Case "SETOR KE BOS" : Return "SB"
            Case "PINDAH REKENING" : Return "PR"
            Case "PINJAMAN SUPPLIER" : Return "PS"
            Case "PINJAMAN PELANGGAN" : Return "PP"
            Case Else : Return "TR"
        End Select
    End Function

    Private Sub SetupTooltips()
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
        SetTooltip(BtnPinjamSuplier, "🏢 PINJAMAN DARI SUPPLIER", "Catat pinjaman tunai yang diterima dari supplier. KAS bertambah, hutang ke supplier bertambah.")
        SetTooltip(BtnPinjamPelanggan, "👤 PINJAMAN KE PELANGGAN", "Catat pinjaman tunai yang diberikan ke pelanggan. KAS berkurang, piutang ke pelanggan bertambah.")
    End Sub

    Private Sub SetTooltip(control As Control, title As String, text As String)
        ToolTip1.SetToolTip(control, $"{title}{Environment.NewLine}{Environment.NewLine}{text}")
    End Sub

    Private Sub IsiComboBoxSupplier(cmb As ComboBox)
        cmb.Items.Clear()
        Using cmd As New MySqlCommand("SELECT KODE, NAMA FROM tbl_supliyer ORDER BY NAMA", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    cmb.Items.Add(rd("NAMA").ToString())
                End While
            End Using
        End Using
    End Sub

    Private Sub IsiComboBoxPelanggan(cmb As ComboBox)
        cmb.Items.Clear()
        Using cmd As New MySqlCommand("SELECT KODE, NAMA FROM tbl_pelanggan ORDER BY NAMA", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    cmb.Items.Add(rd("NAMA").ToString())
                End While
            End Using
        End Using
    End Sub

    Private Sub CmbBantuKKeuangan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbBantuKKeuangan.SelectedIndexChanged
        If CmbBantuKKeuangan.SelectedItem Is Nothing Then Return
        If LblNamaTransaksi.Text = "PINJAMAN SUPPLIER" Then
            Using cmd As New MySqlCommand("SELECT KODE FROM tbl_supliyer WHERE NAMA = @nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@nama", CmbBantuKKeuangan.SelectedItem.ToString())
                Dim result = cmd.ExecuteScalar()
                TxtBantuKKeuangan.Text = If(result IsNot Nothing AndAlso result IsNot DBNull.Value, result.ToString(), "")
                TxtBantuKKeuanganNama.Text = CmbBantuKKeuangan.SelectedItem.ToString()
            End Using
        End If
    End Sub

    Private Sub CmbBantuDKeuangan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbBantuDKeuangan.SelectedIndexChanged
        If CmbBantuDKeuangan.SelectedItem Is Nothing Then Return
        If LblNamaTransaksi.Text = "PINJAMAN PELANGGAN" Then
            Using cmd As New MySqlCommand("SELECT KODE FROM tbl_pelanggan WHERE NAMA = @nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@nama", CmbBantuDKeuangan.SelectedItem.ToString())
                Dim result = cmd.ExecuteScalar()
                TxtBantuDKeuangan.Text = If(result IsNot Nothing AndAlso result IsNot DBNull.Value, result.ToString(), "")
                TxtBantuDKeuanganNama.Text = CmbBantuDKeuangan.SelectedItem.ToString()
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Update HutangAwal supplier atau pelanggan setelah simpan/hapus jurnal pinjaman.
    ''' arah: +1 = tambah (simpan), -1 = kurangi (hapus)
    ''' Dipanggil setelah SP jurnal berhasil COMMIT — pakai transaction baru.
    ''' </summary>
    Private Sub UpdateHutangAwalPinjaman(jenisTransaksi As String, nominal As Decimal, arah As Integer)
        Dim kodeEntitas As String = ""

        ' Ambil kode bantu dari combobox yang visible (supplier dari K, pelanggan dari D)
        Select Case jenisTransaksi
            Case "PINJAMAN SUPPLIER"
                kodeEntitas = If(CmbBantuKKeuangan.Visible, TxtBantuKKeuangan.Text, "")
            Case "PINJAMAN PELANGGAN"
                kodeEntitas = If(CmbBantuDKeuangan.Visible, TxtBantuDKeuangan.Text, "")
            Case Else
                Return  ' Bukan jenis pinjaman, tidak perlu update
        End Select

        If String.IsNullOrEmpty(kodeEntitas) Then Return

        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Select Case jenisTransaksi
                    Case "PINJAMAN SUPPLIER"
                        Using cmd As New MySqlCommand(
                            "UPDATE tbl_supliyer SET HutangAwal = HutangAwal + @delta WHERE Kode = @kode",
                            conn, transaction)
                            cmd.Parameters.AddWithValue("@delta", nominal * arah)
                            cmd.Parameters.AddWithValue("@kode", kodeEntitas)
                            cmd.ExecuteNonQuery()
                        End Using
                        UpdateHutangSupliyer(kodeEntitas, transaction)

                    Case "PINJAMAN PELANGGAN"
                        Using cmd As New MySqlCommand(
                            "UPDATE tbl_pelanggan SET HutangAwal = HutangAwal + @delta WHERE Kode = @kode",
                            conn, transaction)
                            cmd.Parameters.AddWithValue("@delta", nominal * arah)
                            cmd.Parameters.AddWithValue("@kode", kodeEntitas)
                            cmd.ExecuteNonQuery()
                        End Using
                        UpdatePiutangPelanggan(kodeEntitas, transaction)
                End Select

                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show($"Gagal update saldo pinjaman: {ex.Message}", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Using
    End Sub

#End Region

#Region "Keyboard Shortcuts"
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.F2
                BtnPemasukan.PerformClick()
                Return True
            Case Keys.F3
                BtnPengeluaran.PerformClick()
                Return True
            Case Keys.F4
                BtnBiaya.PerformClick()
                Return True
            Case Keys.F5
                BtnPindahR.PerformClick()
                Return True
            Case Keys.F6
                BtnSetorBos.PerformClick()
                Return True
            Case Keys.F7
                BtnPinjamSuplier.PerformClick()
                Return True
            Case Keys.F10
                BtnPinjamPelanggan.PerformClick()
                Return True
            Case Keys.F8
                If BtnSimpanKeuangan.Visible AndAlso BtnSimpanKeuangan.Enabled Then
                    BtnSimpanKeuangan.PerformClick()
                    Return True
                End If
            Case Keys.F9
                If BtnBatalKeuangan.Visible Then
                    BtnBatalKeuangan.PerformClick()
                    Return True
                End If
            Case Keys.Escape
                BTNKeluar.PerformClick()
                Return True
            Case Keys.Enter
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
                    If BtnSimpanKeuangan.Visible AndAlso BtnSimpanKeuangan.Enabled Then
                        BtnSimpanKeuangan.PerformClick()
                    End If
                    Return True
                End If
        End Select

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
#End Region

End Class
