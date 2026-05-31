' ================================================================
' ModulePrinterJual
' Pusat data dan entry point untuk cetak nota penjualan.
'
' Semua variabel data transaksi disimpan di sini dengan prefix
' "Jual_" agar tidak bentrok saat nanti ada ModulePrinterBeli,
' ModulePrinterReturJual, dst.
'
' Alur kerja:
'   1. MuatDataPenjualan(noFaktur) — query DB, isi semua Jual_*
'   2. CetakPenjualan(noFaktur)    — muat data lalu pilih class cetak
'   3. ESC/POS dan GDI+ langsung baca Jual_* dari modul ini,
'      tidak perlu query DB sendiri maupun deklarasi field private.
'
' Cara pakai dari FormPenjualan:
'   ModulePrinterJual.CetakPenjualan(TxtFaktur.Text)
'   ModulePrinterJual.PreviewPenjualan(TxtFaktur.Text)
' ================================================================
Module ModulePrinterJual

    ' ============================================================
    ' DATA HEADER NOTA PENJUALAN
    ' Diisi oleh MuatDataPenjualan() — dibaca oleh ESC/POS & GDI+
    ' ============================================================
    Public Jual_NoFaktur As String = ""
    Public Jual_JudulNota As String = "Nota Jual"
    Public Jual_Tanggal As DateTime
    Public Jual_NamaPelanggan As String = ""
    Public Jual_JenisPelanggan As String = ""
    Public Jual_IdPelanggan As String = ""
    Public Jual_TotalSebelumPajak As Decimal
    Public Jual_Diskon As Decimal
    Public Jual_DiskonPersen As Decimal
    Public Jual_Pajak As Decimal
    Public Jual_PajakPersen As Decimal
    Public Jual_BiayaKirim As Decimal
    Public Jual_Total As Decimal
    Public Jual_Bayar As Decimal
    Public Jual_Kembali As Decimal
    Public Jual_LabelPembayaran As String = "Kembali :"
    Public Jual_StatusTransaksi As String = ""
    Public Jual_JatuhTempo As String = ""          ' string untuk ESC/POS (sudah diformat)
    Public Jual_JatuhTempoDate As DateTime         ' DateTime untuk GDI+
    Public Jual_AdaJatuhTempo As Boolean = False
    Public Jual_TypeAkun As String = ""
    Public Jual_Penerima As String = ""            ' JENIS_PEMBAYARAN — nama akun kas (misal "KAS DI TOKO")
    Public Jual_NamaAkunTransfer As String = ""    ' NAMA_AKUN_TF — nama akun transfer (misal "TRANSFER BANK")
    Public Jual_Metode As String = ""              ' METODE — "Tunai", "Tunai + Transfer", dll
    Public Jual_Bank As String = ""
    Public Jual_NamaRekening As String = ""
    Public Jual_NoRekening As String = ""
    Public Jual_NoReferensi As String = ""
    Public Jual_NominalTransfer As Decimal         ' NOMINAL_TRANSFER — > 0 jika ada transfer
    Public Jual_IdUser As String = ""
    Public Jual_IdKomputer As String = ""
    Public Jual_NamaSales As String = ""
    Public Jual_LokasiBarang As String = ""
    Public Jual_NoSO As String = ""                ' NO_SO — nomor Sales Order referensi (kosong jika bukan dari SO)

    ' ── Data poin loyalitas ───────────────────────────────────
    Public Jual_PoinDiperoleh As Integer = 0       ' Poin EARN dari faktur ini (0 jika tidak ada / poin tidak aktif)
    Public Jual_SaldoPoinAkhir As Integer = 0      ' Saldo poin pelanggan setelah transaksi ini

    ' ── Data hutang pelanggan (Model 8) ──────────────────────
    Public Jual_HutangAwal As Decimal
    Public Jual_TotalHutang As Decimal
    Public Jual_TotalBayarHutang As Decimal
    Public Jual_HutangAkhir As Decimal
    Public Jual_JangkaPiutang As Integer
    Public Jual_AdaDataHutang As Boolean = False

    ' ── Data item barang ─────────────────────────────────────
    Public Jual_DaftarItem As New List(Of ItemNotaJual)

    Public Class ItemNotaJual
        Public NamaBarang As String = ""
        Public Qty As Decimal
        Public Satuan As String = ""
        Public Harga As Decimal
        Public TotalDiskon As Decimal
        Public TotalHarga As Decimal
        Public SerialNumber As String = ""
    End Class

    ' ============================================================
    ' MUAT DATA DARI DATABASE
    ' Dipanggil oleh CetakPenjualan() sebelum class cetak dijalankan.
    ' Bisa juga dipanggil manual jika perlu akses data sebelum cetak.
    ' ============================================================
    Public Sub MuatDataPenjualan(noFaktur As String, Optional isSalesOrder As Boolean = False)
        Jual_NoFaktur = noFaktur
        Jual_JudulNota = If(isSalesOrder, "Nota SO", "Nota Jual")
        Jual_NoSO = ""   ' reset dulu, diisi oleh MuatHeaderPenjualan jika bukan SO
        MuatItemPenjualan(noFaktur, isSalesOrder)
        MuatHeaderPenjualan(noFaktur, isSalesOrder)
        MuatHutangPelangganJual()
        MuatDataPoinJual(noFaktur)
    End Sub

    Private Sub MuatItemPenjualan(noFaktur As String, isSalesOrder As Boolean)
        Jual_DaftarItem.Clear()
        Dim query As String
        If isSalesOrder Then
            query = "SELECT NAMA_BARANG, '' AS SERIAL_NUMBER, QTY, SATUAN, " &
                    "HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA " &
                    "FROM sales_order_detail WHERE FAKTUR_JUAL = @faktur " &
                    "ORDER BY URUTAN"
        Else
            query = "SELECT NAMA_BARANG, SERIAL_NUMBER, QTY, SATUAN, " &
                    "HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA " &
                    "FROM penjualan_detail WHERE FAKTUR_JUAL = @faktur " &
                    "ORDER BY URUTAN"
        End If

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@faktur", noFaktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Jual_DaftarItem.Add(New ItemNotaJual With {
                        .NamaBarang = DbStr(rd, "NAMA_BARANG"),
                        .Qty = DbDec(rd, "QTY"),
                        .Satuan = DbStr(rd, "SATUAN"),
                        .Harga = DbDec(rd, "HARGA_JUAL"),
                        .TotalDiskon = DbDec(rd, "TOTAL_DISKON"),
                        .TotalHarga = DbDec(rd, "TOTAL_HARGA"),
                        .SerialNumber = DbStr(rd, "SERIAL_NUMBER")
                    })
                End While
            End Using
        End Using
    End Sub

    Private Sub MuatHeaderPenjualan(noFaktur As String, isSalesOrder As Boolean)
        Dim query As String
        If isSalesOrder Then
            query = "SELECT NAMA_PELANGGAN, JENIS_PELANGGAN, TGL_TRANSAKSI, " &
                    "GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_RP, DISKON_TOTAL_PERSEN, " &
                    "GRAND_TOTAL_STL_PAJAK, PAJAK_RP, PAJAK_PERSEN, " &
                    "BIAYA_KIRIM, 0 AS BAYAR, 0 AS KEMBALI, 0 AS SISA_TAGIHAN, NULL AS JATUH_TEMPO, " &
                    "STATUS_TRANSAKSI, '' AS TYPE_AKUN, '' AS JENIS_PEMBAYARAN, " &
                    "0 AS NOMINAL_TRANSFER, '' AS NAMA_AKUN_TF, '' AS METODE, " &
                    "'' AS BANK, '' AS NO_REKENING, '' AS NAMA_REKENING, '' AS NO_REFFERENSI, " &
                    "NAMA_SALES, LOKASIBARANG, ID_PELANGGAN, ID_USER, ID_KOMPUTER, '' AS NO_SO " &
                    "FROM sales_order WHERE ID_PENJUALAN = @faktur"
        Else
            query = "SELECT NAMA_PELANGGAN, JENIS_PELANGGAN, TGL_TRANSAKSI, " &
                    "GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_RP, DISKON_TOTAL_PERSEN, " &
                    "GRAND_TOTAL_STL_PAJAK, PAJAK_RP, PAJAK_PERSEN, " &
                    "BIAYA_KIRIM, BAYAR, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, " &
                    "STATUS_TRANSAKSI, TYPE_AKUN, JENIS_PEMBAYARAN, " &
                    "NOMINAL_TRANSFER, NAMA_AKUN_TF, METODE, " &
                    "BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, " &
                    "NAMA_SALES, LOKASIBARANG, ID_PELANGGAN, ID_USER, ID_KOMPUTER, NO_SO " &
                    "FROM penjualan WHERE ID_PENJUALAN = @faktur"
        End If

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@faktur", noFaktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Jual_Tanggal = Convert.ToDateTime(rd("TGL_TRANSAKSI"))
                    Jual_NamaPelanggan = DbStr(rd, "NAMA_PELANGGAN")
                    Jual_JenisPelanggan = DbStr(rd, "JENIS_PELANGGAN")
                    Jual_TotalSebelumPajak = DbDec(rd, "GRAND_TOTAL_SBL_PAJAK")
                    Jual_Diskon = DbDec(rd, "DISKON_TOTAL_RP")
                    Jual_DiskonPersen = DbDec(rd, "DISKON_TOTAL_PERSEN")
                    Jual_Pajak = DbDec(rd, "PAJAK_RP")
                    Jual_PajakPersen = DbDec(rd, "PAJAK_PERSEN")
                    Jual_BiayaKirim = DbDec(rd, "BIAYA_KIRIM")
                    Jual_Total = DbDec(rd, "GRAND_TOTAL_STL_PAJAK")
                    Jual_Bayar = DbDec(rd, "BAYAR")
                    Jual_StatusTransaksi = DbStr(rd, "STATUS_TRANSAKSI")
                    Jual_TypeAkun = DbStr(rd, "TYPE_AKUN")
                    Jual_Penerima = DbStr(rd, "JENIS_PEMBAYARAN")
                    Jual_NominalTransfer = DbDec(rd, "NOMINAL_TRANSFER")
                    Jual_NamaAkunTransfer = DbStr(rd, "NAMA_AKUN_TF")
                    Jual_Metode = DbStr(rd, "METODE")
                    Jual_Bank = DbStr(rd, "BANK")
                    Jual_NamaRekening = DbStr(rd, "NAMA_REKENING")
                    Jual_NoRekening = DbStr(rd, "NO_REKENING")
                    Jual_NoReferensi = DbStr(rd, "NO_REFFERENSI")
                    Jual_IdUser = DbStr(rd, "ID_USER")
                    Jual_IdKomputer = DbStr(rd, "ID_KOMPUTER")
                    Jual_NamaSales = DbStr(rd, "NAMA_SALES")
                    Jual_LokasiBarang = DbStr(rd, "LOKASIBARANG")
                    Jual_IdPelanggan = DbStr(rd, "ID_PELANGGAN")
                    Jual_NoSO = DbStrSafe(rd, "NO_SO")

                    Dim sisa As Decimal = DbDec(rd, "SISA_TAGIHAN")
                    If sisa = 0 Then
                        Jual_Kembali = DbDec(rd, "KEMBALI")
                        Jual_LabelPembayaran = "Kembali :"
                        Jual_JatuhTempo = ""
                        Jual_AdaJatuhTempo = False
                    Else
                        Jual_Kembali = sisa
                        Jual_LabelPembayaran = "Hutang  :"
                        If Not IsDBNull(rd("JATUH_TEMPO")) Then
                            Jual_JatuhTempoDate = Convert.ToDateTime(rd("JATUH_TEMPO"))
                            Jual_JatuhTempo = Jual_JatuhTempoDate.ToString("dd-MM-yyyy")
                            Jual_AdaJatuhTempo = True
                        End If
                    End If
                End If
            End Using
        End Using
    End Sub

    Private Sub MuatHutangPelangganJual()
        Jual_AdaDataHutang = False
        If String.IsNullOrEmpty(Jual_IdPelanggan) Then Exit Sub
        Using cmd As New MySqlCommand(
            "SELECT HUTANGAWAL, TOTALHUTANG, TOTALBAYAR, HUTANGAKHIR, JANGKAPIUTANG " &
            "FROM tbl_pelanggan WHERE KODE = @kode", conn)
            cmd.Parameters.AddWithValue("@kode", Jual_IdPelanggan)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Jual_HutangAwal = DbDec(rd, "HUTANGAWAL")
                    Jual_TotalHutang = DbDec(rd, "TOTALHUTANG")
                    Jual_TotalBayarHutang = DbDec(rd, "TOTALBAYAR")
                    Jual_HutangAkhir = DbDec(rd, "HUTANGAKHIR")
                    Jual_JangkaPiutang = CInt(If(IsDBNull(rd("JANGKAPIUTANG")), 0, rd("JANGKAPIUTANG")))
                    Jual_AdaDataHutang = True
                End If
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Muat data poin loyalitas untuk struk — hanya jika sistem poin aktif dan ada pelanggan.
    ''' Req 6: cetak saldo poin dan poin diperoleh di struk.
    ''' </summary>
    Private Sub MuatDataPoinJual(noFaktur As String)
        Jual_PoinDiperoleh = 0
        Jual_SaldoPoinAkhir = 0

        ' Hanya muat jika sistem poin aktif dan ada pelanggan terpilih
        If Not LP_Aktif Then Exit Sub
        If String.IsNullOrEmpty(Jual_IdPelanggan) Then Exit Sub

        Try
            ' Ambil poin EARN dari faktur ini
            Jual_PoinDiperoleh = ModuleLoyaltyPoin.AmbilPoinEarnDariFaktur(noFaktur)

            ' Ambil saldo poin terkini pelanggan
            Jual_SaldoPoinAkhir = ModuleLoyaltyPoin.AmbilSaldoPoin(Jual_IdPelanggan)
        Catch ex As Exception
            ' Jika tabel poin belum ada (migrasi belum dijalankan), abaikan saja
            Debug.WriteLine($"[ModulePrinterJual.MuatDataPoinJual] {ex.Message}")
            Jual_PoinDiperoleh = 0
            Jual_SaldoPoinAkhir = 0
        End Try
    End Sub

    ' ============================================================
    ' HELPER FORMAT
    ' ============================================================
    Public Function JualRp(nilai As Decimal) As String
        Return nilai.ToString("#,0.##", cultureIndonesia)
    End Function

    ' Helper baca DB — dipakai oleh sub muat data di atas
    Friend Function DbStr(rd As MySqlDataReader, kolom As String) As String
        Return If(IsDBNull(rd(kolom)), "", rd(kolom).ToString().Trim())
    End Function
    Friend Function DbDec(rd As MySqlDataReader, kolom As String) As Decimal
        If IsDBNull(rd(kolom)) Then Return 0
        Dim v As Decimal
        Return If(Decimal.TryParse(rd(kolom).ToString(), v), v, 0)
    End Function
    ' Helper aman — tidak crash jika kolom belum ada di result set (kolom baru belum dimigrasi)
    Friend Function DbStrSafe(rd As MySqlDataReader, kolom As String, Optional defaultVal As String = "") As String
        Try
            Dim ordinal As Integer = rd.GetOrdinal(kolom)
            Return If(rd.IsDBNull(ordinal), defaultVal, rd.GetString(ordinal).Trim())
        Catch ex As IndexOutOfRangeException
            Return defaultVal
        End Try
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER — form custom inline, 6 tombol
    ' Baris 1: Thermal | Dot Matrix | Inkjet/Laser
    ' Baris 2: Monitor | Export PDF | Batal
    ' ============================================================
    Public Sub TanyaPilihPrinter(noFaktur As String, Optional isSalesOrder As Boolean = False)
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

        ' Judul
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

        ' Pemisah
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

        ' Baris 1: printer fisik
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

        ' Baris 2: monitor, pdf, batal
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

        ' Petunjuk ESC
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

        ' Events
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
            CetakPenjualan(noFaktur, pilihan, isSalesOrder)
        End If
    End Sub

    ' ============================================================
    ' ENTRY POINT CETAK PENJUALAN
    ' Overload tanpa override: pakai jenis printer dari printer.ini
    ' Overload dengan override: pakai jenis printer yang diberikan
    ' (dipakai saat TanyakanTampilPilihanPrinter = "Iya")
    ' ============================================================
    Public Sub CetakPenjualan(noFaktur As String, Optional isSalesOrder As Boolean = False)
        CetakPenjualan(noFaktur, "", isSalesOrder)
    End Sub

    Public Sub CetakPenjualan(noFaktur As String, jenisPrinterOverride As String, Optional isSalesOrder As Boolean = False)

        If String.IsNullOrEmpty(noFaktur) Then
            Exit Sub
        End If

        MuatDataPenjualan(noFaktur, isSalesOrder)

        Dim cfg As New KonfigurasiThermal("Jual")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride),
                                 cfg.JenisPrinter,
                                 jenisPrinterOverride)

        Debug.WriteLine($"[ModulePrinterJual.CetakPenjualan] Jenis: {jenis}")
        Debug.WriteLine($"[ModulePrinterJual.CetakPenjualan] ModeCetak: {cfg.ModeCetak}")
        Debug.WriteLine($"[ModulePrinterJual.CetakPenjualan] ModelStruk: {cfg.ModelStruk}")

        Try
            Select Case jenis
                Case "Printer Thermal"
                    Debug.WriteLine("[ModulePrinterJual.CetakPenjualan] Masuk ke Printer Thermal")
                    If cfg.ModeCetak = "GDI+ (Windows Print)" Then
                        Debug.WriteLine("[ModulePrinterJual.CetakPenjualan] Memanggil GdiCetakJualThermalMatrik.Cetak()")
                        Dim cetak As New GdiCetakJualThermalMatrik()
                        cetak.Cetak()
                    Else
                        Debug.WriteLine("[ModulePrinterJual.CetakPenjualan] Memanggil EscPosCetakJualThermalMatrik.CetakThermal()")
                        Dim cetak As New EscPosCetakjualThermalMatrik("Jual")
                        cetak.CetakThermal()
                    End If

                Case "Printer Dot Matrix"
                    Debug.WriteLine("[ModulePrinterJual.CetakPenjualan] Masuk ke Printer Dot Matrix")
                    Dim cfgDot As New KonfigurasiDotMatrix("Jual")
                    Debug.WriteLine($"[ModulePrinterJual.CetakPenjualan] Dot Matrix ModeCetak: {cfgDot.ModeCetak}")
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        Debug.WriteLine("[ModulePrinterJual.CetakPenjualan] Memanggil GdiCetakJualThermalMatrik.CetakDotMatrix()")
                        Dim cetakGdi As New GdiCetakJualThermalMatrik()
                        cetakGdi.CetakDotMatrix()
                    Else
                        Debug.WriteLine("[ModulePrinterJual.CetakPenjualan] Memanggil EscPosCetakJualThermalMatrik.CetakDotMatrix()")
                        Dim cetakEsc As New EscPosCetakjualThermalMatrik("Jual")
                        cetakEsc.CetakDotMatrix()
                    End If
                Case "Tampilkan di Monitor"
                    Dim cfgM As New KonfigurasiMonitor("Jual")
                    FormMonitorRDLC.TampilkanNota(cfgM.TampilFooter1, cfgM.TampilFooter2, cfgM.TampilFooter3)

                Case "Printer Inkjet / Laser"
                    Dim cfgInk As New KonfigurasiInkjet("Jual")
                    ModuleCetakJualInkjet.CetakNota()

                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("Jual")
                    ModuleCetakJualPdf.ExportPdf(cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)

                Case Else
                    Dim cetak As New EscPosCetakjualThermalMatrik("Jual")
                    cetak.CetakThermal()
            End Select


        Catch ex As Exception
        End Try
    End Sub

    ' ============================================================
    ' ENTRY POINT PREVIEW PENJUALAN
    ' ============================================================
    Public Sub PreviewPenjualan(noFaktur As String, Optional isSalesOrder As Boolean = False)
        If String.IsNullOrEmpty(noFaktur) Then Exit Sub
        MuatDataPenjualan(noFaktur, isSalesOrder)
        Dim cetak As New GdiCetakJualThermalMatrik()
        cetak.TampilkanPreview()
    End Sub

End Module
