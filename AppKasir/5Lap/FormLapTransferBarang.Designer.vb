Imports System.Drawing
Imports Microsoft.Reporting.WinForms


<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapTransferBarang
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
        Dim ReportDataSource2 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapTransferBarang))
        Me.ReportViewerReturBarang = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.ReportViewerTF = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelTF = New System.Windows.Forms.Panel()
        Me.PanelReturJual = New System.Windows.Forms.Panel()
        Me.PanelTFDetail = New System.Windows.Forms.Panel()
        Me.ReportViewerTFDetail = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.TxtRekening = New System.Windows.Forms.TextBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.LabelJudul = New System.Windows.Forms.Label()
        Me.PanelReturBarang = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.BtnTampilkan = New System.Windows.Forms.Button()
        Me.PanelTF.SuspendLayout()
        Me.PanelReturJual.SuspendLayout()
        Me.PanelTFDetail.SuspendLayout()
        Me.PanelReturBarang.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ReportViewerReturBarang
        '
        Me.ReportViewerReturBarang.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource2.Name = "DataSet1"
        ReportDataSource2.Value = Nothing
        Me.ReportViewerReturBarang.LocalReport.DataSources.Add(ReportDataSource2)
        Me.ReportViewerReturBarang.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportReturJualBarang.rdlc"
        Me.ReportViewerReturBarang.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerReturBarang.Name = "ReportViewerReturBarang"
        'Me.ReportViewerReturBarang.ServerReport.BearerToken = Nothing
        Me.ReportViewerReturBarang.Size = New System.Drawing.Size(1093, 399)
        Me.ReportViewerReturBarang.TabIndex = 132
        '
        'ReportViewerTF
        '
        Me.ReportViewerTF.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewerTF.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportTFBarang.rdlc"
        Me.ReportViewerTF.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerTF.Name = "ReportViewerTF"
        'Me.ReportViewerTF.ServerReport.BearerToken = Nothing
        Me.ReportViewerTF.Size = New System.Drawing.Size(1093, 399)
        Me.ReportViewerTF.TabIndex = 131
        '
        'PanelTF
        '
        Me.PanelTF.Controls.Add(Me.ReportViewerTF)
        Me.PanelTF.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelTF.Location = New System.Drawing.Point(0, 80)
        Me.PanelTF.Name = "PanelTF"
        Me.PanelTF.Size = New System.Drawing.Size(1093, 399)
        Me.PanelTF.TabIndex = 133
        '
        'PanelReturJual
        '
        Me.PanelReturJual.Controls.Add(Me.PanelTFDetail)
        Me.PanelReturJual.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelReturJual.Location = New System.Drawing.Point(0, 80)
        Me.PanelReturJual.Name = "PanelReturJual"
        Me.PanelReturJual.Size = New System.Drawing.Size(1093, 399)
        Me.PanelReturJual.TabIndex = 132
        '
        'PanelTFDetail
        '
        Me.PanelTFDetail.Controls.Add(Me.ReportViewerTFDetail)
        Me.PanelTFDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelTFDetail.Location = New System.Drawing.Point(0, 0)
        Me.PanelTFDetail.Name = "PanelTFDetail"
        Me.PanelTFDetail.Size = New System.Drawing.Size(1093, 399)
        Me.PanelTFDetail.TabIndex = 134
        '
        'ReportViewerTFDetail
        '
        Me.ReportViewerTFDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewerTFDetail.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportTFBarangDetail.rdlc"
        Me.ReportViewerTFDetail.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewerTFDetail.Name = "ReportViewerTFDetail"
        'Me.ReportViewerTFDetail.ServerReport.BearerToken = Nothing
        Me.ReportViewerTFDetail.Size = New System.Drawing.Size(1093, 399)
        Me.ReportViewerTFDetail.TabIndex = 131
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.Location = New System.Drawing.Point(195, 47)
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
        Me.DTPAkhir.Location = New System.Drawing.Point(227, 44)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(91, 23)
        Me.DTPAkhir.TabIndex = 206
        '
        'TxtRekening
        '
        Me.TxtRekening.Location = New System.Drawing.Point(866, 45)
        Me.TxtRekening.Name = "TxtRekening"
        Me.TxtRekening.Size = New System.Drawing.Size(100, 20)
        Me.TxtRekening.TabIndex = 203
        Me.TxtRekening.Visible = False
        '
        'DTPAwal
        '
        Me.DTPAwal.CustomFormat = "dd-MM-yyyy"
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAwal.Location = New System.Drawing.Point(102, 44)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(91, 23)
        Me.DTPAwal.TabIndex = 201
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
        Me.LabelJudul.Text = "LAPORAN TRANSFER BARANG"
        Me.LabelJudul.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelReturBarang
        '
        Me.PanelReturBarang.Controls.Add(Me.ReportViewerReturBarang)
        Me.PanelReturBarang.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelReturBarang.Location = New System.Drawing.Point(0, 80)
        Me.PanelReturBarang.Name = "PanelReturBarang"
        Me.PanelReturBarang.Size = New System.Drawing.Size(1093, 399)
        Me.PanelReturBarang.TabIndex = 134
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.DTPAkhir)
        Me.Panel1.Controls.Add(Me.TxtRekening)
        Me.Panel1.Controls.Add(Me.BtnClose)
        Me.Panel1.Controls.Add(Me.DTPAwal)
        Me.Panel1.Controls.Add(Me.LabelJudul)
        Me.Panel1.Controls.Add(Me.BtnTampilkan)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1093, 80)
        Me.Panel1.TabIndex = 131
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label1.Location = New System.Drawing.Point(36, 47)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 17)
        Me.Label1.TabIndex = 208
        Me.Label1.Text = "Tanggal"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.BtnTampilkan.Location = New System.Drawing.Point(343, 38)
        Me.BtnTampilkan.Name = "BtnTampilkan"
        Me.BtnTampilkan.Size = New System.Drawing.Size(156, 35)
        Me.BtnTampilkan.TabIndex = 147
        Me.BtnTampilkan.Text = "    TAMPILKAN"
        Me.BtnTampilkan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilkan.UseVisualStyleBackColor = False
        '
        'FormLapTransferBarang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1093, 479)
        Me.Controls.Add(Me.PanelTF)
        Me.Controls.Add(Me.PanelReturJual)
        Me.Controls.Add(Me.PanelReturBarang)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormLapTransferBarang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.PanelTF.ResumeLayout(False)
        Me.PanelReturJual.ResumeLayout(False)
        Me.PanelTFDetail.ResumeLayout(False)
        Me.PanelReturBarang.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ReportViewerReturBarang As ReportViewer
    Friend WithEvents ReportViewerTF As ReportViewer
    Friend WithEvents PanelTF As Panel
    Friend WithEvents PanelReturJual As Panel
    Friend WithEvents Label6 As Label
    Friend WithEvents DTPAkhir As DateTimePicker
    Friend WithEvents TxtRekening As TextBox
    Friend WithEvents BtnClose As Button
    Friend WithEvents DTPAwal As DateTimePicker
    Friend WithEvents LabelJudul As Label
    Friend WithEvents BtnTampilkan As Button
    Friend WithEvents PanelReturBarang As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents PanelTFDetail As Panel
    Friend WithEvents ReportViewerTFDetail As ReportViewer
    Friend WithEvents Label1 As Label
End Class
