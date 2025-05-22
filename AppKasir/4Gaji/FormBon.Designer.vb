<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormBon
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormBon))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.LblJenis = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.LblNomor = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LblBantuDKeuangan = New System.Windows.Forms.Label()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CmbNama = New System.Windows.Forms.ComboBox()
        Me.LblKode = New System.Windows.Forms.Label()
        Me.LblRekening = New System.Windows.Forms.Label()
        Me.LblNominal = New System.Windows.Forms.Label()
        Me.TxtNominal = New System.Windows.Forms.TextBox()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.LblSaldoBon = New System.Windows.Forms.Label()
        Me.LblSisaBon = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtKeterangan = New System.Windows.Forms.TextBox()
        Me.BtnSimpann = New System.Windows.Forms.Button()
        Me.PanelRinciKeuangan = New System.Windows.Forms.Panel()
        Me.LblTotalNominal = New System.Windows.Forms.Label()
        Me.DgvKeuangan = New System.Windows.Forms.DataGridView()
        Me.EDITKEUANGAN = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.HAPUSKEUANGAN = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.PanelHeader.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.PanelRinciKeuangan.SuspendLayout()
        CType(Me.DgvKeuangan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.SandyBrown
        Me.PanelHeader.Controls.Add(Me.LblJenis)
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblUtama)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1145, 34)
        Me.PanelHeader.TabIndex = 2
        '
        'LblJenis
        '
        Me.LblJenis.AutoSize = True
        Me.LblJenis.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenis.Location = New System.Drawing.Point(13, 12)
        Me.LblJenis.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblJenis.Name = "LblJenis"
        Me.LblJenis.Size = New System.Drawing.Size(39, 16)
        Me.LblJenis.TabIndex = 297
        Me.LblJenis.Text = "Jenis"
        Me.LblJenis.Visible = False
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.SandyBrown
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.ForeColor = System.Drawing.Color.Black
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(1111, 2)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 31)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblUtama
        '
        Me.LblUtama.BackColor = System.Drawing.Color.Transparent
        Me.LblUtama.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.Color.Black
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(1145, 34)
        Me.LblUtama.TabIndex = 20
        Me.LblUtama.Text = "BON KARYAWAN"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblNomor
        '
        Me.LblNomor.AutoSize = True
        Me.LblNomor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNomor.Location = New System.Drawing.Point(70, 40)
        Me.LblNomor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblNomor.Name = "LblNomor"
        Me.LblNomor.Size = New System.Drawing.Size(48, 16)
        Me.LblNomor.TabIndex = 237
        Me.LblNomor.Text = "Nomor"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(13, 40)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 16)
        Me.Label2.TabIndex = 226
        Me.Label2.Text = "Nomor :"
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.CustomFormat = "yyyy/MM/dd"
        Me.DtpTanggal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(70, 8)
        Me.DtpTanggal.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(108, 22)
        Me.DtpTanggal.TabIndex = 286
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 11)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 16)
        Me.Label1.TabIndex = 287
        Me.Label1.Text = "Tanggal :"
        '
        'LblBantuDKeuangan
        '
        Me.LblBantuDKeuangan.AutoSize = True
        Me.LblBantuDKeuangan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBantuDKeuangan.Location = New System.Drawing.Point(234, 40)
        Me.LblBantuDKeuangan.Name = "LblBantuDKeuangan"
        Me.LblBantuDKeuangan.Size = New System.Drawing.Size(71, 16)
        Me.LblBantuDKeuangan.TabIndex = 292
        Me.LblBantuDKeuangan.Text = "Rekening :"
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Location = New System.Drawing.Point(310, 36)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(174, 24)
        Me.CmbRekening.TabIndex = 2
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(255, 11)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(50, 16)
        Me.Label6.TabIndex = 295
        Me.Label6.Text = "Nama :"
        '
        'CmbNama
        '
        Me.CmbNama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbNama.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbNama.FormattingEnabled = True
        Me.CmbNama.Location = New System.Drawing.Point(310, 7)
        Me.CmbNama.Name = "CmbNama"
        Me.CmbNama.Size = New System.Drawing.Size(174, 24)
        Me.CmbNama.TabIndex = 1
        '
        'LblKode
        '
        Me.LblKode.AutoSize = True
        Me.LblKode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKode.Location = New System.Drawing.Point(491, 11)
        Me.LblKode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKode.Name = "LblKode"
        Me.LblKode.Size = New System.Drawing.Size(39, 16)
        Me.LblKode.TabIndex = 296
        Me.LblKode.Text = "Kode"
        Me.LblKode.Visible = False
        '
        'LblRekening
        '
        Me.LblRekening.AutoSize = True
        Me.LblRekening.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRekening.Location = New System.Drawing.Point(491, 40)
        Me.LblRekening.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblRekening.Name = "LblRekening"
        Me.LblRekening.Size = New System.Drawing.Size(39, 16)
        Me.LblRekening.TabIndex = 297
        Me.LblRekening.Text = "Kode"
        Me.LblRekening.Visible = False
        '
        'LblNominal
        '
        Me.LblNominal.AutoSize = True
        Me.LblNominal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNominal.Location = New System.Drawing.Point(401, 70)
        Me.LblNominal.Name = "LblNominal"
        Me.LblNominal.Size = New System.Drawing.Size(38, 16)
        Me.LblNominal.TabIndex = 300
        Me.LblNominal.Text = "Rp. 0"
        Me.LblNominal.Visible = False
        '
        'TxtNominal
        '
        Me.TxtNominal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNominal.Location = New System.Drawing.Point(659, 37)
        Me.TxtNominal.Name = "TxtNominal"
        Me.TxtNominal.Size = New System.Drawing.Size(101, 22)
        Me.TxtNominal.TabIndex = 3
        Me.TxtNominal.Text = "0"
        Me.TxtNominal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label71
        '
        Me.Label71.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label71.Location = New System.Drawing.Point(534, 40)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(117, 16)
        Me.Label71.TabIndex = 299
        Me.Label71.Text = "Nominal :"
        Me.Label71.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblSaldoBon
        '
        Me.LblSaldoBon.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSaldoBon.Location = New System.Drawing.Point(659, 10)
        Me.LblSaldoBon.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSaldoBon.Name = "LblSaldoBon"
        Me.LblSaldoBon.Size = New System.Drawing.Size(101, 19)
        Me.LblSaldoBon.TabIndex = 301
        Me.LblSaldoBon.Text = "Saldo"
        Me.LblSaldoBon.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblSisaBon
        '
        Me.LblSisaBon.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSisaBon.Location = New System.Drawing.Point(659, 68)
        Me.LblSisaBon.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSisaBon.Name = "LblSisaBon"
        Me.LblSisaBon.Size = New System.Drawing.Size(101, 20)
        Me.LblSisaBon.TabIndex = 302
        Me.LblSisaBon.Text = "Sisa"
        Me.LblSisaBon.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(532, 11)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(119, 16)
        Me.Label5.TabIndex = 304
        Me.Label5.Text = "Saldo bon :"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(537, 70)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(114, 16)
        Me.Label8.TabIndex = 305
        Me.Label8.Text = "Total bon :"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.BackColor = System.Drawing.Color.OldLace
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.TxtKeterangan)
        Me.Panel1.Controls.Add(Me.BtnSimpann)
        Me.Panel1.Controls.Add(Me.DtpTanggal)
        Me.Panel1.Controls.Add(Me.LblRekening)
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Controls.Add(Me.LblBantuDKeuangan)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.CmbRekening)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.LblNomor)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.LblSisaBon)
        Me.Panel1.Controls.Add(Me.CmbNama)
        Me.Panel1.Controls.Add(Me.LblSaldoBon)
        Me.Panel1.Controls.Add(Me.LblKode)
        Me.Panel1.Controls.Add(Me.LblNominal)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.TxtNominal)
        Me.Panel1.Controls.Add(Me.Label71)
        Me.Panel1.Location = New System.Drawing.Point(0, 37)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1142, 127)
        Me.Panel1.TabIndex = 306
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(534, 95)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(117, 16)
        Me.Label3.TabIndex = 308
        Me.Label3.Text = "Keterangan :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtKeterangan
        '
        Me.TxtKeterangan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKeterangan.Location = New System.Drawing.Point(657, 92)
        Me.TxtKeterangan.Name = "TxtKeterangan"
        Me.TxtKeterangan.Size = New System.Drawing.Size(401, 22)
        Me.TxtKeterangan.TabIndex = 4
        '
        'BtnSimpann
        '
        Me.BtnSimpann.BackColor = System.Drawing.Color.Khaki
        Me.BtnSimpann.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Blue
        Me.BtnSimpann.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Blue
        Me.BtnSimpann.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpann.Font = New System.Drawing.Font("Bookman Old Style", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpann.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpann.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpann.Location = New System.Drawing.Point(923, 24)
        Me.BtnSimpann.Name = "BtnSimpann"
        Me.BtnSimpann.Size = New System.Drawing.Size(135, 32)
        Me.BtnSimpann.TabIndex = 5
        Me.BtnSimpann.Text = "SIMPAN (F8)"
        Me.BtnSimpann.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.BtnSimpann.UseVisualStyleBackColor = False
        '
        'PanelRinciKeuangan
        '
        Me.PanelRinciKeuangan.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelRinciKeuangan.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.PanelRinciKeuangan.Controls.Add(Me.LblTotalNominal)
        Me.PanelRinciKeuangan.Controls.Add(Me.DgvKeuangan)
        Me.PanelRinciKeuangan.Location = New System.Drawing.Point(0, 170)
        Me.PanelRinciKeuangan.Name = "PanelRinciKeuangan"
        Me.PanelRinciKeuangan.Size = New System.Drawing.Size(1142, 342)
        Me.PanelRinciKeuangan.TabIndex = 307
        '
        'LblTotalNominal
        '
        Me.LblTotalNominal.AutoSize = True
        Me.LblTotalNominal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblTotalNominal.Location = New System.Drawing.Point(11, 7)
        Me.LblTotalNominal.Name = "LblTotalNominal"
        Me.LblTotalNominal.Size = New System.Drawing.Size(60, 16)
        Me.LblTotalNominal.TabIndex = 207
        Me.LblTotalNominal.Text = "No Nota"
        '
        'DgvKeuangan
        '
        Me.DgvKeuangan.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvKeuangan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvKeuangan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvKeuangan.BackgroundColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvKeuangan.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvKeuangan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvKeuangan.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.EDITKEUANGAN, Me.HAPUSKEUANGAN})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvKeuangan.DefaultCellStyle = DataGridViewCellStyle2
        Me.DgvKeuangan.Location = New System.Drawing.Point(1, 29)
        Me.DgvKeuangan.Name = "DgvKeuangan"
        Me.DgvKeuangan.ReadOnly = True
        Me.DgvKeuangan.RowHeadersVisible = False
        Me.DgvKeuangan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvKeuangan.Size = New System.Drawing.Size(1139, 311)
        Me.DgvKeuangan.TabIndex = 6
        '
        'EDITKEUANGAN
        '
        Me.EDITKEUANGAN.FillWeight = 50.0!
        Me.EDITKEUANGAN.HeaderText = "Edit"
        Me.EDITKEUANGAN.Name = "EDITKEUANGAN"
        Me.EDITKEUANGAN.ReadOnly = True
        Me.EDITKEUANGAN.Text = "Edit"
        Me.EDITKEUANGAN.UseColumnTextForButtonValue = True
        '
        'HAPUSKEUANGAN
        '
        Me.HAPUSKEUANGAN.FillWeight = 50.0!
        Me.HAPUSKEUANGAN.HeaderText = "Hapus"
        Me.HAPUSKEUANGAN.Name = "HAPUSKEUANGAN"
        Me.HAPUSKEUANGAN.ReadOnly = True
        Me.HAPUSKEUANGAN.Text = "Hapus"
        Me.HAPUSKEUANGAN.UseColumnTextForButtonValue = True
        '
        'FormBon
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1145, 524)
        Me.Controls.Add(Me.PanelRinciKeuangan)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormBon"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormBon"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.PanelRinciKeuangan.ResumeLayout(False)
        Me.PanelRinciKeuangan.PerformLayout()
        CType(Me.DgvKeuangan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblUtama As System.Windows.Forms.Label
    Friend WithEvents LblNomor As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents DtpTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LblBantuDKeuangan As System.Windows.Forms.Label
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents CmbNama As System.Windows.Forms.ComboBox
    Friend WithEvents LblKode As System.Windows.Forms.Label
    Friend WithEvents LblRekening As System.Windows.Forms.Label
    Friend WithEvents LblNominal As System.Windows.Forms.Label
    Friend WithEvents TxtNominal As System.Windows.Forms.TextBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents LblSaldoBon As System.Windows.Forms.Label
    Friend WithEvents LblSisaBon As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PanelRinciKeuangan As System.Windows.Forms.Panel
    Friend WithEvents LblTotalNominal As System.Windows.Forms.Label
    Friend WithEvents DgvKeuangan As System.Windows.Forms.DataGridView
    Friend WithEvents BtnSimpann As System.Windows.Forms.Button
    Friend WithEvents LblJenis As System.Windows.Forms.Label
    Friend WithEvents KodeRek As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NamaRek As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents EDITKEUANGAN As System.Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents HAPUSKEUANGAN As System.Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents TxtKeterangan As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
