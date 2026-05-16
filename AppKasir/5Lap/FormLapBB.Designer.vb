<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapBB
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
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapBB))
        Me.BukuBesarBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        'Me.PossDataSet = New AppKasir.DataSetKL()
        Me.LblTanggal = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DtpAkhir = New System.Windows.Forms.DateTimePicker()
        Me.TxtAkunDK = New System.Windows.Forms.TextBox()
        Me.TxtAkunBB = New System.Windows.Forms.TextBox()
        Me.LblKeterangan = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer3 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.CbmAkunBB = New System.Windows.Forms.ComboBox()
        Me.BtnTampilBB = New System.Windows.Forms.Button()
        Me.DtpAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        'Me.BukuBesarTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.BukuBesarTableAdapter()
        CType(Me.BukuBesarBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BukuBesarBindingSource
        '
        Me.BukuBesarBindingSource.DataMember = "BukuBesar"
        'Me.BukuBesarBindingSource.DataSource = Me.PossDataSet
        '
        'PossDataSet
        '
        'Me.PossDataSet.DataSetName = "PossDataSet"
        'Me.PossDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'LblTanggal
        '
        Me.LblTanggal.AutoSize = True
        Me.LblTanggal.BackColor = System.Drawing.Color.Transparent
        Me.LblTanggal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTanggal.Location = New System.Drawing.Point(94, 165)
        Me.LblTanggal.Name = "LblTanggal"
        Me.LblTanggal.Size = New System.Drawing.Size(109, 16)
        Me.LblTanggal.TabIndex = 213
        Me.LblTanggal.Text = "Sampai Tanggal"
        Me.LblTanggal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.Control
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(314, 27)
        Me.Label1.TabIndex = 212
        Me.Label1.Text = "BUKU BESAR"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DtpAkhir
        '
        Me.DtpAkhir.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAkhir.Location = New System.Drawing.Point(206, 162)
        Me.DtpAkhir.Name = "DtpAkhir"
        Me.DtpAkhir.Size = New System.Drawing.Size(120, 22)
        Me.DtpAkhir.TabIndex = 211
        '
        'TxtAkunDK
        '
        Me.TxtAkunDK.BackColor = System.Drawing.SystemColors.Window
        Me.TxtAkunDK.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAkunDK.Location = New System.Drawing.Point(232, 39)
        Me.TxtAkunDK.Name = "TxtAkunDK"
        Me.TxtAkunDK.Size = New System.Drawing.Size(94, 22)
        Me.TxtAkunDK.TabIndex = 210
        Me.TxtAkunDK.Visible = False
        '
        'TxtAkunBB
        '
        Me.TxtAkunBB.BackColor = System.Drawing.SystemColors.Window
        Me.TxtAkunBB.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAkunBB.Location = New System.Drawing.Point(232, 67)
        Me.TxtAkunBB.Name = "TxtAkunBB"
        Me.TxtAkunBB.Size = New System.Drawing.Size(94, 22)
        Me.TxtAkunBB.TabIndex = 209
        Me.TxtAkunBB.Text = "Kode"
        Me.TxtAkunBB.Visible = False
        '
        'LblKeterangan
        '
        Me.LblKeterangan.AutoSize = True
        Me.LblKeterangan.BackColor = System.Drawing.Color.Transparent
        Me.LblKeterangan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKeterangan.Location = New System.Drawing.Point(12, 70)
        Me.LblKeterangan.Name = "LblKeterangan"
        Me.LblKeterangan.Size = New System.Drawing.Size(78, 16)
        Me.LblKeterangan.TabIndex = 207
        Me.LblKeterangan.Text = "Nama Akun"
        Me.LblKeterangan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer3)
        Me.Panel1.Location = New System.Drawing.Point(332, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(738, 471)
        Me.Panel1.TabIndex = 206
        '
        'ReportViewer3
        '
        Me.ReportViewer3.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.BukuBesarBindingSource
        Me.ReportViewer3.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer3.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportBukuBesar.rdlc"
        Me.ReportViewer3.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer3.Name = "ReportViewer3"
        Me.ReportViewer3.Size = New System.Drawing.Size(738, 471)
        Me.ReportViewer3.TabIndex = 0
        '
        'CbmAkunBB
        '
        Me.CbmAkunBB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbmAkunBB.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbmAkunBB.FormattingEnabled = True
        Me.CbmAkunBB.Location = New System.Drawing.Point(10, 92)
        Me.CbmAkunBB.Name = "CbmAkunBB"
        Me.CbmAkunBB.Size = New System.Drawing.Size(316, 24)
        Me.CbmAkunBB.TabIndex = 205
        '
        'BtnTampilBB
        '
        Me.BtnTampilBB.AutoSize = True
        Me.BtnTampilBB.BackColor = System.Drawing.SystemColors.Control
        Me.BtnTampilBB.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTampilBB.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnTampilBB.FlatAppearance.BorderSize = 1
        Me.BtnTampilBB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnTampilBB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnTampilBB.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampilBB.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampilBB.ForeColor = System.Drawing.Color.Black
        Me.BtnTampilBB.Image = CType(resources.GetObject("BtnTampilBB.Image"), System.Drawing.Image)
        Me.BtnTampilBB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampilBB.Location = New System.Drawing.Point(184, 204)
        Me.BtnTampilBB.Name = "BtnTampilBB"
        Me.BtnTampilBB.Size = New System.Drawing.Size(142, 37)
        Me.BtnTampilBB.TabIndex = 208
        Me.BtnTampilBB.Text = "Tampilkan (F5)"
        Me.BtnTampilBB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampilBB.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampilBB.UseVisualStyleBackColor = False
        '
        'DtpAwal
        '
        Me.DtpAwal.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DtpAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAwal.Location = New System.Drawing.Point(206, 135)
        Me.DtpAwal.Name = "DtpAwal"
        Me.DtpAwal.Size = New System.Drawing.Size(120, 22)
        Me.DtpAwal.TabIndex = 214
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(122, 138)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(81, 16)
        Me.Label2.TabIndex = 215
        Me.Label2.Text = "Dari tanggal"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BukuBesarTableAdapter
        '
        'Me.BukuBesarTableAdapter.ClearBeforeFill = True
        '
        'FormLapBB
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1070, 471)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.DtpAwal)
        Me.Controls.Add(Me.LblTanggal)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.DtpAkhir)
        Me.Controls.Add(Me.TxtAkunDK)
        Me.Controls.Add(Me.TxtAkunBB)
        Me.Controls.Add(Me.BtnTampilBB)
        Me.Controls.Add(Me.LblKeterangan)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.CbmAkunBB)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapBB"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Laporan Buku Besar"
        CType(Me.BukuBesarBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LblTanggal As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DtpAkhir As System.Windows.Forms.DateTimePicker
    Friend WithEvents TxtAkunDK As System.Windows.Forms.TextBox
    Friend WithEvents TxtAkunBB As System.Windows.Forms.TextBox
    Friend WithEvents BtnTampilBB As System.Windows.Forms.Button
    Friend WithEvents LblKeterangan As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer3 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents CbmAkunBB As System.Windows.Forms.ComboBox
    Friend WithEvents DtpAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BukuBesarBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents BukuBesarTableAdapter As AppKasir.PossDataSetLancarTableAdapters.BukuBesarTableAdapter
End Class
