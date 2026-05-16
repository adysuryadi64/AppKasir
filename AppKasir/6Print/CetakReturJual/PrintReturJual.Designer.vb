<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintReturJual
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintReturJual))
        Me.BtnCetak = New System.Windows.Forms.Button()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TxtStatusTrans = New System.Windows.Forms.TextBox()
        Me.TxtBAntuanbayar = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LblJatuhTempo = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.DTPJatuhTempo = New System.Windows.Forms.DateTimePicker()
        Me.TxtBayar = New System.Windows.Forms.TextBox()
        Me.TxtKembali = New System.Windows.Forms.TextBox()
        Me.TxtJmlhBrg = New System.Windows.Forms.TextBox()
        Me.LblPembayaran = New System.Windows.Forms.Label()
        Me.TxtTotal = New System.Windows.Forms.TextBox()
        Me.CmbPelanggan = New System.Windows.Forms.ComboBox()
        Me.TxtDiskonRp = New System.Windows.Forms.TextBox()
        Me.LblJenisPl = New System.Windows.Forms.Label()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.TxtPajakRp = New System.Windows.Forms.TextBox()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.kode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NamaBarang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Satuan = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Harga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TotalDiskon = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TotalHarga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Btnsimpan = New System.Windows.Forms.Button()
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.TxtIdKomputer = New System.Windows.Forms.TextBox()
        Me.TxtIdUser = New System.Windows.Forms.TextBox()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnCetak
        '
        Me.BtnCetak.AutoSize = True
        Me.BtnCetak.BackColor = System.Drawing.Color.White
        Me.BtnCetak.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCetak.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(100, 116, 139)
        Me.BtnCetak.FlatAppearance.BorderSize = 1
        Me.BtnCetak.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(226, 232, 240)
        Me.BtnCetak.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(241, 245, 249)
        Me.BtnCetak.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCetak.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCetak.ForeColor = System.Drawing.Color.Black
        Me.BtnCetak.Image = CType(resources.GetObject("BtnCetak.Image"), System.Drawing.Image)
        Me.BtnCetak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCetak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.Location = New System.Drawing.Point(586, 325)
        Me.BtnCetak.Name = "BtnCetak"
        Me.BtnCetak.Size = New System.Drawing.Size(120, 33)
        Me.BtnCetak.TabIndex = 163
        Me.BtnCetak.Text = "Cetak"
        Me.BtnCetak.UseVisualStyleBackColor = False
        Me.BtnCetak.Visible = False
        '
        'LblStatus
        '
        Me.LblStatus.AutoSize = True
        Me.LblStatus.BackColor = System.Drawing.Color.Transparent
        Me.LblStatus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatus.Location = New System.Drawing.Point(63, 335)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(113, 17)
        Me.LblStatus.TabIndex = 162
        Me.LblStatus.Text = "Status Transaksi :"
        '
        'Button1
        '
        Me.Button1.AutoSize = True
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.Button1.FlatAppearance.BorderSize = 1
        Me.Button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.Button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.Button1.Location = New System.Drawing.Point(742, 233)
        Me.Button1.Name = "Button1"
        Me.Button1.Image = CType(resources.GetObject("Button1.Image"), System.Drawing.Image)
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button1.Size = New System.Drawing.Size(242, 33)
        Me.Button1.TabIndex = 161
        Me.Button1.Text = "Keluar"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'TxtStatusTrans
        '
        Me.TxtStatusTrans.BackColor = System.Drawing.Color.White
        Me.TxtStatusTrans.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStatusTrans.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStatusTrans.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtStatusTrans.Location = New System.Drawing.Point(185, 335)
        Me.TxtStatusTrans.Name = "TxtStatusTrans"
        Me.TxtStatusTrans.Size = New System.Drawing.Size(217, 23)
        Me.TxtStatusTrans.TabIndex = 160
        '
        'TxtBAntuanbayar
        '
        Me.TxtBAntuanbayar.BackColor = System.Drawing.Color.White
        Me.TxtBAntuanbayar.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBAntuanbayar.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBAntuanbayar.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtBAntuanbayar.Location = New System.Drawing.Point(408, 304)
        Me.TxtBAntuanbayar.Name = "TxtBAntuanbayar"
        Me.TxtBAntuanbayar.ReadOnly = True
        Me.TxtBAntuanbayar.Size = New System.Drawing.Size(77, 19)
        Me.TxtBAntuanbayar.TabIndex = 159
        Me.TxtBAntuanbayar.Text = "0"
        Me.TxtBAntuanbayar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(59, 103)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(117, 17)
        Me.Label2.TabIndex = 158
        Me.Label2.Text = "Jenis Pelanggan "
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(103, 189)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(73, 17)
        Me.Label6.TabIndex = 157
        Me.Label6.Text = "Pajak Rp :"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(75, 129)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(101, 17)
        Me.Label12.TabIndex = 156
        Me.Label12.Text = "JumlahBarang"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(21, 158)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(155, 17)
        Me.Label9.TabIndex = 155
        Me.Label9.Text = "Diskon GrandTotal Rp :"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(93, 75)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 17)
        Me.Label4.TabIndex = 154
        Me.Label4.Text = "Pelanggan "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(116, 42)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 17)
        Me.Label3.TabIndex = 153
        Me.Label3.Text = "Tanggal"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(106, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 17)
        Me.Label1.TabIndex = 152
        Me.Label1.Text = "No Faktur"
        '
        'LblJatuhTempo
        '
        Me.LblJatuhTempo.AutoSize = True
        Me.LblJatuhTempo.BackColor = System.Drawing.Color.Transparent
        Me.LblJatuhTempo.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJatuhTempo.Location = New System.Drawing.Point(76, 309)
        Me.LblJatuhTempo.Name = "LblJatuhTempo"
        Me.LblJatuhTempo.Size = New System.Drawing.Size(100, 17)
        Me.LblJatuhTempo.TabIndex = 151
        Me.LblJatuhTempo.Text = "Jatuh Tempo :"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(129, 221)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(47, 17)
        Me.Label7.TabIndex = 150
        Me.Label7.Text = "Total :"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(110, 248)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(66, 17)
        Me.Label5.TabIndex = 149
        Me.Label5.Text = "Dibayar :"
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.Location = New System.Drawing.Point(185, 12)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.Size = New System.Drawing.Size(200, 23)
        Me.TxtFaktur.TabIndex = 139
        '
        'DTPJatuhTempo
        '
        Me.DTPJatuhTempo.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPJatuhTempo.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPJatuhTempo.Location = New System.Drawing.Point(185, 306)
        Me.DTPJatuhTempo.Name = "DTPJatuhTempo"
        Me.DTPJatuhTempo.Size = New System.Drawing.Size(200, 23)
        Me.DTPJatuhTempo.TabIndex = 148
        '
        'TxtBayar
        '
        Me.TxtBayar.BackColor = System.Drawing.Color.White
        Me.TxtBayar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBayar.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBayar.ForeColor = System.Drawing.Color.Black
        Me.TxtBayar.Location = New System.Drawing.Point(185, 248)
        Me.TxtBayar.Name = "TxtBayar"
        Me.TxtBayar.Size = New System.Drawing.Size(200, 23)
        Me.TxtBayar.TabIndex = 140
        Me.TxtBayar.Text = "0"
        Me.TxtBayar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtKembali
        '
        Me.TxtKembali.BackColor = System.Drawing.Color.White
        Me.TxtKembali.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKembali.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKembali.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtKembali.Location = New System.Drawing.Point(185, 277)
        Me.TxtKembali.Name = "TxtKembali"
        Me.TxtKembali.ReadOnly = True
        Me.TxtKembali.Size = New System.Drawing.Size(200, 23)
        Me.TxtKembali.TabIndex = 138
        Me.TxtKembali.Text = "0"
        Me.TxtKembali.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtJmlhBrg
        '
        Me.TxtJmlhBrg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtJmlhBrg.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJmlhBrg.Location = New System.Drawing.Point(185, 129)
        Me.TxtJmlhBrg.Name = "TxtJmlhBrg"
        Me.TxtJmlhBrg.ReadOnly = True
        Me.TxtJmlhBrg.Size = New System.Drawing.Size(200, 23)
        Me.TxtJmlhBrg.TabIndex = 147
        '
        'LblPembayaran
        '
        Me.LblPembayaran.AutoSize = True
        Me.LblPembayaran.BackColor = System.Drawing.Color.Transparent
        Me.LblPembayaran.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblPembayaran.Location = New System.Drawing.Point(90, 279)
        Me.LblPembayaran.Name = "LblPembayaran"
        Me.LblPembayaran.Size = New System.Drawing.Size(86, 17)
        Me.LblPembayaran.TabIndex = 137
        Me.LblPembayaran.Text = "Kembalian :"
        '
        'TxtTotal
        '
        Me.TxtTotal.BackColor = System.Drawing.Color.White
        Me.TxtTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotal.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtTotal.Location = New System.Drawing.Point(185, 219)
        Me.TxtTotal.Name = "TxtTotal"
        Me.TxtTotal.Size = New System.Drawing.Size(200, 23)
        Me.TxtTotal.TabIndex = 143
        Me.TxtTotal.Text = "0"
        Me.TxtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CmbPelanggan
        '
        Me.CmbPelanggan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbPelanggan.FormattingEnabled = True
        Me.CmbPelanggan.Location = New System.Drawing.Point(185, 72)
        Me.CmbPelanggan.Name = "CmbPelanggan"
        Me.CmbPelanggan.Size = New System.Drawing.Size(200, 25)
        Me.CmbPelanggan.TabIndex = 145
        '
        'TxtDiskonRp
        '
        Me.TxtDiskonRp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtDiskonRp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDiskonRp.Location = New System.Drawing.Point(185, 158)
        Me.TxtDiskonRp.Name = "TxtDiskonRp"
        Me.TxtDiskonRp.Size = New System.Drawing.Size(200, 23)
        Me.TxtDiskonRp.TabIndex = 146
        '
        'LblJenisPl
        '
        Me.LblJenisPl.AutoSize = True
        Me.LblJenisPl.BackColor = System.Drawing.Color.Transparent
        Me.LblJenisPl.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisPl.Location = New System.Drawing.Point(185, 103)
        Me.LblJenisPl.Name = "LblJenisPl"
        Me.LblJenisPl.Size = New System.Drawing.Size(79, 17)
        Me.LblJenisPl.TabIndex = 144
        Me.LblJenisPl.Text = "Pelanggan"
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(185, 42)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(200, 23)
        Me.DTPTgl.TabIndex = 141
        '
        'TxtPajakRp
        '
        Me.TxtPajakRp.BackColor = System.Drawing.Color.White
        Me.TxtPajakRp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPajakRp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPajakRp.ForeColor = System.Drawing.Color.Black
        Me.TxtPajakRp.Location = New System.Drawing.Point(185, 187)
        Me.TxtPajakRp.Name = "TxtPajakRp"
        Me.TxtPajakRp.Size = New System.Drawing.Size(200, 23)
        Me.TxtPajakRp.TabIndex = 142
        Me.TxtPajakRp.Text = "0"
        Me.TxtPajakRp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'DgvData
        '
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.kode, Me.NamaBarang, Me.QTY, Me.Satuan, Me.Harga, Me.TotalDiskon, Me.TotalHarga})
        Me.DgvData.Location = New System.Drawing.Point(408, 12)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.Size = New System.Drawing.Size(700, 198)
        Me.DgvData.TabIndex = 136
        '
        'kode
        '
        Me.kode.HeaderText = "kode"
        Me.kode.Name = "kode"
        '
        'NamaBarang
        '
        Me.NamaBarang.HeaderText = "NamaBarang"
        Me.NamaBarang.Name = "NamaBarang"
        '
        'QTY
        '
        Me.QTY.HeaderText = "QTY"
        Me.QTY.Name = "QTY"
        '
        'Satuan
        '
        Me.Satuan.HeaderText = "Satuan"
        Me.Satuan.Name = "Satuan"
        '
        'Harga
        '
        Me.Harga.HeaderText = "Harga"
        Me.Harga.Name = "Harga"
        '
        'TotalDiskon
        '
        Me.TotalDiskon.HeaderText = "TotalDiskon"
        Me.TotalDiskon.Name = "TotalDiskon"
        '
        'TotalHarga
        '
        Me.TotalHarga.HeaderText = "TotalHarga"
        Me.TotalHarga.Name = "TotalHarga"
        '
        'Btnsimpan
        '
        Me.Btnsimpan.AutoSize = True
        Me.Btnsimpan.BackColor = System.Drawing.Color.White
        Me.Btnsimpan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btnsimpan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.Btnsimpan.FlatAppearance.BorderSize = 1
        Me.Btnsimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.Btnsimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.Btnsimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Btnsimpan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Btnsimpan.ForeColor = System.Drawing.Color.Black
        Me.Btnsimpan.Location = New System.Drawing.Point(464, 233)
        Me.Btnsimpan.Name = "Btnsimpan"
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.Btnsimpan.Size = New System.Drawing.Size(242, 33)
        Me.Btnsimpan.Image = CType(resources.GetObject("Btnsimpan.Image"), System.Drawing.Image)
        Me.Btnsimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Btnsimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Btnsimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Btnsimpan.TabIndex = 135
        Me.Btnsimpan.Text = "Cetak"
        Me.Btnsimpan.UseVisualStyleBackColor = False
        '
        'TxtIdKomputer
        '
        Me.TxtIdKomputer.BackColor = System.Drawing.Color.White
        Me.TxtIdKomputer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdKomputer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdKomputer.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtIdKomputer.Location = New System.Drawing.Point(727, 335)
        Me.TxtIdKomputer.Name = "TxtIdKomputer"
        Me.TxtIdKomputer.Size = New System.Drawing.Size(217, 23)
        Me.TxtIdKomputer.TabIndex = 164
        Me.TxtIdKomputer.Text = "Komputer"
        '
        'TxtIdUser
        '
        Me.TxtIdUser.BackColor = System.Drawing.Color.White
        Me.TxtIdUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdUser.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdUser.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtIdUser.Location = New System.Drawing.Point(727, 305)
        Me.TxtIdUser.Name = "TxtIdUser"
        Me.TxtIdUser.Size = New System.Drawing.Size(217, 23)
        Me.TxtIdUser.TabIndex = 165
        Me.TxtIdUser.Text = "User"
        '
        'PrintReturJualThermal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1124, 465)
        Me.Controls.Add(Me.TxtIdUser)
        Me.Controls.Add(Me.TxtIdKomputer)
        Me.Controls.Add(Me.BtnCetak)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TxtStatusTrans)
        Me.Controls.Add(Me.TxtBAntuanbayar)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LblJatuhTempo)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TxtFaktur)
        Me.Controls.Add(Me.DTPJatuhTempo)
        Me.Controls.Add(Me.TxtBayar)
        Me.Controls.Add(Me.TxtKembali)
        Me.Controls.Add(Me.TxtJmlhBrg)
        Me.Controls.Add(Me.LblPembayaran)
        Me.Controls.Add(Me.TxtTotal)
        Me.Controls.Add(Me.CmbPelanggan)
        Me.Controls.Add(Me.TxtDiskonRp)
        Me.Controls.Add(Me.LblJenisPl)
        Me.Controls.Add(Me.DTPTgl)
        Me.Controls.Add(Me.TxtPajakRp)
        Me.Controls.Add(Me.DgvData)
        Me.Controls.Add(Me.Btnsimpan)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "PrintReturJualThermal"
        Me.ShowInTaskbar = False
        Me.Text = "PrintReturJual"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnCetak As System.Windows.Forms.Button
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents TxtStatusTrans As System.Windows.Forms.TextBox
    Friend WithEvents TxtBAntuanbayar As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LblJatuhTempo As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TxtFaktur As System.Windows.Forms.TextBox
    Friend WithEvents DTPJatuhTempo As System.Windows.Forms.DateTimePicker
    Friend WithEvents TxtBayar As System.Windows.Forms.TextBox
    Friend WithEvents TxtKembali As System.Windows.Forms.TextBox
    Friend WithEvents TxtJmlhBrg As System.Windows.Forms.TextBox
    Friend WithEvents LblPembayaran As System.Windows.Forms.Label
    Friend WithEvents TxtTotal As System.Windows.Forms.TextBox
    Friend WithEvents CmbPelanggan As System.Windows.Forms.ComboBox
    Friend WithEvents TxtDiskonRp As System.Windows.Forms.TextBox
    Friend WithEvents LblJenisPl As System.Windows.Forms.Label
    Friend WithEvents DTPTgl As System.Windows.Forms.DateTimePicker
    Friend WithEvents TxtPajakRp As System.Windows.Forms.TextBox
    Friend WithEvents DgvData As System.Windows.Forms.DataGridView
    Friend WithEvents kode As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NamaBarang As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QTY As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Satuan As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Harga As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TotalDiskon As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TotalHarga As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Btnsimpan As System.Windows.Forms.Button
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents TxtIdKomputer As System.Windows.Forms.TextBox
    Friend WithEvents TxtIdUser As System.Windows.Forms.TextBox
End Class