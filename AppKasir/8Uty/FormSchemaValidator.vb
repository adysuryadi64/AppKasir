Imports System.IO

Public Class FormSchemaValidator

    Private _schemaDir As String
    Private _registry As SchemaRegistry
    Private _actualSchemas As Dictionary(Of String, TableSchema)
    Private _diff As SchemaDiff
    Private _plan As MigrationPlan

    Private Sub FormSchemaValidator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        _schemaDir = FindSchemaDefFolder()

        If String.IsNullOrEmpty(_schemaDir) OrElse Not Directory.Exists(_schemaDir) Then
            MessageBox.Show("Folder SchemaDef tidak ditemukan.",
                            "Folder Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        TreeViewDiff.ShowPlusMinus = True
        TreeViewDiff.ShowLines = True
        TreeViewDiff.ShowRootLines = True
        TreeViewDiff.HideSelection = False
        TreeViewDiff.FullRowSelect = True

        LblStatus.Text = "SchemaDef: " & _schemaDir
    End Sub

    Private Function FindSchemaDefFolder() As String
        Dim baseDir As String = Application.StartupPath

        For i As Integer = 0 To 3
            Dim candidate As String = Path.Combine(baseDir, "Database", "SchemaDef")
            If Directory.Exists(candidate) Then Return candidate

            candidate = Path.Combine(baseDir, "AppKasir", "Database", "SchemaDef")
            If Directory.Exists(candidate) Then Return candidate

            baseDir = Path.GetDirectoryName(baseDir)
            If baseDir Is Nothing Then Exit For
        Next

        Return Nothing
    End Function

    Private Sub BtnCekSchema_Click(sender As Object, e As EventArgs) Handles BtnCekSchema.Click
        Cursor = Cursors.WaitCursor
        BtnCekSchema.Enabled = False
        TreeViewDiff.Nodes.Clear()
        TxtSqlPreview.Clear()
        LblSummary.Text = "Memproses..."
        Application.DoEvents()

        Try
            _registry = New SchemaRegistry()
            Dim loaded As Integer = _registry.LoadFromFolder(_schemaDir)
            LblStatus.Text = "Loaded " & loaded.ToString() & " tabel dari SchemaDef"

            DatabaseModule.EnsureConnectionReady()
            _actualSchemas = DbSchemaHelper.GetAllTableSchemas(DatabaseModule.conn)

            _diff = SchemaComparer.Compare(_registry.Tables, _actualSchemas)

            PopulateTreeView(_diff)

            LblSummary.Text = _diff.Summary
            LblStatus.Text = "Schema check selesai - " & _actualSchemas.Count.ToString() & " tabel di database"

            If Not _diff.HasDifferences Then
                MessageBox.Show("Database sudah sesuai dengan SchemaDef.",
                                "Skema Sesuai", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            LblStatus.Text = "Error: " & ex.Message
        Finally
            BtnCekSchema.Enabled = True
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub PopulateTreeView(diff As SchemaDiff)
        TreeViewDiff.Nodes.Clear()

        If diff.MissingTables.Count > 0 Then
            Dim grpNode As New TreeNode("TABEL BARU (" & diff.MissingTables.Count.ToString() & ")")
            grpNode.ForeColor = Color.FromArgb(0, 120, 215)
            grpNode.NodeFont = New Font(TreeViewDiff.Font, FontStyle.Bold)
            For Each tbl As String In diff.MissingTables
                Dim tblNode As New TreeNode("+ " & tbl)
                tblNode.Tag = "MISSING:" & tbl
                tblNode.ForeColor = Color.Green
                grpNode.Nodes.Add(tblNode)
            Next
            TreeViewDiff.Nodes.Add(grpNode)
        End If

        Dim colChanges = diff.ColumnChanges.GroupBy(Function(c) c.TableName)
        If colChanges.Any() Then
            Dim totalCol As Integer = diff.ColumnChanges.Count
            Dim grpNode As New TreeNode("PERUBAHAN KOLOM (" & totalCol.ToString() & " di " & colChanges.Count().ToString() & " tabel)")
            grpNode.ForeColor = Color.FromArgb(255, 140, 0)
            grpNode.NodeFont = New Font(TreeViewDiff.Font, FontStyle.Bold)

            For Each grp In colChanges
                Dim tblNode As New TreeNode(grp.Key & " (" & grp.Count().ToString() & " kolom)")
                tblNode.Tag = "TABLE:" & grp.Key
                For Each ch In grp
                    Dim colNode As New TreeNode(ch.Description)
                    colNode.Tag = "COL:" & ch.TableName & ":" & ch.ColumnName
                    Select Case ch.Type
                        Case ChangeType.[Add]
                            colNode.ForeColor = Color.Green
                        Case ChangeType.Modify
                            colNode.ForeColor = Color.FromArgb(255, 140, 0)
                        Case ChangeType.Drop
                            colNode.ForeColor = Color.Red
                    End Select
                    tblNode.Nodes.Add(colNode)
                Next
                grpNode.Nodes.Add(tblNode)
            Next
            TreeViewDiff.Nodes.Add(grpNode)
        End If

        Dim idxChanges = diff.IndexChanges.GroupBy(Function(c) c.TableName)
        If idxChanges.Any() Then
            Dim totalIdx As Integer = diff.IndexChanges.Count
            Dim grpNode As New TreeNode("PERUBAHAN INDEX (" & totalIdx.ToString() & " di " & idxChanges.Count().ToString() & " tabel)")
            grpNode.ForeColor = Color.FromArgb(128, 0, 128)
            grpNode.NodeFont = New Font(TreeViewDiff.Font, FontStyle.Bold)

            For Each grp In idxChanges
                Dim tblNode As New TreeNode(grp.Key & " (" & grp.Count().ToString() & " index)")
                tblNode.Tag = "TABLE:" & grp.Key
                For Each ch In grp
                    Dim idxNode As New TreeNode(ch.Description)
                    idxNode.Tag = "IDX:" & ch.TableName & ":" & ch.IndexName
                    Select Case ch.Type
                        Case ChangeType.[Add]
                            idxNode.ForeColor = Color.Green
                        Case ChangeType.Drop
                            idxNode.ForeColor = Color.Red
                    End Select
                    tblNode.Nodes.Add(idxNode)
                Next
                grpNode.Nodes.Add(tblNode)
            Next
            TreeViewDiff.Nodes.Add(grpNode)
        End If

        If diff.ExtraTables.Count > 0 Then
            Dim grpNode As New TreeNode("TABEL EKSTRA (" & diff.ExtraTables.Count.ToString() & ") - tidak dihapus")
            grpNode.ForeColor = Color.Gray
            grpNode.NodeFont = New Font(TreeViewDiff.Font, FontStyle.Bold)
            For Each tbl As String In diff.ExtraTables
                Dim tblNode As New TreeNode(tbl)
                tblNode.ForeColor = Color.Gray
                grpNode.Nodes.Add(tblNode)
            Next
            TreeViewDiff.Nodes.Add(grpNode)
        End If

        TreeViewDiff.ExpandAll()
    End Sub

    Private Sub BtnGenerateMigration_Click(sender As Object, e As EventArgs) Handles BtnGenerateMigration.Click
        If _diff Is Nothing OrElse Not _diff.HasDifferences Then
            MessageBox.Show("Jalankan 'Cek Skema' terlebih dahulu.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Cursor = Cursors.WaitCursor
        BtnGenerateMigration.Enabled = False
        Application.DoEvents()

        Try
            _plan = MigrationGenerator.GenerateForMissingTables(_diff, _registry.Tables)

            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine(_plan.ToHeaderSql())

            For Each mStep In _plan.Steps
                For Each stmt As String In mStep.SqlStatements
                    sb.AppendLine(stmt)
                    sb.AppendLine()
                Next
            Next

            TxtSqlPreview.Text = sb.ToString()
            TxtSqlPreview.SelectionStart = 0
            TxtSqlPreview.ScrollToCaret()

            LblSummary.Text = "Migration: " & _plan.Steps.Count.ToString() & " tabel, " & _plan.TotalStatements.ToString() & " statement"

            Dim genDir As String = Path.Combine(Path.GetDirectoryName(_schemaDir), "SchemaGenerated")
            MigrationGenerator.SaveToFolder(_plan, genDir)

            LblStatus.Text = "Migration tersimpan di: " & genDir

        Catch ex As Exception
            MessageBox.Show("Error generate migration: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            BtnGenerateMigration.Enabled = True
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BtnApplyMigration_Click(sender As Object, e As EventArgs) Handles BtnApplyMigration.Click
        If _plan Is Nothing OrElse _plan.TotalStatements = 0 Then
            MessageBox.Show("Generate migration terlebih dahulu.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim answer As DialogResult = MessageBox.Show(
            "Akan menjalankan " & _plan.TotalStatements.ToString() & " SQL statement ke database." & vbCrLf & vbCrLf &
            _plan.Summary & vbCrLf & vbCrLf &
            "Lanjutkan?",
            "Konfirmasi Apply Migration",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If answer <> DialogResult.Yes Then Return

        Cursor = Cursors.WaitCursor
        BtnApplyMigration.Enabled = False
        Application.DoEvents()

        Try
            DatabaseModule.EnsureConnectionReady()
            Dim results As List(Of MigrationResult) = MigrationExecutor.Execute(_plan, DatabaseModule.conn)

            Dim dbName As String = DatabaseModule.conn.Database
            MigrationExecutor.WriteLog(results, dbName)

            Dim summary As String = MigrationExecutor.GetSummaryText(results)
            LblSummary.Text = summary

            Dim failedCount As Integer = 0
            For Each r In results
                If r.Status = MigrationStatus.Failed Then failedCount += 1
            Next
            If failedCount > 0 Then
                MessageBox.Show("Migration selesai dengan error." & vbCrLf & summary,
                                "Selesai dengan Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show("Migration berhasil!" & vbCrLf & summary,
                                "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            BtnCekSchema.PerformClick()

        Catch ex As Exception
            MessageBox.Show("Error apply migration: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            BtnApplyMigration.Enabled = True
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub TreeViewDiff_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeViewDiff.AfterSelect
        If e.Node Is Nothing Then Return

        Dim tag As String = If(e.Node.Tag, "").ToString()

        If tag.StartsWith("MISSING:") Then
            Dim tblName As String = tag.Substring("MISSING:".Length)
            If _registry IsNot Nothing AndAlso _registry.Tables.ContainsKey(tblName) Then
                TxtSqlPreview.Text = _registry.Tables(tblName).ToCreateTableSql()
            End If

        ElseIf tag.StartsWith("COL:") Then
            Dim parts As String() = tag.Split(":"c)
            If parts.Length >= 3 Then
                Dim ch As ColumnChange = Nothing
                For Each c As ColumnChange In _diff.ColumnChanges
                    If String.Equals(c.TableName, parts(1), StringComparison.OrdinalIgnoreCase) AndAlso
                       String.Equals(c.ColumnName, parts(2), StringComparison.OrdinalIgnoreCase) Then
                        ch = c
                        Exit For
                    End If
                Next
                If ch IsNot Nothing Then
                    Dim sb As New System.Text.StringBuilder()
                    sb.AppendLine("Tabel: " & ch.TableName)
                    sb.AppendLine("Kolom: " & ch.ColumnName)
                    sb.AppendLine("Perubahan: " & ch.Type.ToString())
                    sb.AppendLine()
                    If ch.Expected IsNot Nothing Then
                        sb.AppendLine("Expected (SchemaDef):")
                        sb.AppendLine("  Type: " & ch.Expected.DataTypeString())
                        sb.AppendLine("  Nullable: " & ch.Expected.IsNullable.ToString())
                        sb.AppendLine("  Default: " & If(ch.Expected.DefaultValue, "NULL"))
                    End If
                    If ch.Actual IsNot Nothing Then
                        sb.AppendLine()
                        sb.AppendLine("Actual (Database):")
                        sb.AppendLine("  Type: " & ch.Actual.DataTypeString())
                        sb.AppendLine("  Nullable: " & ch.Actual.IsNullable.ToString())
                        sb.AppendLine("  Default: " & If(ch.Actual.DefaultValue, "NULL"))
                    End If
                    TxtSqlPreview.Text = sb.ToString()
                End If
            End If
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class
