<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintJual
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintJual))
        Me.Btnsimpan = New System.Windows.Forms.Button()
        Me.DgvData = New System.Windows.Forms.DataGridView()
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
        Me.LblJatuhTempo = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtBAntuanbayar = New System.Windows.Forms.TextBox()
        Me.TxtStatusTrans = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.BtnCetak = New System.Windows.Forms.Button()
        Me.TxtIdUser = New System.Windows.Forms.TextBox()
        Me.TxtIdKomputer = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TxtNoRek = New System.Windows.Forms.TextBox()
        Me.TxtNamaRek = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtBank = New System.Windows.Forms.TextBox()
        Me.TxtType = New System.Windows.Forms.TextBox()
        Me.TxtPenerima = New System.Windows.Forms.TextBox()
        Me.TxtNoReff = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.TxtMetode = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.TxtSblPajak = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TxtBiayaKirim = New System.Windows.Forms.TextBox()
        Me.kode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NamaBarang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Satuan = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Harga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TotalDiskon = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TotalHarga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.No = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Btnsimpan
        '
        Me.Btnsimpan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Btnsimpan.Location = New System.Drawing.Point(898, 378)
        Me.Btnsimpan.Name = "Btnsimpan"
        Me.Btnsimpan.Size = New System.Drawing.Size(242, 50)
        Me.Btnsimpan.TabIndex = 0
        Me.Btnsimpan.Text = "Print"
        Me.Btnsimpan.UseVisualStyleBackColor = True
        '
        'DgvData
        '
        Me.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.kode, Me.NamaBarang, Me.QTY, Me.Satuan, Me.Harga, Me.TotalDiskon, Me.TotalHarga, Me.SN, Me.No})
        Me.DgvData.Location = New System.Drawing.Point(405, 12)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.Size = New System.Drawing.Size(613, 198)
        Me.DgvData.TabIndex = 3
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.Location = New System.Drawing.Point(171, 12)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.Size = New System.Drawing.Size(200, 23)
        Me.TxtFaktur.TabIndex = 101
        '
        'DTPJatuhTempo
        '
        Me.DTPJatuhTempo.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPJatuhTempo.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPJatuhTempo.Location = New System.Drawing.Point(171, 366)
        Me.DTPJatuhTempo.Name = "DTPJatuhTempo"
        Me.DTPJatuhTempo.Size = New System.Drawing.Size(200, 23)
        Me.DTPJatuhTempo.TabIndex = 110
        '
        'TxtBayar
        '
        Me.TxtBayar.BackColor = System.Drawing.Color.White
        Me.TxtBayar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBayar.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBayar.ForeColor = System.Drawing.Color.Black
        Me.TxtBayar.Location = New System.Drawing.Point(171, 308)
        Me.TxtBayar.Name = "TxtBayar"
        Me.TxtBayar.Size = New System.Drawing.Size(200, 23)
        Me.TxtBayar.TabIndex = 102
        Me.TxtBayar.Text = "0"
        '
        'TxtKembali
        '
        Me.TxtKembali.BackColor = System.Drawing.Color.White
        Me.TxtKembali.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKembali.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKembali.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtKembali.Location = New System.Drawing.Point(171, 337)
        Me.TxtKembali.Name = "TxtKembali"
        Me.TxtKembali.ReadOnly = True
        Me.TxtKembali.Size = New System.Drawing.Size(200, 23)
        Me.TxtKembali.TabIndex = 100
        Me.TxtKembali.Text = "0"
        '
        'TxtJmlhBrg
        '
        Me.TxtJmlhBrg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtJmlhBrg.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJmlhBrg.Location = New System.Drawing.Point(171, 129)
        Me.TxtJmlhBrg.Name = "TxtJmlhBrg"
        Me.TxtJmlhBrg.ReadOnly = True
        Me.TxtJmlhBrg.Size = New System.Drawing.Size(200, 23)
        Me.TxtJmlhBrg.TabIndex = 109
        '
        'LblPembayaran
        '
        Me.LblPembayaran.AutoSize = True
        Me.LblPembayaran.BackColor = System.Drawing.Color.Transparent
        Me.LblPembayaran.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblPembayaran.Location = New System.Drawing.Point(76, 339)
        Me.LblPembayaran.Name = "LblPembayaran"
        Me.LblPembayaran.Size = New System.Drawing.Size(86, 17)
        Me.LblPembayaran.TabIndex = 99
        Me.LblPembayaran.Text = "Kembalian :"
        '
        'TxtTotal
        '
        Me.TxtTotal.BackColor = System.Drawing.Color.White
        Me.TxtTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotal.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtTotal.Location = New System.Drawing.Point(171, 279)
        Me.TxtTotal.Name = "TxtTotal"
        Me.TxtTotal.Size = New System.Drawing.Size(200, 23)
        Me.TxtTotal.TabIndex = 105
        Me.TxtTotal.Text = "0"
        '
        'CmbPelanggan
        '
        Me.CmbPelanggan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbPelanggan.FormattingEnabled = True
        Me.CmbPelanggan.Location = New System.Drawing.Point(171, 72)
        Me.CmbPelanggan.Name = "CmbPelanggan"
        Me.CmbPelanggan.Size = New System.Drawing.Size(200, 25)
        Me.CmbPelanggan.TabIndex = 107
        '
        'TxtDiskonRp
        '
        Me.TxtDiskonRp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtDiskonRp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDiskonRp.Location = New System.Drawing.Point(171, 189)
        Me.TxtDiskonRp.Name = "TxtDiskonRp"
        Me.TxtDiskonRp.Size = New System.Drawing.Size(200, 23)
        Me.TxtDiskonRp.TabIndex = 108
        '
        'LblJenisPl
        '
        Me.LblJenisPl.AutoSize = True
        Me.LblJenisPl.BackColor = System.Drawing.Color.Transparent
        Me.LblJenisPl.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisPl.Location = New System.Drawing.Point(168, 103)
        Me.LblJenisPl.Name = "LblJenisPl"
        Me.LblJenisPl.Size = New System.Drawing.Size(79, 17)
        Me.LblJenisPl.TabIndex = 106
        Me.LblJenisPl.Text = "Pelanggan"
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(171, 42)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(200, 23)
        Me.DTPTgl.TabIndex = 103
        '
        'TxtPajakRp
        '
        Me.TxtPajakRp.BackColor = System.Drawing.Color.White
        Me.TxtPajakRp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPajakRp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPajakRp.ForeColor = System.Drawing.Color.Black
        Me.TxtPajakRp.Location = New System.Drawing.Point(171, 247)
        Me.TxtPajakRp.Name = "TxtPajakRp"
        Me.TxtPajakRp.Size = New System.Drawing.Size(200, 23)
        Me.TxtPajakRp.TabIndex = 104
        Me.TxtPajakRp.Text = "0"
        '
        'LblJatuhTempo
        '
        Me.LblJatuhTempo.AutoSize = True
        Me.LblJatuhTempo.BackColor = System.Drawing.Color.Transparent
        Me.LblJatuhTempo.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJatuhTempo.Location = New System.Drawing.Point(62, 369)
        Me.LblJatuhTempo.Name = "LblJatuhTempo"
        Me.LblJatuhTempo.Size = New System.Drawing.Size(100, 17)
        Me.LblJatuhTempo.TabIndex = 113
        Me.LblJatuhTempo.Text = "Jatuh Tempo :"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(115, 281)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(47, 17)
        Me.Label7.TabIndex = 112
        Me.Label7.Text = "Total :"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(96, 308)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(66, 17)
        Me.Label5.TabIndex = 111
        Me.Label5.Text = "Dibayar :"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(79, 75)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 17)
        Me.Label4.TabIndex = 116
        Me.Label4.Text = "Pelanggan "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(102, 42)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 17)
        Me.Label3.TabIndex = 115
        Me.Label3.Text = "Tanggal"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(92, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 17)
        Me.Label1.TabIndex = 114
        Me.Label1.Text = "No Faktur"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(61, 129)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(101, 17)
        Me.Label12.TabIndex = 118
        Me.Label12.Text = "JumlahBarang"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(7, 189)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(155, 17)
        Me.Label9.TabIndex = 117
        Me.Label9.Text = "Diskon GrandTotal Rp :"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(89, 249)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(73, 17)
        Me.Label6.TabIndex = 119
        Me.Label6.Text = "Pajak Rp :"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(45, 103)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(117, 17)
        Me.Label2.TabIndex = 120
        Me.Label2.Text = "Jenis Pelanggan "
        '
        'TxtBAntuanbayar
        '
        Me.TxtBAntuanbayar.BackColor = System.Drawing.Color.White
        Me.TxtBAntuanbayar.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBAntuanbayar.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBAntuanbayar.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtBAntuanbayar.Location = New System.Drawing.Point(171, 424)
        Me.TxtBAntuanbayar.Name = "TxtBAntuanbayar"
        Me.TxtBAntuanbayar.ReadOnly = True
        Me.TxtBAntuanbayar.Size = New System.Drawing.Size(200, 19)
        Me.TxtBAntuanbayar.TabIndex = 121
        Me.TxtBAntuanbayar.Text = "0"
        Me.TxtBAntuanbayar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtStatusTrans
        '
        Me.TxtStatusTrans.BackColor = System.Drawing.Color.White
        Me.TxtStatusTrans.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStatusTrans.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStatusTrans.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtStatusTrans.Location = New System.Drawing.Point(171, 395)
        Me.TxtStatusTrans.Name = "TxtStatusTrans"
        Me.TxtStatusTrans.Size = New System.Drawing.Size(200, 23)
        Me.TxtStatusTrans.TabIndex = 124
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(898, 437)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(242, 50)
        Me.Button1.TabIndex = 125
        Me.Button1.Text = "keluar"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'LblStatus
        '
        Me.LblStatus.AutoSize = True
        Me.LblStatus.BackColor = System.Drawing.Color.Transparent
        Me.LblStatus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatus.Location = New System.Drawing.Point(49, 395)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(113, 17)
        Me.LblStatus.TabIndex = 126
        Me.LblStatus.Text = "Status Transaksi :"
        '
        'BtnCetak
        '
        Me.BtnCetak.BackColor = System.Drawing.Color.Teal
        Me.BtnCetak.FlatAppearance.BorderSize = 0
        Me.BtnCetak.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnCetak.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BtnCetak.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCetak.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCetak.ForeColor = System.Drawing.Color.White
        Me.BtnCetak.Image = CType(resources.GetObject("BtnCetak.Image"), System.Drawing.Image)
        Me.BtnCetak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.Location = New System.Drawing.Point(898, 263)
        Me.BtnCetak.Name = "BtnCetak"
        Me.BtnCetak.Size = New System.Drawing.Size(120, 37)
        Me.BtnCetak.TabIndex = 134
        Me.BtnCetak.Text = "  CETAK"
        Me.BtnCetak.UseVisualStyleBackColor = False
        Me.BtnCetak.Visible = False
        '
        'TxtIdUser
        '
        Me.TxtIdUser.BackColor = System.Drawing.Color.White
        Me.TxtIdUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdUser.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdUser.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtIdUser.Location = New System.Drawing.Point(898, 309)
        Me.TxtIdUser.Name = "TxtIdUser"
        Me.TxtIdUser.Size = New System.Drawing.Size(217, 23)
        Me.TxtIdUser.TabIndex = 167
        Me.TxtIdUser.Text = "User"
        '
        'TxtIdKomputer
        '
        Me.TxtIdKomputer.BackColor = System.Drawing.Color.White
        Me.TxtIdKomputer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdKomputer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdKomputer.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtIdKomputer.Location = New System.Drawing.Point(898, 339)
        Me.TxtIdKomputer.Name = "TxtIdKomputer"
        Me.TxtIdKomputer.Size = New System.Drawing.Size(217, 23)
        Me.TxtIdKomputer.TabIndex = 166
        Me.TxtIdKomputer.Text = "Komputer"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(462, 255)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(69, 17)
        Me.Label8.TabIndex = 177
        Me.Label8.Text = "Penerima"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(487, 224)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(44, 17)
        Me.Label10.TabIndex = 176
        Me.Label10.Text = "Type :"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.Transparent
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(492, 287)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(39, 17)
        Me.Label11.TabIndex = 175
        Me.Label11.Text = "Bank"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(419, 314)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(112, 17)
        Me.Label13.TabIndex = 174
        Me.Label13.Text = "Nomor rekening"
        '
        'TxtNoRek
        '
        Me.TxtNoRek.BackColor = System.Drawing.Color.White
        Me.TxtNoRek.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNoRek.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNoRek.ForeColor = System.Drawing.Color.Black
        Me.TxtNoRek.Location = New System.Drawing.Point(539, 311)
        Me.TxtNoRek.Name = "TxtNoRek"
        Me.TxtNoRek.Size = New System.Drawing.Size(200, 23)
        Me.TxtNoRek.TabIndex = 170
        Me.TxtNoRek.Text = "0"
        '
        'TxtNamaRek
        '
        Me.TxtNamaRek.BackColor = System.Drawing.Color.White
        Me.TxtNamaRek.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNamaRek.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaRek.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtNamaRek.Location = New System.Drawing.Point(539, 343)
        Me.TxtNamaRek.Name = "TxtNamaRek"
        Me.TxtNamaRek.ReadOnly = True
        Me.TxtNamaRek.Size = New System.Drawing.Size(200, 23)
        Me.TxtNamaRek.TabIndex = 169
        Me.TxtNamaRek.Text = "0"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(423, 346)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(108, 17)
        Me.Label14.TabIndex = 168
        Me.Label14.Text = "Nama rekening"
        '
        'TxtBank
        '
        Me.TxtBank.BackColor = System.Drawing.Color.White
        Me.TxtBank.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBank.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBank.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtBank.Location = New System.Drawing.Point(539, 284)
        Me.TxtBank.Name = "TxtBank"
        Me.TxtBank.Size = New System.Drawing.Size(200, 23)
        Me.TxtBank.TabIndex = 172
        Me.TxtBank.Text = "0"
        '
        'TxtType
        '
        Me.TxtType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtType.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtType.Location = New System.Drawing.Point(537, 221)
        Me.TxtType.Name = "TxtType"
        Me.TxtType.Size = New System.Drawing.Size(200, 23)
        Me.TxtType.TabIndex = 173
        '
        'TxtPenerima
        '
        Me.TxtPenerima.BackColor = System.Drawing.Color.White
        Me.TxtPenerima.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPenerima.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPenerima.ForeColor = System.Drawing.Color.Black
        Me.TxtPenerima.Location = New System.Drawing.Point(537, 252)
        Me.TxtPenerima.Name = "TxtPenerima"
        Me.TxtPenerima.Size = New System.Drawing.Size(200, 23)
        Me.TxtPenerima.TabIndex = 171
        Me.TxtPenerima.Text = "0"
        '
        'TxtNoReff
        '
        Me.TxtNoReff.BackColor = System.Drawing.Color.White
        Me.TxtNoReff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNoReff.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNoReff.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtNoReff.Location = New System.Drawing.Point(539, 374)
        Me.TxtNoReff.Name = "TxtNoReff"
        Me.TxtNoReff.ReadOnly = True
        Me.TxtNoReff.Size = New System.Drawing.Size(200, 23)
        Me.TxtNoReff.TabIndex = 179
        Me.TxtNoReff.Text = "0"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.Color.Transparent
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(444, 377)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(87, 17)
        Me.Label15.TabIndex = 178
        Me.Label15.Text = "No Referensi"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.BackColor = System.Drawing.Color.Transparent
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(49, 424)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(113, 17)
        Me.Label16.TabIndex = 126
        Me.Label16.Text = "bantuan bayar :"
        '
        'TxtKode
        '
        Me.TxtKode.BackColor = System.Drawing.Color.White
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.ForeColor = System.Drawing.Color.Black
        Me.TxtKode.Location = New System.Drawing.Point(761, 252)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.Size = New System.Drawing.Size(121, 23)
        Me.TxtKode.TabIndex = 171
        Me.TxtKode.Text = "0"
        '
        'TxtMetode
        '
        Me.TxtMetode.BackColor = System.Drawing.Color.White
        Me.TxtMetode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtMetode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtMetode.ForeColor = System.Drawing.Color.Black
        Me.TxtMetode.Location = New System.Drawing.Point(761, 309)
        Me.TxtMetode.Name = "TxtMetode"
        Me.TxtMetode.Size = New System.Drawing.Size(121, 23)
        Me.TxtMetode.TabIndex = 171
        Me.TxtMetode.Text = "0"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.BackColor = System.Drawing.Color.Transparent
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(57, 160)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(105, 17)
        Me.Label17.TabIndex = 181
        Me.Label17.Text = "Total sbl pajak:"
        '
        'TxtSblPajak
        '
        Me.TxtSblPajak.BackColor = System.Drawing.Color.White
        Me.TxtSblPajak.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSblPajak.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSblPajak.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtSblPajak.Location = New System.Drawing.Point(171, 158)
        Me.TxtSblPajak.Name = "TxtSblPajak"
        Me.TxtSblPajak.Size = New System.Drawing.Size(200, 23)
        Me.TxtSblPajak.TabIndex = 180
        Me.TxtSblPajak.Text = "0"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.BackColor = System.Drawing.Color.Transparent
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(83, 218)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(79, 17)
        Me.Label18.TabIndex = 183
        Me.Label18.Text = "Jasa kirim :"
        '
        'TxtBiayaKirim
        '
        Me.TxtBiayaKirim.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBiayaKirim.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBiayaKirim.Location = New System.Drawing.Point(171, 218)
        Me.TxtBiayaKirim.Name = "TxtBiayaKirim"
        Me.TxtBiayaKirim.Size = New System.Drawing.Size(200, 23)
        Me.TxtBiayaKirim.TabIndex = 182
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
        'SN
        '
        Me.SN.HeaderText = "SN"
        Me.SN.Name = "SN"
        '
        'No
        '
        Me.No.HeaderText = "Nomor"
        Me.No.Name = "No"
        '
        'PrintJual
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1152, 515)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.TxtBiayaKirim)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.TxtSblPajak)
        Me.Controls.Add(Me.TxtNoReff)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.TxtNoRek)
        Me.Controls.Add(Me.TxtNamaRek)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.TxtBank)
        Me.Controls.Add(Me.TxtType)
        Me.Controls.Add(Me.TxtMetode)
        Me.Controls.Add(Me.TxtKode)
        Me.Controls.Add(Me.TxtPenerima)
        Me.Controls.Add(Me.TxtIdUser)
        Me.Controls.Add(Me.TxtIdKomputer)
        Me.Controls.Add(Me.BtnCetak)
        Me.Controls.Add(Me.Label16)
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
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "PrintJual"
        Me.ShowInTaskbar = False
        Me.Text = "FormPrinting3"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Btnsimpan As System.Windows.Forms.Button
    Friend WithEvents DgvData As System.Windows.Forms.DataGridView
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
    Friend WithEvents LblJatuhTempo As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtBAntuanbayar As System.Windows.Forms.TextBox
    Friend WithEvents TxtStatusTrans As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents BtnCetak As Button
    Friend WithEvents TxtIdUser As System.Windows.Forms.TextBox
    Friend WithEvents TxtIdKomputer As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents TxtNoRek As System.Windows.Forms.TextBox
    Friend WithEvents TxtNamaRek As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents TxtBank As System.Windows.Forms.TextBox
    Friend WithEvents TxtType As System.Windows.Forms.TextBox
    Friend WithEvents TxtPenerima As System.Windows.Forms.TextBox
    Friend WithEvents TxtNoReff As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents TxtMetode As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents TxtSblPajak As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents TxtBiayaKirim As System.Windows.Forms.TextBox
    Friend WithEvents kode As DataGridViewTextBoxColumn
    Friend WithEvents NamaBarang As DataGridViewTextBoxColumn
    Friend WithEvents QTY As DataGridViewTextBoxColumn
    Friend WithEvents Satuan As DataGridViewTextBoxColumn
    Friend WithEvents Harga As DataGridViewTextBoxColumn
    Friend WithEvents TotalDiskon As DataGridViewTextBoxColumn
    Friend WithEvents TotalHarga As DataGridViewTextBoxColumn
    Friend WithEvents SN As DataGridViewTextBoxColumn
    Friend WithEvents No As DataGridViewTextBoxColumn
End Class
