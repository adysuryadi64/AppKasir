<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormCekUpdate
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCekUpdate))
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.LblNama = New System.Windows.Forms.Label()
        Me.PanelBody = New System.Windows.Forms.Panel()
        Me.rtbChangelog = New System.Windows.Forms.RichTextBox()
        Me.PanelVersi = New System.Windows.Forms.Panel()
        Me.lblVersiTerbaru = New System.Windows.Forms.Label()
        Me.lblVersiInstalled = New System.Windows.Forms.Label()
        Me.lblTitleTerbaru = New System.Windows.Forms.Label()
        Me.lblTitleInstalled = New System.Windows.Forms.Label()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.PanelFooter = New System.Windows.Forms.Panel()
        Me.btnCekUpdate = New System.Windows.Forms.Button()
        Me.PanelHeader.SuspendLayout()
        Me.PanelBody.SuspendLayout()
        Me.PanelVersi.SuspendLayout()
        Me.PanelFooter.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.Controls.Add(Me.BtnClose)
        Me.PanelHeader.Controls.Add(Me.LblNama)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(765, 36)
        Me.PanelHeader.TabIndex = 0
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Transparent
        Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnClose.FlatAppearance.BorderSize = 0
        Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Location = New System.Drawing.Point(734, 6)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(23, 23)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'LblNama
        '
        Me.LblNama.BackColor = System.Drawing.Color.Transparent
        Me.LblNama.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblNama.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LblNama.Location = New System.Drawing.Point(0, 0)
        Me.LblNama.Name = "LblNama"
        Me.LblNama.Size = New System.Drawing.Size(765, 36)
        Me.LblNama.TabIndex = 1
        Me.LblNama.Text = "  Cek Update Aplikasi"
        Me.LblNama.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelBody
        '
        Me.PanelBody.Controls.Add(Me.rtbChangelog)
        Me.PanelBody.Controls.Add(Me.PanelVersi)
        Me.PanelBody.Controls.Add(Me.lblStatus)
        Me.PanelBody.Controls.Add(Me.ProgressBar)
        Me.PanelBody.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelBody.Location = New System.Drawing.Point(0, 36)
        Me.PanelBody.Name = "PanelBody"
        Me.PanelBody.Padding = New System.Windows.Forms.Padding(24, 20, 24, 0)
        Me.PanelBody.Size = New System.Drawing.Size(765, 452)
        Me.PanelBody.TabIndex = 1
        '
        'rtbChangelog
        '
        Me.rtbChangelog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rtbChangelog.BackColor = System.Drawing.Color.White
        Me.rtbChangelog.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbChangelog.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.rtbChangelog.Location = New System.Drawing.Point(24, 155)
        Me.rtbChangelog.Name = "rtbChangelog"
        Me.rtbChangelog.ReadOnly = True
        Me.rtbChangelog.Size = New System.Drawing.Size(717, 277)
        Me.rtbChangelog.TabIndex = 29
        Me.rtbChangelog.Text = ""
        '
        'PanelVersi
        '
        Me.PanelVersi.Controls.Add(Me.lblVersiTerbaru)
        Me.PanelVersi.Controls.Add(Me.lblVersiInstalled)
        Me.PanelVersi.Controls.Add(Me.lblTitleTerbaru)
        Me.PanelVersi.Controls.Add(Me.lblTitleInstalled)
        Me.PanelVersi.Location = New System.Drawing.Point(24, 20)
        Me.PanelVersi.Name = "PanelVersi"
        Me.PanelVersi.Padding = New System.Windows.Forms.Padding(16, 12, 16, 12)
        Me.PanelVersi.Size = New System.Drawing.Size(717, 80)
        Me.PanelVersi.TabIndex = 0
        '
        'lblVersiTerbaru
        '
        Me.lblVersiTerbaru.AutoSize = True
        Me.lblVersiTerbaru.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold)
        Me.lblVersiTerbaru.Location = New System.Drawing.Point(375, 30)
        Me.lblVersiTerbaru.Name = "lblVersiTerbaru"
        Me.lblVersiTerbaru.Size = New System.Drawing.Size(19, 25)
        Me.lblVersiTerbaru.TabIndex = 0
        Me.lblVersiTerbaru.Text = "-"
        '
        'lblVersiInstalled
        '
        Me.lblVersiInstalled.AutoSize = True
        Me.lblVersiInstalled.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold)
        Me.lblVersiInstalled.Location = New System.Drawing.Point(14, 30)
        Me.lblVersiInstalled.Name = "lblVersiInstalled"
        Me.lblVersiInstalled.Size = New System.Drawing.Size(19, 25)
        Me.lblVersiInstalled.TabIndex = 1
        Me.lblVersiInstalled.Text = "-"
        '
        'lblTitleTerbaru
        '
        Me.lblTitleTerbaru.AutoSize = True
        Me.lblTitleTerbaru.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.lblTitleTerbaru.Location = New System.Drawing.Point(377, 12)
        Me.lblTitleTerbaru.Name = "lblTitleTerbaru"
        Me.lblTitleTerbaru.Size = New System.Drawing.Size(72, 13)
        Me.lblTitleTerbaru.TabIndex = 2
        Me.lblTitleTerbaru.Text = "Versi Terbaru"
        '
        'lblTitleInstalled
        '
        Me.lblTitleInstalled.AutoSize = True
        Me.lblTitleInstalled.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.lblTitleInstalled.Location = New System.Drawing.Point(16, 12)
        Me.lblTitleInstalled.Name = "lblTitleInstalled"
        Me.lblTitleInstalled.Size = New System.Drawing.Size(86, 13)
        Me.lblTitleInstalled.TabIndex = 3
        Me.lblTitleInstalled.Text = "Versi Terpasang"
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblStatus.Location = New System.Drawing.Point(24, 113)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(717, 20)
        Me.lblStatus.TabIndex = 30
        Me.lblStatus.Text = "Siap untuk cek update."
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ProgressBar
        '
        Me.ProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar.Location = New System.Drawing.Point(24, 141)
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(717, 8)
        Me.ProgressBar.TabIndex = 28
        Me.ProgressBar.Visible = False
        '
        'PanelFooter
        '
        Me.PanelFooter.Controls.Add(Me.btnCekUpdate)
        Me.PanelFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelFooter.Location = New System.Drawing.Point(0, 488)
        Me.PanelFooter.Name = "PanelFooter"
        Me.PanelFooter.Padding = New System.Windows.Forms.Padding(24, 12, 24, 12)
        Me.PanelFooter.Size = New System.Drawing.Size(765, 60)
        Me.PanelFooter.TabIndex = 2
        '
        'btnCekUpdate
        '
        Me.btnCekUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCekUpdate.BackColor = System.Drawing.SystemColors.MenuHighlight
        Me.btnCekUpdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCekUpdate.FlatAppearance.BorderSize = 0
        Me.btnCekUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCekUpdate.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.btnCekUpdate.ForeColor = System.Drawing.Color.White
        Me.btnCekUpdate.Location = New System.Drawing.Point(605, 12)
        Me.btnCekUpdate.Name = "btnCekUpdate"
        Me.btnCekUpdate.Size = New System.Drawing.Size(136, 36)
        Me.btnCekUpdate.TabIndex = 29
        Me.btnCekUpdate.Text = "Cek Update"
        Me.btnCekUpdate.UseVisualStyleBackColor = False
        '
        'FormCekUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(765, 548)
        Me.Controls.Add(Me.PanelBody)
        Me.Controls.Add(Me.PanelFooter)
        Me.Controls.Add(Me.PanelHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "FormCekUpdate"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormCekUpdate"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelBody.ResumeLayout(False)
        Me.PanelVersi.ResumeLayout(False)
        Me.PanelVersi.PerformLayout()
        Me.PanelFooter.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents BtnClose As Button
    Friend WithEvents LblNama As Label
    Friend WithEvents PanelBody As Panel
    Friend WithEvents PanelVersi As Panel
    Friend WithEvents lblTitleInstalled As Label
    Friend WithEvents lblVersiInstalled As Label
    Friend WithEvents lblTitleTerbaru As Label
    Friend WithEvents lblVersiTerbaru As Label
    Friend WithEvents lblStatus As Label
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents btnCekUpdate As Button
    Friend WithEvents rtbChangelog As RichTextBox
End Class
