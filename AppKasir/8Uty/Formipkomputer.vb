Imports System.Net

Public Class Formipkomputer

    Private Sub Formipkomputer_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim HostName As String
        'Dim strIPAddress As String
        HostName = System.Net.Dns.GetHostName()
        'strIPAddress = System.Net.Dns.GetHostByName(HostName).AddressList(0).ToString()
        TxtHostName.Text = HostName
        'TxtIPAddress.Text = strIPAddress

        Dim strHostName As Net.IPHostEntry = Dns.GetHostEntry(System.Net.Dns.GetHostName)
        For Each IPAddress As Net.IPAddress In strHostName.AddressList
            ListIPAddress.Items.Add(IPAddress.ToString())
        Next

    End Sub
End Class