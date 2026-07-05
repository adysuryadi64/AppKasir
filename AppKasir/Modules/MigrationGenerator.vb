Imports System.IO
Imports System.Text

Public Class MigrationGenerator

    Public Shared Function Generate(diff As SchemaDiff) As MigrationPlan
        Dim plan As New MigrationPlan()
        plan.Diff = diff
        plan.GeneratedAt = DateTime.Now

        Dim tableGroups = diff.ColumnChanges.GroupBy(Function(c) c.TableName)

        For Each grp In tableGroups
            Dim mStep As New MigrationStep()
            mStep.TableName = grp.Key
            mStep.Description = "Alter table " & grp.Key & ": " & grp.Count().ToString() & " kolom berubah"

            Dim sqlStr As String = GenerateAlterTableSql(grp.Key, grp.ToList())
            If Not String.IsNullOrWhiteSpace(sqlStr) Then
                mStep.SqlStatements.Add(sqlStr)
            End If

            If mStep.SqlStatements.Count > 0 Then
                plan.Steps.Add(mStep)
            End If
        Next

        Dim idxAdds = diff.IndexChanges.Where(Function(c) c.Type = ChangeType.[Add]).ToList()
        Dim idxGroups = idxAdds.GroupBy(Function(c) c.TableName)

        For Each grp In idxGroups
            Dim existing = plan.Steps.FirstOrDefault(Function(s) s.TableName = grp.Key)
            Dim mStep As MigrationStep
            If existing IsNot Nothing Then
                mStep = existing
            Else
                mStep = New MigrationStep()
                mStep.TableName = grp.Key
            End If

            For Each idxChange In grp
                Dim sqlStr As String = GenerateCreateIndexSql(grp.Key, idxChange.Expected)
                If Not String.IsNullOrWhiteSpace(sqlStr) Then
                    mStep.SqlStatements.Add(sqlStr)
                End If
            Next

            If existing Is Nothing AndAlso mStep.SqlStatements.Count > 0 Then
                plan.Steps.Add(mStep)
            End If
        Next

        Dim idxDrops = diff.IndexChanges.Where(Function(c) c.Type = ChangeType.Drop).ToList()
        Dim idxDropGroups = idxDrops.GroupBy(Function(c) c.TableName)

        For Each grp In idxDropGroups
            Dim existing = plan.Steps.FirstOrDefault(Function(s) s.TableName = grp.Key)
            Dim mStep As MigrationStep
            If existing IsNot Nothing Then
                mStep = existing
            Else
                mStep = New MigrationStep()
                mStep.TableName = grp.Key
            End If

            For Each idxChange In grp
                Dim sqlStr As String = GenerateDropIndexSql(grp.Key, idxChange.IndexName)
                If Not String.IsNullOrWhiteSpace(sqlStr) Then
                    mStep.SqlStatements.Insert(0, sqlStr)
                End If
            Next

            If existing Is Nothing AndAlso mStep.SqlStatements.Count > 0 Then
                plan.Steps.Add(mStep)
            End If
        Next

        Return plan
    End Function

    Public Shared Function GenerateForMissingTables(diff As SchemaDiff, expected As Dictionary(Of String, TableSchema)) As MigrationPlan
        Dim plan As MigrationPlan = Generate(diff)

        For Each tblName In diff.MissingTables
            If expected.ContainsKey(tblName) Then
                Dim mStep As New MigrationStep()
                mStep.TableName = tblName
                mStep.Description = "Create table " & tblName
                mStep.SqlStatements.Add(expected(tblName).ToCreateTableSql())
                plan.Steps.Insert(0, mStep)
            End If
        Next

        Return plan
    End Function

    Private Shared Function GenerateAlterTableSql(tableName As String, changes As List(Of ColumnChange)) As String
        If changes.Count = 0 Then Return Nothing

        Dim sb As New StringBuilder()
        sb.AppendLine("-- " & tableName & ": " & changes.Count.ToString() & " kolom berubah")
        sb.Append("ALTER TABLE `" & tableName & "`")

        Dim first As Boolean = True
        For Each ch In changes
            If Not first Then
                sb.AppendLine(",")
            End If
            first = False

            Select Case ch.Type
                Case ChangeType.[Add]
                    sb.Append("    ADD COLUMN " & ch.Expected.ToSql())
                Case ChangeType.Modify
                    sb.Append("    MODIFY COLUMN " & ch.Expected.ToSql())
                Case ChangeType.Drop
                    sb.Append("    DROP COLUMN `" & ch.ColumnName & "`")
            End Select
        Next

        sb.Append(";")
        Return sb.ToString()
    End Function

    Private Shared Function GenerateCreateIndexSql(tableName As String, idx As IndexSchema) As String
        If idx Is Nothing Then Return Nothing

        Dim prefix As String
        If idx.IsUnique Then
            prefix = "UNIQUE INDEX"
        ElseIf idx.IsFullText Then
            prefix = "FULLTEXT INDEX"
        Else
            prefix = "INDEX"
        End If
        Return "CREATE " & prefix & " IF NOT EXISTS `" & idx.Name & "` ON `" & tableName & "` (" & idx.ColumnList & ");"
    End Function

    Private Shared Function GenerateDropIndexSql(tableName As String, indexName As String) As String
        Return "DROP INDEX IF EXISTS `" & indexName & "` ON `" & tableName & "`;"
    End Function

    Public Shared Sub SaveToFolder(plan As MigrationPlan, outputDir As String)
        If Not Directory.Exists(outputDir) Then
            Directory.CreateDirectory(outputDir)
        End If

        For Each oldFile In Directory.GetFiles(outputDir, "*.sql")
            File.Delete(oldFile)
        Next

        Dim headerFile As String = Path.Combine(outputDir, "0000_header.sql")
        File.WriteAllText(headerFile, plan.ToHeaderSql())

        Dim seq As Integer = 1
        For Each mStep In plan.Steps
            Dim fileName As String = seq.ToString("D4") & "_" & mStep.TableName & ".sql"
            Dim filePath As String = Path.Combine(outputDir, fileName)

            Dim sb As New StringBuilder()
            sb.AppendLine("-- ============================================================")
            sb.AppendLine("-- Table: " & mStep.TableName)
            sb.AppendLine("-- " & mStep.Description)
            sb.AppendLine("-- Generated: " & plan.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss"))
            sb.AppendLine("-- ============================================================")
            sb.AppendLine()

            For Each sqlStr In mStep.SqlStatements
                sb.AppendLine(sqlStr)
                sb.AppendLine()
            Next

            File.WriteAllText(filePath, sb.ToString())
            seq += 1
        Next
    End Sub
End Class
