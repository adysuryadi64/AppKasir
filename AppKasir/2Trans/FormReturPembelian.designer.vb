<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormReturPembelian
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormReturPembelian))
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.DGVReturPembelian = New System.Windows.Forms.DataGridView()
        Me.ID_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMA_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA_BELI = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SATUAN = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.ISI_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA_BELI_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY_SAT = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TOTAL = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LblNoNotaRetur = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.RTBAlasanRetur = New System.Windows.Forms.RichTextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.LblKodeSupplier = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.LblAlamatSupplier = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.LblStatusBeli = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.LblSisaBayar = New System.Windows.Forms.Label()
        Me.TxtSisaBayar = New System.Windows.Forms.TextBox()
        Me.PanelNota = New System.Windows.Forms.Panel()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.LblBayarBeli = New System.Windows.Forms.Label()
        Me.PBcariNotaBeli = New System.Windows.Forms.PictureBox()
        Me.TxtBayarBeli = New System.Windows.Forms.TextBox()
        Me.TxtNotaBeli = New System.Windows.Forms.TextBox()
        Me.DTPtglBeli = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CbJenisRetur = New System.Windows.Forms.CheckBox()
        Me.CmbSupplier = New System.Windows.Forms.ComboBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.LblKontakSupplier = New System.Windows.Forms.Label()
        Me.PanelSimpan = New System.Windows.Forms.Panel()
        Me.LblStatusHutang = New System.Windows.Forms.Label()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.LblKodeAkun = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CbPotongHutang = New System.Windows.Forms.CheckBox()
        Me.CbTunai = New System.Windows.Forms.CheckBox()
        Me.LblTotalQTY = New System.Windows.Forms.Label()
        Me.LblTotalBarang = New System.Windows.Forms.Label()
        Me.LblTotalRupiah = New System.Windows.Forms.Label()
        Me.TxtTotalBarang = New System.Windows.Forms.TextBox()
        Me.TxtTotalQTY = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtTotalRupiah = New System.Windows.Forms.TextBox()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.DTPRetur = New System.Windows.Forms.DateTimePicker()
        Me.PanelDatagridview = New System.Windows.Forms.Panel()
        Me.BtnKeluarDaftar = New System.Windows.Forms.Button()
        Me.DtpBelanja = New System.Windows.Forms.DateTimePicker()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.DGVPembelian = New System.Windows.Forms.DataGridView()
        Me.CMSHapus = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TSMhapus = New System.Windows.Forms.ToolStripMenuItem()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Panelcaribarang = New System.Windows.Forms.Panel()
        Me.TxtCariRetur = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.BtnKeluarBarang = New System.Windows.Forms.Button()
        Me.LblPilihbarang = New System.Windows.Forms.Label()
        Me.DGVPilihBarang = New System.Windows.Forms.DataGridView()
        Me.BtnDaftarBarang = New System.Windows.Forms.Button()
        Me.LblLokasi = New System.Windows.Forms.Label()
        Me.PanelHeader.SuspendLayout()
        CType(Me.DGVReturPembelian, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelNota.SuspendLayout()
        CType(Me.PBcariNotaBeli, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelSimpan.SuspendLayout()
        Me.PanelDatagridview.SuspendLayout()
        CType(Me.DGVPembelian, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CMSHapus.SuspendLayout()
        Me.Panelcaribarang.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVPilihBarang, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Red
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.ForeColor = System.Drawing.Color.Black
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(1180, 5)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 28)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(659, 7)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(168, 23)
        Me.Label8.TabIndex = 20
        Me.Label8.Text = "RETUR PEMBELIAN"
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.SandyBrown
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.Label8)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1214, 36)
        Me.PanelHeader.TabIndex = 88
        '
        'DGVReturPembelian
        '
        Me.DGVReturPembelian.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVReturPembelian.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVReturPembelian.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVReturPembelian.BackgroundColor = System.Drawing.Color.White
        Me.DGVReturPembelian.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVReturPembelian.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ID_BARANG, Me.NAMA_BARANG, Me.HARGA_BELI, Me.QTY, Me.SATUAN, Me.ISI_SATUAN, Me.HARGA_BELI_SATUAN, Me.QTY_SAT, Me.TOTAL})
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Teal
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVReturPembelian.DefaultCellStyle = DataGridViewCellStyle4
        Me.DGVReturPembelian.Location = New System.Drawing.Point(4, 208)
        Me.DGVReturPembelian.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DGVReturPembelian.Name = "DGVReturPembelian"
        Me.DGVReturPembelian.RowHeadersVisible = False
        Me.DGVReturPembelian.Size = New System.Drawing.Size(913, 470)
        Me.DGVReturPembelian.TabIndex = 119
        '
        'ID_BARANG
        '
        Me.ID_BARANG.HeaderText = "ID BARANG"
        Me.ID_BARANG.Name = "ID_BARANG"
        Me.ID_BARANG.Visible = False
        '
        'NAMA_BARANG
        '
        Me.NAMA_BARANG.FillWeight = 300.0!
        Me.NAMA_BARANG.HeaderText = "NAMA BARANG"
        Me.NAMA_BARANG.Name = "NAMA_BARANG"
        '
        'HARGA_BELI
        '
        Me.HARGA_BELI.HeaderText = "HARGA BELI"
        Me.HARGA_BELI.Name = "HARGA_BELI"
        '
        'QTY
        '
        Me.QTY.FillWeight = 50.0!
        Me.QTY.HeaderText = "QTY"
        Me.QTY.Name = "QTY"
        '
        'SATUAN
        '
        Me.SATUAN.FillWeight = 75.0!
        Me.SATUAN.HeaderText = "SATUAN"
        Me.SATUAN.Name = "SATUAN"
        '
        'ISI_SATUAN
        '
        Me.ISI_SATUAN.HeaderText = "ISI_SATUAN"
        Me.ISI_SATUAN.Name = "ISI_SATUAN"
        Me.ISI_SATUAN.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ISI_SATUAN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.ISI_SATUAN.Visible = False
        '
        'HARGA_BELI_SATUAN
        '
        Me.HARGA_BELI_SATUAN.HeaderText = "HARGA_BELI_SATUAN"
        Me.HARGA_BELI_SATUAN.Name = "HARGA_BELI_SATUAN"
        Me.HARGA_BELI_SATUAN.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HARGA_BELI_SATUAN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.HARGA_BELI_SATUAN.Visible = False
        '
        'QTY_SAT
        '
        Me.QTY_SAT.HeaderText = "QTY_SAT"
        Me.QTY_SAT.Name = "QTY_SAT"
        Me.QTY_SAT.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.QTY_SAT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.QTY_SAT.Visible = False
        '
        'TOTAL
        '
        Me.TOTAL.HeaderText = "TOTAL"
        Me.TOTAL.Name = "TOTAL"
        Me.TOTAL.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TOTAL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(0, 36)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(3, 654)
        Me.Panel3.TabIndex = 118
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(1211, 36)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(3, 654)
        Me.Panel2.TabIndex = 117
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 690)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1214, 2)
        Me.Panel1.TabIndex = 116
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(23, 42)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(86, 17)
        Me.Label5.TabIndex = 178
        Me.Label5.Text = "Nota Retur :"
        '
        'LblNoNotaRetur
        '
        Me.LblNoNotaRetur.AutoSize = True
        Me.LblNoNotaRetur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNoNotaRetur.Location = New System.Drawing.Point(113, 42)
        Me.LblNoNotaRetur.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblNoNotaRetur.Name = "LblNoNotaRetur"
        Me.LblNoNotaRetur.Size = New System.Drawing.Size(68, 17)
        Me.LblNoNotaRetur.TabIndex = 169
        Me.LblNoNotaRetur.Text = "No. Nota"
        '
        'Label12
        '
        Me.Label12.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(930, 78)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(104, 16)
        Me.Label12.TabIndex = 207
        Me.Label12.Text = "Alasan diretur:"
        '
        'RTBAlasanRetur
        '
        Me.RTBAlasanRetur.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RTBAlasanRetur.BackColor = System.Drawing.Color.Ivory
        Me.RTBAlasanRetur.Location = New System.Drawing.Point(925, 95)
        Me.RTBAlasanRetur.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.RTBAlasanRetur.Name = "RTBAlasanRetur"
        Me.RTBAlasanRetur.Size = New System.Drawing.Size(282, 141)
        Me.RTBAlasanRetur.TabIndex = 206
        Me.RTBAlasanRetur.Text = ""
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(43, 98)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(66, 17)
        Me.Label6.TabIndex = 214
        Me.Label6.Text = "Supplier :"
        '
        'LblKodeSupplier
        '
        Me.LblKodeSupplier.AutoSize = True
        Me.LblKodeSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeSupplier.Location = New System.Drawing.Point(113, 124)
        Me.LblKodeSupplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeSupplier.Name = "LblKodeSupplier"
        Me.LblKodeSupplier.Size = New System.Drawing.Size(42, 17)
        Me.LblKodeSupplier.TabIndex = 215
        Me.LblKodeSupplier.Text = "Kode"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(45, 146)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(64, 17)
        Me.Label10.TabIndex = 219
        Me.Label10.Text = "Alamat :"
        '
        'LblAlamatSupplier
        '
        Me.LblAlamatSupplier.AutoSize = True
        Me.LblAlamatSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblAlamatSupplier.Location = New System.Drawing.Point(113, 146)
        Me.LblAlamatSupplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblAlamatSupplier.Name = "LblAlamatSupplier"
        Me.LblAlamatSupplier.Size = New System.Drawing.Size(56, 17)
        Me.LblAlamatSupplier.TabIndex = 218
        Me.LblAlamatSupplier.Text = "Alamat"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(11, 61)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(127, 17)
        Me.Label13.TabIndex = 221
        Me.Label13.Text = "Status Pembelian :"
        '
        'LblStatusBeli
        '
        Me.LblStatusBeli.AutoSize = True
        Me.LblStatusBeli.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatusBeli.Location = New System.Drawing.Point(139, 61)
        Me.LblStatusBeli.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblStatusBeli.Name = "LblStatusBeli"
        Me.LblStatusBeli.Size = New System.Drawing.Size(50, 17)
        Me.LblStatusBeli.TabIndex = 220
        Me.LblStatusBeli.Text = "Status "
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(60, 109)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(78, 17)
        Me.Label15.TabIndex = 223
        Me.Label15.Text = "Sisa Bayar :"
        '
        'LblSisaBayar
        '
        Me.LblSisaBayar.AutoSize = True
        Me.LblSisaBayar.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSisaBayar.Location = New System.Drawing.Point(139, 109)
        Me.LblSisaBayar.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSisaBayar.Name = "LblSisaBayar"
        Me.LblSisaBayar.Size = New System.Drawing.Size(31, 17)
        Me.LblSisaBayar.TabIndex = 222
        Me.LblSisaBayar.Text = "Sisa"
        '
        'TxtSisaBayar
        '
        Me.TxtSisaBayar.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSisaBayar.ForeColor = System.Drawing.Color.Green
        Me.TxtSisaBayar.Location = New System.Drawing.Point(234, 106)
        Me.TxtSisaBayar.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtSisaBayar.Name = "TxtSisaBayar"
        Me.TxtSisaBayar.Size = New System.Drawing.Size(85, 23)
        Me.TxtSisaBayar.TabIndex = 224
        Me.TxtSisaBayar.Text = "Sisa"
        Me.TxtSisaBayar.Visible = False
        '
        'PanelNota
        '
        Me.PanelNota.BackColor = System.Drawing.Color.Wheat
        Me.PanelNota.Controls.Add(Me.Label14)
        Me.PanelNota.Controls.Add(Me.LblBayarBeli)
        Me.PanelNota.Controls.Add(Me.PBcariNotaBeli)
        Me.PanelNota.Controls.Add(Me.TxtBayarBeli)
        Me.PanelNota.Controls.Add(Me.TxtSisaBayar)
        Me.PanelNota.Controls.Add(Me.Label15)
        Me.PanelNota.Controls.Add(Me.TxtNotaBeli)
        Me.PanelNota.Controls.Add(Me.LblSisaBayar)
        Me.PanelNota.Controls.Add(Me.DTPtglBeli)
        Me.PanelNota.Controls.Add(Me.Label13)
        Me.PanelNota.Controls.Add(Me.LblStatusBeli)
        Me.PanelNota.Controls.Add(Me.Label3)
        Me.PanelNota.Controls.Add(Me.Label2)
        Me.PanelNota.Location = New System.Drawing.Point(361, 67)
        Me.PanelNota.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelNota.Name = "PanelNota"
        Me.PanelNota.Size = New System.Drawing.Size(391, 135)
        Me.PanelNota.TabIndex = 226
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(39, 84)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(99, 17)
        Me.Label14.TabIndex = 236
        Me.Label14.Text = "Pembayaran :"
        '
        'LblBayarBeli
        '
        Me.LblBayarBeli.AutoSize = True
        Me.LblBayarBeli.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBayarBeli.Location = New System.Drawing.Point(139, 84)
        Me.LblBayarBeli.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblBayarBeli.Name = "LblBayarBeli"
        Me.LblBayarBeli.Size = New System.Drawing.Size(43, 17)
        Me.LblBayarBeli.TabIndex = 235
        Me.LblBayarBeli.Text = "Bayar"
        '
        'PBcariNotaBeli
        '
        Me.PBcariNotaBeli.BackColor = System.Drawing.Color.White
        Me.PBcariNotaBeli.Image = CType(resources.GetObject("PBcariNotaBeli.Image"), System.Drawing.Image)
        Me.PBcariNotaBeli.Location = New System.Drawing.Point(347, 5)
        Me.PBcariNotaBeli.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PBcariNotaBeli.Name = "PBcariNotaBeli"
        Me.PBcariNotaBeli.Size = New System.Drawing.Size(31, 28)
        Me.PBcariNotaBeli.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PBcariNotaBeli.TabIndex = 231
        Me.PBcariNotaBeli.TabStop = False
        '
        'TxtBayarBeli
        '
        Me.TxtBayarBeli.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBayarBeli.ForeColor = System.Drawing.Color.Green
        Me.TxtBayarBeli.Location = New System.Drawing.Point(234, 81)
        Me.TxtBayarBeli.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtBayarBeli.Name = "TxtBayarBeli"
        Me.TxtBayarBeli.Size = New System.Drawing.Size(85, 23)
        Me.TxtBayarBeli.TabIndex = 224
        Me.TxtBayarBeli.Text = "bayar"
        Me.TxtBayarBeli.Visible = False
        '
        'TxtNotaBeli
        '
        Me.TxtNotaBeli.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaBeli.ForeColor = System.Drawing.Color.Green
        Me.TxtNotaBeli.Location = New System.Drawing.Point(139, 6)
        Me.TxtNotaBeli.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtNotaBeli.Name = "TxtNotaBeli"
        Me.TxtNotaBeli.Size = New System.Drawing.Size(200, 23)
        Me.TxtNotaBeli.TabIndex = 229
        '
        'DTPtglBeli
        '
        Me.DTPtglBeli.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPtglBeli.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPtglBeli.Location = New System.Drawing.Point(139, 33)
        Me.DTPtglBeli.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DTPtglBeli.Name = "DTPtglBeli"
        Me.DTPtglBeli.Size = New System.Drawing.Size(200, 23)
        Me.DTPtglBeli.TabIndex = 228
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(28, 36)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(110, 17)
        Me.Label3.TabIndex = 227
        Me.Label3.Text = "Tgl. Pembelian :"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(16, 10)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(122, 17)
        Me.Label2.TabIndex = 226
        Me.Label2.Text = "Nota Pembelian :"
        '
        'CbJenisRetur
        '
        Me.CbJenisRetur.AutoSize = True
        Me.CbJenisRetur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbJenisRetur.Location = New System.Drawing.Point(361, 46)
        Me.CbJenisRetur.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CbJenisRetur.Name = "CbJenisRetur"
        Me.CbJenisRetur.Size = New System.Drawing.Size(233, 21)
        Me.CbJenisRetur.TabIndex = 232
        Me.CbJenisRetur.Text = "Retur diluar transaksi pembelian"
        Me.CbJenisRetur.UseVisualStyleBackColor = True
        '
        'CmbSupplier
        '
        Me.CmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSupplier.FormattingEnabled = True
        Me.CmbSupplier.Location = New System.Drawing.Point(113, 94)
        Me.CmbSupplier.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CmbSupplier.Name = "CmbSupplier"
        Me.CmbSupplier.Size = New System.Drawing.Size(240, 25)
        Me.CmbSupplier.TabIndex = 227
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(47, 169)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(62, 17)
        Me.Label17.TabIndex = 229
        Me.Label17.Text = "Kontak :"
        '
        'LblKontakSupplier
        '
        Me.LblKontakSupplier.AutoSize = True
        Me.LblKontakSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKontakSupplier.Location = New System.Drawing.Point(113, 169)
        Me.LblKontakSupplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKontakSupplier.Name = "LblKontakSupplier"
        Me.LblKontakSupplier.Size = New System.Drawing.Size(54, 17)
        Me.LblKontakSupplier.TabIndex = 228
        Me.LblKontakSupplier.Text = "Kontak"
        '
        'PanelSimpan
        '
        Me.PanelSimpan.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelSimpan.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelSimpan.Controls.Add(Me.LblStatusHutang)
        Me.PanelSimpan.Controls.Add(Me.CmbRekening)
        Me.PanelSimpan.Controls.Add(Me.LblKodeAkun)
        Me.PanelSimpan.Controls.Add(Me.Label4)
        Me.PanelSimpan.Controls.Add(Me.CbPotongHutang)
        Me.PanelSimpan.Controls.Add(Me.CbTunai)
        Me.PanelSimpan.Controls.Add(Me.LblTotalQTY)
        Me.PanelSimpan.Controls.Add(Me.LblTotalBarang)
        Me.PanelSimpan.Controls.Add(Me.LblTotalRupiah)
        Me.PanelSimpan.Controls.Add(Me.TxtTotalBarang)
        Me.PanelSimpan.Controls.Add(Me.TxtTotalQTY)
        Me.PanelSimpan.Controls.Add(Me.Label11)
        Me.PanelSimpan.Controls.Add(Me.Label1)
        Me.PanelSimpan.Controls.Add(Me.Label7)
        Me.PanelSimpan.Controls.Add(Me.TxtTotalRupiah)
        Me.PanelSimpan.Controls.Add(Me.BtnSimpan)
        Me.PanelSimpan.Controls.Add(Me.Label9)
        Me.PanelSimpan.Location = New System.Drawing.Point(925, 242)
        Me.PanelSimpan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelSimpan.Name = "PanelSimpan"
        Me.PanelSimpan.Size = New System.Drawing.Size(282, 436)
        Me.PanelSimpan.TabIndex = 230
        '
        'LblStatusHutang
        '
        Me.LblStatusHutang.AutoSize = True
        Me.LblStatusHutang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatusHutang.Location = New System.Drawing.Point(8, 217)
        Me.LblStatusHutang.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblStatusHutang.Name = "LblStatusHutang"
        Me.LblStatusHutang.Size = New System.Drawing.Size(50, 17)
        Me.LblStatusHutang.TabIndex = 253
        Me.LblStatusHutang.Text = "Status "
        Me.LblStatusHutang.Visible = False
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Location = New System.Drawing.Point(8, 259)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(269, 25)
        Me.CmbRekening.TabIndex = 252
        '
        'LblKodeAkun
        '
        Me.LblKodeAkun.AutoSize = True
        Me.LblKodeAkun.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeAkun.Location = New System.Drawing.Point(8, 295)
        Me.LblKodeAkun.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeAkun.Name = "LblKodeAkun"
        Me.LblKodeAkun.Size = New System.Drawing.Size(42, 17)
        Me.LblKodeAkun.TabIndex = 251
        Me.LblKodeAkun.Text = "Kode"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(8, 238)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 17)
        Me.Label4.TabIndex = 249
        Me.Label4.Text = "Akun :"
        '
        'CbPotongHutang
        '
        Me.CbPotongHutang.AutoSize = True
        Me.CbPotongHutang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbPotongHutang.Location = New System.Drawing.Point(8, 173)
        Me.CbPotongHutang.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CbPotongHutang.Name = "CbPotongHutang"
        Me.CbPotongHutang.Size = New System.Drawing.Size(127, 21)
        Me.CbPotongHutang.TabIndex = 248
        Me.CbPotongHutang.Text = "Potong Hutang"
        Me.CbPotongHutang.UseVisualStyleBackColor = True
        '
        'CbTunai
        '
        Me.CbTunai.AutoSize = True
        Me.CbTunai.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTunai.Location = New System.Drawing.Point(8, 144)
        Me.CbTunai.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CbTunai.Name = "CbTunai"
        Me.CbTunai.Size = New System.Drawing.Size(60, 21)
        Me.CbTunai.TabIndex = 247
        Me.CbTunai.Text = "Tunai"
        Me.CbTunai.UseVisualStyleBackColor = True
        '
        'LblTotalQTY
        '
        Me.LblTotalQTY.AutoSize = True
        Me.LblTotalQTY.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalQTY.Location = New System.Drawing.Point(115, 45)
        Me.LblTotalQTY.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblTotalQTY.Name = "LblTotalQTY"
        Me.LblTotalQTY.Size = New System.Drawing.Size(40, 17)
        Me.LblTotalQTY.TabIndex = 238
        Me.LblTotalQTY.Text = "Rp. 0"
        '
        'LblTotalBarang
        '
        Me.LblTotalBarang.AutoSize = True
        Me.LblTotalBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalBarang.Location = New System.Drawing.Point(115, 15)
        Me.LblTotalBarang.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblTotalBarang.Name = "LblTotalBarang"
        Me.LblTotalBarang.Size = New System.Drawing.Size(40, 17)
        Me.LblTotalBarang.TabIndex = 239
        Me.LblTotalBarang.Text = "Rp. 0"
        '
        'LblTotalRupiah
        '
        Me.LblTotalRupiah.AutoSize = True
        Me.LblTotalRupiah.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalRupiah.Location = New System.Drawing.Point(115, 75)
        Me.LblTotalRupiah.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblTotalRupiah.Name = "LblTotalRupiah"
        Me.LblTotalRupiah.Size = New System.Drawing.Size(40, 17)
        Me.LblTotalRupiah.TabIndex = 237
        Me.LblTotalRupiah.Text = "Rp. 0"
        '
        'TxtTotalBarang
        '
        Me.TxtTotalBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalBarang.ForeColor = System.Drawing.Color.Green
        Me.TxtTotalBarang.Location = New System.Drawing.Point(154, 12)
        Me.TxtTotalBarang.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTotalBarang.Name = "TxtTotalBarang"
        Me.TxtTotalBarang.Size = New System.Drawing.Size(113, 23)
        Me.TxtTotalBarang.TabIndex = 230
        Me.TxtTotalBarang.Visible = False
        '
        'TxtTotalQTY
        '
        Me.TxtTotalQTY.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalQTY.ForeColor = System.Drawing.Color.Green
        Me.TxtTotalQTY.Location = New System.Drawing.Point(154, 41)
        Me.TxtTotalQTY.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTotalQTY.Name = "TxtTotalQTY"
        Me.TxtTotalQTY.Size = New System.Drawing.Size(113, 23)
        Me.TxtTotalQTY.TabIndex = 229
        Me.TxtTotalQTY.Visible = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(44, 45)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(71, 17)
        Me.Label11.TabIndex = 224
        Me.Label11.Text = "Total qty :"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(18, 15)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(97, 17)
        Me.Label1.TabIndex = 224
        Me.Label1.Text = "Total Barang :"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(19, 75)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(96, 17)
        Me.Label7.TabIndex = 223
        Me.Label7.Text = "Total Rupiah :"
        '
        'TxtTotalRupiah
        '
        Me.TxtTotalRupiah.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalRupiah.ForeColor = System.Drawing.Color.Green
        Me.TxtTotalRupiah.Location = New System.Drawing.Point(154, 72)
        Me.TxtTotalRupiah.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTotalRupiah.Name = "TxtTotalRupiah"
        Me.TxtTotalRupiah.Size = New System.Drawing.Size(113, 23)
        Me.TxtTotalRupiah.TabIndex = 221
        Me.TxtTotalRupiah.Visible = False
        '
        'BtnSimpan
        '
        Me.BtnSimpan.BackColor = System.Drawing.Color.FromArgb(CType(CType(36, Byte), Integer), CType(CType(91, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.BorderSize = 0
        Me.BtnSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpan.ForeColor = System.Drawing.Color.White
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.Location = New System.Drawing.Point(63, 318)
        Me.BtnSimpan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(148, 36)
        Me.BtnSimpan.TabIndex = 220
        Me.BtnSimpan.Text = "   SIMPAN (F8)"
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(8, 116)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(154, 17)
        Me.Label9.TabIndex = 219
        Me.Label9.Text = "Metode pembayaran :"
        '
        'DTPRetur
        '
        Me.DTPRetur.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPRetur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPRetur.Location = New System.Drawing.Point(980, 42)
        Me.DTPRetur.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DTPRetur.Name = "DTPRetur"
        Me.DTPRetur.Size = New System.Drawing.Size(200, 23)
        Me.DTPRetur.TabIndex = 231
        Me.DTPRetur.Visible = False
        '
        'PanelDatagridview
        '
        Me.PanelDatagridview.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelDatagridview.BackColor = System.Drawing.Color.Chocolate
        Me.PanelDatagridview.Controls.Add(Me.BtnKeluarDaftar)
        Me.PanelDatagridview.Controls.Add(Me.DtpBelanja)
        Me.PanelDatagridview.Controls.Add(Me.Label18)
        Me.PanelDatagridview.Controls.Add(Me.DGVPembelian)
        Me.PanelDatagridview.Location = New System.Drawing.Point(16, 304)
        Me.PanelDatagridview.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelDatagridview.Name = "PanelDatagridview"
        Me.PanelDatagridview.Size = New System.Drawing.Size(807, 406)
        Me.PanelDatagridview.TabIndex = 232
        '
        'BtnKeluarDaftar
        '
        Me.BtnKeluarDaftar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluarDaftar.BackColor = System.Drawing.Color.LightSeaGreen
        Me.BtnKeluarDaftar.FlatAppearance.BorderSize = 0
        Me.BtnKeluarDaftar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
        Me.BtnKeluarDaftar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluarDaftar.ForeColor = System.Drawing.Color.White
        Me.BtnKeluarDaftar.Image = CType(resources.GetObject("BtnKeluarDaftar.Image"), System.Drawing.Image)
        Me.BtnKeluarDaftar.Location = New System.Drawing.Point(772, 10)
        Me.BtnKeluarDaftar.Name = "BtnKeluarDaftar"
        Me.BtnKeluarDaftar.Size = New System.Drawing.Size(23, 24)
        Me.BtnKeluarDaftar.TabIndex = 232
        Me.BtnKeluarDaftar.UseVisualStyleBackColor = False
        '
        'DtpBelanja
        '
        Me.DtpBelanja.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DtpBelanja.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpBelanja.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpBelanja.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpBelanja.Location = New System.Drawing.Point(356, 13)
        Me.DtpBelanja.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpBelanja.Name = "DtpBelanja"
        Me.DtpBelanja.Size = New System.Drawing.Size(108, 23)
        Me.DtpBelanja.TabIndex = 231
        '
        'Label18
        '
        Me.Label18.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.ForeColor = System.Drawing.Color.White
        Me.Label18.Location = New System.Drawing.Point(20, 15)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(329, 17)
        Me.Label18.TabIndex = 230
        Me.Label18.Text = "Pilih tanggal belanja untuk mencari nomor nota :"
        '
        'DGVPembelian
        '
        Me.DGVPembelian.AllowUserToAddRows = False
        Me.DGVPembelian.AllowUserToDeleteRows = False
        Me.DGVPembelian.AllowUserToResizeRows = False
        Me.DGVPembelian.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVPembelian.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVPembelian.BackgroundColor = System.Drawing.Color.White
        Me.DGVPembelian.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken
        Me.DGVPembelian.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVPembelian.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.DGVPembelian.Location = New System.Drawing.Point(0, 44)
        Me.DGVPembelian.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DGVPembelian.Name = "DGVPembelian"
        Me.DGVPembelian.RowHeadersVisible = False
        Me.DGVPembelian.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVPembelian.Size = New System.Drawing.Size(807, 362)
        Me.DGVPembelian.TabIndex = 0
        '
        'CMSHapus
        '
        Me.CMSHapus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSMhapus})
        Me.CMSHapus.Name = "CMSHapus"
        Me.CMSHapus.Size = New System.Drawing.Size(149, 26)
        Me.CMSHapus.Text = "Hapus barang"
        '
        'TSMhapus
        '
        Me.TSMhapus.Name = "TSMhapus"
        Me.TSMhapus.Size = New System.Drawing.Size(148, 22)
        Me.TSMhapus.Text = "Hapus barang"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(59, 124)
        Me.Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(50, 17)
        Me.Label25.TabIndex = 219
        Me.Label25.Text = "Kode :"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(54, 65)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(55, 17)
        Me.Label16.TabIndex = 233
        Me.Label16.Text = "Lokasi :"
        '
        'Panelcaribarang
        '
        Me.Panelcaribarang.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panelcaribarang.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Panelcaribarang.Controls.Add(Me.TxtCariRetur)
        Me.Panelcaribarang.Controls.Add(Me.PictureBox1)
        Me.Panelcaribarang.Controls.Add(Me.BtnKeluarBarang)
        Me.Panelcaribarang.Controls.Add(Me.LblPilihbarang)
        Me.Panelcaribarang.Controls.Add(Me.DGVPilihBarang)
        Me.Panelcaribarang.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Panelcaribarang.Location = New System.Drawing.Point(16, 379)
        Me.Panelcaribarang.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panelcaribarang.Name = "Panelcaribarang"
        Me.Panelcaribarang.Size = New System.Drawing.Size(807, 406)
        Me.Panelcaribarang.TabIndex = 235
        Me.Panelcaribarang.Visible = False
        '
        'TxtCariRetur
        '
        Me.TxtCariRetur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCariRetur.ForeColor = System.Drawing.Color.Green
        Me.TxtCariRetur.Location = New System.Drawing.Point(4, 4)
        Me.TxtCariRetur.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtCariRetur.Name = "TxtCariRetur"
        Me.TxtCariRetur.Size = New System.Drawing.Size(379, 23)
        Me.TxtCariRetur.TabIndex = 233
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(381, 5)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(20, 21)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 232
        Me.PictureBox1.TabStop = False
        '
        'BtnKeluarBarang
        '
        Me.BtnKeluarBarang.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluarBarang.BackColor = System.Drawing.Color.Teal
        Me.BtnKeluarBarang.FlatAppearance.BorderSize = 0
        Me.BtnKeluarBarang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
        Me.BtnKeluarBarang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluarBarang.ForeColor = System.Drawing.Color.White
        Me.BtnKeluarBarang.Image = CType(resources.GetObject("BtnKeluarBarang.Image"), System.Drawing.Image)
        Me.BtnKeluarBarang.Location = New System.Drawing.Point(774, 3)
        Me.BtnKeluarBarang.Name = "BtnKeluarBarang"
        Me.BtnKeluarBarang.Size = New System.Drawing.Size(23, 24)
        Me.BtnKeluarBarang.TabIndex = 231
        Me.BtnKeluarBarang.UseVisualStyleBackColor = False
        '
        'LblPilihbarang
        '
        Me.LblPilihbarang.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblPilihbarang.AutoSize = True
        Me.LblPilihbarang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblPilihbarang.ForeColor = System.Drawing.Color.Black
        Me.LblPilihbarang.Location = New System.Drawing.Point(427, 7)
        Me.LblPilihbarang.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblPilihbarang.Name = "LblPilihbarang"
        Me.LblPilihbarang.Size = New System.Drawing.Size(172, 17)
        Me.LblPilihbarang.TabIndex = 230
        Me.LblPilihbarang.Text = "Cari barang yang di retur"
        '
        'DGVPilihBarang
        '
        Me.DGVPilihBarang.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVPilihBarang.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVPilihBarang.BackgroundColor = System.Drawing.Color.White
        Me.DGVPilihBarang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVPilihBarang.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.DGVPilihBarang.Location = New System.Drawing.Point(0, 31)
        Me.DGVPilihBarang.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DGVPilihBarang.Name = "DGVPilihBarang"
        Me.DGVPilihBarang.RowHeadersVisible = False
        Me.DGVPilihBarang.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVPilihBarang.Size = New System.Drawing.Size(807, 375)
        Me.DGVPilihBarang.TabIndex = 0
        '
        'BtnDaftarBarang
        '
        Me.BtnDaftarBarang.AutoSize = True
        Me.BtnDaftarBarang.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnDaftarBarang.FlatAppearance.BorderColor = System.Drawing.Color.White
        Me.BtnDaftarBarang.FlatAppearance.BorderSize = 0
        Me.BtnDaftarBarang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSeaGreen
        Me.BtnDaftarBarang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnDaftarBarang.Font = New System.Drawing.Font("Bookman Old Style", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnDaftarBarang.ForeColor = System.Drawing.Color.Black
        Me.BtnDaftarBarang.Image = CType(resources.GetObject("BtnDaftarBarang.Image"), System.Drawing.Image)
        Me.BtnDaftarBarang.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDaftarBarang.Location = New System.Drawing.Point(776, 163)
        Me.BtnDaftarBarang.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnDaftarBarang.Name = "BtnDaftarBarang"
        Me.BtnDaftarBarang.Size = New System.Drawing.Size(138, 38)
        Me.BtnDaftarBarang.TabIndex = 255
        Me.BtnDaftarBarang.Text = "Daftar barang"
        Me.BtnDaftarBarang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnDaftarBarang.UseVisualStyleBackColor = False
        '
        'LblLokasi
        '
        Me.LblLokasi.AutoSize = True
        Me.LblLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLokasi.Location = New System.Drawing.Point(113, 65)
        Me.LblLokasi.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblLokasi.Name = "LblLokasi"
        Me.LblLokasi.Size = New System.Drawing.Size(47, 17)
        Me.LblLokasi.TabIndex = 256
        Me.LblLokasi.Text = "Lokasi"
        '
        'FormReturPembelian
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1214, 692)
        Me.Controls.Add(Me.LblLokasi)
        Me.Controls.Add(Me.Panelcaribarang)
        Me.Controls.Add(Me.PanelDatagridview)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.PanelSimpan)
        Me.Controls.Add(Me.DTPRetur)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.CbJenisRetur)
        Me.Controls.Add(Me.LblKontakSupplier)
        Me.Controls.Add(Me.CmbSupplier)
        Me.Controls.Add(Me.PanelNota)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.LblAlamatSupplier)
        Me.Controls.Add(Me.LblKodeSupplier)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.RTBAlasanRetur)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.LblNoNotaRetur)
        Me.Controls.Add(Me.DGVReturPembelian)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.BtnDaftarBarang)
        Me.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Name = "FormReturPembelian"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "ReturPembelian"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        CType(Me.DGVReturPembelian, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelNota.ResumeLayout(False)
        Me.PanelNota.PerformLayout()
        CType(Me.PBcariNotaBeli, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelSimpan.ResumeLayout(False)
        Me.PanelSimpan.PerformLayout()
        Me.PanelDatagridview.ResumeLayout(False)
        Me.PanelDatagridview.PerformLayout()
        CType(Me.DGVPembelian, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CMSHapus.ResumeLayout(False)
        Me.Panelcaribarang.ResumeLayout(False)
        Me.Panelcaribarang.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVPilihBarang, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents DGVReturPembelian As System.Windows.Forms.DataGridView
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LblNoNotaRetur As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents RTBAlasanRetur As System.Windows.Forms.RichTextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents LblKodeSupplier As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents LblAlamatSupplier As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents LblStatusBeli As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents LblSisaBayar As System.Windows.Forms.Label
    Friend WithEvents TxtSisaBayar As System.Windows.Forms.TextBox
    Friend WithEvents PanelNota As System.Windows.Forms.Panel
    Friend WithEvents CbJenisRetur As System.Windows.Forms.CheckBox
    Friend WithEvents PBcariNotaBeli As System.Windows.Forms.PictureBox
    Friend WithEvents TxtNotaBeli As System.Windows.Forms.TextBox
    Friend WithEvents DTPtglBeli As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents CmbSupplier As System.Windows.Forms.ComboBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents LblKontakSupplier As System.Windows.Forms.Label
    Friend WithEvents PanelSimpan As System.Windows.Forms.Panel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalRupiah As System.Windows.Forms.TextBox
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents DTPRetur As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LblTotalQTY As System.Windows.Forms.Label
    Friend WithEvents LblTotalBarang As System.Windows.Forms.Label
    Friend WithEvents LblTotalRupiah As System.Windows.Forms.Label
    Friend WithEvents TxtTotalBarang As System.Windows.Forms.TextBox
    Friend WithEvents TxtTotalQTY As System.Windows.Forms.TextBox
    Friend WithEvents CbPotongHutang As System.Windows.Forms.CheckBox
    Friend WithEvents CbTunai As System.Windows.Forms.CheckBox
    Friend WithEvents PanelDatagridview As System.Windows.Forms.Panel
    Friend WithEvents DGVPembelian As System.Windows.Forms.DataGridView
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents LblBayarBeli As System.Windows.Forms.Label
    Friend WithEvents TxtBayarBeli As System.Windows.Forms.TextBox
    Friend WithEvents ID_BARANG As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NAMA_BARANG As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HARGA_BELI As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QTY As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SATUAN As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents ISI_SATUAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HARGA_BELI_SATUAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QTY_SAT As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TOTAL As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CMSHapus As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents TSMhapus As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LblKodeAkun As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents DtpBelanja As System.Windows.Forms.DateTimePicker
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents Panelcaribarang As System.Windows.Forms.Panel
    Friend WithEvents LblPilihbarang As System.Windows.Forms.Label
    Friend WithEvents DGVPilihBarang As System.Windows.Forms.DataGridView
    Friend WithEvents BtnKeluarDaftar As System.Windows.Forms.Button
    Friend WithEvents BtnKeluarBarang As System.Windows.Forms.Button
    Friend WithEvents BtnDaftarBarang As System.Windows.Forms.Button
    Friend WithEvents LblStatusHutang As System.Windows.Forms.Label
    Friend WithEvents LblLokasi As System.Windows.Forms.Label
    Friend WithEvents TxtCariRetur As TextBox
    Friend WithEvents PictureBox1 As PictureBox
End Class
