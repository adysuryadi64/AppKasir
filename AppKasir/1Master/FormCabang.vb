Public Class FormCabang

    ' ── TODO: AUDIT TRAIL ────────────────────────────────────────────────────
    ' Saat fitur hapus/edit FormCabang ditambahkan, panggil:
    '   ModuleAuditTrail.CatatAuditMaster("CAB:" & kode, "HAPUS"/"EDIT",
    '       "Master cabang", snapshotJson, "[KRITIS] Hapus/Edit data cabang", trans)
    ' Snapshot: baca data lama dari tbl_cabang sebelum DELETE/UPDATE
    ' ─────────────────────────────────────────────────────────────────────────
    Private _isLoading As Boolean = False
    Private _isEditMode As Boolean = False

    Private Sub FormCabang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        EnsureTabelCabangLokal()
        InitSupabase()
        KondisiAwal()
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub EnsureTabelCabangLokal()
        Using cmd As New MySqlCommand(
            "CREATE TABLE IF NOT EXISTS tbl_cabang (
                kode_cabang VARCHAR(50) NOT NULL,
                nama_cabang VARCHAR(100) DEFAULT NULL,
                alamat VARCHAR(200) DEFAULT NULL,
                kota VARCHAR(60) DEFAULT NULL,
                hp VARCHAR(60) DEFAULT NULL,
                pemilik VARCHAR(100) DEFAULT NULL,
                sumber VARCHAR(20) DEFAULT 'manual',
                id_cloud VARCHAR(50) DEFAULT NULL,
                version INT DEFAULT 1,
                is_dirty TINYINT DEFAULT 0,
                sync_id VARCHAR(36) DEFAULT NULL,
                updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                updated_by VARCHAR(50) DEFAULT NULL,
                PRIMARY KEY (kode_cabang)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci", conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InitSupabase()
        Dim url As String = AppConfig.Instance.GetValue(Of String)("SupabaseUrl", "")
        Dim key As String = AppConfig.Instance.GetValue(Of String)("SupabaseKey", "")
        If Not String.IsNullOrEmpty(url) AndAlso Not String.IsNullOrEmpty(key) Then
            SupabaseHelper.Init(url, key)
        End If
    End Sub

#Region "DataGridView"
    Public Sub TampilCabang()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand(
            "SELECT kode_cabang, nama_cabang, alamat, kota, hp, pemilik, sumber, id_cloud, version, is_dirty FROM tbl_cabang ORDER BY kode_cabang", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        DgvCabang.DataSource = Nothing
        DgvCabang.Rows.Clear()
        DgvCabang.Columns.Clear()

        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 70, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama Cabang", .FillWeight = 140, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColAlamat", .HeaderText = "Alamat", .FillWeight = 160, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKota", .HeaderText = "Kota", .FillWeight = 80, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColHp", .HeaderText = "No. HP", .FillWeight = 90, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColPemilik", .HeaderText = "Pemilik", .FillWeight = 100, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColSumber", .HeaderText = "Sumber", .FillWeight = 70, .ReadOnly = True})
        DgvCabang.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColSync", .HeaderText = "Sync", .FillWeight = 50, .ReadOnly = True})

        ' Tombol Edit
        Dim colEdit As New DataGridViewButtonColumn() With {
            .Name = "ColEdit", .HeaderText = "Edit",
            .Text = "✎ Edit", .UseColumnTextForButtonValue = True,
            .Width = 70, .FlatStyle = FlatStyle.Flat,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        }
        ' Font diatur oleh tema - Calibri 9.75 Bold
        DgvCabang.Columns.Add(colEdit)

        ' Tombol Hapus
        Dim colHapus As New DataGridViewButtonColumn() With {
            .Name = "ColHapus", .HeaderText = "Hapus",
            .Text = "✖ Hapus", .UseColumnTextForButtonValue = True,
            .Width = 75, .FlatStyle = FlatStyle.Flat,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        }
        ' Font diatur oleh tema - Calibri 9.75 Bold
        DgvCabang.Columns.Add(colHapus)

        For Each row As DataRow In dt.Rows
            Dim sumber As String = If(row("sumber") Is DBNull.Value, "manual", row("sumber").ToString())
            Dim isDirty As Integer = If(row("is_dirty") Is DBNull.Value, 0, Convert.ToInt32(row("is_dirty")))
            Dim syncStatus As String = If(isDirty = 1, "⏳", If(Not String.IsNullOrEmpty(row("id_cloud").ToString()), "✓", "−"))
            Dim rowIdx As Integer = DgvCabang.Rows.Add(
                row("kode_cabang"), row("nama_cabang"), row("alamat"), row("kota"),
                row("hp"), row("pemilik"), sumber, syncStatus)

            ' Warna tombol Edit (gunakan tema)
            ModuleTheme.SetWarnaDgvBtnEdit(DgvCabang.Rows(rowIdx).Cells("ColEdit"), True)

            ' Warna tombol Hapus (gunakan tema)
            ModuleTheme.SetWarnaDgvBtnHapus(DgvCabang.Rows(rowIdx).Cells("ColHapus"), True)
        Next

        ' Pengaturan standar dan tema DGV
        ModuleTheme.ApplyStandardDataGridViewSettings(DgvCabang)
        ModuleTheme.ApplyThemeDataGridView(DgvCabang)

        DgvCabang.ClearSelection()
        _isLoading = False
    End Sub

    Private Sub DgvCabang_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvCabang.CellContentClick
        If _isLoading OrElse e.RowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = DgvCabang.Rows(e.RowIndex)
        Dim kode As String = If(row.Cells("ColKode").Value IsNot Nothing, row.Cells("ColKode").Value.ToString(), "")
        Dim nama As String = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")

        Select Case DgvCabang.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                PanelHeader.Text = "E D I T   C A B A N G"
                BtnSimpanManual.Text = "UPDATE (F2)"
                TxtKodeCabang.ReadOnly = True
                TxtKodeCabang.Text = kode
                TxtNamaCabang.Text = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")
                TxtAlamat.Text = If(row.Cells("ColAlamat").Value IsNot Nothing, row.Cells("ColAlamat").Value.ToString(), "")
                TxtKota.Text = If(row.Cells("ColKota").Value IsNot Nothing, row.Cells("ColKota").Value.ToString(), "")
                TxtHp.Text = If(row.Cells("ColHp").Value IsNot Nothing, row.Cells("ColHp").Value.ToString(), "")
                TxtPemilik.Text = If(row.Cells("ColPemilik").Value IsNot Nothing, row.Cells("ColPemilik").Value.ToString(), "")
                LblStatus.Text = "Status: mode edit aktif."
                TxtNamaCabang.Focus()

            Case "ColHapus"
                HapusCabang(kode, nama)
        End Select
    End Sub

