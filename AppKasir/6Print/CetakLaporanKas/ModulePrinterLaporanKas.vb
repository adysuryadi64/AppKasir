' ================================================================
' ModulePrinterLaporanKas
' Entry point untuk cetak laporan mutasi keuangan dari FormLapSaldoKas.
' Data dibaca langsung dari TextBox form via BuatInstanceCetak().
'
' Cara pakai dari FormLapSaldoKas (BtnPrint_Click):
'   Dim cetak As GdiCetakLaporanKas = ModulePrinterLaporanKas.BuatInstanceCetak(Me)
'   ModuleCetakLaporanKasInkjet.CetakNota(cetak)   ' simpan instance aktif
'   ModulePrinterLaporanKas.TanyaPilihPrinterLaporanKas(Me)
' ================================================================
Module ModulePrinterLaporanKas

    ' ============================================================
    ' BUAT INSTANCE GdiCetakLaporanKas DARI FORM
    ' Membaca semua TextBox FormLapSaldoKas dan mengisi property
    ' GdiCetakLaporanKas sebelum dikirim ke printer.
    ' ============================================================
    Public Function BuatInstanceCetak(form As FormLapMutasiKeuangan) As GdiCetakLaporanKas
        Dim c As New GdiCetakLaporanKas()
        c.LK_Rekening = form.CmbRekening.Text
        c.LK_TypeAkun = form.TxtTypeAkun.Text
        c.LK_Kasir = form.CmbKasir.Text
        c.LK_PeriodeLabel = If(form.CbTanggal.Checked,
            form.DtpTanggal.Value.ToString("dd MMMM yyyy"),
            form.CmbBln.Text & " " & form.CmbThn.Text)
        c.LK_Pemilik = ""  ' isi dari tbl_perusahaan jika diperlukan

        Dim ParseDec = Function(txt As String) As Decimal
                           Dim v As Decimal
                           Decimal.TryParse(txt, Globalization.NumberStyles.Any, cultureIndonesia, v)
                           Return v
                       End Function
        Dim ParseInt = Function(txt As String) As Integer
                           Dim v As Integer
                           Integer.TryParse(txt, v)
                           Return v
                       End Function

        c.LK_TotalPembelian = ParseDec(form.TxtTotalPembelian.Text)
        c.LK_NotaPembelian = ParseInt(form.TxtNotaPembelian.Text)
        c.LK_TotalPenjualan = ParseDec(form.TxtTotalPenjualan.Text)
        c.LK_NotaPenjualan = ParseInt(form.TxtNotaPenjualan.Text)
        c.LK_TotalReturBeli = ParseDec(form.TxtTotalReturBeli.Text)
        c.LK_NotaReturBeli = ParseInt(form.TxtNotaReturBeli.Text)
        c.LK_TotalReturJual = ParseDec(form.TxtTotalReturJual.Text)
        c.LK_NotaReturJual = ParseInt(form.TxtNotaReturJual.Text)
        c.LK_TotalBayarHutang = ParseDec(form.TxtTotalBayarHutang.Text)
        c.LK_NotaBayarHutang = ParseInt(form.TxtNotaBAyarHutang.Text)
        c.LK_TotalBayarPiutang = ParseDec(form.TxtTotalBayarPiutang.Text)
        c.LK_NotaBayarPiutang = ParseInt(form.TxtNotaBayarPiutang.Text)
        c.LK_TotalPemasukan = ParseDec(form.TxtTotalJurnalPemasukan.Text)
        c.LK_NotaPemasukan = ParseInt(form.TxtNotaJurnalPemasukan.Text)
        c.LK_TotalPengeluaran = ParseDec(form.TxtTotalJurnalPengeluaran.Text)
        c.LK_NotaPengeluaran = ParseInt(form.TxtNotaJurnalPengeluaran.Text)
        c.LK_TotalBiaya = ParseDec(form.TxtTotalJurnalBiaya.Text)
        c.LK_NotaBiaya = ParseInt(form.TxtNotaJurnalBiaya.Text)
        c.LK_TotalPRDebet = ParseDec(form.TxtTotalJurnalPR.Text)
        c.LK_NotaPRDebet = ParseInt(form.TxtNotaJurnalPR.Text)
        c.LK_TotalPRKredit = ParseDec(form.TxtTotalJurnalPRK.Text)
        c.LK_NotaPRKredit = ParseInt(form.TxtNotaJurnalPRK.Text)
        c.LK_SetorBos = ParseDec(form.TxtSetorbos.Text)
        c.LK_TotalBon = ParseDec(form.TxtTotalJurnalBonKaryawan.Text)
        c.LK_NotaBon = ParseInt(form.TxtNotaJurnalBonKaryawan.Text)
        c.LK_TotalBayarBon = ParseDec(form.TxtTotalJurnalBayarBon.Text)
        c.LK_NotaBayarBon = ParseInt(form.TxtNotaJurnalBayarBon.Text)
        c.LK_TotalGaji = ParseDec(form.TxtTotalJurnalGaji.Text)
        c.LK_NotaGaji = ParseInt(form.TxtNotaJurnalGaji.Text)
        c.LK_TotalPinjamanSupplier = ParseDec(form.TxtTotalJurnalPinjamSupplier.Text)
        c.LK_NotaPinjamanSupplier = ParseInt(form.TxtNotaJurnalPinjamSupplier.Text)
        c.LK_TotalPinjamanPelanggan = ParseDec(form.TxtTotalJurnalPinjamPelanggan.Text)
        c.LK_NotaPinjamanPelanggan = ParseInt(form.TxtNotaJurnalPinjamPelanggan.Text)
        c.LK_SaldoAwal = ParseDec(form.TxtSaldoAwal.Text)
        c.LK_SaldoHariIni = ParseDec(form.TxtTotalHariIni.Text)
        c.LK_TotalHariIni = ParseDec(form.TxtSaldoHAriIni.Text)
        c.LK_SaldoDilaci = ParseDec(form.TxtSaldoDilaci.Text)
        c.LK_SaldoAkhir = ParseDec(form.TxtSaldoAkhir.Text)

        Return c
    End Function

    ' ============================================================
    ' TANYA PILIH PRINTER
    ' ============================================================
    Public Sub TanyaPilihPrinterLaporanKas(form As FormLapMutasiKeuangan)
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
        If Not String.IsNullOrEmpty(pilihan) Then
            CetakLaporanKas(pilihan)
        End If
    End Sub

    ' ============================================================
    ' ENTRY POINT CETAK
    ' ============================================================
    Public Sub CetakLaporanKas(jenisPrinterOverride As String)
        Dim cfgDot As New KonfigurasiDotMatrix("LaporanKas")
        Dim jenis As String = If(String.IsNullOrEmpty(jenisPrinterOverride), BacaPengaturanPrinter("LaporanKas", "JenisPrinter", "Printer Dot Matrix"), jenisPrinterOverride)
        Dim cetak As GdiCetakLaporanKas = ModuleCetakLaporanKasInkjet.GetCurrentCetak()
        If cetak Is Nothing Then
            MessageBox.Show("Data laporan belum disiapkan.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Try
            Select Case jenis
                Case "Printer Dot Matrix"
                    If cfgDot.ModeCetak = "GDI+ (Windows Print)" Then
                        cetak.CetakDotMatrix()
                    Else
                        Dim c As New EscPosCetakLaporanKas("LaporanKas") : c.CetakDotMatrix()
                    End If
                Case "Printer Inkjet / Laser"
                    ModuleCetakLaporanKasInkjet.CetakNota(cetak)
                Case "Export ke PDF"
                    Dim cfgPdf As New KonfigurasiPDF("LaporanKas")
                    ModuleCetakLaporanKasPdf.ExportPdf(cetak, cfgPdf.TampilFooter1, cfgPdf.TampilFooter2, cfgPdf.TampilFooter3)
                Case Else
                    cetak.Cetak()
            End Select
        Catch ex As Exception
        End Try
    End Sub

End Module
