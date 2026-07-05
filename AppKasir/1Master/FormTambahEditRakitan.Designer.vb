<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormTambahEditRakitan
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTambahEditRakitan))
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.PanelInput = New System.Windows.Forms.Panel()
        Me.LblKode = New System.Windows.Forms.Label()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.LblNama = New System.Windows.Forms.Label()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.LblBarcode = New System.Windows.Forms.Label()
        Me.TxtBarcode = New System.Windows.Forms.TextBox()
        Me.BtnGenBarcode = New System.Windows.Forms.Button()
        Me.LblHargaBeli = New System.Windows.Forms.Label()
        Me.TxtHargaBeli = New System.Windows.Forms.TextBox()
        Me.LblHargaJual = New System.Windows.Forms.Label()
        Me.TxtHargaJual = New System.Windows.Forms.TextBox()
        Me.LblSatuan = New System.Windows.Forms.Label()
        Me.CmbSatuan = New System.Windows.Forms.ComboBox()
        Me.DgvKomponen = New System.Windows.Forms.DataGridView()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.PanelFooter = New System.Windows.Forms.Panel()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.BtnBatal = New System.Windows.Forms.Button()
        Me.PnlBatas2 = New System.Windows.Forms.Panel()
        Me.PnlBatas1 = New System.Windows.Forms.Panel()
        Me.PnlBatas3 = New System.Windows.Forms.Panel()
        Me.ColHapus = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.ColId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColNama = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColSatuan = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.ColIsi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColHargaBeli = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColTotalHargaBeli = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColStokToko = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColStokGudang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColStok = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PanelHeader.SuspendLayout()
        Me.PanelInput.SuspendLayout()
        CType(Me.DgvKomponen, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFooter.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(3, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(750, 40)
        Me.PanelHeader.TabIndex = 102
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.GreenYellow
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 14.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle))
        Me.LblHeader.ForeColor = System.Drawing.Color.Black
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(750, 40)
        Me.LblHeader.TabIndex = 0
        Me.LblHeader.Text = "TAMBAH PAKET RAKITAN BARU"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelInput
        '
        Me.PanelInput.Controls.Add(Me.LblKode)
        Me.PanelInput.Controls.Add(Me.TxtKode)
        Me.PanelInput.Controls.Add(Me.LblNama)
        Me.PanelInput.Controls.Add(Me.TxtNama)
        Me.PanelInput.Controls.Add(Me.LblBarcode)
        Me.PanelInput.Controls.Add(Me.TxtBarcode)
        Me.PanelInput.Controls.Add(Me.BtnGenBarcode)
        Me.PanelInput.Controls.Add(Me.LblHargaBeli)
        Me.PanelInput.Controls.Add(Me.TxtHargaBeli)
        Me.PanelInput.Controls.Add(Me.LblHargaJual)
        Me.PanelInput.Controls.Add(Me.TxtHargaJual)
        Me.PanelInput.Controls.Add(Me.LblSatuan)
        Me.PanelInput.Controls.Add(Me.CmbSatuan)
        Me.PanelInput.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelInput.Location = New System.Drawing.Point(3, 40)
        Me.PanelInput.Name = "PanelInput"
        Me.PanelInput.Padding = New System.Windows.Forms.Padding(8)
        Me.PanelInput.Size = New System.Drawing.Size(750, 170)
        Me.PanelInput.TabIndex = 101
        '
        'LblKode
        '
        Me.LblKode.AutoSize = True
        Me.LblKode.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblKode.Location = New System.Drawing.Point(82, 14)
        Me.LblKode.Name = "LblKode"
        Me.LblKode.Size = New System.Drawing.Size(42, 17)
        Me.LblKode.TabIndex = 0
        Me.LblKode.Text = "Kode:"
        '
        'TxtKode
        '
        Me.TxtKode.BackColor = System.Drawing.SystemColors.Control
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.TxtKode.Location = New System.Drawing.Point(130, 11)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.ReadOnly = True
        Me.TxtKode.Size = New System.Drawing.Size(200, 22)
        Me.TxtKode.TabIndex = 1
        '
        'LblNama
        '
        Me.LblNama.AutoSize = True
        Me.LblNama.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblNama.Location = New System.Drawing.Point(77, 40)
        Me.LblNama.Name = "LblNama"
        Me.LblNama.Size = New System.Drawing.Size(47, 17)
        Me.LblNama.TabIndex = 2
        Me.LblNama.Text = "Nama:"
        '
        'TxtNama
        '
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.TxtNama.Location = New System.Drawing.Point(130, 37)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(606, 22)
        Me.TxtNama.TabIndex = 3
        '
        'LblBarcode
        '
        Me.LblBarcode.AutoSize = True
        Me.LblBarcode.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblBarcode.Location = New System.Drawing.Point(412, 14)
        Me.LblBarcode.Name = "LblBarcode"
        Me.LblBarcode.Size = New System.Drawing.Size(62, 17)
        Me.LblBarcode.TabIndex = 4
        Me.LblBarcode.Text = "Barcode:"
        '
        'TxtBarcode
        '
        Me.TxtBarcode.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.TxtBarcode.Location = New System.Drawing.Point(480, 11)
        Me.TxtBarcode.Name = "TxtBarcode"
        Me.TxtBarcode.Size = New System.Drawing.Size(200, 22)
        Me.TxtBarcode.TabIndex = 5
        '
        'BtnGenBarcode
        '
        Me.BtnGenBarcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnGenBarcode.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.BtnGenBarcode.Location = New System.Drawing.Point(686, 9)
        Me.BtnGenBarcode.Name = "BtnGenBarcode"
        Me.BtnGenBarcode.Size = New System.Drawing.Size(50, 26)
        Me.BtnGenBarcode.TabIndex = 6
        Me.BtnGenBarcode.Text = "Auto"
        '
        'LblHargaBeli
        '
        Me.LblHargaBeli.AutoSize = True
        Me.LblHargaBeli.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblHargaBeli.Location = New System.Drawing.Point(19, 100)
        Me.LblHargaBeli.Name = "LblHargaBeli"
        Me.LblHargaBeli.Size = New System.Drawing.Size(105, 17)
        Me.LblHargaBeli.TabIndex = 7
        Me.LblHargaBeli.Text = "Harga Beli (HPP):"
        '
        'TxtHargaBeli
        '
        Me.TxtHargaBeli.BackColor = System.Drawing.SystemColors.Control
        Me.TxtHargaBeli.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.TxtHargaBeli.Location = New System.Drawing.Point(130, 97)
        Me.TxtHargaBeli.Name = "TxtHargaBeli"
        Me.TxtHargaBeli.ReadOnly = True
        Me.TxtHargaBeli.Size = New System.Drawing.Size(150, 22)
        Me.TxtHargaBeli.TabIndex = 8
        Me.TxtHargaBeli.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblHargaJual
        '
        Me.LblHargaJual.AutoSize = True
        Me.LblHargaJual.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblHargaJual.Location = New System.Drawing.Point(50, 128)
        Me.LblHargaJual.Name = "LblHargaJual"
        Me.LblHargaJual.Size = New System.Drawing.Size(74, 17)
        Me.LblHargaJual.TabIndex = 9
        Me.LblHargaJual.Text = "Harga Jual:"
        '
        'TxtHargaJual
        '
        Me.TxtHargaJual.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.TxtHargaJual.Location = New System.Drawing.Point(130, 125)
        Me.TxtHargaJual.Name = "TxtHargaJual"
        Me.TxtHargaJual.Size = New System.Drawing.Size(150, 22)
        Me.TxtHargaJual.TabIndex = 10
        Me.TxtHargaJual.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblSatuan
        '
        Me.LblSatuan.AutoSize = True
        Me.LblSatuan.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblSatuan.Location = New System.Drawing.Point(71, 68)
        Me.LblSatuan.Name = "LblSatuan"
        Me.LblSatuan.Size = New System.Drawing.Size(53, 17)
        Me.LblSatuan.TabIndex = 11
        Me.LblSatuan.Text = "Satuan:"
        '
        'CmbSatuan
        '
        Me.CmbSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSatuan.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.CmbSatuan.Location = New System.Drawing.Point(130, 64)
        Me.CmbSatuan.Name = "CmbSatuan"
        Me.CmbSatuan.Size = New System.Drawing.Size(120, 25)
        Me.CmbSatuan.TabIndex = 12
        '
        'DgvKomponen
        '
        Me.DgvKomponen.AllowUserToDeleteRows = False
        Me.DgvKomponen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvKomponen.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColHapus, Me.ColId, Me.ColNama, Me.ColQty, Me.ColSatuan, Me.ColIsi, Me.ColHargaBeli, Me.ColTotalHargaBeli, Me.ColStokToko, Me.ColStokGudang, Me.ColStok})
        Me.DgvKomponen.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvKomponen.EnableHeadersVisualStyles = False
        Me.DgvKomponen.Location = New System.Drawing.Point(3, 210)
        Me.DgvKomponen.Name = "DgvKomponen"
        Me.DgvKomponen.RowHeadersVisible = False
        Me.DgvKomponen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvKomponen.Size = New System.Drawing.Size(750, 355)
        Me.DgvKomponen.TabIndex = 0
        '
        'LstBarang
        '
        Me.LstBarang.Font = New System.Drawing.Font("Consolas", 10.0!)
        Me.LstBarang.FormattingEnabled = True
        Me.LstBarang.IntegralHeight = False
        Me.LstBarang.ItemHeight = 15
        Me.LstBarang.Location = New System.Drawing.Point(324, 68)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(420, 232)
        Me.LstBarang.TabIndex = 100
        Me.LstBarang.Visible = False
        '
        'PanelFooter
        '
        Me.PanelFooter.Controls.Add(Me.BtnSimpan)
        Me.PanelFooter.Controls.Add(Me.BtnBatal)
        Me.PanelFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelFooter.Location = New System.Drawing.Point(3, 565)
        Me.PanelFooter.Name = "PanelFooter"
        Me.PanelFooter.Padding = New System.Windows.Forms.Padding(6, 6, 6, 4)
        Me.PanelFooter.Size = New System.Drawing.Size(750, 44)
        Me.PanelFooter.TabIndex = 103
        '
        'BtnSimpan
        '
        Me.BtnSimpan.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpan.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.BtnSimpan.ForeColor = System.Drawing.Color.White
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.Location = New System.Drawing.Point(6, 6)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(138, 32)
        Me.BtnSimpan.TabIndex = 0
        Me.BtnSimpan.Text = "Simpan (F2)"
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'BtnBatal
        '
        Me.BtnBatal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBatal.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.BtnBatal.Image = CType(resources.GetObject("BtnBatal.Image"), System.Drawing.Image)
        Me.BtnBatal.Location = New System.Drawing.Point(615, 9)
        Me.BtnBatal.Name = "BtnBatal"
        Me.BtnBatal.Size = New System.Drawing.Size(121, 32)
        Me.BtnBatal.TabIndex = 1
        Me.BtnBatal.Text = "Batal (Esc)"
        Me.BtnBatal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        'PnlBatas2
        '
        Me.PnlBatas2.BackColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.PnlBatas2.Dock = System.Windows.Forms.DockStyle.Left
        Me.PnlBatas2.Location = New System.Drawing.Point(0, 0)
        Me.PnlBatas2.Name = "PnlBatas2"
        Me.PnlBatas2.Size = New System.Drawing.Size(3, 612)
        Me.PnlBatas2.TabIndex = 110
        '
        'PnlBatas1
        '
        Me.PnlBatas1.BackColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.PnlBatas1.Dock = System.Windows.Forms.DockStyle.Right
        Me.PnlBatas1.Location = New System.Drawing.Point(753, 0)
        Me.PnlBatas1.Name = "PnlBatas1"
        Me.PnlBatas1.Size = New System.Drawing.Size(3, 612)
        Me.PnlBatas1.TabIndex = 111
        '
        'PnlBatas3
        '
        Me.PnlBatas3.BackColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.PnlBatas3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PnlBatas3.Location = New System.Drawing.Point(3, 609)
        Me.PnlBatas3.Name = "PnlBatas3"
        Me.PnlBatas3.Size = New System.Drawing.Size(750, 3)
        Me.PnlBatas3.TabIndex = 112
        '
        'ColHapus
        '
        Me.ColHapus.FillWeight = 30.0!
        Me.ColHapus.HeaderText = ""
        Me.ColHapus.Name = "ColHapus"
        Me.ColHapus.Text = "X"
        Me.ColHapus.UseColumnTextForButtonValue = True
        '
        'ColId
        '
        Me.ColId.HeaderText = "Id"
        Me.ColId.Name = "ColId"
        Me.ColId.Visible = False
        '
        'ColNama
        '
        Me.ColNama.FillWeight = 200.0!
        Me.ColNama.HeaderText = "Nama Komponen"
        Me.ColNama.Name = "ColNama"
        '
        'ColQty
        '
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle8.Format = "#,0.####"
        DataGridViewCellStyle8.NullValue = Nothing
        Me.ColQty.DefaultCellStyle = DataGridViewCellStyle8
        Me.ColQty.FillWeight = 40.0!
        Me.ColQty.HeaderText = "Qty"
        Me.ColQty.Name = "ColQty"
        '
        'ColSatuan
        '
        Me.ColSatuan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ColSatuan.HeaderText = "Satuan"
        Me.ColSatuan.Name = "ColSatuan"
        '
        'ColIsi
        '
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle9.NullValue = Nothing
        Me.ColIsi.DefaultCellStyle = DataGridViewCellStyle9
        Me.ColIsi.HeaderText = "Isi"
        Me.ColIsi.Name = "ColIsi"
        Me.ColIsi.ReadOnly = True
        Me.ColIsi.Visible = False
        '
        'ColHargaBeli
        '
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle10.Format = "#,0.####"
        DataGridViewCellStyle10.NullValue = Nothing
        Me.ColHargaBeli.DefaultCellStyle = DataGridViewCellStyle10
        Me.ColHargaBeli.HeaderText = "Harga Beli"
        Me.ColHargaBeli.Name = "ColHargaBeli"
        Me.ColHargaBeli.Visible = False
        '
        'ColTotalHargaBeli
        '
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle11.Format = "#,0.####"
        DataGridViewCellStyle11.NullValue = Nothing
        Me.ColTotalHargaBeli.DefaultCellStyle = DataGridViewCellStyle11
        Me.ColTotalHargaBeli.HeaderText = "Total Harga Beli"
        Me.ColTotalHargaBeli.Name = "ColTotalHargaBeli"
        Me.ColTotalHargaBeli.ReadOnly = True
        '
        'ColStokToko
        '
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle12.BackColor = System.Drawing.Color.LightBlue
        DataGridViewCellStyle12.Format = "#,0.####"
        Me.ColStokToko.DefaultCellStyle = DataGridViewCellStyle12
        Me.ColStokToko.FillWeight = 50.0!
        Me.ColStokToko.HeaderText = "S Toko"
        Me.ColStokToko.Name = "ColStokToko"
        Me.ColStokToko.ReadOnly = True
        '
        'ColStokGudang
        '
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle13.BackColor = System.Drawing.Color.LightBlue
        DataGridViewCellStyle13.Format = "#,0.####"
        Me.ColStokGudang.DefaultCellStyle = DataGridViewCellStyle13
        Me.ColStokGudang.FillWeight = 50.0!
        Me.ColStokGudang.HeaderText = "S Gudang"
        Me.ColStokGudang.Name = "ColStokGudang"
        Me.ColStokGudang.ReadOnly = True
        '
        'ColStok
        '
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle14.Format = "#,0.####"
        DataGridViewCellStyle14.NullValue = Nothing
        Me.ColStok.DefaultCellStyle = DataGridViewCellStyle14
        Me.ColStok.HeaderText = "Stok"
        Me.ColStok.Name = "ColStok"
        Me.ColStok.ReadOnly = True
        Me.ColStok.Visible = False
        '
        'FormTambahEditRakitan
        '
        Me.ClientSize = New System.Drawing.Size(756, 612)
        Me.Controls.Add(Me.DgvKomponen)
        Me.Controls.Add(Me.PanelInput)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.PanelFooter)
        Me.Controls.Add(Me.PnlBatas3)
        Me.Controls.Add(Me.PnlBatas1)
        Me.Controls.Add(Me.PnlBatas2)
        Me.Controls.Add(Me.LstBarang)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormTambahEditRakitan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Tambah / Edit Paket Rakitan"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelInput.ResumeLayout(False)
        Me.PanelInput.PerformLayout()
        CType(Me.DgvKomponen, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFooter.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader   As Panel
    Friend WithEvents LblHeader     As Label
    Friend WithEvents PanelInput     As Panel
    Friend WithEvents LblKode       As Label
    Friend WithEvents TxtKode       As TextBox
    Friend WithEvents LblNama       As Label
    Friend WithEvents TxtNama       As TextBox
    Friend WithEvents LblBarcode    As Label
    Friend WithEvents TxtBarcode    As TextBox
    Friend WithEvents BtnGenBarcode As Button
    Friend WithEvents LblHargaBeli  As Label
    Friend WithEvents TxtHargaBeli  As TextBox
    Friend WithEvents LblHargaJual  As Label
    Friend WithEvents TxtHargaJual  As TextBox
    Friend WithEvents LblSatuan     As Label
    Friend WithEvents CmbSatuan     As ComboBox
    Friend WithEvents DgvKomponen   As DataGridView
    Friend WithEvents LstBarang As ListBox
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents BtnSimpan As Button
    Friend WithEvents BtnBatal As Button
    Friend WithEvents PnlBatas2 As Panel
    Friend WithEvents PnlBatas1 As Panel
    Friend WithEvents PnlBatas3 As Panel
    Friend WithEvents ColHapus As DataGridViewButtonColumn
    Friend WithEvents ColId As DataGridViewTextBoxColumn
    Friend WithEvents ColNama As DataGridViewTextBoxColumn
    Friend WithEvents ColQty As DataGridViewTextBoxColumn
    Friend WithEvents ColSatuan As DataGridViewComboBoxColumn
    Friend WithEvents ColIsi As DataGridViewTextBoxColumn
    Friend WithEvents ColHargaBeli As DataGridViewTextBoxColumn
    Friend WithEvents ColTotalHargaBeli As DataGridViewTextBoxColumn
    Friend WithEvents ColStokToko As DataGridViewTextBoxColumn
    Friend WithEvents ColStokGudang As DataGridViewTextBoxColumn
    Friend WithEvents ColStok As DataGridViewTextBoxColumn
End Class
