<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapBonPerorang
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapBonPerorang))
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.Temp_Bon_KaryawanBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        'Me.PossDataSet = New AppKasir.DataSetKL()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnTampilBB = New System.Windows.Forms.Button()
        Me.DtpAkhir = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.DtpAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CmbNama = New System.Windows.Forms.ComboBox()
        Me.LblKode = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        'Me.Temp_Bon_KaryawanTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.Temp_Bon_KaryawanTableAdapter()
        CType(Me.Temp_Bon_KaryawanBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHeader.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Temp_Bon_KaryawanBindingSource
        '
        Me.Temp_Bon_KaryawanBindingSource.DataMember = "Temp_Bon_Karyawan"
        'Me.Temp_Bon_KaryawanBindingSource.DataSource = Me.PossDataSet
        '
        'PossDataSet
        '
        'Me.PossDataSet.DataSetName = "PossDataSet"
        'Me.PossDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.SandyBrown
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblUtama)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(985, 34)
        Me.PanelHeader.TabIndex = 3
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.SandyBrown
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.ForeColor = System.Drawing.Color.Black
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(951, 2)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 31)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblUtama
        '
        Me.LblUtama.BackColor = System.Drawing.Color.Transparent
        Me.LblUtama.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblUtama.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.Color.Black
        Me.LblUtama.Location = New System.Drawing.Point(0, 0)
        Me.LblUtama.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(985, 34)
        Me.LblUtama.TabIndex = 20
        Me.LblUtama.Text = "LAPORAN BON KARYAWAN PER KARYAWAN"
        Me.LblUtama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.Panel1.AutoScroll = True
        Me.Panel1.BackColor = System.Drawing.Color.OldLace
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.BtnTampilBB)
        Me.Panel1.Controls.Add(Me.DtpAkhir)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.DtpAwal)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.CmbNama)
        Me.Panel1.Controls.Add(Me.LblKode)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 34)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(985, 40)
        Me.Panel1.TabIndex = 5
        '
        'BtnTampilBB
        '
        Me.BtnTampilBB.BackColor = System.Drawing.Color.Yellow
        Me.BtnTampilBB.FlatAppearance.BorderSize = 0
        Me.BtnTampilBB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnTampilBB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnTampilBB.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampilBB.ForeColor = System.Drawing.Color.Black
        Me.BtnTampilBB.Image = CType(resources.GetObject("BtnTampilBB.Image"), System.Drawing.Image)
        Me.BtnTampilBB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampilBB.Location = New System.Drawing.Point(758, 2)
        Me.BtnTampilBB.Name = "BtnTampilBB"
        Me.BtnTampilBB.Size = New System.Drawing.Size(142, 34)
        Me.BtnTampilBB.TabIndex = 266
        Me.BtnTampilBB.Text = "Tampilkan"
        Me.BtnTampilBB.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilBB.UseVisualStyleBackColor = False
        '
        'DtpAkhir
        '
        Me.DtpAkhir.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAkhir.Location = New System.Drawing.Point(610, 7)
        Me.DtpAkhir.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpAkhir.Name = "DtpAkhir"
        Me.DtpAkhir.Size = New System.Drawing.Size(111, 23)
        Me.DtpAkhir.TabIndex = 239
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(580, 10)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 16)
        Me.Label1.TabIndex = 238
        Me.Label1.Text = "s/d "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(356, 10)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(103, 16)
        Me.Label3.TabIndex = 227
        Me.Label3.Text = "Periode kerja :"
        '
        'DtpAwal
        '
        Me.DtpAwal.CalendarFont = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAwal.Location = New System.Drawing.Point(466, 7)
        Me.DtpAwal.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DtpAwal.Name = "DtpAwal"
        Me.DtpAwal.Size = New System.Drawing.Size(111, 23)
        Me.DtpAwal.TabIndex = 228
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label6.Location = New System.Drawing.Point(34, 10)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(56, 16)
        Me.Label6.TabIndex = 255
        Me.Label6.Text = "Nama :"
        '
        'CmbNama
        '
        Me.CmbNama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbNama.FormattingEnabled = True
        Me.CmbNama.Location = New System.Drawing.Point(93, 6)
        Me.CmbNama.Name = "CmbNama"
        Me.CmbNama.Size = New System.Drawing.Size(174, 24)
        Me.CmbNama.TabIndex = 2
        '
        'LblKode
        '
        Me.LblKode.AutoSize = True
        Me.LblKode.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKode.Location = New System.Drawing.Point(278, 10)
        Me.LblKode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblKode.Name = "LblKode"
        Me.LblKode.Size = New System.Drawing.Size(41, 16)
        Me.LblKode.TabIndex = 265
        Me.LblKode.Text = "Kode"
        Me.LblKode.Visible = False
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.ReportViewer1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 74)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(985, 420)
        Me.Panel2.TabIndex = 6
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.Temp_Bon_KaryawanBindingSource
        Me.ReportViewer1.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportBonPerorang.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.Size = New System.Drawing.Size(985, 420)
        Me.ReportViewer1.TabIndex = 0
        '
        'Temp_Bon_KaryawanTableAdapter
        '
        'Me.Temp_Bon_KaryawanTableAdapter.ClearBeforeFill = True
        '
        'FormLapBonPerorang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(985, 494)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapBonPerorang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.Temp_Bon_KaryawanBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHeader.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblUtama As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents DtpAkhir As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents DtpAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents CmbNama As System.Windows.Forms.ComboBox
    Friend WithEvents LblKode As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents BtnTampilBB As System.Windows.Forms.Button
    Friend WithEvents Temp_Bon_KaryawanBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents Temp_Bon_KaryawanTableAdapter As AppKasir.PossDataSetLancarTableAdapters.Temp_Bon_KaryawanTableAdapter
End Class
