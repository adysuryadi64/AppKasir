<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPenjualanDitahan
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormPenjualanDitahan))
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.DgvDetail = New System.Windows.Forms.DataGridView()
        Me.PanelKonten = New System.Windows.Forms.Panel()
        Me.PanelBottom = New System.Windows.Forms.Panel()
        Me.BtnTutup = New System.Windows.Forms.Button()
        Me.BtnHapus = New System.Windows.Forms.Button()
        Me.BtnProses = New System.Windows.Forms.Button()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelDetailHeader = New System.Windows.Forms.Panel()
        Me.LblDetailHeader = New System.Windows.Forms.Label()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBottom.SuspendLayout()
        Me.PanelKonten.SuspendLayout()
        Me.PanelHeader.SuspendLayout()
        Me.PanelDetailHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelKonten
        '
        Me.PanelKonten.Controls.Add(Me.DgvData)
        Me.PanelKonten.Controls.Add(Me.PanelDetailHeader)
        Me.PanelKonten.Controls.Add(Me.DgvDetail)
        Me.PanelKonten.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelKonten.Location = New System.Drawing.Point(0, 36)
        Me.PanelKonten.Name = "PanelKonten"
        Me.PanelKonten.Size = New System.Drawing.Size(1024, 517)
        Me.PanelKonten.TabIndex = 20
        '
        'DgvData
        '
        Me.DgvData.AllowUserToAddRows = False
        Me.DgvData.AllowUserToDeleteRows = False
        Me.DgvData.AllowUserToOrderColumns = False
        Me.DgvData.AllowUserToResizeColumns = False
        Me.DgvData.AllowUserToResizeRows = False
        Me.DgvData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Location = New System.Drawing.Point(0, 36)
        Me.DgvData.MultiSelect = False
        Me.DgvData.Name = "DgvData"
        Me.DgvData.ReadOnly = True
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvData.Size = New System.Drawing.Size(1024, 340)
        Me.DgvData.TabIndex = 0
        '
        'DgvDetail
        '
        Me.DgvDetail.AllowUserToAddRows = False
        Me.DgvDetail.AllowUserToDeleteRows = False
        Me.DgvDetail.AllowUserToResizeColumns = False
        Me.DgvDetail.AllowUserToResizeRows = False
        Me.DgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvDetail.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.DgvDetail.Location = New System.Drawing.Point(0, 441)
        Me.DgvDetail.MultiSelect = False
        Me.DgvDetail.Name = "DgvDetail"
        Me.DgvDetail.ReadOnly = True
        Me.DgvDetail.RowHeadersVisible = False
        Me.DgvDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvDetail.Size = New System.Drawing.Size(1024, 156)
        Me.DgvDetail.TabIndex = 1
        '
        'PanelBottom
        '
        Me.PanelBottom.Controls.Add(Me.BtnTutup)
        Me.PanelBottom.Controls.Add(Me.BtnHapus)
        Me.PanelBottom.Controls.Add(Me.BtnProses)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.Location = New System.Drawing.Point(0, 373)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Size = New System.Drawing.Size(1024, 44)
        Me.PanelBottom.TabIndex = 2
        '
        'BtnTutup
        '
        Me.BtnTutup.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnTutup.AutoSize = True
        Me.BtnTutup.BackColor = System.Drawing.Color.White
        Me.BtnTutup.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTutup.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnTutup.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnTutup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnTutup.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTutup.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTutup.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnTutup.Image = CType(resources.GetObject("BtnTutup.Image"), System.Drawing.Image)
        Me.BtnTutup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTutup.Location = New System.Drawing.Point(907, 9)
        Me.BtnTutup.Name = "BtnTutup"
        Me.BtnTutup.Size = New System.Drawing.Size(105, 29)
        Me.BtnTutup.TabIndex = 2
        Me.BtnTutup.Text = "Tutup (Esc)"
        Me.BtnTutup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTutup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTutup.UseVisualStyleBackColor = False
        '
        'BtnHapus
        '
        Me.BtnHapus.AutoSize = True
        Me.BtnHapus.BackColor = System.Drawing.Color.White
        Me.BtnHapus.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnHapus.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnHapus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnHapus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnHapus.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHapus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHapus.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnHapus.Image = CType(resources.GetObject("BtnHapus.Image"), System.Drawing.Image)
        Me.BtnHapus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHapus.Location = New System.Drawing.Point(123, 9)
        Me.BtnHapus.Name = "BtnHapus"
        Me.BtnHapus.Size = New System.Drawing.Size(114, 29)
        Me.BtnHapus.TabIndex = 1
        Me.BtnHapus.Text = "Hapus (Del)"
        Me.BtnHapus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHapus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnHapus.UseVisualStyleBackColor = False
        '
        'BtnProses
        '
        Me.BtnProses.AutoSize = True
        Me.BtnProses.BackColor = System.Drawing.Color.White
        Me.BtnProses.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnProses.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnProses.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnProses.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnProses.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnProses.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnProses.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnProses.Image = CType(resources.GetObject("BtnProses.Image"), System.Drawing.Image)
        Me.BtnProses.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnProses.Location = New System.Drawing.Point(12, 9)
        Me.BtnProses.Name = "BtnProses"
        Me.BtnProses.Size = New System.Drawing.Size(96, 29)
        Me.BtnProses.TabIndex = 0
        Me.BtnProses.Text = "Pilih (F9)"
        Me.BtnProses.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnProses.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnProses.UseVisualStyleBackColor = False
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.PanelHeader.Controls.Add(Me.LblHeaderForm)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1024, 36)
        Me.PanelHeader.TabIndex = 10
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeaderForm.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.ForeColor = System.Drawing.Color.White
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1024, 36)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "DAFTAR PENJUALAN DITAHAN"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelDetailHeader
        '
        Me.PanelDetailHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.PanelDetailHeader.Controls.Add(Me.LblDetailHeader)
        Me.PanelDetailHeader.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelDetailHeader.Location = New System.Drawing.Point(0, 417)
        Me.PanelDetailHeader.Name = "PanelDetailHeader"
        Me.PanelDetailHeader.Size = New System.Drawing.Size(1024, 24)
        Me.PanelDetailHeader.TabIndex = 5
        '
        'LblDetailHeader
        '
        Me.LblDetailHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblDetailHeader.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDetailHeader.ForeColor = System.Drawing.Color.White
        Me.LblDetailHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblDetailHeader.Name = "LblDetailHeader"
        Me.LblDetailHeader.Padding = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.LblDetailHeader.Size = New System.Drawing.Size(1024, 24)
        Me.LblDetailHeader.TabIndex = 0
        Me.LblDetailHeader.Text = "▼  Detail Barang"
        Me.LblDetailHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FormPenjualanDitahan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1024, 597)
        Me.Controls.Add(Me.PanelKonten)
        Me.Controls.Add(Me.PanelBottom)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormPenjualanDitahan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Daftar Penjualan Ditahan"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DgvDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBottom.ResumeLayout(False)
        Me.PanelBottom.PerformLayout()
        Me.PanelKonten.ResumeLayout(False)
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelDetailHeader.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DgvData As DataGridView
    Friend WithEvents DgvDetail As DataGridView
    Friend WithEvents PanelKonten As Panel
    Friend WithEvents PanelBottom As Panel
    Friend WithEvents BtnTutup As Button
    Friend WithEvents BtnHapus As Button
    Friend WithEvents BtnProses As Button
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents PanelDetailHeader As System.Windows.Forms.Panel
    Friend WithEvents LblDetailHeader As System.Windows.Forms.Label
    Friend WithEvents ColFaktur As DataGridViewTextBoxColumn
    Friend WithEvents ColTanggal As DataGridViewTextBoxColumn
    Friend WithEvents ColPelanggan As DataGridViewTextBoxColumn
    Friend WithEvents ColTotalItem As DataGridViewTextBoxColumn
    Friend WithEvents ColTotalQty As DataGridViewTextBoxColumn
    Friend WithEvents ColGrandTotal As DataGridViewTextBoxColumn
    Friend WithEvents ColUser As DataGridViewTextBoxColumn
End Class


