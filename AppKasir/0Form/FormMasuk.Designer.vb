<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMasuk
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMasuk))
        Me.BtnGudang = New System.Windows.Forms.Button()
        Me.BtnToko = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'BtnGudang
        '
        Me.BtnGudang.AutoEllipsis = True
        Me.BtnGudang.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnGudang.BackColor = System.Drawing.Color.Chocolate
        Me.BtnGudang.FlatAppearance.BorderColor = System.Drawing.Color.Yellow
        Me.BtnGudang.FlatAppearance.BorderSize = 4
        Me.BtnGudang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red
        Me.BtnGudang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gold
        Me.BtnGudang.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnGudang.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnGudang.ForeColor = System.Drawing.Color.Black
        Me.BtnGudang.Image = CType(resources.GetObject("BtnGudang.Image"), System.Drawing.Image)
        Me.BtnGudang.Location = New System.Drawing.Point(217, 25)
        Me.BtnGudang.Name = "BtnGudang"
        Me.BtnGudang.Size = New System.Drawing.Size(169, 133)
        Me.BtnGudang.TabIndex = 2
        Me.BtnGudang.Text = "G U D A N G"
        Me.BtnGudang.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.BtnGudang.UseVisualStyleBackColor = False
        '
        'BtnToko
        '
        Me.BtnToko.AutoEllipsis = True
        Me.BtnToko.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnToko.BackColor = System.Drawing.Color.Chocolate
        Me.BtnToko.FlatAppearance.BorderColor = System.Drawing.Color.Yellow
        Me.BtnToko.FlatAppearance.BorderSize = 4
        Me.BtnToko.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red
        Me.BtnToko.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gold
        Me.BtnToko.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnToko.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnToko.ForeColor = System.Drawing.Color.Black
        Me.BtnToko.Image = CType(resources.GetObject("BtnToko.Image"), System.Drawing.Image)
        Me.BtnToko.Location = New System.Drawing.Point(23, 25)
        Me.BtnToko.Name = "BtnToko"
        Me.BtnToko.Size = New System.Drawing.Size(169, 133)
        Me.BtnToko.TabIndex = 1
        Me.BtnToko.Text = "T O K O"
        Me.BtnToko.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.BtnToko.UseVisualStyleBackColor = False
        '
        'FormMasuk
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Sienna
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(413, 189)
        Me.Controls.Add(Me.BtnGudang)
        Me.Controls.Add(Me.BtnToko)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormMasuk"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnToko As System.Windows.Forms.Button
    Friend WithEvents BtnGudang As System.Windows.Forms.Button
End Class
