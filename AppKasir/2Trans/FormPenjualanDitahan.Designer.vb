<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPenjualanDitahan
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormPenjualanDitahan))
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.BtnProses = New System.Windows.Forms.Button()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.TxtPel = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DgvData
        '
        Me.DgvData.AllowUserToAddRows = False
        Me.DgvData.AllowUserToDeleteRows = False
        Me.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvData.BackgroundColor = System.Drawing.SystemColors.Control
        Me.DgvData.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvData.DefaultCellStyle = DataGridViewCellStyle2
        Me.DgvData.Dock = System.Windows.Forms.DockStyle.Top
        Me.DgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke
        Me.DgvData.Location = New System.Drawing.Point(0, 0)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.ReadOnly = True
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvData.Size = New System.Drawing.Size(689, 280)
        Me.DgvData.TabIndex = 2
        '
        'BtnProses
        '
        Me.BtnProses.BackColor = System.Drawing.Color.DarkOrange
        Me.BtnProses.FlatAppearance.BorderSize = 0
        Me.BtnProses.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnProses.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MidnightBlue
        Me.BtnProses.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnProses.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnProses.ForeColor = System.Drawing.Color.Black
        Me.BtnProses.Image = CType(resources.GetObject("BtnProses.Image"), System.Drawing.Image)
        Me.BtnProses.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnProses.Location = New System.Drawing.Point(293, 287)
        Me.BtnProses.Name = "BtnProses"
        Me.BtnProses.Size = New System.Drawing.Size(224, 32)
        Me.BtnProses.TabIndex = 37
        Me.BtnProses.Text = "       Ambil Data (F9)"
        Me.BtnProses.UseVisualStyleBackColor = False
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.TxtFaktur.Location = New System.Drawing.Point(74, 292)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.ReadOnly = True
        Me.TxtFaktur.Size = New System.Drawing.Size(195, 22)
        Me.TxtFaktur.TabIndex = 82
        Me.TxtFaktur.Text = "Faktur"
        '
        'TxtPel
        '
        Me.TxtPel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.TxtPel.Location = New System.Drawing.Point(523, 293)
        Me.TxtPel.Name = "TxtPel"
        Me.TxtPel.ReadOnly = True
        Me.TxtPel.Size = New System.Drawing.Size(154, 22)
        Me.TxtPel.TabIndex = 83
        Me.TxtPel.Text = "NamaPelanggan"
        Me.TxtPel.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label1.Location = New System.Drawing.Point(8, 295)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 16)
        Me.Label1.TabIndex = 84
        Me.Label1.Text = "No Faktur"
        '
        'FormPenjualanDitahan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(689, 322)
        Me.Controls.Add(Me.DgvData)
        Me.Controls.Add(Me.BtnProses)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TxtPel)
        Me.Controls.Add(Me.TxtFaktur)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "FormPenjualanDitahan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DATA PENJUALAN DI TAHAN"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DgvData As System.Windows.Forms.DataGridView
    Friend WithEvents BtnProses As System.Windows.Forms.Button
    Friend WithEvents TxtFaktur As System.Windows.Forms.TextBox
    Friend WithEvents TxtPel As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
