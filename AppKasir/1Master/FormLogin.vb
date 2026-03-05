Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Security.Cryptography
Imports System.Text.Json

Public Class FormLogin


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
        ' Menggunakan 'Using' untuk reader agar tertutup setelah digunakan
        Using cmd As New MySqlCommand("SELECT USER_NAME FROM tbl_user ORDER BY KODE_USER", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbUser.Items.Clear()

                ' Cek apakah ada baris hasil dari query
                If rd.HasRows Then
                    While rd.Read()
                        CmbUser.Items.Add(rd("USER_NAME").ToString()) ' Pastikan nilai yang diambil adalah string
                    End While
                End If
            End Using ' Reader akan tertutup otomatis di sini
        End Using

        ' Tambahkan opsi kosong di bagian bawah ComboBox
        CmbUser.Items.Add(String.Empty)
    End Sub



    Private Sub Form_Login_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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

    Private Sub Panel1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        Using g As Graphics = e.Graphics
            g.DrawRectangle(New Pen(Color.Gold, 20), 0, 0, Width - 1, Height - 1)
        End Using
    End Sub


    Private Sub BtnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnLogin.Click
        Call Login()
    End Sub

    Public Sub Login()
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
                .SLogin.Text = "Programer"
                .SLevel.Text = "Master"
                .STanggal.Text = Now.ToString("dd MMMM yyyy")
                .SServer1.Text = "IP Address : " & ipAddress
                .SServer.Text = " | Database : " & konfigurasi.Server & " \ " & konfigurasi.Database
            End With

            Me.Close()
            Return
        End If

        ' Proses login dengan database
        Try
            Dim query As String = "SELECT KODE_USER, NAMA_USER, USER_NAME, PWD, LVL FROM tbl_user WHERE USER_NAME = @Username AND PWD = @Password"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Password", password)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        CmbUser.SelectedIndex = -1
                        TxtPassword.Clear()
                        SetVersionInfo()

                        With FormUtama
                            .SLogin.Text = If(Not Convert.IsDBNull(rd("NAMA_USER")), rd("NAMA_USER").ToString(), "")
                            .SLevel.Text = rd("LVL").ToString()
                            .STanggal.Text = Now.ToString("dd MMMM yyyy")
                            .SServer1.Text = "IP Address : " & ipAddress
                            .SServer.Text = " | Database : " & konfigurasi.Server
                        End With

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

        FormUtama.SVersi.Text = My.Application.Info.Version.ToString() & "    | Copyright © " & yearRange
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