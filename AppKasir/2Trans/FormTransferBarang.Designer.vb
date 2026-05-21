<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormTransferBarang
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTransferBarang))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.BtnKeluarForm = New System.Windows.Forms.Button()
        Me.GBGrantotal = New System.Windows.Forms.GroupBox()
        Me.TxtGrandtotal = New System.Windows.Forms.TextBox()
        Me.TxtKomputer = New System.Windows.Forms.TextBox()
        Me.GBInput = New System.Windows.Forms.GroupBox()
        Me.LblTujuanTransfer = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblJenisTrans = New System.Windows.Forms.Label()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.LblLokasiBarang = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtLogin = New System.Windows.Forms.TextBox()
        Me.BtnSimpann = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtTotalRupiah = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtTotalQTY = New System.Windows.Forms.TextBox()
        Me.TxtBarcode = New System.Windows.Forms.TextBox()
        Me.TxtHarga = New System.Windows.Forms.TextBox()
        Me.TxtIsi = New System.Windows.Forms.TextBox()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.PanelFooter = New System.Windows.Forms.Panel()
        Me.BtnSettingPrinter = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblRecord = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.HapusToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TxtStok = New System.Windows.Forms.TextBox()
        Me.Txtsatuan = New System.Windows.Forms.TextBox()
        Me.TxtQty = New System.Windows.Forms.TextBox()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.Id = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Nama = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hargabeli = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Qty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Satuan = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Isi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HargaBeliSat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QtySat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Totalharga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Stok = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PanelHeader.SuspendLayout()
        Me.GBGrantotal.SuspendLayout()
        Me.GBInput.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        Me.PanelFooter.SuspendLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.SandyBrown
        Me.PanelHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PanelHeader.Controls.Add(Me.BtnKeluar)
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Controls.Add(Me.BtnKeluarForm)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1230, 39)
        Me.PanelHeader.TabIndex = 135
        '
        'BtnKeluar
        '
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.BtnKeluar.Location = New System.Drawing.Point(1110, 1)
        Me.BtnKeluar.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(112, 31)
        Me.BtnKeluar.TabIndex = 78
        Me.BtnKeluar.Text = "Keluar (Esc)"
        Me.BtnKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'LblHeader
        '
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold)
        Me.LblHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(1226, 35)
        Me.LblHeader.TabIndex = 1
        Me.LblHeader.Text = "TRANSFER STOK DARI TOKO KE GUDANG BOSS"
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
        Me.BtnKeluarForm.Location = New System.Drawing.Point(1111, 1)
        Me.BtnKeluarForm.Name = "BtnKeluarForm"
        Me.BtnKeluarForm.Size = New System.Drawing.Size(112, 31)
        Me.BtnKeluarForm.TabIndex = 77
        Me.BtnKeluarForm.Text = "Keluar (Esc)"
        Me.BtnKeluarForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarForm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluarForm.UseVisualStyleBackColor = False
        '
        'GBGrantotal
        '
        Me.GBGrantotal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBGrantotal.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GBGrantotal.Controls.Add(Me.TxtGrandtotal)
        Me.GBGrantotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBGrantotal.ForeColor = System.Drawing.Color.Black
        Me.GBGrantotal.Location = New System.Drawing.Point(568, 16)
        Me.GBGrantotal.Name = "GBGrantotal"
        Me.GBGrantotal.Size = New System.Drawing.Size(656, 87)
        Me.GBGrantotal.TabIndex = 3
        Me.GBGrantotal.TabStop = False
        Me.GBGrantotal.Text = "Grand Total"
        '
        'TxtGrandtotal
        '
        Me.TxtGrandtotal.BackColor = System.Drawing.Color.Black
        Me.TxtGrandtotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtGrandtotal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TxtGrandtotal.Font = New System.Drawing.Font("Digital-7", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGrandtotal.ForeColor = System.Drawing.Color.Lime
        Me.TxtGrandtotal.Location = New System.Drawing.Point(3, 25)
        Me.TxtGrandtotal.Multiline = True
        Me.TxtGrandtotal.Name = "TxtGrandtotal"
        Me.TxtGrandtotal.ReadOnly = True
        Me.TxtGrandtotal.Size = New System.Drawing.Size(650, 59)
        Me.TxtGrandtotal.TabIndex = 8
        Me.TxtGrandtotal.Text = "000"
        Me.TxtGrandtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtKomputer
        '
        Me.TxtKomputer.BackColor = System.Drawing.Color.White
        Me.TxtKomputer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtKomputer.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKomputer.ForeColor = System.Drawing.Color.Black
        Me.TxtKomputer.Location = New System.Drawing.Point(784, 21)
        Me.TxtKomputer.Name = "TxtKomputer"
        Me.TxtKomputer.ReadOnly = True
        Me.TxtKomputer.Size = New System.Drawing.Size(73, 15)
        Me.TxtKomputer.TabIndex = 124
        Me.TxtKomputer.Text = "Komputer"
        Me.TxtKomputer.Visible = False
        '
        'GBInput
        '
        Me.GBInput.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GBInput.Controls.Add(Me.LblTujuanTransfer)
        Me.GBInput.Controls.Add(Me.Label4)
        Me.GBInput.Controls.Add(Me.LblJenisTrans)
        Me.GBInput.Controls.Add(Me.DTPTgl)
        Me.GBInput.Controls.Add(Me.LblLokasiBarang)
        Me.GBInput.Controls.Add(Me.Label6)
        Me.GBInput.Controls.Add(Me.TxtFaktur)
        Me.GBInput.Controls.Add(Me.Label3)
        Me.GBInput.Controls.Add(Me.Label1)
        Me.GBInput.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBInput.ForeColor = System.Drawing.Color.White
        Me.GBInput.Location = New System.Drawing.Point(3, 16)
        Me.GBInput.Name = "GBInput"
        Me.GBInput.Size = New System.Drawing.Size(562, 87)
        Me.GBInput.TabIndex = 2
        Me.GBInput.TabStop = False
        '
        'LblTujuanTransfer
        '
        Me.LblTujuanTransfer.BackColor = System.Drawing.Color.Transparent
        Me.LblTujuanTransfer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTujuanTransfer.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTujuanTransfer.ForeColor = System.Drawing.Color.Black
        Me.LblTujuanTransfer.Location = New System.Drawing.Point(392, 39)
        Me.LblTujuanTransfer.Name = "LblTujuanTransfer"
        Me.LblTujuanTransfer.Size = New System.Drawing.Size(156, 21)
        Me.LblTujuanTransfer.TabIndex = 123
        Me.LblTujuanTransfer.Text = "0"
        Me.LblTujuanTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(286, 41)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(95, 16)
        Me.Label4.TabIndex = 122
        Me.Label4.Text = "Stok masuk ke"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblJenisTrans
        '
        Me.LblJenisTrans.BackColor = System.Drawing.Color.Transparent
        Me.LblJenisTrans.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJenisTrans.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisTrans.ForeColor = System.Drawing.Color.Black
        Me.LblJenisTrans.Location = New System.Drawing.Point(6, 62)
        Me.LblJenisTrans.Name = "LblJenisTrans"
        Me.LblJenisTrans.Size = New System.Drawing.Size(242, 21)
        Me.LblJenisTrans.TabIndex = 121
        Me.LblJenisTrans.Text = "TambahTransfer"
        Me.LblJenisTrans.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.LblJenisTrans.Visible = False
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(70, 38)
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
        Me.LblLokasiBarang.Location = New System.Drawing.Point(392, 13)
        Me.LblLokasiBarang.Name = "LblLokasiBarang"
        Me.LblLokasiBarang.Size = New System.Drawing.Size(156, 21)
        Me.LblLokasiBarang.TabIndex = 120
        Me.LblLokasiBarang.Text = "0"
        Me.LblLokasiBarang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(286, 13)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 16)
        Me.Label6.TabIndex = 120
        Me.Label6.Text = "Stok keluar dari"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.ForeColor = System.Drawing.Color.Black
        Me.TxtFaktur.Location = New System.Drawing.Point(72, 10)
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
        Me.Label3.Location = New System.Drawing.Point(6, 41)
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
        Me.Label1.Location = New System.Drawing.Point(15, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 16)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Nomor"
        '
        'TxtLogin
        '
        Me.TxtLogin.BackColor = System.Drawing.Color.White
        Me.TxtLogin.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtLogin.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLogin.ForeColor = System.Drawing.Color.Black
        Me.TxtLogin.Location = New System.Drawing.Point(727, 21)
        Me.TxtLogin.Name = "TxtLogin"
        Me.TxtLogin.ReadOnly = True
        Me.TxtLogin.Size = New System.Drawing.Size(55, 15)
        Me.TxtLogin.TabIndex = 123
        Me.TxtLogin.Text = "Login"
        Me.TxtLogin.Visible = False
        '
        'BtnSimpann
        '
        Me.BtnSimpann.AutoSize = True
        Me.BtnSimpann.BackColor = System.Drawing.Color.White
        Me.BtnSimpann.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpann.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpann.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpann.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpann.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpann.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpann.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpann.Image = CType(resources.GetObject("BtnSimpann.Image"), System.Drawing.Image)
        Me.BtnSimpann.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpann.Location = New System.Drawing.Point(880, 7)
        Me.BtnSimpann.Name = "BtnSimpann"
        Me.BtnSimpann.Size = New System.Drawing.Size(114, 41)
        Me.BtnSimpann.TabIndex = 113
        Me.BtnSimpann.Text = "Simpan (F8)"
        Me.BtnSimpann.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpann.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpann.UseVisualStyleBackColor = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(383, 7)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(84, 16)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "Total Rupiah"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label7.Visible = False
        '
        'TxtTotalRupiah
        '
        Me.TxtTotalRupiah.BackColor = System.Drawing.Color.LightSkyBlue
        Me.TxtTotalRupiah.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTotalRupiah.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalRupiah.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalRupiah.Location = New System.Drawing.Point(485, 7)
        Me.TxtTotalRupiah.Name = "TxtTotalRupiah"
        Me.TxtTotalRupiah.ReadOnly = True
        Me.TxtTotalRupiah.Size = New System.Drawing.Size(160, 15)
        Me.TxtTotalRupiah.TabIndex = 13
        Me.TxtTotalRupiah.Text = "0"
        Me.TxtTotalRupiah.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(151, 7)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(61, 16)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Total Qty"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtTotalQTY
        '
        Me.TxtTotalQTY.BackColor = System.Drawing.Color.LightSkyBlue
        Me.TxtTotalQTY.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTotalQTY.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalQTY.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalQTY.Location = New System.Drawing.Point(235, 7)
        Me.TxtTotalQTY.Name = "TxtTotalQTY"
        Me.TxtTotalQTY.ReadOnly = True
        Me.TxtTotalQTY.Size = New System.Drawing.Size(51, 15)
        Me.TxtTotalQTY.TabIndex = 8
        Me.TxtTotalQTY.Text = "0"
        '
        'TxtBarcode
        '
        Me.TxtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBarcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBarcode.Location = New System.Drawing.Point(1065, 110)
        Me.TxtBarcode.Name = "TxtBarcode"
        Me.TxtBarcode.ReadOnly = True
        Me.TxtBarcode.Size = New System.Drawing.Size(139, 22)
        Me.TxtBarcode.TabIndex = 8
        Me.TxtBarcode.Text = "Barcode"
        Me.TxtBarcode.Visible = False
        '
        'TxtHarga
        '
        Me.TxtHarga.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtHarga.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHarga.Location = New System.Drawing.Point(920, 110)
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
        Me.TxtIsi.Location = New System.Drawing.Point(852, 110)
        Me.TxtIsi.Name = "TxtIsi"
        Me.TxtIsi.ReadOnly = True
        Me.TxtIsi.Size = New System.Drawing.Size(64, 22)
        Me.TxtIsi.TabIndex = 8
        Me.TxtIsi.Text = "Isi"
        Me.TxtIsi.Visible = False
        '
        'PanelCari
        '
        Me.PanelCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCari.Controls.Add(Me.BtnCari)
        Me.PanelCari.Controls.Add(Me.TxtNama)
        Me.PanelCari.Location = New System.Drawing.Point(3, 102)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(562, 36)
        Me.PanelCari.TabIndex = 1
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
        Me.BtnCari.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCari.ForeColor = System.Drawing.Color.Black
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.Location = New System.Drawing.Point(535, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(27, 26)
        Me.BtnCari.TabIndex = 3
        Me.BtnCari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCari.UseVisualStyleBackColor = False
        '
        'TxtNama
        '
        Me.TxtNama.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtNama.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 11.25!)
        Me.TxtNama.Location = New System.Drawing.Point(3, 5)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(533, 26)
        Me.TxtNama.TabIndex = 1
        Me.TxtNama.Text = "Nama"
        '
        'PanelFooter
        '
        Me.PanelFooter.BackColor = System.Drawing.Color.LightSkyBlue
        Me.PanelFooter.Controls.Add(Me.BtnSettingPrinter)
        Me.PanelFooter.Controls.Add(Me.Label2)
        Me.PanelFooter.Controls.Add(Me.LblRecord)
        Me.PanelFooter.Controls.Add(Me.TxtKomputer)
        Me.PanelFooter.Controls.Add(Me.TxtLogin)
        Me.PanelFooter.Controls.Add(Me.BtnSimpann)
        Me.PanelFooter.Controls.Add(Me.TxtTotalRupiah)
        Me.PanelFooter.Controls.Add(Me.TxtTotalQTY)
        Me.PanelFooter.Controls.Add(Me.Label5)
        Me.PanelFooter.Controls.Add(Me.Label7)
        Me.PanelFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelFooter.Location = New System.Drawing.Point(0, 555)
        Me.PanelFooter.Name = "PanelFooter"
        Me.PanelFooter.Size = New System.Drawing.Size(1230, 55)
        Me.PanelFooter.TabIndex = 133
        '
        'BtnSettingPrinter
        '
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
        Me.BtnSettingPrinter.Location = New System.Drawing.Point(1142, 7)
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
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(4, 7)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 16)
        Me.Label2.TabIndex = 126
        Me.Label2.Text = "Record"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblRecord
        '
        Me.LblRecord.AutoSize = True
        Me.LblRecord.BackColor = System.Drawing.Color.Transparent
        Me.LblRecord.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRecord.ForeColor = System.Drawing.Color.Black
        Me.LblRecord.Location = New System.Drawing.Point(62, 7)
        Me.LblRecord.Name = "LblRecord"
        Me.LblRecord.Size = New System.Drawing.Size(52, 16)
        Me.LblRecord.TabIndex = 125
        Me.LblRecord.Text = "Record"
        Me.LblRecord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'TxtKode
        '
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(642, 110)
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
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Id, Me.Nama, Me.Hargabeli, Me.Qty, Me.Satuan, Me.Isi, Me.HargaBeliSat, Me.QtySat, Me.Totalharga, Me.Stok})
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvData.DefaultCellStyle = DataGridViewCellStyle10
        Me.DgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke
        Me.DgvData.Location = New System.Drawing.Point(3, 178)
        Me.DgvData.Name = "DgvData"
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.RowHeadersDefaultCellStyle = DataGridViewCellStyle11
        Me.DgvData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DgvData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DgvData.Size = New System.Drawing.Size(1227, 376)
        Me.DgvData.TabIndex = 132
        '
        'HapusToolStripMenuItem
        '
        Me.HapusToolStripMenuItem.Name = "HapusToolStripMenuItem"
        Me.HapusToolStripMenuItem.Size = New System.Drawing.Size(170, 22)
        Me.HapusToolStripMenuItem.Text = "Hapus barang"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HapusToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.ShowCheckMargin = True
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(171, 26)
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.GroupBox1.Controls.Add(Me.TxtStok)
        Me.GroupBox1.Controls.Add(Me.PanelCari)
        Me.GroupBox1.Controls.Add(Me.GBGrantotal)
        Me.GroupBox1.Controls.Add(Me.GBInput)
        Me.GroupBox1.Controls.Add(Me.TxtBarcode)
        Me.GroupBox1.Controls.Add(Me.TxtHarga)
        Me.GroupBox1.Controls.Add(Me.TxtIsi)
        Me.GroupBox1.Controls.Add(Me.Txtsatuan)
        Me.GroupBox1.Controls.Add(Me.TxtQty)
        Me.GroupBox1.Controls.Add(Me.TxtKode)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(3, 38)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1227, 142)
        Me.GroupBox1.TabIndex = 131
        Me.GroupBox1.TabStop = False
        '
        'TxtStok
        '
        Me.TxtStok.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStok.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStok.Location = New System.Drawing.Point(990, 109)
        Me.TxtStok.Name = "TxtStok"
        Me.TxtStok.ReadOnly = True
        Me.TxtStok.Size = New System.Drawing.Size(64, 22)
        Me.TxtStok.TabIndex = 9
        Me.TxtStok.Text = "Stok"
        Me.TxtStok.Visible = False
        '
        'Txtsatuan
        '
        Me.Txtsatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Txtsatuan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Txtsatuan.Location = New System.Drawing.Point(782, 110)
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
        Me.TxtQty.Location = New System.Drawing.Point(712, 110)
        Me.TxtQty.Name = "TxtQty"
        Me.TxtQty.ReadOnly = True
        Me.TxtQty.Size = New System.Drawing.Size(64, 22)
        Me.TxtQty.TabIndex = 8
        Me.TxtQty.Text = "Qty"
        Me.TxtQty.Visible = False
        '
        'LstBarang
        '
        Me.LstBarang.Font = New System.Drawing.Font("Century Gothic", 11.25!)
        Me.LstBarang.FormattingEnabled = True
        Me.LstBarang.ItemHeight = 20
        Me.LstBarang.Location = New System.Drawing.Point(8, 172)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(533, 284)
        Me.LstBarang.TabIndex = 136
        '
        'Id
        '
        Me.Id.FillWeight = 50.0!
        Me.Id.HeaderText = "Id"
        Me.Id.Name = "Id"
        Me.Id.Visible = False
        '
        'Nama
        '
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        Me.Nama.DefaultCellStyle = DataGridViewCellStyle3
        Me.Nama.FillWeight = 300.0!
        Me.Nama.HeaderText = "Nama"
        Me.Nama.Name = "Nama"
        '
        'Hargabeli
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "N0"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.Hargabeli.DefaultCellStyle = DataGridViewCellStyle4
        Me.Hargabeli.FillWeight = 60.0!
        Me.Hargabeli.HeaderText = "Harga Beli"
        Me.Hargabeli.Name = "Hargabeli"
        Me.Hargabeli.ReadOnly = True
        '
        'Qty
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle5.Format = "N0"
        DataGridViewCellStyle5.NullValue = Nothing
        Me.Qty.DefaultCellStyle = DataGridViewCellStyle5
        Me.Qty.FillWeight = 30.0!
        Me.Qty.HeaderText = "Qty"
        Me.Qty.Name = "Qty"
        Me.Qty.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Qty.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Satuan
        '
        Me.Satuan.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox
        Me.Satuan.FillWeight = 50.0!
        Me.Satuan.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Satuan.HeaderText = "Satuan"
        Me.Satuan.Name = "Satuan"
        '
        'Isi
        '
        Me.Isi.FillWeight = 20.0!
        Me.Isi.HeaderText = "Isi"
        Me.Isi.Name = "Isi"
        Me.Isi.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Isi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Isi.Visible = False
        '
        'HargaBeliSat
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle6.Format = "N0"
        DataGridViewCellStyle6.NullValue = Nothing
        Me.HargaBeliSat.DefaultCellStyle = DataGridViewCellStyle6
        Me.HargaBeliSat.HeaderText = "Harga Beli Sat"
        Me.HargaBeliSat.Name = "HargaBeliSat"
        Me.HargaBeliSat.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HargaBeliSat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'QtySat
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle7.Format = "N0"
        DataGridViewCellStyle7.NullValue = Nothing
        Me.QtySat.DefaultCellStyle = DataGridViewCellStyle7
        Me.QtySat.FillWeight = 40.0!
        Me.QtySat.HeaderText = "QtySat"
        Me.QtySat.Name = "QtySat"
        '
        'Totalharga
        '
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle8.Format = "N0"
        DataGridViewCellStyle8.NullValue = Nothing
        Me.Totalharga.DefaultCellStyle = DataGridViewCellStyle8
        Me.Totalharga.FillWeight = 80.0!
        Me.Totalharga.HeaderText = "Total Harga"
        Me.Totalharga.Name = "Totalharga"
        Me.Totalharga.ReadOnly = True
        Me.Totalharga.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Totalharga.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Stok
        '
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle9.Format = "N0"
        Me.Stok.DefaultCellStyle = DataGridViewCellStyle9
        Me.Stok.FillWeight = 40.0!
        Me.Stok.HeaderText = "Stok"
        Me.Stok.Name = "Stok"
        '
        'FormTransferBarang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1230, 610)
        Me.Controls.Add(Me.LstBarang)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.PanelFooter)
        Me.Controls.Add(Me.DgvData)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormTransferBarang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.GBGrantotal.ResumeLayout(False)
        Me.GBGrantotal.PerformLayout()
        Me.GBInput.ResumeLayout(False)
        Me.GBInput.PerformLayout()
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.PanelFooter.ResumeLayout(False)
        Me.PanelFooter.PerformLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents LblHeader As System.Windows.Forms.Label
    Friend WithEvents GBGrantotal As System.Windows.Forms.GroupBox
    Friend WithEvents TxtGrandtotal As System.Windows.Forms.TextBox
    Friend WithEvents TxtKomputer As System.Windows.Forms.TextBox
    Friend WithEvents GBInput As System.Windows.Forms.GroupBox
    Friend WithEvents DTPTgl As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblLokasiBarang As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TxtFaktur As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents BtnKeluarForm As System.Windows.Forms.Button
    Friend WithEvents TxtLogin As System.Windows.Forms.TextBox
    Friend WithEvents BtnSimpann As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalRupiah As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalQTY As System.Windows.Forms.TextBox
    Friend WithEvents TxtBarcode As System.Windows.Forms.TextBox
    Friend WithEvents TxtHarga As System.Windows.Forms.TextBox
    Friend WithEvents TxtIsi As System.Windows.Forms.TextBox
    Friend WithEvents PanelCari As System.Windows.Forms.Panel
    Friend WithEvents TxtNama As System.Windows.Forms.TextBox
    Friend WithEvents PanelFooter As System.Windows.Forms.Panel
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents DgvData As System.Windows.Forms.DataGridView
    Friend WithEvents HapusToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Txtsatuan As System.Windows.Forms.TextBox
    Friend WithEvents TxtQty As System.Windows.Forms.TextBox
    Friend WithEvents LstBarang As System.Windows.Forms.ListBox
    Friend WithEvents LblRecord As System.Windows.Forms.Label
    Friend WithEvents LblJenisTrans As System.Windows.Forms.Label
    Friend WithEvents LblTujuanTransfer As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtStok As TextBox
    Friend WithEvents BtnSettingPrinter As Button
    Friend WithEvents BtnCari As Button
    Friend WithEvents BtnKeluar As Button
    Friend WithEvents Id As DataGridViewTextBoxColumn
    Friend WithEvents Nama As DataGridViewTextBoxColumn
    Friend WithEvents Hargabeli As DataGridViewTextBoxColumn
    Friend WithEvents Qty As DataGridViewTextBoxColumn
    Friend WithEvents Satuan As DataGridViewComboBoxColumn
    Friend WithEvents Isi As DataGridViewTextBoxColumn
    Friend WithEvents HargaBeliSat As DataGridViewTextBoxColumn
    Friend WithEvents QtySat As DataGridViewTextBoxColumn
    Friend WithEvents Totalharga As DataGridViewTextBoxColumn
    Friend WithEvents Stok As DataGridViewTextBoxColumn
End Class


