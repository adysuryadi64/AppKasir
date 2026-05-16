Imports ESCPOS_NET
Imports ESCPOS_NET.Emitters
Imports ESCPOS_NET.Utilities

' ================================================================
' PrinterEscPos - menggunakan ESCPOS_NET 3.0.0
' Mendukung: USB (Windows Spooler), Network/WiFi (IP:Port), Serial
'
' Cara pakai USB:
'   Dim esc As New PrinterEscPos("Nama Printer", 80)
'
' Cara pakai Network/WiFi:
'   Dim esc As New PrinterEscPos("192.168.1.50", 9100, 80)
'
' Semua method tetap sama — hanya implementasi internal yang berubah.
' ================================================================
Public Class PrinterEscPos

#Region "Field & Konstruktor"

    Private ReadOnly _namaPrinter As String
    Private ReadOnly _ipAddress As String
    Private ReadOnly _port As Integer
    Private ReadOnly _jumlahKarakterPerBaris As Integer
    Private ReadOnly _batasKiri As Integer
    Private ReadOnly _emitter As New EPSON()
    Private _buffer As New List(Of Byte)   ' akumulasi semua data sebelum dikirim

    ' ── Konstruktor USB / Windows Spooler ────────────────────
    Public Sub New(namaPrinter As String, lebarKertasMm As Integer,
                   Optional batasKiri As Integer = 0)
        _namaPrinter = namaPrinter
        _ipAddress = ""
        _port = 0
        _batasKiri = Math.Max(0, batasKiri)
        Dim lebarTotal As Integer = If(lebarKertasMm >= 80, 48, 32)
        _jumlahKarakterPerBaris = Math.Max(16, lebarTotal - _batasKiri)
    End Sub

    ' ── Konstruktor Network / WiFi (IP Address) ───────────────
    Public Sub New(ipAddress As String, port As Integer, lebarKertasMm As Integer,
                   Optional batasKiri As Integer = 0)
        _namaPrinter = ""
        _ipAddress = ipAddress
        _port = port
        _batasKiri = Math.Max(0, batasKiri)
        Dim lebarTotal As Integer = If(lebarKertasMm >= 80, 48, 32)
        _jumlahKarakterPerBaris = Math.Max(16, lebarTotal - _batasKiri)
    End Sub

    Public ReadOnly Property JumlahKarakterPerBaris As Integer
        Get
            Return _jumlahKarakterPerBaris
        End Get
    End Property

#End Region

#Region "Buffer & Flush"

    ' ── Tambah ke buffer (tidak langsung kirim) ──────────────
    Private Sub Tambah(data As Byte())
        If data Is Nothing OrElse data.Length = 0 Then Return
        _buffer.AddRange(data)
    End Sub

    ' ── Kirim semua buffer ke printer sekaligus ──────────────
    Public Sub Flush()
        If _buffer.Count = 0 Then Return
        Dim data As Byte() = _buffer.ToArray()
        _buffer.Clear()
        Try
            If Not String.IsNullOrEmpty(_ipAddress) Then
                Dim pengaturan As New ImmediateNetworkPrinterSettings() With {
                    .ConnectionString = $"{_ipAddress}:{_port}",
                    .PrinterName = "NetworkPrinter"
                }
                Dim printer As New ImmediateNetworkPrinter(pengaturan)
                printer.WriteAsync(data).GetAwaiter().GetResult()
            Else
                Dim ok As Boolean = RawPrinterHelper.KirimKePrinter(_namaPrinter, data)
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' ── Reset printer ke kondisi awal ────────────────────────
    Public Sub Reset()
        Tambah(_emitter.Initialize())
    End Sub

#End Region

