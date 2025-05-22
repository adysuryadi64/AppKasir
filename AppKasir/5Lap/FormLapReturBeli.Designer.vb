<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapReturBeli
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapReturBeli))
        Dim ReportDataSource4 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource5 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource6 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.retur_pembelian_barangBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        'Me.PossDataSet = New AppKasir.PossDataSet
        Me.retur_pembelian_detailBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.retur_pembelianBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
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
        Me.ReportViewerReturBarang = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelReturBarang = New System.Windows.Forms.Panel()
        Me.ReportViewerReturBeliDetail = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelReturBeliDetail = New System.Windows.Forms.Panel()
        Me.PanelReturBeli = New System.Windows.Forms.Panel()
        Me.ReportViewerReturBeli = New Microsoft.Reporting.WinForms.ReportViewer()
        'Me.retur_pembelianTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_pembelianTableAdapter()
        'Me.retur_pembelian_detailTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_pembelian_detailTableAdapter()
        'Me.retur_pembelian_barangTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.retur_pembelian_barangTableAdapter()
        CType(Me.retur_pembelian_barangBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.retur_pembelian_detailBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.retur_pembelianBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.PanelReturBarang.SuspendLayout()
        Me.PanelReturBeliDetail.SuspendLayout()
        Me.PanelReturBeli.SuspendLayout()
        Me.SuspendLayout()
        '
        'retur_pembelian_barangBindingSource
        '
        Me.retur_pembelian_barangBindingSource.DataMember = "retur_pembelian_barang"
        'Me.retur_pembelian_barangBindingSource.DataSource = Me.PossDataSet
        '
        'PossDataSet
        '
        'Me.PossDataSet.DataSetName = "PossDataSet"
        'Me.PossDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'retur_pembelian_detailBindingSource
        '
        Me.retur_pembelian_detailBindingSource.DataMember = "retur_pembelian_detail"
        'Me.retur_pembelian_detailBindingSource.DataSource = Me.PossDataSet
        '
        'retur_pembelianBindingSource
        '
        Me.retur_pembelianBindingSource.DataMember = "retur_pembelian"
        'Me.retur_pembelianBindingSource.DataSource = Me.PossDataSet
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
        Me.Panel1.Size = New System.Drawing.Size(1093, 99)
        Me.Panel1.TabIndex = 131
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.Location = New System.Drawing.Point(194, 42)
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
        Me.DTPAkhir.Location = New System.Drawing.Point(226, 39)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(91, 23)
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
        Me.BtnClose.Location = New System.Drawing.Point(1059, 0)
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
        Me.LabelJudul.Size = New System.Drawing.Size(1093, 31)
        Me.LabelJudul.TabIndex = 124
        Me.LabelJudul.Text = "LAPORAN RETUR PEMBELIAN"
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
        Me.CmbRekening.Size = New System.Drawing.Size(287, 25)
        Me.CmbRekening.TabIndex = 159
        '
        'CmbKasir
        '
        Me.CmbKasir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKasir.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbKasir.FormattingEnabled = True
        Me.CmbKasir.Location = New System.Drawing.Point(446, 38)
        Me.CmbKasir.Name = "CmbKasir"
        Me.CmbKasir.Size = New System.Drawing.Size(287, 25)
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
        Me.BtnTampilkan.Location = New System.Drawing.Point(772, 57)
        Me.BtnTampilkan.Name = "BtnTampilkan"
        Me.BtnTampilkan.Size = New System.Drawing.Size(156, 35)
        Me.BtnTampilkan.TabIndex = 147
        Me.BtnTampilkan.Text = "    TAMPILKAN"
        Me.BtnTampilkan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilkan.UseVisualStyleBackColor = False
        '
        'ReportViewerReturBarang
        '
        Me.ReportViewerReturBarang.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource4.Name = "DataSet1"
        ReportDataSource4.Value = Me.retur_pembelian_barangBindingSource
        Me.ReportViewerReturBarang.LocalReport.DataSources.Add(ReportDataSource4)
        Me.ReportViewerReturBarang.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturBeliBarang.rdlc"
        Me.ReportViewerReturBarang.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturBarang.Name = "ReportViewerReturBarang"
        Me.ReportViewerReturBarang.Size = New System.Drawing.Size(1093, 428)
        Me.ReportViewerReturBarang.TabIndex = 132
        '
        'PanelReturBarang
        '
        Me.PanelReturBarang.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelReturBarang.Controls.Add(Me.ReportViewerReturBarang)
        Me.PanelReturBarang.Location = New System.Drawing.Point(0, 98)
        Me.PanelReturBarang.Name = "PanelReturBarang"
        Me.PanelReturBarang.Size = New System.Drawing.Size(1093, 428)
        Me.PanelReturBarang.TabIndex = 134
        '
        'ReportViewerReturBeliDetail
        '
        Me.ReportViewerReturBeliDetail.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource5.Name = "DataSet1"
        ReportDataSource5.Value = Me.retur_pembelian_detailBindingSource
        Me.ReportViewerReturBeliDetail.LocalReport.DataSources.Add(ReportDataSource5)
        Me.ReportViewerReturBeliDetail.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturBeliDetail.rdlc"
        Me.ReportViewerReturBeliDetail.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturBeliDetail.Name = "ReportViewerReturBeliDetail"
        Me.ReportViewerReturBeliDetail.Size = New System.Drawing.Size(1092, 428)
        Me.ReportViewerReturBeliDetail.TabIndex = 131
        '
        'PanelReturBeliDetail
        '
        Me.PanelReturBeliDetail.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelReturBeliDetail.Controls.Add(Me.ReportViewerReturBeliDetail)
        Me.PanelReturBeliDetail.Location = New System.Drawing.Point(1, 98)
        Me.PanelReturBeliDetail.Name = "PanelReturBeliDetail"
        Me.PanelReturBeliDetail.Size = New System.Drawing.Size(1092, 428)
        Me.PanelReturBeliDetail.TabIndex = 133
        '
        'PanelReturBeli
        '
        Me.PanelReturBeli.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelReturBeli.Controls.Add(Me.ReportViewerReturBeli)
        Me.PanelReturBeli.Location = New System.Drawing.Point(0, 98)
        Me.PanelReturBeli.Name = "PanelReturBeli"
        Me.PanelReturBeli.Size = New System.Drawing.Size(1093, 428)
        Me.PanelReturBeli.TabIndex = 132
        '
        'ReportViewerReturBeli
        '
        Me.ReportViewerReturBeli.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource6.Name = "DataSet1"
        ReportDataSource6.Value = Me.retur_pembelianBindingSource
        Me.ReportViewerReturBeli.LocalReport.DataSources.Add(ReportDataSource6)
        Me.ReportViewerReturBeli.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturPembelian.rdlc"
        Me.ReportViewerReturBeli.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturBeli.Name = "ReportViewerReturBeli"
        Me.ReportViewerReturBeli.Size = New System.Drawing.Size(1093, 428)
        Me.ReportViewerReturBeli.TabIndex = 127
        '
        'retur_pembelianTableAdapter
        '
        'Me.retur_pembelianTableAdapter.ClearBeforeFill = True
        '
        'retur_pembelian_detailTableAdapter
        '
        'Me.retur_pembelian_detailTableAdapter.ClearBeforeFill = True
        '
        'retur_pembelian_barangTableAdapter
        '
        'Me.retur_pembelian_barangTableAdapter.ClearBeforeFill = True
        '
        'FormLapReturBeli
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1093, 526)
        Me.Controls.Add(Me.PanelReturBeliDetail)
        Me.Controls.Add(Me.PanelReturBarang)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelReturBeli)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormLapReturBeli"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.retur_pembelian_barangBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.retur_pembelian_detailBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.retur_pembelianBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.PanelReturBarang.ResumeLayout(False)
        Me.PanelReturBeliDetail.ResumeLayout(False)
        Me.PanelReturBeli.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
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
    Friend WithEvents ReportViewerReturBarang As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelReturBarang As System.Windows.Forms.Panel
    Friend WithEvents ReportViewerReturBeliDetail As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelReturBeliDetail As System.Windows.Forms.Panel
    Friend WithEvents PanelReturBeli As System.Windows.Forms.Panel
    Friend WithEvents ReportViewerReturBeli As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents retur_pembelianBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents retur_pembelianTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_pembelianTableAdapter
    Friend WithEvents retur_pembelian_detailBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents retur_pembelian_detailTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_pembelian_detailTableAdapter
    Friend WithEvents retur_pembelian_barangBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents retur_pembelian_barangTableAdapter As AppKasir.PossDataSetLancarTableAdapters.retur_pembelian_barangTableAdapter
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir As System.Windows.Forms.DateTimePicker
End Class
