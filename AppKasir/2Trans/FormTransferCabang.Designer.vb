<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormTransferCabang
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTransferCabang))
        Me.PanelRoot = New System.Windows.Forms.Panel()
        Me.PanelGrid = New System.Windows.Forms.Panel()
        Me.DgvDetail = New System.Windows.Forms.DataGridView()
        Me.Kode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NamaBarang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Satuan = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Isi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HargaBeli = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QtySat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TotalHarga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StokToko = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StokGudang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.TxtNamaBarang = New System.Windows.Forms.TextBox()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.PanelFooter = New System.Windows.Forms.Panel()
        Me.BtnPelanggan = New System.Windows.Forms.Button()
        Me.BtnBarang = New System.Windows.Forms.Button()
        Me.BtnExportManual = New System.Windows.Forms.Button()
        Me.BtnKirimCloud = New System.Windows.Forms.Button()
        Me.PanelTopInfo = New System.Windows.Forms.Panel()
        Me.BtnKeluarForm = New System.Windows.Forms.Button()
        Me.GBGrantotal = New System.Windows.Forms.GroupBox()
        Me.TxtGrandtotal = New System.Windows.Forms.TextBox()
        Me.BtnRefreshCabang = New System.Windows.Forms.Button()
        Me.TxtKeterangan = New System.Windows.Forms.TextBox()
        Me.LblKeterangan = New System.Windows.Forms.Label()
        Me.CmbCabangTujuan = New System.Windows.Forms.ComboBox()
        Me.LblCabangTujuan = New System.Windows.Forms.Label()
        Me.CmbMode = New System.Windows.Forms.ComboBox()
        Me.LblMode = New System.Windows.Forms.Label()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelRoot.SuspendLayout()
        Me.PanelGrid.SuspendLayout()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCari.SuspendLayout()
        Me.PanelFooter.SuspendLayout()
        Me.PanelTopInfo.SuspendLayout()
        Me.GBGrantotal.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelRoot
        '
        Me.PanelRoot.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelRoot.Controls.Add(Me.PanelGrid)
        Me.PanelRoot.Controls.Add(Me.PanelCari)
        Me.PanelRoot.Controls.Add(Me.LstBarang)
        Me.PanelRoot.Controls.Add(Me.PanelFooter)
        Me.PanelRoot.Controls.Add(Me.PanelTopInfo)
        Me.PanelRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelRoot.Location = New System.Drawing.Point(0, 0)
        Me.PanelRoot.Name = "PanelRoot"
        Me.PanelRoot.Size = New System.Drawing.Size(1291, 692)
        Me.PanelRoot.TabIndex = 0
        '
        'PanelGrid
        '
        Me.PanelGrid.Controls.Add(Me.DgvDetail)
        Me.PanelGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelGrid.Location = New System.Drawing.Point(0, 142)
        Me.PanelGrid.Name = "PanelGrid"
        Me.PanelGrid.Size = New System.Drawing.Size(1291, 489)
        Me.PanelGrid.TabIndex = 6
        '
        'DgvDetail
        '
        Me.DgvDetail.AllowUserToDeleteRows = False
        Me.DgvDetail.AllowUserToResizeColumns = False
        Me.DgvDetail.AllowUserToResizeRows = False
        Me.DgvDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvDetail.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvDetail.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvDetail.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Kode, Me.NamaBarang, Me.QTY, Me.Satuan, Me.Isi, Me.HargaBeli, Me.QtySat, Me.TotalHarga, Me.StokToko, Me.StokGudang})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvDetail.DefaultCellStyle = DataGridViewCellStyle2
        Me.DgvDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvDetail.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.DgvDetail.Location = New System.Drawing.Point(0, 0)
        Me.DgvDetail.Name = "DgvDetail"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!)
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvDetail.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!)
        Me.DgvDetail.RowsDefaultCellStyle = DataGridViewCellStyle4
        Me.DgvDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DgvDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DgvDetail.Size = New System.Drawing.Size(1291, 489)
        Me.DgvDetail.TabIndex = 5
        '
        'Kode
        '
        Me.Kode.FillWeight = 50.0!
        Me.Kode.HeaderText = "Kode"
        Me.Kode.Name = "Kode"
        Me.Kode.ReadOnly = True
        Me.Kode.Visible = False
        '
        'NamaBarang
        '
        Me.NamaBarang.FillWeight = 200.0!
        Me.NamaBarang.HeaderText = "Nama Barang"
        Me.NamaBarang.Name = "NamaBarang"
        Me.NamaBarang.ReadOnly = True
        '
        'QTY
        '
        Me.QTY.FillWeight = 40.0!
        Me.QTY.HeaderText = "QTY"
        Me.QTY.Name = "QTY"
        '
        'Satuan
        '
        Me.Satuan.FillWeight = 60.0!
        Me.Satuan.HeaderText = "Satuan"
        Me.Satuan.Name = "Satuan"
        '
        'Isi
        '
        Me.Isi.FillWeight = 30.0!
        Me.Isi.HeaderText = "Isi"
        Me.Isi.Name = "Isi"
        '
        'HargaBeli
        '
        Me.HargaBeli.FillWeight = 70.0!
        Me.HargaBeli.HeaderText = "Harga"
        Me.HargaBeli.Name = "HargaBeli"
        Me.HargaBeli.ReadOnly = True
        '
        'QtySat
        '
        Me.QtySat.FillWeight = 50.0!
        Me.QtySat.HeaderText = "Total Qty"
        Me.QtySat.Name = "QtySat"
        Me.QtySat.ReadOnly = True
        '
        'TotalHarga
        '
        Me.TotalHarga.FillWeight = 80.0!
        Me.TotalHarga.HeaderText = "Total"
        Me.TotalHarga.Name = "TotalHarga"
        Me.TotalHarga.ReadOnly = True
        '
        'StokToko
        '
        Me.StokToko.FillWeight = 60.0!
        Me.StokToko.HeaderText = "S Toko"
        Me.StokToko.Name = "StokToko"
        Me.StokToko.ReadOnly = True
        '
        'StokGudang
        '
        Me.StokGudang.FillWeight = 70.0!
        Me.StokGudang.HeaderText = "S Gudang"
        Me.StokGudang.Name = "StokGudang"
        Me.StokGudang.ReadOnly = True
        '
        'PanelCari
        '
        Me.PanelCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCari.Controls.Add(Me.BtnCari)
        Me.PanelCari.Controls.Add(Me.TxtNamaBarang)
        Me.PanelCari.Controls.Add(Me.LblStatus)
        Me.PanelCari.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelCari.Location = New System.Drawing.Point(0, 107)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(1291, 35)
        Me.PanelCari.TabIndex = 1
        '
        'BtnCari
        '
        Me.BtnCari.AutoSize = True
        Me.BtnCari.BackColor = System.Drawing.Color.White
        Me.BtnCari.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCari.Enabled = False
        Me.BtnCari.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCari.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCari.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCari.ForeColor = System.Drawing.Color.Black
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.Location = New System.Drawing.Point(538, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(60, 29)
        Me.BtnCari.TabIndex = 1000
        Me.BtnCari.Text = "Cari"
        Me.BtnCari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCari.UseVisualStyleBackColor = False
        '
        'TxtNamaBarang
        '
        Me.TxtNamaBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNamaBarang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaBarang.Location = New System.Drawing.Point(7, 5)
        Me.TxtNamaBarang.Name = "TxtNamaBarang"
        Me.TxtNamaBarang.Size = New System.Drawing.Size(531, 26)
        Me.TxtNamaBarang.TabIndex = 1
        '
        'LblStatus
        '
        Me.LblStatus.AutoEllipsis = True
        Me.LblStatus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblStatus.Location = New System.Drawing.Point(709, 0)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(579, 34)
        Me.LblStatus.TabIndex = 8
        Me.LblStatus.Text = "Status: Siap pakai"
        Me.LblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LstBarang
        '
        Me.LstBarang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LstBarang.FormattingEnabled = True
        Me.LstBarang.ItemHeight = 20
        Me.LstBarang.Location = New System.Drawing.Point(3, 148)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(532, 284)
        Me.LstBarang.TabIndex = 2
        Me.LstBarang.Visible = False
        '
        'PanelFooter
        '
        Me.PanelFooter.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelFooter.Controls.Add(Me.BtnPelanggan)
        Me.PanelFooter.Controls.Add(Me.BtnBarang)
        Me.PanelFooter.Controls.Add(Me.BtnExportManual)
        Me.PanelFooter.Controls.Add(Me.BtnKirimCloud)
        Me.PanelFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelFooter.Location = New System.Drawing.Point(0, 631)
        Me.PanelFooter.Name = "PanelFooter"
        Me.PanelFooter.Size = New System.Drawing.Size(1291, 61)
        Me.PanelFooter.TabIndex = 3
        '
        'BtnPelanggan
        '
        Me.BtnPelanggan.AutoSize = True
        Me.BtnPelanggan.BackColor = System.Drawing.Color.White
        Me.BtnPelanggan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPelanggan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPelanggan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPelanggan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPelanggan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPelanggan.ForeColor = System.Drawing.Color.Black
        Me.BtnPelanggan.Image = CType(resources.GetObject("BtnPelanggan.Image"), System.Drawing.Image)
        Me.BtnPelanggan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPelanggan.Location = New System.Drawing.Point(766, 6)
        Me.BtnPelanggan.Name = "BtnPelanggan"
        Me.BtnPelanggan.Size = New System.Drawing.Size(138, 32)
        Me.BtnPelanggan.TabIndex = 12
        Me.BtnPelanggan.Text = "Cabang (F12)"
        Me.BtnPelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPelanggan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPelanggan.UseVisualStyleBackColor = False
        '
        'BtnBarang
        '
        Me.BtnBarang.AutoSize = True
        Me.BtnBarang.BackColor = System.Drawing.Color.White
        Me.BtnBarang.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBarang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnBarang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnBarang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnBarang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBarang.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBarang.ForeColor = System.Drawing.Color.Black
        Me.BtnBarang.Image = CType(resources.GetObject("BtnBarang.Image"), System.Drawing.Image)
        Me.BtnBarang.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBarang.Location = New System.Drawing.Point(649, 6)
        Me.BtnBarang.Name = "BtnBarang"
        Me.BtnBarang.Size = New System.Drawing.Size(111, 32)
        Me.BtnBarang.TabIndex = 11
        Me.BtnBarang.Text = "Barang (F4)"
        Me.BtnBarang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBarang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBarang.UseVisualStyleBackColor = False
        '
        'BtnExportManual
        '
        Me.BtnExportManual.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnExportManual.AutoSize = True
        Me.BtnExportManual.BackColor = System.Drawing.Color.White
        Me.BtnExportManual.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnExportManual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnExportManual.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnExportManual.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnExportManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnExportManual.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnExportManual.ForeColor = System.Drawing.Color.Black
        Me.BtnExportManual.Image = CType(resources.GetObject("BtnExportManual.Image"), System.Drawing.Image)
        Me.BtnExportManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnExportManual.Location = New System.Drawing.Point(959, 13)
        Me.BtnExportManual.Name = "BtnExportManual"
        Me.BtnExportManual.Size = New System.Drawing.Size(166, 33)
        Me.BtnExportManual.TabIndex = 10
        Me.BtnExportManual.Text = "Export Manual"
        Me.BtnExportManual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnExportManual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnExportManual.UseVisualStyleBackColor = False
        '
        'BtnKirimCloud
        '
        Me.BtnKirimCloud.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKirimCloud.AutoSize = True
        Me.BtnKirimCloud.BackColor = System.Drawing.Color.White
        Me.BtnKirimCloud.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKirimCloud.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnKirimCloud.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnKirimCloud.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnKirimCloud.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKirimCloud.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKirimCloud.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnKirimCloud.Image = CType(resources.GetObject("BtnKirimCloud.Image"), System.Drawing.Image)
        Me.BtnKirimCloud.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKirimCloud.Location = New System.Drawing.Point(1131, 13)
        Me.BtnKirimCloud.Name = "BtnKirimCloud"
        Me.BtnKirimCloud.Size = New System.Drawing.Size(148, 33)
        Me.BtnKirimCloud.TabIndex = 9
        Me.BtnKirimCloud.Text = "Kirim (F10)"
        Me.BtnKirimCloud.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKirimCloud.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKirimCloud.UseVisualStyleBackColor = False
        '
        'PanelTopInfo
        '
        Me.PanelTopInfo.BackColor = System.Drawing.Color.LightSteelBlue
        Me.PanelTopInfo.Controls.Add(Me.BtnKeluarForm)
        Me.PanelTopInfo.Controls.Add(Me.GBGrantotal)
        Me.PanelTopInfo.Controls.Add(Me.BtnRefreshCabang)
        Me.PanelTopInfo.Controls.Add(Me.TxtKeterangan)
        Me.PanelTopInfo.Controls.Add(Me.LblKeterangan)
        Me.PanelTopInfo.Controls.Add(Me.CmbCabangTujuan)
        Me.PanelTopInfo.Controls.Add(Me.LblCabangTujuan)
        Me.PanelTopInfo.Controls.Add(Me.CmbMode)
        Me.PanelTopInfo.Controls.Add(Me.LblMode)
        Me.PanelTopInfo.Controls.Add(Me.LblHeaderForm)
        Me.PanelTopInfo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTopInfo.Location = New System.Drawing.Point(0, 0)
        Me.PanelTopInfo.Name = "PanelTopInfo"
        Me.PanelTopInfo.Size = New System.Drawing.Size(1291, 107)
        Me.PanelTopInfo.TabIndex = 0
        '
        'BtnKeluarForm
        '
        Me.BtnKeluarForm.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluarForm.AutoSize = True
        Me.BtnKeluarForm.BackColor = System.Drawing.Color.White
        Me.BtnKeluarForm.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluarForm.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluarForm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnKeluarForm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnKeluarForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluarForm.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluarForm.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluarForm.Image = CType(resources.GetObject("BtnKeluarForm.Image"), System.Drawing.Image)
        Me.BtnKeluarForm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarForm.Location = New System.Drawing.Point(1176, 0)
        Me.BtnKeluarForm.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnKeluarForm.Name = "BtnKeluarForm"
        Me.BtnKeluarForm.Size = New System.Drawing.Size(112, 31)
        Me.BtnKeluarForm.TabIndex = 22
        Me.BtnKeluarForm.Text = "Keluar (Esc)"
        Me.BtnKeluarForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarForm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluarForm.UseVisualStyleBackColor = False
        '
        'GBGrantotal
        '
        Me.GBGrantotal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBGrantotal.BackColor = System.Drawing.Color.LightSteelBlue
        Me.GBGrantotal.Controls.Add(Me.TxtGrandtotal)
        Me.GBGrantotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBGrantotal.ForeColor = System.Drawing.Color.Black
        Me.GBGrantotal.Location = New System.Drawing.Point(766, 32)
        Me.GBGrantotal.Name = "GBGrantotal"
        Me.GBGrantotal.Size = New System.Drawing.Size(525, 75)
        Me.GBGrantotal.TabIndex = 7
        Me.GBGrantotal.TabStop = False
        Me.GBGrantotal.Text = "Grand Total"
        '
        'TxtGrandtotal
        '
        Me.TxtGrandtotal.BackColor = System.Drawing.Color.Black
        Me.TxtGrandtotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtGrandtotal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TxtGrandtotal.Font = New System.Drawing.Font("Digital-7", 26.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGrandtotal.ForeColor = System.Drawing.Color.Lime
        Me.TxtGrandtotal.Location = New System.Drawing.Point(3, 25)
        Me.TxtGrandtotal.Multiline = True
        Me.TxtGrandtotal.Name = "TxtGrandtotal"
        Me.TxtGrandtotal.ReadOnly = True
        Me.TxtGrandtotal.Size = New System.Drawing.Size(519, 47)
        Me.TxtGrandtotal.TabIndex = 8
        Me.TxtGrandtotal.Text = "000"
        Me.TxtGrandtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BtnRefreshCabang
        '
        Me.BtnRefreshCabang.AutoSize = True
        Me.BtnRefreshCabang.BackColor = System.Drawing.Color.White
        Me.BtnRefreshCabang.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnRefreshCabang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnRefreshCabang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnRefreshCabang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnRefreshCabang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnRefreshCabang.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnRefreshCabang.ForeColor = System.Drawing.Color.Black
        Me.BtnRefreshCabang.Image = CType(resources.GetObject("BtnRefreshCabang.Image"), System.Drawing.Image)
        Me.BtnRefreshCabang.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRefreshCabang.Location = New System.Drawing.Point(60, 77)
        Me.BtnRefreshCabang.Name = "BtnRefreshCabang"
        Me.BtnRefreshCabang.Size = New System.Drawing.Size(172, 29)
        Me.BtnRefreshCabang.TabIndex = 4
        Me.BtnRefreshCabang.Text = "Refresh Cabang Cloud"
        Me.BtnRefreshCabang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRefreshCabang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnRefreshCabang.UseVisualStyleBackColor = False
        '
        'TxtKeterangan
        '
        Me.TxtKeterangan.Location = New System.Drawing.Point(441, 79)
        Me.TxtKeterangan.Name = "TxtKeterangan"
        Me.TxtKeterangan.Size = New System.Drawing.Size(307, 20)
        Me.TxtKeterangan.TabIndex = 3
        '
        'LblKeterangan
        '
        Me.LblKeterangan.AutoSize = True
        Me.LblKeterangan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblKeterangan.Location = New System.Drawing.Point(352, 81)
        Me.LblKeterangan.Name = "LblKeterangan"
        Me.LblKeterangan.Size = New System.Drawing.Size(83, 16)
        Me.LblKeterangan.TabIndex = 1
        Me.LblKeterangan.Text = "Keterangan"
        '
        'CmbCabangTujuan
        '
        Me.CmbCabangTujuan.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CmbCabangTujuan.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CmbCabangTujuan.FormattingEnabled = True
        Me.CmbCabangTujuan.Location = New System.Drawing.Point(441, 54)
        Me.CmbCabangTujuan.Name = "CmbCabangTujuan"
        Me.CmbCabangTujuan.Size = New System.Drawing.Size(307, 21)
        Me.CmbCabangTujuan.TabIndex = 2
        '
        'LblCabangTujuan
        '
        Me.LblCabangTujuan.AutoSize = True
        Me.LblCabangTujuan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblCabangTujuan.Location = New System.Drawing.Point(327, 56)
        Me.LblCabangTujuan.Name = "LblCabangTujuan"
        Me.LblCabangTujuan.Size = New System.Drawing.Size(108, 16)
        Me.LblCabangTujuan.TabIndex = 0
        Me.LblCabangTujuan.Text = "Cabang Tujuan"
        '
        'CmbMode
        '
        Me.CmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbMode.FormattingEnabled = True
        Me.CmbMode.Items.AddRange(New Object() {"KIRIM", "TERIMA"})
        Me.CmbMode.Location = New System.Drawing.Point(60, 54)
        Me.CmbMode.Name = "CmbMode"
        Me.CmbMode.Size = New System.Drawing.Size(165, 21)
        Me.CmbMode.TabIndex = 5
        '
        'LblMode
        '
        Me.LblMode.AutoSize = True
        Me.LblMode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblMode.Location = New System.Drawing.Point(5, 56)
        Me.LblMode.Name = "LblMode"
        Me.LblMode.Size = New System.Drawing.Size(52, 16)
        Me.LblMode.TabIndex = 6
        Me.LblMode.Text = "Mode :"
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Orange
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Century Gothic", 20.25!, System.Drawing.FontStyle.Bold)
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1291, 32)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "Transfer Barang Antar Cabang"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FormTransferCabang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1291, 692)
        Me.Controls.Add(Me.PanelRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormTransferCabang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Transfer Barang Antar Cabang"
        Me.PanelRoot.ResumeLayout(False)
        Me.PanelGrid.ResumeLayout(False)
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.PanelFooter.ResumeLayout(False)
        Me.PanelFooter.PerformLayout()
        Me.PanelTopInfo.ResumeLayout(False)
        Me.PanelTopInfo.PerformLayout()
        Me.GBGrantotal.ResumeLayout(False)
        Me.GBGrantotal.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelRoot As Panel
    Friend WithEvents PanelTopInfo As Panel
    Friend WithEvents PanelGrid As Panel
    Friend WithEvents PanelCari As Panel
    Friend WithEvents BtnCari As Button
    Friend WithEvents DgvDetail As DataGridView
    Friend WithEvents Kode As DataGridViewTextBoxColumn
    Friend WithEvents NamaBarang As DataGridViewTextBoxColumn
    Friend WithEvents QTY As DataGridViewTextBoxColumn
    Friend WithEvents Satuan As DataGridViewComboBoxColumn
    Friend WithEvents Isi As DataGridViewTextBoxColumn
    Friend WithEvents QtySat As DataGridViewTextBoxColumn
    Friend WithEvents HargaBeli As DataGridViewTextBoxColumn
    Friend WithEvents TotalHarga As DataGridViewTextBoxColumn
    Friend WithEvents StokToko As DataGridViewTextBoxColumn
    Friend WithEvents StokGudang As DataGridViewTextBoxColumn
    Friend WithEvents TxtNamaBarang As TextBox
    Friend WithEvents LstBarang As ListBox
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents TxtKeterangan As TextBox
    Friend WithEvents CmbCabangTujuan As ComboBox
    Friend WithEvents BtnRefreshCabang As Button
    Friend WithEvents BtnExportManual As Button
    Friend WithEvents BtnKirimCloud As Button
    Friend WithEvents LblStatus As Label
    Friend WithEvents LblKeterangan As Label
    Friend WithEvents LblCabangTujuan As Label
    Friend WithEvents LblHeaderForm As Label
    Friend WithEvents GBGrantotal As GroupBox
    Friend WithEvents TxtGrandtotal As TextBox
    Friend WithEvents CmbMode As ComboBox
    Friend WithEvents LblMode As Label
    Friend WithEvents BtnPelanggan As Button
    Friend WithEvents BtnBarang As Button
    Friend WithEvents BtnKeluarForm As Button
End Class
