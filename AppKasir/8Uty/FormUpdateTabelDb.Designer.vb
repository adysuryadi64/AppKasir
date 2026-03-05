<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormUpdateTabelDb
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormUpdateTabelDb))
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtFilePath = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BtnCek = New System.Windows.Forms.Button()
        Me.ListBoxHasil = New System.Windows.Forms.ListBox()
        Me.BtnDebug = New System.Windows.Forms.Button()
        Me.BtnHasil = New System.Windows.Forms.Button()
        Me.Panel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.Silver
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel4.Controls.Add(Me.BtnClose)
        Me.Panel4.Controls.Add(Me.Label7)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold)
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(800, 44)
        Me.Panel4.TabIndex = 170
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
        Me.BtnClose.Location = New System.Drawing.Point(757, 1)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(33, 36)
        Me.BtnClose.TabIndex = 204
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label7.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label7.Location = New System.Drawing.Point(0, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(358, 35)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "Update Tabel Database"
        '
        'TxtFilePath
        '
        Me.TxtFilePath.BackColor = System.Drawing.SystemColors.Window
        Me.TxtFilePath.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFilePath.ForeColor = System.Drawing.Color.Black
        Me.TxtFilePath.Location = New System.Drawing.Point(275, 66)
        Me.TxtFilePath.Name = "TxtFilePath"
        Me.TxtFilePath.Size = New System.Drawing.Size(378, 23)
        Me.TxtFilePath.TabIndex = 171
        Me.TxtFilePath.Text = ".sql"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(27, 69)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(242, 17)
        Me.Label6.TabIndex = 172
        Me.Label6.Text = "Cari referensi untuk kunci format .sql"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BtnCari
        '
        Me.BtnCari.BackColor = System.Drawing.Color.Silver
        Me.BtnCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.BtnCari.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BtnCari.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.BtnCari.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCari.ForeColor = System.Drawing.Color.Black
        Me.BtnCari.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.Location = New System.Drawing.Point(659, 66)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(46, 23)
        Me.BtnCari.TabIndex = 173
        Me.BtnCari.Text = "..."
        Me.BtnCari.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCari.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(272, 102)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(91, 17)
        Me.Label1.TabIndex = 174
        Me.Label1.Text = "masterdb.sql"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BtnCek
        '
        Me.BtnCek.Location = New System.Drawing.Point(583, 102)
        Me.BtnCek.Name = "BtnCek"
        Me.BtnCek.Size = New System.Drawing.Size(122, 33)
        Me.BtnCek.TabIndex = 176
        Me.BtnCek.Text = "Cek database"
        Me.BtnCek.UseVisualStyleBackColor = True
        '
        'ListBoxHasil
        '
        Me.ListBoxHasil.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListBoxHasil.FormattingEnabled = True
        Me.ListBoxHasil.HorizontalScrollbar = True
        Me.ListBoxHasil.Location = New System.Drawing.Point(12, 149)
        Me.ListBoxHasil.Name = "ListBoxHasil"
        Me.ListBoxHasil.ScrollAlwaysVisible = True
        Me.ListBoxHasil.Size = New System.Drawing.Size(768, 472)
        Me.ListBoxHasil.TabIndex = 177
        '
        'BtnDebug
        '
        Me.BtnDebug.Location = New System.Drawing.Point(624, 637)
        Me.BtnDebug.Name = "BtnDebug"
        Me.BtnDebug.Size = New System.Drawing.Size(75, 23)
        Me.BtnDebug.TabIndex = 178
        Me.BtnDebug.Text = "Debug"
        Me.BtnDebug.UseVisualStyleBackColor = True
        '
        'BtnHasil
        '
        Me.BtnHasil.Location = New System.Drawing.Point(705, 637)
        Me.BtnHasil.Name = "BtnHasil"
        Me.BtnHasil.Size = New System.Drawing.Size(75, 23)
        Me.BtnHasil.TabIndex = 179
        Me.BtnHasil.Text = "Hasil"
        Me.BtnHasil.UseVisualStyleBackColor = True
        '
        'FormUpdateTabelDb
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 672)
        Me.Controls.Add(Me.BtnHasil)
        Me.Controls.Add(Me.BtnDebug)
        Me.Controls.Add(Me.ListBoxHasil)
        Me.Controls.Add(Me.BtnCek)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnCari)
        Me.Controls.Add(Me.TxtFilePath)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Panel4)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormUpdateTabelDb"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel4 As Panel
    Friend WithEvents BtnClose As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtFilePath As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents BtnCari As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnCek As Button
    Friend WithEvents ListBoxHasil As ListBox
    Friend WithEvents BtnDebug As Button
    Friend WithEvents BtnHasil As Button
End Class
