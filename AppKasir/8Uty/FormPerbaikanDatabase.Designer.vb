<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPerbaikanDatabase
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormPerbaikanDatabase))
        Me.BtnAnalyze = New System.Windows.Forms.Button()
        Me.ListBoxResults = New System.Windows.Forms.ListBox()
        Me.BtnCleanup = New System.Windows.Forms.Button()
        Me.BtnCheckTables = New System.Windows.Forms.Button()
        Me.BtnChecksumTables = New System.Windows.Forms.Button()
        Me.BtnConvertUtf8 = New System.Windows.Forms.Button()
        Me.BtnDuplikat = New System.Windows.Forms.Button()
        Me.BtnCetak = New System.Windows.Forms.Button()
        Me.PrintPreviewDialog1 = New System.Windows.Forms.PrintPreviewDialog()
        Me.BtnSimpanPDF = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'BtnAnalyze
        '
        Me.BtnAnalyze.AutoSize = True
        Me.BtnAnalyze.BackColor = System.Drawing.Color.White
        Me.BtnAnalyze.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnAnalyze.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnAnalyze.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnAnalyze.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnAnalyze.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnAnalyze.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAnalyze.ForeColor = System.Drawing.Color.Black
        Me.BtnAnalyze.Image = CType(resources.GetObject("BtnAnalyze.Image"), System.Drawing.Image)
        Me.BtnAnalyze.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnAnalyze.Location = New System.Drawing.Point(298, 10)
        Me.BtnAnalyze.Name = "BtnAnalyze"
        Me.BtnAnalyze.Size = New System.Drawing.Size(88, 29)
        Me.BtnAnalyze.TabIndex = 0
        Me.BtnAnalyze.Text = "Analyze"
        Me.BtnAnalyze.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnAnalyze.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnAnalyze.UseVisualStyleBackColor = False
        '
        'ListBoxResults
        '
        Me.ListBoxResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListBoxResults.FormattingEnabled = True
        Me.ListBoxResults.Location = New System.Drawing.Point(13, 45)
        Me.ListBoxResults.Name = "ListBoxResults"
        Me.ListBoxResults.Size = New System.Drawing.Size(1096, 524)
        Me.ListBoxResults.TabIndex = 1
        '
        'BtnCleanup
        '
        Me.BtnCleanup.AutoSize = True
        Me.BtnCleanup.BackColor = System.Drawing.Color.White
        Me.BtnCleanup.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCleanup.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCleanup.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCleanup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCleanup.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCleanup.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCleanup.ForeColor = System.Drawing.Color.Black
        Me.BtnCleanup.Image = CType(resources.GetObject("BtnCleanup.Image"), System.Drawing.Image)
        Me.BtnCleanup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCleanup.Location = New System.Drawing.Point(12, 10)
        Me.BtnCleanup.Name = "BtnCleanup"
        Me.BtnCleanup.Size = New System.Drawing.Size(90, 29)
        Me.BtnCleanup.TabIndex = 2
        Me.BtnCleanup.Text = "Cleanup"
        Me.BtnCleanup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCleanup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCleanup.UseVisualStyleBackColor = False
        '
        'BtnCheckTables
        '
        Me.BtnCheckTables.AutoSize = True
        Me.BtnCheckTables.BackColor = System.Drawing.Color.White
        Me.BtnCheckTables.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCheckTables.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCheckTables.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCheckTables.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCheckTables.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCheckTables.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCheckTables.ForeColor = System.Drawing.Color.Black
        Me.BtnCheckTables.Image = CType(resources.GetObject("BtnCheckTables.Image"), System.Drawing.Image)
        Me.BtnCheckTables.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCheckTables.Location = New System.Drawing.Point(392, 10)
        Me.BtnCheckTables.Name = "BtnCheckTables"
        Me.BtnCheckTables.Size = New System.Drawing.Size(119, 29)
        Me.BtnCheckTables.TabIndex = 3
        Me.BtnCheckTables.Text = "Check Tables"
        Me.BtnCheckTables.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCheckTables.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCheckTables.UseVisualStyleBackColor = False
        '
        'BtnChecksumTables
        '
        Me.BtnChecksumTables.AutoSize = True
        Me.BtnChecksumTables.BackColor = System.Drawing.Color.White
        Me.BtnChecksumTables.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnChecksumTables.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnChecksumTables.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnChecksumTables.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnChecksumTables.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnChecksumTables.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnChecksumTables.ForeColor = System.Drawing.Color.Black
        Me.BtnChecksumTables.Image = CType(resources.GetObject("BtnChecksumTables.Image"), System.Drawing.Image)
        Me.BtnChecksumTables.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnChecksumTables.Location = New System.Drawing.Point(517, 10)
        Me.BtnChecksumTables.Name = "BtnChecksumTables"
        Me.BtnChecksumTables.Size = New System.Drawing.Size(145, 29)
        Me.BtnChecksumTables.TabIndex = 4
        Me.BtnChecksumTables.Text = "Checksum Tables"
        Me.BtnChecksumTables.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnChecksumTables.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnChecksumTables.UseVisualStyleBackColor = False
        '
        'BtnConvertUtf8
        '
        Me.BtnConvertUtf8.AutoSize = True
        Me.BtnConvertUtf8.BackColor = System.Drawing.Color.White
        Me.BtnConvertUtf8.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnConvertUtf8.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnConvertUtf8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnConvertUtf8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnConvertUtf8.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnConvertUtf8.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnConvertUtf8.ForeColor = System.Drawing.Color.Black
        Me.BtnConvertUtf8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnConvertUtf8.Location = New System.Drawing.Point(668, 10)
        Me.BtnConvertUtf8.Name = "BtnConvertUtf8"
        Me.BtnConvertUtf8.Size = New System.Drawing.Size(124, 29)
        Me.BtnConvertUtf8.TabIndex = 8
        Me.BtnConvertUtf8.Text = "Convert utf8mb4"
        Me.BtnConvertUtf8.UseVisualStyleBackColor = False
        '
        'BtnDuplikat
        '
        Me.BtnDuplikat.AutoSize = True
        Me.BtnDuplikat.BackColor = System.Drawing.Color.White
        Me.BtnDuplikat.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnDuplikat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnDuplikat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnDuplikat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnDuplikat.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnDuplikat.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnDuplikat.ForeColor = System.Drawing.Color.Black
        Me.BtnDuplikat.Image = CType(resources.GetObject("BtnDuplikat.Image"), System.Drawing.Image)
        Me.BtnDuplikat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDuplikat.Location = New System.Drawing.Point(108, 10)
        Me.BtnDuplikat.Name = "BtnDuplikat"
        Me.BtnDuplikat.Size = New System.Drawing.Size(184, 29)
        Me.BtnDuplikat.TabIndex = 5
        Me.BtnDuplikat.Text = "Cek Duplikat Barang"
        Me.BtnDuplikat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDuplikat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnDuplikat.UseVisualStyleBackColor = False
        '
        'BtnCetak
        '
        Me.BtnCetak.AutoSize = True
        Me.BtnCetak.BackColor = System.Drawing.Color.White
        Me.BtnCetak.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCetak.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCetak.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCetak.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCetak.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCetak.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCetak.ForeColor = System.Drawing.Color.Black
        Me.BtnCetak.Image = CType(resources.GetObject("BtnCetak.Image"), System.Drawing.Image)
        Me.BtnCetak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.Location = New System.Drawing.Point(903, 10)
        Me.BtnCetak.Name = "BtnCetak"
        Me.BtnCetak.Size = New System.Drawing.Size(74, 29)
        Me.BtnCetak.TabIndex = 6
        Me.BtnCetak.Text = "Cetak"
        Me.BtnCetak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCetak.UseVisualStyleBackColor = False
        '
        'PrintPreviewDialog1
        '
        Me.PrintPreviewDialog1.AutoScrollMargin = New System.Drawing.Size(0, 0)
        Me.PrintPreviewDialog1.AutoScrollMinSize = New System.Drawing.Size(0, 0)
        Me.PrintPreviewDialog1.ClientSize = New System.Drawing.Size(400, 300)
        Me.PrintPreviewDialog1.Enabled = True
        Me.PrintPreviewDialog1.Icon = CType(resources.GetObject("PrintPreviewDialog1.Icon"), System.Drawing.Icon)
        Me.PrintPreviewDialog1.Name = "PrintPreviewDialog1"
        Me.PrintPreviewDialog1.Visible = False
        '
        'BtnSimpanPDF
        '
        Me.BtnSimpanPDF.AutoSize = True
        Me.BtnSimpanPDF.BackColor = System.Drawing.Color.White
        Me.BtnSimpanPDF.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpanPDF.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnSimpanPDF.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSimpanPDF.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSimpanPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpanPDF.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpanPDF.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpanPDF.Image = CType(resources.GetObject("BtnSimpanPDF.Image"), System.Drawing.Image)
        Me.BtnSimpanPDF.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanPDF.Location = New System.Drawing.Point(983, 10)
        Me.BtnSimpanPDF.Name = "BtnSimpanPDF"
        Me.BtnSimpanPDF.Size = New System.Drawing.Size(109, 29)
        Me.BtnSimpanPDF.TabIndex = 7
        Me.BtnSimpanPDF.Text = "Export PDF"
        Me.BtnSimpanPDF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanPDF.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpanPDF.UseVisualStyleBackColor = False
        '
        'FormPerbaikanDatabase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1114, 584)
        Me.Controls.Add(Me.BtnSimpanPDF)
        Me.Controls.Add(Me.BtnCetak)
        Me.Controls.Add(Me.BtnDuplikat)
        Me.Controls.Add(Me.BtnConvertUtf8)
        Me.Controls.Add(Me.BtnChecksumTables)
        Me.Controls.Add(Me.BtnCheckTables)
        Me.Controls.Add(Me.BtnCleanup)
        Me.Controls.Add(Me.ListBoxResults)
        Me.Controls.Add(Me.BtnAnalyze)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormPerbaikanDatabase"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents BtnAnalyze As Button
    Friend WithEvents ListBoxResults As ListBox
    Friend WithEvents BtnCleanup As Button
    Friend WithEvents BtnCheckTables As Button
    Friend WithEvents BtnChecksumTables As Button
    Friend WithEvents BtnConvertUtf8 As Button
    Friend WithEvents BtnDuplikat As Button
    Friend WithEvents BtnCetak As Button
    Friend WithEvents PrintPreviewDialog1 As PrintPreviewDialog
    Friend WithEvents BtnSimpanPDF As Button
End Class

