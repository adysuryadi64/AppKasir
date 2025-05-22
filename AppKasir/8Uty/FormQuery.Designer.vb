<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormQuery
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormQuery))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.RtbQuery = New System.Windows.Forms.RichTextBox()
        Me.ListBoxHasil = New System.Windows.Forms.ListBox()
        Me.BtnEksekusi = New System.Windows.Forms.Button()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LabelJudul = New System.Windows.Forms.Label()
        Me.ListBoxTabel = New System.Windows.Forms.ListBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ListBoxKolom = New System.Windows.Forms.ListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(719, 45)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(187, 15)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Masukkan perintah berupa query"
        '
        'RtbQuery
        '
        Me.RtbQuery.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RtbQuery.Location = New System.Drawing.Point(544, 62)
        Me.RtbQuery.Margin = New System.Windows.Forms.Padding(4)
        Me.RtbQuery.Name = "RtbQuery"
        Me.RtbQuery.Size = New System.Drawing.Size(572, 82)
        Me.RtbQuery.TabIndex = 1
        Me.RtbQuery.Text = ""
        '
        'ListBoxHasil
        '
        Me.ListBoxHasil.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListBoxHasil.FormattingEnabled = True
        Me.ListBoxHasil.ItemHeight = 15
        Me.ListBoxHasil.Location = New System.Drawing.Point(544, 187)
        Me.ListBoxHasil.Name = "ListBoxHasil"
        Me.ListBoxHasil.Size = New System.Drawing.Size(572, 259)
        Me.ListBoxHasil.TabIndex = 3
        '
        'BtnEksekusi
        '
        Me.BtnEksekusi.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnEksekusi.BackColor = System.Drawing.Color.Teal
        Me.BtnEksekusi.FlatAppearance.BorderSize = 0
        Me.BtnEksekusi.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnEksekusi.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BtnEksekusi.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnEksekusi.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnEksekusi.ForeColor = System.Drawing.Color.White
        Me.BtnEksekusi.Image = CType(resources.GetObject("BtnEksekusi.Image"), System.Drawing.Image)
        Me.BtnEksekusi.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnEksekusi.Location = New System.Drawing.Point(995, 151)
        Me.BtnEksekusi.Name = "BtnEksekusi"
        Me.BtnEksekusi.Size = New System.Drawing.Size(121, 30)
        Me.BtnEksekusi.TabIndex = 135
        Me.BtnEksekusi.Text = "Jalankan"
        Me.BtnEksekusi.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnEksekusi.UseVisualStyleBackColor = False
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Yellow
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GreenYellow
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GreenYellow
        Me.BtnClose.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.Black
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnClose.Location = New System.Drawing.Point(1083, 0)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(33, 36)
        Me.BtnClose.TabIndex = 203
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LabelJudul
        '
        Me.LabelJudul.BackColor = System.Drawing.Color.Gold
        Me.LabelJudul.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabelJudul.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelJudul.Location = New System.Drawing.Point(0, 0)
        Me.LabelJudul.Name = "LabelJudul"
        Me.LabelJudul.Size = New System.Drawing.Size(1128, 36)
        Me.LabelJudul.TabIndex = 204
        Me.LabelJudul.Text = "PERINTAH QUERY"
        Me.LabelJudul.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ListBoxTabel
        '
        Me.ListBoxTabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListBoxTabel.FormattingEnabled = True
        Me.ListBoxTabel.ItemHeight = 15
        Me.ListBoxTabel.Location = New System.Drawing.Point(4, 53)
        Me.ListBoxTabel.Name = "ListBoxTabel"
        Me.ListBoxTabel.Size = New System.Drawing.Size(254, 394)
        Me.ListBoxTabel.TabIndex = 205
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 36)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(126, 15)
        Me.Label2.TabIndex = 206
        Me.Label2.Text = "Daftar tabel Database"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(262, 36)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(133, 15)
        Me.Label3.TabIndex = 208
        Me.Label3.Text = "Daftar kolom Database"
        '
        'ListBoxKolom
        '
        Me.ListBoxKolom.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListBoxKolom.FormattingEnabled = True
        Me.ListBoxKolom.ItemHeight = 15
        Me.ListBoxKolom.Location = New System.Drawing.Point(264, 53)
        Me.ListBoxKolom.Name = "ListBoxKolom"
        Me.ListBoxKolom.Size = New System.Drawing.Size(274, 394)
        Me.ListBoxKolom.TabIndex = 207
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(103, 26)
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(102, 22)
        Me.CopyToolStripMenuItem.Text = "Copy"
        '
        'FormQuery
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1128, 459)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ListBoxKolom)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ListBoxTabel)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.LabelJudul)
        Me.Controls.Add(Me.BtnEksekusi)
        Me.Controls.Add(Me.ListBoxHasil)
        Me.Controls.Add(Me.RtbQuery)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "FormQuery"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents RtbQuery As System.Windows.Forms.RichTextBox
    Friend WithEvents ListBoxHasil As System.Windows.Forms.ListBox
    Friend WithEvents BtnEksekusi As System.Windows.Forms.Button
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LabelJudul As System.Windows.Forms.Label
    Friend WithEvents ListBoxTabel As System.Windows.Forms.ListBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ListBoxKolom As System.Windows.Forms.ListBox
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
