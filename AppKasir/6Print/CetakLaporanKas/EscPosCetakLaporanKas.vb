' ================================================================
' EscPosCetakLaporanKas — ESC/POS dot matrix laporan mutasi kas
' ================================================================
Public Class EscPosCetakLaporanKas

    Private ReadOnly _cfgDot As KonfigurasiDotMatrix
    Private ReadOnly _transaksi As String

    Public Sub New(transaksi As String)
        _transaksi = transaksi
        _cfgDot = New KonfigurasiDotMatrix(transaksi)
    End Sub

    Private Function Rp(v As Decimal) As String
        Return v.ToString("#,0.##", cultureIndonesia)
    End Function
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
        If _cfgDot.KodeLaciKasir <> "(Tidak Ada)" Then BukaLaciKasir(_transaksi)
    End Sub

    Private Function BangunEscP() As Byte()
        Dim baris As New List(Of String)
        Dim n As Integer = _cfgDot.LebarKertas
        Dim garis As String = New String("-"c, n)
        Dim garisGanda As String = New String("="c, n)
        Dim pJ As String = If(_cfgDot.EscUkuranJudul.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranJudul.Contains("Bold") Then pJ &= "~BOLD~"
        Dim nJ As Integer = If(pJ.Contains("~B~"), n \ 2, n)
        Dim pK As String = If(_cfgDot.EscUkuranKeterangan.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranKeterangan.Contains("Bold") Then pK &= "~BOLD~"
        Dim nK As Integer = If(pK.Contains("~B~"), n \ 2, n)
        Dim pI As String = If(_cfgDot.EscUkuranIsi.Contains("Besar"), "~B~", "~N~")
        If _cfgDot.EscUkuranIsi.Contains("Bold") Then pI &= "~BOLD~"

        ' Header
        baris.Add(pJ & NAMA_PERUSAHAAN.PadLeft((nJ + NAMA_PERUSAHAAN.Length) \ 2))
        baris.Add(pK & ALAMAT_PERUSAHAAN.PadLeft((nK + ALAMAT_PERUSAHAAN.Length) \ 2))
        baris.Add(pI & garis)
        baris.Add(pK & "LAPORAN MUTASI KEUANGAN".PadLeft((nK + "LAPORAN MUTASI KEUANGAN".Length) \ 2))
        baris.Add(pI & garis)

        ' Ambil data dari GdiCetakLaporanKas instance yang aktif
        ' (data sudah diisi di ModulePrinterLaporanKas sebelum cetak)
        Dim cetak As GdiCetakLaporanKas = ModuleCetakLaporanKasInkjet.GetCurrentCetak()
        If cetak Is Nothing Then Return New Byte() {}

        baris.Add(pI & KiriKanan("Rekening", cetak.LK_Rekening, n))
        baris.Add(pI & KiriKanan("Periode", cetak.LK_PeriodeLabel, n))
        baris.Add(pI & KiriKanan("Kasir", cetak.LK_Kasir, n))
        baris.Add(pI & garis)

        ' Kolom
        Dim lebarNama As Integer = CInt(n * 0.45)
        Dim lebarNota As Integer = CInt(n * 0.15)
        Dim lebarTotal As Integer = n - lebarNama - lebarNota
        baris.Add(pI & RataKiri("Transaksi", lebarNama) & RataKanan("Nota", lebarNota) & RataKanan("Sub Total", lebarTotal))
        baris.Add(pI & garis)

        Dim TulisBaris = Sub(tanda As String, nama As String, nota As Integer, total As Decimal)
                             If total = 0 Then Return
                             baris.Add(pI & RataKiri(tanda & " " & nama, lebarNama) &
                                       RataKanan(nota.ToString("N0", cultureIndonesia), lebarNota) &
                                       RataKanan(Rp(total), lebarTotal))
                         End Sub

        TulisBaris("(-)", "Pembelian", cetak.LK_NotaPembelian, cetak.LK_TotalPembelian)
        TulisBaris("(+)", "Penjualan", cetak.LK_NotaPenjualan, cetak.LK_TotalPenjualan)
        TulisBaris("(+)", "Retur Beli", cetak.LK_NotaReturBeli, cetak.LK_TotalReturBeli)
        TulisBaris("(-)", "Retur Jual", cetak.LK_NotaReturJual, cetak.LK_TotalReturJual)
        TulisBaris("(-)", "Bayar Hutang", cetak.LK_NotaBayarHutang, cetak.LK_TotalBayarHutang)
        TulisBaris("(+)", "Bayar Piutang", cetak.LK_NotaBayarPiutang, cetak.LK_TotalBayarPiutang)
        TulisBaris("(+)", "Jurnal Pemasukan", cetak.LK_NotaPemasukan, cetak.LK_TotalPemasukan)
        TulisBaris("(-)", "Jurnal Pengeluaran", cetak.LK_NotaPengeluaran, cetak.LK_TotalPengeluaran)
        TulisBaris("(-)", "Jurnal Biaya", cetak.LK_NotaBiaya, cetak.LK_TotalBiaya)
        TulisBaris("(+)", "Pindah Rek (+)", cetak.LK_NotaPRDebet, cetak.LK_TotalPRDebet)
        TulisBaris("(-)", "Pindah Rek (-)", cetak.LK_NotaPRKredit, cetak.LK_TotalPRKredit)
        TulisBaris("(-)", "Bon Karyawan", cetak.LK_NotaBon, cetak.LK_TotalBon)
        TulisBaris("(+)", "Bayar Bon", cetak.LK_NotaBayarBon, cetak.LK_TotalBayarBon)
        TulisBaris("(-)", "Gaji Karyawan", cetak.LK_NotaGaji, cetak.LK_TotalGaji)
        TulisBaris("(+)", "Pinjaman Supplier", cetak.LK_NotaPinjamanSupplier, cetak.LK_TotalPinjamanSupplier)
        TulisBaris("(-)", "Pinjaman Pelanggan", cetak.LK_NotaPinjamanPelanggan, cetak.LK_TotalPinjamanPelanggan)

        baris.Add(pI & garisGanda)
        baris.Add(pI & KiriKanan("Saldo Hari ini :", Rp(cetak.LK_SaldoHariIni), n))
        If cetak.LK_TypeAkun.ToUpper() = "KAS" Then
            baris.Add(pI & KiriKanan("Uang di setor  :", Rp(cetak.LK_SetorBos), n))
            baris.Add(pI & KiriKanan("Uang di laci   :", Rp(cetak.LK_SaldoDilaci), n))
        End If
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("Saldo Awal     :", Rp(cetak.LK_SaldoAwal), n))
        baris.Add(pI & KiriKanan("Hari ini       :", Rp(cetak.LK_TotalHariIni), n))
        baris.Add(pI & KiriKanan("Saldo Akhir    :", Rp(cetak.LK_SaldoAkhir), n))
        baris.Add(pI & garis)
        baris.Add(pI & KiriKanan("", KOTA_PERUSAHAAN & ", " & Now.ToString("dd-MM-yyyy"), n))
        baris.Add(pI & "")
        baris.Add(pI & "ACC")
        baris.Add(pI & "")
        baris.Add(pI & "")
        baris.Add(pI & If(String.IsNullOrEmpty(cetak.LK_Pemilik), NAMA_PERUSAHAAN, cetak.LK_Pemilik))
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
