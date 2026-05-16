<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapMutasiKeuangan
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapMutasiKeuangan))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.BtnHitung = New System.Windows.Forms.Button()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.TxtBulanThn = New System.Windows.Forms.TextBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.CmbKasir = New System.Windows.Forms.ComboBox()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtTotalPenjualan = New System.Windows.Forms.TextBox()
        Me.TxtTotalReturBeli = New System.Windows.Forms.TextBox()
        Me.TxtTotalPembelian = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtSaldoAkhir = New System.Windows.Forms.TextBox()
        Me.TxtSaldoAwal = New System.Windows.Forms.TextBox()
        Me.TxtSaldoHAriIni = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TxtTotalBayarPiutang = New System.Windows.Forms.TextBox()
        Me.TxtTotalReturJual = New System.Windows.Forms.TextBox()
        Me.TxtTotalBayarHutang = New System.Windows.Forms.TextBox()
        Me.TxtTotalHariIni = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalPemasukan = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalPengeluaran = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalBiaya = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalPR = New System.Windows.Forms.TextBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalPRK = New System.Windows.Forms.TextBox()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.BtnPrint = New System.Windows.Forms.Button()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.TxtNotaJurnalPRK = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalPR = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalBiaya = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalPengeluaran = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalPemasukan = New System.Windows.Forms.TextBox()
        Me.TxtNotaHariIni = New System.Windows.Forms.TextBox()
        Me.TxtNotaBAyarHutang = New System.Windows.Forms.TextBox()
        Me.TxtNotaPenjualan = New System.Windows.Forms.TextBox()
        Me.TxtNotaReturJual = New System.Windows.Forms.TextBox()
        Me.TxtNotaPembelian = New System.Windows.Forms.TextBox()
        Me.TxtNotaBayarPiutang = New System.Windows.Forms.TextBox()
        Me.TxtNotaReturBeli = New System.Windows.Forms.TextBox()
        Me.TxtSaldoDilaci = New System.Windows.Forms.TextBox()
        Me.TxtSetorbos = New System.Windows.Forms.TextBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnSettingPrinter = New System.Windows.Forms.Button()
        Me.CmbProsesCetak = New System.Windows.Forms.ComboBox()
        Me.TxtTypeAkun = New System.Windows.Forms.TextBox()
        Me.CmbPilihCetak = New System.Windows.Forms.ComboBox()
        Me.TxtRekening = New System.Windows.Forms.TextBox()
        Me.BtnBeli = New System.Windows.Forms.Button()
        Me.BtnJual = New System.Windows.Forms.Button()
        Me.BtnReturJual = New System.Windows.Forms.Button()
        Me.BtnReturBeli = New System.Windows.Forms.Button()
        Me.BtnJurnalKeluar = New System.Windows.Forms.Button()
        Me.BtnJurnalMasuk = New System.Windows.Forms.Button()
        Me.BtnPiutang = New System.Windows.Forms.Button()
        Me.BtnHutang = New System.Windows.Forms.Button()
        Me.BtnJurnalPindahKeluar = New System.Windows.Forms.Button()
        Me.BtnJurnalPindahMasuk = New System.Windows.Forms.Button()
        Me.BtnJurnalBiaya = New System.Windows.Forms.Button()
        Me.PanelView = New System.Windows.Forms.Panel()
        Me.LblView = New System.Windows.Forms.Label()
        Me.BtnHide = New System.Windows.Forms.Button()
        Me.DGVView = New System.Windows.Forms.DataGridView()
        Me.BtnSetorBos = New System.Windows.Forms.Button()
        Me.BtnPinjamPelanggan = New System.Windows.Forms.Button()
        Me.BtnPinamSupplier = New System.Windows.Forms.Button()
        Me.BtnGajiKaryawan = New System.Windows.Forms.Button()
        Me.BtnBayarBon = New System.Windows.Forms.Button()
        Me.BtnBonKaryawan = New System.Windows.Forms.Button()
        Me.TxtNotaJurnalPinjamPelanggan = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalPinjamSupplier = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalGaji = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalBayarBon = New System.Windows.Forms.TextBox()
        Me.TxtNotaJurnalBonKaryawan = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalPinjamPelanggan = New System.Windows.Forms.TextBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalPinjamSupplier = New System.Windows.Forms.TextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalGaji = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalBayarBon = New System.Windows.Forms.TextBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.TxtTotalJurnalBonKaryawan = New System.Windows.Forms.TextBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.BtnJuranStorBos = New System.Windows.Forms.Button()
        Me.TxtNotaSetorBos = New System.Windows.Forms.TextBox()
        Me.Panel1.SuspendLayout()
        Me.PanelView.SuspendLayout()
        CType(Me.DGVView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(294, 82)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(68, 16)
        Me.Label8.TabIndex = 157
        Me.Label8.Text = "Rekening"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(323, 53)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(39, 16)
        Me.Label4.TabIndex = 157
        Me.Label4.Text = "Kasir"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.BackColor = System.Drawing.Color.Gold
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label10.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(0, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(1178, 41)
        Me.Label10.TabIndex = 124
        Me.Label10.Text = "LAPORAN MASUK KELUAR KEUANGAN PADA KAS DAN BANK"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnHitung
        '
        Me.BtnHitung.AutoSize = True
        Me.BtnHitung.BackColor = System.Drawing.Color.White
        Me.BtnHitung.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnHitung.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnHitung.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnHitung.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnHitung.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHitung.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHitung.ForeColor = System.Drawing.Color.Black
        Me.BtnHitung.Image = CType(resources.GetObject("BtnHitung.Image"), System.Drawing.Image)
        Me.BtnHitung.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHitung.Location = New System.Drawing.Point(581, 68)
        Me.BtnHitung.Name = "BtnHitung"
        Me.BtnHitung.Size = New System.Drawing.Size(111, 33)
        Me.BtnHitung.TabIndex = 147
        Me.BtnHitung.Text = "Hitung (F8)"
        Me.BtnHitung.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHitung.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnHitung.UseVisualStyleBackColor = False
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbBulan.Location = New System.Drawing.Point(17, 80)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(63, 20)
        Me.CbBulan.TabIndex = 137
        Me.CbBulan.Text = "Bulan"
        Me.CbBulan.UseVisualStyleBackColor = True
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(17, 51)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(80, 20)
        Me.CbTanggal.TabIndex = 136
        Me.CbTanggal.Text = "Tanggal"
        Me.CbTanggal.UseVisualStyleBackColor = True
        '
        'TxtBulanThn
        '
        Me.TxtBulanThn.BackColor = System.Drawing.SystemColors.Window
        Me.TxtBulanThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBulanThn.Location = New System.Drawing.Point(224, 50)
        Me.TxtBulanThn.Name = "TxtBulanThn"
        Me.TxtBulanThn.ReadOnly = True
        Me.TxtBulanThn.Size = New System.Drawing.Size(43, 23)
        Me.TxtBulanThn.TabIndex = 133
        Me.TxtBulanThn.Visible = False
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(203, 78)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(64, 24)
        Me.CmbThn.TabIndex = 132
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(101, 78)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(101, 24)
        Me.CmbBln.TabIndex = 131
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CustomFormat = "dd-MM-yyyy"
        Me.DtpTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(101, 50)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(117, 23)
        Me.DtpTanggal.TabIndex = 130
        '
        'CmbKasir
        '
        Me.CmbKasir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKasir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbKasir.FormattingEnabled = True
        Me.CmbKasir.Items.AddRange(New Object() {"Semua"})
        Me.CmbKasir.Location = New System.Drawing.Point(366, 49)
        Me.CmbKasir.Name = "CmbKasir"
        Me.CmbKasir.Size = New System.Drawing.Size(166, 24)
        Me.CmbKasir.TabIndex = 127
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Location = New System.Drawing.Point(366, 78)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(166, 24)
        Me.CmbRekening.TabIndex = 127
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 177)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(274, 26)
        Me.Label3.TabIndex = 156
        Me.Label3.Text = "Penjualan :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 202)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(274, 26)
        Me.Label2.TabIndex = 155
        Me.Label2.Text = "Retur Pembelian :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 152)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(274, 26)
        Me.Label1.TabIndex = 154
        Me.Label1.Text = "Pembelian :"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtTotalPenjualan
        '
        Me.TxtTotalPenjualan.BackColor = System.Drawing.Color.White
        Me.TxtTotalPenjualan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalPenjualan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalPenjualan.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalPenjualan.Location = New System.Drawing.Point(373, 177)
        Me.TxtTotalPenjualan.Name = "TxtTotalPenjualan"
        Me.TxtTotalPenjualan.ReadOnly = True
        Me.TxtTotalPenjualan.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalPenjualan.TabIndex = 153
        Me.TxtTotalPenjualan.Text = "0"
        Me.TxtTotalPenjualan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTotalReturBeli
        '
        Me.TxtTotalReturBeli.BackColor = System.Drawing.Color.White
        Me.TxtTotalReturBeli.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalReturBeli.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalReturBeli.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalReturBeli.Location = New System.Drawing.Point(373, 202)
        Me.TxtTotalReturBeli.Name = "TxtTotalReturBeli"
        Me.TxtTotalReturBeli.ReadOnly = True
        Me.TxtTotalReturBeli.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalReturBeli.TabIndex = 152
        Me.TxtTotalReturBeli.Text = "0"
        Me.TxtTotalReturBeli.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTotalPembelian
        '
        Me.TxtTotalPembelian.BackColor = System.Drawing.Color.White
        Me.TxtTotalPembelian.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalPembelian.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalPembelian.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalPembelian.Location = New System.Drawing.Point(373, 152)
        Me.TxtTotalPembelian.Name = "TxtTotalPembelian"
        Me.TxtTotalPembelian.ReadOnly = True
        Me.TxtTotalPembelian.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalPembelian.TabIndex = 135
        Me.TxtTotalPembelian.Text = "0"
        Me.TxtTotalPembelian.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(706, 308)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 18)
        Me.Label5.TabIndex = 155
        Me.Label5.Text = "Saldo Akhir :"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(694, 281)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(112, 18)
        Me.Label6.TabIndex = 156
        Me.Label6.Text = "Saldo Hari ini :"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(707, 254)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(99, 18)
        Me.Label7.TabIndex = 154
        Me.Label7.Text = "Saldo Awal :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtSaldoAkhir
        '
        Me.TxtSaldoAkhir.BackColor = System.Drawing.Color.White
        Me.TxtSaldoAkhir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSaldoAkhir.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSaldoAkhir.ForeColor = System.Drawing.Color.Black
        Me.TxtSaldoAkhir.Location = New System.Drawing.Point(812, 304)
        Me.TxtSaldoAkhir.Name = "TxtSaldoAkhir"
        Me.TxtSaldoAkhir.ReadOnly = True
        Me.TxtSaldoAkhir.Size = New System.Drawing.Size(195, 26)
        Me.TxtSaldoAkhir.TabIndex = 152
        Me.TxtSaldoAkhir.Text = "0"
        Me.TxtSaldoAkhir.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtSaldoAwal
        '
        Me.TxtSaldoAwal.BackColor = System.Drawing.Color.White
        Me.TxtSaldoAwal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSaldoAwal.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSaldoAwal.ForeColor = System.Drawing.Color.Black
        Me.TxtSaldoAwal.Location = New System.Drawing.Point(812, 250)
        Me.TxtSaldoAwal.Name = "TxtSaldoAwal"
        Me.TxtSaldoAwal.ReadOnly = True
        Me.TxtSaldoAwal.Size = New System.Drawing.Size(195, 26)
        Me.TxtSaldoAwal.TabIndex = 135
        Me.TxtSaldoAwal.Text = "0"
        Me.TxtSaldoAwal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtSaldoHAriIni
        '
        Me.TxtSaldoHAriIni.BackColor = System.Drawing.Color.White
        Me.TxtSaldoHAriIni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSaldoHAriIni.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSaldoHAriIni.ForeColor = System.Drawing.Color.Black
        Me.TxtSaldoHAriIni.Location = New System.Drawing.Point(812, 277)
        Me.TxtSaldoHAriIni.Name = "TxtSaldoHAriIni"
        Me.TxtSaldoHAriIni.ReadOnly = True
        Me.TxtSaldoHAriIni.Size = New System.Drawing.Size(195, 26)
        Me.TxtSaldoHAriIni.TabIndex = 153
        Me.TxtSaldoHAriIni.Text = "0"
        Me.TxtSaldoHAriIni.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label9
        '
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(12, 277)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(274, 26)
        Me.Label9.TabIndex = 155
        Me.Label9.Text = "Bayar Piutang :"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label11
        '
        Me.Label11.BackColor = System.Drawing.Color.Transparent
        Me.Label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(12, 252)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(274, 26)
        Me.Label11.TabIndex = 156
        Me.Label11.Text = "Bayar Hutang :"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label12
        '
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(12, 227)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(274, 26)
        Me.Label12.TabIndex = 154
        Me.Label12.Text = "Retur Penjualan :"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtTotalBayarPiutang
        '
        Me.TxtTotalBayarPiutang.BackColor = System.Drawing.Color.White
        Me.TxtTotalBayarPiutang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalBayarPiutang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalBayarPiutang.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalBayarPiutang.Location = New System.Drawing.Point(373, 277)
        Me.TxtTotalBayarPiutang.Name = "TxtTotalBayarPiutang"
        Me.TxtTotalBayarPiutang.ReadOnly = True
        Me.TxtTotalBayarPiutang.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalBayarPiutang.TabIndex = 152
        Me.TxtTotalBayarPiutang.Text = "0"
        Me.TxtTotalBayarPiutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTotalReturJual
        '
        Me.TxtTotalReturJual.BackColor = System.Drawing.Color.White
        Me.TxtTotalReturJual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalReturJual.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalReturJual.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalReturJual.Location = New System.Drawing.Point(373, 227)
        Me.TxtTotalReturJual.Name = "TxtTotalReturJual"
        Me.TxtTotalReturJual.ReadOnly = True
        Me.TxtTotalReturJual.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalReturJual.TabIndex = 135
        Me.TxtTotalReturJual.Text = "0"
        Me.TxtTotalReturJual.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTotalBayarHutang
        '
        Me.TxtTotalBayarHutang.BackColor = System.Drawing.Color.White
        Me.TxtTotalBayarHutang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalBayarHutang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalBayarHutang.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalBayarHutang.Location = New System.Drawing.Point(373, 252)
        Me.TxtTotalBayarHutang.Name = "TxtTotalBayarHutang"
        Me.TxtTotalBayarHutang.ReadOnly = True
        Me.TxtTotalBayarHutang.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalBayarHutang.TabIndex = 153
        Me.TxtTotalBayarHutang.Text = "0"
        Me.TxtTotalBayarHutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTotalHariIni
        '
        Me.TxtTotalHariIni.BackColor = System.Drawing.Color.White
        Me.TxtTotalHariIni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalHariIni.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalHariIni.ForeColor = System.Drawing.Color.Red
        Me.TxtTotalHariIni.Location = New System.Drawing.Point(373, 553)
        Me.TxtTotalHariIni.Name = "TxtTotalHariIni"
        Me.TxtTotalHariIni.ReadOnly = True
        Me.TxtTotalHariIni.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalHariIni.TabIndex = 157
        Me.TxtTotalHariIni.Text = "0"
        Me.TxtTotalHariIni.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label13
        '
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.ForeColor = System.Drawing.Color.Red
        Me.Label13.Location = New System.Drawing.Point(12, 553)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(301, 26)
        Me.Label13.TabIndex = 158
        Me.Label13.Text = "Pendapatan hari ini :"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label15
        '
        Me.Label15.BackColor = System.Drawing.Color.Transparent
        Me.Label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(373, 126)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(155, 26)
        Me.Label15.TabIndex = 160
        Me.Label15.Text = "Nominal"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label16
        '
        Me.Label16.BackColor = System.Drawing.Color.Transparent
        Me.Label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(12, 126)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(301, 26)
        Me.Label16.TabIndex = 161
        Me.Label16.Text = "Jenis Transaksi"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label18
        '
        Me.Label18.BackColor = System.Drawing.Color.Transparent
        Me.Label18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.ForeColor = System.Drawing.Color.Navy
        Me.Label18.Location = New System.Drawing.Point(285, 227)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(28, 26)
        Me.Label18.TabIndex = 162
        Me.Label18.Text = "-"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label19
        '
        Me.Label19.BackColor = System.Drawing.Color.Transparent
        Me.Label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label19.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.ForeColor = System.Drawing.Color.Navy
        Me.Label19.Location = New System.Drawing.Point(285, 152)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(28, 26)
        Me.Label19.TabIndex = 163
        Me.Label19.Text = "-"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label20
        '
        Me.Label20.BackColor = System.Drawing.Color.Transparent
        Me.Label20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label20.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.ForeColor = System.Drawing.Color.Navy
        Me.Label20.Location = New System.Drawing.Point(285, 252)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(28, 26)
        Me.Label20.TabIndex = 167
        Me.Label20.Text = "-"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label21
        '
        Me.Label21.BackColor = System.Drawing.Color.Transparent
        Me.Label21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label21.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.ForeColor = System.Drawing.Color.Navy
        Me.Label21.Location = New System.Drawing.Point(285, 277)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(28, 26)
        Me.Label21.TabIndex = 164
        Me.Label21.Text = "+"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label22
        '
        Me.Label22.BackColor = System.Drawing.Color.Transparent
        Me.Label22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label22.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.ForeColor = System.Drawing.Color.Navy
        Me.Label22.Location = New System.Drawing.Point(285, 177)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(28, 26)
        Me.Label22.TabIndex = 166
        Me.Label22.Text = "+"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label23
        '
        Me.Label23.BackColor = System.Drawing.Color.Transparent
        Me.Label23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label23.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.ForeColor = System.Drawing.Color.Navy
        Me.Label23.Location = New System.Drawing.Point(285, 202)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(28, 26)
        Me.Label23.TabIndex = 165
        Me.Label23.Text = "-"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label17
        '
        Me.Label17.BackColor = System.Drawing.Color.Transparent
        Me.Label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.ForeColor = System.Drawing.Color.Navy
        Me.Label17.Location = New System.Drawing.Point(285, 302)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(28, 26)
        Me.Label17.TabIndex = 170
        Me.Label17.Text = "+"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalPemasukan
        '
        Me.TxtTotalJurnalPemasukan.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalPemasukan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalPemasukan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalPemasukan.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalPemasukan.Location = New System.Drawing.Point(373, 302)
        Me.TxtTotalJurnalPemasukan.Name = "TxtTotalJurnalPemasukan"
        Me.TxtTotalJurnalPemasukan.ReadOnly = True
        Me.TxtTotalJurnalPemasukan.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalPemasukan.TabIndex = 168
        Me.TxtTotalJurnalPemasukan.Text = "0"
        Me.TxtTotalJurnalPemasukan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label24
        '
        Me.Label24.BackColor = System.Drawing.Color.Transparent
        Me.Label24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label24.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.Location = New System.Drawing.Point(12, 302)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(274, 26)
        Me.Label24.TabIndex = 169
        Me.Label24.Text = "Jurnal Pemasukan :"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label25
        '
        Me.Label25.BackColor = System.Drawing.Color.Transparent
        Me.Label25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label25.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.ForeColor = System.Drawing.Color.Navy
        Me.Label25.Location = New System.Drawing.Point(285, 327)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(28, 26)
        Me.Label25.TabIndex = 173
        Me.Label25.Text = "-"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalPengeluaran
        '
        Me.TxtTotalJurnalPengeluaran.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalPengeluaran.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalPengeluaran.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalPengeluaran.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalPengeluaran.Location = New System.Drawing.Point(373, 327)
        Me.TxtTotalJurnalPengeluaran.Name = "TxtTotalJurnalPengeluaran"
        Me.TxtTotalJurnalPengeluaran.ReadOnly = True
        Me.TxtTotalJurnalPengeluaran.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalPengeluaran.TabIndex = 171
        Me.TxtTotalJurnalPengeluaran.Text = "0"
        Me.TxtTotalJurnalPengeluaran.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label26
        '
        Me.Label26.BackColor = System.Drawing.Color.Transparent
        Me.Label26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label26.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(12, 327)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(274, 26)
        Me.Label26.TabIndex = 172
        Me.Label26.Text = "Jurnal Pengeluaran :"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label27
        '
        Me.Label27.BackColor = System.Drawing.Color.Transparent
        Me.Label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label27.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label27.ForeColor = System.Drawing.Color.Navy
        Me.Label27.Location = New System.Drawing.Point(285, 352)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(28, 26)
        Me.Label27.TabIndex = 176
        Me.Label27.Text = "-"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalBiaya
        '
        Me.TxtTotalJurnalBiaya.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalBiaya.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalBiaya.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalBiaya.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalBiaya.Location = New System.Drawing.Point(373, 352)
        Me.TxtTotalJurnalBiaya.Name = "TxtTotalJurnalBiaya"
        Me.TxtTotalJurnalBiaya.ReadOnly = True
        Me.TxtTotalJurnalBiaya.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalBiaya.TabIndex = 174
        Me.TxtTotalJurnalBiaya.Text = "0"
        Me.TxtTotalJurnalBiaya.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label28
        '
        Me.Label28.BackColor = System.Drawing.Color.Transparent
        Me.Label28.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label28.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label28.Location = New System.Drawing.Point(12, 352)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(274, 26)
        Me.Label28.TabIndex = 175
        Me.Label28.Text = "Jurnal Biaya :"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label29
        '
        Me.Label29.BackColor = System.Drawing.Color.Transparent
        Me.Label29.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label29.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label29.ForeColor = System.Drawing.Color.Navy
        Me.Label29.Location = New System.Drawing.Point(285, 377)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(28, 26)
        Me.Label29.TabIndex = 179
        Me.Label29.Text = "+"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalPR
        '
        Me.TxtTotalJurnalPR.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalPR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalPR.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalPR.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalPR.Location = New System.Drawing.Point(373, 377)
        Me.TxtTotalJurnalPR.Name = "TxtTotalJurnalPR"
        Me.TxtTotalJurnalPR.ReadOnly = True
        Me.TxtTotalJurnalPR.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalPR.TabIndex = 177
        Me.TxtTotalJurnalPR.Text = "0"
        Me.TxtTotalJurnalPR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label30
        '
        Me.Label30.BackColor = System.Drawing.Color.Transparent
        Me.Label30.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label30.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(12, 377)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(274, 26)
        Me.Label30.TabIndex = 178
        Me.Label30.Text = "Jurnal Pindah Rekening  :"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label31
        '
        Me.Label31.BackColor = System.Drawing.Color.Transparent
        Me.Label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label31.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label31.Location = New System.Drawing.Point(12, 402)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(274, 26)
        Me.Label31.TabIndex = 178
        Me.Label31.Text = "Jurnal Pindah Rekening  :"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtTotalJurnalPRK
        '
        Me.TxtTotalJurnalPRK.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalPRK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalPRK.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalPRK.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalPRK.Location = New System.Drawing.Point(373, 402)
        Me.TxtTotalJurnalPRK.Name = "TxtTotalJurnalPRK"
        Me.TxtTotalJurnalPRK.ReadOnly = True
        Me.TxtTotalJurnalPRK.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalPRK.TabIndex = 177
        Me.TxtTotalJurnalPRK.Text = "0"
        Me.TxtTotalJurnalPRK.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label32
        '
        Me.Label32.BackColor = System.Drawing.Color.Transparent
        Me.Label32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label32.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label32.ForeColor = System.Drawing.Color.Navy
        Me.Label32.Location = New System.Drawing.Point(285, 402)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(28, 26)
        Me.Label32.TabIndex = 179
        Me.Label32.Text = "-"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label33
        '
        Me.Label33.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label33.AutoSize = True
        Me.Label33.BackColor = System.Drawing.Color.Transparent
        Me.Label33.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label33.ForeColor = System.Drawing.Color.Navy
        Me.Label33.Location = New System.Drawing.Point(160, 582)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(153, 48)
        Me.Label33.TabIndex = 180
        Me.Label33.Text = "Keterangan :" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(+) = Saldo bertambah" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(-) = Saldo Berkurang"
        Me.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnPrint
        '
        Me.BtnPrint.AutoSize = True
        Me.BtnPrint.BackColor = System.Drawing.Color.White
        Me.BtnPrint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPrint.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPrint.ForeColor = System.Drawing.Color.Black
        Me.BtnPrint.Image = CType(resources.GetObject("BtnPrint.Image"), System.Drawing.Image)
        Me.BtnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPrint.Location = New System.Drawing.Point(812, 346)
        Me.BtnPrint.Name = "BtnPrint"
        Me.BtnPrint.Size = New System.Drawing.Size(102, 33)
        Me.BtnPrint.TabIndex = 181
        Me.BtnPrint.Text = "Cetak (F4)"
        Me.BtnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPrint.UseVisualStyleBackColor = False
        '
        'Label34
        '
        Me.Label34.BackColor = System.Drawing.Color.Transparent
        Me.Label34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label34.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label34.Location = New System.Drawing.Point(312, 126)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(62, 26)
        Me.Label34.TabIndex = 183
        Me.Label34.Text = "Trx"
        Me.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtNotaJurnalPRK
        '
        Me.TxtNotaJurnalPRK.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalPRK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalPRK.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalPRK.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalPRK.Location = New System.Drawing.Point(312, 402)
        Me.TxtNotaJurnalPRK.Name = "TxtNotaJurnalPRK"
        Me.TxtNotaJurnalPRK.ReadOnly = True
        Me.TxtNotaJurnalPRK.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalPRK.TabIndex = 194
        Me.TxtNotaJurnalPRK.Text = "0"
        Me.TxtNotaJurnalPRK.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalPR
        '
        Me.TxtNotaJurnalPR.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalPR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalPR.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalPR.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalPR.Location = New System.Drawing.Point(312, 377)
        Me.TxtNotaJurnalPR.Name = "TxtNotaJurnalPR"
        Me.TxtNotaJurnalPR.ReadOnly = True
        Me.TxtNotaJurnalPR.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalPR.TabIndex = 195
        Me.TxtNotaJurnalPR.Text = "0"
        Me.TxtNotaJurnalPR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalBiaya
        '
        Me.TxtNotaJurnalBiaya.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalBiaya.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalBiaya.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalBiaya.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalBiaya.Location = New System.Drawing.Point(312, 352)
        Me.TxtNotaJurnalBiaya.Name = "TxtNotaJurnalBiaya"
        Me.TxtNotaJurnalBiaya.ReadOnly = True
        Me.TxtNotaJurnalBiaya.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalBiaya.TabIndex = 193
        Me.TxtNotaJurnalBiaya.Text = "0"
        Me.TxtNotaJurnalBiaya.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalPengeluaran
        '
        Me.TxtNotaJurnalPengeluaran.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalPengeluaran.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalPengeluaran.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalPengeluaran.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalPengeluaran.Location = New System.Drawing.Point(312, 327)
        Me.TxtNotaJurnalPengeluaran.Name = "TxtNotaJurnalPengeluaran"
        Me.TxtNotaJurnalPengeluaran.ReadOnly = True
        Me.TxtNotaJurnalPengeluaran.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalPengeluaran.TabIndex = 192
        Me.TxtNotaJurnalPengeluaran.Text = "0"
        Me.TxtNotaJurnalPengeluaran.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalPemasukan
        '
        Me.TxtNotaJurnalPemasukan.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalPemasukan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalPemasukan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalPemasukan.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalPemasukan.Location = New System.Drawing.Point(312, 302)
        Me.TxtNotaJurnalPemasukan.Name = "TxtNotaJurnalPemasukan"
        Me.TxtNotaJurnalPemasukan.ReadOnly = True
        Me.TxtNotaJurnalPemasukan.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalPemasukan.TabIndex = 191
        Me.TxtNotaJurnalPemasukan.Text = "0"
        Me.TxtNotaJurnalPemasukan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaHariIni
        '
        Me.TxtNotaHariIni.BackColor = System.Drawing.Color.White
        Me.TxtNotaHariIni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaHariIni.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaHariIni.ForeColor = System.Drawing.Color.Red
        Me.TxtNotaHariIni.Location = New System.Drawing.Point(312, 553)
        Me.TxtNotaHariIni.Name = "TxtNotaHariIni"
        Me.TxtNotaHariIni.ReadOnly = True
        Me.TxtNotaHariIni.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaHariIni.TabIndex = 190
        Me.TxtNotaHariIni.Text = "0"
        Me.TxtNotaHariIni.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaBAyarHutang
        '
        Me.TxtNotaBAyarHutang.BackColor = System.Drawing.Color.White
        Me.TxtNotaBAyarHutang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaBAyarHutang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaBAyarHutang.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaBAyarHutang.Location = New System.Drawing.Point(312, 252)
        Me.TxtNotaBAyarHutang.Name = "TxtNotaBAyarHutang"
        Me.TxtNotaBAyarHutang.ReadOnly = True
        Me.TxtNotaBAyarHutang.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaBAyarHutang.TabIndex = 188
        Me.TxtNotaBAyarHutang.Text = "0"
        Me.TxtNotaBAyarHutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaPenjualan
        '
        Me.TxtNotaPenjualan.BackColor = System.Drawing.Color.White
        Me.TxtNotaPenjualan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaPenjualan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaPenjualan.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaPenjualan.Location = New System.Drawing.Point(312, 177)
        Me.TxtNotaPenjualan.Name = "TxtNotaPenjualan"
        Me.TxtNotaPenjualan.ReadOnly = True
        Me.TxtNotaPenjualan.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaPenjualan.TabIndex = 189
        Me.TxtNotaPenjualan.Text = "0"
        Me.TxtNotaPenjualan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaReturJual
        '
        Me.TxtNotaReturJual.BackColor = System.Drawing.Color.White
        Me.TxtNotaReturJual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaReturJual.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaReturJual.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaReturJual.Location = New System.Drawing.Point(312, 227)
        Me.TxtNotaReturJual.Name = "TxtNotaReturJual"
        Me.TxtNotaReturJual.ReadOnly = True
        Me.TxtNotaReturJual.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaReturJual.TabIndex = 184
        Me.TxtNotaReturJual.Text = "0"
        Me.TxtNotaReturJual.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaPembelian
        '
        Me.TxtNotaPembelian.BackColor = System.Drawing.Color.White
        Me.TxtNotaPembelian.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaPembelian.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaPembelian.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaPembelian.Location = New System.Drawing.Point(312, 152)
        Me.TxtNotaPembelian.Name = "TxtNotaPembelian"
        Me.TxtNotaPembelian.ReadOnly = True
        Me.TxtNotaPembelian.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaPembelian.TabIndex = 185
        Me.TxtNotaPembelian.Text = "0"
        Me.TxtNotaPembelian.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaBayarPiutang
        '
        Me.TxtNotaBayarPiutang.BackColor = System.Drawing.Color.White
        Me.TxtNotaBayarPiutang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaBayarPiutang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaBayarPiutang.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaBayarPiutang.Location = New System.Drawing.Point(312, 277)
        Me.TxtNotaBayarPiutang.Name = "TxtNotaBayarPiutang"
        Me.TxtNotaBayarPiutang.ReadOnly = True
        Me.TxtNotaBayarPiutang.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaBayarPiutang.TabIndex = 187
        Me.TxtNotaBayarPiutang.Text = "0"
        Me.TxtNotaBayarPiutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaReturBeli
        '
        Me.TxtNotaReturBeli.BackColor = System.Drawing.Color.White
        Me.TxtNotaReturBeli.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaReturBeli.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaReturBeli.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaReturBeli.Location = New System.Drawing.Point(312, 202)
        Me.TxtNotaReturBeli.Name = "TxtNotaReturBeli"
        Me.TxtNotaReturBeli.ReadOnly = True
        Me.TxtNotaReturBeli.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaReturBeli.TabIndex = 186
        Me.TxtNotaReturBeli.Text = "0"
        Me.TxtNotaReturBeli.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtSaldoDilaci
        '
        Me.TxtSaldoDilaci.BackColor = System.Drawing.Color.White
        Me.TxtSaldoDilaci.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSaldoDilaci.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSaldoDilaci.ForeColor = System.Drawing.Color.Black
        Me.TxtSaldoDilaci.Location = New System.Drawing.Point(812, 179)
        Me.TxtSaldoDilaci.Name = "TxtSaldoDilaci"
        Me.TxtSaldoDilaci.ReadOnly = True
        Me.TxtSaldoDilaci.Size = New System.Drawing.Size(195, 26)
        Me.TxtSaldoDilaci.TabIndex = 197
        Me.TxtSaldoDilaci.Text = "0"
        Me.TxtSaldoDilaci.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtSetorbos
        '
        Me.TxtSetorbos.BackColor = System.Drawing.Color.White
        Me.TxtSetorbos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSetorbos.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSetorbos.ForeColor = System.Drawing.Color.Black
        Me.TxtSetorbos.Location = New System.Drawing.Point(861, 152)
        Me.TxtSetorbos.Name = "TxtSetorbos"
        Me.TxtSetorbos.ReadOnly = True
        Me.TxtSetorbos.Size = New System.Drawing.Size(146, 26)
        Me.TxtSetorbos.TabIndex = 196
        Me.TxtSetorbos.Text = "0"
        Me.TxtSetorbos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.BackColor = System.Drawing.Color.Transparent
        Me.Label35.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label35.Location = New System.Drawing.Point(622, 155)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(186, 18)
        Me.Label35.TabIndex = 198
        Me.Label35.Text = "Setoran tunai ke Bos (-) :"
        Me.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.BackColor = System.Drawing.Color.Transparent
        Me.Label36.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label36.Location = New System.Drawing.Point(662, 184)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(146, 18)
        Me.Label36.TabIndex = 199
        Me.Label36.Text = "Saldo Tunai dilaci :"
        Me.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.BtnSettingPrinter)
        Me.Panel1.Controls.Add(Me.CmbProsesCetak)
        Me.Panel1.Controls.Add(Me.TxtTypeAkun)
        Me.Panel1.Controls.Add(Me.CmbPilihCetak)
        Me.Panel1.Controls.Add(Me.TxtRekening)
        Me.Panel1.Controls.Add(Me.Label10)
        Me.Panel1.Controls.Add(Me.CmbRekening)
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Controls.Add(Me.CmbKasir)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.DtpTanggal)
        Me.Panel1.Controls.Add(Me.CmbBln)
        Me.Panel1.Controls.Add(Me.BtnHitung)
        Me.Panel1.Controls.Add(Me.CmbThn)
        Me.Panel1.Controls.Add(Me.CbBulan)
        Me.Panel1.Controls.Add(Me.TxtBulanThn)
        Me.Panel1.Controls.Add(Me.CbTanggal)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1180, 110)
        Me.Panel1.TabIndex = 200
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
        Me.BtnSettingPrinter.Location = New System.Drawing.Point(1093, 68)
        Me.BtnSettingPrinter.Name = "BtnSettingPrinter"
        Me.BtnSettingPrinter.Size = New System.Drawing.Size(82, 29)
        Me.BtnSettingPrinter.TabIndex = 216
        Me.BtnSettingPrinter.Text = "Printer"
        Me.BtnSettingPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSettingPrinter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSettingPrinter.UseVisualStyleBackColor = False
        '
        'CmbProsesCetak
        '
        Me.CmbProsesCetak.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CmbProsesCetak.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbProsesCetak.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbProsesCetak.FormattingEnabled = True
        Me.CmbProsesCetak.Items.AddRange(New Object() {"LANGSUNG CETAK", "TANYA PILIH PRINTER"})
        Me.CmbProsesCetak.Location = New System.Drawing.Point(938, 79)
        Me.CmbProsesCetak.Name = "CmbProsesCetak"
        Me.CmbProsesCetak.Size = New System.Drawing.Size(149, 21)
        Me.CmbProsesCetak.TabIndex = 215
        '
        'TxtTypeAkun
        '
        Me.TxtTypeAkun.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtTypeAkun.BackColor = System.Drawing.Color.White
        Me.TxtTypeAkun.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTypeAkun.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeAkun.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeAkun.Location = New System.Drawing.Point(740, 75)
        Me.TxtTypeAkun.Name = "TxtTypeAkun"
        Me.TxtTypeAkun.ReadOnly = True
        Me.TxtTypeAkun.Size = New System.Drawing.Size(177, 26)
        Me.TxtTypeAkun.TabIndex = 161
        Me.TxtTypeAkun.Text = "TypeAkun"
        Me.TxtTypeAkun.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtTypeAkun.Visible = False
        '
        'CmbPilihCetak
        '
        Me.CmbPilihCetak.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CmbPilihCetak.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPilihCetak.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbPilihCetak.FormattingEnabled = True
        Me.CmbPilihCetak.Items.AddRange(New Object() {"IYA", "SELALU TANYA", "TAMPILKAN DI MONITOR"})
        Me.CmbPilihCetak.Location = New System.Drawing.Point(938, 51)
        Me.CmbPilihCetak.Name = "CmbPilihCetak"
        Me.CmbPilihCetak.Size = New System.Drawing.Size(149, 21)
        Me.CmbPilihCetak.TabIndex = 214
        '
        'TxtRekening
        '
        Me.TxtRekening.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtRekening.BackColor = System.Drawing.Color.White
        Me.TxtRekening.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtRekening.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtRekening.ForeColor = System.Drawing.Color.Black
        Me.TxtRekening.Location = New System.Drawing.Point(740, 44)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.ReadOnly = True
        Me.TxtRekening.Size = New System.Drawing.Size(177, 26)
        Me.TxtRekening.TabIndex = 160
        Me.TxtRekening.Text = "01.01.111"
        Me.TxtRekening.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtRekening.Visible = False
        '
        'BtnBeli
        '
        Me.BtnBeli.AutoSize = True
        Me.BtnBeli.BackColor = System.Drawing.Color.White
        Me.BtnBeli.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBeli.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnBeli.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnBeli.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnBeli.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBeli.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBeli.ForeColor = System.Drawing.Color.Black
        Me.BtnBeli.Image = CType(resources.GetObject("BtnBeli.Image"), System.Drawing.Image)
        Me.BtnBeli.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBeli.Location = New System.Drawing.Point(534, 153)
        Me.BtnBeli.Name = "BtnBeli"
        Me.BtnBeli.Size = New System.Drawing.Size(69, 28)
        Me.BtnBeli.TabIndex = 201
        Me.BtnBeli.Tag = "Lihat Pembelian"
        Me.BtnBeli.Text = "View"
        Me.BtnBeli.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBeli.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBeli.UseVisualStyleBackColor = False
        '
        'BtnJual
        '
        Me.BtnJual.AutoSize = True
        Me.BtnJual.BackColor = System.Drawing.Color.White
        Me.BtnJual.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJual.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJual.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJual.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJual.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJual.ForeColor = System.Drawing.Color.Black
        Me.BtnJual.Image = CType(resources.GetObject("BtnJual.Image"), System.Drawing.Image)
        Me.BtnJual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJual.Location = New System.Drawing.Point(534, 178)
        Me.BtnJual.Name = "BtnJual"
        Me.BtnJual.Size = New System.Drawing.Size(69, 28)
        Me.BtnJual.TabIndex = 202
        Me.BtnJual.Text = "View"
        Me.BtnJual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJual.UseVisualStyleBackColor = False
        '
        'BtnReturJual
        '
        Me.BtnReturJual.AutoSize = True
        Me.BtnReturJual.BackColor = System.Drawing.Color.White
        Me.BtnReturJual.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnReturJual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnReturJual.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnReturJual.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnReturJual.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnReturJual.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnReturJual.ForeColor = System.Drawing.Color.Black
        Me.BtnReturJual.Image = CType(resources.GetObject("BtnReturJual.Image"), System.Drawing.Image)
        Me.BtnReturJual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnReturJual.Location = New System.Drawing.Point(534, 228)
        Me.BtnReturJual.Name = "BtnReturJual"
        Me.BtnReturJual.Size = New System.Drawing.Size(69, 28)
        Me.BtnReturJual.TabIndex = 204
        Me.BtnReturJual.Text = "View"
        Me.BtnReturJual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnReturJual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnReturJual.UseVisualStyleBackColor = False
        '
        'BtnReturBeli
        '
        Me.BtnReturBeli.AutoSize = True
        Me.BtnReturBeli.BackColor = System.Drawing.Color.White
        Me.BtnReturBeli.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnReturBeli.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnReturBeli.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnReturBeli.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnReturBeli.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnReturBeli.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnReturBeli.ForeColor = System.Drawing.Color.Black
        Me.BtnReturBeli.Image = CType(resources.GetObject("BtnReturBeli.Image"), System.Drawing.Image)
        Me.BtnReturBeli.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnReturBeli.Location = New System.Drawing.Point(534, 203)
        Me.BtnReturBeli.Name = "BtnReturBeli"
        Me.BtnReturBeli.Size = New System.Drawing.Size(69, 28)
        Me.BtnReturBeli.TabIndex = 203
        Me.BtnReturBeli.Text = "View"
        Me.BtnReturBeli.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnReturBeli.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnReturBeli.UseVisualStyleBackColor = False
        '
        'BtnJurnalKeluar
        '
        Me.BtnJurnalKeluar.AutoSize = True
        Me.BtnJurnalKeluar.BackColor = System.Drawing.Color.White
        Me.BtnJurnalKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJurnalKeluar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJurnalKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJurnalKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJurnalKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJurnalKeluar.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJurnalKeluar.ForeColor = System.Drawing.Color.Black
        Me.BtnJurnalKeluar.Image = CType(resources.GetObject("BtnJurnalKeluar.Image"), System.Drawing.Image)
        Me.BtnJurnalKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalKeluar.Location = New System.Drawing.Point(534, 328)
        Me.BtnJurnalKeluar.Name = "BtnJurnalKeluar"
        Me.BtnJurnalKeluar.Size = New System.Drawing.Size(69, 28)
        Me.BtnJurnalKeluar.TabIndex = 208
        Me.BtnJurnalKeluar.Text = "View"
        Me.BtnJurnalKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJurnalKeluar.UseVisualStyleBackColor = False
        '
        'BtnJurnalMasuk
        '
        Me.BtnJurnalMasuk.AutoSize = True
        Me.BtnJurnalMasuk.BackColor = System.Drawing.Color.White
        Me.BtnJurnalMasuk.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJurnalMasuk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJurnalMasuk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJurnalMasuk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJurnalMasuk.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJurnalMasuk.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJurnalMasuk.ForeColor = System.Drawing.Color.Black
        Me.BtnJurnalMasuk.Image = CType(resources.GetObject("BtnJurnalMasuk.Image"), System.Drawing.Image)
        Me.BtnJurnalMasuk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalMasuk.Location = New System.Drawing.Point(534, 303)
        Me.BtnJurnalMasuk.Name = "BtnJurnalMasuk"
        Me.BtnJurnalMasuk.Size = New System.Drawing.Size(69, 28)
        Me.BtnJurnalMasuk.TabIndex = 207
        Me.BtnJurnalMasuk.Text = "View"
        Me.BtnJurnalMasuk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalMasuk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJurnalMasuk.UseVisualStyleBackColor = False
        '
        'BtnPiutang
        '
        Me.BtnPiutang.AutoSize = True
        Me.BtnPiutang.BackColor = System.Drawing.Color.White
        Me.BtnPiutang.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPiutang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPiutang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPiutang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPiutang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPiutang.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPiutang.ForeColor = System.Drawing.Color.Black
        Me.BtnPiutang.Image = CType(resources.GetObject("BtnPiutang.Image"), System.Drawing.Image)
        Me.BtnPiutang.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPiutang.Location = New System.Drawing.Point(534, 278)
        Me.BtnPiutang.Name = "BtnPiutang"
        Me.BtnPiutang.Size = New System.Drawing.Size(69, 28)
        Me.BtnPiutang.TabIndex = 206
        Me.BtnPiutang.Text = "View"
        Me.BtnPiutang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPiutang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPiutang.UseVisualStyleBackColor = False
        '
        'BtnHutang
        '
        Me.BtnHutang.AutoSize = True
        Me.BtnHutang.BackColor = System.Drawing.Color.White
        Me.BtnHutang.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnHutang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnHutang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnHutang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnHutang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHutang.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHutang.ForeColor = System.Drawing.Color.Black
        Me.BtnHutang.Image = CType(resources.GetObject("BtnHutang.Image"), System.Drawing.Image)
        Me.BtnHutang.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHutang.Location = New System.Drawing.Point(534, 253)
        Me.BtnHutang.Name = "BtnHutang"
        Me.BtnHutang.Size = New System.Drawing.Size(69, 28)
        Me.BtnHutang.TabIndex = 205
        Me.BtnHutang.Text = "View"
        Me.BtnHutang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHutang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnHutang.UseVisualStyleBackColor = False
        '
        'BtnJurnalPindahKeluar
        '
        Me.BtnJurnalPindahKeluar.AutoSize = True
        Me.BtnJurnalPindahKeluar.BackColor = System.Drawing.Color.White
        Me.BtnJurnalPindahKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJurnalPindahKeluar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJurnalPindahKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJurnalPindahKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJurnalPindahKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJurnalPindahKeluar.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJurnalPindahKeluar.ForeColor = System.Drawing.Color.Black
        Me.BtnJurnalPindahKeluar.Image = CType(resources.GetObject("BtnJurnalPindahKeluar.Image"), System.Drawing.Image)
        Me.BtnJurnalPindahKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalPindahKeluar.Location = New System.Drawing.Point(534, 403)
        Me.BtnJurnalPindahKeluar.Name = "BtnJurnalPindahKeluar"
        Me.BtnJurnalPindahKeluar.Size = New System.Drawing.Size(69, 28)
        Me.BtnJurnalPindahKeluar.TabIndex = 211
        Me.BtnJurnalPindahKeluar.Text = "View"
        Me.BtnJurnalPindahKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalPindahKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJurnalPindahKeluar.UseVisualStyleBackColor = False
        '
        'BtnJurnalPindahMasuk
        '
        Me.BtnJurnalPindahMasuk.AutoSize = True
        Me.BtnJurnalPindahMasuk.BackColor = System.Drawing.Color.White
        Me.BtnJurnalPindahMasuk.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJurnalPindahMasuk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJurnalPindahMasuk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJurnalPindahMasuk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJurnalPindahMasuk.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJurnalPindahMasuk.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJurnalPindahMasuk.ForeColor = System.Drawing.Color.Black
        Me.BtnJurnalPindahMasuk.Image = CType(resources.GetObject("BtnJurnalPindahMasuk.Image"), System.Drawing.Image)
        Me.BtnJurnalPindahMasuk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalPindahMasuk.Location = New System.Drawing.Point(534, 378)
        Me.BtnJurnalPindahMasuk.Name = "BtnJurnalPindahMasuk"
        Me.BtnJurnalPindahMasuk.Size = New System.Drawing.Size(69, 28)
        Me.BtnJurnalPindahMasuk.TabIndex = 210
        Me.BtnJurnalPindahMasuk.Text = "View"
        Me.BtnJurnalPindahMasuk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalPindahMasuk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJurnalPindahMasuk.UseVisualStyleBackColor = False
        '
        'BtnJurnalBiaya
        '
        Me.BtnJurnalBiaya.AutoSize = True
        Me.BtnJurnalBiaya.BackColor = System.Drawing.Color.White
        Me.BtnJurnalBiaya.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJurnalBiaya.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJurnalBiaya.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJurnalBiaya.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJurnalBiaya.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJurnalBiaya.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJurnalBiaya.ForeColor = System.Drawing.Color.Black
        Me.BtnJurnalBiaya.Image = CType(resources.GetObject("BtnJurnalBiaya.Image"), System.Drawing.Image)
        Me.BtnJurnalBiaya.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalBiaya.Location = New System.Drawing.Point(534, 353)
        Me.BtnJurnalBiaya.Name = "BtnJurnalBiaya"
        Me.BtnJurnalBiaya.Size = New System.Drawing.Size(69, 28)
        Me.BtnJurnalBiaya.TabIndex = 209
        Me.BtnJurnalBiaya.Text = "View"
        Me.BtnJurnalBiaya.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJurnalBiaya.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJurnalBiaya.UseVisualStyleBackColor = False
        '
        'PanelView
        '
        Me.PanelView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelView.Controls.Add(Me.LblView)
        Me.PanelView.Controls.Add(Me.BtnHide)
        Me.PanelView.Controls.Add(Me.DGVView)
        Me.PanelView.Location = New System.Drawing.Point(697, 441)
        Me.PanelView.Name = "PanelView"
        Me.PanelView.Size = New System.Drawing.Size(969, 463)
        Me.PanelView.TabIndex = 212
        '
        'LblView
        '
        Me.LblView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblView.BackColor = System.Drawing.Color.Transparent
        Me.LblView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblView.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblView.Location = New System.Drawing.Point(3, 3)
        Me.LblView.Name = "LblView"
        Me.LblView.Size = New System.Drawing.Size(922, 26)
        Me.LblView.TabIndex = 155
        Me.LblView.Text = "Pembelian :"
        Me.LblView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnHide
        '
        Me.BtnHide.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnHide.BackColor = System.Drawing.Color.White
        Me.BtnHide.FlatAppearance.BorderSize = 0
        Me.BtnHide.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnHide.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHide.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHide.ForeColor = System.Drawing.Color.White
        Me.BtnHide.Image = CType(resources.GetObject("BtnHide.Image"), System.Drawing.Image)
        Me.BtnHide.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHide.Location = New System.Drawing.Point(932, 3)
        Me.BtnHide.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnHide.Name = "BtnHide"
        Me.BtnHide.Size = New System.Drawing.Size(31, 28)
        Me.BtnHide.TabIndex = 65
        Me.BtnHide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHide.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnHide.UseVisualStyleBackColor = False
        '
        'DGVView
        '
        Me.DGVView.AllowUserToAddRows = False
        Me.DGVView.AllowUserToDeleteRows = False
        Me.DGVView.AllowUserToOrderColumns = True
        Me.DGVView.AllowUserToResizeColumns = False
        Me.DGVView.AllowUserToResizeRows = False
        Me.DGVView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DGVView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVView.DefaultCellStyle = DataGridViewCellStyle1
        Me.DGVView.Location = New System.Drawing.Point(3, 31)
        Me.DGVView.Margin = New System.Windows.Forms.Padding(3, 7, 3, 7)
        Me.DGVView.Name = "DGVView"
        Me.DGVView.ReadOnly = True
        Me.DGVView.RowHeadersVisible = False
        Me.DGVView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVView.Size = New System.Drawing.Size(961, 423)
        Me.DGVView.TabIndex = 64
        '
        'BtnSetorBos
        '
        Me.BtnSetorBos.AutoSize = True
        Me.BtnSetorBos.BackColor = System.Drawing.Color.White
        Me.BtnSetorBos.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSetorBos.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnSetorBos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSetorBos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSetorBos.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSetorBos.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSetorBos.ForeColor = System.Drawing.Color.Black
        Me.BtnSetorBos.Image = CType(resources.GetObject("BtnSetorBos.Image"), System.Drawing.Image)
        Me.BtnSetorBos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetorBos.Location = New System.Drawing.Point(1013, 154)
        Me.BtnSetorBos.Name = "BtnSetorBos"
        Me.BtnSetorBos.Size = New System.Drawing.Size(69, 27)
        Me.BtnSetorBos.TabIndex = 213
        Me.BtnSetorBos.Tag = "Lihat Pembelian"
        Me.BtnSetorBos.Text = "View"
        Me.BtnSetorBos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetorBos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSetorBos.UseVisualStyleBackColor = False
        '
        'BtnPinjamPelanggan
        '
        Me.BtnPinjamPelanggan.AutoSize = True
        Me.BtnPinjamPelanggan.BackColor = System.Drawing.Color.White
        Me.BtnPinjamPelanggan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPinjamPelanggan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPinjamPelanggan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPinjamPelanggan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPinjamPelanggan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPinjamPelanggan.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPinjamPelanggan.ForeColor = System.Drawing.Color.Black
        Me.BtnPinjamPelanggan.Image = CType(resources.GetObject("BtnPinjamPelanggan.Image"), System.Drawing.Image)
        Me.BtnPinjamPelanggan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinjamPelanggan.Location = New System.Drawing.Point(534, 526)
        Me.BtnPinjamPelanggan.Name = "BtnPinjamPelanggan"
        Me.BtnPinjamPelanggan.Size = New System.Drawing.Size(69, 28)
        Me.BtnPinjamPelanggan.TabIndex = 238
        Me.BtnPinjamPelanggan.Text = "View"
        Me.BtnPinjamPelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinjamPelanggan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPinjamPelanggan.UseVisualStyleBackColor = False
        '
        'BtnPinamSupplier
        '
        Me.BtnPinamSupplier.AutoSize = True
        Me.BtnPinamSupplier.BackColor = System.Drawing.Color.White
        Me.BtnPinamSupplier.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPinamSupplier.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPinamSupplier.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPinamSupplier.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPinamSupplier.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPinamSupplier.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPinamSupplier.ForeColor = System.Drawing.Color.Black
        Me.BtnPinamSupplier.Image = CType(resources.GetObject("BtnPinamSupplier.Image"), System.Drawing.Image)
        Me.BtnPinamSupplier.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinamSupplier.Location = New System.Drawing.Point(534, 501)
        Me.BtnPinamSupplier.Name = "BtnPinamSupplier"
        Me.BtnPinamSupplier.Size = New System.Drawing.Size(69, 28)
        Me.BtnPinamSupplier.TabIndex = 237
        Me.BtnPinamSupplier.Text = "View"
        Me.BtnPinamSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinamSupplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPinamSupplier.UseVisualStyleBackColor = False
        '
        'BtnGajiKaryawan
        '
        Me.BtnGajiKaryawan.AutoSize = True
        Me.BtnGajiKaryawan.BackColor = System.Drawing.Color.White
        Me.BtnGajiKaryawan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnGajiKaryawan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnGajiKaryawan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnGajiKaryawan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnGajiKaryawan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnGajiKaryawan.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnGajiKaryawan.ForeColor = System.Drawing.Color.Black
        Me.BtnGajiKaryawan.Image = CType(resources.GetObject("BtnGajiKaryawan.Image"), System.Drawing.Image)
        Me.BtnGajiKaryawan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnGajiKaryawan.Location = New System.Drawing.Point(534, 476)
        Me.BtnGajiKaryawan.Name = "BtnGajiKaryawan"
        Me.BtnGajiKaryawan.Size = New System.Drawing.Size(69, 28)
        Me.BtnGajiKaryawan.TabIndex = 236
        Me.BtnGajiKaryawan.Text = "View"
        Me.BtnGajiKaryawan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnGajiKaryawan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnGajiKaryawan.UseVisualStyleBackColor = False
        '
        'BtnBayarBon
        '
        Me.BtnBayarBon.AutoSize = True
        Me.BtnBayarBon.BackColor = System.Drawing.Color.White
        Me.BtnBayarBon.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBayarBon.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnBayarBon.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnBayarBon.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnBayarBon.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBayarBon.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBayarBon.ForeColor = System.Drawing.Color.Black
        Me.BtnBayarBon.Image = CType(resources.GetObject("BtnBayarBon.Image"), System.Drawing.Image)
        Me.BtnBayarBon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayarBon.Location = New System.Drawing.Point(534, 451)
        Me.BtnBayarBon.Name = "BtnBayarBon"
        Me.BtnBayarBon.Size = New System.Drawing.Size(69, 28)
        Me.BtnBayarBon.TabIndex = 235
        Me.BtnBayarBon.Text = "View"
        Me.BtnBayarBon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayarBon.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBayarBon.UseVisualStyleBackColor = False
        '
        'BtnBonKaryawan
        '
        Me.BtnBonKaryawan.AutoSize = True
        Me.BtnBonKaryawan.BackColor = System.Drawing.Color.White
        Me.BtnBonKaryawan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBonKaryawan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnBonKaryawan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnBonKaryawan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnBonKaryawan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBonKaryawan.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBonKaryawan.ForeColor = System.Drawing.Color.Black
        Me.BtnBonKaryawan.Image = CType(resources.GetObject("BtnBonKaryawan.Image"), System.Drawing.Image)
        Me.BtnBonKaryawan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBonKaryawan.Location = New System.Drawing.Point(534, 426)
        Me.BtnBonKaryawan.Name = "BtnBonKaryawan"
        Me.BtnBonKaryawan.Size = New System.Drawing.Size(69, 28)
        Me.BtnBonKaryawan.TabIndex = 234
        Me.BtnBonKaryawan.Text = "View"
        Me.BtnBonKaryawan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBonKaryawan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBonKaryawan.UseVisualStyleBackColor = False
        '
        'TxtNotaJurnalPinjamPelanggan
        '
        Me.TxtNotaJurnalPinjamPelanggan.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalPinjamPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalPinjamPelanggan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalPinjamPelanggan.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalPinjamPelanggan.Location = New System.Drawing.Point(312, 525)
        Me.TxtNotaJurnalPinjamPelanggan.Name = "TxtNotaJurnalPinjamPelanggan"
        Me.TxtNotaJurnalPinjamPelanggan.ReadOnly = True
        Me.TxtNotaJurnalPinjamPelanggan.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalPinjamPelanggan.TabIndex = 232
        Me.TxtNotaJurnalPinjamPelanggan.Text = "0"
        Me.TxtNotaJurnalPinjamPelanggan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalPinjamSupplier
        '
        Me.TxtNotaJurnalPinjamSupplier.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalPinjamSupplier.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalPinjamSupplier.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalPinjamSupplier.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalPinjamSupplier.Location = New System.Drawing.Point(312, 500)
        Me.TxtNotaJurnalPinjamSupplier.Name = "TxtNotaJurnalPinjamSupplier"
        Me.TxtNotaJurnalPinjamSupplier.ReadOnly = True
        Me.TxtNotaJurnalPinjamSupplier.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalPinjamSupplier.TabIndex = 233
        Me.TxtNotaJurnalPinjamSupplier.Text = "0"
        Me.TxtNotaJurnalPinjamSupplier.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalGaji
        '
        Me.TxtNotaJurnalGaji.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalGaji.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalGaji.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalGaji.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalGaji.Location = New System.Drawing.Point(312, 475)
        Me.TxtNotaJurnalGaji.Name = "TxtNotaJurnalGaji"
        Me.TxtNotaJurnalGaji.ReadOnly = True
        Me.TxtNotaJurnalGaji.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalGaji.TabIndex = 231
        Me.TxtNotaJurnalGaji.Text = "0"
        Me.TxtNotaJurnalGaji.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalBayarBon
        '
        Me.TxtNotaJurnalBayarBon.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalBayarBon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalBayarBon.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalBayarBon.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalBayarBon.Location = New System.Drawing.Point(312, 450)
        Me.TxtNotaJurnalBayarBon.Name = "TxtNotaJurnalBayarBon"
        Me.TxtNotaJurnalBayarBon.ReadOnly = True
        Me.TxtNotaJurnalBayarBon.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalBayarBon.TabIndex = 230
        Me.TxtNotaJurnalBayarBon.Text = "0"
        Me.TxtNotaJurnalBayarBon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtNotaJurnalBonKaryawan
        '
        Me.TxtNotaJurnalBonKaryawan.BackColor = System.Drawing.Color.White
        Me.TxtNotaJurnalBonKaryawan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaJurnalBonKaryawan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaJurnalBonKaryawan.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaJurnalBonKaryawan.Location = New System.Drawing.Point(312, 425)
        Me.TxtNotaJurnalBonKaryawan.Name = "TxtNotaJurnalBonKaryawan"
        Me.TxtNotaJurnalBonKaryawan.ReadOnly = True
        Me.TxtNotaJurnalBonKaryawan.Size = New System.Drawing.Size(62, 26)
        Me.TxtNotaJurnalBonKaryawan.TabIndex = 229
        Me.TxtNotaJurnalBonKaryawan.Text = "0"
        Me.TxtNotaJurnalBonKaryawan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label14
        '
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.ForeColor = System.Drawing.Color.Navy
        Me.Label14.Location = New System.Drawing.Point(285, 525)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(28, 26)
        Me.Label14.TabIndex = 228
        Me.Label14.Text = "-"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label37
        '
        Me.Label37.BackColor = System.Drawing.Color.Transparent
        Me.Label37.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label37.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label37.ForeColor = System.Drawing.Color.Navy
        Me.Label37.Location = New System.Drawing.Point(285, 500)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(28, 26)
        Me.Label37.TabIndex = 227
        Me.Label37.Text = "+"
        Me.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalPinjamPelanggan
        '
        Me.TxtTotalJurnalPinjamPelanggan.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalPinjamPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalPinjamPelanggan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalPinjamPelanggan.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalPinjamPelanggan.Location = New System.Drawing.Point(373, 525)
        Me.TxtTotalJurnalPinjamPelanggan.Name = "TxtTotalJurnalPinjamPelanggan"
        Me.TxtTotalJurnalPinjamPelanggan.ReadOnly = True
        Me.TxtTotalJurnalPinjamPelanggan.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalPinjamPelanggan.TabIndex = 224
        Me.TxtTotalJurnalPinjamPelanggan.Text = "0"
        Me.TxtTotalJurnalPinjamPelanggan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label38
        '
        Me.Label38.BackColor = System.Drawing.Color.Transparent
        Me.Label38.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label38.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label38.Location = New System.Drawing.Point(12, 525)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(274, 26)
        Me.Label38.TabIndex = 225
        Me.Label38.Text = "Pinjaman Pelanggan :"
        Me.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtTotalJurnalPinjamSupplier
        '
        Me.TxtTotalJurnalPinjamSupplier.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalPinjamSupplier.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalPinjamSupplier.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalPinjamSupplier.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalPinjamSupplier.Location = New System.Drawing.Point(373, 500)
        Me.TxtTotalJurnalPinjamSupplier.Name = "TxtTotalJurnalPinjamSupplier"
        Me.TxtTotalJurnalPinjamSupplier.ReadOnly = True
        Me.TxtTotalJurnalPinjamSupplier.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalPinjamSupplier.TabIndex = 223
        Me.TxtTotalJurnalPinjamSupplier.Text = "0"
        Me.TxtTotalJurnalPinjamSupplier.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label39
        '
        Me.Label39.BackColor = System.Drawing.Color.Transparent
        Me.Label39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label39.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label39.Location = New System.Drawing.Point(12, 500)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(274, 26)
        Me.Label39.TabIndex = 226
        Me.Label39.Text = "Pinjaman Supplier :"
        Me.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label40
        '
        Me.Label40.BackColor = System.Drawing.Color.Transparent
        Me.Label40.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label40.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label40.ForeColor = System.Drawing.Color.Navy
        Me.Label40.Location = New System.Drawing.Point(285, 475)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(28, 26)
        Me.Label40.TabIndex = 222
        Me.Label40.Text = "-"
        Me.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalGaji
        '
        Me.TxtTotalJurnalGaji.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalGaji.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalGaji.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalGaji.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalGaji.Location = New System.Drawing.Point(373, 475)
        Me.TxtTotalJurnalGaji.Name = "TxtTotalJurnalGaji"
        Me.TxtTotalJurnalGaji.ReadOnly = True
        Me.TxtTotalJurnalGaji.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalGaji.TabIndex = 220
        Me.TxtTotalJurnalGaji.Text = "0"
        Me.TxtTotalJurnalGaji.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label41
        '
        Me.Label41.BackColor = System.Drawing.Color.Transparent
        Me.Label41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label41.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label41.Location = New System.Drawing.Point(12, 475)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(274, 26)
        Me.Label41.TabIndex = 221
        Me.Label41.Text = "Gaji Karyawan :"
        Me.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label42
        '
        Me.Label42.BackColor = System.Drawing.Color.Transparent
        Me.Label42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label42.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label42.ForeColor = System.Drawing.Color.Navy
        Me.Label42.Location = New System.Drawing.Point(285, 450)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(28, 26)
        Me.Label42.TabIndex = 219
        Me.Label42.Text = "+"
        Me.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalBayarBon
        '
        Me.TxtTotalJurnalBayarBon.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalBayarBon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalBayarBon.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalBayarBon.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalBayarBon.Location = New System.Drawing.Point(373, 450)
        Me.TxtTotalJurnalBayarBon.Name = "TxtTotalJurnalBayarBon"
        Me.TxtTotalJurnalBayarBon.ReadOnly = True
        Me.TxtTotalJurnalBayarBon.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalBayarBon.TabIndex = 217
        Me.TxtTotalJurnalBayarBon.Text = "0"
        Me.TxtTotalJurnalBayarBon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label43
        '
        Me.Label43.BackColor = System.Drawing.Color.Transparent
        Me.Label43.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label43.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label43.Location = New System.Drawing.Point(12, 450)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(274, 26)
        Me.Label43.TabIndex = 218
        Me.Label43.Text = "Bayar Bon :"
        Me.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label44
        '
        Me.Label44.BackColor = System.Drawing.Color.Transparent
        Me.Label44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label44.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label44.ForeColor = System.Drawing.Color.Navy
        Me.Label44.Location = New System.Drawing.Point(285, 425)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(28, 26)
        Me.Label44.TabIndex = 216
        Me.Label44.Text = "-"
        Me.Label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtTotalJurnalBonKaryawan
        '
        Me.TxtTotalJurnalBonKaryawan.BackColor = System.Drawing.Color.White
        Me.TxtTotalJurnalBonKaryawan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotalJurnalBonKaryawan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalJurnalBonKaryawan.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalJurnalBonKaryawan.Location = New System.Drawing.Point(373, 425)
        Me.TxtTotalJurnalBonKaryawan.Name = "TxtTotalJurnalBonKaryawan"
        Me.TxtTotalJurnalBonKaryawan.ReadOnly = True
        Me.TxtTotalJurnalBonKaryawan.Size = New System.Drawing.Size(155, 26)
        Me.TxtTotalJurnalBonKaryawan.TabIndex = 214
        Me.TxtTotalJurnalBonKaryawan.Text = "0"
        Me.TxtTotalJurnalBonKaryawan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label45
        '
        Me.Label45.BackColor = System.Drawing.Color.Transparent
        Me.Label45.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label45.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label45.Location = New System.Drawing.Point(12, 425)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(274, 26)
        Me.Label45.TabIndex = 215
        Me.Label45.Text = "Bon Karyawan :"
        Me.Label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BtnJuranStorBos
        '
        Me.BtnJuranStorBos.AutoSize = True
        Me.BtnJuranStorBos.BackColor = System.Drawing.Color.White
        Me.BtnJuranStorBos.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnJuranStorBos.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnJuranStorBos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnJuranStorBos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnJuranStorBos.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnJuranStorBos.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJuranStorBos.ForeColor = System.Drawing.Color.Black
        Me.BtnJuranStorBos.Image = CType(resources.GetObject("BtnJuranStorBos.Image"), System.Drawing.Image)
        Me.BtnJuranStorBos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJuranStorBos.Location = New System.Drawing.Point(812, 113)
        Me.BtnJuranStorBos.Name = "BtnJuranStorBos"
        Me.BtnJuranStorBos.Size = New System.Drawing.Size(168, 33)
        Me.BtnJuranStorBos.TabIndex = 239
        Me.BtnJuranStorBos.Text = "Jurnal Setor Bos (F2)"
        Me.BtnJuranStorBos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnJuranStorBos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnJuranStorBos.UseVisualStyleBackColor = False
        '
        'TxtNotaSetorBos
        '
        Me.TxtNotaSetorBos.BackColor = System.Drawing.Color.White
        Me.TxtNotaSetorBos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNotaSetorBos.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotaSetorBos.ForeColor = System.Drawing.Color.Black
        Me.TxtNotaSetorBos.Location = New System.Drawing.Point(812, 151)
        Me.TxtNotaSetorBos.Name = "TxtNotaSetorBos"
        Me.TxtNotaSetorBos.ReadOnly = True
        Me.TxtNotaSetorBos.Size = New System.Drawing.Size(43, 26)
        Me.TxtNotaSetorBos.TabIndex = 240
        Me.TxtNotaSetorBos.Text = "0"
        Me.TxtNotaSetorBos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FormLapMutasiKeuangan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(1180, 659)
        Me.Controls.Add(Me.TxtNotaSetorBos)
        Me.Controls.Add(Me.BtnJuranStorBos)
        Me.Controls.Add(Me.PanelView)
        Me.Controls.Add(Me.BtnJurnalPindahKeluar)
        Me.Controls.Add(Me.BtnJurnalPindahMasuk)
        Me.Controls.Add(Me.BtnJurnalBiaya)
        Me.Controls.Add(Me.BtnJurnalKeluar)
        Me.Controls.Add(Me.BtnJurnalMasuk)
        Me.Controls.Add(Me.BtnPiutang)
        Me.Controls.Add(Me.BtnHutang)
        Me.Controls.Add(Me.BtnReturJual)
        Me.Controls.Add(Me.BtnReturBeli)
        Me.Controls.Add(Me.BtnJual)
        Me.Controls.Add(Me.BtnBeli)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TxtSaldoDilaci)
        Me.Controls.Add(Me.TxtSetorbos)
        Me.Controls.Add(Me.Label35)
        Me.Controls.Add(Me.Label36)
        Me.Controls.Add(Me.TxtNotaJurnalPRK)
        Me.Controls.Add(Me.TxtNotaJurnalPR)
        Me.Controls.Add(Me.TxtNotaJurnalBiaya)
        Me.Controls.Add(Me.TxtNotaJurnalPengeluaran)
        Me.Controls.Add(Me.TxtNotaJurnalPemasukan)
        Me.Controls.Add(Me.TxtNotaHariIni)
        Me.Controls.Add(Me.TxtNotaBAyarHutang)
        Me.Controls.Add(Me.TxtNotaPenjualan)
        Me.Controls.Add(Me.TxtNotaReturJual)
        Me.Controls.Add(Me.TxtNotaPembelian)
        Me.Controls.Add(Me.TxtNotaBayarPiutang)
        Me.Controls.Add(Me.TxtNotaReturBeli)
        Me.Controls.Add(Me.Label34)
        Me.Controls.Add(Me.BtnPrint)
        Me.Controls.Add(Me.Label33)
        Me.Controls.Add(Me.Label32)
        Me.Controls.Add(Me.Label29)
        Me.Controls.Add(Me.TxtTotalJurnalPRK)
        Me.Controls.Add(Me.Label31)
        Me.Controls.Add(Me.TxtTotalJurnalPR)
        Me.Controls.Add(Me.Label30)
        Me.Controls.Add(Me.Label27)
        Me.Controls.Add(Me.TxtTotalJurnalBiaya)
        Me.Controls.Add(Me.Label28)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.TxtTotalJurnalPengeluaran)
        Me.Controls.Add(Me.Label26)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.TxtTotalJurnalPemasukan)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.TxtTotalHariIni)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.TxtSaldoHAriIni)
        Me.Controls.Add(Me.TxtTotalBayarHutang)
        Me.Controls.Add(Me.TxtTotalPenjualan)
        Me.Controls.Add(Me.TxtSaldoAwal)
        Me.Controls.Add(Me.TxtSaldoAkhir)
        Me.Controls.Add(Me.TxtTotalReturJual)
        Me.Controls.Add(Me.TxtTotalPembelian)
        Me.Controls.Add(Me.TxtTotalBayarPiutang)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.TxtTotalReturBeli)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.BtnSetorBos)
        Me.Controls.Add(Me.BtnPinjamPelanggan)
        Me.Controls.Add(Me.BtnPinamSupplier)
        Me.Controls.Add(Me.BtnGajiKaryawan)
        Me.Controls.Add(Me.BtnBayarBon)
        Me.Controls.Add(Me.BtnBonKaryawan)
        Me.Controls.Add(Me.TxtNotaJurnalPinjamPelanggan)
        Me.Controls.Add(Me.TxtNotaJurnalPinjamSupplier)
        Me.Controls.Add(Me.TxtNotaJurnalGaji)
        Me.Controls.Add(Me.TxtNotaJurnalBayarBon)
        Me.Controls.Add(Me.TxtNotaJurnalBonKaryawan)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label37)
        Me.Controls.Add(Me.TxtTotalJurnalPinjamPelanggan)
        Me.Controls.Add(Me.Label38)
        Me.Controls.Add(Me.TxtTotalJurnalPinjamSupplier)
        Me.Controls.Add(Me.Label39)
        Me.Controls.Add(Me.Label40)
        Me.Controls.Add(Me.TxtTotalJurnalGaji)
        Me.Controls.Add(Me.Label41)
        Me.Controls.Add(Me.Label42)
        Me.Controls.Add(Me.TxtTotalJurnalBayarBon)
        Me.Controls.Add(Me.Label43)
        Me.Controls.Add(Me.Label44)
        Me.Controls.Add(Me.TxtTotalJurnalBonKaryawan)
        Me.Controls.Add(Me.Label45)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapMutasiKeuangan"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Laporan Saldo Kas"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.PanelView.ResumeLayout(False)
        CType(Me.DGVView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents BtnHitung As System.Windows.Forms.Button
    Friend WithEvents CbBulan As System.Windows.Forms.CheckBox
    Friend WithEvents CbTanggal As System.Windows.Forms.CheckBox
    Friend WithEvents TxtBulanThn As System.Windows.Forms.TextBox
    Friend WithEvents CmbThn As System.Windows.Forms.ComboBox
    Friend WithEvents CmbBln As System.Windows.Forms.ComboBox
    Friend WithEvents DtpTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents CmbKasir As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalPenjualan As System.Windows.Forms.TextBox
    Friend WithEvents TxtTotalReturBeli As System.Windows.Forms.TextBox
    Friend WithEvents TxtTotalPembelian As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtSaldoAkhir As System.Windows.Forms.TextBox
    Friend WithEvents TxtSaldoAwal As System.Windows.Forms.TextBox
    Friend WithEvents TxtSaldoHAriIni As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalBayarPiutang As System.Windows.Forms.TextBox
    Friend WithEvents TxtTotalReturJual As System.Windows.Forms.TextBox
    Friend WithEvents TxtTotalBayarHutang As System.Windows.Forms.TextBox
    Friend WithEvents TxtTotalHariIni As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalJurnalPemasukan As System.Windows.Forms.TextBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalJurnalPengeluaran As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalJurnalBiaya As System.Windows.Forms.TextBox
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalJurnalPR As System.Windows.Forms.TextBox
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalJurnalPRK As System.Windows.Forms.TextBox
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents BtnPrint As System.Windows.Forms.Button
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents TxtNotaJurnalPRK As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaJurnalPR As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaJurnalBiaya As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaJurnalPengeluaran As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaJurnalPemasukan As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaHariIni As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaBAyarHutang As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaPenjualan As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaReturJual As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaPembelian As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaBayarPiutang As System.Windows.Forms.TextBox
    Friend WithEvents TxtNotaReturBeli As System.Windows.Forms.TextBox
    Friend WithEvents TxtSaldoDilaci As System.Windows.Forms.TextBox
    Friend WithEvents TxtSetorbos As System.Windows.Forms.TextBox
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents BtnBeli As System.Windows.Forms.Button
    Friend WithEvents BtnJual As System.Windows.Forms.Button
    Friend WithEvents BtnReturJual As System.Windows.Forms.Button
    Friend WithEvents BtnReturBeli As System.Windows.Forms.Button
    Friend WithEvents BtnJurnalKeluar As System.Windows.Forms.Button
    Friend WithEvents BtnJurnalMasuk As System.Windows.Forms.Button
    Friend WithEvents BtnPiutang As System.Windows.Forms.Button
    Friend WithEvents BtnHutang As System.Windows.Forms.Button
    Friend WithEvents BtnJurnalPindahKeluar As System.Windows.Forms.Button
    Friend WithEvents BtnJurnalPindahMasuk As System.Windows.Forms.Button
    Friend WithEvents BtnJurnalBiaya As System.Windows.Forms.Button
    Friend WithEvents PanelView As System.Windows.Forms.Panel
    Friend WithEvents DGVView As System.Windows.Forms.DataGridView
    Friend WithEvents BtnHide As System.Windows.Forms.Button
    Friend WithEvents LblView As System.Windows.Forms.Label
    Friend WithEvents TxtTypeAkun As System.Windows.Forms.TextBox
    Friend WithEvents TxtRekening As System.Windows.Forms.TextBox
    Friend WithEvents BtnSetorBos As System.Windows.Forms.Button
    Friend WithEvents BtnSettingPrinter As Button
    Friend WithEvents CmbProsesCetak As ComboBox
    Friend WithEvents CmbPilihCetak As ComboBox
    Friend WithEvents BtnPinjamPelanggan As Button
    Friend WithEvents BtnPinamSupplier As Button
    Friend WithEvents BtnGajiKaryawan As Button
    Friend WithEvents BtnBayarBon As Button
    Friend WithEvents BtnBonKaryawan As Button
    Friend WithEvents TxtNotaJurnalPinjamPelanggan As TextBox
    Friend WithEvents TxtNotaJurnalPinjamSupplier As TextBox
    Friend WithEvents TxtNotaJurnalGaji As TextBox
    Friend WithEvents TxtNotaJurnalBayarBon As TextBox
    Friend WithEvents TxtNotaJurnalBonKaryawan As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents Label37 As Label
    Friend WithEvents TxtTotalJurnalPinjamPelanggan As TextBox
    Friend WithEvents Label38 As Label
    Friend WithEvents TxtTotalJurnalPinjamSupplier As TextBox
    Friend WithEvents Label39 As Label
    Friend WithEvents Label40 As Label
    Friend WithEvents TxtTotalJurnalGaji As TextBox
    Friend WithEvents Label41 As Label
    Friend WithEvents Label42 As Label
    Friend WithEvents TxtTotalJurnalBayarBon As TextBox
    Friend WithEvents Label43 As Label
    Friend WithEvents Label44 As Label
    Friend WithEvents TxtTotalJurnalBonKaryawan As TextBox
    Friend WithEvents Label45 As Label
    Friend WithEvents BtnJuranStorBos As Button
    Friend WithEvents TxtNotaSetorBos As TextBox
End Class




