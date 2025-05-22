Public Class FormMasuk
    Private Sub FormMasuk_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Select Case My.Settings.PilihanMasuk
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

    Private Sub HandleButtonClick(ByVal backgroundImage As String, ByVal lokasi As String)
        FormUtama.ChangeBackgroundImage(backgroundImage)
        FormUtama.Text = "KASIR LANCAR " & lokasi & " " & CompanyName
        FormUtama.SLokasi.Text = lokasi
        Close()
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