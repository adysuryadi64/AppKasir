Public Class TambahMerk

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit TambahMerk ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("MRK:" & kode, "HAPUS"/"EDIT",
    '       "Master merk", snapshotJson, "[KRITIS] Hapus/Edit data merk", trans)
    ' Snapshot: baca data lama dari tbl_merk sebelum DELETE/UPDATE
    ' ─────────────────────────────────────────────────────────────────────────

    Private _isEditMode As Boolean = False
    Private _isLoading As Boolean = False

#Region "Form Load"
    Private Sub TambahMerk_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub
#End Region

#Region "DataGridView"
    Public Sub TampilMerk()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT kode, nama, keterangan FROM tbl_merk ORDER BY nama", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        DgvData.DataSource = Nothing
        DgvData.Rows.Clear()
        DgvData.Columns.Clear()

        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 60, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama", .FillWeight = 140, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKeterangan", .HeaderText = "Keterangan", .FillWeight = 160, .ReadOnly = True})

        Dim colEdit As New DataGridViewButtonColumn() With {
            .Name = "ColEdit", .HeaderText = "Edit",
            .Text = "✎ Edit", .UseColumnTextForButtonValue = True,
            .Width = 70, .FlatStyle = FlatStyle.Flat,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        }
        ' Font diatur oleh tema - Calibri 9.75 Bold
        DgvData.Columns.Add(colEdit)

        Dim colHapus As New DataGridViewButtonColumn() With {
            .Name = "ColHapus", .HeaderText = "Hapus",
            .Text = "✖ Hapus", .UseColumnTextForButtonValue = True,
            .Width = 75, .FlatStyle = FlatStyle.Flat,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        }
        ' Font diatur oleh tema - Calibri 9.75 Bold
        DgvData.Columns.Add(colHapus)

        For Each row As DataRow In dt.Rows
            Dim rowIdx As Integer = DgvData.Rows.Add(row("kode"), row("nama"), row("keterangan"))
            ModuleTheme.SetWarnaDgvBtnEdit(DgvData.Rows(rowIdx).Cells("ColEdit"), True)
            ModuleTheme.SetWarnaDgvBtnHapus(DgvData.Rows(rowIdx).Cells("ColHapus"), True)
        Next

        ' Pengaturan standar dan tema DGV
        ModuleTheme.ApplyStandardDataGridViewSettings(DgvData)
        ModuleTheme.ApplyThemeDataGridView(DgvData)

        DgvData.ClearSelection()
        _isLoading = False
    End Sub

    Private Sub DgvData_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellContentClick
        If _isLoading OrElse e.RowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = DgvData.Rows(e.RowIndex)
        Dim kode As String = If(row.Cells("ColKode").Value IsNot Nothing, row.Cells("ColKode").Value.ToString(), "")
        Dim nama As String = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")
        Dim ket As String = If(row.Cells("ColKeterangan").Value IsNot Nothing, row.Cells("ColKeterangan").Value.ToString(), "")

        Select Case DgvData.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                LblHeader.Text = "EDIT MERK"
                BtnSimpan.Text = "UPDATE (F2)"
                ' Mode Edit: Kode tidak boleh diubah, CB Manual disembunyikan
                CBManual.Visible = False
                TxtKode.ReadOnly = True
                TxtKode.Text = kode
                TxtNama.Text = nama
                TxtKeterangan.Text = ket
                TxtNama.Focus()

            Case "ColHapus"
                HapusMerk(kode, nama)
        End Select
    End Sub

#End Region

#Region "Kondisi Awal"
    Public Sub Kondisiawal()
        _isEditMode = False
        LblHeader.Text = "TAMBAH MERK"
        BtnSimpan.Text = "SIMPAN (F2)"
        ' Mode Tambah: CBManual visible, unchecked default
        ' Kode readonly, generate otomatis saat Leave dari field Nama
        CBManual.Visible = True
        CBManual.Enabled = True
        CBManual.Checked = False
        TxtKode.ReadOnly = True
        TxtKode.Text = ""
        TxtNama.Text = ""
        TxtKeterangan.Text = ""
        TampilMerk()
        TxtNama.Select()
    End Sub
#End Region

