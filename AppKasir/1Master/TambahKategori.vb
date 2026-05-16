Public Class TambahKategori

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit TambahKategori ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("KAT:" & kode, "HAPUS"/"EDIT",
    '       "Master kategori", snapshotJson, "[KRITIS] Hapus/Edit data kategori", trans)
    ' Snapshot: baca data lama dari tbl_kategori sebelum DELETE/UPDATE
    ' ─────────────────────────────────────────────────────────────────────────

    Private _isEditMode As Boolean = False
    Private _isLoading As Boolean = False

#Region "Form Load"
    Private Sub TambahKategori_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub
#End Region

#Region "DataGridView"
    Public Sub Tampilkategori()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT kode, nama, jenis FROM tbl_kategori ORDER BY nama", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        ' Reset DGV sepenuhnya
        DgvData.DataSource = Nothing
        DgvData.Rows.Clear()
        DgvData.Columns.Clear()

        ' Kolom data manual
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 60, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama", .FillWeight = 140, .ReadOnly = True})
        DgvData.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColJenis", .HeaderText = "Jenis", .FillWeight = 80, .ReadOnly = True})

        ' Kolom tombol Edit
        Dim colEdit As New DataGridViewButtonColumn() With {
            .Name = "ColEdit", .HeaderText = "Edit",
            .Text = "✎ Edit", .UseColumnTextForButtonValue = True,
            .Width = 70, .FlatStyle = FlatStyle.Flat,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        }
        ' Font diatur oleh tema - Calibri 9.75 Bold
        DgvData.Columns.Add(colEdit)

        ' Kolom tombol Hapus
        Dim colHapus As New DataGridViewButtonColumn() With {
            .Name = "ColHapus", .HeaderText = "Hapus",
            .Text = "✖ Hapus", .UseColumnTextForButtonValue = True,
            .Width = 75, .FlatStyle = FlatStyle.Flat,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        }
        ' Font diatur oleh tema - Calibri 9.75 Bold
        DgvData.Columns.Add(colHapus)

        ' Isi baris data
        For Each row As DataRow In dt.Rows
            Dim rowIdx As Integer = DgvData.Rows.Add(row("kode"), row("nama"), row("jenis"))
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
        Dim jenis As String = If(row.Cells("ColJenis").Value IsNot Nothing, row.Cells("ColJenis").Value.ToString(), "")

        Select Case DgvData.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                LblHeader.Text = "EDIT KATEGORI"
                BtnSimpan.Text = "UPDATE (F2)"
                ' Mode Edit: Kode tidak boleh diubah, CB Manual disembunyikan
                CBManual.Visible = False
                TxtKode.ReadOnly = True
                TxtKode.Text = kode
                TxtNama.Text = nama
                TxtJenis.Text = jenis
                TxtNama.Focus()

            Case "ColHapus"
                HapusKategori(kode, nama)
        End Select
    End Sub


#End Region

#Region "Kondisi Awal"
    Public Sub Kondisiawal()
        _isEditMode = False
        LblHeader.Text = "TAMBAH KATEGORI"
        BtnSimpan.Text = "SIMPAN (F2)"
        ' Mode Tambah: CB Manual visible, unchecked default
        ' Kode readonly, generate otomatis saat Leave dari field Nama
        CBManual.Visible = True
        CBManual.Enabled = True
        CBManual.Checked = False
        TxtKode.ReadOnly = True
        TxtKode.Text = ""
        TxtNama.Text = ""
        TxtJenis.Text = "Barang"
        Tampilkategori()
        TxtNama.Select()
    End Sub
#End Region

#Region "Auto Kode"
    ''' <summary>
    ''' Generate singkatan dari nama kategori
    ''' - 1 kata: 3 huruf pertama (atau semua jika kurang dari 3 huruf)
    ''' - 2 kata: 1 huruf kata1 + 2 huruf kata2 (contoh: Air Minum jadi AMN)
    ''' - 3+ kata: 1 huruf dari 3 kata pertama (contoh: Pasta Gigi Anak jadi PGA)
    ''' </summary>
    Public Function GenerateSingkatan(nama As String) As String
        If String.IsNullOrWhiteSpace(nama) Then Return ""

        ' VB.NET: gunakan Split dengan Char array dan StringSplitOptions
        Dim words() As String = nama.Trim().Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        Dim result As String = ""

        If words.Length = 1 Then
            ' 1 kata: ambil 3 huruf pertama (atau semua jika kurang dari 3)
            Dim kata = words(0)
            result = kata.Substring(0, Math.Min(3, kata.Length))
        ElseIf words.Length = 2 Then
            ' 2 kata: 1 huruf kata 1 + 2 huruf kata 2 (contoh: Air Minum → AMN)
            Dim kata1 = words(0)
            Dim kata2 = words(1)
            result = kata1.Substring(0, 1) & kata2.Substring(0, Math.Min(2, kata2.Length))
        Else
            ' 3+ kata: 1 huruf dari 3 kata pertama (contoh: Minyak Goreng Sawit → MGS)
            For i As Integer = 0 To Math.Min(2, words.Length - 1)
                result &= words(i).Substring(0, 1)
            Next
        End If

        Return result.ToUpper()
    End Function

    ''' <summary>
    ''' Cek apakah kode sudah ada di database, jika ya generate alternatif.
    ''' Kode kategori dibatasi maksimal 4 karakter sesuai VARCHAR(4) di database.
    ''' Normal generate 3 huruf, karakter ke-4 dipakai untuk fallback duplikat (AM1, AM2, dst).
    ''' </summary>
    Public Function GenerateKodeUnik(singkatan As String) As String
        ' Pastikan singkatan tidak melebihi batas maksimal kode kategori
        Dim base As String = If(singkatan.Length > 4, singkatan.Substring(0, 4), singkatan)
        Dim kode As String = base
        Dim counter As Integer = 1

        Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_kategori WHERE kode = @Kode", conn)
            cmd.Parameters.AddWithValue("@Kode", kode)
            Dim exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0

            ' Jika sudah ada, coba variasi dengan angka — tetap max 4 karakter
            ' Contoh: AMN → AM1, AM2, ... AM9 (2 huruf + 1 angka)
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

    Public Sub KodeKategori()
        ' Kode akan di-generate otomatis saat user Leave dari field Nama
        ' Di sini hanya set kosong, biarkan Leave event yang handle
        TxtKode.Text = ""
    End Sub
#End Region

#Region "Simpan"
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNama.Text) OrElse
           String.IsNullOrWhiteSpace(TxtJenis.Text) Then
            MessageBox.Show("Nama dan Jenis harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim kode As String = StrConv(TxtKode.Text.Trim(), vbUpperCase)
        Dim nama As String = StrConv(TxtNama.Text.Trim(), vbProperCase)
        Dim jenis As String = StrConv(TxtJenis.Text.Trim(), vbProperCase)

        If _isEditMode Then
            UpdateKategori(kode, nama, jenis)
        Else
            InsertKategori(kode, nama, jenis)
        End If
    End Sub

    Private Sub InsertKategori(kode As String, nama As String, jenis As String)
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Cek duplikat kode
            Using cmd As New MySqlCommand("SELECT 1 FROM tbl_kategori WHERE kode = @Kode LIMIT 1", conn, transaction)
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

            ' Cek nama duplikat — tampilkan pesan, tidak generate ulang
            Using cmd As New MySqlCommand("SELECT 1 FROM tbl_kategori WHERE nama = @Nama LIMIT 1", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", nama)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        rd.Close()
                        transaction.Rollback()
                        MessageBox.Show("Nama kategori sudah ada, silakan ganti dengan yang lain.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TxtNama.Focus()
                        Exit Sub
                    End If
                End Using
            End Using

            Using cmd As New MySqlCommand("INSERT INTO tbl_kategori (kode, nama, jenis) VALUES (@Kode, @Nama, @Jenis)", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", kode)
                cmd.Parameters.AddWithValue("@Nama", nama)
                cmd.Parameters.AddWithValue("@Jenis", jenis)
                cmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_kategori", kode, "INSERT", ModuleVariabel.NamaUser)
            Kondisiawal()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateKategori(kode As String, nama As String, jenis As String)
        ' Mode Edit: kode tidak boleh diubah (TxtKode.ReadOnly = True)
        ' Hanya update nama dan jenis
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmd As New MySqlCommand("UPDATE tbl_kategori SET nama = @Nama, jenis = @Jenis WHERE kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", nama)
                cmd.Parameters.AddWithValue("@Jenis", jenis)
                cmd.Parameters.AddWithValue("@Kode", kode)
                cmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_kategori", kode, "UPDATE", ModuleVariabel.NamaUser)
            Kondisiawal()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Hapus"
    Private Sub HapusKategori(kode As String, nama As String)
        If MessageBox.Show($"Hapus kategori '{nama}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_kategori WHERE kode = @Kode", conn, transaction)
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

    Private Sub TambahKategori_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BtnSimpan.PerformClick()
            Case Keys.F4 : BtnBaru.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub

    ''' <summary>
    ''' Generate kode otomatis saat user meninggalkan field nama (setelah selesai edit)
    ''' Selalu generate kode baru di mode tambah (overwrite jika perlu)
    ''' Mode Edit: kode tidak diubah karena readonly
    ''' </summary>
    Private Sub TxtNama_Leave(sender As Object, e As EventArgs) Handles TxtNama.Leave
        ' Mode Tambah: selalu generate kode saat Leave dari field Nama
        If Not _isEditMode Then
            Dim singkatan As String = GenerateSingkatan(TxtNama.Text)
            If singkatan.Length >= 2 Then
                TxtKode.Text = GenerateKodeUnik(singkatan)
            End If
        End If
    End Sub

    ''' <summary>
    ''' CB Manual: jika dicentang, Kode bisa diedit manual
    ''' Jika tidak dicentang, Kode readonly dan auto-generate
    ''' </summary>
    Private Sub CbManual_CheckedChanged(sender As Object, e As EventArgs) Handles CBManual.CheckedChanged
        If CBManual.Checked Then
            ' Mode manual: enable edit
            TxtKode.ReadOnly = False
            TxtKode.Focus()
        Else
            ' Mode otomatis: readonly, clear kode (nanti di-generate saat Leave)
            TxtKode.ReadOnly = True
            If Not _isEditMode Then
                TxtKode.Text = ""  ' Clear agar nanti generate otomatis
            End If
        End If
    End Sub
#End Region

#Region "Import Data Default dari SQL"
    ''' <summary>
    ''' Parse file SQL dan ekstrak data kategori
    ''' Format: ('KODE','Nama','Jenis'),
    ''' </summary>
    Private Function ParseKategoriFromSql(sqlPath As String) As List(Of Tuple(Of String, String, String))
        Dim result As New List(Of Tuple(Of String, String, String))

        If Not System.IO.File.Exists(sqlPath) Then
            Return result
        End If

        Dim sqlContent As String = System.IO.File.ReadAllText(sqlPath)

        ' Regex untuk match pattern: ('KODE','Nama','Jenis')
        ' Contoh: ('MIN','Minuman',  'Barang Dagangan'),
        Dim pattern As String = "\('([^']+)','([^']+)'\s*,\s*'([^']+)'\)"
        Dim matches As System.Text.RegularExpressions.MatchCollection =
            System.Text.RegularExpressions.Regex.Matches(sqlContent, pattern)

        For Each match As System.Text.RegularExpressions.Match In matches
            If match.Groups.Count >= 4 Then
                Dim kode As String = match.Groups(1).Value.Trim()
                Dim nama As String = match.Groups(2).Value.Trim()
                Dim jenis As String = match.Groups(3).Value.Trim()
                result.Add(Tuple.Create(kode, nama, jenis))
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

    ''' <summary>
    ''' Calculate similarity ratio (0.0 - 1.0)
    ''' </summary>
    Private Function CalculateSimilarity(s1 As String, s2 As String) As Double
        Dim longer As String = If(s1.Length > s2.Length, s1, s2)
        Dim shorter As String = If(s1.Length > s2.Length, s2, s1)

        If longer.Length = 0 Then Return 1.0

        Dim distance As Integer = LevenshteinDistance(longer, shorter)
        Return (longer.Length - distance) / CDbl(longer.Length)
    End Function

    ''' <summary>
    ''' Levenshtein distance calculation
    ''' </summary>
    Private Function LevenshteinDistance(s As String, t As String) As Integer
        Dim n As Integer = s.Length
        Dim m As Integer = t.Length
        Dim d(n + 1, m + 1) As Integer

        If n = 0 Then Return m
        If m = 0 Then Return n

        For i As Integer = 0 To n
            d(i, 0) = i
        Next
        For j As Integer = 0 To m
            d(0, j) = j
        Next

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
            ofd.Title = "Pilih File SQL Data Kategori Default"
            ofd.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            ofd.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, "database_Default_Master")

            ' Jika folder default tidak ada, gunakan startup path
            If Not System.IO.Directory.Exists(ofd.InitialDirectory) Then
                ofd.InitialDirectory = Application.StartupPath
            Else
                ' Otomatis arahkan ke file default jika ada
                Dim defaultFile As String = System.IO.Path.Combine(ofd.InitialDirectory, "01_kategori_default.sql")
                If System.IO.File.Exists(defaultFile) Then
                    ofd.FileName = "01_kategori_default.sql"
                End If
            End If

            If ofd.ShowDialog() <> DialogResult.OK Then Exit Sub

            Dim sqlPath As String = ofd.FileName

            ' 2. Konfirmasi sebelum eksekusi
            Dim confirm = MessageBox.Show(
                $"Apakah Anda yakin ingin memuat data kategori dari file:{vbCrLf}{sqlPath}?",
                "Konfirmasi Load Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

            If confirm <> DialogResult.Yes Then Exit Sub

            ' 3. Parse data dari SQL
            Dim dataDefault = ParseKategoriFromSql(sqlPath)
            If dataDefault.Count = 0 Then
                MessageBox.Show("Tidak ada data kategori yang valid ditemukan dalam file tersebut.",
                               "Data Kosong", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 4. Load data existing dari database untuk validasi duplikat
            Dim existingData As New List(Of Tuple(Of String, String)) ' Kode, Nama
            Try
                Using cmd As New MySqlCommand("SELECT kode, nama FROM tbl_kategori", conn)
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

            ' 5. Proses import
            Dim total As Integer = dataDefault.Count
            Dim inserted As Integer = 0
            Dim skipped As Integer = 0
            Dim ditolak As Integer = 0

            Me.Cursor = Cursors.WaitCursor
            For Each item In dataDefault
                Dim kodeDefault As String = item.Item1
                Dim namaDefault As String = item.Item2
                Dim jenisDefault As String = item.Item3

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
                        $"Kategori '{namaDefault}' mirip dengan '{namaMirip.Item2}' yang sudah ada.{vbCrLf}{vbCrLf}" &
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

                ' Insert ke database
                Try
                    Using cmd As New MySqlCommand(
                        "INSERT INTO tbl_kategori (kode, nama, jenis) VALUES (@Kode, @Nama, @Jenis)", conn)
                        cmd.Parameters.AddWithValue("@Kode", StrConv(kodeDefault, vbUpperCase))
                        cmd.Parameters.AddWithValue("@Nama", StrConv(namaDefault, vbProperCase))
                        cmd.Parameters.AddWithValue("@Jenis", jenisDefault)
                        cmd.ExecuteNonQuery()
                    End Using
                    inserted += 1
                    existingData.Add(Tuple.Create(kodeDefault, namaDefault))
                Catch ex As Exception
                    MessageBox.Show($"Gagal menyimpan '{namaDefault}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
            Me.Cursor = Cursors.Default

            ' 6. Selesai
            Tampilkategori()
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

