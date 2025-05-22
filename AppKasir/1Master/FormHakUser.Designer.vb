<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormHakUser
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormHakUser))
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.DgvUtility = New System.Windows.Forms.DataGridView()
        Me.ModulUtility = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaUtility = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.DgvLaporan = New System.Windows.Forms.DataGridView()
        Me.ModulLaporan = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaLaporan = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.DgvJurnal = New System.Windows.Forms.DataGridView()
        Me.ModulJurnal = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaJurnal = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TambahJurnal = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.EditJurnal = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.HapusJurnal = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.DgvTransaksi = New System.Windows.Forms.DataGridView()
        Me.ModulTransaksi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaTransaksi = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TambahTransaksi = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.EditTransaksi = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.HapusTransaksi = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.CmbUser = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblMasterData = New System.Windows.Forms.Label()
        Me.LblUtility = New System.Windows.Forms.Label()
        Me.LblLaporan = New System.Windows.Forms.Label()
        Me.LblJurnal = New System.Windows.Forms.Label()
        Me.LblTransaksi = New System.Windows.Forms.Label()
        Me.DGVMaster = New System.Windows.Forms.DataGridView()
        Me.ModulMasterData = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaMasterData = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TambahMasterData = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.EditMasterData = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.HapusMasterData = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.ChkAll = New System.Windows.Forms.CheckBox()
        Me.ChkNonAll = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblPosting = New System.Windows.Forms.Label()
        Me.LblKaryawan = New System.Windows.Forms.Label()
        Me.DgvPosting = New System.Windows.Forms.DataGridView()
        Me.ModulPosting = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaPosting = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.DgvKaryawan = New System.Windows.Forms.DataGridView()
        Me.ModulKaryawan = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BacaKaryawan = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TambahKaryawan = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.EditKaryawan = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.HapusKaryawan = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.LblBeliMuculJual = New System.Windows.Forms.Label()
        Me.CmbBeliMuculJual = New System.Windows.Forms.ComboBox()
        Me.LblBeliEditHarga = New System.Windows.Forms.Label()
        Me.CmbBeliEditHarga = New System.Windows.Forms.ComboBox()
        Me.LblBeliAverage = New System.Windows.Forms.Label()
        Me.CmbBeliAverage = New System.Windows.Forms.ComboBox()
        Me.LblBeliUpdate = New System.Windows.Forms.Label()
        Me.CmbBeliUpdate = New System.Windows.Forms.ComboBox()
        Me.LblBeliRugi = New System.Windows.Forms.Label()
        Me.CmbBeliRugi = New System.Windows.Forms.ComboBox()
        Me.LblBeliSatuan = New System.Windows.Forms.Label()
        Me.CmbBeliSatuan = New System.Windows.Forms.ComboBox()
        Me.LblBeliFokus = New System.Windows.Forms.Label()
        Me.CmbBeliFokus = New System.Windows.Forms.ComboBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.LblReturMinus = New System.Windows.Forms.Label()
        Me.CmbReturMinus = New System.Windows.Forms.ComboBox()
        Me.LblReturSatuan = New System.Windows.Forms.Label()
        Me.CmbReturSatuan = New System.Windows.Forms.ComboBox()
        Me.LblReturFokus = New System.Windows.Forms.Label()
        Me.CmbReturFokus = New System.Windows.Forms.ComboBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.LblTransferMinus = New System.Windows.Forms.Label()
        Me.CmbTransferMinus = New System.Windows.Forms.ComboBox()
        Me.LblTransferSatuan = New System.Windows.Forms.Label()
        Me.CmbTransferSatuan = New System.Windows.Forms.ComboBox()
        Me.LblTransferFocus = New System.Windows.Forms.Label()
        Me.CmbTransferFocus = New System.Windows.Forms.ComboBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.LblJualEditHarga = New System.Windows.Forms.Label()
        Me.CmbJualEditHarga = New System.Windows.Forms.ComboBox()
        Me.LblJualMinus = New System.Windows.Forms.Label()
        Me.CmbJualMinus = New System.Windows.Forms.ComboBox()
        Me.LblJualRugi = New System.Windows.Forms.Label()
        Me.CmbJualRugi = New System.Windows.Forms.ComboBox()
        Me.LblJualSatuan = New System.Windows.Forms.Label()
        Me.CmbJualSatuan = New System.Windows.Forms.ComboBox()
        Me.LblJualFokus = New System.Windows.Forms.Label()
        Me.CmbJualFokus = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LblEditHargaJual = New System.Windows.Forms.Label()
        Me.CmbEditHargaJual = New System.Windows.Forms.ComboBox()
        CType(Me.DgvUtility, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvLaporan, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvJurnal, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvTransaksi, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVMaster, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.DgvPosting, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvKaryawan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnSimpan
        '
        Me.BtnSimpan.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnSimpan.BackColor = System.Drawing.Color.SandyBrown
        Me.BtnSimpan.FlatAppearance.BorderSize = 0
        Me.BtnSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MidnightBlue
        Me.BtnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpan.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpan.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.Location = New System.Drawing.Point(217, 547)
        Me.BtnSimpan.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(169, 32)
        Me.BtnSimpan.TabIndex = 37
        Me.BtnSimpan.Text = "       Simpan (F2)"
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'BtnKeluar
        '
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluar.BackColor = System.Drawing.Color.SandyBrown
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderSize = 0
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.HotPink
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson
        Me.BtnKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluar.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.Black
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.Location = New System.Drawing.Point(415, 547)
        Me.BtnKeluar.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(140, 32)
        Me.BtnKeluar.TabIndex = 124
        Me.BtnKeluar.Text = "     Keluar (Esc)"
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'DgvUtility
        '
        Me.DgvUtility.AllowUserToAddRows = False
        Me.DgvUtility.AllowUserToDeleteRows = False
        Me.DgvUtility.AllowUserToResizeColumns = False
        Me.DgvUtility.AllowUserToResizeRows = False
        Me.DgvUtility.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvUtility.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvUtility.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvUtility.BackgroundColor = System.Drawing.Color.Khaki
        Me.DgvUtility.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvUtility.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvUtility.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulUtility, Me.BacaUtility})
        Me.DgvUtility.Location = New System.Drawing.Point(120, 232)
        Me.DgvUtility.Name = "DgvUtility"
        Me.DgvUtility.RowHeadersVisible = False
        Me.DgvUtility.Size = New System.Drawing.Size(461, 482)
        Me.DgvUtility.TabIndex = 136
        '
        'ModulUtility
        '
        Me.ModulUtility.HeaderText = "Modul"
        Me.ModulUtility.Name = "ModulUtility"
        '
        'BacaUtility
        '
        Me.BacaUtility.FillWeight = 50.0!
        Me.BacaUtility.HeaderText = "Baca"
        Me.BacaUtility.Name = "BacaUtility"
        Me.BacaUtility.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaUtility.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'DgvLaporan
        '
        Me.DgvLaporan.AllowUserToAddRows = False
        Me.DgvLaporan.AllowUserToDeleteRows = False
        Me.DgvLaporan.AllowUserToResizeColumns = False
        Me.DgvLaporan.AllowUserToResizeRows = False
        Me.DgvLaporan.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvLaporan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvLaporan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvLaporan.BackgroundColor = System.Drawing.Color.Khaki
        Me.DgvLaporan.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvLaporan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvLaporan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulLaporan, Me.BacaLaporan})
        Me.DgvLaporan.Location = New System.Drawing.Point(94, 198)
        Me.DgvLaporan.Name = "DgvLaporan"
        Me.DgvLaporan.RowHeadersVisible = False
        Me.DgvLaporan.Size = New System.Drawing.Size(461, 482)
        Me.DgvLaporan.TabIndex = 136
        '
        'ModulLaporan
        '
        Me.ModulLaporan.HeaderText = "Modul"
        Me.ModulLaporan.Name = "ModulLaporan"
        '
        'BacaLaporan
        '
        Me.BacaLaporan.FillWeight = 50.0!
        Me.BacaLaporan.HeaderText = "Baca"
        Me.BacaLaporan.Name = "BacaLaporan"
        Me.BacaLaporan.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaLaporan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'DgvJurnal
        '
        Me.DgvJurnal.AllowUserToAddRows = False
        Me.DgvJurnal.AllowUserToDeleteRows = False
        Me.DgvJurnal.AllowUserToResizeColumns = False
        Me.DgvJurnal.AllowUserToResizeRows = False
        Me.DgvJurnal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvJurnal.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvJurnal.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvJurnal.BackgroundColor = System.Drawing.Color.Khaki
        Me.DgvJurnal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvJurnal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvJurnal.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulJurnal, Me.BacaJurnal, Me.TambahJurnal, Me.EditJurnal, Me.HapusJurnal})
        Me.DgvJurnal.Location = New System.Drawing.Point(55, 135)
        Me.DgvJurnal.Name = "DgvJurnal"
        Me.DgvJurnal.RowHeadersVisible = False
        Me.DgvJurnal.Size = New System.Drawing.Size(461, 482)
        Me.DgvJurnal.TabIndex = 136
        '
        'ModulJurnal
        '
        Me.ModulJurnal.HeaderText = "Modul"
        Me.ModulJurnal.Name = "ModulJurnal"
        '
        'BacaJurnal
        '
        Me.BacaJurnal.FillWeight = 50.0!
        Me.BacaJurnal.HeaderText = "Baca"
        Me.BacaJurnal.Name = "BacaJurnal"
        Me.BacaJurnal.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaJurnal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'TambahJurnal
        '
        Me.TambahJurnal.FillWeight = 50.0!
        Me.TambahJurnal.HeaderText = "Tambah"
        Me.TambahJurnal.Name = "TambahJurnal"
        Me.TambahJurnal.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TambahJurnal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'EditJurnal
        '
        Me.EditJurnal.FillWeight = 50.0!
        Me.EditJurnal.HeaderText = "Edit"
        Me.EditJurnal.Name = "EditJurnal"
        Me.EditJurnal.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.EditJurnal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'HapusJurnal
        '
        Me.HapusJurnal.FillWeight = 50.0!
        Me.HapusJurnal.HeaderText = "Hapus"
        Me.HapusJurnal.Name = "HapusJurnal"
        Me.HapusJurnal.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HapusJurnal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'DgvTransaksi
        '
        Me.DgvTransaksi.AllowUserToAddRows = False
        Me.DgvTransaksi.AllowUserToDeleteRows = False
        Me.DgvTransaksi.AllowUserToResizeColumns = False
        Me.DgvTransaksi.AllowUserToResizeRows = False
        Me.DgvTransaksi.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvTransaksi.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvTransaksi.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvTransaksi.BackgroundColor = System.Drawing.Color.Khaki
        Me.DgvTransaksi.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvTransaksi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvTransaksi.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulTransaksi, Me.BacaTransaksi, Me.TambahTransaksi, Me.EditTransaksi, Me.HapusTransaksi})
        Me.DgvTransaksi.Location = New System.Drawing.Point(29, 101)
        Me.DgvTransaksi.Name = "DgvTransaksi"
        Me.DgvTransaksi.RowHeadersVisible = False
        Me.DgvTransaksi.Size = New System.Drawing.Size(461, 482)
        Me.DgvTransaksi.TabIndex = 163
        '
        'ModulTransaksi
        '
        Me.ModulTransaksi.HeaderText = "Modul"
        Me.ModulTransaksi.Name = "ModulTransaksi"
        '
        'BacaTransaksi
        '
        Me.BacaTransaksi.FillWeight = 50.0!
        Me.BacaTransaksi.HeaderText = "Baca"
        Me.BacaTransaksi.Name = "BacaTransaksi"
        Me.BacaTransaksi.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaTransaksi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'TambahTransaksi
        '
        Me.TambahTransaksi.FillWeight = 50.0!
        Me.TambahTransaksi.HeaderText = "Tambah"
        Me.TambahTransaksi.Name = "TambahTransaksi"
        Me.TambahTransaksi.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TambahTransaksi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'EditTransaksi
        '
        Me.EditTransaksi.FillWeight = 50.0!
        Me.EditTransaksi.HeaderText = "Edit"
        Me.EditTransaksi.Name = "EditTransaksi"
        Me.EditTransaksi.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.EditTransaksi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'HapusTransaksi
        '
        Me.HapusTransaksi.FillWeight = 50.0!
        Me.HapusTransaksi.HeaderText = "Hapus"
        Me.HapusTransaksi.Name = "HapusTransaksi"
        Me.HapusTransaksi.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HapusTransaksi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'CmbUser
        '
        Me.CmbUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbUser.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbUser.FormattingEnabled = True
        Me.CmbUser.Items.AddRange(New Object() {"Master", "Admin", "Kasir", "Gudang"})
        Me.CmbUser.Location = New System.Drawing.Point(123, 40)
        Me.CmbUser.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbUser.Name = "CmbUser"
        Me.CmbUser.Size = New System.Drawing.Size(157, 25)
        Me.CmbUser.TabIndex = 123
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(4, 44)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 17)
        Me.Label1.TabIndex = 125
        Me.Label1.Text = "Kelompok User :"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(176, 6)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(240, 19)
        Me.Label2.TabIndex = 126
        Me.Label2.Text = "SETING HAK AKSES UNTUK USER"
        '
        'LblMasterData
        '
        Me.LblMasterData.BackColor = System.Drawing.Color.White
        Me.LblMasterData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblMasterData.Location = New System.Drawing.Point(7, 72)
        Me.LblMasterData.Name = "LblMasterData"
        Me.LblMasterData.Size = New System.Drawing.Size(99, 28)
        Me.LblMasterData.TabIndex = 127
        Me.LblMasterData.Text = "Master Data"
        Me.LblMasterData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblUtility
        '
        Me.LblUtility.BackColor = System.Drawing.Color.White
        Me.LblUtility.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblUtility.Location = New System.Drawing.Point(7, 240)
        Me.LblUtility.Name = "LblUtility"
        Me.LblUtility.Size = New System.Drawing.Size(99, 28)
        Me.LblUtility.TabIndex = 134
        Me.LblUtility.Text = "Utility"
        Me.LblUtility.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblLaporan
        '
        Me.LblLaporan.BackColor = System.Drawing.Color.White
        Me.LblLaporan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblLaporan.Location = New System.Drawing.Point(7, 206)
        Me.LblLaporan.Name = "LblLaporan"
        Me.LblLaporan.Size = New System.Drawing.Size(99, 28)
        Me.LblLaporan.TabIndex = 133
        Me.LblLaporan.Text = "Laporan"
        Me.LblLaporan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblJurnal
        '
        Me.LblJurnal.BackColor = System.Drawing.Color.White
        Me.LblJurnal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJurnal.Location = New System.Drawing.Point(7, 140)
        Me.LblJurnal.Name = "LblJurnal"
        Me.LblJurnal.Size = New System.Drawing.Size(99, 28)
        Me.LblJurnal.TabIndex = 132
        Me.LblJurnal.Text = "Jurnal"
        Me.LblJurnal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblTransaksi
        '
        Me.LblTransaksi.BackColor = System.Drawing.Color.White
        Me.LblTransaksi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTransaksi.Location = New System.Drawing.Point(7, 106)
        Me.LblTransaksi.Name = "LblTransaksi"
        Me.LblTransaksi.Size = New System.Drawing.Size(99, 28)
        Me.LblTransaksi.TabIndex = 131
        Me.LblTransaksi.Text = "Transaksi"
        Me.LblTransaksi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DGVMaster
        '
        Me.DGVMaster.AllowUserToAddRows = False
        Me.DGVMaster.AllowUserToDeleteRows = False
        Me.DGVMaster.AllowUserToResizeColumns = False
        Me.DGVMaster.AllowUserToResizeRows = False
        Me.DGVMaster.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DGVMaster.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVMaster.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVMaster.BackgroundColor = System.Drawing.Color.Khaki
        Me.DGVMaster.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DGVMaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVMaster.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulMasterData, Me.BacaMasterData, Me.TambahMasterData, Me.EditMasterData, Me.HapusMasterData})
        Me.DGVMaster.Location = New System.Drawing.Point(123, 72)
        Me.DGVMaster.Name = "DGVMaster"
        Me.DGVMaster.RowHeadersVisible = False
        Me.DGVMaster.Size = New System.Drawing.Size(451, 482)
        Me.DGVMaster.TabIndex = 135
        '
        'ModulMasterData
        '
        Me.ModulMasterData.HeaderText = "Modul"
        Me.ModulMasterData.Name = "ModulMasterData"
        '
        'BacaMasterData
        '
        Me.BacaMasterData.FillWeight = 50.0!
        Me.BacaMasterData.HeaderText = "Baca"
        Me.BacaMasterData.Name = "BacaMasterData"
        Me.BacaMasterData.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaMasterData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'TambahMasterData
        '
        Me.TambahMasterData.FillWeight = 50.0!
        Me.TambahMasterData.HeaderText = "Tambah"
        Me.TambahMasterData.Name = "TambahMasterData"
        Me.TambahMasterData.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TambahMasterData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'EditMasterData
        '
        Me.EditMasterData.FillWeight = 50.0!
        Me.EditMasterData.HeaderText = "Edit"
        Me.EditMasterData.Name = "EditMasterData"
        Me.EditMasterData.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.EditMasterData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'HapusMasterData
        '
        Me.HapusMasterData.FillWeight = 50.0!
        Me.HapusMasterData.HeaderText = "Hapus"
        Me.HapusMasterData.Name = "HapusMasterData"
        Me.HapusMasterData.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HapusMasterData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'ChkAll
        '
        Me.ChkAll.AutoSize = True
        Me.ChkAll.Location = New System.Drawing.Point(384, 44)
        Me.ChkAll.Name = "ChkAll"
        Me.ChkAll.Size = New System.Drawing.Size(84, 21)
        Me.ChkAll.TabIndex = 164
        Me.ChkAll.Text = "CheckAll"
        Me.ChkAll.UseVisualStyleBackColor = True
        '
        'ChkNonAll
        '
        Me.ChkNonAll.AutoSize = True
        Me.ChkNonAll.Location = New System.Drawing.Point(474, 44)
        Me.ChkNonAll.Name = "ChkNonAll"
        Me.ChkNonAll.Size = New System.Drawing.Size(100, 21)
        Me.ChkNonAll.TabIndex = 165
        Me.ChkNonAll.Text = "UnCheckAll"
        Me.ChkNonAll.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Panel1.AutoScroll = True
        Me.Panel1.BackColor = System.Drawing.Color.Khaki
        Me.Panel1.Controls.Add(Me.LblPosting)
        Me.Panel1.Controls.Add(Me.LblKaryawan)
        Me.Panel1.Controls.Add(Me.LblMasterData)
        Me.Panel1.Controls.Add(Me.LblUtility)
        Me.Panel1.Controls.Add(Me.LblTransaksi)
        Me.Panel1.Controls.Add(Me.LblJurnal)
        Me.Panel1.Controls.Add(Me.LblLaporan)
        Me.Panel1.Controls.Add(Me.BtnKeluar)
        Me.Panel1.Controls.Add(Me.BtnSimpan)
        Me.Panel1.Controls.Add(Me.DgvPosting)
        Me.Panel1.Controls.Add(Me.DgvUtility)
        Me.Panel1.Controls.Add(Me.DgvLaporan)
        Me.Panel1.Controls.Add(Me.DgvKaryawan)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.ChkNonAll)
        Me.Panel1.Controls.Add(Me.ChkAll)
        Me.Panel1.Controls.Add(Me.CmbUser)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.DgvJurnal)
        Me.Panel1.Controls.Add(Me.DgvTransaksi)
        Me.Panel1.Controls.Add(Me.DGVMaster)
        Me.Panel1.Location = New System.Drawing.Point(12, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(593, 618)
        Me.Panel1.TabIndex = 166
        '
        'LblPosting
        '
        Me.LblPosting.BackColor = System.Drawing.Color.White
        Me.LblPosting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblPosting.Location = New System.Drawing.Point(7, 274)
        Me.LblPosting.Name = "LblPosting"
        Me.LblPosting.Size = New System.Drawing.Size(99, 28)
        Me.LblPosting.TabIndex = 169
        Me.LblPosting.Text = "Posting"
        Me.LblPosting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblKaryawan
        '
        Me.LblKaryawan.BackColor = System.Drawing.Color.White
        Me.LblKaryawan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblKaryawan.Location = New System.Drawing.Point(7, 172)
        Me.LblKaryawan.Name = "LblKaryawan"
        Me.LblKaryawan.Size = New System.Drawing.Size(99, 28)
        Me.LblKaryawan.TabIndex = 166
        Me.LblKaryawan.Text = "Karyawan"
        Me.LblKaryawan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DgvPosting
        '
        Me.DgvPosting.AllowUserToAddRows = False
        Me.DgvPosting.AllowUserToDeleteRows = False
        Me.DgvPosting.AllowUserToResizeColumns = False
        Me.DgvPosting.AllowUserToResizeRows = False
        Me.DgvPosting.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvPosting.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvPosting.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvPosting.BackgroundColor = System.Drawing.Color.Khaki
        Me.DgvPosting.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvPosting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvPosting.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulPosting, Me.BacaPosting})
        Me.DgvPosting.Location = New System.Drawing.Point(138, 262)
        Me.DgvPosting.Name = "DgvPosting"
        Me.DgvPosting.RowHeadersVisible = False
        Me.DgvPosting.Size = New System.Drawing.Size(461, 93)
        Me.DgvPosting.TabIndex = 168
        '
        'ModulPosting
        '
        Me.ModulPosting.HeaderText = "Modul"
        Me.ModulPosting.Name = "ModulPosting"
        '
        'BacaPosting
        '
        Me.BacaPosting.FillWeight = 50.0!
        Me.BacaPosting.HeaderText = "Baca"
        Me.BacaPosting.Name = "BacaPosting"
        Me.BacaPosting.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaPosting.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'DgvKaryawan
        '
        Me.DgvKaryawan.AllowUserToAddRows = False
        Me.DgvKaryawan.AllowUserToDeleteRows = False
        Me.DgvKaryawan.AllowUserToResizeColumns = False
        Me.DgvKaryawan.AllowUserToResizeRows = False
        Me.DgvKaryawan.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvKaryawan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvKaryawan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvKaryawan.BackgroundColor = System.Drawing.Color.Khaki
        Me.DgvKaryawan.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvKaryawan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvKaryawan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulKaryawan, Me.BacaKaryawan, Me.TambahKaryawan, Me.EditKaryawan, Me.HapusKaryawan})
        Me.DgvKaryawan.Location = New System.Drawing.Point(72, 166)
        Me.DgvKaryawan.Name = "DgvKaryawan"
        Me.DgvKaryawan.RowHeadersVisible = False
        Me.DgvKaryawan.Size = New System.Drawing.Size(461, 482)
        Me.DgvKaryawan.TabIndex = 167
        '
        'ModulKaryawan
        '
        Me.ModulKaryawan.HeaderText = "Modul"
        Me.ModulKaryawan.Name = "ModulKaryawan"
        '
        'BacaKaryawan
        '
        Me.BacaKaryawan.FillWeight = 50.0!
        Me.BacaKaryawan.HeaderText = "Baca"
        Me.BacaKaryawan.Name = "BacaKaryawan"
        Me.BacaKaryawan.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BacaKaryawan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'TambahKaryawan
        '
        Me.TambahKaryawan.FillWeight = 50.0!
        Me.TambahKaryawan.HeaderText = "Tambah"
        Me.TambahKaryawan.Name = "TambahKaryawan"
        Me.TambahKaryawan.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TambahKaryawan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'EditKaryawan
        '
        Me.EditKaryawan.FillWeight = 50.0!
        Me.EditKaryawan.HeaderText = "Edit"
        Me.EditKaryawan.Name = "EditKaryawan"
        Me.EditKaryawan.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.EditKaryawan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'HapusKaryawan
        '
        Me.HapusKaryawan.FillWeight = 50.0!
        Me.HapusKaryawan.HeaderText = "Hapus"
        Me.HapusKaryawan.Name = "HapusKaryawan"
        Me.HapusKaryawan.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HapusKaryawan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.LblBeliMuculJual)
        Me.GroupBox2.Controls.Add(Me.CmbBeliMuculJual)
        Me.GroupBox2.Controls.Add(Me.LblBeliEditHarga)
        Me.GroupBox2.Controls.Add(Me.CmbBeliEditHarga)
        Me.GroupBox2.Controls.Add(Me.LblBeliAverage)
        Me.GroupBox2.Controls.Add(Me.CmbBeliAverage)
        Me.GroupBox2.Controls.Add(Me.LblBeliUpdate)
        Me.GroupBox2.Controls.Add(Me.CmbBeliUpdate)
        Me.GroupBox2.Controls.Add(Me.LblBeliRugi)
        Me.GroupBox2.Controls.Add(Me.CmbBeliRugi)
        Me.GroupBox2.Controls.Add(Me.LblBeliSatuan)
        Me.GroupBox2.Controls.Add(Me.CmbBeliSatuan)
        Me.GroupBox2.Controls.Add(Me.LblBeliFokus)
        Me.GroupBox2.Controls.Add(Me.CmbBeliFokus)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(570, 233)
        Me.GroupBox2.TabIndex = 131
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Pembelian"
        '
        'LblBeliMuculJual
        '
        Me.LblBeliMuculJual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliMuculJual.Location = New System.Drawing.Point(6, 136)
        Me.LblBeliMuculJual.Name = "LblBeliMuculJual"
        Me.LblBeliMuculJual.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliMuculJual.TabIndex = 133
        Me.LblBeliMuculJual.Text = "Edit harga beli muncul edit harga jual"
        Me.LblBeliMuculJual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliMuculJual
        '
        Me.CmbBeliMuculJual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliMuculJual.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliMuculJual.FormattingEnabled = True
        Me.CmbBeliMuculJual.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbBeliMuculJual.Location = New System.Drawing.Point(330, 138)
        Me.CmbBeliMuculJual.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliMuculJual.Name = "CmbBeliMuculJual"
        Me.CmbBeliMuculJual.Size = New System.Drawing.Size(214, 25)
        Me.CmbBeliMuculJual.TabIndex = 132
        '
        'LblBeliEditHarga
        '
        Me.LblBeliEditHarga.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliEditHarga.Location = New System.Drawing.Point(6, 105)
        Me.LblBeliEditHarga.Name = "LblBeliEditHarga"
        Me.LblBeliEditHarga.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliEditHarga.TabIndex = 131
        Me.LblBeliEditHarga.Text = "User diperbolehkan edit harga beli"
        Me.LblBeliEditHarga.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliEditHarga
        '
        Me.CmbBeliEditHarga.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliEditHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliEditHarga.FormattingEnabled = True
        Me.CmbBeliEditHarga.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbBeliEditHarga.Location = New System.Drawing.Point(330, 107)
        Me.CmbBeliEditHarga.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliEditHarga.Name = "CmbBeliEditHarga"
        Me.CmbBeliEditHarga.Size = New System.Drawing.Size(214, 25)
        Me.CmbBeliEditHarga.TabIndex = 130
        '
        'LblBeliAverage
        '
        Me.LblBeliAverage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliAverage.Location = New System.Drawing.Point(6, 197)
        Me.LblBeliAverage.Name = "LblBeliAverage"
        Me.LblBeliAverage.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliAverage.TabIndex = 129
        Me.LblBeliAverage.Text = "Hitung average berdasarkan dari stok"
        Me.LblBeliAverage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliAverage
        '
        Me.CmbBeliAverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliAverage.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliAverage.FormattingEnabled = True
        Me.CmbBeliAverage.Items.AddRange(New Object() {"Toko", "Gudang", "Toko dan Gudang"})
        Me.CmbBeliAverage.Location = New System.Drawing.Point(329, 199)
        Me.CmbBeliAverage.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliAverage.Name = "CmbBeliAverage"
        Me.CmbBeliAverage.Size = New System.Drawing.Size(215, 25)
        Me.CmbBeliAverage.TabIndex = 128
        '
        'LblBeliUpdate
        '
        Me.LblBeliUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliUpdate.Location = New System.Drawing.Point(6, 168)
        Me.LblBeliUpdate.Name = "LblBeliUpdate"
        Me.LblBeliUpdate.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliUpdate.TabIndex = 129
        Me.LblBeliUpdate.Text = "Update harga beli saat pembelian"
        Me.LblBeliUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliUpdate
        '
        Me.CmbBeliUpdate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliUpdate.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliUpdate.FormattingEnabled = True
        Me.CmbBeliUpdate.Items.AddRange(New Object() {"Harga Terbaru", "Metode Average (Rata - Rata)", "Tidak Ada"})
        Me.CmbBeliUpdate.Location = New System.Drawing.Point(329, 170)
        Me.CmbBeliUpdate.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliUpdate.Name = "CmbBeliUpdate"
        Me.CmbBeliUpdate.Size = New System.Drawing.Size(215, 25)
        Me.CmbBeliUpdate.TabIndex = 128
        '
        'LblBeliRugi
        '
        Me.LblBeliRugi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliRugi.Location = New System.Drawing.Point(6, 76)
        Me.LblBeliRugi.Name = "LblBeliRugi"
        Me.LblBeliRugi.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliRugi.TabIndex = 129
        Me.LblBeliRugi.Text = "Pengecekan harga jual di bawah harga beli"
        Me.LblBeliRugi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliRugi
        '
        Me.CmbBeliRugi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliRugi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliRugi.FormattingEnabled = True
        Me.CmbBeliRugi.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbBeliRugi.Location = New System.Drawing.Point(329, 78)
        Me.CmbBeliRugi.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliRugi.Name = "CmbBeliRugi"
        Me.CmbBeliRugi.Size = New System.Drawing.Size(215, 25)
        Me.CmbBeliRugi.TabIndex = 128
        '
        'LblBeliSatuan
        '
        Me.LblBeliSatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliSatuan.Location = New System.Drawing.Point(6, 47)
        Me.LblBeliSatuan.Name = "LblBeliSatuan"
        Me.LblBeliSatuan.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliSatuan.TabIndex = 129
        Me.LblBeliSatuan.Text = "Membeli kode barang yang sama beda satuan"
        Me.LblBeliSatuan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliSatuan
        '
        Me.CmbBeliSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliSatuan.FormattingEnabled = True
        Me.CmbBeliSatuan.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbBeliSatuan.Location = New System.Drawing.Point(329, 49)
        Me.CmbBeliSatuan.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliSatuan.Name = "CmbBeliSatuan"
        Me.CmbBeliSatuan.Size = New System.Drawing.Size(215, 25)
        Me.CmbBeliSatuan.TabIndex = 128
        '
        'LblBeliFokus
        '
        Me.LblBeliFokus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblBeliFokus.Location = New System.Drawing.Point(6, 18)
        Me.LblBeliFokus.Name = "LblBeliFokus"
        Me.LblBeliFokus.Size = New System.Drawing.Size(317, 28)
        Me.LblBeliFokus.TabIndex = 129
        Me.LblBeliFokus.Text = "Saat pembelian di buka fokus"
        Me.LblBeliFokus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBeliFokus
        '
        Me.CmbBeliFokus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBeliFokus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBeliFokus.FormattingEnabled = True
        Me.CmbBeliFokus.Items.AddRange(New Object() {"Pencarian", "Kolom data"})
        Me.CmbBeliFokus.Location = New System.Drawing.Point(329, 20)
        Me.CmbBeliFokus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbBeliFokus.Name = "CmbBeliFokus"
        Me.CmbBeliFokus.Size = New System.Drawing.Size(215, 25)
        Me.CmbBeliFokus.TabIndex = 128
        '
        'Panel2
        '
        Me.Panel2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel2.AutoScroll = True
        Me.Panel2.Controls.Add(Me.GroupBox4)
        Me.Panel2.Controls.Add(Me.GroupBox3)
        Me.Panel2.Controls.Add(Me.GroupBox1)
        Me.Panel2.Controls.Add(Me.GroupBox2)
        Me.Panel2.Location = New System.Drawing.Point(611, 24)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(587, 606)
        Me.Panel2.TabIndex = 167
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.LblReturMinus)
        Me.GroupBox4.Controls.Add(Me.CmbReturMinus)
        Me.GroupBox4.Controls.Add(Me.LblReturSatuan)
        Me.GroupBox4.Controls.Add(Me.CmbReturSatuan)
        Me.GroupBox4.Controls.Add(Me.LblReturFokus)
        Me.GroupBox4.Controls.Add(Me.CmbReturFokus)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.Location = New System.Drawing.Point(0, 552)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(570, 113)
        Me.GroupBox4.TabIndex = 135
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Retur Pembelian"
        '
        'LblReturMinus
        '
        Me.LblReturMinus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblReturMinus.Location = New System.Drawing.Point(6, 81)
        Me.LblReturMinus.Name = "LblReturMinus"
        Me.LblReturMinus.Size = New System.Drawing.Size(317, 28)
        Me.LblReturMinus.TabIndex = 129
        Me.LblReturMinus.Text = "Perbolehkan retur stok minus"
        Me.LblReturMinus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbReturMinus
        '
        Me.CmbReturMinus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbReturMinus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbReturMinus.FormattingEnabled = True
        Me.CmbReturMinus.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbReturMinus.Location = New System.Drawing.Point(330, 83)
        Me.CmbReturMinus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbReturMinus.Name = "CmbReturMinus"
        Me.CmbReturMinus.Size = New System.Drawing.Size(214, 25)
        Me.CmbReturMinus.TabIndex = 128
        '
        'LblReturSatuan
        '
        Me.LblReturSatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblReturSatuan.Location = New System.Drawing.Point(6, 50)
        Me.LblReturSatuan.Name = "LblReturSatuan"
        Me.LblReturSatuan.Size = New System.Drawing.Size(317, 28)
        Me.LblReturSatuan.TabIndex = 129
        Me.LblReturSatuan.Text = "Retur kode barang yang sama beda satuan"
        Me.LblReturSatuan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbReturSatuan
        '
        Me.CmbReturSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbReturSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbReturSatuan.FormattingEnabled = True
        Me.CmbReturSatuan.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbReturSatuan.Location = New System.Drawing.Point(330, 52)
        Me.CmbReturSatuan.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbReturSatuan.Name = "CmbReturSatuan"
        Me.CmbReturSatuan.Size = New System.Drawing.Size(214, 25)
        Me.CmbReturSatuan.TabIndex = 128
        '
        'LblReturFokus
        '
        Me.LblReturFokus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblReturFokus.Location = New System.Drawing.Point(6, 21)
        Me.LblReturFokus.Name = "LblReturFokus"
        Me.LblReturFokus.Size = New System.Drawing.Size(317, 28)
        Me.LblReturFokus.TabIndex = 129
        Me.LblReturFokus.Text = "Saat retur di buka fokus"
        Me.LblReturFokus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbReturFokus
        '
        Me.CmbReturFokus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbReturFokus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbReturFokus.FormattingEnabled = True
        Me.CmbReturFokus.Items.AddRange(New Object() {"Pencarian", "Kolom data"})
        Me.CmbReturFokus.Location = New System.Drawing.Point(330, 23)
        Me.CmbReturFokus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbReturFokus.Name = "CmbReturFokus"
        Me.CmbReturFokus.Size = New System.Drawing.Size(214, 25)
        Me.CmbReturFokus.TabIndex = 128
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.LblTransferMinus)
        Me.GroupBox3.Controls.Add(Me.CmbTransferMinus)
        Me.GroupBox3.Controls.Add(Me.LblTransferSatuan)
        Me.GroupBox3.Controls.Add(Me.CmbTransferSatuan)
        Me.GroupBox3.Controls.Add(Me.LblTransferFocus)
        Me.GroupBox3.Controls.Add(Me.CmbTransferFocus)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(0, 439)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(570, 113)
        Me.GroupBox3.TabIndex = 134
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Transfer barang"
        '
        'LblTransferMinus
        '
        Me.LblTransferMinus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTransferMinus.Location = New System.Drawing.Point(6, 81)
        Me.LblTransferMinus.Name = "LblTransferMinus"
        Me.LblTransferMinus.Size = New System.Drawing.Size(317, 28)
        Me.LblTransferMinus.TabIndex = 129
        Me.LblTransferMinus.Text = "Perbolehkan transfer stok minus"
        Me.LblTransferMinus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbTransferMinus
        '
        Me.CmbTransferMinus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbTransferMinus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbTransferMinus.FormattingEnabled = True
        Me.CmbTransferMinus.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbTransferMinus.Location = New System.Drawing.Point(330, 83)
        Me.CmbTransferMinus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbTransferMinus.Name = "CmbTransferMinus"
        Me.CmbTransferMinus.Size = New System.Drawing.Size(214, 25)
        Me.CmbTransferMinus.TabIndex = 128
        '
        'LblTransferSatuan
        '
        Me.LblTransferSatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTransferSatuan.Location = New System.Drawing.Point(6, 50)
        Me.LblTransferSatuan.Name = "LblTransferSatuan"
        Me.LblTransferSatuan.Size = New System.Drawing.Size(317, 28)
        Me.LblTransferSatuan.TabIndex = 129
        Me.LblTransferSatuan.Text = "Transfer kode barang yang sama beda satuan"
        Me.LblTransferSatuan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbTransferSatuan
        '
        Me.CmbTransferSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbTransferSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbTransferSatuan.FormattingEnabled = True
        Me.CmbTransferSatuan.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbTransferSatuan.Location = New System.Drawing.Point(330, 52)
        Me.CmbTransferSatuan.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbTransferSatuan.Name = "CmbTransferSatuan"
        Me.CmbTransferSatuan.Size = New System.Drawing.Size(214, 25)
        Me.CmbTransferSatuan.TabIndex = 128
        '
        'LblTransferFocus
        '
        Me.LblTransferFocus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTransferFocus.Location = New System.Drawing.Point(6, 21)
        Me.LblTransferFocus.Name = "LblTransferFocus"
        Me.LblTransferFocus.Size = New System.Drawing.Size(317, 28)
        Me.LblTransferFocus.TabIndex = 129
        Me.LblTransferFocus.Text = "Saat Transfer barang di buka fokus"
        Me.LblTransferFocus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbTransferFocus
        '
        Me.CmbTransferFocus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbTransferFocus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbTransferFocus.FormattingEnabled = True
        Me.CmbTransferFocus.Items.AddRange(New Object() {"Pencarian", "Kolom data"})
        Me.CmbTransferFocus.Location = New System.Drawing.Point(330, 23)
        Me.CmbTransferFocus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbTransferFocus.Name = "CmbTransferFocus"
        Me.CmbTransferFocus.Size = New System.Drawing.Size(214, 25)
        Me.CmbTransferFocus.TabIndex = 128
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.LblEditHargaJual)
        Me.GroupBox1.Controls.Add(Me.CmbEditHargaJual)
        Me.GroupBox1.Controls.Add(Me.LblJualEditHarga)
        Me.GroupBox1.Controls.Add(Me.CmbJualEditHarga)
        Me.GroupBox1.Controls.Add(Me.LblJualMinus)
        Me.GroupBox1.Controls.Add(Me.CmbJualMinus)
        Me.GroupBox1.Controls.Add(Me.LblJualRugi)
        Me.GroupBox1.Controls.Add(Me.CmbJualRugi)
        Me.GroupBox1.Controls.Add(Me.LblJualSatuan)
        Me.GroupBox1.Controls.Add(Me.CmbJualSatuan)
        Me.GroupBox1.Controls.Add(Me.LblJualFokus)
        Me.GroupBox1.Controls.Add(Me.CmbJualFokus)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(0, 233)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(570, 206)
        Me.GroupBox1.TabIndex = 133
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Penjualan"
        '
        'LblJualEditHarga
        '
        Me.LblJualEditHarga.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJualEditHarga.Location = New System.Drawing.Point(6, 79)
        Me.LblJualEditHarga.Name = "LblJualEditHarga"
        Me.LblJualEditHarga.Size = New System.Drawing.Size(317, 28)
        Me.LblJualEditHarga.TabIndex = 129
        Me.LblJualEditHarga.Text = "User diperbolehkan edit harga jual"
        Me.LblJualEditHarga.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbJualEditHarga
        '
        Me.CmbJualEditHarga.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJualEditHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJualEditHarga.FormattingEnabled = True
        Me.CmbJualEditHarga.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbJualEditHarga.Location = New System.Drawing.Point(330, 81)
        Me.CmbJualEditHarga.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbJualEditHarga.Name = "CmbJualEditHarga"
        Me.CmbJualEditHarga.Size = New System.Drawing.Size(214, 25)
        Me.CmbJualEditHarga.TabIndex = 128
        '
        'LblJualMinus
        '
        Me.LblJualMinus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJualMinus.Location = New System.Drawing.Point(6, 137)
        Me.LblJualMinus.Name = "LblJualMinus"
        Me.LblJualMinus.Size = New System.Drawing.Size(317, 28)
        Me.LblJualMinus.TabIndex = 129
        Me.LblJualMinus.Text = "Perbolehkan penjualan stok minus"
        Me.LblJualMinus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbJualMinus
        '
        Me.CmbJualMinus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJualMinus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJualMinus.FormattingEnabled = True
        Me.CmbJualMinus.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbJualMinus.Location = New System.Drawing.Point(330, 139)
        Me.CmbJualMinus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbJualMinus.Name = "CmbJualMinus"
        Me.CmbJualMinus.Size = New System.Drawing.Size(214, 25)
        Me.CmbJualMinus.TabIndex = 128
        '
        'LblJualRugi
        '
        Me.LblJualRugi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJualRugi.Location = New System.Drawing.Point(6, 108)
        Me.LblJualRugi.Name = "LblJualRugi"
        Me.LblJualRugi.Size = New System.Drawing.Size(317, 28)
        Me.LblJualRugi.TabIndex = 129
        Me.LblJualRugi.Text = "Perbolehkan penjualan barang rugi"
        Me.LblJualRugi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbJualRugi
        '
        Me.CmbJualRugi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJualRugi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJualRugi.FormattingEnabled = True
        Me.CmbJualRugi.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbJualRugi.Location = New System.Drawing.Point(330, 110)
        Me.CmbJualRugi.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbJualRugi.Name = "CmbJualRugi"
        Me.CmbJualRugi.Size = New System.Drawing.Size(214, 25)
        Me.CmbJualRugi.TabIndex = 128
        '
        'LblJualSatuan
        '
        Me.LblJualSatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJualSatuan.Location = New System.Drawing.Point(6, 50)
        Me.LblJualSatuan.Name = "LblJualSatuan"
        Me.LblJualSatuan.Size = New System.Drawing.Size(317, 28)
        Me.LblJualSatuan.TabIndex = 129
        Me.LblJualSatuan.Text = "Menjual kode barang yang sama beda satuan"
        Me.LblJualSatuan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbJualSatuan
        '
        Me.CmbJualSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJualSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJualSatuan.FormattingEnabled = True
        Me.CmbJualSatuan.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbJualSatuan.Location = New System.Drawing.Point(330, 52)
        Me.CmbJualSatuan.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbJualSatuan.Name = "CmbJualSatuan"
        Me.CmbJualSatuan.Size = New System.Drawing.Size(214, 25)
        Me.CmbJualSatuan.TabIndex = 128
        '
        'LblJualFokus
        '
        Me.LblJualFokus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJualFokus.Location = New System.Drawing.Point(6, 21)
        Me.LblJualFokus.Name = "LblJualFokus"
        Me.LblJualFokus.Size = New System.Drawing.Size(317, 28)
        Me.LblJualFokus.TabIndex = 129
        Me.LblJualFokus.Text = "Saat penjualan di buka fokus"
        Me.LblJualFokus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbJualFokus
        '
        Me.CmbJualFokus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJualFokus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJualFokus.FormattingEnabled = True
        Me.CmbJualFokus.Items.AddRange(New Object() {"Pencarian", "Kolom data"})
        Me.CmbJualFokus.Location = New System.Drawing.Point(330, 23)
        Me.CmbJualFokus.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbJualFokus.Name = "CmbJualFokus"
        Me.CmbJualFokus.Size = New System.Drawing.Size(214, 25)
        Me.CmbJualFokus.TabIndex = 128
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(769, 4)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(263, 19)
        Me.Label3.TabIndex = 132
        Me.Label3.Text = "GENERAL SETTING UNTUK ALL USER"
        '
        'LblEditHargaJual
        '
        Me.LblEditHargaJual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblEditHargaJual.Location = New System.Drawing.Point(6, 166)
        Me.LblEditHargaJual.Name = "LblEditHargaJual"
        Me.LblEditHargaJual.Size = New System.Drawing.Size(317, 28)
        Me.LblEditHargaJual.TabIndex = 131
        Me.LblEditHargaJual.Text = "Edit master barang harga jual"
        Me.LblEditHargaJual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbEditHargaJual
        '
        Me.CmbEditHargaJual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbEditHargaJual.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbEditHargaJual.FormattingEnabled = True
        Me.CmbEditHargaJual.Items.AddRange(New Object() {"Iya", "Tidak"})
        Me.CmbEditHargaJual.Location = New System.Drawing.Point(330, 168)
        Me.CmbEditHargaJual.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbEditHargaJual.Name = "CmbEditHargaJual"
        Me.CmbEditHargaJual.Size = New System.Drawing.Size(214, 25)
        Me.CmbEditHargaJual.TabIndex = 130
        '
        'FormHakUser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1203, 637)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Label3)
        Me.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "FormHakUser"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "FormHakUser"
        CType(Me.DgvUtility, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DgvLaporan, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DgvJurnal, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DgvTransaksi, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVMaster, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DgvPosting, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DgvKaryawan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents CmbUser As System.Windows.Forms.ComboBox
    Friend WithEvents BtnKeluar As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents LblMasterData As System.Windows.Forms.Label
    Friend WithEvents LblUtility As System.Windows.Forms.Label
    Friend WithEvents LblLaporan As System.Windows.Forms.Label
    Friend WithEvents LblJurnal As System.Windows.Forms.Label
    Friend WithEvents LblTransaksi As System.Windows.Forms.Label
    Friend WithEvents DGVMaster As System.Windows.Forms.DataGridView
    Friend WithEvents ModulMasterData As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaMasterData As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TambahMasterData As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents EditMasterData As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents HapusMasterData As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents DgvTransaksi As System.Windows.Forms.DataGridView
    Friend WithEvents ModulTransaksi As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaTransaksi As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TambahTransaksi As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents EditTransaksi As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents HapusTransaksi As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents DgvUtility As System.Windows.Forms.DataGridView
    Friend WithEvents DgvLaporan As System.Windows.Forms.DataGridView
    Friend WithEvents DgvJurnal As System.Windows.Forms.DataGridView
    Friend WithEvents ModulJurnal As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaJurnal As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TambahJurnal As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents EditJurnal As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents HapusJurnal As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ChkAll As System.Windows.Forms.CheckBox
    Friend WithEvents ChkNonAll As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ModulLaporan As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaLaporan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ModulUtility As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaUtility As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents LblBeliAverage As System.Windows.Forms.Label
    Friend WithEvents CmbBeliAverage As System.Windows.Forms.ComboBox
    Friend WithEvents LblBeliUpdate As System.Windows.Forms.Label
    Friend WithEvents CmbBeliUpdate As System.Windows.Forms.ComboBox
    Friend WithEvents LblBeliRugi As System.Windows.Forms.Label
    Friend WithEvents CmbBeliRugi As System.Windows.Forms.ComboBox
    Friend WithEvents LblBeliSatuan As System.Windows.Forms.Label
    Friend WithEvents CmbBeliSatuan As System.Windows.Forms.ComboBox
    Friend WithEvents LblBeliFokus As System.Windows.Forms.Label
    Friend WithEvents CmbBeliFokus As System.Windows.Forms.ComboBox
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents LblJualEditHarga As System.Windows.Forms.Label
    Friend WithEvents CmbJualEditHarga As System.Windows.Forms.ComboBox
    Friend WithEvents LblJualMinus As System.Windows.Forms.Label
    Friend WithEvents CmbJualMinus As System.Windows.Forms.ComboBox
    Friend WithEvents LblJualRugi As System.Windows.Forms.Label
    Friend WithEvents CmbJualRugi As System.Windows.Forms.ComboBox
    Friend WithEvents LblJualSatuan As System.Windows.Forms.Label
    Friend WithEvents CmbJualSatuan As System.Windows.Forms.ComboBox
    Friend WithEvents LblJualFokus As System.Windows.Forms.Label
    Friend WithEvents CmbJualFokus As System.Windows.Forms.ComboBox
    Friend WithEvents LblBeliEditHarga As System.Windows.Forms.Label
    Friend WithEvents CmbBeliEditHarga As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents LblTransferMinus As System.Windows.Forms.Label
    Friend WithEvents CmbTransferMinus As System.Windows.Forms.ComboBox
    Friend WithEvents LblTransferSatuan As System.Windows.Forms.Label
    Friend WithEvents CmbTransferSatuan As System.Windows.Forms.ComboBox
    Friend WithEvents LblTransferFocus As System.Windows.Forms.Label
    Friend WithEvents CmbTransferFocus As System.Windows.Forms.ComboBox
    Friend WithEvents LblKaryawan As System.Windows.Forms.Label
    Friend WithEvents DgvKaryawan As System.Windows.Forms.DataGridView
    Friend WithEvents ModulKaryawan As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TambahKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents EditKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents HapusKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents LblBeliMuculJual As System.Windows.Forms.Label
    Friend WithEvents CmbBeliMuculJual As System.Windows.Forms.ComboBox
    Friend WithEvents DgvPosting As DataGridView
    Friend WithEvents LblPosting As Label
    Friend WithEvents ModulPosting As DataGridViewTextBoxColumn
    Friend WithEvents BacaPosting As DataGridViewCheckBoxColumn
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents LblReturMinus As Label
    Friend WithEvents CmbReturMinus As ComboBox
    Friend WithEvents LblReturSatuan As Label
    Friend WithEvents CmbReturSatuan As ComboBox
    Friend WithEvents LblReturFokus As Label
    Friend WithEvents CmbReturFokus As ComboBox
    Friend WithEvents LblEditHargaJual As Label
    Friend WithEvents CmbEditHargaJual As ComboBox
End Class
