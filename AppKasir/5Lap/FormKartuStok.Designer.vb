<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormKartuStok
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormKartuStok))
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.LblSd = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.LblLokasi = New System.Windows.Forms.Label()
        Me.CmbLokasi = New System.Windows.Forms.ComboBox()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.TxtCari = New System.Windows.Forms.TextBox()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.LblKodeBarang = New System.Windows.Forms.Label()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.LblLabelMasuk = New System.Windows.Forms.Label()
        Me.LblTotalMasuk = New System.Windows.Forms.Label()
        Me.LblLabelKeluar = New System.Windows.Forms.Label()
        Me.LblTotalKeluar = New System.Windows.Forms.Label()
        Me.LblLabelSaldo = New System.Windows.Forms.Label()
        Me.LblSaldoAkhir = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelKiri.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Gold
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1168, 34)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "KARTU STOK"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelKiri
        '
        Me.PanelKiri.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PanelKiri.Controls.Add(Me.CbTanggal)
        Me.PanelKiri.Controls.Add(Me.DTPAwal)
        Me.PanelKiri.Controls.Add(Me.LblSd)
        Me.PanelKiri.Controls.Add(Me.DTPAkhir)
        Me.PanelKiri.Controls.Add(Me.CbBulan)
        Me.PanelKiri.Controls.Add(Me.CmbBln)
        Me.PanelKiri.Controls.Add(Me.CmbThn)
        Me.PanelKiri.Controls.Add(Me.LblLokasi)
        Me.PanelKiri.Controls.Add(Me.CmbLokasi)
        Me.PanelKiri.Controls.Add(Me.PanelCari)
        Me.PanelKiri.Controls.Add(Me.LblKodeBarang)
        Me.PanelKiri.Controls.Add(Me.LstBarang)
        Me.PanelKiri.Controls.Add(Me.BtnTampil)
        Me.PanelKiri.Controls.Add(Me.LblLabelMasuk)
        Me.PanelKiri.Controls.Add(Me.LblTotalMasuk)
        Me.PanelKiri.Controls.Add(Me.LblLabelKeluar)
        Me.PanelKiri.Controls.Add(Me.LblTotalKeluar)
        Me.PanelKiri.Controls.Add(Me.LblLabelSaldo)
        Me.PanelKiri.Controls.Add(Me.LblSaldoAkhir)
        Me.PanelKiri.Location = New System.Drawing.Point(0, 34)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Size = New System.Drawing.Size(370, 600)
        Me.PanelKiri.TabIndex = 1
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(12, 12)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(103, 21)
        Me.CbTanggal.TabIndex = 0
        Me.CbTanggal.Text = "Per Tanggal"
        '
        'DTPAwal
        '
        Me.DTPAwal.CustomFormat = "dd/MM/yyyy"
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAwal.Location = New System.Drawing.Point(12, 36)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(155, 23)
        Me.DTPAwal.TabIndex = 1
        '
        'LblSd
        '
        Me.LblSd.AutoSize = True
        Me.LblSd.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSd.Location = New System.Drawing.Point(172, 40)
        Me.LblSd.Name = "LblSd"
        Me.LblSd.Size = New System.Drawing.Size(28, 17)
        Me.LblSd.TabIndex = 2
        Me.LblSd.Text = "s/d"
        '
        'DTPAkhir
        '
        Me.DTPAkhir.CustomFormat = "dd/MM/yyyy"
        Me.DTPAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAkhir.Location = New System.Drawing.Point(204, 36)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(148, 23)
        Me.DTPAkhir.TabIndex = 3
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(175, 94)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(90, 25)
        Me.CmbThn.TabIndex = 6
        '
        'LblLokasi
        '
        Me.LblLokasi.AutoSize = True
        Me.LblLokasi.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLokasi.Location = New System.Drawing.Point(12, 134)
        Me.LblLokasi.Name = "LblLokasi"
        Me.LblLokasi.Size = New System.Drawing.Size(50, 17)
        Me.LblLokasi.TabIndex = 7
        Me.LblLokasi.Text = "Lokasi :"
        '
        'CmbLokasi
        '
        Me.CmbLokasi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbLokasi.FormattingEnabled = True
        Me.CmbLokasi.Items.AddRange(New Object() {"SEMUA", "TOKO", "GUDANG"})
        Me.CmbLokasi.Location = New System.Drawing.Point(12, 154)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(110, 25)
        Me.CmbLokasi.TabIndex = 8
        '
        'PanelCari
        '
        Me.PanelCari.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.PanelCari.Controls.Add(Me.TxtCari)
        Me.PanelCari.Controls.Add(Me.BtnCari)
        Me.PanelCari.Location = New System.Drawing.Point(12, 192)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(340, 30)
        Me.PanelCari.TabIndex = 9
        '
        'TxtCari
        '
        Me.TxtCari.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtCari.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtCari.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCari.Location = New System.Drawing.Point(4, 5)
        Me.TxtCari.Name = "TxtCari"
        Me.TxtCari.Size = New System.Drawing.Size(308, 23)
        Me.TxtCari.TabIndex = 0
        '
        'BtnCari
        '
        Me.BtnCari.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnCari.BackColor = System.Drawing.Color.White
        Me.BtnCari.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCari.FlatAppearance.BorderSize = 0
        Me.BtnCari.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCari.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.Location = New System.Drawing.Point(316, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(20, 20)
        Me.BtnCari.TabIndex = 1
        Me.BtnCari.UseVisualStyleBackColor = False
        '
        'LblKodeBarang
        '
        Me.LblKodeBarang.AutoSize = True
        Me.LblKodeBarang.Font = New System.Drawing.Font("Century Gothic", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeBarang.Location = New System.Drawing.Point(12, 226)
        Me.LblKodeBarang.Name = "LblKodeBarang"
        Me.LblKodeBarang.Size = New System.Drawing.Size(0, 15)
        Me.LblKodeBarang.TabIndex = 10
        Me.LblKodeBarang.Text = ""
        Me.LblKodeBarang.Visible = False
        '
        'LstBarang
        '
        Me.LstBarang.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LstBarang.FormattingEnabled = True
        Me.LstBarang.ItemHeight = 17
        Me.LstBarang.Location = New System.Drawing.Point(12, 226)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(340, 106)
        Me.LstBarang.TabIndex = 11
        Me.LstBarang.Visible = False
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.BackColor = System.Drawing.Color.White
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
        Me.BtnTampil.Location = New System.Drawing.Point(12, 342)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(200, 33)
        Me.BtnTampil.TabIndex = 12
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'LblLabelMasuk
        '
        Me.LblLabelMasuk.AutoSize = True
        Me.LblLabelMasuk.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLabelMasuk.Location = New System.Drawing.Point(12, 396)
        Me.LblLabelMasuk.Name = "LblLabelMasuk"
        Me.LblLabelMasuk.Size = New System.Drawing.Size(80, 17)
        Me.LblLabelMasuk.TabIndex = 13
        Me.LblLabelMasuk.Text = "Total Masuk :"
        '
        'LblTotalMasuk
        '
        Me.LblTotalMasuk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalMasuk.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalMasuk.Location = New System.Drawing.Point(130, 394)
        Me.LblTotalMasuk.Name = "LblTotalMasuk"
        Me.LblTotalMasuk.Size = New System.Drawing.Size(120, 20)
        Me.LblTotalMasuk.TabIndex = 14
        Me.LblTotalMasuk.Text = "0"
        Me.LblTotalMasuk.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblLabelKeluar
        '
        Me.LblLabelKeluar.AutoSize = True
        Me.LblLabelKeluar.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLabelKeluar.Location = New System.Drawing.Point(12, 422)
        Me.LblLabelKeluar.Name = "LblLabelKeluar"
        Me.LblLabelKeluar.Size = New System.Drawing.Size(80, 17)
        Me.LblLabelKeluar.TabIndex = 15
        Me.LblLabelKeluar.Text = "Total Keluar :"
        '
        'LblTotalKeluar
        '
        Me.LblTotalKeluar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalKeluar.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalKeluar.Location = New System.Drawing.Point(130, 420)
        Me.LblTotalKeluar.Name = "LblTotalKeluar"
        Me.LblTotalKeluar.Size = New System.Drawing.Size(120, 20)
        Me.LblTotalKeluar.TabIndex = 16
        Me.LblTotalKeluar.Text = "0"
        Me.LblTotalKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblLabelSaldo
        '
        Me.LblLabelSaldo.AutoSize = True
        Me.LblLabelSaldo.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLabelSaldo.Location = New System.Drawing.Point(12, 450)
        Me.LblLabelSaldo.Name = "LblLabelSaldo"
        Me.LblLabelSaldo.Size = New System.Drawing.Size(80, 17)
        Me.LblLabelSaldo.TabIndex = 17
        Me.LblLabelSaldo.Text = "Saldo Akhir :"
        '
        'LblSaldoAkhir
        '
        Me.LblSaldoAkhir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblSaldoAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSaldoAkhir.Location = New System.Drawing.Point(130, 448)
        Me.LblSaldoAkhir.Name = "LblSaldoAkhir"
        Me.LblSaldoAkhir.Size = New System.Drawing.Size(120, 20)
        Me.LblSaldoAkhir.TabIndex = 18
        Me.LblSaldoAkhir.Text = "0"
        Me.LblSaldoAkhir.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Location = New System.Drawing.Point(376, 34)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(792, 600)
        Me.Panel1.TabIndex = 2
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Nothing
        Me.ReportViewer1.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportMutasiBarang.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(792, 600)
        Me.ReportViewer1.TabIndex = 0
        '
        'FormKartuStok
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1168, 634)
        Me.Controls.Add(Me.PanelKiri)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormKartuStok"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Kartu Stok"
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm   As System.Windows.Forms.Label
    Friend WithEvents PanelKiri       As System.Windows.Forms.Panel
    Friend WithEvents CbTanggal       As System.Windows.Forms.CheckBox
    Friend WithEvents DTPAwal         As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblSd           As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir        As System.Windows.Forms.DateTimePicker
    Friend WithEvents CbBulan         As System.Windows.Forms.CheckBox
    Friend WithEvents CmbBln          As System.Windows.Forms.ComboBox
    Friend WithEvents CmbThn          As System.Windows.Forms.ComboBox
    Friend WithEvents LblLokasi       As System.Windows.Forms.Label
    Friend WithEvents CmbLokasi       As System.Windows.Forms.ComboBox
    Friend WithEvents PanelCari       As System.Windows.Forms.Panel
    Friend WithEvents TxtCari         As System.Windows.Forms.TextBox
    Friend WithEvents BtnCari         As System.Windows.Forms.Button
    Friend WithEvents LblKodeBarang   As System.Windows.Forms.Label
    Friend WithEvents LstBarang       As System.Windows.Forms.ListBox
    Friend WithEvents BtnTampil       As System.Windows.Forms.Button
    Friend WithEvents LblLabelMasuk   As System.Windows.Forms.Label
    Friend WithEvents LblTotalMasuk   As System.Windows.Forms.Label
    Friend WithEvents LblLabelKeluar  As System.Windows.Forms.Label
    Friend WithEvents LblTotalKeluar  As System.Windows.Forms.Label
    Friend WithEvents LblLabelSaldo   As System.Windows.Forms.Label
    Friend WithEvents LblSaldoAkhir   As System.Windows.Forms.Label
    Friend WithEvents Panel1          As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1   As Microsoft.Reporting.WinForms.ReportViewer
End Class