#Region "Cetak Teks"

    Public Sub CetakHeader(teksHeader As String, Optional ukuran As String = "Normal")
        CetakTengah(teksHeader, ukuran, bold:=True)
    End Sub

    ' Cetak nama toko dengan ukuran double (lebih besar) — center + bold
    Public Sub CetakHeaderBesar(teksHeader As String)
        CetakTengah(teksHeader, "Besar (2x)", bold:=True)
    End Sub

    Public Sub CetakBaris(teks As String, Optional ukuran As String = "Normal", Optional bold As Boolean = False)
        Dim isBesar As Boolean = (ukuran.Contains("Besar"))
        Dim isBold As Boolean = bold OrElse ukuran.Contains("Bold")
        Dim style As PrintStyle = If(isBold, PrintStyle.Bold, PrintStyle.None)
        If isBesar Then style = style Or PrintStyle.DoubleWidth Or PrintStyle.DoubleHeight

        Dim spasi As Integer = If(isBesar, _batasKiri \ 2, _batasKiri)
        Dim prefix As String = New String(" "c, spasi)

        Tambah(ByteSplicer.Combine(
            _emitter.Initialize(),
            _emitter.SetStyles(style),
            _emitter.LeftAlign(),
            _emitter.PrintLine(prefix & teks),
            _emitter.SetStyles(PrintStyle.None)
        ))
    End Sub

    Public Sub CetakTengah(teks As String, Optional ukuran As String = "Normal", Optional bold As Boolean = False)
        Dim isBesar As Boolean = (ukuran.Contains("Besar"))
        Dim isBold As Boolean = bold OrElse ukuran.Contains("Bold")
        Dim style As PrintStyle = If(isBold, PrintStyle.Bold, PrintStyle.None)
        If isBesar Then style = style Or PrintStyle.DoubleWidth Or PrintStyle.DoubleHeight

        Dim len As Integer = If(isBesar, teks.Length * 2, teks.Length)
        Dim spasiTengah As Integer = Math.Max(0, (_jumlahKarakterPerBaris - len) \ 2)
        Dim spasi As Integer = If(isBesar, (_batasKiri + spasiTengah) \ 2, _batasKiri + spasiTengah)
        Dim prefix As String = New String(" "c, spasi)

        Tambah(ByteSplicer.Combine(
            _emitter.Initialize(),
            _emitter.SetStyles(style),
            _emitter.LeftAlign(),
            _emitter.PrintLine(prefix & teks),
            _emitter.SetStyles(PrintStyle.None)
        ))
    End Sub

    Public Sub CetakKiriKanan(teksKiri As String, teksKanan As String)
        Dim jumlahSpasi As Integer = _jumlahKarakterPerBaris - teksKiri.Length - teksKanan.Length
        If jumlahSpasi < 1 Then jumlahSpasi = 1
        Dim prefix As String = New String(" "c, _batasKiri)

        Tambah(ByteSplicer.Combine(
            _emitter.LeftAlign(),
            _emitter.PrintLine(prefix & teksKiri & New String(" "c, jumlahSpasi) & teksKanan)
        ))
    End Sub

    Public Sub CetakBarisKolom(namaBarang As String, jumlahQty As String,
                                hargaSatuan As String, subtotal As String)
        Const lebarKolomNama As Integer = 20
        Const lebarKolomQty As Integer = 6
        Const lebarKolomHarga As Integer = 10
        Const lebarKolomSubtotal As Integer = 12
        Dim barisLengkap As String =
            namaBarang.PadRight(lebarKolomNama).Substring(0, lebarKolomNama) &
            jumlahQty.PadLeft(lebarKolomQty) &
            hargaSatuan.PadLeft(lebarKolomHarga) &
            subtotal.PadLeft(lebarKolomSubtotal)

        Dim prefix As String = New String(" "c, _batasKiri)
        Tambah(ByteSplicer.Combine(
            _emitter.LeftAlign(),
            _emitter.PrintLine(prefix & barisLengkap)
        ))
    End Sub

#End Region

#Region "Cetak Garis & Spasi"

    Public Sub CetakGaris()
        Dim prefix As String = New String(" "c, _batasKiri)
        Tambah(_emitter.PrintLine(prefix & New String("-"c, _jumlahKarakterPerBaris)))
    End Sub

    Public Sub CetakGarisGanda()
        Dim prefix As String = New String(" "c, _batasKiri)
        Tambah(_emitter.PrintLine(prefix & New String("="c, _jumlahKarakterPerBaris)))
    End Sub

    Public Sub CetakGarisTitik()
        Dim prefix As String = New String(" "c, _batasKiri)
        Tambah(_emitter.PrintLine(prefix & New String("."c, _jumlahKarakterPerBaris)))
    End Sub

    Public Sub CetakGarisKustom(karakterPembatas As Char)
        Dim prefix As String = New String(" "c, _batasKiri)
        Tambah(_emitter.PrintLine(prefix & New String(karakterPembatas, _jumlahKarakterPerBaris)))
    End Sub

    Public Sub CetakBarisKosong(Optional jumlahBaris As Integer = 1)
        Dim baris As Byte() = {}
        For i As Integer = 1 To jumlahBaris
            baris = ByteSplicer.Combine(baris, _emitter.PrintLine(""))
        Next
        Tambah(baris)
    End Sub

