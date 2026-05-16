Imports System.IO
Imports System.Text.Json

''' <summary>
''' Sistem konfigurasi aplikasi berbasis JSON
''' Menggantikan My.Settings untuk kompatibilitas versi baru
''' </summary>
Public Class AppConfig
    Private Shared _instance As AppConfig
    Private _settings As Dictionary(Of String, Object)
    Private Shared ReadOnly _filePath As String = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AppKasir",
        "config.json"
    )

    ''' <summary>
    ''' Singleton instance untuk akses global
    ''' </summary>
    Public Shared ReadOnly Property Instance As AppConfig
        Get
            If _instance Is Nothing Then
                _instance = New AppConfig()
                _instance.Load()
            End If
            Return _instance
        End Get
    End Property

    ''' <summary>
    ''' Default values untuk semua setting
    ''' </summary>
    Private Shared ReadOnly DefaultSettings As New Dictionary(Of String, Object) From {
        {"PilihanMasuk", ""},
        {"CbSatuansama", False},
        {"CetakJual", "SELALU TANYA"},
        {"TampilSN", False}
    }

    Public Sub New()
        _settings = New Dictionary(Of String, Object)()
    End Sub

    ''' <summary>
    ''' Load konfigurasi dari file JSON
    ''' </summary>
    Public Sub Load()
        Try
            _settings = New Dictionary(Of String, Object)()

            ' Set default values dulu
            For Each kvp In DefaultSettings
                _settings(kvp.Key) = kvp.Value
            Next

            ' Jika file ada, load nilai dari file
            If File.Exists(_filePath) Then
                Dim json As String = File.ReadAllText(_filePath)
                If Not String.IsNullOrWhiteSpace(json) Then
                    Dim options = New JsonSerializerOptions With {
                        .PropertyNameCaseInsensitive = True,
                        .WriteIndented = True
                    }

                    Dim loadedSettings = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(json, options)

                    If loadedSettings IsNot Nothing Then
                        For Each kvp In loadedSettings
                            ' Konversi JsonElement ke tipe yang sesuai
                            Select Case kvp.Value.ValueKind
                                Case JsonValueKind.String
                                    _settings(kvp.Key) = kvp.Value.GetString()

                                Case JsonValueKind.True, JsonValueKind.False
                                    _settings(kvp.Key) = kvp.Value.GetBoolean()

                                Case JsonValueKind.Number
                                    If kvp.Value.TryGetInt32(CInt(_settings(kvp.Key))) Then
                                        ' Sudah di-set di atas
                                    ElseIf kvp.Value.TryGetDecimal(CDec(_settings(kvp.Key))) Then
                                        ' Sudah di-set di atas
                                    Else
                                        _settings(kvp.Key) = kvp.Value.GetDouble()
                                    End If

                                Case Else
                                    ' Gunakan nilai default untuk tipe yang tidak dikenali
                            End Select
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            ' Jika ada error, gunakan default settings
            _settings = New Dictionary(Of String, Object)(DefaultSettings)
            ' Optional: Log error jika diperlukan
            ' Debug.WriteLine($"Error loading config: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Simpan konfigurasi ke file JSON
    ''' </summary>
    Public Sub Save()
        Try
            ' Pastikan direktori ada
            Dim dirPath As String = Path.GetDirectoryName(_filePath)
            If Not String.IsNullOrEmpty(dirPath) AndAlso Not Directory.Exists(dirPath) Then
                Directory.CreateDirectory(dirPath)
            End If

            Dim options = New JsonSerializerOptions With {
                .PropertyNameCaseInsensitive = True,
                .WriteIndented = True
            }

            Dim json As String = JsonSerializer.Serialize(_settings, options)
            File.WriteAllText(_filePath, json)

        Catch ex As Exception
            ' Optional: Log error
            ' Debug.WriteLine($"Error saving config: {ex.Message}")
            Throw New Exception($"Gagal menyimpan konfigurasi: {ex.Message}", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Ambil nilai setting dengan tipe generic
    ''' </summary>
    Public Function GetValue(Of T)(key As String, Optional defaultValue As T = Nothing) As T
        Try
            If _settings.ContainsKey(key) Then
                Dim value = _settings(key)

                If value Is Nothing Then
                    Return defaultValue
                End If

                ' Konversi tipe
                If GetType(T) Is GetType(String) Then
                    Return CType(CObj(value.ToString()), T)

                ElseIf GetType(T) Is GetType(Boolean) Then
                    Dim boolVal As Boolean
                    If TypeOf value Is Boolean Then
                        boolVal = CBool(value)
                    ElseIf Boolean.TryParse(value.ToString(), boolVal) Then
                        Return CType(CObj(boolVal), T)
                    Else
                        Return defaultValue
                    End If
                    Return CType(CObj(boolVal), T)

                ElseIf GetType(T) Is GetType(Integer) Then
                    If Integer.TryParse(value.ToString(), 0) Then
                        Return CType(CObj(CInt(value)), T)
                    Else
                        Return defaultValue
                    End If

                ElseIf GetType(T) Is GetType(Decimal) Then
                    If Decimal.TryParse(value.ToString(), CDec(0)) Then
                        Return CType(CObj(CDec(value)), T)
                    Else
                        Return defaultValue
                    End If

                Else
                    Return CType(value, T)
                End If
            Else
                Return defaultValue
            End If

        Catch
            Return defaultValue
        End Try
    End Function

    ''' <summary>
    ''' Set nilai setting
    ''' </summary>
    Public Sub SetValue(key As String, value As Object)
        _settings(key) = value
    End Sub

    ''' <summary>
    ''' Cek apakah key ada dalam setting
    ''' </summary>
    Public Function ContainsKey(key As String) As Boolean
        Return _settings.ContainsKey(key)
    End Function

    ''' <summary>
    ''' Reset ke default settings
    ''' </summary>
    Public Sub ResetToDefaults()
        _settings = New Dictionary(Of String, Object)(DefaultSettings)
        Save()
    End Sub

End Class
