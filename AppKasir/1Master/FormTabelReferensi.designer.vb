<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormTabelReferensi
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTabelReferensi))
        Me.CmbType = New System.Windows.Forms.ComboBox()
        Me.CmbNRLR = New System.Windows.Forms.ComboBox()
        Me.CmbDK = New System.Windows.Forms.ComboBox()
        Me.CmbSubAkun = New System.Windows.Forms.ComboBox()
        Me.TxtSaldoAwal = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Dgvdata = New System.Windows.Forms.DataGridView()
        Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CmbJenisAkun = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.CmbJenisAkunCari = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.BtnHapus = New System.Windows.Forms.Button()
        Me.BtnKOsong = New System.Windows.Forms.Button()
        Me.TxtStatus = New System.Windows.Forms.TextBox()
        Me.LblSAwal = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        CType(Me.Dgvdata, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CmbType
        '
        Me.CmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbType.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbType.FormattingEnabled = True
        Me.CmbType.Location = New System.Drawing.Point(109, 96)
        Me.CmbType.Name = "CmbType"
        Me.CmbType.Size = New System.Drawing.Size(143, 25)
        Me.CmbType.TabIndex = 45
        '
        'CmbNRLR
        '
        Me.CmbNRLR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbNRLR.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbNRLR.FormattingEnabled = True
        Me.CmbNRLR.Location = New System.Drawing.Point(109, 289)
        Me.CmbNRLR.Name = "CmbNRLR"
        Me.CmbNRLR.Size = New System.Drawing.Size(143, 25)
        Me.CmbNRLR.TabIndex = 38
        '
        'CmbDK
        '
        Me.CmbDK.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbDK.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbDK.FormattingEnabled = True
        Me.CmbDK.Location = New System.Drawing.Point(109, 260)
        Me.CmbDK.Name = "CmbDK"
        Me.CmbDK.Size = New System.Drawing.Size(143, 25)
        Me.CmbDK.TabIndex = 37
        '
        'CmbSubAkun
        '
        Me.CmbSubAkun.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSubAkun.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbSubAkun.FormattingEnabled = True
        Me.CmbSubAkun.Location = New System.Drawing.Point(109, 231)
        Me.CmbSubAkun.Name = "CmbSubAkun"
        Me.CmbSubAkun.Size = New System.Drawing.Size(143, 25)
        Me.CmbSubAkun.TabIndex = 36
        '
        'TxtSaldoAwal
        '
        Me.TxtSaldoAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtSaldoAwal.Location = New System.Drawing.Point(109, 318)
        Me.TxtSaldoAwal.Name = "TxtSaldoAwal"
        Me.TxtSaldoAwal.Size = New System.Drawing.Size(143, 23)
        Me.TxtSaldoAwal.TabIndex = 35
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label7.Location = New System.Drawing.Point(10, 321)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(89, 17)
        Me.Label7.TabIndex = 34
        Me.Label7.Text = "Saldo Awal :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.Location = New System.Drawing.Point(11, 293)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 17)
        Me.Label6.TabIndex = 33
        Me.Label6.Text = "Akun NL/LR :"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label5.Location = New System.Drawing.Point(29, 264)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(70, 17)
        Me.Label5.TabIndex = 32
        Me.Label5.Text = "Akun DK :"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label4.Location = New System.Drawing.Point(24, 235)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 17)
        Me.Label4.TabIndex = 31
        Me.Label4.Text = "Sub Akun :"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtNama
        '
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtNama.Location = New System.Drawing.Point(109, 152)
        Me.TxtNama.Multiline = True
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(143, 73)
        Me.TxtNama.TabIndex = 30
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label3.Location = New System.Drawing.Point(6, 152)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 17)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "Nama Akun :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtKode
        '
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKode.Location = New System.Drawing.Point(109, 125)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.Size = New System.Drawing.Size(143, 23)
        Me.TxtKode.TabIndex = 28
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label2.Location = New System.Drawing.Point(13, 128)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 17)
        Me.Label2.TabIndex = 27
        Me.Label2.Text = "Kode Akun :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label1.Location = New System.Drawing.Point(19, 100)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 17)
        Me.Label1.TabIndex = 26
        Me.Label1.Text = "Type Akun :"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Dgvdata
        '
        Me.Dgvdata.AllowUserToAddRows = False
        Me.Dgvdata.AllowUserToDeleteRows = False
        Me.Dgvdata.AllowUserToResizeRows = False
        Me.Dgvdata.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Dgvdata.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.Dgvdata.BackgroundColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Dgvdata.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.Dgvdata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Dgvdata.DefaultCellStyle = DataGridViewCellStyle2
        Me.Dgvdata.Location = New System.Drawing.Point(259, 67)
        Me.Dgvdata.Name = "Dgvdata"
        Me.Dgvdata.ReadOnly = True
        Me.Dgvdata.RowHeadersVisible = False
        Me.Dgvdata.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.Dgvdata.Size = New System.Drawing.Size(1006, 562)
        Me.Dgvdata.TabIndex = 25
        '
        'ErrorProvider1
        '
        Me.ErrorProvider1.ContainerControl = Me
        '
        'Label8
        '
        Me.Label8.BackColor = System.Drawing.Color.Orange
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(0, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(1271, 28)
        Me.Label8.TabIndex = 57
        Me.Label8.Text = "T A B E L  A K U N   R E F E R E N S I"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CmbJenisAkun
        '
        Me.CmbJenisAkun.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisAkun.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbJenisAkun.FormattingEnabled = True
        Me.CmbJenisAkun.Location = New System.Drawing.Point(109, 67)
        Me.CmbJenisAkun.Name = "CmbJenisAkun"
        Me.CmbJenisAkun.Size = New System.Drawing.Size(143, 25)
        Me.CmbJenisAkun.TabIndex = 59
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label13.Location = New System.Drawing.Point(17, 71)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(82, 17)
        Me.Label13.TabIndex = 58
        Me.Label13.Text = "Jenis Akun :"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbJenisAkunCari
        '
        Me.CmbJenisAkunCari.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisAkunCari.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbJenisAkunCari.FormattingEnabled = True
        Me.CmbJenisAkunCari.Location = New System.Drawing.Point(412, 37)
        Me.CmbJenisAkunCari.Name = "CmbJenisAkunCari"
        Me.CmbJenisAkunCari.Size = New System.Drawing.Size(214, 25)
        Me.CmbJenisAkunCari.TabIndex = 60
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label14.Location = New System.Drawing.Point(278, 41)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(129, 17)
        Me.Label14.TabIndex = 61
        Me.Label14.Text = "Sort by Jenis akun :"
        '
        'BtnSimpan
        '
        Me.BtnSimpan.BackColor = System.Drawing.Color.Goldenrod
        Me.BtnSimpan.FlatAppearance.BorderSize = 0
        Me.BtnSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnSimpan.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpan.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.Location = New System.Drawing.Point(97, 379)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(155, 37)
        Me.BtnSimpan.TabIndex = 62
        Me.BtnSimpan.Text = "SIMPAN (F2)"
        Me.BtnSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'BtnHapus
        '
        Me.BtnHapus.BackColor = System.Drawing.Color.Goldenrod
        Me.BtnHapus.FlatAppearance.BorderSize = 0
        Me.BtnHapus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BtnHapus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.BtnHapus.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHapus.ForeColor = System.Drawing.Color.Black
        Me.BtnHapus.Image = CType(resources.GetObject("BtnHapus.Image"), System.Drawing.Image)
        Me.BtnHapus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHapus.Location = New System.Drawing.Point(98, 422)
        Me.BtnHapus.Name = "BtnHapus"
        Me.BtnHapus.Size = New System.Drawing.Size(155, 37)
        Me.BtnHapus.TabIndex = 63
        Me.BtnHapus.Text = "HAPUS (F3)"
        Me.BtnHapus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHapus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnHapus.UseVisualStyleBackColor = False
        '
        'BtnKOsong
        '
        Me.BtnKOsong.BackColor = System.Drawing.Color.Goldenrod
        Me.BtnKOsong.FlatAppearance.BorderSize = 0
        Me.BtnKOsong.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnKOsong.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnKOsong.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKOsong.ForeColor = System.Drawing.Color.Black
        Me.BtnKOsong.Image = CType(resources.GetObject("BtnKOsong.Image"), System.Drawing.Image)
        Me.BtnKOsong.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKOsong.Location = New System.Drawing.Point(98, 465)
        Me.BtnKOsong.Name = "BtnKOsong"
        Me.BtnKOsong.Size = New System.Drawing.Size(155, 37)
        Me.BtnKOsong.TabIndex = 64
        Me.BtnKOsong.Text = "RESET (F4)"
        Me.BtnKOsong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKOsong.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKOsong.UseVisualStyleBackColor = False
        '
        'TxtStatus
        '
        Me.TxtStatus.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtStatus.Location = New System.Drawing.Point(109, 39)
        Me.TxtStatus.Name = "TxtStatus"
        Me.TxtStatus.Size = New System.Drawing.Size(143, 23)
        Me.TxtStatus.TabIndex = 66
        Me.TxtStatus.Visible = False
        '
        'LblSAwal
        '
        Me.LblSAwal.BackColor = System.Drawing.Color.Transparent
        Me.LblSAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblSAwal.Location = New System.Drawing.Point(111, 344)
        Me.LblSAwal.Name = "LblSAwal"
        Me.LblSAwal.Size = New System.Drawing.Size(141, 18)
        Me.LblSAwal.TabIndex = 67
        Me.LblSAwal.Text = "Rp. 0"
        Me.LblSAwal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Yellow
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GreenYellow
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GreenYellow
        Me.BtnClose.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.Black
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnClose.Location = New System.Drawing.Point(1241, 0)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(30, 30)
        Me.BtnClose.TabIndex = 172
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'FormTabelReferensi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1271, 632)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.LblSAwal)
        Me.Controls.Add(Me.TxtStatus)
        Me.Controls.Add(Me.BtnSimpan)
        Me.Controls.Add(Me.BtnHapus)
        Me.Controls.Add(Me.BtnKOsong)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.CmbJenisAkunCari)
        Me.Controls.Add(Me.CmbJenisAkun)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.CmbType)
        Me.Controls.Add(Me.CmbNRLR)
        Me.Controls.Add(Me.CmbDK)
        Me.Controls.Add(Me.CmbSubAkun)
        Me.Controls.Add(Me.TxtSaldoAwal)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TxtNama)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtKode)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Dgvdata)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormTabelReferensi"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.Dgvdata, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CmbType As System.Windows.Forms.ComboBox
    Friend WithEvents CmbNRLR As System.Windows.Forms.ComboBox
    Friend WithEvents CmbDK As System.Windows.Forms.ComboBox
    Friend WithEvents CmbSubAkun As System.Windows.Forms.ComboBox
    Friend WithEvents TxtSaldoAwal As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtNama As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Dgvdata As System.Windows.Forms.DataGridView
    Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents CmbJenisAkun As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents CmbJenisAkunCari As System.Windows.Forms.ComboBox
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents BtnHapus As System.Windows.Forms.Button
    Friend WithEvents BtnKOsong As System.Windows.Forms.Button
    Friend WithEvents TxtStatus As System.Windows.Forms.TextBox
    Friend WithEvents LblSAwal As System.Windows.Forms.Label
    Friend WithEvents BtnClose As System.Windows.Forms.Button

End Class
