Imports Microsoft.Win32
Imports System.IO

Public Class FormAbout

    Private Sub FormAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Configure IE emulation for better HTML5/CSS3 support
        SetIEEmulation()

        ' Load the PHP guide
        LoadGuide()
    End Sub

    ''' <summary>
    ''' Sets IE emulation mode to IE11 for better HTML5/CSS3 support in WebBrowser control
    ''' </summary>
    Private Sub SetIEEmulation()
        Try
            Dim appName As String = Path.GetFileName(Application.ExecutablePath)
            Dim emulationValue As Integer = 11001 ' IE11 Edge mode

            Using rk As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", True)
                If rk IsNot Nothing Then
                    Dim currentValue As Object = rk.GetValue(appName)
                    If currentValue Is Nothing Then
                        rk.SetValue(appName, emulationValue, RegistryValueKind.DWord)
                    End If
                End If
            End Using
        Catch ex As Exception
            ' Silently fail if registry access is denied
            Debug.WriteLine("Error setting IE emulation: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the HTML guide into the WebBrowser control
    ''' </summary>
    Private Sub LoadGuide()
        Try
            Dim guidePath As String = Path.Combine(Application.StartupPath, "guide.html")
            
            If File.Exists(guidePath) Then
                ' Convert to absolute file path for WebBrowser
                Dim fileUri As New Uri(guidePath)
                WebBrowser1.Navigate(fileUri.AbsoluteUri)
            Else
                ' Fallback: show error message in HTML
                ShowErrorMessage("File guide.html tidak ditemukan")
            End If
        Catch ex As Exception
            ShowErrorMessage("Error memuat guide: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Shows an error message in the WebBrowser control
    ''' </summary>
    Private Sub ShowErrorMessage(message As String)
        Dim html As String = $"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        margin: 0;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    }}
                    .error-box {{
                        background: white;
                        padding: 40px;
                        border-radius: 10px;
                        box-shadow: 0 10px 40px rgba(0,0,0,0.3);
                        text-align: center;
                        max-width: 500px;
                    }}
                    .error-icon {{
                        font-size: 60px;
                        margin-bottom: 20px;
                    }}
                    h1 {{
                        color: #dc3545;
                        margin-bottom: 15px;
                    }}
                    p {{
                        color: #666;
                        line-height: 1.6;
                    }}
                </style>
            </head>
            <body>
                <div class='error-box'>
                    <div class='error-icon'>⚠️</div>
                    <h1>Error</h1>
                    <p>{message}</p>
                </div>
            </body>
            </html>
        "
        WebBrowser1.DocumentText = html
    End Sub

End Class