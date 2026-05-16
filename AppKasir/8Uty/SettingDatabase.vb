Imports System.IO
Imports System.Text.Json

Public Class SettingDatabase

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        LblStatus.Text = "Silahkan tes koneksi dulu sebelum menyimpan ... !!!"
        LoadConfiguration()
    End Sub

    Private Sub LoadConfiguration()
        If File.Exists(configFilePath) Then
            Try
                Dim json As String = File.ReadAllText(configFilePath)
                Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)

                TxtServer.Text = konfigurasi.Server
                TxtPort.Text = konfigurasi.Port
                TxtUsername.Text = konfigurasi.User
                TxtPassword.Text = DecryptPassword(konfigurasi.Password)
                TxtDatabase.Text = konfigurasi.Database
            Catch ex As Exception
                MessageBox.Show("Gagal memuat konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            CreateDefaultConfigurationFile()
            LoadConfiguration()
        End If
    End Sub

    Private Sub CreateDefaultConfigurationFile()
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

    Private Sub SaveConfiguration()
        Dim konfigurasi As New DatabaseConfiguration With {
            .Server = TxtServer.Text,
            .Port = TxtPort.Text,
            .User = TxtUsername.Text,
            .Password = EncryptPassword(TxtPassword.Text),
            .Database = TxtDatabase.Text
        }

        Dim json As String = JsonSerializer.Serialize(konfigurasi, New JsonSerializerOptions With {.WriteIndented = True})
        File.WriteAllText(configFilePath, json)

        MessageBox.Show("Konfigurasi disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Function ValidateCek() As Boolean
        If TxtServer.Text = "" Then
            ShowValidationError("Kolom Server tidak boleh kosong.") : TxtServer.Focus() : Return False
        ElseIf TxtPort.Text = "" Then
            ShowValidationError("Kolom Port tidak boleh kosong.") : TxtPort.Focus() : Return False
        ElseIf Not Integer.TryParse(TxtPort.Text, Nothing) Then
            ShowValidationError("Kolom Port harus berupa angka.") : TxtPort.Focus() : Return False
        ElseIf TxtUsername.Text = "" Then
            ShowValidationError("Kolom Username tidak boleh kosong.") : TxtUsername.Focus() : Return False
        ElseIf TxtPassword.Text = "" Then
            ShowValidationError("Kolom Password tidak boleh kosong.") : TxtPassword.Focus() : Return False
        End If
        Return True
    End Function

    Private Sub BtnCekDatabase_Click(sender As Object, e As EventArgs) Handles BtnCekDatabase.Click
        If ValidateCek() Then
            ListDatabase.Items.Clear()
            Dim connectionString = $"Server={TxtServer.Text};Port={TxtPort.Text};User ID={TxtUsername.Text};Password={TxtPassword.Text};SslMode=None;charset=utf8mb4;"

            Try
                Using conn As New MySqlConnection(connectionString)
                    conn.Open()
                    Using cmd As New MySqlCommand("SHOW DATABASES;", conn)
                        Using reader As MySqlDataReader = cmd.ExecuteReader()
                            While reader.Read()
                                ListDatabase.Items.Add(reader.GetString(0))
                            End While
                        End Using
                    End Using
                    If ListDatabase.Items.Count > 0 Then
                        ListDatabase.Focus()
                    Else
                        MessageBox.Show("Tidak ada database pada IP ini.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub ListDatabase_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListDatabase.MouseDoubleClick
        TxtDatabase.Text = ListDatabase.SelectedItem.ToString()
    End Sub

    Private Sub ListDatabase_KeyDown(sender As Object, e As KeyEventArgs) Handles ListDatabase.KeyDown
        If e.KeyCode = Keys.Enter AndAlso ListDatabase.SelectedItem IsNot Nothing Then
            TxtDatabase.Text = ListDatabase.SelectedItem.ToString()
        End If
    End Sub

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        If ValidateInputs() Then
            SaveConfiguration()
            CloseConnection()
            OpenConnection()
            Me.Close()
            FormUtama.Terkunci()
            FormLogin.BringToFront()
            FormLogin.ShowDialog()
            FormMasuk.BringToFront()
            FormMasuk.ShowDialog()
            With FormLoading
                .Label1.Text = "Selamat datang! Aplikasi saat ini dalam proses inisialisasi dan menunggu konfigurasi data"
                .BringToFront()
                .Show()
                .MulaiLoading()
            End With
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        If TxtServer.Text = "" Then
            ShowValidationError("Kolom Server tidak boleh kosong.") : TxtServer.Focus() : Return False
        ElseIf TxtPort.Text = "" Then
            ShowValidationError("Kolom Port tidak boleh kosong.") : TxtPort.Focus() : Return False
        ElseIf Not Integer.TryParse(TxtPort.Text, Nothing) Then
            ShowValidationError("Kolom Port harus berupa angka.") : TxtPort.Focus() : Return False
        ElseIf TxtUsername.Text = "" Then
            ShowValidationError("Kolom Username tidak boleh kosong.") : TxtUsername.Focus() : Return False
        ElseIf TxtPassword.Text = "" Then
            ShowValidationError("Kolom Password tidak boleh kosong.") : TxtPassword.Focus() : Return False
        ElseIf TxtDatabase.Text = "" Then
            ShowValidationError("Kolom Database tidak boleh kosong.") : TxtDatabase.Focus() : Return False
        End If
        Return True
    End Function

    Private Sub ShowValidationError(errorMessage As String)
        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub BtnCek_Click(sender As Object, e As EventArgs) Handles BtnCek.Click
        LblStatus.Text = "Sedang proses ... !!!"
        LblStatus.ForeColor = Color.Black
        LblStatus.BackColor = Color.Transparent

        Try
            Dim connectionString = $"Server={TxtServer.Text};Port={TxtPort.Text};Database={TxtDatabase.Text};User ID={TxtUsername.Text};Password={TxtPassword.Text};SslMode=None;"
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                If conn.State = ConnectionState.Open Then
                    LblStatus.Text = "Koneksi berhasil"
                    LblStatus.ForeColor = Color.Green
                Else
                    LblStatus.Text = "Koneksi gagal"
                    LblStatus.ForeColor = Color.Red
                End If
            End Using
        Catch ex As Exception
            LblStatus.Text = "Koneksi gagal: " & ex.Message
            LblStatus.ForeColor = Color.Red
        End Try
    End Sub

    Private Sub SettingDatabase_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8 : BtnSimpan.PerformClick()
            Case Keys.F5 : BtnCek.PerformClick()
            Case Keys.Escape : Me.Close()
        End Select
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub BtnBuatDB_Click(sender As Object, e As EventArgs) Handles BtnBuatDB.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            openFileDialog.Title = "Pilih File Backup"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim backupFilePath As String = openFileDialog.FileName

                ' Deserialisasi konfigurasi dari file biner
                Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
                    Dim json As String = File.ReadAllText(configFilePath)
                    Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
                    konfigurasi.Password = DecryptPassword(konfigurasi.Password)

                    Cursor = Cursors.WaitCursor
                    ' Panggil metode restore
                    DatabaseRestore.RestoreDatabase(konfigurasi, backupFilePath)
                    Cursor = Cursors.Default
                End Using
            End If
        End Using
    End Sub
End Class
