Public Class FormMasuk
    Private Sub FormMasuk_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Select Case AppConfig.Instance.GetValue(Of String)("PilihanMasuk", "").ToUpper()
            Case "TOKO"
                HandleButtonClick("Toko.jpg", "TOKO")
            Case "GUDANG"
                HandleButtonClick("Gudang.jpg", "GUDANG")
        End Select
    End Sub

    Private Sub BtnToko_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnToko.Click
        HandleButtonClick("Toko.jpg", "TOKO")
    End Sub

    Private Sub BtnGudang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGudang.Click
        HandleButtonClick("Gudang.jpg", "GUDANG")
    End Sub

    Public Sub HandleButtonClick(ByVal backgroundImage As String, ByVal lokasi As String)
        Dim fullPath As String = IO.Path.Combine(Application.StartupPath, backgroundImage)

        Try
            If IO.File.Exists(fullPath) Then
                ' Baca semua byte dan langsung tutup file
                Dim imageBytes As Byte() = IO.File.ReadAllBytes(fullPath)

                ' Buat gambar dari memory stream tanpa mengunci file
                Using ms As New IO.MemoryStream(imageBytes)
                    ' Buat salinan gambar untuk memastikan tidak tergantung stream
                    Dim newImage As Image = Image.FromStream(ms)
                    ' Hapus gambar lama jika ada
                    If FormUtama.BackgroundImage IsNot Nothing Then
                        FormUtama.BackgroundImage.Dispose()
                    End If
                    FormUtama.BackgroundImage = newImage.Clone() ' Buat salinan independen
                End Using
            Else
                MessageBox.Show($"Gambar '{backgroundImage}' tidak ditemukan.", "Perhatian",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            FormUtama.Text = "KASIR LANCAR " & lokasi & " " & CompanyName
            FormUtama.SLokasi.Text = lokasi
            Close()

        Catch ex As Exception
            MessageBox.Show("Gagal memuat gambar: " & ex.Message, "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




    Private Sub BtnToko_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnToko.GotFocus
        BtnToko.BackColor = Color.Yellow
    End Sub

    Private Sub BtnToko_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnToko.LostFocus
        BtnToko.BackColor = Color.RoyalBlue
    End Sub

    Private Sub BtnGudang_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGudang.GotFocus
        BtnGudang.BackColor = Color.Yellow
    End Sub

    Private Sub BtnGudang_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGudang.LostFocus
        BtnGudang.BackColor = Color.RoyalBlue
    End Sub

End Class