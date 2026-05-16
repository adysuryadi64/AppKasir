Public Class FormSync

    Private _sedangSync As Boolean = False

#Region "Form Load"
    Private Sub FormSync_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Dim url As String = AppConfig.Instance.GetValue(Of String)("SupabaseUrl", "")
        Dim key As String = AppConfig.Instance.GetValue(Of String)("SupabaseKey", "")

        If Not String.IsNullOrEmpty(url) AndAlso Not String.IsNullOrEmpty(key) Then
            SupabaseHelper.Init(url, key)
        End If

        LblKodeToko.Text = "Cabang: " & SyncConfig.KodeCabang
        LblLastSync.Text = "Last sync: " & SyncConfig.LastSyncBarang

        AddHandler SyncManager.OnProgress, AddressOf OnSyncProgress
        AddHandler SyncManager.OnSelesai, AddressOf OnSyncSelesai

        RefreshStatusQueue()
        TampilkanModeKoneksi()
    End Sub
#End Region

#Region "Tombol Sync"
    Private Sub BtnSync_Click(sender As Object, e As EventArgs) Handles BtnSync.Click
        JalankanSync(Sub() SyncManager.SyncSemua(), "Upload + Download")
    End Sub

    Private Sub BtnUpload_Click(sender As Object, e As EventArgs) Handles BtnUpload.Click
        If SyncQueue.CountPending() = 0 Then
            AppendLog("Tidak ada data yang perlu diupload.")
            Return
        End If
        JalankanSync(Sub() SyncManager.SyncUploadSemua(), "Upload")
    End Sub

    Private Sub BtnDownload_Click(sender As Object, e As EventArgs) Handles BtnDownload.Click
        JalankanSync(Sub() SyncManager.SyncDownloadSemua(), "Download")
    End Sub

    Private Sub JalankanSync(aksi As Action, label As String)
        If _sedangSync Then Return

        If Not SupabaseHelper.IsInitialized() Then
            MessageBox.Show("Supabase belum dikonfigurasi.", "Konfigurasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not SupabaseHelper.CekKoneksi() Then
            MessageBox.Show("Tidak ada koneksi ke Supabase." & Environment.NewLine &
                            "Data tetap tersimpan lokal dan akan disync saat online.",
                            "Offline", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Validasi kode cabang sebelum upload — cegah konflik dengan cabang lain
        If label <> "Download" Then
            Dim pesanError As String = ""
            If Not SyncManager.ValidasiKodeCabang(pesanError) Then
                MessageBox.Show(pesanError, "Kode Cabang Konflik",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                AppendLog("UPLOAD DIBATALKAN: " & pesanError)
                Return
            End If
        End If

        _sedangSync = True
        SetTombolSync(False)
        TxtLog.Clear()
        LblStatus.Text = $"{label} sedang berjalan..."
        LblStatus.ForeColor = Color.Orange

        Task.Run(Sub()
                     Try
                         aksi()
                     Catch ex As Exception
                         Me.Invoke(Sub() AppendLog("ERROR: " & ex.Message))
                     Finally
                         Me.Invoke(Sub()
                                       _sedangSync = False
                                       SetTombolSync(True)
                                       LblLastSync.Text = "Last sync: " & SyncConfig.LastSyncBarang
                                       LblStatus.Text = $"{label} selesai"
                                       LblStatus.ForeColor = Color.Green
                                       RefreshStatusQueue()
                                   End Sub)
                     End Try
                 End Sub)
    End Sub

    Private Sub SetTombolSync(enabled As Boolean)
        BtnSync.Enabled = enabled
        BtnUpload.Enabled = enabled
        BtnDownload.Enabled = enabled
    End Sub
#End Region

#Region "Mode Koneksi"
    Private Sub TampilkanModeKoneksi()
        If SupabaseHelper.IsInitialized() AndAlso SupabaseHelper.CekKoneksi() Then
            LblStatus.Text = "Online — siap sync"
            LblStatus.ForeColor = Color.Green
        Else
            LblStatus.Text = "Offline — data tersimpan lokal"
            LblStatus.ForeColor = Color.Gray
        End If
    End Sub
#End Region

#Region "Log & Status"
    Private Sub OnSyncProgress(pesan As String)
        If Me.IsHandleCreated Then
            Me.Invoke(Sub() AppendLog(pesan))
        End If
    End Sub

    Private Sub OnSyncSelesai(sukses As Integer, gagal As Integer)
        If Me.IsHandleCreated Then
            Me.Invoke(Sub()
                          AppendLog($"--- Selesai: {sukses} sukses, {gagal} gagal ---")
                          RefreshStatusQueue()
                      End Sub)
        End If
    End Sub

    Private Sub AppendLog(pesan As String)
        TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {pesan}{Environment.NewLine}")
        TxtLog.ScrollToCaret()
    End Sub

    Private Sub RefreshStatusQueue()
        Dim pending As Integer = SyncQueue.CountPending()
        LblQueue.Text = $"Queue pending: {pending}"
        LblQueue.ForeColor = If(pending > 0, Color.OrangeRed, Color.Green)
        BtnUpload.Enabled = Not _sedangSync AndAlso pending > 0
        BtnSync.Enabled = Not _sedangSync AndAlso pending > 0
        BtnDownload.Enabled = Not _sedangSync
    End Sub

    Private Sub BtnLihatLog_Click(sender As Object, e As EventArgs) Handles BtnLihatLog.Click
        DgvLog.DataSource = SyncLog.GetLog(100)
    End Sub

    Private Sub BtnRefreshQueue_Click(sender As Object, e As EventArgs) Handles BtnRefreshQueue.Click
        RefreshStatusQueue()
        TampilkanModeKoneksi()
    End Sub
#End Region

#Region "Cek Koneksi"
    Private Sub BtnCekKoneksi_Click(sender As Object, e As EventArgs) Handles BtnCekKoneksi.Click
        Dim online As Boolean = SupabaseHelper.IsInitialized() AndAlso SupabaseHelper.CekKoneksi()
        If online Then
            LblStatus.Text = "Online — siap sync"
            LblStatus.ForeColor = Color.Green
            MessageBox.Show("Koneksi ke Supabase OK", "Online", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            LblStatus.Text = "Offline — data tersimpan lokal"
            LblStatus.ForeColor = Color.Gray
            MessageBox.Show("Tidak dapat terhubung ke Supabase." & Environment.NewLine &
                            "Toko tetap berjalan normal dalam mode offline.",
                            "Offline", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        RefreshStatusQueue()
    End Sub
#End Region

    Private Sub FormSync_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        RemoveHandler SyncManager.OnProgress, AddressOf OnSyncProgress
        RemoveHandler SyncManager.OnSelesai, AddressOf OnSyncSelesai
    End Sub

End Class
