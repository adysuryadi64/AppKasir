<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapPoin
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Private Sub InitializeComponent()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.PanelFilter = New System.Windows.Forms.Panel()
        Me.BtnTampilkan = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DtpAkhir = New System.Windows.Forms.DateTimePicker()
        Me.DtpAwal = New System.Windows.Forms.DateTimePicker()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.ReportViewer3 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelFilter.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.SystemColors.Control
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold)
        Me.LblHeader.ForeColor = System.Drawing.Color.DarkSlateBlue
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(1066, 34)
        Me.LblHeader.TabIndex = 130
        Me.LblHeader.Text = "LAPORAN MUTASI POIN"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelFilter
        '
        Me.PanelFilter.Controls.Add(Me.BtnTampilkan)
        Me.PanelFilter.Controls.Add(Me.Label2)
        Me.PanelFilter.Controls.Add(Me.Label1)
        Me.PanelFilter.Controls.Add(Me.DtpAkhir)
        Me.PanelFilter.Controls.Add(Me.DtpAwal)
        Me.PanelFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFilter.Location = New System.Drawing.Point(0, 34)
        Me.PanelFilter.Name = "PanelFilter"
        Me.PanelFilter.Size = New System.Drawing.Size(1066, 40)
        Me.PanelFilter.TabIndex = 131
        '
        'BtnTampilkan
        '
        Me.BtnTampilkan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnTampilkan.Location = New System.Drawing.Point(450, 6)
        Me.BtnTampilkan.Name = "BtnTampilkan"
        Me.BtnTampilkan.Size = New System.Drawing.Size(110, 28)
        Me.BtnTampilkan.TabIndex = 4
        Me.BtnTampilkan.Text = "Tampilkan"
        Me.BtnTampilkan.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label2.Location = New System.Drawing.Point(238, 11)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(60, 16)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Sampai :"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label1.Location = New System.Drawing.Point(20, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 16)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Dari :"
        '
        'DtpAkhir
        '
        Me.DtpAkhir.CustomFormat = "dd/MM/yyyy"
        Me.DtpAkhir.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.DtpAkhir.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpAkhir.Location = New System.Drawing.Point(300, 8)
        Me.DtpAkhir.Name = "DtpAkhir"
        Me.DtpAkhir.Size = New System.Drawing.Size(120, 22)
        Me.DtpAkhir.TabIndex = 1
        '
        'DtpAwal
        '
        Me.DtpAwal.CustomFormat = "dd/MM/yyyy"
        Me.DtpAwal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.DtpAwal.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpAwal.Location = New System.Drawing.Point(68, 8)
        Me.DtpAwal.Name = "DtpAwal"
        Me.DtpAwal.Size = New System.Drawing.Size(120, 22)
        Me.DtpAwal.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 74)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1066, 415)
        Me.Panel1.TabIndex = 132
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportMutasiPoin.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(1066, 415)
        Me.ReportViewer1.TabIndex = 0
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.ReportViewer2)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 74)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1066, 415)
        Me.Panel2.TabIndex = 133
        '
        'ReportViewer2
        '
        Me.ReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportRekapTukarPoin.rdlc"
        Me.ReportViewer2.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer2.Name = "ReportViewer2"
        Me.ReportViewer2.ServerReport.BearerToken = Nothing
        Me.ReportViewer2.Size = New System.Drawing.Size(1066, 415)
        Me.ReportViewer2.TabIndex = 0
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.ReportViewer3)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 74)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1066, 415)
        Me.Panel3.TabIndex = 134
        '
        'ReportViewer3
        '
        Me.ReportViewer3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ReportViewer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer3.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportSaldoPoin.rdlc"
        Me.ReportViewer3.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer3.Name = "ReportViewer3"
        Me.ReportViewer3.ServerReport.BearerToken = Nothing
        Me.ReportViewer3.Size = New System.Drawing.Size(1066, 415)
        Me.ReportViewer3.TabIndex = 0
        '
        'FormLapPoin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1066, 489)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelFilter)
        Me.Controls.Add(Me.LblHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormLapPoin"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Laporan Poin"
        Me.PanelFilter.ResumeLayout(False)
        Me.PanelFilter.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeader As System.Windows.Forms.Label
    Friend WithEvents PanelFilter As System.Windows.Forms.Panel
    Friend WithEvents DtpAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents DtpAkhir As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BtnTampilkan As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer2 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer3 As Microsoft.Reporting.WinForms.ReportViewer
End Class