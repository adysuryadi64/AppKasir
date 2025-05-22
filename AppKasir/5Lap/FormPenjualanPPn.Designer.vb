<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPenjualanPPn
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormPenjualanPPn))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnFilter = New System.Windows.Forms.Button()
        Me.BtnNonFilter = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.TxtRekening = New System.Windows.Forms.TextBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LabelJudul = New System.Windows.Forms.Label()
        Me.BtnTampilkan = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.RbtKategori = New System.Windows.Forms.RadioButton()
        Me.RbtNama = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtKunci = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.DGVFilter = New System.Windows.Forms.DataGridView()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.DGVFilter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.BtnFilter)
        Me.Panel1.Controls.Add(Me.BtnNonFilter)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.DTPAkhir)
        Me.Panel1.Controls.Add(Me.TxtRekening)
        Me.Panel1.Controls.Add(Me.DTPAwal)
        Me.Panel1.Controls.Add(Me.CbTanggal)
        Me.Panel1.Controls.Add(Me.CmbBln)
        Me.Panel1.Controls.Add(Me.CmbThn)
        Me.Panel1.Controls.Add(Me.CbBulan)
        Me.Panel1.Location = New System.Drawing.Point(666, 88)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(319, 170)
        Me.Panel1.TabIndex = 132
        '
        'BtnFilter
        '
        Me.BtnFilter.AutoSize = True
        Me.BtnFilter.BackColor = System.Drawing.Color.Gold
        Me.BtnFilter.FlatAppearance.BorderSize = 0
        Me.BtnFilter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnFilter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnFilter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnFilter.ForeColor = System.Drawing.Color.Black
        Me.BtnFilter.Image = CType(resources.GetObject("BtnFilter.Image"), System.Drawing.Image)
        Me.BtnFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnFilter.Location = New System.Drawing.Point(12, 81)
        Me.BtnFilter.Name = "BtnFilter"
        Me.BtnFilter.Size = New System.Drawing.Size(167, 30)
        Me.BtnFilter.TabIndex = 148
        Me.BtnFilter.Text = "Export Excell Terfilter"
        Me.BtnFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnFilter.UseVisualStyleBackColor = False
        '
        'BtnNonFilter
        '
        Me.BtnNonFilter.AutoSize = True
        Me.BtnNonFilter.BackColor = System.Drawing.Color.Gold
        Me.BtnNonFilter.FlatAppearance.BorderSize = 0
        Me.BtnNonFilter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnNonFilter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnNonFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnNonFilter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnNonFilter.ForeColor = System.Drawing.Color.Black
        Me.BtnNonFilter.Image = CType(resources.GetObject("BtnNonFilter.Image"), System.Drawing.Image)
        Me.BtnNonFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnNonFilter.Location = New System.Drawing.Point(11, 127)
        Me.BtnNonFilter.Name = "BtnNonFilter"
        Me.BtnNonFilter.Size = New System.Drawing.Size(179, 30)
        Me.BtnNonFilter.TabIndex = 149
        Me.BtnNonFilter.Text = "Export Excell Non Filter"
        Me.BtnNonFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnNonFilter.UseVisualStyleBackColor = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(184, 10)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(23, 15)
        Me.Label6.TabIndex = 207
        Me.Label6.Text = "s/d"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DTPAkhir
        '
        Me.DTPAkhir.CustomFormat = "dd-MM-yyyy"
        Me.DTPAkhir.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAkhir.Location = New System.Drawing.Point(208, 7)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(91, 21)
        Me.DTPAkhir.TabIndex = 206
        '
        'TxtRekening
        '
        Me.TxtRekening.Location = New System.Drawing.Point(970, 67)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.Size = New System.Drawing.Size(100, 20)
        Me.TxtRekening.TabIndex = 203
        Me.TxtRekening.Visible = False
        '
        'DTPAwal
        '
        Me.DTPAwal.CustomFormat = "dd-MM-yyyy"
        Me.DTPAwal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAwal.Location = New System.Drawing.Point(92, 7)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(91, 21)
        Me.DTPAwal.TabIndex = 201
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(12, 8)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(71, 19)
        Me.CbTanggal.TabIndex = 200
        Me.CbTanggal.Text = "Tanggal"
        Me.CbTanggal.UseVisualStyleBackColor = True
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(92, 34)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(101, 23)
        Me.CmbBln.TabIndex = 131
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(194, 34)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(64, 23)
        Me.CmbThn.TabIndex = 132
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbBulan.Location = New System.Drawing.Point(11, 36)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(58, 19)
        Me.CbBulan.TabIndex = 137
        Me.CbBulan.Text = "Bulan"
        Me.CbBulan.UseVisualStyleBackColor = True
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
        Me.BtnClose.Location = New System.Drawing.Point(955, 0)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(30, 30)
        Me.BtnClose.TabIndex = 202
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LabelJudul
        '
        Me.LabelJudul.BackColor = System.Drawing.Color.Gold
        Me.LabelJudul.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabelJudul.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelJudul.Location = New System.Drawing.Point(0, 0)
        Me.LabelJudul.Name = "LabelJudul"
        Me.LabelJudul.Size = New System.Drawing.Size(985, 31)
        Me.LabelJudul.TabIndex = 124
        Me.LabelJudul.Text = "LAPORAN PENJUALAN PPn/ non PPn"
        Me.LabelJudul.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnTampilkan
        '
        Me.BtnTampilkan.AutoSize = True
        Me.BtnTampilkan.BackColor = System.Drawing.Color.Gold
        Me.BtnTampilkan.FlatAppearance.BorderSize = 0
        Me.BtnTampilkan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnTampilkan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnTampilkan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampilkan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampilkan.ForeColor = System.Drawing.Color.Black
        Me.BtnTampilkan.Image = CType(resources.GetObject("BtnTampilkan.Image"), System.Drawing.Image)
        Me.BtnTampilkan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampilkan.Location = New System.Drawing.Point(523, 14)
        Me.BtnTampilkan.Name = "BtnTampilkan"
        Me.BtnTampilkan.Size = New System.Drawing.Size(125, 30)
        Me.BtnTampilkan.TabIndex = 147
        Me.BtnTampilkan.Text = "Tambah Filter"
        Me.BtnTampilkan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilkan.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.RbtKategori)
        Me.Panel2.Controls.Add(Me.RbtNama)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.TxtKunci)
        Me.Panel2.Controls.Add(Me.BtnTampilkan)
        Me.Panel2.Location = New System.Drawing.Point(0, 34)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(660, 51)
        Me.Panel2.TabIndex = 133
        '
        'RbtKategori
        '
        Me.RbtKategori.AutoSize = True
        Me.RbtKategori.Checked = True
        Me.RbtKategori.Location = New System.Drawing.Point(6, 7)
        Me.RbtKategori.Name = "RbtKategori"
        Me.RbtKategori.Size = New System.Drawing.Size(100, 17)
        Me.RbtKategori.TabIndex = 149
        Me.RbtKategori.TabStop = True
        Me.RbtKategori.Text = "Kategori barang"
        Me.RbtKategori.UseVisualStyleBackColor = True
        '
        'RbtNama
        '
        Me.RbtNama.AutoSize = True
        Me.RbtNama.Location = New System.Drawing.Point(6, 27)
        Me.RbtNama.Name = "RbtNama"
        Me.RbtNama.Size = New System.Drawing.Size(89, 17)
        Me.RbtNama.TabIndex = 148
        Me.RbtNama.TabStop = True
        Me.RbtNama.Text = "Nama barang"
        Me.RbtNama.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(124, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(243, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Kata kunci filter barang (Pisahkan dengan koma ,)"
        '
        'TxtKunci
        '
        Me.TxtKunci.Location = New System.Drawing.Point(124, 24)
        Me.TxtKunci.Name = "TxtKunci"
        Me.TxtKunci.Size = New System.Drawing.Size(393, 20)
        Me.TxtKunci.TabIndex = 0
        '
        'Panel3
        '
        Me.Panel3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Panel3.Controls.Add(Me.DGVFilter)
        Me.Panel3.Location = New System.Drawing.Point(0, 88)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(660, 465)
        Me.Panel3.TabIndex = 134
        '
        'DGVFilter
        '
        Me.DGVFilter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DGVFilter.BackgroundColor = System.Drawing.Color.White
        Me.DGVFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVFilter.Location = New System.Drawing.Point(0, 0)
        Me.DGVFilter.Name = "DGVFilter"
        Me.DGVFilter.Size = New System.Drawing.Size(660, 465)
        Me.DGVFilter.TabIndex = 0
        '
        'FormPenjualanPPn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(985, 557)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LabelJudul)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormPenjualanPPn"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormPenjualanPPn"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        CType(Me.DGVFilter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label6 As Label
    Friend WithEvents DTPAkhir As DateTimePicker
    Friend WithEvents TxtRekening As TextBox
    Friend WithEvents BtnClose As Button
    Friend WithEvents DTPAwal As DateTimePicker
    Friend WithEvents CbTanggal As CheckBox
    Friend WithEvents LabelJudul As Label
    Friend WithEvents CmbBln As ComboBox
    Friend WithEvents CmbThn As ComboBox
    Friend WithEvents CbBulan As CheckBox
    Friend WithEvents BtnTampilkan As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents TxtKunci As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents DGVFilter As DataGridView
    Friend WithEvents BtnFilter As Button
    Friend WithEvents BtnNonFilter As Button
    Friend WithEvents RbtKategori As RadioButton
    Friend WithEvents RbtNama As RadioButton
End Class
