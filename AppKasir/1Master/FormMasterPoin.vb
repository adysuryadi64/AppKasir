''' <summary>
''' FormMasterPoin — Form Konfigurasi Earn Rate, Harga Poin Barang, dan Riwayat Poin Pelanggan.
'''
''' Tab 1 — Konfigurasi Poin: atur nilai earn rate, simpan ke poin_config.
''' Tab 2 — Harga Poin Barang: atur harga poin per barang, simpan ke poin_barang.
''' Tab 3 — Riwayat Poin Pelanggan: lihat riwayat transaksi poin per pelanggan.
'''
''' Requirement: Req 1, Req 4, Req 7
''' </summary>
Public Class FormMasterPoin

    ' ─── State ───────────────────────────────────────────────────────────────
    Private _kodePelangganRiwayat As String = ""

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: FORM LOAD
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Form Load"

    Private Sub FormMasterPoin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        MuatKonfigurasiKeForm()
        MuatDataPoinBarang()

        ' Inisialisasi DateTimePicker riwayat
        DtpDari.Value = DateTime.Today.AddMonths(-1)
        DtpSampai.Value = DateTime.Today
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB 1 — KONFIGURASI POIN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab 1 — Konfigurasi Poin"

    Private Sub MuatKonfigurasiKeForm()
        Try
            Using cmd As New MySqlCommand(
                "SELECT AKTIF, MEKANISME, POIN_PER_QTY, KELIPATAN_NOMINAL, MINIMUM_REDEEM " &
                "FROM poin_config ORDER BY ID DESC LIMIT 1", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        CmbPoinAktif.Text = If(IsDBNull(rd("AKTIF")), "Tidak",
                            If(Convert.ToBoolean(rd("AKTIF")), "Iya", "Tidak"))
                        Dim mekanisme As String = If(IsDBNull(rd("MEKANISME")), "PER_ITEM", rd("MEKANISME").ToString())
                        CmbPoinMekanisme.Text = If(mekanisme.ToUpper().Trim() = "PER_ITEM", "Per Item (Qty)", "Per Kelipatan Nominal")
                        TxtPoinPerQty.Text = If(IsDBNull(rd("POIN_PER_QTY")), "1,00",
                            Math.Max(1, Convert.ToDecimal(rd("POIN_PER_QTY"))).ToString("N2"))
                        TxtKelipatanNominal.Text = If(IsDBNull(rd("KELIPATAN_NOMINAL")), "10.000",
                            Math.Max(1, Convert.ToDecimal(rd("KELIPATAN_NOMINAL"))).ToString("N0"))
                        TxtMinimumRedeem.Text = If(IsDBNull(rd("MINIMUM_REDEEM")), "100",
                            Math.Max(0, Convert.ToInt32(rd("MINIMUM_REDEEM"))).ToString())
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.MuatKonfigurasiKeForm] {ex.Message}")
        End Try

        TerapkanVisibilitasEarnRate()
        PerbaruiFormatLabel()
    End Sub


    Private Sub TerapkanVisibilitasEarnRate()
        Dim poinAktif As Boolean = (CmbPoinAktif.Text = "Iya")
        Dim isPoinPerItem As Boolean = (CmbPoinMekanisme.Text.Contains("Item"))

        LblPoinMekanisme.Visible = poinAktif
        CmbPoinMekanisme.Visible = poinAktif

        LblPoinPerQty.Visible = poinAktif AndAlso isPoinPerItem
        TxtPoinPerQty.Visible = poinAktif AndAlso isPoinPerItem
        LblPoinPerQtyFormat.Visible = poinAktif AndAlso isPoinPerItem
        LblKelipatanNominal.Visible = poinAktif AndAlso Not isPoinPerItem
        TxtKelipatanNominal.Visible = poinAktif AndAlso Not isPoinPerItem
        LblKelipatanNominalFormat.Visible = poinAktif AndAlso Not isPoinPerItem

        ' Minimum Redeem hanya tampil jika sistem poin aktif
        LblMinimumRedeem.Visible = poinAktif
        TxtMinimumRedeem.Visible = poinAktif
        LblMinimumRedeemInfo.Visible = poinAktif

        If isPoinPerItem Then
            LblPoinPerQty.Text = "Poin per 1 Qty Item :"
        Else
            LblKelipatanNominal.Text = "Setiap belanja Rp ... → 1 poin :"
        End If

        If poinAktif Then
            Dim mekanismeStr As String = If(isPoinPerItem,
                "Per Item (Qty) — poin dihitung dari jumlah qty terjual",
                "Per Kelipatan Nominal — poin dihitung dari total belanja (Rp)")
            LblInfoMekanisme.Text = "Mekanisme aktif: " & mekanismeStr
        Else
            LblInfoMekanisme.Text = "Sistem poin tidak aktif."
        End If
    End Sub

    Private Sub CmbPoinAktif_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbPoinAktif.SelectedIndexChanged
        TerapkanVisibilitasEarnRate()
    End Sub

    Private Sub CmbPoinMekanisme_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbPoinMekanisme.SelectedIndexChanged
        TerapkanVisibilitasEarnRate()
    End Sub

    Private Sub PerbaruiFormatLabel()
        Dim poinPerQty As Decimal
        If Decimal.TryParse(TxtPoinPerQty.Text, poinPerQty) Then
            LblPoinPerQtyFormat.Text = "= " & poinPerQty.ToString("N2") & " poin per item"
        End If
        Dim nominal As Decimal
        If Decimal.TryParse(TxtKelipatanNominal.Text, nominal) Then
            LblKelipatanNominalFormat.Text = "= Rp " & nominal.ToString("N0") & " → 1 poin"
        End If
    End Sub

    Private Sub TxtPoinPerQty_TextChanged(sender As Object, e As EventArgs) Handles TxtPoinPerQty.TextChanged
        PerbaruiFormatLabel()
    End Sub

    Private Sub TxtKelipatanNominal_TextChanged(sender As Object, e As EventArgs) Handles TxtKelipatanNominal.TextChanged
        PerbaruiFormatLabel()
    End Sub

    Private Function ParseDecimal(text As String) As Decimal?
        Dim val As Decimal
        If Decimal.TryParse(text, val) AndAlso val > 0 Then
            Return val
        End If
        Return Nothing
    End Function

    Private Sub BtnSimpanKonfig_Click(sender As Object, e As EventArgs) Handles BtnSimpanKonfig.Click
        Dim aktif As Integer = If(CmbPoinAktif.Text = "Iya", 1, 0)
        Dim mekanisme As String = If(CmbPoinMekanisme.Text.Contains("Item"), "PER_ITEM", "PER_NOMINAL")

        Dim poinPerQty As Decimal?
        Dim kelipatanNominal As Decimal?
        If TxtPoinPerQty.Visible Then
            poinPerQty = ParseDecimal(TxtPoinPerQty.Text)
            If poinPerQty Is Nothing OrElse poinPerQty < 1 Then
                MessageBox.Show("Nilai 'Poin per 1 Qty Item' harus lebih dari 0." & vbCrLf &
                                "Gunakan format: 1,50 (koma sebagai desimal).", "Validasi Gagal",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtPoinPerQty.Focus()
                Return
            End If
        End If
        If TxtKelipatanNominal.Visible Then
            kelipatanNominal = ParseDecimal(TxtKelipatanNominal.Text)
            If kelipatanNominal Is Nothing OrElse kelipatanNominal < 1 Then
                MessageBox.Show("Nilai 'Kelipatan Nominal (Rp)' harus lebih dari 0." & vbCrLf &
                                "Gunakan format: 10.000 (titik sebagai pemisah ribuan).", "Validasi Gagal",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtKelipatanNominal.Focus()
                Return
            End If
        End If

        ' Validasi MINIMUM_REDEEM
        Dim minimumRedeem As Integer = 0
        If Not Integer.TryParse(TxtMinimumRedeem.Text.Trim(), minimumRedeem) OrElse minimumRedeem < 0 Then
            MessageBox.Show("Nilai 'Minimum Poin untuk Redeem' harus berupa angka ≥ 0.", "Validasi Gagal",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtMinimumRedeem.Focus()
            Return
        End If

        Dim trans As MySqlTransaction = Nothing
        Try
            trans = conn.BeginTransaction()

            Using cmd As New MySqlCommand(
                "UPDATE poin_config SET " &
                "AKTIF = @aktif, " &
                "MEKANISME = @mekanisme, " &
                "POIN_PER_QTY = @poinPerQty, " &
                "KELIPATAN_NOMINAL = @kelipatanNominal, " &
                "MINIMUM_REDEEM = @minRedeem, " &
                "UPDATED_AT = NOW() " &
                "ORDER BY ID ASC LIMIT 1",
                conn, trans)
                cmd.Parameters.AddWithValue("@aktif", aktif)
                cmd.Parameters.AddWithValue("@mekanisme", mekanisme)
                cmd.Parameters.AddWithValue("@poinPerQty", poinPerQty.GetValueOrDefault(1))
                cmd.Parameters.AddWithValue("@kelipatanNominal", kelipatanNominal.GetValueOrDefault(10000))
                cmd.Parameters.AddWithValue("@minRedeem", minimumRedeem)
                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Using cmdIns As New MySqlCommand(
                        "INSERT INTO poin_config " &
                        "(AKTIF, MEKANISME, POIN_PER_QTY, KELIPATAN_NOMINAL, MINIMUM_REDEEM, UPDATED_AT) " &
                        "VALUES (@aktif, @mekanisme, @poinPerQty, @kelipatanNominal, @minRedeem, NOW())",
                        conn, trans)
                        cmdIns.Parameters.AddWithValue("@aktif", aktif)
                        cmdIns.Parameters.AddWithValue("@mekanisme", mekanisme)
                        cmdIns.Parameters.AddWithValue("@poinPerQty", poinPerQty.GetValueOrDefault(1))
                        cmdIns.Parameters.AddWithValue("@kelipatanNominal", kelipatanNominal.GetValueOrDefault(10000))
                        cmdIns.Parameters.AddWithValue("@minRedeem", minimumRedeem)
                        cmdIns.ExecuteNonQuery()
                    End Using
                End If
            End Using

            trans.Commit()

            ModuleLoyaltyPoin.MuatKonfigurasi()

            MessageBox.Show("Konfigurasi poin berhasil disimpan.", "Informasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            PerbaruiFormatLabel()

        Catch ex As Exception
            If trans IsNot Nothing Then trans.Rollback()
            MessageBox.Show("Terjadi kesalahan saat menyimpan konfigurasi:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Reset nilai ke konfigurasi yang tersimpan di DB.</summary>
    Private Sub BtnResetKonfig_Click(sender As Object, e As EventArgs) Handles BtnResetKonfig.Click
        MuatKonfigurasiKeForm()
        MessageBox.Show("Nilai dikembalikan ke konfigurasi tersimpan.", "Reset",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region


    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB 2 — HARGA POIN BARANG
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab 2 — Harga Poin Barang"

    ''' <summary>
    ''' Saat form load, muat barang yang sudah punya data di poin_barang (AKTIF atau tidak).
    ''' Barang yang belum pernah di-set tidak ditampilkan — user harus cari dulu.
    ''' </summary>
    Private Sub MuatDataPoinBarang(Optional filter As String = "")
        Try
            ' Hanya tampilkan barang yang sudah pernah di-set di poin_barang
            Dim sql As String =
                "SELECT b.ID_BARANG, b.NAMA_BARANG, b.SATUAN_STOK, " &
                "COALESCE(pb.HARGA_POIN, 0) AS HARGA_POIN, " &
                "COALESCE(pb.AKTIF, 0) AS AKTIF_POIN " &
                "FROM poin_barang pb " &
                "INNER JOIN tbl_barang b ON pb.ID_BARANG = b.ID_BARANG " &
                "WHERE b.STATUS <> 'Non Aktif' " &
                "ORDER BY b.NAMA_BARANG ASC"

            Using cmd As New MySqlCommand(sql, conn)
                Using ds As New DataSet
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(ds)
                        DgvPoinBarang.DataSource = ds.Tables(0)
                    End Using
                End Using
            End Using

            AturKolomDgvPoinBarang()

        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.MuatDataPoinBarang] {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Saat teks pencarian berubah, tampilkan ListBox hasil pencarian dari tbl_barang.
    ''' Barang yang sudah ada di DGV tidak ditampilkan lagi.
    ''' </summary>
    Private Sub TxtCariBarang_TextChanged(sender As Object, e As EventArgs) Handles TxtCariBarang.TextChanged
        Dim keyword As String = TxtCariBarang.Text.Trim()
        LstHasilCariBarang.Items.Clear()

        If keyword.Length < 2 Then
            LstHasilCariBarang.Visible = False
            Return
        End If

        Try
            Dim sql As String =
                "SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang " &
                "WHERE STATUS <> 'Non Aktif' " &
                "AND (ID_BARANG LIKE @cari OR NAMA_BARANG LIKE @cari) " &
                "ORDER BY NAMA_BARANG ASC LIMIT 30"

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@cari", "%" & keyword & "%")
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd("ID_BARANG").ToString()
                        Dim nama As String = rd("NAMA_BARANG").ToString()
                        ' Jangan tampilkan yang sudah ada di DGV
                        If Not BarangSudahDiDgv(kode) Then
                            LstHasilCariBarang.Items.Add($"{kode} - {nama}")
                        End If
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.TxtCariBarang_TextChanged] {ex.Message}")
        End Try

        LstHasilCariBarang.Visible = LstHasilCariBarang.Items.Count > 0
    End Sub

    ''' <summary>Cek apakah barang dengan kode tertentu sudah ada di DGV.</summary>
    Private Function BarangSudahDiDgv(kode As String) As Boolean
        For Each row As DataGridViewRow In DgvPoinBarang.Rows
            If row.IsNewRow Then Continue For
            If row.Cells("ID_BARANG").Value IsNot Nothing AndAlso
               row.Cells("ID_BARANG").Value.ToString() = kode Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Saat item di ListBox diklik/dipilih, tambahkan barang ke DGV dengan HARGA_POIN = 0, AKTIF = 0.
    ''' </summary>
    Private Sub LstHasilCariBarang_Click(sender As Object, e As EventArgs) Handles LstHasilCariBarang.Click
        TambahBarangDariListBox()
    End Sub

    Private Sub LstHasilCariBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstHasilCariBarang.KeyDown
        If e.KeyCode = Keys.Enter Then TambahBarangDariListBox()
        If e.KeyCode = Keys.Escape Then
            LstHasilCariBarang.Visible = False
            TxtCariBarang.Focus()
        End If
    End Sub

    Private Sub TxtCariBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCariBarang.KeyDown
        If e.KeyCode = Keys.Down AndAlso LstHasilCariBarang.Visible AndAlso LstHasilCariBarang.Items.Count > 0 Then
            LstHasilCariBarang.SelectedIndex = 0
            LstHasilCariBarang.Focus()
        End If
        If e.KeyCode = Keys.Escape Then
            LstHasilCariBarang.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Ambil item terpilih dari ListBox, tambahkan sebagai baris baru di DGV.
    ''' </summary>
    Private Sub TambahBarangDariListBox()
        If LstHasilCariBarang.SelectedIndex < 0 Then Return
        Dim teks As String = LstHasilCariBarang.SelectedItem.ToString()
        Dim parts() As String = teks.Split(New String() {" - "}, 2, StringSplitOptions.None)
        If parts.Length < 2 Then Return

        Dim kode As String = parts(0).Trim()
        Dim nama As String = parts(1).Trim()
        Dim satuan As String = ""

        ' Ambil SATUAN_STOK dari tbl_barang
        Try
            Using cmd As New MySqlCommand(
                "SELECT SATUAN_STOK FROM tbl_barang WHERE ID_BARANG = @kode LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                Dim val = cmd.ExecuteScalar()
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    satuan = val.ToString()
                End If
            End Using
        Catch
        End Try

        ' Tambah baris baru ke DGV secara manual (bukan dari DataSource)
        ' Lepas DataSource dulu agar bisa tambah baris manual
        If DgvPoinBarang.DataSource IsNot Nothing Then
            ' Konversi DataTable ke rows manual agar bisa tambah baris baru
            Dim dt As DataTable = CType(DgvPoinBarang.DataSource, DataTable)
            Dim newRow As DataRow = dt.NewRow()
            newRow("ID_BARANG") = kode
            newRow("NAMA_BARANG") = nama
            newRow("SATUAN_STOK") = satuan
            newRow("HARGA_POIN") = 0
            newRow("AKTIF_POIN") = 0
            dt.Rows.Add(newRow)
        Else
            ' DGV belum punya DataSource — inisialisasi DataTable baru
            Dim dt As New DataTable()
            dt.Columns.Add("ID_BARANG", GetType(String))
            dt.Columns.Add("NAMA_BARANG", GetType(String))
            dt.Columns.Add("SATUAN_STOK", GetType(String))
            dt.Columns.Add("HARGA_POIN", GetType(Integer))
            dt.Columns.Add("AKTIF_POIN", GetType(Integer))
            dt.Rows.Add(kode, nama, satuan, 0, 0)
            DgvPoinBarang.DataSource = dt
            AturKolomDgvPoinBarang()
        End If

        ' Tutup ListBox, bersihkan pencarian, fokus ke baris baru
        LstHasilCariBarang.Visible = False
        TxtCariBarang.Clear()
        TxtCariBarang.Focus()

        ' Fokus ke sel HARGA_POIN baris yang baru ditambah
        Dim barisBaruIdx As Integer = DgvPoinBarang.Rows.Count - 1
        If barisBaruIdx >= 0 AndAlso DgvPoinBarang.Columns.Contains("HARGA_POIN") Then
            DgvPoinBarang.CurrentCell = DgvPoinBarang.Rows(barisBaruIdx).Cells("HARGA_POIN")
            DgvPoinBarang.BeginEdit(True)
        End If
    End Sub

    ''' <summary>
    ''' Hapus baris secara langsung via tombol ✕ inline di kolom DGV.
    ''' Dipanggil dari DgvPoinBarang_CellClick saat kolom COL_HAPUS diklik.
    ''' </summary>
    Private Sub HapusBarisPoинDariDgv(idx As Integer)
        Dim dt As DataTable = TryCast(DgvPoinBarang.DataSource, DataTable)
        If dt IsNot Nothing AndAlso idx >= 0 AndAlso idx < dt.Rows.Count Then
            dt.Rows.RemoveAt(idx)
        End If
    End Sub

    ''' <summary>Klik pada kolom tombol ✕ di DGV — hapus baris tersebut langsung.</summary>
    Private Sub DgvPoinBarang_CellClick(sender As Object, e As DataGridViewCellEventArgs) _
        Handles DgvPoinBarang.CellClick
        If e.RowIndex < 0 Then Return  ' klik di header
        If e.ColumnIndex < 0 Then Return
        If DgvPoinBarang.Columns(e.ColumnIndex).Name <> "COL_HAPUS" Then Return
        HapusBarisPoинDariDgv(e.RowIndex)
    End Sub

    ''' <summary>Tombol luar BtnHapusBarisPoin — tetap berfungsi sebagai fallback untuk baris terpilih.</summary>
    Private Sub BtnHapusBarisPoin_Click(sender As Object, e As EventArgs) Handles BtnHapusBarisPoin.Click
        If DgvPoinBarang.CurrentRow Is Nothing OrElse DgvPoinBarang.CurrentRow.IsNewRow Then Return
        HapusBarisPoинDariDgv(DgvPoinBarang.CurrentRow.Index)
    End Sub

    Private Sub AturKolomDgvPoinBarang()
        If DgvPoinBarang.Columns.Count = 0 Then Return

        ' Matikan AutoSizeColumnsMode global dulu sebelum set Width manual,
        ' karena mode Fill konflik dengan assignment Width dan menyebabkan NullReferenceException.
        DgvPoinBarang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

        DgvPoinBarang.Columns("ID_BARANG").HeaderText = "Kode Barang"
        DgvPoinBarang.Columns("ID_BARANG").ReadOnly = True
        DgvPoinBarang.Columns("ID_BARANG").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        DgvPoinBarang.Columns("ID_BARANG").Width = 120

        DgvPoinBarang.Columns("NAMA_BARANG").HeaderText = "Nama Barang"
        DgvPoinBarang.Columns("NAMA_BARANG").ReadOnly = True
        DgvPoinBarang.Columns("NAMA_BARANG").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        DgvPoinBarang.Columns("SATUAN_STOK").HeaderText = "Satuan"
        DgvPoinBarang.Columns("SATUAN_STOK").ReadOnly = True
        DgvPoinBarang.Columns("SATUAN_STOK").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        DgvPoinBarang.Columns("SATUAN_STOK").Width = 60
        DgvPoinBarang.Columns("SATUAN_STOK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        DgvPoinBarang.Columns("HARGA_POIN").HeaderText = "Harga Poin"
        DgvPoinBarang.Columns("HARGA_POIN").ReadOnly = False
        DgvPoinBarang.Columns("HARGA_POIN").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        DgvPoinBarang.Columns("HARGA_POIN").Width = 100
        DgvPoinBarang.Columns("HARGA_POIN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Ganti kolom AKTIF_POIN dari TextBox default menjadi CheckBox
        Dim idxAktif As Integer = DgvPoinBarang.Columns("AKTIF_POIN").Index
        Dim chkCol As New DataGridViewCheckBoxColumn()
        chkCol.HeaderText = "Aktif"
        chkCol.Name = "AKTIF_POIN"
        chkCol.DataPropertyName = "AKTIF_POIN"
        chkCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        chkCol.Width = 60
        chkCol.ReadOnly = False
        chkCol.TrueValue = 1
        chkCol.FalseValue = 0
        DgvPoinBarang.Columns.Remove("AKTIF_POIN")
        DgvPoinBarang.Columns.Insert(idxAktif, chkCol)

        ' Tambahkan kolom tombol Hapus inline (✕) di ujung kanan — hanya jika belum ada
        If Not DgvPoinBarang.Columns.Contains("COL_HAPUS") Then
            Dim btnHapus As New DataGridViewButtonColumn()
            btnHapus.Name = "COL_HAPUS"
            btnHapus.HeaderText = ""
            btnHapus.Text = "✕"
            btnHapus.UseColumnTextForButtonValue = True
            btnHapus.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            btnHapus.Width = 36
            btnHapus.ReadOnly = False
            btnHapus.ToolTipText = "Hapus barang ini dari daftar"
            btnHapus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            btnHapus.DefaultCellStyle.Font = New Font(DgvPoinBarang.Font.FontFamily, 9, FontStyle.Bold)
            btnHapus.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38)   ' merah
            btnHapus.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242) ' merah muda
            btnHapus.DefaultCellStyle.SelectionForeColor = Color.White
            btnHapus.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 38, 38)
            DgvPoinBarang.Columns.Add(btnHapus)
        End If

        DgvPoinBarang.AllowUserToAddRows = False
        DgvPoinBarang.AllowUserToDeleteRows = False
        DgvPoinBarang.RowHeadersVisible = True
        DgvPoinBarang.RowHeadersWidth = 45
        DgvPoinBarang.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ModuleTheme.ApplyThemeDataGridView(DgvPoinBarang)

        ' Pastikan kolom COL_HAPUS tidak di-override warnanya oleh tema
        If DgvPoinBarang.Columns.Contains("COL_HAPUS") Then
            DgvPoinBarang.Columns("COL_HAPUS").DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38)
            DgvPoinBarang.Columns("COL_HAPUS").DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242)
        End If
    End Sub

    ''' <summary>
    ''' Simpan perubahan harga poin ke tabel poin_barang menggunakan INSERT ... ON DUPLICATE KEY UPDATE.
    ''' Baris yang dihapus dari DGV akan dihapus juga dari DB (sinkronisasi DELETE).
    ''' Req 4.3, Req 4.4, Req 4.6
    ''' </summary>
    Private Sub BtnSimpanHargaPoin_Click(sender As Object, e As EventArgs) Handles BtnSimpanHargaPoin.Click
        Dim trans As MySqlTransaction = Nothing
        Try
            trans = conn.BeginTransaction()

            ' Kumpulkan ID barang yang masih ada di DGV (untuk sinkronisasi hapus)
            Dim idAktif As New List(Of String)()

            ' ── 1. Upsert baris yang masih ada di DGV ──────────────────────────
            Dim sqlUpsert As String =
                "INSERT INTO poin_barang (ID_BARANG, HARGA_POIN, AKTIF, updated_at) " &
                "VALUES (@id, @harga, @aktif, NOW()) " &
                "ON DUPLICATE KEY UPDATE HARGA_POIN = @harga, AKTIF = @aktif, updated_at = NOW()"

            Using cmd As New MySqlCommand(sqlUpsert, conn, trans)
                cmd.Parameters.Add("@id", MySqlDbType.VarChar)
                cmd.Parameters.Add("@harga", MySqlDbType.Int32)
                cmd.Parameters.Add("@aktif", MySqlDbType.Byte)
                cmd.Prepare()

                For Each row As DataGridViewRow In DgvPoinBarang.Rows
                    If row.IsNewRow Then Continue For
                    Dim idBarang As String = If(row.Cells("ID_BARANG").Value IsNot Nothing,
                                                row.Cells("ID_BARANG").Value.ToString(), "")
                    If String.IsNullOrEmpty(idBarang) Then Continue For

                    Dim hargaPoin As Integer = 0
                    Integer.TryParse(If(row.Cells("HARGA_POIN").Value IsNot Nothing,
                                        row.Cells("HARGA_POIN").Value.ToString(), "0"), hargaPoin)
                    Dim aktif As Byte = If(row.Cells("AKTIF_POIN").Value IsNot Nothing AndAlso
                                           Convert.ToBoolean(row.Cells("AKTIF_POIN").Value), CByte(1), CByte(0))

                    cmd.Parameters("@id").Value = idBarang
                    cmd.Parameters("@harga").Value = hargaPoin
                    cmd.Parameters("@aktif").Value = aktif
                    cmd.ExecuteNonQuery()
                    idAktif.Add(idBarang)
                Next
            End Using

            ' ── 2. Hapus dari DB barang yang sudah dihapus dari DGV ────────────
            ' idAktif berisi ID barang yang masih ada di DGV.
            ' Barang di DB yang ID-nya TIDAK ada di idAktif harus dihapus.
            If idAktif.Count = 0 Then
                ' Semua baris dihapus dari DGV — kosongkan poin_barang
                Using cmdDel As New MySqlCommand("DELETE FROM poin_barang", conn, trans)
                    cmdDel.ExecuteNonQuery()
                End Using
            Else
                ' Hapus baris yang ID-nya tidak lagi ada di DGV
                Dim paramNames As String = String.Join(",",
                    Enumerable.Range(0, idAktif.Count).Select(Function(i) "@del" & i))
                Using cmdDel As New MySqlCommand(
                    "DELETE FROM poin_barang WHERE ID_BARANG NOT IN (" & paramNames & ")",
                    conn, trans)
                    For i As Integer = 0 To idAktif.Count - 1
                        cmdDel.Parameters.AddWithValue("@del" & i, idAktif(i))
                    Next
                    cmdDel.ExecuteNonQuery()
                End Using
            End If

            trans.Commit()
            MessageBox.Show("Harga poin barang berhasil disimpan.", "Informasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            MuatDataPoinBarang()

        Catch ex As Exception
            If trans IsNot Nothing Then trans.Rollback()
            MessageBox.Show("Terjadi kesalahan saat menyimpan harga poin:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DgvPoinBarang_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvPoinBarang.RowPostPaint
        Dim nomor As String = (e.RowIndex + 1).ToString()
        Dim fmt As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        Dim bounds As New Rectangle(e.RowBounds.Left, e.RowBounds.Top,
                                    DgvPoinBarang.RowHeadersWidth, e.RowBounds.Height)
        e.Graphics.DrawString(nomor, DgvPoinBarang.DefaultCellStyle.Font,
                              SystemBrushes.ControlText, bounds, fmt)
    End Sub

#End Region


    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB 3 — RIWAYAT POIN PELANGGAN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab 3 — Riwayat Poin Pelanggan"

    ''' <summary>Cari pelanggan via ListBox (sama metode seperti pencarian barang).</summary>
    Private Sub TxtCariPelanggan_TextChanged(sender As Object, e As EventArgs) Handles TxtCariPelanggan.TextChanged
        Dim keyword As String = TxtCariPelanggan.Text.Trim()
        LstHasilCariPelanggan.Items.Clear()

        If keyword.Length < 1 Then
            LstHasilCariPelanggan.Visible = False
            _kodePelangganRiwayat = ""
            LblKodePelanggan.Text = ""
            LblSaldoPoin.Text = "Saldo Poin: -"
            Return
        End If

        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA FROM tbl_pelanggan " &
                "WHERE Status <> 'Non Aktif' " &
                "AND (KODE LIKE @cari OR NAMA LIKE @cari) " &
                "ORDER BY NAMA ASC LIMIT 30", conn)
                cmd.Parameters.AddWithValue("@cari", "%" & keyword & "%")
                Using dt As New DataTable
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                    For Each row As DataRow In dt.Rows
                        LstHasilCariPelanggan.Items.Add(row("NAMA").ToString())
                    Next
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.TxtCariPelanggan_TextChanged] {ex.Message}")
        End Try

        If LstHasilCariPelanggan.Items.Count > 0 Then
            LstHasilCariPelanggan.Visible = True
            LstHasilCariPelanggan.BringToFront()
        Else
            LstHasilCariPelanggan.Visible = False
            _kodePelangganRiwayat = ""
            LblKodePelanggan.Text = ""
            LblSaldoPoin.Text = "Saldo Poin: -"
        End If
    End Sub

    Private Sub LstHasilCariPelanggan_Click(sender As Object, e As EventArgs) Handles LstHasilCariPelanggan.Click
        PilihPelangganDariListBox()
    End Sub

    Private Sub LstHasilCariPelanggan_KeyDown(sender As Object, e As KeyEventArgs) Handles LstHasilCariPelanggan.KeyDown
        If e.KeyCode = Keys.Enter Then PilihPelangganDariListBox()
        If e.KeyCode = Keys.Escape Then
            LstHasilCariPelanggan.Visible = False
            TxtCariPelanggan.Focus()
        End If
    End Sub

    Private Sub TxtCariPelanggan_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCariPelanggan.KeyDown
        If e.KeyCode = Keys.Down AndAlso LstHasilCariPelanggan.Visible AndAlso LstHasilCariPelanggan.Items.Count > 0 Then
            LstHasilCariPelanggan.SelectedIndex = 0
            LstHasilCariPelanggan.Focus()
        End If
        If e.KeyCode = Keys.Escape Then
            LstHasilCariPelanggan.Visible = False
        End If
    End Sub

    Private Sub PilihPelangganDariListBox()
        If LstHasilCariPelanggan.SelectedIndex < 0 Then Return
        Dim nama As String = LstHasilCariPelanggan.SelectedItem.ToString()
        Dim kode As String = ""
        Dim saldo As Integer = 0

        Try
            Using cmd As New MySqlCommand(
                "SELECT KODE, SALDO_POIN FROM tbl_pelanggan " &
                "WHERE NAMA = @nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@nama", nama)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        kode = rd("KODE").ToString()
                        saldo = If(IsDBNull(rd("SALDO_POIN")), 0, Convert.ToInt32(rd("SALDO_POIN")))
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.PilihPelangganDariListBox] {ex.Message}")
        End Try

        ' Reader sudah closed — baru update kontrol
        If Not String.IsNullOrEmpty(kode) Then
            _kodePelangganRiwayat = kode
            TxtCariPelanggan.Text = nama
            LblKodePelanggan.Text = "(" & kode & ")"
            LblSaldoPoin.Text = "Saldo Poin: " & saldo.ToString("N0")
        End If

        LstHasilCariPelanggan.Visible = False
        TxtCariPelanggan.Focus()
        TxtCariPelanggan.SelectionStart = TxtCariPelanggan.Text.Length
    End Sub

    ''' <summary>
    ''' Tampilkan riwayat poin dari poin_ledger dengan filter tanggal.
    ''' Req 7.1, Req 7.2, Req 7.3
    ''' </summary>
    Private Sub BtnTampilkanRiwayat_Click(sender As Object, e As EventArgs) Handles BtnTampilkanRiwayat.Click
        If String.IsNullOrEmpty(_kodePelangganRiwayat) Then
            MessageBox.Show("Pilih pelanggan terlebih dahulu.", "Informasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            TxtCariPelanggan.Focus()
            Return
        End If

        Try
            Dim dari As String = DtpDari.Value.ToString("yyyy-MM-dd") & " 00:00:00"
            Dim sampai As String = DtpSampai.Value.ToString("yyyy-MM-dd") & " 23:59:59"

            Dim sql As String =
                "SELECT CREATED_AT AS Tanggal, NO_REFERENSI AS `No Referensi`, " &
                "TIPE AS Tipe, JUMLAH_POIN AS `Jumlah Poin`, " &
                "KETERANGAN AS Keterangan " &
                "FROM poin_ledger " &
                "WHERE KODE_PELANGGAN = @kode " &
                "AND CREATED_AT BETWEEN @dari AND @sampai " &
                "ORDER BY CREATED_AT DESC"

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@kode", _kodePelangganRiwayat)
                cmd.Parameters.AddWithValue("@dari", dari)
                cmd.Parameters.AddWithValue("@sampai", sampai)
                Using ds As New DataSet
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(ds)
                        DgvRiwayatPoin.DataSource = ds.Tables(0)
                    End Using
                End Using
            End Using

            AturKolomDgvRiwayat()

            ' Refresh saldo terkini (Req 7.2)
            Dim saldo As Integer = ModuleLoyaltyPoin.AmbilSaldoPoin(_kodePelangganRiwayat)
            LblSaldoPoin.Text = "Saldo Poin: " & saldo.ToString("N0")

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat memuat riwayat poin:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AturKolomDgvRiwayat()
        If DgvRiwayatPoin.Columns.Count = 0 Then Return

        If DgvRiwayatPoin.Columns.Contains("Tanggal") Then
            DgvRiwayatPoin.Columns("Tanggal").HeaderText = "Tanggal"
            DgvRiwayatPoin.Columns("Tanggal").Width = 140
        End If
        If DgvRiwayatPoin.Columns.Contains("No Referensi") Then
            DgvRiwayatPoin.Columns("No Referensi").HeaderText = "No Referensi"
            DgvRiwayatPoin.Columns("No Referensi").Width = 160
        End If
        If DgvRiwayatPoin.Columns.Contains("Tipe") Then
            DgvRiwayatPoin.Columns("Tipe").HeaderText = "Tipe"
            DgvRiwayatPoin.Columns("Tipe").Width = 90
        End If
        If DgvRiwayatPoin.Columns.Contains("Jumlah Poin") Then
            DgvRiwayatPoin.Columns("Jumlah Poin").HeaderText = "Jumlah Poin"
            DgvRiwayatPoin.Columns("Jumlah Poin").Width = 100
            DgvRiwayatPoin.Columns("Jumlah Poin").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If
        If DgvRiwayatPoin.Columns.Contains("Keterangan") Then
            DgvRiwayatPoin.Columns("Keterangan").HeaderText = "Keterangan"
            DgvRiwayatPoin.Columns("Keterangan").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If

        DgvRiwayatPoin.AllowUserToAddRows = False
        DgvRiwayatPoin.AllowUserToDeleteRows = False
        DgvRiwayatPoin.ReadOnly = True
        DgvRiwayatPoin.RowHeadersVisible = True
        DgvRiwayatPoin.RowHeadersWidth = 45
        DgvRiwayatPoin.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ModuleTheme.ApplyThemeDataGridView(DgvRiwayatPoin)
    End Sub

    Private Sub DgvRiwayatPoin_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvRiwayatPoin.RowPostPaint
        Dim nomor As String = (e.RowIndex + 1).ToString()
        Dim fmt As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        Dim bounds As New Rectangle(e.RowBounds.Left, e.RowBounds.Top,
                                    DgvRiwayatPoin.RowHeadersWidth, e.RowBounds.Height)
        e.Graphics.DrawString(nomor, DgvRiwayatPoin.DefaultCellStyle.Font,
                              SystemBrushes.ControlText, bounds, fmt)
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB CONTROL DRAW (OwnerDraw)
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab Control Draw"

    Private Sub TabControlPoin_DrawItem(sender As Object, e As DrawItemEventArgs) Handles TabControlPoin.DrawItem
        Dim tc As TabControl = CType(sender, TabControl)
        Dim tab As TabPage = tc.TabPages(e.Index)
        Dim isSelected As Boolean = (tc.SelectedIndex = e.Index)

        ' Ambil warna header kategori Master (biru)
        Dim headerBack As Color = ModuleTheme.C(ModuleTheme.L_HeaderMaster, ModuleTheme.D_HeaderMaster)
        Dim inactiveBack As Color = ModuleTheme.C(Color.FromArgb(226, 232, 240), Color.FromArgb(51, 65, 85))
        Dim borderColor As Color = ModuleTheme.C(Color.FromArgb(148, 163, 184), Color.FromArgb(100, 116, 139))

        ' Latar belakang tab
        Dim bg As Color = If(isSelected, headerBack, inactiveBack)
        Using br As New SolidBrush(bg)
            e.Graphics.FillRectangle(br, e.Bounds)
        End Using

        ' Border tab
        Using pen As New Pen(borderColor, 1)
            e.Graphics.DrawRectangle(pen, e.Bounds)
        End Using

        ' Teks: putih di tab aktif, gelap/terang di tab tidak aktif
        Dim txtColor As Color
        If isSelected Then
            txtColor = Color.White
        Else
            txtColor = ModuleTheme.C(Color.FromArgb(30, 41, 59), Color.FromArgb(203, 213, 225))
        End If

        Using fnt As New Font(tc.Font.FontFamily, 9, FontStyle.Bold)
            Dim sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
            e.Graphics.DrawString(tab.Text, fnt, New SolidBrush(txtColor), RectangleF.op_Implicit(e.Bounds), sf)
        End Using
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: CLOSE
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Close"

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub FormMasterPoin_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then Close()
    End Sub

#End Region

End Class
