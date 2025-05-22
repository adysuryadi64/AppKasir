<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapkAS
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapkAS))
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.LapKASBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.penjualanBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.RbNonBayar = New System.Windows.Forms.RadioButton()
        Me.RbtNonTunai = New System.Windows.Forms.RadioButton()
        Me.RbtTunai = New System.Windows.Forms.RadioButton()
        Me.RbtSemua = New System.Windows.Forms.RadioButton()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtPiutang = New System.Windows.Forms.TextBox()
        Me.TxtDiterima = New System.Windows.Forms.TextBox()
        Me.BtnHitung = New System.Windows.Forms.Button()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.TxtGrantotal = New System.Windows.Forms.TextBox()
        Me.TxtBulanThn = New System.Windows.Forms.TextBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.DtpTanggal = New System.Windows.Forms.DateTimePicker()
        Me.CmbKasir = New System.Windows.Forms.ComboBox()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        CType(Me.LapKASBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.penjualanBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'LapKASBindingSource
        '
        Me.LapKASBindingSource.DataMember = "LapKAS"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.RbNonBayar)
        Me.GroupBox2.Controls.Add(Me.RbtNonTunai)
        Me.GroupBox2.Controls.Add(Me.RbtTunai)
        Me.GroupBox2.Controls.Add(Me.RbtSemua)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.TxtPiutang)
        Me.GroupBox2.Controls.Add(Me.TxtDiterima)
        Me.GroupBox2.Controls.Add(Me.BtnHitung)
        Me.GroupBox2.Controls.Add(Me.CbBulan)
        Me.GroupBox2.Controls.Add(Me.CbTanggal)
        Me.GroupBox2.Controls.Add(Me.TxtGrantotal)
        Me.GroupBox2.Controls.Add(Me.TxtBulanThn)
        Me.GroupBox2.Controls.Add(Me.CmbThn)
        Me.GroupBox2.Controls.Add(Me.CmbBln)
        Me.GroupBox2.Controls.Add(Me.DtpTanggal)
        Me.GroupBox2.Controls.Add(Me.CmbKasir)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(1159, 124)
        Me.GroupBox2.TabIndex = 123
        Me.GroupBox2.TabStop = False
        '
        'RbNonBayar
        '
        Me.RbNonBayar.AutoSize = True
        Me.RbNonBayar.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow
        Me.RbNonBayar.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RbNonBayar.Location = New System.Drawing.Point(496, 88)
        Me.RbNonBayar.Name = "RbNonBayar"
        Me.RbNonBayar.Size = New System.Drawing.Size(103, 22)
        Me.RbNonBayar.TabIndex = 161
        Me.RbNonBayar.TabStop = True
        Me.RbNonBayar.Text = "Non Bayar"
        Me.RbNonBayar.UseVisualStyleBackColor = True
        '
        'RbtNonTunai
        '
        Me.RbtNonTunai.AutoSize = True
        Me.RbtNonTunai.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow
        Me.RbtNonTunai.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RbtNonTunai.Location = New System.Drawing.Point(389, 88)
        Me.RbtNonTunai.Name = "RbtNonTunai"
        Me.RbtNonTunai.Size = New System.Drawing.Size(98, 22)
        Me.RbtNonTunai.TabIndex = 160
        Me.RbtNonTunai.TabStop = True
        Me.RbtNonTunai.Text = "Non Tunai"
        Me.RbtNonTunai.UseVisualStyleBackColor = True
        '
        'RbtTunai
        '
        Me.RbtTunai.AutoSize = True
        Me.RbtTunai.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow
        Me.RbtTunai.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RbtTunai.Location = New System.Drawing.Point(496, 60)
        Me.RbtTunai.Name = "RbtTunai"
        Me.RbtTunai.Size = New System.Drawing.Size(64, 22)
        Me.RbtTunai.TabIndex = 159
        Me.RbtTunai.TabStop = True
        Me.RbtTunai.Text = "Tunai"
        Me.RbtTunai.UseVisualStyleBackColor = True
        '
        'RbtSemua
        '
        Me.RbtSemua.AutoSize = True
        Me.RbtSemua.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow
        Me.RbtSemua.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RbtSemua.Location = New System.Drawing.Point(389, 60)
        Me.RbtSemua.Name = "RbtSemua"
        Me.RbtSemua.Size = New System.Drawing.Size(77, 22)
        Me.RbtSemua.TabIndex = 158
        Me.RbtSemua.TabStop = True
        Me.RbtSemua.Text = "Semua"
        Me.RbtSemua.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(232, 63)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(40, 16)
        Me.Label4.TabIndex = 157
        Me.Label4.Text = "Kasir"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(898, 63)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(126, 19)
        Me.Label3.TabIndex = 156
        Me.Label3.Text = "TOTAL PIUTANG"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label10
        '
        Me.Label10.BackColor = System.Drawing.Color.Gold
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label10.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(3, 16)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(1153, 31)
        Me.Label10.TabIndex = 124
        Me.Label10.Text = "LAPORAN UANG MASUK DARI PENJUALAN"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(1023, 61)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(129, 19)
        Me.Label2.TabIndex = 155
        Me.Label2.Text = "UANG DITERIMA"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(749, 63)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(149, 19)
        Me.Label1.TabIndex = 154
        Me.Label1.Text = "TOTAL PENJUALAN"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPiutang
        '
        Me.TxtPiutang.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtPiutang.BackColor = System.Drawing.Color.Black
        Me.TxtPiutang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPiutang.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtPiutang.ForeColor = System.Drawing.Color.Lime
        Me.TxtPiutang.Location = New System.Drawing.Point(902, 86)
        Me.TxtPiutang.Multiline = True
        Me.TxtPiutang.Name = "TxtPiutang"
        Me.TxtPiutang.ReadOnly = True
        Me.TxtPiutang.Size = New System.Drawing.Size(122, 26)
        Me.TxtPiutang.TabIndex = 153
        Me.TxtPiutang.Text = "000"
        Me.TxtPiutang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtDiterima
        '
        Me.TxtDiterima.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtDiterima.BackColor = System.Drawing.Color.Black
        Me.TxtDiterima.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtDiterima.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtDiterima.ForeColor = System.Drawing.Color.Lime
        Me.TxtDiterima.Location = New System.Drawing.Point(1027, 86)
        Me.TxtDiterima.Multiline = True
        Me.TxtDiterima.Name = "TxtDiterima"
        Me.TxtDiterima.ReadOnly = True
        Me.TxtDiterima.Size = New System.Drawing.Size(125, 26)
        Me.TxtDiterima.TabIndex = 152
        Me.TxtDiterima.Text = "000"
        Me.TxtDiterima.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BtnHitung
        '
        Me.BtnHitung.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.BtnHitung.FlatAppearance.BorderSize = 0
        Me.BtnHitung.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnHitung.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnHitung.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHitung.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHitung.ForeColor = System.Drawing.Color.White
        Me.BtnHitung.Image = CType(resources.GetObject("BtnHitung.Image"), System.Drawing.Image)
        Me.BtnHitung.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHitung.Location = New System.Drawing.Point(616, 82)
        Me.BtnHitung.Name = "BtnHitung"
        Me.BtnHitung.Size = New System.Drawing.Size(127, 30)
        Me.BtnHitung.TabIndex = 147
        Me.BtnHitung.Text = "   HITUNG"
        Me.BtnHitung.UseVisualStyleBackColor = False
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbBulan.Location = New System.Drawing.Point(11, 89)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(64, 20)
        Me.CbBulan.TabIndex = 137
        Me.CbBulan.Text = "Bulan"
        Me.CbBulan.UseVisualStyleBackColor = True
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(11, 61)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(81, 20)
        Me.CbTanggal.TabIndex = 136
        Me.CbTanggal.Text = "Tanggal"
        Me.CbTanggal.UseVisualStyleBackColor = True
        '
        'TxtGrantotal
        '
        Me.TxtGrantotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtGrantotal.BackColor = System.Drawing.Color.Black
        Me.TxtGrantotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtGrantotal.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtGrantotal.ForeColor = System.Drawing.Color.Lime
        Me.TxtGrantotal.Location = New System.Drawing.Point(753, 86)
        Me.TxtGrantotal.Multiline = True
        Me.TxtGrantotal.Name = "TxtGrantotal"
        Me.TxtGrantotal.ReadOnly = True
        Me.TxtGrantotal.Size = New System.Drawing.Size(130, 26)
        Me.TxtGrantotal.TabIndex = 135
        Me.TxtGrantotal.Text = "000"
        Me.TxtGrantotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtBulanThn
        '
        Me.TxtBulanThn.BackColor = System.Drawing.SystemColors.Window
        Me.TxtBulanThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBulanThn.Location = New System.Drawing.Point(148, 41)
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
        Me.CmbThn.Location = New System.Drawing.Point(191, 87)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(52, 24)
        Me.CmbThn.TabIndex = 132
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(95, 87)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(96, 24)
        Me.CmbBln.TabIndex = 131
        '
        'DtpTanggal
        '
        Me.DtpTanggal.CustomFormat = "dd-MM-yyyy"
        Me.DtpTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpTanggal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTanggal.Location = New System.Drawing.Point(95, 60)
        Me.DtpTanggal.Name = "DtpTanggal"
        Me.DtpTanggal.Size = New System.Drawing.Size(117, 23)
        Me.DtpTanggal.TabIndex = 130
        '
        'CmbKasir
        '
        Me.CmbKasir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKasir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbKasir.FormattingEnabled = True
        Me.CmbKasir.Location = New System.Drawing.Point(278, 59)
        Me.CmbKasir.Name = "CmbKasir"
        Me.CmbKasir.Size = New System.Drawing.Size(105, 24)
        Me.CmbKasir.TabIndex = 127
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.LapKASBindingSource
        Me.ReportViewer1.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportKas.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 124)
        Me.ReportViewer1.Name = "ReportViewer1"
        'Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(1159, 498)
        Me.ReportViewer1.TabIndex = 125
        '
        'FormLapkAS
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.YellowGreen
        Me.ClientSize = New System.Drawing.Size(1159, 622)
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.GroupBox2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormLapkAS"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.LapKASBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.penjualanBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents TxtGrantotal As System.Windows.Forms.TextBox
    Friend WithEvents TxtBulanThn As System.Windows.Forms.TextBox
    Friend WithEvents CmbThn As System.Windows.Forms.ComboBox
    Friend WithEvents CmbBln As System.Windows.Forms.ComboBox
    Friend WithEvents DtpTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents CmbKasir As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents CbBulan As CheckBox
    Friend WithEvents CbTanggal As CheckBox
    Friend WithEvents BtnHitung As Button
    Friend WithEvents penjualanBindingSource As System.Windows.Forms.BindingSource


    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtPiutang As System.Windows.Forms.TextBox
    Friend WithEvents TxtDiterima As System.Windows.Forms.TextBox
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents LapKASBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents LapKASTableAdapter As AppKasir.PossDataSetLancarTableAdapters.LapKASTableAdapter
    Friend WithEvents RbtNonTunai As System.Windows.Forms.RadioButton
    Friend WithEvents RbtTunai As System.Windows.Forms.RadioButton
    Friend WithEvents RbtSemua As System.Windows.Forms.RadioButton
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents RbNonBayar As System.Windows.Forms.RadioButton
    'Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
End Class
