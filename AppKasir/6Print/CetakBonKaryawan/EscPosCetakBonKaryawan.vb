' ================================================================
' EscPosCetakBonKaryawan — ESC/POS thermal & dot matrix slip bon karyawan
' ================================================================
Public Class EscPosCetakBonKaryawan

    Private ReadOnly _cfg As KonfigurasiThermal
    Private ReadOnly _transaksi As String
    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    Public Sub New(transaksi As String)
        _transaksi = transaksi
        _cfg = New KonfigurasiThermal(transaksi)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private Function Rp(v As Decimal) As String
        Return BKRp(v)
    End Function
    Private Function RataKanan(t As String, l As Integer) As String
        If t.Length >= l Then
            Return t.Substring(t.Length - l)
        Else
            Return t.PadLeft(l)
        End If
    End Function
    Private Function RataKiri(t As String, l As Integer) As String
        If t.Length >= l Then
            Return t.Substring(0, l)
        Else
            Return t.PadRight(l)
        End If
    End Function
    Private Function KiriKanan(kiri As String, kanan As String, n As Integer) As String
        Dim s As Integer = n - kiri.Length - kanan.Length
        If s < 1 Then s = 1
        Return kiri & New String(" "c, s) & kanan
    End Function
    Private Function FmtLabel(label As String, nilai As String, n As Integer) As String
        Const ll As Integer = 9
        Dim v As String = If(nilai.Length > n - ll - 1, nilai.Substring(0, n - ll - 1), nilai)
        Return label.PadRight(ll) & " " & v
    End Function

#Region "Cetak Thermal"
    Public Sub CetakThermal()
        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer thermal belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        If _cfg.ModeCetak = "GDI+ (Windows Print)" Then
            Dim c As New GdiCetakBonKaryawan() : c.Cetak() : Exit Sub
        End If
        For i As Integer = 1 To _cfg.JumlahCetak
            Dim esc As PrinterEscPos
            If _cfg.TipeKoneksi = "Network / WiFi (IP)" AndAlso Not String.IsNullOrEmpty(_cfg.IpAddress) Then
                esc = New PrinterEscPos(_cfg.IpAddress, _cfg.NetworkPort, _cfg.LebarKertas, _cfg.BatasKiri)
            Else
                esc = New PrinterEscPos(_cfg.NamaPrinter, _cfg.LebarKertas, _cfg.BatasKiri)
            End If
            CetakModel1(esc)
        Next
        If _cfg.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir(_transaksi)
    End Sub

    Private Sub CetakModel1(esc As PrinterEscPos)
        Dim n As Integer = esc.JumlahKarakterPerBaris
        ' Header
        Dim logoPath As String = System.IO.Path.Combine(Application.StartupPath, "logo.png")
        If Not System.IO.File.Exists(logoPath) Then logoPath = System.IO.Path.Combine(Application.StartupPath, "logo.jpg")
        If _cfg.TampilLogo Then esc.CetakLogo(logoPath, If(_cfg.LebarKertas >= 80, 384, 256))
        esc.CetakHeader(NAMA_PERUSAHAAN, _cfg.EscUkuranJudul)
        esc.CetakTengah(ALAMAT_PERUSAHAAN, _cfg.EscUkuranKeterangan)
        esc.CetakTengah(KOTA_PERUSAHAAN, _cfg.EscUkuranKeterangan)
        esc.CetakTengah(KONTAK_PERUSAHAAN, _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
        esc.CetakBaris(FmtLabel("Faktur   ", ": " & BK_Faktur, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Tanggal  ", ": " & BK_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Kasir    ", ": " & BK_IdUser & " - " & BK_IdKomputer, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Karyawan ", ": " & BK_NamaKaryawan, n), _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
        esc.CetakTengah("SLIP BON KARYAWAN", _cfg.EscUkuranKeterangan)
        esc.CetakGaris()

        ' Isi
        Dim posNilai As Integer = CInt(n * 0.6)
        Dim lebarLabel As Integer = posNilai
        Dim lebarNilai As Integer = n - posNilai
        esc.CetakBaris(RataKiri("Saldo Awal", lebarLabel) & RataKanan(Rp(BK_AwalBon), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakBaris(RataKiri("Bon (" & BK_Jenis & ")", lebarLabel) & RataKanan(Rp(BK_Nominal), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()
        esc.CetakBaris(RataKiri("Saldo Akhir", lebarLabel) & RataKanan(Rp(BK_AkhirBon), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()
        If Not String.IsNullOrEmpty(BK_Keterangan) Then
            esc.CetakBaris("Ket: " & BK_Keterangan, _cfg.EscUkuranIsi)
            esc.CetakGaris()
        End If

        ' Footer
        If _cfg.TampilFooter1 Then
            For Each b As String In FOOTER1.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                esc.CetakTengah(b, _cfg.EscUkuranFooter)
            Next
        End If
        If _cfg.TampilFooter2 Then
            For Each b As String In FOOTER2.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                esc.CetakTengah(b, _cfg.EscUkuranFooter)
            Next
        End If
        If _cfg.TampilFooter3 Then
            For Each b As String In FOOTER3.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                esc.CetakTengah(b, _cfg.EscUkuranFooter)
            Next
        End If
        esc.CetakBarisKosong(3)
        If _cfg.PotongOtomatis Then esc.PotongKertas()
        esc.Flush()
    End Sub
#End Region

#Region "Cetak Dot Matrix"
    Public Sub CetakDotMatrix()
        If String.IsNullOrEmpty(_cfgDot.NamaPrinter) Then
            MessageBox.Show("Printer dot matrix belum diatur.", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
        Dim pJ As String = If(_cfgDot.EscUkuranJudul.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranJudul.Contains("Bold") Then pJ &= "~BOLD~"
        Dim nJ As Integer = If(pJ.Contains("~B~"), n \ 2, n)
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim nK As Integer = If(pK.Contains("~B~"), n \ 2, n)
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"
        Dim pF As String = If(_cfgDot.EscUkuranFooter.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranFooter.Contains("Bold") Then pF &= "~BOLD~"

        baris.Add(pJ & NAMA_PERUSAHAAN.PadLeft((nJ + NAMA_PERUSAHAAN.Length) \ 2))
        baris.Add(pK & ALAMAT_PERUSAHAAN.PadLeft((nK + ALAMAT_PERUSAHAAN.Length) \ 2))
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Faktur", BK_Faktur, n))
        baris.Add(pI & KiriKanan("Tanggal", BK_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), n))
        baris.Add(pI & KiriKanan("Kasir", BK_IdUser & " - " & BK_IdKomputer, n))
        baris.Add(pI & KiriKanan("Karyawan", BK_NamaKaryawan, n))
        baris.Add(pI & garis)
        baris.Add(pK & "SLIP BON KARYAWAN".PadLeft((nK + "SLIP BON KARYAWAN".Length) \ 2))
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Saldo Awal", BKRp(BK_AwalBon), n))
        baris.Add(pI & KiriKanan("Bon (" & BK_Jenis & ")", BKRp(BK_Nominal), n))
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Saldo Akhir", BKRp(BK_AkhirBon), n))
        baris.Add(pI & garis)
        If Not String.IsNullOrEmpty(BK_Keterangan) Then
            baris.Add(pI & "Ket: " & BK_Keterangan)
        End If
        If _cfgDot.TampilFooter1 Then baris.Add(pF & FOOTER1.PadLeft((n + FOOTER1.Length) \ 2))
        If _cfgDot.TampilFooter2 Then baris.Add(pF & FOOTER2.PadLeft((n + FOOTER2.Length) \ 2))
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