#End Region

    ''' <summary>
    ''' Generate kode cabang otomatis: CB-[4 char DeviceId]-[urutan 4 digit].
    ''' Contoh: CB-A3F2-0001. Urutan berdasarkan jumlah cabang lokal + 1.
    ''' </summary>
    Private Function GenerateKodeCabang() As String
        Dim suffix As String = SyncConfig.DeviceId.Replace("-", "").Substring(0, 4).ToUpper()
        Dim prefix As String = $"CB-{suffix}-"

        ' Cari urutan tertinggi yang sudah ada lokal dengan prefix ini
        Dim maxUrutan As Integer = 0
        Using cmd As New MySqlCommand(
            "SELECT kode_cabang FROM tbl_cabang WHERE kode_cabang LIKE @prefix", conn)
            cmd.Parameters.AddWithValue("@prefix", prefix & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd(0).ToString()
                    Dim bagian As String = kode.Replace(prefix, "")
                    Dim urutan As Integer
                    If Integer.TryParse(bagian, urutan) Then
                        If urutan > maxUrutan Then maxUrutan = urutan
                    End If
                End While
            End Using
        End Using

        Return $"{prefix}{(maxUrutan + 1).ToString("D4")}"
    End Function

    Private Sub KondisiAwal()
        _isEditMode = False
        PanelHeader.Text = "M A S T E R   C A B A N G"
        BtnSimpanManual.Text = "SIMPAN (F2)"
        ' Kode otomatis — readonly, di-generate saat tambah baru
        TxtKodeCabang.ReadOnly = True
        TxtKodeCabang.Text = GenerateKodeCabang()
        TxtNamaCabang.Clear()
        TxtAlamat.Clear()
        TxtKota.Clear()
        TxtHp.Clear()
        TxtPemilik.Clear()
        TampilCabang()
        LblStatus.Text = "Status: siap tambah cabang baru."
        TxtNamaCabang.Focus()
    End Sub

    Private Function ValidasiInput() As Boolean
        If String.IsNullOrWhiteSpace(TxtNamaCabang.Text) Then
            MessageBox.Show("Nama cabang wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Sub BtnSimpanManual_Click(sender As Object, e As EventArgs) Handles BtnSimpanManual.Click
        If Not ValidasiInput() Then Return

        Dim kode As String = TxtKodeCabang.Text.Trim().ToUpper()
        Dim aksi As String = If(_isEditMode, "UPDATE", "INSERT")

        SimpanCabangLokal(kode, TxtNamaCabang.Text.Trim(), TxtAlamat.Text.Trim(),
                          TxtKota.Text.Trim(), TxtHp.Text.Trim(), TxtPemilik.Text.Trim(), "lokal")
        SyncTriggerCabang(kode, aksi)

        Dim modeTxt = If(_isEditMode, "diupdate", "tersimpan")
        KondisiAwal()
        LblStatus.Text = $"Status: cabang {modeTxt}. Sync saat online untuk konfirmasi kode."
    End Sub

    Private Sub SimpanCabangLokal(kode As String, nama As String, alamat As String,
                                  kota As String, hp As String, pemilik As String, sumber As String)
        Using cmd As New MySqlCommand(
            "INSERT INTO tbl_cabang
             (kode_cabang, nama_cabang, alamat, kota, hp, pemilik, sumber, updated_at, updated_by, sync_id)
             VALUES (@kode, @nama, @alamat, @kota, @hp, @pemilik, @sumber, NOW(), @user, UUID())
             ON DUPLICATE KEY UPDATE
               nama_cabang = VALUES(nama_cabang),
               alamat = VALUES(alamat),
               kota = VALUES(kota),
               hp = VALUES(hp),
               pemilik = VALUES(pemilik),
               sumber = VALUES(sumber),
               updated_at = NOW(),
               updated_by = @user", conn)
            cmd.Parameters.AddWithValue("@kode", kode)
            cmd.Parameters.AddWithValue("@nama", nama)
            cmd.Parameters.AddWithValue("@alamat", alamat)
            cmd.Parameters.AddWithValue("@kota", kota)
            cmd.Parameters.AddWithValue("@hp", hp)
            cmd.Parameters.AddWithValue("@pemilik", pemilik)
            cmd.Parameters.AddWithValue("@sumber", sumber)
            cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub SyncTriggerCabang(kode As String, aksi As String)
        Try
            ' Tandai is_dirty = 1 dan naikkan version
            Using cmd As New MySqlCommand(
                "UPDATE tbl_cabang
                 SET is_dirty = 1,
                     version = COALESCE(version, 0) + 1,
                     updated_by = @user
                 WHERE kode_cabang = @kode", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                cmd.Parameters.AddWithValue("@user", ModuleVariabel.NamaUser)
                cmd.ExecuteNonQuery()
            End Using

            ' Ambil data untuk payload
            Dim payload = AmbilPayloadCabang(kode)
            If payload Is Nothing Then Return

            Dim idCloud As String = ""
            If payload("id_cloud") IsNot Nothing AndAlso payload("id_cloud").Type <> Newtonsoft.Json.Linq.JTokenType.Null Then
                idCloud = payload("id_cloud").ToString()
            End If

            ' Masukkan ke queue
            SyncQueue.Enqueue(aksi, "tbl_cabang", kode, idCloud, payload)

        Catch ex As Exception
            SyncLog.Tulis("ERROR", "tbl_cabang", kode, "", "SyncTriggerCabang error: " & ex.Message)
        End Try
    End Sub

    Private Function AmbilPayloadCabang(kode As String) As Newtonsoft.Json.Linq.JObject
        Using cmd As New MySqlCommand(
            "SELECT kode_cabang, nama_cabang, alamat, kota, hp, pemilik, sumber, id_cloud, version, sync_id FROM tbl_cabang WHERE kode_cabang = @kode LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@kode", kode)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If Not rd.Read() Then Return Nothing
                Dim obj As New Newtonsoft.Json.Linq.JObject()
                For i As Integer = 0 To rd.FieldCount - 1
                    Dim key As String = rd.GetName(i).ToLower()
                    If rd.IsDBNull(i) Then
                        obj(key) = Nothing
                    Else
                        obj(key) = Newtonsoft.Json.Linq.JToken.FromObject(rd.GetValue(i))
                    End If
                Next
                Return obj
            End Using
        End Using
    End Function

    Private Sub BtnSyncCloud_Click(sender As Object, e As EventArgs) Handles BtnSyncCloud.Click
        If Not SupabaseHelper.IsInitialized() OrElse Not SupabaseHelper.CekKoneksi() Then
            MessageBox.Show("Tidak ada koneksi cloud. Silakan coba lagi saat online.", "Offline", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LblStatus.Text = "Status: sync cloud gagal (offline)."
            Return
        End If

        ' Auto-rename kode yang konflik sebelum download
        Dim renamed As Integer = AutoRenameKonflikCabang()
        If renamed > 0 Then
            LblStatus.Text = $"Status: {renamed} kode cabang direname otomatis karena konflik."
        End If

        Try
            Dim lastSync As String = SyncConfig.LastSyncCabang
            Dim query As String = "select=kode_cabang,nama_cabang,alamat,kota,hp,pemilik,id,version&order=kode_cabang.asc"
            If Not String.IsNullOrEmpty(lastSync) Then
                query &= $"&updated_at=gt.{Uri.EscapeDataString(lastSync)}"
            End If

            Dim rows = SupabaseHelper.Get("cabang_master", query)
            Dim count As Integer = 0
            For Each row In rows
                Dim kode As String = row("kode_cabang").ToString()
                Dim versionCloud As Integer = If(row("version") IsNot Nothing, Convert.ToInt32(row("version")), 1)
                Dim idCloud As String = If(row("id") IsNot Nothing, row("id").ToString(), "")

                Dim versionLokal As Integer = 0
                Using cmd As New MySqlCommand(
                    "SELECT COALESCE(version, 0) FROM tbl_cabang WHERE kode_cabang = @kode LIMIT 1", conn)
                    cmd.Parameters.AddWithValue("@kode", kode)
                    Dim val = cmd.ExecuteScalar()
                    If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                        versionLokal = Convert.ToInt32(val)
                    End If
                End Using

                If versionLokal = 0 Then
                    InsertCabangDariCloud(row, idCloud, versionCloud)
                    SyncLog.Tulis("DOWNLOAD", "tbl_cabang", kode, idCloud, "INSERT dari cloud")
                ElseIf versionCloud > versionLokal Then
                    UpdateCabangDariCloud(row, kode, idCloud, versionCloud)
                    SyncLog.Tulis("DOWNLOAD", "tbl_cabang", kode, idCloud, "UPDATE dari cloud")
                End If
                count += 1
            Next

            SyncConfig.UpdateLastSyncCabang()
            TampilCabang()
            LblStatus.Text = $"Status: sync cloud sukses ({count} cabang)."
        Catch ex As Exception
            MessageBox.Show("Gagal sync cabang cloud: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            LblStatus.Text = "Status: sync cloud error."
        End Try
    End Sub

    ''' <summary>
    ''' Cek cabang lokal pending (is_dirty=1, belum punya id_cloud) yang kodenya
    ''' sudah diklaim device lain di cloud. Jika konflik, rename otomatis ke urutan
    ''' berikutnya yang tersedia di cloud, lalu update lokal + queue.
    ''' Return: jumlah kode yang direname.
    ''' </summary>
    Private Function AutoRenameKonflikCabang() As Integer
        Dim renamed As Integer = 0
        Try
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(
                "SELECT kode_cabang FROM tbl_cabang WHERE is_dirty = 1 AND (id_cloud IS NULL OR id_cloud = '')", conn),
                  da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

            For Each row As DataRow In dt.Rows
                Dim kodeAsli As String = row("kode_cabang").ToString()
                Try
                    Dim existing = SupabaseHelper.Get("cabang_master",
                        $"kode_cabang=eq.{Uri.EscapeDataString(kodeAsli)}&select=kode_cabang,device_id")
                    If existing.Count = 0 Then Continue For ' Tidak konflik

                    Dim deviceCloud As String = If(existing(0)("device_id") IsNot Nothing,
                                                   existing(0)("device_id").ToString(), "")
                    If deviceCloud = SyncConfig.DeviceId OrElse String.IsNullOrEmpty(deviceCloud) Then
                        Continue For ' Milik device ini sendiri
                    End If

                    ' Konflik — cari kode baru yang belum dipakai di cloud maupun lokal
                    Dim kodeBaru As String = CariKodeBerikutnya(kodeAsli)
                    If String.IsNullOrEmpty(kodeBaru) Then Continue For

                    ' Rename di lokal: update PK
                    RenameKodeCabangLokal(kodeAsli, kodeBaru)
                    SyncLog.Tulis("UPLOAD", "tbl_cabang", kodeAsli, "",
                                  $"Auto-rename konflik: {kodeAsli} → {kodeBaru}")
                    renamed += 1
                Catch
                    ' Skip jika gagal cek satu baris
                End Try
            Next
        Catch
        End Try
        Return renamed
    End Function

    ''' <summary>
    ''' Cari kode berikutnya yang belum dipakai di cloud maupun lokal.
    ''' Pola: CB-[suffix]-NNNN, increment NNNN sampai bebas.
    ''' </summary>
    Private Function CariKodeBerikutnya(kodeAsli As String) As String
        ' Ambil prefix CB-XXXX- dari kode asli
        Dim parts() As String = kodeAsli.Split("-"c)
        If parts.Length < 3 Then Return ""
        Dim prefix As String = $"{parts(0)}-{parts(1)}-"

        ' Cari semua kode dengan prefix ini di cloud
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
                    If Integer.TryParse(bagian, urutan) AndAlso urutan > maxUrutan Then
                        maxUrutan = urutan
                    End If
                End While
            End Using
        End Using

        ' Cari urutan yang bebas di cloud dan lokal
        Dim kandidat As Integer = maxUrutan + 1
        Dim batas As Integer = kandidat + 100 ' Batas pencarian
        While kandidat <= batas
            Dim kodeBaru As String = $"{prefix}{kandidat.ToString("D4")}"
            If Not kodeCloud.Contains(kodeBaru.ToUpper()) Then
                ' Cek lokal juga
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

    ''' <summary>
    ''' Rename kode_cabang di lokal: update PK dan queue yang terkait.
    ''' </summary>
    Private Sub RenameKodeCabangLokal(kodeAsli As String, kodeBaru As String)
        ' Update tbl_cabang
        Using cmd As New MySqlCommand(
            "UPDATE tbl_cabang SET kode_cabang = @baru, is_dirty = 1, version = COALESCE(version,0)+1
             WHERE kode_cabang = @asli", conn)
            cmd.Parameters.AddWithValue("@baru", kodeBaru)
            cmd.Parameters.AddWithValue("@asli", kodeAsli)
            cmd.ExecuteNonQuery()
        End Using

        ' Update sync_queue yang masih pending untuk kode lama
        Using cmd As New MySqlCommand(
            "UPDATE sync_queue SET id_lokal = @baru
             WHERE tabel = 'tbl_cabang' AND id_lokal = @asli AND status = 'pending'", conn)
            cmd.Parameters.AddWithValue("@baru", kodeBaru)
            cmd.Parameters.AddWithValue("@asli", kodeAsli)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertCabangDariCloud(row As Newtonsoft.Json.Linq.JObject, idCloud As String, version As Integer)
        Using cmd As New MySqlCommand(
            "INSERT IGNORE INTO tbl_cabang
             (kode_cabang, nama_cabang, alamat, kota, hp, pemilik, sumber, id_cloud, version, is_dirty, updated_at)
             VALUES (@kode, @nama, @alamat, @kota, @hp, @pemilik, 'cloud', @idCloud, @version, 0, NOW())", conn)
            MapParamCabang(cmd, row, idCloud, version)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub UpdateCabangDariCloud(row As Newtonsoft.Json.Linq.JObject, kode As String, idCloud As String, version As Integer)
        Using cmd As New MySqlCommand(
            "UPDATE tbl_cabang SET
             nama_cabang = @nama, alamat = @alamat, kota = @kota, hp = @hp, pemilik = @pemilik,
             sumber = 'cloud', id_cloud = @idCloud, version = @version, is_dirty = 0, updated_at = NOW()
             WHERE kode_cabang = @kode", conn)
            MapParamCabang(cmd, row, idCloud, version)
            cmd.Parameters.AddWithValue("@kode", kode)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub MapParamCabang(cmd As MySqlCommand, row As Newtonsoft.Json.Linq.JObject, idCloud As String, version As Integer)
        Dim g = Function(key As String) As String
                    Return If(row(key) IsNot Nothing AndAlso row(key).Type <> Newtonsoft.Json.Linq.JTokenType.Null,
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

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles BtnRefresh.Click
        TampilCabang()
        LblStatus.Text = "Status: data cabang direfresh."
    End Sub

    Private Sub HapusCabang(kode As String, nama As String)
        If MessageBox.Show($"Hapus cabang '{kode}' dari referensi lokal?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_cabang WHERE kode_cabang = @kode", conn)
                    cmd.Parameters.AddWithValue("@kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                KondisiAwal()
                LblStatus.Text = "Status: cabang lokal dihapus."
            Catch ex As Exception
                MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub BtnTambah_Click(sender As Object, e As EventArgs) Handles BtnTambah.Click
        KondisiAwal()
        LblStatus.Text = "Status: mode tambah baru aktif."
    End Sub

    Private Sub FormCabang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpanManual_Click(Nothing, EventArgs.Empty)
                e.SuppressKeyPress = True
            Case Keys.F4
                BtnTambah_Click(Nothing, EventArgs.Empty)
                e.SuppressKeyPress = True
            Case Keys.F5
                BtnRefresh_Click(Nothing, EventArgs.Empty)
                e.SuppressKeyPress = True
            Case Keys.Escape
                Me.Close()
        End Select
    End Sub

End Class
