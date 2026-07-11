<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapStokLampau
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapStokLampau))
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelFilter = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DTPTanggal = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CmbLokasi = New System.Windows.Forms.ComboBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelTotal = New System.Windows.Forms.Panel()
        Me.LblTotalItem = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtNilaiToko = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtNilaiGudang = New System.Windows.Forms.TextBox()
        Me.PanelFilter.SuspendLayout()
        Me.PanelTotal.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 13.0!, System.Drawing.FontStyle.Bold)
        Me.LblHeaderForm.ForeColor = System.Drawing.Color.DarkRed
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1100, 36)
        Me.LblHeaderForm.TabIndex = 3
        Me.LblHeaderForm.Text = "LAPORAN STOK BARANG MASA LAMPAU"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelFilter
        '
        Me.PanelFilter.Controls.Add(Me.Label1)
        Me.PanelFilter.Controls.Add(Me.DTPTanggal)
        Me.PanelFilter.Controls.Add(Me.Label2)
        Me.PanelFilter.Controls.Add(Me.CmbLokasi)
        Me.PanelFilter.Controls.Add(Me.BtnTampil)
        Me.PanelFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFilter.Location = New System.Drawing.Point(0, 36)
        Me.PanelFilter.Name = "PanelFilter"
        Me.PanelFilter.Size = New System.Drawing.Size(1100, 44)
        Me.PanelFilter.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 15)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Per Tanggal :"
        '
        'DTPTanggal
        '
        Me.DTPTanggal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPTanggal.Location = New System.Drawing.Point(80, 9)
        Me.DTPTanggal.Name = "DTPTanggal"
        Me.DTPTanggal.Size = New System.Drawing.Size(120, 21)
        Me.DTPTanggal.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(215, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(50, 15)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Lokasi :"
        '
        'CmbLokasi
        '
        Me.CmbLokasi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbLokasi.Items.AddRange(New Object() {"SEMUA", "TOKO", "GUDANG"})
        Me.CmbLokasi.Location = New System.Drawing.Point(260, 9)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(110, 23)
        Me.CmbLokasi.TabIndex = 3
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.BackColor = System.Drawing.SystemColors.Control
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
        Me.BtnTampil.Location = New System.Drawing.Point(385, 7)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(90, 28)
        Me.BtnTampil.TabIndex = 4
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportStokLampau.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 80)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(1100, 534)
        Me.ReportViewer1.TabIndex = 0
        '
        'PanelTotal
        '
        Me.PanelTotal.Controls.Add(Me.LblTotalItem)
        Me.PanelTotal.Controls.Add(Me.Label3)
        Me.PanelTotal.Controls.Add(Me.TxtNilaiToko)
        Me.PanelTotal.Controls.Add(Me.Label4)
        Me.PanelTotal.Controls.Add(Me.TxtNilaiGudang)
        Me.PanelTotal.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelTotal.Location = New System.Drawing.Point(0, 614)
        Me.PanelTotal.Name = "PanelTotal"
        Me.PanelTotal.Size = New System.Drawing.Size(1100, 36)
        Me.PanelTotal.TabIndex = 1
        '
        'LblTotalItem
        '
        Me.LblTotalItem.AutoSize = True
        Me.LblTotalItem.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblTotalItem.Location = New System.Drawing.Point(6, 10)
        Me.LblTotalItem.Name = "LblTotalItem"
        Me.LblTotalItem.Size = New System.Drawing.Size(42, 15)
        Me.LblTotalItem.TabIndex = 0
        Me.LblTotalItem.Text = "0 item"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.Location = New System.Drawing.Point(100, 10)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(96, 15)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Nilai Stok Toko :"
        '
        'TxtNilaiToko
        '
        Me.TxtNilaiToko.Location = New System.Drawing.Point(200, 7)
        Me.TxtNilaiToko.Name = "TxtNilaiToko"
        Me.TxtNilaiToko.ReadOnly = True
        Me.TxtNilaiToko.Size = New System.Drawing.Size(150, 21)
        Me.TxtNilaiToko.TabIndex = 2
        Me.TxtNilaiToko.Text = "0"
        Me.TxtNilaiToko.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.Location = New System.Drawing.Point(365, 10)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(112, 15)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Nilai Stok Gudang :"
        '
        'TxtNilaiGudang
        '
        Me.TxtNilaiGudang.Location = New System.Drawing.Point(475, 7)
        Me.TxtNilaiGudang.Name = "TxtNilaiGudang"
        Me.TxtNilaiGudang.ReadOnly = True
        Me.TxtNilaiGudang.Size = New System.Drawing.Size(150, 21)
        Me.TxtNilaiGudang.TabIndex = 4
        Me.TxtNilaiGudang.Text = "0"
        Me.TxtNilaiGudang.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FormLapStokLampau
        '
        Me.ClientSize = New System.Drawing.Size(1100, 650)
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.PanelTotal)
        Me.Controls.Add(Me.PanelFilter)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.Name = "FormLapStokLampau"
        Me.KeyPreview = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Stok Barang Masa Lampau"
        Me.PanelFilter.ResumeLayout(False)
        Me.PanelFilter.PerformLayout()
        Me.PanelTotal.ResumeLayout(False)
        Me.PanelTotal.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents PanelFilter As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DTPTanggal As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents CmbLokasi As System.Windows.Forms.ComboBox
    Friend WithEvents BtnTampil As System.Windows.Forms.Button
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelTotal As System.Windows.Forms.Panel
    Friend WithEvents LblTotalItem As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtNilaiToko As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtNilaiGudang As System.Windows.Forms.TextBox

End Class


