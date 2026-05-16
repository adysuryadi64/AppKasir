Imports System.IO
Imports System.IO.Compression

Public Class DatabaseRestore

    Public Shared Sub RestoreDatabase(config As DatabaseConfiguration, backupFilePath As String)
        Try
            ' Validasi input
            If config Is Nothing Then
                MessageBox.Show("Konfigurasi database tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If String.IsNullOrWhiteSpace(backupFilePath) OrElse Not File.Exists(backupFilePath) Then
                MessageBox.Show("File backup tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Lokasi mysql.exe
            Dim mysqlPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mysql.exe")
            If Not File.Exists(mysqlPath) Then
                MessageBox.Show("mysql.exe tidak ditemukan di folder aplikasi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Jika ZIP, extract ke temp folder dulu
            Dim sqlFilePath As String = backupFilePath
            Dim tempExtractDir As String = ""

            If Path.GetExtension(backupFilePath).ToLower() = ".zip" Then
                tempExtractDir = Path.Combine(Path.GetTempPath(), "db_restore_" & Guid.NewGuid().ToString("N"))
                Directory.CreateDirectory(tempExtractDir)
                ZipFile.ExtractToDirectory(backupFilePath, tempExtractDir)

                Dim sqlFiles As String() = Directory.GetFiles(tempExtractDir, "*.sql")
                If sqlFiles.Length = 0 Then
                    Directory.Delete(tempExtractDir, True)
                    MessageBox.Show("Tidak ada file .sql di dalam ZIP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                sqlFilePath = sqlFiles(0)
            End If

            Try
                Dim arguments As String = $"-h {config.Server} -P {config.Port} -u {config.User} --password={config.Password} ""{config.Database}"" < ""{sqlFilePath}"""

                Dim processInfo As New ProcessStartInfo() With {
                    .FileName = "cmd.exe",
                    .Arguments = $"/C """"{mysqlPath}"" {arguments}""",
                    .RedirectStandardOutput = True,
                    .RedirectStandardError = True,
                    .UseShellExecute = False,
                    .CreateNoWindow = True
                }

                Using process As Process = Process.Start(processInfo)
                    Dim errorMessage As String = process.StandardError.ReadToEnd()
                    process.WaitForExit()

                    If process.ExitCode = 0 Then
                        MessageBox.Show("Restore database berhasil.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Terjadi kesalahan saat restore: " & errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using

            Finally
                ' Hapus temp folder jika dari ZIP
                If Not String.IsNullOrEmpty(tempExtractDir) AndAlso Directory.Exists(tempExtractDir) Then
                    Directory.Delete(tempExtractDir, True)
                End If
            End Try

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
