Imports AutoUpdaterDotNET

Public Class FormCekUpdate

    ' URL untuk mengakses file XML update di internet (misal: GitHub raw url, gist, dsb.)
    ' Anda harus mengganti URL ini dengan URL file XML Anda nanti.
    Private Const urlUpdateXML As String = "https://raw.githubusercontent.com/adysuryadi64/AppKasir/master/update.xml"

    Private Sub FormCekUpdate_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Saat form pertama kali dimuat, kita siapkan progres bar dan label status
        ProgressBar.Visible = False
        lblStatus.Text = "Siap untuk cek update."
    End Sub

    Private Sub btnCekUpdate_Click(sender As Object, e As EventArgs) Handles btnCekUpdate.Click
        ' Nonaktifkan tombol agar tidak bisa diklik ganda saat proses berjalan
        btnCekUpdate.Enabled = False
        lblStatus.Text = "Mengecek versi terbaru..."
        ProgressBar.Visible = True
        ProgressBar.Style = ProgressBarStyle.Marquee

        ' Tambahkan event handler untuk mengetahui kapan proses cek selesai
        AddHandler AutoUpdater.CheckForUpdateEvent, AddressOf AutoUpdaterOnCheckForUpdateEvent

        ' Mulai cek update aplikasi
        AutoUpdater.Start(urlUpdateXML)
    End Sub

    Private Sub AutoUpdaterOnCheckForUpdateEvent(args As UpdateInfoEventArgs)
        ' Hapus handler agar tidak menumpuk
        RemoveHandler AutoUpdater.CheckForUpdateEvent, AddressOf AutoUpdaterOnCheckForUpdateEvent

        ' Jika form sudah ditutup/dispose, abaikan callback ini
        If Me.IsDisposed Then Return

        ' Karena event ini berjalan di background thread, kita butuh Invoke untuk memodifikasi UI
        If Me.InvokeRequired Then
            Me.Invoke(Sub() AutoUpdaterOnCheckForUpdateEvent(args))
            Return
        End If

        ' Kembalikan tombol dan sembunyikan progress bar
        btnCekUpdate.Enabled = True
        ProgressBar.Visible = False

        If args.Error Is Nothing Then
            If args.IsUpdateAvailable Then
                lblStatus.Text = "Versi baru tersedia: " & args.CurrentVersion
                ' Memunculkan dialog download bawaan AutoUpdater.NET
                Try
                    If AutoUpdater.DownloadUpdate(args) Then
                        Application.Exit()
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error Update", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Else
                lblStatus.Text = "Aplikasi sudah versi terbaru."
                MessageBox.Show("Aplikasi sudah berada di versi terbaru (" & args.InstalledVersion.ToString() & ").", "Cek Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            lblStatus.Text = "Gagal cek update!"
            MessageBox.Show("Terjadi masalah saat cek update: " & args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close() ' Menutup form cek update
    End Sub
End Class

