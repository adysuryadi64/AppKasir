' ================================================================
' ModulePrinterBonKaryawan — Slip bon karyawan
' Prefix: "BK_"
' ================================================================
Module ModulePrinterBonKaryawan

    Public BK_Faktur As String = ""
    Public BK_Tanggal As DateTime
    Public BK_NamaKaryawan As String = ""
    Public BK_Jenis As String = ""
    Public BK_Nominal As Decimal
    Public BK_AwalBon As Decimal
    Public BK_AkhirBon As Decimal
    Public BK_Keterangan As String = ""
    Public BK_IdUser As String = ""
    Public BK_IdKomputer As String = ""
    Public BK_Lokasi As String = ""

    Public Sub MuatDataBonKaryawan(faktur As String)
        BK_Faktur = faktur
        Using cmd As New MySqlCommand(
            "SELECT TANGGAL, NAMA, JENIS, NOMINAL, AWAL_BON, AKHIR_BON, " &
            "KETERANGAN, ID_USER, ID_KOMPUTER, LOKASI " &
            "FROM bon_karyawan WHERE FAKTUR = @faktur", conn)
            cmd.Parameters.AddWithValue("@faktur", faktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    BK_Tanggal = If(IsDBNull(rd("TANGGAL")), Date.MinValue, Convert.ToDateTime(rd("TANGGAL")))
                    BK_NamaKaryawan = BKDbStr(rd, "NAMA")
                    BK_Jenis = BKDbStr(rd, "JENIS")
                    BK_Nominal = BKDbDec(rd, "NOMINAL")
                    BK_AwalBon = BKDbDec(rd, "AWAL_BON")
                    BK_AkhirBon = BKDbDec(rd, "AKHIR_BON")
                    BK_Keterangan = BKDbStr(rd, "KETERANGAN")
                    BK_IdUser = BKDbStr(rd, "ID_USER")
                    BK_IdKomputer = BKDbStr(rd, "ID_KOMPUTER")
                    BK_Lokasi = BKDbStr(rd, "LOKASI")
                End If
            End Using
        End Using
    End Sub

    Public Function BKRp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function BKDbStr(rd As MySqlDataReader, k As String) As String
        Return If(IsDBNull(rd(k)), "", rd(k).ToString().Trim())
    End Function
    Friend Function BKDbDec(rd As MySqlDataReader, k As String) As Decimal
        If IsDBNull(rd(k)) Then Return 0
        Dim v As Decimal : Return If(Decimal.TryParse(rd(k).ToString(), v), v, 0)
    End Function

    Public Sub TanyaPilihPrinterBonKaryawan(faktur As String)
        Dim pilihan As String = ""
        Dim frm As New Form() With {.Text = "", .Size = New Size(440, 220), .StartPosition = FormStartPosition.CenterScreen, .ControlBox = False, .FormBorderStyle = FormBorderStyle.FixedDialog, .BackColor = Color.White, .TopMost = True, .KeyPreview = True}
        Dim layout As New TableLayoutPanel() With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 4, .Padding = New Padding(16, 12, 16, 8), .BackColor = Color.White}
        Dim lblJudul As New Label() With {.Text = "PILIH PRINTER", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 6)}
        Dim lblGaris As New Label() With {.Text = "══════════════════════════════", .Font = New Font("Courier New", 9, FontStyle.Bold), .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 8)}
        Dim panel1 As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False, .AutoSize = True, .BackColor = Color.White, .Margin = New Padding(0, 0, 0, 6)}
        Dim btnThermal As New Button() With {.Text = "Thermal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(30, 80, 160), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnThermal.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnThermal, btnInk, btnPdf, btnBatal})
        Dim lblPetunjuk As New Label() With {.Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8), .ForeColor = Color.Gray, .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0)}
        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris) : layout.Controls.Add(panel1) : layout.Controls.Add(lblPetunjuk)
        frm.Controls.Add(layout)
        AddHandler btnThermal.Click, Sub(s, ev)
                pilihan = "Printer Thermal"
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
        If Not String.IsNullOrEmpty(pilihan) Then CetakBonKaryawan(faktur, pilihan)
    End Sub

    Public Sub CetakBonKaryawan(faktur As String)
        CetakBonKaryawan(faktur, "")
    End Sub

    Public Sub CetakBonKaryawan(faktur As String, jenis As String)
        If String.IsNullOrEmpty(faktur) Then Exit Sub
        MuatDataBonKaryawan(faktur)
        Dim cfg As New KonfigurasiThermal("BonKaryawan")
        Dim j As String = If(String.IsNullOrEmpty(jenis), cfg.JenisPrinter, jenis)
        Try
            Select Case j
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBonKaryawan() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakBonKaryawan("BonKaryawan") : c.CetakThermal()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakBonKaryawanInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("BonKaryawan")
                    ModuleCetakBonKaryawanPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New EscPosCetakBonKaryawan("BonKaryawan") : c.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
