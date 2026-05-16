Public Class EscPosCetakTransferBarang

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
        Dim n As Integer = _cfgDot.LebarKertas
        Dim garis As String = New String("-"c, n)
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan IsNot Nothing AndAlso _cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan IsNot Nothing AndAlso _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim pI As String = If(_cfgDot.EscUkuranIsi IsNot Nothing AndAlso _cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi IsNot Nothing AndAlso _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"

        baris.Add(pK & KiriKanan(NAMA_PERUSAHAAN, TB_KeteranganLokasi, n))
        baris.Add(pI & KiriKanan("Tgl    : " & TB_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), "Nomor  : " & TB_IdTransfer, n))
        baris.Add(pI & KiriKanan("Kasir  : " & TB_IdUser, "Lokasi : " & TB_Lokasi, n))
        baris.Add(pI & garis)

        Dim lebarNo As Integer = 3
        Dim lebarKode As Integer = CInt(n * 0.10)
        Dim lebarNama As Integer = CInt(n * 0.28)
        Dim lebarHarga As Integer = CInt(n * 0.14)
        Dim lebarQty As Integer = 6
        Dim lebarSat As Integer = 6
        Dim lebarTotal As Integer = n - lebarNo - lebarKode - lebarNama - lebarHarga - lebarQty - lebarSat
        If lebarTotal < 8 Then lebarTotal = 8

        baris.Add(pI & RataKiri("No", lebarNo) & RataKiri("Kode", lebarKode) &
                  RataKiri("Nama Barang", lebarNama) &
                  RataKanan("Harga", lebarHarga) & RataKanan("Qty", lebarQty) &
                  RataKiri("Sat", lebarSat) & RataKanan("Jumlah", lebarTotal))
        baris.Add(pI & garis)

        Dim no As Integer = 1
        For Each item As ItemTransferBarang In TB_DaftarItem
            Dim kode As String = If(item.IdBarang.Length > lebarKode, item.IdBarang.Substring(0, lebarKode), item.IdBarang)
            Dim nama As String = If(item.NamaBarang.Length > lebarNama, item.NamaBarang.Substring(0, lebarNama), item.NamaBarang)
            baris.Add(pI & RataKiri(no.ToString(), lebarNo) & RataKiri(kode, lebarKode) &
                      RataKiri(nama, lebarNama) &
                      RataKanan(item.Harga.ToString("N0"), lebarHarga) &
                      RataKanan(item.Qty.ToString("N0"), lebarQty) &
                      RataKiri(item.Satuan, lebarSat) &
                      RataKanan(item.Total.ToString("N0"), lebarTotal))
            no += 1
        Next
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Total :", TB_TotalRupiah.ToString("#,##0"), n))
        baris.Add(pI & garis)
        baris.Add(pI & "Terbilang : " & Terbilang(TB_TotalRupiah) & " Rupiah")
        baris.Add(pI & "")
        baris.Add(pI & "Dicetak : " & TB_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"))
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
