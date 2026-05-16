<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLapBBPembantu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLapBBPembantu))
        Me.LblHeaderForm = New System.Windows.Forms.Label()
        Me.PanelFilter = New System.Windows.Forms.Panel()
        Me.LabelEntitas = New System.Windows.Forms.Label()
        Me.CmbEntitas = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DTPAwal = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.DTPAkhir = New System.Windows.Forms.DateTimePicker()
        Me.BtnTampil = New System.Windows.Forms.Button()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.PanelTotal = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtTotalDebet = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtTotalKredit = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtSaldoAkhir = New System.Windows.Forms.TextBox()
        Me.PanelFilter.SuspendLayout()
        Me.PanelTotal.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblHeaderForm
        '
        Me.LblHeaderForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblHeaderForm.Font = New System.Drawing.Font("Bookman Old Style", 13.0!, System.Drawing.FontStyle.Bold)
        Me.LblHeaderForm.ForeColor = System.Drawing.Color.DarkRed
        Me.LblHeaderForm.Location = New System.Drawing.Point(0, 0)
        Me.LblHeaderForm.Name = "LblHeaderForm"
        Me.LblHeaderForm.Size = New System.Drawing.Size(1100, 36)
        Me.LblHeaderForm.TabIndex = 3
        Me.LblHeaderForm.Text = "BUKU BESAR PEMBANTU"
        Me.LblHeaderForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PanelFilter
        '
        Me.PanelFilter.Controls.Add(Me.LabelEntitas)
        Me.PanelFilter.Controls.Add(Me.CmbEntitas)
        Me.PanelFilter.Controls.Add(Me.Label1)
        Me.PanelFilter.Controls.Add(Me.DTPAwal)
        Me.PanelFilter.Controls.Add(Me.Label2)
        Me.PanelFilter.Controls.Add(Me.DTPAkhir)
        Me.PanelFilter.Controls.Add(Me.BtnTampil)
        Me.PanelFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFilter.Location = New System.Drawing.Point(0, 36)
        Me.PanelFilter.Name = "PanelFilter"
        Me.PanelFilter.Size = New System.Drawing.Size(1100, 44)
        Me.PanelFilter.TabIndex = 2
        '
        'LabelEntitas
        '
        Me.LabelEntitas.AutoSize = True
        Me.LabelEntitas.Location = New System.Drawing.Point(6, 13)
        Me.LabelEntitas.Name = "LabelEntitas"
        Me.LabelEntitas.Size = New System.Drawing.Size(73, 15)
        Me.LabelEntitas.TabIndex = 0
        Me.LabelEntitas.Text = "Pelanggan :"
        '
        'CmbEntitas
        '
        Me.CmbEntitas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbEntitas.Location = New System.Drawing.Point(80, 9)
        Me.CmbEntitas.Name = "CmbEntitas"
        Me.CmbEntitas.Size = New System.Drawing.Size(200, 23)
        Me.CmbEntitas.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(290, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 15)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Dari :"
        '
        'DTPAwal
        '
        Me.DTPAwal.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAwal.Location = New System.Drawing.Point(330, 9)
        Me.DTPAwal.Name = "DTPAwal"
        Me.DTPAwal.Size = New System.Drawing.Size(110, 21)
        Me.DTPAwal.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(448, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(30, 15)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "s/d :"
        '
        'DTPAkhir
        '
        Me.DTPAkhir.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTPAkhir.Location = New System.Drawing.Point(480, 9)
        Me.DTPAkhir.Name = "DTPAkhir"
        Me.DTPAkhir.Size = New System.Drawing.Size(110, 21)
        Me.DTPAkhir.TabIndex = 5
        '
        'BtnTampil
        '
        Me.BtnTampil.AutoSize = True
        Me.BtnTampil.BackColor = System.Drawing.SystemColors.Control
        Me.BtnTampil.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnTampil.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.BtnTampil.FlatAppearance.BorderSize = 1
        Me.BtnTampil.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.BtnTampil.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.BtnTampil.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnTampil.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTampil.ForeColor = System.Drawing.Color.Black
        Me.BtnTampil.Image = CType(resources.GetObject("BtnTampil.Image"), System.Drawing.Image)
        Me.BtnTampil.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.Location = New System.Drawing.Point(600, 7)
        Me.BtnTampil.Name = "BtnTampil"
        Me.BtnTampil.Size = New System.Drawing.Size(90, 28)
        Me.BtnTampil.TabIndex = 6
        Me.BtnTampil.Text = "Tampilkan (F5)"
        Me.BtnTampil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BtnTampil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BtnTampil.UseVisualStyleBackColor = False
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "AppKasir.ReportBBPembantu.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 80)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(1100, 534)
        Me.ReportViewer1.TabIndex = 0
        '
        'PanelTotal
        '
        Me.PanelTotal.Controls.Add(Me.Label3)
        Me.PanelTotal.Controls.Add(Me.TxtTotalDebet)
        Me.PanelTotal.Controls.Add(Me.Label4)
        Me.PanelTotal.Controls.Add(Me.TxtTotalKredit)
        Me.PanelTotal.Controls.Add(Me.Label5)
        Me.PanelTotal.Controls.Add(Me.TxtSaldoAkhir)
        Me.PanelTotal.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelTotal.Location = New System.Drawing.Point(0, 614)
        Me.PanelTotal.Name = "PanelTotal"
        Me.PanelTotal.Size = New System.Drawing.Size(1100, 36)
        Me.PanelTotal.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.Location = New System.Drawing.Point(6, 10)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 15)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Total Debet :"
        '
        'TxtTotalDebet
        '
        Me.TxtTotalDebet.Location = New System.Drawing.Point(90, 7)
        Me.TxtTotalDebet.Name = "TxtTotalDebet"
        Me.TxtTotalDebet.ReadOnly = True
        Me.TxtTotalDebet.Size = New System.Drawing.Size(130, 21)
        Me.TxtTotalDebet.TabIndex = 1
        Me.TxtTotalDebet.Text = "0"
        Me.TxtTotalDebet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.Location = New System.Drawing.Point(236, 10)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(77, 15)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "Total Kredit :"
        '
        'TxtTotalKredit
        '
        Me.TxtTotalKredit.Location = New System.Drawing.Point(320, 7)
        Me.TxtTotalKredit.Name = "TxtTotalKredit"
        Me.TxtTotalKredit.ReadOnly = True
        Me.TxtTotalKredit.Size = New System.Drawing.Size(130, 21)
        Me.TxtTotalKredit.TabIndex = 3
        Me.TxtTotalKredit.Text = "0"
        Me.TxtTotalKredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.DarkRed
        Me.Label5.Location = New System.Drawing.Point(466, 10)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(78, 15)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Saldo Akhir :"
        '
        'TxtSaldoAkhir
        '
        Me.TxtSaldoAkhir.BackColor = System.Drawing.SystemColors.Control
        Me.TxtSaldoAkhir.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.TxtSaldoAkhir.Location = New System.Drawing.Point(550, 7)
        Me.TxtSaldoAkhir.Name = "TxtSaldoAkhir"
        Me.TxtSaldoAkhir.ReadOnly = True
        Me.TxtSaldoAkhir.Size = New System.Drawing.Size(150, 21)
        Me.TxtSaldoAkhir.TabIndex = 5
        Me.TxtSaldoAkhir.Text = "0"
        Me.TxtSaldoAkhir.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FormLapBBPembantu
        '
        Me.ClientSize = New System.Drawing.Size(1100, 650)
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.PanelTotal)
        Me.Controls.Add(Me.PanelFilter)
        Me.Controls.Add(Me.LblHeaderForm)
        Me.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.Name = "FormLapBBPembantu"
        Me.KeyPreview = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Buku Besar Pembantu"
        Me.PanelFilter.ResumeLayout(False)
        Me.PanelFilter.PerformLayout()
        Me.PanelTotal.ResumeLayout(False)
        Me.PanelTotal.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LblHeaderForm As System.Windows.Forms.Label
    Friend WithEvents PanelFilter As System.Windows.Forms.Panel
    Friend WithEvents LabelEntitas As System.Windows.Forms.Label
    Friend WithEvents CmbEntitas As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DTPAwal As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents DTPAkhir As System.Windows.Forms.DateTimePicker
    Friend WithEvents BtnTampil As System.Windows.Forms.Button
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PanelTotal As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalDebet As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtTotalKredit As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TxtSaldoAkhir As System.Windows.Forms.TextBox

End Class


