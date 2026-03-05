<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormBayarPiutang
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
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormBayarPiutang))
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtRekening = New System.Windows.Forms.Label()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.LblNomorBayar = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblSisaPiutang = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtSisaPiutang = New System.Windows.Forms.TextBox()
        Me.LblTotalPiutang = New System.Windows.Forms.Label()
        Me.LblTotalBayar = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.TxtTotalPiutang = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TxtTotalBayar = New System.Windows.Forms.TextBox()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.Check = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.IDPEMBELIAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.KODESUPLIYER = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMASUPLIYER = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Uraian = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TGLPEMBELIAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TOTALBELANJA = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DIBAYAR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Retur = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NOMINALHUTANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TGLJATUHTEMPO = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Bayar = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.View = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.LblKodePelanggan = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.CmbPelanggan = New System.Windows.Forms.ComboBox()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.BtnBayar = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PanelView = New System.Windows.Forms.Panel()
        Me.LblDetail = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.DgvDetail = New System.Windows.Forms.DataGridView()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelView.SuspendLayout()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(18, 128)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(97, 16)
        Me.Label2.TabIndex = 130
        Me.Label2.Text = "Metode bayar :"
        '
        'TxtRekening
        '
        Me.TxtRekening.AutoSize = True
        Me.TxtRekening.BackColor = System.Drawing.Color.Transparent
        Me.TxtRekening.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtRekening.Location = New System.Drawing.Point(319, 128)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.Size = New System.Drawing.Size(39, 16)
        Me.TxtRekening.TabIndex = 129
        Me.TxtRekening.Text = "Kode"
        Me.TxtRekening.Visible = False
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Location = New System.Drawing.Point(126, 124)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(187, 24)
        Me.CmbRekening.TabIndex = 128
        '
        'LblNomorBayar
        '
        Me.LblNomorBayar.AutoSize = True
        Me.LblNomorBayar.BackColor = System.Drawing.Color.Transparent
        Me.LblNomorBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNomorBayar.Location = New System.Drawing.Point(126, 73)
        Me.LblNomorBayar.Name = "LblNomorBayar"
        Me.LblNomorBayar.Size = New System.Drawing.Size(39, 16)
        Me.LblNomorBayar.TabIndex = 127
        Me.LblNomorBayar.Text = "Kode"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(61, 73)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(54, 16)
        Me.Label4.TabIndex = 126
        Me.Label4.Text = "Nomor :"
        '
        'LblSisaPiutang
        '
        Me.LblSisaPiutang.BackColor = System.Drawing.Color.Transparent
        Me.LblSisaPiutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSisaPiutang.Location = New System.Drawing.Point(558, 128)
        Me.LblSisaPiutang.Name = "LblSisaPiutang"
        Me.LblSisaPiutang.Size = New System.Drawing.Size(91, 18)
        Me.LblSisaPiutang.TabIndex = 125
        Me.LblSisaPiutang.Text = "0"
        Me.LblSisaPiutang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(439, 129)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(112, 16)
        Me.Label3.TabIndex = 124
        Me.Label3.Text = "Sisa Piutang : Rp."
        '
        'TxtSisaPiutang
        '
        Me.TxtSisaPiutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSisaPiutang.Location = New System.Drawing.Point(669, 126)
        Me.TxtSisaPiutang.Name = "TxtSisaPiutang"
        Me.TxtSisaPiutang.Size = New System.Drawing.Size(126, 22)
        Me.TxtSisaPiutang.TabIndex = 123
        Me.TxtSisaPiutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtSisaPiutang.Visible = False
        '
        'LblTotalPiutang
        '
        Me.LblTotalPiutang.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalPiutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalPiutang.Location = New System.Drawing.Point(558, 75)
        Me.LblTotalPiutang.Name = "LblTotalPiutang"
        Me.LblTotalPiutang.Size = New System.Drawing.Size(91, 18)
        Me.LblTotalPiutang.TabIndex = 122
        Me.LblTotalPiutang.Text = "0"
        Me.LblTotalPiutang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblTotalBayar
        '
        Me.LblTotalBayar.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalBayar.Location = New System.Drawing.Point(558, 102)
        Me.LblTotalBayar.Name = "LblTotalBayar"
        Me.LblTotalBayar.Size = New System.Drawing.Size(91, 18)
        Me.LblTotalBayar.TabIndex = 121
        Me.LblTotalBayar.Text = "0"
        Me.LblTotalBayar.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.BackColor = System.Drawing.Color.Transparent
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(435, 76)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(116, 16)
        Me.Label22.TabIndex = 120
        Me.Label22.Text = "Total Piutang : Rp."
        '
        'TxtTotalPiutang
        '
        Me.TxtTotalPiutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalPiutang.Location = New System.Drawing.Point(669, 73)
        Me.TxtTotalPiutang.Name = "TxtTotalPiutang"
        Me.TxtTotalPiutang.Size = New System.Drawing.Size(126, 22)
        Me.TxtTotalPiutang.TabIndex = 119
        Me.TxtTotalPiutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtTotalPiutang.Visible = False
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.BackColor = System.Drawing.Color.Transparent
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(444, 103)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(107, 16)
        Me.Label19.TabIndex = 118
        Me.Label19.Text = "Total Bayar : Rp."
        '
        'TxtTotalBayar
        '
        Me.TxtTotalBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalBayar.Location = New System.Drawing.Point(669, 100)
        Me.TxtTotalBayar.Name = "TxtTotalBayar"
        Me.TxtTotalBayar.Size = New System.Drawing.Size(126, 22)
        Me.TxtTotalBayar.TabIndex = 117
        Me.TxtTotalBayar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtTotalBayar.Visible = False
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CustomFormat = "dd/MM/yyyy"
        Me.DtpTanggal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(126, 43)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(121, 22)
        Me.DtpTanggal.TabIndex = 114
        '
        'DgvData
        '
        Me.DgvData.AllowUserToAddRows = False
        Me.DgvData.AllowUserToDeleteRows = False
        Me.DgvData.AllowUserToResizeRows = False
        Me.DgvData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvData.BackgroundColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Check, Me.IDPEMBELIAN, Me.KODESUPLIYER, Me.NAMASUPLIYER, Me.Uraian, Me.TGLPEMBELIAN, Me.TOTALBELANJA, Me.DIBAYAR, Me.Retur, Me.NOMINALHUTANG, Me.TGLJATUHTEMPO, Me.Bayar, Me.View})
        Me.DgvData.EnableHeadersVisualStyles = False
        Me.DgvData.Location = New System.Drawing.Point(6, 165)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvData.Size = New System.Drawing.Size(1136, 385)
        Me.DgvData.TabIndex = 110
        '
        'Check
        '
        Me.Check.FillWeight = 40.0!
        Me.Check.HeaderText = "Pilih"
        Me.Check.Name = "Check"
        '
        'IDPEMBELIAN
        '
        Me.IDPEMBELIAN.HeaderText = "No Nota"
        Me.IDPEMBELIAN.Name = "IDPEMBELIAN"
        '
        'KODESUPLIYER
        '
        Me.KODESUPLIYER.FillWeight = 60.0!
        Me.KODESUPLIYER.HeaderText = "Kode"
        Me.KODESUPLIYER.Name = "KODESUPLIYER"
        '
        'NAMASUPLIYER
        '
        Me.NAMASUPLIYER.FillWeight = 80.0!
        Me.NAMASUPLIYER.HeaderText = "Supliyer"
        Me.NAMASUPLIYER.Name = "NAMASUPLIYER"
        '
        'Uraian
        '
        Me.Uraian.FillWeight = 50.0!
        Me.Uraian.HeaderText = "Jenis"
        Me.Uraian.Name = "Uraian"
        '
        'TGLPEMBELIAN
        '
        Me.TGLPEMBELIAN.HeaderText = "Tgl Pembelian"
        Me.TGLPEMBELIAN.Name = "TGLPEMBELIAN"
        '
        'TOTALBELANJA
        '
        Me.TOTALBELANJA.HeaderText = "Total Belanja"
        Me.TOTALBELANJA.Name = "TOTALBELANJA"
        '
        'DIBAYAR
        '
        Me.DIBAYAR.HeaderText = "Sudah Dibayar"
        Me.DIBAYAR.Name = "DIBAYAR"
        '
        'Retur
        '
        Me.Retur.HeaderText = "Retur"
        Me.Retur.Name = "Retur"
        '
        'NOMINALHUTANG
        '
        Me.NOMINALHUTANG.HeaderText = "Hutang"
        Me.NOMINALHUTANG.Name = "NOMINALHUTANG"
        '
        'TGLJATUHTEMPO
        '
        Me.TGLJATUHTEMPO.HeaderText = "Jatuh Tempo"
        Me.TGLJATUHTEMPO.Name = "TGLJATUHTEMPO"
        '
        'Bayar
        '
        Me.Bayar.HeaderText = "Pembayaran"
        Me.Bayar.Name = "Bayar"
        '
        'View
        '
        Me.View.HeaderText = "Lihat"
        Me.View.Name = "View"
        Me.View.Text = "Detail"
        Me.View.UseColumnTextForButtonValue = True
        '
        'LblKodePelanggan
        '
        Me.LblKodePelanggan.AutoSize = True
        Me.LblKodePelanggan.BackColor = System.Drawing.Color.Transparent
        Me.LblKodePelanggan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodePelanggan.Location = New System.Drawing.Point(319, 99)
        Me.LblKodePelanggan.Name = "LblKodePelanggan"
        Me.LblKodePelanggan.Size = New System.Drawing.Size(39, 16)
        Me.LblKodePelanggan.TabIndex = 113
        Me.LblKodePelanggan.Text = "Kode"
        Me.LblKodePelanggan.Visible = False
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(36, 99)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(79, 16)
        Me.Label12.TabIndex = 112
        Me.Label12.Text = "Pelanggan :"
        '
        'CmbPelanggan
        '
        Me.CmbPelanggan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPelanggan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbPelanggan.FormattingEnabled = True
        Me.CmbPelanggan.Location = New System.Drawing.Point(126, 95)
        Me.CmbPelanggan.Name = "CmbPelanggan"
        Me.CmbPelanggan.Size = New System.Drawing.Size(187, 24)
        Me.CmbPelanggan.TabIndex = 111
        '
        'LblUtama
        '
        Me.LblUtama.BackColor = System.Drawing.Color.Sienna
        Me.LblUtama.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.Color.PaleGreen
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(1156, 36)
        Me.LblUtama.TabIndex = 109
        Me.LblUtama.Text = "BAYAR PIUTANG PENJUALAN"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnKeluar
        '
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluar.BackColor = System.Drawing.Color.Red
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderSize = 0
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.HotPink
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson
        Me.BtnKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluar.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.White
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.Location = New System.Drawing.Point(1112, 0)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(32, 32)
        Me.BtnKeluar.TabIndex = 116
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'BtnBayar
        '
        Me.BtnBayar.BackColor = System.Drawing.Color.GreenYellow
        Me.BtnBayar.FlatAppearance.BorderSize = 0
        Me.BtnBayar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnBayar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MidnightBlue
        Me.BtnBayar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBayar.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBayar.ForeColor = System.Drawing.Color.Black
        Me.BtnBayar.Image = CType(resources.GetObject("BtnBayar.Image"), System.Drawing.Image)
        Me.BtnBayar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayar.Location = New System.Drawing.Point(819, 113)
        Me.BtnBayar.Name = "BtnBayar"
        Me.BtnBayar.Size = New System.Drawing.Size(153, 35)
        Me.BtnBayar.TabIndex = 115
        Me.BtnBayar.Text = "       Simpan (F8)"
        Me.BtnBayar.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(51, 46)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 16)
        Me.Label1.TabIndex = 131
        Me.Label1.Text = "Tanggal :"
        '
        'PanelView
        '
        Me.PanelView.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.PanelView.BackColor = System.Drawing.Color.Orange
        Me.PanelView.Controls.Add(Me.LblDetail)
        Me.PanelView.Controls.Add(Me.Button2)
        Me.PanelView.Controls.Add(Me.DgvDetail)
        Me.PanelView.Location = New System.Drawing.Point(200, 207)
        Me.PanelView.Name = "PanelView"
        Me.PanelView.Size = New System.Drawing.Size(753, 333)
        Me.PanelView.TabIndex = 132
        '
        'LblDetail
        '
        Me.LblDetail.AutoSize = True
        Me.LblDetail.BackColor = System.Drawing.Color.Transparent
        Me.LblDetail.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDetail.Location = New System.Drawing.Point(3, 5)
        Me.LblDetail.Name = "LblDetail"
        Me.LblDetail.Size = New System.Drawing.Size(103, 16)
        Me.LblDetail.TabIndex = 234
        Me.LblDetail.Text = "Sumber dana :"
        '
        'Button2
        '
        Me.Button2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button2.BackColor = System.Drawing.Color.Red
        Me.Button2.FlatAppearance.BorderSize = 0
        Me.Button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.ForeColor = System.Drawing.Color.White
        Me.Button2.Image = CType(resources.GetObject("Button2.Image"), System.Drawing.Image)
        Me.Button2.Location = New System.Drawing.Point(727, 3)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(23, 23)
        Me.Button2.TabIndex = 233
        Me.Button2.UseVisualStyleBackColor = False
        '
        'DgvDetail
        '
        Me.DgvDetail.AllowUserToAddRows = False
        Me.DgvDetail.AllowUserToDeleteRows = False
        Me.DgvDetail.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvDetail.BackgroundColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvDetail.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        Me.DgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvDetail.DefaultCellStyle = DataGridViewCellStyle6
        Me.DgvDetail.Location = New System.Drawing.Point(3, 27)
        Me.DgvDetail.Name = "DgvDetail"
        Me.DgvDetail.RowHeadersVisible = False
        Me.DgvDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvDetail.Size = New System.Drawing.Size(747, 303)
        Me.DgvDetail.TabIndex = 0
        '
        'FormBayarPiutang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1156, 562)
        Me.Controls.Add(Me.PanelView)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtRekening)
        Me.Controls.Add(Me.CmbRekening)
        Me.Controls.Add(Me.LblNomorBayar)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.LblSisaPiutang)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtSisaPiutang)
        Me.Controls.Add(Me.LblTotalPiutang)
        Me.Controls.Add(Me.LblTotalBayar)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.TxtTotalPiutang)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.TxtTotalBayar)
        Me.Controls.Add(Me.BtnKeluar)
        Me.Controls.Add(Me.BtnBayar)
        Me.Controls.Add(Me.DtpTanggal)
        Me.Controls.Add(Me.DgvData)
        Me.Controls.Add(Me.LblKodePelanggan)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.CmbPelanggan)
        Me.Controls.Add(Me.LblUtama)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormBayarPiutang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormBayarPiutang"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelView.ResumeLayout(False)
        Me.PanelView.PerformLayout()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtRekening As System.Windows.Forms.Label
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents LblNomorBayar As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LblSisaPiutang As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtSisaPiutang As System.Windows.Forms.TextBox
    Friend WithEvents LblTotalPiutang As System.Windows.Forms.Label
    Friend WithEvents LblTotalBayar As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalPiutang As System.Windows.Forms.TextBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalBayar As System.Windows.Forms.TextBox
    Friend WithEvents BtnKeluar As System.Windows.Forms.Button
    Friend WithEvents BtnBayar As System.Windows.Forms.Button
    Friend WithEvents DtpTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents DgvData As System.Windows.Forms.DataGridView
    Friend WithEvents LblKodePelanggan As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents CmbPelanggan As System.Windows.Forms.ComboBox
    Friend WithEvents LblUtama As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents PanelView As System.Windows.Forms.Panel
    Friend WithEvents LblDetail As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents DgvDetail As System.Windows.Forms.DataGridView
    Friend WithEvents Check As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents IDPEMBELIAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents KODESUPLIYER As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NAMASUPLIYER As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Uraian As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TGLPEMBELIAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TOTALBELANJA As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DIBAYAR As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Retur As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NOMINALHUTANG As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TGLJATUHTEMPO As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Bayar As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents View As System.Windows.Forms.DataGridViewButtonColumn
End Class
