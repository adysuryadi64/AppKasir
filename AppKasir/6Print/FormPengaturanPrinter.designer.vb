<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormPengaturanPrinter
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormPengaturanPrinter))
        Me.PnlHeader = New System.Windows.Forms.Panel()
        Me.LblTitleHeader = New System.Windows.Forms.Label()
        Me.PanelGlobal = New System.Windows.Forms.Panel()
        Me.LblDefPrinter = New System.Windows.Forms.Label()
        Me.CmbPrinterDefault = New System.Windows.Forms.ComboBox()
        Me.LblPrinterAktif = New System.Windows.Forms.Label()
        Me.BtnSetDefault = New System.Windows.Forms.Button()
        Me.LblStatusKomp = New System.Windows.Forms.Label()
        Me.LblNamaKomputer = New System.Windows.Forms.Label()
        Me.CmbStatusKomputer = New System.Windows.Forms.ComboBox()
        Me.PnlBottom = New System.Windows.Forms.Panel()
        Me.LblKeteranganSimpan = New System.Windows.Forms.Label()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.TabTransaksi = New System.Windows.Forms.TabControl()
        Me.PnlHeader.SuspendLayout()
        Me.PanelGlobal.SuspendLayout()
        Me.PnlBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'PnlHeader
        '
        Me.PnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(160, Byte), Integer))
        Me.PnlHeader.Controls.Add(Me.LblTitleHeader)
        Me.PnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.PnlHeader.Name = "PnlHeader"
        Me.PnlHeader.Size = New System.Drawing.Size(1100, 38)
        Me.PnlHeader.TabIndex = 0
        '
        'LblTitleHeader
        '
        Me.LblTitleHeader.AutoSize = True
        Me.LblTitleHeader.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.LblTitleHeader.ForeColor = System.Drawing.Color.White
        Me.LblTitleHeader.Location = New System.Drawing.Point(10, 3)
        Me.LblTitleHeader.Name = "LblTitleHeader"
        Me.LblTitleHeader.Size = New System.Drawing.Size(353, 30)
        Me.LblTitleHeader.TabIndex = 0
        Me.LblTitleHeader.Text = "Pengaturan Printer per Transaksi"
        '
        'PanelGlobal
        '
        Me.PanelGlobal.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(230, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.PanelGlobal.Controls.Add(Me.LblDefPrinter)
        Me.PanelGlobal.Controls.Add(Me.CmbPrinterDefault)
        Me.PanelGlobal.Controls.Add(Me.LblPrinterAktif)
        Me.PanelGlobal.Controls.Add(Me.BtnSetDefault)
        Me.PanelGlobal.Controls.Add(Me.LblStatusKomp)
        Me.PanelGlobal.Controls.Add(Me.LblNamaKomputer)
        Me.PanelGlobal.Controls.Add(Me.CmbStatusKomputer)
        Me.PanelGlobal.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelGlobal.Location = New System.Drawing.Point(0, 38)
        Me.PanelGlobal.Name = "PanelGlobal"
        Me.PanelGlobal.Size = New System.Drawing.Size(1100, 56)
        Me.PanelGlobal.TabIndex = 1
        '
        'LblDefPrinter
        '
        Me.LblDefPrinter.AutoSize = True
        Me.LblDefPrinter.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblDefPrinter.Location = New System.Drawing.Point(12, 15)
        Me.LblDefPrinter.Name = "LblDefPrinter"
        Me.LblDefPrinter.Size = New System.Drawing.Size(151, 15)
        Me.LblDefPrinter.TabIndex = 0
        Me.LblDefPrinter.Text = "Printer Default Windows :"
        '
        'CmbPrinterDefault
        '
        Me.CmbPrinterDefault.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPrinterDefault.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.CmbPrinterDefault.Location = New System.Drawing.Point(166, 11)
        Me.CmbPrinterDefault.Name = "CmbPrinterDefault"
        Me.CmbPrinterDefault.Size = New System.Drawing.Size(280, 23)
        Me.CmbPrinterDefault.TabIndex = 0
        '
        'LblPrinterAktif
        '
        Me.LblPrinterAktif.AutoSize = True
        Me.LblPrinterAktif.Font = New System.Drawing.Font("Segoe UI", 8.0!, System.Drawing.FontStyle.Italic)
        Me.LblPrinterAktif.ForeColor = System.Drawing.Color.DarkGreen
        Me.LblPrinterAktif.Location = New System.Drawing.Point(190, 40)
        Me.LblPrinterAktif.Name = "LblPrinterAktif"
        Me.LblPrinterAktif.Size = New System.Drawing.Size(0, 13)
        Me.LblPrinterAktif.TabIndex = 1
        '
        'BtnSetDefault
        '
        Me.BtnSetDefault.AutoSize = True
        Me.BtnSetDefault.BackColor = System.Drawing.Color.White
        Me.BtnSetDefault.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSetDefault.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnSetDefault.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSetDefault.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSetDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSetDefault.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSetDefault.ForeColor = System.Drawing.Color.Black
        Me.BtnSetDefault.Image = CType(resources.GetObject("BtnSetDefault.Image"), System.Drawing.Image)
        Me.BtnSetDefault.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetDefault.Location = New System.Drawing.Point(480, 8)
        Me.BtnSetDefault.Name = "BtnSetDefault"
        Me.BtnSetDefault.Size = New System.Drawing.Size(170, 29)
        Me.BtnSetDefault.TabIndex = 1
        Me.BtnSetDefault.Text = "Set Default Windows"
        Me.BtnSetDefault.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetDefault.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSetDefault.UseVisualStyleBackColor = False
        '
        'LblStatusKomp
        '
        Me.LblStatusKomp.AutoSize = True
        Me.LblStatusKomp.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblStatusKomp.Location = New System.Drawing.Point(681, 15)
        Me.LblStatusKomp.Name = "LblStatusKomp"
        Me.LblStatusKomp.Size = New System.Drawing.Size(184, 15)
        Me.LblStatusKomp.TabIndex = 2
        Me.LblStatusKomp.Text = "Komputer ini berperan sebagai :"
        '
        'LblNamaKomputer
        '
        Me.LblNamaKomputer.AutoSize = True
        Me.LblNamaKomputer.Font = New System.Drawing.Font("Segoe UI", 8.0!, System.Drawing.FontStyle.Italic)
        Me.LblNamaKomputer.ForeColor = System.Drawing.Color.DimGray
        Me.LblNamaKomputer.Location = New System.Drawing.Point(660, 38)
        Me.LblNamaKomputer.Name = "LblNamaKomputer"
        Me.LblNamaKomputer.Size = New System.Drawing.Size(0, 13)
        Me.LblNamaKomputer.TabIndex = 3
        '
        'CmbStatusKomputer
        '
        Me.CmbStatusKomputer.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.CmbStatusKomputer.Items.AddRange(New Object() {"Server", "Admin1", "Admin2", "Admin3", "Kasir1", "Kasir2", "Kasir3"})
        Me.CmbStatusKomputer.Location = New System.Drawing.Point(876, 11)
        Me.CmbStatusKomputer.Name = "CmbStatusKomputer"
        Me.CmbStatusKomputer.Size = New System.Drawing.Size(160, 23)
        Me.CmbStatusKomputer.TabIndex = 2
        '
        'PnlBottom
        '
        Me.PnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(230, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.PnlBottom.Controls.Add(Me.LblKeteranganSimpan)
        Me.PnlBottom.Controls.Add(Me.BtnSimpan)
        Me.PnlBottom.Controls.Add(Me.BtnKeluar)
        Me.PnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PnlBottom.Location = New System.Drawing.Point(0, 628)
        Me.PnlBottom.Name = "PnlBottom"
        Me.PnlBottom.Size = New System.Drawing.Size(1100, 52)
        Me.PnlBottom.TabIndex = 2
        '
        'LblKeteranganSimpan
        '
        Me.LblKeteranganSimpan.Font = New System.Drawing.Font("Segoe UI", 8.5!)
        Me.LblKeteranganSimpan.ForeColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(20, Byte), Integer))
        Me.LblKeteranganSimpan.Location = New System.Drawing.Point(12, 6)
        Me.LblKeteranganSimpan.Name = "LblKeteranganSimpan"
        Me.LblKeteranganSimpan.Size = New System.Drawing.Size(525, 37)
        Me.LblKeteranganSimpan.TabIndex = 10
        Me.LblKeteranganSimpan.Text = "✅  Sudah selesai mengatur? Klik Simpan agar printer yang dipilih langsung dipakai" &
    " saat cetak."
        Me.LblKeteranganSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnSimpan
        '
        Me.BtnSimpan.AutoSize = True
        Me.BtnSimpan.BackColor = System.Drawing.Color.White
        Me.BtnSimpan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpan.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.Location = New System.Drawing.Point(558, 7)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(114, 33)
        Me.BtnSimpan.TabIndex = 0
        Me.BtnSimpan.Text = "Simpan (F8)"
        Me.BtnSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'BtnKeluar
        '
        Me.BtnKeluar.AutoSize = True
        Me.BtnKeluar.BackColor = System.Drawing.Color.White
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKeluar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.Location = New System.Drawing.Point(738, 7)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(112, 33)
        Me.BtnKeluar.TabIndex = 1
        Me.BtnKeluar.Text = "Keluar (Esc)"
        Me.BtnKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'TabTransaksi
        '
        Me.TabTransaksi.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabTransaksi.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed
        Me.TabTransaksi.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.TabTransaksi.ItemSize = New System.Drawing.Size(130, 34)
        Me.TabTransaksi.Location = New System.Drawing.Point(0, 94)
        Me.TabTransaksi.Name = "TabTransaksi"
        Me.TabTransaksi.SelectedIndex = 0
        Me.TabTransaksi.Size = New System.Drawing.Size(1100, 534)
        Me.TabTransaksi.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabTransaksi.TabIndex = 3
        '
        'FormPengaturanPrinter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(244, Byte), Integer), CType(CType(248, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1100, 680)
        Me.Controls.Add(Me.TabTransaksi)
        Me.Controls.Add(Me.PnlBottom)
        Me.Controls.Add(Me.PanelGlobal)
        Me.Controls.Add(Me.PnlHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormPengaturanPrinter"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Pengaturan Printer"
        Me.PnlHeader.ResumeLayout(False)
        Me.PnlHeader.PerformLayout()
        Me.PanelGlobal.ResumeLayout(False)
        Me.PanelGlobal.PerformLayout()
        Me.PnlBottom.ResumeLayout(False)
        Me.PnlBottom.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PnlHeader As System.Windows.Forms.Panel
    Friend WithEvents LblTitleHeader As System.Windows.Forms.Label
    Friend WithEvents PanelGlobal As System.Windows.Forms.Panel
    Friend WithEvents LblDefPrinter As System.Windows.Forms.Label
    Friend WithEvents LblPrinterAktif As System.Windows.Forms.Label
    Friend WithEvents CmbPrinterDefault As System.Windows.Forms.ComboBox
    Friend WithEvents BtnSetDefault As System.Windows.Forms.Button
    Friend WithEvents LblStatusKomp As System.Windows.Forms.Label
    Friend WithEvents LblNamaKomputer As System.Windows.Forms.Label
    Friend WithEvents CmbStatusKomputer As System.Windows.Forms.ComboBox
    Friend WithEvents PnlBottom As System.Windows.Forms.Panel
    Friend WithEvents LblKeteranganSimpan As System.Windows.Forms.Label
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents BtnKeluar As System.Windows.Forms.Button
    Friend WithEvents TabTransaksi As System.Windows.Forms.TabControl

End Class