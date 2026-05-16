<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapLabaRugi
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapLabaRugi))
        Me.PanelKiri = New System.Windows.Forms.Panel()
        Me.LblJudul = New System.Windows.Forms.Label()
        Me.RbtSemua = New System.Windows.Forms.RadioButton()
        Me.RbtTanggal = New System.Windows.Forms.RadioButton()
        Me.PanelTanggal = New System.Windows.Forms.Panel()
        Me.LblAwal = New System.Windows.Forms.Label()
        Me.DtpAwal = New System.Windows.Forms.DateTimePicker()
        Me.LblAkhir = New System.Windows.Forms.Label()
        Me.DtpAkhir = New System.Windows.Forms.DateTimePicker()
        Me.RbtBulan = New System.Windows.Forms.RadioButton()
        Me.PanelBulan = New System.Windows.Forms.Panel()
        Me.LblBulan = New System.Windows.Forms.Label()
        Me.CmbBln = New System.Windows.Forms.ComboBox()
        Me.LblTahun = New System.Windows.Forms.Label()
        Me.CmbThn = New System.Windows.Forms.ComboBox()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.WebBrowserLR = New System.Windows.Forms.WebBrowser()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelKiri.SuspendLayout()
        Me.PanelTanggal.SuspendLayout()
        Me.PanelBulan.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelKiri
        '
        Me.PanelKiri.Controls.Add(Me.LblJudul)
        Me.PanelKiri.Controls.Add(Me.RbtSemua)
        Me.PanelKiri.Controls.Add(Me.RbtTanggal)
        Me.PanelKiri.Controls.Add(Me.PanelTanggal)
        Me.PanelKiri.Controls.Add(Me.RbtBulan)
        Me.PanelKiri.Controls.Add(Me.PanelBulan)
        Me.PanelKiri.Controls.Add(Me.BtnTampil)
        Me.PanelKiri.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelKiri.Location = New System.Drawing.Point(0, 0)
        Me.PanelKiri.Name = "PanelKiri"
        Me.PanelKiri.Padding = New System.Windows.Forms.Padding(10, 12, 10, 10)
        Me.PanelKiri.Size = New System.Drawing.Size(285, 700)
        Me.PanelKiri.TabIndex = 0
        '
        'LblJudul
        '
        Me.LblJudul.Font = New System.Drawing.Font("Century Gothic", 10.0!, System.Drawing.FontStyle.Bold)
        Me.LblJudul.Location = New System.Drawing.Point(10, 12)
        Me.LblJudul.Name = "LblJudul"
        Me.LblJudul.Size = New System.Drawing.Size(195, 22)
        Me.LblJudul.TabIndex = 0
        Me.LblJudul.Text = "Laporan Laba Rugi"
        '
        'RbtSemua
        '
        Me.RbtSemua.Checked = True
        Me.RbtSemua.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.RbtSemua.Location = New System.Drawing.Point(10, 44)
        Me.RbtSemua.Name = "RbtSemua"
        Me.RbtSemua.Size = New System.Drawing.Size(90, 22)
        Me.RbtSemua.TabIndex = 1
        Me.RbtSemua.TabStop = True
        Me.RbtSemua.Text = "Semua"
        '
        'RbtTanggal
        '
        Me.RbtTanggal.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.RbtTanggal.Location = New System.Drawing.Point(10, 70)
        Me.RbtTanggal.Name = "RbtTanggal"
        Me.RbtTanggal.Size = New System.Drawing.Size(90, 22)
        Me.RbtTanggal.TabIndex = 2
        Me.RbtTanggal.Text = "Tanggal"
        '
        'PanelTanggal
        '
        Me.PanelTanggal.Controls.Add(Me.LblAwal)
        Me.PanelTanggal.Controls.Add(Me.DtpAwal)
        Me.PanelTanggal.Controls.Add(Me.LblAkhir)
        Me.PanelTanggal.Controls.Add(Me.DtpAkhir)
        Me.PanelTanggal.Location = New System.Drawing.Point(10, 125)
        Me.PanelTanggal.Name = "PanelTanggal"
        Me.PanelTanggal.Size = New System.Drawing.Size(206, 56)
        Me.PanelTanggal.TabIndex = 3
        Me.PanelTanggal.Visible = False
        '
        'LblAwal
        '
        Me.LblAwal.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblAwal.Location = New System.Drawing.Point(0, 3)
        Me.LblAwal.Name = "LblAwal"
        Me.LblAwal.Size = New System.Drawing.Size(45, 20)
        Me.LblAwal.TabIndex = 0
        Me.LblAwal.Text = "Dari :"
        '
        'DtpAwal
        '
        Me.DtpAwal.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.DtpAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAwal.Location = New System.Drawing.Point(50, 1)
        Me.DtpAwal.Name = "DtpAwal"
        Me.DtpAwal.Size = New System.Drawing.Size(140, 22)
        Me.DtpAwal.TabIndex = 1
        '
        'LblAkhir
        '
        Me.LblAkhir.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblAkhir.Location = New System.Drawing.Point(0, 31)
        Me.LblAkhir.Name = "LblAkhir"
        Me.LblAkhir.Size = New System.Drawing.Size(45, 20)
        Me.LblAkhir.TabIndex = 2
        Me.LblAkhir.Text = "s/d :"
        '
        'DtpAkhir
        '
        Me.DtpAkhir.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.DtpAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DtpAkhir.Location = New System.Drawing.Point(50, 29)
        Me.DtpAkhir.Name = "DtpAkhir"
        Me.DtpAkhir.Size = New System.Drawing.Size(140, 22)
        Me.DtpAkhir.TabIndex = 3
        '
        'RbtBulan
        '
        Me.RbtBulan.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.RbtBulan.Location = New System.Drawing.Point(10, 96)
        Me.RbtBulan.Name = "RbtBulan"
        Me.RbtBulan.Size = New System.Drawing.Size(70, 22)
        Me.RbtBulan.TabIndex = 4
        Me.RbtBulan.Text = "Bulan"
        '
        'PanelBulan
        '
        Me.PanelBulan.Controls.Add(Me.LblBulan)
        Me.PanelBulan.Controls.Add(Me.CmbBln)
        Me.PanelBulan.Controls.Add(Me.LblTahun)
        Me.PanelBulan.Controls.Add(Me.CmbThn)
        Me.PanelBulan.Location = New System.Drawing.Point(10, 125)
        Me.PanelBulan.Name = "PanelBulan"
        Me.PanelBulan.Size = New System.Drawing.Size(206, 56)
        Me.PanelBulan.TabIndex = 5
        Me.PanelBulan.Visible = False
        '
        'LblBulan
        '
        Me.LblBulan.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblBulan.Location = New System.Drawing.Point(0, 3)
        Me.LblBulan.Name = "LblBulan"
        Me.LblBulan.Size = New System.Drawing.Size(50, 20)
        Me.LblBulan.TabIndex = 0
        Me.LblBulan.Text = "Bulan :"
        '
        'CmbBln
        '
        Me.CmbBln.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbBln.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.CmbBln.Location = New System.Drawing.Point(55, 1)
        Me.CmbBln.Name = "CmbBln"
        Me.CmbBln.Size = New System.Drawing.Size(135, 25)
        Me.CmbBln.TabIndex = 1
        '
        'LblTahun
        '
        Me.LblTahun.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.LblTahun.Location = New System.Drawing.Point(0, 31)
        Me.LblTahun.Name = "LblTahun"
        Me.LblTahun.Size = New System.Drawing.Size(50, 20)
        Me.LblTahun.TabIndex = 2
        Me.LblTahun.Text = "Tahun :"
        '
        'CmbThn
        '
        Me.CmbThn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbThn.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.CmbThn.Location = New System.Drawing.Point(55, 29)
        Me.CmbThn.Name = "CmbThn"
        Me.CmbThn.Size = New System.Drawing.Size(90, 25)
        Me.CmbThn.TabIndex = 3
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.Font = New System.Drawing.Font("Century Gothic", 9.0!, System.Drawing.FontStyle.Bold)
        Me.BtnTampil.Image = CType(resources.GetObject("BtnTampil.Image"), System.Drawing.Image)
        Me.BtnTampil.Location = New System.Drawing.Point(65, 201)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(123, 32)
        Me.BtnTampil.TabIndex = 6
        Me.BtnTampil.Text = "Tampilkan [F5]"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        'WebBrowserLR
        '
        Me.WebBrowserLR.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WebBrowserLR.Location = New System.Drawing.Point(285, 34)
        Me.WebBrowserLR.Name = "WebBrowserLR"
        Me.WebBrowserLR.ScriptErrorsSuppressed = True
        Me.WebBrowserLR.Size = New System.Drawing.Size(815, 666)
        Me.WebBrowserLR.TabIndex = 7
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.BackColor = System.Drawing.Color.Gold
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeaderForm.Location = New System.Drawing.Point(285, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(815, 31)
        Me.LblHeaderForm.TabIndex = 8
        Me.LblHeaderForm.Text = "LAPORAN LABA RUGI PERIODE"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FormLapLabaRugi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1100, 700)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.Controls.Add(Me.WebBrowserLR)
        Me.Controls.Add(Me.PanelKiri)
        Me.Font = New System.Drawing.Font("Century Gothic", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(800, 500)
        Me.Name = "FormLapLabaRugi"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Laporan Laba Rugi"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelKiri.ResumeLayout(False)
        Me.PanelKiri.PerformLayout()
        Me.PanelTanggal.ResumeLayout(False)
        Me.PanelBulan.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelKiri    As System.Windows.Forms.Panel
    Friend WithEvents LblJudul     As System.Windows.Forms.Label
    Friend WithEvents RbtSemua     As System.Windows.Forms.RadioButton
    Friend WithEvents RbtTanggal   As System.Windows.Forms.RadioButton
    Friend WithEvents RbtBulan     As System.Windows.Forms.RadioButton
    Friend WithEvents PanelTanggal As System.Windows.Forms.Panel
    Friend WithEvents LblAwal      As System.Windows.Forms.Label
    Friend WithEvents DtpAwal      As System.Windows.Forms.DateTimePicker
    Friend WithEvents LblAkhir     As System.Windows.Forms.Label
    Friend WithEvents DtpAkhir     As System.Windows.Forms.DateTimePicker
    Friend WithEvents PanelBulan   As System.Windows.Forms.Panel
    Friend WithEvents LblBulan     As System.Windows.Forms.Label
    Friend WithEvents CmbBln       As System.Windows.Forms.ComboBox
    Friend WithEvents LblTahun     As System.Windows.Forms.Label
    Friend WithEvents CmbThn       As System.Windows.Forms.ComboBox
    Friend WithEvents BtnTampil    As System.Windows.Forms.Button
    Friend WithEvents WebBrowserLR As System.Windows.Forms.WebBrowser
    Friend WithEvents LblHeaderForm As Label
End Class
