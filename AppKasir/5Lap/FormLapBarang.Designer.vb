<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLapBarang
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapBarang))
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.Stok_BarangBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        'Me.PossDataSet = New AppKasir.DataSetKL()
        Me.LblJudul = New System.Windows.Forms.Label()
        Me.BtnStokKosong = New System.Windows.Forms.Button()
        Me.BtnStok = New System.Windows.Forms.Button()
        Me.LblTotalQty = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblTotalRp = New System.Windows.Forms.Label()
        Me.LblStokGudang = New System.Windows.Forms.Label()
        Me.LblStokToko = New System.Windows.Forms.Label()
        Me.LblRecordToko = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.TxtCari = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.BtnStokMinus = New System.Windows.Forms.Button()
        Me.PanelCari = New System.Windows.Forms.Panel()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.BtnSemua = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LblRecordGudang = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LblRpToko = New System.Windows.Forms.Label()
        Me.LblRpGudang = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        'Me.Stok_BarangTableAdapter = New AppKasir.PossDataSetLancarTableAdapters.Stok_BarangTableAdapter()
        CType(Me.Stok_BarangBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.PanelCari.SuspendLayout()
        Me.SuspendLayout()
        '
        'Stok_BarangBindingSource
        '
        Me.Stok_BarangBindingSource.DataMember = "Stok_Barang"
        'Me.Stok_BarangBindingSource.DataSource = Me.PossDataSet
        '
        'PossDataSet
        '
        'Me.PossDataSet.DataSetName = "PossDataSet"
        'Me.PossDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'LblJudul
        '
        Me.LblJudul.BackColor = System.Drawing.SystemColors.Control
        Me.LblJudul.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblJudul.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblJudul.Location = New System.Drawing.Point(0, 0)
        Me.LblJudul.Name = "LblJudul"
        Me.LblJudul.Size = New System.Drawing.Size(1168, 31)
        Me.LblJudul.TabIndex = 92
        Me.LblJudul.Text = "LAPORAN DATA STOK BARANG DI TOKO DAN GUDANG"
        Me.LblJudul.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnStokKosong
        '
        Me.BtnStokKosong.AutoSize = True
        Me.BtnStokKosong.BackColor = System.Drawing.SystemColors.Control
        Me.BtnStokKosong.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnStokKosong.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnStokKosong.FlatAppearance.BorderSize = 1
        Me.BtnStokKosong.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnStokKosong.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnStokKosong.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnStokKosong.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnStokKosong.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnStokKosong.Image = CType(resources.GetObject("BtnStokKosong.Image"), System.Drawing.Image)
        Me.BtnStokKosong.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnStokKosong.Location = New System.Drawing.Point(16, 197)
        Me.BtnStokKosong.Name = "BtnStokKosong"
        Me.BtnStokKosong.Size = New System.Drawing.Size(210, 36)
        Me.BtnStokKosong.TabIndex = 96
        Me.BtnStokKosong.Text = "Stok Kosong"
        Me.BtnStokKosong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnStokKosong.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnStokKosong.UseVisualStyleBackColor = False
        '
        'BtnStok
        '
        Me.BtnStok.AutoSize = True
        Me.BtnStok.BackColor = System.Drawing.SystemColors.Control
        Me.BtnStok.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnStok.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnStok.FlatAppearance.BorderSize = 1
        Me.BtnStok.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnStok.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnStok.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnStok.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnStok.ForeColor = System.Drawing.Color.Black
        Me.BtnStok.Image = CType(resources.GetObject("BtnStok.Image"), System.Drawing.Image)
        Me.BtnStok.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnStok.Location = New System.Drawing.Point(16, 161)
        Me.BtnStok.Name = "BtnStok"
        Me.BtnStok.Size = New System.Drawing.Size(210, 36)
        Me.BtnStok.TabIndex = 95
        Me.BtnStok.Text = "Stok Ada"
        Me.BtnStok.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnStok.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnStok.UseVisualStyleBackColor = False
        '
        'LblTotalQty
        '
        Me.LblTotalQty.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalQty.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalQty.Location = New System.Drawing.Point(83, 415)
        Me.LblTotalQty.Name = "LblTotalQty"
        Me.LblTotalQty.Size = New System.Drawing.Size(133, 20)
        Me.LblTotalQty.TabIndex = 107
        Me.LblTotalQty.Text = "0"
        Me.LblTotalQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(4, 415)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(77, 17)
        Me.Label4.TabIndex = 106
        Me.Label4.Text = "Total stok :"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblTotalRp
        '
        Me.LblTotalRp.BackColor = System.Drawing.Color.Transparent
        Me.LblTotalRp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblTotalRp.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblTotalRp.Location = New System.Drawing.Point(83, 443)
        Me.LblTotalRp.Name = "LblTotalRp"
        Me.LblTotalRp.Size = New System.Drawing.Size(133, 20)
        Me.LblTotalRp.TabIndex = 105
        Me.LblTotalRp.Text = "Rp. 0"
        Me.LblTotalRp.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblStokGudang
        '
        Me.LblStokGudang.BackColor = System.Drawing.Color.Transparent
        Me.LblStokGudang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblStokGudang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStokGudang.Location = New System.Drawing.Point(222, 351)
        Me.LblStokGudang.Name = "LblStokGudang"
        Me.LblStokGudang.Size = New System.Drawing.Size(132, 20)
        Me.LblStokGudang.TabIndex = 103
        Me.LblStokGudang.Text = "0"
        Me.LblStokGudang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblStokToko
        '
        Me.LblStokToko.BackColor = System.Drawing.Color.Transparent
        Me.LblStokToko.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblStokToko.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStokToko.Location = New System.Drawing.Point(83, 351)
        Me.LblStokToko.Name = "LblStokToko"
        Me.LblStokToko.Size = New System.Drawing.Size(133, 20)
        Me.LblStokToko.TabIndex = 104
        Me.LblStokToko.Text = "0"
        Me.LblStokToko.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRecordToko
        '
        Me.LblRecordToko.BackColor = System.Drawing.Color.Transparent
        Me.LblRecordToko.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblRecordToko.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRecordToko.Location = New System.Drawing.Point(83, 325)
        Me.LblRecordToko.Name = "LblRecordToko"
        Me.LblRecordToko.Size = New System.Drawing.Size(133, 20)
        Me.LblRecordToko.TabIndex = 102
        Me.LblRecordToko.Text = "0"
        Me.LblRecordToko.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.Transparent
        Me.Label11.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(13, 443)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(68, 17)
        Me.Label11.TabIndex = 101
        Me.Label11.Text = "Total Rp :"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(38, 351)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(43, 17)
        Me.Label8.TabIndex = 100
        Me.Label8.Text = "Stok :"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(36, 325)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 17)
        Me.Label1.TabIndex = 98
        Me.Label1.Text = "Item :"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.ReportViewer1)
        Me.Panel1.Location = New System.Drawing.Point(376, 34)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(787, 433)
        Me.Panel1.TabIndex = 108
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.Stok_BarangBindingSource
        Me.ReportViewer1.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "KasirLancar.ReportStokBarang.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.Size = New System.Drawing.Size(787, 433)
        Me.ReportViewer1.TabIndex = 0
        '
        'TxtCari
        '
        Me.TxtCari.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCari.Location = New System.Drawing.Point(6, 5)
        Me.TxtCari.Name = "TxtCari"
        Me.TxtCari.Size = New System.Drawing.Size(335, 26)
        Me.TxtCari.TabIndex = 109
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(5, 44)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(302, 20)
        Me.Label9.TabIndex = 110
        Me.Label9.Text = "Ketik nama barang atau biarkan kosong"
        '
        'BtnStokMinus
        '
        Me.BtnStokMinus.AutoSize = True
        Me.BtnStokMinus.BackColor = System.Drawing.SystemColors.Control
        Me.BtnStokMinus.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnStokMinus.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnStokMinus.FlatAppearance.BorderSize = 1
        Me.BtnStokMinus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnStokMinus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnStokMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnStokMinus.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnStokMinus.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnStokMinus.Image = CType(resources.GetObject("BtnStokMinus.Image"), System.Drawing.Image)
        Me.BtnStokMinus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnStokMinus.Location = New System.Drawing.Point(16, 236)
        Me.BtnStokMinus.Name = "BtnStokMinus"
        Me.BtnStokMinus.Size = New System.Drawing.Size(210, 34)
        Me.BtnStokMinus.TabIndex = 111
        Me.BtnStokMinus.Text = "Stok Minus"
        Me.BtnStokMinus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnStokMinus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnStokMinus.UseVisualStyleBackColor = False
        '
        'PanelCariNama
        '
        Me.PanelCari.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PanelCari.Controls.Add(Me.BtnCari)
        Me.PanelCari.Controls.Add(Me.TxtCari)
        Me.PanelCari.Location = New System.Drawing.Point(5, 67)
        Me.PanelCari.Name = "PanelCari"
        Me.PanelCari.Size = New System.Drawing.Size(371, 36)
        Me.PanelCari.TabIndex = 112
        '
        'BtnCari
        '
        Me.BtnCari.BackColor = System.Drawing.SystemColors.Control
        Me.BtnCari.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnCari.FlatAppearance.BorderSize = 0
        Me.BtnCari.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnCari.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnCari.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.Location = New System.Drawing.Point(338, 5)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(26, 26)
        Me.BtnCari.TabIndex = 2
        Me.BtnCari.UseVisualStyleBackColor = True
        '
        'BtnSemua
        '
        Me.BtnSemua.AutoSize = True
        Me.BtnSemua.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSemua.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSemua.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnSemua.FlatAppearance.BorderSize = 1
        Me.BtnSemua.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnSemua.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnSemua.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSemua.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSemua.ForeColor = System.Drawing.Color.Black
        Me.BtnSemua.Image = CType(resources.GetObject("BtnSemua.Image"), System.Drawing.Image)
        Me.BtnSemua.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSemua.Location = New System.Drawing.Point(16, 121)
        Me.BtnSemua.Name = "BtnSemua"
        Me.BtnSemua.Size = New System.Drawing.Size(210, 36)
        Me.BtnSemua.TabIndex = 113
        Me.BtnSemua.Text = "Semua"
        Me.BtnSemua.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSemua.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSemua.UseVisualStyleBackColor = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(83, 302)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(43, 17)
        Me.Label3.TabIndex = 114
        Me.Label3.Text = "TOKO"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRecordGudang
        '
        Me.LblRecordGudang.BackColor = System.Drawing.Color.Transparent
        Me.LblRecordGudang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblRecordGudang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRecordGudang.Location = New System.Drawing.Point(222, 325)
        Me.LblRecordGudang.Name = "LblRecordGudang"
        Me.LblRecordGudang.Size = New System.Drawing.Size(132, 20)
        Me.LblRecordGudang.TabIndex = 115
        Me.LblRecordGudang.Text = "0"
        Me.LblRecordGudang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(10, 376)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(71, 17)
        Me.Label5.TabIndex = 100
        Me.Label5.Text = "Nominal :"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRpToko
        '
        Me.LblRpToko.BackColor = System.Drawing.Color.Transparent
        Me.LblRpToko.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblRpToko.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRpToko.Location = New System.Drawing.Point(83, 376)
        Me.LblRpToko.Name = "LblRpToko"
        Me.LblRpToko.Size = New System.Drawing.Size(133, 20)
        Me.LblRpToko.TabIndex = 104
        Me.LblRpToko.Text = "0"
        Me.LblRpToko.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRpGudang
        '
        Me.LblRpGudang.BackColor = System.Drawing.Color.Transparent
        Me.LblRpGudang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LblRpGudang.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblRpGudang.Location = New System.Drawing.Point(222, 376)
        Me.LblRpGudang.Name = "LblRpGudang"
        Me.LblRpGudang.Size = New System.Drawing.Size(132, 20)
        Me.LblRpGudang.TabIndex = 103
        Me.LblRpGudang.Text = "0"
        Me.LblRpGudang.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(222, 302)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(67, 17)
        Me.Label10.TabIndex = 116
        Me.Label10.Text = "GUDANG"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Stok_BarangTableAdapter
        '
        'Me.Stok_BarangTableAdapter.ClearBeforeFill = True
        '
        'FormLapBarang
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1168, 468)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.LblRecordGudang)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.BtnSemua)
        Me.Controls.Add(Me.PanelCari)
        Me.Controls.Add(Me.BtnStokMinus)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.LblTotalQty)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.LblTotalRp)
        Me.Controls.Add(Me.LblRpGudang)
        Me.Controls.Add(Me.LblStokGudang)
        Me.Controls.Add(Me.LblRpToko)
        Me.Controls.Add(Me.LblStokToko)
        Me.Controls.Add(Me.LblRecordToko)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnStokKosong)
        Me.Controls.Add(Me.BtnStok)
        Me.Controls.Add(Me.LblJudul)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormLapBarang"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.Stok_BarangBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.PossDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.PanelCari.ResumeLayout(False)
        Me.PanelCari.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LblJudul As System.Windows.Forms.Label
    Friend WithEvents BtnStokKosong As System.Windows.Forms.Button
    Friend WithEvents BtnStok As System.Windows.Forms.Button
    Friend WithEvents LblTotalQty As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LblTotalRp As System.Windows.Forms.Label
    Friend WithEvents LblStokGudang As System.Windows.Forms.Label
    Friend WithEvents LblStokToko As System.Windows.Forms.Label
    Friend WithEvents LblRecordToko As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents TxtCari As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents BtnStokMinus As System.Windows.Forms.Button
    Friend WithEvents PanelCari As System.Windows.Forms.Panel
    Friend WithEvents BtnCari As System.Windows.Forms.Button
    Friend WithEvents BtnSemua As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents LblRecordGudang As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LblRpToko As System.Windows.Forms.Label
    Friend WithEvents LblRpGudang As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Stok_BarangBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents PossDataSet As AppKasir.PossDataSet
    'Friend WithEvents Stok_BarangTableAdapter As AppKasir.PossDataSetLancarTableAdapters.Stok_BarangTableAdapter
End Class
