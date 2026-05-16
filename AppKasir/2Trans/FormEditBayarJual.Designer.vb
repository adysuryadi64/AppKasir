<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditBayarJual
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEditBayarJual))
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelBody = New System.Windows.Forms.Panel()
        Me.TablePembayaran = New System.Windows.Forms.TableLayoutPanel()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.lblTotalBelanja = New System.Windows.Forms.Label()
        Me.lblTotalFmt = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.cmbBayarTunai = New System.Windows.Forms.ComboBox()
        Me.txtNominalTunai = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtKodeBayarTunai = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cmbBayarTransfer = New System.Windows.Forms.ComboBox()
        Me.txtNominalTransfer = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtKodeBayarTransfer = New System.Windows.Forms.TextBox()
        Me.lblHasilCaption = New System.Windows.Forms.Label()
        Me.lblHasilValue = New System.Windows.Forms.Label()
        Me.lblJatuhTempo = New System.Windows.Forms.Label()
        Me.dtpJatuhTempo = New System.Windows.Forms.DateTimePicker()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblStatusValue = New System.Windows.Forms.Label()
        Me.PanelSeparator = New System.Windows.Forms.Panel()
        Me.TableHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblFakturValue = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblPelangganValue = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblAlamatPelangganValue = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblJenisPelangganValue = New System.Windows.Forms.Label()
        Me.lblTunaiFmt = New System.Windows.Forms.Label()
        Me.lblTransferFmt = New System.Windows.Forms.Label()
        Me.PanelFooter = New System.Windows.Forms.Panel()
        Me.BtnBatal = New System.Windows.Forms.Button()
        Me.BtnSimpan = New System.Windows.Forms.Button()
        Me.PanelInfoTransfer = New System.Windows.Forms.Panel()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.cmbBankPengirim = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtNoRek = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtNamaRek = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtNoReff = New System.Windows.Forms.TextBox()
        Me.PanelHeader.SuspendLayout()
        Me.PanelBody.SuspendLayout()
        Me.TablePembayaran.SuspendLayout()
        Me.TableHeader.SuspendLayout()
        Me.PanelFooter.SuspendLayout()
        Me.PanelInfoTransfer.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.Panel1)
        Me.PanelHeader.Controls.Add(Me.LblHeaderForm)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(480, 50)
        Me.PanelHeader.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Location = New System.Drawing.Point(271, 49)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 100)
        Me.Panel1.TabIndex = 1
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblHeaderForm.Font = New System.Drawing.Font("Segoe UI Semibold", 13.0!, System.Drawing.FontStyle.Bold)
        Me.LblHeaderForm.ForeColor = System.Drawing.Color.White
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(480, 50)
        Me.LblHeaderForm.TabIndex = 0
        Me.LblHeaderForm.Text = "Edit Pembayaran Penjualan"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelBody
        '
        Me.PanelBody.AutoSize = True
        Me.PanelBody.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.PanelBody.Controls.Add(Me.TablePembayaran)
        Me.PanelBody.Controls.Add(Me.PanelSeparator)
        Me.PanelBody.Controls.Add(Me.TableHeader)
        Me.PanelBody.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelBody.Location = New System.Drawing.Point(0, 50)
        Me.PanelBody.Name = "PanelBody"
        Me.PanelBody.Padding = New System.Windows.Forms.Padding(12, 10, 12, 8)
        Me.PanelBody.Size = New System.Drawing.Size(480, 311)
        Me.PanelBody.TabIndex = 1
        '
        'TablePembayaran
        '
        Me.TablePembayaran.AutoSize = True
        Me.TablePembayaran.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TablePembayaran.ColumnCount = 3
        Me.TablePembayaran.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140.0!))
        Me.TablePembayaran.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.0!))
        Me.TablePembayaran.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160.0!))
        Me.TablePembayaran.Controls.Add(Me.Label10, 0, 0)
        Me.TablePembayaran.Controls.Add(Me.lblTotalBelanja, 1, 0)
        Me.TablePembayaran.Controls.Add(Me.lblTotalFmt, 2, 0)
        Me.TablePembayaran.Controls.Add(Me.Label7, 0, 1)
        Me.TablePembayaran.Controls.Add(Me.cmbBayarTunai, 1, 1)
        Me.TablePembayaran.Controls.Add(Me.txtNominalTunai, 2, 1)
        Me.TablePembayaran.Controls.Add(Me.Label6, 0, 2)
        Me.TablePembayaran.Controls.Add(Me.txtKodeBayarTunai, 1, 2)
        Me.TablePembayaran.Controls.Add(Me.Label8, 0, 3)
        Me.TablePembayaran.Controls.Add(Me.cmbBayarTransfer, 1, 3)
        Me.TablePembayaran.Controls.Add(Me.txtNominalTransfer, 2, 3)
        Me.TablePembayaran.Controls.Add(Me.Label9, 0, 4)
        Me.TablePembayaran.Controls.Add(Me.txtKodeBayarTransfer, 1, 4)
        Me.TablePembayaran.Controls.Add(Me.lblHasilCaption, 0, 5)
        Me.TablePembayaran.Controls.Add(Me.lblHasilValue, 1, 5)
        Me.TablePembayaran.Controls.Add(Me.lblJatuhTempo, 0, 6)
        Me.TablePembayaran.Controls.Add(Me.dtpJatuhTempo, 1, 6)
        Me.TablePembayaran.Controls.Add(Me.Label5, 0, 7)
        Me.TablePembayaran.Controls.Add(Me.lblStatusValue, 1, 7)
        Me.TablePembayaran.Dock = System.Windows.Forms.DockStyle.Top
        Me.TablePembayaran.Location = New System.Drawing.Point(12, 123)
        Me.TablePembayaran.Name = "TablePembayaran"
        Me.TablePembayaran.RowCount = 8
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0!))
        Me.TablePembayaran.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.TablePembayaran.Size = New System.Drawing.Size(456, 180)
        Me.TablePembayaran.TabIndex = 1
        '
        'Label10
        '
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label10.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label10.Location = New System.Drawing.Point(3, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(134, 36)
        Me.Label10.TabIndex = 0
        Me.Label10.Text = "Total Belanja"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTotalBelanja
        '
        Me.lblTotalBelanja.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalBelanja.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotalBelanja.Location = New System.Drawing.Point(143, 0)
        Me.lblTotalBelanja.Name = "lblTotalBelanja"
        Me.lblTotalBelanja.Padding = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblTotalBelanja.Size = New System.Drawing.Size(150, 36)
        Me.lblTotalBelanja.TabIndex = 1
        Me.lblTotalBelanja.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTotalFmt
        '
        Me.lblTotalFmt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalFmt.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotalFmt.Location = New System.Drawing.Point(299, 0)
        Me.lblTotalFmt.Name = "lblTotalFmt"
        Me.lblTotalFmt.Padding = New System.Windows.Forms.Padding(6, 0, 4, 0)
        Me.lblTotalFmt.Size = New System.Drawing.Size(154, 36)
        Me.lblTotalFmt.TabIndex = 2
        Me.lblTotalFmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label7.Location = New System.Drawing.Point(3, 36)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(134, 36)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "Bayar Tunai"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbBayarTunai
        '
        Me.cmbBayarTunai.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbBayarTunai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBayarTunai.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.cmbBayarTunai.FormattingEnabled = True
        Me.cmbBayarTunai.Location = New System.Drawing.Point(142, 40)
        Me.cmbBayarTunai.Margin = New System.Windows.Forms.Padding(2, 4, 2, 4)
        Me.cmbBayarTunai.Name = "cmbBayarTunai"
        Me.cmbBayarTunai.Size = New System.Drawing.Size(152, 25)
        Me.cmbBayarTunai.TabIndex = 2
        '
        'txtNominalTunai
        '
        Me.txtNominalTunai.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtNominalTunai.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.txtNominalTunai.Location = New System.Drawing.Point(298, 40)
        Me.txtNominalTunai.Margin = New System.Windows.Forms.Padding(2, 4, 4, 4)
        Me.txtNominalTunai.Name = "txtNominalTunai"
        Me.txtNominalTunai.Size = New System.Drawing.Size(154, 25)
        Me.txtNominalTunai.TabIndex = 1
        Me.txtNominalTunai.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(3, 72)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 1)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Kode Akun"
        Me.Label6.Visible = False
        '
        'txtKodeBayarTunai
        '
        Me.txtKodeBayarTunai.Location = New System.Drawing.Point(143, 75)
        Me.txtKodeBayarTunai.Name = "txtKodeBayarTunai"
        Me.txtKodeBayarTunai.ReadOnly = True
        Me.txtKodeBayarTunai.Size = New System.Drawing.Size(100, 25)
        Me.txtKodeBayarTunai.TabIndex = 6
        Me.txtKodeBayarTunai.TabStop = False
        Me.txtKodeBayarTunai.Visible = False
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label8.Location = New System.Drawing.Point(3, 72)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(134, 36)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "Bayar Transfer"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbBayarTransfer
        '
        Me.cmbBayarTransfer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbBayarTransfer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBayarTransfer.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.cmbBayarTransfer.FormattingEnabled = True
        Me.cmbBayarTransfer.Location = New System.Drawing.Point(142, 76)
        Me.cmbBayarTransfer.Margin = New System.Windows.Forms.Padding(2, 4, 2, 4)
        Me.cmbBayarTransfer.Name = "cmbBayarTransfer"
        Me.cmbBayarTransfer.Size = New System.Drawing.Size(152, 25)
        Me.cmbBayarTransfer.TabIndex = 4
        '
        'txtNominalTransfer
        '
        Me.txtNominalTransfer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtNominalTransfer.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.txtNominalTransfer.Location = New System.Drawing.Point(298, 76)
        Me.txtNominalTransfer.Margin = New System.Windows.Forms.Padding(2, 4, 4, 4)
        Me.txtNominalTransfer.Name = "txtNominalTransfer"
        Me.txtNominalTransfer.Size = New System.Drawing.Size(154, 25)
        Me.txtNominalTransfer.TabIndex = 3
        Me.txtNominalTransfer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(3, 108)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(100, 1)
        Me.Label9.TabIndex = 9
        Me.Label9.Text = "Kode Akun"
        Me.Label9.Visible = False
        '
        'txtKodeBayarTransfer
        '
        Me.txtKodeBayarTransfer.Location = New System.Drawing.Point(143, 111)
        Me.txtKodeBayarTransfer.Name = "txtKodeBayarTransfer"
        Me.txtKodeBayarTransfer.ReadOnly = True
        Me.txtKodeBayarTransfer.Size = New System.Drawing.Size(100, 25)
        Me.txtKodeBayarTransfer.TabIndex = 10
        Me.txtKodeBayarTransfer.TabStop = False
        Me.txtKodeBayarTransfer.Visible = False
        '
        'lblHasilCaption
        '
        Me.lblHasilCaption.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHasilCaption.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblHasilCaption.Location = New System.Drawing.Point(3, 108)
        Me.lblHasilCaption.Name = "lblHasilCaption"
        Me.lblHasilCaption.Size = New System.Drawing.Size(134, 36)
        Me.lblHasilCaption.TabIndex = 13
        Me.lblHasilCaption.Text = "Kembalian :"
        Me.lblHasilCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblHasilValue
        '
        Me.TablePembayaran.SetColumnSpan(Me.lblHasilValue, 2)
        Me.lblHasilValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHasilValue.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblHasilValue.Location = New System.Drawing.Point(143, 108)
        Me.lblHasilValue.Name = "lblHasilValue"
        Me.lblHasilValue.Padding = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblHasilValue.Size = New System.Drawing.Size(310, 36)
        Me.lblHasilValue.TabIndex = 15
        Me.lblHasilValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblJatuhTempo
        '
        Me.lblJatuhTempo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblJatuhTempo.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblJatuhTempo.Location = New System.Drawing.Point(3, 144)
        Me.lblJatuhTempo.Name = "lblJatuhTempo"
        Me.lblJatuhTempo.Size = New System.Drawing.Size(134, 1)
        Me.lblJatuhTempo.TabIndex = 16
        Me.lblJatuhTempo.Text = "Jatuh Tempo"
        Me.lblJatuhTempo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblJatuhTempo.Visible = False
        '
        'dtpJatuhTempo
        '
        Me.dtpJatuhTempo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dtpJatuhTempo.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.dtpJatuhTempo.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpJatuhTempo.Location = New System.Drawing.Point(142, 148)
        Me.dtpJatuhTempo.Margin = New System.Windows.Forms.Padding(2, 4, 2, 4)
        Me.dtpJatuhTempo.Name = "dtpJatuhTempo"
        Me.dtpJatuhTempo.Size = New System.Drawing.Size(152, 25)
        Me.dtpJatuhTempo.TabIndex = 5
        Me.dtpJatuhTempo.Visible = False
        '
        'Label5
        '
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label5.Location = New System.Drawing.Point(3, 144)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(134, 36)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Status"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblStatusValue
        '
        Me.lblStatusValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblStatusValue.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblStatusValue.Location = New System.Drawing.Point(143, 144)
        Me.lblStatusValue.Name = "lblStatusValue"
        Me.lblStatusValue.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblStatusValue.Size = New System.Drawing.Size(150, 36)
        Me.lblStatusValue.TabIndex = 9
        Me.lblStatusValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelSeparator
        '
        Me.PanelSeparator.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelSeparator.Location = New System.Drawing.Point(12, 122)
        Me.PanelSeparator.Name = "PanelSeparator"
        Me.PanelSeparator.Size = New System.Drawing.Size(456, 1)
        Me.PanelSeparator.TabIndex = 2
        '
        'TableHeader
        '
        Me.TableHeader.AutoSize = True
        Me.TableHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableHeader.ColumnCount = 2
        Me.TableHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130.0!))
        Me.TableHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableHeader.Controls.Add(Me.Label1, 0, 0)
        Me.TableHeader.Controls.Add(Me.lblFakturValue, 1, 0)
        Me.TableHeader.Controls.Add(Me.Label2, 0, 1)
        Me.TableHeader.Controls.Add(Me.lblPelangganValue, 1, 1)
        Me.TableHeader.Controls.Add(Me.Label3, 0, 2)
        Me.TableHeader.Controls.Add(Me.lblAlamatPelangganValue, 1, 2)
        Me.TableHeader.Controls.Add(Me.Label4, 0, 3)
        Me.TableHeader.Controls.Add(Me.lblJenisPelangganValue, 1, 3)
        Me.TableHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableHeader.Location = New System.Drawing.Point(12, 10)
        Me.TableHeader.Name = "TableHeader"
        Me.TableHeader.RowCount = 4
        Me.TableHeader.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableHeader.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableHeader.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableHeader.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableHeader.Size = New System.Drawing.Size(456, 112)
        Me.TableHeader.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(124, 28)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "No. Faktur"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFakturValue
        '
        Me.lblFakturValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblFakturValue.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.lblFakturValue.Location = New System.Drawing.Point(133, 0)
        Me.lblFakturValue.Name = "lblFakturValue"
        Me.lblFakturValue.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblFakturValue.Size = New System.Drawing.Size(320, 28)
        Me.lblFakturValue.TabIndex = 1
        Me.lblFakturValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.Label2.Location = New System.Drawing.Point(3, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(124, 28)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Pelanggan"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPelangganValue
        '
        Me.lblPelangganValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPelangganValue.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.lblPelangganValue.Location = New System.Drawing.Point(133, 28)
        Me.lblPelangganValue.Name = "lblPelangganValue"
        Me.lblPelangganValue.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblPelangganValue.Size = New System.Drawing.Size(320, 28)
        Me.lblPelangganValue.TabIndex = 3
        Me.lblPelangganValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.Label3.Location = New System.Drawing.Point(3, 56)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(124, 28)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Alamat"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAlamatPelangganValue
        '
        Me.lblAlamatPelangganValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblAlamatPelangganValue.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.lblAlamatPelangganValue.Location = New System.Drawing.Point(133, 56)
        Me.lblAlamatPelangganValue.Name = "lblAlamatPelangganValue"
        Me.lblAlamatPelangganValue.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblAlamatPelangganValue.Size = New System.Drawing.Size(320, 28)
        Me.lblAlamatPelangganValue.TabIndex = 5
        Me.lblAlamatPelangganValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.Label4.Location = New System.Drawing.Point(3, 84)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(124, 28)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Jenis Pelanggan"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblJenisPelangganValue
        '
        Me.lblJenisPelangganValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblJenisPelangganValue.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.lblJenisPelangganValue.Location = New System.Drawing.Point(133, 84)
        Me.lblJenisPelangganValue.Name = "lblJenisPelangganValue"
        Me.lblJenisPelangganValue.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblJenisPelangganValue.Size = New System.Drawing.Size(320, 28)
        Me.lblJenisPelangganValue.TabIndex = 7
        Me.lblJenisPelangganValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTunaiFmt
        '
        Me.lblTunaiFmt.Location = New System.Drawing.Point(0, 0)
        Me.lblTunaiFmt.Name = "lblTunaiFmt"
        Me.lblTunaiFmt.Size = New System.Drawing.Size(100, 23)
        Me.lblTunaiFmt.TabIndex = 11
        Me.lblTunaiFmt.Visible = False
        '
        'lblTransferFmt
        '
        Me.lblTransferFmt.Location = New System.Drawing.Point(0, 0)
        Me.lblTransferFmt.Name = "lblTransferFmt"
        Me.lblTransferFmt.Size = New System.Drawing.Size(100, 23)
        Me.lblTransferFmt.TabIndex = 12
        Me.lblTransferFmt.Visible = False
        '
        'PanelFooter
        '
        Me.PanelFooter.Controls.Add(Me.BtnBatal)
        Me.PanelFooter.Controls.Add(Me.BtnSimpan)
        Me.PanelFooter.Controls.Add(Me.PanelInfoTransfer)
        Me.PanelFooter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFooter.Location = New System.Drawing.Point(0, 361)
        Me.PanelFooter.Name = "PanelFooter"
        Me.PanelFooter.Padding = New System.Windows.Forms.Padding(12, 10, 12, 10)
        Me.PanelFooter.Size = New System.Drawing.Size(480, 60)
        Me.PanelFooter.TabIndex = 2
        '
        'BtnBatal
        '
        Me.BtnBatal.AutoSize = True
        Me.BtnBatal.BackColor = System.Drawing.Color.White
        Me.BtnBatal.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnBatal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(165, Byte), Integer))
        Me.BtnBatal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.BtnBatal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnBatal.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnBatal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.BtnBatal.Image = CType(resources.GetObject("BtnBatal.Image"), System.Drawing.Image)
        Me.BtnBatal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnBatal.Location = New System.Drawing.Point(348, 18)
        Me.BtnBatal.Name = "BtnBatal"
        Me.BtnBatal.Size = New System.Drawing.Size(110, 32)
        Me.BtnBatal.TabIndex = 11
        Me.BtnBatal.Text = "Batal (Esc)"
        Me.BtnBatal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnBatal.UseVisualStyleBackColor = False
        '
        'BtnSimpan
        '
        Me.BtnSimpan.AutoSize = True
        Me.BtnSimpan.BackColor = System.Drawing.Color.White
        Me.BtnSimpan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(187, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.BtnSimpan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(252, Byte), Integer), CType(CType(231, Byte), Integer))
        Me.BtnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSimpan.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSimpan.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(163, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.BtnSimpan.Image = CType(resources.GetObject("BtnSimpan.Image"), System.Drawing.Image)
        Me.BtnSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnSimpan.Location = New System.Drawing.Point(12, 18)
        Me.BtnSimpan.Name = "BtnSimpan"
        Me.BtnSimpan.Size = New System.Drawing.Size(114, 32)
        Me.BtnSimpan.TabIndex = 10
        Me.BtnSimpan.Text = "Simpan (F8)"
        Me.BtnSimpan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnSimpan.UseVisualStyleBackColor = False
        '
        'PanelInfoTransfer
        '
        Me.PanelInfoTransfer.Controls.Add(Me.Label11)
        Me.PanelInfoTransfer.Controls.Add(Me.cmbBankPengirim)
        Me.PanelInfoTransfer.Controls.Add(Me.Label12)
        Me.PanelInfoTransfer.Controls.Add(Me.txtNoRek)
        Me.PanelInfoTransfer.Controls.Add(Me.Label13)
        Me.PanelInfoTransfer.Controls.Add(Me.txtNamaRek)
        Me.PanelInfoTransfer.Controls.Add(Me.Label14)
        Me.PanelInfoTransfer.Controls.Add(Me.txtNoReff)
        Me.PanelInfoTransfer.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelInfoTransfer.Location = New System.Drawing.Point(12, 10)
        Me.PanelInfoTransfer.Name = "PanelInfoTransfer"
        Me.PanelInfoTransfer.Padding = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.PanelInfoTransfer.Size = New System.Drawing.Size(456, 135)
        Me.PanelInfoTransfer.TabIndex = 0
        Me.PanelInfoTransfer.Visible = False
        '
        'Label11
        '
        Me.Label11.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label11.Location = New System.Drawing.Point(0, 10)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(120, 24)
        Me.Label11.TabIndex = 0
        Me.Label11.Text = "Bank Pengirim"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbBankPengirim
        '
        Me.cmbBankPengirim.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.cmbBankPengirim.FormattingEnabled = True
        Me.cmbBankPengirim.Items.AddRange(New Object() {"BCA", "BNI", "BRI", "Mandiri", "BSI", "CIMB", "Danamon", "Permata", "Lainnya"})
        Me.cmbBankPengirim.Location = New System.Drawing.Point(125, 8)
        Me.cmbBankPengirim.Name = "cmbBankPengirim"
        Me.cmbBankPengirim.Size = New System.Drawing.Size(200, 25)
        Me.cmbBankPengirim.TabIndex = 6
        '
        'Label12
        '
        Me.Label12.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label12.Location = New System.Drawing.Point(0, 40)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(120, 24)
        Me.Label12.TabIndex = 2
        Me.Label12.Text = "No. Rekening"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtNoRek
        '
        Me.txtNoRek.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.txtNoRek.Location = New System.Drawing.Point(125, 38)
        Me.txtNoRek.Name = "txtNoRek"
        Me.txtNoRek.Size = New System.Drawing.Size(200, 25)
        Me.txtNoRek.TabIndex = 7
        '
        'Label13
        '
        Me.Label13.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label13.Location = New System.Drawing.Point(0, 70)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(120, 24)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "Nama Rekening"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtNamaRek
        '
        Me.txtNamaRek.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.txtNamaRek.Location = New System.Drawing.Point(125, 68)
        Me.txtNamaRek.Name = "txtNamaRek"
        Me.txtNamaRek.Size = New System.Drawing.Size(200, 25)
        Me.txtNamaRek.TabIndex = 8
        '
        'Label14
        '
        Me.Label14.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label14.Location = New System.Drawing.Point(0, 100)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(120, 22)
        Me.Label14.TabIndex = 6
        Me.Label14.Text = "No. Referensi"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtNoReff
        '
        Me.txtNoReff.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.txtNoReff.Location = New System.Drawing.Point(125, 98)
        Me.txtNoReff.Name = "txtNoReff"
        Me.txtNoReff.Size = New System.Drawing.Size(200, 25)
        Me.txtNoReff.TabIndex = 9
        '
        'FormEditBayarJual
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(480, 520)
        Me.Controls.Add(Me.PanelFooter)
        Me.Controls.Add(Me.PanelBody)
        Me.Controls.Add(Me.PanelHeader)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormEditBayarJual"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Edit Pembayaran Penjualan"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelBody.ResumeLayout(False)
        Me.PanelBody.PerformLayout()
        Me.TablePembayaran.ResumeLayout(False)
        Me.TablePembayaran.PerformLayout()
        Me.TableHeader.ResumeLayout(False)
        Me.PanelFooter.ResumeLayout(False)
        Me.PanelFooter.PerformLayout()
        Me.PanelInfoTransfer.ResumeLayout(False)
        Me.PanelInfoTransfer.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents PanelBody As System.Windows.Forms.Panel
    Friend WithEvents TableHeader As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblFakturValue As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblPelangganValue As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblAlamatPelangganValue As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblJenisPelangganValue As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblStatusValue As System.Windows.Forms.Label
    Friend WithEvents PanelSeparator As System.Windows.Forms.Panel
    Friend WithEvents TablePembayaran As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents lblTotalBelanja As System.Windows.Forms.Label
    Friend WithEvents lblTotalFmt As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents cmbBayarTunai As System.Windows.Forms.ComboBox
    Friend WithEvents txtNominalTunai As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtKodeBayarTunai As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cmbBayarTransfer As System.Windows.Forms.ComboBox
    Friend WithEvents txtNominalTransfer As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtKodeBayarTransfer As System.Windows.Forms.TextBox
    Friend WithEvents lblTunaiFmt As System.Windows.Forms.Label
    Friend WithEvents lblTransferFmt As System.Windows.Forms.Label
    Friend WithEvents lblHasilCaption As System.Windows.Forms.Label
    Friend WithEvents lblHasilValue As System.Windows.Forms.Label
    Friend WithEvents lblJatuhTempo As System.Windows.Forms.Label
    Friend WithEvents dtpJatuhTempo As System.Windows.Forms.DateTimePicker
    Friend WithEvents PanelFooter As System.Windows.Forms.Panel
    Friend WithEvents PanelInfoTransfer As System.Windows.Forms.Panel
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents cmbBankPengirim As System.Windows.Forms.ComboBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtNoRek As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtNamaRek As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtNoReff As System.Windows.Forms.TextBox
    Friend WithEvents BtnSimpan As System.Windows.Forms.Button
    Friend WithEvents BtnBatal As System.Windows.Forms.Button
    Friend WithEvents Panel1 As Panel
End Class










