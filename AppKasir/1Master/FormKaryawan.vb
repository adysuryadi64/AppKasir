Public Class FormKaryawan

    Private _isLoading As Boolean = False
    Private _isEditMode As Boolean = False
    Private _canAdd As Boolean = True
    Private _canEdit As Boolean = True
    Private _canDelete As Boolean = True

#Region "Form Load"
    Private Sub FormKaryawan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Dim hakAkses As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Karyawan")
        _canAdd = hakAkses(1)
        _canEdit = hakAkses(2)
        _canDelete = hakAkses(3)

        BTNSimpan.Visible = _canAdd

        Kondisiawal()
        UpdateTotalBonDanTotalBayarKaryawan()
        Me.Cursor = Cursors.Default
    End Sub
#End Region

#Region "Kondisi Awal"
    Public Sub Kondisiawal()
        _isEditMode = False
        PanelHeader.Text = "TAMBAH KARYAWAN"
        BTNSimpan.Text = "SIMPAN (F2)"
        TxtKode.Clear()
        TxtNama.Clear()
        TxtJabatan.Clear()
        TxtAwal.Clear()
        DtpTransaksi.Value = DateTime.Today
        DtpTransaksi.Format = DateTimePickerFormat.Custom
        DtpTransaksi.CustomFormat = "dd/MM/yyyy"
        KodeKaryawan()
        TampilKaryawan()
        TxtNama.Focus()
    End Sub
#End Region

