Imports Newtonsoft.Json.Linq

''' <summary>
''' Inti sistem sinkronisasi: upload, download, queue, transfer
''' </summary>
Public Class SyncManager

    Public Shared Event OnProgress(pesan As String)
    Public Shared Event OnSelesai(sukses As Integer, gagal As Integer)

    Private Shared Sub Log(pesan As String)
        RaiseEvent OnProgress(pesan)
    End Sub

#Region "SYNC UTAMA"

    ''' <summary>Upload + Download sekaligus</summary>
    Public Shared Sub SyncSemua()
        If Not CekKoneksiDanLog() Then Return
        Log("Memulai sinkronisasi penuh...")
        SyncUploadSemua()
        SyncDownloadSemua()
        Log("Sinkronisasi selesai.")
    End Sub

    ''' <summary>Hanya upload — kirim perubahan lokal ke Supabase</summary>
    Public Shared Sub SyncUploadSemua()
        If Not CekKoneksiDanLog() Then Return
        Log("=== UPLOAD ===")
        ProcessQueue()
        UploadTransferOffline()
        UploadKonfirmasiTerimaPending()
        UploadSemuaSnapshot()
    End Sub

    ''' <summary>Upload konfirmasi terima transfer yang pending (diterima saat offline)</summary>
    Public Shared Sub UploadKonfirmasiTerimaPending()
        Try
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT id, id_cloud, kode_barang, id_user FROM transfer_terima_pending WHERE status='PENDING'", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            If dt.Rows.Count = 0 Then Return
            Log($"Upload {dt.Rows.Count} konfirmasi terima pending...")
            For Each row As DataRow In dt.Rows
                Try
                    SupabaseHelper.Patch("transfer_barang_cloud", $"id=eq.{row("id_cloud")}",
                        New Dictionary(Of String, Object) From {
                            {"status", "diterima"},
                            {"id_user_terima", row("id_user").ToString()},
                            {"tgl_terima", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}
                        })
                    Using cmd As New MySqlCommand(
                        "UPDATE transfer_terima_pending SET status='DONE' WHERE id=@id", conn)
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(row("id")))
                        cmd.ExecuteNonQuery()
                    End Using
                    Log($"  Konfirmasi terima [{row("id_cloud")}] sukses")
                Catch ex As Exception
                    SyncLog.Tulis("ERROR", "transfer_terima", row("kode_barang").ToString(), row("id_cloud").ToString(), ex.Message)
                End Try
            Next
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "transfer_terima", "", "", "UploadKonfirmasiTerimaPending: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Upload semua snapshot untuk laporan</summary>
    Public Shared Sub UploadSemuaSnapshot()
        Log("Upload snapshot laporan...")
        UploadSnapshotCabang()
        UploadSnapshotHutangSupliyer()
        UploadSnapshotPiutangPelanggan()
        UploadSnapshotKaryawan()
        UploadSnapshotGaji()
        UploadSnapshotCOA()
        Log("Snapshot laporan selesai.")
    End Sub

    ''' <summary>Hanya download — ambil data terbaru dari Supabase</summary>
    Public Shared Sub SyncDownloadSemua()
        If Not CekKoneksiDanLog() Then Return
        Log("=== DOWNLOAD ===")
        SyncDownloadBarang()
        SyncDownloadMaster("kategori_master", "tbl_kategori", "KODE", SyncConfig.LastSyncKategori, AddressOf SyncConfig.UpdateLastSyncKategori)
        SyncDownloadMaster("satuan_master", "tbl_satuan", "KODE", SyncConfig.LastSyncSatuan, AddressOf SyncConfig.UpdateLastSyncSatuan)
        SyncDownloadMaster("merk_master", "tbl_merk", "KODE", SyncConfig.LastSyncMerk, AddressOf SyncConfig.UpdateLastSyncMerk)
        SyncDownloadMaster("supliyer_master", "tbl_supliyer", "KODE", SyncConfig.LastSyncSupliyer, AddressOf SyncConfig.UpdateLastSyncSupliyer)
        SyncDownloadMaster("pelanggan_master", "tbl_pelanggan", "KODE", SyncConfig.LastSyncPelanggan, AddressOf SyncConfig.UpdateLastSyncPelanggan)
        SyncDownloadMaster("armada_master", "tbl_armada", "KODE", SyncConfig.LastSyncArmada, AddressOf SyncConfig.UpdateLastSyncArmada)
        SyncDownloadCabang()
        SyncDownloadTransfer()
    End Sub

    Private Shared Function CekKoneksiDanLog() As Boolean
        If Not SupabaseHelper.IsInitialized() Then
            Log("Supabase belum dikonfigurasi.")
            Return False
        End If
        If Not SupabaseHelper.CekKoneksi() Then
            Log("Tidak ada koneksi ke Supabase. Mode offline.")
            Return False
        End If
        Return True
    End Function

#End Region

#Region "PROCESS QUEUE (UPLOAD)"

    ''' <summary>Proses antrian upload ke Supabase</summary>
    Public Shared Sub ProcessQueue()
        Dim items = SyncQueue.GetPending()
        If items.Count = 0 Then
            Log("Tidak ada data pending di queue.")
            Return
        End If

        Log($"Memproses {items.Count} item queue...")
        Dim sukses As Integer = 0
        Dim gagal As Integer = 0

        For Each item In items
            Try
                Select Case item.Tabel
                    Case "tbl_barang"
                        ProsesQueueBarang(item)
                    Case "tbl_cabang"
                        ProsesQueueCabang(item)
                    Case Else
                        SyncQueue.SetFailed(item.Id, "Tabel tidak dikenal: " & item.Tabel)
                        Continue For
                End Select
                sukses += 1
            Catch ex As Exception
                gagal += 1
                SyncQueue.SetFailed(item.Id, ex.Message)
                SyncLog.Tulis("ERROR", item.Tabel, item.IdLokal, item.IdCloud, ex.Message)
                Log($"  GAGAL [{item.IdLokal}]: {ex.Message}")
            End Try
        Next

        Log($"Queue selesai: {sukses} sukses, {gagal} gagal.")
        RaiseEvent OnSelesai(sukses, gagal)
    End Sub

    Private Shared Sub ProsesQueueBarang(item As SyncQueueItem)
        Dim data As JObject = JObject.Parse(item.Payload)

        If item.Aksi = "INSERT" OrElse String.IsNullOrEmpty(item.IdCloud) Then
            ' Cek apakah sudah ada di Supabase berdasarkan id_barang
            Dim existing = SupabaseHelper.Get("barang_master",
                $"id_barang=eq.{Uri.EscapeDataString(item.IdLokal)}&select=id,version")

            If existing.Count > 0 Then
                ' Sudah ada, lakukan update
                Dim idCloud As String = existing(0)("id").ToString()
                Dim versionCloud As Integer = existing(0)("version").Value(Of Integer)()
                DoUpdateBarang(item, data, idCloud, versionCloud)
            Else
                ' Benar-benar baru
                DoInsertBarang(item, data)
            End If
        Else
            ' UPDATE — ambil version cloud dulu
            Dim existing = SupabaseHelper.Get("barang_master",
                $"id=eq.{item.IdCloud}&select=id,version")
            If existing.Count = 0 Then
                ' Tidak ada di cloud, insert ulang
                DoInsertBarang(item, data)
                Return
            End If
            Dim versionCloud As Integer = existing(0)("version").Value(Of Integer)()
            DoUpdateBarang(item, data, item.IdCloud, versionCloud)
        End If
    End Sub

    Private Shared Sub DoInsertBarang(item As SyncQueueItem, data As JObject)
        data("kode_cabang_asal") = SyncConfig.KodeCabang
        Dim result = SupabaseHelper.Post("barang_master", data)
        Dim idCloud As String = result("id").ToString()

        ' Simpan id_cloud ke lokal
        UpdateIdCloudLokal("tbl_barang", "ID_BARANG", item.IdLokal, idCloud)
        SyncQueue.SetDone(item.Id, idCloud)
        SyncLog.Tulis("UPLOAD", "tbl_barang", item.IdLokal, idCloud, "INSERT sukses")
        Log($"  INSERT barang [{item.IdLokal}] → cloud [{idCloud}]")

        ' Upload stok ke stok_per_cabang untuk laporan
        UploadStokPerCabang(item.IdLokal, data)
    End Sub

    Private Shared Sub DoUpdateBarang(item As SyncQueueItem, data As JObject,
                                       idCloud As String, versionCloud As Integer)
        Dim versionLokal As Integer = If(data("version") IsNot Nothing,
                                         data("version").Value(Of Integer)(), 1)

        If versionLokal < versionCloud Then
            SimpanConflict("tbl_barang", item.IdLokal, idCloud, versionLokal, versionCloud, data.ToString())
            SyncQueue.SetFailed(item.Id, $"CONFLICT: version lokal {versionLokal} < cloud {versionCloud}")
            SyncLog.Tulis("CONFLICT", "tbl_barang", item.IdLokal, idCloud,
                          $"Version lokal={versionLokal}, cloud={versionCloud}")
            Log($"  CONFLICT barang [{item.IdLokal}] — perlu resolusi manual")
            Return
        End If

        data("version") = versionCloud + 1
        data("kode_cabang_asal") = SyncConfig.KodeCabang
        data.Remove("id_cloud")
        data.Remove("is_dirty")

        SupabaseHelper.Patch("barang_master", $"id=eq.{idCloud}", data)
        UpdateVersionLokal("tbl_barang", "ID_BARANG", item.IdLokal, versionCloud + 1)
        SyncQueue.SetDone(item.Id, idCloud)
        SyncLog.Tulis("UPLOAD", "tbl_barang", item.IdLokal, idCloud, "UPDATE sukses")
        Log($"  UPDATE barang [{item.IdLokal}] → cloud [{idCloud}]")

        ' Upload stok ke stok_per_cabang untuk laporan
        UploadStokPerCabang(item.IdLokal, data)
    End Sub

    Private Shared Sub ProsesQueueCabang(item As SyncQueueItem)
        Dim data As JObject = JObject.Parse(item.Payload)

        If item.Aksi = "INSERT" OrElse String.IsNullOrEmpty(item.IdCloud) Then
            ' Cek apakah sudah ada di Supabase
            Dim existing = SupabaseHelper.Get("cabang_master",
                $"kode_cabang=eq.{Uri.EscapeDataString(item.IdLokal)}&select=id,version")

            If existing.Count > 0 Then
                Dim idCloud As String = existing(0)("id").ToString()
                Dim versionCloud As Integer = existing(0)("version").Value(Of Integer)()
                DoUpdateCabang(item, data, idCloud, versionCloud)
            Else
                DoInsertCabang(item, data)
            End If
        Else
            Dim existing = SupabaseHelper.Get("cabang_master",
                $"id=eq.{item.IdCloud}&select=id,version")
            If existing.Count = 0 Then
                DoInsertCabang(item, data)
                Return
            End If
            Dim versionCloud As Integer = existing(0)("version").Value(Of Integer)()
            DoUpdateCabang(item, data, item.IdCloud, versionCloud)
        End If
    End Sub

    Private Shared Sub DoInsertCabang(item As SyncQueueItem, data As JObject)
        Dim kodeCabang As String = item.IdLokal
        Dim existing = SupabaseHelper.Get("cabang_master",
            $"kode_cabang=eq.{Uri.EscapeDataString(kodeCabang)}&select=id,device_id,version")

        If existing.Count > 0 Then
            Dim deviceCloud As String = If(existing(0)("device_id") IsNot Nothing,
                                           existing(0)("device_id").ToString(), "")
            If deviceCloud <> SyncConfig.DeviceId AndAlso Not String.IsNullOrEmpty(deviceCloud) Then
                ' Konflik — cari kode baru yang bebas di cloud
                Dim kodeBaru As String = CariKodeBerikutnyaCloud(kodeCabang)
                If String.IsNullOrEmpty(kodeBaru) Then
                    SyncQueue.SetFailed(item.Id, $"CONFLICT: kode '{kodeCabang}' dipakai toko lain, gagal cari kode baru")
                    SyncLog.Tulis("CONFLICT", "tbl_cabang", kodeCabang, "", "Gagal auto-rename")
                    Return
                End If

                ' Rename lokal
                RenameKodeCabangLokalStatic(kodeCabang, kodeBaru)
                SyncLog.Tulis("UPLOAD", "tbl_cabang", kodeCabang, "",
                              $"Auto-rename konflik: {kodeCabang} → {kodeBaru}")
                Log($"  RENAME cabang [{kodeCabang}] → [{kodeBaru}] (konflik cloud)")

                ' Update item untuk insert dengan kode baru
                item.IdLokal = kodeBaru
                data("kode_cabang") = kodeBaru
                kodeCabang = kodeBaru
            Else
                ' Milik device ini — lakukan update
                Dim idCloud As String = existing(0)("id").ToString()
                Dim versionCloud As Integer = existing(0)("version").Value(Of Integer)()
                DoUpdateCabang(item, data, idCloud, versionCloud)
                Return
            End If
        End If

        ' Insert baru dengan device_id sebagai claim
        data("device_id") = SyncConfig.DeviceId
        Dim result = SupabaseHelper.Post("cabang_master", data)
        Dim idCloudBaru As String = result("id").ToString()

        UpdateIdCloudLokal("tbl_cabang", "kode_cabang", kodeCabang, idCloudBaru)
        SyncQueue.SetDone(item.Id, idCloudBaru)
        SyncLog.Tulis("UPLOAD", "tbl_cabang", kodeCabang, idCloudBaru, "INSERT sukses")
        Log($"  INSERT cabang [{kodeCabang}] → cloud [{idCloudBaru}]")
    End Sub

    ''' <summary>Cari kode cabang berikutnya yang bebas di cloud. Pola: CB-XXXX-NNNN</summary>
    Private Shared Function CariKodeBerikutnyaCloud(kodeAsli As String) As String
        Dim parts() As String = kodeAsli.Split("-"c)
        If parts.Length < 3 Then Return ""
        Dim prefix As String = $"{parts(0)}-{parts(1)}-"

        ' Ambil semua kode dengan prefix ini dari cloud
        Dim kodeCloud As New HashSet(Of String)()
        Try
            Dim rows = SupabaseHelper.Get("cabang_master",
                $"kode_cabang=like.{Uri.EscapeDataString(prefix)}*&select=kode_cabang")
            For Each r In rows
                kodeCloud.Add(r("kode_cabang").ToString().ToUpper())
            Next
        Catch
        End Try

        ' Cari urutan tertinggi lokal
        Dim maxUrutan As Integer = 0
        Using cmd As New MySqlCommand(
            "SELECT kode_cabang FROM tbl_cabang WHERE kode_cabang LIKE @prefix", conn)
            cmd.Parameters.AddWithValue("@prefix", prefix & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim bagian As String = rd(0).ToString().Replace(prefix, "")
                    Dim urutan As Integer
                    If Integer.TryParse(bagian, urutan) AndAlso urutan > maxUrutan Then maxUrutan = urutan
                End While
            End Using
        End Using

        Dim kandidat As Integer = maxUrutan + 1
        Dim batas As Integer = kandidat + 100
        While kandidat <= batas
            Dim kodeBaru As String = $"{prefix}{kandidat.ToString("D4")}"
            If Not kodeCloud.Contains(kodeBaru.ToUpper()) Then
                Dim adaLokal As Boolean = False
                Using cmd As New MySqlCommand(
                    "SELECT COUNT(1) FROM tbl_cabang WHERE kode_cabang = @kode", conn)
                    cmd.Parameters.AddWithValue("@kode", kodeBaru)
                    adaLokal = Convert.ToInt32(cmd.ExecuteScalar()) > 0
                End Using
                If Not adaLokal Then Return kodeBaru
            End If
            kandidat += 1
        End While
        Return ""
    End Function

    Private Shared Sub RenameKodeCabangLokalStatic(kodeAsli As String, kodeBaru As String)
        Using cmd As New MySqlCommand(
            "UPDATE tbl_cabang SET kode_cabang = @baru, is_dirty = 1, version = COALESCE(version,0)+1
             WHERE kode_cabang = @asli", conn)
            cmd.Parameters.AddWithValue("@baru", kodeBaru)
            cmd.Parameters.AddWithValue("@asli", kodeAsli)
            cmd.ExecuteNonQuery()
        End Using
        Using cmd As New MySqlCommand(
            "UPDATE sync_queue SET id_lokal = @baru
             WHERE tabel = 'tbl_cabang' AND id_lokal = @asli AND status = 'pending'", conn)
            cmd.Parameters.AddWithValue("@baru", kodeBaru)
            cmd.Parameters.AddWithValue("@asli", kodeAsli)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Shared Sub DoUpdateCabang(item As SyncQueueItem, data As JObject,
                                       idCloud As String, versionCloud As Integer)
        Dim versionLokal As Integer = If(data("version") IsNot Nothing,
                                         data("version").Value(Of Integer)(), 1)

        If versionLokal < versionCloud Then
            SimpanConflict("tbl_cabang", item.IdLokal, idCloud, versionLokal, versionCloud, data.ToString())
            SyncQueue.SetFailed(item.Id, $"CONFLICT: version lokal {versionLokal} < cloud {versionCloud}")
            SyncLog.Tulis("CONFLICT", "tbl_cabang", item.IdLokal, idCloud,
                          $"Version lokal={versionLokal}, cloud={versionCloud}")
            Log($"  CONFLICT cabang [{item.IdLokal}] — perlu resolusi manual")
            Return
        End If

        data("version") = versionCloud + 1
        data.Remove("id_cloud")
        data.Remove("is_dirty")

        SupabaseHelper.Patch("cabang_master", $"id=eq.{idCloud}", data)
        UpdateVersionLokal("tbl_cabang", "kode_cabang", item.IdLokal, versionCloud + 1)
        SyncQueue.SetDone(item.Id, idCloud)
        SyncLog.Tulis("UPLOAD", "tbl_cabang", item.IdLokal, idCloud, "UPDATE sukses")
        Log($"  UPDATE cabang [{item.IdLokal}] → cloud [{idCloud}]")
    End Sub

#End Region

#Region "DOWNLOAD BARANG"

    ''' <summary>Download perubahan barang dari Supabase sejak last_sync</summary>
    Public Shared Sub SyncDownloadBarang()
        Dim lastSync As String = SyncConfig.LastSyncBarang
        Log($"Download barang sejak {lastSync}...")

        Dim rows = SupabaseHelper.Get("barang_master",
            $"updated_at=gt.{Uri.EscapeDataString(lastSync)}&order=updated_at.asc&limit=200")

        If rows.Count = 0 Then
            Log("Tidak ada barang baru dari cloud.")
            Return
        End If

        Log($"Ditemukan {rows.Count} barang dari cloud...")
        Dim diproses As Integer = 0

        For Each row As JObject In rows
            Try
                Dim idBarang As String = row("id_barang").ToString()
                Dim idCloud As String = row("id").ToString()
                Dim versionCloud As Integer = row("version").Value(Of Integer)()

                ' Cek apakah sudah ada lokal
                Dim existLokal As Boolean = False
                Dim versionLokal As Integer = 0
                Using cmd As New MySqlCommand(
                    "SELECT version FROM tbl_barang WHERE ID_BARANG = @id OR id_cloud = @idCloud LIMIT 1", conn)
                    cmd.Parameters.AddWithValue("@id", idBarang)
                    cmd.Parameters.AddWithValue("@idCloud", idCloud)
                    Dim val = cmd.ExecuteScalar()
                    If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                        existLokal = True
                        versionLokal = Convert.ToInt32(val)
                    End If
                End Using

                If Not existLokal Then
                    InsertBarangLokal(row, idCloud)
                    SyncLog.Tulis("DOWNLOAD", "tbl_barang", idBarang, idCloud, "INSERT dari cloud")
                ElseIf versionCloud > versionLokal Then
                    UpdateBarangLokal(row, idBarang, idCloud)
                    SyncLog.Tulis("DOWNLOAD", "tbl_barang", idBarang, idCloud, "UPDATE dari cloud")
                End If
                diproses += 1
            Catch ex As Exception
                SyncLog.Tulis("ERROR", "tbl_barang", "", "", "Download error: " & ex.Message)
            End Try
        Next

        SyncConfig.UpdateLastSyncBarang()
        Log($"Download selesai: {diproses} barang diproses.")
    End Sub

    Private Shared Sub InsertBarangLokal(row As JObject, idCloud As String)
        Using cmd As New MySqlCommand(
            "INSERT IGNORE INTO tbl_barang
             (ID_BARANG, NAMA_BARANG, JENIS, KODE_KATEGORI, NAMA_KATEGORI,
              KODE_SUPLIYER, NAMA_SUPLIYER, JENIS_SATUAN, HARGA_BELI,
              BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR,
              SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR,
              ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR,
              HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR,
              id_cloud, version, is_dirty, updated_by)
             VALUES
             (@id, @nama, @jenis, @kodeKat, @namaKat,
              @kodeSup, @namaSup, @jenisSat, @hargaBeli,
              @bkecil, @bsedang, @bbesar,
              @skecil, @ssedang, @sbesar,
              @ikecil, @isedang, @ibesar,
              @hjkecil, @hjsedang, @hjbesar,
              @idCloud, @version, 0, @updatedBy)", conn)
            MapParamBarang(cmd, row, idCloud)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Shared Sub UpdateBarangLokal(row As JObject, idBarang As String, idCloud As String)
        Using cmd As New MySqlCommand(
            "UPDATE tbl_barang SET
             NAMA_BARANG = @nama, JENIS = @jenis,
             KODE_KATEGORI = @kodeKat, NAMA_KATEGORI = @namaKat,
             KODE_SUPLIYER = @kodeSup, NAMA_SUPLIYER = @namaSup,
             JENIS_SATUAN = @jenisSat, HARGA_BELI = @hargaBeli,
             BARCODE_KECIL = @bkecil, BARCODE_SEDANG = @bsedang, BARCODE_BESAR = @bbesar,
             SATUAN_UMUM_KECIL = @skecil, SATUAN_UMUM_SEDANG = @ssedang, SATUAN_UMUM_BESAR = @sbesar,
             ISI_UMUM_KECIL = @ikecil, ISI_UMUM_SEDANG = @isedang, ISI_UMUM_BESAR = @ibesar,
             HARGA_JUAL_UMUM_KECIL = @hjkecil, HARGA_JUAL_UMUM_SEDANG = @hjsedang,
             HARGA_JUAL_UMUM_BESAR = @hjbesar,
             id_cloud = @idCloud, version = @version, is_dirty = 0, updated_by = @updatedBy
             WHERE ID_BARANG = @id OR id_cloud = @idCloud", conn)
            MapParamBarang(cmd, row, idCloud)
            cmd.Parameters.AddWithValue("@id", idBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Shared Sub MapParamBarang(cmd As MySqlCommand, row As JObject, idCloud As String)
        Dim g = Function(key As String) As String
                    Return If(row(key) IsNot Nothing AndAlso row(key).Type <> JTokenType.Null,
                              row(key).ToString(), "")
                End Function
        Dim gd = Function(key As String) As Decimal
                     Dim v As String = g(key)
                     Dim d As Decimal
                     Return If(Decimal.TryParse(v, d), d, 0)
                 End Function
        Dim gi = Function(key As String) As Integer
                     Dim v As String = g(key)
                     Dim i As Integer
                     Return If(Integer.TryParse(v, i), i, 0)
                 End Function

        cmd.Parameters.AddWithValue("@id", g("id_barang").Trim())
        cmd.Parameters.AddWithValue("@nama", g("nama_barang").Trim())
        cmd.Parameters.AddWithValue("@jenis", g("jenis"))
        cmd.Parameters.AddWithValue("@kodeKat", g("kode_kategori"))
        cmd.Parameters.AddWithValue("@namaKat", g("nama_kategori"))
        cmd.Parameters.AddWithValue("@kodeSup", g("kode_supliyer"))
        cmd.Parameters.AddWithValue("@namaSup", g("nama_supliyer"))
        cmd.Parameters.AddWithValue("@jenisSat", g("jenis_satuan"))
        cmd.Parameters.AddWithValue("@hargaBeli", gd("harga_beli"))
        cmd.Parameters.AddWithValue("@bkecil", g("barcode_kecil").Trim())
        cmd.Parameters.AddWithValue("@bsedang", g("barcode_sedang").Trim())
        cmd.Parameters.AddWithValue("@bbesar", g("barcode_besar").Trim())
        cmd.Parameters.AddWithValue("@skecil", g("satuan_umum_kecil"))
        cmd.Parameters.AddWithValue("@ssedang", g("satuan_umum_sedang"))
        cmd.Parameters.AddWithValue("@sbesar", g("satuan_umum_besar"))
        cmd.Parameters.AddWithValue("@ikecil", gi("isi_umum_kecil"))
        cmd.Parameters.AddWithValue("@isedang", gi("isi_umum_sedang"))
        cmd.Parameters.AddWithValue("@ibesar", gi("isi_umum_besar"))
        cmd.Parameters.AddWithValue("@hjkecil", gd("harga_jual_umum_kecil"))
        cmd.Parameters.AddWithValue("@hjsedang", gd("harga_jual_umum_sedang"))
        cmd.Parameters.AddWithValue("@hjbesar", gd("harga_jual_umum_besar"))
        cmd.Parameters.AddWithValue("@idCloud", idCloud)
        cmd.Parameters.AddWithValue("@version", gi("version"))
        cmd.Parameters.AddWithValue("@updatedBy", g("updated_by"))
    End Sub

    ''' <summary>
    ''' Download generic untuk tabel master sederhana (kategori, satuan, merk, armada, dll).
    ''' Cocokkan berdasarkan kolom PK lokal (kode).
    ''' </summary>
    Public Shared Sub SyncDownloadMaster(tabelCloud As String, tabelLokal As String,
                                          pkLokal As String, lastSync As String,
                                          updateLastSync As Action)
        Log($"Download {tabelLokal} sejak {lastSync}...")
        Try
            Dim rows = SupabaseHelper.Get(tabelCloud,
                $"updated_at=gt.{Uri.EscapeDataString(lastSync)}&order=updated_at.asc&limit=500")

            If rows.Count = 0 Then
                Log($"  Tidak ada {tabelLokal} baru dari cloud.")
                updateLastSync()
                Return
            End If

            Log($"  Ditemukan {rows.Count} baris dari cloud...")
            For Each row As JObject In rows
                Try
                    ' Ambil semua kolom kecuali kolom cloud-only
                    Dim kode As String = ""
                    For Each prop In row.Properties()
                        If prop.Name.ToLower() = "kode" Then
                            kode = prop.Value.ToString()
                            Exit For
                        End If
                    Next
                    If String.IsNullOrEmpty(kode) Then Continue For

                    Dim idCloud As String = row("id").ToString()
                    Dim versionCloud As Integer = If(row("version") IsNot Nothing, row("version").Value(Of Integer)(), 1)

                    ' Cek lokal
                    Dim versionLokal As Integer = -1
                    Using cmd As New MySqlCommand(
                        $"SELECT COALESCE(version, 0) FROM `{tabelLokal}` WHERE `{pkLokal}` = @kode OR id_cloud = @idCloud LIMIT 1", conn)
                        cmd.Parameters.AddWithValue("@kode", kode)
                        cmd.Parameters.AddWithValue("@idCloud", idCloud)
                        Dim val = cmd.ExecuteScalar()
                        If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                            versionLokal = Convert.ToInt32(val)
                        End If
                    End Using

                    ' Bangun SET clause dari kolom yang ada di row (kecuali kolom cloud-only)
                    Dim skipCols As New HashSet(Of String)({"id", "updated_at", "updated_by", "version", "kode_cabang_asal"})
                    Dim setCols As New List(Of String)
                    Dim paramList As New Dictionary(Of String, Object)

                    For Each prop In row.Properties()
                        If skipCols.Contains(prop.Name.ToLower()) Then Continue For
                        Dim colName As String = prop.Name.ToUpper()
                        Dim paramName As String = "@p_" & prop.Name.ToLower().Replace("_", "")
                        setCols.Add($"`{colName}` = {paramName}")
                        paramList(paramName) = If(prop.Value.Type = JTokenType.Null, DBNull.Value, CObj(prop.Value.ToString()))
                    Next
                    setCols.Add("`id_cloud` = @p_idcloud")
                    setCols.Add("`version` = @p_version")
                    setCols.Add("`is_dirty` = 0")
                    paramList("@p_idcloud") = idCloud
                    paramList("@p_version") = versionCloud

                    If versionLokal = -1 Then
                        ' INSERT
                        Dim colNames As String = String.Join(", ", paramList.Keys.Select(Function(k) "`" & k.Replace("@p_", "").ToUpper() & "`"))
                        Dim paramNames As String = String.Join(", ", paramList.Keys)
                        ' Pakai INSERT IGNORE agar tidak duplikat
                        Using cmd As New MySqlCommand(
                            $"INSERT IGNORE INTO `{tabelLokal}` ({colNames}) VALUES ({paramNames})", conn)
                            For Each kv In paramList
                                cmd.Parameters.AddWithValue(kv.Key, kv.Value)
                            Next
                            cmd.ExecuteNonQuery()
                        End Using
                        SyncLog.Tulis("DOWNLOAD", tabelLokal, kode, idCloud, "INSERT dari cloud")
                    ElseIf versionCloud > versionLokal Then
                        ' UPDATE
                        Dim setClause As String = String.Join(", ", setCols)
                        Using cmd As New MySqlCommand(
                            $"UPDATE `{tabelLokal}` SET {setClause} WHERE `{pkLokal}` = @p_kode OR id_cloud = @p_idcloud2", conn)
                            For Each kv In paramList
                                cmd.Parameters.AddWithValue(kv.Key, kv.Value)
                            Next
                            cmd.Parameters.AddWithValue("@p_kode", kode)
                            cmd.Parameters.AddWithValue("@p_idcloud2", idCloud)
                            cmd.ExecuteNonQuery()
                        End Using
                        SyncLog.Tulis("DOWNLOAD", tabelLokal, kode, idCloud, "UPDATE dari cloud")
                    End If
                Catch ex As Exception
                    SyncLog.Tulis("ERROR", tabelLokal, "", "", "Download row error: " & ex.Message)
                End Try
            Next

            updateLastSync()
            Log($"  {tabelLokal} selesai diproses.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", tabelLokal, "", "", "SyncDownloadMaster error: " & ex.Message)
            Log($"  ERROR download {tabelLokal}: {ex.Message}")
        End Try
    End Sub

    ''' <summary>Download cabang dari Supabase</summary>
    Public Shared Sub SyncDownloadCabang()
        Dim lastSync As String = SyncConfig.LastSyncCabang
        Log($"Download tbl_cabang sejak {lastSync}...")

        Try
            Dim query As String = "select=kode_cabang,nama_cabang,alamat,kota,hp,pemilik,id,version&order=updated_at.asc&limit=500"
            If Not String.IsNullOrEmpty(lastSync) Then
                query &= $"&updated_at=gt.{Uri.EscapeDataString(lastSync)}"
            End If

            Dim rows = SupabaseHelper.Get("cabang_master", query)

            If rows.Count = 0 Then
                Log("  Tidak ada cabang baru dari cloud.")
                SyncConfig.UpdateLastSyncCabang()
                Return
            End If

            Log($"  Ditemukan {rows.Count} cabang dari cloud...")
            Dim diproses As Integer = 0

            For Each row As JObject In rows
                Try
                    Dim kode As String = row("kode_cabang").ToString()
                    Dim idCloud As String = row("id").ToString()
                    Dim versionCloud As Integer = If(row("version") IsNot Nothing, row("version").Value(Of Integer)(), 1)

                    ' Cek lokal
                    Dim versionLokal As Integer = 0
                    Using cmd As New MySqlCommand(
                        "SELECT COALESCE(version, 0) FROM tbl_cabang WHERE kode_cabang = @kode OR id_cloud = @idCloud LIMIT 1", conn)
                        cmd.Parameters.AddWithValue("@kode", kode)
                        cmd.Parameters.AddWithValue("@idCloud", idCloud)
                        Dim val = cmd.ExecuteScalar()
                        If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                            versionLokal = Convert.ToInt32(val)
                        End If
                    End Using

                    If versionLokal = 0 Then
                        ' INSERT
                        Using cmd As New MySqlCommand(
                            "INSERT IGNORE INTO tbl_cabang
                             (kode_cabang, nama_cabang, alamat, kota, hp, pemilik, sumber, id_cloud, version, is_dirty)
                             VALUES (@kode, @nama, @alamat, @kota, @hp, @pemilik, 'cloud', @idCloud, @version, 0)", conn)
                            MapParamCabangSync(cmd, row, idCloud, versionCloud)
                            cmd.ExecuteNonQuery()
                        End Using
                        SyncLog.Tulis("DOWNLOAD", "tbl_cabang", kode, idCloud, "INSERT dari cloud")
                    ElseIf versionCloud > versionLokal Then
                        ' UPDATE
                        Using cmd As New MySqlCommand(
                            "UPDATE tbl_cabang SET
                             nama_cabang = @nama, alamat = @alamat, kota = @kota, hp = @hp, pemilik = @pemilik,
                             sumber = 'cloud', id_cloud = @idCloud, version = @version, is_dirty = 0
                             WHERE kode_cabang = @kode", conn)
                            MapParamCabangSync(cmd, row, idCloud, versionCloud)
                            cmd.Parameters.AddWithValue("@kode", kode)
                            cmd.ExecuteNonQuery()
                        End Using
                        SyncLog.Tulis("DOWNLOAD", "tbl_cabang", kode, idCloud, "UPDATE dari cloud")
                    End If
                    diproses += 1
                Catch ex As Exception
                    SyncLog.Tulis("ERROR", "tbl_cabang", "", "", "Download row error: " & ex.Message)
                End Try
            Next

            SyncConfig.UpdateLastSyncCabang()
            Log($"  Download cabang selesai: {diproses} diproses.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "tbl_cabang", "", "", "SyncDownloadCabang error: " & ex.Message)
            Log($"  ERROR download cabang: {ex.Message}")
        End Try
    End Sub

    Private Shared Sub MapParamCabangSync(cmd As MySqlCommand, row As JObject, idCloud As String, version As Integer)
        Dim g = Function(key As String) As String
                    Return If(row(key) IsNot Nothing AndAlso row(key).Type <> JTokenType.Null,
                              row(key).ToString(), "")
                End Function
        cmd.Parameters.AddWithValue("@kode", g("kode_cabang"))
        cmd.Parameters.AddWithValue("@nama", g("nama_cabang"))
        cmd.Parameters.AddWithValue("@alamat", g("alamat"))
        cmd.Parameters.AddWithValue("@kota", g("kota"))
        cmd.Parameters.AddWithValue("@hp", g("hp"))
        cmd.Parameters.AddWithValue("@pemilik", g("pemilik"))
        cmd.Parameters.AddWithValue("@idCloud", idCloud)
        cmd.Parameters.AddWithValue("@version", version)
    End Sub

#End Region

#Region "TRANSFER"

    ''' <summary>Kirim transfer barang ke Supabase. Return True jika berhasil.</summary>
    Public Shared Function KirimTransfer(kodeBarang As String, namaBarang As String,
                                          keToko As String, qty As Decimal, satuan As String,
                                          isiSatuan As Integer, keterangan As String,
                                          idUser As String) As Boolean
        If Not SupabaseHelper.IsInitialized() OrElse Not SupabaseHelper.CekKoneksi() Then
            SyncLog.Tulis("ERROR", "transfer", kodeBarang, "", "Tidak ada koneksi saat kirim transfer")
            Return False
        End If

        Dim payload As New Dictionary(Of String, Object) From {
            {"dari_toko", SyncConfig.KodeCabang},
            {"ke_toko", keToko},
            {"kode_barang", kodeBarang},
            {"nama_barang", namaBarang},
            {"qty", qty},
            {"satuan", satuan},
            {"isi_satuan", isiSatuan},
            {"qty_satuan", qty * isiSatuan},
            {"keterangan", keterangan},
            {"status", "pending"},
            {"id_user_kirim", idUser}
        }

        Try
            Dim result = SupabaseHelper.Post("transfer_barang_cloud", payload)
            SyncLog.Tulis("UPLOAD", "transfer", kodeBarang, result("id").ToString(),
                          $"Transfer ke {keToko} qty={qty}")
            Return True
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "transfer", kodeBarang, "", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>Upload semua transfer offline pending ke Supabase</summary>
    Public Shared Sub UploadTransferOffline()
        If Not SupabaseHelper.IsInitialized() OrElse Not SupabaseHelper.CekKoneksi() Then Return

        Try
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT id, id_transfer, ke_cabang, kode_barang, nama_barang,
                        qty, satuan, isi_satuan, qty_satuan, keterangan
                 FROM transfer_keluar_offline WHERE status = 'PENDING' ORDER BY id ASC", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

            If dt.Rows.Count = 0 Then Return
            Log($"Upload {dt.Rows.Count} transfer offline pending...")

            For Each row As DataRow In dt.Rows
                Try
                    Dim ok = KirimTransfer(
                        row("kode_barang").ToString(),
                        row("nama_barang").ToString(),
                        row("ke_cabang").ToString(),
                        Convert.ToDecimal(row("qty")),
                        row("satuan").ToString(),
                        Convert.ToInt32(row("isi_satuan")),
                        row("keterangan").ToString(),
                        SyncConfig.GetNilai("last_user"))
                    If ok Then
                        Using cmd As New MySqlCommand(
                            "UPDATE transfer_keluar_offline SET status='TERKIRIM' WHERE id=@id", conn)
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(row("id")))
                            cmd.ExecuteNonQuery()
                        End Using
                        Using cmd As New MySqlCommand(
                            "UPDATE transfer_cabang SET STATUS_TRANSFER='TERKIRIM' WHERE ID_TRANSFER=@id", conn)
                            cmd.Parameters.AddWithValue("@id", row("id_transfer").ToString())
                            cmd.ExecuteNonQuery()
                        End Using
                        Log($"  Transfer offline [{row("id_transfer")}] → cloud sukses")
                    End If
                Catch ex As Exception
                    SyncLog.Tulis("ERROR", "transfer_offline", row("kode_barang").ToString(), "", ex.Message)
                End Try
            Next
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "transfer_offline", "", "", "UploadTransferOffline: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Download transfer masuk untuk cabang ini</summary>
    Public Shared Sub SyncDownloadTransfer()
        Dim lastSync As String = SyncConfig.LastSyncTransfer
        Dim kodeCabang As String = SyncConfig.KodeCabang
        Log($"Download transfer masuk untuk cabang {kodeCabang}...")

        Dim rows = SupabaseHelper.Get("transfer_barang_cloud",
            $"ke_toko=eq.{Uri.EscapeDataString(kodeCabang)}&status=eq.pending" &
            $"&updated_at=gt.{Uri.EscapeDataString(lastSync)}&order=tgl_kirim.asc")

        If rows.Count = 0 Then
            Log("Tidak ada transfer masuk.")
            SyncConfig.UpdateLastSyncTransfer()
            Return
        End If

        Log($"Ada {rows.Count} transfer masuk.")
        ' Simpan ke tabel lokal untuk ditampilkan di form
        For Each row As JObject In rows
            Try
                SimpanTransferMasukLokal(row)
            Catch ex As Exception
                SyncLog.Tulis("ERROR", "transfer", "", "", "Download transfer error: " & ex.Message)
            End Try
        Next

        SyncConfig.UpdateLastSyncTransfer()
    End Sub

    Private Shared Sub SimpanTransferMasukLokal(row As JObject)
        ' Simpan ke tabel transfer_masuk_manual agar bisa diterima saat offline
        Try
            Dim g = Function(key As String) As String
                        Return If(row(key) IsNot Nothing AndAlso row(key).Type <> JTokenType.Null,
                                  row(key).ToString(), "")
                    End Function
            Dim idCloud As String = g("id")
            Dim idTransfer As String = If(Not String.IsNullOrEmpty(g("id")), "CLD-" & g("id").Substring(0, 8).ToUpper(), "")
            Dim kodeBarang As String = g("kode_barang")
            Dim qty As Decimal = If(row("qty") IsNot Nothing, row("qty").Value(Of Decimal)(), 0D)
            Dim qtySatuan As Decimal = If(row("qty_satuan") IsNot Nothing, row("qty_satuan").Value(Of Decimal)(), qty)

            Using cmd As New MySqlCommand(
                "INSERT INTO transfer_masuk_manual
                 (id_transfer, sumber_transfer, dari_cabang, ke_cabang, kode_barang, nama_barang,
                  qty, satuan, isi_satuan, qty_satuan, keterangan, tgl_kirim, status_transfer)
                 VALUES (@id, 'CLOUD', @dari, @ke, @kode, @nama, @qty, @sat, @isi, @qtySat, @ket, @tglKirim, 'PENDING')
                 ON DUPLICATE KEY UPDATE
                   qty_satuan      = VALUES(qty_satuan),
                   tgl_kirim       = VALUES(tgl_kirim),
                   status_transfer = IF(status_transfer='DITERIMA','DITERIMA','PENDING')", conn)
                cmd.Parameters.AddWithValue("@id", If(String.IsNullOrEmpty(idTransfer), idCloud, idTransfer))
                cmd.Parameters.AddWithValue("@dari", g("dari_toko"))
                cmd.Parameters.AddWithValue("@ke", g("ke_toko"))
                cmd.Parameters.AddWithValue("@kode", kodeBarang)
                cmd.Parameters.AddWithValue("@nama", g("nama_barang"))
                cmd.Parameters.AddWithValue("@qty", qty)
                cmd.Parameters.AddWithValue("@sat", g("satuan"))
                cmd.Parameters.AddWithValue("@isi", If(row("isi_satuan") IsNot Nothing, row("isi_satuan").Value(Of Integer)(), 1))
                cmd.Parameters.AddWithValue("@qtySat", qtySatuan)
                cmd.Parameters.AddWithValue("@ket", g("keterangan"))
                cmd.Parameters.AddWithValue("@tglKirim", If(row("tgl_kirim") IsNot Nothing AndAlso row("tgl_kirim").Type <> JTokenType.Null,
                                                            row("tgl_kirim").ToString(),
                                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
                cmd.ExecuteNonQuery()
            End Using

            SyncLog.Tulis("DOWNLOAD", "transfer", kodeBarang, idCloud,
                          $"Transfer masuk dari {g("dari_toko")} qty={qtySatuan} — disimpan lokal")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "transfer", "", "", "SimpanTransferMasukLokal: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Terima transfer — tambah stok lokal dan update status di Supabase</summary>
    Public Shared Function TerimaTransfer(idCloud As String, kodeBarang As String,
                                           qtySatuan As Decimal, idUser As String) As Boolean
        Try
            ' Update counter keluar lokal (STOK_TOKO dihitung ulang oleh sp_hlp_stok_hitung)
            Using trx = conn.BeginTransaction()
                Using cmd As New MySqlCommand(
                    "UPDATE tbl_barang SET
                     TRANSFER_CABANG_MASUK_TOKO = TRANSFER_CABANG_MASUK_TOKO + @qty,
                     is_dirty = 1, version = version + 1, updated_by = @user
                     WHERE ID_BARANG = @kode", conn, trx)
                    cmd.Parameters.AddWithValue("@qty", qtySatuan)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    cmd.Parameters.AddWithValue("@user", idUser)
                    cmd.ExecuteNonQuery()
                End Using
                ' Task 8.2 — ganti HitungStokPerubahan dengan sp_hlp_stok_hitung
                Using cmdSp As New MySqlCommand("CALL sp_hlp_stok_hitung(@kode)", conn, trx)
                    cmdSp.Parameters.AddWithValue("@kode", kodeBarang)
                    cmdSp.ExecuteNonQuery()
                End Using
                trx.Commit()
            End Using

            ' Update status di Supabase
            If SupabaseHelper.CekKoneksi() Then
                SupabaseHelper.Patch("transfer_barang_cloud", $"id=eq.{idCloud}",
                    New Dictionary(Of String, Object) From {
                        {"status", "diterima"},
                        {"id_user_terima", idUser},
                        {"tgl_terima", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}
                    })
            End If

            SyncLog.Tulis("UPLOAD", "transfer", kodeBarang, idCloud, "Transfer diterima")
            Return True
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "transfer", kodeBarang, idCloud, "TerimaTransfer: " & ex.Message)
            Return False
        End Try
    End Function

#End Region

#Region "HELPER"

    Private Shared Sub UpdateIdCloudLokal(tabel As String, pkField As String,
                                           idLokal As String, idCloud As String)
        Using cmd As New MySqlCommand(
            $"UPDATE {tabel} SET id_cloud = @idCloud, is_dirty = 0 WHERE {pkField} = @idLokal", conn)
            cmd.Parameters.AddWithValue("@idCloud", idCloud)
            cmd.Parameters.AddWithValue("@idLokal", idLokal)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Shared Sub UpdateVersionLokal(tabel As String, pkField As String,
                                           idLokal As String, version As Integer)
        Using cmd As New MySqlCommand(
            $"UPDATE {tabel} SET version = @version, is_dirty = 0 WHERE {pkField} = @idLokal", conn)
            cmd.Parameters.AddWithValue("@version", version)
            cmd.Parameters.AddWithValue("@idLokal", idLokal)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>Upload stok toko + gudang lengkap ke stok_per_cabang</summary>
    Private Shared Sub UploadStokPerCabang(idBarang As String, _dataBarang As JObject)
        Try
            ' Ambil semua kolom stok dari lokal
            Dim payload As New Dictionary(Of String, Object)
            Using cmd As New MySqlCommand(
                "SELECT NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, NAMA_SUPLIYER,
                        AWAL_TOKO, TAMBAH_TOKO, KURANG_TOKO, PEMBELIAN_TOKO, PENJUALAN_TOKO,
                        RETUR_BELI_TOKO, RETUR_JUAL_TOKO, OPNAME_TOKO,
                        TRANSFER_STOK_MASUK_TOKO, TRANSFER_STOK_KELUAR_TOKO,
                        TRANSFER_BARANG_MASUK_TOKO, TRANSFER_BARANG_KELUAR_TOKO,
                        TRANSFER_CABANG_MASUK_TOKO, TRANSFER_CABANG_KELUAR_TOKO, STOK_TOKO,
                        AWAL_GUDANG, TAMBAH_GUDANG, KURANG_GUDANG, PEMBELIAN_GUDANG, PENJUALAN_GUDANG,
                        RETUR_BELI_GUDANG, RETUR_JUAL_GUDANG, OPNAME_GUDANG,
                        TRANSFER_STOK_MASUK_GUDANG, TRANSFER_STOK_KELUAR_GUDANG,
                        TRANSFER_BARANG_MASUK_GUDANG, TRANSFER_BARANG_KELUAR_GUDANG,
                        TRANSFER_CABANG_MASUK_GUDANG, TRANSFER_CABANG_KELUAR_GUDANG, STOK_GUDANG,
                        HARGA_BELI, HARGA_BELI_TERAKHIR,
                        HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR,
                        HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR,
                        STOK_MIN, STOK_MAX
                 FROM tbl_barang WHERE ID_BARANG = @id LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@id", idBarang)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then Return
                    payload("kode_cabang") = SyncConfig.KodeCabang
                    payload("id_barang") = idBarang
                    For i As Integer = 0 To rd.FieldCount - 1
                        Dim colName As String = rd.GetName(i).ToLower()
                        payload(colName) = If(rd.IsDBNull(i), CObj(0), rd.GetValue(i))
                    Next
                End Using
            End Using

            SupabaseHelper.PostUpsert("stok_per_cabang", payload)
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "stok_per_cabang", idBarang, "", "UploadStokPerCabang: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Upload hutang supplier ke cloud untuk laporan</summary>
    Public Shared Sub UploadSnapshotHutangSupliyer()
        Try
            Log("  Upload hutang supplier...")
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT KODE, NAMA, ALAMAT, HP, JANGKAHUTANG, HUTANGAWAL, TOTALHUTANG, TOTALBAYAR, HUTANGAKHIR FROM tbl_supliyer", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            For Each row As DataRow In dt.Rows
                Dim payload As New Dictionary(Of String, Object) From {
                    {"kode_cabang", SyncConfig.KodeCabang},
                    {"kode", row("KODE").ToString()},
                    {"nama", row("NAMA").ToString()},
                    {"alamat", row("ALAMAT").ToString()},
                    {"hp", row("HP").ToString()},
                    {"jangkahutang", Convert.ToInt32(row("JANGKAHUTANG"))},
                    {"hutangawal", Convert.ToDecimal(row("HUTANGAWAL"))},
                    {"totalhutang", Convert.ToDecimal(row("TOTALHUTANG"))},
                    {"totalbayar", Convert.ToDecimal(row("TOTALBAYAR"))},
                    {"hutangakhir", Convert.ToDecimal(row("HUTANGAKHIR"))}
                }
                SupabaseHelper.PostUpsert("hutang_supliyer_snapshot", payload)
            Next
            Log($"  {dt.Rows.Count} supplier diupload.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "hutang_supliyer_snapshot", "", "", ex.Message)
        End Try
    End Sub

    ''' <summary>Upload piutang pelanggan ke cloud untuk laporan</summary>
    Public Shared Sub UploadSnapshotPiutangPelanggan()
        Try
            Log("  Upload piutang pelanggan...")
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT KODE, NAMA, ALAMAT, NO_TELP, JENIS, JANGKAPIUTANG, HUTANGAWAL, TOTALHUTANG, TOTALBAYAR, HUTANGAKHIR FROM tbl_pelanggan", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            For Each row As DataRow In dt.Rows
                Dim payload As New Dictionary(Of String, Object) From {
                    {"kode_cabang", SyncConfig.KodeCabang},
                    {"kode", row("KODE").ToString()},
                    {"nama", row("NAMA").ToString()},
                    {"alamat", row("ALAMAT").ToString()},
                    {"no_telp", row("NO_TELP").ToString()},
                    {"jenis", row("JENIS").ToString()},
                    {"jangkapiutang", Convert.ToInt32(row("JANGKAPIUTANG"))},
                    {"hutangawal", Convert.ToDecimal(row("HUTANGAWAL"))},
                    {"totalhutang", Convert.ToDecimal(row("TOTALHUTANG"))},
                    {"totalbayar", Convert.ToDecimal(row("TOTALBAYAR"))},
                    {"hutangakhir", Convert.ToDecimal(row("HUTANGAKHIR"))}
                }
                SupabaseHelper.PostUpsert("piutang_pelanggan_snapshot", payload)
            Next
            Log($"  {dt.Rows.Count} pelanggan diupload.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "piutang_pelanggan_snapshot", "", "", ex.Message)
        End Try
    End Sub

    ''' <summary>Upload data karyawan ke cloud untuk laporan</summary>
    Public Shared Sub UploadSnapshotKaryawan()
        Try
            Log("  Upload karyawan...")
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT KODE, NAMA, JABATAN, TGLMASUK, GAJI, SALDOAWAL, TOTALBON, TOTALBAYAR, SALDOAKHIR FROM tbl_karyawan", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            For Each row As DataRow In dt.Rows
                Dim payload As New Dictionary(Of String, Object) From {
                    {"kode_cabang", SyncConfig.KodeCabang},
                    {"kode", row("KODE").ToString()},
                    {"nama", row("NAMA").ToString()},
                    {"jabatan", row("JABATAN").ToString()},
                    {"tglmasuk", If(row.IsNull("TGLMASUK"), CObj(DBNull.Value), CObj(Convert.ToDateTime(row("TGLMASUK")).ToString("yyyy-MM-ddTHH:mm:ssZ")))},
                    {"gaji", Convert.ToDecimal(row("GAJI"))},
                    {"saldoawal", Convert.ToDecimal(row("SALDOAWAL"))},
                    {"totalbon", Convert.ToDecimal(row("TOTALBON"))},
                    {"totalbayar", Convert.ToDecimal(row("TOTALBAYAR"))},
                    {"saldoakhir", Convert.ToDecimal(row("SALDOAKHIR"))}
                }
                SupabaseHelper.PostUpsert("karyawan_snapshot", payload)
            Next
            Log($"  {dt.Rows.Count} karyawan diupload.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "karyawan_snapshot", "", "", ex.Message)
        End Try
    End Sub

    ''' <summary>Upload ringkasan gaji bulan terakhir ke cloud</summary>
    Public Shared Sub UploadSnapshotGaji()
        Try
            Log("  Upload ringkasan gaji...")
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT NOMOR, BULAN, TANGGAL, KODE, NAMA, POKOK, PENDAPATAN, POTONGAN, TERIMA, LOKASI
                 FROM gaji_karyawan
                 WHERE BULAN = (SELECT MAX(BULAN) FROM gaji_karyawan)", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            For Each row As DataRow In dt.Rows
                Dim payload As New Dictionary(Of String, Object) From {
                    {"kode_cabang", SyncConfig.KodeCabang},
                    {"bulan", row("BULAN").ToString()},
                    {"kode", row("KODE").ToString()},
                    {"nama", row("NAMA").ToString()},
                    {"pokok", Convert.ToDecimal(row("POKOK"))},
                    {"pendapatan", Convert.ToDecimal(row("PENDAPATAN"))},
                    {"potongan", Convert.ToDecimal(row("POTONGAN"))},
                    {"terima", Convert.ToDecimal(row("TERIMA"))},
                    {"tanggal", If(row.IsNull("TANGGAL"), CObj(DBNull.Value), CObj(Convert.ToDateTime(row("TANGGAL")).ToString("yyyy-MM-ddTHH:mm:ssZ")))},
                    {"lokasi", row("LOKASI").ToString()}
                }
                SupabaseHelper.PostUpsert("gaji_ringkasan_snapshot", payload)
            Next
            Log($"  {dt.Rows.Count} data gaji diupload.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "gaji_ringkasan_snapshot", "", "", ex.Message)
        End Try
    End Sub

    ''' <summary>Upload COA / tbl_datareferensi ke cloud untuk laporan neraca &amp; laba rugi</summary>
    Public Shared Sub UploadSnapshotCOA()
        Try
            Log("  Upload COA / akun...")
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN,
                        AKUN_DK, AKUN_NRLR, SALDO_AWAL, SALDO_SEBELUMNYA, S_DEBET, S_KREDIT, SALDO_AKHIR
                 FROM tbl_datareferensi", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
            For Each row As DataRow In dt.Rows
                Dim payload As New Dictionary(Of String, Object) From {
                    {"kode_cabang", SyncConfig.KodeCabang},
                    {"kode_akun", row("KODE_AKUN").ToString()},
                    {"status", If(row.IsNull("STATUS"), "", row("STATUS").ToString())},
                    {"jenis_akun", row("JENIS_AKUN").ToString()},
                    {"type_akun", row("TYPE_AKUN").ToString()},
                    {"nama_akun", row("NAMA_AKUN").ToString()},
                    {"sub_akun", If(row.IsNull("SUB_AKUN"), "", row("SUB_AKUN").ToString())},
                    {"akun_dk", If(row.IsNull("AKUN_DK"), "", row("AKUN_DK").ToString())},
                    {"akun_nrlr", If(row.IsNull("AKUN_NRLR"), "", row("AKUN_NRLR").ToString())},
                    {"saldo_awal", Convert.ToDecimal(row("SALDO_AWAL"))},
                    {"saldo_sebelumnya", Convert.ToDecimal(row("SALDO_SEBELUMNYA"))},
                    {"s_debet", Convert.ToDecimal(row("S_DEBET"))},
                    {"s_kredit", Convert.ToDecimal(row("S_KREDIT"))},
                    {"saldo_akhir", Convert.ToDecimal(row("SALDO_AKHIR"))}
                }
                SupabaseHelper.PostUpsert("coa_snapshot", payload)
            Next
            Log($"  {dt.Rows.Count} akun COA diupload.")
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "coa_snapshot", "", "", ex.Message)
        End Try
    End Sub

    Private Shared Sub SimpanConflict(tabel As String, idLokal As String, idCloud As String,
                                       vLokal As Integer, vCloud As Integer, payload As String)
        Try
            If SupabaseHelper.CekKoneksi() Then
                SupabaseHelper.Post("sync_conflict_log", New Dictionary(Of String, Object) From {
                    {"tabel", tabel},
                    {"id_lokal", idLokal},
                    {"id_cloud", idCloud},
                    {"kode_cabang", SyncConfig.KodeCabang},
                    {"version_lokal", vLokal},
                    {"version_cloud", vCloud},
                    {"payload_lokal", payload}
                })
            End If
        Catch
            ' Conflict log tidak boleh crash proses utama
        End Try
    End Sub

#End Region

    ''' <summary>
    ''' Validasi kode_cabang ke Supabase sebelum upload pertama.
    ''' Return True = boleh upload. False = kode sudah dipakai cabang lain.
    ''' </summary>
    Public Shared Function ValidasiKodeCabang(ByRef pesanError As String) As Boolean
        Dim kodeCabang As String = SyncConfig.KodeCabang
        Dim deviceId As String = SyncConfig.DeviceId

        If String.IsNullOrEmpty(kodeCabang) Then
            pesanError = "Kode cabang belum diisi. Isi di menu Pengaturan Sync."
            Return False
        End If

        Try
            Dim rows = SupabaseHelper.Get("cabang_master",
                $"kode_cabang=eq.{Uri.EscapeDataString(kodeCabang)}&select=kode_cabang,device_id")

            If rows.Count = 0 Then
                ' Belum ada di cloud — kode bebas dipakai, lanjut
                Return True
            End If

            Dim cloudDeviceId As String = ""
            If rows(0)("device_id") IsNot Nothing AndAlso
               rows(0)("device_id").Type <> Newtonsoft.Json.Linq.JTokenType.Null Then
                cloudDeviceId = rows(0)("device_id").ToString()
            End If

            If String.IsNullOrEmpty(cloudDeviceId) OrElse cloudDeviceId = deviceId Then
                ' Kode ini milik instalasi ini sendiri — aman
                Return True
            End If

            ' Kode sudah diklaim instalasi lain
            pesanError = $"Kode cabang ""{kodeCabang}"" sudah digunakan oleh cabang lain." &
                         Environment.NewLine &
                         "Ganti kode cabang di menu Pengaturan Sync, lalu coba upload lagi."
            Return False

        Catch ex As Exception
            ' Jika tidak bisa cek (misal timeout), izinkan upload agar tidak blokir offline
            SyncLog.Tulis("ERROR", "cabang_master", kodeCabang, "", "ValidasiKodeCabang: " & ex.Message)
            Return True
        End Try
    End Function

    ''' <summary>
    ''' Upload identitas cabang ke cabang_master di Supabase.
    ''' Dipanggil saat FormCompany simpan dan saat UploadSemuaSnapshot.
    ''' Hanya kolom identitas — kode rekening dan konfigurasi lokal tidak diupload.
    ''' </summary>
    Public Shared Sub UploadSnapshotCabang()
        Try
            Log("  Upload identitas cabang...")
            Using cmd As New MySqlCommand(
                "SELECT KODE, KODE_CLOUD, NAMA_CLOUD, ALAMAT_CLOUD, NAMA, ALAMAT, KOTA, HP, PEMILIK FROM tbl_perusahaan LIMIT 1", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then
                        Log("  tbl_perusahaan kosong, skip.")
                        Return
                    End If

                    Dim kodeCloud As String = If(rd.IsDBNull(rd.GetOrdinal("KODE_CLOUD")) OrElse
                                                 String.IsNullOrEmpty(rd.GetString("KODE_CLOUD")),
                                                 SyncConfig.KodeCabang,
                                                 rd.GetString("KODE_CLOUD"))
                    Dim namaCloud As String = If(rd.IsDBNull(rd.GetOrdinal("NAMA_CLOUD")) OrElse
                                                 String.IsNullOrEmpty(rd.GetString("NAMA_CLOUD")),
                                                 rd.GetString("NAMA"),
                                                 rd.GetString("NAMA_CLOUD"))
                    Dim alamatCloud As String = If(rd.IsDBNull(rd.GetOrdinal("ALAMAT_CLOUD")) OrElse
                                                   String.IsNullOrEmpty(rd.GetString("ALAMAT_CLOUD")),
                                                   If(rd.IsDBNull(rd.GetOrdinal("ALAMAT")), "", rd.GetString("ALAMAT")),
                                                   rd.GetString("ALAMAT_CLOUD"))

                    Dim payload As New Dictionary(Of String, Object) From {
                        {"kode_cabang", kodeCloud},
                        {"nama_cabang", namaCloud},
                        {"alamat", alamatCloud},
                        {"kota", If(rd.IsDBNull(rd.GetOrdinal("KOTA")), "", rd.GetString("KOTA"))},
                        {"hp", If(rd.IsDBNull(rd.GetOrdinal("HP")), "", rd.GetString("HP"))},
                        {"pemilik", If(rd.IsDBNull(rd.GetOrdinal("PEMILIK")), "", rd.GetString("PEMILIK"))},
                        {"device_id", SyncConfig.DeviceId},
                        {"claimed_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}
                    }

                    SupabaseHelper.PostUpsert("cabang_master", payload)
                    SyncLog.Tulis("UPLOAD", "cabang_master", kodeCloud, "", "Identitas cabang terupload")
                    Log($"  Cabang [{kodeCloud}] berhasil diupload ke cabang_master.")
                End Using
            End Using
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "cabang_master", SyncConfig.KodeCabang, "", "UploadSnapshotCabang: " & ex.Message)
            Log($"  ERROR upload cabang_master: {ex.Message}")
        End Try
    End Sub

End Class
