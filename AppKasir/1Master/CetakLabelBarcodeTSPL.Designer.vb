<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CetakLabelBarcodeTSPL
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CetakLabelBarcodeTSPL))
        Me.cmbPrintColumns = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.TxtJumlahLabelDicetak = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.BtnPrint = New System.Windows.Forms.Button()
        Me.txtShopName = New System.Windows.Forms.TextBox()
        Me.cmbSelectPrinter = New System.Windows.Forms.ComboBox()
        Me.BtnCalibrateSensor = New System.Windows.Forms.Button()
        Me.trkVerticalOffset = New System.Windows.Forms.TrackBar()
        Me.nudLabelWidthMM = New System.Windows.Forms.NumericUpDown()
        Me.nudLabelHeightMM = New System.Windows.Forms.NumericUpDown()
        Me.nudGapHorizontalMM = New System.Windows.Forms.NumericUpDown()
        Me.lblVerticalOffsetValue = New System.Windows.Forms.Label()
        Me.BtnResetPosition = New System.Windows.Forms.Button()
        Me.nudGapVerticalMM = New System.Windows.Forms.NumericUpDown()
        Me.nudBarcodeHeightMM = New System.Windows.Forms.NumericUpDown()
        Me.chkUseCutter = New System.Windows.Forms.CheckBox()
        Me.chkUsePeel = New System.Windows.Forms.CheckBox()
        Me.nudBarcodeNarrowRatio = New System.Windows.Forms.NumericUpDown()
        Me.nudMarginTopMM = New System.Windows.Forms.NumericUpDown()
        Me.nudMarginLeftMM = New System.Windows.Forms.NumericUpDown()
        Me.cmbBarcodeFormat = New System.Windows.Forms.ComboBox()
        Me.BtnRestoreDefaults = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbBarcodeRotation = New System.Windows.Forms.ComboBox()
        Me.nudFontSizeName = New System.Windows.Forms.NumericUpDown()
        Me.nudBarcodeWideRatio = New System.Windows.Forms.NumericUpDown()
        Me.nudPrintDensity = New System.Windows.Forms.NumericUpDown()
        Me.nudPrintSpeed = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.nudFontSizePrice = New System.Windows.Forms.NumericUpDown()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.nudFontSizeUnit = New System.Windows.Forms.NumericUpDown()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.nudFontSizeShop = New System.Windows.Forms.NumericUpDown()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.nudPriceWidthMult = New System.Windows.Forms.NumericUpDown()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.nudMarginBottomMM = New System.Windows.Forms.NumericUpDown()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.nudGapNameBcMM = New System.Windows.Forms.NumericUpDown()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.nudBlockNameHeightMM = New System.Windows.Forms.NumericUpDown()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.nudGapPriceUnitMM = New System.Windows.Forms.NumericUpDown()
        Me.RTLog = New System.Windows.Forms.RichTextBox()
        Me.CmbPilihSatuanBarang = New System.Windows.Forms.ComboBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TxtKodeBarcodeInput = New System.Windows.Forms.TextBox()
        Me.TxtInputHargaBarang = New System.Windows.Forms.TextBox()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.TxtInputNamaBarang = New System.Windows.Forms.TextBox()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.LblHeader = New System.Windows.Forms.Label()
        CType(Me.trkVerticalOffset, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudLabelWidthMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudLabelHeightMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudGapHorizontalMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudGapVerticalMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudBarcodeHeightMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudBarcodeNarrowRatio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudMarginTopMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudMarginLeftMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFontSizeName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudBarcodeWideRatio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPrintDensity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPrintSpeed, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFontSizePrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFontSizeUnit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFontSizeShop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPriceWidthMult, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudMarginBottomMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudGapNameBcMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudBlockNameHeightMM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudGapPriceUnitMM, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmbPrintColumns
        '
        Me.cmbPrintColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrintColumns.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.cmbPrintColumns.FormattingEnabled = True
        Me.cmbPrintColumns.Items.AddRange(New Object() {"1", "2", "3", "4"})
        Me.cmbPrintColumns.Location = New System.Drawing.Point(729, 100)
        Me.cmbPrintColumns.Name = "cmbPrintColumns"
        Me.cmbPrintColumns.Size = New System.Drawing.Size(177, 25)
        Me.cmbPrintColumns.TabIndex = 85
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(562, 108)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(158, 17)
        Me.Label6.TabIndex = 86
        Me.Label6.Text = "Jumlah Kolom per Baris"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label21.ForeColor = System.Drawing.Color.Black
        Me.Label21.Location = New System.Drawing.Point(239, 64)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(49, 17)
        Me.Label21.TabIndex = 88
        Me.Label21.Text = "Nama"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtJumlahLabelDicetak
        '
        Me.TxtJumlahLabelDicetak.BackColor = System.Drawing.SystemColors.Window
        Me.TxtJumlahLabelDicetak.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtJumlahLabelDicetak.Location = New System.Drawing.Point(729, 129)
        Me.TxtJumlahLabelDicetak.Name = "TxtJumlahLabelDicetak"
        Me.TxtJumlahLabelDicetak.Size = New System.Drawing.Size(66, 23)
        Me.TxtJumlahLabelDicetak.TabIndex = 95
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(621, 132)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(99, 17)
        Me.Label18.TabIndex = 96
        Me.Label18.Text = "Jumlah Cetak"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BtnPrint
        '
        Me.BtnPrint.AutoSize = True
        Me.BtnPrint.BackColor = System.Drawing.Color.White
        Me.BtnPrint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPrint.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPrint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnPrint.Image = CType(resources.GetObject("BtnPrint.Image"), System.Drawing.Image)
        Me.BtnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPrint.Location = New System.Drawing.Point(358, 474)
        Me.BtnPrint.Name = "BtnPrint"
        Me.BtnPrint.Size = New System.Drawing.Size(74, 31)
        Me.BtnPrint.TabIndex = 97
        Me.BtnPrint.Text = "Cetak"
        Me.BtnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPrint.UseVisualStyleBackColor = False
        '
        'txtShopName
        '
        Me.txtShopName.BackColor = System.Drawing.SystemColors.Window
        Me.txtShopName.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.txtShopName.Location = New System.Drawing.Point(312, 192)
        Me.txtShopName.Name = "txtShopName"
        Me.txtShopName.Size = New System.Drawing.Size(155, 23)
        Me.txtShopName.TabIndex = 98
        '
        'cmbSelectPrinter
        '
        Me.cmbSelectPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSelectPrinter.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.cmbSelectPrinter.FormattingEnabled = True
        Me.cmbSelectPrinter.Location = New System.Drawing.Point(729, 66)
        Me.cmbSelectPrinter.Name = "cmbSelectPrinter"
        Me.cmbSelectPrinter.Size = New System.Drawing.Size(177, 25)
        Me.cmbSelectPrinter.TabIndex = 99
        '
        'BtnCalibrateSensor
        '
        Me.BtnCalibrateSensor.AutoSize = True
        Me.BtnCalibrateSensor.BackColor = System.Drawing.Color.White
        Me.BtnCalibrateSensor.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCalibrateSensor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCalibrateSensor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCalibrateSensor.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCalibrateSensor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCalibrateSensor.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCalibrateSensor.ForeColor = System.Drawing.Color.Black
        Me.BtnCalibrateSensor.Image = CType(resources.GetObject("BtnCalibrateSensor.Image"), System.Drawing.Image)
        Me.BtnCalibrateSensor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCalibrateSensor.Location = New System.Drawing.Point(311, 522)
        Me.BtnCalibrateSensor.Name = "BtnCalibrateSensor"
        Me.BtnCalibrateSensor.Size = New System.Drawing.Size(129, 31)
        Me.BtnCalibrateSensor.TabIndex = 101
        Me.BtnCalibrateSensor.Text = "Auto Calibrate"
        Me.BtnCalibrateSensor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCalibrateSensor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCalibrateSensor.UseVisualStyleBackColor = False
        '
        'trkVerticalOffset
        '
        Me.trkVerticalOffset.Location = New System.Drawing.Point(729, 330)
        Me.trkVerticalOffset.Name = "trkVerticalOffset"
        Me.trkVerticalOffset.Size = New System.Drawing.Size(104, 45)
        Me.trkVerticalOffset.TabIndex = 102
        '
        'nudLabelWidthMM
        '
        Me.nudLabelWidthMM.Location = New System.Drawing.Point(729, 162)
        Me.nudLabelWidthMM.Name = "nudLabelWidthMM"
        Me.nudLabelWidthMM.Size = New System.Drawing.Size(120, 20)
        Me.nudLabelWidthMM.TabIndex = 103
        '
        'nudLabelHeightMM
        '
        Me.nudLabelHeightMM.Location = New System.Drawing.Point(729, 188)
        Me.nudLabelHeightMM.Name = "nudLabelHeightMM"
        Me.nudLabelHeightMM.Size = New System.Drawing.Size(120, 20)
        Me.nudLabelHeightMM.TabIndex = 104
        '
        'nudGapHorizontalMM
        '
        Me.nudGapHorizontalMM.Location = New System.Drawing.Point(729, 214)
        Me.nudGapHorizontalMM.Name = "nudGapHorizontalMM"
        Me.nudGapHorizontalMM.Size = New System.Drawing.Size(120, 20)
        Me.nudGapHorizontalMM.TabIndex = 105
        '
        'lblVerticalOffsetValue
        '
        Me.lblVerticalOffsetValue.AutoSize = True
        Me.lblVerticalOffsetValue.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.lblVerticalOffsetValue.ForeColor = System.Drawing.Color.Black
        Me.lblVerticalOffsetValue.Location = New System.Drawing.Point(729, 368)
        Me.lblVerticalOffsetValue.Name = "lblVerticalOffsetValue"
        Me.lblVerticalOffsetValue.Size = New System.Drawing.Size(100, 17)
        Me.lblVerticalOffsetValue.TabIndex = 106
        Me.lblVerticalOffsetValue.Text = "LblOffsetValue"
        Me.lblVerticalOffsetValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BtnResetPosition
        '
        Me.BtnResetPosition.AutoSize = True
        Me.BtnResetPosition.BackColor = System.Drawing.Color.White
        Me.BtnResetPosition.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnResetPosition.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnResetPosition.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnResetPosition.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnResetPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnResetPosition.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnResetPosition.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnResetPosition.Image = CType(resources.GetObject("BtnResetPosition.Image"), System.Drawing.Image)
        Me.BtnResetPosition.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnResetPosition.Location = New System.Drawing.Point(126, 522)
        Me.BtnResetPosition.Name = "BtnResetPosition"
        Me.BtnResetPosition.Size = New System.Drawing.Size(128, 31)
        Me.BtnResetPosition.TabIndex = 107
        Me.BtnResetPosition.Text = "Reset Position"
        Me.BtnResetPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnResetPosition.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnResetPosition.UseVisualStyleBackColor = False
        '
        'nudGapVerticalMM
        '
        Me.nudGapVerticalMM.Location = New System.Drawing.Point(729, 240)
        Me.nudGapVerticalMM.Name = "nudGapVerticalMM"
        Me.nudGapVerticalMM.Size = New System.Drawing.Size(120, 20)
        Me.nudGapVerticalMM.TabIndex = 108
        '
        'nudBarcodeHeightMM
        '
        Me.nudBarcodeHeightMM.Location = New System.Drawing.Point(729, 459)
        Me.nudBarcodeHeightMM.Name = "nudBarcodeHeightMM"
        Me.nudBarcodeHeightMM.Size = New System.Drawing.Size(120, 20)
        Me.nudBarcodeHeightMM.TabIndex = 109
        '
        'chkUseCutter
        '
        Me.chkUseCutter.AutoSize = True
        Me.chkUseCutter.Location = New System.Drawing.Point(1070, 111)
        Me.chkUseCutter.Name = "chkUseCutter"
        Me.chkUseCutter.Size = New System.Drawing.Size(81, 17)
        Me.chkUseCutter.TabIndex = 110
        Me.chkUseCutter.Text = "CheckBox1"
        Me.chkUseCutter.UseVisualStyleBackColor = True
        '
        'chkUsePeel
        '
        Me.chkUsePeel.AutoSize = True
        Me.chkUsePeel.Location = New System.Drawing.Point(1070, 134)
        Me.chkUsePeel.Name = "chkUsePeel"
        Me.chkUsePeel.Size = New System.Drawing.Size(81, 17)
        Me.chkUsePeel.TabIndex = 111
        Me.chkUsePeel.Text = "CheckBox1"
        Me.chkUsePeel.UseVisualStyleBackColor = True
        '
        'nudBarcodeNarrowRatio
        '
        Me.nudBarcodeNarrowRatio.Location = New System.Drawing.Point(729, 485)
        Me.nudBarcodeNarrowRatio.Name = "nudBarcodeNarrowRatio"
        Me.nudBarcodeNarrowRatio.Size = New System.Drawing.Size(120, 20)
        Me.nudBarcodeNarrowRatio.TabIndex = 117
        '
        'nudMarginTopMM
        '
        Me.nudMarginTopMM.Location = New System.Drawing.Point(729, 292)
        Me.nudMarginTopMM.Name = "nudMarginTopMM"
        Me.nudMarginTopMM.Size = New System.Drawing.Size(120, 20)
        Me.nudMarginTopMM.TabIndex = 114
        '
        'nudMarginLeftMM
        '
        Me.nudMarginLeftMM.Location = New System.Drawing.Point(729, 266)
        Me.nudMarginLeftMM.Name = "nudMarginLeftMM"
        Me.nudMarginLeftMM.Size = New System.Drawing.Size(120, 20)
        Me.nudMarginLeftMM.TabIndex = 113
        '
        'cmbBarcodeFormat
        '
        Me.cmbBarcodeFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBarcodeFormat.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.cmbBarcodeFormat.FormattingEnabled = True
        Me.cmbBarcodeFormat.Location = New System.Drawing.Point(729, 397)
        Me.cmbBarcodeFormat.Name = "cmbBarcodeFormat"
        Me.cmbBarcodeFormat.Size = New System.Drawing.Size(177, 25)
        Me.cmbBarcodeFormat.TabIndex = 118
        '
        'BtnRestoreDefaults
        '
        Me.BtnRestoreDefaults.AutoSize = True
        Me.BtnRestoreDefaults.BackColor = System.Drawing.Color.White
        Me.BtnRestoreDefaults.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnRestoreDefaults.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnRestoreDefaults.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnRestoreDefaults.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnRestoreDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnRestoreDefaults.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnRestoreDefaults.ForeColor = System.Drawing.Color.Black
        Me.BtnRestoreDefaults.Image = CType(resources.GetObject("BtnRestoreDefaults.Image"), System.Drawing.Image)
        Me.BtnRestoreDefaults.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRestoreDefaults.Location = New System.Drawing.Point(82, 474)
        Me.BtnRestoreDefaults.Name = "BtnRestoreDefaults"
        Me.BtnRestoreDefaults.Size = New System.Drawing.Size(123, 31)
        Me.BtnRestoreDefaults.TabIndex = 119
        Me.BtnRestoreDefaults.Text = "Reset Default"
        Me.BtnRestoreDefaults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRestoreDefaults.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnRestoreDefaults.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(672, 69)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 17)
        Me.Label1.TabIndex = 120
        Me.Label1.Text = "Printer"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(205, 195)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 17)
        Me.Label2.TabIndex = 121
        Me.Label2.Text = "Nama Toko"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmbBarcodeRotation
        '
        Me.cmbBarcodeRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBarcodeRotation.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.cmbBarcodeRotation.FormattingEnabled = True
        Me.cmbBarcodeRotation.Location = New System.Drawing.Point(729, 428)
        Me.cmbBarcodeRotation.Name = "cmbBarcodeRotation"
        Me.cmbBarcodeRotation.Size = New System.Drawing.Size(177, 25)
        Me.cmbBarcodeRotation.TabIndex = 122
        '
        'nudFontSizeName
        '
        Me.nudFontSizeName.Location = New System.Drawing.Point(312, 221)
        Me.nudFontSizeName.Name = "nudFontSizeName"
        Me.nudFontSizeName.Size = New System.Drawing.Size(120, 20)
        Me.nudFontSizeName.TabIndex = 124
        '
        'nudBarcodeWideRatio
        '
        Me.nudBarcodeWideRatio.Location = New System.Drawing.Point(729, 510)
        Me.nudBarcodeWideRatio.Name = "nudBarcodeWideRatio"
        Me.nudBarcodeWideRatio.Size = New System.Drawing.Size(120, 20)
        Me.nudBarcodeWideRatio.TabIndex = 123
        '
        'nudPrintDensity
        '
        Me.nudPrintDensity.Location = New System.Drawing.Point(1070, 80)
        Me.nudPrintDensity.Name = "nudPrintDensity"
        Me.nudPrintDensity.Size = New System.Drawing.Size(120, 20)
        Me.nudPrintDensity.TabIndex = 126
        '
        'nudPrintSpeed
        '
        Me.nudPrintSpeed.Location = New System.Drawing.Point(1070, 54)
        Me.nudPrintSpeed.Name = "nudPrintSpeed"
        Me.nudPrintSpeed.Size = New System.Drawing.Size(120, 20)
        Me.nudPrintSpeed.TabIndex = 125
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(602, 165)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(118, 17)
        Me.Label4.TabIndex = 128
        Me.Label4.Text = "(Lebar Label mm"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(606, 191)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(114, 17)
        Me.Label5.TabIndex = 129
        Me.Label5.Text = "Tinggi Label mm"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(553, 217)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(167, 17)
        Me.Label7.TabIndex = 130
        Me.Label7.Text = "Jarak Antara Kolom mm"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(532, 243)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(188, 17)
        Me.Label8.TabIndex = 131
        Me.Label8.Text = "Jarak Antara Baris/Gap mm"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(616, 266)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(104, 17)
        Me.Label9.TabIndex = 132
        Me.Label9.Text = "Margin Kiri mm"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(606, 292)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(114, 17)
        Me.Label10.TabIndex = 133
        Me.Label10.Text = "Margin Atas mm"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(607, 330)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(113, 17)
        Me.Label11.TabIndex = 134
        Me.Label11.Text = "TrackBar Offset Y"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(608, 400)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(112, 17)
        Me.Label12.TabIndex = 135
        Me.Label12.Text = "Barcode Format"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(598, 433)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(122, 17)
        Me.Label13.TabIndex = 136
        Me.Label13.Text = "Barcode Rotation"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(612, 462)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(108, 17)
        Me.Label14.TabIndex = 137
        Me.Label14.Text = "Barcode Height"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(607, 485)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(113, 17)
        Me.Label15.TabIndex = 138
        Me.Label15.Text = "Bc Narrow Ratio"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label16.ForeColor = System.Drawing.Color.Black
        Me.Label16.Location = New System.Drawing.Point(622, 513)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(98, 17)
        Me.Label16.TabIndex = 139
        Me.Label16.Text = "Bc Wide Ratio"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label17.ForeColor = System.Drawing.Color.Black
        Me.Label17.Location = New System.Drawing.Point(157, 218)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(131, 17)
        Me.Label17.TabIndex = 140
        Me.Label17.Text = "Font Nama Barang"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label22.ForeColor = System.Drawing.Color.Black
        Me.Label22.Location = New System.Drawing.Point(608, 368)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(112, 17)
        Me.Label22.TabIndex = 141
        Me.Label22.Text = "Label nilai offset"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label23.ForeColor = System.Drawing.Color.Black
        Me.Label23.Location = New System.Drawing.Point(949, 128)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(81, 17)
        Me.Label23.TabIndex = 145
        Me.Label23.Text = "Peel Check"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label24.ForeColor = System.Drawing.Color.Black
        Me.Label24.Location = New System.Drawing.Point(951, 105)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(95, 17)
        Me.Label24.TabIndex = 144
        Me.Label24.Text = "Cutter Check"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label25.ForeColor = System.Drawing.Color.Black
        Me.Label25.Location = New System.Drawing.Point(951, 77)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(111, 17)
        Me.Label25.TabIndex = 143
        Me.Label25.Text = "Kepadatan 1-15"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label26.ForeColor = System.Drawing.Color.Black
        Me.Label26.Location = New System.Drawing.Point(951, 54)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(102, 17)
        Me.Label26.TabIndex = 142
        Me.Label26.Text = "Kecepatan 1-4"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label27.ForeColor = System.Drawing.Color.Black
        Me.Label27.Location = New System.Drawing.Point(208, 241)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(80, 17)
        Me.Label27.TabIndex = 147
        Me.Label27.Text = "Font Harga"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudFontSizePrice
        '
        Me.nudFontSizePrice.Location = New System.Drawing.Point(312, 244)
        Me.nudFontSizePrice.Name = "nudFontSizePrice"
        Me.nudFontSizePrice.Size = New System.Drawing.Size(120, 20)
        Me.nudFontSizePrice.TabIndex = 146
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label28.ForeColor = System.Drawing.Color.Black
        Me.Label28.Location = New System.Drawing.Point(203, 267)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(85, 17)
        Me.Label28.TabIndex = 149
        Me.Label28.Text = "Font Satuan"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudFontSizeUnit
        '
        Me.nudFontSizeUnit.Location = New System.Drawing.Point(312, 270)
        Me.nudFontSizeUnit.Name = "nudFontSizeUnit"
        Me.nudFontSizeUnit.Size = New System.Drawing.Size(120, 20)
        Me.nudFontSizeUnit.TabIndex = 148
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label29.ForeColor = System.Drawing.Color.Black
        Me.Label29.Location = New System.Drawing.Point(218, 293)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(70, 17)
        Me.Label29.TabIndex = 151
        Me.Label29.Text = "Font Toko"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudFontSizeShop
        '
        Me.nudFontSizeShop.Location = New System.Drawing.Point(312, 296)
        Me.nudFontSizeShop.Name = "nudFontSizeShop"
        Me.nudFontSizeShop.Size = New System.Drawing.Size(120, 20)
        Me.nudFontSizeShop.TabIndex = 150
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label30.ForeColor = System.Drawing.Color.Black
        Me.Label30.Location = New System.Drawing.Point(148, 319)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(140, 17)
        Me.Label30.TabIndex = 153
        Me.Label30.Text = "Pengali Lebar Harga"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudPriceWidthMult
        '
        Me.nudPriceWidthMult.Location = New System.Drawing.Point(312, 322)
        Me.nudPriceWidthMult.Name = "nudPriceWidthMult"
        Me.nudPriceWidthMult.Size = New System.Drawing.Size(120, 20)
        Me.nudPriceWidthMult.TabIndex = 152
        '
        'BtnSave
        '
        Me.BtnSave.AutoSize = True
        Me.BtnSave.BackColor = System.Drawing.Color.White
        Me.BtnSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSave.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSave.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSave.Image = CType(resources.GetObject("BtnSave.Image"), System.Drawing.Image)
        Me.BtnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSave.Location = New System.Drawing.Point(221, 474)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(121, 31)
        Me.BtnSave.TabIndex = 154
        Me.BtnSave.Text = "Simpan"
        Me.BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSave.UseVisualStyleBackColor = False
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label31.ForeColor = System.Drawing.Color.Black
        Me.Label31.Location = New System.Drawing.Point(187, 430)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(101, 17)
        Me.Label31.TabIndex = 160
        Me.Label31.Text = "Margin Bawah"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudMarginBottomMM
        '
        Me.nudMarginBottomMM.Location = New System.Drawing.Point(312, 433)
        Me.nudMarginBottomMM.Name = "nudMarginBottomMM"
        Me.nudMarginBottomMM.Size = New System.Drawing.Size(120, 20)
        Me.nudMarginBottomMM.TabIndex = 159
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label32.ForeColor = System.Drawing.Color.Black
        Me.Label32.Location = New System.Drawing.Point(138, 371)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(150, 17)
        Me.Label32.TabIndex = 158
        Me.Label32.Text = "Jarak Nama & Barcode"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudGapNameBcMM
        '
        Me.nudGapNameBcMM.Location = New System.Drawing.Point(312, 374)
        Me.nudGapNameBcMM.Name = "nudGapNameBcMM"
        Me.nudGapNameBcMM.Size = New System.Drawing.Size(120, 20)
        Me.nudGapNameBcMM.TabIndex = 157
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label33.ForeColor = System.Drawing.Color.Black
        Me.Label33.Location = New System.Drawing.Point(106, 345)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(182, 17)
        Me.Label33.TabIndex = 156
        Me.Label33.Text = "Tinggi Kotak Nama Barang"
        Me.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudBlockNameHeightMM
        '
        Me.nudBlockNameHeightMM.Location = New System.Drawing.Point(312, 348)
        Me.nudBlockNameHeightMM.Name = "nudBlockNameHeightMM"
        Me.nudBlockNameHeightMM.Size = New System.Drawing.Size(120, 20)
        Me.nudBlockNameHeightMM.TabIndex = 155
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label34.ForeColor = System.Drawing.Color.Black
        Me.Label34.Location = New System.Drawing.Point(148, 397)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(140, 17)
        Me.Label34.TabIndex = 162
        Me.Label34.Text = "Jarak Harga & Satuan"
        Me.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'nudGapPriceUnitMM
        '
        Me.nudGapPriceUnitMM.Location = New System.Drawing.Point(312, 400)
        Me.nudGapPriceUnitMM.Name = "nudGapPriceUnitMM"
        Me.nudGapPriceUnitMM.Size = New System.Drawing.Size(120, 20)
        Me.nudGapPriceUnitMM.TabIndex = 161
        '
        'RTLog
        '
        Me.RTLog.Location = New System.Drawing.Point(952, 166)
        Me.RTLog.Name = "RTLog"
        Me.RTLog.Size = New System.Drawing.Size(570, 450)
        Me.RTLog.TabIndex = 163
        Me.RTLog.Text = ""
        '
        'CmbPilihSatuanBarang
        '
        Me.CmbPilihSatuanBarang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPilihSatuanBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbPilihSatuanBarang.FormattingEnabled = True
        Me.CmbPilihSatuanBarang.Location = New System.Drawing.Point(312, 95)
        Me.CmbPilihSatuanBarang.Name = "CmbPilihSatuanBarang"
        Me.CmbPilihSatuanBarang.Size = New System.Drawing.Size(196, 25)
        Me.CmbPilihSatuanBarang.TabIndex = 167
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label35.ForeColor = System.Drawing.Color.Black
        Me.Label35.Location = New System.Drawing.Point(251, 99)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(53, 17)
        Me.Label35.TabIndex = 166
        Me.Label35.Text = "Satuan"
        Me.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label19.ForeColor = System.Drawing.Color.Black
        Me.Label19.Location = New System.Drawing.Point(243, 166)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(62, 17)
        Me.Label19.TabIndex = 171
        Me.Label19.Text = "Barcode"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtKodeBarcodeInput
        '
        Me.TxtKodeBarcodeInput.BackColor = System.Drawing.SystemColors.Window
        Me.TxtKodeBarcodeInput.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKodeBarcodeInput.Location = New System.Drawing.Point(311, 162)
        Me.TxtKodeBarcodeInput.Name = "TxtKodeBarcodeInput"
        Me.TxtKodeBarcodeInput.Size = New System.Drawing.Size(155, 23)
        Me.TxtKodeBarcodeInput.TabIndex = 170
        '
        'TxtInputHargaBarang
        '
        Me.TxtInputHargaBarang.BackColor = System.Drawing.SystemColors.Window
        Me.TxtInputHargaBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtInputHargaBarang.Location = New System.Drawing.Point(311, 129)
        Me.TxtInputHargaBarang.Name = "TxtInputHargaBarang"
        Me.TxtInputHargaBarang.Size = New System.Drawing.Size(155, 23)
        Me.TxtInputHargaBarang.TabIndex = 168
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label36.ForeColor = System.Drawing.Color.Black
        Me.Label36.Location = New System.Drawing.Point(257, 131)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(48, 17)
        Me.Label36.TabIndex = 169
        Me.Label36.Text = "Harga"
        Me.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtInputNamaBarang
        '
        Me.TxtInputNamaBarang.BackColor = System.Drawing.SystemColors.Window
        Me.TxtInputNamaBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtInputNamaBarang.Location = New System.Drawing.Point(311, 63)
        Me.TxtInputNamaBarang.Name = "TxtInputNamaBarang"
        Me.TxtInputNamaBarang.Size = New System.Drawing.Size(155, 23)
        Me.TxtInputNamaBarang.TabIndex = 172
        '
        'BtnKeluar
        '
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluar.AutoSize = True
        Me.BtnKeluar.BackColor = System.Drawing.Color.White
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.Location = New System.Drawing.Point(737, 559)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(112, 35)
        Me.BtnKeluar.TabIndex = 173
        Me.BtnKeluar.Text = "Keluar (Esc)"
        Me.BtnKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.Gold
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 24.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle))
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(1547, 43)
        Me.LblHeader.TabIndex = 174
        Me.LblHeader.Text = "PROSES PENGEMBANGAN JANGAN DI GUNAKAN"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CetakLabelBarcodeTSPL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1547, 650)
        Me.Controls.Add(Me.LblHeader)
        Me.Controls.Add(Me.BtnKeluar)
        Me.Controls.Add(Me.TxtInputNamaBarang)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.TxtKodeBarcodeInput)
        Me.Controls.Add(Me.TxtInputHargaBarang)
        Me.Controls.Add(Me.Label36)
        Me.Controls.Add(Me.CmbPilihSatuanBarang)
        Me.Controls.Add(Me.Label35)
        Me.Controls.Add(Me.RTLog)
        Me.Controls.Add(Me.Label34)
        Me.Controls.Add(Me.nudGapPriceUnitMM)
        Me.Controls.Add(Me.Label31)
        Me.Controls.Add(Me.nudMarginBottomMM)
        Me.Controls.Add(Me.Label32)
        Me.Controls.Add(Me.nudGapNameBcMM)
        Me.Controls.Add(Me.Label33)
        Me.Controls.Add(Me.nudBlockNameHeightMM)
        Me.Controls.Add(Me.BtnSave)
        Me.Controls.Add(Me.Label30)
        Me.Controls.Add(Me.nudPriceWidthMult)
        Me.Controls.Add(Me.Label29)
        Me.Controls.Add(Me.nudFontSizeShop)
        Me.Controls.Add(Me.Label28)
        Me.Controls.Add(Me.nudFontSizeUnit)
        Me.Controls.Add(Me.Label27)
        Me.Controls.Add(Me.nudFontSizePrice)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.Label26)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.nudPrintDensity)
        Me.Controls.Add(Me.nudPrintSpeed)
        Me.Controls.Add(Me.nudFontSizeName)
        Me.Controls.Add(Me.nudBarcodeWideRatio)
        Me.Controls.Add(Me.cmbBarcodeRotation)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnRestoreDefaults)
        Me.Controls.Add(Me.cmbBarcodeFormat)
        Me.Controls.Add(Me.nudBarcodeNarrowRatio)
        Me.Controls.Add(Me.nudMarginTopMM)
        Me.Controls.Add(Me.nudMarginLeftMM)
        Me.Controls.Add(Me.chkUsePeel)
        Me.Controls.Add(Me.chkUseCutter)
        Me.Controls.Add(Me.nudBarcodeHeightMM)
        Me.Controls.Add(Me.nudGapVerticalMM)
        Me.Controls.Add(Me.BtnResetPosition)
        Me.Controls.Add(Me.lblVerticalOffsetValue)
        Me.Controls.Add(Me.nudGapHorizontalMM)
        Me.Controls.Add(Me.nudLabelHeightMM)
        Me.Controls.Add(Me.nudLabelWidthMM)
        Me.Controls.Add(Me.trkVerticalOffset)
        Me.Controls.Add(Me.BtnCalibrateSensor)
        Me.Controls.Add(Me.cmbSelectPrinter)
        Me.Controls.Add(Me.txtShopName)
        Me.Controls.Add(Me.BtnPrint)
        Me.Controls.Add(Me.TxtJumlahLabelDicetak)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.cmbPrintColumns)
        Me.Controls.Add(Me.Label6)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "CetakLabelBarcodeTSPL"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormBarcode"
        CType(Me.trkVerticalOffset, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudLabelWidthMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudLabelHeightMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudGapHorizontalMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudGapVerticalMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudBarcodeHeightMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudBarcodeNarrowRatio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudMarginTopMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudMarginLeftMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFontSizeName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudBarcodeWideRatio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPrintDensity, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPrintSpeed, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFontSizePrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFontSizeUnit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFontSizeShop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPriceWidthMult, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudMarginBottomMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudGapNameBcMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudBlockNameHeightMM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudGapPriceUnitMM, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmbPrintColumns As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents TxtJumlahLabelDicetak As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents BtnPrint As Button
    Friend WithEvents txtShopName As TextBox
    Friend WithEvents cmbSelectPrinter As ComboBox
    Friend WithEvents BtnCalibrateSensor As Button
    Friend WithEvents trkVerticalOffset As TrackBar
    Friend WithEvents nudLabelWidthMM As NumericUpDown
    Friend WithEvents nudLabelHeightMM As NumericUpDown
    Friend WithEvents nudGapHorizontalMM As NumericUpDown
    Friend WithEvents lblVerticalOffsetValue As Label
    Friend WithEvents BtnResetPosition As Button
    Friend WithEvents nudGapVerticalMM As NumericUpDown
    Friend WithEvents nudBarcodeHeightMM As NumericUpDown
    Friend WithEvents chkUseCutter As CheckBox
    Friend WithEvents chkUsePeel As CheckBox
    Friend WithEvents nudBarcodeNarrowRatio As NumericUpDown
    Friend WithEvents nudMarginTopMM As NumericUpDown
    Friend WithEvents nudMarginLeftMM As NumericUpDown
    Friend WithEvents cmbBarcodeFormat As ComboBox
    Friend WithEvents BtnRestoreDefaults As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents cmbBarcodeRotation As ComboBox
    Friend WithEvents nudFontSizeName As NumericUpDown
    Friend WithEvents nudBarcodeWideRatio As NumericUpDown
    Friend WithEvents nudPrintDensity As NumericUpDown
    Friend WithEvents nudPrintSpeed As NumericUpDown
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents Label23 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents Label25 As Label
    Friend WithEvents Label26 As Label
    Friend WithEvents Label27 As Label
    Friend WithEvents nudFontSizePrice As NumericUpDown
    Friend WithEvents Label28 As Label
    Friend WithEvents nudFontSizeUnit As NumericUpDown
    Friend WithEvents Label29 As Label
    Friend WithEvents nudFontSizeShop As NumericUpDown
    Friend WithEvents Label30 As Label
    Friend WithEvents nudPriceWidthMult As NumericUpDown
    Friend WithEvents BtnSave As Button
    Friend WithEvents Label31 As Label
    Friend WithEvents nudMarginBottomMM As NumericUpDown
    Friend WithEvents Label32 As Label
    Friend WithEvents nudGapNameBcMM As NumericUpDown
    Friend WithEvents Label33 As Label
    Friend WithEvents nudBlockNameHeightMM As NumericUpDown
    Friend WithEvents Label34 As Label
    Friend WithEvents nudGapPriceUnitMM As NumericUpDown
    Friend WithEvents RTLog As RichTextBox
    Friend WithEvents CmbPilihSatuanBarang As ComboBox
    Friend WithEvents Label35 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents TxtKodeBarcodeInput As TextBox
    Friend WithEvents TxtInputHargaBarang As TextBox
    Friend WithEvents Label36 As Label
    Friend WithEvents TxtInputNamaBarang As TextBox
    Friend WithEvents BtnKeluar As Button
    Friend WithEvents LblHeader As Label
End Class
