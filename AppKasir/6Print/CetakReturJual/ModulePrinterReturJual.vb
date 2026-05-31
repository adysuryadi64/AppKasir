' ================================================================
' ModulePrinterReturJual
' Pusat data dan entry point untuk cetak nota retur penjualan.
'
' Semua variabel data transaksi disimpan di sini dengan prefix
' "ReturJual_" agar tidak bentrok dengan modul cetak lain.
'
' Alur kerja:
'   1. MuatDataReturJual(noRetur) — query DB, isi semua ReturJual_*
'   2. CetakReturJual(noRetur)    — muat data lalu pilih class cetak
'
' Cara pakai dari FormReturPenjualan:
'   ModulePrinterReturJual.CetakReturJual(TxtFaktur.Text)
' ================================================================
Module ModulePrinterReturJual

    ' ============================================================
    ' DATA HEADER NOTA RETUR PENJUALAN
    ' ============================================================
    Public ReturJual_NoRetur As String = ""
    Public ReturJual_Tanggal As DateTime
    Public ReturJual_NamaPelanggan As String = ""
    Public ReturJual_JenisPelanggan As String = ""
    Public ReturJual_Total As Decimal
    Public ReturJual_IdUser As String = ""
    Public ReturJual_IdKomputer As String = ""

    ' ── Data item barang ─────────────────────────────────────
    Public ReturJual_DaftarItem As New List(Of ItemNotaReturJual)

    Public Class ItemNotaReturJual
        Public NamaBarang As String = ""
        Public Qty As Decimal
        Public Satuan As String = ""
        Public Harga As Decimal
        Public TotalDiskon As Decimal
        Public TotalHarga As Decimal
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' ============================================================
    Public Sub MuatDataReturJual(noRetur As String)
        ReturJual_NoRetur = noRetur
        MuatItemReturJual(noRetur)
        MuatHeaderReturJual(noRetur)
    End Sub

    Private Sub MuatItemReturJual(noRetur As String)
        ReturJual_DaftarItem.Clear()
        Using cmd As New MySqlCommand(
            "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_BELI_SATUAN, TOTAL_DISKON, TOTAL_HARGA " &
            "FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @noRetur " &
            "ORDER BY URUTAN", conn)
            cmd.Parameters.AddWithValue("@noRetur", noRetur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    ReturJual_DaftarItem.Add(New ItemNotaReturJual With {
                        .NamaBarang = RJDbStr(rd, "NAMA_BARANG"),
                        .Qty = RJDbDec(rd, "QTY"),
                        .Satuan = RJDbStr(rd, "SATUAN"),
                        .Harga = RJDbDec(rd, "HARGA_BELI_SATUAN"),
                        .TotalDiskon = RJDbDec(rd, "TOTAL_DISKON"),
                        .TotalHarga = RJDbDec(rd, "TOTAL_HARGA")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderReturJual(noRetur As String)
        Using cmd As New MySqlCommand(
            "SELECT TGL_RETUR_JUAL, NAMA_PELANGGAN, JENIS_PELANGGAN, " &
            "TOTAL_RUPIAH, ID_USER, ID_KOMPUTER " &
            "FROM retur_penjualan WHERE ID_RETUR_PENJUALAN = @noRetur", conn)
            cmd.Parameters.AddWithValue("@noRetur", noRetur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ReturJual_Tanggal = Convert.ToDateTime(rd("TGL_RETUR_JUAL"))
                    ReturJual_NamaPelanggan = RJDbStr(rd, "NAMA_PELANGGAN")
                    ReturJual_JenisPelanggan = RJDbStr(rd, "JENIS_PELANGGAN")
                    ReturJual_Total = RJDbDec(rd, "TOTAL_RUPIAH")
                    ReturJual_IdUser = RJDbStr(rd, "ID_USER")
                    ReturJual_IdKomputer = RJDbStr(rd, "ID_KOMPUTER")
                End If
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function ReturJualRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function

    Friend Function RJDbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function RJDbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterReturJual(noRetur As String)
        Dim pilihan As String = ""

        Dim frm As New Form()
        With frm
            .Text = ""
            .Size = New Size(440, 260)
            .StartPosition = FormStartPosition.CenterScreen
            .ControlBox = False
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .BackColor = Color.White
            .TopMost = True
            .KeyPreview = True
        End With

        Dim layout As New TableLayoutPanel()
        With layout
            .Dock = DockStyle.Fill
            .ColumnCount = 1
            .RowCount = 5
            .Padding = New Padding(16, 12, 16, 8)
            .BackColor = Color.White
        End With

        Dim lblJudul As New Label()
        With lblJudul
            .Text = "PILIH PRINTER"
            .Font = New Font("Segoe UI", 16, FontStyle.Bold)
            .ForeColor = Color.FromArgb(30, 80, 160)
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
            .Margin = New Padding(0, 0, 0, 6)
        End With

        Dim lblGaris As New Label()
        With lblGaris
            .Text = "══════════════════════════════"
            .Font = New Font("Courier New", 9, FontStyle.Bold)
            .ForeColor = Color.FromArgb(30, 80, 160)
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
            .Margin = New Padding(0, 0, 0, 8)
        End With

        Dim panel1 As New FlowLayoutPanel()
        With panel1
            .Dock = DockStyle.Fill
            .FlowDirection = FlowDirection.LeftToRight
            .WrapContents = False
            .AutoSize = True
            .BackColor = Color.White
            .Margin = New Padding(0, 0, 0, 6)
        End With
        Dim btnThermal As New Button() With {
            .Text = "Thermal", .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(110, 42), .BackColor = Color.FromArgb(30, 80, 160),
            .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat,
            .Margin = New Padding(0, 0, 8, 0)}
        btnThermal.FlatAppearance.BorderSize = 0
        Dim btnDot As New Button() With {
            .Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60),
            .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat,
            .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {
            .Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0),
            .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat,
            .Margin = New Padding(0, 0, 0, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnThermal, btnDot, btnInk})

        Dim panel2 As New FlowLayoutPanel()
        With panel2
            .Dock = DockStyle.Fill
            .FlowDirection = FlowDirection.LeftToRight
            .WrapContents = False
            .AutoSize = True
            .BackColor = Color.White
            .Margin = New Padding(0, 0, 0, 6)
        End With
        Dim btnMonitor As New Button() With {
            .Text = "Monitor", .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(110, 42), .BackColor = Color.FromArgb(80, 40, 140),
            .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat,
            .Margin = New Padding(0, 0, 8, 0)}
        btnMonitor.FlatAppearance.BorderSize = 0
        Dim btnPdf As New Button() With {
            .Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30),
            .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat,
            .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {
            .Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200),
            .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat,
            .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel2.Controls.AddRange(New Control() {btnMonitor, btnPdf, btnBatal})

        Dim lblPetunjuk As New Label()
        With lblPetunjuk
            .Text = "Tekan ESC untuk batal"
            .Font = New Font("Segoe UI", 8)
            .ForeColor = Color.Gray
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
            .Margin = New Padding(0)
        End With

        layout.Controls.Add(lblJudul)
        layout.Controls.Add(lblGaris)
        layout.Controls.Add(panel1)
        layout.Controls.Add(panel2)
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

        If Not String.IsNullOrEmpty(pilihan) Then
            CetakReturJual(noRetur, pilihan)
        End If
    End Sub

    ' ============================================================
    ' ENTRY POINT CETAK RETUR JUAL
    ' ============================================================
    Public Sub CetakReturJual(noRetur As String)
        CetakReturJual(noRetur, "")
    End Sub

    Public Sub CetakReturJual(noRetur As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(noRetur) Then Exit Sub

        MuatDataReturJual(noRetur)

        Dim cfg As New KonfigurasiThermal("ReturJual")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride),
                                 cfg.JenisPrinter,
                                 jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim cetak As New GdiCetakReturJualThermalMatrik()
                        cetak.Cetak()
                    Else
                        Dim cetak As New EscPosCetakReturJualThermalMatrik("ReturJual")
                        cetak.CetakThermal()
                    End If

                Case "Printer Dot Matrix"
                    Dim cfgDot As New KonfigurasiDotMatrix("ReturJual")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim cetakGdi As New GdiCetakReturJualThermalMatrik()
                        cetakGdi.CetakDotMatrix()
                    Else
                        Dim cetakEsc As New EscPosCetakReturJualThermalMatrik("ReturJual")
                        cetakEsc.CetakDotMatrix()
                    End If

                Case "Tampilkan di Monitor"
                    Dim cfgM As New KonfigurasiMonitor("ReturJual")
                    GdiCetakReturJualThermalMatrik.TampilkanPreviewStatic(
                        cfgM.TampilFooter1, cfgM.TampilFooter2, cfgM.TampilFooter3)

                Case "Printer Inkjet / Laser"
                    ModuleCetakReturJualInkjet.CetakNota()

                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("ReturJual")
                    ModuleCetakReturJualPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)

                Case Else
                    Dim cetak As New EscPosCetakReturJualThermalMatrik("ReturJual")
                    cetak.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

    Public Sub PreviewReturJual(noRetur As String)
        If String.IsNullOrEmpty(noRetur) Then Exit Sub
        MuatDataReturJual(noRetur)
        Dim cetak As New GdiCetakReturJualThermalMatrik()
        cetak.TampilkanPreview()
    End Sub

End Module
