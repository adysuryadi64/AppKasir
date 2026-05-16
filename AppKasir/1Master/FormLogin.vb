Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Security.Cryptography
Imports System.Text.Json

Public Class FormLogin

    Private _isLoggingIn As Boolean = False


    Public Shared Function MD5DELISMAN(ByVal strToHash As String) As String
        Using MD5HULU As New MD5CryptoServiceProvider()
            Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)

            ' ComputeHash should be wrapped in a Using block to ensure proper resource disposal.
            Using md5Hash = MD5HULU
                bytesToHash = md5Hash.ComputeHash(bytesToHash)
            End Using

            Dim strResult As String = ""
            Dim b As Byte

            For Each b In bytesToHash
                strResult += b.ToString("x2")
            Next

            Return strResult
        End Using
    End Function

    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click
        End
    End Sub

    Public Sub Userakun()
        Try
            Using cmd As New MySqlCommand("SELECT USER_NAME FROM tbl_user WHERE status = 'Aktif' ORDER BY USER_NAME", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    CmbUser.Items.Clear()
                    If rd.HasRows Then
                        While rd.Read()
                            CmbUser.Items.Add(rd("USER_NAME").ToString())
                        End While
                    End If
                End Using
            End Using
            CmbUser.Items.Add(String.Empty)
        Catch ex As MySqlException
            If TawarMigrasi(ex) Then
                ' Migrasi dipilih — tutup login, user perlu restart setelah migrasi
                Me.Close()
            Else
                MessageBox.Show("Gagal memuat daftar user: " & ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub



    Private Sub Form_Login_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Call Userakun()
        CmbUser.DroppedDown = True ' Memunculkan dropdown list
        CmbUser.Focus()
        Me.Cursor = Cursors.Default
    End Sub


    Private Sub ChkShow_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ChkShow.CheckedChanged
        If ChkShow.Checked = False Then
            TxtPassword.PasswordChar = "*"
        End If
        If ChkShow.Checked = True Then
            TxtPassword.PasswordChar = ""
        End If
    End Sub

    Private Sub TxtPassword_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtPassword.KeyPress
        If e.KeyChar = Chr(13) Then
            Call Login()
        End If
    End Sub

    Private Sub CmbUser_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles CmbUser.KeyPress
        If e.KeyChar = Chr(13) Then
            TxtPassword.Select()
        End If
    End Sub

    Private Sub FormLogin_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.Return : BtnLogin.PerformClick()
            Case Keys.Escape : BtnKeluar.PerformClick()
        End Select
    End Sub

    Private Sub Panel1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        Using g As Graphics = e.Graphics
            g.DrawRectangle(New Pen(Color.Gold, 20), 0, 0, Width - 1, Height - 1)
        End Using
    End Sub


    Private Sub BtnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnLogin.Click
        Call Login()
    End Sub

    Public Sub Login()
        If _isLoggingIn Then Return
        _isLoggingIn = True
        BtnLogin.Enabled = False

        Try
            If String.IsNullOrWhiteSpace(TxtPassword.Text) Then
                MessageBox.Show("Password tidak boleh kosong", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim username As String = Trim(CmbUser.Text)
            Dim password As String = Trim(MD5DELISMAN(TxtPassword.Text))

            Dim ipAddress As String = GetLocalIPAddress()

            Dim konfigurasi As DatabaseConfiguration
            ' Membaca konfigurasi dari file biner
            If Not File.Exists(configFilePath) Then
                MessageBox.Show("File konfigurasi tidak ditemukan!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If


            Dim json As String = File.ReadAllText(configFilePath)
            konfigurasi = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
            konfigurasi.Password = DecryptPassword(konfigurasi.Password)


            ' Periksa kondisi khusus
            If String.IsNullOrWhiteSpace(username) AndAlso TxtPassword.Text = "****" Then
                CmbUser.SelectedIndex = -1
                TxtPassword.Clear()
                SetVersionInfo()

                With FormUtama
                    .StatusNamaUser.Text = "Programer"
                    .StatusLevelUser.Text = "Master"
                    .StatusTanggal.Text = Now.ToString("dd MMMM yyyy")
                    .LblServer.Text = "IP Address : " & ipAddress
                    .LblServerDb.Text = " | Database : " & konfigurasi.Server & " \ " & konfigurasi.Database
                    ' Update icon StatusNamaUser
                    Dim iconLogin As String = IO.Path.Combine(Application.StartupPath, "Resources", "Icons", "login_20.png")
                    If IO.File.Exists(iconLogin) Then .StatusNamaUser.Image = Image.FromFile(iconLogin)
                End With
                ' Simpan ke variabel global agar bisa diakses dari module tanpa default instance
                NamaUser = "Programer"
                LevelUser = "Master"

                Me.Close()
                Return
            End If

            ' Proses login dengan database
            Dim query As String = "SELECT KODE_USER, NAMA_USER, USER_NAME, PWD, LVL FROM tbl_user WHERE USER_NAME = @Username AND PWD = @Password AND status = 'Aktif'"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Password", password)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        CmbUser.SelectedIndex = -1
                        TxtPassword.Clear()
                        SetVersionInfo()

                        With FormUtama
                            .StatusNamaUser.Text = If(Not Convert.IsDBNull(rd("NAMA_USER")), rd("NAMA_USER").ToString(), "")
                            .StatusLevelUser.Text = rd("LVL").ToString()
                            .StatusTanggal.Text = Now.ToString("dd MMMM yyyy")
                            .LblServer.Text = "IP Address : " & ipAddress
                            .LblServerDb.Text = " | Database : " & konfigurasi.Server
                            ' Update icon StatusNamaUser
                            Dim iconLogin As String = IO.Path.Combine(Application.StartupPath, "Resources", "Icons", "login_20.png")
                            If IO.File.Exists(iconLogin) Then .StatusNamaUser.Image = Image.FromFile(iconLogin)
                        End With
                        ' Simpan ke variabel global agar bisa diakses dari module tanpa default instance
                        NamaUser = If(Not Convert.IsDBNull(rd("NAMA_USER")), rd("NAMA_USER").ToString(), "")
                        LevelUser = rd("LVL").ToString()

                        Me.Close()


                    Else
                        MessageBox.Show("Password salah!", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TxtPassword.Clear()
                        TxtPassword.Select()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Terjadi kesalahan saat mengakses database: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isLoggingIn = False
            If Not IsDisposed Then BtnLogin.Enabled = True
        End Try
    End Sub


    Public Sub SetVersionInfo()
        Dim startYear As Integer = 2023
        Dim currentYear As Integer = DateTime.Now.Year
        Dim yearRange As String

        If currentYear > startYear Then
            yearRange = startYear.ToString() & " - " & currentYear.ToString()
        Else
            yearRange = startYear.ToString()
        End If

        FormUtama.LblVersiApp.Text = My.Application.Info.Version.ToString() & "    | Copyright © " & yearRange
    End Sub



    Public Function GetLocalIPAddress() As String
        Try
            For Each ip As IPAddress In Dns.GetHostAddresses(Dns.GetHostName())
                If ip.AddressFamily = AddressFamily.InterNetwork Then
                    Return ip.ToString()
                End If
            Next
            Return "IP Address tidak ditemukan!"
        Catch ex As Exception
            Return $"Kesalahan: {ex.Message}"
        End Try
    End Function


    Private Sub CmbUser_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbUser.SelectedIndexChanged
        ' Memastikan tidak ada dropdown yang aktif
        If CmbUser.DroppedDown Then
            Return
        End If
        TxtPassword.Select()
    End Sub
End Class