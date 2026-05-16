<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormSync
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormSync))
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblKodeToko = New System.Windows.Forms.Label()
        Me.LblLastSync = New System.Windows.Forms.Label()
        Me.LblQueue = New System.Windows.Forms.Label()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.BtnSync = New System.Windows.Forms.Button()
        Me.BtnUpload = New System.Windows.Forms.Button()
        Me.BtnDownload = New System.Windows.Forms.Button()
        Me.BtnCekKoneksi = New System.Windows.Forms.Button()
        Me.BtnRefreshQueue = New System.Windows.Forms.Button()
        Me.BtnLihatLog = New System.Windows.Forms.Button()
        Me.TxtLog = New System.Windows.Forms.RichTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DgvLog = New System.Windows.Forms.DataGridView()
        CType(Me.DgvLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Black
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Century Gothic", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.ForeColor = System.Drawing.Color.White
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(800, 36)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "SINKRONISASI DATA"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.White
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.ForeColor = System.Drawing.Color.White
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(762, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 28)
        Me.BtnClose.TabIndex = 1
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblKodeToko
        '
        Me.LblKodeToko.AutoSize = True
        Me.LblKodeToko.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblKodeToko.Location = New System.Drawing.Point(12, 48)
        Me.LblKodeToko.Name = "LblKodeToko"
        Me.LblKodeToko.Size = New System.Drawing.Size(80, 16)
        Me.LblKodeToko.TabIndex = 2
        Me.LblKodeToko.Text = "Cabang: -"
        '
        'LblLastSync
        '
        Me.LblLastSync.AutoSize = True
        Me.LblLastSync.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLastSync.Location = New System.Drawing.Point(12, 70)
        Me.LblLastSync.Name = "LblLastSync"
        Me.LblLastSync.Size = New System.Drawing.Size(100, 16)
        Me.LblLastSync.TabIndex = 3
        Me.LblLastSync.Text = "Last sync: -"
        '
        'LblQueue
        '
        Me.LblQueue.AutoSize = True
        Me.LblQueue.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblQueue.Location = New System.Drawing.Point(12, 92)
        Me.LblQueue.Name = "LblQueue"
        Me.LblQueue.Size = New System.Drawing.Size(110, 16)
        Me.LblQueue.TabIndex = 4
        Me.LblQueue.Text = "Queue pending: 0"
        '
        'LblStatus
        '
        Me.LblStatus.AutoSize = True
        Me.LblStatus.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatus.ForeColor = System.Drawing.Color.Green
        Me.LblStatus.Location = New System.Drawing.Point(12, 114)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(80, 16)
        Me.LblStatus.TabIndex = 5
        Me.LblStatus.Text = "Status: -"
        '
        'BtnSync
        '
        Me.BtnSync.AutoSize = True
        Me.BtnSync.BackColor = System.Drawing.Color.White
        Me.BtnSync.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSync.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSync.FlatAppearance.BorderSize = 1
        Me.BtnSync.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSync.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSync.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSync.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSync.Image = CType(resources.GetObject("BtnSync.Image"), System.Drawing.Image)
        Me.BtnSync.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSync.Location = New System.Drawing.Point(600, 48)
        Me.BtnSync.Name = "BtnSync"
        Me.BtnSync.Size = New System.Drawing.Size(150, 34)
        Me.BtnSync.TabIndex = 6
        Me.BtnSync.Text = "Sync Semua"
        Me.BtnSync.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSync.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSync.UseVisualStyleBackColor = False
        '
        'BtnUpload
        '
        Me.BtnUpload.AutoSize = True
        Me.BtnUpload.BackColor = System.Drawing.Color.White
        Me.BtnUpload.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnUpload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnUpload.FlatAppearance.BorderSize = 1
        Me.BtnUpload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnUpload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnUpload.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnUpload.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnUpload.Image = CType(resources.GetObject("BtnUpload.Image"), System.Drawing.Image)
        Me.BtnUpload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnUpload.Location = New System.Drawing.Point(600, 90)
        Me.BtnUpload.Name = "BtnUpload"
        Me.BtnUpload.Size = New System.Drawing.Size(150, 34)
        Me.BtnUpload.TabIndex = 7
        Me.BtnUpload.Text = "Upload"
        Me.BtnUpload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnUpload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnUpload.UseVisualStyleBackColor = False
        '
        'BtnDownload
        '
        Me.BtnDownload.AutoSize = True
        Me.BtnDownload.BackColor = System.Drawing.Color.White
        Me.BtnDownload.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnDownload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnDownload.FlatAppearance.BorderSize = 1
        Me.BtnDownload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnDownload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnDownload.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnDownload.ForeColor = System.Drawing.Color.Black
        Me.BtnDownload.Image = CType(resources.GetObject("BtnDownload.Image"), System.Drawing.Image)
        Me.BtnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDownload.Location = New System.Drawing.Point(600, 132)
        Me.BtnDownload.Name = "BtnDownload"
        Me.BtnDownload.Size = New System.Drawing.Size(150, 34)
        Me.BtnDownload.TabIndex = 8
        Me.BtnDownload.Text = "Download"
        Me.BtnDownload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnDownload.UseVisualStyleBackColor = False
        '
        'BtnCekKoneksi
        '
        Me.BtnCekKoneksi.AutoSize = True
        Me.BtnCekKoneksi.BackColor = System.Drawing.Color.White
        Me.BtnCekKoneksi.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCekKoneksi.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCekKoneksi.FlatAppearance.BorderSize = 1
        Me.BtnCekKoneksi.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCekKoneksi.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCekKoneksi.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCekKoneksi.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCekKoneksi.ForeColor = System.Drawing.Color.Black
        Me.BtnCekKoneksi.Image = CType(resources.GetObject("BtnCekKoneksi.Image"), System.Drawing.Image)
        Me.BtnCekKoneksi.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCekKoneksi.Location = New System.Drawing.Point(440, 48)
        Me.BtnCekKoneksi.Name = "BtnCekKoneksi"
        Me.BtnCekKoneksi.Size = New System.Drawing.Size(140, 34)
        Me.BtnCekKoneksi.TabIndex = 9
        Me.BtnCekKoneksi.Text = "Cek Koneksi"
        Me.BtnCekKoneksi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCekKoneksi.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCekKoneksi.UseVisualStyleBackColor = False
        '
        'BtnRefreshQueue
        '
        Me.BtnRefreshQueue.AutoSize = True
        Me.BtnRefreshQueue.BackColor = System.Drawing.Color.White
        Me.BtnRefreshQueue.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnRefreshQueue.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnRefreshQueue.FlatAppearance.BorderSize = 1
        Me.BtnRefreshQueue.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnRefreshQueue.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnRefreshQueue.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnRefreshQueue.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnRefreshQueue.ForeColor = System.Drawing.Color.Black
        Me.BtnRefreshQueue.Image = CType(resources.GetObject("BtnRefreshQueue.Image"), System.Drawing.Image)
        Me.BtnRefreshQueue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRefreshQueue.Location = New System.Drawing.Point(440, 90)
        Me.BtnRefreshQueue.Name = "BtnRefreshQueue"
        Me.BtnRefreshQueue.Size = New System.Drawing.Size(140, 34)
        Me.BtnRefreshQueue.TabIndex = 10
        Me.BtnRefreshQueue.Text = "Refresh Status"
        Me.BtnRefreshQueue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnRefreshQueue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnRefreshQueue.UseVisualStyleBackColor = False
        '
        'BtnLihatLog
        '
        Me.BtnLihatLog.AutoSize = True
        Me.BtnLihatLog.BackColor = System.Drawing.Color.White
        Me.BtnLihatLog.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnLihatLog.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnLihatLog.FlatAppearance.BorderSize = 1
        Me.BtnLihatLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnLihatLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnLihatLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnLihatLog.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnLihatLog.ForeColor = System.Drawing.Color.Black
        Me.BtnLihatLog.Image = CType(resources.GetObject("BtnLihatLog.Image"), System.Drawing.Image)
        Me.BtnLihatLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnLihatLog.Location = New System.Drawing.Point(440, 132)
        Me.BtnLihatLog.Name = "BtnLihatLog"
        Me.BtnLihatLog.Size = New System.Drawing.Size(140, 34)
        Me.BtnLihatLog.TabIndex = 11
        Me.BtnLihatLog.Text = "Lihat Log"
        Me.BtnLihatLog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnLihatLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnLihatLog.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 180)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 16)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Log Sync"
        '
        'TxtLog
        '
        Me.TxtLog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtLog.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(23, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.TxtLog.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLog.ForeColor = System.Drawing.Color.LightGreen
        Me.TxtLog.Location = New System.Drawing.Point(12, 199)
        Me.TxtLog.Name = "TxtLog"
        Me.TxtLog.ReadOnly = True
        Me.TxtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.TxtLog.Size = New System.Drawing.Size(776, 200)
        Me.TxtLog.TabIndex = 13
        Me.TxtLog.Text = ""
        '
        'DgvLog
        '
        Me.DgvLog.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvLog.Location = New System.Drawing.Point(12, 410)
        Me.DgvLog.Name = "DgvLog"
        Me.DgvLog.ReadOnly = True
        Me.DgvLog.RowHeadersVisible = False
        Me.DgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvLog.Size = New System.Drawing.Size(776, 180)
        Me.DgvLog.TabIndex = 14
        '
        'FormSync
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(800, 600)
        Me.Controls.Add(Me.DgvLog)
        Me.Controls.Add(Me.TxtLog)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnLihatLog)
        Me.Controls.Add(Me.BtnRefreshQueue)
        Me.Controls.Add(Me.BtnCekKoneksi)
        Me.Controls.Add(Me.BtnDownload)
        Me.Controls.Add(Me.BtnUpload)
        Me.Controls.Add(Me.BtnSync)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.LblQueue)
        Me.Controls.Add(Me.LblLastSync)
        Me.Controls.Add(Me.LblKodeToko)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "FormSync"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormSync"
        CType(Me.DgvLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblKodeToko As System.Windows.Forms.Label
    Friend WithEvents LblLastSync As System.Windows.Forms.Label
    Friend WithEvents LblQueue As System.Windows.Forms.Label
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents BtnSync As System.Windows.Forms.Button
    Friend WithEvents BtnUpload As System.Windows.Forms.Button
    Friend WithEvents BtnDownload As System.Windows.Forms.Button
    Friend WithEvents BtnCekKoneksi As System.Windows.Forms.Button
    Friend WithEvents BtnRefreshQueue As System.Windows.Forms.Button
    Friend WithEvents BtnLihatLog As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtLog As System.Windows.Forms.RichTextBox
    Friend WithEvents DgvLog As System.Windows.Forms.DataGridView
End Class
