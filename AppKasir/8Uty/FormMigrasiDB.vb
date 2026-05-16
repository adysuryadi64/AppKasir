Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Public Class FormMigrasiDB

    Private Sub FormMigrasiDB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ListBoxHasil.Items.Clear()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    ' ── Pilih file SQL ──────────────────────────────────────────────
    Private Sub BtnCari_Click(sender As Object, e As EventArgs) Handles BtnCari.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            ofd.Title = "Pilih File Migrasi (.sql)"
            If ofd.ShowDialog() = DialogResult.OK Then
                TxtFilePath.Text = ofd.FileName
            End If
        End Using
    End Sub

    ' ── Proses migrasi ─────────────────────────────────────────────
    Private Sub BtnProses_Click(sender As Object, e As EventArgs) Handles BtnProses.Click
        If String.IsNullOrWhiteSpace(TxtFilePath.Text) OrElse Not File.Exists(TxtFilePath.Text) Then
            MessageBox.Show("Pilih file SQL terlebih dahulu.", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ListBoxHasil.Items.Clear()
        BtnProses.Enabled = False
        Cursor = Cursors.WaitCursor

        Try
            Dim migrasiConn As New MySqlConnection(_connectionString)
            migrasiConn.Open()

            ' SplitStatements membaca file langsung — tidak load ke memori
            Dim statements As List(Of String) = SplitStatements(TxtFilePath.Text)

            Log($"File: {Path.GetFileName(TxtFilePath.Text)}")
            Log($"Total statement ditemukan: {statements.Count}")
            Log("─────────────────────────────────────────")

            Dim berhasil As Integer = 0
            Dim dilewati As Integer = 0
            Dim gagal As Integer = 0

            For Each stmt As String In statements
                Dim trimmed As String = stmt.Trim()
                If String.IsNullOrWhiteSpace(trimmed) Then Continue For

                If IsSkippable(trimmed) Then
                    dilewati += 1
                    Continue For
                End If

                Try
                    Using cmd As New MySqlCommand(trimmed, migrasiConn)
                        Dim preview As String = GetStatementPreview(trimmed)
                        If trimmed.StartsWith("ALTER TABLE", StringComparison.OrdinalIgnoreCase) OrElse
                           trimmed.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase) OrElse
                           trimmed.StartsWith("CALL ", StringComparison.OrdinalIgnoreCase) Then
                            Log($"⏳ {preview}")
                            Application.DoEvents()
                        End If
                        cmd.ExecuteNonQuery()
                    End Using

                    Dim preview2 As String = GetStatementPreview(trimmed)
                    If ListBoxHasil.Items.Count > 0 AndAlso
                       ListBoxHasil.Items(ListBoxHasil.Items.Count - 1).ToString().StartsWith("⏳") Then
                        ListBoxHasil.Items(ListBoxHasil.Items.Count - 1) = $"✓ {preview2}"
                    Else
                        Log($"✓ {preview2}")
                    End If
                    berhasil += 1

                Catch ex As MySqlException
                    If ListBoxHasil.Items.Count > 0 AndAlso
                       ListBoxHasil.Items(ListBoxHasil.Items.Count - 1).ToString().StartsWith("⏳") Then
                        ListBoxHasil.Items.RemoveAt(ListBoxHasil.Items.Count - 1)
                    End If
                    If ex.Number = 1060 OrElse ex.Number = 1061 OrElse ex.Number = 1091 Then
                        Log($"⊘ Sudah ada, dilewati: {GetStatementPreview(trimmed)}")
                        dilewati += 1
                    Else
                        Log($"✗ ERROR [{ex.Number}]: {ex.Message}")
                        Log($"  → {trimmed}")
                        gagal += 1
                    End If
                End Try

                Application.DoEvents()
            Next

            Log("─────────────────────────────────────────")
            Log($"Selesai — Berhasil: {berhasil} | Dilewati: {dilewati} | Gagal: {gagal}")

            If gagal = 0 Then
                MessageBox.Show($"Migrasi selesai.{vbCrLf}Berhasil: {berhasil} | Dilewati: {dilewati}",
                                "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show($"Migrasi selesai dengan {gagal} error.{vbCrLf}Periksa log untuk detail.",
                                "Selesai dengan Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            Log($"✗ FATAL: {ex.Message}")
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            BtnProses.Enabled = True
            Cursor = Cursors.Default
        End Try
    End Sub

    ' ── Pecah file SQL menjadi statement individual ─────────────────
    ' Membaca file per baris via StreamReader — tidak load seluruh file ke memori
    ' Menangani DELIMITER $$ / DELIMITER ; untuk stored procedure
    Private Function SplitStatements(filePath As String) As List(Of String)
        Dim result As New List(Of String)()
        Dim current As New StringBuilder()
        Dim delimiter As String = ";"

        Using sr As New StreamReader(filePath, Encoding.UTF8)
            Dim line As String = sr.ReadLine()
            Do While line IsNot Nothing
                Dim trimLine As String = line.Trim()

                ' Tangani DELIMITER
                If trimLine.StartsWith("DELIMITER", StringComparison.OrdinalIgnoreCase) Then
                    Dim prev As String = current.ToString().Trim()
                    If prev.Length > 0 Then result.Add(prev)
                    current.Clear()
                    Dim parts As String() = trimLine.Split(New Char() {" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
                    If parts.Length >= 2 Then delimiter = parts(1).Trim()
                    line = sr.ReadLine()
                    Continue Do
                End If

                ' Lewati komentar murni — selalu, di manapun posisinya
                If trimLine.StartsWith("--") OrElse trimLine.StartsWith("#") Then
                    line = sr.ReadLine()
                    Continue Do
                End If

                current.Append(line & vbLf)

                ' Cek apakah baris ini mengakhiri statement
                If trimLine.EndsWith(delimiter, StringComparison.OrdinalIgnoreCase) Then
                    Dim stmt As String = current.ToString().Trim()
                    If stmt.EndsWith(delimiter) Then
                        stmt = stmt.Substring(0, stmt.Length - delimiter.Length).Trim()
                    End If
                    If stmt.Length > 0 Then result.Add(stmt)
                    current.Clear()
                End If

                line = sr.ReadLine()
            Loop
        End Using

        ' Sisa terakhir
        Dim last As String = current.ToString().Trim()
        If last.Length > 0 Then result.Add(last)

        Return result
    End Function

    ' ── Cek apakah statement perlu dilewati ────────────────────────
    Private Function IsSkippable(stmt As String) As Boolean
        Dim upper As String = stmt.ToUpper().TrimStart()
        Return upper.StartsWith("DELIMITER") OrElse
               upper.StartsWith("USE ") OrElse
               upper.StartsWith("SET SQL_MODE") OrElse
               upper.StartsWith("SET TIME_ZONE") OrElse
               upper.StartsWith("START TRANSACTION") OrElse
               upper.StartsWith("COMMIT") OrElse
               upper.StartsWith("/*!") OrElse
               upper.StartsWith("--") OrElse
               upper.StartsWith("#")
    End Function

    ' ── Preview singkat statement untuk log ────────────────────────
    Private Function GetStatementPreview(stmt As String) As String
        Dim upper As String = stmt.ToUpper().TrimStart()
        Dim first As String = stmt.TrimStart()

        If upper.StartsWith("ALTER TABLE") Then
            Dim m As Match = Regex.Match(first, "ALTER\s+TABLE\s+`?(\w+)`?\s+(.*)",
                                         RegexOptions.IgnoreCase Or RegexOptions.Singleline)
            If m.Success Then
                Dim action As String = m.Groups(2).Value.Trim()

                Return $"ALTER TABLE {m.Groups(1).Value} — {action}"
            End If
        ElseIf upper.StartsWith("CREATE PROCEDURE") Then
            Return "CREATE PROCEDURE"
        ElseIf upper.StartsWith("DROP PROCEDURE") Then
            Return "DROP PROCEDURE"
        ElseIf upper.StartsWith("CALL ") Then
            Return first
        ElseIf upper.StartsWith("SELECT") Then
            Return "SELECT (verifikasi)"
        End If

        Return first
    End Function

    ' ── Jalankan semua file migrasi dari folder Database\ ──────────
    Private Sub BtnJalankanSemua_Click(sender As Object, e As EventArgs) Handles BtnJalankanSemua.Click
        Dim folderDb As String = Path.Combine(Application.StartupPath, "Database")

        If Not Directory.Exists(folderDb) Then
            Dim dir As String = Application.StartupPath
            For i As Integer = 1 To 3
                dir = Path.GetDirectoryName(dir)
                Dim candidate As String = Path.Combine(dir, "Database")
                If Directory.Exists(candidate) Then
                    folderDb = candidate
                    Exit For
                End If
            Next
        End If

        If Not Directory.Exists(folderDb) Then
            MessageBox.Show($"Folder Database tidak ditemukan:{vbCrLf}{folderDb}",
                            "Folder Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim files As String() = Directory.GetFiles(folderDb, "*.sql") _
            .Where(Function(f) Char.IsDigit(Path.GetFileName(f)(0))) _
            .OrderBy(Function(f) Path.GetFileName(f)) _
            .ToArray()

        If files.Length = 0 Then
            MessageBox.Show("Tidak ada file migrasi (01_*.sql dst) di folder Database.",
                            "Tidak Ada File", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim konfirmasi As DialogResult = MessageBox.Show(
            $"Akan menjalankan {files.Length} file migrasi secara berurutan:{vbCrLf}" &
            String.Join(vbCrLf, files.Select(Function(f) "  • " & Path.GetFileName(f))) &
            vbCrLf & vbCrLf & "Lanjutkan?",
            "Konfirmasi Jalankan Semua Migrasi",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If konfirmasi <> DialogResult.Yes Then Return

        ListBoxHasil.Items.Clear()
        BtnJalankanSemua.Enabled = False
        BtnProses.Enabled = False
        Cursor = Cursors.WaitCursor

        Dim totalBerhasil As Integer = 0
        Dim totalDilewati As Integer = 0
        Dim totalGagal As Integer = 0
        Dim migrasiConn As MySqlConnection = Nothing

        Try
            migrasiConn = New MySqlConnection(_connectionString)
            migrasiConn.Open()

            For Each filePath As String In files
                Dim namaFile As String = Path.GetFileName(filePath)
                Log("")
                Log("══════════════════════════════════════════")
                Log($"▶ {namaFile}")
                Log("══════════════════════════════════════════")
                Application.DoEvents()

                Dim statements As List(Of String) = SplitStatements(filePath)
                Log($"  {statements.Count} statement ditemukan")

                Dim berhasil As Integer = 0
                Dim dilewati As Integer = 0
                Dim gagal As Integer = 0
                Dim adaFatal As Boolean = False

                For Each stmt As String In statements
                    Dim trimmed As String = stmt.Trim()
                    If String.IsNullOrWhiteSpace(trimmed) Then Continue For

                    If IsSkippable(trimmed) Then
                        dilewati += 1
                        Continue For
                    End If

                    Try
                        Using cmd As New MySqlCommand(trimmed, migrasiConn)
                            Dim preview As String = GetStatementPreview(trimmed)
                            If trimmed.StartsWith("ALTER TABLE", StringComparison.OrdinalIgnoreCase) OrElse
                               trimmed.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase) OrElse
                               trimmed.StartsWith("CALL ", StringComparison.OrdinalIgnoreCase) Then
                                Log($"  ⏳ {preview}")
                                Application.DoEvents()
                            End If
                            cmd.ExecuteNonQuery()
                        End Using
                        Dim preview2 As String = GetStatementPreview(trimmed)
                        If ListBoxHasil.Items.Count > 0 AndAlso
                           ListBoxHasil.Items(ListBoxHasil.Items.Count - 1).ToString().TrimStart().StartsWith("⏳") Then
                            ListBoxHasil.Items(ListBoxHasil.Items.Count - 1) = $"  ✓ {preview2}"
                        Else
                            Log($"  ✓ {preview2}")
                        End If
                        berhasil += 1

                    Catch ex As MySqlException
                        If ListBoxHasil.Items.Count > 0 AndAlso
                           ListBoxHasil.Items(ListBoxHasil.Items.Count - 1).ToString().TrimStart().StartsWith("⏳") Then
                            ListBoxHasil.Items.RemoveAt(ListBoxHasil.Items.Count - 1)
                        End If
                        If ex.Number = 1060 OrElse ex.Number = 1061 OrElse ex.Number = 1091 Then
                            Log($"  ⊘ Sudah ada: {GetStatementPreview(trimmed)}")
                            dilewati += 1
                        Else
                            Log($"  ✗ ERROR [{ex.Number}]: {ex.Message}")
                            gagal += 1
                            adaFatal = True
                        End If
                    End Try

                    Application.DoEvents()
                Next

                Log($"  → Berhasil: {berhasil} | Dilewati: {dilewati} | Gagal: {gagal}")
                totalBerhasil += berhasil
                totalDilewati += dilewati
                totalGagal += gagal

                If adaFatal AndAlso filePath <> files.Last() Then
                    Dim lanjut As DialogResult = MessageBox.Show(
                        $"{namaFile} selesai dengan {gagal} error.{vbCrLf}Lanjut ke file berikutnya?",
                        "Ada Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    If lanjut = DialogResult.No Then
                        Log("  ⛔ Dihentikan oleh user.")
                        Exit For
                    End If
                End If

                System.Threading.Thread.Sleep(200)
                Application.DoEvents()
            Next

            Log("")
            Log("══════════════════════════════════════════")
            Log($"SELESAI — Total: Berhasil {totalBerhasil} | Dilewati {totalDilewati} | Gagal {totalGagal}")
            Log("══════════════════════════════════════════")

            Dim icon As MessageBoxIcon = If(totalGagal = 0, MessageBoxIcon.Information, MessageBoxIcon.Warning)
            Dim judul As String = If(totalGagal = 0, "Semua Migrasi Selesai", "Selesai dengan Error")
            MessageBox.Show(
                $"Semua file migrasi telah diproses.{vbCrLf}" &
                $"Berhasil: {totalBerhasil} | Dilewati: {totalDilewati} | Gagal: {totalGagal}",
                judul, MessageBoxButtons.OK, icon)

        Catch ex As Exception
            Log($"✗ FATAL: {ex.Message}")
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            migrasiConn?.Close()
            migrasiConn?.Dispose()
            BtnJalankanSemua.Enabled = True
            BtnProses.Enabled = True
            Cursor = Cursors.Default
        End Try
    End Sub

    ' ── Helper log ke ListBox ───────────────────────────────────────
    Private Sub Log(message As String)
        ListBoxHasil.Items.Add(message)
        ListBoxHasil.TopIndex = ListBoxHasil.Items.Count - 1
    End Sub

End Class
