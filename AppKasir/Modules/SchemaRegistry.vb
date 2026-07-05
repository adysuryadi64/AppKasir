Imports System.IO
Imports System.Text.RegularExpressions

Public Class SchemaRegistry
    Private _tables As New Dictionary(Of String, TableSchema)(StringComparer.OrdinalIgnoreCase)

    Public ReadOnly Property Tables As Dictionary(Of String, TableSchema)
        Get
            Return _tables
        End Get
    End Property

    Public Function LoadFromFolder(schemaDir As String) As Integer
        _tables.Clear()

        If Not Directory.Exists(schemaDir) Then
            Throw New DirectoryNotFoundException("SchemaDef folder tidak ditemukan: " & schemaDir)
        End If

        Dim files As String() = Directory.GetFiles(schemaDir, "*.sql")
        Dim loaded As Integer = 0

        For Each filePath In files
            Try
                Dim content As String = File.ReadAllText(filePath)
                Dim table As TableSchema = ParseCreateTable(content)
                If table IsNot Nothing AndAlso Not _tables.ContainsKey(table.TableName) Then
                    _tables(table.TableName) = table
                    loaded += 1
                End If
            Catch ex As Exception
                Debug.WriteLine("Warning: Gagal parse " & Path.GetFileName(filePath) & ": " & ex.Message)
            End Try
        Next

        Return loaded
    End Function

    Public Function GetTableSchema(tableName As String) As TableSchema
        Dim result As TableSchema = Nothing
        _tables.TryGetValue(tableName, result)
        Return result
    End Function

    Public Function ParseCreateTable(sqlContent As String) As TableSchema
        sqlContent = RemoveComments(sqlContent)

        Dim match As Match = Regex.Match(sqlContent,
            "CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?`?(\w+)`?\s*\(",
            RegexOptions.IgnoreCase Or RegexOptions.Singleline)

        If Not match.Success Then Return Nothing

        Dim table As New TableSchema()
        table.TableName = match.Groups(1).Value

        Dim openParen As Integer = match.Index + match.Length - 1
        Dim closeParen As Integer = FindMatchingParenthesis(sqlContent, openParen)
        If closeParen = -1 Then Return Nothing

        Dim body As String = sqlContent.Substring(openParen + 1, closeParen - openParen - 1)

        Dim afterParen As String = sqlContent.Substring(closeParen + 1)
        ParseTableOptions(afterParen, table)

        ParseTableBody(body, table)

        Return table
    End Function

    Private Sub ParseTableBody(body As String, table As TableSchema)
        Dim definitions As List(Of String) = SplitDefinitions(body)

        For Each def In definitions
            Dim trimmed As String = def.Trim()
            If String.IsNullOrWhiteSpace(trimmed) Then Continue For

            Dim upper As String = trimmed.ToUpper()

            If upper.StartsWith("PRIMARY KEY") Then
                Dim pkMatch As Match = Regex.Match(trimmed, "PRIMARY\s+KEY\s*\(([^)]+)\)", RegexOptions.IgnoreCase)
                If pkMatch.Success Then
                    table.PrimaryKey = ParseColumnList(pkMatch.Groups(1).Value)
                End If

            ElseIf upper.StartsWith("UNIQUE KEY") OrElse upper.StartsWith("UNIQUE INDEX") Then
                Dim idx As IndexSchema = ParseIndexDefinition(trimmed, True)
                If idx IsNot Nothing Then table.Indexes.Add(idx)

            ElseIf upper.StartsWith("FULLTEXT KEY") OrElse upper.StartsWith("FULLTEXT INDEX") Then
                Dim idx As IndexSchema = ParseIndexDefinition(trimmed, False)
                If idx IsNot Nothing Then
                    idx.IsFullText = True
                    table.Indexes.Add(idx)
                End If

            ElseIf upper.StartsWith("KEY ") OrElse upper.StartsWith("INDEX ") Then
                Dim idx As IndexSchema = ParseIndexDefinition(trimmed, False)
                If idx IsNot Nothing Then table.Indexes.Add(idx)

            ElseIf upper.StartsWith("CONSTRAINT") Then
                Continue For

            Else
                Dim col As ColumnSchema = ParseColumnDefinition(trimmed)
                If col IsNot Nothing Then table.Columns.Add(col)
            End If
        Next
    End Sub

    Private Function ParseColumnDefinition(def As String) As ColumnSchema
        Dim nameMatch As Match = Regex.Match(def, "^`?(\w+)`?")
        If Not nameMatch.Success Then Return Nothing

        Dim col As New ColumnSchema()
        col.Name = nameMatch.Groups(1).Value

        Dim rest As String = def.Substring(nameMatch.Length).Trim()

        Dim typeMatch As Match = Regex.Match(rest,
            "(tinyint|smallint|mediumint|int|bigint|decimal|float|double|varchar|char|text|mediumtext|longtext|datetime|date|timestamp|time|blob|enum|set|json|bit|binary|varbinary)(?:\(([^)]+)\))?(?:\s+(?:unsigned|zerofill))*",
            RegexOptions.IgnoreCase)

        If typeMatch.Success Then
            col.DataType = typeMatch.Groups(1).Value.ToLower()
            Dim restLower As String = rest.ToLower()
            col.IsUnsigned = restLower.Contains("unsigned")
            col.IsZerofill = restLower.Contains("zerofill")

            If typeMatch.Groups(2).Success Then
                Dim sizeStr As String = typeMatch.Groups(2).Value
                If col.DataType = "decimal" Then
                    Dim parts As String() = sizeStr.Split(","c)
                    If parts.Length >= 1 Then Integer.TryParse(parts(0).Trim(), col.Precision)
                    If parts.Length >= 2 Then Integer.TryParse(parts(1).Trim(), col.Scale)
                Else
                    Integer.TryParse(sizeStr, col.Length)
                End If
            End If
        Else
            Return Nothing
        End If

        col.IsNullable = Not Regex.IsMatch(rest, "\bNOT\s+NULL\b", RegexOptions.IgnoreCase)

        Dim defaultMatch As Match = Regex.Match(rest, "DEFAULT\s+('(?:[^'\\]|\\.)*'|CURRENT_TIMESTAMP|NOW\(\)|NULL|\S+)", RegexOptions.IgnoreCase)
        If defaultMatch.Success Then
            Dim val As String = defaultMatch.Groups(1).Value.Trim("'"c)
            col.DefaultValue = val
        End If

        col.IsAutoIncrement = Regex.IsMatch(rest, "\bAUTO_INCREMENT\b", RegexOptions.IgnoreCase)

        Dim commentMatch As Match = Regex.Match(rest, "COMMENT\s+'((?:[^'\\]|\\.)*)'", RegexOptions.IgnoreCase)
        If commentMatch.Success Then
            col.Comment = commentMatch.Groups(1).Value
        End If

        Return col
    End Function

    Private Function ParseIndexDefinition(def As String, isUnique As Boolean) As IndexSchema
        Dim match As Match = Regex.Match(def,
            "(?:UNIQUE\s+)?(?:FULLTEXT\s+)?(?:KEY|INDEX)\s+`?(\w+)`?\s*\(([^)]+)\)",
            RegexOptions.IgnoreCase)
        If Not match.Success Then Return Nothing

        Dim idx As New IndexSchema()
        idx.Name = match.Groups(1).Value
        idx.IsUnique = isUnique
        idx.IsFullText = def.ToUpper().Contains("FULLTEXT")
        idx.Columns = ParseColumnList(match.Groups(2).Value)

        Return idx
    End Function

    Private Function ParseColumnList(columnListStr As String) As List(Of String)
        Dim result As New List(Of String)
        Dim parts As String() = columnListStr.Split(","c)
        For Each part In parts
            Dim cleaned As String = part.Trim().Trim("`"c)
            If Not String.IsNullOrWhiteSpace(cleaned) Then
                result.Add(cleaned)
            End If
        Next
        Return result
    End Function

    Private Sub ParseTableOptions(optionsStr As String, table As TableSchema)
        Dim engineMatch As Match = Regex.Match(optionsStr, "ENGINE\s*=\s*(\w+)", RegexOptions.IgnoreCase)
        If engineMatch.Success Then table.Engine = engineMatch.Groups(1).Value

        Dim charsetMatch As Match = Regex.Match(optionsStr, "DEFAULT\s+CHARSET\s*=\s*(\w+)", RegexOptions.IgnoreCase)
        If charsetMatch.Success Then table.Charset = charsetMatch.Groups(1).Value

        Dim collateMatch As Match = Regex.Match(optionsStr, "COLLATE\s*=\s*(\w+)", RegexOptions.IgnoreCase)
        If collateMatch.Success Then table.Collation = collateMatch.Groups(1).Value

        Dim commentMatch As Match = Regex.Match(optionsStr, "COMMENT\s*=\s*'([^']*)'", RegexOptions.IgnoreCase)
        If commentMatch.Success Then table.Comment = commentMatch.Groups(1).Value
    End Sub

    Private Function RemoveComments(sql As String) As String
        Dim result As New System.Text.StringBuilder()
        For Each line In sql.Split({vbLf, vbCr}, StringSplitOptions.None)
            Dim trimmed As String = line.TrimStart()
            If trimmed.StartsWith("--") Then Continue For
            result.AppendLine(line)
        Next
        Return result.ToString()
    End Function

    Private Function FindMatchingParenthesis(text As String, openPos As Integer) As Integer
        Dim depth As Integer = 1
        Dim inQuote As Boolean = False
        Dim i As Integer = openPos + 1

        While i < text.Length AndAlso depth > 0
            Dim c As Char = text(i)
            If c = "'"c AndAlso (i = 0 OrElse text(i - 1) <> "\"c) Then
                inQuote = Not inQuote
            ElseIf Not inQuote Then
                If c = "("c Then
                    depth += 1
                ElseIf c = ")"c Then
                    depth -= 1
                    If depth = 0 Then Return i
                End If
            End If
            i += 1
        End While

        Return -1
    End Function

    Private Function SplitDefinitions(body As String) As List(Of String)
        Dim result As New List(Of String)
        Dim current As New System.Text.StringBuilder()
        Dim depth As Integer = 0

        For Each c As Char In body
            If c = "("c Then
                depth += 1
                current.Append(c)
            ElseIf c = ")"c Then
                depth -= 1
                current.Append(c)
            ElseIf c = ","c AndAlso depth = 0 Then
                result.Add(current.ToString())
                current.Clear()
            Else
                current.Append(c)
            End If
        Next

        If current.Length > 0 Then result.Add(current.ToString())
        Return result
    End Function
End Class