#End Region

#Region "Potong Kertas & Laci Kasir"

    Public Sub PotongKertas()
        Tambah(_emitter.FullCutAfterFeed(3))
    End Sub

    Public Sub BukaLaci(Optional pinNumber As Integer = 2)
        If pinNumber = 5 Then
            Tambah(_emitter.CashDrawerOpenPin5())
        Else
            Tambah(_emitter.CashDrawerOpenPin2())
        End If
    End Sub

#End Region

#Region "Cetak Logo"

    ' Konversi gambar ke 1-bit raster bitmap untuk ESC/POS.
    ' Menggunakan perintah GS v 0 (raster bit image) secara langsung
    ' sehingga tidak bergantung pada SixLabors.ImageSharp di ESCPOS_NET.
    ' maxWidth: lebar maksimal dalam pixel (default 384 = 80mm @203dpi)
    ' Return True jika berhasil, False jika file tidak ada atau gagal.
    Public Function CetakLogo(filePath As String,
                               Optional maxWidthPx As Integer = 384,
                               Optional isHiDPI As Boolean = False) As Boolean
        If Not System.IO.File.Exists(filePath) Then Return False
        Try
            Using bmpAsli As New System.Drawing.Bitmap(filePath)
                ' Hitung ukuran proporsional agar tidak melebihi maxWidthPx
                ' Lebar harus kelipatan 8 (syarat GS v 0)
                Dim lebarBaru As Integer = Math.Min(bmpAsli.Width, maxWidthPx)
                lebarBaru = CInt(Math.Floor(lebarBaru / 8.0)) * 8
                If lebarBaru = 0 Then Return False
                Dim tinggiBaru As Integer = CInt(bmpAsli.Height * (lebarBaru / bmpAsli.Width))
                If tinggiBaru = 0 Then Return False

                ' Buat canvas logical padding sesuai batas kiri dan karakter per baris
                Dim lebarCanvasPx As Integer = (_batasKiri + _jumlahKarakterPerBaris) * 12
                lebarCanvasPx = CInt(Math.Floor(lebarCanvasPx / 8.0)) * 8
                Dim finalWidth As Integer = Math.Max(lebarCanvasPx, lebarBaru)
                
                Dim xOffset As Integer = (_batasKiri * 12) + ((_jumlahKarakterPerBaris * 12) - lebarBaru) \ 2
                If xOffset < 0 Then xOffset = 0

                Using bmpResize As New System.Drawing.Bitmap(finalWidth, tinggiBaru)
                    Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmpResize)
                        g.Clear(System.Drawing.Color.White)
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                        g.DrawImage(bmpAsli, xOffset, 0, lebarBaru, tinggiBaru)
                    End Using

                    ' Konversi ke byte array 1-bit (MSB first, per baris)
                    Dim lebarBytes As Integer = finalWidth \ 8
                    Dim imgBytes(lebarBytes * tinggiBaru - 1) As Byte
                    For y As Integer = 0 To tinggiBaru - 1
                        For x As Integer = 0 To finalWidth - 1
                            Dim px As System.Drawing.Color = bmpResize.GetPixel(x, y)
                            If px.GetBrightness() < 0.5F Then
                                Dim byteIdx As Integer = y * lebarBytes + (x \ 8)
                                Dim bitIdx As Integer = 7 - (x Mod 8)
                                imgBytes(byteIdx) = imgBytes(byteIdx) Or CByte(1 << bitIdx)
                            End If
                        Next
                    Next

                    ' Bangun perintah GS v 0 secara manual
                    Dim xL As Byte = CByte(lebarBytes And &HFF)
                    Dim xH As Byte = CByte((lebarBytes >> 8) And &HFF)
                    Dim yL As Byte = CByte(tinggiBaru And &HFF)
                    Dim yH As Byte = CByte((tinggiBaru >> 8) And &HFF)

                    Dim header As Byte() = {
                        &H1B, &H61, &H0,           ' ESC a 0  — rata kiri (sudah ditengahkan dari bmp padding)
                        &H1D, &H76, &H30, &H0,     ' GS v 0 m=0
                        xL, xH, yL, yH              ' dimensi
                    }

                    Dim total(header.Length + imgBytes.Length - 1) As Byte
                    Array.Copy(header, 0, total, 0, header.Length)
                    Array.Copy(imgBytes, 0, total, header.Length, imgBytes.Length)

                    _buffer.AddRange(total)
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

#End Region

End Class
