<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormAuditTrailArsip
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormAuditTrailArsip))
        Me.PanelJudul = New System.Windows.Forms.Panel()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelFilter = New System.Windows.Forms.Panel()
        Me.LblAwal = New System.Windows.Forms.Label()
        Me.DtpAwal = New System.Windows.Forms.DateTimePicker()
        Me.LblAkhir = New System.Windows.Forms.Label()
        Me.DtpAkhir = New System.Windows.Forms.DateTimePicker()
        Me.LblUser = New System.Windows.Forms.Label()
        Me.CmbUser = New System.Windows.Forms.ComboBox()
        Me.LblAksi = New System.Windows.Forms.Label()
        Me.CmbJenisAksi = New System.Windows.Forms.ComboBox()
        Me.LblJenis = New System.Windows.Forms.Label()
        Me.CmbJenisTrans = New System.Windows.Forms.ComboBox()
        Me.BtnCari = New System.Windows.Forms.Button()
        Me.BtnExport = New System.Windows.Forms.Button()
        Me.BtnTutup = New System.Windows.Forms.Button()
        Me.LblTotalRecord = New System.Windows.Forms.Label()
        Me.DgvAudit = New System.Windows.Forms.DataGridView()
        Me.ColWaktu = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColAksi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColJenisTrans = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColIdentifier = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColUser = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColLokasi = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColKomputer = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColKet = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColIdAudit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PanelDetail = New System.Windows.Forms.Panel()
        Me.LblDetail = New System.Windows.Forms.Label()
        Me.TxtDetail = New System.Windows.Forms.TextBox()
        Me.PanelJudul.SuspendLayout()
        Me.PanelFilter.SuspendLayout()
        CType(Me.DgvAudit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDetail.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelJudul
        '
        Me.PanelJudul.Controls.Add(Me.BtnTutup)
        Me.PanelJudul.Controls.Add(Me.LblHeaderForm)
        Me.PanelJudul.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelJudul.Location = New System.Drawing.Point(0, 0)
        Me.PanelJudul.Name = "PanelJudul"
        Me.PanelJudul.Size = New System.Drawing.Size(1261, 42)
        Me.PanelJudul.TabIndex = 3
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Gold
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold)
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1261, 39)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "AUDIT TRAIL ARSIP — Data Lama"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelFilter
        '
        Me.PanelFilter.Controls.Add(Me.LblAwal)
        Me.PanelFilter.Controls.Add(Me.DtpAwal)
        Me.PanelFilter.Controls.Add(Me.LblAkhir)
        Me.PanelFilter.Controls.Add(Me.DtpAkhir)
        Me.PanelFilter.Controls.Add(Me.LblUser)
        Me.PanelFilter.Controls.Add(Me.CmbUser)
        Me.PanelFilter.Controls.Add(Me.LblAksi)
        Me.PanelFilter.Controls.Add(Me.CmbJenisAksi)
        Me.PanelFilter.Controls.Add(Me.LblJenis)
        Me.PanelFilter.Controls.Add(Me.CmbJenisTrans)
        Me.PanelFilter.Controls.Add(Me.BtnCari)
        Me.PanelFilter.Controls.Add(Me.BtnExport)
        Me.PanelFilter.Controls.Add(Me.LblTotalRecord)
        Me.PanelFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFilter.Location = New System.Drawing.Point(0, 42)
        Me.PanelFilter.Name = "PanelFilter"
        Me.PanelFilter.Padding = New System.Windows.Forms.Padding(7)
        Me.PanelFilter.Size = New System.Drawing.Size(1261, 65)
        Me.PanelFilter.TabIndex = 2
        '
        'LblAwal
        '
        Me.LblAwal.AutoSize = True
        Me.LblAwal.Location = New System.Drawing.Point(7, 14)
        Me.LblAwal.Name = "LblAwal"
        Me.LblAwal.Size = New System.Drawing.Size(29, 13)
        Me.LblAwal.TabIndex = 0
        Me.LblAwal.Text = "Dari:"
        '
        'DtpAwal
        '
        Me.DtpAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAwal.Location = New System.Drawing.Point(40, 10)
        Me.DtpAwal.Name = "DtpAwal"
        Me.DtpAwal.Size = New System.Drawing.Size(112, 20)
        Me.DtpAwal.TabIndex = 1
        '
        'LblAkhir
        '
        Me.LblAkhir.AutoSize = True
        Me.LblAkhir.Location = New System.Drawing.Point(161, 14)
        Me.LblAkhir.Name = "LblAkhir"
        Me.LblAkhir.Size = New System.Drawing.Size(26, 13)
        Me.LblAkhir.TabIndex = 2
        Me.LblAkhir.Text = "s/d:"
        '
        'DtpAkhir
        '
        Me.DtpAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAkhir.Location = New System.Drawing.Point(193, 10)
        Me.DtpAkhir.Name = "DtpAkhir"
        Me.DtpAkhir.Size = New System.Drawing.Size(112, 20)
        Me.DtpAkhir.TabIndex = 3
        '
        'LblUser
        '
        Me.LblUser.AutoSize = True
        Me.LblUser.Location = New System.Drawing.Point(313, 14)
        Me.LblUser.Name = "LblUser"
        Me.LblUser.Size = New System.Drawing.Size(32, 13)
        Me.LblUser.TabIndex = 4
        Me.LblUser.Text = "User:"
        '
        'CmbUser
        '
        Me.CmbUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbUser.Location = New System.Drawing.Point(351, 10)
        Me.CmbUser.Name = "CmbUser"
        Me.CmbUser.Size = New System.Drawing.Size(103, 21)
        Me.CmbUser.TabIndex = 5
        '
        'LblAksi
        '
        Me.LblAksi.AutoSize = True
        Me.LblAksi.Location = New System.Drawing.Point(463, 14)
        Me.LblAksi.Name = "LblAksi"
        Me.LblAksi.Size = New System.Drawing.Size(30, 13)
        Me.LblAksi.TabIndex = 6
        Me.LblAksi.Text = "Aksi:"
        '
        'CmbJenisAksi
        '
        Me.CmbJenisAksi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisAksi.Location = New System.Drawing.Point(499, 10)
        Me.CmbJenisAksi.Name = "CmbJenisAksi"
        Me.CmbJenisAksi.Size = New System.Drawing.Size(95, 21)
        Me.CmbJenisAksi.TabIndex = 7
        '
        'LblJenis
        '
        Me.LblJenis.AutoSize = True
        Me.LblJenis.Location = New System.Drawing.Point(602, 14)
        Me.LblJenis.Name = "LblJenis"
        Me.LblJenis.Size = New System.Drawing.Size(34, 13)
        Me.LblJenis.TabIndex = 8
        Me.LblJenis.Text = "Jenis:"
        '
        'CmbJenisTrans
        '
        Me.CmbJenisTrans.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbJenisTrans.Location = New System.Drawing.Point(642, 10)
        Me.CmbJenisTrans.Name = "CmbJenisTrans"
        Me.CmbJenisTrans.Size = New System.Drawing.Size(121, 21)
        Me.CmbJenisTrans.TabIndex = 9
        '
        'BtnCari
        '
        Me.BtnCari.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnCari.Image = CType(resources.GetObject("BtnCari.Image"), System.Drawing.Image)
        Me.BtnCari.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.Location = New System.Drawing.Point(784, 8)
        Me.BtnCari.Name = "BtnCari"
        Me.BtnCari.Size = New System.Drawing.Size(94, 31)
        Me.BtnCari.TabIndex = 10
        Me.BtnCari.Text = "Filter (F5)"
        Me.BtnCari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnCari.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        'BtnExport
        '
        Me.BtnExport.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnExport.Image = CType(resources.GetObject("BtnExport.Image"), System.Drawing.Image)
        Me.BtnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnExport.Location = New System.Drawing.Point(938, 8)
        Me.BtnExport.Name = "BtnExport"
        Me.BtnExport.Size = New System.Drawing.Size(101, 31)
        Me.BtnExport.TabIndex = 11
        Me.BtnExport.Text = "Export CSV"
        Me.BtnExport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        'BtnTutup
        '
        Me.BtnTutup.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.BtnTutup.Image = CType(resources.GetObject("BtnTutup.Image"), System.Drawing.Image)
        Me.BtnTutup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTutup.Location = New System.Drawing.Point(1176, 0)
        Me.BtnTutup.Name = "BtnTutup"
        Me.BtnTutup.Size = New System.Drawing.Size(79, 31)
        Me.BtnTutup.TabIndex = 12
        Me.BtnTutup.Text = "Tutup"
        Me.BtnTutup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTutup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        'LblTotalRecord
        '
        Me.LblTotalRecord.AutoSize = True
        Me.LblTotalRecord.Location = New System.Drawing.Point(12, 45)
        Me.LblTotalRecord.Name = "LblTotalRecord"
        Me.LblTotalRecord.Size = New System.Drawing.Size(101, 13)
        Me.LblTotalRecord.TabIndex = 13
        Me.LblTotalRecord.Text = "Total record arsip: 0"
        '
        'DgvAudit
        '
        Me.DgvAudit.AllowUserToAddRows = False
        Me.DgvAudit.AllowUserToDeleteRows = False
        Me.DgvAudit.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColWaktu, Me.ColAksi, Me.ColJenisTrans, Me.ColIdentifier, Me.ColUser, Me.ColLokasi, Me.ColKomputer, Me.ColKet, Me.ColIdAudit})
        Me.DgvAudit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvAudit.Location = New System.Drawing.Point(0, 107)
        Me.DgvAudit.MultiSelect = False
        Me.DgvAudit.Name = "DgvAudit"
        Me.DgvAudit.ReadOnly = True
        Me.DgvAudit.RowHeadersVisible = False
        Me.DgvAudit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DgvAudit.Size = New System.Drawing.Size(1261, 437)
        Me.DgvAudit.TabIndex = 0
        '
        'ColWaktu
        '
        Me.ColWaktu.HeaderText = "Waktu Aksi"
        Me.ColWaktu.Name = "ColWaktu"
        Me.ColWaktu.ReadOnly = True
        Me.ColWaktu.Width = 140
        '
        'ColAksi
        '
        Me.ColAksi.HeaderText = "Jenis Aksi"
        Me.ColAksi.Name = "ColAksi"
        Me.ColAksi.ReadOnly = True
        '
        'ColJenisTrans
        '
        Me.ColJenisTrans.HeaderText = "Jenis Transaksi"
        Me.ColJenisTrans.Name = "ColJenisTrans"
        Me.ColJenisTrans.ReadOnly = True
        Me.ColJenisTrans.Width = 130
        '
        'ColIdentifier
        '
        Me.ColIdentifier.HeaderText = "Identifier"
        Me.ColIdentifier.Name = "ColIdentifier"
        Me.ColIdentifier.ReadOnly = True
        Me.ColIdentifier.Width = 160
        '
        'ColUser
        '
        Me.ColUser.HeaderText = "User"
        Me.ColUser.Name = "ColUser"
        Me.ColUser.ReadOnly = True
        '
        'ColLokasi
        '
        Me.ColLokasi.HeaderText = "Lokasi"
        Me.ColLokasi.Name = "ColLokasi"
        Me.ColLokasi.ReadOnly = True
        Me.ColLokasi.Width = 70
        '
        'ColKomputer
        '
        Me.ColKomputer.HeaderText = "Komputer"
        Me.ColKomputer.Name = "ColKomputer"
        Me.ColKomputer.ReadOnly = True
        '
        'ColKet
        '
        Me.ColKet.HeaderText = "Keterangan"
        Me.ColKet.Name = "ColKet"
        Me.ColKet.ReadOnly = True
        Me.ColKet.Visible = False
        Me.ColKet.Width = 200
        '
        'ColIdAudit
        '
        Me.ColIdAudit.HeaderText = "ID"
        Me.ColIdAudit.Name = "ColIdAudit"
        Me.ColIdAudit.ReadOnly = True
        Me.ColIdAudit.Visible = False
        '
        'PanelDetail
        '
        Me.PanelDetail.Controls.Add(Me.LblDetail)
        Me.PanelDetail.Controls.Add(Me.TxtDetail)
        Me.PanelDetail.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelDetail.Location = New System.Drawing.Point(0, 544)
        Me.PanelDetail.Name = "PanelDetail"
        Me.PanelDetail.Size = New System.Drawing.Size(1261, 156)
        Me.PanelDetail.TabIndex = 1
        '
        'LblDetail
        '
        Me.LblDetail.AutoSize = True
        Me.LblDetail.Location = New System.Drawing.Point(3, 3)
        Me.LblDetail.Name = "LblDetail"
        Me.LblDetail.Size = New System.Drawing.Size(85, 13)
        Me.LblDetail.TabIndex = 0
        Me.LblDetail.Text = "Detail Snapshot:"
        '
        'TxtDetail
        '
        Me.TxtDetail.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtDetail.Font = New System.Drawing.Font("Consolas", 9.0!)
        Me.TxtDetail.Location = New System.Drawing.Point(3, 19)
        Me.TxtDetail.Multiline = True
        Me.TxtDetail.Name = "TxtDetail"
        Me.TxtDetail.ReadOnly = True
        Me.TxtDetail.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TxtDetail.Size = New System.Drawing.Size(1252, 130)
        Me.TxtDetail.TabIndex = 1
        '
        'FormAuditTrailArsip
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1261, 700)
        Me.Controls.Add(Me.DgvAudit)
        Me.Controls.Add(Me.PanelDetail)
        Me.Controls.Add(Me.PanelFilter)
        Me.Controls.Add(Me.PanelJudul)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(774, 439)
        Me.Name = "FormAuditTrailArsip"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AUDIT TRAIL ARSIP"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelJudul.ResumeLayout(False)
        Me.PanelFilter.ResumeLayout(False)
        Me.PanelFilter.PerformLayout()
        CType(Me.DgvAudit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDetail.ResumeLayout(False)
        Me.PanelDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelJudul As System.Windows.Forms.Panel
    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents PanelFilter As System.Windows.Forms.Panel
    Friend WithEvents LblAwal As System.Windows.Forms.Label
    Friend WithEvents DtpAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblAkhir As System.Windows.Forms.Label
    Friend WithEvents DtpAkhir As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblUser As System.Windows.Forms.Label
    Friend WithEvents CmbUser As System.Windows.Forms.ComboBox
    Friend WithEvents LblAksi As System.Windows.Forms.Label
    Friend WithEvents CmbJenisAksi As System.Windows.Forms.ComboBox
    Friend WithEvents LblJenis As System.Windows.Forms.Label
    Friend WithEvents CmbJenisTrans As System.Windows.Forms.ComboBox
    Friend WithEvents BtnCari As System.Windows.Forms.Button
    Friend WithEvents BtnExport As System.Windows.Forms.Button
    Friend WithEvents BtnTutup As System.Windows.Forms.Button
    Friend WithEvents LblTotalRecord As System.Windows.Forms.Label
    Friend WithEvents DgvAudit As System.Windows.Forms.DataGridView
    Friend WithEvents ColWaktu As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColAksi As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColJenisTrans As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColIdentifier As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColUser As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColLokasi As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColKomputer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColKet As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColIdAudit As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PanelDetail As System.Windows.Forms.Panel
    Friend WithEvents LblDetail As System.Windows.Forms.Label
    Friend WithEvents TxtDetail As System.Windows.Forms.TextBox
End Class
