<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapRekapBayar
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapRekapBayar))
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.LblNama = New System.Windows.Forms.Label()
        Me.CmbNama = New System.Windows.Forms.ComboBox()
        Me.LblLokasi = New System.Windows.Forms.Label()
        Me.CmbLokasi = New System.Windows.Forms.ComboBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.LblLabelTotal1 = New System.Windows.Forms.Label()
        Me.LblTotal1 = New System.Windows.Forms.Label()
        Me.LblLabelTotal2 = New System.Windows.Forms.Label()
        Me.LblTotal2 = New System.Windows.Forms.Label()
        Me.LblLabelTotal3 = New System.Windows.Forms.Label()
        Me.LblTotal3 = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.PanelKiri.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.SystemColors.Control
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1168, 31)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "REKAP BAYAR HUTANG"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer2)
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Location = New System.Drawing.Point(376, 34)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(792, 434)
        Me.Panel1.TabIndex = 1
        '
        'ReportViewer2
        '
        Me.ReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportRekapBayarPiutang.rdlc"
        Me.ReportViewer2.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer2.Name = "ReportViewer2"
        Me.ReportViewer2.ServerReport.BearerToken = Nothing
        Me.ReportViewer2.Size = New System.Drawing.Size(792, 434)
        Me.ReportViewer2.TabIndex = 1
        Me.ReportViewer2.Visible = False
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportRekapBayarHutang.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(792, 434)
        Me.ReportViewer1.TabIndex = 0
        '
        'PanelKiri
        '
        Me.PanelKiri.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PanelKiri.Controls.Add(Me.CbTanggal)
        Me.PanelKiri.Controls.Add(Me.DTPAwal)
        Me.PanelKiri.Controls.Add(Me.Label1)
        Me.PanelKiri.Controls.Add(Me.DTPAkhir)
        Me.PanelKiri.Controls.Add(Me.CbBulan)
        Me.PanelKiri.Controls.Add(Me.CmbBln)
        Me.PanelKiri.Controls.Add(Me.CmbThn)
        Me.PanelKiri.Controls.Add(Me.LblNama)
        Me.PanelKiri.Controls.Add(Me.CmbNama)
        Me.PanelKiri.Controls.Add(Me.LblLokasi)
        Me.PanelKiri.Controls.Add(Me.CmbLokasi)
        Me.PanelKiri.Controls.Add(Me.BtnTampil)
        Me.PanelKiri.Controls.Add(Me.LblLabelTotal1)
        Me.PanelKiri.Controls.Add(Me.LblTotal1)
        Me.PanelKiri.Controls.Add(Me.LblLabelTotal2)
        Me.PanelKiri.Controls.Add(Me.LblTotal2)
        Me.PanelKiri.Controls.Add(Me.LblLabelTotal3)
        Me.PanelKiri.Controls.Add(Me.LblTotal3)
        Me.PanelKiri.Location = New System.Drawing.Point(0, 34)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Size = New System.Drawing.Size(370, 434)
        Me.PanelKiri.TabIndex = 2
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CbTanggal.Location = New System.Drawing.Point(12, 12)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(103, 21)
        Me.CbTanggal.TabIndex = 0
        Me.CbTanggal.Text = "Per Tanggal"
        '
        'DTPAwal
        '
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAwal.Location = New System.Drawing.Point(12, 36)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(155, 23)
        Me.DTPAwal.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label1.Location = New System.Drawing.Point(172, 40)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(28, 17)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "s/d"
        '
        'DTPAkhir
        '
        Me.DTPAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAkhir.Location = New System.Drawing.Point(204, 36)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(148, 23)
        Me.DTPAkhir.TabIndex = 3
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CbBulan.Location = New System.Drawing.Point(12, 70)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(86, 21)
        Me.CbBulan.TabIndex = 4
        Me.CbBulan.Text = "Per Bulan"
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Enabled = False
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(12, 94)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(155, 25)
        Me.CmbBln.TabIndex = 5
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Enabled = False
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(175, 94)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(90, 25)
        Me.CmbThn.TabIndex = 6
        '
        'LblNama
        '
        Me.LblNama.AutoSize = True
        Me.LblNama.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblNama.Location = New System.Drawing.Point(12, 134)
        Me.LblNama.Name = "LblNama"
        Me.LblNama.Size = New System.Drawing.Size(62, 17)
        Me.LblNama.TabIndex = 7
        Me.LblNama.Text = "Supplier :"
        '
        'CmbNama
        '
        Me.CmbNama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbNama.FormattingEnabled = True
        Me.CmbNama.Location = New System.Drawing.Point(12, 154)
        Me.CmbNama.Name = "CmbNama"
        Me.CmbNama.Size = New System.Drawing.Size(270, 25)
        Me.CmbNama.TabIndex = 8
        '
        'LblLokasi
        '
        Me.LblLokasi.AutoSize = True
        Me.LblLokasi.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblLokasi.Location = New System.Drawing.Point(12, 190)
        Me.LblLokasi.Name = "LblLokasi"
        Me.LblLokasi.Size = New System.Drawing.Size(50, 17)
        Me.LblLokasi.TabIndex = 9
        Me.LblLokasi.Text = "Lokasi :"
        '
        'CmbLokasi
        '
        Me.CmbLokasi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbLokasi.FormattingEnabled = True
        Me.CmbLokasi.Items.AddRange(New Object() {"SEMUA", "TOKO", "GUDANG"})
        Me.CmbLokasi.Location = New System.Drawing.Point(12, 210)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(120, 25)
        Me.CmbLokasi.TabIndex = 10
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.BackColor = System.Drawing.SystemColors.Control
        Me.BtnTampil.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTampil.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnTampil.FlatAppearance.BorderSize = 1
        Me.BtnTampil.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnTampil.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnTampil.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampil.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampil.ForeColor = System.Drawing.Color.Black
        Me.BtnTampil.Image = CType(resources.GetObject("BtnTampil.Image"), System.Drawing.Image)
        Me.BtnTampil.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.Location = New System.Drawing.Point(12, 250)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(233, 33)
        Me.BtnTampil.TabIndex = 11
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'LblLabelTotal1
        '
        Me.LblLabelTotal1.AutoSize = True
        Me.LblLabelTotal1.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblLabelTotal1.Location = New System.Drawing.Point(12, 310)
        Me.LblLabelTotal1.Name = "LblLabelTotal1"
        Me.LblLabelTotal1.Size = New System.Drawing.Size(89, 17)
        Me.LblLabelTotal1.TabIndex = 12
        Me.LblLabelTotal1.Text = "Total Hutang :"
        '
        'LblTotal1
        '
        Me.LblTotal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotal1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblTotal1.Location = New System.Drawing.Point(120, 308)
        Me.LblTotal1.Name = "LblTotal1"
        Me.LblTotal1.Size = New System.Drawing.Size(140, 20)
        Me.LblTotal1.TabIndex = 13
        Me.LblTotal1.Text = "0"
        Me.LblTotal1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblLabelTotal2
        '
        Me.LblLabelTotal2.AutoSize = True
        Me.LblLabelTotal2.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblLabelTotal2.Location = New System.Drawing.Point(12, 336)
        Me.LblLabelTotal2.Name = "LblLabelTotal2"
        Me.LblLabelTotal2.Size = New System.Drawing.Size(92, 17)
        Me.LblLabelTotal2.TabIndex = 14
        Me.LblLabelTotal2.Text = "Total Dibayar :"
        '
        'LblTotal2
        '
        Me.LblTotal2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotal2.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblTotal2.Location = New System.Drawing.Point(120, 334)
        Me.LblTotal2.Name = "LblTotal2"
        Me.LblTotal2.Size = New System.Drawing.Size(140, 20)
        Me.LblTotal2.TabIndex = 15
        Me.LblTotal2.Text = "0"
        Me.LblTotal2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblLabelTotal3
        '
        Me.LblLabelTotal3.AutoSize = True
        Me.LblLabelTotal3.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblLabelTotal3.Location = New System.Drawing.Point(12, 362)
        Me.LblLabelTotal3.Name = "LblLabelTotal3"
        Me.LblLabelTotal3.Size = New System.Drawing.Size(66, 16)
        Me.LblLabelTotal3.TabIndex = 16
        Me.LblLabelTotal3.Text = "Total Sisa :"
        '
        'LblTotal3
        '
        Me.LblTotal3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotal3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblTotal3.Location = New System.Drawing.Point(120, 360)
        Me.LblTotal3.Name = "LblTotal3"
        Me.LblTotal3.Size = New System.Drawing.Size(140, 20)
        Me.LblTotal3.TabIndex = 17
        Me.LblTotal3.Text = "0"
        Me.LblTotal3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FormLapRekapBayar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1168, 468)
        Me.Controls.Add(Me.PanelKiri)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapRekapBayar"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Rekap Bayar Hutang"
        Me.Panel1.ResumeLayout(False)
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm        As System.Windows.Forms.Label
    Friend WithEvents Panel1          As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1   As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer2   As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelKiri       As System.Windows.Forms.Panel
    Friend WithEvents CbTanggal       As System.Windows.Forms.CheckBox
    Friend WithEvents DTPAwal         As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1          As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir        As System.Windows.Forms.DateTimePicker
    Friend WithEvents CbBulan         As System.Windows.Forms.CheckBox
    Friend WithEvents CmbBln          As System.Windows.Forms.ComboBox
    Friend WithEvents CmbThn          As System.Windows.Forms.ComboBox
    Friend WithEvents LblNama         As System.Windows.Forms.Label
    Friend WithEvents CmbNama         As System.Windows.Forms.ComboBox
    Friend WithEvents LblLokasi       As System.Windows.Forms.Label
    Friend WithEvents CmbLokasi       As System.Windows.Forms.ComboBox
    Friend WithEvents BtnTampil       As System.Windows.Forms.Button
    Friend WithEvents LblLabelTotal1  As System.Windows.Forms.Label
    Friend WithEvents LblTotal1       As System.Windows.Forms.Label
    Friend WithEvents LblLabelTotal2  As System.Windows.Forms.Label
    Friend WithEvents LblTotal2       As System.Windows.Forms.Label
    Friend WithEvents LblLabelTotal3  As System.Windows.Forms.Label
    Friend WithEvents LblTotal3       As System.Windows.Forms.Label
End Class
