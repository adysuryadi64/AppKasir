<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormReturBeli
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormReturBeli))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.BtnKeluarForm = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TxtLevel = New System.Windows.Forms.TextBox()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.GBGrantotal = New System.Windows.Forms.GroupBox()
        Me.TxtGrandtotal = New System.Windows.Forms.TextBox()
        Me.GBInput = New System.Windows.Forms.GroupBox()
        Me.TxtSupplier = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.LblKontakSupplier = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.LblAlamatSupplier = New System.Windows.Forms.Label()
        Me.LblKodeSupplier = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.LblJenisTrans = New System.Windows.Forms.Label()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.LblLokasiBarang = New System.Windows.Forms.Label()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtBarcode = New System.Windows.Forms.TextBox()
        Me.TxtHarga = New System.Windows.Forms.TextBox()
        Me.TxtIsi = New System.Windows.Forms.TextBox()
        Me.Txtsatuan = New System.Windows.Forms.TextBox()
        Me.TxtQty = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.ID_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMA_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA_BELI_TERAKHIR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SATUAN = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.ISI_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA_BELI_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY_SAT = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TOTAL = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StokToko = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StokGudang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PanelFooter = New System.Windows.Forms.Panel()
        Me.BtnSettingPrinter = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblRecord = New System.Windows.Forms.Label()
        Me.TxtKomputer = New System.Windows.Forms.TextBox()
        Me.TxtLogin = New System.Windows.Forms.TextBox()
        Me.BtnBayar = New System.Windows.Forms.Button()
        Me.TxtTotalQTY = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.HapusToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RefreshStokBarisIniToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RefreshStokSemuaBarisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GBBayar = New System.Windows.Forms.GroupBox()
        Me.TxtKodeAkunTransfer = New System.Windows.Forms.TextBox()
        Me.TxtKodeAkunTunai = New System.Windows.Forms.TextBox()
        Me.LblBayarTransfer = New System.Windows.Forms.Label()
        Me.LblBayarTunai = New System.Windows.Forms.Label()
        Me.LblGrandTotalRetur = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TxtNominalBayarTransfer = New System.Windows.Forms.TextBox()
        Me.CmbAkunTransfer = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.CmbAkunTunai = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TxtGrandTotalRetur = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtNominalBayarTunai = New System.Windows.Forms.TextBox()
        Me.RTBAlasanRetur = New System.Windows.Forms.RichTextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.BtnBatal = New System.Windows.Forms.Button()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.listSupplier = New System.Windows.Forms.ListBox()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.PanelHeader.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        Me.GBGrantotal.SuspendLayout()
        Me.GBInput.SuspendLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFooter.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.GBBayar.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.SandyBrown
        Me.PanelHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PanelHeader.Controls.Add(Me.Button1)
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Controls.Add(Me.BtnKeluarForm)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1443, 39)
        Me.PanelHeader.TabIndex = 136
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.AutoSize = True
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.Button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.Button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.Button1.Image = CType(resources.GetObject("Button1.Image"), System.Drawing.Image)
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button1.Location = New System.Drawing.Point(1323, 3)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(112, 31)
        Me.Button1.TabIndex = 78
        Me.Button1.Text = "Keluar (Esc)"
        Me.Button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button1.UseVisualStyleBackColor = False
        '
        'LblHeader
        '
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold)
        Me.LblHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(1439, 35)
        Me.LblHeader.TabIndex = 1
        Me.LblHeader.Text = "TAMBAH RETUR BELI"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.BtnKeluarForm.Location = New System.Drawing.Point(1319, 1)
        Me.BtnKeluarForm.Name = "BtnKeluarForm"
        Me.BtnKeluarForm.Size = New System.Drawing.Size(112, 31)
        Me.BtnKeluarForm.TabIndex = 77
        Me.BtnKeluarForm.Text = "Keluar (Esc)"
        Me.BtnKeluarForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarForm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluarForm.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.GroupBox1.Controls.Add(Me.TxtLevel)
        Me.GroupBox1.Controls.Add(Me.BtnCari)
        Me.GroupBox1.Controls.Add(Me.PanelCari)
        Me.GroupBox1.Controls.Add(Me.GBGrantotal)
        Me.GroupBox1.Controls.Add(Me.GBInput)
        Me.GroupBox1.Controls.Add(Me.TxtBarcode)
        Me.GroupBox1.Controls.Add(Me.TxtHarga)
        Me.GroupBox1.Controls.Add(Me.TxtIsi)
        Me.GroupBox1.Controls.Add(Me.Txtsatuan)
        Me.GroupBox1.Controls.Add(Me.TxtQty)
        Me.GroupBox1.Controls.Add(Me.TxtKode)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(0, 39)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1443, 142)
        Me.GroupBox1.TabIndex = 137
        Me.GroupBox1.TabStop = False
        '
        'TxtLevel
        '
        Me.TxtLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtLevel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLevel.Location = New System.Drawing.Point(833, 107)
        Me.TxtLevel.Name = "TxtLevel"
        Me.TxtLevel.ReadOnly = True
        Me.TxtLevel.Size = New System.Drawing.Size(64, 22)
        Me.TxtLevel.TabIndex = 9
        Me.TxtLevel.Text = "Level"
        Me.TxtLevel.Visible = False
        '
        'BtnCari
        '
        Me.BtnCari.AutoSize = True
        Me.BtnCari.BackColor = System.Drawing.Color.White
        Me.BtnCari.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCari.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCari.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCari.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCari.ForeColor = System.Drawing.Color.Black
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.Location = New System.Drawing.Point(605, 109)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(60, 29)
        Me.BtnCari.TabIndex = 2
        Me.BtnCari.Text = "Cari"
        Me.BtnCari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCari.UseVisualStyleBackColor = False
        '
        'PanelCari
        '
        Me.PanelCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCari.Controls.Add(Me.TxtNama)
        Me.PanelCari.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelCari.Location = New System.Drawing.Point(3, 102)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(562, 36)
        Me.PanelCari.TabIndex = 1
        '
        'TxtNama
        '
        Me.TxtNama.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtNama.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNama.Location = New System.Drawing.Point(3, 5)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(533, 26)
        Me.TxtNama.TabIndex = 1
        Me.TxtNama.Text = "Nama"
        '
        'GBGrantotal
        '
        Me.GBGrantotal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBGrantotal.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GBGrantotal.Controls.Add(Me.TxtGrandtotal)
        Me.GBGrantotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBGrantotal.ForeColor = System.Drawing.Color.Black
        Me.GBGrantotal.Location = New System.Drawing.Point(849, 16)
        Me.GBGrantotal.Name = "GBGrantotal"
        Me.GBGrantotal.Size = New System.Drawing.Size(591, 87)
        Me.GBGrantotal.TabIndex = 3
        Me.GBGrantotal.TabStop = False
        Me.GBGrantotal.Text = "Grand Total"
        '
        'TxtGrandtotal
        '
        Me.TxtGrandtotal.BackColor = System.Drawing.Color.Black
        Me.TxtGrandtotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtGrandtotal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TxtGrandtotal.Font = New System.Drawing.Font("Digital-7", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGrandtotal.ForeColor = System.Drawing.Color.Lime
        Me.TxtGrandtotal.Location = New System.Drawing.Point(3, 25)
        Me.TxtGrandtotal.Multiline = True
        Me.TxtGrandtotal.Name = "TxtGrandtotal"
        Me.TxtGrandtotal.ReadOnly = True
        Me.TxtGrandtotal.Size = New System.Drawing.Size(585, 59)
        Me.TxtGrandtotal.TabIndex = 8
        Me.TxtGrandtotal.Text = "000"
        Me.TxtGrandtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GBInput
        '
        Me.GBInput.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GBInput.Controls.Add(Me.TxtSupplier)
        Me.GBInput.Controls.Add(Me.Label17)
        Me.GBInput.Controls.Add(Me.LblKontakSupplier)
        Me.GBInput.Controls.Add(Me.Label10)
        Me.GBInput.Controls.Add(Me.LblAlamatSupplier)
        Me.GBInput.Controls.Add(Me.LblKodeSupplier)
        Me.GBInput.Controls.Add(Me.Label6)
        Me.GBInput.Controls.Add(Me.LblJenisTrans)
        Me.GBInput.Controls.Add(Me.DTPTgl)
        Me.GBInput.Controls.Add(Me.LblLokasiBarang)
        Me.GBInput.Controls.Add(Me.TxtFaktur)
        Me.GBInput.Controls.Add(Me.Label3)
        Me.GBInput.Controls.Add(Me.Label1)
        Me.GBInput.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBInput.ForeColor = System.Drawing.Color.White
        Me.GBInput.Location = New System.Drawing.Point(3, 16)
        Me.GBInput.Name = "GBInput"
        Me.GBInput.Size = New System.Drawing.Size(843, 87)
        Me.GBInput.TabIndex = 2
        Me.GBInput.TabStop = False
        '
        'TxtSupplier
        '
        Me.TxtSupplier.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtSupplier.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSupplier.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSupplier.Location = New System.Drawing.Point(364, 12)
        Me.TxtSupplier.Name = "TxtSupplier"
        Me.TxtSupplier.Size = New System.Drawing.Size(364, 26)
        Me.TxtSupplier.TabIndex = 238
        Me.TxtSupplier.Text = "Nama"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label17.Location = New System.Drawing.Point(295, 64)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(62, 17)
        Me.Label17.TabIndex = 237
        Me.Label17.Text = "Kontak :"
        '
        'LblKontakSupplier
        '
        Me.LblKontakSupplier.AutoSize = True
        Me.LblKontakSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKontakSupplier.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblKontakSupplier.Location = New System.Drawing.Point(361, 64)
        Me.LblKontakSupplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKontakSupplier.Name = "LblKontakSupplier"
        Me.LblKontakSupplier.Size = New System.Drawing.Size(22, 17)
        Me.LblKontakSupplier.TabIndex = 236
        Me.LblKontakSupplier.Text = "08"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label10.Location = New System.Drawing.Point(293, 41)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(64, 17)
        Me.Label10.TabIndex = 234
        Me.Label10.Text = "Alamat :"
        '
        'LblAlamatSupplier
        '
        Me.LblAlamatSupplier.AutoSize = True
        Me.LblAlamatSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblAlamatSupplier.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblAlamatSupplier.Location = New System.Drawing.Point(361, 41)
        Me.LblAlamatSupplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblAlamatSupplier.Name = "LblAlamatSupplier"
        Me.LblAlamatSupplier.Size = New System.Drawing.Size(21, 17)
        Me.LblAlamatSupplier.TabIndex = 232
        Me.LblAlamatSupplier.Text = "Jl."
        '
        'LblKodeSupplier
        '
        Me.LblKodeSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeSupplier.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblKodeSupplier.Location = New System.Drawing.Point(736, 17)
        Me.LblKodeSupplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeSupplier.Name = "LblKodeSupplier"
        Me.LblKodeSupplier.Size = New System.Drawing.Size(82, 17)
        Me.LblKodeSupplier.TabIndex = 231
        Me.LblKodeSupplier.Text = "SPL-0000"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label6.Location = New System.Drawing.Point(291, 17)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(66, 17)
        Me.Label6.TabIndex = 230
        Me.Label6.Text = "Supplier :"
        '
        'LblJenisTrans
        '
        Me.LblJenisTrans.BackColor = System.Drawing.Color.Transparent
        Me.LblJenisTrans.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJenisTrans.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisTrans.ForeColor = System.Drawing.Color.Black
        Me.LblJenisTrans.Location = New System.Drawing.Point(6, 62)
        Me.LblJenisTrans.Name = "LblJenisTrans"
        Me.LblJenisTrans.Size = New System.Drawing.Size(117, 21)
        Me.LblJenisTrans.TabIndex = 121
        Me.LblJenisTrans.Text = "TambahTransfer"
        Me.LblJenisTrans.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(70, 36)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(178, 22)
        Me.DTPTgl.TabIndex = 9
        '
        'LblLokasiBarang
        '
        Me.LblLokasiBarang.BackColor = System.Drawing.Color.Transparent
        Me.LblLokasiBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblLokasiBarang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLokasiBarang.ForeColor = System.Drawing.Color.Black
        Me.LblLokasiBarang.Location = New System.Drawing.Point(136, 62)
        Me.LblLokasiBarang.Name = "LblLokasiBarang"
        Me.LblLokasiBarang.Size = New System.Drawing.Size(112, 21)
        Me.LblLokasiBarang.TabIndex = 120
        Me.LblLokasiBarang.Text = "0"
        Me.LblLokasiBarang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.ForeColor = System.Drawing.Color.Black
        Me.TxtFaktur.Location = New System.Drawing.Point(70, 10)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.ReadOnly = True
        Me.TxtFaktur.Size = New System.Drawing.Size(178, 22)
        Me.TxtFaktur.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(6, 39)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 16)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Tanggal"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 16)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Nomor"
        '
        'TxtBarcode
        '
        Me.TxtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBarcode.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBarcode.Location = New System.Drawing.Point(1274, 106)
        Me.TxtBarcode.Name = "TxtBarcode"
        Me.TxtBarcode.ReadOnly = True
        Me.TxtBarcode.Size = New System.Drawing.Size(139, 26)
        Me.TxtBarcode.TabIndex = 8
        Me.TxtBarcode.Text = "Barcode"
        Me.TxtBarcode.Visible = False
        '
        'TxtHarga
        '
        Me.TxtHarga.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtHarga.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHarga.Location = New System.Drawing.Point(1198, 107)
        Me.TxtHarga.Name = "TxtHarga"
        Me.TxtHarga.ReadOnly = True
        Me.TxtHarga.Size = New System.Drawing.Size(64, 22)
        Me.TxtHarga.TabIndex = 8
        Me.TxtHarga.Text = "Harga"
        Me.TxtHarga.Visible = False
        '
        'TxtIsi
        '
        Me.TxtIsi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIsi.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsi.Location = New System.Drawing.Point(1130, 107)
        Me.TxtIsi.Name = "TxtIsi"
        Me.TxtIsi.ReadOnly = True
        Me.TxtIsi.Size = New System.Drawing.Size(64, 22)
        Me.TxtIsi.TabIndex = 8
        Me.TxtIsi.Text = "Isi"
        Me.TxtIsi.Visible = False
        '
        'Txtsatuan
        '
        Me.Txtsatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Txtsatuan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Txtsatuan.Location = New System.Drawing.Point(1060, 107)
        Me.Txtsatuan.Name = "Txtsatuan"
        Me.Txtsatuan.ReadOnly = True
        Me.Txtsatuan.Size = New System.Drawing.Size(64, 22)
        Me.Txtsatuan.TabIndex = 8
        Me.Txtsatuan.Text = "satuan"
        Me.Txtsatuan.Visible = False
        '
        'TxtQty
        '
        Me.TxtQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtQty.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQty.Location = New System.Drawing.Point(990, 107)
        Me.TxtQty.Name = "TxtQty"
        Me.TxtQty.ReadOnly = True
        Me.TxtQty.Size = New System.Drawing.Size(64, 22)
        Me.TxtQty.TabIndex = 8
        Me.TxtQty.Text = "Qty"
        Me.TxtQty.Visible = False
        '
        'TxtKode
        '
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(920, 107)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.ReadOnly = True
        Me.TxtKode.Size = New System.Drawing.Size(64, 22)
        Me.TxtKode.TabIndex = 8
        Me.TxtKode.Text = "Kode"
        Me.TxtKode.Visible = False
        '
        'DgvData
        '
        Me.DgvData.AllowUserToResizeColumns = False
        Me.DgvData.AllowUserToResizeRows = False
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        Me.DgvData.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ID_BARANG, Me.NAMA_BARANG, Me.HARGA_BELI_TERAKHIR, Me.QTY, Me.SATUAN, Me.ISI_SATUAN, Me.HARGA_BELI_SATUAN, Me.QTY_SAT, Me.TOTAL, Me.StokToko, Me.StokGudang})
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvData.DefaultCellStyle = DataGridViewCellStyle11
        Me.DgvData.Location = New System.Drawing.Point(6, 180)
        Me.DgvData.Name = "DgvData"
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.RowHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.DgvData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DgvData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DgvData.Size = New System.Drawing.Size(1431, 375)
        Me.DgvData.TabIndex = 139
        '
        'ID_BARANG
        '
        Me.ID_BARANG.FillWeight = 50.0!
        Me.ID_BARANG.HeaderText = "ID BARANG"
        Me.ID_BARANG.Name = "ID_BARANG"
        Me.ID_BARANG.Visible = False
        '
        'NAMA_BARANG
        '
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        Me.NAMA_BARANG.DefaultCellStyle = DataGridViewCellStyle3
        Me.NAMA_BARANG.FillWeight = 300.0!
        Me.NAMA_BARANG.HeaderText = "NAMA BARANG"
        Me.NAMA_BARANG.Name = "NAMA_BARANG"
        '
        'HARGA_BELI_TERAKHIR
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "N0"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.HARGA_BELI_TERAKHIR.DefaultCellStyle = DataGridViewCellStyle4
        Me.HARGA_BELI_TERAKHIR.FillWeight = 60.0!
        Me.HARGA_BELI_TERAKHIR.HeaderText = "HARGA BELI"
        Me.HARGA_BELI_TERAKHIR.Name = "HARGA_BELI_TERAKHIR"
        '
        'QTY
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle5.Format = "N0"
        DataGridViewCellStyle5.NullValue = Nothing
        Me.QTY.DefaultCellStyle = DataGridViewCellStyle5
        Me.QTY.FillWeight = 30.0!
        Me.QTY.HeaderText = "QTY"
        Me.QTY.Name = "QTY"
        Me.QTY.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.QTY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'SATUAN
        '
        Me.SATUAN.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox
        Me.SATUAN.FillWeight = 50.0!
        Me.SATUAN.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.SATUAN.HeaderText = "SATUAN"
        Me.SATUAN.Name = "SATUAN"
        '
        'ISI_SATUAN
        '
        Me.ISI_SATUAN.FillWeight = 20.0!
        Me.ISI_SATUAN.HeaderText = "ISI SATUAN"
        Me.ISI_SATUAN.Name = "ISI_SATUAN"
        Me.ISI_SATUAN.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ISI_SATUAN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.ISI_SATUAN.Visible = False
        '
        'HARGA_BELI_SATUAN
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle6.Format = "N0"
        DataGridViewCellStyle6.NullValue = Nothing
        Me.HARGA_BELI_SATUAN.DefaultCellStyle = DataGridViewCellStyle6
        Me.HARGA_BELI_SATUAN.HeaderText = "HARGA BELI SATUAN"
        Me.HARGA_BELI_SATUAN.Name = "HARGA_BELI_SATUAN"
        Me.HARGA_BELI_SATUAN.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HARGA_BELI_SATUAN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.HARGA_BELI_SATUAN.Visible = False
        '
        'QTY_SAT
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle7.Format = "N0"
        DataGridViewCellStyle7.NullValue = Nothing
        Me.QTY_SAT.DefaultCellStyle = DataGridViewCellStyle7
        Me.QTY_SAT.FillWeight = 40.0!
        Me.QTY_SAT.HeaderText = "QTY SAT"
        Me.QTY_SAT.Name = "QTY_SAT"
        Me.QTY_SAT.Visible = False
        '
        'TOTAL
        '
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle8.Format = "N0"
        DataGridViewCellStyle8.NullValue = Nothing
        Me.TOTAL.DefaultCellStyle = DataGridViewCellStyle8
        Me.TOTAL.FillWeight = 80.0!
        Me.TOTAL.HeaderText = "TOTAL"
        Me.TOTAL.Name = "TOTAL"
        Me.TOTAL.ReadOnly = True
        Me.TOTAL.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TOTAL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'StokToko
        '
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle9.Format = "#,0.#"
        Me.StokToko.DefaultCellStyle = DataGridViewCellStyle9
        Me.StokToko.FillWeight = 40.0!
        Me.StokToko.HeaderText = "S TOKO"
        Me.StokToko.Name = "StokToko"
        '
        'StokGudang
        '
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle10.Format = "#,0.#"
        Me.StokGudang.DefaultCellStyle = DataGridViewCellStyle10
        Me.StokGudang.FillWeight = 40.0!
        Me.StokGudang.HeaderText = "S GUDANG"
        Me.StokGudang.Name = "StokGudang"
        '
        'PanelFooter
        '
        Me.PanelFooter.BackColor = System.Drawing.Color.LightSkyBlue
        Me.PanelFooter.Controls.Add(Me.BtnSettingPrinter)
        Me.PanelFooter.Controls.Add(Me.Label2)
        Me.PanelFooter.Controls.Add(Me.LblRecord)
        Me.PanelFooter.Controls.Add(Me.TxtKomputer)
        Me.PanelFooter.Controls.Add(Me.TxtLogin)
        Me.PanelFooter.Controls.Add(Me.BtnBayar)
        Me.PanelFooter.Controls.Add(Me.TxtTotalQTY)
        Me.PanelFooter.Controls.Add(Me.Label5)
        Me.PanelFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelFooter.Location = New System.Drawing.Point(0, 554)
        Me.PanelFooter.Name = "PanelFooter"
        Me.PanelFooter.Size = New System.Drawing.Size(1443, 76)
        Me.PanelFooter.TabIndex = 140
        '
        'BtnSettingPrinter
        '
        Me.BtnSettingPrinter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnSettingPrinter.AutoSize = True
        Me.BtnSettingPrinter.BackColor = System.Drawing.Color.White
        Me.BtnSettingPrinter.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSettingPrinter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSettingPrinter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSettingPrinter.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSettingPrinter.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSettingPrinter.ForeColor = System.Drawing.Color.Black
        Me.BtnSettingPrinter.Image = CType(resources.GetObject("BtnSettingPrinter.Image"), System.Drawing.Image)
        Me.BtnSettingPrinter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSettingPrinter.Location = New System.Drawing.Point(1349, 13)
        Me.BtnSettingPrinter.Name = "BtnSettingPrinter"
        Me.BtnSettingPrinter.Size = New System.Drawing.Size(82, 29)
        Me.BtnSettingPrinter.TabIndex = 219
        Me.BtnSettingPrinter.Text = "Printer"
        Me.BtnSettingPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSettingPrinter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSettingPrinter.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(980, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 17)
        Me.Label2.TabIndex = 126
        Me.Label2.Text = "Record :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblRecord
        '
        Me.LblRecord.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblRecord.AutoSize = True
        Me.LblRecord.BackColor = System.Drawing.Color.Transparent
        Me.LblRecord.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRecord.ForeColor = System.Drawing.Color.Black
        Me.LblRecord.Location = New System.Drawing.Point(1048, 13)
        Me.LblRecord.Name = "LblRecord"
        Me.LblRecord.Size = New System.Drawing.Size(54, 17)
        Me.LblRecord.TabIndex = 125
        Me.LblRecord.Text = "Record"
        Me.LblRecord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtKomputer
        '
        Me.TxtKomputer.BackColor = System.Drawing.Color.White
        Me.TxtKomputer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtKomputer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKomputer.ForeColor = System.Drawing.Color.Black
        Me.TxtKomputer.Location = New System.Drawing.Point(748, 48)
        Me.TxtKomputer.Name = "TxtKomputer"
        Me.TxtKomputer.ReadOnly = True
        Me.TxtKomputer.Size = New System.Drawing.Size(73, 16)
        Me.TxtKomputer.TabIndex = 124
        Me.TxtKomputer.Text = "Komputer"
        Me.TxtKomputer.Visible = False
        '
        'TxtLogin
        '
        Me.TxtLogin.BackColor = System.Drawing.Color.White
        Me.TxtLogin.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtLogin.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLogin.ForeColor = System.Drawing.Color.Black
        Me.TxtLogin.Location = New System.Drawing.Point(691, 48)
        Me.TxtLogin.Name = "TxtLogin"
        Me.TxtLogin.ReadOnly = True
        Me.TxtLogin.Size = New System.Drawing.Size(55, 16)
        Me.TxtLogin.TabIndex = 123
        Me.TxtLogin.Text = "Login"
        Me.TxtLogin.Visible = False
        '
        'BtnBayar
        '
        Me.BtnBayar.AutoSize = True
        Me.BtnBayar.BackColor = System.Drawing.Color.White
        Me.BtnBayar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBayar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnBayar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnBayar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnBayar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBayar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBayar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnBayar.Image = CType(resources.GetObject("BtnBayar.Image"), System.Drawing.Image)
        Me.BtnBayar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayar.Location = New System.Drawing.Point(3, 7)
        Me.BtnBayar.Name = "BtnBayar"
        Me.BtnBayar.Size = New System.Drawing.Size(135, 41)
        Me.BtnBayar.TabIndex = 113
        Me.BtnBayar.Text = "Bayar (F8)"
        Me.BtnBayar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBayar.UseVisualStyleBackColor = False
        '
        'TxtTotalQTY
        '
        Me.TxtTotalQTY.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtTotalQTY.BackColor = System.Drawing.Color.LightSkyBlue
        Me.TxtTotalQTY.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTotalQTY.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalQTY.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalQTY.Location = New System.Drawing.Point(1229, 13)
        Me.TxtTotalQTY.Name = "TxtTotalQTY"
        Me.TxtTotalQTY.ReadOnly = True
        Me.TxtTotalQTY.Size = New System.Drawing.Size(51, 16)
        Me.TxtTotalQTY.TabIndex = 8
        Me.TxtTotalQTY.Text = "0"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(1127, 13)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(65, 17)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Total Qty"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HapusToolStripMenuItem, Me.RefreshStokBarisIniToolStripMenuItem, Me.RefreshStokSemuaBarisToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.ShowCheckMargin = True
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(229, 70)
        '
        'HapusToolStripMenuItem
        '
        Me.HapusToolStripMenuItem.Name = "HapusToolStripMenuItem"
        Me.HapusToolStripMenuItem.Size = New System.Drawing.Size(228, 22)
        Me.HapusToolStripMenuItem.Text = "Hapus barang"
        '
        'RefreshStokBarisIniToolStripMenuItem
        '
        Me.RefreshStokBarisIniToolStripMenuItem.Name = "RefreshStokBarisIniToolStripMenuItem"
        Me.RefreshStokBarisIniToolStripMenuItem.Size = New System.Drawing.Size(228, 22)
        Me.RefreshStokBarisIniToolStripMenuItem.Text = "Refresh Stok Baris Ini"
        '
        'RefreshStokSemuaBarisToolStripMenuItem
        '
        Me.RefreshStokSemuaBarisToolStripMenuItem.Name = "RefreshStokSemuaBarisToolStripMenuItem"
        Me.RefreshStokSemuaBarisToolStripMenuItem.Size = New System.Drawing.Size(228, 22)
        Me.RefreshStokSemuaBarisToolStripMenuItem.Text = "Refresh Stok Semua Baris"
        '
        'GBBayar
        '
        Me.GBBayar.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GBBayar.Controls.Add(Me.TxtKodeAkunTransfer)
        Me.GBBayar.Controls.Add(Me.TxtKodeAkunTunai)
        Me.GBBayar.Controls.Add(Me.LblBayarTransfer)
        Me.GBBayar.Controls.Add(Me.LblBayarTunai)
        Me.GBBayar.Controls.Add(Me.LblGrandTotalRetur)
        Me.GBBayar.Controls.Add(Me.Label12)
        Me.GBBayar.Controls.Add(Me.TxtNominalBayarTransfer)
        Me.GBBayar.Controls.Add(Me.CmbAkunTransfer)
        Me.GBBayar.Controls.Add(Me.Label11)
        Me.GBBayar.Controls.Add(Me.CmbAkunTunai)
        Me.GBBayar.Controls.Add(Me.Label8)
        Me.GBBayar.Controls.Add(Me.Label13)
        Me.GBBayar.Controls.Add(Me.TxtGrandTotalRetur)
        Me.GBBayar.Controls.Add(Me.Label14)
        Me.GBBayar.Controls.Add(Me.TxtNominalBayarTunai)
        Me.GBBayar.Controls.Add(Me.RTBAlasanRetur)
        Me.GBBayar.Controls.Add(Me.Label9)
        Me.GBBayar.Controls.Add(Me.BtnBatal)
        Me.GBBayar.Controls.Add(Me.BtnSimpan)
        Me.GBBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBBayar.ForeColor = System.Drawing.Color.Black
        Me.GBBayar.Location = New System.Drawing.Point(581, 154)
        Me.GBBayar.Name = "GBBayar"
        Me.GBBayar.Size = New System.Drawing.Size(526, 366)
        Me.GBBayar.TabIndex = 141
        Me.GBBayar.TabStop = False
        Me.GBBayar.Text = "Informasi pembayaran"
        Me.GBBayar.Visible = False
        '
        'TxtKodeAkunTransfer
        '
        Me.TxtKodeAkunTransfer.BackColor = System.Drawing.Color.White
        Me.TxtKodeAkunTransfer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtKodeAkunTransfer.Enabled = False
        Me.TxtKodeAkunTransfer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKodeAkunTransfer.ForeColor = System.Drawing.Color.Black
        Me.TxtKodeAkunTransfer.Location = New System.Drawing.Point(14, 329)
        Me.TxtKodeAkunTransfer.Name = "TxtKodeAkunTransfer"
        Me.TxtKodeAkunTransfer.ReadOnly = True
        Me.TxtKodeAkunTransfer.Size = New System.Drawing.Size(116, 16)
        Me.TxtKodeAkunTransfer.TabIndex = 222
        Me.TxtKodeAkunTransfer.Text = "Transfer"
        Me.TxtKodeAkunTransfer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtKodeAkunTransfer.Visible = False
        '
        'TxtKodeAkunTunai
        '
        Me.TxtKodeAkunTunai.BackColor = System.Drawing.Color.White
        Me.TxtKodeAkunTunai.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtKodeAkunTunai.Enabled = False
        Me.TxtKodeAkunTunai.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKodeAkunTunai.ForeColor = System.Drawing.Color.Black
        Me.TxtKodeAkunTunai.Location = New System.Drawing.Point(14, 308)
        Me.TxtKodeAkunTunai.Name = "TxtKodeAkunTunai"
        Me.TxtKodeAkunTunai.ReadOnly = True
        Me.TxtKodeAkunTunai.Size = New System.Drawing.Size(116, 16)
        Me.TxtKodeAkunTunai.TabIndex = 221
        Me.TxtKodeAkunTunai.Text = "Tunai"
        Me.TxtKodeAkunTunai.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtKodeAkunTunai.Visible = False
        '
        'LblBayarTransfer
        '
        Me.LblBayarTransfer.BackColor = System.Drawing.Color.Transparent
        Me.LblBayarTransfer.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblBayarTransfer.ForeColor = System.Drawing.Color.Black
        Me.LblBayarTransfer.Location = New System.Drawing.Point(348, 163)
        Me.LblBayarTransfer.Name = "LblBayarTransfer"
        Me.LblBayarTransfer.Size = New System.Drawing.Size(129, 19)
        Me.LblBayarTransfer.TabIndex = 220
        Me.LblBayarTransfer.Text = "Rp. 0"
        Me.LblBayarTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblBayarTunai
        '
        Me.LblBayarTunai.BackColor = System.Drawing.Color.Transparent
        Me.LblBayarTunai.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblBayarTunai.ForeColor = System.Drawing.Color.Black
        Me.LblBayarTunai.Location = New System.Drawing.Point(348, 133)
        Me.LblBayarTunai.Name = "LblBayarTunai"
        Me.LblBayarTunai.Size = New System.Drawing.Size(129, 19)
        Me.LblBayarTunai.TabIndex = 219
        Me.LblBayarTunai.Text = "Rp. 0"
        Me.LblBayarTunai.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblGrandTotalRetur
        '
        Me.LblGrandTotalRetur.BackColor = System.Drawing.Color.Transparent
        Me.LblGrandTotalRetur.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblGrandTotalRetur.ForeColor = System.Drawing.Color.Black
        Me.LblGrandTotalRetur.Location = New System.Drawing.Point(348, 102)
        Me.LblGrandTotalRetur.Name = "LblGrandTotalRetur"
        Me.LblGrandTotalRetur.Size = New System.Drawing.Size(129, 19)
        Me.LblGrandTotalRetur.TabIndex = 218
        Me.LblGrandTotalRetur.Text = "Rp. 0"
        Me.LblGrandTotalRetur.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(15, 163)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(161, 19)
        Me.Label12.TabIndex = 217
        Me.Label12.Text = "Nominal Transfer :"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtNominalBayarTransfer
        '
        Me.TxtNominalBayarTransfer.BackColor = System.Drawing.Color.White
        Me.TxtNominalBayarTransfer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNominalBayarTransfer.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNominalBayarTransfer.ForeColor = System.Drawing.Color.Black
        Me.TxtNominalBayarTransfer.Location = New System.Drawing.Point(182, 159)
        Me.TxtNominalBayarTransfer.Name = "TxtNominalBayarTransfer"
        Me.TxtNominalBayarTransfer.Size = New System.Drawing.Size(160, 26)
        Me.TxtNominalBayarTransfer.TabIndex = 216
        Me.TxtNominalBayarTransfer.Text = "0"
        Me.TxtNominalBayarTransfer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CmbAkunTransfer
        '
        Me.CmbAkunTransfer.BackColor = System.Drawing.Color.White
        Me.CmbAkunTransfer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbAkunTransfer.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbAkunTransfer.ForeColor = System.Drawing.Color.Black
        Me.CmbAkunTransfer.FormattingEnabled = True
        Me.CmbAkunTransfer.Location = New System.Drawing.Point(182, 56)
        Me.CmbAkunTransfer.Name = "CmbAkunTransfer"
        Me.CmbAkunTransfer.Size = New System.Drawing.Size(295, 27)
        Me.CmbAkunTransfer.TabIndex = 214
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.Transparent
        Me.Label11.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(2, 60)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(174, 19)
        Me.Label11.TabIndex = 215
        Me.Label11.Text = "Akun Transfer (F12)"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbAkunTunai
        '
        Me.CmbAkunTunai.BackColor = System.Drawing.Color.White
        Me.CmbAkunTunai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbAkunTunai.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbAkunTunai.ForeColor = System.Drawing.Color.Black
        Me.CmbAkunTunai.FormattingEnabled = True
        Me.CmbAkunTunai.Location = New System.Drawing.Point(182, 26)
        Me.CmbAkunTunai.Name = "CmbAkunTunai"
        Me.CmbAkunTunai.Size = New System.Drawing.Size(295, 27)
        Me.CmbAkunTunai.TabIndex = 208
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(23, 30)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(153, 19)
        Me.Label8.TabIndex = 213
        Me.Label8.Text = "Akun Tunai (F11)"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(23, 103)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(150, 19)
        Me.Label13.TabIndex = 212
        Me.Label13.Text = "Total Retur Beli :"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtGrandTotalRetur
        '
        Me.TxtGrandTotalRetur.BackColor = System.Drawing.Color.White
        Me.TxtGrandTotalRetur.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtGrandTotalRetur.Enabled = False
        Me.TxtGrandTotalRetur.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGrandTotalRetur.ForeColor = System.Drawing.Color.Black
        Me.TxtGrandTotalRetur.Location = New System.Drawing.Point(182, 98)
        Me.TxtGrandTotalRetur.Name = "TxtGrandTotalRetur"
        Me.TxtGrandTotalRetur.ReadOnly = True
        Me.TxtGrandTotalRetur.Size = New System.Drawing.Size(160, 26)
        Me.TxtGrandTotalRetur.TabIndex = 211
        Me.TxtGrandTotalRetur.Text = "0"
        Me.TxtGrandTotalRetur.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.Font = New System.Drawing.Font("Bookman Old Style", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(35, 133)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(141, 19)
        Me.Label14.TabIndex = 210
        Me.Label14.Text = "Nominal Tunai :"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtNominalBayarTunai
        '
        Me.TxtNominalBayarTunai.BackColor = System.Drawing.Color.White
        Me.TxtNominalBayarTunai.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNominalBayarTunai.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNominalBayarTunai.ForeColor = System.Drawing.Color.Black
        Me.TxtNominalBayarTunai.Location = New System.Drawing.Point(182, 129)
        Me.TxtNominalBayarTunai.Name = "TxtNominalBayarTunai"
        Me.TxtNominalBayarTunai.Size = New System.Drawing.Size(160, 26)
        Me.TxtNominalBayarTunai.TabIndex = 209
        Me.TxtNominalBayarTunai.Text = "0"
        Me.TxtNominalBayarTunai.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'RTBAlasanRetur
        '
        Me.RTBAlasanRetur.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RTBAlasanRetur.BackColor = System.Drawing.Color.Ivory
        Me.RTBAlasanRetur.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RTBAlasanRetur.Location = New System.Drawing.Point(14, 221)
        Me.RTBAlasanRetur.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.RTBAlasanRetur.Name = "RTBAlasanRetur"
        Me.RTBAlasanRetur.Size = New System.Drawing.Size(505, 69)
        Me.RTBAlasanRetur.TabIndex = 207
        Me.RTBAlasanRetur.Text = ""
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(20, 199)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(86, 18)
        Me.Label9.TabIndex = 25
        Me.Label9.Text = "Alasan retur"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnBatal
        '
        Me.BtnBatal.AutoSize = True
        Me.BtnBatal.BackColor = System.Drawing.Color.White
        Me.BtnBatal.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBatal.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnBatal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnBatal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnBatal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBatal.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBatal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnBatal.Image = CType(resources.GetObject("BtnBatal.Image"), System.Drawing.Image)
        Me.BtnBatal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatal.Location = New System.Drawing.Point(409, 303)
        Me.BtnBatal.Name = "BtnBatal"
        Me.BtnBatal.Size = New System.Drawing.Size(106, 38)
        Me.BtnBatal.TabIndex = 5
        Me.BtnBatal.Text = "Batal (F11)"
        Me.BtnBatal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBatal.UseVisualStyleBackColor = False
        '
        'BtnSimpan
        '
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
        Me.BtnSimpan.Location = New System.Drawing.Point(268, 303)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(121, 38)
        Me.BtnSimpan.TabIndex = 4
        Me.BtnSimpan.Text = "Simpan (F10)"
        Me.BtnSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'listSupplier
        '
        Me.listSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.listSupplier.FormattingEnabled = True
        Me.listSupplier.ItemHeight = 17
        Me.listSupplier.Location = New System.Drawing.Point(367, 93)
        Me.listSupplier.Name = "listSupplier"
        Me.listSupplier.Size = New System.Drawing.Size(364, 55)
        Me.listSupplier.TabIndex = 142
        '
        'LstBarang
        '
        Me.LstBarang.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LstBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LstBarang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LstBarang.ItemHeight = 20
        Me.LstBarang.Location = New System.Drawing.Point(6, 178)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(533, 242)
        Me.LstBarang.TabIndex = 143
        '
        'FormReturBeli
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1443, 630)
        Me.Controls.Add(Me.LstBarang)
        Me.Controls.Add(Me.listSupplier)
        Me.Controls.Add(Me.GBBayar)
        Me.Controls.Add(Me.PanelFooter)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.DgvData)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormReturBeli"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormReturBeli"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.GBGrantotal.ResumeLayout(False)
        Me.GBGrantotal.PerformLayout()
        Me.GBInput.ResumeLayout(False)
        Me.GBInput.PerformLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFooter.ResumeLayout(False)
        Me.PanelFooter.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.GBBayar.ResumeLayout(False)
        Me.GBBayar.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents LblHeader As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents PanelCari As Panel
    Friend WithEvents BtnCari As Button
    Friend WithEvents TxtNama As TextBox
    Friend WithEvents GBGrantotal As GroupBox
    Friend WithEvents TxtGrandtotal As TextBox
    Friend WithEvents GBInput As GroupBox
    Friend WithEvents LblJenisTrans As Label
    Friend WithEvents DTPTgl As DateTimePicker
    Friend WithEvents TxtFaktur As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtBarcode As TextBox
    Friend WithEvents TxtHarga As TextBox
    Friend WithEvents TxtIsi As TextBox
    Friend WithEvents Txtsatuan As TextBox
    Friend WithEvents TxtQty As TextBox
    Friend WithEvents TxtKode As TextBox
    Friend WithEvents DgvData As DataGridView
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents LblRecord As Label
    Friend WithEvents BtnKeluarForm As Button
    Friend WithEvents TxtKomputer As TextBox
    Friend WithEvents TxtLogin As TextBox
    Friend WithEvents BtnBayar As Button
    Friend WithEvents TxtTotalQTY As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents HapusToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RefreshStokBarisIniToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RefreshStokSemuaBarisToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LblLokasiBarang As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents LblKontakSupplier As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents LblAlamatSupplier As Label
    Friend WithEvents LblKodeSupplier As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents GBBayar As GroupBox
    Friend WithEvents BtnBatal As Button
    Friend WithEvents BtnSimpan As Button
    Friend WithEvents Label9 As Label
    Friend WithEvents RTBAlasanRetur As RichTextBox
    Friend WithEvents listSupplier As ListBox
    Friend WithEvents TxtSupplier As TextBox
    Friend WithEvents BtnSettingPrinter As Button
    Friend WithEvents LstBarang As ListBox
    Friend WithEvents TxtLevel As TextBox
    Friend WithEvents LblBayarTransfer As Label
    Friend WithEvents LblBayarTunai As Label
    Friend WithEvents LblGrandTotalRetur As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents TxtNominalBayarTransfer As TextBox
    Friend WithEvents CmbAkunTransfer As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents CmbAkunTunai As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents TxtGrandTotalRetur As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents TxtNominalBayarTunai As TextBox
    Friend WithEvents TxtKodeAkunTransfer As TextBox
    Friend WithEvents TxtKodeAkunTunai As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents ID_BARANG As DataGridViewTextBoxColumn
    Friend WithEvents NAMA_BARANG As DataGridViewTextBoxColumn
    Friend WithEvents HARGA_BELI_TERAKHIR As DataGridViewTextBoxColumn
    Friend WithEvents QTY As DataGridViewTextBoxColumn
    Friend WithEvents SATUAN As DataGridViewComboBoxColumn
    Friend WithEvents ISI_SATUAN As DataGridViewTextBoxColumn
    Friend WithEvents HARGA_BELI_SATUAN As DataGridViewTextBoxColumn
    Friend WithEvents QTY_SAT As DataGridViewTextBoxColumn
    Friend WithEvents TOTAL As DataGridViewTextBoxColumn
    Friend WithEvents StokToko As DataGridViewTextBoxColumn
    Friend WithEvents StokGudang As DataGridViewTextBoxColumn
End Class



