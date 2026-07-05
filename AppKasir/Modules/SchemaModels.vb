''' <summary>
''' Definisi satu kolom tabel.
''' </summary>
Public Class ColumnSchema
    Public Property Name As String
    Public Property DataType As String          ' varchar, int, decimal, datetime, dll
    Public Property Length As Integer = -1      ' varchar(50) → 50, -1 jika tidak ada
    Public Property Precision As Integer = -1   ' decimal(15,2) → 15
    Public Property Scale As Integer = -1       ' decimal(15,2) → 2
    Public Property IsNullable As Boolean = True
    Public Property DefaultValue As String = Nothing  ' Nothing = tidak ada default
    Public Property IsAutoIncrement As Boolean = False
    Public Property Comment As String = Nothing
    Public Property IsUnsigned As Boolean = False
    Public Property IsZerofill As Boolean = False

    ''' <summary>
    ''' Representasi SQL lengkap kolom ini (untuk CREATE TABLE).
    ''' </summary>
    Public Function ToSql() As String
        Dim parts As New List(Of String)
        parts.Add($"`{Name}`")
        parts.Add(DataTypeString())
        If Not IsNullable Then parts.Add("NOT NULL")
        If DefaultValue IsNot Nothing Then
            If DefaultValue.Equals("CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase) Then
                parts.Add("DEFAULT CURRENT_TIMESTAMP")
            ElseIf DefaultValue.Equals("NOW()", StringComparison.OrdinalIgnoreCase) Then
                parts.Add("DEFAULT CURRENT_TIMESTAMP")
            Else
                parts.Add($"DEFAULT '{DefaultValue}'")
            End If
        End If
        If IsAutoIncrement Then parts.Add("AUTO_INCREMENT")
        If Comment IsNot Nothing Then parts.Add($"COMMENT '{Comment}'")
        Return String.Join(" ", parts)
    End Function

    ''' <summary>
    ''' Tipe data lengkap termasuk panjang/precision.
    ''' </summary>
    Public Function DataTypeString() As String
        If String.IsNullOrWhiteSpace(DataType) Then Return "unknown"
        Dim t As String = DataType.ToLower()
        If t = "int" AndAlso IsUnsigned Then t = "int unsigned"
        If t = "tinyint" AndAlso IsUnsigned Then t = "tinyint unsigned"
        If t = "smallint" AndAlso IsUnsigned Then t = "smallint unsigned"
        If t = "bigint" AndAlso IsUnsigned Then t = "bigint unsigned"

        If Length > 0 Then
            Return $"{t}({Length})"
        ElseIf Precision > 0 AndAlso Scale >= 0 Then
            Return $"{t}({Precision},{Scale})"
        ElseIf Precision > 0 Then
            Return $"{t}({Precision})"
        End If
        Return t
    End Function

    ''' <summary>
    ''' Normalisasi tipe data untuk perbandingan.
    ''' Menghilangkan spasi, lowercase, ignore display width MySQL.
    ''' </summary>
    Public Function NormalizedType() As String
        Dim raw As String = DataType.ToLower().Replace(" ", "")
        ' Hilangkan display width dari int(11) → int
        raw = System.Text.RegularExpressions.Regex.Replace(raw, "\(\d+\)", "")
        Return raw
    End Function

    Public Overrides Function ToString() As String
        Return $"{Name} {DataTypeString()} {If(Not IsNullable, "NOT NULL", "NULL")}"
    End Function
End Class

''' <summary>
''' Definisi index (biasa, unique, primary key, fulltext).
''' </summary>
Public Class IndexSchema
    Public Property Name As String
    Public Property Columns As New List(Of String)
    Public Property IsUnique As Boolean = False
    Public Property IsPrimary As Boolean = False
    Public Property IsFullText As Boolean = False

    ''' <summary>
    ''' Kolom yang diindex, diurutkan berdasarkan seq_in_index.
    ''' </summary>
    Public ReadOnly Property ColumnList As String
        Get
            Return String.Join(", ", Columns.Select(Function(c) $"`{c}`"))
        End Get
    End Property

    Public Overrides Function ToString() As String
        Dim prefix = If(IsPrimary, "PRIMARY KEY", If(IsUnique, "UNIQUE KEY", "KEY"))
        Return $"{prefix} `{Name}` ({ColumnList})"
    End Function
End Class

''' <summary>
''' Definisi lengkap satu tabel.
''' Digunakan oleh SchemaRegistry (expected) dan DbSchemaHelper (actual).
''' </summary>
Public Class TableSchema
    Public Property TableName As String
    Public Property Columns As New List(Of ColumnSchema)
    Public Property Indexes As New List(Of IndexSchema)
    Public Property PrimaryKey As New List(Of String)
    Public Property Engine As String = "InnoDB"
    Public Property Charset As String = "utf8mb4"
    Public Property Collation As String = "utf8mb4_unicode_ci"
    Public Property Comment As String = Nothing

    ''' <summary>
    ''' Cari kolom berdasarkan nama (case-insensitive).
    ''' </summary>
    Public Function FindColumn(name As String) As ColumnSchema
        Return Columns.FirstOrDefault(Function(c) String.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
    End Function

    ''' <summary>
    ''' Cari index berdasarkan nama (case-insensitive).
    ''' </summary>
    Public Function FindIndex(name As String) As IndexSchema
        Return Indexes.FirstOrDefault(Function(i) String.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase))
    End Function

    ''' <summary>
    ''' Generate CREATE TABLE IF NOT EXISTS statement.
    ''' </summary>
    Public Function ToCreateTableSql() As String
        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine($"CREATE TABLE IF NOT EXISTS `{TableName}` (")

        Dim defs As New List(Of String)
        For Each col In Columns
            defs.Add("    " & col.ToSql())
        Next

        If PrimaryKey.Count > 0 Then
            Dim pkCols = String.Join(", ", PrimaryKey.Select(Function(c) $"`{c}`"))
            defs.Add($"    PRIMARY KEY ({pkCols})")
        End If

        For Each idx In Indexes.Where(Function(i) Not i.IsPrimary)
            If idx.IsUnique Then
                defs.Add($"    UNIQUE KEY `{idx.Name}` ({idx.ColumnList})")
            ElseIf idx.IsFullText Then
                defs.Add($"    FULLTEXT KEY `{idx.Name}` ({idx.ColumnList})")
            Else
                defs.Add($"    KEY `{idx.Name}` ({idx.ColumnList})")
            End If
        Next

        sb.AppendLine(String.Join("," & vbCrLf, defs))
        sb.Append(")")
        If Engine IsNot Nothing Then sb.Append($" ENGINE={Engine}")
        If Charset IsNot Nothing Then sb.Append($" DEFAULT CHARSET={Charset}")
        If Collation IsNot Nothing Then sb.Append($" COLLATE={Collation}")
        If Comment IsNot Nothing Then sb.Append($" COMMENT='{Comment}'")
        sb.Append(";")

        Return sb.ToString()
    End Function
End Class

''' <summary>
''' Tipe perubahan kolom/index.
''' </summary>
Public Enum ChangeType
    [Add]
    Modify
    Drop
End Enum

''' <summary>
''' Perubahan satu kolom dalam satu tabel.
''' </summary>
Public Class ColumnChange
    Public Property TableName As String
    Public Property ColumnName As String
    Public Property Type As ChangeType
    Public Property Expected As ColumnSchema   ' Nothing jika Add
    Public Property Actual As ColumnSchema     ' Nothing jika Drop

    Public ReadOnly Property Description As String
        Get
            Select Case Type
                Case ChangeType.[Add]
                    Return $"ADD COLUMN {ColumnName} {Expected?.DataTypeString()}"
                Case ChangeType.Modify
                    Return $"MODIFY COLUMN {ColumnName}: {Actual?.DataTypeString()} → {Expected?.DataTypeString()}"
                Case ChangeType.Drop
                    Return $"DROP COLUMN {ColumnName}"
                Case Else
                    Return ""
            End Select
        End Get
    End Property
End Class

''' <summary>
''' Perubahan satu index dalam satu tabel.
''' </summary>
Public Class IndexChange
    Public Property TableName As String
    Public Property IndexName As String
    Public Property Type As ChangeType
    Public Property Expected As IndexSchema    ' Nothing jika Drop
    Public Property Actual As IndexSchema      ' Nothing jika Add

    Public ReadOnly Property Description As String
        Get
            Select Case Type
                Case ChangeType.[Add]
                    Return $"ADD INDEX {IndexName} ({Expected?.ColumnList})"
                Case ChangeType.Drop
                    Return $"DROP INDEX {IndexName}"
                Case Else
                    Return ""
            End Select
        End Get
    End Property
End Class

''' <summary>
''' Hasil perbandingan antara expected dan actual schema.
''' </summary>
Public Class SchemaDiff
    Public Property MissingTables As New List(Of String)      ' Ada di expected, tidak ada di actual
    Public Property ExtraTables As New List(Of String)        ' Ada di actual, tidak ada di expected (info saja)
    Public Property ColumnChanges As New List(Of ColumnChange)
    Public Property IndexChanges As New List(Of IndexChange)

    Public ReadOnly Property HasDifferences As Boolean
        Get
            Return MissingTables.Count > 0 OrElse
                   ColumnChanges.Count > 0 OrElse
                   IndexChanges.Count > 0
        End Get
    End Property

    Public ReadOnly Property Summary As String
        Get
            Dim parts As New List(Of String)
            If MissingTables.Count > 0 Then parts.Add($"{MissingTables.Count} tabel baru")
            If ColumnChanges.Count > 0 Then parts.Add($"{ColumnChanges.Count} perubahan kolom")
            If IndexChanges.Count > 0 Then parts.Add($"{IndexChanges.Count} perubahan index")
            If ExtraTables.Count > 0 Then parts.Add($"{ExtraTables.Count} tabel ekstra (info)")
            If parts.Count = 0 Then Return "Tidak ada perubahan"
            Return String.Join(", ", parts)
        End Get
    End Property
End Class

''' <summary>
''' Satu langkah migrasi (per tabel).
''' Berisi SQL statements yang perlu dieksekusi.
''' </summary>
Public Class MigrationStep
    Public Property TableName As String
    Public Property SqlStatements As New List(Of String)
    Public Property Description As String

    Public ReadOnly Property StatementCount As Integer
        Get
            Return SqlStatements.Count
        End Get
    End Property
End Class

''' <summary>
''' Rencana migrasi lengkap.
''' Kumpulan MigrationStep yang akan dieksekusi.
''' </summary>
Public Class MigrationPlan
    Public Property Steps As New List(Of MigrationStep)
    Public Property GeneratedAt As DateTime = DateTime.Now
    Public Property Diff As SchemaDiff

    Public ReadOnly Property TotalStatements As Integer
        Get
            Return Steps.Sum(Function(s) s.SqlStatements.Count)
        End Get
    End Property

    Public ReadOnly Property Summary As String
        Get
            Return $"{Steps.Count} tabel, {TotalStatements} statement — {GeneratedAt:yyyy-MM-dd HH:mm:ss}"
        End Get
    End Property

    ''' <summary>
    ''' Generate header SQL dengan timestamp dan ringkasan.
    ''' </summary>
    Public Function ToHeaderSql() As String
        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine("-- ============================================================")
        sb.AppendLine($"-- AUTO-GENERATED MIGRATION — {GeneratedAt:yyyy-MM-dd HH:mm:ss}")
        sb.AppendLine($"-- Tables: {Steps.Count} | Statements: {TotalStatements}")
        If Diff IsNot Nothing Then
            sb.AppendLine($"-- {Diff.Summary}")
        End If
        sb.AppendLine("-- ============================================================")
        sb.AppendLine()
        Return sb.ToString()
    End Function
End Class

''' <summary>
''' Hasil eksekusi satu statement migrasi.
''' </summary>
Public Class MigrationResult
    Public Property SequenceNumber As Integer
    Public Property TableName As String
    Public Property SqlStatement As String
    Public Property Status As MigrationStatus
    Public Property ErrorMessage As String = Nothing
    Public Property ExecutedAt As DateTime = DateTime.Now
End Class

''' <summary>
''' Status eksekusi statement migrasi.
''' </summary>
Public Enum MigrationStatus
    Success
    Skipped
    Failed
End Enum
