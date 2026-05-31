' ================================================================
' EscPosCetakSuratJalan — ESC/P raw untuk dot matrix surat jalan
' ================================================================
Public Class EscPosCetakSuratJalan

    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    Public Sub New(transaksi As String)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

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
        Dim n As Integer = CInt(_cfgDot.LebarKertas * 42 / 80)
        If n < 20 Then n = 20
        Dim garis As String = New String("-"c, n)
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"

        ' Header
        baris.Add(pK & KiriKanan(NAMA_PERUSAHAAN, "SURAT JALAN PENGIRIMAN", n))
        baris.Add(pI & KiriKanan("Tgl    : " & SJ_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), "Nomor  : " & SJ_Nota, n))
        baris.Add(pI & KiriKanan("Kasir  : " & SJ_IdUser, "Armada : " & SJ_Armada & " " & SJ_JenisArmada, n))
        baris.Add(pI & garis)

        ' Kolom header
        Dim lebarNo As Integer = 3
        Dim lebarNota As Integer = CInt(n * 0.12)
        Dim lebarNama As Integer = CInt(n * 0.18)
        Dim lebarAlamat As Integer = CInt(n * 0.22)
        Dim lebarJml As Integer = CInt(n * 0.12)
        Dim lebarLokasi As Integer = CInt(n * 0.08)
        Dim lebarTtd As Integer = n - lebarNo - lebarNota - lebarNama - lebarAlamat - lebarJml - lebarLokasi
        If lebarTtd < 5 Then lebarTtd = 5

        baris.Add(pI & RataKiri("No", lebarNo) & RataKiri("Nota", lebarNota) &
                  RataKiri("Pelanggan", lebarNama) & RataKiri("Alamat", lebarAlamat) &
                  RataKanan("Jumlah", lebarJml) & RataKiri("Lok", lebarLokasi) &
                  RataKiri("TTD", lebarTtd))
        baris.Add(pI & garis)

        Dim no As Integer = 1
        For Each item As ItemSuratJalan In SJ_DaftarDetail
            Dim nota As String = If(item.NotaBelanja.Length > lebarNota, item.NotaBelanja.Substring(0, lebarNota), item.NotaBelanja)
            Dim nama As String = If(item.NamaPelanggan.Length > lebarNama, item.NamaPelanggan.Substring(0, lebarNama), item.NamaPelanggan)
            Dim alamat As String = If(item.AlamatPelanggan.Length > lebarAlamat, item.AlamatPelanggan.Substring(0, lebarAlamat), item.AlamatPelanggan)
            baris.Add(pI & RataKiri(no.ToString(), lebarNo) & RataKiri(nota, lebarNota) &
                      RataKiri(nama, lebarNama) & RataKiri(alamat, lebarAlamat) &
                      RataKanan(item.NilaiBelanja.ToString("#,##0"), lebarJml) &
                      RataKiri(item.Lokasi, lebarLokasi) &
                      RataKiri(". . . . . .", lebarTtd))
            no += 1
        Next
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Total :", SJRp(SJ_TotalRupiah), n))
        baris.Add(pI & garis)
        baris.Add(pI & "Terbilang : " & Terbilang(SJ_TotalRupiah) & " Rupiah")
        baris.Add(pI & "")
        baris.Add(pI & KiriKanan(KiriKanan("Sopir", "Helper 1", CInt(n * 0.6)), "Helper 2", n))
        baris.Add(pI & "")
        baris.Add(pI & KiriKanan(KiriKanan(SJ_Supir, SJ_Helper1, CInt(n * 0.6)), SJ_Helper2, n))
        baris.Add(pI & "")
        baris.Add(pI & "Dicetak : " & SJ_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"))
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

End Class
