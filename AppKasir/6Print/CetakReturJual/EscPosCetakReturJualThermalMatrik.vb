' ================================================================
' EscPosCetakReturJualThermalMatrik
' Cetak nota retur penjualan via ESC/POS (Thermal & Dot Matrix).
'
' Cara pakai:
'   Dim cetak As New EscPosCetakReturJualThermalMatrik("ReturJual")
'   cetak.CetakThermal()    ' atau cetak.CetakDotMatrix()
' ================================================================
Public Class EscPosCetakReturJualThermalMatrik

#Region "Field & Konstruktor"

    Private ReadOnly _cfg As KonfigurasiThermal
    Private ReadOnly _transaksi As String
    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    Public Sub New(transaksi As String)
        _transaksi = transaksi
        _cfg = New KonfigurasiThermal(transaksi)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private ReadOnly Property JumlahItem As Integer
        Get
            Return ReturJual_DaftarItem.Count
        End Get
    End Property

    Private Function Rp(nilai As Decimal) As String
        Return ReturJualRp(nilai)
    End Function

#End Region

#Region "Helper Tata Letak Kolom ESC/POS"

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

#Region "Cetak Thermal"

    Public Sub CetakThermal()
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If _cfg.ModeCetak = "GDI+ (Windows Print)" Then
            Dim cetak As New GdiCetakReturJualThermalMatrik()
            cetak.Cetak()
            Exit Sub
        End If

        For i As Integer = 1 To _cfg.JumlahCetak
            Dim esc As PrinterEscPos
            If _cfg.TipeKoneksi = "Network / WiFi (IP)" AndAlso Not String.IsNullOrEmpty(_cfg.IpAddress) Then
                esc = New PrinterEscPos(_cfg.IpAddress, _cfg.NetworkPort, _cfg.LebarKertas, _cfg.BatasKiri)
            Else
                esc = New PrinterEscPos(_cfg.NamaPrinter, _cfg.LebarKertas, _cfg.BatasKiri)
            End If

            Select Case _cfg.ModelStruk
                Case "Model 2 Tanpa Diskon"  : CetakModel2(esc)
                Case "Model 3 Tanpa Header"  : CetakModel3(esc)
                Case Else                    : CetakModel1(esc)
            End Select
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir(_transaksi)
    End Sub

#End Region

#Region "Model Thermal"

    Private Sub CetakModel1(esc As PrinterEscPos)
        CetakHeaderToko(esc)
        CetakInfoTransaksi(esc)
        CetakItemDenganDiskon(esc)
        CetakTotal(esc)
        CetakFooter(esc)
    End Sub

    Private Sub CetakModel2(esc As PrinterEscPos)
        CetakHeaderToko(esc)
        CetakInfoTransaksi(esc)
        CetakItemTanpaDiskon(esc)
        CetakTotal(esc)
        CetakFooter(esc)
    End Sub

    Private Sub CetakModel3(esc As PrinterEscPos)
        CetakHeaderToko(esc)
        CetakInfoTransaksiSingkat(esc)
        CetakItemTanpaDiskon(esc)
        CetakTotal(esc)
        CetakFooter(esc)
    End Sub

    Private Sub CetakModel4(esc As PrinterEscPos)
        ' Alias lama — logo dikontrol via _cfg.TampilLogo
        CetakHeaderToko(esc)
        CetakInfoTransaksi(esc)
        CetakItemDenganDiskon(esc)
        CetakTotal(esc)
        CetakFooter(esc)
    End Sub

#End Region

