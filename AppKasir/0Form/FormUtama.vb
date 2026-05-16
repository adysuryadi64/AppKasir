Imports System.IO
Imports System.Text.Json


Public Class FormUtama

    ' ── WebBrowser Dashboard ─────────────────────────────────────────────────
    ' WbDashboard sudah didefinisikan di designer — posisi dan anchor sama dengan GBTransaksi
    ' Toggle visibility keduanya untuk berganti tampilan

    ''' <summary>Inisialisasi dashboard — dipanggil sekali saat form load</summary>
    Private Sub InisDashboard()
        ' Tidak perlu setup dinamis — WbDashboard sudah ada di designer
        ' Langsung tampilkan jika GBTransaksi sedang hidden
        If Not GBTransaksi.Visible Then TampilDashboard()
    End Sub

    ''' <summary>Tampilkan dashboard HTML di WebBrowser</summary>
    Public Sub TampilDashboard()
        If WbDashboard Is Nothing OrElse WbDashboard.IsDisposed Then Return
        Try
            Dim lokasi As String = If(StatusLokasi IsNot Nothing, StatusLokasi.Text, "TOKO")
            Dim html As String = ModuleDashboard.BangunHTML(lokasi)
            Dim tmpPath As String = IO.Path.Combine(Application.StartupPath, "_dashboard_tmp.html")
            IO.File.WriteAllText(tmpPath, html, System.Text.Encoding.UTF8)
            WbDashboard.Visible = True
            WbDashboard.Navigate(New Uri(tmpPath))
        Catch
        End Try
    End Sub

    ''' <summary>Toggle dashboard/GBTransaksi saat visibility berubah</summary>
    Private Sub GBTransaksi_VisibleChanged(sender As Object, e As EventArgs) Handles GBTransaksi.VisibleChanged
        ' Saat GBTransaksi tampil, sembunyikan WbDashboard
        If GBTransaksi.Visible Then WbDashboard.Visible = False
    End Sub



    ' Timer untuk cek perubahan general setting dari client lain (setiap 60 detik)
    Private WithEvents TimerCekSetting As New System.Windows.Forms.Timer()

    ' Tracking button nav yang sedang aktif — untuk restore setelah TerapkanModeSemua
    Private _activeNavButton As Button = Nothing

    ''' <summary>
    ''' Dipanggil dari FormCompany saat logo/background perusahaan diubah.
    ''' Dashboard WebView2 di-refresh agar perubahan langsung terlihat.
    ''' BackgroundImage tidak dipakai lagi sejak dashboard beralih ke WebView2.
    ''' </summary>
    Public Sub ChangeBackgroundImage(ByVal imageFileName As String)
        ' Refresh dashboard — gambar background tidak dipakai lagi
        If Not GBTransaksi.Visible Then TampilDashboard()
    End Sub




    ' ==================== HELPER METHODS ====================
    ''' <summary>Buka form sebagai MDI child dengan Dock Fill</summary>
    Private Sub BukaFormMdi(frm As Form)
        ' Sembunyikan dashboard sebelum form MDI tampil
        WbDashboard.Visible = False
        frm.MdiParent = Me
        frm.BringToFront()
        frm.Dock = DockStyle.Fill
        frm.Show()
    End Sub

    ''' <summary>Tutup semua MDI children</summary>
    Private Sub TutupSemuaForm()
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        ' Tampilkan dashboard jika tidak ada GBTransaksi dan tidak ada MDI child tersisa
        If Not GBTransaksi.Visible AndAlso MdiChildren.Length = 0 Then
            TampilDashboard()
        End If
    End Sub

    ''' <summary>Reset panel dan sembunyikan GBTransaksi</summary>
    Private Sub ResetPanelMenu()
        GBTransaksi.Visible = False
        WbDashboard.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False
    End Sub

    ''' <summary>Inisialisasi panel transaksi saat tombol transaksi diklik</summary>
    Private Sub IniTransaksiPanel(btn As Button, namaTransaksi As String, Optional showDetail As Boolean = True)
        SetButtonBackgroundColor(btn)
        TutupSemuaForm()
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = namaTransaksi
        SplitTransaksi.Panel2Collapsed = Not showDetail
        LblDetailTransaksi.Visible = showDetail
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()
    End Sub

    ''' <summary>Terapkan hak akses ke tombol CRUD dari cache</summary>
    Private Sub TerapkanHakAkses(namaMenu As String,
                                  Optional showEdit As Boolean = True,
                                  Optional showPrint As Boolean = True)
        Dim ha As Boolean() = ModulHakAkses.BacaHakAksesDariCache(namaMenu)
        BtnTambah.Visible = ha(1)
        BTNEdit.Visible = If(showEdit, ha(2), False)
        BtnHapus.Visible = ha(3)
        BtnPrint.Visible = If(showPrint, ha(1), False)
    End Sub

    ''' <summary>Terapkan hak akses ke context menu strip dari cache</summary>
    Private Sub TerapkanHakAksesContextMenu(namaMenu As String,
                                             Optional showEdit As Boolean = True,
                                             Optional showCetak As Boolean = True,
                                             Optional showEditBayar As Boolean = False)
        Dim ha As Boolean() = ModulHakAkses.BacaHakAksesDariCache(namaMenu)
        TambahToolStripMenuItem.Visible = ha(1)
        EditToolStripMenuItem.Visible = If(showEdit, ha(2), False)
        HapusToolStripMenuItem.Visible = ha(3)
        CetakToolStripMenuItem.Visible = showCetak
        EditPembayaranToolStripMenuItem.Visible = showEditBayar
    End Sub

    ''' <summary>Set teks tombol CRUD sesuai nama transaksi</summary>
    Private Sub AturTombolTransaksi(nama As String)
        BtnTambah.Text = "Tambah " & nama & " (F2)"
        BTNEdit.Text = "Edit " & nama & " (F3)"
        BtnHapus.Text = "Hapus " & nama & " (F4)"
        BtnPrint.Text = "Cetak " & nama & " (F5)"
        LblDetailTransaksi.Text = "Detail " & nama & " : "
        SusunTombolPanel1()
    End Sub

    ''' <summary>Susun posisi button Panel1 secara dinamis sesuai lebar teks masing-masing.</summary>
    Private Sub SusunTombolPanel1()
        Const GAP As Integer = 4
        Const Y As Integer = 1

        ' Reset Size ke Empty dulu agar AutoSize recalculate dari nol
        ' (AutoSize tidak mengecil jika Size pernah di-set eksplisit)
        For Each btn As Button In {BtnTambah, BTNEdit, BtnHapus, BtnPrint}
            btn.AutoSize = False
            btn.Size = btn.GetPreferredSize(Size.Empty)
            btn.AutoSize = True
        Next

        Dim x As Integer = 2
        BtnTambah.Location = New Point(x, Y)
        x += BtnTambah.Width + GAP

        BTNEdit.Location = New Point(x, Y)
        x += BTNEdit.Width + GAP

        BtnHapus.Location = New Point(x, Y)
        x += BtnHapus.Width + GAP

        BtnPrint.Location = New Point(x, Y)
        x += BtnPrint.Width + GAP + 12

        LblRangkuman.Location = New Point(x, 6)
    End Sub

    ''' <summary>Hitung rangkuman record dan total, tampilkan di LblRangkuman</summary>
    Private Sub HitungRangkuman(queryJumlah As String, labelTotal As String,
                                 tanggalAwal As Date, tanggalAkhir As Date,
                                 searchFilter As String)
        Using cmd As New MySqlCommand(queryJumlah, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@SearchText", searchFilter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    LblRangkuman.Text = "Jumlah Record: " & CInt(rd("RECORD")).ToString("N0") &
                                        Environment.NewLine & labelTotal & ": Rp. " & CDec(rd("TOTAL")).ToString("N0")
                Else
                    LblRangkuman.Text = "0"
                End If
            End Using
        End Using
    End Sub

    ''' <summary>Load data ke DGVTransaksi dari query dengan filter tanggal dan search</summary>
    Private Sub LoadDataTransaksi(queryString As String, namaTable As String,
                                   tanggalAwal As Date, tanggalAkhir As Date,
                                   searchFilter As String)
        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@SearchText", searchFilter)
            Using ds As New DataSet
                da.Fill(ds, namaTable)
                DGVTransaksi.DataSource = ds.Tables(namaTable)
            End Using
        End Using
    End Sub

    ''' <summary>Set alignment kanan + format angka pada satu atau lebih kolom DGV</summary>
    Private Sub AturKolomAngka(dgv As DataGridView, ParamArray kolom() As Object)
        Dim cs As New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleRight,
            .Format = "#,0.##"
        }
        For Each k In kolom
            dgv.Columns(k).DefaultCellStyle = cs
        Next
    End Sub

    ''' <summary>Bersihkan kontrol setelah load data transaksi</summary>
    Private Sub BersihkanKontrolTransaksi(Optional labelDetail As String = "")
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        If labelDetail <> "" Then LblDetailTransaksi.Text = labelDetail
    End Sub

    ''' <summary>Jalankan proses posting dengan konfirmasi</summary>
    Private Sub JalankanPosting(jenis As String)
        ResetPanelMenu()
        TutupSemuaForm()
        Dim result As DialogResult = MessageBox.Show(
            "Penting! Jangan lupa untuk sering melakukan posting data agar sinkronisasi data tetap terjaga dan tidak terjadi perbedaan data antara sistem dan realita.",
            "Pesan Penting: Posting Data", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
        If result = DialogResult.No Then Exit Sub
        With FormLoading
            .Label1.Text = "Proses posting! Silahkan menunggu konfigurasi data"
            .BringToFront()
            .Show()
            .MulaiPosting(jenis)
        End With
        PanelTransaksi.Visible = True
    End Sub

    ' ==================== END HELPER METHODS ====================

    Private Sub SetMenuBackgroundColor(ByVal clickedMenu As ToolStripMenuItem)
        For Each menu As ToolStripMenuItem In {FileToolStripMenuItem, MenuMaster, MenuTransaksi, MenuJurnal, MenuKaryawan, MenuLaporan, MenuUtility, MenuPosting, HelpToolStripMenuItem, WindowToolStripMenuItem}
            menu.BackColor = ModuleTheme.C(ModuleTheme.L_Toolbar, ModuleTheme.D_Toolbar)
        Next
        clickedMenu.BackColor = ModuleTheme.C(ModuleTheme.L_MenuActive, ModuleTheme.D_MenuActive)
    End Sub

    Private Sub SetButtonBackgroundColor(ByVal clickedButton As Button)
        Dim buttons As Button() = {
        BtnToko, BtnBarang, BTnPelanggan, BtnSupliyer, BtnUser, BtnTabelRef, BtnKirimCabang, BtnHakAksesUser, BtnGeneralSetting, BtnKaryawan, BtnArmada,
        BtnBelanja, BtnPenjualan, BtnRetuBelanja, BtnReturPenjualan, BtnBayarHutang, BtnBayarPiutang, BtnStokOpname, BtnPindahStok, BtnTransferBarang,
        BtnSuratJalan, BtnMasterCabang
        }
        For Each button As Button In buttons
            ModuleTheme.SetNavButtonIdle(button)
        Next
        ModuleTheme.SetNavButtonActive(clickedButton)
        _activeNavButton = clickedButton
    End Sub

    Private Sub FormUtama_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.LoadFromConfig()
        TerapkanModeSemua()


        AturTooltip()
        Terkunci()

        OpenConnection()

        WbDashboard.Visible = False

        With FormLogin
            .BringToFront()
            .ShowDialog()
        End With
        If String.IsNullOrWhiteSpace(StatusNamaUser.Text) Then
            Close()
            Return
        End If

        InisDashboard()

        Dim pilihanMasuk As String = AppConfig.Instance.GetValue(Of String)("PilihanMasuk", "").ToUpperInvariant()
        If pilihanMasuk = "TOKO" OrElse pilihanMasuk = "GUDANG" Then
            FormMasuk.TerapkanLokasiKeFormUtama(pilihanMasuk)
        Else
            With FormMasuk
                .BringToFront()
                .ShowDialog()
            End With
        End If
        If String.IsNullOrWhiteSpace(StatusLokasi.Text) Then
            Close()
            Return
        End If

        ' === CACHE HAK AKSES USER SETELAH LOGIN BERHASIL ===
        If Not String.IsNullOrEmpty(StatusNamaUser.Text) Then
            ModulHakAkses.CacheHakAksesUser(StatusLevelUser.Text)
        End If

        With FormLoading
            .Label1.Text = "Selamat datang! Aplikasi saat ini dalam proses inisialisasi dan menunggu konfigurasi data"
            .BringToFront()
            .Show()
            .MulaiLoading()
        End With

        ' Update title setelah NAMA_PERUSAHAAN sudah diisi oleh MulaiLoading
        If Not String.IsNullOrEmpty(NAMA_PERUSAHAAN) Then
            Me.Text = "KASIR LANCAR " & StatusLokasi.Text & " " & NAMA_PERUSAHAAN
        End If

        'Rekeningkasbank()
        'AmbilAkunKasBankEkuitas()

        DtpTransaksi.Value = DateTime.Today
        DtpTransaksi.Format = DateTimePickerFormat.Custom
        DtpTransaksi.CustomFormat = "dd/MM/yyyy"
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()

        If Not String.IsNullOrEmpty(StatusNamaUser.Text) Then
            TimerCekSetting.Interval = 60000
            TimerCekSetting.Start()
        End If

        ' Pilih Penjualan sebagai tampilan default setelah loading selesai
        ' Cek hak akses dulu — jika tidak ada akses Penjualan, coba Pembelian
        If BtnPenjualan.Visible Then
            BtnPenjualan.PerformClick()
        ElseIf BtnBelanja.Visible Then
            BtnBelanja.PerformClick()
        End If

        ' Jalankan arsip audit trail otomatis untuk Master/Owner (sekali per hari)
        If StatusLevelUser.Text = "Master" OrElse StatusLevelUser.Text = "Owner" Then
            ModuleAuditTrail.JalankanArsipJikaPerlu()
        End If
    End Sub

    Private Sub FormUtama_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Re-apply theme setelah semua handle selesai dibuat
        TerapkanModeSemua()
        ' Restore warna button nav aktif yang di-reset oleh TerapkanFormUtama
        If _activeNavButton IsNot Nothing Then
            ModuleTheme.SetNavButtonActive(_activeNavButton)
        End If
    End Sub

    Private Sub FormUtama_MdiChildActivate(sender As Object, e As EventArgs) Handles MyBase.MdiChildActivate
        ' Setiap kali MDI child dibuka atau diaktifkan, terapkan theme otomatis
        If ActiveMdiChild IsNot Nothing Then
            ModuleTheme.TerapkanTheme(ActiveMdiChild)
        End If
    End Sub


    Private Sub AturTooltip()
        ModuleTooltip.AturTooltip(Me)
    End Sub

    Public Sub Terkunci()
        BtnNotif.Visible = False
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        LogOutToolStripMenuItem.Enabled = False
        MenuMaster.Visible = False
        MenuTransaksi.Visible = False
        MenuJurnal.Visible = False
        MenuKaryawan.Visible = False
        MenuLaporan.Visible = False
        MenuUtility.Visible = False
        MenuPosting.Visible = False
        'HelpToolStripMenuItem.Visible = False
        WindowToolStripMenuItem.Visible = False
        LblServer.Text = "DB :"
        LblServerDb.Text = ""
        StatusTanggal.Text = ""
        LblJamSekarang.Text = ""
        LblVersiApp.Text = ""
        LblServerDb.Text = ""
        StatusNamaUser.Text = ""
        StatusLevelUser.Text = ""
        StatusLokasi.Text = ""
        StatusLokasi.Image = Nothing
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False
        PanelTransaksi.Location = New Point(0, 31)
        PanelTransaksi.Dock = System.Windows.Forms.DockStyle.Top
    End Sub


    Public Sub CekaktivasiProgram()
        ' Delegasikan sepenuhnya ke ACTIVATION_FORM yang ada di 7Reg
        If Not ACTIVATION_FORM.IsActivated() Then
            RegristerToolStripMenuItem.Enabled = True
            ACTIVATION_FORM.ShowDialog()
        Else
            RegristerToolStripMenuItem.Enabled = False
        End If
    End Sub

    Public Sub CheckLicense()
        ' Delegasikan ke ACTIVATION_FORM
        ACTIVATION_FORM.CheckLicense()
    End Sub

    Private Sub Timer2_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer2.Tick
        LblJamSekarang.Text = TimeOfDay
    End Sub

    Private Sub TimerCekSetting_Tick(sender As Object, e As EventArgs) Handles TimerCekSetting.Tick
        ' Cek apakah ada perubahan general setting dari client lain
        ' Query ringan: hanya MAX(updated_at), bukan load semua data
        If Not String.IsNullOrEmpty(StatusNamaUser.Text) Then
            ModulHakAkses.CekDanRefreshGeneralSetting()
        End If
    End Sub

    '----------------------------------------- MAIN MENU ---------------------------------------------------------------------------

    Private Sub FileToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles FileToolStripMenuItem.Click
        SetMenuBackgroundColor(FileToolStripMenuItem)
    End Sub

    Private Sub MenuMaster_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuMaster.Click
        SetMenuBackgroundColor(MenuMaster)
        ResetPanelMenu()
        TutupSemuaForm()
        PanelMaster.Visible = True
        PanelMaster.Location = New Point(0, 31)
        PanelMaster.Dock = System.Windows.Forms.DockStyle.Top
    End Sub

    Private Sub MenuTransaksi_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuTransaksi.Click
        SetMenuBackgroundColor(MenuTransaksi)
        ResetPanelMenu()
        TutupSemuaForm()
        PanelTransaksi.Visible = True
        PanelTransaksi.Location = New Point(0, 31)
        PanelTransaksi.Dock = System.Windows.Forms.DockStyle.Top
    End Sub

    Private Sub MenuJurnal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuJurnal.Click
        SetMenuBackgroundColor(MenuJurnal)
        ResetPanelMenu()
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormKeuangan)
    End Sub

    Private Sub MenuKaryawan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuKaryawan.Click
        SetMenuBackgroundColor(MenuKaryawan)
        ResetPanelMenu()
        TutupSemuaForm()
    End Sub

    Private Sub MenuLaporan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuLaporan.Click
        SetMenuBackgroundColor(MenuLaporan)
        ResetPanelMenu()
    End Sub

    Private Sub MenuUtility_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuUtility.Click
        SetMenuBackgroundColor(MenuUtility)
        ResetPanelMenu()
        TutupSemuaForm()
    End Sub


    Private Sub PostingTokoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PostingTokoToolStripMenuItem.Click
        JalankanPosting("Toko")
    End Sub

    Private Sub PostingGudangToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PostingGudangToolStripMenuItem.Click
        JalankanPosting("Gudang")
    End Sub

    Private Sub PostingSemuaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PostingSemuaToolStripMenuItem.Click
        JalankanPosting("Semua")
    End Sub

    Private Sub AuditTrailToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles AuditTrailToolStripMenuItem1.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormAuditTrail)
    End Sub

    Private Sub AuditTrailArsipToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AuditTrailArsipToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormAuditTrailArsip)
    End Sub

    Private Sub MenuPosting_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuPosting.Click
        SetMenuBackgroundColor(MenuPosting)
    End Sub

    Private Sub WindowToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles WindowToolStripMenuItem.Click
        ResetPanelMenu()
        TutupSemuaForm()
        SetMenuBackgroundColor(WindowToolStripMenuItem)
    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HelpToolStripMenuItem.Click
        ResetPanelMenu()
        TutupSemuaForm()
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormAbout)
    End Sub

    '----------------------------------------- FILE ---------------------------------------------------------------------------

    Private Sub LoginToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LoginToolStripMenuItem.Click
        HandleLoginClick()
    End Sub

    Public Sub HandleLoginClick()
        For Each form As Form In Application.OpenForms
            If TypeOf form Is FormLogin Then
                ' Form form_login sudah terbuka, keluar dari sub
                Exit Sub
            End If
        Next

        ' Jika kode mencapai sini, berarti form_login belum terbuka
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        FormLogin.MdiParent = Nothing ' Pastikan properti MdiParent tidak teratur.
        FormLogin.BringToFront()
        FormLogin.ShowDialog()
    End Sub

    Private Sub LogOutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LogOutToolStripMenuItem.Click
        ' === CLEAR CACHE SAAT LOGOUT ===
        ModulHakAkses.ClearHakAksesCache()
        TimerCekSetting.Stop()

        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()

        Call Terkunci()

        LoginToolStripMenuItem.Enabled = True

        With FormLogin
            .MdiParent = Nothing ' Pastikan properti MdiParent tidak teratur.
            .BringToFront()
            .ShowDialog()
        End With
        If String.IsNullOrWhiteSpace(StatusNamaUser.Text) Then
            Close()
            Return
        End If

        Dim pilihanMasuk As String = AppConfig.Instance.GetValue(Of String)("PilihanMasuk", "").ToUpperInvariant()
        If pilihanMasuk = "TOKO" OrElse pilihanMasuk = "GUDANG" Then
            FormMasuk.TerapkanLokasiKeFormUtama(pilihanMasuk)
        Else
            With FormMasuk
                .MdiParent = Nothing
                .BringToFront()
                .ShowDialog()
            End With
        End If
        If String.IsNullOrWhiteSpace(StatusLokasi.Text) Then
            Close()
            Return
        End If

        ' === RE-CACHE HAK AKSES SETELAH LOGIN KEMBALI ===
        If Not String.IsNullOrEmpty(StatusNamaUser.Text) Then
            ModulHakAkses.CacheHakAksesUser(StatusLevelUser.Text)
        End If

        With FormLoading
            .MdiParent = Nothing
            .BringToFront()
            .Show()
            .MulaiLoading()
        End With

        ' Pilih Penjualan sebagai tampilan default setelah re-login
        If BtnPenjualan.Visible Then
            BtnPenjualan.PerformClick()
        ElseIf BtnBelanja.Visible Then
            BtnBelanja.PerformClick()
        End If
    End Sub

    Private Sub RegristerToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RegristerToolStripMenuItem.Click
        TutupSemuaForm()
        With ACTIVATION_FORM
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    '----------------------------------------- MASTER ---------------------------------------------------------------------------

    Private Sub BtnToko_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnToko.Click
        CekaktivasiProgram()
        SetButtonBackgroundColor(BtnToko)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormCompany)
    End Sub

    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click
        SetButtonBackgroundColor(BtnBarang)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormBarang)
    End Sub

    Private Sub BtnSupliyer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSupliyer.Click
        SetButtonBackgroundColor(BtnSupliyer)
        TutupSemuaForm()
        BukaFormMdi(TambahSupliyer)
    End Sub

    Private Sub BTnPelanggan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTnPelanggan.Click
        SetButtonBackgroundColor(BTnPelanggan)
        TutupSemuaForm()
        BukaFormMdi(TambahPelanggan)
    End Sub

    Private Sub BtnUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnUser.Click
        SetButtonBackgroundColor(BtnUser)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormUser)
    End Sub

    Private Sub BtnTabelRef_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTabelRef.Click
        SetButtonBackgroundColor(BtnTabelRef)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormTabelReferensi)
    End Sub

    Private Sub BtnArmada_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnArmada.Click
        SetButtonBackgroundColor(BtnArmada)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormArmada)
    End Sub

    Private Sub BtnKaryawan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKaryawan.Click
        SetButtonBackgroundColor(BtnKaryawan)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormKaryawan)
    End Sub

    Private Sub BtnCabang_Click(sender As Object, e As EventArgs) Handles BtnKirimCabang.Click
        IniTransaksiPanel(BtnKirimCabang, "Transfer Cabang")
        TerapkanHakAkses("Transfer Cabang")
        AturTombolTransaksi("Transfer Cabang")
        BtnTambah.Text = "Transfer Antar Cabang (F2)"
        BtnPrint.Text = "Cetak Nota Transfer Cabang (F5)"
        AturTombolSettingPrinter()
        DataTransferCabang()
    End Sub

    Private Sub BtnMasterCabang_Click(sender As Object, e As EventArgs) Handles BtnMasterCabang.Click
        SetButtonBackgroundColor(BtnMasterCabang)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormCabang)
    End Sub

    Private Sub BtnHakAksesUser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHakAksesUser.Click
        SetButtonBackgroundColor(BtnHakAksesUser)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormHakUser)
    End Sub

    Private Sub BtnGeneralSetting_Click(sender As Object, e As EventArgs) Handles BtnGeneralSetting.Click
        SetButtonBackgroundColor(BtnGeneralSetting)
        TutupSemuaForm()
        BukaFormMdi(My.Forms.FormGeneralSetting)
    End Sub


    '----------------------------------------- TRANSAKSI ---------------------------------------------------------------------------

    Private Sub BtnBelanja_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBelanja.Click
        IniTransaksiPanel(BtnBelanja, "Pembelian")
        TerapkanHakAkses("Pembelian")
        AturTombolTransaksi("Pembelian")
        BtnTambah.Text = "Tambah Pembelian (F2)"
        BtnPrint.Text = "Cetak Nota Beli (F5)"
        AturTombolSettingPrinter()
        Datapembelian()
    End Sub

    Private Sub UbahTampilanDataTransaksi()
        With DGVTransaksi
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .BorderStyle = BorderStyle.FixedSingle
        End With
        ModuleTheme.ApplyThemeDataGridView(DGVTransaksi)
    End Sub

    Private Function GetFilterTabPrinter(namaTransaksi As String) As String
        Select Case namaTransaksi
            Case "Pembelian"
                Return "Beli"
            Case "Penjualan"
                Return "Jual"
            Case "Retur Pembelian"
                Return "ReturBeli"
            Case "Retur Penjualan"
                Return "ReturJual"
            Case "Bayar Hutang"
                Return "BayarHutang"
            Case "Bayar Piutang"
                Return "BayarPiutang"
            Case "Surat Jalan"
                Return "SuratJalan"
            Case "Transfer Barang"
                Return "TransferBarang"
            Case "Transfer Cabang"
                Return "TransferCabang"
            Case Else
                Return ""
        End Select
    End Function

    Private Sub AturTombolSettingPrinter()
        Dim trxKey As String = GetFilterTabPrinter(TxtTransaksi.Text)
        Dim ada As Boolean = Not String.IsNullOrEmpty(trxKey)
        BtnSettingPrinter.Visible = ada
        CmbPilihCetak.Visible = ada
        CmbProsesCetak.Visible = ada
        If ada Then
            ' Muat nilai dari file tanpa trigger SelectedIndexChanged
            RemoveHandler CmbPilihCetak.SelectedIndexChanged, AddressOf CmbPilihCetak_SelectedIndexChanged
            RemoveHandler CmbProsesCetak.SelectedIndexChanged, AddressOf CmbProsesCetak_SelectedIndexChanged
            CmbPilihCetak.Text = BacaPengaturanPrinter(trxKey, "CetakOtomatis", "IYA")
            CmbProsesCetak.Text = BacaPengaturanPrinter(trxKey, "PilihPrinter", "LANGSUNG CETAK")
            AddHandler CmbPilihCetak.SelectedIndexChanged, AddressOf CmbPilihCetak_SelectedIndexChanged
            AddHandler CmbProsesCetak.SelectedIndexChanged, AddressOf CmbProsesCetak_SelectedIndexChanged
        End If
    End Sub

    Private Sub CmbPilihCetak_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim trxKey As String = GetFilterTabPrinter(TxtTransaksi.Text)
        If Not String.IsNullOrEmpty(trxKey) Then
            TulisPengaturanPrinter(trxKey, "CetakOtomatis", CmbPilihCetak.Text)
        End If
    End Sub

    Private Sub CmbProsesCetak_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim trxKey As String = GetFilterTabPrinter(TxtTransaksi.Text)
        If Not String.IsNullOrEmpty(trxKey) Then
            TulisPengaturanPrinter(trxKey, "PilihPrinter", CmbProsesCetak.Text)
        End If
    End Sub



    Public Sub Datapembelian()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(GRAND_TOTAL_BELI), 0) AS TOTAL FROM pembelian WHERE TGL_BELI >= @tanggalAwal AND TGL_BELI <= @tanggalAkhir AND ID_PEMBELIAN LIKE @SearchText", "Total Belanja", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_PEMBELIAN, NAMA_SUPLIYER, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, PEMBAYARAN, RETUR, TAGIHAN, STATUS_TRANSAKSI_BELI, ID_USER FROM pembelian WHERE TGL_BELI >= @tanggalAwal AND TGL_BELI <= @tanggalAkhir AND ID_PEMBELIAN LIKE @SearchText ORDER BY ID_PEMBELIAN ASC", "pembelian", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).HeaderText = "SUPLIYER" : .Columns(2).HeaderText = "LOKASI"
            .Columns(3).HeaderText = "R KREDIT" : .Columns(4).HeaderText = "TOTAL" : .Columns(5).HeaderText = "PEMBAYARAN"
            .Columns(6).HeaderText = "RETUR" : .Columns(7).HeaderText = "HUTANG" : .Columns(8).HeaderText = "STATUS" : .Columns(9).HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, 4, 5, 6, 7)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Pembelian : ")
    End Sub

    Private Sub BtnPenjualan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPenjualan.Click
        IniTransaksiPanel(BtnPenjualan, "Penjualan")
        TerapkanHakAkses("Penjualan")
        AturTombolTransaksi("Penjualan")
        BtnPrint.Text = "Cetak Struk Jual (F5)"
        AturTombolSettingPrinter()
        Datapenjualan()
    End Sub

    Public Sub Datapenjualan()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(GRAND_TOTAL_STL_PAJAK), 0) AS TOTAL FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_PENJUALAN LIKE @SearchText", "Total Penjualan", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_PENJUALAN, NAMA_PELANGGAN, LOKASIBARANG, JENIS_PEMBAYARAN, GRAND_TOTAL_STL_PAJAK, BAYAR, NOMINAL_TRANSFER, KEMBALI, NILAI_RETUR, SISA_TAGIHAN, STATUS_TRANSAKSI, ID_USER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_PENJUALAN LIKE @SearchText ORDER BY ID_PENJUALAN ASC", "penjualan", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns("ID_PENJUALAN").HeaderText = "NOTA" : .Columns(0).FillWeight = 130
            .Columns("NAMA_PELANGGAN").HeaderText = "PELANGGAN" : .Columns("LOKASIBARANG").HeaderText = "LOKASI"
            .Columns("JENIS_PEMBAYARAN").HeaderText = "R DEBET" : .Columns("GRAND_TOTAL_STL_PAJAK").HeaderText = "TOTAL"
            .Columns("BAYAR").HeaderText = "BAYAR" : .Columns("NOMINAL_TRANSFER").HeaderText = "TRANSFER"
            .Columns("KEMBALI").HeaderText = "KEMBALI" : .Columns("NILAI_RETUR").HeaderText = "RETUR"
            .Columns("SISA_TAGIHAN").HeaderText = "PIUTANG" : .Columns("STATUS_TRANSAKSI").HeaderText = "STATUS" : .Columns("ID_USER").HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, "GRAND_TOTAL_STL_PAJAK", "BAYAR", "NOMINAL_TRANSFER", "NILAI_RETUR", "KEMBALI", "SISA_TAGIHAN")
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Penjualan : ")
    End Sub

    Private Sub BtnRetuBelanja_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnRetuBelanja.Click
        IniTransaksiPanel(BtnRetuBelanja, "Retur Pembelian")
        TerapkanHakAkses("Retur Pembelian", showPrint:=False)
        AturTombolTransaksi("Retur Beli")
        BtnPrint.Text = "Cetak Nota Retur Beli (F5)"
        AturTombolSettingPrinter()
        DatareturPembelian()
    End Sub

    Public Sub DatareturPembelian()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(TOTAL_RUPIAH), 0) AS TOTAL FROM retur_pembelian WHERE TGL_RETUR_BELI >= @tanggalAwal AND TGL_RETUR_BELI <= @tanggalAkhir AND ID_RETUR_PEMBELIAN LIKE @SearchText", "Total Retur Beli", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_RETUR_PEMBELIAN, NAMA_SUPPLIER, ID_PEMBELIAN, TGL_PEMBELIAN, PENYIMPANAN, TOTAL_BARANG, TOTAL_RUPIAH, NAMA_REKENING, KODE_REKENING, ID_USER FROM retur_pembelian WHERE TGL_RETUR_BELI >= @tanggalAwal AND TGL_RETUR_BELI <= @tanggalAkhir AND ID_RETUR_PEMBELIAN LIKE @SearchText ORDER BY ID_RETUR_PEMBELIAN ASC", "retur_pembelian", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).HeaderText = "SUPLIYER" : .Columns(2).HeaderText = "NO BELI"
            .Columns(3).HeaderText = "TGL BELI" : .Columns(4).HeaderText = "LOKASI" : .Columns(5).HeaderText = "BARANG"
            .Columns(6).HeaderText = "TOTAL" : .Columns(7).HeaderText = "REKENING" : .Columns(8).Visible = False : .Columns(9).HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, 6)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Retur Pembelian : ")
    End Sub

    Private Sub BtnReturPenjualan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnReturPenjualan.Click
        IniTransaksiPanel(BtnReturPenjualan, "Retur Penjualan")
        TerapkanHakAkses("Retur Penjualan", showEdit:=False)
        AturTombolTransaksi("Retur Jual")
        BtnPrint.Text = "Cetak Nota Retur Jual (F5)"
        AturTombolSettingPrinter()
        DataReturPenjualan()
    End Sub

    Public Sub DataReturPenjualan()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(TOTAL_RUPIAH), 0) AS TOTAL FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @tanggalAwal AND TGL_RETUR_JUAL <= @tanggalAkhir AND ID_RETUR_PENJUALAN LIKE @SearchText", "Total Retur Jual", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_RETUR_PENJUALAN, NAMA_PELANGGAN, ID_PENJUALAN, TGL_PENJUALAN, PENYIMPANAN, TOTAL_BARANG, TOTAL_RUPIAH, NAMA_REKENING, ID_USER FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @tanggalAwal AND TGL_RETUR_JUAL <= @tanggalAkhir AND ID_RETUR_PENJUALAN LIKE @SearchText ORDER BY ID_RETUR_PENJUALAN ASC", "retur_penjualan", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).HeaderText = "PELANGGAN" : .Columns(2).HeaderText = "NO JUAL"
            .Columns(3).HeaderText = "TGL JUAL" : .Columns(4).HeaderText = "LOKASI" : .Columns(5).HeaderText = "BARANG"
            .Columns(6).HeaderText = "TOTAL" : .Columns(7).HeaderText = "REKENING" : .Columns(8).HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, 6)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Retur Penjualan : ")
    End Sub

    Private Sub BtnBayarHutang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayarHutang.Click
        IniTransaksiPanel(BtnBayarHutang, "Bayar Hutang")
        TerapkanHakAkses("Bayar Hutang", showEdit:=False)
        AturTombolTransaksi("Bayar Hutang")
        BtnPrint.Text = "Cetak Nota Bayar Hutang (F5)"
        AturTombolSettingPrinter()
        DataBayarHutang()
    End Sub

    Public Sub DataBayarHutang()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(NOMINALBAYAR), 0) AS TOTAL FROM hutang WHERE TGLPEMBAYARAN >= @tanggalAwal AND TGLPEMBAYARAN <= @tanggalAkhir AND NOBAYARHUTANG LIKE @SearchText", "Total Bayar Hutang", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT NOBAYARHUTANG, NAMASUPLIYER, TGLPEMBAYARAN, TOTALHUTANG, NOMINALBAYAR, SISAHUTANG, ID_USER_BAYAR FROM hutang WHERE TGLPEMBAYARAN >= @tanggalAwal AND TGLPEMBAYARAN <= @tanggalAkhir AND NOBAYARHUTANG LIKE @SearchText ORDER BY NOBAYARHUTANG ASC", "hutang", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).HeaderText = "SUPPLIER" : .Columns(2).HeaderText = "TGL BAYAR"
            .Columns(3).HeaderText = "TOTAL" : .Columns(4).HeaderText = "BAYAR" : .Columns(5).HeaderText = "SISA" : .Columns(6).HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, 3, 4, 5)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Bayar Hutang : ")
    End Sub

    Private Sub BtnBayarPiutang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBayarPiutang.Click
        IniTransaksiPanel(BtnBayarPiutang, "Bayar Piutang")
        TerapkanHakAkses("Bayar Piutang", showEdit:=False)
        AturTombolTransaksi("Bayar Piutang")
        BtnPrint.Text = "Cetak Nota Bayar Piutang (F5)"
        AturTombolSettingPrinter()
        DataBayarPiutang()
    End Sub

    Public Sub DataBayarPiutang()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(NOMINAL_BAYAR), 0) AS TOTAL FROM Piutang WHERE TGL_BAYAR >= @tanggalAwal AND TGL_BAYAR <= @tanggalAkhir AND ID_BAYAR_PIUTANG LIKE @SearchText", "Total Bayar Piutang", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_BAYAR_PIUTANG, NAMA_PELANGGAN, TGL_BAYAR, TOTAL_PIUTANG, NOMINAL_BAYAR, SISA_PIUTANG, ID_USER_BAYAR FROM Piutang WHERE TGL_BAYAR >= @tanggalAwal AND TGL_BAYAR <= @tanggalAkhir AND ID_BAYAR_PIUTANG LIKE @SearchText ORDER BY ID_BAYAR_PIUTANG ASC", "Piutang", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).HeaderText = "PELANGGAN" : .Columns(2).HeaderText = "TGL BAYAR"
            .Columns(3).HeaderText = "TOTAL" : .Columns(4).HeaderText = "BAYAR" : .Columns(5).HeaderText = "SISA" : .Columns(6).HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, 3, 4, 5)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Bayar Piutang : ")
    End Sub

    Private Sub BtnStokOpname_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStokOpname.Click
        IniTransaksiPanel(BtnStokOpname, "Stok Opname", showDetail:=False)
        TerapkanHakAkses("Stok Opname")
        AturTombolTransaksi("Stok Opname")
        BtnPrint.Text = "Cetak Nota Stok Opname (F5)"
        AturTombolSettingPrinter()
        DataStokOpname()
    End Sub

    Public Sub DataStokOpname()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(TOTAL_QTY), 0) AS TOTAL FROM stok_opname WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir AND ID_STOK_OPNAME LIKE @SearchText", "Total Selisih", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_STOK_OPNAME, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, TOTAL_QTY, SATUAN, KETERANGAN, ID_USER FROM stok_opname WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir AND ID_STOK_OPNAME LIKE @SearchText ORDER BY ID_STOK_OPNAME ASC", "stok_opname", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).HeaderText = "LOKASI" : .Columns(2).HeaderText = "KODE"
            .Columns(3).HeaderText = "NAMA BARANG" : .Columns(4).HeaderText = "KATEGORI"
            .Columns(5).HeaderText = "S SYSTEM" : .Columns(5).FillWeight = 60
            .Columns(6).HeaderText = "S NYATA" : .Columns(6).FillWeight = 60
            .Columns(7).HeaderText = "SELISIH" : .Columns(7).FillWeight = 60
            .Columns(8).Visible = False : .Columns(9).HeaderText = "SATUAN"
            .Columns(10).HeaderText = "KETERANGAN" : .Columns(11).HeaderText = "ID USER"
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi()
    End Sub

    Private Sub BtnSuratJalan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSuratJalan.Click
        IniTransaksiPanel(BtnSuratJalan, "Surat Jalan")
        TerapkanHakAkses("Surat Jalan")
        AturTombolTransaksi("Surat Jalan")
        BtnPrint.Text = "Cetak Surat Jalan (F5)"
        AturTombolSettingPrinter()
        DataSuratjalan()
    End Sub

    Public Sub DataSuratjalan()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(TOTAL_RUPIAH), 0) AS TOTAL FROM Surat_Jalan WHERE TGL_PENGIRIMAN >= @tanggalAwal AND TGL_PENGIRIMAN <= @tanggalAkhir AND NOTA LIKE @SearchText", "Total Pengiriman", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT NOTA, TOTAL_PELANGGAN, TOTAL_RUPIAH, ARMADA, JENIS_ARMADA, SUPIR, HELPER1, HELPER2, ID_USER FROM Surat_Jalan WHERE TGL_PENGIRIMAN >= @tanggalAwal AND TGL_PENGIRIMAN <= @tanggalAkhir AND NOTA LIKE @SearchText ORDER BY NOTA ASC", "SuratJalan", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns("NOTA").HeaderText = "NOTA" : .Columns("NOTA").FillWeight = 130
            .Columns("TOTAL_PELANGGAN").HeaderText = "PELANGGAN" : .Columns("TOTAL_RUPIAH").HeaderText = "RUPIAH"
            .Columns("ARMADA").HeaderText = "ARMADA" : .Columns("JENIS_ARMADA").HeaderText = "ARMADA"
            .Columns("SUPIR").HeaderText = "SUPIR" : .Columns("HELPER1").HeaderText = "HELPER 1"
            .Columns("HELPER2").HeaderText = "HELPER 2" : .Columns("ID_USER").HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, "TOTAL_RUPIAH")
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Surat Jalan : ")
    End Sub

    Private Sub BtnTransferBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTransferBarang.Click
        IniTransaksiPanel(BtnTransferBarang, "Transfer Barang")
        TerapkanHakAkses("Transfer Barang")
        AturTombolTransaksi("Transfer Barang")
        BtnPrint.Text = "Cetak Transfer Barang (F5)"
        AturTombolSettingPrinter()
        DataTransferBarang()
    End Sub

    Public Sub DataTransferBarang()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(TOTAL_RUPIAH), 0) AS TOTAL FROM Transfer_Barang WHERE TGL_TRANSFER >= @tanggalAwal AND TGL_TRANSFER <= @tanggalAkhir AND ID_TRANSFER LIKE @SearchText", "Total transfer barang", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_TRANSFER, LOKASI, TOTAL_QTY, TOTAL_BARANG, TOTAL_RUPIAH, ID_USER FROM Transfer_Barang WHERE TGL_TRANSFER >= @tanggalAwal AND TGL_TRANSFER <= @tanggalAkhir AND ID_TRANSFER LIKE @SearchText ORDER BY ID_TRANSFER ASC", "TransferBarang", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns("ID_TRANSFER").HeaderText = "ID Transfer" : .Columns("ID_TRANSFER").FillWeight = 130
            .Columns("LOKASI").HeaderText = "Lokasi" : .Columns("TOTAL_QTY").HeaderText = "Total Qty"
            .Columns("TOTAL_BARANG").HeaderText = "Total Barang" : .Columns("TOTAL_RUPIAH").HeaderText = "Total Rupiah" : .Columns("ID_USER").HeaderText = "User"
            AturKolomAngka(DGVTransaksi, "TOTAL_QTY", "TOTAL_BARANG", "TOTAL_RUPIAH")
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Transfer Barang : ")
    End Sub

    Public Sub DataTransferCabang()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(TOTAL_RUPIAH), 0) AS TOTAL FROM transfer_cabang WHERE TGL_TRANSFER >= @tanggalAwal AND TGL_TRANSFER <= @tanggalAkhir AND ID_TRANSFER LIKE @SearchText", "Total nilai", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_TRANSFER, TGL_TRANSFER, DARI_CABANG, KE_CABANG, MODE_KIRIM, TOTAL_QTY, TOTAL_BARANG, TOTAL_RUPIAH, STATUS_TRANSFER, ID_USER FROM transfer_cabang WHERE TGL_TRANSFER >= @tanggalAwal AND TGL_TRANSFER <= @tanggalAkhir AND ID_TRANSFER LIKE @SearchText ORDER BY ID_TRANSFER ASC", "TransferCabang", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns("ID_TRANSFER").HeaderText = "ID Transfer" : .Columns("ID_TRANSFER").FillWeight = 120
            .Columns("TGL_TRANSFER").HeaderText = "Tanggal" : .Columns("TGL_TRANSFER").FillWeight = 110
            .Columns("DARI_CABANG").HeaderText = "Dari Cabang" : .Columns("KE_CABANG").HeaderText = "Ke Cabang"
            .Columns("MODE_KIRIM").HeaderText = "Mode" : .Columns("TOTAL_QTY").HeaderText = "Total Qty"
            .Columns("TOTAL_BARANG").HeaderText = "Jml Item" : .Columns("TOTAL_RUPIAH").HeaderText = "Total Nilai"
            .Columns("STATUS_TRANSFER").HeaderText = "Status" : .Columns("ID_USER").HeaderText = "User"
            AturKolomAngka(DGVTransaksi, "TOTAL_QTY", "TOTAL_BARANG", "TOTAL_RUPIAH")
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi("Detail Transfer Cabang : ")
    End Sub

    Private Sub BtnPindahStok_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPindahStok.Click
        IniTransaksiPanel(BtnPindahStok, "Transfer Stok", showDetail:=False)
        Dim ha As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Transfer Stok")
        BtnTambah.Visible = ha(1)
        BTNEdit.Visible = False
        BtnHapus.Visible = ha(3)
        BtnPrint.Visible = False
        AturTombolTransaksi("Transfer Stok")
        BtnPrint.Text = "Cetak Nota Transfer Stok (F5)"
        AturTombolSettingPrinter()
        Datatransferstok()
    End Sub

    Public Sub Datatransferstok()
        Dim sf As String = "%" & TxtFilter.Text & "%"
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman("SELECT COUNT(*) AS RECORD, COALESCE(SUM(Selisih), 0) AS TOTAL FROM Transfer_stok WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir AND ID_TRANSFER LIKE @SearchText", "Total Selisih", tAwal, tAkhir, sf)
        LoadDataTransaksi("SELECT ID_TRANSFER, JENIS_TRANSFER, URAIAN, TANGGAL, ID_BARANG_M, NAMA_BARANG_M, QTY_M, SATUAN_M, ISI_M, QTY_SAT_M, HARGA_SAT_M, TOTAL_HARGA_M, ID_BARANG_K, NAMA_BARANG_K, QTY_K, SATUAN_K, ISI_K, QTY_SAT_K, HARGA_SAT_K, TOTAL_HARGA_K, Selisih, ID_USER FROM Transfer_stok WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir AND ID_TRANSFER LIKE @SearchText ORDER BY ID_TRANSFER ASC", "Transfer_stok", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = "NOTA" : .Columns(1).Visible = False : .Columns(2).HeaderText = "URAIAN" : .Columns(2).FillWeight = 170
            .Columns(3).Visible = False : .Columns(4).Visible = False
            .Columns(5).HeaderText = "BARANG MASUK" : .Columns(5).FillWeight = 170
            .Columns(6).HeaderText = "QTY" : .Columns(6).FillWeight = 50 : .Columns(7).HeaderText = "SAT" : .Columns(7).FillWeight = 60
            .Columns(8).Visible = False : .Columns(9).Visible = False : .Columns(10).Visible = False
            .Columns(11).HeaderText = "HARGA MASUK" : .Columns(12).Visible = False
            .Columns(13).HeaderText = "BARANG KELUAR" : .Columns(13).FillWeight = 170
            .Columns(14).HeaderText = "QTY" : .Columns(14).FillWeight = 50 : .Columns(15).HeaderText = "SAT" : .Columns(15).FillWeight = 60
            .Columns(16).Visible = False : .Columns(17).Visible = False : .Columns(18).Visible = False
            .Columns(19).HeaderText = "HARGA KELUAR" : .Columns(20).HeaderText = "SELISIH" : .Columns(20).FillWeight = 80 : .Columns(21).HeaderText = "USER"
            AturKolomAngka(DGVTransaksi, 6, 11, 19, 20)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi()
    End Sub



    Private Sub DtpTransaksi_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DtpTransaksi.ValueChanged, TxtFilter.TextChanged
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                Datapembelian()
            Case "Penjualan"
                Datapenjualan()
            Case "Retur Pembelian"
                DatareturPembelian()
            Case "Retur Penjualan"
                DataReturPenjualan()
            Case "Bayar Hutang"
                DataBayarHutang()
            Case "Bayar Piutang"
                DataBayarPiutang()
            Case "Stok Opname"
                DataStokOpname()
            Case "Transfer Stok"
                Datatransferstok()
            Case "Surat Jalan"
                DataSuratjalan()
            Case "Transfer Barang"
                DataTransferBarang()
            Case "Transfer Cabang"
                DataTransferCabang()
        End Select
    End Sub

    Private Sub FormUtama_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                If BtnTambah.Visible = True Then
                    Tambahtransaksi()
                End If
            Case Keys.F3
                If BTNEdit.Visible = True Then
                    Edittransaksi()
                End If
            Case Keys.F4
                If BtnHapus.Visible = True Then
                    Hapustransaksi()
                End If
            Case Keys.F5
                If BtnPrint.Visible = True Then
                    Cetaktransaksi()
                End If
            Case Keys.Escape
                GBTransaksi.Hide()
        End Select
    End Sub

    Private Sub DGVTransaksi_SelectionChanged(sender As Object, e As EventArgs) Handles DGVTransaksi.SelectionChanged
        ' Navigasi keyboard (panah atas/bawah) tidak memicu CellClick,
        ' jadi kita panggil ulang logika yang sama saat baris aktif berubah.
        If DGVTransaksi.CurrentRow Is Nothing OrElse DGVTransaksi.CurrentRow.Cells(0).Value Is Nothing Then Return
        DGVTransaksi_CellClick(sender, New DataGridViewCellEventArgs(0, DGVTransaksi.CurrentRow.Index))
    End Sub

    Private Sub DGVTransaksi_CellMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DGVTransaksi.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Memastikan bahwa baris yang diklik valid
            If e.RowIndex < DGVTransaksi.Rows.Count AndAlso DGVTransaksi.Rows(e.RowIndex).Cells(0).Value IsNot Nothing Then
                DGVTransaksi.CurrentCell = DGVTransaksi.Rows(e.RowIndex).Cells(0)
                ' Memanggil event CellClick untuk baris yang valid
                DGVTransaksi_CellClick(sender, New System.Windows.Forms.DataGridViewCellEventArgs(0, e.RowIndex))

                ' === OPTIMASI: Baca hak akses dari CACHE (instant, tanpa DB query) ===
                EditPembayaranToolStripMenuItem.Visible = False
                Select Case TxtTransaksi.Text
                    Case "Pembelian"
                        TerapkanHakAksesContextMenu("Pembelian")
                    Case "Penjualan"
                        TerapkanHakAksesContextMenu("Penjualan", showEditBayar:=True)
                    Case "Retur Pembelian"
                        TerapkanHakAksesContextMenu("Retur Pembelian", showEdit:=False)
                    Case "Retur Penjualan"
                        TerapkanHakAksesContextMenu("Retur Penjualan", showEdit:=False)
                    Case "Bayar Hutang"
                        TerapkanHakAksesContextMenu("Bayar Hutang", showEdit:=False)
                    Case "Bayar Piutang"
                        TerapkanHakAksesContextMenu("Bayar Piutang", showEdit:=False)
                    Case "Stok Opname"
                        TerapkanHakAksesContextMenu("Stok Opname")
                    Case "Transfer Stok"
                        TerapkanHakAksesContextMenu("Transfer Stok", showEdit:=False, showCetak:=False)
                    Case "Transfer Barang"
                        TerapkanHakAksesContextMenu("Transfer Barang")
                    Case "Transfer Cabang"
                        TerapkanHakAksesContextMenu("Transfer Cabang")
                    Case "Surat Jalan"
                        TerapkanHakAksesContextMenu("Surat Jalan")
                End Select
                Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                CMSTransaksi.Show(cursorPosition)

            End If
        End If
    End Sub

    Private Sub TambahToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TambahToolStripMenuItem.Click
        Tambahtransaksi()
        Refresdatagridview()
    End Sub

    Private Sub EditToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditToolStripMenuItem.Click
        Edittransaksi()
        Refresdatagridview()
    End Sub

    Private Sub HapusToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HapusToolStripMenuItem.Click
        Hapustransaksi()
        Refresdatagridview()
    End Sub

    Private Sub CetakToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CetakToolStripMenuItem.Click
        Cetaktransaksi()
    End Sub

    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Try
            CekaktivasiProgram()
            Tambahtransaksi()
            Refresdatagridview()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Tambahtransaksi()
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                With FormPembelian
                    IsiComboBoxAkun(.CmbAkunTunai, "KAS", "EKUITAS")
                    IsiComboBoxAkun(.CmbAkunTransfer, "BANK")
                    .LblHeader.Text = "T A M B A H  P E M B E L I A N"
                    .TxtJenisTrans.Text = "TambahPembelian"
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog(Me)
                End With

            Case "Penjualan"
                With FormJual

                    .TxtJenistransaksi.Text = "TambahPenjualan"
                    .DgvDataTransaksi.Rows.Clear()
                    .BringToFront()
                    .ShowDialog(Me)
                End With

            Case "Retur Pembelian"
                With FormReturBeli
                    .LblJenisTrans.Text = "TambahReturBeli"
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog(Me)
                End With

            Case "Retur Penjualan"
                GBTransaksi.Visible = False
                With FormReturPenjualan
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Bayar Hutang"
                GBTransaksi.Visible = False
                With FormBayarHutang
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Bayar Piutang"
                GBTransaksi.Visible = False
                With FormBayarPiutang
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Stok Opname"
                GBTransaksi.Visible = False
                With FormStokOpname
                    .LblUtama.Text = "TAMBAH STOK OPNAME"
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Transfer Stok"
                GBTransaksi.Visible = False

                With FormTransferStok
                    .LblHeaderForm.Text = "TRANSFER STOK ANTAR BARANG DI " & StatusLokasi.Text
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show() '
                End With

            Case "Transfer Barang"
                With FormTransferBarang
                    If StatusLokasi.Text = "TOKO" Then
                        .LblLokasiBarang.Text = "TOKO"
                        .LblUtama.Text = "TRANSFER STOK DARI TOKO KE GUDANG"
                    ElseIf StatusLokasi.Text = "GUDANG" Then
                        .LblLokasiBarang.Text = "GUDANG"
                        .LblUtama.Text = "TRANSFER STOK DARI GUDANG KE TOKO"
                    End If
                    .LblJenisTrans.Text = "TambahTransfer"
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog()
                End With

            Case "Transfer Cabang"
                GBTransaksi.Visible = False
                With FormTransferCabang
                    .LokasiBarang = StatusLokasi.Text
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Surat Jalan" '
                GBTransaksi.Visible = False
                With FormSuratJalan
                    .LblJenisTrans.Text = "TambahSuratJalan"
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show() 'Surat Jalan
                End With
        End Select

    End Sub

    Private Sub BTNEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNEdit.Click
        Try
            Edittransaksi()
            Refresdatagridview()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Edittransaksi()
        If TxtFakturTransaksi.Text = "" Then
            MessageBox.Show("Pilih Data yang akan di Edit ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Select Case TxtTransaksi.Text
            Case "Pembelian"

                If StatusLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit pembelian ini." & Environment.NewLine &
                                          "User " & StatusLokasi.Text & " tidak berhak edit transaksi pembelian " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()
                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_pembelian WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PEMBELIAN", idPembelian)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, buka FormPembelian untuk edit


                        With FormPembelian
                            IsiComboBoxAkun(.CmbAkunTunai, "KAS", "EKUITAS")
                            IsiComboBoxAkun(.CmbAkunTransfer, "BANK")

                            .LblHeader.Text = "E D I T  P E M B E L I A N"
                            .TxtJenisTrans.Text = "EditPembelian"
                            .TxtIdPembelian.Text = idPembelian
                            .AmbilDataSupplier()
                            .BringToFront()
                            .ShowDialog(Me)
                        End With
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat diedit karena terdapat Retur pembelian pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur pembelian pada transaksi ini jika ingin melanjutkan proses pengeditan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Penjualan"

                If StatusLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit penjualan ini." & Environment.NewLine &
                                          "User " & StatusLokasi.Text & " tidak berhak edit transaksi penjualan " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim idpenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()

                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PENJUALAN", idpenjualan)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Penjualan
                        With FormJual
                            .TxtJenistransaksi.Text = "EditPenjualan"
                            .TxtFaktur.Text = idpenjualan
                            .TampilPelanggan()
                            .AmbilDataKaryawan()
                            .BringToFront()
                            .ShowDialog(Me)
                        End With
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat diedit karena terdapat Retur penjualan pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur penjualan pada transaksi ini jika ingin melanjutkan proses pengeditan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Retur Pembelian"
                If StatusLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit retur pembelian ini." & Environment.NewLine &
                                          "User " & StatusLokasi.Text & " tidak berhak edit transaksi retur pembelian " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim idretrubeli As String = DGVTransaksi.CurrentRow.Cells("ID_RETUR_PEMBELIAN").Value.ToString()
                With FormReturBeli
                    .LblJenisTrans.Text = "EditReturBeli"
                    .TxtFaktur.Text = idretrubeli
                    .BringToFront()
                    .ShowDialog(Me)
                End With


            Case "Retur Penjualan"
                ' tidak membuat edit
            Case "Bayar Hutang"
                ' tidak membuat edit
            Case "Bayar Piutang"
                ' tidak membuat edit
            Case "Stok Opname"
                GBTransaksi.Visible = False
                With FormStokOpname
                    .LblUtama.Text = "EDIT STOK OPNAME"
                    .TxtFaktur.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    .TxtQtyUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(8).Value.ToString()
                    .Panel3.Enabled = False
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With
            Case "Transfer Stok"
                ' tidak membuat edit

            Case "Transfer Barang"
                If StatusLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit transfer barang ini." & Environment.NewLine &
                                          "User " & StatusLokasi.Text & " tidak berhak edit transaksi edit barang " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                With FormTransferBarang
                    If TxtLokasiUntukEdit.Text = "TOKO" Then
                        .LblLokasiBarang.Text = "TOKO"
                        .LblUtama.Text = "EDIT TRANSFER STOK DARI TOKO KE GUDANG"
                    ElseIf TxtLokasiUntukEdit.Text = "GUDANG" Then
                        .LblLokasiBarang.Text = "GUDANG"
                        .LblUtama.Text = "EDIT TRANSFER STOK DARI GUDANG KE TOKO"
                    End If
                    .LblJenisTrans.Text = "EditTransfer"
                    .TxtFaktur.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog()
                End With
            Case "Transfer Cabang"
                ' Transfer Cabang tidak mendukung edit — hanya bisa hapus dan buat ulang
                MessageBox.Show("Edit Transfer Cabang tidak tersedia. Hapus transaksi ini dan buat yang baru.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Case "Surat Jalan"
                ' Mengedit data Surat Jalan
                With FormSuratJalan
                    GBTransaksi.Visible = False
                    ' Set jenis transaksi
                    .LblJenisTrans.Text = "EditSuratJalan"
                    .AmbilDataArmada()
                    .AmbilDataKaryawan()

                    ' Mengisi data dari DataGridView
                    .LblNoNota.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                    Dim armadaIndex As String = DGVTransaksi.CurrentRow.Cells(3).Value.ToString()
                    .CmbArmada.Text = armadaIndex

                    Dim sopir As String = DGVTransaksi.CurrentRow.Cells(5).Value.ToString()
                    .CmbSopir.Text = sopir

                    Dim helper1 As String = DGVTransaksi.CurrentRow.Cells(6).Value.ToString()
                    .CmbHelper1.Text = helper1

                    Dim helper2 As String = DGVTransaksi.CurrentRow.Cells(7).Value.ToString()
                    .CmbHelper2.Text = helper2

                    ' Mengatur tanggal
                    .DtpSuratJalan.Value = DtpTransaksi.Value
                    .DtpPenjualan.Value = DtpTransaksi.Value

                    ' Membersihkan dan menampilkan DataGridView Surat Jalan
                    .DGVSuratJalan.Rows.Clear()


                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

        End Select

    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHapus.Click
        Try
            Hapustransaksi()
            Refresdatagridview()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function CekLokasiBarang() As Boolean
        If StatusLokasi.Text <> TxtLokasiUntukEdit.Text Then
            Dim pesan As String = "Oops! Tidak ada hak untuk menghapus transaksi ini." & Environment.NewLine &
                                  "Karena login di " & StatusLokasi.Text & " tidak berhak menghapus/edit transaksi " & TxtLokasiUntukEdit.Text
            MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Sub Hapustransaksi()
        If TxtFakturTransaksi.Text = "" Then
            MessageBox.Show("Pilih Data yang akan di Hapus ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Select Case TxtTransaksi.Text
            Case "Pembelian"

                If Not CekLokasiBarang() Then Exit Sub

                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()

                Using cmdCheck As New MySqlCommand("SELECT COUNT(ID_RETUR_PEMBELIAN) FROM retur_pembelian WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PEMBELIAN", idPembelian)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Pembelian
                        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                            Hapusbelanja()
                        End If
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat hapus karena terdapat Retur pembelian pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur pembelian pada transaksi ini jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Penjualan"

                If Not CekLokasiBarang() Then Exit Sub

                Dim idpenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()

                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PENJUALAN", idpenjualan)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Penjualan
                        Hapuspenjualan()
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat dihapus karena terdapat Retur penjualan pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur penjualan pada transaksi ini jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Retur Pembelian"

                If Not CekLokasiBarang() Then Exit Sub

                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()

                ' Query untuk mendapatkan NOBAYARHUTANG — filter JENIS='BAYAR' agar baris TIMBUL tidak ikut
                Dim queryNobayar As String = "SELECT ID_BAYAR FROM Hutang_Detail WHERE ID_BELI = @ID_BELI AND JENIS = 'BAYAR'"
                Using cmdNobayar As New MySqlCommand(queryNobayar, conn)
                    cmdNobayar.Parameters.AddWithValue("@ID_BELI", idPembelian)

                    ' Mengecek NOBAYARHUTANG
                    Dim noBayarHutang As Object = cmdNobayar.ExecuteScalar()

                    If noBayarHutang Is Nothing OrElse IsDBNull(noBayarHutang) OrElse String.IsNullOrWhiteSpace(noBayarHutang.ToString()) Then
                        ' NOBAYARHUTANG kosong atau null, maka lanjutkan dengan menghapus retur pembelian
                        Hapusreturpembelian()
                        'RefreshDataGridView()
                    Else
                        ' NOBAYARHUTANG tidak kosong, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat dihapus karena terdapat pembayaran hutang terkait." & Environment.NewLine &
                                              "Silakan batalkan pembayaran hutang terlebih dahulu jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Retur Penjualan"
                If Not CekLokasiBarang() Then Exit Sub

                Dim idpenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()

                ' Query untuk mendapatkan NOBAYARHUTANG — filter JENIS='BAYAR' agar baris TIMBUL tidak ikut
                Dim queryNobayar As String = "SELECT ID_BAYAR FROM Piutang_Detail WHERE ID_JUAL = @ID_JUAL AND JENIS = 'BAYAR'"
                Using cmdNobayar As New MySqlCommand(queryNobayar, conn)
                    cmdNobayar.Parameters.AddWithValue("@ID_JUAL", idpenjualan)

                    ' Mengecek NOBAYARHUTANG
                    Dim idBayarPiutang As Object = cmdNobayar.ExecuteScalar()

                    If idBayarPiutang Is Nothing OrElse IsDBNull(idBayarPiutang) OrElse String.IsNullOrWhiteSpace(idBayarPiutang.ToString()) Then
                        ' NOBAYARHUTANG kosong atau null, maka lanjutkan dengan menghapus retur pembelian
                        Hapusreturpenjualan()
                    Else
                        ' NOBAYARHUTANG tidak kosong, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat dihapus karena terdapat pembayaran piutang terkait." & Environment.NewLine &
                                              "Silakan batalkan pembayaran piutang terlebih dahulu jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Bayar Hutang"
                Hapusbayarhutang()
            Case "Bayar Piutang"
                HapusbayarPiutang()
            Case "Stok Opname"
                If Not CekLokasiBarang() Then Exit Sub
                Hapusstokopname()
            Case "Transfer Stok"
                If Not CekLokasiBarang() Then Exit Sub
                Hapustransferstok()
            Case "Surat Jalan"
                HapusSuratJalan()
            Case "Transfer Barang"
                If Not CekLokasiBarang() Then Exit Sub
                HapusTransferBarang()
            Case "Transfer Cabang"
                HapusTransferCabang()
        End Select

    End Sub

    Public Sub Hapusbelanja()

        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' ========================================
            ' START: Audit Trail - Hapus Pembelian
            ' ========================================
            ModuleAuditTrail.CatatAudit(TxtFakturTransaksi.Text, "HAPUS", "Pembelian", trans:=transaction)
            ' ========================================
            ' END: Audit Trail - Hapus Pembelian
            ' ========================================

            ModuleHapusTransaksi.HapusPembelian(TxtFakturTransaksi.Text, TxtLokasiUntukEdit.Text, transaction)
            transaction.Commit()

        Catch ex As OperationCanceledException
            transaction.Rollback()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                                "Detail kesalahan: " & ex.Message,
                                                "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Public Sub Hapuspenjualan()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' 1. Catat Audit Trail
                ModuleAuditTrail.CatatAudit(TxtFakturTransaksi.Text, "HAPUS", "Penjualan", trans:=transaction)

                ' 2. Panggil fungsi pusat (logika reversal stok, piutang, jurnal, saldo 100% akurat)
                ModuleHapusTransaksi.HapusPenjualan(TxtFakturTransaksi.Text, TxtLokasiUntukEdit.Text, transaction)

                transaction.Commit()
                ' Refresdatagridview() dipanggil otomatis oleh BtnHapus_Click
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan saat menghapus penjualan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Hapusreturpembelian()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' 1. Catat Audit Trail
                ModuleAuditTrail.CatatAudit(TxtFakturTransaksi.Text, "HAPUS", "Retur Pembelian", trans:=transaction)

                ' 2. Panggil fungsi pusat (logika reversal stok, hutang, jurnal, saldo 100% akurat)
                ModuleHapusTransaksi.HapusReturPembelian(TxtFakturTransaksi.Text, TxtLokasiUntukEdit.Text, transaction)

                transaction.Commit()
                ' Refresdatagridview() dipanggil otomatis oleh BtnHapus_Click
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan saat menghapus retur: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Hapusreturpenjualan()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' 1. Catat Audit Trail
                ModuleAuditTrail.CatatAudit(TxtFakturTransaksi.Text, "HAPUS", "Retur Penjualan", trans:=transaction)

                ' 2. Panggil fungsi pusat (logika reversal stok, piutang, jurnal, saldo 100% akurat)
                ModuleHapusTransaksi.HapusReturPenjualan(TxtFakturTransaksi.Text, TxtLokasiUntukEdit.Text, transaction)

                transaction.Commit()
                ' Refresdatagridview() dipanggil otomatis oleh BtnHapus_Click
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan saat menghapus retur jual: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub


    Private Sub Hapusbayarhutang()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                For Each row As DataGridViewRow In DGVDetail.Rows
                    Using cmdUpdateBeli As New MySqlCommand("UPDATE pembelian SET PEMBAYARAN = PEMBAYARAN - @PEMBAYARAN, TAGIHAN = TAGIHAN + @TAGIHAN, TGL_BAYAR = NULL, NOMINALBAYAR = NOMINALBAYAR - @NOMINALBAYAR, STATUS_TRANSAKSI_BELI = 'Belum Lunas' WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn, transaction)

                        ' Menggunakan default 0 jika nilai tidak valid
                        Dim nominalBayar As Decimal = If(IsDBNull(row.Cells("PEMBAYARAN").Value) OrElse row.Cells("PEMBAYARAN").Value Is Nothing, 0D, Convert.ToDecimal(row.Cells("PEMBAYARAN").Value))

                        cmdUpdateBeli.Parameters.AddWithValue("@PEMBAYARAN", nominalBayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@TAGIHAN", nominalBayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@NOMINALBAYAR", nominalBayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@ID_PEMBELIAN", row.Cells("ID_BELI").Value)

                        ' Eksekusi perintah
                        cmdUpdateBeli.ExecuteNonQuery()
                    End Using
                Next

                ' ========================================
                ' START: Audit Trail - Hapus Bayar Hutang
                ' ========================================
                ModuleAuditTrail.CatatAudit(TxtFakturTransaksi.Text, "HAPUS", "Bayar Hutang", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Hapus Bayar Hutang
                ' ========================================

                ' SEBELUM menghapus JurnalUmum: SIMPAN daftar akun terlibat terlebih dahulu!
                Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using cmdAkun As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, transaction)
                    cmdAkun.Parameters.AddWithValue("@fk", TxtFakturTransaksi.Text)
                    Using rd = cmdAkun.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibat.Add(kode)
                        End While
                    End Using
                End Using

                Dim deleteQueries As String() = {
                    "DELETE FROM hutang WHERE NOBAYARHUTANG = @NO_TRANSAKSI",
                    "DELETE FROM Hutang_Detail WHERE ID_BAYAR = @NO_TRANSAKSI",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NO_TRANSAKSI"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Panggil UpdateSaldoAkun per akun yang disimpan SEBELUM delete JurnalUmum
                For Each kodeAkun As String In akunTerlibat
                    UpdateSaldoAkun(kodeAkun, transaction)
                Next
                transaction.Commit()

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Private Sub HapusbayarPiutang()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try 'SELECT ID_JUAL, KODE, NAMA, DIBAYAR, PIUTANG, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Piutang_Detail
                For Each row As DataGridViewRow In DGVDetail.Rows

                    Using cmdUpdatePembelian As New MySqlCommand("UPDATE penjualan SET BAYAR = BAYAR - @BAYAR, SISA_TAGIHAN = SISA_TAGIHAN + @SISA_TAGIHAN, TGL_PEMBAYARAN = NULL, NOMINALBAYARPIUTANG = NOMINALBAYARPIUTANG - @NOMINALBAYARPIUTANG, STATUS_TRANSAKSI = 'Belum Lunas' WHERE ID_PENJUALAN = @ID_PENJUALAN", conn, transaction)

                        ' Menggunakan variabel untuk nilai BAYAR
                        Dim bayar As Decimal = If(IsDBNull(row.Cells(6).Value) OrElse row.Cells(6).Value Is Nothing, 0D, CDec(row.Cells(6).Value))

                        ' Menambahkan parameter dengan nilai yang sudah dicek
                        cmdUpdatePembelian.Parameters.AddWithValue("@BAYAR", bayar)
                        cmdUpdatePembelian.Parameters.AddWithValue("@SISA_TAGIHAN", bayar) ' Menggunakan nilai yang sama untuk SISA_TAGIHAN
                        cmdUpdatePembelian.Parameters.AddWithValue("@NOMINALBAYARPIUTANG", bayar) ' Menggunakan nilai yang sama untuk NOMINALBAYARPIUTANG
                        cmdUpdatePembelian.Parameters.AddWithValue("@ID_PENJUALAN", row.Cells(0).Value)

                        ' Eksekusi perintah
                        cmdUpdatePembelian.ExecuteNonQuery()
                    End Using

                Next

                ' ========================================
                ' START: Audit Trail - Hapus Bayar Piutang
                ' ========================================
                ModuleAuditTrail.CatatAudit(TxtFakturTransaksi.Text, "HAPUS", "Bayar Piutang", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Hapus Bayar Piutang
                ' ========================================

                ' SEBELUM menghapus JurnalUmum: SIMPAN daftar akun terlibat terlebih dahulu!
                Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using cmdAkun As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, transaction)
                    cmdAkun.Parameters.AddWithValue("@fk", TxtFakturTransaksi.Text)
                    Using rd = cmdAkun.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibat.Add(kode)
                        End While
                    End Using
                End Using

                Dim deleteQueries As String() = {
                    "DELETE FROM Piutang WHERE ID_BAYAR_PIUTANG = @NO_TRANSAKSI",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NO_TRANSAKSI",
                    "DELETE FROM Piutang_Detail WHERE ID_BAYAR = @NO_TRANSAKSI"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Panggil UpdateSaldoAkun per akun yang disimpan SEBELUM delete JurnalUmum
                For Each kodeAkun As String In akunTerlibat
                    UpdateSaldoAkun(kodeAkun, transaction)
                Next
                transaction.Commit()

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Public Sub Hapusstokopname()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = Nothing

            Try
                ' Mulai transaksi
                transaction = conn.BeginTransaction()

                Dim updateQuery As String = ""
                Dim stokField As String = ""

                Select Case TxtLokasiUntukEdit.Text
                    Case "TOKO"
                        stokField = "OPNAME_TOKO"
                    Case "GUDANG"
                        stokField = "OPNAME_GUDANG"
                End Select

                updateQuery = "UPDATE tbl_barang SET " & stokField & " = " & stokField & " - ? WHERE ID_BARANG = ?"

                Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                    cmd.Parameters.AddWithValue("@STOK_OPNAME", CDec(DGVTransaksi.CurrentRow.Cells(8).Value))
                    cmd.Parameters.AddWithValue("@ID_BARANG", DGVTransaksi.CurrentRow.Cells(2).Value.ToString())
                    cmd.ExecuteNonQuery()
                End Using

                ' ========================================
                ' START: Audit Trail - Hapus Stok Opname
                ' ========================================
                Dim idStokOpname As String = TxtFakturTransaksi.Text
                Dim idBarang As String = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                Dim namaBarang As String = If(DGVTransaksi.CurrentRow.Cells(3).Value IsNot Nothing, DGVTransaksi.CurrentRow.Cells(3).Value.ToString(), "")
                Dim stokOpname As Decimal = CDec(DGVTransaksi.CurrentRow.Cells(8).Value)
                Dim lokasi As String = TxtLokasiUntukEdit.Text

                Dim snapshot As New System.Text.StringBuilder()
                snapshot.AppendLine(idStokOpname & " | " & DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
                snapshot.AppendLine("Barang: " & idBarang & " — " & namaBarang)
                snapshot.AppendLine("Lokasi: " & lokasi & " | Qty: " & stokOpname.ToString("N0"))

                ModuleAuditTrail.CatatAuditMaster(
                    "OPN:" & idStokOpname,
                    "HAPUS",
                    "Stok Opname",
                    snapshot.ToString(),
                    "Hapus stok opname",
                    transaction
                )
                ' ========================================
                ' END: Audit Trail - Hapus Stok Opname
                ' ========================================

                ' SEBELUM menghapus JurnalUmum: SIMPAN daftar akun terlibat terlebih dahulu!
                Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using cmdAkun As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, transaction)
                    cmdAkun.Parameters.AddWithValue("@fk", TxtFakturTransaksi.Text)
                    Using rd = cmdAkun.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibat.Add(kode)
                        End While
                    End Using
                End Using

                Dim deleteQueries As String() = {
                          "DELETE FROM Stok_Opname WHERE ID_STOK_OPNAME = @ID_STOK_OPNAME",
                          "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_STOK_OPNAME",
                          "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_STOK_OPNAME"
                      }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_STOK_OPNAME", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Recalculate stok + audit delta (hapus stok opname)
                Dim kodeOpname As String = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                Dim sebelumOpname As Decimal = BacaStokSaatIni(kodeOpname, TxtLokasiUntukEdit.Text, transaction)
                HitungStokPerubahan(kodeOpname, transaction)
                Dim sesudahOpname As Decimal = BacaStokSaatIni(kodeOpname, TxtLokasiUntukEdit.Text, transaction)
                Dim auditHapusOpname As New Dictionary(Of String, Decimal)() From {{kodeOpname, Math.Abs(sesudahOpname - sebelumOpname)}}
                AuditStokTransaksi(TxtFakturTransaksi.Text, "Hapus Stok Opname", Nothing, Nothing, Nothing, auditHapusOpname, transaction)

                ' Panggil UpdateSaldoAkun per akun yang disimpan SEBELUM delete JurnalUmum
                For Each kodeAkun As String In akunTerlibat
                    UpdateSaldoAkun(kodeAkun, transaction)
                Next
                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                                "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Hapustransferstok()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            Dim idBarangMasuk As String = DGVTransaksi.CurrentRow.Cells(4).Value.ToString()
            Dim idBarangKeluar As String = DGVTransaksi.CurrentRow.Cells(12).Value.ToString()

            ' Mulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim qtySatMasuk As Decimal = If(String.IsNullOrEmpty(DGVTransaksi.CurrentRow.Cells(9).Value.ToString()), 0, Decimal.Parse(DGVTransaksi.CurrentRow.Cells(9).Value.ToString()))
                Dim qtySatKeluar As Decimal = If(String.IsNullOrEmpty(DGVTransaksi.CurrentRow.Cells(17).Value.ToString()), 0, Decimal.Parse(DGVTransaksi.CurrentRow.Cells(17).Value.ToString()))

                Dim queryUpdateStokMasuk As String = String.Empty
                Dim queryUpdateStokKeluar As String = String.Empty

                ' Tentukan query berdasarkan lokasi
                Select Case TxtLokasiUntukEdit.Text
                    Case "GUDANG"
                        queryUpdateStokMasuk = "UPDATE tbl_barang SET TRANSFER_STOK_MASUK_GUDANG = TRANSFER_STOK_MASUK_GUDANG - ? WHERE ID_BARANG = ?"
                        queryUpdateStokKeluar = "UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_GUDANG = TRANSFER_STOK_KELUAR_GUDANG - ? WHERE ID_BARANG = ?"
                    Case "TOKO"
                        queryUpdateStokMasuk = "UPDATE tbl_barang SET TRANSFER_STOK_MASUK_TOKO = TRANSFER_STOK_MASUK_TOKO - ? WHERE ID_BARANG = ?"
                        queryUpdateStokKeluar = "UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_TOKO = TRANSFER_STOK_KELUAR_TOKO - ? WHERE ID_BARANG = ?"
                End Select

                ' Update stok masuk
                Using cmdUpdateStok As New MySqlCommand(queryUpdateStokMasuk, conn, transaction)
                    cmdUpdateStok.Parameters.AddWithValue("@QtySat", qtySatMasuk)
                    cmdUpdateStok.Parameters.AddWithValue("@ID_BARANG", idBarangMasuk)
                    cmdUpdateStok.ExecuteNonQuery()
                End Using

                ' Update stok keluar
                Using cmdUpdateKeluar As New MySqlCommand(queryUpdateStokKeluar, conn, transaction)
                    cmdUpdateKeluar.Parameters.AddWithValue("@QtySat", qtySatKeluar)
                    cmdUpdateKeluar.Parameters.AddWithValue("@ID_BARANG", idBarangKeluar)
                    cmdUpdateKeluar.ExecuteNonQuery()
                End Using

                ' ========================================
                ' START: Audit Trail - Hapus Transfer Stok
                ' ========================================
                Dim idTransfer As String = TxtFakturTransaksi.Text
                Dim namaBarangMasuk As String = If(DGVTransaksi.CurrentRow.Cells(5).Value IsNot Nothing, DGVTransaksi.CurrentRow.Cells(5).Value.ToString(), "")
                Dim namaBarangKeluar As String = If(DGVTransaksi.CurrentRow.Cells(13).Value IsNot Nothing, DGVTransaksi.CurrentRow.Cells(13).Value.ToString(), "")
                Dim lokasi As String = TxtLokasiUntukEdit.Text

                Dim snapshot As New System.Text.StringBuilder()
                snapshot.AppendLine(idTransfer & " | " & DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
                snapshot.AppendLine("Barang Masuk: " & idBarangMasuk & " — " & namaBarangMasuk & " | Qty: " & qtySatMasuk.ToString("N0"))
                snapshot.AppendLine("Barang Keluar: " & idBarangKeluar & " — " & namaBarangKeluar & " | Qty: " & qtySatKeluar.ToString("N0"))
                snapshot.AppendLine("Lokasi: " & lokasi)

                ModuleAuditTrail.CatatAuditMaster(
                    "TRF-STK:" & idTransfer,
                    "HAPUS",
                    "Transfer Stok",
                    snapshot.ToString(),
                    "Hapus transfer stok",
                    transaction
                )
                ' ========================================
                ' END: Audit Trail - Hapus Transfer Stok
                ' ========================================

                ' SEBELUM menghapus JurnalUmum: SIMPAN daftar akun terlibat terlebih dahulu!
                Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using cmdAkun As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, transaction)
                    cmdAkun.Parameters.AddWithValue("@fk", TxtFakturTransaksi.Text)
                    Using rd = cmdAkun.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibat.Add(kode)
                        End While
                    End Using
                End Using

                Dim deleteQueries As String() = {
                  "DELETE FROM Transfer_stok WHERE ID_TRANSFER = @ID_TRANSFER",
                  "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_TRANSFER",
                  "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_TRANSFER"
              }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Recalculate stok + audit delta (hapus transfer stok)
                Dim sebelumMsk As Decimal = BacaStokSaatIni(idBarangMasuk, TxtLokasiUntukEdit.Text, transaction)
                HitungStokPerubahan(idBarangMasuk, transaction)
                Dim sesudahMsk As Decimal = BacaStokSaatIni(idBarangMasuk, TxtLokasiUntukEdit.Text, transaction)

                Dim sebelumKlr As Decimal = BacaStokSaatIni(idBarangKeluar, TxtLokasiUntukEdit.Text, transaction)
                HitungStokPerubahan(idBarangKeluar, transaction)
                Dim sesudahKlr As Decimal = BacaStokSaatIni(idBarangKeluar, TxtLokasiUntukEdit.Text, transaction)

                Dim auditHapusTS As New Dictionary(Of String, Decimal)() From {
                    {idBarangMasuk, sebelumMsk - sesudahMsk},
                    {idBarangKeluar, sesudahKlr - sebelumKlr}
                }
                AuditStokTransaksi(TxtFakturTransaksi.Text, "Hapus Transfer Stok", Nothing, Nothing, Nothing, auditHapusTS, transaction)

                ' Panggil UpdateSaldoAkun per akun yang disimpan SEBELUM delete JurnalUmum
                For Each kodeAkun As String In akunTerlibat
                    UpdateSaldoAkun(kodeAkun, transaction)
                Next
                transaction.Commit()

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try


        End If
    End Sub

    Private Sub HapusSuratJalan()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            ' Mulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try

                ' ========================================
                ' START: Audit Trail - Hapus Surat Jalan
                ' ========================================
                Dim notaSJ As String = TxtFakturTransaksi.Text
                Dim snapshot As New System.Text.StringBuilder()
                snapshot.AppendLine(notaSJ & " | " & DateTime.Now.ToString("yyyy-MM-dd HH:mm"))

                If DGVTransaksi.CurrentRow IsNot Nothing Then
                    Dim colIdx As Integer = 0
                    For Each col As DataGridViewColumn In DGVTransaksi.Columns
                        If col.Visible AndAlso DGVTransaksi.CurrentRow.Cells(colIdx).Value IsNot Nothing Then
                            snapshot.AppendLine(col.HeaderText & ": " & DGVTransaksi.CurrentRow.Cells(colIdx).Value.ToString())
                        End If
                        colIdx += 1
                    Next
                End If

                ModuleAuditTrail.CatatAuditMaster(
                    "SJ:" & notaSJ,
                    "HAPUS",
                    "Surat Jalan",
                    snapshot.ToString(),
                    "Hapus surat jalan",
                    transaction
                )
                ' ========================================
                ' END: Audit Trail - Hapus Surat Jalan
                ' ========================================

                Dim deleteQueries As String() = {
                  "DELETE FROM Surat_Jalan WHERE NOTA = @NOTA",
                  "DELETE FROM Surat_Jalan_Detail WHERE NOTA = @NOTA"
              }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@NOTA", notaSJ)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                                 "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

    Private Sub HapusTransferBarang()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            ' Mulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim stokKeluarField As String
                Dim stokMasukField As String

                Select Case TxtLokasiUntukEdit.Text
                    Case "TOKO"
                        stokKeluarField = "TRANSFER_BARANG_KELUAR_TOKO"
                        stokMasukField = "TRANSFER_BARANG_MASUK_GUDANG"
                    Case "GUDANG"
                        stokKeluarField = "TRANSFER_BARANG_KELUAR_GUDANG"
                        stokMasukField = "TRANSFER_BARANG_MASUK_TOKO"
                    Case Else
                        Throw New Exception("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & stokKeluarField & " = " & stokKeluarField & " - ?, " & stokMasukField & " = " & stokMasukField & " - ? WHERE ID_BARANG = ?"

                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                        If Not String.IsNullOrEmpty(kodeBarang) Then
                            Dim qtySat As Decimal = If(row.Cells("TOTAL_QTY").Value IsNot Nothing, Convert.ToDecimal(row.Cells("TOTAL_QTY").Value), 0D)

                            Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                                cmd.Parameters.AddWithValue("@QtySatKeluar", qtySat)
                                cmd.Parameters.AddWithValue("@QtySatMasuk", qtySat)
                                cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                Next

                ' ========================================
                ' START: Audit Trail - Hapus Transfer Barang
                ' ========================================
                Dim idTransferBarang As String = TxtFakturTransaksi.Text
                Dim lokasi As String = TxtLokasiUntukEdit.Text

                Dim snapshot As New System.Text.StringBuilder()
                snapshot.AppendLine(idTransferBarang & " | " & DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
                snapshot.AppendLine("Lokasi: " & lokasi)
                snapshot.AppendLine("Daftar Barang:")

                Dim noItem As Integer = 0
                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        noItem += 1
                        Dim kode As String = row.Cells("ID_BARANG").Value.ToString()
                        Dim nama As String = If(row.Cells("NAMA_BARANG").Value IsNot Nothing, row.Cells("NAMA_BARANG").Value.ToString(), "")
                        Dim qty As Decimal = If(row.Cells("TOTAL_QTY").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("TOTAL_QTY").Value), CDec(row.Cells("TOTAL_QTY").Value), 0D)
                        snapshot.AppendLine($"  {noItem}. {kode} — {nama} | Qty: {qty:N0}")
                    End If
                Next

                ModuleAuditTrail.CatatAuditMaster(
                    "TRF:" & idTransferBarang,
                    "HAPUS",
                    "Transfer Barang",
                    snapshot.ToString(),
                    "Hapus transfer barang",
                    transaction
                )
                ' ========================================
                ' END: Audit Trail - Hapus Transfer Barang
                ' ========================================

                ' SEBELUM menghapus JurnalUmum: SIMPAN daftar akun terlibat terlebih dahulu!
                Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using cmdAkun As New MySqlCommand(
                    "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                    "UNION " &
                    "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                    conn, transaction)
                    cmdAkun.Parameters.AddWithValue("@fk", TxtFakturTransaksi.Text)
                    Using rd = cmdAkun.ExecuteReader()
                        While rd.Read()
                            Dim kode As String = rd(0).ToString().Trim()
                            If kode <> "" Then akunTerlibat.Add(kode)
                        End While
                    End Using
                End Using

                Dim deleteQueries As String() = {
                    "DELETE FROM Transfer_Barang WHERE ID_TRANSFER = @ID_TRANSFER",
                    "DELETE FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_TRANSFER",
                    "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_TRANSFER"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Recalculate stok + audit delta (hapus transfer barang)
                ' Audit A: qty dari DGVDetail (kolom TOTAL_QTY dari Transfer_Barang_Detail)
                Dim auditHapusTB As New Dictionary(Of String, Decimal)()
                Dim auditDGVHapusTB As New Dictionary(Of String, Decimal)()
                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        Dim kode As String = row.Cells("ID_BARANG").Value.ToString()
                        Dim qtyA As Decimal = If(row.Cells("TOTAL_QTY").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("TOTAL_QTY").Value), CDec(row.Cells("TOTAL_QTY").Value), 0D)
                        If auditDGVHapusTB.ContainsKey(kode) Then auditDGVHapusTB(kode) += qtyA Else auditDGVHapusTB(kode) = qtyA
                        Dim sebelum As Decimal = BacaStokSaatIni(kode, TxtLokasiUntukEdit.Text, transaction)
                        HitungStokPerubahan(kode, transaction)
                        Dim sesudah As Decimal = BacaStokSaatIni(kode, TxtLokasiUntukEdit.Text, transaction)
                        auditHapusTB(kode) = sesudah - sebelum  ' hapus transfer barang mengembalikan stok asal
                    End If
                Next
                AuditStokTransaksi(TxtFakturTransaksi.Text, "Hapus Transfer Barang", auditDGVHapusTB, Nothing, Nothing, auditHapusTB, transaction)

                ' Panggil UpdateSaldoAkun per akun yang disimpan SEBELUM delete JurnalUmum
                For Each kodeAkun As String In akunTerlibat
                    UpdateSaldoAkun(kodeAkun, transaction)
                Next
                transaction.Commit()

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Private Sub HapusTransferCabang()
        If MessageBox.Show("Apakah data transfer cabang ini akan dihapus?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Dim faktur As String = TxtFakturTransaksi.Text
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' 1. Baca lokasi asal dari HistoryBarang (LOKASI = LokasiBarang saat transaksi dibuat)
            Dim lokasiAsal As String = "TOKO"
            Using cmdLok As New MySqlCommand(
                "SELECT LOKASI FROM HistoryBarang WHERE FAKTUR = @id AND JENIS = 'TRANSFER_CABANG_KELUAR' LIMIT 1", conn, transaction)
                cmdLok.Parameters.AddWithValue("@id", faktur)
                Dim val = cmdLok.ExecuteScalar()
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    If val.ToString().ToUpper() = "GUDANG" Then lokasiAsal = "GUDANG"
                End If
            End Using
            Dim kolomKeluar As String = If(lokasiAsal = "GUDANG",
                "TRANSFER_CABANG_KELUAR_GUDANG", "TRANSFER_CABANG_KELUAR_TOKO")

            ' 2. Ambil item dari DGVDetail yang sudah terisi saat row diklik
            Dim kodeItems As New List(Of String)()
            For Each row As DataGridViewRow In DGVDetail.Rows
                If row.IsNewRow OrElse row.Cells("ID_BARANG").Value Is Nothing Then Continue For
                Dim kode As String = row.Cells("ID_BARANG").Value.ToString()
                Dim qtySat As Decimal = CDec(If(row.Cells("TOTAL_QTY").Value, 0))
                If String.IsNullOrEmpty(kode) Then Continue For

                Using cmdStok As New MySqlCommand(
                    $"UPDATE tbl_barang SET {kolomKeluar} = {kolomKeluar} - @qty WHERE ID_BARANG = @kode",
                    conn, transaction)
                    cmdStok.Parameters.AddWithValue("@qty", qtySat)
                    cmdStok.Parameters.AddWithValue("@kode", kode)
                    cmdStok.ExecuteNonQuery()
                End Using
                kodeItems.Add(kode)
            Next

            ' ========================================
            ' START: Audit Trail - Hapus Transfer Cabang
            ' ========================================
            Dim snapshot As New System.Text.StringBuilder()
            snapshot.AppendLine(faktur & " | " & DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
            snapshot.AppendLine("Lokasi Asal: " & lokasiAsal)
            snapshot.AppendLine("Daftar Barang:")

            Dim noItemCabang As Integer = 0
            For Each row As DataGridViewRow In DGVDetail.Rows
                If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                    noItemCabang += 1
                    Dim kode As String = row.Cells("ID_BARANG").Value.ToString()
                    Dim nama As String = If(row.Cells("NAMA_BARANG").Value IsNot Nothing, row.Cells("NAMA_BARANG").Value.ToString(), "")
                    Dim qty As Decimal = CDec(If(row.Cells("TOTAL_QTY").Value, 0))
                    snapshot.AppendLine($"  {noItemCabang}. {kode} — {nama} | Qty: {qty:N0}")
                End If
            Next

            ModuleAuditTrail.CatatAuditMaster(
                "TRF-CAB:" & faktur,
                "HAPUS",
                "Transfer Cabang",
                snapshot.ToString(),
                "Hapus transfer cabang",
                transaction
            )
            ' ========================================
            ' END: Audit Trail - Hapus Transfer Cabang
            ' ========================================

            ' 2. SEBELUM menghapus JurnalUmum: SIMPAN daftar akun terlibat terlebih dahulu!
            Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmdAkun As New MySqlCommand(
                "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                "UNION " &
                "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                conn, transaction)
                cmdAkun.Parameters.AddWithValue("@fk", faktur)
                Using rd = cmdAkun.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd(0).ToString().Trim()
                        If kode <> "" Then akunTerlibat.Add(kode)
                    End While
                End Using
            End Using

            ' 3. Hapus HistoryBarang, JurnalUmum, detail, header
            For Each q As String In {
                "DELETE FROM HistoryBarang WHERE FAKTUR = @id",
                "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @id",
                "DELETE FROM transfer_cabang_detail WHERE ID_TRANSFER = @id",
                "DELETE FROM transfer_cabang WHERE ID_TRANSFER = @id"
            }
                Using cmd As New MySqlCommand(q, conn, transaction)
                    cmd.Parameters.AddWithValue("@id", faktur)
                    cmd.ExecuteNonQuery()
                End Using
            Next

            ' 4. Recalculate STOK dari semua kolom mutasi (setelah HistoryBarang dihapus)
            For Each kode As String In kodeItems
                HitungStokPerubahan(kode, transaction)
            Next

            ' 5. Update saldo akun jurnal (hanya akun yang terlibat)
            For Each kodeAkun As String In akunTerlibat
                UpdateSaldoAkun(kodeAkun, transaction)
            Next

            transaction.Commit()
            MessageBox.Show("Data transfer cabang berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            DataTransferCabang()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Hapus gagal: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPrint.Click
        If TxtFakturTransaksi.Text <> "" Then
            ' Panggil metode Cetaktransaksi
            Cetaktransaksi()
        Else
            ' Tampilkan pesan jika TxtFakturTransaksi kosong
            MessageBox.Show("Tidak ada transaksi yang dipilih untuk dicetak.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub EksekusiCetakJual(noFaktur As String)
        If BacaPengaturanPrinter("Jual", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterJual.TanyaPilihPrinter(noFaktur)
        Else
            ModulePrinterJual.CetakPenjualan(noFaktur)
        End If
    End Sub

    Private Sub Cetaktransaksi()
        Dim faktur As String = TxtFakturTransaksi.Text
        If String.IsNullOrEmpty(faktur) Then Return

        Select Case TxtTransaksi.Text
            Case "Pembelian"
                Select Case BacaPengaturanPrinter("Beli", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("Beli", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota pembelian?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("Beli", faktur)
                        End If
                End Select

            Case "Penjualan"
                Select Case BacaPengaturanPrinter("Jual", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        EksekusiCetakJual(faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota penjualan?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            EksekusiCetakJual(faktur)
                        End If
                    Case "TAMPILKAN DI MONITOR"
                        ModulePrinterJual.PreviewPenjualan(faktur)
                End Select

            Case "Retur Pembelian"
                Select Case BacaPengaturanPrinter("ReturBeli", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("ReturBeli", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota retur pembelian?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("ReturBeli", faktur)
                        End If
                End Select

            Case "Retur Penjualan"
                Select Case BacaPengaturanPrinter("ReturJual", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("ReturJual", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota retur penjualan?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("ReturJual", faktur)
                        End If
                    Case "TAMPILKAN DI MONITOR"
                        ModulePrinterReturJual.PreviewReturJual(faktur)
                End Select

            Case "Bayar Hutang"
                Select Case BacaPengaturanPrinter("BayarHutang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("BayarHutang", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak bukti bayar hutang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("BayarHutang", faktur)
                        End If
                End Select

            Case "Bayar Piutang"
                Select Case BacaPengaturanPrinter("BayarPiutang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("BayarPiutang", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak bukti bayar piutang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("BayarPiutang", faktur)
                        End If
                End Select

            Case "Surat Jalan"
                Select Case BacaPengaturanPrinter("SuratJalan", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("SuratJalan", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak surat jalan?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("SuratJalan", faktur)
                        End If
                End Select

            Case "Transfer Barang"
                Select Case BacaPengaturanPrinter("TransferBarang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("TransferBarang", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota transfer barang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("TransferBarang", faktur)
                        End If
                End Select

            Case "Transfer Cabang"
                Select Case BacaPengaturanPrinter("TransferCabang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakUlang("TransferCabang", faktur)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak nota transfer cabang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakUlang("TransferCabang", faktur)
                        End If
                End Select

            Case "Stok Opname", "Transfer Stok"
                MessageBox.Show("Cetak ulang tidak tersedia untuk transaksi ini.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Select
    End Sub

    ' Cetak ulang nota — pakai ModulePrinter sesuai transaksi, respek PilihPrinter
    Private Sub LakukanCetakUlang(transaksi As String, faktur As String)
        Dim tanya As Boolean = BacaPengaturanPrinter(transaksi, "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER"
        Select Case transaksi
            Case "Beli"
                If tanya Then ModulePrinterBeli.TanyaPilihPrinterBeli(faktur) Else ModulePrinterBeli.CetakPembelian(faktur)
            Case "ReturBeli"
                If tanya Then ModulePrinterReturBeli.TanyaPilihPrinterReturBeli(faktur) Else ModulePrinterReturBeli.CetakReturBeli(faktur)
            Case "ReturJual"
                If tanya Then ModulePrinterReturJual.TanyaPilihPrinterReturJual(faktur) Else ModulePrinterReturJual.CetakReturJual(faktur)
            Case "BayarHutang"
                If tanya Then ModulePrinterBayarHutang.TanyaPilihPrinterBayarHutang(faktur) Else ModulePrinterBayarHutang.CetakBayarHutang(faktur)
            Case "BayarPiutang"
                If tanya Then ModulePrinterBayarPiutang.TanyaPilihPrinterBayarPiutang(faktur) Else ModulePrinterBayarPiutang.CetakBayarPiutang(faktur)
            Case "SuratJalan"
                If tanya Then ModulePrinterSuratJalan.TanyaPilihPrinterSuratJalan(faktur) Else ModulePrinterSuratJalan.CetakSuratJalan(faktur)
            Case "TransferBarang"
                If tanya Then ModulePrinterTransferBarang.TanyaPilihPrinterTransferBarang(faktur) Else ModulePrinterTransferBarang.CetakTransferBarang(faktur)
            Case "TransferCabang"
                If tanya Then ModulePrinterTransferCabang.TanyaPilihPrinterTransferCabang(faktur) Else ModulePrinterTransferCabang.CetakTransferCabang(faktur)
        End Select
    End Sub

    Private Sub DGVTransaksi_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVTransaksi.CellClick
        If DGVTransaksi.Rows.Count < 1 Then
            MessageBox.Show("Tidak ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else

            Select Case TxtTransaksi.Text
                Case "Pembelian"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL FROM pembelian_detail WHERE FAKTUR_BELI = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@FAKTUR_BELI", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "pembelian_detail")

                                DGVDetail.DataSource = ds.Tables("pembelian_detail")

                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("HARGA_BELI").HeaderText = "HARGA"
                                    .Columns("HARGA_AVERAGE").Visible = False
                                    .Columns("HARGA_BELI_SEBELUMNYA").Visible = False
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("HARGA_BELI_SATUAN").Visible = False
                                    .Columns("QTY_SAT").Visible = False
                                    .Columns("TOTAL").HeaderText = "TOTAL"

                                    AturKolomAngka(DGVDetail, "HARGA_BELI")

                                    AturKolomAngka(DGVDetail, "HARGA_BELI_SATUAN")

                                    AturKolomAngka(DGVDetail, "TOTAL")

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("HARGA_BELI").FillWeight = 60
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("HARGA_BELI_SATUAN").FillWeight = 70
                                    .Columns("TOTAL").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using

                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Belanja : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Penjualan"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@FAKTUR_JUAL", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "penjualan_detail")
                                DGVDetail.DataSource = ds.Tables("penjualan_detail")
                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("HARGA_JUAL").HeaderText = "HARGA"
                                    .Columns("QTY_SATUAN").Visible = False
                                    .Columns("TOTAL_DISKON").HeaderText = "DISKON"
                                    .Columns("TOTAL_HARGA").HeaderText = "TOTAL"

                                    AturKolomAngka(DGVDetail, "QTY")

                                    AturKolomAngka(DGVDetail, "HARGA_JUAL")

                                    AturKolomAngka(DGVDetail, "TOTAL_DISKON")

                                    AturKolomAngka(DGVDetail, "TOTAL_HARGA")

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("HARGA_JUAL").FillWeight = 60
                                    .Columns("TOTAL_DISKON").FillWeight = 60
                                    .Columns("TOTAL_HARGA").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Penjualan : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                Case "Retur Pembelian"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, QTY_SAT, TOTAL FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "penjualan_detail")
                                DGVDetail.DataSource = ds.Tables("penjualan_detail")
                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("TOTAL").HeaderText = "TOTAL"

                                    AturKolomAngka(DGVDetail, "QTY")

                                    .Columns("QTY_SAT").Visible = False

                                    AturKolomAngka(DGVDetail, "TOTAL")

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("TOTAL").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(4).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Retur Pembelian : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Retur Penjualan"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "retur_penjualan_detail")
                                DGVDetail.DataSource = ds.Tables("retur_penjualan_detail")
                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("HARGA_JUAL").HeaderText = "HARGA"
                                    .Columns("QTY_SATUAN").Visible = False
                                    .Columns("TOTAL_DISKON").HeaderText = "DISKON"
                                    .Columns("TOTAL_HARGA").HeaderText = "TOTAL"

                                    AturKolomAngka(DGVDetail, "QTY")

                                    AturKolomAngka(DGVDetail, "HARGA_JUAL")

                                    AturKolomAngka(DGVDetail, "TOTAL_DISKON")

                                    AturKolomAngka(DGVDetail, "TOTAL_HARGA")

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("HARGA_JUAL").FillWeight = 60
                                    .Columns("TOTAL_DISKON").FillWeight = 60
                                    .Columns("TOTAL_HARGA").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(4).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Retur Penjualan : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Bayar Hutang"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    Dim sqlSelect As String = "SELECT ID_BELI, KODE, NAMA, TOTAL_HUTANG, DIBAYAR, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Hutang_Detail WHERE ID_BAYAR = @ID_BAYAR"

                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@ID_BAYAR", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "HutangDetail")
                                DGVDetail.DataSource = ds.Tables("HutangDetail")

                                With DGVDetail
                                    .Columns("ID_BELI").HeaderText = "NOTA BELI"
                                    .Columns("KODE").Visible = False
                                    .Columns("NAMA").HeaderText = "SUPLIYER"
                                    .Columns("DIBAYAR").Visible = False
                                    .Columns("TOTAL_HUTANG").Visible = False
                                    .Columns("TANGGAL_BAYAR").HeaderText = "TANGGAL"
                                    .Columns("PEMBAYARAN").HeaderText = "NOMINAL"
                                    .Columns("STATUS").HeaderText = "STATUS"

                                    AturKolomAngka(DGVDetail, "PEMBAYARAN")

                                    .Columns("NAMA").FillWeight = 150
                                End With
                            End Using
                        End Using
                    End Using

                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Bayar Hutang : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                Case "Bayar Piutang"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_JUAL, KODE, NAMA, DIBAYAR, PIUTANG, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Piutang_Detail WHERE ID_BAYAR = @ID_BAYAR"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        ' Set parameter ID_BAYAR dari baris yang dipilih di DGVTransaksi
                        cmdSelect.Parameters.AddWithValue("@ID_BAYAR", DGVTransaksi.CurrentRow.Cells(0).Value)

                        ' Menggunakan data adapter untuk mengambil data dari database
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                ' Mengisi dataset dengan data dari database
                                da.Fill(ds, "penjualan_piutang")
                                ' Menetapkan sumber data untuk DataGridView DGVDetail
                                DGVDetail.DataSource = ds.Tables("penjualan_piutang")

                                ' Konfigurasi tampilan kolom pada DataGridView
                                With DGVDetail
                                    .Columns("ID_JUAL").HeaderText = "NOTA JUAL"
                                    .Columns("KODE").Visible = False ' Kolom KODE mungkin tidak diperlukan
                                    .Columns("NAMA").HeaderText = "PELANGGAN"
                                    .Columns("DIBAYAR").Visible = False ' Kolom DIBAYAR mungkin tidak diperlukan
                                    .Columns("PIUTANG").Visible = False ' Kolom PIUTANG mungkin tidak diperlukan
                                    .Columns("TANGGAL_BAYAR").HeaderText = "TANGGAL"
                                    .Columns("PEMBAYARAN").HeaderText = "NOMINAL"
                                    .Columns("STATUS").HeaderText = "STATUS"

                                    ' Format dan pengaturan alignment untuk kolom NOMINAL
                                    AturKolomAngka(DGVDetail, "PEMBAYARAN")

                                    ' Mengatur ukuran kolom NAMA_PELANGGAN
                                    .Columns("NAMA").FillWeight = 150
                                End With
                            End Using
                        End Using
                    End Using

                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Bayar Piutang : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Stok Opname"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(1).Value.ToString()

                Case "Transfer Stok"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(1).Value.ToString()


                Case "Surat Jalan"
                    ' Clear existing data
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    ' SQL query to select the necessary fields
                    Dim sqlSelect As String = "SELECT NOTA_BELANJA, NAMA_PELANGGAN, NILAI_BELANJA, LOKASI FROM Surat_Jalan_Detail WHERE NOTA = ?"

                    ' Prepare the command
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        ' Add the parameter
                        cmdSelect.Parameters.AddWithValue("@NOTA", DGVTransaksi.CurrentRow.Cells(0).Value)

                        ' Use a DataAdapter to fill a DataSet
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                ' Fill the DataSet
                                da.Fill(ds, "Surat_Jalan_Detail")

                                ' Set the DataSource of the DataGridView
                                DGVDetail.DataSource = ds.Tables("Surat_Jalan_Detail")

                                ' Configure the DataGridView columns
                                With DGVDetail
                                    ' Ensure column names match those in the dataset
                                    .Columns("NOTA_BELANJA").Visible = False
                                    .Columns("NAMA_PELANGGAN").HeaderText = "NAMA PELANGGAN"
                                    .Columns("NILAI_BELANJA").HeaderText = "NILAI BELANJA"
                                    .Columns("LOKASI").HeaderText = "LOKASI"

                                    ' Align and format columns
                                    AturKolomAngka(DGVDetail, "NILAI_BELANJA")

                                    ' Set column widths
                                    .Columns("NAMA_PELANGGAN").FillWeight = 150
                                    .Columns("NILAI_BELANJA").FillWeight = 60
                                    .Columns("LOKASI").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using

                    ' Update related controls
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Clear()
                    LblDetailTransaksi.Text = "Detail Surat Jalan : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Transfer Barang"
                    ' Bersihkan data yang ada
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    ' Query SQL untuk memilih kolom yang diperlukan
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, TOTAL_QTY, TOTAL FROM Transfer_Barang_Detail WHERE ID_TRANSFER = ?"

                    ' Persiapkan perintah
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        ' Tambahkan parameter
                        cmdSelect.Parameters.AddWithValue("@ID_TRANSFER", DGVTransaksi.CurrentRow.Cells(0).Value)

                        ' Gunakan DataAdapter untuk mengisi DataSet
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                ' Isi DataSet
                                da.Fill(ds, "Transfer_Barang_Detail")

                                ' Atur DataSource dari DataGridView
                                DGVDetail.DataSource = ds.Tables("Transfer_Barang_Detail")

                                ' Konfigurasi kolom DataGridView
                                With DGVDetail
                                    ' Pastikan nama kolom cocok dengan dataset
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("TOTAL_QTY").Visible = False
                                    .Columns("TOTAL").HeaderText = "TOTAL"

                                    ' Ratakan dan format kolom
                                    AturKolomAngka(DGVDetail, "TOTAL")

                                    ' Set lebar kolom
                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 40
                                    .Columns("SATUAN").FillWeight = 60
                                    .Columns("TOTAL").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using

                    ' Perbarui kontrol terkait
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(1).Value.ToString()
                    LblDetailTransaksi.Text = "Detail transfer barang: " & DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                Case "Transfer Cabang"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    ' Baca lokasi asal dari HistoryBarang untuk TxtLokasiUntukEdit
                    Dim lokasiAsal As String = "TOKO"
                    Using cmdLokasi As New MySqlCommand(
                        "SELECT LOKASI FROM HistoryBarang WHERE FAKTUR = @id AND JENIS = 'TRANSFER_CABANG_KELUAR' LIMIT 1", conn)
                        cmdLokasi.Parameters.AddWithValue("@id", DGVTransaksi.CurrentRow.Cells(0).Value)
                        Dim lokVal = cmdLokasi.ExecuteScalar()
                        If lokVal IsNot Nothing AndAlso Not IsDBNull(lokVal) Then
                            If lokVal.ToString().ToUpper() = "GUDANG" Then lokasiAsal = "GUDANG"
                        End If
                    End Using

                    Using cmdSelect As New MySqlCommand(
                        "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL " &
                        "FROM transfer_cabang_detail WHERE ID_TRANSFER = @id ORDER BY NAMA_BARANG", conn)
                        cmdSelect.Parameters.AddWithValue("@id", DGVTransaksi.CurrentRow.Cells(0).Value)
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "transfer_cabang_detail")
                                DGVDetail.DataSource = ds.Tables("transfer_cabang_detail")
                            End Using
                        End Using
                    End Using
                    With DGVDetail
                        .Columns("ID_BARANG").Visible = False
                        .Columns("NAMA_BARANG").HeaderText = "Barang"
                        .Columns("QTY").HeaderText = "Qty"
                        .Columns("SATUAN").HeaderText = "Satuan"
                        .Columns("ISI_SATUAN").HeaderText = "Isi"
                        .Columns("TOTAL_QTY").HeaderText = "Total Qty"
                        .Columns("TOTAL").HeaderText = "Total Nilai"
                        Dim angka As New DataGridViewCellStyle With {
                            .Alignment = DataGridViewContentAlignment.MiddleRight,
                            .Format = "#,0.##"
                        }
                        .Columns("QTY").DefaultCellStyle = angka
                        .Columns("TOTAL_QTY").DefaultCellStyle = angka
                        .Columns("TOTAL").DefaultCellStyle = angka
                    End With
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = lokasiAsal
                    LblDetailTransaksi.Text = "Detail Transfer Cabang: " & DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

            End Select

            With DGVDetail
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToOrderColumns = False
                .AllowUserToResizeColumns = False
                .AllowUserToResizeRows = False
                .BorderStyle = BorderStyle.FixedSingle
            End With
            ModuleTheme.ApplyThemeDataGridView(DGVDetail)

        End If
    End Sub

    Private Sub BTNKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNKeluar.Click
        WbDashboard.Visible = True
        GBTransaksi.Hide()

    End Sub

    Public Sub Refresdatagridview()
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                Datapembelian()
            Case "Penjualan"
                Datapenjualan()
            Case "Retur Pembelian"
                DatareturPembelian()
            Case "Retur Penjualan"
                DataReturPenjualan()
            Case "Bayar Hutang"
                DataBayarHutang()
            Case "Bayar Piutang"
                DataBayarPiutang()
            Case "Stok Opname"
                DataStokOpname()
            Case "Transfer Stok"
                Datatransferstok()
            Case "Surat Jalan"
                DataSuratjalan()
            Case "Transfer Barang"
                DataTransferBarang()
            Case "Transfer Cabang"
                DataTransferCabang()
        End Select
    End Sub

    '----------------------------------------- KARYAWAN ---------------------------------------------------------------------------
    Private Sub MasterGajiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasterGajiToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormMasterGaji)
    End Sub

    Private Sub BonKaryawanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BonKaryawanToolStripMenuItem.Click
        TutupSemuaForm()
        With FormBon
            .LblUtama.Text = "BON KARYAWAN" : .LblJenis.Text = "BON"
            BukaFormMdi(My.Forms.FormBon)
        End With
    End Sub

    Private Sub BayarBonDiluarGajiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BayarBonDiluarGajiToolStripMenuItem.Click
        TutupSemuaForm()
        With FormBon
            .LblUtama.Text = "BAYAR BON KARYAWAN DILUAR POTONGAN GAJI" : .LblJenis.Text = "BAYAR"
            BukaFormMdi(My.Forms.FormBon)
        End With
    End Sub

    Private Sub LaporanBonToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaporanBonToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapBon)
    End Sub

    Private Sub LaporanBonPerKaryawanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaporanBonPerKaryawanToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapBonPerorang)
    End Sub

    Private Sub GajiKaryawanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GajiKaryawanToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormGaji)
    End Sub

    Private Sub LaporanGajiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaporanGajiToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLaporanGaji)
    End Sub
    '----------------------------------------- LAPORAN ---------------------------------------------------------------------------

    Private Sub MutasiSaldoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MutasiSaldoToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapMutasiKeuangan)
    End Sub

    Private Sub MutasiBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MutasiBarangToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapMutasiBarang)
    End Sub

    Private Sub JurnalUmumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JurnalUmumToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapJurnal)
    End Sub

    Private Sub NeracaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NeracaToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapNeracaLR)
    End Sub

    Private Sub BukuBesarToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BukuBesarToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapBB)
    End Sub

    Private Sub BukuBesarPembantuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BukuBesarPembantuToolStripMenuItem.Click
        TutupSemuaForm()
        My.Forms.FormLapBBPembantu.JenisLaporan = "Piutang"
        BukaFormMdi(My.Forms.FormLapBBPembantu)
    End Sub

    Private Sub LabaRugiBerjalanToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles LabaRugiBerjalanToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapLabaRugi)
    End Sub
    Private Sub PembelianToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapPembelian.LblHeaderForm.Text = "LAPORAN PEMBELIAN" : BukaFormMdi(My.Forms.FormLapPembelian)
    End Sub

    Private Sub PembelianDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianDetailToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPembelian.LblHeaderForm.Text = "LAPORAN PEMBELIAN DETAIL" : BukaFormMdi(My.Forms.FormLapPembelian)
    End Sub

    Private Sub PembelianBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianBarangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPembelian.LblHeaderForm.Text = "LAPORAN BARANG PEMBELIAN" : BukaFormMdi(My.Forms.FormLapPembelian)
    End Sub

    Private Sub PembelianDihutangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianDihutangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPembelian.LblHeaderForm.Text = "LAPORAN PEMBELIAN DIHUTANG" : BukaFormMdi(My.Forms.FormLapPembelian)
    End Sub

    Private Sub RekapPenjualanByNotaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RekapPenjualanByNotaToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPenjualanBaru.LblHeaderForm.Text = "LAPORAN REKAP PENJUALAN NOTA" : BukaFormMdi(My.Forms.FormLapPenjualanBaru)
    End Sub

    Private Sub RekapPenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RekapPenjualanToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPenjualanBaru.LblHeaderForm.Text = "LAPORAN REKAP PENJUALAN BARANG" : BukaFormMdi(My.Forms.FormLapPenjualanBaru)
    End Sub

    Private Sub PenjualanToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapPenjualanBaru.LblHeaderForm.Text = "LAPORAN PENJUALAN" : BukaFormMdi(My.Forms.FormLapPenjualanBaru)
    End Sub

    Private Sub PenjualanDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanDetailToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPenjualanBaru.LblHeaderForm.Text = "LAPORAN PENJUALAN DETAIL" : BukaFormMdi(My.Forms.FormLapPenjualanBaru)
    End Sub

    Private Sub PenjualanBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanBarangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPenjualanBaru.LblHeaderForm.Text = "LAPORAN BARANG PENJUALAN" : BukaFormMdi(My.Forms.FormLapPenjualanBaru)
    End Sub

    Private Sub PenjualanTerhutangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanTerhutangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPenjualanBaru.LblHeaderForm.Text = "LAPORAN PENJUALAN DIHUTANG" : BukaFormMdi(My.Forms.FormLapPenjualanBaru)
    End Sub

    Private Sub PenjualanSalesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanSalesToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapPenjualanSales)
    End Sub

    Private Sub PenjualanQtyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PenjualanQtyToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormRopertJual)
    End Sub

    Private Sub PenjualanPPNNonPPNToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PenjualanPPNNonPPNToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormPenjualanPPn)
    End Sub
    Private Sub ReturPembelianToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPembelianToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapReturBeli.LblHeaderForm.Text = "LAPORAN RETUR PEMBELIAN" : BukaFormMdi(My.Forms.FormLapReturBeli)
    End Sub

    Private Sub ReturPembelianDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPembelianDetailToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapReturBeli.LblHeaderForm.Text = "LAPORAN RETUR PEMBELIAN DETAIL" : BukaFormMdi(My.Forms.FormLapReturBeli)
    End Sub

    Private Sub ReturPembelianBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPembelianBarangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapReturBeli.LblHeaderForm.Text = "LAPORAN BARANG RETUR PEMBELIAN" : BukaFormMdi(My.Forms.FormLapReturBeli)
    End Sub

    Private Sub ReturPenjualanToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPenjualanToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapReturJual.LblHeaderForm.Text = "LAPORAN RETUR PENJUALAN" : BukaFormMdi(My.Forms.FormLapReturJual)
    End Sub

    Private Sub ReturPenjualanDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPenjualanDetailToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapReturJual.LblHeaderForm.Text = "LAPORAN RETUR PENJUALAN DETAIL" : BukaFormMdi(My.Forms.FormLapReturJual)
    End Sub

    Private Sub ReturPenjualanBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPenjualanBarangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapReturJual.LblHeaderForm.Text = "LAPORAN BARANG RETUR PENJUALAN" : BukaFormMdi(My.Forms.FormLapReturJual)
    End Sub

    Private Sub ByTanggalBelanjaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalBelanjaToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapHutang.LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY PEMBELIAN" : BukaFormMdi(My.Forms.FormLapHutang)
    End Sub

    Private Sub ByTanggalPelunasanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalPelunasanToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapHutang.LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY PELUNASAN" : BukaFormMdi(My.Forms.FormLapHutang)
    End Sub

    Private Sub ByTanggalJatuhTempoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalJatuhTempoToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapHutang.LblHeaderForm.Text = "LAPORAN HUTANG KE SUPPLIER BY JATUH TEMPO" : BukaFormMdi(My.Forms.FormLapHutang)
    End Sub

    Private Sub ByTanggalPenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalPenjualanToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapPiutang.LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY PENJUALAN" : BukaFormMdi(My.Forms.FormLapPiutang)
    End Sub

    Private Sub ByTanggalPelunasanToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalPelunasanToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapPiutang.LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY PELUNASAN" : BukaFormMdi(My.Forms.FormLapPiutang)
    End Sub

    Private Sub ByTanggalJatuhTempoToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalJatuhTempoToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapPiutang.LblHeaderForm.Text = "LAPORAN PIUTANG PELANGGAN BY JATUH TEMPO" : BukaFormMdi(My.Forms.FormLapPiutang)
    End Sub

    Private Sub KasPenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KasPenjualanToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapkAS)
    End Sub

    Private Sub TransferStokToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TransferStokToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapTransferStok)
    End Sub

    Private Sub TransferBarangToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles TransferBarangToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapTransferBarang.LblHeaderForm.Text = "LAPORAN TRANSFER BARANG" : BukaFormMdi(My.Forms.FormLapTransferBarang)
    End Sub

    Private Sub TransferBarangDetailToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransferBarangDetailToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapTransferBarang.LblHeaderForm.Text = "LAPORAN TRANSFER BARANG DETAIL" : BukaFormMdi(My.Forms.FormLapTransferBarang)
    End Sub

    Private Sub StokOpnameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StokOpnameToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(NotaStokOpname)
    End Sub

    Private Sub GrafikToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GrafikToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormGrafikLaba)
    End Sub

    Private Sub HistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistoryToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormHistory)
    End Sub

    Private Sub StokBarangToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles StokBarangToolStripMenuItem1.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapBarang)
    End Sub

    Private Sub KartuStokToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles KartuStokToolStripMenuItem1.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormKartuStok)
    End Sub

    Private Sub StokBarangTerlarisToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StokBarangTerlarisToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapBarangTerlaris)
    End Sub

    Private Sub StokBarangTakBergerakToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StokBarangTakBergerakToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapStokMinim_takGerak.JenisLaporan = "BarangTidakBergerak" : BukaFormMdi(My.Forms.FormLapStokMinim_takGerak)
    End Sub

    Private Sub StokMinimumToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles StokMinimumToolStripMenuItem1.Click
        TutupSemuaForm() : My.Forms.FormLapStokMinim_takGerak.JenisLaporan = "StokMinimum" : BukaFormMdi(My.Forms.FormLapStokMinim_takGerak)
    End Sub

    Private Sub StokLampauToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StokLampauToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapStokLampau)
    End Sub

    Private Sub RekapBayarHutangToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RekapBayarHutangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRekapBayar.JenisLaporan = "Hutang" : BukaFormMdi(My.Forms.FormLapRekapBayar)
    End Sub

    Private Sub RekapBayarPiutangToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RekapBayarPiutangToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRekapBayar.JenisLaporan = "Piutang" : BukaFormMdi(My.Forms.FormLapRekapBayar)
    End Sub

    Private Sub RankingSupplierToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RankingSupplierToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRanking.JenisLaporan = "Supplier" : BukaFormMdi(My.Forms.FormLapRanking)
    End Sub

    Private Sub RankingKasirUserPenjualanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RankingKasirUserPenjualanToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRanking.JenisLaporan = "Kasir" : BukaFormMdi(My.Forms.FormLapRanking)
    End Sub

    Private Sub RankingBarangTerbanyakDibeliToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RankingBarangTerbanyakDibeliToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRanking.JenisLaporan = "BarangBeli" : BukaFormMdi(My.Forms.FormLapRanking)
    End Sub

    Private Sub RankingPelangganPiutangTerbesarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RankingPelangganPiutangTerbesarToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRankingTagihan.JenisLaporan = "Piutang" : BukaFormMdi(My.Forms.FormLapRankingTagihan)
    End Sub

    Private Sub RankingSupplierHutangTerbesarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RankingSupplierHutangTerbesarToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapRankingTagihan.JenisLaporan = "Hutang" : BukaFormMdi(My.Forms.FormLapRankingTagihan)
    End Sub

    Private Sub OmsetPerPelangganToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OmsetPerPelangganToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapOmset.JenisLaporan = "Pelanggan" : BukaFormMdi(My.Forms.FormLapOmset)
    End Sub

    Private Sub OmsetPerKategoriToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OmsetPerKategoriToolStripMenuItem.Click
        TutupSemuaForm() : My.Forms.FormLapOmset.JenisLaporan = "Kategori" : BukaFormMdi(My.Forms.FormLapOmset)
    End Sub

    Private Sub ProfiMarginToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProfiMarginToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormLapMarginProfit)
    End Sub


    '----------------------------------------- UTILITY ---------------------------------------------------------------------------

    Private Sub SystemTutupBulanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SystemTutupBulanToolStripMenuItem.Click
        'With FormPenjualan1
        '    .MdiParent = Me
        '    .BringToFront()
        '    .Dock = DockStyle.Fill
        '    .Show()
        'End With
    End Sub

    Private Sub PilihanSaatMasukAplikasiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PilihanSaatMasukAplikasiToolStripMenuItem.Click
        TutupSemuaForm()
        With FormPilihanMasuk
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub

    Private Sub DatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatabaseToolStripMenuItem.Click
        TutupSemuaForm()
        With SettingDatabase
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub

    Private Sub FormatSqlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormatSqlToolStripMenuItem.Click
        TutupSemuaForm()
        Cursor = Cursors.WaitCursor
        BackupDatabase("SQL")
        Cursor = Cursors.Default
    End Sub

    Private Sub FormatZipToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormatZipToolStripMenuItem.Click
        TutupSemuaForm()
        Cursor = Cursors.WaitCursor
        BackupDatabase("ZIP")
        Cursor = Cursors.Default
    End Sub

    Private Sub FormatSqlToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FormatSqlToolStripMenuItem1.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            openFileDialog.Title = "Pilih File Backup"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim backupFilePath As String = openFileDialog.FileName

                ' Deserialisasi konfigurasi dari file biner
                Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
                    Dim json As String = File.ReadAllText(configFilePath)
                    Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
                    konfigurasi.Password = DecryptPassword(konfigurasi.Password)

                    Cursor = Cursors.WaitCursor
                    ' Panggil metode restore
                    DatabaseRestore.RestoreDatabase(konfigurasi, backupFilePath)
                    Cursor = Cursors.Default
                End Using
            End If
        End Using
    End Sub

    Private Sub FormatZipToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FormatZipToolStripMenuItem1.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "ZIP Files (*.zip)|*.zip|All Files (*.*)|*.*"
            openFileDialog.Title = "Pilih File Backup ZIP"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim backupFilePath As String = openFileDialog.FileName

                Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
                    Dim json As String = File.ReadAllText(configFilePath)
                    Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
                    konfigurasi.Password = DecryptPassword(konfigurasi.Password)

                    Cursor = Cursors.WaitCursor
                    DatabaseRestore.RestoreDatabase(konfigurasi, backupFilePath)
                    Cursor = Cursors.Default
                End Using
            End If
        End Using
    End Sub


    Private Sub PerbaikiDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PerbaikiDatabaseToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormPerbaikanDatabase)
    End Sub

    Private Sub UpdateTabelDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateTabelDatabaseToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormUpdateTabelDb)
    End Sub

    Private Sub QueryDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QueryDatabaseToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormQuery)
    End Sub

    Private Sub MigrasiDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MigrasiDatabaseToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormMigrasiDB)
    End Sub

    Private Sub SettingPrinterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingPrinterToolStripMenuItem.Click
        TutupSemuaForm() : BukaFormMdi(My.Forms.FormPengaturanPrinter)
    End Sub

    Private Sub HapusTransaksiTokoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HapusTransaksiTokoToolStripMenuItem.Click
        TutupSemuaForm()
        Using f As New FormHapusTransaksi With {.Mode = "TOKO"}
            f.ShowDialog()
        End Using
    End Sub

    Private Sub HapusTransaksiGudangToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HapusTransaksiGudangToolStripMenuItem.Click
        TutupSemuaForm()
        Using f As New FormHapusTransaksi With {.Mode = "GUDANG"}
            f.ShowDialog()
        End Using
    End Sub

    Private Sub HapusTransaksiSemuaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HapusTransaksiSemuaToolStripMenuItem.Click
        TutupSemuaForm()
        Using f As New FormHapusTransaksi With {.Mode = "SEMUA"}
            f.ShowDialog()
        End Using
    End Sub


    Private Sub PeriksaUpdateAplikasiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PeriksaUpdateAplikasiToolStripMenuItem.Click
        TutupSemuaForm()
        With FormCekUpdate
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub

    Private Sub CekIpKomputerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CekIpKomputerToolStripMenuItem.Click
        TutupSemuaForm()
        With Formipkomputer
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub


    Public Sub BackupDatabase(ByVal typedata As String)

        ' Deserialisasi konfigurasi dari file biner
        Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
            Dim json As String = File.ReadAllText(configFilePath)
            Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
            konfigurasi.Password = DecryptPassword(konfigurasi.Password)

            ' Ganti informasi koneksi berikut sesuai dengan konfigurasi Anda

            ' Ambil nilai dari TextBox di form SettingDatabase
            Dim config As New DatabaseConfiguration() With {
            .Server = konfigurasi.Server,
            .Port = konfigurasi.Port,
            .User = konfigurasi.User,
            .Password = konfigurasi.Password,
            .Database = konfigurasi.Database
        }

            MySqlBackup.BackupDatabase(config, typedata)
        End Using
    End Sub



    ' Deklarasi variabel untuk menyimpan hasil query
    Dim record As String = ""
    Dim Qty As Decimal = 0
    Dim Rupiah As Decimal = 0
    Private Sub AmbilNilaiBarang(ByVal transaction As MySqlTransaction, ByVal LokasiQuery As String)
        Dim querySelect As String = "SELECT FAKTUR as record, Sum(QTY) as QTY, sum(TOTAL_RUPIAH) as Rupiah " &
                             "FROM HistoryBarang " &
                             "WHERE LOKASI Like @Lokasi"
        ' Eksekusi query SELECT
        Using cmdSelect As New MySqlCommand(querySelect, conn, transaction)
            cmdSelect.Parameters.AddWithValue("@Lokasi", LokasiQuery.ToUpper()) ' Mengisi parameter lokasi dengan huruf kapital
            Using reader As MySqlDataReader = cmdSelect.ExecuteReader()
                If reader.Read() Then
                    ' Mengambil data dari hasil query
                    record = If(reader("record") IsNot DBNull.Value, reader("record").ToString(), String.Empty)
                    Qty = If(reader("QTY") IsNot DBNull.Value, Convert.ToDecimal(reader("QTY")), 0D)
                    Rupiah = If(reader("Rupiah") IsNot DBNull.Value, Convert.ToDecimal(reader("Rupiah")), 0D)
                End If
            End Using
        End Using
    End Sub

    Private Sub JurnalEksekusiTransaksi(ByVal transaction As MySqlTransaction, ByVal LokasiQuery As String)
        ' Membuat nomor transaksi unik berdasarkan tanggal dan waktu saat ini
        Dim noTransaksi As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        ' Eksekusi INSERT ke dalam JurnalUmum
        Using cmdInsert As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                             "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            ' Tambahkan parameter untuk query INSERT
            cmdInsert.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmdInsert.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdInsert.Parameters.AddWithValue("@URAIAN", "Hapus Transaksi " & LokasiQuery & " barang : " & record & " Qty : " & Qty.ToString())
            cmdInsert.Parameters.AddWithValue("@NAMA_AKUN_D", LAWAN_NAMA_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NOMOR_AKUN_D", LAWAN_KODE_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NOMINAL", Rupiah)
            cmdInsert.Parameters.AddWithValue("@JENIS_TRANSAKSI", "HAPUS BARANG")
            cmdInsert.Parameters.AddWithValue("@LOKASI", "")
            cmdInsert.Parameters.AddWithValue("@ID_USER", StatusNamaUser.Text)
            cmdInsert.Parameters.AddWithValue("@ID_KOMPUTER", StatusNamaPC.Text)

            ' Eksekusi perintah INSERT
            cmdInsert.ExecuteNonQuery()
        End Using
    End Sub




    '----------------------------------------- WINDOWS ---------------------------------------------------------------------------

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CascadeToolStripMenuItem.Click
        LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TitleHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TitleHorizontalToolStripMenuItem.Click
        LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub TitelVerticalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TitelVerticalToolStripMenuItem.Click
        LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ArrangeIconsToolStripMenuItem.Click
        LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CloseAllToolStripMenuItem.Click
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        TutupSemuaForm()
    End Sub

    '----------------------------------------- KELUAR ---------------------------------------------------------------------------

    Private Sub KeluarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles KeluarToolStripMenuItem.Click
        Close()
    End Sub


    Private Sub FormUtama_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        ' Tanyakan apakah pengguna ingin melakukan backup sebelum keluar
        If MessageBox.Show("BACKUP DATA ?", "Konfirmasi Backup", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ' Panggil metode BackupDatabase jika pengguna memilih "Yes"
            Me.Cursor = Cursors.WaitCursor
            Dim typesql As String = "ZIP"
            BackupDatabase(typesql)
            Me.Cursor = Cursors.Default
        End If

        ' Tanyakan apakah pengguna ingin keluar dari aplikasi
        If MessageBox.Show("Apakah Anda yakin ingin keluar dari aplikasi ini?", "Informasi Keluar", MessageBoxButtons.YesNo) = DialogResult.No Then
            ' Batalkan penutupan form
            e.Cancel = True
        Else
            ' CATATAN HISTORY jika pengguna memilih "Yes"
        End If
    End Sub


    Private Sub BtnNotif_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnNotif.Click
        ' Buat instance dari form NotifikasiJatuhTempo
        Dim notifForm As New NotifikasiJatuhTempo()

        ' Dapatkan posisi layar dari BtnNotif
        Dim btnPosition As Point = BtnNotif.PointToScreen(New Point(BtnNotif.Width, BtnNotif.Height))

        ' Setel posisi form NotifikasiJatuhTempo di samping pojok kanan atas BtnNotif
        notifForm.StartPosition = FormStartPosition.Manual

        ' Sesuaikan posisi form agar rata kanan dengan BtnNotif
        Dim notifFormX As Integer = btnPosition.X - notifForm.Width
        Dim notifFormY As Integer = btnPosition.Y

        ' Setel posisi form
        notifForm.Location = New Point(notifFormX, notifFormY)

        ' Tampilkan form NotifikasiJatuhTempo
        notifForm.Show()
    End Sub

    Private Sub DGVTransaksi_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DGVTransaksi.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DGVTransaksi.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DGVDetail_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DGVDetail.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DGVDetail.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub



    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter()
            frm.FilterTab = GetFilterTabPrinter(TxtTransaksi.Text)

            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

    ' ==================== DARK / LIGHT MODE ====================

    Private Sub BtnMode_Click(sender As Object, e As EventArgs) Handles BtnMode.Click
        ModuleTheme.Toggle()
        TerapkanModeSemua()
    End Sub

    ''' <summary>Terapkan theme ke FormUtama dan semua MDI children</summary>
    Public Sub TerapkanModeSemua()
        ' Update icon & tooltip BtnMode
        Dim oldImg As Image = BtnMode.Image
        BtnMode.Image = ModuleTheme.GetModeIcon(16)
        oldImg?.Dispose()
        ToolTip1.SetToolTip(BtnMode, ModuleTheme.GetModeTooltip())

        ' BtnMode warna — ikut toolbar
        BtnMode.BackColor = ModuleTheme.C(ModuleTheme.L_NavIdle, ModuleTheme.D_NavIdle)
        BtnMode.ForeColor = ModuleTheme.C(ModuleTheme.L_NavIdleFore, ModuleTheme.D_NavIdleFore)
        BtnMode.FlatStyle = FlatStyle.Flat
        BtnMode.FlatAppearance.BorderSize = 0
        BtnMode.FlatAppearance.MouseOverBackColor = ModuleTheme.C(ModuleTheme.L_NavHover, ModuleTheme.D_NavHover)
        BtnMode.FlatAppearance.MouseDownBackColor = ModuleTheme.C(ModuleTheme.L_NavDown, ModuleTheme.D_NavDown)

        ' Terapkan ke FormUtama
        ModuleTheme.TerapkanTheme(Me)

        ' Terapkan ke semua MDI children
        For Each frm As Form In MdiChildren
            ModuleTheme.TerapkanTheme(frm)
        Next

        ' Restore warna button nav aktif yang di-reset oleh TerapkanFormUtama
        If _activeNavButton IsNot Nothing Then
            _activeNavButton.BackColor = ModuleTheme.C(ModuleTheme.L_NavActive, ModuleTheme.D_NavActive)
            _activeNavButton.ForeColor = ModuleTheme.C(ModuleTheme.L_NavActiveFore, ModuleTheme.D_NavActiveFore)
        End If

        ' Refresh Dashboard jika sedang tampil
        If WbDashboard.Visible Then TampilDashboard()

        Me.Refresh()
    End Sub

    ' ==================== END DARK / LIGHT MODE ====================

    Private Sub EditPembayaranToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPembayaranToolStripMenuItem.Click
        If TxtTransaksi.Text <> "Penjualan" Then
            MessageBox.Show("Untuk sementara edit pembayaran hanya tersedia untuk transaksi penjualan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(TxtFakturTransaksi.Text) Then
            MessageBox.Show("Pilih transaksi penjualan yang akan diedit pembayarannya.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If StatusLokasi.Text <> TxtLokasiUntukEdit.Text Then
            Dim pesan As String = "Oops! Tidak ada hak untuk edit pembayaran penjualan ini." & Environment.NewLine &
                                  "User " & StatusLokasi.Text & " tidak berhak edit transaksi penjualan " & TxtLokasiUntukEdit.Text
            MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN", conn)
            cmdCheck.Parameters.AddWithValue("@ID_PENJUALAN", TxtFakturTransaksi.Text)
            Dim rowCount As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

            If rowCount > 0 Then
                Dim pesan As String = "Oops! Pembayaran tidak dapat diedit karena transaksi ini sudah memiliki retur penjualan." & Environment.NewLine &
                                      "Silakan sesuaikan retur terlebih dahulu jika ingin mengubah pembayaran."
                MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        End Using

        Using frm As New FormEditBayarJual()
            frm.IdPenjualan = TxtFakturTransaksi.Text
            If frm.ShowDialog() = DialogResult.OK Then
                Refresdatagridview()
            End If
        End Using
    End Sub

    Private Sub LabaRugiBerjalanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LabaRugiBerjalanToolStripMenuItem.Click

    End Sub
End Class

