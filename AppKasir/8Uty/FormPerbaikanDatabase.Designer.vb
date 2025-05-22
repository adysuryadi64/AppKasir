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
        Me.BtnAnalyze = New System.Windows.Forms.Button()
        Me.ListBoxResults = New System.Windows.Forms.ListBox()
        Me.BtnCleanup = New System.Windows.Forms.Button()
        Me.BtnCheckTables = New System.Windows.Forms.Button()
        Me.BtnChecksumTables = New System.Windows.Forms.Button()
        Me.BtnDuplikat = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'BtnAnalyze
        '
        Me.BtnAnalyze.AutoSize = True
        Me.BtnAnalyze.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnAnalyze.Location = New System.Drawing.Point(283, 12)
        Me.BtnAnalyze.Name = "BtnAnalyze"
        Me.BtnAnalyze.Size = New System.Drawing.Size(75, 26)
        Me.BtnAnalyze.TabIndex = 0
        Me.BtnAnalyze.Text = "Analyze"
        Me.BtnAnalyze.UseVisualStyleBackColor = True
        '
        'ListBoxResults
        '
        Me.ListBoxResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListBoxResults.FormattingEnabled = True
        Me.ListBoxResults.Location = New System.Drawing.Point(13, 45)
        Me.ListBoxResults.Name = "ListBoxResults"
        Me.ListBoxResults.Size = New System.Drawing.Size(948, 511)
        Me.ListBoxResults.TabIndex = 1
        '
        'BtnCleanup
        '
        Me.BtnCleanup.AutoSize = True
        Me.BtnCleanup.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnCleanup.Location = New System.Drawing.Point(12, 12)
        Me.BtnCleanup.Name = "BtnCleanup"
        Me.BtnCleanup.Size = New System.Drawing.Size(75, 26)
        Me.BtnCleanup.TabIndex = 2
        Me.BtnCleanup.Text = "Cleanup"
        Me.BtnCleanup.UseVisualStyleBackColor = True
        '
        'BtnCheckTables
        '
        Me.BtnCheckTables.AutoSize = True
        Me.BtnCheckTables.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnCheckTables.Location = New System.Drawing.Point(364, 12)
        Me.BtnCheckTables.Name = "BtnCheckTables"
        Me.BtnCheckTables.Size = New System.Drawing.Size(85, 26)
        Me.BtnCheckTables.TabIndex = 3
        Me.BtnCheckTables.Text = "CheckTables"
        Me.BtnCheckTables.UseVisualStyleBackColor = True
        '
        'BtnChecksumTables
        '
        Me.BtnChecksumTables.AutoSize = True
        Me.BtnChecksumTables.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnChecksumTables.Location = New System.Drawing.Point(455, 12)
        Me.BtnChecksumTables.Name = "BtnChecksumTables"
        Me.BtnChecksumTables.Size = New System.Drawing.Size(107, 26)
        Me.BtnChecksumTables.TabIndex = 4
        Me.BtnChecksumTables.Text = "ChecksumTables"
        Me.BtnChecksumTables.UseVisualStyleBackColor = True
        '
        'BtnDuplikat
        '
        Me.BtnDuplikat.AutoSize = True
        Me.BtnDuplikat.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnDuplikat.Location = New System.Drawing.Point(93, 12)
        Me.BtnDuplikat.Name = "BtnDuplikat"
        Me.BtnDuplikat.Size = New System.Drawing.Size(184, 26)
        Me.BtnDuplikat.TabIndex = 5
        Me.BtnDuplikat.Text = "Duplikat kode dan nama barang"
        Me.BtnDuplikat.UseVisualStyleBackColor = True
        '
        'FormPerbaikanDatabase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(966, 560)
        Me.Controls.Add(Me.BtnDuplikat)
        Me.Controls.Add(Me.BtnChecksumTables)
        Me.Controls.Add(Me.BtnCheckTables)
        Me.Controls.Add(Me.BtnCleanup)
        Me.Controls.Add(Me.ListBoxResults)
        Me.Controls.Add(Me.BtnAnalyze)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
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
    Friend WithEvents BtnDuplikat As Button
End Class