#Region "Auto Kode"
    ''' <summary>
    ''' Generate singkatan dari nama merk
    ''' - 1 kata: 3 huruf pertama
    ''' - 2 kata: 1 huruf kata1 + 2 huruf kata2
    ''' - 3+ kata: 1 huruf dari 3 kata pertama
    ''' </summary>
    Public Function GenerateSingkatan(nama As String) As String
        If String.IsNullOrWhiteSpace(nama) Then Return ""

        Dim words() As String = nama.Trim().Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        Dim result As String = ""

        If words.Length = 1 Then
            Dim kata = words(0)
            result = kata.Substring(0, Math.Min(3, kata.Length))
        ElseIf words.Length = 2 Then
            Dim kata1 = words(0)
            Dim kata2 = words(1)
            result = kata1.Substring(0, 1) & kata2.Substring(0, Math.Min(2, kata2.Length))
        Else
            For i As Integer = 0 To Math.Min(2, words.Length - 1)
                result &= words(i).Substring(0, 1)
            Next
        End If

        Return result.ToUpper()
    End Function

    ''' <summary>
    ''' Cek apakah kode sudah ada di database, jika ya generate alternatif.
    ''' Kode merk dibatasi maksimal 4 karakter sesuai VARCHAR(4) di database.
    ''' Normal generate 3 huruf, karakter ke-4 dipakai untuk fallback duplikat (AQ1, AQ2, dst).
    ''' </summary>
    Public Function GenerateKodeUnik(singkatan As String) As String
        ' Pastikan singkatan tidak melebihi batas maksimal kode merk
        Dim base As String = If(singkatan.Length > 4, singkatan.Substring(0, 4), singkatan)
        Dim kode As String = base
        Dim counter As Integer = 1

        Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_merk WHERE kode = @Kode", conn)
            cmd.Parameters.AddWithValue("@Kode", kode)
            Dim exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0

            ' Jika sudah ada, coba variasi dengan angka — tetap max 4 karakter
            ' Contoh: AQU → AQ1, AQ2, ... AQ9 (2 huruf + 1 angka)
            While exists AndAlso counter < 100
                kode = base.Substring(0, Math.Min(3, base.Length)) & counter.ToString()
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@Kode", kode)
                exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0
                counter += 1
            End While
        End Using

        Return kode
    End Function

    Public Sub KodeMerk()
        ' Kode di-generate otomatis saat user Leave dari field Nama
        TxtKode.Text = ""
    End Sub
#End Region

#Region "Simpan"
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNama.Text) Then
            MessageBox.Show("Nama harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim kode As String = StrConv(TxtKode.Text.Trim(), vbUpperCase)
        Dim nama As String = StrConv(TxtNama.Text.Trim(), vbProperCase)
        Dim ket As String = TxtKeterangan.Text.Trim()

        If _isEditMode Then
            UpdateMerk(kode, nama, ket)
        Else
            InsertMerk(kode, nama, ket)
        End If
    End Sub

    Private Sub InsertMerk(kode As String, nama As String, ket As String)
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Cek duplikat kode
            Using cmd As New MySqlCommand("SELECT 1 FROM tbl_merk WHERE kode = @Kode LIMIT 1", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", kode)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        rd.Close()
                        transaction.Rollback()
                        ' Generate kode alternatif
                        Dim kodeBaru As String = GenerateKodeUnik(GenerateSingkatan(nama))
                        TxtKode.Text = kodeBaru
                        MessageBox.Show($"Kode '{kode}' sudah dipakai. Kode baru '{kodeBaru}' telah digenerate. Silakan simpan ulang.", "Kode Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TxtKode.Focus()
                        Exit Sub
                    End If
                End Using
            End Using

            ' Cek nama duplikat
            Using cmd As New MySqlCommand("SELECT 1 FROM tbl_merk WHERE nama = @Nama LIMIT 1", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", nama)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        rd.Close()
                        transaction.Rollback()
                        MessageBox.Show("Nama merk sudah ada, silakan ganti dengan yang lain.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TxtNama.Focus()
                        Exit Sub
                    End If
                End Using
            End Using

            Using cmd As New MySqlCommand("INSERT INTO tbl_merk (kode, nama, keterangan) VALUES (@Kode, @Nama, @Ket)", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", kode)
                cmd.Parameters.AddWithValue("@Nama", nama)
                cmd.Parameters.AddWithValue("@Ket", ket)
                cmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_merk", kode, "INSERT", ModuleVariabel.NamaUser)
            Kondisiawal()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateMerk(kode As String, nama As String, ket As String)
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmd As New MySqlCommand("UPDATE tbl_merk SET nama = @Nama, keterangan = @Ket WHERE kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", nama)
                cmd.Parameters.AddWithValue("@Ket", ket)
                cmd.Parameters.AddWithValue("@Kode", kode)
                cmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_merk", kode, "UPDATE", ModuleVariabel.NamaUser)
            Kondisiawal()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Hapus"
    Private Sub HapusMerk(kode As String, nama As String)
        If MessageBox.Show($"Hapus merk '{nama}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_merk WHERE kode = @Kode", conn, transaction)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                transaction.Commit()
                Kondisiawal()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
#End Region

#Region "Tombol & Keyboard"
    Private Sub BtnBaru_Click(sender As Object, e As EventArgs) Handles BtnBaru.Click
        Kondisiawal()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub TambahMerk_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BtnSimpan.PerformClick()
            Case Keys.F4 : BtnBaru.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub

    ''' <summary>
    ''' Generate kode otomatis saat user meninggalkan field nama
    ''' </summary>
    Private Sub TxtNama_Leave(sender As Object, e As EventArgs) Handles TxtNama.Leave
        If Not _isEditMode Then
            Dim singkatan As String = GenerateSingkatan(TxtNama.Text)
            If singkatan.Length >= 2 Then
                TxtKode.Text = GenerateKodeUnik(singkatan)
            End If
        End If
    End Sub

    ''' <summary>
    ''' CB Manual: jika dicentang, Kode bisa diedit manual
    ''' </summary>
    Private Sub CbManual_CheckedChanged(sender As Object, e As EventArgs) Handles CBManual.CheckedChanged
        If CBManual.Checked Then
            TxtKode.ReadOnly = False
            TxtKode.Focus()
        Else
            TxtKode.ReadOnly = True
            If Not _isEditMode Then
                TxtKode.Text = ""
            End If
        End If
    End Sub
#End Region

#Region "Import Data Default dari SQL"
    ''' <summary>
    ''' Parse file SQL dan ekstrak data merk
    ''' Format: ('KODE','Nama','Keterangan'),
    ''' </summary>
    Private Function ParseMerkFromSql(sqlPath As String) As List(Of Tuple(Of String, String, String))
        Dim result As New List(Of Tuple(Of String, String, String))

        If Not System.IO.File.Exists(sqlPath) Then
            Return result
        End If

        Dim sqlContent As String = System.IO.File.ReadAllText(sqlPath)

        ' Regex untuk match pattern: ('KODE','Nama','Keterangan')
        Dim pattern As String = "\('([^']+)','([^']+)'\s*,\s*'([^']+)'\)"
        Dim matches As System.Text.RegularExpressions.MatchCollection =
            System.Text.RegularExpressions.Regex.Matches(sqlContent, pattern)

        For Each match As System.Text.RegularExpressions.Match In matches
            If match.Groups.Count >= 4 Then
                Dim kode As String = match.Groups(1).Value.Trim()
                Dim nama As String = match.Groups(2).Value.Trim()
                Dim ket As String = match.Groups(3).Value.Trim()
                result.Add(Tuple.Create(kode, nama, ket))
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' Cek apakah nama hampir sama (contains atau similarity >= 80%)
    ''' </summary>
    Private Function IsNamaHampirSama(namaDb As String, namaDefault As String) As Boolean
        If String.IsNullOrWhiteSpace(namaDb) OrElse String.IsNullOrWhiteSpace(namaDefault) Then
            Return False
        End If

        Dim db As String = namaDb.Trim().ToUpper()
        Dim def As String = namaDefault.Trim().ToUpper()

        ' Exact match
        If db = def Then Return True

        ' Contains (salah satu mengandung yang lain)
        If db.Contains(def) OrElse def.Contains(db) Then Return True

        ' Similarity >= 80%
        If CalculateSimilarity(db, def) >= 0.8 Then Return True

        Return False
    End Function

    Private Function CalculateSimilarity(s1 As String, s2 As String) As Double
        Dim longer As String = If(s1.Length > s2.Length, s1, s2)
        Dim shorter As String = If(s1.Length > s2.Length, s2, s1)
        If longer.Length = 0 Then Return 1.0
        Return (longer.Length - LevenshteinDistance(longer, shorter)) / CDbl(longer.Length)
    End Function

    Private Function LevenshteinDistance(s As String, t As String) As Integer
        Dim n As Integer = s.Length
        Dim m As Integer = t.Length
        Dim d(n + 1, m + 1) As Integer
        If n = 0 Then Return m
        If m = 0 Then Return n
        For i As Integer = 0 To n : d(i, 0) = i : Next
        For j As Integer = 0 To m : d(0, j) = j : Next
        For i As Integer = 1 To n
            For j As Integer = 1 To m
                Dim cost As Integer = If(s(i - 1) = t(j - 1), 0, 1)
                d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1), d(i - 1, j - 1) + cost)
            Next
        Next
        Return d(n, m)
    End Function

    ''' <summary>
    ''' Tombol Load Data Default - pilih file SQL dan import
    ''' </summary>
    Private Sub BtnLoadDataDefault_Click(sender As Object, e As EventArgs) Handles BtnLoadDataDefault.Click
        ' 1. Tampilkan Dialog Pemilihan File
        Using ofd As New OpenFileDialog()
            ofd.Title = "Pilih File SQL Data Merk Default"
            ofd.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            ofd.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, "database_Default_Master")

            If Not System.IO.Directory.Exists(ofd.InitialDirectory) Then
                ofd.InitialDirectory = Application.StartupPath
            Else
                ' Otomatis arahkan ke file default jika ada
                Dim defaultFile As String = System.IO.Path.Combine(ofd.InitialDirectory, "03_merk_default.sql")
                If System.IO.File.Exists(defaultFile) Then
                    ofd.FileName = "03_merk_default.sql"
                End If
            End If

            If ofd.ShowDialog() <> DialogResult.OK Then Exit Sub

            Dim sqlPath As String = ofd.FileName

            ' 2. Konfirmasi sebelum eksekusi
            Dim confirm = MessageBox.Show(
                $"Apakah Anda yakin ingin memuat data merk dari file:{vbCrLf}{sqlPath}?",
                "Konfirmasi Load Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

            If confirm <> DialogResult.Yes Then Exit Sub

            ' 3. Parse data dari SQL
            Dim dataDefault = ParseMerkFromSql(sqlPath)
            If dataDefault.Count = 0 Then
                MessageBox.Show("Tidak ada data merk yang valid ditemukan dalam file tersebut.",
                               "Data Kosong", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 4. Load existing data
            Dim existingData As New List(Of Tuple(Of String, String))
            Try
                Using cmd As New MySqlCommand("SELECT kode, nama FROM tbl_merk", conn)
                    Using rd = cmd.ExecuteReader()
                        While rd.Read()
                            existingData.Add(Tuple.Create(rd("kode").ToString(), rd("nama").ToString()))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Gagal membaca data existing: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try

            ' 5. Proses Import
            Dim total As Integer = dataDefault.Count
            Dim inserted As Integer = 0
            Dim skipped As Integer = 0
            Dim ditolak As Integer = 0

            Me.Cursor = Cursors.WaitCursor
            For Each item In dataDefault
                Dim kodeDefault As String = item.Item1
                Dim namaDefault As String = item.Item2
                Dim ketDefault As String = item.Item3

                ' Cek duplikat KODE
                If existingData.Any(Function(x) x.Item1.Equals(kodeDefault, StringComparison.OrdinalIgnoreCase)) Then
                    skipped += 1
                    Continue For
                End If

                ' Cek duplikat NAMA (Exact)
                If existingData.Any(Function(x) x.Item2.Trim().Equals(namaDefault.Trim(), StringComparison.OrdinalIgnoreCase)) Then
                    skipped += 1
                    Continue For
                End If

                ' Cek NAMA hampir sama
                Dim namaMirip = existingData.FirstOrDefault(Function(x) IsNamaHampirSama(x.Item2, namaDefault))
                If namaMirip IsNot Nothing Then
                    Dim result = MessageBox.Show(
                        $"Merk '{namaDefault}' mirip dengan '{namaMirip.Item2}' yang sudah ada.{vbCrLf}{vbCrLf}" &
                        "Tetap tambahkan?",
                        "Konfirmasi Kemiripan Data",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question)

                    If result = DialogResult.Cancel Then
                        Exit For
                    ElseIf result = DialogResult.No Then
                        ditolak += 1
                        Continue For
                    End If
                End If

                ' Insert
                Try
                    Using cmd As New MySqlCommand("INSERT INTO tbl_merk (kode, nama, keterangan) VALUES (@Kode, @Nama, @Ket)", conn)
                        cmd.Parameters.AddWithValue("@Kode", StrConv(kodeDefault, vbUpperCase))
                        cmd.Parameters.AddWithValue("@Nama", StrConv(namaDefault, vbProperCase))
                        cmd.Parameters.AddWithValue("@Ket", ketDefault)
                        cmd.ExecuteNonQuery()
                    End Using
                    inserted += 1
                    existingData.Add(Tuple.Create(kodeDefault, namaDefault))
                Catch ex As Exception
                    MessageBox.Show($"Gagal menyimpan '{namaDefault}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
            Me.Cursor = Cursors.Default

            ' 6. Refresh & Summary
            TampilMerk()
            MessageBox.Show(
                $"Proses selesai.{vbCrLf}{vbCrLf}" &
                $"Total data di file: {total}{vbCrLf}" &
                $"Berhasil ditambah: {inserted}{vbCrLf}" &
                $"Dilewati (duplikat): {skipped}{vbCrLf}" &
                $"Ditolak user: {ditolak}",
                "Import Selesai",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)
        End Using
    End Sub
#End Region

End Class

