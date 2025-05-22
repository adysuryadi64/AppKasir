<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormBayarHutang
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormBayarHutang))
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.Check = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.ID_PEMBELIAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ID_SUPPLIER = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMA_SUPLIYER = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TGL_BELI = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GRAND_TOTAL_BELI = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PEMBAYARAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RETUR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TAGIHAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.JATUH_TEMPO = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Bayar = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.View = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.CmbSupliyer = New System.Windows.Forms.ComboBox()
        Me.LblKodeSupliyer = New System.Windows.Forms.Label()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.BtnBayar = New System.Windows.Forms.Button()
        Me.LblTotalHutang = New System.Windows.Forms.Label()
        Me.LblTotalBayar = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.TxtTotalHutang = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TxtTotalBayar = New System.Windows.Forms.TextBox()
        Me.LblSisaHutang = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtSisaHutang = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblNomorBayar = New System.Windows.Forms.Label()
        Me.TxtRekening = New System.Windows.Forms.Label()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PanelView = New System.Windows.Forms.Panel()
        Me.LblDetail = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.DgvDetail = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelView.SuspendLayout()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LblUtama
        '
        Me.LblUtama.BackColor = System.Drawing.Color.Sienna
        Me.LblUtama.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.Color.PaleGreen
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(1135, 36)
        Me.LblUtama.TabIndex = 75
        Me.LblUtama.Text = "BAYAR HUTANG PEMBELIAN"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Check, Me.ID_PEMBELIAN, Me.ID_SUPPLIER, Me.NAMA_SUPLIYER, Me.TGL_BELI, Me.GRAND_TOTAL_BELI, Me.PEMBAYARAN, Me.RETUR, Me.TAGIHAN, Me.JATUH_TEMPO, Me.Bayar, Me.View})
        Me.DgvData.EnableHeadersVisualStyles = False
        Me.DgvData.Location = New System.Drawing.Point(8, 150)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvData.Size = New System.Drawing.Size(1117, 400)
        Me.DgvData.TabIndex = 79
        '
        'Check
        '
        Me.Check.FillWeight = 40.0!
        Me.Check.HeaderText = "Pilih"
        Me.Check.Name = "Check"
        '
        'ID_PEMBELIAN
        '
        Me.ID_PEMBELIAN.HeaderText = "No Nota"
        Me.ID_PEMBELIAN.Name = "ID_PEMBELIAN"
        Me.ID_PEMBELIAN.ReadOnly = True
        '
        'ID_SUPPLIER
        '
        Me.ID_SUPPLIER.FillWeight = 60.0!
        Me.ID_SUPPLIER.HeaderText = "Kode"
        Me.ID_SUPPLIER.Name = "ID_SUPPLIER"
        Me.ID_SUPPLIER.ReadOnly = True
        '
        'NAMA_SUPLIYER
        '
        Me.NAMA_SUPLIYER.FillWeight = 80.0!
        Me.NAMA_SUPLIYER.HeaderText = "Supliyer"
        Me.NAMA_SUPLIYER.Name = "NAMA_SUPLIYER"
        Me.NAMA_SUPLIYER.ReadOnly = True
        '
        'TGL_BELI
        '
        Me.TGL_BELI.HeaderText = "Tgl Beli"
        Me.TGL_BELI.Name = "TGL_BELI"
        Me.TGL_BELI.ReadOnly = True
        '
        'GRAND_TOTAL_BELI
        '
        Me.GRAND_TOTAL_BELI.HeaderText = "Total Belanja"
        Me.GRAND_TOTAL_BELI.Name = "GRAND_TOTAL_BELI"
        Me.GRAND_TOTAL_BELI.ReadOnly = True
        '
        'PEMBAYARAN
        '
        Me.PEMBAYARAN.HeaderText = "Pembayaran"
        Me.PEMBAYARAN.Name = "PEMBAYARAN"
        Me.PEMBAYARAN.ReadOnly = True
        '
        'RETUR
        '
        Me.RETUR.HeaderText = "Retur"
        Me.RETUR.Name = "RETUR"
        Me.RETUR.ReadOnly = True
        '
        'TAGIHAN
        '
        Me.TAGIHAN.HeaderText = "Hutang"
        Me.TAGIHAN.Name = "TAGIHAN"
        Me.TAGIHAN.ReadOnly = True
        '
        'JATUH_TEMPO
        '
        Me.JATUH_TEMPO.HeaderText = "Jatuh Tempo"
        Me.JATUH_TEMPO.Name = "JATUH_TEMPO"
        Me.JATUH_TEMPO.ReadOnly = True
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
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(37, 96)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(63, 16)
        Me.Label12.TabIndex = 84
        Me.Label12.Text = "Supliyer :"
        '
        'CmbSupliyer
        '
        Me.CmbSupliyer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSupliyer.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSupliyer.FormattingEnabled = True
        Me.CmbSupliyer.Location = New System.Drawing.Point(106, 92)
        Me.CmbSupliyer.Name = "CmbSupliyer"
        Me.CmbSupliyer.Size = New System.Drawing.Size(255, 24)
        Me.CmbSupliyer.TabIndex = 82
        '
        'LblKodeSupliyer
        '
        Me.LblKodeSupliyer.AutoSize = True
        Me.LblKodeSupliyer.BackColor = System.Drawing.Color.Transparent
        Me.LblKodeSupliyer.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeSupliyer.Location = New System.Drawing.Point(367, 96)
        Me.LblKodeSupliyer.Name = "LblKodeSupliyer"
        Me.LblKodeSupliyer.Size = New System.Drawing.Size(40, 16)
        Me.LblKodeSupliyer.TabIndex = 85
        Me.LblKodeSupliyer.Text = "Kode"
        Me.LblKodeSupliyer.Visible = False
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CustomFormat = "dd/MM/yyyy"
        Me.DtpTanggal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(106, 40)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(121, 22)
        Me.DtpTanggal.TabIndex = 86
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
        Me.BtnKeluar.Location = New System.Drawing.Point(1097, 2)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(32, 32)
        Me.BtnKeluar.TabIndex = 89
        Me.BtnKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
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
        Me.BtnBayar.Location = New System.Drawing.Point(776, 105)
        Me.BtnBayar.Name = "BtnBayar"
        Me.BtnBayar.Size = New System.Drawing.Size(149, 35)
        Me.BtnBayar.TabIndex = 88
        Me.BtnBayar.Text = "       Simpan (F8)"
        Me.BtnBayar.UseVisualStyleBackColor = False
        '
        'LblTotalHutang
        '
        Me.LblTotalHutang.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalHutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalHutang.Location = New System.Drawing.Point(561, 68)
        Me.LblTotalHutang.Name = "LblTotalHutang"
        Me.LblTotalHutang.Size = New System.Drawing.Size(100, 18)
        Me.LblTotalHutang.TabIndex = 100
        Me.LblTotalHutang.Text = "0"
        Me.LblTotalHutang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblTotalBayar
        '
        Me.LblTotalBayar.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalBayar.Location = New System.Drawing.Point(561, 95)
        Me.LblTotalBayar.Name = "LblTotalBayar"
        Me.LblTotalBayar.Size = New System.Drawing.Size(100, 18)
        Me.LblTotalBayar.TabIndex = 99
        Me.LblTotalBayar.Text = "0"
        Me.LblTotalBayar.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.BackColor = System.Drawing.Color.Transparent
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(438, 69)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(115, 16)
        Me.Label22.TabIndex = 98
        Me.Label22.Text = "Total Hutang : Rp."
        '
        'TxtTotalHutang
        '
        Me.TxtTotalHutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalHutang.Location = New System.Drawing.Point(694, 66)
        Me.TxtTotalHutang.Name = "TxtTotalHutang"
        Me.TxtTotalHutang.Size = New System.Drawing.Size(58, 22)
        Me.TxtTotalHutang.TabIndex = 97
        Me.TxtTotalHutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtTotalHutang.Visible = False
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.BackColor = System.Drawing.Color.Transparent
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(445, 96)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(108, 16)
        Me.Label19.TabIndex = 96
        Me.Label19.Text = "Total Bayar : Rp."
        '
        'TxtTotalBayar
        '
        Me.TxtTotalBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalBayar.Location = New System.Drawing.Point(694, 93)
        Me.TxtTotalBayar.Name = "TxtTotalBayar"
        Me.TxtTotalBayar.Size = New System.Drawing.Size(58, 22)
        Me.TxtTotalBayar.TabIndex = 95
        Me.TxtTotalBayar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtTotalBayar.Visible = False
        '
        'LblSisaHutang
        '
        Me.LblSisaHutang.BackColor = System.Drawing.Color.Transparent
        Me.LblSisaHutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSisaHutang.Location = New System.Drawing.Point(561, 123)
        Me.LblSisaHutang.Name = "LblSisaHutang"
        Me.LblSisaHutang.Size = New System.Drawing.Size(100, 18)
        Me.LblSisaHutang.TabIndex = 103
        Me.LblSisaHutang.Text = "0"
        Me.LblSisaHutang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(442, 124)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(111, 16)
        Me.Label3.TabIndex = 102
        Me.Label3.Text = "Sisa Hutang : Rp."
        '
        'TxtSisaHutang
        '
        Me.TxtSisaHutang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSisaHutang.Location = New System.Drawing.Point(694, 121)
        Me.TxtSisaHutang.Name = "TxtSisaHutang"
        Me.TxtSisaHutang.Size = New System.Drawing.Size(58, 22)
        Me.TxtSisaHutang.TabIndex = 101
        Me.TxtSisaHutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtSisaHutang.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(45, 69)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(55, 16)
        Me.Label4.TabIndex = 104
        Me.Label4.Text = "Nomor :"
        '
        'LblNomorBayar
        '
        Me.LblNomorBayar.AutoSize = True
        Me.LblNomorBayar.BackColor = System.Drawing.Color.Transparent
        Me.LblNomorBayar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNomorBayar.Location = New System.Drawing.Point(106, 69)
        Me.LblNomorBayar.Name = "LblNomorBayar"
        Me.LblNomorBayar.Size = New System.Drawing.Size(40, 16)
        Me.LblNomorBayar.TabIndex = 105
        Me.LblNomorBayar.Text = "Kode"
        '
        'TxtRekening
        '
        Me.TxtRekening.AutoSize = True
        Me.TxtRekening.BackColor = System.Drawing.Color.Transparent
        Me.TxtRekening.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtRekening.Location = New System.Drawing.Point(367, 124)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.Size = New System.Drawing.Size(40, 16)
        Me.TxtRekening.TabIndex = 107
        Me.TxtRekening.Text = "Kode"
        Me.TxtRekening.Visible = False
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Location = New System.Drawing.Point(106, 120)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(255, 24)
        Me.CmbRekening.TabIndex = 106
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(5, 124)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(95, 16)
        Me.Label2.TabIndex = 108
        Me.Label2.Text = "Sumber dana :"
        '
        'PanelView
        '
        Me.PanelView.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.PanelView.BackColor = System.Drawing.Color.Orange
        Me.PanelView.Controls.Add(Me.LblDetail)
        Me.PanelView.Controls.Add(Me.Button2)
        Me.PanelView.Controls.Add(Me.DgvDetail)
        Me.PanelView.Location = New System.Drawing.Point(191, 189)
        Me.PanelView.Name = "PanelView"
        Me.PanelView.Size = New System.Drawing.Size(753, 361)
        Me.PanelView.TabIndex = 109
        '
        'LblDetail
        '
        Me.LblDetail.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LblDetail.AutoSize = True
        Me.LblDetail.BackColor = System.Drawing.Color.Transparent
        Me.LblDetail.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDetail.Location = New System.Drawing.Point(3, 6)
        Me.LblDetail.Name = "LblDetail"
        Me.LblDetail.Size = New System.Drawing.Size(104, 16)
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
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvDetail.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.DgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvDetail.DefaultCellStyle = DataGridViewCellStyle3
        Me.DgvDetail.Location = New System.Drawing.Point(3, 27)
        Me.DgvDetail.Name = "DgvDetail"
        Me.DgvDetail.RowHeadersVisible = False
        Me.DgvDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvDetail.Size = New System.Drawing.Size(747, 331)
        Me.DgvDetail.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(35, 43)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 16)
        Me.Label1.TabIndex = 110
        Me.Label1.Text = "Tanggal :"
        '
        'FormBayarHutang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1135, 562)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PanelView)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtRekening)
        Me.Controls.Add(Me.CmbRekening)
        Me.Controls.Add(Me.LblNomorBayar)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.LblSisaHutang)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtSisaHutang)
        Me.Controls.Add(Me.LblTotalHutang)
        Me.Controls.Add(Me.LblTotalBayar)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.TxtTotalHutang)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.TxtTotalBayar)
        Me.Controls.Add(Me.BtnKeluar)
        Me.Controls.Add(Me.BtnBayar)
        Me.Controls.Add(Me.DtpTanggal)
        Me.Controls.Add(Me.LblKodeSupliyer)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.CmbSupliyer)
        Me.Controls.Add(Me.DgvData)
        Me.Controls.Add(Me.LblUtama)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormBayarHutang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormBayarHutang"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelView.ResumeLayout(False)
        Me.PanelView.PerformLayout()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblUtama As Label
    Friend WithEvents DgvData As DataGridView
    Friend WithEvents Label12 As Label
    Friend WithEvents CmbSupliyer As ComboBox
    Friend WithEvents LblKodeSupliyer As Label
    Friend WithEvents DtpTanggal As DateTimePicker
    Friend WithEvents BtnKeluar As Button
    Friend WithEvents BtnBayar As Button
    Friend WithEvents LblTotalHutang As Label
    Friend WithEvents LblTotalBayar As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents TxtTotalHutang As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents TxtTotalBayar As TextBox
    Friend WithEvents LblSisaHutang As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtSisaHutang As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents LblNomorBayar As Label
    Friend WithEvents TxtRekening As System.Windows.Forms.Label
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents PanelView As System.Windows.Forms.Panel
    Friend WithEvents DgvDetail As System.Windows.Forms.DataGridView
    Friend WithEvents LblDetail As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Check As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ID_PEMBELIAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ID_SUPPLIER As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NAMA_SUPLIYER As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TGL_BELI As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents GRAND_TOTAL_BELI As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PEMBAYARAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents RETUR As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TAGIHAN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents JATUH_TEMPO As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Bayar As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents View As System.Windows.Forms.DataGridViewButtonColumn
End Class
