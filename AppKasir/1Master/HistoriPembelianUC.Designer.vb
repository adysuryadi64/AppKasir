<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HistoriPembelianUC
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.DGVHistori = New System.Windows.Forms.DataGridView()
        Me.LblStatistik = New System.Windows.Forms.Label()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnRefresh = New System.Windows.Forms.Button()
        CType(Me.DGVHistori, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHeader.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DGVHistori
        '
        Me.DGVHistori.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVHistori.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVHistori.Location = New System.Drawing.Point(0, 0)
        Me.DGVHistori.Name = "DGVHistori"
        Me.DGVHistori.Size = New System.Drawing.Size(1309, 536)
        Me.DGVHistori.TabIndex = 0
        '
        'LblStatistik
        '
        Me.LblStatistik.AutoSize = True
        Me.LblStatistik.Location = New System.Drawing.Point(3, 9)
        Me.LblStatistik.Name = "LblStatistik"
        Me.LblStatistik.Size = New System.Drawing.Size(39, 13)
        Me.LblStatistik.TabIndex = 1
        Me.LblStatistik.Text = "Label1"
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.BtnRefresh)
        Me.PanelHeader.Controls.Add(Me.LblStatistik)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelHeader.Location = New System.Drawing.Point(0, 536)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1309, 33)
        Me.PanelHeader.TabIndex = 4
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.DGVHistori)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1309, 536)
        Me.Panel1.TabIndex = 5
        '
        'BtnRefresh
        '
        Me.BtnRefresh.Location = New System.Drawing.Point(1221, 6)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(75, 23)
        Me.BtnRefresh.TabIndex = 2
        Me.BtnRefresh.Text = "Refresh"
        Me.BtnRefresh.UseVisualStyleBackColor = True
        '
        'HistoriPembelianUC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelHeader)
        Me.Name = "HistoriPembelianUC"
        Me.Size = New System.Drawing.Size(1309, 569)
        CType(Me.DGVHistori, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DGVHistori As DataGridView
    Friend WithEvents LblStatistik As Label
    Friend WithEvents PanelHeader As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnRefresh As Button
End Class
