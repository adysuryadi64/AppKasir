Imports System.IO
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Class MigrationExecutor

    Public Shared Function Execute(plan As MigrationPlan, conn As MySqlConnection) As List(Of MigrationResult)
        Dim results As New List(Of MigrationResult)
        Dim seq As Integer = 0

        For Each mStep In plan.Steps
            For Each sqlStr In mStep.SqlStatements
                seq += 1
                Dim trimmed = sqlStr.Trim()
                If String.IsNullOrWhiteSpace(trimmed) Then Continue For

                Dim result As New MigrationResult() With {
                    .SequenceNumber = seq,
                    .TableName = mStep.TableName,
                    .SqlStatement = trimmed,
                    .ExecutedAt = DateTime.Now
                }

                Try
                    Using cmd As New MySqlCommand(trimmed, conn)
                        cmd.ExecuteNonQuery()
                    End Using
                    result.Status = MigrationStatus.Success
                Catch ex As MySqlException
                    If ex.Number = 1060 OrElse ex.Number = 1061 OrElse ex.Number = 1091 OrElse
                       ex.Number = 1050 OrElse ex.Number = 1022 OrElse ex.Number = 1824 Then
                        result.Status = MigrationStatus.Skipped
                        result.ErrorMessage = "[" & ex.Number & "] " & ex.Message
                    Else
                        result.Status = MigrationStatus.Failed
                        result.ErrorMessage = "[" & ex.Number & "] " & ex.Message
                        results.Add(result)
                        Return results
                    End If
                Catch ex As Exception
                    result.Status = MigrationStatus.Failed
                    result.ErrorMessage = ex.Message
                    results.Add(result)
                    Return results
                End Try

                results.Add(result)
            Next
        Next

        Return results
    End Function

    Public Shared Sub WriteLog(results As List(Of MigrationResult), databaseName As String)
        Dim logDir As String = Application.StartupPath
        Dim logFile As String = Path.Combine(logDir, "migration_log_" & DateTime.Now.ToString("yyyyMMdd") & ".log")
        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("Migration Run: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        sb.AppendLine("Database: " & databaseName)
        sb.AppendLine("Statements: " & results.Count.ToString())
        sb.AppendLine("---------------------------------------------------")

        For Each r In results
            Dim st As String = r.Status.ToString().ToUpper()
            sb.AppendLine("#" & r.SequenceNumber.ToString("D4") & " [" & st & "] " & r.TableName)
            Dim sqlPreview As String = r.SqlStatement
            If sqlPreview.Length > 120 Then sqlPreview = sqlPreview.Substring(0, 120) & "..."
            sb.AppendLine("  SQL: " & sqlPreview)
            If r.ErrorMessage IsNot Nothing Then
                sb.AppendLine("  ERROR: " & r.ErrorMessage)
            End If
        Next

        Dim sc As Integer = 0
        Dim sk As Integer = 0
        Dim fa As Integer = 0
        For Each r In results
            If r.Status = MigrationStatus.Success Then sc += 1
            If r.Status = MigrationStatus.Skipped Then sk += 1
            If r.Status = MigrationStatus.Failed Then fa += 1
        Next
        sb.AppendLine("SUMMARY: Success=" & sc.ToString() & " Skipped=" & sk.ToString() & " Failed=" & fa.ToString())
        sb.AppendLine()

        File.AppendAllText(logFile, sb.ToString(), System.Text.Encoding.UTF8)
    End Sub

    Public Shared Function GetSummaryText(results As List(Of MigrationResult)) As String
        Dim sc As Integer = 0
        Dim sk As Integer = 0
        Dim fa As Integer = 0
        For Each r In results
            If r.Status = MigrationStatus.Success Then sc += 1
            If r.Status = MigrationStatus.Skipped Then sk += 1
            If r.Status = MigrationStatus.Failed Then fa += 1
        Next
        Return "Berhasil: " & sc.ToString() & " | Dilewati: " & sk.ToString() & " | Gagal: " & fa.ToString()
    End Function
End Class
