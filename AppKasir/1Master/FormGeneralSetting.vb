
Public Class FormGeneralSetting

    Private ReadOnly RoleComboList As List(Of (Label As Label, ComboBox As ComboBox, DefaultValue As Integer))

    ' NumericUpDown untuk konfigurasi retensi audit trail (bulan)
    ' Dibuat secara programatik agar tidak perlu modifikasi Designer
    Private NudRetensiBulan As New System.Windows.Forms.NumericUpDown() With {
        .Minimum = 1, .Maximum = 120, .Value = 3, .Width = 60
    }

    ' NumericUpDown untuk batas qty auto level satuan
    ' Dibuat secara programatik — tidak perlu modifikasi Designer
    Private NudBatasSatuanSedang As New System.Windows.Forms.NumericUpDown() With {
        .Minimum = 1, .Maximum = 9999, .Value = 3, .Width = 70
    }
    Private NudBatasSatuanBesar As New System.Windows.Forms.NumericUpDown() With {
        .Minimum = 1, .Maximum = 9999, .Value = 6, .Width = 70
    }

    ' Label untuk batas satuan — disimpan agar bisa di-show/hide
    Private _lblBatasSedang As Label
    Private _lblBatasBesar As Label

    ' ToolTip untuk penjelasan setting
    Private toolTip As New ToolTip()

    Public Sub New()
        InitializeComponent()

        ' Setup tooltip
        toolTip.AutoPopDelay = 5000
        toolTip.InitialDelay = 500
        toolTip.ReshowDelay = 100
        toolTip.ShowAlways = True

        ' Set tooltip untuk label setting
        SetupTooltipLabel()

        RoleComboList = New List(Of (Label, ComboBox, Integer)) From {
            (LblBeliRugi, CmbBeliRugi, 0),
            (LblBeliMuculJual, CmbBeliMuculJual, 0),
            (LblBeliUpdate, CmbBeliUpdate, 0),
            (LblBeliEditHarga, CmbBeliEditHarga, 0),
            (LblBeliAverage, CmbBeliAverage, 2),
            (LblBeliTanpaSupplier, CmbBeliTanpaSupplier, 1),
            (LblbeliNominal0, CmbbeliNominal0, 1),
            (LblJualEditHarga, CmbJualEditHarga, 0),
            (LblJualRugi, CmbJualRugi, 1),
            (LblDiskonItem, CmbDiskonItem, 0),
            (LblJualNominal0, CmbJualNominal0, 1),
            (LblEditHargaJual, CmbEditHargaJual, 1),
            (LblReturBeliAlasan, CmbReturBeliAlasan, 0),
            (LblReturJualAlasan, CmbReturJualAlasan, 0),
            (LblGlobalTransaksiLampau, CmbGlobalTransaksiLampau, 1),
            (LblGlobalBarangMinus, CmbGlobalBarangMinus, 1),
            (LblGlobalFokus, CmbGlobalFokus, 0),
            (LblGlobalSatuan, CmbGlobalSatuan, 1),
            (LblGlobalIsiNominal, CmbGlobalIsiNominal, 1),
            (LblGlobalInfoStok, CmbGlobalInfoStok, 0),
            (LblHidePencarianAtas, CmbHidePencarianAtas, 0),
            (LblJualAutoLevelSatuan, CmbJualAutoLevelSatuan, 1)
        }
    End Sub

    Private Sub SetupTooltipLabel()
        ' GBPembelian
        toolTip.SetToolTip(LblBeliRugi, "Jika 'Iya': Tampilkan peringatan saat harga beli > harga jual" & vbCrLf & "Jika 'Tidak': Blokir transaksi jika harga beli > harga jual")
        toolTip.SetToolTip(LblBeliMuculJual, "Jika 'Iya': Edit harga beli otomatis update harga jual di master" & vbCrLf & "Harga jual dihitung dari harga beli baru")
        toolTip.SetToolTip(LblBeliUpdate, "Metode update harga beli:" & vbCrLf & "- Harga Terbaru: Ganti harga lama langsung" & vbCrLf & "- Average: Hitung rata-rata dengan stok" & vbCrLf & "- Tidak Ada: Hanya update stok")
        toolTip.SetToolTip(LblBeliEditHarga, "Jika 'Iya': Kolom harga beli bisa diedit" & vbCrLf & "Jika 'Tidak': Kolom harga beli read-only")
        toolTip.SetToolTip(LblBeliAverage, "Basis stok untuk perhitungan rata-rata:" & vbCrLf & "- Toko: Hanya stok toko" & vbCrLf & "- Gudang: Hanya stok gudang" & vbCrLf & "- Toko dan Gudang: Jumlah keduanya")
        toolTip.SetToolTip(LblBeliTanpaSupplier, "Jika 'Iya': Pembelian tanpa supplier diizinkan" & vbCrLf & "Jika 'Tidak': Supplier wajib dipilih")
        toolTip.SetToolTip(LblbeliNominal0, "Jika 'Iya': Pembelian nominal 0 diizinkan" & vbCrLf & "Jika 'Tidak': Tolak pembelian nominal 0")

        ' GBPenjualan
        toolTip.SetToolTip(LblJualEditHarga, "Jika 'Iya': Kolom harga jual bisa diedit" & vbCrLf & "Jika 'Tidak': Kolom harga jual read-only")
        toolTip.SetToolTip(LblJualRugi, "Jika 'Iya': Penjualan rugi diizinkan" & vbCrLf & "Jika 'Tidak': Blokir jika harga jual < harga beli")
        toolTip.SetToolTip(LblDiskonItem, "Jika 'Iya': Diskon per item diizinkan" & vbCrLf & "Jika 'Tidak': Diskon per item tidak diizinkan")
        toolTip.SetToolTip(LblJualNominal0, "Jika 'Iya': Penjualan nominal 0 diizinkan" & vbCrLf & "Jika 'Tidak': Tolak penjualan nominal 0")
        toolTip.SetToolTip(LblEditHargaJual, "Jika 'Iya': Edit harga jual di master barang diizinkan" & vbCrLf & "Jika 'Tidak': Edit harga jual di master barang tidak diizinkan")

        ' GbReturBeli
        toolTip.SetToolTip(LblReturBeliAlasan, "Jika 'Iya': Wajib isi alasan retur pembelian" & vbCrLf & "Jika 'Tidak': Alasan retur pembelian opsional")

        ' GBReturJual
        toolTip.SetToolTip(LblReturJualAlasan, "Jika 'Iya': Wajib isi alasan retur penjualan" & vbCrLf & "Jika 'Tidak': Alasan retur penjualan opsional")

        ' GbGlobalTransaksi
        toolTip.SetToolTip(LblGlobalTransaksiLampau, "Jika 'Iya': Tanggal lampau diizinkan" & vbCrLf & "Jika 'Tidak': Tanggal dibatasi hari ini")
        toolTip.SetToolTip(LblGlobalBarangMinus, "Jika 'Iya': Transaksi stok minus diizinkan" & vbCrLf & "Jika 'Tidak': Tolak jika stok tidak cukup")
        toolTip.SetToolTip(LblGlobalFokus, "Mode fokus saat buka transaksi:" & vbCrLf & "- Pencarian: Fokus ke TxtNama" & vbCrLf & "- Kolom data: Fokus ke grid")
        toolTip.SetToolTip(LblGlobalSatuan, "Jika 'Iya': Satuan berbeda diizinkan" & vbCrLf & "Jika 'Tidak': Cek duplikat & gabung qty")
        toolTip.SetToolTip(LblGlobalIsiNominal, "Jika 'Iya': Auto isi nominal total saat bayar" & vbCrLf & "Jika 'Tidak': Field pembayaran kosong")
        toolTip.SetToolTip(LblGlobalInfoStok, "Jika 'Iya': Tampilkan stok Toko & Gudang" & vbCrLf & "Jika 'Tidak': Tampilkan stok lokasi aktif saja")
        toolTip.SetToolTip(LblHidePencarianAtas, "Jika 'Iya': Sembunyikan panel pencarian di atas data grid" & vbCrLf & "Jika 'Tidak': Tampilkan panel pencarian di atas data grid")
        toolTip.SetToolTip(LblJualAutoLevelSatuan, "Jika 'Iya': Satuan otomatis berubah sesuai qty (kecil/sedang/besar)" & vbCrLf & "Batas qty diatur di General Setting → Batas Satuan Sedang & Besar" & vbCrLf & "Jika 'Tidak': Satuan tidak berubah otomatis")
    End Sub

    Private Sub FormGeneralSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        SinkronkanHakAksesTanpaDuplikat()
        BacaCombobox()

        ' Buat label dan NUD batas satuan secara programatik
        ' Disembunyikan dulu — baru tampil saat CmbJualAutoLevelSatuan = "Iya"
        _lblBatasSedang = New Label() With {
            .Text = "Batas qty satuan sedang (qty >=):",
            .Font = New System.Drawing.Font("Century Gothic", 9.0!),
            .Location = New System.Drawing.Point(6, 221),
            .Size = New System.Drawing.Size(376, 28),
            .BorderStyle = BorderStyle.FixedSingle,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Visible = False
        }
        NudBatasSatuanSedang.Location = New System.Drawing.Point(388, 223)
        NudBatasSatuanSedang.Visible = False

        _lblBatasBesar = New Label() With {
            .Text = "Batas qty satuan besar (qty >=):",
            .Font = New System.Drawing.Font("Century Gothic", 9.0!),
            .Location = New System.Drawing.Point(6, 255),
            .Size = New System.Drawing.Size(376, 28),
            .BorderStyle = BorderStyle.FixedSingle,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Visible = False
        }
        NudBatasSatuanBesar.Location = New System.Drawing.Point(388, 257)
        NudBatasSatuanBesar.Visible = False

        GBPenjualan.Controls.Add(_lblBatasSedang)
        GBPenjualan.Controls.Add(NudBatasSatuanSedang)
        GBPenjualan.Controls.Add(_lblBatasBesar)
        GBPenjualan.Controls.Add(NudBatasSatuanBesar)
        GBPenjualan.Size = New System.Drawing.Size(GBPenjualan.Width, 298)

        ' Pasang event handler untuk show/hide batas satuan
        AddHandler CmbJualAutoLevelSatuan.SelectedIndexChanged, AddressOf CmbJualAutoLevelSatuan_SelectedIndexChanged

        ' Pasang event handler untuk validasi konflik Mode Pencarian vs Sembunyikan Pencarian
        AddHandler CmbGlobalFokus.SelectedIndexChanged, AddressOf CmbGlobalFokus_SelectedIndexChanged

        ' Terapkan visibilitas awal sesuai nilai yang sudah dibaca dari DB
        TerapkanVisibilitasBatasSatuan()
    End Sub

    ''' <summary>
    ''' Tampilkan atau sembunyikan kontrol batas satuan berdasarkan pilihan CmbJualAutoLevelSatuan.
    ''' </summary>
    Private Sub TerapkanVisibilitasBatasSatuan()
        Dim tampil As Boolean = CmbJualAutoLevelSatuan.Text = "Iya"
        If _lblBatasSedang IsNot Nothing Then _lblBatasSedang.Visible = tampil
        If _lblBatasBesar IsNot Nothing Then _lblBatasBesar.Visible = tampil
        NudBatasSatuanSedang.Visible = tampil
        NudBatasSatuanBesar.Visible = tampil
    End Sub

    Private Sub CmbJualAutoLevelSatuan_SelectedIndexChanged(sender As Object, e As EventArgs)
        TerapkanVisibilitasBatasSatuan()
    End Sub

    ''' <summary>
    ''' Saat Mode Pencarian diubah ke "Pencarian", pastikan Sembunyikan Pencarian tidak aktif.
    ''' Konflik: TxtNama.Focus() gagal diam-diam jika PanelCari tidak visible.
    ''' </summary>
    Private Sub CmbGlobalFokus_SelectedIndexChanged(sender As Object, e As EventArgs)
        If CmbGlobalFokus.Text.Trim() = "Pencarian" AndAlso CmbHidePencarianAtas.Text.Trim() = "Iya" Then
            MessageBox.Show(
                "Konflik Setting Terdeteksi:" & vbCrLf & vbCrLf &
                "  Mode pencarian   : Pencarian" & vbCrLf &
                "  Sembunyikan panel: Iya" & vbCrLf & vbCrLf &
                "Panel pencarian tidak bisa disembunyikan saat mode fokus adalah 'Pencarian'." & vbCrLf &
                "'Sembunyikan pencarian' otomatis direset ke 'Tidak'.",
                "Konflik Setting", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbHidePencarianAtas.Text = "Tidak"
        End If
    End Sub

    Public Sub SinkronkanHakAksesTanpaDuplikat()
        Dim listRoleDariLabel = RoleComboList.Select(Function(item) item.Label.Text).ToList()

        Dim listRoleDB As New Dictionary(Of String, List(Of String))
        Using cmd As New MySqlCommand("SELECT UserName, Role FROM hakaksesuser", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim role = rd("Role").ToString()
                    Dim user = rd("UserName").ToString()
                    If Not listRoleDB.ContainsKey(role) Then
                        listRoleDB(role) = New List(Of String)
                    End If
                    listRoleDB(role).Add(user)
                End While
            End Using
        End Using

        ' Hapus duplikat untuk 'Semua'
        For Each role In listRoleDB.Keys
            If listRoleDB(role).Count > 1 Then
                Using delCmd As New MySqlCommand("DELETE FROM hakaksesuser WHERE Role = @Role AND UserName = 'Semua'", conn)
                    delCmd.Parameters.AddWithValue("@Role", role)
                    delCmd.ExecuteNonQuery()
                End Using
            End If
        Next

        ' Tambahkan role yang belum ada
        Dim roleDBSaatIni = listRoleDB.Keys.ToList()
        Dim roleBaru = listRoleDariLabel.Distinct().Except(roleDBSaatIni).ToList()

        If roleBaru.Count > 0 Then
            Using insBaru As New MySqlCommand("INSERT INTO hakaksesuser (UserName, Role, ModuleName) VALUES (@UserName, @Role, @ModuleName)", conn)
                For Each role In roleBaru
                    Dim match = RoleComboList.FirstOrDefault(Function(item) item.Label.Text = role)
                    If match.ComboBox IsNot Nothing Then
                        Dim defaultValue As String = match.ComboBox.Items(match.DefaultValue).ToString()
                        insBaru.Parameters.Clear()
                        insBaru.Parameters.AddWithValue("@UserName", "Semua")
                        insBaru.Parameters.AddWithValue("@Role", role)
                        insBaru.Parameters.AddWithValue("@ModuleName", defaultValue)
                        insBaru.ExecuteNonQuery()
                    End If
                Next
            End Using
        End If
    End Sub

    Public Sub BacaCombobox()
        Dim SelectQuery As String = "SELECT Role, ModuleName FROM hakaksesuser WHERE ModuleName <> ''"
        Dim moduleDict As New Dictionary(Of String, String)()

        Using cmd As New MySqlCommand(SelectQuery, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    moduleDict(reader("Role").ToString()) = reader("ModuleName").ToString()
                End While
            End Using
        End Using

        For Each item In RoleComboList
            Dim role = item.Label.Text
            If moduleDict.ContainsKey(role) Then
                item.ComboBox.Text = moduleDict(role)
            Else
                item.ComboBox.SelectedIndex = item.DefaultValue
            End If
        Next

        ' Baca nilai retensi audit trail
        Try
            Using cmd As New MySqlCommand(
                "SELECT ModuleName FROM hakaksesuser WHERE Role = 'AuditRetensi' AND UserName = 'SYSTEM' LIMIT 1", conn)
                Dim result As Object = cmd.ExecuteScalar()
                Dim retensi As Integer = ModuleAngka.ParseInteger(result, 3)
                If retensi < 1 Then retensi = 1
                NudRetensiBulan.Value = retensi
            End Using
        Catch
            NudRetensiBulan.Value = 3
        End Try

        ' Baca nilai batas qty auto level satuan
        Try
            Using cmd As New MySqlCommand(
                "SELECT ModuleName FROM hakaksesuser WHERE Role = 'JualBatasSatuanSedang' AND UserName = 'SYSTEM' LIMIT 1", conn)
                Dim result As Object = cmd.ExecuteScalar()
                Dim batas As Integer = ModuleAngka.ParseInteger(result, 3)
                If batas < 1 Then batas = 1
                NudBatasSatuanSedang.Value = batas
            End Using
        Catch
            NudBatasSatuanSedang.Value = 3
        End Try
        Try
            Using cmd As New MySqlCommand(
                "SELECT ModuleName FROM hakaksesuser WHERE Role = 'JualBatasSatuanBesar' AND UserName = 'SYSTEM' LIMIT 1", conn)
                Dim result As Object = cmd.ExecuteScalar()
                Dim batas As Integer = ModuleAngka.ParseInteger(result, 6)
                If batas < 1 Then batas = 1
                NudBatasSatuanBesar.Value = batas
            End Using
        Catch
            NudBatasSatuanBesar.Value = 6
        End Try
    End Sub

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click

        ' ─── Validasi Konflik: Mode Pencarian vs Sembunyikan Pencarian ───
        ' Jika Mode fokus = "Pencarian", panel pencarian TIDAK BOLEH disembunyikan.
        ' Sebab TxtNama.Focus() akan gagal diam-diam jika PanelCari tidak visible.
        If CmbGlobalFokus.Text.Trim() = "Pencarian" AndAlso CmbHidePencarianAtas.Text.Trim() = "Iya" Then
            MessageBox.Show(
                "Konflik Setting Terdeteksi:" & vbCrLf & vbCrLf &
                "  Mode pencarian   : Pencarian" & vbCrLf &
                "  Sembunyikan panel: Iya" & vbCrLf & vbCrLf &
                "Panel pencarian tidak bisa disembunyikan saat mode fokus adalah 'Pencarian'." & vbCrLf &
                "'Sembunyikan pencarian' otomatis direset ke 'Tidak'.",
                "Konflik Setting", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbHidePencarianAtas.Text = "Tidak"
        End If
        ' ──────────────────────────────────────────────────────────────────

        Dim transaksi As MySqlTransaction = Nothing
        Try
            transaksi = conn.BeginTransaction()

            ' Simpan nilai retensi audit trail
            Dim retensiBulan As Integer = ModuleAngka.ParseInteger(NudRetensiBulan.Value, 3)
            If retensiBulan < 1 Then retensiBulan = 1
            Using cmdRetensi As New MySqlCommand(
                "INSERT INTO hakaksesuser (UserName, Role, ModuleName, CanRead, CanAdd, CanEdit, CanDelete) " &
                "VALUES ('SYSTEM', 'AuditRetensi', @val, 0, 0, 0, 0) " &
                "ON DUPLICATE KEY UPDATE ModuleName = @val",
                conn, transaksi)
                cmdRetensi.Parameters.AddWithValue("@val", retensiBulan.ToString())
                cmdRetensi.ExecuteNonQuery()
            End Using

            ' Simpan nilai batas qty auto level satuan
            Dim batasSedang As Integer = ModuleAngka.ParseInteger(NudBatasSatuanSedang.Value, 3)
            If batasSedang < 1 Then batasSedang = 1
            Using cmdBatasSedang As New MySqlCommand(
                "INSERT INTO hakaksesuser (UserName, Role, ModuleName, CanRead, CanAdd, CanEdit, CanDelete) " &
                "VALUES ('SYSTEM', 'JualBatasSatuanSedang', @val, 0, 0, 0, 0) " &
                "ON DUPLICATE KEY UPDATE ModuleName = @val",
                conn, transaksi)
                cmdBatasSedang.Parameters.AddWithValue("@val", batasSedang.ToString())
                cmdBatasSedang.ExecuteNonQuery()
            End Using
            Dim batasBesar As Integer = ModuleAngka.ParseInteger(NudBatasSatuanBesar.Value, 6)
            If batasBesar < 1 Then batasBesar = 1
            Using cmdBatasBesar As New MySqlCommand(
                "INSERT INTO hakaksesuser (UserName, Role, ModuleName, CanRead, CanAdd, CanEdit, CanDelete) " &
                "VALUES ('SYSTEM', 'JualBatasSatuanBesar', @val, 0, 0, 0, 0) " &
                "ON DUPLICATE KEY UPDATE ModuleName = @val",
                conn, transaksi)
                cmdBatasBesar.Parameters.AddWithValue("@val", batasBesar.ToString())
                cmdBatasBesar.ExecuteNonQuery()
            End Using

            Using cmd As New MySqlCommand("UPDATE hakaksesuser SET ModuleName = @ModuleName WHERE Role = @Role", conn, transaksi)
                cmd.Parameters.Add("@ModuleName", MySqlDbType.VarChar)
                cmd.Parameters.Add("@Role", MySqlDbType.VarChar)
                cmd.Prepare()

                ' ========================================
                ' START: Audit Trail - Ubah General Setting
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Dim jumlahSetting As Integer = 0
                sbSnapshot.AppendLine($"Daftar Setting:")
                For Each item In RoleComboList
                    If Not String.IsNullOrEmpty(item.Label.Text) Then
                        jumlahSetting += 1
                        Dim settingName As String = item.Label.Text
                        sbSnapshot.AppendLine($"  - {settingName}")
                    End If
                Next
                sbSnapshot.Insert(0, $"Jumlah Setting Diubah: {jumlahSetting}" & vbNewLine)
                ModuleAuditTrail.CatatAuditMaster("SET:GeneralSetting", "EDIT", "General Setting", sbSnapshot.ToString(), trans:=transaksi)
                ' ========================================
                ' END: Audit Trail - Ubah General Setting
                ' ========================================

                For Each item In RoleComboList
                    Dim moduleName As String = item.ComboBox.Text.Trim()
                    Dim roleName As String = item.Label.Text.Trim()
                    If Not String.IsNullOrEmpty(moduleName) AndAlso Not String.IsNullOrEmpty(roleName) Then
                        cmd.Parameters("@ModuleName").Value = moduleName
                        cmd.Parameters("@Role").Value = roleName
                        cmd.ExecuteNonQuery()
                    End If
                Next
            End Using

            transaksi.Commit()
            MessageBox.Show("Perubahan telah disimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ModulHakAkses.CacheGeneralSetting()
            ModulHakAkses.CacheBatasSatuan() ' Refresh cache batas qty satuan
        Catch ex As Exception
            If transaksi IsNot Nothing Then transaksi.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub BtnClose_KeyDown(sender As Object, e As KeyEventArgs) Handles BtnClose.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BtnSimpan.PerformClick()
            Case Keys.F5 : BtnRestore.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub

    Private Sub BtnRestore_Click(sender As Object, e As EventArgs) Handles BtnRestore.Click
        Dim konfirmasi = MessageBox.Show("Reset semua setting ke nilai default?", "Konfirmasi",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi <> DialogResult.Yes Then Return

        For Each item In RoleComboList
            item.ComboBox.SelectedIndex = item.DefaultValue
        Next
    End Sub
End Class
