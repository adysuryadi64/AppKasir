Public Class FormCekUpdate

    ' URL untuk mengakses file version.txt di Google Drive
    Private Const urlVersiOnline As String = "https://drive.google.com/uc?export=download&id=1tbxflnu2Jh5t3PvS2Akw_4sTI3RjivUT" ' Link ini adalah link untuk mendownload file version.txt

    ' Versi aplikasi lokal (gunakan versi aplikasi yang sudah didefinisikan di AssemblyInfo)
    Private ReadOnly versiLokal As String = Application.ProductVersion

    Private Sub FormCekUpdate_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Saat form pertama kali dimuat, kita siapkan progres bar dan label status
        ProgressBar.Visible = False
        lblStatus.Text = "Siap untuk cek update."
    End Sub

    Private Sub btnCekUpdate_Click(sender As Object, e As EventArgs) Handles btnCekUpdate.Click
        ' Ketika tombol "Cek Update" ditekan, kita akan mulai cek update
        lblStatus.Text = "Mengecek versi terbaru..."
        ProgressBar.Visible = True
        ProgressBar.Style = ProgressBarStyle.Marquee ' Menunjukkan proses berjalan

        ' Mulai cek update aplikasi
        CheckForUpdate()
    End Sub

    Private Sub CheckForUpdate()
        Try
            ' Gunakan WebClient untuk download file version.txt dari Google Drive
            Using client As New Net.WebClient()
                ' Men-download isi file version.txt
                Dim versiOnline As String = client.DownloadString(urlVersiOnline).Trim()

                ' Pisahkan isi file version.txt, bagian pertama adalah versi, bagian kedua adalah link download
                Dim versiInfo() As String = versiOnline.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
                Dim versiTerbaru As String = versiInfo(0)
                Dim linkDownload As String = versiInfo(1)

                ' Bandingkan versi lokal dengan versi online
                If New Version(versiLokal) < New Version(versiTerbaru) Then
                    lblStatus.Text = "Versi baru tersedia: " & versiTerbaru
                    MessageBox.Show("Versi baru tersedia! Versi terbaru: " & versiTerbaru, "Update Tersedia", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Arahkan user untuk download update menggunakan link yang diperoleh dari version.txt
                    Process.Start(linkDownload) ' Ganti dengan link download
                Else
                    lblStatus.Text = "Aplikasi sudah versi terbaru: " & versiLokal
                    MessageBox.Show("Aplikasi sudah versi terbaru.", "Cek Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            ' Jika terjadi error saat mengambil data (misal file tidak ditemukan atau internet bermasalah)
            lblStatus.Text = "Gagal cek update!"
            MessageBox.Show("Gagal cek update: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Menyembunyikan progress bar setelah selesai
            ProgressBar.Visible = False
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close() ' Menutup form cek update
    End Sub
End Class
