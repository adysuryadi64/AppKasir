Imports System.Drawing.Printing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Public Class CetakLabelBarcodeTSPL
    Inherits Form

#Region "CLASS HELPER INI (Config Manager - FIXED VERSION)"
    Public Class SimpleIni
        Private ReadOnly _filePath As String

        Public Sub New(path As String)
            _filePath = path

            ' PERBAIKAN: Memanggil GetDirectoryName dari kelas IO.Path, bukan dari _filePath
            Dim folderPath As String = IO.Path.GetDirectoryName(_filePath)

            ' Cek apakah folder spesifik ada (jika path relatif, folderPath bisa kosong)
            If Not String.IsNullOrEmpty(folderPath) AndAlso Not IO.Directory.Exists(folderPath) Then
                IO.Directory.CreateDirectory(folderPath)
            End If

            If Not IO.File.Exists(_filePath) Then
                ' Buat file baru dengan header jika belum ada
                IO.File.WriteAllText(_filePath, "; Config Printer TSPL v3.0" & vbCrLf, Encoding.UTF8)
            End If
        End Sub


        ''' <summary>
        ''' Membaca nilai dari Key. Toleran terhadap spasi di sekitar tanda =.
        ''' </summary>
        Public Function ReadValue(section As String, key As String, defaultVal As String) As String
            Try
                ' Cek apakah file ada untuk mencegah exception
                If Not File.Exists(_filePath) Then Return defaultVal

                Dim lines = File.ReadAllLines(_filePath, Encoding.UTF8)
                Dim inSection As Boolean = False

                For Each line In lines
                    Dim t = line.Trim()

                    ' 1. Cari Section
                    If t.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase) Then
                        inSection = True
                        Continue For
                    End If

                    ' 2. Jika sudah ketemu section baru lain, berhenti mencari
                    If inSection AndAlso t.StartsWith("[") Then Exit For

                    ' 3. Cari Key (FIXED LOGIC)
                    ' Kita cek apakah baris mengandung '=' dulu, baru pecah
                    If inSection AndAlso t.Contains("=") Then
                        ' Pecah hanya di tanda = pertama saja (Split 2)
                        Dim parts() As String = t.Split({"="c}, 2)

                        ' Bandingkan bagian kiri (key) dengan key yang dicari (Trim spasi)
                        If parts(0).Trim().Equals(key, StringComparison.OrdinalIgnoreCase) Then
                            ' Kembalikan bagian kanan (value)
                            Return parts(1).Trim()
                        End If
                    End If
                Next
            Catch ex As Exception
                ' Log error ke debug window agar developer tahu
                Debug.WriteLine("INI Read Error: " & ex.Message)
            End Try
            ' Jika tidak ketemu atau error, kembalikan default
            Return defaultVal
        End Function

        Public Sub WriteValue(section As String, key As String, value As String)
            Try
                If Not File.Exists(_filePath) Then
                    File.WriteAllText(_filePath, "", Encoding.UTF8)
                End If

                Dim lines = File.ReadAllLines(_filePath, Encoding.UTF8).ToList()
                Dim sectionIndex = lines.FindIndex(Function(l) l.Trim().Equals($"[{section}]", StringComparison.OrdinalIgnoreCase))

                ' Jika Section belum ada, buat baru
                If sectionIndex = -1 Then
                    ' Tambah baris kosong jika file tidak kosong
                    If lines.Count > 0 Then lines.Add("")
                    lines.Add($"[{section}]")
                    lines.Add($"{key}={value}") ' Tulis tanpa spasi agar konsisten
                    File.WriteAllLines(_filePath, lines, Encoding.UTF8)
                    Exit Sub
                End If

                ' Jika Section sudah ada, cari Key
                Dim i = sectionIndex + 1
                Dim keyFound As Boolean = False
                Dim keyInsertIndex As Integer = -1

                While i < lines.Count AndAlso Not lines(i).Trim().StartsWith("[")
                    Dim t = lines(i).Trim()

                    ' Gunakan logika parsing yang sama dengan ReadValue untuk update
                    If t.Contains("=") Then
                        Dim parts() As String = t.Split({"="c}, 2)
                        If parts(0).Trim().Equals(key, StringComparison.OrdinalIgnoreCase) Then
                            ' Update Key yang ada
                            lines(i) = $"{key}={value}"
                            keyFound = True
                            Exit While
                        End If
                    End If
                    i += 1
                End While

                If Not keyFound Then
                    ' Jika Key tidak ditemukan dalam loop, insert di posisi terakhir loop berhenti
                    ' (Atau di akhir section jika loop selesai karena habis baris)
                    lines.Insert(i, $"{key}={value}")
                End If

                File.WriteAllLines(_filePath, lines, Encoding.UTF8)

            Catch ex As Exception
                Debug.WriteLine("INI Write Error: " & ex.Message)
                MessageBox.Show("Gagal menyimpan config: " & ex.Message)
            End Try
        End Sub
    End Class
#End Region


#Region "KONSTANTA (VARIABLE TETAP)"
    Private Const INT_DPI As Integer = 203
    Private Const INT_DOTS_PER_MM As Integer = 8
    Private Const INT_ROLL_WIDTH_MM As Integer = 110
    Private Const STR_CONFIG_FILENAME As String = "config_printer.ini"

    ' KONSTANTA MINIMAL (Untuk mencegah overlap)
    Private Const INT_MIN_MARGIN_DOTS As Integer = 8    ' 1mm
    Private Const INT_MIN_GAP_DOTS As Integer = 4       ' 0.5mm
    Private Const INT_MIN_BARCODE_DOTS As Integer = 16  ' 2mm

    ' ===== PROPERTIES UNTUK DATA DARI FORM LAIN =====
    Public Property NamaBarangDikirim As String = ""
    Public Property KodeBarangDikirim As String = ""
#End Region

#Region "WIN32 API (PRINTER DRIVER)"
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Private Class DocInfoA
        <MarshalAs(UnmanagedType.LPStr)> Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)> Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)> Public pDataType As String
    End Class

    <DllImport("winspool.drv", EntryPoint:="OpenPrinterA", SetLastError:=True)>
    Private Shared Function OpenPrinter(strPrinterName As String, ByRef hPrinter As IntPtr, pDefault As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", SetLastError:=True)>
    Private Shared Function ClosePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", SetLastError:=True)>
    Private Shared Function StartDocPrinter(hPrinter As IntPtr, intLevel As Integer, di As DocInfoA) As Boolean
    End Function

    <DllImport("winspool.drv", SetLastError:=True)>
    Private Shared Function EndDocPrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", SetLastError:=True)>
    Private Shared Function StartPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", SetLastError:=True)>
    Private Shared Function EndPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", SetLastError:=True)>
    Private Shared Function WritePrinter(hPrinter As IntPtr, pBytes As IntPtr, intCount As Integer, ByRef intWritten As Integer) As Boolean
    End Function
#End Region

#Region "MODEL DATA (BARANG)"

    ''' <summary>
    ''' Model sederhana untuk menampung daftar produk (misal untuk AutoComplete/ComboBox)
    ''' </summary>
    Private Class ProductInfo
        Public Property Name As String
        Public Property Barcode As String
        Public Property Price As Decimal
        Public Property Unit As String

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' Dictionary Sementara untuk menyimpan detail lengkap produk yang sedang diproses.
    ''' Menangani data Multi-Satuan (Kecil, Sedang, Besar).
    ''' </summary>
    Private dicProductDetail As New Dictionary(Of String, Object)

    ''' <summary>
    ''' Mengambil nilai String dari Dictionary Produk
    ''' </summary>
    Private Function GetDictValue(key As String) As String
        If dicProductDetail.ContainsKey(key) AndAlso dicProductDetail(key) IsNot Nothing Then
            Return dicProductDetail(key).ToString()
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Mengambil nilai Decimal (Angka) dari Dictionary Produk
    ''' </summary>
    Private Function GetDictDecimal(key As String) As Decimal
        If dicProductDetail.ContainsKey(key) AndAlso IsNumeric(dicProductDetail(key)) Then
            Return Convert.ToDecimal(dicProductDetail(key))
        End If
        Return 0D
    End Function

    ''' <summary>
    ''' Event: Saat user mengganti satuan di ComboBox
    ''' </summary>
    Private Sub CmbPilihSatuanBarang_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles CmbPilihSatuanBarang.SelectedIndexChanged

        UpdateSatuanSelection()

    End Sub

    ''' <summary>
    ''' Logika Update Harga dan Barcode berdasarkan satuan yang dipilih
    ''' </summary>
    Private Sub UpdateSatuanSelection()

        ' Pastikan ada data dan user sudah memilih item
        If dicProductDetail Is Nothing OrElse dicProductDetail.Count = 0 Then Exit Sub
        If CmbPilihSatuanBarang.SelectedItem Is Nothing Then Exit Sub

        Dim selectedUnit As String =
            CmbPilihSatuanBarang.SelectedItem.ToString()

        ' Tentukan Key mana yang akan diambil berdasarkan Satuan
        Select Case selectedUnit
            Case GetDictValue("SatuanKecil")
                SetHargaDanBarcode("HargaKecil", "BarcodeKecil")

            Case GetDictValue("SatuanSedang")
                SetHargaDanBarcode("HargaSedang", "BarcodeSedang")

            Case GetDictValue("SatuanBesar")
                SetHargaDanBarcode("HargaBesar", "BarcodeBesar")
        End Select

    End Sub

    ''' <summary>
    ''' Menampilkan Harga dan Barcode ke TextBox
    ''' </summary>
    Private Sub SetHargaDanBarcode(
        priceKey As String,
        barcodeKey As String
    )

        Dim priceValue As Decimal = GetDictDecimal(priceKey)
        TxtInputHargaBarang.Text = priceValue.ToString("N0") ' Format ribuan

        TxtKodeBarcodeInput.Text = GetDictValue(barcodeKey)

    End Sub

    ''' <summary>
    ''' Mengambil data lengkap barang dari Database dan mengisi Dictionary dan UI
    ''' </summary>
    Private Sub AmbildataLengkapBarang(ByVal searchText As String)

        Try
            If String.IsNullOrWhiteSpace(searchText) Then
                MessageBox.Show(
                    "Nama barang tidak boleh kosong!",
                    "Validasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                Exit Sub
            End If

            Dim sqlQuery As String =
            "SELECT ID_BARANG, NAMA_BARANG,
                    BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR,
                    SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR,
                    HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR
             FROM tbl_barang
             WHERE TRIM(NAMA_BARANG) = @Pencarian
                OR TRIM(ID_BARANG) = @Pencarian
             LIMIT 1"

            Using cmd As New MySqlCommand(sqlQuery, conn)
                cmd.Parameters.AddWithValue("@Pencarian", searchText.Trim())

                Using reader = cmd.ExecuteReader()

                    If Not reader.Read() Then
                        MessageBox.Show(
                            "Data barang tidak ditemukan!",
                            "Info",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        )
                        TxtKodeBarcodeInput.Focus()
                        Exit Sub
                    End If

                    ' === 1. DATA DASAR ===
                    TxtInputNamaBarang.Text = reader("NAMA_BARANG").ToString()

                    Dim unitSmall As String = reader("SATUAN_UMUM_KECIL").ToString()
                    Dim unitMedium As String = reader("SATUAN_UMUM_SEDANG").ToString()
                    Dim unitLarge As String = reader("SATUAN_UMUM_BESAR").ToString()

                    ' === 2. RESET & ISI COMBO SATUAN ===
                    CmbPilihSatuanBarang.Items.Clear()
                    CmbPilihSatuanBarang.SelectedIndex = -1

                    ' Hanya tambahkan satuan jika tidak kosong
                    If unitSmall <> "" Then CmbPilihSatuanBarang.Items.Add(unitSmall)
                    If unitMedium <> "" Then CmbPilihSatuanBarang.Items.Add(unitMedium)
                    If unitLarge <> "" Then CmbPilihSatuanBarang.Items.Add(unitLarge)

                    ' === 3. SIMPAN KE DICTIONARY (dicProductDetail) ===
                    dicProductDetail.Clear()

                    ' Mapping Data dari Reader ke Key Dictionary
                    dicProductDetail("HargaKecil") = If(IsDBNull(reader(8)), 0D, reader.GetDecimal(8))
                    dicProductDetail("HargaSedang") = If(IsDBNull(reader(9)), 0D, reader.GetDecimal(9))
                    dicProductDetail("HargaBesar") = If(IsDBNull(reader(10)), 0D, reader.GetDecimal(10))

                    dicProductDetail("BarcodeKecil") = reader("BARCODE_KECIL").ToString()
                    dicProductDetail("BarcodeSedang") = reader("BARCODE_SEDANG").ToString()
                    dicProductDetail("BarcodeBesar") = reader("BARCODE_BESAR").ToString()

                    dicProductDetail("SatuanKecil") = unitSmall
                    dicProductDetail("SatuanSedang") = unitMedium
                    dicProductDetail("SatuanBesar") = unitLarge

                    ' === 4. SET DEFAULT (Pilih satuan pertama) ===
                    If CmbPilihSatuanBarang.Items.Count > 0 Then
                        CmbPilihSatuanBarang.SelectedIndex = 0
                        UpdateSatuanSelection() ' Trigger update UI Harga/Barcode
                    Else
                        ' Jika tidak ada satuan
                        TxtInputHargaBarang.Text = "0"
                        TxtKodeBarcodeInput.Text = ""
                    End If

                    ' Fokus ke input jumlah cetak
                    If TxtJumlahLabelDicetak IsNot Nothing Then TxtJumlahLabelDicetak.Focus()
                    If TxtJumlahLabelDicetak IsNot Nothing Then TxtJumlahLabelDicetak.SelectAll()

                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(
                "Error ambil data barang: " & ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        End Try

    End Sub

#End Region


#Region "VARIABLE GLOBAL"
    Private objIniConfig As SimpleIni
    Private bolSuppressEvents As Boolean = False  ' Flag untuk mencegah event loop
#End Region

#Region "LOGGING SYSTEM"
    Private Sub AddLog(strMessage As String, Optional strType As String = "INFO")
        Try
            If RTLog Is Nothing Then Exit Sub

            Dim ts As String = DateTime.Now.ToString("HH:mm:ss")
            Dim msg As String = $"[{ts}] [{strType}] {strMessage}" & Environment.NewLine

            RTLog.SelectionStart = RTLog.TextLength
            RTLog.SelectionLength = 0

            Select Case strType.ToUpper()
                Case "ERROR"
                    RTLog.SelectionColor = Color.Red
                Case "WARN"
                    RTLog.SelectionColor = Color.Orange
                Case "SUCCESS"
                    RTLog.SelectionColor = Color.Green
                Case "CALC"
                    RTLog.SelectionColor = Color.Blue
                Case Else
                    RTLog.SelectionColor = Color.Black
            End Select

            RTLog.AppendText(msg)
            RTLog.SelectionColor = RTLog.ForeColor
            RTLog.ScrollToCaret()

        Catch ex As Exception
            Debug.WriteLine("Error Logging: " & ex.Message)
        End Try
    End Sub
#End Region

#Region "DYNAMIC CALCULATION ENGINE (100% MATEMATIS)"


    Private Function GetTsplFontHeightDots(fontSize As Integer) As Integer
        Select Case fontSize
            Case 1 : Return 24
            Case 2 : Return 32
            Case 3 : Return 48
            Case 4 : Return 64
            Case 5 : Return 80
            Case Else : Return 32
        End Select
    End Function



    ''' <summary>
    ''' Menghitung tinggi block yang dibutuhkan untuk text dengan word wrap
    ''' </summary>
    Private Function CalculateRequiredBlockHeight(
    strText As String,
    intFontSize As Integer,
    intWidthDots As Integer) As Integer

        AddLog($"[BLOCK] ===== HITUNG BLOCK NAMA =====", "CALC")
        AddLog($"[BLOCK] FontSize={intFontSize} | Width={intWidthDots} dots", "CALC")
        AddLog($"[BLOCK] Text='{strText}' | Len={If(strText Is Nothing, 0, strText.Length)}", "CALC")

        If String.IsNullOrEmpty(strText) Then
            Dim h As Integer = GetTsplFontHeightDots(intFontSize)
            AddLog($"[BLOCK] Text kosong → Height={h}", "CALC")
            Return h
        End If

        Dim fontHeight As Integer = GetTsplFontHeightDots(intFontSize)
        Dim charWidth As Integer = fontHeight \ 2

        AddLog($"[BLOCK] FontHeight={fontHeight} | EstCharWidth={charWidth}", "CALC")

        Dim maxCharsPerLine As Integer =
        Math.Max(intWidthDots \ charWidth, 1)

        Dim totalLines As Integer =
        CInt(Math.Ceiling(strText.Length / CDbl(maxCharsPerLine)))

        AddLog($"[BLOCK] MaxChar/Line={maxCharsPerLine}", "CALC")
        AddLog($"[BLOCK] TotalLines={totalLines}", "CALC")

        Dim totalHeight As Integer = totalLines * fontHeight
        AddLog($"[BLOCK] TotalHeight={totalHeight} dots", "CALC")

        Return totalHeight
    End Function


    ''' <summary>
    ''' Menghitung maksimal barcode height yang diperbolehkan untuk label saat ini
    ''' </summary>
    Private Function CalculateMaxAllowedBarcodeHeight() As Integer
        Try
            Dim labelHeight As Integer =
            CInt(nudLabelHeightMM.Value) * INT_DOTS_PER_MM

            Dim marginBottom As Integer =
            CInt(nudMarginBottomMM.Value) * INT_DOTS_PER_MM

            Dim widthAvailable As Integer =
            (CInt(nudLabelWidthMM.Value) * INT_DOTS_PER_MM) -
            (2 * CInt(nudMarginLeftMM.Value) * INT_DOTS_PER_MM)

            ' --- AMBIL DATA DARI TEXTBOX ---
            Dim strProductName As String = ""
            If TxtInputNamaBarang IsNot Nothing Then
                strProductName = TxtInputNamaBarang.Text
            End If
            ' ---------------------------------

            Dim nameHeight As Integer =
            CalculateRequiredBlockHeight(
                strProductName, ' Menggunakan string dari textbox
                CInt(nudFontSizeName.Value),
                widthAvailable)


            Dim gapNameBarcode As Integer =
            CInt(nudGapNameBcMM.Value) * INT_DOTS_PER_MM

            Dim priceHeight As Integer =
            GetTsplFontHeightDots(CInt(nudFontSizePrice.Value))

            Dim fixedTotal As Integer =
            nameHeight + gapNameBarcode + priceHeight + marginBottom

            Dim remaining As Integer =
            labelHeight - fixedTotal

            Return Math.Max(remaining, INT_MIN_BARCODE_DOTS)

        Catch
            Return INT_MIN_BARCODE_DOTS
        End Try
    End Function

    ''' <summary>
    ''' Update maksimal barcode height di UI berdasarkan perhitungan
    ''' </summary>
    Private Sub UpdateBarcodeHeightConstraints()
        If bolSuppressEvents OrElse nudBarcodeHeightMM Is Nothing Then Exit Sub

        bolSuppressEvents = True
        Try
            Dim maxDots As Integer = CalculateMaxAllowedBarcodeHeight()
            Dim maxMM As Integer =
            Math.Max(1, CInt(Math.Floor(maxDots / INT_DOTS_PER_MM)))

            nudBarcodeHeightMM.Maximum = maxMM

            If nudBarcodeHeightMM.Value > maxMM Then
                nudBarcodeHeightMM.Value = maxMM
            End If

        Finally
            bolSuppressEvents = False
        End Try
    End Sub


#End Region

#Region "FORM EVENT & LOGIC SETUP"
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' 1. KUNCI EVENT (Mencegah penyimpanan otomatis yang tidak diinginkan)
            bolSuppressEvents = True

            txtShopName.Text = NAMA_PERUSAHAAN

            AddLog("Aplikasi dimulai...", "SYSTEM")
            AddLog("Versi: 3.0 - Dynamic Calculation Engine", "SYSTEM")

            Dim strConfigPath As String = Path.Combine(Application.StartupPath, STR_CONFIG_FILENAME)
            objIniConfig = New SimpleIni(strConfigPath)
            AddLog("File Config: " & strConfigPath)

            ' 2. Urutan Inisialisasi yang Aman
            InitializeUIControls()     ' Set Default UI
            LoadAvailablePrinters()    ' Load Printer Driver

            ' 3. LOAD PENGATURAN DARI FILE INI (Menimpa default)
            LoadSettingsFromConfig()

            ' 4. === PERBAIKAN: CEK DATA PANGGILAN DARI FORM LAIN ===
            ' Jika properti terisi, berarti form dipanggil untuk cetak barang tertentu
            If Not String.IsNullOrEmpty(Me.NamaBarangDikirim) OrElse Not String.IsNullOrEmpty(Me.KodeBarangDikirim) Then

                ' Prioritaskan pencarian menggunakan Kode Barang (ID) karena lebih unik
                Dim cariData As String = If(String.IsNullOrEmpty(Me.KodeBarangDikirim), Me.NamaBarangDikirim, Me.KodeBarangDikirim)

                AddLog($"Menerima data eksternal: {cariData}", "INFO")

                ' Pastikan koneksi database (conn) terbuka sebelum query
                ' Catatan: Pastikan variabel 'conn' (MySqlConnection) sudah accessible di sini (misal dari Module)
                If conn IsNot Nothing AndAlso conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If

                ' Panggil fungsi untuk ambil data dan isi TextBox/Combo
                AmbildataLengkapBarang(cariData)
            End If
            ' =======================================================

            ' 5. Hitung ulang layout berdasarkan setting DAN DATA barang yang baru masuk
            ' PENTING: Ini harus dipanggil setelah AmbildataLengkapBarang agar panjang nama barang diperhitungkan
            UpdateBarcodeHeightConstraints()

            AddLog("Inisialisasi selesai.", "SYSTEM")
            AddLog("----------------------------------------", "SYSTEM")

        Catch ex As Exception
            AddLog("Gagal inisialisasi: " & ex.Message, "ERROR")
            MessageBox.Show("Error inisialisasi: " & ex.Message)
        Finally
            ' 6. BUKA KUNCI EVENT
            bolSuppressEvents = False
        End Try
    End Sub
    Private Sub InitializeUIControls()
        bolSuppressEvents = True

        Try
            ' ComboBox
            If cmbBarcodeFormat IsNot Nothing AndAlso cmbBarcodeFormat.Items.Count = 0 Then
                cmbBarcodeFormat.Items.AddRange({"CODE_128", "CODE_39", "EAN_13", "QR_CODE"})
                cmbBarcodeFormat.SelectedIndex = 0
            End If

            If cmbPrintColumns IsNot Nothing AndAlso cmbPrintColumns.Items.Count = 0 Then
                cmbPrintColumns.Items.AddRange({1, 2, 3, 4})
                cmbPrintColumns.SelectedIndex = 1  ' Default 2 kolom
            End If

            If cmbBarcodeRotation IsNot Nothing AndAlso cmbBarcodeRotation.Items.Count = 0 Then
                cmbBarcodeRotation.Items.AddRange({0, 90, 180, 270})
                cmbBarcodeRotation.SelectedIndex = 0
            End If

            ' NumericUpDown - Label Size
            SetNumericRange(nudLabelWidthMM, 10, 150, 33)
            SetNumericRange(nudLabelHeightMM, 10, 150, 20)  ' Default 20mm (lebih aman)

            ' Gap & Margin
            SetNumericRange(nudGapHorizontalMM, 0, 20, 2)
            SetNumericRange(nudGapVerticalMM, 0, 20, 2)
            SetNumericRange(nudMarginLeftMM, 0, 20, 2)
            SetNumericRange(nudMarginTopMM, 0, 20, 0)
            SetNumericRange(nudMarginBottomMM, 0, 20, 0)

            ' Font Sizes
            SetNumericRange(nudFontSizeName, 1, 5, 2)
            SetNumericRange(nudFontSizePrice, 1, 5, 2)
            SetNumericRange(nudFontSizeUnit, 1, 5, 1)
            SetNumericRange(nudFontSizeShop, 1, 5, 1)
            SetNumericRange(nudPriceWidthMult, 1, 2, 1)

            ' Layout Spacing
            SetNumericRange(nudBlockNameHeightMM, 2, 10, 3)
            SetNumericRange(nudGapNameBcMM, 0, 10, 1)

            If nudGapPriceUnitMM IsNot Nothing Then
                SetNumericRange(nudGapPriceUnitMM, 0, 5, 1)
            End If

            ' Barcode - AWALNYA MAX BESAR, AKAN DI-UPDATE NANTI
            SetNumericRange(nudBarcodeHeightMM, 1, 200, 10)  ' Default 10mm
            SetNumericRange(nudBarcodeNarrowRatio, 1, 5, 2)
            SetNumericRange(nudBarcodeWideRatio, 1, 5, 2)

            ' Hardware
            SetNumericRange(nudPrintSpeed, 1, 4, 3)
            SetNumericRange(nudPrintDensity, 1, 15, 8)

            ' Offset Slider
            If trkVerticalOffset IsNot Nothing Then
                trkVerticalOffset.Minimum = -50
                trkVerticalOffset.Maximum = 50
                trkVerticalOffset.Value = 0
            End If

        Finally
            bolSuppressEvents = False
        End Try
    End Sub

    Private Sub SetNumericRange(ctrl As NumericUpDown, intMin As Integer, intMax As Integer, intDefault As Integer)
        If ctrl IsNot Nothing Then
            ctrl.Minimum = intMin
            ctrl.Maximum = intMax
            If ctrl.Value < intMin OrElse ctrl.Value > intMax Then ctrl.Value = intDefault
        End If
    End Sub

    Private Sub LoadAvailablePrinters()
        If cmbSelectPrinter Is Nothing Then Exit Sub
        cmbSelectPrinter.Items.Clear()
        For Each strPrinter As String In PrinterSettings.InstalledPrinters
            cmbSelectPrinter.Items.Add(strPrinter)
        Next
    End Sub

    Private Sub SetNumericSafe(ctrl As NumericUpDown, value As Decimal)
        If ctrl Is Nothing Then Exit Sub
        ctrl.Value = Math.Min(ctrl.Maximum, Math.Max(ctrl.Minimum, value))
    End Sub

    Private Sub SetComboSafeIndex(cmb As ComboBox, index As Integer)
        If cmb Is Nothing OrElse cmb.Items.Count = 0 Then Exit Sub
        If index < 0 OrElse index >= cmb.Items.Count Then
            cmb.SelectedIndex = 0
        Else
            cmb.SelectedIndex = index
        End If
    End Sub

#End Region

#Region "LOAD & SAVE SETTINGS"
    Private Sub LoadSettingsFromConfig()
        bolSuppressEvents = True
        Try
            AddLog("[CONFIG] Load INI...", "CONFIG")

            ' --- PRINTER ---
            Dim printerName As String = objIniConfig.ReadValue("PRINTER", "Name", "")
            If Not String.IsNullOrEmpty(printerName) Then
                Dim idxPrinter As Integer = cmbSelectPrinter.FindStringExact(printerName)
                If idxPrinter >= 0 Then
                    cmbSelectPrinter.SelectedIndex = idxPrinter
                    AddLog($"[CONFIG] Printer OK: {printerName}", "DEBUG")
                Else
                    AddLog($"[CONFIG] Printer '{printerName}' tidak ditemukan di list!", "WARN")
                End If
            End If

            ' --- SIZE ---
            Dim w As Integer = SafeIntVal(objIniConfig.ReadValue("SIZE", "LabelWidth", "33"), 10, 150, 33)
            nudLabelWidthMM.Value = w
            AddLog($"[CONFIG] LabelWidth = {w}", "DEBUG")

            Dim h As Integer = SafeIntVal(objIniConfig.ReadValue("SIZE", "LabelHeight", "20"), 10, 150, 20)
            nudLabelHeightMM.Value = h
            AddLog($"[CONFIG] LabelHeight = {h}", "DEBUG")

            nudGapHorizontalMM.Value = SafeIntVal(objIniConfig.ReadValue("SIZE", "GapX", "2"), 0, 20, 2)
            nudGapVerticalMM.Value = SafeIntVal(objIniConfig.ReadValue("SIZE", "GapY", "2"), 0, 20, 2)

            ' --- OFFSET ---
            nudMarginLeftMM.Value = SafeIntVal(objIniConfig.ReadValue("OFFSET", "Left", "2"), 0, 20, 2)
            nudMarginTopMM.Value = SafeIntVal(objIniConfig.ReadValue("OFFSET", "Top", "1"), 0, 20, 0)
            nudMarginBottomMM.Value = SafeIntVal(objIniConfig.ReadValue("OFFSET", "Bottom", "2"), 0, 20, 2)
            trkVerticalOffset.Value = SafeIntVal(objIniConfig.ReadValue("OFFSET", "Y", "0"), -50, 50, 0)

            ' --- FONT ---
            Dim fsName As Integer = SafeIntVal(objIniConfig.ReadValue("FONT", "NameSize", "2"), 1, 5, 2)
            nudFontSizeName.Value = fsName
            AddLog($"[CONFIG] FontSize Name = {fsName}", "DEBUG") ' Cek ini di Log

            nudFontSizePrice.Value = SafeIntVal(objIniConfig.ReadValue("FONT", "PriceSize", "2"), 1, 5, 2)
            nudFontSizeUnit.Value = SafeIntVal(objIniConfig.ReadValue("FONT", "UnitSize", "1"), 1, 5, 1)
            nudFontSizeShop.Value = SafeIntVal(objIniConfig.ReadValue("FONT", "ShopSize", "1"), 1, 5, 1)

            ' --- LAYOUT ---
            nudGapNameBcMM.Value = SafeIntVal(objIniConfig.ReadValue("LAYOUT", "GapNameBarcode", "1"), 0, 10, 1)
            If nudGapPriceUnitMM IsNot Nothing Then
                nudGapPriceUnitMM.Value = SafeIntVal(objIniConfig.ReadValue("LAYOUT", "GapBarcodePrice", "1"), 0, 10, 1)
            End If

            ' --- BARCODE ---
            Dim bcFmt As String = objIniConfig.ReadValue("BARCODE", "Format", "CODE_128")
            Dim idxFmt As Integer = cmbBarcodeFormat.FindStringExact(bcFmt)
            If idxFmt >= 0 Then cmbBarcodeFormat.SelectedIndex = idxFmt

            Dim bcH As Integer = SafeIntVal(objIniConfig.ReadValue("BARCODE", "Height", "10"), 1, 50, 10)
            AddLog($"[CONFIG] BarcodeHeight (from INI) = {bcH} mm", "DEBUG")
            nudBarcodeHeightMM.Value = bcH

            Dim rotVal As Integer = SafeIntVal(objIniConfig.ReadValue("BARCODE", "Rotation", "0"), 0, 3, 0)
            ' Karena item ComboBox adalah integer (0, 90, dll), kita cari berdasarkan string convert
            Dim idxRot As Integer = cmbBarcodeRotation.FindStringExact(rotVal.ToString())
            If idxRot >= 0 Then cmbBarcodeRotation.SelectedIndex = idxRot

            nudBarcodeNarrowRatio.Value = SafeIntVal(objIniConfig.ReadValue("BARCODE", "Narrow", "2"), 1, 5, 2)
            nudBarcodeWideRatio.Value = SafeIntVal(objIniConfig.ReadValue("BARCODE", "Wide", "2"), 1, 5, 2)

            ' --- PRINT ---
            Dim cols As Integer = SafeIntVal(objIniConfig.ReadValue("PRINT", "Columns", "2"), 1, 4, 2)
            ' Karena item adalah {1, 2, 3, 4}, index 0 adalah 1. Jadi index = cols - 1
            If cols >= 1 AndAlso cols <= 4 Then
                cmbPrintColumns.SelectedIndex = cols - 1
                AddLog($"[CONFIG] Columns = {cols}", "DEBUG")
            End If

            nudPrintSpeed.Value = SafeIntVal(objIniConfig.ReadValue("PRINT", "Speed", "3"), 1, 4, 3)
            nudPrintDensity.Value = SafeIntVal(objIniConfig.ReadValue("PRINT", "Density", "8"), 1, 15, 8)

            chkUseCutter.Checked = Boolean.Parse(objIniConfig.ReadValue("PRINT", "Cutter", "False"))
            chkUsePeel.Checked = Boolean.Parse(objIniConfig.ReadValue("PRINT", "Peel", "False"))

            AddLog("[CONFIG] Load OK", "SUCCESS")
        Catch ex As Exception
            ' TAMBAHKAN INI
            MessageBox.Show("Error Load Config: " & ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
            AddLog("[CONFIG] ERROR: " & ex.Message, "ERROR")
        Finally
            bolSuppressEvents = False
        End Try
    End Sub

    Private Function SaveSettingsToConfig() As Boolean
        If bolSuppressEvents Then Return False

        Try
            objIniConfig.WriteValue("PRINTER", "Name", cmbSelectPrinter.Text)

            objIniConfig.WriteValue("SIZE", "LabelWidth", nudLabelWidthMM.Value.ToString())
            objIniConfig.WriteValue("SIZE", "LabelHeight", nudLabelHeightMM.Value.ToString())
            objIniConfig.WriteValue("SIZE", "GapX", nudGapHorizontalMM.Value.ToString())
            objIniConfig.WriteValue("SIZE", "GapY", nudGapVerticalMM.Value.ToString())

            objIniConfig.WriteValue("OFFSET", "Left", nudMarginLeftMM.Value.ToString())
            objIniConfig.WriteValue("OFFSET", "Top", nudMarginTopMM.Value.ToString())
            objIniConfig.WriteValue("OFFSET", "Bottom", nudMarginBottomMM.Value.ToString())
            objIniConfig.WriteValue("OFFSET", "Y", trkVerticalOffset.Value.ToString())

            objIniConfig.WriteValue("FONT", "NameSize", nudFontSizeName.Value.ToString())
            objIniConfig.WriteValue("FONT", "PriceSize", nudFontSizePrice.Value.ToString())
            objIniConfig.WriteValue("FONT", "UnitSize", nudFontSizeUnit.Value.ToString())
            objIniConfig.WriteValue("FONT", "ShopSize", nudFontSizeShop.Value.ToString())
            objIniConfig.WriteValue("FONT", "PriceWidthMult", nudPriceWidthMult.Value.ToString())

            objIniConfig.WriteValue("LAYOUT", "GapNameBarcode", nudGapNameBcMM.Value.ToString())
            objIniConfig.WriteValue("LAYOUT", "GapBarcodePrice", nudGapPriceUnitMM.Value.ToString())

            objIniConfig.WriteValue("BARCODE", "Format", cmbBarcodeFormat.Text)
            objIniConfig.WriteValue("BARCODE", "Height", nudBarcodeHeightMM.Value.ToString())
            objIniConfig.WriteValue("BARCODE", "Rotation", cmbBarcodeRotation.SelectedIndex.ToString())
            objIniConfig.WriteValue("BARCODE", "Narrow", nudBarcodeNarrowRatio.Value.ToString())
            objIniConfig.WriteValue("BARCODE", "Wide", nudBarcodeWideRatio.Value.ToString())

            If cmbPrintColumns IsNot Nothing AndAlso cmbPrintColumns.SelectedIndex >= 0 Then
                objIniConfig.WriteValue("PRINT", "Columns", (cmbPrintColumns.SelectedIndex + 1).ToString())
            End If

            objIniConfig.WriteValue("PRINT", "Speed", nudPrintSpeed.Value.ToString())
            objIniConfig.WriteValue("PRINT", "Density", nudPrintDensity.Value.ToString())
            objIniConfig.WriteValue("PRINT", "Cutter", chkUseCutter.Checked.ToString())
            objIniConfig.WriteValue("PRINT", "Peel", chkUsePeel.Checked.ToString())

            AddLog("[CONFIG] Save OK", "SUCCESS")
            Return True

        Catch ex As Exception
            AddLog("[CONFIG] Save GAGAL: " & ex.Message, "ERROR")
            Return False
        End Try
    End Function


    Private Sub SetComboIndexSafe(cmb As ComboBox, intIndex As Integer)
        If cmb IsNot Nothing Then
            If intIndex >= 0 AndAlso intIndex < cmb.Items.Count Then
                cmb.SelectedIndex = intIndex
            Else
                cmb.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Function SafeIntVal(strVal As String, intMin As Integer, intMax As Integer, intDefault As Integer) As Integer
        If String.IsNullOrEmpty(strVal) Then Return intDefault
        Dim intTemp As Integer = 0
        If Integer.TryParse(strVal, intTemp) Then
            If intTemp < intMin Then Return intDefault
            If intTemp > intMax Then Return intMax
            Return intTemp
        Else
            Return intDefault
        End If
    End Function

    ' EVENT: Saat setting berubah yang mempengaruhi layout
    Private Sub OnLayoutSettingChanged(sender As Object, e As EventArgs) Handles _
        nudLabelHeightMM.ValueChanged, nudMarginTopMM.ValueChanged, nudMarginBottomMM.ValueChanged,
        nudGapNameBcMM.ValueChanged, nudFontSizeName.ValueChanged, nudFontSizePrice.ValueChanged,
        nudFontSizeShop.ValueChanged, nudBlockNameHeightMM.ValueChanged

        UpdateBarcodeHeightConstraints()
        SaveSettingsToConfig()
    End Sub

    ' EVENT: Saat setting berubah yang TIDAK mempengaruhi layout
    Private Sub OnOtherSettingChanged(sender As Object, e As EventArgs) Handles _
        nudLabelWidthMM.ValueChanged, nudGapHorizontalMM.ValueChanged, nudGapVerticalMM.ValueChanged,
        trkVerticalOffset.Scroll, nudMarginLeftMM.ValueChanged, cmbBarcodeFormat.SelectedIndexChanged,
        nudBarcodeHeightMM.ValueChanged, cmbBarcodeRotation.SelectedIndexChanged,
        nudBarcodeNarrowRatio.ValueChanged, nudBarcodeWideRatio.ValueChanged,
        cmbSelectPrinter.SelectedIndexChanged, nudPrintSpeed.ValueChanged, nudPrintDensity.ValueChanged,
        chkUseCutter.CheckedChanged, chkUsePeel.CheckedChanged, nudFontSizeUnit.ValueChanged,
        nudPriceWidthMult.ValueChanged, cmbPrintColumns.SelectedIndexChanged

        SaveSettingsToConfig()
        If sender Is trkVerticalOffset Then lblVerticalOffsetValue.Text = trkVerticalOffset.Value & " dot"
    End Sub

    ' EVENT: Untuk nudGapPriceUnitMM jika ada
    Private Sub OnGapPriceUnitChanged(sender As Object, e As EventArgs)
        If nudGapPriceUnitMM IsNot Nothing Then
            UpdateBarcodeHeightConstraints()
            SaveSettingsToConfig()
        End If
    End Sub
#End Region

#Region "LAYOUT CALCULATION (100% PERFECT MATH - NO AUTO-FIT!)"
    Private Structure LayoutPosition
        Public X As Integer
        Public Y As Integer
    End Structure

    Private Structure ColumnLayout
        Public PosName As LayoutPosition
        Public PosBarcode As LayoutPosition
        Public PosPrice As LayoutPosition
        Public PosShop As LayoutPosition
        Public BarcodeHeight As Integer
        Public NameBlockHeight As Integer
    End Structure

    Private Function CalculateLayoutPositions(
    intColIndex As Integer,
    intStartXDots As Integer,
    intWidthDots As Integer,
    strProductName As String) As ColumnLayout

        AddLog($"[LAYOUT] ===== KOLOM {intColIndex} =====", "CALC")

        Dim layout As New ColumnLayout()

        Dim contentTopOffset As Integer =
        CInt(nudMarginTopMM.Value) * INT_DOTS_PER_MM

        Dim marginBottom As Integer =
        CInt(nudMarginBottomMM.Value) * INT_DOTS_PER_MM

        Dim marginLeft As Integer =
        CInt(nudMarginLeftMM.Value) * INT_DOTS_PER_MM

        Dim labelHeight As Integer =
        CInt(nudLabelHeightMM.Value) * INT_DOTS_PER_MM

        Dim fontName As Integer = CInt(nudFontSizeName.Value)
        Dim fontPrice As Integer = CInt(nudFontSizePrice.Value)

        Dim widthAvailable As Integer =
        intWidthDots - (2 * marginLeft)

        layout.NameBlockHeight =
        CalculateRequiredBlockHeight(strProductName, fontName, widthAvailable)

        Dim gapNameBarcode As Integer =
        CInt(nudGapNameBcMM.Value) * INT_DOTS_PER_MM

        Dim textPriceHeight As Integer =
        GetTsplFontHeightDots(fontPrice)

        ' === Y POS ===
        layout.PosName.Y = contentTopOffset

        layout.PosBarcode.Y =
        layout.PosName.Y + layout.NameBlockHeight + gapNameBarcode

        layout.BarcodeHeight =
        CInt(nudBarcodeHeightMM.Value) * INT_DOTS_PER_MM

        layout.PosPrice.Y =
        layout.PosBarcode.Y + layout.BarcodeHeight

        Dim requiredHeight As Integer =
        layout.PosPrice.Y + textPriceHeight + marginBottom

        AddLog($"[CHECK] Required={requiredHeight} / Label={labelHeight}", "CALC")

        If requiredHeight > labelHeight Then
            Throw New Exception("LAYOUT OVERFLOW")
        End If

        ' === X POS ===
        Dim leftX As Integer = intStartXDots + marginLeft
        layout.PosName.X = leftX
        layout.PosBarcode.X = leftX
        layout.PosPrice.X = leftX

        AddLog($"[LAYOUT] KOLOM {intColIndex} SELESAI", "CALC")

        Return layout
    End Function

#End Region

#Region "TSPL COMMAND BUILDER - FINAL VERSION (DENGAN FONT DYNAMIS)"

    ''' <summary>
    ''' Fungsi untuk merender Nama Toko menggunakan Font dari UI (nudFontSizeShop)
    ''' </summary>
    Private Sub RenderShopBelowPrice(
        sb As StringBuilder,
        intStartX As Integer,
        intPriceY As Integer,
        intLabelHeightDots As Integer)

        ' 1. Ambil teks nama toko dari kontrol TextBox
        Dim shopText As String = txtShopName.Text
        If String.IsNullOrWhiteSpace(shopText) Then shopText = "TOKO"

        ' 2. AMBIL FONT DARI UI (Tidak hardcode lagi)
        ' Kita ambil nilai dari NumericUpDown nudFontSizeShop yang sudah Anda buat
        Dim fontID As Integer = CInt(nudFontSizeShop.Value)

        ' 3. Hitung estimasi tinggi font dalam satuan dots untuk proteksi layout
        ' Font 1 = 24 dots, Font 2 = 32 dots, dst.
        Dim shopFontHeight As Integer = GetTsplFontHeightDots(fontID)

        ' 4. Tentukan posisi Y (di bawah harga)
        ' Kita beri jarak 30 dots dari posisi Y harga
        Dim shopY As Integer = intPriceY + 30

        ' 5. PROTEKSI OVERFLOW (Agar tidak nasibnya sama seperti log Y=128 vs Label=120)
        ' Jika posisi Y + tinggi font melebihi tinggi label, kita paksa geser ke atas sedikit
        If (shopY + shopFontHeight) > intLabelHeightDots Then
            shopY = intLabelHeightDots - shopFontHeight - 5 ' Beri margin bawah 5 dots
            AddLog($"[SHOP] Posisi Y terlalu bawah, disesuaikan ke {shopY}", "WARN")
        End If

        ' 6. Kirim perintah TEXT ke StringBuilder
        ' Format: TEXT X,Y,"font",rotation,x-multi,y-multi,"content"
        sb.AppendLine($"TEXT {intStartX},{shopY},""{fontID}"",0,1,1,""{shopText}""")

        AddLog($"[SHOP] Render '{shopText}' Font={fontID} di Y={shopY}", "CALC")
    End Sub

    ''' <summary>
    ''' Fungsi Utama Membangun Perintah TSPL
    ''' </summary>
    Private Function BuildTSPLCommand(Optional intColsToPrint As Integer = 1) As String
        ' --- 1. VALIDASI AWAL ---
        ' Jika Nama Barang kosong, tidak perlu melanjutkan proses
        If TxtInputNamaBarang Is Nothing OrElse String.IsNullOrWhiteSpace(TxtInputNamaBarang.Text) Then Return ""

        ' --- 2. AMBIL KONFIGURASI DARI UI ---
        ' Mengambil nilai-nilai MM dan dikonversi ke DOTS (8 dots = 1mm)
        Dim labelWidthMM As Integer = CInt(nudLabelWidthMM.Value)
        Dim labelHeightMM As Integer = CInt(nudLabelHeightMM.Value)
        Dim labelHeightDots As Integer = labelHeightMM * INT_DOTS_PER_MM
        Dim gapHorizontalDots As Integer = CInt(nudGapHorizontalMM.Value) * INT_DOTS_PER_MM
        Dim marginLeftDots As Integer = CInt(nudMarginLeftMM.Value) * INT_DOTS_PER_MM
        Dim labelWidthDots As Integer = labelWidthMM * INT_DOTS_PER_MM

        ' Menentukan jumlah kolom (default 3 jika tidak terpilih)
        Dim jmlKolomSetting As Integer = 3
        If cmbPrintColumns.SelectedItem IsNot Nothing Then
            Integer.TryParse(cmbPrintColumns.SelectedItem.ToString(), jmlKolomSetting)
        End If

        ' Hitung total lebar roll (Lebar label * kolom + gap antar label)
        Dim totalRollWidthMM As Integer = (labelWidthMM * jmlKolomSetting) + (CInt(nudGapHorizontalMM.Value) * (jmlKolomSetting - 1))

        ' Siapkan penampung string perintah
        Dim sb As New StringBuilder()

        ' --- 3. HEADER PRINTER (SETUP KERTAS) ---
        ' Mengatur ukuran fisik label
        sb.AppendLine($"SIZE {totalRollWidthMM} mm,{labelHeightMM} mm")
        ' Mengatur jarak antar label (vertical gap)
        sb.AppendLine($"GAP {nudGapVerticalMM.Value} mm,0")
        ' Arah cetak 1,0 (keluar dari depan printer)
        sb.AppendLine("DIRECTION 1,0")
        ' Referensi koordinat di titik 0,0
        sb.AppendLine("REFERENCE 0,0")
        ' Bersihkan buffer sebelum menggambar konten baru
        sb.AppendLine("CLS")

        ' --- 4. LOOPING CETAK PER KOLOM ---
        For col As Integer = 0 To intColsToPrint - 1
            ' Hitung titik X awal untuk setiap kolom label
            Dim startX As Integer = col * (labelWidthDots + gapHorizontalDots)

            ' Hitung posisi layout otomatis (Nama, Barcode, Harga)
            Dim layout As ColumnLayout = CalculateLayoutPositions(col, startX, labelWidthDots, TxtInputNamaBarang.Text.Trim())

            ' Koordinat X setelah ditambah margin kiri
            Dim fixX As Integer = startX + marginLeftDots
            ' Lebar area teks yang tersedia
            Dim contentWidthDots As Integer = labelWidthDots - (2 * marginLeftDots)

            ' A. CETAK NAMA BARANG (Gunakan BLOCK agar teks bisa bungkus/wrap)
            sb.AppendLine($"BLOCK {fixX},{layout.PosName.Y},{contentWidthDots},{layout.NameBlockHeight},""{nudFontSizeName.Value}"",0,1,1,""{TxtInputNamaBarang.Text.Trim()}""")

            ' B. CETAK BARCODE (Jenis Code 128)
            sb.AppendLine($"BARCODE {fixX},{layout.PosBarcode.Y},""128"",{layout.BarcodeHeight},0,0,{nudBarcodeNarrowRatio.Value},{nudBarcodeWideRatio.Value},""{TxtKodeBarcodeInput.Text.Trim()}""")

            ' C. CETAK HARGA (Gunakan Font sesuai nudFontSizePrice)
            Dim strPrice As String = $"Rp {TxtInputHargaBarang.Text.Trim()}"
            sb.AppendLine($"TEXT {fixX},{layout.PosPrice.Y},""{nudFontSizePrice.Value}"",0,1,1,""{strPrice}""")

            ' D. CETAK NAMA TOKO (MENGGUNAKAN nudFontSizeShop DARI FORM)
            ' Memanggil fungsi pembantu yang sudah diperbaiki di atas
            RenderShopBelowPrice(sb, fixX, layout.PosPrice.Y, labelHeightDots)
        Next

        ' --- 5. EKSEKUSI ---
        ' Perintah untuk mencetak (1 set, 1 copy)
        sb.AppendLine("PRINT 1,1")

        Return sb.ToString()
    End Function

#End Region

#Region "PRINTER RAW DATA"
    Private Sub SendRawData(strPrinterName As String, strTSPLCommand As String)
        If String.IsNullOrEmpty(strPrinterName) Then
            AddLog("Printer belum dipilih!", "ERROR")
            MessageBox.Show("Printer belum dipilih!")
            Exit Sub
        End If

        Dim ptrPrinterHandle As IntPtr = IntPtr.Zero
        Dim objDocInfo As New DocInfoA With {.pDocName = "TSPL Job", .pDataType = "RAW"}

        Try
            AddLog($"Membuka printer: {strPrinterName}", "PRINTER")

            If Not OpenPrinter(strPrinterName, ptrPrinterHandle, IntPtr.Zero) Then
                Throw New Exception("Gagal membuka printer.")
            End If

            If Not StartDocPrinter(ptrPrinterHandle, 1, objDocInfo) Then
                Throw New Exception("Gagal StartDoc.")
            End If

            If Not StartPagePrinter(ptrPrinterHandle) Then
                Throw New Exception("Gagal StartPage.")
            End If

            Dim bytData As Byte() = Encoding.GetEncoding(1252).GetBytes(strTSPLCommand & vbCrLf)
            AddLog($"Mengirim {bytData.Length} bytes...", "PRINTER")

            Dim ptrData As IntPtr = Marshal.AllocHGlobal(bytData.Length)
            Marshal.Copy(bytData, 0, ptrData, bytData.Length)

            Dim intWritten As Integer
            If Not WritePrinter(ptrPrinterHandle, ptrData, bytData.Length, intWritten) Then
                Throw New Exception("Gagal WritePrinter.")
            End If

            AddLog($"Berhasil ({intWritten} bytes).", "SUCCESS")
            Marshal.FreeHGlobal(ptrData)

        Catch ex As Exception
            AddLog("ERROR: " & ex.Message, "ERROR")
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            EndPagePrinter(ptrPrinterHandle)
            EndDocPrinter(ptrPrinterHandle)
            If ptrPrinterHandle <> IntPtr.Zero Then ClosePrinter(ptrPrinterHandle)
        End Try
    End Sub
#End Region

#Region "BUTTON EVENTS"

    Private Function ValidateBeforeSave() As Boolean
        If cmbSelectPrinter Is Nothing OrElse cmbSelectPrinter.Text = "" Then
            AddLog("[VALIDATE] Printer belum dipilih", "WARN")
            Return False
        End If

        If nudLabelWidthMM.Value <= 0 Or nudLabelHeightMM.Value <= 0 Then
            AddLog("[VALIDATE] Ukuran label tidak valid", "WARN")
            Return False
        End If

        Return True
    End Function

    ' --- ADDED: Detailed runtime calculation / diagnostic report helper ---
    Private Sub LogCalculationReport(
        intColsToPrint As Integer,
        sampleName As String,
        sampleBarcode As String,
        totalLabels As Integer)

        Try
            AddLog("[REPORT] === Calculation report (runtime) ===", "SYSTEM")

            Dim labelWidthMM As Integer = CInt(nudLabelWidthMM.Value)
            Dim labelHeightMM As Integer = CInt(nudLabelHeightMM.Value)
            Dim labelWidthDots As Integer = labelWidthMM * INT_DOTS_PER_MM
            Dim labelHeightDots As Integer = labelHeightMM * INT_DOTS_PER_MM

            Dim gapHMM As Integer = CInt(nudGapHorizontalMM.Value)
            Dim gapVMM As Integer = CInt(nudGapVerticalMM.Value)
            Dim gapHorizontalDots As Integer = gapHMM * INT_DOTS_PER_MM
            Dim gapVerticalDots As Integer = gapVMM * INT_DOTS_PER_MM

            Dim marginLeftMM As Integer = CInt(nudMarginLeftMM.Value)
            Dim marginTopMM As Integer = CInt(nudMarginTopMM.Value)
            Dim marginBottomMM As Integer = CInt(nudMarginBottomMM.Value)
            Dim marginLeftDots As Integer = marginLeftMM * INT_DOTS_PER_MM
            Dim marginTopDots As Integer = marginTopMM * INT_DOTS_PER_MM
            Dim marginBottomDots As Integer = marginBottomMM * INT_DOTS_PER_MM

            Dim fontNameId As Integer = CInt(nudFontSizeName.Value)
            Dim fontPriceId As Integer = CInt(nudFontSizePrice.Value)
            Dim fontNameHeight As Integer = GetTsplFontHeightDots(fontNameId)
            Dim fontPriceHeight As Integer = GetTsplFontHeightDots(fontPriceId)
            Dim estCharWidth As Integer = fontNameHeight \ 2

            Dim contentWidthDots As Integer = labelWidthDots - (2 * marginLeftDots)
            Dim maxCharsPerLine As Integer = Math.Max(contentWidthDots \ estCharWidth, 1)
            Dim nameLen As Integer = If(sampleName Is Nothing, 0, sampleName.Length)
            Dim nameLines As Integer = CInt(Math.Ceiling(nameLen / CDbl(maxCharsPerLine)))
            Dim nameBlockHeight As Integer = nameLines * fontNameHeight

            Dim gapNameBarcodeDots As Integer = CInt(nudGapNameBcMM.Value) * INT_DOTS_PER_MM
            Dim barcodeHeightMM As Integer = CInt(nudBarcodeHeightMM.Value)
            Dim barcodeHeightDots As Integer = barcodeHeightMM * INT_DOTS_PER_MM

            Dim allowedBarcodeDots As Integer = CalculateMaxAllowedBarcodeHeight()
            Dim allowedBarcodeMM As Integer = Math.Floor(allowedBarcodeDots / INT_DOTS_PER_MM)

            Dim requiredHeight As Integer =
                marginTopDots + nameBlockHeight + gapNameBarcodeDots + barcodeHeightDots + fontPriceHeight + marginBottomDots

            Dim uiColumns As Integer = 3
            If cmbPrintColumns.SelectedItem IsNot Nothing Then Integer.TryParse(cmbPrintColumns.SelectedItem.ToString(), uiColumns)
            Dim totalRollUIColsMM As Integer = (labelWidthMM * uiColumns) + (gapHMM * (uiColumns - 1))
            Dim totalRollRowColsMM As Integer = (labelWidthMM * intColsToPrint) + (gapHMM * (intColsToPrint - 1))

            AddLog($"[REPORT] Label: {labelWidthMM}x{labelHeightMM} mm => {labelWidthDots}x{labelHeightDots} dots", "CALC")
            AddLog($"[REPORT] Gaps: H={gapHMM} mm ({gapHorizontalDots} dots) | V={gapVMM} mm ({gapVerticalDots} dots)", "CALC")
            AddLog($"[REPORT] Margins: Left={marginLeftMM} mm ({marginLeftDots} dots) Top={marginTopMM} mm ({marginTopDots} dots) Bottom={marginBottomMM} mm ({marginBottomDots} dots)", "CALC")
            AddLog($"[REPORT] ContentWidth={contentWidthDots} dots | EstCharWidth={estCharWidth} dots | MaxCharsPerLine={maxCharsPerLine}", "CALC")
            AddLog($"[REPORT] SampleName='{sampleName}' Len={nameLen} -> Lines={nameLines} -> NameBlockHeight={nameBlockHeight} dots ({nameBlockHeight / INT_DOTS_PER_MM} mm)", "CALC")
            AddLog($"[REPORT] FontPriceHeight={fontPriceHeight} dots", "CALC")
            AddLog($"[REPORT] GapName-Barcode={gapNameBarcodeDots} dots", "CALC")
            AddLog($"[REPORT] Barcode requested={barcodeHeightMM} mm ({barcodeHeightDots} dots) | Allowed (calc)={allowedBarcodeMM} mm ({allowedBarcodeDots} dots)", "CALC")
            AddLog($"[REPORT] RequiredHeight(including margins)={requiredHeight} dots ; LabelHeight={labelHeightDots} dots", "CALC")
            If requiredHeight > labelHeightDots Then
                AddLog($"[REPORT] WARNING: RequiredHeight > LabelHeight -> LAYOUT OVERFLOW expected", "WARN")
            Else
                AddLog($"[REPORT] OK: Layout fits within label height", "SUCCESS")
            End If

            AddLog($"[REPORT] UI Columns={uiColumns} => SIZE header would be {totalRollUIColsMM} mm", "CALC")
            AddLog($"[REPORT] Current row Columns={intColsToPrint} => SIZE header should be {totalRollRowColsMM} mm", "CALC")
            AddLog($"[REPORT] Total target labels={totalLabels}", "CALC")

            For col As Integer = 0 To intColsToPrint - 1
                Dim startX As Integer = col * (labelWidthDots + gapHorizontalDots)
                Try
                    Dim layout As ColumnLayout = CalculateLayoutPositions(col, startX, labelWidthDots, sampleName)
                    AddLog($"[REPORT] Col#{col}: PosName=({layout.PosName.X},{layout.PosName.Y}) PosBarcodeY={layout.PosBarcode.Y} PosPriceY={layout.PosPrice.Y} BarcodeHeight={layout.BarcodeHeight} NameBlockHeight={layout.NameBlockHeight}", "CALC")
                Catch ex As Exception
                    AddLog($"[REPORT] Col#{col} layout error: {ex.Message}", "ERROR")
                End Try
            Next

            Try
                Dim tspl As String = BuildTSPLCommand(intColsToPrint)
                Dim preview As String = If(tspl.Length > 512, tspl.Substring(0, 512) & "...(truncated)", tspl)
                AddLog($"[REPORT] TSPL length={tspl.Length} chars. Preview:{Environment.NewLine}{preview}", "DEBUG")
            Catch ex As Exception
                AddLog($"[REPORT] BuildTSPLCommand error: {ex.Message}", "ERROR")
            End Try

            AddLog("[REPORT] === End report ===", "SYSTEM")

        Catch ex As Exception
            AddLog("LogCalculationReport failed: " & ex.Message, "ERROR")
        End Try
    End Sub

    ' --- MODIFIED: call diagnostic report from BtnPrint_Click ---
    Private Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click
        If Not ValidateBeforeSave() Then
            MessageBox.Show("Data belum valid, tidak bisa disimpan.", "Validasi")
            Exit Sub
        End If

        Try
            If cmbSelectPrinter Is Nothing OrElse cmbSelectPrinter.SelectedIndex = -1 Then
                MessageBox.Show("Pilih printer dulu!")
                Exit Sub
            End If

            AddLog("========================================", "PROCESS")
            AddLog("MULAI PROSES CETAK MULTI-KOLOM", "PROCESS")
            SaveSettingsToConfig()

            Dim totalLabelTarget As Integer = 0
            Integer.TryParse(TxtJumlahLabelDicetak.Text.Trim(), totalLabelTarget)

            Dim jmlKolomSetting As Integer = 1
            If cmbPrintColumns.SelectedItem IsNot Nothing Then
                Integer.TryParse(cmbPrintColumns.SelectedItem.ToString(), jmlKolomSetting)
            End If

            ' --- DIAGNOSTIC: log full calculation report for this run ---
            Try
                LogCalculationReport(jmlKolomSetting, If(TxtInputNamaBarang Is Nothing, "", TxtInputNamaBarang.Text.Trim()), If(TxtKodeBarcodeInput Is Nothing, "", TxtKodeBarcodeInput.Text.Trim()), totalLabelTarget)
            Catch ex As Exception
                AddLog("Failed to generate calculation report: " & ex.Message, "WARN")
            End Try

            AddLog($"[INFO] Target: {totalLabelTarget} label | Layout: {jmlKolomSetting} kolom", "INFO")

            Dim finalCommand As New StringBuilder()
            Dim labelTercetak As Integer = 0
            Dim barisKe As Integer = 1

            ' Loop bangun baris demi baris
            While labelTercetak < totalLabelTarget
                Dim sisaLabel As Integer = totalLabelTarget - labelTercetak
                Dim kolomAktif As Integer = If(sisaLabel >= jmlKolomSetting, jmlKolomSetting, sisaLabel)

                AddLog($"[BATCH] Membangun baris ke-{barisKe} ({kolomAktif} kolom aktif)", "DEBUG")

                ' Panggil Build dengan jumlah kolom yang diinginkan
                Dim rowCommand As String = BuildTSPLCommand(kolomAktif)

                If Not String.IsNullOrEmpty(rowCommand) Then
                    finalCommand.Append(rowCommand)
                    labelTercetak += kolomAktif
                    barisKe += 1
                Else
                    AddLog("[ERROR] Gagal memproses perintah TSPL pada baris ini.", "ERROR")
                    Exit While
                End If
            End While

            ' Kirim data utuh
            If finalCommand.Length > 0 Then
                AddLog($"[PRINTER] Mengirim total {finalCommand.Length} bytes ke spooler...", "PRINTER")
                SendRawData(cmbSelectPrinter.Text, finalCommand.ToString())
                AddLog("PROSES TRANSMISI SELESAI", "SUCCESS")
            End If

            AddLog("========================================", "PROCESS")

        Catch ex As Exception
            AddLog($"EXCEPTION: {ex.Message}", "ERROR")
            MessageBox.Show($"Error cetak:{vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub BtnRestoreDefaults_Click(sender As Object, e As EventArgs) Handles BtnRestoreDefaults.Click
        If MessageBox.Show("Reset semua ke default?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        bolSuppressEvents = True
        Try
            AddLog("[CONFIG] Restore defaults dimulai...", "CONFIG")

            ' ===== SIZE =====
            SetNumericSafe(nudLabelWidthMM, 33)
            SetNumericSafe(nudLabelHeightMM, 15)
            SetNumericSafe(nudGapHorizontalMM, 2)
            SetNumericSafe(nudGapVerticalMM, 2)

            ' ===== OFFSET =====
            SetNumericSafe(nudMarginLeftMM, 2)
            SetNumericSafe(nudMarginTopMM, 0)
            SetNumericSafe(nudMarginBottomMM, 2)
            If trkVerticalOffset IsNot Nothing Then trkVerticalOffset.Value = 0
            lblVerticalOffsetValue.Text = "0 dot"

            ' ===== FONT =====
            SetNumericSafe(nudFontSizeName, 1)
            SetNumericSafe(nudFontSizePrice, 1)
            SetNumericSafe(nudFontSizeUnit, 1)
            SetNumericSafe(nudFontSizeShop, 1)
            SetNumericSafe(nudPriceWidthMult, 1)

            ' ===== LAYOUT =====
            SetNumericSafe(nudGapNameBcMM, 1)
            If nudGapPriceUnitMM IsNot Nothing Then SetNumericSafe(nudGapPriceUnitMM, 1)

            ' ===== BARCODE (NON-DINAMIS DULU) =====
            SetComboSafeIndex(cmbBarcodeFormat, 0)     ' CODE_128
            SetComboSafeIndex(cmbBarcodeRotation, 0)   ' 0 derajat
            SetNumericSafe(nudBarcodeNarrowRatio, 2)
            SetNumericSafe(nudBarcodeWideRatio, 2)

            ' ===== PRINT =====
            SetComboSafeIndex(cmbPrintColumns, 1)      ' 2 kolom (index 1)
            SetNumericSafe(nudPrintSpeed, 3)
            SetNumericSafe(nudPrintDensity, 8)
            chkUseCutter.Checked = False
            chkUsePeel.Checked = False

            ' ===== HITUNG ULANG CONSTRAINT DINAMIS =====
            ' PENTING: barcode height tergantung layout → hitung dulu
            UpdateBarcodeHeightConstraints()

            ' ===== BARCODE HEIGHT (DINAMIS, TERAKHIR) =====
            SetNumericSafe(nudBarcodeHeightMM, nudBarcodeHeightMM.Maximum)

            AddLog("[CONFIG] Restore defaults selesai (aman).", "SUCCESS")

        Finally
            bolSuppressEvents = False
            ' Simpan setelah state valid
            SaveSettingsToConfig()
        End Try
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If Not ValidateBeforeSave() Then
            MessageBox.Show("Data belum valid, tidak bisa disimpan.", "Validasi")
            Exit Sub
        End If

        Dim ok As Boolean = SaveSettingsToConfig()

        If ok Then
            AddLog("Pengaturan berhasil disimpan.", "SUCCESS")
            MessageBox.Show(
                "Pengaturan berhasil disimpan.",
                "Sukses",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
        Else
            AddLog("Pengaturan GAGAL disimpan!", "ERROR")
            MessageBox.Show(
                "Pengaturan gagal disimpan." & vbCrLf &
                "Periksa log untuk detail error.",
                "Gagal",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        End If
    End Sub

    Private Sub BtnCalibrateSensor_Click(sender As Object, e As EventArgs) Handles BtnCalibrateSensor.Click
        If cmbSelectPrinter.SelectedIndex = -1 Then Exit Sub

        AddLog("Menjalankan Kalibrasi Master...", "HARDWARE")

        Dim sb As New StringBuilder()
        ' 1. Beritahu printer ukuran label satu kotak saja (BUKAN lebar roll 110mm)
        Dim w As String = nudLabelWidthMM.Value.ToString()
        Dim h As String = nudLabelHeightMM.Value.ToString()
        Dim g As String = nudGapVerticalMM.Value.ToString()

        sb.AppendLine($"SIZE {w} mm,{h} mm")
        sb.AppendLine($"GAP {g} mm,0")

        ' 2. Perintah deteksi celah fisik (Kertas akan maju beberapa label)
        sb.AppendLine("GAPDETECT")

        ' 3. Kembalikan ke posisi awal (HOME)
        sb.AppendLine("HOME")
        sb.AppendLine("CLS")

        SendRawData(cmbSelectPrinter.Text, sb.ToString())
        AddLog("Kalibrasi Selesai. Printer siap di titik 0,0.", "SUCCESS")
    End Sub

    Private Sub BtnResetPosition_Click(sender As Object, e As EventArgs) Handles BtnResetPosition.Click
        If cmbSelectPrinter.SelectedIndex = -1 Then Exit Sub

        AddLog("Mencari Titik Nol (HOME)...", "HARDWARE")

        Dim sb As New StringBuilder()
        ' Set settingan dasar dulu
        sb.AppendLine($"SIZE {nudLabelWidthMM.Value} mm,{nudLabelHeightMM.Value} mm")
        sb.AppendLine($"GAP {nudGapVerticalMM.Value} mm,0")

        ' Tarik kertas ke posisi awal gap terdekat
        sb.AppendLine("HOME")
        sb.AppendLine("CLS")

        SendRawData(cmbSelectPrinter.Text, sb.ToString())
    End Sub

#End Region

End Class