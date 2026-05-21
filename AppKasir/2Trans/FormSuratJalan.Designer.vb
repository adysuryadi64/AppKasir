<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormSuratJalan
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormSuratJalan))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelNota = New System.Windows.Forms.Panel()
        Me.BtnSettingPrinter = New System.Windows.Forms.Button()
        Me.LblKodeHelper2 = New System.Windows.Forms.Label()
        Me.LblKodeHelper1 = New System.Windows.Forms.Label()
        Me.LblKodeSupir = New System.Windows.Forms.Label()
        Me.LblKodeArmada = New System.Windows.Forms.Label()
        Me.BtnDaftarBarang = New System.Windows.Forms.Button()
        Me.LblJenisArmada = New System.Windows.Forms.Label()
        Me.CmbHelper2 = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.CmbHelper1 = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.CmbSopir = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CmbArmada = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LblNoNota = New System.Windows.Forms.Label()
        Me.DtpSuratJalan = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnKeluarForm = New System.Windows.Forms.Button()
        Me.LblJenisTrans = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.PanelInput = New System.Windows.Forms.Panel()
        Me.BtnTransfer = New System.Windows.Forms.Button()
        Me.BtnHideDaftar = New System.Windows.Forms.Button()
        Me.DtpPenjualan = New System.Windows.Forms.DateTimePicker()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.DGVPenjualan = New System.Windows.Forms.DataGridView()
        Me.DGVSuratJalan = New System.Windows.Forms.DataGridView()
        Me.PanelInput2 = New System.Windows.Forms.Panel()
        Me.BtnSimpann = New System.Windows.Forms.Button()
        Me.TxtTotalPelanggan = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtTotalRupiah = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.chk = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.ID_PENJUALAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ID_PELANGGAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMA_PELANGGAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ALAMAT_PELANGGAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TGL_TRANSAKSI = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GRAND_TOTAL_STL_PAJAK = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LOKASIBARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SUMBER = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SUMBER_TRANS = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Nota = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Kode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Pelanggan = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Alamat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Tanggal = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Nominal = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Lokasi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnHapus = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.PanelNota.SuspendLayout()
        Me.PanelHeader.SuspendLayout()
        Me.PanelInput.SuspendLayout()
        CType(Me.DGVPenjualan, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVSuratJalan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelInput2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelNota
        '
        Me.PanelNota.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelNota.BackColor = System.Drawing.Color.Wheat
        Me.PanelNota.Controls.Add(Me.BtnSettingPrinter)
        Me.PanelNota.Controls.Add(Me.LblKodeHelper2)
        Me.PanelNota.Controls.Add(Me.LblKodeHelper1)
        Me.PanelNota.Controls.Add(Me.LblKodeSupir)
        Me.PanelNota.Controls.Add(Me.LblKodeArmada)
        Me.PanelNota.Controls.Add(Me.BtnDaftarBarang)
        Me.PanelNota.Controls.Add(Me.LblJenisArmada)
        Me.PanelNota.Controls.Add(Me.CmbHelper2)
        Me.PanelNota.Controls.Add(Me.Label12)
        Me.PanelNota.Controls.Add(Me.CmbHelper1)
        Me.PanelNota.Controls.Add(Me.Label10)
        Me.PanelNota.Controls.Add(Me.CmbSopir)
        Me.PanelNota.Controls.Add(Me.Label6)
        Me.PanelNota.Controls.Add(Me.CmbArmada)
        Me.PanelNota.Controls.Add(Me.Label5)
        Me.PanelNota.Controls.Add(Me.LblNoNota)
        Me.PanelNota.Controls.Add(Me.DtpSuratJalan)
        Me.PanelNota.Controls.Add(Me.Label3)
        Me.PanelNota.Controls.Add(Me.Label2)
        Me.PanelNota.Location = New System.Drawing.Point(0, 33)
        Me.PanelNota.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelNota.Name = "PanelNota"
        Me.PanelNota.Size = New System.Drawing.Size(1144, 84)
        Me.PanelNota.TabIndex = 227
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
        Me.BtnSettingPrinter.Location = New System.Drawing.Point(1059, 7)
        Me.BtnSettingPrinter.Name = "BtnSettingPrinter"
        Me.BtnSettingPrinter.Size = New System.Drawing.Size(82, 29)
        Me.BtnSettingPrinter.TabIndex = 233
        Me.BtnSettingPrinter.Text = "Printer"
        Me.BtnSettingPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSettingPrinter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSettingPrinter.UseVisualStyleBackColor = False
        '
        'LblKodeHelper2
        '
        Me.LblKodeHelper2.AutoSize = True
        Me.LblKodeHelper2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeHelper2.Location = New System.Drawing.Point(485, 60)
        Me.LblKodeHelper2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeHelper2.Name = "LblKodeHelper2"
        Me.LblKodeHelper2.Size = New System.Drawing.Size(40, 16)
        Me.LblKodeHelper2.TabIndex = 267
        Me.LblKodeHelper2.Text = "Kode"
        '
        'LblKodeHelper1
        '
        Me.LblKodeHelper1.AutoSize = True
        Me.LblKodeHelper1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeHelper1.Location = New System.Drawing.Point(485, 34)
        Me.LblKodeHelper1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeHelper1.Name = "LblKodeHelper1"
        Me.LblKodeHelper1.Size = New System.Drawing.Size(40, 16)
        Me.LblKodeHelper1.TabIndex = 266
        Me.LblKodeHelper1.Text = "Kode"
        '
        'LblKodeSupir
        '
        Me.LblKodeSupir.AutoSize = True
        Me.LblKodeSupir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeSupir.Location = New System.Drawing.Point(485, 9)
        Me.LblKodeSupir.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeSupir.Name = "LblKodeSupir"
        Me.LblKodeSupir.Size = New System.Drawing.Size(40, 16)
        Me.LblKodeSupir.TabIndex = 265
        Me.LblKodeSupir.Text = "Kode"
        '
        'LblKodeArmada
        '
        Me.LblKodeArmada.AutoSize = True
        Me.LblKodeArmada.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeArmada.Location = New System.Drawing.Point(24, 34)
        Me.LblKodeArmada.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKodeArmada.Name = "LblKodeArmada"
        Me.LblKodeArmada.Size = New System.Drawing.Size(40, 16)
        Me.LblKodeArmada.TabIndex = 264
        Me.LblKodeArmada.Text = "Kode"
        Me.LblKodeArmada.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblKodeArmada.Visible = False
        '
        'BtnDaftarBarang
        '
        Me.BtnDaftarBarang.AutoSize = True
        Me.BtnDaftarBarang.BackColor = System.Drawing.Color.White
        Me.BtnDaftarBarang.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnDaftarBarang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnDaftarBarang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnDaftarBarang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnDaftarBarang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnDaftarBarang.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnDaftarBarang.ForeColor = System.Drawing.Color.Black
        Me.BtnDaftarBarang.Image = CType(resources.GetObject("BtnDaftarBarang.Image"), System.Drawing.Image)
        Me.BtnDaftarBarang.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDaftarBarang.Location = New System.Drawing.Point(576, 41)
        Me.BtnDaftarBarang.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnDaftarBarang.Name = "BtnDaftarBarang"
        Me.BtnDaftarBarang.Size = New System.Drawing.Size(273, 36)
        Me.BtnDaftarBarang.TabIndex = 5
        Me.BtnDaftarBarang.Text = "Ambil Daftar Penjualan (F2)"
        Me.BtnDaftarBarang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDaftarBarang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnDaftarBarang.UseVisualStyleBackColor = False
        '
        'LblJenisArmada
        '
        Me.LblJenisArmada.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisArmada.Location = New System.Drawing.Point(81, 34)
        Me.LblJenisArmada.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblJenisArmada.Name = "LblJenisArmada"
        Me.LblJenisArmada.Size = New System.Drawing.Size(124, 43)
        Me.LblJenisArmada.TabIndex = 262
        Me.LblJenisArmada.Text = "Jenis"
        '
        'CmbHelper2
        '
        Me.CmbHelper2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbHelper2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbHelper2.FormattingEnabled = True
        Me.CmbHelper2.Location = New System.Drawing.Point(298, 58)
        Me.CmbHelper2.Name = "CmbHelper2"
        Me.CmbHelper2.Size = New System.Drawing.Size(182, 24)
        Me.CmbHelper2.TabIndex = 4
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(222, 60)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(69, 16)
        Me.Label12.TabIndex = 259
        Me.Label12.Text = "Helper 2 :"
        '
        'CmbHelper1
        '
        Me.CmbHelper1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbHelper1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbHelper1.FormattingEnabled = True
        Me.CmbHelper1.Location = New System.Drawing.Point(298, 32)
        Me.CmbHelper1.Name = "CmbHelper1"
        Me.CmbHelper1.Size = New System.Drawing.Size(182, 24)
        Me.CmbHelper1.TabIndex = 3
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(222, 34)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(69, 16)
        Me.Label10.TabIndex = 257
        Me.Label10.Text = "Helper 1 :"
        '
        'CmbSopir
        '
        Me.CmbSopir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSopir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSopir.FormattingEnabled = True
        Me.CmbSopir.Location = New System.Drawing.Point(298, 7)
        Me.CmbSopir.Name = "CmbSopir"
        Me.CmbSopir.Size = New System.Drawing.Size(182, 24)
        Me.CmbSopir.TabIndex = 2
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(243, 9)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(48, 16)
        Me.Label6.TabIndex = 255
        Me.Label6.Text = "Sopir :"
        '
        'CmbArmada
        '
        Me.CmbArmada.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbArmada.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbArmada.FormattingEnabled = True
        Me.CmbArmada.Location = New System.Drawing.Point(81, 7)
        Me.CmbArmada.Name = "CmbArmada"
        Me.CmbArmada.Size = New System.Drawing.Size(124, 24)
        Me.CmbArmada.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(10, 9)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 16)
        Me.Label5.TabIndex = 253
        Me.Label5.Text = "Armada :"
        '
        'LblNoNota
        '
        Me.LblNoNota.AutoSize = True
        Me.LblNoNota.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNoNota.Location = New System.Drawing.Point(989, 9)
        Me.LblNoNota.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblNoNota.Name = "LblNoNota"
        Me.LblNoNota.Size = New System.Drawing.Size(64, 16)
        Me.LblNoNota.TabIndex = 237
        Me.LblNoNota.Text = "No. Nota"
        '
        'DtpSuratJalan
        '
        Me.DtpSuratJalan.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpSuratJalan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpSuratJalan.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpSuratJalan.Location = New System.Drawing.Point(692, 6)
        Me.DtpSuratJalan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpSuratJalan.Name = "DtpSuratJalan"
        Me.DtpSuratJalan.Size = New System.Drawing.Size(176, 23)
        Me.DtpSuratJalan.TabIndex = 228
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(573, 9)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(116, 16)
        Me.Label3.TabIndex = 227
        Me.Label3.Text = "Tgl. Pengiriman :"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(876, 9)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(110, 16)
        Me.Label2.TabIndex = 226
        Me.Label2.Text = "No Surat Jalan :"
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.Chocolate
        Me.PanelHeader.Controls.Add(Me.BtnKeluarForm)
        Me.PanelHeader.Controls.Add(Me.LblJenisTrans)
        Me.PanelHeader.Controls.Add(Me.Label8)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1144, 34)
        Me.PanelHeader.TabIndex = 228
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
        Me.BtnKeluarForm.Location = New System.Drawing.Point(1031, 1)
        Me.BtnKeluarForm.Name = "BtnKeluarForm"
        Me.BtnKeluarForm.Size = New System.Drawing.Size(112, 31)
        Me.BtnKeluarForm.TabIndex = 2
        Me.BtnKeluarForm.Text = "Keluar (Esc)"
        Me.BtnKeluarForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarForm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluarForm.UseVisualStyleBackColor = False
        '
        'LblJenisTrans
        '
        Me.LblJenisTrans.AutoSize = True
        Me.LblJenisTrans.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisTrans.Location = New System.Drawing.Point(23, 9)
        Me.LblJenisTrans.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblJenisTrans.Name = "LblJenisTrans"
        Me.LblJenisTrans.Size = New System.Drawing.Size(92, 16)
        Me.LblJenisTrans.TabIndex = 265
        Me.LblJenisTrans.Text = "LblJenisTrans"
        Me.LblJenisTrans.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblJenisTrans.Visible = False
        '
        'Label8
        '
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 21.75!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(0, 0)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(1144, 34)
        Me.Label8.TabIndex = 20
        Me.Label8.Text = "SURAT JALAN PENGIRIMAN"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelInput
        '
        Me.PanelInput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelInput.BackColor = System.Drawing.Color.SaddleBrown
        Me.PanelInput.Controls.Add(Me.BtnTransfer)
        Me.PanelInput.Controls.Add(Me.BtnHideDaftar)
        Me.PanelInput.Controls.Add(Me.DtpPenjualan)
        Me.PanelInput.Controls.Add(Me.Label18)
        Me.PanelInput.Controls.Add(Me.DGVPenjualan)
        Me.PanelInput.Location = New System.Drawing.Point(205, 38)
        Me.PanelInput.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelInput.Name = "PanelInput"
        Me.PanelInput.Size = New System.Drawing.Size(869, 475)
        Me.PanelInput.TabIndex = 233
        '
        'BtnTransfer
        '
        Me.BtnTransfer.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnTransfer.AutoSize = True
        Me.BtnTransfer.BackColor = System.Drawing.Color.White
        Me.BtnTransfer.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTransfer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnTransfer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnTransfer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTransfer.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTransfer.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnTransfer.Image = CType(resources.GetObject("BtnTransfer.Image"), System.Drawing.Image)
        Me.BtnTransfer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTransfer.Location = New System.Drawing.Point(41, 436)
        Me.BtnTransfer.Name = "BtnTransfer"
        Me.BtnTransfer.Size = New System.Drawing.Size(141, 32)
        Me.BtnTransfer.TabIndex = 233
        Me.BtnTransfer.Text = "Transfer (F6)"
        Me.BtnTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTransfer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTransfer.UseVisualStyleBackColor = False
        '
        'BtnHideDaftar
        '
        Me.BtnHideDaftar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnHideDaftar.BackColor = System.Drawing.Color.White
        Me.BtnHideDaftar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnHideDaftar.FlatAppearance.BorderSize = 0
        Me.BtnHideDaftar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnHideDaftar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnHideDaftar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHideDaftar.Image = CType(resources.GetObject("BtnHideDaftar.Image"), System.Drawing.Image)
        Me.BtnHideDaftar.Location = New System.Drawing.Point(834, 6)
        Me.BtnHideDaftar.Name = "BtnHideDaftar"
        Me.BtnHideDaftar.Size = New System.Drawing.Size(23, 23)
        Me.BtnHideDaftar.TabIndex = 232
        Me.BtnHideDaftar.UseVisualStyleBackColor = False
        '
        'DtpPenjualan
        '
        Me.DtpPenjualan.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpPenjualan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpPenjualan.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpPenjualan.Location = New System.Drawing.Point(365, 6)
        Me.DtpPenjualan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpPenjualan.Name = "DtpPenjualan"
        Me.DtpPenjualan.Size = New System.Drawing.Size(144, 23)
        Me.DtpPenjualan.TabIndex = 1
        '
        'Label18
        '
        Me.Label18.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.ForeColor = System.Drawing.Color.White
        Me.Label18.Location = New System.Drawing.Point(20, 9)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(345, 16)
        Me.Label18.TabIndex = 230
        Me.Label18.Text = "Pilih tanggal penjualan untuk mencari nomor nota :"
        '
        'DGVPenjualan
        '
        Me.DGVPenjualan.AllowUserToAddRows = False
        Me.DGVPenjualan.AllowUserToDeleteRows = False
        Me.DGVPenjualan.AllowUserToResizeRows = False
        Me.DGVPenjualan.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVPenjualan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVPenjualan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVPenjualan.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVPenjualan.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DGVPenjualan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVPenjualan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.chk, Me.ID_PENJUALAN, Me.ID_PELANGGAN, Me.NAMA_PELANGGAN, Me.ALAMAT_PELANGGAN, Me.TGL_TRANSAKSI, Me.GRAND_TOTAL_STL_PAJAK, Me.LOKASIBARANG, Me.SUMBER})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVPenjualan.DefaultCellStyle = DataGridViewCellStyle2
        Me.DGVPenjualan.Location = New System.Drawing.Point(4, 35)
        Me.DGVPenjualan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DGVPenjualan.Name = "DGVPenjualan"
        Me.DGVPenjualan.RowHeadersVisible = False
        Me.DGVPenjualan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVPenjualan.Size = New System.Drawing.Size(861, 395)
        Me.DGVPenjualan.TabIndex = 2
        '
        'DGVSuratJalan
        '
        Me.DGVSuratJalan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVSuratJalan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVSuratJalan.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.DGVSuratJalan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVSuratJalan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.SUMBER_TRANS, Me.Nota, Me.Kode, Me.Pelanggan, Me.Alamat, Me.Tanggal, Me.Nominal, Me.Lokasi, Me.btnHapus})
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVSuratJalan.DefaultCellStyle = DataGridViewCellStyle5
        Me.DGVSuratJalan.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVSuratJalan.Location = New System.Drawing.Point(0, 0)
        Me.DGVSuratJalan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DGVSuratJalan.Name = "DGVSuratJalan"
        Me.DGVSuratJalan.RowHeadersVisible = False
        Me.DGVSuratJalan.Size = New System.Drawing.Size(1141, 400)
        Me.DGVSuratJalan.TabIndex = 234
        '
        'PanelInput2
        '
        Me.PanelInput2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelInput2.Controls.Add(Me.BtnSimpann)
        Me.PanelInput2.Controls.Add(Me.TxtTotalPelanggan)
        Me.PanelInput2.Controls.Add(Me.Label1)
        Me.PanelInput2.Controls.Add(Me.Label7)
        Me.PanelInput2.Controls.Add(Me.TxtTotalRupiah)
        Me.PanelInput2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelInput2.Location = New System.Drawing.Point(0, 519)
        Me.PanelInput2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelInput2.Name = "PanelInput2"
        Me.PanelInput2.Size = New System.Drawing.Size(1144, 58)
        Me.PanelInput2.TabIndex = 235
        '
        'BtnSimpann
        '
        Me.BtnSimpann.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.BtnSimpann.Location = New System.Drawing.Point(843, 12)
        Me.BtnSimpann.Name = "BtnSimpann"
        Me.BtnSimpann.Size = New System.Drawing.Size(114, 35)
        Me.BtnSimpann.TabIndex = 1
        Me.BtnSimpann.Text = "Simpan (F8)"
        Me.BtnSimpann.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpann.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpann.UseVisualStyleBackColor = False
        '
        'TxtTotalPelanggan
        '
        Me.TxtTotalPelanggan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalPelanggan.ForeColor = System.Drawing.Color.Green
        Me.TxtTotalPelanggan.Location = New System.Drawing.Point(153, 6)
        Me.TxtTotalPelanggan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTotalPelanggan.Name = "TxtTotalPelanggan"
        Me.TxtTotalPelanggan.Size = New System.Drawing.Size(70, 23)
        Me.TxtTotalPelanggan.TabIndex = 230
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(23, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(122, 17)
        Me.Label1.TabIndex = 224
        Me.Label1.Text = "Total Pelanggan :"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(49, 33)
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
        Me.TxtTotalRupiah.Location = New System.Drawing.Point(153, 30)
        Me.TxtTotalRupiah.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTotalRupiah.Name = "TxtTotalRupiah"
        Me.TxtTotalRupiah.Size = New System.Drawing.Size(182, 23)
        Me.TxtTotalRupiah.TabIndex = 221
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.DGVSuratJalan)
        Me.Panel1.Location = New System.Drawing.Point(0, 122)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1141, 400)
        Me.Panel1.TabIndex = 236
        '
        'chk
        '
        Me.chk.FillWeight = 30.0!
        Me.chk.HeaderText = "Pilih"
        Me.chk.Name = "chk"
        '
        'ID_PENJUALAN
        '
        Me.ID_PENJUALAN.HeaderText = "Nota"
        Me.ID_PENJUALAN.Name = "ID_PENJUALAN"
        Me.ID_PENJUALAN.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ID_PENJUALAN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'ID_PELANGGAN
        '
        Me.ID_PELANGGAN.HeaderText = "Kode"
        Me.ID_PELANGGAN.Name = "ID_PELANGGAN"
        '
        'NAMA_PELANGGAN
        '
        Me.NAMA_PELANGGAN.FillWeight = 150.0!
        Me.NAMA_PELANGGAN.HeaderText = "Pelanggan"
        Me.NAMA_PELANGGAN.Name = "NAMA_PELANGGAN"
        Me.NAMA_PELANGGAN.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.NAMA_PELANGGAN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'ALAMAT_PELANGGAN
        '
        Me.ALAMAT_PELANGGAN.HeaderText = "Alamat"
        Me.ALAMAT_PELANGGAN.Name = "ALAMAT_PELANGGAN"
        '
        'TGL_TRANSAKSI
        '
        Me.TGL_TRANSAKSI.HeaderText = "Tanggal"
        Me.TGL_TRANSAKSI.Name = "TGL_TRANSAKSI"
        Me.TGL_TRANSAKSI.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TGL_TRANSAKSI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'GRAND_TOTAL_STL_PAJAK
        '
        Me.GRAND_TOTAL_STL_PAJAK.FillWeight = 80.0!
        Me.GRAND_TOTAL_STL_PAJAK.HeaderText = "Nominal"
        Me.GRAND_TOTAL_STL_PAJAK.Name = "GRAND_TOTAL_STL_PAJAK"
        Me.GRAND_TOTAL_STL_PAJAK.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.GRAND_TOTAL_STL_PAJAK.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'LOKASIBARANG
        '
        Me.LOKASIBARANG.FillWeight = 80.0!
        Me.LOKASIBARANG.HeaderText = "Lokasi"
        Me.LOKASIBARANG.Name = "LOKASIBARANG"
        Me.LOKASIBARANG.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.LOKASIBARANG.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'SUMBER
        '
        Me.SUMBER.HeaderText = "Sumber"
        Me.SUMBER.Name = "SUMBER"
        '
        'SUMBER_TRANS
        '
        Me.SUMBER_TRANS.HeaderText = "Sumber"
        Me.SUMBER_TRANS.Name = "SUMBER_TRANS"
        '
        'Nota
        '
        Me.Nota.HeaderText = "Nota"
        Me.Nota.Name = "Nota"
        '
        'Kode
        '
        Me.Kode.HeaderText = "Kode"
        Me.Kode.Name = "Kode"
        '
        'Pelanggan
        '
        Me.Pelanggan.HeaderText = "Pelanggan"
        Me.Pelanggan.Name = "Pelanggan"
        '
        'Alamat
        '
        Me.Alamat.HeaderText = "Alamat"
        Me.Alamat.Name = "Alamat"
        '
        'Tanggal
        '
        Me.Tanggal.HeaderText = "Tanggal"
        Me.Tanggal.Name = "Tanggal"
        '
        'Nominal
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "N2"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.Nominal.DefaultCellStyle = DataGridViewCellStyle4
        Me.Nominal.HeaderText = "Nominal"
        Me.Nominal.Name = "Nominal"
        '
        'Lokasi
        '
        Me.Lokasi.HeaderText = "Lokasi"
        Me.Lokasi.Name = "Lokasi"
        '
        'btnHapus
        '
        Me.btnHapus.FillWeight = 40.0!
        Me.btnHapus.HeaderText = "Hapus"
        Me.btnHapus.Name = "btnHapus"
        Me.btnHapus.Text = "Hapus"
        Me.btnHapus.UseColumnTextForButtonValue = True
        '
        'FormSuratJalan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1144, 577)
        Me.Controls.Add(Me.PanelInput)
        Me.Controls.Add(Me.PanelInput2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.PanelNota)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormSuratJalan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.PanelNota.ResumeLayout(False)
        Me.PanelNota.PerformLayout()
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.PanelInput.ResumeLayout(False)
        Me.PanelInput.PerformLayout()
        CType(Me.DGVPenjualan, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVSuratJalan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelInput2.ResumeLayout(False)
        Me.PanelInput2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelNota As System.Windows.Forms.Panel
    Friend WithEvents DtpSuratJalan As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents PanelInput As System.Windows.Forms.Panel
    Friend WithEvents BtnHideDaftar As System.Windows.Forms.Button
    Friend WithEvents DtpPenjualan As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents DGVPenjualan As System.Windows.Forms.DataGridView
    Friend WithEvents DGVSuratJalan As System.Windows.Forms.DataGridView
    Friend WithEvents PanelInput2 As System.Windows.Forms.Panel
    Friend WithEvents TxtTotalPelanggan As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalRupiah As System.Windows.Forms.TextBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents LblNoNota As System.Windows.Forms.Label
    Friend WithEvents CmbHelper2 As System.Windows.Forms.ComboBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents CmbHelper1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents CmbSopir As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents CmbArmada As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LblJenisArmada As System.Windows.Forms.Label
    Friend WithEvents BtnDaftarBarang As System.Windows.Forms.Button
    Friend WithEvents BtnKeluarForm As System.Windows.Forms.Button
    Friend WithEvents BtnSimpann As System.Windows.Forms.Button
    Friend WithEvents BtnTransfer As System.Windows.Forms.Button
    Friend WithEvents LblKodeArmada As System.Windows.Forms.Label
    Friend WithEvents LblKodeHelper2 As System.Windows.Forms.Label
    Friend WithEvents LblKodeHelper1 As System.Windows.Forms.Label
    Friend WithEvents LblKodeSupir As System.Windows.Forms.Label
    Friend WithEvents LblJenisTrans As Label
    Friend WithEvents BtnSettingPrinter As Button
    Friend WithEvents chk As DataGridViewCheckBoxColumn
    Friend WithEvents ID_PENJUALAN As DataGridViewTextBoxColumn
    Friend WithEvents ID_PELANGGAN As DataGridViewTextBoxColumn
    Friend WithEvents NAMA_PELANGGAN As DataGridViewTextBoxColumn
    Friend WithEvents ALAMAT_PELANGGAN As DataGridViewTextBoxColumn
    Friend WithEvents TGL_TRANSAKSI As DataGridViewTextBoxColumn
    Friend WithEvents GRAND_TOTAL_STL_PAJAK As DataGridViewTextBoxColumn
    Friend WithEvents LOKASIBARANG As DataGridViewTextBoxColumn
    Friend WithEvents SUMBER As DataGridViewTextBoxColumn
    Friend WithEvents SUMBER_TRANS As DataGridViewTextBoxColumn
    Friend WithEvents Nota As DataGridViewTextBoxColumn
    Friend WithEvents Kode As DataGridViewTextBoxColumn
    Friend WithEvents Pelanggan As DataGridViewTextBoxColumn
    Friend WithEvents Alamat As DataGridViewTextBoxColumn
    Friend WithEvents Tanggal As DataGridViewTextBoxColumn
    Friend WithEvents Nominal As DataGridViewTextBoxColumn
    Friend WithEvents Lokasi As DataGridViewTextBoxColumn
    Friend WithEvents btnHapus As DataGridViewButtonColumn
End Class



