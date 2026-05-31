' ================================================================
' ModulePrinterTransferCabang
' Entry point cetak nota transfer barang antar cabang.
' Prefix variabel: "TC_"
'
' Cara pakai:
'   ModulePrinterTransferCabang.CetakTransferCabang(idTransfer)
' ================================================================
Module ModulePrinterTransferCabang

    ' ── Header ──────────────────────────────────────────────────
    Public TC_IdTransfer As String = ""
    Public TC_Tanggal As DateTime
    Public TC_DariCabang As String = ""
    Public TC_KeCabang As String = ""
    Public TC_ModeKirim As String = ""
    Public TC_Keterangan As String = ""
    Public TC_TotalQty As Decimal
    Public TC_TotalRupiah As Decimal
    Public TC_IdUser As String = ""
    Public TC_StatusTransfer As String = ""

    ' ── Item ────────────────────────────────────────────────────
    Public TC_DaftarItem As New List(Of ItemTransferCabang)

    Public Class ItemTransferCabang
        Public IdBarang As String = ""
        Public NamaBarang As String = ""
        Public Harga As Decimal
        Public Qty As Decimal
        Public Satuan As String = ""
        Public IsiSatuan As Integer = 1
        Public QtySatuan As Decimal
        Public Total As Decimal
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE LOKAL
    ' ============================================================
    Public Sub MuatDataTransferCabang(idTransfer As String)
        TC_IdTransfer = idTransfer
        TC_DaftarItem.Clear()

        ' Header
        Using cmd As New MySqlCommand(
            "SELECT TGL_TRANSFER, DARI_CABANG, KE_CABANG, MODE_KIRIM, TOTAL_QTY,
                    TOTAL_RUPIAH, ID_USER, STATUS_TRANSFER,
                    COALESCE(FILE_MANUAL,'') AS KETERANGAN
             FROM transfer_cabang WHERE ID_TRANSFER = @id LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@id", idTransfer)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TC_Tanggal = If(IsDBNull(rd("TGL_TRANSFER")), Date.Now, Convert.ToDateTime(rd("TGL_TRANSFER")))
                    TC_DariCabang = TCStr(rd, "DARI_CABANG")
                    TC_KeCabang = TCStr(rd, "KE_CABANG")
                    TC_ModeKirim = TCStr(rd, "MODE_KIRIM")
                    TC_TotalQty = TCDec(rd, "TOTAL_QTY")
                    TC_TotalRupiah = TCDec(rd, "TOTAL_RUPIAH")
                    TC_IdUser = TCStr(rd, "ID_USER")
                    TC_StatusTransfer = TCStr(rd, "STATUS_TRANSFER")
                    TC_Keterangan = TCStr(rd, "KETERANGAN")
                End If
            End Using
        End Using

        ' Detail
        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, HARGA, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL
              FROM transfer_cabang_detail WHERE ID_TRANSFER = @id ORDER BY URUTAN", conn)
            cmd.Parameters.AddWithValue("@id", idTransfer)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    TC_DaftarItem.Add(New ItemTransferCabang With {
                        .IdBarang = TCStr(rd, "ID_BARANG"),
                        .NamaBarang = TCStr(rd, "NAMA_BARANG"),
                        .Harga = TCDec(rd, "HARGA"),
                        .Qty = TCDec(rd, "QTY"),
                        .Satuan = TCStr(rd, "SATUAN"),
                        .IsiSatuan = CInt(If(IsDBNull(rd("ISI_SATUAN")), 1, rd("ISI_SATUAN"))),
                        .QtySatuan = TCDec(rd, "TOTAL_QTY"),
                        .Total = TCDec(rd, "TOTAL")
                    })
                End While
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER
    ' ============================================================
    Public Function TCRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function TCStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function TCDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterTransferCabang(idTransfer As String)
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

        If Not String.IsNullOrEmpty(pilihan) Then CetakTransferCabang(idTransfer, pilihan)
    End Sub

    ' ============================================================
    ' ENTRY POINT
    ' ============================================================
    Public Sub CetakTransferCabang(idTransfer As String)
        CetakTransferCabang(idTransfer, "")
    End Sub

    Public Sub CetakTransferCabang(idTransfer As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(idTransfer) Then Exit Sub
        MuatDataTransferCabang(idTransfer)
        If TC_DaftarItem.Count = 0 Then Exit Sub

        Dim cfgT As New KonfigurasiThermal("TransferCabang")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride),
                                  cfgT.JenisPrinter,
                                  jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Thermal"
                    If cfgT.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakTransferCabang() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakTransferCabang("TransferCabang") : c.CetakThermal()
                    End If
                Case "Printer Dot Matrix"
                    Dim cfgDot As New KonfigurasiDotMatrix("TransferCabang")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakTransferCabang() : c.CetakDotMatrix()
                    Else
                        Dim c As New EscPosCetakTransferCabang("TransferCabang") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakTransferCabangInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("TransferCabang")
                    ModuleCetakTransferCabangPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New GdiCetakTransferCabang() : c.Cetak()
            End Select
        Catch ex As Exception
            SyncLog.Tulis("ERROR", "cetak", idTransfer, "", "CetakTransferCabang: " & ex.Message)
        End Try
    End Sub

End Module
