' ================================================================
' ModulePrinterStokOpname — Laporan stok opname
' Prefix: "SO_"
' ================================================================
Module ModulePrinterStokOpname

    Public SO_IdOpname As String = ""
    Public SO_Tanggal As DateTime
    Public SO_Lokasi As String = ""
    Public SO_IdUser As String = ""
    Public SO_IdKomputer As String = ""

    Public SO_DaftarItem As New List(Of ItemStokOpname)

    Public Class ItemStokOpname
        Public NamaBarang As String = ""
        Public Satuan As String = ""
        Public StokSystem As Decimal
        Public StokNyata As Decimal
        Public StokSelisih As Decimal
        Public TotalHarga As Decimal
    End Class

    Public Sub MuatDataStokOpname(idOpname As String)
        SO_IdOpname = idOpname
        SO_DaftarItem.Clear()
        Using cmd As New MySqlCommand(
            "SELECT NAMA_BARANG, SATUAN, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, TOTAL_HARGA, " &
            "TANGGAL, LOKASI, ID_USER, ID_KOMPUTER " &
            "FROM stok_opname WHERE ID_STOK_OPNAME = @id ORDER BY NAMA_BARANG", conn)
            cmd.Parameters.AddWithValue("@id", idOpname)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Dim firstRow As Boolean = True
                While rd.Read()
                    If firstRow Then
                        SO_Tanggal = If(IsDBNull(rd("TANGGAL")), Date.MinValue, Convert.ToDateTime(rd("TANGGAL")))
                        SO_Lokasi = SODbStr(rd, "LOKASI")
                        SO_IdUser = SODbStr(rd, "ID_USER")
                        SO_IdKomputer = SODbStr(rd, "ID_KOMPUTER")
                        firstRow = False
                    End If
                    SO_DaftarItem.Add(New ItemStokOpname With {
                        .NamaBarang = SODbStr(rd, "NAMA_BARANG"),
                        .Satuan = SODbStr(rd, "SATUAN"),
                        .StokSystem = SODbDec(rd, "STOK_SYSTEM"),
                        .StokNyata = SODbDec(rd, "STOK_NYATA"),
                        .StokSelisih = SODbDec(rd, "STOK_SELISIH"),
                        .TotalHarga = SODbDec(rd, "TOTAL_HARGA")
                    })
                End While
            End Using
        End Using
    End Sub

    Public Function SORp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function SODbStr(rd As MySqlDataReader, k As String) As String
        Return If(IsDBNull(rd(k)), "", rd(k).ToString().Trim())
    End Function
    Friend Function SODbDec(rd As MySqlDataReader, k As String) As Decimal
        If IsDBNull(rd(k)) Then Return 0
        Dim v As Decimal : Return If(Decimal.TryParse(rd(k).ToString(), v), v, 0)
    End Function

    Public Sub TanyaPilihPrinterStokOpname(idOpname As String)
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
        If Not String.IsNullOrEmpty(pilihan) Then CetakStokOpname(idOpname, pilihan)
    End Sub

    Public Sub CetakStokOpname(idOpname As String)
        CetakStokOpname(idOpname, "")
    End Sub

    Public Sub CetakStokOpname(idOpname As String, jenis As String)
        If String.IsNullOrEmpty(idOpname) Then Exit Sub
        MuatDataStokOpname(idOpname)
        Dim cfgDot As New KonfigurasiDotMatrix("StokOpname")
        Dim j As String = If(String.IsNullOrEmpty(jenis), BacaPengaturanPrinter("StokOpname", "JenisPrinter", "Printer Dot Matrix"), jenis)
        Try
            Select Case j
                Case "Printer Dot Matrix"
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakStokOpname() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakStokOpname("StokOpname") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakStokOpnameInkjet.CetakNota()
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("StokOpname")
                    ModuleCetakStokOpnamePdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New GdiCetakStokOpname() : c.Cetak()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
