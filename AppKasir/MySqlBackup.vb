Imports System.IO
Imports System.IO.Compression

Public Class MySqlBackup
    Public Shared Function BackupDatabase(config As DatabaseConfiguration, typedata As String) As Boolean
        Try
            ' Tentukan lokasi file mysqldump
            Dim mysqldumpPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mysqldump.exe")

            ' Validasi file mysqldump
            If Not File.Exists(mysqldumpPath) Then
                Throw New FileNotFoundException("mysqldump.exe tidak ditemukan di folder aplikasi.")
            End If

            ' Tentukan folder backup berdasarkan tanggal
            Dim backupBaseFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup")
            Dim today As Date = Date.Now

            Dim namaBulan() As String = {
                "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                "Juli", "Agustus", "September", "Oktober", "November", "Desember"
            }

            Dim bulan As String = today.ToString("MM")
            Dim namaBulanIndonesia As String = namaBulan(today.Month - 1)
            Dim backupFolder As String = Path.Combine(backupBaseFolder, today.ToString("yyyy"), $"{bulan} - {namaBulanIndonesia}", today.ToString("dd"))

            If Not Directory.Exists(backupFolder) Then
                Directory.CreateDirectory(backupFolder)
            End If

            Dim backupFileName As String = $"{config.Database}_backup_{today:yyyyMMdd_HHmmss}.sql"
            Dim backupFile As String = Path.Combine(backupFolder, backupFileName)

            ' Perintah mysqldump
            Dim arguments As String = $"-h {config.Server} -P {config.Port} -u {config.User} --password={config.Password} {config.Database} --result-file=""{backupFile}"" --routines --events --triggers"

            Dim processInfo As New ProcessStartInfo() With {
                .FileName = mysqldumpPath,
                .Arguments = arguments,
                .RedirectStandardOutput = False,
                .RedirectStandardError = False,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            Using process As Process = Process.Start(processInfo)
                process.WaitForExit()

                If process.ExitCode = 0 Then
                    ' Backup berhasil
                    If typedata.ToUpper() = "SQL" Then
                        MessageBox.Show($"Backup SQL berhasil disimpan di: {backupFile}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        ' Kompres ke ZIP
                        Dim zipFilePath As String = Path.ChangeExtension(backupFile, ".zip")

                        Using archive As ZipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create)
                            archive.CreateEntryFromFile(backupFile, Path.GetFileName(backupFile))
                        End Using

                        ' Hapus file .sql setelah dikompres
                        File.Delete(backupFile)

                        MessageBox.Show($"Backup disimpan dalam bentuk ZIP: {zipFilePath}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                    Return True
                Else
                    Throw New Exception("Backup gagal. Periksa pengaturan koneksi dan lokasi output.")
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show($"Error saat backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function
End Class
