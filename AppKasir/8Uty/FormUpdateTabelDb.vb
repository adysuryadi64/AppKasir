Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Text.RegularExpressions

Public Class FormUpdateTabelDb
    ' Variabel konfigurasi database
    Private server As String
    Private port As String
    Private username As String
    Private password As String
    Private database As String

    ' Struktur sederhana untuk tabel dan kolom
    Private Class TableInfo
        Public Property TableName As String
        Public Property Columns As New List(Of ColumnInfo)
    End Class

    Private Class ColumnInfo
        Public Property ColumnName As String
        Public Property DataType As String
        Public Property IsNullable As Boolean
        Public Property MaxLength As Integer = -1
    End Class

    Private Sub FormUpdateTabelDb_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadConfiguration()
    End Sub

    Private Sub LoadConfiguration()
        If File.Exists(configFilePath) Then
            Try
                Dim json As String = File.ReadAllText(configFilePath)
                Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)

                server = konfigurasi.Server
                port = konfigurasi.Port
                username = konfigurasi.User
                password = DecryptPassword(konfigurasi.Password)
                database = konfigurasi.Database

            Catch ex As Exception
                MessageBox.Show("Gagal memuat konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub BtnCari_Click(sender As Object, e As EventArgs) Handles BtnCari.Click
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
        openFileDialog.Title = "Pilih File Master Kunci (.sql)"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            TxtFilePath.Text = openFileDialog.FileName
        End If
    End Sub

    Private Sub BtnCek_Click(sender As Object, e As EventArgs) Handles BtnCek.Click
        If String.IsNullOrEmpty(TxtFilePath.Text) Then
            MessageBox.Show("Silakan pilih file master SQL terlebih dahulu.", "Peringatan",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not File.Exists(TxtFilePath.Text) Then
            MessageBox.Show("File SQL tidak ditemukan.", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ListBoxHasil.Items.Clear()
        ListBoxHasil.Items.Add("Sedang memproses...")
        Application.DoEvents()

        Try
            ' Baca file SQL
            Dim sqlContent As String = File.ReadAllText(TxtFilePath.Text)

            ' Debug: Simpan konten asli untuk analisis
            File.WriteAllText("debug_original.sql", sqlContent)

            ' 1. Ekstrak struktur dari file SQL dengan metode sederhana
            ListBoxHasil.Items.Add("Mengekstrak struktur dari file SQL...")
            Application.DoEvents()

            Dim masterTables As List(Of TableInfo) = ExtractTablesSimple(sqlContent)
            ListBoxHasil.Items.Add($"✓ Ditemukan {masterTables.Count} tabel di file master")

            ' Simpan untuk debugging
            SaveTablesToFile(masterTables, "debug_master_tables.txt", "Struktur dari File SQL")

            ' 2. Ambil struktur dari database
            ListBoxHasil.Items.Add("Mengambil struktur dari database...")
            Application.DoEvents()

            Dim dbTables As List(Of TableInfo) = GetDatabaseTables()
            ListBoxHasil.Items.Add($"✓ Ditemukan {dbTables.Count} tabel di database")

            SaveTablesToFile(dbTables, "debug_db_tables.txt", "Struktur dari Database")

            ' 3. Bandingkan
            ListBoxHasil.Items.Add("Membandingkan struktur...")
            Application.DoEvents()

            Dim discrepancies As New List(Of String)
            CompareStructuresSimple(masterTables, dbTables, discrepancies)

            ' 4. Tampilkan hasil
            DisplayResults(discrepancies)

        Catch ex As Exception
            ListBoxHasil.Items.Clear()
            ListBoxHasil.Items.Add("ERROR: " & ex.Message)
            ListBoxHasil.Items.Add("Stack Trace: " & ex.StackTrace)
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' METODE SEDERHANA: Ekstrak tabel dari SQL dengan parsing sederhana
    Private Function ExtractTablesSimple(sqlContent As String) As List(Of TableInfo)
        Dim tables As New List(Of TableInfo)()

        ' Hilangkan komentar
        sqlContent = RemoveComments(sqlContent)

        ' Cari semua CREATE TABLE
        Dim createTableIndex As Integer = 0

        While True
            ' Cari "CREATE TABLE" (case insensitive)
            createTableIndex = sqlContent.IndexOf("CREATE TABLE", createTableIndex, StringComparison.OrdinalIgnoreCase)
            If createTableIndex = -1 Then Exit While

            ' Temukan nama tabel
            Dim startPos As Integer = createTableIndex + "CREATE TABLE".Length

            ' Lewati "IF NOT EXISTS"
            If sqlContent.Substring(startPos).Trim().StartsWith("IF NOT EXISTS", StringComparison.OrdinalIgnoreCase) Then
                startPos += "IF NOT EXISTS".Length
            End If

            ' Cari karakter pertama setelah spasi
            startPos = SkipWhitespace(sqlContent, startPos)

            ' Ekstrak nama tabel (bisa dengan backtick atau tanpa)
            Dim tableName As String = ""
            If startPos < sqlContent.Length Then
                If sqlContent(startPos) = "`"c Then
                    ' Nama dengan backtick
                    Dim endQuote As Integer = sqlContent.IndexOf("`"c, startPos + 1)
                    If endQuote > startPos Then
                        tableName = sqlContent.Substring(startPos + 1, endQuote - startPos - 1)
                        startPos = endQuote + 1
                    End If
                Else
                    ' Nama tanpa backtick
                    Dim endName As Integer = startPos
                    While endName < sqlContent.Length AndAlso
                          (Char.IsLetterOrDigit(sqlContent(endName)) OrElse sqlContent(endName) = "_"c)
                        endName += 1
                    End While
                    tableName = sqlContent.Substring(startPos, endName - startPos)
                    startPos = endName
                End If
            End If

            If String.IsNullOrEmpty(tableName) Then
                createTableIndex += 1
                Continue While
            End If

            ' Cari kurung buka untuk definisi kolom
            Dim openParen As Integer = sqlContent.IndexOf("("c, startPos)
            If openParen = -1 Then
                createTableIndex += 1
                Continue While
            End If

            ' Cari kurung tutup yang sesuai
            Dim closeParen As Integer = FindMatchingParenthesis(sqlContent, openParen)
            If closeParen = -1 Then
                createTableIndex += 1
                Continue While
            End If

            ' Ekstrak definisi kolom
            Dim columnDefinitions As String = sqlContent.Substring(openParen + 1, closeParen - openParen - 1)

            ' Buat objek tabel
            Dim tableInfo As New TableInfo() With {.TableName = tableName}

            ' Parse kolom
            ParseColumnDefinitions(columnDefinitions, tableInfo.Columns)

            tables.Add(tableInfo)

            ' Lanjutkan pencarian
            createTableIndex = closeParen + 1
        End While

        Return tables
    End Function

    Private Sub ParseColumnDefinitions(columnDefs As String, columns As List(Of ColumnInfo))
        Dim lines As List(Of String) = SplitColumnsSafe(columnDefs)

        For Each line In lines
            Dim trimmed As String = line.Trim()
            If String.IsNullOrEmpty(trimmed) Then Continue For
            If IsConstraintLine(trimmed) Then Continue For

            Dim parts As List(Of String) = SplitSqlLine(trimmed)
            If parts.Count < 2 Then Continue For

            Dim colName As String = parts(0).Replace("`", "").Replace("""", "")
            Dim rawType As String = parts(1).ToUpper()

            Dim col As New ColumnInfo With {
            .ColumnName = colName,
            .DataType = rawType,
            .IsNullable = True
        }

            ' NULL / NOT NULL
            For i As Integer = 2 To parts.Count - 2
                If parts(i).ToUpper() = "NOT" AndAlso parts(i + 1).ToUpper() = "NULL" Then
                    col.IsNullable = False
                End If
            Next

            ' Length
            Dim m As Match = Regex.Match(rawType, "^(\w+)\((\d+)")
            If m.Success Then
                col.DataType = m.Groups(1).Value.ToUpper()
                Integer.TryParse(m.Groups(2).Value, col.MaxLength)
            Else
                col.DataType = rawType.Split(" "c)(0)
            End If

            columns.Add(col)
        Next
    End Sub


    Private Function SplitColumnsSafe(sql As String) As List(Of String)
        Dim result As New List(Of String)
        Dim buffer As New StringBuilder()
        Dim level As Integer = 0
        Dim inQuote As Boolean = False
        Dim quoteChar As Char = Chr(0)

        For Each c As Char In sql
            ' Handle quote
            If (c = "'"c OrElse c = """"c) Then
                If Not inQuote Then
                    inQuote = True
                    quoteChar = c
                ElseIf quoteChar = c Then
                    inQuote = False
                End If
            End If

            ' Handle kurung
            If Not inQuote Then
                If c = "("c Then level += 1
                If c = ")"c Then level -= 1
            End If

            ' Split hanya pada koma LEVEL LUAR
            If c = ","c AndAlso level = 0 AndAlso Not inQuote Then
                result.Add(buffer.ToString().Trim())
                buffer.Clear()
            Else
                buffer.Append(c)
            End If
        Next

        If buffer.Length > 0 Then
            result.Add(buffer.ToString().Trim())
        End If

        Return result
    End Function


    Private Function SplitSqlLine(line As String) As List(Of String)
        Dim parts As New List(Of String)
        Dim sb As New StringBuilder()
        Dim inQuote As Boolean = False
        Dim quoteChar As Char = Chr(0)
        Dim level As Integer = 0

        For Each c As Char In line
            ' Quote
            If (c = "'"c OrElse c = """"c) Then
                If Not inQuote Then
                    inQuote = True
                    quoteChar = c
                ElseIf quoteChar = c Then
                    inQuote = False
                End If
            End If

            ' Kurung
            If Not inQuote Then
                If c = "("c Then level += 1
                If c = ")"c Then level -= 1
            End If

            ' Split hanya di spasi luar
            If Not inQuote AndAlso level = 0 AndAlso Char.IsWhiteSpace(c) Then
                If sb.Length > 0 Then
                    parts.Add(sb.ToString())
                    sb.Clear()
                End If
            Else
                sb.Append(c)
            End If
        Next

        If sb.Length > 0 Then
            parts.Add(sb.ToString())
        End If

        Return parts
    End Function


    Private Function IsConstraintLine(line As String) As Boolean
        Dim upperLine As String = line.ToUpper()
        Return upperLine.StartsWith("PRIMARY KEY") OrElse
               upperLine.StartsWith("FOREIGN KEY") OrElse
               upperLine.StartsWith("CONSTRAINT") OrElse
               upperLine.StartsWith("UNIQUE") OrElse
               upperLine.StartsWith("KEY") OrElse
               upperLine.StartsWith("INDEX") OrElse
               upperLine.StartsWith("CHECK") OrElse
               upperLine.StartsWith("FULLTEXT") OrElse
               upperLine.StartsWith("SPATIAL")
    End Function

    Private Function FindMatchingParenthesis(text As String, startIndex As Integer) As Integer
        Dim count As Integer = 1
        Dim i As Integer = startIndex + 1

        While i < text.Length
            If text(i) = "("c Then
                count += 1
            ElseIf text(i) = ")"c Then
                count -= 1
                If count = 0 Then Return i
            End If
            i += 1
        End While

        Return -1
    End Function

    Private Function SkipWhitespace(text As String, startIndex As Integer) As Integer
        Dim i As Integer = startIndex
        While i < text.Length AndAlso Char.IsWhiteSpace(text(i))
            i += 1
        End While
        Return i
    End Function

    Private Function RemoveComments(sqlContent As String) As String
        ' Hapus komentar satu baris
        Dim lines As String() = sqlContent.Split(New String() {vbCrLf, vbLf, vbCr}, StringSplitOptions.None)
        Dim result As New StringBuilder()

        For Each line In lines
            Dim trimmedLine As String = line.Trim()
            If trimmedLine.StartsWith("--") OrElse trimmedLine.StartsWith("#") Then
                Continue For
            End If

            ' Hapus komentar di akhir baris
            Dim dashIndex As Integer = line.IndexOf("--")
            If dashIndex >= 0 Then
                result.AppendLine(line.Substring(0, dashIndex))
            Else
                result.AppendLine(line)
            End If
        Next

        Return result.ToString()
    End Function

    ' Ambil struktur dari database
    Private Function GetDatabaseTables() As List(Of TableInfo)
        Dim tables As New List(Of TableInfo)()
        Dim tableNames As New List(Of String)()

        Try
            ' 1️⃣ Ambil nama tabel SAJA
            Using cmd As New MySqlCommand("SHOW TABLES", conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        tableNames.Add(reader.GetString(0))
                    End While
                End Using
            End Using

            ' 2️⃣ Ambil kolom SETELAH reader pertama ditutup
            For Each tableName In tableNames
                Dim tableInfo As New TableInfo() With {.TableName = tableName}
                GetTableColumns(conn, tableName, tableInfo.Columns)
                tables.Add(tableInfo)
            Next

        Catch ex As Exception
            ListBoxHasil.Items.Add($"ERROR mengambil struktur database: {ex.Message}")
            Throw
        End Try

        Return tables
    End Function


    Private Sub GetTableColumns(conn As MySqlConnection, tableName As String, columns As List(Of ColumnInfo))
        Dim query As String = $"DESCRIBE `{tableName}`"

        Using cmd As New MySqlCommand(query, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim colInfo As New ColumnInfo() With {
                        .ColumnName = reader.GetString(0),
                        .DataType = reader.GetString(1).ToUpper(),
                        .IsNullable = (reader.GetString(2).ToUpper() = "YES")
                    }

                    ' Parse max length
                    Dim typeMatch As Match = Regex.Match(colInfo.DataType, "^(\w+)\((\d+)\)")
                    If typeMatch.Success Then
                        colInfo.DataType = typeMatch.Groups(1).Value.ToUpper()
                        Integer.TryParse(typeMatch.Groups(2).Value, colInfo.MaxLength)
                    End If

                    columns.Add(colInfo)
                End While
            End Using
        End Using
    End Sub

    ' Perbandingan sederhana
    Private Sub CompareStructuresSimple(masterTables As List(Of TableInfo), dbTables As List(Of TableInfo),
                                   discrepancies As List(Of String))

        ' Buat dictionary untuk pencarian cepat
        Dim dbTablesDict As New Dictionary(Of String, TableInfo)(StringComparer.OrdinalIgnoreCase)
        For Each dbTable In dbTables
            dbTablesDict(dbTable.TableName) = dbTable
        Next

        Dim masterTablesDict As New Dictionary(Of String, TableInfo)(StringComparer.OrdinalIgnoreCase)
        For Each masterTable In masterTables
            masterTablesDict(masterTable.TableName) = masterTable
        Next

        ' 1. Cek tabel yang ada di master tapi tidak di database
        For Each masterTable In masterTables
            If Not dbTablesDict.ContainsKey(masterTable.TableName) Then
                discrepancies.Add($"❌ Tabel '{masterTable.TableName}' ada di master tapi tidak di database")
            End If
        Next

        ' 2. Cek tabel yang ada di database tapi tidak di master
        For Each dbTable In dbTables
            If Not masterTablesDict.ContainsKey(dbTable.TableName) Then
                discrepancies.Add($"⚠️ Tabel '{dbTable.TableName}' ada di database tapi tidak di master")
            End If
        Next

        ' 3. Untuk tabel yang ada di kedua, bandingkan kolom
        For Each masterTable In masterTables
            If dbTablesDict.ContainsKey(masterTable.TableName) Then
                CompareTableColumns(masterTable, dbTablesDict(masterTable.TableName), discrepancies)
            End If
        Next
    End Sub

    Private Sub CompareTableColumns(masterTable As TableInfo, dbTable As TableInfo,
                               discrepancies As List(Of String))

        Dim dbColumnsDict As New Dictionary(Of String, ColumnInfo)(StringComparer.OrdinalIgnoreCase)
        For Each dbCol In dbTable.Columns
            If Not dbColumnsDict.ContainsKey(dbCol.ColumnName) Then
                dbColumnsDict.Add(dbCol.ColumnName, dbCol)
            End If
        Next

        Dim masterColumnsDict As New Dictionary(Of String, ColumnInfo)(StringComparer.OrdinalIgnoreCase)
        For Each masterCol In masterTable.Columns
            If Not masterColumnsDict.ContainsKey(masterCol.ColumnName) Then
                masterColumnsDict.Add(masterCol.ColumnName, masterCol)
            End If
        Next

        ' 1. Kolom di master tapi tidak di database
        For Each masterCol In masterTable.Columns
            If Not dbColumnsDict.ContainsKey(masterCol.ColumnName) Then
                discrepancies.Add(
                $"❌ Tabel '{masterTable.TableName}': Kolom '{masterCol.ColumnName}' tidak ditemukan di database"
            )
            End If
        Next

        ' 2. Kolom di database tapi tidak di master
        For Each dbCol In dbTable.Columns
            If Not masterColumnsDict.ContainsKey(dbCol.ColumnName) Then
                discrepancies.Add(
                $"⚠️ Tabel '{masterTable.TableName}': Kolom '{dbCol.ColumnName}' ada di database tapi tidak di master"
            )
            End If
        Next

        ' 3. Bandingkan properti kolom
        For Each masterCol In masterTable.Columns
            If Not dbColumnsDict.ContainsKey(masterCol.ColumnName) Then Continue For

            Dim dbCol As ColumnInfo = dbColumnsDict(masterCol.ColumnName)

            ' --- TYPE ---
            Dim masterType As String = NormalizeType(masterCol.DataType)
            Dim dbType As String = NormalizeType(dbCol.DataType)

            If masterType <> dbType Then
                discrepancies.Add(
                $"⚠️ Tabel '{masterTable.TableName}'.'{masterCol.ColumnName}': " &
                $"Tipe berbeda (Master: {masterCol.DataType}, DB: {dbCol.DataType})"
            )
            End If

            ' --- LENGTH ---
            If masterCol.MaxLength > 0 AndAlso dbCol.MaxLength > 0 Then
                If masterCol.MaxLength <> dbCol.MaxLength Then
                    discrepancies.Add(
                    $"⚠️ Tabel '{masterTable.TableName}'.'{masterCol.ColumnName}': " &
                    $"Panjang berbeda (Master: {masterCol.MaxLength}, DB: {dbCol.MaxLength})"
                )
                End If
            End If

            ' --- NULLABLE ---
            If masterCol.IsNullable <> dbCol.IsNullable Then
                discrepancies.Add(
                $"⚠️ Tabel '{masterTable.TableName}'.'{masterCol.ColumnName}': " &
                $"NULLABLE berbeda (Master: {If(masterCol.IsNullable, "NULL", "NOT NULL")}, " &
                $"DB: {If(dbCol.IsNullable, "NULL", "NOT NULL")})"
            )
            End If
        Next
    End Sub


    Private Function NormalizeType(dataType As String) As String
        If String.IsNullOrWhiteSpace(dataType) Then Return ""

        Dim t As String = dataType.ToUpper()

        t = t.Replace("UNSIGNED", "")
        t = Regex.Replace(t, "CHARACTER SET .*", "")
        t = Regex.Replace(t, "COLLATE .*", "")
        t = Regex.Replace(t, "\([^)]+\)", "")
        t = t.Trim()

        Select Case t
            Case "INTEGER", "INT4" : Return "INT"
            Case "DEC", "NUMERIC", "FIXED" : Return "DECIMAL"
            Case "BOOLEAN", "BOOL" : Return "TINYINT"
            Case "INT1" : Return "TINYINT"
            Case "INT2" : Return "SMALLINT"
            Case "INT3" : Return "MEDIUMINT"
            Case "INT8" : Return "BIGINT"
            Case "CHARACTER VARYING" : Return "VARCHAR"
            Case "CHARACTER" : Return "CHAR"
            Case Else : Return t
        End Select
    End Function


    ' Helper untuk debugging
    Private Sub SaveTablesToFile(tables As List(Of TableInfo), fileName As String, title As String)
        Try
            Dim fullPath As String = System.IO.Path.Combine(Application.StartupPath, fileName)

            Using writer As New StreamWriter(fullPath, False)
                writer.WriteLine(title)
                writer.WriteLine(New String("=", 60))
                writer.WriteLine()

                For Each table In tables
                    writer.WriteLine($"TABEL: {table.TableName} ({table.Columns.Count} kolom)")
                    writer.WriteLine(New String("-", 40))

                    For Each col In table.Columns
                        writer.WriteLine($"  {col.ColumnName.PadRight(30)} : {col.DataType.PadRight(20)} " &
                                     $"{If(col.MaxLength > 0, $"({col.MaxLength})", "").PadRight(10)} " &
                                     $"{If(col.IsNullable, "NULL", "NOT NULL")}")
                    Next
                    writer.WriteLine()
                Next
            End Using
        Catch
            ' abaikan error debug
        End Try
    End Sub


    ' Tampilkan hasil
    Private Sub DisplayResults(discrepancies As List(Of String))
        ListBoxHasil.Items.Clear()

        If discrepancies.Count = 0 Then
            ListBoxHasil.Items.Add("✓ TIDAK ADA PERBEDAAN")
            ListBoxHasil.Items.Add("Struktur database sudah sesuai dengan file master.")
            ListBoxHasil.Items.Add("")
            ListBoxHasil.Items.Add("Semua tabel dan kolom sesuai!")

            MessageBox.Show("Struktur database sudah sesuai dengan file master.",
                       "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            ListBoxHasil.Items.Add("⚠️ PERBEDAAN DITEMUKAN")
            ListBoxHasil.Items.Add("======================")
            ListBoxHasil.Items.Add("")

            For i As Integer = 0 To Math.Min(discrepancies.Count - 1, 50) ' Batasi tampilan
                ListBoxHasil.Items.Add($"{i + 1}. {discrepancies(i)}")
            Next

            If discrepancies.Count > 50 Then
                ListBoxHasil.Items.Add("")
                ListBoxHasil.Items.Add($"... dan {discrepancies.Count - 50} perbedaan lainnya")
            End If

            ListBoxHasil.Items.Add("")
            ListBoxHasil.Items.Add($"Total: {discrepancies.Count} perbedaan")

            ' Simpan ke file
            SaveDiscrepanciesToFile(discrepancies)

            'MessageBox.Show($"Ditemukan {discrepancies.Count} perbedaan struktur. " &
            '"Lihat ListBox untuk detail dan file discrepancies.txt untuk daftar lengkap.",
            '"Hasil Pengecekan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub SaveDiscrepanciesToFile(discrepancies As List(Of String))
        Try
            Dim fullPath As String = System.IO.Path.Combine(Application.StartupPath, "discrepancies.txt")

            Using writer As New StreamWriter(fullPath, False)
                writer.WriteLine("Hasil Pengecekan Struktur Database")
                writer.WriteLine($"Tanggal: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                writer.WriteLine($"Database: {database}")
                writer.WriteLine(New String("-", 50))
                writer.WriteLine()

                If discrepancies.Count = 0 Then
                    writer.WriteLine("Tidak ada perbedaan.")
                Else
                    writer.WriteLine($"DAFTAR PERBEDAAN ({discrepancies.Count} item):")
                    writer.WriteLine()

                    For i As Integer = 0 To discrepancies.Count - 1
                        writer.WriteLine($"{i + 1}. {discrepancies(i)}")
                    Next
                End If
            End Using

            ListBoxHasil.Items.Add("")
            ListBoxHasil.Items.Add("✓ Detail disimpan ke: discrepancies.txt")

        Catch ex As Exception
            ListBoxHasil.Items.Add("")
            ListBoxHasil.Items.Add($"✗ Gagal menyimpan: {ex.Message}")
        End Try
    End Sub


    ' Tombol untuk melihat file debug
    Private Sub BtnDebug_Click(sender As Object, e As EventArgs) Handles BtnDebug.Click
        Try
            If File.Exists("debug_master_tables.txt") Then
                Process.Start("notepad.exe", "debug_master_tables.txt")
            End If
            If File.Exists("debug_db_tables.txt") Then
                Process.Start("notepad.exe", "debug_db_tables.txt")
            End If
        Catch ex As Exception
            ' Ignore
        End Try
    End Sub

    Private Sub ListBoxHasil_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListBoxHasil.DrawItem
        If e.Index < 0 Then Return

        e.DrawBackground()

        Dim itemText As String = ListBoxHasil.Items(e.Index).ToString()
        Dim brushColor As Brush = Brushes.Black
        Dim fontStyle As FontStyle = FontStyle.Regular

        If itemText.Contains("❌") Then
            brushColor = Brushes.Red
            fontStyle = FontStyle.Bold
        ElseIf itemText.Contains("⚠️") Then
            brushColor = Brushes.OrangeRed
            fontStyle = FontStyle.Bold
        ElseIf itemText.Contains("✓") Then
            brushColor = Brushes.Green
            fontStyle = FontStyle.Bold
        ElseIf itemText.StartsWith("PERBEDAAN") OrElse
               itemText.StartsWith("TIDAK ADA") OrElse
               itemText.Contains("=====") Then
            brushColor = Brushes.Blue
            fontStyle = FontStyle.Bold
        End If

        Using font As New Font(ListBoxHasil.Font, fontStyle)
            e.Graphics.DrawString(itemText, font, brushColor, e.Bounds)
        End Using

        e.DrawFocusRectangle()
    End Sub

    Private Sub BtnHasil_Click(sender As Object, e As EventArgs) Handles BtnHasil.Click
        Try
            If File.Exists("discrepancies.txt") Then
                Process.Start("notepad.exe", "discrepancies.txt")
            End If
        Catch ex As Exception
            ' Ignore
        End Try
    End Sub
End Class