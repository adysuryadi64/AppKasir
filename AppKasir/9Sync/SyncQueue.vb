Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

''' <summary>
''' Manajemen antrian sinkronisasi ke Supabase
''' </summary>
Public Class SyncQueue

    ''' <summary>Tambah item ke queue saat data berubah</summary>
    Public Shared Sub Enqueue(aksi As String, tabel As String, idLokal As String,
                               idCloud As String, payload As Object)
        Try
            Dim json As String = JsonConvert.SerializeObject(payload)
            Using cmd As New MySqlCommand(
                "INSERT INTO sync_queue (aksi, tabel, id_lokal, id_cloud, payload, status, retry_count)
                 VALUES (@aksi, @tabel, @idLokal, @idCloud, @payload, 'pending', 0)
                 ON DUPLICATE KEY UPDATE
                     aksi = @aksi, payload = @payload, status = 'pending',
                     retry_count = 0, last_error = NULL, updated_at = NOW()", conn)
                cmd.Parameters.AddWithValue("@aksi", aksi)
                cmd.Parameters.AddWithValue("@tabel", tabel)
                cmd.Parameters.AddWithValue("@idLokal", idLokal)
                cmd.Parameters.AddWithValue("@idCloud", If(idCloud, ""))
                cmd.Parameters.AddWithValue("@payload", json)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            SyncLog.Tulis("ERROR", Nothing, idLokal, Nothing, "Enqueue gagal: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Ambil semua item pending (max 50 per batch)</summary>
    Public Shared Function GetPending() As List(Of SyncQueueItem)
        Dim list As New List(Of SyncQueueItem)
        Using cmd As New MySqlCommand(
            "SELECT id, aksi, tabel, id_lokal, id_cloud, payload, retry_count
             FROM sync_queue
             WHERE status = 'pending' AND retry_count < 5
             ORDER BY id ASC LIMIT 50", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    list.Add(New SyncQueueItem() With {
                        .Id = rd.GetInt32("id"),
                        .Aksi = rd.GetString("aksi"),
                        .Tabel = rd.GetString("tabel"),
                        .IdLokal = rd.GetString("id_lokal"),
                        .IdCloud = If(rd.IsDBNull(rd.GetOrdinal("id_cloud")), "", rd.GetString("id_cloud")),
                        .Payload = rd.GetString("payload"),
                        .RetryCount = rd.GetByte("retry_count")
                    })
                End While
            End Using
        End Using
        Return list
    End Function

    ''' <summary>Tandai item sebagai berhasil</summary>
    Public Shared Sub SetDone(id As Integer, idCloud As String)
        Using cmd As New MySqlCommand(
            "UPDATE sync_queue SET status = 'done', id_cloud = @idCloud, updated_at = NOW()
             WHERE id = @id", conn)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.Parameters.AddWithValue("@idCloud", idCloud)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>Tandai item gagal, tambah retry count</summary>
    Public Shared Sub SetFailed(id As Integer, errorMsg As String)
        Using cmd As New MySqlCommand(
            "UPDATE sync_queue
             SET retry_count = retry_count + 1,
                 last_error = @err,
                 status = IF(retry_count + 1 >= 5, 'failed', 'pending'),
                 updated_at = NOW()
             WHERE id = @id", conn)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.Parameters.AddWithValue("@err", errorMsg)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>Jumlah item pending</summary>
    Public Shared Function CountPending() As Integer
        Using cmd As New MySqlCommand(
            "SELECT COUNT(*) FROM sync_queue WHERE status = 'pending' AND retry_count < 5", conn)
            Return Convert.ToInt32(cmd.ExecuteScalar())
        End Using
    End Function

End Class

Public Class SyncQueueItem
    Public Property Id As Integer
    Public Property Aksi As String
    Public Property Tabel As String
    Public Property IdLokal As String
    Public Property IdCloud As String
    Public Property Payload As String
    Public Property RetryCount As Byte
End Class
