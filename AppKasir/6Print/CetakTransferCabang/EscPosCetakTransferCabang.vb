' ================================================================
' EscPosCetakTransferCabang
' Cetak nota transfer antar cabang via ESC/POS (Thermal) dan ESC/P (Dot Matrix).
' Mengikuti pola EscPosCetakjualThermalMatrik yang sudah verified.
'
' Cara pakai:
'   Dim c As New EscPosCetakTransferCabang("TransferCabang")
'   c.CetakThermal()     ' Thermal ESC/POS
'   c.CetakDotMatrix()   ' Dot Matrix ESC/P Raw
' ================================================================
Public Class EscPosCetakTransferCabang

#Region "Field & Konstruktor"

    Private ReadOnly _cfg As KonfigurasiThermal
    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    Public Sub New(transaksi As String)
        _cfg = New KonfigurasiThermal(transaksi)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private Function Rp(nilai As Decimal) As String
        Return TCRp(nilai)
    End Function

    Private Function RataKiri(teks As String, lebar As Integer) As String
        If teks.Length >= lebar Then Return teks.Substring(0, lebar)
        Return teks.PadRight(lebar)
    End Function

    Private Function RataKanan(teks As String, lebar As Integer) As String
        If teks.Length >= lebar Then Return teks.Substring(teks.Length - lebar)
        Return teks.PadLeft(lebar)
    End Function

    Private Function KiriKanan(kiri As String, kanan As String, total As Integer) As String
        Dim spasi As Integer = total - kiri.Length - kanan.Length
        If spasi < 1 Then spasi = 1
        Return kiri & New String(" "c, spasi) & kanan
    End Function

    Private Function FormatLabelNilai(label As String, nilai As String, totalLebar As Integer) As String
        Const lebarLabel As Integer = 9
        Dim sisaLebar As Integer = totalLebar - lebarLabel - 1
        Dim nilaiTerpotong As String = If(nilai.Length > sisaLebar, nilai.Substring(0, sisaLebar), nilai)
        Return label.PadRight(lebarLabel) & " " & nilaiTerpotong
    End Function

#End Region

