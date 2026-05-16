Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json.Linq

''' <summary>
''' Dipanggil setiap kali data barang berubah (insert/update)
''' untuk memasukkan ke sync_queue secara otomatis
''' </summary>
Public Class SyncTrigger

    ''' <summary>
    ''' Panggil ini setelah INSERT atau UPDATE tbl_barang berhasil.
    ''' Contoh: SyncTrigger.BarangBerubah("BRG-001", "INSERT", "admin")
    ''' </summary>
    Public Shared Sub BarangBerubah(idBarang As String, aksi As String, idUser As String)
        Try
            ' Tandai is_dirty = 1 dan naikkan version
            Using cmd As New MySqlCommand(
                "UPDATE tbl_barang
                 SET is_dirty = 1,
                     version = version + 1,
                     updated_by = @user,
                     updated_at = NOW()
                 WHERE ID_BARANG = @id", conn)
                cmd.Parameters.AddWithValue("@id", idBarang)
                cmd.Parameters.AddWithValue("@user", idUser)
                cmd.ExecuteNonQuery()
            End Using

            ' Ambil data terbaru untuk payload
            Dim payload = AmbilPayloadBarang(idBarang)
            If payload Is Nothing Then Return

            Dim idCloud As String = ""
            If payload("id_cloud") IsNot Nothing AndAlso payload("id_cloud").Type <> JTokenType.Null Then
                idCloud = payload("id_cloud").ToString()
            End If

            ' Masukkan ke queue
            SyncQueue.Enqueue(aksi, "tbl_barang", idBarang, idCloud, payload)

        Catch ex As Exception
            SyncLog.Tulis("ERROR", "tbl_barang", idBarang, "", "SyncTrigger error: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Panggil ini setelah INSERT atau UPDATE tabel master selain tbl_barang.
    ''' Contoh: SyncTrigger.MasterBerubah("tbl_kategori", "KAT-001", "INSERT", "admin")
    ''' </summary>
    Public Shared Sub MasterBerubah(tabel As String, idLokal As String, aksi As String, idUser As String)
        Try
            ' Tandai is_dirty = 1, naikkan version, catat updated_by
            Using cmd As New MySqlCommand(
                $"UPDATE `{tabel}`
                  SET is_dirty = 1,
                      version = version + 1,
                      updated_by = @user,
                      updated_at = NOW()
                  WHERE KODE = @id", conn)
                cmd.Parameters.AddWithValue("@id", idLokal)
                cmd.Parameters.AddWithValue("@user", idUser)
                cmd.ExecuteNonQuery()
            End Using

            ' Ambil payload generik (semua kolom)
            Dim payload = AmbilPayloadGeneric(tabel, idLokal)
            If payload Is Nothing Then Return

            Dim idCloud As String = ""
            If payload("id_cloud") IsNot Nothing AndAlso payload("id_cloud").Type <> JTokenType.Null Then
                idCloud = payload("id_cloud").ToString()
            End If

            SyncQueue.Enqueue(aksi, tabel, idLokal, idCloud, payload)

        Catch ex As Exception
            SyncLog.Tulis("ERROR", tabel, idLokal, "", "SyncTrigger.MasterBerubah error: " & ex.Message)
        End Try
    End Sub

    Private Shared Function AmbilPayloadGeneric(tabel As String, idLokal As String) As JObject
        Using cmd As New MySqlCommand(
            $"SELECT * FROM `{tabel}` WHERE KODE = @id LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@id", idLokal)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If Not rd.Read() Then Return Nothing
                Dim obj As New JObject()
                For i As Integer = 0 To rd.FieldCount - 1
                    Dim key As String = rd.GetName(i).ToLower()
                    If rd.IsDBNull(i) Then
                        obj(key) = Nothing
                    Else
                        obj(key) = JToken.FromObject(rd.GetValue(i))
                    End If
                Next
                Return obj
            End Using
        End Using
    End Function

    Private Shared Function AmbilPayloadBarang(idBarang As String) As JObject
        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, ID_BARANG_BANTU, NAMA_BARANG, NAMA_BARANG_BANTU,
                    JENIS, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, NAMA_SUPLIYER,
                    JENIS_SATUAN, HARGA_BELI,
                    BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR,
                    SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR,
                    ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR,
                    HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR,
                    SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR,
                    ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR,
                    HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR,
                    STOK_MIN, STOK_MAX,
                    id_cloud, version, updated_by
             FROM tbl_barang WHERE ID_BARANG = @id LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@id", idBarang)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If Not rd.Read() Then Return Nothing
                Dim obj As New JObject()
                For i As Integer = 0 To rd.FieldCount - 1
                    Dim key As String = rd.GetName(i).ToLower()
                    If rd.IsDBNull(i) Then
                        obj(key) = Nothing
                    Else
                        obj(key) = JToken.FromObject(rd.GetValue(i))
                    End If
                Next
                Return obj
            End Using
        End Using
    End Function

End Class
