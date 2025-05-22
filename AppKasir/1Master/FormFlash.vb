Public Class FormFlash
    Private Sub Form_Flash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ProgressBar1.Value = 0
        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ProgressBar1.Value += 25
        If ProgressBar1.Value = 100 Then
            Timer1.Dispose()
            Hide()
            'Form_Login.Show()
        End If
    End Sub
End Class