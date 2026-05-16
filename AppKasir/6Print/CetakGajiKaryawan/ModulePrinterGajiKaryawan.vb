' ================================================================
' ModulePrinterGajiKaryawan — Slip gaji karyawan
' Prefix: "GK_"
' ================================================================
Module ModulePrinterGajiKaryawan

    Public GK_Nomor As String = ""
    Public GK_Tanggal As DateTime
    Public GK_TanggalAwal As DateTime
    Public GK_TanggalAkhir As DateTime
    Public GK_Bulan As String = ""
    Public GK_NamaKaryawan As String = ""
    Public GK_Lokasi As String = ""
    Public GK_IdUser As String = ""
    Public GK_IdKomputer As String = ""

    ' Pendapatan
    Public GK_GajiPokok As Decimal
    Public GK_KomisiJual As Decimal
    Public GK_SupirRp As Decimal
    Public GK_HelperRp As Decimal
    Public GK_LemburRp As Decimal
    Public GK_Tunjangan As Decimal
    Public GK_Transport As Decimal
    Public GK_UangMakan As Decimal
    Public GK_TotalPendapatan As Decimal

    ' Potongan
    Public GK_PotBon As Decimal
    Public GK_Angsuran As Decimal
    Public GK_AbsenRp As Decimal
    Public GK_AbsenKhususRp As Decimal
    Public GK_TerlambatRp As Decimal
    Public GK_PotLain As Decimal
    Public GK_TotalPotongan As Decimal

    Public GK_TotalTerima As Decimal

    Public Sub MuatDataGajiKaryawan(nomor As String)
        GK_Nomor = nomor
        Using cmd As New MySqlCommand(
            "SELECT BULAN, TANGGAL, TANGGALAWAL, TANGGALAKHIR, NAMA, LOKASI, " &
            "POKOK, KOMISI_JUAL, SUPIR_RP, HELPER_RP, LEMBUR_RP, TUNJANGAN, TRANSPORT, UANG_MAKAN, " &
            "POT_BON, ANGSURAN, ABSEN_RP, ABSEN_KHUSUS_RP, TERLAMBAT_RP, POT_LAIN, " &
            "PENDAPATAN, POTONGAN, TERIMA, ID_USER, ID_KOMPUTER " &
            "FROM gaji_karyawan WHERE NOMOR = @nomor", conn)
            cmd.Parameters.AddWithValue("@nomor", nomor)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    GK_Bulan = GKDbStr(rd, "BULAN")
                    GK_Tanggal = If(IsDBNull(rd("TANGGAL")), Date.MinValue, Convert.ToDateTime(rd("TANGGAL")))
                    GK_TanggalAwal = If(IsDBNull(rd("TANGGALAWAL")), Date.MinValue, Convert.ToDateTime(rd("TANGGALAWAL")))
                    GK_TanggalAkhir = If(IsDBNull(rd("TANGGALAKHIR")), Date.MinValue, Convert.ToDateTime(rd("TANGGALAKHIR")))
                    GK_NamaKaryawan = GKDbStr(rd, "NAMA")
                    GK_Lokasi = GKDbStr(rd, "LOKASI")
                    GK_GajiPokok = GKDbDec(rd, "POKOK")
                    GK_KomisiJual = GKDbDec(rd, "KOMISI_JUAL")
                    GK_SupirRp = GKDbDec(rd, "SUPIR_RP")
                    GK_HelperRp = GKDbDec(rd, "HELPER_RP")
                    GK_LemburRp = GKDbDec(rd, "LEMBUR_RP")
                    GK_Tunjangan = GKDbDec(rd, "TUNJANGAN")
                    GK_Transport = GKDbDec(rd, "TRANSPORT")
                    GK_UangMakan = GKDbDec(rd, "UANG_MAKAN")
                    GK_PotBon = GKDbDec(rd, "POT_BON")
                    GK_Angsuran = GKDbDec(rd, "ANGSURAN")
                    GK_AbsenRp = GKDbDec(rd, "ABSEN_RP")
                    GK_AbsenKhususRp = GKDbDec(rd, "ABSEN_KHUSUS_RP")
                    GK_TerlambatRp = GKDbDec(rd, "TERLAMBAT_RP")
                    GK_PotLain = GKDbDec(rd, "POT_LAIN")
                    GK_TotalPendapatan = GKDbDec(rd, "PENDAPATAN")
                    GK_TotalPotongan = GKDbDec(rd, "POTONGAN")
                    GK_TotalTerima = GKDbDec(rd, "TERIMA")
                    GK_IdUser = GKDbStr(rd, "ID_USER")
                    GK_IdKomputer = GKDbStr(rd, "ID_KOMPUTER")
                End If
            End Using
        End Using
    End Sub

    Public Function GKRp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function GKDbStr(rd As MySqlDataReader, k As String) As String
        Return If(IsDBNull(rd(k)), "", rd(k).ToString().Trim())
    End Function
    Friend Function GKDbDec(rd As MySqlDataReader, k As String) As Decimal
        If IsDBNull(rd(k)) Then Return 0
        Dim v As Decimal : Return If(Decimal.TryParse(rd(k).ToString(), v), v, 0)
    End Function

    Public Sub TanyaPilihPrinterGajiKaryawan(nomor As String)
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
        If Not String.IsNullOrEmpty(pilihan) Then CetakGajiKaryawan(nomor, pilihan)
    End Sub

    Public Sub CetakGajiKaryawan(nomor As String)
        CetakGajiKaryawan(nomor, "")
    End Sub

    Public Sub CetakGajiKaryawan(nomor As String, jenis As String)
        If String.IsNullOrEmpty(nomor) Then Exit Sub
        MuatDataGajiKaryawan(nomor)
        Dim cfg As New KonfigurasiThermal("GajiKaryawan")
        Dim j As String = If(String.IsNullOrEmpty(jenis), cfg.JenisPrinter, jenis)
        Try
            Select Case j
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakGajiKaryawan() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakGajiKaryawan("GajiKaryawan") : c.CetakThermal()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakGajiKaryawanInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("GajiKaryawan")
                    ModuleCetakGajiKaryawanPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New EscPosCetakGajiKaryawan("GajiKaryawan") : c.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
