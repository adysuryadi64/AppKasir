Public Class TambahPelanggan

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit TambahPelanggan ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("PLG:" & kode, "HAPUS"/"EDIT",
    '       "Master pelanggan", snapshotJson, "[KRITIS] Hapus/Edit data pelanggan", trans)
    ' Snapshot: baca data lama dari tbl_pelanggan sebelum DELETE/UPDATE
    ' ─────────────────────────────────────────────────────────────────────────

    Private _isLoading As Boolean = False
    Private _isEditMode As Boolean = False
    Private _canAdd As Boolean = True
    Private _canEdit As Boolean = True
    Private _canDelete As Boolean = True
    Private _toolTip As New ToolTip()

#Region "Form Load"
    Private Sub TambahPelanggan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Dim hakAkses As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Pelanggan")
        _canAdd = hakAkses(1)
        _canEdit = hakAkses(2)
        _canDelete = hakAkses(3)

        BTNSimpan.Visible = _canAdd

        Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub
#End Region

#Region "Kondisi Awal"
    Public Sub Kondisiawal()
        _isEditMode = False
        LblHeader.Text = "TAMBAH PELANGGAN"
        BTNSimpan.Text = "SIMPAN (F2)"
        TxtKode.Clear()
        TxtNama.Clear()
        TxtAlamat.Clear()
        TxtTelp.Clear()
        CmbJenis.Text = ""
        TxtJangkaPiutang.Text = "0"
        TxtAwal.Text = "0"
        TxtBayar.Text = "0"
        TxtTotal.Text = "0"
        TxtSisa.Text = "0"
        ' Task 9.1b — pastikan TxtAwal tidak terkunci saat mode tambah baru
        TxtAwal.ReadOnly = False
        TxtAwal.BackColor = SystemColors.Window
        UpdatePiutangDibayar()
        Kodepelanggan()
        TampilPelanggan()
    End Sub
#End Region

#Region "DataGridView"
    Public Sub TampilPelanggan()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand(
            "SELECT KODE, NAMA, ALAMAT, NO_TELP, JENIS, JangkaPiutang, HutangAwal, HutangAkhir, Status FROM tbl_pelanggan ORDER BY NAMA", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Dgvdata.DataSource = Nothing
        Dgvdata.Rows.Clear()
        Dgvdata.Columns.Clear()

        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 70, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama", .FillWeight = 160, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColAlamat", .HeaderText = "Alamat", .FillWeight = 160, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColTelp", .HeaderText = "No. Telp", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColJenis", .HeaderText = "Jenis", .FillWeight = 70, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColJangka", .HeaderText = "Jangka", .FillWeight = 55, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColHutangAwal", .HeaderText = "Hutang Awal", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColHutangAkhir", .HeaderText = "Hutang Akhir", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColStatus", .HeaderText = "Status", .FillWeight = 70, .ReadOnly = True})

        ModuleAngka.TerapkanFormatKolomAngka(Dgvdata, "ColJangka", "ColHutangAwal", "ColHutangAkhir")

        If _canEdit Then
            Dim colEdit As New DataGridViewButtonColumn() With {
                .Name = "ColEdit", .HeaderText = "Edit",
                .Text = "✎ Edit", .UseColumnTextForButtonValue = True,
                .Width = 70, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            Dgvdata.Columns.Add(colEdit)
        End If

        If _canDelete Then
            Dim colNonaktif As New DataGridViewButtonColumn() With {
                .Name = "ColNonaktif", .HeaderText = "Nonaktif",
                .UseColumnTextForButtonValue = False,
                .Width = 100, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            Dgvdata.Columns.Add(colNonaktif)

            Dim colHapus As New DataGridViewButtonColumn() With {
                .Name = "ColHapus", .HeaderText = "Hapus",
                .Text = "✖ Hapus", .UseColumnTextForButtonValue = True,
                .Width = 75, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            Dgvdata.Columns.Add(colHapus)
        End If

        For Each row As DataRow In dt.Rows
            Dim status As String = If(row("Status") Is DBNull.Value, "Aktif", row("Status").ToString())
            Dim hutang As Decimal = If(row("HutangAkhir") Is DBNull.Value, 0D, Convert.ToDecimal(row("HutangAkhir")))
            Dim rowIdx As Integer = Dgvdata.Rows.Add(
                row("KODE"), row("NAMA"), row("ALAMAT"), row("NO_TELP"),
                row("JENIS"), row("JangkaPiutang"), row("HutangAwal"), row("HutangAkhir"), status)

            ' Warna baris nonaktif
            ModuleTheme.SetWarnaBarisDgvNonaktif(Dgvdata.Rows(rowIdx), status = "Nonaktif")

            ' Tombol Edit — ikut warna status
            If _canEdit Then
                ModuleTheme.SetWarnaDgvBtnEdit(Dgvdata.Rows(rowIdx).Cells("ColEdit"), status <> "Nonaktif")
            End If

            If _canDelete Then
                ' Tombol Nonaktif/Aktifkan
                If status = "Aktif" Then
                    Dgvdata.Rows(rowIdx).Cells("ColNonaktif").Value = "⊘ Nonaktifkan"
                Else
                    Dgvdata.Rows(rowIdx).Cells("ColNonaktif").Value = "✔ Aktifkan"
                End If
                ModuleTheme.SetWarnaDgvBtnStatus(Dgvdata.Rows(rowIdx).Cells("ColNonaktif"), status = "Aktif")

                ' Tombol Hapus — merah jika bisa hapus, abu-abu jika tidak
                Dim bisaHapus As Boolean = hutang = 0 AndAlso status <> "Nonaktif"
                ModuleTheme.SetWarnaDgvBtnHapus(Dgvdata.Rows(rowIdx).Cells("ColHapus"), bisaHapus)
            End If
        Next

        ' Pengaturan standar dan tema DGV
        ModuleTheme.ApplyStandardDataGridViewSettings(Dgvdata)
        ModuleTheme.ApplyThemeDataGridView(Dgvdata)

        Dgvdata.ClearSelection()
        _isLoading = False
    End Sub

    Private Sub Dgvdata_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles Dgvdata.CellContentClick
        If _isLoading OrElse e.RowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = Dgvdata.Rows(e.RowIndex)
        Dim kode As String = If(row.Cells("ColKode").Value IsNot Nothing, row.Cells("ColKode").Value.ToString(), "")
        Dim nama As String = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")

        Select Case Dgvdata.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                LblHeader.Text = "EDIT PELANGGAN"
                BTNSimpan.Text = "UPDATE (F2)"
                BTNSimpan.Visible = _canEdit
                TxtKode.Text = kode
                TxtNama.Text = nama
                TxtAlamat.Text = If(row.Cells("ColAlamat").Value IsNot Nothing, row.Cells("ColAlamat").Value.ToString(), "")
                TxtTelp.Text = If(row.Cells("ColTelp").Value IsNot Nothing, row.Cells("ColTelp").Value.ToString(), "")
                CmbJenis.Text = If(row.Cells("ColJenis").Value IsNot Nothing, row.Cells("ColJenis").Value.ToString(), "")
                TxtJangkaPiutang.Text = If(row.Cells("ColJangka").Value IsNot Nothing, row.Cells("ColJangka").Value.ToString(), "0")
                TxtAwal.Text = If(row.Cells("ColHutangAwal").Value IsNot Nothing, Convert.ToDecimal(row.Cells("ColHutangAwal").Value).ToString("0"), "0")
                TxtSisa.Text = If(row.Cells("ColHutangAkhir").Value IsNot Nothing, Convert.ToDecimal(row.Cells("ColHutangAkhir").Value).ToString("0"), "0")

                ' Task 9.1b — kunci TxtAwal jika HutangAwal > 0 agar tidak bisa diubah
                Dim hutangAwalEdit As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
                If hutangAwalEdit > 0 Then
                    TxtAwal.ReadOnly = True
                    TxtAwal.BackColor = SystemColors.Control
                    _toolTip.SetToolTip(TxtAwal, "Saldo awal tidak bisa diubah setelah ada nilai. Gunakan jurnal manual untuk koreksi.")
                Else
                    TxtAwal.ReadOnly = False
                    TxtAwal.BackColor = SystemColors.Window
                    _toolTip.SetToolTip(TxtAwal, "")
                End If

                TxtNama.Focus()

            Case "ColHapus"
                Dim hutang As Decimal = 0D
                If row.Cells("ColHutangAkhir").Value IsNot Nothing Then
                    hutang = ModuleAngka.ParseDecimal(row.Cells("ColHutangAkhir").Value)
                End If
                Dim statusHapus As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If hutang > 0 OrElse statusHapus = "Nonaktif" Then
                    MessageBox.Show("Pelanggan tidak dapat dihapus karena masih memiliki hutang atau berstatus Nonaktif." & vbCrLf &
                                    "Gunakan tombol 'Nonaktifkan' untuk menonaktifkan pelanggan ini.",
                                    "Tidak Dapat Dihapus", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    HapusPelanggan(kode, nama)
                End If

            Case "ColNonaktif"
                Dim statusNonaktif As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If statusNonaktif = "Aktif" Then
                    NonaktifkanPelanggan(kode, nama)
                Else
                    AktifkanPelanggan(kode, nama)
                End If
        End Select
    End Sub

#End Region

#Region "Auto Kode"
    Public Sub Kodepelanggan()
        Dim existingKodes As New List(Of String)
        Using cmd As New MySqlCommand("SELECT KODE FROM tbl_pelanggan ORDER BY KODE", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        If existingKodes.Count = 0 Then
            TxtKode.Text = "PEL-0001"
            Exit Sub
        End If

        Dim maxKode As String = ""
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "PEL-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "PEL-" & hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub
#End Region


#Region "Simpan"
    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNama.Text) Then
            MessageBox.Show("Nama pelanggan harus diisi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNama.Focus() : Exit Sub
        ElseIf String.IsNullOrWhiteSpace(TxtAlamat.Text) Then
            MessageBox.Show("Alamat pelanggan harus diisi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtAlamat.Focus() : Exit Sub
        ElseIf String.IsNullOrWhiteSpace(CmbJenis.Text) Then
            MessageBox.Show("Jenis pelanggan harus dipilih !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbJenis.Focus() : Exit Sub
        End If

        If _isEditMode Then
            UpdatePelanggan()
        Else
            InsertPelanggan()
        End If
    End Sub

    ' ParseDecimal dan ParseInt lokal dihapus — gunakan ModuleAngka.ParseDecimal / ModuleAngka.ParseInteger

    Private Sub InsertPelanggan()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Cek nama duplikat
            Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_pelanggan WHERE NAMA = @Nama", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", TxtNama.Text)
                If Convert.ToInt32(cmd.ExecuteScalar()) > 0 Then
                    MessageBox.Show("Nama sudah ada dalam database.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TxtNama.Focus()
                    transaction.Rollback()
                    Exit Sub
                End If
            End Using

            Using cmd As New MySqlCommand(
                "INSERT INTO tbl_pelanggan (Kode, Nama, Alamat, NO_TELP, Jenis, JangkaPiutang, HutangAwal, TotalHutang, TotalBayar, HutangAkhir, Status) " &
                "VALUES (@Kode, @Nama, @Alamat, @NoTelp, @Jenis, @Jangka, @HutangAwal, @TotalHutang, @TotalBayar, @HutangAkhir, 'Aktif')", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@NoTelp", TxtTelp.Text)
                cmd.Parameters.AddWithValue("@Jenis", StrConv(CmbJenis.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@Jangka", ModuleAngka.ParseInteger(TxtJangkaPiutang.Text))
                cmd.Parameters.AddWithValue("@HutangAwal", ModuleAngka.ParseDecimal(TxtAwal.Text))
                cmd.Parameters.AddWithValue("@TotalHutang", ModuleAngka.ParseDecimal(TxtTotal.Text))
                cmd.Parameters.AddWithValue("@TotalBayar", ModuleAngka.ParseDecimal(TxtBayar.Text))
                cmd.Parameters.AddWithValue("@HutangAkhir", ModuleAngka.ParseDecimal(TxtSisa.Text))
                cmd.ExecuteNonQuery()
            End Using

            ' Jurnal saldo awal piutang pelanggan
            Dim hutangAwal As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
            If hutangAwal <> 0 Then
                SimpanJurnalSaldoAwal(transaction, "Saldo awal piutang pelanggan " & TxtNama.Text,
                                      "HUTANG BELANJA", "03.01.001",
                                      "MODAL", "04.01.001",
                                      hutangAwal)
            End If

            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_pelanggan", TxtKode.Text, "INSERT", ModuleVariabel.NamaUser)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdatePelanggan()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Ambil saldo awal lama untuk hitung selisih jurnal
            Dim hutangAwalLama As Decimal = 0
            Using cmd As New MySqlCommand("SELECT HutangAwal FROM tbl_pelanggan WHERE Kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Dim val = cmd.ExecuteScalar()
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then hutangAwalLama = Convert.ToDecimal(val)
            End Using

            Using cmd As New MySqlCommand(
                "UPDATE tbl_pelanggan SET Nama = @Nama, Alamat = @Alamat, NO_TELP = @NoTelp, Jenis = @Jenis, " &
                "JangkaPiutang = @Jangka, HutangAwal = @HutangAwal, TotalHutang = @TotalHutang, " &
                "TotalBayar = @TotalBayar, HutangAkhir = @HutangAkhir WHERE Kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@NoTelp", TxtTelp.Text)
                cmd.Parameters.AddWithValue("@Jenis", StrConv(CmbJenis.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@Jangka", ModuleAngka.ParseInteger(TxtJangkaPiutang.Text))
                cmd.Parameters.AddWithValue("@HutangAwal", ModuleAngka.ParseDecimal(TxtAwal.Text))
                cmd.Parameters.AddWithValue("@TotalHutang", ModuleAngka.ParseDecimal(TxtTotal.Text))
                cmd.Parameters.AddWithValue("@TotalBayar", ModuleAngka.ParseDecimal(TxtBayar.Text))
                cmd.Parameters.AddWithValue("@HutangAkhir", ModuleAngka.ParseDecimal(TxtSisa.Text))
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.ExecuteNonQuery()
            End Using

            ' Jurnal selisih perubahan saldo awal
            Dim selisih As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text) - hutangAwalLama
            If selisih <> 0 Then
                Dim uraian As String = "Perubahan saldo awal piutang pelanggan " & TxtNama.Text
                If selisih > 0 Then
                    SimpanJurnalSaldoAwal(transaction, uraian,
                                          "HUTANG BELANJA", "03.01.001",
                                          "MODAL", "04.01.001",
                                          selisih)
                Else
                    SimpanJurnalSaldoAwal(transaction, uraian,
                                          "MODAL", "04.01.001",
                                          "HUTANG BELANJA", "03.01.001",
                                          Math.Abs(selisih))
                End If
            End If

            transaction.Commit()
            ' Task 9.1b — recalculate HutangAkhir setelah simpan agar langsung terupdate di DGV
            UpdatePiutangPelanggan(TxtKode.Text, Nothing)
            SyncTrigger.MasterBerubah("tbl_pelanggan", TxtKode.Text, "UPDATE", ModuleVariabel.NamaUser)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SimpanJurnalSaldoAwal(ByVal transaction As MySqlTransaction,
                                       ByVal uraian As String,
                                       ByVal namaAkunD As String, ByVal nomorAkunD As String,
                                       ByVal namaAkunK As String, ByVal nomorAkunK As String,
                                       ByVal nominal As Decimal)
        Dim noTransaksi As String = "SA-" & DateTime.Now.ToString("yyyyMMddHHmmss") & "-" & TxtKode.Text
        Using cmd As New MySqlCommand(
            "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
            "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, " &
            "NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
            "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, " &
            "@NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, " &
            "@NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", TxtKode.Text)
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", namaAkunD)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkunD)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", namaAkunK)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkunK)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "SALDO AWAL")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
#End Region

#Region "Hapus & Nonaktif"
    Private Sub HapusPelanggan(kode As String, nama As String)
        If MessageBox.Show($"Hapus pelanggan '{nama}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_pelanggan WHERE KODE = @Kode", conn, transaction)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                transaction.Commit()
                Kondisiawal()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub NonaktifkanPelanggan(kode As String, nama As String)
        If MessageBox.Show($"Nonaktifkan pelanggan '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_pelanggan SET Status = 'Nonaktif' WHERE KODE = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                TampilPelanggan()
            Catch ex As Exception
                MessageBox.Show("Gagal menonaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub AktifkanPelanggan(kode As String, nama As String)
        If MessageBox.Show($"Aktifkan kembali pelanggan '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_pelanggan SET Status = 'Aktif' WHERE KODE = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                TampilPelanggan()
            Catch ex As Exception
                MessageBox.Show("Gagal mengaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
#End Region

#Region "Input Helper"
    Private Sub TxtValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotal.TextChanged, TxtAwal.TextChanged, TxtBayar.TextChanged
        Dim awal As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
        Dim total As Decimal = ModuleAngka.ParseDecimal(TxtTotal.Text)
        Dim bayar As Decimal = ModuleAngka.ParseDecimal(TxtBayar.Text)
        TxtSisa.Text = (awal + total - bayar).ToString()
    End Sub

    Private Sub TxtAwal_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtAwal.KeyPress
        If Not (Char.IsDigit(e.KeyChar) Or e.KeyChar = "." Or e.KeyChar = "," Or e.KeyChar = vbBack) Then
            e.Handled = True
        End If
    End Sub
#End Region

#Region "Tombol & Keyboard"
    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Kondisiawal()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub TambahPelanggan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BTNSimpan.PerformClick()
            Case Keys.F4 : BtnTambah.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub




#End Region

End Class