#Region "DataGridView"
    Public Sub TampilKaryawan()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT Kode, Nama, Jabatan, TglMasuk, Gaji, SaldoAkhir, Status FROM tbl_karyawan ORDER BY Nama", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        ' Reset DGV sepenuhnya
        Dgvdata.DataSource = Nothing
        Dgvdata.Rows.Clear()
        Dgvdata.Columns.Clear()

        ' Kolom data
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 80, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama Karyawan", .FillWeight = 160, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColJabatan", .HeaderText = "Jabatan", .FillWeight = 120, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColTglMasuk", .HeaderText = "Tgl Masuk", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColGaji", .HeaderText = "Gaji", .FillWeight = 100, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColSaldo", .HeaderText = "Saldo Bon", .FillWeight = 100, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColStatus", .HeaderText = "Status", .FillWeight = 70, .ReadOnly = True})

        ModuleAngka.TerapkanFormatKolomAngka(Dgvdata, "ColGaji", "ColSaldo")

        ' Kolom tombol Edit
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

        ' Isi baris data
        For Each row As DataRow In dt.Rows
            Dim status As String = If(row("Status") Is DBNull.Value, "Aktif", row("Status").ToString())
            Dim saldo As Decimal = If(row("SaldoAkhir") Is DBNull.Value, 0D, Convert.ToDecimal(row("SaldoAkhir")))
            Dim rowIdx As Integer = Dgvdata.Rows.Add(
                row("Kode"), row("Nama"), row("Jabatan"),
                Convert.ToDateTime(row("TglMasuk")).ToString("dd/MM/yyyy"),
                row("Gaji"), row("SaldoAkhir"), status)

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
                ModuleTheme.SetWarnaDgvBtnHapus(Dgvdata.Rows(rowIdx).Cells("ColHapus"), saldo = 0 AndAlso status <> "Nonaktif")
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
                PanelHeader.Text = "EDIT KARYAWAN"
                BTNSimpan.Text = "UPDATE (F2)"
                TxtKode.Text = kode
                TxtNama.Text = nama
                TxtJabatan.Text = If(row.Cells("ColJabatan").Value IsNot Nothing, row.Cells("ColJabatan").Value.ToString(), "")
                ' Ambil TglMasuk dari DB langsung agar format aman
                Using cmd As New MySqlCommand("SELECT TglMasuk, Gaji FROM tbl_karyawan WHERE Kode = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            DtpTransaksi.Value = ModuleAngka.SafeGetValue(Of DateTime)(rd, "TglMasuk", DtpTransaksi.Value)
                            TxtAwal.Text = ModuleAngka.ParseDecimal(rd("Gaji")).ToString("0")
                        End If
                    End Using
                End Using
                TxtNama.Focus()

            Case "ColNonaktif"
                Dim status As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If status = "Aktif" Then
                    NonaktifkanKaryawan(kode, nama)
                Else
                    AktifkanKaryawan(kode, nama)
                End If

            Case "ColHapus"
                Dim saldo As Decimal = 0D
                If row.Cells("ColSaldo").Value IsNot Nothing Then Decimal.TryParse(row.Cells("ColSaldo").Value.ToString(), saldo)
                Dim statusHapus As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If saldo <> 0 OrElse statusHapus = "Nonaktif" Then
                    MessageBox.Show("Karyawan tidak dapat dihapus karena masih memiliki saldo bon atau berstatus Nonaktif." & vbCrLf &
                                    "Gunakan tombol 'Nonaktifkan' untuk menonaktifkan karyawan ini.",
                                    "Tidak Dapat Dihapus", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    HapusKaryawan(kode, nama)
                End If
        End Select
    End Sub

#End Region

#Region "Auto Kode"
    Public Sub KodeKaryawan()
        Dim existingKodes As New List(Of String)
        Using cmd As New MySqlCommand("SELECT kode FROM tbl_karyawan ORDER BY Kode", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        If existingKodes.Count = 0 Then
            TxtKode.Text = "KRY-0001"
            Exit Sub
        End If

        Dim maxKode As String = ""
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "KRY-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "KRY-" & hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub
#End Region

#Region "Simpan"
    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNama.Text) OrElse String.IsNullOrWhiteSpace(TxtJabatan.Text) Then
            MessageBox.Show("Data harus diisi dengan lengkap !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        TxtAwal.Text = If(String.IsNullOrEmpty(TxtAwal.Text), "0", TxtAwal.Text)

        If _isEditMode Then
            UpdateKaryawan()
        Else
            InsertKaryawan()
        End If
    End Sub

    Private Sub InsertKaryawan()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Dim gaji As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
            Using cmd As New MySqlCommand("INSERT INTO tbl_karyawan (Kode, Nama, Jabatan, TglMasuk, Gaji, Status) VALUES (@Kode, @Nama, @Jabatan, @TglMasuk, @Gaji, 'Aktif')", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Jabatan", StrConv(TxtJabatan.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@TglMasuk", DtpTransaksi.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@Gaji", gaji)
                cmd.ExecuteNonQuery()
            End Using
            transaction.Commit()
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateKaryawan()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Dim gaji As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)

            ' ========================================
            ' START: Audit Trail - Edit Karyawan
            ' ========================================
            Dim sbSnapshot As New System.Text.StringBuilder()
            Using oldCmd As New MySqlCommand(
                "SELECT Kode, Nama, Jabatan, TglMasuk, Gaji FROM tbl_karyawan WHERE Kode = @Kode LIMIT 1", conn, transaction)
                oldCmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Using oldRd As MySqlDataReader = oldCmd.ExecuteReader()
                    If oldRd.Read() Then
                        sbSnapshot.AppendLine($"Kode Karyawan: {oldRd("Kode")}")
                        sbSnapshot.AppendLine($"Nama: {oldRd("Nama")}")
                        sbSnapshot.AppendLine($"Jabatan: {oldRd("Jabatan")}")
                        sbSnapshot.AppendLine($"Tgl Masuk: {oldRd("TglMasuk")}")
                        sbSnapshot.AppendLine($"Gaji Pokok (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Gaji")))}")
                        sbSnapshot.AppendLine($"Gaji Pokok (sesudah): {ModuleAngka.FormatRupiah(gaji)}")
                    End If
                End Using
            End Using
            ModuleAuditTrail.CatatAuditMaster("KRY:" & TxtKode.Text, "EDIT", "Master Karyawan", sbSnapshot.ToString(), trans:=transaction)
            ' ========================================
            ' END: Audit Trail - Edit Karyawan
            ' ========================================

            Using cmd As New MySqlCommand("UPDATE tbl_karyawan SET Nama = @Nama, Jabatan = @Jabatan, TglMasuk = @TglMasuk, Gaji = @Gaji WHERE Kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Jabatan", StrConv(TxtJabatan.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@TglMasuk", DtpTransaksi.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@Gaji", gaji)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.ExecuteNonQuery()
            End Using
            transaction.Commit()
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Hapus & Nonaktif"
    Private Sub HapusKaryawan(kode As String, nama As String)
        If MessageBox.Show($"Hapus karyawan '{nama}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' ========================================
                ' START: Audit Trail - Hapus Karyawan
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Using oldCmd As New MySqlCommand(
                    "SELECT Kode, Nama, Jabatan, TglMasuk, Gaji FROM tbl_karyawan WHERE Kode = @Kode LIMIT 1", conn, transaction)
                    oldCmd.Parameters.AddWithValue("@Kode", kode)
                    Using oldRd As MySqlDataReader = oldCmd.ExecuteReader()
                        If oldRd.Read() Then
                            sbSnapshot.AppendLine($"Kode Karyawan: {oldRd("Kode")}")
                            sbSnapshot.AppendLine($"Nama: {oldRd("Nama")}")
                            sbSnapshot.AppendLine($"Jabatan: {oldRd("Jabatan")}")
                            sbSnapshot.AppendLine($"Tgl Masuk: {oldRd("TglMasuk")}")
                            sbSnapshot.AppendLine($"Gaji Pokok: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Gaji")))}")
                        End If
                    End Using
                End Using
                ModuleAuditTrail.CatatAuditMaster("KRY:" & kode, "HAPUS", "Master Karyawan", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Hapus Karyawan
                ' ========================================
                Using cmd As New MySqlCommand("DELETE FROM tbl_karyawan WHERE Kode = @Kode", conn, transaction)
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

    Private Sub NonaktifkanKaryawan(kode As String, nama As String)
        If MessageBox.Show($"Nonaktifkan karyawan '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_karyawan SET Status = 'Nonaktif' WHERE Kode = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                TampilKaryawan()
            Catch ex As Exception
                MessageBox.Show("Gagal menonaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub AktifkanKaryawan(kode As String, nama As String)
        If MessageBox.Show($"Aktifkan kembali karyawan '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_karyawan SET Status = 'Aktif' WHERE Kode = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                TampilKaryawan()
            Catch ex As Exception
                MessageBox.Show("Gagal mengaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Sub TambahSupliyer_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BTNSimpan.PerformClick()
            Case Keys.F4 : BtnTambah.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub
#End Region

#Region "Input Helper"
    Private Sub TxtValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtAwal.TextChanged
        Dim awal As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
        Label6.Text = "Rp. " & ModuleAngka.FormatAngka(awal)
    End Sub

    Private Sub TxtAwal_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtAwal.KeyPress
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

#End Region

End Class
