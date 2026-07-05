''' <summary>
''' Membandingkan ExpectedSchema (dari SchemaRegistry) dengan
''' ActualSchema (dari DbSchemaHelper) dan menghasilkan SchemaDiff.
''' </summary>
Public Class SchemaComparer

    ''' <summary>
    ''' Bandingkan expected vs actual.
    ''' </summary>
    Public Shared Function Compare(
        expected As Dictionary(Of String, TableSchema),
        actual As Dictionary(Of String, TableSchema)
    ) As SchemaDiff

        Dim diff As New SchemaDiff()

        ' 1. Cari tabel yang ada di expected tapi tidak ada di actual (missing)
        For Each kvp In expected
            If Not actual.ContainsKey(kvp.Key) Then
                diff.MissingTables.Add(kvp.Key)
            End If
        Next

        ' 2. Cari tabel yang ada di actual tapi tidak ada di expected (extra) — info saja
        For Each kvp In actual
            If Not expected.ContainsKey(kvp.Key) Then
                diff.ExtraTables.Add(kvp.Key)
            End If
        Next

        ' 3. Untuk tabel yang ada di kedua-duanya, bandingkan kolom dan index
        For Each kvp In expected
            If actual.ContainsKey(kvp.Key) Then
                CompareTableColumns(kvp.Key, kvp.Value, actual(kvp.Key), diff)
                CompareTableIndexes(kvp.Key, kvp.Value, actual(kvp.Key), diff)
            End If
        Next

        Return diff
    End Function

    ''' <summary>
    ''' Bandingkan kolom satu tabel.
    ''' </summary>
    Private Shared Sub CompareTableColumns(
        tableName As String,
        expected As TableSchema,
        actual As TableSchema,
        diff As SchemaDiff
    )

        ' Kolom yang ada di expected tapi tidak ada di actual → ADD
        For Each expCol In expected.Columns
            Dim actCol = actual.FindColumn(expCol.Name)
            If actCol Is Nothing Then
                diff.ColumnChanges.Add(New ColumnChange() With {
                    .TableName = tableName,
                    .ColumnName = expCol.Name,
                    .Type = ChangeType.[Add],
                    .Expected = expCol,
                    .Actual = Nothing
                })
            Else
                ' Kolom ada di kedua — cek perubahan tipe, nullable, default
                If HasColumnChanged(expCol, actCol) Then
                    diff.ColumnChanges.Add(New ColumnChange() With {
                        .TableName = tableName,
                        .ColumnName = expCol.Name,
                        .Type = ChangeType.Modify,
                        .Expected = expCol,
                        .Actual = actCol
                    })
                End If
            End If
        Next

        ' Kolom yang ada di actual tapi tidak ada di expected → DROP (info saja, tidak auto-drop)
        ' Tidak ditambahkan ke diff karena policy: never auto-drop
    End Sub

    ''' <summary>
    ''' Cek apakah kolom expected berbeda dari actual.
    ''' </summary>
    Private Shared Function HasColumnChanged(expected As ColumnSchema, actual As ColumnSchema) As Boolean
        ' Cek tipe data (normalized)
        If Not String.Equals(expected.NormalizedType(), actual.NormalizedType(), StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If

        ' Cek length/precision/scale
        If expected.Length <> actual.Length Then Return True
        If expected.Precision <> actual.Precision Then Return True
        If expected.Scale <> actual.Scale Then Return True

        ' Cek nullable
        If expected.IsNullable <> actual.IsNullable Then Return True

        ' Cek unsigned
        If expected.IsUnsigned <> actual.IsUnsigned Then Return True

        ' Cek default value
        Dim expDefault = If(expected.DefaultValue, "")
        Dim actDefault = If(actual.DefaultValue, "")
        If Not String.Equals(expDefault, actDefault, StringComparison.OrdinalIgnoreCase) Then
            ' Abaikan CURRENT_TIMESTAMP vs CURRENT_TIMESTAMP ON UPDATE
            If Not (expDefault.Contains("CURRENT_TIMESTAMP") AndAlso actDefault.Contains("CURRENT_TIMESTAMP")) Then
                Return True
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Bandingkan index satu tabel.
    ''' </summary>
    Private Shared Sub CompareTableIndexes(
        tableName As String,
        expected As TableSchema,
        actual As TableSchema,
        diff As SchemaDiff
    )

        ' Index yang ada di expected tapi tidak ada di actual → ADD
        For Each expIdx In expected.Indexes
            Dim actIdx = actual.FindIndex(expIdx.Name)
            If actIdx Is Nothing Then
                diff.IndexChanges.Add(New IndexChange() With {
                    .TableName = tableName,
                    .IndexName = expIdx.Name,
                    .Type = ChangeType.[Add],
                    .Expected = expIdx,
                    .Actual = Nothing
                })
            ElseIf Not IndexesMatch(expIdx, actIdx) Then
                ' Index ada tapi definisi berbeda → drop lama + add baru
                diff.IndexChanges.Add(New IndexChange() With {
                    .TableName = tableName,
                    .IndexName = expIdx.Name,
                    .Type = ChangeType.Drop,
                    .Expected = Nothing,
                    .Actual = actIdx
                })
                diff.IndexChanges.Add(New IndexChange() With {
                    .TableName = tableName,
                    .IndexName = expIdx.Name,
                    .Type = ChangeType.[Add],
                    .Expected = expIdx,
                    .Actual = Nothing
                })
            End If
        Next

        ' Index yang ada di actual tapi tidak ada di expected → INFO (tidak auto-drop)
    End Sub

    ''' <summary>
    ''' Cek apakah dua index memiliki definisi yang sama.
    ''' </summary>
    Private Shared Function IndexesMatch(expected As IndexSchema, actual As IndexSchema) As Boolean
        If expected.IsUnique <> actual.IsUnique Then Return False
        If expected.Columns.Count <> actual.Columns.Count Then Return False

        For i As Integer = 0 To expected.Columns.Count - 1
            If Not String.Equals(expected.Columns(i), actual.Columns(i), StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If
        Next

        Return True
    End Function
End Class
