<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormCetakBarcode
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCetakBarcode))
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnMinimize = New System.Windows.Forms.Button()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.CmbJenisKertas = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.CmbJenisPrinter = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtLebarKertas = New System.Windows.Forms.TextBox()
        Me.TxtTinggiKertas = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtTinggiBarcode = New System.Windows.Forms.TextBox()
        Me.TxtLebarBarcode = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CmbJumlahBarcodePerBaris = New System.Windows.Forms.ComboBox()
        Me.CmbTipeBarcode = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TxtStartY = New System.Windows.Forms.TextBox()
        Me.TxtStartX = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TxtGabY = New System.Windows.Forms.TextBox()
        Me.TxtGabX = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.ChkBoldSatuan = New System.Windows.Forms.CheckBox()
        Me.CmbFontSatuan = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranSatuan = New System.Windows.Forms.ComboBox()
        Me.ChkBoldToko = New System.Windows.Forms.CheckBox()
        Me.ChkBoldHarga = New System.Windows.Forms.CheckBox()
        Me.ChkBoldNama = New System.Windows.Forms.CheckBox()
        Me.CmbFontToko = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranToko = New System.Windows.Forms.ComboBox()
        Me.CmbFontHarga = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranHarga = New System.Windows.Forms.ComboBox()
        Me.CmbFontNama = New System.Windows.Forms.ComboBox()
        Me.CmbUkuranNama = New System.Windows.Forms.ComboBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtJumlahCetak = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TxtKodeBarcode = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.CmbSatuan = New System.Windows.Forms.ComboBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TxtNamaBarang = New System.Windows.Forms.TextBox()
        Me.TxtHargaBarang = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.PicPreviewBarcode3 = New System.Windows.Forms.PictureBox()
        Me.PicPreviewBarcode2 = New System.Windows.Forms.PictureBox()
        Me.PicPreviewBarcode1 = New System.Windows.Forms.PictureBox()
        Me.PicPreviewBarcode0 = New System.Windows.Forms.PictureBox()
        Me.BtnRestore = New System.Windows.Forms.Button()
        Me.btnSimpanPerubahan = New System.Windows.Forms.Button()
        Me.BtnCetakBarcode = New System.Windows.Forms.Button()
        Me.BtnPreviewCetak = New System.Windows.Forms.Button()
        Me.BtnResetGap = New System.Windows.Forms.Button()
        Me.BtnResetPosisiXY = New System.Windows.Forms.Button()
        Me.BtnExportBarcode = New System.Windows.Forms.Button()
        Me.PanelHeader.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        CType(Me.PicPreviewBarcode3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicPreviewBarcode2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicPreviewBarcode1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicPreviewBarcode0, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.PanelHeader.Size = New System.Drawing.Size(1089, 30)
        Me.PanelHeader.TabIndex = 39
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
        Me.BtnMinimize.Location = New System.Drawing.Point(1023, 3)
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
        Me.BtnClose.Location = New System.Drawing.Point(1052, 3)
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
        Me.LblUtama.Size = New System.Drawing.Size(1089, 30)
        Me.LblUtama.TabIndex = 20
        Me.LblUtama.Text = "CETAK BARCODE"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Transparent
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.CmbJenisKertas)
        Me.Panel2.Controls.Add(Me.Label14)
        Me.Panel2.Controls.Add(Me.CmbJenisPrinter)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.TxtLebarKertas)
        Me.Panel2.Controls.Add(Me.TxtTinggiKertas)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Location = New System.Drawing.Point(5, 46)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(503, 68)
        Me.Panel2.TabIndex = 67
        '
        'CmbJenisKertas
        '
        Me.CmbJenisKertas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisKertas.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbJenisKertas.FormattingEnabled = True
        Me.CmbJenisKertas.Location = New System.Drawing.Point(97, 37)
        Me.CmbJenisKertas.Name = "CmbJenisKertas"
        Me.CmbJenisKertas.Size = New System.Drawing.Size(177, 25)
        Me.CmbJenisKertas.TabIndex = 82
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(4, 41)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(89, 17)
        Me.Label14.TabIndex = 81
        Me.Label14.Text = "Jenis Kertas :"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbJenisPrinter
        '
        Me.CmbJenisPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisPrinter.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbJenisPrinter.FormattingEnabled = True
        Me.CmbJenisPrinter.Location = New System.Drawing.Point(97, 8)
        Me.CmbJenisPrinter.Name = "CmbJenisPrinter"
        Me.CmbJenisPrinter.Size = New System.Drawing.Size(177, 25)
        Me.CmbJenisPrinter.TabIndex = 66
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(3, 12)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(90, 17)
        Me.Label7.TabIndex = 25
        Me.Label7.Text = "Jenis Printer :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtLebarKertas
        '
        Me.TxtLebarKertas.BackColor = System.Drawing.SystemColors.Window
        Me.TxtLebarKertas.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtLebarKertas.Location = New System.Drawing.Point(397, 9)
        Me.TxtLebarKertas.Name = "TxtLebarKertas"
        Me.TxtLebarKertas.Size = New System.Drawing.Size(58, 23)
        Me.TxtLebarKertas.TabIndex = 0
        '
        'TxtTinggiKertas
        '
        Me.TxtTinggiKertas.BackColor = System.Drawing.SystemColors.Window
        Me.TxtTinggiKertas.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtTinggiKertas.Location = New System.Drawing.Point(397, 38)
        Me.TxtTinggiKertas.Name = "TxtTinggiKertas"
        Me.TxtTinggiKertas.Size = New System.Drawing.Size(58, 23)
        Me.TxtTinggiKertas.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(304, 41)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 17)
        Me.Label2.TabIndex = 16
        Me.Label2.Text = "Tinggi Kertas"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(306, 12)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(87, 17)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Lebar Kertas"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtTinggiBarcode
        '
        Me.TxtTinggiBarcode.BackColor = System.Drawing.SystemColors.Window
        Me.TxtTinggiBarcode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtTinggiBarcode.Location = New System.Drawing.Point(132, 100)
        Me.TxtTinggiBarcode.Name = "TxtTinggiBarcode"
        Me.TxtTinggiBarcode.Size = New System.Drawing.Size(75, 23)
        Me.TxtTinggiBarcode.TabIndex = 4
        '
        'TxtLebarBarcode
        '
        Me.TxtLebarBarcode.BackColor = System.Drawing.SystemColors.Window
        Me.TxtLebarBarcode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtLebarBarcode.Location = New System.Drawing.Point(132, 73)
        Me.TxtLebarBarcode.Name = "TxtLebarBarcode"
        Me.TxtLebarBarcode.Size = New System.Drawing.Size(75, 23)
        Me.TxtLebarBarcode.TabIndex = 2
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(221, 103)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(48, 17)
        Me.Label8.TabIndex = 22
        Me.Label8.Text = "Start Y"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(220, 76)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 17)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Strat X"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbJumlahBarcodePerBaris
        '
        Me.CmbJumlahBarcodePerBaris.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJumlahBarcodePerBaris.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbJumlahBarcodePerBaris.FormattingEnabled = True
        Me.CmbJumlahBarcodePerBaris.Location = New System.Drawing.Point(132, 15)
        Me.CmbJumlahBarcodePerBaris.Name = "CmbJumlahBarcodePerBaris"
        Me.CmbJumlahBarcodePerBaris.Size = New System.Drawing.Size(357, 25)
        Me.CmbJumlahBarcodePerBaris.TabIndex = 68
        '
        'CmbTipeBarcode
        '
        Me.CmbTipeBarcode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbTipeBarcode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbTipeBarcode.FormattingEnabled = True
        Me.CmbTipeBarcode.Location = New System.Drawing.Point(132, 44)
        Me.CmbTipeBarcode.Name = "CmbTipeBarcode"
        Me.CmbTipeBarcode.Size = New System.Drawing.Size(357, 25)
        Me.CmbTipeBarcode.TabIndex = 83
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(29, 10)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(98, 17)
        Me.Label4.TabIndex = 85
        Me.Label4.Text = "Fonta Nama :"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(13, 18)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(113, 17)
        Me.Label6.TabIndex = 84
        Me.Label6.Text = "Jumlah Barcode"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(39, 40)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(88, 17)
        Me.Label9.TabIndex = 87
        Me.Label9.Text = "Font Harga :"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(34, 69)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(93, 17)
        Me.Label11.TabIndex = 89
        Me.Label11.Text = "Font Satuan :"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(32, 47)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(94, 17)
        Me.Label13.TabIndex = 91
        Me.Label13.Text = "Type Barcode"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtStartY
        '
        Me.TxtStartY.BackColor = System.Drawing.SystemColors.Window
        Me.TxtStartY.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtStartY.Location = New System.Drawing.Point(271, 100)
        Me.TxtStartY.Name = "TxtStartY"
        Me.TxtStartY.Size = New System.Drawing.Size(75, 23)
        Me.TxtStartY.TabIndex = 93
        '
        'TxtStartX
        '
        Me.TxtStartX.BackColor = System.Drawing.SystemColors.Window
        Me.TxtStartX.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtStartX.Location = New System.Drawing.Point(271, 73)
        Me.TxtStartX.Name = "TxtStartX"
        Me.TxtStartX.Size = New System.Drawing.Size(75, 23)
        Me.TxtStartX.TabIndex = 92
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(23, 102)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(103, 17)
        Me.Label10.TabIndex = 95
        Me.Label10.Text = "Tinggi Barcode"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(24, 75)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(102, 17)
        Me.Label12.TabIndex = 94
        Me.Label12.Text = "Lebar Barcode"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtGabY
        '
        Me.TxtGabY.BackColor = System.Drawing.SystemColors.Window
        Me.TxtGabY.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtGabY.Location = New System.Drawing.Point(414, 100)
        Me.TxtGabY.Name = "TxtGabY"
        Me.TxtGabY.Size = New System.Drawing.Size(75, 23)
        Me.TxtGabY.TabIndex = 99
        '
        'TxtGabX
        '
        Me.TxtGabX.BackColor = System.Drawing.SystemColors.Window
        Me.TxtGabX.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtGabX.Location = New System.Drawing.Point(414, 73)
        Me.TxtGabX.Name = "TxtGabX"
        Me.TxtGabX.Size = New System.Drawing.Size(75, 23)
        Me.TxtGabX.TabIndex = 98
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(364, 103)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(48, 17)
        Me.Label15.TabIndex = 97
        Me.Label15.Text = "Gab Y"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label16.ForeColor = System.Drawing.Color.Black
        Me.Label16.Location = New System.Drawing.Point(363, 76)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(49, 17)
        Me.Label16.TabIndex = 96
        Me.Label16.Text = "Gab X"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ChkBoldSatuan
        '
        Me.ChkBoldSatuan.AutoSize = True
        Me.ChkBoldSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldSatuan.Location = New System.Drawing.Point(409, 67)
        Me.ChkBoldSatuan.Name = "ChkBoldSatuan"
        Me.ChkBoldSatuan.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldSatuan.TabIndex = 76
        Me.ChkBoldSatuan.Text = "Bold"
        Me.ChkBoldSatuan.UseVisualStyleBackColor = True
        '
        'CmbFontSatuan
        '
        Me.CmbFontSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontSatuan.FormattingEnabled = True
        Me.CmbFontSatuan.Location = New System.Drawing.Point(134, 65)
        Me.CmbFontSatuan.Name = "CmbFontSatuan"
        Me.CmbFontSatuan.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontSatuan.TabIndex = 74
        '
        'CmbUkuranSatuan
        '
        Me.CmbUkuranSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranSatuan.FormattingEnabled = True
        Me.CmbUkuranSatuan.Location = New System.Drawing.Point(351, 65)
        Me.CmbUkuranSatuan.Name = "CmbUkuranSatuan"
        Me.CmbUkuranSatuan.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranSatuan.TabIndex = 75
        '
        'ChkBoldToko
        '
        Me.ChkBoldToko.AutoSize = True
        Me.ChkBoldToko.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldToko.Location = New System.Drawing.Point(409, 97)
        Me.ChkBoldToko.Name = "ChkBoldToko"
        Me.ChkBoldToko.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldToko.TabIndex = 79
        Me.ChkBoldToko.Text = "Bold"
        Me.ChkBoldToko.UseVisualStyleBackColor = True
        '
        'ChkBoldHarga
        '
        Me.ChkBoldHarga.AutoSize = True
        Me.ChkBoldHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldHarga.Location = New System.Drawing.Point(409, 38)
        Me.ChkBoldHarga.Name = "ChkBoldHarga"
        Me.ChkBoldHarga.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldHarga.TabIndex = 73
        Me.ChkBoldHarga.Text = "Bold"
        Me.ChkBoldHarga.UseVisualStyleBackColor = True
        '
        'ChkBoldNama
        '
        Me.ChkBoldNama.AutoSize = True
        Me.ChkBoldNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.ChkBoldNama.Location = New System.Drawing.Point(409, 8)
        Me.ChkBoldNama.Name = "ChkBoldNama"
        Me.ChkBoldNama.Size = New System.Drawing.Size(55, 21)
        Me.ChkBoldNama.TabIndex = 70
        Me.ChkBoldNama.Text = "Bold"
        Me.ChkBoldNama.UseVisualStyleBackColor = True
        '
        'CmbFontToko
        '
        Me.CmbFontToko.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontToko.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontToko.FormattingEnabled = True
        Me.CmbFontToko.Location = New System.Drawing.Point(134, 95)
        Me.CmbFontToko.Name = "CmbFontToko"
        Me.CmbFontToko.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontToko.TabIndex = 77
        '
        'CmbUkuranToko
        '
        Me.CmbUkuranToko.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranToko.FormattingEnabled = True
        Me.CmbUkuranToko.Location = New System.Drawing.Point(351, 95)
        Me.CmbUkuranToko.Name = "CmbUkuranToko"
        Me.CmbUkuranToko.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranToko.TabIndex = 78
        '
        'CmbFontHarga
        '
        Me.CmbFontHarga.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontHarga.FormattingEnabled = True
        Me.CmbFontHarga.Location = New System.Drawing.Point(134, 36)
        Me.CmbFontHarga.Name = "CmbFontHarga"
        Me.CmbFontHarga.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontHarga.TabIndex = 71
        '
        'CmbUkuranHarga
        '
        Me.CmbUkuranHarga.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranHarga.FormattingEnabled = True
        Me.CmbUkuranHarga.Location = New System.Drawing.Point(351, 36)
        Me.CmbUkuranHarga.Name = "CmbUkuranHarga"
        Me.CmbUkuranHarga.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranHarga.TabIndex = 72
        '
        'CmbFontNama
        '
        Me.CmbFontNama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbFontNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbFontNama.FormattingEnabled = True
        Me.CmbFontNama.Location = New System.Drawing.Point(134, 6)
        Me.CmbFontNama.Name = "CmbFontNama"
        Me.CmbFontNama.Size = New System.Drawing.Size(212, 25)
        Me.CmbFontNama.TabIndex = 68
        '
        'CmbUkuranNama
        '
        Me.CmbUkuranNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbUkuranNama.FormattingEnabled = True
        Me.CmbUkuranNama.Location = New System.Drawing.Point(351, 6)
        Me.CmbUkuranNama.Name = "CmbUkuranNama"
        Me.CmbUkuranNama.Size = New System.Drawing.Size(51, 25)
        Me.CmbUkuranNama.TabIndex = 69
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label17)
        Me.Panel1.Controls.Add(Me.CmbFontNama)
        Me.Panel1.Controls.Add(Me.Label11)
        Me.Panel1.Controls.Add(Me.ChkBoldSatuan)
        Me.Panel1.Controls.Add(Me.Label9)
        Me.Panel1.Controls.Add(Me.CmbUkuranNama)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.CmbFontSatuan)
        Me.Panel1.Controls.Add(Me.CmbUkuranHarga)
        Me.Panel1.Controls.Add(Me.CmbUkuranSatuan)
        Me.Panel1.Controls.Add(Me.CmbFontHarga)
        Me.Panel1.Controls.Add(Me.ChkBoldToko)
        Me.Panel1.Controls.Add(Me.CmbUkuranToko)
        Me.Panel1.Controls.Add(Me.ChkBoldHarga)
        Me.Panel1.Controls.Add(Me.CmbFontToko)
        Me.Panel1.Controls.Add(Me.ChkBoldNama)
        Me.Panel1.Location = New System.Drawing.Point(5, 120)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(503, 129)
        Me.Panel1.TabIndex = 90
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label17.ForeColor = System.Drawing.Color.Black
        Me.Label17.Location = New System.Drawing.Point(49, 99)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(78, 17)
        Me.Label17.TabIndex = 90
        Me.Label17.Text = "Font Toko :"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.TxtGabY)
        Me.Panel3.Controls.Add(Me.CmbJumlahBarcodePerBaris)
        Me.Panel3.Controls.Add(Me.TxtGabX)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.Label15)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Controls.Add(Me.Label16)
        Me.Panel3.Controls.Add(Me.TxtLebarBarcode)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.TxtTinggiBarcode)
        Me.Panel3.Controls.Add(Me.Label12)
        Me.Panel3.Controls.Add(Me.CmbTipeBarcode)
        Me.Panel3.Controls.Add(Me.TxtStartY)
        Me.Panel3.Controls.Add(Me.Label6)
        Me.Panel3.Controls.Add(Me.TxtStartX)
        Me.Panel3.Controls.Add(Me.Label13)
        Me.Panel3.Location = New System.Drawing.Point(5, 255)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(503, 133)
        Me.Panel3.TabIndex = 91
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.Transparent
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.LstBarang)
        Me.Panel4.Controls.Add(Me.Label3)
        Me.Panel4.Controls.Add(Me.TxtJumlahCetak)
        Me.Panel4.Controls.Add(Me.Label18)
        Me.Panel4.Controls.Add(Me.TxtKodeBarcode)
        Me.Panel4.Controls.Add(Me.TxtKode)
        Me.Panel4.Controls.Add(Me.CmbSatuan)
        Me.Panel4.Controls.Add(Me.Label19)
        Me.Panel4.Controls.Add(Me.TxtNamaBarang)
        Me.Panel4.Controls.Add(Me.TxtHargaBarang)
        Me.Panel4.Controls.Add(Me.Label20)
        Me.Panel4.Controls.Add(Me.Label21)
        Me.Panel4.Location = New System.Drawing.Point(514, 46)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(563, 155)
        Me.Panel4.TabIndex = 92
        '
        'LstBarang
        '
        Me.LstBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LstBarang.FormattingEnabled = True
        Me.LstBarang.ItemHeight = 17
        Me.LstBarang.Location = New System.Drawing.Point(114, 32)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(440, 123)
        Me.LstBarang.TabIndex = 72
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(3, 102)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(103, 17)
        Me.Label3.TabIndex = 71
        Me.Label3.Text = "Code Barcode"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtJumlahCetak
        '
        Me.TxtJumlahCetak.BackColor = System.Drawing.SystemColors.Window
        Me.TxtJumlahCetak.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtJumlahCetak.Location = New System.Drawing.Point(114, 127)
        Me.TxtJumlahCetak.Name = "TxtJumlahCetak"
        Me.TxtJumlahCetak.Size = New System.Drawing.Size(66, 23)
        Me.TxtJumlahCetak.TabIndex = 69
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(7, 130)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(99, 17)
        Me.Label18.TabIndex = 70
        Me.Label18.Text = "Jumlah Cetak"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtKodeBarcode
        '
        Me.TxtKodeBarcode.BackColor = System.Drawing.SystemColors.Window
        Me.TxtKodeBarcode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKodeBarcode.Location = New System.Drawing.Point(114, 99)
        Me.TxtKodeBarcode.Name = "TxtKodeBarcode"
        Me.TxtKodeBarcode.Size = New System.Drawing.Size(177, 23)
        Me.TxtKodeBarcode.TabIndex = 68
        '
        'TxtKode
        '
        Me.TxtKode.BackColor = System.Drawing.SystemColors.Window
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKode.Location = New System.Drawing.Point(297, 43)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.Size = New System.Drawing.Size(78, 23)
        Me.TxtKode.TabIndex = 67
        '
        'CmbSatuan
        '
        Me.CmbSatuan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSatuan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbSatuan.FormattingEnabled = True
        Me.CmbSatuan.Location = New System.Drawing.Point(114, 39)
        Me.CmbSatuan.Name = "CmbSatuan"
        Me.CmbSatuan.Size = New System.Drawing.Size(177, 25)
        Me.CmbSatuan.TabIndex = 66
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label19.ForeColor = System.Drawing.Color.Black
        Me.Label19.Location = New System.Drawing.Point(53, 43)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(53, 17)
        Me.Label19.TabIndex = 25
        Me.Label19.Text = "Satuan"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtNamaBarang
        '
        Me.TxtNamaBarang.BackColor = System.Drawing.SystemColors.Window
        Me.TxtNamaBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtNamaBarang.Location = New System.Drawing.Point(114, 10)
        Me.TxtNamaBarang.Name = "TxtNamaBarang"
        Me.TxtNamaBarang.Size = New System.Drawing.Size(440, 23)
        Me.TxtNamaBarang.TabIndex = 0
        '
        'TxtHargaBarang
        '
        Me.TxtHargaBarang.BackColor = System.Drawing.SystemColors.Window
        Me.TxtHargaBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtHargaBarang.Location = New System.Drawing.Point(114, 70)
        Me.TxtHargaBarang.Name = "TxtHargaBarang"
        Me.TxtHargaBarang.Size = New System.Drawing.Size(177, 23)
        Me.TxtHargaBarang.TabIndex = 1
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label20.ForeColor = System.Drawing.Color.Black
        Me.Label20.Location = New System.Drawing.Point(8, 73)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(98, 17)
        Me.Label20.TabIndex = 16
        Me.Label20.Text = "Harga Barang"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label21.ForeColor = System.Drawing.Color.Black
        Me.Label21.Location = New System.Drawing.Point(7, 13)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(99, 17)
        Me.Label21.TabIndex = 10
        Me.Label21.Text = "Nama Barang"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.Transparent
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.PicPreviewBarcode3)
        Me.Panel5.Controls.Add(Me.PicPreviewBarcode2)
        Me.Panel5.Controls.Add(Me.PicPreviewBarcode1)
        Me.Panel5.Controls.Add(Me.PicPreviewBarcode0)
        Me.Panel5.Location = New System.Drawing.Point(514, 255)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(563, 304)
        Me.Panel5.TabIndex = 93
        '
        'PicPreviewBarcode3
        '
        Me.PicPreviewBarcode3.Location = New System.Drawing.Point(3, 152)
        Me.PicPreviewBarcode3.Name = "PicPreviewBarcode3"
        Me.PicPreviewBarcode3.Size = New System.Drawing.Size(273, 143)
        Me.PicPreviewBarcode3.TabIndex = 3
        Me.PicPreviewBarcode3.TabStop = False
        '
        'PicPreviewBarcode2
        '
        Me.PicPreviewBarcode2.Location = New System.Drawing.Point(282, 152)
        Me.PicPreviewBarcode2.Name = "PicPreviewBarcode2"
        Me.PicPreviewBarcode2.Size = New System.Drawing.Size(272, 143)
        Me.PicPreviewBarcode2.TabIndex = 2
        Me.PicPreviewBarcode2.TabStop = False
        '
        'PicPreviewBarcode1
        '
        Me.PicPreviewBarcode1.Location = New System.Drawing.Point(282, 3)
        Me.PicPreviewBarcode1.Name = "PicPreviewBarcode1"
        Me.PicPreviewBarcode1.Size = New System.Drawing.Size(272, 143)
        Me.PicPreviewBarcode1.TabIndex = 1
        Me.PicPreviewBarcode1.TabStop = False
        '
        'PicPreviewBarcode0
        '
        Me.PicPreviewBarcode0.Location = New System.Drawing.Point(4, 3)
        Me.PicPreviewBarcode0.Name = "PicPreviewBarcode0"
        Me.PicPreviewBarcode0.Size = New System.Drawing.Size(272, 143)
        Me.PicPreviewBarcode0.TabIndex = 0
        Me.PicPreviewBarcode0.TabStop = False
        '
        'BtnRestore
        '
        Me.BtnRestore.AutoSize = True
        Me.BtnRestore.BackColor = System.Drawing.Color.Orange
        Me.BtnRestore.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnRestore.Image = CType(resources.GetObject("BtnRestore.Image"), System.Drawing.Image)
        Me.BtnRestore.Location = New System.Drawing.Point(83, 394)
        Me.BtnRestore.Name = "BtnRestore"
        Me.BtnRestore.Size = New System.Drawing.Size(130, 31)
        Me.BtnRestore.TabIndex = 97
        Me.BtnRestore.Text = "Restore Default"
        Me.BtnRestore.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnRestore.UseVisualStyleBackColor = False
        '
        'btnSimpanPerubahan
        '
        Me.btnSimpanPerubahan.AutoSize = True
        Me.btnSimpanPerubahan.BackColor = System.Drawing.Color.Orange
        Me.btnSimpanPerubahan.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnSimpanPerubahan.Image = CType(resources.GetObject("btnSimpanPerubahan.Image"), System.Drawing.Image)
        Me.btnSimpanPerubahan.Location = New System.Drawing.Point(349, 461)
        Me.btnSimpanPerubahan.Name = "btnSimpanPerubahan"
        Me.btnSimpanPerubahan.Size = New System.Drawing.Size(146, 31)
        Me.btnSimpanPerubahan.TabIndex = 96
        Me.btnSimpanPerubahan.Text = "Simpan perubahan"
        Me.btnSimpanPerubahan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnSimpanPerubahan.UseVisualStyleBackColor = False
        '
        'BtnCetakBarcode
        '
        Me.BtnCetakBarcode.AutoSize = True
        Me.BtnCetakBarcode.BackColor = System.Drawing.Color.Orange
        Me.BtnCetakBarcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnCetakBarcode.Image = CType(resources.GetObject("BtnCetakBarcode.Image"), System.Drawing.Image)
        Me.BtnCetakBarcode.Location = New System.Drawing.Point(653, 209)
        Me.BtnCetakBarcode.Name = "BtnCetakBarcode"
        Me.BtnCetakBarcode.Size = New System.Drawing.Size(85, 31)
        Me.BtnCetakBarcode.TabIndex = 95
        Me.BtnCetakBarcode.Text = "Cetak"
        Me.BtnCetakBarcode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCetakBarcode.UseVisualStyleBackColor = False
        '
        'BtnPreviewCetak
        '
        Me.BtnPreviewCetak.AutoSize = True
        Me.BtnPreviewCetak.BackColor = System.Drawing.Color.Orange
        Me.BtnPreviewCetak.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnPreviewCetak.Image = CType(resources.GetObject("BtnPreviewCetak.Image"), System.Drawing.Image)
        Me.BtnPreviewCetak.Location = New System.Drawing.Point(536, 209)
        Me.BtnPreviewCetak.Name = "BtnPreviewCetak"
        Me.BtnPreviewCetak.Size = New System.Drawing.Size(86, 31)
        Me.BtnPreviewCetak.TabIndex = 94
        Me.BtnPreviewCetak.Text = "Preview"
        Me.BtnPreviewCetak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPreviewCetak.UseVisualStyleBackColor = False
        '
        'BtnResetGap
        '
        Me.BtnResetGap.AutoSize = True
        Me.BtnResetGap.BackColor = System.Drawing.Color.Orange
        Me.BtnResetGap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnResetGap.Image = CType(resources.GetObject("BtnResetGap.Image"), System.Drawing.Image)
        Me.BtnResetGap.Location = New System.Drawing.Point(392, 394)
        Me.BtnResetGap.Name = "BtnResetGap"
        Me.BtnResetGap.Size = New System.Drawing.Size(103, 31)
        Me.BtnResetGap.TabIndex = 98
        Me.BtnResetGap.Text = "Reset Gab"
        Me.BtnResetGap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnResetGap.UseVisualStyleBackColor = False
        '
        'BtnResetPosisiXY
        '
        Me.BtnResetPosisiXY.AutoSize = True
        Me.BtnResetPosisiXY.BackColor = System.Drawing.Color.Orange
        Me.BtnResetPosisiXY.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnResetPosisiXY.Image = CType(resources.GetObject("BtnResetPosisiXY.Image"), System.Drawing.Image)
        Me.BtnResetPosisiXY.Location = New System.Drawing.Point(250, 394)
        Me.BtnResetPosisiXY.Name = "BtnResetPosisiXY"
        Me.BtnResetPosisiXY.Size = New System.Drawing.Size(102, 31)
        Me.BtnResetPosisiXY.TabIndex = 99
        Me.BtnResetPosisiXY.Text = "Reset X & Y"
        Me.BtnResetPosisiXY.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnResetPosisiXY.UseVisualStyleBackColor = False
        '
        'BtnExportBarcode
        '
        Me.BtnExportBarcode.AutoSize = True
        Me.BtnExportBarcode.BackColor = System.Drawing.Color.Orange
        Me.BtnExportBarcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.BtnExportBarcode.Image = CType(resources.GetObject("BtnExportBarcode.Image"), System.Drawing.Image)
        Me.BtnExportBarcode.Location = New System.Drawing.Point(768, 209)
        Me.BtnExportBarcode.Name = "BtnExportBarcode"
        Me.BtnExportBarcode.Size = New System.Drawing.Size(143, 31)
        Me.BtnExportBarcode.TabIndex = 101
        Me.BtnExportBarcode.Text = "Export Barcode"
        Me.BtnExportBarcode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnExportBarcode.UseVisualStyleBackColor = False
        '
        'FormCetakBarcode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1089, 573)
        Me.Controls.Add(Me.BtnExportBarcode)
        Me.Controls.Add(Me.BtnResetPosisiXY)
        Me.Controls.Add(Me.BtnResetGap)
        Me.Controls.Add(Me.BtnRestore)
        Me.Controls.Add(Me.btnSimpanPerubahan)
        Me.Controls.Add(Me.BtnCetakBarcode)
        Me.Controls.Add(Me.BtnPreviewCetak)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormCetakBarcode"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.PanelHeader.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        CType(Me.PicPreviewBarcode3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicPreviewBarcode2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicPreviewBarcode1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicPreviewBarcode0, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents BtnMinimize As Button
    Friend WithEvents BtnClose As Button
    Friend WithEvents LblUtama As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents CmbJenisPrinter As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtTinggiBarcode As TextBox
    Friend WithEvents TxtLebarBarcode As TextBox
    Friend WithEvents TxtLebarKertas As TextBox
    Friend WithEvents TxtTinggiKertas As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents CmbJenisKertas As ComboBox
    Friend WithEvents Label14 As Label
    Friend WithEvents CmbJumlahBarcodePerBaris As ComboBox
    Friend WithEvents Label13 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents CmbTipeBarcode As ComboBox
    Friend WithEvents TxtStartY As TextBox
    Friend WithEvents TxtStartX As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents TxtGabY As TextBox
    Friend WithEvents TxtGabX As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents ChkBoldSatuan As CheckBox
    Friend WithEvents CmbFontSatuan As ComboBox
    Friend WithEvents CmbUkuranSatuan As ComboBox
    Friend WithEvents ChkBoldToko As CheckBox
    Friend WithEvents ChkBoldHarga As CheckBox
    Friend WithEvents ChkBoldNama As CheckBox
    Friend WithEvents CmbFontToko As ComboBox
    Friend WithEvents CmbUkuranToko As ComboBox
    Friend WithEvents CmbFontHarga As ComboBox
    Friend WithEvents CmbUkuranHarga As ComboBox
    Friend WithEvents CmbFontNama As ComboBox
    Friend WithEvents CmbUkuranNama As ComboBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label17 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel4 As Panel
    Friend WithEvents CmbSatuan As ComboBox
    Friend WithEvents Label19 As Label
    Friend WithEvents TxtNamaBarang As TextBox
    Friend WithEvents TxtHargaBarang As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents TxtKode As TextBox
    Friend WithEvents TxtKodeBarcode As TextBox
    Friend WithEvents Panel5 As Panel
    Friend WithEvents PicPreviewBarcode3 As PictureBox
    Friend WithEvents PicPreviewBarcode2 As PictureBox
    Friend WithEvents PicPreviewBarcode1 As PictureBox
    Friend WithEvents PicPreviewBarcode0 As PictureBox
    Friend WithEvents TxtJumlahCetak As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents BtnRestore As Button
    Friend WithEvents btnSimpanPerubahan As Button
    Friend WithEvents BtnCetakBarcode As Button
    Friend WithEvents BtnPreviewCetak As Button
    Friend WithEvents BtnResetGap As Button
    Friend WithEvents BtnResetPosisiXY As Button
    Friend WithEvents BtnExportBarcode As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents LstBarang As ListBox
End Class
