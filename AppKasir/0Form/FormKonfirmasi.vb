Public Class FormKonfirmasi

    Public BtnCloseClicked As Boolean = False

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        BtnCloseClicked = True
        Close()
    End Sub

    Private Sub BtnKonfirmasi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKonfirmasi.Click


    End Sub


    Private Sub Konfirmasi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Panel1/5 = area input (otomatis biru muda via nama PanelInput*)
        ' Rename Panel1/5 -> PanelInput untuk tema otomatis
    End Sub
End Class