<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormReturBeli
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormReturBeli))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblUtama = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TxtStok = New System.Windows.Forms.TextBox()
        Me.PanelCariNama = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.TxtNama = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Txtlihattotal = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.LblTujuanTransfer = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblJenisTrans = New System.Windows.Forms.Label()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.LblLokasiBarang = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtBarcode = New System.Windows.Forms.TextBox()
        Me.TxtHarga = New System.Windows.Forms.TextBox()
        Me.TxtIsi = New System.Windows.Forms.TextBox()
        Me.Txtsatuan = New System.Windows.Forms.TextBox()
        Me.TxtQty = New System.Windows.Forms.TextBox()
        Me.TxtKode = New System.Windows.Forms.TextBox()
        Me.LstBarang = New System.Windows.Forms.ListBox()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.Id = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Nama = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hargabeli = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Qty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Satuan = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Isi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HargaBeliSat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QtySat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Totalharga = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Stok = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblRecord = New System.Windows.Forms.Label()
        Me.BtnKeluar = New System.Windows.Forms.Button()
        Me.TxtKomputer = New System.Windows.Forms.TextBox()
        Me.TxtLogin = New System.Windows.Forms.TextBox()
        Me.BtnSimpann = New System.Windows.Forms.Button()
        Me.TxtGrandtotal = New System.Windows.Forms.TextBox()
        Me.TxtTotalQTY = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.HapusToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel4.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.PanelCariNama.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.SandyBrown
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel4.Controls.Add(Me.BtnClose)
        Me.Panel4.Controls.Add(Me.LblUtama)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1189, 39)
        Me.Panel4.TabIndex = 136
        '
        'BtnClose
        '
        Me.BtnClose.AllowDrop = True
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Transparent
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red
        Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Font = New System.Drawing.Font("Century Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.White
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(1150, 4)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(31, 28)
        Me.BtnClose.TabIndex = 140
        Me.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblUtama
        '
        Me.LblUtama.AutoSize = True
        Me.LblUtama.Font = New System.Drawing.Font("Arial Black", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUtama.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.LblUtama.Location = New System.Drawing.Point(10, 3)
        Me.LblUtama.Name = "LblUtama"
        Me.LblUtama.Size = New System.Drawing.Size(567, 30)
        Me.LblUtama.TabIndex = 1
        Me.LblUtama.Text = "TRANSFER STOK DARI TOKO KE GUDANG BOSS"
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.GroupBox1.Controls.Add(Me.TxtStok)
        Me.GroupBox1.Controls.Add(Me.PanelCariNama)
        Me.GroupBox1.Controls.Add(Me.GroupBox3)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Controls.Add(Me.TxtBarcode)
        Me.GroupBox1.Controls.Add(Me.TxtHarga)
        Me.GroupBox1.Controls.Add(Me.TxtIsi)
        Me.GroupBox1.Controls.Add(Me.Txtsatuan)
        Me.GroupBox1.Controls.Add(Me.TxtQty)
        Me.GroupBox1.Controls.Add(Me.TxtKode)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(0, 39)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1189, 142)
        Me.GroupBox1.TabIndex = 137
        Me.GroupBox1.TabStop = False
        '
        'TxtStok
        '
        Me.TxtStok.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtStok.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtStok.Location = New System.Drawing.Point(919, 109)
        Me.TxtStok.Name = "TxtStok"
        Me.TxtStok.ReadOnly = True
        Me.TxtStok.Size = New System.Drawing.Size(64, 22)
        Me.TxtStok.TabIndex = 9
        Me.TxtStok.Text = "Stok"
        Me.TxtStok.Visible = False
        '
        'PanelCariNama
        '
        Me.PanelCariNama.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCariNama.Controls.Add(Me.BtnCari)
        Me.PanelCariNama.Controls.Add(Me.TxtNama)
        Me.PanelCariNama.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelCariNama.Location = New System.Drawing.Point(3, 102)
        Me.PanelCariNama.Name = "PanelCariNama"
        Me.PanelCariNama.Size = New System.Drawing.Size(562, 36)
        Me.PanelCariNama.TabIndex = 1
        '
        'BtnCari
        '
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.Location = New System.Drawing.Point(534, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(26, 26)
        Me.BtnCari.TabIndex = 2
        Me.BtnCari.UseVisualStyleBackColor = True
        '
        'TxtNama
        '
        Me.TxtNama.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtNama.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNama.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNama.Location = New System.Drawing.Point(3, 5)
        Me.TxtNama.Name = "TxtNama"
        Me.TxtNama.Size = New System.Drawing.Size(533, 26)
        Me.TxtNama.TabIndex = 1
        Me.TxtNama.Text = "Nama"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GroupBox3.Controls.Add(Me.Txtlihattotal)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Black
        Me.GroupBox3.Location = New System.Drawing.Point(568, 16)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(618, 87)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Grand Total"
        '
        'Txtlihattotal
        '
        Me.Txtlihattotal.BackColor = System.Drawing.Color.Black
        Me.Txtlihattotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Txtlihattotal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Txtlihattotal.Font = New System.Drawing.Font("Century", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Txtlihattotal.ForeColor = System.Drawing.Color.Lime
        Me.Txtlihattotal.Location = New System.Drawing.Point(3, 25)
        Me.Txtlihattotal.Multiline = True
        Me.Txtlihattotal.Name = "Txtlihattotal"
        Me.Txtlihattotal.ReadOnly = True
        Me.Txtlihattotal.Size = New System.Drawing.Size(612, 59)
        Me.Txtlihattotal.TabIndex = 8
        Me.Txtlihattotal.Text = "000"
        Me.Txtlihattotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.LightSkyBlue
        Me.GroupBox2.Controls.Add(Me.LblTujuanTransfer)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.LblJenisTrans)
        Me.GroupBox2.Controls.Add(Me.DTPTgl)
        Me.GroupBox2.Controls.Add(Me.LblLokasiBarang)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.TxtFaktur)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Location = New System.Drawing.Point(3, 16)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(562, 87)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        '
        'LblTujuanTransfer
        '
        Me.LblTujuanTransfer.BackColor = System.Drawing.Color.Transparent
        Me.LblTujuanTransfer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTujuanTransfer.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTujuanTransfer.ForeColor = System.Drawing.Color.Black
        Me.LblTujuanTransfer.Location = New System.Drawing.Point(434, 39)
        Me.LblTujuanTransfer.Name = "LblTujuanTransfer"
        Me.LblTujuanTransfer.Size = New System.Drawing.Size(112, 21)
        Me.LblTujuanTransfer.TabIndex = 123
        Me.LblTujuanTransfer.Text = "0"
        Me.LblTujuanTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(286, 39)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(95, 16)
        Me.Label4.TabIndex = 122
        Me.Label4.Text = "Stok masuk ke"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblJenisTrans
        '
        Me.LblJenisTrans.BackColor = System.Drawing.Color.Transparent
        Me.LblJenisTrans.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblJenisTrans.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJenisTrans.ForeColor = System.Drawing.Color.Black
        Me.LblJenisTrans.Location = New System.Drawing.Point(6, 62)
        Me.LblJenisTrans.Name = "LblJenisTrans"
        Me.LblJenisTrans.Size = New System.Drawing.Size(242, 21)
        Me.LblJenisTrans.TabIndex = 121
        Me.LblJenisTrans.Text = "TambahTransfer"
        Me.LblJenisTrans.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.LblJenisTrans.Visible = False
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(70, 36)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(178, 22)
        Me.DTPTgl.TabIndex = 9
        '
        'LblLokasiBarang
        '
        Me.LblLokasiBarang.BackColor = System.Drawing.Color.Transparent
        Me.LblLokasiBarang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblLokasiBarang.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLokasiBarang.ForeColor = System.Drawing.Color.Black
        Me.LblLokasiBarang.Location = New System.Drawing.Point(434, 13)
        Me.LblLokasiBarang.Name = "LblLokasiBarang"
        Me.LblLokasiBarang.Size = New System.Drawing.Size(112, 21)
        Me.LblLokasiBarang.TabIndex = 120
        Me.LblLokasiBarang.Text = "0"
        Me.LblLokasiBarang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(286, 13)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 16)
        Me.Label6.TabIndex = 120
        Me.Label6.Text = "Stok keluar dari"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.ForeColor = System.Drawing.Color.Black
        Me.TxtFaktur.Location = New System.Drawing.Point(70, 10)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.ReadOnly = True
        Me.TxtFaktur.Size = New System.Drawing.Size(178, 22)
        Me.TxtFaktur.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(6, 39)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 16)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Tanggal"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 16)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Nomor"
        '
        'TxtBarcode
        '
        Me.TxtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtBarcode.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBarcode.Location = New System.Drawing.Point(1065, 110)
        Me.TxtBarcode.Name = "TxtBarcode"
        Me.TxtBarcode.ReadOnly = True
        Me.TxtBarcode.Size = New System.Drawing.Size(139, 26)
        Me.TxtBarcode.TabIndex = 8
        Me.TxtBarcode.Text = "Barcode"
        Me.TxtBarcode.Visible = False
        '
        'TxtHarga
        '
        Me.TxtHarga.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtHarga.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtHarga.Location = New System.Drawing.Point(849, 110)
        Me.TxtHarga.Name = "TxtHarga"
        Me.TxtHarga.ReadOnly = True
        Me.TxtHarga.Size = New System.Drawing.Size(64, 22)
        Me.TxtHarga.TabIndex = 8
        Me.TxtHarga.Text = "Harga"
        Me.TxtHarga.Visible = False
        '
        'TxtIsi
        '
        Me.TxtIsi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIsi.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIsi.Location = New System.Drawing.Point(781, 110)
        Me.TxtIsi.Name = "TxtIsi"
        Me.TxtIsi.ReadOnly = True
        Me.TxtIsi.Size = New System.Drawing.Size(64, 22)
        Me.TxtIsi.TabIndex = 8
        Me.TxtIsi.Text = "Isi"
        Me.TxtIsi.Visible = False
        '
        'Txtsatuan
        '
        Me.Txtsatuan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Txtsatuan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Txtsatuan.Location = New System.Drawing.Point(711, 110)
        Me.Txtsatuan.Name = "Txtsatuan"
        Me.Txtsatuan.ReadOnly = True
        Me.Txtsatuan.Size = New System.Drawing.Size(64, 22)
        Me.Txtsatuan.TabIndex = 8
        Me.Txtsatuan.Text = "satuan"
        Me.Txtsatuan.Visible = False
        '
        'TxtQty
        '
        Me.TxtQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtQty.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtQty.Location = New System.Drawing.Point(641, 110)
        Me.TxtQty.Name = "TxtQty"
        Me.TxtQty.ReadOnly = True
        Me.TxtQty.Size = New System.Drawing.Size(64, 22)
        Me.TxtQty.TabIndex = 8
        Me.TxtQty.Text = "Qty"
        Me.TxtQty.Visible = False
        '
        'TxtKode
        '
        Me.TxtKode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKode.Location = New System.Drawing.Point(571, 110)
        Me.TxtKode.Name = "TxtKode"
        Me.TxtKode.ReadOnly = True
        Me.TxtKode.Size = New System.Drawing.Size(64, 22)
        Me.TxtKode.TabIndex = 8
        Me.TxtKode.Text = "Kode"
        Me.TxtKode.Visible = False
        '
        'LstBarang
        '
        Me.LstBarang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LstBarang.FormattingEnabled = True
        Me.LstBarang.ItemHeight = 17
        Me.LstBarang.Location = New System.Drawing.Point(6, 173)
        Me.LstBarang.Name = "LstBarang"
        Me.LstBarang.Size = New System.Drawing.Size(533, 293)
        Me.LstBarang.TabIndex = 138
        '
        'DgvData
        '
        Me.DgvData.AllowUserToResizeColumns = False
        Me.DgvData.AllowUserToResizeRows = False
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black
        Me.DgvData.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.DgvData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DgvData.BackgroundColor = System.Drawing.Color.White
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Id, Me.Nama, Me.Hargabeli, Me.Qty, Me.Satuan, Me.Isi, Me.HargaBeliSat, Me.QtySat, Me.Totalharga, Me.Stok})
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DgvData.DefaultCellStyle = DataGridViewCellStyle10
        Me.DgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke
        Me.DgvData.Location = New System.Drawing.Point(0, 180)
        Me.DgvData.Name = "DgvData"
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvData.RowHeadersDefaultCellStyle = DataGridViewCellStyle11
        Me.DgvData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle12.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.InactiveCaption
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black
        Me.DgvData.RowsDefaultCellStyle = DataGridViewCellStyle12
        Me.DgvData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DgvData.Size = New System.Drawing.Size(1227, 359)
        Me.DgvData.TabIndex = 139
        '
        'Id
        '
        Me.Id.FillWeight = 50.0!
        Me.Id.HeaderText = "Id"
        Me.Id.Name = "Id"
        Me.Id.Visible = False
        '
        'Nama
        '
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black
        Me.Nama.DefaultCellStyle = DataGridViewCellStyle3
        Me.Nama.FillWeight = 200.0!
        Me.Nama.HeaderText = "Nama"
        Me.Nama.Name = "Nama"
        '
        'Hargabeli
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "N0"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.Hargabeli.DefaultCellStyle = DataGridViewCellStyle4
        Me.Hargabeli.FillWeight = 60.0!
        Me.Hargabeli.HeaderText = "Harga Beli"
        Me.Hargabeli.Name = "Hargabeli"
        Me.Hargabeli.ReadOnly = True
        '
        'Qty
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle5.Format = "N0"
        DataGridViewCellStyle5.NullValue = Nothing
        Me.Qty.DefaultCellStyle = DataGridViewCellStyle5
        Me.Qty.FillWeight = 30.0!
        Me.Qty.HeaderText = "Qty"
        Me.Qty.Name = "Qty"
        Me.Qty.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Qty.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Satuan
        '
        Me.Satuan.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox
        Me.Satuan.FillWeight = 50.0!
        Me.Satuan.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Satuan.HeaderText = "Satuan"
        Me.Satuan.Name = "Satuan"
        '
        'Isi
        '
        Me.Isi.FillWeight = 20.0!
        Me.Isi.HeaderText = "Isi"
        Me.Isi.Name = "Isi"
        Me.Isi.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Isi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Isi.Visible = False
        '
        'HargaBeliSat
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle6.Format = "N0"
        DataGridViewCellStyle6.NullValue = Nothing
        Me.HargaBeliSat.DefaultCellStyle = DataGridViewCellStyle6
        Me.HargaBeliSat.HeaderText = "Harga Beli Sat"
        Me.HargaBeliSat.Name = "HargaBeliSat"
        Me.HargaBeliSat.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HargaBeliSat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.HargaBeliSat.Visible = False
        '
        'QtySat
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle7.Format = "N0"
        DataGridViewCellStyle7.NullValue = Nothing
        Me.QtySat.DefaultCellStyle = DataGridViewCellStyle7
        Me.QtySat.FillWeight = 40.0!
        Me.QtySat.HeaderText = "QtySat"
        Me.QtySat.Name = "QtySat"
        Me.QtySat.Visible = False
        '
        'Totalharga
        '
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle8.Format = "N0"
        DataGridViewCellStyle8.NullValue = Nothing
        Me.Totalharga.DefaultCellStyle = DataGridViewCellStyle8
        Me.Totalharga.FillWeight = 80.0!
        Me.Totalharga.HeaderText = "Total Harga"
        Me.Totalharga.Name = "Totalharga"
        Me.Totalharga.ReadOnly = True
        Me.Totalharga.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Totalharga.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Stok
        '
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle9.Format = "N0"
        Me.Stok.DefaultCellStyle = DataGridViewCellStyle9
        Me.Stok.FillWeight = 40.0!
        Me.Stok.HeaderText = "Stok"
        Me.Stok.Name = "Stok"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.LightSkyBlue
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.LblRecord)
        Me.Panel1.Controls.Add(Me.BtnKeluar)
        Me.Panel1.Controls.Add(Me.TxtKomputer)
        Me.Panel1.Controls.Add(Me.TxtLogin)
        Me.Panel1.Controls.Add(Me.BtnSimpann)
        Me.Panel1.Controls.Add(Me.TxtGrandtotal)
        Me.Panel1.Controls.Add(Me.TxtTotalQTY)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.Label7)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 539)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1189, 55)
        Me.Panel1.TabIndex = 140
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(4, 7)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 17)
        Me.Label2.TabIndex = 126
        Me.Label2.Text = "Record"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblRecord
        '
        Me.LblRecord.AutoSize = True
        Me.LblRecord.BackColor = System.Drawing.Color.Transparent
        Me.LblRecord.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRecord.ForeColor = System.Drawing.Color.Black
        Me.LblRecord.Location = New System.Drawing.Point(72, 7)
        Me.LblRecord.Name = "LblRecord"
        Me.LblRecord.Size = New System.Drawing.Size(54, 17)
        Me.LblRecord.TabIndex = 125
        Me.LblRecord.Text = "Record"
        Me.LblRecord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BtnKeluar
        '
        Me.BtnKeluar.AllowDrop = True
        Me.BtnKeluar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnKeluar.BackColor = System.Drawing.Color.Orange
        Me.BtnKeluar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnKeluar.FlatAppearance.BorderSize = 0
        Me.BtnKeluar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightCoral
        Me.BtnKeluar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightCoral
        Me.BtnKeluar.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnKeluar.ForeColor = System.Drawing.Color.Black
        Me.BtnKeluar.Image = CType(resources.GetObject("BtnKeluar.Image"), System.Drawing.Image)
        Me.BtnKeluar.Location = New System.Drawing.Point(1027, 6)
        Me.BtnKeluar.Name = "BtnKeluar"
        Me.BtnKeluar.Size = New System.Drawing.Size(156, 42)
        Me.BtnKeluar.TabIndex = 77
        Me.BtnKeluar.Text = "Keluar (Esc)"
        Me.BtnKeluar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnKeluar.UseVisualStyleBackColor = False
        '
        'TxtKomputer
        '
        Me.TxtKomputer.BackColor = System.Drawing.Color.White
        Me.TxtKomputer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtKomputer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtKomputer.ForeColor = System.Drawing.Color.Black
        Me.TxtKomputer.Location = New System.Drawing.Point(641, 27)
        Me.TxtKomputer.Name = "TxtKomputer"
        Me.TxtKomputer.ReadOnly = True
        Me.TxtKomputer.Size = New System.Drawing.Size(73, 16)
        Me.TxtKomputer.TabIndex = 124
        Me.TxtKomputer.Text = "Komputer"
        Me.TxtKomputer.Visible = False
        '
        'TxtLogin
        '
        Me.TxtLogin.BackColor = System.Drawing.Color.White
        Me.TxtLogin.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtLogin.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLogin.ForeColor = System.Drawing.Color.Black
        Me.TxtLogin.Location = New System.Drawing.Point(584, 27)
        Me.TxtLogin.Name = "TxtLogin"
        Me.TxtLogin.ReadOnly = True
        Me.TxtLogin.Size = New System.Drawing.Size(55, 16)
        Me.TxtLogin.TabIndex = 123
        Me.TxtLogin.Text = "Login"
        Me.TxtLogin.Visible = False
        '
        'BtnSimpann
        '
        Me.BtnSimpann.BackColor = System.Drawing.Color.SandyBrown
        Me.BtnSimpann.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Blue
        Me.BtnSimpann.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Blue
        Me.BtnSimpann.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpann.ForeColor = System.Drawing.Color.Black
        Me.BtnSimpann.Image = CType(resources.GetObject("BtnSimpann.Image"), System.Drawing.Image)
        Me.BtnSimpann.Location = New System.Drawing.Point(810, 7)
        Me.BtnSimpann.Name = "BtnSimpann"
        Me.BtnSimpann.Size = New System.Drawing.Size(182, 41)
        Me.BtnSimpann.TabIndex = 113
        Me.BtnSimpann.Text = "Simpan (F8)"
        Me.BtnSimpann.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BtnSimpann.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpann.UseVisualStyleBackColor = False
        '
        'TxtGrandtotal
        '
        Me.TxtGrandtotal.BackColor = System.Drawing.Color.LightSkyBlue
        Me.TxtGrandtotal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtGrandtotal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGrandtotal.ForeColor = System.Drawing.Color.Black
        Me.TxtGrandtotal.Location = New System.Drawing.Point(394, 7)
        Me.TxtGrandtotal.Name = "TxtGrandtotal"
        Me.TxtGrandtotal.ReadOnly = True
        Me.TxtGrandtotal.Size = New System.Drawing.Size(160, 16)
        Me.TxtGrandtotal.TabIndex = 13
        Me.TxtGrandtotal.Text = "0"
        Me.TxtGrandtotal.Visible = False
        '
        'TxtTotalQTY
        '
        Me.TxtTotalQTY.BackColor = System.Drawing.Color.LightSkyBlue
        Me.TxtTotalQTY.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTotalQTY.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotalQTY.ForeColor = System.Drawing.Color.Black
        Me.TxtTotalQTY.Location = New System.Drawing.Point(228, 7)
        Me.TxtTotalQTY.Name = "TxtTotalQTY"
        Me.TxtTotalQTY.ReadOnly = True
        Me.TxtTotalQTY.Size = New System.Drawing.Size(51, 16)
        Me.TxtTotalQTY.TabIndex = 8
        Me.TxtTotalQTY.Text = "0"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(151, 7)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(65, 17)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Total Qty"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(292, 7)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(88, 17)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "Total Rupiah"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label7.Visible = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HapusToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.ShowCheckMargin = True
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(171, 26)
        '
        'HapusToolStripMenuItem
        '
        Me.HapusToolStripMenuItem.Name = "HapusToolStripMenuItem"
        Me.HapusToolStripMenuItem.Size = New System.Drawing.Size(170, 22)
        Me.HapusToolStripMenuItem.Text = "Hapus barang"
        '
        'FormReturBeli
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1189, 594)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LstBarang)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.DgvData)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormReturBeli"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormReturBeli"
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.PanelCariNama.ResumeLayout(False)
        Me.PanelCariNama.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel4 As Panel
    Friend WithEvents BtnClose As Button
    Friend WithEvents LblUtama As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents TxtStok As TextBox
    Friend WithEvents PanelCariNama As Panel
    Friend WithEvents BtnCari As Button
    Friend WithEvents TxtNama As TextBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Txtlihattotal As TextBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents LblTujuanTransfer As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents LblJenisTrans As Label
    Friend WithEvents DTPTgl As DateTimePicker
    Friend WithEvents LblLokasiBarang As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents TxtFaktur As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtBarcode As TextBox
    Friend WithEvents TxtHarga As TextBox
    Friend WithEvents TxtIsi As TextBox
    Friend WithEvents Txtsatuan As TextBox
    Friend WithEvents TxtQty As TextBox
    Friend WithEvents TxtKode As TextBox
    Friend WithEvents LstBarang As ListBox
    Friend WithEvents DgvData As DataGridView
    Friend WithEvents Id As DataGridViewTextBoxColumn
    Friend WithEvents Nama As DataGridViewTextBoxColumn
    Friend WithEvents Hargabeli As DataGridViewTextBoxColumn
    Friend WithEvents Qty As DataGridViewTextBoxColumn
    Friend WithEvents Satuan As DataGridViewComboBoxColumn
    Friend WithEvents Isi As DataGridViewTextBoxColumn
    Friend WithEvents HargaBeliSat As DataGridViewTextBoxColumn
    Friend WithEvents QtySat As DataGridViewTextBoxColumn
    Friend WithEvents Totalharga As DataGridViewTextBoxColumn
    Friend WithEvents Stok As DataGridViewTextBoxColumn
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents LblRecord As Label
    Friend WithEvents BtnKeluar As Button
    Friend WithEvents TxtKomputer As TextBox
    Friend WithEvents TxtLogin As TextBox
    Friend WithEvents BtnSimpann As Button
    Friend WithEvents TxtGrandtotal As TextBox
    Friend WithEvents TxtTotalQTY As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents HapusToolStripMenuItem As ToolStripMenuItem
End Class
