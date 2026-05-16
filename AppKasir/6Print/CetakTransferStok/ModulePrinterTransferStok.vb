' ================================================================
' ModulePrinterTransferStok — Bukti transfer stok antar lokasi
' Prefix: "TS_"
' ================================================================
Module ModulePrinterTransferStok

    Public TS_IdTransfer As String = ""
    Public TS_Tanggal As DateTime
    Public TS_JenisTransfer As String = ""
    Public TS_Uraian As String = ""
    Public TS_IdUser As String = ""
    Public TS_IdKomputer As String = ""

    Public TS_DaftarItem As New List(Of ItemTransferStok)

    Public Class ItemTransferStok
        Public NamaBarangMasuk As String = ""
        Public QtyMasuk As Decimal
        Public SatuanMasuk As String = ""
        Public HargaMasuk As Decimal
        Public TotalMasuk As Decimal
        Public NamaBarangKeluar As String = ""
        Public QtyKeluar As Decimal
        Public SatuanKeluar As String = ""
        Public HargaKeluar As Decimal
        Public TotalKeluar As Decimal
        Public Selisih As Decimal
    End Class

    Public Sub MuatDataTransferStok(idTransfer As String)
        TS_IdTransfer = idTransfer
        TS_DaftarItem.Clear()
        Using cmd As New MySqlCommand(
            "SELECT JENIS_TRANSFER, URAIAN, TANGGAL, " &
            "NAMA_BARANG_M, QTY_M, SATUAN_M, HARGA_SAT_M, TOTAL_HARGA_M, " &
            "NAMA_BARANG_K, QTY_K, SATUAN_K, HARGA_SAT_K, TOTAL_HARGA_K, Selisih, " &
            "ID_USER, ID_KOMPUTER " &
            "FROM transfer_stok WHERE ID_TRANSFER = @id ORDER BY TANGGAL", conn)
            cmd.Parameters.AddWithValue("@id", idTransfer)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Dim firstRow As Boolean = True
                While rd.Read()
                    If firstRow Then
                        TS_JenisTransfer = TSDbStr(rd, "JENIS_TRANSFER")
                        TS_Uraian = TSDbStr(rd, "URAIAN")
                        TS_Tanggal = If(IsDBNull(rd("TANGGAL")), Date.MinValue, Convert.ToDateTime(rd("TANGGAL")))
                        TS_IdUser = TSDbStr(rd, "ID_USER")
                        TS_IdKomputer = TSDbStr(rd, "ID_KOMPUTER")
                        firstRow = False
                    End If
                    TS_DaftarItem.Add(New ItemTransferStok With {
                        .NamaBarangMasuk = TSDbStr(rd, "NAMA_BARANG_M"),
                        .QtyMasuk = TSDbDec(rd, "QTY_M"),
                        .SatuanMasuk = TSDbStr(rd, "SATUAN_M"),
                        .HargaMasuk = TSDbDec(rd, "HARGA_SAT_M"),
                        .TotalMasuk = TSDbDec(rd, "TOTAL_HARGA_M"),
                        .NamaBarangKeluar = TSDbStr(rd, "NAMA_BARANG_K"),
                        .QtyKeluar = TSDbDec(rd, "QTY_K"),
                        .SatuanKeluar = TSDbStr(rd, "SATUAN_K"),
                        .HargaKeluar = TSDbDec(rd, "HARGA_SAT_K"),
                        .TotalKeluar = TSDbDec(rd, "TOTAL_HARGA_K"),
                        .Selisih = TSDbDec(rd, "Selisih")
                    })
                End While
            End Using
        End Using
    End Sub

    Public Function TSRp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function TSDbStr(rd As MySqlDataReader, k As String) As String
        Return If(IsDBNull(rd(k)), "", rd(k).ToString().Trim())
    End Function
    Friend Function TSDbDec(rd As MySqlDataReader, k As String) As Decimal
        If IsDBNull(rd(k)) Then Return 0
        Dim v As Decimal : Return If(Decimal.TryParse(rd(k).ToString(), v), v, 0)
    End Function

    Public Sub TanyaPilihPrinterTransferStok(idTransfer As String)
        Dim pilihan As String = ""
        Dim frm As New Form() With {.Text = "", .Size = New Size(440, 220), .StartPosition = FormStartPosition.CenterScreen, .ControlBox = False, .FormBorderStyle = FormBorderStyle.FixedDialog, .BackColor = Color.White, .TopMost = True, .KeyPreview = True}
        Dim layout As New TableLayoutPanel() With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 4, .Padding = New Padding(16, 12, 16, 8), .BackColor = Color.White}
        Dim lblJudul As New Label() With {.Text = "PILIH PRINTER", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 6)}
        Dim lblGaris As New Label() With {.Text = "══════════════════════════════", .Font = New Font("Courier New", 9, FontStyle.Bold), .ForeColor = Color.FromArgb(30, 80, 160), .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 0, 8)}
        Dim panel1 As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False, .AutoSize = True, .BackColor = Color.White, .Margin = New Padding(0, 0, 0, 6)}
        Dim btnDot As New Button() With {.Text = "Dot Matrix", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(0, 120, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnDot.FlatAppearance.BorderSize = 0
        Dim btnInk As New Button() With {.Text = "Inkjet / Laser", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(140, 60, 0), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnInk.FlatAppearance.BorderSize = 0
        Dim btnPdf As New Button() With {.Text = "Export PDF", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(180, 30, 30), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0)}
        btnPdf.FlatAppearance.BorderSize = 0
        Dim btnBatal As New Button() With {.Text = "Batal", .Font = New Font("Segoe UI", 10, FontStyle.Bold), .Size = New Size(110, 42), .BackColor = Color.FromArgb(200, 200, 200), .ForeColor = Color.FromArgb(60, 60, 60), .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 0, 0)}
        btnBatal.FlatAppearance.BorderSize = 0
        panel1.Controls.AddRange(New Control() {btnDot, btnInk, btnPdf, btnBatal})
        Dim lblPetunjuk As New Label() With {.Text = "Tekan ESC untuk batal", .Font = New Font("Segoe UI", 8), .ForeColor = Color.Gray, .AutoSize = True, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Fill, .Margin = New Padding(0)}
        layout.Controls.Add(lblJudul) : layout.Controls.Add(lblGaris) : layout.Controls.Add(panel1) : layout.Controls.Add(lblPetunjuk)
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
        If Not String.IsNullOrEmpty(pilihan) Then CetakTransferStok(idTransfer, pilihan)
    End Sub

    Public Sub CetakTransferStok(idTransfer As String)
        CetakTransferStok(idTransfer, "")
    End Sub

    Public Sub CetakTransferStok(idTransfer As String, jenis As String)
        If String.IsNullOrEmpty(idTransfer) Then Exit Sub
        MuatDataTransferStok(idTransfer)
        Dim cfgDot As New KonfigurasiDotMatrix("TransferStok")
        Dim j As String = If(String.IsNullOrEmpty(jenis), BacaPengaturanPrinter("TransferStok", "JenisPrinter", "Printer Dot Matrix"), jenis)
        Try
            Select Case j
                Case "Printer Dot Matrix"
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakTransferStok() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakTransferStok("TransferStok") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakTransferStokInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("TransferStok")
                    ModuleCetakTransferStokPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New GdiCetakTransferStok() : c.Cetak()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
