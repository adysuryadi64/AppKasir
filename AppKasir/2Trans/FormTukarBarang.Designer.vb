<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormTukarBarang
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTukarBarang))
        Me.GBPilihan = New System.Windows.Forms.GroupBox()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.ID_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMA_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ISI_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA_JUAL = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TOTAL_DISKON = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ListBoxFakturjual = New System.Windows.Forms.ListBox()
        Me.LblKodePelanggan = New System.Windows.Forms.Label()
        Me.LblNamaPel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.TxtHargaKeluar = New System.Windows.Forms.TextBox()
        Me.CmbSatuanKeluar = New System.Windows.Forms.ComboBox()
        Me.TxtNamaKeluar = New System.Windows.Forms.TextBox()
        Me.TxtIsiKeluar = New System.Windows.Forms.TextBox()
        Me.TxtKodeKeluar = New System.Windows.Forms.TextBox()
        Me.QtyKeluar = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.LblHargaSatKeluar = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblDiskonKeluar = New System.Windows.Forms.Label()
        Me.LblTotalhargaKeluar = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.TxtDiskonKeluar = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.TxtQtySatKeluar = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TxtTotalHargaKeluar = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.LblDiskonMasuk = New System.Windows.Forms.Label()
        Me.LblTotalhargaMasuk = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.TxtDiskonMasuk = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.TxtQtySat = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TxtTotalHargaMasuk = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtKodeMasuk = New System.Windows.Forms.TextBox()
        Me.LblHargaSatMasuk = New System.Windows.Forms.Label()
        Me.TxtIsiMasuk = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TxtNamaMasuk = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.CmbSatuanMasuk = New System.Windows.Forms.ComboBox()
        Me.TxtHargaMasuk = New System.Windows.Forms.TextBox()
        Me.QtyMasuk = New System.Windows.Forms.TextBox()
        Me.LblIdTransaksi = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.LblIdFakturJual = New System.Windows.Forms.Label()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.LblJenisPel = New System.Windows.Forms.Label()
        Me.LblKeterangan = New System.Windows.Forms.Label()
        Me.LblNominalKet = New System.Windows.Forms.Label()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.BtnBayar = New System.Windows.Forms.Button()
        Me.GBPilihan.SuspendLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GBPilihan
        '
        Me.GBPilihan.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GBPilihan.BackColor = System.Drawing.Color.Tan
        Me.GBPilihan.Controls.Add(Me.DgvData)
        Me.GBPilihan.Controls.Add(Me.Label2)
        Me.GBPilihan.Controls.Add(Me.Label1)
        Me.GBPilihan.Controls.Add(Me.ListBoxFakturjual)
        Me.GBPilihan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBPilihan.ForeColor = System.Drawing.Color.Black
        Me.GBPilihan.Location = New System.Drawing.Point(12, 91)
        Me.GBPilihan.Name = "GBPilihan"
        Me.GBPilihan.Size = New System.Drawing.Size(600, 466)
        Me.GBPilihan.TabIndex = 5
        Me.GBPilihan.TabStop = False
        Me.GBPilihan.Text = "Pilih barang berdasarkan dari penjualan"
        '
        'DgvData
        '
        Me.DgvData.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvData.BackgroundColor = System.Drawing.Color.Tan
        Me.DgvData.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ID_BARANG, Me.NAMA_BARANG, Me.QTY, Me.SATUAN, Me.ISI_SATUAN, Me.HARGA_JUAL, Me.QTY_SATUAN, Me.TOTAL_DISKON, Me.HARGA})
        Me.DgvData.EnableHeadersVisualStyles = False
        Me.DgvData.Location = New System.Drawing.Point(135, 53)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.ReadOnly = True
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvData.Size = New System.Drawing.Size(459, 404)
        Me.DgvData.TabIndex = 78
        '
        'ID_BARANG
        '
        Me.ID_BARANG.FillWeight = 60.0!
        Me.ID_BARANG.HeaderText = "KODE"
        Me.ID_BARANG.Name = "ID_BARANG"
        Me.ID_BARANG.ReadOnly = True
        '
        'NAMA_BARANG
        '
        Me.NAMA_BARANG.FillWeight = 150.0!
        Me.NAMA_BARANG.HeaderText = "NAMA_BARANG"
        Me.NAMA_BARANG.Name = "NAMA_BARANG"
        Me.NAMA_BARANG.ReadOnly = True
        '
        'QTY
        '
        Me.QTY.FillWeight = 30.0!
        Me.QTY.HeaderText = "QTY"
        Me.QTY.Name = "QTY"
        Me.QTY.ReadOnly = True
        '
        'SATUAN
        '
        Me.SATUAN.FillWeight = 50.0!
        Me.SATUAN.HeaderText = "SATUAN"
        Me.SATUAN.Name = "SATUAN"
        Me.SATUAN.ReadOnly = True
        '
        'ISI_SATUAN
        '
        Me.ISI_SATUAN.HeaderText = "ISI SAT"
        Me.ISI_SATUAN.Name = "ISI_SATUAN"
        Me.ISI_SATUAN.ReadOnly = True
        '
        'HARGA_JUAL
        '
        Me.HARGA_JUAL.HeaderText = "HARGA SAT"
        Me.HARGA_JUAL.Name = "HARGA_JUAL"
        Me.HARGA_JUAL.ReadOnly = True
        '
        'QTY_SATUAN
        '
        Me.QTY_SATUAN.HeaderText = "QTY SAT"
        Me.QTY_SATUAN.Name = "QTY_SATUAN"
        Me.QTY_SATUAN.ReadOnly = True
        '
        'TOTAL_DISKON
        '
        Me.TOTAL_DISKON.HeaderText = "DISKON"
        Me.TOTAL_DISKON.Name = "TOTAL_DISKON"
        Me.TOTAL_DISKON.ReadOnly = True
        '
        'HARGA
        '
        Me.HARGA.FillWeight = 60.0!
        Me.HARGA.HeaderText = "HARGA"
        Me.HARGA.Name = "HARGA"
        Me.HARGA.ReadOnly = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(310, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(164, 16)
        Me.Label2.TabIndex = 77
        Me.Label2.Text = "Barang yang akan di tukar"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(6, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 16)
        Me.Label1.TabIndex = 76
        Me.Label1.Text = "Nota Jual"
        '
        'ListBoxFakturjual
        '
        Me.ListBoxFakturjual.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListBoxFakturjual.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListBoxFakturjual.FormattingEnabled = True
        Me.ListBoxFakturjual.ItemHeight = 16
        Me.ListBoxFakturjual.Location = New System.Drawing.Point(6, 53)
        Me.ListBoxFakturjual.Name = "ListBoxFakturjual"
        Me.ListBoxFakturjual.Size = New System.Drawing.Size(123, 404)
        Me.ListBoxFakturjual.TabIndex = 74
        '
        'LblKodePelanggan
        '
        Me.LblKodePelanggan.AutoSize = True
        Me.LblKodePelanggan.BackColor = System.Drawing.Color.Transparent
        Me.LblKodePelanggan.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodePelanggan.Location = New System.Drawing.Point(877, 68)
        Me.LblKodePelanggan.Name = "LblKodePelanggan"
        Me.LblKodePelanggan.Size = New System.Drawing.Size(43, 18)
        Me.LblKodePelanggan.TabIndex = 73
        Me.LblKodePelanggan.Text = "Kode"
        Me.LblKodePelanggan.Visible = False
        '
        'LblNamaPel
        '
        Me.LblNamaPel.AutoSize = True
        Me.LblNamaPel.BackColor = System.Drawing.Color.Transparent
        Me.LblNamaPel.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNamaPel.Location = New System.Drawing.Point(713, 68)
        Me.LblNamaPel.Name = "LblNamaPel"
        Me.LblNamaPel.Size = New System.Drawing.Size(77, 18)
        Me.LblNamaPel.TabIndex = 26
        Me.LblNamaPel.Text = "Pelanggan"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(622, 68)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(85, 18)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "Pelanggan :"
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy"
        Me.DTPTgl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(165, 64)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(95, 22)
        Me.DTPTgl.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(10, 67)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(149, 16)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Pilih Tanggal penjualan"
        '
        'LblUtama
        '
        Me.LblUtama.BackColor = System.Drawing.Color.Sienna
        Me.LblUtama.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.Color.PaleGreen
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(1125, 36)
        Me.LblUtama.TabIndex = 74
        Me.LblUtama.Text = "TUKAR BARANG PENJUALAN"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtHargaKeluar
        '
        Me.TxtHargaKeluar.Enabled = False
        Me.TxtHargaKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaKeluar.Location = New System.Drawing.Point(366, 105)
        Me.TxtHargaKeluar.Name = "TxtHargaKeluar"
        Me.TxtHargaKeluar.ReadOnly = True
        Me.TxtHargaKeluar.Size = New System.Drawing.Size(112, 24)
        Me.TxtHargaKeluar.TabIndex = 77
        Me.TxtHargaKeluar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtHargaKeluar.Visible = False
        '
        'CmbSatuanKeluar
        '
        Me.CmbSatuanKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSatuanKeluar.FormattingEnabled = True
        Me.CmbSatuanKeluar.Location = New System.Drawing.Point(115, 104)
        Me.CmbSatuanKeluar.Name = "CmbSatuanKeluar"
        Me.CmbSatuanKeluar.Size = New System.Drawing.Size(99, 26)
        Me.CmbSatuanKeluar.TabIndex = 78
        '
        'TxtNamaKeluar
        '
        Me.TxtNamaKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaKeluar.Location = New System.Drawing.Point(115, 25)
        Me.TxtNamaKeluar.Name = "TxtNamaKeluar"
        Me.TxtNamaKeluar.Size = New System.Drawing.Size(363, 24)
        Me.TxtNamaKeluar.TabIndex = 76
        '
        'TxtIsiKeluar
        '
        Me.TxtIsiKeluar.BackColor = System.Drawing.Color.White
        Me.TxtIsiKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiKeluar.Location = New System.Drawing.Point(214, 105)
        Me.TxtIsiKeluar.Name = "TxtIsiKeluar"
        Me.TxtIsiKeluar.ReadOnly = True
        Me.TxtIsiKeluar.Size = New System.Drawing.Size(27, 24)
        Me.TxtIsiKeluar.TabIndex = 79
        '
        'TxtKodeKeluar
        '
        Me.TxtKodeKeluar.Enabled = False
        Me.TxtKodeKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKodeKeluar.Location = New System.Drawing.Point(115, 51)
        Me.TxtKodeKeluar.Name = "TxtKodeKeluar"
        Me.TxtKodeKeluar.ReadOnly = True
        Me.TxtKodeKeluar.Size = New System.Drawing.Size(177, 24)
        Me.TxtKodeKeluar.TabIndex = 75
        '
        'QtyKeluar
        '
        Me.QtyKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QtyKeluar.Location = New System.Drawing.Point(115, 77)
        Me.QtyKeluar.Name = "QtyKeluar"
        Me.QtyKeluar.Size = New System.Drawing.Size(53, 24)
        Me.QtyKeluar.TabIndex = 80
        Me.QtyKeluar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(10, 54)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(94, 18)
        Me.Label6.TabIndex = 81
        Me.Label6.Text = "Kode Barang"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(10, 108)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(54, 18)
        Me.Label7.TabIndex = 81
        Me.Label7.Text = "Satuan"
        '
        'LblHargaSatKeluar
        '
        Me.LblHargaSatKeluar.AutoSize = True
        Me.LblHargaSatKeluar.BackColor = System.Drawing.Color.Transparent
        Me.LblHargaSatKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHargaSatKeluar.Location = New System.Drawing.Point(247, 107)
        Me.LblHargaSatKeluar.Name = "LblHargaSatKeluar"
        Me.LblHargaSatKeluar.Size = New System.Drawing.Size(96, 18)
        Me.LblHargaSatKeluar.TabIndex = 81
        Me.LblHargaSatKeluar.Text = "Harga satuan"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(10, 80)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(38, 18)
        Me.Label9.TabIndex = 81
        Me.Label9.Text = "QTY"
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.BackColor = System.Drawing.Color.Aquamarine
        Me.Panel1.Controls.Add(Me.LblDiskonKeluar)
        Me.Panel1.Controls.Add(Me.LblTotalhargaKeluar)
        Me.Panel1.Controls.Add(Me.Label22)
        Me.Panel1.Controls.Add(Me.TxtDiskonKeluar)
        Me.Panel1.Controls.Add(Me.Label23)
        Me.Panel1.Controls.Add(Me.TxtQtySatKeluar)
        Me.Panel1.Controls.Add(Me.Label16)
        Me.Panel1.Controls.Add(Me.Label19)
        Me.Panel1.Controls.Add(Me.TxtTotalHargaKeluar)
        Me.Panel1.Controls.Add(Me.Label15)
        Me.Panel1.Controls.Add(Me.Label9)
        Me.Panel1.Controls.Add(Me.TxtKodeKeluar)
        Me.Panel1.Controls.Add(Me.LblHargaSatKeluar)
        Me.Panel1.Controls.Add(Me.TxtIsiKeluar)
        Me.Panel1.Controls.Add(Me.Label7)
        Me.Panel1.Controls.Add(Me.TxtNamaKeluar)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.CmbSatuanKeluar)
        Me.Panel1.Controls.Add(Me.TxtHargaKeluar)
        Me.Panel1.Controls.Add(Me.QtyKeluar)
        Me.Panel1.Location = New System.Drawing.Point(618, 290)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(495, 195)
        Me.Panel1.TabIndex = 82
        '
        'LblDiskonKeluar
        '
        Me.LblDiskonKeluar.AutoSize = True
        Me.LblDiskonKeluar.BackColor = System.Drawing.Color.Transparent
        Me.LblDiskonKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDiskonKeluar.Location = New System.Drawing.Point(247, 135)
        Me.LblDiskonKeluar.Name = "LblDiskonKeluar"
        Me.LblDiskonKeluar.Size = New System.Drawing.Size(43, 18)
        Me.LblDiskonKeluar.TabIndex = 94
        Me.LblDiskonKeluar.Text = "Rp. 0"
        '
        'LblTotalhargaKeluar
        '
        Me.LblTotalhargaKeluar.AutoSize = True
        Me.LblTotalhargaKeluar.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalhargaKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalhargaKeluar.Location = New System.Drawing.Point(247, 162)
        Me.LblTotalhargaKeluar.Name = "LblTotalhargaKeluar"
        Me.LblTotalhargaKeluar.Size = New System.Drawing.Size(43, 18)
        Me.LblTotalhargaKeluar.TabIndex = 93
        Me.LblTotalhargaKeluar.Text = "Rp. 0"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.BackColor = System.Drawing.Color.Transparent
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(10, 135)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(55, 18)
        Me.Label22.TabIndex = 92
        Me.Label22.Text = "Diskon"
        '
        'TxtDiskonKeluar
        '
        Me.TxtDiskonKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDiskonKeluar.Location = New System.Drawing.Point(115, 132)
        Me.TxtDiskonKeluar.Name = "TxtDiskonKeluar"
        Me.TxtDiskonKeluar.Size = New System.Drawing.Size(126, 24)
        Me.TxtDiskonKeluar.TabIndex = 91
        Me.TxtDiskonKeluar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.BackColor = System.Drawing.Color.Transparent
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(303, 79)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(57, 18)
        Me.Label23.TabIndex = 90
        Me.Label23.Text = "Qty Sat"
        Me.Label23.Visible = False
        '
        'TxtQtySatKeluar
        '
        Me.TxtQtySatKeluar.Enabled = False
        Me.TxtQtySatKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQtySatKeluar.Location = New System.Drawing.Point(366, 77)
        Me.TxtQtySatKeluar.Name = "TxtQtySatKeluar"
        Me.TxtQtySatKeluar.ReadOnly = True
        Me.TxtQtySatKeluar.Size = New System.Drawing.Size(112, 24)
        Me.TxtQtySatKeluar.TabIndex = 89
        Me.TxtQtySatKeluar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtQtySatKeluar.Visible = False
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.BackColor = System.Drawing.Color.Transparent
        Me.Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(10, 28)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(99, 18)
        Me.Label16.TabIndex = 87
        Me.Label16.Text = "Nama Barang"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.BackColor = System.Drawing.Color.Transparent
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(10, 162)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(85, 18)
        Me.Label19.TabIndex = 86
        Me.Label19.Text = "Total Harga"
        '
        'TxtTotalHargaKeluar
        '
        Me.TxtTotalHargaKeluar.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalHargaKeluar.Location = New System.Drawing.Point(115, 159)
        Me.TxtTotalHargaKeluar.Name = "TxtTotalHargaKeluar"
        Me.TxtTotalHargaKeluar.Size = New System.Drawing.Size(126, 24)
        Me.TxtTotalHargaKeluar.TabIndex = 85
        Me.TxtTotalHargaKeluar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.Color.Transparent
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(4, 4)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(232, 18)
        Me.Label15.TabIndex = 83
        Me.Label15.Text = "BARANG KELUAR / PENGGANTI"
        '
        'Panel2
        '
        Me.Panel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel2.BackColor = System.Drawing.Color.Salmon
        Me.Panel2.Controls.Add(Me.LblDiskonMasuk)
        Me.Panel2.Controls.Add(Me.LblTotalhargaMasuk)
        Me.Panel2.Controls.Add(Me.Label21)
        Me.Panel2.Controls.Add(Me.TxtDiskonMasuk)
        Me.Panel2.Controls.Add(Me.Label20)
        Me.Panel2.Controls.Add(Me.TxtQtySat)
        Me.Panel2.Controls.Add(Me.Label18)
        Me.Panel2.Controls.Add(Me.TxtTotalHargaMasuk)
        Me.Panel2.Controls.Add(Me.Label14)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.TxtKodeMasuk)
        Me.Panel2.Controls.Add(Me.LblHargaSatMasuk)
        Me.Panel2.Controls.Add(Me.TxtIsiMasuk)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.TxtNamaMasuk)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.CmbSatuanMasuk)
        Me.Panel2.Controls.Add(Me.TxtHargaMasuk)
        Me.Panel2.Controls.Add(Me.QtyMasuk)
        Me.Panel2.Location = New System.Drawing.Point(618, 91)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(495, 193)
        Me.Panel2.TabIndex = 83
        '
        'LblDiskonMasuk
        '
        Me.LblDiskonMasuk.AutoSize = True
        Me.LblDiskonMasuk.BackColor = System.Drawing.Color.Transparent
        Me.LblDiskonMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDiskonMasuk.Location = New System.Drawing.Point(247, 137)
        Me.LblDiskonMasuk.Name = "LblDiskonMasuk"
        Me.LblDiskonMasuk.Size = New System.Drawing.Size(43, 18)
        Me.LblDiskonMasuk.TabIndex = 90
        Me.LblDiskonMasuk.Text = "Rp. 0"
        '
        'LblTotalhargaMasuk
        '
        Me.LblTotalhargaMasuk.AutoSize = True
        Me.LblTotalhargaMasuk.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalhargaMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalhargaMasuk.Location = New System.Drawing.Point(247, 164)
        Me.LblTotalhargaMasuk.Name = "LblTotalhargaMasuk"
        Me.LblTotalhargaMasuk.Size = New System.Drawing.Size(43, 18)
        Me.LblTotalhargaMasuk.TabIndex = 89
        Me.LblTotalhargaMasuk.Text = "Rp. 0"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.BackColor = System.Drawing.Color.Transparent
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(11, 136)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(55, 18)
        Me.Label21.TabIndex = 88
        Me.Label21.Text = "Diskon"
        '
        'TxtDiskonMasuk
        '
        Me.TxtDiskonMasuk.Enabled = False
        Me.TxtDiskonMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDiskonMasuk.Location = New System.Drawing.Point(115, 134)
        Me.TxtDiskonMasuk.Name = "TxtDiskonMasuk"
        Me.TxtDiskonMasuk.ReadOnly = True
        Me.TxtDiskonMasuk.Size = New System.Drawing.Size(126, 24)
        Me.TxtDiskonMasuk.TabIndex = 87
        Me.TxtDiskonMasuk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.BackColor = System.Drawing.Color.Transparent
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(303, 83)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(57, 18)
        Me.Label20.TabIndex = 86
        Me.Label20.Text = "Qty Sat"
        Me.Label20.Visible = False
        '
        'TxtQtySat
        '
        Me.TxtQtySat.Enabled = False
        Me.TxtQtySat.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQtySat.Location = New System.Drawing.Point(366, 81)
        Me.TxtQtySat.Name = "TxtQtySat"
        Me.TxtQtySat.ReadOnly = True
        Me.TxtQtySat.Size = New System.Drawing.Size(112, 24)
        Me.TxtQtySat.TabIndex = 85
        Me.TxtQtySat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtQtySat.Visible = False
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.BackColor = System.Drawing.Color.Transparent
        Me.Label18.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.Location = New System.Drawing.Point(11, 163)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(85, 18)
        Me.Label18.TabIndex = 84
        Me.Label18.Text = "Total Harga"
        '
        'TxtTotalHargaMasuk
        '
        Me.TxtTotalHargaMasuk.Enabled = False
        Me.TxtTotalHargaMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalHargaMasuk.Location = New System.Drawing.Point(115, 160)
        Me.TxtTotalHargaMasuk.Name = "TxtTotalHargaMasuk"
        Me.TxtTotalHargaMasuk.ReadOnly = True
        Me.TxtTotalHargaMasuk.Size = New System.Drawing.Size(126, 24)
        Me.TxtTotalHargaMasuk.TabIndex = 83
        Me.TxtTotalHargaMasuk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(4, 3)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(248, 18)
        Me.Label14.TabIndex = 82
        Me.Label14.Text = "BARANG MASUK / YANG DITUKAR"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(11, 34)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(99, 18)
        Me.Label5.TabIndex = 81
        Me.Label5.Text = "Nama Barang"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(11, 84)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(38, 18)
        Me.Label10.TabIndex = 81
        Me.Label10.Text = "QTY"
        '
        'TxtKodeMasuk
        '
        Me.TxtKodeMasuk.Enabled = False
        Me.TxtKodeMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKodeMasuk.Location = New System.Drawing.Point(115, 56)
        Me.TxtKodeMasuk.Name = "TxtKodeMasuk"
        Me.TxtKodeMasuk.ReadOnly = True
        Me.TxtKodeMasuk.Size = New System.Drawing.Size(177, 24)
        Me.TxtKodeMasuk.TabIndex = 75
        '
        'LblHargaSatMasuk
        '
        Me.LblHargaSatMasuk.AutoSize = True
        Me.LblHargaSatMasuk.BackColor = System.Drawing.Color.Transparent
        Me.LblHargaSatMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHargaSatMasuk.Location = New System.Drawing.Point(247, 109)
        Me.LblHargaSatMasuk.Name = "LblHargaSatMasuk"
        Me.LblHargaSatMasuk.Size = New System.Drawing.Size(98, 18)
        Me.LblHargaSatMasuk.TabIndex = 81
        Me.LblHargaSatMasuk.Text = "Harga Satuan"
        '
        'TxtIsiMasuk
        '
        Me.TxtIsiMasuk.BackColor = System.Drawing.Color.White
        Me.TxtIsiMasuk.Enabled = False
        Me.TxtIsiMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiMasuk.Location = New System.Drawing.Point(214, 107)
        Me.TxtIsiMasuk.Name = "TxtIsiMasuk"
        Me.TxtIsiMasuk.ReadOnly = True
        Me.TxtIsiMasuk.Size = New System.Drawing.Size(27, 24)
        Me.TxtIsiMasuk.TabIndex = 79
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(11, 110)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(54, 18)
        Me.Label12.TabIndex = 81
        Me.Label12.Text = "Satuan"
        '
        'TxtNamaMasuk
        '
        Me.TxtNamaMasuk.Enabled = False
        Me.TxtNamaMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaMasuk.Location = New System.Drawing.Point(115, 31)
        Me.TxtNamaMasuk.Name = "TxtNamaMasuk"
        Me.TxtNamaMasuk.ReadOnly = True
        Me.TxtNamaMasuk.Size = New System.Drawing.Size(363, 24)
        Me.TxtNamaMasuk.TabIndex = 76
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(11, 59)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(94, 18)
        Me.Label13.TabIndex = 81
        Me.Label13.Text = "Kode Barang"
        '
        'CmbSatuanMasuk
        '
        Me.CmbSatuanMasuk.Enabled = False
        Me.CmbSatuanMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSatuanMasuk.FormattingEnabled = True
        Me.CmbSatuanMasuk.Location = New System.Drawing.Point(115, 106)
        Me.CmbSatuanMasuk.Name = "CmbSatuanMasuk"
        Me.CmbSatuanMasuk.Size = New System.Drawing.Size(99, 26)
        Me.CmbSatuanMasuk.TabIndex = 78
        '
        'TxtHargaMasuk
        '
        Me.TxtHargaMasuk.Enabled = False
        Me.TxtHargaMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaMasuk.Location = New System.Drawing.Point(366, 106)
        Me.TxtHargaMasuk.Name = "TxtHargaMasuk"
        Me.TxtHargaMasuk.ReadOnly = True
        Me.TxtHargaMasuk.Size = New System.Drawing.Size(112, 24)
        Me.TxtHargaMasuk.TabIndex = 77
        Me.TxtHargaMasuk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtHargaMasuk.Visible = False
        '
        'QtyMasuk
        '
        Me.QtyMasuk.Enabled = False
        Me.QtyMasuk.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QtyMasuk.Location = New System.Drawing.Point(115, 81)
        Me.QtyMasuk.Name = "QtyMasuk"
        Me.QtyMasuk.ReadOnly = True
        Me.QtyMasuk.Size = New System.Drawing.Size(53, 24)
        Me.QtyMasuk.TabIndex = 80
        Me.QtyMasuk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblIdTransaksi
        '
        Me.LblIdTransaksi.AutoSize = True
        Me.LblIdTransaksi.BackColor = System.Drawing.Color.Transparent
        Me.LblIdTransaksi.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblIdTransaksi.Location = New System.Drawing.Point(165, 42)
        Me.LblIdTransaksi.Name = "LblIdTransaksi"
        Me.LblIdTransaksi.Size = New System.Drawing.Size(30, 16)
        Me.LblIdTransaksi.TabIndex = 85
        Me.LblIdTransaksi.Text = "TB-"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.BackColor = System.Drawing.Color.Transparent
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(70, 42)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(89, 16)
        Me.Label17.TabIndex = 84
        Me.Label17.Text = "No Transaksi"
        '
        'LblIdFakturJual
        '
        Me.LblIdFakturJual.AutoSize = True
        Me.LblIdFakturJual.BackColor = System.Drawing.Color.Transparent
        Me.LblIdFakturJual.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblIdFakturJual.Location = New System.Drawing.Point(352, 42)
        Me.LblIdFakturJual.Name = "LblIdFakturJual"
        Me.LblIdFakturJual.Size = New System.Drawing.Size(47, 16)
        Me.LblIdFakturJual.TabIndex = 88
        Me.LblIdFakturJual.Text = "Id Jual"
        Me.LblIdFakturJual.Visible = False
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CustomFormat = "dd/MM/yyyy"
        Me.DtpTanggal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(456, 39)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(95, 22)
        Me.DtpTanggal.TabIndex = 9
        Me.DtpTanggal.Visible = False
        '
        'LblJenisPel
        '
        Me.LblJenisPel.AutoSize = True
        Me.LblJenisPel.BackColor = System.Drawing.Color.Transparent
        Me.LblJenisPel.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisPel.Location = New System.Drawing.Point(935, 67)
        Me.LblJenisPel.Name = "LblJenisPel"
        Me.LblJenisPel.Size = New System.Drawing.Size(43, 18)
        Me.LblJenisPel.TabIndex = 89
        Me.LblJenisPel.Text = "Jenis"
        Me.LblJenisPel.Visible = False
        '
        'LblKeterangan
        '
        Me.LblKeterangan.AutoSize = True
        Me.LblKeterangan.BackColor = System.Drawing.Color.Transparent
        Me.LblKeterangan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblKeterangan.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKeterangan.Location = New System.Drawing.Point(629, 488)
        Me.LblKeterangan.Name = "LblKeterangan"
        Me.LblKeterangan.Size = New System.Drawing.Size(185, 22)
        Me.LblKeterangan.TabIndex = 90
        Me.LblKeterangan.Text = "Kembali / Kekurangan"
        '
        'LblNominalKet
        '
        Me.LblNominalKet.AutoSize = True
        Me.LblNominalKet.BackColor = System.Drawing.Color.Transparent
        Me.LblNominalKet.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNominalKet.Location = New System.Drawing.Point(1028, 488)
        Me.LblNominalKet.Name = "LblNominalKet"
        Me.LblNominalKet.Size = New System.Drawing.Size(35, 18)
        Me.LblNominalKet.TabIndex = 91
        Me.LblNominalKet.Text = "Rp. "
        Me.LblNominalKet.Visible = False
        '
        'BtnKeluar
        '
        Me.BtnKeluar.BackColor = System.Drawing.Color.Red
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderSize = 0
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.HotPink
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson
        Me.BtnKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluar.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.White
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.Location = New System.Drawing.Point(1000, 522)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(106, 30)
        Me.BtnKeluar.TabIndex = 87
        Me.BtnKeluar.Text = "     Keluar"
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'BtnBayar
        '
        Me.BtnBayar.BackColor = System.Drawing.Color.Red
        Me.BtnBayar.FlatAppearance.BorderSize = 0
        Me.BtnBayar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnBayar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MidnightBlue
        Me.BtnBayar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBayar.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBayar.ForeColor = System.Drawing.Color.White
        Me.BtnBayar.Image = CType(resources.GetObject("BtnBayar.Image"), System.Drawing.Image)
        Me.BtnBayar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayar.Location = New System.Drawing.Point(632, 520)
        Me.BtnBayar.Name = "BtnBayar"
        Me.BtnBayar.Size = New System.Drawing.Size(123, 35)
        Me.BtnBayar.TabIndex = 86
        Me.BtnBayar.Text = "       Simpan"
        Me.BtnBayar.UseVisualStyleBackColor = False
        '
        'FormTukarBarang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1125, 557)
        Me.Controls.Add(Me.LblNominalKet)
        Me.Controls.Add(Me.LblKeterangan)
        Me.Controls.Add(Me.LblJenisPel)
        Me.Controls.Add(Me.LblIdFakturJual)
        Me.Controls.Add(Me.BtnKeluar)
        Me.Controls.Add(Me.BtnBayar)
        Me.Controls.Add(Me.LblIdTransaksi)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblUtama)
        Me.Controls.Add(Me.LblKodePelanggan)
        Me.Controls.Add(Me.DtpTanggal)
        Me.Controls.Add(Me.DTPTgl)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.GBPilihan)
        Me.Controls.Add(Me.LblNamaPel)
        Me.Controls.Add(Me.Label4)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormTukarBarang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.GBPilihan.ResumeLayout(False)
        Me.GBPilihan.PerformLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GBPilihan As System.Windows.Forms.GroupBox
    Friend WithEvents DTPTgl As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents LblKodePelanggan As System.Windows.Forms.Label
    Friend WithEvents LblNamaPel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ListBoxFakturjual As System.Windows.Forms.ListBox
    Friend WithEvents LblUtama As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtHargaKeluar As System.Windows.Forms.TextBox
    Friend WithEvents CmbSatuanKeluar As System.Windows.Forms.ComboBox
    Friend WithEvents TxtNamaKeluar As System.Windows.Forms.TextBox
    Friend WithEvents TxtIsiKeluar As System.Windows.Forms.TextBox
    Friend WithEvents TxtKodeKeluar As System.Windows.Forms.TextBox
    Friend WithEvents QtyKeluar As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents LblHargaSatKeluar As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TxtKodeMasuk As System.Windows.Forms.TextBox
    Friend WithEvents LblHargaSatMasuk As System.Windows.Forms.Label
    Friend WithEvents TxtIsiMasuk As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents TxtNamaMasuk As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents CmbSatuanMasuk As System.Windows.Forms.ComboBox
    Friend WithEvents TxtHargaMasuk As System.Windows.Forms.TextBox
    Friend WithEvents QtyMasuk As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents LblIdTransaksi As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalHargaKeluar As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalHargaMasuk As System.Windows.Forms.TextBox
    Friend WithEvents BtnKeluar As System.Windows.Forms.Button
    Friend WithEvents BtnBayar As System.Windows.Forms.Button
    Friend WithEvents LblIdFakturJual As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents DtpTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents DgvData As System.Windows.Forms.DataGridView
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents TxtDiskonMasuk As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents TxtQtySat As System.Windows.Forms.TextBox
    Friend WithEvents ID_BARANG As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NAMA_BARANG As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QTY As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SATUAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ISI_SATUAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HARGA_JUAL As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QTY_SATUAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TOTAL_DISKON As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HARGA As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents LblJenisPel As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents TxtDiskonKeluar As System.Windows.Forms.TextBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TxtQtySatKeluar As System.Windows.Forms.TextBox
    Friend WithEvents LblKeterangan As System.Windows.Forms.Label
    Friend WithEvents LblNominalKet As System.Windows.Forms.Label
    Friend WithEvents LblDiskonKeluar As System.Windows.Forms.Label
    Friend WithEvents LblTotalhargaKeluar As System.Windows.Forms.Label
    Friend WithEvents LblDiskonMasuk As System.Windows.Forms.Label
    Friend WithEvents LblTotalhargaMasuk As System.Windows.Forms.Label
End Class
