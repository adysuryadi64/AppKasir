<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapStokMinim_takGerak
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapStokMinim_takGerak))
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.LblTidakTerjualSejak = New System.Windows.Forms.Label()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.LblLokasi = New System.Windows.Forms.Label()
        Me.CmbLokasi = New System.Windows.Forms.ComboBox()
        Me.LblKategori = New System.Windows.Forms.Label()
        Me.CmbKategori = New System.Windows.Forms.ComboBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.LblLabelTotalItem = New System.Windows.Forms.Label()
        Me.LblTotalItem = New System.Windows.Forms.Label()
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
        Me.LblHeaderForm.Text = "LAPORAN STOK MINIMUM"
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
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportBarangTidakBergerak.rdlc"
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
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportStokMinimum.rdlc"
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
        Me.PanelKiri.Controls.Add(Me.LblTidakTerjualSejak)
        Me.PanelKiri.Controls.Add(Me.CbTanggal)
        Me.PanelKiri.Controls.Add(Me.DTPAwal)
        Me.PanelKiri.Controls.Add(Me.Label1)
        Me.PanelKiri.Controls.Add(Me.DTPAkhir)
        Me.PanelKiri.Controls.Add(Me.CbBulan)
        Me.PanelKiri.Controls.Add(Me.CmbBln)
        Me.PanelKiri.Controls.Add(Me.CmbThn)
        Me.PanelKiri.Controls.Add(Me.LblLokasi)
        Me.PanelKiri.Controls.Add(Me.CmbLokasi)
        Me.PanelKiri.Controls.Add(Me.LblKategori)
        Me.PanelKiri.Controls.Add(Me.CmbKategori)
        Me.PanelKiri.Controls.Add(Me.BtnTampil)
        Me.PanelKiri.Controls.Add(Me.LblLabelTotalItem)
        Me.PanelKiri.Controls.Add(Me.LblTotalItem)
        Me.PanelKiri.Location = New System.Drawing.Point(0, 34)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Size = New System.Drawing.Size(370, 434)
        Me.PanelKiri.TabIndex = 2
        '
        'LblTidakTerjualSejak
        '
        Me.LblTidakTerjualSejak.AutoSize = True
        Me.LblTidakTerjualSejak.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTidakTerjualSejak.ForeColor = System.Drawing.Color.DarkRed
        Me.LblTidakTerjualSejak.Location = New System.Drawing.Point(12, 0)
        Me.LblTidakTerjualSejak.Name = "LblTidakTerjualSejak"
        Me.LblTidakTerjualSejak.Size = New System.Drawing.Size(118, 16)
        Me.LblTidakTerjualSejak.TabIndex = 16
        Me.LblTidakTerjualSejak.Text = "Tidak terjual sejak :"
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(12, 17)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(103, 21)
        Me.CbTanggal.TabIndex = 2
        Me.CbTanggal.Text = "Per Tanggal"
        '
        'DTPAwal
        '
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAwal.Location = New System.Drawing.Point(12, 41)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(155, 23)
        Me.DTPAwal.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(172, 45)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(28, 17)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "s/d"
        '
        'DTPAkhir
        '
        Me.DTPAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAkhir.Location = New System.Drawing.Point(204, 41)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(148, 23)
        Me.DTPAkhir.TabIndex = 5
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbBulan.Location = New System.Drawing.Point(12, 75)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(86, 21)
        Me.CbBulan.TabIndex = 6
        Me.CbBulan.Text = "Per Bulan"
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Enabled = False
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(12, 99)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(155, 25)
        Me.CmbBln.TabIndex = 7
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Enabled = False
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(175, 99)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(90, 25)
        Me.CmbThn.TabIndex = 8
        '
        'LblLokasi
        '
        Me.LblLokasi.AutoSize = True
        Me.LblLokasi.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLokasi.Location = New System.Drawing.Point(12, 139)
        Me.LblLokasi.Name = "LblLokasi"
        Me.LblLokasi.Size = New System.Drawing.Size(50, 17)
        Me.LblLokasi.TabIndex = 9
        Me.LblLokasi.Text = "Lokasi :"
        '
        'CmbLokasi
        '
        Me.CmbLokasi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbLokasi.FormattingEnabled = True
        Me.CmbLokasi.Items.AddRange(New Object() {"SEMUA", "TOKO", "GUDANG"})
        Me.CmbLokasi.Location = New System.Drawing.Point(12, 159)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(120, 25)
        Me.CmbLokasi.TabIndex = 10
        '
        'LblKategori
        '
        Me.LblKategori.AutoSize = True
        Me.LblKategori.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKategori.Location = New System.Drawing.Point(12, 194)
        Me.LblKategori.Name = "LblKategori"
        Me.LblKategori.Size = New System.Drawing.Size(65, 17)
        Me.LblKategori.TabIndex = 14
        Me.LblKategori.Text = "Kategori :"
        '
        'CmbKategori
        '
        Me.CmbKategori.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKategori.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbKategori.FormattingEnabled = True
        Me.CmbKategori.Location = New System.Drawing.Point(12, 214)
        Me.CmbKategori.Name = "CmbKategori"
        Me.CmbKategori.Size = New System.Drawing.Size(270, 25)
        Me.CmbKategori.TabIndex = 15
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
        Me.BtnTampil.Location = New System.Drawing.Point(12, 254)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(233, 33)
        Me.BtnTampil.TabIndex = 11
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'LblLabelTotalItem
        '
        Me.LblLabelTotalItem.AutoSize = True
        Me.LblLabelTotalItem.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLabelTotalItem.Location = New System.Drawing.Point(12, 316)
        Me.LblLabelTotalItem.Name = "LblLabelTotalItem"
        Me.LblLabelTotalItem.Size = New System.Drawing.Size(73, 17)
        Me.LblLabelTotalItem.TabIndex = 12
        Me.LblLabelTotalItem.Text = "Total Item :"
        '
        'LblTotalItem
        '
        Me.LblTotalItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalItem.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalItem.Location = New System.Drawing.Point(110, 314)
        Me.LblTotalItem.Name = "LblTotalItem"
        Me.LblTotalItem.Size = New System.Drawing.Size(120, 20)
        Me.LblTotalItem.TabIndex = 13
        Me.LblTotalItem.Text = "0"
        Me.LblTotalItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FormLapStokMinim_takGerak
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1168, 468)
        Me.Controls.Add(Me.PanelKiri)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapStokMinim_takGerak"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Laporan Stok Minimum"
        Me.Panel1.ResumeLayout(False)
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm             As System.Windows.Forms.Label
    Friend WithEvents Panel1               As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1        As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer2        As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelKiri            As System.Windows.Forms.Panel
    Friend WithEvents LblTidakTerjualSejak As System.Windows.Forms.Label
    Friend WithEvents CbTanggal            As System.Windows.Forms.CheckBox
    Friend WithEvents DTPAwal              As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1               As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir             As System.Windows.Forms.DateTimePicker
    Friend WithEvents CbBulan              As System.Windows.Forms.CheckBox
    Friend WithEvents CmbBln               As System.Windows.Forms.ComboBox
    Friend WithEvents CmbThn               As System.Windows.Forms.ComboBox
    Friend WithEvents LblLokasi            As System.Windows.Forms.Label
    Friend WithEvents CmbLokasi            As System.Windows.Forms.ComboBox
    Friend WithEvents LblKategori          As System.Windows.Forms.Label
    Friend WithEvents CmbKategori          As System.Windows.Forms.ComboBox
    Friend WithEvents BtnTampil            As System.Windows.Forms.Button
    Friend WithEvents LblLabelTotalItem    As System.Windows.Forms.Label
    Friend WithEvents LblTotalItem         As System.Windows.Forms.Label
End Class
