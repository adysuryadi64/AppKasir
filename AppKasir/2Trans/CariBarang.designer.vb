<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CariBarang
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CariBarang))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblNama = New System.Windows.Forms.Label()
        Me.TxtStok = New System.Windows.Forms.TextBox()
        Me.TextCari = New System.Windows.Forms.TextBox()
        Me.TxtHargaJual = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.TxtHargabeli = New System.Windows.Forms.TextBox()
        Me.TxtIsiKecil = New System.Windows.Forms.TextBox()
        Me.CmbSatuanKecil = New System.Windows.Forms.ComboBox()
        Me.TxtQtyKecil = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblHarga = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.BtnTambah = New System.Windows.Forms.Button()
        Me.TxtJenisTransaksi = New System.Windows.Forms.TextBox()
        Me.DGCariBarang = New System.Windows.Forms.DataGridView()
        Me.TXtStokGudang = New System.Windows.Forms.TextBox()
        Me.TxtStokToko = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtQtySedang = New System.Windows.Forms.TextBox()
        Me.CmbSatuanSedang = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TxtQtyBesar = New System.Windows.Forms.TextBox()
        Me.CmbSatuanBesar = New System.Windows.Forms.ComboBox()
        Me.PanelCariNama = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.TxtIsiSedang = New System.Windows.Forms.TextBox()
        Me.TxtIsiBesar = New System.Windows.Forms.TextBox()
        Me.TxtHargaBesar = New System.Windows.Forms.TextBox()
        Me.TxtHargaSedang = New System.Windows.Forms.TextBox()
        Me.TxtHargaKecil = New System.Windows.Forms.TextBox()
        Me.PanelHeader.SuspendLayout()
        Me.Panel4.SuspendLayout()
        CType(Me.DGCariBarang, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCariNama.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(0, 34)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(3, 383)
        Me.Panel3.TabIndex = 48
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(1016, 34)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(3, 383)
        Me.Panel2.TabIndex = 47
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 417)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1019, 2)
        Me.Panel1.TabIndex = 46
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.PanelHeader.Controls.Add(Me.Panel4)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1019, 34)
        Me.PanelHeader.TabIndex = 45
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.SaddleBrown
        Me.Panel4.Controls.Add(Me.BtnClose)
        Me.Panel4.Controls.Add(Me.LblNama)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.ForeColor = System.Drawing.Color.Black
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1019, 34)
        Me.Panel4.TabIndex = 2
        '
        'BtnClose
        '
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.White
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnClose.Location = New System.Drawing.Point(936, 4)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(78, 26)
        Me.BtnClose.TabIndex = 1
        Me.BtnClose.Text = "Esc"
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'LblNama
        '
        Me.LblNama.AutoSize = True
        Me.LblNama.BackColor = System.Drawing.Color.Transparent
        Me.LblNama.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblNama.ForeColor = System.Drawing.Color.White
        Me.LblNama.Location = New System.Drawing.Point(414, 6)
        Me.LblNama.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.LblNama.Name = "LblNama"
        Me.LblNama.Size = New System.Drawing.Size(190, 23)
        Me.LblNama.TabIndex = 20
        Me.LblNama.Text = "C A R I   B A R A N G"
        '
        'TxtStok
        '
        Me.TxtStok.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStok.Location = New System.Drawing.Point(492, 288)
        Me.TxtStok.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtStok.Name = "TxtStok"
        Me.TxtStok.ReadOnly = True
        Me.TxtStok.Size = New System.Drawing.Size(55, 22)
        Me.TxtStok.TabIndex = 16
        Me.TxtStok.Text = "TxtStok"
        Me.TxtStok.Visible = False
        '
        'TextCari
        '
        Me.TextCari.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextCari.Location = New System.Drawing.Point(9, 7)
        Me.TextCari.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TextCari.Name = "TextCari"
        Me.TextCari.Size = New System.Drawing.Size(968, 21)
        Me.TextCari.TabIndex = 0
        '
        'TxtHargaJual
        '
        Me.TxtHargaJual.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaJual.Location = New System.Drawing.Point(71, 392)
        Me.TxtHargaJual.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtHargaJual.Name = "TxtHargaJual"
        Me.TxtHargaJual.ReadOnly = True
        Me.TxtHargaJual.Size = New System.Drawing.Size(91, 21)
        Me.TxtHargaJual.TabIndex = 9
        Me.TxtHargaJual.Text = "TxtHargaJual"
        Me.TxtHargaJual.Visible = False
        '
        'TxtKode
        '
        Me.TxtKode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(71, 354)
        Me.TxtKode.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.ReadOnly = True
        Me.TxtKode.Size = New System.Drawing.Size(91, 21)
        Me.TxtKode.TabIndex = 7
        Me.TxtKode.Text = "TxtKode"
        Me.TxtKode.Visible = False
        '
        'TxtNama
        '
        Me.TxtNama.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNama.Location = New System.Drawing.Point(219, 354)
        Me.TxtNama.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.ReadOnly = True
        Me.TxtNama.Size = New System.Drawing.Size(728, 21)
        Me.TxtNama.TabIndex = 8
        Me.TxtNama.Text = "TxtNama"
        '
        'TxtHargabeli
        '
        Me.TxtHargabeli.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargabeli.Location = New System.Drawing.Point(71, 372)
        Me.TxtHargabeli.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtHargabeli.Name = "TxtHargabeli"
        Me.TxtHargabeli.ReadOnly = True
        Me.TxtHargabeli.Size = New System.Drawing.Size(91, 22)
        Me.TxtHargabeli.TabIndex = 11
        Me.TxtHargabeli.Text = "TxtHargabeli"
        Me.TxtHargabeli.Visible = False
        '
        'TxtIsiKecil
        '
        Me.TxtIsiKecil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiKecil.Location = New System.Drawing.Point(337, 325)
        Me.TxtIsiKecil.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtIsiKecil.Name = "TxtIsiKecil"
        Me.TxtIsiKecil.ReadOnly = True
        Me.TxtIsiKecil.Size = New System.Drawing.Size(28, 22)
        Me.TxtIsiKecil.TabIndex = 12
        Me.TxtIsiKecil.Text = "Isi1"
        Me.TxtIsiKecil.Visible = False
        '
        'CmbSatuanKecil
        '
        Me.CmbSatuanKecil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSatuanKecil.Enabled = False
        Me.CmbSatuanKecil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSatuanKecil.FormattingEnabled = True
        Me.CmbSatuanKecil.Location = New System.Drawing.Point(268, 391)
        Me.CmbSatuanKecil.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.CmbSatuanKecil.Name = "CmbSatuanKecil"
        Me.CmbSatuanKecil.Size = New System.Drawing.Size(93, 23)
        Me.CmbSatuanKecil.TabIndex = 3
        '
        'TxtQtyKecil
        '
        Me.TxtQtyKecil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQtyKecil.Location = New System.Drawing.Point(216, 392)
        Me.TxtQtyKecil.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtQtyKecil.Name = "TxtQtyKecil"
        Me.TxtQtyKecil.Size = New System.Drawing.Size(51, 21)
        Me.TxtQtyKecil.TabIndex = 3
        Me.TxtQtyKecil.Text = "Qk"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(10, 357)
        Me.Label3.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 15)
        Me.Label3.TabIndex = 65
        Me.Label3.Text = "Kode"
        Me.Label3.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(175, 357)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 15)
        Me.Label1.TabIndex = 66
        Me.Label1.Text = "Nama"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(264, 375)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 15)
        Me.Label2.TabIndex = 67
        Me.Label2.Text = "Satuan kecil"
        '
        'LblHarga
        '
        Me.LblHarga.AutoSize = True
        Me.LblHarga.BackColor = System.Drawing.Color.Transparent
        Me.LblHarga.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHarga.ForeColor = System.Drawing.Color.Black
        Me.LblHarga.Location = New System.Drawing.Point(10, 394)
        Me.LblHarga.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.LblHarga.Name = "LblHarga"
        Me.LblHarga.Size = New System.Drawing.Size(41, 15)
        Me.LblHarga.TabIndex = 68
        Me.LblHarga.Text = "Harga"
        Me.LblHarga.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(216, 375)
        Me.Label5.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(24, 15)
        Me.Label5.TabIndex = 69
        Me.Label5.Text = "Qty"
        '
        'BtnTambah
        '
        Me.BtnTambah.BackColor = System.Drawing.Color.FromArgb(CType(CType(36, Byte), Integer), CType(CType(91, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.BtnTambah.FlatAppearance.BorderSize = 0
        Me.BtnTambah.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.BtnTambah.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green
        Me.BtnTambah.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTambah.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTambah.ForeColor = System.Drawing.Color.White
        Me.BtnTambah.Image = CType(resources.GetObject("BtnTambah.Image"), System.Drawing.Image)
        Me.BtnTambah.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTambah.Location = New System.Drawing.Point(849, 380)
        Me.BtnTambah.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.BtnTambah.Name = "BtnTambah"
        Me.BtnTambah.Size = New System.Drawing.Size(151, 33)
        Me.BtnTambah.TabIndex = 6
        Me.BtnTambah.Text = "   TAMBAH (F10)"
        Me.BtnTambah.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTambah.UseVisualStyleBackColor = False
        '
        'TxtJenisTransaksi
        '
        Me.TxtJenisTransaksi.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtJenisTransaksi.Location = New System.Drawing.Point(71, 311)
        Me.TxtJenisTransaksi.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtJenisTransaksi.Name = "TxtJenisTransaksi"
        Me.TxtJenisTransaksi.ReadOnly = True
        Me.TxtJenisTransaksi.Size = New System.Drawing.Size(97, 22)
        Me.TxtJenisTransaksi.TabIndex = 10
        Me.TxtJenisTransaksi.Text = "jenistransaksi"
        Me.TxtJenisTransaksi.Visible = False
        '
        'DGCariBarang
        '
        Me.DGCariBarang.AllowUserToAddRows = False
        Me.DGCariBarang.AllowUserToDeleteRows = False
        Me.DGCariBarang.AllowUserToOrderColumns = True
        Me.DGCariBarang.AllowUserToResizeColumns = False
        Me.DGCariBarang.AllowUserToResizeRows = False
        Me.DGCariBarang.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGCariBarang.BackgroundColor = System.Drawing.Color.White
        Me.DGCariBarang.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGCariBarang.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DGCariBarang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGCariBarang.Location = New System.Drawing.Point(3, 66)
        Me.DGCariBarang.Margin = New System.Windows.Forms.Padding(5, 8, 5, 8)
        Me.DGCariBarang.Name = "DGCariBarang"
        Me.DGCariBarang.ReadOnly = True
        Me.DGCariBarang.RowHeadersVisible = False
        Me.DGCariBarang.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGCariBarang.Size = New System.Drawing.Size(1011, 284)
        Me.DGCariBarang.TabIndex = 2
        '
        'TXtStokGudang
        '
        Me.TXtStokGudang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TXtStokGudang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TXtStokGudang.Location = New System.Drawing.Point(417, 288)
        Me.TXtStokGudang.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TXtStokGudang.Name = "TXtStokGudang"
        Me.TXtStokGudang.ReadOnly = True
        Me.TXtStokGudang.Size = New System.Drawing.Size(62, 22)
        Me.TXtStokGudang.TabIndex = 15
        Me.TXtStokGudang.Text = "SGudang"
        Me.TXtStokGudang.Visible = False
        '
        'TxtStokToko
        '
        Me.TxtStokToko.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStokToko.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStokToko.Location = New System.Drawing.Point(357, 288)
        Me.TxtStokToko.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtStokToko.Name = "TxtStokToko"
        Me.TxtStokToko.ReadOnly = True
        Me.TxtStokToko.Size = New System.Drawing.Size(50, 22)
        Me.TxtStokToko.TabIndex = 14
        Me.TxtStokToko.Text = "SToko"
        Me.TxtStokToko.Visible = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(383, 375)
        Me.Label6.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(24, 15)
        Me.Label6.TabIndex = 143
        Me.Label6.Text = "Qty"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(430, 375)
        Me.Label7.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(90, 15)
        Me.Label7.TabIndex = 142
        Me.Label7.Text = "Satuan sedang"
        '
        'TxtQtySedang
        '
        Me.TxtQtySedang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQtySedang.Location = New System.Drawing.Point(383, 392)
        Me.TxtQtySedang.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtQtySedang.Name = "TxtQtySedang"
        Me.TxtQtySedang.Size = New System.Drawing.Size(45, 21)
        Me.TxtQtySedang.TabIndex = 4
        Me.TxtQtySedang.Text = "Qs"
        '
        'CmbSatuanSedang
        '
        Me.CmbSatuanSedang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSatuanSedang.Enabled = False
        Me.CmbSatuanSedang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSatuanSedang.FormattingEnabled = True
        Me.CmbSatuanSedang.Location = New System.Drawing.Point(430, 391)
        Me.CmbSatuanSedang.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.CmbSatuanSedang.Name = "CmbSatuanSedang"
        Me.CmbSatuanSedang.Size = New System.Drawing.Size(117, 23)
        Me.CmbSatuanSedang.TabIndex = 141
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(579, 375)
        Me.Label8.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 15)
        Me.Label8.TabIndex = 147
        Me.Label8.Text = "Qty"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(625, 375)
        Me.Label9.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(80, 15)
        Me.Label9.TabIndex = 146
        Me.Label9.Text = "Satuan besar"
        '
        'TxtQtyBesar
        '
        Me.TxtQtyBesar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQtyBesar.Location = New System.Drawing.Point(579, 392)
        Me.TxtQtyBesar.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtQtyBesar.Name = "TxtQtyBesar"
        Me.TxtQtyBesar.Size = New System.Drawing.Size(45, 21)
        Me.TxtQtyBesar.TabIndex = 5
        Me.TxtQtyBesar.Text = "Qb"
        '
        'CmbSatuanBesar
        '
        Me.CmbSatuanBesar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbSatuanBesar.Enabled = False
        Me.CmbSatuanBesar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmbSatuanBesar.FormattingEnabled = True
        Me.CmbSatuanBesar.Location = New System.Drawing.Point(625, 391)
        Me.CmbSatuanBesar.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.CmbSatuanBesar.Name = "CmbSatuanBesar"
        Me.CmbSatuanBesar.Size = New System.Drawing.Size(105, 23)
        Me.CmbSatuanBesar.TabIndex = 145
        '
        'PanelCariNama
        '
        Me.PanelCariNama.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCariNama.Controls.Add(Me.BtnCari)
        Me.PanelCariNama.Controls.Add(Me.TextCari)
        Me.PanelCariNama.Location = New System.Drawing.Point(3, 34)
        Me.PanelCariNama.Name = "PanelCariNama"
        Me.PanelCariNama.Size = New System.Drawing.Size(1011, 34)
        Me.PanelCariNama.TabIndex = 1
        '
        'BtnCari
        '
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.Location = New System.Drawing.Point(971, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(26, 24)
        Me.BtnCari.TabIndex = 2
        Me.BtnCari.UseVisualStyleBackColor = True
        '
        'TxtIsiSedang
        '
        Me.TxtIsiSedang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiSedang.Location = New System.Drawing.Point(553, 323)
        Me.TxtIsiSedang.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtIsiSedang.Name = "TxtIsiSedang"
        Me.TxtIsiSedang.ReadOnly = True
        Me.TxtIsiSedang.Size = New System.Drawing.Size(35, 22)
        Me.TxtIsiSedang.TabIndex = 148
        Me.TxtIsiSedang.Text = "ISi2"
        Me.TxtIsiSedang.Visible = False
        '
        'TxtIsiBesar
        '
        Me.TxtIsiBesar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsiBesar.Location = New System.Drawing.Point(771, 324)
        Me.TxtIsiBesar.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtIsiBesar.Name = "TxtIsiBesar"
        Me.TxtIsiBesar.ReadOnly = True
        Me.TxtIsiBesar.Size = New System.Drawing.Size(25, 22)
        Me.TxtIsiBesar.TabIndex = 149
        Me.TxtIsiBesar.Text = "Isi3"
        Me.TxtIsiBesar.Visible = False
        '
        'TxtHargaBesar
        '
        Me.TxtHargaBesar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaBesar.Location = New System.Drawing.Point(807, 324)
        Me.TxtHargaBesar.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtHargaBesar.Name = "TxtHargaBesar"
        Me.TxtHargaBesar.ReadOnly = True
        Me.TxtHargaBesar.Size = New System.Drawing.Size(25, 22)
        Me.TxtHargaBesar.TabIndex = 152
        Me.TxtHargaBesar.Text = "Isi3"
        Me.TxtHargaBesar.Visible = False
        '
        'TxtHargaSedang
        '
        Me.TxtHargaSedang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaSedang.Location = New System.Drawing.Point(589, 323)
        Me.TxtHargaSedang.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtHargaSedang.Name = "TxtHargaSedang"
        Me.TxtHargaSedang.ReadOnly = True
        Me.TxtHargaSedang.Size = New System.Drawing.Size(35, 22)
        Me.TxtHargaSedang.TabIndex = 151
        Me.TxtHargaSedang.Text = "ISi2"
        Me.TxtHargaSedang.Visible = False
        '
        'TxtHargaKecil
        '
        Me.TxtHargaKecil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHargaKecil.Location = New System.Drawing.Point(373, 325)
        Me.TxtHargaKecil.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.TxtHargaKecil.Name = "TxtHargaKecil"
        Me.TxtHargaKecil.ReadOnly = True
        Me.TxtHargaKecil.Size = New System.Drawing.Size(28, 22)
        Me.TxtHargaKecil.TabIndex = 150
        Me.TxtHargaKecil.Text = "Isi1"
        Me.TxtHargaKecil.Visible = False
        '
        'CariBarang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Chocolate
        Me.ClientSize = New System.Drawing.Size(1019, 419)
        Me.Controls.Add(Me.TxtHargaBesar)
        Me.Controls.Add(Me.TxtHargaSedang)
        Me.Controls.Add(Me.TxtHargaKecil)
        Me.Controls.Add(Me.TxtIsiBesar)
        Me.Controls.Add(Me.TxtIsiSedang)
        Me.Controls.Add(Me.PanelCariNama)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.TxtQtyBesar)
        Me.Controls.Add(Me.CmbSatuanBesar)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.TxtQtySedang)
        Me.Controls.Add(Me.CmbSatuanSedang)
        Me.Controls.Add(Me.TXtStokGudang)
        Me.Controls.Add(Me.TxtStokToko)
        Me.Controls.Add(Me.TxtJenisTransaksi)
        Me.Controls.Add(Me.BtnTambah)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.LblHarga)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtQtyKecil)
        Me.Controls.Add(Me.CmbSatuanKecil)
        Me.Controls.Add(Me.TxtIsiKecil)
        Me.Controls.Add(Me.TxtKode)
        Me.Controls.Add(Me.TxtNama)
        Me.Controls.Add(Me.TxtHargabeli)
        Me.Controls.Add(Me.TxtHargaJual)
        Me.Controls.Add(Me.TxtStok)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.DGCariBarang)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Name = "CariBarang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.PanelHeader.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        CType(Me.DGCariBarang, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCariNama.ResumeLayout(False)
        Me.PanelCariNama.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents TxtStok As System.Windows.Forms.TextBox
    Friend WithEvents TextCari As System.Windows.Forms.TextBox
    Friend WithEvents TxtHargaJual As System.Windows.Forms.TextBox
    Friend WithEvents TxtKode As System.Windows.Forms.TextBox
    Friend WithEvents TxtNama As System.Windows.Forms.TextBox
    Friend WithEvents TxtHargabeli As System.Windows.Forms.TextBox
    Friend WithEvents TxtIsiKecil As System.Windows.Forms.TextBox
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents LblNama As System.Windows.Forms.Label
    Friend WithEvents CmbSatuanKecil As System.Windows.Forms.ComboBox
    Friend WithEvents TxtQtyKecil As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents LblHarga As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents BtnTambah As System.Windows.Forms.Button
    Friend WithEvents TxtJenisTransaksi As System.Windows.Forms.TextBox
    Friend WithEvents DGCariBarang As System.Windows.Forms.DataGridView
    Friend WithEvents TXtStokGudang As System.Windows.Forms.TextBox
    Friend WithEvents TxtStokToko As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtQtySedang As System.Windows.Forms.TextBox
    Friend WithEvents CmbSatuanSedang As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents TxtQtyBesar As System.Windows.Forms.TextBox
    Friend WithEvents CmbSatuanBesar As System.Windows.Forms.ComboBox
    Friend WithEvents PanelCariNama As System.Windows.Forms.Panel
    Friend WithEvents BtnCari As System.Windows.Forms.Button
    Friend WithEvents TxtIsiSedang As TextBox
    Friend WithEvents TxtIsiBesar As TextBox
    Friend WithEvents TxtHargaBesar As TextBox
    Friend WithEvents TxtHargaSedang As TextBox
    Friend WithEvents TxtHargaKecil As TextBox
End Class
