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
        Me.BtnBuatMigrasi = New System.Windows.Forms.Button()
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
        Me.BtnClose.BackColor = System.Drawing.Color.White
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(757, 1)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(33, 36)
        Me.BtnClose.TabIndex = 204
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
        Me.BtnCari.AutoSize = True
        Me.BtnCari.BackColor = System.Drawing.Color.White
        Me.BtnCari.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCari.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCari.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCari.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCari.ForeColor = System.Drawing.Color.Black
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.Location = New System.Drawing.Point(659, 60)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(120, 32)
        Me.BtnCari.TabIndex = 173
        Me.BtnCari.Text = "Cari File"
        Me.BtnCari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.BtnCek.AutoSize = True
        Me.BtnCek.BackColor = System.Drawing.Color.White
        Me.BtnCek.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCek.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnCek.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCek.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCek.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCek.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCek.ForeColor = System.Drawing.Color.Black
        Me.BtnCek.Image = CType(resources.GetObject("BtnCek.Image"), System.Drawing.Image)
        Me.BtnCek.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCek.Location = New System.Drawing.Point(583, 102)
        Me.BtnCek.Name = "BtnCek"
        Me.BtnCek.Size = New System.Drawing.Size(160, 32)
        Me.BtnCek.TabIndex = 176
        Me.BtnCek.Text = "Cek Database"
        Me.BtnCek.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCek.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCek.UseVisualStyleBackColor = False
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
        Me.BtnDebug.AutoSize = True
        Me.BtnDebug.BackColor = System.Drawing.Color.White
        Me.BtnDebug.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnDebug.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnDebug.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnDebug.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnDebug.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnDebug.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnDebug.ForeColor = System.Drawing.Color.Black
        Me.BtnDebug.Image = CType(resources.GetObject("BtnDebug.Image"), System.Drawing.Image)
        Me.BtnDebug.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDebug.Location = New System.Drawing.Point(624, 637)
        Me.BtnDebug.Name = "BtnDebug"
        Me.BtnDebug.Size = New System.Drawing.Size(120, 32)
        Me.BtnDebug.TabIndex = 178
        Me.BtnDebug.Text = "Debug"
        Me.BtnDebug.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnDebug.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnDebug.UseVisualStyleBackColor = False
        '
        'BtnHasil
        '
        Me.BtnHasil.AutoSize = True
        Me.BtnHasil.BackColor = System.Drawing.Color.White
        Me.BtnHasil.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnHasil.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnHasil.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnHasil.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnHasil.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnHasil.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHasil.ForeColor = System.Drawing.Color.Black
        Me.BtnHasil.Image = CType(resources.GetObject("BtnHasil.Image"), System.Drawing.Image)
        Me.BtnHasil.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHasil.Location = New System.Drawing.Point(705, 637)
        Me.BtnHasil.Name = "BtnHasil"
        Me.BtnHasil.Size = New System.Drawing.Size(120, 32)
        Me.BtnHasil.TabIndex = 179
        Me.BtnHasil.Text = "Lihat Hasil"
        Me.BtnHasil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHasil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnHasil.UseVisualStyleBackColor = False
        '
        'BtnBuatMigrasi
        '
        Me.BtnBuatMigrasi.AutoSize = True
        Me.BtnBuatMigrasi.BackColor = System.Drawing.Color.White
        Me.BtnBuatMigrasi.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBuatMigrasi.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnBuatMigrasi.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnBuatMigrasi.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnBuatMigrasi.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBuatMigrasi.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBuatMigrasi.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnBuatMigrasi.Image = CType(resources.GetObject("BtnBuatMigrasi.Image"), System.Drawing.Image)
        Me.BtnBuatMigrasi.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBuatMigrasi.Location = New System.Drawing.Point(288, 637)
        Me.BtnBuatMigrasi.Name = "BtnBuatMigrasi"
        Me.BtnBuatMigrasi.Size = New System.Drawing.Size(160, 32)
        Me.BtnBuatMigrasi.TabIndex = 180
        Me.BtnBuatMigrasi.Text = "Buat Migrasi"
        Me.BtnBuatMigrasi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBuatMigrasi.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBuatMigrasi.UseVisualStyleBackColor = False
        '
        'FormUpdateTabelDb
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 672)
        Me.Controls.Add(Me.BtnBuatMigrasi)
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
    Friend WithEvents BtnBuatMigrasi As Button
End Class
