Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' Helper untuk komunikasi REST API ke Supabase
''' </summary>
Public Class SupabaseHelper

    Private Shared ReadOnly _client As New HttpClient()
    Private Shared _supabaseUrl As String = ""
    Private Shared _supabaseKey As String = ""
    Private Shared _initialized As Boolean = False

    Public Shared Sub Init(url As String, apiKey As String)
        _supabaseUrl = url.TrimEnd("/"c)
        _supabaseKey = apiKey
        _client.DefaultRequestHeaders.Clear()
        _client.DefaultRequestHeaders.Add("apikey", apiKey)
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " & apiKey)
        _client.Timeout = TimeSpan.FromSeconds(30)
        _initialized = True
    End Sub

    Public Shared Function IsInitialized() As Boolean
        Return _initialized AndAlso Not String.IsNullOrEmpty(_supabaseUrl)
    End Function

    ''' <summary>Cek koneksi ke Supabase</summary>
    Public Shared Function CekKoneksi() As Boolean
        Try
            Dim url As String = $"{_supabaseUrl}/rest/v1/barang_master?select=id&limit=1"
            Dim resp = _client.GetAsync(url).Result
            Return resp.IsSuccessStatusCode
        Catch
            Return False
        End Try
    End Function

    ''' <summary>GET data dari tabel Supabase</summary>
    Public Shared Function [Get](tabel As String, query As String) As JArray
        Try
            Dim url As String = $"{_supabaseUrl}/rest/v1/{tabel}?{query}"
            Dim resp = _client.GetAsync(url).Result
            Dim body As String = resp.Content.ReadAsStringAsync().Result
            If resp.IsSuccessStatusCode Then
                Return JArray.Parse(body)
            End If
            Throw New Exception($"GET {tabel} gagal [{resp.StatusCode}]: {body}")
        Catch ex As Exception
            Throw New Exception("SupabaseHelper.Get: " & ex.Message)
        End Try
    End Function

    ''' <summary>POST (INSERT) data ke Supabase, return JObject baris baru</summary>
    Public Shared Function Post(tabel As String, data As Object) As JObject
        Try
            Dim json As String = JsonConvert.SerializeObject(data)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim url As String = $"{_supabaseUrl}/rest/v1/{tabel}"

            ' Prefer: return=representation agar dapat data yang baru dibuat
            Dim req As New HttpRequestMessage(HttpMethod.Post, url) With {
                .Content = content
            }
            req.Headers.Add("Prefer", "return=representation")

            Dim resp = _client.SendAsync(req).Result
            Dim body As String = resp.Content.ReadAsStringAsync().Result
            If resp.IsSuccessStatusCode Then
                Dim arr = JArray.Parse(body)
                Return If(arr.Count > 0, CType(arr(0), JObject), New JObject())
            End If
            Throw New Exception($"POST {tabel} gagal [{resp.StatusCode}]: {body}")
        Catch ex As Exception
            Throw New Exception("SupabaseHelper.Post: " & ex.Message)
        End Try
    End Function

    ''' <summary>PATCH (UPDATE) data di Supabase berdasarkan filter</summary>
    Public Shared Function Patch(tabel As String, filter As String, data As Object) As Boolean
        Try
            Dim json As String = JsonConvert.SerializeObject(data)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim url As String = $"{_supabaseUrl}/rest/v1/{tabel}?{filter}"

            Dim req As New HttpRequestMessage(New HttpMethod("PATCH"), url) With {
                .Content = content
            }
            req.Headers.Add("Prefer", "return=minimal")

            Dim resp = _client.SendAsync(req).Result
            If resp.IsSuccessStatusCode Then Return True
            Dim body As String = resp.Content.ReadAsStringAsync().Result
            Throw New Exception($"PATCH {tabel} gagal [{resp.StatusCode}]: {body}")
        Catch ex As Exception
            Throw New Exception("SupabaseHelper.Patch: " & ex.Message)
        End Try
    End Function

    ''' <summary>POST dengan UPSERT — insert or update berdasarkan conflict columns</summary>
    Public Shared Sub PostUpsert(tabel As String, data As Object)
        Try
            Dim json As String = JsonConvert.SerializeObject(data)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim url As String = $"{_supabaseUrl}/rest/v1/{tabel}"

            Dim req As New HttpRequestMessage(HttpMethod.Post, url) With {
                .Content = content
            }
            req.Headers.Add("Prefer", $"resolution=merge-duplicates,return=minimal")

            Dim resp = _client.SendAsync(req).Result
            If Not resp.IsSuccessStatusCode Then
                Dim body As String = resp.Content.ReadAsStringAsync().Result
                Throw New Exception($"UPSERT {tabel} gagal [{resp.StatusCode}]: {body}")
            End If
        Catch ex As Exception
            Throw New Exception("SupabaseHelper.PostUpsert: " & ex.Message)
        End Try
    End Sub

    Public Shared Function GetBaseUrl() As String
        Return _supabaseUrl
    End Function
    Public Shared Function Delete(tabel As String, filter As String) As Boolean
        Try
            Dim url As String = $"{_supabaseUrl}/rest/v1/{tabel}?{filter}"
            Dim req As New HttpRequestMessage(HttpMethod.Delete, url)
            Dim resp = _client.SendAsync(req).Result
            Return resp.IsSuccessStatusCode
        Catch ex As Exception
            Throw New Exception("SupabaseHelper.Delete: " & ex.Message)
        End Try
    End Function

End Class