#Region "Blok Header & Info Transaksi Thermal"

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
    End Sub

    Private Sub CetakInfoTransaksi(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakGaris()
        esc.CetakBaris(FormatLabelNilai("Nota Retur", ": " & ReturJual_NoRetur, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FormatLabelNilai("Tanggal  ", ": " & ReturJual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FormatLabelNilai("Kasir    ", ": " & ReturJual_IdUser & " - " & ReturJual_IdKomputer, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FormatLabelNilai("Pelanggan", ": " & ReturJual_JenisPelanggan & " - " & ReturJual_NamaPelanggan, n), _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
        esc.CetakTengah("RETUR PENJUALAN", _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
    End Sub

    Private Sub CetakInfoTransaksiSingkat(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakGaris()
        esc.CetakBaris(FormatLabelNilai("Nota Retur", ": " & ReturJual_NoRetur, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FormatLabelNilai("Tgl      ", ": " & ReturJual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FormatLabelNilai("Kasir    ", ": " & ReturJual_IdUser & " - " & ReturJual_IdKomputer, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FormatLabelNilai("Pel      ", ": " & ReturJual_JenisPelanggan & " - " & ReturJual_NamaPelanggan, n), _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
        esc.CetakTengah("RETUR PENJUALAN", _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
    End Sub

    Private Function FormatLabelNilai(label As String, nilai As String, totalLebar As Integer) As String
        Const lebarLabel As Integer = 9
        Dim sisaLebar As Integer = totalLebar - lebarLabel - 1
        Dim nilaiTerpotong As String = If(nilai.Length > sisaLebar, nilai.Substring(0, sisaLebar), nilai)
        Return label.PadRight(lebarLabel) & " " & nilaiTerpotong
    End Function

#End Region

#Region "Blok Item Thermal"

    Private Sub CetakItemDenganDiskon(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        Dim posQtyKanan As Integer = CInt(n * 0.11)
        Dim posSatuan As Integer = CInt(n * 0.11)
        Dim posHargaKanan As Integer = CInt(n * 0.51)
        Dim posDiscKanan As Integer = CInt(n * 0.7)

        Dim lebarQty As Integer = posQtyKanan
        Dim lebarSatuan As Integer = posSatuan + 4
        Dim lebarHarga As Integer = posHargaKanan - posSatuan - 4
        Dim lebarDisc As Integer = posDiscKanan - posHargaKanan
        Dim lebarJumlah As Integer = n - posDiscKanan - 1

        Dim prefixHeader As Integer = lebarQty + 1 + 4
        esc.CetakBaris(RataKiri("Nama Barang", prefixHeader) &
                       RataKanan("Harga", lebarHarga) &
                       RataKanan("Disc", lebarDisc) &
                       RataKanan("Jml", lebarJumlah), _cfg.EscUkuranIsi)
        esc.CetakGaris()

        For Each item As ItemNotaReturJual In ReturJual_DaftarItem
            esc.CetakBaris(item.NamaBarang, _cfg.EscUkuranIsi)
            esc.CetakBaris(
                RataKanan(item.Qty.ToString("#,0.##", cultureIndonesia), lebarQty) &
                " " & RataKiri(item.Satuan, 4) &
                RataKanan(Rp(item.Harga), lebarHarga) &
                RataKanan(Rp(item.TotalDiskon), lebarDisc) &
                RataKanan(Rp(item.TotalHarga), lebarJumlah), _cfg.EscUkuranIsi)
            If _cfg.JarakBarisEsc > 0 Then esc.CetakBarisKosong(_cfg.JarakBarisEsc)
        Next
        esc.CetakGaris()
    End Sub

    Private Sub CetakItemTanpaDiskon(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        Dim posQtyKanan As Integer = CInt(n * 0.11)
        Dim posSatuan As Integer = CInt(n * 0.11)
        Dim posHargaKanan As Integer = CInt(n * 0.65)

        Dim lebarQty As Integer = posQtyKanan
        Dim lebarHarga As Integer = posHargaKanan - posSatuan - 4
        Dim lebarJumlah As Integer = n - posHargaKanan - 1

        Dim prefixHeader As Integer = lebarQty + 1 + 4
        esc.CetakBaris(RataKiri("Nama Barang", prefixHeader) &
                       RataKanan("Harga", lebarHarga) &
                       RataKanan("Jml", lebarJumlah), _cfg.EscUkuranIsi)
        esc.CetakGaris()

        For Each item As ItemNotaReturJual In ReturJual_DaftarItem
            esc.CetakBaris(item.NamaBarang, _cfg.EscUkuranIsi)
            esc.CetakBaris(
                RataKanan(item.Qty.ToString("#,0.##", cultureIndonesia), lebarQty) &
                " " & RataKiri(item.Satuan, 4) &
                RataKanan(Rp(item.Harga), lebarHarga) &
                RataKanan(Rp(item.TotalHarga), lebarJumlah), _cfg.EscUkuranIsi)
            If _cfg.JarakBarisEsc > 0 Then esc.CetakBarisKosong(_cfg.JarakBarisEsc)
        Next
        esc.CetakGaris()
    End Sub

#End Region

#Region "Blok Total Thermal"

    Private Sub CetakTotal(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        Dim posLabel As Integer = CInt(n * 0.51)
        Dim lebarKanan As Integer = n - posLabel
        Dim lebarNilai As Integer = CInt(lebarKanan * 0.45)
        Dim lebarLbl As Integer = lebarKanan - lebarNilai

        esc.CetakBaris((JumlahItem & " item").PadRight(posLabel) &
            RataKanan("Total :", lebarLbl) & RataKanan(Rp(ReturJual_Total), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()
    End Sub

#End Region

#Region "Blok Footer Thermal"

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
        Dim lebar As Integer = n
        For Each b As String In baris
            Dim wantBesar As Boolean = False
            Dim wantBold As Boolean = False
            If b.StartsWith("~B~") Then : wantBesar = True : b = b.Substring(3) : lebar = n \ 2
            ElseIf b.StartsWith("~N~") Then : wantBesar = False : b = b.Substring(3) : lebar = n : End If
            If b.StartsWith("~BOLD~") Then : wantBold = True : b = b.Substring(6) : End If
            If wantBesar AndAlso Not isBesar Then : sb.Append(Chr(27) & "W" & Chr(1)) : isBesar = True
            ElseIf Not wantBesar AndAlso isBesar Then : sb.Append(Chr(27) & "W" & Chr(0)) : isBesar = False : End If
            If wantBold AndAlso Not isBold Then : sb.Append(Chr(27) & "E" & Chr(1)) : isBold = True
            ElseIf Not wantBold AndAlso isBold Then : sb.Append(Chr(27) & "E" & Chr(0)) : isBold = False : End If
            If b.Contains(Chr(1)) Then
                Dim bagian As String() = b.Split(Chr(1))
                sb.Append(KiriKanan(bagian(0), bagian(1), lebar) & Chr(13) & Chr(10))
            Else
                sb.Append(b & Chr(13) & Chr(10))
            End If
        Next
        If isBesar Then sb.Append(Chr(27) & "W" & Chr(0))
        If isBold Then sb.Append(Chr(27) & "E" & Chr(0))
        Return System.Text.Encoding.GetEncoding(437).GetBytes(sb.ToString())
    End Function

    Private Sub KumpulkanBarisDotMatrix(baris As List(Of String))
        Dim n As Integer = _cfgDot.LebarKertas
        Dim garis As String = New String("-"c, n)
        Dim pJ As String = If(_cfgDot.EscUkuranJudul.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranJudul.Contains("Bold") Then pJ &= "~BOLD~"
        Dim nJ As Integer = If(pJ.Contains("~B~"), n \ 2, n)
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim nK As Integer = If(pK.Contains("~B~"), n \ 2, n)
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"
        Dim nI As Integer = If(pI.Contains("~B~"), n \ 2, n)
        Dim pF As String = If(_cfgDot.EscUkuranFooter.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranFooter.Contains("Bold") Then pF &= "~BOLD~"

        ' Header
        baris.Add(pJ & NAMA_PERUSAHAAN.PadLeft((nJ + NAMA_PERUSAHAAN.Length) \ 2))
        baris.Add(pK & ALAMAT_PERUSAHAAN.PadLeft((nK + ALAMAT_PERUSAHAAN.Length) \ 2))
        baris.Add(pK & KOTA_PERUSAHAAN.PadLeft((nK + KOTA_PERUSAHAAN.Length) \ 2))
        baris.Add(pI & garis)

        ' Info transaksi
        baris.Add(pI & FormatLabelNilai("Nota Retur", ": " & ReturJual_NoRetur, nI))
        baris.Add(pI & FormatLabelNilai("Tanggal  ", ": " & ReturJual_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), nI))
        baris.Add(pI & FormatLabelNilai("Kasir    ", ": " & ReturJual_IdUser & " - " & ReturJual_IdKomputer, nI))
        baris.Add(pI & FormatLabelNilai("Pelanggan", ": " & ReturJual_JenisPelanggan & " - " & ReturJual_NamaPelanggan, nI))
        baris.Add(pI & garis)
        baris.Add(pK & "RETUR PENJUALAN".PadLeft((nK + "RETUR PENJUALAN".Length) \ 2))
        baris.Add(pI & garis)

        ' Kolom item
        Dim lebarJumlah As Integer = CInt(nI * 0.22)
        Dim lebarHarga As Integer = 12
        Dim lebarSat As Integer = 6
        Dim lebarQty As Integer = 5
        Dim lebarNo As Integer = CInt(nI * 0.04)
        Dim lebarNama As Integer = nI - lebarNo - lebarQty - 1 - lebarSat - lebarHarga - lebarJumlah
        If lebarNama < 10 Then lebarNama = 10

        baris.Add(pI & RataKiri("No", lebarNo) &
            RataKiri("Nama Barang", lebarNama) &
            RataKanan("Qty", lebarQty) & " " &
            RataKiri("Sat", lebarSat) &
            RataKanan("Harga", lebarHarga) &
            RataKanan("Jumlah", lebarJumlah))
        baris.Add(pI & garis)

        Dim no As Integer = 1
        For Each item As ItemNotaReturJual In ReturJual_DaftarItem
            Dim namaTerpotong As String = If(item.NamaBarang.Length > lebarNama,
                item.NamaBarang.Substring(0, lebarNama), item.NamaBarang)
            baris.Add(pI &
                RataKiri(no.ToString(), lebarNo) &
                RataKiri(namaTerpotong, lebarNama) &
                RataKanan(item.Qty.ToString("#,0.##", cultureIndonesia), lebarQty) & " " &
                RataKiri(item.Satuan, lebarSat) &
                RataKanan(ReturJualRp(item.Harga), lebarHarga) &
                RataKanan(ReturJualRp(item.TotalHarga), lebarJumlah))
            no += 1
        Next
        baris.Add(pI & garis)

        ' Total
        Dim lebarJmlTotal As Integer = CInt(nI * 0.22)
        Dim lebarLblTotal As Integer = 7
        baris.Add(pI & (JumlahItem & " item").PadRight(nI - lebarLblTotal - lebarJmlTotal) &
                  RataKanan("Total :", lebarLblTotal) & RataKanan(ReturJualRp(ReturJual_Total), lebarJmlTotal))
        baris.Add(pI & "".PadRight(nI - lebarLblTotal - lebarJmlTotal) & New String("="c, lebarLblTotal + lebarJmlTotal))
        baris.Add(pI & garis)

        ' Footer
        If _cfgDot.TampilFooter1 Then baris.Add(pF & FOOTER1.PadLeft((n + FOOTER1.Length) \ 2))
        If _cfgDot.TampilFooter2 Then baris.Add(pF & FOOTER2.PadLeft((n + FOOTER2.Length) \ 2))
        If _cfgDot.TampilFooter3 Then baris.Add(pF & FOOTER3.PadLeft((n + FOOTER3.Length) \ 2))
        baris.Add("")
        baris.Add("")
        baris.Add("")
    End Sub

#End Region

End Class
