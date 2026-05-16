Imports System.IO
Imports System.Text.Json

Public Class SettingDatabase

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
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
            DatabaseModule.CreateDefaultConfigurationFile()
            LoadConfiguration()
        End If
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
            Dim connectionString = $"Server={TxtServer.Text};Port={TxtPort.Text};Database={TxtDatabase.Text};User ID={TxtUsername.Text};Password={TxtPassword.Text};SslMode=None;charset=utf8mb4;"
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
        If TxtServer.Text = "" OrElse TxtUsername.Text = "" Then
            ShowValidationError("Isi Server dan Username terlebih dahulu.")
            Return
        End If

        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            openFileDialog.Title = "Pilih File SQL Database"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim backupFilePath As String = openFileDialog.FileName

                Dim namaDB As String = TxtDatabase.Text.Trim()
                If String.IsNullOrEmpty(namaDB) Then
                    ShowValidationError("Isi nama Database di kolom Database terlebih dahulu.")
                    TxtDatabase.Focus()
                    Return
                End If

                ' Nonaktifkan tombol dan tampilkan status proses
                BtnBuatDB.Enabled = False
                BtnSimpan.Enabled = False
                BtnCek.Enabled = False
                LblStatus.ForeColor = Color.DarkOrange
                LblStatus.Text = "Sedang membuat database '" & namaDB & "' ... mohon tunggu"
                Application.DoEvents()

                Try
                    Cursor = Cursors.WaitCursor

                    LblStatus.Text = "Menghubungkan ke server MySQL ..."
                    Application.DoEvents()

                    Dim connStr As String = $"Server={TxtServer.Text};Port={TxtPort.Text};User ID={TxtUsername.Text};Password={TxtPassword.Text};SslMode=None;charset=utf8mb4;"
                    Using tmpConn As New MySqlConnection(connStr)
                        tmpConn.Open()

                        ' Cek apakah database sudah ada
                        Dim dbSudahAda As Boolean = False
                        Using cmdCek As New MySqlCommand("SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @db", tmpConn)
                            cmdCek.Parameters.AddWithValue("@db", namaDB)
                            Dim hasil As Object = cmdCek.ExecuteScalar()
                            dbSudahAda = (hasil IsNot Nothing AndAlso hasil IsNot DBNull.Value)
                        End Using

                        If dbSudahAda Then
                            Dim jawab As DialogResult = MessageBox.Show(
                                $"Database '{namaDB}' sudah ada." & Environment.NewLine &
                                "Melanjutkan akan menimpa data yang ada." & Environment.NewLine & Environment.NewLine &
                                "Lanjutkan?",
                                "Database Sudah Ada", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            If jawab = DialogResult.No Then
                                LblStatus.Text = "Dibatalkan. Ganti nama database atau pilih database lain."
                                LblStatus.ForeColor = Color.Gray
                                Return
                            End If
                        End If

                        LblStatus.Text = "Membuat database '" & namaDB & "' ..."
                        Application.DoEvents()

                        Using cmd As New MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{namaDB}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", tmpConn)
                            cmd.ExecuteNonQuery()
                        End Using
                    End Using

                    LblStatus.Text = "Mengimpor struktur dan data dari file SQL ..."
                    Application.DoEvents()

                    Dim konfigurasi As New DatabaseConfiguration With {
                        .Server = TxtServer.Text,
                        .Port = TxtPort.Text,
                        .User = TxtUsername.Text,
                        .Password = TxtPassword.Text,
                        .Database = namaDB
                    }

                    DatabaseRestore.RestoreDatabase(konfigurasi, backupFilePath)

                    LblStatus.Text = "Database '" & namaDB & "' berhasil dibuat."
                    LblStatus.ForeColor = Color.Green

                Catch ex As Exception
                    LblStatus.Text = "Gagal: " & ex.Message
                    LblStatus.ForeColor = Color.Red
                    MessageBox.Show("Gagal membuat database: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    Cursor = Cursors.Default
                    BtnBuatDB.Enabled = True
                    BtnSimpan.Enabled = True
                    BtnCek.Enabled = True
                End Try
            End If
        End Using
    End Sub
End Class
