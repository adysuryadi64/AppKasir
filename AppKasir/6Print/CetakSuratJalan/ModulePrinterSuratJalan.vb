' ================================================================
' ModulePrinterSuratJalan
' Pusat data dan entry point untuk cetak surat jalan.
' Prefix variabel: "SJ_"
'
' Cara pakai:
'   ModulePrinterSuratJalan.CetakSuratJalan(TxtNota.Text)
'   ModulePrinterSuratJalan.TanyaPilihPrinterSuratJalan(TxtNota.Text)
' ================================================================
Module ModulePrinterSuratJalan

    ' ============================================================
    ' DATA HEADER
    ' ============================================================
    Public SJ_Nota As String = ""
    Public SJ_Tanggal As DateTime
    Public SJ_TotalRupiah As Decimal
    Public SJ_Armada As String = ""
    Public SJ_JenisArmada As String = ""
    Public SJ_Supir As String = ""
    Public SJ_Helper1 As String = ""
    Public SJ_Helper2 As String = ""
    Public SJ_IdUser As String = ""

    ' ── Data detail pengiriman ───────────────────────────────
    Public SJ_DaftarDetail As New List(Of ItemSuratJalan)

    Public Class ItemSuratJalan
        Public NotaBelanja As String = ""
        Public NamaPelanggan As String = ""
        Public AlamatPelanggan As String = ""
        Public NilaiBelanja As Decimal
        Public Lokasi As String = ""
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' ============================================================
    Public Sub MuatDataSuratJalan(nota As String)
        SJ_Nota = nota
        MuatDetailSuratJalan(nota)
        MuatHeaderSuratJalan(nota)
    End Sub

    Private Sub MuatDetailSuratJalan(nota As String)
        SJ_DaftarDetail.Clear()
        Using cmd As New MySqlCommand(
            "SELECT NOTA_BELANJA, NAMA_PELANGGAN, ALAMAT_PELANGGAN, NILAI_BELANJA, LOKASI " &
            "FROM surat_jalan_detail WHERE NOTA = @nota ORDER BY NOTA_BELANJA", conn)
            cmd.Parameters.AddWithValue("@nota", nota)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    SJ_DaftarDetail.Add(New ItemSuratJalan With {
                        .NotaBelanja = SJDbStr(rd, "NOTA_BELANJA"),
                        .NamaPelanggan = SJDbStr(rd, "NAMA_PELANGGAN"),
                        .AlamatPelanggan = SJDbStr(rd, "ALAMAT_PELANGGAN"),
                        .NilaiBelanja = SJDbDec(rd, "NILAI_BELANJA"),
                        .Lokasi = SJDbStr(rd, "LOKASI")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderSuratJalan(nota As String)
        Using cmd As New MySqlCommand(
            "SELECT TGL_PENGIRIMAN, TOTAL_RUPIAH, ARMADA, JENIS_ARMADA, " &
            "SUPIR, HELPER1, HELPER2, ID_USER " &
            "FROM surat_jalan WHERE NOTA = @nota", conn)
            cmd.Parameters.AddWithValue("@nota", nota)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    SJ_Tanggal = If(IsDBNull(rd("TGL_PENGIRIMAN")), Date.MinValue, Convert.ToDateTime(rd("TGL_PENGIRIMAN")))
                    SJ_TotalRupiah = SJDbDec(rd, "TOTAL_RUPIAH")
                    SJ_Armada = SJDbStr(rd, "ARMADA")
                    SJ_JenisArmada = SJDbStr(rd, "JENIS_ARMADA")
                    SJ_Supir = SJDbStr(rd, "SUPIR")
                    SJ_Helper1 = SJDbStr(rd, "HELPER1")
                    SJ_Helper2 = SJDbStr(rd, "HELPER2")
                    SJ_IdUser = SJDbStr(rd, "ID_USER")
                End If
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function SJRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function

    Friend Function SJDbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function SJDbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterSuratJalan(nota As String)
        Dim pilihan As String = ""
        Dim frm As New Form() With {
            .Text = "", .Size = New Size(440, 220),
            .StartPosition = FormStartPosition.CenterScreen,
            .ControlBox = False, .FormBorderStyle = FormBorderStyle.FixedDialog,
            .BackColor = Color.White, .TopMost = True, .KeyPreview = True}

        Dim layout As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 4,
            .Padding = New Padding(16, 12, 16, 8), .BackColor = Color.White}

        Dim lblJudul As New Label() With {
            .Text = "PILIH PRINTER", .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill,
            .Margin = New Padding(0, 0, 0, 6)}
        Dim lblGaris As New Label() With {
            .Text = "══════════════════════════════",
            .Font = New Font("Courier New", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill,
            .Margin = New Padding(0, 0, 0, 8)}

        Dim panel1 As New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False, .AutoSize = True, .BackColor = Color.White,
            .Margin = New Padding(0, 0, 0, 6)}
        Dim btnDot As New Button() With {.Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnDot, btnInk, btnPdf, btnBatal})

        Dim lblPetunjuk As New Label() With {
            .Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.Gray, .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill,
            .Margin = New Padding(0)}

        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris)
        layout.Controls.Add(panel1) : layout.Controls.Add(lblPetunjuk)
        frm.Controls.Add(layout)

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

        If Not String.IsNullOrEmpty(pilihan) Then CetakSuratJalan(nota, pilihan)
    End Sub

    ' ============================================================
    ' ENTRY POINT
    ' ============================================================
    Public Sub CetakSuratJalan(nota As String)
        CetakSuratJalan(nota, "")
    End Sub

    Public Sub CetakSuratJalan(nota As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(nota) Then Exit Sub
        MuatDataSuratJalan(nota)
        Dim cfgDot As New KonfigurasiDotMatrix("SuratJalan")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride), BacaPengaturanPrinter("SuratJalan", "JenisPrinter", "Printer Dot Matrix"), jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Dot Matrix"
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakSuratJalan() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakSuratJalan("SuratJalan") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakSuratJalanInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("SuratJalan")
                    ModuleCetakSuratJalanPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New GdiCetakSuratJalan() : c.Cetak()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
