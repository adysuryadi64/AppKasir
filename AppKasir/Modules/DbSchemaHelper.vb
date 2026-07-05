Imports MySql.Data.MySqlClient
Imports System.Text.RegularExpressions

''' <summary>
''' Helper untuk membaca struktur aktual database dari information_schema MySQL.
''' Menghasilkan TableSchema dari database yang sedang berjalan.
''' </summary>
Public Class DbSchemaHelper

    ''' <summary>
    ''' Baca struktur aktual satu tabel dari information_schema.
    ''' Return Nothing jika tabel tidak ada.
    ''' </summary>
    Public Shared Function GetActualTableSchema(conn As MySqlConnection, tableName As String) As TableSchema
        ' Cek tabel ada
        If Not TableExists(conn, tableName) Then Return Nothing

        Dim table As New TableSchema()
        table.TableName = tableName

        ' Baca metadata tabel (engine, collation, comment)
        ReadTableMetadata(conn, table)

        ' Baca kolom
        ReadColumns(conn, table)

        ' Baca indexes (termasuk primary key)
        ReadIndexes(conn, table)

        Return table
    End Function

    ''' <summary>
    ''' Baca semua tabel dari database.
    ''' </summary>
    Public Shared Function GetAllTableSchemas(conn As MySqlConnection) As Dictionary(Of String, TableSchema)
        Dim result As New Dictionary(Of String, TableSchema)(StringComparer.OrdinalIgnoreCase)

        ' Kumpulkan nama tabel dulu — tutup reader sebelum query berikutnya
        Dim tableNames As New List(Of String)
        Using cmd As New MySqlCommand(
            "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_TYPE = 'BASE TABLE'", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    tableNames.Add(reader.GetString("TABLE_NAME"))
                End While
            End Using
        End Using

        ' Proses tiap tabel setelah reader ditutup
        For Each tblName In tableNames
            Dim schema = GetActualTableSchema(conn, tblName)
            If schema IsNot Nothing Then
                result(tblName) = schema
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' Cek apakah tabel ada di database.
    ''' </summary>
    Public Shared Function TableExists(conn As MySqlConnection, tableName As String) As Boolean
        Using cmd As New MySqlCommand(
            "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tbl", conn)
            cmd.Parameters.AddWithValue("@tbl", tableName)
            Dim count = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    ''' <summary>
    ''' Baca metadata tabel: engine, collation, comment.
    ''' </summary>
    Private Shared Sub ReadTableMetadata(conn As MySqlConnection, table As TableSchema)
        Using cmd As New MySqlCommand(
            "SELECT ENGINE, TABLE_COLLATION, TABLE_COMMENT FROM information_schema.TABLES " &
            "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tbl", conn)
            cmd.Parameters.AddWithValue("@tbl", table.TableName)
            Using reader = cmd.ExecuteReader()
                If reader.Read() Then
                    table.Engine = If(IsDBNull(reader("ENGINE")), "InnoDB", reader("ENGINE").ToString())
                    Dim collation = If(IsDBNull(reader("TABLE_COLLATION")), "", reader("TABLE_COLLATION").ToString())
                    If collation.Contains("_") Then
                        table.Charset = collation.Split("_"c)(0)
                        table.Collation = collation
                    End If
                    table.Comment = If(IsDBNull(reader("TABLE_COMMENT")), Nothing, reader("TABLE_COMMENT").ToString())
                End If
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Baca semua kolom dari information_schema.COLUMNS.
    ''' </summary>
    Private Shared Sub ReadColumns(conn As MySqlConnection, table As TableSchema)
        Using cmd As New MySqlCommand(
            "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, " &
            "IS_NULLABLE, COLUMN_DEFAULT, EXTRA, COLUMN_COMMENT, COLUMN_TYPE " &
            "FROM information_schema.COLUMNS " &
            "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tbl " &
            "ORDER BY ORDINAL_POSITION", conn)
            cmd.Parameters.AddWithValue("@tbl", table.TableName)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim col As New ColumnSchema()
                    col.Name = reader("COLUMN_NAME").ToString()
                    col.DataType = reader("DATA_TYPE").ToString().ToLower()

                    ' Parse COLUMN_TYPE untuk length/precision/scale (lebih akurat dari DATA_TYPE)
                    Dim columnType = reader("COLUMN_TYPE").ToString().ToLower()
                    ParseColumnType(columnType, col)

                    ' Nullable
                    col.IsNullable = reader("IS_NULLABLE").ToString() = "YES"

                    ' Default value
                    If Not IsDBNull(reader("COLUMN_DEFAULT")) Then
                        col.DefaultValue = reader("COLUMN_DEFAULT").ToString()
                    End If

                    ' AUTO_INCREMENT
                    Dim extra = reader("EXTRA").ToString().ToLower()
                    col.IsAutoIncrement = extra.Contains("auto_increment")

                    ' Unsigned (dari COLUMN_TYPE)
                    col.IsUnsigned = columnType.Contains("unsigned")

                    ' Comment
                    If Not IsDBNull(reader("COLUMN_COMMENT")) Then
                        Dim comment = reader("COLUMN_COMMENT").ToString()
                        If Not String.IsNullOrWhiteSpace(comment) Then
                            col.Comment = comment
                        End If
                    End If

                    table.Columns.Add(col)
                End While
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Parse COLUMN_TYPE MySQL untuk extract length, precision, scale.
    ''' Contoh: varchar(50) → Length=50, decimal(15,2) → Precision=15, Scale=2
    ''' </summary>
    Private Shared Sub ParseColumnType(columnType As String, col As ColumnSchema)
        Dim match = Regex.Match(columnType, "\(([^)]+)\)")
        If Not match.Success Then Return

        Dim inside = match.Groups(1).Value

        If col.DataType = "decimal" OrElse col.DataType = "float" OrElse col.DataType = "double" Then
            Dim parts = inside.Split(","c)
            If parts.Length >= 1 Then Integer.TryParse(parts(0).Trim(), col.Precision)
            If parts.Length >= 2 Then Integer.TryParse(parts(1).Trim(), col.Scale)
        Else
            Integer.TryParse(inside, col.Length)
        End If
    End Sub

    ''' <summary>
    ''' Baca semua index dari information_schema.STATISTICS.
    ''' Includes PRIMARY KEY dan UNIQUE constraints.
    ''' </summary>
    Private Shared Sub ReadIndexes(conn As MySqlConnection, table As TableSchema)
        Using cmd As New MySqlCommand(
            "SELECT INDEX_NAME, COLUMN_NAME, SEQ_IN_INDEX, NON_UNIQUE " &
            "FROM information_schema.STATISTICS " &
            "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tbl " &
            "ORDER BY INDEX_NAME, SEQ_IN_INDEX", conn)
            cmd.Parameters.AddWithValue("@tbl", table.TableName)
            Using reader = cmd.ExecuteReader()
                ' Group by index name
                Dim indexDict As New Dictionary(Of String, IndexSchema)(StringComparer.OrdinalIgnoreCase)

                While reader.Read()
                    Dim idxName = reader("INDEX_NAME").ToString()
                    Dim colName = reader("COLUMN_NAME").ToString()
                    Dim seqInIndex = Convert.ToInt32(reader("SEQ_IN_INDEX"))
                    Dim nonUnique = Convert.ToInt32(reader("NON_UNIQUE"))

                    ' PRIMARY KEY
                    If idxName.Equals("PRIMARY", StringComparison.OrdinalIgnoreCase) Then
                        If seqInIndex = 1 Then table.PrimaryKey.Clear()
                        table.PrimaryKey.Add(colName)
                        Continue While
                    End If

                    ' Regular / Unique index
                    If Not indexDict.ContainsKey(idxName) Then
                        indexDict(idxName) = New IndexSchema() With {
                            .Name = idxName,
                            .IsUnique = (nonUnique = 0)
                        }
                    End If

                    ' Pastikan kolom urut berdasarkan SEQ_IN_INDEX
                    Dim idx = indexDict(idxName)
                    While idx.Columns.Count < seqInIndex
                        idx.Columns.Add("")
                    End While
                    idx.Columns(seqInIndex - 1) = colName
                End While

                ' Tambahkan semua index ke tabel
                For Each idx In indexDict.Values
                    table.Indexes.Add(idx)
                Next
            End Using
        End Using
    End Sub
End Class
