Imports System.Management

Public Class SecurityManager

    Friend Function GetProcessorId() As String
        Dim strProcessorId As String = String.Empty
        Dim query As New SelectQuery("Win32_processor")
        Dim search As New ManagementObjectSearcher(query)
        Dim info As ManagementObject

        For Each info In search.Get()
            strProcessorId = info("processorId").ToString()
        Next
        Return strProcessorId
    End Function

    Friend Function GetMotherBoardID() As String
        Dim strMotherBoardID As String = String.Empty
        Dim query As New SelectQuery("Win32_BaseBoard")
        Dim search As New ManagementObjectSearcher(query)
        Dim info As ManagementObject
        For Each info In search.Get()
            strMotherBoardID = info("SerialNumber").ToString()
        Next
        Return strMotherBoardID
    End Function

    Friend Function GetVolumeSerial(Optional ByVal strDriveLetter As String = "C") As String
        Dim disk As New ManagementObject(String.Format("win32_logicaldisk.deviceid=""{0}:""", strDriveLetter))
        disk.Get()
        Return disk("VolumeSerialNumber").ToString()
    End Function


    Public Function GetSerial() As Long
        Dim mac As String
        mac = GetMotherBoardID() & GetProcessorId() & GetVolumeSerial("C")
        Dim sum As Long = 0
        Dim index As Integer = 1
        For Each ch As Char In mac
            If Char.IsDigit(ch) Then
                sum += sum + Integer.Parse(ch) * (index * 2)
            ElseIf Char.IsLetter(ch) Then
                Select Case ch.ToString.ToUpper
                    Case "A"
                        sum += sum + 10 * (index * 2)
                    Case "B"
                        sum += sum + 11 * (index * 2)
                    Case "C"
                        sum += sum + 12 * (index * 2)
                    Case "D"
                        sum += sum + 13 * (index * 2)
                    Case "E"
                        sum += sum + 14 * (index * 2)
                    Case "F"
                        sum += sum + 15 * (index * 2)
                End Select
            End If

            index += 1
        Next

        Return sum
    End Function

    Public Function CheckKey(ByVal key As Long) As Boolean
        Dim x As Long = GetSerial()
        Dim y As Long = x * x + 53 / x + 113 * (x / 4)
        Return y = key
    End Function
End Class
