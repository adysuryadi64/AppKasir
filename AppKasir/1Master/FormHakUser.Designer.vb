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
        Me.PanelInput = New System.Windows.Forms.Panel()
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
        Me.LblHeader = New System.Windows.Forms.Label()
        CType(Me.DgvUtility, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvLaporan, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvJurnal, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvTransaksi, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVMaster, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelInput.SuspendLayout()
        CType(Me.DgvPosting, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvKaryawan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnSimpan
        '
        Me.BtnSimpan.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnSimpan.AutoSize = True
        Me.BtnSimpan.BackColor = System.Drawing.Color.White
        Me.BtnSimpan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpan.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.Location = New System.Drawing.Point(217, 476)
        Me.BtnSimpan.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(169, 32)
        Me.BtnSimpan.TabIndex = 37
        Me.BtnSimpan.Text = "Simpan (F2)"
        Me.BtnSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'BtnKeluar
        '
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluar.AutoSize = True
        Me.BtnKeluar.BackColor = System.Drawing.Color.White
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.Location = New System.Drawing.Point(415, 476)
        Me.BtnKeluar.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(140, 32)
        Me.BtnKeluar.TabIndex = 124
        Me.BtnKeluar.Text = "Keluar (Esc)"
        Me.BtnKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
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
        Me.DgvUtility.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvUtility.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvUtility.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulUtility, Me.BacaUtility})
        Me.DgvUtility.Location = New System.Drawing.Point(120, 196)
        Me.DgvUtility.Name = "DgvUtility"
        Me.DgvUtility.RowHeadersVisible = False
        Me.DgvUtility.Size = New System.Drawing.Size(461, 447)
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
        Me.DgvLaporan.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvLaporan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvLaporan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulLaporan, Me.BacaLaporan})
        Me.DgvLaporan.Location = New System.Drawing.Point(94, 162)
        Me.DgvLaporan.Name = "DgvLaporan"
        Me.DgvLaporan.RowHeadersVisible = False
        Me.DgvLaporan.Size = New System.Drawing.Size(461, 447)
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
        Me.DgvJurnal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvJurnal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvJurnal.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulJurnal, Me.BacaJurnal, Me.TambahJurnal, Me.EditJurnal, Me.HapusJurnal})
        Me.DgvJurnal.Location = New System.Drawing.Point(55, 99)
        Me.DgvJurnal.Name = "DgvJurnal"
        Me.DgvJurnal.RowHeadersVisible = False
        Me.DgvJurnal.Size = New System.Drawing.Size(461, 447)
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
        Me.DgvTransaksi.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvTransaksi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvTransaksi.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulTransaksi, Me.BacaTransaksi, Me.TambahTransaksi, Me.EditTransaksi, Me.HapusTransaksi})
        Me.DgvTransaksi.Location = New System.Drawing.Point(29, 65)
        Me.DgvTransaksi.Name = "DgvTransaksi"
        Me.DgvTransaksi.RowHeadersVisible = False
        Me.DgvTransaksi.Size = New System.Drawing.Size(461, 447)
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
        Me.CmbUser.Items.AddRange(New Object() {"Owner", "Master", "Admin", "Kasir", "Gudang"})
        Me.CmbUser.Location = New System.Drawing.Point(123, 4)
        Me.CmbUser.Margin = New System.Windows.Forms.Padding(4)
        Me.CmbUser.Name = "CmbUser"
        Me.CmbUser.Size = New System.Drawing.Size(157, 25)
        Me.CmbUser.TabIndex = 123
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(4, 8)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 17)
        Me.Label1.TabIndex = 125
        Me.Label1.Text = "Kelompok User :"
        '
        'LblMasterData
        '
        Me.LblMasterData.BackColor = System.Drawing.Color.White
        Me.LblMasterData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblMasterData.Location = New System.Drawing.Point(7, 36)
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
        Me.LblUtility.Location = New System.Drawing.Point(7, 204)
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
        Me.LblLaporan.Location = New System.Drawing.Point(7, 170)
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
        Me.LblJurnal.Location = New System.Drawing.Point(7, 104)
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
        Me.LblTransaksi.Location = New System.Drawing.Point(7, 70)
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
        Me.DGVMaster.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DGVMaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVMaster.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulMasterData, Me.BacaMasterData, Me.TambahMasterData, Me.EditMasterData, Me.HapusMasterData})
        Me.DGVMaster.Location = New System.Drawing.Point(123, 36)
        Me.DGVMaster.Name = "DGVMaster"
        Me.DGVMaster.RowHeadersVisible = False
        Me.DGVMaster.Size = New System.Drawing.Size(451, 447)
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
        Me.ChkAll.Location = New System.Drawing.Point(384, 8)
        Me.ChkAll.Name = "ChkAll"
        Me.ChkAll.Size = New System.Drawing.Size(75, 21)
        Me.ChkAll.TabIndex = 164
        Me.ChkAll.Text = "CheckAll"
        Me.ChkAll.UseVisualStyleBackColor = True
        '
        'ChkNonAll
        '
        Me.ChkNonAll.AutoSize = True
        Me.ChkNonAll.Location = New System.Drawing.Point(474, 8)
        Me.ChkNonAll.Name = "ChkNonAll"
        Me.ChkNonAll.Size = New System.Drawing.Size(91, 21)
        Me.ChkNonAll.TabIndex = 165
        Me.ChkNonAll.Text = "UnCheckAll"
        Me.ChkNonAll.UseVisualStyleBackColor = True
        '
        'PanelInput
        '
        Me.PanelInput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PanelInput.AutoScroll = True
        Me.PanelInput.BackColor = System.Drawing.SystemColors.Control
        Me.PanelInput.Controls.Add(Me.LblPosting)
        Me.PanelInput.Controls.Add(Me.LblKaryawan)
        Me.PanelInput.Controls.Add(Me.LblMasterData)
        Me.PanelInput.Controls.Add(Me.LblUtility)
        Me.PanelInput.Controls.Add(Me.LblTransaksi)
        Me.PanelInput.Controls.Add(Me.LblJurnal)
        Me.PanelInput.Controls.Add(Me.LblLaporan)
        Me.PanelInput.Controls.Add(Me.BtnKeluar)
        Me.PanelInput.Controls.Add(Me.BtnSimpan)
        Me.PanelInput.Controls.Add(Me.DgvPosting)
        Me.PanelInput.Controls.Add(Me.DgvUtility)
        Me.PanelInput.Controls.Add(Me.DgvLaporan)
        Me.PanelInput.Controls.Add(Me.DgvKaryawan)
        Me.PanelInput.Controls.Add(Me.ChkNonAll)
        Me.PanelInput.Controls.Add(Me.ChkAll)
        Me.PanelInput.Controls.Add(Me.CmbUser)
        Me.PanelInput.Controls.Add(Me.Label1)
        Me.PanelInput.Controls.Add(Me.DgvJurnal)
        Me.PanelInput.Controls.Add(Me.DgvTransaksi)
        Me.PanelInput.Controls.Add(Me.DGVMaster)
        Me.PanelInput.Location = New System.Drawing.Point(12, 64)
        Me.PanelInput.Name = "PanelInput"
        Me.PanelInput.Size = New System.Drawing.Size(703, 566)
        Me.PanelInput.TabIndex = 166
        '
        'LblPosting
        '
        Me.LblPosting.BackColor = System.Drawing.Color.White
        Me.LblPosting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblPosting.Location = New System.Drawing.Point(7, 238)
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
        Me.LblKaryawan.Location = New System.Drawing.Point(7, 136)
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
        Me.DgvPosting.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvPosting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvPosting.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulPosting, Me.BacaPosting})
        Me.DgvPosting.Location = New System.Drawing.Point(138, 226)
        Me.DgvPosting.Name = "DgvPosting"
        Me.DgvPosting.RowHeadersVisible = False
        Me.DgvPosting.Size = New System.Drawing.Size(461, 58)
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
        Me.DgvKaryawan.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvKaryawan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvKaryawan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ModulKaryawan, Me.BacaKaryawan, Me.TambahKaryawan, Me.EditKaryawan, Me.HapusKaryawan})
        Me.DgvKaryawan.Location = New System.Drawing.Point(72, 130)
        Me.DgvKaryawan.Name = "DgvKaryawan"
        Me.DgvKaryawan.RowHeadersVisible = False
        Me.DgvKaryawan.Size = New System.Drawing.Size(461, 447)
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
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.GreenYellow
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 24.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeader.ForeColor = System.Drawing.Color.Black
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(721, 47)
        Me.LblHeader.TabIndex = 167
        Me.LblHeader.Text = "SETING HAK AKSES UNTUK USER"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FormHakUser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(721, 637)
        Me.Controls.Add(Me.LblHeader)
        Me.Controls.Add(Me.PanelInput)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Me.PanelInput.ResumeLayout(False)
        Me.PanelInput.PerformLayout()
        CType(Me.DgvPosting, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DgvKaryawan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents CmbUser As System.Windows.Forms.ComboBox
    Friend WithEvents BtnKeluar As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
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
    Friend WithEvents PanelInput As System.Windows.Forms.Panel
    Friend WithEvents LblKaryawan As System.Windows.Forms.Label
    Friend WithEvents DgvKaryawan As System.Windows.Forms.DataGridView
    Friend WithEvents ModulKaryawan As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BacaKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TambahKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents EditKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents HapusKaryawan As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents DgvPosting As DataGridView
    Friend WithEvents LblPosting As Label
    Friend WithEvents ModulPosting As DataGridViewTextBoxColumn
    Friend WithEvents BacaPosting As DataGridViewCheckBoxColumn
    Friend WithEvents LblHeader As Label
End Class
