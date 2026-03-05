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
        Me.BtnBayarBon = New System.Windows.Forms.Button()
        Me.BtnSetorBos = New System.Windows.Forms.Button()
        Me.BtnPindahR = New System.Windows.Forms.Button()
        Me.BtnPengeluaran = New System.Windows.Forms.Button()
        Me.BtnPemasukan = New System.Windows.Forms.Button()
        Me.PanelPemasukan = New System.Windows.Forms.Panel()
        Me.BTNKeluar = New System.Windows.Forms.Button()
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
        Me.BtnEditKeuangan = New System.Windows.Forms.Button()
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
        Me.PanelRinciKeuangan = New System.Windows.Forms.Panel()
        Me.LblTotalNominal = New System.Windows.Forms.Label()
        Me.DgvKeuangan = New System.Windows.Forms.DataGridView()
        Me.LblRinciPengeluaran = New System.Windows.Forms.Label()
        Me.BtnBiaya = New System.Windows.Forms.Button()
        Me.PanelUtility = New System.Windows.Forms.Panel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.PanelPemasukan.SuspendLayout()
        Me.PanelRinciKeuangan.SuspendLayout()
        CType(Me.DgvKeuangan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelUtility.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnBayarBon
        '
        Me.BtnBayarBon.AutoSize = True
        Me.BtnBayarBon.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnBayarBon.FlatAppearance.BorderSize = 0
        Me.BtnBayarBon.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnBayarBon.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnBayarBon.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnBayarBon.ForeColor = System.Drawing.Color.Black
        Me.BtnBayarBon.Image = CType(resources.GetObject("BtnBayarBon.Image"), System.Drawing.Image)
        Me.BtnBayarBon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBayarBon.Location = New System.Drawing.Point(714, 2)
        Me.BtnBayarBon.Name = "BtnBayarBon"
        Me.BtnBayarBon.Size = New System.Drawing.Size(132, 34)
        Me.BtnBayarBon.TabIndex = 202
        Me.BtnBayarBon.Text = "BAYAR BON"
        Me.BtnBayarBon.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBayarBon.UseVisualStyleBackColor = False
        Me.BtnBayarBon.Visible = False
        '
        'BtnSetorBos
        '
        Me.BtnSetorBos.AutoSize = True
        Me.BtnSetorBos.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnSetorBos.FlatAppearance.BorderSize = 0
        Me.BtnSetorBos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnSetorBos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnSetorBos.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnSetorBos.ForeColor = System.Drawing.Color.Black
        Me.BtnSetorBos.Image = CType(resources.GetObject("BtnSetorBos.Image"), System.Drawing.Image)
        Me.BtnSetorBos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSetorBos.Location = New System.Drawing.Point(572, 2)
        Me.BtnSetorBos.Name = "BtnSetorBos"
        Me.BtnSetorBos.Size = New System.Drawing.Size(141, 34)
        Me.BtnSetorBos.TabIndex = 201
        Me.BtnSetorBos.Text = "SETOR KE BOS"
        Me.BtnSetorBos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSetorBos.UseVisualStyleBackColor = False
        '
        'BtnPindahR
        '
        Me.BtnPindahR.AutoSize = True
        Me.BtnPindahR.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnPindahR.FlatAppearance.BorderSize = 0
        Me.BtnPindahR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnPindahR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnPindahR.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnPindahR.ForeColor = System.Drawing.Color.Black
        Me.BtnPindahR.Image = CType(resources.GetObject("BtnPindahR.Image"), System.Drawing.Image)
        Me.BtnPindahR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPindahR.Location = New System.Drawing.Point(392, 2)
        Me.BtnPindahR.Name = "BtnPindahR"
        Me.BtnPindahR.Size = New System.Drawing.Size(177, 34)
        Me.BtnPindahR.TabIndex = 200
        Me.BtnPindahR.Text = "PINDAH REKENING"
        Me.BtnPindahR.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPindahR.UseVisualStyleBackColor = False
        '
        'BtnPengeluaran
        '
        Me.BtnPengeluaran.AutoSize = True
        Me.BtnPengeluaran.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnPengeluaran.FlatAppearance.BorderSize = 0
        Me.BtnPengeluaran.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnPengeluaran.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnPengeluaran.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnPengeluaran.ForeColor = System.Drawing.Color.Black
        Me.BtnPengeluaran.Image = CType(resources.GetObject("BtnPengeluaran.Image"), System.Drawing.Image)
        Me.BtnPengeluaran.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPengeluaran.Location = New System.Drawing.Point(147, 2)
        Me.BtnPengeluaran.Name = "BtnPengeluaran"
        Me.BtnPengeluaran.Size = New System.Drawing.Size(151, 34)
        Me.BtnPengeluaran.TabIndex = 199
        Me.BtnPengeluaran.Text = "PENGELUARAN"
        Me.BtnPengeluaran.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPengeluaran.UseVisualStyleBackColor = False
        '
        'BtnPemasukan
        '
        Me.BtnPemasukan.AutoSize = True
        Me.BtnPemasukan.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnPemasukan.FlatAppearance.BorderSize = 0
        Me.BtnPemasukan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnPemasukan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnPemasukan.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnPemasukan.ForeColor = System.Drawing.Color.Black
        Me.BtnPemasukan.Image = CType(resources.GetObject("BtnPemasukan.Image"), System.Drawing.Image)
        Me.BtnPemasukan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnPemasukan.Location = New System.Drawing.Point(14, 2)
        Me.BtnPemasukan.Name = "BtnPemasukan"
        Me.BtnPemasukan.Size = New System.Drawing.Size(133, 34)
        Me.BtnPemasukan.TabIndex = 198
        Me.BtnPemasukan.Text = "PEMASUKAN"
        Me.BtnPemasukan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnPemasukan.UseVisualStyleBackColor = False
        '
        'PanelPemasukan
        '
        Me.PanelPemasukan.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PanelPemasukan.BackColor = System.Drawing.Color.PowderBlue
        Me.PanelPemasukan.Controls.Add(Me.BTNKeluar)
        Me.PanelPemasukan.Controls.Add(Me.LblBantuKKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.TxtDebetKeuanganNama)
        Me.PanelPemasukan.Controls.Add(Me.LblBantuDKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.TxtKreditKeuanganNama)
        Me.PanelPemasukan.Controls.Add(Me.Label60)
        Me.PanelPemasukan.Controls.Add(Me.TxtBantuDKeuanganNama)
        Me.PanelPemasukan.Controls.Add(Me.Label53)
        Me.PanelPemasukan.Controls.Add(Me.TxtBantuKKeuanganNama)
        Me.PanelPemasukan.Controls.Add(Me.CmbBantuKKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.TxtDebetKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.TxtKreditKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.CmbBantuDKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.CmbKreditKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.CmbDebetKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.Label45)
        Me.PanelPemasukan.Controls.Add(Me.LblIdBayar)
        Me.PanelPemasukan.Controls.Add(Me.Label42)
        Me.PanelPemasukan.Controls.Add(Me.BtnBatalKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.BtnSimpanKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.BtnEditKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.TxtBantuDKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.TxtBantuKKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.LblNominalKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.Label73)
        Me.PanelPemasukan.Controls.Add(Me.TxtNoNota)
        Me.PanelPemasukan.Controls.Add(Me.LblNamaTransaksi)
        Me.PanelPemasukan.Controls.Add(Me.TxtNominalKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.Label71)
        Me.PanelPemasukan.Controls.Add(Me.DTPTglKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.Label70)
        Me.PanelPemasukan.Controls.Add(Me.TxtUraianKeuangan)
        Me.PanelPemasukan.Controls.Add(Me.Label69)
        Me.PanelPemasukan.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelPemasukan.Location = New System.Drawing.Point(12, 55)
        Me.PanelPemasukan.Name = "PanelPemasukan"
        Me.PanelPemasukan.Size = New System.Drawing.Size(390, 527)
        Me.PanelPemasukan.TabIndex = 203
        '
        'BTNKeluar
        '
        Me.BTNKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BTNKeluar.BackColor = System.Drawing.Color.Peru
        Me.BTNKeluar.FlatAppearance.BorderColor = System.Drawing.Color.White
        Me.BTNKeluar.FlatAppearance.BorderSize = 0
        Me.BTNKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BTNKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow
        Me.BTNKeluar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BTNKeluar.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BTNKeluar.ForeColor = System.Drawing.Color.Black
        Me.BTNKeluar.Image = CType(resources.GetObject("BTNKeluar.Image"), System.Drawing.Image)
        Me.BTNKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BTNKeluar.Location = New System.Drawing.Point(84, 480)
        Me.BTNKeluar.Name = "BTNKeluar"
        Me.BTNKeluar.Size = New System.Drawing.Size(159, 34)
        Me.BTNKeluar.TabIndex = 265
        Me.BTNKeluar.Text = " Keluar"
        Me.BTNKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BTNKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BTNKeluar.UseVisualStyleBackColor = False
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
        Me.BtnBatalKeuangan.BackColor = System.Drawing.Color.Peru
        Me.BtnBatalKeuangan.FlatAppearance.BorderSize = 0
        Me.BtnBatalKeuangan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnBatalKeuangan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.BtnBatalKeuangan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBatalKeuangan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBatalKeuangan.ForeColor = System.Drawing.Color.Black
        Me.BtnBatalKeuangan.Image = CType(resources.GetObject("BtnBatalKeuangan.Image"), System.Drawing.Image)
        Me.BtnBatalKeuangan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatalKeuangan.Location = New System.Drawing.Point(70, 414)
        Me.BtnBatalKeuangan.Name = "BtnBatalKeuangan"
        Me.BtnBatalKeuangan.Size = New System.Drawing.Size(159, 34)
        Me.BtnBatalKeuangan.TabIndex = 196
        Me.BtnBatalKeuangan.Text = " Kosong"
        Me.BtnBatalKeuangan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatalKeuangan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBatalKeuangan.UseVisualStyleBackColor = False
        '
        'BtnSimpanKeuangan
        '
        Me.BtnSimpanKeuangan.BackColor = System.Drawing.Color.Peru
        Me.BtnSimpanKeuangan.FlatAppearance.BorderSize = 0
        Me.BtnSimpanKeuangan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnSimpanKeuangan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnSimpanKeuangan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpanKeuangan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpanKeuangan.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpanKeuangan.Image = CType(resources.GetObject("BtnSimpanKeuangan.Image"), System.Drawing.Image)
        Me.BtnSimpanKeuangan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanKeuangan.Location = New System.Drawing.Point(70, 331)
        Me.BtnSimpanKeuangan.Name = "BtnSimpanKeuangan"
        Me.BtnSimpanKeuangan.Size = New System.Drawing.Size(159, 34)
        Me.BtnSimpanKeuangan.TabIndex = 192
        Me.BtnSimpanKeuangan.Text = " Simpan"
        Me.BtnSimpanKeuangan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanKeuangan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpanKeuangan.UseVisualStyleBackColor = False
        '
        'BtnEditKeuangan
        '
        Me.BtnEditKeuangan.BackColor = System.Drawing.Color.Peru
        Me.BtnEditKeuangan.FlatAppearance.BorderSize = 0
        Me.BtnEditKeuangan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnEditKeuangan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnEditKeuangan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnEditKeuangan.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnEditKeuangan.ForeColor = System.Drawing.Color.Black
        Me.BtnEditKeuangan.Image = CType(resources.GetObject("BtnEditKeuangan.Image"), System.Drawing.Image)
        Me.BtnEditKeuangan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnEditKeuangan.Location = New System.Drawing.Point(70, 374)
        Me.BtnEditKeuangan.Name = "BtnEditKeuangan"
        Me.BtnEditKeuangan.Size = New System.Drawing.Size(159, 34)
        Me.BtnEditKeuangan.TabIndex = 191
        Me.BtnEditKeuangan.Text = " Simpan Edit"
        Me.BtnEditKeuangan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnEditKeuangan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnEditKeuangan.UseVisualStyleBackColor = False
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
        'PanelRinciKeuangan
        '
        Me.PanelRinciKeuangan.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelRinciKeuangan.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
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
        Me.DgvKeuangan.BackgroundColor = System.Drawing.Color.White
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
        'BtnBiaya
        '
        Me.BtnBiaya.AutoSize = True
        Me.BtnBiaya.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtnBiaya.FlatAppearance.BorderSize = 0
        Me.BtnBiaya.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnBiaya.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnBiaya.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnBiaya.ForeColor = System.Drawing.Color.Black
        Me.BtnBiaya.Image = CType(resources.GetObject("BtnBiaya.Image"), System.Drawing.Image)
        Me.BtnBiaya.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBiaya.Location = New System.Drawing.Point(300, 2)
        Me.BtnBiaya.Name = "BtnBiaya"
        Me.BtnBiaya.Size = New System.Drawing.Size(88, 34)
        Me.BtnBiaya.TabIndex = 266
        Me.BtnBiaya.Text = "BIAYA"
        Me.BtnBiaya.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBiaya.UseVisualStyleBackColor = False
        '
        'PanelUtility
        '
        Me.PanelUtility.BackColor = System.Drawing.Color.Orange
        Me.PanelUtility.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelUtility.Controls.Add(Me.BtnPengeluaran)
        Me.PanelUtility.Controls.Add(Me.BtnBiaya)
        Me.PanelUtility.Controls.Add(Me.BtnPemasukan)
        Me.PanelUtility.Controls.Add(Me.BtnPindahR)
        Me.PanelUtility.Controls.Add(Me.BtnSetorBos)
        Me.PanelUtility.Controls.Add(Me.BtnBayarBon)
        Me.PanelUtility.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelUtility.Location = New System.Drawing.Point(0, 0)
        Me.PanelUtility.Name = "PanelUtility"
        Me.PanelUtility.Size = New System.Drawing.Size(1737, 40)
        Me.PanelUtility.TabIndex = 267
        '
        'FormKeuangan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1737, 594)
        Me.Controls.Add(Me.PanelUtility)
        Me.Controls.Add(Me.PanelRinciKeuangan)
        Me.Controls.Add(Me.PanelPemasukan)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormKeuangan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormKeuangan"
        Me.PanelPemasukan.ResumeLayout(False)
        Me.PanelPemasukan.PerformLayout()
        Me.PanelRinciKeuangan.ResumeLayout(False)
        Me.PanelRinciKeuangan.PerformLayout()
        CType(Me.DgvKeuangan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelUtility.ResumeLayout(False)
        Me.PanelUtility.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnBayarBon As System.Windows.Forms.Button
    Friend WithEvents BtnSetorBos As System.Windows.Forms.Button
    Friend WithEvents BtnPindahR As System.Windows.Forms.Button
    Friend WithEvents BtnPengeluaran As System.Windows.Forms.Button
    Friend WithEvents BtnPemasukan As System.Windows.Forms.Button
    Friend WithEvents PanelPemasukan As System.Windows.Forms.Panel
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
    Friend WithEvents BtnEditKeuangan As System.Windows.Forms.Button
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
    Friend WithEvents BtnBiaya As System.Windows.Forms.Button
    Friend WithEvents PanelUtility As System.Windows.Forms.Panel
    Friend WithEvents ToolTip1 As ToolTip
End Class
