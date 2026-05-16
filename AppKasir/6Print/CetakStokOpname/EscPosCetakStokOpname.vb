' ================================================================
' EscPosCetakStokOpname — ESC/POS dot matrix laporan stok opname
' ================================================================
Public Class EscPosCetakStokOpname

    Private ReadOnly _cfgDot As KonfigurasiDotMatrix

    Public Sub New(transaksi As String)
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private Function RataKiri(t As String, l As Integer) As String
        If t.Length >= l Then
            Return t.Substring(0, l)
        Else
            Return t.PadRight(l)
        End If
    End Function
    Private Function RataKanan(t As String, l As Integer) As String
        If t.Length >= l Then
            Return t.Substring(t.Length - l)
        Else
            Return t.PadLeft(l)
        End If
    End Function
    Private Function KiriKanan(kiri As String, kanan As String, n As Integer) As String
        Dim s As Integer = n - kiri.Length - kanan.Length
        If s < 1 Then s = 1
        Return kiri & New String(" "c, s) & kanan
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
        Dim pJ As String = If(_cfgDot.EscUkuranJudul.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranJudul.Contains("Bold") Then pJ &= "~BOLD~"
        Dim nJ As Integer = If(pJ.Contains("~B~"), n \ 2, n)
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"

        baris.Add(pK & KiriKanan(NAMA_PERUSAHAAN, "LAPORAN STOK OPNAME", n))
        baris.Add(pI & KiriKanan("Tgl    : " & SO_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), "ID     : " & SO_IdOpname, n))
        baris.Add(pI & KiriKanan("Kasir  : " & SO_IdUser, "Lokasi : " & SO_Lokasi, n))
        baris.Add(pI & garis)

        Dim lebarNo As Integer = 3
        Dim lebarNama As Integer = CInt(n * 0.30)
        Dim lebarSat As Integer = 6
        Dim lebarStokSys As Integer = CInt(n * 0.10)
        Dim lebarStokNyata As Integer = CInt(n * 0.10)
        Dim lebarSelisih As Integer = CInt(n * 0.10)
        Dim lebarTotal As Integer = n - lebarNo - lebarNama - lebarSat - lebarStokSys - lebarStokNyata - lebarSelisih
        If lebarTotal < 8 Then lebarTotal = 8

        baris.Add(pI & RataKiri("No", lebarNo) & RataKiri("Nama Barang", lebarNama) &
                  RataKiri("Sat", lebarSat) &
                  RataKanan("Sys", lebarStokSys) & RataKanan("Nyata", lebarStokNyata) &
                  RataKanan("Selisih", lebarSelisih) & RataKanan("Total Harga", lebarTotal))
        baris.Add(pI & garis)

        Dim no As Integer = 1
        For Each item As ItemStokOpname In SO_DaftarItem
            Dim nama As String = If(item.NamaBarang.Length > lebarNama, item.NamaBarang.Substring(0, lebarNama), item.NamaBarang)
            baris.Add(pI & RataKiri(no.ToString(), lebarNo) & RataKiri(nama, lebarNama) &
                      RataKiri(item.Satuan, lebarSat) &
                      RataKanan(SORp(item.StokSystem), lebarStokSys) &
                      RataKanan(SORp(item.StokNyata), lebarStokNyata) &
                      RataKanan(SORp(item.StokSelisih), lebarSelisih) &
                      RataKanan(SORp(item.TotalHarga), lebarTotal))
            no += 1
        Next
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Total Item :", SO_DaftarItem.Count.ToString(), n))
        baris.Add(pI & garis)
        baris.Add(pI & "Dicetak : " & SO_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"))
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
