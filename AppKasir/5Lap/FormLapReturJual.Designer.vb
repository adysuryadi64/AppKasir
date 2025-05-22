<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapReturJual
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapReturJual))
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource2 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource3 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.retur_penjualanBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        'Me.PossDataSet = New AppKasir.PossDataSet
        Me.retur_penjualan_detailBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.retur_penjualan_barangBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
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
        Me.ReportViewerReturJual = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelReturJual = New System.Windows.Forms.Panel()
        Me.PanelReturJualDetail = New System.Windows.Forms.Panel()
        Me.ReportViewerReturJualDetail = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelReturBarang = New System.Windows.Forms.Panel()
        Me.ReportViewerReturBarang = New Microsoft.Reporting.WinForms.ReportViewer()
        'Me.retur_penjualanTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_penjualanTableAdapter()
        'Me.retur_penjualan_detailTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_penjualan_detailTableAdapter()
        'Me.retur_penjualan_barangTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_penjualan_barangTableAdapter()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        CType(Me.retur_penjualanBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.retur_penjualan_detailBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.retur_penjualan_barangBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.PanelReturJual.SuspendLayout()
        Me.PanelReturJualDetail.SuspendLayout()
        Me.PanelReturBarang.SuspendLayout()
        Me.SuspendLayout()
        '
        'retur_penjualanBindingSource
        '
        Me.retur_penjualanBindingSource.DataMember = "retur_penjualan"
        'Me.retur_penjualanBindingSource.DataSource = Me.PossDataSet
        '
        'PossDataSet
        '
        'Me.PossDataSet.DataSetName = "PossDataSet"
        'Me.PossDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'retur_penjualan_detailBindingSource
        '
        Me.retur_penjualan_detailBindingSource.DataMember = "retur_penjualan_detail"
        'Me.retur_penjualan_detailBindingSource.DataSource = Me.PossDataSet
        '
        'retur_penjualan_barangBindingSource
        '
        Me.retur_penjualan_barangBindingSource.DataMember = "retur_penjualan_barang"
        'Me.retur_penjualan_barangBindingSource.DataSource = Me.PossDataSet
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.DTPAkhir)
        Me.Panel1.Controls.Add(Me.TxtRekening)
        Me.Panel1.Controls.Add(Me.BtnClose)
        Me.Panel1.Controls.Add(Me.DTPAwal)
        Me.Panel1.Controls.Add(Me.CbTanggal)
        Me.Panel1.Controls.Add(Me.LabelJudul)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.CmbRekening)
        Me.Panel1.Controls.Add(Me.CmbKasir)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.CmbBln)
        Me.Panel1.Controls.Add(Me.CmbThn)
        Me.Panel1.Controls.Add(Me.CbBulan)
        Me.Panel1.Controls.Add(Me.BtnTampilkan)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1081, 99)
        Me.Panel1.TabIndex = 126
        '
        'TxtRekening
        '
        Me.TxtRekening.Location = New System.Drawing.Point(879, 67)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.Size = New System.Drawing.Size(100, 20)
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
        Me.BtnClose.Location = New System.Drawing.Point(1047, 0)
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
        Me.LabelJudul.Size = New System.Drawing.Size(1081, 31)
        Me.LabelJudul.TabIndex = 124
        Me.LabelJudul.Text = "LAPORAN RETUR PENJUALAN"
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
        Me.CmbRekening.Size = New System.Drawing.Size(218, 25)
        Me.CmbRekening.TabIndex = 159
        '
        'CmbKasir
        '
        Me.CmbKasir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKasir.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbKasir.FormattingEnabled = True
        Me.CmbKasir.Location = New System.Drawing.Point(446, 38)
        Me.CmbKasir.Name = "CmbKasir"
        Me.CmbKasir.Size = New System.Drawing.Size(218, 25)
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
        Me.BtnTampilkan.Location = New System.Drawing.Point(681, 57)
        Me.BtnTampilkan.Name = "BtnTampilkan"
        Me.BtnTampilkan.Size = New System.Drawing.Size(156, 35)
        Me.BtnTampilkan.TabIndex = 147
        Me.BtnTampilkan.Text = "    TAMPILKAN"
        Me.BtnTampilkan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilkan.UseVisualStyleBackColor = False
        '
        'ReportViewerReturJual
        '
        Me.ReportViewerReturJual.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.retur_penjualanBindingSource
        Me.ReportViewerReturJual.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewerReturJual.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturPenjualan.rdlc"
        Me.ReportViewerReturJual.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturJual.Name = "ReportViewerReturJual"
        Me.ReportViewerReturJual.Size = New System.Drawing.Size(1081, 431)
        Me.ReportViewerReturJual.TabIndex = 127
        '
        'PanelReturJual
        '
        Me.PanelReturJual.Controls.Add(Me.ReportViewerReturJual)
        Me.PanelReturJual.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelReturJual.Location = New System.Drawing.Point(0, 99)
        Me.PanelReturJual.Name = "PanelReturJual"
        Me.PanelReturJual.Size = New System.Drawing.Size(1081, 431)
        Me.PanelReturJual.TabIndex = 128
        '
        'PanelReturJualDetail
        '
        Me.PanelReturJualDetail.Controls.Add(Me.ReportViewerReturJualDetail)
        Me.PanelReturJualDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelReturJualDetail.Location = New System.Drawing.Point(0, 99)
        Me.PanelReturJualDetail.Name = "PanelReturJualDetail"
        Me.PanelReturJualDetail.Size = New System.Drawing.Size(1081, 431)
        Me.PanelReturJualDetail.TabIndex = 129
        '
        'ReportViewerReturJualDetail
        '
        Me.ReportViewerReturJualDetail.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource2.Name = "DataSet1"
        ReportDataSource2.Value = Me.retur_penjualan_detailBindingSource
        Me.ReportViewerReturJualDetail.LocalReport.DataSources.Add(ReportDataSource2)
        Me.ReportViewerReturJualDetail.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturJualDetail.rdlc"
        Me.ReportViewerReturJualDetail.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturJualDetail.Name = "ReportViewerReturJualDetail"
        Me.ReportViewerReturJualDetail.Size = New System.Drawing.Size(1081, 431)
        Me.ReportViewerReturJualDetail.TabIndex = 131
        '
        'PanelReturBarang
        '
        Me.PanelReturBarang.Controls.Add(Me.ReportViewerReturBarang)
        Me.PanelReturBarang.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelReturBarang.Location = New System.Drawing.Point(0, 99)
        Me.PanelReturBarang.Name = "PanelReturBarang"
        Me.PanelReturBarang.Size = New System.Drawing.Size(1081, 431)
        Me.PanelReturBarang.TabIndex = 130
        '
        'ReportViewerReturBarang
        '
        Me.ReportViewerReturBarang.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource3.Name = "DataSet1"
        ReportDataSource3.Value = Me.retur_penjualan_barangBindingSource
        Me.ReportViewerReturBarang.LocalReport.DataSources.Add(ReportDataSource3)
        Me.ReportViewerReturBarang.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturJualBarang.rdlc"
        Me.ReportViewerReturBarang.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturBarang.Name = "ReportViewerReturBarang"
        Me.ReportViewerReturBarang.Size = New System.Drawing.Size(1081, 431)
        Me.ReportViewerReturBarang.TabIndex = 132
        '
        'retur_penjualanTableAdapter
        '
        'Me.retur_penjualanTableAdapter.ClearBeforeFill = True
        '
        'retur_penjualan_detailTableAdapter
        '
        'Me.retur_penjualan_detailTableAdapter.ClearBeforeFill = True
        '
        'retur_penjualan_barangTableAdapter
        '
        'Me.retur_penjualan_barangTableAdapter.ClearBeforeFill = True
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
        'FormLapReturJual
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1081, 530)
        Me.Controls.Add(Me.PanelReturBarang)
        Me.Controls.Add(Me.PanelReturJualDetail)
        Me.Controls.Add(Me.PanelReturJual)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormLapReturJual"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.retur_penjualanBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.retur_penjualan_detailBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.retur_penjualan_barangBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.PanelReturJual.ResumeLayout(False)
        Me.PanelReturJualDetail.ResumeLayout(False)
        Me.PanelReturBarang.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
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
    Friend WithEvents ReportViewerReturJual As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelReturJual As System.Windows.Forms.Panel
    Friend WithEvents PanelReturJualDetail As System.Windows.Forms.Panel
    Friend WithEvents PanelReturBarang As System.Windows.Forms.Panel
    Friend WithEvents ReportViewerReturJualDetail As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewerReturBarang As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents retur_penjualanBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents retur_penjualanTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_penjualanTableAdapter
    Friend WithEvents retur_penjualan_detailBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents retur_penjualan_detailTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_penjualan_detailTableAdapter
    Friend WithEvents retur_penjualan_barangBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents retur_penjualan_barangTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_penjualan_barangTableAdapter
    Friend WithEvents TxtRekening As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir As System.Windows.Forms.DateTimePicker
End Class
