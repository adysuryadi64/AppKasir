' ================================================================
' ModulePrinterReturBeli
' Pusat data dan entry point untuk cetak nota retur pembelian.
' Prefix variabel: "ReturBeli_"
'
' Cara pakai dari FormReturPembelian:
'   ModulePrinterReturBeli.CetakReturBeli(TxtFaktur.Text)
'   ModulePrinterReturBeli.TanyaPilihPrinterReturBeli(TxtFaktur.Text)
' ================================================================
Module ModulePrinterReturBeli

    ' ============================================================
    ' DATA HEADER
    ' ============================================================
    Public ReturBeli_NoRetur As String = ""
    Public ReturBeli_Tanggal As DateTime
    Public ReturBeli_NamaSupplier As String = ""
    Public ReturBeli_AlamatSupplier As String = ""
    Public ReturBeli_Total As Decimal
    Public ReturBeli_IdUser As String = ""
    Public ReturBeli_IdKomputer As String = ""

    ' ── Data item ────────────────────────────────────────────
    Public ReturBeli_DaftarItem As New List(Of ItemNotaReturBeli)

    Public Class ItemNotaReturBeli
        Public NamaBarang As String = ""
        Public Qty As Decimal
        Public Satuan As String = ""
        Public Harga As Decimal
        Public Total As Decimal
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' ============================================================
    Public Sub MuatDataReturBeli(noRetur As String)
        ReturBeli_NoRetur = noRetur
        MuatItemReturBeli(noRetur)
        MuatHeaderReturBeli(noRetur)
    End Sub

    Private Sub MuatItemReturBeli(noRetur As String)
        ReturBeli_DaftarItem.Clear()
        Using cmd As New MySqlCommand(
            "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_BELI_SATUAN, TOTAL " &
            "FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @noRetur " &
            "ORDER BY NAMA_BARANG", conn)
            cmd.Parameters.AddWithValue("@noRetur", noRetur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    ReturBeli_DaftarItem.Add(New ItemNotaReturBeli With {
                        .NamaBarang = RBDbStr(rd, "NAMA_BARANG"),
                        .Qty = RBDbDec(rd, "QTY"),
                        .Satuan = RBDbStr(rd, "SATUAN"),
                        .Harga = RBDbDec(rd, "HARGA_BELI_SATUAN"),
                        .Total = RBDbDec(rd, "TOTAL")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderReturBeli(noRetur As String)
        Using cmd As New MySqlCommand(
            "SELECT TGL_RETUR_BELI, NAMA_SUPPLIER, ALAMAT_SUPPLIER, " &
            "TOTAL_RUPIAH, ID_USER, ID_KOMPUTER " &
            "FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @noRetur", conn)
            cmd.Parameters.AddWithValue("@noRetur", noRetur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ReturBeli_Tanggal = Convert.ToDateTime(rd("TGL_RETUR_BELI"))
                    ReturBeli_NamaSupplier = RBDbStr(rd, "NAMA_SUPPLIER")
                    ReturBeli_AlamatSupplier = RBDbStr(rd, "ALAMAT_SUPPLIER")
                    ReturBeli_Total = RBDbDec(rd, "TOTAL_RUPIAH")
                    ReturBeli_IdUser = RBDbStr(rd, "ID_USER")
                    ReturBeli_IdKomputer = RBDbStr(rd, "ID_KOMPUTER")
                End If
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function ReturBeliRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function

    Friend Function RBDbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function RBDbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterReturBeli(noRetur As String)
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
        Dim btnThermal As New Button() With {.Text = "Thermal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(30, 80, 160), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnThermal.FlatAppearance.BorderSize = 0
        Dim btnDot As New Button() With {.Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnThermal, btnDot, btnInk})

        Dim panel2 As New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False, .AutoSize = True, .BackColor = Color.White,
            .Margin = New Padding(0, 0, 0, 6)}
        Dim btnMonitor As New Button() With {.Text = "Monitor", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(80, 40, 140), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnMonitor.FlatAppearance.BorderSize = 0
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel2.Controls.AddRange(New Control() {btnMonitor, btnPdf, btnBatal})

        Dim lblPetunjuk As New Label() With {
            .Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.Gray, .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill,
            .Margin = New Padding(0)}

        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris)
        layout.Controls.Add(panel1) : layout.Controls.Add(panel2)
        layout.Controls.Add(lblPetunjuk)
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
        AddHandler btnMonitor.Click, Sub(s, ev)
                pilihan = "Tampilkan di Monitor"
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

        If Not String.IsNullOrEmpty(pilihan) Then CetakReturBeli(noRetur, pilihan)
    End Sub

    ' ============================================================
    ' ENTRY POINT
    ' ============================================================
    Public Sub CetakReturBeli(noRetur As String)
        CetakReturBeli(noRetur, "")
    End Sub

    Public Sub CetakReturBeli(noRetur As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(noRetur) Then Exit Sub
        MuatDataReturBeli(noRetur)
        Dim cfg As New KonfigurasiThermal("ReturBeli")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride), cfg.JenisPrinter, jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakReturBeliThermalMatrik() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakReturBeliThermalMatrik("ReturBeli") : c.CetakThermal()
                    End If
                Case "Printer Dot Matrix"
                    Dim cfgDot As New KonfigurasiDotMatrix("ReturBeli")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakReturBeliThermalMatrik() : c.CetakDotMatrix()
                    Else
                        Dim c As New EscPosCetakReturBeliThermalMatrik("ReturBeli") : c.CetakDotMatrix()
                    End If
                Case "Tampilkan di Monitor"
                    Dim cfgM As New KonfigurasiMonitor("ReturBeli")
                    GdiCetakReturBeliThermalMatrik.TampilkanPreviewStatic(cfgM.TampilFooter1, cfgM.TampilFooter2, cfgM.TampilFooter3)
                Case "Printer Inkjet / Laser"
                    ModuleCetakReturBeliInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("ReturBeli")
                    ModuleCetakReturBeliPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New EscPosCetakReturBeliThermalMatrik("ReturBeli") : c.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

    Public Sub PreviewReturBeli(noRetur As String)
        If String.IsNullOrEmpty(noRetur) Then Exit Sub
        MuatDataReturBeli(noRetur)
        Dim c As New GdiCetakReturBeliThermalMatrik() : c.TampilkanPreview()
    End Sub

End Module
