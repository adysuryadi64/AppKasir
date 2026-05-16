Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Ports
Imports System.Management

Public Class FormPengaturanPrinter

    ' Jika diisi, hanya tab transaksi ini yang ditampilkan (dipanggil dari form transaksi)
    ' Contoh: New FormPengaturanPrinter("Jual") -> hanya tab Penjualan
    ' Kosong = tampil semua tab (dipanggil dari menu utama)
    Public Property FilterTab As String = ""
    Private _iniCache As Dictionary(Of String, String) = Nothing
    Private _printerPortCache As Dictionary(Of String, String) = Nothing  ' cache port per printer
    Private _defaultPrinterCache As String = Nothing                       ' cache default printer

#Region "Load Form"

    Private Sub FormPengaturanPrinter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.KeyPreview = True
        Me.Cursor = Cursors.WaitCursor
        AddHandler TabTransaksi.DrawItem, AddressOf TabTransaksi_DrawItem

        ' Bangun cache WMI di background — tidak blocking UI
        _defaultPrinterCache = New PrinterSettings().PrinterName
        _printerPortCache = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Task.Run(Sub()
                     Try
                         Dim cache As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
                         Dim query As New ManagementObjectSearcher("SELECT Name, PortName FROM Win32_Printer")
                         For Each obj As ManagementObject In query.Get()
                             Dim n As String = If(obj("Name") IsNot Nothing, obj("Name").ToString(), "")
                             Dim p As String = If(obj("PortName") IsNot Nothing, obj("PortName").ToString(), "")
                             If Not String.IsNullOrEmpty(n) Then cache(n) = p
                         Next
                         _printerPortCache = cache
                     Catch
                     End Try
                 End Sub)

        BangunTabUI()
        TerapkanFilterTab()
        ' Inisialisasi warna header untuk tab pertama yang aktif
        If String.IsNullOrEmpty(FilterTab) AndAlso TabTransaksi.TabPages.Count > 0 Then
            SetWarnaHeader(TabTransaksi.TabPages(0).Name)
        End If
        IsiDaftarPrinter()
        IsiDaftarPort()
        IsiDaftarFont()
        MuatSemuaTab()

        Me.Cursor = Cursors.Default
    End Sub

    ''' <summary>Set warna PnlHeader dan PnlBottom dengan warna Cetak yang konsisten + update teks judul.</summary>
    Private Sub SetWarnaHeader(key As String)
        Dim back As Color = ModuleTheme.C(ModuleTheme.L_HeaderCetak, ModuleTheme.D_HeaderCetak)
        PnlHeader.BackColor = back
        PnlBottom.BackColor = back
        LblTitleHeader.Text = "Pengaturan Printer : " & GetJudulTab(key)
    End Sub

    Private Sub FormPengaturanPrinter_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F8 Then
            e.Handled = True
            BtnSimpan.PerformClick()
        ElseIf e.KeyCode = Keys.Escape Then
            e.Handled = True
            BtnKeluar.PerformClick()
        End If
    End Sub

    ''' <summary>
    ''' Gambar tab header dengan satu warna konsisten — aktif = warna Cetak, tidak aktif = abu-abu.
    ''' Teks selalu putih agar kontras di kedua mode.
    ''' </summary>
    Private Sub TabTransaksi_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim tab As TabPage = TabTransaksi.TabPages(e.Index)
        Dim isSelected As Boolean = (TabTransaksi.SelectedIndex = e.Index)

        ' Aktif: warna Cetak | Tidak aktif: abu-abu sesuai mode
        Dim bgColor As Color = If(isSelected,
            ModuleTheme.C(ModuleTheme.L_HeaderCetak, ModuleTheme.D_HeaderCetak),
            ModuleTheme.C(Color.FromArgb(203, 213, 225), Color.FromArgb(51, 65, 85)))  ' Slate-300 / Slate-700

        Using br As New SolidBrush(bgColor)
            e.Graphics.FillRectangle(br, e.Bounds)
        End Using

        ' Teks: putih di atas warna Cetak, hitam/putih di atas abu-abu
        Dim txtColor As Color = If(isSelected,
            Color.White,
            ModuleTheme.C(Color.FromArgb(30, 41, 59), Color.FromArgb(203, 213, 225)))  ' Slate-800 / Slate-300

        Dim fs As FontStyle = If(isSelected, FontStyle.Bold, FontStyle.Regular)
        Using fnt As New Font("Segoe UI", 9, fs)
            Dim sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
            e.Graphics.DrawString(tab.Text, fnt, New SolidBrush(txtColor), RectangleF.op_Implicit(e.Bounds), sf)
        End Using
    End Sub

#End Region

#Region "Filter Tab"

    Private Sub TerapkanFilterTab()
        If String.IsNullOrEmpty(FilterTab) Then Exit Sub

        ' Hapus semua tab kecuali yang sesuai FilterTab — bukan sekadar disable
        Dim tabsToRemove As New List(Of TabPage)()
        For Each tab As TabPage In TabTransaksi.TabPages
            If tab.Name <> FilterTab Then tabsToRemove.Add(tab)
        Next
        For Each tab As TabPage In tabsToRemove
            TabTransaksi.TabPages.Remove(tab)
        Next

        ' Pilih tab yang tersisa (satu-satunya) dan pastikan sudah dibangun
        If TabTransaksi.TabPages.Count > 0 Then
            BangunKontenTab(TabTransaksi.TabPages(0))
            TabTransaksi.SelectedIndex = 0
        End If

        SetWarnaHeader(FilterTab)
    End Sub

    Private Function GetJudulTab(key As String) As String
        Select Case key
            Case "Jual" : Return "Penjualan"
            Case "Beli" : Return "Pembelian"
            Case "ReturJual" : Return "Retur Jual"
            Case "ReturBeli" : Return "Retur Beli"
            Case "SuratJalan" : Return "Surat Jalan"
            Case "TransferBarang" : Return "Transfer Barang"
            Case "TransferCabang" : Return "Transfer Cabang"
            Case "BayarHutang" : Return "Bayar Hutang"
            Case "BayarPiutang" : Return "Bayar Piutang"
            Case "Gaji" : Return "Slip Gaji"
            Case "Bon" : Return "Bon Karyawan"
            Case "Laporan" : Return "Laporan"
            Case Else : Return key
        End Select
    End Function

#End Region

#Region "Bangun Tab UI Dinamis"

    Private Sub BangunTabUI()
        Dim tabDefs As String(,) = {
            {"Jual", "Penjualan"},
            {"Beli", "Pembelian"},
            {"ReturJual", "Retur Jual"},
            {"ReturBeli", "Retur Beli"},
            {"SuratJalan", "Surat Jalan"},
            {"TransferBarang", "Transfer Barang"},
            {"TransferCabang", "Transfer Cabang"},
            {"BayarHutang", "Bayar Hutang"},
            {"BayarPiutang", "Bayar Piutang"},
            {"Gaji", "Slip Gaji"},
            {"Bon", "Bon Karyawan"},
            {"Laporan", "Laporan"}
        }
        For i As Integer = 0 To tabDefs.GetUpperBound(0)
            ' Buat TabPage kosong dulu — konten dibuat saat pertama kali diklik (lazy)
            Dim key As String = tabDefs(i, 0)
            Dim judul As String = tabDefs(i, 1)
            Dim tab As New TabPage() With {
                .Name = key,
                .Text = judul,
                .BackColor = ModuleTheme.C(Color.FromArgb(248, 250, 255), Color.FromArgb(30, 41, 59)),
                .Padding = New Padding(8),
                .AutoScroll = True,
                .Tag = "pending"   ' penanda belum dibangun
            }
            TabTransaksi.TabPages.Add(tab)
        Next

        ' Bangun tab pertama langsung (yang langsung terlihat)
        BangunKontenTab(TabTransaksi.TabPages(0))

        AddHandler TabTransaksi.Selecting, AddressOf TabTransaksi_Selecting

        Dim tt As New ToolTip()
        tt.SetToolTip(CmbStatusKomputer,
            "Server    : Komputer utama / pusat data" & vbCrLf &
            "Admin1-3  : Komputer admin / back office" & vbCrLf &
            "Kasir1-3  : Komputer kasir / point of sale" & vbCrLf & vbCrLf &
            "Pilih sesuai fungsi komputer ini di jaringan toko.")
    End Sub

    ' Bangun konten tab saat pertama kali dipilih
    Private Sub TabTransaksi_Selecting(sender As Object, e As TabControlCancelEventArgs)
        BangunKontenTab(e.TabPage)
        SetWarnaHeader(e.TabPage.Name)
    End Sub

    Private Sub BangunKontenTab(tab As TabPage)
        If tab Is Nothing OrElse tab.Tag?.ToString() <> "pending" Then Return
        tab.Tag = "built"

        Dim key As String = tab.Name
        Dim judul As String = tab.Text

        tab.SuspendLayout()

        tab.Controls.Add(New Label() With {
            .Text = "Pengaturan Printer : " & judul,
            .Font = New Font("Segoe UI", 13, FontStyle.Bold),
            .ForeColor = ModuleTheme.C(Color.FromArgb(30, 80, 160), Color.FromArgb(147, 197, 253)),
            .AutoSize = True,
            .Location = New Point(10, 10)
        })
        tab.Controls.Add(New Label() With {
            .Text = "Jenis Printer yang Digunakan :",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(10, 44)
        })

        Dim cmbJenis As New ComboBox() With {
            .Name = "cmbJenisPrinter_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Location = New Point(260, 40),
            .Size = New Size(260, 28)
        }
        cmbJenis.Items.AddRange(New Object() {
            "Printer Thermal", "Printer Dot Matrix",
            "Printer Inkjet / Laser", "Tampilkan di Monitor", "Export ke PDF"})
        cmbJenis.Tag = tab
        AddHandler cmbJenis.SelectedIndexChanged, AddressOf CmbJenisPrinter_SelectedIndexChanged
        tab.Controls.Add(cmbJenis)

        tab.Controls.Add(New Label() With {
            .Text = "  Pilih jenis printer sesuai perangkat yang terpasang untuk transaksi ini",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.Gray,
            .AutoSize = True,
            .Location = New Point(530, 46)
        })

        Dim btnRestore As New Button() With {
            .Text = "Restore Default",
            .Font = New Font("Segoe UI", 9),
            .BackColor = ModuleTheme.C(Color.FromArgb(71, 85, 105), Color.FromArgb(51, 65, 85)),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Size = New Size(140, 28),
            .Location = New Point(900, 40),
            .Tag = tab
        }
        AddHandler btnRestore.Click, AddressOf BtnRestore_Click
        tab.Controls.Add(btnRestore)

        Dim grpT As GroupBox = BuatPanelThermal(key)
        grpT.Location = New Point(8, 78)
        grpT.Size = New Size(1060, 740)
        grpT.Visible = True
        Dim btnTesCash As Button = TryCast(grpT.Controls.Find("btnTesCash_" & key, True).FirstOrDefault(), Button)
        If btnTesCash IsNot Nothing Then btnTesCash.Tag = tab
        tab.Controls.Add(grpT)

        Dim grpD As GroupBox = BuatPanelDot(key)
        grpD.Location = New Point(8, 78)
        grpD.Size = New Size(1060, 490)
        grpD.Visible = False
        tab.Controls.Add(grpD)

        Dim grpI As GroupBox = BuatPanelInk(key)
        grpI.Location = New Point(8, 78)
        grpI.Size = New Size(1060, 490)
        grpI.Visible = False
        tab.Controls.Add(grpI)

        Dim grpM As GroupBox = BuatPanelFooterSaja(key, "Monitor", "Tampilkan di Monitor",
                                                    Color.FromArgb(245, 240, 255), Color.FromArgb(60, 0, 120))
        grpM.Location = New Point(8, 78)
        grpM.Size = New Size(1060, 185)
        grpM.Visible = False
        tab.Controls.Add(grpM)

        Dim grpPdf As GroupBox = BuatPanelFooterSaja(key, "PDF", "Export ke PDF",
                                                      Color.FromArgb(240, 255, 245), Color.FromArgb(0, 100, 40))
        grpPdf.Location = New Point(8, 78)
        grpPdf.Size = New Size(1060, 185)
        grpPdf.Visible = False
        tab.Controls.Add(grpPdf)

        tab.ResumeLayout(True)

        If _iniCache Is Nothing Then _iniCache = BacaIni()
        IsiDaftarPrinterUntukTab(key)
        IsiDaftarPortUntukTab(key)
        IsiDaftarFontUntukTab(key)
        MuatTab(tab, _iniCache, key)
        tab.Tag = "loaded"

        ' Apply theme ke semua kontrol yang baru dibuat secara dinamis
        TerapkanThemeTab(tab)
    End Sub

    ''' <summary>Apply warna mode-aware ke semua label di dalam tab yang baru dibangun.</summary>
    Private Sub TerapkanThemeTab(tab As TabPage)
        Dim backTab As Color = ModuleTheme.C(Color.FromArgb(248, 250, 255), Color.FromArgb(30, 41, 59))
        tab.BackColor = backTab
        TerapkanKontrolTabRekursif(tab)
    End Sub

    ''' <summary>Rekursif — apply warna ke semua kontrol di dalam tab berdasarkan nama/tag.</summary>
    Private Sub TerapkanKontrolTabRekursif(parent As Control)
        Dim dark As Boolean = ModuleTheme.IsDarkMode

        For Each ctrl As Control In parent.Controls
            Select Case True
                Case TypeOf ctrl Is Label
                    Dim lbl = CType(ctrl, Label)
                    ' Identifikasi via nama kontrol (lebih reliable dari warna)
                    Dim nm As String = lbl.Name.ToLower()
                    If nm.StartsWith("lblpengaturan") OrElse nm = "" AndAlso lbl.Font.Size >= 12 Then
                        ' Judul tab — aksen biru
                        lbl.ForeColor = ModuleTheme.C(Color.FromArgb(30, 80, 160), Color.FromArgb(147, 197, 253))
                    ElseIf nm.StartsWith("lblprinteraktif") OrElse nm.StartsWith("lblstatus") Then
                        ' Status/info — hijau
                        lbl.ForeColor = ModuleTheme.C(Color.FromArgb(22, 163, 74), Color.FromArgb(134, 239, 172))
                    ElseIf nm.StartsWith("lblportusb") AndAlso Not nm.EndsWith("caption") Then
                        ' Port info — biru slate
                        lbl.ForeColor = ModuleTheme.C(Color.DarkSlateBlue, Color.FromArgb(147, 197, 253))
                    ElseIf lbl.Font.Size <= 8 OrElse lbl.Size.Width > 400 Then
                        ' Hint/keterangan panjang — abu
                        lbl.ForeColor = ModuleTheme.C(Color.FromArgb(100, 116, 139), Color.FromArgb(148, 163, 184))
                    Else
                        ' Label biasa
                        lbl.ForeColor = ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240))
                    End If
                    lbl.BackColor = Color.Transparent

                Case TypeOf ctrl Is GroupBox
                    Dim grp = CType(ctrl, GroupBox)
                    ' GroupBox dengan nama grpPrinter/grpKertas/grpLaci/grpFont = sub-section
                    grp.BackColor = ModuleTheme.C(Color.Transparent, Color.FromArgb(15, 23, 42))
                    grp.ForeColor = ModuleTheme.C(Color.FromArgb(80, 60, 20), Color.FromArgb(148, 163, 184))
                    TerapkanKontrolTabRekursif(grp)

                Case TypeOf ctrl Is TextBox
                    ctrl.BackColor = ModuleTheme.C(Color.White, Color.FromArgb(30, 41, 59))
                    ctrl.ForeColor = ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240))

                Case TypeOf ctrl Is ComboBox
                    ctrl.BackColor = ModuleTheme.C(Color.White, Color.FromArgb(30, 41, 59))
                    ctrl.ForeColor = ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240))

                Case TypeOf ctrl Is CheckBox OrElse TypeOf ctrl Is RadioButton
                    ctrl.ForeColor = ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240))
                    ctrl.BackColor = Color.Transparent

                Case TypeOf ctrl Is NumericUpDown
                    ctrl.BackColor = ModuleTheme.C(Color.White, Color.FromArgb(30, 41, 59))
                    ctrl.ForeColor = ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240))

                Case TypeOf ctrl Is Button
                    ' Jangan override button — biarkan style dari designer
            End Select
        Next
    End Sub

#End Region

#Region "Panel Thermal"

    Private Function BuatPanelThermal(key As String) As GroupBox
        Dim grp As New GroupBox() With {
            .Name = "grpThermal_" & key,
            .Text = "  Pengaturan Printer Thermal / Struk",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .BackColor = ModuleTheme.C(Color.FromArgb(255, 252, 240), Color.FromArgb(15, 23, 42)),
            .ForeColor = ModuleTheme.C(Color.FromArgb(150, 80, 0), Color.FromArgb(251, 191, 36))
        }

        Dim y As Integer = 22
        Const xL As Integer = 10
        Const xC As Integer = 170
        Const lebarPanel As Integer = 1040

        ' ── 1. PRINTER ──────────────────────────────────────────
        Dim grpPrinter As New GroupBox() With {
            .Text = "Printer",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(100, 60, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(lebarPanel, 152)
        }

        Dim lblModeCetak As Label = MakeLbl("lblModeCetak_" & key, "Mode Cetak :", 8, 14)
        grpPrinter.Controls.Add(lblModeCetak)
        Dim cmbMode As New ComboBox() With {
            .Name = "cmbModeCetak_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .Location = New Point(xC - 10, 14),
            .Size = New Size(200, 26)
        }
        cmbMode.Items.AddRange(New Object() {"ESC/POS (Raw)", "GDI+ (Windows Print)"})
        grpPrinter.Controls.Add(cmbMode)
        grpPrinter.Controls.Add(New Label() With {
            .Text = "ESC/POS (Raw) = perintah langsung ke printer thermal, tanpa driver Windows  |  GDI+ (Windows Print) = cetak via driver Windows, mendukung font & logo",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Regular),
            .ForeColor = Color.Gray,
            .AutoSize = False,
            .Size = New Size(900, 20),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(xC - 10, 40)
        })

        Dim lblTipeKoneksi As Label = MakeLbl("lblTipeKoneksi_" & key, "Tipe Koneksi :", 8, 64)
        grpPrinter.Controls.Add(lblTipeKoneksi)
        Dim cmbKoneksi As New ComboBox() With {
            .Name = "cmbTipeKoneksi_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC - 10, 64),
            .Size = New Size(160, 26)
        }
        cmbKoneksi.Items.AddRange(New Object() {"USB / Windows Spooler", "Network / WiFi (IP)"})
        grpPrinter.Controls.Add(cmbKoneksi)

        Dim lblNamaPrinter As Label = MakeLbl("lblNamaPrinter_" & key, "Nama Printer :", 8, 96)
        grpPrinter.Controls.Add(lblNamaPrinter)
        Dim cmbP As New ComboBox() With {
            .Name = "cmbPrinterThermal_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC - 10, 96),
            .Size = New Size(340, 26)
        }
        grpPrinter.Controls.Add(cmbP)

        Dim lblPortUsbCaption As Label = MakeLbl("lblPortUsbCaption_" & key, "Port :", xC + 340, 96)
        grpPrinter.Controls.Add(lblPortUsbCaption)
        Dim lblPortUsb As New Label() With {
            .Name = "lblPortUsb_" & key,
            .Text = "",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.DarkSlateBlue,
            .AutoSize = False,
            .Size = New Size(120, 26),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(xC + 380, 96)
        }
        grpPrinter.Controls.Add(lblPortUsb)

        AddHandler cmbP.SelectedIndexChanged,
            Sub(s, ev)
                Dim selectedPrinter As String = CType(s, ComboBox).Text
                lblPortUsb.Text = GetPrinterPort(selectedPrinter)
            End Sub

        Dim lblPrinterAktifT As Label = MakeLbl("lblPrinterAktif_" & key & "_Thermal", "", xC - 10, 126, Color.DarkGreen, 8)
        lblPrinterAktifT.AutoSize = True
        grpPrinter.Controls.Add(lblPrinterAktifT)

        Dim lblIp As Label = MakeLbl("lblIpAddress_" & key, "IP Address :", 8, 96)
        lblIp.Visible = False
        grpPrinter.Controls.Add(lblIp)
        Dim txtIp As New TextBox() With {
            .Name = "txtIpAddress_" & key,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC - 10, 96),
            .Size = New Size(160, 26),
            .Text = "192.168.1.50",
            .Visible = False
        }
        grpPrinter.Controls.Add(txtIp)
        Dim lblNetworkPort As Label = MakeLbl("lblNetworkPort_" & key, "Port :", xC + 160, 96)
        lblNetworkPort.Visible = False
        grpPrinter.Controls.Add(lblNetworkPort)
        Dim txtPort As New TextBox() With {
            .Name = "txtNetworkPort_" & key,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 200, 96),
            .Size = New Size(60, 26),
            .Text = "9100",
            .Visible = False
        }
        grpPrinter.Controls.Add(txtPort)

        AddHandler cmbKoneksi.SelectedIndexChanged,
            Sub(s, ev)
                Dim isNetwork As Boolean = (CType(s, ComboBox).SelectedIndex = 1)
                lblNamaPrinter.Visible = Not isNetwork
                cmbP.Visible = Not isNetwork
                lblPortUsbCaption.Visible = Not isNetwork
                lblPortUsb.Visible = Not isNetwork
                lblIp.Visible = isNetwork
                lblNetworkPort.Visible = isNetwork
                txtIp.Visible = isNetwork
                txtPort.Visible = isNetwork
            End Sub

        grp.Controls.Add(grpPrinter)
        y += 160

        ' ── 2. KERTAS & CETAK ───────────────────────────────────
        Dim grpKertas As New GroupBox() With {
            .Text = "Kertas & Cetak",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(100, 60, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(lebarPanel, 110)
        }
        AddLbl(grpKertas, "Ukuran Kertas :", 8, 16)
        Dim cmbKertas As New ComboBox() With {
            .Name = "cmbUkuranKertas_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC - 10, 16),
            .Size = New Size(150, 26)
        }
        cmbKertas.Items.AddRange(New Object() {"POS-80 (80mm)", "POS-58 (58mm)", "Custom"})
        grpKertas.Controls.Add(cmbKertas)
        AddHandler cmbKertas.SelectedIndexChanged,
            Sub(s, ev)
                Dim cmb As ComboBox = CType(s, ComboBox)
                Dim txtL As TextBox = CariKontrol(Of TextBox)(grpKertas, "txtLebar_" & key)
                If txtL Is Nothing Then Return
                Select Case cmb.Text
                    Case "POS-80 (80mm)" : txtL.Text = "80" : txtL.ReadOnly = True
                    Case "POS-58 (58mm)" : txtL.Text = "58" : txtL.ReadOnly = True
                    Case "Custom" : txtL.ReadOnly = False
                End Select
            End Sub
        Dim lblLebarKertas As Label = MakeLbl("lblLebarKertas_" & key, "Lebar (mm) :", xC + 150, 16)
        grpKertas.Controls.Add(lblLebarKertas)
        AddTxt(grpKertas, "txtLebar_" & key, xC + 240, 16, 55)
        Dim lblBatasKiri As Label = MakeLbl("lblBatasKiri_" & key, "Batas Kiri :", xC + 305, 16)
        grpKertas.Controls.Add(lblBatasKiri)
        AddTxt(grpKertas, "txtBatasKiri_" & key, xC + 375, 16, 50)
        Dim lblJarak As Label = MakeLbl("lblJarakBaris_" & key, "Jarak Baris :", xC + 435, 16)
        lblJarak.AutoSize = True
        grpKertas.Controls.Add(lblJarak)
        AddTxt(grpKertas, "txtJarakGdi_" & key, xC + 530, 16, 50)
        AddTxt(grpKertas, "txtJarakEsc_" & key, xC + 530, 16, 50)
        Dim lblDpi As Label = MakeLbl("lblDpiCetak_" & key, "DPI GDI+ :", xC + 590, 16)
        grpKertas.Controls.Add(lblDpi)
        AddTxt(grpKertas, "txtDpiCetak_" & key, xC + 660, 16, 50)
        Dim lblModelStruk As Label = MakeLbl("lblModelStruk_" & key, "Model Struk :", 8, 50)
        grpKertas.Controls.Add(lblModelStruk)
        Dim cmbModel As New ComboBox() With {
            .Name = "cmbModelStruk_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC - 10, 50),
            .Size = New Size(300, 26)
        }
        IsiModelStruk(cmbModel, isEscPos:=True, transaksi:=key)
        grpKertas.Controls.Add(cmbModel)
        PasangHandlerSeparatorModel(cmbModel)
        Dim lblJumlahCetak As Label = MakeLbl("lblJumlahCetak_" & key, "Jumlah Cetak :", xC + 310, 50)
        grpKertas.Controls.Add(lblJumlahCetak)
        Dim numCopies As New NumericUpDown() With {
            .Name = "numCopiesEsc_" & key,
            .Minimum = 1, .Maximum = 5, .Value = 1,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 410, 50),
            .Size = New Size(55, 26),
            .Visible = True
        }
        grpKertas.Controls.Add(numCopies)
        Dim numCopiesGdi As New NumericUpDown() With {
            .Name = "numCopiesGdi_" & key,
            .Minimum = 1, .Maximum = 5, .Value = 1,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 410, 50),
            .Size = New Size(55, 26),
            .Visible = False
        }
        grpKertas.Controls.Add(numCopiesGdi)
        Dim chkPotongEsc As New CheckBox() With {
            .Name = "chkPotongEsc_" & key,
            .Text = "Potong kertas otomatis",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 480, 52),
            .AutoSize = True,
            .Visible = True
        }
        grpKertas.Controls.Add(chkPotongEsc)
        Dim chkPotongGdi As New CheckBox() With {
            .Name = "chkPotongGdi_" & key,
            .Text = "Potong kertas otomatis",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 480, 52),
            .AutoSize = True,
            .Visible = False
        }
        grpKertas.Controls.Add(chkPotongGdi)
        grp.Controls.Add(grpKertas)
        y += 118

        ' ── 3. LACI KASIR ───────────────────────────────────────
        Dim grpLaci As New GroupBox() With {
            .Name = "grpLaci_" & key,
            .Text = "Laci Kasir (Cash Drawer)",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(100, 60, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(lebarPanel, 70)
        }

        ' Kode perintah — wajib diisi, menentukan pin & pulse
        Dim lblKodePerintah As New Label() With {
            .Name = "lblKodePerintah_" & key,
            .Text = "Buka Laci :",
            .Font = New Font("Segoe UI", 9),
            .AutoSize = False,
            .Size = New Size(TextRenderer.MeasureText("Buka Laci :", New Font("Segoe UI", 9)).Width + 6, 26),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(8, 14)
        }
        grpLaci.Controls.Add(lblKodePerintah)
        Dim cmbCode As New ComboBox() With {
            .Name = "cmbCodeCash_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC - 10, 14),
            .Size = New Size(200, 26)
        }
        cmbCode.Items.AddRange(New Object() {
            "(Tidak Ada)",
            "Pin 2 — Pulse 100ms (standar)",
            "Pin 2 — Pulse 200ms",
            "Pin 5 — Pulse 100ms",
            "Pin 5 — Pulse 200ms"
        })
        grpLaci.Controls.Add(cmbCode)

        ' Port Serial — hanya untuk laci yang colok langsung ke COM port (bukan via printer)
        Dim lblPortSerial As New Label() With {
            .Name = "lblPortSerial_" & key,
            .Text = "Port COM (opsional) :",
            .Font = New Font("Segoe UI", 9),
            .AutoSize = False,
            .Size = New Size(TextRenderer.MeasureText("Port COM (opsional) :", New Font("Segoe UI", 9)).Width + 6, 26),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(xC + 200, 14)
        }
        grpLaci.Controls.Add(lblPortSerial)
        Dim cmbPort As New ComboBox() With {
            .Name = "cmbPortCash_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 360, 14),
            .Size = New Size(90, 26)
        }
        grpLaci.Controls.Add(cmbPort)

        Dim btnTes As New Button() With {
            .Name = "btnTesCash_" & key,
            .Text = "Tes Buka Laci",
            .Font = New Font("Segoe UI", 8, FontStyle.Bold),
            .BackColor = Color.Goldenrod,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Size = New Size(120, 26),
            .Location = New Point(xC + 460, 14)
        }
        AddHandler btnTes.Click, AddressOf BtnTesCash_Click
        grpLaci.Controls.Add(btnTes)
        grpLaci.Controls.Add(New Label() With {
            .Text = "Port COM hanya diisi jika laci colok langsung ke komputer (bukan via printer)",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Regular),
            .ForeColor = Color.Gray,
            .AutoSize = True,
            .Location = New Point(xC + 200, 44)
        })
        grp.Controls.Add(grpLaci)
        y += 78

        ' ── 4. FONT (kiri) + FOOTER (kanan) ─────────────────────
        Dim grpFont As New GroupBox() With {
            .Text = "Pengaturan Font Struk  (hanya untuk mode GDI+)",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(100, 60, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(620, 191)
        }
        Dim yF As Integer = 18
        Const xLF As Integer = 8
        Const xCF As Integer = 170
        AddLbl(grpFont, "Bagian", xLF, yF, Color.Gray, 8, True)
        AddLbl(grpFont, "Nama Font", xCF, yF, Color.Gray, 8, True)
        AddLbl(grpFont, "Ukuran", xCF + 290, yF, Color.Gray, 8, True)
        yF += 30
        AddBarisFont(grpFont, "Nama Toko / Header :", key, "Thermal", "FontJudul", xLF, yF, xCF) : yF += 26
        AddBarisFont(grpFont, "Keterangan / Sub Header :", key, "Thermal", "FontKeterangan", xLF, yF, xCF) : yF += 26
        AddBarisFont(grpFont, "Isi Nota / Item :", key, "Thermal", "FontIsi", xLF, yF, xCF) : yF += 26
        AddBarisFont(grpFont, "Footer :", key, "Thermal", "FontFooter", xLF, yF, xCF)
        grp.Controls.Add(grpFont)

        Dim grpFontEsc As New GroupBox() With {
            .Name = "grpFontEsc_" & key,
            .Text = "Pengaturan Ukuran Font Struk (hanya untuk mode ESC/POS)",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(0, 80, 150),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(620, 191)
        }
        Dim yFEsc As Integer = 18
        AddLbl(grpFontEsc, "Bagian", xLF, yFEsc, Color.Gray, 8, True)
        AddLbl(grpFontEsc, "Ukuran Text", xCF, yFEsc, Color.Gray, 8, True)
        yFEsc += 30
        AddBarisFontEsc(grpFontEsc, "Nama Toko / Header :", key, "Thermal", "EscUkuranJudul", xLF, yFEsc, xCF) : yFEsc += 26
        AddBarisFontEsc(grpFontEsc, "Keterangan / Sub Header :", key, "Thermal", "EscUkuranKeterangan", xLF, yFEsc, xCF) : yFEsc += 26
        AddBarisFontEsc(grpFontEsc, "Isi Nota / Item :", key, "Thermal", "EscUkuranIsi", xLF, yFEsc, xCF) : yFEsc += 26
        AddBarisFontEsc(grpFontEsc, "Footer :", key, "Thermal", "EscUkuranFooter", xLF, yFEsc, xCF)
        grp.Controls.Add(grpFontEsc)

        Dim grpFooterThermal As New GroupBox() With {
            .Name = "grpFooterThermal_" & key,
            .Text = "Tampilkan Footer & Logo",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(100, 60, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL + 630, y),
            .Size = New Size(lebarPanel - 630, 191)
        }
        Dim yChkF As Integer = 20
        Dim chkLogo As New CheckBox() With {
            .Name = "chkTampilLogoThermal_" & key,
            .Text = "Tampilkan Logo",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(100, 60, 0),
            .Location = New Point(10, yChkF),
            .AutoSize = True, .Checked = True
        }
        grpFooterThermal.Controls.Add(chkLogo)
        yChkF += 30
        Dim chkF1 As New CheckBox() With {
            .Name = "chkFooterT1_" & key,
            .Text = "Footer 1 : " & TeksFooter(FOOTER1),
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(10, yChkF),
            .AutoSize = True, .Checked = True
        }
        Dim chkF2 As New CheckBox() With {
            .Name = "chkFooterT2_" & key,
            .Text = "Footer 2 : " & TeksFooter(FOOTER2),
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(10, yChkF + 30),
            .AutoSize = True, .Checked = True
        }
        Dim chkF3 As New CheckBox() With {
            .Name = "chkFooterT3_" & key,
            .Text = "Footer 3 : " & TeksFooter(FOOTER3),
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(10, yChkF + 60),
            .AutoSize = True, .Checked = True
        }
        grpFooterThermal.Controls.AddRange(New Control() {chkF1, chkF2, chkF3})
        grp.Controls.Add(grpFooterThermal)

        ' Event: saat nama printer berubah, update visibilitas checkbox logo
        AddHandler cmbP.SelectedIndexChanged,
            Sub(s2, ev2)
                Dim selectedPrinter As String = CType(s2, ComboBox).Text
                Dim chkL As CheckBox = CariKontrol(Of CheckBox)(grp, "chkTampilLogoThermal_" & key)
                If chkL IsNot Nothing Then
                    Dim cmbModeCtrl2 As ComboBox = CariKontrol(Of ComboBox)(grp, "cmbModeCetak_" & key)
                    Dim modeCetak2 As String = If(cmbModeCtrl2 IsNot Nothing, cmbModeCtrl2.Text, "ESC/POS (Raw)")
                    Dim bisaLogo As Boolean = LogoBisaDicetak("Printer Thermal", modeCetak2, selectedPrinter)
                    chkL.Enabled = bisaLogo
                    If Not bisaLogo Then chkL.Checked = False
                End If
            End Sub

        ' ── Event Mode Cetak ─────────────────────────────────────
        AddHandler cmbMode.SelectedIndexChanged,
            Sub(s, ev)
                Dim isEscPos As Boolean = (CType(s, ComboBox).Text = "ESC/POS (Raw)")
                grpFont.Visible = Not isEscPos

                Dim lblJarakCtrl As Label = CariKontrol(Of Label)(grp, "lblJarakBaris_" & key)
                If lblJarakCtrl IsNot Nothing Then
                    lblJarakCtrl.Text = If(isEscPos, "Jarak (baris) :", "Jarak (px) :")
                End If
                Dim txtJarakGdi As Control = CariKontrol(Of Control)(grp, "txtJarakGdi_" & key)
                Dim txtJarakEsc As Control = CariKontrol(Of Control)(grp, "txtJarakEsc_" & key)
                If txtJarakGdi IsNot Nothing Then txtJarakGdi.Visible = Not isEscPos
                If txtJarakEsc IsNot Nothing Then txtJarakEsc.Visible = isEscPos

                ' Potong otomatis — dua checkbox terpisah per mode
                Dim chkPotongEscCtrl As Control = CariKontrol(Of Control)(grp, "chkPotongEsc_" & key)
                Dim chkPotongGdiCtrl As Control = CariKontrol(Of Control)(grp, "chkPotongGdi_" & key)
                If chkPotongEscCtrl IsNot Nothing Then chkPotongEscCtrl.Visible = isEscPos
                If chkPotongGdiCtrl IsNot Nothing Then chkPotongGdiCtrl.Visible = Not isEscPos
                ' Jumlah cetak — dua kontrol terpisah per mode
                Dim numCopiesEscCtrl As Control = CariKontrol(Of Control)(grp, "numCopiesEsc_" & key)
                Dim numCopiesGdiCtrl As Control = CariKontrol(Of Control)(grp, "numCopiesGdi_" & key)
                If numCopiesEscCtrl IsNot Nothing Then numCopiesEscCtrl.Visible = isEscPos
                If numCopiesGdiCtrl IsNot Nothing Then numCopiesGdiCtrl.Visible = Not isEscPos

                Dim lblDpiCtrl As Control = CariKontrol(Of Control)(grp, "lblDpiCetak_" & key)
                Dim txtDpiCtrl As Control = CariKontrol(Of Control)(grp, "txtDpiCetak_" & key)
                If lblDpiCtrl IsNot Nothing Then lblDpiCtrl.Visible = Not isEscPos
                If txtDpiCtrl IsNot Nothing Then txtDpiCtrl.Visible = Not isEscPos

                Dim cmbMdl As ComboBox = CariKontrol(Of ComboBox)(grp, "cmbModelStruk_" & key)
                If cmbMdl IsNot Nothing Then
                    Dim nilaiLama As String = If(cmbMdl.SelectedItem IsNot Nothing, cmbMdl.SelectedItem.ToString(), "")
                    IsiModelStruk(cmbMdl, isEscPos, transaksi:=key)
                    Dim idx As Integer = cmbMdl.Items.IndexOf(nilaiLama)
                    cmbMdl.SelectedIndex = If(idx >= 0, idx, 0)
                End If

                Dim kontrolEscPos As String() = {
                    "lblTipeKoneksi_" & key, "cmbTipeKoneksi_" & key
                }
                For Each nama As String In kontrolEscPos
                    Dim ctrl As Control = CariKontrol(Of Control)(grp, nama)
                    If ctrl IsNot Nothing Then ctrl.Visible = isEscPos
                Next

                Dim grpLaciCtrl As GroupBox = CariKontrol(Of GroupBox)(grp, "grpLaci_" & key)
                ' Laci kasir tampil jika transaksi relevan — tidak peduli mode ESC/POS atau GDI+
                ' karena GDI+ sudah support buka laci via RawPrinterHelper
                If grpLaciCtrl IsNot Nothing Then grpLaciCtrl.Visible = TransaksiButuhLaci(key)

                Dim cmbKon As ComboBox = CariKontrol(Of ComboBox)(grp, "cmbTipeKoneksi_" & key)
                If cmbKon IsNot Nothing Then
                    If Not isEscPos Then
                        cmbKon.Text = "USB / Windows Spooler"
                        cmbKon.Enabled = False
                    Else
                        cmbKon.Enabled = True
                    End If
                End If
            End Sub

        ' Visibilitas awal (default ESC/POS)
        grpFont.Visible = False
        grpLaci.Visible = TransaksiButuhLaci(key)
        Dim lblJarakInit As Label = CariKontrol(Of Label)(grp, "lblJarakBaris_" & key)
        If lblJarakInit IsNot Nothing Then lblJarakInit.Text = "Jarak (baris) :"
        Dim txtJarakGdiInit As Control = CariKontrol(Of Control)(grp, "txtJarakGdi_" & key)
        Dim txtJarakEscInit As Control = CariKontrol(Of Control)(grp, "txtJarakEsc_" & key)
        If txtJarakGdiInit IsNot Nothing Then txtJarakGdiInit.Visible = False
        If txtJarakEscInit IsNot Nothing Then txtJarakEscInit.Visible = True
        Dim chkPotongEscInit As Control = CariKontrol(Of Control)(grp, "chkPotongEsc_" & key)
        Dim chkPotongGdiInit As Control = CariKontrol(Of Control)(grp, "chkPotongGdi_" & key)
        If chkPotongEscInit IsNot Nothing Then chkPotongEscInit.Visible = True
        If chkPotongGdiInit IsNot Nothing Then chkPotongGdiInit.Visible = False
        Dim numCopiesEscInit As Control = CariKontrol(Of Control)(grp, "numCopiesEsc_" & key)
        Dim numCopiesGdiInit As Control = CariKontrol(Of Control)(grp, "numCopiesGdi_" & key)
        If numCopiesEscInit IsNot Nothing Then numCopiesEscInit.Visible = True
        If numCopiesGdiInit IsNot Nothing Then numCopiesGdiInit.Visible = False
        Dim lblDpiInit As Control = CariKontrol(Of Control)(grp, "lblDpiCetak_" & key)
        Dim txtDpiInit As Control = CariKontrol(Of Control)(grp, "txtDpiCetak_" & key)
        If lblDpiInit IsNot Nothing Then lblDpiInit.Visible = False
        If txtDpiInit IsNot Nothing Then txtDpiInit.Visible = False

        Return grp
    End Function

#End Region

#Region "Panel Monitor & PDF"

    Private Function BuatPanelFooterSaja(key As String, suffix As String, judul As String,
                                          bgColor As Color, fgColor As Color) As GroupBox
        Dim grp As New GroupBox() With {
            .Name = "grp" & suffix & "_" & key,
            .Text = "  Pengaturan " & judul,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .BackColor = bgColor,
            .ForeColor = fgColor
        }

        ' suffix = "Monitor" -> chkFooterM1/M2/M3, "PDF" -> chkFooterP1/P2/P3
        Dim pfx As String = If(suffix = "Monitor", "M", "P")

        Dim xL As Integer = 14
        grp.Controls.Add(New Label() With {
            .Text = "Pilih footer yang akan ditampilkan :",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = fgColor,
            .AutoSize = False,
            .Size = New Size(400, 26),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(xL, 28)
        })
        Dim yChk As Integer = 52
        Dim chkF1 As New CheckBox() With {
            .Name = "chkFooter" & pfx & "1_" & key,
            .Text = "Footer 1 : " & TeksFooter(FOOTER1),
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xL, yChk),
            .AutoSize = True, .Checked = True
        }
        grp.Controls.Add(chkF1)
        yChk += 36
        Dim chkF2 As New CheckBox() With {
            .Name = "chkFooter" & pfx & "2_" & key,
            .Text = "Footer 2 : " & TeksFooter(FOOTER2),
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xL, yChk),
            .AutoSize = True, .Checked = True
        }
        grp.Controls.Add(chkF2)
        yChk += 36
        Dim chkF3 As New CheckBox() With {
            .Name = "chkFooter" & pfx & "3_" & key,
            .Text = "Footer 3 : " & TeksFooter(FOOTER3),
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xL, yChk),
            .AutoSize = True, .Checked = True
        }
        grp.Controls.Add(chkF3)

        Return grp
    End Function

#End Region

#Region "Panel Dot Matrix"

    Private Function BuatPanelDot(key As String) As GroupBox
        Dim grp As New GroupBox() With {
            .Name = "grpDot_" & key,
            .Text = "  Pengaturan Printer Dot Matrix",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .BackColor = Color.FromArgb(240, 255, 240),
            .ForeColor = Color.FromArgb(0, 100, 0)
        }

        Dim y As Integer = 24 : Dim xL As Integer = 14 : Dim xC As Integer = 210

        AddLbl(grp, "Printer Dot Matrix :", xL, y)
        Dim cmbP As New ComboBox() With {
            .Name = "cmbPrinterDot_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC, y),
            .Size = New Size(320, 26)
        }
        grp.Controls.Add(cmbP)
        Dim lblPrinterAktifD As Label = MakeLbl("lblPrinterAktif_" & key & "_Dot", "", xC + 330, y, Color.DarkGreen, 8)
        lblPrinterAktifD.AutoSize = True
        grp.Controls.Add(lblPrinterAktifD)

        y += 34
        AddLbl(grp, "Mode Cetak :", xL, y)
        Dim cmbModeDot As New ComboBox() With {
            .Name = "cmbModeDot_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .Location = New Point(xC, y),
            .Size = New Size(200, 26)
        }
        cmbModeDot.Items.AddRange(New Object() {"GDI+ (Windows Print)", "ESC/P (Raw)"})
        cmbModeDot.SelectedIndex = 0
        grp.Controls.Add(cmbModeDot)
        grp.Controls.Add(New Label() With {
            .Text = "GDI+ (Windows Print) = cetak via driver Windows, mendukung font & grafis  |  ESC/P (Raw) = perintah langsung ke printer, butuh driver Generic Text Only",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Regular),
            .ForeColor = Color.Gray,
            .AutoSize = False,
            .Size = New Size(900, 20),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(xL, y + 30)
        })

        y += 56
        AddSep(grp, xL, y, 1020) : y += 8

        ' ── Sub-panel GDI+ ───────────────────────────────────
        Dim grpGdi As New GroupBox() With {
            .Name = "grpDotGdi_" & key,
            .Text = "  GDI+ (Windows Print) — Continuous Form via driver Windows",
            .Font = New Font("Segoe UI", 8, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 80, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(1020, 298)
        }
        Dim yG As Integer = 18 : Dim xCG As Integer = 200
        AddLbl(grpGdi, "Lebar Kertas (karakter) :", xL, yG)
        AddTxt(grpGdi, "txtLebarDotGdi_" & key, xCG, yG, 55)
        AddLbl(grpGdi, "27=9pin | 40=wide | 80=wide", xCG + 63, yG, Color.Gray, 8)
        AddLbl(grpGdi, "Batas Kiri :", xCG + 310, yG)
        AddTxt(grpGdi, "txtBatasKiriDotGdi_" & key, xCG + 390, yG, 50)
        AddLbl(grpGdi, "Jarak Baris (px) :", xCG + 450, yG)
        AddTxt(grpGdi, "txtJarakDotGdi_" & key, xCG + 560, yG, 50)
        AddLbl(grpGdi, "Ukuran Font :", xCG + 620, yG)
        AddTxt(grpGdi, "txtUkuranFontDotGdi_" & key, xCG + 705, yG, 45)
        AddLbl(grpGdi, "pt", xCG + 755, yG, Color.Gray, 8)

        yG += 32
        AddLbl(grpGdi, "Ukuran Kertas :", xL, yG)
        Dim cmbKertasGdi As New ComboBox() With {
            .Name = "cmbKertasDotGdi_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCG, yG),
            .Size = New Size(220, 26)
        }
        cmbKertasGdi.Items.AddRange(New Object() {
            "Continuous Form (Auto)",
            "9.5 x 11 inch (Continuous)",
            "9.5 x 12 inch (Continuous)",
            "14.875 x 11 inch (Wide)",
            "A4 (210 x 297 mm)",
            "Letter (8.5 x 11 inch)"})
        cmbKertasGdi.SelectedIndex = 0
        grpGdi.Controls.Add(cmbKertasGdi)
        grpGdi.Controls.Add(New Label() With {
            .Text = "Continuous Form = tinggi otomatis sesuai isi nota (direkomendasikan untuk dot matrix)",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Regular),
            .ForeColor = Color.FromArgb(0, 80, 0),
            .AutoSize = False,
            .Size = New Size(700, 20),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(xL, yG + 30)
        })
        yG += 56
        AddLbl(grpGdi, "Jumlah Cetak :", xL, yG)
        Dim numGdi As New NumericUpDown() With {
            .Name = "numCopiesDotGdi_" & key,
            .Minimum = 1, .Maximum = 5, .Value = 1,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCG, yG),
            .Size = New Size(55, 26)
        }
        grpGdi.Controls.Add(numGdi)
        AddLbl(grpGdi, "Model Struk :", xCG + 65, yG)
        Dim cmbModelGdi As New ComboBox() With {
            .Name = "cmbModelDotGdi_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCG + 145, yG),
            .Size = New Size(240, 26)
        }
        IsiModelDot(cmbModelGdi, key)
        grpGdi.Controls.Add(cmbModelGdi)
        yG += 36
        AddSep(grpGdi, xL, yG, 990) : yG += 8
        TambahCheckboxFooterDotMode(grpGdi, key, "Gdi", yG)
        grp.Controls.Add(grpGdi)
        Dim grpEsc As New GroupBox() With {
            .Name = "grpDotEsc_" & key,
            .Text = "  ESC/P (Raw) — Continuous Form via Generic Text Only driver",
            .Font = New Font("Segoe UI", 8, FontStyle.Bold),
            .ForeColor = Color.FromArgb(140, 60, 0),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, y),
            .Size = New Size(1020, 258),
            .Visible = False
        }
        Dim yE As Integer = 18 : Dim xCE As Integer = 200
        AddLbl(grpEsc, "Lebar Kertas (karakter) :", xL, yE)
        AddTxt(grpEsc, "txtLebarDotEsc_" & key, xCE, yE, 55)
        AddLbl(grpEsc, "27=9pin | 40=wide | 80=wide", xCE + 63, yE, Color.Gray, 8)
        AddLbl(grpEsc, "Batas Kiri :", xCE + 310, yE)
        AddTxt(grpEsc, "txtBatasKiriDotEsc_" & key, xCE + 390, yE, 50)
        AddLbl(grpEsc, "Jarak Baris (baris) :", xCE + 450, yE)
        AddTxt(grpEsc, "txtJarakDotEsc_" & key, xCE + 570, yE, 50)
        yE += 32
        AddLbl(grpEsc, "Jumlah Cetak :", xL, yE)
        Dim numEsc As New NumericUpDown() With {
            .Name = "numCopiesDotEsc_" & key,
            .Minimum = 1, .Maximum = 5, .Value = 1,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCE, yE),
            .Size = New Size(55, 26)
        }
        grpEsc.Controls.Add(numEsc)
        AddLbl(grpEsc, "Model Struk :", xCE + 65, yE)
        Dim cmbModelEsc As New ComboBox() With {
            .Name = "cmbModelDotEsc_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCE + 145, yE),
            .Size = New Size(240, 26)
        }
        IsiModelDot(cmbModelEsc, key)
        grpEsc.Controls.Add(cmbModelEsc)
        AddLbl(grpEsc, "⚠ Driver: Generic Text Only / passthrough",
               xCE + 400, yE, Color.FromArgb(140, 60, 0), 8)
        yE += 36

        Dim grpFontEscDot As New GroupBox() With {
            .Name = "grpFontEscDot_" & key,
            .Text = "Pengaturan Ukuran Font (ESC/P)",
            .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.FromArgb(0, 80, 150),
            .BackColor = Color.Transparent,
            .Location = New Point(xL, yE),
            .Size = New Size(500, 150)
        }
        Dim yFD As Integer = 18
        AddLbl(grpFontEscDot, "Bagian", xL, yFD, Color.Gray, 8, True)
        AddLbl(grpFontEscDot, "Ukuran (ESC W 1)", 180, yFD, Color.Gray, 8, True)
        yFD += 30
        AddBarisFontEsc(grpFontEscDot, "Judul & Keterangan :", key, "DotEsc", "EscUkuranJudul", xL, yFD, 180) : yFD += 26
        AddBarisFontEsc(grpFontEscDot, "Isi Nota / Item :", key, "DotEsc", "EscUkuranIsi", xL, yFD, 180) : yFD += 26
        AddBarisFontEsc(grpFontEscDot, "Footer :", key, "DotEsc", "EscUkuranFooter", xL, yFD, 180)
        grpEsc.Controls.Add(grpFontEscDot)

        yE += 160
        AddSep(grpEsc, xL, yE, 990) : yE += 8
        TambahCheckboxFooterDotMode(grpEsc, key, "Esc", yE)
        grp.Controls.Add(grpEsc)

        AddHandler cmbModeDot.SelectedIndexChanged,
            Sub(s, ev)
                Dim isGdi As Boolean = (CType(s, ComboBox).Text = "GDI+ (Windows Print)")
                grpGdi.Visible = isGdi
                grpEsc.Visible = Not isGdi
            End Sub

        Return grp
    End Function

    Private Sub TambahCheckboxFooterDotMode(parent As Control, key As String,
                                             mode As String, y As Integer)
        AddLbl(parent, "Tampilkan Footer", 14, y + 6, Color.Gray, 8, True)
        y += 36
        parent.Controls.Add(New CheckBox() With {
            .Name = "chkF1Dot" & mode & "_" & key,
            .Text = "Footer 1 : " & TeksFooter(FOOTER1),
            .Font = New Font("Segoe UI", 8),
            .Location = New Point(14, y),
            .AutoSize = True, .Checked = True})
        parent.Controls.Add(New CheckBox() With {
            .Name = "chkF2Dot" & mode & "_" & key,
            .Text = "Footer 2 : " & TeksFooter(FOOTER2),
            .Font = New Font("Segoe UI", 8),
            .Location = New Point(14, y + 36),
            .AutoSize = True, .Checked = True})
        parent.Controls.Add(New CheckBox() With {
            .Name = "chkF3Dot" & mode & "_" & key,
            .Text = "Footer 3 : " & TeksFooter(FOOTER3),
            .Font = New Font("Segoe UI", 8),
            .Location = New Point(14, y + 72),
            .AutoSize = True, .Checked = True})
    End Sub

#End Region

#Region "Panel Inkjet / Laser"

    Private Function BuatPanelInk(key As String) As GroupBox
        Dim grp As New GroupBox() With {
            .Name = "grpInk_" & key,
            .Text = "  Pengaturan Printer Inkjet / Laser",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .BackColor = Color.FromArgb(240, 248, 255),
            .ForeColor = Color.FromArgb(0, 60, 130)
        }

        Dim y As Integer = 24 : Dim xL As Integer = 14 : Dim xC As Integer = 210

        AddLbl(grp, "Printer Inkjet / Laser :", xL, y)
        Dim cmbP As New ComboBox() With {
            .Name = "cmbPrinterInk_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC, y),
            .Size = New Size(320, 26)
        }
        grp.Controls.Add(cmbP)
        Dim lblPrinterAktifI As Label = MakeLbl("lblPrinterAktif_" & key & "_Ink", "", xC + 330, y, Color.DarkGreen, 8)
        lblPrinterAktifI.AutoSize = True
        grp.Controls.Add(lblPrinterAktifI)

        y += 34
        AddLbl(grp, "Ukuran Kertas :", xL, y)
        Dim cmbPaper As New ComboBox() With {
            .Name = "cmbPaperSize_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC, y),
            .Size = New Size(140, 26)
        }
        cmbPaper.Items.AddRange(New Object() {"A4", "F4 / Folio", "Letter", "A5"})
        grp.Controls.Add(cmbPaper)

        AddLbl(grp, "Orientasi :", xC + 150, y)
        Dim cmbOrient As New ComboBox() With {
            .Name = "cmbOrientasi_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 220, y),
            .Size = New Size(130, 26)
        }
        cmbOrient.Items.AddRange(New Object() {"Portrait", "Landscape"})
        grp.Controls.Add(cmbOrient)

        AddLbl(grp, "Jumlah Cetak :", xC + 360, y)
        Dim numCopies As New NumericUpDown() With {
            .Name = "numCopiesInk_" & key,
            .Minimum = 1, .Maximum = 10, .Value = 1,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 460, y),
            .Size = New Size(55, 26)
        }
        grp.Controls.Add(numCopies)

        y += 32
        AddLbl(grp, "Margin (mm) :", xL, y)
        AddLbl(grp, "Atas :", xC, y)
        AddTxt(grp, "txtMarginAtas_" & key, xC + 50, y, 50)
        AddLbl(grp, "Bawah :", xC + 110, y)
        AddTxt(grp, "txtMarginBawah_" & key, xC + 172, y, 50)
        AddLbl(grp, "Kiri :", xC + 232, y)
        AddTxt(grp, "txtMarginKiri_" & key, xC + 276, y, 50)
        AddLbl(grp, "Kanan :", xC + 336, y)
        AddTxt(grp, "txtMarginKanan_" & key, xC + 396, y, 50)

        y += 40
        AddSep(grp, xL, y, 1020) : y += 10
        AddLbl(grp, "Pengaturan Font", xL, y, Color.FromArgb(0, 60, 130), 9, True)
        y += 22
        Dim yFontStart As Integer = y
        AddHeaderFont(grp, xL, y, xC) : y += 30
        AddBarisFont(grp, "Font Judul / Header :", key, "Ink", "FontJudul", xL, y, xC) : y += 28
        AddBarisFont(grp, "Font Isi Laporan :", key, "Ink", "FontIsi", xL, y, xC)
        TambahCheckboxFooter(grp, key, yFontStart)

        y += 80
        AddSep(grp, xL, y, 1020) : y += 10
        AddLbl(grp, "Layout Nota", xL, y, Color.FromArgb(0, 60, 130), 9, True)
        y += 22

        AddLbl(grp, "Model Nota :", xL, y)
        Dim cmbModel As New ComboBox() With {
            .Name = "cmbModelNotaInk_" & key,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC, y),
            .Size = New Size(200, 26)
        }
        IsiModelNota(cmbModel, key)
        grp.Controls.Add(cmbModel)

        Dim chkLogo As New CheckBox() With {
            .Name = "chkTampilLogoInk_" & key,
            .Text = "Tampilkan Logo",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 220, y + 2),
            .AutoSize = True, .Checked = True
        }
        grp.Controls.Add(chkLogo)

        Dim chkTtd As New CheckBox() With {
            .Name = "chkTampilTtdInk_" & key,
            .Text = "Tampilkan Tanda Tangan",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xC + 360, y + 2),
            .AutoSize = True, .Checked = True
        }
        grp.Controls.Add(chkTtd)

        y += 32
        AddLbl(grp, "Lebar Kolom (%) :", xL, y)
        AddLbl(grp, "No :", xC, y)
        AddTxt(grp, "txtPctKolomNo_" & key, xC + 43, y, 45)
        AddLbl(grp, "Qty :", xC + 98, y)
        AddTxt(grp, "txtPctKolomQty_" & key, xC + 144, y, 45)
        AddLbl(grp, "Harga :", xC + 199, y)
        AddTxt(grp, "txtPctKolomHarga_" & key, xC + 258, y, 45)
        AddLbl(grp, "Diskon :", xC + 313, y)
        AddTxt(grp, "txtPctKolomDiskon_" & key, xC + 376, y, 45)
        AddLbl(grp, "(Kolom Nama = sisa, Jumlah = sama dengan Harga)",
               xC + 431, y, Color.Gray, 8)

        grp.Size = New Size(1060, y + 60)
        Return grp
    End Function

#End Region

#Region "Helper UI"

    ' Transaksi yang melibatkan uang tunai — butuh kontrol laci kasir
    Private Function TransaksiButuhLaci(key As String) As Boolean
        Select Case key
            Case "Jual", "Beli", "ReturJual", "ReturBeli",
                 "BayarHutang", "BayarPiutang", "Bon", "Laporan"
                Return True
            Case Else
                Return False
        End Select
    End Function

    ' ================================================================
    ' IsiModelStruk — isi daftar model struk sesuai transaksi & mode
    '
    ' Logo ditentukan dari checkbox TampilLogo, bukan dari nama model.
    ' Sales, Transfer, Persen tampil otomatis jika ada data.
    ' Penjualan: 8 kombinasi dari 3 dimensi independen:
    '   Header Kolom (ada/tidak) × Kolom Diskon (ada/tidak) × Hutang (ada/tidak)
    ' ================================================================
    Private Sub IsiModelStruk(cmb As ComboBox, isEscPos As Boolean,
                               Optional transaksi As String = "")
        cmb.Items.Clear()
        Select Case transaksi
            Case "Jual"
                cmb.Items.Add("Model 1 — Judul Kolom, Diskon, Sisa Hutang")
                cmb.Items.Add("Model 2 — Judul Kolom, Diskon")
                cmb.Items.Add("Model 3 — Judul Kolom, Sisa Hutang")
                cmb.Items.Add("Model 4 — Judul Kolom")
                cmb.Items.Add("Model 5 — Diskon, Sisa Hutang")
                cmb.Items.Add("Model 6 — Diskon")
                cmb.Items.Add("Model 7 — Sisa Hutang")
                cmb.Items.Add("Model 8 — Ringkas")
            Case "Beli"
                cmb.Items.Add("Model 1 Lengkap")
                cmb.Items.Add("Model 2 Tanpa Header")
                cmb.Items.Add("Model 3 Dengan Total Hutang")
            Case "ReturJual"
                cmb.Items.Add("Model 1 Lengkap")
                cmb.Items.Add("Model 2 Tanpa Diskon")
                cmb.Items.Add("Model 3 Tanpa Header")
            Case "ReturBeli"
                cmb.Items.Add("Model 1 Lengkap")
                cmb.Items.Add("Model 2 Tanpa Header")
            Case Else
                cmb.Items.Add("Model 1 Lengkap")
        End Select
        If cmb.Items.Count > 0 Then cmb.SelectedIndex = 0
    End Sub

    ' ================================================================
    ' IsiModelDot — model struk untuk panel Dot Matrix (GDI & ESC/P)
    '   Jual : 6 model khusus dot matrix
    '   Lain : 1 model saja
    ' ================================================================
    Private Sub IsiModelDot(cmb As ComboBox, transaksi As String)
        cmb.Items.Clear()
        Select Case transaksi
            Case "Jual"
                cmb.Items.Add("Model 1 Lengkap")
                cmb.Items.Add("Model 2 Tanpa Diskon")
                cmb.Items.Add("Model 3 Dengan Sales")
                cmb.Items.Add("Model 4 Dengan Transfer")
                cmb.Items.Add("Model 5 Dengan Hutang")
                cmb.Items.Add("Model 6 Dengan Pemisah")
            Case Else
                cmb.Items.Add("Model 1 Lengkap")
        End Select
        If cmb.Items.Count > 0 Then cmb.SelectedIndex = 0
    End Sub

    ' ================================================================
    ' IsiModelNota — model nota untuk panel Inkjet/Laser
    '   Jual, ReturJual : Model 1 Lengkap, Model 2 Tanpa Diskon
    '   Beli            : Model 1 Lengkap, Model 2 Dengan Total Hutang
    '   Lain            : Model 1 Lengkap saja
    ' ================================================================
    Private Sub IsiModelNota(cmb As ComboBox, transaksi As String)
        cmb.Items.Clear()
        Select Case transaksi
            Case "Jual", "ReturJual"
                cmb.Items.Add("Model 1 Lengkap")
                cmb.Items.Add("Model 2 Tanpa Diskon")
            Case "Beli"
                cmb.Items.Add("Model 1 Lengkap")
                cmb.Items.Add("Model 2 Dengan Total Hutang")
            Case Else
                cmb.Items.Add("Model 1 Lengkap")
        End Select
        If cmb.Items.Count > 0 Then cmb.SelectedIndex = 0
    End Sub

    Private Sub PilihItemPertamaBukanSeparator(cmb As ComboBox)
        For i As Integer = 0 To cmb.Items.Count - 1
            If Not cmb.Items(i).ToString().StartsWith("──") Then
                cmb.SelectedIndex = i : Exit For
            End If
        Next
    End Sub

    Private Sub PasangHandlerSeparatorModel(cmb As ComboBox)
        AddHandler cmb.SelectedIndexChanged,
            Sub(s, ev)
                Dim c As ComboBox = CType(s, ComboBox)
                If c.SelectedItem IsNot Nothing AndAlso
                   c.SelectedItem.ToString().StartsWith("──") Then
                    Dim next_ As Integer = c.SelectedIndex + 1
                    If next_ < c.Items.Count Then c.SelectedIndex = next_
                End If
            End Sub
    End Sub

    Private Sub AddLbl(parent As Control, text As String, x As Integer, y As Integer,
                        Optional color As Color = Nothing,
                        Optional size As Single = 9,
                        Optional bold As Boolean = False)
        Dim fs As FontStyle = If(bold, FontStyle.Bold, FontStyle.Regular)
        Dim fore As Color = If(color = Nothing OrElse color = Color.Empty,
                               ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240)),
                               color)
        Dim lbl As New Label() With {
            .Text = text,
            .Font = New Font("Segoe UI", size, fs),
            .ForeColor = fore,
            .AutoSize = False,
            .Size = New Size(TextRenderer.MeasureText(text, New Font("Segoe UI", size, fs)).Width + 6, 26),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(x, y)
        }
        parent.Controls.Add(lbl)
    End Sub

    ' Buat label inline yang selalu sejajar tengah dengan combobox/textbox (H=26, MiddleLeft)
    Private Function MakeLbl(name As String, text As String, x As Integer, y As Integer,
                              Optional color As Color = Nothing,
                              Optional size As Single = 9,
                              Optional bold As Boolean = False) As Label
        Dim fs As FontStyle = If(bold, FontStyle.Bold, FontStyle.Regular)
        Dim fore As Color = If(color = Nothing OrElse color = Color.Empty,
                               ModuleTheme.C(Color.Black, Color.FromArgb(226, 232, 240)),
                               color)
        Return New Label() With {
            .Name = name,
            .Text = text,
            .Font = New Font("Segoe UI", size, fs),
            .ForeColor = fore,
            .AutoSize = False,
            .Size = New Size(TextRenderer.MeasureText(text, New Font("Segoe UI", size, fs)).Width + 6, 26),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Location = New Point(x, y)
        }
    End Function

    Private Sub AddTxt(parent As Control, name As String, x As Integer, y As Integer, w As Integer)
        Dim txt As New TextBox() With {
            .Name = name,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(x, y),
            .Size = New Size(w, 26)
        }
        parent.Controls.Add(txt)
    End Sub

    Private Sub AddSep(parent As Control, x As Integer, y As Integer, w As Integer)
        Dim sep As New Label() With {
            .BorderStyle = BorderStyle.Fixed3D,
            .Height = 2,
            .Location = New Point(x, y),
            .Size = New Size(w, 2)
        }
        parent.Controls.Add(sep)
    End Sub

    Private Sub AddHeaderFont(parent As Control, x As Integer, y As Integer, xCmb As Integer)
        AddLbl(parent, "Bagian", x, y, Color.Gray, 8, True)
        AddLbl(parent, "Nama Font", xCmb, y, Color.Gray, 8, True)
        AddLbl(parent, "Ukuran", xCmb + 290, y, Color.Gray, 8, True)
    End Sub

    Private Sub AddBarisFont(parent As Control, labelText As String, key As String,
                              suffix As String, bagian As String, x As Integer, y As Integer,
                              Optional xCmb As Integer = -1)
        If xCmb < 0 Then xCmb = x + 170
        AddLbl(parent, labelText, x, y)
        Dim cmbFont As New ComboBox() With {
            .Name = "cmb" & bagian & "_" & key & "_" & suffix,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCmb, y),
            .Size = New Size(280, 26)
        }
        parent.Controls.Add(cmbFont)

        Dim cmbUkuran As New ComboBox() With {
            .Name = "cmbUkuran" & bagian.Replace("Font", "") & "_" & key & "_" & suffix,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCmb + 290, y),
            .Size = New Size(70, 26)
        }
        parent.Controls.Add(cmbUkuran)
    End Sub

    Private Sub AddBarisFontEsc(parent As Control, labelText As String, key As String,
                                 suffix As String, bagian As String, x As Integer, y As Integer,
                                 Optional xCmb As Integer = -1)
        If xCmb < 0 Then xCmb = x + 170
        AddLbl(parent, labelText, x, y)

        Dim cmbUkuran As New ComboBox() With {
            .Name = "cmb" & bagian & "_" & key & "_" & suffix,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(xCmb, y),
            .Size = New Size(150, 26)
        }
        cmbUkuran.Items.AddRange(New Object() {"Normal", "Bold", "Besar (2x)", "Besar + Bold"})
        parent.Controls.Add(cmbUkuran)
    End Sub

    Private Sub TambahCheckboxFooter(parent As Control, key As String, yFont As Integer)
        Const xFooter As Integer = 580
        Dim yChk As Integer = yFont + 28  ' mulai setelah label "Tampilkan Footer" (H=26) + gap 2px

        AddLbl(parent, "Tampilkan Footer", xFooter, yFont, Color.Gray, 8, True)

        parent.Controls.Add(New CheckBox() With {
            .Name = "chkFooterI1_" & key,
            .Text = "Footer 1 : " & TeksFooter(FOOTER1),
            .Font = New Font("Segoe UI", 8),
            .Location = New Point(xFooter, yChk),
            .AutoSize = True, .Checked = True})

        parent.Controls.Add(New CheckBox() With {
            .Name = "chkFooterI2_" & key,
            .Text = "Footer 2 : " & TeksFooter(FOOTER2),
            .Font = New Font("Segoe UI", 8),
            .Location = New Point(xFooter, yChk + 36),
            .AutoSize = True, .Checked = True})

        parent.Controls.Add(New CheckBox() With {
            .Name = "chkFooterI3_" & key,
            .Text = "Footer 3 : " & TeksFooter(FOOTER3),
            .Font = New Font("Segoe UI", 8),
            .Location = New Point(xFooter, yChk + 72),
            .AutoSize = True, .Checked = True})
    End Sub

    ' Strip newline dari teks footer agar tidak wrap dan tumpuk di checkbox
    Private Function TeksFooter(nilai As String) As String
        If String.IsNullOrEmpty(nilai) Then Return ""
        Return nilai.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
    End Function

#End Region

#Region "Isi Daftar Printer, Port, Font"

    Private Function GetPrinterPort(printerName As String) As String
        If String.IsNullOrEmpty(printerName) Then Return ""
        ' Bangun cache sekali untuk semua printer (satu WMI query, bukan per printer)
        If _printerPortCache Is Nothing Then
            _printerPortCache = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Try
                Dim query As New ManagementObjectSearcher("SELECT Name, PortName FROM Win32_Printer")
                For Each obj As ManagementObject In query.Get()
                    Dim n As String = If(obj("Name") IsNot Nothing, obj("Name").ToString(), "")
                    Dim p As String = If(obj("PortName") IsNot Nothing, obj("PortName").ToString(), "")
                    If Not String.IsNullOrEmpty(n) Then _printerPortCache(n) = p
                Next
            Catch
            End Try
        End If
        Dim port As String = ""
        _printerPortCache.TryGetValue(printerName, port)
        Return port
    End Function

    Private Sub IsiDaftarPrinter()
        Dim windowsDefault As String = New PrinterSettings().PrinterName
        For Each p As String In PrinterSettings.InstalledPrinters
            CmbPrinterDefault.Items.Add(p)
        Next
        LblPrinterAktif.Text = "Default Windows saat ini: " & windowsDefault
        For Each tabPage As TabPage In TabTransaksi.TabPages
            If tabPage.Tag?.ToString() = "built" Then
                IsiDaftarPrinterUntukTab(tabPage.Name)
            End If
        Next
    End Sub

    Private Sub IsiDaftarPrinterUntukTab(key As String)
        Dim tabPage As TabPage = TabTransaksi.TabPages(key)
        If tabPage Is Nothing Then Return
        IsiCmbPrinter(tabPage, "cmbPrinterThermal_" & key, "grpThermal_" & key)
        IsiCmbPrinter(tabPage, "cmbPrinterDot_" & key, "grpDot_" & key)
        IsiCmbPrinter(tabPage, "cmbPrinterInk_" & key, "grpInk_" & key)
    End Sub

    Private Sub IsiCmbPrinter(tabPage As TabPage, cmbName As String, grpName As String)
        Dim grp As GroupBox = TryCast(tabPage.Controls(grpName), GroupBox)
        If grp Is Nothing Then Return
        Dim cmb As ComboBox = CariKontrol(Of ComboBox)(grp, cmbName)
        If cmb Is Nothing Then Return
        cmb.Items.Clear()
        For Each p As String In PrinterSettings.InstalledPrinters
            cmb.Items.Add(p)
        Next
    End Sub

    Private Sub IsiDaftarPort()
        Dim ports As String() = SerialPort.GetPortNames()
        For Each tabPage As TabPage In TabTransaksi.TabPages
            If tabPage.Tag?.ToString() = "built" Then
                IsiDaftarPortUntukTab(tabPage.Name)
            End If
        Next
    End Sub

    Private Sub IsiDaftarPortUntukTab(key As String)
        Dim tabPage As TabPage = TabTransaksi.TabPages(key)
        If tabPage Is Nothing Then Return
        Dim ports As String() = SerialPort.GetPortNames()
        Dim grp As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
        If grp Is Nothing Then Return
        Dim cmb As ComboBox = CariKontrol(Of ComboBox)(grp, "cmbPortCash_" & key)
        If cmb Is Nothing Then Return
        cmb.Items.Clear()
        For Each p As String In ports
            cmb.Items.Add(p)
        Next
    End Sub

    Private Sub IsiDaftarFont()
        Dim fontNames As New List(Of String)
        For Each ff As FontFamily In FontFamily.Families
            fontNames.Add(ff.Name)
        Next
        Dim sizes As New List(Of Object)
        For i As Integer = 7 To 24
            sizes.Add(i)
        Next
        For Each tabPage As TabPage In TabTransaksi.TabPages
            If tabPage.Tag?.ToString() = "built" Then
                IsiDaftarFontUntukTab(tabPage.Name)
            End If
        Next
    End Sub

    Private Sub IsiDaftarFontUntukTab(key As String)
        Dim tabPage As TabPage = TabTransaksi.TabPages(key)
        If tabPage Is Nothing Then Return
        Dim fontNames As New List(Of String)
        For Each ff As FontFamily In FontFamily.Families
            fontNames.Add(ff.Name)
        Next
        Dim sizes As New List(Of Object)
        For i As Integer = 7 To 24
            sizes.Add(i)
        Next
        IsiFont(tabPage, key, "Thermal", fontNames, sizes)
        IsiFont(tabPage, key, "Dot", fontNames, sizes)
        IsiFont(tabPage, key, "Ink", fontNames, sizes)
    End Sub

    Private Sub IsiFont(tabPage As TabPage, key As String, suffix As String,
                         fontNames As List(Of String), sizes As List(Of Object))
        Dim grpName As String = "grp" & If(suffix = "Thermal", "Thermal", If(suffix = "Dot", "Dot", "Ink")) & "_" & key
        Dim grp As GroupBox = TryCast(tabPage.Controls(grpName), GroupBox)
        If grp Is Nothing Then Return
        IsiCmbFontRekursif(grp, fontNames, sizes)
    End Sub

    Private Sub IsiCmbFontRekursif(parent As Control, fontNames As List(Of String), sizes As List(Of Object))
        For Each ctrl As Control In parent.Controls
            If TypeOf ctrl Is ComboBox Then
                Dim cmb As ComboBox = CType(ctrl, ComboBox)
                If cmb.Name.StartsWith("cmbFont") Then
                    If cmb.Items.Count = 0 Then cmb.Items.AddRange(fontNames.ToArray())
                ElseIf cmb.Name.StartsWith("cmbUkuran") AndAlso Not cmb.Name.StartsWith("cmbUkuranKertas") Then
                    If cmb.Items.Count = 0 Then cmb.Items.AddRange(sizes.ToArray())
                End If
            ElseIf ctrl.Controls.Count > 0 Then
                IsiCmbFontRekursif(ctrl, fontNames, sizes)
            End If
        Next
    End Sub

#End Region

#Region "Muat Data ke Semua Tab"

    Private Sub MuatSemuaTab()
        If _iniCache Is Nothing Then _iniCache = BacaIni()
        Dim ini = _iniCache

        Dim savedDefault As String = GetVal(ini, "DefaultPrinter", "")
        If String.IsNullOrEmpty(savedDefault) Then savedDefault = New PrinterSettings().PrinterName
        CmbPrinterDefault.Text = savedDefault

        Dim hostname As String = System.Net.Dns.GetHostName()
        LblNamaKomputer.Text = "Nama komputer ini: " & hostname

        Dim savedPeran As String = GetVal(ini, "StatusKomputer", "")
        If String.IsNullOrEmpty(savedPeran) Then savedPeran = AutoSuggestPeran(hostname)
        CmbStatusKomputer.Text = savedPeran

        ' Hanya muat tab yang sudah dibangun tapi belum dimuat (lazy — sisanya dimuat saat diklik)
        For Each tabPage As TabPage In TabTransaksi.TabPages
            If tabPage.Tag?.ToString() = "built" Then
                IsiDaftarPrinterUntukTab(tabPage.Name)
                IsiDaftarPortUntukTab(tabPage.Name)
                IsiDaftarFontUntukTab(tabPage.Name)
                MuatTab(tabPage, ini, tabPage.Name)
                tabPage.Tag = "loaded"
            End If
        Next
    End Sub

    Private Sub MuatTab(tabPage As TabPage, ini As Dictionary(Of String, String), key As String)
        Dim cmbJenis As ComboBox = TryCast(tabPage.Controls("cmbJenisPrinter_" & key), ComboBox)
        If cmbJenis IsNot Nothing Then
            cmbJenis.Text = GetVal(ini, key & "_JenisPrinter", "Printer Thermal")
            TampilkanPanel(tabPage, cmbJenis.Text)
        End If

        Dim grpT As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
        If grpT IsNot Nothing Then
            Dim modeCetak As String = GetVal(ini, key & "_Thermal_ModeCetak", "ESC/POS (Raw)")
            Dim isEscPos As Boolean = (modeCetak = "ESC/POS (Raw)")
            Dim cmbModeCtrl As ComboBox = CariKontrol(Of ComboBox)(grpT, "cmbModeCetak_" & key)
            If cmbModeCtrl IsNot Nothing Then
                cmbModeCtrl.SelectedIndex = If(isEscPos, 0, 1)
            End If
            Dim cmbMdl As ComboBox = CariKontrol(Of ComboBox)(grpT, "cmbModelStruk_" & key)
            If cmbMdl IsNot Nothing Then
                IsiModelStruk(cmbMdl, isEscPos:=isEscPos, transaksi:=key)
            End If
            SetCmb(grpT, "cmbTipeKoneksi_" & key, GetVal(ini, key & "_Thermal_TipeKoneksi", "USB / Windows Spooler"))
            SetCmbPrinter(grpT, "cmbPrinterThermal_" & key, GetVal(ini, key & "_Thermal_NamaPrinter", ""))
            Dim lblPU As Label = CariKontrol(Of Label)(grpT, "lblPortUsb_" & key)
            If lblPU IsNot Nothing Then lblPU.Text = GetPrinterPort(GetVal(ini, key & "_Thermal_NamaPrinter", ""))
            Dim lblAktifT As Label = CariKontrol(Of Label)(grpT, "lblPrinterAktif_" & key & "_Thermal")
            If lblAktifT IsNot Nothing Then
                Dim savedT As String = GetVal(ini, key & "_Thermal_NamaPrinter", "")
                lblAktifT.Text = If(String.IsNullOrEmpty(savedT), "", "Printer aktif: " & savedT)
            End If
            SetTxt(grpT, "txtIpAddress_" & key, GetVal(ini, key & "_Thermal_IpAddress", "192.168.1.50"))
            SetTxt(grpT, "txtNetworkPort_" & key, GetVal(ini, key & "_Thermal_NetworkPort", "9100"))
            SetCmb(grpT, "cmbUkuranKertas_" & key, GetVal(ini, key & "_Thermal_UkuranKertas", "POS-80 (80mm)"))
            SetTxt(grpT, "txtLebar_" & key, GetVal(ini, key & "_Thermal_LebarKertas", "80"))
            SetTxt(grpT, "txtBatasKiri_" & key, GetVal(ini, key & "_Thermal_BatasKiri", "0"))
            SetTxt(grpT, "txtJarakGdi_" & key, GetVal(ini, key & "_Thermal_JarakBaris", "4"))
            SetTxt(grpT, "txtJarakEsc_" & key, GetVal(ini, key & "_Thermal_JarakBarisEsc", "0"))
            SetCmb(grpT, "cmbPortCash_" & key, GetVal(ini, key & "_Thermal_PortLaciKasir", ""))
            SetCmb(grpT, "cmbCodeCash_" & key, GetVal(ini, key & "_Thermal_KodeLaciKasir", "(Tidak Ada)"))
            SetChk(grpT, "chkPotongEsc_" & key, GetVal(ini, key & "_Thermal_PotongOtomatisEsc", "True"))
            SetChk(grpT, "chkPotongGdi_" & key, GetVal(ini, key & "_Thermal_PotongOtomatisGdi", "True"))
            SetNum(grpT, "numCopiesEsc_" & key, GetVal(ini, key & "_Thermal_JumlahCetakEsc", "1"))
            SetNum(grpT, "numCopiesGdi_" & key, GetVal(ini, key & "_Thermal_JumlahCetakGdi", "1"))
            SetCmb(grpT, "cmbModelStruk_" & key, GetVal(ini, key & "_Thermal_ModelStruk", "Model 2 — Judul Kolom, Diskon"))
            SetCmb(grpT, "cmbFontJudul_" & key & "_Thermal", GetVal(ini, key & "_Thermal_FontJudul", "Arial Narrow"))
            SetCmb(grpT, "cmbFontKeterangan_" & key & "_Thermal", GetVal(ini, key & "_Thermal_FontKeterangan", "Arial Narrow"))
            SetCmb(grpT, "cmbFontIsi_" & key & "_Thermal", GetVal(ini, key & "_Thermal_FontIsi", "Arial Narrow"))
            SetCmb(grpT, "cmbFontFooter_" & key & "_Thermal", GetVal(ini, key & "_Thermal_FontFooter", "Arial Narrow"))
            SetCmb(grpT, "cmbUkuranJudul_" & key & "_Thermal", GetVal(ini, key & "_Thermal_UkuranJudul", "12"))
            SetCmb(grpT, "cmbUkuranKeterangan_" & key & "_Thermal", GetVal(ini, key & "_Thermal_UkuranKeterangan", "9"))
            SetCmb(grpT, "cmbUkuranIsi_" & key & "_Thermal", GetVal(ini, key & "_Thermal_UkuranIsi", "8"))
            SetCmb(grpT, "cmbUkuranFooter_" & key & "_Thermal", GetVal(ini, key & "_Thermal_UkuranFooter", "8"))
            SetCmb(grpT, "cmbEscUkuranJudul_" & key & "_Thermal", GetVal(ini, key & "_Thermal_EscUkuranJudul", "Besar (2x)"))
            SetCmb(grpT, "cmbEscUkuranKeterangan_" & key & "_Thermal", GetVal(ini, key & "_Thermal_EscUkuranKeterangan", "Normal"))
            SetCmb(grpT, "cmbEscUkuranIsi_" & key & "_Thermal", GetVal(ini, key & "_Thermal_EscUkuranIsi", "Normal"))
            SetCmb(grpT, "cmbEscUkuranFooter_" & key & "_Thermal", GetVal(ini, key & "_Thermal_EscUkuranFooter", "Normal"))
            SetChk(grpT, "chkFooterT1_" & key, GetVal(ini, key & "_Thermal_TampilFooter1", "True"))
            SetChk(grpT, "chkFooterT2_" & key, GetVal(ini, key & "_Thermal_TampilFooter2", "True"))
            SetChk(grpT, "chkFooterT3_" & key, GetVal(ini, key & "_Thermal_TampilFooter3", "True"))
            SetTxt(grpT, "txtDpiCetak_" & key, GetVal(ini, key & "_Thermal_DpiCetak", "100"))
            ' Baca TampilLogo dan update visibilitas berdasarkan nama printer
            Dim savedTampilLogo As String = GetVal(ini, key & "_Thermal_TampilLogo", "True")
            Dim savedNamaPrinterT As String = GetVal(ini, key & "_Thermal_NamaPrinter", "")
            Dim savedModeCetakT As String = GetVal(ini, key & "_Thermal_ModeCetak", "ESC/POS (Raw)")
            Dim bisaLogoT As Boolean = LogoBisaDicetak("Printer Thermal", savedModeCetakT, savedNamaPrinterT)
            Dim chkLogoT As CheckBox = CariKontrol(Of CheckBox)(grpT, "chkTampilLogoThermal_" & key)
            If chkLogoT IsNot Nothing Then
                chkLogoT.Enabled = bisaLogoT
                chkLogoT.Checked = If(bisaLogoT, savedTampilLogo.ToLower() = "true", False)
            End If
        End If

        Dim grpD As GroupBox = TryCast(tabPage.Controls("grpDot_" & key), GroupBox)
        If grpD IsNot Nothing Then
            SetCmbPrinter(grpD, "cmbPrinterDot_" & key, GetVal(ini, key & "_DotMatrix_NamaPrinter", ""))
            Dim lblAktifD As Label = CariKontrol(Of Label)(grpD, "lblPrinterAktif_" & key & "_Dot")
            If lblAktifD IsNot Nothing Then
                Dim savedD As String = GetVal(ini, key & "_DotMatrix_NamaPrinter", "")
                lblAktifD.Text = If(String.IsNullOrEmpty(savedD), "", "Printer aktif: " & savedD)
            End If
            Dim modeDot As String = GetVal(ini, key & "_DotMatrix_ModeCetak", "GDI+ (Windows Print)")
            Dim cmbModeDotCtrl As ComboBox = CariKontrol(Of ComboBox)(grpD, "cmbModeDot_" & key)
            If cmbModeDotCtrl IsNot Nothing Then
                cmbModeDotCtrl.SelectedIndex = If(modeDot = "GDI+ (Windows Print)", 0, 1)
            End If

            Dim grpGdi As GroupBox = TryCast(grpD.Controls("grpDotGdi_" & key), GroupBox)
            If grpGdi IsNot Nothing Then
                SetTxt(grpGdi, "txtLebarDotGdi_" & key, GetVal(ini, key & "_DotGdi_LebarKertas", "80"))
                SetCmb(grpGdi, "cmbKertasDotGdi_" & key, GetVal(ini, key & "_DotGdi_UkuranKertas", "Continuous Form (Auto)"))
                SetTxt(grpGdi, "txtBatasKiriDotGdi_" & key, GetVal(ini, key & "_DotGdi_BatasKiri", "2"))
                SetTxt(grpGdi, "txtJarakDotGdi_" & key, GetVal(ini, key & "_DotGdi_JarakBaris", "2"))
                SetTxt(grpGdi, "txtUkuranFontDotGdi_" & key, GetVal(ini, key & "_DotGdi_UkuranFont", "9"))
                SetNum(grpGdi, "numCopiesDotGdi_" & key, GetVal(ini, key & "_DotGdi_JumlahCetak", "1"))
                SetCmb(grpGdi, "cmbModelDotGdi_" & key, GetVal(ini, key & "_DotGdi_ModelStruk", "Model 1 Lengkap"))
                SetChk(grpGdi, "chkF1DotGdi_" & key, GetVal(ini, key & "_DotGdi_TampilFooter1", "True"))
                SetChk(grpGdi, "chkF2DotGdi_" & key, GetVal(ini, key & "_DotGdi_TampilFooter2", "True"))
                SetChk(grpGdi, "chkF3DotGdi_" & key, GetVal(ini, key & "_DotGdi_TampilFooter3", "True"))
                grpGdi.Visible = (modeDot = "GDI+ (Windows Print)")
            End If

            Dim grpEsc As GroupBox = TryCast(grpD.Controls("grpDotEsc_" & key), GroupBox)
            If grpEsc IsNot Nothing Then
                SetTxt(grpEsc, "txtLebarDotEsc_" & key, GetVal(ini, key & "_DotEsc_LebarKertas", "80"))
                SetTxt(grpEsc, "txtBatasKiriDotEsc_" & key, GetVal(ini, key & "_DotEsc_BatasKiri", "2"))
                SetTxt(grpEsc, "txtJarakDotEsc_" & key, GetVal(ini, key & "_DotEsc_JarakBaris", "1"))
                SetNum(grpEsc, "numCopiesDotEsc_" & key, GetVal(ini, key & "_DotEsc_JumlahCetak", "1"))
                SetCmb(grpEsc, "cmbModelDotEsc_" & key, GetVal(ini, key & "_DotEsc_ModelStruk", "Model 1 Lengkap"))
                SetCmb(grpEsc, "cmbEscUkuranJudul_" & key & "_DotEsc", GetVal(ini, key & "_DotEsc_EscUkuranJudul", "Besar (2x)"))
                SetCmb(grpEsc, "cmbEscUkuranIsi_" & key & "_DotEsc", GetVal(ini, key & "_DotEsc_EscUkuranIsi", "Normal"))
                SetCmb(grpEsc, "cmbEscUkuranFooter_" & key & "_DotEsc", GetVal(ini, key & "_DotEsc_EscUkuranFooter", "Normal"))
                SetChk(grpEsc, "chkF1DotEsc_" & key, GetVal(ini, key & "_DotEsc_TampilFooter1", "True"))
                SetChk(grpEsc, "chkF2DotEsc_" & key, GetVal(ini, key & "_DotEsc_TampilFooter2", "True"))
                SetChk(grpEsc, "chkF3DotEsc_" & key, GetVal(ini, key & "_DotEsc_TampilFooter3", "True"))
                grpEsc.Visible = (modeDot = "ESC/P (Raw)")
            End If
        End If

        Dim grpI As GroupBox = TryCast(tabPage.Controls("grpInk_" & key), GroupBox)
        If grpI IsNot Nothing Then
            SetCmbPrinter(grpI, "cmbPrinterInk_" & key, GetVal(ini, key & "_Inkjet_NamaPrinter", ""))
            Dim lblAktifI As Label = CariKontrol(Of Label)(grpI, "lblPrinterAktif_" & key & "_Ink")
            If lblAktifI IsNot Nothing Then
                Dim savedI As String = GetVal(ini, key & "_Inkjet_NamaPrinter", "")
                lblAktifI.Text = If(String.IsNullOrEmpty(savedI), "", "Printer aktif: " & savedI)
            End If
            SetCmb(grpI, "cmbPaperSize_" & key, GetVal(ini, key & "_Inkjet_UkuranKertas", "A4"))
            SetCmb(grpI, "cmbOrientasi_" & key, GetVal(ini, key & "_Inkjet_Orientasi", "Portrait"))
            SetNum(grpI, "numCopiesInk_" & key, GetVal(ini, key & "_Inkjet_JumlahCetak", "1"))
            SetTxt(grpI, "txtMarginAtas_" & key, GetVal(ini, key & "_Inkjet_MarginAtas", "10"))
            SetTxt(grpI, "txtMarginBawah_" & key, GetVal(ini, key & "_Inkjet_MarginBawah", "10"))
            SetTxt(grpI, "txtMarginKiri_" & key, GetVal(ini, key & "_Inkjet_MarginKiri", "15"))
            SetTxt(grpI, "txtMarginKanan_" & key, GetVal(ini, key & "_Inkjet_MarginKanan", "10"))
            SetCmb(grpI, "cmbFontJudul_" & key & "_Ink", GetVal(ini, key & "_Inkjet_FontJudul", "Arial"))
            SetCmb(grpI, "cmbFontIsi_" & key & "_Ink", GetVal(ini, key & "_Inkjet_FontIsi", "Arial"))
            SetCmb(grpI, "cmbUkuranJudul_" & key & "_Ink", GetVal(ini, key & "_Inkjet_UkuranJudul", "12"))
            SetCmb(grpI, "cmbUkuranIsi_" & key & "_Ink", GetVal(ini, key & "_Inkjet_UkuranIsi", "10"))
            SetChk(grpI, "chkFooterI1_" & key, GetVal(ini, key & "_Inkjet_TampilFooter1", "True"))
            SetChk(grpI, "chkFooterI2_" & key, GetVal(ini, key & "_Inkjet_TampilFooter2", "True"))
            SetChk(grpI, "chkFooterI3_" & key, GetVal(ini, key & "_Inkjet_TampilFooter3", "True"))
            SetCmb(grpI, "cmbModelNotaInk_" & key, GetVal(ini, key & "_Inkjet_ModelNota", "Model 1 Lengkap"))
            SetChk(grpI, "chkTampilLogoInk_" & key, GetVal(ini, key & "_Inkjet_TampilLogo", "True"))
            SetChk(grpI, "chkTampilTtdInk_" & key, GetVal(ini, key & "_Inkjet_TampilTandaTangan", "True"))
            SetTxt(grpI, "txtPctKolomNo_" & key, GetVal(ini, key & "_Inkjet_PctKolomNo", "5"))
            SetTxt(grpI, "txtPctKolomQty_" & key, GetVal(ini, key & "_Inkjet_PctKolomQty", "8"))
            SetTxt(grpI, "txtPctKolomHarga_" & key, GetVal(ini, key & "_Inkjet_PctKolomHarga", "15"))
            SetTxt(grpI, "txtPctKolomDiskon_" & key, GetVal(ini, key & "_Inkjet_PctKolomDiskon", "10"))
        End If

        Dim grpM As GroupBox = TryCast(tabPage.Controls("grpMonitor_" & key), GroupBox)
        If grpM IsNot Nothing Then
            SetChk(grpM, "chkFooterM1_" & key, GetVal(ini, key & "_Monitor_TampilFooter1", "True"))
            SetChk(grpM, "chkFooterM2_" & key, GetVal(ini, key & "_Monitor_TampilFooter2", "True"))
            SetChk(grpM, "chkFooterM3_" & key, GetVal(ini, key & "_Monitor_TampilFooter3", "True"))
        End If

        Dim grpPdf As GroupBox = TryCast(tabPage.Controls("grpPDF_" & key), GroupBox)
        If grpPdf IsNot Nothing Then
            SetChk(grpPdf, "chkFooterP1_" & key, GetVal(ini, key & "_PDF_TampilFooter1", "True"))
            SetChk(grpPdf, "chkFooterP2_" & key, GetVal(ini, key & "_PDF_TampilFooter2", "True"))
            SetChk(grpPdf, "chkFooterP3_" & key, GetVal(ini, key & "_PDF_TampilFooter3", "True"))
        End If
        tabPage.ResumeLayout(False)
    End Sub

#End Region

#Region "Helper Set/Get Kontrol (rekursif)"

    Private Function CariKontrol(Of T As Control)(parent As Control, name As String) As T
        Dim c As T = TryCast(parent.Controls(name), T)
        If c IsNot Nothing Then Return c
        For Each child As Control In parent.Controls
            If child.Controls.Count > 0 Then
                Dim hasil As T = CariKontrol(Of T)(child, name)
                If hasil IsNot Nothing Then Return hasil
            End If
        Next
        Return Nothing
    End Function

    Private Sub SetCmb(parent As Control, name As String, value As String)
        Dim c As ComboBox = CariKontrol(Of ComboBox)(parent, name)
        If c IsNot Nothing Then c.Text = value
    End Sub

    ' SetCmb khusus printer — jika value kosong, fallback ke printer default Windows
    Private Sub SetCmbPrinter(parent As Control, name As String, value As String)
        Dim c As ComboBox = CariKontrol(Of ComboBox)(parent, name)
        If c Is Nothing Then Return
        If Not String.IsNullOrEmpty(value) Then
            c.Text = value
        Else
            If _defaultPrinterCache Is Nothing Then
                _defaultPrinterCache = New PrinterSettings().PrinterName
            End If
            Dim idx As Integer = c.Items.IndexOf(_defaultPrinterCache)
            c.Text = If(idx >= 0, _defaultPrinterCache, If(c.Items.Count > 0, c.Items(0).ToString(), ""))
        End If
    End Sub
    Private Sub SetTxt(parent As Control, name As String, value As String)
        Dim c As TextBox = CariKontrol(Of TextBox)(parent, name)
        If c IsNot Nothing Then c.Text = value
    End Sub
    Private Sub SetChk(parent As Control, name As String, value As String)
        Dim c As CheckBox = CariKontrol(Of CheckBox)(parent, name)
        If c IsNot Nothing Then c.Checked = (value.ToLower() = "true")
    End Sub
    Private Sub SetNum(parent As Control, name As String, value As String)
        Dim c As NumericUpDown = CariKontrol(Of NumericUpDown)(parent, name)
        If c IsNot Nothing Then
            Dim v As Decimal
            If Decimal.TryParse(value, v) Then c.Value = Math.Max(c.Minimum, Math.Min(c.Maximum, v))
        End If
    End Sub
    Private Function GetCmb(parent As Control, name As String) As String
        Dim c As ComboBox = CariKontrol(Of ComboBox)(parent, name)
        Return If(c IsNot Nothing, c.Text, "")
    End Function
    Private Function GetTxt(parent As Control, name As String) As String
        Dim c As TextBox = CariKontrol(Of TextBox)(parent, name)
        Return If(c IsNot Nothing, c.Text, "")
    End Function
    Private Function GetChk(parent As Control, name As String) As String
        Dim c As CheckBox = CariKontrol(Of CheckBox)(parent, name)
        Return If(c IsNot Nothing, c.Checked.ToString(), "False")
    End Function
    Private Function GetNum(parent As Control, name As String) As String
        Dim c As NumericUpDown = CariKontrol(Of NumericUpDown)(parent, name)
        Return If(c IsNot Nothing, c.Value.ToString(), "1")
    End Function

#End Region

#Region "Tampilkan Panel & Event Jenis Printer"

    Private Sub TampilkanPanel(tabPage As TabPage, jenis As String)
        Dim key As String = tabPage.Name
        Dim grpT As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
        Dim grpD As GroupBox = TryCast(tabPage.Controls("grpDot_" & key), GroupBox)
        Dim grpI As GroupBox = TryCast(tabPage.Controls("grpInk_" & key), GroupBox)
        Dim grpM As GroupBox = TryCast(tabPage.Controls("grpMonitor_" & key), GroupBox)
        Dim grpPdf As GroupBox = TryCast(tabPage.Controls("grpPDF_" & key), GroupBox)
        If grpT IsNot Nothing Then grpT.Visible = (jenis = "Printer Thermal")
        If grpD IsNot Nothing Then grpD.Visible = (jenis = "Printer Dot Matrix")
        If grpI IsNot Nothing Then grpI.Visible = (jenis = "Printer Inkjet / Laser")
        If grpM IsNot Nothing Then grpM.Visible = (jenis = "Tampilkan di Monitor")
        If grpPdf IsNot Nothing Then grpPdf.Visible = (jenis = "Export ke PDF")
    End Sub

    Private Sub CmbJenisPrinter_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim cmb As ComboBox = CType(sender, ComboBox)
        Dim tabPage As TabPage = TryCast(cmb.Tag, TabPage)
        If tabPage IsNot Nothing Then TampilkanPanel(tabPage, cmb.Text)
    End Sub

#End Region

#Region "Baca / Tulis pengaturan_cetak.ini"

    Private Function BacaIni() As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)
        If Not File.Exists("pengaturan_cetak.ini") Then Return result
        For Each line As String In File.ReadAllLines("pengaturan_cetak.ini")
            Dim parts = line.Split({"="c}, 2)
            If parts.Length = 2 Then result(parts(0).Trim()) = parts(1).Trim()
        Next
        Return result
    End Function

    Private Function GetVal(ini As Dictionary(Of String, String), key As String, def As String) As String
        Return If(ini.ContainsKey(key), ini(key), def)
    End Function

    Private Sub SimpanIni(ini As Dictionary(Of String, String))
        Dim lines As New List(Of String)
        Dim transaksiKeys As String() = {"Jual", "Beli", "ReturJual", "ReturBeli",
                                          "SuratJalan", "TransferBarang", "BayarHutang", "BayarPiutang",
                                          "Gaji", "Bon", "Laporan"}

        lines.Add("; ============================================================")
        lines.Add("; PENGATURAN UMUM")
        lines.Add("; ============================================================")
        For Each k In {"DefaultPrinter", "StatusKomputer"}
            If ini.ContainsKey(k) Then lines.Add(k & "=" & ini(k))
        Next
        lines.Add("")

        Dim judulTab As New Dictionary(Of String, String) From {
            {"Jual", "PENJUALAN"}, {"Beli", "PEMBELIAN"},
            {"ReturJual", "RETUR JUAL"}, {"ReturBeli", "RETUR BELI"},
            {"SuratJalan", "SURAT JALAN"}, {"TransferBarang", "TRANSFER BARANG"},
            {"BayarHutang", "BAYAR HUTANG"}, {"BayarPiutang", "BAYAR PIUTANG"},
            {"Gaji", "SLIP GAJI"}, {"Bon", "BON KARYAWAN"},
            {"Laporan", "LAPORAN"}
        }

        For Each trx In transaksiKeys
            ' Kumpulkan data per section dulu — header hanya ditulis jika ada isi
            Dim secUtama As New List(Of String)
            Dim jKey = trx & "_JenisPrinter"
            If ini.ContainsKey(jKey) Then secUtama.Add(jKey & "=" & ini(jKey))

            Dim secThermal As New List(Of String)
            If ini.ContainsKey(trx & "_DefaultCetak") Then secThermal.Add(trx & "_DefaultCetak=" & ini(trx & "_DefaultCetak"))
            For Each k In ini.Keys.Where(Function(x) x.StartsWith(trx & "_Thermal_")).OrderBy(Function(x) x)
                secThermal.Add(k & "=" & ini(k))
            Next

            Dim secDot As New List(Of String)
            For Each k In ini.Keys.Where(Function(x) x.StartsWith(trx & "_DotMatrix_") OrElse
                                                      x.StartsWith(trx & "_DotGdi_") OrElse
                                                      x.StartsWith(trx & "_DotEsc_")).OrderBy(Function(x) x)
                secDot.Add(k & "=" & ini(k))
            Next

            Dim secInk As New List(Of String)
            For Each k In ini.Keys.Where(Function(x) x.StartsWith(trx & "_Inkjet_")).OrderBy(Function(x) x)
                secInk.Add(k & "=" & ini(k))
            Next

            Dim secMon As New List(Of String)
            For Each k In ini.Keys.Where(Function(x) x.StartsWith(trx & "_Monitor_") OrElse
                                                      x.StartsWith(trx & "_PDF_")).OrderBy(Function(x) x)
                secMon.Add(k & "=" & ini(k))
            Next

            ' Tulis hanya jika ada data di salah satu section
            Dim adaData As Boolean = secUtama.Count > 0 OrElse secThermal.Count > 0 OrElse
                                     secDot.Count > 0 OrElse secInk.Count > 0 OrElse secMon.Count > 0
            If Not adaData Then Continue For

            lines.Add("; ============================================================")
            lines.Add("; " & judulTab(trx))
            lines.Add("; ============================================================")
            lines.AddRange(secUtama)
            If secThermal.Count > 0 Then
                lines.Add("; -- Thermal --")
                lines.AddRange(secThermal)
            End If
            If secDot.Count > 0 Then
                lines.Add("; -- Dot Matrix --")
                lines.AddRange(secDot)
            End If
            If secInk.Count > 0 Then
                lines.Add("; -- Inkjet / Laser --")
                lines.AddRange(secInk)
            End If
            If secMon.Count > 0 Then
                lines.Add("; -- Monitor & PDF --")
                lines.AddRange(secMon)
            End If
            lines.Add("")
        Next

        File.WriteAllLines("pengaturan_cetak.ini", lines)
    End Sub

#End Region

#Region "Simpan Semua"

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        _iniCache = Nothing  ' invalidate cache agar tab berikutnya baca data terbaru
        Dim ini = BacaIni()
        ini("DefaultPrinter") = CmbPrinterDefault.Text
        ini("StatusKomputer") = CmbStatusKomputer.Text

        For Each tabPage As TabPage In TabTransaksi.TabPages
            Dim key As String = tabPage.Name
            Dim cmbJenis As ComboBox = TryCast(tabPage.Controls("cmbJenisPrinter_" & key), ComboBox)
            If cmbJenis IsNot Nothing Then
                ini(key & "_JenisPrinter") = cmbJenis.Text

                ' Derive dan simpan DefaultCetak — profil lengkap untuk "Langsung Cetak"
                Dim grpTMode As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
                Dim grpDMode As GroupBox = TryCast(tabPage.Controls("grpDot_" & key), GroupBox)
                Dim defaultCetak As String
                Select Case cmbJenis.Text
                    Case "Printer Thermal"
                        Dim mode As String = If(grpTMode IsNot Nothing, GetCmb(grpTMode, "cmbModeCetak_" & key), "ESC/POS (Raw)")
                        defaultCetak = If(mode = "GDI+ (Windows Print)", "Thermal_GDI", "Thermal_ESC")
                    Case "Printer Dot Matrix"
                        Dim modeDot As String = If(grpDMode IsNot Nothing, GetCmb(grpDMode, "cmbModeDot_" & key), "GDI+ (Windows Print)")
                        defaultCetak = If(modeDot = "ESC/P (Raw)", "DotMatrix_ESC", "DotMatrix_GDI")
                    Case "Printer Inkjet / Laser" : defaultCetak = "Inkjet"
                    Case "Tampilkan di Monitor" : defaultCetak = "Monitor"
                    Case "Export ke PDF" : defaultCetak = "PDF"
                    Case Else : defaultCetak = "Thermal_ESC"
                End Select
                ini(key & "_DefaultCetak") = defaultCetak
            End If

            Dim grpT As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
            If grpT IsNot Nothing Then
                ini(key & "_Thermal_ModeCetak") = GetCmb(grpT, "cmbModeCetak_" & key)
                ini(key & "_Thermal_TipeKoneksi") = GetCmb(grpT, "cmbTipeKoneksi_" & key)
                ini(key & "_Thermal_NamaPrinter") = GetCmb(grpT, "cmbPrinterThermal_" & key)
                ini(key & "_Thermal_IpAddress") = GetTxt(grpT, "txtIpAddress_" & key)
                ini(key & "_Thermal_NetworkPort") = GetTxt(grpT, "txtNetworkPort_" & key)
                ini(key & "_Thermal_UkuranKertas") = GetCmb(grpT, "cmbUkuranKertas_" & key)
                ini(key & "_Thermal_LebarKertas") = GetTxt(grpT, "txtLebar_" & key)
                ini(key & "_Thermal_BatasKiri") = GetTxt(grpT, "txtBatasKiri_" & key)
                ini(key & "_Thermal_JarakBaris") = GetTxt(grpT, "txtJarakGdi_" & key)
                ini(key & "_Thermal_JarakBarisEsc") = GetTxt(grpT, "txtJarakEsc_" & key)
                ini(key & "_Thermal_PortLaciKasir") = GetCmb(grpT, "cmbPortCash_" & key)
                ini(key & "_Thermal_KodeLaciKasir") = GetCmb(grpT, "cmbCodeCash_" & key)
                ' Potong otomatis — disimpan terpisah per mode
                ini(key & "_Thermal_PotongOtomatisEsc") = GetChk(grpT, "chkPotongEsc_" & key)
                ini(key & "_Thermal_PotongOtomatisGdi") = GetChk(grpT, "chkPotongGdi_" & key)
                ini(key & "_Thermal_JumlahCetakEsc") = GetNum(grpT, "numCopiesEsc_" & key)
                ini(key & "_Thermal_JumlahCetakGdi") = GetNum(grpT, "numCopiesGdi_" & key)
                ini(key & "_Thermal_ModelStruk") = GetCmb(grpT, "cmbModelStruk_" & key)
                ini(key & "_Thermal_FontJudul") = GetCmb(grpT, "cmbFontJudul_" & key & "_Thermal")
                ini(key & "_Thermal_FontKeterangan") = GetCmb(grpT, "cmbFontKeterangan_" & key & "_Thermal")
                ini(key & "_Thermal_FontIsi") = GetCmb(grpT, "cmbFontIsi_" & key & "_Thermal")
                ini(key & "_Thermal_FontFooter") = GetCmb(grpT, "cmbFontFooter_" & key & "_Thermal")
                ini(key & "_Thermal_UkuranJudul") = GetCmb(grpT, "cmbUkuranJudul_" & key & "_Thermal")
                ini(key & "_Thermal_UkuranKeterangan") = GetCmb(grpT, "cmbUkuranKeterangan_" & key & "_Thermal")
                ini(key & "_Thermal_UkuranIsi") = GetCmb(grpT, "cmbUkuranIsi_" & key & "_Thermal")
                ini(key & "_Thermal_UkuranFooter") = GetCmb(grpT, "cmbUkuranFooter_" & key & "_Thermal")
                ini(key & "_Thermal_EscUkuranJudul") = GetCmb(grpT, "cmbEscUkuranJudul_" & key & "_Thermal")
                ini(key & "_Thermal_EscUkuranKeterangan") = GetCmb(grpT, "cmbEscUkuranKeterangan_" & key & "_Thermal")
                ini(key & "_Thermal_EscUkuranIsi") = GetCmb(grpT, "cmbEscUkuranIsi_" & key & "_Thermal")
                ini(key & "_Thermal_EscUkuranFooter") = GetCmb(grpT, "cmbEscUkuranFooter_" & key & "_Thermal")
                ini(key & "_Thermal_TampilFooter1") = GetChk(grpT, "chkFooterT1_" & key)
                ini(key & "_Thermal_TampilFooter2") = GetChk(grpT, "chkFooterT2_" & key)
                ini(key & "_Thermal_TampilFooter3") = GetChk(grpT, "chkFooterT3_" & key)
                ini(key & "_Thermal_DpiCetak") = GetTxt(grpT, "txtDpiCetak_" & key)
                ini(key & "_Thermal_TampilLogo") = GetChk(grpT, "chkTampilLogoThermal_" & key)
            End If

            Dim grpD As GroupBox = TryCast(tabPage.Controls("grpDot_" & key), GroupBox)
            If grpD IsNot Nothing Then
                ini(key & "_DotMatrix_NamaPrinter") = GetCmb(grpD, "cmbPrinterDot_" & key)
                ini(key & "_DotMatrix_ModeCetak") = GetCmb(grpD, "cmbModeDot_" & key)

                Dim grpGdi As GroupBox = TryCast(grpD.Controls("grpDotGdi_" & key), GroupBox)
                If grpGdi IsNot Nothing Then
                    ini(key & "_DotGdi_LebarKertas") = GetTxt(grpGdi, "txtLebarDotGdi_" & key)
                    ini(key & "_DotGdi_UkuranKertas") = GetCmb(grpGdi, "cmbKertasDotGdi_" & key)
                    ini(key & "_DotGdi_BatasKiri") = GetTxt(grpGdi, "txtBatasKiriDotGdi_" & key)
                    ini(key & "_DotGdi_JarakBaris") = GetTxt(grpGdi, "txtJarakDotGdi_" & key)
                    ini(key & "_DotGdi_UkuranFont") = GetTxt(grpGdi, "txtUkuranFontDotGdi_" & key)
                    ini(key & "_DotGdi_JumlahCetak") = GetNum(grpGdi, "numCopiesDotGdi_" & key)
                    ini(key & "_DotGdi_ModelStruk") = GetCmb(grpGdi, "cmbModelDotGdi_" & key)
                    ini(key & "_DotGdi_TampilFooter1") = GetChk(grpGdi, "chkF1DotGdi_" & key)
                    ini(key & "_DotGdi_TampilFooter2") = GetChk(grpGdi, "chkF2DotGdi_" & key)
                    ini(key & "_DotGdi_TampilFooter3") = GetChk(grpGdi, "chkF3DotGdi_" & key)
                End If

                Dim grpEsc As GroupBox = TryCast(grpD.Controls("grpDotEsc_" & key), GroupBox)
                If grpEsc IsNot Nothing Then
                    ini(key & "_DotEsc_LebarKertas") = GetTxt(grpEsc, "txtLebarDotEsc_" & key)
                    ini(key & "_DotEsc_BatasKiri") = GetTxt(grpEsc, "txtBatasKiriDotEsc_" & key)
                    ini(key & "_DotEsc_JarakBaris") = GetTxt(grpEsc, "txtJarakDotEsc_" & key)
                    ini(key & "_DotEsc_JumlahCetak") = GetNum(grpEsc, "numCopiesDotEsc_" & key)
                    ini(key & "_DotEsc_ModelStruk") = GetCmb(grpEsc, "cmbModelDotEsc_" & key)
                    ini(key & "_DotEsc_EscUkuranJudul") = GetCmb(grpEsc, "cmbEscUkuranJudul_" & key & "_DotEsc")
                    ini(key & "_DotEsc_EscUkuranIsi") = GetCmb(grpEsc, "cmbEscUkuranIsi_" & key & "_DotEsc")
                    ini(key & "_DotEsc_EscUkuranFooter") = GetCmb(grpEsc, "cmbEscUkuranFooter_" & key & "_DotEsc")
                    ' Keterangan uses same combobox as Judul in DotMatrix
                    ini(key & "_DotEsc_EscUkuranKeterangan") = GetCmb(grpEsc, "cmbEscUkuranJudul_" & key & "_DotEsc")

                    ini(key & "_DotEsc_TampilFooter1") = GetChk(grpEsc, "chkF1DotEsc_" & key)
                    ini(key & "_DotEsc_TampilFooter2") = GetChk(grpEsc, "chkF2DotEsc_" & key)
                    ini(key & "_DotEsc_TampilFooter3") = GetChk(grpEsc, "chkF3DotEsc_" & key)
                End If
            End If

            Dim grpI As GroupBox = TryCast(tabPage.Controls("grpInk_" & key), GroupBox)
            If grpI IsNot Nothing Then
                ini(key & "_Inkjet_NamaPrinter") = GetCmb(grpI, "cmbPrinterInk_" & key)
                ini(key & "_Inkjet_UkuranKertas") = GetCmb(grpI, "cmbPaperSize_" & key)
                ini(key & "_Inkjet_Orientasi") = GetCmb(grpI, "cmbOrientasi_" & key)
                ini(key & "_Inkjet_JumlahCetak") = GetNum(grpI, "numCopiesInk_" & key)
                ini(key & "_Inkjet_MarginAtas") = GetTxt(grpI, "txtMarginAtas_" & key)
                ini(key & "_Inkjet_MarginBawah") = GetTxt(grpI, "txtMarginBawah_" & key)
                ini(key & "_Inkjet_MarginKiri") = GetTxt(grpI, "txtMarginKiri_" & key)
                ini(key & "_Inkjet_MarginKanan") = GetTxt(grpI, "txtMarginKanan_" & key)
                ini(key & "_Inkjet_FontJudul") = GetCmb(grpI, "cmbFontJudul_" & key & "_Ink")
                ini(key & "_Inkjet_FontIsi") = GetCmb(grpI, "cmbFontIsi_" & key & "_Ink")
                ini(key & "_Inkjet_UkuranJudul") = GetCmb(grpI, "cmbUkuranJudul_" & key & "_Ink")
                ini(key & "_Inkjet_UkuranIsi") = GetCmb(grpI, "cmbUkuranIsi_" & key & "_Ink")
                ini(key & "_Inkjet_TampilFooter1") = GetChk(grpI, "chkFooterI1_" & key)
                ini(key & "_Inkjet_TampilFooter2") = GetChk(grpI, "chkFooterI2_" & key)
                ini(key & "_Inkjet_TampilFooter3") = GetChk(grpI, "chkFooterI3_" & key)
                ini(key & "_Inkjet_ModelNota") = GetCmb(grpI, "cmbModelNotaInk_" & key)
                ini(key & "_Inkjet_TampilLogo") = GetChk(grpI, "chkTampilLogoInk_" & key)
                ini(key & "_Inkjet_TampilTandaTangan") = GetChk(grpI, "chkTampilTtdInk_" & key)
                ini(key & "_Inkjet_PctKolomNo") = GetTxt(grpI, "txtPctKolomNo_" & key)
                ini(key & "_Inkjet_PctKolomQty") = GetTxt(grpI, "txtPctKolomQty_" & key)
                ini(key & "_Inkjet_PctKolomHarga") = GetTxt(grpI, "txtPctKolomHarga_" & key)
                ini(key & "_Inkjet_PctKolomDiskon") = GetTxt(grpI, "txtPctKolomDiskon_" & key)
            End If

            Dim grpM As GroupBox = TryCast(tabPage.Controls("grpMonitor_" & key), GroupBox)
            If grpM IsNot Nothing Then
                ini(key & "_Monitor_TampilFooter1") = GetChk(grpM, "chkFooterM1_" & key)
                ini(key & "_Monitor_TampilFooter2") = GetChk(grpM, "chkFooterM2_" & key)
                ini(key & "_Monitor_TampilFooter3") = GetChk(grpM, "chkFooterM3_" & key)
            End If

            Dim grpPdf As GroupBox = TryCast(tabPage.Controls("grpPDF_" & key), GroupBox)
            If grpPdf IsNot Nothing Then
                ini(key & "_PDF_TampilFooter1") = GetChk(grpPdf, "chkFooterP1_" & key)
                ini(key & "_PDF_TampilFooter2") = GetChk(grpPdf, "chkFooterP2_" & key)
                ini(key & "_PDF_TampilFooter3") = GetChk(grpPdf, "chkFooterP3_" & key)
            End If
        Next

        SimpanIni(ini)
        MuatSemuaPengaturan()
        FormUtama.StatusNamaPC.Text = AppStatusKomputer
        MessageBox.Show("Pengaturan printer berhasil disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region

#Region "Tes Buka Laci Kasir"

    Private Sub BtnTesCash_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim tabPage As TabPage = TryCast(btn.Tag, TabPage)
        If tabPage Is Nothing Then
            Dim ctrl As Control = btn.Parent
            Do While ctrl IsNot Nothing AndAlso TypeOf ctrl IsNot TabPage
                ctrl = ctrl.Parent
            Loop
            tabPage = TryCast(ctrl, TabPage)
        End If
        If tabPage Is Nothing Then Return

        Dim key As String = tabPage.Name
        Dim grpT As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
        If grpT Is Nothing Then Return

        Dim namaPrinter As String = GetCmb(grpT, "cmbPrinterThermal_" & key)
        Dim lebarKertas As Integer = CInt(Val(GetTxt(grpT, "txtLebar_" & key)))
        If lebarKertas = 0 Then lebarKertas = 80

        Dim tipeKoneksi As String = GetCmb(grpT, "cmbTipeKoneksi_" & key)
        Dim ipAddress As String = GetTxt(grpT, "txtIpAddress_" & key)
        Dim networkPort As Integer = CInt(Val(GetTxt(grpT, "txtNetworkPort_" & key)))
        If networkPort = 0 Then networkPort = 9100
        Dim isNetwork As Boolean = tipeKoneksi = "Network / WiFi (IP)"

        If isNetwork AndAlso String.IsNullOrEmpty(ipAddress) Then
            MessageBox.Show("Masukkan IP Address printer terlebih dahulu.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If
        If Not isNetwork AndAlso String.IsNullOrEmpty(namaPrinter) Then
            MessageBox.Show("Pilih nama printer terlebih dahulu.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Dim kodePerintah As String = GetCmb(grpT, "cmbCodeCash_" & key)
        Dim pinLaci As Integer = If(kodePerintah.Contains("Pin 5"), 5, 2)
        Dim pulseMs As Integer = If(kodePerintah.Contains("200ms"), 200, 100)
        Dim pulseByte As Byte = CByte(Math.Min(255, pulseMs \ 10))
        Dim escP As Byte() = {&H1B, &H70, If(pinLaci = 5, CByte(1), CByte(0)), pulseByte, pulseByte}

        Try
            Dim modeCetak As String = GetCmb(grpT, "cmbModeCetak_" & key)
            If isNetwork Then
                Dim mesinCetak As New PrinterEscPos(ipAddress, networkPort, lebarKertas)
                mesinCetak.BukaLaci(pinLaci)
            ElseIf modeCetak = "GDI+ (Windows Print)" Then
                RawPrinterHelper.KirimKePrinter(namaPrinter, escP)
            Else
                Dim mesinCetak As New PrinterEscPos(namaPrinter, lebarKertas)
                mesinCetak.BukaLaci(pinLaci)
            End If
            MessageBox.Show("Perintah buka laci kasir berhasil dikirim.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            Dim comPort As String = GetCmb(grpT, "cmbPortCash_" & key)
            If Not String.IsNullOrEmpty(comPort) Then
                Try
                    Using port As New SerialPort(comPort, 9600, Parity.None, 8, StopBits.One)
                        port.Open()
                        port.Write(escP, 0, escP.Length)
                    End Using
                    MessageBox.Show("Perintah buka laci berhasil dikirim via Serial.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch exSerial As Exception
                    MessageBox.Show("Gagal: " & exSerial.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Else
                MessageBox.Show("Gagal: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

#End Region

#Region "Restore Default Tab"

    Private Sub BtnRestore_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim tabPage As TabPage = TryCast(btn.Tag, TabPage)
        If tabPage Is Nothing Then Return
        Dim key As String = tabPage.Name

        ' Bangun dictionary default dari alias profil CetakDefault_*
        ' sehingga MuatTab mengisi JenisPrinter dan ModeCetak sesuai profil yang tersimpan
        Dim iniDefault As New Dictionary(Of String, String)
        Dim profil As String = AmbilCetakDefault(key)
        Select Case profil
            Case "Thermal_ESC"
                iniDefault(key & "_JenisPrinter") = "Printer Thermal"
                iniDefault(key & "_Thermal_ModeCetak") = "ESC/POS (Raw)"
            Case "Thermal_GDI"
                iniDefault(key & "_JenisPrinter") = "Printer Thermal"
                iniDefault(key & "_Thermal_ModeCetak") = "GDI+ (Windows Print)"
            Case "DotMatrix_GDI"
                iniDefault(key & "_JenisPrinter") = "Printer Dot Matrix"
                iniDefault(key & "_DotMatrix_ModeCetak") = "GDI+ (Windows Print)"
            Case "DotMatrix_ESC"
                iniDefault(key & "_JenisPrinter") = "Printer Dot Matrix"
                iniDefault(key & "_DotMatrix_ModeCetak") = "ESC/P (Raw)"
            Case "Inkjet"
                iniDefault(key & "_JenisPrinter") = "Printer Inkjet / Laser"
            Case "Monitor"
                iniDefault(key & "_JenisPrinter") = "Tampilkan di Monitor"
            Case "PDF"
                iniDefault(key & "_JenisPrinter") = "Export ke PDF"
            Case Else  ' fallback jika profil belum diset
                iniDefault(key & "_JenisPrinter") = "Printer Thermal"
                iniDefault(key & "_Thermal_ModeCetak") = "ESC/POS (Raw)"
        End Select

        MuatTab(tabPage, iniDefault, key)

        ' Refresh visibilitas panel sesuai jenis printer dari profil
        Dim cmbJenis As ComboBox = TryCast(tabPage.Controls("cmbJenisPrinter_" & key), ComboBox)
        If cmbJenis IsNot Nothing Then TampilkanPanel(tabPage, cmbJenis.Text)

        Dim grpT As GroupBox = TryCast(tabPage.Controls("grpThermal_" & key), GroupBox)
        If grpT IsNot Nothing Then
            Dim cmbMode As ComboBox = CariKontrol(Of ComboBox)(grpT, "cmbModeCetak_" & key)
            If cmbMode IsNot Nothing Then
                Dim isEscPos As Boolean = (cmbMode.Text = "ESC/POS (Raw)")
                Dim lblJarak As Label = CariKontrol(Of Label)(grpT, "lblJarakBaris_" & key)
                If lblJarak IsNot Nothing Then
                    lblJarak.Text = If(isEscPos, "Jarak (baris) :", "Jarak (px) :")
                End If
                Dim txtJarakGdiR As Control = CariKontrol(Of Control)(grpT, "txtJarakGdi_" & key)
                Dim txtJarakEscR As Control = CariKontrol(Of Control)(grpT, "txtJarakEsc_" & key)
                If txtJarakGdiR IsNot Nothing Then txtJarakGdiR.Visible = Not isEscPos
                If txtJarakEscR IsNot Nothing Then txtJarakEscR.Visible = isEscPos
                ' chkPotong berlaku untuk kedua mode — masing-masing terpisah
                Dim chkPotongEscR As Control = CariKontrol(Of Control)(grpT, "chkPotongEsc_" & key)
                Dim chkPotongGdiR As Control = CariKontrol(Of Control)(grpT, "chkPotongGdi_" & key)
                If chkPotongEscR IsNot Nothing Then chkPotongEscR.Visible = isEscPos
                If chkPotongGdiR IsNot Nothing Then chkPotongGdiR.Visible = Not isEscPos
                ' Jumlah cetak — masing-masing terpisah
                Dim numCopiesEscR As Control = CariKontrol(Of Control)(grpT, "numCopiesEsc_" & key)
                Dim numCopiesGdiR As Control = CariKontrol(Of Control)(grpT, "numCopiesGdi_" & key)
                If numCopiesEscR IsNot Nothing Then numCopiesEscR.Visible = isEscPos
                If numCopiesGdiR IsNot Nothing Then numCopiesGdiR.Visible = Not isEscPos
                Dim lblDpi As Control = CariKontrol(Of Control)(grpT, "lblDpiCetak_" & key)
                Dim txtDpi As Control = CariKontrol(Of Control)(grpT, "txtDpiCetak_" & key)
                If lblDpi IsNot Nothing Then lblDpi.Visible = Not isEscPos
                If txtDpi IsNot Nothing Then txtDpi.Visible = Not isEscPos

                Dim grpFnt As Control = CariKontrol(Of Control)(grpT, "grpFont")
                Dim grpFntEsc As Control = CariKontrol(Of Control)(grpT, "grpFontEsc_" & key)
                ' Find the original grpFont without explicit name (it was not named). Let's fetch it based on type.
                For Each c As Control In grpT.Controls
                    If TypeOf c Is GroupBox AndAlso c.Text.Contains("untuk mode GDI+") Then grpFnt = c
                Next
                If grpFnt IsNot Nothing Then grpFnt.Visible = Not isEscPos
                If grpFntEsc IsNot Nothing Then grpFntEsc.Visible = isEscPos

                ' Interactive toggle for ESC/POS vs GDI+ visibility
                AddHandler cmbMode.SelectedIndexChanged,
                    Sub(s, ev)
                        Dim modeEsc As Boolean = (cmbMode.Text = "ESC/POS (Raw)")
                        If lblJarak IsNot Nothing Then lblJarak.Text = If(modeEsc, "Jarak (baris) :", "Jarak (px) :")
                        If txtJarakGdiR IsNot Nothing Then txtJarakGdiR.Visible = Not modeEsc
                        If txtJarakEscR IsNot Nothing Then txtJarakEscR.Visible = modeEsc
                        If chkPotongEscR IsNot Nothing Then chkPotongEscR.Visible = modeEsc
                        If chkPotongGdiR IsNot Nothing Then chkPotongGdiR.Visible = Not modeEsc
                        If numCopiesEscR IsNot Nothing Then numCopiesEscR.Visible = modeEsc
                        If numCopiesGdiR IsNot Nothing Then numCopiesGdiR.Visible = Not modeEsc
                        If lblDpi IsNot Nothing Then lblDpi.Visible = Not modeEsc
                        If txtDpi IsNot Nothing Then txtDpi.Visible = Not modeEsc
                        If grpFnt IsNot Nothing Then grpFnt.Visible = Not modeEsc
                        If grpFntEsc IsNot Nothing Then grpFntEsc.Visible = modeEsc
                    End Sub
            End If
        End If

        Dim grpD As GroupBox = TryCast(tabPage.Controls("grpDot_" & key), GroupBox)
        If grpD IsNot Nothing Then
            Dim cmbModeDot As ComboBox = CariKontrol(Of ComboBox)(grpD, "cmbModeDot_" & key)
            If cmbModeDot IsNot Nothing Then
                Dim isGdi As Boolean = (cmbModeDot.Text = "GDI+ (Windows Print)")
                Dim grpGdi As GroupBox = TryCast(grpD.Controls("grpDotGdi_" & key), GroupBox)
                Dim grpEsc As GroupBox = TryCast(grpD.Controls("grpDotEsc_" & key), GroupBox)
                If grpGdi IsNot Nothing Then grpGdi.Visible = isGdi
                If grpEsc IsNot Nothing Then grpEsc.Visible = Not isGdi
            End If
        End If

        MessageBox.Show("Pengaturan tab ini dikembalikan ke nilai default.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region

#Region "Set Printer Default Windows"

    Private Sub BtnSetDefault_Click(sender As Object, e As EventArgs) Handles BtnSetDefault.Click
        If String.IsNullOrWhiteSpace(CmbPrinterDefault.Text) Then
            MessageBox.Show("Pilih printer terlebih dahulu.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If
        Try
            Dim query As String = "SELECT * FROM Win32_Printer WHERE Name = '" &
                CmbPrinterDefault.Text.Replace("\", "\\") & "'"
            Using searcher As New ManagementObjectSearcher(query)
                For Each printer As ManagementObject In searcher.Get()
                    printer.InvokeMethod("SetDefaultPrinter", Nothing)
                Next
            End Using
            MessageBox.Show("Printer default Windows diubah ke: " & CmbPrinterDefault.Text,
                   "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LblPrinterAktif.Text = "Default Windows saat ini: " & CmbPrinterDefault.Text
        Catch ex As Exception
            MessageBox.Show("Gagal: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Auto Suggest Peran & Keluar"

    ''' <summary>
    ''' Baca profil DefaultCetak dari INI untuk key transaksi tertentu.
    ''' Nilai: Thermal_ESC | Thermal_GDI | DotMatrix_GDI | DotMatrix_ESC | Inkjet | Monitor | PDF
    ''' </summary>
    Private Function AmbilCetakDefault(key As String) As String
        Dim ini = BacaIni()
        Return GetVal(ini, key & "_DefaultCetak", "Thermal_ESC")
    End Function

    Private Function AutoSuggestPeran(hostname As String) As String
        Dim h As String = hostname.ToLower()
        If h.Contains("server") Then Return "Server"
        If h.Contains("admin") Then
            If h.Contains("2") Then Return "Admin2"
            If h.Contains("3") Then Return "Admin3"
            Return "Admin1"
        End If
        If h.Contains("kasir") Then
            If h.Contains("2") Then Return "Kasir2"
            If h.Contains("3") Then Return "Kasir3"
            Return "Kasir1"
        End If
        Return "Server"
    End Function

    Private Sub BtnKeluar_Click(sender As Object, e As EventArgs) Handles BtnKeluar.Click
        Close()
    End Sub

#End Region

#Region "API Publik"

    ''' <summary>
    ''' Baca setting printer untuk modul cetak.
    ''' Contoh: FormPengaturanPrinter.GetPrinterSetting("Jual", "T_Printer")
    ''' </summary>
    Public Shared Function GetPrinterSetting(transaksi As String, field As String,
                                              Optional def As String = "") As String
        If Not File.Exists("pengaturan_cetak.ini") Then Return def
        Dim key As String = transaksi & "_" & field
        For Each line As String In File.ReadAllLines("pengaturan_cetak.ini")
            Dim parts = line.Split({"="c}, 2)
            If parts.Length = 2 AndAlso parts(0).Trim() = key Then Return parts(1).Trim()
        Next
        Return def
    End Function

#End Region

End Class
