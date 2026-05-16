''' <summary>
''' Pencatatan semua aktivitas sinkronisasi
''' </summary>
Public Class SyncLog

    Public Shared Sub Tulis(jenis As String, tabel As String,
                             idLokal As String, idCloud As String, pesan As String)
        Try
            Using cmd As New MySqlCommand(
                "INSERT INTO sync_log (jenis, tabel, id_lokal, id_cloud, pesan)
                 VALUES (@jenis, @tabel, @idLokal, @idCloud, @pesan)", conn)
                cmd.Parameters.AddWithValue("@jenis", jenis)
                cmd.Parameters.AddWithValue("@tabel", If(tabel, ""))
                cmd.Parameters.AddWithValue("@idLokal", If(idLokal, ""))
                cmd.Parameters.AddWithValue("@idCloud", If(idCloud, ""))
                cmd.Parameters.AddWithValue("@pesan", If(pesan, ""))
                cmd.ExecuteNonQuery()
            End Using
        Catch
            ' Log tidak boleh crash aplikasi utama
        End Try
    End Sub

    Public Shared Function GetLog(limit As Integer) As DataTable
        Dim dt As New DataTable()
        Using cmd As New MySqlCommand(
            "SELECT waktu, jenis, tabel, id_lokal, id_cloud, pesan
             FROM sync_log ORDER BY waktu DESC LIMIT @limit", conn)
            cmd.Parameters.AddWithValue("@limit", limit)
            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

End Class
