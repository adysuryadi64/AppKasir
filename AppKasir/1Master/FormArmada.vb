Public Class FormArmada

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit FormArmada ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("ARMADA:" & kode, "HAPUS"/"EDIT",
    '       "Master armada", snapshotJson, "[KRITIS] Hapus/Edit data armada", trans)
    ' Snapshot: baca data lama dari tbl_Armada sebelum DELETE/UPDATE
    ' ─────────────────────────────────────────────────────────────────────────

    Private _isLoading As Boolean = False
    Private _isEditMode As Boolean = False
    Private _canAdd As Boolean = True
    Private _canEdit As Boolean = True
    Private _canDelete As Boolean = True

#Region "Form Load"
    Private Sub FormArmada_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Dim hakAkses As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Armada")
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
        PanelHeader.Text = "TAMBAH ARMADA"
        BTNSimpan.Text = "SIMPAN (F2)"
        TxtKode.Clear()
        TxtNopol.Clear()
        TxtJenis.Clear()
        KodeArmada()
        TampilArmada()
        TxtNopol.Focus()
    End Sub
#End Region

#Region "DataGridView"
    Public Sub TampilArmada()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT KODE, NOPOL, JENIS FROM tbl_Armada ORDER BY NOPOL", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Dgvdata.DataSource = Nothing
        Dgvdata.Rows.Clear()
        Dgvdata.Columns.Clear()

        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 80, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNopol", .HeaderText = "No. Polisi", .FillWeight = 160, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColJenis", .HeaderText = "Jenis", .FillWeight = 160, .ReadOnly = True})

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
            Dim rowIdx As Integer = Dgvdata.Rows.Add(row("KODE"), row("NOPOL"), row("JENIS"))

            If _canEdit Then
                ModuleTheme.SetWarnaDgvBtnEdit(Dgvdata.Rows(rowIdx).Cells("ColEdit"), True)
            End If
            If _canDelete Then
                ModuleTheme.SetWarnaDgvBtnHapus(Dgvdata.Rows(rowIdx).Cells("ColHapus"), True)
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
        Dim nopol As String = If(row.Cells("ColNopol").Value IsNot Nothing, row.Cells("ColNopol").Value.ToString(), "")

        Select Case Dgvdata.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                PanelHeader.Text = "EDIT ARMADA"
                BTNSimpan.Text = "UPDATE (F2)"
                TxtKode.Text = kode
                TxtNopol.Text = nopol
                TxtJenis.Text = If(row.Cells("ColJenis").Value IsNot Nothing, row.Cells("ColJenis").Value.ToString(), "")
                TxtNopol.Focus()

            Case "ColHapus"
                HapusArmada(kode, nopol)
        End Select
    End Sub

#End Region

#Region "Auto Kode"
    Public Sub KodeArmada()
        Dim existingKodes As New List(Of String)
        Using cmd As New MySqlCommand("SELECT KODE FROM tbl_Armada ORDER BY KODE", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        If existingKodes.Count = 0 Then
            TxtKode.Text = "ARM-0001"
            Exit Sub
        End If

        Dim maxKode As String = ""
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "ARM-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "ARM-" & hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub
#End Region

#Region "Simpan"
    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNopol.Text) OrElse String.IsNullOrWhiteSpace(TxtJenis.Text) Then
            MessageBox.Show("Data harus diisi dengan lengkap !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If _isEditMode Then
            UpdateArmada()
        Else
            InsertArmada()
        End If
    End Sub

    Private Sub InsertArmada()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmd As New MySqlCommand("INSERT INTO tbl_Armada (KODE, NOPOL, JENIS) VALUES (@KODE, @NOPOL, @JENIS)", conn, transaction)
                cmd.Parameters.AddWithValue("@KODE", TxtKode.Text)
                cmd.Parameters.AddWithValue("@NOPOL", StrConv(TxtNopol.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@JENIS", StrConv(TxtJenis.Text, vbProperCase))
                cmd.ExecuteNonQuery()
            End Using
            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_armada", TxtKode.Text, "INSERT", ModuleVariabel.NamaUser)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateArmada()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmd As New MySqlCommand("UPDATE tbl_Armada SET NOPOL = @NOPOL, JENIS = @JENIS WHERE KODE = @KODE", conn, transaction)
                cmd.Parameters.AddWithValue("@NOPOL", StrConv(TxtNopol.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@JENIS", StrConv(TxtJenis.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@KODE", TxtKode.Text)
                cmd.ExecuteNonQuery()
            End Using
            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_armada", TxtKode.Text, "UPDATE", ModuleVariabel.NamaUser)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Hapus"
    Private Sub HapusArmada(kode As String, nopol As String)
        If MessageBox.Show($"Hapus armada '{nopol}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_Armada WHERE KODE = @KODE", conn, transaction)
                    cmd.Parameters.AddWithValue("@KODE", kode)
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
#End Region

#Region "Tombol & Keyboard"
    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Kondisiawal()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub FormArmada_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BTNSimpan.PerformClick()
            Case Keys.F4 : BtnTambah.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub



#End Region

End Class

