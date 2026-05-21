' ================================================================
' ClassCetakPenjualanThermalMatrik
' Satu class lengkap untuk cetak nota penjualan:
'   - Query data dari database sendiri (cukup berikan nomor faktur)
'   - Thermal via PrinterEscPos (ESCPOS.NET) — ESC/POS raw bytes
'   - Dot Matrix via PrinterEscPos (ESCPOS.NET) — ESC/POS raw bytes
'   - Tata letak kolom sama persis dengan PrintJual.vb (versi lama)
'
' Cara pakai:
'   Dim cetak As New ClassCetakPenjualanThermalMatrik("Jual", "JL-2026-001")
'   cetak.MuatData()
'   cetak.CetakThermal()    ' atau cetak.CetakDotMatrix()
'
' Posisi kolom (dalam karakter, dari kiri):
'   Thermal 80mm (48 kar):
'     Model dengan Diskon  : Nama=0, Qty=11%, Harga=51%, Disc=70%, Jumlah=95%
'     Model tanpa Diskon   : Nama=0, Qty=11%, Harga=65%, Jumlah=95%
'   Dot Matrix (lebar dari cfgDot.LebarKertas):
'     No=5%, Nama=20%, Qty=50%, Sat=50%, Harga=68%, Disc=80%, Jumlah=93%
' ================================================================
Public Class EscPosCetakjualThermalMatrik

#Region "Field & Konstruktor"

    ' ── Konfigurasi printer ──────────────────────────────────
    Private ReadOnly _cfg As KonfigurasiThermal
    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    ' Data transaksi sudah dimuat di ModulePrinterJual.MuatDataPenjualan()
    Public Sub New(transaksi As String)
        _cfg = New KonfigurasiThermal(transaksi)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private ReadOnly Property JumlahItem As Integer
        Get
            Return Jual_DaftarItem.Count
        End Get
    End Property

    Private Function Rp(nilai As Decimal) As String
        Return JualRp(nilai)
    End Function

#End Region

#Region "Helper Tata Letak Kolom ESC/POS"

    ' Posisi kolom dihitung dari persentase lebar kertas,
    ' sama persis dengan PrintJual.vb (GDI+ versi lama).
    '
    ' Thermal 80mm = 48 karakter, 58mm = 32 karakter.
    ' Persentase diambil dari Mulaikata di PrintJual.vb:
    '   Mulaikata  = 25% (posisi nilai kanan di info transaksi)
    '   Mulaikata1 = 11% (qty, rata kanan)
    '   Mulaikata2 = 11% (satuan, rata kiri)
    '   Mulaikata3 = 51% (harga, rata kanan) — model dengan diskon
    '   Mulaikata3 = 65% (harga, rata kanan) — model tanpa diskon
    '   Mulaikata4 = 70% (disc, rata kanan)
    '   Mulaikata5 = 95% (jumlah, rata kanan)

    Private Function RataKanan(teks As String, lebarKolom As Integer) As String
        If teks.Length >= lebarKolom Then Return teks.Substring(teks.Length - lebarKolom)
        Return teks.PadLeft(lebarKolom)
    End Function

    Private Function RataKiri(teks As String, lebarKolom As Integer) As String
        If teks.Length >= lebarKolom Then Return teks.Substring(0, lebarKolom)
        Return teks.PadRight(lebarKolom)
    End Function

    Private Function KiriKanan(kiri As String, kanan As String, totalLebar As Integer) As String
        Dim spasi As Integer = totalLebar - kiri.Length - kanan.Length
        If spasi < 1 Then spasi = 1
        Return kiri & New String(" "c, spasi) & kanan
    End Function

#End Region


#Region "Cetak Thermal — Dispatcher Model"

    Public Sub CetakThermal()
        Debug.WriteLine("[EscPosCetakJualThermalMatrik.CetakThermal] Mulai cetak THERMAL")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakThermal] NamaPrinter: {_cfg.NamaPrinter}")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakThermal] ModeCetak: {_cfg.ModeCetak}")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakThermal] ModelStruk: {_cfg.ModelStruk}")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakThermal] LebarKertas: {_cfg.LebarKertas} mm")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakThermal] UkuranKertas: {_cfg.UkuranKertas}")
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' GDI+ mode: redirect ke GdiCetakJualThermalMatrik
        If _cfg.ModeCetak = "GDI+ (Windows Print)" Then
            Debug.WriteLine("[EscPosCetakJualThermalMatrik.CetakThermal] Mode GDI+ terdeteksi, redirect ke GdiCetakJualThermalMatrik")
            Dim cetak As New GdiCetakJualThermalMatrik()
            cetak.Cetak()
            Exit Sub
        End If
        Debug.WriteLine("[EscPosCetakJualThermalMatrik.CetakThermal] Mode ESC/POS, cetak langsung")

        ' ESC/POS mode: cetak langsung via ESCPOS.NET
        For i As Integer = 1 To _cfg.JumlahCetak
            Dim esc As PrinterEscPos
            If _cfg.TipeKoneksi = "Network / WiFi (IP)" AndAlso Not String.IsNullOrEmpty(_cfg.IpAddress) Then
                esc = New PrinterEscPos(_cfg.IpAddress, _cfg.NetworkPort, _cfg.LebarKertas, _cfg.BatasKiri)
            Else
                esc = New PrinterEscPos(_cfg.NamaPrinter, _cfg.LebarKertas, _cfg.BatasKiri)
            End If

            Select Case _cfg.ModelStruk
                Case "Model 1 — Judul Kolom, Diskon, Sisa Hutang" : CetakSatu(esc, True, True, True)
                Case "Model 2 — Judul Kolom, Diskon" : CetakSatu(esc, True, True, False)
                Case "Model 3 — Judul Kolom, Sisa Hutang" : CetakSatu(esc, True, False, True)
                Case "Model 4 — Judul Kolom" : CetakSatu(esc, True, False, False)
                Case "Model 5 — Diskon, Sisa Hutang" : CetakSatu(esc, False, True, True)
                Case "Model 6 — Diskon" : CetakSatu(esc, False, True, False)
                Case "Model 7 — Sisa Hutang" : CetakSatu(esc, False, False, True)
                Case "Model 8 — Ringkas" : CetakSatu(esc, False, False, False)
                Case Else : CetakSatu(esc, True, True, False)
            End Select
        Next

        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir("Jual")
    End Sub

