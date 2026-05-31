<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormTukarPoin
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
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.PanelPelanggan = New System.Windows.Forms.Panel()
        Me.LstHasilCariPelanggan = New System.Windows.Forms.ListBox()
        Me.LblSaldoPoinTukar = New System.Windows.Forms.Label()
        Me.LblKodePelanggan = New System.Windows.Forms.Label()
        Me.TxtPelanggan = New System.Windows.Forms.TextBox()
        Me.LblCariPelanggan = New System.Windows.Forms.Label()
        Me.DgvBarangTukar = New System.Windows.Forms.DataGridView()
        Me.PanelRingkasan = New System.Windows.Forms.Panel()
        Me.LblSummary = New System.Windows.Forms.Label()
        Me.LblTotalPoinDibutuhkan = New System.Windows.Forms.Label()
        Me.LblSisaPoinSetelah = New System.Windows.Forms.Label()
        Me.BtnKonfirmasiTukar = New System.Windows.Forms.Button()
        Me.BtnRefresh = New System.Windows.Forms.Button()
        Me.TxtJenisTrans = New System.Windows.Forms.TextBox()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.PanelHeader.SuspendLayout()
        Me.PanelPelanggan.SuspendLayout()
        CType(Me.DgvBarangTukar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelRingkasan.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblHeader)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold)
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(900, 40)
        Me.PanelHeader.TabIndex = 0
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnClose.ForeColor = System.Drawing.Color.DarkRed
        Me.BtnClose.Location = New System.Drawing.Point(865, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(23, 23)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.Text = "X"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblHeader
        '
        Me.LblHeader.BackColor = System.Drawing.Color.Transparent
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeader.Font = New System.Drawing.Font("Bookman Old Style", 18.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle))
        Me.LblHeader.Location = New System.Drawing.Point(0, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Size = New System.Drawing.Size(900, 40)
        Me.LblHeader.TabIndex = 1
        Me.LblHeader.Text = "T U K A R   P O I N"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelPelanggan
        '
        Me.PanelPelanggan.BackColor = System.Drawing.SystemColors.Control
        Me.PanelPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelPelanggan.Controls.Add(Me.LblSaldoPoinTukar)
        Me.PanelPelanggan.Controls.Add(Me.LblKodePelanggan)
        Me.PanelPelanggan.Controls.Add(Me.TxtPelanggan)
        Me.PanelPelanggan.Controls.Add(Me.LblCariPelanggan)
        Me.PanelPelanggan.Location = New System.Drawing.Point(8, 48)
        Me.PanelPelanggan.Name = "PanelPelanggan"
        Me.PanelPelanggan.Size = New System.Drawing.Size(884, 60)
        Me.PanelPelanggan.TabIndex = 1
        '
        'LstHasilCariPelanggan
        '
        Me.LstHasilCariPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LstHasilCariPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LstHasilCariPelanggan.ItemHeight = 17
        Me.LstHasilCariPelanggan.Location = New System.Drawing.Point(122, 88)
        Me.LstHasilCariPelanggan.Name = "LstHasilCariPelanggan"
        Me.LstHasilCariPelanggan.Size = New System.Drawing.Size(280, 189)
        Me.LstHasilCariPelanggan.TabIndex = 5
        Me.LstHasilCariPelanggan.Visible = False
        '
        'LblSaldoPoinTukar
        '
        Me.LblSaldoPoinTukar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblSaldoPoinTukar.ForeColor = System.Drawing.Color.DarkBlue
        Me.LblSaldoPoinTukar.Location = New System.Drawing.Point(544, 14)
        Me.LblSaldoPoinTukar.Name = "LblSaldoPoinTukar"
        Me.LblSaldoPoinTukar.Size = New System.Drawing.Size(250, 24)
        Me.LblSaldoPoinTukar.TabIndex = 3
        Me.LblSaldoPoinTukar.Text = "Saldo Poin: -"
        Me.LblSaldoPoinTukar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblKodePelanggan
        '
        Me.LblKodePelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblKodePelanggan.ForeColor = System.Drawing.Color.DimGray
        Me.LblKodePelanggan.Location = New System.Drawing.Point(404, 14)
        Me.LblKodePelanggan.Name = "LblKodePelanggan"
        Me.LblKodePelanggan.Size = New System.Drawing.Size(130, 24)
        Me.LblKodePelanggan.TabIndex = 2
        Me.LblKodePelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtPelanggan
        '
        Me.TxtPelanggan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.TxtPelanggan.Location = New System.Drawing.Point(114, 14)
        Me.TxtPelanggan.Name = "TxtPelanggan"
        Me.TxtPelanggan.Size = New System.Drawing.Size(280, 25)
        Me.TxtPelanggan.TabIndex = 1
        '
        'LblCariPelanggan
        '
        Me.LblCariPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblCariPelanggan.Location = New System.Drawing.Point(8, 14)
        Me.LblCariPelanggan.Name = "LblCariPelanggan"
        Me.LblCariPelanggan.Size = New System.Drawing.Size(100, 24)
        Me.LblCariPelanggan.TabIndex = 0
        Me.LblCariPelanggan.Text = "Cari Pelanggan :"
        Me.LblCariPelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DgvBarangTukar
        '
        Me.DgvBarangTukar.AllowUserToAddRows = False
        Me.DgvBarangTukar.AllowUserToDeleteRows = False
        Me.DgvBarangTukar.AllowUserToResizeRows = False
        Me.DgvBarangTukar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvBarangTukar.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvBarangTukar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvBarangTukar.Location = New System.Drawing.Point(8, 108)
        Me.DgvBarangTukar.Name = "DgvBarangTukar"
        Me.DgvBarangTukar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvBarangTukar.Size = New System.Drawing.Size(884, 400)
        Me.DgvBarangTukar.TabIndex = 2
        '
        'PanelRingkasan
        '
        Me.PanelRingkasan.Controls.Add(Me.LblSummary)
        Me.PanelRingkasan.Controls.Add(Me.LblTotalPoinDibutuhkan)
        Me.PanelRingkasan.Controls.Add(Me.LblSisaPoinSetelah)
        Me.PanelRingkasan.Controls.Add(Me.BtnKonfirmasiTukar)
        Me.PanelRingkasan.Controls.Add(Me.BtnRefresh)
        Me.PanelRingkasan.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelRingkasan.Location = New System.Drawing.Point(0, 520)
        Me.PanelRingkasan.Name = "PanelRingkasan"
        Me.PanelRingkasan.Size = New System.Drawing.Size(900, 80)
        Me.PanelRingkasan.TabIndex = 3
        '
        'LblTotalPoinDibutuhkan
        '
        Me.LblTotalPoinDibutuhkan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblTotalPoinDibutuhkan.ForeColor = System.Drawing.Color.DarkSlateBlue
        Me.LblTotalPoinDibutuhkan.Location = New System.Drawing.Point(8, 8)
        Me.LblTotalPoinDibutuhkan.Name = "LblTotalPoinDibutuhkan"
        Me.LblTotalPoinDibutuhkan.Size = New System.Drawing.Size(280, 24)
        Me.LblTotalPoinDibutuhkan.TabIndex = 0
        Me.LblTotalPoinDibutuhkan.Text = "Total Poin Dibutuhkan: 0"
        Me.LblTotalPoinDibutuhkan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblSisaPoinSetelah
        '
        Me.LblSisaPoinSetelah.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LblSisaPoinSetelah.ForeColor = System.Drawing.Color.DarkGreen
        Me.LblSisaPoinSetelah.Location = New System.Drawing.Point(300, 8)
        Me.LblSisaPoinSetelah.Name = "LblSisaPoinSetelah"
        Me.LblSisaPoinSetelah.Size = New System.Drawing.Size(280, 24)
        Me.LblSisaPoinSetelah.TabIndex = 1
        Me.LblSisaPoinSetelah.Text = "Sisa Poin Setelah Tukar: 0"
        Me.LblSisaPoinSetelah.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnKonfirmasiTukar
        '
        Me.BtnKonfirmasiTukar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKonfirmasiTukar.BackColor = System.Drawing.Color.White
        Me.BtnKonfirmasiTukar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKonfirmasiTukar.Enabled = False
        Me.BtnKonfirmasiTukar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnKonfirmasiTukar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnKonfirmasiTukar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnKonfirmasiTukar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnKonfirmasiTukar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnKonfirmasiTukar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnKonfirmasiTukar.Location = New System.Drawing.Point(700, 12)
        Me.BtnKonfirmasiTukar.Name = "BtnKonfirmasiTukar"
        Me.BtnKonfirmasiTukar.Size = New System.Drawing.Size(160, 36)
        Me.BtnKonfirmasiTukar.TabIndex = 2
        Me.BtnKonfirmasiTukar.Text = "Konfirmasi Tukar (F2)"
        Me.BtnKonfirmasiTukar.UseVisualStyleBackColor = False
        '
        'BtnRefresh
        '
        Me.BtnRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnRefresh.BackColor = System.Drawing.Color.White
        Me.BtnRefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnRefresh.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnRefresh.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnRefresh.Location = New System.Drawing.Point(610, 12)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(80, 36)
        Me.BtnRefresh.TabIndex = 3
        Me.BtnRefresh.Text = "Refresh"
        Me.BtnRefresh.UseVisualStyleBackColor = False
        '
        'LblSummary
        '
        Me.LblSummary.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.LblSummary.ForeColor = System.Drawing.Color.DimGray
        Me.LblSummary.Location = New System.Drawing.Point(8, 48)
        Me.LblSummary.Name = "LblSummary"
        Me.LblSummary.Size = New System.Drawing.Size(580, 24)
        Me.LblSummary.TabIndex = 4
        Me.LblSummary.Text = "0 item | Total Qty: 0"
        Me.LblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TxtJenisTrans
        '
        Me.TxtJenisTrans.Location = New System.Drawing.Point(10, 10)
        Me.TxtJenisTrans.Name = "TxtJenisTrans"
        Me.TxtJenisTrans.Size = New System.Drawing.Size(10, 20)
        Me.TxtJenisTrans.TabIndex = 99
        Me.TxtJenisTrans.Visible = False
        '
        'TxtFaktur
        '
        Me.TxtFaktur.Location = New System.Drawing.Point(30, 10)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.Size = New System.Drawing.Size(10, 20)
        Me.TxtFaktur.TabIndex = 100
        Me.TxtFaktur.Visible = False
        '
        'FormTukarPoin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(900, 600)
        Me.Controls.Add(Me.TxtJenisTrans)
        Me.Controls.Add(Me.TxtFaktur)
        Me.Controls.Add(Me.PanelRingkasan)
        Me.Controls.Add(Me.PanelPelanggan)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.DgvBarangTukar)
        Me.Controls.Add(Me.LstHasilCariPelanggan)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormTukarPoin"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Tukar Poin dengan Barang"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelPelanggan.ResumeLayout(False)
        Me.PanelPelanggan.PerformLayout()
        CType(Me.DgvBarangTukar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelRingkasan.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblHeader As System.Windows.Forms.Label
    Friend WithEvents PanelPelanggan As System.Windows.Forms.Panel
    Friend WithEvents LblCariPelanggan As System.Windows.Forms.Label
    Friend WithEvents TxtPelanggan As System.Windows.Forms.TextBox
    Friend WithEvents LblKodePelanggan As System.Windows.Forms.Label
    Friend WithEvents LstHasilCariPelanggan As System.Windows.Forms.ListBox
    Friend WithEvents LblSummary As System.Windows.Forms.Label
    Friend WithEvents LblSaldoPoinTukar As System.Windows.Forms.Label
    Friend WithEvents DgvBarangTukar As System.Windows.Forms.DataGridView
    Friend WithEvents PanelRingkasan As System.Windows.Forms.Panel
    Friend WithEvents LblTotalPoinDibutuhkan As System.Windows.Forms.Label
    Friend WithEvents LblSisaPoinSetelah As System.Windows.Forms.Label
    Friend WithEvents BtnKonfirmasiTukar As System.Windows.Forms.Button
    Friend WithEvents BtnRefresh As System.Windows.Forms.Button
    Friend WithEvents TxtJenisTrans As System.Windows.Forms.TextBox
    Friend WithEvents TxtFaktur As System.Windows.Forms.TextBox

End Class
