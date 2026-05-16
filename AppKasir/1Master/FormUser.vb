Public Class FormUser

    Private _isEditMode As Boolean = False
    Private _isLoading As Boolean = False

#Region "Form Load"
    Private Sub Form_User_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor

        Dim hakAkses As Boolean() = ModulHakAkses.BacaHakAksesDariCache("User")
        ' Simpan hak akses untuk dipakai saat rebuild kolom DGV
        _canAdd = hakAkses(1)
        _canEdit = hakAkses(2)
        _canDelete = hakAkses(3)

        ' Tombol Simpan & Hapus di panel kiri tetap ikut hak akses
        BTNSimpan.Visible = _canAdd

        KondisiAwal()
        Me.Cursor = Cursors.Default
    End Sub

    Private _canAdd As Boolean = True
    Private _canEdit As Boolean = True
    Private _canDelete As Boolean = True
#End Region

#Region "DataGridView"
    Public Sub Tampil_user()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT kode_user, nama_user, user_name, pwd, lvl, status FROM tbl_user ORDER BY nama_user", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        ' Reset DGV sepenuhnya
        DgvData.DataSource = Nothing
        DgvData.Rows.Clear()
        DgvData.Columns.Clear()

        ' Kolom data
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 60, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama User", .FillWeight = 140, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColUsername", .HeaderText = "Username", .FillWeight = 100, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColPwd", .HeaderText = "Password", .FillWeight = 80, .ReadOnly = True, .Visible = False})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColLevel", .HeaderText = "Level", .FillWeight = 70, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColStatus", .HeaderText = "Status", .FillWeight = 60, .ReadOnly = True})

        ' Kolom tombol Edit (tampil sesuai hak akses)
        If _canEdit Then
            Dim colEdit As New DataGridViewButtonColumn() With {
                .Name = "ColEdit", .HeaderText = "Edit",
                .Text = "✎ Edit", .UseColumnTextForButtonValue = True,
                .Width = 70, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            DgvData.Columns.Add(colEdit)
        End If

        If _canDelete Then
            Dim colNonaktif As New DataGridViewButtonColumn() With {
                .Name = "ColNonaktif", .HeaderText = "Nonaktif",
                .UseColumnTextForButtonValue = False,
                .Width = 100, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            DgvData.Columns.Add(colNonaktif)

            ' Kolom tombol Hapus (tampil sesuai hak akses)
            Dim colHapus As New DataGridViewButtonColumn() With {
                .Name = "ColHapus", .HeaderText = "Hapus",
                .Text = "✖ Hapus", .UseColumnTextForButtonValue = True,
                .Width = 75, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            DgvData.Columns.Add(colHapus)
        End If

        ' Isi baris data
        For Each row As DataRow In dt.Rows
            Dim status As String = row("status").ToString()
            Dim idx As Integer = DgvData.Rows.Add(
                row("kode_user"), row("nama_user"), row("user_name"),
                row("pwd"), row("lvl"), row("status"))

            ' Warna baris Non Aktif
            ModuleTheme.SetWarnaBarisDgvNonaktif(DgvData.Rows(idx), status = "Non Aktif")

            ' Tombol Edit — ikut warna status
            If _canEdit Then
                ModuleTheme.SetWarnaDgvBtnEdit(DgvData.Rows(idx).Cells("ColEdit"), status <> "Non Aktif")
            End If

            If _canDelete Then
                ' Tombol Nonaktif/Aktifkan
                If status = "Non Aktif" Then
                    DgvData.Rows(idx).Cells("ColNonaktif").Value = "✔ Aktifkan"
                Else
                    DgvData.Rows(idx).Cells("ColNonaktif").Value = "⊘ Nonaktifkan"
                End If
                ModuleTheme.SetWarnaDgvBtnStatus(DgvData.Rows(idx).Cells("ColNonaktif"), status <> "Non Aktif")

                ' Tombol Hapus — merah jika aktif, abu-abu jika non aktif
                ModuleTheme.SetWarnaDgvBtnHapus(DgvData.Rows(idx).Cells("ColHapus"), status <> "Non Aktif")
            End If
        Next

        ' Pengaturan standar dan tema DGV
        ModuleTheme.ApplyStandardDataGridViewSettings(DgvData)
        ModuleTheme.ApplyThemeDataGridView(DgvData)

        DgvData.ClearSelection()
        _isLoading = False
    End Sub

    Private Sub DgvData_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvData.CellContentClick
        If _isLoading OrElse e.RowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = DgvData.Rows(e.RowIndex)
        Dim kode As String = If(row.Cells("ColKode").Value IsNot Nothing, row.Cells("ColKode").Value.ToString(), "")
        Dim nama As String = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")

        Select Case DgvData.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                LblHeader.Text = "EDIT USER"
                BTNSimpan.Text = "UPDATE (F2)"
                TxtKode.Text = kode
                TxtNama.Text = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")
                TxtUsername.Text = If(row.Cells("ColUsername").Value IsNot Nothing, row.Cells("ColUsername").Value.ToString(), "")
                TxtPassword.Clear()
                CmbLevel.Text = If(row.Cells("ColLevel").Value IsNot Nothing, row.Cells("ColLevel").Value.ToString(), "")
                ' Tampilkan field password lama untuk konfirmasi
                Label7.Visible = True
                TxtPAsswordLama.Visible = True
                TxtPAsswordLama.Clear()
                TxtNama.Focus()

            Case "ColNonaktif"
                Dim status As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If status = "Non Aktif" Then
                    AktifkanUser(kode, nama)
                Else
                    NonaktifkanUser(kode, nama)
                End If

            Case "ColHapus"
                Dim statusHapus As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If statusHapus = "Non Aktif" Then
                    MessageBox.Show("User Non Aktif tidak dapat dihapus. Aktifkan terlebih dahulu.",
                                    "Tidak Dapat Dihapus", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    HapusUser(kode, nama)
                End If
        End Select
    End Sub

#End Region

#Region "Kondisi Awal"
    Public Sub KondisiAwal()
        _isEditMode = False
        LblHeader.Text = "TAMBAH USER"
        BTNSimpan.Text = "SIMPAN (F2)"
        TxtKode.Clear()
        TxtNama.Clear()
        TxtUsername.Clear()
        TxtPassword.Clear()
        CmbLevel.Text = ""
        TxtPAsswordLama.Clear()
        Label7.Visible = False
        TxtPAsswordLama.Visible = False
        Kodeuser()
        Tampil_user()
        TxtNama.Focus()
    End Sub

    ' Alias untuk kompatibilitas kode lama
    Public Sub Bersih()
        KondisiAwal()
    End Sub
#End Region

#Region "Auto Kode"
    Public Sub Kodeuser()
        Dim existingKodes As New List(Of String)
        Using cmd As New MySqlCommand("SELECT kode_user FROM tbl_user ORDER BY kode_user", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        If existingKodes.Count = 0 Then
            TxtKode.Text = "US-001"
            Exit Sub
        End If

        Dim maxKode As String = ""
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "US-" & i.ToString("000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 3)) + 1
            maxKode = "US-" & hitung.ToString("000")
        End If

        TxtKode.Text = maxKode
    End Sub
#End Region

#Region "MD5"
    Public Shared Function MD5DELISMAN(ByVal strToHash As String) As String
        Using MD5HULU As New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)
            Using md5Hash = MD5HULU
                bytesToHash = md5Hash.ComputeHash(bytesToHash)
            End Using
            Dim strResult As String = ""
            For Each b As Byte In bytesToHash
                strResult += b.ToString("x2")
            Next
            Return strResult
        End Using
    End Function
#End Region

#Region "Simpan"
    Private Sub BTNSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNama.Text) Then
            MessageBox.Show("Nama user wajib diisi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNama.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(TxtUsername.Text) Then
            MessageBox.Show("Username wajib diisi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtUsername.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(TxtPassword.Text) Then
            MessageBox.Show("Password wajib diisi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtPassword.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(CmbLevel.Text) Then
            MessageBox.Show("Level wajib dipilih", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbLevel.Focus()
            Exit Sub
        End If

        If _isEditMode Then
            ' Mode edit: wajib isi password lama
            If String.IsNullOrWhiteSpace(TxtPAsswordLama.Text) Then
                MessageBox.Show("Isi Password lama untuk konfirmasi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtPAsswordLama.Focus()
                Exit Sub
            End If
            UpdateUser()
        Else
            InsertUser()
        End If
    End Sub

    Private Sub InsertUser()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Cek username duplikat
            Using cmd As New MySqlCommand("SELECT user_name FROM tbl_user WHERE user_name = @user_name", conn, transaction)
                cmd.Parameters.AddWithValue("@user_name", TxtUsername.Text)
                If cmd.ExecuteScalar() IsNot Nothing Then
                    MessageBox.Show("Username sudah ada, silahkan ganti dengan yang lain!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TxtUsername.Focus()
                    transaction.Rollback()
                    Exit Sub
                End If
            End Using

            Using cmd As New MySqlCommand("INSERT INTO tbl_user (kode_user, nama_user, user_name, pwd, lvl, status) VALUES (@kode, @nama, @username, @password, @level, @status)", conn, transaction)
                cmd.Parameters.AddWithValue("@kode", TxtKode.Text)
                cmd.Parameters.AddWithValue("@nama", StrConv(TxtNama.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@username", StrConv(TxtUsername.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@password", MD5DELISMAN(TxtPassword.Text))
                cmd.Parameters.AddWithValue("@level", CmbLevel.Text)
                cmd.Parameters.AddWithValue("@status", "Aktif")
                cmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            KondisiAwal()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal menyimpan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateUser()
        ' Verifikasi password lama
        Dim pwdLama As String = MD5DELISMAN(TxtPAsswordLama.Text)
        Using cmdCek As New MySqlCommand("SELECT COUNT(*) FROM tbl_user WHERE kode_user = @kode AND pwd = @pwd", conn)
            cmdCek.Parameters.AddWithValue("@kode", TxtKode.Text)
            cmdCek.Parameters.AddWithValue("@pwd", pwdLama)
            Dim jumlah As Integer = Convert.ToInt32(cmdCek.ExecuteScalar())
            If jumlah = 0 Then
                MessageBox.Show("Password lama salah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtPAsswordLama.Focus()
                Exit Sub
            End If
        End Using

        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' ========================================
            ' START: Audit Trail - Edit User
            ' ========================================
            Dim sbSnapshot As New System.Text.StringBuilder()
            Using cmdSnap As New MySqlCommand(
                "SELECT kode_user, nama_user, user_name, lvl, status FROM tbl_user WHERE kode_user = @k", conn, transaction)
                cmdSnap.Parameters.AddWithValue("@k", TxtKode.Text)
                Using rdSnap = cmdSnap.ExecuteReader()
                    If rdSnap.Read() Then
                        sbSnapshot.AppendLine($"Kode User: {rdSnap("kode_user")}")
                        sbSnapshot.AppendLine($"Nama User: {rdSnap("nama_user")}")
                        sbSnapshot.AppendLine($"Username: {rdSnap("user_name")}")
                        sbSnapshot.AppendLine($"Level: {rdSnap("lvl")}")
                        sbSnapshot.AppendLine($"Status: {rdSnap("status")}")
                    End If
                End Using
            End Using
            ModuleAuditTrail.CatatAuditMaster("USER:" & TxtKode.Text, "EDIT", "Master User", sbSnapshot.ToString(), trans:=transaction)
            ' ========================================
            ' END: Audit Trail - Edit User
            ' ========================================

            Using cmd As New MySqlCommand("UPDATE tbl_user SET nama_user = @nama, user_name = @username, pwd = @password, lvl = @level WHERE kode_user = @kode", conn, transaction)
                cmd.Parameters.AddWithValue("@nama", StrConv(TxtNama.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@username", StrConv(TxtUsername.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@password", MD5DELISMAN(TxtPassword.Text))
                cmd.Parameters.AddWithValue("@level", CmbLevel.Text)
                cmd.Parameters.AddWithValue("@kode", TxtKode.Text)
                cmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            KondisiAwal()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal mengupdate data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Hapus & Nonaktif"
    Private Sub HapusUser(kode As String, nama As String)
        If MessageBox.Show($"Hapus user '{nama}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' ========================================
                ' START: Audit Trail - Hapus User
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Using cmdSnap As New MySqlCommand(
                    "SELECT kode_user, nama_user, user_name, lvl, status FROM tbl_user WHERE kode_user = @k", conn, transaction)
                    cmdSnap.Parameters.AddWithValue("@k", kode)
                    Using rdSnap = cmdSnap.ExecuteReader()
                        If rdSnap.Read() Then
                            sbSnapshot.AppendLine($"Kode User: {rdSnap("kode_user")}")
                            sbSnapshot.AppendLine($"Nama User: {rdSnap("nama_user")}")
                            sbSnapshot.AppendLine($"Username: {rdSnap("user_name")}")
                            sbSnapshot.AppendLine($"Level: {rdSnap("lvl")}")
                            sbSnapshot.AppendLine($"Status: {rdSnap("status")}")
                        End If
                    End Using
                End Using
                ModuleAuditTrail.CatatAuditMaster("USER:" & kode, "HAPUS", "Master User", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Hapus User
                ' ========================================

                Using cmd As New MySqlCommand("DELETE FROM tbl_user WHERE kode_user = @kode", conn, transaction)
                    cmd.Parameters.AddWithValue("@kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                transaction.Commit()
                KondisiAwal()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub NonaktifkanUser(kode As String, nama As String)
        If MessageBox.Show($"Nonaktifkan user '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_user SET status = 'Non Aktif' WHERE kode_user = @kode", conn)
                    cmd.Parameters.AddWithValue("@kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                Tampil_user()
            Catch ex As Exception
                MessageBox.Show("Gagal menonaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub AktifkanUser(kode As String, nama As String)
        If MessageBox.Show($"Aktifkan kembali user '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_user SET status = 'Aktif' WHERE kode_user = @kode", conn)
                    cmd.Parameters.AddWithValue("@kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                Tampil_user()
            Catch ex As Exception
                MessageBox.Show("Gagal mengaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
#End Region

#Region "Tombol & Keyboard"
    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        KondisiAwal()
    End Sub

    Private Sub BTNKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNKeluar.Click
        Close()
    End Sub

    Private Sub Form_User_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BTNSimpan.PerformClick()
            Case Keys.F4 : BtnTambah.PerformClick()
            Case Keys.Escape : BTNKeluar.PerformClick()
        End Select
    End Sub
#End Region

End Class