#End Region

#Region "Model Thermal — 8 Kombinasi"

    ' Satu fungsi universal — semua 8 model memanggil ini dengan parameter berbeda
    ' pakaHeader : tampilkan baris header kolom (Nama Barang | Harga | Disc | Jml)
    ' pakaDiskon : tampilkan kolom Disc di item + posisi harga 51% (vs 65% tanpa diskon)
    ' pakaHutang : tampilkan sisa hutang pelanggan setelah total
    Private Sub CetakSatu(esc As PrinterEscPos,
                           pakaHeader As Boolean,
                           pakaDiskon As Boolean,
                           pakaHutang As Boolean)
        CetakHeaderToko(esc)
        CetakInfoTransaksi(esc)
        CetakItem(esc, pakaHeader, pakaDiskon)
        CetakTotal(esc, pakaDiskon)
        CetakInfoBank(esc)
        If pakaHutang AndAlso Jual_AdaDataHutang Then CetakRingkasanHutangPelanggan(esc)
        CetakFooter(esc)
    End Sub

    ' Alias untuk kompatibilitas kode lama yang masih memanggil CetakModel1-8
    Private Sub CetakModel1(esc As PrinterEscPos)
        CetakSatu(esc, True, True, False)
    End Sub
    Private Sub CetakModel2(esc As PrinterEscPos)
        CetakSatu(esc, True, False, False)
    End Sub
    Private Sub CetakModel3(esc As PrinterEscPos)
        CetakSatu(esc, False, False, False)
    End Sub
    Private Sub CetakModel4(esc As PrinterEscPos)
        CetakSatu(esc, True, False, False)
    End Sub
    Private Sub CetakModel5(esc As PrinterEscPos)
        CetakSatu(esc, True, False, False)
    End Sub
    Private Sub CetakModel6(esc As PrinterEscPos)
        CetakSatu(esc, True, True, False)
    End Sub
    Private Sub CetakModel7(esc As PrinterEscPos)
        CetakSatu(esc, True, True, False)
    End Sub
    Private Sub CetakModel8(esc As PrinterEscPos)
        CetakSatu(esc, True, True, True)
    End Sub

#End Region


