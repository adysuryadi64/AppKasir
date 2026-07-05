<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormSchemaValidator
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
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.TreeViewDiff = New System.Windows.Forms.TreeView()
        Me.TxtSqlPreview = New System.Windows.Forms.TextBox()
        Me.BtnCekSchema = New System.Windows.Forms.Button()
        Me.BtnGenerateMigration = New System.Windows.Forms.Button()
        Me.BtnApplyMigration = New System.Windows.Forms.Button()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.LblSummary = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        ' LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Black
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Century Gothic", 14.0!, System.Drawing.FontStyle.Bold)
        Me.LblHeaderForm.ForeColor = System.Drawing.Color.White
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(984, 36)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "SCHEMA VALIDATOR"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        ' BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.White
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.ForeColor = System.Drawing.Color.White
        Me.BtnClose.Location = New System.Drawing.Point(950, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 28)
        Me.BtnClose.TabIndex = 1
        Me.BtnClose.Text = "X"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        ' BtnCekSchema
        '
        Me.BtnCekSchema.AutoSize = True
        Me.BtnCekSchema.BackColor = System.Drawing.Color.White
        Me.BtnCekSchema.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCekSchema.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnCekSchema.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnCekSchema.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnCekSchema.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCekSchema.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnCekSchema.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnCekSchema.Location = New System.Drawing.Point(12, 42)
        Me.BtnCekSchema.Name = "BtnCekSchema"
        Me.BtnCekSchema.Size = New System.Drawing.Size(140, 30)
        Me.BtnCekSchema.TabIndex = 2
        Me.BtnCekSchema.Text = "🔍 Cek Skema"
        Me.BtnCekSchema.UseVisualStyleBackColor = False
        '
        ' BtnGenerateMigration
        '
        Me.BtnGenerateMigration.AutoSize = True
        Me.BtnGenerateMigration.BackColor = System.Drawing.Color.White
        Me.BtnGenerateMigration.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnGenerateMigration.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnGenerateMigration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnGenerateMigration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnGenerateMigration.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnGenerateMigration.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnGenerateMigration.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnGenerateMigration.Location = New System.Drawing.Point(160, 42)
        Me.BtnGenerateMigration.Name = "BtnGenerateMigration"
        Me.BtnGenerateMigration.Size = New System.Drawing.Size(180, 30)
        Me.BtnGenerateMigration.TabIndex = 3
        Me.BtnGenerateMigration.Text = "📄 Generate Migration"
        Me.BtnGenerateMigration.UseVisualStyleBackColor = False
        '
        ' BtnApplyMigration
        '
        Me.BtnApplyMigration.AutoSize = True
        Me.BtnApplyMigration.BackColor = System.Drawing.Color.White
        Me.BtnApplyMigration.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnApplyMigration.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnApplyMigration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(202, Byte), Integer), CType(CType(202, Byte), Integer))
        Me.BtnApplyMigration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnApplyMigration.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnApplyMigration.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnApplyMigration.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnApplyMigration.Location = New System.Drawing.Point(348, 42)
        Me.BtnApplyMigration.Name = "BtnApplyMigration"
        Me.BtnApplyMigration.Size = New System.Drawing.Size(170, 30)
        Me.BtnApplyMigration.TabIndex = 4
        Me.BtnApplyMigration.Text = "⚡ Apply Migration"
        Me.BtnApplyMigration.UseVisualStyleBackColor = False
        '
        ' LblStatus
        '
        Me.LblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblStatus.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.LblStatus.ForeColor = System.Drawing.Color.Gray
        Me.LblStatus.Location = New System.Drawing.Point(524, 48)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(448, 20)
        Me.LblStatus.TabIndex = 5
        Me.LblStatus.Text = "Siap"
        Me.LblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' LblSummary
        '
        Me.LblSummary.AutoSize = True
        Me.LblSummary.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblSummary.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.LblSummary.Location = New System.Drawing.Point(12, 78)
        Me.LblSummary.Name = "LblSummary"
        Me.LblSummary.Size = New System.Drawing.Size(0, 17)
        Me.LblSummary.TabIndex = 6
        '
        ' Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(12, 100)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(120, 16)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Selisih Skema"
        '
        ' Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(5, 5)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(130, 16)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "SQL Migration"
        '
        ' SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(12, 118)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        ' SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TreeViewDiff)
        '
        ' SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TxtSqlPreview)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
        Me.SplitContainer1.Size = New System.Drawing.Size(960, 540)
        Me.SplitContainer1.SplitterDistance = 380
        Me.SplitContainer1.TabIndex = 9
        '
        ' TreeViewDiff
        '
        Me.TreeViewDiff.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeViewDiff.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TreeViewDiff.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewDiff.Name = "TreeViewDiff"
        Me.TreeViewDiff.Size = New System.Drawing.Size(380, 540)
        Me.TreeViewDiff.TabIndex = 0
        '
        ' TxtSqlPreview
        '
        Me.TxtSqlPreview.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TxtSqlPreview.Font = New System.Drawing.Font("Consolas", 9.0!)
        Me.TxtSqlPreview.Location = New System.Drawing.Point(0, 22)
        Me.TxtSqlPreview.Multiline = True
        Me.TxtSqlPreview.Name = "TxtSqlPreview"
        Me.TxtSqlPreview.ReadOnly = True
        Me.TxtSqlPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TxtSqlPreview.Size = New System.Drawing.Size(576, 518)
        Me.TxtSqlPreview.TabIndex = 0
        Me.TxtSqlPreview.WordWrap = False
        '
        ' FormSchemaValidator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(984, 671)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.LblSummary)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.BtnApplyMigration)
        Me.Controls.Add(Me.BtnGenerateMigration)
        Me.Controls.Add(Me.BtnCekSchema)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormSchemaValidator"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormSchemaValidator"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents BtnCekSchema As System.Windows.Forms.Button
    Friend WithEvents BtnGenerateMigration As System.Windows.Forms.Button
    Friend WithEvents BtnApplyMigration As System.Windows.Forms.Button
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents LblSummary As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TreeViewDiff As System.Windows.Forms.TreeView
    Friend WithEvents TxtSqlPreview As System.Windows.Forms.TextBox
End Class
