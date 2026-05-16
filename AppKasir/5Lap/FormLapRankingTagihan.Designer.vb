<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapRankingTagihan
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapRankingTagihan))
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.LblLokasi = New System.Windows.Forms.Label()
        Me.CmbLokasi = New System.Windows.Forms.ComboBox()
        Me.LblJumlah = New System.Windows.Forms.Label()
        Me.CmbJumlah = New System.Windows.Forms.ComboBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.LblLabelTotalItem = New System.Windows.Forms.Label()
        Me.LblTotalItem = New System.Windows.Forms.Label()
        Me.LblLabelTotalNilai = New System.Windows.Forms.Label()
        Me.LblTotalNilai = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.PanelKiri.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.SystemColors.Control
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1168, 31)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "RANKING PIUTANG PELANGGAN"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer2)
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Location = New System.Drawing.Point(376, 34)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(792, 434)
        Me.Panel1.TabIndex = 1
        '
        'ReportViewer2
        '
        Me.ReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportRankingHutang.rdlc"
        Me.ReportViewer2.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer2.Name = "ReportViewer2"
        Me.ReportViewer2.ServerReport.BearerToken = Nothing
        Me.ReportViewer2.Size = New System.Drawing.Size(792, 434)
        Me.ReportViewer2.TabIndex = 1
        Me.ReportViewer2.Visible = False
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportRankingPiutang.rdlc"
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
        Me.PanelKiri.Controls.Add(Me.LblLokasi)
        Me.PanelKiri.Controls.Add(Me.CmbLokasi)
        Me.PanelKiri.Controls.Add(Me.LblJumlah)
        Me.PanelKiri.Controls.Add(Me.CmbJumlah)
        Me.PanelKiri.Controls.Add(Me.BtnTampil)
        Me.PanelKiri.Controls.Add(Me.LblLabelTotalItem)
        Me.PanelKiri.Controls.Add(Me.LblTotalItem)
        Me.PanelKiri.Controls.Add(Me.LblLabelTotalNilai)
        Me.PanelKiri.Controls.Add(Me.LblTotalNilai)
        Me.PanelKiri.Location = New System.Drawing.Point(0, 34)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Size = New System.Drawing.Size(370, 434)
        Me.PanelKiri.TabIndex = 2
        '
        'LblLokasi
        '
        Me.LblLokasi.AutoSize = True
        Me.LblLokasi.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblLokasi.Location = New System.Drawing.Point(12, 12)
        Me.LblLokasi.Name = "LblLokasi"
        Me.LblLokasi.Size = New System.Drawing.Size(50, 17)
        Me.LblLokasi.TabIndex = 0
        Me.LblLokasi.Text = "Lokasi :"
        '
        'CmbLokasi
        '
        Me.CmbLokasi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbLokasi.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbLokasi.FormattingEnabled = True
        Me.CmbLokasi.Items.AddRange(New Object() {"SEMUA", "TOKO", "GUDANG"})
        Me.CmbLokasi.Location = New System.Drawing.Point(12, 32)
        Me.CmbLokasi.Name = "CmbLokasi"
        Me.CmbLokasi.Size = New System.Drawing.Size(120, 25)
        Me.CmbLokasi.TabIndex = 1
        '
        'LblJumlah
        '
        Me.LblJumlah.AutoSize = True
        Me.LblJumlah.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblJumlah.Location = New System.Drawing.Point(148, 12)
        Me.LblJumlah.Name = "LblJumlah"
        Me.LblJumlah.Size = New System.Drawing.Size(73, 17)
        Me.LblJumlah.TabIndex = 2
        Me.LblJumlah.Text = "Tampilkan :"
        '
        'CmbJumlah
        '
        Me.CmbJumlah.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJumlah.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.CmbJumlah.FormattingEnabled = True
        Me.CmbJumlah.Location = New System.Drawing.Point(148, 32)
        Me.CmbJumlah.Name = "CmbJumlah"
        Me.CmbJumlah.Size = New System.Drawing.Size(90, 25)
        Me.CmbJumlah.TabIndex = 3
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
        Me.BtnTampil.Location = New System.Drawing.Point(12, 74)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(233, 33)
        Me.BtnTampil.TabIndex = 4
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'LblLabelTotalItem
        '
        Me.LblLabelTotalItem.AutoSize = True
        Me.LblLabelTotalItem.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblLabelTotalItem.Location = New System.Drawing.Point(12, 134)
        Me.LblLabelTotalItem.Name = "LblLabelTotalItem"
        Me.LblLabelTotalItem.Size = New System.Drawing.Size(110, 17)
        Me.LblLabelTotalItem.TabIndex = 5
        Me.LblLabelTotalItem.Text = "Total Pelanggan :"
        '
        'LblTotalItem
        '
        Me.LblTotalItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalItem.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblTotalItem.Location = New System.Drawing.Point(150, 132)
        Me.LblTotalItem.Name = "LblTotalItem"
        Me.LblTotalItem.Size = New System.Drawing.Size(110, 20)
        Me.LblTotalItem.TabIndex = 6
        Me.LblTotalItem.Text = "0"
        Me.LblTotalItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblLabelTotalNilai
        '
        Me.LblLabelTotalNilai.AutoSize = True
        Me.LblLabelTotalNilai.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblLabelTotalNilai.Location = New System.Drawing.Point(12, 160)
        Me.LblLabelTotalNilai.Name = "LblLabelTotalNilai"
        Me.LblLabelTotalNilai.Size = New System.Drawing.Size(91, 17)
        Me.LblLabelTotalNilai.TabIndex = 7
        Me.LblLabelTotalNilai.Text = "Total Piutang :"
        '
        'LblTotalNilai
        '
        Me.LblTotalNilai.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalNilai.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.LblTotalNilai.Location = New System.Drawing.Point(150, 158)
        Me.LblTotalNilai.Name = "LblTotalNilai"
        Me.LblTotalNilai.Size = New System.Drawing.Size(200, 20)
        Me.LblTotalNilai.TabIndex = 8
        Me.LblTotalNilai.Text = "0"
        Me.LblTotalNilai.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FormLapRankingTagihan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1168, 468)
        Me.Controls.Add(Me.PanelKiri)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapRankingTagihan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Ranking Piutang"
        Me.Panel1.ResumeLayout(False)
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm           As System.Windows.Forms.Label
    Friend WithEvents Panel1             As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1      As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents ReportViewer2      As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelKiri          As System.Windows.Forms.Panel
    Friend WithEvents LblLokasi          As System.Windows.Forms.Label
    Friend WithEvents CmbLokasi          As System.Windows.Forms.ComboBox
    Friend WithEvents LblJumlah          As System.Windows.Forms.Label
    Friend WithEvents CmbJumlah          As System.Windows.Forms.ComboBox
    Friend WithEvents BtnTampil          As System.Windows.Forms.Button
    Friend WithEvents LblLabelTotalItem  As System.Windows.Forms.Label
    Friend WithEvents LblTotalItem       As System.Windows.Forms.Label
    Friend WithEvents LblLabelTotalNilai As System.Windows.Forms.Label
    Friend WithEvents LblTotalNilai      As System.Windows.Forms.Label
End Class
