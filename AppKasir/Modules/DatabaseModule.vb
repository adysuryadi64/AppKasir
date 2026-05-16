Imports System.IO
Imports System.Text.Json

Public Module DatabaseModule
    Public conn As MySqlConnection
    Public koneksiDatabase As Boolean
    Public cultureIndonesia As New Globalization.CultureInfo("id-ID")

    Public ReadOnly configFilePath As String = Path.Combine(Application.StartupPath, "database.json")

    Private ReadOnly _connectionLock As New Object()

    ' ── Connection string cache ──────────────────────────────────────
    Public _connectionString As String = ""

    ''' <summary>
    ''' Buka koneksi baru dari file konfigurasi.
    ''' Dipanggil saat startup atau saat koneksi perlu dibuat ulang.
    ''' </summary>
    Public Function OpenConnection(Optional useSSL As Boolean = False) As Boolean
        Try
            SyncLock _connectionLock
                ' Sudah terbuka dan masih hidup — tidak perlu buka lagi
                If IsConnectionAlive() Then Return True

                If Not File.Exists(configFilePath) Then
                    CreateDefaultConfigurationFile()
                End If

                Dim json As String = File.ReadAllText(configFilePath)
                Dim cfg As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
                cfg.Password = DecryptPassword(cfg.Password)

                Dim server As String = cfg.Server
                Dim port As Integer = If(Integer.TryParse(cfg.Port, Nothing), CInt(cfg.Port), 3306)
                Dim database As String = cfg.Database
                Dim user As String = cfg.User
                Dim password As String = cfg.Password
                Dim sslMode As String = If(useSSL, "Preferred", "None")

                ' Cek server dulu tanpa database
                Dim csNoDB As String = $"Server={server};Port={port};User ID={user};Password={password};SslMode={sslMode};charset=utf8mb4;"
                Using testConn As New MySqlConnection(csNoDB)
                    testConn.Open()

                    ' 1. Cek apakah database ada
                    Using cmd As New MySqlCommand("SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @db", testConn)
                        cmd.Parameters.AddWithValue("@db", database)
                        Dim dbExists As Object = cmd.ExecuteScalar()
                        If dbExists Is Nothing OrElse dbExists Is DBNull.Value Then
                            CloseConnection()
                            SettingDatabase.ShowDialog()
                            Return False
                        End If
                    End Using

                    ' 2. Cek collation database — jika bukan utf8mb4_unicode_ci, migrasi otomatis
                    Using cmd As New MySqlCommand(
                        "SELECT DEFAULT_COLLATION_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @db", testConn)
                        cmd.Parameters.AddWithValue("@db", database)
                        Dim collation As String = If(cmd.ExecuteScalar()?.ToString(), "")
                        If collation <> "utf8mb4_unicode_ci" Then
                            Try
                                Using cmdAlter As New MySqlCommand(
                                    $"ALTER DATABASE `{database}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci", testConn)
                                    cmdAlter.ExecuteNonQuery()
                                End Using
                            Catch exCollation As Exception
                                ' Collation tidak bisa diubah otomatis — lanjutkan saja, tidak fatal
                                ' User bisa jalankan migrasi manual via FormMigrasiDB
                            End Try
                        End If
                    End Using
                End Using

                ' Connection string dengan parameter optimal untuk desktop app:
                ' - Default Command Timeout=0  : tidak ada batas waktu query
                ' - Connection Timeout=10      : batas waktu saat membuka koneksi baru
                ' - Pooling=false              : satu koneksi global, tidak pakai pool
                '   (pool cocok untuk web/multi-thread; desktop app lebih stabil tanpa pool)
                ' - Allow Zero Datetime=True   : tangani nilai datetime 0000-00-00 dari MySQL
                ' - Convert Zero Datetime=True : konversi ke DateTime.MinValue otomatis
                ' - CharSet=utf8mb4            : support emoji dan karakter unicode penuh
                _connectionString = $"Server={server};Port={port};Database={database};" &
                                    $"User ID={user};Password={password};SslMode={sslMode};" &
                                    $"charset=utf8mb4;Default Command Timeout=0;" &
                                    $"Connection Timeout=10;Pooling=false;" &
                                    $"Allow Zero Datetime=True;Convert Zero Datetime=True;" &
                                    $"Allow User Variables=True;"

                conn = New MySqlConnection(_connectionString)
                conn.Open()

                If conn.State = ConnectionState.Open Then
                    Return True
                Else
                    HandleConnectionError("Koneksi gagal.")
                    Return False
                End If
            End SyncLock

        Catch ex As MySqlException When ex.Number = 1042
            HandleConnectionError("Koneksi ke server gagal. Pastikan server dapat dijangkau.")
            Return False
        Catch ex As MySqlException When ex.Number = 1045
            HandleConnectionError("User atau password salah.")
            Return False
        Catch ex As MySqlException When ex.Number = 1049
            HandleConnectionError(Nothing)
            Return False
        Catch ex As Exception
            HandleConnectionError(ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Cek apakah koneksi benar-benar masih hidup.
    ''' Tidak pakai Ping() karena bisa throw exception — cukup cek State.
    ''' Jika State=Open tapi server sudah disconnect, query berikutnya akan
    ''' ditangani oleh EnsureConnectionReady via reconnect.
    ''' </summary>
    Private Function IsConnectionAlive() As Boolean
        Return conn IsNot Nothing AndAlso conn.State = ConnectionState.Open
    End Function

    ''' <summary>
    ''' Pastikan koneksi siap sebelum query.
    ''' Jika koneksi mati (server restart, idle timeout, jaringan putus),
    ''' otomatis buat koneksi baru dari connection string yang sudah di-cache.
    ''' Pattern ini menangani "MySQL server has gone away" secara transparan.
    ''' </summary>
    Public Sub EnsureConnectionReady()
        SyncLock _connectionLock
            If IsConnectionAlive() Then Exit Sub

            ' Koneksi mati — coba reconnect
            Try
                ' Dispose koneksi lama dulu
                If conn IsNot Nothing Then
                    Try
                        conn.Dispose()
                    Catch
                    End Try
                    conn = Nothing
                End If

                ' Jika sudah punya connection string dari sesi sebelumnya, pakai langsung
                If Not String.IsNullOrEmpty(_connectionString) Then
                    conn = New MySqlConnection(_connectionString)
                    conn.Open()
                    If conn.State = ConnectionState.Open Then Exit Sub
                End If

                ' Fallback: buka ulang dari file konfigurasi
                OpenConnection()

            Catch ex As Exception
                ' Reconnect gagal — buka dialog setting
                HandleConnectionError("Koneksi terputus: " & ex.Message)
            End Try
        End SyncLock
    End Sub

    Private Sub HandleConnectionError(ByVal errorMessage As String)
        If Not String.IsNullOrEmpty(errorMessage) Then
            MessageBox.Show($"Terjadi kesalahan koneksi: {errorMessage}",
                            "Gagal Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        CloseConnection()

        For Each f As Form In Application.OpenForms
            If TypeOf f Is SettingDatabase Then Return
        Next

        SettingDatabase.ShowDialog()
    End Sub

    Public Sub CloseConnection()
        SyncLock _connectionLock
            If conn IsNot Nothing Then
                Try
                    If conn.State = ConnectionState.Open Then conn.Close()
                    conn.Dispose()
                Catch
                End Try
                conn = Nothing
            End If
        End SyncLock
    End Sub

    ' Alias lama — tetap berfungsi
    Public Sub FlushAllReaders()
    End Sub

    Public Sub CreateDefaultConfigurationFile()
        Dim defaultConfig As New DatabaseConfiguration With {
            .Server = "localhost",
            .Port = "3306",
            .User = "root",
            .Password = EncryptPassword("12345678"),
            .Database = "databaseKasirLancar"
        }
        Dim json As String = JsonSerializer.Serialize(defaultConfig, New JsonSerializerOptions With {.WriteIndented = True})
        File.WriteAllText(configFilePath, json)
    End Sub

    <Serializable()>
    Public Class DatabaseConfiguration
        Public Property Server As String
        Public Property Port As String
        Public Property User As String
        Public Property Password As String
        Public Property Database As String
    End Class
End Module
