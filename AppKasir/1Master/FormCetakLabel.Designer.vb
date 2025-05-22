<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormCetakLabel
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCetakLabel))
        Dim DataGridViewCellStyle17 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle18 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnMinimize = New System.Windows.Forms.Button()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TxtLebarKertas = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtPanjangKertas = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.CmbJenisPrinter = New System.Windows.Forms.ComboBox()
        Me.CmbJenisKertas = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtbatasKiri = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TxtBatasAtas = New System.Windows.Forms.TextBox()
        Me.PanelThermal = New System.Windows.Forms.Panel()
        Me.ChkBoldSatuan = New System.Windows.Forms.CheckBox()
        Me.CmbFontSatuan = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranSatuan = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.CmbBentuklabel = New System.Windows.Forms.ComboBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.ChkBoldToko = New System.Windows.Forms.CheckBox()
        Me.ChkBoldHarga = New System.Windows.Forms.CheckBox()
        Me.ChkBoldNama = New System.Windows.Forms.CheckBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CmbFontToko = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranToko = New System.Windows.Forms.ComboBox()
        Me.CmbFontHarga = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranHarga = New System.Windows.Forms.ComboBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.TxtJarakX = New System.Windows.Forms.TextBox()
        Me.CmbFontNama = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranNama = New System.Windows.Forms.ComboBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.TxtJumlahPerBaris = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtJarakY = New System.Windows.Forms.TextBox()
        Me.TxtTinggiLabel = New System.Windows.Forms.TextBox()
        Me.DGVLabel = New System.Windows.Forms.DataGridView()
        Me.BtnPreview = New System.Windows.Forms.Button()
        Me.BtnCetak = New System.Windows.Forms.Button()
        Me.btnSimpanPerubahan = New System.Windows.Forms.Button()
        Me.BtnRestore = New System.Windows.Forms.Button()
        Me.BtnClear = New System.Windows.Forms.Button()
        Me.Kode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Nama = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Satuan = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Isi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Toko = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Gudang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hapus = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.PanelHeader.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.PanelThermal.SuspendLayout()
        CType(Me.DGVLabel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.DarkGray
        Me.PanelHeader.Controls.Add(Me.BtnMinimize)
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblUtama)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1132, 30)
        Me.PanelHeader.TabIndex = 40
        '
        'BtnMinimize
        '
        Me.BtnMinimize.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnMinimize.BackColor = System.Drawing.Color.DimGray
        Me.BtnMinimize.FlatAppearance.BorderSize = 0
        Me.BtnMinimize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(54, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(98, Byte), Integer))
        Me.BtnMinimize.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnMinimize.ForeColor = System.Drawing.Color.Blue
        Me.BtnMinimize.Image = CType(resources.GetObject("BtnMinimize.Image"), System.Drawing.Image)
        Me.BtnMinimize.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.BtnMinimize.Location = New System.Drawing.Point(1066, 3)
        Me.BtnMinimize.Name = "BtnMinimize"
        Me.BtnMinimize.Size = New System.Drawing.Size(23, 23)
        Me.BtnMinimize.TabIndex = 0
        Me.BtnMinimize.UseVisualStyleBackColor = False
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Red
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson
        Me.BtnClose.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.Blue
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(1095, 3)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(23, 23)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblUtama
        '
        Me.LblUtama.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblUtama.BackColor = System.Drawing.Color.Yellow
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.Color.Black
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(1132, 30)
        Me.LblUtama.TabIndex = 20
        Me.LblUtama.Text = "CETAK LABEL RAK"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.Label17)
        Me.Panel2.Controls.Add(Me.Label18)
        Me.Panel2.Controls.Add(Me.Label19)
        Me.Panel2.Controls.Add(Me.TxtLebarKertas)
        Me.Panel2.Controls.Add(Me.Label21)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.TxtPanjangKertas)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.Label15)
        Me.Panel2.Controls.Add(Me.CmbJenisPrinter)
        Me.Panel2.Controls.Add(Me.CmbJenisKertas)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label14)
        Me.Panel2.Controls.Add(Me.TxtbatasKiri)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.TxtBatasAtas)
        Me.Panel2.Location = New System.Drawing.Point(656, 45)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(476, 161)
        Me.Panel2.TabIndex = 2
        '
        'Label10
        '
        Me.Label10.BackColor = System.Drawing.Color.Silver
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(0, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(474, 23)
        Me.Label10.TabIndex = 208
        Me.Label10.Text = "Setting kertas"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.BackColor = System.Drawing.Color.Transparent
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.ForeColor = System.Drawing.Color.Black
        Me.Label17.Location = New System.Drawing.Point(189, 102)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(34, 17)
        Me.Label17.TabIndex = 207
        Me.Label17.Text = "mm"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.BackColor = System.Drawing.Color.Transparent
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(189, 128)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(34, 17)
        Me.Label18.TabIndex = 206
        Me.Label18.Text = "mm"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.BackColor = System.Drawing.Color.Transparent
        Me.Label19.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.ForeColor = System.Drawing.Color.Black
        Me.Label19.Location = New System.Drawing.Point(42, 128)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(52, 17)
        Me.Label19.TabIndex = 205
        Me.Label19.Text = "Lebar :"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtLebarKertas
        '
        Me.TxtLebarKertas.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLebarKertas.Location = New System.Drawing.Point(100, 125)
        Me.TxtLebarKertas.Name = "TxtLebarKertas"
        Me.TxtLebarKertas.Size = New System.Drawing.Size(83, 23)
        Me.TxtLebarKertas.TabIndex = 4
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.BackColor = System.Drawing.Color.Transparent
        Me.Label21.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.ForeColor = System.Drawing.Color.Black
        Me.Label21.Location = New System.Drawing.Point(24, 102)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(70, 17)
        Me.Label21.TabIndex = 203
        Me.Label21.Text = "Panjang :"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(425, 105)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(34, 17)
        Me.Label8.TabIndex = 194
        Me.Label8.Text = "mm"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPanjangKertas
        '
        Me.TxtPanjangKertas.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPanjangKertas.Location = New System.Drawing.Point(100, 99)
        Me.TxtPanjangKertas.Name = "TxtPanjangKertas"
        Me.TxtPanjangKertas.Size = New System.Drawing.Size(83, 23)
        Me.TxtPanjangKertas.TabIndex = 3
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(425, 131)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(34, 17)
        Me.Label9.TabIndex = 193
        Me.Label9.Text = "mm"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.Color.Transparent
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(6, 71)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(88, 17)
        Me.Label15.TabIndex = 200
        Me.Label15.Text = "Jenis kertas :"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbJenisPrinter
        '
        Me.CmbJenisPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisPrinter.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJenisPrinter.FormattingEnabled = True
        Me.CmbJenisPrinter.Location = New System.Drawing.Point(100, 36)
        Me.CmbJenisPrinter.Name = "CmbJenisPrinter"
        Me.CmbJenisPrinter.Size = New System.Drawing.Size(359, 25)
        Me.CmbJenisPrinter.TabIndex = 1
        '
        'CmbJenisKertas
        '
        Me.CmbJenisKertas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisKertas.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJenisKertas.FormattingEnabled = True
        Me.CmbJenisKertas.Location = New System.Drawing.Point(100, 68)
        Me.CmbJenisKertas.Name = "CmbJenisKertas"
        Me.CmbJenisKertas.Size = New System.Drawing.Size(163, 25)
        Me.CmbJenisKertas.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(4, 39)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(90, 17)
        Me.Label2.TabIndex = 95
        Me.Label2.Text = "Jenis Printer :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(269, 105)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(83, 17)
        Me.Label14.TabIndex = 123
        Me.Label14.Text = "Batas atas :"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtbatasKiri
        '
        Me.TxtbatasKiri.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtbatasKiri.Location = New System.Drawing.Point(358, 128)
        Me.TxtbatasKiri.Name = "TxtbatasKiri"
        Me.TxtbatasKiri.Size = New System.Drawing.Size(61, 23)
        Me.TxtbatasKiri.TabIndex = 6
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(279, 131)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(73, 17)
        Me.Label13.TabIndex = 121
        Me.Label13.Text = "Batas Kiri :"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtBatasAtas
        '
        Me.TxtBatasAtas.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBatasAtas.Location = New System.Drawing.Point(358, 102)
        Me.TxtBatasAtas.Name = "TxtBatasAtas"
        Me.TxtBatasAtas.Size = New System.Drawing.Size(61, 23)
        Me.TxtBatasAtas.TabIndex = 5
        '
        'PanelThermal
        '
        Me.PanelThermal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelThermal.Controls.Add(Me.ChkBoldSatuan)
        Me.PanelThermal.Controls.Add(Me.CmbFontSatuan)
        Me.PanelThermal.Controls.Add(Me.CmbUkuranSatuan)
        Me.PanelThermal.Controls.Add(Me.Label23)
        Me.PanelThermal.Controls.Add(Me.CmbBentuklabel)
        Me.PanelThermal.Controls.Add(Me.Label22)
        Me.PanelThermal.Controls.Add(Me.ChkBoldToko)
        Me.PanelThermal.Controls.Add(Me.ChkBoldHarga)
        Me.PanelThermal.Controls.Add(Me.ChkBoldNama)
        Me.PanelThermal.Controls.Add(Me.Label12)
        Me.PanelThermal.Controls.Add(Me.Label4)
        Me.PanelThermal.Controls.Add(Me.CmbFontToko)
        Me.PanelThermal.Controls.Add(Me.CmbUkuranToko)
        Me.PanelThermal.Controls.Add(Me.CmbFontHarga)
        Me.PanelThermal.Controls.Add(Me.CmbUkuranHarga)
        Me.PanelThermal.Controls.Add(Me.Label24)
        Me.PanelThermal.Controls.Add(Me.Label25)
        Me.PanelThermal.Controls.Add(Me.TxtJarakX)
        Me.PanelThermal.Controls.Add(Me.CmbFontNama)
        Me.PanelThermal.Controls.Add(Me.CmbUkuranNama)
        Me.PanelThermal.Controls.Add(Me.Label20)
        Me.PanelThermal.Controls.Add(Me.TxtJumlahPerBaris)
        Me.PanelThermal.Controls.Add(Me.Label1)
        Me.PanelThermal.Controls.Add(Me.Label6)
        Me.PanelThermal.Controls.Add(Me.TxtJarakY)
        Me.PanelThermal.Controls.Add(Me.TxtTinggiLabel)
        Me.PanelThermal.Location = New System.Drawing.Point(656, 212)
        Me.PanelThermal.Name = "PanelThermal"
        Me.PanelThermal.Size = New System.Drawing.Size(476, 354)
        Me.PanelThermal.TabIndex = 3
        '

        'ChkBoldSatuan
        '
        Me.ChkBoldSatuan.AutoSize = True
        Me.ChkBoldSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldSatuan.Location = New System.Drawing.Point(387, 211)
        Me.ChkBoldSatuan.Name = "ChkBoldSatuan"
        Me.ChkBoldSatuan.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldSatuan.TabIndex = 16
        Me.ChkBoldSatuan.Text = "Bold"
        Me.ChkBoldSatuan.UseVisualStyleBackColor = True
        '
        'CmbFontSatuan
        '
        Me.CmbFontSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontSatuan.FormattingEnabled = True
        Me.CmbFontSatuan.Location = New System.Drawing.Point(112, 209)
        Me.CmbFontSatuan.Name = "CmbFontSatuan"
        Me.CmbFontSatuan.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontSatuan.TabIndex = 14
        '
        'CmbUkuranSatuan
        '
        Me.CmbUkuranSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranSatuan.FormattingEnabled = True
        Me.CmbUkuranSatuan.Location = New System.Drawing.Point(329, 209)
        Me.CmbUkuranSatuan.Name = "CmbUkuranSatuan"
        Me.CmbUkuranSatuan.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranSatuan.TabIndex = 15
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.BackColor = System.Drawing.Color.Transparent
        Me.Label23.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.ForeColor = System.Drawing.Color.Black
        Me.Label23.Location = New System.Drawing.Point(16, 120)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(95, 17)
        Me.Label23.TabIndex = 211
        Me.Label23.Text = "Bentuk label :"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbBentuklabel
        '
        Me.CmbBentuklabel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBentuklabel.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBentuklabel.FormattingEnabled = True
        Me.CmbBentuklabel.Items.AddRange(New Object() {"Type 1", "Type 2", "Type 3"})
        Me.CmbBentuklabel.Location = New System.Drawing.Point(112, 116)
        Me.CmbBentuklabel.Name = "CmbBentuklabel"
        Me.CmbBentuklabel.Size = New System.Drawing.Size(321, 25)
        Me.CmbBentuklabel.TabIndex = 210
        '
        'Label22
        '
        Me.Label22.BackColor = System.Drawing.Color.Silver
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label22.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.ForeColor = System.Drawing.Color.Black
        Me.Label22.Location = New System.Drawing.Point(0, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(474, 20)
        Me.Label22.TabIndex = 209
        Me.Label22.Text = "Setting label"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ChkBoldToko
        '
        Me.ChkBoldToko.AutoSize = True
        Me.ChkBoldToko.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldToko.Location = New System.Drawing.Point(387, 242)
        Me.ChkBoldToko.Name = "ChkBoldToko"
        Me.ChkBoldToko.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldToko.TabIndex = 21
        Me.ChkBoldToko.Text = "Bold"
        Me.ChkBoldToko.UseVisualStyleBackColor = True
        '
        'ChkBoldHarga
        '
        Me.ChkBoldHarga.AutoSize = True
        Me.ChkBoldHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldHarga.Location = New System.Drawing.Point(387, 183)
        Me.ChkBoldHarga.Name = "ChkBoldHarga"
        Me.ChkBoldHarga.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldHarga.TabIndex = 12
        Me.ChkBoldHarga.Text = "Bold"
        Me.ChkBoldHarga.UseVisualStyleBackColor = True
        '
        'ChkBoldNama
        '
        Me.ChkBoldNama.AutoSize = True
        Me.ChkBoldNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldNama.Location = New System.Drawing.Point(387, 154)
        Me.ChkBoldNama.Name = "ChkBoldNama"
        Me.ChkBoldNama.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldNama.TabIndex = 8
        Me.ChkBoldNama.Text = "Bold"
        Me.ChkBoldNama.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(179, 63)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(34, 17)
        Me.Label12.TabIndex = 195
        Me.Label12.Text = "mm"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(220, 63)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(137, 17)
        Me.Label4.TabIndex = 192
        Me.Label4.Text = "Jarak Y Antar label :"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbFontToko
        '
        Me.CmbFontToko.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontToko.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontToko.FormattingEnabled = True
        Me.CmbFontToko.Location = New System.Drawing.Point(112, 240)
        Me.CmbFontToko.Name = "CmbFontToko"
        Me.CmbFontToko.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontToko.TabIndex = 19
        '
        'CmbUkuranToko
        '
        Me.CmbUkuranToko.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranToko.FormattingEnabled = True
        Me.CmbUkuranToko.Location = New System.Drawing.Point(329, 240)
        Me.CmbUkuranToko.Name = "CmbUkuranToko"
        Me.CmbUkuranToko.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranToko.TabIndex = 20
        '
        'CmbFontHarga
        '
        Me.CmbFontHarga.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontHarga.FormattingEnabled = True
        Me.CmbFontHarga.Location = New System.Drawing.Point(112, 181)
        Me.CmbFontHarga.Name = "CmbFontHarga"
        Me.CmbFontHarga.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontHarga.TabIndex = 10
        '
        'CmbUkuranHarga
        '
        Me.CmbUkuranHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranHarga.FormattingEnabled = True
        Me.CmbUkuranHarga.Location = New System.Drawing.Point(329, 181)
        Me.CmbUkuranHarga.Name = "CmbUkuranHarga"
        Me.CmbUkuranHarga.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranHarga.TabIndex = 11
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.BackColor = System.Drawing.Color.Transparent
        Me.Label24.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.ForeColor = System.Drawing.Color.Black
        Me.Label24.Location = New System.Drawing.Point(425, 34)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(34, 17)
        Me.Label24.TabIndex = 182
        Me.Label24.Text = "mm"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.BackColor = System.Drawing.Color.Transparent
        Me.Label25.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.ForeColor = System.Drawing.Color.Black
        Me.Label25.Location = New System.Drawing.Point(219, 34)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(138, 17)
        Me.Label25.TabIndex = 181
        Me.Label25.Text = "Jarak X Antar label :"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtJarakX
        '
        Me.TxtJarakX.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJarakX.Location = New System.Drawing.Point(358, 31)
        Me.TxtJarakX.Name = "TxtJarakX"
        Me.TxtJarakX.Size = New System.Drawing.Size(61, 23)
        Me.TxtJarakX.TabIndex = 3
        '
        'CmbFontNama
        '
        Me.CmbFontNama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontNama.FormattingEnabled = True
        Me.CmbFontNama.Location = New System.Drawing.Point(112, 152)
        Me.CmbFontNama.Name = "CmbFontNama"
        Me.CmbFontNama.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontNama.TabIndex = 6
        '
        'CmbUkuranNama
        '
        Me.CmbUkuranNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranNama.FormattingEnabled = True
        Me.CmbUkuranNama.Location = New System.Drawing.Point(329, 152)
        Me.CmbUkuranNama.Name = "CmbUkuranNama"
        Me.CmbUkuranNama.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranNama.TabIndex = 7
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.BackColor = System.Drawing.Color.Transparent
        Me.Label20.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.ForeColor = System.Drawing.Color.Black
        Me.Label20.Location = New System.Drawing.Point(11, 34)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(95, 17)
        Me.Label20.TabIndex = 178
        Me.Label20.Text = "Label / baris :"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtJumlahPerBaris
        '
        Me.TxtJumlahPerBaris.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJumlahPerBaris.Location = New System.Drawing.Point(112, 31)
        Me.TxtJumlahPerBaris.Name = "TxtJumlahPerBaris"
        Me.TxtJumlahPerBaris.Size = New System.Drawing.Size(61, 23)
        Me.TxtJumlahPerBaris.TabIndex = 1
        Me.TxtJumlahPerBaris.Text = "3"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(425, 63)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 17)
        Me.Label1.TabIndex = 176
        Me.Label1.Text = "mm"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(22, 63)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(89, 17)
        Me.Label6.TabIndex = 129
        Me.Label6.Text = "Tinggi label :"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtJarakY
        '
        Me.TxtJarakY.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJarakY.Location = New System.Drawing.Point(358, 60)
        Me.TxtJarakY.Name = "TxtJarakY"
        Me.TxtJarakY.Size = New System.Drawing.Size(61, 23)
        Me.TxtJarakY.TabIndex = 4
        '
        'TxtTinggiLabel
        '
        Me.TxtTinggiLabel.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTinggiLabel.Location = New System.Drawing.Point(112, 60)
        Me.TxtTinggiLabel.Name = "TxtTinggiLabel"
        Me.TxtTinggiLabel.Size = New System.Drawing.Size(61, 23)
        Me.TxtTinggiLabel.TabIndex = 2
        '
        'DGVLabel
        '
        Me.DGVLabel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVLabel.BackgroundColor = System.Drawing.Color.White
        Me.DGVLabel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVLabel.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Kode, Me.Nama, Me.Satuan, Me.Isi, Me.Toko, Me.Gudang, Me.Hapus})
        Me.DGVLabel.Location = New System.Drawing.Point(5, 45)
        Me.DGVLabel.Name = "DGVLabel"
        Me.DGVLabel.RowHeadersVisible = False
        Me.DGVLabel.Size = New System.Drawing.Size(645, 521)
        Me.DGVLabel.TabIndex = 1
        '
        'BtnPreview
        '
        Me.BtnPreview.AutoSize = True
        Me.BtnPreview.BackColor = System.Drawing.Color.Orange
        Me.BtnPreview.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnPreview.Image = CType(resources.GetObject("BtnPreview.Image"), System.Drawing.Image)
        Me.BtnPreview.Location = New System.Drawing.Point(424, 572)
        Me.BtnPreview.Name = "BtnPreview"
        Me.BtnPreview.Size = New System.Drawing.Size(86, 31)
        Me.BtnPreview.TabIndex = 4
        Me.BtnPreview.Text = "Preview"
        Me.BtnPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPreview.UseVisualStyleBackColor = False
        '
        'BtnCetak
        '
        Me.BtnCetak.AutoSize = True
        Me.BtnCetak.BackColor = System.Drawing.Color.Orange
        Me.BtnCetak.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnCetak.Image = CType(resources.GetObject("BtnCetak.Image"), System.Drawing.Image)
        Me.BtnCetak.Location = New System.Drawing.Point(565, 572)
        Me.BtnCetak.Name = "BtnCetak"
        Me.BtnCetak.Size = New System.Drawing.Size(85, 31)
        Me.BtnCetak.TabIndex = 5
        Me.BtnCetak.Text = "Cetak"
        Me.BtnCetak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCetak.UseVisualStyleBackColor = False
        '
        'btnSimpanPerubahan
        '
        Me.btnSimpanPerubahan.AutoSize = True
        Me.btnSimpanPerubahan.BackColor = System.Drawing.Color.Orange
        Me.btnSimpanPerubahan.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnSimpanPerubahan.Image = CType(resources.GetObject("btnSimpanPerubahan.Image"), System.Drawing.Image)
        Me.btnSimpanPerubahan.Location = New System.Drawing.Point(763, 572)
        Me.btnSimpanPerubahan.Name = "btnSimpanPerubahan"
        Me.btnSimpanPerubahan.Size = New System.Drawing.Size(146, 31)
        Me.btnSimpanPerubahan.TabIndex = 6
        Me.btnSimpanPerubahan.Text = "Simpan perubahan"
        Me.btnSimpanPerubahan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnSimpanPerubahan.UseVisualStyleBackColor = False
        '
        'BtnRestore
        '
        Me.BtnRestore.AutoSize = True
        Me.BtnRestore.BackColor = System.Drawing.Color.Orange
        Me.BtnRestore.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnRestore.Image = CType(resources.GetObject("BtnRestore.Image"), System.Drawing.Image)
        Me.BtnRestore.Location = New System.Drawing.Point(939, 572)
        Me.BtnRestore.Name = "BtnRestore"
        Me.BtnRestore.Size = New System.Drawing.Size(130, 31)
        Me.BtnRestore.TabIndex = 7
        Me.BtnRestore.Text = "Restore Default"
        Me.BtnRestore.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnRestore.UseVisualStyleBackColor = False
        '
        'BtnClear
        '
        Me.BtnClear.AutoSize = True
        Me.BtnClear.BackColor = System.Drawing.Color.Orange
        Me.BtnClear.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnClear.Image = CType(resources.GetObject("BtnClear.Image"), System.Drawing.Image)
        Me.BtnClear.Location = New System.Drawing.Point(38, 572)
        Me.BtnClear.Name = "BtnClear"
        Me.BtnClear.Size = New System.Drawing.Size(101, 31)
        Me.BtnClear.TabIndex = 41
        Me.BtnClear.Text = "Clear Data"
        Me.BtnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnClear.UseVisualStyleBackColor = False
        '
        'Kode
        '
        Me.Kode.HeaderText = "kode"
        Me.Kode.Name = "Kode"
        Me.Kode.Visible = False
        '
        'Nama
        '
        Me.Nama.FillWeight = 200.0!
        Me.Nama.HeaderText = "Nama"
        Me.Nama.Name = "Nama"
        '
        'Satuan
        '
        Me.Satuan.FillWeight = 50.0!
        Me.Satuan.HeaderText = "Satuan"
        Me.Satuan.Name = "Satuan"
        Me.Satuan.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Isi
        '
        Me.Isi.HeaderText = "isi"
        Me.Isi.Name = "Isi"
        Me.Isi.Visible = False
        '

        '
        'Toko
        '
        DataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle17.Format = "N0"
        DataGridViewCellStyle17.NullValue = Nothing
        Me.Toko.DefaultCellStyle = DataGridViewCellStyle17
        Me.Toko.FillWeight = 40.0!
        Me.Toko.HeaderText = "Toko"
        Me.Toko.Name = "Toko"
        '
        'Gudang
        '
        DataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle18.Format = "N0"
        DataGridViewCellStyle18.NullValue = Nothing
        Me.Gudang.DefaultCellStyle = DataGridViewCellStyle18
        Me.Gudang.FillWeight = 40.0!
        Me.Gudang.HeaderText = "Gudang"
        Me.Gudang.Name = "Gudang"
        '
        'Hapus
        '
        Me.Hapus.FillWeight = 40.0!
        Me.Hapus.HeaderText = "Hapus"
        Me.Hapus.Name = "Hapus"
        Me.Hapus.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Hapus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Hapus.Text = "X"
        Me.Hapus.UseColumnTextForButtonValue = True
        '
        'FormCetakLabel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(1132, 611)
        Me.Controls.Add(Me.BtnClear)
        Me.Controls.Add(Me.BtnRestore)
        Me.Controls.Add(Me.btnSimpanPerubahan)
        Me.Controls.Add(Me.PanelThermal)
        Me.Controls.Add(Me.BtnCetak)
        Me.Controls.Add(Me.BtnPreview)
        Me.Controls.Add(Me.DGVLabel)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormCetakLabel"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.PanelHeader.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.PanelThermal.ResumeLayout(False)
        Me.PanelThermal.PerformLayout()
        CType(Me.DGVLabel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents BtnMinimize As Button
    Friend WithEvents BtnClose As Button
    Friend WithEvents LblUtama As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents PanelThermal As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents CmbJenisPrinter As ComboBox
    Friend WithEvents Label25 As Label
    Friend WithEvents TxtJarakX As TextBox
    Friend WithEvents CmbFontNama As ComboBox
    Friend WithEvents CmbUkuranNama As ComboBox
    Friend WithEvents Label20 As Label
    Friend WithEvents TxtJumlahPerBaris As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtBatasAtas As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents TxtbatasKiri As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents TxtJarakY As TextBox
    Friend WithEvents TxtTinggiLabel As TextBox
    Friend WithEvents CmbFontToko As ComboBox
    Friend WithEvents CmbUkuranToko As ComboBox
    Friend WithEvents CmbFontHarga As ComboBox
    Friend WithEvents CmbUkuranHarga As ComboBox
    Friend WithEvents Label12 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents ChkBoldToko As CheckBox
    Friend WithEvents ChkBoldHarga As CheckBox
    Friend WithEvents ChkBoldNama As CheckBox
    Friend WithEvents DGVLabel As DataGridView
    Friend WithEvents Label15 As Label
    Friend WithEvents CmbJenisKertas As ComboBox
    Friend WithEvents BtnPreview As Button
    Friend WithEvents BtnCetak As Button
    Friend WithEvents btnSimpanPerubahan As Button
    Friend WithEvents Label17 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents TxtLebarKertas As TextBox
    Friend WithEvents Label21 As Label
    Friend WithEvents TxtPanjangKertas As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents Label23 As Label
    Friend WithEvents CmbBentuklabel As ComboBox
    Friend WithEvents ChkBoldSatuan As CheckBox
    Friend WithEvents CmbFontSatuan As ComboBox
    Friend WithEvents CmbUkuranSatuan As ComboBox

    Friend WithEvents BtnRestore As Button
    Friend WithEvents BtnClear As Button
    Friend WithEvents Kode As DataGridViewTextBoxColumn
    Friend WithEvents Nama As DataGridViewTextBoxColumn
    Friend WithEvents Satuan As DataGridViewComboBoxColumn
    Friend WithEvents Isi As DataGridViewTextBoxColumn
    Friend WithEvents Toko As DataGridViewTextBoxColumn
    Friend WithEvents Gudang As DataGridViewTextBoxColumn
    Friend WithEvents Hapus As DataGridViewButtonColumn
End Class
