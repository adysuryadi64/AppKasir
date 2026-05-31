' ================================================================
' ModulePrinterBeli
' Pusat data dan entry point untuk cetak nota pembelian.
' Prefix variabel: "Beli_"
'
' Cara pakai dari FormPembelian:
'   ModulePrinterBeli.CetakPembelian(TxtFaktur.Text)
'   ModulePrinterBeli.TanyaPilihPrinterBeli(TxtFaktur.Text)
' ================================================================
Module ModulePrinterBeli

    ' ============================================================
    ' DATA HEADER
    ' ============================================================
    Public Beli_IdPembelian As String = ""
    Public Beli_NotaPembelian As String = ""
    Public Beli_Tanggal As DateTime
    Public Beli_NamaSupplier As String = ""
    Public Beli_AlamatSupplier As String = ""
    Public Beli_KontakSupplier As String = ""
    Public Beli_Lokasi As String = ""
    Public Beli_JenisBayar As String = ""
    Public Beli_Pembayaran As Decimal
    Public Beli_NominalTransfer As Decimal
    Public Beli_NamaAkunTf As String = ""
    Public Beli_Tagihan As Decimal
    Public Beli_NominalBayar As Decimal
    Public Beli_JatuhTempo As DateTime
    Public Beli_StatusTransaksi As String = ""
    Public Beli_IdUser As String = ""
    Public Beli_IdKomputer As String = ""
    ' Data hutang supplier dari tbl_supliyer
    Public Beli_HutangAkhir As Decimal

    ' ── Data item barang ─────────────────────────────────────
    Public Beli_DaftarItem As New List(Of ItemNotaBeli)

    Public Class ItemNotaBeli
        Public IdBarang As String = ""
        Public NamaBarang As String = ""
        Public Qty As Decimal
        Public Satuan As String = ""
        Public HargaBeli As Decimal
        Public Total As Decimal
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' ============================================================
    Public Sub MuatDataBeli(idPembelian As String)
        Beli_IdPembelian = idPembelian
        MuatItemBeli(idPembelian)
        MuatHeaderBeli(idPembelian)
    End Sub

    Private Sub MuatItemBeli(idPembelian As String)
        Beli_DaftarItem.Clear()
        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_BELI, TOTAL " &
            "FROM pembelian_detail WHERE FAKTUR_BELI = @id ORDER BY URUTAN", conn)
            cmd.Parameters.AddWithValue("@id", idPembelian)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Beli_DaftarItem.Add(New ItemNotaBeli With {
                        .IdBarang = BeliDbStr(rd, "ID_BARANG"),
                        .NamaBarang = BeliDbStr(rd, "NAMA_BARANG"),
                        .Qty = BeliDbDec(rd, "QTY"),
                        .Satuan = BeliDbStr(rd, "SATUAN"),
                        .HargaBeli = BeliDbDec(rd, "HARGA_BELI"),
                        .Total = BeliDbDec(rd, "TOTAL")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderBeli(idPembelian As String)
        ' JOIN tbl_supliyer untuk alamat, kontak, dan data hutang supplier
        Using cmd As New MySqlCommand(
            "SELECT p.NOTA_PEMBELIAN, p.TGL_BELI, p.NAMA_SUPLIYER, " &
            "COALESCE(s.ALAMAT,'') AS ALAMAT_SUPLIYER, COALESCE(s.HP,'') AS KONTAK_SUPLIYER, " &
            "p.LOKASI, p.JENIS_BAYAR, p.PEMBAYARAN, " &
            "COALESCE(p.NOMINAL_TRANSFER,0) AS NOMINAL_TRANSFER, " &
            "COALESCE(p.NAMA_AKUN_TF,'') AS NAMA_AKUN_TF, " &
            "p.TAGIHAN, p.NOMINALBAYAR, " &
            "p.JATUH_TEMPO, p.STATUS_TRANSAKSI_BELI, p.ID_USER, p.ID_KOMPUTER, " &
            "COALESCE(s.HUTANGAKHIR,0) AS HUTANGAKHIR " &
            "FROM pembelian p " &
            "LEFT JOIN tbl_supliyer s ON s.KODE = p.ID_SUPPLIER " &
            "WHERE p.ID_PEMBELIAN = @id", conn)
            cmd.Parameters.AddWithValue("@id", idPembelian)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Beli_NotaPembelian = BeliDbStr(rd, "NOTA_PEMBELIAN")
                    Beli_Tanggal = If(IsDBNull(rd("TGL_BELI")), Date.MinValue, Convert.ToDateTime(rd("TGL_BELI")))
                    Beli_NamaSupplier = BeliDbStr(rd, "NAMA_SUPLIYER")
                    Beli_AlamatSupplier = BeliDbStr(rd, "ALAMAT_SUPLIYER")
                    Beli_KontakSupplier = BeliDbStr(rd, "KONTAK_SUPLIYER")
                    Beli_Lokasi = BeliDbStr(rd, "LOKASI")
                    Beli_JenisBayar = BeliDbStr(rd, "JENIS_BAYAR")
                    Beli_Pembayaran = BeliDbDec(rd, "PEMBAYARAN")
                    Beli_NominalTransfer = BeliDbDec(rd, "NOMINAL_TRANSFER")
                    Beli_NamaAkunTf = BeliDbStr(rd, "NAMA_AKUN_TF")
                    Beli_Tagihan = BeliDbDec(rd, "TAGIHAN")
                    Beli_NominalBayar = BeliDbDec(rd, "NOMINALBAYAR")
                    Beli_JatuhTempo = If(IsDBNull(rd("JATUH_TEMPO")), Date.MinValue, Convert.ToDateTime(rd("JATUH_TEMPO")))
                    Beli_StatusTransaksi = BeliDbStr(rd, "STATUS_TRANSAKSI_BELI")
                    Beli_IdUser = BeliDbStr(rd, "ID_USER")
                    Beli_IdKomputer = BeliDbStr(rd, "ID_KOMPUTER")
                    Beli_HutangAkhir = BeliDbDec(rd, "HUTANGAKHIR")
                End If
            End Using
        End Using
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function BeliRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function
    Friend Function BeliDbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function BeliDbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterBeli(idPembelian As String)
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
        If Not String.IsNullOrEmpty(pilihan) Then CetakPembelian(idPembelian, pilihan)
    End Sub

    ' ============================================================
    ' ENTRY POINT
    ' ============================================================
    Public Sub CetakPembelian(idPembelian As String)
        CetakPembelian(idPembelian, "")
    End Sub

    Public Sub CetakPembelian(idPembelian As String, jenisPrinterOverride As String)
        If String.IsNullOrEmpty(idPembelian) Then Exit Sub
        MuatDataBeli(idPembelian)
        Dim cfg As New KonfigurasiThermal("Beli")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride), cfg.JenisPrinter, jenisPrinterOverride)
        Try
            Select Case jenis
                Case "Printer Thermal"
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBeliThermalMatrik() : c.Cetak()
                    Else
                        Dim c As New EscPosCetakBeliThermalMatrik("Beli") : c.CetakThermal()
                    End If
                Case "Printer Dot Matrix"
                    Dim cfgDot As New KonfigurasiDotMatrix("Beli")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Dim c As New GdiCetakBeliThermalMatrik() : c.CetakDotMatrix()
                    Else
                        Dim c As New EscPosCetakBeliThermalMatrik("Beli") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakBeliInkjet.CetakNota()
                Case "Tampilkan di Monitor"
                    NotaPembelian.TampilkanNota(idPembelian)
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("Beli")
                    ModuleCetakBeliPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    Dim c As New EscPosCetakBeliThermalMatrik("Beli") : c.CetakThermal()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
