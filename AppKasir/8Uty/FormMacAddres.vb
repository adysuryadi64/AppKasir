Imports System.Net.NetworkInformation

Public Class FormMacAddres
    Public Function GetMacAddress()

        Dim nics() As NetworkInterface = NetworkInterface.GetAllNetworkInterfaces
        Return nics(1).GetPhysicalAddress.ToString
    End Function

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim a As Integer ' Declare variable 'a'

        For a = 1 To NetworkInterface.GetAllNetworkInterfaces().Length - 1
            Dim nics() As NetworkInterface = NetworkInterface.GetAllNetworkInterfaces()
            ListBox1.Items.Add(nics(a).GetPhysicalAddress.ToString)
        Next

    End Sub
End Class