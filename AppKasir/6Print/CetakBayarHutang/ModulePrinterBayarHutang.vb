' ================================================================
' ModulePrinterBayarHutang
' Pusat data dan entry point untuk cetak bukti bayar hutang.
' Prefix variabel: "BH_"
'
' Cara pakai:
'   ModulePrinterBayarHutang.CetakBayarHutang(noBayar)
'   ModulePrinterBayarHutang.TanyaPilihPrinterBayarHutang(noBayar)
' ================================================================
Module ModulePrinterBayarHutang

    ' ============================================================
    ' DATA HEADER
    ' ============================================================
    Public BH_NoBayar As String = ""
    Public BH_Tanggal As DateTime
    Public BH_NamaSupplier As String = ""
    Public BH_TotalHutang As Decimal
    Public BH_NominalBayar As Decimal
    Public BH_SisaHutang As Decimal
    Public BH_IdUser As String = ""
    Public BH_IdKomputer As String = ""
    Public BH_Lokasi As String = ""

    ' ── Data detail hutang yang dibayar ──────────────────────
    Public BH_DaftarDetail As New List(Of ItemBayarHutang)

    Public Class ItemBayarHutang
        Public IdBeli As String = ""
        Public TanggalBeli As DateTime
        Public TotalHutang As Decimal
        Public Pembayaran As Decimal
        Public Sisa As Decimal
        Public JatuhTempo As String = ""
        Public Status As String = ""
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' ============================================================
    Public Sub MuatDataBayarHutang(noBayar As String)
        BH_NoBayar = noBayar
        MuatDetailBayarHutang(noBayar)
        MuatHeaderBayarHutang(noBayar)
    End Sub

    Private Sub MuatDetailBayarHutang(noBayar As String)
        BH_DaftarDetail.Clear()
        Using cmd As New MySqlCommand(
            "SELECT ID_BELI, TANGGAL_BELI, TOTAL_HUTANG, PEMBAYARAN, HUTANG, JATUH_TEMPO, STATUS " &
            "FROM hutang_detail WHERE ID_BAYAR = @noBayar ORDER BY TANGGAL_BELI", conn)
            cmd.Parameters.AddWithValue("@noBayar", noBayar)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    BH_DaftarDetail.Add(New ItemBayarHutang With {
                        .IdBeli = BHDbStr(rd, "ID_BELI"),
                        .TanggalBeli = If(IsDBNull(rd("TANGGAL_BELI")), Date.MinValue, Convert.ToDateTime(rd("TANGGAL_BELI"))),
                        .TotalHutang = BHDbDec(rd, "TOTAL_HUTANG"),
                        .Pembayaran = BHDbDec(rd, "PEMBAYARAN"),
                        .Sisa = BHDbDec(rd, "HUTANG"),
                        .JatuhTempo = If(IsDBNull(rd("JATUH_TEMPO")), "", Convert.ToDateTime(rd("JATUH_TEMPO")).ToString("dd-MM-yyyy")),
                        .Status = BHDbStr(rd, "STATUS")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderBayarHutang(noBayar As String)
        Using cmd As New MySqlCommand(
            "SELECT NAMASUPLIYER, TGLPEMBAYARAN, TOTALHUTANG, NOMINALBAYAR, SISAHUTANG, " &
            "ID_USER_BAYAR, ID_KOMPUTER_BAYAR, LOKASI " &
            "FROM hutang WHERE NOBAYARHUTANG = @noBayar", conn)
            cmd.Parameters.AddWithValue("@noBayar", noBayar)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    BH_NamaSupplier = BHDbStr(rd, "NAMASUPLIYER")
                    BH_Tanggal = If(IsDBNull(rd("TGLPEMBAYARAN")), Date.MinValue, Convert.ToDateTime(rd("TGLPEMBAYARAN")))
                    BH_TotalHutang = BHDbDec(rd, "TOTALHUTANG")
                    BH_NominalBayar = BHDbDec(rd, "NOMINALBAYAR")
                    BH_SisaHutang = BHDbDec(rd, "SISAHUTANG")
                    BH_IdUser = BHDbStr(rd, "ID_USER_BAYAR")
                    BH_IdKomputer = BHDbStr(rd, "ID_KOMPUTER_BAYAR")
                    BH_Lokasi = BHDbStr(rd, "LOKASI")
                End If
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function BHRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function BHDbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function BHDbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterBayarHutang(noBayar As String)
        Dim pilihan As String = ""
        Dim frm As New Form() With {
            .Text = "", .Size = New Size(440, 260),
            .StartPosition = FormStartPosition.CenterScreen,
            .ControlBox = False, .FormBorderStyle = FormBorderStyle.FixedDialog,
            .BackColor = Color.White, .TopMost = True, .KeyPreview = True}
        Dim layout As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 5,
            .Padding = New Padding(16, 12, 16, 8), .BackColor = Color.White}
        Dim lblJudul As New Label() With {
            .Text = "PILIH PRINTER", .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 6)}
        Dim lblGaris As New Label() With {
            .Text = "══════════════════════════════",
            .Font = New Font("Courier New", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 8)}
        Dim panel1 As New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False, .AutoSize = True, .BackColor = Color.White, .Margin = New Padding(0, 0, 0, 6)}
        Dim btnThermal As New Button() With {.Text = "Thermal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(30, 80, 160), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnThermal.FlatAppearance.BorderSize = 0
        Dim btnDot As New Button() With {.Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnThermal, btnDot, btnInk})
        Dim panel2 As New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False, .AutoSize = True, .BackColor = Color.White, .Margin = New Padding(0, 0, 0, 6)}
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel2.Controls.AddRange(New Control() {btnPdf, btnBatal})
        Dim lblPetunjuk As New Label() With {
            .Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.Gray, .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0)}
        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris)
        layout.Controls.Add(panel1) : layout.Controls.Add(panel2) : layout.Controls.Add(lblPetunjuk)
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
        If Not String.IsNullOrEmpty(pilihan) Then CetakBayarHutang(noBayar, pilihan)
    End Sub

    ' ============================================================
    ' ENTRY POINT
    ' ============================================================
    Public Sub CetakBayarHutang(noBayar As String)
        CetakBayarHutang(noBayar, "")
    End Sub

    Public Sub CetakBayarHutang(noBayar As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(noBayar) Then Exit Sub
        MuatDataBayarHutang(noBayar)
        Dim cfg As New KonfigurasiThermal("BayarHutang")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride), cfg.JenisPrinter, jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBayarHutang() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakBayarHutang("BayarHutang") : c.CetakThermal()
                    End If
                Case "Printer Dot Matrix"
                    Dim cfgDot As New KonfigurasiDotMatrix("BayarHutang")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBayarHutang() : c.CetakDotMatrix()
                    Else
                        Dim c As New EscPosCetakBayarHutang("BayarHutang") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakBayarHutangInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("BayarHutang")
                    ModuleCetakBayarHutangPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New EscPosCetakBayarHutang("BayarHutang") : c.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
