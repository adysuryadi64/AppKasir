<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormArmada
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormArmada))
        Me.Dgvdata = New System.Windows.Forms.DataGridView()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblNama = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.BtnTambah = New System.Windows.Forms.Button()
        Me.BTNHapus = New System.Windows.Forms.Button()
        Me.BTNSimpan = New System.Windows.Forms.Button()
        Me.TxtJenis = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.TxtNopol = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        CType(Me.Dgvdata, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHeader.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Dgvdata
        '
        Me.Dgvdata.AllowUserToAddRows = False
        Me.Dgvdata.AllowUserToDeleteRows = False
        Me.Dgvdata.AllowUserToResizeRows = False
        Me.Dgvdata.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Dgvdata.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.Dgvdata.BackgroundColor = System.Drawing.Color.White
        Me.Dgvdata.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised
        Me.Dgvdata.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Dgvdata.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.Dgvdata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Dgvdata.DefaultCellStyle = DataGridViewCellStyle2
        Me.Dgvdata.Location = New System.Drawing.Point(12, 176)
        Me.Dgvdata.Name = "Dgvdata"
        Me.Dgvdata.ReadOnly = True
        Me.Dgvdata.RowHeadersVisible = False
        Me.Dgvdata.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.Dgvdata.Size = New System.Drawing.Size(527, 342)
        Me.Dgvdata.TabIndex = 172
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.Orange
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblNama)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(551, 30)
        Me.PanelHeader.TabIndex = 171
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.ForeColor = System.Drawing.Color.White
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(516, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(23, 23)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'LblNama
        '
        Me.LblNama.AutoSize = True
        Me.LblNama.BackColor = System.Drawing.Color.Transparent
        Me.LblNama.Font = New System.Drawing.Font("Times New Roman", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNama.ForeColor = System.Drawing.Color.Black
        Me.LblNama.Location = New System.Drawing.Point(122, 3)
        Me.LblNama.Name = "LblNama"
        Me.LblNama.Size = New System.Drawing.Size(203, 24)
        Me.LblNama.TabIndex = 20
        Me.LblNama.Text = "D A T A  A R M A D A"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Transparent
        Me.Panel2.Controls.Add(Me.BtnTambah)
        Me.Panel2.Controls.Add(Me.BTNHapus)
        Me.Panel2.Controls.Add(Me.BTNSimpan)
        Me.Panel2.Controls.Add(Me.TxtJenis)
        Me.Panel2.Controls.Add(Me.TxtKode)
        Me.Panel2.Controls.Add(Me.TxtNopol)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Location = New System.Drawing.Point(12, 43)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(527, 127)
        Me.Panel2.TabIndex = 170
        '
        'BtnTambah
        '
        Me.BtnTambah.BackColor = System.Drawing.Color.Goldenrod
        Me.BtnTambah.FlatAppearance.BorderSize = 0
        Me.BtnTambah.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnTambah.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnTambah.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTambah.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTambah.ForeColor = System.Drawing.Color.Black
        Me.BtnTambah.Image = CType(resources.GetObject("BtnTambah.Image"), System.Drawing.Image)
        Me.BtnTambah.Location = New System.Drawing.Point(378, 85)
        Me.BtnTambah.Name = "BtnTambah"
        Me.BtnTambah.Size = New System.Drawing.Size(130, 33)
        Me.BtnTambah.TabIndex = 175
        Me.BtnTambah.Text = " RESET (F4)"
        Me.BtnTambah.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTambah.UseVisualStyleBackColor = False
        '
        'BTNHapus
        '
        Me.BTNHapus.BackColor = System.Drawing.Color.Goldenrod
        Me.BTNHapus.FlatAppearance.BorderSize = 0
        Me.BTNHapus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BTNHapus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.BTNHapus.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BTNHapus.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BTNHapus.ForeColor = System.Drawing.Color.Black
        Me.BTNHapus.Image = CType(resources.GetObject("BTNHapus.Image"), System.Drawing.Image)
        Me.BTNHapus.Location = New System.Drawing.Point(238, 85)
        Me.BTNHapus.Name = "BTNHapus"
        Me.BTNHapus.Size = New System.Drawing.Size(134, 33)
        Me.BTNHapus.TabIndex = 174
        Me.BTNHapus.Text = "HAPUS (F3)"
        Me.BTNHapus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BTNHapus.UseVisualStyleBackColor = False
        '
        'BTNSimpan
        '
        Me.BTNSimpan.BackColor = System.Drawing.Color.Goldenrod
        Me.BTNSimpan.FlatAppearance.BorderSize = 0
        Me.BTNSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BTNSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BTNSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BTNSimpan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BTNSimpan.ForeColor = System.Drawing.Color.Black
        Me.BTNSimpan.Image = CType(resources.GetObject("BTNSimpan.Image"), System.Drawing.Image)
        Me.BTNSimpan.Location = New System.Drawing.Point(90, 85)
        Me.BTNSimpan.Name = "BTNSimpan"
        Me.BTNSimpan.Size = New System.Drawing.Size(142, 33)
        Me.BTNSimpan.TabIndex = 173
        Me.BTNSimpan.Text = "SIMPAN (F2)"
        Me.BTNSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BTNSimpan.UseVisualStyleBackColor = False
        '
        'TxtJenis
        '
        Me.TxtJenis.BackColor = System.Drawing.SystemColors.Window
        Me.TxtJenis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtJenis.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJenis.Location = New System.Drawing.Point(130, 57)
        Me.TxtJenis.Name = "TxtJenis"
        Me.TxtJenis.Size = New System.Drawing.Size(376, 22)
        Me.TxtJenis.TabIndex = 2
        '
        'TxtKode
        '
        Me.TxtKode.BackColor = System.Drawing.SystemColors.Window
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Enabled = False
        Me.TxtKode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(130, 10)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.Size = New System.Drawing.Size(135, 22)
        Me.TxtKode.TabIndex = 0
        '
        'TxtNopol
        '
        Me.TxtNopol.BackColor = System.Drawing.SystemColors.Window
        Me.TxtNopol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNopol.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNopol.Location = New System.Drawing.Point(130, 33)
        Me.TxtNopol.Name = "TxtNopol"
        Me.TxtNopol.Size = New System.Drawing.Size(135, 22)
        Me.TxtNopol.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(23, 60)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(102, 18)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Jenis :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(21, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(103, 24)
        Me.Label2.TabIndex = 16
        Me.Label2.Text = "Nopol :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(23, 13)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 18)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Kode :"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtNama
        '
        Me.TxtNama.BackColor = System.Drawing.SystemColors.Window
        Me.TxtNama.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNama.Location = New System.Drawing.Point(130, 33)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(135, 22)
        Me.TxtNama.TabIndex = 1
        '
        'FormArmada
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(551, 523)
        Me.Controls.Add(Me.Dgvdata)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.Panel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormArmada"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormArmada"
        CType(Me.Dgvdata, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BTNSimpan As System.Windows.Forms.Button
    Friend WithEvents BTNHapus As System.Windows.Forms.Button
    Friend WithEvents BtnTambah As System.Windows.Forms.Button
    Friend WithEvents Dgvdata As System.Windows.Forms.DataGridView
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblNama As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents TxtJenis As System.Windows.Forms.TextBox
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents TxtNopol As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TxtNama As System.Windows.Forms.TextBox
End Class