#Region "Cetak Thermal — ESC/POS"

    Public Sub CetakThermal()
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If _cfg.ModeCetak = "GDI+ (Windows Print)" Then
            Dim c As New GdiCetakTransferCabang()
            c.Cetak()
            Exit Sub
        End If

        For i As Integer = 1 To _cfg.JumlahCetak
            Dim esc As PrinterEscPos
            If _cfg.TipeKoneksi = "Network / WiFi (IP)" AndAlso Not String.IsNullOrEmpty(_cfg.IpAddress) Then
                esc = New PrinterEscPos(_cfg.IpAddress, _cfg.NetworkPort, _cfg.LebarKertas, _cfg.BatasKiri)
            Else
                esc = New PrinterEscPos(_cfg.NamaPrinter, _cfg.LebarKertas, _cfg.BatasKiri)
            End If
            CetakSatu(esc)
        Next
    End Sub

    Private Sub CetakSatu(esc As PrinterEscPos)
        CetakHeaderToko(esc)
        CetakInfoTransaksi(esc)
        CetakItem(esc)
        CetakTotal(esc)
        CetakFooter(esc)
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
    End Sub

    Private Sub CetakInfoTransaksi(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        esc.CetakGaris()
        esc.CetakTengah("NOTA TRANSFER ANTAR CABANG", _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
        esc.CetakBaris(FormatLabelNilai("No Trans ", ": " & TC_IdTransfer, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Tanggal  ", ": " & TC_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Dari     ", ": " & TC_DariCabang, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Ke       ", ": " & TC_KeCabang, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Mode     ", ": " & TC_ModeKirim, n), _cfg.EscUkuranIsi)
        esc.CetakBaris(FormatLabelNilai("Status   ", ": " & TC_StatusTransfer, n), _cfg.EscUkuranIsi)
        If Not String.IsNullOrEmpty(TC_Keterangan) Then
            esc.CetakBaris(FormatLabelNilai("Ket      ", ": " & TC_Keterangan, n), _cfg.EscUkuranIsi)
        End If
        esc.CetakBaris(FormatLabelNilai("User     ", ": " & TC_IdUser, n), _cfg.EscUkuranIsi)
        esc.CetakGaris()
    End Sub

    Private Sub CetakItem(esc As PrinterEscPos)
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
                       RataKanan("Total", lebarJumlah), _cfg.EscUkuranIsi)
        esc.CetakGaris()

        For Each item As ItemTransferCabang In TC_DaftarItem
            esc.CetakBaris(item.NamaBarang, _cfg.EscUkuranIsi)
            esc.CetakBaris(
                RataKanan(item.QtySatuan.ToString("#,0.##", cultureIndonesia), lebarQty) &
                " " & RataKiri(item.Satuan, 4) &
                RataKanan(Rp(item.Harga), lebarHarga) &
                RataKanan(Rp(item.Total), lebarJumlah), _cfg.EscUkuranIsi)
            If _cfg.JarakBarisEsc > 0 Then esc.CetakBarisKosong(_cfg.JarakBarisEsc)
        Next
        esc.CetakGaris()
    End Sub

    Private Sub CetakTotal(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        Dim posLabel As Integer = CInt(n * 0.51)
        Dim lebarKanan As Integer = n - posLabel
        Dim lebarNilai As Integer = CInt(lebarKanan * 0.55)
        Dim lebarLbl As Integer = lebarKanan - lebarNilai

        esc.CetakBaris((TC_DaftarItem.Count & " item").PadRight(posLabel) &
            RataKanan("Total Qty :", lebarLbl) & RataKanan(TC_TotalQty.ToString("N0"), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakBaris("".PadRight(posLabel) &
            RataKanan("Total Nilai :", lebarLbl) & RataKanan(Rp(TC_TotalRupiah), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()
        esc.CetakBaris("Terbilang: " & Terbilang(TC_TotalRupiah) & " Rupiah", _cfg.EscUkuranIsi)
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

#Region "Cetak Dot Matrix — ESC/P Raw"

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
        Dim n As Integer = _cfgDot.LebarKertas
        Dim garis As String = New String("-"c, n)

        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"

        baris.Add(pK & KiriKanan(NAMA_PERUSAHAAN, "NOTA TRANSFER ANTAR CABANG", n))
        baris.Add(pI & KiriKanan("No     : " & TC_IdTransfer, "Tgl : " & TC_Tanggal.ToString("dd/MM/yy HH:mm"), n))
        baris.Add(pI & KiriKanan("Dari   : " & TC_DariCabang, "Ke  : " & TC_KeCabang, n))
        baris.Add(pI & KiriKanan("Mode   : " & TC_ModeKirim, "Status: " & TC_StatusTransfer, n))
        If Not String.IsNullOrEmpty(TC_Keterangan) Then
            baris.Add(pI & "Ket    : " & TC_Keterangan)
        End If
        baris.Add(pI & "User   : " & TC_IdUser)
        baris.Add(pI & garis)

        ' Kolom header
        Dim lebarNo As Integer = 3
        Dim lebarNama As Integer = CInt(n * 0.32)
        Dim lebarQty As Integer = 7
        Dim lebarSat As Integer = 6
        Dim lebarHarga As Integer = CInt(n * 0.15)
        Dim lebarTotal As Integer = n - lebarNo - lebarNama - lebarQty - lebarSat - lebarHarga
        If lebarTotal < 8 Then lebarTotal = 8

        baris.Add(pI & RataKiri("No", lebarNo) & RataKiri("Nama Barang", lebarNama) &
                  RataKanan("Qty", lebarQty) & RataKiri("Sat", lebarSat) &
                  RataKanan("Harga", lebarHarga) & RataKanan("Total", lebarTotal))
        baris.Add(pI & garis)

        Dim no As Integer = 1
        For Each item As ItemTransferCabang In TC_DaftarItem
            Dim nama As String = If(item.NamaBarang.Length > lebarNama,
                                    item.NamaBarang.Substring(0, lebarNama), item.NamaBarang)
            baris.Add(pI & RataKiri(no.ToString(), lebarNo) &
                      RataKiri(nama, lebarNama) &
                      RataKanan(item.QtySatuan.ToString("N0"), lebarQty) &
                      RataKiri(item.Satuan, lebarSat) &
                      RataKanan(item.Harga.ToString("N0"), lebarHarga) &
                      RataKanan(item.Total.ToString("N0"), lebarTotal))
            no += 1
        Next

        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Total Qty : " & TC_TotalQty.ToString("N0"),
                                  "Total Nilai : " & Rp(TC_TotalRupiah), n))
        baris.Add(pI & garis)
        baris.Add(pI & "Terbilang : " & Terbilang(TC_TotalRupiah) & " Rupiah")
        baris.Add("") : baris.Add("") : baris.Add("")

        Dim sb As New System.Text.StringBuilder()
        sb.Append(Chr(27) & "@")
        If _cfgDot.BatasKiri > 0 Then sb.Append(Chr(27) & "l" & Chr(_cfgDot.BatasKiri))
        Dim isBesar As Boolean = False
        Dim isBold As Boolean = False
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
            sb.Append(b & Chr(13) & Chr(10))
        Next
        If isBesar Then sb.Append(Chr(27) & "W" & Chr(0))
        If isBold Then sb.Append(Chr(27) & "E" & Chr(0))
        Return System.Text.Encoding.GetEncoding(437).GetBytes(sb.ToString())
    End Function

#End Region

End Class
