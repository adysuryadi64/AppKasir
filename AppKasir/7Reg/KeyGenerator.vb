Public Class KeyGenerator
    Public Function GenerateKey(ByVal serial As String) As String
        Dim x As String = serial
        Dim temp As String = x * x / (x / 2) + x / 12031991
        Dim a As String = ""
        For i As Integer = 0 To temp.Length - 1
            Dim s As Char = temp.Substring(i, 1)
            If s = "," Or s = "." Then
                a &= Asc(".")
            Else
                a &= Asc(temp.Substring(i, 1))
            End If
        Next
        Dim b As String = a.Substring(0, 5) & "-" & a.Substring(5, 5) & "-" & a.Substring(10, 5) & "-" & a.Substring(15, 5) & "-" & a.Substring(20, 5)
        Return b
        'Return True
    End Function
End Class

