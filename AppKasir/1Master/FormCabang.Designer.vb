<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormCabang
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCabang))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.PnlBatas2 = New System.Windows.Forms.Panel()
        Me.PnlBatas1 = New System.Windows.Forms.Panel()
        Me.PnlBatas3 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtKodeCabang = New System.Windows.Forms.TextBox()
        Me.TxtNamaCabang = New System.Windows.Forms.TextBox()
        Me.TxtAlamat = New System.Windows.Forms.TextBox()
        Me.TxtKota = New System.Windows.Forms.TextBox()
        Me.TxtHp = New System.Windows.Forms.TextBox()
        Me.TxtPemilik = New System.Windows.Forms.TextBox()
        Me.BtnSimpanManual = New System.Windows.Forms.Button()
        Me.BtnTambah = New System.Windows.Forms.Button()
        Me.BtnRefresh = New System.Windows.Forms.Button()
        Me.BtnSyncCloud = New System.Windows.Forms.Button()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.DgvCabang = New System.Windows.Forms.DataGridView()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.PanelHeader.SuspendLayout()
        CType(Me.DgvCabang, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(760, 37)
        Me.PanelHeader.TabIndex = 87
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.Gold
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 24.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle))
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(760, 37)
        Me.LblHeader.TabIndex = 141
        Me.LblHeader.Text = "DAFTAR CABANG"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PnlBatas2
        '
        Me.PnlBatas2.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.PnlBatas2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PnlBatas2.Location = New System.Drawing.Point(0, 614)
        Me.PnlBatas2.Name = "PnlBatas2"
        Me.PnlBatas2.Size = New System.Drawing.Size(760, 6)
        Me.PnlBatas2.TabIndex = 88
        '
        'PnlBatas1
        '
        Me.PnlBatas1.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.PnlBatas1.Dock = System.Windows.Forms.DockStyle.Right
        Me.PnlBatas1.Location = New System.Drawing.Point(750, 37)
        Me.PnlBatas1.Name = "PnlBatas1"
        Me.PnlBatas1.Size = New System.Drawing.Size(10, 577)
        Me.PnlBatas1.TabIndex = 89
        '
        'PnlBatas3
        '
        Me.PnlBatas3.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.PnlBatas3.Dock = System.Windows.Forms.DockStyle.Left
        Me.PnlBatas3.Location = New System.Drawing.Point(0, 37)
        Me.PnlBatas3.Name = "PnlBatas3"
        Me.PnlBatas3.Size = New System.Drawing.Size(6, 577)
        Me.PnlBatas3.TabIndex = 90
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(15, 55)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(98, 16)
        Me.Label1.TabIndex = 70
        Me.Label1.Text = "Kode Cabang"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(15, 86)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(105, 16)
        Me.Label2.TabIndex = 71
        Me.Label2.Text = "Nama Cabang"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(15, 117)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(55, 16)
        Me.Label3.TabIndex = 72
        Me.Label3.Text = "Alamat"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(15, 148)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(36, 16)
        Me.Label4.TabIndex = 73
        Me.Label4.Text = "Kota"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(15, 179)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(23, 16)
        Me.Label5.TabIndex = 74
        Me.Label5.Text = "HP"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(15, 210)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(54, 16)
        Me.Label6.TabIndex = 75
        Me.Label6.Text = "Pemilik"
        '
        'TxtKodeCabang
        '
        Me.TxtKodeCabang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKodeCabang.Location = New System.Drawing.Point(130, 52)
        Me.TxtKodeCabang.Name = "TxtKodeCabang"
        Me.TxtKodeCabang.ReadOnly = True
        Me.TxtKodeCabang.Size = New System.Drawing.Size(160, 23)
        Me.TxtKodeCabang.TabIndex = 1
        '
        'TxtNamaCabang
        '
        Me.TxtNamaCabang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaCabang.Location = New System.Drawing.Point(130, 83)
        Me.TxtNamaCabang.Name = "TxtNamaCabang"
        Me.TxtNamaCabang.Size = New System.Drawing.Size(280, 23)
        Me.TxtNamaCabang.TabIndex = 2
        '
        'TxtAlamat
        '
        Me.TxtAlamat.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAlamat.Location = New System.Drawing.Point(130, 114)
        Me.TxtAlamat.Name = "TxtAlamat"
        Me.TxtAlamat.Size = New System.Drawing.Size(280, 23)
        Me.TxtAlamat.TabIndex = 3
        '
        'TxtKota
        '
        Me.TxtKota.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKota.Location = New System.Drawing.Point(130, 145)
        Me.TxtKota.Name = "TxtKota"
        Me.TxtKota.Size = New System.Drawing.Size(180, 23)
        Me.TxtKota.TabIndex = 4
        '
        'TxtHp
        '
        Me.TxtHp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHp.Location = New System.Drawing.Point(130, 176)
        Me.TxtHp.Name = "TxtHp"
        Me.TxtHp.Size = New System.Drawing.Size(180, 23)
        Me.TxtHp.TabIndex = 5
        '
        'TxtPemilik
        '
        Me.TxtPemilik.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPemilik.Location = New System.Drawing.Point(130, 207)
        Me.TxtPemilik.Name = "TxtPemilik"
        Me.TxtPemilik.Size = New System.Drawing.Size(280, 23)
        Me.TxtPemilik.TabIndex = 6
        '
        'BtnSimpanManual
        '
        Me.BtnSimpanManual.AutoSize = True
        Me.BtnSimpanManual.BackColor = System.Drawing.Color.White
        Me.BtnSimpanManual.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpanManual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanManual.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpanManual.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpanManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpanManual.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpanManual.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanManual.Image = CType(resources.GetObject("BtnSimpanManual.Image"), System.Drawing.Image)
        Me.BtnSimpanManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanManual.Location = New System.Drawing.Point(430, 52)
        Me.BtnSimpanManual.Name = "BtnSimpanManual"
        Me.BtnSimpanManual.Size = New System.Drawing.Size(119, 34)
        Me.BtnSimpanManual.TabIndex = 7
        Me.BtnSimpanManual.Text = "Simpan (F2)"
        Me.BtnSimpanManual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanManual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpanManual.UseVisualStyleBackColor = False
        '
        'BtnTambah
        '
        Me.BtnTambah.AutoSize = True
        Me.BtnTambah.BackColor = System.Drawing.Color.White
        Me.BtnTambah.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTambah.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnTambah.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnTambah.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnTambah.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTambah.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTambah.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnTambah.Image = CType(resources.GetObject("BtnTambah.Image"), System.Drawing.Image)
        Me.BtnTambah.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTambah.Location = New System.Drawing.Point(430, 94)
        Me.BtnTambah.Name = "BtnTambah"
        Me.BtnTambah.Size = New System.Drawing.Size(119, 34)
        Me.BtnTambah.TabIndex = 8
        Me.BtnTambah.Text = "Baru (F4)"
        Me.BtnTambah.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTambah.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTambah.UseVisualStyleBackColor = False
        '
        'BtnRefresh
        '
        Me.BtnRefresh.AutoSize = True
        Me.BtnRefresh.BackColor = System.Drawing.Color.White
        Me.BtnRefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnRefresh.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnRefresh.ForeColor = System.Drawing.Color.Black
        Me.BtnRefresh.Image = CType(resources.GetObject("BtnRefresh.Image"), System.Drawing.Image)
        Me.BtnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRefresh.Location = New System.Drawing.Point(430, 136)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(119, 34)
        Me.BtnRefresh.TabIndex = 9
        Me.BtnRefresh.Text = "Refresh (F5)"
        Me.BtnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnRefresh.UseVisualStyleBackColor = False
        '
        'BtnSyncCloud
        '
        Me.BtnSyncCloud.AutoSize = True
        Me.BtnSyncCloud.BackColor = System.Drawing.Color.White
        Me.BtnSyncCloud.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSyncCloud.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSyncCloud.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSyncCloud.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSyncCloud.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSyncCloud.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSyncCloud.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSyncCloud.Image = CType(resources.GetObject("BtnSyncCloud.Image"), System.Drawing.Image)
        Me.BtnSyncCloud.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSyncCloud.Location = New System.Drawing.Point(430, 178)
        Me.BtnSyncCloud.Name = "BtnSyncCloud"
        Me.BtnSyncCloud.Size = New System.Drawing.Size(119, 34)
        Me.BtnSyncCloud.TabIndex = 10
        Me.BtnSyncCloud.Text = "Sync Cloud"
        Me.BtnSyncCloud.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSyncCloud.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSyncCloud.UseVisualStyleBackColor = False
        '
        'LblStatus
        '
        Me.LblStatus.AutoSize = True
        Me.LblStatus.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatus.Location = New System.Drawing.Point(15, 240)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(208, 17)
        Me.LblStatus.TabIndex = 80
        Me.LblStatus.Text = "Status: siap tambah cabang baru."
        '
        'DgvCabang
        '
        Me.DgvCabang.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvCabang.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvCabang.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvCabang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvCabang.Location = New System.Drawing.Point(12, 265)
        Me.DgvCabang.Name = "DgvCabang"
        Me.DgvCabang.ReadOnly = True
        Me.DgvCabang.RowHeadersVisible = False
        Me.DgvCabang.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvCabang.Size = New System.Drawing.Size(736, 343)
        Me.DgvCabang.TabIndex = 11
        '
        'BtnClose
        '
        Me.BtnClose.AutoSize = True
        Me.BtnClose.BackColor = System.Drawing.Color.White
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnClose.Location = New System.Drawing.Point(430, 218)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(119, 30)
        Me.BtnClose.TabIndex = 182
        Me.BtnClose.Text = "Keluar (Esc)"
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'FormCabang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(760, 620)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.DgvCabang)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.BtnSyncCloud)
        Me.Controls.Add(Me.BtnRefresh)
        Me.Controls.Add(Me.BtnTambah)
        Me.Controls.Add(Me.BtnSimpanManual)
        Me.Controls.Add(Me.TxtPemilik)
        Me.Controls.Add(Me.TxtHp)
        Me.Controls.Add(Me.TxtKota)
        Me.Controls.Add(Me.TxtAlamat)
        Me.Controls.Add(Me.TxtNamaCabang)
        Me.Controls.Add(Me.TxtKodeCabang)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PnlBatas3)
        Me.Controls.Add(Me.PnlBatas1)
        Me.Controls.Add(Me.PnlBatas2)
        Me.Controls.Add(Me.PanelHeader)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormCabang"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormCabang"
        Me.PanelHeader.ResumeLayout(False)
        CType(Me.DgvCabang, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents PnlBatas2 As System.Windows.Forms.Panel
    Friend WithEvents PnlBatas1 As System.Windows.Forms.Panel
    Friend WithEvents PnlBatas3 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TxtKodeCabang As System.Windows.Forms.TextBox
    Friend WithEvents TxtNamaCabang As System.Windows.Forms.TextBox
    Friend WithEvents TxtAlamat As System.Windows.Forms.TextBox
    Friend WithEvents TxtKota As System.Windows.Forms.TextBox
    Friend WithEvents TxtHp As System.Windows.Forms.TextBox
    Friend WithEvents TxtPemilik As System.Windows.Forms.TextBox
    Friend WithEvents BtnSimpanManual As System.Windows.Forms.Button
    Friend WithEvents BtnTambah As System.Windows.Forms.Button
    Friend WithEvents BtnRefresh As System.Windows.Forms.Button
    Friend WithEvents BtnSyncCloud As System.Windows.Forms.Button
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents DgvCabang As System.Windows.Forms.DataGridView
    Friend WithEvents LblHeader As Label
    Friend WithEvents BtnClose As Button
End Class
