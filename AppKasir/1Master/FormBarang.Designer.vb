<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormBarang
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
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormBarang))
        Me.DGBarang = New System.Windows.Forms.DataGridView()
        Me.PanelOperasi = New System.Windows.Forms.Panel()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.TxtCari = New System.Windows.Forms.TextBox()
        Me.BtnAkhir = New System.Windows.Forms.Button()
        Me.BtnNaik = New System.Windows.Forms.Button()
        Me.BtnTurun = New System.Windows.Forms.Button()
        Me.BtnAwal = New System.Windows.Forms.Button()
        Me.BtnHapus = New System.Windows.Forms.Button()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.BtnTambah = New System.Windows.Forms.Button()
        Me.BtnUbah = New System.Windows.Forms.Button()
        Me.Txtnamabarang = New System.Windows.Forms.TextBox()
        Me.Txtkodebarang = New System.Windows.Forms.TextBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DetailStokToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HistoriPembelianToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TambahToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HapusStokToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.TambahStokToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.KurangiStokToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.FilterBarangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByKodeToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByNamaToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByHargaBeliToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByStokTokoToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByStokGudangToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.HargaBeliHargaJualUmumKecilToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UmumKecilToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UmumSedangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UmumBesarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PartaiKecilToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PartaiSedangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PartaiBesarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportDataBarangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportDataBarangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.CetakBarcodeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LabelBarangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PerbaikiDatabase = New System.Windows.Forms.ToolStripMenuItem()
        Me.PerbaruiStokBarangToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripSeparator()
        Me.CetakLabelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CetakBarcodeToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.PAnelTambahKurang = New System.Windows.Forms.Panel()
        Me.GBTambah = New System.Windows.Forms.GroupBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtStokTotalGudangT = New System.Windows.Forms.TextBox()
        Me.TxtStokTotalTokoT = New System.Windows.Forms.TextBox()
        Me.TxtSatIsiGudangT = New System.Windows.Forms.TextBox()
        Me.TxtSatIsiTokoT = New System.Windows.Forms.TextBox()
        Me.CmbIsiGUdangT = New System.Windows.Forms.ComboBox()
        Me.CmbIsiTokoT = New System.Windows.Forms.ComboBox()
        Me.TxtIsiStokGudangT = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtIsiStokTokoT = New System.Windows.Forms.TextBox()
        Me.GbStokSaatIni = New System.Windows.Forms.GroupBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtSatuanGudang = New System.Windows.Forms.TextBox()
        Me.TxtSatuanToko = New System.Windows.Forms.TextBox()
        Me.TxtSatIsiGudang = New System.Windows.Forms.TextBox()
        Me.TxtSatIsiToko = New System.Windows.Forms.TextBox()
        Me.TxtIsiStokGudang = New System.Windows.Forms.TextBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.TxtIsiStokToko = New System.Windows.Forms.TextBox()
        Me.LblJudulStok = New System.Windows.Forms.Label()
        Me.BtnSimpanStok = New System.Windows.Forms.Button()
        Me.BtnKeluarStok = New System.Windows.Forms.Button()
        Me.TxtNamaSupliyer = New System.Windows.Forms.TextBox()
        Me.TxtNamaKategori = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtHargaBeli = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.LabelProgress = New System.Windows.Forms.Label()
        Me.PanelDetailBarang = New System.Windows.Forms.Panel()
        Me.Label96 = New System.Windows.Forms.Label()
        Me.Label107 = New System.Windows.Forms.Label()
        Me.BtnSembunyi = New System.Windows.Forms.Button()
        Me.Label94 = New System.Windows.Forms.Label()
        Me.Label95 = New System.Windows.Forms.Label()
        Me.Label97 = New System.Windows.Forms.Label()
        Me.Label98 = New System.Windows.Forms.Label()
        Me.Label99 = New System.Windows.Forms.Label()
        Me.Label100 = New System.Windows.Forms.Label()
        Me.Label101 = New System.Windows.Forms.Label()
        Me.Label102 = New System.Windows.Forms.Label()
        Me.Label103 = New System.Windows.Forms.Label()
        Me.Label104 = New System.Windows.Forms.Label()
        Me.Label105 = New System.Windows.Forms.Label()
        Me.Label106 = New System.Windows.Forms.Label()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.Label78 = New System.Windows.Forms.Label()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.Label82 = New System.Windows.Forms.Label()
        Me.Label56 = New System.Windows.Forms.Label()
        Me.Label92 = New System.Windows.Forms.Label()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.Label93 = New System.Windows.Forms.Label()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.Label91 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.Label88 = New System.Windows.Forms.Label()
        Me.Label65 = New System.Windows.Forms.Label()
        Me.Label70 = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.Label87 = New System.Windows.Forms.Label()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.Label90 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.Label67 = New System.Windows.Forms.Label()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label86 = New System.Windows.Forms.Label()
        Me.Label68 = New System.Windows.Forms.Label()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.Label85 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.Label81 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label89 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label74 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label73 = New System.Windows.Forms.Label()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.Label61 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label72 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.Label79 = New System.Windows.Forms.Label()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.Label84 = New System.Windows.Forms.Label()
        Me.Label80 = New System.Windows.Forms.Label()
        Me.Label83 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.BarcodeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.DGBarang, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelOperasi.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.PAnelTambahKurang.SuspendLayout()
        Me.GBTambah.SuspendLayout()
        Me.GbStokSaatIni.SuspendLayout()
        Me.PanelDetailBarang.SuspendLayout()
        Me.SuspendLayout()
        '
        'DGBarang
        '
        Me.DGBarang.AllowUserToDeleteRows = False
        Me.DGBarang.AllowUserToOrderColumns = True
        Me.DGBarang.AllowUserToResizeRows = False
        Me.DGBarang.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGBarang.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.DGBarang.BackgroundColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.Yellow
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGBarang.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DGBarang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGBarang.DefaultCellStyle = DataGridViewCellStyle2
        Me.DGBarang.Location = New System.Drawing.Point(0, 0)
        Me.DGBarang.Name = "DGBarang"
        Me.DGBarang.ReadOnly = True
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ActiveBorder
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGBarang.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.DGBarang.RowHeadersVisible = False
        Me.DGBarang.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGBarang.Size = New System.Drawing.Size(1386, 512)
        Me.DGBarang.TabIndex = 2
        '
        'PanelOperasi
        '
        Me.PanelOperasi.Controls.Add(Me.PanelCari)
        Me.PanelOperasi.Controls.Add(Me.BtnAkhir)
        Me.PanelOperasi.Controls.Add(Me.BtnNaik)
        Me.PanelOperasi.Controls.Add(Me.BtnTurun)
        Me.PanelOperasi.Controls.Add(Me.BtnAwal)
        Me.PanelOperasi.Controls.Add(Me.BtnHapus)
        Me.PanelOperasi.Controls.Add(Me.BtnKeluar)
        Me.PanelOperasi.Controls.Add(Me.BtnTambah)
        Me.PanelOperasi.Controls.Add(Me.BtnUbah)
        Me.PanelOperasi.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelOperasi.Location = New System.Drawing.Point(0, 513)
        Me.PanelOperasi.Name = "PanelOperasi"
        Me.PanelOperasi.Size = New System.Drawing.Size(1386, 41)
        Me.PanelOperasi.TabIndex = 0
        '
        'PanelCari
        '
        Me.PanelCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCari.Controls.Add(Me.BtnCari)
        Me.PanelCari.Controls.Add(Me.TxtCari)
        Me.PanelCari.Location = New System.Drawing.Point(3, 5)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(465, 30)
        Me.PanelCari.TabIndex = 174
        '
        'BtnCari
        '
        Me.BtnCari.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.Location = New System.Drawing.Point(434, 4)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(25, 23)
        Me.BtnCari.TabIndex = 18
        Me.BtnCari.UseVisualStyleBackColor = True
        '
        'TxtCari
        '
        Me.TxtCari.BackColor = System.Drawing.Color.White
        Me.TxtCari.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtCari.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCari.ForeColor = System.Drawing.Color.Black
        Me.TxtCari.Location = New System.Drawing.Point(5, 4)
        Me.TxtCari.Name = "TxtCari"
        Me.TxtCari.Size = New System.Drawing.Size(428, 23)
        Me.TxtCari.TabIndex = 0
        '
        'BtnAkhir
        '
        Me.BtnAkhir.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnAkhir.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAkhir.Location = New System.Drawing.Point(605, 8)
        Me.BtnAkhir.Name = "BtnAkhir"
        Me.BtnAkhir.Size = New System.Drawing.Size(37, 25)
        Me.BtnAkhir.TabIndex = 164
        Me.BtnAkhir.Text = ">>"
        Me.BtnAkhir.UseVisualStyleBackColor = True
        Me.BtnAkhir.Visible = False
        '
        'BtnNaik
        '
        Me.BtnNaik.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnNaik.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnNaik.Location = New System.Drawing.Point(566, 8)
        Me.BtnNaik.Name = "BtnNaik"
        Me.BtnNaik.Size = New System.Drawing.Size(37, 25)
        Me.BtnNaik.TabIndex = 163
        Me.BtnNaik.Text = ">"
        Me.BtnNaik.UseVisualStyleBackColor = True
        Me.BtnNaik.Visible = False
        '
        'BtnTurun
        '
        Me.BtnTurun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnTurun.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTurun.Location = New System.Drawing.Point(523, 8)
        Me.BtnTurun.Name = "BtnTurun"
        Me.BtnTurun.Size = New System.Drawing.Size(37, 25)
        Me.BtnTurun.TabIndex = 162
        Me.BtnTurun.Text = "<"
        Me.BtnTurun.UseVisualStyleBackColor = True
        Me.BtnTurun.Visible = False
        '
        'BtnAwal
        '
        Me.BtnAwal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnAwal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAwal.Location = New System.Drawing.Point(484, 8)
        Me.BtnAwal.Name = "BtnAwal"
        Me.BtnAwal.Size = New System.Drawing.Size(37, 25)
        Me.BtnAwal.TabIndex = 161
        Me.BtnAwal.Text = "<<"
        Me.BtnAwal.UseVisualStyleBackColor = True
        Me.BtnAwal.Visible = False
        '
        'BtnHapus
        '
        Me.BtnHapus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnHapus.BackColor = System.Drawing.Color.Yellow
        Me.BtnHapus.FlatAppearance.BorderSize = 0
        Me.BtnHapus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BtnHapus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.BtnHapus.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnHapus.ForeColor = System.Drawing.Color.Black
        Me.BtnHapus.Image = CType(resources.GetObject("BtnHapus.Image"), System.Drawing.Image)
        Me.BtnHapus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnHapus.Location = New System.Drawing.Point(1111, 3)
        Me.BtnHapus.Name = "BtnHapus"
        Me.BtnHapus.Size = New System.Drawing.Size(115, 35)
        Me.BtnHapus.TabIndex = 158
        Me.BtnHapus.Text = "HAPUS (F4)"
        Me.BtnHapus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnHapus.UseVisualStyleBackColor = False
        '
        'BtnKeluar
        '
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluar.BackColor = System.Drawing.Color.Yellow
        Me.BtnKeluar.FlatAppearance.BorderSize = 0
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BtnKeluar.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.Black
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluar.Location = New System.Drawing.Point(1235, 3)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(139, 35)
        Me.BtnKeluar.TabIndex = 134
        Me.BtnKeluar.Text = "KELUAR (Esc)"
        Me.BtnKeluar.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'BtnTambah
        '
        Me.BtnTambah.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnTambah.BackColor = System.Drawing.Color.Yellow
        Me.BtnTambah.FlatAppearance.BorderSize = 0
        Me.BtnTambah.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnTambah.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnTambah.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTambah.ForeColor = System.Drawing.Color.Black
        Me.BtnTambah.Image = CType(resources.GetObject("BtnTambah.Image"), System.Drawing.Image)
        Me.BtnTambah.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTambah.Location = New System.Drawing.Point(845, 3)
        Me.BtnTambah.Name = "BtnTambah"
        Me.BtnTambah.Size = New System.Drawing.Size(138, 35)
        Me.BtnTambah.TabIndex = 19
        Me.BtnTambah.Text = " TAMBAH (F2)"
        Me.BtnTambah.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnTambah.UseVisualStyleBackColor = False
        '
        'BtnUbah
        '
        Me.BtnUbah.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnUbah.BackColor = System.Drawing.Color.Yellow
        Me.BtnUbah.FlatAppearance.BorderSize = 0
        Me.BtnUbah.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtnUbah.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MidnightBlue
        Me.BtnUbah.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnUbah.ForeColor = System.Drawing.Color.Black
        Me.BtnUbah.Image = CType(resources.GetObject("BtnUbah.Image"), System.Drawing.Image)
        Me.BtnUbah.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnUbah.Location = New System.Drawing.Point(1004, 3)
        Me.BtnUbah.Name = "BtnUbah"
        Me.BtnUbah.Size = New System.Drawing.Size(98, 35)
        Me.BtnUbah.TabIndex = 20
        Me.BtnUbah.Text = "EDIT (F3)"
        Me.BtnUbah.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnUbah.UseVisualStyleBackColor = False
        '
        'Txtnamabarang
        '
        Me.Txtnamabarang.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Txtnamabarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Txtnamabarang.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Txtnamabarang.ForeColor = System.Drawing.Color.Teal
        Me.Txtnamabarang.Location = New System.Drawing.Point(535, 446)
        Me.Txtnamabarang.Name = "Txtnamabarang"
        Me.Txtnamabarang.Size = New System.Drawing.Size(25, 22)
        Me.Txtnamabarang.TabIndex = 159
        Me.Txtnamabarang.Visible = False
        '
        'Txtkodebarang
        '
        Me.Txtkodebarang.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Txtkodebarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Txtkodebarang.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Txtkodebarang.ForeColor = System.Drawing.Color.Teal
        Me.Txtkodebarang.Location = New System.Drawing.Point(566, 446)
        Me.Txtkodebarang.Name = "Txtkodebarang"
        Me.Txtkodebarang.Size = New System.Drawing.Size(53, 22)
        Me.Txtkodebarang.TabIndex = 112
        Me.Txtkodebarang.Visible = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DetailStokToolStripMenuItem, Me.HistoriPembelianToolStripMenuItem, Me.RefreshToolStripMenuItem, Me.ToolStripMenuItem2, Me.TambahToolStripMenuItem, Me.EditToolStripMenuItem, Me.HapusStokToolStripMenuItem, Me.ToolStripMenuItem3, Me.TambahStokToolStripMenuItem, Me.KurangiStokToolStripMenuItem, Me.ToolStripMenuItem1, Me.FilterBarangToolStripMenuItem, Me.HargaBeliHargaJualUmumKecilToolStripMenuItem, Me.ToolStripMenuItem5, Me.ExportDataBarangToolStripMenuItem, Me.ImportDataBarangToolStripMenuItem, Me.ToolStripMenuItem4, Me.CetakBarcodeToolStripMenuItem, Me.LabelBarangToolStripMenuItem, Me.PerbaikiDatabase, Me.PerbaruiStokBarangToolStripMenuItem, Me.ToolStripMenuItem6, Me.CetakLabelToolStripMenuItem, Me.CetakBarcodeToolStripMenuItem1, Me.BarcodeToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(212, 480)
        '
        'DetailStokToolStripMenuItem
        '
        Me.DetailStokToolStripMenuItem.Name = "DetailStokToolStripMenuItem"
        Me.DetailStokToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.DetailStokToolStripMenuItem.Text = "Detail Barang"
        '
        'HistoriPembelianToolStripMenuItem
        '
        Me.HistoriPembelianToolStripMenuItem.Name = "HistoriPembelianToolStripMenuItem"
        Me.HistoriPembelianToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.HistoriPembelianToolStripMenuItem.Text = "Histori pembelian"
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(208, 6)
        '
        'TambahToolStripMenuItem
        '
        Me.TambahToolStripMenuItem.Name = "TambahToolStripMenuItem"
        Me.TambahToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.TambahToolStripMenuItem.Text = "Tambah Barang"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.EditToolStripMenuItem.Text = "Edit Barang"
        '
        'HapusStokToolStripMenuItem
        '
        Me.HapusStokToolStripMenuItem.Name = "HapusStokToolStripMenuItem"
        Me.HapusStokToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.HapusStokToolStripMenuItem.Text = "Hapus Barang"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(208, 6)
        '
        'TambahStokToolStripMenuItem
        '
        Me.TambahStokToolStripMenuItem.Name = "TambahStokToolStripMenuItem"
        Me.TambahStokToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.TambahStokToolStripMenuItem.Text = "Tambah Stok"
        '
        'KurangiStokToolStripMenuItem
        '
        Me.KurangiStokToolStripMenuItem.Name = "KurangiStokToolStripMenuItem"
        Me.KurangiStokToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.KurangiStokToolStripMenuItem.Text = "Kurangi Stok"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(208, 6)
        '
        'FilterBarangToolStripMenuItem
        '
        Me.FilterBarangToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ByKodeToolStripMenuItem1, Me.ByNamaToolStripMenuItem1, Me.ByHargaBeliToolStripMenuItem1, Me.ByStokTokoToolStripMenuItem1, Me.ByStokGudangToolStripMenuItem1})
        Me.FilterBarangToolStripMenuItem.Name = "FilterBarangToolStripMenuItem"
        Me.FilterBarangToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.FilterBarangToolStripMenuItem.Text = "Filter Barang"
        '
        'ByKodeToolStripMenuItem1
        '
        Me.ByKodeToolStripMenuItem1.Name = "ByKodeToolStripMenuItem1"
        Me.ByKodeToolStripMenuItem1.Size = New System.Drawing.Size(158, 22)
        Me.ByKodeToolStripMenuItem1.Text = "By Kode"
        '
        'ByNamaToolStripMenuItem1
        '
        Me.ByNamaToolStripMenuItem1.Name = "ByNamaToolStripMenuItem1"
        Me.ByNamaToolStripMenuItem1.Size = New System.Drawing.Size(158, 22)
        Me.ByNamaToolStripMenuItem1.Text = "By Nama"
        '
        'ByHargaBeliToolStripMenuItem1
        '
        Me.ByHargaBeliToolStripMenuItem1.Name = "ByHargaBeliToolStripMenuItem1"
        Me.ByHargaBeliToolStripMenuItem1.Size = New System.Drawing.Size(158, 22)
        Me.ByHargaBeliToolStripMenuItem1.Text = "By Harga Beli"
        '
        'ByStokTokoToolStripMenuItem1
        '
        Me.ByStokTokoToolStripMenuItem1.Name = "ByStokTokoToolStripMenuItem1"
        Me.ByStokTokoToolStripMenuItem1.Size = New System.Drawing.Size(158, 22)
        Me.ByStokTokoToolStripMenuItem1.Text = "By Stok Toko"
        '
        'ByStokGudangToolStripMenuItem1
        '
        Me.ByStokGudangToolStripMenuItem1.Name = "ByStokGudangToolStripMenuItem1"
        Me.ByStokGudangToolStripMenuItem1.Size = New System.Drawing.Size(158, 22)
        Me.ByStokGudangToolStripMenuItem1.Text = "By Stok Gudang"
        '
        'HargaBeliHargaJualUmumKecilToolStripMenuItem
        '
        Me.HargaBeliHargaJualUmumKecilToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UmumKecilToolStripMenuItem, Me.UmumSedangToolStripMenuItem, Me.UmumBesarToolStripMenuItem, Me.PartaiKecilToolStripMenuItem, Me.PartaiSedangToolStripMenuItem, Me.PartaiBesarToolStripMenuItem})
        Me.HargaBeliHargaJualUmumKecilToolStripMenuItem.Name = "HargaBeliHargaJualUmumKecilToolStripMenuItem"
        Me.HargaBeliHargaJualUmumKecilToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.HargaBeliHargaJualUmumKecilToolStripMenuItem.Text = "Harga beli > Harga jual"
        '
        'UmumKecilToolStripMenuItem
        '
        Me.UmumKecilToolStripMenuItem.Name = "UmumKecilToolStripMenuItem"
        Me.UmumKecilToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.UmumKecilToolStripMenuItem.Text = "Umum Kecil"
        '
        'UmumSedangToolStripMenuItem
        '
        Me.UmumSedangToolStripMenuItem.Name = "UmumSedangToolStripMenuItem"
        Me.UmumSedangToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.UmumSedangToolStripMenuItem.Text = "Umum Sedang"
        '
        'UmumBesarToolStripMenuItem
        '
        Me.UmumBesarToolStripMenuItem.Name = "UmumBesarToolStripMenuItem"
        Me.UmumBesarToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.UmumBesarToolStripMenuItem.Text = "Umum Besar"
        '
        'PartaiKecilToolStripMenuItem
        '
        Me.PartaiKecilToolStripMenuItem.Name = "PartaiKecilToolStripMenuItem"
        Me.PartaiKecilToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.PartaiKecilToolStripMenuItem.Text = "Partai Kecil"
        '
        'PartaiSedangToolStripMenuItem
        '
        Me.PartaiSedangToolStripMenuItem.Name = "PartaiSedangToolStripMenuItem"
        Me.PartaiSedangToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.PartaiSedangToolStripMenuItem.Text = "Partai Sedang"
        '
        'PartaiBesarToolStripMenuItem
        '
        Me.PartaiBesarToolStripMenuItem.Name = "PartaiBesarToolStripMenuItem"
        Me.PartaiBesarToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.PartaiBesarToolStripMenuItem.Text = "Partai Besar"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(208, 6)
        '
        'ExportDataBarangToolStripMenuItem
        '
        Me.ExportDataBarangToolStripMenuItem.Name = "ExportDataBarangToolStripMenuItem"
        Me.ExportDataBarangToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.ExportDataBarangToolStripMenuItem.Text = "Export Data Barang"
        '
        'ImportDataBarangToolStripMenuItem
        '
        Me.ImportDataBarangToolStripMenuItem.Name = "ImportDataBarangToolStripMenuItem"
        Me.ImportDataBarangToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.ImportDataBarangToolStripMenuItem.Text = "Import Data Barang"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(208, 6)
        '
        'CetakBarcodeToolStripMenuItem
        '
        Me.CetakBarcodeToolStripMenuItem.Name = "CetakBarcodeToolStripMenuItem"
        Me.CetakBarcodeToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.CetakBarcodeToolStripMenuItem.Text = "Cetak Barcode"
        Me.CetakBarcodeToolStripMenuItem.Visible = False
        '
        'LabelBarangToolStripMenuItem
        '
        Me.LabelBarangToolStripMenuItem.Name = "LabelBarangToolStripMenuItem"
        Me.LabelBarangToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.LabelBarangToolStripMenuItem.Text = "Label Barang"
        Me.LabelBarangToolStripMenuItem.Visible = False
        '
        'PerbaikiDatabase
        '
        Me.PerbaikiDatabase.Name = "PerbaikiDatabase"
        Me.PerbaikiDatabase.Size = New System.Drawing.Size(211, 22)
        Me.PerbaikiDatabase.Text = "Perbaiki Database"
        '
        'PerbaruiStokBarangToolStripMenuItem
        '
        Me.PerbaruiStokBarangToolStripMenuItem.Name = "PerbaruiStokBarangToolStripMenuItem"
        Me.PerbaruiStokBarangToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.PerbaruiStokBarangToolStripMenuItem.Text = "Perbarui Isi Satuan Barang"
        Me.PerbaruiStokBarangToolStripMenuItem.Visible = False
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(208, 6)
        '
        'CetakLabelToolStripMenuItem
        '
        Me.CetakLabelToolStripMenuItem.Name = "CetakLabelToolStripMenuItem"
        Me.CetakLabelToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.CetakLabelToolStripMenuItem.Text = "Cetak Label"
        '
        'CetakBarcodeToolStripMenuItem1
        '
        Me.CetakBarcodeToolStripMenuItem1.Name = "CetakBarcodeToolStripMenuItem1"
        Me.CetakBarcodeToolStripMenuItem1.Size = New System.Drawing.Size(211, 22)
        Me.CetakBarcodeToolStripMenuItem1.Text = "Cetak Barcode"
        '
        'PAnelTambahKurang
        '
        Me.PAnelTambahKurang.BackColor = System.Drawing.Color.White
        Me.PAnelTambahKurang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PAnelTambahKurang.Controls.Add(Me.GBTambah)
        Me.PAnelTambahKurang.Controls.Add(Me.GbStokSaatIni)
        Me.PAnelTambahKurang.Controls.Add(Me.LblJudulStok)
        Me.PAnelTambahKurang.Controls.Add(Me.BtnSimpanStok)
        Me.PAnelTambahKurang.Controls.Add(Me.BtnKeluarStok)
        Me.PAnelTambahKurang.Controls.Add(Me.TxtNamaSupliyer)
        Me.PAnelTambahKurang.Controls.Add(Me.TxtNamaKategori)
        Me.PAnelTambahKurang.Controls.Add(Me.Label8)
        Me.PAnelTambahKurang.Controls.Add(Me.Label3)
        Me.PAnelTambahKurang.Controls.Add(Me.TxtNama)
        Me.PAnelTambahKurang.Controls.Add(Me.Label2)
        Me.PAnelTambahKurang.Controls.Add(Me.Label4)
        Me.PAnelTambahKurang.Controls.Add(Me.TxtHargaBeli)
        Me.PAnelTambahKurang.Controls.Add(Me.TxtKode)
        Me.PAnelTambahKurang.Location = New System.Drawing.Point(22, 46)
        Me.PAnelTambahKurang.Name = "PAnelTambahKurang"
        Me.PAnelTambahKurang.Size = New System.Drawing.Size(446, 440)
        Me.PAnelTambahKurang.TabIndex = 3
        '
        'GBTambah
        '
        Me.GBTambah.BackColor = System.Drawing.Color.White
        Me.GBTambah.Controls.Add(Me.Label48)
        Me.GBTambah.Controls.Add(Me.Label10)
        Me.GBTambah.Controls.Add(Me.Label57)
        Me.GBTambah.Controls.Add(Me.Label9)
        Me.GBTambah.Controls.Add(Me.Label7)
        Me.GBTambah.Controls.Add(Me.TxtStokTotalGudangT)
        Me.GBTambah.Controls.Add(Me.TxtStokTotalTokoT)
        Me.GBTambah.Controls.Add(Me.TxtSatIsiGudangT)
        Me.GBTambah.Controls.Add(Me.TxtSatIsiTokoT)
        Me.GBTambah.Controls.Add(Me.CmbIsiGUdangT)
        Me.GBTambah.Controls.Add(Me.CmbIsiTokoT)
        Me.GBTambah.Controls.Add(Me.TxtIsiStokGudangT)
        Me.GBTambah.Controls.Add(Me.Label5)
        Me.GBTambah.Controls.Add(Me.Label6)
        Me.GBTambah.Controls.Add(Me.TxtIsiStokTokoT)
        Me.GBTambah.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBTambah.Location = New System.Drawing.Point(11, 277)
        Me.GBTambah.Name = "GBTambah"
        Me.GBTambah.Size = New System.Drawing.Size(418, 115)
        Me.GBTambah.TabIndex = 172
        Me.GBTambah.TabStop = False
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label48.Location = New System.Drawing.Point(11, 82)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(67, 17)
        Me.Label48.TabIndex = 174
        Me.Label48.Text = "GUDANG"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(312, 23)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(76, 20)
        Me.Label10.TabIndex = 130
        Me.Label10.Text = "Total Stok"
        Me.Label10.Visible = False
        '
        'Label57
        '
        Me.Label57.AutoSize = True
        Me.Label57.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label57.Location = New System.Drawing.Point(11, 52)
        Me.Label57.Name = "Label57"
        Me.Label57.Size = New System.Drawing.Size(43, 17)
        Me.Label57.TabIndex = 173
        Me.Label57.Text = "TOKO"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(278, 23)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(23, 20)
        Me.Label9.TabIndex = 129
        Me.Label9.Text = "Isi"
        Me.Label9.Visible = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(170, 23)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(59, 20)
        Me.Label7.TabIndex = 128
        Me.Label7.Text = "Satuan"
        '
        'TxtStokTotalGudangT
        '
        Me.TxtStokTotalGudangT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStokTotalGudangT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStokTotalGudangT.Location = New System.Drawing.Point(312, 78)
        Me.TxtStokTotalGudangT.Name = "TxtStokTotalGudangT"
        Me.TxtStokTotalGudangT.ReadOnly = True
        Me.TxtStokTotalGudangT.Size = New System.Drawing.Size(97, 26)
        Me.TxtStokTotalGudangT.TabIndex = 127
        Me.TxtStokTotalGudangT.Visible = False
        '
        'TxtStokTotalTokoT
        '
        Me.TxtStokTotalTokoT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStokTotalTokoT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStokTotalTokoT.Location = New System.Drawing.Point(312, 47)
        Me.TxtStokTotalTokoT.Name = "TxtStokTotalTokoT"
        Me.TxtStokTotalTokoT.ReadOnly = True
        Me.TxtStokTotalTokoT.Size = New System.Drawing.Size(97, 26)
        Me.TxtStokTotalTokoT.TabIndex = 126
        Me.TxtStokTotalTokoT.Visible = False
        '
        'TxtSatIsiGudangT
        '
        Me.TxtSatIsiGudangT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSatIsiGudangT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSatIsiGudangT.Location = New System.Drawing.Point(279, 78)
        Me.TxtSatIsiGudangT.Name = "TxtSatIsiGudangT"
        Me.TxtSatIsiGudangT.ReadOnly = True
        Me.TxtSatIsiGudangT.Size = New System.Drawing.Size(27, 26)
        Me.TxtSatIsiGudangT.TabIndex = 125
        Me.TxtSatIsiGudangT.Visible = False
        '
        'TxtSatIsiTokoT
        '
        Me.TxtSatIsiTokoT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSatIsiTokoT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSatIsiTokoT.Location = New System.Drawing.Point(279, 47)
        Me.TxtSatIsiTokoT.Name = "TxtSatIsiTokoT"
        Me.TxtSatIsiTokoT.ReadOnly = True
        Me.TxtSatIsiTokoT.Size = New System.Drawing.Size(27, 26)
        Me.TxtSatIsiTokoT.TabIndex = 124
        Me.TxtSatIsiTokoT.Visible = False
        '
        'CmbIsiGUdangT
        '
        Me.CmbIsiGUdangT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbIsiGUdangT.Enabled = False
        Me.CmbIsiGUdangT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbIsiGUdangT.FormattingEnabled = True
        Me.CmbIsiGUdangT.Location = New System.Drawing.Point(174, 77)
        Me.CmbIsiGUdangT.Name = "CmbIsiGUdangT"
        Me.CmbIsiGUdangT.Size = New System.Drawing.Size(98, 28)
        Me.CmbIsiGUdangT.TabIndex = 123
        '
        'CmbIsiTokoT
        '
        Me.CmbIsiTokoT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbIsiTokoT.Enabled = False
        Me.CmbIsiTokoT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbIsiTokoT.FormattingEnabled = True
        Me.CmbIsiTokoT.Location = New System.Drawing.Point(174, 46)
        Me.CmbIsiTokoT.Name = "CmbIsiTokoT"
        Me.CmbIsiTokoT.Size = New System.Drawing.Size(98, 28)
        Me.CmbIsiTokoT.TabIndex = 122
        '
        'TxtIsiStokGudangT
        '
        Me.TxtIsiStokGudangT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIsiStokGudangT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiStokGudangT.Location = New System.Drawing.Point(105, 78)
        Me.TxtIsiStokGudangT.Name = "TxtIsiStokGudangT"
        Me.TxtIsiStokGudangT.Size = New System.Drawing.Size(63, 26)
        Me.TxtIsiStokGudangT.TabIndex = 121
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(10, 23)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(53, 20)
        Me.Label5.TabIndex = 118
        Me.Label5.Text = "Lokasi"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(101, 23)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(39, 20)
        Me.Label6.TabIndex = 117
        Me.Label6.Text = "Stok"
        '
        'TxtIsiStokTokoT
        '
        Me.TxtIsiStokTokoT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIsiStokTokoT.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiStokTokoT.Location = New System.Drawing.Point(105, 47)
        Me.TxtIsiStokTokoT.Name = "TxtIsiStokTokoT"
        Me.TxtIsiStokTokoT.Size = New System.Drawing.Size(63, 26)
        Me.TxtIsiStokTokoT.TabIndex = 116
        '
        'GbStokSaatIni
        '
        Me.GbStokSaatIni.Controls.Add(Me.Label16)
        Me.GbStokSaatIni.Controls.Add(Me.Label15)
        Me.GbStokSaatIni.Controls.Add(Me.Label12)
        Me.GbStokSaatIni.Controls.Add(Me.Label13)
        Me.GbStokSaatIni.Controls.Add(Me.Label14)
        Me.GbStokSaatIni.Controls.Add(Me.TxtSatuanGudang)
        Me.GbStokSaatIni.Controls.Add(Me.TxtSatuanToko)
        Me.GbStokSaatIni.Controls.Add(Me.TxtSatIsiGudang)
        Me.GbStokSaatIni.Controls.Add(Me.TxtSatIsiToko)
        Me.GbStokSaatIni.Controls.Add(Me.TxtIsiStokGudang)
        Me.GbStokSaatIni.Controls.Add(Me.Label35)
        Me.GbStokSaatIni.Controls.Add(Me.TxtIsiStokToko)
        Me.GbStokSaatIni.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GbStokSaatIni.Location = New System.Drawing.Point(11, 161)
        Me.GbStokSaatIni.Name = "GbStokSaatIni"
        Me.GbStokSaatIni.Size = New System.Drawing.Size(418, 115)
        Me.GbStokSaatIni.TabIndex = 171
        Me.GbStokSaatIni.TabStop = False
        Me.GbStokSaatIni.Text = "Stok Sebelumnya"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(10, 82)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(79, 20)
        Me.Label16.TabIndex = 136
        Me.Label16.Text = "GUDANG"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(10, 51)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(49, 20)
        Me.Label15.TabIndex = 135
        Me.Label15.Text = "TOKO"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(278, 22)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(23, 20)
        Me.Label12.TabIndex = 133
        Me.Label12.Text = "Isi"
        Me.Label12.Visible = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(170, 22)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(59, 20)
        Me.Label13.TabIndex = 132
        Me.Label13.Text = "Satuan"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(101, 22)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(39, 20)
        Me.Label14.TabIndex = 131
        Me.Label14.Text = "Stok"
        '
        'TxtSatuanGudang
        '
        Me.TxtSatuanGudang.BackColor = System.Drawing.Color.White
        Me.TxtSatuanGudang.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSatuanGudang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSatuanGudang.Location = New System.Drawing.Point(174, 83)
        Me.TxtSatuanGudang.Name = "TxtSatuanGudang"
        Me.TxtSatuanGudang.ReadOnly = True
        Me.TxtSatuanGudang.Size = New System.Drawing.Size(97, 19)
        Me.TxtSatuanGudang.TabIndex = 126
        '
        'TxtSatuanToko
        '
        Me.TxtSatuanToko.BackColor = System.Drawing.Color.White
        Me.TxtSatuanToko.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSatuanToko.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSatuanToko.Location = New System.Drawing.Point(174, 52)
        Me.TxtSatuanToko.Name = "TxtSatuanToko"
        Me.TxtSatuanToko.ReadOnly = True
        Me.TxtSatuanToko.Size = New System.Drawing.Size(97, 19)
        Me.TxtSatuanToko.TabIndex = 126
        '
        'TxtSatIsiGudang
        '
        Me.TxtSatIsiGudang.BackColor = System.Drawing.Color.White
        Me.TxtSatIsiGudang.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSatIsiGudang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSatIsiGudang.Location = New System.Drawing.Point(278, 83)
        Me.TxtSatIsiGudang.Name = "TxtSatIsiGudang"
        Me.TxtSatIsiGudang.ReadOnly = True
        Me.TxtSatIsiGudang.Size = New System.Drawing.Size(27, 19)
        Me.TxtSatIsiGudang.TabIndex = 125
        Me.TxtSatIsiGudang.Visible = False
        '
        'TxtSatIsiToko
        '
        Me.TxtSatIsiToko.BackColor = System.Drawing.Color.White
        Me.TxtSatIsiToko.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSatIsiToko.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSatIsiToko.Location = New System.Drawing.Point(278, 52)
        Me.TxtSatIsiToko.Name = "TxtSatIsiToko"
        Me.TxtSatIsiToko.ReadOnly = True
        Me.TxtSatIsiToko.Size = New System.Drawing.Size(27, 19)
        Me.TxtSatIsiToko.TabIndex = 124
        Me.TxtSatIsiToko.Visible = False
        '
        'TxtIsiStokGudang
        '
        Me.TxtIsiStokGudang.BackColor = System.Drawing.Color.White
        Me.TxtIsiStokGudang.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtIsiStokGudang.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiStokGudang.Location = New System.Drawing.Point(105, 83)
        Me.TxtIsiStokGudang.Name = "TxtIsiStokGudang"
        Me.TxtIsiStokGudang.ReadOnly = True
        Me.TxtIsiStokGudang.Size = New System.Drawing.Size(63, 19)
        Me.TxtIsiStokGudang.TabIndex = 121
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label35.Location = New System.Drawing.Point(10, 22)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(53, 20)
        Me.Label35.TabIndex = 118
        Me.Label35.Text = "Lokasi"
        '
        'TxtIsiStokToko
        '
        Me.TxtIsiStokToko.BackColor = System.Drawing.Color.White
        Me.TxtIsiStokToko.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtIsiStokToko.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiStokToko.Location = New System.Drawing.Point(105, 52)
        Me.TxtIsiStokToko.Name = "TxtIsiStokToko"
        Me.TxtIsiStokToko.ReadOnly = True
        Me.TxtIsiStokToko.Size = New System.Drawing.Size(63, 19)
        Me.TxtIsiStokToko.TabIndex = 116
        '
        'LblJudulStok
        '
        Me.LblJudulStok.BackColor = System.Drawing.Color.PaleTurquoise
        Me.LblJudulStok.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblJudulStok.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJudulStok.Location = New System.Drawing.Point(0, 0)
        Me.LblJudulStok.Name = "LblJudulStok"
        Me.LblJudulStok.Size = New System.Drawing.Size(444, 37)
        Me.LblJudulStok.TabIndex = 170
        Me.LblJudulStok.Text = "Kode"
        Me.LblJudulStok.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnSimpanStok
        '
        Me.BtnSimpanStok.BackColor = System.Drawing.Color.Yellow
        Me.BtnSimpanStok.FlatAppearance.BorderSize = 0
        Me.BtnSimpanStok.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GreenYellow
        Me.BtnSimpanStok.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GreenYellow
        Me.BtnSimpanStok.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpanStok.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpanStok.Image = CType(resources.GetObject("BtnSimpanStok.Image"), System.Drawing.Image)
        Me.BtnSimpanStok.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpanStok.Location = New System.Drawing.Point(35, 399)
        Me.BtnSimpanStok.Name = "BtnSimpanStok"
        Me.BtnSimpanStok.Size = New System.Drawing.Size(130, 32)
        Me.BtnSimpanStok.TabIndex = 168
        Me.BtnSimpanStok.Text = "SIMPAN (F8)"
        Me.BtnSimpanStok.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnSimpanStok.UseVisualStyleBackColor = False
        '
        'BtnKeluarStok
        '
        Me.BtnKeluarStok.BackColor = System.Drawing.Color.Yellow
        Me.BtnKeluarStok.FlatAppearance.BorderSize = 0
        Me.BtnKeluarStok.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GreenYellow
        Me.BtnKeluarStok.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GreenYellow
        Me.BtnKeluarStok.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluarStok.ForeColor = System.Drawing.Color.Black
        Me.BtnKeluarStok.Image = CType(resources.GetObject("BtnKeluarStok.Image"), System.Drawing.Image)
        Me.BtnKeluarStok.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnKeluarStok.Location = New System.Drawing.Point(289, 400)
        Me.BtnKeluarStok.Name = "BtnKeluarStok"
        Me.BtnKeluarStok.Size = New System.Drawing.Size(138, 31)
        Me.BtnKeluarStok.TabIndex = 169
        Me.BtnKeluarStok.Text = "KELUAR (Esc)"
        Me.BtnKeluarStok.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnKeluarStok.UseVisualStyleBackColor = False
        '
        'TxtNamaSupliyer
        '
        Me.TxtNamaSupliyer.BackColor = System.Drawing.Color.White
        Me.TxtNamaSupliyer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNamaSupliyer.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaSupliyer.Location = New System.Drawing.Point(86, 131)
        Me.TxtNamaSupliyer.Name = "TxtNamaSupliyer"
        Me.TxtNamaSupliyer.ReadOnly = True
        Me.TxtNamaSupliyer.Size = New System.Drawing.Size(248, 19)
        Me.TxtNamaSupliyer.TabIndex = 103
        '
        'TxtNamaKategori
        '
        Me.TxtNamaKategori.BackColor = System.Drawing.Color.White
        Me.TxtNamaKategori.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNamaKategori.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaKategori.Location = New System.Drawing.Point(86, 101)
        Me.TxtNamaKategori.Name = "TxtNamaKategori"
        Me.TxtNamaKategori.ReadOnly = True
        Me.TxtNamaKategori.Size = New System.Drawing.Size(248, 19)
        Me.TxtNamaKategori.TabIndex = 102
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(15, 131)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(64, 20)
        Me.Label8.TabIndex = 99
        Me.Label8.Text = "Supliyer"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(8, 101)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(71, 20)
        Me.Label3.TabIndex = 96
        Me.Label3.Text = "Kategori"
        '
        'TxtNama
        '
        Me.TxtNama.BackColor = System.Drawing.Color.White
        Me.TxtNama.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNama.Location = New System.Drawing.Point(86, 75)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.ReadOnly = True
        Me.TxtNama.Size = New System.Drawing.Size(333, 19)
        Me.TxtNama.TabIndex = 94
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(26, 74)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 20)
        Me.Label2.TabIndex = 92
        Me.Label2.Text = "Nama"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(31, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 20)
        Me.Label4.TabIndex = 91
        Me.Label4.Text = "Kode"
        '
        'TxtHargaBeli
        '
        Me.TxtHargaBeli.BackColor = System.Drawing.Color.White
        Me.TxtHargaBeli.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtHargaBeli.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaBeli.Location = New System.Drawing.Point(269, 49)
        Me.TxtHargaBeli.Name = "TxtHargaBeli"
        Me.TxtHargaBeli.ReadOnly = True
        Me.TxtHargaBeli.Size = New System.Drawing.Size(150, 19)
        Me.TxtHargaBeli.TabIndex = 90
        Me.TxtHargaBeli.Visible = False
        '
        'TxtKode
        '
        Me.TxtKode.BackColor = System.Drawing.Color.White
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtKode.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(86, 49)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.ReadOnly = True
        Me.TxtKode.Size = New System.Drawing.Size(177, 19)
        Me.TxtKode.TabIndex = 90
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(525, 221)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(336, 35)
        Me.ProgressBar1.TabIndex = 5
        '
        'LabelProgress
        '
        Me.LabelProgress.AutoSize = True
        Me.LabelProgress.BackColor = System.Drawing.Color.Transparent
        Me.LabelProgress.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelProgress.Location = New System.Drawing.Point(663, 230)
        Me.LabelProgress.Name = "LabelProgress"
        Me.LabelProgress.Size = New System.Drawing.Size(87, 16)
        Me.LabelProgress.TabIndex = 139
        Me.LabelProgress.Text = "0% Complete"
        '
        'PanelDetailBarang
        '
        Me.PanelDetailBarang.BackColor = System.Drawing.Color.White
        Me.PanelDetailBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelDetailBarang.Controls.Add(Me.Label96)
        Me.PanelDetailBarang.Controls.Add(Me.Label107)
        Me.PanelDetailBarang.Controls.Add(Me.BtnSembunyi)
        Me.PanelDetailBarang.Controls.Add(Me.Label94)
        Me.PanelDetailBarang.Controls.Add(Me.Label95)
        Me.PanelDetailBarang.Controls.Add(Me.Label97)
        Me.PanelDetailBarang.Controls.Add(Me.Label98)
        Me.PanelDetailBarang.Controls.Add(Me.Label99)
        Me.PanelDetailBarang.Controls.Add(Me.Label100)
        Me.PanelDetailBarang.Controls.Add(Me.Label101)
        Me.PanelDetailBarang.Controls.Add(Me.Label102)
        Me.PanelDetailBarang.Controls.Add(Me.Label103)
        Me.PanelDetailBarang.Controls.Add(Me.Label104)
        Me.PanelDetailBarang.Controls.Add(Me.Label105)
        Me.PanelDetailBarang.Controls.Add(Me.Label106)
        Me.PanelDetailBarang.Controls.Add(Me.Label59)
        Me.PanelDetailBarang.Controls.Add(Me.Label60)
        Me.PanelDetailBarang.Controls.Add(Me.Label55)
        Me.PanelDetailBarang.Controls.Add(Me.Label78)
        Me.PanelDetailBarang.Controls.Add(Me.Label50)
        Me.PanelDetailBarang.Controls.Add(Me.Label82)
        Me.PanelDetailBarang.Controls.Add(Me.Label56)
        Me.PanelDetailBarang.Controls.Add(Me.Label92)
        Me.PanelDetailBarang.Controls.Add(Me.Label51)
        Me.PanelDetailBarang.Controls.Add(Me.Label93)
        Me.PanelDetailBarang.Controls.Add(Me.Label52)
        Me.PanelDetailBarang.Controls.Add(Me.Label58)
        Me.PanelDetailBarang.Controls.Add(Me.Label91)
        Me.PanelDetailBarang.Controls.Add(Me.Label34)
        Me.PanelDetailBarang.Controls.Add(Me.Label47)
        Me.PanelDetailBarang.Controls.Add(Me.Label36)
        Me.PanelDetailBarang.Controls.Add(Me.Label88)
        Me.PanelDetailBarang.Controls.Add(Me.Label65)
        Me.PanelDetailBarang.Controls.Add(Me.Label70)
        Me.PanelDetailBarang.Controls.Add(Me.Label37)
        Me.PanelDetailBarang.Controls.Add(Me.Label87)
        Me.PanelDetailBarang.Controls.Add(Me.Label66)
        Me.PanelDetailBarang.Controls.Add(Me.Label90)
        Me.PanelDetailBarang.Controls.Add(Me.Label32)
        Me.PanelDetailBarang.Controls.Add(Me.Label46)
        Me.PanelDetailBarang.Controls.Add(Me.Label67)
        Me.PanelDetailBarang.Controls.Add(Me.Label63)
        Me.PanelDetailBarang.Controls.Add(Me.Label33)
        Me.PanelDetailBarang.Controls.Add(Me.Label86)
        Me.PanelDetailBarang.Controls.Add(Me.Label68)
        Me.PanelDetailBarang.Controls.Add(Me.Label69)
        Me.PanelDetailBarang.Controls.Add(Me.Label38)
        Me.PanelDetailBarang.Controls.Add(Me.Label85)
        Me.PanelDetailBarang.Controls.Add(Me.Label21)
        Me.PanelDetailBarang.Controls.Add(Me.Label44)
        Me.PanelDetailBarang.Controls.Add(Me.Label39)
        Me.PanelDetailBarang.Controls.Add(Me.Label81)
        Me.PanelDetailBarang.Controls.Add(Me.Label22)
        Me.PanelDetailBarang.Controls.Add(Me.Label62)
        Me.PanelDetailBarang.Controls.Add(Me.Label40)
        Me.PanelDetailBarang.Controls.Add(Me.Label77)
        Me.PanelDetailBarang.Controls.Add(Me.Label23)
        Me.PanelDetailBarang.Controls.Add(Me.Label76)
        Me.PanelDetailBarang.Controls.Add(Me.Label20)
        Me.PanelDetailBarang.Controls.Add(Me.Label41)
        Me.PanelDetailBarang.Controls.Add(Me.Label24)
        Me.PanelDetailBarang.Controls.Add(Me.Label43)
        Me.PanelDetailBarang.Controls.Add(Me.Label1)
        Me.PanelDetailBarang.Controls.Add(Me.Label75)
        Me.PanelDetailBarang.Controls.Add(Me.Label11)
        Me.PanelDetailBarang.Controls.Add(Me.Label89)
        Me.PanelDetailBarang.Controls.Add(Me.Label17)
        Me.PanelDetailBarang.Controls.Add(Me.Label45)
        Me.PanelDetailBarang.Controls.Add(Me.Label18)
        Me.PanelDetailBarang.Controls.Add(Me.Label19)
        Me.PanelDetailBarang.Controls.Add(Me.Label74)
        Me.PanelDetailBarang.Controls.Add(Me.Label31)
        Me.PanelDetailBarang.Controls.Add(Me.Label27)
        Me.PanelDetailBarang.Controls.Add(Me.Label64)
        Me.PanelDetailBarang.Controls.Add(Me.Label28)
        Me.PanelDetailBarang.Controls.Add(Me.Label73)
        Me.PanelDetailBarang.Controls.Add(Me.Label29)
        Me.PanelDetailBarang.Controls.Add(Me.Label61)
        Me.PanelDetailBarang.Controls.Add(Me.Label30)
        Me.PanelDetailBarang.Controls.Add(Me.Label72)
        Me.PanelDetailBarang.Controls.Add(Me.Label49)
        Me.PanelDetailBarang.Controls.Add(Me.Label79)
        Me.PanelDetailBarang.Controls.Add(Me.Label53)
        Me.PanelDetailBarang.Controls.Add(Me.Label42)
        Me.PanelDetailBarang.Controls.Add(Me.Label54)
        Me.PanelDetailBarang.Controls.Add(Me.Label71)
        Me.PanelDetailBarang.Controls.Add(Me.Label84)
        Me.PanelDetailBarang.Controls.Add(Me.Label80)
        Me.PanelDetailBarang.Controls.Add(Me.Label83)
        Me.PanelDetailBarang.Controls.Add(Me.Label25)
        Me.PanelDetailBarang.Controls.Add(Me.Label26)
        Me.PanelDetailBarang.Location = New System.Drawing.Point(471, 12)
        Me.PanelDetailBarang.Name = "PanelDetailBarang"
        Me.PanelDetailBarang.Size = New System.Drawing.Size(890, 344)
        Me.PanelDetailBarang.TabIndex = 4
        '
        'Label96
        '
        Me.Label96.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label96.Location = New System.Drawing.Point(209, 110)
        Me.Label96.Name = "Label96"
        Me.Label96.Size = New System.Drawing.Size(182, 20)
        Me.Label96.TabIndex = 234
        Me.Label96.Text = "Harga"
        Me.Label96.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label107
        '
        Me.Label107.AutoSize = True
        Me.Label107.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label107.Location = New System.Drawing.Point(72, 110)
        Me.Label107.Name = "Label107"
        Me.Label107.Size = New System.Drawing.Size(133, 17)
        Me.Label107.TabIndex = 233
        Me.Label107.Text = "Harga Beli terakhir :"
        '
        'BtnSembunyi
        '
        Me.BtnSembunyi.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnSembunyi.BackColor = System.Drawing.Color.Yellow
        Me.BtnSembunyi.FlatAppearance.BorderSize = 0
        Me.BtnSembunyi.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GreenYellow
        Me.BtnSembunyi.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GreenYellow
        Me.BtnSembunyi.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSembunyi.ForeColor = System.Drawing.Color.Black
        Me.BtnSembunyi.Image = CType(resources.GetObject("BtnSembunyi.Image"), System.Drawing.Image)
        Me.BtnSembunyi.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSembunyi.Location = New System.Drawing.Point(839, 2)
        Me.BtnSembunyi.Name = "BtnSembunyi"
        Me.BtnSembunyi.Size = New System.Drawing.Size(33, 33)
        Me.BtnSembunyi.TabIndex = 169
        Me.BtnSembunyi.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnSembunyi.UseVisualStyleBackColor = False
        '
        'Label94
        '
        Me.Label94.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label94.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label94.Location = New System.Drawing.Point(652, 295)
        Me.Label94.Name = "Label94"
        Me.Label94.Size = New System.Drawing.Size(26, 22)
        Me.Label94.TabIndex = 231
        Me.Label94.Text = "-"
        Me.Label94.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label95
        '
        Me.Label95.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label95.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label95.Location = New System.Drawing.Point(652, 274)
        Me.Label95.Name = "Label95"
        Me.Label95.Size = New System.Drawing.Size(26, 22)
        Me.Label95.TabIndex = 232
        Me.Label95.Text = "+"
        Me.Label95.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label97
        '
        Me.Label97.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label97.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label97.Location = New System.Drawing.Point(652, 253)
        Me.Label97.Name = "Label97"
        Me.Label97.Size = New System.Drawing.Size(26, 22)
        Me.Label97.TabIndex = 228
        Me.Label97.Text = "-"
        Me.Label97.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label98
        '
        Me.Label98.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label98.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label98.Location = New System.Drawing.Point(652, 232)
        Me.Label98.Name = "Label98"
        Me.Label98.Size = New System.Drawing.Size(26, 22)
        Me.Label98.TabIndex = 230
        Me.Label98.Text = "+"
        Me.Label98.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label99
        '
        Me.Label99.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label99.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label99.Location = New System.Drawing.Point(652, 148)
        Me.Label99.Name = "Label99"
        Me.Label99.Size = New System.Drawing.Size(26, 22)
        Me.Label99.TabIndex = 229
        Me.Label99.Text = "-"
        Me.Label99.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label100
        '
        Me.Label100.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label100.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label100.Location = New System.Drawing.Point(652, 211)
        Me.Label100.Name = "Label100"
        Me.Label100.Size = New System.Drawing.Size(26, 22)
        Me.Label100.TabIndex = 227
        Me.Label100.Text = "+"
        Me.Label100.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label101
        '
        Me.Label101.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label101.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label101.Location = New System.Drawing.Point(652, 127)
        Me.Label101.Name = "Label101"
        Me.Label101.Size = New System.Drawing.Size(26, 22)
        Me.Label101.TabIndex = 221
        Me.Label101.Text = "+"
        Me.Label101.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label102
        '
        Me.Label102.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label102.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label102.Location = New System.Drawing.Point(652, 190)
        Me.Label102.Name = "Label102"
        Me.Label102.Size = New System.Drawing.Size(26, 22)
        Me.Label102.TabIndex = 220
        Me.Label102.Text = "+"
        Me.Label102.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label103
        '
        Me.Label103.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label103.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label103.Location = New System.Drawing.Point(652, 106)
        Me.Label103.Name = "Label103"
        Me.Label103.Size = New System.Drawing.Size(26, 22)
        Me.Label103.TabIndex = 225
        Me.Label103.Text = "-"
        Me.Label103.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label104
        '
        Me.Label104.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label104.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label104.Location = New System.Drawing.Point(652, 169)
        Me.Label104.Name = "Label104"
        Me.Label104.Size = New System.Drawing.Size(26, 22)
        Me.Label104.TabIndex = 224
        Me.Label104.Text = "-"
        Me.Label104.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label105
        '
        Me.Label105.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label105.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label105.Location = New System.Drawing.Point(652, 64)
        Me.Label105.Name = "Label105"
        Me.Label105.Size = New System.Drawing.Size(26, 22)
        Me.Label105.TabIndex = 223
        Me.Label105.Text = "+"
        Me.Label105.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label106
        '
        Me.Label106.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label106.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label106.Location = New System.Drawing.Point(652, 85)
        Me.Label106.Name = "Label106"
        Me.Label106.Size = New System.Drawing.Size(26, 22)
        Me.Label106.TabIndex = 222
        Me.Label106.Text = "+"
        Me.Label106.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label59
        '
        Me.Label59.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label59.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label59.Location = New System.Drawing.Point(507, 295)
        Me.Label59.Name = "Label59"
        Me.Label59.Size = New System.Drawing.Size(146, 22)
        Me.Label59.TabIndex = 217
        Me.Label59.Text = "Transfer barang keluar"
        Me.Label59.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label60
        '
        Me.Label60.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label60.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label60.Location = New System.Drawing.Point(507, 274)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(146, 22)
        Me.Label60.TabIndex = 218
        Me.Label60.Text = "Transfer barang masuk"
        Me.Label60.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label55
        '
        Me.Label55.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label55.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label55.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label55.Location = New System.Drawing.Point(143, 279)
        Me.Label55.Name = "Label55"
        Me.Label55.Size = New System.Drawing.Size(120, 20)
        Me.Label55.TabIndex = 217
        Me.Label55.Text = "Kecil"
        Me.Label55.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label78
        '
        Me.Label78.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label78.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label78.Location = New System.Drawing.Point(775, 295)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(100, 22)
        Me.Label78.TabIndex = 216
        Me.Label78.Text = "Isi"
        Me.Label78.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label50
        '
        Me.Label50.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label50.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label50.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label50.Location = New System.Drawing.Point(262, 241)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(120, 20)
        Me.Label50.TabIndex = 218
        Me.Label50.Text = "Sedang"
        Me.Label50.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label82
        '
        Me.Label82.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label82.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label82.Location = New System.Drawing.Point(775, 274)
        Me.Label82.Name = "Label82"
        Me.Label82.Size = New System.Drawing.Size(100, 22)
        Me.Label82.TabIndex = 215
        Me.Label82.Text = "Isi"
        Me.Label82.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label56
        '
        Me.Label56.AutoSize = True
        Me.Label56.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label56.Location = New System.Drawing.Point(12, 279)
        Me.Label56.Name = "Label56"
        Me.Label56.Size = New System.Drawing.Size(128, 17)
        Me.Label56.TabIndex = 216
        Me.Label56.Text = "Harga Jual Partai :"
        Me.Label56.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label92
        '
        Me.Label92.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label92.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label92.Location = New System.Drawing.Point(676, 295)
        Me.Label92.Name = "Label92"
        Me.Label92.Size = New System.Drawing.Size(100, 22)
        Me.Label92.TabIndex = 213
        Me.Label92.Text = "Isi"
        Me.Label92.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label51
        '
        Me.Label51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label51.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label51.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label51.Location = New System.Drawing.Point(143, 241)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(120, 20)
        Me.Label51.TabIndex = 217
        Me.Label51.Text = "Kecil"
        Me.Label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label93
        '
        Me.Label93.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label93.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label93.Location = New System.Drawing.Point(676, 274)
        Me.Label93.Name = "Label93"
        Me.Label93.Size = New System.Drawing.Size(100, 22)
        Me.Label93.TabIndex = 214
        Me.Label93.Text = "Isi"
        Me.Label93.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label52
        '
        Me.Label52.AutoSize = True
        Me.Label52.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label52.Location = New System.Drawing.Point(8, 241)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(132, 17)
        Me.Label52.TabIndex = 216
        Me.Label52.Text = "Harga Jual Umum :"
        Me.Label52.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label58
        '
        Me.Label58.BackColor = System.Drawing.Color.Cornsilk
        Me.Label58.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label58.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label58.Location = New System.Drawing.Point(507, 42)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(171, 22)
        Me.Label58.TabIndex = 212
        Me.Label58.Text = "Jenis Trx"
        Me.Label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label91
        '
        Me.Label91.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label91.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label91.Location = New System.Drawing.Point(507, 317)
        Me.Label91.Name = "Label91"
        Me.Label91.Size = New System.Drawing.Size(171, 22)
        Me.Label91.TabIndex = 211
        Me.Label91.Text = "Stok Akhir"
        Me.Label91.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label34
        '
        Me.Label34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label34.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label34.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label34.Location = New System.Drawing.Point(381, 203)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(120, 20)
        Me.Label34.TabIndex = 188
        Me.Label34.Text = "Besar"
        Me.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label47
        '
        Me.Label47.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label47.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label47.Location = New System.Drawing.Point(507, 253)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(146, 22)
        Me.Label47.TabIndex = 211
        Me.Label47.Text = "Transfer stok keluar"
        Me.Label47.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label36
        '
        Me.Label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label36.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label36.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label36.Location = New System.Drawing.Point(262, 203)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(120, 20)
        Me.Label36.TabIndex = 187
        Me.Label36.Text = "Sedang"
        Me.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label88
        '
        Me.Label88.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label88.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label88.Location = New System.Drawing.Point(507, 232)
        Me.Label88.Name = "Label88"
        Me.Label88.Size = New System.Drawing.Size(146, 22)
        Me.Label88.TabIndex = 211
        Me.Label88.Text = "Transfer stok masuk"
        Me.Label88.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label65
        '
        Me.Label65.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label65.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label65.Location = New System.Drawing.Point(381, 260)
        Me.Label65.Name = "Label65"
        Me.Label65.Size = New System.Drawing.Size(120, 20)
        Me.Label65.TabIndex = 207
        Me.Label65.Text = "Besar"
        Me.Label65.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label70
        '
        Me.Label70.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label70.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label70.Location = New System.Drawing.Point(507, 148)
        Me.Label70.Name = "Label70"
        Me.Label70.Size = New System.Drawing.Size(146, 22)
        Me.Label70.TabIndex = 211
        Me.Label70.Text = "Penjualan"
        Me.Label70.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label37
        '
        Me.Label37.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label37.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label37.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label37.Location = New System.Drawing.Point(143, 203)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(120, 20)
        Me.Label37.TabIndex = 186
        Me.Label37.Text = "Kecil"
        Me.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label87
        '
        Me.Label87.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label87.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label87.Location = New System.Drawing.Point(507, 211)
        Me.Label87.Name = "Label87"
        Me.Label87.Size = New System.Drawing.Size(146, 22)
        Me.Label87.TabIndex = 211
        Me.Label87.Text = "Stok opname"
        Me.Label87.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label66
        '
        Me.Label66.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label66.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label66.Location = New System.Drawing.Point(262, 260)
        Me.Label66.Name = "Label66"
        Me.Label66.Size = New System.Drawing.Size(120, 20)
        Me.Label66.TabIndex = 206
        Me.Label66.Text = "Sedang"
        Me.Label66.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label90
        '
        Me.Label90.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label90.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label90.Location = New System.Drawing.Point(775, 317)
        Me.Label90.Name = "Label90"
        Me.Label90.Size = New System.Drawing.Size(100, 22)
        Me.Label90.TabIndex = 209
        Me.Label90.Text = "Isi"
        Me.Label90.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label32
        '
        Me.Label32.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label32.Location = New System.Drawing.Point(209, 89)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(182, 20)
        Me.Label32.TabIndex = 185
        Me.Label32.Text = "Harga"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label46
        '
        Me.Label46.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label46.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label46.Location = New System.Drawing.Point(775, 253)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(100, 22)
        Me.Label46.TabIndex = 209
        Me.Label46.Text = "Isi"
        Me.Label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label67
        '
        Me.Label67.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label67.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label67.Location = New System.Drawing.Point(143, 260)
        Me.Label67.Name = "Label67"
        Me.Label67.Size = New System.Drawing.Size(120, 20)
        Me.Label67.TabIndex = 205
        Me.Label67.Text = "Kecil"
        Me.Label67.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label63
        '
        Me.Label63.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label63.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label63.Location = New System.Drawing.Point(507, 127)
        Me.Label63.Name = "Label63"
        Me.Label63.Size = New System.Drawing.Size(146, 22)
        Me.Label63.TabIndex = 211
        Me.Label63.Text = "Pembelian"
        Me.Label63.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label33.Location = New System.Drawing.Point(32, 90)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(173, 17)
        Me.Label33.TabIndex = 184
        Me.Label33.Text = "Harga Pokok Pembelian :"
        '
        'Label86
        '
        Me.Label86.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label86.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label86.Location = New System.Drawing.Point(775, 232)
        Me.Label86.Name = "Label86"
        Me.Label86.Size = New System.Drawing.Size(100, 22)
        Me.Label86.TabIndex = 209
        Me.Label86.Text = "Isi"
        Me.Label86.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label68
        '
        Me.Label68.AutoSize = True
        Me.Label68.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label68.Location = New System.Drawing.Point(13, 260)
        Me.Label68.Name = "Label68"
        Me.Label68.Size = New System.Drawing.Size(127, 17)
        Me.Label68.TabIndex = 204
        Me.Label68.Text = "Satuan Partai (Isi) :"
        Me.Label68.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label69
        '
        Me.Label69.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label69.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label69.Location = New System.Drawing.Point(775, 148)
        Me.Label69.Name = "Label69"
        Me.Label69.Size = New System.Drawing.Size(100, 22)
        Me.Label69.TabIndex = 209
        Me.Label69.Text = "Isi"
        Me.Label69.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label38
        '
        Me.Label38.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label38.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label38.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label38.Location = New System.Drawing.Point(381, 222)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(120, 20)
        Me.Label38.TabIndex = 207
        Me.Label38.Text = "Besar"
        Me.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label85
        '
        Me.Label85.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label85.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label85.Location = New System.Drawing.Point(507, 190)
        Me.Label85.Name = "Label85"
        Me.Label85.Size = New System.Drawing.Size(146, 22)
        Me.Label85.TabIndex = 211
        Me.Label85.Text = "Retur penjualan"
        Me.Label85.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(70, 203)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(70, 17)
        Me.Label21.TabIndex = 182
        Me.Label21.Text = "Barcode :"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label44
        '
        Me.Label44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label44.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label44.Location = New System.Drawing.Point(507, 106)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(146, 22)
        Me.Label44.TabIndex = 211
        Me.Label44.Text = "Kurang stok"
        Me.Label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label39
        '
        Me.Label39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label39.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label39.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label39.Location = New System.Drawing.Point(262, 222)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(120, 20)
        Me.Label39.TabIndex = 206
        Me.Label39.Text = "Sedang"
        Me.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label81
        '
        Me.Label81.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label81.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label81.Location = New System.Drawing.Point(775, 211)
        Me.Label81.Name = "Label81"
        Me.Label81.Size = New System.Drawing.Size(100, 22)
        Me.Label81.TabIndex = 209
        Me.Label81.Text = "Isi"
        Me.Label81.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label22
        '
        Me.Label22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label22.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.ForeColor = System.Drawing.Color.Red
        Me.Label22.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label22.Location = New System.Drawing.Point(381, 183)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(120, 20)
        Me.Label22.TabIndex = 181
        Me.Label22.Text = "Satuan Besar"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label62
        '
        Me.Label62.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label62.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label62.Location = New System.Drawing.Point(775, 127)
        Me.Label62.Name = "Label62"
        Me.Label62.Size = New System.Drawing.Size(100, 22)
        Me.Label62.TabIndex = 209
        Me.Label62.Text = "Isi"
        Me.Label62.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label40
        '
        Me.Label40.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label40.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label40.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label40.Location = New System.Drawing.Point(143, 222)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(120, 20)
        Me.Label40.TabIndex = 205
        Me.Label40.Text = "Kecil"
        Me.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label77
        '
        Me.Label77.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label77.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label77.Location = New System.Drawing.Point(507, 169)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(146, 22)
        Me.Label77.TabIndex = 211
        Me.Label77.Text = "Retur pembelian"
        Me.Label77.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label23
        '
        Me.Label23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label23.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.ForeColor = System.Drawing.Color.Red
        Me.Label23.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label23.Location = New System.Drawing.Point(262, 183)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(120, 20)
        Me.Label23.TabIndex = 180
        Me.Label23.Text = "Satuan Sedang"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label76
        '
        Me.Label76.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label76.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label76.Location = New System.Drawing.Point(775, 190)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(100, 22)
        Me.Label76.TabIndex = 209
        Me.Label76.Text = "Isi"
        Me.Label76.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(9, 222)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(131, 17)
        Me.Label20.TabIndex = 204
        Me.Label20.Text = "Satuan Umum (Isi) :"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label41
        '
        Me.Label41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label41.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label41.Location = New System.Drawing.Point(507, 85)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(146, 22)
        Me.Label41.TabIndex = 211
        Me.Label41.Text = "Tambah stok"
        Me.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label24
        '
        Me.Label24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label24.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.ForeColor = System.Drawing.Color.Red
        Me.Label24.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label24.Location = New System.Drawing.Point(143, 183)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(120, 20)
        Me.Label24.TabIndex = 179
        Me.Label24.Text = "Satuan Kecil"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label43
        '
        Me.Label43.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label43.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label43.Location = New System.Drawing.Point(775, 106)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(100, 22)
        Me.Label43.TabIndex = 209
        Me.Label43.Text = "Isi"
        Me.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label1.Location = New System.Drawing.Point(209, 149)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(292, 20)
        Me.Label1.TabIndex = 175
        Me.Label1.Text = "Kode"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label75
        '
        Me.Label75.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label75.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label75.Location = New System.Drawing.Point(775, 169)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(100, 22)
        Me.Label75.TabIndex = 209
        Me.Label75.Text = "Isi"
        Me.Label75.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label11
        '
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label11.Location = New System.Drawing.Point(209, 130)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(292, 20)
        Me.Label11.TabIndex = 174
        Me.Label11.Text = "Kode"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label89
        '
        Me.Label89.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label89.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label89.Location = New System.Drawing.Point(676, 317)
        Me.Label89.Name = "Label89"
        Me.Label89.Size = New System.Drawing.Size(100, 22)
        Me.Label89.TabIndex = 205
        Me.Label89.Text = "Isi"
        Me.Label89.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label17
        '
        Me.Label17.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label17.Location = New System.Drawing.Point(209, 70)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(292, 20)
        Me.Label17.TabIndex = 173
        Me.Label17.Text = "Nama"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label45
        '
        Me.Label45.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label45.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label45.Location = New System.Drawing.Point(676, 253)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(100, 22)
        Me.Label45.TabIndex = 205
        Me.Label45.Text = "Isi"
        Me.Label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label18
        '
        Me.Label18.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label18.Location = New System.Drawing.Point(209, 51)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(292, 20)
        Me.Label18.TabIndex = 172
        Me.Label18.Text = "Kode"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label19
        '
        Me.Label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label19.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(507, 64)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(146, 22)
        Me.Label19.TabIndex = 211
        Me.Label19.Text = "Stok awal"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label74
        '
        Me.Label74.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label74.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label74.Location = New System.Drawing.Point(676, 232)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(100, 22)
        Me.Label74.TabIndex = 205
        Me.Label74.Text = "Isi"
        Me.Label74.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label31
        '
        Me.Label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label31.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label31.Location = New System.Drawing.Point(775, 85)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(100, 22)
        Me.Label31.TabIndex = 209
        Me.Label31.Text = "Isi"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label27.Location = New System.Drawing.Point(142, 150)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(63, 17)
        Me.Label27.TabIndex = 99
        Me.Label27.Text = "Supliyer :"
        '
        'Label64
        '
        Me.Label64.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label64.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label64.Location = New System.Drawing.Point(676, 148)
        Me.Label64.Name = "Label64"
        Me.Label64.Size = New System.Drawing.Size(100, 22)
        Me.Label64.TabIndex = 205
        Me.Label64.Text = "Isi"
        Me.Label64.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label28.Location = New System.Drawing.Point(134, 131)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(71, 17)
        Me.Label28.TabIndex = 96
        Me.Label28.Text = "Kategori :"
        '
        'Label73
        '
        Me.Label73.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label73.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label73.Location = New System.Drawing.Point(676, 211)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(100, 22)
        Me.Label73.TabIndex = 205
        Me.Label73.Text = "Isi"
        Me.Label73.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label29.Location = New System.Drawing.Point(148, 71)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(57, 17)
        Me.Label29.TabIndex = 92
        Me.Label29.Text = "Nama :"
        '
        'Label61
        '
        Me.Label61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label61.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label61.Location = New System.Drawing.Point(676, 127)
        Me.Label61.Name = "Label61"
        Me.Label61.Size = New System.Drawing.Size(100, 22)
        Me.Label61.TabIndex = 205
        Me.Label61.Text = "Isi"
        Me.Label61.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Font = New System.Drawing.Font("Century Gothic", 9.75!)
        Me.Label30.Location = New System.Drawing.Point(155, 52)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(50, 17)
        Me.Label30.TabIndex = 91
        Me.Label30.Text = "Kode :"
        '
        'Label72
        '
        Me.Label72.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label72.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label72.Location = New System.Drawing.Point(676, 190)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(100, 22)
        Me.Label72.TabIndex = 205
        Me.Label72.Text = "Isi"
        Me.Label72.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label49
        '
        Me.Label49.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label49.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label49.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label49.Location = New System.Drawing.Point(381, 241)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(120, 20)
        Me.Label49.TabIndex = 219
        Me.Label49.Text = "Besar"
        Me.Label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label79
        '
        Me.Label79.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label79.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label79.Location = New System.Drawing.Point(775, 64)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(100, 22)
        Me.Label79.TabIndex = 209
        Me.Label79.Text = "Isi"
        Me.Label79.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label53
        '
        Me.Label53.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label53.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label53.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label53.Location = New System.Drawing.Point(381, 279)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(120, 20)
        Me.Label53.TabIndex = 219
        Me.Label53.Text = "Besar"
        Me.Label53.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label42
        '
        Me.Label42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label42.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label42.Location = New System.Drawing.Point(676, 106)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(100, 22)
        Me.Label42.TabIndex = 205
        Me.Label42.Text = "Isi"
        Me.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label54
        '
        Me.Label54.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label54.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label54.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label54.Location = New System.Drawing.Point(262, 279)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(120, 20)
        Me.Label54.TabIndex = 218
        Me.Label54.Text = "Sedang"
        Me.Label54.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label71
        '
        Me.Label71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label71.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label71.Location = New System.Drawing.Point(676, 169)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(100, 22)
        Me.Label71.TabIndex = 205
        Me.Label71.Text = "Isi"
        Me.Label71.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label84
        '
        Me.Label84.BackColor = System.Drawing.Color.Cornsilk
        Me.Label84.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label84.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label84.Location = New System.Drawing.Point(676, 42)
        Me.Label84.Name = "Label84"
        Me.Label84.Size = New System.Drawing.Size(100, 22)
        Me.Label84.TabIndex = 204
        Me.Label84.Text = "Toko"
        Me.Label84.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label80
        '
        Me.Label80.BackColor = System.Drawing.Color.Cornsilk
        Me.Label80.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label80.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label80.Location = New System.Drawing.Point(775, 42)
        Me.Label80.Name = "Label80"
        Me.Label80.Size = New System.Drawing.Size(100, 22)
        Me.Label80.TabIndex = 208
        Me.Label80.Text = "Gudang"
        Me.Label80.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label83
        '
        Me.Label83.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label83.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label83.Location = New System.Drawing.Point(676, 64)
        Me.Label83.Name = "Label83"
        Me.Label83.Size = New System.Drawing.Size(100, 22)
        Me.Label83.TabIndex = 205
        Me.Label83.Text = "Isi"
        Me.Label83.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label25
        '
        Me.Label25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label25.Font = New System.Drawing.Font("Arial Narrow", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(676, 85)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(100, 22)
        Me.Label25.TabIndex = 205
        Me.Label25.Text = "Isi"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label26
        '
        Me.Label26.BackColor = System.Drawing.Color.PaleTurquoise
        Me.Label26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label26.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label26.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(0, 0)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(888, 38)
        Me.Label26.TabIndex = 170
        Me.Label26.Text = "Kode"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BarcodeToolStripMenuItem
        '
        Me.BarcodeToolStripMenuItem.Name = "BarcodeToolStripMenuItem"
        Me.BarcodeToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.BarcodeToolStripMenuItem.Text = "barcode"
        '
        'FormBarang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(1386, 554)
        Me.Controls.Add(Me.PanelDetailBarang)
        Me.Controls.Add(Me.Txtnamabarang)
        Me.Controls.Add(Me.Txtkodebarang)
        Me.Controls.Add(Me.PAnelTambahKurang)
        Me.Controls.Add(Me.LabelProgress)
        Me.Controls.Add(Me.PanelOperasi)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.DGBarang)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormBarang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Barang"
        CType(Me.DGBarang, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelOperasi.ResumeLayout(False)
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.PAnelTambahKurang.ResumeLayout(False)
        Me.PAnelTambahKurang.PerformLayout()
        Me.GBTambah.ResumeLayout(False)
        Me.GBTambah.PerformLayout()
        Me.GbStokSaatIni.ResumeLayout(False)
        Me.GbStokSaatIni.PerformLayout()
        Me.PanelDetailBarang.ResumeLayout(False)
        Me.PanelDetailBarang.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DGBarang As DataGridView
    Friend WithEvents PanelOperasi As System.Windows.Forms.Panel
    Friend WithEvents BtnKeluar As System.Windows.Forms.Button
    Friend WithEvents Txtkodebarang As System.Windows.Forms.TextBox
    Friend WithEvents TxtCari As System.Windows.Forms.TextBox
    Friend WithEvents BtnTambah As System.Windows.Forms.Button
    Friend WithEvents BtnUbah As System.Windows.Forms.Button
    Friend WithEvents BtnHapus As System.Windows.Forms.Button
    Friend WithEvents Txtnamabarang As System.Windows.Forms.TextBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents RefreshToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FilterBarangToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ByKodeToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ByNamaToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ByHargaBeliToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ByStokTokoToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ByStokGudangToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents TambahToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HapusStokToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents PerbaruiStokBarangToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents DetailStokToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents ExportDataBarangToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportDataBarangToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents CetakBarcodeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LabelBarangToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PAnelTambahKurang As Panel
    Friend WithEvents TxtNamaSupliyer As TextBox
    Friend WithEvents TxtNamaKategori As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtNama As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtKode As TextBox
    Friend WithEvents BtnSimpanStok As Button
    Friend WithEvents BtnKeluarStok As Button
    Friend WithEvents LblJudulStok As Label
    Friend WithEvents GBTambah As GroupBox
    Friend WithEvents TxtStokTotalGudangT As TextBox
    Friend WithEvents TxtStokTotalTokoT As TextBox
    Friend WithEvents TxtSatIsiGudangT As TextBox
    Friend WithEvents TxtSatIsiTokoT As TextBox
    Friend WithEvents CmbIsiGUdangT As ComboBox
    Friend WithEvents CmbIsiTokoT As ComboBox
    Friend WithEvents TxtIsiStokGudangT As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents TxtIsiStokTokoT As TextBox
    Friend WithEvents GbStokSaatIni As GroupBox
    Friend WithEvents TxtSatIsiGudang As TextBox
    Friend WithEvents TxtSatIsiToko As TextBox
    Friend WithEvents TxtIsiStokGudang As TextBox
    Friend WithEvents Label35 As Label
    Friend WithEvents TxtIsiStokToko As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents TxtSatuanGudang As TextBox
    Friend WithEvents TxtSatuanToko As TextBox
    Friend WithEvents KurangiStokToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TambahStokToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PerbaikiDatabase As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents LabelProgress As System.Windows.Forms.Label
    Friend WithEvents PanelDetailBarang As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents BtnSembunyi As System.Windows.Forms.Button
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents Label83 As System.Windows.Forms.Label
    Friend WithEvents Label84 As System.Windows.Forms.Label
    Friend WithEvents TxtHargaBeli As System.Windows.Forms.TextBox
    Friend WithEvents BtnAkhir As System.Windows.Forms.Button
    Friend WithEvents BtnNaik As System.Windows.Forms.Button
    Friend WithEvents BtnTurun As System.Windows.Forms.Button
    Friend WithEvents BtnAwal As System.Windows.Forms.Button
    Friend WithEvents ToolStripMenuItem6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents HargaBeliHargaJualUmumKecilToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UmumKecilToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UmumSedangToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UmumBesarToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PartaiKecilToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PartaiSedangToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PartaiBesarToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PanelCari As System.Windows.Forms.Panel
    Friend WithEvents BtnCari As System.Windows.Forms.Button
    Friend WithEvents Label91 As System.Windows.Forms.Label
    Friend WithEvents Label88 As System.Windows.Forms.Label
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents Label87 As System.Windows.Forms.Label
    Friend WithEvents Label90 As System.Windows.Forms.Label
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents Label86 As System.Windows.Forms.Label
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents Label85 As System.Windows.Forms.Label
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents Label89 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents Label82 As System.Windows.Forms.Label
    Friend WithEvents Label92 As System.Windows.Forms.Label
    Friend WithEvents Label93 As System.Windows.Forms.Label
    Friend WithEvents Label94 As System.Windows.Forms.Label
    Friend WithEvents Label95 As System.Windows.Forms.Label
    Friend WithEvents Label97 As System.Windows.Forms.Label
    Friend WithEvents Label98 As System.Windows.Forms.Label
    Friend WithEvents Label99 As System.Windows.Forms.Label
    Friend WithEvents Label100 As System.Windows.Forms.Label
    Friend WithEvents Label101 As System.Windows.Forms.Label
    Friend WithEvents Label102 As System.Windows.Forms.Label
    Friend WithEvents Label103 As System.Windows.Forms.Label
    Friend WithEvents Label104 As System.Windows.Forms.Label
    Friend WithEvents Label105 As System.Windows.Forms.Label
    Friend WithEvents Label106 As System.Windows.Forms.Label
    Friend WithEvents Label96 As System.Windows.Forms.Label
    Friend WithEvents Label107 As System.Windows.Forms.Label
    Friend WithEvents CetakLabelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CetakBarcodeToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents HistoriPembelianToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BarcodeToolStripMenuItem As ToolStripMenuItem
End Class
