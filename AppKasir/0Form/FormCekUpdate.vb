Imports AutoUpdaterDotNET
Imports System.Reflection

Public Class FormCekUpdate

    Private Const urlUpdateXML As String = "https://raw.githubusercontent.com/adysuryadi64/AppKasir/master/update.xml"

    ' ── Load ────────────────────────────────────────────────────────
    Private Sub FormCekUpdate_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        TerapkanThemeForm()

        Dim versiInstalled As String = Assembly.GetExecutingAssembly().GetName().Version.ToString()
        lblVersiInstalled.Text = versiInstalled
        lblVersiTerbaru.Text = "-"
        lblStatus.Text = "Klik 'Cek Update' untuk memeriksa versi terbaru."
    End Sub

    ''' <summary>
    ''' Terapkan warna dari token ModuleTheme ke kontrol yang butuh warna spesifik.
    ''' Dipanggil setelah TerapkanTheme agar override warna default tema bisa dilakukan.
    ''' </summary>
    Private Sub TerapkanThemeForm()
        ' Header — pakai warna kategori "Sync" (abu-abu/slate) karena ini form utilitas
        Dim warnaHeader = ModuleTheme.GetWarnaHeader("Sync")
        PanelHeader.BackColor = warnaHeader.Back
        LblNama.ForeColor = warnaHeader.Fore

        ' Body & Footer — pakai surface/bg token
        PanelBody.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
        PanelFooter.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)

        ' Kartu versi — pakai subtle sebagai background kartu
        PanelVersi.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Surface)

        ' Label judul versi — muted/secondary
        lblTitleInstalled.ForeColor = ModuleTheme.C(ModuleTheme.L_Secondary, ModuleTheme.D_Secondary)
        lblTitleTerbaru.ForeColor = ModuleTheme.C(ModuleTheme.L_Secondary, ModuleTheme.D_Secondary)

        ' Label nilai versi — teks utama
        lblVersiInstalled.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
        lblVersiTerbaru.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

        ' Status text — secondary
        lblStatus.ForeColor = ModuleTheme.C(ModuleTheme.L_Secondary, ModuleTheme.D_Secondary)
        lblStatus.BackColor = Color.Transparent

        ' RichTextBox Changelog
        rtbChangelog.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
        rtbChangelog.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

        ' Tombol — pakai primary (biru)
        SetWarnaBtn(False)
    End Sub

    ''' <summary>Set warna tombol: biru (normal) atau hijau (ada update).</summary>
    Private Sub SetWarnaBtn(adaUpdate As Boolean)
        If adaUpdate Then
            btnCekUpdate.BackColor = ModuleTheme.C(ModuleTheme.L_Success, ModuleTheme.D_Success)
            btnCekUpdate.FlatAppearance.MouseOverBackColor = ModuleTheme.C(ModuleTheme.L_SuccessHover, ModuleTheme.D_SuccessHover)
            btnCekUpdate.FlatAppearance.MouseDownBackColor = ModuleTheme.C(ModuleTheme.L_SuccessDown, ModuleTheme.D_SuccessDown)
        Else
            btnCekUpdate.BackColor = ModuleTheme.C(ModuleTheme.L_Primary, ModuleTheme.D_Primary)
            btnCekUpdate.FlatAppearance.MouseOverBackColor = ModuleTheme.C(ModuleTheme.L_PrimaryHover, ModuleTheme.D_PrimaryHover)
            btnCekUpdate.FlatAppearance.MouseDownBackColor = ModuleTheme.C(ModuleTheme.L_PrimaryHover, ModuleTheme.D_PrimaryHover)
        End If
        btnCekUpdate.ForeColor = ModuleTheme.White
    End Sub

    Private updateArgs As UpdateInfoEventArgs = Nothing

    ' ── Cek Update ──────────────────────────────────────────────────
    Private Sub btnCekUpdate_Click(sender As Object, e As EventArgs) Handles btnCekUpdate.Click
        If btnCekUpdate.Text = "Unduh Update" AndAlso updateArgs IsNot Nothing Then
            ModuleVariabel.AplikasiSedangUpdate = True
            Try
                AutoUpdater.DownloadUpdate(updateArgs)
            Catch ex As Exception
                lblStatus.Text = "Gagal mengunduh: " & ex.Message
                SetWarnaBtn(False)
                btnCekUpdate.Text = "Coba Lagi"
            End Try
            Return
        End If

        btnCekUpdate.Enabled = False
        lblVersiTerbaru.Text = "..."
        lblVersiTerbaru.ForeColor = ModuleTheme.C(ModuleTheme.L_Muted, ModuleTheme.D_Muted)
        lblStatus.Text = "Menghubungi server..."
        ProgressBar.Visible = True
        ProgressBar.Style = ProgressBarStyle.Marquee

        ' Pastikan AutoUpdater tidak tampil dialog sendiri — kita handle via event
        AutoUpdater.ShowSkipButton = False
        AutoUpdater.ShowRemindLaterButton = False

        AddHandler AutoUpdater.CheckForUpdateEvent, AddressOf AutoUpdaterOnCheckForUpdateEvent
        AddHandler AutoUpdater.ApplicationExitEvent, AddressOf AutoUpdaterOnApplicationExitEvent
        AutoUpdater.Start(urlUpdateXML)
    End Sub

    Private Sub AutoUpdaterOnApplicationExitEvent()
        ' Karena ModuleVariabel.AplikasiSedangUpdate sudah True, FormUtama akan tertutup otomatis tanpa peringatan
        Application.Exit()
    End Sub

    Private Sub AutoUpdaterOnCheckForUpdateEvent(args As UpdateInfoEventArgs)
        RemoveHandler AutoUpdater.CheckForUpdateEvent, AddressOf AutoUpdaterOnCheckForUpdateEvent

        If Me.IsDisposed Then Return

        If Me.InvokeRequired Then
            Me.Invoke(Sub() AutoUpdaterOnCheckForUpdateEvent(args))
            Return
        End If

        btnCekUpdate.Enabled = True
        ProgressBar.Visible = False

        If args.Error Is Nothing Then
            lblVersiTerbaru.Text = args.CurrentVersion.ToString()

            If args.IsUpdateAvailable Then
                updateArgs = args
                ' Ada update — versi terbaru warna hijau (sukses/positif)
                lblVersiTerbaru.ForeColor = ModuleTheme.C(ModuleTheme.L_Success, ModuleTheme.D_Success)
                lblStatus.Text = "Update tersedia! Versi " & args.CurrentVersion & " siap diunduh."
                btnCekUpdate.Text = "Unduh Update"
                SetWarnaBtn(True)

                ' Ambil changelog
                Try
                    Using client As New System.Net.WebClient()
                        client.Encoding = System.Text.Encoding.UTF8
                        Dim changelogText As String = client.DownloadString("https://raw.githubusercontent.com/adysuryadi64/AppKasir/master/changelog.md")
                        rtbChangelog.Text = "=== CATATAN RILIS === " & vbCrLf & vbCrLf & changelogText
                    End Using
                Catch ex As Exception
                    rtbChangelog.Text = "Gagal memuat catatan rilis."
                End Try
            Else
                ' Sudah terbaru — versi terbaru warna teks normal
                lblVersiTerbaru.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                lblStatus.Text = "Aplikasi sudah versi terbaru."
                btnCekUpdate.Text = "Cek Update"
                SetWarnaBtn(False)
            End If
        Else
            ' Error — versi terbaru warna merah (bahaya)
            lblVersiTerbaru.Text = "Error"
            lblVersiTerbaru.ForeColor = ModuleTheme.C(ModuleTheme.L_Danger, ModuleTheme.D_Danger)
            lblStatus.Text = "Gagal: " & args.Error.Message
            btnCekUpdate.Text = "Coba Lagi"
            SetWarnaBtn(False)
        End If
    End Sub

    ' ── Close ───────────────────────────────────────────────────────
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    ' ── Drag form (FormBorderStyle = None) ──────────────────────────
    Private _dragging As Boolean = False
    Private _dragStart As System.Drawing.Point

    Private Sub PanelHeader_MouseDown(sender As Object, e As MouseEventArgs) Handles PanelHeader.MouseDown, LblNama.MouseDown
        _dragging = True
        _dragStart = e.Location
    End Sub

    Private Sub PanelHeader_MouseMove(sender As Object, e As MouseEventArgs) Handles PanelHeader.MouseMove, LblNama.MouseMove
        If _dragging Then
            Dim p As System.Drawing.Point = Me.PointToScreen(e.Location)
            Me.Location = New System.Drawing.Point(p.X - _dragStart.X, p.Y - _dragStart.Y)
        End If
    End Sub

    Private Sub PanelHeader_MouseUp(sender As Object, e As MouseEventArgs) Handles PanelHeader.MouseUp, LblNama.MouseUp
        _dragging = False
    End Sub

End Class
