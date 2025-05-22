Imports System.IO

Public Class DatabaseRestore

    ' Metode untuk melakukan restore database
    Public Shared Sub RestoreDatabase(config As DatabaseConfiguration, backupFilePath As String)
        Try

            ' Validasi input
            If config Is Nothing Then
                MessageBox.Show("Konfigurasi database tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Throw New ArgumentNullException(NameOf(config))
            End If
            If String.IsNullOrWhiteSpace(backupFilePath) Then
                MessageBox.Show("Path file backup tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Throw New ArgumentException("Path file backup tidak valid.", NameOf(backupFilePath))
            End If
            If Not File.Exists(backupFilePath) Then
                MessageBox.Show("File backup tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Throw New FileNotFoundException("File backup tidak ditemukan.")
            End If

            ' Lokasi mysql.exe
            Dim mysqlPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mysql.exe")
            If Not File.Exists(mysqlPath) Then
                MessageBox.Show("mysql.exe tidak ditemukan di folder aplikasi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Throw New FileNotFoundException("mysql.exe tidak ditemukan di folder aplikasi.")
            End If

            ' Perintah untuk restore
            Dim arguments As String = $"-h {config.Server} -P {config.Port} -u {config.User} --password={config.Password} ""{config.Database}"" < ""{backupFilePath}"""

            ' Konfigurasi proses
            Dim processInfo As New ProcessStartInfo() With {
                .FileName = "cmd.exe",
                .Arguments = $"/C """"{mysqlPath}"" {arguments}""",
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            ' Eksekusi proses
            Using process As Process = Process.Start(processInfo)
                Dim output As String = process.StandardOutput.ReadToEnd()
                Dim errorMessage As String = process.StandardError.ReadToEnd()
                process.WaitForExit()

                ' Periksa hasil eksekusi
                If process.ExitCode = 0 Then
                    ' Tampilkan pesan sukses
                    MessageBox.Show("Restore database berhasil.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    ' Tampilkan pesan error
                    MessageBox.Show("Terjadi kesalahan saat melakukan restore: " & errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using

        Catch ex As Exception
            ' Laporkan error
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class

