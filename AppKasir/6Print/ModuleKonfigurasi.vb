Imports System.IO
Module ModuleKonfigurasi
    ' Struk printer thermal
    Public LblPrinterStrukString As String
    Public jenisprinter As String
    Public TxtMAjuString As Integer
    Public TxtMundurString As Integer
    Public TxtLebarString As Integer
    Public TxtpikselString As Integer
    Public TxtBatasKiriString As Integer
    Public TxtJarakString As Integer
    Public TxtPanjangString As Integer
    Public lblPortNameString As String
    Public lblBaudRateString As Integer
    Public lblDataBitsString As Integer
    Public CmbPortCashString As String
    Public CmbCodeCashString As String
    Public CmbModelStrukString As String
    Public CmbFNAmaString As String
    Public CmbFKetString As String
    Public CmbFIsiString As String
    Public CmbFFootString As String
    Public CmbUNamaString As Integer
    Public CmbUKetString As Integer
    Public CmbUIsiString As Integer
    Public CmbUFootString As Integer
    Public CBPotongChecked As Boolean

    ' Dot matrix
    Public PrinterDot As String
    Public LebarDot As Integer
    Public BatasKiriDot As Integer
    Public JarakBarisDot As Integer
    Public FontJudulDot As String
    Public FontIsiDot As String
    Public UkuranFontJudul As Integer
    Public UkuranFontIsi As Integer
    Public TinggiDot As Integer

    ' Flag untuk mencegah load ulang
    Public KonfigurasiSudahDibaca As Boolean = False


    Public Sub LoadKonfigurasiPrinter(Optional ByVal path As String = "printer.ini")
        If ModuleKonfigurasi.KonfigurasiSudahDibaca Then Exit Sub ' hanya load sekali

        If Not File.Exists(path) Then
            MessageBox.Show("File konfigurasi printer tidak ditemukan: " & path)
            Exit Sub
        End If

        Dim config = File.ReadAllLines(path).Where(Function(l) l.Contains("="c)).
            Select(Function(l) l.Split({"="c}, 2)).ToDictionary(Function(k) k(0).Trim(), Function(v) v(1).Trim())

        LblPrinterStrukString = GetStr(config, "PrinterPos")
        jenisprinter = GetStr(config, "JenisPrinterJual")
        TxtMAjuString = GetInt(config, "Maju")
        TxtMundurString = GetInt(config, "Mundur")
        TxtLebarString = GetInt(config, "Lebar")
        TxtpikselString = GetInt(config, "Piksel")
        TxtBatasKiriString = GetInt(config, "BatasKiri")
        TxtJarakString = GetInt(config, "Jarak")
        TxtPanjangString = GetInt(config, "Panjang")
        lblPortNameString = GetStr(config, "PortName")
        lblBaudRateString = GetInt(config, "BaudRate")
        lblDataBitsString = GetInt(config, "DataBits")
        CmbPortCashString = GetStr(config, "PortCashDraw")
        CmbCodeCashString = GetStr(config, "CodeCashDraw")
        CmbModelStrukString = GetStr(config, "ModelStruk")
        CmbFNAmaString = GetStr(config, "FontNama")
        CmbFKetString = GetStr(config, "FontKet")
        CmbFIsiString = GetStr(config, "FontIsi")
        CmbFFootString = GetStr(config, "FOntFoot")
        CmbUNamaString = GetInt(config, "FontUNama")
        CmbUKetString = GetInt(config, "FontUKet")
        CmbUIsiString = GetInt(config, "FontUIsi")
        CmbUFootString = GetInt(config, "FontUFoot")
        CBPotongChecked = GetBool(config, "Potongkertas")

        PrinterDot = GetStr(config, "PrinterDot")
        LebarDot = GetInt(config, "LebarDot")
        BatasKiriDot = GetInt(config, "BatasKiriDot")
        JarakBarisDot = GetInt(config, "JarakBarisDot")
        FontJudulDot = GetStr(config, "FontJudulDot")
        FontIsiDot = GetStr(config, "FontIsiDot")
        UkuranFontJudul = GetInt(config, "UkuranFontJudul")
        UkuranFontIsi = GetInt(config, "UkuranFontIsi")
        TinggiDot = GetInt(config, "TinggiDot")

        KonfigurasiSudahDibaca = True

    End Sub

    Private Function GetStr(dict As Dictionary(Of String, String), key As String) As String
        Return If(dict.ContainsKey(key), dict(key), "")
    End Function

    Private Function GetInt(dict As Dictionary(Of String, String), key As String) As Integer
        Return If(dict.ContainsKey(key) AndAlso IsNumeric(dict(key)), CInt(dict(key)), 0)
    End Function

    Private Function GetBool(dict As Dictionary(Of String, String), key As String) As Boolean
        Return If(dict.ContainsKey(key), dict(key).ToLower() = "true", False)
    End Function
End Module


'Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'    LoadKonfigurasiPrinter()

'    ' Akses langsung:
'    LabelPrinter.Text = ModuleKonfigurasi.LblPrinterStrukString
'    TextLebar.Text = ModuleKonfigurasi.TxtLebarString.ToString()
'    CheckPotongKertas.Checked = ModuleKonfigurasi.CBPotongChecked
'End Sub

