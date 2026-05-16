<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormKeuangan
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormKeuangan))
        Me.BtnPinjamSuplier = New System.Windows.Forms.Button()
        Me.BtnSetorBos = New System.Windows.Forms.Button()
        Me.BtnPindahR = New System.Windows.Forms.Button()
        Me.BtnPengeluaran = New System.Windows.Forms.Button()
        Me.BtnPemasukan = New System.Windows.Forms.Button()
        Me.PanelInput = New System.Windows.Forms.Panel()
        Me.LblBantuKKeuangan = New System.Windows.Forms.Label()
        Me.TxtDebetKeuanganNama = New System.Windows.Forms.TextBox()
        Me.LblBantuDKeuangan = New System.Windows.Forms.Label()
        Me.TxtKreditKeuanganNama = New System.Windows.Forms.TextBox()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.TxtBantuDKeuanganNama = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.TxtBantuKKeuanganNama = New System.Windows.Forms.TextBox()
        Me.CmbBantuKKeuangan = New System.Windows.Forms.ComboBox()
        Me.TxtDebetKeuangan = New System.Windows.Forms.TextBox()
        Me.TxtKreditKeuangan = New System.Windows.Forms.TextBox()
        Me.CmbBantuDKeuangan = New System.Windows.Forms.ComboBox()
        Me.CmbKreditKeuangan = New System.Windows.Forms.ComboBox()
        Me.CmbDebetKeuangan = New System.Windows.Forms.ComboBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.LblIdBayar = New System.Windows.Forms.Label()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.BtnBatalKeuangan = New System.Windows.Forms.Button()
        Me.BtnSimpanKeuangan = New System.Windows.Forms.Button()
        Me.TxtBantuDKeuangan = New System.Windows.Forms.TextBox()
        Me.TxtBantuKKeuangan = New System.Windows.Forms.TextBox()
        Me.LblNominalKeuangan = New System.Windows.Forms.Label()
        Me.Label73 = New System.Windows.Forms.Label()
        Me.TxtNoNota = New System.Windows.Forms.TextBox()
        Me.LblNamaTransaksi = New System.Windows.Forms.Label()
        Me.TxtNominalKeuangan = New System.Windows.Forms.TextBox()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.DTPTglKeuangan = New System.Windows.Forms.DateTimePicker()
        Me.Label70 = New System.Windows.Forms.Label()
        Me.TxtUraianKeuangan = New System.Windows.Forms.TextBox()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.BTNKeluar = New System.Windows.Forms.Button()
        Me.PanelRinciKeuangan = New System.Windows.Forms.Panel()
        Me.LblTotalNominal = New System.Windows.Forms.Label()
        Me.DgvKeuangan = New System.Windows.Forms.DataGridView()
        Me.LblRinciPengeluaran = New System.Windows.Forms.Label()
        Me.PanelUtility = New System.Windows.Forms.Panel()
        Me.BtnPinjamPelanggan = New System.Windows.Forms.Button()
        Me.BtnBiaya = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.PanelInput.SuspendLayout()
        Me.PanelRinciKeuangan.SuspendLayout()
        CType(Me.DgvKeuangan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelUtility.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnPinjamSuplier
        '
        Me.BtnPinjamSuplier.AutoSize = True
        Me.BtnPinjamSuplier.BackColor = System.Drawing.SystemColors.Control
        Me.BtnPinjamSuplier.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPinjamSuplier.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPinjamSuplier.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPinjamSuplier.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPinjamSuplier.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPinjamSuplier.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPinjamSuplier.ForeColor = System.Drawing.Color.Black
        Me.BtnPinjamSuplier.Image = CType(resources.GetObject("BtnPinjamSuplier.Image"), System.Drawing.Image)
        Me.BtnPinjamSuplier.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinjamSuplier.Location = New System.Drawing.Point(802, 2)
        Me.BtnPinjamSuplier.Name = "BtnPinjamSuplier"
        Me.BtnPinjamSuplier.Size = New System.Drawing.Size(181, 34)
        Me.BtnPinjamSuplier.TabIndex = 202
        Me.BtnPinjamSuplier.Text = "Pinjaman Supplier (F7)"
        Me.BtnPinjamSuplier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinjamSuplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPinjamSuplier.UseVisualStyleBackColor = False
        '
        'BtnSetorBos
        '
        Me.BtnSetorBos.AutoSize = True
        Me.BtnSetorBos.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSetorBos.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSetorBos.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnSetorBos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSetorBos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSetorBos.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSetorBos.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSetorBos.ForeColor = System.Drawing.Color.Black
        Me.BtnSetorBos.Image = CType(resources.GetObject("BtnSetorBos.Image"), System.Drawing.Image)
        Me.BtnSetorBos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetorBos.Location = New System.Drawing.Point(632, 2)
        Me.BtnSetorBos.Name = "BtnSetorBos"
        Me.BtnSetorBos.Size = New System.Drawing.Size(144, 34)
        Me.BtnSetorBos.TabIndex = 201
        Me.BtnSetorBos.Text = "Setor ke Bos (F6)"
        Me.BtnSetorBos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetorBos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSetorBos.UseVisualStyleBackColor = False
        '
        'BtnPindahR
        '
        Me.BtnPindahR.AutoSize = True
        Me.BtnPindahR.BackColor = System.Drawing.SystemColors.Control
        Me.BtnPindahR.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPindahR.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPindahR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPindahR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPindahR.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPindahR.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPindahR.ForeColor = System.Drawing.Color.Black
        Me.BtnPindahR.Image = CType(resources.GetObject("BtnPindahR.Image"), System.Drawing.Image)
        Me.BtnPindahR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPindahR.Location = New System.Drawing.Point(437, 2)
        Me.BtnPindahR.Name = "BtnPindahR"
        Me.BtnPindahR.Size = New System.Drawing.Size(172, 34)
        Me.BtnPindahR.TabIndex = 200
        Me.BtnPindahR.Text = "Pindah Rekening (F5)"
        Me.BtnPindahR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPindahR.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPindahR.UseVisualStyleBackColor = False
        '
        'BtnPengeluaran
        '
        Me.BtnPengeluaran.AutoSize = True
        Me.BtnPengeluaran.BackColor = System.Drawing.SystemColors.Control
        Me.BtnPengeluaran.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPengeluaran.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPengeluaran.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPengeluaran.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPengeluaran.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPengeluaran.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPengeluaran.ForeColor = System.Drawing.Color.Black
        Me.BtnPengeluaran.Image = CType(resources.GetObject("BtnPengeluaran.Image"), System.Drawing.Image)
        Me.BtnPengeluaran.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPengeluaran.Location = New System.Drawing.Point(161, 2)
        Me.BtnPengeluaran.Name = "BtnPengeluaran"
        Me.BtnPengeluaran.Size = New System.Drawing.Size(151, 34)
        Me.BtnPengeluaran.TabIndex = 199
        Me.BtnPengeluaran.Text = "Pengeluaran (F3)"
        Me.BtnPengeluaran.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPengeluaran.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPengeluaran.UseVisualStyleBackColor = False
        '
        'BtnPemasukan
        '
        Me.BtnPemasukan.AutoSize = True
        Me.BtnPemasukan.BackColor = System.Drawing.SystemColors.Control
        Me.BtnPemasukan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPemasukan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPemasukan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPemasukan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPemasukan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPemasukan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPemasukan.ForeColor = System.Drawing.Color.Black
        Me.BtnPemasukan.Image = CType(resources.GetObject("BtnPemasukan.Image"), System.Drawing.Image)
        Me.BtnPemasukan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPemasukan.Location = New System.Drawing.Point(14, 2)
        Me.BtnPemasukan.Name = "BtnPemasukan"
        Me.BtnPemasukan.Size = New System.Drawing.Size(138, 34)
        Me.BtnPemasukan.TabIndex = 198
        Me.BtnPemasukan.Text = "Pemasukan (F2)"
        Me.BtnPemasukan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPemasukan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPemasukan.UseVisualStyleBackColor = False
        '
        'PanelPemasukan
        '
        Me.PanelInput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PanelInput.BackColor = System.Drawing.SystemColors.Control
        Me.PanelInput.Controls.Add(Me.LblBantuKKeuangan)
        Me.PanelInput.Controls.Add(Me.TxtDebetKeuanganNama)
        Me.PanelInput.Controls.Add(Me.LblBantuDKeuangan)
        Me.PanelInput.Controls.Add(Me.TxtKreditKeuanganNama)
        Me.PanelInput.Controls.Add(Me.Label60)
        Me.PanelInput.Controls.Add(Me.TxtBantuDKeuanganNama)
        Me.PanelInput.Controls.Add(Me.Label53)
        Me.PanelInput.Controls.Add(Me.TxtBantuKKeuanganNama)
        Me.PanelInput.Controls.Add(Me.CmbBantuKKeuangan)
        Me.PanelInput.Controls.Add(Me.TxtDebetKeuangan)
        Me.PanelInput.Controls.Add(Me.TxtKreditKeuangan)
        Me.PanelInput.Controls.Add(Me.CmbBantuDKeuangan)
        Me.PanelInput.Controls.Add(Me.CmbKreditKeuangan)
        Me.PanelInput.Controls.Add(Me.CmbDebetKeuangan)
        Me.PanelInput.Controls.Add(Me.Label45)
        Me.PanelInput.Controls.Add(Me.LblIdBayar)
        Me.PanelInput.Controls.Add(Me.Label42)
        Me.PanelInput.Controls.Add(Me.BtnBatalKeuangan)
        Me.PanelInput.Controls.Add(Me.BtnSimpanKeuangan)
        Me.PanelInput.Controls.Add(Me.TxtBantuDKeuangan)
        Me.PanelInput.Controls.Add(Me.TxtBantuKKeuangan)
        Me.PanelInput.Controls.Add(Me.LblNominalKeuangan)
        Me.PanelInput.Controls.Add(Me.Label73)
        Me.PanelInput.Controls.Add(Me.TxtNoNota)
        Me.PanelInput.Controls.Add(Me.LblNamaTransaksi)
        Me.PanelInput.Controls.Add(Me.TxtNominalKeuangan)
        Me.PanelInput.Controls.Add(Me.Label71)
        Me.PanelInput.Controls.Add(Me.DTPTglKeuangan)
        Me.PanelInput.Controls.Add(Me.Label70)
        Me.PanelInput.Controls.Add(Me.TxtUraianKeuangan)
        Me.PanelInput.Controls.Add(Me.Label69)
        Me.PanelInput.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelInput.Location = New System.Drawing.Point(12, 55)
        Me.PanelInput.Name = "PanelInput"
        Me.PanelInput.Size = New System.Drawing.Size(390, 527)
        Me.PanelInput.TabIndex = 203
        '
        'LblBantuKKeuangan
        '
        Me.LblBantuKKeuangan.AutoSize = True
        Me.LblBantuKKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBantuKKeuangan.Location = New System.Drawing.Point(10, 272)
        Me.LblBantuKKeuangan.Name = "LblBantuKKeuangan"
        Me.LblBantuKKeuangan.Size = New System.Drawing.Size(58, 17)
        Me.LblBantuKKeuangan.TabIndex = 258
        Me.LblBantuKKeuangan.Text = "Bantu K :"
        '
        'TxtDebetKeuanganNama
        '
        Me.TxtDebetKeuanganNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDebetKeuanganNama.Location = New System.Drawing.Point(338, 411)
        Me.TxtDebetKeuanganNama.Name = "TxtDebetKeuanganNama"
        Me.TxtDebetKeuanganNama.Size = New System.Drawing.Size(39, 23)
        Me.TxtDebetKeuanganNama.TabIndex = 261
        Me.TxtDebetKeuanganNama.Visible = False
        '
        'LblBantuDKeuangan
        '
        Me.LblBantuDKeuangan.AutoSize = True
        Me.LblBantuDKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBantuDKeuangan.Location = New System.Drawing.Point(8, 241)
        Me.LblBantuDKeuangan.Name = "LblBantuDKeuangan"
        Me.LblBantuDKeuangan.Size = New System.Drawing.Size(60, 17)
        Me.LblBantuDKeuangan.TabIndex = 257
        Me.LblBantuDKeuangan.Text = "Bantu D :"
        '
        'TxtKreditKeuanganNama
        '
        Me.TxtKreditKeuanganNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKreditKeuanganNama.Location = New System.Drawing.Point(338, 440)
        Me.TxtKreditKeuanganNama.Name = "TxtKreditKeuanganNama"
        Me.TxtKreditKeuanganNama.Size = New System.Drawing.Size(39, 23)
        Me.TxtKreditKeuanganNama.TabIndex = 262
        Me.TxtKreditKeuanganNama.Visible = False
        '
        'Label60
        '
        Me.Label60.AutoSize = True
        Me.Label60.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label60.Location = New System.Drawing.Point(9, 209)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(59, 17)
        Me.Label60.TabIndex = 256
        Me.Label60.Text = "R Kredit :"
        '
        'TxtBantuDKeuanganNama
        '
        Me.TxtBantuDKeuanganNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBantuDKeuanganNama.Location = New System.Drawing.Point(338, 469)
        Me.TxtBantuDKeuanganNama.Name = "TxtBantuDKeuanganNama"
        Me.TxtBantuDKeuanganNama.Size = New System.Drawing.Size(39, 23)
        Me.TxtBantuDKeuanganNama.TabIndex = 263
        Me.TxtBantuDKeuanganNama.Visible = False
        '
        'Label53
        '
        Me.Label53.AutoSize = True
        Me.Label53.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label53.Location = New System.Drawing.Point(6, 179)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(62, 17)
        Me.Label53.TabIndex = 255
        Me.Label53.Text = "R Debet :"
        '
        'TxtBantuKKeuanganNama
        '
        Me.TxtBantuKKeuanganNama.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBantuKKeuanganNama.Location = New System.Drawing.Point(338, 498)
        Me.TxtBantuKKeuanganNama.Name = "TxtBantuKKeuanganNama"
        Me.TxtBantuKKeuanganNama.Size = New System.Drawing.Size(39, 23)
        Me.TxtBantuKKeuanganNama.TabIndex = 264
        Me.TxtBantuKKeuanganNama.Visible = False
        '
        'CmbBantuKKeuangan
        '
        Me.CmbBantuKKeuangan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBantuKKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBantuKKeuangan.FormattingEnabled = True
        Me.CmbBantuKKeuangan.Location = New System.Drawing.Point(70, 268)
        Me.CmbBantuKKeuangan.Name = "CmbBantuKKeuangan"
        Me.CmbBantuKKeuangan.Size = New System.Drawing.Size(307, 25)
        Me.CmbBantuKKeuangan.TabIndex = 253
        '
        'TxtDebetKeuangan
        '
        Me.TxtDebetKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDebetKeuangan.Location = New System.Drawing.Point(338, 353)
        Me.TxtDebetKeuangan.Name = "TxtDebetKeuangan"
        Me.TxtDebetKeuangan.Size = New System.Drawing.Size(39, 23)
        Me.TxtDebetKeuangan.TabIndex = 259
        Me.TxtDebetKeuangan.Visible = False
        '
        'TxtKreditKeuangan
        '
        Me.TxtKreditKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKreditKeuangan.Location = New System.Drawing.Point(338, 382)
        Me.TxtKreditKeuangan.Name = "TxtKreditKeuangan"
        Me.TxtKreditKeuangan.Size = New System.Drawing.Size(39, 23)
        Me.TxtKreditKeuangan.TabIndex = 260
        Me.TxtKreditKeuangan.Visible = False
        '
        'CmbBantuDKeuangan
        '
        Me.CmbBantuDKeuangan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBantuDKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbBantuDKeuangan.FormattingEnabled = True
        Me.CmbBantuDKeuangan.Location = New System.Drawing.Point(70, 237)
        Me.CmbBantuDKeuangan.Name = "CmbBantuDKeuangan"
        Me.CmbBantuDKeuangan.Size = New System.Drawing.Size(307, 25)
        Me.CmbBantuDKeuangan.TabIndex = 251
        '
        'CmbKreditKeuangan
        '
        Me.CmbKreditKeuangan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbKreditKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbKreditKeuangan.FormattingEnabled = True
        Me.CmbKreditKeuangan.Location = New System.Drawing.Point(70, 205)
        Me.CmbKreditKeuangan.Name = "CmbKreditKeuangan"
        Me.CmbKreditKeuangan.Size = New System.Drawing.Size(307, 25)
        Me.CmbKreditKeuangan.TabIndex = 249
        '
        'CmbDebetKeuangan
        '
        Me.CmbDebetKeuangan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbDebetKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbDebetKeuangan.FormattingEnabled = True
        Me.CmbDebetKeuangan.Location = New System.Drawing.Point(70, 175)
        Me.CmbDebetKeuangan.Name = "CmbDebetKeuangan"
        Me.CmbDebetKeuangan.Size = New System.Drawing.Size(307, 25)
        Me.CmbDebetKeuangan.TabIndex = 247
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label45.Location = New System.Drawing.Point(170, 96)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(131, 17)
        Me.Label45.TabIndex = 200
        Me.Label45.Text = "* di isi hanya jika ada"
        '
        'LblIdBayar
        '
        Me.LblIdBayar.AutoSize = True
        Me.LblIdBayar.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblIdBayar.Location = New System.Drawing.Point(70, 72)
        Me.LblIdBayar.Name = "LblIdBayar"
        Me.LblIdBayar.Size = New System.Drawing.Size(58, 17)
        Me.LblIdBayar.TabIndex = 199
        Me.LblIdBayar.Text = "ID Trans :"
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label42.Location = New System.Drawing.Point(10, 72)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(58, 17)
        Me.Label42.TabIndex = 198
        Me.Label42.Text = "ID Trans :"
        '
        'BtnBatalKeuangan
        '
        Me.BtnBatalKeuangan.AutoSize = True
        Me.BtnBatalKeuangan.BackColor = System.Drawing.SystemColors.Control
        Me.BtnBatalKeuangan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBatalKeuangan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnBatalKeuangan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnBatalKeuangan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnBatalKeuangan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBatalKeuangan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBatalKeuangan.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnBatalKeuangan.Image = CType(resources.GetObject("BtnBatalKeuangan.Image"), System.Drawing.Image)
        Me.BtnBatalKeuangan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatalKeuangan.Location = New System.Drawing.Point(70, 369)
        Me.BtnBatalKeuangan.Name = "BtnBatalKeuangan"
        Me.BtnBatalKeuangan.Size = New System.Drawing.Size(140, 32)
        Me.BtnBatalKeuangan.TabIndex = 196
        Me.BtnBatalKeuangan.Text = "Baru (F9)"
        Me.BtnBatalKeuangan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatalKeuangan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBatalKeuangan.UseVisualStyleBackColor = False
        '
        'BtnSimpanKeuangan
        '
        Me.BtnSimpanKeuangan.AutoSize = True
        Me.BtnSimpanKeuangan.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSimpanKeuangan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSimpanKeuangan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanKeuangan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpanKeuangan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpanKeuangan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpanKeuangan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpanKeuangan.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpanKeuangan.Image = CType(resources.GetObject("BtnSimpanKeuangan.Image"), System.Drawing.Image)
        Me.BtnSimpanKeuangan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanKeuangan.Location = New System.Drawing.Point(70, 331)
        Me.BtnSimpanKeuangan.Name = "BtnSimpanKeuangan"
        Me.BtnSimpanKeuangan.Size = New System.Drawing.Size(140, 32)
        Me.BtnSimpanKeuangan.TabIndex = 192
        Me.BtnSimpanKeuangan.Text = "Simpan (F8)"
        Me.BtnSimpanKeuangan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanKeuangan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpanKeuangan.UseVisualStyleBackColor = False
        '
        'TxtBantuDKeuangan
        '
        Me.TxtBantuDKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBantuDKeuangan.Location = New System.Drawing.Point(338, 297)
        Me.TxtBantuDKeuangan.Name = "TxtBantuDKeuangan"
        Me.TxtBantuDKeuangan.Size = New System.Drawing.Size(39, 22)
        Me.TxtBantuDKeuangan.TabIndex = 252
        Me.TxtBantuDKeuangan.Visible = False
        '
        'TxtBantuKKeuangan
        '
        Me.TxtBantuKKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBantuKKeuangan.Location = New System.Drawing.Point(338, 325)
        Me.TxtBantuKKeuangan.Name = "TxtBantuKKeuangan"
        Me.TxtBantuKKeuangan.Size = New System.Drawing.Size(39, 22)
        Me.TxtBantuKKeuangan.TabIndex = 254
        Me.TxtBantuKKeuangan.Visible = False
        '
        'LblNominalKeuangan
        '
        Me.LblNominalKeuangan.AutoSize = True
        Me.LblNominalKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNominalKeuangan.Location = New System.Drawing.Point(227, 302)
        Me.LblNominalKeuangan.Name = "LblNominalKeuangan"
        Me.LblNominalKeuangan.Size = New System.Drawing.Size(36, 17)
        Me.LblNominalKeuangan.TabIndex = 189
        Me.LblNominalKeuangan.Text = "Rp. 0"
        '
        'Label73
        '
        Me.Label73.AutoSize = True
        Me.Label73.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label73.Location = New System.Drawing.Point(4, 96)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(64, 17)
        Me.Label73.TabIndex = 188
        Me.Label73.Text = "No Nota :"
        '
        'TxtNoNota
        '
        Me.TxtNoNota.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNoNota.Location = New System.Drawing.Point(70, 93)
        Me.TxtNoNota.Name = "TxtNoNota"
        Me.TxtNoNota.Size = New System.Drawing.Size(94, 22)
        Me.TxtNoNota.TabIndex = 187
        '
        'LblNamaTransaksi
        '
        Me.LblNamaTransaksi.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNamaTransaksi.Location = New System.Drawing.Point(8, 9)
        Me.LblNamaTransaksi.Name = "LblNamaTransaksi"
        Me.LblNamaTransaksi.Size = New System.Drawing.Size(298, 24)
        Me.LblNamaTransaksi.TabIndex = 186
        Me.LblNamaTransaksi.Text = "NAMA TRANSAKSI"
        Me.LblNamaTransaksi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtNominalKeuangan
        '
        Me.TxtNominalKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNominalKeuangan.Location = New System.Drawing.Point(70, 299)
        Me.TxtNominalKeuangan.Name = "TxtNominalKeuangan"
        Me.TxtNominalKeuangan.Size = New System.Drawing.Size(142, 22)
        Me.TxtNominalKeuangan.TabIndex = 184
        '
        'Label71
        '
        Me.Label71.AutoSize = True
        Me.Label71.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label71.Location = New System.Drawing.Point(5, 302)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(63, 17)
        Me.Label71.TabIndex = 185
        Me.Label71.Text = "Nominal :"
        '
        'DTPTglKeuangan
        '
        Me.DTPTglKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTglKeuangan.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPTglKeuangan.Location = New System.Drawing.Point(70, 47)
        Me.DTPTglKeuangan.Name = "DTPTglKeuangan"
        Me.DTPTglKeuangan.Size = New System.Drawing.Size(117, 22)
        Me.DTPTglKeuangan.TabIndex = 182
        '
        'Label70
        '
        Me.Label70.AutoSize = True
        Me.Label70.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label70.Location = New System.Drawing.Point(7, 50)
        Me.Label70.Name = "Label70"
        Me.Label70.Size = New System.Drawing.Size(61, 17)
        Me.Label70.TabIndex = 183
        Me.Label70.Text = "Tanggal :"
        '
        'TxtUraianKeuangan
        '
        Me.TxtUraianKeuangan.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtUraianKeuangan.Location = New System.Drawing.Point(70, 121)
        Me.TxtUraianKeuangan.Multiline = True
        Me.TxtUraianKeuangan.Name = "TxtUraianKeuangan"
        Me.TxtUraianKeuangan.Size = New System.Drawing.Size(307, 48)
        Me.TxtUraianKeuangan.TabIndex = 178
        '
        'Label69
        '
        Me.Label69.AutoSize = True
        Me.Label69.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label69.Location = New System.Drawing.Point(16, 121)
        Me.Label69.Name = "Label69"
        Me.Label69.Size = New System.Drawing.Size(52, 17)
        Me.Label69.TabIndex = 179
        Me.Label69.Text = "Uraian :"
        '
        'BTNKeluar
        '
        Me.BTNKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BTNKeluar.AutoSize = True
        Me.BTNKeluar.BackColor = System.Drawing.SystemColors.Control
        Me.BTNKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BTNKeluar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BTNKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BTNKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BTNKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BTNKeluar.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BTNKeluar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BTNKeluar.Image = CType(resources.GetObject("BTNKeluar.Image"), System.Drawing.Image)
        Me.BTNKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BTNKeluar.Location = New System.Drawing.Point(1612, 4)
        Me.BTNKeluar.Name = "BTNKeluar"
        Me.BTNKeluar.Size = New System.Drawing.Size(112, 32)
        Me.BTNKeluar.TabIndex = 265
        Me.BTNKeluar.Text = "Keluar (Esc)"
        Me.BTNKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BTNKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BTNKeluar.UseVisualStyleBackColor = False
        '
        'PanelRinciKeuangan
        '
        Me.PanelRinciKeuangan.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelRinciKeuangan.BackColor = System.Drawing.SystemColors.Control
        Me.PanelRinciKeuangan.Controls.Add(Me.LblTotalNominal)
        Me.PanelRinciKeuangan.Controls.Add(Me.DgvKeuangan)
        Me.PanelRinciKeuangan.Controls.Add(Me.LblRinciPengeluaran)
        Me.PanelRinciKeuangan.Location = New System.Drawing.Point(408, 55)
        Me.PanelRinciKeuangan.Name = "PanelRinciKeuangan"
        Me.PanelRinciKeuangan.Size = New System.Drawing.Size(1317, 527)
        Me.PanelRinciKeuangan.TabIndex = 265
        '
        'LblTotalNominal
        '
        Me.LblTotalNominal.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalNominal.Location = New System.Drawing.Point(340, 7)
        Me.LblTotalNominal.Name = "LblTotalNominal"
        Me.LblTotalNominal.Size = New System.Drawing.Size(388, 26)
        Me.LblTotalNominal.TabIndex = 207
        Me.LblTotalNominal.Text = "No Nota"
        Me.LblTotalNominal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DgvKeuangan
        '
        Me.DgvKeuangan.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvKeuangan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvKeuangan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvKeuangan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvKeuangan.Location = New System.Drawing.Point(0, 36)
        Me.DgvKeuangan.Name = "DgvKeuangan"
        Me.DgvKeuangan.ReadOnly = True
        Me.DgvKeuangan.RowHeadersVisible = False
        Me.DgvKeuangan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvKeuangan.Size = New System.Drawing.Size(1314, 488)
        Me.DgvKeuangan.TabIndex = 205
        '
        'LblRinciPengeluaran
        '
        Me.LblRinciPengeluaran.AutoSize = True
        Me.LblRinciPengeluaran.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRinciPengeluaran.Location = New System.Drawing.Point(3, 9)
        Me.LblRinciPengeluaran.Name = "LblRinciPengeluaran"
        Me.LblRinciPengeluaran.Size = New System.Drawing.Size(186, 20)
        Me.LblRinciPengeluaran.TabIndex = 204
        Me.LblRinciPengeluaran.Text = "RINCIAN PENGELUARAN"
        '
        'PanelUtility
        '
        Me.PanelUtility.BackColor = System.Drawing.SystemColors.Control
        Me.PanelUtility.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelUtility.Controls.Add(Me.BtnPinjamPelanggan)
        Me.PanelUtility.Controls.Add(Me.BTNKeluar)
        Me.PanelUtility.Controls.Add(Me.BtnBiaya)
        Me.PanelUtility.Controls.Add(Me.BtnPengeluaran)
        Me.PanelUtility.Controls.Add(Me.BtnPemasukan)
        Me.PanelUtility.Controls.Add(Me.BtnPindahR)
        Me.PanelUtility.Controls.Add(Me.BtnSetorBos)
        Me.PanelUtility.Controls.Add(Me.BtnPinjamSuplier)
        Me.PanelUtility.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelUtility.Location = New System.Drawing.Point(0, 0)
        Me.PanelUtility.Name = "PanelUtility"
        Me.PanelUtility.Size = New System.Drawing.Size(1737, 40)
        Me.PanelUtility.TabIndex = 267
        '
        'BtnPinjamPelanggan
        '
        Me.BtnPinjamPelanggan.AutoSize = True
        Me.BtnPinjamPelanggan.BackColor = System.Drawing.SystemColors.Control
        Me.BtnPinjamPelanggan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnPinjamPelanggan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnPinjamPelanggan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnPinjamPelanggan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnPinjamPelanggan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPinjamPelanggan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPinjamPelanggan.ForeColor = System.Drawing.Color.Black
        Me.BtnPinjamPelanggan.Image = CType(resources.GetObject("BtnPinjamPelanggan.Image"), System.Drawing.Image)
        Me.BtnPinjamPelanggan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinjamPelanggan.Location = New System.Drawing.Point(997, 2)
        Me.BtnPinjamPelanggan.Name = "BtnPinjamPelanggan"
        Me.BtnPinjamPelanggan.Size = New System.Drawing.Size(202, 34)
        Me.BtnPinjamPelanggan.TabIndex = 266
        Me.BtnPinjamPelanggan.Text = "Pinjaman Pelanggan (F10)"
        Me.BtnPinjamPelanggan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPinjamPelanggan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPinjamPelanggan.UseVisualStyleBackColor = False
        '
        'BtnBiaya
        '
        Me.BtnBiaya.AutoSize = True
        Me.BtnBiaya.BackColor = System.Drawing.SystemColors.Control
        Me.BtnBiaya.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnBiaya.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnBiaya.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnBiaya.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnBiaya.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBiaya.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBiaya.ForeColor = System.Drawing.Color.Black
        Me.BtnBiaya.Image = CType(resources.GetObject("BtnBiaya.Image"), System.Drawing.Image)
        Me.BtnBiaya.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBiaya.Location = New System.Drawing.Point(318, 2)
        Me.BtnBiaya.Name = "BtnBiaya"
        Me.BtnBiaya.Size = New System.Drawing.Size(101, 34)
        Me.BtnBiaya.TabIndex = 203
        Me.BtnBiaya.Text = "Biaya (F4)"
        Me.BtnBiaya.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBiaya.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBiaya.UseVisualStyleBackColor = False
        '
        'FormKeuangan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1737, 594)
        Me.Controls.Add(Me.PanelUtility)
        Me.Controls.Add(Me.PanelRinciKeuangan)
        Me.Controls.Add(Me.PanelInput)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormKeuangan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormKeuangan"
        Me.PanelInput.ResumeLayout(False)
        Me.PanelInput.PerformLayout()
        Me.PanelRinciKeuangan.ResumeLayout(False)
        Me.PanelRinciKeuangan.PerformLayout()
        CType(Me.DgvKeuangan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelUtility.ResumeLayout(False)
        Me.PanelUtility.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnPinjamSuplier As System.Windows.Forms.Button
    Friend WithEvents BtnSetorBos As System.Windows.Forms.Button
    Friend WithEvents BtnPindahR As System.Windows.Forms.Button
    Friend WithEvents BtnPengeluaran As System.Windows.Forms.Button
    Friend WithEvents BtnPemasukan As System.Windows.Forms.Button
    Friend WithEvents PanelInput As System.Windows.Forms.Panel
    Friend WithEvents LblBantuKKeuangan As System.Windows.Forms.Label
    Friend WithEvents LblBantuDKeuangan As System.Windows.Forms.Label
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents CmbBantuKKeuangan As System.Windows.Forms.ComboBox
    Friend WithEvents CmbBantuDKeuangan As System.Windows.Forms.ComboBox
    Friend WithEvents CmbKreditKeuangan As System.Windows.Forms.ComboBox
    Friend WithEvents CmbDebetKeuangan As System.Windows.Forms.ComboBox
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents LblIdBayar As System.Windows.Forms.Label
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents BtnBatalKeuangan As System.Windows.Forms.Button
    Friend WithEvents BtnSimpanKeuangan As System.Windows.Forms.Button
    Friend WithEvents TxtBantuDKeuangan As System.Windows.Forms.TextBox
    Friend WithEvents TxtBantuKKeuangan As System.Windows.Forms.TextBox
    Friend WithEvents LblNominalKeuangan As System.Windows.Forms.Label
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents TxtNoNota As System.Windows.Forms.TextBox
    Friend WithEvents LblNamaTransaksi As System.Windows.Forms.Label
    Friend WithEvents TxtNominalKeuangan As System.Windows.Forms.TextBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents DTPTglKeuangan As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents TxtUraianKeuangan As System.Windows.Forms.TextBox
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents TxtDebetKeuanganNama As System.Windows.Forms.TextBox
    Friend WithEvents TxtKreditKeuanganNama As System.Windows.Forms.TextBox
    Friend WithEvents TxtBantuDKeuanganNama As System.Windows.Forms.TextBox
    Friend WithEvents TxtBantuKKeuanganNama As System.Windows.Forms.TextBox
    Friend WithEvents TxtDebetKeuangan As System.Windows.Forms.TextBox
    Friend WithEvents TxtKreditKeuangan As System.Windows.Forms.TextBox
    Friend WithEvents PanelRinciKeuangan As System.Windows.Forms.Panel
    Friend WithEvents LblTotalNominal As System.Windows.Forms.Label
    Friend WithEvents DgvKeuangan As System.Windows.Forms.DataGridView
    Friend WithEvents LblRinciPengeluaran As System.Windows.Forms.Label
    Friend WithEvents BTNKeluar As System.Windows.Forms.Button
    Friend WithEvents PanelUtility As System.Windows.Forms.Panel
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents BtnBiaya As Button
    Friend WithEvents BtnPinjamPelanggan As Button
End Class

