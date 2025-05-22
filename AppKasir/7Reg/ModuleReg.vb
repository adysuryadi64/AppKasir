Friend Module ModuleReg
    Public bejoDirectory As String = My.Application.Info.DirectoryPath
    Public bejoLicenseFile As String = bejoDirectory & "\license.ini"

    Private Declare Unicode Function GetPrivateProfileString Lib "kernel32" _
    Alias "GetPrivateProfileStringW" (ByVal lpApplicationName As String,
    ByVal lpKeyName As String, ByVal lpDefault As String,
    ByVal lpReturnedString As String, ByVal nSize As Int32,
    ByVal lpFileName As String) As Int32

    Private Declare Unicode Function WritePrivateProfileString Lib "kernel32" _
        Alias "WritePrivateProfileStringW" (ByVal lpApplicationName As String,
        ByVal lpKeyName As String, ByVal lpString As String,
        ByVal lpFileName As String) As Int32


    Public Sub BejoWriteSettings(ByVal iniFileName As String, ByVal Section As String, ByVal ParamName As String, ByVal ParamVal As String)

        Dim Result As Integer = WritePrivateProfileString(Section, ParamName, ParamVal, iniFileName)
    End Sub


    Public Function BejoReadSettings(ByVal IniFileName As String, ByVal Section As String, ByVal ParamName As String, ByVal ParamDefault As String) As String

        Dim ParamVal As String = Space$(1024)
        Dim LenParamVal As Long = GetPrivateProfileString(Section, ParamName, ParamDefault, ParamVal, Len(ParamVal), IniFileName)
        'bejoReadSettings = Left$(ParamVal, LenParamVal)
        BejoReadSettings = Microsoft.VisualBasic.Left(ParamVal, LenParamVal)
    End Function

End Module
