<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormRakitan
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.PanelToolbar = New System.Windows.Forms.Panel()
        Me.BtnPaketBaru = New System.Windows.Forms.Button()
        Me.BtnEdit = New System.Windows.Forms.Button()
        Me.BtnHapus = New System.Windows.Forms.Button()
        Me.BtnDetail = New System.Windows.Forms.Button()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.LblCari = New System.Windows.Forms.Label()
        Me.TxtCari = New System.Windows.Forms.TextBox()
        Me.DgvDaftarPaket = New System.Windows.Forms.DataGridView()
        Me.PanelPopup = New System.Windows.Forms.Panel()
        Me.LblPopupTitle = New System.Windows.Forms.Label()
        Me.BtnPopupClose = New System.Windows.Forms.Button()
        Me.DgvPopup = New System.Windows.Forms.DataGridView()
        Me.CtxMenu = New System.Windows.Forms.ContextMenuStrip()
        Me.CtxTambah = New System.Windows.Forms.ToolStripMenuItem()
        Me.CtxEdit = New System.Windows.Forms.ToolStripMenuItem()
        Me.CtxHapus = New System.Windows.Forms.ToolStripMenuItem()
        Me.CtxSep1 = New System.Windows.Forms.ToolStripSeparator()
        Me.CtxDetail = New System.Windows.Forms.ToolStripMenuItem()
        Me.PanelHeader.SuspendLayout()
        Me.PanelToolbar.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        CType(Me.DgvDaftarPaket, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelPopup.SuspendLayout()
        CType(Me.DgvPopup, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CtxMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(884, 47)
        Me.PanelHeader.TabIndex = 3
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.GreenYellow
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 20.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle))
        Me.LblHeader.ForeColor = System.Drawing.Color.Black
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(884, 47)
        Me.LblHeader.TabIndex = 0
        Me.LblHeader.Text = "DAFTAR PAKET RAKITAN"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnClose
        '
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Location = New System.Drawing.Point(4, 8)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(110, 30)
        Me.BtnClose.TabIndex = 1
        Me.BtnClose.Text = "✕ Tutup (Esc)"
        '
        'PanelToolbar
        '
        Me.PanelToolbar.Controls.Add(Me.BtnPaketBaru)
        Me.PanelToolbar.Controls.Add(Me.BtnEdit)
        Me.PanelToolbar.Controls.Add(Me.BtnHapus)
        Me.PanelToolbar.Controls.Add(Me.BtnDetail)
        Me.PanelToolbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelToolbar.Location = New System.Drawing.Point(0, 47)
        Me.PanelToolbar.Name = "PanelToolbar"
        Me.PanelToolbar.Padding = New System.Windows.Forms.Padding(6, 6, 6, 0)
        Me.PanelToolbar.Size = New System.Drawing.Size(884, 44)
        Me.PanelToolbar.TabIndex = 2
        '
        'BtnPaketBaru
        '
        Me.BtnPaketBaru.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnPaketBaru.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPaketBaru.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.BtnPaketBaru.ForeColor = System.Drawing.Color.White
        Me.BtnPaketBaru.Location = New System.Drawing.Point(6, 6)
        Me.BtnPaketBaru.Name = "BtnPaketBaru"
        Me.BtnPaketBaru.Size = New System.Drawing.Size(110, 32)
        Me.BtnPaketBaru.TabIndex = 0
        Me.BtnPaketBaru.Text = "+ Paket Baru"
        Me.BtnPaketBaru.UseVisualStyleBackColor = False
        '
        'BtnEdit
        '
        Me.BtnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnEdit.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.BtnEdit.Location = New System.Drawing.Point(122, 6)
        Me.BtnEdit.Name = "BtnEdit"
        Me.BtnEdit.Size = New System.Drawing.Size(80, 32)
        Me.BtnEdit.TabIndex = 1
        Me.BtnEdit.Text = "✏ Edit"
        '
        'BtnHapus
        '
        Me.BtnHapus.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnHapus.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHapus.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.BtnHapus.ForeColor = System.Drawing.Color.White
        Me.BtnHapus.Location = New System.Drawing.Point(208, 6)
        Me.BtnHapus.Name = "BtnHapus"
        Me.BtnHapus.Size = New System.Drawing.Size(80, 32)
        Me.BtnHapus.TabIndex = 2
        Me.BtnHapus.Text = "🗑 Hapus"
        Me.BtnHapus.UseVisualStyleBackColor = False
        '
        'BtnDetail
        '
        Me.BtnDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnDetail.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.BtnDetail.Location = New System.Drawing.Point(300, 6)
        Me.BtnDetail.Name = "BtnDetail"
        Me.BtnDetail.Size = New System.Drawing.Size(110, 32)
        Me.BtnDetail.TabIndex = 3
        Me.BtnDetail.Text = "📋 Detail"
        '
        'PanelCari
        '
        Me.PanelCari.Controls.Add(Me.LblCari)
        Me.PanelCari.Controls.Add(Me.TxtCari)
        Me.PanelCari.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelCari.Location = New System.Drawing.Point(0, 91)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Padding = New System.Windows.Forms.Padding(6, 6, 6, 0)
        Me.PanelCari.Size = New System.Drawing.Size(884, 36)
        Me.PanelCari.TabIndex = 1
        '
        'LblCari
        '
        Me.LblCari.AutoSize = True
        Me.LblCari.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblCari.Location = New System.Drawing.Point(6, 10)
        Me.LblCari.Name = "LblCari"
        Me.LblCari.Size = New System.Drawing.Size(35, 17)
        Me.LblCari.TabIndex = 0
        Me.LblCari.Text = "Cari:"
        '
        'TxtCari
        '
        Me.TxtCari.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.TxtCari.Location = New System.Drawing.Point(45, 7)
        Me.TxtCari.Name = "TxtCari"
        Me.TxtCari.Size = New System.Drawing.Size(280, 22)
        Me.TxtCari.TabIndex = 1
        '
        'DgvDaftarPaket
        '
        Me.DgvDaftarPaket.ContextMenuStrip = Me.CtxMenu
        Me.DgvDaftarPaket.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvDaftarPaket.Location = New System.Drawing.Point(0, 127)
        Me.DgvDaftarPaket.Name = "DgvDaftarPaket"
        Me.DgvDaftarPaket.Size = New System.Drawing.Size(884, 434)
        Me.DgvDaftarPaket.TabIndex = 0
        '
        'PanelPopup — floating dialog di atas DGV
        '
        Me.PanelPopup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelPopup.BackColor = System.Drawing.Color.White
        Me.PanelPopup.Controls.Add(Me.DgvPopup)
        Me.PanelPopup.Controls.Add(Me.BtnPopupClose)
        Me.PanelPopup.Controls.Add(Me.LblPopupTitle)
        Me.PanelPopup.Location = New System.Drawing.Point(150, 200)
        Me.PanelPopup.Name = "PanelPopup"
        Me.PanelPopup.Size = New System.Drawing.Size(700, 250)
        Me.PanelPopup.TabIndex = 6
        Me.PanelPopup.Visible = False
        '
        'LblPopupTitle
        '
        Me.LblPopupTitle.BackColor = System.Drawing.Color.SteelBlue
        Me.LblPopupTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblPopupTitle.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblPopupTitle.ForeColor = System.Drawing.Color.White
        Me.LblPopupTitle.Location = New System.Drawing.Point(0, 0)
        Me.LblPopupTitle.Name = "LblPopupTitle"
        Me.LblPopupTitle.Size = New System.Drawing.Size(698, 26)
        Me.LblPopupTitle.TabIndex = 0
        Me.LblPopupTitle.Text = "Komponen BOM"
        Me.LblPopupTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnPopupClose
        '
        Me.BtnPopupClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnPopupClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPopupClose.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.BtnPopupClose.ForeColor = System.Drawing.Color.White
        Me.BtnPopupClose.Location = New System.Drawing.Point(664, 1)
        Me.BtnPopupClose.Name = "BtnPopupClose"
        Me.BtnPopupClose.Size = New System.Drawing.Size(30, 24)
        Me.BtnPopupClose.TabIndex = 1
        Me.BtnPopupClose.Text = "✕"
        Me.BtnPopupClose.BackColor = System.Drawing.Color.SteelBlue
        '
        'DgvPopup
        '
        Me.DgvPopup.AllowUserToAddRows = False
        Me.DgvPopup.AllowUserToDeleteRows = False
        Me.DgvPopup.BackgroundColor = System.Drawing.Color.White
        Me.DgvPopup.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DgvPopup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvPopup.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvPopup.Location = New System.Drawing.Point(0, 26)
        Me.DgvPopup.Name = "DgvPopup"
        Me.DgvPopup.ReadOnly = True
        Me.DgvPopup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvPopup.Size = New System.Drawing.Size(698, 222)
        Me.DgvPopup.TabIndex = 2
        '
        'CtxMenu
        '
        Me.CtxMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CtxTambah, Me.CtxEdit, Me.CtxHapus, Me.CtxSep1, Me.CtxDetail})
        Me.CtxMenu.Name = "CtxMenu"
        Me.CtxMenu.Size = New System.Drawing.Size(200, 120)
        '
        'CtxTambah
        '
        Me.CtxTambah.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.CtxTambah.Name = "CtxTambah"
        Me.CtxTambah.Size = New System.Drawing.Size(199, 22)
        Me.CtxTambah.Text = "+ Tambah Paket Baru"
        '
        'CtxEdit
        '
        Me.CtxEdit.Name = "CtxEdit"
        Me.CtxEdit.Size = New System.Drawing.Size(199, 22)
        Me.CtxEdit.Text = "✏ Edit Paket"
        '
        'CtxHapus
        '
        Me.CtxHapus.ForeColor = System.Drawing.Color.Red
        Me.CtxHapus.Name = "CtxHapus"
        Me.CtxHapus.Size = New System.Drawing.Size(199, 22)
        Me.CtxHapus.Text = "🗑 Hapus Paket"
        '
        'CtxSep1
        '
        Me.CtxSep1.Name = "CtxSep1"
        Me.CtxSep1.Size = New System.Drawing.Size(196, 6)
        '
        'CtxDetail
        '
        Me.CtxDetail.Name = "CtxDetail"
        Me.CtxDetail.Size = New System.Drawing.Size(199, 22)
        Me.CtxDetail.Text = "📋 Tampilkan Detail"
        '
        'FormRakitan
        '
        Me.Controls.Add(Me.DgvDaftarPaket)
        Me.Controls.Add(Me.PanelPopup)
        Me.Controls.Add(Me.PanelCari)
        Me.Controls.Add(Me.PanelToolbar)
        Me.Controls.Add(Me.PanelHeader)
        Me.ClientSize = New System.Drawing.Size(884, 561)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormRakitan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Daftar Paket Rakitan"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelToolbar.ResumeLayout(False)
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        CType(Me.DgvDaftarPaket, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelPopup.ResumeLayout(False)
        CType(Me.DgvPopup, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CtxMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader    As System.Windows.Forms.Panel
    Friend WithEvents LblHeader      As System.Windows.Forms.Label
    Friend WithEvents BtnClose       As System.Windows.Forms.Button
    Friend WithEvents PanelToolbar   As System.Windows.Forms.Panel
    Friend WithEvents BtnPaketBaru   As System.Windows.Forms.Button
    Friend WithEvents BtnEdit        As System.Windows.Forms.Button
    Friend WithEvents BtnHapus       As System.Windows.Forms.Button
    Friend WithEvents BtnDetail      As System.Windows.Forms.Button
    Friend WithEvents PanelCari      As System.Windows.Forms.Panel
    Friend WithEvents LblCari        As System.Windows.Forms.Label
    Friend WithEvents TxtCari        As System.Windows.Forms.TextBox
    Friend WithEvents DgvDaftarPaket As System.Windows.Forms.DataGridView
    Friend WithEvents PanelPopup     As System.Windows.Forms.Panel
    Friend WithEvents LblPopupTitle  As System.Windows.Forms.Label
    Friend WithEvents BtnPopupClose  As System.Windows.Forms.Button
    Friend WithEvents DgvPopup       As System.Windows.Forms.DataGridView
    Friend WithEvents CtxMenu        As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CtxTambah      As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CtxEdit        As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CtxHapus       As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CtxSep1        As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CtxDetail      As System.Windows.Forms.ToolStripMenuItem

End Class
