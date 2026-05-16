<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintReturBeli
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintReturBeli))
        Me.TxtIdUser = New System.Windows.Forms.TextBox()
        Me.TxtIdKomputer = New System.Windows.Forms.TextBox()
        Me.BtnCetak = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtFaktur = New System.Windows.Forms.TextBox()
        Me.TxtTotal = New System.Windows.Forms.TextBox()
        Me.DTPTgl = New System.Windows.Forms.DateTimePicker()
        Me.DgvData = New System.Windows.Forms.DataGridView()
        Me.Btnsimpan = New System.Windows.Forms.Button()
        Me.TxtNamaSupplier = New System.Windows.Forms.TextBox()
        Me.TxtAlamatSupplier = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ID_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NAMA_BARANG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HARGA_BELI_SATUAN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TOTAL = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TxtIdUser
        '
        Me.TxtIdUser.BackColor = System.Drawing.Color.White
        Me.TxtIdUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdUser.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdUser.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtIdUser.Location = New System.Drawing.Point(185, 168)
        Me.TxtIdUser.Name = "TxtIdUser"
        Me.TxtIdUser.Size = New System.Drawing.Size(200, 23)
        Me.TxtIdUser.TabIndex = 196
        Me.TxtIdUser.Text = "User"
        '
        'TxtIdKomputer
        '
        Me.TxtIdKomputer.BackColor = System.Drawing.Color.White
        Me.TxtIdKomputer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtIdKomputer.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtIdKomputer.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtIdKomputer.Location = New System.Drawing.Point(185, 198)
        Me.TxtIdKomputer.Name = "TxtIdKomputer"
        Me.TxtIdKomputer.Size = New System.Drawing.Size(200, 23)
        Me.TxtIdKomputer.TabIndex = 195
        Me.TxtIdKomputer.Text = "Komputer"
        '
        'BtnCetak
        '
        Me.BtnCetak.AutoSize = True
        Me.BtnCetak.BackColor = System.Drawing.Color.White
        Me.BtnCetak.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCetak.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(100, 116, 139)
        Me.BtnCetak.FlatAppearance.BorderSize = 1
        Me.BtnCetak.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(226, 232, 240)
        Me.BtnCetak.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(241, 245, 249)
        Me.BtnCetak.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCetak.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCetak.ForeColor = System.Drawing.Color.Black
        Me.BtnCetak.Image = CType(resources.GetObject("BtnCetak.Image"), System.Drawing.Image)
        Me.BtnCetak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnCetak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCetak.Location = New System.Drawing.Point(464, 299)
        Me.BtnCetak.Name = "BtnCetak"
        Me.BtnCetak.Size = New System.Drawing.Size(120, 33)
        Me.BtnCetak.TabIndex = 194
        Me.BtnCetak.Text = "Cetak"
        Me.BtnCetak.UseVisualStyleBackColor = False
        Me.BtnCetak.Visible = False
        '
        'Button1
        '
        Me.Button1.AutoSize = True
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.Button1.FlatAppearance.BorderSize = 1
        Me.Button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.Button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.Button1.Location = New System.Drawing.Point(742, 243)
        Me.Button1.Name = "Button1"
        Me.Button1.Image = CType(resources.GetObject("Button1.Image"), System.Drawing.Image)
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button1.Size = New System.Drawing.Size(242, 33)
        Me.Button1.TabIndex = 192
        Me.Button1.Text = "Keluar"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(93, 85)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 17)
        Me.Label4.TabIndex = 185
        Me.Label4.Text = "Pelanggan "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(116, 52)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 17)
        Me.Label3.TabIndex = 184
        Me.Label3.Text = "Tanggal"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(106, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 17)
        Me.Label1.TabIndex = 183
        Me.Label1.Text = "No Faktur"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(129, 141)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(47, 17)
        Me.Label7.TabIndex = 181
        Me.Label7.Text = "Total :"
        '
        'TxtFaktur
        '
        Me.TxtFaktur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtFaktur.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFaktur.Location = New System.Drawing.Point(185, 22)
        Me.TxtFaktur.Name = "TxtFaktur"
        Me.TxtFaktur.Size = New System.Drawing.Size(200, 23)
        Me.TxtFaktur.TabIndex = 170
        '
        'TxtTotal
        '
        Me.TxtTotal.BackColor = System.Drawing.Color.White
        Me.TxtTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtTotal.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotal.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.TxtTotal.Location = New System.Drawing.Point(185, 139)
        Me.TxtTotal.Name = "TxtTotal"
        Me.TxtTotal.Size = New System.Drawing.Size(200, 23)
        Me.TxtTotal.TabIndex = 174
        Me.TxtTotal.Text = "0"
        Me.TxtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'DTPTgl
        '
        Me.DTPTgl.CustomFormat = "dd/MM/yyyy hh:mm:ss"
        Me.DTPTgl.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DTPTgl.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPTgl.Location = New System.Drawing.Point(185, 52)
        Me.DTPTgl.Name = "DTPTgl"
        Me.DTPTgl.Size = New System.Drawing.Size(200, 23)
        Me.DTPTgl.TabIndex = 172
        '
        'DgvData
        '
        Me.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ID_BARANG, Me.NAMA_BARANG, Me.QTY, Me.SATUAN, Me.HARGA_BELI_SATUAN, Me.TOTAL})
        Me.DgvData.Location = New System.Drawing.Point(408, 22)
        Me.DgvData.Name = "DgvData"
        Me.DgvData.RowHeadersVisible = False
        Me.DgvData.Size = New System.Drawing.Size(700, 198)
        Me.DgvData.TabIndex = 167
        '
        'Btnsimpan
        '
        Me.Btnsimpan.AutoSize = True
        Me.Btnsimpan.BackColor = System.Drawing.Color.White
        Me.Btnsimpan.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btnsimpan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.Btnsimpan.FlatAppearance.BorderSize = 1
        Me.Btnsimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.Btnsimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.Btnsimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Btnsimpan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Btnsimpan.ForeColor = System.Drawing.Color.Black
        Me.Btnsimpan.Location = New System.Drawing.Point(464, 243)
        Me.Btnsimpan.Name = "Btnsimpan"
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Btnsimpan.Size = New System.Drawing.Size(242, 33)
        Me.Btnsimpan.TabIndex = 166
        Me.Btnsimpan.Text = "Cetak"
        Me.Btnsimpan.UseVisualStyleBackColor = False
        '
        'TxtNamaSupplier
        '
        Me.TxtNamaSupplier.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtNamaSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNamaSupplier.Location = New System.Drawing.Point(185, 81)
        Me.TxtNamaSupplier.Name = "TxtNamaSupplier"
        Me.TxtNamaSupplier.ReadOnly = True
        Me.TxtNamaSupplier.Size = New System.Drawing.Size(200, 23)
        Me.TxtNamaSupplier.TabIndex = 197
        '
        'TxtAlamatSupplier
        '
        Me.TxtAlamatSupplier.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtAlamatSupplier.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAlamatSupplier.Location = New System.Drawing.Point(185, 110)
        Me.TxtAlamatSupplier.Name = "TxtAlamatSupplier"
        Me.TxtAlamatSupplier.ReadOnly = True
        Me.TxtAlamatSupplier.Size = New System.Drawing.Size(200, 23)
        Me.TxtAlamatSupplier.TabIndex = 199
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(93, 114)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 17)
        Me.Label2.TabIndex = 198
        Me.Label2.Text = "Alamat"
        '
        'ID_BARANG
        '
        Me.ID_BARANG.HeaderText = "kode"
        Me.ID_BARANG.Name = "ID_BARANG"
        '
        'NAMA_BARANG
        '
        Me.NAMA_BARANG.HeaderText = "NamaBarang"
        Me.NAMA_BARANG.Name = "NAMA_BARANG"
        '
        'QTY
        '
        Me.QTY.HeaderText = "QTY"
        Me.QTY.Name = "QTY"
        '
        'SATUAN
        '
        Me.SATUAN.HeaderText = "Satuan"
        Me.SATUAN.Name = "SATUAN"
        '
        'HARGA_BELI_SATUAN
        '
        Me.HARGA_BELI_SATUAN.HeaderText = "Harga"
        Me.HARGA_BELI_SATUAN.Name = "HARGA_BELI_SATUAN"
        '
        'TOTAL
        '
        Me.TOTAL.HeaderText = "TotalHarga"
        Me.TOTAL.Name = "TOTAL"
        '
        'PrintReturBeli
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1351, 539)
        Me.Controls.Add(Me.TxtAlamatSupplier)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtNamaSupplier)
        Me.Controls.Add(Me.TxtIdUser)
        Me.Controls.Add(Me.TxtIdKomputer)
        Me.Controls.Add(Me.BtnCetak)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.TxtFaktur)
        Me.Controls.Add(Me.TxtTotal)
        Me.Controls.Add(Me.DTPTgl)
        Me.Controls.Add(Me.DgvData)
        Me.Controls.Add(Me.Btnsimpan)
        Me.Name = "PrintReturBeli"
        Me.Text = "PrintReturBeli"
        CType(Me.DgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TxtIdUser As TextBox
    Friend WithEvents TxtIdKomputer As TextBox
    Friend WithEvents BtnCetak As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtFaktur As TextBox
    Friend WithEvents TxtTotal As TextBox
    Friend WithEvents DTPTgl As DateTimePicker
    Friend WithEvents DgvData As DataGridView
    Friend WithEvents Btnsimpan As Button
    Friend WithEvents TxtNamaSupplier As TextBox
    Friend WithEvents TxtAlamatSupplier As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents ID_BARANG As DataGridViewTextBoxColumn
    Friend WithEvents NAMA_BARANG As DataGridViewTextBoxColumn
    Friend WithEvents QTY As DataGridViewTextBoxColumn
    Friend WithEvents SATUAN As DataGridViewTextBoxColumn
    Friend WithEvents HARGA_BELI_SATUAN As DataGridViewTextBoxColumn
    Friend WithEvents TOTAL As DataGridViewTextBoxColumn
End Class
