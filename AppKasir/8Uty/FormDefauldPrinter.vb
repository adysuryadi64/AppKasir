Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Ports
Imports System.Management


Public Class FormDefauldPrinter
    Private isChecked As Boolean

    Public Sub CekPrinter()
        If PrinterSettings.InstalledPrinters.Count = 0 Then
            LblPrinter.Text = "No printer installed"
            Exit Sub
        End If
    End Sub

    Public Sub TampilPrinter()
        Using prntDoc As New PrintDocument
            Try
                Dim strInstalledPrinters As String
                For Each strInstalledPrinters In PrinterSettings.InstalledPrinters
                    LblPrinter.Text = prntDoc.PrinterSettings.PrinterName
                    CmbPrinter.Items.Add(strInstalledPrinters)
                    CmbPrinter.Text = prntDoc.PrinterSettings.PrinterName
                Next strInstalledPrinters
            Catch ex As Exception
                ' Tangani kesalahan di sini, misalnya:
                MessageBox.Show("Terjadi kesalahan saat mencoba mengakses daftar printer: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub FormDefauldPrinter_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Dim borderWidth As Integer = 5

        Using borderPen As New Pen(Color.Blue, borderWidth)
            Using g As Graphics = CreateGraphics()
                g.DrawRectangle(borderPen, New Rectangle(0, 0, Width - 1, Height - 1))
            End Using
        End Using

        CekPrinter()
        TampilPrinter()
        Cekportthermal()
        Tampilfont()
        Ambildata()
        AmbilNilaiPrinterJualDariIniFile()
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub Panel1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel1.Paint
        Using goldPen As New Pen(Color.Gold, 10)
            e.Graphics.DrawRectangle(goldPen, 0, 0, Width - 1, Height - 1)
        End Using
    End Sub

    Public Function SetDefaultPrinter(ByVal strPrinterName As String) As Boolean
        Dim originalPrinter As String

        Using prntDoc As New PrintDocument()
            originalPrinter = prntDoc.PrinterSettings.PrinterName
        End Using

        Try
            Dim query As String = "SELECT * FROM Win32_Printer WHERE Name = '" & strPrinterName.Replace("\", "\\") & "'"
            Using searcher As New ManagementObjectSearcher(query)
                Using collection As ManagementObjectCollection = searcher.Get()
                    If collection.Count = 0 Then Return False

                    For Each printer As ManagementObject In collection
                        printer.InvokeMethod("SetDefaultPrinter", Nothing)
                    Next
                End Using
            End Using

            Using prntDoc As New PrintDocument()
                If prntDoc.PrinterSettings.PrinterName = strPrinterName AndAlso prntDoc.PrinterSettings.IsValid Then
                    Return True
                End If
            End Using
        Catch ex As Exception
            ' Optional: log error
        End Try

        Return False
    End Function


    Private Sub BtnSet_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSet.Click
        If String.IsNullOrWhiteSpace(CmbPrinter.Text) Then
            MessageBox.Show("Pilih nama printer terlebih dahulu!", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        If SetDefaultPrinter(CmbPrinter.Text) Then
            MessageBox.Show("Printer default diubah menjadi: " & CmbPrinter.Text, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Gagal mengatur printer default. Pastikan nama printer '" & CmbPrinter.Text & "' benar dan terpasang.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub BtnKeluar_Click_1(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click
        Close()
    End Sub

    Public Sub Cekportthermal()

        ' Deklarasi objek port
        Dim ports As String() = SerialPort.GetPortNames()

        ' Isi ComboBox dengan daftar port
        For Each port As String In ports
            'CmbPort.Items.Add(port)
            CmbPortCash.Items.Add(port)
        Next

        For Each portName As String In System.Drawing.Printing.PrinterSettings.InstalledPrinters
            CmbJenisPrinterThermal.Items.Add(portName)
            CmbJenisPrinterDot.Items.Add(portName)
        Next
    End Sub

    Public Sub Aturnilaiport()

        Try
            SerialPort1.PortName = "COM1" 'Ganti COM1 dengan port serial yang digunakan.
            SerialPort1.BaudRate = 9600 'Atur baud rate yang sesuai.
            SerialPort1.Parity = Parity.None 'Atur parity yang sesuai.
            SerialPort1.DataBits = 8 'Atur data bits yang sesuai.
            SerialPort1.StopBits = StopBits.One 'Atur stop bits yang sesuai.
            SerialPort1.Open() 'Buka port serial.
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Private Sub CutPaper()
        Dim cutCommand As String = Chr(&H1B) & "m" 'Perintah untuk memotong kertas pada printer thermal.
        SerialPort1.WriteLine(cutCommand)
    End Sub
    Private Sub DetectCutLocation()
        'Kode untuk mendeteksi lokasi potong kertas menggunakan sensor.
        'Jika sensor mendeteksi lokasi potong, panggil fungsi CutPaper() untuk memotong kertas.
        CutPaper()
    End Sub
    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        Dim textToPrint As String = "Ini adalah teks yang akan dicetak pada printer thermal."
        SerialPort1.WriteLine(textToPrint)
    End Sub

    Private Sub CmbJenisPrinterThermal_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbJenisPrinterThermal.SelectedIndexChanged
        ' Memeriksa apakah ada elemen yang terpilih
        If CmbJenisPrinterThermal.SelectedItem IsNot Nothing Then
            ' Mendapatkan nama port printer dari ComboBox
            Dim portName As String = CmbJenisPrinterThermal.SelectedItem.ToString()

            ' Membuat instance kelas SerialPort dengan menggunakan Using
            Using serialPort As New SerialPort(portName)
                ' Mengatur nilai BaudRate, Parity, dan DataBits pada kelas SerialPort
                Dim baudRate As Integer = serialPort.BaudRate
                Dim parity As String = serialPort.Parity.ToString()
                Dim dataBits As Integer = serialPort.DataBits
                CmbPort.Items.Clear()

                ' Tambahkan daftar port yang tersedia ke dalam ComboBox
                For Each port As String In My.Computer.Ports.SerialPortNames
                    CmbPort.Items.Add(port)
                Next

                ' Menampilkan informasi pada kontrol Label
                lblPortName.Text = portName
                lblBaudRate.Text = baudRate.ToString()
                lblParity.Text = parity
                lblDataBits.Text = dataBits.ToString()
            End Using
        End If
    End Sub

    Public Sub Tampilfont()

        CmbFNAma.Items.Clear()
        CmbFKet.Items.Clear()
        CmbFIsi.Items.Clear()
        CmbFFoot.Items.Clear()

        For Each font As FontFamily In FontFamily.Families
            CmbFNAma.Items.Add(font.Name)
            CmbFKet.Items.Add(font.Name)
            CmbFIsi.Items.Add(font.Name)
            CmbFFoot.Items.Add(font.Name)

            CmbFontJuduDot.Items.Add(font.Name)
            CmbFontIsiDot.Items.Add(font.Name)
        Next
        For i As Integer = 8 To 24 Step 1
            CmbUNama.Items.Add(i)
            CmbUKet.Items.Add(i)
            CmbUIsi.Items.Add(i)
            CmbUFoot.Items.Add(i)

            CmbUkuranJuduDot.Items.Add(i)
            CmbUkuranIsiDot.Items.Add(i)
        Next
    End Sub

    Public Sub Ambildata()
        Dim filePath As String = "printer.ini"

        ' Check if the file exists
        If File.Exists(filePath) Then
            ' File exists, read the values from the file
            Dim lines As String() = File.ReadAllLines(filePath)

            ' Define expected keys as List(Of String)
            Dim expectedKeys As New List(Of String) From {
                "PrinterPos", "PortPrinter", "Maju", "Mundur", "Panjang", "Lebar", "Piksel",
                "BatasKiri", "Jarak", "PortName", "BaudRate", "Parity", "DataBits",
                "PortCashDraw", "CodeCashDraw", "Potongkertas", "ModelStruk",
                "FontNama", "FontKet", "FontIsi", "FOntFoot",
                "FontUNama", "FontUKet", "FontUIsi", "FontUFoot",
                "StatusComp", "JenisPrinterJual", "JenisPrinterLap",
                "PrinterDot", "LebarDot", "TinggiDot", "BatasKiriDot", "JarakBarisDot",
                "FontJudulDot", "FontIsiDot", "UkuranFontJudul", "UkuranFontIsi"
            }

            ' Initialize a dictionary to store key-value pairs
            Dim settings As New Dictionary(Of String, String)

            ' Populate the dictionary with existing settings
            For Each line As String In lines
                Dim parts As String() = line.Split("="c)
                If parts.Length = 2 AndAlso expectedKeys.IndexOf(parts(0)) <> -1 Then
                    settings(parts(0)) = parts(1)
                End If
            Next


            ' Update application settings with values from dictionary
            LblPrinterStruk.Text = GetSettingOrDefault(settings, "PrinterPos", "Default Printer")
            CmbJenisPrinterThermal.Text = GetSettingOrDefault(settings, "PrinterPos", "Default Printer")


            'CmbJenisPrinterThermal.Text = GetSettingOrDefault(settings, "PrinterPos", "Default Printer")
            CmbPort.Text = GetSettingOrDefault(settings, "PortPrinter", "COM1")
            TxtMAju.Text = GetSettingOrDefault(settings, "Maju", "0")
            TxtMundur.Text = GetSettingOrDefault(settings, "Mundur", "0")
            TxtPanjang.Text = GetSettingOrDefault(settings, "Panjang", "0")
            TxtLebar.Text = GetSettingOrDefault(settings, "Lebar", "80")
            Txtpiksel.Text = GetSettingOrDefault(settings, "Piksel", "100")
            TxtBatasKiri.Text = GetSettingOrDefault(settings, "BatasKiri", "0")
            TxtJarak.Text = GetSettingOrDefault(settings, "Jarak", "2")
            lblPortName.Text = GetSettingOrDefault(settings, "PortName", "")
            lblBaudRate.Text = GetSettingOrDefault(settings, "BaudRate", "")
            lblParity.Text = GetSettingOrDefault(settings, "Parity", "")
            lblDataBits.Text = GetSettingOrDefault(settings, "DataBits", "")
            CmbPortCash.Text = GetSettingOrDefault(settings, "PortCashDraw", "")
            CmbCodeCash.Text = GetSettingOrDefault(settings, "CodeCashDraw", "")
            CBPotong.Checked = GetSettingOrDefault(settings, "Potongkertas", "False").ToLower() = "true"
            CmbModelStruk.Text = GetSettingOrDefault(settings, "ModelStruk", "Model 1 Lengkap")
            CmbFNAma.Text = GetSettingOrDefault(settings, "FontNama", "Century")
            CmbFKet.Text = GetSettingOrDefault(settings, "FontKet", "Arial Narrow")
            CmbFIsi.Text = GetSettingOrDefault(settings, "FontIsi", "Arial Narrow")
            CmbFFoot.Text = GetSettingOrDefault(settings, "FOntFoot", "Arial Narrow")
            CmbUNama.Text = GetSettingOrDefault(settings, "FontUNama", "14")
            CmbUKet.Text = GetSettingOrDefault(settings, "FontUKet", "10")
            CmbUIsi.Text = GetSettingOrDefault(settings, "FontUIsi", "10")
            CmbUFoot.Text = GetSettingOrDefault(settings, "FontUFoot", "10")
            CmbStatusKomputer.SelectedIndex = GetSettingOrDefaultIndex(settings, "StatusComp", "Server")
            CmbJenisPrinterJual.SelectedIndex = GetSettingOrDefaultIndex(settings, "JenisPrinterJual", "Printer Thermal")
            CmbJenisLap.SelectedIndex = GetSettingOrDefaultIndex(settings, "JenisPrinterLap", "Printer Ink Tank")

            LblPrinterTersimpanDot.Text = GetSettingOrDefault(settings, "PrinterDot", "Default Printer")
            CmbJenisPrinterDot.Text = GetSettingOrDefault(settings, "PrinterDot", "Default Printer")

            'CmbJenisPrinterDot.Text = GetSettingOrDefault(settings, "PrinterDot", "Default Printer")
            TxtLebarKertasDot.Text = GetSettingOrDefault(settings, "LebarDot", "27")
            TxtTinggiDot.Text = GetSettingOrDefault(settings, "TinggiDot", "7")
            TxtBatasKiriDot.Text = GetSettingOrDefault(settings, "BatasKiriDot", "0")
            TxtJarakBarisDot.Text = GetSettingOrDefault(settings, "JarakBarisDot", "2")
            CmbFontJuduDot.Text = GetSettingOrDefault(settings, "FontJudulDot", "Consolas")
            CmbFontIsiDot.Text = GetSettingOrDefault(settings, "FontIsiDot", "Consolas")
            CmbUkuranJuduDot.Text = GetSettingOrDefault(settings, "UkuranFontJudul", "12")
            CmbUkuranIsiDot.Text = GetSettingOrDefault(settings, "UkuranFontIsi", "9")

            ' Update the file with default values for missing settings
            Using writer As New StreamWriter(filePath, True)
                For Each key As String In expectedKeys
                    If Not settings.ContainsKey(key) Then
                        Select Case key
                            Case "PrinterPos"
                                writer.WriteLine("PrinterPos=Default Printer")
                            Case "PortPrinter"
                                writer.WriteLine("PortPrinter=COM1")
                            Case "Maju"
                                writer.WriteLine("Maju=0")
                            Case "Mundur"
                                writer.WriteLine("Mundur=0")
                            Case "Panjang"
                                writer.WriteLine("Panjang=0")
                            Case "Lebar"
                                writer.WriteLine("Lebar=80")
                            Case "Piksel"
                                writer.WriteLine("Piksel=100")
                            Case "BatasKiri"
                                writer.WriteLine("BatasKiri=0")
                            Case "Jarak"
                                writer.WriteLine("Jarak=2")
                            Case "PortName"
                                writer.WriteLine("PortName=")
                            Case "BaudRate"
                                writer.WriteLine("BaudRate=")
                            Case "Parity"
                                writer.WriteLine("Parity=")
                            Case "DataBits"
                                writer.WriteLine("DataBits=")
                            Case "PortCashDraw"
                                writer.WriteLine("PortCashDraw=")
                            Case "CodeCashDraw"
                                writer.WriteLine("CodeCashDraw=")
                            Case "Potongkertas"
                                writer.WriteLine("Potongkertas=False")
                            Case "ModelStruk"
                                writer.WriteLine("ModelStruk=Model 1 Lengkap")
                            Case "FontNama"
                                writer.WriteLine("FontNama=Century")
                            Case "FontKet"
                                writer.WriteLine("FontKet=Arial Narrow")
                            Case "FontIsi"
                                writer.WriteLine("FontIsi=Arial Narrow")
                            Case "FOntFoot"
                                writer.WriteLine("FOntFoot=Arial Narrow")
                            Case "FontUNama"
                                writer.WriteLine("FontUNama=14")
                            Case "FontUKet"
                                writer.WriteLine("FontUKet=10")
                            Case "FontUIsi"
                                writer.WriteLine("FontUIsi=10")
                            Case "FontUFoot"
                                writer.WriteLine("FontUFoot=10")
                            Case "StatusComp"
                                writer.WriteLine("StatusComp=Server")
                            Case "JenisPrinterJual"
                                writer.WriteLine("JenisPrinterJual=Printer Thermal")
                            Case "JenisPrinterLap"
                                writer.WriteLine("JenisPrinterLap=Printer Ink Tank")

                            Case "PrinterDot"
                                writer.WriteLine("PrinterDot=Default Printer")
                            Case "LebarDot"
                                writer.WriteLine("LebarDot=27")
                            Case "TinggiDot"
                                writer.WriteLine("TinggiDot=7")
                            Case "BatasKiriDot"
                                writer.WriteLine("BatasKiriDot=0")
                            Case "JarakBarisDot"
                                writer.WriteLine("JarakBarisDot=2")
                            Case "FontJudulDot"
                                writer.WriteLine("FontJudulDot=Consolas")
                            Case "FontIsiDot"
                                writer.WriteLine("FontIsiDot=Consolas")
                            Case "UkuranFontJudul"
                                writer.WriteLine("UkuranFontJudul=12")
                            Case "UkuranFontIsi"
                                writer.WriteLine("UkuranFontIsi=9")
                        End Select
                    End If
                Next
            End Using
        Else
            ' File doesn't exist, create a new file and write all default values
            Using writer As New StreamWriter(filePath)
                writer.WriteLine("PrinterPos=Default Printer")
                writer.WriteLine("PortPrinter=COM1")
                writer.WriteLine("Maju=0")
                writer.WriteLine("Mundur=0")
                writer.WriteLine("Panjang=0")
                writer.WriteLine("Lebar=80")
                writer.WriteLine("Piksel=100")
                writer.WriteLine("BatasKiri=0")
                writer.WriteLine("Jarak=2")
                writer.WriteLine("PortName=")
                writer.WriteLine("BaudRate=")
                writer.WriteLine("Parity=")
                writer.WriteLine("DataBits=")
                writer.WriteLine("PortCashDraw=")
                writer.WriteLine("CodeCashDraw=")
                writer.WriteLine("Potongkertas=False")
                writer.WriteLine("ModelStruk=Model 1 Lengkap")
                writer.WriteLine("FontNama=Century")
                writer.WriteLine("FontKet=Arial Narrow")
                writer.WriteLine("FontIsi=Arial Narrow")
                writer.WriteLine("FOntFoot=Arial Narrow")
                writer.WriteLine("FontUNama=14")
                writer.WriteLine("FontUKet=10")
                writer.WriteLine("FontUIsi=10")
                writer.WriteLine("FontUFoot=10")
                writer.WriteLine("StatusComp=Server")
                writer.WriteLine("JenisPrinterJual=Printer Thermal")
                writer.WriteLine("JenisPrinterLap=Printer Ink Tank")

                writer.WriteLine("PrinterDot=Default Printer")
                writer.WriteLine("LebarDot=27")
                writer.WriteLine("TinggiDot=7")
                writer.WriteLine("BatasKiriDot=0")
                writer.WriteLine("JarakBarisDot=2")
                writer.WriteLine("FontJudulDot=Consolas")
                writer.WriteLine("FontIsiDot=Consolas")
                writer.WriteLine("UkuranFontJudul=12")
                writer.WriteLine("UkuranFontIsi=9")
            End Using

            ' Set default values in the form controls
            LblPrinterStruk.Text = "Default Printer"
            ' Find an available thermal printer
            Dim thermalPrinter As String = FindThermalPrinter()
            If Not String.IsNullOrEmpty(thermalPrinter) Then
                CmbJenisPrinterThermal.Text = thermalPrinter
            Else
                CmbJenisPrinterThermal.Text = LblPrinter.Text
            End If
            CmbPort.Text = "COM1"
            TxtMAju.Text = "0"
            TxtMundur.Text = "0"
            TxtPanjang.Text = "0"
            TxtLebar.Text = "80"
            Txtpiksel.Text = "100"
            TxtBatasKiri.Text = "0"
            TxtJarak.Text = "2"
            lblPortName.Text = ""
            lblBaudRate.Text = ""
            lblParity.Text = ""
            lblDataBits.Text = ""
            CmbPortCash.Text = ""
            CmbCodeCash.Text = ""
            CBPotong.Checked = False
            CmbModelStruk.Text = "Model 1 Lengkap"
            CmbFNAma.Text = "Century"
            CmbFKet.Text = "Arial Narrow"
            CmbFIsi.Text = "Arial Narrow"
            CmbFFoot.Text = "Arial Narrow"
            CmbUNama.Text = "14"
            CmbUKet.Text = "10"
            CmbUIsi.Text = "10"
            CmbUFoot.Text = "10"
            CmbStatusKomputer.SelectedIndex = 0
            CmbJenisPrinterJual.SelectedIndex = 0
            CmbJenisLap.SelectedIndex = 1

            LblPrinterTersimpanDot.Text = "Default Printer"
            Dim DotMatrikPrinter As String = FindDotMatrixPrinter()
            If Not String.IsNullOrEmpty(DotMatrikPrinter) Then
                CmbJenisPrinterDot.Text = DotMatrikPrinter
            Else
                CmbJenisPrinterDot.Text = LblPrinter.Text
            End If
            TxtLebarKertasDot.Text = "27"
            TxtTinggiDot.Text = "7"
            TxtBatasKiriDot.Text = "0"
            TxtJarakBarisDot.Text = "2"
            CmbFontJuduDot.Text = "Consolas"
            CmbFontIsiDot.Text = "Consolas"
            CmbUkuranJuduDot.Text = "12"
            CmbUkuranIsiDot.Text = "9"
        End If
    End Sub

    Private Function GetSettingOrDefault(ByVal settings As Dictionary(Of String, String), ByVal key As String, ByVal defaultValue As String) As String
        If settings.ContainsKey(key) Then
            Return settings(key)
        Else
            Return defaultValue
        End If
    End Function

    Private Function GetSettingOrDefaultIndex(ByVal settings As Dictionary(Of String, String), ByVal key As String, ByVal defaultItem As String) As Integer
        If settings.ContainsKey(key) Then
            Dim value As String = settings(key)
            Dim index As Integer = CmbStatusKomputer.FindStringExact(value)
            If index <> -1 Then
                Return index
            Else
                Return 0 ' Default index if value not found
            End If
        Else
            Return 0 ' Default index if key not found
        End If
    End Function


    Private Function FindThermalPrinter() As String
        Dim printerSettings As New PrinterSettings()
        Dim printers As String() = New String(PrinterSettings.InstalledPrinters.Count - 1) {}
        PrinterSettings.InstalledPrinters.CopyTo(printers, 0)

        ' Check if any printer name contains "thermal" or "receipt"
        For Each printer As String In printers
            If printer.ToLower().Contains("thermal") OrElse printer.ToLower().Contains("receipt") Then
                Return printer
            End If
        Next

        ' No thermal printer found
        Return ""
    End Function


    Private Function FindDotMatrixPrinter() As String
        Dim dotMatrixPrinter As String = ""

        ' Ambil daftar printer terinstal
        Dim printerSettings As New PrinterSettings()
        Dim printers As String() = New String(PrinterSettings.InstalledPrinters.Count - 1) {}
        PrinterSettings.InstalledPrinters.CopyTo(printers, 0)

        ' Loop melalui daftar printer dan temukan yang jenisnya dot matrix
        For Each printer As String In printers
            If printer.ToLower().Contains("dot matrix") Then
                dotMatrixPrinter = printer
                Exit For
            End If
        Next

        Return dotMatrixPrinter
    End Function


    Public Sub AmbilNilaiPrinterJualDariIniFile()
        Dim filePath As String = "printer.ini"

        ' Pastikan file ada sebelum membaca
        If File.Exists(filePath) Then
            ' Baca semua baris dari file
            Dim lines As String() = File.ReadAllLines(filePath)

            ' Loop melalui baris-baris ini untuk mencari nilai yang diperlukan
            For Each line As String In lines
                ' Split baris berdasarkan "=" untuk memisahkan kunci dan nilai
                Dim parts As String() = line.Split("="c)
                If parts.Length = 2 Then
                    Dim key As String = parts(0).Trim()
                    Dim value As String = parts(1).Trim()

                    ' Cek jika kunci sesuai dengan yang Anda cari
                    If key = "JenisPrinterJual" Then
                        ' Set nilai ComboBox berdasarkan nilai dari file printer.ini
                        CmbJenisPrinterJual.Text = value
                        Exit For ' Keluar dari loop setelah nilai ditemukan
                    End If
                End If
            Next
        End If
    End Sub



    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpan.Click
        Dim filePath As String = "printer.ini"

        If Not File.Exists(filePath) Then
            ' File doesn't exist, create a new file and write the settings values
            Using writer As New StreamWriter(filePath)
                writer.WriteLine("PrinterPos=" & CmbJenisPrinterThermal.Text)
                writer.WriteLine("PortPrinter=" & CmbPort.Text)
                writer.WriteLine("Maju=" & TxtMAju.Text)
                writer.WriteLine("Mundur=" & TxtMundur.Text)
                writer.WriteLine("Piksel=" & Txtpiksel.Text)
                writer.WriteLine("Panjang=" & TxtPanjang.Text)
                writer.WriteLine("Lebar=" & TxtLebar.Text)
                writer.WriteLine("BatasKiri=" & TxtBatasKiri.Text)
                writer.WriteLine("Jarak=" & TxtJarak.Text)
                writer.WriteLine("PortName=" & lblPortName.Text)
                writer.WriteLine("BaudRate=" & lblBaudRate.Text)
                writer.WriteLine("Parity=" & lblParity.Text)
                writer.WriteLine("DataBits=" & lblDataBits.Text)
                writer.WriteLine("PortCashDraw=" & CmbPortCash.Text)
                writer.WriteLine("CodeCashDraw=" & CmbCodeCash.Text)
                writer.WriteLine("Potongkertas=" & CBPotong.Checked.ToString())
                writer.WriteLine("ModelStruk=" & CmbModelStruk.Text)
                writer.WriteLine("FontNama=" & CmbFNAma.Text)
                writer.WriteLine("FontKet=" & CmbFKet.Text)
                writer.WriteLine("FontIsi=" & CmbFIsi.Text)
                writer.WriteLine("FOntFoot=" & CmbFFoot.Text)
                writer.WriteLine("FontUNama=" & CmbUNama.Text)
                writer.WriteLine("FontUKet=" & CmbUKet.Text)
                writer.WriteLine("FontUIsi=" & CmbUIsi.Text)
                writer.WriteLine("FontUFoot=" & CmbUFoot.Text)
                writer.WriteLine("StatusComp=" & CmbStatusKomputer.Text)
                writer.WriteLine("JenisPrinterJual=" & CmbJenisPrinterJual.Text)
                writer.WriteLine("JenisPrinterLap=" & CmbJenisLap.Text)


                writer.WriteLine("PrinterDot=" & CmbJenisPrinterDot.Text)
                writer.WriteLine("LebarDot=" & TxtLebarKertasDot.Text)
                writer.WriteLine("TinggiDot=" & TxtTinggiDot.Text)
                writer.WriteLine("BatasKiriDot=" & TxtBatasKiriDot.Text)
                writer.WriteLine("JarakBarisDot=" & TxtJarakBarisDot.Text)
                writer.WriteLine("FontJudulDot=" & CmbFontJuduDot.Text)
                writer.WriteLine("FontIsiDot=" & CmbFontIsiDot.Text)
                writer.WriteLine("UkuranFontJudul=" & CmbUkuranJuduDot.Text)
                writer.WriteLine("UkuranFontIsi=" & CmbUkuranIsiDot.Text)

            End Using
        Else
            ' File already exists, overwrite the settings values in the file
            Dim lines As New List(Of String)(File.ReadAllLines(filePath))

            ' Update the settings values in the list
            UpdateValueInList(lines, "PrinterPos", CmbJenisPrinterThermal.Text)
            UpdateValueInList(lines, "PortPrinter", CmbPort.Text)
            UpdateValueInList(lines, "Maju", TxtMAju.Text)
            UpdateValueInList(lines, "Mundur", TxtMundur.Text)
            UpdateValueInList(lines, "Piksel", Txtpiksel.Text)
            UpdateValueInList(lines, "Panjang", TxtPanjang.Text)
            UpdateValueInList(lines, "Lebar", TxtLebar.Text)
            UpdateValueInList(lines, "BatasKiri", TxtBatasKiri.Text)
            UpdateValueInList(lines, "Jarak", TxtJarak.Text)
            UpdateValueInList(lines, "PortName", lblPortName.Text)
            UpdateValueInList(lines, "BaudRate", lblBaudRate.Text)
            UpdateValueInList(lines, "Parity", lblParity.Text)
            UpdateValueInList(lines, "DataBits", lblDataBits.Text)
            UpdateValueInList(lines, "PortCashDraw", CmbPortCash.Text)
            UpdateValueInList(lines, "CodeCashDraw", CmbCodeCash.Text)
            UpdateValueInList(lines, "Potongkertas", CBPotong.Checked.ToString())
            UpdateValueInList(lines, "ModelStruk", CmbModelStruk.Text)
            UpdateValueInList(lines, "FontNama", CmbFNAma.Text)
            UpdateValueInList(lines, "FontKet", CmbFKet.Text)
            UpdateValueInList(lines, "FontIsi", CmbFIsi.Text)
            UpdateValueInList(lines, "FOntFoot", CmbFFoot.Text)
            UpdateValueInList(lines, "FontUNama", CmbUNama.Text)
            UpdateValueInList(lines, "FontUKet", CmbUKet.Text)
            UpdateValueInList(lines, "FontUIsi", CmbUIsi.Text)
            UpdateValueInList(lines, "FontUFoot", CmbUFoot.Text)
            UpdateValueInList(lines, "StatusComp", CmbStatusKomputer.Text)
            UpdateValueInList(lines, "JenisPrinterJual", CmbJenisPrinterJual.Text)
            UpdateValueInList(lines, "JenisPrinterLap", CmbJenisLap.Text)

            UpdateValueInList(lines, "PrinterDot", CmbJenisPrinterDot.Text)
            UpdateValueInList(lines, "LebarDot", TxtLebarKertasDot.Text)
            UpdateValueInList(lines, "TinggiDot", TxtTinggiDot.Text)
            UpdateValueInList(lines, "BatasKiriDot", TxtBatasKiriDot.Text)
            UpdateValueInList(lines, "JarakBarisDot", TxtJarakBarisDot.Text)
            UpdateValueInList(lines, "FontJudulDot", CmbFontJuduDot.Text)
            UpdateValueInList(lines, "FontIsiDot", CmbFontIsiDot.Text)
            UpdateValueInList(lines, "UkuranFontJudul", CmbUkuranJuduDot.Text)
            UpdateValueInList(lines, "UkuranFontIsi", CmbUkuranIsiDot.Text)
            ' Write the updated list back to the file
            File.WriteAllLines(filePath, lines)
        End If
        FormUtama.AmbilKomputer()
        MsgBox("Berhasil disimpan", , "Sukses")
    End Sub

    Private Sub BtnRestore_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnRestore.Click

        ' Find an available thermal printer
        Dim thermalPrinter As String = FindThermalPrinter()
        If Not String.IsNullOrEmpty(thermalPrinter) Then
            CmbJenisPrinterThermal.Text = thermalPrinter
        Else
            CmbJenisPrinterThermal.SelectedIndex = 0
        End If

        ' Set the default values in the form controls
        LblPrinterStruk.Text = CmbJenisPrinterThermal.Text

        CmbPort.Text = "COM1"
        TxtMAju.Text = "0"
        TxtMundur.Text = "0"
        TxtPanjang.Text = "0"
        TxtLebar.Text = "80"
        Txtpiksel.Text = "100"
        TxtBatasKiri.Text = "0"
        TxtJarak.Text = "2"
        CBPotong.Checked = True
        CmbModelStruk.Text = "Model 1 Lengkap"
        CmbFNAma.Text = "Century"
        CmbFKet.Text = "Arial Narrow"
        CmbFIsi.Text = "Arial Narrow"
        CmbFFoot.Text = "Arial Narrow"
        CmbUNama.Text = "14"
        CmbUKet.Text = "10"
        CmbUIsi.Text = "10"
        CmbUFoot.Text = "10"




        Dim DotMatrikPrinter As String = FindDotMatrixPrinter()
        If Not String.IsNullOrEmpty(DotMatrikPrinter) Then
            CmbJenisPrinterDot.Text = DotMatrikPrinter
        Else
            CmbJenisPrinterDot.Text = LblPrinter.Text
        End If
        LblPrinterTersimpanDot.Text = CmbJenisPrinterDot.Text

        TxtLebarKertasDot.Text = "27"
        TxtTinggiDot.Text = "7"
        TxtBatasKiriDot.Text = "0"
        TxtJarakBarisDot.Text = "2"
        CmbFontJuduDot.Text = "Consolas"
        CmbFontIsiDot.Text = "Consolas"
        CmbUkuranJuduDot.Text = "12"
        CmbUkuranIsiDot.Text = "9"



    End Sub

    Private Sub UpdateValueInList(lines As List(Of String), key As String, value As String)
        Dim index As Integer = lines.FindIndex(Function(line) line.StartsWith(key & "="))

        If index <> -1 Then
            ' Update the existing line with the new value
            lines(index) = key & "=" & value
        Else
            ' Add a new line with the key and value
            lines.Add(key & "=" & value)
        End If
    End Sub


    Private Sub BtnTes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTes.Click
        Try
            Dim comPort As String = CmbPortCash.Text ' Ganti dengan nama port serial yang sesuai dengan komputer Anda
            Dim baudRate As Integer = 9600 ' Sesuaikan dengan baud rate pada laci kasir Anda
            Dim dataBits As Integer = 8
            Dim stopBits As StopBits = StopBits.One
            Dim parity As Parity = Parity.None

            Using port As New SerialPort(comPort, baudRate, parity, dataBits, stopBits)
                port.Open()

                If CmbCodeCash.Text = "OPTION 1" Then
                    port.Write(Chr(&H1B) & Chr(&H70) & Chr(&H0) & Chr(&H50) & Chr(&H50)) ' Perintah untuk membuka laci kasir
                ElseIf CmbCodeCash.Text = "OPTION 2" Then
                    Dim openDrawer As Byte() = {&H1B, &H70, &H0, &H50, &H50} ' Perintah untuk membuka laci kasir
                    port.Write(openDrawer, 0, openDrawer.Length)
                End If
            End Using

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Peringatan")
        End Try
    End Sub


    Private Sub CmbCodeCash_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbCodeCash.SelectedIndexChanged
        If CmbPortCash.Text = "" Then
            MsgBox("Port serial belum dipilih", vbCritical, "Peringatan")
        End If
    End Sub

    Private Sub TextBox_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TxtMAju.MouseMove, TxtMundur.MouseMove, TxtPanjang.MouseMove, TxtLebar.MouseMove, Txtpiksel.MouseMove, TxtBatasKiri.MouseMove, TxtJarak.MouseMove
        Dim control As TextBox = DirectCast(sender, TextBox)

        Select Case control.Name
            Case "TxtMAju"
                ToolTip1.SetToolTip(control, "Untuk mengurangi panjang kertas kosong sebelum di cetak")
            Case "TxtMundur"
                ToolTip1.SetToolTip(control, "Untuk mengurangi panjang kertas kosong setelah di cetak")
            Case "TxtPanjang"
                ToolTip1.SetToolTip(control, "Untuk menambah panjang kertas saat di cetak")
            Case "TxtLebar"
                ToolTip1.SetToolTip(control, "Sesuaikan lebar kertas thermal, jika pakai thermal pos 80 isi dengan angka 75,jika pakai thermal pos 58 isi dengan angka 51 ")
            Case "Txtpiksel"
                ToolTip1.SetToolTip(control, "Diisi piksel printer, nilai default program adalah : 100")
            Case "TxtBatasKiri"
                ToolTip1.SetToolTip(control, "Menambah batas kiri sebelum cetak")
            Case "TxtJarak"
                ToolTip1.SetToolTip(control, "Menambah jarak antar tulisan")
        End Select
    End Sub


    Private Sub CBPotong_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBPotong.CheckedChanged
        If CBPotong.Checked = True Then
            isChecked = CBPotong.Checked
        End If
    End Sub


    Private Sub CmbJenisPrinterJual_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJenisPrinterJual.SelectedIndexChanged
        If CmbJenisPrinterJual.SelectedIndex = 0 Then
            PanelThermal.Visible = True
            PanelDotMatrik.Visible = False
            BtnRestore.Visible = True
        ElseIf CmbJenisPrinterJual.SelectedIndex = 1 Then
            PanelThermal.Visible = False
            PanelDotMatrik.Visible = True
            BtnRestore.Visible = True
        Else
            PanelThermal.Visible = False
            PanelDotMatrik.Visible = False
            BtnRestore.Visible = False
        End If
    End Sub
End Class