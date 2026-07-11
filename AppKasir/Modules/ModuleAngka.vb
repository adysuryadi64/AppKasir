Imports System.Globalization
Imports System.Windows.Forms

''' <summary>
''' ModuleAngka — Standar penanganan angka untuk AppKasir.
''' Semua parsing, formatting, dan setup DGV terpusat di sini.
''' Referensi: .kiro/steering/standar-input-angka.md
''' </summary>
Module ModuleAngka

    ' ─────────────────────────────────────────────────────────────────────────
    ' KONSTANTA CULTURE
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>Culture Indonesia: titik ribuan, koma desimal (1.500,50)</summary>
    Public ReadOnly CultureID As CultureInfo = CultureInfo.GetCultureInfo("id-ID")

    ''' <summary>InvariantCulture: titik desimal (1500.50) — dipakai untuk parse internal</summary>
    Public ReadOnly CultureInv As CultureInfo = CultureInfo.InvariantCulture

    ' ─────────────────────────────────────────────────────────────────────────
    ' PARSE
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Parse angka dari berbagai format input user atau nilai cell/DB.
    ''' Handle semua format berikut tanpa exception:
    '''   Bulat    : "1500000", "1500"
    '''   Indonesia: "1.500.000", "1.500,50", "1,5"
    '''   US/EN    : "1,500,000", "1,500.50", "1.5"
    '''   Negatif  : "-1500", "-1.500,50", "-1,500.50"
    '''   Kosong   : Nothing, DBNull, "", " " → 0D
    ''' Tidak pernah throw exception.
    ''' </summary>
    Public Function ParseDecimal(ByVal value As Object) As Decimal
        ' Guard: null / DBNull / kosong
        If value Is Nothing OrElse IsDBNull(value) Then Return 0D

        ' Konversi numerik langsung tanpa lewat string — akurat 100%
        ' Ini menghindari masalah culture saat Double/Single.ToString()
        Select Case value.GetType()
            Case GetType(Decimal) : Return CDec(value)
            Case GetType(Integer) : Return CDec(CInt(value))
            Case GetType(Long)    : Return CDec(CLng(value))
            Case GetType(Short)   : Return CDec(CShort(value))
            Case GetType(Byte)    : Return CDec(CByte(value))
            Case GetType(Double)
                ' Double bisa kehilangan presisi saat ke Decimal, tapi cukup untuk keuangan
                Return CDec(CDbl(value))
            Case GetType(Single)
                Return CDec(CSng(value))
        End Select

        ' Untuk String dan tipe lain — normalisasi format dulu
        Dim s As String = value.ToString().Trim()
        If String.IsNullOrEmpty(s) Then Return 0D

        ' Tangani tanda negatif di depan
        Dim isNegative As Boolean = s.StartsWith("-")
        If isNegative Then s = s.Substring(1).Trim()

        ' Hapus simbol mata uang dan spasi jika ada (Rp, $, €, dll)
        s = System.Text.RegularExpressions.Regex.Replace(s, "[^\d.,]", "").Trim()
        If String.IsNullOrEmpty(s) Then Return 0D

        Dim hasComma As Boolean = s.Contains(",")
        Dim hasDot   As Boolean = s.Contains(".")
        Dim normalized As String

        If hasComma AndAlso hasDot Then
            ' Ada keduanya — tentukan mana ribuan, mana desimal
            ' Pemisah ribuan selalu di kiri, desimal di kanan
            If s.IndexOf("."c) < s.IndexOf(","c) Then
                ' Format Indonesia: "1.500,50" → hapus titik, koma jadi titik
                normalized = s.Replace(".", "").Replace(",", ".")
            Else
                ' Format US/EN: "1,500.50" → hapus koma
                normalized = s.Replace(",", "")
            End If

        ElseIf hasComma AndAlso Not hasDot Then
            ' Hanya koma — bisa desimal atau ribuan.
            ' Aturan: pemisah ribuan selalu memisahkan tepat 3 digit ("1,500" / "1,500,000").
            ' Jika ada lebih dari 1 koma → pasti ribuan ("1,500,000").
            ' Jika ada tepat 1 koma dan bagian kanannya tepat 3 digit → ribuan ("1,500").
            ' Selain itu → desimal ("1,5" / "1,50" / "1,5000" / ",5").
            Dim parts() As String = s.Split(","c)
            Dim isRibuan As Boolean = False
            If parts.Length > 2 Then
                isRibuan = True   ' lebih dari satu koma → pasti ribuan
            ElseIf parts.Length = 2 Then
                ' Tepat satu koma: ribuan hanya jika bagian kanan persis 3 digit
                isRibuan = (parts(1).Length = 3 AndAlso parts(1).All(Function(c) Char.IsDigit(c)))
            End If

            If isRibuan Then
                ' "1,500" / "1,500,000" → ribuan, hapus koma
                normalized = s.Replace(",", "")
            Else
                ' "1,5" / "1,50" / "1,5000" / ",5" / "1," → desimal
                Dim temp As String = If(s.StartsWith(","), "0" & s, s)
                If temp.EndsWith(",") Then temp = temp.TrimEnd(","c)
                normalized = temp.Replace(",", ".")
            End If

        ElseIf hasDot AndAlso Not hasComma Then
            ' Hanya titik — bisa desimal atau ribuan.
            ' Aturan: pemisah ribuan selalu memisahkan tepat 3 digit ("1.500" / "1.500.000").
            ' Jika ada lebih dari 1 titik → pasti ribuan ("1.500.000").
            ' Jika ada tepat 1 titik dan bagian kanannya tepat 3 digit → ribuan ("1.500").
            ' Selain itu → desimal ("1.5" / "1.50" / "1.5000" / "7801.0000" / ".5").
            ' Catatan: nilai DB decimal(15,4) seperti "7801.0000" punya 4 digit → desimal ✓
            Dim parts() As String = s.Split("."c)
            Dim isRibuan As Boolean = False
            If parts.Length > 2 Then
                isRibuan = True   ' lebih dari satu titik → pasti ribuan
            ElseIf parts.Length = 2 Then
                ' Tepat satu titik: ribuan hanya jika bagian kanan persis 3 digit
                isRibuan = (parts(1).Length = 3 AndAlso parts(1).All(Function(c) Char.IsDigit(c)))
            End If

            If isRibuan Then
                ' "1.500" / "1.500.000" → ribuan, hapus titik
                normalized = s.Replace(".", "")
            Else
                ' "1.5" / "1.50" / "1.5000" / "7801.0000" / ".5" / "1." → desimal, biarkan
                normalized = If(s.StartsWith("."), "0" & s, s)
                If normalized.EndsWith(".") Then normalized = normalized.TrimEnd("."c)
            End If

        Else
            ' Tidak ada pemisah: angka bulat biasa
            normalized = s
        End If

        Dim result As Decimal = 0D
        Decimal.TryParse(normalized, NumberStyles.Any, CultureInv, result)
        Return If(isNegative, -result, result)
    End Function

    ''' <summary>
    ''' Parse Integer — untuk field yang tidak boleh desimal.
    ''' Contoh: isi satuan, jumlah item, nomor urut.
    ''' Jika hasil &lt;= 0, kembalikan defaultValue.
    ''' </summary>
    Public Function ParseInteger(ByVal value As Object,
                                 Optional defaultValue As Integer = 0) As Integer
        Dim d As Decimal = ParseDecimal(value)
        Dim i As Integer = CInt(Math.Truncate(d))
        Return If(i > 0, i, defaultValue)
    End Function

    ' ─────────────────────────────────────────────────────────────────────────
    ' FORMAT DISPLAY
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Format Decimal ke tampilan Indonesia dengan desimal jika ada.
    ''' Contoh: 1500.5 → "1.500,5" | 1500 → "1.500"
    ''' </summary>
    Public Function FormatAngka(value As Decimal) As String
        Return value.ToString("#,0.##", CultureID)
    End Function

    ''' <summary>
    ''' Format Decimal ke Rupiah bulat tanpa desimal.
    ''' Contoh: 1500500 → "1.500.500"
    ''' </summary>
    Public Function FormatRupiah(value As Decimal) As String
        Return value.ToString("N0", CultureID)
    End Function

    ''' <summary>
    ''' Format Decimal ke Rupiah dengan prefix "Rp. ".
    ''' Contoh: 1500500 → "Rp. 1.500.500"
    ''' </summary>
    Public Function FormatRupiahLabel(value As Decimal) As String
        Return "Rp. " & value.ToString("N0", CultureID)
    End Function

    ''' <summary>
    ''' Format Decimal untuk TextBox INPUT — plain tanpa format ribuan, tanpa trailing zero.
    ''' Contoh: 1500.5 → "1500.5" | 1500 → "1500" | 400000.00 → "400000"
    ''' </summary>
    Public Function FormatUntukInput(value As Decimal) As String
        Return value.ToString("0.##", CultureInv)
    End Function

    ' ─────────────────────────────────────────────────────────────────────────
    ' SETUP DATAGRIDVIEW
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Terapkan format angka Indonesia ke kolom-kolom DGV secara seragam.
    ''' Format: #,0.## — tampilkan desimal hanya jika ada (21.000 atau 21.000,25).
    ''' Cocok untuk semua kolom angka: harga, total, qty, stok, nominal.
    ''' Gunakan ini sebagai default untuk semua kolom angka karena aman untuk harga pecahan.
    ''' Panggil sekali saat SetupGrid() atau form Load.
    ''' Kolom yang tidak ditemukan di DGV akan dilewati tanpa error.
    ''' </summary>
    Public Sub TerapkanFormatKolomAngka(dgv As DataGridView,
                                        ParamArray namaKolom() As String)
        For Each nama As String In namaKolom
            If dgv.Columns.Contains(nama) Then
                With dgv.Columns(nama).DefaultCellStyle
                    .Format         = "#,0.####"
                    .Alignment      = DataGridViewContentAlignment.MiddleRight
                    .FormatProvider = CultureID
                End With
            End If
        Next
    End Sub

    ''' <summary>
    ''' Terapkan format Rupiah bulat (tanpa desimal) ke kolom-kolom DGV.
    ''' Format: N0 — selalu bulat, desimal dipotong (21.000,25 → 21.000).
    ''' HANYA gunakan jika kolom dipastikan tidak pernah mengandung nilai pecahan,
    ''' misalnya: jumlah nota, nomor urut, atau field integer by design.
    ''' Untuk harga dan total yang bisa pecahan, gunakan TerapkanFormatKolomAngka.
    ''' </summary>
    Public Sub TerapkanFormatKolomBulat(dgv As DataGridView,
                                        ParamArray namaKolom() As String)
        For Each nama As String In namaKolom
            If dgv.Columns.Contains(nama) Then
                With dgv.Columns(nama).DefaultCellStyle
                    .Format         = "N0"
                    .Alignment      = DataGridViewContentAlignment.MiddleRight
                    .FormatProvider = CultureID
                End With
            End If
        Next
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────
    ' HELPER KEYPRESS
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Handler KeyPress standar untuk TextBox yang menerima input Decimal.
    ''' Izinkan: angka, backspace, titik, koma. Cegah dua pemisah desimal.
    ''' Cara pakai: panggil dari event KeyPress TextBox.
    ''' </summary>
    Public Sub KeyPressDecimal(sender As Object, e As KeyPressEventArgs)
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return

        ' Izinkan kontrol (backspace, delete, dll)
        If Char.IsControl(e.KeyChar) Then Return

        ' Izinkan angka
        If Char.IsDigit(e.KeyChar) Then Return

        ' Izinkan satu pemisah desimal (titik atau koma)
        If e.KeyChar = "."c OrElse e.KeyChar = ","c Then
            If txt.Text.Contains(".") OrElse txt.Text.Contains(",") Then
                e.Handled = True  ' sudah ada pemisah desimal
            End If
            Return
        End If

        ' Izinkan tanda minus hanya di posisi pertama
        If e.KeyChar = "-"c AndAlso txt.SelectionStart = 0 AndAlso Not txt.Text.Contains("-") Then
            Return
        End If

        ' Semua karakter lain ditolak
        e.Handled = True
    End Sub

    ''' <summary>
    ''' Handler KeyPress standar untuk TextBox yang hanya menerima Integer positif.
    ''' Izinkan: angka dan backspace saja.
    ''' </summary>
    Public Sub KeyPressInteger(sender As Object, e As KeyPressEventArgs)
        If Not (Char.IsDigit(e.KeyChar) OrElse Char.IsControl(e.KeyChar)) Then
            e.Handled = True
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────
    ' HELPER DB READER
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Baca nilai dari MySqlDataReader dengan aman — handle DBNull dan konversi tipe.
    ''' Tidak pernah throw exception; kembalikan defaultValue jika kolom null atau tidak ada.
    ''' Contoh:
    '''   Dim harga As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
    '''   Dim nama  As String  = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_BARANG", "")
    '''   Dim isi   As Integer = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)
    ''' </summary>
    Public Function SafeGetValue(Of T)(rd As MySqlDataReader,
                                       columnName As String,
                                       defaultValue As T) As T
        If rd Is Nothing Then Return defaultValue
        Try
            Dim ordinal As Integer = rd.GetOrdinal(columnName)
            If rd.IsDBNull(ordinal) Then Return defaultValue
            Dim value As Object = rd.GetValue(ordinal)
            If IsDBNull(value) Then Return defaultValue
            If TypeOf value Is T Then Return CType(value, T)
            Return CType(Convert.ChangeType(value, GetType(T)), T)
        Catch
            Return defaultValue
        End Try
    End Function

End Module
