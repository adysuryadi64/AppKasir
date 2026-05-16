Imports System.IO

Public Class ACTIVATION_FORM
    Private Const ConfigFilePath As String = "config.bin"
    Private ReadOnly LicenseDuration As TimeSpan

    Public Sub New()
        InitializeComponent()
        LicenseDuration = TimeSpan.FromDays(90) ' Durasi 3 bulan (90 hari)
    End Sub

    Private Function GetInstallationDate() As DateTime
        Dim installationDate As DateTime

        If File.Exists(ConfigFilePath) Then
            Using reader As New BinaryReader(File.Open(ConfigFilePath, FileMode.Open))
                installationDate = New DateTime(reader.ReadInt64())
            End Using
        Else
            installationDate = DateTime.Now
            Using writer As New BinaryWriter(File.Open(ConfigFilePath, FileMode.Create))
                writer.Write(installationDate.Ticks)
            End Using
        End If

        Return installationDate
    End Function


    Public Function IsLicenseValid() As Boolean
        Dim currentDate As DateTime = DateTime.Now
        Dim installationDate As DateTime = GetInstallationDate()
        Dim expirationDate As DateTime = installationDate.Add(LicenseDuration)

        If currentDate < expirationDate Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetRemainingLicenseTime() As TimeSpan
        Dim currentDate As DateTime = DateTime.Now
        Dim installationDate As DateTime = GetInstallationDate()
        Dim expirationDate As DateTime = installationDate.Add(LicenseDuration)

        If currentDate < expirationDate Then
            Return expirationDate - currentDate
        Else
            Return TimeSpan.Zero
        End If
    End Function


    Private Sub ACTIVATION_FORM_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        CekAktivasiProgramDiAktivasiForm()
    End Sub

    Private Sub CekAktivasiProgramDiAktivasiForm()
        Dim HostName As String
        'Dim strIPAddress As String
        HostName = System.Net.Dns.GetHostName()
        'strIPAddress = System.Net.Dns.GetHostByName(HostName).AddressList(0).ToString()
        TxtDeviceName.Text = HostName


        Dim serial As Long
        Dim sm As New SecurityManager
        Dim temp As String
        Dim pjg As Integer
        Dim newSerial As String = ""
        serial = sm.GetSerial
        temp = serial
        pjg = temp.Length
        For i As Integer = 1 To pjg
            Dim a As String
            Dim b As Integer
            a = Mid(temp, i, 1)
            b = Asc(a)
            newSerial = newSerial & a & b Mod 2
        Next
        serialTextBox.Text = newSerial

        CheckLicense()

        Dim kg As New KeyGenerator
        Dim key As String = kg.GenerateKey(serialTextBox.Text)

        If activationKeyTextBox.Text <> key Then
            statusLabel.ForeColor = Color.Red
            statusLabel.Text = "Not Activated/invalid key"
            BtnAktivasi.Enabled = True

            Using licenseManager As New ACTIVATION_FORM()
                If Not licenseManager.IsLicenseValid() Then
                    Dim Message As String = "TERIMA KASIH ... !!!" & vbCrLf & "UNTUK TETAP MENGGUNAKAN PROGRAM INI HUBUNGI : 082 335 314 336 / ADI"
                    lblValidasi.Text = Message
                    'BtnClose.Visible = False
                End If
            End Using


            ' Menampilkan sisa waktu lisensi
            Dim remainingTime As TimeSpan = GetRemainingLicenseTime()
            Dim formattedTime As String = String.Format("{0} hari, {1} jam, {2} menit, {3} detik", remainingTime.Days, remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds)
            lblValidasi.Text = "Unlocking Full Potential, Time Left in Trial!" & formattedTime
            BtnClose.Text = "Use the application"
        Else
            statusLabel.ForeColor = Color.DarkGreen
            statusLabel.Text = "Activated"
            BtnAktivasi.Enabled = False
            lblValidasi.Visible = False
            BtnClose.Text = "Keluar"
        End If
    End Sub


    ''' <summary>Serial number yang dihitung dari hardware</summary>
    Public ReadOnly Property SerialNumber As String
        Get
            Return serialTextBox.Text
        End Get
    End Property

    ''' <summary>Activation key yang tersimpan di license.ini</summary>
    Public ReadOnly Property ActivationKey As String
        Get
            Return activationKeyTextBox.Text
        End Get
    End Property

    ''' <summary>True jika program sudah diaktivasi</summary>
    Public Function IsActivated() As Boolean
        CekAktivasiProgramDiAktivasiForm()
        Return statusLabel.ForeColor = Color.DarkGreen
    End Function

    Public Sub CheckLicense()
        If System.IO.File.Exists(bejoLicenseFile) Then
            BejoWriteSettings(bejoLicenseFile, "LICENSE", "serial", serialTextBox.Text)
            activationKeyTextBox.Text = BejoReadSettings(bejoLicenseFile, "LICENSE", "activation_key", "")
        Else
            BejoWriteSettings(bejoLicenseFile, "LICENSE", "serial", serialTextBox.Text)
            activationKeyTextBox.Text = ""
        End If
    End Sub

    Private Sub BtnAktivasi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAktivasi.Click
        Dim kg As New KeyGenerator
        Dim key As String = kg.GenerateKey(serialTextBox.Text)

        If activationKeyTextBox.Text <> key Then
            statusLabel.ForeColor = Color.Red
            statusLabel.Text = "Not Activated/invalid key"
            MessageBox.Show("Invalid activation key!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            BejoWriteSettings(bejoLicenseFile, "LICENSE", "activation_key", activationKeyTextBox.Text)
            statusLabel.ForeColor = Color.DarkGreen
            statusLabel.Text = "Activated"
            Close()
        End If
    End Sub

    Private Sub BtnGenerate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGenerate.Click
        Dim kg As New KeyGenerator
        Dim key As String = kg.GenerateKey(serialTextBox.Text)
        activationKeyTextBox.Text = key
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        If BtnClose.Text = "Use the application" Then
            Close()
        Else
            End
        End If

    End Sub
End Class