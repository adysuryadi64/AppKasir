<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapBarangTerlaris
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapBarangTerlaris))
        Me.BarangTerlarisBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CmbJumlah = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.CmbLokasi = New System.Windows.Forms.ComboBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LblTotalItem = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblTotalQty = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LblTotalOmset = New System.Windows.Forms.Label()
        CType(Me.BarangTerlarisBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.PanelKiri.SuspendLayout()
        Me.SuspendLayout()
        '
        'BarangTerlarisBindingSource
        '
        Me.BarangTerlarisBindingSource.DataMember = "BarangTerlaris"
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Gold
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1168, 31)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "LAPORAN BARANG TERLARIS"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Location = New System.Drawing.Point(376, 34)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(792, 434)
        Me.Panel1.TabIndex = 1
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportBarangTerlaris.rdlc"
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
        Me.PanelKiri.Controls.Add(Me.Label2)
        Me.PanelKiri.Controls.Add(Me.CmbJumlah)
        Me.PanelKiri.Controls.Add(Me.Label7)
        Me.PanelKiri.Controls.Add(Me.CmbLokasi)
        Me.PanelKiri.Controls.Add(Me.BtnTampil)
        Me.PanelKiri.Controls.Add(Me.Label3)
        Me.PanelKiri.Controls.Add(Me.LblTotalItem)
        Me.PanelKiri.Controls.Add(Me.Label4)
        Me.PanelKiri.Controls.Add(Me.LblTotalQty)
        Me.PanelKiri.Controls.Add(Me.Label5)
        Me.PanelKiri.Controls.Add(Me.LblTotalOmset)
        Me.PanelKiri.Location = New System.Drawing.Point(0, 34)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Size = New System.Drawing.Size(370, 434)
        Me.PanelKiri.TabIndex = 2
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
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAwal.Location = New System.Drawing.Point(12, 36)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(155, 23)
        Me.DTPAwal.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(172, 40)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(28, 17)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "s/d"
        '
        'DTPAkhir
        '
        Me.DTPAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAkhir.Location = New System.Drawing.Point(204, 36)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(155, 23)
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
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 134)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(73, 17)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Tampilkan :"
        '
        'CmbJumlah
        '
        Me.CmbJumlah.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJumlah.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbJumlah.FormattingEnabled = True
        Me.CmbJumlah.Location = New System.Drawing.Point(12, 153)
        Me.CmbJumlah.Name = "CmbJumlah"
        Me.CmbJumlah.Size = New System.Drawing.Size(110, 25)
        Me.CmbJumlah.TabIndex = 8
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(135, 134)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(50, 17)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Lokasi :"
        '
        'CmbLokasi
        '
        Me.CmbLokasi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbLokasi.FormattingEnabled = True
        Me.CmbLokasi.Items.AddRange(New Object() {"SEMUA", "TOKO", "GUDANG"})
        Me.CmbLokasi.Location = New System.Drawing.Point(135, 153)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(110, 25)
        Me.CmbLokasi.TabIndex = 10
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.BackColor = System.Drawing.Color.White
        Me.BtnTampil.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTampil.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnTampil.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnTampil.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnTampil.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampil.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampil.ForeColor = System.Drawing.Color.Black
        Me.BtnTampil.Image = CType(resources.GetObject("BtnTampil.Image"), System.Drawing.Image)
        Me.BtnTampil.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.Location = New System.Drawing.Point(12, 190)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(132, 33)
        Me.BtnTampil.TabIndex = 11
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(53, 258)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 17)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "tem :"
        '
        'LblTotalItem
        '
        Me.LblTotalItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalItem.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalItem.Location = New System.Drawing.Point(100, 256)
        Me.LblTotalItem.Name = "LblTotalItem"
        Me.LblTotalItem.Size = New System.Drawing.Size(120, 20)
        Me.LblTotalItem.TabIndex = 13
        Me.LblTotalItem.Text = "0"
        Me.LblTotalItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(27, 284)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(67, 17)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Total Qty :"
        '
        'LblTotalQty
        '
        Me.LblTotalQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalQty.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalQty.Location = New System.Drawing.Point(100, 282)
        Me.LblTotalQty.Name = "LblTotalQty"
        Me.LblTotalQty.Size = New System.Drawing.Size(120, 20)
        Me.LblTotalQty.TabIndex = 15
        Me.LblTotalQty.Text = "0"
        Me.LblTotalQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(9, 312)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(85, 17)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "Total Omset :"
        '
        'LblTotalOmset
        '
        Me.LblTotalOmset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalOmset.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalOmset.Location = New System.Drawing.Point(100, 310)
        Me.LblTotalOmset.Name = "LblTotalOmset"
        Me.LblTotalOmset.Size = New System.Drawing.Size(230, 20)
        Me.LblTotalOmset.TabIndex = 17
        Me.LblTotalOmset.Text = "0"
        Me.LblTotalOmset.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FormLapBarangTerlaris
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1168, 468)
        Me.Controls.Add(Me.PanelKiri)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapBarangTerlaris"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.BarangTerlarisBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm      As System.Windows.Forms.Label
    Friend WithEvents Panel1        As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelKiri     As System.Windows.Forms.Panel
    Friend WithEvents CbTanggal     As System.Windows.Forms.CheckBox
    Friend WithEvents DTPAwal       As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1        As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir      As System.Windows.Forms.DateTimePicker
    Friend WithEvents CbBulan       As System.Windows.Forms.CheckBox
    Friend WithEvents CmbBln        As System.Windows.Forms.ComboBox
    Friend WithEvents CmbThn        As System.Windows.Forms.ComboBox
    Friend WithEvents Label2        As System.Windows.Forms.Label
    Friend WithEvents CmbJumlah     As System.Windows.Forms.ComboBox
    Friend WithEvents Label7        As System.Windows.Forms.Label
    Friend WithEvents CmbLokasi     As System.Windows.Forms.ComboBox
    Friend WithEvents BtnTampil     As System.Windows.Forms.Button
    Friend WithEvents Label3        As System.Windows.Forms.Label
    Friend WithEvents LblTotalItem  As System.Windows.Forms.Label
    Friend WithEvents Label4        As System.Windows.Forms.Label
    Friend WithEvents LblTotalQty   As System.Windows.Forms.Label
    Friend WithEvents Label5        As System.Windows.Forms.Label
    Friend WithEvents LblTotalOmset As System.Windows.Forms.Label
    Friend WithEvents BarangTerlarisBindingSource As System.Windows.Forms.BindingSource
End Class
