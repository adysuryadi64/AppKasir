' ================================================================
' EscPosCetakGajiKaryawan — ESC/POS thermal slip gaji karyawan
' ================================================================
Public Class EscPosCetakGajiKaryawan

    Private ReadOnly _cfg As KonfigurasiThermal
    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    Public Sub New(transaksi As String)
        _cfg = New KonfigurasiThermal(transaksi)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private Function Rp(v As Decimal) As String
        Return GKRp(v)
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
            Dim c As New GdiCetakGajiKaryawan() : c.Cetak() : Exit Sub
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
        esc.CetakBaris(FmtLabel("Nomor    ", ": " & GK_Nomor, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Bulan    ", ": " & GK_Bulan, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Tanggal  ", ": " & GK_Tanggal.ToString("yyyy-MM-dd"), n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Karyawan ", ": " & GK_NamaKaryawan, n), _cfg.EscUkuranKeterangan)
        esc.CetakBaris(FmtLabel("Periode  ", ": " & GK_TanggalAwal.ToString("dd-MM-yyyy") & " s/d " & GK_TanggalAkhir.ToString("dd-MM-yyyy"), n), _cfg.EscUkuranKeterangan)
        esc.CetakGaris()
        esc.CetakTengah("SLIP GAJI KARYAWAN", _cfg.EscUkuranKeterangan)
        esc.CetakGaris()

        ' Pendapatan
        Dim posNilai As Integer = CInt(n * 0.55)
        Dim lebarLabel As Integer = posNilai
        Dim lebarNilai As Integer = n - posNilai
        esc.CetakBaris("-- PENDAPATAN --", _cfg.EscUkuranIsi)
        esc.CetakBaris(RataKiri("Gaji Pokok", lebarLabel) & RataKanan(Rp(GK_GajiPokok), lebarNilai), _cfg.EscUkuranIsi)
        If GK_KomisiJual <> 0 Then esc.CetakBaris(RataKiri("Komisi Jual", lebarLabel) & RataKanan(Rp(GK_KomisiJual), lebarNilai), _cfg.EscUkuranIsi)
        If GK_SupirRp <> 0 Then esc.CetakBaris(RataKiri("Supir", lebarLabel) & RataKanan(Rp(GK_SupirRp), lebarNilai), _cfg.EscUkuranIsi)
        If GK_HelperRp <> 0 Then esc.CetakBaris(RataKiri("Helper", lebarLabel) & RataKanan(Rp(GK_HelperRp), lebarNilai), _cfg.EscUkuranIsi)
        If GK_LemburRp <> 0 Then esc.CetakBaris(RataKiri("Lembur", lebarLabel) & RataKanan(Rp(GK_LemburRp), lebarNilai), _cfg.EscUkuranIsi)
        If GK_Tunjangan <> 0 Then esc.CetakBaris(RataKiri("Tunjangan", lebarLabel) & RataKanan(Rp(GK_Tunjangan), lebarNilai), _cfg.EscUkuranIsi)
        If GK_Transport <> 0 Then esc.CetakBaris(RataKiri("Transport", lebarLabel) & RataKanan(Rp(GK_Transport), lebarNilai), _cfg.EscUkuranIsi)
        If GK_UangMakan <> 0 Then esc.CetakBaris(RataKiri("Uang Makan", lebarLabel) & RataKanan(Rp(GK_UangMakan), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakBaris(RataKiri("Total Pendapatan", lebarLabel) & RataKanan(Rp(GK_TotalPendapatan), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()

        ' Potongan
        esc.CetakBaris("-- POTONGAN --", _cfg.EscUkuranIsi)
        If GK_PotBon <> 0 Then esc.CetakBaris(RataKiri("Bon", lebarLabel) & RataKanan(Rp(GK_PotBon), lebarNilai), _cfg.EscUkuranIsi)
        If GK_Angsuran <> 0 Then esc.CetakBaris(RataKiri("Angsuran", lebarLabel) & RataKanan(Rp(GK_Angsuran), lebarNilai), _cfg.EscUkuranIsi)
        If GK_AbsenRp <> 0 Then esc.CetakBaris(RataKiri("Absen", lebarLabel) & RataKanan(Rp(GK_AbsenRp), lebarNilai), _cfg.EscUkuranIsi)
        If GK_AbsenKhususRp <> 0 Then esc.CetakBaris(RataKiri("Absen Khusus", lebarLabel) & RataKanan(Rp(GK_AbsenKhususRp), lebarNilai), _cfg.EscUkuranIsi)
        If GK_TerlambatRp <> 0 Then esc.CetakBaris(RataKiri("Terlambat", lebarLabel) & RataKanan(Rp(GK_TerlambatRp), lebarNilai), _cfg.EscUkuranIsi)
        If GK_PotLain <> 0 Then esc.CetakBaris(RataKiri("Pot. Lain", lebarLabel) & RataKanan(Rp(GK_PotLain), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakBaris(RataKiri("Total Potongan", lebarLabel) & RataKanan(Rp(GK_TotalPotongan), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()

        ' Total Terima
        esc.CetakBaris(RataKiri("TOTAL TERIMA", lebarLabel) & RataKanan(Rp(GK_TotalTerima), lebarNilai), _cfg.EscUkuranIsi)
        esc.CetakGaris()

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

End Class
