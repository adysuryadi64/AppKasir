<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMasterPoin
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.TabControlPoin = New System.Windows.Forms.TabControl()
        Me.TabKonfigurasi = New System.Windows.Forms.TabPage()
        Me.LblInfoMekanisme = New System.Windows.Forms.Label()
        Me.LblPoinAktif = New System.Windows.Forms.Label()
        Me.CmbPoinAktif = New System.Windows.Forms.ComboBox()
        Me.LblPoinMekanisme = New System.Windows.Forms.Label()
        Me.CmbPoinMekanisme = New System.Windows.Forms.ComboBox()
        Me.BtnResetKonfig = New System.Windows.Forms.Button()
        Me.BtnSimpanKonfig = New System.Windows.Forms.Button()
        Me.TxtKelipatanNominal = New System.Windows.Forms.TextBox()
        Me.LblKelipatanNominal = New System.Windows.Forms.Label()
        Me.LblKelipatanNominalFormat = New System.Windows.Forms.Label()
        Me.TxtPoinPerQty = New System.Windows.Forms.TextBox()
        Me.LblPoinPerQty = New System.Windows.Forms.Label()
        Me.LblPoinPerQtyFormat = New System.Windows.Forms.Label()
        Me.LblMinimumRedeem = New System.Windows.Forms.Label()
        Me.TxtMinimumRedeem = New System.Windows.Forms.TextBox()
        Me.LblMinimumRedeemInfo = New System.Windows.Forms.Label()
        Me.TabHargaPoin = New System.Windows.Forms.TabPage()
        Me.BtnSimpanHargaPoin = New System.Windows.Forms.Button()
        Me.BtnHapusBarisPoin = New System.Windows.Forms.Button()
        Me.TxtCariBarang = New System.Windows.Forms.TextBox()
        Me.LblCariBarang = New System.Windows.Forms.Label()
        Me.LstHasilCariBarang = New System.Windows.Forms.ListBox()
        Me.DgvPoinBarang = New System.Windows.Forms.DataGridView()
        Me.TabRiwayat = New System.Windows.Forms.TabPage()
        Me.DgvRiwayatPoin = New System.Windows.Forms.DataGridView()
        Me.BtnTampilkanRiwayat = New System.Windows.Forms.Button()
        Me.DtpSampai = New System.Windows.Forms.DateTimePicker()
        Me.LblSampai = New System.Windows.Forms.Label()
        Me.DtpDari = New System.Windows.Forms.DateTimePicker()
        Me.LblDari = New System.Windows.Forms.Label()
        Me.LblSaldoPoin = New System.Windows.Forms.Label()
        Me.TxtCariPelanggan = New System.Windows.Forms.TextBox()
        Me.LblCariPelanggan = New System.Windows.Forms.Label()
        Me.LstHasilCariPelanggan = New System.Windows.Forms.ListBox()
        Me.LblKodePelanggan = New System.Windows.Forms.Label()
        Me.PanelHeader.SuspendLayout()
        Me.TabControlPoin.SuspendLayout()
        Me.TabKonfigurasi.SuspendLayout()
        Me.TabHargaPoin.SuspendLayout()
        CType(Me.DgvPoinBarang, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabRiwayat.SuspendLayout()
        CType(Me.DgvRiwayatPoin, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold)
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(900, 40)
        Me.PanelHeader.TabIndex = 0
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnClose.ForeColor = System.Drawing.Color.DarkRed
        Me.BtnClose.Location = New System.Drawing.Point(865, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(23, 23)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.Text = "X"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.Transparent
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 18.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle))
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(900, 40)
        Me.LblHeader.TabIndex = 1
        Me.LblHeader.Text = "M A S T E R   P O I N   L O Y A L T A S"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TabControlPoin
        '
        Me.TabControlPoin.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        Me.TabControlPoin.Controls.Add(Me.TabKonfigurasi)
        Me.TabControlPoin.Controls.Add(Me.TabHargaPoin)
        Me.TabControlPoin.Controls.Add(Me.TabRiwayat)
        Me.TabControlPoin.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed
        Me.TabControlPoin.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.TabControlPoin.Location = New System.Drawing.Point(8, 48)
        Me.TabControlPoin.Name = "TabControlPoin"
        Me.TabControlPoin.SelectedIndex = 0
        Me.TabControlPoin.Size = New System.Drawing.Size(884, 580)
        Me.TabControlPoin.TabIndex = 1
        '
        '─── TAB 1: KONFIGURASI POIN ───────────────────────────────────────────
        '
        'TabKonfigurasi
        '
        Me.TabKonfigurasi.BackColor = System.Drawing.SystemColors.Control
        Me.TabKonfigurasi.Controls.Add(Me.LblInfoMekanisme)
        Me.TabKonfigurasi.Controls.Add(Me.LblPoinAktif)
        Me.TabKonfigurasi.Controls.Add(Me.CmbPoinAktif)
        Me.TabKonfigurasi.Controls.Add(Me.LblPoinMekanisme)
        Me.TabKonfigurasi.Controls.Add(Me.CmbPoinMekanisme)
        Me.TabKonfigurasi.Controls.Add(Me.BtnResetKonfig)
        Me.TabKonfigurasi.Controls.Add(Me.BtnSimpanKonfig)
        Me.TabKonfigurasi.Controls.Add(Me.TxtKelipatanNominal)
        Me.TabKonfigurasi.Controls.Add(Me.LblKelipatanNominal)
        Me.TabKonfigurasi.Controls.Add(Me.LblKelipatanNominalFormat)
        Me.TabKonfigurasi.Controls.Add(Me.TxtPoinPerQty)
        Me.TabKonfigurasi.Controls.Add(Me.LblPoinPerQty)
        Me.TabKonfigurasi.Controls.Add(Me.LblPoinPerQtyFormat)
        Me.TabKonfigurasi.Controls.Add(Me.LblMinimumRedeem)
        Me.TabKonfigurasi.Controls.Add(Me.TxtMinimumRedeem)
        Me.TabKonfigurasi.Controls.Add(Me.LblMinimumRedeemInfo)
        Me.TabKonfigurasi.Location = New System.Drawing.Point(4, 26)
        Me.TabKonfigurasi.Name = "TabKonfigurasi"
        Me.TabKonfigurasi.Padding = New System.Windows.Forms.Padding(3)
        Me.TabKonfigurasi.Size = New System.Drawing.Size(876, 550)
        Me.TabKonfigurasi.TabIndex = 0
        Me.TabKonfigurasi.Text = "Konfigurasi Poin"
        '
        'LblInfoMekanisme
        '
        Me.LblInfoMekanisme.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Italic)
        Me.LblInfoMekanisme.ForeColor = System.Drawing.Color.DimGray
        Me.LblInfoMekanisme.Location = New System.Drawing.Point(20, 20)
        Me.LblInfoMekanisme.Name = "LblInfoMekanisme"
        Me.LblInfoMekanisme.Size = New System.Drawing.Size(500, 24)
        Me.LblInfoMekanisme.TabIndex = 0
        Me.LblInfoMekanisme.Text = "Mekanisme aktif: -"
        '
        'LblPoinAktif
        '
        Me.LblPoinAktif.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblPoinAktif.Location = New System.Drawing.Point(20, 55)
        Me.LblPoinAktif.Name = "LblPoinAktif"
        Me.LblPoinAktif.Size = New System.Drawing.Size(260, 28)
        Me.LblPoinAktif.TabIndex = 1
        Me.LblPoinAktif.Text = "Aktifkan Sistem Poin :"
        Me.LblPoinAktif.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbPoinAktif
        '
        Me.CmbPoinAktif.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPoinAktif.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.CmbPoinAktif.FormattingEnabled = True
        Me.CmbPoinAktif.Items.AddRange(New Object() {"Tidak", "Iya"})
        Me.CmbPoinAktif.Location = New System.Drawing.Point(290, 57)
        Me.CmbPoinAktif.Name = "CmbPoinAktif"
        Me.CmbPoinAktif.Size = New System.Drawing.Size(120, 25)
        Me.CmbPoinAktif.TabIndex = 2
        '
        'LblPoinMekanisme
        '
        Me.LblPoinMekanisme.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblPoinMekanisme.Location = New System.Drawing.Point(20, 90)
        Me.LblPoinMekanisme.Name = "LblPoinMekanisme"
        Me.LblPoinMekanisme.Size = New System.Drawing.Size(260, 28)
        Me.LblPoinMekanisme.TabIndex = 3
        Me.LblPoinMekanisme.Text = "Mekanisme Perolehan Poin :"
        Me.LblPoinMekanisme.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbPoinMekanisme
        '
        Me.CmbPoinMekanisme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPoinMekanisme.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.CmbPoinMekanisme.FormattingEnabled = True
        Me.CmbPoinMekanisme.Items.AddRange(New Object() {"Per Item (Qty)", "Per Kelipatan Nominal"})
        Me.CmbPoinMekanisme.Location = New System.Drawing.Point(290, 92)
        Me.CmbPoinMekanisme.Name = "CmbPoinMekanisme"
        Me.CmbPoinMekanisme.Size = New System.Drawing.Size(200, 25)
        Me.CmbPoinMekanisme.TabIndex = 4
        '
        'LblPoinPerQty
        '
        Me.LblPoinPerQty.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblPoinPerQty.Location = New System.Drawing.Point(20, 130)
        Me.LblPoinPerQty.Name = "LblPoinPerQty"
        Me.LblPoinPerQty.Size = New System.Drawing.Size(260, 28)
        Me.LblPoinPerQty.TabIndex = 5
        Me.LblPoinPerQty.Text = "Poin per 1 Qty Item :"
        Me.LblPoinPerQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPoinPerQty
        '
        Me.TxtPoinPerQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPoinPerQty.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.TxtPoinPerQty.Location = New System.Drawing.Point(290, 132)
        Me.TxtPoinPerQty.Name = "TxtPoinPerQty"
        Me.TxtPoinPerQty.Size = New System.Drawing.Size(120, 25)
        Me.TxtPoinPerQty.TabIndex = 6
        Me.TxtPoinPerQty.Text = "1,00"
        Me.TxtPoinPerQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblPoinPerQtyFormat
        '
        Me.LblPoinPerQtyFormat.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Italic)
        Me.LblPoinPerQtyFormat.ForeColor = System.Drawing.Color.DimGray
        Me.LblPoinPerQtyFormat.Location = New System.Drawing.Point(420, 132)
        Me.LblPoinPerQtyFormat.Name = "LblPoinPerQtyFormat"
        Me.LblPoinPerQtyFormat.Size = New System.Drawing.Size(200, 25)
        Me.LblPoinPerQtyFormat.TabIndex = 7
        Me.LblPoinPerQtyFormat.Text = "= 1,00 poin per item"
        Me.LblPoinPerQtyFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblKelipatanNominal
        '
        Me.LblKelipatanNominal.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblKelipatanNominal.Location = New System.Drawing.Point(20, 130)
        Me.LblKelipatanNominal.Name = "LblKelipatanNominal"
        Me.LblKelipatanNominal.Size = New System.Drawing.Size(260, 28)
        Me.LblKelipatanNominal.TabIndex = 8
        Me.LblKelipatanNominal.Text = "Kelipatan Nominal (Rp) :"
        Me.LblKelipatanNominal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblKelipatanNominal.Visible = False
        '
        'TxtKelipatanNominal
        '
        Me.TxtKelipatanNominal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKelipatanNominal.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.TxtKelipatanNominal.Location = New System.Drawing.Point(290, 132)
        Me.TxtKelipatanNominal.Name = "TxtKelipatanNominal"
        Me.TxtKelipatanNominal.Size = New System.Drawing.Size(120, 25)
        Me.TxtKelipatanNominal.TabIndex = 9
        Me.TxtKelipatanNominal.Text = "10.000"
        Me.TxtKelipatanNominal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtKelipatanNominal.Visible = False
        '
        'LblKelipatanNominalFormat
        '
        Me.LblKelipatanNominalFormat.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Italic)
        Me.LblKelipatanNominalFormat.ForeColor = System.Drawing.Color.DimGray
        Me.LblKelipatanNominalFormat.Location = New System.Drawing.Point(420, 132)
        Me.LblKelipatanNominalFormat.Name = "LblKelipatanNominalFormat"
        Me.LblKelipatanNominalFormat.Size = New System.Drawing.Size(200, 25)
        Me.LblKelipatanNominalFormat.TabIndex = 10
        Me.LblKelipatanNominalFormat.Text = "= Rp 10.000 → 1 poin"
        Me.LblKelipatanNominalFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.LblKelipatanNominalFormat.Visible = False
        'LblMinimumRedeem
        '
        Me.LblMinimumRedeem.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblMinimumRedeem.Location = New System.Drawing.Point(20, 168)
        Me.LblMinimumRedeem.Name = "LblMinimumRedeem"
        Me.LblMinimumRedeem.Size = New System.Drawing.Size(260, 28)
        Me.LblMinimumRedeem.TabIndex = 11
        Me.LblMinimumRedeem.Text = "Minimum Poin untuk Redeem :"
        Me.LblMinimumRedeem.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblMinimumRedeem.Visible = False
        '
        'TxtMinimumRedeem
        '
        Me.TxtMinimumRedeem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtMinimumRedeem.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.TxtMinimumRedeem.Location = New System.Drawing.Point(290, 170)
        Me.TxtMinimumRedeem.Name = "TxtMinimumRedeem"
        Me.TxtMinimumRedeem.Size = New System.Drawing.Size(80, 25)
        Me.TxtMinimumRedeem.TabIndex = 12
        Me.TxtMinimumRedeem.Text = "100"
        Me.TxtMinimumRedeem.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtMinimumRedeem.Visible = False
        '
        'LblMinimumRedeemInfo
        '
        Me.LblMinimumRedeemInfo.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Italic)
        Me.LblMinimumRedeemInfo.ForeColor = System.Drawing.Color.DimGray
        Me.LblMinimumRedeemInfo.Location = New System.Drawing.Point(380, 170)
        Me.LblMinimumRedeemInfo.Name = "LblMinimumRedeemInfo"
        Me.LblMinimumRedeemInfo.Size = New System.Drawing.Size(260, 25)
        Me.LblMinimumRedeemInfo.TabIndex = 13
        Me.LblMinimumRedeemInfo.Text = "poin (0 = tidak ada batas minimum)"
        Me.LblMinimumRedeemInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.LblMinimumRedeemInfo.Visible = False
        '
        'BtnSimpanKonfig
        '
        Me.BtnSimpanKonfig.BackColor = System.Drawing.Color.White
        Me.BtnSimpanKonfig.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpanKonfig.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanKonfig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpanKonfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpanKonfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpanKonfig.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnSimpanKonfig.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanKonfig.Location = New System.Drawing.Point(290, 200)
        Me.BtnSimpanKonfig.Name = "BtnSimpanKonfig"
        Me.BtnSimpanKonfig.Size = New System.Drawing.Size(120, 33)
        Me.BtnSimpanKonfig.TabIndex = 9
        Me.BtnSimpanKonfig.Text = "Simpan (F2)"
        Me.BtnSimpanKonfig.UseVisualStyleBackColor = False
        '
        'BtnResetKonfig
        '
        Me.BtnResetKonfig.BackColor = System.Drawing.Color.White
        Me.BtnResetKonfig.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnResetKonfig.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnResetKonfig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnResetKonfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnResetKonfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnResetKonfig.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnResetKonfig.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnResetKonfig.Location = New System.Drawing.Point(420, 200)
        Me.BtnResetKonfig.Name = "BtnResetKonfig"
        Me.BtnResetKonfig.Size = New System.Drawing.Size(100, 33)
        Me.BtnResetKonfig.TabIndex = 10
        Me.BtnResetKonfig.Text = "Reset"
        Me.BtnResetKonfig.UseVisualStyleBackColor = False
        '
        '─── TAB 2: HARGA POIN BARANG ──────────────────────────────────────────
        '
        'TabHargaPoin
        '
        Me.TabHargaPoin.BackColor = System.Drawing.SystemColors.Control
        Me.TabHargaPoin.Controls.Add(Me.BtnSimpanHargaPoin)
        Me.TabHargaPoin.Controls.Add(Me.BtnHapusBarisPoin)
        Me.TabHargaPoin.Controls.Add(Me.TxtCariBarang)
        Me.TabHargaPoin.Controls.Add(Me.LblCariBarang)
        Me.TabHargaPoin.Controls.Add(Me.LstHasilCariBarang)
        Me.TabHargaPoin.Controls.Add(Me.DgvPoinBarang)
        Me.TabHargaPoin.Location = New System.Drawing.Point(4, 26)
        Me.TabHargaPoin.Name = "TabHargaPoin"
        Me.TabHargaPoin.Padding = New System.Windows.Forms.Padding(3)
        Me.TabHargaPoin.Size = New System.Drawing.Size(876, 550)
        Me.TabHargaPoin.TabIndex = 1
        Me.TabHargaPoin.Text = "Harga Poin Barang"
        '
        'LblCariBarang
        '
        Me.LblCariBarang.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblCariBarang.Location = New System.Drawing.Point(8, 12)
        Me.LblCariBarang.Name = "LblCariBarang"
        Me.LblCariBarang.Size = New System.Drawing.Size(100, 24)
        Me.LblCariBarang.TabIndex = 0
        Me.LblCariBarang.Text = "Cari Barang :"
        Me.LblCariBarang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtCariBarang
        '
        Me.TxtCariBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtCariBarang.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.TxtCariBarang.Location = New System.Drawing.Point(114, 12)
        Me.TxtCariBarang.Name = "TxtCariBarang"
        Me.TxtCariBarang.Size = New System.Drawing.Size(300, 25)
        Me.TxtCariBarang.TabIndex = 1
        '
        'LstHasilCariBarang
        '
        Me.LstHasilCariBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LstHasilCariBarang.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LstHasilCariBarang.Location = New System.Drawing.Point(114, 38)
        Me.LstHasilCariBarang.Name = "LstHasilCariBarang"
        Me.LstHasilCariBarang.Size = New System.Drawing.Size(300, 120)
        Me.LstHasilCariBarang.TabIndex = 2
        Me.LstHasilCariBarang.Visible = False
        '
        'DgvPoinBarang
        '
        Me.DgvPoinBarang.AllowUserToAddRows = False
        Me.DgvPoinBarang.AllowUserToDeleteRows = False
        Me.DgvPoinBarang.AllowUserToResizeRows = False
        Me.DgvPoinBarang.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        Me.DgvPoinBarang.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvPoinBarang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvPoinBarang.Location = New System.Drawing.Point(8, 46)
        Me.DgvPoinBarang.Name = "DgvPoinBarang"
        Me.DgvPoinBarang.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvPoinBarang.Size = New System.Drawing.Size(860, 415)
        Me.DgvPoinBarang.TabIndex = 4
        '
        'BtnSimpanHargaPoin
        '
        Me.BtnSimpanHargaPoin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnSimpanHargaPoin.BackColor = System.Drawing.Color.White
        Me.BtnSimpanHargaPoin.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpanHargaPoin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanHargaPoin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpanHargaPoin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpanHargaPoin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpanHargaPoin.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnSimpanHargaPoin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanHargaPoin.Location = New System.Drawing.Point(8, 468)
        Me.BtnSimpanHargaPoin.Name = "BtnSimpanHargaPoin"
        Me.BtnSimpanHargaPoin.Size = New System.Drawing.Size(160, 33)
        Me.BtnSimpanHargaPoin.TabIndex = 5
        Me.BtnSimpanHargaPoin.Text = "Simpan Harga Poin"
        Me.BtnSimpanHargaPoin.UseVisualStyleBackColor = False
        '
        'BtnHapusBarisPoin
        '
        Me.BtnHapusBarisPoin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnHapusBarisPoin.BackColor = System.Drawing.Color.White
        Me.BtnHapusBarisPoin.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnHapusBarisPoin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnHapusBarisPoin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(202, Byte), Integer), CType(CType(202, Byte), Integer))
        Me.BtnHapusBarisPoin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnHapusBarisPoin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHapusBarisPoin.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnHapusBarisPoin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnHapusBarisPoin.Location = New System.Drawing.Point(178, 468)
        Me.BtnHapusBarisPoin.Name = "BtnHapusBarisPoin"
        Me.BtnHapusBarisPoin.Size = New System.Drawing.Size(130, 33)
        Me.BtnHapusBarisPoin.TabIndex = 6
        Me.BtnHapusBarisPoin.Text = "Hapus Baris"
        Me.BtnHapusBarisPoin.UseVisualStyleBackColor = False
        '
        '─── TAB 3: RIWAYAT POIN PELANGGAN ────────────────────────────────────
        '
        'TabRiwayat
        '
        Me.TabRiwayat.BackColor = System.Drawing.SystemColors.Control
        Me.TabRiwayat.Controls.Add(Me.DgvRiwayatPoin)
        Me.TabRiwayat.Controls.Add(Me.BtnTampilkanRiwayat)
        Me.TabRiwayat.Controls.Add(Me.DtpSampai)
        Me.TabRiwayat.Controls.Add(Me.LblSampai)
        Me.TabRiwayat.Controls.Add(Me.DtpDari)
        Me.TabRiwayat.Controls.Add(Me.LblDari)
        Me.TabRiwayat.Controls.Add(Me.LblKodePelanggan)
        Me.TabRiwayat.Controls.Add(Me.LblSaldoPoin)
        Me.TabRiwayat.Controls.Add(Me.TxtCariPelanggan)
        Me.TabRiwayat.Controls.Add(Me.LstHasilCariPelanggan)
        Me.TabRiwayat.Controls.Add(Me.LblCariPelanggan)
        Me.TabRiwayat.Location = New System.Drawing.Point(4, 26)
        Me.TabRiwayat.Name = "TabRiwayat"
        Me.TabRiwayat.Padding = New System.Windows.Forms.Padding(3)
        Me.TabRiwayat.Size = New System.Drawing.Size(876, 550)
        Me.TabRiwayat.TabIndex = 2
        Me.TabRiwayat.Text = "Riwayat Poin Pelanggan"
        '
        'LblCariPelanggan
        '
        Me.LblCariPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblCariPelanggan.Location = New System.Drawing.Point(8, 12)
        Me.LblCariPelanggan.Name = "LblCariPelanggan"
        Me.LblCariPelanggan.Size = New System.Drawing.Size(110, 24)
        Me.LblCariPelanggan.TabIndex = 0
        Me.LblCariPelanggan.Text = "Cari Pelanggan :"
        Me.LblCariPelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtCariPelanggan
        '
        Me.TxtCariPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtCariPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.TxtCariPelanggan.Location = New System.Drawing.Point(124, 12)
        Me.TxtCariPelanggan.Name = "TxtCariPelanggan"
        Me.TxtCariPelanggan.Size = New System.Drawing.Size(200, 25)
        Me.TxtCariPelanggan.TabIndex = 1
        '
        'LblKodePelanggan
        '
        Me.LblKodePelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblKodePelanggan.ForeColor = System.Drawing.Color.DimGray
        Me.LblKodePelanggan.Location = New System.Drawing.Point(328, 12)
        Me.LblKodePelanggan.Name = "LblKodePelanggan"
        Me.LblKodePelanggan.Size = New System.Drawing.Size(100, 24)
        Me.LblKodePelanggan.TabIndex = 2
        Me.LblKodePelanggan.Text = ""
        Me.LblKodePelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblSaldoPoin
        '
        Me.LblSaldoPoin.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblSaldoPoin.ForeColor = System.Drawing.Color.DarkBlue
        Me.LblSaldoPoin.Location = New System.Drawing.Point(440, 12)
        Me.LblSaldoPoin.Name = "LblSaldoPoin"
        Me.LblSaldoPoin.Size = New System.Drawing.Size(250, 24)
        Me.LblSaldoPoin.TabIndex = 2
        Me.LblSaldoPoin.Text = "Saldo Poin: -"
        Me.LblSaldoPoin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblDari
        '
        Me.LblDari.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblDari.Location = New System.Drawing.Point(8, 46)
        Me.LblDari.Name = "LblDari"
        Me.LblDari.Size = New System.Drawing.Size(110, 24)
        Me.LblDari.TabIndex = 3
        Me.LblDari.Text = "Dari Tanggal :"
        Me.LblDari.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DtpDari
        '
        Me.DtpDari.CustomFormat = "dd/MM/yyyy"
        Me.DtpDari.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.DtpDari.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpDari.Location = New System.Drawing.Point(124, 46)
        Me.DtpDari.Name = "DtpDari"
        Me.DtpDari.Size = New System.Drawing.Size(130, 25)
        Me.DtpDari.TabIndex = 4
        '
        'LblSampai
        '
        Me.LblSampai.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblSampai.Location = New System.Drawing.Point(264, 46)
        Me.LblSampai.Name = "LblSampai"
        Me.LblSampai.Size = New System.Drawing.Size(80, 24)
        Me.LblSampai.TabIndex = 5
        Me.LblSampai.Text = "s/d :"
        Me.LblSampai.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DtpSampai
        '
        Me.DtpSampai.CustomFormat = "dd/MM/yyyy"
        Me.DtpSampai.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.DtpSampai.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpSampai.Location = New System.Drawing.Point(350, 46)
        Me.DtpSampai.Name = "DtpSampai"
        Me.DtpSampai.Size = New System.Drawing.Size(130, 25)
        Me.DtpSampai.TabIndex = 6
        '
        'BtnTampilkanRiwayat
        '
        Me.BtnTampilkanRiwayat.BackColor = System.Drawing.Color.White
        Me.BtnTampilkanRiwayat.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTampilkanRiwayat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(99, Byte), Integer), CType(CType(235, Byte), Integer))
        Me.BtnTampilkanRiwayat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(191, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(254, Byte), Integer))
        Me.BtnTampilkanRiwayat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(234, Byte), Integer), CType(CType(254, Byte), Integer))
        Me.BtnTampilkanRiwayat.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampilkanRiwayat.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnTampilkanRiwayat.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(99, Byte), Integer), CType(CType(235, Byte), Integer))
        Me.BtnTampilkanRiwayat.Location = New System.Drawing.Point(490, 44)
        Me.BtnTampilkanRiwayat.Name = "BtnTampilkanRiwayat"
        Me.BtnTampilkanRiwayat.Size = New System.Drawing.Size(120, 28)
        Me.BtnTampilkanRiwayat.TabIndex = 7
        Me.BtnTampilkanRiwayat.Text = "Tampilkan"
        Me.BtnTampilkanRiwayat.UseVisualStyleBackColor = False
        '
        'LstHasilCariPelanggan
        '
        Me.LstHasilCariPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LstHasilCariPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LstHasilCariPelanggan.Location = New System.Drawing.Point(124, 38)
        Me.LstHasilCariPelanggan.Name = "LstHasilCariPelanggan"
        Me.LstHasilCariPelanggan.Size = New System.Drawing.Size(200, 120)
        Me.LstHasilCariPelanggan.TabIndex = 9
        Me.LstHasilCariPelanggan.Visible = False
        '
        'DgvRiwayatPoin
        '
        Me.DgvRiwayatPoin.AllowUserToAddRows = False
        Me.DgvRiwayatPoin.AllowUserToDeleteRows = False
        Me.DgvRiwayatPoin.AllowUserToResizeRows = False
        Me.DgvRiwayatPoin.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        Me.DgvRiwayatPoin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvRiwayatPoin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvRiwayatPoin.Location = New System.Drawing.Point(8, 82)
        Me.DgvRiwayatPoin.Name = "DgvRiwayatPoin"
        Me.DgvRiwayatPoin.ReadOnly = True
        Me.DgvRiwayatPoin.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvRiwayatPoin.Size = New System.Drawing.Size(860, 460)
        Me.DgvRiwayatPoin.TabIndex = 8
        '
        '─── FORM ──────────────────────────────────────────────────────────────
        '
        'FormMasterPoin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(900, 640)
        Me.Controls.Add(Me.TabControlPoin)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormMasterPoin"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Master Poin Loyalitas"
        Me.PanelHeader.ResumeLayout(False)
        Me.TabControlPoin.ResumeLayout(False)
        Me.TabKonfigurasi.ResumeLayout(False)
        Me.TabKonfigurasi.PerformLayout()
        Me.TabHargaPoin.PerformLayout()
        CType(Me.DgvPoinBarang, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabRiwayat.ResumeLayout(False)
        Me.TabRiwayat.PerformLayout()
        CType(Me.DgvRiwayatPoin, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblHeader As System.Windows.Forms.Label
    Friend WithEvents TabControlPoin As System.Windows.Forms.TabControl
    Friend WithEvents TabKonfigurasi As System.Windows.Forms.TabPage
    Friend WithEvents LblInfoMekanisme As System.Windows.Forms.Label
    Friend WithEvents LblPoinAktif As System.Windows.Forms.Label
    Friend WithEvents CmbPoinAktif As System.Windows.Forms.ComboBox
    Friend WithEvents LblPoinMekanisme As System.Windows.Forms.Label
    Friend WithEvents CmbPoinMekanisme As System.Windows.Forms.ComboBox
    Friend WithEvents BtnResetKonfig As System.Windows.Forms.Button
    Friend WithEvents BtnSimpanKonfig As System.Windows.Forms.Button
    Friend WithEvents TxtKelipatanNominal As System.Windows.Forms.TextBox
    Friend WithEvents LblKelipatanNominal As System.Windows.Forms.Label
    Friend WithEvents LblKelipatanNominalFormat As System.Windows.Forms.Label
    Friend WithEvents TxtPoinPerQty As System.Windows.Forms.TextBox
    Friend WithEvents LblPoinPerQty As System.Windows.Forms.Label
    Friend WithEvents LblPoinPerQtyFormat As System.Windows.Forms.Label
    Friend WithEvents TabHargaPoin As System.Windows.Forms.TabPage
    Friend WithEvents BtnSimpanHargaPoin As System.Windows.Forms.Button
    Friend WithEvents BtnHapusBarisPoin As System.Windows.Forms.Button
    Friend WithEvents TxtCariBarang As System.Windows.Forms.TextBox
    Friend WithEvents LblCariBarang As System.Windows.Forms.Label
    Friend WithEvents LstHasilCariBarang As System.Windows.Forms.ListBox
    Friend WithEvents DgvPoinBarang As System.Windows.Forms.DataGridView
    Friend WithEvents TabRiwayat As System.Windows.Forms.TabPage
    Friend WithEvents DgvRiwayatPoin As System.Windows.Forms.DataGridView
    Friend WithEvents BtnTampilkanRiwayat As System.Windows.Forms.Button
    Friend WithEvents DtpSampai As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblSampai As System.Windows.Forms.Label
    Friend WithEvents DtpDari As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblDari As System.Windows.Forms.Label
    Friend WithEvents LblSaldoPoin As System.Windows.Forms.Label
    Friend WithEvents TxtCariPelanggan As System.Windows.Forms.TextBox
    Friend WithEvents LblCariPelanggan As System.Windows.Forms.Label
    Friend WithEvents LstHasilCariPelanggan As System.Windows.Forms.ListBox
    Friend WithEvents LblKodePelanggan As System.Windows.Forms.Label
    Friend WithEvents LblMinimumRedeem As System.Windows.Forms.Label
    Friend WithEvents TxtMinimumRedeem As System.Windows.Forms.TextBox
    Friend WithEvents LblMinimumRedeemInfo As System.Windows.Forms.Label

End Class