#Region "Blok Header & Info Transaksi Thermal"

    Private Sub CetakRingkasanHutangPelanggan(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakBaris(KiriKanan("Sisa Hutang :", Rp(Jual_HutangAkhir), n), _cfg.EscUkuranIsi)
        esc.CetakGaris()
    End Sub

    Private Sub CetakHeaderToko(esc As PrinterEscPos)
        If _cfg.TampilLogo Then
            Dim logoPath As String = System.IO.Path.Combine(Application.StartupPath, "logo.png")
            If Not System.IO.File.Exists(logoPath) Then
                logoPath = System.IO.Path.Combine(Application.StartupPath, "logo.jpg")
            End If
            Dim maxPx As Integer = If(_cfg.LebarKertas >= 80, 384, 256)
            esc.CetakLogo(logoPath, maxPx)
        End If
        esc.CetakHeader(NAMA_PERUSAHAAN, _cfg.EscUkuranJudul)
        esc.CetakTengah(ALAMAT_PERUSAHAAN, _cfg.EscUkuranKeterangan)
        esc.CetakTengah(KOTA_PERUSAHAAN, _cfg.EscUkuranKeterangan)
        esc.CetakTengah(KONTAK_PERUSAHAAN, _cfg.EscUkuranKeterangan)
        ' Tampilkan label jenis dokumen jika Sales Order
        If Jual_JudulNota = "Nota Order" Then
            esc.CetakTengah("================================", _cfg.EscUkuranIsi)
            esc.CetakTengah("  ** NOTA PESANAN / SALES ORDER **  ", _cfg.EscUkuranIsi)
            esc.CetakTengah("================================", _cfg.EscUkuranIsi)
        End If
    End Sub

    Private Sub CetakInfoTransaksi(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakGaris()
        esc.CetakBaris(FormatLabelNilai(Jual_JudulNota, ": " & Jual_NoFaktur, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Tanggal  ", ": " & Jual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Kasir    ", ": " & Jual_IdUser & " - " & Jual_IdKomputer, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Pelanggan", ": " & Jual_JenisPelanggan & " - " & Jual_NamaPelanggan, n), _cfg.EscUkuranIsi)
        If Not String.IsNullOrEmpty(Jual_NoSO) Then
            esc.CetakBaris(FormatLabelNilai("Ref. SO  ", ": " & Jual_NoSO, n), _cfg.EscUkuranIsi)
        End If
        esc.CetakGaris()
    End Sub

    Private Sub CetakInfoTransaksiSingkat(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakGaris()
        esc.CetakBaris(FormatLabelNilai(Jual_JudulNota, ": " & Jual_NoFaktur, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Tgl      ", ": " & Jual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Kasir    ", ": " & Jual_IdUser & " - " & Jual_IdKomputer, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Pel      ", ": " & Jual_JenisPelanggan & " - " & Jual_NamaPelanggan, n), _cfg.EscUkuranIsi)
        If Not String.IsNullOrEmpty(Jual_NoSO) Then
            esc.CetakBaris(FormatLabelNilai("Ref. SO  ", ": " & Jual_NoSO, n), _cfg.EscUkuranIsi)
        End If
        esc.CetakGaris()
    End Sub

    Private Sub CetakInfoTransaksiDenganSales(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakGaris()
        esc.CetakBaris(FormatLabelNilai(Jual_JudulNota, ": " & Jual_NoFaktur, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Tanggal  ", ": " & Jual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Kasir    ", ": " & Jual_IdUser & " - " & Jual_IdKomputer, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Pelanggan", ": " & Jual_JenisPelanggan & " - " & Jual_NamaPelanggan, n), _cfg.EscUkuranIsi)
        If Not String.IsNullOrEmpty(Jual_NamaSales) Then
            esc.CetakBaris(FormatLabelNilai("Sales    ", ": " & Jual_NamaSales, n), _cfg.EscUkuranIsi)
        End If
        If Not String.IsNullOrEmpty(Jual_LokasiBarang) Then
            esc.CetakBaris(FormatLabelNilai("Lokasi   ", ": " & Jual_LokasiBarang, n), _cfg.EscUkuranIsi)
        End If
        If Not String.IsNullOrEmpty(Jual_NoSO) Then
            esc.CetakBaris(FormatLabelNilai("Ref. SO  ", ": " & Jual_NoSO, n), _cfg.EscUkuranIsi)
        End If
        esc.CetakGaris()
    End Sub

    ' Label 9 karakter, nilai mulai kolom 11 (sama dengan Mulaikata=25% di PrintJual.vb)
    Private Function FormatLabelNilai(label As String, nilai As String, totalLebar As Integer) As String
        Const lebarLabel As Integer = 9
        Dim sisaLebar As Integer = totalLebar - lebarLabel - 1
        Dim nilaiTerpotong As String = If(nilai.Length > sisaLebar, nilai.Substring(0, sisaLebar), nilai)
        Return label.PadRight(lebarLabel) & " " & nilaiTerpotong
    End Function

#End Region

#Region "Blok Item Thermal"

    ' Satu fungsi item — pakaHeader mengontrol baris header kolom,
    ' pakaDiskon mengontrol kolom Disc dan posisi harga (51% vs 65%)
    Private Sub CetakItem(esc As PrinterEscPos, pakaHeader As Boolean, pakaDiskon As Boolean)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        Dim posQtyKanan As Integer = CInt(n * 0.11)
        Dim posSatuan As Integer = CInt(n * 0.11)

        If pakaDiskon Then
            Dim posHargaKanan As Integer = CInt(n * 0.51)
            Dim posDiscKanan As Integer = CInt(n * 0.7)
            Dim lebarQty As Integer = posQtyKanan
            Dim lebarHarga As Integer = posHargaKanan - posSatuan - 4
            Dim lebarDisc As Integer = posDiscKanan - posHargaKanan
            Dim lebarJumlah As Integer = n - posDiscKanan - 1
            Dim prefixHeader As Integer = lebarQty + 1 + 4

            If pakaHeader Then
                esc.CetakBaris(RataKiri("Nama Barang", prefixHeader) &
                               RataKanan("Harga", lebarHarga) &
                               RataKanan("Disc", lebarDisc) &
                               RataKanan("Jml", lebarJumlah), _cfg.EscUkuranIsi)
                esc.CetakGaris()
            End If

            For Each item As ItemNotaJual In Jual_DaftarItem
                esc.CetakBaris(item.NamaBarang, _cfg.EscUkuranIsi)
                esc.CetakBaris(
                    RataKanan(item.Qty.ToString("#,0.##", cultureIndonesia), lebarQty) &
                    " " & RataKiri(item.Satuan, 4) &
                    RataKanan(Rp(item.Harga), lebarHarga) &
                    RataKanan(Rp(item.TotalDiskon), lebarDisc) &
                    RataKanan(Rp(item.TotalHarga), lebarJumlah), _cfg.EscUkuranIsi)
                If Not String.IsNullOrEmpty(item.SerialNumber) Then
                    esc.CetakBaris("  SN: " & item.SerialNumber, _cfg.EscUkuranIsi)
                End If
                If _cfg.JarakBarisEsc > 0 Then esc.CetakBarisKosong(_cfg.JarakBarisEsc)
            Next
        Else
            Dim posHargaKanan As Integer = CInt(n * 0.65)
            Dim lebarQty As Integer = posQtyKanan
            Dim lebarHarga As Integer = posHargaKanan - posSatuan - 4
            Dim lebarJumlah As Integer = n - posHargaKanan - 1
            Dim prefixHeader As Integer = lebarQty + 1 + 4

            If pakaHeader Then
                esc.CetakBaris(RataKiri("Nama Barang", prefixHeader) &
                               RataKanan("Harga", lebarHarga) &
                               RataKanan("Jml", lebarJumlah), _cfg.EscUkuranIsi)
                esc.CetakGaris()
            End If

            For Each item As ItemNotaJual In Jual_DaftarItem
                esc.CetakBaris(item.NamaBarang, _cfg.EscUkuranIsi)
                esc.CetakBaris(
                    RataKanan(item.Qty.ToString("#,0.##", cultureIndonesia), lebarQty) &
                    " " & RataKiri(item.Satuan, 4) &
                    RataKanan(Rp(item.Harga), lebarHarga) &
                    RataKanan(Rp(item.TotalHarga), lebarJumlah), _cfg.EscUkuranIsi)
                If Not String.IsNullOrEmpty(item.SerialNumber) Then
                    esc.CetakBaris("  SN: " & item.SerialNumber, _cfg.EscUkuranIsi)
                End If
                If _cfg.JarakBarisEsc > 0 Then esc.CetakBarisKosong(_cfg.JarakBarisEsc)
            Next
        End If
        esc.CetakGaris()
    End Sub

    ' Alias lama — dipakai oleh kode dot matrix yang belum dimigrasi
    Private Sub CetakItemDenganDiskon(esc As PrinterEscPos)
        CetakItem(esc, pakaHeader:=True, pakaDiskon:=True)
    End Sub
    Private Sub CetakItemTanpaDiskon(esc As PrinterEscPos)
        CetakItem(esc, pakaHeader:=True, pakaDiskon:=False)
    End Sub

#End Region

#Region "Blok Total Thermal"

    ' Satu fungsi total — pakaDiskon mengontrol posisi label (51% vs 65%)
    ' dan apakah baris Diskon ditampilkan.
    ' Diskon, Pajak, Transfer, Jatuh Tempo tampil otomatis jika ada data.
    Private Sub CetakTotal(esc As PrinterEscPos, pakaDiskon As Boolean)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        Dim posLabel As Integer = If(pakaDiskon, CInt(n * 0.51), CInt(n * 0.65))
        Dim lebarKanan As Integer = n - posLabel
        Dim lebarNilai As Integer = CInt(lebarKanan * If(pakaDiskon, 0.45, 0.55))
        Dim lebarLbl As Integer = lebarKanan - lebarNilai

        ' Jika ada persen diskon/pajak — tampilkan Subtotal dulu
        Dim pakaPersen As Boolean = (Jual_DiskonPersen > 0 OrElse Jual_PajakPersen > 0)

        If pakaPersen Then
            esc.CetakBaris((JumlahItem & " item").PadRight(posLabel) &
                RataKanan("Subtotal :", lebarLbl) & RataKanan(Rp(Jual_TotalSebelumPajak), lebarNilai), _cfg.EscUkuranIsi)
        Else
            esc.CetakBaris((JumlahItem & " item").PadRight(posLabel) &
                RataKanan("Total :", lebarLbl) & RataKanan(Rp(Jual_Total), lebarNilai), _cfg.EscUkuranIsi)
        End If

        If pakaDiskon AndAlso Jual_Diskon <> 0 Then
            Dim lblDis As String = If(Jual_DiskonPersen > 0,
                "Diskon " & Jual_DiskonPersen.ToString("0.##") & "% :", "Diskon :")
            esc.CetakBaris("".PadRight(posLabel) &
                RataKanan(lblDis, lebarLbl) & RataKanan(Rp(Jual_Diskon), lebarNilai), _cfg.EscUkuranIsi)
        End If
        If Jual_Pajak <> 0 Then
            Dim lblPjk As String = If(Jual_PajakPersen > 0,
                "Pajak " & Jual_PajakPersen.ToString("0.##") & "% :", "Pajak :")
            esc.CetakBaris("".PadRight(posLabel) &
                RataKanan(lblPjk, lebarLbl) & RataKanan(Rp(Jual_Pajak), lebarNilai), _cfg.EscUkuranIsi)
        End If
        If Jual_BiayaKirim <> 0 Then
            esc.CetakBaris("".PadRight(posLabel) &
                RataKanan("Biaya Kirim :", lebarLbl) & RataKanan(Rp(Jual_BiayaKirim), lebarNilai), _cfg.EscUkuranIsi)
        End If
        If pakaPersen Then
            esc.CetakBaris("".PadRight(posLabel) &
                RataKanan("Total :", lebarLbl) & RataKanan(Rp(Jual_Total), lebarNilai), _cfg.EscUkuranIsi)
        End If
        If Jual_JudulNota <> "Nota Order" Then
            If Jual_NominalTransfer > 0 Then
                If Jual_Bayar > 0 Then
                    esc.CetakBaris("".PadRight(posLabel) &
                        RataKanan("Tunai (" & Jual_Penerima & ") :", lebarLbl) & RataKanan(Rp(Jual_Bayar), lebarNilai), _cfg.EscUkuranIsi)
                End If
                esc.CetakBaris("".PadRight(posLabel) &
                    RataKanan("Transfer (" & Jual_NamaAkunTransfer & ") :", lebarLbl) & RataKanan(Rp(Jual_NominalTransfer), lebarNilai), _cfg.EscUkuranIsi)
            Else
                esc.CetakBaris("".PadRight(posLabel) &
                    RataKanan("Bayar :", lebarLbl) & RataKanan(Rp(Jual_Bayar), lebarNilai), _cfg.EscUkuranIsi)
            End If
            esc.CetakBaris("".PadRight(posLabel) & New String("="c, lebarKanan), _cfg.EscUkuranIsi)
            esc.CetakBaris("".PadRight(posLabel) &
                RataKanan(Jual_LabelPembayaran, lebarLbl) & RataKanan(Rp(Jual_Kembali), lebarNilai), _cfg.EscUkuranIsi)
        End If
        If Jual_StatusTransaksi = "Belum Lunas" AndAlso Not String.IsNullOrEmpty(Jual_JatuhTempo) Then
            esc.CetakBaris("".PadRight(posLabel) &
                RataKanan("Jatuh Tempo :", lebarLbl) & RataKanan(Jual_JatuhTempo, lebarNilai), _cfg.EscUkuranIsi)
        End If
        esc.CetakGaris()
    End Sub

    ' Alias lama — dipakai kode dot matrix yang belum dimigrasi
    Private Sub CetakTotalDenganDiskon(esc As PrinterEscPos)
        CetakTotal(esc, pakaDiskon:=True)
    End Sub
    Private Sub CetakTotalTanpaDiskon(esc As PrinterEscPos)
        CetakTotal(esc, pakaDiskon:=False)
    End Sub
    Private Sub CetakTotalDenganPersen(esc As PrinterEscPos)
        CetakTotal(esc, pakaDiskon:=True)
    End Sub

#End Region

#Region "Blok Info Bank & Footer Thermal"

    Private Sub CetakInfoBank(esc As PrinterEscPos)
        If Jual_NominalTransfer <= 0 Then Exit Sub
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakBaris(FormatLabelNilai("Metode    ", ": " & Jual_Metode, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Bank      ", ": " & Jual_Bank & " - " & Jual_NamaRekening, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("No Rek    ", ": " & Jual_NoRekening, n), _cfg.EscUkuranIsi)
        If Not String.IsNullOrEmpty(Jual_NoReferensi) Then
            esc.CetakBaris(FormatLabelNilai("No Reff   ", ": " & Jual_NoReferensi, n), _cfg.EscUkuranIsi)
        End If
        esc.CetakGaris()
    End Sub

    Private Sub CetakFooter(esc As PrinterEscPos)
        If _cfg.TampilFooter1 Then
            For Each baris As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                esc.CetakTengah(baris, _cfg.EscUkuranFooter)
            Next
        End If
        If _cfg.TampilFooter2 Then
            For Each baris As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                esc.CetakTengah(baris, _cfg.EscUkuranFooter)
            Next
        End If
        If _cfg.TampilFooter3 Then
            For Each baris As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                esc.CetakTengah(baris, _cfg.EscUkuranFooter)
            Next
        End If
        esc.CetakBarisKosong(3)
        If _cfg.PotongOtomatis Then esc.PotongKertas()
        esc.Flush()
    End Sub

#End Region


#Region "Cetak Dot Matrix — ESC/P (Raw)"

    Public Sub CetakDotMatrix()
        Debug.WriteLine("[EscPosCetakJualThermalMatrik.CetakDotMatrix] Mulai cetak DOT MATRIX ESC/P")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakDotMatrix] NamaPrinter: {_cfgDot.NamaPrinter}")
        Debug.WriteLine($"[EscPosCetakJualThermalMatrik.CetakDotMatrix] ModelStruk: {_cfgDot.ModelStruk}")
        If String.IsNullOrEmpty(_cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For i As Integer = 1 To _cfgDot.JumlahCetak
            RawPrinterHelper.KirimKePrinter(_cfgDot.NamaPrinter, BangunEscP())
        Next
    End Sub

    Private Function BangunEscP() As Byte()
        Dim baris As New List(Of String)
        KumpulkanBarisDotMatrix(baris)
        Dim sb As New System.Text.StringBuilder()
        sb.Append(Chr(27) & "@")
        If _cfgDot.BatasKiri > 0 Then sb.Append(Chr(27) & "l" & Chr(_cfgDot.BatasKiri))

        Dim isBesar As Boolean = False
        Dim isBold As Boolean = False
        Dim n As Integer = _cfgDot.LebarKertas

        For Each b As String In baris
            Dim wantBesar As Boolean = False
            Dim wantBold As Boolean = False

            ' Parse ~B~ or ~N~ size tag
            If b.StartsWith("~B~") Then
                wantBesar = True
                b = b.Substring(3)
                n = _cfgDot.LebarKertas \ 2
            ElseIf b.StartsWith("~N~") Then
                wantBesar = False
                b = b.Substring(3)
                n = _cfgDot.LebarKertas
            End If

            ' Parse optional ~BOLD~ tag
            If b.StartsWith("~BOLD~") Then
                wantBold = True
                b = b.Substring(6)
            End If

            ' Apply double-width toggle
            If wantBesar AndAlso Not isBesar Then
                sb.Append(Chr(27) & "W" & Chr(1))
                isBesar = True
            ElseIf Not wantBesar AndAlso isBesar Then
                sb.Append(Chr(27) & "W" & Chr(0))
                isBesar = False
            End If

            ' Apply bold toggle  (ESC E 1 = bold on,  ESC E 0 = bold off)
            If wantBold AndAlso Not isBold Then
                sb.Append(Chr(27) & "E" & Chr(1))
                isBold = True
            ElseIf Not wantBold AndAlso isBold Then
                sb.Append(Chr(27) & "E" & Chr(0))
                isBold = False
            End If

            If b.Contains(Chr(1)) Then
                Dim bagian As String() = b.Split(Chr(1))
                sb.Append(KiriKanan(bagian(0), bagian(1), n) & Chr(13) & Chr(10))
            Else
                sb.Append(b & Chr(13) & Chr(10))
            End If
        Next

        If isBesar Then sb.Append(Chr(27) & "W" & Chr(0)) ' Reset double-width
        If isBold Then sb.Append(Chr(27) & "E" & Chr(0))  ' Reset bold
        Return System.Text.Encoding.GetEncoding(437).GetBytes(sb.ToString())
    End Function

#End Region

#Region "Dot Matrix — Dispatcher & Helper Kolom"

    Private Sub KumpulkanBarisDotMatrix(baris As List(Of String))
        Select Case _cfgDot.ModelStruk
            Case "Model 2 Tanpa Diskon" : KumpulkanDotModel2(baris)
            Case "Model 3 Dengan Sales" : KumpulkanDotModel3(baris)
            Case "Model 4 Dengan Transfer" : KumpulkanDotModel4(baris)
            Case "Model 5 Dengan Hutang" : KumpulkanDotModel5(baris)
            Case "Model 6 Dengan Pemisah" : KumpulkanDotModel6(baris)
            Case Else : KumpulkanDotModel1(baris)
        End Select
    End Sub

    ' Layout: No(4%) | Nama(sisa) | Qty(5) | Sat(6) | Harga(12) | Disc(7) | Jumlah(22%)
    Private Sub HitungKolomDot(n As Integer,
                                ByRef lebarNo As Integer, ByRef lebarNama As Integer,
                                ByRef lebarQty As Integer, ByRef lebarSat As Integer,
                                ByRef lebarHarga As Integer, ByRef lebarDisc As Integer,
                                ByRef lebarJumlah As Integer, ByRef posHarga As Integer,
                                ByRef lebarLabelTotal As Integer, ByRef lebarNilaiTotal As Integer)
        lebarJumlah = CInt(n * 0.22)
        lebarDisc = 7
        lebarHarga = 12
        lebarSat = 6
        lebarQty = 5
        lebarNo = CInt(n * 0.04)
        lebarNama = n - lebarNo - lebarQty - 1 - lebarSat - lebarHarga - lebarDisc - lebarJumlah
        If lebarNama < 10 Then lebarNama = 10
        posHarga = lebarNo + lebarNama + lebarQty + 1 + lebarSat
        lebarLabelTotal = lebarDisc : lebarNilaiTotal = lebarJumlah
    End Sub

    Private Sub HitungKolomDotTanpaDiskon(n As Integer,
                                           ByRef lebarNo As Integer, ByRef lebarNama As Integer,
                                           ByRef lebarQty As Integer, ByRef lebarSat As Integer,
                                           ByRef lebarHarga As Integer, ByRef lebarJumlah As Integer,
                                           ByRef posHarga As Integer,
                                           ByRef lebarLabelTotal As Integer, ByRef lebarNilaiTotal As Integer)
        lebarJumlah = CInt(n * 0.22)
        lebarHarga = 14
        lebarSat = 6
        lebarQty = 5
        lebarNo = CInt(n * 0.04)
        lebarNama = n - lebarNo - lebarQty - 1 - lebarSat - lebarHarga - lebarJumlah
        If lebarNama < 10 Then lebarNama = 10
        posHarga = lebarNo + lebarNama + lebarQty + 1 + lebarSat
        lebarLabelTotal = CInt(lebarJumlah * 0.55) : lebarNilaiTotal = lebarJumlah - lebarLabelTotal
    End Sub

#End Region

    Private ReadOnly Property DotPJudul As String
        Get
            Dim s As String = _cfgDot.EscUkuranJudul
            Dim tag As String = If(s.Contains("Besar"), "~B~", "~N~")
            If s.Contains("Bold") Then tag &= "~BOLD~"
            Return tag
        End Get
    End Property
    Private ReadOnly Property DotNJudul As Integer
        Get
            Return If(DotPJudul = "~B~", _cfgDot.LebarKertas \ 2, _cfgDot.LebarKertas)
        End Get
    End Property
    Private ReadOnly Property DotPKet As String
        Get
            Dim s As String = _cfgDot.EscUkuranKeterangan
            Dim tag As String = If(s.Contains("Besar"), "~B~", "~N~")
            If s.Contains("Bold") Then tag &= "~BOLD~"
            Return tag
        End Get
    End Property
    Private ReadOnly Property DotNKet As Integer
        Get
            Return If(DotPKet = "~B~", _cfgDot.LebarKertas \ 2, _cfgDot.LebarKertas)
        End Get
    End Property
    Private ReadOnly Property DotPIsi As String
        Get
            Dim s As String = _cfgDot.EscUkuranIsi
            Dim tag As String = If(s.Contains("Besar"), "~B~", "~N~")
            If s.Contains("Bold") Then tag &= "~BOLD~"
            Return tag
        End Get
    End Property
    Private ReadOnly Property DotNIsi As Integer
        Get
            Return If(DotPIsi = "~B~", _cfgDot.LebarKertas \ 2, _cfgDot.LebarKertas)
        End Get
    End Property
    Private ReadOnly Property DotPFooter As String
        Get
            Dim s As String = _cfgDot.EscUkuranFooter
            Dim tag As String = If(s.Contains("Besar"), "~B~", "~N~")
            If s.Contains("Bold") Then tag &= "~BOLD~"
            Return tag
        End Get
    End Property
    Private ReadOnly Property DotNFooter As Integer
        Get
            Return If(DotPFooter = "~B~", _cfgDot.LebarKertas \ 2, _cfgDot.LebarKertas)
        End Get
    End Property
#Region "Dot Matrix — Blok Header, Item, Total, Footer"

    Private Sub TambahHeaderDot(baris As List(Of String))
        baris.Add(DotPJudul & KiriKanan(NAMA_PERUSAHAAN, "N O T A  P E N J U A L A N", DotNJudul))
        baris.Add(DotPKet & KiriKanan(ALAMAT_PERUSAHAAN, "Trx : " & Jual_NoFaktur, DotNKet))
        baris.Add(DotPKet & KiriKanan(KOTA_PERUSAHAAN, "Tgl : " & Jual_Tanggal.ToString("dd-MM-yyyy HH:mm"), DotNKet))
        baris.Add(DotPKet & KiriKanan(KONTAK_PERUSAHAAN, "Pel : " & Jual_JenisPelanggan & " " & Jual_NamaPelanggan, DotNKet))
        If Not String.IsNullOrEmpty(Jual_NoSO) Then
            baris.Add(DotPKet & "Ref. SO : " & Jual_NoSO)
        End If
        If Jual_JudulNota = "Nota Order" Then
            baris.Add(DotPKet & New String("*"c, DotNKet))
            baris.Add(DotPKet & "** NOTA PESANAN / SALES ORDER **".PadLeft((DotNKet + 32) \ 2).PadRight(DotNKet))
            baris.Add(DotPKet & New String("*"c, DotNKet))
        End If
        baris.Add(DotPKet & New String("-"c, DotNKet))
    End Sub

    Private Sub TambahItemDenganDiskon(baris As List(Of String),
                                        lebarNo As Integer, lebarNama As Integer,
                                        lebarQty As Integer, lebarSat As Integer,
                                        lebarHarga As Integer, lebarDisc As Integer,
                                        lebarJumlah As Integer)
        baris.Add(DotPIsi & RataKiri("No", lebarNo) & RataKiri("Barang", lebarNama) &
                  RataKanan("Qty", lebarQty) & " " & RataKiri("Sat", lebarSat - 1) &
                  RataKanan("Harga", lebarHarga) & RataKanan("Disc", lebarDisc) &
                  RataKanan("Jumlah", lebarJumlah))
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
        Dim nomor As Integer = 1
        For Each item As ItemNotaJual In Jual_DaftarItem
            Dim namaList As List(Of String) = WrapTeks(item.NamaBarang, lebarNama)
            baris.Add(
                RataKiri(nomor.ToString() & ".", lebarNo) &
                RataKiri(namaList(0), lebarNama) &
                RataKanan(item.Qty.ToString("#,0", cultureIndonesia), lebarQty) &
                " " & RataKiri(item.Satuan, lebarSat - 1) &
                RataKanan(Rp(item.Harga), lebarHarga) &
                RataKanan(Rp(item.TotalDiskon), lebarDisc) &
                RataKanan(Rp(item.TotalHarga), lebarJumlah))
            For k As Integer = 1 To namaList.Count - 1
                baris.Add(DotPIsi & "".PadRight(lebarNo) & RataKiri(namaList(k), lebarNama))
            Next
            If Not String.IsNullOrEmpty(item.SerialNumber) Then
                baris.Add(DotPIsi & "".PadRight(lebarNo) & "SN: " & item.SerialNumber)
            End If
            For j As Integer = 1 To _cfgDot.JarakBaris : baris.Add(DotPIsi & "") : Next
            nomor += 1
        Next
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
    End Sub

    Private Sub TambahItemTanpaDiskon(baris As List(Of String),
                                       lebarNo As Integer, lebarNama As Integer,
                                       lebarQty As Integer, lebarSat As Integer,
                                       lebarHarga As Integer, lebarJumlah As Integer)
        baris.Add(DotPIsi & RataKiri("No", lebarNo) & RataKiri("Barang", lebarNama) &
                  RataKanan("Qty", lebarQty) & " " & RataKiri("Sat", lebarSat - 1) &
                  RataKanan("Harga", lebarHarga) & RataKanan("Jumlah", lebarJumlah))
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
        Dim nomor As Integer = 1
        For Each item As ItemNotaJual In Jual_DaftarItem
            Dim namaList As List(Of String) = WrapTeks(item.NamaBarang, lebarNama)
            baris.Add(
                RataKiri(nomor.ToString() & ".", lebarNo) &
                RataKiri(namaList(0), lebarNama) &
                RataKanan(item.Qty.ToString("#,0", cultureIndonesia), lebarQty) &
                " " & RataKiri(item.Satuan, lebarSat - 1) &
                RataKanan(Rp(item.Harga), lebarHarga) &
                RataKanan(Rp(item.TotalHarga), lebarJumlah))
            For k As Integer = 1 To namaList.Count - 1
                baris.Add(DotPIsi & "".PadRight(lebarNo) & RataKiri(namaList(k), lebarNama))
            Next
            If Not String.IsNullOrEmpty(item.SerialNumber) Then
                baris.Add(DotPIsi & "".PadRight(lebarNo) & "SN: " & item.SerialNumber)
            End If
            For j As Integer = 1 To _cfgDot.JarakBaris : baris.Add(DotPIsi & "") : Next
            nomor += 1
        Next
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
    End Sub

    Private Function WrapTeks(teks As String, lebarMaks As Integer) As List(Of String)
        Dim hasil As New List(Of String)
        If teks.Length <= lebarMaks Then
            hasil.Add(teks)
            Return hasil
        End If
        Dim kata As String() = teks.Split(" "c)
        Dim sb As New System.Text.StringBuilder()
        For Each k As String In kata
            If sb.Length + k.Length + (If(sb.Length > 0, 1, 0)) > lebarMaks Then
                If sb.Length > 0 Then
                    hasil.Add(sb.ToString())
                    sb.Clear()
                End If
                If k.Length > lebarMaks Then
                    hasil.Add(k.Substring(0, lebarMaks))
                    k = k.Substring(lebarMaks)
                End If
            End If
            If sb.Length > 0 Then sb.Append(" ")
            sb.Append(k)
        Next
        If sb.Length > 0 Then hasil.Add(sb.ToString())
        Return hasil
    End Function

    ' Kolom kiri (posHarga kar): terbilang + tanda tangan
    ' Kolom kanan (posHarga..n): baris-baris total
    Private Sub TambahTotalDenganTtd(baris As List(Of String),
                                      posHarga As Integer,
                                      lebarLabelTotal As Integer, lebarNilaiTotal As Integer,
                                      denganDiskon As Boolean)
        Dim ttd As Integer = posHarga
        Dim terbilangBaris As String() = PecahTeks(Terbilang(Jual_Total), ttd - 1)

        Dim totalBaris As New List(Of String) From {
            RataKanan("Sub Total :", lebarLabelTotal) & RataKanan(Rp(Jual_TotalSebelumPajak), lebarNilaiTotal)
        }
        If denganDiskon AndAlso Jual_Diskon <> 0 Then
            totalBaris.Add(RataKanan("Diskon :", lebarLabelTotal) & RataKanan(Rp(Jual_Diskon), lebarNilaiTotal))
        End If
        If Jual_Pajak <> 0 Then
            totalBaris.Add(RataKanan("Pajak :", lebarLabelTotal) & RataKanan(Rp(Jual_Pajak), lebarNilaiTotal))
        End If
        If Jual_BiayaKirim <> 0 Then
            totalBaris.Add(RataKanan("Biaya Kirim :", lebarLabelTotal) & RataKanan(Rp(Jual_BiayaKirim), lebarNilaiTotal))
        End If
        totalBaris.Add(RataKanan("Total :", lebarLabelTotal) & RataKanan(Rp(Jual_Total), lebarNilaiTotal))
        ' Jika split bayar: Tunai + Transfer langsung (tanpa baris Bayar)
        If Jual_JudulNota <> "Nota Order" Then
            If Jual_NominalTransfer > 0 Then
                If Jual_Bayar > 0 Then
                    totalBaris.Add(RataKanan("Tunai (" & Jual_Penerima & ") :", lebarLabelTotal) & RataKanan(Rp(Jual_Bayar), lebarNilaiTotal))
                End If
                totalBaris.Add(RataKanan("Transfer (" & Jual_NamaAkunTransfer & ") :", lebarLabelTotal) & RataKanan(Rp(Jual_NominalTransfer), lebarNilaiTotal))
            Else
                totalBaris.Add(RataKanan("Bayar :", lebarLabelTotal) & RataKanan(Rp(Jual_Bayar), lebarNilaiTotal))
            End If
            totalBaris.Add(RataKanan(Jual_LabelPembayaran, lebarLabelTotal) & RataKanan(Rp(Jual_Kembali), lebarNilaiTotal))
        End If
        If Jual_StatusTransaksi = "Belum Lunas" AndAlso Not String.IsNullOrEmpty(Jual_JatuhTempo) Then
            totalBaris.Add(RataKanan("Jatuh Tempo :", lebarLabelTotal) & RataKanan(Jual_JatuhTempo, lebarNilaiTotal))
        End If

        Dim maxBaris As Integer = Math.Max(terbilangBaris.Length, totalBaris.Count)
        For i As Integer = 0 To maxBaris - 1
            Dim kiri As String = If(i < terbilangBaris.Length, terbilangBaris(i), "")
            Dim kanan As String = If(i < totalBaris.Count, totalBaris(i), "")
            baris.Add(DotPIsi & RataKiri(kiri, ttd) & kanan)
        Next

        Dim lebarKol As Integer = Math.Max(8, ttd \ 3)
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
        baris.Add(DotPIsi & "Hormat kami".PadRight(lebarKol) & "  " &
                  "Penerima".PadRight(lebarKol) & "  " & "Kasir")
        baris.Add(DotPIsi & "")
        baris.Add(DotPIsi & New String("."c, lebarKol - 2).PadRight(lebarKol) & "  " &
                  New String("."c, lebarKol - 2).PadRight(lebarKol) & "  " & Jual_IdUser)
    End Sub

    Private Function PecahTeks(teks As String, lebarMaks As Integer) As String()
        If lebarMaks < 1 Then Return New String() {teks}
        Dim hasil As New List(Of String)
        Dim kata As String() = teks.Split(" "c)
        Dim sb As New System.Text.StringBuilder()
        For Each k As String In kata
            If sb.Length + k.Length + 1 > lebarMaks AndAlso sb.Length > 0 Then
                hasil.Add(sb.ToString().TrimEnd())
                sb.Clear()
            End If
            If sb.Length > 0 Then sb.Append(" ")
            sb.Append(k)
        Next
        If sb.Length > 0 Then hasil.Add(sb.ToString())
        Return hasil.ToArray()
    End Function

    Private Sub TambahFooterDot(baris As List(Of String))
        baris.Add(DotPFooter & "")
        If _cfgDot.TampilFooter1 OrElse _cfgDot.TampilFooter3 Then
            Dim baris1 As String() = If(_cfgDot.TampilFooter1,
                FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None), New String() {""})
            Dim baris3 As String() = If(_cfgDot.TampilFooter3,
                FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None), New String() {""})
            Dim maxBaris As Integer = Math.Max(baris1.Length, baris3.Length)
            For i As Integer = 0 To maxBaris - 1
                Dim kiri As String = If(i < baris1.Length, baris1(i), "")
                Dim kanan As String = If(i < baris3.Length, baris3(i), "")
                baris.Add(If(String.IsNullOrEmpty(kanan), kiri, kiri & Chr(1) & kanan))
            Next
        End If
        If _cfgDot.TampilFooter2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                baris.Add(b)
            Next
        End If
    End Sub

#End Region

#Region "Dot Matrix — Model 1-6"

    Private Sub KumpulkanDotModel1(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDot(DotNIsi, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal)
        TambahHeaderDot(baris)
        TambahItemDenganDiskon(baris, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah)
        TambahTotalDenganTtd(baris, posHarga, lebarLabelTotal, lebarNilaiTotal, denganDiskon:=True)
        TambahFooterDot(baris)
    End Sub

    Private Sub KumpulkanDotModel2(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDotTanpaDiskon(DotNIsi, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal)
        TambahHeaderDot(baris)
        TambahItemTanpaDiskon(baris, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarJumlah)
        TambahTotalDenganTtd(baris, posHarga, lebarLabelTotal, lebarNilaiTotal, denganDiskon:=False)
        TambahFooterDot(baris)
    End Sub

    Private Sub KumpulkanDotModel3(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDot(DotNIsi, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal)
        baris.Add(DotPJudul & KiriKanan(NAMA_PERUSAHAAN, "N O T A  P E N J U A L A N", DotNJudul))
        baris.Add(DotPKet & KiriKanan(ALAMAT_PERUSAHAAN, "Trx : " & Jual_NoFaktur, DotNKet))
        baris.Add(DotPKet & KiriKanan(KOTA_PERUSAHAAN, "Tgl : " & Jual_Tanggal.ToString("dd-MM-yyyy HH:mm"), DotNKet))
        baris.Add(DotPKet & KiriKanan(KONTAK_PERUSAHAAN, "Pel : " & Jual_JenisPelanggan & " " & Jual_NamaPelanggan, DotNKet))
        If Not String.IsNullOrEmpty(Jual_NamaSales) Then
            baris.Add(DotPKet & KiriKanan("Sales : " & Jual_NamaSales,
                                If(Not String.IsNullOrEmpty(Jual_LokasiBarang), "Lok: " & Jual_LokasiBarang, ""), DotNKet))
        End If
        baris.Add(DotPKet & New String("-"c, DotNKet))
        TambahItemDenganDiskon(baris, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah)
        TambahTotalDenganTtd(baris, posHarga, lebarLabelTotal, lebarNilaiTotal, denganDiskon:=True)
        TambahFooterDot(baris)
    End Sub

    Private Sub KumpulkanDotModel4(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDot(DotNIsi, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal)
        TambahHeaderDot(baris)
        TambahItemDenganDiskon(baris, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah)
        TambahTotalDenganTtd(baris, posHarga, lebarLabelTotal, lebarNilaiTotal, denganDiskon:=True)
        If Jual_NominalTransfer > 0 Then
            baris.Add(DotPIsi & New String("-"c, DotNIsi))
            baris.Add(DotPIsi & KiriKanan("Metode  : " & Jual_Metode, "", DotNIsi))
            baris.Add(DotPIsi & KiriKanan("Bank    : " & Jual_Bank & " - " & Jual_NamaRekening, "No: " & Jual_NoRekening, DotNIsi))
            If Not String.IsNullOrEmpty(Jual_NoReferensi) Then
                baris.Add("No Ref
                " & Jual_NoReferensi)
            End If
        End If
        TambahFooterDot(baris)
    End Sub

    Private Sub KumpulkanDotModel5(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDot(DotNIsi, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal)
        TambahHeaderDot(baris)
        TambahItemDenganDiskon(baris, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah)
        TambahTotalDenganTtd(baris, posHarga, lebarLabelTotal, lebarNilaiTotal, denganDiskon:=True)
        If Jual_AdaDataHutang Then
            baris.Add(DotPIsi & New String("-"c, DotNIsi))
            baris.Add(DotPIsi & KiriKanan("Sisa Hutang :", Rp(Jual_HutangAkhir), DotNIsi))
        End If
        TambahFooterDot(baris)
    End Sub

    Private Sub KumpulkanDotModel6(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal As Integer
        HitungKolomDot(DotNIsi, lebarNo, lebarNama, lebarQty, lebarSat, lebarHarga, lebarDisc, lebarJumlah, posHarga, lebarLabelTotal, lebarNilaiTotal)
        TambahHeaderDot(baris)
        baris.Add(DotPIsi & RataKiri("No", lebarNo) & RataKiri("Barang", lebarNama) &
                  RataKanan("Qty", lebarQty) & " " & RataKiri("Sat", lebarSat - 1) &
                  RataKanan("Harga", lebarHarga) & RataKanan("Disc", lebarDisc) &
                  RataKanan("Jumlah", lebarJumlah))
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
        Dim garisTipis As String =
            "".PadRight(lebarNo + lebarNama + lebarQty + lebarSat) &
            New String("·"c, lebarHarga + lebarDisc + lebarJumlah)
        Dim nomor As Integer = 1
        For Each item As ItemNotaJual In Jual_DaftarItem
            baris.Add(DotPIsi & RataKiri(nomor.ToString() & ".", lebarNo) & RataKiri(item.NamaBarang, lebarNama) &
                      RataKanan(item.Qty.ToString("#,0", cultureIndonesia), lebarQty) &
                      " " & RataKiri(item.Satuan, lebarSat - 1) &
                      RataKanan(Rp(item.Harga), lebarHarga) & RataKanan(Rp(item.TotalDiskon), lebarDisc) &
                      RataKanan(Rp(item.TotalHarga), lebarJumlah))
            If Not String.IsNullOrEmpty(item.SerialNumber) Then
                baris.Add(DotPIsi & "".PadRight(lebarNo) & "SN: " & item.SerialNumber)
            End If
            For j As Integer = 1 To _cfgDot.JarakBaris : baris.Add(DotPIsi & "") : Next
            If nomor Mod 2 = 0 AndAlso nomor < Jual_DaftarItem.Count Then baris.Add(garisTipis)
            nomor += 1
        Next
        baris.Add(DotPIsi & New String("-"c, DotNIsi))
        TambahTotalDenganTtd(baris, posHarga, lebarLabelTotal, lebarNilaiTotal, denganDiskon:=True)
        TambahFooterDot(baris)
    End Sub

#End Region

End Class

