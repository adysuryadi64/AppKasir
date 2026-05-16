<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapMarginProfit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapMarginProfit))
        Me.CbMarginAtas = New System.Windows.Forms.CheckBox()
        Me.CbMarginTerendah = New System.Windows.Forms.CheckBox()
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
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BarangTerlarisBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.CbTanggal = New System.Windows.Forms.CheckBox()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.CbBulan = New System.Windows.Forms.CheckBox()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.Panel1.SuspendLayout()
        CType(Me.BarangTerlarisBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelKiri.SuspendLayout()
        Me.SuspendLayout()
        '
        'CbMarginAtas
        '
        Me.CbMarginAtas.AutoSize = True
        Me.CbMarginAtas.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbMarginAtas.Location = New System.Drawing.Point(15, 141)
        Me.CbMarginAtas.Name = "CbMarginAtas"
        Me.CbMarginAtas.Size = New System.Drawing.Size(140, 21)
        Me.CbMarginAtas.TabIndex = 0
        Me.CbMarginAtas.Text = "Margin terbanyak"
        '
        'CbMarginTerendah
        '
        Me.CbMarginTerendah.AutoSize = True
        Me.CbMarginTerendah.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbMarginTerendah.Location = New System.Drawing.Point(15, 177)
        Me.CbMarginTerendah.Name = "CbMarginTerendah"
        Me.CbMarginTerendah.Size = New System.Drawing.Size(116, 21)
        Me.CbMarginTerendah.TabIndex = 4
        Me.CbMarginTerendah.Text = "Margin Sedikit"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(18, 241)
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
        Me.CmbJumlah.Items.AddRange(New Object() {"10", "25", "50", "100"})
        Me.CmbJumlah.Location = New System.Drawing.Point(96, 237)
        Me.CmbJumlah.Name = "CmbJumlah"
        Me.CmbJumlah.Size = New System.Drawing.Size(110, 25)
        Me.CmbJumlah.TabIndex = 8
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(41, 283)
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
        Me.CmbLokasi.Location = New System.Drawing.Point(96, 279)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(110, 25)
        Me.CmbLokasi.TabIndex = 10
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.BackColor = System.Drawing.SystemColors.Control
        Me.BtnTampil.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTampil.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnTampil.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnTampil.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnTampil.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampil.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampil.ForeColor = System.Drawing.Color.Black
        Me.BtnTampil.Image = CType(resources.GetObject("BtnTampil.Image"), System.Drawing.Image)
        Me.BtnTampil.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.Location = New System.Drawing.Point(96, 332)
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
        Me.Label3.Location = New System.Drawing.Point(53, 396)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 17)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "tem :"
        '
        'LblTotalItem
        '
        Me.LblTotalItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalItem.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalItem.Location = New System.Drawing.Point(96, 394)
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
        Me.Label4.Location = New System.Drawing.Point(24, 422)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(67, 17)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Total Qty :"
        '
        'LblTotalQty
        '
        Me.LblTotalQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalQty.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalQty.Location = New System.Drawing.Point(96, 420)
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
        Me.Label5.Location = New System.Drawing.Point(6, 450)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(85, 17)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "Total Omset :"
        '
        'LblTotalOmset
        '
        Me.LblTotalOmset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalOmset.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalOmset.Location = New System.Drawing.Point(96, 448)
        Me.LblTotalOmset.Name = "LblTotalOmset"
        Me.LblTotalOmset.Size = New System.Drawing.Size(120, 20)
        Me.LblTotalOmset.TabIndex = 17
        Me.LblTotalOmset.Text = "0"
        Me.LblTotalOmset.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportMarginProfit.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(694, 502)
        Me.ReportViewer1.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Location = New System.Drawing.Point(457, 36)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(694, 502)
        Me.Panel1.TabIndex = 4
        '
        'BarangTerlarisBindingSource
        '
        Me.BarangTerlarisBindingSource.DataMember = "BarangTerlaris"
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
        Me.PanelKiri.Controls.Add(Me.CbMarginAtas)
        Me.PanelKiri.Controls.Add(Me.CbMarginTerendah)
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
        Me.PanelKiri.Location = New System.Drawing.Point(6, 36)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Size = New System.Drawing.Size(445, 502)
        Me.PanelKiri.TabIndex = 5
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.SystemColors.Control
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1151, 31)
        Me.LblHeaderForm.TabIndex = 3
        Me.LblHeaderForm.Text = "LAPORAN PROFIT MARGIN"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CbTanggal
        '
        Me.CbTanggal.AutoSize = True
        Me.CbTanggal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbTanggal.Location = New System.Drawing.Point(15, 8)
        Me.CbTanggal.Name = "CbTanggal"
        Me.CbTanggal.Size = New System.Drawing.Size(103, 21)
        Me.CbTanggal.TabIndex = 18
        Me.CbTanggal.Text = "Per Tanggal"
        '
        'DTPAwal
        '
        Me.DTPAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAwal.Location = New System.Drawing.Point(15, 32)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(155, 23)
        Me.DTPAwal.TabIndex = 19
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(175, 36)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(28, 17)
        Me.Label1.TabIndex = 20
        Me.Label1.Text = "s/d"
        '
        'DTPAkhir
        '
        Me.DTPAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAkhir.Location = New System.Drawing.Point(207, 32)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(155, 23)
        Me.DTPAkhir.TabIndex = 21
        '
        'CbBulan
        '
        Me.CbBulan.AutoSize = True
        Me.CbBulan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CbBulan.Location = New System.Drawing.Point(15, 66)
        Me.CbBulan.Name = "CbBulan"
        Me.CbBulan.Size = New System.Drawing.Size(86, 21)
        Me.CbBulan.TabIndex = 22
        Me.CbBulan.Text = "Per Bulan"
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Enabled = False
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBln.FormattingEnabled = True
        Me.CmbBln.Location = New System.Drawing.Point(15, 90)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(155, 25)
        Me.CmbBln.TabIndex = 23
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Enabled = False
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbThn.FormattingEnabled = True
        Me.CmbThn.Location = New System.Drawing.Point(178, 90)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(90, 25)
        Me.CmbThn.TabIndex = 24
        '
        'FormLapMarginProfit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1151, 541)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelKiri)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormLapMarginProfit"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormLapMarginProfit"
        Me.Panel1.ResumeLayout(False)
        CType(Me.BarangTerlarisBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CbMarginAtas As CheckBox
    Friend WithEvents CbMarginTerendah As CheckBox
    Friend WithEvents Label2 As Label
    Friend WithEvents CmbJumlah As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents CmbLokasi As ComboBox
    Friend WithEvents BtnTampil As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents LblTotalItem As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents LblTotalQty As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents LblTotalOmset As Label
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BarangTerlarisBindingSource As BindingSource
    Friend WithEvents PanelKiri As Panel
    Friend WithEvents LblHeaderForm As Label
    Friend WithEvents CbTanggal As CheckBox
    Friend WithEvents DTPAwal As DateTimePicker
    Friend WithEvents Label1 As Label
    Friend WithEvents DTPAkhir As DateTimePicker
    Friend WithEvents CbBulan As CheckBox
    Friend WithEvents CmbBln As ComboBox
    Friend WithEvents CmbThn As ComboBox
End Class
