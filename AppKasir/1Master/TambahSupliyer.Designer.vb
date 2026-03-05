<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TambahSupliyer
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
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TambahSupliyer))
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TxtJAngkaHutang = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtTelp = New System.Windows.Forms.TextBox()
        Me.TxtAwal = New System.Windows.Forms.TextBox()
        Me.TxtAlamat = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.Dgvdata = New System.Windows.Forms.DataGridView()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblNama = New System.Windows.Forms.Label()
        Me.BTNSimpan = New System.Windows.Forms.Button()
        Me.BTNHapus = New System.Windows.Forms.Button()
        Me.BtnTambah = New System.Windows.Forms.Button()
        Me.Panel2.SuspendLayout()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Dgvdata, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Transparent
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.TxtJAngkaHutang)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.TxtTelp)
        Me.Panel2.Controls.Add(Me.TxtAwal)
        Me.Panel2.Controls.Add(Me.TxtAlamat)
        Me.Panel2.Controls.Add(Me.TxtKode)
        Me.Panel2.Controls.Add(Me.TxtNama)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Location = New System.Drawing.Point(4, 36)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(523, 134)
        Me.Panel2.TabIndex = 60
        '
        'Label9
        '
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(472, 80)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(39, 18)
        Me.Label9.TabIndex = 26
        Me.Label9.Text = "Hari"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtJAngkaHutang
        '
        Me.TxtJAngkaHutang.BackColor = System.Drawing.SystemColors.Window
        Me.TxtJAngkaHutang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtJAngkaHutang.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtJAngkaHutang.Location = New System.Drawing.Point(400, 78)
        Me.TxtJAngkaHutang.Name = "TxtJAngkaHutang"
        Me.TxtJAngkaHutang.Size = New System.Drawing.Size(71, 23)
        Me.TxtJAngkaHutang.TabIndex = 5
        Me.TxtJAngkaHutang.Text = "30"
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(277, 80)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(117, 18)
        Me.Label7.TabIndex = 25
        Me.Label7.Text = "Jangka Hutang :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(264, 105)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(228, 18)
        Me.Label6.TabIndex = 23
        Me.Label6.Text = "Rp. 0"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtTelp
        '
        Me.TxtTelp.BackColor = System.Drawing.SystemColors.Window
        Me.TxtTelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTelp.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtTelp.Location = New System.Drawing.Point(119, 78)
        Me.TxtTelp.Name = "TxtTelp"
        Me.TxtTelp.Size = New System.Drawing.Size(135, 23)
        Me.TxtTelp.TabIndex = 3
        '
        'TxtAwal
        '
        Me.TxtAwal.BackColor = System.Drawing.SystemColors.Window
        Me.TxtAwal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAwal.Location = New System.Drawing.Point(119, 103)
        Me.TxtAwal.Name = "TxtAwal"
        Me.TxtAwal.Size = New System.Drawing.Size(135, 23)
        Me.TxtAwal.TabIndex = 4
        Me.TxtAwal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtAlamat
        '
        Me.TxtAlamat.BackColor = System.Drawing.SystemColors.Window
        Me.TxtAlamat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtAlamat.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtAlamat.Location = New System.Drawing.Point(119, 53)
        Me.TxtAlamat.Name = "TxtAlamat"
        Me.TxtAlamat.Size = New System.Drawing.Size(393, 23)
        Me.TxtAlamat.TabIndex = 2
        '
        'TxtKode
        '
        Me.TxtKode.BackColor = System.Drawing.SystemColors.Window
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Enabled = False
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtKode.Location = New System.Drawing.Point(119, 5)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.Size = New System.Drawing.Size(135, 23)
        Me.TxtKode.TabIndex = 0
        '
        'TxtNama
        '
        Me.TxtNama.BackColor = System.Drawing.SystemColors.Window
        Me.TxtNama.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.TxtNama.Location = New System.Drawing.Point(119, 29)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(393, 23)
        Me.TxtNama.TabIndex = 1
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(11, 105)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(102, 18)
        Me.Label8.TabIndex = 22
        Me.Label8.Text = "Hutang Awal :"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(11, 80)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(102, 18)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Telphone :"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(12, 55)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(102, 18)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Alamat :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(10, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(103, 24)
        Me.Label2.TabIndex = 16
        Me.Label2.Text = "Nama :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(12, 7)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 18)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Kode :"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ErrorProvider1
        '
        Me.ErrorProvider1.ContainerControl = Me
        '
        'Dgvdata
        '
        Me.Dgvdata.AllowUserToAddRows = False
        Me.Dgvdata.AllowUserToDeleteRows = False
        Me.Dgvdata.AllowUserToResizeRows = False
        Me.Dgvdata.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Dgvdata.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.Dgvdata.BackgroundColor = System.Drawing.Color.White
        Me.Dgvdata.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised
        Me.Dgvdata.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Dgvdata.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.Dgvdata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Dgvdata.DefaultCellStyle = DataGridViewCellStyle4
        Me.Dgvdata.Location = New System.Drawing.Point(4, 176)
        Me.Dgvdata.Name = "Dgvdata"
        Me.Dgvdata.ReadOnly = True
        Me.Dgvdata.RowHeadersVisible = False
        Me.Dgvdata.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.Dgvdata.Size = New System.Drawing.Size(958, 400)
        Me.Dgvdata.TabIndex = 12
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.Orange
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblNama)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(966, 30)
        Me.PanelHeader.TabIndex = 164
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
        Me.BtnClose.Location = New System.Drawing.Point(931, 0)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(30, 30)
        Me.BtnClose.TabIndex = 170
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblNama
        '
        Me.LblNama.BackColor = System.Drawing.Color.Transparent
        Me.LblNama.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblNama.Font = New System.Drawing.Font("Times New Roman", 15.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNama.ForeColor = System.Drawing.Color.Black
        Me.LblNama.Location = New System.Drawing.Point(0, 0)
        Me.LblNama.Name = "LblNama"
        Me.LblNama.Size = New System.Drawing.Size(966, 30)
        Me.LblNama.TabIndex = 20
        Me.LblNama.Text = "T A M B A H   S U P L I Y E R"
        Me.LblNama.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BTNSimpan
        '
        Me.BTNSimpan.BackColor = System.Drawing.Color.Goldenrod
        Me.BTNSimpan.FlatAppearance.BorderSize = 0
        Me.BTNSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BTNSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BTNSimpan.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BTNSimpan.ForeColor = System.Drawing.Color.Black
        Me.BTNSimpan.Image = CType(resources.GetObject("BTNSimpan.Image"), System.Drawing.Image)
        Me.BTNSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BTNSimpan.Location = New System.Drawing.Point(543, 56)
        Me.BTNSimpan.Name = "BTNSimpan"
        Me.BTNSimpan.Size = New System.Drawing.Size(135, 34)
        Me.BTNSimpan.TabIndex = 8
        Me.BTNSimpan.Text = "SIMPAN (F2)"
        Me.BTNSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BTNSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BTNSimpan.UseVisualStyleBackColor = False
        '
        'BTNHapus
        '
        Me.BTNHapus.BackColor = System.Drawing.Color.Goldenrod
        Me.BTNHapus.FlatAppearance.BorderSize = 0
        Me.BTNHapus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BTNHapus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.BTNHapus.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BTNHapus.ForeColor = System.Drawing.Color.Black
        Me.BTNHapus.Image = CType(resources.GetObject("BTNHapus.Image"), System.Drawing.Image)
        Me.BTNHapus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BTNHapus.Location = New System.Drawing.Point(543, 95)
        Me.BTNHapus.Name = "BTNHapus"
        Me.BTNHapus.Size = New System.Drawing.Size(135, 34)
        Me.BTNHapus.TabIndex = 9
        Me.BTNHapus.Text = "HAPUS (F3)"
        Me.BTNHapus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BTNHapus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BTNHapus.UseVisualStyleBackColor = False
        '
        'BtnTambah
        '
        Me.BtnTambah.BackColor = System.Drawing.Color.Goldenrod
        Me.BtnTambah.FlatAppearance.BorderSize = 0
        Me.BtnTambah.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnTambah.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnTambah.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTambah.ForeColor = System.Drawing.Color.Black
        Me.BtnTambah.Image = CType(resources.GetObject("BtnTambah.Image"), System.Drawing.Image)
        Me.BtnTambah.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTambah.Location = New System.Drawing.Point(543, 135)
        Me.BtnTambah.Name = "BtnTambah"
        Me.BtnTambah.Size = New System.Drawing.Size(135, 34)
        Me.BtnTambah.TabIndex = 10
        Me.BtnTambah.Text = "RESET (F4)"
        Me.BtnTambah.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnTambah.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTambah.UseVisualStyleBackColor = False
        '
        'TambahSupliyer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(966, 588)
        Me.Controls.Add(Me.BTNSimpan)
        Me.Controls.Add(Me.BTNHapus)
        Me.Controls.Add(Me.BtnTambah)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Dgvdata)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "TambahSupliyer"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "TambahSupliyer"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Dgvdata, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHeader.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents TxtTelp As System.Windows.Forms.TextBox
    Friend WithEvents TxtAwal As System.Windows.Forms.TextBox
    Friend WithEvents TxtAlamat As System.Windows.Forms.TextBox
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents TxtNama As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
    Friend WithEvents Dgvdata As System.Windows.Forms.DataGridView
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents LblNama As System.Windows.Forms.Label
    Friend WithEvents BTNSimpan As System.Windows.Forms.Button
    Friend WithEvents BTNHapus As System.Windows.Forms.Button
    Friend WithEvents BtnTambah As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents TxtJAngkaHutang As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents BtnClose As System.Windows.Forms.Button
End Class
