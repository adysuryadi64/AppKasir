<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormStokOpname
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormStokOpname))
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.BtNCetak = New System.Windows.Forms.Button()
        Me.BtnKeluarForm = New System.Windows.Forms.Button()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.lstBarang = New System.Windows.Forms.ListBox()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.TxtKategori = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.TxtTotalRupiah = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.LblSat = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.TxtSelisih = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TxtKeteranganToko = New System.Windows.Forms.TextBox()
        Me.TxtStokSystem = New System.Windows.Forms.TextBox()
        Me.TxtNyata = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblSatIsi = New System.Windows.Forms.Label()
        Me.TxtSelisihRp = New System.Windows.Forms.TextBox()
        Me.TxtnamaHasil = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtQty = New System.Windows.Forms.TextBox()
        Me.TxtSelisihQty = New System.Windows.Forms.TextBox()
        Me.TxtHarga = New System.Windows.Forms.TextBox()
        Me.TxtIdUser = New System.Windows.Forms.TextBox()
        Me.TxtKomputer = New System.Windows.Forms.TextBox()
        Me.TxtBarcode = New System.Windows.Forms.TextBox()
        Me.TxtLokasi = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.LblToko = New System.Windows.Forms.Label()
        Me.LblKetTerakhir = New System.Windows.Forms.Label()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.DGVData = New System.Windows.Forms.DataGridView()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtQtyUntukEdit = New System.Windows.Forms.TextBox()
        Me.Panel4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.DGVData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.Yellow
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel4.Controls.Add(Me.BtnKeluarForm)
        Me.Panel4.Controls.Add(Me.LblUtama)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1112, 45)
        Me.Panel4.TabIndex = 5
        '
        'BtNCetak
        '
        Me.BtNCetak.AutoSize = True
        Me.BtNCetak.BackColor = System.Drawing.Color.White
        Me.BtNCetak.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtNCetak.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtNCetak.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtNCetak.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtNCetak.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtNCetak.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtNCetak.ForeColor = System.Drawing.Color.Black
        Me.BtNCetak.Image = CType(resources.GetObject("BtNCetak.Image"), System.Drawing.Image)
        Me.BtNCetak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtNCetak.Location = New System.Drawing.Point(382, 88)
        Me.BtNCetak.Name = "BtNCetak"
        Me.BtNCetak.Size = New System.Drawing.Size(196, 36)
        Me.BtNCetak.TabIndex = 79
        Me.BtNCetak.Text = "Cetak bahan stokopname"
        Me.BtNCetak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtNCetak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtNCetak.UseVisualStyleBackColor = False
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
        Me.BtnKeluarForm.Location = New System.Drawing.Point(989, 4)
        Me.BtnKeluarForm.Name = "BtnKeluarForm"
        Me.BtnKeluarForm.Size = New System.Drawing.Size(112, 31)
        Me.BtnKeluarForm.TabIndex = 78
        Me.BtnKeluarForm.Text = "Keluar (Esc)"
        Me.BtnKeluarForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarForm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluarForm.UseVisualStyleBackColor = False
        '
        'LblUtama
        '
        Me.LblUtama.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold)
        Me.LblUtama.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(1108, 41)
        Me.LblUtama.TabIndex = 1
        Me.LblUtama.Text = "Add Stok Opname"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lstBarang
        '
        Me.lstBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstBarang.FormattingEnabled = True
        Me.lstBarang.ItemHeight = 17
        Me.lstBarang.Location = New System.Drawing.Point(18, 160)
        Me.lstBarang.Name = "lstBarang"
        Me.lstBarang.Size = New System.Drawing.Size(532, 327)
        Me.lstBarang.TabIndex = 2
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.ForeColor = System.Drawing.Color.Black
        Me.TxtFaktur.Location = New System.Drawing.Point(85, 49)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.ReadOnly = True
        Me.TxtFaktur.Size = New System.Drawing.Size(165, 23)
        Me.TxtFaktur.TabIndex = 130
        '
        'TxtNama
        '
        Me.TxtNama.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNama.Location = New System.Drawing.Point(6, 7)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(532, 23)
        Me.TxtNama.TabIndex = 1
        Me.TxtNama.Text = "Nama"
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(85, 74)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(165, 23)
        Me.DTPTgl.TabIndex = 131
        '
        'TxtKategori
        '
        Me.TxtKategori.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKategori.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKategori.Location = New System.Drawing.Point(240, 10)
        Me.TxtKategori.Name = "TxtKategori"
        Me.TxtKategori.ReadOnly = True
        Me.TxtKategori.Size = New System.Drawing.Size(203, 23)
        Me.TxtKategori.TabIndex = 133
        Me.TxtKategori.Text = "Kategori"
        Me.TxtKategori.Visible = False
        '
        'TxtKode
        '
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(69, 9)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.ReadOnly = True
        Me.TxtKode.Size = New System.Drawing.Size(165, 23)
        Me.TxtKode.TabIndex = 132
        Me.TxtKode.Text = "kode"
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
        Me.BtnSimpan.Location = New System.Drawing.Point(437, 497)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(138, 34)
        Me.BtnSimpan.TabIndex = 139
        Me.BtnSimpan.Text = "Simpan (F8)"
        Me.BtnSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(315, 9)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(93, 17)
        Me.Label20.TabIndex = 150
        Me.Label20.Text = "Selisih Rupiah"
        '
        'TxtTotalRupiah
        '
        Me.TxtTotalRupiah.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.TxtTotalRupiah.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalRupiah.Enabled = False
        Me.TxtTotalRupiah.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold)
        Me.TxtTotalRupiah.Location = New System.Drawing.Point(315, 33)
        Me.TxtTotalRupiah.Name = "TxtTotalRupiah"
        Me.TxtTotalRupiah.ReadOnly = True
        Me.TxtTotalRupiah.Size = New System.Drawing.Size(217, 33)
        Me.TxtTotalRupiah.TabIndex = 149
        Me.TxtTotalRupiah.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(104, 9)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(78, 17)
        Me.Label17.TabIndex = 148
        Me.Label17.Text = "Stok Nyata"
        '
        'LblSat
        '
        Me.LblSat.AutoSize = True
        Me.LblSat.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSat.Location = New System.Drawing.Point(272, 47)
        Me.LblSat.Name = "LblSat"
        Me.LblSat.Size = New System.Drawing.Size(28, 17)
        Me.LblSat.TabIndex = 144
        Me.LblSat.Text = "Sat"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(194, 9)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(75, 17)
        Me.Label23.TabIndex = 142
        Me.Label23.Text = "Stok Selisih"
        '
        'TxtSelisih
        '
        Me.TxtSelisih.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.TxtSelisih.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSelisih.Enabled = False
        Me.TxtSelisih.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSelisih.Location = New System.Drawing.Point(194, 34)
        Me.TxtSelisih.Name = "TxtSelisih"
        Me.TxtSelisih.ReadOnly = True
        Me.TxtSelisih.Size = New System.Drawing.Size(72, 33)
        Me.TxtSelisih.TabIndex = 141
        Me.TxtSelisih.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(14, 9)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(82, 17)
        Me.Label10.TabIndex = 138
        Me.Label10.Text = "Stok System"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(11, 78)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(37, 17)
        Me.Label11.TabIndex = 137
        Me.Label11.Text = "Ket :"
        '
        'TxtKeteranganToko
        '
        Me.TxtKeteranganToko.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKeteranganToko.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKeteranganToko.Location = New System.Drawing.Point(52, 75)
        Me.TxtKeteranganToko.Name = "TxtKeteranganToko"
        Me.TxtKeteranganToko.Size = New System.Drawing.Size(480, 23)
        Me.TxtKeteranganToko.TabIndex = 4
        '
        'TxtStokSystem
        '
        Me.TxtStokSystem.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.TxtStokSystem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStokSystem.Enabled = False
        Me.TxtStokSystem.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStokSystem.Location = New System.Drawing.Point(14, 34)
        Me.TxtStokSystem.Name = "TxtStokSystem"
        Me.TxtStokSystem.ReadOnly = True
        Me.TxtStokSystem.Size = New System.Drawing.Size(79, 33)
        Me.TxtStokSystem.TabIndex = 135
        Me.TxtStokSystem.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtNyata
        '
        Me.TxtNyata.BackColor = System.Drawing.Color.White
        Me.TxtNyata.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNyata.Font = New System.Drawing.Font("Century Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNyata.Location = New System.Drawing.Point(104, 34)
        Me.TxtNyata.Name = "TxtNyata"
        Me.TxtNyata.Size = New System.Drawing.Size(75, 33)
        Me.TxtNyata.TabIndex = 3
        Me.TxtNyata.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.Label20)
        Me.Panel1.Controls.Add(Me.TxtNyata)
        Me.Panel1.Controls.Add(Me.TxtTotalRupiah)
        Me.Panel1.Controls.Add(Me.Label17)
        Me.Panel1.Controls.Add(Me.LblSatIsi)
        Me.Panel1.Controls.Add(Me.LblSat)
        Me.Panel1.Controls.Add(Me.TxtStokSystem)
        Me.Panel1.Controls.Add(Me.Label23)
        Me.Panel1.Controls.Add(Me.TxtKeteranganToko)
        Me.Panel1.Controls.Add(Me.TxtSelisih)
        Me.Panel1.Controls.Add(Me.Label11)
        Me.Panel1.Controls.Add(Me.Label10)
        Me.Panel1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Panel1.Location = New System.Drawing.Point(18, 299)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(560, 111)
        Me.Panel1.TabIndex = 145
        '
        'LblSatIsi
        '
        Me.LblSatIsi.AutoSize = True
        Me.LblSatIsi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSatIsi.Location = New System.Drawing.Point(272, 26)
        Me.LblSatIsi.Name = "LblSatIsi"
        Me.LblSatIsi.Size = New System.Drawing.Size(28, 17)
        Me.LblSatIsi.TabIndex = 144
        Me.LblSatIsi.Text = "Sat"
        '
        'TxtSelisihRp
        '
        Me.TxtSelisihRp.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.TxtSelisihRp.Enabled = False
        Me.TxtSelisihRp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSelisihRp.Location = New System.Drawing.Point(336, 496)
        Me.TxtSelisihRp.Name = "TxtSelisihRp"
        Me.TxtSelisihRp.ReadOnly = True
        Me.TxtSelisihRp.Size = New System.Drawing.Size(81, 23)
        Me.TxtSelisihRp.TabIndex = 149
        Me.TxtSelisihRp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtSelisihRp.Visible = False
        '
        'TxtnamaHasil
        '
        Me.TxtnamaHasil.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtnamaHasil.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtnamaHasil.Location = New System.Drawing.Point(69, 36)
        Me.TxtnamaHasil.Name = "TxtnamaHasil"
        Me.TxtnamaHasil.ReadOnly = True
        Me.TxtnamaHasil.Size = New System.Drawing.Size(485, 23)
        Me.TxtnamaHasil.TabIndex = 132
        Me.TxtnamaHasil.Text = "Nama"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(17, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 17)
        Me.Label1.TabIndex = 150
        Me.Label1.Text = "Kode :"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(10, 39)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 17)
        Me.Label2.TabIndex = 150
        Me.Label2.Text = "Nama :"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(11, 65)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 17)
        Me.Label3.TabIndex = 150
        Me.Label3.Text = "Harga :"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(15, 108)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(310, 17)
        Me.Label4.TabIndex = 150
        Me.Label4.Text = "Cari barang berdasrakan nama atau barcode"
        '
        'TxtQty
        '
        Me.TxtQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtQty.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQty.Location = New System.Drawing.Point(450, 10)
        Me.TxtQty.Name = "TxtQty"
        Me.TxtQty.ReadOnly = True
        Me.TxtQty.Size = New System.Drawing.Size(42, 23)
        Me.TxtQty.TabIndex = 133
        Me.TxtQty.Text = "Qty"
        Me.TxtQty.Visible = False
        '
        'TxtSelisihQty
        '
        Me.TxtSelisihQty.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.TxtSelisihQty.Enabled = False
        Me.TxtSelisihQty.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSelisihQty.Location = New System.Drawing.Point(229, 496)
        Me.TxtSelisihQty.Name = "TxtSelisihQty"
        Me.TxtSelisihQty.ReadOnly = True
        Me.TxtSelisihQty.Size = New System.Drawing.Size(101, 23)
        Me.TxtSelisihQty.TabIndex = 149
        Me.TxtSelisihQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtSelisihQty.Visible = False
        '
        'TxtHarga
        '
        Me.TxtHarga.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHarga.Location = New System.Drawing.Point(69, 62)
        Me.TxtHarga.Name = "TxtHarga"
        Me.TxtHarga.ReadOnly = True
        Me.TxtHarga.Size = New System.Drawing.Size(165, 23)
        Me.TxtHarga.TabIndex = 132
        Me.TxtHarga.Text = "Harga"
        '
        'TxtIdUser
        '
        Me.TxtIdUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdUser.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdUser.Location = New System.Drawing.Point(25, 496)
        Me.TxtIdUser.Name = "TxtIdUser"
        Me.TxtIdUser.ReadOnly = True
        Me.TxtIdUser.Size = New System.Drawing.Size(89, 23)
        Me.TxtIdUser.TabIndex = 133
        Me.TxtIdUser.Text = "User"
        Me.TxtIdUser.Visible = False
        '
        'TxtKomputer
        '
        Me.TxtKomputer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKomputer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKomputer.Location = New System.Drawing.Point(120, 497)
        Me.TxtKomputer.Name = "TxtKomputer"
        Me.TxtKomputer.ReadOnly = True
        Me.TxtKomputer.Size = New System.Drawing.Size(89, 23)
        Me.TxtKomputer.TabIndex = 133
        Me.TxtKomputer.Text = "TxtKomputer"
        Me.TxtKomputer.Visible = False
        '
        'TxtBarcode
        '
        Me.TxtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBarcode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBarcode.ForeColor = System.Drawing.Color.Black
        Me.TxtBarcode.Location = New System.Drawing.Point(249, 62)
        Me.TxtBarcode.Name = "TxtBarcode"
        Me.TxtBarcode.ReadOnly = True
        Me.TxtBarcode.Size = New System.Drawing.Size(123, 23)
        Me.TxtBarcode.TabIndex = 151
        Me.TxtBarcode.Visible = False
        '
        'TxtLokasi
        '
        Me.TxtLokasi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLokasi.ForeColor = System.Drawing.Color.Black
        Me.TxtLokasi.Location = New System.Drawing.Point(374, 49)
        Me.TxtLokasi.Name = "TxtLokasi"
        Me.TxtLokasi.ReadOnly = True
        Me.TxtLokasi.Size = New System.Drawing.Size(123, 23)
        Me.TxtLokasi.TabIndex = 152
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.LemonChiffon
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.LblToko)
        Me.Panel2.Controls.Add(Me.LblKetTerakhir)
        Me.Panel2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Panel2.Location = New System.Drawing.Point(18, 413)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(560, 77)
        Me.Panel2.TabIndex = 153
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(12, 7)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(207, 17)
        Me.Label9.TabIndex = 133
        Me.Label9.Text = "Recent stok opname terakhir  :"
        '
        'LblToko
        '
        Me.LblToko.AutoSize = True
        Me.LblToko.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblToko.Location = New System.Drawing.Point(12, 52)
        Me.LblToko.Name = "LblToko"
        Me.LblToko.Size = New System.Drawing.Size(46, 17)
        Me.LblToko.TabIndex = 132
        Me.LblToko.Text = "Toko :"
        '
        'LblKetTerakhir
        '
        Me.LblKetTerakhir.AutoSize = True
        Me.LblKetTerakhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKetTerakhir.Location = New System.Drawing.Point(12, 30)
        Me.LblKetTerakhir.Name = "LblKetTerakhir"
        Me.LblKetTerakhir.Size = New System.Drawing.Size(288, 17)
        Me.LblKetTerakhir.TabIndex = 129
        Me.LblKetTerakhir.Text = "Stok Opnam terakhir pada nama barang : "
        '
        'PanelCariNama
        '
        Me.PanelCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCari.Controls.Add(Me.BtnCari)
        Me.PanelCari.Controls.Add(Me.TxtNama)
        Me.PanelCari.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelCari.Location = New System.Drawing.Point(12, 130)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(566, 36)
        Me.PanelCari.TabIndex = 154
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
        Me.BtnCari.Location = New System.Drawing.Point(538, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(60, 29)
        Me.BtnCari.TabIndex = 2
        Me.BtnCari.Text = "Cari"
        Me.BtnCari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCari.UseVisualStyleBackColor = False
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.LemonChiffon
        Me.Panel3.Controls.Add(Me.TxtKode)
        Me.Panel3.Controls.Add(Me.TxtKategori)
        Me.Panel3.Controls.Add(Me.TxtQty)
        Me.Panel3.Controls.Add(Me.TxtHarga)
        Me.Panel3.Controls.Add(Me.TxtnamaHasil)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.Label3)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.TxtBarcode)
        Me.Panel3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Panel3.Location = New System.Drawing.Point(18, 204)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(560, 92)
        Me.Panel3.TabIndex = 155
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(23, 52)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(61, 17)
        Me.Label5.TabIndex = 156
        Me.Label5.Text = "Nomor :"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(16, 77)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(68, 17)
        Me.Label6.TabIndex = 157
        Me.Label6.Text = "Tanggal :"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(317, 52)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(55, 17)
        Me.Label7.TabIndex = 158
        Me.Label7.Text = "Lokasi :"
        '
        'DGVData
        '
        Me.DGVData.AllowUserToAddRows = False
        Me.DGVData.AllowUserToDeleteRows = False
        Me.DGVData.AllowUserToResizeRows = False
        Me.DGVData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.DGVData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVData.Location = New System.Drawing.Point(596, 80)
        Me.DGVData.Name = "DGVData"
        Me.DGVData.RowHeadersVisible = False
        Me.DGVData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVData.Size = New System.Drawing.Size(504, 454)
        Me.DGVData.TabIndex = 159
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(599, 57)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(139, 17)
        Me.Label8.TabIndex = 160
        Me.Label8.Text = "Daftar stok opname"
        '
        'TxtQtyUntukEdit
        '
        Me.TxtQtyUntukEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtQtyUntukEdit.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQtyUntukEdit.Location = New System.Drawing.Point(25, 520)
        Me.TxtQtyUntukEdit.Name = "TxtQtyUntukEdit"
        Me.TxtQtyUntukEdit.ReadOnly = True
        Me.TxtQtyUntukEdit.Size = New System.Drawing.Size(89, 23)
        Me.TxtQtyUntukEdit.TabIndex = 161
        Me.TxtQtyUntukEdit.Text = "QtyEdit"
        Me.TxtQtyUntukEdit.Visible = False
        '
        'FormStokOpname
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.ClientSize = New System.Drawing.Size(1112, 546)
        Me.Controls.Add(Me.BtNCetak)
        Me.Controls.Add(Me.TxtQtyUntukEdit)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.DGVData)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.lstBarang)
        Me.Controls.Add(Me.PanelCari)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.TxtLokasi)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TxtSelisihQty)
        Me.Controls.Add(Me.TxtSelisihRp)
        Me.Controls.Add(Me.TxtFaktur)
        Me.Controls.Add(Me.DTPTgl)
        Me.Controls.Add(Me.TxtKomputer)
        Me.Controls.Add(Me.TxtIdUser)
        Me.Controls.Add(Me.BtnSimpan)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormStokOpname"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "BarangStokOpnameForm"
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.DGVData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents LblUtama As System.Windows.Forms.Label
    Friend WithEvents BtnKeluarForm As System.Windows.Forms.Button
    Friend WithEvents lstBarang As System.Windows.Forms.ListBox
    Friend WithEvents TxtFaktur As System.Windows.Forms.TextBox
    Friend WithEvents TxtNama As System.Windows.Forms.TextBox
    Friend WithEvents DTPTgl As System.Windows.Forms.DateTimePicker
    Friend WithEvents TxtKategori As System.Windows.Forms.TextBox
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalRupiah As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents LblSat As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TxtSelisih As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents TxtKeteranganToko As System.Windows.Forms.TextBox
    Friend WithEvents TxtStokSystem As System.Windows.Forms.TextBox
    Friend WithEvents TxtNyata As System.Windows.Forms.TextBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents LblSatIsi As System.Windows.Forms.Label
    Friend WithEvents TxtSelisihRp As System.Windows.Forms.TextBox
    Friend WithEvents TxtnamaHasil As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtQty As System.Windows.Forms.TextBox
    Friend WithEvents TxtSelisihQty As System.Windows.Forms.TextBox
    Friend WithEvents TxtHarga As System.Windows.Forms.TextBox
    Friend WithEvents TxtIdUser As System.Windows.Forms.TextBox
    Friend WithEvents TxtKomputer As System.Windows.Forms.TextBox
    Friend WithEvents TxtBarcode As System.Windows.Forms.TextBox
    Friend WithEvents TxtLokasi As System.Windows.Forms.TextBox
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents LblToko As System.Windows.Forms.Label
    Friend WithEvents LblKetTerakhir As System.Windows.Forms.Label
    Friend WithEvents PanelCari As System.Windows.Forms.Panel
    Friend WithEvents BtnCari As System.Windows.Forms.Button
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents DGVData As System.Windows.Forms.DataGridView
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents BtNCetak As System.Windows.Forms.Button
    Friend WithEvents TxtQtyUntukEdit As System.Windows.Forms.TextBox
End Class
