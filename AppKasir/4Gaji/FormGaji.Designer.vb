<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormGaji
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormGaji))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.Gaji_karyawanBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.LblNomor = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PanelNota = New System.Windows.Forms.Panel()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.LblSisaBon = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.LblSaldoBon = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.TxtPotBon = New System.Windows.Forms.TextBox()
        Me.TxtAngsuran = New System.Windows.Forms.TextBox()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.TxtPendapatan = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.TxtPotongan = New System.Windows.Forms.TextBox()
        Me.TxtTerima = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.LblTelat = New System.Windows.Forms.Label()
        Me.LblAbsenkhusus = New System.Windows.Forms.Label()
        Me.LblAbsen = New System.Windows.Forms.Label()
        Me.TxtKeterlambatan = New System.Windows.Forms.TextBox()
        Me.TxtKeterlambatanRp = New System.Windows.Forms.TextBox()
        Me.TxtAbsenKhusus = New System.Windows.Forms.TextBox()
        Me.TxtAbsenKhususRp = New System.Windows.Forms.TextBox()
        Me.TxtAbsen = New System.Windows.Forms.TextBox()
        Me.TxtAbsenRp = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtPotLain = New System.Windows.Forms.TextBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.TxtMakan = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TxtTunjangan = New System.Windows.Forms.TextBox()
        Me.TxtTransport = New System.Windows.Forms.TextBox()
        Me.LblLembur = New System.Windows.Forms.Label()
        Me.TxtLembur = New System.Windows.Forms.TextBox()
        Me.TxtLemburRp = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.TxtOmsetJual = New System.Windows.Forms.TextBox()
        Me.TxtKomisiJual = New System.Windows.Forms.TextBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.LblKomisJual = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblKetSupir = New System.Windows.Forms.Label()
        Me.LblKetHelp = New System.Windows.Forms.Label()
        Me.TxtPokok = New System.Windows.Forms.TextBox()
        Me.TxtSupir = New System.Windows.Forms.TextBox()
        Me.TxtHelper = New System.Windows.Forms.TextBox()
        Me.BtnSimpann = New System.Windows.Forms.Button()
        Me.LblHelper = New System.Windows.Forms.Label()
        Me.LblSupir = New System.Windows.Forms.Label()
        Me.LblKode = New System.Windows.Forms.Label()
        Me.CmbNama = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.PanelData = New System.Windows.Forms.Panel()
        Me.DGVGaji = New System.Windows.Forms.DataGridView()
        Me.CmbPilihCetak = New System.Windows.Forms.ComboBox()
        Me.LabelSupir = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnSettingPrinter = New System.Windows.Forms.Button()
        Me.CmbProsesCetak = New System.Windows.Forms.ComboBox()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.TxtPotAbsen = New System.Windows.Forms.TextBox()
        Me.TxtPotBonUntukEdit = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.LblRekening = New System.Windows.Forms.Label()
        Me.TxtTanggal = New System.Windows.Forms.TextBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.DtpAkhir = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.DtpAwal = New System.Windows.Forms.DateTimePicker()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        CType(Me.Gaji_karyawanBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHeader.SuspendLayout()
        Me.PanelNota.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.PanelData.SuspendLayout()
        CType(Me.DGVGaji, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Gaji_karyawanBindingSource
        '
        Me.Gaji_karyawanBindingSource.DataMember = "Gaji_karyawan"
        '
        'PanelHeader
        '
        Me.PanelHeader.AutoScroll = True
        Me.PanelHeader.BackColor = System.Drawing.SystemColors.Control
        Me.PanelHeader.Controls.Add(Me.DtpTanggal)
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.Label8)
        Me.PanelHeader.Controls.Add(Me.LblNomor)
        Me.PanelHeader.Controls.Add(Me.Label2)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1194, 34)
        Me.PanelHeader.TabIndex = 1
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DtpTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(190, 7)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(175, 23)
        Me.DtpTanggal.TabIndex = 238
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.SystemColors.Control
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(1160, 2)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 31)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(471, 6)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(167, 23)
        Me.Label8.TabIndex = 20
        Me.Label8.Text = "GAJI KARYAWAN"
        '
        'LblNomor
        '
        Me.LblNomor.AutoSize = True
        Me.LblNomor.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNomor.Location = New System.Drawing.Point(89, 9)
        Me.LblNomor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblNomor.Name = "LblNomor"
        Me.LblNomor.Size = New System.Drawing.Size(50, 16)
        Me.LblNomor.TabIndex = 237
        Me.LblNomor.Text = "Nomor"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(27, 9)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 16)
        Me.Label2.TabIndex = 226
        Me.Label2.Text = "Nomor :"
        '
        'PanelNota
        '
        Me.PanelNota.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelNota.AutoScroll = True
        Me.PanelNota.BackColor = System.Drawing.SystemColors.Control
        Me.PanelNota.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelNota.Controls.Add(Me.Panel7)
        Me.PanelNota.Controls.Add(Me.Panel6)
        Me.PanelNota.Controls.Add(Me.Panel5)
        Me.PanelNota.Controls.Add(Me.Panel4)
        Me.PanelNota.Controls.Add(Me.Button2)
        Me.PanelNota.Controls.Add(Me.Panel3)
        Me.PanelNota.Controls.Add(Me.BtnSimpann)
        Me.PanelNota.Location = New System.Drawing.Point(0, 100)
        Me.PanelNota.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelNota.Name = "PanelNota"
        Me.PanelNota.Size = New System.Drawing.Size(1191, 158)
        Me.PanelNota.TabIndex = 2
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.SystemColors.Control
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel7.Controls.Add(Me.LblSisaBon)
        Me.Panel7.Controls.Add(Me.Label32)
        Me.Panel7.Controls.Add(Me.Label25)
        Me.Panel7.Controls.Add(Me.LblSaldoBon)
        Me.Panel7.Controls.Add(Me.Label16)
        Me.Panel7.Controls.Add(Me.Label10)
        Me.Panel7.Controls.Add(Me.Label28)
        Me.Panel7.Controls.Add(Me.TxtPotBon)
        Me.Panel7.Controls.Add(Me.TxtAngsuran)
        Me.Panel7.Location = New System.Drawing.Point(511, 1)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(196, 150)
        Me.Panel7.TabIndex = 307
        '
        'LblSisaBon
        '
        Me.LblSisaBon.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblSisaBon.Location = New System.Drawing.Point(84, 98)
        Me.LblSisaBon.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSisaBon.Name = "LblSisaBon"
        Me.LblSisaBon.Size = New System.Drawing.Size(105, 16)
        Me.LblSisaBon.TabIndex = 308
        Me.LblSisaBon.Text = "Rp. 5.000"
        Me.LblSisaBon.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label32.Location = New System.Drawing.Point(12, 98)
        Me.Label32.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(67, 17)
        Me.Label32.TabIndex = 307
        Me.Label32.Text = "Sisa Bon :"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(33, 4)
        Me.Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(128, 16)
        Me.Label25.TabIndex = 292
        Me.Label25.Text = "POTONGAN BON"
        '
        'LblSaldoBon
        '
        Me.LblSaldoBon.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblSaldoBon.Location = New System.Drawing.Point(84, 30)
        Me.LblSaldoBon.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSaldoBon.Name = "LblSaldoBon"
        Me.LblSaldoBon.Size = New System.Drawing.Size(105, 16)
        Me.LblSaldoBon.TabIndex = 306
        Me.LblSaldoBon.Text = "Rp. 5.000"
        Me.LblSaldoBon.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label16.Location = New System.Drawing.Point(11, 52)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(68, 17)
        Me.Label16.TabIndex = 286
        Me.Label16.Text = "Pot bon :"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label10.Location = New System.Drawing.Point(-1, 30)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(80, 17)
        Me.Label10.TabIndex = 305
        Me.Label10.Text = "Saldo Bon :"
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label28.Location = New System.Drawing.Point(6, 75)
        Me.Label28.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(73, 17)
        Me.Label28.TabIndex = 287
        Me.Label28.Text = "Pot Angs :"
        '
        'TxtPotBon
        '
        Me.TxtPotBon.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtPotBon.ForeColor = System.Drawing.Color.Black
        Me.TxtPotBon.Location = New System.Drawing.Point(84, 49)
        Me.TxtPotBon.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPotBon.Name = "TxtPotBon"
        Me.TxtPotBon.Size = New System.Drawing.Size(105, 23)
        Me.TxtPotBon.TabIndex = 288
        Me.TxtPotBon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtAngsuran
        '
        Me.TxtAngsuran.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAngsuran.ForeColor = System.Drawing.Color.Black
        Me.TxtAngsuran.Location = New System.Drawing.Point(84, 72)
        Me.TxtAngsuran.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtAngsuran.Name = "TxtAngsuran"
        Me.TxtAngsuran.Size = New System.Drawing.Size(105, 23)
        Me.TxtAngsuran.TabIndex = 289
        Me.TxtAngsuran.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.SystemColors.Control
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.TxtPendapatan)
        Me.Panel6.Controls.Add(Me.Label24)
        Me.Panel6.Controls.Add(Me.Label23)
        Me.Panel6.Controls.Add(Me.Label22)
        Me.Panel6.Controls.Add(Me.TxtPotongan)
        Me.Panel6.Controls.Add(Me.TxtTerima)
        Me.Panel6.Controls.Add(Me.Label21)
        Me.Panel6.Location = New System.Drawing.Point(958, 1)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(205, 103)
        Me.Panel6.TabIndex = 306
        '
        'TxtPendapatan
        '
        Me.TxtPendapatan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtPendapatan.ForeColor = System.Drawing.Color.Black
        Me.TxtPendapatan.Location = New System.Drawing.Point(103, 23)
        Me.TxtPendapatan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPendapatan.Name = "TxtPendapatan"
        Me.TxtPendapatan.ReadOnly = True
        Me.TxtPendapatan.Size = New System.Drawing.Size(96, 23)
        Me.TxtPendapatan.TabIndex = 295
        Me.TxtPendapatan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label24.Location = New System.Drawing.Point(1, 26)
        Me.Label24.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(98, 17)
        Me.Label24.TabIndex = 293
        Me.Label24.Text = "Pendapatan :"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label23.Location = New System.Drawing.Point(18, 49)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(81, 17)
        Me.Label23.TabIndex = 296
        Me.Label23.Text = "Potongan :"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label22.Location = New System.Drawing.Point(41, 72)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(58, 17)
        Me.Label22.TabIndex = 297
        Me.Label22.Text = "Terima :"
        '
        'TxtPotongan
        '
        Me.TxtPotongan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtPotongan.ForeColor = System.Drawing.Color.Black
        Me.TxtPotongan.Location = New System.Drawing.Point(103, 46)
        Me.TxtPotongan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPotongan.Name = "TxtPotongan"
        Me.TxtPotongan.ReadOnly = True
        Me.TxtPotongan.Size = New System.Drawing.Size(96, 23)
        Me.TxtPotongan.TabIndex = 298
        Me.TxtPotongan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTerima
        '
        Me.TxtTerima.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtTerima.ForeColor = System.Drawing.Color.Black
        Me.TxtTerima.Location = New System.Drawing.Point(103, 69)
        Me.TxtTerima.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTerima.Name = "TxtTerima"
        Me.TxtTerima.ReadOnly = True
        Me.TxtTerima.Size = New System.Drawing.Size(96, 23)
        Me.TxtTerima.TabIndex = 299
        Me.TxtTerima.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(48, 4)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(103, 16)
        Me.Label21.TabIndex = 300
        Me.Label21.Text = "RANGKUMAN"
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.SystemColors.Control
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.Label20)
        Me.Panel5.Controls.Add(Me.LblTelat)
        Me.Panel5.Controls.Add(Me.LblAbsenkhusus)
        Me.Panel5.Controls.Add(Me.LblAbsen)
        Me.Panel5.Controls.Add(Me.TxtKeterlambatan)
        Me.Panel5.Controls.Add(Me.TxtKeterlambatanRp)
        Me.Panel5.Controls.Add(Me.TxtAbsenKhusus)
        Me.Panel5.Controls.Add(Me.TxtAbsenKhususRp)
        Me.Panel5.Controls.Add(Me.TxtAbsen)
        Me.Panel5.Controls.Add(Me.TxtAbsenRp)
        Me.Panel5.Controls.Add(Me.Label14)
        Me.Panel5.Controls.Add(Me.TxtPotLain)
        Me.Panel5.Location = New System.Drawing.Point(706, 1)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(253, 150)
        Me.Panel5.TabIndex = 305
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(41, 4)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(129, 16)
        Me.Label20.TabIndex = 292
        Me.Label20.Text = "POTONGAN LAIN"
        '
        'LblTelat
        '
        Me.LblTelat.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblTelat.Location = New System.Drawing.Point(4, 75)
        Me.LblTelat.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblTelat.Name = "LblTelat"
        Me.LblTelat.Size = New System.Drawing.Size(147, 16)
        Me.LblTelat.TabIndex = 283
        Me.LblTelat.Text = "Keterlambatan :"
        Me.LblTelat.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblAbsenkhusus
        '
        Me.LblAbsenkhusus.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblAbsenkhusus.Location = New System.Drawing.Point(4, 52)
        Me.LblAbsenkhusus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblAbsenkhusus.Name = "LblAbsenkhusus"
        Me.LblAbsenkhusus.Size = New System.Drawing.Size(147, 16)
        Me.LblAbsenkhusus.TabIndex = 283
        Me.LblAbsenkhusus.Text = "Absen Khusus :"
        Me.LblAbsenkhusus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblAbsen
        '
        Me.LblAbsen.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblAbsen.Location = New System.Drawing.Point(1, 29)
        Me.LblAbsen.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblAbsen.Name = "LblAbsen"
        Me.LblAbsen.Size = New System.Drawing.Size(150, 16)
        Me.LblAbsen.TabIndex = 283
        Me.LblAbsen.Text = "Absen :"
        Me.LblAbsen.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtKeterlambatan
        '
        Me.TxtKeterlambatan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKeterlambatan.ForeColor = System.Drawing.Color.Black
        Me.TxtKeterlambatan.Location = New System.Drawing.Point(153, 72)
        Me.TxtKeterlambatan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtKeterlambatan.Name = "TxtKeterlambatan"
        Me.TxtKeterlambatan.Size = New System.Drawing.Size(28, 23)
        Me.TxtKeterlambatan.TabIndex = 284
        '
        'TxtKeterlambatanRp
        '
        Me.TxtKeterlambatanRp.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKeterlambatanRp.ForeColor = System.Drawing.Color.Black
        Me.TxtKeterlambatanRp.Location = New System.Drawing.Point(183, 72)
        Me.TxtKeterlambatanRp.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtKeterlambatanRp.Name = "TxtKeterlambatanRp"
        Me.TxtKeterlambatanRp.Size = New System.Drawing.Size(65, 23)
        Me.TxtKeterlambatanRp.TabIndex = 285
        Me.TxtKeterlambatanRp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtAbsenKhusus
        '
        Me.TxtAbsenKhusus.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAbsenKhusus.ForeColor = System.Drawing.Color.Black
        Me.TxtAbsenKhusus.Location = New System.Drawing.Point(153, 49)
        Me.TxtAbsenKhusus.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtAbsenKhusus.Name = "TxtAbsenKhusus"
        Me.TxtAbsenKhusus.Size = New System.Drawing.Size(28, 23)
        Me.TxtAbsenKhusus.TabIndex = 284
        '
        'TxtAbsenKhususRp
        '
        Me.TxtAbsenKhususRp.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAbsenKhususRp.ForeColor = System.Drawing.Color.Black
        Me.TxtAbsenKhususRp.Location = New System.Drawing.Point(183, 49)
        Me.TxtAbsenKhususRp.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtAbsenKhususRp.Name = "TxtAbsenKhususRp"
        Me.TxtAbsenKhususRp.Size = New System.Drawing.Size(65, 23)
        Me.TxtAbsenKhususRp.TabIndex = 285
        Me.TxtAbsenKhususRp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtAbsen
        '
        Me.TxtAbsen.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAbsen.ForeColor = System.Drawing.Color.Black
        Me.TxtAbsen.Location = New System.Drawing.Point(153, 26)
        Me.TxtAbsen.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtAbsen.Name = "TxtAbsen"
        Me.TxtAbsen.Size = New System.Drawing.Size(28, 23)
        Me.TxtAbsen.TabIndex = 284
        '
        'TxtAbsenRp
        '
        Me.TxtAbsenRp.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAbsenRp.ForeColor = System.Drawing.Color.Black
        Me.TxtAbsenRp.Location = New System.Drawing.Point(183, 26)
        Me.TxtAbsenRp.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtAbsenRp.Name = "TxtAbsenRp"
        Me.TxtAbsenRp.Size = New System.Drawing.Size(65, 23)
        Me.TxtAbsenRp.TabIndex = 285
        Me.TxtAbsenRp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label14
        '
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label14.Location = New System.Drawing.Point(7, 98)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(144, 16)
        Me.Label14.TabIndex = 287
        Me.Label14.Text = "Pot Lain :"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPotLain
        '
        Me.TxtPotLain.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtPotLain.ForeColor = System.Drawing.Color.Black
        Me.TxtPotLain.Location = New System.Drawing.Point(153, 95)
        Me.TxtPotLain.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPotLain.Name = "TxtPotLain"
        Me.TxtPotLain.Size = New System.Drawing.Size(95, 23)
        Me.TxtPotLain.TabIndex = 289
        Me.TxtPotLain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.SystemColors.Control
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.Label31)
        Me.Panel4.Controls.Add(Me.TxtMakan)
        Me.Panel4.Controls.Add(Me.Label19)
        Me.Panel4.Controls.Add(Me.Label7)
        Me.Panel4.Controls.Add(Me.Label11)
        Me.Panel4.Controls.Add(Me.TxtTunjangan)
        Me.Panel4.Controls.Add(Me.TxtTransport)
        Me.Panel4.Controls.Add(Me.LblLembur)
        Me.Panel4.Controls.Add(Me.TxtLembur)
        Me.Panel4.Controls.Add(Me.TxtLemburRp)
        Me.Panel4.Location = New System.Drawing.Point(303, 1)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(209, 150)
        Me.Panel4.TabIndex = 304
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label31.Location = New System.Drawing.Point(42, 98)
        Me.Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(60, 17)
        Me.Label31.TabIndex = 292
        Me.Label31.Text = "Makan :"
        '
        'TxtMakan
        '
        Me.TxtMakan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtMakan.ForeColor = System.Drawing.Color.Black
        Me.TxtMakan.Location = New System.Drawing.Point(105, 95)
        Me.TxtMakan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtMakan.Name = "TxtMakan"
        Me.TxtMakan.Size = New System.Drawing.Size(100, 23)
        Me.TxtMakan.TabIndex = 293
        Me.TxtMakan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(13, 4)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(158, 16)
        Me.Label19.TabIndex = 291
        Me.Label19.Text = "TUNJANGAN/BONUS"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label7.Location = New System.Drawing.Point(19, 52)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(83, 17)
        Me.Label7.TabIndex = 268
        Me.Label7.Text = "Tunjangan :"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label11.Location = New System.Drawing.Point(28, 75)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(74, 17)
        Me.Label11.TabIndex = 269
        Me.Label11.Text = "Transport :"
        '
        'TxtTunjangan
        '
        Me.TxtTunjangan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtTunjangan.ForeColor = System.Drawing.Color.Black
        Me.TxtTunjangan.Location = New System.Drawing.Point(105, 49)
        Me.TxtTunjangan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTunjangan.Name = "TxtTunjangan"
        Me.TxtTunjangan.Size = New System.Drawing.Size(100, 23)
        Me.TxtTunjangan.TabIndex = 274
        Me.TxtTunjangan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtTransport
        '
        Me.TxtTransport.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtTransport.ForeColor = System.Drawing.Color.Black
        Me.TxtTransport.Location = New System.Drawing.Point(105, 72)
        Me.TxtTransport.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTransport.Name = "TxtTransport"
        Me.TxtTransport.Size = New System.Drawing.Size(100, 23)
        Me.TxtTransport.TabIndex = 275
        Me.TxtTransport.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblLembur
        '
        Me.LblLembur.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblLembur.Location = New System.Drawing.Point(3, 29)
        Me.LblLembur.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblLembur.Name = "LblLembur"
        Me.LblLembur.Size = New System.Drawing.Size(99, 16)
        Me.LblLembur.TabIndex = 276
        Me.LblLembur.Text = "Lembur :"
        Me.LblLembur.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtLembur
        '
        Me.TxtLembur.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtLembur.ForeColor = System.Drawing.Color.Black
        Me.TxtLembur.Location = New System.Drawing.Point(105, 26)
        Me.TxtLembur.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtLembur.Name = "TxtLembur"
        Me.TxtLembur.Size = New System.Drawing.Size(34, 23)
        Me.TxtLembur.TabIndex = 279
        '
        'TxtLemburRp
        '
        Me.TxtLemburRp.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtLemburRp.ForeColor = System.Drawing.Color.Black
        Me.TxtLemburRp.Location = New System.Drawing.Point(140, 26)
        Me.TxtLemburRp.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtLemburRp.Name = "TxtLemburRp"
        Me.TxtLemburRp.Size = New System.Drawing.Size(65, 23)
        Me.TxtLemburRp.TabIndex = 282
        Me.TxtLemburRp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Button2
        '
        Me.Button2.AutoSize = True
        Me.Button2.BackColor = System.Drawing.SystemColors.Control
        Me.Button2.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.Button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.Button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.Button2.Image = CType(resources.GetObject("Button2.Image"), System.Drawing.Image)
        Me.Button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button2.Location = New System.Drawing.Point(1132, 110)
        Me.Button2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(68, 31)
        Me.Button2.TabIndex = 302
        Me.Button2.Text = "Baru"
        Me.Button2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.SystemColors.Control
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.TxtOmsetJual)
        Me.Panel3.Controls.Add(Me.TxtKomisiJual)
        Me.Panel3.Controls.Add(Me.Label30)
        Me.Panel3.Controls.Add(Me.LblKomisJual)
        Me.Panel3.Controls.Add(Me.Label17)
        Me.Panel3.Controls.Add(Me.Label4)
        Me.Panel3.Controls.Add(Me.LblKetSupir)
        Me.Panel3.Controls.Add(Me.LblKetHelp)
        Me.Panel3.Controls.Add(Me.TxtPokok)
        Me.Panel3.Controls.Add(Me.TxtSupir)
        Me.Panel3.Controls.Add(Me.TxtHelper)
        Me.Panel3.Location = New System.Drawing.Point(6, 1)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(298, 150)
        Me.Panel3.TabIndex = 303
        '
        'TxtOmsetJual
        '
        Me.TxtOmsetJual.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtOmsetJual.ForeColor = System.Drawing.Color.Black
        Me.TxtOmsetJual.Location = New System.Drawing.Point(182, 49)
        Me.TxtOmsetJual.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtOmsetJual.Name = "TxtOmsetJual"
        Me.TxtOmsetJual.ReadOnly = True
        Me.TxtOmsetJual.Size = New System.Drawing.Size(112, 23)
        Me.TxtOmsetJual.TabIndex = 294
        Me.TxtOmsetJual.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtKomisiJual
        '
        Me.TxtKomisiJual.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKomisiJual.ForeColor = System.Drawing.Color.Black
        Me.TxtKomisiJual.Location = New System.Drawing.Point(182, 72)
        Me.TxtKomisiJual.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtKomisiJual.Name = "TxtKomisiJual"
        Me.TxtKomisiJual.Size = New System.Drawing.Size(112, 23)
        Me.TxtKomisiJual.TabIndex = 293
        Me.TxtKomisiJual.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(88, 52)
        Me.Label30.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(88, 17)
        Me.Label30.TabIndex = 292
        Me.Label30.Text = "Omset Jual :"
        '
        'LblKomisJual
        '
        Me.LblKomisJual.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKomisJual.Location = New System.Drawing.Point(4, 75)
        Me.LblKomisJual.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKomisJual.Name = "LblKomisJual"
        Me.LblKomisJual.Size = New System.Drawing.Size(172, 16)
        Me.LblKomisJual.TabIndex = 291
        Me.LblKomisJual.Text = "Komisi Jual :"
        Me.LblKomisJual.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(99, 4)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(97, 16)
        Me.Label17.TabIndex = 290
        Me.Label17.Text = "GAJI UTAMA"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(89, 29)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(87, 17)
        Me.Label4.TabIndex = 266
        Me.Label4.Text = "Gaji pokok :"
        '
        'LblKetSupir
        '
        Me.LblKetSupir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKetSupir.Location = New System.Drawing.Point(4, 98)
        Me.LblKetSupir.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKetSupir.Name = "LblKetSupir"
        Me.LblKetSupir.Size = New System.Drawing.Size(172, 16)
        Me.LblKetSupir.TabIndex = 270
        Me.LblKetSupir.Text = "Tugas supir :"
        Me.LblKetSupir.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblKetHelp
        '
        Me.LblKetHelp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKetHelp.Location = New System.Drawing.Point(4, 121)
        Me.LblKetHelp.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKetHelp.Name = "LblKetHelp"
        Me.LblKetHelp.Size = New System.Drawing.Size(172, 16)
        Me.LblKetHelp.TabIndex = 271
        Me.LblKetHelp.Text = "Tugas helper :"
        Me.LblKetHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPokok
        '
        Me.TxtPokok.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPokok.ForeColor = System.Drawing.Color.Black
        Me.TxtPokok.Location = New System.Drawing.Point(182, 26)
        Me.TxtPokok.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPokok.Name = "TxtPokok"
        Me.TxtPokok.Size = New System.Drawing.Size(112, 23)
        Me.TxtPokok.TabIndex = 273
        Me.TxtPokok.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtSupir
        '
        Me.TxtSupir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSupir.ForeColor = System.Drawing.Color.Black
        Me.TxtSupir.Location = New System.Drawing.Point(182, 95)
        Me.TxtSupir.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtSupir.Name = "TxtSupir"
        Me.TxtSupir.Size = New System.Drawing.Size(112, 23)
        Me.TxtSupir.TabIndex = 280
        Me.TxtSupir.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtHelper
        '
        Me.TxtHelper.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHelper.ForeColor = System.Drawing.Color.Black
        Me.TxtHelper.Location = New System.Drawing.Point(182, 118)
        Me.TxtHelper.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtHelper.Name = "TxtHelper"
        Me.TxtHelper.Size = New System.Drawing.Size(112, 23)
        Me.TxtHelper.TabIndex = 281
        Me.TxtHelper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BtnSimpann
        '
        Me.BtnSimpann.AutoSize = True
        Me.BtnSimpann.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSimpann.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpann.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpann.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpann.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpann.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpann.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpann.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpann.Image = CType(resources.GetObject("BtnSimpann.Image"), System.Drawing.Image)
        Me.BtnSimpann.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpann.Location = New System.Drawing.Point(987, 110)
        Me.BtnSimpann.Name = "BtnSimpann"
        Me.BtnSimpann.Size = New System.Drawing.Size(114, 32)
        Me.BtnSimpann.TabIndex = 301
        Me.BtnSimpann.Text = "Simpan (F8)"
        Me.BtnSimpann.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpann.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpann.UseVisualStyleBackColor = False
        '
        'LblHelper
        '
        Me.LblHelper.AutoSize = True
        Me.LblHelper.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHelper.Location = New System.Drawing.Point(1155, 38)
        Me.LblHelper.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblHelper.Name = "LblHelper"
        Me.LblHelper.Size = New System.Drawing.Size(25, 13)
        Me.LblHelper.TabIndex = 272
        Me.LblHelper.Text = "100"
        Me.LblHelper.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblHelper.Visible = False
        '
        'LblSupir
        '
        Me.LblSupir.AutoSize = True
        Me.LblSupir.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSupir.Location = New System.Drawing.Point(1155, 10)
        Me.LblSupir.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSupir.Name = "LblSupir"
        Me.LblSupir.Size = New System.Drawing.Size(25, 13)
        Me.LblSupir.TabIndex = 267
        Me.LblSupir.Text = "100"
        Me.LblSupir.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblSupir.Visible = False
        '
        'LblKode
        '
        Me.LblKode.AutoSize = True
        Me.LblKode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKode.Location = New System.Drawing.Point(274, 38)
        Me.LblKode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKode.Name = "LblKode"
        Me.LblKode.Size = New System.Drawing.Size(42, 17)
        Me.LblKode.TabIndex = 265
        Me.LblKode.Text = "Kode"
        Me.LblKode.Visible = False
        '
        'CmbNama
        '
        Me.CmbNama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbNama.FormattingEnabled = True
        Me.CmbNama.Location = New System.Drawing.Point(89, 34)
        Me.CmbNama.Name = "CmbNama"
        Me.CmbNama.Size = New System.Drawing.Size(174, 25)
        Me.CmbNama.TabIndex = 2
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(30, 38)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(57, 17)
        Me.Label6.TabIndex = 255
        Me.Label6.Text = "Nama :"
        '
        'PanelData
        '
        Me.PanelData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelData.BackColor = System.Drawing.SystemColors.Control
        Me.PanelData.Controls.Add(Me.DGVGaji)
        Me.PanelData.Controls.Add(Me.CmbPilihCetak)
        Me.PanelData.Controls.Add(Me.LabelSupir)
        Me.PanelData.Location = New System.Drawing.Point(0, 260)
        Me.PanelData.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelData.Name = "PanelData"
        Me.PanelData.Size = New System.Drawing.Size(1194, 284)
        Me.PanelData.TabIndex = 3
        '
        'DGVGaji
        '
        Me.DGVGaji.AllowUserToAddRows = False
        Me.DGVGaji.AllowUserToDeleteRows = False
        Me.DGVGaji.AllowUserToResizeRows = False
        Me.DGVGaji.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVGaji.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVGaji.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVGaji.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DGVGaji.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVGaji.DefaultCellStyle = DataGridViewCellStyle2
        Me.DGVGaji.Location = New System.Drawing.Point(0, 0)
        Me.DGVGaji.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DGVGaji.Name = "DGVGaji"
        Me.DGVGaji.RowHeadersVisible = False
        Me.DGVGaji.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVGaji.Size = New System.Drawing.Size(1194, 281)
        Me.DGVGaji.TabIndex = 2
        '
        'CmbPilihCetak
        '
        Me.CmbPilihCetak.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPilihCetak.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbPilihCetak.FormattingEnabled = True
        Me.CmbPilihCetak.Items.AddRange(New Object() {"IYA", "SELALU TANYA", "TAMPILKAN DI MONITOR"})
        Me.CmbPilihCetak.Location = New System.Drawing.Point(706, 135)
        Me.CmbPilihCetak.Name = "CmbPilihCetak"
        Me.CmbPilihCetak.Size = New System.Drawing.Size(149, 21)
        Me.CmbPilihCetak.TabIndex = 309
        '
        'LabelSupir
        '
        Me.LabelSupir.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LabelSupir.AutoSize = True
        Me.LabelSupir.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSupir.Location = New System.Drawing.Point(4, 262)
        Me.LabelSupir.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelSupir.Name = "LabelSupir"
        Me.LabelSupir.Size = New System.Drawing.Size(51, 15)
        Me.LabelSupir.TabIndex = 287
        Me.LabelSupir.Text = "Nomor :"
        Me.LabelSupir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.LabelSupir.Visible = False
        '
        'Label18
        '
        Me.Label18.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(33, 10)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(51, 17)
        Me.Label18.TabIndex = 230
        Me.Label18.Text = "Bulan :"
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(89, 6)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(106, 25)
        Me.CmbBln.TabIndex = 234
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.AutoScroll = True
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.BtnSettingPrinter)
        Me.Panel1.Controls.Add(Me.CmbProsesCetak)
        Me.Panel1.Controls.Add(Me.ComboBox1)
        Me.Panel1.Controls.Add(Me.TxtPotAbsen)
        Me.Panel1.Controls.Add(Me.TxtPotBonUntukEdit)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.CmbRekening)
        Me.Panel1.Controls.Add(Me.LblRekening)
        Me.Panel1.Controls.Add(Me.TxtTanggal)
        Me.Panel1.Controls.Add(Me.CmbThn)
        Me.Panel1.Controls.Add(Me.LblSupir)
        Me.Panel1.Controls.Add(Me.DtpAkhir)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.LblHelper)
        Me.Panel1.Controls.Add(Me.CmbBln)
        Me.Panel1.Controls.Add(Me.Label18)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.DtpAwal)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.CmbNama)
        Me.Panel1.Controls.Add(Me.LblKode)
        Me.Panel1.Location = New System.Drawing.Point(0, 35)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1191, 64)
        Me.Panel1.TabIndex = 4
        '
        'BtnSettingPrinter
        '
        Me.BtnSettingPrinter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnSettingPrinter.AutoSize = True
        Me.BtnSettingPrinter.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSettingPrinter.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSettingPrinter.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnSettingPrinter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSettingPrinter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSettingPrinter.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSettingPrinter.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSettingPrinter.ForeColor = System.Drawing.Color.Black
        Me.BtnSettingPrinter.Image = CType(resources.GetObject("BtnSettingPrinter.Image"), System.Drawing.Image)
        Me.BtnSettingPrinter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSettingPrinter.Location = New System.Drawing.Point(1062, 23)
        Me.BtnSettingPrinter.Name = "BtnSettingPrinter"
        Me.BtnSettingPrinter.Size = New System.Drawing.Size(118, 32)
        Me.BtnSettingPrinter.TabIndex = 311
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
        Me.CmbProsesCetak.Location = New System.Drawing.Point(907, 34)
        Me.CmbProsesCetak.Name = "CmbProsesCetak"
        Me.CmbProsesCetak.Size = New System.Drawing.Size(149, 21)
        Me.CmbProsesCetak.TabIndex = 310
        '
        'ComboBox1
        '
        Me.ComboBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"IYA", "SELALU TANYA", "TAMPILKAN DI MONITOR"})
        Me.ComboBox1.Location = New System.Drawing.Point(907, 6)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(149, 21)
        Me.ComboBox1.TabIndex = 309
        '
        'TxtPotAbsen
        '
        Me.TxtPotAbsen.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPotAbsen.ForeColor = System.Drawing.Color.Black
        Me.TxtPotAbsen.Location = New System.Drawing.Point(933, 7)
        Me.TxtPotAbsen.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPotAbsen.Name = "TxtPotAbsen"
        Me.TxtPotAbsen.Size = New System.Drawing.Size(45, 23)
        Me.TxtPotAbsen.TabIndex = 308
        Me.TxtPotAbsen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtPotAbsen.Visible = False
        '
        'TxtPotBonUntukEdit
        '
        Me.TxtPotBonUntukEdit.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPotBonUntukEdit.ForeColor = System.Drawing.Color.Black
        Me.TxtPotBonUntukEdit.Location = New System.Drawing.Point(879, 35)
        Me.TxtPotBonUntukEdit.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtPotBonUntukEdit.Name = "TxtPotBonUntukEdit"
        Me.TxtPotBonUntukEdit.Size = New System.Drawing.Size(99, 23)
        Me.TxtPotBonUntukEdit.TabIndex = 307
        Me.TxtPotBonUntukEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtPotBonUntukEdit.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(437, 38)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(104, 17)
        Me.Label5.TabIndex = 303
        Me.Label5.Text = "Sumber Dana :"
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Location = New System.Drawing.Point(548, 34)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(255, 25)
        Me.CmbRekening.TabIndex = 302
        '
        'LblRekening
        '
        Me.LblRekening.AutoSize = True
        Me.LblRekening.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRekening.Location = New System.Drawing.Point(813, 38)
        Me.LblRekening.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblRekening.Name = "LblRekening"
        Me.LblRekening.Size = New System.Drawing.Size(42, 17)
        Me.LblRekening.TabIndex = 304
        Me.LblRekening.Text = "Kode"
        Me.LblRekening.Visible = False
        '
        'TxtTanggal
        '
        Me.TxtTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTanggal.ForeColor = System.Drawing.Color.Black
        Me.TxtTanggal.Location = New System.Drawing.Point(879, 7)
        Me.TxtTanggal.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TxtTanggal.Name = "TxtTanggal"
        Me.TxtTanggal.Size = New System.Drawing.Size(46, 23)
        Me.TxtTanggal.TabIndex = 285
        Me.TxtTanggal.Visible = False
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(198, 6)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(65, 25)
        Me.CmbThn.TabIndex = 266
        '
        'DtpAkhir
        '
        Me.DtpAkhir.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAkhir.Location = New System.Drawing.Point(692, 7)
        Me.DtpAkhir.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpAkhir.Name = "DtpAkhir"
        Me.DtpAkhir.Size = New System.Drawing.Size(111, 23)
        Me.DtpAkhir.TabIndex = 239
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(662, 10)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 17)
        Me.Label1.TabIndex = 238
        Me.Label1.Text = "s/d "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(438, 10)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(100, 17)
        Me.Label3.TabIndex = 227
        Me.Label3.Text = "Periode kerja :"
        '
        'DtpAwal
        '
        Me.DtpAwal.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAwal.Location = New System.Drawing.Point(548, 7)
        Me.DtpAwal.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpAwal.Name = "DtpAwal"
        Me.DtpAwal.Size = New System.Drawing.Size(111, 23)
        Me.DtpAwal.TabIndex = 228
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.Control
        Me.Panel2.Controls.Add(Me.Button1)
        Me.Panel2.Controls.Add(Me.ReportViewer1)
        Me.Panel2.Location = New System.Drawing.Point(98, 29)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(912, 487)
        Me.Panel2.TabIndex = 305
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.BackColor = System.Drawing.SystemColors.Control
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.Button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Image = CType(resources.GetObject("Button1.Image"), System.Drawing.Image)
        Me.Button1.Location = New System.Drawing.Point(875, 3)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(31, 31)
        Me.Button1.TabIndex = 1
        Me.Button1.UseVisualStyleBackColor = False
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.Gaji_karyawanBindingSource
        Me.ReportViewer1.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportGaji.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(912, 487)
        Me.ReportViewer1.TabIndex = 0
        '
        'FormGaji
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1194, 544)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelData)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.PanelNota)
        Me.Controls.Add(Me.Panel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormGaji"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormGaji"
        CType(Me.Gaji_karyawanBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.PanelNota.ResumeLayout(False)
        Me.PanelNota.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.PanelData.ResumeLayout(False)
        Me.PanelData.PerformLayout()
        CType(Me.DGVGaji, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents PanelNota As System.Windows.Forms.Panel
    Friend WithEvents LblKode As System.Windows.Forms.Label
    Friend WithEvents CmbNama As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents PanelData As System.Windows.Forms.Panel
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents DGVGaji As System.Windows.Forms.DataGridView
    Friend WithEvents CmbBln As System.Windows.Forms.ComboBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents DtpAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblNomor As System.Windows.Forms.Label
    Friend WithEvents DtpAkhir As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LblKetSupir As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents LblHelper As System.Windows.Forms.Label
    Friend WithEvents LblKetHelp As System.Windows.Forms.Label
    Friend WithEvents LblSupir As System.Windows.Forms.Label
    Friend WithEvents TxtTransport As System.Windows.Forms.TextBox
    Friend WithEvents TxtTunjangan As System.Windows.Forms.TextBox
    Friend WithEvents TxtPokok As System.Windows.Forms.TextBox
    Friend WithEvents TxtHelper As System.Windows.Forms.TextBox
    Friend WithEvents TxtSupir As System.Windows.Forms.TextBox
    Friend WithEvents TxtLembur As System.Windows.Forms.TextBox
    Friend WithEvents LblLembur As System.Windows.Forms.Label
    Friend WithEvents TxtLemburRp As System.Windows.Forms.TextBox
    Friend WithEvents TxtPotLain As System.Windows.Forms.TextBox
    Friend WithEvents TxtPotBon As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents TxtAbsenRp As System.Windows.Forms.TextBox
    Friend WithEvents TxtAbsen As System.Windows.Forms.TextBox
    Friend WithEvents LblAbsen As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents TxtTerima As System.Windows.Forms.TextBox
    Friend WithEvents TxtPotongan As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TxtPendapatan As System.Windows.Forms.TextBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents BtnSimpann As System.Windows.Forms.Button
    Friend WithEvents CmbThn As System.Windows.Forms.ComboBox
    Friend WithEvents TxtTanggal As System.Windows.Forms.TextBox
    Friend WithEvents LabelSupir As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents LblRekening As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Gaji_karyawanBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents Gaji_karyawanTableAdapter As AppKasir.PossDataSetLancarTableAdapters.Gaji_karyawanTableAdapter
    Friend WithEvents LblSaldoBon As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TxtPotBonUntukEdit As System.Windows.Forms.TextBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents TxtAngsuran As System.Windows.Forms.TextBox
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents TxtKomisiJual As System.Windows.Forms.TextBox
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents LblKomisJual As System.Windows.Forms.Label
    Friend WithEvents TxtOmsetJual As System.Windows.Forms.TextBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents TxtMakan As System.Windows.Forms.TextBox
    Friend WithEvents LblSisaBon As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents LblTelat As System.Windows.Forms.Label
    Friend WithEvents LblAbsenkhusus As System.Windows.Forms.Label
    Friend WithEvents TxtKeterlambatan As System.Windows.Forms.TextBox
    Friend WithEvents TxtKeterlambatanRp As System.Windows.Forms.TextBox
    Friend WithEvents TxtAbsenKhusus As System.Windows.Forms.TextBox
    Friend WithEvents TxtAbsenKhususRp As System.Windows.Forms.TextBox
    Friend WithEvents TxtPotAbsen As System.Windows.Forms.TextBox
    Friend WithEvents DtpTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents CmbPilihCetak As ComboBox
    Friend WithEvents BtnSettingPrinter As Button
    Friend WithEvents CmbProsesCetak As ComboBox
    Friend WithEvents ComboBox1 As ComboBox
End Class



