<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapPenjualanBaru
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapPenjualanBaru))
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource2 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource3 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource4 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.penjualanBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        'Me.PossDataSet = New AppKasir.PossDataSet
        Me.penjualan_detailBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.retur_penjualan_barangBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PenjualanHutangBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelJudul = New System.Windows.Forms.Panel()
        Me.TxtRekening = New System.Windows.Forms.TextBox()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.LabelJudul = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.CmbRekening = New System.Windows.Forms.ComboBox()
        Me.CmbKasir = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.BtnTampilkan = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.ReportViewer3 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.ReportViewer4 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.ReportViewer5 = New Microsoft.Reporting.WinForms.ReportViewer()
        'Me.penjualanTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.penjualanTableAdapter()
        'Me.penjualan_detailTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.penjualan_detailTableAdapter()
        'Me.retur_penjualan_barangTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_penjualan_barangTableAdapter()
        'Me.PenjualanHutangTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.PenjualanHutangTableAdapter()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        CType(Me.penjualanBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.penjualan_detailBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.retur_penjualan_barangBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PenjualanHutangBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelJudul.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'penjualanBindingSource
        '
        Me.penjualanBindingSource.DataMember = "penjualan"
        'Me.penjualanBindingSource.DataSource = Me.PossDataSet
        '
        'PossDataSet
        '
        'Me.PossDataSet.DataSetName = "PossDataSet"
        'Me.PossDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'penjualan_detailBindingSource
        '
        Me.penjualan_detailBindingSource.DataMember = "penjualan_detail"
        'Me.penjualan_detailBindingSource.DataSource = Me.PossDataSet
        '
        'retur_penjualan_barangBindingSource
        '
        Me.retur_penjualan_barangBindingSource.DataMember = "retur_penjualan_barang"
        'Me.retur_penjualan_barangBindingSource.DataSource = Me.PossDataSet
        '
        'PenjualanHutangBindingSource
        '
        Me.PenjualanHutangBindingSource.DataMember = "PenjualanHutang"
        'Me.PenjualanHutangBindingSource.DataSource = Me.PossDataSet
        '
        'PanelJudul
        '
        Me.PanelJudul.Controls.Add(Me.Label6)
        Me.PanelJudul.Controls.Add(Me.DTPAkhir)
        Me.PanelJudul.Controls.Add(Me.TxtRekening)
        Me.PanelJudul.Controls.Add(Me.BtnClose)
        Me.PanelJudul.Controls.Add(Me.DTPAwal)
        Me.PanelJudul.Controls.Add(Me.CbTanggal)
        Me.PanelJudul.Controls.Add(Me.LabelJudul)
        Me.PanelJudul.Controls.Add(Me.Label5)
        Me.PanelJudul.Controls.Add(Me.CmbRekening)
        Me.PanelJudul.Controls.Add(Me.CmbKasir)
        Me.PanelJudul.Controls.Add(Me.Label4)
        Me.PanelJudul.Controls.Add(Me.CmbBln)
        Me.PanelJudul.Controls.Add(Me.CmbThn)
        Me.PanelJudul.Controls.Add(Me.CbBulan)
        Me.PanelJudul.Controls.Add(Me.BtnTampilkan)
        Me.PanelJudul.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelJudul.Location = New System.Drawing.Point(0, 0)
        Me.PanelJudul.Name = "PanelJudul"
        Me.PanelJudul.Size = New System.Drawing.Size(1066, 99)
        Me.PanelJudul.TabIndex = 133
        '
        'TxtRekening
        '
        Me.TxtRekening.Location = New System.Drawing.Point(966, 69)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.Size = New System.Drawing.Size(72, 20)
        Me.TxtRekening.TabIndex = 203
        Me.TxtRekening.Visible = False
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
        Me.BtnClose.Location = New System.Drawing.Point(1032, 0)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(30, 30)
        Me.BtnClose.TabIndex = 202
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'DTPAwal
        '
        Me.DTPAwal.CustomFormat = "dd-MM-yyyy"
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAwal.Location = New System.Drawing.Point(102, 39)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(91, 23)
        Me.DTPAwal.TabIndex = 201
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(22, 40)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(79, 21)
        Me.CbTanggal.TabIndex = 200
        Me.CbTanggal.Text = "Tanggal"
        Me.CbTanggal.UseVisualStyleBackColor = True
        '
        'LabelJudul
        '
        Me.LabelJudul.BackColor = System.Drawing.Color.Gold
        Me.LabelJudul.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabelJudul.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelJudul.Location = New System.Drawing.Point(0, 0)
        Me.LabelJudul.Name = "LabelJudul"
        Me.LabelJudul.Size = New System.Drawing.Size(1066, 31)
        Me.LabelJudul.TabIndex = 124
        Me.LabelJudul.Text = "LAPORAN PEMBELIAN"
        Me.LabelJudul.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label5.Location = New System.Drawing.Point(299, 71)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(141, 17)
        Me.Label5.TabIndex = 160
        Me.Label5.Text = "Rekening"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmbRekening
        '
        Me.CmbRekening.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbRekening.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbRekening.FormattingEnabled = True
        Me.CmbRekening.Items.AddRange(New Object() {"Semua", "Sudah Lunas", "Belum Lunas"})
        Me.CmbRekening.Location = New System.Drawing.Point(446, 67)
        Me.CmbRekening.Name = "CmbRekening"
        Me.CmbRekening.Size = New System.Drawing.Size(292, 25)
        Me.CmbRekening.TabIndex = 159
        '
        'CmbKasir
        '
        Me.CmbKasir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKasir.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbKasir.FormattingEnabled = True
        Me.CmbKasir.Location = New System.Drawing.Point(446, 38)
        Me.CmbKasir.Name = "CmbKasir"
        Me.CmbKasir.Size = New System.Drawing.Size(292, 25)
        Me.CmbKasir.TabIndex = 127
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label4.Location = New System.Drawing.Point(403, 42)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 17)
        Me.Label4.TabIndex = 157
        Me.Label4.Text = "Kasir"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(103, 67)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(101, 25)
        Me.CmbBln.TabIndex = 131
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(205, 67)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(64, 25)
        Me.CmbThn.TabIndex = 132
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CbBulan.Location = New System.Drawing.Point(22, 69)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(62, 21)
        Me.CbBulan.TabIndex = 137
        Me.CbBulan.Text = "Bulan"
        Me.CbBulan.UseVisualStyleBackColor = True
        '
        'BtnTampilkan
        '
        Me.BtnTampilkan.BackColor = System.Drawing.Color.Gold
        Me.BtnTampilkan.FlatAppearance.BorderSize = 0
        Me.BtnTampilkan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnTampilkan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnTampilkan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampilkan.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampilkan.ForeColor = System.Drawing.Color.Black
        Me.BtnTampilkan.Image = CType(resources.GetObject("BtnTampilkan.Image"), System.Drawing.Image)
        Me.BtnTampilkan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampilkan.Location = New System.Drawing.Point(781, 59)
        Me.BtnTampilkan.Name = "BtnTampilkan"
        Me.BtnTampilkan.Size = New System.Drawing.Size(156, 35)
        Me.BtnTampilkan.TabIndex = 147
        Me.BtnTampilkan.Text = "    TAMPILKAN"
        Me.BtnTampilkan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilkan.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 99)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1066, 390)
        Me.Panel1.TabIndex = 134
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.DocumentMapWidth = 73
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportJualLengkap.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.Size = New System.Drawing.Size(1066, 390)
        Me.ReportViewer1.TabIndex = 136
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.ReportViewer2)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 99)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1066, 390)
        Me.Panel2.TabIndex = 135
        '
        'ReportViewer2
        '
        Me.ReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer2.DocumentMapWidth = 73
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.penjualanBindingSource
        Me.ReportViewer2.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportPenjualan.rdlc"
        Me.ReportViewer2.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer2.Name = "ReportViewer2"
        Me.ReportViewer2.Size = New System.Drawing.Size(1066, 390)
        Me.ReportViewer2.TabIndex = 137
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.ReportViewer3)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 99)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1066, 390)
        Me.Panel3.TabIndex = 135
        '
        'ReportViewer3
        '
        Me.ReportViewer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer3.DocumentMapWidth = 73
        ReportDataSource2.Name = "DataSet1"
        ReportDataSource2.Value = Me.penjualan_detailBindingSource
        Me.ReportViewer3.LocalReport.DataSources.Add(ReportDataSource2)
        Me.ReportViewer3.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportPenjualanDetail.rdlc"
        Me.ReportViewer3.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer3.Name = "ReportViewer3"
        Me.ReportViewer3.Size = New System.Drawing.Size(1066, 390)
        Me.ReportViewer3.TabIndex = 137
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.ReportViewer4)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(0, 99)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1066, 390)
        Me.Panel4.TabIndex = 135
        '
        'ReportViewer4
        '
        Me.ReportViewer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer4.DocumentMapWidth = 73
        ReportDataSource3.Name = "DataSet1"
        ReportDataSource3.Value = Me.retur_penjualan_barangBindingSource
        Me.ReportViewer4.LocalReport.DataSources.Add(ReportDataSource3)
        Me.ReportViewer4.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportJualBarang.rdlc"
        Me.ReportViewer4.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer4.Name = "ReportViewer4"
        Me.ReportViewer4.Size = New System.Drawing.Size(1066, 390)
        Me.ReportViewer4.TabIndex = 137
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.ReportViewer5)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(0, 99)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(1066, 390)
        Me.Panel5.TabIndex = 135
        '
        'ReportViewer5
        '
        Me.ReportViewer5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer5.DocumentMapWidth = 73
        ReportDataSource4.Name = "DataSet1"
        ReportDataSource4.Value = Me.PenjualanHutangBindingSource
        Me.ReportViewer5.LocalReport.DataSources.Add(ReportDataSource4)
        Me.ReportViewer5.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportPenjualanHutang.rdlc"
        Me.ReportViewer5.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer5.Name = "ReportViewer5"
        Me.ReportViewer5.Size = New System.Drawing.Size(1066, 390)
        Me.ReportViewer5.TabIndex = 137
        '
        'penjualanTableAdapter
        '
        'Me.penjualanTableAdapter.ClearBeforeFill = True
        '
        'penjualan_detailTableAdapter
        '
        'Me.penjualan_detailTableAdapter.ClearBeforeFill = True
        '
        'retur_penjualan_barangTableAdapter
        '
        'Me.retur_penjualan_barangTableAdapter.ClearBeforeFill = True
        '
        'PenjualanHutangTableAdapter
        '
        'Me.PenjualanHutangTableAdapter.ClearBeforeFill = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.Location = New System.Drawing.Point(195, 42)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(28, 17)
        Me.Label6.TabIndex = 207
        Me.Label6.Text = "s/d"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DTPAkhir
        '
        Me.DTPAkhir.CustomFormat = "dd-MM-yyyy"
        Me.DTPAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAkhir.Location = New System.Drawing.Point(227, 39)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(91, 23)
        Me.DTPAkhir.TabIndex = 206
        '
        'FormLapPenjualanBaru
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1066, 489)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelJudul)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormLapPenjualanBaru"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormLapPenjualanBaru"
        CType(Me.penjualanBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.penjualan_detailBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.retur_penjualan_barangBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PenjualanHutangBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelJudul.ResumeLayout(False)
        Me.PanelJudul.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelJudul As System.Windows.Forms.Panel
    Friend WithEvents TxtRekening As System.Windows.Forms.TextBox
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents DTPAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents CbTanggal As System.Windows.Forms.CheckBox
    Friend WithEvents LabelJudul As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents CmbRekening As System.Windows.Forms.ComboBox
    Friend WithEvents CmbKasir As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CmbBln As System.Windows.Forms.ComboBox
    Friend WithEvents CmbThn As System.Windows.Forms.ComboBox
    Friend WithEvents CbBulan As System.Windows.Forms.CheckBox
    Friend WithEvents BtnTampilkan As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer2 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer3 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer4 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer5 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents penjualanBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents penjualanTableAdapter As AppKasir.PossDataSetLancarTableAdapters.penjualanTableAdapter
    Friend WithEvents penjualan_detailBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents penjualan_detailTableAdapter As AppKasir.PossDataSetLancarTableAdapters.penjualan_detailTableAdapter
    Friend WithEvents retur_penjualan_barangBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents retur_penjualan_barangTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_penjualan_barangTableAdapter
    Friend WithEvents PenjualanHutangBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PenjualanHutangTableAdapter As AppKasir.PossDataSetLancarTableAdapters.PenjualanHutangTableAdapter
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir As System.Windows.Forms.DateTimePicker
End Class
