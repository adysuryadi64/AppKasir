' ================================================================
' ModulePrinterTransferBarang
' Pusat data dan entry point untuk cetak nota transfer barang.
' Prefix variabel: "TB_"
'
' Cara pakai:
'   ModulePrinterTransferBarang.CetakTransferBarang(TxtNota.Text)
'   ModulePrinterTransferBarang.TanyaPilihPrinterTransferBarang(TxtNota.Text)
' ================================================================
Module ModulePrinterTransferBarang

    ' ============================================================
    ' DATA HEADER
    ' ============================================================
    Public TB_IdTransfer As String = ""
    Public TB_Tanggal As DateTime
    Public TB_Lokasi As String = ""
    Public TB_KeteranganLokasi As String = ""
    Public TB_TotalRupiah As Decimal
    Public TB_IdUser As String = ""

    ' ── Data item ────────────────────────────────────────────
    Public TB_DaftarItem As New List(Of ItemTransferBarang)

    Public Class ItemTransferBarang
        Public IdBarang As String = ""
        Public NamaBarang As String = ""
        Public Harga As Decimal
        Public Qty As Decimal
        Public Satuan As String = ""
        Public Total As Decimal
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' ============================================================
    Public Sub MuatDataTransferBarang(idTransfer As String)
        TB_IdTransfer = idTransfer
        MuatItemTransferBarang(idTransfer)
        MuatHeaderTransferBarang(idTransfer)
    End Sub

    Private Sub MuatItemTransferBarang(idTransfer As String)
        TB_DaftarItem.Clear()
        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, HARGA, QTY, SATUAN, TOTAL " &
            "FROM transfer_barang_detail WHERE ID_TRANSFER = @id " &
            "ORDER BY URUTAN", conn)
            cmd.Parameters.AddWithValue("@id", idTransfer)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    TB_DaftarItem.Add(New ItemTransferBarang With {
                        .IdBarang = TBDbStr(rd, "ID_BARANG"),
                        .NamaBarang = TBDbStr(rd, "NAMA_BARANG"),
                        .Harga = TBDbDec(rd, "HARGA"),
                        .Qty = TBDbDec(rd, "QTY"),
                        .Satuan = TBDbStr(rd, "SATUAN"),
                        .Total = TBDbDec(rd, "TOTAL")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderTransferBarang(idTransfer As String)
        Using cmd As New MySqlCommand(
            "SELECT TGL_TRANSFER, LOKASI, TOTAL_RUPIAH, ID_USER " &
            "FROM transfer_barang WHERE ID_TRANSFER = @id", conn)
            cmd.Parameters.AddWithValue("@id", idTransfer)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TB_Tanggal = If(IsDBNull(rd("TGL_TRANSFER")), Date.MinValue, Convert.ToDateTime(rd("TGL_TRANSFER")))
                    TB_Lokasi = TBDbStr(rd, "LOKASI")
                    TB_KeteranganLokasi = If(TB_Lokasi = "TOKO",
                        "TRANSFER BARANG DARI TOKO KE GUDANG",
                        "TRANSFER BARANG DARI GUDANG KE TOKO")
                    TB_TotalRupiah = TBDbDec(rd, "TOTAL_RUPIAH")
                    TB_IdUser = TBDbStr(rd, "ID_USER")
                End If
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function TBRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function

    Friend Function TBDbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function TBDbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterTransferBarang(idTransfer As String)
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
        Dim btnThermal As New Button() With {.Text = "Thermal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(30, 80, 160), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnThermal.FlatAppearance.BorderSize = 0
        Dim btnDot As New Button() With {.Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnThermal, btnDot, btnInk, btnPdf, btnBatal})

        Dim lblPetunjuk As New Label() With {
            .Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8),
            .ForeColor = Color.Gray, .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill,
            .Margin = New Padding(0)}

        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris)
        layout.Controls.Add(panel1) : layout.Controls.Add(lblPetunjuk)
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

        If Not String.IsNullOrEmpty(pilihan) Then CetakTransferBarang(idTransfer, pilihan)
    End Sub

    ' ============================================================
    ' ENTRY POINT
    ' ============================================================
    Public Sub CetakTransferBarang(idTransfer As String)
        CetakTransferBarang(idTransfer, "")
    End Sub

    Public Sub CetakTransferBarang(idTransfer As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(idTransfer) Then Exit Sub
        MuatDataTransferBarang(idTransfer)
        Dim cfgDot As New KonfigurasiDotMatrix("TransferBarang")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride), BacaPengaturanPrinter("TransferBarang", "JenisPrinter", "Printer Dot Matrix"), jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Thermal"
                    Dim cfgT As New KonfigurasiThermal("TransferBarang")
                    If cfgT.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakTransferBarang() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakTransferBarang("TransferBarang") : c.CetakDotMatrix()
                    End If
                Case "Printer Dot Matrix"
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakTransferBarang() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakTransferBarang("TransferBarang") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakTransferBarangInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("TransferBarang")
                    ModuleCetakTransferBarangPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New GdiCetakTransferBarang() : c.Cetak()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
