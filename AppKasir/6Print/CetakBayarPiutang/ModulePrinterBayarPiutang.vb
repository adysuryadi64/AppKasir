' ================================================================
' ModulePrinterBayarPiutang — Bukti bayar piutang pelanggan
' Prefix: "BP_"
' ================================================================
Module ModulePrinterBayarPiutang

    Public BP_NoBayar As String = ""
    Public BP_Tanggal As DateTime
    Public BP_NamaPelanggan As String = ""
    Public BP_TotalPiutang As Decimal
    Public BP_NominalBayar As Decimal
    Public BP_SisaPiutang As Decimal
    Public BP_IdUser As String = ""
    Public BP_IdKomputer As String = ""
    Public BP_Lokasi As String = ""

    Public BP_DaftarDetail As New List(Of ItemBayarPiutang)

    Public Class ItemBayarPiutang
        Public IdJual As String = ""
        Public TanggalJual As DateTime
        Public Piutang As Decimal
        Public Pembayaran As Decimal
        Public Sisa As Decimal
        Public JatuhTempo As String = ""
        Public Status As String = ""
    End Class

    Public Sub MuatDataBayarPiutang(noBayar As String)
        BP_NoBayar = noBayar
        BP_DaftarDetail.Clear()
        Using cmd As New MySqlCommand(
            "SELECT ID_JUAL, TANGGAL_JUAL, PIUTANG, PEMBAYARAN, HUTANG, JATUH_TEMPO, STATUS " &
            "FROM piutang_detail WHERE ID_BAYAR = @nb ORDER BY TANGGAL_JUAL", conn)
            cmd.Parameters.AddWithValue("@nb", noBayar)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    BP_DaftarDetail.Add(New ItemBayarPiutang With {
                        .IdJual = BPDbStr(rd, "ID_JUAL"),
                        .TanggalJual = If(IsDBNull(rd("TANGGAL_JUAL")), Date.MinValue, Convert.ToDateTime(rd("TANGGAL_JUAL"))),
                        .Piutang = BPDbDec(rd, "PIUTANG"),
                        .Pembayaran = BPDbDec(rd, "PEMBAYARAN"),
                        .Sisa = BPDbDec(rd, "HUTANG"),
                        .JatuhTempo = If(IsDBNull(rd("JATUH_TEMPO")), "", Convert.ToDateTime(rd("JATUH_TEMPO")).ToString("dd-MM-yyyy")),
                        .Status = BPDbStr(rd, "STATUS")
                    })
                End While
            End Using
        End Using
        Using cmd As New MySqlCommand(
            "SELECT NAMA_PELANGGAN, TGL_BAYAR, TOTAL_PIUTANG, NOMINAL_BAYAR, SISA_PIUTANG, " &
            "ID_USER_BAYAR, ID_KOMPUTER_BAYAR, LOKASI " &
            "FROM piutang WHERE ID_BAYAR_PIUTANG = @nb", conn)
            cmd.Parameters.AddWithValue("@nb", noBayar)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    BP_NamaPelanggan = BPDbStr(rd, "NAMA_PELANGGAN")
                    BP_Tanggal = If(IsDBNull(rd("TGL_BAYAR")), Date.MinValue, Convert.ToDateTime(rd("TGL_BAYAR")))
                    BP_TotalPiutang = BPDbDec(rd, "TOTAL_PIUTANG")
                    BP_NominalBayar = BPDbDec(rd, "NOMINAL_BAYAR")
                    BP_SisaPiutang = BPDbDec(rd, "SISA_PIUTANG")
                    BP_IdUser = BPDbStr(rd, "ID_USER_BAYAR")
                    BP_IdKomputer = BPDbStr(rd, "ID_KOMPUTER_BAYAR")
                    BP_Lokasi = BPDbStr(rd, "LOKASI")
                End If
            End Using
        End Using
    End Sub

    Public Function BPRp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function BPDbStr(rd As MySqlDataReader, k As String) As String
        Return If(IsDBNull(rd(k)), "", rd(k).ToString().Trim())
    End Function
    Friend Function BPDbDec(rd As MySqlDataReader, k As String) As Decimal
        If IsDBNull(rd(k)) Then Return 0
        Dim v As Decimal : Return If(Decimal.TryParse(rd(k).ToString(), v), v, 0)
    End Function

    Public Sub TanyaPilihPrinterBayarPiutang(noBayar As String)
        Dim pilihan As String = ""
        Dim frm As New Form() With {.Text = "", .Size = New Size(440, 260), .StartPosition = FormStartPosition.CenterScreen, .ControlBox = False, .FormBorderStyle = FormBorderStyle.FixedDialog, .BackColor = Color.White, .TopMost = True, .KeyPreview = True}
        Dim layout As New TableLayoutPanel() With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 5, .Padding = New Padding(16, 12, 16, 8), .BackColor = Color.White}
        Dim lblJudul As New Label() With {.Text = "PILIH PRINTER", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 6)}
        Dim lblGaris As New Label() With {.Text = "══════════════════════════════", .Font = New Font("Courier New", 9, FontStyle.Bold), .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 8)}
        Dim panel1 As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False, .AutoSize = True, .BackColor = Color.White, .Margin = New Padding(0, 0, 0, 6)}
        Dim btnThermal As New Button() With {.Text = "Thermal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(30, 80, 160), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnThermal.FlatAppearance.BorderSize = 0
        Dim btnDot As New Button() With {.Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnThermal, btnDot, btnInk})
        Dim panel2 As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False, .AutoSize = True, .BackColor = Color.White, .Margin = New Padding(0, 0, 0, 6)}
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel2.Controls.AddRange(New Control() {btnPdf, btnBatal})
        Dim lblPetunjuk As New Label() With {.Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8), .ForeColor = Color.Gray, .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0)}
        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris) : layout.Controls.Add(panel1) : layout.Controls.Add(panel2) : layout.Controls.Add(lblPetunjuk)
        frm.Controls.Add(layout)
        AddHandler btnThermal.Click, Sub(s, ev)
                pilihan = "Printer Thermal"
                frm.Close()
            End Sub
        AddHandler btnDot.Click, Sub(s, ev)
                pilihan = "Printer Dot Matrix"
                frm.Close()
            End Sub
        AddHandler btnInk.Click, Sub(s, ev)
                pilihan = "Printer Inkjet / Laser"
                frm.Close()
            End Sub
        AddHandler btnPdf.Click, Sub(s, ev)
                pilihan = "Export ke PDF"
                frm.Close()
            End Sub
        AddHandler btnBatal.Click, Sub(s, ev)
                frm.Close()
            End Sub
        AddHandler frm.KeyDown, Sub(s, ev)
                If ev.KeyCode = Keys.Escape Then frm.Close()
            End Sub
        frm.ShowDialog()
        If Not String.IsNullOrEmpty(pilihan) Then CetakBayarPiutang(noBayar, pilihan)
    End Sub

    Public Sub CetakBayarPiutang(noBayar As String)
        CetakBayarPiutang(noBayar, "")
    End Sub

    Public Sub CetakBayarPiutang(noBayar As String, jenis As String)
        If String.IsNullOrEmpty(noBayar) Then Exit Sub
        MuatDataBayarPiutang(noBayar)
        Dim cfg As New KonfigurasiThermal("BayarPiutang")
        Dim j As String = If(String.IsNullOrEmpty(jenis), cfg.JenisPrinter, jenis)
        Try
            Select Case j
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBayarPiutang() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakBayarPiutang("BayarPiutang") : c.CetakThermal()
                    End If
                Case "Printer Dot Matrix"
                    Dim cfgDot As New KonfigurasiDotMatrix("BayarPiutang")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBayarPiutang() : c.CetakDotMatrix()
                    Else
                        Dim c As New EscPosCetakBayarPiutang("BayarPiutang") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakBayarPiutangInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("BayarPiutang")
                    ModuleCetakBayarPiutangPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New EscPosCetakBayarPiutang("BayarPiutang") : c.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
