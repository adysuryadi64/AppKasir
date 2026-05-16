' ================================================================
' EscPosCetakTransferStok — ESC/POS dot matrix bukti transfer stok
' ================================================================
Public Class EscPosCetakTransferStok

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
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"

        baris.Add(pK & KiriKanan(NAMA_PERUSAHAAN, "BUKTI TRANSFER STOK", n))
        baris.Add(pI & KiriKanan("Tgl    : " & TS_Tanggal.ToString("yyyy-MM-dd HH:mm:ss"), "ID     : " & TS_IdTransfer, n))
        baris.Add(pI & KiriKanan("Kasir  : " & TS_IdUser, "Jenis  : " & TS_JenisTransfer, n))
        If Not String.IsNullOrEmpty(TS_Uraian) Then
            baris.Add(pI & "Uraian
            " & TS_Uraian)
        End If
        baris.Add(pI & garis)

        Dim lebarNamaMasuk As Integer = CInt(n * 0.22)
        Dim lebarQtyMasuk As Integer = 6
        Dim lebarHargaMasuk As Integer = CInt(n * 0.12)
        Dim lebarNamaKeluar As Integer = CInt(n * 0.22)
        Dim lebarQtyKeluar As Integer = 6
        Dim lebarHargaKeluar As Integer = CInt(n * 0.12)
        Dim lebarSelisih As Integer = n - lebarNamaMasuk - lebarQtyMasuk - lebarHargaMasuk - lebarNamaKeluar - lebarQtyKeluar - lebarHargaKeluar
        If lebarSelisih < 8 Then lebarSelisih = 8

        baris.Add(pI & RataKiri("Barang Masuk", lebarNamaMasuk) & RataKanan("Qty", lebarQtyMasuk) &
                  RataKanan("Harga", lebarHargaMasuk) &
                  RataKiri("Barang Keluar", lebarNamaKeluar) & RataKanan("Qty", lebarQtyKeluar) &
                  RataKanan("Harga", lebarHargaKeluar) & RataKanan("Selisih", lebarSelisih))
        baris.Add(pI & garis)

        Dim no As Integer = 1
        For Each item As ItemTransferStok In TS_DaftarItem
            Dim namaMasuk As String = If(item.NamaBarangMasuk.Length > lebarNamaMasuk, item.NamaBarangMasuk.Substring(0, lebarNamaMasuk), item.NamaBarangMasuk)
            Dim namaKeluar As String = If(item.NamaBarangKeluar.Length > lebarNamaKeluar, item.NamaBarangKeluar.Substring(0, lebarNamaKeluar), item.NamaBarangKeluar)
            baris.Add(pI & RataKiri(namaMasuk, lebarNamaMasuk) &
                      RataKanan(TSRp(item.QtyMasuk), lebarQtyMasuk) &
                      RataKanan(TSRp(item.HargaMasuk), lebarHargaMasuk) &
                      RataKiri(namaKeluar, lebarNamaKeluar) &
                      RataKanan(TSRp(item.QtyKeluar), lebarQtyKeluar) &
                      RataKanan(TSRp(item.HargaKeluar), lebarHargaKeluar) &
                      RataKanan(TSRp(item.Selisih), lebarSelisih))
            no += 1
        Next
        baris.Add(pI & garis)
        baris.Add(pI & "Dicetak : " & TS_IdUser & " " & Now.ToString("dd-MM-yy HH:mm:ss"))
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
